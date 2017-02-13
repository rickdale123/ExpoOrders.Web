<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentDetail.aspx.cs" Inherits="ExpoOrders.Web.Owners.PaymentDetail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="pageHead" runat="server">
    <title>ExpoOrders - Payment Detail</title>
    <link id="OwnerStyleSheet" href="Styles/style.css" rel="Stylesheet" type="text/css" />
</head>
<body class="pageBody">
    <form id="mainForm" runat="server">
    <telerik:RadScriptManager ID="ScriptManager1" runat="server" />
        <div id="container">
        <CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

        <asp:PlaceHolder ID="plcFriendlyMessage" Visible="false" runat="server">
            <table class="friendlyMessage" border="0" width="100%" cellpadding="3" cellspacing="0">
                <tr>
                    <td class="friendlyMessage">
                        <asp:Literal ID="ltrFriendlyMessage" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>

            <table width="100%" border="0" cellpadding="2" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Payment TrxId</td>
                    <td>
                        <asp:Label ID="lblPaymentTransactionId" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">User</td>
                    <td>
                        <asp:Label ID="txtUserName" CssClass="colData" runat="server" />
                        &nbsp;
                        <asp:Label ID="lblUserId" CssClass="techieInfo" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Order #</td>
                    <td>
                        <asp:DropDownList ID="ddlOrderId" CssClass="inputText" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Payment Date</td>
                    <td>
                        <asp:TextBox ID="txtTransactionDate" CssClass="inputText" Width="100" maxlength="100" runat="server" /><cc1:CalendarExtender ID="calExtender" TargetControlID="txtTransactionDate" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Trx Amount</td>
                    <td>
                        <asp:TextBox ID="txtTransactionAmount" Width="75" CssClass="inputText" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Payment Amount</td>
                    <td>
                        <asp:TextBox ID="txtPaymentAmount" Width="75" CssClass="inputText" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                
                 <tr>
                    <td colspan="2" style="text-align: left;">
                        <asp:Button ID="btnSavePaymentDetail" Text="Update" OnClick="btnSavePaymentDetail_Click" CssClass="button" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>

                <tr>
                    <td colspan="3"><hr /></td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Gateway Request</td>
                    <td>
                        <asp:TextBox ID="txtRequestParams" CssClass="inputText" TextMode="MultiLine" Columns="65" Rows="8" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Gateway Response</td>
                    <td>
                        <asp:TextBox ID="txtResponseParams" CssClass="inputText" TextMode="MultiLine" Columns="65" Rows="8" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr id="trNotes" runat="server" visible="true">
                    <td class="contentLabelRight">Notes</td>
                    <td>
                        <asp:Label ID="lblPaymentTransactionNotes" CssClass="techieInfo" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                
            </table>
        </div>
    </form>
</body>
</html>