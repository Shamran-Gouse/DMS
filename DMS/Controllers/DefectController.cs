using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using DMS.Models;
using System.Net.Mail;
using System.Net;

namespace DMS.Controllers
{
    public class DefectController : Controller
    {
        // GET: Defect
        public ActionResult Index()
        {

            if ((string)Session["User_Role"] == "ML" | (string)Session["User_Role"] == "Developer")
            {

                return RedirectToAction("ViewDefects", "Defect");
            }
            else if((string)Session["User_Role"] == "QA")
            {
                return RedirectToAction("ViewDefectsQA", "Defect");
            }
            else
                return RedirectToAction("Login", "Home");
        }

        public ActionResult ViewDefects()
        {
            if ((string)Session["User_Role"] == "ML" | (string)Session["User_Role"] == "Developer")
            {
                try
                {
                    DataTable DT = new DataTable();


                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT Defect.[Defect_id], Defect.[Row_ID], Defect.[Description], Defect.[OpenDate], Defect.[CloseDate], Defect.[Defect_Status], Defect.[RCA], Defect.[Defect_Count] FROM Defect WHERE Assigned_To = @Assigned_To", con))
                        {
                            int UID = Convert.ToInt32(Session["User_ID"]);
                            sqlDA.SelectCommand.Parameters.AddWithValue("@Assigned_To", UID);

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

        public ActionResult ViewDefectsQA()
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

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT Defect.[Defect_id], Defect.[Row_ID], Defect.[Description], Defect.[OpenDate], Defect.[CloseDate], Defect.[Owner], Defect.[Assigned_To], Defect.[Defect_Status], Defect.[RCA], Defect.[Defect_Count] FROM Defect INNER JOIN TestCase ON TestCase.Row_ID = Defect.Row_ID WHERE TestCase.TestBy = @TestBy AND Defect_Status != 'CLOSED' AND Defect_Status != 'REJECT' ", con))
                        {
                            int UID = Convert.ToInt32(Session["User_ID"]);
                            sqlDA.SelectCommand.Parameters.AddWithValue("@TestBy", UID);

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



        public ActionResult DefectTestCase(int id)
        {
            if ((string)Session["User_Role"] != "")
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






        // Module Leader
        [ChildActionOnly]
        public ActionResult _MLDefectControl(int id)
        {
            Defect defect = new Defect();
            defect.Row_id = id;

            return PartialView("_MLDefectControl", defect);
        }

        [HttpPost]
        public ActionResult MLReject(int id)
        {
            string description = "";
            int UID = 0;

            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Defect] SET Defect_Status = 'REJECT', CloseDate = @CloseDate, [Owner] = @Owner, Assigned_To = @Assigned_To WHERE Row_ID = '" + id +"'", con))
                    {
                        cmd.Parameters.AddWithValue("@CloseDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Owner", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Assigned_To", DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[TestCase] SET[Status] = 'REJECT' WHERE Row_ID = @id", con))
                    {
                        cmd.Parameters.AddWithValue("@UID", Convert.ToInt32(Session["User_ID"]));
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("SELECT Defect.[Description], TestCase.TestBy FROM Defect INNER JOIN TestCase ON TestCase.Row_ID = Defect.Row_ID WHERE Defect.Row_ID = '" + id + "'", con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    description = reader[0].ToString();
                                    UID = Convert.ToInt32(reader[1].ToString());
                                }
                            }
                        }
                    }


                }

                // Send Email
                DefectDeatils(id, "REJECT", UID, description);
                return RedirectToAction("Index", "Defect");
            }
            catch
            {
                return RedirectToAction("DefectTestCase", "Defect", new { id = id });
            }
        }

        [HttpPost]
        public ActionResult AssignTODeveloper(int id, Defect defect)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT SubModule.Developer FROM TestCase INNER JOIN SubModule ON SubModule.PID = TestCase.PID AND SubModule.MID = TestCase.MID AND SubModule.SMID = TestCase.SMID WHERE TestCase.Row_ID = '"+ id +"'", con))
                    {
                        using (SqlDataAdapter reader = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            reader.Fill(dt);

                            if (dt.Rows.Count != 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    defect.Assigned_to = Convert.ToInt32(dr[0].ToString());
                                }
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Defect] SET [Owner] = 'DEVELOPER', Defect_Status = 'ASSiGNED TO DEVELOPER', Assigned_To = @Assigned_To, RCA = @RCA WHERE Row_ID = '" + id + "'", con))
                    {
                        cmd.Parameters.AddWithValue("@Assigned_To", defect.Assigned_to);
                        cmd.Parameters.AddWithValue("@RCA", defect.RCA);
                        cmd.ExecuteNonQuery();
                    }

                }

                // Send Email
                DefectDeatils(id, "Raised", defect.Assigned_to, defect.Description);
                return RedirectToAction("Index", "Defect");
            }
            catch(Exception ex)
            {
                return Content(ex.Message);
                //return RedirectToAction("DefectTestCase", "Defect", new { id = id });
            }
        }





