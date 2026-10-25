using FreelanceManager.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FreelanceManager.Repositry;
using FreelanceManager.ViewModels.Projectvm;
using FreelanceManager.Models;
using FreelanceManager.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using FreelanceManager.Hubs;

namespace FreelanceManager.Controllers
{

    public class ProjectController : Controller
    {
        private readonly IProjectRepo projectRepo;
        private readonly IMissionRepo missionRepo;
        private readonly IClientRepo clientRepo;
        private readonly IHubContext<ChatAdminHub> hubContext;

        public ProjectController(IHubContext<ChatAdminHub> hubContext,IProjectRepo projectRepo, IMissionRepo missionRepo, IClientRepo clientRepo)
        {
            this.projectRepo = projectRepo;
            this.missionRepo = missionRepo;
            this.clientRepo = clientRepo;
            this.hubContext= hubContext;
        }
        // project
        [HttpGet]
        [Authorize]
        public IActionResult Index(string statusFilter = null)
        {

		    string Userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value; // this return id of user
            var projectsQuery = projectRepo.GetAll().Where(p=>p.FreelancerId == Userid);

			if (!string.IsNullOrEmpty(statusFilter))
			{
				projectsQuery = projectsQuery.Where(p => p.Status.ToString() == statusFilter);
			}

            var projects = projectsQuery
                .Select(p => new AllProjectsVM
                {
                    Name = p.Name,
                    Priority = p.Priority,
                    Budget = p.Budget,
                    Company = p.Company,
                    AllMissionsCount = p.Missions.Count(),
                    CompletedMissionsCount = p.Missions.Count(m => m.Status == status.Completed),
                    Description = p.Description,
                    EndDate = p.EndDate,
                    HourlyRate = p.HourlyRate,
                    Id = p.Id,
                    ProjectStatus = p.Status,
                    StartDate = p.StartDate
                }).ToList();

            return View(projects);
			//return View(projects);
        }


        public IActionResult Details(int id)
        {
            var project = projectRepo.GetById(id);
            ProjectDetialsVM projectDetialsVM = new ProjectDetialsVM();
            projectDetialsVM.Id = id;
            projectDetialsVM.Name = project.Name;
            projectDetialsVM.Budget = project.Budget;
            projectDetialsVM.HourlyRate = project.HourlyRate;
            projectDetialsVM.Description = project.Description;
            projectDetialsVM.Status = project.Status;
            projectDetialsVM.StartDate = project.StartDate;
            projectDetialsVM.EndDate = project.EndDate;
            projectDetialsVM.Company = project.Company;
            projectDetialsVM.Priority = project.Priority;
            projectDetialsVM.AllMissionsCount = project.Missions.Count();
            projectDetialsVM.Categoty = project.Categoty;
            projectDetialsVM.ClientName = project.Client.Name;
            projectDetialsVM.CompletedMissionsCount = project.Missions.Count(m => m.Status == status.Completed);
            projectDetialsVM.Missions = missionRepo.GetAll();
            return View(projectDetialsVM);
        }

        [HttpGet]
        public IActionResult Add()
        {
            AddProjectVM newproject = new AddProjectVM();
            ViewBag.Clients = clientRepo.GetAll();
            return PartialView("AddProjectPartialView", newproject);
        }

        [HttpPost]

        public async Task<IActionResult> SaveAdd(AddProjectVM project)
          {
            if (!ModelState.IsValid)
            {
                ViewBag.Clients = clientRepo.GetAll();
                return PartialView("AddProjectPartialView", project);
            }


            Project newProject = new()
            {
                Name = project.Name,
                Budget = project.Budget,
                ClientId = project.ClientId,
                Company = project.Company,
                Description = project.Description,
                StartDate= project.StartDate,
                EndDate= project.EndDate,
                HourlyRate  = project.HourlyRate,
                Priority = project.Priority,
                FreelancerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value,


			};

            projectRepo.Add(newProject);

            projectRepo.Save();
            await hubContext.Clients.All.SendAsync("ProjectAdded", newProject.FreelancerId);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var project = projectRepo.GetById(id);
            AddProjectVM EditVM = new AddProjectVM
            {
                Id = project.Id,
                Name = project.Name,
                Budget = project.Budget,
                ClientId = project.ClientId,
                Company = project.Company,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                category = project.Categoty,
                HourlyRate= project.HourlyRate,
                Priority = project.Priority,
			};
			ViewBag.Clients = clientRepo.GetAll();
            return PartialView("Edit", EditVM);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult SaveEdit(AddProjectVM editFromReq)
        {
            if (ModelState.IsValid)
            {
                Project project = projectRepo.GetById(editFromReq.Id);
                project.Name = editFromReq.Name;
				project.ClientId = editFromReq.ClientId;
                project.Company = editFromReq.Company;
                project.Budget = editFromReq.Budget;
				project.StartDate = editFromReq.StartDate;
				project.EndDate = editFromReq.EndDate;
                project.Description = editFromReq.Description;
                project.Categoty = editFromReq.category;
                project.HourlyRate = editFromReq.HourlyRate;
                project.Priority = editFromReq.Priority;

				missionRepo.Save();
				return RedirectToAction("Index");
			}
            return View("Edit", editFromReq);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Project deleteProject = projectRepo.GetById(id);
            if (deleteProject != null)
            {
                projectRepo.RemoveById(deleteProject.Id);
                projectRepo.Save();
                var freelancerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                await hubContext.Clients.All.SendAsync("ProjectDeleted", freelancerId);

                return RedirectToAction("Index");
            }
            return NotFound();

        }



        public IActionResult PriortyFilter(string priority)
        { 
          var projects =   projectRepo.GetAll().Select(p=>p.Priority.ToString()==priority);


            return Json(projects);
        }


	}
}
