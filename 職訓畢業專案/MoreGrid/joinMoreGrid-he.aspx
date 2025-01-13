<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="joinMoreGrid-he.aspx.cs" Inherits="joinMoreGrid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .join-container{
            width:600px;
            margin:auto;
            margin-top:50px;
        }
        #header {
            padding:5px;
            text-align:center;
            background-color:#222;
            color:#fff;
            height:30px;
        }
        table {
            margin:auto;
        }
       .TextBox{
           height:25px;           
        }

       .btn-submit{
           margin-top:30px;
       }
       .table tr td:nth-child(1){
           width:30%;
           text-align:right;
           padding-right:20px;
       }
       .table tr td:nth-child(2){
           width:70%;
           text-align:left;
       }
    </style>
        

    <script>
        
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="left" Runat="Server">

    <%--註冊資料庫ID Sqlregistered--%>
    <%--<asp:SqlDataSource ID="Sqlregistered" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" ProviderName="System.Data.SqlClient" 
        SelectCommand="SELECT * FROM [會員]"
        UpdateCommand="update 會員 set 帳號=@帳號,密碼=@密碼,暱稱=@暱稱,註冊日期=@註冊日期,啟用日期=@啟用日期,停用日期=@停用日期,啟用=@啟用,其他=@其他,性別=@性別,email=@email,市話=@市話,手機=@手機,生日=@生日,權限代碼=@權限代碼,認證=@認證 where 帳號=@帳號"></asp:SqlDataSource>--%>
    <%--<asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false" ShowMessageBox="true"/>--%>
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
    <div class="join-container">

            <table class="table">
                <thead id="header">
                    <tr>
                        <th colspan="3" style="color: white;">請輸入註冊資料</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>帳號：</td>
                        <td>
                            <asp:TextBox ID="txtNo" CssClass="TextBox input-sm" runat="server" placeholder="長度6-20英數字"></asp:TextBox>
                            <asp:Button ID="checkNo" runat="server" Text="檢查重複" OnClick="checkNo_Click" ValidationGroup="3"  CssClass="btn-sm btn-danger"/>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtNo" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtNo" ErrorMessage="請檢查帳號格式" Text="*(帳號長度6-20)" ForeColor="red" Display="Dynamic" ValidationExpression="\w{6,20}"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>密碼：</td>
                        <td>
                            <asp:TextBox ID="txtPsd" CssClass="TextBox input-sm" runat="server" TextMode="Password"  placeholder="長度6-20英數字"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtPsd" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="txtPsd" ErrorMessage="請檢查密碼格式" Text="*(密碼長度6-20)" ForeColor="red" Display="Dynamic" ValidationExpression="\w{6,20}"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>確認密碼：</td>
                        <td>
                            <asp:TextBox ID="txtPsd2" CssClass="TextBox input-sm" runat="server" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtPsd2" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                             <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtPsd" ControlToValidate="txtPsd2" ErrorMessage="請檢查是否相同" ForeColor="Red"></asp:CompareValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtPsd2" ErrorMessage="請檢查密碼格式" Text="*(密碼長度6-20)" ForeColor="red" Display="Dynamic" ValidationExpression="\w{6,20}"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>暱稱：</td>
                        <td>
                            <asp:TextBox ID="txtName" CssClass="TextBox input-sm" runat="server"></asp:TextBox>
                            <%-- \u0800 以上的爲中、韓、日字符。中文的範圍：\u4e00 - \u9fa5
                                日文在\u0800 - \u4e00  韓文爲\u9fa5以上 --%>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtName" ErrorMessage="請檢查是否為中文" Text="*(請輸入2-20字中文)" ForeColor="red" Display="Dynamic" ValidationExpression="[\w\u4e00-\u9fa5]{2,20}"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>

                        <td>性別：</td>
                        <td>
                            <asp:RadioButtonList ID="txtGender" runat="server" RepeatDirection="Horizontal" CssClass="pull-left" >
                                 <asp:ListItem Text="男" Value="True" Selected="True"></asp:ListItem>
                                 <asp:ListItem Text="女" Value="False"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td>E-mail：</td>
                        <td>
                            <asp:TextBox ID="txtEmail" CssClass="TextBox input-sm" runat="server" placeholder="xxx@xxx.xxx.xx"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtemail" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                           <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ErrorMessage="(請檢查格式)" ValidationExpression="\w+([-+.']\w+)*@[A-Za-z]+([-.][A-Za-z]+)*\.[A-Za-z]+([-.][A-Za-z]+)*" ForeColor="Red" Display="Dynamic" ControlToValidate="txtEmail"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>市話：</td>
                        <td>
                             <asp:Label ID="txtTel3" runat="server" Visible="true"></asp:Label>
                            <asp:TextBox ID="txtTel2" CssClass="TextBox input-sm" runat="server" placeholder="區碼" Width="50px"></asp:TextBox>
                            <asp:TextBox ID="txtTel" CssClass="TextBox input-sm" runat="server" placeholder="xxx-xxxx" Width="117px"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator11" runat="server" ControlToValidate="txtTel2" ErrorMessage="請檢查格式" Text="*(區碼)" ForeColor="red" Display="Dynamic" ValidationExpression="[0]+\d{1,2}"></asp:RegularExpressionValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="txtTel" ErrorMessage="請檢查格式" Text="*(請輸入7或8碼xxx-xxxx)" ForeColor="red" Display="Dynamic" ValidationExpression="\d{3,4}(\-+)[0-9]{4}"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                    <td>
                        <asp:Label ID="Label11" runat="server" Text="住址："></asp:Label></td>
                    <td>
                        <asp:DropDownList ID="ddlcity" runat="server" DataSourceID="SqlDataSource3" DataTextField="縣市" DataValueField="縣市代碼" 
                             OnSelectedIndexChanged="ddlcity_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true" CssClass="input-sm">
                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" ControlToValidate="ddlcity" runat="server" ErrorMessage="*(必要欄位)" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic" InitialValue="0"></asp:RequiredFieldValidator>

                        <asp:DropDownList ID="ddlarea" runat="server" DataSourceID="SqlDataSource4" DataTextField="鄉鎮市區" DataValueField="郵遞代碼" CssClass="input-sm"
                            AppendDataBoundItems="true">
                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                        </asp:DropDownList>
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator11" ControlToValidate="ddlarea" runat="server" ErrorMessage="*(必要欄位)" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic" InitialValue="0"></asp:RequiredFieldValidator>
                         </td> 
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                          <asp:TextBox ID="txtAddress2" runat="server"  MaxLength="50" Width="60px" CssClass="input-sm"  placeholder="村里"></asp:TextBox>
                        <asp:TextBox ID="txtAddress" runat="server"  MaxLength="50" Width="240px" CssClass="input-sm" placeholder="地址"></asp:TextBox>
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator12" ControlToValidate="txtAddress" runat="server" ErrorMessage="*(必要欄位)" ForeColor="Red" ValidationGroup="inaccount" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                    </tr>
                   
              
                    <tr>
                        <td>手機：</td>
                        <td>
                            <asp:TextBox ID="txtMobile" CssClass="TextBox input-sm" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtMobile" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" ControlToValidate="txtMobile" ErrorMessage="請檢查格式" Text="*(請輸入09xxxxxxxx)" ForeColor="red" Display="Dynamic" ValidationExpression="[0]+[9]+\d{8}"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>生日：</td>
                        <td>
                            <asp:Label ID="txtBirth" runat="server" Visible="true"></asp:Label>
                            <asp:TextBox ID="txtYear" CssClass="TextBox input-sm" runat="server" Width="50" placeholder="yyyy"></asp:TextBox><span> / </span>
                            <asp:TextBox ID="txtMon" CssClass="TextBox input-sm" runat="server" Width="44" placeholder="mm"></asp:TextBox><span> / </span>
                            <asp:TextBox ID="txtDay" CssClass="TextBox input-sm" runat="server" Width="44" placeholder="dd"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtYear" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtMon" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="*(必要欄位)" ControlToValidate="txtDay" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" ControlToValidate="txtYear" ErrorMessage="請檢查格式" Text="*(請輸入西元年)" ForeColor="red" Display="Dynamic" ValidationExpression="\d{4}"></asp:RegularExpressionValidator>
                            <%--日期驗證--%>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" ControlToValidate="txtMon" ErrorMessage="請檢查格式" Text="*(請輸入2位數)" ForeColor="red" Display="Dynamic" ValidationExpression="\d{2}"></asp:RegularExpressionValidator>
                            <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="*超出範圍" Display="Dynamic" ForeColor="Red" ControlToValidate="txtMon" Type="Integer" MaximumValue="12" MinimumValue="1"></asp:RangeValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator10" runat="server" ControlToValidate="txtDay" ErrorMessage="請檢查格式" Text="*(請輸入2位數)" ForeColor="red" Display="Dynamic" ValidationExpression="\d{2}"></asp:RegularExpressionValidator>
                            <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="*超出範圍" Display="Dynamic" ForeColor="Red" ControlToValidate="txtDay" Type="Integer" MaximumValue="31" MinimumValue="1"></asp:RangeValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="text-align: center">
                            <asp:Button ID="btGo" runat="server" Text="送出" Width="200" OnClick="btGo_Click" CssClass="btn btn-primary btn-submit" />
                        </td>
                    </tr>
                </tbody>
            </table>
    </div>
</asp:Content>

