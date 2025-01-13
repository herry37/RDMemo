using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這

public partial class AuthorityManageEmp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Convert.ToBoolean(Session["T6"]) != true)
                Response.Redirect("Login.aspx");

             Session["PageName"] = "員工權限管理";
             Session["action"] = "";
             Session["descript"] = "";
        }
       
    }

    protected void DropDownList1_Init(object sender, EventArgs e)
    {
        DropDownList dl = sender as DropDownList;
        if (Session["authorityno"].ToString() == "07")
            dl.DataSource = SqlDataSource4;
        else
            dl.DataSource = SqlDataSource5;
        
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
        if ( e.CommandName == "Update")
        {
            int i = Convert.ToInt32(e.CommandArgument);
            //GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            //int i = row.RowIndex;
            Session["action"] = e.CommandName.ToString();
            Session["descript"] = GridView1.Rows[i].Cells[2].Text;
            pageLog();
        }
    }
//    protected void Button1_Click(object sender, EventArgs e)
//    {
//        switch (ddlstatus.SelectedValue.ToString())
//        {
//            case "True":
//            case "False":
//                SqlDataSource3.SelectCommand = @"SELECT 員工帳號基本資料.姓名,部門.部門名稱,職稱.職稱名稱,員工帳號.員工帳號,員工帳號.啟用,員工帳號.權限代碼,帳號使用權限.權限名稱  FROM [員工帳號基本資料]
//                inner join 部門 on 員工帳號基本資料.部門代號=部門.部門代號
//                inner join 職稱 on 員工帳號基本資料.職稱代號=職稱.職稱代號
//                inner join 員工帳號 on 員工帳號基本資料.員工帳號=員工帳號.員工帳號
//                inner join 帳號使用權限 on 員工帳號.權限代碼=帳號使用權限.權限代碼
//                WHERE (((@部門代號=0) OR (部門.部門代號=@部門代號))
//                AND ((@職稱代號=0) OR (職稱.職稱代號=@職稱代號))
//                AND (員工帳號基本資料.員工帳號 LIKE '%'+@員工帳號+'%')
//                AND (員工帳號基本資料.姓名 LIKE '%'+@姓名+'%')
//                AND (啟用=@啟用))";
//                break;
//            default:
//                break;
//        }
//    }
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

    protected void SqlDataSource3_Selected(object sender, SqlDataSourceStatusEventArgs e)
    {
        
        Label12.Text = "總共有 " + e.AffectedRows + " 筆記錄";

    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
       
        if (Session["authorityno"].ToString() != "07")
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HiddenField h1 = (HiddenField) e.Row.FindControl("hidNo");
                 if (h1.Value  == "07")
                     e.Row.Cells[7].Controls[0].Visible = false;
            }
        }
    }
    
}