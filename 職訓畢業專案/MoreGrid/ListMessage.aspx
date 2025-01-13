<%@ Page Title="" Language="C#" MasterPageFile="Left.master" AutoEventWireup="true" CodeFile="ListMessage.aspx.cs" Inherits="ListMessage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" runat="Server">

    <style>
        #ListMessagepacket {
            width: 856px;
        }

        #ListMessageleft {
            /*background-color:#93fbf2;*/
            float: left;
            width: 428px;
            height: 50px;
        }

        #ListMessageright {
            /*background-color:#f9ffae;*/
            float: right;
            width: 428px;
            height: 50px;
        }

        /*隱藏多餘字元*/
        .gssummaryskip {
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
            height: 50px;
        }

        .gvheard {
            text-align: center;
            line-height: 50px;
        }

        th.asc a:after {
            content: " ▲";
        }

        th.desc a:after {
            content: " ▼";
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CContent" runat="Server">
    <h1>留言清單</h1>

    <div id="ListMessagepacket">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div id="ListMessageleft">
                    <asp:Button ID="btnQ" CssClass="btn btn-primary btn-lg btn-block" runat="server" Text="我是買家(提問)" OnClick="btnQ_Click" />
                </div>
                <div id="ListMessageright">
                    <asp:Button ID="btnR" CssClass="btn btn-default btn-lg btn-block" runat="server" Text="我是賣家(回覆)" OnClick="btnR_Click" />
                </div>


                <asp:Panel ID="ListMessageQ" CssClass="ListMessageinfo" runat="server" Visible="true">

                    <%--購物提問留言(買)--%>
                    <asp:SqlDataSource ID="SqlDataSourceListMessageQ" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                        ProviderName="System.Data.SqlClient"></asp:SqlDataSource>

                    <asp:GridView ID="GVLMQ" runat="server" AutoGenerateColumns="False" DataKeyNames="留言編號" DataSourceID="SqlDataSourceListMessageQ"
                        AllowPaging="True" PageSize="5" AllowSorting="True">

                        <SortedAscendingHeaderStyle CssClass="asc" />
                        <SortedDescendingHeaderStyle CssClass="desc" />
                        <Columns>

                            <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="商品名稱" SortExpression="">
                                <ItemTemplate>
                                    <div class="gssummaryskip gvheard" style='width: 150px;'>
                                    <asp:Label ID="Label1" runat="server" ><a href='<%#"GoodsDisp.aspx?id="+Eval("商品編號")%>'><%#Eval("商品名稱")%></a></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="提問內容" SortExpression="">
                                <ItemTemplate>
                                    <div class="gssummaryskip gvheard" style='width: 175px;'>
                                        <asp:Label ID="HL1" runat="server" Text='<%#Eval("內容") %>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="留言時間" SortExpression="">
                                <ItemTemplate>
                                    <div class="gssummaryskip gvheard" style='width: 175px;'>
                                        <asp:Label ID="Label1" runat="server" Text='<%#Eval("留言時間")%>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="回覆留言" SortExpression="">
                                <ItemTemplate>
                                    <div class="gssummaryskip gvheard" style='width: 175px;'>
                                        <asp:Label ID="HL1" runat="server" Text='<%#Eval("回覆內容") %>'></asp:Label>
                                        <asp:Label ID="Label1" runat="server" ForeColor="Red" Text='<%#shipment_Check("回覆內容")%>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="回覆時間" SortExpression="">
                                <ItemTemplate>
                                    <div class="gssummaryskip gvheard" style='width: 175px;'>
                                        <asp:Label ID="Label1" runat="server" Text='<%#Eval("回覆時間")%>'></asp:Label>
                                        <asp:Label ID="Label2" runat="server" ForeColor="Red" Text='<%#shipment_Check("回覆時間")%>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" Height="30px" />
                        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                    </asp:GridView>

                </asp:Panel>


                <asp:Panel ID="ListMessageR" runat="server" Visible="false">

                    <%--商品出售留言(賣)--%>

                    <asp:SqlDataSource ID="SqlDataSourceListMessageR" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                        ProviderName="System.Data.SqlClient"></asp:SqlDataSource>

                    <asp:GridView ID="GVLMR" runat="server" AutoGenerateColumns="False" DataKeyNames="留言編號" DataSourceID="SqlDataSourceListMessageR"
                        AllowPaging="True" PageSize="5" AllowSorting="True" BackColor="White" BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" CellPadding="4">

                        <RowStyle BackColor="White" ForeColor="#003399" />
                        <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                        <SortedAscendingCellStyle BackColor="#EDF6F6" />

                        <SortedAscendingHeaderStyle CssClass="asc" BackColor="#0D4AC4" />
                        <SortedDescendingCellStyle BackColor="#D6DFDF" />
                        <SortedDescendingHeaderStyle CssClass="desc" BackColor="#002876" />
                        <Columns>

                            <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="商品名稱" SortExpression="">
                                <ItemTemplate>
                                    <div class="gssummaryskip gvheard" style='width: 150px;'>
                                    <asp:Label ID="Label1" runat="server" ><a href='<%#"GoodsDisp.aspx?id="+Eval("商品編號")%>'><%#Eval("商品名稱")%></a></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="提問內容" SortExpression="">
                                <ItemTemplate>
                                    <div class="gssummaryskip gvheard" style='width: 175px;'>
                                        <asp:Label ID="HL1" runat="server" Text='<%#Eval("內容") %>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="留言時間" SortExpression="">
                                <ItemTemplate>
                                    <div class="gssummaryskip gvheard" style='width: 175px;'>
                                        <asp:Label ID="Label1" runat="server" Text='<%#Eval("留言時間")%>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="回覆留言" SortExpression="">
                                <ItemTemplate>
                                    <div class="gssummaryskip gvheard" style='width: 175px;'>
                                        <asp:Label ID="HL1" runat="server" Text='<%#Eval("回覆內容") %>'></asp:Label>
                                        <asp:Label ID="Label2" runat="server" ForeColor="Red" Text='<%#shipment_Check("回覆內容")%>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="gvheard" HeaderText="回覆時間" SortExpression="">
                                <ItemTemplate>
                                    <div class="gssummaryskip gvheard" style='width: 175px;'>
                                        <asp:Label ID="Label1" runat="server" Text='<%#Eval("回覆時間")%>'></asp:Label>
                                        <asp:Label ID="Label2" runat="server" ForeColor="Red" Text='<%#shipment_Check("回覆時間")%>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
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

