using FreelanceManager.Interfaces;
using FreelanceManager.Models;
using FreelanceManager.Repositry;
using FreelanceManager.ViewModels.ClientVM;
using FreelanceManager.Enums;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Security.Claims;

namespace FreelanceManager.Controllers
{

    public class ClientsController : Controller
    {
        private readonly IClientRepo clientRepo;
        private readonly IMissionRepo missionRepo;

        public ClientsController(IClientRepo clientRepo,IMissionRepo missionRepo)
        {
            this.clientRepo = clientRepo;
            this.missionRepo = missionRepo;
        }

        public IActionResult Index()
        {
            string Userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            List<AllClientsVM> clientsListVM = new List<AllClientsVM>();

            IEnumerable<Client> clientsList = clientRepo.GetAllWithProjects().Where(c=>c.FreelancerId==Userid);
            IEnumerable<Mission> tasksList = missionRepo.GetAll();

           // AllClientsVM clientVM = new AllClientsVM();
            //ViewBag.tasks = tasksList;
            if(clientsList != null)
            {
                foreach (var clientItem in clientsList)
                {
                    AllClientsVM clientVM = new AllClientsVM
                    {
                        Id = clientItem.Id,
                        Name = clientItem.Name,
                        Address = clientItem.Address,
                        Email = clientItem.Email,
                        Phone = clientItem.Phone,
                        ContactPerson = clientItem.ContactPerson,
                        Status = clientItem.Status,
                        CompletedTasks = 0,
                        InProgressTasks = 0,
                        TasksCount = 0,
                        FirstProjectDate = null
                    };

                    if (clientItem.projects?.Any() == true)
                    {
                        foreach (var project in clientItem.projects)
                        {
                            if (project.Missions?.Any() == true)
                            {
                                clientVM.CompletedTasks += project.Missions.Count(m => m.Status == status.Completed);
                                clientVM.InProgressTasks += project.Missions.Count(t => t.Status == status.InProgress);
                                clientVM.TasksCount += project.Missions.Count();
                            }

                            if (clientVM.FirstProjectDate == null || project.StartDate < clientVM.FirstProjectDate)
                            {
                                clientVM.FirstProjectDate = project.StartDate;
                            }
                        }
                    }


                    clientsListVM.Add(clientVM);
                }
            }
            

            return View("Index", clientsListVM);
        }


        public IActionResult New()
        {
            return PartialView("AddClientPartial", new AddClientVM());
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SaveNew(AddClientVM clientFromReq)
        {
            if (ModelState.IsValid)
            {
                Client client = new Client();
                client.Name = clientFromReq.Name;
                client.Phone = clientFromReq.Phone;
                client.Address = clientFromReq.Address;
                client.Email = clientFromReq.Email;
                client.ContactPerson = clientFromReq.ContactPerson;
                client.Status = clientFromReq.Status;
                client.FreelancerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                clientRepo.Add(client);
                clientRepo.Save();
                return RedirectToAction("Index");
            }
            return PartialView("AddClientPartial", clientFromReq);
        }

        public IActionResult Edit(int Id)
        {
            Client clientFromDb = clientRepo.GetById(Id);
            AddClientVM clientVM = new AddClientVM
            {
                Id = clientFromDb.Id,
                Name = clientFromDb.Name,
                Phone = clientFromDb.Phone,
                Address = clientFromDb.Address,
                Email = clientFromDb.Email,
                ContactPerson = clientFromDb.ContactPerson,
                Status = clientFromDb.Status
            };

            return PartialView("EditClientPartial", clientVM);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SaveEdit(AddClientVM clientFromReq)
        {

            if (ModelState.IsValid)
            {
                Client client = clientRepo.GetById(clientFromReq.Id);

                client.Name = clientFromReq.Name;
                client.Phone = clientFromReq.Phone;
                client.Address = clientFromReq.Address;
                client.Email = clientFromReq.Email;
                client.ContactPerson = clientFromReq.ContactPerson;
                client.Status = clientFromReq.Status;

                clientRepo.Update(clientFromReq.Id, client);
                clientRepo.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return PartialView("EditClientPartial", clientFromReq);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                clientRepo.RemoveById(id);
                clientRepo.Save();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return NotFound("Sorry You Cant Delete This Course {This Course Not Empty}");
            }

        }
    }
}
