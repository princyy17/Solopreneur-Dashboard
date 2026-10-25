using FreelanceManager.Enums;
using FreelanceManager.Interfaces;
using FreelanceManager.Models;
using FreelanceManager.ViewModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FreelanceManager.Repositry
{
    //.Include(m => m.Project)
    public class MissionRepo: GenericRepo<Mission>, IMissionRepo
    {
        ITIContext context;
        public MissionRepo(ITIContext context) : base (context) 
        {
            this.context = context;
        }


        public IEnumerable<Mission> GetAllFilter(string? search, status? status, priority? priority)
        {
            var missions = context.missions.Where(m =>
             (string.IsNullOrEmpty(search) || m.Title.Contains(search)) &&
             (status == null || m.Status == status) &&
             (priority == null || m.Priority == priority));


            if (!missions.Any())
            {
                Console.WriteLine("Tasks Not Found");
            }

            return missions;
        }
    }
}
