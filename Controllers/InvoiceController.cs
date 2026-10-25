using FreelanceManager.Interfaces;
using FreelanceManager.ViewModels.InvoiceVM;
using FreelanceManager.Enums;
using Microsoft.AspNetCore.Mvc;
using FreelanceManager.ViewModels.ClientVM;
using FreelanceManager.Repositry;
using FreelanceManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FreelanceManager.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepo invoiceRepo;
        private readonly IProjectRepo ProjectRepo;
        public InvoiceController(IInvoiceRepo invoiceRepo, IProjectRepo ProjectRepo)
        {
            this.invoiceRepo = invoiceRepo;
            this.ProjectRepo = ProjectRepo;
        }
        public IActionResult Index(string statusFilter=null)
        {
            string Userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            IEnumerable<Invoice> invoicesList = invoiceRepo.GetAllWithProjects().Where(P=>P.Project.FreelancerId==Userid);
            List<AllInvoicesVM> invoicesListVM= new List<AllInvoicesVM>();

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                invoicesList = invoicesList.Where(i=>i.CurrencyName.ToString() == statusFilter);
            }

            int OverdueCount = 0;
            int Paid=0;

            if (invoicesList.Any())
            {
                
                foreach (var invoiceItem in invoicesList)
                {
                    AllInvoicesVM invoiceVM = new AllInvoicesVM
                    {
                        Id = invoiceItem.Id,
                        Name = invoiceItem.Name,
                        ClientName = invoiceItem.Project.Client.Name,
                        InvoiceDate = invoiceItem.InvoiceDate,
                        DueDate = invoiceItem.DueDate,
                        Status = invoiceItem.Status,
                        CurrencyName = invoiceItem.CurrencyName,
                        Hours = invoiceItem.Hours,
                        HourlyRate= invoiceItem.Project.HourlyRate,
                        Amount = (decimal)invoiceItem.Hours * (decimal)invoiceItem.Project.HourlyRate
                    };
                    if(invoiceItem.Status==InvoiceStatus.Overdue)
                    {
                        OverdueCount++;
                        invoiceVM.StatusColor = "red-600";
                    }
                    else if (invoiceItem.Status == InvoiceStatus.Paid)
                    {
                        Paid++;
                        invoiceVM.StatusColor = "black";
                    }
                    else
                    {
                        invoiceVM.StatusColor = "gray-400";
                    }

                        invoicesListVM.Add(invoiceVM);
                }
                
            }
            decimal totalAmount = 0;
            foreach (var invoice in invoicesListVM)
            {
                totalAmount += invoice.Amount;
            }

            ViewBag.OverdueCount = OverdueCount;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.Paid = OverdueCount;

            return View("Index",invoicesListVM);
        }

        public IActionResult New()
        {
            string Userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            ViewBag.Projects=ProjectRepo.GetAll().Where(p=>p.FreelancerId==Userid).ToList();
            ViewBag.CurrencyList = new SelectList(Enum.GetValues(typeof(Currency)));
            return PartialView("AddInvoicePartial", new AddInvoiceVM());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SaveNew(AddInvoiceVM invoiceFromReq)
        {
            if (ModelState.IsValid)
            {
                Invoice invoice = new Invoice();
                invoice.Name = invoiceFromReq.Name;
                invoice.CurrencyName = invoiceFromReq.CurrencyName;
                invoice.ProjectId = invoiceFromReq.ProjectId;
                invoice.Hours = invoiceFromReq.Hours;
                invoice.Notes = invoiceFromReq.Notes;
                invoice.Status = invoiceFromReq.Status;
                invoice.InvoiceDate = invoiceFromReq.InvoiceDate;
                invoice.DueDate = invoiceFromReq.DueDate;
                invoice.IsDeleted = false;

                invoiceRepo.Add(invoice);
                invoiceRepo.Save();
                return RedirectToAction("Index");
            }
            return PartialView("AddInvoicePartial", invoiceFromReq);
        }


        public IActionResult Edit(int Id)
        {
            Invoice invoiceFromReq = invoiceRepo.GetById(Id);
            AddInvoiceVM invoiceVM = new AddInvoiceVM
            {
                Name = invoiceFromReq.Name,
                CurrencyName = invoiceFromReq.CurrencyName,
                ProjectId = invoiceFromReq.ProjectId,
                Hours = invoiceFromReq.Hours,
                Notes = invoiceFromReq.Notes,
                Status = invoiceFromReq.Status,
                InvoiceDate = invoiceFromReq.InvoiceDate,
                DueDate = invoiceFromReq.DueDate
            };
            string Userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            ViewBag.Projects = ProjectRepo.GetAll().Where(p => p.FreelancerId == Userid).ToList();
            ViewBag.CurrencyList = new SelectList(Enum.GetValues(typeof(Currency)));

            return PartialView("EditInvoicePartial", invoiceVM);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SaveEdit(AddInvoiceVM invoiceFromReq)
        {

            if (ModelState.IsValid)
            {
                Invoice invoice = invoiceRepo.GetById(invoiceFromReq.Id);

                //if (invoice == null)
                //{
                //    return NotFound(); 
                //}

                invoice.Name = invoiceFromReq.Name;
                invoice.CurrencyName = invoiceFromReq.CurrencyName;
                invoice.ProjectId = invoiceFromReq.ProjectId;
                invoice.Hours = invoiceFromReq.Hours;
                invoice.Notes = invoiceFromReq.Notes;
                invoice.Status = invoiceFromReq.Status;
                invoice.InvoiceDate = invoiceFromReq.InvoiceDate;
                invoice.DueDate = invoiceFromReq.DueDate;

                invoiceRepo.Update(invoiceFromReq.Id, invoice);
                invoiceRepo.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return PartialView("EditInvoicePartial", invoiceFromReq);
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                invoiceRepo.RemoveById(id);
                invoiceRepo.Save();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return NotFound("Sorry You Cant Delete This Course {This Course Not Empty}");
            }

        }


    }
}
