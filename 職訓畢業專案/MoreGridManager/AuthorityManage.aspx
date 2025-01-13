<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="AuthorityManage.aspx.cs" Inherits="AuthorityManage" MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
       table{text-align:center}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div style="">
        <div style="color: #e14286; text-align: left; margin-top: 20px">
            <asp:Label ID="Label1" runat="server" Text="權限類別管理" CssClass="h2"></asp:Label>
        </div>
        <br />
        <div>
            <asp:Label ID="Label2" runat="server" Text="權限代碼"></asp:Label>
            <asp:TextBox ID="txtno" runat="server" AutoPostBack="false" MaxLength="10" Width="100" Text="" CssClass="input-sm"></asp:TextBox>
            &nbsp&nbsp<asp:Label ID="Label3" runat="server" Text="權限名稱"></asp:Label>
            <asp:TextBox ID="txtname" runat="server" AutoPostBack="false" MaxLength="20" Width="100" Text="" CssClass="input-sm"></asp:TextBox>
            &nbsp&nbsp<asp:Button ID="Button3" runat="server" Text="查詢" CausesValidation="false" CssClass="btn btn-primary" />&nbsp&nbsp
            <asp:Button ID="Button1" runat="server" Text="新增" OnClick="Button1_Click" CssClass="btn btn-success" ValidationGroup="authority" />
        </div>
        <div style="margin-left: 100px">
            <asp:Label ID="labmsg" runat="server" Text="" ForeColor="Red"></asp:Label>
        </div>
        <div>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtno" ErrorMessage="權限代碼不能空白"
                Display="Dynamic" ForeColor="red" ValidationGroup="authority"></asp:RequiredFieldValidator>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtname" ErrorMessage="權限名稱不能空白"
                Display="Dynamic" ForeColor="red" ValidationGroup="authority"></asp:RequiredFieldValidator>
        </div>
        <hr />
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
            DeleteCommand="DELETE FROM [權限範圍] WHERE [權限代碼] = @權限代碼;DELETE FROM [帳號使用權限] WHERE [權限代碼] = @權限代碼"
            SelectCommand="SELECT * FROM [帳號使用權限] WHERE (([權限代碼] LIKE '%'+@權限代碼+'%') AND ([權限名稱] LIKE '%'+ @權限名稱+'%'))"
            UpdateCommand="UPDATE [帳號使用權限] SET [權限名稱] = @權限名稱 WHERE [權限代碼] = @權限代碼">
            <DeleteParameters>
                <asp:Parameter Name="權限代碼" Type="String" />
            </DeleteParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="txtno" Name="權限代碼" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
                <asp:ControlParameter ControlID="txtname" Name="權限名稱" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="權限名稱" Type="String" />
                <asp:Parameter Name="權限代碼" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <label style="color:red">員工帳號的權限代碼預設為01</label>
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" CellPadding="4" PageSize="15" OnRowCreated="GridView1_RowCreated"
            DataKeyNames="權限代碼" DataSourceID="SqlDataSource1" ForeColor="Black" GridLines="None" OnRowCommand="GridView1_RowCommand"
            OnRowDataBound="GridView1_RowDataBound" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" CssClass="table-bordered table-condensed">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField DataField="權限代碼" HeaderText="權限代碼" ReadOnly="True" SortExpression="權限代碼" ItemStyle-Width="100"></asp:BoundField>
                <asp:BoundField DataField="權限名稱" HeaderText="權限名稱" SortExpression="權限名稱" ItemStyle-Width="170"></asp:BoundField>
                <asp:TemplateField ShowHeader="False" HeaderText="功能" ItemStyle-Width="100">
                    <EditItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="更新"></asp:LinkButton>
                        &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消" ></asp:LinkButton>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="編輯" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:LinkButton>
                        &nbsp;<asp:LinkButton ID="btn_del" runat="server" CausesValidation="False" CommandName="Delete" Text="刪除" OnClientClick="return confirm('確定刪除嗎?')" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:HyperLinkField DataNavigateUrlFields="權限代碼,權限名稱" DataNavigateUrlFormatString="AuthorityRange.aspx?權限代碼={0}&權限名稱={1}" HeaderText="權限範圍" Text="權限" />
            </Columns>
            <FooterStyle BackColor="#CCCC99" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#6B696B" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F7DE" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#FBFBF2" />
            <SortedAscendingHeaderStyle BackColor="#848384" />
            <SortedDescendingCellStyle BackColor="#EAEAD3" />
            <SortedDescendingHeaderStyle BackColor="#575357" />
        </asp:GridView>
    </div>
</asp:Content>

