using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS.Models
{
    public class TestCaseSteps : TestCase
    {
        [DisplayName("Step ID")]
        [Required(ErrorMessage = "Please enter Step ID.")]
        public int SID { get; set; }

        [DisplayName("Step")]
        [Required(ErrorMessage = "Please enter Step.")]
        public string Step { get; set; }

        [DisplayName("Step Data")]
        [Required(ErrorMessage = "Please enter Step Data.")]
        public string StepData { get; set; }

        [DisplayName("Expected Result")]
        [Required(ErrorMessage = "Please enter Expected Result.")]
        public string ExpectedResult { get; set; }
    }
}