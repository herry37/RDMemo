using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class custModifyData_aboutme : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Login"] != null)
        {
            string uid = Session["u_id"].ToString();
            Sqlregistered.SelectCommand = "SELECT * FROM [會員] where 帳號='" + uid + "'";
        }
        else
        {
            Response.Write("<script>alert('請先登入會員！'); location.href='memberLoginValidate.aspx';</script>");
            // Response.Redirect("memberLoginValidate.aspx");
        }
       // HttpBrowserCapabilities hbc = Request.Browser;
       //// Response.Write(hbc.Browser.ToString() + "<br/>"); //取得瀏覽器名稱
       // //Response.Write(hbc.Version.ToString() + "<br/>"); //取得瀏覽器版本號
       // //Response.Write(hbc.Platform.ToString() + "<br/>");     //取得作業系統名稱

       // if (hbc.Browser.ToString() == "Chrome")
       // {
       //     TextBox u_about = (TextBox)DataList1.EditItemTemplate("txtAboutme");
       //     u_about.Columns = 70;//Rows="15" Columns="73"
       //     u_about.Rows = 15;
       // }


       
    }

    protected void DataList1_EditCommand(object source, DataListCommandEventArgs e)
    {
        DataList1.EditItemIndex = e.Item.ItemIndex;
        DataList1.DataBind();
    }
    protected void DataList1_CancelCommand(object source, DataListCommandEventArgs e)
    {
        DataList1.EditItemIndex = -1;
        DataList1.DataBind();
    }
    protected void DataList1_UpdateCommand(object source, DataListCommandEventArgs e)
    {
        TextBox u_about = (TextBox)e.Item.FindControl("txtAboutme");


        Sqlregistered.UpdateParameters["其他"].DefaultValue = u_about.Text;
        Sqlregistered.UpdateParameters["帳號"].DefaultValue = Session["u_id"].ToString();
        Sqlregistered.Update();

        DataList1.EditItemIndex = -1;
        DataList1.DataBind();
    }
}