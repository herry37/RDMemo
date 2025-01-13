using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class Top2 : System.Web.UI.MasterPage
{
    SqlConnection Conn;
    string sql;
    DataRow[] Rows;
    int count = 0;
    int count1 = 0;
    int count2 = 0;
    int count3 = 0;
    protected void Page_Load(object sender, EventArgs e)
    {

        //string pos = "server=192.168.1.221;database=C501_04;uid=C501_04;pwd=C501_04";    
        //string pos = "server=localhost\\SQLEXPRESS;database=C501_04;uid=C501_04;pwd=C501_04";
        //string pos = "server=(local);database=C501_04;uid=C501_04;pwd=C501_04";
        //string pos = "server=Jesse99.mssql.somee.com;database=Jesse99;uid=Jesse99_SQLLogin_1;pwd=hxfx88o4ve";
        //SqlConnection Conn = new SqlConnection(pos);
        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        sql = "select top 10 * from [商品] where [停權]=0 order by [商品編號] desc";
        SqlDataAdapter objCmd = new SqlDataAdapter(sql, Conn);
        DataSet ds = new DataSet();
        Conn.Open();

        objCmd.Fill(ds, "Adv");
        DataTable AdvTable = ds.Tables["Adv"];
        Rows = AdvTable.Select(null, null, DataViewRowState.CurrentRows);
        Conn.Close();


    }

    protected string img_Check(string img_Check)
    {
       
            img_Check = "goodPicture/" + Rows[count][9].ToString();
            count++;
            return img_Check;     
    }
    protected string goodname_Check(string goodname_Check)
    {
        goodname_Check =  Rows[count1][2].ToString();
        count1++;
        return goodname_Check;        
    }
    protected string detal_Check(string detal_Check)
    {
        detal_Check = Rows[count2][4].ToString();
        count2++;
        return detal_Check;
    }
    protected string id_Check(string id_Check)
    {
        int quotient = count3 / 3;
        id_Check = Rows[quotient][0].ToString();
        count3++;
        return id_Check;
    }
    
}
