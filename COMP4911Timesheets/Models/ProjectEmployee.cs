using System;
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
        public static readonly int RESPONSIBLE_ENGINEER = 300;
        public static readonly int ASSIGNED_EMPLOYEE = 400;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectEmployeeId { get; set; }
        public int Status { get; set; }
        public int Role { get; set; }

        public string ProjectId { get; set; }
        public Project Project { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
