<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ckeditor.aspx.cs" Inherits="gmManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <%--崁入ckeditor--%>
    <script src="ckeditor/ckeditor.js" type="text/javascript"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%--呼叫ckeditor--%>
            <textarea id="editor12" class="ckeditor" cols="80" rows="10" name="editor12"></textarea>

        </div>


    </form>
</body>
</html>
