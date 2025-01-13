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


public partial class GoodsInsert : System.Web.UI.Page
{
    private int[] getThumbnailImageScale(int maxWidth, int maxHeight, int oldWidth, int oldHeight)
    {
        int[] result = new int[] { 0, 0 };
        float widthDividend, heightDividend, commonDividend;

        widthDividend = (float)oldWidth / (float)maxWidth;
        heightDividend = (float)oldHeight / (float)maxHeight;

        commonDividend = (heightDividend > widthDividend) ? heightDividend : widthDividend;
        result[0] = (int)(oldWidth / commonDividend);
        result[1] = (int)(oldHeight / commonDividend);

        return result;
    }

    private bool ThumbnailCallback()
    {
        return false;
    }

    //ThumbnailImage(F1, fileNameS, 200, 200);
    void ThumbnailImage(FileUpload F, string fileName, int width, int height)
    {
        string savePath = Server.MapPath("goodPicture/"); //求出此資料夾在伺服器上的實體路徑
        string tempName = savePath + F.FileName;
        F.SaveAs(tempName);

        //https://msdn.microsoft.com/zh-tw/library/system.drawing.image.getthumbnailimage(v=vs.110).aspx
        System.Drawing.Image.GetThumbnailImageAbort callBack =
     new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

        Bitmap image = new Bitmap(tempName);

        if (image.Width > width || image.Height > height)
        {
            // 計算維持比例的縮圖大小
            int[] thumbnailScale = getThumbnailImageScale(width, height, image.Width, image.Height);

            // 產生縮圖
            System.Drawing.Image smallImage =
            image.GetThumbnailImage(thumbnailScale[0], thumbnailScale[1], callBack, IntPtr.Zero);

            // 將縮圖存檔            
            smallImage.Save(savePath+fileName);
            image.Dispose();
            System.IO.File.Delete(tempName);
        }
        else
        {
            image.Save(savePath + fileName);
        }
    }





