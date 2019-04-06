using System.Collections.Generic;
using COMP4911Timesheets.Models;

namespace COMP4911Timesheets.ViewModels
{
    public class LineManagerManagement
    {
        public Employee Employee { get; set; }
        public Project Project { get; set; }
        public Timesheet Timesheet { get; set; }
        public ProjectRequest ProjectRequest { get; set; }
        public ProjectEmployee ProjectEmployee { get; set; }
        public EmployeePay EmployeePay { get; set; }
        public List<ProjectEmployee> ProjectEmployees { get; set; }
        public List<EmployeePay> EmployeePays { get; set; }
        public List<Employee> Employees { get; set; }
        public List<string> EmployeeIds { get; set; }
    }
}