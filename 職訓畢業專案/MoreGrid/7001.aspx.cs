
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

//---------------------------------------------------------------------------- 
//程式功能	線上客服-客戶使用端 (提出客服要求)
//---------------------------------------------------------------------------- 


public partial class Service_7001 : System.Web.UI.Page
{
    private int p_wait_time = 600;		// 等待客服回應時間(秒)

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 客服編號並存入登入紀錄           
            Session["mg_sid"] = Session["mg_sid"];
        }
    }



    // 提出要求
    protected void bn_ok_Click(object sender, EventArgs e)
    {
        if (tb_cu_name.Text != "")
        {

            bn_ok.Visible = false;
            tab_wait.Visible = true;
            lb_cu_time.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            lb_cu_sid.Text = "0";

            // 產生客服要求紀錄
            create_cs_user();

            // 啟用 Timer 等候客服回應
            ti_check.Enabled = true;
        }
        else
            ClientScript.RegisterStartupScript(this.GetType(), "ClientScript", "alert(\"請先輸入使用者名稱!\");location.replace(\"7001.aspx\");", true);
    }

    // 輪詢查看客服是否有回應
    protected void ti_check_Tick(object sender, EventArgs e)
    {
        int cu_rtn = 0;

        #region 計算已經過的時間
        DateTime dt_now = DateTime.Now;
        DateTime dt_cu_time = DateTime.Parse(lb_cu_time.Text);
        TimeSpan ts_sec = dt_now - dt_cu_time;

        lb_wait_sec.Text = ts_sec.TotalSeconds.ToString("0");
        #endregion

        // 檢查客服是否有回應
        cu_rtn = check_response();

        if (cu_rtn == 0 && int.Parse(ts_sec.TotalSeconds.ToString("0")) > p_wait_time)
        {
            // 超過等待時間
            lt_msg.Text = "<br>等待時間超過" + p_wait_time + "秒，請重新提出要求...<br><p align=\"center\" style=\"margin:10pt 0pt 10pt 0pt\">";
            lt_msg.Text += "<a href=\"7001.aspx\" class=\"abtn\">&nbsp;重新提出要求&nbsp;</a></p>";

            // 超時無回應，填入結束旗標
            end_ask();

            ti_check.Enabled = false;
        }
        else if (cu_rtn == 1)
        {
            // 有回應時，導向對話視窗
            Response.Redirect("70011.aspx?sid=" + lb_cu_sid.Text + "&mg_sid=" + lb_mg_sid.Text);
        }
    }

    // 產生客服要求紀錄
    private void create_cs_user()
    {
        string SqlString = "";

        using (SqlConnection Sql_Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString))
        {
            using (SqlCommand Sql_Command = new SqlCommand())
            {
                Sql_Conn.Open();

                SqlString = "Insert Into Cs_User (cu_rtn, cu_name) Values (0, @cu_name);";
                SqlString += "Select @cu_sid = Scope_Identity()";

                Sql_Command.Connection = Sql_Conn;
                Sql_Command.CommandText = SqlString;
                Sql_Command.Parameters.AddWithValue("cu_name", tb_cu_name.Text.Trim());

                SqlParameter spt_cu_sid = Sql_Command.Parameters.Add("cu_sid", SqlDbType.Int);
                spt_cu_sid.Direction = ParameterDirection.Output;

                Sql_Command.ExecuteNonQuery();

                lb_cu_sid.Text = spt_cu_sid.Value.ToString();

                Sql_Command.Dispose();
            }
        }
    }

    // 檢查客服是否有回應，有回應時傳回客服編號
    private int check_response()
    {
        int cu_rtn = 0;
        string SqlString = "";

        using (SqlConnection Sql_Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString))
        {
            using (SqlCommand Sql_Command = new SqlCommand())
            {
                Sql_Conn.Open();
                SqlString = "Select Top 1 cu_rtn, mg_sid From Cs_User Where cu_sid = @cu_sid And cu_rtn > 0";

                Sql_Command.Connection = Sql_Conn;
                Sql_Command.CommandText = SqlString;
                Sql_Command.Parameters.AddWithValue("cu_sid", lb_cu_sid.Text);

                SqlDataReader Sql_Reader = Sql_Command.ExecuteReader();

                if (Sql_Reader.Read())
                {
                    lb_mg_sid.Text = Sql_Reader["mg_sid"].ToString();
                    cu_rtn = int.Parse(Sql_Reader["cu_rtn"].ToString());
                }

                Sql_Reader.Close();
                Sql_Reader.Dispose();
                Sql_Command.Dispose();

                Sql_Conn.Close();
            }
        }
        return cu_rtn;
    }

    // 超時無回應，填入結束旗標
    private void end_ask()
    {
        string SqlString = "";

        #region 更新回應狀態旗標
        using (SqlConnection Sql_Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString))
        {
            using (SqlCommand Sql_Command = new SqlCommand())
            {
                Sql_Conn.Open();
                SqlString = "Update Cs_User Set cu_rtn = 2 Where cu_sid = @cu_sid;";
                SqlString += "Insert Into Cs_Message (cu_sid, cm_object, cm_desc, cm_end) Values";
                SqlString += "(@cu_sid, 1, '客服超時未回應', 1)";

                Sql_Command.Connection = Sql_Conn;
                Sql_Command.CommandText = SqlString;
                Sql_Command.Parameters.AddWithValue("cu_sid", lb_cu_sid.Text);

                Sql_Command.ExecuteNonQuery();
                Sql_Command.Dispose();
            }
        }
        #endregion
    }
}