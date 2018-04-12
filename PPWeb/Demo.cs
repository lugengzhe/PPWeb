using System;
using System.IO;
using System.Web;
using System.Web.Services; //引入命名空间

public class Class1: System.Web.UI.Page
{

    [WebMethod]

    public static string SayHello()
    {
        return "Hello Ajax!";
    }
    protected void Page_load(object sender,EventArgs e)
    {
        try

        {
            if (Request.HttpMethod.ToLower() == "post")
            {
                Stream s = System.Web.HttpContext.Current.Request.InputStream;
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int)s.Length);
                string poststr = System.Text.Encoding.UTF8.GetString(b);
                if (!string.IsNullOrEmpty(poststr))
                {
                    Response.Write(poststr);
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
            Response.Write(exp.Message);  //不能解析来源和目标，不能返回XML数据
        }
    }
}