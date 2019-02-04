using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CostingProposal { get; set; }
        public string OriginalBudget { get; set; }
        public string CostAtImplementation { get; set; }
        public int Status { get; set; }

        public List<ProjectReport> ProjectReports { get; set; }
        public List<ProjectEmployee> ProjectEmployees { get; set; }
        public List<WorkPackage> WorkPackages { get; set; }
    }
}
