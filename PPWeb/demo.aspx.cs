using System;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Configuration;

namespace PPWeb
{
public partial class WebForm1 : System.Web.UI.Page
    {
        string abc;
        public SqlConnection Getconnection()
        {
            string constring = ConfigurationManager.ConnectionStrings["SqlServer1"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);
            if (con.State == ConnectionState.Closed)
                con.Open();
            return con;
        }
        public void LoadSql()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.test1",Getconnection());
            SqlDataReader sqldata = cmd.ExecuteReader();
            sqldata.Read();
            abc= (String)sqldata[1].ToString().Trim();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try

            {
                if (Request.HttpMethod.ToLower() == "post")
                {
                    LoadSql();
                    Stream s = System.Web.HttpContext.Current.Request.InputStream;
                    s.Position = 0;
                    StreamReader reader = new StreamReader(s);
                    string poststr = reader.ReadToEnd();
                    s.Close();
                    //string val = Request["aa"];
                    if (!string.IsNullOrEmpty(poststr))
                    {
                        Response.Write(abc);
                    }

                }
                else
                {
                    string msg = "不支持非Post方式访问.";
                    Response.Write(msg);
                }
            }
            catch (Exception exp)
            {
                Response.Write(exp.Message);
            }
        }
    }
}