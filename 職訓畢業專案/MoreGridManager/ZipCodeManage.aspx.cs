using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這


public partial class ZipCodeManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!IsPostBack)
        {
            if (Convert.ToBoolean(Session["T4"]) != true)
                Response.Redirect("Login.aspx");

            labtemp.Text = "0";
            Session["descript"] = "";
            Session["action"] = "";
            Session["PageName"] = "";
        }
        labCity.Text = "";
        labArea.Text = "";

       
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand objCmd = new SqlCommand("select * from [縣市] where [縣市] = @cityName",Conn);
        objCmd.Parameters.AddWithValue("cityName",txtCity.Text);
        Conn.Open();
        SqlDataReader dr = objCmd.ExecuteReader();
        if(dr.HasRows)
        {
            labCity.ForeColor = System.Drawing.Color.Red;
            labCity.Text=txtCity.Text+"  --資料重複--";
        }
        else
        {
            SqlDataSource1.InsertParameters.Add("縣市",txtCity.Text);
            SqlDataSource1.InsertCommand = "INSERT INTO [縣市] ([縣市]) VALUES (@縣市)";
            int addMsg=SqlDataSource1.Insert();
            if (addMsg == 0)
            {
                labCity.ForeColor = System.Drawing.Color.Red;
                labCity.Text=txtCity.Text+"  --新增失敗！--";
            }
               
            else
            {
                labCity.ForeColor = System.Drawing.Color.Blue;
                labCity.Text=txtCity.Text+"  --新增成功！--";
                 Session["PageName"]="縣市";
                Session["action"] = "新增";
                Session["descript"] = txtCity.Text;
                pageLog();
            }
                
        }
        Conn.Close();
        GridView1.DataBind();
        txtCity.Text = "";
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (txtAreaNo.Text == "")
            labtemp.Text = "0";
        else
            labtemp.Text = txtAreaNo.Text;

               
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand objCmd = new SqlCommand(" SELECT * FROM 郵遞區號 WHERE 縣市代碼 = @cityNo AND 鄉鎮市區 = @cityAreaName", Conn);
        objCmd.Parameters.AddWithValue("cityNo", ddl1.SelectedValue);
        objCmd.Parameters.AddWithValue("cityAreaName", txtAreaName.Text);
        Conn.Open();
        SqlDataReader dr = objCmd.ExecuteReader();
        if (dr.HasRows)
        {
            labArea.ForeColor = System.Drawing.Color.Red;
            labArea.Text = txtAreaNo.Text + "/" + ddl1.SelectedItem.Text + "/" + txtAreaName.Text + " --重複--\t----資料新增失敗----";
        }
        else
        {
            SqlDataSource4.InsertParameters.Add("縣市代碼", ddl1.SelectedValue);
            SqlDataSource4.InsertParameters.Add("郵遞區號", txtAreaNo.Text);
            SqlDataSource4.InsertParameters.Add("鄉鎮市區", txtAreaName.Text);
            SqlDataSource4.InsertCommand = "INSERT INTO [郵遞區號] ([郵遞區號], [縣市代碼], [鄉鎮市區]) VALUES (@郵遞區號, @縣市代碼, @鄉鎮市區)";
            int addMsg = SqlDataSource4.Insert();
            if (addMsg == 0)
            {
                labArea.ForeColor = System.Drawing.Color.Red;
                labArea.Text = txtAreaNo.Text + "/" + ddl1.SelectedItem.Text + "/" + txtAreaName.Text + "<br>----資料新增失敗----";
            }

            else
            {
                labArea.ForeColor=System.Drawing.Color.Blue;
                labArea.Text = txtAreaNo.Text+"/"+ddl1.SelectedItem.Text +"/"+txtAreaName.Text + "<br>----資料新增成功----";
                Session["PageName"]="郵遞區號";
                Session["action"] = "新增";
                Session["descript"] = txtAreaNo.Text+","+txtAreaName.Text;
                pageLog();
            }
        }

        Conn.Close();
        GridView2.DataBind();
        txtAreaNo.Text = ""; txtAreaName.Text = "";
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1 && e.Row.RowIndex == GridView1.EditIndex)
        {
            TextBox city = (TextBox)e.Row.Cells[2].Controls[0];
            Session["descript"] = "["+GridView1.DataKeys[e.Row.RowIndex].Value + "]," + city.Text;
            city.Width = 120;
            city.MaxLength = 10;
        }
    }

    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex != -1 && e.Row.RowIndex == GridView2.EditIndex)
        {
            TextBox areano = (TextBox)e.Row.Cells[0].Controls[0];
            TextBox city = (TextBox)e.Row.Cells[2].Controls[0];
            Session["descript"] = "["+GridView2.DataKeys[e.Row.RowIndex].Value+"],"+ areano.Text + "," + city.Text;
            city.Width = 120;
            city.MaxLength = 20;
        }
    }

    protected void page_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is LinkButton)
        {
            switch (((LinkButton)(sender)).ID)
            {
                case "Linkpage1":
                    if (GridView2.PageIndex > 0)
                        GridView2.PageIndex--;
                    break;
                case "Linkpage2":
                    if (GridView2.PageIndex < (GridView2.PageCount - 1))
                        GridView2.PageIndex++;
                    break;
            }
        }
        else
        {
            GridViewRow pageRow = GridView2.BottomPagerRow;
            DropDownList ddlpageNum = (DropDownList)pageRow.Cells[0].FindControl("ddlpage");
            GridView2.PageIndex = ddlpageNum.SelectedIndex;
        }
    }



    protected void GridView2_DataBound(object sender, EventArgs e)
    {
        if(GridView2.PageCount!=0)
        {
            GridViewRow pageRow = GridView2.BottomPagerRow;
            Label pageNum = (Label)pageRow.Cells[0].FindControl("labNum");
            pageNum.Text = "Page: " + (GridView2.PageIndex + 1)+ " of "+GridView2.PageCount+"&nbsp ";
            DropDownList ddlpageNum = (DropDownList)pageRow.Cells[0].FindControl("ddlpage");
            for (int i = 0; i < GridView2.PageCount; i++)
            {
                ListItem item = new ListItem((i + 1).ToString());
                if (i == GridView2.PageIndex)
                    item.Selected = true;
                ddlpageNum.Items.Add(item);
            }
        }
       

    }
    protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if(e.CommandName=="Delete" || e.CommandName=="Update")
        {
            GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            int i = row.RowIndex;
            //int i = Convert.ToInt32(e.CommandArgument);
               if (e.CommandName == "Delete")
               {
                   Session["descript"] = GridView2.Rows[i].Cells[0].Text + "," + GridView2.Rows[i].Cells[2].Text;
               }
           Session["PageName"] = "郵遞區號";
           Session["action"] = e.CommandName.ToString();
           pageLog();
        }
        
    //    if (e.CommandName == "Update")
    //    {
    //        //取得哪Row的列索引
    //        int index = Convert.ToInt32(e.CommandArgument);
    //        DropDownList drpcity = (DropDownList)GridView2.Rows[index].FindControl("ddlcity");
    //        labCity.Text = drpcity.SelectedValue;
    //        //◆在UpdateParameters 已有Gender 的項目
    //        SqlDataSource4.UpdateParameters["縣市代碼"].DefaultValue = drpcity.SelectedValue;
    //        //SqlDataSource4.Update();
    //    }
   }
    protected void GridView1_DataBound(object sender, EventArgs e)
    {
        ddl1.Items.Clear();
        ddl1.DataBind();
        ddl1.Items.Insert(0, new ListItem("請選擇", "0"));
        citySearch();
        GridView2.DataBind();

    }

     
    protected void ddlcity_Init(object sender, EventArgs e)
    {
        DropDownList dl = sender as DropDownList;
        dl.DataSource = SqlDataSource3;
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        citySearch();
    }
    protected void citySearch()
    {

        if (txtCity.Text == "全部" || txtCity.Text == "")
        {
            SqlDataSource1.SelectCommand = "SELECT * FROM [縣市]";
        }
        else
        {
            SqlDataSource1.SelectCommand = "SELECT * FROM [縣市] WHERE 縣市 LIKE '%'+@縣市+'%'";
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
        
        if(e.CommandName=="Delete" || e.CommandName=="Update")
        {
            GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            int i = row.RowIndex;
            if (e.CommandName == "Delete")
            {
                Session["descript"] = GridView1.Rows[i].Cells[2].Text;
            }
            Session["PageName"] = "縣市"; ;
            Session["action"] = e.CommandName.ToString();
            pageLog();
        }
       
    }

    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((e.Row.Cells[4].FindControl("btn_del") != null) && (Session["authorityno"].ToString() != "07"))
            {
                e.Row.Cells[4].FindControl("btn_del").Visible = false;
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
