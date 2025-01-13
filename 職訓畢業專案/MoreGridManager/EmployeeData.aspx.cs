using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這


public partial class EmployeeData : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Convert.ToBoolean(Session["T7"]) != true)
                Response.Redirect("Login.aspx");

            Session["PageName"] = "員工資料管理";
            Session["descript"] = "";
            Session["action"] = "";
        }
    }

    public string FormatSex(bool sex)

    { return sex ? "男" : "女"; } 

    protected void DetailsView1_DataBound(object sender, EventArgs e)
    {
        GridView1.DataBind();
        

    }
    protected void DropDownList1_Init(object sender, EventArgs e)
    {
        DropDownList dl = sender as DropDownList;
        dl.DataSource = SqlDataSource1;
    }

    protected void DropDownList2_Init(object sender, EventArgs e)
    {
        DropDownList d2 = sender as DropDownList;
        d2.DataSource = SqlDataSource2;
    }
    protected void Calendar1_SelectionChanged(object sender, EventArgs e)
    {
        Calendar Ca = (Calendar) sender;
        TextBox Tb = (TextBox)Ca.FindControl("txtBirth");
        Tb.Text = Ca.SelectedDate.ToShortDateString();
    }
    protected void ddl4_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl4 = (DropDownList)DetailsView1.FindControl("ddl4");
        TextBox tb = (TextBox)ddl4.FindControl("txtareano");
        tb.Text = ddl4.SelectedValue;
    }

    protected void ddl3_DataBound(object sender, EventArgs e)
    {
        DropDownList ddl4 = (DropDownList)DetailsView1.FindControl("ddl4");
        Label labareano = (Label)DetailsView1.FindControl("lab4");
        ddl4.SelectedValue = labareano.Text;
    }
    protected void ddl3_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl4 = (DropDownList)DetailsView1.FindControl("ddl4");  
        ddl4.Items.Clear();
        ddl4.Items.Insert(0, new ListItem("請選擇", "0"));
    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Select" || e.CommandName == "Update" || e.CommandName == "Delete")
        {
            int i = Convert.ToInt32(e.CommandArgument); ;
            //GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            //int i = row.RowIndex;
            Session["action"] = e.CommandName.ToString();
            Session["descript"] = GridView1.Rows[i].Cells[2].Text;

            pageLog();
        }
        if(e.CommandName=="Select")
        {
            Panel1.Visible = false;
            Panel2.Visible = true;
        }
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if(e.Row.RowIndex!=-1)
        {
             //string Script ="javascript:return confirm('確定刪除" ;
           
            if (((LinkButton)e.Row.Cells[6].Controls[2]).CommandName == "Delete")
            {
                ((LinkButton)e.Row.Cells[6].Controls[2]).Attributes["onclick"] = "return confirm('確定刪除【" + e.Row.Cells[2].Text + "】的資料嗎？');";
                //((LinkButton)e.Row.Cells[6].Controls[2]).Attributes["onclick"] = ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", Script, true);

            }

        }


    }

    protected void DetailsView1_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        if(e.CommandName=="CancelEdit")
        {
            Panel2.Visible = false;
            Panel1.Visible = true;
            GridView1.SelectedIndex = -1;
        }
        if(e.CommandName=="Update")
        {
            Session["action"] = e.CommandName.ToString();
            Session["descript"] = DetailsView1.Rows[2].Cells[1].Text;
            pageLog();
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
    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((LinkButton)e.Row.Cells[6].Controls[2]).CommandName == "Delete" && (Session["authorityno"].ToString() != "07"))
            {
                ((LinkButton)e.Row.Cells[6].Controls[2]).Visible = false;
            }

            //if ((e.Row.Cells[3].FindControl("btn_del") != null) && (Session["authorityno"].ToString() != "07"))
            //{
            //    e.Row.Cells[3].FindControl("btn_del").Visible = false;
            //}

        }

    }
    protected void SqlDataSource3_Selected(object sender, SqlDataSourceStatusEventArgs e)
    {
        Label12.Text = "總共有 " + e.AffectedRows + " 筆記錄";

    }
}