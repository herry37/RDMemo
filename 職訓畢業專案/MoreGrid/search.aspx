<%@ Page Title="" Language="C#" MasterPageFile="Top2.master" AutoEventWireup="true" CodeFile="search.aspx.cs" Inherits="search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" runat="Server">
    <style>
        .gvheard {
            text-align: center;
            line-height: 50px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SiteDisp" runat="Server">

 
    <%--搜尋結果--%>
    <asp:SqlDataSource ID="SqlGoodsSearch2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"></asp:SqlDataSource>


   <div>
            <asp:Repeater ID="gvSearch" runat="server" >

                    <ItemTemplate>
                        <div class="col-lg-3 goodsItem">
                            <div style="height:200px;">
                                <a href="GoodsDisp.aspx?id=<%#Eval("商品編號") %>" target="_blank"><img src="goodPicture/<%#Eval("商品小圖片") %>" style="max-height:200px; max-width:200px" /></a>
                            </div>
                            <br />
                            <div>
                                <b><a href="GoodsDisp.aspx?id=<%#Eval("商品編號") %>" target="_blank"><%#Eval("商品名稱") %></a></b>
                            </div>
                            <div>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("商品摘要") %>'></asp:Label>
                            </div>
                            <div>
                                <asp:Label ID="Label5" runat="server" Text='網路價：' ForeColor="#ff0066" Font-Size="14"></asp:Label>   
                                 <asp:Label ID="Label2" runat="server" Text="$"></asp:Label>                             
                                <asp:Label ID="Label6" runat="server" Text='<%# Convert.ToInt64(Eval("單價")).ToString("#,#") %>' ForeColor="#ff0066" Font-Size="16"></asp:Label>                                                                
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
   </div>
    <br />
    &nbsp;<br />

 
    <script src="../Scripts/jquery-2.2.1.js"></script>
    <script src="../js/bootstrap.js"></script>
    <script>
      

    </script>
</asp:Content>

