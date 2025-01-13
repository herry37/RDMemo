using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這
using System.Text;
using System.Net;
using System.Web.Configuration;
//using System.Text.RegularExpressions;
//using System.Net.Sockets;

public partial class MoreGridManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Login_M"]== null)
            Response.Redirect("Login.aspx");
        
        if(!IsPostBack)
        {
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
            SqlCommand objCmd = new SqlCommand("select 員工帳號基本資料.姓名,員工帳號.權限代碼 from [員工帳號] inner join 員工帳號基本資料 on 員工帳號基本資料.員工帳號=員工帳號.員工帳號 where (員工帳號.員工帳號 ='" + Session["account"] + "')", Conn);
            Conn.Open();
            SqlDataReader dr = objCmd.ExecuteReader();
            dr.Read();
            Session["username"] = dr[0];
            Session["authorityno"] = dr[1];
            Conn.Close();
            Session["PageName"] = "登入系統";
            Session["action"] = "login";


            Session["descript"] = GetIPAddress();
            //IPAddress HostIP = "";
            //Session["descript"] = GetIPAddress()+","+HostIP;

            pageLog();  /* 寫入登入記錄  */

        }
    }


    protected void pageLog()
    {
        //try
        //{
        //    using (SqlConnection Sql_Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString)) {
        //        using (SqlCommand Sql_Command = new SqlCommand()) {
        //            Sql_Conn.Open();
        //            SqlDataSource sql1 = new SqlDataSource();
        //            sql1.InsertParameters.Clear();
        //            sql1.ConnectionString = Sql_Conn.ConnectionString;
        //            sql1.InsertParameters.Add("員工帳號", Session["account"].ToString());
        //            sql1.InsertParameters.Add("操作頁面", Session["PageName"].ToString());
        //            sql1.InsertParameters.Add("操作動作", Session["action"].ToString());
        //            sql1.InsertParameters.Add("操作時間", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //            sql1.InsertParameters.Add("備註", Session["descript"].ToString());
        //            sql1.InsertCommand = "INSERT INTO [員工使用記錄] ([員工帳號], [操作頁面],[操作動作],[操作時間],[備註]) VALUES (@員工帳號, @操作頁面,@操作動作,@操作時間,@備註)";
        //            sql1.Insert();
        //            Sql_Conn.Dispose();
        //            sql1.InsertParameters.Clear();
        //            Sql_Conn.Close();
        //        }
        //    }
        ////    SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        ////    Conn.Open();
        ////    SqlDataSource sql1 = new SqlDataSource();
        ////    sql1.ConnectionString = Conn.ConnectionString;
        ////    sql1.InsertParameters.Add("員工帳號", Session["account"].ToString());
        ////    sql1.InsertParameters.Add("操作頁面", Session["PageName"].ToString());
        ////    sql1.InsertParameters.Add("操作動作", Session["action"].ToString());
        ////    sql1.InsertParameters.Add("操作時間", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        ////    sql1.InsertParameters.Add("備註", Session["descript"].ToString());
        ////    sql1.InsertCommand = "INSERT INTO [員工使用記錄] ([員工帳號], [操作頁面],[操作動作],[操作時間],[備註]) VALUES (@員工帳號, @操作頁面,@操作動作,@操作時間,@備註)";
        ////    sql1.Insert();
        ////    Conn.Close();
        //}
        ////catch (SqlException ex)
        //catch (Exception ex)
        //{
        //    var b = ex.ToString();
        //    throw;
        //}
       
    }

    public string GetIPAddress()
    {
        System.Web.HttpContext context = System.Web.HttpContext.Current;
        string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (string.IsNullOrEmpty(sIPAddress))
        {
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
        else
        {
            //string[] ipArray = sIPAddress.Split(new Char[] { ',' });
            return sIPAddress;
        }
    }

    //public  string GetIPAddress2()
    //{
    //    //string pp;
    //    string hostName = System.Net.Dns.GetHostName();//取得對方的主機名稱
    //    //string[] a = System.Net.Dns.GetHostAddresses(hostName).Select(i => i.ToString()).ToArray();//取得使用者IP(ipv6,ipv4)
    //    System.Net.IPAddress[] a = System.Net.Dns.GetHostAddresses(hostName);
    //    string ip = "";
    //    //for (int i = 0; i < a.Length; i++)
    //    //{
    //    //    Response.Write(i+","+a[i]+"</br>");
    //    //}
    //        if (a.Length > 0) //判斷是否有取得ip
    //            ip = a[a.Length-1] + "@" + hostName.ToString();//取出ipv4結合主機名稱
    //        else
    //            ip = "@" + hostName.ToString();
    //    return ip; 
    //}
}