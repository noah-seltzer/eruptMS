using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class ProjectReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectReportId { get; set; }
        public double StartingPercentage { get; set; }
        public double CompletedPercentage { get; set; }
        public DateTime CreatedTime { get; set; }
        public int Status { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
