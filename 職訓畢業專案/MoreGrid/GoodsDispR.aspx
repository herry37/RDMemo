<%@ Page Title="" Language="C#" MasterPageFile="Top2.master" AutoEventWireup="true" CodeFile="GoodsDispR.aspx.cs" Inherits="GoodsDispR" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" Runat="Server">
    <link href="Mystyle/StyleMessageBoard.css" rel="stylesheet" />
     <script>
        $(document).ready(function () {
            var maxChar = 250;
            $('#txt_areal').text(maxChar);
            $('#left_SiteDisp_txtGcontent').keydown(function (event) {
                var msg = $(this).val().length;
                $('#txt_areal').text(maxChar - msg);
                //if (maxChar - msg == 0) {
                //    event.preventDefault();
                //}
            });
            });
    
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SiteDisp" Runat="Server">
    <%--回覆留言--%>

           <div>
            <asp:Panel ID="Panelmessage" runat="server">
                &nbsp 回覆問題：(為避免個人資料遭有人士利用，請勿在提問內容填寫個人相關資料，如:姓名、銀行帳戶..等。)        
            </asp:Panel>
            <asp:Panel ID="Panelmessage1" runat="server">
                <br />
                <br /> 
                &nbsp &nbsp &nbsp &nbsp<label id="txt_area">您可以輸入&nbsp<span id="txt_areal"></span>字元</label><br />
                <asp:Label runat="server" Font-Size="24px" ForeColor="Red" Font-Bold="true" ID="errMessage"></asp:Label><br />
                <table id="add_book">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtGcontent" runat="server" TextMode="MultiLine" Rows="15" Columns="50"></asp:TextBox>
                  <asp:RegularExpressionValidator ControlToValidate="txtGcontent" runat="server" ErrorMessage="只能輸入250字數" Font-Bold="true" ForeColor="Red" ValidationExpression=".{1,250}"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="ButMessage" runat="server" Text="提出問題" OnClick="ButMessage_Click" />
                            <asp:Button ID="ButMessageDel" runat="server" Text="重新填寫" CausesValidation="false" OnClick="ButMessageDel_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
</asp:Content>

