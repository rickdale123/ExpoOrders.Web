<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnerLanding.Master" AutoEventWireup="true" CodeBehind="Stats.aspx.cs" Inherits="ExpoOrders.Web.Owners.Stats" %>
<%@ MasterType VirtualPath="~/Owners/OwnerLanding.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

<script language="javascript" type="text/javascript">
    function displayExhibitors(showId) {

        hideObject('displayExhibitors_' + showId);
        showObject('hideExhibitors_' + showId);
        showObject('trShowExhibitors_' + showId);
    }

    function hideExhibitors( showId) {
        showObject('displayExhibitors_' + showId);
        hideObject('hideExhibitors_' + showId);
        hideObject('trShowExhibitors_' + showId);
    }

    
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">
<CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

    <fieldset class="commonControls">
        <legend class="commonControls">Show Stats</legend>
 

        <asp:Button ID="btnDisplayStats" Visible="true" Text="Display Event Stats" CssClass="button" OnClick="btnDisplayStats_Click" runat="server" />
        <asp:CheckBox ID="chkShowInActive" Visible="true" Text="Display Inactive Shows" Checked="false" AutoPostBack="false" runat="server" /><br />

        <asp:PlaceHolder id="plcShowStats" Visible="false" runat="server">
            <asp:Label id="lblNoSearchResults" text="No Search Results Found" Visible="false" runat="server" />
            <asp:Repeater ID="rptrShowStats" OnItemCommand="rptrShowStats_ItemCommand" Visible="false" runat="server">
                <HeaderTemplate>
                    <table border="0" cellpadding="2" cellspacing="1">
                        <tr>
                            <th style="white-space:nowrap;">&nbsp;</th>
                            <th style="white-space:nowrap;">Show ID</th>
                            <th style="white-space:nowrap;">Show Name</th>
                            <th style="white-space:nowrap;">Dates</th>
                            <th style="white-space:nowrap;"># of Exhibitors</th>
                            <th style="white-space:nowrap;">Order Total</th>
                            <th style="white-space:nowrap;">Receivables</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                        <tr class='<%# GetAlternatingClassName(Container.ItemIndex, "altItem", "item")%>'>
                            <td style="white-space:nowrap;">
                                <a name='show_<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' />
                                <!-- <a id='displayExhibitors_<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' style="display: block;" href='#show_<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' onclick='displayExhibitors(<%# DataBinder.Eval(Container.DataItem, "ShowId")%>)'>[ + ]</a>
                                <a id='hideExhibitors_<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' style="display: none;" href='#show_<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' onclick='hideExhibitors(<%# DataBinder.Eval(Container.DataItem, "ShowId")%>)'>[ - ]</a>
                                    -->
                            </td>
                            <td style="white-space:nowrap;"><asp:Label ID="lblShowId" Text='<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' runat="server" /></td>
                            <td style="width:350px;">
                                <asp:LinkButton Visible="true" ID="lbtnGoToShow" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "ShowName").ToString())%>' CommandName="GoToShow"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' runat="server" /><br />
                                    &nbsp;<%# DataBinder.Eval(Container.DataItem, "VenueName")%>
                            </td>
                           <td style="white-space:nowrap;"><%# DataBinder.Eval(Container.DataItem, "ShowDatesDisplay")%></td>
                           <td style="white-space:nowrap; text-align:center"><%# DataBinder.Eval(Container.DataItem, "NumberExhibitors")%></td>
                           <td style="white-space:nowrap; text-align:right"><%# DataBinder.Eval(Container.DataItem, "OrderTotalFormatted")%></td>
                           <td style="white-space:nowrap; text-align:right"><%# DataBinder.Eval(Container.DataItem, "TotalReceivablesFormatted")%></td>
                        </tr>
                        <tr id='trShowExhibitors_<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' style="display: none;">
                            <td>&nbsp;</td>
                            <td colspan="5" valign="top">
                                    <asp:Repeater ID="rptrShowExhibitor" OnItemCommand="rptrShowExhibitor_ItemCommand" OnItemDataBound="rptrShowExhibitor_ItemDataBound" Visible="false" runat="server">
                                        <HeaderTemplate>
                                            <table border="0" cellpadding="3" cellspacing="1" width="100%">
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                                <tr id="trColumnHeaders" class="altItem" runat="server">
                                                    <th scope="col" style="white-space:nowrap;">Invoice # (Id)</th>
                                                    <th scope="col" style="white-space:nowrap;">Company Name</th>
                                                    <th scope="col" style="white-space:nowrap;">Contact Info</th>
                                                    <th scope="col" style="white-space:nowrap;">Booth #</th>
                                                    <th>&nbsp;</th>
                                                    <th scope="col" style="white-space:nowrap;">Order Total</th>
                                                    <th scope="col" style="white-space:nowrap; text-align:right">Balance</th>
                                                </tr>
                                                <tr class="altItem">
                                                    <td style="white-space:nowrap;"><%# DataBinder.Eval(Container.DataItem, "ExhibitorId")%></td>
                                                    <td style="width: 300;">
                                                        <asp:LinkButton Visible="true" ID="lbtnGoToExhibitor" Text='<%# DataBinder.Eval(Container.DataItem, "ExhibitorCompanyNameDisplay")%>' CommandName="GoToExhibitor" runat="server" />
                                                    </td>
                                                    <td>
                                                        <%# DataBinder.Eval(Container.DataItem, "PrimaryContactName")%><br />
                                                        <a id="lnkMailToExhibitor" class="mailTo" runat="server"></a>
                                                    </td>
                                                    <td style="white-space:nowrap;">
                                                            <%# DataBinder.Eval(Container.DataItem, "BoothNumber")%><br />
                                                            (<%# DataBinder.Eval(Container.DataItem, "BoothDescription")%>)
                                                    </td>
                                                    <td style="white-space:nowrap;">
                                                        <a id="lnkPrintInvoice" class="action-link" href="#" style="text-align: right;" runat="server">View Invoice</a>
                                                    </td>
                                                    <td style="text-align:right"><%# DataBinder.Eval(Container.DataItem, "OrderTotalFormatted")%></td>
                                                    <td style="text-align:right"><%# DataBinder.Eval(Container.DataItem, "TotalReceivablesFormatted")%></td>
                                                
                                                </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                            </td>
                        </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            
        </asp:PlaceHolder>

    </fieldset>
</asp:Content>
