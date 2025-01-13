using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這


public partial class PersonInfo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Convert.ToBoolean(Session["T9"]) != true)
                Response.Redirect("Login.aspx");

            Session["PageName"] = "個人資料管理";
            Session["descript"] = "";
            Session["action"] = "";
        }
            
           
    }

    public string FormatSex(bool sex)

    { return sex ? "男" : "女"; } 

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
        Calendar Ca = (Calendar)sender;
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
    protected void TextBox2_TextChanged(object sender, EventArgs e)
    {
        SqlDataSource4.UpdateCommand="UPDATE [員工帳號基本資料] SET 部門代號=@部門代號,職稱代號=@職稱代號,姓名=@姓名,性別=@性別,出生日期=@出生日期,email=@email,聯絡電話=@聯絡電話,郵遞代號=@郵遞代號,地址=@地址 WHERE 員工帳號 = @員工帳號;UPDATE [員工帳號] SET [員工密碼] = @員工密碼 WHERE [員工帳號] = @員工帳號";
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
        //sql1.InsertParameters.Add("備註", Session["descript"].ToString());
        sql1.InsertCommand = "INSERT INTO [員工使用記錄] ([員工帳號], [操作頁面],[操作動作],[操作時間]) VALUES (@員工帳號, @操作頁面,@操作動作,@操作時間)";
        sql1.Insert();
        Conn.Close();
    }
    protected void DetailsView1_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "Update":
                Session["action"] = e.CommandName.ToString();
                pageLog();
                break;
            default:
                break;
        }
    }
}