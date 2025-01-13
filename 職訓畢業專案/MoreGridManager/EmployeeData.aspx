<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="EmployeeData.aspx.cs" Inherits="EmployeeData" 
    MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
<link href="css/bootstrap.css" rel="stylesheet" />
<%--    <link rel="stylesheet" href="http://code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css" />
    <script src="http://code.jquery.com/jquery-2.1.4.js"></script>
    <script src="http://code.jquery.com/ui/1.11.4/jquery-ui.js"></script>--%>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [部門]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [職稱]"></asp:SqlDataSource>
    <div style="color: #e14286; text-align: left; margin-top: 20px">
        <asp:Label ID="Label11" runat="server" Text="員工資料管理" CssClass="h2"></asp:Label>
    </div>
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <asp:Panel ID="Panel1" runat="server">
                <div class="container">

                    <div>

                        <table style="text-align: center" class="table-condensed">
                            <tr>
                                <td>
                                    <asp:Label ID="Label1" runat="server" Text="部門"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="Label9" runat="server" Text="職稱"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="Label4" runat="server" Text="帳號"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="Label8" runat="server" Text="姓名"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="Label6" runat="server" Text="狀態"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlDep" runat="server" DataSourceID="SqlDataSource1" CssClass="input-sm"
                                        DataTextField="部門名稱" DataValueField="部門代號" AppendDataBoundItems="true">
                                        <asp:ListItem Selected="True" Value="0">全部</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlEmp" runat="server" DataSourceID="SqlDataSource2" CssClass="input-sm"
                                        DataTextField="職稱名稱" DataValueField="職稱代號" AppendDataBoundItems="true">
                                        <asp:ListItem Selected="True" Value="0">全部</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtAccount" runat="server" CssClass="input-sm" Width="120"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtName" runat="server" CssClass="input-sm" Width="120"></asp:TextBox>
                                </td>
                                <td style="width: 50px">
                                    <asp:DropDownList ID="ddlstatus" runat="server" CssClass="input-sm">
                                        <asp:ListItem Selected="True" Value="0">全部</asp:ListItem>
                                        <asp:ListItem Value="True">在職</asp:ListItem>
                                        <asp:ListItem Value="False">不在職</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 50px">
                                    <asp:DropDownList ID="ddlstatus2" runat="server" CssClass="input-sm">
                                        <asp:ListItem Selected="True" Value="0">全部</asp:ListItem>
                                        <asp:ListItem Value="True">啟用</asp:ListItem>
                                        <asp:ListItem Value="False">未啟用</asp:ListItem>
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
                <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" OnSelected="SqlDataSource3_Selected"
                    DeleteCommand="DELETE FROM [員工帳號基本資料] WHERE [員工帳號] = @員工帳號;DELETE FROM [員工帳號] WHERE [員工帳號] = @員工帳號"
                    UpdateCommand="UPDATE [員工帳號基本資料] SET [在職] = @在職 WHERE [員工帳號] = @員工帳號;UPDATE [員工帳號] SET [啟用] = @啟用 WHERE [員工帳號] = @員工帳號"
                    SelectCommand="SELECT 員工帳號基本資料.*,部門.部門名稱,職稱.職稱名稱,員工帳號.*  FROM [員工帳號基本資料] 
                                    inner join 部門 on 員工帳號基本資料.部門代號=部門.部門代號 
                                    inner join 職稱 on 員工帳號基本資料.職稱代號=職稱.職稱代號 
                                    inner join 員工帳號 on 員工帳號基本資料.員工帳號=員工帳號.員工帳號
                                    WHERE (((@部門代號=0) OR (部門.部門代號=@部門代號))
                                    AND ((@職稱代號=0) OR (職稱.職稱代號=@職稱代號))
                                    AND (員工帳號基本資料.員工帳號 LIKE '%'+@員工帳號+'%')
                                    AND (員工帳號基本資料.姓名 LIKE '%'+@姓名+'%')
                                    AND ((@啟用='0') OR (啟用=@啟用)) AND ((@在職='0') OR (在職=@在職))
                                    AND (員工帳號基本資料.員工帳號 !='k_admin'))">
                    <DeleteParameters>
                        <asp:Parameter Name="員工帳號" Type="String" />
                    </DeleteParameters>
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlDep" Name="部門代號" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="ddlEmp" Name="職稱代號" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="txtName" Name="姓名" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:ControlParameter ControlID="txtAccount" Name="員工帳號" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:ControlParameter ControlID="ddlstatus" Name="在職" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="ddlstatus2" Name="啟用" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="在職" Type="Boolean" />
                    </UpdateParameters>
                </asp:SqlDataSource>
                    <label style="color: red">員工帳號的權限代碼預設為01</label>
                 <div>
                    <asp:Label ID="Label12" runat="server" Text=''  ForeColor="Green" ></asp:Label>
                </div>
                <%--InsertCommand="INSERT INTO [員工帳號基本資料] ([身份證字號], [部門代號], [職稱代號], [姓名], [性別], [出生日期], [email], [聯絡電話], [郵遞代號], [地址], [在職]) VALUES (@身份證字號, @部門代號, @職稱代號, @姓名, @性別, @出生日期, @email, @聯絡電話, @郵遞代號, @地址, @在職)">--%>


                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="員工帳號" OnDataBound="GridView1_DataBound" AllowPaging="true" PageSize="15"
                    DataSourceID="SqlDataSource3" CellPadding="4" ForeColor="#333333" GridLines="Both" CssClass="table-bordered table-condensed" OnRowDataBound="GridView1_RowDataBound"
                    OnRowCommand="GridView1_RowCommand" OnRowCreated="GridView1_RowCreated">
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
                        <asp:BoundField DataField="部門名稱" HeaderText="部門名稱" SortExpression="部門名稱" ReadOnly="true" ItemStyle-Width="140px" />
                        <asp:BoundField DataField="職稱名稱" HeaderText="職稱名稱" SortExpression="職稱名稱" ReadOnly="true" ItemStyle-Width="140px" />
                        <asp:BoundField DataField="員工帳號" HeaderText="員工帳號" SortExpression="員工帳號" ReadOnly="true" ItemStyle-Width="140px" />
                        <asp:BoundField DataField="姓名" HeaderText="姓名" SortExpression="姓名" ReadOnly="true" ItemStyle-Width="140px" />
                        <asp:CheckBoxField DataField="在職" HeaderText="在職" SortExpression="在職" ItemStyle-Width="40px" />
                        <asp:CheckBoxField DataField="啟用" HeaderText="啟用" SortExpression="啟用" ItemStyle-Width="40px" />
                        <asp:CommandField HeaderText="功能" EditText="修改" SelectText="編輯" DeleteText="刪除" ShowEditButton="True" ShowSelectButton="True" ShowDeleteButton="true" ItemStyle-Width="120px" />
                    </Columns>
                    <PagerTemplate>
                        <table style="width: 100%;">
                            <tr>
                                <td style="width: 70px"></td>
                                <td>
                                    <asp:Label ID="Label5" runat="server" Text="請選擇頁碼"></asp:Label>
                                    <asp:LinkButton ID="Linkpage1" runat="server" OnClick="page_SelectedIndexChanged" ForeColor="White"><span class="glyphicon glyphicon-chevron-left"></span></asp:LinkButton>
                                    <asp:DropDownList ID="ddlpage" AutoPostBack="true" runat="server" OnSelectedIndexChanged="page_SelectedIndexChanged" ForeColor="Black"></asp:DropDownList>
                                    <asp:LinkButton ID="Linkpage2" runat="server" OnClick="page_SelectedIndexChanged" ForeColor="White"><span class="glyphicon glyphicon-chevron-right"></span></asp:LinkButton>
                                    <%--                                    <asp:LinkButton ID="Linkpage1" runat="server" Font-Names="Webdings" Font-Underline="false" OnClick="page_SelectedIndexChanged" ForeColor="White">3</asp:LinkButton>
                                    <asp:DropDownList ID="ddlpage" AutoPostBack="true" runat="server" OnSelectedIndexChanged="page_SelectedIndexChanged" ForeColor="Black"></asp:DropDownList>
                                    <asp:LinkButton ID="Linkpage2" runat="server" Font-Names="Webdings" Font-Underline="false" OnClick="page_SelectedIndexChanged" ForeColor="White">4</asp:LinkButton>--%>
                                </td>
                                <td style="text-align: right">
                                    <asp:Label ID="labNum" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </PagerTemplate>

                    <EditRowStyle BackColor="#E7EFFA" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EFF3FB" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                    <SortedDescendingHeaderStyle BackColor="#4870BE" />
                </asp:GridView>
                <br />
                <br />

                <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
                    DeleteCommand="DELETE FROM [員工帳號基本資料] WHERE [員工帳號] = @員工帳號"
                    UpdateCommand="UPDATE [員工帳號基本資料] SET 部門代號=@部門代號,職稱代號=@職稱代號,姓名=@姓名,性別=@性別,出生日期=@出生日期,email=@email,聯絡電話=@聯絡電話,郵遞代號=@郵遞代號,地址=@地址,在職=@在職 WHERE 員工帳號 = @員工帳號;UPDATE [員工帳號] SET [啟用] = @啟用 WHERE [員工帳號] = @員工帳號"
                    SelectCommand="SELECT 員工帳號基本資料.*,部門.部門名稱,職稱.職稱名稱,員工帳號.*,郵遞區號.*,縣市.*  FROM [員工帳號基本資料] 
        inner join 部門 on 員工帳號基本資料.部門代號=部門.部門代號 
        inner join 職稱 on 員工帳號基本資料.職稱代號=職稱.職稱代號 
        inner join 員工帳號 on 員工帳號基本資料.員工帳號=員工帳號.員工帳號
        inner join 郵遞區號 on 員工帳號基本資料.郵遞代號=郵遞區號.郵遞代碼
        inner join 縣市 on 縣市.縣市代碼=郵遞區號.縣市代碼
        WHERE 員工帳號基本資料.[員工帳號] = @員工帳號">
                    <DeleteParameters>
                        <asp:Parameter Name="員工帳號" Type="String" />
                    </DeleteParameters>
                    <SelectParameters>
                        <asp:ControlParameter ControlID="GridView1" Name="員工帳號" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="姓名" Type="String" />
                        <asp:Parameter Name="出生日期" Type="DateTime" />
                        <asp:Parameter Name="郵遞代號" Type="Int32" />
                        <asp:Parameter Name="email" Type="String" />
                        <asp:Parameter Name="聯絡電話" />
                        <asp:Parameter Name="地址" Type="String" />
                    </UpdateParameters>
                </asp:SqlDataSource>
                <%--<asp:Parameter Name="部門代號" Type="Int32" />
            <asp:Parameter Name="職稱代號" Type="Int32" />
            <asp:Parameter Name="性別" Type="Boolean" />
            <asp:Parameter Name="郵遞代號" />
            <asp:Parameter Name="地址" />
            <asp:Parameter Name="在職" Type="Boolean" />
            <asp:Parameter Name="員工帳號" Type="String" />
            <asp:Parameter Name="啟用" Type="Boolean" />--%>
            </asp:Panel>

            <asp:Panel ID="Panel2" runat="server">
                <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataSourceID="SqlDataSource4" CssClass="table-bordered table-condensed"
                    Height="27px" Width="700px" DataKeyNames="員工帳號" OnDataBound="DetailsView1_DataBound" CellPadding="0"
                    ForeColor="#333333" OnItemCommand="DetailsView1_ItemCommand" HeaderText="員工基本資料">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <CommandRowStyle BackColor="#EFF1F5" Font-Bold="True" HorizontalAlign="Center" />
                    <EditRowStyle BackColor="#E6E6E6" HorizontalAlign="Left" />
                    <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" HorizontalAlign="Right" Width="140px" />
                    <Fields>
                        <asp:TemplateField HeaderText="部門名稱" SortExpression="部門名稱">
                            <EditItemTemplate>
                                <asp:DropDownList ID="DropDownList1" runat="server" OnInit="DropDownList1_Init" DataTextField="部門名稱" DataValueField="部門代號" SelectedValue='<%# Bind("部門代號") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("部門名稱") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="職稱名稱" SortExpression="職稱名稱">
                            <EditItemTemplate>
                                <asp:DropDownList ID="DropDownList2" runat="server" OnInit="DropDownList2_Init" DataTextField="職稱名稱" DataValueField="職稱代號" SelectedValue='<%# Bind("職稱代號") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("職稱名稱") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="員工帳號" HeaderText="員工帳號" ReadOnly="true" />
                        <asp:BoundField DataField="姓名" HeaderText="姓名" />
                        <asp:BoundField DataField="身份證字號" HeaderText="身份證字號" SortExpression="身份證字號" ReadOnly="true" />
                        <asp:TemplateField HeaderText="性別" SortExpression="性別">
                            <EditItemTemplate>
                                <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" SelectedValue='<%# Bind("性別") %>'>
                                    <asp:ListItem Value="True">男</asp:ListItem>
                                    <asp:ListItem Value="False">女</asp:ListItem>
                                </asp:RadioButtonList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# FormatSex((bool)Eval("性別")) %>'></asp:Label>

                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="出生日期">
                            <EditItemTemplate>
                                <div class="dropdown">
                                    <asp:TextBox ID="txtBirth" runat="server" Text='<%# Bind("出生日期","{0:yyyy/MM/dd}") %>' Enabled="false"></asp:TextBox>
