using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
public partial class GoodsDispR : System.Web.UI.Page
{
    //留言版編號
    int message_id;
    protected void Page_Load(object sender, EventArgs e)
    {
        ((TextBox)txtGcontent).Attributes["maxlength"] = "250";

        message_id = Convert.ToInt32(Request.QueryString["id"]);
    }
    protected void ButMessage_Click(object sender, EventArgs e)
    {
        if (txtGcontent.Text != null || txtGcontent.Text != "")
        {
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
            SqlCommand objCmd = new SqlCommand("INSERT INTO [回覆留言] ([留言編號],[回覆內容],[回覆時間]) VALUES ('" + message_id + "','" + txtGcontent.Text + "',getdate()) ", Conn);
            Conn.Open();
            objCmd.ExecuteNonQuery();
            Conn.Close();
            txtGcontent.Text = "";
            Response.Redirect("GoodsDisp.aspx");
        }
        else errMessage.Text = "回覆留言內未填寫";

    }

    protected void ButMessageDel_Click(object sender, EventArgs e)
    {
        txtGcontent.Text = "";
    }

}

