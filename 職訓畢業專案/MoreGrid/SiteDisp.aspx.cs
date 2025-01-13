using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class SiteDisp : System.Web.UI.Page
{
    //顯示導覽列
    void ShowNav(string subsite_id)
    {
        hlToTop.NavigateUrl = "SiteDisp.aspx?ss=" + subsite_id;
    }

    //顯示商品標籤
    void ShowGoodsTag(string subsite_id)
    {
        sqlGoodsTags.SelectParameters.Add("subsite_id", subsite_id);
        sqlGoodsTags.SelectCommand = "select * from goodstag where subsite_id=@subsite_id";
    }  

    //顯示商品
    void ShowGoods()
    {
        if (Request["ss"] != null)
        {
            sqlGoods.SelectParameters.Add("subsite_id",Request["ss"].ToString());
            sqlGoods.SelectCommand = "select * from goods where subsite_id=@subsite_id";
        }
        else
        {
            sqlGoods.SelectParameters.Add("tag", Request["ta"].ToString());
            sqlGoods.SelectCommand = "select * from goods where tag=@tag";
        }

    }
      
    //輸入商品類別編號，輸出網站ID
    string Findsubsite_id(string tag)
    {
        
        string subsite_id;
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        Conn.Open();
        SqlCommand cmd = new SqlCommand("select subsite_id from goodstag where id=@tag", Conn);
        cmd.Parameters.AddWithValue("tag",tag);
        SqlDataReader dr = cmd.ExecuteReader();
        dr.Read();
        subsite_id = dr[0].ToString();
        Conn.Close();

        return subsite_id;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
     


        //先判斷網址參數是 ss or ta，如果都沒有則丟到Main.aspx
        if (Request["ss"] != null || Request["ta"] != null)
        {
            string subsite_id;
            if (Request["ta"] != null)
            {
                subsite_id = Findsubsite_id(Request["ta"]);
            }
            else
            {
                subsite_id = Request["ss"].ToString();
            }
            ShowNav(subsite_id);
            ShowGoodsTag(subsite_id);
            ShowGoods();
            
        }
        else
        {
            Response.Redirect("Main.aspx");
        }
    

    }

}