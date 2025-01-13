<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="MemberManage.aspx.cs" Inherits="MemberManage"  MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" 
        SelectCommand="SELECT * FROM [帳號使用權限] where 權限代碼  LIKE 'M'+'%'"></asp:SqlDataSource>
    <div style="color: #e14286; text-align: left; margin-top: 20px">
        <asp:Label ID="Label3" runat="server" Text="會員帳號管理" CssClass="h2"></asp:Label>
    </div>
    <br />
    <asp:Panel ID="Panel1" runat="server">
        <div class="container">
                <table style="text-align: center" class="table-condensed">
                    <tr>
                        <td style="width: 100px">&nbsp
                            <asp:Label ID="Label4" runat="server" Text="帳　　　　號"></asp:Label>
                        </td>
                        <td style="width: 100px">
                            <asp:Label ID="Label8" runat="server" Text="暱　　　　稱"></asp:Label>
                        </td>
                        <td style="width: 100px">
                            <asp:Label ID="Label6" runat="server" Text="狀　態"></asp:Label>
                        </td>
                        <td style="width: 70px"></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtAccount" runat="server" CssClass="input-sm"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtName" runat="server" CssClass="input-sm"></asp:TextBox>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlstatus" runat="server" CssClass="input-sm">
                                <asp:ListItem Selected="True" Value="0">全部</asp:ListItem>
                                <asp:ListItem Value="True">啟用</asp:ListItem>
                                <asp:ListItem Value="False">停用</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Button ID="Button1" runat="server" Text="查詢" CssClass="btn btn-primary" />
                        </td>
                    </tr>
                </table>
        <hr />
        </div>
        <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" OnSelected="SqlDataSource3_Selected"
            UpdateCommand="UPDATE [會員] SET [啟用] = @啟用, [停用日期] = @停用日期, [權限代碼] = @權限代碼 WHERE [帳號] = @帳號;UPDATE [商品] SET [停權] = @停權 WHERE [帳號] = @帳號"
            SelectCommand="SELECT 會員.帳號, [暱稱], 會員.權限代碼,[啟用], [停用日期],帳號使用權限.權限名稱  FROM [會員] left join 帳號使用權限 on 會員.權限代碼 =帳號使用權限.權限代碼 WHERE((會員.帳號 Like '%'+@帳號+'%')AND (暱稱 Like'%'+@暱稱+'%')AND((@啟用='0') OR (啟用=@啟用)))"
            DeleteCommand="DELETE FROM [會員] WHERE [帳號] = @帳號">
            <DeleteParameters>
                <asp:Parameter Name="帳號" Type="String" />
            </DeleteParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="txtAccount" Name="帳號" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
                <asp:ControlParameter ControlID="txtName" Name="暱稱" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
                <asp:ControlParameter ControlID="ddlstatus" Name="啟用" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="啟用" Type="Boolean" />
                <asp:Parameter Name="停用日期" Type="DateTime" />
                <asp:Parameter Name="暱稱" Type="String" />
                <asp:Parameter Name="帳號" Type="String" />
                <asp:Parameter Name="權限代碼" Type="String" />
                <asp:Parameter Name="停權" Type="Boolean" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <div>
            <asp:Label ID="Label12" runat="server" Text='' ForeColor="Green"></asp:Label>
        </div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="帳號" OnRowUpdating="GridView1_RowUpdating" AllowPaging="true" PageSize="15"
            DataSourceID="SqlDataSource3" CellPadding="4" ForeColor="#333333" OnRowCommand="GridView1_RowCommand" CssClass="table-bordered table-condensed" 
            OnDataBound="GridView1_DataBound" >
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField DataField="帳號" HeaderText="帳號" SortExpression="帳號" ReadOnly="true" ItemStyle-Width="100px" />
                <asp:BoundField DataField="暱稱" HeaderText="暱稱" SortExpression="暱稱" ReadOnly="true" ItemStyle-Width="140px" />
                <asp:TemplateField HeaderText="權限名稱" SortExpression="權限名稱" ItemStyle-Width="140px">
                    <EditItemTemplate>
                        <asp:DropDownList ID="DropDownList1" runat="server" AppendDataBoundItems="True" OnInit="DropDownList1_Init"
                            DataTextField="權限名稱" DataValueField="權限代碼" SelectedValue='<%# Bind("權限代碼") %>'>
                            <asp:ListItem Value="">請選擇</asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("權限名稱") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="啟用" SortExpression="啟用" ItemStyle-Width="70px">
                    <EditItemTemplate>
                        <asp:CheckBox ID="chkuse" runat="server" Checked='<%# Bind("啟用") %>' />
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Eval("啟用") %>' Enabled="false" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="功能" ItemStyle-Width="100">
                    <EditItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="更新"  CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:LinkButton>
                        &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消"  CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:LinkButton>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="修改" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:LinkButton>
                        &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Select" Text="查詢" OnClick="LinkButton2_Click"  CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerTemplate>
                <table style="width: 100%;">
                    <tr>
                        <td style="width: 70px"></td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="請選擇頁碼"></asp:Label>
                            <asp:LinkButton ID="Linkpage1" runat="server" Font-Names="Webdings" Font-Underline="false" OnClick="page_SelectedIndexChanged" ForeColor="White">3</asp:LinkButton>
                            <asp:DropDownList ID="ddlpage" AutoPostBack="true" runat="server" OnSelectedIndexChanged="page_SelectedIndexChanged" ForeColor="Black"></asp:DropDownList>
                            <asp:LinkButton ID="Linkpage2" runat="server" Font-Names="Webdings" Font-Underline="false" OnClick="page_SelectedIndexChanged" ForeColor="White">4</asp:LinkButton>
                        </td>
                        <td style="text-align: right">
                            <asp:Label ID="labNum" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                </table>
            </PagerTemplate>

            <EditRowStyle BackColor="#E7EFFA" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
            <PagerStyle BackColor="#507CD1" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
