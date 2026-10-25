namespace FreelanceManager.ViewModels.overviewVm
{
    public class OverviewVM
    {

        public int TasksNum { get; set; } = 0;
        public int ClientsNum { get; set; } = 0;
        public int PendingInvoices { get; set; } = 0;

        public int ProjectsNum{ get; set; } = 0;
        public IEnumerable<Mission> RecentTasks { get; set; } = new List<Mission>();
        public IEnumerable<Project> RecentProjects { get; set; } = new List<Project>();





    }
}
