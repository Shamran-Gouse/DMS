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
    public class TestCaseStepsController : Controller
    {
        // GET: TestCaseSteps
        public ActionResult Index(int id)
        {
            if ((string)Session["User_Role"] == "QA")
            {
                TestCaseSteps testCaseSteps = new TestCaseSteps();

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT PID,MID, SMID FROM TestCase WHERE Row_ID = '" + id + "'", con))
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
        public ActionResult _StepTable(int id)
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

                    return PartialView("_StepTable", DT);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [ChildActionOnly]
        public ActionResult _TestStepsHeader(int id)
        {
            TestCase testcase = new TestCase();

            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT TCID,Title FROM [dbo].[TestCase] WHERE Row_ID = '" + id + "'", con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    testcase.TCID = reader[0].ToString();
                                    testcase.Title = reader[1].ToString();
                                }
                            }
                        }
                    }
                }

                return PartialView("_TestStepsHeader", testcase);

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        // GET: TestCaseSteps/Details/5
        public ActionResult Details(int? id)
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

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT TestCase.Row_ID, TestCase.PID, TestCase.MID, TestCase.SMID, TestCase.TCID, TestCase.Title, TestCaseStep.StepID, TestCaseStep.Step, TestCaseStep.StepData, TestCaseStep.ExpectedResult FROM TestCase INNER JOIN TestCaseStep ON TestCase.Row_ID = TestCaseStep.Row_ID WHERE TestCaseStep.Row_ID = @Row_ID ", con))
                        {
                            sqlDA.SelectCommand.Parameters.AddWithValue("@Row_ID", id);
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

        [ChildActionOnly]
        // GET: TestCaseSteps/Create
        public ActionResult _Create(int id)
        {
            if ((string)Session["User_Role"] == "QA")
            {
                return PartialView("_Create", new TestCaseSteps());
            }
            else
                return RedirectToAction("Login", "Home");

        }

        // GET MAX STEP ID + 1
        public int GetMaxStepID(int id)
        {
            int StepId = 1;

            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(StepID) FROM [dbo].[TestCaseStep] WHERE Row_ID = '" + id + "'", con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    StepId = Convert.ToInt32(reader[0].ToString()) + 1;
                                }
                            }
                            else
                            {
                                StepId = 1;
                            }
                        }
                    }
                }

                return StepId;

            }
            catch (Exception ex)
            {
                return StepId;
            }
        }


        // POST: TestCaseSteps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Create(int id, TestCaseSteps testCaseSteps)
        {


            if (string.IsNullOrEmpty(testCaseSteps.Step) | string.IsNullOrEmpty(testCaseSteps.StepData) | string.IsNullOrEmpty(testCaseSteps.ExpectedResult))
            {
                return View("_Create", testCaseSteps);
            }
            else
            {
                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("INSERT INTO TestCaseStep VALUES (@Row_ID,@StepID,@Step,@StepData,@ExpectedResult)", con))
                        {
                            cmd.Parameters.AddWithValue("@Row_ID", id);
                            cmd.Parameters.AddWithValue("@StepID", GetMaxStepID(id));

                            cmd.Parameters.AddWithValue("@Step", testCaseSteps.Step);
                            cmd.Parameters.AddWithValue("@StepData", testCaseSteps.StepData);
                            cmd.Parameters.AddWithValue("@ExpectedResult", testCaseSteps.ExpectedResult);
                            cmd.ExecuteNonQuery();
                        }

                    }

                    return RedirectToAction("Index", "TestCaseSteps", new { id = id });
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
        }







        // GET: TestCaseSteps/Edit/5
        [ChildActionOnly]
        public ActionResult _Edit(int Rid, int Sid)
        {
            if ((string)Session["User_Role"] == "QA")
            {

                TestCaseSteps testCaseSteps = new TestCaseSteps();
                DataTable DT = new DataTable();

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM TestCaseStep WHERE Row_ID = @Row_ID AND StepID = @StepID", con))
                        {
                            sqlDA.SelectCommand.Parameters.AddWithValue("@Row_ID", Rid);
                            sqlDA.SelectCommand.Parameters.AddWithValue("@StepID", Sid);
                            sqlDA.Fill(DT);
                        }
                    }

                    if (DT.Rows.Count == 1)
                    {
                        testCaseSteps.SID = Convert.ToInt32(DT.Rows[0][1].ToString());
                        testCaseSteps.Step = DT.Rows[0][2].ToString();
                        testCaseSteps.StepData = DT.Rows[0][3].ToString();
                        testCaseSteps.ExpectedResult = DT.Rows[0][4].ToString();

                        return PartialView("_Edit", testCaseSteps);
                    }
                    else
                    {
                        return RedirectToAction("Index", "TestCaseSteps", new { id = Rid });
                    }
                }
                catch
                {

                    return RedirectToAction("Index", "TestCaseSteps", new { id = Rid });
                }
            }
            else
                return RedirectToAction("Login", "Home");
        }

        // POST: TestCaseSteps/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }




        // GET: TestCaseSteps/Delete/5
        public ActionResult Delete(int Rid, int sid)
        {
            if ((string)Session["User_Role"] == "QA")
            {

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("DELETE FROM TestCaseStep WHERE Row_ID = @Row_ID AND StepID = @StepID", con))
                    {
                        cmd.Parameters.AddWithValue("@Row_ID", Rid);
                        cmd.Parameters.AddWithValue("@StepID", sid);
                        cmd.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("Index", "TestCaseSteps", new { id = Rid });
            }
            else
                return RedirectToAction("Login", "Home");
        }
    }
}
