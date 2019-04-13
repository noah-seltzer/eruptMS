using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class ProjectReport
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
        [Display(Name = "Project Report ID")]
        public int ProjectReportId { get; set; }

        [Display(Name = "Starting Percentage")]
        public double StartingPercentage { get; set; }

        [Display(Name = "Completed Percentage")]
        public double CompletedPercentage { get; set; }

        [Display(Name = "Cost Started")]
        public double CostStarted { get; set; }

        [Display(Name = "Cost Finished")]
        public double CostFinished { get; set; }

        [Display(Name = "Created Time")]
        public DateTime CreatedTime { get; set; }

        public int Status { get; set; }

        [Display(Name = "Project ID")]
        public int? ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
