<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdmin/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="ShowStats.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.ShowStats" %>
<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

    <asp:PlaceHolder ID="plcShowStats" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Show Statistics</legend>

            <asp:CheckBox ID="chkExcludeRemoved" Text="Exclude Removed?" Checked="true" AutoPostBack="true" OnCheckedChanged="chkExclude_CheckChanged" runat="server" />&nbsp;
            <asp:CheckBox ID="chkExcludeAlliance" Text="Exclude Alliance?" Checked="true" AutoPostBack="true" OnCheckedChanged="chkExclude_CheckChanged" runat="server" />&nbsp;
            <asp:CheckBox ID="chkExcludeReceived" Text="Exclude Received?" Checked="true" AutoPostBack="true" OnCheckedChanged="chkExclude_CheckChanged" runat="server" />
            &nbsp;&nbsp;
            <asp:RadioButtonList ID="rdoListSortBy" CssClass="inputText" RepeatDirection="Horizontal" runat="server">
                            <asp:ListItem Value="0" Selected="True">Company, Show Date</asp:ListItem>
                            <asp:ListItem Value="1">Show Date, Company</asp:ListItem>
                        </asp:RadioButtonList>
            <asp:Button ID="btnRefresh" Text="Refresh" OnClick="btnRefresh_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Label ID="lblListRowCount" Text="" CssClass="techieInfo" runat="server" />&nbsp;&nbsp;
            <asp:Label ID="lblInvoiceTotals" Text="" CssClass="techieInfo" runat="server" /><br />


                <asp:GridView EnableViewState="true"
                        runat="server" ID="grdvwShowStats" AllowPaging="false" AllowSorting="false"
                        AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" OnRowCommand="grdvwShowStats_RowCommand" 
                        RowStyle-CssClass="dataRow" AlternatingRowStyle-CssClass="dataRowAlt"
                        EmptyDataText="No shows to view." PageSize="200" GridLines="Both">
                        <Columns>
                            <asp:TemplateField HeaderText="Owner Name" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                                <ItemTemplate>
                                    <%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "OwnerName").ToString())%>
                                    (<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "Terms").ToString())%>)
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Show Name" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                                <ItemTemplate>
                                    <asp:LinkButton Visible="true" ID="lbtnEditShowStat" CommandName="EditShowStat" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' runat="server">
                                        <%# DataBinder.Eval(Container.DataItem, "ShowId")%>.&nbsp;<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "ShowName").ToString())%>
                                    </asp:LinkButton>
                                    <br />
                                    <%# DataBinder.Eval(Container.DataItem, "ShowGuid")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Dates" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                                <ItemTemplate>
                                <%# String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(Container.DataItem, "ShowStartDate"))%> -
                                <%# String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(Container.DataItem, "ShowEndDate"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="# Exhibitors" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "NumberExhibitors")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Order Total" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "OrderTotal"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Invoice Amount" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "InvoiceAmount"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Amount Paid" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "AmountPaid"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Status" HeaderStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "InvoiceStatus")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText=" " HeaderStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <a href='Archive.aspx?showid=<%# DataBinder.Eval(Container.DataItem, "ShowId")%>'>Archive</a>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcShowStatDetail" Visible="false" runat="server">
    <table border="0" width="50%" cellpadding="1" cellspacing="0">
        
        <tr>
            <td class="contentLabelRight">Show Id:</td>
            <td>
                <asp:Label ID="lblShowId" runat="server" />
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">Show Name:</td>
            <td>
                <asp:Label ID="lblShowName" runat="server" />
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">Owner:</td>
            <td>
                <asp:Label ID="lblOwnerName" runat="server" />
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">Invoiced Amount:</td>
            <td>
                <asp:TextBox ID="txtInvoicedAmount" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">Invoice Date:</td>
            <td>
                <telerik:RadDatePicker ID="txtInvoiceDate" runat="server" Width="100px" AutoPostBack="false"
                    DateInput-EmptyMessage="" DateInput-Width="100" DateInput-CssClass="inputText" CssClass="inputText" MinDate="01/01/1000" MaxDate="01/01/3000">
                    <Calendar ShowRowHeaders="false">
                        <SpecialDays>
                            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                        </SpecialDays>
                    </Calendar>
                </telerik:RadDatePicker>
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">Amount Paid:</td>
            <td>
                <asp:TextBox ID="txtAmountPaid" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">Date Paid:</td>
            <td>
                <telerik:RadDatePicker ID="txtPaymentReceiveDate" runat="server" DateInput-CssClass="inputText" CssClass="inputText" Width="100px"
                    DateInput-EmptyMessage="" MinDate="01/01/1000" MaxDate="01/01/3000" >
                    <Calendar ShowRowHeaders="false">
                        <SpecialDays>
                            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                        </SpecialDays>
                    </Calendar>
                </telerik:RadDatePicker>
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">Status:</td>
            <td>
                <asp:DropDownList ID="ddlShowInvoiceStatus" CssClass="inputText" runat="server">
                    <asp:ListItem Value="">-- Select One --</asp:ListItem>
                    <asp:ListItem Value="Pending">Pending</asp:ListItem>
                    <asp:ListItem Value="Sent">Sent</asp:ListItem>
                    <asp:ListItem Value="Received">Received</asp:ListItem>
                    <asp:ListItem Value="Removed">Removed</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        
    </table>
    
    
    <asp:Button ID="btnSaveShowStat" Text="Save" OnClick="btnSaveShowStat_Click" runat="server" />
    &nbsp;&nbsp;
    <asp:Button ID="btnCancelShowStat" Text="Cancel" OnClick="btnCancelShowStat_Click" runat="server" />

    <asp:HiddenField ID="hdnShowId" runat="server" />
    </asp:PlaceHolder>
</asp:Content>
