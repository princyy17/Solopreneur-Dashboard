using FreelanceManager.Enums;
using FreelanceManager.Hubs;
using FreelanceManager.Interfaces;
using FreelanceManager.Models;
using FreelanceManager.Repositry;
using FreelanceManager.ViewModels.TimeTracking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FreelanceManager.Controllers
{
    [Authorize]
    public class TimeTrackingController : Controller
    {
        private readonly ITimeTrackingRepo timerRepo;
        private readonly IProjectRepo projectRepo;
        private readonly IMissionRepo missionRepo;
        private readonly IHubContext<ChatAdminHub> hubContext;


        public TimeTrackingController(IHubContext<ChatAdminHub> hubContext,ITimeTrackingRepo timerRepo, IMissionRepo missionRepo, IProjectRepo projectRepo) 
        {
            this.projectRepo = projectRepo;
            this.timerRepo = timerRepo;
            this.missionRepo = missionRepo;
            this.hubContext =hubContext;
        }
        public IActionResult Index()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            // get all project IDs associated with the current freelancer
            var freelancerProjectIds = projectRepo.GetAll()
                .Where(p => p.FreelancerId == userId)
                .Select(p => p.Id)
                .ToList();

            // Get missions for these projects
            var missions = missionRepo.GetAll()
                .Where(m => freelancerProjectIds.Contains(m.ProjectId))
                .ToList();

            // Get mission IDs
            var missionIds = missions.Select(m => m.Id).ToList();

            // Get timers for these specific missions
            var timers = timerRepo.GetAll()
                .Where(t => missionIds.Contains(t.MissionId)) 
                .ToList();

            var model = timers.Select(timer => new TimeTrackingVM
            {
                Id = timer.Id,
                Date = timer.Date,
                Duration = timer.Duration,
                MissionId = timer.MissionId
            }).ToList();

            ViewBag.AvailableMissions = missions;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTimeTracking([FromBody] SaveTimeTracking saveTime)
        {
            if (ModelState.IsValid)
            {
                var timeTracking = new TimeTracking
                {
                    Date = DateTime.Now,
                    Duration = saveTime.Duration,
                    MissionId = saveTime.MissionId,
                };
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                // First get all project IDs associated with the current freelancer
                var freelancerProjectIds = projectRepo.GetAll()
                    .Where(p => p.FreelancerId == userId)
                    .Select(p => p.Id)
                    .ToList();

                // Then filter missions by those project IDs
                var missions = missionRepo.GetAll()
                    .Where(m => freelancerProjectIds.Contains(m.ProjectId))
                    .ToList();

                ViewBag.AvailableMissions = missions;

                timerRepo.Add(timeTracking);
                timerRepo.Save();
                var totalTodayHours =new TimeSpan( timerRepo.GetAll()
                .Where(t => t.Mission.Project.FreelancerId == userId && t.Date.Date == DateTime.Today)
                .Sum(t => t.Duration.Ticks));


                await hubContext.Clients.All.SendAsync("DurationAdded", userId, totalTodayHours);

                return Json(new { sucess=true ,Message = ""});
            }
            return Json(new { sucess = false, Message = "Something not coorect happen" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            var record = timerRepo.GetById(id);
            if (record == null)
                return Json(new { success = false, message = "Not found" });

            var mission = missionRepo.GetById(record.MissionId);
            var project = projectRepo.GetById(mission.ProjectId);
            string userId = project.FreelancerId;

            timerRepo.RemoveById(id);
            timerRepo.Save();

            var totalTodayHours =new TimeSpan( timerRepo.GetAll()
                .Where(t => t.Mission.Project.FreelancerId == userId )
                .Sum(t => t.Duration.Ticks));

            await hubContext.Clients.All.SendAsync("DurationDelete", userId, totalTodayHours);

            return Json(new { success = true });
        }


    }
}
