
using FreelanceManager.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static FreelanceManager.Models.Mission;

namespace FreelanceManager.Models
{
    public class Mission
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public status Status { get; set; }
        public priority Priority { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsDeleted { get; set; } = false;

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan EstimateTime { get; set; }
        public int ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }
        public IEnumerable<TimeTracking>? TimeTracking { get; set; }

       

    }
}