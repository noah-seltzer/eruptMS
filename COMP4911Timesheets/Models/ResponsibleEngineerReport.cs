using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class ResponsibleEngineerReport
    {
        public static readonly int INVALID = 0;
        public static readonly int VALID = 1;

        public static readonly Dictionary<int, string> Statuses = new Dictionary<int, string>
        {
            {0, "Invalid"},
            {1, "Valid"}
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Responsible Engineer Report ID")]
        public int ResponsibleEngineerReportId { get; set; }

        [Display(Name = "Week Number")]
        public int WeekNumber { get; set; }

        public int Status { get; set; }

        public string Comments { get; set; }

        public string ResponsibleEngineerEstimate { get; set; }

        [Display(Name = "Starting Percentage")]
        public double StartingPercentage { get; set; }

        [Display(Name = "Completed Percentage")]
        public double CompletedPercentage { get; set; }

        [Display(Name = "Cost Started")]
        public double CostStarted { get; set; }

        [Display(Name = "Cost Finished")]
        public double CostFinished { get; set; }

        [Display(Name = "Work Accomplished This Period")]
        public string WorkAccomplished { get; set; }

        [Display(Name = "Work Planned For Next Period")]
        public string WorkPlanned { get; set; }

        [Display(Name = "Problems This Period")]
        public string Problem { get; set; }

        [Display(Name = "Problems Anticipated")]
        public string ProblemAnticipated { get; set; }

        [Display(Name = "Work Package ID")]
        public int? WorkPackageId { get; set; }
        public WorkPackage WorkPackage { get; set; }
    }
}
