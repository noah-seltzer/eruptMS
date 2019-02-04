using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class ParentWorkPackage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Status { get; set; }

        public int ParentWorkPckageId { get; set; }
        public WorkPackage ParentWorkPckage { get; set; }

        public int ChildWorkPackageId { get; set; }
        public WorkPackage ChildWorkPackage { get; set; }
    }
}
