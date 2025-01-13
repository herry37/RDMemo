<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="GoodsClass.aspx.cs" Inherits="GoodsClass" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
     
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div>
        <div style="">
            <div style="color: #e14286; text-align: left; margin-top: 20px">
                <asp:Label ID="Label3" runat="server" Text="商品類別管理" CssClass="h2"></asp:Label>
            </div>
            <br />
            <asp:Label ID="Label2" runat="server" Text="請輸入商品類別名稱"></asp:Label>
            <asp:TextBox ID="txtGoodsName" runat="server" AutoPostBack="false" MaxLength="30" Width="140" Text="" CssClass="input-sm"></asp:TextBox>
            <asp:Button ID="Button3" runat="server" Text="查詢" CausesValidation="false" CssClass="btn btn-primary" />
            <asp:Button ID="Button1" runat="server" Text="新增" OnClick="Button1_Click" CssClass="btn btn-success" ValidationGroup="GoodsName" />
            <div>
                <asp:Label ID="labgoods" runat="server" Text="" ForeColor="Red"></asp:Label>
            </div>
            <div>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtGoodsName" ErrorMessage="商品類別名稱不能空白"
                  Display="Dynamic" ForeColor="red" ValidationGroup="GoodsName"></asp:RequiredFieldValidator>
           </div>

            <hr />
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                DeleteCommand="DELETE FROM [商品類別] WHERE [商品類別代碼] = @商品類別代碼"
                SelectCommand="SELECT * FROM [商品類別] WHERE ((@商品類別名稱='') OR ([商品類別名稱] LIKE '%'+ @商品類別名稱+'%'))"
                UpdateCommand="UPDATE [商品類別] SET [商品類別名稱] = @商品類別名稱 WHERE [商品類別代碼] = @商品類別代碼">
                <DeleteParameters>
                    <asp:Parameter Name="商品類別代碼" Type="Int32" />
                </DeleteParameters>
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtGoodsName" Name="商品類別名稱" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="商品類別名稱" Type="String" />
                    <asp:Parameter Name="商品類別代碼" Type="Int32" />
                </UpdateParameters>
            </asp:SqlDataSource>
            <br />
            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" CellPadding="4" PageSize="15"
                DataKeyNames="商品類別代碼" DataSourceID="SqlDataSource1" ForeColor="#333333" OnRowCommand="GridView1_RowCommand"
                OnRowDataBound="GridView1_RowDataBound" CssClass="table-bordered table-condensed" OnRowCreated="GridView1_RowCreated">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
<%--                    <asp:TemplateField Visible="false">
                        <ItemTemplate>
                            <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Eval("商品類別代碼") %>'/>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:BoundField DataField="商品類別代碼" HeaderText="商品類別代碼" InsertVisible="False" ReadOnly="True" SortExpression="商品類別代碼" Visible="false" />
                    <asp:TemplateField HeaderText="序號" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50px">
                        <ItemStyle />
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#(Container.DataItemIndex+1).ToString("#0") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="商品類別名稱" SortExpression="商品類別名稱">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditname" runat="server" Text='<%# Bind("商品類別名稱") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEditname" ErrorMessage="不能空白"
                                Display="Dynamic" ForeColor="red" ></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="labname" runat="server" Text='<%# Bind("商品類別名稱") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="240px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="功能" ItemStyle-Width="100px">
                        <EditItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="更新" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消"></asp:LinkButton>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="編輯"></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="btn_del" runat="server" CausesValidation="False" CommandName="Delete" Text="刪除" 
                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" OnClientClick="return confirm('確定刪除嗎?')"></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="100px"></ItemStyle>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            </asp:GridView>
            <br /><br />
        </div>
    </div>
</asp:Content>

