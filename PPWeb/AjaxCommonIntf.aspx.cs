using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Zj.WebFrame.DataQuery.IModel;
using Zj.WebFrame.Util;

namespace Zj.WebFrame.PC.Web.Votes
{
    public partial class AjaxCommonIntf1 : System.Web.UI.Page
    {
        private RequestData _req;
        private ResponseData _res;
        private string _serialNo = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Log.Info("连接进入", System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                if (Request.HttpMethod.ToLower() == "post")
                {
                    //0、判断登录是否有效 zhangqi 20160520
                    if (!CheckDebugState() && (Session["_APPID"] == null || Session["_USERCERT"] == null))
                    { //登录已经失效
                        ReturnError("登录已失效，请重新登录", "-9998");
                        return;
                    }
                    //1、获取PostData
                    string postString = Tools.GetPostString(System.Web.HttpContext.Current.Request.InputStream);
                    if (string.IsNullOrEmpty(postString))
                    {
                        string msg = "报文为空";
                        Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, msg);
                        ReturnError(msg);
                        return;
                    }
                    //2、解析
                    Log.Info("输入", postString);
                    _req = RequestData.Parse(postString);
                    //3、处理消息
                    //3.1、获取Session
                    _req.HasSession = false;
                    if (Session[Zj.WebFrame.Model.InternalTypes.SESSION_userInfo] != null)
                    {
                        Zj.WebFrame.Model.UserInfo userInfo = (Zj.WebFrame.Model.UserInfo)Session[Zj.WebFrame.Model.InternalTypes.SESSION_userInfo];
                        _req.UserData = new Zj.WebFrame.DataQuery.IModel.UserInfo();
                        _req.UserData.UserCode = userInfo.userCode;
                        _req.UserData.BranchNo = userInfo.branchNo;
                        _req.UserData.Mobile = userInfo.mobile;
                        _req.UserData.Name = userInfo.name;
                        _req.UserToken = userInfo.userCode;
                        if (Session[Zj.WebFrame.Model.InternalTypes.SESSION_extraData] != null)
                            _req.MsgExtraData = Session[Zj.WebFrame.Model.InternalTypes.SESSION_extraData].ToString();
                        _req.HasSession = true;//》》》》》》》》》》》》》正式需要改回去
                    }
                    _serialNo = _req.SerialNo;
                    _res = _req.Process();
                    _res.SerialNo = _req.SerialNo;

                    //4、返回消息
                    ReturnData(_res.Format());
                }
                else

                {
                    string msg = "不支持非Post方式访问.";
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, msg);
                    ReturnError(msg);
                }
            }
            catch (Exception exp)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, exp.Message + "\n" + exp.StackTrace);
                ReturnError(exp.Message);  //不能解析来源和目标，不能返回XML数据
            }
        }
        /// <summary>
        /// 返回错误消息
        /// </summary>
        /// <param name="msg"></param>
        private void ReturnError(string msg)
        {
            ResponseData err = new ResponseData();
            err.IsError = true;
            err.RetCode = "-9999";
            err.SerialNo = _serialNo;
            err.RetMsg = msg;

            ReturnData(err.Format());
        }

        /// <summary>
        /// 返回错误消息
        /// </summary>
        /// <param name="msg"></param>
        private void ReturnError(string msg, String retCode)
        {
            ResponseData err = new ResponseData();
            err.IsError = true;
            err.RetCode = retCode;
            err.SerialNo = _serialNo;
            err.RetMsg = msg;

            ReturnData(err.Format());
        }
        /// <summary>
        /// 检查是否处于单网页调试状态
        /// </summary>
        /// <returns></returns>
        private bool CheckDebugState()
        {
            System.Configuration.AppSettingsReader rd = new System.Configuration.AppSettingsReader();
            String isDebugStat = rd.GetValue("IsDebugState", typeof(string)).ToString();
            try
            {
                if (isDebugStat.ToLower() == "false")
                    return false;
                else
                {
                    Zj.WebFrame.Model.UserInfo userInfo = new WebFrame.Model.UserInfo();
                    userInfo.branchNo = rd.GetValue("DebugBranch", typeof(string)).ToString();
                    userInfo.name = "测试" + DateTime.Now.ToString();
                    userInfo.userCode = rd.GetValue("DebugUserCode", typeof(string)).ToString();
                    userInfo.mobile = "1";
                    if (Session[Zj.WebFrame.Model.InternalTypes.SESSION_userInfo] != null) Session.Remove(Zj.WebFrame.Model.InternalTypes.SESSION_userInfo);
                    Session.Add(Zj.WebFrame.Model.InternalTypes.SESSION_userInfo, userInfo);
                }
            }
            catch
            {
                //可能没有定义这个参数导致异常
                return false;
            }
            return true;
        }
        private void ReturnData(string msg)
        {
            Log.Info("返回", msg);
            Response.Write(msg);
        }
    }
}