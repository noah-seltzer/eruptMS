using COMP4911Timesheets.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace COMP4911Timesheets.ViewModels
{
    public class ManageProject
    {
        public Project project { get; set; }
        public List<ProjectRequest> requests{ get; set; }
        [Display(Name = "Project Manager")]
        public string projectManager { get; set; }
        [Display(Name = "Manager's Assistant")]
        public string managersAssistant { get; set; }
    }
}
