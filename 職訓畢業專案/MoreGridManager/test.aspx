<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test.aspx.cs" Inherits="test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
   
    <form id="form1" runat="server">
    <div>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT [員工帳號], [姓名], [郵遞代號], [地址],縣市.* FROM [員工帳號基本資料] 
            inner join 郵遞區號 on 員工帳號基本資料.郵遞代號=郵遞區號.郵遞代碼 inner join 縣市 on 縣市.縣市代碼=郵遞區號.縣市代碼 "></asp:SqlDataSource>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1">
            <Columns>
                <asp:BoundField DataField="員工帳號" HeaderText="員工帳號" SortExpression="員工帳號" />
                <asp:BoundField DataField="姓名" HeaderText="姓名" SortExpression="姓名" />
                <asp:TemplateField HeaderText="郵遞代號" SortExpression="郵遞代號">
                    <EditItemTemplate>
                        <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="SqlDataSource1" DataTextField="縣市" DataValueField="縣市代碼" SelectedValue='<%# Bind("縣市代碼") %>' AutoPostBack="true">
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [縣市]"  ></asp:SqlDataSource>
                        <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="SqlDataSource2" DataTextField="鄉鎮市區" DataValueField="郵遞代碼" SelectedValue='<%# Bind("郵遞代號") %>' AppendDataBoundItems="true"> 
                            
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [郵遞區號] WHERE ([縣市代碼] = @縣市代碼)">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="DropDownList1" Name="縣市代碼" PropertyName="SelectedValue" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("郵遞代號") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="地址" HeaderText="地址" SortExpression="地址" />
                <asp:CommandField ShowEditButton="True" />
            </Columns>
        </asp:GridView>
    </div>
    </form>
</body>
</html>

