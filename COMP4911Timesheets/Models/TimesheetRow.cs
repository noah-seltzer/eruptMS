using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class TimesheetRow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TimesheetRowId { get; set; }
        public double SatHour { get; set; }
        public double SunHour { get; set; }
        public double MonHour { get; set; }
        public double TueHour { get; set; }
        public double WedHour { get; set; }
        public double ThuHour { get; set; }
        public double FriHour { get; set; }
        public string Notes { get; set; }

        public int TimesheetId { get; set; }
        public Timesheet Timesheet { get; set; }

        public int WorkPackageId { get; set; }
        public WorkPackage WorkPackage { get; set; }
    }
}
