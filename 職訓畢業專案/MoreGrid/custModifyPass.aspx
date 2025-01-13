<%@ Page Title="" Language="C#" MasterPageFile="Left.master" AutoEventWireup="true" CodeFile="custModifyPass.aspx.cs" Inherits="custModifyPass" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentCSS" Runat="Server">

    <style type="text/css">
        .join-container {
            width: 600px;
            margin: auto;
            margin-top: 50px;
        }

        .header {
            padding: 5px;
            text-align: center;
            background-color: #222;
            color: #fff;
            height: 30px;
        }

        .table {
            margin: auto;
        }
        .table tr td:nth-child(1){
           width:30%;
           text-align:right;
           padding-right:20px;
       }
       .table tr td:nth-child(2){
           width:70%;
           text-align:left;
       }

        .TextBox {
            height: 25px;
        }
    </style>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="CContent" Runat="Server">
        <asp:SqlDataSource ID="Sqlregistered" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" ProviderName="System.Data.SqlClient"
        UpdateCommand="update 會員 set 密碼=@密碼 where 帳號=@帳號">

        <UpdateParameters>
            <asp:Parameter Name="密碼" Type="String" />
            <asp:Parameter Name="帳號" Type="String" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <div class="join-container">

    <asp:DataList ID="DataList1" runat="server" Width="600px" DataKeyField="帳號" DataSourceID="Sqlregistered" RepeatColumns="1" RepeatDirection="Vertical" OnEditCommand="DataList1_EditCommand" OnCancelCommand="DataList1_CancelCommand" OnUpdateCommand="DataList1_UpdateCommand">

        <HeaderStyle CssClass="header" HorizontalAlign="Center" />
        <HeaderTemplate>
            會員密碼修改
        </HeaderTemplate>
        <EditItemTemplate>
            <table class="table">
               
                <tr>
                    <td>舊密碼：</td>
                    <td>
                        <asp:Label ID="txtNo" runat="server" Text='<%#Eval("帳號") %>' Visible="false"></asp:Label>
                        <asp:TextBox ID="txtOldPsd" CssClass="TextBox input-sm" runat="server" TextMode="Password" placeholder="長度6-20英數字"></asp:TextBox>
                        <asp:Label ID="LabeltxtOldPsdok" runat="server" ForeColor="Green" Visible="false" > &radic;</asp:Label>
                        <asp:Label ID="LabeltxtOldPsdgg" runat="server" ForeColor="Red" Visible="false"> &chi;</asp:Label>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtOldPsd" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtOldPsd" ErrorMessage="請檢查密碼格式" Text="*(密碼長度6-20)" ForeColor="red" Display="Dynamic" ValidationExpression="\w{6,20}"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td>新密碼：</td>
                    <td>
                        <asp:TextBox ID="txtPsd" CssClass="TextBox input-sm" runat="server" TextMode="Password" placeholder="長度6-20英數字"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtPsd" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="txtPsd" ErrorMessage="請檢查密碼格式" Text="*(密碼長度6-20)" ForeColor="red" Display="Dynamic" ValidationExpression="\w{6,20}"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td>確認密碼：</td>
                    <td>
                        <asp:TextBox ID="txtPsd2" CssClass="TextBox input-sm" runat="server" TextMode="Password" placeholder="長度6-20英數字"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtPsd2" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtPsd" ControlToValidate="txtPsd2" ErrorMessage="請檢查是否相同" ForeColor="Red"></asp:CompareValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtPsd2" ErrorMessage="請檢查密碼格式" Text="*(密碼長度6-20)" ForeColor="red" Display="Dynamic" ValidationExpression="\w{6,20}"></asp:RegularExpressionValidator>
                    </td>
                </tr>
            </table>
            <div style="margin-top:35px;">
                <div class="col-lg-offset-2 col-lg-2">
                    <asp:Button ID="LinkButton1" runat="server" CommandName="Update" Text="儲存修改" CssClass="btn btn-info"></asp:Button>
                </div>
                <div class="col-lg-offset-3 col-lg-2">
                    <asp:Button ID="LinkButton2" runat="server" CausesValidation="false" CommandName="Cancel" Text="取消" CssClass="btn btn-warning"></asp:Button>
                </div>
            </div>

            
            


        </EditItemTemplate>
        
        <ItemTemplate>
            <table class="table">
                <tr>
                    <td>帳號：</td>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text='<%#Eval("帳號") %>'></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td>密碼：</td>
                    <td><%#Eval("密碼").ToString().Substring(0,3)%>*****<%#Eval("密碼").ToString().Substring(Eval("密碼").ToString().Length-1,1)%></td>
                </tr>

            </table>
            <div style="margin-top:35px;">
                <div class="col-lg-offset-2 col-lg-2">
                    <asp:Button ID="LinkButton1" runat="server" CommandName="Edit" Text="修改密碼" CssClass="btn btn-info"></asp:Button>
                </div>
                <div class="col-lg-offset-3 col-lg-2">
                    <asp:Button ID="Button1" runat="server" Text="取消" OnClick="Button1_Click"  CssClass="btn btn-warning"></asp:Button>
                </div>                            
            </div>
            
        </ItemTemplate>
    </asp:DataList>
    </div>
</asp:Content>

