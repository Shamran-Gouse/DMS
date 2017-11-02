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
    public class TestController : Controller
    {
        // GET: Test
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
                    return RedirectToAction("ViewTestCases", "Test", new { Pid = testcase.PID, Mid = testcase.MID, SMid = testcase.SMID });
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

        public ActionResult TestSteps(int id)
        {
            if ((string)Session["User_Role"] == "QA")
            {
                TestCaseSteps testCaseSteps = new TestCaseSteps();

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT PID,MID,SMID,TCID,Title,Status FROM TestCase WHERE Row_ID = '" + id + "'", con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {

                                    testCaseSteps.PID = reader[0].ToString();
                                    testCaseSteps.MID = reader[1].ToString();
                                    testCaseSteps.SMID = reader[2].ToString();
                                    testCaseSteps.TCID = reader[3].ToString();
                                    testCaseSteps.Title = reader[4].ToString();
                                    testCaseSteps.Status = reader[5].ToString();
                                }
                            }
                        }
                    }
                }

                return View(testCaseSteps);

            }
            else
                return RedirectToAction("Login", "Home");
        }

        [ChildActionOnly]
        public ActionResult _TestStepTable(int id)
        {
            try
            {
                DataTable DT = new DataTable();


                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT TestCase.Row_ID, TestCase.PID, TestCase.MID, TestCase.SMID, TestCase.TCID, TestCase.Title, TestCaseStep.StepID, TestCaseStep.Step, TestCaseStep.StepData, TestCaseStep.ExpectedResult FROM TestCase INNER JOIN TestCaseStep ON TestCase.Row_ID = TestCaseStep.Row_ID WHERE TestCaseStep.Row_ID = @Row_ID ", con))
                    {
                        sqlDA.SelectCommand.Parameters.AddWithValue("@Row_ID", id);
                        sqlDA.Fill(DT);
                    }

                    return PartialView("_TestStepTable", DT);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Pass(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[TestCase] SET[Status] = 'Pass', [TestBy] = @UID WHERE Row_ID = @id", con))
                    {
                        cmd.Parameters.AddWithValue("@UID", Convert.ToInt32(Session["User_ID"]));
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("TestSteps", "Test", new { id = id });
            }
            catch
            {
                return RedirectToAction("TestSteps", "Test", new { id = id });
            }
        }
    }
}
