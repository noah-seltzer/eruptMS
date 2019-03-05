using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP4911Timesheets.Models
{
    public class ApplicationRole : IdentityRole
    {

        public static readonly string AD = "AD";
        public static readonly string TA = "TA";
        public static readonly string HR = "HR";
        public static readonly string PM = "PM";
        public static readonly string PA = "PA";
        public static readonly string LM = "LM";
        public static readonly string RE = "RE";
        public static readonly string EM = "EM";

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
