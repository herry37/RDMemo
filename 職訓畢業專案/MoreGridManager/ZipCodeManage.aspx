<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="ZipCodeManage.aspx.cs" Inherits="ZipCodeManage" 
    MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
        table{text-align:center;vertical-align:central}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server"><ContentTemplate>
    <div>
        <div style="color: #e14286; text-align: left; margin-top: 20px">
            <asp:Label ID="Label6" runat="server" Text="縣市代碼管理" CssClass="h2"></asp:Label>
        </div>
        <br />
        <div style="float: left">
            <br /><br />
            <asp:Label ID="Label2" runat="server" Text="請輸入縣市"></asp:Label>
            <asp:TextBox ID="txtCity" runat="server" AutoPostBack="false" MaxLength="10" placeholder="全部" CssClass="input-sm"></asp:TextBox>
            <asp:Button ID="Button3" runat="server" Text="查詢" CausesValidation="false" OnClick="Button3_Click" CssClass="btn btn-primary"/>
            <asp:Button ID="Button1" runat="server" Text="新增" OnClick="Button1_Click" ValidationGroup="addCity" CssClass="btn btn-success" />
            <div>
                <asp:Label ID="labCity" runat="server" Text="" ForeColor="Red"></asp:Label>
            </div>
            <div>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtCity" ErrorMessage="縣市不能空白"
                     Display="Dynamic" ForeColor="red" ValidationGroup="addCity"></asp:RequiredFieldValidator>
            </div>
            <hr />
            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" CellPadding="4"
                 DataKeyNames="縣市代碼" DataSourceID="SqlDataSource1" ForeColor="#333333" PageSize="15" OnRowCommand="GridView1_RowCommand"
                 EmptyDataText="查無資料" OnRowDataBound="GridView1_RowDataBound" HorizontalAlign="Center" OnDataBound="GridView1_DataBound"
                  CssClass="table-bordered table-condensed" OnRowCreated="GridView1_RowCreated">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="縣市代碼" HeaderText="縣市代碼" InsertVisible="False" ReadOnly="True" Visible="false" />
                    <asp:TemplateField HeaderText="序號" ItemStyle-HorizontalAlign="Center">
                        <ItemStyle Width="50" CssClass="test"/>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#(Container.DataItemIndex+1).ToString("#0") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="縣市" HeaderText="縣市" SortExpression="縣市" ItemStyle-Width="140" >
                    </asp:BoundField>
                    <asp:CheckBoxField DataField="顯示" HeaderText="使用" ItemStyle-Width="40">
                    </asp:CheckBoxField>
                    <asp:TemplateField HeaderText="功能" ShowHeader="False">
                        <EditItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="更新"></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消"></asp:LinkButton>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="編輯"></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="btn_del" runat="server" CausesValidation="False" CommandName="Delete" Text="刪除" OnClientClick="return confirm('確定刪除嗎?')"></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="90px" />
                    </asp:TemplateField>
                </Columns>
