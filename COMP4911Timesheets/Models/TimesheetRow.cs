using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class TimesheetRow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Timesheet Row ID")]
        public int TimesheetRowId { get; set; }

        [Display(Name = "Saturday")]
        public double SatHour { get; set; }

        [Display(Name = "Sunday")]
        public double SunHour { get; set; }

        [Display(Name = "Monday")]
        public double MonHour { get; set; }

        [Display(Name = "Tuesday")]
        public double TueHour { get; set; }

        [Display(Name = "Wednesday")]
        public double WedHour { get; set; }

        [Display(Name = "Thursday")]
        public double ThuHour { get; set; }

        [Display(Name = "Friday")]
        public double FriHour { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Timesheet ID")]
        public int? TimesheetId { get; set; }
        public Timesheet Timesheet { get; set; }

        [Display(Name = "Work Package ID")]
        public int? WorkPackageId { get; set; }
        public WorkPackage WorkPackage { get; set; }
    }
}
