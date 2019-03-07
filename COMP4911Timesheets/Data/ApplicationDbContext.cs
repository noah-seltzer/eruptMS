using System;
using System.Collections.Generic;
using System.Text;
using COMP4911Timesheets.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace COMP4911Timesheets.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.EmployeePay)
                .WithMany(ep => ep.Timesheets)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<PayGrade> PayGrades { get; set; }
        public DbSet<EmployeePay> EmployeePays { get; set; }
        public DbSet<Signature> Signatures { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectReport> ProjectReports { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployees { get; set; }
        public DbSet<WorkPackage> WorkPackages { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<WorkPackageReport> WorkPackageReports { get; set; }
        public DbSet<WorkPackageEmployee> WorkPackageEmployees { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<TimesheetRow> TimesheetRows { get; set; }
    }
}
