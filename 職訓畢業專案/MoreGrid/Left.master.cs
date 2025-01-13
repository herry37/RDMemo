using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Left : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Response.Write(Session["u_validate"]);
        if (Session["Login"] != null)
        {
            if (Session["Login"].ToString() == "OK")
            {
                if (Session["u_authenticate"] != null)
                {
                    if (Session["u_authenticate"].ToString() != "True")
                    {
                        Response.Write("<script>alert('請至信箱收信認證帳號！'); location.href='Main.aspx'; </script>");

                        //Response.Redirect("Main.aspx");
                    }
                    else
                    {
                        
                    }
                }                
            }
            else
            {
                Response.Write("<script>alert('請先登入會員！location.href='memberLoginValidate.aspx';');</script>");
                
                //Response.End();
                
            }
        }
        
    }
}
