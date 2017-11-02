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
    public class User
    {
        public int UID { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Please enter User Name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter User Email.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter Password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("User Role")]
        [Required(ErrorMessage = "Please enter User Role.")]
        public string User_Role { get; set; }
    }

    public class UserRole
    {
        public static List<SelectListItem> GetUserRoles()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Admin", Value = "Admin" });
            list.Add(new SelectListItem() { Text = "PM", Value = "PM" });
            list.Add(new SelectListItem() { Text = "ML", Value = "ML" });
            list.Add(new SelectListItem() { Text = "Developer", Value = "Developer" });
            list.Add(new SelectListItem() { Text = "QA", Value = "QA" });


            return list;

        }

        public static List<SelectListItem> GetUserByRole(string role)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT UID,Name FROM [dbo].[User] WHERE User_Role = '" + role + "' ", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            list.Add(new SelectListItem() { Text = reader[1].ToString(), Value = reader[0].ToString() });

                        }
                    }
                }

            }

            return list;
        }

        public static string GetUserNameById(int UID)
        {
            string Username = "";

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT Name FROM [dbo].[User] WHERE UID = '" + UID + "' ", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Username = reader[0].ToString();
                            }

                            return Username;
                        }
                        else
                            return "";

                    }
                }

            }
        }

    }
}