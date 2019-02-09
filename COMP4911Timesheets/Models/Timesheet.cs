using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Timesheet
    {

        public static readonly int INVALID = 0;
        public static readonly int NOT_SUBMITTED_NOT_APPROVED = 1;
        public static readonly int SUBMITTED_NOT_APPROVED = 2;
        public static readonly int SUBMITTED_APPROVED = 3;
        public static readonly int REJECTED_NEED_RESUBMISSION = 4;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TimesheetId { get; set; }
        public DateTime WeekEnding { get; set; }
        public int WeekNumber { get; set; }
        public string ESignature { get; set; }
        public double FlexTime { get; set; }
        public int Status { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int? EmployeePayId { get; set; }
        public EmployeePay EmployeePay { get; set; }

        public List<TimesheetRow> TimesheetRows { get; set; }
    }
}