    //使用者id
    String userId;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["u_id"] != null)
        {
            userId = Session["u_id"].ToString();
        }
        else
        {
            Response.Redirect("memberLoginValidate.aspx");
        }
    }


    protected void FormViewGoodsInsert_ItemInserting(object sender, FormViewInsertEventArgs e)
    {

        txtMessage.Text = "";

        SqlDataSource SqlData = new SqlDataSource();

        SqlData.InsertParameters.Clear();

        SqlData.ConnectionString = WebConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString;

        //取text值
        TextBox goodName = (TextBox)FormViewGoodsInsert.FindControl("txtName");
        SqlData.InsertParameters.Add("商品名稱", goodName.Text);
        TextBox goodSummary = (TextBox)FormViewGoodsInsert.FindControl("txtSummary");
        SqlData.InsertParameters.Add("商品摘要", goodSummary.Text);
        TextBox goodDetails = (TextBox)FormViewGoodsInsert.FindControl("txtDetails");
        SqlData.InsertParameters.Add("商品明細", goodDetails.Text);
        TextBox goodStock = (TextBox)FormViewGoodsInsert.FindControl("txtStock");
        SqlData.InsertParameters.Add("庫存量", goodStock.Text);
        TextBox goodPrice = (TextBox)FormViewGoodsInsert.FindControl("txtPrice");
        SqlData.InsertParameters.Add("單價", goodPrice.Text);
        
        //HttpUtility.HtmlEncode()，轉碼
        TextBox goodedit = (TextBox)FormViewGoodsInsert.FindControl("txtedit");
        String goodeditrand = Server.HtmlEncode(goodedit.Text);
        SqlData.InsertParameters.Add("編輯", goodeditrand);
        //TextBox goodedit = (TextBox)FormViewGoodsInsert.FindControl("txtedit");
        //SqlData.InsertParameters.Add("編輯", goodedit.Text);

        TextBox goodDataUp = (TextBox)FormViewGoodsInsert.FindControl("txtDataUp");
        TextBox goodDataDown = (TextBox)FormViewGoodsInsert.FindControl("txtDataDown");



        SqlData.InsertParameters.Add("帳號", userId);

        //照片編輯
        //Label goodsEditor12 = (Label)FormViewGoodsInsert.FindControl("editor12");


        //TextBox goodPicture2 = (TextBox)FormViewGoodsInsert.FindControl("txtpicture2");
        //SqlData.InsertParameters.Add("網外照片2", goodPicture2.Text);
        //TextBox goodPicture3 = (TextBox)FormViewGoodsInsert.FindControl("txtpicture3");
        //SqlData.InsertParameters.Add("網外照片3", goodPicture3.Text);


        Random nameRandom = new Random();
        String fileName1 = null;
        String fileName2 = null;
        String fileName3 = null;
        String fileNameS = null;
        
        FileUpload F1 = (FileUpload)FormViewGoodsInsert.FindControl("FileUpload1");
        FileUpload F2 = (FileUpload)FormViewGoodsInsert.FindControl("FileUpload2");
        FileUpload F3 = (FileUpload)FormViewGoodsInsert.FindControl("FileUpload3");
        try
        {
            if (F1.HasFile)
            {
                String savePath = Server.MapPath("goodPicture/");
                //另名檔名,以帳號加日期
                fileName1 = F1.FileName;
                string extension = Path.GetExtension(fileName1).ToLowerInvariant();
                fileName1 = userId + DateTime.Now.ToString("yyMMddhhmmss") + "_1" + extension;

                // 判斷是否為允許上傳的檔案附檔名
                List<string> allowedExtextsion = new List<string> { ".jpg", ".bmp", ".png", ".gif" };
                if (allowedExtextsion.IndexOf(extension) == -1)
                {
                    txtMessage.Text += "不允許該檔案上傳";
                    return;
                }
                int filesize = F1.PostedFile.ContentLength;
                if (filesize > 2100000)
                {
                    txtMessage.Text += "檔案大小上限為 2MB，該檔案無法上傳";
                    return;
                }
                savePath = savePath + fileName1;
                F1.SaveAs(savePath);
                txtMessage.Text += "上傳成功~檔名" + fileName1 + "<br>";


                fileNameS = userId + DateTime.Now.ToString("yyMMddhhmmss") + "_s" + extension;
                ThumbnailImage(F1, fileNameS, 200, 200);



            }
            if (F2.HasFile)
            {

                String savePath = Server.MapPath("goodPicture/");
                //另名檔名,以帳號加日期
                fileName2 = F2.FileName;
                string extension = Path.GetExtension(fileName2).ToLowerInvariant();
                fileName2 = userId + DateTime.Now.ToString("yyMMddhhmmss") + "_2" + extension;

                // 判斷是否為允許上傳的檔案附檔名
                List<string> allowedExtextsion = new List<string> { ".jpg", ".bmp", ".png", ".gif" };
                if (allowedExtextsion.IndexOf(extension) == -1)
                {
                    txtMessage.Text += "不允許該檔案上傳";
                    return;
                }
                int filesize = F2.PostedFile.ContentLength;
                if (filesize > 2100000)
                {
                    txtMessage.Text += "檔案大小上限為 2MB，該檔案無法上傳";
                    return;
                }
                savePath = savePath + fileName2;
                F2.SaveAs(savePath);
                txtMessage.Text += "上傳成功~檔名" + fileName2 + "<br>";
            }

            if (F3.HasFile)
            {
                String savePath = Server.MapPath("goodPicture/");
                //另名檔名,以帳號加日期
                fileName3 = F3.FileName;
                string extension = Path.GetExtension(fileName3).ToLowerInvariant();
                fileName3 = userId + DateTime.Now.ToString("yyMMddhhmmss") + "_3" + extension;

                // 判斷是否為允許上傳的檔案附檔名
                List<string> allowedExtextsion = new List<string> { ".jpg", ".bmp", ".png", ".gif" };
                if (allowedExtextsion.IndexOf(extension) == -1)
                {
                    txtMessage.Text += "不允許該檔案上傳";
                    return;
                }
                int filesize = F3.PostedFile.ContentLength;
                if (filesize > 2100000)
                {
                    txtMessage.Text += "檔案大小上限為 2MB，該檔案無法上傳";
                    return;
                }
                savePath = savePath + fileName3;
                F3.SaveAs(savePath);
                txtMessage.Text += "上傳成功~檔名" + fileName3 + "<br>";
            }
        }
        catch (Exception ex)
        {            //ex.Message
            txtMessage.Text += "程式有錯,";
        }


        SqlData.InsertParameters.Add("商品小圖片", fileNameS);
        SqlData.InsertParameters.Add("商品大圖片1", fileName1);
        SqlData.InsertParameters.Add("商品大圖片2", fileName2);
        SqlData.InsertParameters.Add("商品大圖片3", fileName3);

        DropDownList categoryId = (DropDownList)FormViewGoodsInsert.FindControl("DropDownListCategory");


        SqlData.InsertParameters.Add("商品類別代碼", categoryId.SelectedValue);


        if (goodDataDown.Text.Length == 0 && goodDataUp.Text.Length != 0)
        {
            //編輯,下架時間未放進去
            SqlData.InsertParameters.Add("上架時間", Convert.ToDateTime(goodDataUp.Text).ToShortDateString());
            SqlData.InsertCommand = "INSERT INTO [商品] ([帳號], [商品名稱], [商品摘要], [商品明細], [庫存量], [單價],[上架時間],[商品小圖片],[商品大圖片1], [商品大圖片2], [商品大圖片3],[商品類別代碼]) VALUES (@帳號,@商品名稱, @商品摘要, @商品明細, @庫存量, @單價,@上架時間,@商品小圖片, @商品大圖片1, @商品大圖片2, @商品大圖片3,@商品類別代碼)";
        }
        else if (goodDataUp.Text.Length == 0 && goodDataDown.Text.Length != 0)
        {
            //編輯,上架時間未放進去
            SqlData.InsertParameters.Add("下架時間", Convert.ToDateTime(goodDataDown.Text).ToShortDateString());
            SqlData.InsertCommand = "INSERT INTO [商品] ([帳號], [商品名稱], [商品摘要], [商品明細], [庫存量], [單價], [下架時間],[商品小圖片], [商品大圖片1], [商品大圖片2], [商品大圖片3],[商品類別代碼]) VALUES (@帳號,@商品名稱, @商品摘要, @商品明細, @庫存量, @單價, @下架時間,@商品小圖片, @商品大圖片1, @商品大圖片2, @商品大圖片3,@商品類別代碼)";

        }
        else if (goodDataDown.Text.Length == 0 && goodDataUp.Text.Length == 0)
        {
            //編輯,上架時間,下架時間,未放進去
            SqlData.InsertCommand = "INSERT INTO  [商品] ([帳號],[商品名稱], [商品摘要], [商品明細], [庫存量], [單價],[商品小圖片], [商品大圖片1], [商品大圖片2], [商品大圖片3],[商品類別代碼]) VALUES (@帳號,@商品名稱, @商品摘要, @商品明細, @庫存量, @單價,@商品小圖片, @商品大圖片1, @商品大圖片2, @商品大圖片3,@商品類別代碼)";
        }
        //else if (goodedit.Text != "" || goodedit.Text != null)
        //{
        //    //編輯,未放進去
        //    SqlData.InsertParameters["編輯"].DefaultValue = goodedit.Text;
        //    //SqlData.InsertParameters.Add("@編輯", goodedit.Text);
        //    SqlData.InsertCommand = "INSERT INTO [商品] ([帳號] ,[商品名稱], [商品摘要], [商品明細], [庫存量], [單價], [上架時間], [下架時間], [商品大圖片1], [商品大圖片2], [商品大圖片3],[商品類別代碼],[編輯]) VALUES (@帳號,@商品名稱, @商品摘要, @商品明細, @庫存量, @單價, @上架時間, @下架時間,@商品大圖片1, @商品大圖片2, @商品大圖片3,@商品類別代碼,@編輯)";
        //}
        else
        {

            SqlData.InsertParameters.Add("上架時間", Convert.ToDateTime(goodDataUp.Text).ToShortDateString());
            SqlData.InsertParameters.Add("下架時間", Convert.ToDateTime(goodDataDown.Text).ToShortDateString());
           
            //放進SqlData
            SqlData.InsertCommand = "INSERT INTO [商品] ([帳號], [商品名稱], [商品摘要], [商品明細], [庫存量], [單價], [上架時間], [下架時間],[商品小圖片], [商品大圖片1], [商品大圖片2], [商品大圖片3],[商品類別代碼],[編輯]) VALUES (@帳號,@商品名稱, @商品摘要, @商品明細, @庫存量, @單價, @上架時間, @下架時間,@商品小圖片, @商品大圖片1, @商品大圖片2, @商品大圖片3,@商品類別代碼,@編輯)";
        }
        //執行insert
        int affectRow = SqlData.Insert();
        goodDataUp.Text = "";
        goodName.Text = "";
        goodPrice.Text = "";
        goodStock.Text = "";
        goodSummary.Text = "";
        goodDataDown.Text = "";
        goodDetails.Text = "";
        goodedit.Text = "";

        categoryId.SelectedIndex = 0;


        if (affectRow > 0) { txtMessage.Text += "資料新增~成功"; }



        //Response.Write("<Script language='JavaScript'>alert('"+txtMessage.Text+"');window.location('GoodsInsert.aspx');</Script>");

        SqlData.Dispose();
        SqlData.InsertParameters.Clear();
        Session["message"] = txtMessage.Text;
        Response.Redirect("GoodsInsertAfter.aspx");
        Response.End();
    }

    protected void FormViewGoodsInsert_ItemCommand(object sender, FormViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Cancel"))
        {
            Response.Redirect("GoodsInsert.aspx");
        }
    }


    protected void FormViewGoodsInsert_ItemCreated(object sender, EventArgs e)
    {
        ((TextBox)FormViewGoodsInsert.FindControl("txtSummary")).Attributes["maxlength"]="30";
        ((TextBox)FormViewGoodsInsert.FindControl("txtDetails")).Attributes["maxlength"] = "1000";
        ((TextBox)FormViewGoodsInsert.FindControl("txtedit")).Attributes["maxlength"] = "8000";
    }
    protected void ButInsert_Click(object sender, EventArgs e)
    {
        if (Session["Login"] != null)
        {
            if (Session["Login"].ToString() != "OK")
            {
                 Response.Write("<script>alert('逾時登入！location.href='memberLoginValidate.aspx';');</script>");
            }
           
        }
    }
}
