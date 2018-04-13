using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace PPWeb
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try

            {
                if (Request.HttpMethod.ToLower() == "post")
                {
                    Stream s = System.Web.HttpContext.Current.Request.InputStream;
                    s.Position = 0;
                    StreamReader reader = new StreamReader(s);
                    string poststr = reader.ReadToEnd();
                    //string val = Request["aa"];
                    s.Close();
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
                Response.Write("??");
            }
        }
    }
}