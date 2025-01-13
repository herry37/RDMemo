using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using System.IO;
using System.Data;

public partial class GoodsShipper1 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((TextBox)FormView1.FindControl("tbMessage")).Attributes["maxlength"] = "50";
       
        if (Session["u_id"] == null || Session["u_id"] == "")
        {

            Response.Redirect("memberLoginValidate.aspx");
        }

        //  string label1=Request.QueryString["id"];


        // Label1.Text = label1;


    }

    protected void ddlcity_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlarea = (DropDownList)FormView1.FindControl("ddlarea");
        ddlarea.Items.Clear();
        ddlarea.Items.Insert(0, new ListItem("請選擇", "0"));
    }

    protected void FormView1_ItemInserting(object sender, FormViewInsertEventArgs e)
    {
        // 開資料庫         
        SqlDataSource SqlData = new SqlDataSource();
        SqlData.InsertParameters.Clear();
        SqlData.ConnectionString = WebConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString;


        //取text值 訂單
        //帳號
        SqlData.InsertParameters.Add("帳號", Session["u_id"].ToString());

        //下單日期
        SqlData.InsertParameters.Add("下單日期", DateTime.Now.ToString("yyyyMMdd"));
        //insert
        SqlData.InsertCommand = "INSERT INTO [訂單] ([下單日期], [帳號]) VALUES (@下單日期,@帳號)";
        SqlData.Insert();

        string sql = "server=127.0.0.1;database=C501_04;uid=C501_04;pwd=C501_04";
        SqlConnection conn = new SqlConnection(sql);
        SqlCommand command = new SqlCommand("SELECT MAX ([訂單編號]) FROM [訂單]", conn);
        conn.Open();
        SqlDataReader reader = command.ExecuteReader();
        reader.Read();
        String 訂單編號 = reader.GetInt64(0).ToString();
      

        //取text值 訂單名細
        //訂單編號 int         
        SqlData.InsertParameters.Add("訂單編號", 訂單編號);
        reader.Close();
        conn.Close();
        //商品編號 int
        string id = Request.QueryString["id"];
        SqlData.InsertParameters.Add("商品編號", id);
        //訂購數量 int
        TextBox tbNum = (TextBox)FormView1.FindControl("tbNum");
        SqlData.InsertParameters.Add("訂購數量", tbNum.Text);
        //抓運費
        TextBox tbShippment = (TextBox)FormView1.FindControl("tbShippment");

        //購買價格 money
        //TextBox tbPrice = (TextBox)FormView1.FindControl("tbPrice");      
        Label lbSum = (Label)FormView1.FindControl("lbSum");
        //lbSum = Convert.ToInt32(lbprice) * Convert.ToInt32(tbNum) + Convert.ToInt32(lbprice);
        SqlData.InsertParameters.Add("購買價格", lbSum.Text);
        //買家留言 string
        TextBox tbMessage = (TextBox)FormView1.FindControl("tbMessage");
        SqlData.InsertParameters.Add("買家留言", tbMessage.Text);
        //insert
        SqlData.InsertCommand = "INSERT INTO [訂單明細] ([訂單編號], [商品編號], [訂購數量], [購買價格], [買家留言]) VALUES (@訂單編號,@商品編號,@訂購數量,@購買價格,@買家留言)";
        SqlData.Insert();
        // 取text值 出貨地址 
        // 收貨人 varchar(30)
        TextBox tbReceiver = (TextBox)FormView1.FindControl("tbReceiver");
        SqlData.InsertParameters.Add("收貨人", tbReceiver.Text);
        //縣市代碼
        DropDownList ddlcity = (DropDownList)FormView1.FindControl("ddlcity");
        SqlData.InsertParameters.Add("縣市代碼", ddlcity.SelectedValue);
        //郵遞代碼
        DropDownList ddlarea = (DropDownList)FormView1.FindControl("ddlarea");
        SqlData.InsertParameters.Add("郵遞代碼", ddlarea.SelectedValue);
        //村里 varchar(30)
        TextBox txtAddress = (TextBox)FormView1.FindControl("txtAddress");
        SqlData.InsertParameters.Add("村里", txtAddress.Text);
        //Insert
        SqlData.InsertCommand = "INSERT INTO [收貨地址] ([村里], [收貨人], [郵遞代碼], [縣市代碼], [訂單編號]) VALUES (@村里,@收貨人,@郵遞代碼,@縣市代碼,@訂單編號)";
        SqlData.Insert();
        //資料清空
        tbNum.Text = "";
        tbShippment.Text = "";
        //tbPrice.Text = "";
        tbMessage.Text = "";
        txtAddress.Text = "";
        tbReceiver.Text = "";
        ddlarea.SelectedIndex = 0;
        ddlcity.SelectedIndex = 0;
        Response.Write("<script>confirm('已購買成功');location.href='Main.aspx';</script>");
    }
    protected void FormView1_ItemCommand(object sender, FormViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Cancel"))
        {
            string id = Request.QueryString["id"];      //抓網址上的ID後用的變數

            Response.Redirect("GoodsDisp.aspx?id=" + id);
        }
    }
    //protected void FormView1_DataBound(object sender, EventArgs e)
    //{
    //    TextBox tbShippment = (TextBox)FormView1.FindControl("tbShippment");
    //    Label lbprice = (Label)FormView1.FindControl("lbprice");
    //    TextBox tbNum = (TextBox)FormView1.FindControl("tbNum");
    //    Label lbSum = (Label)FormView1.FindControl("lbSum");
    //    if (tbShippment.Text != "" && lbprice.Text != "" && tbNum.Text!="")
    //    lbSum.Text = (Convert.ToInt32(lbprice.Text) * Convert.ToInt32(tbNum.Text) + Convert.ToInt32(tbShippment.Text)).ToString();

    //}
    protected void tbNum_TextChanged(object sender, EventArgs e)
    {
        TextBox tbShippment = (TextBox)FormView1.FindControl("tbShippment");
        //抓價格
        int goods_id = Convert.ToInt32(Request.QueryString["id"]);
        string sql = "server=127.0.0.1;database=C501_04;uid=C501_04;pwd=C501_04";
        SqlConnection conn = new SqlConnection(sql);
        SqlCommand command = new SqlCommand("select [單價] from [商品] where 商品編號=" + goods_id + "", conn);
        conn.Open();
        SqlDataReader reader = command.ExecuteReader();
        reader.Read();
        int price = Convert.ToInt32(reader[0]);
        reader.Close();
        conn.Close();
        //
        int a;
        TextBox tbNum = (TextBox)FormView1.FindControl("tbNum");
        Label lbSum = (Label)FormView1.FindControl("lbSum");
        if (tbShippment.Text != "" && price.ToString() != "" && tbNum.Text != "")
        {
            if (int.TryParse(tbShippment.Text, out a))
                lbSum.Text = (price * Convert.ToInt32(tbNum.Text) + Convert.ToInt32(tbShippment.Text)).ToString("#,#");
        }

    }
}