using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP4911Timesheets.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using COMP4911Timesheets.Models;
using COMP4911Timesheets.Controllers;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Common;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using COMP4911Timesheets.Services;

namespace COMP4911Timesheets
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            Utility.ConnectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddIdentity<Employee, ApplicationRole>(
                options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddScoped<IService, VacationTimeService>();
            services.AddScoped<ISickLeaveService, SickLeaveService>();
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager,
            UserManager<Employee> userManager
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DummyData.InitializeAsync(app);

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            var manager = new RecurringJobManager();
            manager.AddOrUpdate("UPDATE-VACATION-TIME", Job.FromExpression(() => updateVacationTime()), "0 0 1 * * ", TimeZoneInfo.Local);
            manager.AddOrUpdate("UPDATE-SICK-LEAVE", Job.FromExpression(() => updateSickLeave()), "0 0 1 1 * ", TimeZoneInfo.Local);
        }

        public static void updateVacationTime()
        {
            BackgroundJob.Enqueue<VacationTimeService>(vts => vts.updateVacationTimes());
        }

        public static void updateSickLeave()
        {
            BackgroundJob.Enqueue<SickLeaveService>(sls => sls.updateSickLeaves());
        }
    }
}
