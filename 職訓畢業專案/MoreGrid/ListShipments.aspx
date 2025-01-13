<%@ Page Title="" Language="C#" MasterPageFile="Left.master" AutoEventWireup="true" CodeFile="ListShipments.aspx.cs" Inherits="ListShipments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" Runat="Server">
    <style type="text/css">
        .HeaderStylebtn{
            text-align:center;
        }
        #DetailsView1{
            margin-bottom:20px;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CContent" Runat="Server">
    
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" 
        SelectCommand="select * from order_detail WHERE (擁有者 = @帳號)">
        <SelectParameters>
            <asp:SessionParameter Name="帳號" SessionField="u_id" Type="String" />
        </SelectParameters>        
    </asp:SqlDataSource>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="訂單編號" DataSourceID="SqlDataSource1" ForeColor="#333333" GridLines="None"
         OnRowDataBound="GridView1_RowDataBound" OnRowCommand="GridView1_RowCommand" AllowPaging="true" PageSize="10">
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <Columns>
            <asp:BoundField DataField="訂單編號" HeaderText="訂單編號" InsertVisible="False" HeaderStyle-CssClass="HeaderStylebtn" ReadOnly="True" SortExpression="訂單編號" ItemStyle-Width="100" ItemStyle-Height="30" ItemStyle-HorizontalAlign="Center" >
<ItemStyle Height="30px" Width="100px"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="購買人" HeaderText="購買人" ItemStyle-Width="100" HeaderStyle-CssClass="HeaderStylebtn"  ItemStyle-HorizontalAlign="Center" >
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="下單日期" HeaderText="下單日期" SortExpression="下單日期" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-Width="100" HeaderStyle-CssClass="HeaderStylebtn" ItemStyle-HorizontalAlign="Center">
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="出貨日期" HeaderText="出貨日期" SortExpression="出貨日期" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-Width="100" HeaderStyle-CssClass="HeaderStylebtn" ItemStyle-HorizontalAlign="Center">                        
<ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
            
            <asp:CommandField ShowSelectButton="true" SelectText="訂單明細" HeaderText="訂單明細" ControlStyle-ForeColor="#3399ff" />
           <%-- <asp:HyperLinkField DataNavigateUrlFields="商品編號" DataNavigateUrlFormatString="GoodsDisp.aspx?id={0}" DataTextField="商品編號" DataTextFormatString="商品連結" HeaderText="訂單明細" ItemStyle-Width="100" >
            
<ItemStyle Width="100px"></ItemStyle>
            </asp:HyperLinkField>--%>
            <asp:ButtonField ButtonType="Button" CommandName="ship" Text="出貨" HeaderText="出貨" HeaderStyle-CssClass="HeaderStylebtn" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center" >
                <ItemStyle Width="100px"></ItemStyle>
            </asp:ButtonField>
            
        </Columns>
        <EditRowStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#E9E7E2" />
        <SortedAscendingHeaderStyle BackColor="#506C8C" />
        <SortedDescendingCellStyle BackColor="#FFFDF8" />
        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
    </asp:GridView>
    <br />
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT 訂單.訂單編號,訂單.下單日期,訂單.出貨日期,訂單.帳號 as 買家,收貨地址.*,購買會員.手機 as 買家電話,訂單明細.*,商品.*,會員.*,郵遞區號.*,縣市.* FROM 訂單 
        inner join 收貨地址 on 訂單.訂單編號=收貨地址.訂單編號
		inner join 會員 as 購買會員 on 訂單.帳號=購買會員.帳號
        inner join 訂單明細 on 收貨地址.訂單編號=訂單明細.訂單編號 
        inner join 商品 on 商品.商品編號=訂單明細.商品編號 
        inner join 會員 on 商品.帳號=會員.帳號 
        inner join 郵遞區號 on 收貨地址.郵遞代碼=郵遞區號.郵遞代碼 
        inner join 縣市 on 縣市.縣市代碼=郵遞區號.縣市代碼 where 訂單.訂單編號=@訂單編號">

        <SelectParameters>
            <asp:ControlParameter ControlID="GridView1" Name="訂單編號" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:DetailsView ID="DetailsView1" runat="server" Height="50px" Width="555px" AutoGenerateRows="false" DataSourceID="SqlDataSource2" DataKeyNames="訂單編號" CellPadding="0" HeaderText="訂單明細" HeaderStyle-Font-Size="Medium" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="#5D7B9D" HeaderStyle-ForeColor="White" HeaderStyle-Font-Bold="true"  RowStyle-HorizontalAlign="Center">
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
            <asp:BoundField DataField="訂單編號" HeaderText="訂 單 編 號" ControlStyle-Height="30px" ControlStyle-Width="300px" HeaderStyle-Height="30px" HeaderStyle-Width="100px"/>
            <asp:BoundField DataField="商品名稱" HeaderText="商 品 名 稱" ControlStyle-Height="30px" ControlStyle-Width="300px"/>
            <asp:TemplateField>
                <HeaderTemplate>
                    下 單 日 期
                </HeaderTemplate>
                <ItemTemplate>
                     <asp:Label ID="Label1" runat="server" Text='<%#Eval("下單日期","{0:d}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="帳號" HeaderText="賣 家 帳 號" ControlStyle-Height="30px" ControlStyle-Width="300px"/>
            <asp:BoundField DataField="手機" HeaderText="聯 絡 賣 家" ControlStyle-Height="30px" ControlStyle-Width="300px"/>
            <asp:BoundField DataField="訂購數量" HeaderText="訂 購 數 量" ControlStyle-Height="30px" ControlStyle-Width="300px"/>
            <asp:TemplateField>
                <HeaderTemplate>
                    購 買 價 格
                </HeaderTemplate>
                <ItemTemplate>
                     <asp:Label ID="Label4" runat="server" Text="NT$ "></asp:Label>
                     <asp:Label ID="Label1" runat="server" Text='<%#Eval("購買價格","{0:0}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="買家" HeaderText="買 家 帳 號" ControlStyle-Height="30px" ControlStyle-Width="300px" HeaderStyle-BackColor="#88e1ff" HeaderStyle-ForeColor="#484848"/>
            <asp:BoundField DataField="收貨人" HeaderText="買 家 姓 名" ControlStyle-Height="30px" ControlStyle-Width="300px" HeaderStyle-BackColor="#88e1ff" HeaderStyle-ForeColor="#484848"/>
            <asp:BoundField DataField="買家電話" HeaderText="買 家 電 話" ControlStyle-Height="30px" ControlStyle-Width="300px" HeaderStyle-BackColor="#88e1ff" HeaderStyle-ForeColor="#484848"/>
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

</asp:Content>

