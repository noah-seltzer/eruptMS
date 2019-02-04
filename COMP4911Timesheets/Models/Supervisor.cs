using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace COMP4911Timesheets.Models
{
    public class Supervisor
    {
        [Key]
        public int SupervisorId { get; set; }
        public int Status { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
