using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Net;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["account"] != null)
            Server.Transfer("MoreGridManage.aspx");
        //Session.Clear();
        labmsg.Text = "";
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand objCmd = new SqlCommand("select * from [員工帳號] where ([員工帳號] = @account AND [啟用]='true')", Conn);
        objCmd.Parameters.AddWithValue("account", txtAccount.Text);
        Conn.Open();
        SqlDataReader dr = objCmd.ExecuteReader();
        if (dr.HasRows)
        {
            Conn.Close();
            Conn.Open();
            objCmd = new SqlCommand("select * from [員工帳號] where ([員工帳號] = @account AND [員工密碼]=@pwd)", Conn);
            objCmd.Parameters.AddWithValue("account", txtAccount.Text);
            objCmd.Parameters.AddWithValue("pwd", txtpwd.Text);
            dr = objCmd.ExecuteReader();
            if (dr.HasRows)
            {
                Session["Login_M"] = "4";
                Session["account"] = txtAccount.Text;
                Server.Transfer("MoreGridManage.aspx");

                //labmsg.Text = labmsg.Text + "  --登入成功--";
            }
            else
                labmsg.Text = "..帳號或密碼錯誤..";
        }
        else
        {

            labmsg.Text = "..帳號不存在或帳號未啟用..";
        }
        Conn.Close();
    }
    protected void btnSubmitEmil_Click(object sender, EventArgs e)
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlDataReader objReader = null;
        SqlCommand checkCmd = new SqlCommand("select 員工帳號基本資料.員工帳號,員工帳號.員工密碼 from 員工帳號基本資料 left join 員工帳號 on 員工帳號基本資料.員工帳號=員工帳號.員工帳號 where email=@email", Conn);
        checkCmd.Parameters.AddWithValue("@email", txtEmail.Text);

        Conn.Open();
        objReader = checkCmd.ExecuteReader();

        if (objReader.HasRows)
        {
            objReader.Read();

            SmtpClient client = new SmtpClient("msa.hinet.net");
            string subject = "您的密碼";
            string from = "iltusyou@gmail.com";
            string recipients = txtEmail.Text;
            string body = "您的帳號是：" + objReader[0].ToString() +
                          "\n您的密碼是：" + objReader[1].ToString();

            client.Send(from, recipients, subject, body);
            //StringBuilder sbJScript = new StringBuilder();
            checkCmd.Cancel();
            objReader.Close();
            Conn.Close();
            Conn.Dispose();

            Response.Write("<script>alert('密碼已寄到您的信箱！');</script>");
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
        labmsg.Text = "";
        txtAccount.Text = "";
        txtpwd.Text = "";
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        txtEmail.Text = "";
    }
}