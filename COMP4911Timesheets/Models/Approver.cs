using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Approver
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Status { get; set; }

        public int ApprovrId { get; set; }
        public Employee Approvr { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
