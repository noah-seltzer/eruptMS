using COMP4911Timesheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP4911Timesheets.ViewModels
{
    public class NewProject
    {

        public int ProjectId { get; set; }

        public string ProjectCode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProjectManager { get; set; } 

    }
}
