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
    string FindSubsite_id(int id)
     {
         string uid;
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        Conn.Open();
     
        SqlCommand cmd = new SqlCommand("select * from [商品] where 商品編號=" + id + "", Conn);
        //此處待修正
        SqlDataReader dr = cmd.ExecuteReader();
        dr.Read();
        hlShop.Text = "店家" + dr[1].ToString() + "的商品";
        hlShop.NavigateUrl = "Main.aspx?subsite=" + dr[1].ToString() + "";

        uid = dr[1].ToString();
        Conn.Close();

        return uid;

     }

   
    
    protected void Page_Load(object sender, EventArgs e)
    {


        if (Request.QueryString["id"] != null)
            FindSubsite_id(Convert.ToInt32(Request.QueryString["id"]));
        

        ((TextBox) txtGcontent).Attributes["maxlength"] = "250";

       
     

           // 不用再判斷使用者是否登入由於"購買鍵寫在前端所以驗在出貨區"
         //if (Session["u_id"] == null || Session["u_id"] == "")
         //{

         //Response.Redirect("memberLoginValidate.aspx");
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
        if (Session["u_id"] != null)
        {
            //商品編號
            int goods_id = Convert.ToInt32(Request.QueryString["id"]);

            if (txtGcontent.Text != null && txtGcontent.Text != "")
            {
                SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);

                //使用者帳號
                //int User_id;
                //User_id = Convert.ToInt16(Session["u_id"]);
                string User_id = Session["u_id"].ToString();

                SqlCommand objCmd = new SqlCommand("insert into 留言版(帳號,商品編號,內容,留言時間) values('" + User_id + "', " + goods_id + ",'" + txtGcontent.Text + "',getdate())", Conn);

                Conn.Open();
                objCmd.ExecuteNonQuery();
                Conn.Close();
                RepeaterMessage.DataBind();
                txtGcontent.Text = "";
            }
            else txtGcontent.Text = "";
        }
        else
        {
            Response.Write("<script>alert('請先登入會員！');location.href='memberLoginValidate.aspx';</script>");
           
        }
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
        Response.Cookies[goods_id.ToString()].Value = goods_id.ToString();
        Response.Cookies[goods_id.ToString()].Expires = DateTime.Now.AddDays(1);
        //Response.Cookies["ShoppingCar"][goods_id.ToString()] = goods_id.ToString();
        //Response.Cookies["ShoppingCar"].Expires = DateTime.Now.AddDays(1);
        Response.Write("<script>alert('已加入購物車');location.href='Main.aspx';</script>");  //先跳出alert後再換頁的寫法   
    }

    protected void DataList1_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        Label lbEdit = (Label)e.Item.FindControl("lbEdit");
        string goodeditrand = Server.HtmlDecode(lbEdit.Text);
        lbEdit.Text = goodeditrand;

    

   
    }
    protected void btShipper_Click(object sender, EventArgs e)
    {
        string id = Request.QueryString["id"];      //抓網址上的ID後用的變數
        Response.Redirect("GoodsShipper.aspx?id="+id);
    }

    //回覆留言
    protected void ButMessage_Click1(object sender, EventArgs e)
    {
        if (txtGcontent1.Text != null || txtGcontent1.Text != "")
        {
            int message_id = Convert.ToInt32(ViewState["留言編號"]);
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
            SqlCommand objCmd = new SqlCommand("INSERT INTO [回覆留言] ([留言編號],[回覆內容],[回覆時間]) VALUES ('" + message_id + "','" + txtGcontent1.Text + "',getdate()) ", Conn);
            Conn.Open();
            objCmd.ExecuteNonQuery();
            Conn.Close();
            txtGcontent1.Text = "";
            RepeaterMessage.DataBind();
            //Response.Redirect("GoodsDisp.aspx");
        }
        else errMessage.Text = "回覆留言內未填寫";

    }

    protected void ButMessageDel_Click1(object sender, EventArgs e)
    {
        txtGcontent.Text = "";
    }
    protected void RepeaterMessage_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Re")
        {
            PanelMessageR.Visible = true;
           //ViewState["留言編號"] = Convert.ToInt64(e.CommandArgument);
            ViewState["留言編號"] = Convert.ToInt32(e.CommandArgument);
           
            PanelMessage4.Visible = false;
        }
    }
    protected void RepeaterMessage_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //判斷是使用者或店家
        try
        {
            if (Session["u_id"].ToString() == FindSubsite_id(Convert.ToInt32(Request.QueryString["id"].ToString())))
            {
         
                //LinkButton reply = (LinkButton)RepeaterMessage.FindControl("LBttnReply");
                Button reply = (Button)e.Item.FindControl("ButR");
                reply.Visible = true;

            }
        }
        catch (Exception ex)
        {

            //Button reply = (Button)RepeaterMessage.FindControl("ButR");
            //reply.Visible = false;
        }   
    }
}