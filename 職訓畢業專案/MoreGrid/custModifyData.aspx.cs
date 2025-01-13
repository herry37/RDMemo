using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class custModifyData : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Login"] != null) {
            string uid = Session["u_id"].ToString();
            Sqlregistered.SelectCommand = "SELECT 會員.*,會員住址.*,郵遞區號.*,縣市.* FROM 會員 inner join 會員住址 on 會員.帳號=會員住址.帳號 inner join 郵遞區號 on 會員住址.郵遞代碼=郵遞區號.郵遞代碼 inner join 縣市 on 縣市.縣市代碼=郵遞區號.縣市代碼  where 會員.帳號='" + uid + "'";
       }
        else
        {
            Response.Write("<script>alert('請先登入會員！'); location.href='memberLoginValidate.aspx';</script>");
           // Response.Redirect("memberLoginValidate.aspx");
        }
        //Label txtPsd = (Label)DataList1.NamingContainer.FindControl("txtPsd");
        
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
        TextBox u_uid = (TextBox)e.Item.FindControl("txtName");
        TextBox u_email = (TextBox)e.Item.FindControl("txtEmail");
        TextBox u_tel = (TextBox)e.Item.FindControl("txtTel");
        TextBox u_Mobile = (TextBox)e.Item.FindControl("txtMobile");
        Label u_id = (Label)e.Item.FindControl("txtNo");
        DropDownList u_addcity = (DropDownList)e.Item.FindControl("ddl3");
        DropDownList u_addarea = (DropDownList)e.Item.FindControl("ddl4");
        TextBox u_add = (TextBox)e.Item.FindControl("txtadd");
        TextBox u_address = (TextBox)e.Item.FindControl("txtaddress");

        Sqlregistered.UpdateParameters["暱稱"].DefaultValue = u_uid.Text;
        Sqlregistered.UpdateParameters["email"].DefaultValue = u_email.Text;
        Sqlregistered.UpdateParameters["市話"].DefaultValue = u_tel.Text;
        Sqlregistered.UpdateParameters["手機"].DefaultValue = u_Mobile.Text;
        Sqlregistered.UpdateParameters["帳號"].DefaultValue = u_id.Text;
        Sqlregistered.UpdateParameters["縣市代碼"].DefaultValue = u_addcity.SelectedValue;
        Sqlregistered.UpdateParameters["郵遞代碼"].DefaultValue = u_addarea.SelectedValue;
        Sqlregistered.UpdateParameters["村里"].DefaultValue = u_address.Text;
        Sqlregistered.UpdateParameters["地址其他"].DefaultValue = u_add.Text;


        Sqlregistered.Update();

        DataList1.EditItemIndex = -1;
        DataList1.DataBind();
    }
    protected void ddl4_SelectedIndexChanged(object sender, EventArgs e)
    {

        //DropDownList ddl4 =  (DropDownList)DataList1.FindControl("ddl4");
       DropDownList ddl4 =  sender as DropDownList;

        TextBox tb = (TextBox)ddl4.FindControl("txtareano");
        tb.Text = ddl4.SelectedValue;
    }

    protected void ddl3_DataBound(object sender, EventArgs e)
    {
        DropDownList ddl3 = sender as DropDownList; 
        DropDownList ddl4 = (DropDownList)ddl3.FindControl("ddl4");
        Label labareano = (Label)ddl3.FindControl("lab4");
        ddl4.SelectedValue = labareano.Text;

    }
    protected void ddl3_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl3 = sender as DropDownList;
        DropDownList ddl4 = (DropDownList)ddl3.FindControl("ddl4");
        //DropDownList ddl4 = (DropDownList)DataList1.FindControl("ddl4");
        ddl4.Items.Clear();
        ddl4.Items.Insert(0, new ListItem("請選擇", "0"));
    }
    protected void btn_psd_Click(object sender, EventArgs e)
    {
        Response.Redirect("custModifyPass.aspx");
    }
}