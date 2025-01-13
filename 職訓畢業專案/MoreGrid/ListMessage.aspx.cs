using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ListMessage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Login"] != null)
        {
            string uid = Session["u_id"].ToString();
            SqlDataSourceListMessageQ.SelectCommand = "SELECT [留言版].留言編號,[留言版].留言時間,[留言版].內容,[留言版].帳號 買家帳號,[留言版].留言編號,[商品].商品名稱,[商品].帳號 AS 賣家帳號,[商品].商品編號,[回覆留言].回覆內容,[回覆留言].回覆時間 FROM [留言版] left join [商品] on [留言版].商品編號=[商品].商品編號 left join [回覆留言] on [留言版].留言編號=[回覆留言].留言編號 where [留言版].帳號='" + uid + "' order by [留言版].留言時間 desc";

            SqlDataSourceListMessageR.SelectCommand = "SELECT [留言版].留言編號,[留言版].留言時間,[留言版].內容,[留言版].帳號 AS 買家帳號,[留言版].留言編號,[回覆留言].回覆內容,[回覆留言].回覆時間,[商品].商品名稱,[商品].帳號 AS 賣家帳號,[商品].商品編號 FROM [留言版] left join [商品] on [留言版].商品編號=[商品].商品編號 left join [回覆留言] on [留言版].留言編號=[回覆留言].留言編號 WHERE [商品].帳號='" + uid + "' order by [留言版].留言時間 desc";
        }
        else
        {
            Response.Write("<script>alert('請先登入會員！'); location.href='memberLoginValidate.aspx';</script>");
            // Response.Redirect("memberLoginValidate.aspx");
        }
    }
    protected string shipment_Check(string shipment)
    {
        if (Eval(shipment).ToString() == null || Eval(shipment).ToString() == "")

            return "尚未回覆";
        else
            return "";

    }

    protected void btnR_Click(object sender, EventArgs e)
    {
        ListMessageR.Visible = true;
        btnQ.CssClass = "btn btn-default btn-lg btn-block";
        ListMessageQ.Visible = false;
        btnR.CssClass = "btn btn-primary btn-lg btn-block";
        GVLMR.PageIndex = 0;
    }
    protected void btnQ_Click(object sender, EventArgs e)
    {
        ListMessageQ.Visible = true;
        btnQ.CssClass = "btn btn-primary btn-lg btn-block";
        ListMessageR.Visible = false;
        btnR.CssClass = "btn btn-default btn-lg btn-block";
        GVLMQ.PageIndex = 0;
    }
}