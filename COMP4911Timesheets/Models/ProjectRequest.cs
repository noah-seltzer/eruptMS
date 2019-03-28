using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class ProjectRequest
    {
        public static readonly int VALID = 1;
        public static readonly int INVALID = 0;

        public static readonly Dictionary<int, string> Statuses = new Dictionary<int, string> {
            {0, "Invalid"},
            {1, "Valid"}
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Project Request ID")]
        public int ProjectRequestId { get; set; }

        [Display(Name = "# of Request")]
        public int AmountRequested { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Project ID")]
        public int? ProjectId { get; set; }
        public Project Project { get; set; }

        [Display(Name = "Pay Grade ID")]
        public int? PayGradeId { get; set; }
        public PayGrade PayGrade { get; set; }
    }
}