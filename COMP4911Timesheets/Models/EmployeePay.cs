using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class EmployeePay
    {
        public static readonly int INVALID = 0;
        public static readonly int VALID = 1;

        public static readonly Dictionary<int, string> Statuses = new Dictionary<int, string>
        {
            {0, "Invalid"},
            {1, "Valid"}
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Employee Pay ID")]
        public int EmployeePayId { get; set; }

        [Display(Name = "Assigned Date")]
        public DateTime AssignedDate { get; set; }

        public int Status { get; set; }

        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Display(Name = "Pay Grade ID")]
        public int? PayGradeId { get; set; }
        public PayGrade PayGrade { get; set; }

        public List<Timesheet> Timesheets { get; set; }
    }
}
