using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS.Models
{
    public class TestCase
    {
        [DisplayName("Project ID")]
        [Required(ErrorMessage = "Please enter Project ID.")]
        public string PID { get; set; }

        [DisplayName("Module ID")]
        [Required(ErrorMessage = "Please enter Module ID.")]
        public string MID { get; set; }

        [DisplayName("Sub Module ID")]
        [Required(ErrorMessage = "Please enter Sub Module ID.")]
        public string SMID { get; set; }

        [DisplayName("Test Case ID")]
        [Required(ErrorMessage = "Please enter Test Case ID.")]
        public string TCID { get; set; }

        [DisplayName("Test Case Name")]
        [Required(ErrorMessage = "Please enter Test Case Title.")]
        public string Title { get; set; }

        public string Status { get; set; }

        [DisplayName("Test By")]
        public int TestBy { get; set; }

    }
}