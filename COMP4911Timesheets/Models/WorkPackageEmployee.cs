using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class WorkPackageEmployee
    {

        public static readonly int NOT_WORKING = 0;
        public static readonly int CURRENTLY_WORKING = 1;
        public static readonly int PENDING = 2;
        public static readonly int RESPONSIBLE_ENGINEER = 100;
        public static readonly int ASSIGNED_EMPLOYEE = 200;

        public readonly Dictionary<int, string> Statuses = new Dictionary<int, string>
        {
            {0, "Not Working"},
            {1, "Currently Working"},
            {2, "Pending"}
        };

        public readonly Dictionary<int, string> Roles = new Dictionary<int, string>
        {
            {100, "Responsible Engineer"},
            {200, "Assigned Employee"}
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Work Package Employee ID")]
        public int WorkPackageEmployeeId { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }

        [Display(Name = "Work Package ID")]
        public int? WorkPackageId { get; set; }
        public WorkPackage WorkPackage { get; set; }

        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
