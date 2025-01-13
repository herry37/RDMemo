using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class GoodsEdit : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Login"] != null)
        {
            string uid = Session["u_id"].ToString();
            //TODO SqlDataSourceGoodsEdit.SelectCommand = "SELECT * FROM [商品] inner join [商品類別] on [商品].商品類別代碼=[商品類別].商品類別代碼 where [帳號]='" + uid + "' and [下架時間]> getdate() and 停權=0 order by [商品編號] DESC";
            SqlDataSourceGoodsEdit.SelectCommand = "SELECT * FROM [商品] inner join [商品類別] on [商品].商品類別代碼=[商品類別].商品類別代碼 where [帳號]='" + uid + "' order by [商品編號] DESC";
            //TODO SqlDataSourceGoodsEditdown.SelectCommand = "SELECT * FROM [商品] inner join [商品類別] on [商品].商品類別代碼=[商品類別].商品類別代碼 where [帳號]='" + uid + "' and [下架時間]< getdate() and 停權=0 order by [商品編號] DESC";
            SqlDataSourceGoodsEditdown.SelectCommand = "SELECT * FROM [商品] inner join [商品類別] on [商品].商品類別代碼=[商品類別].商品類別代碼 where [帳號]='" + uid + "' and 停權=0 order by [商品編號] DESC";

        }
        else
        {
            Response.Write("<script>alert('請先登入會員！'); location.href='memberLoginValidate.aspx';</script>");
            // Response.Redirect("memberLoginValidate.aspx");
        }
    }

    protected void GVShow_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //if (e.CommandName == "showInfo")
        //{
        //    int index = Convert.ToInt16(e.CommandArgument);
        //    string GsNo = GridView1.Rows[index].Cells[0].Text;       //讀取grid第幾筆的title資料值

        //    //秀在gridview上的資料來源
        //    SqlDataSourceGoodsEdit.SelectCommand = "SELECT * FROM [StudentScores] where 商品編號='" + GsNo + "'";
        //}

    }

    protected void btnshow_Click(object sender, EventArgs e)
    {
        GoodsEditshow.Visible = true;
        btnshow.CssClass = "btn btn-primary btn-lg btn-block";
        GoodsEditdown.Visible = false;
        btndown.CssClass = "btn btn-default btn-lg btn-block";
        GVShow.PageIndex = 0;
    }
    protected void btndown_Click(object sender, EventArgs e)
    {
        GoodsEditdown.Visible = true;
        btndown.CssClass = "btn btn-primary btn-lg btn-block";
        GoodsEditshow.Visible = false;
        btnshow.CssClass = "btn btn-default btn-lg btn-block";
        GVDown.PageIndex = 0;
    }
}