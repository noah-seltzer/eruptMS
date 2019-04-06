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

            // modelBuilder.Entity<Employee>()
            //     .HasOne(e => e.Supervisor)
            //     .WithMany(s => s.Supervisees)
            //     .OnDelete(DeleteBehavior.Restrict);
            // modelBuilder.Entity<Employee>()
            //     .HasOne(e => e.Approver)
            //     .WithMany(a => a.Approvees)
            //     .OnDelete(DeleteBehavior.Restrict);
            // modelBuilder.Entity<Employee>()
            //     .HasOne(e => e.Supervisor)
            //     .WithMany(s => s.Supervisees)
            //     .OnDelete(DeleteBehavior.Restrict);
            // modelBuilder.Entity<Employee>()
            //     .HasOne(e => e.Approver)
            //     .WithMany(a => a.Approvees)
            //     .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Signature>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.Signatures)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<EmployeePay>()
                .HasOne(ep => ep.Employee)
                .WithMany(e => e.EmployeePays)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(pe => pe.Employee)
                .WithMany(e => e.ProjectEmployees)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(pe => pe.Project)
                .WithMany(p => p.ProjectEmployees)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.Employee)
                .WithMany(e => e.Timesheets)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<ProjectRequest>()
                .HasOne(pr => pr.Project)
                .WithMany(p => p.ProjectRequests)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<ProjectRequest>()
                .HasOne(pr => pr.PayGrade)
                .WithMany(pg => pg.ProjectRequests)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(pe => pe.WorkPackage)
                .WithMany(wp => wp.ProjectEmployees)
                .OnDelete(DeleteBehavior.SetNull);
        }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<PayGrade> PayGrades { get; set; }
        public DbSet<EmployeePay> EmployeePays { get; set; }
        public DbSet<Signature> Signatures { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectRequest> ProjectRequests { get; set; }
        public DbSet<ProjectReport> ProjectReports { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployees { get; set; }
        public DbSet<WorkPackage> WorkPackages { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<WorkPackageReport> WorkPackageReports { get; set; }
        public DbSet<ResponsibleEngineerReport> ResponsibleEngineerReport { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<TimesheetRow> TimesheetRows { get; set; }
    }
}
