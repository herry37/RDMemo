<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="7002.aspx.cs" Inherits="Service_7002" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
     <link href="../css/bootstrap.css" rel="stylesheet" />
    <style type="text/css">
        table {
            border-collapse: collapse;
            border: solid 2px Black;
           
        }

            table td {
                width: 80px;
                height: 30px;
                border: solid 1px Black;
                padding: 5px;
               
            }

        #content {          
            margin: 0 auto;            
            width: 80%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <div id="content">
          <br /><br />
           <a href="Service.aspx" class="btn btn-success">返回客服系統</a>
          	<asp:ScriptManager ID="sm_manager" runat="server"></asp:ScriptManager>
	
	<p   style="margin:10pt 0pt 10pt 0pt;text-align:center; font-family:標楷體;font-size:36px">客服人員端</p>
	<table style="width:650px;border:2px solid">
	<tr><td class="text12pt" style="background-color:#99FF99;text-align:center;"><b>目前客戶的狀況</b></td></tr>
	<tr><td style="text-align:left;vertical-align:top">
			<asp:UpdatePanel ID="up_list" runat="server" UpdateMode="Conditional">
				<Triggers>
					<asp:AsyncPostBackTrigger ControlID="ti_Cs_User" EventName="Tick" />
				</Triggers>
				<ContentTemplate>
					<asp:Literal ID="lt_list" runat="server"></asp:Literal>
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
	</tr>
	</table>
	
	<asp:Timer ID="ti_Cs_User" runat="server" Interval="3000" ontick="ti_Cs_User_Tick"></asp:Timer>
	<asp:Label ID="lb_tb_title" runat="server" Visible="false"></asp:Label>
	<asp:Label ID="lb_Sql_Cs_User" runat="server" Visible="false"></asp:Label>
      </div>
</asp:Content>

