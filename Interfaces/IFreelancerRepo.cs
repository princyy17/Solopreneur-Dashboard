using FreelanceManager.Models;

namespace FreelanceManager.Interfaces
{
    public interface IFreelancerRepo:IGenericRepo<Freelancer>
    {
        public Freelancer GetByIdString(string UserId);
    }
}
