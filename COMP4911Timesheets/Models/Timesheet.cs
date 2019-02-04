using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Timesheet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TimesheetId { get; set; }
        public DateTime WeekEnding { get; set; }
        public int WeekNumber { get; set; }
        public double FlexTime { get; set; }
        public int Status { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public List<TimesheetRow> TimesheetRows { get; set; }
    }
}
