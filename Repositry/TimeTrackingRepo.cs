using FreelanceManager.Interfaces;
using FreelanceManager.Models;

namespace FreelanceManager.Repositry
{
    public class TimeTrackingRepo:GenericRepo<TimeTracking>,ITimeTrackingRepo
    {
        ITIContext context;
        public TimeTrackingRepo(ITIContext context):base(context)
        {
            this.context = context;
        }
    }
}
