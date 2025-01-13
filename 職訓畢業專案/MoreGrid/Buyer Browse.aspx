<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Left.master" CodeFile="Buyer Browse.aspx.cs" Inherits="Buyer_Browse" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" runat="Server">
    <style>
        .center {
            text-align: center;
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
    </style>
    <script>
        $(document).ready(function () {
            $('img[src="goodPicture/"]').attr("src", "Image/subsite_logo/Gshan_200x200.png");

        });
    </script>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="CContent" runat="Server">

    <div>
        <asp:DropDownList ID="dlBrowse" runat="server" AutoPostBack="true" OnSelectedIndexChanged="dlBrowse_SelectedIndexChanged">
            <asp:ListItem Text="所有記錄" Value="0"></asp:ListItem>
            <asp:ListItem Text="尚未出貨" Value="1"></asp:ListItem>
            <asp:ListItem Text="已經出貨" Value="2"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div>
        <asp:Label ID="lbbuy" runat="server" Text="未有購買記錄..." ForeColor="#ff33cc" Font-Bold="true" Visible="false" Font-Size="Larger"></asp:Label>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"></asp:SqlDataSource>

                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="商品編號" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="3" OnDataBound="GridView1_DataBound" CellPadding="4" ForeColor="#333333" GridLines="None">
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
                        <asp:TemplateField HeaderText="圖片" HeaderStyle-CssClass="center">
                            <ItemTemplate>
                                <img src="goodPicture/<%#Eval("商品小圖片") %>" style="width: 50px; height: 50px" />
                                <%--<asp:Label ID="lbPhoto" runat="server" Text='<%#Eval("商品小圖片") %>'></asp:Label>--%>
                            </ItemTemplate>
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:CommandField ShowSelectButton="true" SelectText="訂單明細" HeaderText="訂單明細" ControlStyle-ForeColor="#3399ff" />
                        <%--<asp:TemplateField HeaderText="商品編號" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="center"  >
                    <ItemTemplate>
                        <asp:Label ID="lbNO" runat="server" Text='<%#Eval("商品編號") %>'></asp:Label>
                    </ItemTemplate>
                     <HeaderStyle HorizontalAlign="Center" />
                     <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>--%>
                        <asp:TemplateField HeaderText="商品名稱" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="center">
                            <ItemTemplate>
                                <asp:Label ID="lbName" CssClass="gssummaryskip gvheard" Width="150px" runat="server" Text='<%#Eval("商品名稱") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="成交價格" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="center">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text="$"></asp:Label>
                                <asp:Label ID="lbPrice" runat="server" Text='<%# Convert.ToInt64(Eval("購買價格")).ToString("#,#")  %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="訂購數量" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="center">
                            <ItemTemplate>
                                <asp:Label ID="lbNum" runat="server" Text='<%#Eval("訂購數量") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="出貨日期" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="center">
                            <ItemTemplate>
                                <asp:Label ID="lbNum" runat="server" Text='<%#Eval("出貨日期") %>'></asp:Label>
                                <asp:Label ID="Label1" runat="server" Text='<%#shipment_Check("出貨日期")%>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="收貨人" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="center">
                            <ItemTemplate>
                                <asp:Label ID="lbNum" runat="server" Text='<%#Eval("收貨人") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="賣家" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="center">
                            <ItemTemplate>
                                <asp:Label ID="lbSeller" runat="server" Text='<%#Eval("賣家帳號") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="給賣家的留言" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="center">
                            <ItemTemplate>
                                <asp:Label ID="lbNum" CssClass="gssummaryskip gvheard" Width="150px" runat="server" Text='<%#Eval("買家留言") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <EditRowStyle BackColor="#2461BF" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle HorizontalAlign="Center" BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <PagerTemplate>
                        <table style="width: 100%">
                            <tr>
                                <td>
                                    <asp:Label ID="Label1" runat="server" Text="Label">請選擇頁碼</asp:Label>
                                    <asp:DropDownList ID="dlPager" runat="server" OnSelectedIndexChanged="dlPager_SelectedIndexChanged" AutoPostBack="true" BackColor="#000099"></asp:DropDownList>
                                    <asp:LinkButton ID="lbPrev" runat="server" Style="font-family: Webdings; text-decoration: none" OnClick="dlPager_SelectedIndexChanged">3</asp:LinkButton>
                                    <asp:LinkButton ID="lbNext" runat="server" Style="font-family: Webdings; text-decoration: none" OnClick="dlPager_SelectedIndexChanged">4</asp:LinkButton>
                                </td>
                                <td style="float: right; /*text-align: right*/">
                                    <asp:Label ID="lblInfor" runat="server" Text="page of" float="right"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </PagerTemplate>

                    <RowStyle BackColor="#EFF3FB" />
                    <SelectedRowStyle BackColor="#33ccff" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                    <SortedDescendingHeaderStyle BackColor="#4870BE" />

                </asp:GridView>

                <br />
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT 訂單.訂單編號,訂單.下單日期,訂單.出貨日期,訂單.帳號 as 買家,收貨地址.*,購買會員.手機 as 買家電話,訂單明細.*,商品.*,會員.*,郵遞區號.*,縣市.* FROM 訂單 
        inner join 收貨地址 on 訂單.訂單編號=收貨地址.訂單編號
		inner join 會員 as 購買會員 on 訂單.帳號=購買會員.帳號
        inner join 訂單明細 on 收貨地址.訂單編號=訂單明細.訂單編號 
        inner join 商品 on 商品.商品編號=訂單明細.商品編號 
        inner join 會員 on 商品.帳號=會員.帳號 
        inner join 郵遞區號 on 收貨地址.郵遞代碼=郵遞區號.郵遞代碼 
        inner join 縣市 on 縣市.縣市代碼=郵遞區號.縣市代碼 where 訂單明細.商品編號=@商品編號">

                    <SelectParameters>
                        <asp:ControlParameter ControlID="GridView1" Name="商品編號" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:DetailsView ID="DetailsView1" runat="server" Height="50px" Width="555px" AutoGenerateRows="false" DataSourceID="SqlDataSource2" DataKeyNames="商品編號" CellPadding="0" HeaderText="訂單明細" HeaderStyle-Font-Size="Medium" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="#5D7B9D" HeaderStyle-ForeColor="White" HeaderStyle-Font-Bold="true" RowStyle-HorizontalAlign="Center">
                    <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" Height="30px" HorizontalAlign="Center" Width="140px" />
                    <Fields>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                商 品 小 圖
                            </HeaderTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>

                                <asp:Image ID="Image2" runat="server" ImageUrl='<%# "goodPicture/"+Eval("商品小圖片")%>' Width="100" Height="100" />

                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="訂單編號" HeaderText="訂 單 編 號" ControlStyle-Height="30px" ControlStyle-Width="300px" HeaderStyle-Height="30px" HeaderStyle-Width="100px" />
                        <asp:BoundField DataField="商品名稱" HeaderText="商 品 名 稱" ControlStyle-Height="30px" ControlStyle-Width="300px" />
                        <asp:TemplateField>
                            <HeaderTemplate>
                                下 單 日 期
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("下單日期","{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="帳號" HeaderText="賣 家 帳 號" ControlStyle-Height="30px" ControlStyle-Width="300px" />
                        <asp:BoundField DataField="手機" HeaderText="聯 絡 賣 家" ControlStyle-Height="30px" ControlStyle-Width="300px" />
                        <asp:BoundField DataField="訂購數量" HeaderText="訂 購 數 量" ControlStyle-Height="30px" ControlStyle-Width="300px" />
                        <asp:TemplateField>
                            <HeaderTemplate>
                                購 買 價 格
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text="NT$ "></asp:Label>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("購買價格","{0:0}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="買家" HeaderText="買 家 帳 號" ControlStyle-Height="30px" ControlStyle-Width="300px" HeaderStyle-BackColor="#88e1ff" HeaderStyle-ForeColor="#484848" />
                        <asp:BoundField DataField="收貨人" HeaderText="買 家 姓 名" ControlStyle-Height="30px" ControlStyle-Width="300px" HeaderStyle-BackColor="#88e1ff" HeaderStyle-ForeColor="#484848" />
                        <asp:BoundField DataField="買家電話" HeaderText="買 家 電 話" ControlStyle-Height="30px" ControlStyle-Width="300px" HeaderStyle-BackColor="#88e1ff" HeaderStyle-ForeColor="#484848" />
                        <asp:TemplateField>
                            <HeaderStyle BackColor="#88e1ff" ForeColor="#484848" />
                            <HeaderTemplate>買 家 地 址</HeaderTemplate>
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text='<%#Eval("縣市") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text='<%#Eval("鄉鎮市區") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label3" runat="server" Text='<%#Eval("村里") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Fields>
                </asp:DetailsView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>

