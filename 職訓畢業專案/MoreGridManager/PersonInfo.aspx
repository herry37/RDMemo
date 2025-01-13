<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="PersonInfo.aspx.cs" Inherits="PersonInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css"/>
    <script src="http://code.jquery.com/jquery-2.1.4.js"></script>
    <script src="http://code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [部門]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [職稱]"></asp:SqlDataSource>
    <div style="color: #e14286; text-align: left; margin-top: 20px">
        <asp:Label ID="Label3" runat="server" Text="個人資料管理 " CssClass="h2"></asp:Label>
    </div>
    <br />
    <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
        DeleteCommand="DELETE FROM [員工帳號基本資料] WHERE [員工帳號] = @員工帳號"
        UpdateCommand="UPDATE [員工帳號基本資料] SET 部門代號=@部門代號,職稱代號=@職稱代號,姓名=@姓名,出生日期=@出生日期,email=@email,聯絡電話=@聯絡電話,郵遞代號=@郵遞代號,地址=@地址 WHERE 員工帳號 = @員工帳號"
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
            <asp:SessionParameter Name="員工帳號" SessionField="account" Type="String" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="部門代號" />
            <asp:Parameter Name="職稱代號" />
            <asp:Parameter Name="姓名" Type="String" />
            <asp:Parameter Name="性別" />
            <asp:Parameter Name="出生日期" Type="DateTime" />
            <asp:Parameter Name="email" Type="String" />
            <asp:Parameter Name="聯絡電話" />
            <asp:Parameter Name="郵遞代號" Type="Int32" />
            <asp:Parameter Name="地址" Type="String" />
            <asp:Parameter Name="員工帳號" Type="String" />
            <asp:Parameter Name="員工密碼" Type="String" />
        </UpdateParameters>
    </asp:SqlDataSource>

    <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataSourceID="SqlDataSource4"
        Height="27px" Width="700px" DataKeyNames="員工帳號" CellPadding="0" OnItemCommand="DetailsView1_ItemCommand"
        ForeColor="#333333" HeaderText="員工基本資料" CssClass="table-bordered table-condensed">
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <CommandRowStyle BackColor="#EFF1F5" Font-Bold="True" Height="30px" HorizontalAlign="Center" />
        <EditRowStyle BackColor="#E6E6E6" HorizontalAlign="Left" />
        <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" Height="30px" HorizontalAlign="Center" Width="140px" />
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
            <asp:TemplateField HeaderText="員工密碼">
                <EditItemTemplate>
                    <asp:TextBox ID="txtpwd" runat="server" Text='<%# Bind("員工密碼","****") %>' OnTextChanged="TextBox2_TextChanged"></asp:TextBox>
                    <asp:Label ID="Label8" runat="server" Text="不更改密碼無需輸入" ForeColor="Red"></asp:Label>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtpwd" runat="server" ErrorMessage="不能空白"></asp:RequiredFieldValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label6" runat="server" Text='<%# Eval("員工密碼","*******") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="姓名" HeaderText="姓名" />
            <asp:BoundField DataField="身份證字號" HeaderText="身份證字號" SortExpression="身份證字號" ReadOnly="true" />
            <asp:TemplateField HeaderText="性別" SortExpression="性別">
                <ItemTemplate>
                    <asp:Label ID="Label10" runat="server" Text='<%# FormatSex((bool)Eval("性別")) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="出生日期">
                <EditItemTemplate>
                    <div>
                        <asp:TextBox ID="txtBirth" runat="server" Text='<%# Bind("出生日期","{0:yyyy/MM/dd}") %>'></asp:TextBox>
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
            <%--    <asp:CheckBoxField DataField="在職" HeaderText="在職" SortExpression="在職" />
            <asp:CheckBoxField DataField="啟用" HeaderText="啟用" SortExpression="啟用" />--%>
            <asp:TemplateField ShowHeader="False">
                <ItemStyle HorizontalAlign="Center" BackColor="#E9ECF1" Font-Bold="True" ForeColor="White" Height="30px" />
                <EditItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" Text="更新"></asp:LinkButton>
                    &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="取消"></asp:LinkButton>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="編輯"></asp:LinkButton>
                    &nbsp; 
                    <%--                    <asp:LinkButton ID="LinkButton3" runat="server" CausesValidation="False" CommandName="CancelEdit" Text="回上一頁"></asp:LinkButton>--%>
                </ItemTemplate>
            </asp:TemplateField>
        </Fields>
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Height="30px" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
    </asp:DetailsView>

    <br />
    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.11.1/i18n/jquery-ui-i18n.min.js"></script>
    <script type="text/javascript">
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
    </script>

</asp:Content>

