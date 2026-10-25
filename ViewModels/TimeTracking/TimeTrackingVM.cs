using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FreelanceManager.ViewModels.TimeTracking
{
    public class TimeTrackingVM
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }


        [Required(ErrorMessage = "Please select a mission.")]
        public int MissionId { get; set; }
    }
}
