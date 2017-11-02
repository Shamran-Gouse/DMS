using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DMS.Models;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace DMS.Controllers
{
    public class ModuleController : Controller
    {
        // GET: Module/P0001
        public ActionResult Index(string id)
        {
            if ((string)Session["User_Role"] == "PM")
            {
                DataTable DT = new DataTable();

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM [dbo].[Module] WHERE PID = '" + id + "'", con))
                    {
                        sqlDA.Fill(DT);
                    }
                }

                return View(DT);
            }
            else
                return RedirectToAction("Login", "Home");
        }

        // GET: Module/Create/P0001
        public ActionResult Create(string id)
        {
            if ((string)Session["User_Role"] == "PM")
            {
                Module module = new Module();
                module.PID = id;

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("SELECT MAX(MID) FROM [dbo].[Module] WHERE PID = '" + id + "' ", con))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        string mid = reader[0].ToString();

                                        if (string.IsNullOrEmpty(mid))
                                        {
                                            module.MID = "M" + "001";
                                        }
                                        else
                                        {
                                            var prefix = Regex.Match(mid, "^\\D+").Value;
                                            var number = Regex.Replace(mid, "^\\D+", "");
                                            var i = int.Parse(number) + 1;
                                            module.MID = prefix + i.ToString(new string('0', number.Length));
                                        }
                                    }
                                }
                                else
                                {
                                    module.MID = "M" + "001";
                                }
                            }
                        }
                    }

                    return View(module);

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("CreateError", ex.Message);
                    return View();
                }

            }
            else
                return RedirectToAction("Login", "Home");
        }

        // POST: Module/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string id, Module module)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("INSERT INTO Module VALUES (@PID,@MID,@Name,@Module_Lead)", con))
                        {

                            cmd.Parameters.AddWithValue("@PID", module.PID);
                            cmd.Parameters.AddWithValue("@MID", module.MID);
                            cmd.Parameters.AddWithValue("@Name", module.Name);
                            cmd.Parameters.AddWithValue("@Module_Lead", module.ModuleLead);
                            cmd.ExecuteNonQuery();
                        }

                    }

                    return RedirectToAction("Index", "Module", new { id = module.PID });
                }
                catch(Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            else
            {
                return View("Create", module);
            }

        }

        // GET: Module/Edit/M0001
        public ActionResult Edit(string Pid, string Mid)
        {
            if ((string)Session["User_Role"] == "PM")
            {

                Module module = new Module();
                DataTable DT = new DataTable();

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM Module WHERE MID = @MID AND PID = @PID", con))
                        {
                            sqlDA.SelectCommand.Parameters.AddWithValue("@MID", Mid);
                            sqlDA.SelectCommand.Parameters.AddWithValue("@PID", Pid);
                            sqlDA.Fill(DT);
                        }
                    }

                    if (DT.Rows.Count == 1)
                    {
                        module.PID = DT.Rows[0][0].ToString();
                        module.MID = DT.Rows[0][1].ToString();
                        module.Name = DT.Rows[0][2].ToString();
                        module.ModuleLead = Convert.ToInt32(DT.Rows[0][3].ToString());

                        return View(module);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Module", new { id = module.PID });
                    }
                }
                catch
                {

                    return RedirectToAction("Index", "Module", new { id = module.PID });
                }
            }
            else
                return RedirectToAction("Login", "Home");
        }

        // POST: Module/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Pid,string Mid, Module module)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("UPDATE Module SET Name = @Name, Module_Lead = @Module_Lead WHERE MID = @MID AND PID = @PID", con))
                        {
                            cmd.Parameters.AddWithValue("@MID", Mid);
                            cmd.Parameters.AddWithValue("@PID", Pid);
                            cmd.Parameters.AddWithValue("@Name", module.Name);
                            cmd.Parameters.AddWithValue("@Module_Lead", module.ModuleLead);
                            cmd.ExecuteNonQuery();
                        }

                    }

                    return RedirectToAction("Index", "Module", new { id = module.PID });
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return View("Edit", module);
            }
        }

        // GET: Module/Delete/5
        [Route("Module/{Pid}/{Mid}")]
        public ActionResult Delete(string Pid, string Mid)
        {
            if ((string)Session["User_Role"] == "PM")
            {

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Module WHERE MID = @MID AND PID = @PID", con))
                    {
                        cmd.Parameters.AddWithValue("@MID", Mid);
                        cmd.Parameters.AddWithValue("@PID", Pid);
                        cmd.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("Index", "Module", new { id = Pid });
            }
            else
                return RedirectToAction("Login", "Home");
        }

        public ActionResult Modules()
        {
            if ((string)Session["User_Role"] == "ML")
            {
                DataTable DT = new DataTable();

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    int UID = Convert.ToInt32(Session["User_ID"]);

                    using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM [dbo].[Module] WHERE Module_Lead = '" + UID + "'", con))
                    {
                        sqlDA.Fill(DT);
                    }
                }

                return View(DT);
            }
            else
                return RedirectToAction("Login", "Home");
    }
    }
}
