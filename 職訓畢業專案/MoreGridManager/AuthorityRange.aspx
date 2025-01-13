<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="AuthorityRange.aspx.cs" Inherits="AuthorityRange" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
       .t1{text-align:center;width:400px}
       /*.t2{width:200px}*/
       /*tr{height:30px}*/
       .titlecolor{background-color:#ffd800}
       .itemcollor{background-color:#00a9ff}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div style="">
        <div style="color: #e14286; text-align: left; margin-top: 20px">
            <asp:Label ID="Label3" runat="server" Text="權限範圍管理" CssClass="h2"></asp:Label>
        </div>
        <br />
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
            SelectCommand="SELECT * FROM [權限範圍] WHERE ([權限代碼] = @權限代碼)"
            UpdateCommand="UPDATE 權限範圍 SET T1 =@T1, T2 =@T2, T3 =@T3, T4 =@T4, T5 =@T5, T6 =@T6, T7 =@T7, T8 =@T8, T9 =@T9,T10=@T10,T11=@T11 WHERE ([權限代碼] = @權限代碼) ">
            <SelectParameters>
                <asp:QueryStringParameter Name="權限代碼" QueryStringField="權限代碼" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:FormView ID="FormView1" runat="server" BackColor="White" BorderColor="#DEDFDE"  OnItemCommand="FormView1_ItemCommand"
            BorderStyle="None" BorderWidth="1px" CellPadding="4" DataSourceID="SqlDataSource1"
            ForeColor="Black" GridLines="Vertical">
            <EditItemTemplate>
                <div style="text-align:center">
                <asp:Label ID="Label" runat="server" Text='<%#Bind("權限代碼")%>' />
                <asp:Label ID="Label13" runat="server" Text='<%# Request.QueryString.Get("權限名稱") %>' />
                </div>
                <table class="t1 table-bordered table-condensed">
                    <tr>
                        <td colspan="2" class="titlecolor">
                            <asp:Label ID="Label2" runat="server" Text="魔格網站管理"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="商品類別管理"></asp:Label></td>
                        <td style="width:200px">
                            <asp:CheckBox ID="T1CheckBox" runat="server" Checked='<%# Bind("T1") %>' /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="會員帳號管理"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("T2") %>' /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="商品管制設定"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox8" runat="server" Checked='<%# Bind("T10") %>' /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="線上客服系統"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox11" runat="server" Checked='<%# Bind("T11") %>' /></td>
                    </tr>

                    <tr>
                        <td colspan="2" class="titlecolor">
                            <asp:Label ID="Label4" runat="server" Text="內部網站管理"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="權限類別管理"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("T3") %>' /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="縣市代碼管理"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Bind("T4") %>' /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="內部資料設定"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox4" runat="server" Checked='<%# Bind("T5") %>' /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="員工權限設定"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox5" runat="server" Checked='<%# Bind("T6") %>' /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="員工資料管理"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox6" runat="server" Checked='<%# Bind("T7") %>' /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="網站紀錄"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox7" runat="server" Checked='<%# Bind("T8") %>' /></td>
                    </tr>
                    <tr>
                        <td colspan="2" class="titlecolor">
                            <asp:Label ID="Label11" runat="server" Text="個人資料管理"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="個人資料管理"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox10" runat="server" Checked='<%# Bind("T9") %>'/></td>
                    </tr>

                    <br />
                    <tr>
                        <td colspan="2">
                            <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True"  CommandName="Update" Text="更新" />
                            &nbsp;<asp:LinkButton ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消" />
                        </td>
                    </tr>
                </table>

            </EditItemTemplate>
            <ItemTemplate>
                <div style="text-align:center">
                <asp:Label ID="Label" runat="server" Text='<%#Eval("權限代碼")%>' />
                <asp:Label ID="Label13" runat="server" Text='<%# Request.QueryString.Get("權限名稱") %>' />
                </div>
                <table class="t1 table-bordered table-condensed">
                    <tr>
                        <td colspan="2" class="itemcollor">
                            <asp:Label ID="Label2" runat="server" Text="魔格網站管理"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="商品類別管理"></asp:Label></td>
                        <td style="width:200px">
                            <asp:CheckBox ID="T1CheckBox" runat="server" Checked='<%# Bind("T1") %>' Enabled="false" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="會員帳號管理"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("T2") %>' Enabled="false" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label14" runat="server" Text="商品管制設定"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox9" runat="server" Checked='<%# Bind("T10") %>' Enabled="false" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="線上客服系統"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox11" runat="server" Checked='<%# Bind("T11") %>' Enabled="false" /></td>
                    </tr>
                    <tr>
                        <td colspan="2" class="itemcollor">
                            <asp:Label ID="Label4" runat="server" Text="內部網站管理"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="權限類別管理"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("T3") %>' Enabled="false" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="縣市代碼管理"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Bind("T4") %>' Enabled="false" /></td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="內部資料設定"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox4" runat="server" Checked='<%# Bind("T5") %>' Enabled="false" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="員工權限設定"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox5" runat="server" Checked='<%# Bind("T6") %>' Enabled="false" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="員工資料管理"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox6" runat="server" Checked='<%# Bind("T7") %>' Enabled="false" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="網站紀錄"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox7" runat="server" Checked='<%# Bind("T8") %>' Enabled="false" /></td>
                    </tr>
                    <tr>
                        <td colspan="2" class="itemcollor">
                            <asp:Label ID="Label11" runat="server" Text="個人資料管理"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="個人資料管理"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="CheckBox8" runat="server" Checked='<%# Bind("T9") %>' Enabled="false" /></td>
                    </tr>
                    <br />
                    <tr>
                        <td colspan="2">
                            <asp:LinkButton ID="editbutton" runat="server" CausesValidation="True" CommandName="Edit" Text="編輯" />
                            &nbsp&nbsp&nbsp&nbsp<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="AuthorityManage.aspx">回上頁</asp:HyperLink>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:FormView>
    </div>
</asp:Content>

