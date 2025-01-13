<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="CompanyManage.aspx.cs" Inherits="CompanyManage" 
    MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
        table{text-align:center;vertical-align:central;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <div style="color: #e14286; text-align: left; margin-top: 20px">
            <asp:Label ID="Label3" runat="server" Text="內部資料設定" CssClass="h2"></asp:Label>
        </div>

        <div style="float: left">
            <br />
            <asp:Label ID="Label2" runat="server" Text="請輸入部門名稱"></asp:Label>
            <asp:TextBox ID="txtDepName" runat="server" AutoPostBack="false" MaxLength="20" Width="140" CssClass="input-sm"></asp:TextBox>
            <asp:Button ID="Button3" runat="server" Text="查詢" CausesValidation="false" CssClass="btn btn-primary" />
            <asp:Button ID="Button1" runat="server" Text="新增" OnClick="Button1_Click" ValidationGroup="Depname" CssClass="btn btn-success" />
            <div>
                <asp:Label ID="labdep" runat="server" Text="" ForeColor="Red"></asp:Label>
            </div>
            <div>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDepName" ErrorMessage="部門名稱不能空白"
                    Display="Dynamic" ForeColor="red" ValidationGroup="Depname"></asp:RequiredFieldValidator>
            </div>
            <hr />
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                DeleteCommand="DELETE FROM [部門] WHERE [部門代號] = @部門代號"
                SelectCommand="SELECT * FROM [部門] WHERE (部門名稱 LIKE '%'+@部門名稱+'%')"
                UpdateCommand="UPDATE [部門] SET [部門名稱] = @部門名稱 WHERE [部門代號] = @部門代號">
                <DeleteParameters>
                    <asp:Parameter Name="部門代號" Type="Int32" />
                </DeleteParameters>
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtDepName" Name="部門名稱" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="部門名稱" Type="String" />
                    <asp:Parameter Name="部門代號" Type="Int32" />
                </UpdateParameters>
            </asp:SqlDataSource>
            <br />
            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" CellPadding="4" PageSize="15"
                DataKeyNames="部門代號" DataSourceID="SqlDataSource1" ForeColor="#333333" OnRowCommand="GridView1_RowCommand"
                OnRowDataBound="GridView1_RowDataBound" CssClass="table-bordered table-condensed" OnRowCreated="GridView1_RowCreated">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="部門代號" HeaderText="部門代號" InsertVisible="False" ReadOnly="True" Visible="false" SortExpression="部門代號" />
                    <asp:TemplateField HeaderText="序號" ItemStyle-HorizontalAlign="Center">
                        <ItemStyle Width="70" CssClass="test" />
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#(Container.DataItemIndex+1).ToString("#0") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="部門名稱" HeaderText="部門名稱" SortExpression="部門名稱" ItemStyle-Width="140"></asp:BoundField>
                    <asp:TemplateField HeaderText="功能" ShowHeader="False">
                        <EditItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="更新"></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消"></asp:LinkButton>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="編輯"></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="btn_del" runat="server" CausesValidation="False" CommandName="Delete" Text="刪除" OnClientClick="return confirm('你確定嗎?')"></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="120px" />
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            </asp:GridView>
        </div>
        <%--職稱管理--%>
        <div style="float: right">
            <br />
            <asp:Label ID="Labe1" runat="server" Text="請輸入職稱"></asp:Label>
            <asp:TextBox ID="txtEmpName" runat="server" AutoPostBack="false" MaxLength="20" Width="140" CssClass=" input-sm"></asp:TextBox>
            <asp:Button ID="Button2" runat="server" Text="查詢" CausesValidation="false" CssClass="btn btn-primary" />
            <asp:Button ID="Button4" runat="server" Text="新增" OnClick="Button2_Click" ValidationGroup="Empname" CssClass="btn btn-success" />
            <div>
                <asp:Label ID="labemp" runat="server" Text="" ForeColor="Red"></asp:Label>
            </div>
            <div>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtEmpName" ErrorMessage="職稱不能空白"
                    Display="Dynamic" ForeColor="red" ValidationGroup="Empname"></asp:RequiredFieldValidator>
            </div>
            <hr />
            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                DeleteCommand="DELETE FROM [職稱] WHERE [職稱代號] = @職稱代號"
                SelectCommand="SELECT * FROM [職稱] WHERE (職稱名稱 LIKE '%'+@職稱名稱+'%')"
                UpdateCommand="UPDATE [職稱] SET [職稱名稱] = @職稱名稱 WHERE [職稱代號] = @職稱代號">
                <DeleteParameters>
                    <asp:Parameter Name="職稱代號" Type="Int32" />
                </DeleteParameters>
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtEmpName" Name="職稱名稱" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="職稱名稱" Type="String" />
                    <asp:Parameter Name="職稱代號" Type="Int32" />
                </UpdateParameters>
            </asp:SqlDataSource>
            <br />
            <asp:GridView ID="GridView2" runat="server" AllowPaging="True" AutoGenerateColumns="False" CellPadding="4" PageSize="15"
                DataKeyNames="職稱代號" DataSourceID="SqlDataSource2" ForeColor="#333333" OnRowCommand="GridView2_RowCommand"
                OnRowDataBound="GridView2_RowDataBound" CssClass="table-bordered table-condensed" OnRowCreated="GridView2_RowCreated">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="職稱代號" HeaderText="職稱代號" InsertVisible="False" ReadOnly="True" Visible="false" SortExpression="職稱代號" />
                    <asp:TemplateField HeaderText="序號" ItemStyle-HorizontalAlign="Center">
                        <ItemStyle Width="70" />
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#(Container.DataItemIndex+1).ToString("#0") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="職稱名稱" HeaderText="職稱" SortExpression="職稱名稱" ItemStyle-Width="140">
                        <ItemStyle Width="140px"></ItemStyle>
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="功能" ShowHeader="False">
                        <EditItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="更新"></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消"></asp:LinkButton>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="編輯"></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="btn_del" runat="server" CausesValidation="False" CommandName="Delete" Text="刪除" OnClientClick="return confirm('確定刪除?')"></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="120px" />
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            </asp:GridView>

        </div>
    </div>
    <br />
    <br />

</asp:Content>

