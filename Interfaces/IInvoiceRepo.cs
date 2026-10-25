using FreelanceManager.Models;

namespace FreelanceManager.Interfaces
{
    public interface IInvoiceRepo:IGenericRepo<Invoice>
    {
        public IEnumerable<Invoice> GetAllWithProjects();
    }
}
