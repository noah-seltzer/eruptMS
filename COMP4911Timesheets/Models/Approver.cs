using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace COMP4911Timesheets.Models
{
    public class Approver
    {
        [Key]
        public int ApproverId { get; set; }
        public int Status { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