<%--                <EditRowStyle BackColor="#2461BF" />--%>
                <EmptyDataRowStyle BackColor="#ffff00" Width="300" />
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#EFF3FB" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            </asp:GridView>
                 <%--SelectCommand="SELECT * FROM [縣市] WHERE ((@縣市='') OR (縣市 LIKE '%'+@縣市+'%'))">--%>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                DeleteCommand="DELETE FROM [縣市] WHERE [縣市代碼] = @縣市代碼"
                UpdateCommand="UPDATE [縣市] SET [縣市] = @縣市,[顯示]=@顯示 WHERE [縣市代碼] = @縣市代碼">
               <SelectParameters>
                    <asp:ControlParameter ControlID="txtCity" Name="縣市" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
                </SelectParameters>
                <DeleteParameters>
                    <asp:Parameter Name="縣市代碼" Type="Int32" />
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="縣市" Type="String" />
                    <asp:Parameter Name="顯示" Type="Boolean" />
                    <asp:Parameter Name="縣市代碼" Type="Int32" />
                </UpdateParameters>
            </asp:SqlDataSource>
            <hr />
        </div>
        
        <%-- 第二部分郵遞區號 --%>
        <div style="float: right">
            <table style="text-align:center" class="table-condensed">
                    <tr>
                        <td style="width: 70px">&nbsp
                        </td>
                        <td style="width: 110px">
                        <asp:Label ID="Label4" runat="server" Text="請輸入郵遞區號"></asp:Label></td>
                        <td style="width: 110px">
                        <asp:Label ID="Label3" runat="server" Text="請輸入鄉鎮市"></asp:Label></td>
                        <td ></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="ddl1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" CssClass="input-sm"
                                DataSourceID="SqlDataSource3" DataTextField="縣市" DataValueField="縣市代碼" AppendDataBoundItems="true" ValidationGroup="addArea">
                                <asp:ListItem Value="0">全部</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                        <asp:TextBox ID="txtAreaNo" runat="server" AutoPostBack="false" Width="110px" Text="" CssClass="input-sm"></asp:TextBox></td>
                        <td>
                        <asp:TextBox ID="txtAreaName" runat="server" AutoPostBack="false" Width="110px" MaxLength="20" Text="" CssClass="input-sm"></asp:TextBox></td>
                        <td>
                        <asp:Button ID="Button4" runat="server" Text="查詢" OnClick="DropDownList1_SelectedIndexChanged" CssClass="btn btn-primary" />
                        <asp:Button ID="Button2" runat="server" Text="新增" OnClick="Button2_Click" ValidationGroup="addArea" CssClass="btn btn-success" />
                        </td>
                    </tr>
                </table>
          <%--  <table>
                <tr>
                    <td></td>
                    <td style="width: 10px"></td>
                    <td style="width: 100px">
                        <asp:Label ID="Label4" runat="server" Text="請輸入郵遞區號"></asp:Label></td>
                    <td style="width: 10px"></td>
                    <td style="width: 100px">
                        <asp:Label ID="Label3" runat="server" Text="請輸入鄉鎮市"></asp:Label></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="ddl1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged"
                            DataSourceID="SqlDataSource3" DataTextField="縣市" DataValueField="縣市代碼" AppendDataBoundItems="true" ValidationGroup="addArea">
                            <asp:ListItem Value="0">全部</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td></td>
                    <td>
                        <asp:TextBox ID="txtAreaNo" runat="server" AutoPostBack="false" Width="110px" Text=""></asp:TextBox></td>
                    <td></td>
                    <td>
                        <asp:TextBox ID="txtAreaName" runat="server" AutoPostBack="false" Width="110px" MaxLength="20" Text=""></asp:TextBox></td>
                    <td style="width: 10px"></td>
                    <td>
                        <asp:Button ID="Button4" runat="server" Text="查詢" OnClick="DropDownList1_SelectedIndexChanged" CssClass="btn btn-primary" />
                        <asp:Button ID="Button2" runat="server" Text="新增" OnClick="Button2_Click" ValidationGroup="addArea" CssClass="btn btn-success" />
                    </td>
                </tr>
            </table>--%>
            <div>
                <asp:Label ID="labArea" runat="server" Text="" ForeColor="Red"></asp:Label>
                <asp:Label ID="labtemp" runat="server" Text="" Visible="false"></asp:Label>
            </div>
            <div>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="ddl1" InitialValue="0" ErrorMessage="--要選擇縣市--" Display="Dynamic" ForeColor="red" ValidationGroup="addArea"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtAreaNo" ErrorMessage="--郵遞區號不能空白--" Display="Dynamic" ForeColor="red" ValidationGroup="addArea"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtAreaName" ErrorMessage="--鄉鎮市區不能空白--" Display="Dynamic" ForeColor="red" ValidationGroup="addArea"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ValidationExpression="\d+" ControlToValidate="txtAreaNo" ErrorMessage="郵遞區號需為數字" Display="Dynamic" ForeColor="red" ValidationGroup="addArea"></asp:RegularExpressionValidator>
            </div>
            <hr />
            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataKeyNames="郵遞代碼"
                DataSourceID="SqlDataSource4" BackColor="White"  CellPadding="3" PageSize="15"
                EmptyDataText="查無資料" OnRowDataBound="GridView2_RowDataBound" AllowPaging="True" OnRowCreated="GridView2_RowCreated"
                OnDataBound="GridView2_DataBound" OnRowCommand="GridView2_RowCommand" CssClass="table-bordered table-condensed">
                <AlternatingRowStyle BackColor="#F7F7F7" />
                <Columns>
                    <asp:BoundField DataField="郵遞區號" HeaderText="郵遞區號" ItemStyle-Width="70" SortExpression="郵遞區號">
                    </asp:BoundField>
