using COMP4911Timesheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP4911Timesheets.ViewModels
{
    public class EmployeeManagement
    {
        public int Role { get; set; }
        public Employee Employee { get; set; }
        public EmployeePay EmployeePay { get; set; }
    }
}
