<%@ Page Title="" Language="C#" MasterPageFile="Left.master" AutoEventWireup="true" CodeFile="GoodsEdit.aspx.cs" Inherits="GoodsEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" Runat="Server">

    <style>
        #GoodsEditpacket{
            width:845px;
       }
        #GoodsEditleft{
            /*background-color:#93fbf2;*/
            float:left;
            width:422px;
            height:50px;
       }
        #GoodsEditright{
            /*background-color:#f9ffae;*/
            float:right;
            width:422px;
            height:50px;
       }

        /*隱藏多餘字元*/
       .gssummaryskip {
          overflow:hidden;
          text-overflow:ellipsis; 
          white-space: nowrap;
          height:50px; 
       }
       .gvheard{
           text-align:center;
           line-height:50px;

       }

        th.asc a:after {
            content: " ▲";
        }

        th.desc a:after {
            content: " ▼";
        }
       /*.TbHeight{
           height:30px;
       }*/
    </style>
    <script>
        $(document).ready(function () {
            $('img[src="goodPicture/"]').attr("src", "Image/subsite_logo/Gshan_200x200.png");

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CContent" Runat="Server">
    <h2>商品管理畫面</h2>
    <%--<asp:SqlDataSource ID="SqlDataSource1zzzzzzzzz" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [order_detail]"></asp:SqlDataSource>--%>
    <div id="GoodsEditpacket">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

        <div id="GoodsEditleft">
            <asp:Button ID="btnshow" CssClass="btn btn-primary btn-lg btn-block" runat="server" Text="上架商品" OnClick="btnshow_Click" />
        </div>
        <div id="GoodsEditright">
            <asp:Button ID="btndown" CssClass="btn btn-default btn-lg btn-block" runat="server" Text="下架商品" OnClick="btndown_Click" />
        </div>
                        
                <asp:Panel id="GoodsEditshow" CssClass="GoodsEditinfo" runat="server">
            
            <%--上架商品--%>
            <asp:SqlDataSource ID="SqlDataSourceGoodsEdit" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                ProviderName="System.Data.SqlClient"></asp:SqlDataSource>

            <asp:GridView ID="GVShow" runat="server" AutoGenerateColumns="False" DataKeyNames="商品編號" DataSourceID="SqlDataSourceGoodsEdit"
                AllowPaging="true" PageSize="5" OnRowCommand="GVShow_RowCommand" AllowSorting="True" Visible="true">

                <SortedAscendingHeaderStyle CssClass="asc" />
                <SortedDescendingHeaderStyle CssClass="desc" />
                <Columns>

                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="商品圖示" SortExpression="">
                        <ItemTemplate>
                            <div class="gssummaryskip gvheard" style='width: 70px;'>
                                <img alt="" src="goodPicture/<%#Eval("商品小圖片")%>" style="max-height: 50px; max-width: 50px" />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="商品名稱" SortExpression="">
                        <ItemTemplate>
                            <div class="gssummaryskip" style='width: 130px;'>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("商品名稱")%>'></asp:Label></div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="商品摘要" SortExpression="">
                        <ItemTemplate>
                            <div class="gssummaryskip" style='width: 130px;'>
                                <asp:Label ID="HL1" runat="server" Text='<%#Eval("商品摘要") %>'></asp:Label></div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderStyle-CssClass="gvheard" DataField="庫存量" HeaderText="庫存量" SortExpression="" ControlStyle-Width="80px" />
                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="單價" SortExpression="單價" ControlStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="HL1" runat="server" Text='<%#Eval("單價","{0:0}")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="上架時間" SortExpression="上架時間" ControlStyle-Width="80px">
                        <ItemTemplate>
                            <asp:Label ID="HL1" runat="server" Text='<%#Eval("上架時間","{0:d}")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="下架時間" SortExpression="下架時間" ControlStyle-Width="80px">
                        <ItemTemplate>
                            <asp:Label ID="HL1" runat="server" Text='<%#Eval("下架時間","{0:d}")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="商品類別名稱" SortExpression="商品類別名稱" ControlStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#Eval("商品類別名稱")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:HyperLinkField HeaderText="編輯" Text="詳細資料" ControlStyle-CssClass="btn btn-info"
                        DataTextFormatString="編輯"
                        DataTextField="商品編號"
                        DataNavigateUrlFormatString="GoodsEditfew.aspx?GsNo={0}"
                        DataNavigateUrlFields="商品編號"
                        HeaderStyle-CssClass="gvheard" ControlStyle-Width="50px" />
                </Columns>

                <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" Height="30px" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
            </asp:GridView>

        </asp:Panel>

        
        <asp:Panel id="GoodsEditdown"  runat="server" Visible="false">

            <%--下架商品--%>

            <asp:SqlDataSource ID="SqlDataSourceGoodsEditdown" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                ProviderName="System.Data.SqlClient"></asp:SqlDataSource>

            <asp:GridView ID="GVDown" runat="server" AutoGenerateColumns="False" DataKeyNames="商品編號" DataSourceID="SqlDataSourceGoodsEditdown"
                AllowPaging="True" PageSize="5" AllowSorting="True" BackColor="White" BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" CellPadding="4">

                <RowStyle BackColor="White" ForeColor="#003399" />
                <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                <SortedAscendingCellStyle BackColor="#EDF6F6" />

                <SortedAscendingHeaderStyle CssClass="asc" BackColor="#0D4AC4" />
                <SortedDescendingCellStyle BackColor="#D6DFDF" />
                <SortedDescendingHeaderStyle CssClass="desc" BackColor="#002876" />
                <Columns>

                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="商品圖示" SortExpression="">
                        <ItemTemplate>
                            <div class="gssummaryskip gvheard" style='width: 70px;'>
                                <img alt="" src="goodPicture/<%#Eval("商品小圖片")%>" style="max-height: 50px; max-width: 50px" />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="商品名稱" SortExpression="">
                        <ItemTemplate>
                            <div class="gssummaryskip" style='width: 130px;'>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("商品名稱")%>'></asp:Label></div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="商品摘要" SortExpression="">
                        <ItemTemplate>
                            <div class="gssummaryskip" style='width: 130px;'>
                                <asp:Label ID="HL1" runat="server" Text='<%#Eval("商品摘要") %>'></asp:Label></div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderStyle-CssClass="gvheard" DataField="庫存量" HeaderText="庫存量" SortExpression="" ControlStyle-Width="80px" />
                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="單價" SortExpression="單價" ControlStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="HL1" runat="server" Text='<%#Eval("單價","{0:0}")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="上架時間" SortExpression="上架時間" ControlStyle-Width="80px">
                        <ItemTemplate>
                            <asp:Label ID="HL1" runat="server" Text='<%#Eval("上架時間","{0:d}")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="下架時間" SortExpression="下架時間" ControlStyle-Width="80px">
                        <ItemTemplate>
                            <asp:Label ID="HL1" runat="server" Text='<%#Eval("下架時間","{0:d}")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="商品類別名稱" SortExpression="商品類別名稱" ControlStyle-Width="150px">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#Eval("商品類別名稱")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:HyperLinkField HeaderText="編輯" Text="詳細資料" ControlStyle-CssClass="btn btn-info"
                        DataTextFormatString="編輯"
                        DataTextField="商品編號"
                        DataNavigateUrlFormatString="GoodsEditfew.aspx?GsNo={0}"
                        DataNavigateUrlFields="商品編號"
                        HeaderStyle-CssClass="gvheard" ControlStyle-Width="50px" />
                </Columns>
                <%--        <PagerSettings Mode="NextPreviousFirstLast" 
            FirstPageImageUrl="/Image/first.png" FirstPageText="第一頁"
            LastPageImageUrl="/Image/last.png" LastPageText="上一頁" 
            PreviousPageImageUrl="/Image/previous.png" PreviousPageText="最後頁"
            NextPageImageUrl="/Image/next.png" NextPageText="下一頁"
            Position="Bottom" PageButtonCount="5" />--%>

                <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" Height="30px" />
                <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
                <HeaderStyle BackColor="#003399" Font-Bold="True" ForeColor="#CCCCFF" />
            </asp:GridView>

        </asp:Panel>
                </ContentTemplate>
        </asp:UpdatePanel>
    </div>


</asp:Content>
