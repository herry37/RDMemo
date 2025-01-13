using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這

public partial class AuthorityManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Convert.ToBoolean(Session["T3"]) != true)
                Response.Redirect("Login.aspx");

            Session["PageName"] = "權限類別管理";
            Session["descript"] = "";
            Session["action"] = "";
        }

    }
    

    protected void Button1_Click(object sender, EventArgs e)
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand objCmd = new SqlCommand("select * from [帳號使用權限] where [權限代碼] = @txtno", Conn);
        objCmd.Parameters.AddWithValue("txtno", txtno.Text);
        Conn.Open();
        SqlDataReader dr = objCmd.ExecuteReader();
        if (dr.HasRows)
        {
            labmsg.ForeColor = System.Drawing.Color.Red;
            labmsg.Text = txtno.Text +"/" +txtname.Text+"  --資料重複--";
        }
        else
        {
            SqlDataSource1.InsertParameters.Add("權限代碼", txtno.Text);
            SqlDataSource1.InsertParameters.Add("權限名稱", txtname.Text);
            SqlDataSource1.InsertParameters.Add("T1", "False");
            SqlDataSource1.InsertParameters.Add("T2", "False");
            SqlDataSource1.InsertParameters.Add("T3", "False");
            SqlDataSource1.InsertParameters.Add("T4", "False");
            SqlDataSource1.InsertParameters.Add("T5", "False");
            SqlDataSource1.InsertParameters.Add("T6", "False");
            SqlDataSource1.InsertParameters.Add("T7", "False");
            SqlDataSource1.InsertParameters.Add("T8", "False");
            SqlDataSource1.InsertParameters.Add("T9", "False");
            SqlDataSource1.InsertParameters.Add("T10", "False");
            SqlDataSource1.InsertCommand = "INSERT INTO [帳號使用權限] (權限代碼,權限名稱) VALUES (@權限代碼,@權限名稱);INSERT INTO [權限範圍] (權限代碼,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10) VALUES (@權限代碼,@T1,@T2,@T3,@T4,@T5,@T6,@T7,@T8,@T9,@T10)";
            int addMsg = SqlDataSource1.Insert();
            if (addMsg == 0)
            {
                labmsg.ForeColor = System.Drawing.Color.Red;
                labmsg.Text = txtno.Text + "/" + txtname.Text + "  --新增失敗！--";
            }

            else
            {
                labmsg.ForeColor = System.Drawing.Color.Blue;
                labmsg.Text = txtno.Text + "/" + txtname.Text + "  --新增成功！--";
                Session["action"] = "新增";
                Session["descript"] = txtno.Text+","+txtname.Text;
                pageLog();
            }

        }
        Conn.Close();
        GridView1.DataBind();
        txtno.Text = ""; txtname.Text = "";
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((e.Row.Cells[0].Text) == "01" || (e.Row.Cells[0].Text) == "07")
            {
                LinkButton btn_del = (LinkButton)e.Row.Cells[2].FindControl("btn_del");
                if(btn_del !=null)  btn_del.Visible = false;
            }
        }
        if(e.Row.RowIndex!=-1 && e.Row.RowIndex == GridView1.EditIndex)
        {
                TextBox name = (TextBox)e.Row.Cells[1].Controls[0];
                Session["descript"] = GridView1.DataKeys[e.Row.RowIndex].Value+","+ name.Text;
                name.Width = 120;
                name.MaxLength = 20;
        }
    }
    protected void pageLog()
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        Conn.Open();
        SqlDataSource sql1 = new SqlDataSource();
        sql1.ConnectionString = Conn.ConnectionString;
        sql1.InsertParameters.Add("員工帳號", Session["account"].ToString());
        sql1.InsertParameters.Add("操作頁面", Session["PageName"].ToString());
        sql1.InsertParameters.Add("操作動作", Session["action"].ToString());
        sql1.InsertParameters.Add("操作時間", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sql1.InsertParameters.Add("備註", Session["descript"].ToString());
        sql1.InsertCommand = "INSERT INTO [員工使用記錄] ([員工帳號], [操作頁面],[操作動作],[操作時間],[備註]) VALUES (@員工帳號, @操作頁面,@操作動作,@操作時間,@備註)";
        sql1.Insert();
        Conn.Close();
    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {


        if (e.CommandName == "Delete" || e.CommandName == "Update")
        {
            //GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            //int i = row.RowIndex;
            if (e.CommandName == "Delete")
            {
                int i = Convert.ToInt32(e.CommandArgument);
                Session["descript"] = GridView1.Rows[i].Cells[1].Text;
            }
            Session["action"] = e.CommandName.ToString();
            pageLog();
        }
    }
    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((e.Row.Cells[3].FindControl("btn_del") != null) && (Session["authorityno"].ToString() != "07"))
            {
                e.Row.Cells[3].FindControl("btn_del").Visible = false;
            }

        }

    }
}