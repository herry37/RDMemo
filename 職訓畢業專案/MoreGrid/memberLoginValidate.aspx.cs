using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//加入資料庫
using System.Data;
using System.Data.SqlClient;
//加入config設定
using System.Configuration;
//加入mail
using System.Net.Mail;
using System.Text;
using System.Threading;

public partial class memberLoginValidate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btGo_Click(object sender, EventArgs e)
    {
        if (Check_SQLInjection(txtNo.Text, txtPsd.Text) != 0)
        {
            Response.End();
        }
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
    
        SqlDataReader objReader = null;
        SqlCommand checkCmd = new SqlCommand("select * from 會員 where 帳號=@id and 密碼=@psd and 啟用=1", Conn);
        checkCmd.Parameters.AddWithValue("@id", txtNo.Text);
        checkCmd.Parameters.AddWithValue("@psd", txtPsd.Text);

        Conn.Open();
        objReader = checkCmd.ExecuteReader();

        if (objReader.HasRows)
        {
            objReader.Read();
            Session["u_id"] = objReader["帳號"].ToString();
            Session["u_psd"] = objReader["密碼"].ToString();
            Session["u_uid"] = objReader["暱稱"].ToString();
            Session["u_validate"] = objReader["啟用"].ToString();
            Session["u_authenticate"] = objReader["認證"].ToString();
            Session["Login"] = "OK";


            //抓IP
            //string sFromIP = HttpContext.Current.Request.UserHostAddress;
            Session["descript"] = GetIPAddress();
            //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
            //Conn.Open();
            SqlDataSource sql1 = new SqlDataSource();
            try
            {
                sql1.ConnectionString = Conn.ConnectionString;
                sql1.InsertCommand = "INSERT INTO [會員登入記錄] ([帳號], [登入時間], [登入ip]) VALUES (@帳號, @登入時間, @登入ip)";
                sql1.InsertParameters.Add("帳號", Session["u_id"].ToString());
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sql1.InsertParameters.Add("登入時間", date);
                sql1.InsertParameters.Add("登入ip", Session["descript"].ToString());
                sql1.Insert();
            }
            catch (Exception ex)
            {

                Response.Write("<script>alert('"+ ex.ToString() + "！');location.href='Main.aspx';</script>");
            }
         
     
            //Conn.Close();


            checkCmd.Cancel();
            objReader.Close();
            Conn.Close();
            Conn.Dispose();
            Response.Write("<script>alert('登入成功！');location.href='Main.aspx';</script>");
        }
        else
        {
            Response.Write("<script>alert('帳號密碼有誤');</script>");
            checkCmd.Cancel();
            objReader.Close();
            Conn.Close();
            return;
        }
    }
    protected int Check_SQLInjection(String str_1, String str_2)
    {
        int i = 0;
        if ((str_1.IndexOf("--", 0) != -1) || (str_1.IndexOf("1=1", 0) != -1))
        {
            Response.Write("<h2> 帳號欄位請勿輸入符號字元 </h2>");
            i = 1;
        }
        else
        {
            i = 0;
            if ((str_2.IndexOf("--", 0) != -1) || (str_2.IndexOf("1=1", 0) != -1))
            {
                Response.Write("<h2> 密碼欄位請勿輸入符號字元 </h2>");
                i = 1;
            }
            else
            {
                i = 0;
            }
        }
        return i;
    }


    protected void btnSubmitEmil_Click(object sender, EventArgs e)
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlDataReader objReader = null;
        SqlCommand checkCmd = new SqlCommand("select * from 會員 where email=@email", Conn);
        checkCmd.Parameters.AddWithValue("@email", txtEmail.Text);

        Conn.Open();
        objReader = checkCmd.ExecuteReader();

        if (objReader.HasRows)
        {
            objReader.Read();

            SmtpClient client = new SmtpClient("msa.hinet.net");
            string subject = "您的會員密碼";
            string from = "iltusyou@gmail.com";
            string recipients = objReader["email"].ToString();
            string body = "您的帳號是：" + objReader["帳號"].ToString() +
                          "\n您的密碼是：" + objReader["密碼"].ToString();

            client.Send(from, recipients, subject, body);
            StringBuilder sbJScript = new StringBuilder();
            checkCmd.Cancel();
            objReader.Close();
            Conn.Close();
            Conn.Dispose();

            Response.Write("<script>alert('會員密碼已寄到您的信箱！');</script>");
            Response.Write("<script>window.close();</script>");

        }
        else
        {
            Response.Write("<script>alert('信箱有誤');</script>");
            checkCmd.Cancel();
            objReader.Close();
            Conn.Close();
            return;

        }
        txtEmail.Text = "";
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        txtEmail.Text = "";
    }
    public string GetIPAddress()
    {
        System.Web.HttpContext context = System.Web.HttpContext.Current;

        // 首先嘗試從HTTP_X_FORWARDED_FOR頭部獲取IP地址
        string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (string.IsNullOrEmpty(sIPAddress))
        {
            // 如果HTTP_X_FORWARDED_FOR為空，使用REMOTE_ADDR頭部的IP位址
            sIPAddress = context.Request.ServerVariables["REMOTE_ADDR"];

            // 假設內部 IP 位址是 ::1 或 127.0.0.1，這些是本地回環位址
            if (sIPAddress.Trim().Equals("::1"))
            {
                sIPAddress = "127.0.0.1";
            }
            return sIPAddress;
        }
        else
        {
            // 如果 HTTP_X_FORWARDED_FOR 包含多個 IP 地址，選擇第一個非內部 IP
            string[] ipArray = sIPAddress.Split(new char[] { ',' });

            foreach (string ip in ipArray)
            {
                // 假設內部 IP 位址是 ::1 或 127.0.0.1，這些是本地回環位址
                if (!ip.Trim().Equals("::1") && !ip.Trim().Equals("127.0.0.1"))
                {
                    sIPAddress = ip.Trim();
                    break;
                }
            }
            return sIPAddress;
        }
    }
}