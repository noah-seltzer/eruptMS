using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class PayGrade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PayGradeId { get; set; }
        public string PayLevel { get; set; }
        public double Cost { get; set; }
        public int Year { get; set; }

        public List<EmployeePay> EmployeePays { get; set; }
        public List<Budget> Budgets { get; set; }
        public List<TimesheetRow> TimesheetRows { get; set; }
    }
}