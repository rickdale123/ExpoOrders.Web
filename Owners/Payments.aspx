<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnersMaster.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="Payments.aspx.cs" Inherits="ExpoOrders.Web.Owners.Payments" %>
<%@ MasterType VirtualPath="~/Owners/OwnersMaster.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

 <script type="text/javascript">
   
     $(document).ready(function () {
         var allCheckBoxSelector = '#<%=grdvwPendingPaymentList.ClientID%> input[id*="chkAllPendingPayments"]:checkbox';
         var checkBoxSelector = '#<%=grdvwPendingPaymentList.ClientID%> input[id*="chkPendingPaymentSelected"]:checkbox';

         $(allCheckBoxSelector).click(function () {
             var checkedStatus = this.checked;
             $(checkBoxSelector).each(function () {
                 $(this).prop('checked', checkedStatus);
             });
         });
     });
</script>

    <CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

    <asp:PlaceHolder ID="plcExhibitorList" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Search/Filter Exhibitor Orders</legend>

            <table class="searchTable" border="0" cellpadding="1" cellspacing="2">
                <tr>
                    <td class="searchOption" valign="top">
                        <b>Search On:</b><br />
                        <table border="0" width="100%" cellpadding="2" cellspacing="1">
                            <tr>
                                <td style="text-align: right;">Company Name</td>
                                <td><telerik:RadComboBox ID="cboSearchExhibitorId" MarkFirstMatch="true" AllowCustomText="false" Width="350" runat="server" /></td>
                            </tr>
                            <tr>
                                    <td style="text-align: right;">Invoice # (Id)</td>
                                    <td><asp:TextBox ID="txtSearchExhibitorId" CssClass="inputText" Width="100" runat="server" /></td>
                                </tr>
                            <tr>
                                <td style="text-align: right;">Booth #</td>
                                <td><asp:TextBox ID="txtSearchBoothNumber" CssClass="inputText" Width="100" runat="server" /></td>
                            </tr>
                             <tr>
                                <td style="text-align: right;">Transaction Id</td>
                                <td><asp:TextBox ID="txtSearchTrxId" CssClass="inputText" Width="200" runat="server" /></td>
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

            <br />
            <asp:Button ID="btnFindPendingCreditCardPayments" Text="Run Credit Card Sweep" OnClick="btnFindPendingCreditCardPayments_Click" CssClass="button" runat="server" />

            <a id="runSweepHelp" href="#" onclick="return false;"><img id="Img1" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
            <br />

            <asp:Label ID="lblExhibitorListRowCount" Text="" CssClass="techieInfo" runat="server" />

             <asp:GridView EnableViewState="true" GridLines="None"
                runat="server" ID="grdvwExhibitorList" AllowPaging="false" AllowSorting="true"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="2" AlternatingRowStyle-CssClass="altItem"
                RowStyle-CssClass="item" OnRowDataBound="grdvwExhibitorList_RowDataBound"  OnRowCommand="grdvwExhibitorList_RowCommand"
                EmptyDataText="No results to display.">
                <Columns>
                    <asp:TemplateField HeaderText="Inv # (Id)" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "ExhibitorId")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Company Name" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:LinkButton Visible="true" ID="lbtnLoadExhibitorPaymentDetail" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "ExhibitorCompanyName").ToString())%>' CommandName="LoadPaymentDetail"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ExhibitorId")%>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Booth #" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "BoothDisplay")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Order(s)" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "TotalOrderAmount"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Payment(s)" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            &nbsp;<%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "TotalPaymentAmount"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Balance" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            &nbsp;&nbsp;<%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "TotalBalance"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcPaymentDetail" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Payment Details</legend>

            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Button ID="btnPrintInvoice" Text="Print Invoice" Visible="false" CssClass="button" runat="server" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnSendInvoice" Text="Send Invoice" CssClass="button" Onclick="btnSendInvoice_Click" runat="server" />
                    </td>
                    <td style="white-space: nowrap;">
                        &nbsp;
                    </td>
                    <td>&nbsp;</td>
                    <td align="right" style="white-space: nowrap;">
                        <a id="lnkPrintInvoice" class="action-link" href="#" runat="server">View Invoice</a>
                        |
                        <asp:LinkButton ID="lbtnViewCallLogs" OnClick="lbtnViewCallLogs_Click" Text="Call Log" CssClass="action-link" runat="server" />
                        |
                        <asp:LinkButton ID="lbtnViewEmailLogs" OnClick="lbtnViewEmailLogs_Click" Text="Email Log" CssClass="action-link" runat="server" />
                        |
                        <asp:LinkButton ID="btnRefreshPage" CssClass="action-link" OnClick="btnRefreshPage_Click" Text="Refresh Page" runat="server" />
                    </td>
                </tr>
            </table>
            
            <table border="0" cellpadding="1" cellspacing="2" width="100%">
                <tr>
                    <td class="colData" valign="top">
                        <asp:LinkButton ID="lnkBtnExhibitorName" CssClass="action-link" OnClick="lnkBtnExhibitorName_Click" CommandName="ViewExhibitorDetail" runat="server" />

                        <asp:PlaceHolder ID="plcClassification" Visible="false" runat="server">
                            &nbsp;&nbsp;<telerik:RadComboBox ID="ddlClassification" MarkFirstMatch="true" AllowCustomText="false" Visible="false" AutoPostBack="true" OnSelectedIndexChanged="ddlClassification_Changed" runat="server" />
                        </asp:PlaceHolder>
                        
                        <br />

                        

                        <asp:Label ID="lblBoothNumberLabel" CssClass="label" runat="server">Booth #</asp:Label>&nbsp;
                        <asp:Label ID="lblBoothNumber" CssClass="label" runat="server" /><br />

                        <asp:Label ID="lblExhibitorIdLabel" CssClass="label" runat="server">Invoice #</asp:Label>&nbsp;
                        <asp:Label ID="lblExhibitorId" CssClass="label" runat="server" />
                        
                    </td>
                </tr>
                <tr>
                    <td class="sectionDivider">&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionHeading">Orders with Payment Method of Credit Card</td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView EnableViewState="true" GridLines="None"
                            runat="server" ID="grdvwCreditCardOrders" AllowPaging="false" AllowSorting="true" Width="100%"
                            AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                            RowStyle-CssClass="item" OnRowCommand="grdvwCreditCardOrders_RowCommand"
                            EmptyDataText="No Credit Card Orders to display.">
                            <Columns>
                                <asp:TemplateField HeaderText="Order #" HeaderStyle-Wrap="false">
                                    <ItemTemplate>
                                        <asp:LinkButton Visible="true" ID="lbtnLoadExhibitorPaymentDetail" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "OrderId").ToString())%>' CommandName="DisplayOrderDetail"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Date" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "OrderDate")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Status" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "OrderStatusCd")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Card Type">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "CreditCardTypeCd")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Card #">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "CreditCardNumberMasked")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Amount" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "OrderTotal"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="sectionHeadingRight">Total of Credit Card Orders = <asp:Label ID="lblTotalCreditCardOrders" CssClass="dataValueStrong" runat="server" /></td>
                </tr>
                <tr>
                    <td class="sectionDivider">&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionHeading">
                        Credit Card Payments
                        &nbsp;&nbsp;
                        <a id="addCreditCardHelp" href="#" onclick="return false;"><img id="Img3" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                    </td>
                </tr>
                <tr>
                    <td class="dataHighlight">
                        <asp:GridView EnableViewState="true" GridLines="None"
                            runat="server" ID="grdvwCreditCardPayments" AllowPaging="false" AllowSorting="true" Width="100%"
                            AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                            RowStyle-CssClass="item" OnRowDataBound="grdvwCreditCardPayments_RowDataBound" OnRowCommand="grdvwCreditCardPayments_RowCommand"
                            EmptyDataText="No Credit Card Payments to display.">
                            <Columns>
                                <asp:TemplateField HeaderText="Transaction Date" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "TransactionDate")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Id">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "OrderId")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Transaction Type">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "TransactionTypeCd")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Credit Card #" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "MaskedCardNumber")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Transaction Id">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "TransactionId")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Trx Amount" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "TransactionAmount"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Result">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "ResultDescription")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payment Amount" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "PaymentAmount"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <a id="lnkViewPaymentDetail" href="#" class="action-link" runat="server">view</a>
                                        &nbsp;|&nbsp;
                                        <asp:LinkButton ID="lnkBtnDeleteCreditCardPayment" CssClass="action-link" Text="del" CommandName="DeleteCreditCardPayment"
                                                 CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PaymentTransactionId")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="sectionHeadingRight">Total of Credit Card Payments = <asp:Label ID="lblTotalCreditCardPayments" CssClass="dataValueStrong" runat="server" /></td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnDisplayAddCreditCardPayment" Text="Add Credit Card Transaction" CssClass="button" OnClick="btnDisplayAddCreditCardPayment_Click" runat="server" />
                        &nbsp;&nbsp;
                        <asp:LinkButton ID="lbtnManageCreditCards" OnClick="lbtnManageCreditCards_Click" Text="Manage Credit Cards" runat="server" />
                        &nbsp;
                        <br />
                        
                        <asp:PlaceHolder ID="plcAddCreditCardPayment" Visible="false" runat="server">

                            <table border="0" cellpadding="2" cellspacing="2" width="100%">
                                <tr>
                                    <td class="contentLabelRight">Order #</td>
                                    <td><asp:DropDownList ID="ddlCreditCardOrderId" CssClass="inputText" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Transaction Type</td>
                                    <td>
                                        <asp:DropDownList ID="ddlAddCreditCardTransactionType" AutoPostBack="true" OnSelectedIndexChanged="ddlAddCreditCardTransactionType_Changed" CssClass="inputText" runat="server" />
                                        &nbsp;<a id="ccTrxTypeHelp" href="#" onclick="return false;"><img id="Img6" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                                    </td>
                                    <td>&nbsp;</td>
                                </tr>

                                <asp:PlaceHolder ID="plcTransactionId" Visible="false" runat="server">
                                    <tr>
                                        <td class="contentLabelRight">Transaction Id</td>
                                        <td>
                                            <asp:TextBox ID="txtCreditCardTransactionId" CssClass="inputText" Width="200" runat="server" />
                                            &nbsp;
                                            <a id="transactionIDHelp" href="#" onclick="return false;"><img id="Img2" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plcCreditCardDetails" Visible="false" runat="server">
                                    <tr>
                                        <td class="contentLabelRight">Credit Card</td>
                                        <td><asp:DropDownList ID="ddlCreditCardId" CssClass="inputText" AutoPostBack="true" OnSelectedIndexChanged="ddlCreditCardId_Changed" runat="server" /></td>
                                        <td>&nbsp;</td>
                                    </tr>
                                    <asp:PlaceHolder ID="plcNewCreditCard" Visible="false" runat="server">
                                        <tr>
                                            <td colspan="3" class="contentLabel">
                                                <h3>Credit Card Information</h3>
                                                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                                                    <tr>
                                                        <td class="contentLabelRight">Name on Card</td>
                                                        <td>
                                                            <asp:TextBox ID="txtCreditCardName" Width="200" CssClass="inputText" MaxLength="50" runat="server" />

                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="errorMessage" EnableClientScript="false" ErrorMessage="Name is Required" runat="server" ControlToValidate="txtCreditCardName" ValidationGroup="CreditCardInfo">Missing Name</asp:RequiredFieldValidator>
                                                        </td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                     <tr>
                                                        <td class="contentLabelRight">Card Type</td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlCreditCardType" CssClass="inputText" runat="server" />
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="errorMessage" ErrorMessage="Card Type is Required" EnableClientScript="false" runat="server" ControlToValidate="ddlCreditCardType" ValidationGroup="CreditCardInfo">Missing Card Type</asp:RequiredFieldValidator>
                                                        </td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                     <tr>
                                                        <td class="contentLabelRight">Card Number</td>
                                                        <td>
                                                            <input type="hidden" id="hdnCreditCardId" runat="server" />
                                                            <input type="hidden" id="hdnCreditCardAddressId" runat="server" />
                                                            <asp:TextBox ID="txtCreditCardNumber" Width="200" CssClass="inputText" MaxLength="50" runat="server" />

                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" CssClass="errorMessage" ErrorMessage="Card Type is Required"  EnableClientScript="false" runat="server" ControlToValidate="txtCreditCardNumber" ValidationGroup="CreditCardInfo">Missing Card Type</asp:RequiredFieldValidator>
                                                        </td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                     <tr>
                                                        <td class="contentLabelRight">Expiration Date</td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlCreditCardExpMonth" CssClass="inputText" runat="server" />&nbsp;/&nbsp;<asp:DropDownList ID="ddlCreditCardExpYear" CssClass="inputText" runat="server" />
                        
                                                            <asp:CustomValidator ID="customCreditCardExpDateValidator" CssClass="errorMessage" ValidationGroup="CreditCardInfo" EnableClientScript="false" runat="server"></asp:CustomValidator>
                                                            &nbsp;<asp:Label ID="Label7" CssClass="contentLabel" runat="server">Security Code</asp:Label>
                                                            <asp:TextBox ID="txtCreditCardSecurityCode" Width="60" CssClass="inputText" MaxLength="10" runat="server" /> 

                                                        </td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="contentLabelRight">Address</td>
                                                        <td><asp:TextBox ID="txtCreditCardAddressLine1" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="contentLabelRight">&nbsp;</td>
                                                        <td><asp:TextBox ID="txtCreditCardAddressLine2" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="contentLabelRight">City</td>
                                                        <td><asp:TextBox ID="txtCreditCardCity" Width="125" CssClass="inputText" MaxLength="50" runat="server" /></td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="contentLabelRight">State/Province/Region</td>
                                                        <td><asp:TextBox ID="txtCreditCardState" Width="150" CssClass="inputText" MaxLength="20" runat="server" /></td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="contentLabelRight">Postal Code</td>
                                                        <td><asp:TextBox ID="txtCreditCardPostalCode" Width="50" CssClass="inputText" MaxLength="20" runat="server" /></td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="contentLabelRight">Country</td>
                                                        <td><asp:TextBox ID="txtCreditCardCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" /></td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="contentLabelRight">Email Address</td>
                                                        <td><asp:TextBox ID="txtCreditCardEmail" Width="200" CssClass="inputText" MaxLength="150" runat="server" /></td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="contentLabelRight">Save this Credit Card on file?</td>
                                                        <td><asp:CheckBox ID="chkSaveCreditCard" Text="" runat="server" /></td>
                                                        <td style="width:10%">&nbsp;</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </asp:PlaceHolder>
                                </asp:PlaceHolder>
                                <tr>
                                    <td class="contentLabelRight">Amount <asp:Literal ID="ltrCreditCardCurrencySymbol" runat="server" /></td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtAddCreditCardAmount" Width="75" CssClass="inputText" runat="server" />
                                        &nbsp;
                                        <asp:Label ID="lblCreditCardAmountHelpInfo" CssClass="techieInfo" runat="server" />
                                        <br />
                                        <asp:Button ID="btnAddCreditCardPayment" Text="Process Payment" CssClass="button" OnClick="btnAddCreditCardPayment_Click" runat="server" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="btnCancelAddCreditCardPayment" Text="Cancel" CssClass="button" OnClick="btnCancelAddCreditCardPayment_Click" runat="server" />
                                    </td>
                                </tr>
                                <asp:PlaceHolder ID="plcNewOrderItem" Visible="false" runat="server">
                                <tr>
                                    <td class="contentLabelRight">Add a line item to the Invoice?</td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtAddOrderLineItemDescription" CssClass="inputText" TextMode="MultiLine" Columns="50" Rows="3" runat="server" />
                                    </td>
                                </tr>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcNewExpirationDate" Visible="false" runat="server">
                                <tr>
                                    <td class="contentLabelRight">New Credit Card Expiration Date (optional):</td>
                                    <td colspan="2" style="white-space: nowrap">
                                        <asp:TextBox ID="txtNewExpirationDate" CssClass="inputText" Width="75" runat="server" /> <span class="techieInfo">MMYYYY or MM/YYYY</span>
                                    </td>
                                </tr>
                                </asp:PlaceHolder>

                            </table>
                        </asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <td class="sectionDivider">&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionHeading">Orders with Payment Method of Check</td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView EnableViewState="true" GridLines="None"
                            runat="server" ID="grdvwCheckOrders" AllowPaging="false" AllowSorting="true" Width="100%"
                            AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                            RowStyle-CssClass="item" OnRowCommand="grdvwCheckOrders_RowCommand"
                            EmptyDataText="No Check Orders to display.">
                            <Columns>
                                <asp:TemplateField HeaderText="Order #" HeaderStyle-Wrap="false">
                                    <ItemTemplate>
                                        <asp:LinkButton Visible="true" ID="lbtnLoadExhibitorPaymentDetail" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "OrderId").ToString())%>' CommandName="DisplayOrderDetail"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Date" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "OrderDate")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Order Status" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "OrderStatusCd")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Billing Address">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "ShortBillingAddress")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Amount" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "OrderTotal"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="sectionHeadingRight">Total of Check Orders = <asp:Label ID="lblTotalCheckOrders" CssClass="dataValueStrong" runat="server" /></td>
                </tr>
                <tr>
                    <td class="sectionDivider">&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionHeading">
                        Check Payments
                        &nbsp;
                        <a id="addCheckHelp" href="#" onclick="return false;"><img id="Img4" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                    </td>
                </tr>
                <tr>
                    <td class="dataHighlight">
                         <asp:GridView EnableViewState="true" GridLines="None"
                            runat="server" ID="grdvwCheckPayments" AllowPaging="false" AllowSorting="true" Width="100%"
                            AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                            OnRowDataBound="grdvwCheckPayments_RowDataBound"  OnRowCommand="grdvwCheckPayments_RowCommand"
                            RowStyle-CssClass="item" 
                            EmptyDataText="No Check Payments to display.">
                            <Columns>
                                <asp:TemplateField HeaderText="Order #" HeaderStyle-Wrap="false">
                                    <ItemTemplate>
                                        <asp:LinkButton Visible="true" ID="lbtnLoadExhibitorPaymentDetail" Text='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' CommandName="DisplayOrderDetail"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date Received" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# String.Format("{0:d}", DataBinder.Eval(Container.DataItem, "CheckReceivedDate"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Check #">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "CheckNumber")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payor">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "CheckPayor")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payment Amount" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "PaymentAmount"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <a id="lnkViewPaymentDetail" href="#" class="action-link" runat="server">view</a>
                                        &nbsp;|&nbsp;
                                        <asp:LinkButton ID="lnkBtnDeleteCheckPayment" CssClass="action-link" Text="del" CommandName="DeleteCheckPayment"
                                                 CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PaymentTransactionId")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="sectionHeadingRight">Total of Check Payments = <asp:Label ID="lblTotalCheckPayments" CssClass="dataValueStrong" runat="server" /></td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnDisplayAddCheckPayment" Text="Add Check Payment" CssClass="button" OnClick="btnDisplayAddCheckPayment_Click" runat="server" />
                        <asp:PlaceHolder ID="plcAddCheckPayment" Visible="false" runat="server">
                            <table border="0" cellpadding="2" cellspacing="2" width="100%">
                                <tr>
                                    <td class="contentLabelRight">Order #</td>
                                    <td><asp:DropDownList ID="ddlCheckOrderId" CssClass="inputText" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Date Received</td>
                                    <td><asp:TextBox ID="txtCheckReceiveDate" CssClass="inputText" Width="100" maxlength="100" runat="server" /><cc1:CalendarExtender ID="calExtender" TargetControlID="txtCheckReceiveDate" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Payor</td>
                                    <td><asp:TextBox ID="txtCheckPayor" CssClass="inputText" Width="150" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Check #</td>
                                    <td><asp:TextBox ID="txtCheckNumber" CssClass="inputText" Width="60" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Amount <asp:Literal ID="ltrCheckCurrencySymbol" runat="server" /></td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtAddCheckAmount" Width="75" CssClass="inputText" runat="server" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="btnAddCheckPayment" Text="Record Check Payment" CssClass="button" OnClick="btnAddCheckPayment_Click" runat="server" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="btnCancelAddCheckPayment" Text="Cancel" CssClass="button" OnClick="btnCancelAddCheckPayment_Click" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </asp:PlaceHolder>    
                    </td>
                </tr>
                <tr>
                    <td class="sectionDivider">&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionHeading">Orders with Payment Method of Wire Transfer</td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView EnableViewState="true" GridLines="None"
                            runat="server" ID="grdvwWireOrders" AllowPaging="false" AllowSorting="true" Width="100%"
                            AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                            RowStyle-CssClass="item" OnRowCommand="grdvwWireOrders_RowCommand"
                            EmptyDataText="No Wire Orders to display.">
                            <Columns>
                                <asp:TemplateField HeaderText="Order #" HeaderStyle-Wrap="false">
                                    <ItemTemplate>
                                        <asp:LinkButton Visible="true" ID="lbtnLoadExhibitorPaymentDetail" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "OrderId").ToString())%>' CommandName="DisplayOrderDetail"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Date" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "OrderDate")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Order Status" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "OrderStatusCd")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Billing Address">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "ShortBillingAddress")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Amount" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "OrderTotal"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="sectionHeadingRight">Total of Wire Transfer Orders = <asp:Label ID="lblTotalWireOrders" CssClass="dataValueStrong" runat="server" /></td>
                </tr>
                <tr>
                    <td class="sectionDivider">&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionHeading">
                        Wire Transfer Payments
                        &nbsp;
                        <a id="addWireHelp" href="#" onclick="return false;"><img id="Img5" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                    </td>
                </tr>
                <tr>
                    <td class="dataHighlight">
                         <asp:GridView EnableViewState="true" GridLines="None"
                            runat="server" ID="grdvwWirePayments" AllowPaging="false" AllowSorting="true" Width="100%"
                            AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                            OnRowDataBound="grdvwWirePayments_RowDataBound"  OnRowCommand="grdvwWirePayments_RowCommand"
                            RowStyle-CssClass="item" 
                            EmptyDataText="No Wire Payments to display.">
                            <Columns>
                                <asp:TemplateField HeaderText="Order #" HeaderStyle-Wrap="false">
                                    <ItemTemplate>
                                        <asp:LinkButton Visible="true" ID="lbtnLoadExhibitorPaymentDetail" Text='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' CommandName="DisplayOrderDetail"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date Received" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# String.Format("{0:d}", DataBinder.Eval(Container.DataItem, "WireReceivedDate"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Wire Details">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "WireDetails")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payment Amount" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "PaymentAmount"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <a id="lnkViewPaymentDetail" href="#" class="action-link" runat="server">view</a>
                                        &nbsp;|&nbsp;
                                        <asp:LinkButton ID="lnkBtnDeleteWirePayment" CssClass="action-link" Text="del" CommandName="DeleteWirePayment"
                                                 CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PaymentTransactionId")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="sectionHeadingRight">Total of Wire Payments = <asp:Label ID="lblTotalWirePayments" CssClass="dataValueStrong" runat="server" /></td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnDisplayAddWirePayment" Text="Add Wire Payment" CssClass="button" OnClick="btnDisplayAddWirePayment_Click" runat="server" />
                        <asp:PlaceHolder ID="plcAddWirePayment" Visible="false" runat="server">
                            <table border="0" cellpadding="2" cellspacing="2" width="100%">
                                <tr>
                                    <td class="contentLabelRight">Order #</td>
                                    <td><asp:DropDownList ID="ddlWireOrderId" CssClass="inputText" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Date Received</td>
                                    <td><asp:TextBox ID="txtWireReceiveDate" CssClass="inputText" Width="100" maxlength="100" runat="server" /><cc1:CalendarExtender ID="CalendarExtender1" TargetControlID="txtWireReceiveDate" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Wire Details</td>
                                    <td><asp:TextBox ID="txtWireDetails" CssClass="inputText" TextMode="MultiLine" Columns="50" Rows="5" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Amount <asp:Literal ID="ltrWireCurrencySymbol" runat="server" /></td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtAddWireAmount" Width="75" CssClass="inputText" runat="server" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="btnAddWirePayment" Text="Record Wire Payment" CssClass="button" OnClick="btnAddWirePayment_Click" runat="server" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="btnCancelAddWirePayment" Text="Cancel" CssClass="button" OnClick="btnCancelAddWirePayment_Click" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </asp:PlaceHolder>    
                    </td>
                </tr>
                 <tr>
                    <td class="sectionDivider">&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionHeading">Orders with Payment Method of Exhibitor Funds Transfer</td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView EnableViewState="true" GridLines="None"
                            runat="server" ID="grdvwExhibitorFundsTransferOrders" AllowPaging="false" AllowSorting="true" Width="100%"
                            AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                            RowStyle-CssClass="item" OnRowCommand="grdvwExhibitorFundsTransferOrders_RowCommand"
                            EmptyDataText="No Exhibitor Funds Transfer Orders to display.">
                            <Columns>
                                <asp:TemplateField HeaderText="Order #" HeaderStyle-Wrap="false">
                                    <ItemTemplate>
                                        <asp:LinkButton Visible="true" ID="lbtnLoadExhibitorPaymentDetail" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "OrderId").ToString())%>' CommandName="DisplayOrderDetail"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Date" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "OrderDate")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Order Status" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "OrderStatusCd")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Billing Address">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "ShortBillingAddress")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Amount" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "OrderTotal"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="sectionHeadingRight">Total of Exhibitor Funds Transfer Orders = <asp:Label ID="lblTotalExhibitorFundsTransferOrders" CssClass="dataValueStrong" runat="server" /></td>
                </tr>
                <tr>
                    <td class="sectionDivider">&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionHeading">
                        Exhibitor Funds Transfer Payments
                        &nbsp;
                        <a id="addExhibitorFundsTransferHelp" href="#" onclick="return false;"><img id="Img7" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                    </td>
                </tr>
                <tr>
                    <td class="dataHighlight">
                         <asp:GridView EnableViewState="true" GridLines="None"
                            runat="server" ID="grdvwExhibitorFundsTransferPayments" AllowPaging="false" AllowSorting="true" Width="100%"
                            AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                            OnRowDataBound="grdvwExhibitorFundsTransferPayments_RowDataBound"  OnRowCommand="grdvwExhibitorFundsTransferPayments_RowCommand"
                            RowStyle-CssClass="item" 
                            EmptyDataText="No Exhibitor Funds Transfer Payments to display.">
                            <Columns>
                                <asp:TemplateField HeaderText="Order #" HeaderStyle-Wrap="false">
                                    <ItemTemplate>
                                        <asp:LinkButton Visible="true" ID="lbtnLoadExhibitorPaymentDetail" Text='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' CommandName="DisplayOrderDetail"
                                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date Received" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                       <%-- <%# String.Format("{0:d}", DataBinder.Eval(Container.DataItem, "EEFTReceivedDate"))%>--%>
                                        <%#  DataBinder.Eval(Container.DataItem, "EEFTReceivedDate")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Exhibitor Funds Transfer Details">
                                    <ItemTemplate>
                                        <%# DataBinder.Eval(Container.DataItem, "EEFTDetails")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payment Amount" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "PaymentAmount"))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <a id="lnkViewPaymentDetail" href="#" class="action-link" runat="server">view</a>
                                        &nbsp;|&nbsp;
                                        <asp:LinkButton ID="lnkBtnDeleteExhibitorFundsTransferPayment" CssClass="action-link" Text="del" CommandName="DeleteExhibitorFundsTransferPayment"
                                                 CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PaymentTransactionId")%>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="sectionHeadingRight">Total of Exhibitor Funds Transfer Payments = <asp:Label ID="lblTotalExhibitorFundsTransferPayments" CssClass="dataValueStrong" runat="server" /></td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnDisplayAddExhibitorFundsTransferPayment" Text="Add Exhibitor Funds Transfer Payment" CssClass="button" OnClick="btnDisplayAddExhibitorFundsTransferPayment_Click" runat="server" />
                        <asp:PlaceHolder ID="plcAddExhibitorFundsTransferPayment" Visible="false" runat="server">
                            <table border="0" cellpadding="2" cellspacing="2" width="100%">
                                <tr>
                                    <td class="contentLabelRight">Order #</td>
                                    <td><asp:DropDownList ID="ddlExhibitorFundsTransferOrderId" CssClass="inputText" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Date Received</td>
                                    <td><asp:TextBox ID="txtExhibitorFundsTransferReceivedDate" CssClass="inputText" Enabled="false" Width="100" maxlength="100" runat="server" /><cc1:CalendarExtender ID="CalendarExtender2" TargetControlID="txtWireReceiveDate" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Funds Transfer Details</td>
                                    <td><asp:TextBox ID="txtExhibitorFundsTransferDetails" CssClass="inputText" TextMode="MultiLine" Columns="50" Rows="5" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Amount <asp:Literal ID="ltrExhibitorFundsTransferCurrencySymbol" runat="server" /></td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtAddExhibitorFundsTransferAmount" Width="75" CssClass="inputText" runat="server" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="btnAddExhibitorFundsTransferPayment" Text="Record Exhibitor Fund Transfer Payment" CssClass="button" OnClick="btnAddExhibitorFundsTransferPayment_Click" runat="server" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="btnCancelAddExhibitorFundsTransferPayment" Text="Cancel" CssClass="button" OnClick="btnCancelAddExhibitorFundsTransferPayment_Click" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </asp:PlaceHolder>    
                    </td>
                </tr>
                <tr>
                    <td class="sectionHeadingRight">Total Balance = <asp:Label ID="lblGrandTotalBalanceAmount" CssClass="dataValueStrong" runat="server" /></td>
                </tr>
            </table>
            
            <br />
            <center>
                <asp:Button ID="btnBackToList" Text="Back To List" CssClass="button" OnClick="btnBackToList_Click" runat="server" />
                &nbsp;
                &nbsp;
                <asp:Button ID="btnRefreshLists" Text="Refresh All" CssClass="button" OnClick="btnRefreshLists_Click" runat="server" />
                &nbsp;
                &nbsp;
                <asp:CheckBox ID="chkShowAllInactive" Text="Show all inactive" CssClass="inputText" runat="server" />
            </center>

        </fieldset>

        <asp:HiddenField ID="hdnExhibitorId" runat="server" />
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcPendingPayments" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Pending Credit Card Payments</legend>
            <asp:Label ID="lblPendingCreditCardPaymentRowCount" Text="" CssClass="techieInfo" runat="server" />

             <asp:GridView EnableViewState="true" GridLines="None"
                runat="server" ID="grdvwPendingPaymentList" AllowPaging="false" AllowSorting="true"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                RowStyle-CssClass="item" OnRowDataBound="grdvwPendingPaymentList_RowDataBound"
                OnRowCommand="grdvwPendingPaymentList_RowCommand"
                EmptyDataText="No pending payments to display.">
                <Columns>
                    <asp:TemplateField HeaderText="">
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkAllPendingPayments" Checked="true" runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkPendingPaymentSelected" Checked="true" runat="server" />
                            <asp:HiddenField ID="hdnOrderId" Value='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Order #" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                        <asp:LinkButton Visible="true" ID="lbtnLoadPaymentDetail" Text='<%# DataBinder.Eval(Container.DataItem, "OrderId").ToString()%>' CommandName="LoadPaymentDetail"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ExhibitorId").ToString() + ":" + DataBinder.Eval(Container.DataItem, "OrderId").ToString()%>' runat="server" />
                            </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Company Name" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "ExhibitorCompanyName")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Credit Card" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "MaskedCardNumber")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Order Date" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "OrderDate")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Order Total" ItemStyle-HorizontalAlign="Right" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "OrderTotal"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Total Payments" ItemStyle-HorizontalAlign="Right" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "PaymentTotal"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Balance" ItemStyle-HorizontalAlign="Right" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "OrderBalance"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                </Columns>
            </asp:GridView>

            <br />

            <asp:Button ID="btnSweepOrders" Text="Process Selected Orders" CssClass="button" OnClick="btnSweepOrders_Click" runat="server" />

        </fieldset>
       
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcCreditCardSweepResults" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Credit Card Sweep Results</legend>

            <asp:Label ID="lblCreditCardSweepRowCount" Text="" CssClass="techieInfo" runat="server" />

                <asp:GridView EnableViewState="true" OnRowDataBound="grdvSweepResults_RowDataBound" OnRowCommand="grdvSweepResults_RowCommand"
                    runat="server" ID="grdvSweepResults" AllowPaging="false" AllowSorting="true"
                    AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                    RowStyle-CssClass="item" EmptyDataText="No data processed." GridLines="None">
                    <Columns>
                       
                        <asp:TemplateField HeaderText="Exhibitor">
                            <ItemTemplate>
                            <asp:LinkButton Visible="true" ID="lbtnLoadPaymentDetail" Text='<%# DataBinder.Eval(Container.DataItem, "ExhibitorCompanyName")%>' CommandName="LoadPaymentDetail"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ExhibitorId")%>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Order #">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "OrderId")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sweep Results">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "SweepResults")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

        </fieldset>
    </asp:PlaceHolder>

    <telerik:RadToolTip runat="server" ID="runSweepHelpTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="runSweepHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(runSweepHelpTT, "RunSweepNote")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="transactionIdTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="transactionIDHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(transactionIdTT, "CreditCardTrxId")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="addCreditCardTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="addCreditCardHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(addCreditCardTT, "CreditCardPaymentHelp")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="addCheckTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="addCheckHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(addCheckTT, "CheckPaymentHelp")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="addWireTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="addWireHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(addWireTT, "WirePaymentHelp")%>
    </telerik:RadToolTip>
    <telerik:RadToolTip runat="server" ID="ccTrxTypeTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="ccTrxTypeHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(ccTrxTypeTT, "CreditCardTrxTypeHelp")%>
    </telerik:RadToolTip>
    <telerik:RadToolTip runat="server" ID="addEEFTransferTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="addExhibitorFundsTransferHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(addEEFTransferTT, "AddExhibitorFundsTransferHelp")%>
    </telerik:RadToolTip>



</asp:Content>
