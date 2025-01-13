<%@ Page Title="" Language="C#" MasterPageFile="Top2.master" AutoEventWireup="true" CodeFile="GoodsDisp.aspx.cs" Inherits="GoodsDisp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" runat="Server">
    <link href="Mystyle/StyleMessageBoard.css" rel="stylesheet" />


    <script>
        $(document).ready(function () {
            var maxChar = 250;
            $('#txt_areal').text(maxChar);
            $('#left_SiteDisp_txtGcontent').keydown(function (event) {
                var msg = $(this).val().length;
                $('#txt_areal').text(maxChar - msg);
           
            });

            $('img[src="goodPicture/"]').remove();

          var maxChar1 = 250;
          $('#txt_areal3').text(maxChar1);
            $('#left_SiteDisp_txtGcontent1').keydown(function (event) {
                var msg1 = $(this).val().length;
                $('#txt_areal3').text(maxChar1 - msg1);               
        });
        
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SiteDisp" runat="Server">
    <div>            
        <asp:HyperLink ID="hlShop" runat="server" CssClass="btn btn-default" Text="同店家的商品"></asp:HyperLink>
         
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [商品] WHERE ([商品編號] = @id)">
            <SelectParameters>
                <asp:QueryStringParameter Name="id" QueryStringField="id" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:DataList ID="DataList1" runat="server" DataSourceID="SqlDataSource1" OnItemDataBound="DataList1_ItemDataBound" >
            <ItemTemplate>
                <div>
                    <h1><asp:Label ID="Label3" runat="server" Text='<%# Eval("商品名稱") %>'></asp:Label></h1>                    
                </div>
                <div>
                    <table>
                        <tr>
                            <td style="height:500px; width:500px;">
                                <img src="goodPicture/<%# Eval("商品大圖片1") %>"  style="max-height:500px; max-width:500px;"/>
                            </td>                        
                            <td>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("商品明細").ToString().Replace("\n","<br>") %>' style="font-size:18pt;"></asp:Label>
                                <br /><br />

                                <asp:Label ID="Label5" runat="server" Text='網路價：$' ForeColor="#ff0066" Font-Size="24"></asp:Label>                                
                                <asp:Label ID="Label6" runat="server" Text='<%# Convert.ToInt64(Eval("單價")).ToString("#,#")  %>' ForeColor="#ff0066" Font-Size="28"></asp:Label>   
                                <%--<a href="GoodsSHipper.aspx?id=<%# Eval("商品編號")%>">  </a>  用超連結在網站間傳值的方法 --%>   
                                 <br /><br />   
                                <asp:Button ID="btShipper" runat="server" Text="我要購買" OnClick="btShipper_Click" CssClass="btn btn-primary " />                           
                                <asp:Button ID="btShoppingcar" runat="server" Text="加入購物車" OnClick="btShoppingcar_Click" CssClass="btn  btn-default " />                    
                            </td>                          
                        </tr>                     
                    </table>
                    
                </div>  
                
                <div>
                    <img src="goodPicture/<%# Eval("商品大圖片2") %>"  style="max-height:500px; max-width:500px;"/>
                </div>
                <div>
                    <img src="goodPicture/<%# Eval("商品大圖片3") %>"  style="max-height:500px; max-width:500px;"/>
                </div>
                <div>
                    <asp:Label ID="lbEdit" runat="server" Text='<%# Bind("編輯") %>'></asp:Label>
                    <asp:Label ID="lbA" runat="server"></asp:Label>
                </div>
                
                
            </ItemTemplate>
        </asp:DataList>            
       
        
       
        
    </div>

            
<%-- 以下留言板 --%>

        <asp:SqlDataSource ID="SqlDataMessage" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
            SelectCommand="select 留言版.*,回覆留言.* from 留言版 left join 回覆留言 on 留言版.留言編號=回覆留言.留言編號 WHERE ([商品編號] = @id)">
            <SelectParameters>
                <asp:QueryStringParameter Name="id" QueryStringField="id" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:Panel runat="server" Width="987px">
            <asp:Repeater ID="RepeaterMessage" runat="server" DataSourceID="SqlDataMessage" OnItemCommand="RepeaterMessage_ItemCommand" 
                 OnItemDataBound="RepeaterMessage_ItemDataBound">
                <ItemTemplate>           

                    <table class="auto-style4" border="0" style="border-collapse: collapse;">
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
                                <asp:Label runat="server" Text='<%# Eval("內容").ToString().Replace("\n","<br>") %>'></asp:Label></td>
                        </tr>
                        <tr style="background-color: #FFFF66;">
                            <td class="auto-style2">&nbsp;</td>
                            <td>&nbsp;</td>
                            <td class="auto-style3" style="text-align: right">
                                <asp:Label runat="server" Text='<%#Eval("留言時間") %>'></asp:Label></td>
                            <td>&nbsp;</td>
                        </tr>
                         <tr style="background-color:#FFFF66;">
                            <td class="auto-style2">&nbsp;</td>
                            <td>&nbsp;</td>
                            <td class="auto-style3">&nbsp;</td>                      
                             <td style="text-align:center;height:40px">
                            <%--<asp:LinkButton ID="LBttnReply" runat="server" Visible="false"><a href='GoodsDispR.aspx?id=<%# Eval("留言編號") %>'>回覆</a></asp:LinkButton>--%>
                             <asp:Button ID="ButR" Visible="false" runat="server" Text="回覆" CommandName="Re" CommandArgument='<%#Eval("留言編號") %>' /></td>
                        </tr>
                        <tr>
                            <td class="auto-style2">答覆</td>
                            <td>&nbsp;</td>
                            <td class="auto-style3">&nbsp;</td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="auto-style2">&nbsp;</td>
                            <td colspan="3" style="word-break:break-all;">
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
            <asp:Panel runat="server" ID="PanelMessage4">
            <asp:Panel ID="Panelmessage" runat="server" Width="988px">
                &nbsp;&nbsp;&nbsp 
                提出問題：(為避免個人資料遭有人士利用，請勿在提問內容填寫個人相關資料，如:姓名、銀行帳戶..等。)        
            </asp:Panel>
            <asp:Panel ID="Panelmessage1" runat="server" Width="999px">
                <br />
                <br />
                &nbsp &nbsp &nbsp &nbsp<label id="txt_area">您可以輸入&nbsp<span id="txt_areal"></span>字元</label>
                <table id="add_book">
                    <tr>
                        <td>
                            <%--<textarea id="txtGcontent" cols="50" rows="15" maxlength="250"></textarea>--%>
                            <asp:TextBox ID="txtGcontent" runat="server" TextMode="MultiLine" Rows="15" Columns="50"></asp:TextBox>
                            <%--<asp:RequiredFieldValidator ID="RFVmessageBoard" ControlToValidate="txtGcontent" ForeColor="red" runat="server" ErrorMessage="(必填)" Font-Size="10pt"></asp:RequiredFieldValidator>--%>
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
            <br /><br /><br /></asp:Panel>
        </div>
      <%--回覆留言--%>

    <asp:Panel runat="server" ID="PanelMessageR" Visible="false">
           <div>
            <asp:Panel ID="PanelmessageR1" runat="server" Width="988px">
                &nbsp 回覆問題：(為避免個人資料遭有人士利用，請勿在提問內容填寫個人相關資料，如:姓名、銀行帳戶..等。)        
            </asp:Panel>
            <asp:Panel ID="PanelmessageR3" runat="server" Width="999px">
                <br />
                <br /> 
                &nbsp &nbsp &nbsp &nbsp<label id="txt_area2">您可以輸入&nbsp<span id="txt_areal3"></span>字元</label><br />
                <asp:Label runat="server" Font-Size="24px" ForeColor="Red" Font-Bold="true" ID="errMessage"></asp:Label><br />
                <table id="add_book1" style="margin:auto">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtGcontent1" runat="server" TextMode="MultiLine" Rows="15" Columns="50"></asp:TextBox>
                  <%--<asp:RegularExpressionValidator ControlToValidate="txtGcontent1" runat="server" ErrorMessage="只能輸入250字數" Font-Bold="true" ForeColor="Red" ValidationExpression=".{1,250}"></asp:RegularExpressionValidator>--%>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:center;background-color: #507CD1">
                            <asp:Button ID="Button1" runat="server" Text="提出問題" OnClick="ButMessage_Click1" />
                            <asp:Button ID="Button2" runat="server" Text="重新填寫" CausesValidation="false" OnClick="ButMessageDel_Click1" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>               
       </div>   <br /><br /><br />
</asp:Panel>
    
</asp:Content>