        // Developer
        [ChildActionOnly]
        public ActionResult _DeveloperDefectControl(int id)
        {
            Defect defect = new Defect();
            defect.Row_id = id;

            return PartialView("_DeveloperDefectControl", defect);
        }

        [HttpPost]
        public ActionResult Fix(int id)
        {
            string description = "";
            int UID = 0;

            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Defect] SET Defect_Status = 'FIXED', [Owner] = 'ASSIGNED TO QA FOR RETEST' WHERE Row_ID = '" + id + "'", con))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[TestCase] SET[Status] = 'FIXED' WHERE Row_ID = @id", con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("SELECT Defect.[Description], TestCase.TestBy FROM Defect INNER JOIN TestCase ON TestCase.Row_ID = Defect.Row_ID WHERE Defect.Row_ID = '" + id + "'", con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    description = reader[0].ToString();
                                    UID = Convert.ToInt32(reader[1].ToString());
                                }
                            }
                        }
                    }


                }

                // Send Email
                DefectDeatils(id, "FIXED", UID, description);
                return RedirectToAction("Index", "Defect");
            }
            catch
            {
                return RedirectToAction("DefectTestCase", "Defect", new { id = id });
            }
        }






        // QA
        [ChildActionOnly]
        public ActionResult _QADefectControl(int id)
        {
            Defect defect = new Defect();
            defect.Row_id = id;

            return PartialView("_QADefectControl", defect);
        }

        [HttpPost]
        public ActionResult Close(int id)
        {
            string description = "";
            int UID = 0;

            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT [Description], Assigned_To FROM Defect WHERE Row_ID = '" + id + "'", con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    description = reader[0].ToString();
                                    UID = Convert.ToInt32(reader[1].ToString());
                                }
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Defect] SET Defect_Status = 'CLOSED', CloseDate = @CloseDate, [Owner] = @Owner, Assigned_To = @Assigned_To WHERE Row_ID = '" + id + "'", con))
                    {
                        cmd.Parameters.AddWithValue("@CloseDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Owner", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Assigned_To", DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[TestCase] SET[Status] = 'FIXED' WHERE Row_ID = @id", con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                }

                // Send Email
                DefectDeatils(id, "CLOSED", UID, description);
                return RedirectToAction("Index", "Defect");
            }
            catch
            {
                return RedirectToAction("DefectTestCase", "Defect", new { id = id });
            }
        }

        [HttpPost]
        public ActionResult ReAssign(int id)
        {
            string description = "";
            int UID = 0;

            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Defect] SET Defect_Status = 'RE - ASSIGNED FOR FIX', [Owner] = 'DEVELOPER' WHERE Row_ID = '" + id + "'", con))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[TestCase] SET[Status] = 'RE - ASSIGNED' WHERE Row_ID = @id", con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("SELECT [Description], Assigned_To FROM Defect WHERE Row_ID = '" + id + "'", con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    description = reader[0].ToString();
                                    UID = Convert.ToInt32(reader[1].ToString());
                                }
                            }
                        }
                    }
                }

                // Send Email
                DefectDeatils(id, "Re - Assigned", UID, description);
                return RedirectToAction("Index", "Defect");
            }
            catch
            {
                return RedirectToAction("DefectTestCase", "Defect", new { id = id });
            }
        }

        // GET: Defect/Create
        public ActionResult Create(int Row_id)
        {
            if ((string)Session["User_Role"] == "QA")
            {
                Defect defect = new Defect();
                defect.Row_id = Row_id;

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        DataTable DT = new DataTable();

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT * FROM Defect WHERE Row_ID = '" + Row_id + "'", con))
                        {
                            sqlDA.Fill(DT);
                        }

                        if (DT.Rows.Count == 1)
                        {
                            defect.Defect_id = DT.Rows[0][0].ToString();
                        }

                        if (DT.Rows.Count == 0)
                        {
                            using (SqlCommand cmd = new SqlCommand("SELECT MAX(Defect_id) FROM Defect", con))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            string dID = reader[0].ToString();

                                            if (string.IsNullOrEmpty(dID))
                                            {
                                                defect.Defect_id = "DEF" + "0001";
                                            }
                                            else
                                            {
                                                var prefix = Regex.Match(dID, "^\\D+").Value;
                                                var number = Regex.Replace(dID, "^\\D+", "");
                                                var i = int.Parse(number) + 1;
                                                defect.Defect_id = prefix + i.ToString(new string('0', number.Length));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        defect.Defect_id = "DEF" + "0001";
                                    }
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand("SELECT TestCase.TCID, Module.Module_Lead, TestCase.PID, TestCase.MID, TestCase.SMID FROM TestCase INNER JOIN Module ON Module.MID = TestCase.MID AND Module.PID = TestCase.PID WHERE TestCase.Row_ID = '" + Row_id + "'", con))
                            {
                                using (SqlDataAdapter reader =new  SqlDataAdapter(cmd))
                                {
                                    DataTable dt = new DataTable();
                                    reader.Fill(dt);

                                    if (dt.Rows.Count!=0)
                                    {
                                        foreach (DataRow dr in dt.Rows)
                                        {
                                            defect.TCID = dr[0].ToString();
                                            defect.Assigned_to = Convert.ToInt32(dr[1].ToString());
                                            defect.PID = dr[2].ToString();
                                            defect.MID = dr[3].ToString();
                                            defect.SMID = dr[4].ToString();
                                        }
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("CreateError", "Something Went Wrong.");
                                        return View();
                                    }
                                }
                            }

                            return View(defect);
                        }
                        else
                        {
                            return RedirectToAction("Edit", "Defect", new { @id = defect.Defect_id });
                        } 
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

        // POST: Defect/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int Row_id, Defect defect)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Defect]([Defect_id],[Row_ID],[Description],[OpenDate],[Owner],[Assigned_To],[Defect_Status],[RCA],[Defect_Count],[DefectPiority],[DefectSeverity]) VALUES (@Defect_id, @Row_ID, @Description, @OpenDate, @Owner, @Assigned_To, @Defect_Status, @RCA, @Defect_Count, @DefectPiority, @DefectSeverity)", con))
                    {
                        cmd.Parameters.AddWithValue("@Defect_id", defect.Defect_id);
                        cmd.Parameters.AddWithValue("@Row_ID", Row_id);
                        cmd.Parameters.AddWithValue("@Description", defect.Description);
                        cmd.Parameters.AddWithValue("@OpenDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Owner", "MODULE LEADER");
                        cmd.Parameters.AddWithValue("@Assigned_To", defect.Assigned_to);
                        cmd.Parameters.AddWithValue("@Defect_Status", "RAISED");
                        cmd.Parameters.AddWithValue("@RCA", "Not Defined");
                        cmd.Parameters.AddWithValue("@Defect_Count", 1);

                        cmd.Parameters.AddWithValue("@DefectPiority", defect.Piority);
                        cmd.Parameters.AddWithValue("@DefectSeverity", defect.Severity);

                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[TestCase] SET Status = @Status, TestBy = @UID WHERE Row_ID = @id", con))
                    {
                        cmd.Parameters.AddWithValue("@Status", "DEFECT");
                        cmd.Parameters.AddWithValue("@UID", Convert.ToInt32(Session["User_ID"]));
                        cmd.Parameters.AddWithValue("@id", Row_id);
                        cmd.ExecuteNonQuery();
                    }

                }

                // Send Email
                DefectDeatils(defect.Row_id, "Raised", defect.Assigned_to, defect.Description);
                return RedirectToAction("ViewTestCases", "Test", new { Pid = defect.PID, Mid = defect.MID, SMid = defect.SMID });
            }
            catch (SqlException ex)
            {

                ModelState.AddModelError("CreateError", "Please provide valid entries for fields.");
                //return Content("defect id: " + defect.Defect_id + "\n" + "rowid: " + defect.Row_id + "\n" + "defect description: " + defect.Description + "\n" + "defect OPen: " + DateTime.Now + "\n" + "defect close: " + null + "\n" + "owner: " + "MODULE LEADER" + "\n" + "Assigned_To: " + defect.Assigned_to + "\n" + "Defect_Status" + "RAISED" + "\n" + "RCA" + "not Defined" + "\n" + "Defect_Count" + 1 );
                return Content(ex.Message);
            }
        }

        // GET: Defect/Edit/5
        public ActionResult Edit(string id)
        {
            if ((string)Session["User_Role"] == "QA")
            {

                Defect defect = new Defect();
                DataTable DT = new DataTable();

                try
                {
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                        con.Open();

                        using (SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT TestCase.TCID, Defect.* FROM TestCase INNER JOIN Defect ON Defect.Row_ID = TestCase.Row_ID WHERE Defect.Defect_id = @Defect_id", con))
                        {
                            sqlDA.SelectCommand.Parameters.AddWithValue("@Defect_id", id);
                            sqlDA.Fill(DT);
                        }
                    }

                    if (DT.Rows.Count == 1)
                    {
                        defect.TCID = DT.Rows[0][0].ToString();
                        defect.Defect_id = DT.Rows[0][1].ToString();
                        defect.Row_id = Convert.ToInt32(DT.Rows[0][2].ToString());
                        defect.Description = DT.Rows[0][3].ToString();
                        defect.Defect_Count = Convert.ToInt32(DT.Rows[0][10].ToString());

                        return View(defect);
                    }
                    else
                    {
                        return RedirectToAction("ViewTestCases", "Test", new { Pid = defect.PID, Mid = defect.MID, SMid = defect.SMID });
                    }
                }
                catch
                {

                    return RedirectToAction("ViewTestCases", "Test", new { Pid = defect.PID, Mid = defect.MID, SMid = defect.SMID });
                }
            }
            else
                return RedirectToAction("Login", "Home");
        }

        // POST: Defect/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, Defect defect)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT TestCase.TCID, Module.Module_Lead, TestCase.PID, TestCase.MID, TestCase.SMID FROM TestCase JOIN Module ON Module.MID = TestCase.MID AND Module.PID = TestCase.PID JOIN Defect ON Defect.Row_ID = TestCase.Row_ID WHERE Defect.Defect_id = '" + id + "'", con))
                    {
                        using (SqlDataAdapter reader = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            reader.Fill(dt);

                            if (dt.Rows.Count != 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    defect.TCID = dr[0].ToString();
                                    defect.Assigned_to = Convert.ToInt32(dr[1].ToString());
                                    defect.PID = dr[2].ToString();
                                    defect.MID = dr[3].ToString();
                                    defect.SMID = dr[4].ToString();
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("CreateError", "Something Went Wrong.");
                                return View();
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE Defect SET Description = @Description, OpenDate = @OpenDate, CloseDate = @CloseDate, [Owner] = @Owner, Assigned_To = @Assigned_To, Defect_Status = @Defect_Status, Defect_Count = @Defect_Count, DefectPiority = @DefectPiority, DefectSeverity = @DefectSeverity WHERE Defect_id = @Defect_id", con))
                    {
                        cmd.Parameters.AddWithValue("@Description", defect.Description);
                        cmd.Parameters.AddWithValue("@OpenDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@CloseDate", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Owner", "MODULE LEADER");
                        cmd.Parameters.AddWithValue("@Assigned_To", defect.Assigned_to);
                        cmd.Parameters.AddWithValue("@Defect_Status", "RAISED");
                        cmd.Parameters.AddWithValue("@Defect_Count", defect.Defect_Count + 1);

                        cmd.Parameters.AddWithValue("@DefectPiority", defect.Piority);
                        cmd.Parameters.AddWithValue("@DefectSeverity", defect.Severity);

                        cmd.Parameters.AddWithValue("@Defect_id", id);
                        cmd.ExecuteNonQuery();

                    }

                }

                // Send Email
                DefectDeatils(defect.Row_id, "Re-Raised", defect.Assigned_to, defect.Description);
                return RedirectToAction("ViewTestCases", "Test", new { Pid = defect.PID, Mid = defect.MID, SMid = defect.SMID });
            }
            catch(Exception ex)
            {
                return Content(ex.Message);
                //return View();
            }
        }

        public void DefectDeatils(int Row_id,string Status, int UserID , string Description)
        {
            string PID = "";
            string MID = "";
            string SMID = "";
            string Email = "";

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = DMS.Properties.Settings.Default.ConnectionString;
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT PID,MID,SMID FROM TestCase WHERE Row_ID = '"+ Row_id +"'", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                PID = reader[0].ToString();
                                MID = reader[1].ToString();
                                SMID = reader[2].ToString();
                            }
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand("SELECT Email FROM [dbo].[User] WHERE UID = '" + UserID + "'", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Email = reader[0].ToString();
                            }
                        }
                    }
                }

            }

            SendMail(Email, Status, Description, PID, MID, SMID);
        }

        public void SendMail(string email,string status , string description, string pid, string mid, string smid)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("reveur.infor@gmail.com", "reveur@gmail");

                MailMessage msg = new MailMessage();
                msg.To.Add(email);
                msg.From = new MailAddress("reveur.infor@gmail.com");
                msg.Subject = "Defect ";

                string body = "Defect Has "+ status +" pleas see the details below." + Environment.NewLine + " Description : " + description + Environment.NewLine + " Project ID    : " + pid + Environment.NewLine + "Module ID     :  " + mid + Environment.NewLine + "Sub Module ID :  " + smid;
                msg.Body = body;

                //client.SendAsync(msg, msg);
                client.Send(msg);
            }
            catch (Exception ex)
            {
            }
        }

    }
}
