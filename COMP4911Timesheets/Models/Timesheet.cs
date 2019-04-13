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

        public static readonly Dictionary<int, string> Statuses = new Dictionary<int, string>
        {
            {0, "Invalid"},
            {1, "Not Submitted & Not Approved"},
            {2, "Submitted & Not Approved"},
            {3, "Submitted & Approved"},
            {4, "Rejected & Need Resubmission"}
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Timesheet ID")]
        public int TimesheetId { get; set; }

        [Display(Name = "Week Ending")]
        public DateTime WeekEnding { get; set; }

        [Display(Name = "Week Number")]
        public int WeekNumber { get; set; }

        [Display(Name = "Flex Time")]
        public double FlexTime { get; set; }

        public int Status { get; set; }

        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Display(Name = "Employee Pay ID")]
        public int? EmployeePayId { get; set; }
        public EmployeePay EmployeePay { get; set; }

        [Display(Name = "Signature ID")]
        public int? SignatureId { get; set; }
        public Signature Signature { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        public List<TimesheetRow> TimesheetRows { get; set; }
    }
}
