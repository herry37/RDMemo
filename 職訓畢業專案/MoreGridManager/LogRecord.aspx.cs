using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這

public partial class LogRecord : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Login_M"]==null)
        {
            Response.Redirect("Login.aspx");

        }
        if (!IsPostBack)
        {
            if (Convert.ToBoolean(Session["T8"]) != true)
                Response.Redirect("Login.aspx");

             Session["PageName"] = "網站操作紀錄";
             Session["action"] = "查詢";
             Session["descript"] = "瀏覽";
             pageLog();
             txttimeE.Text = DateTime.Now.ToString("yyyy/MM/dd");
             txttimeF.Text = DateTime.Now.ToString("yyyy/MM/dd");
            
        }
        if (Session["account"].ToString().ToLower() == "k_admin")
        {
            SqlDataSource1.SelectCommand = @"SELECT * FROM [員工使用記錄] WHERE (([員工帳號] LIKE '%' + @員工帳號 + '%')
                AND ((@操作頁面='0') OR ([操作頁面] = @操作頁面))
                AND ((@操作動作='0') OR ([操作動作] = @操作動作))
                AND (convert(date,操作時間) between @時間F AND @時間E))";
        }
    }


    protected void pageLog()
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        Conn.Open();
        SqlDataSource sql2 = new SqlDataSource();
        sql2.ConnectionString = Conn.ConnectionString;
        sql2.InsertParameters.Add("員工帳號", Session["account"].ToString());
        sql2.InsertParameters.Add("操作頁面", Session["PageName"].ToString());
        sql2.InsertParameters.Add("操作動作", Session["action"].ToString());
        sql2.InsertParameters.Add("操作時間", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sql2.InsertParameters.Add("備註", Session["descript"].ToString());
        sql2.InsertCommand = "INSERT INTO [員工使用記錄] ([員工帳號], [操作頁面],[操作動作],[操作時間],[備註]) VALUES (@員工帳號, @操作頁面,@操作動作,@操作時間,@備註)";
        sql2.Insert();
        Conn.Close();
    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList2.Items.Clear();
        DropDownList2.Items.Insert(0, new ListItem("全部", "0"));
    }
    protected void page_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is LinkButton)
        {
            switch (((LinkButton)(sender)).ID)
            {
                case "Linkpage1":
                    if (GridView1.PageIndex > 0)
                        GridView1.PageIndex--;
                    break;
                case "Linkpage2":
                    if (GridView1.PageIndex < (GridView1.PageCount - 1))
                        GridView1.PageIndex++;
                    break;
            }
        }
        else
        {
            GridViewRow pageRow = GridView1.BottomPagerRow;
            DropDownList ddlpageNum = (DropDownList)pageRow.Cells[0].FindControl("ddlpage");
            GridView1.PageIndex = ddlpageNum.SelectedIndex;
        }
    }

    protected void GridView1_DataBound(object sender, EventArgs e)
    {
        if (GridView1.PageCount != 0)
        {
            GridViewRow pageRow = GridView1.BottomPagerRow;
            Label pageNum = (Label)pageRow.Cells[0].FindControl("labNum");
            pageNum.Text = "Page: " + (GridView1.PageIndex + 1) + " of " + GridView1.PageCount + "&nbsp ";
            DropDownList ddlpageNum = (DropDownList)pageRow.Cells[0].FindControl("ddlpage");
            for (int i = 0; i < GridView1.PageCount; i++)
            {
                ListItem item = new ListItem((i + 1).ToString());
                if (i == GridView1.PageIndex)
                    item.Selected = true;
                ddlpageNum.Items.Add(item);
            }
        }
    }

    protected void SqlDataSource1_Selected(object sender, SqlDataSourceStatusEventArgs e)
    {
        Label12.Text = "總共有 " + e.AffectedRows.ToString() + " 筆記錄";

    }
}