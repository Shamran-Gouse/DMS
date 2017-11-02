using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using DMS.Models;

namespace DMS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["User_Name"] != null & Session["Password"] != null)
            {
                return View();
            }
            else
                return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[User] WHERE Name = @Name AND Password = @Password AND User_Role = @User_Role", con))
                    {
                        cmd.Parameters.AddWithValue("@Name", user.Name);
                        cmd.Parameters.AddWithValue("@Password", user.Password);
                        cmd.Parameters.AddWithValue("@User_Role", user.User_Role);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Session["User_ID"] = reader[0].ToString();
                                    Session["User_Name"] = reader[1].ToString();
                                    Session["Email"] = reader[2].ToString();
                                    Session["Password"] = reader[3].ToString();
                                    Session["User_Role"] = reader[4].ToString();
                                }

                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ModelState.AddModelError("LogOnError", "Please provide valid entries for fields.");
                                return View();
                            }
                        }
                    }

                }

                
            }
            catch
            {
                ModelState.AddModelError("LogOnError", "Please provide valid entries for fields.");
                return View();
            }
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}