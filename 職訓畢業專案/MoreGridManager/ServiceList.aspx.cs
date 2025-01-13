using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ServiceList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void ServiecListView_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.CommandName == "CnacelSeleted")
        {
            ServiecListView.SelectedIndex = -1;
        }
    }
}