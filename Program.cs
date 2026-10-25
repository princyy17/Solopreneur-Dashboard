using FreelanceManager.Controllers;
using FreelanceManager.Interfaces;
using FreelanceManager.Models;
using FreelanceManager.Repositry;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FreelanceManager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSignalR();

            // ✅ Exp 10 — Memory Cache & Response Caching
            builder.Services.AddMemoryCache();
            builder.Services.AddResponseCaching();

            // ✅ Exp 9 — Session
            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // ✅ Switched from SQL Server to MySQL
            var connectionString = builder.Configuration.GetConnectionString("cs");
            builder.Services.AddDbContext<ITIContext>(options =>
                options.UseMySql(connectionString,
                    ServerVersion.AutoDetect(connectionString)));

            builder.Services.AddScoped<IProjectRepo, ProjectRepo>();
            builder.Services.AddScoped<IClientRepo, ClientRepo>();
            builder.Services.AddScoped<IFreelancerRepo, FreelancerRepo>();
            builder.Services.AddScoped<IInvoiceRepo, InvoiceRepo>();
            builder.Services.AddScoped<IMissionRepo, MissionRepo>();
            builder.Services.AddScoped<ITimeTrackingRepo, TimeTrackingRepo>();

            builder.Services.AddIdentity<Freelancer, IdentityRole>()
                .AddEntityFrameworkStores<ITIContext>();

            // ✅ Exp 8 — Async HTTP Client for quote fetching
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            // ✅ Exp 10 — Response Caching middleware
            app.UseResponseCaching();

            // ✅ Exp 9 — Session middleware
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatAdminHub>("/dashboardHub");
            app.MapHub<ChatAdminHub>("/ChatAdmin");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // ✅ Seed roles into database
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = new[] { "FREELANCER", "ADMIN", "CLIENT" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            app.Run();
        }
    }
}