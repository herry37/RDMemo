using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class GoodsClass : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            if (Convert.ToBoolean(Session["T1"]) != true)
                Response.Redirect("Login.aspx");

            Session["PageName"] = "商品類別管理";
            Session["descript"] = "";
            Session["action"] = "";
        }
        labgoods.Text = "";
        
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        //  InsertCommand="INSERT INTO [商品類別] ([商品類別名稱]) VALUES (@商品類別名稱)" 
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand objCmd = new SqlCommand("select * from [商品類別] where [商品類別名稱] = @labGoodsName", Conn);
        objCmd.Parameters.AddWithValue("labGoodsName", txtGoodsName.Text);
        Conn.Open();
        SqlDataReader dr = objCmd.ExecuteReader();
        if (dr.HasRows)
        {
            labgoods.ForeColor = System.Drawing.Color.Red;
            labgoods.Text = txtGoodsName.Text + "  --資料重複--";
        }
        else
        {
            SqlDataSource1.InsertParameters.Add("商品類別名稱", txtGoodsName.Text);
            SqlDataSource1.InsertCommand = "INSERT INTO [商品類別] ([商品類別名稱]) VALUES (@商品類別名稱)";
            int addMsg = SqlDataSource1.Insert();
            if (addMsg == 0)
            {
                labgoods.ForeColor = System.Drawing.Color.Red;
                labgoods.Text = txtGoodsName.Text + "  --新增失敗！--";
            }

            else
            {
                labgoods.ForeColor = System.Drawing.Color.Blue;
                Session["action"] = "新增";
                Session["descript"] = txtGoodsName.Text;
                pageLog();
                labgoods.Text = txtGoodsName.Text + "  --新增成功！--";

            }

        }
        Conn.Close();
        GridView1.DataBind();
        txtGoodsName.Text = "";
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1 && e.Row.RowIndex == GridView1.EditIndex)
        {
            TextBox goods = (TextBox)e.Row.FindControl("txtEditname");
            Session["descript"] = "[" + GridView1.DataKeys[e.Row.RowIndex].Value + "]," + goods.Text;
            goods.Width = 120;
            goods.MaxLength = 30;
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
            int i = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "Delete")
            {
                Session["descript"] = ((Label)GridView1.Rows[i].Cells[2].FindControl("labname")).Text;
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