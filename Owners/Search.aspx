<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnerLanding.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="ExpoOrders.Web.Owners.Search" %>
<%@ MasterType VirtualPath="~/Owners/OwnerLanding.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

<CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

    <fieldset class="commonControls">
        <legend class="commonControls">Exhibitor Search</legend>

        <asp:PlaceHolder id="plcSearchCriteria" Visible="false" runat="server">
            <table class="searchTable" border="0" cellpadding="1" cellspacing="1">
                <tr>
                    <td class="searchOption" valign="top">
                        <b>Search On:</b><br />
                        <table border="0" width="100%" cellpadding="2" cellspacing="1">
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Show</td>
                                <td>
                                    <telerik:RadComboBox ID="cboSearchShowId" MarkFirstMatch="true" AllowCustomText="false" DataTextField="ShowName" DataValueField="ShowId" Width="325" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Company Name</td>
                                <td>
                                    <asp:TextBox ID="txtSearchExhibitorCompanyName" CssClass="inputText" Width="200" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Invoice # (ExhibitorId)</td>
                                <td><asp:TextBox ID="txtSearchExhibitorId" CssClass="inputText" Width="100" runat="server" /></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Order #</td>
                                <td><asp:TextBox ID="txtSearchOrderNumber" CssClass="inputText" Width="100" runat="server" /></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Booth #</td>
                                <td><asp:TextBox ID="txtSearchBoothNumber" CssClass="inputText" Width="100" runat="server" /></td>
                            </tr>
                             <tr>
                                <td style="text-align: right; white-space:nowrap;">Contact Name</td>
                                <td><asp:TextBox ID="txtSearchContactName" CssClass="inputText" Width="100" runat="server" /></td>
                            </tr>
                             <tr>
                                <td style="text-align: right; white-space:nowrap;">Login Name</td>
                                <td><asp:TextBox ID="txtSearchUserName" CssClass="inputText" Width="100" runat="server" /></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Email Address</td>
                                <td><asp:TextBox ID="txtSearchEmailAddress" CssClass="inputText" Width="275" runat="server" /></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Trx Id</td>
                                <td><asp:TextBox ID="txtSearchTransactionId" CssClass="inputText" Width="275" runat="server" /></td>
                            </tr>
                            <asp:PlaceHolder ID="plcExhibitorClassificationSearch" Visible="false" runat="server">
                                <tr>
                                    <td style="text-align: right; white-space:nowrap;">Classification</td>
                                    <td>
                                        <telerik:RadComboBox ID="cboExhibitorClassification" MarkFirstMatch="true" AllowCustomText="true" runat="server" />
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Last digits of Credit Card</td>
                                <td><asp:TextBox ID="txtLastDigitsCreditCard" CssClass="inputText" Width="275" runat="server" /></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Name on Credit Card</td>
                                <td><asp:TextBox ID="txtNameOnCreditCard" CssClass="inputText" Width="275" runat="server" /></td>
                            </tr>
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Credit Card on file?</td>
                                <td>
                                    <asp:RadioButtonList ID="rdoLstCreditCardOnFile" CssClass="inputText" RepeatColumns="0" RepeatDirection="Horizontal" runat="server">
                                        <asp:ListItem Value="0" Selected="True">Either</asp:ListItem>
                                        <asp:ListItem Value="1">Has None</asp:ListItem>
                                        <asp:ListItem Value="2">Has At least One</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Issued a Credit?</td>
                                <td>
                                    <asp:RadioButtonList ID="rdoLstIssuedCredit" CssClass="inputText" RepeatColumns="0" RepeatDirection="Horizontal" runat="server">
                                        <asp:ListItem Value="0" Selected="True">Either</asp:ListItem>
                                        <asp:ListItem Value="1">Yes, issued a credit or refund.</asp:ListItem>
                                        <asp:ListItem Value="2">No, never issued a credit or refund.</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                             <tr>
                                <td style="text-align: right; white-space:nowrap;">Declined Credit Card?</td>
                                <td>
                                    <asp:RadioButtonList ID="rdoLstDeclinedCreditCard" CssClass="inputText" RepeatColumns="0" RepeatDirection="Horizontal" runat="server">
                                        <asp:ListItem Value="0" Selected="True">Either</asp:ListItem>
                                        <asp:ListItem Value="1">Yes, Credit Card was declined.</asp:ListItem>
                                        <asp:ListItem Value="2">No, was never declined.</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right; white-space:nowrap;">Include Inactive?</td>
                                <td><asp:checkbox id="chkSearchIncludeInactive" runat="server" /></td>
                            </tr>
                            <tr>
                                <td colspan="2" style="text-align: right;">
                                    <asp:Button ID="btnSearch" Text="Search" CssClass="button" OnClick="btnSearch_Click" runat="server" />
                                    &nbsp;
                                    <asp:Button ID="btnClearSearch" Text="Reset" CssClass="button" OnClick="btnClearSearch_Click" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>

        <asp:PlaceHolder id="plcSearchResults" Visible="false" runat="server">
            <asp:Label id="lblNoSearchResults" text="No Search Results Found" Visible="false" runat="server" />
            <asp:Repeater ID="rptrSearchList" OnItemCommand="rptrSearchList_ItemCommand" OnItemDataBound="rptrSearchList_ItemDataBound" Visible="false" runat="server">
                <HeaderTemplate>
                    <table border="0" width="100%" cellpadding="2" cellspacing="1">
                </HeaderTemplate>
                <ItemTemplate>
                        <tr id="trShowName" runat="server">
                            <td colspan="4"><h3><%# DataBinder.Eval(Container.DataItem, "ExhibitorShow.ShowName")%></h3></td>
                        </tr>
                        <tr id="trColumnHeaders" runat="server">
                            <th scope="col" style="white-space:nowrap;">Invoice # (Id)</th>
                            <th scope="col" style="white-space:nowrap;">Company Name</th>
                            <th scope="col" style="white-space:nowrap;">Contact Info</th>
                            <th scope="col" style="white-space:nowrap;">Booth #</th>
                            <th scope="col" style="white-space:nowrap;">&nbsp;</th>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td style="white-space:nowrap;"><%# DataBinder.Eval(Container.DataItem, "ExhibitorDetail.ExhibitorId")%></td>
                            <td style="width: 350px;">
                                <asp:LinkButton Visible="true" ID="lbtnGoToExhibitor" Text='<%# DataBinder.Eval(Container.DataItem, "ExhibitorDetail.ExhibitorCompanyNameDisplay")%>' CommandName="GoToExhibitor" runat="server" />
                            </td>
                            <td>
                                <asp:Literal ID="ltrContactName" runat="server" /><br />
                                <a id="lnkMailToExhibitor" class="mailTo" runat="server"></a>
                            </td>
                            <td style="white-space:nowrap;">
                                    <%# DataBinder.Eval(Container.DataItem, "ExhibitorDetail.BoothNumber")%><br />
                                    (<%# DataBinder.Eval(Container.DataItem, "ExhibitorDetail.BoothDescription")%>)
                            </td>
                            <td style="white-space: nowrap;">
                                <asp:LinkButton Visible="true" ID="lbtnCallLog" Text="Call Log" class="action-link" CommandName="ViewCallLog" runat="server" />
                                &nbsp;&nbsp;
                                <asp:LinkButton Visible="true" ID="lbtnEmailLog" Text="Email Log" class="action-link" CommandName="ViewEmailLog" runat="server" />
                            </td>
                            <td style="width: 5%;">&nbsp;</td>
                        </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </asp:PlaceHolder>

    </fieldset>


</asp:Content>
