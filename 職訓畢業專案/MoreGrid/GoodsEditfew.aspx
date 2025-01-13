<%@ Page Title="" Language="C#" MasterPageFile="Left.master" AutoEventWireup="true" CodeFile="GoodsEditfew.aspx.cs" Inherits="GoodsEditfew" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentCSS" Runat="Server">
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
<asp:Content ID="Content2" ContentPlaceHolderID="CContent" Runat="Server">
    <h2>商品修改畫面</h2>
    <asp:Label runat="server" ID="txtMessage"></asp:Label>
    <asp:Panel runat="server" >
        
        <br />
        <%--抓GoodsData--%>
        <asp:SqlDataSource ID="SqlDataGoodsEditfew" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" 
                            ProviderName="System.Data.SqlClient"
                            UpdateCommand="update [商品] set [商品名稱]=@商品名稱, [商品摘要]=@商品摘要, [商品明細]=@商品明細, 
                                        [庫存量]=@庫存量, [單價]=@單價, [上架時間]=@上架時間, [下架時間]=@下架時間,
                                        [商品小圖片]=@商品小圖片, [商品大圖片1]=@商品大圖片1, [商品大圖片2]=@商品大圖片2, [商品大圖片3]=@商品大圖片3, 
                                        [商品類別代碼]=@商品類別代碼, [編輯]=@編輯 where [商品編號]=@商品編號"
                            >
        </asp:SqlDataSource>
        
        <asp:FormView ID="FormViewGoodsEditfew" runat="server" EditRowStyle-Font-Size="14" DataKeyNames="商品編號" DataSourceID="SqlDataGoodsEditfew"
             EditRowStyle-Font-Bold="true" OnItemUpdating="FormViewGoodsEditfew_ItemUpdating" OnDataBound="FormViewGoodsEditfew_DataBound" >
            <ItemTemplate>
                <table class="auto-style1" border="1" >
                    
                </table>  
            </ItemTemplate>
            <EditItemTemplate>
                <table class="auto-style1" border="1" >
                    <tr>
                        <td >&nbsp;<asp:Label runat="server" Text="商品名稱" Font-Bold="true" Font-Size="14"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" ID="txtName" Text='<%#Bind("商品名稱") %>' Width="400" ></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtName"></asp:RequiredFieldValidator>
                            
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="商品摘要"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" CssClass="txtstyle" ID="txtSummary" Text='<%#Bind("商品摘要") %>' TextMode="MultiLine" Rows="2" Columns="40" ></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtSummary"></asp:RequiredFieldValidator></td>
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="商品明細"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" CssClass="txtstyle" ID="txtDetails" Text='<%#Bind("商品明細") %>' TextMode="MultiLine" Rows="8" Columns="40" ></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtDetails"></asp:RequiredFieldValidator></td>
                 
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="庫存量" Visible="true"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" ID="txtStock" Text='<%#Bind("庫存量") %>' ></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtStock"></asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator runat="server" ControlToValidate="txtStock" ErrorMessage="請填寫數字" ForeColor="red" Display="Dynamic" ValidationExpression="\d+"></asp:RegularExpressionValidator>

                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="單價"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" ID="txtPrice" Text='<%# Bind("單價","{0:0}")%>' ViewState="false"></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtPrice"></asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPrice" Display="Dynamic" ErrorMessage="請填寫數字" ValidationExpression="\d+" ForeColor="Red"></asp:RegularExpressionValidator></td>
                        
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="上架時間"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" ID="txtDataUp" onclick="SetDate(this.id)" Text='<%# Bind("上架時間","{0:d}") %>' ></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtDataUp"></asp:RequiredFieldValidator>
                          <asp:CompareValidator runat="server" ErrorMessage="請檢查日期格式" ForeColor="Red" ControlToValidate="txtDataUp" Type="Date" Operator="DataTypeCheck" Display="Dynamic" ></asp:CompareValidator></td>
                      
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="下架時間"></asp:Label></td>
                        <td colspan="3">&nbsp;<asp:TextBox runat="server" ID="txtDataDown" onclick="SetDate(this.id)" Text='<%# Bind("下架時間", "{0:d}") %>' ></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ErrorMessage="必需填寫" ForeColor="Red" Display="Dynamic" ControlToValidate="txtDataDown"></asp:RequiredFieldValidator>
                         <asp:CompareValidator runat="server" ErrorMessage="請檢查日期格式" ForeColor="Red" ControlToValidate="txtDataDown" Type="Date" Operator="DataTypeCheck" Display="Dynamic" ></asp:CompareValidator>
                          <asp:CompareValidator runat="server" ErrorMessage="不得少於上架時間" ControlToValidate="txtDataDown" ControlToCompare="txtDataUp" Operator="GreaterThanEqual" Type="Date" Display="Dynamic" ForeColor="#3333CC"></asp:CompareValidator></td>
                    </tr>
                    <tr>
                        <td>&nbsp;<asp:Label runat="server" Text="商品照片<br>(限傳jpg、jpeg、png、gif)"></asp:Label></td>
                        <td>
                            <div><img class="preview1" src="goodPicture/<%#Eval("商品大圖片1") %>" style="max-height:150px; max-width:150px" /></div>
                            <asp:FileUpload CssClass="upl1" ID="FileUpload1" runat="server" AllowMultiple="false" Width="150px" /> 
                            <%--<img class="preview1" style="max-width: 150px; max-height: 150px;"> --%>                                   
                                 
                        </td>
                        <td>    
                            <img class="preview2" src="goodPicture/<%#Eval("商品大圖片2") %>" style="max-height:150px; max-width:150px" />
                            <asp:FileUpload CssClass="upl2" ID="FileUpload2" runat="server" AllowMultiple="false" Width="100px" />
                        </td>
                        <td>    
                            <img class="preview3" src="goodPicture/<%#Eval("商品大圖片3") %>" style="max-height:150px; max-width:150px" />
                            <asp:FileUpload CssClass="upl3" ID="FileUpload3" runat="server" AllowMultiple="false" Width="150px" />
                        </td>                 
                        <%--<td>&nbsp<asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="false" /></td>--%>
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
                                        <asp:ListItem Value=""></asp:ListItem>
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
                         <td colspan="3">&nbsp;<asp:Label runat="server" ID="lblCKE" Text='<%# Eval("編輯") %>' Visible="false" ></asp:Label>
                           <asp:TextBox runat="server" CssClass="ckeditor" ID="txtedit" TextMode="MultiLine" Rows="15" Columns="80" ></asp:TextBox>  
                           <%--<asp:TextBox runat="server" CssClass="ckeditor" ID="txtedit" Text='<%# Bind("編輯") %>' TextMode="MultiLine" Rows="15" Columns="80" ></asp:TextBox>--%>
                        </td>
                    </tr>                   
                    <tr style="text-align: center; height:50px;">
                        <td colspan="2">&nbsp;<asp:Button ID="ButEdit" runat="server" CausesValidation="true" CssClass="btn btn-primary"
                            CommandName="Update" Text="更新" /></td>
                        <td colspan="2">&nbsp;<asp:Button ID="ButCancel" runat="server" CausesValidation="false" CssClass="btn btn-success"
                            CommandName="Cancel" Text="取消" OnClick="ButCancel_Click"></asp:Button></td>
                    </tr>
                </table>  
                <br /><br /><br />          
            </EditItemTemplate>
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
                if (Date.parse(endtime) > (Date.parse(starttime) + (days * 180))) {
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

