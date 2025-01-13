<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="AuthorityManageEmp.aspx.cs" Inherits="AuthorityManageEmp" 
    MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/bootstrap.css" rel="stylesheet" />
</asp:Content>
<%--    <asp:SqlDataSource ID="SqlDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [帳號使用權限] WHERE ([權限代碼] &lt;&gt; @權限代碼)">--%>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [部門]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [職稱]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [帳號使用權限] where 權限代碼 not like 'M'+'%'"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [帳號使用權限] WHERE (([權限代碼] &lt;&gt; @權限代碼)AND(權限代碼 not like 'M'+'%'))">
        <SelectParameters>
            <asp:Parameter DefaultValue="07" Name="權限代碼" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>

    <div class="container">
        <div style="color: #e14286; text-align: left; margin-top: 20px">
            <asp:Label ID="Label2" runat="server" Text="員工權限管理" CssClass="h2"></asp:Label>
        </div>
        <br />
        <div>
            <table style="width: 400px; text-align: center" class="table-condensed">
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="部　　門"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="職　　　稱"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="帳　　　號"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="Label8" runat="server" Text="姓　　　名"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="Label6" runat="server" Text="狀　態"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="ddlDep" runat="server" DataSourceID="SqlDataSource1" CssClass="input-sm"
                            DataTextField="部門名稱" DataValueField="部門代號" AppendDataBoundItems="true">
                            <asp:ListItem Selected="True" Value="0">全部</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlEmp" runat="server" DataSourceID="SqlDataSource2" CssClass="input-sm"
                            DataTextField="職稱名稱" DataValueField="職稱代號" AppendDataBoundItems="true">
                            <asp:ListItem Selected="True" Value="0">全部</asp:ListItem>
                        </asp:DropDownList>
                    </td>
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
                            <asp:ListItem Value="False">未啟用</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="查詢" CssClass="btn btn-primary" />
                    </td>
                </tr>
            </table>
            <hr />
        </div>
    </div>
    <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" OnSelected="SqlDataSource3_Selected"
        UpdateCommand="UPDATE [員工帳號] SET [啟用] = @啟用,[權限代碼]=@權限代碼 WHERE [員工帳號] = @員工帳號"
        SelectCommand="SELECT 員工帳號基本資料.姓名,部門.部門名稱,職稱.職稱名稱,員工帳號.員工帳號,員工帳號.啟用,員工帳號.權限代碼,帳號使用權限.權限名稱  FROM [員工帳號基本資料]
        inner join 部門 on 員工帳號基本資料.部門代號=部門.部門代號 
        inner join 職稱 on 員工帳號基本資料.職稱代號=職稱.職稱代號 
        inner join 員工帳號 on 員工帳號基本資料.員工帳號=員工帳號.員工帳號
        inner join 帳號使用權限 on 員工帳號.權限代碼=帳號使用權限.權限代碼
        WHERE (((@部門代號=0) OR (部門.部門代號=@部門代號))
        AND ((@職稱代號=0) OR (職稱.職稱代號=@職稱代號))
        AND (員工帳號基本資料.員工帳號 LIKE '%'+@員工帳號+'%')
        AND (員工帳號基本資料.姓名 LIKE '%'+@姓名+'%')
        AND ((@啟用='0') OR (啟用=@啟用)) AND (員工帳號基本資料.員工帳號 !='k_admin'))">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlDep" Name="部門代號" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlEmp" Name="職稱代號" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtName" Name="姓名" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
            <asp:ControlParameter ControlID="txtAccount" Name="員工帳號" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
            <asp:ControlParameter ControlID="ddlstatus" Name="啟用" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="員工帳號" Type="String" />
            <asp:Parameter Name="啟用" Type="Boolean" />
            <asp:Parameter Name="權限代碼" Type="String" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <label style="color:red">員工帳號的權限代碼預設為01</label>
    <div>
        <asp:Label ID="Label12" runat="server" Text='' ForeColor="Green"></asp:Label>
    </div>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="員工帳號" CssClass="table-bordered table-condensed" AllowPaging="true" PageSize="15"
        DataSourceID="SqlDataSource3" CellPadding="4" ForeColor="#333333" OnRowCommand="GridView1_RowCommand" OnDataBound="GridView1_DataBound" OnRowDataBound="GridView1_RowDataBound" 
        Height="30px">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:BoundField DataField="部門名稱" HeaderText="部門名稱" SortExpression="部門名稱" ReadOnly="true" ItemStyle-Width="120px" />
            <asp:BoundField DataField="職稱名稱" HeaderText="職稱名稱" SortExpression="職稱名稱" ReadOnly="true" ItemStyle-Width="120px" />
            <asp:BoundField DataField="員工帳號" HeaderText="員工帳號" SortExpression="員工帳號" ReadOnly="true" ItemStyle-Width="120px" />
            <asp:BoundField DataField="姓名" HeaderText="姓名" SortExpression="姓名" ReadOnly="true" ItemStyle-Width="140px" />
            <asp:TemplateField HeaderText="權限名稱" SortExpression="權限名稱" ItemStyle-Width="140px">
                <EditItemTemplate>
                    <asp:DropDownList ID="DropDownList1" runat="server" OnInit="DropDownList1_Init"
                        DataTextField="權限名稱" DataValueField="權限代碼" SelectedValue='<%# Bind("權限代碼") %>'>
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("權限名稱") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField Visible="false">
                <ItemTemplate>
                     <asp:HiddenField ID="hidNo" runat="server" Value='<%# Eval("權限代碼")%>' Visible="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField DataField="啟用" HeaderText="啟用" SortExpression="啟用" ItemStyle-Width="50px" />
            <asp:CommandField HeaderText="功能" EditText="修改" ShowEditButton="True" ItemStyle-Width="100px" />
        </Columns>
        <PagerTemplate>
            <table style="width: 100%;">
                <tr>
                    <td style="width: 70px"></td>
                    <td>
                        <asp:Label ID="Label5" runat="server" Text="請選擇頁碼"></asp:Label>
                        <asp:LinkButton ID="Linkpage1" runat="server"  OnClick="page_SelectedIndexChanged" ForeColor="White" ><span class="glyphicon glyphicon-chevron-left"></span></asp:LinkButton>
                        <asp:DropDownList ID="ddlpage" AutoPostBack="true" runat="server" OnSelectedIndexChanged="page_SelectedIndexChanged" ForeColor="Black"></asp:DropDownList>
                        <asp:LinkButton ID="Linkpage2" runat="server"  OnClick="page_SelectedIndexChanged"  ForeColor="White"><span class="glyphicon glyphicon-chevron-right"></span></asp:LinkButton>
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
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#EFF3FB" />
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#F5F7FB" />
        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
        <SortedDescendingCellStyle BackColor="#E9EBEF" />
        <SortedDescendingHeaderStyle BackColor="#4870BE" />
    </asp:GridView>
    <br />
    <br />
    <script src="Scripts/jquery-1.9.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>
</asp:Content>

