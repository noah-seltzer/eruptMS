using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Employee
    {

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