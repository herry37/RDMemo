<%@ Page Language="C#" AutoEventWireup="true" CodeFile="70021.aspx.cs" Inherits="_7002_70021" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>客服人員端</title>
<script language="javascript" type="text/javascript">
	var btime = new Date("<%=lb_cu_time.Text%>");

	// 使用時間計數器
	function show_sec() {
		var spanobj;
		var ntime = new Date();
		var rsec = Math.floor((ntime.getTime() - btime.getTime()) / 1000);

		spanobj = document.getElementById("span_sec");
		spanobj.innerHTML = rsec.toString();

		setTimeout("show_sec()", 1000);
	}
</script>
</head>
<body onload="show_sec()">
	<form id="form1" runat="server">
	<div>
	<center>
	<p align="center" class="text18pt" style="margin:10pt 0pt 5pt 0pt; font-family:標楷體;">客服人員端</p>
	<p align="center" class="text14pt" style="margin:5pt 0pt 5pt 0pt; font-family:標楷體;">對話視窗</p>
	<table width="95%" border="1" cellpadding="4" cellspacing="0">
	<tr style="background-color:#66FF66; height:24px">
		<td align="left" style="width:50%">客戶姓名：<asp:Label ID="lb_cu_name" runat="server" Text=""></asp:Label></td>
		<td align="left" style="width:50%">經過時間：<span id="span_sec">0</span> sec</td>
	</tr>
	<tr><td colspan="2">
			<iframe id="if_msg" width="98%" src="700212.aspx?sid=<%=lb_cu_sid.Text%>" scrolling="yes" frameborder="1" style="height:300px"></iframe>
		</td>
	</tr>
	<tr><td colspan="2">
			<iframe id="if_send" width="100%" src="700211.aspx?sid=<%=lb_cu_sid.Text%>" frameborder="0" style="height:150px"></iframe>
		</td>
	</tr>
	</table>
	<p style="margin:10px 0px 10px 0px"><asp:Button ID="bn_end" runat="server" Text="結束離開" onclick="bn_end_Click" Height="24px" /></p>
	</center>
	<asp:Label ID="lb_cu_sid" runat="server" Text="0" Visible="false"></asp:Label>
	<asp:Label ID="lb_mg_sid" runat="server" Text="-1" Visible="false"></asp:Label>
	<asp:Label ID="lb_mg_name" runat="server" Text="" Visible="false"></asp:Label>
	<asp:Label ID="lb_cu_time" runat="server" Text="" Visible="false"></asp:Label>
	</div>
	</form>
</body>
</html>

