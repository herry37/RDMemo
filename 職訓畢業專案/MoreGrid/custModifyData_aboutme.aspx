<%@ Page Language="C#" MasterPageFile="Left.master" AutoEventWireup="true" CodeFile="custModifyData_aboutme.aspx.cs" Inherits="custModifyData_aboutme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" runat="Server">
       <style type="text/css">
        .table1 {
            border: 1px solid black;
            width: 100%;
        }
           .btn {
                align-items:center;
           }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="CContent" runat="Server">
     <asp:SqlDataSource ID="Sqlregistered" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" ProviderName="System.Data.SqlClient"
        UpdateCommand="update 會員 set 其他=@其他 where 帳號=@帳號">

        <UpdateParameters>
            <asp:Parameter Name="其他" Type="String" />
            <asp:Parameter Name="帳號" Type="String" />
        </UpdateParameters>
    </asp:SqlDataSource>
   <asp:DataList ID="DataList1" runat="server" Width="600px" DataKeyField="帳號" DataSourceID="Sqlregistered" RepeatColumns="1" RepeatDirection="Vertical" GridLines="Both" OnEditCommand="DataList1_EditCommand" OnCancelCommand="DataList1_CancelCommand" OnUpdateCommand="DataList1_UpdateCommand">

        <HeaderStyle BackColor="#3399ff" ForeColor="White" HorizontalAlign="Center" />
        <HeaderTemplate>
            關於我
        </HeaderTemplate>
       <EditItemStyle HorizontalAlign="Center" />
        <EditItemTemplate>
            <table class="table1">
                <tr>
                    <td>例如：<br />匯款帳戶<br />聯絡資訊<br />店家資訊<br />雜七雜八<br /></td>
                    <%--<td><asp:TextBox runat="server" ID="txtAboutme" Text='<%# Bind("其他") %>' TextMode="MultiLine" Rows="18" Columns="80"></asp:TextBox>--%>
                    <td><asp:TextBox runat="server" ID="txtAboutme" Text='<%# Bind("其他") %>' TextMode="MultiLine" Rows="15" Columns="70"></asp:TextBox>
                </tr>
            </table>
           
            <asp:Button ID="LinkButton1" runat="server" CommandName="Update" Text="儲存修改" Width="90px"></asp:Button>
            <asp:Button ID="LinkButton2" runat="server" CausesValidation="false" CommandName="Cancel" Text="取消" Width="90px"></asp:Button>

        </EditItemTemplate>

        <ItemStyle BorderWidth="3" BorderStyle="Solid" HorizontalAlign="Center" />
        <ItemTemplate>
            <table class="table1">
               
                <tr>
                    <td>關於我：</td>
                    <td><asp:Label runat="server" ID="txtAboutme" Text='<%# Eval("其他").ToString().Replace("\n","<br>") %>' Width="300px" Height="300"  ></asp:Label>
                </tr>

            </table>
            <asp:Button ID="LinkButton1" runat="server" CommandName="Edit" Text="修改" Width="90px"></asp:Button>
        </ItemTemplate>
    </asp:DataList>


</asp:Content>
