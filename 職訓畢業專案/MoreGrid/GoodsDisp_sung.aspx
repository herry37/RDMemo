<%@ Page Title="" Language="C#" MasterPageFile="Top2.master" AutoEventWireup="true" CodeFile="GoodsDisp.aspx.cs" Inherits="GoodsDisp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" runat="Server">

    <link href="Mystyle/StyleMessageBoard.css" rel="stylesheet" />
    <link href="css/bootstrap.css" rel="stylesheet" />
    <script src="Scripts/jquery-1.12.1.js"></script>
    <script src="js/bootstrap.js"></script>
    <script>
        $(document).ready(function () {
            var maxChar = 250;
            $('#txt_areal').text(maxChar);
            $('#left_SiteDisp_txtGcontent').keydown(function (event) {
                var msg = $(this).val().length;
                $('#txt_areal').text(maxChar - msg);
                if (maxChar - msg == 0) {
                    event.preventDefault();
                }
            });
            });
    
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SiteDisp" runat="Server">
    <div>            
        <asp:HyperLink ID="HyperLink1" runat="server" CssClass="btn btn-default"></asp:HyperLink>
         <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [商品] WHERE ([商品編號] = @id)">
            <SelectParameters>
                <asp:QueryStringParameter Name="id" QueryStringField="id" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:DataList ID="DataList1" runat="server" DataSourceID="SqlDataSource1" >
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" Text='<%# Eval("商品編號") %>'></asp:Label>               
                <a href="GoodsSHipper.aspx?id=<%# Eval("商品編號")%>"> 
                <asp:Label ID="Label2" runat="server" Text="我要購買"></asp:Label>
            </ItemTemplate>
        </asp:DataList>            
       
        <asp:Button ID="btShoppingcar" runat="server" Text="加入購物車" OnClick="btShoppingcar_Click" />
       
        
    </div>

            
<%-- 以下留言板 --%>

        <asp:SqlDataSource ID="SqlDataMessage" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
            SelectCommand="select 留言版.*,回覆留言.* from 留言版 left join 回覆留言 on 留言版.留言編號=回覆留言.主留言編號"></asp:SqlDataSource>
        <asp:Panel runat="server" Width="987px">
            <asp:Repeater ID="RepeaterMessage" runat="server" DataSourceID="SqlDataMessage">
                <ItemTemplate>
                    <table class="auto-style4" border="0" style="border-collapse: collapse">
                        <tr style="background-color: #FFFF66;">
                            <td class="auto-style2" style="background-color: #FFFF66; height: 40px">&nbsp;問題 &nbsp;<asp:Label runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label></td>
                            <td>&nbsp;<asp:Label runat="server" Text='<%#Eval("帳號") %>'></asp:Label></td>
                            <%--留言者帳號--%>
                            <td class="auto-style3">&nbsp;</td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr style="background-color: #FFFF66;">
                            <td class="auto-style2">&nbsp;</td>
                            <td colspan="3">
                                <asp:Label runat="server" Text='<%# Eval("內容").ToString().Replace("\n","<br>") %>'></asp:Label>

                            </td>
                        </tr>
                        <tr style="background-color: #FFFF66;">
                            <td class="auto-style2">&nbsp;</td>
                            <td>&nbsp;</td>
                            <td class="auto-style3" style="text-align: right">
                                <asp:Label runat="server" Text='<%#Eval("留言時間") %>'></asp:Label></td>
                            <td>&nbsp;</td>
                        </tr>
                        <%-- <tr style="background-color:#FFFF66;">
                            <td class="auto-style2">&nbsp;</td>
                            <td>&nbsp;</td>
                            <td class="auto-style3">&nbsp;</td>
                            <td style="text-align:center;height:40px"><a href='GoodsDispR.aspx?id=<%# Eval("留言編號") %>'>回覆</a></td>
                            <asp:LinkButton ID="LBttnReply" runat="server" Visible="false"><a href='GoodsDispR.aspx?id=<%# Eval("留言編號") %>'>回覆</a></asp:LinkButton>
                        </tr>--%>
                        <tr>
                            <td class="auto-style2">答覆</td>
                            <td>&nbsp;</td>
                            <td class="auto-style3">&nbsp;</td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="auto-style2">&nbsp;</td>
                            <td colspan="3">
                                <asp:Label runat="server" Text='<%# Eval("回覆內容").ToString().Replace("\n","<br>") %>'></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="auto-style2">&nbsp;</td>
                            <td>&nbsp;</td>
                            <td class="auto-style3" style="text-align: right">
                                <asp:Label runat="server" Text='<%#Eval("回覆時間") %>'></asp:Label></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="auto-style2">&nbsp;</td>
                            <td>&nbsp;</td>
                            <td class="auto-style3">&nbsp;</td>
                            <td>&nbsp;</td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

        <div>
            <asp:Panel ID="Panelmessage" runat="server" Width="988px">
                &nbsp 提出問題：(為避免個人資料遭有人士利用，請勿在提問內容填寫個人相關資料，如:姓名、銀行帳戶..等。)        
            </asp:Panel>
            <asp:Panel ID="Panelmessage1" runat="server" Width="999px">
                <br />
                <br />
                &nbsp &nbsp &nbsp &nbsp<label id="txt_area">您可以輸入&nbsp<span id="txt_areal"></span>字元</label>
                <table id="add_book">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtGcontent" runat="server" TextMode="MultiLine" Rows="15" Columns="50"></asp:TextBox>
                  <asp:RegularExpressionValidator ControlToValidate="txtGcontent" runat="server" ErrorMessage="只能輸入250字數" Font-Bold="true" ForeColor="Red" ValidationExpression=".{1,250}"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="ButMessage" runat="server" Text="提出問題" OnClick="ButMessage_Click" />
                            <asp:Button ID="ButMessageDel" runat="server" Text="重新填寫" OnClick="ButMessageDel_Click" CausesValidation="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>


        </div>

   

</asp:Content>

