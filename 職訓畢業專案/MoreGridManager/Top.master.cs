using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class Top : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Session["Login_M"]==null)
        {
            lbtnLogin.Text = "登入";
            labetitle.Visible = false;
            Response.Redirect("Login.aspx");
            //legal();
        }
        else
        {
            lbtnLogin.Text = "登出";
            labetitle.Visible = true;
            labusername.Text = Session["username"].ToString();
            if (!IsPostBack)
            {
                Range();
            }
        }
        //if (Convert.ToInt16(Session["Login_M"]) == 4)
        //{
        //    lbtnLogin.Text = "登出";
        //    labetitle.Visible = true;
        //    labusername.Text = Session["username"].ToString();
        //    if(!IsPostBack)
        //    {
        //        Range();     
        //    }
        //}
        //else 
        //{
        //    lbtnLogin.Text = "登入";
        //    labetitle.Visible = false;
        //    legal();
           
        //}
    }
    //protected void legal()
    //{
    //    if (Convert.ToInt16(Session["Login_M"]) != 4)
    //    {
    //        Session.Clear();
    //        //Response.Write("<script> location.href='login.aspx'; </script>");
    //        Response.Redirect("Login.aspx");
    //    }
    //}

    protected void lbtnLogin_Click(object sender, EventArgs e)
    {
        //if(Convert.ToInt16(Session["Login_M"])==4)
        //{
            Session["PageName"]="logout";
            Session["action"] = "logout";
            pageLog();
        //}
        Session.Clear();
        Server.Transfer("Login.aspx");
    }
    protected void pageLog()
    {       
        using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString))
        {
            Conn.Open();
            using (SqlDataSource sql1 = new SqlDataSource())
            {
                sql1.ConnectionString = Conn.ConnectionString;
                sql1.InsertParameters.Add("員工帳號", Session["account"].ToString());
                sql1.InsertParameters.Add("操作頁面", Session["PageName"].ToString());
                sql1.InsertParameters.Add("操作動作", Session["action"].ToString());
                sql1.InsertParameters.Add("操作時間", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //  sql1.InsertParameters.Add("備註", Session["descript"].ToString());
                sql1.InsertCommand = "INSERT INTO [員工使用記錄] ([員工帳號], [操作頁面],[操作動作],[操作時間]) VALUES (@員工帳號, @操作頁面,@操作動作,@操作時間)";
                sql1.Insert();
                Conn.Close();
            }
        }
    }
    protected void Range()
    {
        int h1, h2;
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand objCmd = new SqlCommand("select * from [權限範圍] where (權限代碼 ='" + Session["authorityno"] + "')", Conn);
        Conn.Open();
        SqlDataReader dr = objCmd.ExecuteReader();
        dr.Read();
        T1.Visible = Convert.ToBoolean(dr[1]);
        T2.Visible = Convert.ToBoolean(dr[2]);
        T3.Visible = Convert.ToBoolean(dr[3]);
        T4.Visible = Convert.ToBoolean(dr[4]);
        T5.Visible = Convert.ToBoolean(dr[5]);
        T6.Visible = Convert.ToBoolean(dr[6]);
        T7.Visible = Convert.ToBoolean(dr[7]);
        T8.Visible = Convert.ToBoolean(dr[8]);
        T9.Visible = Convert.ToBoolean(dr[9]);
        T10.Visible = Convert.ToBoolean(dr[10]);
        T11.Visible = Convert.ToBoolean(dr[11]);
        Session["T1"] = dr[1];
        Session["T2"] = dr[2];
        Session["T3"] = dr[3];
        Session["T4"] = dr[4];
        Session["T5"] = dr[5];
        Session["T6"] = dr[6];
        Session["T7"] = dr[7];
        Session["T8"] = dr[8];
        Session["T9"] = dr[9];
        Session["T10"] = dr[10];
        Session["T11"] = dr[11];

        h1 = Convert.ToInt16(dr[1]) + Convert.ToInt16(dr[2])+Convert.ToInt16(dr[10])+Convert.ToInt16(dr[11]);
        h2 = Convert.ToInt16(dr[3]) + Convert.ToInt16(dr[4]) + Convert.ToInt16(dr[5]) + Convert.ToInt16(dr[6]) + Convert.ToInt16(dr[7]) + Convert.ToInt16(dr[8]);
        if (h1 == 0) outside.Visible = false;
        if (h2 == 0) inside.Visible = false;
        Conn.Close();
    }



}
