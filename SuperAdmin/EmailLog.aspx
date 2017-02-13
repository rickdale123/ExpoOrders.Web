<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdmin/SuperAdminMaster.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="EmailLog.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.EmailLog" %>
<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <script type="text/javascript">
     

     $(document).ready(function () {
         var allCheckBoxSelector = '#<%=grdvEmailLog.ClientID%> input[id*="chkAll"]:checkbox';
         var checkBoxSelector = '#<%=grdvEmailLog.ClientID%> input[id*="chkSelected"]:checkbox';

         $(allCheckBoxSelector).click(function () {
             var checkedStatus = this.checked;
             $(checkBoxSelector).each(function () {
                 $(this).prop('checked', checkedStatus);
             });
         });
     });
    </script>

Date To Send: <telerik:RadDatePicker ID="txtStartDate"  DateInput-CssClass="inputText" CssClass="inputText" runat="server" Width="100px" AutoPostBack="false"
                        DateInput-EmptyMessage="" MinDate="01/01/1000" MaxDate="01/01/3000">
                        <Calendar ShowRowHeaders="false">
                            <SpecialDays>
                                <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                            </SpecialDays>
                        </Calendar>
                    </telerik:RadDatePicker>

<asp:Button ID="btnGo" Text="Search" OnClick="btnGo_Click" runat="server" />
&nbsp;&nbsp;
<asp:CheckBox ID="chkOnlyShowErrors" Text="Only Errors" runat="server" />
&nbsp;&nbsp;
<asp:DropDownList ID="ddlEmailStatus" CssClass="inputText" runat="server">
    <asp:ListItem Value="U">UnSent</asp:ListItem>
    <asp:ListItem Value="S">Sent</asp:ListItem>
    <asp:ListItem Value="E">Error</asp:ListItem>
</asp:DropDownList>
<asp:Button ID="btnUpdateAllStatus" Text="Update Status to" OnClick="btnUpdateAllStatus_Click" runat="server" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<asp:Button ID="btnShowAll" Text="Show All" OnClick="btnShowAll_Click" runat="server" />
<hr />

    <asp:PlaceHolder ID="plcEmailLog" Visible="true" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Email Logs</legend>

            <asp:Label ID="lblEmailLogRowCount" Text="" CssClass="techieInfo" runat="server" />
            <asp:GridView EnableViewState="true" OnRowDataBound="grdvEmailLog_RowDataBound"
                runat="server" ID="grdvEmailLog" AllowPaging="false" AllowSorting="true"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="dataRowAlt"
                RowStyle-CssClass="dataRow" GridLines="Both"
                EmptyDataText="No Logs to display.">
                <Columns>
                    <asp:TemplateField HeaderText="">
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkAll" runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSelected" runat="server" />
                            <asp:HiddenField ID="hdnEmailId" Value='<%# DataBinder.Eval(Container.DataItem, "EmailId")%>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Date to Send" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%#Convert.ToInt64(DataBinder.Eval(Container, "RowIndex")) + 1%>.&nbsp;
                            <%# DataBinder.Eval(Container.DataItem, "SendDate")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Status" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "StatusCode")%>
                            &nbsp;&nbsp;
                            make&nbsp;
                            <asp:LinkButton ID="lnkUpdateStatus" Text="{Update Status}" OnClick="lnkUpdateStatus_Click" runat="server" />

                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Addresses">
                        <ItemTemplate>
                            EmailId: <%# DataBinder.Eval(Container.DataItem, "EmailId")%><hr />
                            To: <%# DataBinder.Eval(Container.DataItem, "ToAddress")%><br />
                            From: <%# DataBinder.Eval(Container.DataItem, "FromAddress")%><br />
                            Reply To: <%# DataBinder.Eval(Container.DataItem, "ReplyAddress")%><br />
                            CC: <%# DataBinder.Eval(Container.DataItem, "CCAddress")%><br />
                            Bcc: <%# DataBinder.Eval(Container.DataItem, "BccAddress")%>

                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Subject">
                        <ItemTemplate>
                            Subject: <%# DataBinder.Eval(Container.DataItem, "Subject")%>
                            <br />
                            Attachments: <%# DataBinder.Eval(Container.DataItem, "Attachments")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Body">
                        <ItemTemplate>
                           <asp:TextBox ID="txtEmailBody" CssClass="inputText" TextMode="MultiLine" Rows="4" Columns="35" runat="server" /><br />
                           Processing Details: <%# DataBinder.Eval(Container.DataItem, "ProcessingDetails")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </fieldset>
    </asp:PlaceHolder>

</asp:Content>
