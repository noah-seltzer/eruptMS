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
        [InverseProperty("Supervsr")]
        public List<Supervisor> Supervsrs { get; set; }
        [InverseProperty("Employee")]
        public List<Supervisor> SupervisedEmployees { get; set; }
        [InverseProperty("Approvr")]
        public List<Approver> Approvrs { get; set; }
        [InverseProperty("Employee")]
        public List<Approver> ApprovedEmployees { get; set; }
        public List<ProjectEmployee> ProjectEmployees { get; set; }
        public List<WorkPackageEmployee> WorkPackageEmployees { get; set; }
        public List<Timesheet> Timesheets { get; set; }
    }
}