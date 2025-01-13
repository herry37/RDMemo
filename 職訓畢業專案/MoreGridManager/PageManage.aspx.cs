using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這


public partial class PageManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
               
        if (!IsPostBack)
        {
            labtemp.Text = "0";
            Session["descript"] = "";
            Session["action"] = "";
            Session["PageName"] = "";
        }
       
    }
    
    protected void Button1_Click(object sender, EventArgs e)
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand objCmd = new SqlCommand("select * from [頁面主項] where [主項標題] = @title",Conn);
        objCmd.Parameters.AddWithValue("title",txtTitle.Text);
        Conn.Open();
        SqlDataReader dr = objCmd.ExecuteReader();
        if(dr.HasRows)
        {
            labmsg.ForeColor = System.Drawing.Color.Red;
            labmsg.Text=txtTitle.Text+"  --資料重複--";
        }
        else
        {
            SqlDataSource1.InsertParameters.Add("主項標題", txtTitle.Text);
            SqlDataSource1.InsertCommand = "INSERT INTO [頁面主項] ([主項標題]) VALUES (@主項標題)";
            int addMsg=SqlDataSource1.Insert();
            if (addMsg == 0)
            {
                labmsg.ForeColor = System.Drawing.Color.Red;
                labmsg.Text=txtTitle.Text+"  --新增失敗！--";
            }
               
            else
            {
                labmsg.ForeColor = System.Drawing.Color.Blue;
                labmsg.Text=txtTitle.Text+"  --新增成功！--";
                 Session["PageName"]="頁面主項";
                Session["action"] = "新增";
                Session["descript"] = txtTitle.Text;
                pageLog();
            }
                
        }
        Conn.Close();
        GridView1.DataBind();
        txtTitle.Text = "";
        
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
               
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand objCmd = new SqlCommand(" SELECT * FROM 頁面細項 WHERE 主項流水號 = @TitleNo AND 細項標題 = @detailName", Conn);
        objCmd.Parameters.AddWithValue("TitleNo", ddl1.SelectedValue);
        objCmd.Parameters.AddWithValue("detailName", txtdetailName.Text);
        Conn.Open();
        SqlDataReader dr = objCmd.ExecuteReader();
        if (dr.HasRows)
        {
            labArea.ForeColor = System.Drawing.Color.Red;
            labArea.Text =  ddl1.SelectedItem.Text + "/" + txtdetailName.Text + " --重複--\t----資料新增失敗----";
        }
        else
        {
            SqlDataSource2.InsertParameters.Add("主項流水號", ddl1.SelectedValue);
            SqlDataSource2.InsertParameters.Add("細項標題", txtdetailName.Text);
            SqlDataSource2.InsertParameters.Add("網址", txtlink.Text);
            SqlDataSource2.InsertCommand = "INSERT INTO [頁面細項] ([主項流水號], [細項標題], [網址]) VALUES (@主項流水號,@細項標題,@網址)";
            int addMsg = SqlDataSource2.Insert();
            if (addMsg == 0)
            {
                labArea.ForeColor = System.Drawing.Color.Red;
                labArea.Text = ddl1.SelectedItem.Text + "/" + txtdetailName.Text + "<br>----資料新增失敗----";
            }

            else
            {
                labArea.ForeColor=System.Drawing.Color.Blue;
                labArea.Text = ddl1.SelectedItem.Text +"/"+txtdetailName.Text + "<br>----資料新增成功----";
                Session["PageName"]="細項標題";
                Session["action"] = "新增";
                Session["descript"] = ddl1.SelectedItem.Text+","+txtdetailName.Text;
                pageLog();
            }
        }

        Conn.Close();
        GridView2.DataBind();
        txtdetailName.Text = "";
        txtlink.Text = "";
    }

   // protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
   // {
   //     if (e.Row.RowIndex != -1 && e.Row.RowIndex == GridView1.EditIndex)
   //     {
   //         TextBox city = (TextBox)e.Row.Cells[2].Controls[0];
   //         Session["descript"] = "["+GridView1.DataKeys[e.Row.RowIndex].Value + "]," + city.Text;
   //         city.Width = 120;
   //         city.MaxLength = 10;
   //     }
   // }

   // protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
   // {
   //     if (e.Row.RowIndex != -1 && e.Row.RowIndex == GridView2.EditIndex)
   //     {
   //         TextBox areano = (TextBox)e.Row.Cells[0].Controls[0];
   //         TextBox city = (TextBox)e.Row.Cells[2].Controls[0];
   //         Session["descript"] = "["+GridView2.DataKeys[e.Row.RowIndex].Value+"],"+ areano.Text + "," + city.Text;
   //         city.Width = 120;
   //         city.MaxLength = 20;
   //     }
   // }





   // protected void GridView2_DataBound(object sender, EventArgs e)
   // {
   //     if(GridView2.PageCount!=0)
   //     {
   //         GridViewRow pageRow = GridView2.BottomPagerRow;
   //         Label pageNum = (Label)pageRow.Cells[0].FindControl("labNum");
   //         pageNum.Text = "Page: " + (GridView2.PageIndex + 1)+ " of "+GridView2.PageCount+"&nbsp ";
   //         DropDownList ddlpageNum = (DropDownList)pageRow.Cells[0].FindControl("ddlpage");
   //         for (int i = 0; i < GridView2.PageCount; i++)
   //         {
   //             ListItem item = new ListItem((i + 1).ToString());
   //             if (i == GridView2.PageIndex)
   //                 item.Selected = true;
   //             ddlpageNum.Items.Add(item);
   //         }
   //     }
       

   // }
   // protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
   // {

   //     if(e.CommandName=="Delete" || e.CommandName=="Update")
   //     {
   //        //  GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
   //        //  int i = row.RowIndex;
   //        int i = Convert.ToInt32(e.CommandArgument);

   //            if (e.CommandName == "Delete")
   //            {
   //                Session["descript"] = GridView2.Rows[i].Cells[0].Text + "," + GridView2.Rows[i].Cells[2].Text;
   //            }
   //        Session["PageName"] = "郵遞區號";
   //        Session["action"] = e.CommandName.ToString();
   //        pageLog();
   //     }
      
  
   //}
    protected void GridView1_DataBound(object sender, EventArgs e)
    {
        ddl1.Items.Clear();
        ddl1.Items.Insert(0, new ListItem("請選擇", "0"));
        GridView2.DataBind();

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

    protected void DropDownList1_Init(object sender, EventArgs e)
    {
        DropDownList dl = sender as DropDownList;
        dl.DataSource = SqlDataSource1;
    }
}
