using System;
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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkPackageEmployeeId { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }

        public int WorkPackageId { get; set; }
        public WorkPackage WorkPackage { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
