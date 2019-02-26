using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Credential
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Credential ID")]
        public int CredentialId { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }

        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}