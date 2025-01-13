using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這
//using System.Drawing;
//using System.IO;
using System.Web.Security;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;


public partial class GoodsManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Convert.ToBoolean(Session["T10"]) != true)
                Response.Redirect("Login.aspx");

            Session["PageName"] = "商品管制設定";
            Session["action"] = "瀏覽";
            Session["descript"] = "";
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


    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Select")
        {
                Panel1.Visible = false;
                Panel2.Visible = true;
                FormView1.DataBind();
        }
        if (e.CommandName == "Update") 
        {
            //GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            //int i = row.RowIndex;
            int i = Convert.ToInt32(e.CommandArgument);
            Session["action"] = e.CommandName.ToString();
            Session["descript"] = GridView1.Rows[i].Cells[0].Text +","+ GridView1.Rows[i].Cells[1].Text +","+ GridView1.Rows[i].Cells[4].Text;
            pageLog();
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

    protected void Button1_Click(object sender, EventArgs e)
    {
        Panel1.Visible = true;
        Panel2.Visible = false;
        DataBind();
        GridView1.SelectedIndex = -1;
        //GridView1.SelectedRowStyle.Reset();
    }

    protected void SqlDataSource2_Selected(object sender, SqlDataSourceStatusEventArgs e)
    {
        Label12.Text = "總共有 " + e.AffectedRows.ToString() + " 筆記錄";

    }
    protected void FormView1_DataBound(object sender, EventArgs e)
    {
       
            Label labedit = (Label)FormView1.FindControl("LabEdit");
            if (labedit!=null)
            {
                String stredit = labedit.Text;
                labedit.Text = Server.HtmlDecode(stredit);
            }
        
    }
    protected string check_null(string e_img)
    {
        if (Eval(e_img).ToString() == "" || Eval(e_img).ToString() == null)
            return "Gshan_200x200.png";
        else return Eval(e_img).ToString();
    }
}