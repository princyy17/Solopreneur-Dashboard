using FreelanceManager.Enums;
using System.ComponentModel.DataAnnotations;

namespace FreelanceManager.ViewModels.ClientVM
{
    public class AllClientsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public ClientStatus Status { get; set; }
        public int TasksCount { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public DateTime? FirstProjectDate { get; set; }
    }
}
