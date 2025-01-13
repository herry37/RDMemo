<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="login_OLD .aspx.cs" Inherits="login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
        table{text-align:center;width:auto;height:30px}
        table td:first-child{text-align:right;width:100px;}
        table td:last-child{text-align:center;width:100px}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<br /><br />
    <div class="container">
        <div style="margin-left:170px">
            <table style="width: 400px" class="well table-condensed">
                <tr class="label-primary" style="color:white">
                        <th colspan="3">
                            <asp:Label ID="Label12" runat="server" Text="員工帳號登入"></asp:Label>
                        </th>
                    </tr>
                <tr><td></td></tr>
                <tr>
                    <td>
                        <label>帳號&nbsp&nbsp</label></td>
                    <td>
                        <asp:TextBox ID="txtAccount" runat="server" CssClass="input-sm"></asp:TextBox></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <label>密碼&nbsp&nbsp</label></td>
                    <td>
                        <asp:TextBox ID="txtpwd" runat="server" TextMode="Password"  CssClass="input-sm"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="Sign In" class="btn btn-primary" OnClick="Button1_Click"/>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
<%--                        <asp:LinkButton ID="LinkButton1" runat="server" data-toggle="collapse" data-target="#myModal">忘記密碼</asp:LinkButton>--%>
                        <input id="btn1" type="button" value="忘記密碼" data-toggle="collapse" data-target="#myModal" class="btn btn-link"/>
                        &nbsp&nbsp<asp:HyperLink ID="HyperLink1" NavigateUrl="EmployAccount.aspx" runat="server">申請帳號</asp:HyperLink>
                    </td>
                    <td></td>
                </tr>
            </table>
            <div class="modal" id="myModal">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <%--<button class="close" data-dismiss="modal">&times;</button>--%>
                            <h3 class="modal-title"><asp:Label ID="Label4" runat="server" Text="忘記密碼"></asp:Label></h3>
                        </div>
                        <div class="modal-body">
                            <h3><asp:Label ID="Label5" runat="server" Text="輸入e-mail"></asp:Label></h3>
                            <asp:TextBox ID="txtEmail" runat="server" Width="250"   ></asp:TextBox>
                             <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ErrorMessage="(請檢查格式)" ValidationExpression="\w+([-+.']\w+)*@[A-Za-z]+([-.][A-Za-z]+)*\.[A-Za-z]+([-.][A-Za-z]+)*" ForeColor="Red" Display="Dynamic" ControlToValidate="txtEmail"></asp:RegularExpressionValidator>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSubmitEmil" runat="server" Text="送出" CssClass="btn btn-default" OnClick="btnSubmitEmil_Click" />
                            <asp:Button ID="Button2" runat="server" Text="取消" CssClass="btn btn-default" data-dismiss="modal"  CausesValidation="false" OnClick="Button2_Click"/>
                                                       
                        </div>
                    </div>
                </div>
            </div>



            <div>
                <asp:Label ID="labmsg" runat="server" Text="" ForeColor="Red"></asp:Label>
            </div>
        </div>
    </div>
    
</asp:Content>

