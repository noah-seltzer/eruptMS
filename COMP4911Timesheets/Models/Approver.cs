using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Approver
    {
        public static readonly int INVALID = 0;
        public static readonly int VALID = 1;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ApproverId { get; set; }
        public int Status { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
