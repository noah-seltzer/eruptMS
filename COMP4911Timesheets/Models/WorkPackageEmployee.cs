using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class WorkPackageEmployee
    {
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
