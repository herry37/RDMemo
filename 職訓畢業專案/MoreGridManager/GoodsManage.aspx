<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="GoodsManage.aspx.cs" Inherits="GoodsManage" ValidateRequest="false" MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%--     <script src="../MoreGrid/ckeditor/ckeditor.js"></script>--%>
    <style>
        .display{display:none;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

     <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" 
         SelectCommand="SELECT * FROM [商品類別]"></asp:SqlDataSource>
    <div style="color: #e14286; text-align: left; margin-top: 20px">
        <asp:Label ID="Label3" runat="server" Text="商品管制設定" CssClass="h2"></asp:Label>
    </div>
    <br />
    <asp:Panel ID="Panel1" runat="server">
        <div class="container">
            <div>
                <table style="text-align: center" class="table-condensed">
                    <tr>
                        <td  style="width: 100px">
                           <asp:Label ID="Label2" runat="server" Text="商品類別"></asp:Label>
                        </td>
                        <td style="width: 100px">
                            <asp:Label ID="Label8" runat="server" Text="商品名稱"></asp:Label>
                        </td>

                        <td style="width: 100px">
                            <asp:Label ID="Label4" runat="server" Text="帳　號"></asp:Label>
                        </td>
                        <td style="width: 100px">
                            <asp:Label ID="Label6" runat="server" Text="狀　態"></asp:Label>
                        </td>
                        <td style="width: 70px"></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="DropDownList3" runat="server" DataSourceID="SqlDataSource1" AppendDataBoundItems="true"
                                DataTextField="商品類別名稱" DataValueField="商品類別代碼"  CssClass="input-sm">
                                <asp:ListItem Text="全部" Value="0" Selected="True">
                                </asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="txtName" runat="server" CssClass="input-sm"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAccount" runat="server" CssClass="input-sm"></asp:TextBox>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlstatus" runat="server" CssClass="input-sm">
                                <asp:ListItem Selected="True" Value="0">全部</asp:ListItem>
                                <asp:ListItem Value="True">停權</asp:ListItem>
                                <asp:ListItem Value="False">開放</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Button ID="Button1" runat="server" Text="查詢" CssClass="btn btn-primary" />
                        </td>
                    </tr>
                </table>
                <hr />
            </div>
        </div>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" OnSelected="SqlDataSource2_Selected"
        SelectCommand="SELECT 商品.帳號, 商品.商品名稱, 商品.停權, 商品.商品類別代碼, 商品.商品編號, 商品.上架時間, 商品類別.商品類別名稱 
        FROM 商品 LEFT OUTER JOIN 商品類別 ON 商品.商品類別代碼 = 商品類別.商品類別代碼 
        WHERE ((商品.帳號  LIKE '%' + @帳號 +'%') AND (商品.商品名稱 LIKE '%' + @商品名稱 + '%') 
        AND ((@商品類別代碼='0') OR(商品.商品類別代碼 = @商品類別代碼))
        AND ((@停權='0') OR (停權=@停權))) order by 商品.商品編號 desc"
        UpdateCommand="UPDATE [商品] SET  [停權] = @停權 WHERE [商品編號] = @商品編號">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtAccount" Name="帳號" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false"/>
            <asp:ControlParameter ControlID="txtName" Name="商品名稱" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false"/>
            <asp:ControlParameter ControlID="DropDownList3" Name="商品類別代碼" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="ddlstatus" Name="停權" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="停權" Type="Boolean" />
            <asp:Parameter Name="商品編號" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
   <asp:Label ID="Label12" runat="server" Text='' ForeColor="Green"></asp:Label>
    <asp:GridView ID="GridView1" runat="server" DataSourceID="SqlDataSource2" AutoGenerateColumns="False" DataKeyNames="商品編號" CssClass="table-bordered table-condensed"
        BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" 
        OnRowCommand="GridView1_RowCommand" AllowPaging="True" PageSize="15" OnDataBound="GridView1_DataBound">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:BoundField DataField="帳號" HeaderText="帳號" SortExpression="帳號"  ReadOnly="True" ItemStyle-Width="100">
            </asp:BoundField>
            <asp:BoundField DataField="商品編號" HeaderText="商品編號" InsertVisible="False" ReadOnly="True" SortExpression="商品編號" ItemStyle-CssClass="display" HeaderStyle-CssClass="display"/>
            <asp:BoundField DataField="商品類別代碼" HeaderText="商品類別代碼" SortExpression="商品類別代碼" Visible="false" />
            <asp:BoundField DataField="商品類別名稱" HeaderText="商品類別名稱" SortExpression="商品類別名稱"  ReadOnly="True" ItemStyle-Width="170">
            </asp:BoundField>
            <asp:BoundField DataField="商品名稱" HeaderText="商品名稱" SortExpression="商品名稱"  ReadOnly="True" ItemStyle-Width="140">
            </asp:BoundField>
            <asp:BoundField DataField="上架時間" HeaderText="上架時間" SortExpression="上架時間"  ReadOnly="True" ItemStyle-Width="200">
            </asp:BoundField>
            <asp:CheckBoxField DataField="停權" HeaderText="停權" SortExpression="停權" ItemStyle-Width="50">
            </asp:CheckBoxField>
            <asp:CommandField ShowEditButton="True" ItemStyle-Width="100" SelectText="查詢" ShowSelectButton="True" >
            </asp:CommandField>
        </Columns>
        <PagerTemplate>
            <table style="width: 100%;">
                <tr>
                    <td style="width: 70px"></td>
                    <td style="text-align:center">
                        <asp:Label ID="Label5" runat="server" Text="請選擇頁碼"></asp:Label>
                        <asp:LinkButton ID="Linkpage1" runat="server" Font-Names="Webdings" Font-Underline="false" OnClick="page_SelectedIndexChanged" ForeColor="White">3</asp:LinkButton>
                        <asp:DropDownList ID="ddlpage" AutoPostBack="true" runat="server" OnSelectedIndexChanged="page_SelectedIndexChanged" ForeColor="Black"></asp:DropDownList>
                        <asp:LinkButton ID="Linkpage2" runat="server" Font-Names="Webdings" Font-Underline="false" OnClick="page_SelectedIndexChanged" ForeColor="White">4</asp:LinkButton>
                    </td>
                    <td style="text-align: right">
                        <asp:Label ID="labNum" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </table>
        </PagerTemplate>
        <FooterStyle BackColor="#CCCC99" />
        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#6B696B" ForeColor="White" HorizontalAlign="Right" />
        <RowStyle BackColor="#F7F7DE" />
<%--        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#FBFBF2" />
        <SortedAscendingHeaderStyle BackColor="#848384" />
        <SortedDescendingCellStyle BackColor="#EAEAD3" />
        <SortedDescendingHeaderStyle BackColor="#575357" />--%>
    </asp:GridView>
   <br />
        </asp:Panel>
    <asp:Panel ID="Panel2" runat="server">
        <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" 
            SelectCommand="SELECT 商品.商品編號, 商品.帳號, 商品.商品名稱, 商品.商品摘要, 商品.商品明細, 商品.庫存量, 商品.單價, 商品.上架時間, 商品.下架時間, 商品.商品小圖片, 商品.商品類別代碼, 商品.停權, 商品類別.商品類別名稱, 商品.商品大圖片1, 商品.商品大圖片2, 商品.商品大圖片3, 商品.編輯 FROM 商品 INNER JOIN 商品類別 ON 商品.商品類別代碼 = 商品類別.商品類別代碼 WHERE (商品.商品編號 = @商品編號)">
            <SelectParameters>
                <asp:ControlParameter ControlID="GridView1" Name="商品編號" PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:FormView ID="FormView1" runat="server" DataKeyNames="商品編號" DataSourceID="SqlDataSource3" OnDataBound="FormView1_DataBound">
<%--            <EditItemTemplate>
                商品編號:
                <asp:Label ID="商品編號Label1" runat="server" Text='<%# Eval("商品編號") %>' />
                <br />
                帳號:
                <asp:TextBox ID="帳號TextBox" runat="server" Text='<%# Bind("帳號") %>' />
                <br />
                商品名稱:
                <asp:TextBox ID="商品名稱TextBox" runat="server" Text='<%# Bind("商品名稱") %>' />
                <br />
                商品摘要:
                <asp:TextBox ID="商品摘要TextBox" runat="server" Text='<%# Bind("商品摘要") %>' />
                <br />
                商品明細:
                <asp:TextBox ID="商品明細TextBox" runat="server" Text='<%# Bind("商品明細") %>' />
                <br />
                庫存量:
                <asp:TextBox ID="庫存量TextBox" runat="server" Text='<%# Bind("庫存量") %>' />
                <br />
                單價:
                <asp:TextBox ID="單價TextBox" runat="server" Text='<%# Bind("單價") %>' />
                <br />
                上架時間:
                <asp:TextBox ID="上架時間TextBox" runat="server" Text='<%# Bind("上架時間") %>' />
                <br />
                下架時間:
                <asp:TextBox ID="下架時間TextBox" runat="server" Text='<%# Bind("下架時間") %>' />
                <br />
                商品小圖片:
                <asp:TextBox ID="商品小圖片TextBox" runat="server" Text='<%# Bind("商品小圖片") %>' />
                <br />
                商品類別代碼:
                <asp:TextBox ID="商品類別代碼TextBox" runat="server" Text='<%# Bind("商品類別代碼") %>' />
                <br />
                停權:
                <asp:CheckBox ID="停權CheckBox" runat="server" Checked='<%# Bind("停權") %>' />
                <br />
                商品類別名稱:
                <asp:TextBox ID="商品類別名稱TextBox" runat="server" Text='<%# Bind("商品類別名稱") %>' />
                <br />
                商品大圖片1:
                <asp:TextBox ID="商品大圖片1TextBox" runat="server" Text='<%# Bind("商品大圖片1") %>' />
                <br />
                商品大圖片2:
                <asp:TextBox ID="商品大圖片2TextBox" runat="server" Text='<%# Bind("商品大圖片2") %>' />
                <br />
                商品大圖片3:
                <asp:TextBox ID="商品大圖片3TextBox" runat="server" Text='<%# Bind("商品大圖片3") %>' />
                <br />
                編輯:
                <asp:TextBox ID="編輯TextBox" runat="server" Text='<%# Bind("編輯") %>' />
                <br />
                <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update" Text="更新" />
                &nbsp;<asp:LinkButton ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消" />
            </EditItemTemplate>--%>
            <EditItemTemplate></EditItemTemplate>
            <ItemTemplate>
                <table class="table-bordered table-condensed">
                    <tr>
                        <td rowspan="7" colspan="2">
<%--                            <img src="../MoreGrid/goodPicture/<%# Eval("商品大圖片1")%>" width="400"/>--%>
                            <asp:Image ID="Image1" runat="server"  ImageUrl='<%# "../MoreGrid/goodPicture/"+check_null("商品大圖片1")%>' Height="400" Width="400"/>
                        </td>
                        <td>
<%--                            <img src="../MoreGrid/goodPicture/<%# Eval("商品小圖片")%>" width="50"/>--%>
                             <asp:Image ID="Image2" runat="server" ImageUrl='<%# "../MoreGrid/goodPicture/"+check_null("商品小圖片")%>' Width="100" Height="100" />
                        </td>
                    </tr>
                    <tr>
                        <td>帳號:
                <asp:Label ID="帳號Label" runat="server" Text='<%# Eval("帳號") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td>商品類別名稱:
                <asp:Label ID="商品類別名稱Label" runat="server" Text='<%# Eval("商品類別名稱") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td>商品名稱:
                <asp:Label ID="商品名稱Label" runat="server" Text='<%# Eval("商品名稱") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td>商品摘要:
                <asp:Label ID="商品摘要Label" runat="server" Text='<%# Eval("商品摘要") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td>商品明細:
                <asp:Label ID="商品明細Label" runat="server" Text='<%# Eval("商品明細") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td>單價:
                <asp:Label ID="單價Label" runat="server" Text='<%# Eval("單價") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td rowspan="4">
<%--                           <img src="../MoreGrid/goodPicture/<%# Eval("商品大圖片2")%>" width="200"/>--%>
                           <asp:Image ID="Image3" runat="server" ImageUrl='<%# "../MoreGrid/goodPicture/"+check_null("商品大圖片2")%>' Height="200" Width="200"/>


                        </td>
                        <td rowspan="4">
<%--                             <img src="../MoreGrid/goodPicture/<%# Eval("商品大圖片3")%>" width="200"/>--%>
<%--                              <asp:Image ID="Image4" runat="server" ImageUrl='<%# "../MoreGrid/goodPicture/"+Eval("商品大圖片3")%>'  Height="200" Width="200" />--%>
                        <asp:Image ID="Image4" runat="server" ImageUrl='<%# "../MoreGrid/goodPicture/"+check_null("商品大圖片3")%>'  Height="200" Width="200" />

                        </td>
                        <td>庫存量:
                <asp:Label ID="庫存量Label" runat="server" Text='<%# Eval("庫存量") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td>上架時間:
                <asp:Label ID="上架時間Label" runat="server" Text='<%# Eval("上架時間") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td>下架時間:
                <asp:Label ID="下架時間Label" runat="server" Text='<%# Eval("下架時間") %>' />

                        </td>
                    </tr>
                    <tr>
                        <td>停權:
                <asp:CheckBox ID="停權CheckBox" runat="server" Checked='<%# Eval("停權") %>' Enabled="false" />

                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                編輯:
                            <asp:Label ID="LabEdit" runat="server" Text='<%# Eval("編輯") %>' ></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="Button1_Click">回上頁</asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <asp:Label ID="商品編號Label" runat="server" Text='<%# Eval("商品編號") %>'  Visible="false"/>
                <asp:Label ID="商品類別代碼Label" runat="server" Text='<%# Eval("商品類別代碼") %>' Visible="false" />
                <br />
            </ItemTemplate>
        </asp:FormView>
    </asp:Panel>
</asp:Content>

