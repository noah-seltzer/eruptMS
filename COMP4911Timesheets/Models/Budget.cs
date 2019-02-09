using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Budget
    {

        public static readonly int INVALID = 0;
        public static readonly int VALID = 1;
        public static readonly int ESTIMATE = 10;
        public static readonly int ACTUAL = 20;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BudgetId { get; set; }
        public double Hour { get; set; }
        public int Status { get; set; }
        public int WeekNumber { get; set; }
        public int Type { get; set; }

        public int WorkPackageId { get; set; }
        public WorkPackage WorkPackage { get; set; }

        public int PayGradeId { get; set; }
        public PayGrade PayGrade { get; set; }
    }
}
