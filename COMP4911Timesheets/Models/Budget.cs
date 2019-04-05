using System.Collections.Generic;
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

        public static readonly Dictionary<int, string> Statuses = new Dictionary<int, string>
        {
            {0, "Invalid"},
            {1, "valid"}
        };

        public static readonly Dictionary<int, string> Types = new Dictionary<int, string>
        {
            {10, "Estimates"},
            {20, "Actuals"}
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Budget ID")]
        public int BudgetId { get; set; }

        public double Hour { get; set; }

        public double REHour { get; set; }

        public int Status { get; set; }

        [Display(Name = "Week Number")]
        public int WeekNumber { get; set; }

        public int Type { get; set; }

        [Display(Name = "Work Package ID")]
        public int? WorkPackageId { get; set; }
        public WorkPackage WorkPackage { get; set; }

        [Display(Name = "Pay Grade ID")]
        public int? PayGradeId { get; set; }
        public PayGrade PayGrade { get; set; }
    }
}
