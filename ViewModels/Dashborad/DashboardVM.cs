namespace FreelanceManager.ViewModels.DashboradVM
{
    public class DashboardVM 
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<int> missionId { get; set; }
        public int ProjectCount { get; set; } 
        public int MissionCount { get; set; } 
        public TimeSpan TodayHours { get; set; }

        
    }
}
