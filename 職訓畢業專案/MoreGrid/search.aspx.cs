using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class search : System.Web.UI.Page
{


    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (Session["cmd"] != null)
        {
            string cmd = Session["cmd"].ToString();
            if (Request.QueryString["Category"] != null)
            {
                cmd += "and 商品類別代碼=" + Convert.ToInt32(Request.QueryString["Category"].ToString());
            }
            //有商品名稱
            if (Request.QueryString["name"] != null)
            {
                cmd += "and [商品名稱] LIKE '%" + Request.QueryString["name"].ToString() + "%'";
            }
            //有商品價格底限
            if (Request.QueryString["min"] != null)
            {
                cmd += "and 單價 >=" + Convert.ToInt32(Request.QueryString["min"].ToString());
            }
            //有商品價格上限
            if (Request.QueryString["max"] != null)
            {
                cmd += "and 單價 <=" + Convert.ToInt32(Request.QueryString["max"].ToString());
            }
            SqlGoodsSearch2.SelectCommand = cmd;

            gvSearch.DataSourceID = "SqlGoodsSearch2";

        }
    }




}