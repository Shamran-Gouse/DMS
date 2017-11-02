using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS.Models
{
    public class Module
    {
        [DisplayName("Project ID")]
        public string PID { get; set; }

        [DisplayName("Module ID")]
        public string MID { get; set; }

        [DisplayName("Module Name")]
        [Required(ErrorMessage = "Please enter Module Name.")]
        public string Name { get; set; }

        [DisplayName("Module Leader")]
        [Required(ErrorMessage = "Please enter Module Leader.")]
        public int ModuleLead { get; set; }

    }
}