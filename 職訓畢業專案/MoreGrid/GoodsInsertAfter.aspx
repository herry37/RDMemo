<%@ Page Title="" Language="C#" MasterPageFile="Left.master" AutoEventWireup="true" CodeFile="GoodsInsertAfter.aspx.cs" Inherits="GoodsInsertAfter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CContent" Runat="Server">
    <asp:Panel runat="server">
        <asp:Label runat="server" ID="InsertAfter" Font-Bold="True" ForeColor="#3366FF" Font-Size="20"></asp:Label><br />
        <asp:Button runat="server" ID="goodsInsert" Text="新增商品" CssClass="btn btn-primary" OnClick="goodsInsert_Click" />

        <asp:Button runat="server" ID="MoreGird" Text="返回主頁" CssClass="btn btn-danger" OnClick="MoreGird_Click"  />
    </asp:Panel>
</asp:Content>

