<%@ Page Title="" Language="C#" MasterPageFile="Left.master" AutoEventWireup="true" CodeFile="GoodsInsert.aspx.cs" Inherits="GoodsInsert" ValidateRequest="false" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" runat="Server">
    <style type="text/css">
        .auto-style1 {
            width: 100%;
            border-color: #FFFFFF;
            background-color: #F2DEDE;   
           
        }
        .txtstyle{
            resize:none;
        }
    </style>
     <%--崁入ckeditor--%>
    <script src="ckeditor/ckeditor.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CContent" runat="Server">
 
    <h2>新增商品</h2>
    
    <asp:Label runat="server" ID="txtMessage"></asp:Label>
    <asp:Panel runat="server" >
        
        <asp:FormView ID="FormViewGoodsInsert" runat="server" DefaultMode="Insert"  Width="1000px" 
            OnItemInserting="FormViewGoodsInsert_ItemInserting" OnItemCommand="FormViewGoodsInsert_ItemCommand" EditRowStyle-Font-Size="14"
             EditRowStyle-Font-Bold="true" OnItemCreated="FormViewGoodsInsert_ItemCreated" >
            <InsertItemTemplate>
                <table class="auto-style1" border="1" >
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="商品名稱" Font-Bold="true" Font-Size="14"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" ID="txtName" Text='<%# Bind("商品名稱") %>' Width="400" ></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtName"></asp:RequiredFieldValidator>
                               <asp:RegularExpressionValidator ControlToValidate="txtName" runat="server" ErrorMessage="只能輸入30字數" Font-Bold="true" ForeColor="Red" ValidationExpression=".{0,30}"></asp:RegularExpressionValidator>
                            
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="商品摘要"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" CssClass="txtstyle" ID="txtSummary" Text='<%# Bind("商品摘要") %>' TextMode="MultiLine" Rows="2" Columns="40"  ></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtSummary"></asp:RequiredFieldValidator>

                            <%--<asp:RegularExpressionValidator ControlToValidate="txtSummary" runat="server" ErrorMessage="只能輸入60字數" Font-Bold="true" ForeColor="Red" ValidationExpression=".{0,60}"></asp:RegularExpressionValidator>--%>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="商品明細"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" CssClass="txtstyle" ID="txtDetails" Text='<%# Bind("商品明細") %>' TextMode="MultiLine" Rows="8" Columns="40" ></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtDetails"></asp:RequiredFieldValidator></td>
                  <%--<asp:RegularExpressionValidator ControlToValidate="txtDetails" runat="server" ErrorMessage="只能輸入1000字數" Font-Bold="true" ForeColor="Red" ValidationExpression=".{0,1000}"></asp:RegularExpressionValidator>--%>

                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="庫存量" Visible="true"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" ID="txtStock" Text='<%# Bind("庫存量", "{0}") %>' ></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtStock"></asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator runat="server" ControlToValidate="txtStock" ErrorMessage="請填寫數字" ForeColor="red" Display="Dynamic" ValidationExpression="\d+"></asp:RegularExpressionValidator>

                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="單價"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" ID="txtPrice" Text='<%# Bind("單價", "{0:C}") %>' ViewState="false"></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtPrice"></asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPrice" Display="Dynamic" ErrorMessage="請填寫數字" ValidationExpression="\d+" ForeColor="Red"></asp:RegularExpressionValidator></td>
                        
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="上架時間"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" ID="txtDataUp" onclick="SetDate(this.id)" Text='<%# Bind("上架時間", "{0:D}") %>' ></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtDataUp"></asp:RequiredFieldValidator>
                          <asp:CompareValidator runat="server" ErrorMessage="請檢查日期格式" ForeColor="Red" ControlToValidate="txtDataUp" Type="Date" Operator="DataTypeCheck" Display="Dynamic" ></asp:CompareValidator></td>
                      
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="下架時間"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" ID="txtDataDown" onclick="SetDate(this.id)" Text='<%# Bind("下架時間", "{0:D}") %>' ></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtDataDown"></asp:RequiredFieldValidator>
                         <asp:CompareValidator runat="server" ErrorMessage="請檢查日期格式" ForeColor="Red" ControlToValidate="txtDataDown" Type="Date" Operator="DataTypeCheck" Display="Dynamic" ></asp:CompareValidator>
                          <asp:CompareValidator runat="server" ErrorMessage="不得少於上架時間" ControlToValidate="txtDataDown" ControlToCompare="txtDataUp" Operator="GreaterThanEqual" Type="Date" Display="Dynamic" ForeColor="#3333CC"></asp:CompareValidator></td>
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="商品照片<br>(限傳jpg、jpeg、png、gif)"></asp:Label></td>
                        <td>&nbsp;
                            <asp:FileUpload CssClass="upl1" ID="FileUpload1" runat="server" AllowMultiple="false" Width="150px" /> 
                                <div>
                                    <img class="preview1" style="max-width: 150px; max-height: 150px;">                                    
                                </div>                          
                        </td>
                        <td>&nbsp;
                            <asp:FileUpload CssClass="upl2" ID="FileUpload2" runat="server" AllowMultiple="false" Width="150px" />
                                <%--<div><input type='file' class="upl2"></div>--%>
                                <div>
                                    <img class="preview2" style="max-width: 150px; max-height: 150px;">                                    
                                </div>
                        </td>
                        <td>&nbsp;
                            <asp:FileUpload CssClass="upl3" ID="FileUpload3" runat="server" AllowMultiple="false" Width="150px" />
                                <%--<div><input type='file' class="upl3"></div>--%>
                                <div>
                                    <img class="preview3" style="max-width: 150px; max-height: 150px;">
                                </div>
                        </td>
                    </tr>
                    
                    <asp:SqlDataSource ID="SqlCategory" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" 
                        SelectCommand="SELECT * FROM [商品類別]"></asp:SqlDataSource>

                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="商品類別"></asp:Label></td>
                        <td colspan="3">&nbsp;
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false" >
                                <ContentTemplate>
                                    <asp:DropDownList ID="DropDownListCategory" runat="server"
                                        AppendDataBoundItems="True" DataSourceID="SqlCategory"
                                        DataTextField="商品類別名稱" DataValueField="商品類別代碼"
                                        SelectedValue='<%# Bind("商品類別代碼") %>' AutoPostBack="True">
                                        <asp:ListItem Value="0">請選擇</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                                    </asp:ScriptManager>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="DropDownListCategory" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:RequiredFieldValidator runat="server" ErrorMessage="請挑選商品類別" ForeColor="Red" SetFocusOnError="true" ControlToValidate="DropDownListCategory" InitialValue="0"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr> <%--TextBox套裝呼叫ckeditor--%>            
                        <td>&nbsp;<asp:Label runat="server" Text="編輯文章"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:Label runat="server" ID="lblCKE" ></asp:Label>
                           <asp:TextBox runat="server" CssClass="ckeditor" ID="txtedit" Text='<%#Bind("編輯") %>' TextMode="MultiLine" Rows="50" Columns="80" ></asp:TextBox>
                             <%--<asp:RegularExpressionValidator ControlToValidate="txtedit" runat="server" ErrorMessage="只能輸入8000字數" Font-Bold="true" ForeColor="Red" ValidationExpression=".{0,8000}"></asp:RegularExpressionValidator>--%>
                        </td>
                    </tr>                   
                    <tr style="text-align: center; height:50px;">
                        <td colspan="2">&nbsp;<asp:Button ID="ButInsert" runat="server" CausesValidation="true" CssClass="btn btn-primary"
                            CommandName="Insert" Text="新增" OnClick="ButInsert_Click" /></td>
                        <td colspan="2">&nbsp;<asp:Button ID="ButCancel" runat="server" CausesValidation="false" CssClass="btn btn-success"
                            CommandName="Cancel" Text="取消"></asp:Button></td>
                    </tr>
                </table>
                <br /><br /><br />            
            </InsertItemTemplate>
        </asp:FormView>
    </asp:Panel>


    <script type="text/javascript">
     
        function SetDate(ClientID) {
            var Url = 'GoodDate1.aspx?DialogClientID=' + ClientID;
            window.open(Url, 'Calendar', "toolbar=no,scrollbars=no,resizable=yes,top=350,left=500,width=300,height=250,status=no, location=no,directories=no,menubar=no,center=yes");
           
        }
        $(document).ready(function () {
            $('#left_CContent_FormViewGoodsInsert_ButInsert').mousedown(function () {
                var starttime = document.getElementById('left_CContent_FormViewGoodsInsert_txtDataUp').value;
                var endtime = document.getElementById('left_CContent_FormViewGoodsInsert_txtDataDown').value;
                var minutes = 1000 * 60
                var hours = minutes * 60
                var days = hours * 24
                if (Date.parse(endtime) > (Date.parse(starttime)+ (days * 180))) {
                    alert("上架時間至下架時間為期六個月");
                }
                return false;
            });
        });

        //圖片預覽
        $(function () {
            function format_float(num, pos) {
                var size = Math.pow(10, pos);
                return Math.round(num * size) / size;
            }
            function preview1(inpreview1) {
                if (inpreview1.files && inpreview1.files[0]) {
                    var reader = new FileReader();                    
                    reader.onload = function (e) {
                        $('.preview1').attr('src', e.target.result);                        
                    }                    
                    reader.readAsDataURL(inpreview1.files[0]);
                }
            }            
            $("body").on("change", ".upl1", function () {
                preview1(this);
            })

            function preview2(inpreview2) {
                if (inpreview2.files && inpreview2.files[0]) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $('.preview2').attr('src', e.target.result);
                    }
                    reader.readAsDataURL(inpreview2.files[0]);
                }
            }
            $("body").on("change", ".upl2", function () {
                preview2(this);
            })

            function preview3(inpreview3) {
                if (inpreview3.files && inpreview3.files[0]) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $('.preview3').attr('src', e.target.result);
                    }
                    reader.readAsDataURL(inpreview3.files[0]);
                }
            }
            $("body").on("change", ".upl3", function () {
                preview3(this);
            })
        })
    </script>
</asp:Content>
