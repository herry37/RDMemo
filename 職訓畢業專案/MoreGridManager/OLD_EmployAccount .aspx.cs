using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;              //使用sqlconnectoin要加這兩行
using System.Data.SqlClient;   //
using System.Configuration; //使用config檔要記得加這


public partial class EmployAccount : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        labmsg.Text = "";
        if (!IsPostBack)
        {
            int inYear = System.DateTime.Now.Year;
            for (int i = 0; i < 101; i++)
            {
                DropDownList1.Items.Add((inYear - i).ToString());
            }
            for(int i=1; i<=12;i++)
            {
                DropDownList2.Items.Add(i.ToString());
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand objCmd = new SqlCommand(" SELECT * FROM 員工帳號 WHERE 員工帳號 = @Account", Conn);
        objCmd.Parameters.AddWithValue("Account", txtAccount.Text);
        Conn.Open();
        SqlDataReader dr = objCmd.ExecuteReader();
       
        if (dr.HasRows)
        {
            labmsg.ForeColor = System.Drawing.Color.Red;
            labmsg.Text = txtAccount.Text +" --重複--\t----資料新增失敗----";
        }
        else
        {
            //<%--InsertCommand="INSERT INTO [員工帳號基本資料] ([身份證字號], [部門代號], [職稱代號], [姓名], [性別], [出生日期], [email], [聯絡電話], [郵遞代號], [地址], [在職]) VALUES (@身份證字號, @部門代號, @職稱代號, @姓名, @性別, @出生日期, @email, @聯絡電話, @郵遞代號, @地址, @在職)">--%>
            SqlDataSource sql1 = new SqlDataSource();
            sql1.ConnectionString = Conn.ConnectionString;
            sql1.InsertParameters.Add("員工帳號", txtAccount.Text);
            sql1.InsertParameters.Add("員工密碼", txtpwd.Text);
            sql1.InsertParameters.Add("姓名",txtName.Text);
            sql1.InsertParameters.Add("身份證字號", txtId.Text);
            sql1.InsertParameters.Add("性別", rdbsex.SelectedValue);
            sql1.InsertParameters.Add("部門代號", ddlDep.SelectedValue);
            sql1.InsertParameters.Add("職稱代號", ddlEmp.SelectedValue);
            sql1.InsertParameters.Add("出生日期", txtBirth.Text);
            sql1.InsertParameters.Add("Email", txtmail.Text);
            sql1.InsertParameters.Add("郵遞代號",ddlarea.SelectedValue);
            sql1.InsertParameters.Add("地址", txtAddress.Text);
            sql1.InsertParameters.Add("聯絡電話", txtphone.Text);
            sql1.InsertCommand = "INSERT INTO [員工帳號] ([員工帳號], [員工密碼]) VALUES (@員工帳號, @員工密碼);INSERT INTO [員工帳號基本資料] ([員工帳號],[身份證字號], [部門代號], [職稱代號], [姓名], [性別], [出生日期], [email], [聯絡電話], [郵遞代號], [地址]) VALUES (@員工帳號,@身份證字號, @部門代號, @職稱代號, @姓名, @性別, @出生日期, @Email, @聯絡電話, @郵遞代號, @地址)";

            int addMsg = sql1.Insert();
            if (addMsg == 0)
            {
                labmsg.ForeColor = System.Drawing.Color.Red;
               // labmsg.Text = txtAccount.Text + "<br>----資料新增失敗----";
                Response.Write("<script>alert('----帳號申請失敗----');location.href='login.aspx';</script>");

            }

            else
            {
                labmsg.ForeColor = System.Drawing.Color.Blue;
                
                //labmsg.Text = txtAccount.Text + "<br>----資料新增成功----";
                Response.Write("<script>alert('----帳號申請成功----\\n待系統管理員啟用後即可使用');location.href='login.aspx';</script>");
            }
        }
        Conn.Close();

    }
    protected void Calendar1_SelectionChanged(object sender, EventArgs e)
    {
        txtBirth.Text = CalBirth.SelectedDate.ToShortDateString();
        txtDay.Text = CalBirth.SelectedDate.Day.ToString();
        DropDownList1.SelectedValue = CalBirth.SelectedDate.Year.ToString();
        DropDownList2.SelectedValue = CalBirth.SelectedDate.Month.ToString();

    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        CalBirth.VisibleDate = new DateTime(Convert.ToInt32(DropDownList1.SelectedValue), Convert.ToInt32(DropDownList2.SelectedValue), 1);
        txtBirth.Text = DropDownList1.Text +"-"+ DropDownList2.Text+"-" + txtDay.Text;

    }

    protected void ddlcity_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlarea.Items.Clear();
        ddlarea.Items.Insert(0, new ListItem("請選擇", "0"));
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        Response.Redirect("login.aspx");
    }
    protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string id = txtId.Text.ToUpper();
        txtId.Text = id;
        string[] arrFw = { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "W", "Z", "I", "O" };
        int intFwNum = 0;

        for (int i = 0; i < 26; i++)
        {
            if (arrFw[i] == id.Substring(0, 1))
            {
                intFwNum = i + 10;
                break;
            }
        }

        int[] arrNum = new int[9];
        int intN1 = intFwNum / 10;
        int intN2 = intFwNum % 10;

        for (int j = 0; j < 9; j++)
        {
            arrNum[j] = Convert.ToInt16(id.Substring(j + 1, 1));
        }

        int intCheckNum = 0;
        for (int k = 0; k < 8; k++)
        {
            intCheckNum += arrNum[k] * (8 - k);
        }
        intCheckNum += intN1 + intN2 * 9 + arrNum[8];
        if (intCheckNum % 10 == 0)
            args.IsValid = true;
        else
            args.IsValid = false;
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        //新增資料庫物件 (資料表,資料庫)
        SqlCommand objCmd = new SqlCommand("select * from 員工帳號基本資料 where 員工帳號=@id", Conn);
        objCmd.Parameters.AddWithValue("@id", txtAccount.Text);
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