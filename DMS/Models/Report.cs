using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS.Models
{
    public class Report
    {
       
        [Required(ErrorMessage = "Please enter Project ID.")]
        public string projectID { get; set; }
        public int QA_ID { get; set; }

        public int ClientIdendtifyDefect { get; set; }

        public int weak { get; set; }

        public string MID { get; set; }

        public string RCA { get; set; }

        public string Status { get; set; }
        
    }
    
    public class ReportAge {
        public static List<SelectListItem> GetWeak()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "1 Weak", Value = "-7" });
            list.Add(new SelectListItem() { Text = "2 Weak", Value = "-14" });
            list.Add(new SelectListItem() { Text = "3 Weak", Value = "-21" });
            list.Add(new SelectListItem() { Text = "4 Weak", Value = "-28" });
            list.Add(new SelectListItem() { Text = "More than 4 Weak", Value = "-100" });


            return list;

        }


    }

    public class ReportRCA
    {
        public static List<SelectListItem> GetRCA()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "1 Weak", Value = "-7" });
            list.Add(new SelectListItem() { Text = "2 Weak", Value = "-14" });
            list.Add(new SelectListItem() { Text = "3 Weak", Value = "-21" });
            list.Add(new SelectListItem() { Text = "4 Weak", Value = "-28" });
            list.Add(new SelectListItem() { Text = "More than 4 Weak", Value = "-100" });


            return list;

        }

    }
}