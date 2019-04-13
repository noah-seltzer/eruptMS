using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class ProjectEmployee
    {

        public static readonly int NOT_WORKING = 0;
        public static readonly int CURRENTLY_WORKING = 1;
        public static readonly int PENDING = 2;
        public static readonly int PROJECT_MANAGER = 100;
        public static readonly int PROJECT_ASSISTANT = 110;
        public static readonly int RESPONSIBLE_ENGINEER = 200;
        public static readonly int EMPLOYEE = 300;
        public static readonly int NOT_ASSIGNED = 900;

        public static readonly Dictionary<int, string> Statuses = new Dictionary<int, string>
        {
            {0, "Not Working"},
            {1, "Currently Working"},
            {2, "Pending"}
        };

        public static readonly Dictionary<int, string> Roles = new Dictionary<int, string>
        {
            {100, "Project Manager"},
            {110, "Project Assistant"},
            {200, "Responsible Engineer"},
            {300, "Employee"},
            {900, "Not Assigned"}
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Project Employee ID")]
        public int ProjectEmployeeId { get; set; }
        public int Status { get; set; }
        public int Role { get; set; }

        [Display(Name = "Project ID")]
        public int? ProjectId { get; set; }
        public Project Project { get; set; }

        [Display(Name = "Work Package ID")]
        public int? WorkPackageId { get; set; }
        public WorkPackage WorkPackage { get; set; }

        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

    }
}
