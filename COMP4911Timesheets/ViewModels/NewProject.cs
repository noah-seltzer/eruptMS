using COMP4911Timesheets.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace COMP4911Timesheets.ViewModels
{
    public class NewProject
    {
        [Display(Name = "Project ID")]
        public int ProjectId { get; set; }

        [Display(Name = "Project Code")]
        public string ProjectCode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Project Manager")]
        public string ProjectManager { get; set; } 

        [Display(Name = "Manager's Assistant")]
        public string ManagersAssistant { get; set; }

        [Display(Name = "Markup Rate")]
        public double MarkupRate { get; set; }

    }
}
