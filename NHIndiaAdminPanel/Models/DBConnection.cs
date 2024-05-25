using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Backend.Models
{
    public class DBConnection
    {
        //string connectionStr = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString"]);
        string connectionStr = "Data Source=GAUTAM-TECHBINA;Initial Catalog=NHIndiaDB;Integrated Security=True;";



        public Task<DataSet> GetDataSetAsync(String SPName, SqlParameter[] para, CancellationToken cts, int timeout)
        {
            return Task.Run(() =>
            {
                using (SqlConnection con = new SqlConnection(connectionStr))
                {
                    cts.ThrowIfCancellationRequested();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = SPName;
                    foreach (SqlParameter par in para)
                    {
                        cmd.Parameters.Add(par);
                    }
                    cmd.Connection = con;
                    cmd.CommandTimeout = timeout;
                    con.Open();
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    con.Close();
                    con.Dispose();
                    return ds;
                }
            }, cts);

        }
        public async Task<string> GetJsonDataAsync(String SPName, SqlParameter[] para, CancellationToken cts)
        {
            string jsonOutputParam = "@jsonOutput";

            using (SqlConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(SPName, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    foreach (SqlParameter par in para)
                    {
                        cmd.Parameters.Add(par);
                    }
                    // Create output parameter. "-1" is used for nvarchar(max)
                    cmd.Parameters.Add(jsonOutputParam, SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;

                    // Execute the command
                    await cmd.ExecuteNonQueryAsync();

                    // Get the values
                    string json = cmd.Parameters[jsonOutputParam].Value.ToString();
                    conn.Close();
                    return json;
                }
            }
        }
        public object GetExecuteScalarSP(String SPName, SqlParameter[] para)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SPName;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                con.Open();
                return cmd.ExecuteScalar();
            }
        }
        public DataTable GetDataTable1(String SPName)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SPName;
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }
        public object GetExecuteScalarQry(String Qry, SqlParameter[] para)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = Qry;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                con.Open();
                return cmd.ExecuteScalar();
            }
        }

        public DataTable GetDataTable(String SPName, SqlParameter[] para)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SPName;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public DataSet GetDataSet(String SPName, SqlParameter[] para)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SPName;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                cmd.CommandTimeout = 1000;
                con.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                return ds;
            }
        }

        public int ExecuteNonQuerywithconnection(string spName, SqlParameter[] para, string connectionStr)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(string spName, SqlParameter[] para)
        {
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                foreach (SqlParameter par in para)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.Connection = con;
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }

    public class MessageStatus
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }
}