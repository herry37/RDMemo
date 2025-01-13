using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class ListShipments : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ship")
        {
            int rowsCount = Convert.ToInt32(e.CommandArgument);
            int goods_id = Convert.ToInt32(GridView1.Rows[rowsCount].Cells[0].Text);//商品編號     

            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
            SqlCommand Cmd = new SqlCommand("update 訂單 set 出貨日期=@出貨日期 where 訂單編號=@訂單編號", Conn);
            Conn.Open();

            Cmd.Parameters.Add("@出貨日期", SqlDbType.Date).Value = DateTime.Now.ToString("yyyy-MM-dd");
            Cmd.Parameters.Add("@訂單編號", SqlDbType.Int).Value = goods_id;

            Cmd.ExecuteNonQuery();
            Conn.Close();

            Response.Redirect(Request.FilePath);
        }        
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //GridViewRow pagerrow = GridView1.BottomPagerRow;            

        //Label PageInfor = (Label)pagerrow.Cells[0].FindControl("lblInfor");
        //PageInfor.Text = "Page " + (GridView1.PageIndex + 1) + "of " + GridView1.PageCount;

        //DropDownList ddrPage = (DropDownList)pagerrow.Cells[0].FindControl("DropDownList1");
        //for (int i = 0; i < GridView1.PageCount; i++)
        //{
        //    ListItem item = new ListItem((i + 1).ToString());
        //    ddrPage.Items.Add(item);
        //}
        //ddrPage.SelectedIndex = GridView1.PageIndex;

        if (e.Row.RowIndex != -1)
        {
            if (e.Row.Cells[3].Text == "&nbsp;")
            {
                e.Row.Cells[3].Text = "尚未出貨";
                ((Button)e.Row.Cells[5].Controls[0]).Attributes.Add("onclick", "javascript:if(!confirm('您確定編號【" + e.Row.Cells[0].Text + "】的訂單已出貨嗎？')) return;");
            }

            else
                ((Button)e.Row.Cells[5].Controls[0]).Visible = false;

        }
    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow pagerrow = GridView1.BottomPagerRow;
        DropDownList ddrPage = (DropDownList)pagerrow.Cells[0].FindControl("DropDownList1");
        GridView1.PageIndex = ddrPage.SelectedIndex;

        if (sender is LinkButton)
        {
            if (((LinkButton)sender).ID == "LinkButton1" && GridView1.PageIndex > 0)
                GridView1.PageIndex--;

            if (((LinkButton)sender).ID == "LinkButton2" && GridView1.PageIndex < GridView1.PageCount - 1)
                GridView1.PageIndex++;
        }
    }
}