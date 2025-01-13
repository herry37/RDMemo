<%@ Page Title="" Language="C#" MasterPageFile="Top.master" AutoEventWireup="true" CodeFile="ServiceList.aspx.cs" Inherits="ServiceList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div >
        <asp:SqlDataSource ID="SqlService" runat="server" ConnectionString="<%$ ConnectionStrings:C501_04ConnectionString %>"
            SelectCommand="select * from Cs_User RIGHT JOIN
 ( select cu_sid,
  (select ','+cm_desc   from Cs_Message t1 where t1.cu_sid=t2.cu_sid for xml path(''))cm_desc
  from Cs_Message t2
  group by cu_sid) as t3  on Cs_User.cu_sid=t3.cu_sid"></asp:SqlDataSource>
        <br /> <a href="Service.aspx" class="btn btn-success">返回客服系統</a>
        &nbsp;&nbsp;&nbsp;&nbsp
        <div style="width:100%"><h2 style="text-align:center">客服記錄</h2></div>
        <asp:ListView ID="ServiecListView" runat="server" DataSourceID="SqlService" DataKeyNames="cu_sid1" OnItemCommand="ServiecListView_ItemCommand">

            <AlternatingItemTemplate>
                <tr style="background-color: #FAFAD2; color: #284775;text-align:center;font-size:16px">
                    <td>
                        <asp:Label ID="cu_sidLabel" runat="server" Text='<%# Eval("cu_sid1") %>' />
                    </td>
                    <td>
                        <asp:Label ID="cu_nameLabel" runat="server" Text='<%# Eval("cu_name") %>' />
                    </td>
                    <td>
                        <asp:Label ID="cu_rtnLabel" runat="server" Text='<%# Eval("cu_rtn") %>' />
                    </td>
                    <td>
                        <asp:Label ID="cu_timeLabel" runat="server" Text='<%# Eval("cu_time") %>' />
                    </td>
                    <td>
                        <asp:Label ID="cu_utimeLabel" runat="server" Text='<%# Eval("cu_utime") %>' />
                    </td>
                    <td>
                        <asp:Label ID="cu_stimeLabel" runat="server" Text='<%# Eval("cu_stime") %>' />
                    </td>
                    <td>
                        <asp:Label ID="mg_sidLabel" runat="server" Text='<%# Eval("mg_sid") %>' />
                    </td>                 
                    <td>
                        <asp:LinkButton runat="server" CommandName="Select" ><%# Eval("cu_sid1") %></asp:LinkButton>
                    </td>
                </tr>
            </AlternatingItemTemplate>
          
            <EmptyDataTemplate>
                <table runat="server" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px;">
                    <tr>
                        <td>未傳回資料。</td>
                    </tr>
                </table>
            </EmptyDataTemplate>

            <ItemTemplate>
                <tr style="background-color: #FFFBD6; color: #333333;text-align:center;font-size:16px">
                    <td>
                        <asp:Label ID="cu_sidLabel" runat="server" Text='<%# Eval("cu_sid1") %>' />
                    </td>
                    <td>
                        <asp:Label ID="cu_nameLabel" runat="server" Text='<%# Eval("cu_name") %>' />
                    </td>
                    <td>
                       <asp:Label ID="cu_rtnLabel" runat="server" Text='<%# Eval("cu_rtn") %>' />
                    </td>
                    <td>
                         <asp:Label ID="cu_timeLabel" runat="server" Text='<%# Eval("cu_time") %>' />
                    </td>
                    <td>
                        <asp:Label ID="cu_utimeLabel" runat="server" Text='<%# Eval("cu_utime") %>' />
                    </td>
                    <td>
                        <asp:Label ID="cu_stimeLabel" runat="server" Text='<%# Eval("cu_stime") %>' />
                    </td>
                    <td>
                        <asp:Label ID="mg_sidLabel" runat="server" Text='<%# Eval("mg_sid") %>' />
                    </td>
                    <td>
                        <asp:LinkButton runat="server" CommandName="Select" ><%# Eval("cu_sid1") %></asp:LinkButton>                             
                    </td>
    
                </tr>
            </ItemTemplate>
            <LayoutTemplate>
                
                <table runat="server" style="width:100%" >
                    <tr runat="server">
                        <td runat="server">
                            <table id="itemPlaceholderContainer" runat="server" border="1" style="width:100%;background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px; font-family: Verdana, Arial, Helvetica, sans-serif;">
                                <tr runat="server" style="background-color: #FFFBD6; color: #333333;">
                                    <th runat="server" style="text-align:center;font-size:16px">服務要求編號</th>
                                    <th runat="server" style="text-align:center;font-size:16px">使用者名稱</th>
                                    <th runat="server" style="text-align:center;font-size:16px">回應狀態</th>
                                    <th runat="server" style="text-align:center;font-size:16px">要求提出時間</th>
                                    <th runat="server" style="text-align:center;font-size:16px">客戶回應時間</th>
                                    <th runat="server" style="text-align:center;font-size:16px">服務回應時間</th>
                                    <th runat="server" style="text-align:center;font-size:16px">客服編號</th>                                    
                                    <th runat="server" style="text-align:center;font-size:16px">內容</th>                                 
                                </tr>
                                <tr runat="server" id="itemPlaceholder">
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
                        <td runat="server" style="text-align: center; background-color: #FFCC66; font-family: Verdana, Arial, Helvetica, sans-serif; color: #333333;font-size:14px;text-align:center;">
                            <asp:DataPager ID="DataPager1" runat="server" PageSize="20">
                                <Fields>
                                    <asp:NextPreviousPagerField ButtonType="Button" ShowFirstPageButton="True" ShowLastPageButton="True" />
                                </Fields>
                            </asp:DataPager>
                        </td>
                    </tr>
                </table>
                  
            </LayoutTemplate>
            <SelectedItemTemplate>
                <tr style="background-color: #FFCC66; font-weight: bold; color: #000080;font-size:16px">
                    <td>
                        <asp:LinkButton runat="server" CommandName="CnacelSeleted" Text="取消"></asp:LinkButton>                       
                    </td>                   
                    <td colspan="7">
                        <asp:Label ID="cm_descLabel" runat="server" Text='<%# Eval("cm_desc") %>' />
                    </td>
                </tr>
            </SelectedItemTemplate>
        </asp:ListView>
    </div>
</asp:Content>

