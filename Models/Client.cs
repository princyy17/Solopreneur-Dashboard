using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FreelanceManager.Enums;

namespace FreelanceManager.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
        public ClientStatus Status { get; set; }
        public string FreelancerId { get; set; }

        [ForeignKey(nameof(FreelancerId))]
        public Freelancer Freelancer { get; set; }
        public ICollection<Project>? projects { get; set; }

    }
}
