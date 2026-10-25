using FreelanceManager.Enums;
using FreelanceManager.Models;
using FreelanceManager.ViewModel;
namespace FreelanceManager.Interfaces
{
    public interface IMissionRepo:IGenericRepo<Mission>
    {
        public IEnumerable<Mission> GetAllFilter(string? search, status? status, priority? priority);

    }
}
