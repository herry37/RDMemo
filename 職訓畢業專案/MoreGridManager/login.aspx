<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta name="viewport" content="device-width" />
    <title></title>
    <link href="css/bootstrap.css" rel="stylesheet" />
    <style>
        /*#master a{color:white;
        }
        #master,master div{            
            background-color:#48a6cf ;vertical-align:central;text-align:center;
        }
        #accordion div{width:150px;text-align:center}*/
        .jumbotron{text-shadow:2px 2px 4px;}
        .panel-info > .panel-heading {background-color:#48a6cf;color:white}
        #container{position:absolute;left:1024px;top:10px}
        #logo{position:absolute}
    </style>
   
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="jumbotron">
                <div class="container">
                    <h2>MORE GRID 員工入口網</h2>
                    <br />
                    <a href="../MoreGrid/Main.aspx" target="_blank" class="btn btn-primary">魔格主網</a>
                    
                </div>
            </div>
            <div class="container" style="width: 440px;margin-top:100px">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        <h5>員工帳號登入</h5>
                    </div>
                    <div class="panel-body">
                        <div class="form-horizontal">
                            <div class="form-group alert-danger">
                                <asp:Label ID="labmsg" runat="server" Text=""></asp:Label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtAccount" ErrorMessage="帳號不能空白" Display="Dynamic" ValidationGroup="login_in"></asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtpwd" ErrorMessage="密碼不能空白" Display="Dynamic" ValidationGroup="login_in"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group">
                                <label class="col-md-2 control-label" for="txtAccount">帳號</label>
                                <div class="col-md-9 input-group">
                                    <span class="input-group-addon"><i class="glyphicon glyphicon-user"></i></span>
                                    <asp:TextBox ID="txtAccount" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-md-2 control-label" for="txtpwd">密碼</label>
                                <div class="col-md-9 input-group">
                                    <span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span>
                                    <asp:TextBox ID="txtpwd" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <a href="#" id="btn1" class="col-md-offset-4 col-sm-3" data-toggle="collapse" data-target="#myModal">忘記密碼</a>
                                <a href="EmployAccount.aspx" target="_blank">申請帳號</a>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer text-right">
                        <asp:Button ID="Button1" runat="server" Text="登入" CssClass="btn btn-primary" ValidationGroup="login_in" CausesValidation="true" OnClick="Button1_Click" />
                        <asp:Button ID="Button2" runat="server" Text="取消" CssClass="btn btn-default" CausesValidation="false" OnClick="Button2_Click" />
                    </div>
                </div>
            </div>
            <div class="modal" id="myModal">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h3 class="modal-title">
                                <asp:Label ID="Label4" runat="server" Text="忘記密碼"></asp:Label></h3>
                        </div>
                        <div class="modal-body">
                            <h3>
                                <asp:Label ID="Label5" runat="server" Text="輸入e-mail"></asp:Label></h3>
                            <asp:TextBox ID="txtEmail" runat="server" Width="250"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ErrorMessage="(請檢查格式)" ValidationExpression="\w+([-+.']\w+)*@[A-Za-z]+([-.][A-Za-z]+)*\.[A-Za-z]+([-.][A-Za-z]+)*" ForeColor="Red" Display="Dynamic" ControlToValidate="txtEmail"></asp:RegularExpressionValidator>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnSubmitEmil" runat="server" Text="送出" CssClass="btn btn-default" OnClick="btnSubmitEmil_Click" />
                            <asp:Button ID="Button3" runat="server" Text="取消" CssClass="btn btn-default" data-dismiss="modal" CausesValidation="false" OnClick="Button3_Click" />

                        </div>
                    </div>
                </div>
            </div>
            <div id="container">
<%--                <img src="GSHAN.png" id="=logo"/>
                <img src="moveGshan.gif" />--%>
                <img src="moveGshan2.gif" width="150" height="150"/>
            </div>
        </div>
    </form>
    <script src="Scripts/jquery-1.9.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>
        <script>
            function a(){
                $('#container').animate({ 'left': '-=400' }, 5000, 'swing', function () {
                    $('#container').animate({ 'left': '+=400' }, 5000),  a()
                  })};

                $(document).ready(a());
        </script>

</body>
</html>
