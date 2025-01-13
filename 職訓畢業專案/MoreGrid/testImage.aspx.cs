using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

using System.Drawing;

public partial class testImage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Label1.Text = "";

        string appPath = Request.PhysicalApplicationPath;//求出網站的根目錄在伺服器上的實體路徑
        string savePath_b = Server.MapPath("bimg/");//求出此資料夾在伺服器上的實體路徑
        string savePath_s = Server.MapPath("simg/");//求出此資料夾在伺服器上的實體路徑
        
        //如果有檔案則執行，否則顯示錯誤訊息
        if (FileUpload1.HasFile)
        {
            string fileName = FileUpload1.FileName; //上傳的檔案名稱            
            string fileExtension = System.IO.Path.GetExtension(fileName);//上傳的檔案名稱的副檔名
            string[] allowedExtensions = {".jpg", ".jpeg", ".png", ".gif"};//允許上傳的副檔名
            Boolean fileOK = false;//判斷副檔名是否符合

            for (int i = 0; i < allowedExtensions.Length; i++)
            {
                if (fileExtension == allowedExtensions[i])
                    fileOK = true;
            }

            if (fileOK)
            {                
                fileName = DateTime.Now.ToString("yyMMddhhmmss")+fileExtension;//依上傳時間重設檔名，並加回副檔名

                //縮圖功能
                string tempPath = Server.MapPath("temp/"); //求出此資料夾在伺服器上的實體路徑
                string tempName = tempPath + FileUpload1.FileName; //

                FileUpload1.SaveAs(tempName);

                System.Drawing.Image.GetThumbnailImageAbort callBack =
             new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

                Bitmap image = new Bitmap(tempName);

                if (image.Width > 500 || image.Height > 500)
                {
                    // 計算維持比例的縮圖大小
                    int[] thumbnailScale = getThumbnailImageScale(500, 500, image.Width, image.Height);

                    // 產生縮圖
                    System.Drawing.Image smallImage =
                    image.GetThumbnailImage(thumbnailScale[0], thumbnailScale[1], callBack, IntPtr.Zero);

                    // 將縮圖存檔
                    savePath_b = savePath_b + "b" + fileName;
                    smallImage.Save(savePath_b);
                }
                else
                {
                    savePath_b = savePath_b + "b" + fileName;
                    image.Save(savePath_b);
                }

                if (image.Width > 200 || image.Height > 200)
                {
                    // 計算維持比例的縮圖大小
                    int[] thumbnailScale = getThumbnailImageScale(200, 200, image.Width, image.Height);

                    // 產生縮圖
                    System.Drawing.Image smallImage =
                    image.GetThumbnailImage(thumbnailScale[0], thumbnailScale[1], callBack, IntPtr.Zero);

                    // 將縮圖存檔
                    savePath_s = savePath_s + "s" + fileName;
                    smallImage.Save(savePath_s);
                }
                else
                {
                    savePath_s = savePath_s + "s" + fileName;
                    image.Save(savePath_s);
                }

                // 釋放並刪除暫存檔
                image.Dispose();
                System.IO.File.Delete(tempName);
                //FileUpload1.SaveAs(savePath);
                Label1.Text = "上傳成功";
            }
            else
            {
                Label1.Text = "上傳檔案格式錯誤";
            }                
        }
        else
        {
            Label1.Text = "未選擇檔案";
        }
    }
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
}