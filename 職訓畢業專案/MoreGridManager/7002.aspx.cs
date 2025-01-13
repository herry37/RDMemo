
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web.Configuration;

//---------------------------------------------------------------------------- 
//程式功能	線上客服-客服人員端 (監控客戶要求)
//---------------------------------------------------------------------------- 

public partial class Service_7002 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["account"] == null)
                Response.Redirect("Login.aspx");
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
            SqlDataReader objReader = null;
            SqlCommand checkCmd = new SqlCommand("select mg_sid from Manager where mg_id=@id", Conn);
            Conn.Open();
            checkCmd.Parameters.AddWithValue("@id", Session["account"].ToString());
            objReader = checkCmd.ExecuteReader();
            if (objReader.HasRows)
            {
                objReader.Read();

                // 檢查客服人員編號並存入登入紀錄         
                Session["mg_sid"] = objReader["mg_sid"].ToString();
                checkCmd.Cancel();
                objReader.Close();
                Conn.Close();
                Conn.Dispose();
            }
            else
            {
                Response.Write("<script>alert('無此客服人員');</script>");
                checkCmd.Cancel();
                objReader.Close();
                Conn.Close();
                Response.Write("<script>location.href='Service.aspx'; </script>");
             
                return;
            }



            #region 定義訊息 Table 的 Title
            lb_tb_title.Text = "<table width=\"100%\" border=\"1\" cellpadding=\"4\" cellspacing=\"0\" style=\"background-color:#F0F8FF\">";
            lb_tb_title.Text += "<tr style=\"background-color:#FFFFE0\" align=\"center\">";
            lb_tb_title.Text += "<td width=\"10%\">狀態</td>";
            lb_tb_title.Text += "<td width=\"20%\">要求提出時間</td>";
            lb_tb_title.Text += "<td width=\"15%\">客戶名稱</td>";
            lb_tb_title.Text += "<td width=\"20%\">客戶回應時間</td>";
            lb_tb_title.Text += "<td width=\"20%\">客服回應時間</td>";
            lb_tb_title.Text += "<td width=\"15%\">客服人員</td>";
            lb_tb_title.Text += "</tr>";
            #endregion

            // 清除及設定過期的資料
            Clean_Data();

            #region 定義 SQL 語法，取得目前有效的客服要求
            lb_Sql_Cs_User.Text = "Select u.cu_sid, u.cu_rtn, u.cu_time, u.cu_name, u.cu_utime, u.cu_stime, IsNull(m.mg_name, '尚未回應') as mg_name";
            lb_Sql_Cs_User.Text += " From Cs_User u Left Outer Join Manager m On u.mg_sid = m.mg_sid";
            lb_Sql_Cs_User.Text += " Where cu_rtn < 2 Order by u.cu_sid";
            #endregion

            // 取得客戶服務要求狀態
            lt_list.Text = Get_Cs_User();
        }
    }



    // 檢查是否有客戶提出要求
    protected void ti_Cs_User_Tick(object sender, EventArgs e)
    {
        lt_list.Text = Get_Cs_User();
    }

    // 傳回客戶服務要求狀態，並重整客服要求 (將過期未回應的要求，全部清成結束)
    private string Get_Cs_User()
    {
        string cu_rtn = "", cu_time = "";
        StringBuilder mData = new StringBuilder();

        using (SqlConnection Sql_Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["APPSYSConnectionString"].ConnectionString))
        {
            using (SqlCommand Sql_Command = new SqlCommand())
            {
                Sql_Conn.Open();

                #region 同時擷取資料
                Sql_Command.Connection = Sql_Conn;
                Sql_Command.CommandText = lb_Sql_Cs_User.Text;

                using (SqlDataReader Sql_Reader = Sql_Command.ExecuteReader())
                {
                    if (Sql_Reader.Read())
                    {
                        mData.Append(lb_tb_title.Text);
                        do
                        {
                            switch (Sql_Reader["cu_rtn"].ToString())
                            {
                                case "0":
                                    cu_rtn = "未回應";
                                    cu_time = "<a href=\"70021.aspx?sid=" + Sql_Reader["cu_sid"].ToString() + "\" title=\"點我進入對話視窗\">"
                                        + DateTime.Parse(Sql_Reader["cu_time"].ToString()).ToString("yyyy/MM/dd HH:mm:ss") + "</a>";
                                    break;
                                case "1":
                                    cu_rtn = "回應中";
                                    cu_time = DateTime.Parse(Sql_Reader["cu_time"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                    break;
                                default:
                                    cu_rtn = "已結束";
                                    cu_time = DateTime.Parse(Sql_Reader["cu_time"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                                    break;
                            }
                            mData.Append("<tr align=\"center\"><td>" + cu_rtn + "</td>");
                            mData.Append("<td>" + cu_time + "</td>");
                            mData.Append("<td>" + Sql_Reader["cu_name"].ToString().Trim() + "</td>");
                            mData.Append("<td>" + DateTime.Parse(Sql_Reader["cu_utime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss") + "</td>");
                            mData.Append("<td>" + DateTime.Parse(Sql_Reader["cu_stime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss") + "</td>");
                            mData.Append("<td>" + Sql_Reader["mg_name"].ToString().Trim() + "</td></tr>");

                        } while (Sql_Reader.Read());
                        mData.Append("</table>");
                        mData.Append("<p align=\"center\" style=\"margin: 5pt 0pt 5pt 0pt\">" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "</p>");
                    }
                    else
                        mData.Append("<p align=\"center\" class=\"text11pt\" style=\"margin: 10pt 0pt 10pt 0pt\">無任何客服要求<br>" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "</p>");

                    Sql_Reader.Close();
                    Sql_Reader.Dispose();
                }
                #endregion

                Sql_Conn.Close();
            }
        }

        return mData.ToString();
    }

    // 清除及設定過期的資料
    private void Clean_Data()
    {
        string SqlString = "";

        using (SqlConnection Sql_Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["APPSYSConnectionString"].ConnectionString))
        {
            using (SqlCommand Sql_Command = new SqlCommand())
            {
                Sql_Conn.Open();

                // 刪除 7 天以前的交談紀錄
                SqlString += "Delete Cs_Message Where cu_sid In (Select cu_sid From Cs_User Where DateDiff(d, cu_time, getdate()) > 7);";

                // 刪除 7 天以前的服務要求
                SqlString += "Delete Cs_User Where DateDiff(d, cu_time, getdate()) > 7;";

                // 將過期未回應的要求，全部設成結束 (超過30分鐘無回應者) 
                SqlString += "Update Cs_User Set cu_rtn = 2 Where cu_rtn < 2 And DateDiff(mi, cu_utime, getdate()) > 30 And DateDiff(mi, cu_stime, getdate()) > 30;";

                Sql_Command.Connection = Sql_Conn;
                Sql_Command.CommandText = SqlString;

                Sql_Command.ExecuteNonQuery();

                Sql_Command.Dispose();

                Sql_Conn.Close();
            }
        }
    }
}
