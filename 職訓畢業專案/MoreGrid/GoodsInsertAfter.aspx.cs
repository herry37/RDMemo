using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GoodsInsertAfter : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["message"] != null)
        {
            InsertAfter.Text = Session["message"].ToString();
        }
    }
    protected void goodsInsert_Click(object sender, EventArgs e)
    {
        Response.Redirect("GoodsInsert.aspx");
    }
    protected void MoreGird_Click(object sender, EventArgs e)
    {
        Response.Redirect("Main.aspx");
    }
}