using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP4911Timesheets.Models
{
    public class ProjectSumReport
    {
        public int WorkPackageCode { get; set; }
        public string WorkPackageName { get; set; }
        public double AHour { get; set; }
        public double EHour { get; set; }
        public int Variance { get; set; }
        public string Comment { get; set; }

    }
}
