using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace COMP4911Timesheets.Models
{
    public class ParentWorkPackage
    {
        [Key]
        public int ParentWorkPckageId { get; set; }
        public int Status { get; set; }

        public List<WorkPackage> WorkPackages { get; set; }
    }
}
