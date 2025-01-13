<%@ Page Title="" Language="C#" MasterPageFile="Left.master" AutoEventWireup="true" CodeFile="custModifyData.aspx.cs" Inherits="custModifyData" %>




<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" runat="Server">
    <style type="text/css">
        .table1 {
            border: 1px solid black;
            width: 100%;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="CContent" runat="Server">

    <asp:SqlDataSource ID="Sqlregistered" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" ProviderName="System.Data.SqlClient"
        UpdateCommand="update 會員 set 暱稱=@暱稱,email=@email,市話=@市話,手機=@手機 where 帳號=@帳號">

        <UpdateParameters>
            <asp:Parameter Name="暱稱" Type="String" />
            <asp:Parameter Name="email" Type="String" />
            <asp:Parameter Name="市話" Type="String" />
            <asp:Parameter Name="郵遞代號" Type="Int32"/>
            <asp:Parameter Name="地址其他" Type="String" />
            <asp:Parameter Name="手機" Type="String" />
            <asp:Parameter Name="帳號" Type="String" />
        </UpdateParameters>
    </asp:SqlDataSource>

   <%-- <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT [縣市代碼], [縣市] FROM [縣市] WHERE ([顯示] = @顯示)">
        <SelectParameters>
            <asp:Parameter DefaultValue="true" Name="顯示" Type="Boolean" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT [郵遞代碼], [郵遞區號], [鄉鎮市區] FROM [郵遞區號] WHERE ([縣市代碼] = @縣市代碼)">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlcity" Name="縣市代碼" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>--%>

    <asp:DataList ID="DataList1" runat="server" Width="600px" DataKeyField="帳號" DataSourceID="Sqlregistered" RepeatColumns="1" RepeatDirection="Vertical" GridLines="Both" OnEditCommand="DataList1_EditCommand" OnCancelCommand="DataList1_CancelCommand" OnUpdateCommand="DataList1_UpdateCommand">

        <HeaderStyle BackColor="#3399ff" ForeColor="White" HorizontalAlign="Center" />
        <HeaderTemplate>
            會員資料修改
        </HeaderTemplate>
        <EditItemTemplate>
            <table class="table1">
                <tr>
                    <td>帳號：</td>
                    <td>
                        <asp:Label ID="txtNo" runat="server" Text='<%#Eval("帳號") %>'>></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td>密碼：</td>
                    <td><asp:Label ID="txtPsd" runat="server" ><%#Eval("密碼").ToString().Substring(0,3)%>*****<%#Eval("密碼").ToString().Substring(Eval("密碼").ToString().Length-1,1)%></asp:Label>
                       </td>
                </tr>
                <tr>
                    <td>暱稱：</td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" Text='<%#Bind("暱稱") %>'></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtName" ErrorMessage="請檢查是否為中文" Text="*(請輸入2-20字中文)" ForeColor="red" Display="Dynamic" ValidationExpression="[\w\u4e00-\u9fa5]{2,20}"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td>性別：</td>
                    <td><%#Eval("性別").ToString().Replace("True","男性").Replace("False","女性") %></td>
                </tr>
                <tr>
                    <td>E-mail：</td>
                    <td>
                        <asp:TextBox ID="txtEmail" runat="server" Text='<%#Bind("email") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtemail" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ErrorMessage="(請檢查格式)" ValidationExpression="\w+([-+.']\w+)*@[A-Za-z]+([-.][A-Za-z]+)*\.[A-Za-z]+([-.][A-Za-z]+)*" ForeColor="Red" Display="Dynamic" ControlToValidate="txtEmail"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td>市話：</td>
                    <td>
                        <asp:TextBox ID="txtTel" runat="server" placeholder="(xx)xxx-xxxx" Text='<%#Bind("市話") %>'></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="txtTel" ErrorMessage="請檢查格式" Text="*(請輸入正確格式)" ForeColor="red" Display="Dynamic" ValidationExpression="[(]+[0]+\d{1,2}[)]+\d{3,4}(\-+)[0-9]{4}"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td>

                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [縣市] where 顯示=@顯示" >
                        <SelectParameters>
                            <asp:Parameter DefaultValue="true" Name="顯示" Type="Boolean" />
                        </SelectParameters>
                    </asp:SqlDataSource>

                        <asp:Label ID="lab4" runat="server" Text='<%# Eval("郵遞代號") %>' Visible="false"></asp:Label>

                    <asp:DropDownList ID="ddl3" runat="server" DataSourceID="SqlDataSource1" DataTextField="縣市" DataValueField="縣市代碼" SelectedValue='<%#Eval("縣市代碼") %>' AutoPostBack="true" OnDataBound="ddl3_DataBound" OnSelectedIndexChanged="ddl3_SelectedIndexChanged"></asp:DropDownList>

                      <asp:SqlDataSource ID="SqlDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [郵遞區號]  WHERE ([縣市代碼] = @縣市代碼)">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddl3" Name="縣市代碼" PropertyName="SelectedValue" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                        <asp:DropDownList ID="ddl4" runat="server" DataSourceID="SqlDataSource5" DataTextField="鄉鎮市區" DataValueField="郵遞代碼"  AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddl4_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:TextBox ID="txtaddress" runat="server" Text='<%# Bind("村里") %>'></asp:TextBox>
                    <asp:TextBox ID="txtadd" runat="server" Text='<%# Bind("住址其他") %>'></asp:TextBox>
                    <asp:TextBox ID="txtareano" runat="server" Text='<%# Bind("郵遞代號") %>' ReadOnly="true" Visible="false"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="ddl4" runat="server" ErrorMessage="請選擇鄉鎮市區" Display="Dynamic" InitialValue="0" ForeColor="Red"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>手機：</td>
                    <td>
                        <asp:TextBox ID="txtMobile" runat="server" Text='<%#Bind("手機") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtMobile" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" ControlToValidate="txtMobile" ErrorMessage="請檢查格式" Text="*(請輸入09xxxxxxxx)" ForeColor="red" Display="Dynamic" ValidationExpression="[0]+[9]+\d{8}"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td>生日：</td>
                    <td><%#Eval("生日","{0:d}") %></td>
                </tr>
            </table>

            <asp:Button ID="LinkButton1" runat="server" CommandName="Update" Text="儲存修改"></asp:Button>

            <asp:Button ID="LinkButton2" runat="server" CausesValidation="false" CommandName="Cancel" Text="取消"></asp:Button>


        </EditItemTemplate>

        <ItemStyle BorderWidth="3" BorderStyle="Solid" />
        <ItemTemplate>
            <table class="table1">
                <tr>
                    <td>帳號：</td>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text='<%#Eval("帳號") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td>密碼：</td>
                    <td><%#Eval("密碼").ToString().Substring(0,3)%>*****<%#Eval("密碼").ToString().Substring(Eval("密碼").ToString().Length-1,1)%></td>
                </tr>
                <tr>
                    <td>暱稱：</td>
                    <td><%#Eval("暱稱") %></td>
                </tr>
                <tr>
                    <td>性別：</td>
                    <td><%#Eval("性別").ToString().Replace("True","男性").Replace("False","女性") %></td>
                </tr>
                <tr>
                    <td>E-mail：</td>
                    <td><%#Eval("email") %></td>
                </tr>
                <tr>
                    <td>市話：</td>
                    <td><%#Eval("市話") %></td>
                </tr>
                <tr>
                    <td>住址：</td>
                    <td>
                    <asp:Label ID="Label5" runat="server" Text='<%# Eval("縣市") %>'></asp:Label>
                    <asp:Label ID="Label7" runat="server" Text='<%# Eval("鄉鎮市區") %>'></asp:Label>
                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("村里") %>'></asp:Label>
                    <asp:Label ID="Label9" runat="server" Text='<%# Eval("住址其他") %>'></asp:Label></td>
                </tr>
                <tr>
                    <td>手機：</td>
                    <td><%#Eval("手機") %></td>
                </tr>
                <tr>
                    <td>生日：</td>
                    <td><%#Eval("生日","{0:d}") %></td>
                </tr>


            </table>
            <asp:Button ID="LinkButton1" runat="server" CommandName="Edit" Text="修改資料"></asp:Button>

             <asp:Button ID="btn_psd" runat="server" Text="修改密碼" OnClick="btn_psd_Click"></asp:Button>
        </ItemTemplate>
    </asp:DataList>

</asp:Content>
