using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS.Models;

namespace DMS.Controllers
{
    public class TestCaseController : Controller
    {
        // GET: TestCase
        public ActionResult Index()
        {
            if ((string)Session["User_Role"] == "QA")
            {

                return View(new TestCase());

            }
            else
                return RedirectToAction("Login", "Home");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(TestCase testcase)
        {

            if ((string)Session["User_Role"] == "QA")
            {
                if (string.IsNullOrEmpty(testcase.PID) | string.IsNullOrEmpty(testcase.MID) | string.IsNullOrEmpty(testcase.SMID))
                {
                    return View("index", testcase);
                }
                else
                {
                    return RedirectToAction("ViewTestCases", "TestCase", new { Pid = testcase.PID, Mid = testcase.MID, SMid = testcase.SMID });
                }
            }
            else
                return RedirectToAction("Login", "Home");
        }

        public ActionResult ViewTestCases(string Pid, string Mid, string SMid)
        {
            if ((string)Session["User_Role"] == "QA")
            {
                try
                {
                    DataTable DT = new DataTable();


                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM [dbo].[Testcase] WHERE PID = @PID AND MID = @MID AND SMID = @SMID ", con))
                        {
                            sqlDA.SelectCommand.Parameters.AddWithValue("@PID", Pid);
                            sqlDA.SelectCommand.Parameters.AddWithValue("@MID", Mid);
                            sqlDA.SelectCommand.Parameters.AddWithValue("@SMID", SMid);

                            sqlDA.Fill(DT);
                        }

                        return View(DT);
                    }

                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }

            }
            else
                return RedirectToAction("Login", "Home");
        }

        // GET: TestCase/Create
        public ActionResult Create(string Pid, string Mid, string SMid)
        {
            if ((string)Session["User_Role"] == "QA")
            {
                TestCase testcase = new TestCase();
                testcase.PID = Pid;
                testcase.MID = Mid;
                testcase.SMID = SMid;

                return View(testcase);

            }
            else
                return RedirectToAction("Login", "Home");

        }

        // POST: TestCase/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Pid, string Mid, string SMid, TestCase testcase)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("INSERT INTO TestCase VALUES (@PID,@MID,@SMID,@TCID,@Title,@Status,@Testby)", con))
                        {
                            cmd.Parameters.AddWithValue("@PID", testcase.PID);
                            cmd.Parameters.AddWithValue("@MID", testcase.MID);
                            cmd.Parameters.AddWithValue("@SMID", testcase.SMID);
                            cmd.Parameters.AddWithValue("@TCID", testcase.TCID);
                            cmd.Parameters.AddWithValue("@Title", testcase.Title);

                            cmd.Parameters.AddWithValue("@Status", "Created");
                            cmd.Parameters.AddWithValue("@Testby", 0);
                            cmd.ExecuteNonQuery();
                        }

                    }

                    return RedirectToAction("ViewTestCases", "TestCase", new { Pid = testcase.PID, Mid = testcase.MID, SMid = testcase.SMID});
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

        // GET: TestCase/Edit/5
        public ActionResult Edit(int id)
        {
            if ((string)Session["User_Role"] == "QA")
            {

                TestCase testcase = new TestCase();
                DataTable DT = new DataTable();

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM TestCase WHERE Row_ID = @Row_ID", con))
                        {
                            sqlDA.SelectCommand.Parameters.AddWithValue("@Row_ID", id);
                            sqlDA.Fill(DT);
                        }
                    }

                    if (DT.Rows.Count == 1)
                    {
                        testcase.PID = DT.Rows[0][1].ToString();
                        testcase.MID = DT.Rows[0][2].ToString();
                        testcase.SMID = DT.Rows[0][3].ToString();
                        testcase.TCID = DT.Rows[0][4].ToString();
                        testcase.Title = DT.Rows[0][5].ToString();

                        return View(testcase);
                    }
                    else
                    {
                        return RedirectToAction("ViewTestCases", "TestCase", new { Pid = testcase.PID, Mid = testcase.MID, SMid = testcase.SMID });
                    }
                }
                catch
                {
                    return RedirectToAction("ViewTestCases", "TestCase", new { Pid = testcase.PID, Mid = testcase.MID, SMid = testcase.SMID });
                }
            }
            else
                return RedirectToAction("Login", "Home");
        }

        // POST: TestCase/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TestCase testcase)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("UPDATE TestCase SET Title = @Title WHERE Row_ID = @Row_ID", con))
                        {
                            cmd.Parameters.AddWithValue("@Title", testcase.Title);
                            cmd.Parameters.AddWithValue("@Row_ID", id);
                            cmd.ExecuteNonQuery();
                        }

                    }

                    return RedirectToAction("ViewTestCases", "TestCase", new { Pid = testcase.PID, Mid = testcase.MID, SMid = testcase.SMID });
                }
                catch (SqlException ex)
                {
                    return Content(ex.Message);
                }
            }
            else
            {
                return View("Edit", testcase);
            }
        }

        // GET: TestCase/Delete/5
        public ActionResult Delete(int id, string Pid, string Mid, string SMid)
        {
            if ((string)Session["User_Role"] == "QA")
            {

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("DELETE FROM TestCase WHERE Row_ID = @Row_ID", con))
                    {
                        cmd.Parameters.AddWithValue("@Row_ID", id);
                        cmd.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("ViewTestCases", "TestCase", new { Pid = Pid, Mid = Mid, SMid = SMid });
            }
            else
                return RedirectToAction("Login", "Home");
        }
    }
}
