using FreelanceManager.Models;
using FreelanceManager.Repositry;

namespace FreelanceManager.Interfaces
{
    public interface IClientRepo : IGenericRepo<Client>
    {

        public IEnumerable<Client> GetAllWithProjects();
        //void Update(Client clientFromReq);

    }
}
