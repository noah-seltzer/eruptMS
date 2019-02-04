using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Budget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BudgetId { get; set; }
        public double Hour { get; set; }
        public int Status { get; set; }

        public int Type { get; set; }

        public int WorkPacakgeId { get; set; }
        public WorkPackage WorkPackage { get; set; }

        public int PayGradeId { get; set; }
        public PayGrade PayGrade { get; set; }
    }
}
