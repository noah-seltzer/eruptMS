using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP4911Timesheets.Models
{
    public class ApplicationRole : IdentityRole
    {

        public static readonly string AD = "AD"; // Admin
        public static readonly string TA = "TA"; // Timesheet Approver
        public static readonly string HR = "HR"; // Human Resources
        public static readonly string PM = "PM"; // Project Manager
        public static readonly string PA = "PA"; // Project Assistant
        public static readonly string LM = "LM"; // Line Manager
        public static readonly string RE = "RE"; // Responsible Engineer
        public static readonly string EM = "EM"; // Employee

        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }

        public ApplicationRole(string roleName, string description,
            DateTime createdDate)
            : base(roleName)
        {
            base.Name = roleName;

            this.Description = description;
            this.CreatedDate = createdDate;
        }

        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
