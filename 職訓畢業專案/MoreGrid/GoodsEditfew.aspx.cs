using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
//加入config設定
using System.Configuration;
using System.Drawing;
using System.Web.Configuration;


public partial class GoodsEditfew : System.Web.UI.Page
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
            smallImage.Save(savePath + fileName);
            image.Dispose();
            System.IO.File.Delete(tempName);
        }
        else
        {
            image.Save(savePath + fileName);
        }
    }

    String userId;

    protected void Page_Load(object sender, EventArgs e)
    {
        //在Page_Load切換為EditItemTemplate
        FormViewGoodsEditfew.ChangeMode(FormViewMode.Edit);

        //if (Session["u_id"] != null)
        //{
        //    userId = Session["u_id"].ToString();
        //}
        //else
        //{
        //    Response.Redirect("memberLoginValidate.aspx");
        //}

        //擷取網址資料，存入新變數
        string fuxx ="alert('在那邊亂輸入網址！');alert('勿以惡小而為之！');alert('再點10次再放過你！');alert('再點9次再放過你！');alert('再點8次再放過你！');alert('再點7次再放過你！');alert('再點6次再放過你！');alert('再點5次再放過你！');alert('再點4次再放過你！');alert('再點3次再放過你！');alert('再點2次再放過你！');alert('再點1次再放過你！');alert('請勿再犯！你走吧'); location.href='GoodsEdit.aspx';";
        string GoodsNo = Request.QueryString["GsNo"];
        if (Session["u_id"] != null) { 
        userId = Session["u_id"].ToString();
        }
        else
        {
            Response.Redirect("memberLoginValidate.aspx");
        }

        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand checkCmd = new SqlCommand("select 帳號 from 商品 where 商品編號='" + GoodsNo + "'", Conn);

        SqlDataReader objReader;
        Conn.Open();
        objReader = checkCmd.ExecuteReader();
        objReader.Read();
        if (objReader[0].ToString() != userId)
        {

            Response.Write("<script>" + fuxx + "</script>");
        }
        Conn.Close();

       


        if (GoodsNo == null)
        {
            Response.Redirect("GoodsEdit.aspx");
        }
        SqlDataGoodsEditfew.SelectCommand = "SELECT * FROM [商品] where 帳號='" + Session["u_id"].ToString() + "' AND [商品編號]='" + GoodsNo + "'";


    }


    protected void FormViewGoodsEditfew_ItemUpdating(object sender, FormViewUpdateEventArgs e)
    {
        
        string GoodsNo = Request.QueryString["GsNo"];

        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
        SqlCommand checkCmd = new SqlCommand("select * from 商品 where 商品編號='" + GoodsNo + "'", Conn);
 
        SqlDataReader objReader;
        Conn.Open();

        objReader = checkCmd.ExecuteReader();
        objReader.Read();
   
        String fileNameS = objReader["商品小圖片"].ToString();
        String fileName1 = objReader["商品大圖片1"].ToString();
        String fileName2 = objReader["商品大圖片2"].ToString();
        String fileName3 = objReader["商品大圖片3"].ToString();
        

        FileUpload F1 = (FileUpload)FormViewGoodsEditfew.FindControl("FileUpload1");
        FileUpload F2 = (FileUpload)FormViewGoodsEditfew.FindControl("FileUpload2");
        FileUpload F3 = (FileUpload)FormViewGoodsEditfew.FindControl("FileUpload3");
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

        
        //判斷圖片檔名，並寫入資料庫
        if (fileNameS != "" || fileNameS != null)
            SqlDataGoodsEditfew.UpdateParameters.Add("商品小圖片", fileNameS);
        if (fileName1 != "" || fileName1 != null)
            SqlDataGoodsEditfew.UpdateParameters.Add("商品大圖片1", fileName1);
        if (fileName2 != "" || fileName2 != null)
            SqlDataGoodsEditfew.UpdateParameters.Add("商品大圖片2", fileName2);
        if (fileName3 != "" || fileName3 != null)
            SqlDataGoodsEditfew.UpdateParameters.Add("商品大圖片3", fileName3);
        //else
        //    SqlDataGoodsEditfew.InsertParameters.Add("商品大圖片3", objReader["商品大圖片3"].ToString());


        //Response.Write(objReader["商品大圖片1"].ToString());
        //Response.Write(objReader["商品大圖片2"].ToString());
        //Response.Write(objReader["商品大圖片3"].ToString());

        //HTML編碼
        TextBox goodedit = (TextBox)FormViewGoodsEditfew.FindControl("txtedit");
        String goodeditrand = Server.HtmlEncode(goodedit.Text);
        SqlDataGoodsEditfew.UpdateParameters.Add("編輯", goodeditrand);

        Conn.Close();



        //Response.Redirect("GoodsEdit.aspx");
        Response.Write("<script>location.href='GoodsEdit.aspx'; </script>");
    }
    protected void ButCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("GoodsEdit.aspx");
    }

    protected void FormViewGoodsEditfew_DataBound(object sender, EventArgs e)
    {


        if (!IsPostBack) {

            //加入字數限制
            ((TextBox)FormViewGoodsEditfew.FindControl("txtSummary")).Attributes["maxlength"] = "30";
            ((TextBox)FormViewGoodsEditfew.FindControl("txtDetails")).Attributes["maxlength"] = "1000";
            ((TextBox)FormViewGoodsEditfew.FindControl("txtedit")).Attributes["maxlength"] = "8000";

        //HTML編碼還原
        Label goodedit = (Label)FormViewGoodsEditfew.FindControl("lblCKE");
        String goodeditrand = Server.HtmlDecode(goodedit.Text);
        TextBox txt = (TextBox)FormViewGoodsEditfew.FindControl("txtedit");
        txt.Text = goodeditrand;
        //SqlData.InsertParameters.Add("編輯", goodeditrand);
        }
    }

}