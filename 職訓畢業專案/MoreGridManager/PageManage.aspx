<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="PageManage.aspx.cs" Inherits="PageManage" 
    MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
        table{text-align:center;vertical-align:central}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
     <br /><br />
    <div>
        <div style="float: left;width:400px">
            <br />
            <asp:Label ID="Label2" runat="server" Text="請輸入主項標題"></asp:Label>
            <asp:TextBox ID="txtTitle" runat="server" AutoPostBack="false" MaxLength="20" placeholder=""></asp:TextBox>
<%--            <asp:Button ID="Button3" runat="server" Text="查詢" CausesValidation="false"  />--%>
            <asp:Button ID="Button1" runat="server" Text="新增" OnClick="Button1_Click" ValidationGroup="addTitle" />
            <div>
                <asp:Label ID="labmsg" runat="server" Text="" ForeColor="Red"></asp:Label>
            </div>
            <div>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtTitle" ErrorMessage="不能空白"
                     Display="Dynamic" ForeColor="red" ValidationGroup="addTitle"></asp:RequiredFieldValidator>
            </div>
            <hr />
                 <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                     DataKeyNames="流水號" DataSourceID="SqlDataSource1" OnDataBound="GridView1_DataBound">
                     <Columns>
                         <asp:BoundField DataField="流水號" HeaderText="流水號" InsertVisible="False" ReadOnly="True" SortExpression="流水號" />
                         <asp:BoundField DataField="主項標題" HeaderText="主項標題" SortExpression="主項標題" />
                         <asp:TemplateField ShowHeader="False">
                             <EditItemTemplate>
                                 <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="更新"></asp:LinkButton>
                                 &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消"></asp:LinkButton>
                             </EditItemTemplate>
                             <ItemTemplate>
                                 <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="編輯"></asp:LinkButton>
                                 &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Delete" Text="刪除" OnClientClick="return confirm('確定刪除嗎?')"></asp:LinkButton>
                             </ItemTemplate>
                         </asp:TemplateField>
                     </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" 
                SelectCommand="SELECT * FROM [頁面主項]" 
                DeleteCommand="DELETE FROM [頁面主項] WHERE [流水號] = @流水號" 
                UpdateCommand="UPDATE [頁面主項] SET [主項標題] = @主項標題 WHERE [流水號] = @流水號">
                <DeleteParameters>
                    <asp:Parameter Name="流水號" Type="Int32" />
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="主項標題" Type="String" />
                    <asp:Parameter Name="流水號" Type="Int32" />
                </UpdateParameters>
            </asp:SqlDataSource>
            <br />
        </div>
        <%-- 第二部分頁面細項 --%>
        <div style="float: right">
            <table>
                <tr>
                    <td></td>
                    <td style="width: 10px"></td>
                    <td style="width: 100px">
                        <asp:Label ID="Label3" runat="server" Text="請輸入細項標題"></asp:Label></td>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="請輸入細項網址"></asp:Label></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="ddl1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged"
                            DataSourceID="SqlDataSource1" DataTextField="主項標題" DataValueField="流水號" AppendDataBoundItems="True" ValidationGroup="adddetail">
                            <asp:ListItem Value="0">全部</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td style="width: 10px"></td>
                    <td>
                        <asp:TextBox ID="txtdetailName" runat="server" AutoPostBack="false" Width="110px" MaxLength="20" Text=""></asp:TextBox></td>

                    <td>
                        <asp:TextBox ID="txtlink" runat="server" AutoPostBack="false" Width="110px" MaxLength="20" Text=""></asp:TextBox></td>

                    <td>
<%--                        <asp:Button ID="Button4" runat="server" Text="查詢" OnClick="DropDownList1_SelectedIndexChanged" />--%>
                        <asp:Button ID="Button2" runat="server" Text="新增" OnClick="Button2_Click" ValidationGroup="adddetail" />
                    </td>
                </tr>
            </table>
            <div>
                <asp:Label ID="labArea" runat="server" Text="" ForeColor="Red"></asp:Label>
                <asp:Label ID="labtemp" runat="server" Text="" Visible="false"></asp:Label>

            </div>
            <div>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="ddl1" InitialValue="0" ErrorMessage="--要選擇主項--" Display="Dynamic" ForeColor="red" ValidationGroup="adddetail"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtdetailName" ErrorMessage="--細項不能空白--" Display="Dynamic" ForeColor="red" ValidationGroup="adddetail"></asp:RequiredFieldValidator>
            </div>
            <hr />
            <br />

            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataKeyNames="流水號" DataSourceID="SqlDataSource2">
                <Columns>
                    <asp:BoundField DataField="流水號" HeaderText="流水號" ReadOnly="True" SortExpression="流水號" InsertVisible="False" />
                    <asp:TemplateField HeaderText="主項標題" SortExpression="主項標題">
                        <EditItemTemplate>
                            <asp:DropDownList ID="DropDownList1" runat="server" OnInit="DropDownList1_Init" 
                                DataTextField="主項標題" DataValueField="主項流水號" SelectedValue='<%# Bind("主項流水號") %>'></asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("主項標題") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="細項標題" HeaderText="細項標題" SortExpression="細項標題" />
                    <asp:BoundField DataField="網址" HeaderText="網址" SortExpression="網址" />
                    <asp:TemplateField ShowHeader="False">
                        <EditItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="更新"></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消"></asp:LinkButton>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="編輯"></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Delete" Text="刪除" OnClientClick="return confirm('確定刪除嗎?')"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" 
                SelectCommand="SELECT 頁面細項.*,頁面主項.主項標題 FROM [頁面細項] left join 頁面主項 on 頁面細項.主項流水號=頁面主項.流水號" 
                DeleteCommand="DELETE FROM [頁面細項] WHERE [流水號] = @流水號" 
                UpdateCommand="UPDATE [頁面細項] SET [主項流水號] = @主項流水號, [細項標題] = @細項標題, [網址] = @網址 WHERE [流水號] = @流水號">
                <DeleteParameters>
                    <asp:Parameter Name="流水號" Type="Int32" />
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="主項流水號" Type="Int32" />
                    <asp:Parameter Name="細項標題" Type="String" />
                    <asp:Parameter Name="網址" Type="String" />
                    <asp:Parameter Name="流水號" Type="Int32" />
                </UpdateParameters>
            </asp:SqlDataSource>
        </div>
    <br /><br />
</asp:Content>
