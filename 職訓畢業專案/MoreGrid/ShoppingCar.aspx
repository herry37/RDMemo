<%@ Page Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="ShoppingCar.aspx.cs" Inherits="ShopingCar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
   <style>
       .shoppingCar-container{
            margin: 0 auto;
                      
            width:1000px;
          
            border-radius:2px;
            background-color: #F7F7F7;
        }
   </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="left" Runat="Server">
    
   
  
    <div class="shoppingCar-container">     
        <asp:Button ID="Button1" runat="server" Text="刪除" OnClick="btDelete_Click" CssClass="btn btn-warning btn-lg"  />
        <asp:Button ID="Button2" runat="server" Text="清空購物車" OnClientClick="return confirm('您確認要刪除購物車嗎?');" OnClick="btclear_Click" CssClass="btn btn-danger btn-lg" />
        <br /><br />
        <asp:Label ID="Labeltxt5" runat="server" Text="目前購物車中尚無商品..." Font-Size="Larger" ForeColor="Red" Font-Bold="true" Visible="false" ></asp:Label>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"  ></asp:SqlDataSource>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="商品編號" DataSourceID="SqlDataSource1"  BackColor="White" BorderColor="#336666" BorderStyle="None" BorderWidth="3px" CellPadding="4" GridLines="Horizontal" Width="800px"  >
            <Columns>              
                <asp:TemplateField  >
                    <ItemTemplate >                       
                      <asp:CheckBox  runat="server" ID="cbCheck" Text=' <%#Eval("商品編號") %>' Font-Size="0"></asp:CheckBox>                                                                                                                                                                                                                                      
                    </ItemTemplate>                               
                </asp:TemplateField>
                 <asp:TemplateField>
                    <ItemTemplate>                                                               
                        <img src="goodPicture/<%# Eval("商品小圖片") %>" style="width:50%; height:50%" />                                                                                                                                                                                          
                    </ItemTemplate>                               
                </asp:TemplateField>  
                <asp:BoundField DataField="商品名稱" HeaderText="商品名稱" SortExpression="商品名稱" ItemStyle-ForeColor="#336666"  ItemStyle-Font-Size="Large" ItemStyle-Font-Bold="true"/>
                 <asp:TemplateField HeaderText="庫存量" >
                    <ItemTemplate >                       
                       <asp:Label ID="Labeltxt1" runat="server" Text=' <%#Eval("庫存量") %>' ></asp:Label>    
                       <asp:Label ID="Labeltxt2" runat="server" Text="個"></asp:Label>                                                                                                                                                                                                                               
                    </ItemTemplate>                               
                </asp:TemplateField>
                <%--<asp:BoundField DataField="庫存量" HeaderText="庫存量" SortExpression="庫存量" />--%>
                  <asp:TemplateField HeaderText="單價">
                    <ItemTemplate>                                                                  
                        <asp:Label ID="Labeltxt3" runat="server" Text="$" ForeColor="#ff00ff" Font-Bold="true"></asp:Label> 
                        <asp:Label ID="Labeltxt4" runat="server" Text='<%# Convert.ToInt64(Eval("單價")).ToString("#,#")  %>'  ForeColor="#ff00ff" Font-Bold="true"></asp:Label>                                                                                                                                                                               
                    </ItemTemplate>                                                                                                                                                                                                                                                                                
                </asp:TemplateField> 
                   
               
                <asp:HyperLinkField HeaderText="結帳" Text="結帳" ControlStyle-CssClass="btn btn-primary"
                     DataTextFormatString="結帳"
                     DataNavigateUrlFormatString="GoodsShipper.aspx?id={0}"  
                      DataTextField="商品編號" DataNavigateUrlFields="商品編號" />
             
                 </Columns> 
                
            <FooterStyle BackColor="White" ForeColor="#333333" />
            <HeaderStyle BackColor="#336666" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="White" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F7F7F7" />
            <SortedAscendingHeaderStyle BackColor="#487575" />
            <SortedDescendingCellStyle BackColor="#E5E5E5" />
            <SortedDescendingHeaderStyle BackColor="#275353" />
                
        </asp:GridView>
        <br />
        <asp:Button runat="server" Text="刪除" ID="btDelete" OnClick="btDelete_Click" CssClass="btn btn-warning btn-lg" />
        <asp:Button runat="server" Text="清空購物車" ID="btclear" OnClientClick="return confirm('您確認要刪除購物車嗎?');" OnClick="btclear_Click" CssClass="btn btn-danger btn-lg" /> 
        <%--<asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="Main.aspx">回到主頁面</asp:LinkButton>--%>
    </div>
    </asp:Content>
