<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="memberLoginValidate.aspx.cs" Inherits="memberLoginValidate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
        .login-container{
            margin: 0 auto;
                      
            width:400px;

            display:block;
            border-radius:2px;
            background-color: #F7F7F7;
        }
        .login-header{
            background-color:#222;
            color:#fff;
            padding: 38px;
            padding-top:15px;
        }
        .main{
            padding: 38px;  
        }
        .main span{
             font-weight:bold;
        }
        .input-group {
            margin-bottom: 15px;
           
        }
        .login-footer{
            padding:15px;
            background:#f0f0f0;
        }
        .modal-header{
            background-color:black;
            color:white;
        }
    </style>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="left" Runat="Server">

    <asp:SqlDataSource ID="Sqlregistered" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" ProviderName="System.Data.SqlClient" 
        SelectCommand="SELECT * FROM [會員]"
        UpdateCommand="update 會員 set 帳號=@帳號,密碼=@密碼,暱稱=@暱稱,註冊日期=@註冊日期,啟用日期=@啟用日期,停用日期=@停用日期,啟用=@啟用,其他=@其他,性別=@性別,email=@email,市話=@市話,手機=@手機,生日=@生日,權限代碼=@權限代碼,認證=@認證 where 會員=@會員"></asp:SqlDataSource>
    <%--<asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false" ShowMessageBox="true"/>--%>

    <div class="login-container">
        <div class="login-header">
            <h3><asp:Label ID="Label1" runat="server" Text="會員登入"></asp:Label></h3>
        </div>        
        <div class="main">
            <asp:Label ID="Label2" runat="server" Text="帳號"></asp:Label>
            <div class="input-group">
                <span class="input-group-addon"><i class="glyphicon glyphicon-user"></i></span>
                <asp:TextBox ID="txtNo" runat="server" placeholder="長度6-20英數字" CssClass="form-control"></asp:TextBox>
            </div>
            <asp:Label ID="Label3" runat="server" Text="密碼"></asp:Label>
            <div class="input-group">
                <span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span>
                <asp:TextBox ID="txtPsd" runat="server" TextMode="Password" placeholder="長度6-20英數字" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="input-group pull-right">
                <asp:Button ID="btGo" runat="server" Text="登入" OnClick="btGo_Click" CssClass="btn btn-primary"
                      />
            </div>            
        </div>
        <div class="login-footer">                        
            <%--<a onclick="Forget()">忘記密碼</a>--%>
            <input id="btForget" type="button"  value="忘記密碼"  class="btn btn-danger" data-toggle="collapse" data-target="#myModal"/>
        </div>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtNo" ErrorMessage="請檢查格式" Text="*(長度6-20)" ForeColor="red" Display="Dynamic" ValidationExpression="\w{6,20}"></asp:RegularExpressionValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="txtPsd" ErrorMessage="請檢查格式" Text="*(長度6-20)" ForeColor="red" Display="Dynamic" ValidationExpression="\w{6,20}"></asp:RegularExpressionValidator>        
    </div>
    
    <div class="modal" id="myModal">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <%--<button class="close" data-dismiss="modal">&times;</button>--%>
                            <h3 class="modal-title"><asp:Label ID="Label4" runat="server" Text="忘記密碼"></asp:Label></h3>
                        </div>
                        <div class="modal-body">
                            <h3><asp:Label ID="Label5" runat="server" Text="輸入e-mail"></asp:Label></h3>
                            <asp:TextBox ID="txtEmail" runat="server" Width="250"   ></asp:TextBox>
                             <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ErrorMessage="(請檢查格式)" ValidationExpression="\w+([-+.']\w+)*@[A-Za-z]+([-.][A-Za-z]+)*\.[A-Za-z]+([-.][A-Za-z]+)*" ForeColor="Red" Display="Dynamic" ControlToValidate="txtEmail"></asp:RegularExpressionValidator>
                            
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSubmitEmil" runat="server" Text="送出" CssClass="btn btn-default" OnClick="btnSubmitEmil_Click" />
                            <asp:Button ID="Button2" runat="server" Text="取消" CssClass="btn btn-default" data-dismiss="modal"  CausesValidation="false" OnClick="Button2_Click"/>
                                                       
                        </div>
                    </div>
                </div>
            </div>



    <%-- <script type="text/javascript">
         function Forget() {
             var isSafari = navigator.userAgent.search("Safari") > -1;//Google瀏覽器是用這核心
             var Url = 'memberLoginValidate_Forgetpsd.aspx';
             if (isSafari)
                 window.open(Url, '忘記密碼', "toolbar=no,scrollbars=no,resizable=yes,left=650,top=450,width=250,height=100,status=no, location=no,directories=no,menubar=no,center=yes");
            else
                 window.open(Url, '忘記密碼', "toolbar=no,scrollbars=no,resizable=yes,top=250,width=250,height=100,status=no, location=no,directories=no,menubar=no,center=yes");
        }
    </script>--%>
</asp:Content>

