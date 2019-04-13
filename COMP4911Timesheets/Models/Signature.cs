using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Signature
    {

        public static readonly int INVALID = 0;
        public static readonly int VALID = 1;

        public static readonly Dictionary<int, string> Statuses = new Dictionary<int, string>
        {
            {0, "Invalid"},
            {1, "Valid"}
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Signature ID")]
        public int SignatureId { get; set; }

        [Display(Name = "Pass Phrase")]
        public string PassPhrase { get; set; }

        [Display(Name = "Hashed Signature")]
        public string HashedSignature { get; set; }

        [Display(Name = "Created Time")]
        public DateTime CreatedTime { get; set; }

        public int Status { get; set; }

        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        List<Timesheet> Timesheets { get; set; }
    }
}
