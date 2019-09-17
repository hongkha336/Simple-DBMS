using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Database_API_ver1
{
    class Connect
    {
        public String DBName ="";
        public String SeverName = "";
        public String User = "";
        public String Password = "";

        string strConnectionString = "";
        public Connect(String SeverName, String DBName, String User, String Password)
        {
      
            this.SeverName = SeverName;
            this.DBName = DBName;
            this.User = User;
            this.Password = Password;
            strConnectionString =
           @"Data Source="+SeverName+";" +
           "Initial Catalog=" + DBName + ";" + "User ID="+User+";Password="+Password+"";
        }

        // Đối tượng kết nối 
        SqlConnection conn = null;

        public SqlConnection getConnect()
        {
            try
            {
                conn = new SqlConnection(strConnectionString);
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Open();
                return conn;
            }
            catch { }
            return null;
        }
    }
}
