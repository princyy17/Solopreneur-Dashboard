using FreelanceManager.Enums;
using FreelanceManager.Hubs;
using FreelanceManager.Interfaces;
using FreelanceManager.Models;
using FreelanceManager.Repositry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace FreelanceManager.Controllers
{
    [Authorize]
    public class MissionController : Controller
    {
        private readonly IMissionRepo missionRepo;
        private readonly IProjectRepo projectRepo;
        private readonly IHubContext<ChatAdminHub> hubContext;


        public MissionController(IHubContext<ChatAdminHub> hubContext,IMissionRepo missionRepo,IProjectRepo projectRepo) 
        {
            this.missionRepo = missionRepo;
            this.projectRepo =projectRepo;
            this.hubContext= hubContext;
        }
        #region GetAll

        [HttpGet]
        public IActionResult Index(string? search, status? status, priority? priority)
        {

            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            // First get all project IDs associated with the current freelancer
            var freelancerProjectIds = projectRepo.GetAll()
                .Where(p => p.FreelancerId == userId)
                .Select(p => p.Id)
                .ToList();

            // Then filter missions by those project IDs
            var missions = missionRepo.GetAllFilter(search, status, priority)
                .Where(m => freelancerProjectIds.Contains(m.ProjectId))
                .ToList();

            // Map to MissionViewModel
            List<MissionViewModel> MissView = missions.Select(m => new MissionViewModel
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Status = m.Status,
                Priority = m.Priority,
                Deadline = m.Deadline,
                ProjectId = m.ProjectId,
                ProjectName= projectRepo.GetById(m.ProjectId).Name
            }).ToList();

            return View("All", MissView);
        }
        #endregion

        #region Details

        [HttpGet]
        public IActionResult Details(int id)
        {
            Mission mission = missionRepo.GetById(id);
            MissionViewModel viewMission = new MissionViewModel
            {
                Id = mission.Id,
                Title = mission.Title,
                Description = mission.Description,
                Status = mission.Status,
                Priority = mission.Priority,
                Deadline = mission.Deadline,
                ProjectId = mission.ProjectId,
                ProjectName = projectRepo.GetById(mission.ProjectId).Name
            };
            return PartialView("DetailsPartial",viewMission);
        }

        #endregion

        #region Add

        [HttpGet]
        public IActionResult Add()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var freelancerProject = projectRepo.GetAll()
                .Where(p => p.FreelancerId == userId)
                .ToList();

            var model = new MissionViewModel();
            ViewBag.ProjectList = freelancerProject;
            return PartialView("AddPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAdd(MissionViewModel mission)
        {

            if (ModelState.IsValid)
            {

                Mission newmission = new Mission();
                newmission.Title = mission.Title;
                newmission.Description = mission.Description;
                newmission.Status = mission.Status;
                newmission.Priority = mission.Priority;
                newmission.Deadline = mission.Deadline;
                newmission.ProjectId = mission.ProjectId;

                missionRepo.Add(newmission);
                missionRepo.Save();
                var freelancerId = projectRepo.GetById(newmission.ProjectId).FreelancerId;

                await hubContext.Clients.All.SendAsync("MissionAdded", freelancerId);
                return RedirectToAction("Index");
            }
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var freelancerProject = projectRepo.GetAll()
                .Where(p => p.FreelancerId == userId)
                .ToList();
            ViewBag.ProjectList = freelancerProject;
           

            return PartialView("AddPartial", mission);
        }
        #endregion

        #region Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var mission = missionRepo.GetById(id);
            MissionViewModel Editmission = new MissionViewModel {
                Id = mission.Id,
                Title = mission.Title,
                Description = mission.Description,
                Status = mission.Status,
                Priority = mission.Priority,
                Deadline = mission.Deadline,
                ProjectId = mission.ProjectId,
                ProjectName = projectRepo.GetById(mission.ProjectId).Name

            };
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var freelancerProject = projectRepo.GetAll()
                .Where(p => p.FreelancerId == userId)
                .ToList();

            ViewBag.ProjectList = freelancerProject;
            return View("EditPartial",Editmission);
        }
        #endregion

        #region SaveEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveEdit(MissionViewModel mission)
        {
            if (ModelState.IsValid)
            {
                Mission Editmission = missionRepo.GetById(mission.Id);

                Editmission.Title = mission.Title;
                Editmission.Description = mission.Description;
                Editmission.Status = mission.Status;
                Editmission.Priority = mission.Priority;
                Editmission.Deadline = mission.Deadline;
                Editmission.ProjectId = mission.ProjectId;


                missionRepo.Save();
                return RedirectToAction("Index");
            }
            return View("EditPartial",mission);
        }
        #endregion


        #region Delete


        [HttpPost]
        public async Task<IActionResult> Delete(MissionViewModel obj)
        {
            Mission mission = missionRepo.GetById(obj.Id);

            if (mission != null)
            {
                missionRepo.RemoveById(obj.Id);
                missionRepo.Save();
                var freelancerId = projectRepo.GetById(mission.ProjectId).FreelancerId;
                await hubContext.Clients.All.SendAsync("MissionDeleted", freelancerId);

                return RedirectToAction("Index");

            }
            else
            {
                return NotFound("Task not Exist");
            }

        }

        #endregion
    }
}
