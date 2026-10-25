using Microsoft.AspNetCore.Identity;

namespace FreelanceManager.Models
{
    public class Freelancer: IdentityUser
    {
		public bool IsDeleted { get; set; }
		public ICollection<Project> projects { get; set; }
        public ICollection<Client> clients { get; set; }
    }
}
