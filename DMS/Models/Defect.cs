using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS.Models
{
    public class Defect : TestCase
    {
        public int Row_id { get; set; }

        [DisplayName("Defect Id")]
        public string Defect_id { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Please enter Description.")]
        public string Description { get; set; }

        [DisplayName("Open Date")]
        [DataType(DataType.Date)]
        public DateTime OpenDate { get; set; }

        [DisplayName("Close Date")]
        [DataType(DataType.Date)]
        public string CloseDate { get; set; }

        [DisplayName("Owner")]
        public string Owner { get; set; }

        [DisplayName("Assigned To")]
        [Required(ErrorMessage = "Please Assign an Owner.")]
        public int Assigned_to { get; set; }

        [DisplayName("Status")]
        public string Defect_Status { get; set; }

        [DisplayName("RCA Result")]
        // [Required(ErrorMessage = "Please enter Project Name.")]
        public string RCA { get; set; }

        [DisplayName("Defect Count")] // count 
        public int Defect_Count { get; set; }

        [DisplayName("Piority")] // count 
        public string Piority { get; set; }

        [DisplayName("Severity")] // count 
        public string Severity { get; set; }
    }

    public class DefectMethods
    {
        public static List<SelectListItem> GetOwners()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "MODULE LEAD", Value = "MODULE LEAD" });
            list.Add(new SelectListItem() { Text = "DEVELOPER", Value = "DEVELOPER" });
            list.Add(new SelectListItem() { Text = "QA", Value = "QA" });

            return list;
        }

        public static List<SelectListItem> GetStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "ASSIGNED", Value = "ASSIGNED" });
            list.Add(new SelectListItem() { Text = "REASSIGN", Value = "REASSIGN" });
            list.Add(new SelectListItem() { Text = "CLOSED", Value = "CLOSED" });

            return list;
        }

        public static List<SelectListItem> GetRCAresult()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Error in Rquirements", Value = "Requirement Error" });
            list.Add(new SelectListItem() { Text = "Error in Design", Value = "Design Error" });
            list.Add(new SelectListItem() { Text = "Error in Coding", Value = "Coding Error" });
            list.Add(new SelectListItem() { Text = "Error in Testing", Value = "Tessing Error" });
            list.Add(new SelectListItem() { Text = "Other", Value = "Other" });

            return list;
        }

        public static List<SelectListItem> Piority()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "LOW", Value = "LOW" });
            list.Add(new SelectListItem() { Text = "MEDIUM", Value = "MEDIUM" });
            list.Add(new SelectListItem() { Text = "HIGH", Value = "HIGH" });
            list.Add(new SelectListItem() { Text = "IMMEDIATE", Value = "IMMEDIATE" });

            return list;

        }

        public static List<SelectListItem> Severity()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "LOW", Value = "LOW" });
            list.Add(new SelectListItem() { Text = "MINOR", Value = "MINOR" });
            list.Add(new SelectListItem() { Text = "MAJOR", Value = "MAJOR" });
            list.Add(new SelectListItem() { Text = "CRITICAL", Value = "CRITICAL" });

            return list;

        }

    }

}