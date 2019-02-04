using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Supervisor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Status { get; set; }

        public int SupervsrId { get; set; }
        public Employee Supervsr { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
