<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="OLD_EmployAccount .aspx.cs" Inherits="EmployAccount" 
    MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
        table{text-align:center;vertical-align:central;}
        table td:first-child{text-align:right}
        table td:last-child{text-align:left}
    </style>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [部門]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT * FROM [職稱]"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT [縣市代碼], [縣市] FROM [縣市] WHERE ([顯示] = @顯示)">
        <SelectParameters>
            <asp:Parameter DefaultValue="true" Name="顯示" Type="Boolean" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" SelectCommand="SELECT [郵遞代碼], [郵遞區號], [鄉鎮市區] FROM [郵遞區號] WHERE ([縣市代碼] = @縣市代碼)">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlcity" Name="縣市代碼" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
    <br />
    <div class="container">
        <div>
            <table style="width: 790px" class="well table">
                <tr class="label-primary" style="color: white">
                    <th colspan="2">
                        <asp:Label ID="Label12" runat="server" Text="申請帳號"></asp:Label>
                    </th>
                </tr>
                <tr>
                    <td style="width: 140px">
                        <asp:Label ID="Label1" runat="server" Text="帳號"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtAccount" runat="server" MaxLength="20" CssClass="input-sm"></asp:TextBox>
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">查詢帳號是否重複</asp:LinkButton>
                        <asp:Label ID="labmsg" runat="server" Text=""></asp:Label>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtAccount" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="密碼"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtpwd" runat="server" TextMode="Password" MaxLength="20" CssClass="input-sm"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtpwd" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic"></asp:RequiredFieldValidator>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="部門"></asp:Label></td>
                    <td>
                        <asp:DropDownList ID="ddlDep" runat="server" DataSourceID="SqlDataSource1" CssClass="input-sm"
                            DataTextField="部門名稱" DataValueField="部門代號" AppendDataBoundItems="true">
                            <asp:ListItem Selected="True" Value="0">請選擇</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="ddlDep" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic" InitialValue="0"></asp:RequiredFieldValidator>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label5" runat="server" Text="職稱"></asp:Label></td>
                    <td>
                        <asp:DropDownList ID="ddlEmp" runat="server" DataSourceID="SqlDataSource2" CssClass="input-sm"
                            DataTextField="職稱名稱" DataValueField="職稱代號" AppendDataBoundItems="true">
                            <asp:ListItem Selected="True" Value="0">請選擇</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="ddlEmp" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic" InitialValue="0"></asp:RequiredFieldValidator>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="姓名"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" MaxLength="20" CssClass="input-sm"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="txtName" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic"></asp:RequiredFieldValidator>

                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="Label6" runat="server" Text="身份證字號"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtId" runat="server" MaxLength="10" CssClass="input-sm"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" ControlToValidate="txtId" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtId" runat="server" ValidationExpression="[A-Za-z][1-2][0-9]{8}" ErrorMessage="格式有錯" ForeColor="Red" Display="Dynamic" ValidationGroup="inaccount"></asp:RegularExpressionValidator>
                        <asp:CustomValidator ID="CustomValidator1" ControlToValidate="txtId" OnServerValidate="CustomValidator1_ServerValidate" runat="server" ErrorMessage="不合法身份證字號" ForeColor="Red" Display="Dynamic" ValidationGroup="inaccount"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label8" runat="server" Text="出生日期"></asp:Label></td>
                    <td>
                        <div class="dropdown">
                            <asp:DropDownList ID="DropDownList1" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" AutoPostBack="true" CssClass="input-sm"></asp:DropDownList>年
                            <asp:DropDownList ID="DropDownList2" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" AutoPostBack="true" CssClass="input-sm"></asp:DropDownList>月
                            <asp:TextBox ID="txtDay" runat="server" data-toggle="dropdown" Width="40px" BackColor="White" ReadOnly="true" CssClass="input-sm"></asp:TextBox>日
                            <%--<asp:TextBox ID="" runat="server" ReadOnly="true" Width="30"></asp:TextBox>--%>
                            <a href="#" class="glyphicon glyphicon-calendar dropdown-toggle" data-toggle="dropdown"></a>
                            <div class="dropdown-menu">
                                <asp:Calendar ID="CalBirth" runat="server" OnSelectionChanged="Calendar1_SelectionChanged" BackColor="White" BorderColor="#999999" CellPadding="4" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" Height="180px" Width="200px">
                                    <DayHeaderStyle BackColor="#CCCCCC" Font-Bold="True" Font-Size="7pt" />
                                    <NextPrevStyle VerticalAlign="Bottom" />
                                    <OtherMonthDayStyle ForeColor="#808080" />
                                    <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                                    <SelectorStyle BackColor="#CCCCCC" />
                                    <TitleStyle BackColor="#999999" BorderColor="Black" Font-Bold="True" />
                                    <TodayDayStyle BackColor="#CCCCCC" ForeColor="Black" />
                                    <WeekendDayStyle BackColor="#FFFFCC" />
                                </asp:Calendar>
                            </div>
                            <asp:TextBox ID="txtBirth" runat="server" ReadOnly="true" Visible="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" ControlToValidate="txtBirth" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic"></asp:RequiredFieldValidator>

                        </div>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label7" runat="server" Text="性別"></asp:Label></td>
                    <td>
                        <asp:RadioButtonList ID="rdbsex" runat="server" CellSpacing="1" RepeatDirection="Horizontal">
                            <asp:ListItem Selected="True" Value="True">男&nbsp&nbsp&nbsp</asp:ListItem>
                            <asp:ListItem Value="False">女</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label10" runat="server" Text="連絡電話"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtphone" runat="server" TextMode="Phone" MaxLength="20" CssClass="input-sm"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" ControlToValidate="txtphone" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic"></asp:RequiredFieldValidator>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label9" runat="server" Text="Email"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtmail" runat="server" TextMode="Email" MaxLength="50" CssClass="input-sm" Width="250"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" ControlToValidate="txtmail" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic"></asp:RequiredFieldValidator>

                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="Label11" runat="server" Text="住址"></asp:Label></td>
                    <td>
                        <asp:DropDownList ID="ddlcity" runat="server" DataSourceID="SqlDataSource3" DataTextField="縣市" DataValueField="縣市代碼"
                            OnSelectedIndexChanged="ddlcity_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true" CssClass="input-sm">
                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" ControlToValidate="ddlcity" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic" InitialValue="0"></asp:RequiredFieldValidator>

                        <asp:DropDownList ID="ddlarea" runat="server" DataSourceID="SqlDataSource4" DataTextField="鄉鎮市區" DataValueField="郵遞代碼" CssClass="input-sm"
                            AppendDataBoundItems="true">
                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" ControlToValidate="ddlarea" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic" InitialValue="0"></asp:RequiredFieldValidator>

                        <asp:TextBox ID="txtAddress" runat="server" MaxLength="50" Width="300px" CssClass="input-sm"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator12" ControlToValidate="txtAddress" runat="server" ErrorMessage="必填欄位" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic"></asp:RequiredFieldValidator>

                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="確定" CssClass="btn btn-success" OnClick="Button1_Click" ValidationGroup="inaccount" />
                        <asp:Button ID="Button2" runat="server" Text="取消" CssClass="btn btn-default" OnClick="Button2_Click" />

                    </td>

                </tr>

            </table>
        </div>

    </div>

    <br />
    <%--InsertCommand="INSERT INTO [員工帳號基本資料] ([身份證字號], [部門代號], [職稱代號], [姓名], [性別], [出生日期], [email], [聯絡電話], [郵遞代號], [地址], [在職]) VALUES (@身份證字號, @部門代號, @職稱代號, @姓名, @性別, @出生日期, @email, @聯絡電話, @郵遞代號, @地址, @在職)">--%>
    <br />
    <br />

</asp:Content>
