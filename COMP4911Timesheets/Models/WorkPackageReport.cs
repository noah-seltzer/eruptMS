using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class WorkPackageReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkPackageReportId { get; set; }
        public int WeekNumber { get; set; }
        public int Status { get; set; }
        public string Comments { get; set; }
        public string WorkAccomplished { get; set; }
        public string WorkAccomplishedNP { get; set; }
        public string Problem { get; set; }
        public string ProblemAnticipated { get; set; }

        public int WorkPackageId { get; set; }
        public WorkPackage WorkPackage { get; set; }
    }
}
