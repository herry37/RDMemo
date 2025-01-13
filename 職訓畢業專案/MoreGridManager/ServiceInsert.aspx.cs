using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ServiceInsert : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnInsert_Click(object sender, EventArgs e)
    {
        Sqlgv_Manager.Insert();
        Response.Write("<script> alert('資料新增成功');</script>");
        txt_mg_name.Text = "";
        txt_mg_id.Text = "";
    }
}