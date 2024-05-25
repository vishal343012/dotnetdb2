using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace Jitbit.Utils
{
    public static class DBUtils
    {
        //private static string _strcon= "Data Source=74.225.143.126,50001;Initial Catalog=ASPState; User ID=sa;Password=Blueberryuatadmin#4321;";
        private static string _strcon = "Data Source=74.225.201.149;Initial Catalog=nhidb; User ID=BlueberryuatDB;Password=Blueberryuatadmin#4321;";
        public static string _connStr
        {
            get { return _strcon; }
        }

         
        private static string _strconReports;
        private static string _connStrReports
        {
            get { return _strconReports ?? _strcon; }
        }

         
        static DBUtils()
        {
           
        }

        public static SqlCommand GetNewCommandObject(SqlConnection connection = null)
        {
            if (connection == null) connection = new SqlConnection(_strcon);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            return cmd;
        }


        public static SqlConnection GetNewConnection()
        {
            return new SqlConnection(_connStr);
        }

        public static SqlConnection GetNewOpenConnection()
        {
            var cn = new SqlConnection(_connStr);
            cn.Open();
            return cn;
        }

        public static SqlConnection GetNewOpenReportsConnection()
        {
            var cn = new SqlConnection(_connStrReports);
            cn.Open();
            return cn;
        }

       
    }
}
