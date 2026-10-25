using FreelanceManager.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelanceManager.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Budget { get; set; }
        public double HourlyRate { get; set; }
        public string Company { get; set; }
        public priority Priority { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public status Status { get; set; }
        public category Categoty { get; set; }

        public bool IsDeleted { get; set; }
        public int ClientId { get; set; }
        public string FreelancerId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; }

		[ForeignKey(nameof(FreelancerId))]
		public Freelancer Freelancer { get; set; }


        public ICollection<Mission>? Missions { get; set; }
        public ICollection<Invoice>? Invoices { get; set; }
    }
}
