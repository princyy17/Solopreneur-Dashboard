using System;
using System.Linq;
using System.Security.Claims;
using FreelanceManager.Enums;
using FreelanceManager.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceManager.Controllers
{
    public class OverviewController : Controller
    {
        private readonly IClientRepo clientRepo;
        private readonly IMissionRepo missionRepo;
        private readonly IProjectRepo projectRepo;
        private readonly IInvoiceRepo invoiceRepo;
        private readonly ITimeTrackingRepo timeTrackingRepo;

        public OverviewController(
            IClientRepo clientRepo,
            IMissionRepo missionRepo,
            IProjectRepo projectRepo,
            IInvoiceRepo invoiceRepo,
            ITimeTrackingRepo timeTrackingRepo)
        {
            this.clientRepo = clientRepo;
            this.missionRepo = missionRepo;
            this.projectRepo = projectRepo;
            this.invoiceRepo = invoiceRepo;
            this.timeTrackingRepo = timeTrackingRepo;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated != true)
                return RedirectToAction("Register", "Freelancer");

            string userId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Register", "Freelancer");

            var projects = projectRepo.GetAll()?
                .Where(p => p != null && p.FreelancerId == userId)
                .ToList() ?? new();

            var missions = projects
                .Where(p => p.Missions != null)
                .SelectMany(p => p.Missions)
                .ToList();

            // Fix 1: use clientRepo directly — p.Client was never eagerly loaded
            var clients = clientRepo.GetAll()?
                .Where(c => c.FreelancerId == userId)
                .ToList() ?? new();

            // Fix 2: use invoiceRepo.GetAllWithProjects() — p.Invoices was never loaded
            var invoices = invoiceRepo.GetAllWithProjects()?
                .Where(i => i.Project != null && i.Project.FreelancerId == userId)
                .ToList() ?? new();

            // Fix 3: show actual pending dollar amount, not just a count
            decimal pendingAmount = invoices
                .Where(i => i.Status == InvoiceStatus.Pending)
                .Sum(i => (decimal)i.Hours * (decimal)(i.Project?.HourlyRate ?? 0));

            var overview = new OverviewVM
            {
                ClientsNum      = clients.Count,
                TasksNum        = missions.Count,
                ProjectsNum     = projects.Count,
                PendingInvoices = (int)pendingAmount,
                RecentTasks     = missions.TakeLast(5).ToList(),
                RecentProjects  = projects.TakeLast(5).ToList(),
            };

            return View(overview);
        }
    }
}