using COMP4911Timesheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP4911Timesheets.ViewModels
{
    public class ProjectListingModel
    {
        public List<Project> managedProjects  { get; set; }
        public List<Project> assignedProjects { get; set; }
    }
}