<%--            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />--%>
        </asp:GridView>
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server">
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
            SelectCommand="SELECT * FROM [會員] left join [帳號使用權限] on 會員.權限代碼=帳號使用權限.權限代碼 WHERE ([帳號] = @帳號)">
            <SelectParameters>
                <asp:ControlParameter ControlID="GridView1" Name="帳號" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataKeyNames="帳號" DataSourceID="SqlDataSource1"
            Width="604px" CellPadding="4" ForeColor="#333333" GridLines="None" HeaderText="會員資料" CssClass="table-border table-condensed">
            <%--   <AlternatingRowStyle BackColor="White" ForeColor="#284775" />--%>
            <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
            <EditRowStyle BackColor="#999999" />
            <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" />
            <Fields>
                <asp:BoundField DataField="帳號" HeaderText="帳號" ReadOnly="True" SortExpression="帳號" />
                <asp:BoundField DataField="密碼" HeaderText="密碼" SortExpression="密碼" Visible="false" />
                <asp:BoundField DataField="暱稱" HeaderText="暱稱" SortExpression="暱稱" />

                <asp:TemplateField HeaderText="性別" SortExpression="性別">
                    <ItemTemplate>
                        <asp:Label ID="Label10" runat="server" Text='<%# FormatSex((bool)Eval("性別")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="生日" HeaderText="生日" SortExpression="生日" DataFormatString="{0:D}" />
                <asp:BoundField DataField="email" HeaderText="email" SortExpression="email" />
                <asp:BoundField DataField="市話" HeaderText="市話" SortExpression="市話" />
                <asp:BoundField DataField="手機" HeaderText="手機" SortExpression="手機" />
                <asp:BoundField DataField="權限代碼" HeaderText="權限代碼" SortExpression="權限代碼" Visible="false" />
                <asp:BoundField DataField="權限名稱" HeaderText="權限名稱" SortExpression="權限名稱" />
                <asp:CheckBoxField DataField="認證" HeaderText="認證" SortExpression="認證" />
                <asp:CheckBoxField DataField="啟用" HeaderText="啟用" SortExpression="啟用" />
                <asp:BoundField DataField="其他" HeaderText="其他" SortExpression="其他" />
                <asp:BoundField DataField="註冊日期" HeaderText="註冊日期" SortExpression="註冊日期" DataFormatString="{0:D}" />
                <asp:BoundField DataField="啟用日期" HeaderText="啟用日期" SortExpression="啟用日期" DataFormatString="{0:D}" />
                <asp:BoundField DataField="停用日期" HeaderText="停用日期" SortExpression="停用日期" DataFormatString="{0:D}" />
                <asp:BoundField DataField="Validate" HeaderText="Validate" SortExpression="Validate" Visible="false" />
            </Fields>
            <FooterTemplate>
                <asp:LinkButton ID="LinkButton3" runat="server" OnClick="LinkButton3_Click">回上頁</asp:LinkButton>
            </FooterTemplate>
            <FooterStyle BackColor="#cdcd9c" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle" Wrap="True" Font-Underline="False" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Height="30" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        </asp:DetailsView>
    </asp:Panel>
    <br />
    <br />
</asp:Content>

