using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Title { get; set; }
        public DateTime CreatedTime { get; set; }
        public double FlexTime { get; set; }
        public double VacationTime { get; set; }
        public int Status { get; set; }

        public List<EmployeePay> EmployeePays { get; set; }
        public List<Credential> Credentials { get; set; }
        public List<Signature> Signatures { get; set; }
        public List<ProjectEmployee> ProjectEmployees { get; set; }
        public List<WorkPackageEmployee> WorkPackageEmployees { get; set; }
        public List<Timesheet> Timesheets { get; set; }

        public int? ApproverId { get; set; }
        public Approver Approver { get; set; }

        public int? SupervisorId { get; set; }
        public Supervisor Supervisor { get; set; }
    }
}