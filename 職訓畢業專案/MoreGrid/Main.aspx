<%@ Page Title="" Language="C#" MasterPageFile="Top2.master" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" Runat="Server">
    <style>
        .goodsItem{            
            padding:5px;
            text-align:center;
            margin-top:10px;
            height:300px;
        }
        .goodsItem:hover{
            background-color:#d0d0d0;
        }
        .price{
            font-size:20px;
            font-weight:bold;
            color:#ff00dc;
        }
    </style>
    <script>
        $(document).ready(function () {
            $('img[src="goodPicture/"]').attr("src", "Image/subsite_logo/Gshan_200x200.png");
      
        });
    </script>

</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SiteDisp" Runat="Server">
    <div>
        <div>
            <%-- 新進商品 --%>
            <div class="row myTitle">
                新進商品
            </div>


            <div class="container-fluid">
                <div class="row">
                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                     SelectCommand="select * from 商品 where [下架時間]>getdate() and 停權=0 order by 商品編號 desc"  ></asp:SqlDataSource>
                                         
                    <asp:DataList ID="DataList1" runat="server" DataSourceID="SqlDataSource1"
                         RepeatColumns="4" RepeatDirection="Horizontal">
                        <ItemStyle Width="250" Height="350" />
                        <ItemTemplate>
                            <div class="goodsItem">
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
                                    <asp:Label ID="Label5" runat="server" Text='網路價：$' ForeColor="#ff0066" Font-Size="14"></asp:Label>                                
                                    <asp:Label ID="Label6" runat="server" Text='<%# Convert.ToInt64(Eval("單價")).ToString("#,#")  %>' ForeColor="#ff0066" Font-Size="16"></asp:Label>                                                                
                                </div>
                            </div>
                        </ItemTemplate>                        
                    </asp:DataList>
                    
                    
                    
                             
                    <%--<asp:Repeater ID="Repeater1" runat="server" DataSourceID="SqlDataSource1">
                       

                        <ItemTemplate>
                            <div class="col-lg-3 goodsItem">
                                <div style="height:200px;">
                                    <a href="GoodsDisp.aspx?id=<%#Eval("商品編號") %>"><img src="goodPicture/<%#Eval("商品小圖片") %>" style="max-height:200px; max-width:200px" /></a>
                                </div>
                                <br />
                                <div>
                                    <b><a href="GoodsDisp.aspx?id=<%#Eval("商品編號") %>"><%#Eval("商品名稱") %></a></b>
                                </div>
                                <div>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("商品摘要") %>'></asp:Label>
                                </div>
                                <div>
                                    <asp:Label ID="Label5" runat="server" Text='網路價：' ForeColor="#ff0066" Font-Size="14"></asp:Label>                                
                                    <asp:Label ID="Label6" runat="server" Text='<%# String.Format("{0:0}", Eval("單價")) %>' ForeColor="#ff0066" Font-Size="16"></asp:Label>                                                                
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>--%>
                </div>
            </div>
        </div>

<%--        <div>
            <div class="row" style="background-color:blue;">新進網站</div>
            <div class="row">
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" 
                    SelectCommand="SELECT Top 12 * FROM [subsite] order by subsite_id desc"></asp:SqlDataSource>
                <asp:Repeater ID="Repeater2" runat="server" DataSourceID="SqlDataSource2">
                    <ItemTemplate>
                        <div class="col-lg-3" >
                            
                            <%#Eval("name") %>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>                
            </div>


        </div>--%>






    </div>
       




</asp:Content>

