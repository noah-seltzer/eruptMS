using COMP4911Timesheets.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace COMP4911Timesheets.ViewModels
{
    public class EmployeeManagement
    {
        public List<int> Role { get; set; }
        public Employee Employee { get; set; }
        public EmployeePay EmployeePay { get; set; }
        [DisplayName("Pass Phrase")]
        public string passPhrase { get; set; }
    }
}
