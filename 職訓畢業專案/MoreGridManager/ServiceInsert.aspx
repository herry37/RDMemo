<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="ServiceInsert.aspx.cs" Inherits="ServiceInsert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
      <link href="../css/bootstrap.css" rel="stylesheet" />
    <style type="text/css">
              #content {
            margin: 0 auto;
            width: 70%;
        }
        .heardtxt {
           text-align:center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <br /><br />
        <%--新增客服人員--%>
        <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#myModal">
        新增人員
    </button>&nbsp;&nbsp;&nbsp;&nbsp
     <a href="Service.aspx" class="btn btn-success">返回客服系統</a>
    

    <%--顯示客服人員--%>
    <h2 style="text-align:center">客服人員</h2>
    <asp:SqlDataSource ID="Sqlgv_Manager" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>" DeleteCommand="DELETE FROM [Manager] WHERE [mg_sid] = @mg_sid" InsertCommand="INSERT INTO [Manager] ([mg_name],  [mg_id]) VALUES (@mg_name,  @mg_id)" SelectCommand="SELECT * FROM [Manager]" UpdateCommand="UPDATE [Manager] SET [mg_name] = @mg_name,[mg_id] = @mg_id WHERE [mg_sid] = @mg_sid">
        <DeleteParameters>
            <asp:Parameter Name="mg_sid" Type="Int32" />
        </DeleteParameters>
        <InsertParameters>
            <asp:ControlParameter ControlID="txt_mg_name" Name="mg_name" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txt_mg_id" Name="mg_id" PropertyName="Text" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="mg_name" Type="String" />
        
            <asp:Parameter Name="mg_id" Type="String" />
            <asp:Parameter Name="mg_sid" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:GridView ID="gv_Manager" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="mg_sid" DataSourceID="Sqlgv_Manager" BackColor="White" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1" Font-Size="16px" GridLines="None" HorizontalAlign="Center" Width="100%">
        <AlternatingRowStyle HorizontalAlign="Center" />
        <Columns>
            <asp:CommandField ShowEditButton="True" />
            <asp:BoundField DataField="mg_sid" HeaderText="客服編號" InsertVisible="False" ReadOnly="True" SortExpression="mg_sid" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
<HeaderStyle HorizontalAlign="Center" CssClass="heardtxt"></HeaderStyle>

                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="mg_name" HeaderText="人員名字" SortExpression="mg_name" ItemStyle-HorizontalAlign="Center">
                <HeaderStyle CssClass="heardtxt" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="last_date" HeaderText="最後上線時間" SortExpression="last_date" ReadOnly="true" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
<HeaderStyle HorizontalAlign="Center" CssClass="heardtxt"></HeaderStyle>

                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="mg_id" HeaderText="員編" SortExpression="mg_id" ItemStyle-HorizontalAlign="Center">
                <HeaderStyle CssClass="heardtxt" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
                        <asp:CommandField ShowDeleteButton="True" />
        </Columns>
        <EditRowStyle Font-Size="14px" HorizontalAlign="Center" Width="500px" />
        <FooterStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Center" />
        <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#E7E7FF" HorizontalAlign="Center"  />
        <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />
        <RowStyle BackColor="#DEDFDE" ForeColor="Black" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#9471DE" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
        <SortedAscendingCellStyle BackColor="#F1F1F1" HorizontalAlign="Center" />
        <SortedAscendingHeaderStyle BackColor="#594B9C" HorizontalAlign="Center" />
        <SortedDescendingCellStyle BackColor="#CAC9C9" />
        <SortedDescendingHeaderStyle BackColor="#33276A" />
    </asp:GridView>
    <br />
    &nbsp;<br />
   


    <!-- Modal -->
    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    <h4 class="modal-title" id="myModalLabel" style="text-align:center">新增客服人員資料</h4>
                </div>
                <div class="modal-body">
                    <div id="content">
                    <div style="width: 400px;margin:auto">
                        <table style="align-content: center; text-align: center; font-size: 20px; background-color: #00ffff" class="nav-justified">
                            <tr>
                             <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" Text="客服名稱"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txt_mg_name" Text='<%#Bind("mg_name") %>'></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" Text="員編"></asp:Label></td>
                                <td>
                                    <asp:TextBox runat="server" ID="txt_mg_id" Text='<%#Bind("mg_id") %>'></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                          
                            </tr>

                        </table>
                    </div>
                </div>

                </div>

                <div class="modal-footer">

                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    <asp:Button type="button" runat="server" class="btn btn-primary" OnClick="btnInsert_Click" Text="Save" />

                </div>
            </div>
        </div>
    </div>

    <script src="../Scripts/jquery-2.2.1.js"></script>
    <script src="../js/bootstrap.js"></script>

</asp:Content>

