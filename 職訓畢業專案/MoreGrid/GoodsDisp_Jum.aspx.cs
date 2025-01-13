using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


public partial class GoodsDisp : System.Web.UI.Page
{
    //從商品id去找子網站id
    void FindSubsite_id(int id)
     {
      
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        Conn.Open();
     
        SqlCommand cmd = new SqlCommand("select * from [商品] where 商品編號=" + id + "", Conn);
        //此處待修正
        SqlDataReader dr = cmd.ExecuteReader();
        dr.Read();
        hlShop.Text = "店家" + dr[1].ToString() + "的商品";
        hlShop.NavigateUrl = "Main.aspx?subsite=" + dr[1].ToString() + "";
        Conn.Close();

     }
    
    protected void Page_Load(object sender, EventArgs e)
    {
       
        
         if (Request["id"]!=null)
             FindSubsite_id(Convert.ToInt32(Request["id"]));



         

           // 不用再判斷使用者是否登入由於"購買鍵寫在前端所以驗在出貨區"
         //if (Session["u_id"] == null || Session["u_id"] == "")
         //{

         //Response.Redirect("memberLoginValidate.aspx");
         //}

          //判斷是使用者或店家
        //if (Session["u_id"] == null || Session["u_id"] == "")
        //{
          // LinkButton reply = (LinkButton)RepeaterMessage.FindControl("LBttnReply");
          // reply.Visible = true;

        //}
    }
    //Master時ID包在content中所以要用下面的抓法
    //void ClearTextBoxes()
    //{

    //    ContentPlaceHolder mpContentPlaceHolder;
    //    //使用ContentPlaceHolde物件抓ContentPlaceHolde
    //    mpContentPlaceHolder =
    //      (ContentPlaceHolder)Master.FindControl("SiteDisp");
    //    int count = 1;
    //    foreach (object ctrl in mpContentPlaceHolder.Controls)
    //    {
    //        //再在ContentPlaceHolde中抓TextBox
    //        if (ctrl is System.Web.UI.WebControls.TextBox)
    //        {
    //            count++;
    //            TextBox TextBox1 = (TextBox)ctrl;
    //            TextBox1.Text = "56";
    //            //count.ToString();
    //        }

    //    }
    //}
    
    protected void ButMessage_Click(object sender, EventArgs e)
    {
        //商品編號
        int goods_id = Convert.ToInt32(Request.QueryString["id"]);

        if (txtGcontent.Text != null && txtGcontent.Text != "") {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);

        //使用者帳號
        //int User_id;
        //User_id = Convert.ToInt16(Session["u_id"]);
        string User_id = Session["u_id"].ToString();

        SqlCommand objCmd = new SqlCommand("insert into 留言版(帳號,商品編號,內容,留言時間) values('" + User_id +"', "+ goods_id + ",'" + txtGcontent.Text + "',getdate())", Conn);
        
        Conn.Open();
        objCmd.ExecuteNonQuery();
        Conn.Close();
        RepeaterMessage.DataBind();
        txtGcontent.Text = "";
    }else txtGcontent.Text="";

    }

    protected void ButMessageDel_Click(object sender, EventArgs e)
    {
        txtGcontent.Text = "";
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (Session["u_id"] != null | Session["Login"] != null)
        {

            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
            //新增資料庫物件 (資料表,資料庫)
            SqlCommand objCmd = new SqlCommand("select * from 會員 where 帳號=@id", Conn);
            objCmd.Parameters.AddWithValue("@id", Session["u_id"]);
            //宣告Reader物件
            SqlDataReader objReader;

            Conn.Open();//連接資料庫
                        //呼叫Reader物件
            objReader = objCmd.ExecuteReader();
             if (objReader.Read())
             {
             Response.Write("<script>alert('已有相同帳號');</script>");
             }
            else
             {
              Response.Write("<script>alert('此帳號可以使用');</script>");
             }
             Conn.Close();
            Response.Redirect("ShoppingCart.aspx");
        }
    }

    //protected void btBuy_Click(object sender, EventArgs e)
    //{

    //    string id = Request.QueryString["id"];      //抓網址上的ID後用的變數
    //    Response.Redirect("GoodsShipper.aspx?id="+id);
    //}
    //public string GetString()
    //{
    //    string strTemp = Label3.Text;
    //    return strTemp;
    //}
    //如何用
    protected void btShoppingcar_Click(object sender, EventArgs e)
    {
        int goods_id = Convert.ToInt32(Request.QueryString["id"]);
        Response.Cookies["ShoppingCar"][goods_id.ToString()] = goods_id.ToString();
        Response.Cookies["ShoppingCar"].Expires = DateTime.Now.AddDays(1);
        Response.Write("<script>alert('已加入購物車');</script>");
    }
 
}