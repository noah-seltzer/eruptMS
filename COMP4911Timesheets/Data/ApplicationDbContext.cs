using System;
using System.Collections.Generic;
using System.Text;
using COMP4911Timesheets.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace COMP4911Timesheets.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Approver)
                .WithMany(a => a.Employees)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Supervisor)
                .WithMany(s => s.Employees)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkPackage>()
                .HasOne(wp => wp.ParentWorkPackage)
                .WithMany(pwp => pwp.WorkPackages)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<PayGrade> PayGrade { get; set; }
        public DbSet<EmployeePay> EmployeePays { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<Signature> Signatures { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<Approver> Approvers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectReport> ProjectReports { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployees { get; set; }
        public DbSet<WorkPackage> WorkPackages { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<WorkPackageReport> WorkPackageReports { get; set; }
        public DbSet<ParentWorkPackage> ParentWorkPackages { get; set; }
        public DbSet<WorkPackageEmployee> WorkPackageEmployees { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<TimesheetRow> TimesheetRows { get; set; }
    }
}
