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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeePayId { get; set; }
        public DateTime AssignedDate { get; set; }
        public int Status { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int PayGradeId { get; set; }
        public PayGrade PayGrade { get; set; }

        public List<Timesheet> Timesheets { get; set; }
    }
}
