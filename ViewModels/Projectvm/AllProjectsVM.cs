using FreelanceManager.Enums;

namespace FreelanceManager.ViewModels.Projectvm
{
    public class AllProjectsVM
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
		public status ProjectStatus { get; set; } 
		public int AllMissionsCount { get; set; }
		public int CompletedMissionsCount { get; set; }
	}
}
