<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="LogRecord.aspx.cs" Inherits="LogRecord" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css" />
    <script src="http://code.jquery.com/jquery-2.1.4.js"></script>
    <script src="http://code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT DISTINCT [操作頁面] FROM [員工使用記錄]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT DISTINCT [操作動作] FROM [員工使用記錄] WHERE ([操作頁面] = @操作頁面)">
        <SelectParameters>
            <asp:ControlParameter ControlID="DropDownList1" Name="操作頁面" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <div class="container">
        <div style="color: #e14286; text-align: left; margin-top: 20px">
            <asp:Label ID="Label6" runat="server" Text="網站操作紀錄" CssClass="h2"></asp:Label>
        </div>
        <br />
        <div>
            <table class="table-condensed" style="text-align: center;">
                <tr>
                    <td style="width: 120px">
                        <asp:Label ID="Label1" runat="server" Text="員工帳號"></asp:Label>
                    </td>
                    <td style="width: 140px">
                        <asp:Label ID="Label3" runat="server" Text="操作頁面"></asp:Label>
                    </td>
                    <td style="width: 100px">
                        <asp:Label ID="Label4" runat="server" Text="操作動作"></asp:Label>
                    </td>
                    <td >
                        <asp:Label ID="Label8" runat="server" Text="開始日期"></asp:Label>
                    </td>
                        <td>
                        <asp:Label ID="Label7" runat="server" Text="結束日期"></asp:Label>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="txtAccount" runat="server" CssClass="input-sm"></asp:TextBox>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="SqlDataSource2" DataTextField="操作頁面" DataValueField="操作頁面"
                            AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" CssClass="input-sm">
                            <asp:ListItem Selected="True" Value="0">全部</asp:ListItem>
                        </asp:DropDownList>

                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="SqlDataSource3" DataTextField="操作動作" DataValueField="操作動作"
                            AppendDataBoundItems="true" CssClass="input-sm">
                            <asp:ListItem Selected="True" Value="0">全部</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td >
                        <asp:TextBox ID="txttimeF" runat="server" CssClass="input-sm" Width="90"></asp:TextBox>
                    </td>
                    <td>
                        ~&nbsp&nbsp<asp:TextBox ID="txttimeE" runat="server" CssClass="input-sm" Width="90"></asp:TextBox>
                    </td>
                    <td style="width: 100px">
                        <asp:Button ID="Button1" runat="server" Text="查詢" CssClass="btn btn-primary" />
                    </td>
                </tr>
                <tr>
                    <td></td><td></td><td></td>
                    <td colspan="2">
                        <asp:CompareValidator ID="CompareValidator1" runat="server" 
                            ErrorMessage="結束日要大於開始日期" ControlToCompare="txttimeF" 
                            ControlToValidate="txttimeE" Type="Date" Operator="GreaterThanEqual"
                             Display="Dynamic" ForeColor="Red"> </asp:CompareValidator>
                    </td>
                    <td></td>
                </tr>
            </table>
            <hr />
        </div>
    </div>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" OnSelected="SqlDataSource1_Selected"
        SelectCommand="SELECT * FROM [員工使用記錄] WHERE (([員工帳號] LIKE '%' + @員工帳號 + '%')
        AND ((@操作頁面='0') OR ([操作頁面] = @操作頁面)) 
        AND ((@操作動作='0') OR ([操作動作] = @操作動作))
        AND (convert(date,操作時間) between @時間F AND @時間E)
        AND (員工帳號!='k_admin'))" >
        <SelectParameters>
            <asp:ControlParameter ControlID="txtAccount" Name="員工帳號" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
            <asp:ControlParameter ControlID="DropDownList1" Name="操作頁面" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="DropDownList2" Name="操作動作" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="txttimeF" Name="時間F" PropertyName="Text" Type="DateTime" />
            <asp:ControlParameter ControlID="txttimeE" Name="時間E" PropertyName="Text" Type="DateTime" />
        </SelectParameters>
    </asp:SqlDataSource>

    <div>
        <asp:Label ID="Label12" runat="server" Text='' ForeColor="Green"></asp:Label>
    </div>

    <asp:GridView ID="GridView1" runat="server" DataSourceID="SqlDataSource1" DataKeyNames="流水號" AutoGenerateColumns="False" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None"
        BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Vertical" AllowPaging="true" PageSize="20" OnDataBound="GridView1_DataBound" 
        CssClass="table-bordered table-condensed" >
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:BoundField DataField="流水號" HeaderText="流水號" InsertVisible="False" ReadOnly="True" SortExpression="流水號" Visible="false" />
            <asp:TemplateField HeaderText="序號" ItemStyle-Width="100">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%#(Container.DataItemIndex+1).ToString("#0") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="員工帳號" HeaderText="員工帳號" SortExpression="員工帳號" ItemStyle-Width="100" />
            <asp:BoundField DataField="操作頁面" HeaderText="操作頁面" SortExpression="操作頁面" ItemStyle-Width="120" />
            <asp:BoundField DataField="操作動作" HeaderText="操作動作" SortExpression="操作動作" ItemStyle-Width="90" />
            <asp:BoundField DataField="操作時間" HeaderText="操作時間" SortExpression="操作時間" ItemStyle-Width="180" />
            <asp:BoundField DataField="備註" HeaderText="備註" SortExpression="備註" ItemStyle-Width="210" />
        </Columns>
        <PagerTemplate>
            <table style="width: 100%;">
                <tr>
                    <td style="width: 70px"></td>
                    <td style="text-align: center">
                        <asp:Label ID="Label5" runat="server" Text="請選擇頁碼"></asp:Label>
                        <asp:LinkButton ID="Linkpage1" runat="server" Font-Names="Webdings" Font-Underline="false" OnClick="page_SelectedIndexChanged" ForeColor="White">3</asp:LinkButton>
                        <asp:DropDownList ID="ddlpage" AutoPostBack="true" runat="server" OnSelectedIndexChanged="page_SelectedIndexChanged" ForeColor="Black"></asp:DropDownList>
                        <asp:LinkButton ID="Linkpage2" runat="server" Font-Names="Webdings" Font-Underline="false" OnClick="page_SelectedIndexChanged"  ForeColor="White">4</asp:LinkButton>
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
        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <%--<SortedAscendingCellStyle BackColor="#FBFBF2" />
        <SortedAscendingHeaderStyle BackColor="#848384" />
        <SortedDescendingCellStyle BackColor="#EAEAD3" />
        <SortedDescendingHeaderStyle BackColor="#575357" />--%>
    </asp:GridView>
    <br />
    <br />
        <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.11.1/i18n/jquery-ui-i18n.min.js"></script>
        <script type="text/javascript">
        //function SetDate(ClientID) {
        //    var Url = 'BirthDay.aspx?DialogClientID=' + ClientID;
        //    window.open(Url, 'Calendar', "toolbar=no,scrollbars=no,resizable=yes,top=350,left=500,width=300,height=350,status=no, location=no,directories=no,menubar=no,center=yes");
        //}

            $('#ContentPlaceHolder1_txttimeF').datepicker({
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

            $('#ContentPlaceHolder1_txttimeE').datepicker({
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

