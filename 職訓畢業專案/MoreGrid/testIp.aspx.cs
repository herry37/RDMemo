using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class testIp : System.Web.UI.Page
{
    
    /// <summary>
    /// 取得正確的Client端IP
    /// </summary>
    /// <returns></returns>
    protected string GetClientIP()
    {
        //判所client端是否有設定代理伺服器
        if (Request.ServerVariables["HTTP_VIA"] == null)
            return Request.ServerVariables["REMOTE_ADDR"].ToString();
        else
            return Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
    }
    private static string RetrieveIP(HttpRequest request)
    {
        string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (ip == null || ip.Trim() == string.Empty)
        {
            ip = request.ServerVariables["REMOTE_ADDR"];
        }
        return ip;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string clientIP = GetClientIP();
        Response.Write(clientIP);

    }
}