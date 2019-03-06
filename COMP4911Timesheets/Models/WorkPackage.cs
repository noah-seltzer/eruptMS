using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class WorkPackage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Work Package ID")]
        public int WorkPackageId { get; set; }

        [Display(Name = "Work Package")]
        public string WorkPackageCode { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Contractor { get; set; }
        public string Purpose { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string Activity { get; set; }
        public bool IsParent { get; set; }

        [Display(Name = "Project ID")]
        public string ProjectId { get; set; }
        public Project Project { get; set; }

        [Display(Name = "Parent Work Package ID")]
        public int? ParentWorkPackageId { get; set; }
        public ParentWorkPackage ParentWorkPackage { get; set; }

        public List<Budget> Budgets { get; set; }
        public List<WorkPackageReport> WorkPackageReports { get; set; }
        public List<WorkPackageEmployee> WorkPackageEmployees { get; set; }
        public List<TimesheetRow> TimesheetRows { get; set; }
    }
}
