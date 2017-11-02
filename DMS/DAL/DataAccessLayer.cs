using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace DMS.DAL
{
    public class DataAccessLayer
    {

        //  public static string str = @"Data Source=.; initial Catalog=NAOMI_VILLA;Integrated Security=True;";
        public static string str = System.Configuration.ConfigurationManager.ConnectionStrings["DMS.Properties.Settings.ConnectionString"].ConnectionString;

        public static SqlConnection con;
        protected static SqlCommand cmd;

        public static void open()
        {
            //  SqlConnection con = new SqlConnection();
            try
            {
                con = new SqlConnection(str);
                con.Open();

            }
            catch (SqlException e)
            {
              //  MessageBox.Show(e.Message);

            }

        }// end of open methode


        // Close Connection
        public static void close()
        {
            //  SqlConnection con = new SqlConnection();
            try
            {
                con = new SqlConnection(str);
                con.Close();

            }
            catch (SqlException e)
            {
             //   MessageBox.Show(e.Message);

            }

        }// end of Close methode



        /*  Create a Object   */

        public static object ExecuteScalar(string query, CommandType type, params SqlParameter[] arr)
        {
            // initialize
            cmd = new SqlCommand(query, con);
            cmd.Parameters.AddRange(arr);
            cmd.CommandType = type;

            // return object
            object obj = cmd.ExecuteScalar();
            return obj;
        }

        /*  Create a insert/Update Methode  */

        public static int ExecuteNoneQuery(string query, CommandType type, params SqlParameter[] arr)
        {
            // initialize
            cmd = new SqlCommand(query, con);
            cmd.Parameters.AddRange(arr);
            cmd.CommandType = type;

            // retunr int value
            int n = cmd.ExecuteNonQuery();
            return n;

        }


        /*  Create a DELETE/SEARCH Methode  */
        /* I am using DataGridview  */

        public static DataTable ExecuteTable(string query, CommandType type, params SqlParameter[] arr)
        {
            // initialize
            cmd = new SqlCommand(query, con);
            cmd.Parameters.AddRange(arr);
            cmd.CommandType = type;

            // retun data
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            // add data adapter into datat Table
            da.Fill(dt);
            // retunr data Table
            return dt;

        }




        /*  Create parameter Methode  */
        /*  bcz i am using stored procedure  */
        public static SqlParameter CreateParameter(string name, SqlDbType type, object value)
        {
            SqlParameter sp = new SqlParameter();
            sp.ParameterName = name;
            sp.SqlDbType = type;
            sp.SqlValue = value;

            // retunt parameter
            return sp;
        }





    }
}