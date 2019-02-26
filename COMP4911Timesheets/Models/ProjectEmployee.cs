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

        public readonly Dictionary<int, string> Statuses = new Dictionary<int, string>
        {
            {0, "Not Working"},
            {1, "Currently Working"},
            {2, "Pending"}
        };

        public readonly Dictionary<int, string> Roles = new Dictionary<int, string>
        {
            {100, "Project Manager"},
            {110, "Project Assistant"}
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Project Employee ID")]
        public int ProjectEmployeeId { get; set; }
        public int Status { get; set; }
        public int Role { get; set; }

        [Display(Name = "Project ID")]
        public string ProjectId { get; set; }
        public Project Project { get; set; }

        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
