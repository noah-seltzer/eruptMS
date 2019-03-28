using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Project
    {

        public static readonly int INVALID = 0;
        public static readonly int ONGOING = 1;
        public static readonly int INTERNAL = 2;
        public static readonly int PAUSED = 3;

        public readonly Dictionary<int, string> Statuses = new Dictionary<int, string>
        {
            {0, "Invalid"},
            {1, "Ongoing"},
            {2, "Internal"},
            {3, "Paused"}
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Project ID")]
        public int ProjectId { get; set; }

        [Display(Name = "Project Code")]
        public string ProjectCode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Costing Proposal")]
        public double CostingProposal { get; set; }

        [Display(Name = "Original Budget")]
        public double OriginalBudget { get; set; }

        public int Status { get; set; }

        public List<ProjectReport> ProjectReports { get; set; }
        public List<ProjectEmployee> ProjectEmployees { get; set; }
        public List<WorkPackage> WorkPackages { get; set; }
        public List<ProjectRequest> ProjectRequests { get; set; }
    }
}
