<%@ Page Title="" Language="C#" MasterPageFile="Top2.master" AutoEventWireup="true" CodeFile="GoodsShipper.aspx.cs" Inherits="GoodsShipper1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" runat="Server">
    <link href="css/bootstrap.css" rel="stylesheet" />
    <script src="Scripts/jquery-1.12.1.js"></script>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <style type="text/css">
        .auto-style1 {
            width: 175px;
        }

        .auto-style2 {
            height: 34px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SiteDisp" runat="Server">
    <div>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
            SelectCommand="SELECT * FROM [商品] WHERE  商品編號=@id">
            <SelectParameters>
                <asp:QueryStringParameter Name="id" QueryStringField="id" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>

        <%-- 顯示圖片 --%>
        <asp:Repeater ID="Repeater1" runat="server" DataSourceID="SqlDataSource1">
            <ItemTemplate>
                <div>
                    <asp:Label ID="lbstate" runat="server" Text='<%# Eval("商品名稱") %>' Font-Bold="true" Font-Names="微軟正黑體" Font-Size="18" />
                    <div>
                        <table id="table">
                            <%--style="width: 100%;--%>
                            <tr>
                                <td style="width: 300px; vertical-align: text-top;">
                                    <img src="goodPicture/<%# Eval("商品小圖片") %>" style="border: 1px solid;" />
                                </td>

                                <td style="width: 500px; border: 1px solid #e8e5e5; text-align: left;">
                                    <p style="list-style-type: none; background-color: #FFFFDE; text-align: left;">
                                        <asp:Label ID="Label4" runat="server" Text="售價：" Font-Size="15" />
                                        <asp:Label ID="Label5" runat="server" Text="$" Font-Size="15" ForeColor="Red" />
                                        <asp:Label ID="lbPprice" runat="server" Text='<%# Convert.ToInt64(Eval("單價")).ToString("#,#")%>' />
                                        <br></br>
                                        <asp:Label ID="Label7" runat="server" Text="尚餘："></asp:Label>
                                        <asp:Label ID="Label8" runat="server" Text='<%# Eval("庫存量")%>'></asp:Label>
                                        <asp:Label ID="Label9" runat="server" Text="個"></asp:Label>
                                        <br></br>
                                    </p>
                                </td>
                            </tr>
                        </table>
            </ItemTemplate>
        </asp:Repeater>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <h2>請填寫出貨訂單</h2>

        <asp:FormView ID="FormView1" runat="server" DefaultMode="Insert" Width="1000px" OnItemInserting="FormView1_ItemInserting"
            OnItemCommand="FormView1_ItemCommand" EditRowStyle-Font-Size="14"
            EditRowStyle-Font-Bold="true">

            <EditRowStyle Font-Bold="True" Font-Size="14pt"></EditRowStyle>

            <InsertItemTemplate>
                <table>
                    <tr>
                        <td class="auto-style1">
                            <asp:Label ID="Label1" runat="server" Text="收件人："></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="tbReceiver" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="tbReceiver"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ControlToValidate="tbReceiver" Display="Dynamic" runat="server" ErrorMessage="只能輸入30字數" Font-Bold="true" ForeColor="Red" ValidationExpression=".{0,30}"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                      
                        <td class="auto-style1">
                            <asp:Label ID="Label2" runat="server" Text="數量："></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="tbNum" runat="server" OnTextChanged="tbNum_TextChanged" AutoPostBack="true"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="tbNum"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ControlToValidate="tbNum" Display="Dynamic" runat="server" ErrorMessage="只能輸入字數" Font-Bold="true" ForeColor="Red" ValidationExpression="[\d]"></asp:RegularExpressionValidator>

                        </td>
                    </tr> 
                    <tr>
                        <td class="auto-style1">
                            <asp:Label ID="Label10" runat="server" Text="運費："></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="tbShippment" runat="server" Text="" OnTextChanged="tbNum_TextChanged" AutoPostBack="true"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="tbShippment"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ControlToValidate="tbShippment" Display="Dynamic" runat="server" ErrorMessage="只能輸入數字" Font-Bold="true" ForeColor="Red" ValidationExpression="^\d*\.?\d*$"></asp:RegularExpressionValidator>
                        </td>
                    </tr> 
                    <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                        SelectCommand="SELECT * FROM [商品] WHERE  商品編號=@id">
                        <SelectParameters>
                            <asp:QueryStringParameter Name="id" QueryStringField="id" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <tr>
                        <td class="auto-style1">
                            <asp:Label ID="Label3" runat="server" Text="價格"></asp:Label>
                        </td>
                        <td>
                            <%-- <asp:TextBox ID="tbPrice" runat="server"></asp:TextBox>--%>
                            <asp:Repeater ID="Repeater2" runat="server" DataSourceID="SqlDataSource4">
                                <ItemTemplate>
                                    <asp:Label ID="Label12" runat="server" Text="$"></asp:Label>
                                    <asp:Label ID="lbprice" runat="server" Text='<%# Convert.ToInt64(Eval("單價")).ToString("#,#")  %>'></asp:Label>
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style1">
                            <asp:Label ID="Label11" runat="server" Text="合計："></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="$" ForeColor="#ff3399" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbSum" runat="server" ForeColor="#ff3399" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
             
                            <tr>
                                <td class="auto-style1">
                                    <asp:Label ID="Label4" runat="server" Text="收貨地址"></asp:Label>
                                </td>
                                <td>

                                    <asp:DropDownList ID="ddlcity" runat="server" DataSourceID="SqlDataSource2" DataTextField="縣市"
                                        DataValueField="縣市代碼"
                                        AutoPostBack="true" AppendDataBoundItems="true"
                                        OnSelectedIndexChanged="ddlcity_SelectedIndexChanged">
                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator10" ControlToValidate="ddlcity" runat="server"
                                        ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic"
                                        InitialValue="0"></asp:RequiredFieldValidator>

                                    <asp:DropDownList ID="ddlarea" runat="server" DataSourceID="SqlDataSource3" DataTextField="鄉鎮市區"
                                        DataValueField="郵遞代碼"
                                        AppendDataBoundItems="true">
                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator11" ControlToValidate="ddlarea" runat="server" ErrorMessage="必填欄位" ForeColor="Red" Display="Dynamic" InitialValue="0"></asp:RequiredFieldValidator>
              

                    <asp:TextBox ID="txtAddress" runat="server" MaxLength="50" Width="300px"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtAddress"></asp:RequiredFieldValidator>
   
                    </td> 

                            </tr>
                            <tr>
                                <td class="auto-style1">
                                    <asp:Label ID="Label6" runat="server" Text="Label">給賣家的話</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="tbMessage" runat="server" TextMode="MultiLine" Rows="15" Columns="50"></asp:TextBox>
                                </td>
                            </tr>
  
                    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                        SelectCommand="SELECT [縣市代碼], [縣市] FROM [縣市]  WHERE ([顯示] = @顯示) ">
                        <SelectParameters>
                            <asp:Parameter DefaultValue="true" Name="顯示" Type="Boolean" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                        SelectCommand="SELECT [郵遞代碼], [郵遞區號], [鄉鎮市區] FROM [郵遞區號] WHERE ([縣市代碼] = @縣市代碼)">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="ddlcity" Name="縣市代碼" PropertyName="SelectedValue" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>


                    <tr style="text-align: center">
                        <td class="auto-style1">&nbsp;<asp:LinkButton ID="ButInsert" runat="server" CausesValidation="true" CssClass="btn btn-primary btn-lg active"
                            CommandName="Insert" Text="新增" OnClientClick="return confirm('確定要購買嗎?')" /></td>
                        <td>&nbsp;<asp:LinkButton ID="ButCancel" runat="server" CausesValidation="false"
                            CommandName="Cancel" Text="取消" CssClass="btn btn-danger btn-lg active"></asp:LinkButton></td>
                    </tr>
                </table>
            </InsertItemTemplate>
        </asp:FormView>

    </div>
</asp:Content>

