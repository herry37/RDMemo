using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using System.IO;
using System.Text;

public partial class ShopingCar : System.Web.UI.Page
{
    int a;
    int count ;
    string str2;
    string[] str;
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (Session["u_id"] == null || Session["u_id"] == "")
        //{
        //    Response.Redirect("memberLoginValidate.aspx");
        //}

        //int scLenth;     //求cookies的長度                       
        //Response.Cookies["ShoppingCar"]["166"] = "166";
        //Response.Cookies["ShoppingCar"].Expires = DateTime.Now.AddDays(1); //這個會使cookie重新寫入要和上面一起注解掉
        //scLenth = Request.Cookies["ShoppingCar2"].Values.Count;
        //如果ShoppingCar2中沒有東西會出錯，所以請if判斷

        //不能使用    
        //count = 0;
        ////檢查是否有COOKIES
        //if (Request.Cookies["ShoppingCar"] == null)
        //{
        //    Response.Write("目前購物車中尚無商品");
        //    Button1.Visible = false;
        //    Button2.Visible = false;
        //    btDelete.Visible = false;
        //    btclear.Visible = false;
        //}
        //else
        //{ 
        // str = new string[Request.Cookies["ShoppingCar"].Values.Count];
        //foreach (string strVar in Request.Cookies["ShoppingCar"].Values)
        //{

        //    str[count] = Request.Cookies["ShoppingCar"][strVar];


        //    count++;

        //}
        ////Response.Write(Request.Cookies["ShoppingCar"]["6"]+"</br>");
        //str2 = string.Join(",", str);


        //////string[] str = { "164", "170", "169", "168", "167" };
        ////string str2 = string.Join(",", str);
        ////Response.Write(str2);

        //SqlDataSource1.SelectCommand = "SELECT * FROM [商品] WHERE [商品編號] IN (" + str2 + ")order by CHARINDEX (','+CONVERT(varchar,[商品編號])+',','," + str2 + ",')";
        //SqlDataSource1.Dispose();
        //}

        ////DataBind();
//重寫
       
       
              
            HttpCookie aCookie;
            count = 0;
         
           
            str = new string[Request.Cookies.Count];
          
            for (int i = 0; i < (Request.Cookies.Count ); i++)
            {
                aCookie = Request.Cookies[i];                            
                  str[count] = aCookie.Name;
                  count++;                            
            }
            count = 0;


            for (int i = 0; i < str.Count(); i++)
            {
                
                if(!int.TryParse(str[i], out a))
                {
                 count++;
                }

            }
            string[] str3 = new string[str.Count() - count];
            count = 0;
            for (int i = 0; i < str.Count(); i++)
            {
               
                if (int.TryParse(str[i], out a))
                {
                    str3[count] = str[i];
                    count++;
                }
               
            }
            if (str3.Count() == 0)
            {
                Labeltxt5.Visible = true; 
               // Response.Write("目前購物車中尚無商品");
                Button1.Visible = false;
                Button2.Visible = false;
                btDelete.Visible = false;
                btclear.Visible = false;
            }
            else
            {
                string str2 = string.Join(",", str3);
                SqlDataSource1.SelectCommand = "SELECT * FROM [商品] WHERE [商品編號] IN (" + str2 + ")order by CHARINDEX (','+CONVERT(varchar,[商品編號])+',','," + str2 + ",')";
                SqlDataSource1.Dispose();
            }

           
        }

    

    protected void btDelete_Click(object sender, EventArgs e)
    {
        
                //int tmp = 0;//計算被勾的個數
                
                 
                
                    for (int i = 0; i < GridView1.Rows.Count; i++) 
                    {
                     CheckBox cbCheck = (CheckBox)GridView1.Rows[i].Cells[1].FindControl("cbCheck");
                     if (cbCheck.Checked == true)
                     {
                     // HttpCookie aCookie;
                        Response.Cookies[cbCheck.Text].Expires = DateTime.Now.AddDays(-1);                  
                     }
                    
                    }
                    Response.Redirect("shoppingCar.aspx"); 
                  

                    //HttpCookie acookie;
                    //for (int i = 0; i < GridView1.Rows.Count; i++)//如果被勾的數量等於欄數代表全被刪除
                    // {//抓ChickBox勾起來的索引值
                    //     CheckBox cbCheck = (CheckBox)GridView1.Rows[i].Cells[1].FindControl("cbCheck");                        
                    //     if (cbCheck.Checked == true)
                    //        {
                    //           tmp++;  
                    //         if (GridView1.Rows.Count == tmp)
                    //            {                                  
                    //                Response.Cookies[tmp].Expires = DateTime.Now.AddDays(-1);
                    //                Response.Redirect("shoppingCar.aspx");                                   
                    //            }
                    //            else
                    //            { 
                    //                //Response.Write(i);                   
                    //                acookie = Request.Cookies["ShoppingCar"];
                    //                acookie.Values.Remove(cbCheck.Text);
                    //                //acookie.Values.Remove(i.ToString());
                    //                acookie.Expires = DateTime.Now.AddDays(1);
                    //                Response.Cookies.Add(acookie);                 
                    //                Response.Cookies["ShoppingCar"].Expires = DateTime.Now.AddDays(1);
                    //            }
                            
                    //                 str = new string[Request.Cookies["ShoppingCar"].Values.Count];
                    //                 count = 0;//計算陣列索引值
                    //                 foreach (string strVar in Request.Cookies["ShoppingCar"].Values)
                    //                 {
                    //                     str[count] = Request.Cookies["ShoppingCar"][strVar];
                    //                     count++;
                    //                 }
                    //                 string str2 = string.Join(",", str);
                    //                 SqlDataSource1.SelectCommand = "SELECT * FROM [商品] WHERE [商品編號] IN (" + str2 + ")order by CHARINDEX (','+CONVERT(varchar,[商品編號])+',','," + str2 + ",')";
                    //                 SqlDataSource1.Dispose(); 
                    //         }

                        
                //}
                              
                  
    }
    
    protected void btclear_Click(object sender, EventArgs e)
    {
        
        //cookies沒辦法清除，只有藉由把存續時間調成-1後下次進入頁面時才能清掉   
        
        HttpCookie aCookie;
        for (int i = 0; i < (Request.Cookies.Count); i++)
        {
          
             aCookie = Request.Cookies[i];
             Response.Cookies[aCookie.Name].Expires = DateTime.Now.AddDays(-1);  // Response.Cookies["裡面不能放索引值"].Expires
          
        }
           
        Response.Redirect("shoppingCar.aspx"); //哈哈，就給他重新導回自己頁面就搞定了
                 
    }

    protected void Button2_Click(object sender, EventArgs e)
    {

    }
}
        

