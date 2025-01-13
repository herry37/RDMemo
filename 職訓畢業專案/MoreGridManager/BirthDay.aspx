<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BirthDay.aspx.cs" Inherits="BirthDay" %>

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
            <asp:DropDownList ID="ddlyear" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>年
            <asp:DropDownList ID="ddlmonth" runat="server"  OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>月
        </div>
        <br />
        <asp:Calendar ID="Cal" runat="server" BackColor="White" BorderColor="#3366CC" BorderWidth="1px" CellPadding="1" DayNameFormat="Shortest" Font-Names="Verdana"
             Font-Size="8pt" ForeColor="#003399" Height="200px" Width="220px" OnSelectionChanged="Cal_SelectionChanged">
            <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
            <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
            <OtherMonthDayStyle ForeColor="#999999" />
            <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
            <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
            <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True" Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
            <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
            <WeekendDayStyle BackColor="#CCCCFF" />
        </asp:Calendar>
        <br />
        <div>
            日期：<asp:TextBox ID="txtday" runat="server" Enabled="false" Width="120px"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" Text="確認" OnClick="Button1_Click" />
           <%-- PostBackUrl="EmployeeData.aspx" --%>
        </div>
    </div>
    </form>
</body>
</html>
