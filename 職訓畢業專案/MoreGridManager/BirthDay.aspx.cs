using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BirthDay : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            int inYear = System.DateTime.Now.Year;
            for (int i = 0; i < 101; i++)
            {
                ddlyear.Items.Add((inYear - i).ToString());
            }
            for (int i = 1; i <= 12; i++)
            {
                ddlmonth.Items.Add(i.ToString());
            }
        }
    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        Cal.VisibleDate = new DateTime(Convert.ToInt32(ddlyear.SelectedValue), Convert.ToInt32(ddlmonth.SelectedValue), 1);

    }
    protected void Cal_SelectionChanged(object sender, EventArgs e)
    {
        txtday.Text = Cal.SelectedDate.ToShortDateString();

       
        
    }


    protected void Button1_Click(object sender, EventArgs e)
    {

 if (Request.QueryString["DialogClientID"] != null)
        {
            string strClientID = Request["DialogClientID"].ToString().Trim();
            StringBuilder sbJScript = new StringBuilder();
           sbJScript.Append("window.opener.document.getElementById('" + strClientID + "').value= '" + this.txtday.Text + "';");
          //  sbJScript.Append("window.opener.document.getElementById('" + strClientID + "').value = '" + this.Cal.SelectedDate.ToString("yyyy/MM/dd") + "';");
            sbJScript.Append("window.close();");
            ClientScript.RegisterStartupScript(this.GetType(), "ReturnValue", sbJScript.ToString(), true);
        }
    }
}