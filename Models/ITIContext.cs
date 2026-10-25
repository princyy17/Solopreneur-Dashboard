using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection.Emit;

namespace FreelanceManager.Models
{
    public class ITIContext : IdentityDbContext<Freelancer>
    {
        public ITIContext(DbContextOptions<ITIContext> options) : base(options)
        {

        }
        public DbSet<Client> clients { get; set; }
        public DbSet<Invoice> invoices { get; set; }
        public DbSet<Mission> missions { get; set; }
        public DbSet<Project> projects { get; set; }
        public DbSet<TimeTracking> timeTracking { get; set; }



       


    }




}