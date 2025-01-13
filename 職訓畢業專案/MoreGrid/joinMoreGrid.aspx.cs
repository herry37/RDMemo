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
//加入mail
using System.Net.Mail;
using System.Text;
using System.Threading;
//加入SHA256
using System.Security.Cryptography;

public partial class joinMoreGrid : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Response.Write(ddlcity.SelectedValue + "," + ddlarea.SelectedValue);
    }
    protected void btGo_Click(object sender, EventArgs e)
    {
        txtBirth.Text=txtYear.Text+"/"+txtMon.Text+"/"+txtDay.Text;
        if (txtTel2.Text != "" || txtTel.Text != "")
            txtTel3.Text = "(" + txtTel2.Text + ")" + txtTel.Text;
        else
            txtTel3.Text = "";
        
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        //SqlCommand objCmd = new SqlCommand("insert into 會員(帳號,密碼,暱稱,註冊日期,性別,email,市話,手機,生日) values('" + txtNo.Text + "','" + txtPsd.Text + "','" + txtName.Text + "',getdate() ,'" + txtGender.SelectedItem.Value + "','" + txtEmail.Text + "','" + txtTel3.Text + "','" + txtMobile.Text + "','" + txtBirth.Text + "');insert into 會員住址(帳號,村里,地址其他,郵遞代碼,縣市代碼) values('" + txtNo.Text + "','" + txtAddress2.Text + "','" + txtAddress.Text + "','" + ddlarea.SelectedValue + "','" + ddlcity.SelectedValue + "')", Conn);
        SqlCommand objCmd = new SqlCommand("insert into 會員(帳號,密碼,暱稱,註冊日期,性別,email,市話,手機,生日) values(@id, @密碼, @暱稱, getdate(), @性別, @email, @市話, @手機, @生日);insert into 會員住址(帳號,村里,地址其他,郵遞代碼,縣市代碼) values(@id, @村里, @地址其他,'" + ddlarea.SelectedValue + "','" + ddlcity.SelectedValue + "')", Conn);
        SqlCommand checkCmd = new SqlCommand("select * from 會員 where 帳號=@id", Conn);

        checkCmd.Parameters.AddWithValue("@id", txtNo.Text);
        checkCmd.Parameters.AddWithValue("@密碼", txtPsd.Text);
        //checkCmd.Parameters.AddWithValue("@暱稱", txtName.Text);
        //checkCmd.Parameters.AddWithValue("@性別", txtGender.SelectedItem.Value);
        //checkCmd.Parameters.AddWithValue("@email", txtEmail.Text);
        //checkCmd.Parameters.AddWithValue("@市話", txtTel3.Text);
        //checkCmd.Parameters.AddWithValue("@手機", txtMobile.Text);
        //checkCmd.Parameters.AddWithValue("@生日", txtBirth.Text);
        //checkCmd.Parameters.AddWithValue("@村里", txtAddress2.Text);
        //checkCmd.Parameters.AddWithValue("@地址其他", txtAddress.Text);
        //checkCmd.Parameters.AddWithValue("@郵遞代碼", ddlarea.SelectedValue);
        //checkCmd.Parameters.AddWithValue("@縣市代碼", ddlcity.SelectedValue);

        //自製純量變數；並且using System.Data.SqlClient;使用SqlDbType
        objCmd.Parameters.Add("id", SqlDbType.NVarChar);
        objCmd.Parameters["id"].Value = txtNo.Text;
        objCmd.Parameters.Add("密碼", SqlDbType.VarChar);
        objCmd.Parameters["密碼"].Value = txtPsd.Text;
        objCmd.Parameters.Add("暱稱", SqlDbType.NVarChar);
        objCmd.Parameters["暱稱"].Value = txtName.Text;
        objCmd.Parameters.Add("性別", SqlDbType.Bit);
        objCmd.Parameters["性別"].Value = txtGender.SelectedItem.Value;
        objCmd.Parameters.Add("email", SqlDbType.VarChar);
        objCmd.Parameters["email"].Value = txtEmail.Text;
        objCmd.Parameters.Add("市話", SqlDbType.VarChar);
        objCmd.Parameters["市話"].Value = txtTel3.Text;
        objCmd.Parameters.Add("手機", SqlDbType.VarChar);
        objCmd.Parameters["手機"].Value = txtMobile.Text;
        objCmd.Parameters.Add("生日", SqlDbType.VarChar);
        objCmd.Parameters["生日"].Value = txtBirth.Text;
        objCmd.Parameters.Add("村里", SqlDbType.NVarChar);
        objCmd.Parameters["村里"].Value = txtAddress2.Text;
        objCmd.Parameters.Add("地址其他", SqlDbType.NVarChar);
        objCmd.Parameters["地址其他"].Value = txtAddress.Text;
        //objCmd.Parameters.Add("郵遞代碼", SqlDbType.Int);
        //objCmd.Parameters["郵遞代碼"].Value = "@郵遞代碼";
        //objCmd.Parameters.Add("縣市代碼", SqlDbType.Int);
        //objCmd.Parameters["縣市代碼"].Value = "@縣市代碼";

        //SHA256 sha256 = new SHA256CryptoServiceProvider();
        //string resultSha256 = Convert.ToBase64String(sha256.ComputeHash(Encoding.Default.GetBytes(txtNo.Text)));
        //checkCmd.Parameters.AddWithValue("@Validate", resultSha256);
        //Response.Write("SHA256加密:  " + resultSha256);
        SqlDataReader objReader;

        Conn.Open();
        objReader = checkCmd.ExecuteReader();
        if (objReader.Read())
        {
            Response.Write("<script>alert('已有相同帳號');</script>");
            txtBirth.Text = "";
            txtYear.Text = "";
            txtMon.Text = "";
            txtDay.Text = "";
        }
        else 
        {
            objReader.Dispose();
            objCmd.ExecuteNonQuery();

            txtBirth.Text = "";
            
            txtName.Text = "";
            
            txtTel.Text = "";
            txtMobile.Text = "";
            txtYear.Text = "";
            txtMon.Text = "";
            txtDay.Text = "";
            Conn.Close();
        }

        SqlConnection Conn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);

        SqlCommand checkCmd2 = new SqlCommand("select * from 會員 where 帳號=@id", Conn2);
        checkCmd2.Parameters.AddWithValue("@id", txtNo.Text);
        txtNo.Text = "";
        SqlDataReader objReader2;
        Conn2.Open();
        objReader2 = checkCmd2.ExecuteReader();
        objReader2.Read();

        Session["u_authenticate"] = objReader2["認證"].ToString();

       
        SmtpClient client = new SmtpClient("msa.hinet.net");
        string subject = "魔格會員認證信";
        string from = "iltusyou@gmail.com";
        string recipients = txtEmail.Text;
        //string body = "請點擊以下網址認證：\n" + "http://59.120.110.135/project10501/team4/MoreGrid/joinMoreGridValidate.aspx?Validate=" + objReader2["Validate"].ToString();
        string body = "請點擊以下網址認證：\n" + "http://124.218.89.248:8082/MoreGrid/joinMoreGridValidate.aspx?Validate=" + objReader2["Validate"].ToString();
        client.EnableSsl = true;
        client.Send(from, recipients, subject, body);
        //System.Net.Mail.MailMessage client = new System.Net.Mail.MailMessage();
        //client.From = new System.Net.Mail.MailAddress("x810820x@gmail.com", "安安妳好", System.Text.Encoding.UTF8);
        //client.To.Add(new System.Net.Mail.MailAddress(txtEmail.Text));    //收件者
        //client.Subject = "魔格會員認證信";     //信件主題 
        //client.SubjectEncoding = System.Text.Encoding.UTF8;
        //client.Body = "請點擊以下網址認證：\n" + "http://59.120.110.135/project10501/team4/MoreGrid/joinMoreGridValidate.aspx?Validate=" + objReader2["Validate"].ToString();            //內容 
        //client.BodyEncoding = System.Text.Encoding.UTF8;
        //client.IsBodyHtml = true;	  //信件內容是否使用HTML格式
        //System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
        ////登入帳號認證  
        //smtp.Credentials = new System.Net.NetworkCredential("帳號", "密碼");
        ////使用587 Port - google要設定
        //smtp.Port = 587;
        //smtp.EnableSsl = true;   //啟動SSL 
        ////end of google設定
        //smtp.Host = "smtp.gmail.com";   //SMTP伺服器
        //smtp.Send(client);            //寄出
        //txtEmail.Text = "";
        //objReader2.Dispose();
        Conn2.Close();

        Response.Write("<script>alert('加入會員成功！請至信箱收取認證信！');location.href='Main.aspx';</script>");
        
        //Response.Redirect("Main.aspx");
    }
    protected void ddlcity_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlarea.Items.Clear();
        ddlarea.Items.Insert(0, new ListItem("請選擇", "0"));
    }
    protected void checkNo_Click(object sender, EventArgs e)
    {

        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        //新增資料庫物件 (資料表,資料庫)
        SqlCommand objCmd = new SqlCommand("select * from 會員 where 帳號=@id", Conn);
        objCmd.Parameters.AddWithValue("@id", txtNo.Text);
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
        
    }
}