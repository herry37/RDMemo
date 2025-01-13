using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request["tag"] != null)
        {
            SqlDataSource1.SelectCommand = "select Top 12 * from 商品 where 商品類別代碼=" + Request["tag"].ToString() + " and 停權=0 order by 商品編號 desc";
        }
        else if (Request["subsite"] != null)
        {
            SqlDataSource1.SelectCommand = "select * from 商品 where 帳號='" + Request["subsite"] + "' and [下架時間] > getdate() and 停權=0  order by 商品編號 desc";            
        }

    }

}