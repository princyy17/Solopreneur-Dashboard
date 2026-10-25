using FreelanceManager.Interfaces;
using FreelanceManager.Models;

namespace FreelanceManager.Repositry
{
    public class FreelancerRepo:GenericRepo<Freelancer> ,IFreelancerRepo
    {
        ITIContext context;
        public FreelancerRepo(ITIContext context) : base(context)
        {
            this.context = context;
        }
        public Freelancer GetByIdString(string UserId)
        {
            return context.Users.FirstOrDefault(u => u.Id == UserId);
        }



    }
}
