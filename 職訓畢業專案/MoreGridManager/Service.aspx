<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="Service.aspx.cs" Inherits="Servicet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <br /><br />
      <ul class="nav nav-pills">
  <li role="presentation" class="active"><a href="Service.aspx">客服</a></li>
  <li role="presentation"><a href="7002.aspx" >線上客服</a></li>
  <li role="presentation"><a href="ServiceList.aspx" >對話清單</a></li>
  <asp:Panel runat="server" ID="PaneService" CssClass="btn btn-danger"><li role="presentation"><a href="ServiceInsert.aspx">客服人員</a></li></asp:Panel>
</ul>

        <script src="../Scripts/jquery-2.2.1.js"></script>
    <script src="../js/bootstrap.js"></script>
</asp:Content>

