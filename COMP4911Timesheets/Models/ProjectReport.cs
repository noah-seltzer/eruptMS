using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class ProjectReport
    {
        public static readonly int INVALID = 0;
        public static readonly int VALID = 1;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectReportId { get; set; }
        public double StartingPercentage { get; set; }
        public double CompletedPercentage { get; set; }
        public double CostStarted { get; set; }
        public double CostFinished { get; set; }
        public DateTime CreatedTime { get; set; }
        public int Status { get; set; }

        public string ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
