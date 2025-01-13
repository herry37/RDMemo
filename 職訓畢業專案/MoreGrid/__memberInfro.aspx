<%@ Page Language="C#" AutoEventWireup="true" CodeFile="__memberInfro.aspx.cs" Inherits="memberInformation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>
            暱稱:<asp:TextBox ID="TBnikename" runat="server"></asp:TextBox>
            主要聯絡email:<asp:TextBox ID="TBemail" runat="server"></asp:TextBox>
            地址:<asp:TextBox ID="TBaddress" runat="server"></asp:TextBox>
            電話:<asp:TextBox ID="TBphone1" runat="server"></asp:TextBox>
            備用電話:<asp:TextBox ID="TBphone2" runat="server"></asp:TextBox>
        </div>
    
    </div>
    </form>
</body>
</html>
