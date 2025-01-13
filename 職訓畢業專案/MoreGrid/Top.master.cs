using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Convert.ToString(Session["Login"]) == "OK")
        {
            user_UID.Visible = true;
            user_UID.Text = "Hi " + Session["u_uid"].ToString();
            lbtnLogin.Text = "登出";
        }
        else
        {
            lbtnLogin.Text = "登入";
        }
    }
    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        if (Convert.ToString(Session["Login"]) == "OK")
        {
            Session["Login"] = null;
            Session["u_Validate"] = null;
            Response.Redirect("Main.aspx");
        }
        else
        {
            Response.Redirect("memberLoginValidate.aspx");
        }

    }
    protected void but_search_Click(object sender, EventArgs e)
    {
        //TODO Session["cmd"] = "SELECT * FROM [商品] where 1=1 AND [下架時間]>getdate() ";
        Session["cmd"] = "SELECT * FROM [商品] where 1=1 ";
        string address = "search.aspx?";
        //有商品類別
        if (DropDownListCategory.SelectedIndex > 0)
        {
            address += "&Category=" + DropDownListCategory.SelectedValue;


        }

        //有商品名稱
        if (txt_name.Text.Length > 0)
        {
            address += "&name=" + txt_name.Text;
        }
        //有商品價格底限
        if (txt_price.Text.Length > 0)
        {
            address += "&min=" + txt_price.Text;
        }
        //有商品價格上限
        if (txt_price1.Text.Length > 0)
        {

            address += "&max=" + txt_price1.Text;
        }

        /*********************************************************************************************************/

        Response.Redirect(address);

    }
}
