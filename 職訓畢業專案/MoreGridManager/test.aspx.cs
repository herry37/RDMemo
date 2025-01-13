using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    //protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
    //{

    //}
    //protected void DropDownList4_SelectedIndexChanged(object sender, EventArgs e)
    //{
        
    //}
    //protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    //{
    //    //if (e.CommandName == "Update")
    //    //{
    //    //    int index = Convert.ToInt32(e.CommandArgument);
    //    //    GridViewRow row = GridView1.Rows[index];
            
    //    //    DropDownList ddla = (DropDownList)row.FindControl("DropDownList4");
    //    //    GridView1.
    //    //    listPriceTextBox.Text = (Convert.ToDouble(listPriceTextBox.Text) * 1.05).ToString();     

    //    //}
    //}

    //protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowIndex != -1)
    //    {
    //        if (((LinkButton)e.Row.Cells[4].Controls[0]).CommandName == "Update")
    //        {
    //            Label a = (Label)e.Row.FindControl("Label4");
    //            a.Text = ((DropDownList)e.Row.FindControl("DropDownList4")).SelectedValue;
    //        }
    //    }
    //}
   
   
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList d1 = sender as DropDownList;
        DropDownList d2 = (DropDownList)d1.FindControl("DropDownList2");
        d2.Items.Clear();
        // ddlarea.DataBind();
        d2.Items.Insert(0, new ListItem("請選擇", "0"));
     

    }
   
}