<%--                <asp:BoundField DataField="縣市代碼" HeaderText="縣市代碼" Visible="false"/>--%>
                    <asp:TemplateField HeaderText="縣市" SortExpression="縣市">
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlcity" runat="server"  DataTextField="縣市" DataValueField="縣市代碼" SelectedValue='<%# Bind("縣市代碼") %>' OnInit="ddlcity_Init">
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("縣市") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle Width="140px" />
                        
                    </asp:TemplateField>
                    <asp:BoundField DataField="鄉鎮市區" HeaderText="鄉鎮市區" ItemStyle-Width="140" SortExpression="鄉鎮市區">
                    <ItemStyle Width="140px"></ItemStyle>
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="功能" ItemStyle-Width="100">
                        <EditItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="更新" CommandArgument='<% #Container.DataItemIndex %>'></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消"></asp:LinkButton>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="編輯"></asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="btn_del" runat="server" CausesValidation="False" CommandName="Delete" Text="刪除" OnClientClick="return confirm('確定刪除嗎?')" CommandArgument='<% #Container.DataItemIndex %>' ></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerTemplate>
                    <table style="width: 100%;">
                        <tr>
                            <td style="width:70px"></td>
                            <td>
                                <asp:Label ID="Label5" runat="server" Text="請選擇頁碼"></asp:Label>
                                <asp:LinkButton ID="Linkpage1" runat="server" Font-Names="Webdings" Font-Underline="false" OnClick="page_SelectedIndexChanged" >3</asp:LinkButton>
                                <asp:DropDownList ID="ddlpage" AutoPostBack="true" runat="server" OnSelectedIndexChanged="page_SelectedIndexChanged"></asp:DropDownList>
                                <asp:LinkButton ID="Linkpage2" runat="server" Font-Names="Webdings" Font-Underline="false" OnClick="page_SelectedIndexChanged">4</asp:LinkButton>
                            </td>
                            <td style="text-align: right">
                                <asp:Label ID="labNum" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                    </table>
                </PagerTemplate>
<%--                <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />--%>
                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" HorizontalAlign="Center" VerticalAlign="Middle" />
                <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Center" VerticalAlign="Middle" />
                <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" 
                DeleteCommand="DELETE FROM 郵遞區號 WHERE (郵遞代碼 = @郵遞代碼)" 
                UpdateCommand="UPDATE 郵遞區號 SET 郵遞區號 = @郵遞區號, 縣市代碼 = @縣市代碼, 鄉鎮市區 = @鄉鎮市區 WHERE (郵遞代碼 = @郵遞代碼)"
                SelectCommand="SELECT 郵遞區號.*, 縣市.縣市 FROM 郵遞區號 
                                     INNER JOIN 縣市 ON 郵遞區號.縣市代碼 = 縣市.縣市代碼
                                        WHERE (((@縣市代碼=0) OR (郵遞區號.縣市代碼=@縣市代碼))
                                           AND ((@鄉鎮市區='') OR (郵遞區號.鄉鎮市區 LIKE '%'+@鄉鎮市區+'%')) 
                                           AND ((@郵遞區號=0) OR (郵遞區號.郵遞區號=@郵遞區號)))">
                <DeleteParameters>
                    <asp:Parameter Name="郵遞代碼" Type="Int32"/>
                </DeleteParameters>
                <UpdateParameters>
                    <asp:Parameter Name="郵遞代碼" Type="Int32"/>
                    <asp:Parameter Name="縣市代碼" Type="Int32"/>
                    <asp:Parameter Name="鄉鎮市區" Type="String"/>
                    <asp:Parameter Name="郵遞區號" Type="Int32"/>
                </UpdateParameters>
                <SelectParameters>
                    <asp:ControlParameter ControlID="ddl1" Name="縣市代碼" PropertyName="SelectedValue" Type="Int32" />
                    <asp:ControlParameter ControlID="labtemp" Name="郵遞區號" Type="Int32" />
                    <asp:ControlParameter ControlID="txtAreaName" Name="鄉鎮市區" Type="String" ConvertEmptyStringToNull="false" />
                </SelectParameters>
            </asp:SqlDataSource>
            <br />
            <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" 
                SelectCommand="SELECT * FROM [縣市] WHERE ([顯示] = @顯示)">
                <SelectParameters>
                    <asp:Parameter DefaultValue="true" Name="顯示" Type="Boolean" />
                </SelectParameters>
            </asp:SqlDataSource>
        </div>
    </div>
        </ContentTemplate></asp:UpdatePanel>
    <br /><br />
</asp:Content>
