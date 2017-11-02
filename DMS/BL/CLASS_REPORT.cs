using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DMS.DAL;
using System.Data;
using System.Data.SqlClient;

namespace DMS.BL
{
    public class CLASS_REPORT:DataAccessLayer
    {
        public static DataTable SP_TOTAL_RAISED_DEFEECT(string projectID,string status,int QA_ID)
        {
            open();
            DataTable dt = ExecuteTable("SP_TOTAL_RAISED_DEFEECT", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar,projectID),
                CreateParameter("@STATUS", SqlDbType.NVarChar,status),
                CreateParameter("@QA_ID", SqlDbType.Int,QA_ID) 
                );
            close();
            return dt;

        }
        public static DataTable SP_TOTAL_NOT_CLOSED_DEFECT(string projectID)
        {
            open();
            DataTable dt = ExecuteTable("SP_TOTAL_NOT_CLOSED_DEFECT", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar, projectID));
            close();
            return dt;
        }
        public static DataTable SP_TOTAL_DEFECT(string projectID)
        {
            open();
            DataTable dt = ExecuteTable("SP_TOTAL_DEFECT", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar, projectID));
            close();
            return dt;
        }
        public static DataTable SP_TOTAL_TEST_CASE(string projectID)
        {
            open();
            DataTable dt = ExecuteTable("SP_TOTAL_TEST_CASE", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar, projectID));
            close();
            return dt;
        }

        public static DataTable SP_GET_TOTAL_TESTCASE_MODULE_WISE(string projectID,string MID)
        {
            open();
            DataTable dt = ExecuteTable("SP_GET_TOTAL_TESTCASE_MODULE_WISE", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar, projectID),
                CreateParameter("@MID", SqlDbType.NVarChar, MID)
                );
            close();
            return dt;
        }
        public static DataTable SP_GET_DEFECT_DEFECT_DENSITY_MODUEL_WISE(string projectID, string MID)
        {
            open();
            DataTable dt = ExecuteTable("SP_GET_DEFECT_DEFECT_DENSITY_MODUEL_WISE", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar, projectID),
                CreateParameter("@MID", SqlDbType.NVarChar, MID)
                );
            close();
            return dt;
        }

        

        public static DataTable SP_GET_DEFECT_DESIGN(string projectID)
        {
            open();
            DataTable dt = ExecuteTable("SP_GET_DEFECT_DESIGN", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar, projectID));
            close();
            return dt;
        }

        public static DataTable SP_GET_DEFECT_REQUIRMENT(string projectID)
        {
            open();
            DataTable dt = ExecuteTable("SP_GET_DEFECT_REQUIRMENT", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar, projectID));
            close();
            return dt;
        }

        public static DataTable SP_GET_DEFECT_DEVELOPMENT(string projectID)
        {
            open();
            DataTable dt = ExecuteTable("SP_GET_DEFECT_DEVELOPMENT", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar, projectID));
            close();
            return dt;
        }

        public static DataTable SP_GET_DEFECT_OTHERS(string projectID)
        {
            open();
            DataTable dt = ExecuteTable("SP_GET_DEFECT_OTHERS", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar, projectID));
            close();
            return dt;
        }

        public static DataTable SP_GET_DEFECT_AGE(string projectID, int day)
        {
            open();
            DataTable dt = ExecuteTable("SP_GET_DEFECT_AGE", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar, projectID),
                CreateParameter("@day", SqlDbType.Int, day) 
                );
            close();
            return dt;

        }

        public static DataTable SP_GET_DEFECT_RCA(string projectID, string status, string RCA)
        {
            open();
            DataTable dt = ExecuteTable("SP_GET_DEFECT_RCA", CommandType.StoredProcedure,
                CreateParameter("@PID", SqlDbType.NVarChar, projectID),
                CreateParameter("@STATUS", SqlDbType.NVarChar, status),
                CreateParameter("@RCA", SqlDbType.NVarChar, RCA) 
                
                );
            close();
            return dt;

        }










        public static DS_REPORTS getDataforOpenVSclosed(string projectID)
        {
            DataAccessLayer.open();
            string query = @"SELECT        Defect.Defect_id, Defect.Row_ID, Defect.Description, Defect.OpenDate, Defect.CloseDate, Defect.Owner, Defect.Assigned_To, Defect.Defect_Status, Defect.RCA, Defect.Defect_Count, TestCase.Row_ID AS Expr1, TestCase.PID, 
                         TestCase.MID, TestCase.SMID, TestCase.TCID, TestCase.Title, TestCase.Status, TestCase.TestBy
FROM            Defect INNER JOIN
                         TestCase ON Defect.Row_ID = TestCase.Row_ID INNER JOIN
                         Project ON TestCase.PID = Project.PID where Project.PID='"+projectID+"'";
            SqlCommand cmd = new SqlCommand(query, DataAccessLayer.con);
            
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DS_REPORTS dataset = new DS_REPORTS();

            //fill datat into drr datatTable
            adapter.Fill(dataset.OpenVSClosed);




            DataAccessLayer.close();
            return dataset;
        }

        public static DS_REPORTS getDataforDRR(string projectID, int QA_ID)
        {
            DataAccessLayer.open();
            string query = @"SELECT   Defect.Defect_id, Defect.Row_ID, Defect.Description, Defect.OpenDate, Defect.CloseDate, Defect.Owner, Defect.Assigned_To, Defect.Defect_Status, Defect.RCA, Defect.Defect_Count, TestCase.Row_ID AS Expr1, TestCase.PID, 
                         TestCase.MID, TestCase.SMID, TestCase.TCID, TestCase.Title, TestCase.Status, TestCase.TestBy, Project.PID AS Expr2, Project.Project_Name, Project.StartDate, Project.EndDate, Project.PMID, [User].UID, [User].Name, 
                         [User].Email, [User].Password, [User].User_Role
FROM            Defect INNER JOIN TestCase INNER JOIN
                         Project ON TestCase.PID = Project.PID INNER JOIN
                         [User] ON TestCase.TestBy = [User].UID ON Defect.Row_ID = TestCase.Row_ID 
WHERE TestBy='" + QA_ID + "'   and Project.PID='" + projectID + "'";
            SqlCommand cmd = new SqlCommand(query, DataAccessLayer.con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DS_REPORTS dataset = new DS_REPORTS();

            //fill datat into drr datatTable
            adapter.Fill(dataset.DRR);




            DataAccessLayer.close();
            return dataset;
        }

        public static DS_REPORTS getDataforDD(string projectID)
        {
            DataAccessLayer.open();
            string query = @"SELECT   * from  Defect  
                        JOIN TestCase ON Defect.Row_ID = TestCase.Row_ID
						 JOIN Project ON TestCase.PID = Project.PID where Project.PID='"+projectID+"'";
            SqlCommand cmd = new SqlCommand(query, DataAccessLayer.con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DS_REPORTS dataset = new DS_REPORTS();

            //fill datat into drr datatTable
            adapter.Fill(dataset.DD);




            DataAccessLayer.close();
            return dataset;
        }

        public static DS_REPORTS getDataforLifeCycle(string projectID)
        {
            DataAccessLayer.open();
            string query = @"
SELECT        Defect.Defect_id, Defect.Row_ID, Defect.Description, Defect.OpenDate, Defect.CloseDate, Defect.Owner, Defect.Assigned_To, Defect.Defect_Status, Defect.RCA, Defect.Defect_Count, TestCase.Row_ID AS Expr1, TestCase.PID, 
                         TestCase.MID, TestCase.SMID, TestCase.TCID, TestCase.Title, TestCase.Status, TestCase.TestBy, Project.PID AS Expr2, Project.Project_Name, Project.StartDate, Project.EndDate, Project.PMID
FROM            Defect INNER JOIN
                         TestCase ON Defect.Row_ID = TestCase.Row_ID CROSS JOIN
                         Project where Project.PID='" + projectID + "'";
            SqlCommand cmd = new SqlCommand(query, DataAccessLayer.con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DS_REPORTS dataset = new DS_REPORTS();

            //fill datat into drr datatTable
            adapter.Fill(dataset.lIFE_CYCLE);




            DataAccessLayer.close();
            return dataset;
        }

        public static DS_REPORTS getDataforDefectAging(string projectID,int day)
        {
            DataAccessLayer.open();
            string query = "SP_GET_DEFECT_AGE";

            SqlCommand cmd = new SqlCommand(query, DataAccessLayer.con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DS_REPORTS dataset = new DS_REPORTS();

            cmd.Parameters.AddWithValue("@PID", projectID);
            cmd.Parameters.AddWithValue("@day", day);
            cmd.CommandType = CommandType.StoredProcedure;

            // retun data
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            //fill datat into drr datatTable
            adapter.Fill(dataset.SP_GET_DEFECT_AGE);




            DataAccessLayer.close();
            return dataset;
        }

        public static DS_REPORTS getDataforDefectSeverity(string projectID)
        {
            DataAccessLayer.open();
            string query = "SP_DEFECT_SEVERITY";

            SqlCommand cmd = new SqlCommand(query, DataAccessLayer.con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DS_REPORTS dataset = new DS_REPORTS();

            cmd.Parameters.AddWithValue("@PID", projectID);
            
            cmd.CommandType = CommandType.StoredProcedure;

            // retun data
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            //fill datat into drr datatTable
            adapter.Fill(dataset.SP_DEFECT_SEVERITY);




            DataAccessLayer.close();
            return dataset;
        }

        public static DS_REPORTS getDefectRCA(string projectID,string RCA)
        {
            DataAccessLayer.open();
            string query = "select * from Defect D join Testcase T on T.Row_ID=D.Row_ID join Project P on P.PID=T.PID  where T.PID=@PID and RCA=@RCA";
            SqlCommand cmd = new SqlCommand(query, DataAccessLayer.con);
           // cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PID", projectID);
           
            cmd.Parameters.AddWithValue("@RCA", RCA);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DS_REPORTS dataset = new DS_REPORTS();

            //fill datat into drr datatTable
            adapter.Fill(dataset.RCA);




            DataAccessLayer.close();
            return dataset;
        }


    }
}