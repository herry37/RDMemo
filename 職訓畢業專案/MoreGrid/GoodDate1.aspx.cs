using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GoodDate1 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Calendar1_SelectionChanged(object sender, EventArgs e)
    {
        if (Request.QueryString["DialogClientID"] != null)
        {
            string strClientID = Request["DialogClientID"].ToString().Trim();
            StringBuilder sbJScript = new StringBuilder();
            sbJScript.Append("window.opener.document.getElementById('" + strClientID + "').value = '" + this.Calendar1.SelectedDate.ToString("yyyy/MM/dd") + "';");
            sbJScript.Append("window.close();");
            ClientScript.RegisterStartupScript(this.GetType(), "ReturnValue", sbJScript.ToString(), true);
        }
    }
}