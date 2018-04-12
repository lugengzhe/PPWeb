using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace PPWeb
{
    public class Class1
    {
        public SqlConnection GetConnection()
        {
            string constr = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServer1"].ToString();
            con = new SqlConnection(constr);
            if (con.state == ConnectionState.closed)
                con.open();
            return con;
        }
    }
}