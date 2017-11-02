using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS.Models
{
    public class Project
    {
        [DisplayName("Project ID")]
        public string PID { get; set; }

        [DisplayName("Project Name")]
        [Required(ErrorMessage = "Please enter Project Name.")]
        public string Project_Name { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please enter Project Start Date.")]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please enter Project End Date.")]
        public DateTime EndDate { get; set; }

        [DisplayName("PM ID")] // PM - Project Manager( who creates the Project)
        public int PMID { get; set; }
    }
}