using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS.Models
{
    public class SubModule
    {
        [DisplayName("Project ID")]
        public string PID { get; set; }

        [DisplayName("Module ID")]
        public string MID { get; set; }

        [DisplayName("Sub Module ID")]
        public string SMID { get; set; }

        [DisplayName("Sub Module Name")]
        [Required(ErrorMessage = "Please enter Sub Module Name.")]
        public string Name { get; set; }

        [DisplayName("Developer")]
        [Required(ErrorMessage = "Please enter Developer.")]
        public int Developer { get; set; }
    }
}