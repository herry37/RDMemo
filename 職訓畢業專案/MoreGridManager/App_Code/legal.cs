using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// legal 的摘要描述
/// </summary>
public class legal
{
	public legal()
	{
		//
		// TODO: 在這裡新增建構函式邏輯
		//
	}
    public static void defense() 
    {
        if (Convert.ToInt16(HttpContext.Current.Session["Login"]) != 4)
        {
            HttpContext.Current.Response.Redirect("login.aspx");
        }
    }


}