<%@ Page Language="C#" AutoEventWireup="true" CodeFile="700111.aspx.cs" Inherits="_7001_700111" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<title>客戶使用端</title>
<script language="javascript" type="text/javascript">
	function send_msg() {
		var bnobj = document.getElementById("bn_smsg");
		
		if (bnobj != null)
			bnobj.click();
	}
</script>
</head>
<body style="margin:5px 0px 5px 0px" onkeydown="if(!(window.event.shiftKey) && window.event.keyCode==13) {send_msg();}">
	<form id="form1" runat="server">
	<div>
	<center>
	<table width="100%" border="0" cellpadding="2" cellspacing="0">
	<tr><td colspan="3" align="center">
			<asp:TextBox ID="tb_cm_desc" runat="server" MaxLength="1000" Rows="4" TextMode="MultiLine" Width="98%" ToolTip="一個訊息最多1000個字，超過會截斷！"></asp:TextBox>
		</td>
	</tr>
	<tr><td align="left">&nbsp;<asp:Button ID="bn_smsg" runat="server" Text="傳送訊息" Height="24px" onclick="bn_smsg_Click" /></td>
		<td align="center">訊息輸入時請用 Shift-Enter 換行。按下 Enter 即發送訊息。</td>
		<td align="right">
            &nbsp;&nbsp;
		</td>
	</tr>
	</table>
	</center>
	<asp:Label ID="lb_cu_sid" runat="server" Text="0" Visible="false"></asp:Label>
	<asp:Label ID="lb_mg_sid" runat="server" Text="-1" Visible="false"></asp:Label>
	</div>
	<script language="javascript" type="text/javascript">
		resize();

		// 重新調整母頁框的高度
		function resize() {
			var ifobj;
			ifobj = parent.document.getElementById("if_send");
			if (ifobj != null) {
				ifobj.style.height = (document.body.clientHeight + 10) + "px";
			}
		}
	</script>
	</form>
</body>
</html>