<%--                                    <a href="#" class="glyphicon glyphicon-calendar dropdown-toggle" data-toggle="dropdown"></a>--%>
                                    <%--     <div class="dropdown-menu">--%>
                                        <asp:Calendar ID="CalBirth" runat="server" OnSelectionChanged="Calendar1_SelectionChanged" BackColor="White"
                                             BorderColor="#999999" CellPadding="4" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" 
                                            ForeColor="Black" Height="180px" Width="200px" VisibleDate='<%# Convert.ToDateTime(Eval("出生日期","{0:yyyy/MM/dd}")) %>'>
                                            <DayHeaderStyle BackColor="#CCCCCC" Font-Bold="True" Font-Size="7pt" />
                                            <NextPrevStyle VerticalAlign="Bottom" />
                                            <OtherMonthDayStyle ForeColor="#808080" />
                                            <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                                            <SelectorStyle BackColor="#CCCCCC" />
                                            <TitleStyle BackColor="#999999" BorderColor="Black" Font-Bold="True" />
                                            <TodayDayStyle BackColor="#CCCCCC" ForeColor="Black" />
                                            <WeekendDayStyle BackColor="#FFFFCC" />
                                        </asp:Calendar>
<%--                                    </div>--%>
                                </div>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("出生日期","{0:yyyy/MM/dd}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="email" HeaderText="Email" />
                        <asp:BoundField DataField="聯絡電話" HeaderText="聯絡電話" />
                        <asp:TemplateField HeaderText="住址">
                            <EditItemTemplate>
                                <asp:Label ID="lab4" runat="server" Text='<%# Eval("郵遞代碼") %>' Visible="false"></asp:Label>
                                <asp:DropDownList ID="ddl3" runat="server" DataSourceID="SqlDataSource1" DataTextField="縣市" DataValueField="縣市代碼"
                                    SelectedValue='<%#Eval("縣市代碼") %>' AutoPostBack="true" OnDataBound="ddl3_DataBound" OnSelectedIndexChanged="ddl3_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:DropDownList ID="ddl4" runat="server" DataSourceID="SqlDataSource5" DataTextField="鄉鎮市區" DataValueField="郵遞代碼" AppendDataBoundItems="true"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddl4_SelectedIndexChanged">
                                </asp:DropDownList>

                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("地址") %>'></asp:TextBox>
                                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [縣市] where 顯示=@顯示">
                                    <SelectParameters>
                                        <asp:Parameter DefaultValue="true" Name="顯示" Type="Boolean" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                                <asp:TextBox ID="txtareano" runat="server" Text='<%# Bind("郵遞代號") %>' ReadOnly="true" Visible="false"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="ddl4" runat="server" ErrorMessage="請選擇鄉鎮市區" Display="Dynamic" InitialValue="0" ForeColor="Red"></asp:RequiredFieldValidator>
                                <asp:SqlDataSource ID="SqlDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [郵遞區號]  WHERE ([縣市代碼] = @縣市代碼)">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddl3" Name="縣市代碼" PropertyName="SelectedValue" Type="Int32" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("郵遞區號") %>'></asp:Label>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("縣市") %>'></asp:Label>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("鄉鎮市區") %>'></asp:Label>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("地址") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--            <asp:BoundField DataField="郵遞區號" HeaderText="郵遞區號" SortExpression="郵遞區號"/>
            <asp:BoundField DataField="縣市" HeaderText="縣市" SortExpression="縣市" Visible="false"/>
            <asp:BoundField DataField="鄉鎮市區" HeaderText="鄉鎮市區" SortExpression="鄉鎮市區" Visible="false" />--%>
                        <%--            <asp:BoundField DataField="地址" HeaderText="地址" SortExpression="地址" />--%>
                        <asp:CheckBoxField DataField="在職" HeaderText="在職" SortExpression="在職" />
                        <asp:CheckBoxField DataField="啟用" HeaderText="啟用" SortExpression="啟用" />
                        <%--            <asp:BoundField DataField="員工帳號1" HeaderText="員工帳號1" SortExpression="員工帳號1" Visible="False" />
            <asp:BoundField DataField="員工密碼" HeaderText="員工密碼" SortExpression="員工密碼" Visible="false" />
            <asp:BoundField DataField="權限代碼" HeaderText="權限代碼" SortExpression="權限代碼" Visible="false" />
            <asp:BoundField DataField="郵遞代碼" HeaderText="郵遞代碼" InsertVisible="False" ReadOnly="True" SortExpression="郵遞代碼" Visible="false" />
            <asp:BoundField DataField="縣市代碼1" HeaderText="縣市代碼1" InsertVisible="False" ReadOnly="True" SortExpression="縣市代碼1" Visible="false" />--%>
                        <asp:TemplateField ShowHeader="False">
                            <ItemStyle HorizontalAlign="Center" BackColor="#E9ECF1" Font-Bold="True" ForeColor="White" />
                            <EditItemTemplate>
                                <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="更新"></asp:LinkButton>
                                &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消"></asp:LinkButton>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="編輯"></asp:LinkButton>
                                &nbsp;<asp:LinkButton ID="LinkButton3" runat="server" CausesValidation="False" CommandName="CancelEdit" Text="回上一頁"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                </asp:DetailsView>
                <br />
                <br />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
     <script src="Scripts/jquery-1.9.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>


<%--            <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.11.1/i18n/jquery-ui-i18n.min.js"></script>
            <script type="text/javascript">
                //function SetDate(ClientID) {
                //    var Url = 'BirthDay.aspx?DialogClientID=' + ClientID;
                //    window.open(Url, 'Calendar', "toolbar=no,scrollbars=no,resizable=yes,top=350,left=500,width=300,height=350,status=no, location=no,directories=no,menubar=no,center=yes");
                //}

                $('#ContentPlaceHolder1_DetailsView1_txtBirth').datepicker({
                    firstDay: 1,              // 以週一為每週首日  
                    numberOfMonths: 1,        // 顯示的月數
                    showOtherMonths: true,    // 顯示其它月份日期
                    selectOtherMonths: true,  // 可選其它月份日期
                    changeMonth: true,        // 月份選單        
                    changeYear: true,         // 年份選單 
                    yearRange: '-100:+0',
                    showAnim: 'fadeIn',       // 淡入效果
                    duration: 750
                }).attr('readonly', 'readonly');

            </script>--%>

</asp:Content>
