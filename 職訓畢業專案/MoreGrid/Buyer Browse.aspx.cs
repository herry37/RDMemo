using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Buyer_Browse : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {//確認登入
        if (Session["u_id"] == null || Session["u_id"] == "")
        {
            Response.Redirect("memberLoginValidate.aspx");
        }
        else
        {
           

            SqlDataSource1.SelectCommand = "select 訂單.帳號 as 買家帳號 , 商品.商品小圖片 , 訂單明細.商品編號 , 商品.商品名稱 , 訂單明細.購買價格,訂單明細.訂購數量,收貨地址.收貨人 ,商品.帳號 as 賣家帳號 ,訂單.出貨日期 ,訂單明細.買家留言 from 訂單 inner join  收貨地址 on 訂單.訂單編號=收貨地址.訂單編號 inner join 訂單明細 on 收貨地址.訂單編號=訂單明細.訂單編號 inner  join 商品 on  訂單明細.商品編號=商品.商品編號 where 訂單.帳號='" + Session["u_id"] + "'order by 訂單.下單日期 desc";
            SqlDataSource1.Dispose();
            //  Label Shipment = (Label)GridView1.FindControl("lbNum");
        }
    }
    protected string shipment_Check(string shipment)
    {
        if (Eval(shipment).ToString() == null || Eval(shipment).ToString() == "")
        
            return "尚未出貨";
        else
            return "";

    }

    protected void dlBrowse_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (dlBrowse.SelectedValue == "0")
        {
            DataBind();                                 
        }
        if (dlBrowse.SelectedValue == "1")
        {
            SqlDataSource1.SelectCommand = "select 訂單.帳號 as 買家帳號 , 商品.商品小圖片 , 訂單明細.商品編號 , 商品.商品名稱 , 訂單明細.購買價格,訂單明細.訂購數量,收貨地址.收貨人 ,商品.帳號 as 賣家帳號 ,訂單.出貨日期 ,訂單明細.買家留言 from 訂單 inner join  收貨地址 on 訂單.訂單編號=收貨地址.訂單編號 inner join 訂單明細 on 收貨地址.訂單編號=訂單明細.訂單編號 inner  join 商品 on  訂單明細.商品編號=商品.商品編號 where 訂單.帳號='" + Session["u_id"] + "'and 訂單.出貨日期  IS NULL order by 訂單.下單日期 desc";                    
        }
        if (dlBrowse.SelectedValue == "2")
        {
            SqlDataSource1.SelectCommand = "select 訂單.帳號 as 買家帳號 , 商品.商品小圖片 , 訂單明細.商品編號 , 商品.商品名稱 , 訂單明細.購買價格,訂單明細.訂購數量,收貨地址.收貨人 ,商品.帳號 as 賣家帳號 ,訂單.出貨日期 ,訂單明細.買家留言 from 訂單 inner join  收貨地址 on 訂單.訂單編號=收貨地址.訂單編號 inner join 訂單明細 on 收貨地址.訂單編號=訂單明細.訂單編號 inner  join 商品 on  訂單明細.商品編號=商品.商品編號 where 訂單.帳號='" + Session["u_id"] + "'and 訂單.出貨日期  IS not NULL order by 訂單.下單日期 desc";                      
        }
        SqlDataSource1.Dispose();
    }
    protected void dlPager_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow pagerrow = GridView1.BottomPagerRow;
        DropDownList ddrpage = (DropDownList)pagerrow.Cells[0].FindControl("dlPager");
        GridView1.PageIndex = ddrpage.SelectedIndex;

        if (sender is LinkButton) 
        {
            switch (((LinkButton)sender).ID)
            {
                case "lbPrev":
                    if (GridView1.PageIndex >= 1)                     
                     GridView1.PageIndex--; 
                    break;
                case "lbNext":
                    if (GridView1.PageIndex + 1 < GridView1.PageCount)
                        GridView1.PageIndex++;
                    break;
            }

        }
        
    }
    protected void GridView1_DataBound(object sender, EventArgs e)
    {
       
         GridViewRow pagerrow = GridView1.BottomPagerRow;
         if (pagerrow != null)
         {
             Label PagerInfor = (Label)GridView1.BottomPagerRow.Cells[0].FindControl("lblInfor");
             PagerInfor.Text = "Page" + (GridView1.PageIndex + 1) + "of" + GridView1.PageCount;
             DropDownList ddrPage = (DropDownList)pagerrow.Cells[0].FindControl("dlPager");
             for (int i = 0; i < GridView1.PageCount; i++)
             {
                 ListItem item = new ListItem((i + 1).ToString());
                 if (i == GridView1.PageIndex)
                     item.Selected = true;
                 ddrPage.Items.Add(item);
             }
         }
         else
         {
             lbbuy.Visible = true;
         }
       
    }
}