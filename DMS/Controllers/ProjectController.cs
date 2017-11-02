using DMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace DMS.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult Index()
        {
            if ((string)Session["User_Role"] == "PM")
            {
                DataTable DT = new DataTable();
                int PMID = Convert.ToInt32(Session["User_ID"]);

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM [dbo].[Project] WHERE PMID = '"+ PMID +"'", con))
                    {
                        sqlDA.Fill(DT);
                    }
                }
                return View(DT);
            }
            else
                return RedirectToAction("Login", "Home");

            
        }

        // GET: Project/Create
        public ActionResult Create()
        {
            if ((string)Session["User_Role"] == "PM")
            {
                Project project = new Project();

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("SELECT MAX(PID) FROM Project", con))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        string pid = reader[0].ToString();

                                        if (string.IsNullOrEmpty(pid))
                                        {
                                            project.PID = "P" + "0001";
                                        }
                                        else
                                        {
                                            var prefix = Regex.Match(pid, "^\\D+").Value;
                                            var number = Regex.Replace(pid, "^\\D+", "");
                                            var i = int.Parse(number) + 1;
                                            project.PID = prefix + i.ToString(new string('0', number.Length));
                                        }
                                    }
                                }
                                else
                                {
                                    project.PID = "P" + "0001";
                                }
                            }
                        }
                    }

                    return View(project);

                }
                catch (Exception)
                {
                    ModelState.AddModelError("CreateError", "Somthing Went Wrong.");
                    return View();
                }

            }
            else
                return RedirectToAction("Login", "Home");

            
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();
                    
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Project VALUES(@PID,@Project_Name,@StartDate,@EndDate,@PMID)", con))
                    {
                        int PMID = Convert.ToInt32(Session["User_ID"]);

                        cmd.Parameters.AddWithValue("@PID", project.PID);
                        cmd.Parameters.AddWithValue("@Project_Name", project.Project_Name);
                        cmd.Parameters.AddWithValue("@StartDate", project.StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", project.EndDate);
                        cmd.Parameters.AddWithValue("@PMID", PMID); //project.PMID
                        cmd.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("CreateError", "Please provide valid entries for fields.");
                return View();
            }
        }

        // GET: Project/Edit/5
        public ActionResult Edit(string id)
        {
            if ((string)Session["User_Role"] == "PM")
            {

                Project project = new Project();
                DataTable DT = new DataTable();

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM Project WHERE PID = @PID", con))
                        {
                            sqlDA.SelectCommand.Parameters.AddWithValue("@PID", id);
                            sqlDA.Fill(DT);
                        }
                    }

                    if (DT.Rows.Count == 1)
                    {
                        project.PID = DT.Rows[0][0].ToString();
                        project.Project_Name = DT.Rows[0][1].ToString();
                        project.StartDate = Convert.ToDateTime(DT.Rows[0][2].ToString());
                        project.EndDate = Convert.ToDateTime(DT.Rows[0][3].ToString());
                        project.PMID = Convert.ToInt32(DT.Rows[0][4].ToString());

                        return View(project);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch
                {

                    return RedirectToAction("Index");
                }
            }
            else
                return RedirectToAction("Login", "Home");
        }

        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Project project)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE Project SET Project_Name = @Project_Name, StartDate = @StartDate, EndDate = @EndDate WHERE PID = @PID", con))
                    {
                        cmd.Parameters.AddWithValue("@PID", id);
                        cmd.Parameters.AddWithValue("@Project_Name", project.Project_Name);
                        cmd.Parameters.AddWithValue("@StartDate", project.StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", project.EndDate);
                        cmd.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("EditError", "Please provide valid entries for fields.");
                return View();
            }
        }

        // GET: Project/Delete/5
        public ActionResult Delete(string id)
        {
            if ((string)Session["User_Role"] == "PM")
            {

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Project WHERE PID = @PID", con))
                    {
                        cmd.Parameters.AddWithValue("@PID", id);
                        cmd.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Login", "Home");
        }
    }
}
