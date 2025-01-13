using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這


public partial class CompanyManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Convert.ToBoolean(Session["T5"]) != true)
                Response.Redirect("Login.aspx");

            Session["descript"] = "";
            Session["action"] = "";
        }
        labdep.Text = "";
        labemp.Text = "";

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand objCmd = new SqlCommand("select * from [部門] where [部門名稱] = @depName", Conn);
        objCmd.Parameters.AddWithValue("depName", txtDepName.Text);
        Conn.Open();
        SqlDataReader dr = objCmd.ExecuteReader();
        if (dr.HasRows)
        {
            labdep.ForeColor = System.Drawing.Color.Red;
            labdep.Text = txtDepName.Text + "  --資料重複--";
        }
        else
        {
            SqlDataSource1.InsertParameters.Add("部門名稱", txtDepName.Text);
            SqlDataSource1.InsertCommand = "INSERT INTO [部門] ([部門名稱]) VALUES (@部門名稱)";
            int addMsg = SqlDataSource1.Insert();
            if (addMsg == 0)
            {
                labdep.ForeColor = System.Drawing.Color.Red;
                labdep.Text = txtDepName.Text + "  --新增失敗！--";
            }

            else
            {
                labdep.ForeColor = System.Drawing.Color.Blue;
                labdep.Text = txtDepName.Text + "  --新增成功！--";
                Session["PageName"]="部門名稱";
                Session["action"] = "新增";
                Session["descript"] = txtDepName.Text;
                pageLog();

            }

        }
        Conn.Close();
        GridView1.DataBind();
        txtDepName.Text = "";
    }

    //protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    //{
     
    //    if (txtAreaNo.Text == "")
    //        labtemp.Text = "0";
    //    else
    //        labtemp.Text = txtAreaNo.Text;
           
    //}

    protected void Button2_Click(object sender, EventArgs e)
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand objCmd = new SqlCommand("select * from [職稱] where [職稱名稱] = @empName", Conn);
        objCmd.Parameters.AddWithValue("empName", txtEmpName.Text);
        Conn.Open();
        SqlDataReader dr = objCmd.ExecuteReader();
        if (dr.HasRows)
        {
            labemp.ForeColor = System.Drawing.Color.Red;
            labemp.Text = txtEmpName.Text + "  --資料重複--";
        }
        else
        {
            SqlDataSource1.InsertParameters.Add("職稱名稱", txtEmpName.Text);
            SqlDataSource1.InsertCommand = "INSERT INTO [職稱] ([職稱名稱]) VALUES (@職稱名稱)";
            int addMsg = SqlDataSource1.Insert();
            if (addMsg == 0)
            {
                labemp.ForeColor = System.Drawing.Color.Red;
                labemp.Text = txtEmpName.Text + "  --新增失敗！--";
            }

            else
            {
                labemp.ForeColor = System.Drawing.Color.Blue;
                labemp.Text = txtEmpName.Text + "  --新增成功！--";
                Session["PageName"]="職稱名稱";
                Session["action"] = "新增";
                Session["descript"] = txtEmpName.Text;
                pageLog();
            }
        }
        Conn.Close();
        GridView2.DataBind();
        txtEmpName.Text = ""; 
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1 && e.Row.RowIndex == GridView1.EditIndex)
        {
            TextBox dep = (TextBox)e.Row.Cells[2].Controls[0];
            Session["descript"] = "[" + GridView1.DataKeys[e.Row.RowIndex].Value + "]," + dep.Text;
            dep.Width = 120;
            dep.MaxLength = 10;
        }
    }
    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1 && e.Row.RowIndex == GridView2.EditIndex)
        {
            TextBox emp = (TextBox)e.Row.Cells[2].Controls[0];
            Session["descript"] = "[" + GridView2.DataKeys[e.Row.RowIndex].Value + "]," + emp.Text;
            emp.Width = 120;
            emp.MaxLength = 20;
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
            GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            int i = row.RowIndex;
            if (e.CommandName == "Delete")
            {
                Session["descript"] = GridView1.Rows[i].Cells[2].Text;
            }
            Session["PageName"] = "部門名稱"; ;
            Session["action"] = e.CommandName.ToString();
            pageLog();
        }
    }
    protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
       
        if (e.CommandName == "Delete" || e.CommandName == "Update")
        {
            GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            int i = row.RowIndex;
            if (e.CommandName == "Delete")
            {
                Session["descript"] = GridView2.Rows[i].Cells[2].Text;
            }
            Session["PageName"] = "職稱名稱";
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
    protected void GridView2_RowCreated(object sender, GridViewRowEventArgs e)
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