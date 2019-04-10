using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class WorkPackage
    {
        public static readonly int INVALID = 0;
        public static readonly int VALID = 1;
        public static readonly int OPENED = 2;
        public static readonly int CLOSED = 3;
        public static readonly int ARCHIVED = 4;

        public static readonly Dictionary<int, string> Statuses = new Dictionary<int, string> {
            {0, "Invalid"},
            {1, "Valid"},
            {2, "Opened"},
            {3, "Closed"},
            {4, "Archived"}
        };

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
        public int Status { get; set; }

        [Display(Name = "Project ID")]
        public int? ProjectId { get; set; }
        public Project Project { get; set; }

        [ForeignKey("ParentWorkPackage")]
        public int? ParentWorkPackageId { get; set; }
        [Display(Name = "Parent WP")]
        public WorkPackage ParentWorkPackage { get; set; }

        [InverseProperty("ParentWorkPackage")]
        public List<WorkPackage> ChildWorkPackages { get; set; }

        public List<Budget> Budgets { get; set; }
        public List<WorkPackageReport> WorkPackageReports { get; set; }
        public List<ResponsibleEngineerReport> ResponsibleEngineerReports { get; set; }
        public List<TimesheetRow> TimesheetRows { get; set; }
        public List<ProjectEmployee> ProjectEmployees { get; set; }
    }
}
