using FreelanceManager.Interfaces;
using FreelanceManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FreelanceManager.Repositry
{
    public class InvoiceRepo:IGenericRepo<Invoice>,IInvoiceRepo
    {
        ITIContext context;
        public InvoiceRepo(ITIContext context)
        {
            this.context = context;
        }

        public IEnumerable<Invoice> GetAll()
        {
            IEnumerable<Invoice> invoicesList = context.invoices.ToList().Where(c=>c.IsDeleted==false);
            return invoicesList;
        }

        public IEnumerable<Invoice> GetAllWithProjects()
        {
            //return context.clients.Include(c => c.projects);
            return context.invoices.Include(c => c.Project).ThenInclude(p=>p.Client).ToList().Where(c => c.IsDeleted == false);
        }
        public Invoice GetById(int id)
        {
            return context.invoices.FirstOrDefault(c => c.Id == id);
        }
        public void Add(Invoice invoiceFromReq)
        {
            context.Add(invoiceFromReq);
        }
        public void RemoveById(int id)
        {
            Invoice invoice = GetById(id);
            invoice.IsDeleted = true;
        }
        public void Update(int id, Invoice invoiceFromReq)
        {
            Invoice invoiceFromDb = GetById(invoiceFromReq.Id);

            if (invoiceFromDb != null)
            {
                context.invoices.Update(invoiceFromReq);
            }
        }
        public void Save()
        {
            context.SaveChanges();
        }

        public void RemoveByObj(Invoice obj)
        {
            throw new NotImplementedException();
        }
    }
}
