using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//加入資料庫
using System.Data;
using System.Data.SqlClient;
//加入config設定
using System.Configuration;

public partial class custModifyPass : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Login"] != null)
        {
            string uid = Session["u_id"].ToString();
            Sqlregistered.SelectCommand = "SELECT * FROM [會員] where 帳號='" + uid + "'";
        }
        else
        {
            Response.Write("<script>alert('請先登入會員！'); location.href='memberLoginValidate.aspx';</script>");
            // Response.Redirect("memberLoginValidate.aspx");
        }

    }
    protected void DataList1_EditCommand(object source, DataListCommandEventArgs e)
    {
        DataList1.EditItemIndex = e.Item.ItemIndex;
        DataList1.DataBind();
    }
    protected void DataList1_CancelCommand(object source, DataListCommandEventArgs e)
    {
        DataList1.EditItemIndex = -1;
        DataList1.DataBind();
    }
    protected void DataList1_UpdateCommand(object source, DataListCommandEventArgs e)
    {
        TextBox u_psd = (TextBox)e.Item.FindControl("txtPsd");
        TextBox u_oldpsd = (TextBox)e.Item.FindControl("txtOldPsd");
        Label u_id = (Label)e.Item.FindControl("txtNo");
        Label u_psdok = (Label)e.Item.FindControl("LabeltxtOldPsdok");
        Label u_psdgg = (Label)e.Item.FindControl("LabeltxtOldPsdgg");

        

        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlDataReader objReader = null;
        SqlCommand checkCmd = new SqlCommand("select 密碼 from 會員 where 帳號=@u_id", Conn);
        checkCmd.Parameters.AddWithValue("@u_id", Session["u_id"]);

        Conn.Open();
        objReader = checkCmd.ExecuteReader();
        objReader.Read();
        if (u_oldpsd.Text == objReader["密碼"].ToString())
        {
            u_psdok.Visible = true;
            Sqlregistered.UpdateParameters["密碼"].DefaultValue = u_psd.Text;
            Sqlregistered.UpdateParameters["帳號"].DefaultValue = u_id.Text;

            Sqlregistered.Update();

           
            checkCmd.Cancel();
            objReader.Close();
            Conn.Close();
            Conn.Dispose();
            Response.Write("<script>alert('修改成功！');location.href='custModifyData.aspx';</script>");
        }
        else
        {
            u_psdgg.Visible = true;
            Response.Write("<script>alert('舊密碼有誤');</script>");
            checkCmd.Cancel();
            objReader.Close();
            Conn.Close();
            return;
        }

       

        DataList1.EditItemIndex = -1;
        DataList1.DataBind();
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("custModifyData.aspx");
    }
}