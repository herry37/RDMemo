<%@ Page Language="C#" AutoEventWireup="true" CodeFile="700112.aspx.cs" Inherits="_7001_700112" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<title>客戶使用端</title>
</head>
<body>
	<form id="form1" runat="server"><div>
	<asp:ScriptManager ID="sm_manager" runat="server"></asp:ScriptManager>
	<asp:UpdatePanel ID="up_messange" runat="server" ChildrenAsTriggers="False" UpdateMode="Conditional">
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="ti_getdata" EventName="Tick">
			</asp:AsyncPostBackTrigger>
		</Triggers>
		<ContentTemplate>
			<asp:Literal ID="lt_data" runat="server" Text=""></asp:Literal>
			<asp:Label ID="lb_cm_sid" runat="server" Text="0" Visible="false"></asp:Label>
			<asp:Label ID="lb_count" runat="server" Text="0" Visible="false"></asp:Label>
		</ContentTemplate>
	</asp:UpdatePanel>
	<asp:Timer ID="ti_getdata" runat="server" Interval="1500" ontick="ti_getdata_Tick"></asp:Timer>
	<asp:Label ID="lb_cu_sid" runat="server" Text="0" Visible="false"></asp:Label>
	<asp:Label ID="lb_mg_sid" runat="server" Text="-1" Visible="false"></asp:Label>
	<asp:Label ID="lb_cu_name" runat="server" Text="***" Visible="false"></asp:Label>
	<asp:Label ID="lb_mg_name" runat="server" Text="***" Visible="false"></asp:Label></div>
	</form>
</body>
</html>
