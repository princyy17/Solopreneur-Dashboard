using FreelanceManager.Enums;
using FreelanceManager.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelanceManager.ViewModel
{
    public class MissionViewModel
    {

        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public status Status { get; set; }
        [Required]
        public priority Priority { get; set; }
        [Required]
        public DateTime Deadline { get; set; }

        public string?ProjectName { get; set; }
        public int ProjectId { get; set; }
       

    }
  
}
