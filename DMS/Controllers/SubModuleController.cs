using DMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DMS.Controllers
{
    public class SubModuleController : Controller
    {
        // GET: SubModule
        public ActionResult Index(string Pid, string Mid)
        {
            if ((string)Session["User_Role"] == "ML")
            {
                DataTable DT = new DataTable();

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM [dbo].[SubModule] WHERE PID = '" + Pid + "' AND MID = '"+ Mid + "' ", con))
                    {
                        sqlDA.Fill(DT);
                    }
                }

                return View(DT);
            }
            else
                return RedirectToAction("Login", "Home");
        }

        // GET: SubModule/Create
        [Route("SubModule/{Pid}/{Mid}")]
        public ActionResult Create(string Pid, string Mid)
        {
            if ((string)Session["User_Role"] == "ML")
            {
                SubModule submodule = new SubModule();
                submodule.PID = Pid;
                submodule.MID = Mid;

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("SELECT MAX(SMID) FROM [dbo].[SubModule] WHERE PID = '" + Pid + "' AND MID = '"+ Mid +"' ", con))
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
                                            submodule.SMID = "SM" + "001";
                                        }
                                        else
                                        {
                                            var prefix = Regex.Match(mid, "^\\D+").Value;
                                            var number = Regex.Replace(mid, "^\\D+", "");
                                            var i = int.Parse(number) + 1;
                                            submodule.SMID = prefix + i.ToString(new string('0', number.Length));
                                        }
                                    }
                                }
                                else
                                {
                                    submodule.SMID = "SM" + "001";
                                }
                            }
                        }
                    }

                    return View(submodule);

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

        // POST: SubModule/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Pid, string Mid, SubModule submodule)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("INSERT INTO SubModule VALUES(@PID,@MID,@SMID,@Name,@Developer)", con))
                        {
                            cmd.Parameters.AddWithValue("@PID", submodule.PID);
                            cmd.Parameters.AddWithValue("@MID", submodule.MID);
                            cmd.Parameters.AddWithValue("@SMID", submodule.SMID);
                            cmd.Parameters.AddWithValue("@Name", submodule.Name);
                            cmd.Parameters.AddWithValue("@Developer", submodule.Developer);
                            cmd.ExecuteNonQuery();
                        }

                    }

                    return RedirectToAction("Index", "SubModule", new { Pid = submodule.PID, Mid = submodule.MID });
                }
                catch (SqlException ex)
                {
                    return Content(ex.Message);
                }
            }
            else
            {
                return View();
            }
        }

        // GET: SubModule/Edit/5
        public ActionResult Edit(string Pid, string Mid, string SMid)
        {
            if ((string)Session["User_Role"] == "ML")
            {

                SubModule submodule = new SubModule();
                DataTable DT = new DataTable();

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM SubModule WHERE PID = @PID AND MID = @MID AND SMID = @SMID", con))
                        {
                            sqlDA.SelectCommand.Parameters.AddWithValue("@PID", Pid);
                            sqlDA.SelectCommand.Parameters.AddWithValue("@MID", Mid);
                            sqlDA.SelectCommand.Parameters.AddWithValue("@SMID", SMid);
                            sqlDA.Fill(DT);
                        }
                    }

                    if (DT.Rows.Count == 1)
                    {
                        submodule.PID = DT.Rows[0][0].ToString();
                        submodule.MID = DT.Rows[0][1].ToString();
                        submodule.SMID = DT.Rows[0][2].ToString();
                        submodule.Name = DT.Rows[0][3].ToString();
                        submodule.Developer = Convert.ToInt32(DT.Rows[0][4].ToString());

                        return View(submodule);
                    }
                    else
                    {
                        return RedirectToAction("Index", "SubModule", new { Pid = submodule.PID, Mid = submodule.MID });
                    }
                }
                catch
                {

                    return RedirectToAction("Index", "SubModule", new { Pid = submodule.PID, Mid = submodule.MID });
                }
            }
            else
                return RedirectToAction("Login", "Home");
        }

        // POST: SubModule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Pid, string Mid, string SMid,SubModule submodule)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE SubModule SET Name = @Name, Developer = @Developer WHERE PID = @PID AND MID = @MID AND SMID = @SMID", con))
                    {
                        cmd.Parameters.AddWithValue("@PID", Pid);
                        cmd.Parameters.AddWithValue("@MID", Mid);
                        cmd.Parameters.AddWithValue("@SMID", SMid);
                        cmd.Parameters.AddWithValue("@Name", submodule.Name);
                        cmd.Parameters.AddWithValue("@Developer", submodule.Developer);
                        cmd.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("Index", "SubModule", new { Pid = submodule.PID, Mid = submodule.MID });
            }
            catch
            {
                return View();
            }
        }

        // GET: SubModule/Delete/5
        public ActionResult Delete(string Pid, string Mid, string SMid)
        {
            if ((string)Session["User_Role"] == "ML")
            {

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("DELETE FROM SubModule WHERE PID = @PID AND MID = @MID AND SMID = @SMID", con))
                    {
                        cmd.Parameters.AddWithValue("@PID", Pid);
                        cmd.Parameters.AddWithValue("@MID", Mid);
                        cmd.Parameters.AddWithValue("@SMID", SMid);
                        cmd.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("Index", "SubModule", new { Pid = Pid, Mid = Mid });
            }
            else
                return RedirectToAction("Login", "Home");
        }
    }
}
