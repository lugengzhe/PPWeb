using System;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
namespace PPWeb
{
public partial class WebForm1 : System.Web.UI.Page
    {
        string sqldataName;
        string sqldataValue;
        //string sqldataStr;
        List<string> datalist = new List<string>();
        public class Singledata
        {
            public string source { get; set; }
            public string head { get; set; }
            public string child { get; set; }
            public string parent { get; set; }
            public string starttime { get; set; }
            public string hometeam { get; set; }
            public string awayteam { get; set; }
            public string ht00 { get; set; }
            public string at00 { get; set; }
            public string htspread { get; set; }
            public string atspread { get; set; }
            public string htspread00 { get; set; }
            public string atspread00 { get; set; }
            public string total { get; set; }
            public string overt { get; set; }
            public string undert { get; set; }
            public string nowtime { get; set; }
            public string uniStr { get; set; }
        }
        List<Singledata> sqldataArray = new List<Singledata>();
        //Singledata lu = new Singledata();

        public SqlConnection Getconnection()
        {
            string constring = ConfigurationManager.ConnectionStrings["SqlServer1"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);
            if (con.State == ConnectionState.Closed)
                con.Open();
            return con;
        }
        public void LoadSql(string SqlString)
         {
      
            SqlCommand cmd = new SqlCommand(SqlString, Getconnection());
            SqlDataReader sqldata = cmd.ExecuteReader();
            //sqldata.Read();
            sqldataArray.Clear();
            while (sqldata.Read())  
            {
                Singledata lu = new Singledata();
                //sqldataStr = "{";
                for (int i = 0; i < sqldata.FieldCount; i++)
                {
                    

                    sqldataName = sqldata.GetName(i);
                    sqldataValue = Regex.Replace(sqldata[i].ToString(), @"\u0020", string.Empty);
                    sqldataValue = Regex.Replace(sqldataValue, @"\u000A", string.Empty);
                    lu.GetType().GetProperty(sqldataName).SetValue(lu, sqldataValue, null);
                    //sqldataStr += sqldataName + ':' + sqldataValue + ',';自己拼接字符串
                }
                //sqldataStr += "}";
                sqldataArray.Add(lu);
                //datalist.Add(sqldataStr);

            }
            //datalist.Add(sqldataArray);
            sqldata.Close();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try

            {
                if (Request.HttpMethod.ToLower() == "post")
                {
                    string SqlString;
                    string postOutstring;
                    
                    Stream s = System.Web.HttpContext.Current.Request.InputStream;
                    s.Position = 0;
                    StreamReader reader = new StreamReader(s);
                    string postInstr = reader.ReadToEnd();
                    s.Close();
                    switch (postInstr)
                    {
                        case "req001":
                            SqlString = "SELECT DISTINCT uniStr FROM dbo.OddData";
                            LoadSql(SqlString);
                            postOutstring = JsonConvert.SerializeObject(sqldataArray);
                            break;
                        case "req002":
                            SqlString = "SELECT top(10)* FROM dbo.OddData";
                            LoadSql(SqlString);
                            postOutstring = JsonConvert.SerializeObject(sqldataArray);
                            break;
                        case "reqDiy":
                            SqlString = "SELECT top(10)* FROM dbo.OddData";
                            LoadSql(SqlString);
                            postOutstring = JsonConvert.SerializeObject(sqldataArray);
                            break;
                        default:
                            postOutstring = "无效请求";
                            break;
                    }
                    //string val = Request["aa"];
                    if (!string.IsNullOrEmpty(postInstr))
                    {
                        Response.Write(postOutstring);
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
//namespace CrjIIOfflineAccept.CrjIITools
//{
//    public class JsonTools
//    {
//        // 从一个对象信息生成Json串
//        public static string ObjectToJson(object obj)
//        {
//            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
//            MemoryStream stream = new MemoryStream();
//            serializer.WriteObject(stream, obj);
//            byte[] dataBytes = new byte[stream.Length];
//            stream.Position = 0;
//            stream.Read(dataBytes, 0, (int)stream.Length);
//            return Encoding.UTF8.GetString(dataBytes);
//        }
//        // 从一个Json串生成对象信息
//        public static object JsonToObject(string jsonString, object obj)
//        {
//            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
//            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
//            return serializer.ReadObject(mStream);
//        }
//    }
//}