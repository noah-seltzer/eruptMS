using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class Signature
    {

        public static readonly int INVALID = 0;
        public static readonly int VALID = 1;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SignatureId { get; set; }
        public string PassPhrase { get; set; }
        public string HashedSignature { get; set; }
        public DateTime CreatedTime { get; set; }
        public int Status { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
