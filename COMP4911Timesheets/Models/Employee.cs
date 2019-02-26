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

        public static readonly int NOT_EMPLOYEED = 0;
        public static readonly int CURRENTLY_EMPLOYEED = 1;
        public static readonly int MATERNITY_LEAVE = 2;

        public static readonly int ADMIN = 10;
        public static readonly int HR_MANAGER = 100;
        public static readonly int LINE_MANAGER = 200;
        public static readonly int SUPERVISOR = 210;
        public static readonly int BUSINESS_ANALYST = 510;
        public static readonly int TECHNICAL_WRITER = 520;
        public static readonly int SOFTWARE_ARCHITECT = 610;
        public static readonly int SOFTWARE_DEVELOPER = 620;
        public static readonly int SENIOR_SOFTWARE_DEVELOPER = 621;
        public static readonly int JUNIOR_SOFTWARE_DEVELOPER = 623;
        public static readonly int INTERMEDIATE_SOFTWARE_DEVELOPER = 622;
        public static readonly int SOFTWARE_TESTER = 630;
        public static readonly int UI_DESIGNER = 710;

        public readonly Dictionary<int, string> Statuses = new Dictionary<int, string>
        {
            {0, "Not Employeed"},
            {1, "Currently Employeed"},
            {2, "Maternity Leave"}
        };

        public readonly Dictionary<int, string> JobTitles = new Dictionary<int, string>
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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }

        public override string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public int Title { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "Flex Time")]
        public double FlexTime { get; set; }

        [Display(Name = "Vacation Time")]
        public double VacationTime { get; set; }

        public int Status { get; set; }

        public List<EmployeePay> EmployeePays { get; set; }
        public List<Credential> Credentials { get; set; }
        public List<Signature> Signatures { get; set; }
        public List<ProjectEmployee> ProjectEmployees { get; set; }
        public List<WorkPackageEmployee> WorkPackageEmployees { get; set; }
        public List<Timesheet> Timesheets { get; set; }

        [Display(Name = "Approver ID")]
        public int? ApproverId { get; set; }
        public Approver Approver { get; set; }

        [Display(Name = "Supervisor ID")]
        public int? SupervisorId { get; set; }
        public Supervisor Supervisor { get; set; }
    }
}