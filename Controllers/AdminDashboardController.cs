using FreelanceManager.Hubs;
using FreelanceManager.Interfaces;
using FreelanceManager.Models;
using FreelanceManager.Repositry;
using FreelanceManager.ViewModels.DashboradVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace FreelanceManager.Controllers
{
    //[Authorize(Roles = "Admin")]

    public class AdminDashboardController : Controller
    {
        private readonly IClientRepo clientRepo;
        private readonly IInvoiceRepo invoiceRepo;
        private readonly IMissionRepo missionRepo;
        private readonly IFreelancerRepo freelancerRepo;
        private readonly IProjectRepo projectRepo;
        private readonly ITimeTrackingRepo timeTrackingRepo;
        UserManager<Freelancer> userManager;
        private readonly IHubContext<ChatAdminHub> hubContext;


        public AdminDashboardController(IInvoiceRepo invoiceRepo ,IHubContext<ChatAdminHub> hubContext,IFreelancerRepo freelancerRepo,IClientRepo clientRepo, IMissionRepo missionRepo, IProjectRepo projectRepo, ITimeTrackingRepo timeTrackingRepo, UserManager<Freelancer> userManager)
        {
            this.invoiceRepo=invoiceRepo;
            this.freelancerRepo = freelancerRepo;
            this.clientRepo = clientRepo;
            this.missionRepo = missionRepo;
            this.projectRepo = projectRepo;
            this.timeTrackingRepo = timeTrackingRepo;
            this.userManager = userManager;
            this.hubContext = hubContext;   
        }


        public async Task<IActionResult> Index()
        {
            var freelancers = await userManager.GetUsersInRoleAsync("Freelancer");

            var dashboardList = new List<DashboardVM>();

            foreach (var user in freelancers)
            {
                var userId = user.Id;

                var projects = projectRepo.GetAll()
                    .Where(p => p.FreelancerId == userId && p.IsDeleted == false)
                    .ToList();

                var missions = missionRepo.GetAll()
                    .Where(m => projects.Select(p => p.Id).Contains(m.ProjectId))
                    .ToList();

                var missionIds = missions.Select(m => m.Id);

                var totalDuration = new TimeSpan(
                     timeTrackingRepo.GetAll()
                    .Where(t => missionIds.Contains(t.MissionId))
                    .Sum(d => d.Duration.Ticks)
                    );

                dashboardList.Add(new DashboardVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    ProjectCount = projects.Count,
                    MissionCount = missions.Count,
                    TodayHours = totalDuration,
                    missionId = missionIds.ToList()
                });
                ViewBag.missionId = missionIds;
                //await hubContext.Clients.All.SendAsync("UserAddedFull", userId);
                ViewBag.ClientsNum = clientRepo.GetAll().Count();
                ViewBag.TasksNum = missionRepo.GetAll().Count();
                ViewBag.ProjectsNum = projectRepo.GetAll().Count();
                ViewBag.PendingInvoices = invoiceRepo.GetAll().Count();

            }
            return View("Index", dashboardList);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string UserId)
        {
            var client = freelancerRepo.GetByIdString(UserId);
            if (client == null)
            {
                return NotFound();
            }
            
                client.IsDeleted = true;
                freelancerRepo.Save();
            await hubContext.Clients.All.SendAsync("FreelancerDeleted", UserId);

            return Json(new { success = true });


        }
    }
}

