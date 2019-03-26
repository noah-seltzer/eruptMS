using COMP4911Timesheets.Models;

namespace COMP4911Timesheets.ViewModels
{
    public class LineManagerManagement
    {
        public Employee Employee { get; set; }
        public Project Project { get; set; }
        public ProjectEmployee ProjectEmployee { get; set; }
        public Timesheet Timesheet { get; set; }
    }
}