<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CallLog.aspx.cs" Inherits="ExpoOrders.Web.Owners.CallLog" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="pageHead" runat="server">
    <title>ExpoOrders - Call Log</title>
    <link id="OwnerStyleSheet" href="Style/owner.css" rel="Stylesheet" type="text/css" />
    <link id="Link1" href="Style/CommonAdmin.css" rel="Stylesheet" type="text/css" />
</head>
<body class="pageBody">
    <form id="mainForm" runat="server">
    <telerik:RadScriptManager ID="ScriptManager1" runat="server" />

    <div id="container" style="width: 100%">
        <fieldset class="commonControls">
            <legend class="commonControls">Call Log</legend>

            <a href="#" onclick="window.close();" class="action-link" style="float: right;">[x] - Close</a>

            <h3><asp:Literal ID="ltrExhibitorCompanyName" runat="server" /></h3>

            <asp:Button ID="btnShowAddCallLog" CssClass="button" Text="Add Log" OnClick="btnShowAddCallLog_Click" runat="server" />

            <table border="0" width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblNoLogs" CssClass="friendlyMessage" Text="No Call Logs to view" Visible="false" runat="server" />

                        <asp:Repeater ID="rptrCallLog" OnItemCommand="rptrCallLog_ItemCommand" OnItemDataBound="rptrCallLog_ItemDataBound" runat="server">
                            <HeaderTemplate>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table border="0" cellpadding="1" cellspacing="2">
                                    <tr>
                                        <td class="contentLabelRight">Date:</td>
                                        <td nowrap="true"><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "LogDateTime").ToString())%>

                                            <asp:LinkButton Visible="false" ID="lbtnDeleteCallLog" Text="Delete" CommandName="DeleteCallLog" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CallLogId") %>' runat="server" />
                                        </td>
                                        <td style="width: 10%">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="contentLabelRight">User:</td>
                                        <td><asp:Literal ID="ltrOwnerPersonName" runat="server" /></td>
                                        <td style="width: 10%">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="contentLabelRight">Exhibitor:</td>
                                        <td><asp:Literal ID="ltrExhibitorPersonName" runat="server" /></td>
                                        <td style="width: 10%">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="contentLabelRight">Call Detail:</td>
                                        <td><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "CallDetails").ToString())%></td>
                                        <td style="width: 10%">&nbsp;</td>
                                    </tr>
                                </table>
                                <hr />
                            </ItemTemplate>
                            <FooterTemplate>
                
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <asp:HiddenField ID="hdnExhibitorId" runat="server" />

    <asp:Panel CssClass="outerPopup" Style="display: none;" runat="server" ID="pnlOuterAddCallLog">
        <asp:Panel Width="500px" CssClass="innerPopup" runat="server" ID="pnlAddCallLog">

        <fieldset class="commonControls">
            <legend class="commonControls">Add Call Log</legend>
             <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Name:</td>
                    <td style="text-align: left">
                        <asp:Label ID="lblCurrentUserName" CssClass="techieInfo" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Date:</td>
                    <td style="text-align: left">
                        <asp:Label ID="lblCurrentDate" CssClass="techieInfo" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Exhibitor:</td>
                    <td style="text-align: left">
                        <asp:DropDownList ID="ddlExhibitorUserId" CssClass="inputText" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Call Details:</td>
                    <td style="text-align: left">
                        <asp:TextBox ID="txtCallDetails" TextMode="MultiLine" CssClass="inputText" Columns="45" Rows="5" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Button ID="btnInsertCallLog" CssClass="button" OnClick="btnInsertCallLog_Click" Text="Add" runat="server" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnCancel" CssClass="button" Text="Cancel"  onclick="btnCancelPopup_Click" runat="server" />
                    </td>
                </tr>
            </table>
            </fieldset>
        </asp:Panel>
    </asp:Panel>

    <asp:Button ID="dummyBtn" style="visibility: hidden;" runat="server" />
    <cc1:ModalPopupExtender ID="MPE" runat="server" TargetControlID="btnShowAddCallLog" PopupControlID="pnlOuterAddCallLog" BackgroundCssClass="modalBackground"  DropShadow="true" CancelControlID="btnCancel" />
    <cc1:RoundedCornersExtender ID="RCE" runat="server" TargetControlID="pnlAddCallLog" BorderColor="black" Radius="6" />

    </form>
</body>
</html>
