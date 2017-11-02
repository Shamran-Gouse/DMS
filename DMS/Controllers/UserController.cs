using DMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            if ((string)Session["User_Role"] == "Admin")
            {
                //return View();
                return RedirectToAction("Details");
            }
            else
                return RedirectToAction("Login", "Home");
        }

        // GET: Default/Details
        public ActionResult Details()
        {
            if ((string)Session["User_Role"] == "Admin")
            {
                DataTable DT = new DataTable();

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM [dbo].[User]", con))
                        {
                            sqlDA.Fill(DT);
                        }
                    }

                    return View(DT);
                }
                catch
                {

                    return Content("Something went wrong in the details page!");
                }
            }
            else
                return RedirectToAction("Login", "Home");
        }

        // GET: User/Create
        public ActionResult Create()
        {
            if ((string)Session["User_Role"] == "Admin")
            {
                return View(new User());
            }
            else
                return RedirectToAction("Login", "Home");
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {

            if (!string.IsNullOrEmpty(user.User_Role))
            {
                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[User] VALUES(@UID,@Name,@Email,@Password,@User_Role)", con))
                        {
                            cmd.Parameters.AddWithValue("@UID", user.UID);
                            cmd.Parameters.AddWithValue("@Name", user.Name);
                            cmd.Parameters.AddWithValue("@Email", user.Email);
                            cmd.Parameters.AddWithValue("@Password", user.Password);
                            cmd.Parameters.AddWithValue("@User_Role", user.User_Role);
                            cmd.ExecuteNonQuery();
                        }

                    }

                    return RedirectToAction("Details");
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("UserError", "Please select a User Role.");
                return View();
            }
            
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            if ((string)Session["User_Role"] == "Admin")
            {
                User user = new User();
                DataTable DT = new DataTable();

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM [dbo].[User] WHERE UID = @UID", con))
                        {
                            sqlDA.SelectCommand.Parameters.AddWithValue("@UID", id);
                            sqlDA.Fill(DT);
                        }
                    }

                    if (DT.Rows.Count == 1)
                    {
                        user.UID = Convert.ToInt32(DT.Rows[0][0].ToString());
                        user.Name = DT.Rows[0][1].ToString();
                        user.Email = DT.Rows[0][2].ToString();
                        user.Password = DT.Rows[0][3].ToString();
                        user.User_Role = DT.Rows[0][4].ToString();

                        return View(user);
                    }
                    else
                    {
                        return RedirectToAction("Details");
                    }
                }
                catch
                {

                    return RedirectToAction("Details");
                }
            }
            else
                return RedirectToAction("Login", "Home");
            
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, User user)
        {
            if (!string.IsNullOrEmpty(user.User_Role))
            {
                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[User] SET Name = @Name, Email = @Email, User_Role = @User_Role WHERE UID = @UID", con))
                        {
                            cmd.Parameters.AddWithValue("@UID", id);
                            cmd.Parameters.AddWithValue("@Name", user.Name);
                            cmd.Parameters.AddWithValue("@Email", user.Email);
                            cmd.Parameters.AddWithValue("@User_Role", user.User_Role);
                            cmd.ExecuteNonQuery();
                        }

                    }

                    return RedirectToAction("Details");
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("UserError", "Please select a User Role.");
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            if ((string)Session["User_Role"] == "Admin")
            {

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[User] WHERE UID = @UID", con))
                    {
                        cmd.Parameters.AddWithValue("@UID", id);
                        cmd.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("Details");
            }
            else
                return RedirectToAction("Login", "Home");
        }
    }
}
