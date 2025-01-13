//---------------------------------------------------------------------------- 
//程式功能	線上客服-客服人員端 > 對話視窗
//---------------------------------------------------------------------------- 
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _7002_70021 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string mErr = "", SqlString = "";
        int cu_sid = -1;

        if (!IsPostBack)
        {
            mErr = "參數傳送錯誤!返回主頁面\\n";

            if (Request["sid"] != null)
            {
                if (int.TryParse(Request["sid"], out cu_sid))
                {
                    lb_cu_sid.Text = cu_sid.ToString();
                    lb_mg_sid.Text = Session["mg_sid"].ToString();

                    #region 填入管理者編號及更改回應狀態，並取得相關資料
                    using (SqlConnection Sql_Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["APPSYSConnectionString"].ConnectionString))
                    {
                        using (SqlCommand Sql_Command = new SqlCommand())
                        {
                            Sql_Conn.Open();

                            SqlString = "Execute dbo.p_Cs_Join @cu_sid, @mg_sid;";
                            SqlString += "Select Top 1 u.cu_name, u.cu_time, m.mg_name From Cs_User u";
                            SqlString += " Inner Join Manager m On u.mg_sid = m.mg_sid";
                            SqlString += " Where u.cu_sid = @cu_sid";

                            Sql_Command.Connection = Sql_Conn;
                            Sql_Command.CommandText = SqlString;
                            Sql_Command.Parameters.AddWithValue("cu_sid", cu_sid);
                            Sql_Command.Parameters.AddWithValue("mg_sid", lb_mg_sid.Text);
           
                           

                            using (SqlDataReader Sql_Reader = Sql_Command.ExecuteReader())
                            {
                                if (Sql_Reader.Read())
                                {
                                    lb_mg_name.Text = Sql_Reader["mg_name"].ToString().Trim();
                                    lb_cu_time.Text = DateTime.Parse(Sql_Reader["cu_time"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                    lb_cu_name.Text = Sql_Reader["cu_name"].ToString().Trim();
                                    mErr = "";
                                }
                                else
                                    mErr = "找不到客服要求紀錄!\\n";
                                Sql_Reader.Close();
                            }
                        }
                    }
                    #endregion
                }
            }

            if (mErr != "")
                ClientScript.RegisterStartupScript(this.GetType(), "ClientScript", "alert(\"" + mErr + "\");location.replace(\"7002.aspx\");", true);
        }
    }

   

    // 結束離開
    protected void bn_end_Click(object sender, EventArgs e)
    {
        string SqlString = "";

        #region 更新回應狀態旗標
        using (SqlConnection Sql_Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["APPSYSConnectionString"].ConnectionString))
        {
            using (SqlCommand Sql_Command = new SqlCommand())
            {
                Sql_Conn.Open();
                SqlString = "Update Cs_User Set cu_rtn = 2 Where cu_sid = @cu_sid And mg_sid = @mg_sid;";
                SqlString += "Insert Into Cs_Message (cu_sid, cm_object, cm_desc, cm_end) Values";
                SqlString += "(@cu_sid, 2, '結束對話', 1)";

                Sql_Command.Connection = Sql_Conn;
                Sql_Command.CommandText = SqlString;
                Sql_Command.Parameters.AddWithValue("cu_sid", lb_cu_sid.Text);
                Sql_Command.Parameters.AddWithValue("mg_sid", lb_mg_sid.Text);

                Sql_Command.ExecuteNonQuery();
                Sql_Command.Dispose();
            }
        }
        #endregion

        // 返回上一頁
        Response.Redirect("7002.aspx");
    }
}
