using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Employee : IdentityUser<string>
    {
        public Employee() : base() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }
        //public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public int Title { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
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

        public readonly Dictionary<int, string> jobTitles = new Dictionary<int, string>
        {
            {10, "Administrator"},
            {100, "HR Manager"},
            {200, "Line Manager"},
            {210, "Supervisor"},
            {510, "Business Analyst"},
            {520, "Technical Writer"},
            {610, "Software Architect"},
            {620, "Software Developer"},
            {621, "Senior Software Developer"},
            {622, "Junior Software Developer"},
            {630, "Software Tester"},
            {710, "UI Designer"}
        };
        
        public string StringTitle(int key)
        {
            string result = null;

            if (jobTitles.ContainsKey(key))
            {
                result = jobTitles[key];
            }

            return result;
        }
    }
}