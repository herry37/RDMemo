using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//加入資料庫設定
using System.Data;
using System.Data.SqlClient;
//加入config設定
using System.Configuration;

public partial class joinMoreGrid : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string URL = Request.Url.Query;
        string validate = URL.Substring(10, 36);

        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand checkCmd = new SqlCommand("select * from 會員 where Validate=@Validate", Conn);
        checkCmd.Parameters.AddWithValue("@Validate", validate);
        SqlCommand objCmd = new SqlCommand("update 會員 set 認證='True', 啟用日期=getdate() where Validate=@Validate", Conn);
        objCmd.Parameters.AddWithValue("@Validate", validate);
        SqlDataReader objReader;

        Conn.Open();
        objReader = checkCmd.ExecuteReader();
        objReader.Read();
        objReader.Dispose();

        objCmd.ExecuteNonQuery();
        
        Conn.Close();

        //Response.Write(validate);
    }
}