<%@ Page Language="C#" AutoEventWireup="true" CodeFile="testImage.aspx.cs" Inherits="testImage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="Button1" runat="server" Text="Button"  OnClick="Button1_Click"/>
        <br />
        <asp:Label ID="Label1" runat="server" ></asp:Label>
        <br />
        <br />
        <div>
            <ol>
                <li>上傳圖檔，限定上傳檔案類型為 .jpg .jepg .png .gif</li>
                <li>在bimg資料家儲存一個大圖 長高超過500px會自動等比例縮圖為長高最大值500</li>
                <li>在simg資料家儲存一個小圖 長高超過200px會自動等比例縮圖為長高最大值200</li>
                <li>依上傳時間改檔名</li>
            </ol>
        </div>
    </div>
    </form>
</body>
</html>
