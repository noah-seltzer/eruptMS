using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP4911Timesheets.Models
{
    public class PayGrade
    {

        public static readonly string P1 = "P1";
        public static readonly string P2 = "P2";
        public static readonly string P3 = "P3";
        public static readonly string P4 = "P4";
        public static readonly string P5 = "P5";
        public static readonly string P6 = "P6";
        public static readonly string SS = "SS";
        public static readonly string JS = "JS";
        public static readonly string DS = "DS";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PayGradeId { get; set; }
        public string PayLevel { get; set; }
        public double Cost { get; set; }
        public int Year { get; set; }

        public List<EmployeePay> EmployeePays { get; set; }
        public List<Budget> Budgets { get; set; }
    }
}