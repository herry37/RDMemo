<%@ Page Title="" Language="C#" MasterPageFile="Top2.master" AutoEventWireup="true" CodeFile="7001.aspx.cs" Inherits="Service_7001" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" runat="Server">
    <%--<link href="../css/bootstrap.css" rel="stylesheet" />--%>
    <style type="text/css">
        #mytable {
            border-collapse: collapse;
            border: solid 2px Black;
        }

            #mytable td {
                width: 80px;
                height: 30px;
                border: solid 1px Black;
                padding: 5px;
            }

        #content1 {
            margin: 0 auto;
            width: 35%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SiteDisp" runat="Server">


    <div id="content1">
       
        <p  style="margin: 10pt 0pt 5pt 0pt; text-align: center; font-family: 標楷體;font-size:26px">客戶使用端</p>
        <p  style="margin: 5pt 0pt 5pt 0pt; text-align: center; font-family: 標楷體;font-size:20px">提出客服要求</p>
        <asp:ScriptManager ID="sm_manager" runat="server"></asp:ScriptManager>
        <table style="width: 300px;" id="mytable">
            <tr style="height: 24px">
                <td style="background-color: #FFFFE0; text-align: center">使用者名稱</td>
                <td style="text-align: left">
                    <asp:TextBox ID="tb_cu_name" runat="server" MaxLength="20" Width="160px"></asp:TextBox></td>
            </tr>
            <tr style="height: 24px; background-color: #F0F8FF;">
                <td colspan="2" style="text-align: left">請輸入10個字以內的名稱，以方便客服人員稱呼。</td>
            </tr>
        </table>
        <table id="tab_wait" runat="server" visible="false" border="1" cellpadding="4" cellspacing="0" style="width: 300px; background-color: #FFFACD">
            <tr style="height: 24px">
                <td style="background-color: #FFFFE0; text-align: center">要求提出時間</td>
                <td>
                    <asp:Label ID="lb_cu_time" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td style="background-color: #FFFFE0; text-align: center">等待回應時間</td>
                <td>
                    <asp:UpdatePanel ID="up_wait" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ti_check" EventName="Tick"></asp:AsyncPostBackTrigger>
                        </Triggers>
                        <ContentTemplate>
                            <asp:Label ID="lb_wait_sec" runat="server" Text="1"></asp:Label>
                            sec
		                       		<asp:Literal ID="lt_msg" runat="server" EnableViewState="False"></asp:Literal>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr style="height: 24px; background-color: #F0F8FF; text-align: center">
                <td colspan="2">若超過等待時間，請重新提出要求</td>
            </tr>
        </table>

        <asp:Timer ID="ti_check" runat="server" Enabled="False" Interval="1100" OnTick="ti_check_Tick"></asp:Timer>
        <asp:Label ID="lb_cu_sid" runat="server" Text="0" Visible="false"></asp:Label>
        <asp:Label ID="lb_mg_sid" runat="server" Text="-1" Visible="false"></asp:Label>

        <br />
        <div style="text-align: center">

            <asp:Button type="button" ID="bn_ok" runat="server" class="btn btn-primary" OnClick="bn_ok_Click" Text="連絡客服" />
        </div>

    </div>
    <script src="../Scripts/jquery-2.2.1.js"></script>
    <script src="../js/bootstrap.js"></script>
</asp:Content>

