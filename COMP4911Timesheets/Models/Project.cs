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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double CostingProposal { get; set; }
        public double OriginalBudget { get; set; }
        public int Status { get; set; }

        public List<ProjectReport> ProjectReports { get; set; }
        public List<ProjectEmployee> ProjectEmployees { get; set; }
        public List<WorkPackage> WorkPackages { get; set; }
    }
}
