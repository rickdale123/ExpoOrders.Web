<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnersMaster.Master" ValidateRequest="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="ExpoOrders.Web.Owners.Orders" %>
<%@ MasterType VirtualPath="~/Owners/OwnersMaster.Master" %>
<%@ Register Src="~/CustomControls/OrderDetail.ascx" TagPrefix="uc" TagName="OrderDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   <script type="text/javascript">
       
       $(document).ready(function () {
           var allCheckBoxSelector = '#<%=grdvwExhibitorOrderList.ClientID%> input[id*="chkAll"]:checkbox';
           var checkBoxSelector = '#<%=grdvwExhibitorOrderList.ClientID%> input[id*="chkAcceptOrder"]:checkbox';

           $(allCheckBoxSelector).click(function () {
               var checkedStatus = this.checked;
               $(checkBoxSelector).each(function () {
                   $(this).prop('checked', checkedStatus);
               });
           });
       });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">
<CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

    <asp:PlaceHolder ID="plcOrderList" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Search/Filter Exhibitor Orders</legend>

            <table class="searchTable" border="0" cellpadding="1" cellspacing="2">
                <tr>
                    <td class="searchOption" valign="top" style="white-space: nowrap;">
                        <b>Booth Orders</b>
                        <ul>
                            <li><asp:LinkButton ID="lnkBtnBoothOrderSubmitted" Text="Submitted" OnClick="lnkBtnOrderFilter_Click" runat="server" /></li>
                            <li><asp:LinkButton ID="lnkBtnBoothOrderAccepted" Text="Accepted" OnClick="lnkBtnOrderFilter_Click" runat="server" /></li>
                            <li><asp:LinkButton ID="lnkBtnBoothOrderRejected" Text="Rejected" OnClick="lnkBtnOrderFilter_Click" runat="server" /></li>
                            <li><asp:LinkButton ID="lnkBtnBoothOrderDeleted" Text="Deleted" OnClick="lnkBtnOrderFilter_Click" runat="server" /></li>
                        </ul>
                    </td>
                    <td style="width:10px;">&nbsp;</td>
                    <td class="searchOption" valign="top" style="white-space: nowrap;">
                        <b>Form Submissions</b>
                        <ul>
                            <li><asp:LinkButton ID="lnkBtnFormOrderSubmitted" Text="Submitted" OnClick="lnkBtnOrderFilter_Click" runat="server" /></li>
                            <li><asp:LinkButton ID="lnkBtnFormOrderAccepted" Text="Accepted" OnClick="lnkBtnOrderFilter_Click" runat="server" /></li>
                            <li><asp:LinkButton ID="lnkBtnFormOrderRejected" Text="Rejected" OnClick="lnkBtnOrderFilter_Click" runat="server" /></li>
                            <li><asp:LinkButton ID="lnkBtnFormOrderDeleted" Text="Deleted" OnClick="lnkBtnOrderFilter_Click" runat="server" /></li>
                        </ul>
                    </td>
                    <td style="width:10px;">&nbsp;</td>
                    <td class="searchOption" valign="top">
                        <b>Search On:</b><br />
                        <table border="0" width="100%" cellpadding="2" cellspacing="0">
                            <tr>
                                <td style="text-align: right;">Company</td>
                                <td><telerik:RadComboBox ID="cboSearchExhibitorId" MarkFirstMatch="true" AllowCustomText="false" Width="300" runat="server" /></td>
                            </tr>
                             <tr>
                                    <td style="text-align: right; white-space: nowrap;">Inv# (Exb. Id)</td>
                                    <td><asp:TextBox ID="txtSearchExhibitorId" CssClass="inputText" Width="100" runat="server" /></td>
                                </tr>
                            <tr>
                                <td style="text-align: right;">Booth #</td>
                                <td><asp:TextBox ID="txtSearchBoothNumber" CssClass="inputText" Width="100" runat="server" /></td>
                            </tr>
                            <tr>
                                <td style="text-align: right;">Order #</td>
                                <td><asp:TextBox ID="txtSearchOrderNumber" CssClass="inputText" Width="100" runat="server" /></td>
                            </tr>
                            <tr>
                                <td colspan="2" style="text-align: right;">
                                    <asp:Button ID="btnSearchOrders" Text="Search" CssClass="button" OnClick="btnSearchOrders_Click" runat="server" />
                                    &nbsp;
                                    <asp:Button ID="btnClearSearch" Text="Reset" CssClass="button" OnClick="btnClearSearch_Click" runat="server" />
                                    &nbsp;
                                    <asp:Button ID="btnCreateNewOrder" Text="Create Order" CssClass="button" OnClick="btnDisplayCreateOrder_Click" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
           
            <br />

            <asp:Label ID="lblExhibitorOrderListRowCount" Text="" CssClass="techieInfo" runat="server" />

            <asp:Button ID="btnAcceptSelectedOrders" Visible="false" Text="Accept All Selected Orders" CssClass="button" OnClick="btnAcceptSelectedOrders_Click" runat="server" />

            <asp:GridView EnableViewState="true" GridLines="None"
                runat="server" ID="grdvwExhibitorOrderList" AllowPaging="false" AllowSorting="true" Width="90%"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                RowStyle-CssClass="item" OnRowDataBound="grdvwExhibitorOrderList_RowDataBound" OnRowCommand="grdvwExhibitorOrderList_RowCommand"
                EmptyDataText="No exhibitor orders to display.">
                <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:CheckBox ID="chkAll" runat="server" />
                    </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkAcceptOrder" runat="server" />
                            <asp:HiddenField ID="hdnRowOrderId" Value='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' runat="server" />
                         </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Order #" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                        <asp:LinkButton Visible="true" ID="lbtnLoadOrderDetail" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "OrderId").ToString())%>' CommandName="EditOrder"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>' runat="server" />
                         </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="Company Name" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "Exhibitor.ExhibitorCompanyName")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Order Date" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "OrderDate")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="Amount" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <%# String.Format("{0:#,##0.00}", DataBinder.Eval(Container.DataItem, "OrderTotal"))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Order Type" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "OrderTypeCd")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Pay Type" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "PaymentTypeCd")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Order Status" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "OrderStatusCd")%>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>

            <asp:Button ID="btnAcceptSelectedOrders2" Visible="false" Text="Accept All Selected Orders" CssClass="button" OnClick="btnAcceptSelectedOrders_Click" runat="server" />

        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcOrderDetail" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Order Detail</legend>

            <table border="0" width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td colspan="2">
                        <asp:LinkButton ID="lnkBtnExhibitorName" CssClass="action-link" OnClick="lnkBtnExhibitorName_Click" CommandName="ViewExhibitorDetail" runat="server" />

                        <asp:PlaceHolder ID="plcClassification" Visible="false" runat="server">
                            &nbsp;&nbsp;<telerik:RadComboBox ID="ddlClassification" MarkFirstMatch="true" AllowCustomText="false" Visible="false" AutoPostBack="true" OnSelectedIndexChanged="ddlClassification_Changed" runat="server" />
                        </asp:PlaceHolder>

                    </td>
                    <td style="text-align:right; white-space: nowrap;">
                        <a id="lnkPrintInvoice" class="action-link" href="#" style="text-align: right;" runat="server">View Invoice</a>
                        |
                        <asp:LinkButton ID="lbtnViewCallLogs" OnClick="lbtnViewCallLogs_Click" Text="Call Log" class="action-link" runat="server" />
                        |
                        <a id="lnkViewOrderHistory2" href="#" class="action-link" runat="server">Order History</a>
                        |
                        <telerik:RadComboBox ID="cboJumpToOrderId" MarkFirstMatch="true" AllowCustomText="false" Width="110px" Visible="false" AutoPostBack="true" OnSelectedIndexChanged="cboJumpToOrderId_Changed" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3">&nbsp;</td>
                </tr>
                <tr>
                    <td style="vertical-align: top">
                        <table border="0" width="100%" cellpadding="1" cellspacing="0">
                            <tr>
                                <td class="contentLabelRight">Order #:</td>
                                <td class="dataValueStrong"><asp:Literal ID="ltrOrderId" runat="server" /></td>
                                <td width="325">&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="contentLabelRight">Order Status:</td>
                                <td><asp:DropDownList ID="ddlOrderStatus" CssClass="inputText" runat="server" /></td>
                                <td width="325">&nbsp;</td>
                            </tr>
                            <asp:PlaceHolder ID="plcOrderUserId" Visible="false" runat="server">
                                <tr>
                                    <td class="contentLabelRight">Submitted by (Invoiced To):</td>
                                    <td>
                                        <asp:DropDownList ID="ddlOrderUserId" CssClass="inputText" runat="server" />
                                    </td>
                                    <td width="325">&nbsp;</td>
                                </tr>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plcOrderPaymentType" Visible="false" runat="server">
                                <tr>
                                    <td class="contentLabelRight">Payment Type:</td>
                                    <td>
                                        <asp:DropDownList ID="ddlPaymentType" CssClass="inputText" runat="server" />
                                    </td>
                                    <td width="325">&nbsp;</td>
                                </tr>
                            </asp:PlaceHolder>
                        </table>

                        <asp:PlaceHolder ID="plcApplyCreditCard" visible="false" runat="server">
                            <fieldset class="commonControls">
                                <legend class="commonControls"><asp:Literal ID="ltrApplyCreditCardLabel" Text="Apply Credit Card" runat="server" /></legend>
                                    <table border="0" width="100%" cellpadding="1" cellspacing="0">
                                         <tr>
                                            <td><asp:DropDownList ID="cboApplyCreditCardId" CssClass="inputText" DataValueField="CreditCardId" DataTextField="CreditCardNameDisplay" runat="server" /></td>
                                            <td><asp:Button ID="btnApplyCreditCard" Text="Apply Card" OnClick="btnApplyCreditCard_Click" runat="server" /></td>
                                            <td width="30%">&nbsp;</td>
                                        </tr>
                                    </table>
                            </fieldset>
                        </asp:PlaceHolder>
                    </td>
                    <td>&nbsp;</td>
                    <td style="vertical-align: top">
                        <asp:PlaceHolder ID="plcOrderBillingAddress" Visible="false" runat="server">
                            <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td class="contentLabelRight">Email</td>
                                    <td>
                                        <asp:TextBox ID="txtOrderEmailAddress" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                                    </td>
                                    <td style="width: 10%">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Address</td>
                                    <td>
                                        <asp:TextBox ID="txtOrderBillingAddressLine1" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                                    </td>
                                    <td style="width: 10%">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">&nbsp;</td>
                                    <td>
                                        <asp:TextBox ID="txtOrderBillingAddressLine2" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                                    </td>
                                    <td style="width: 10%">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">City</td>
                                    <td>
                                        <asp:TextBox ID="txtOrderBillingCity" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                                    </td>
                                    <td style="width: 10%">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">State/Province</td>
                                    <td>
                                        <asp:TextBox ID="txtOrderBillingState" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                                    </td>
                                    <td style="width: 10%">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Postal Code</td>
                                    <td>
                                        <asp:TextBox ID="txtOrderBillingPostalCode" Width="50" CssClass="inputText" MaxLength="50" runat="server" />
                                    </td>
                                    <td style="width: 10%">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td class="contentLabelRight">Country</td>
                                    <td>
                                        <asp:TextBox ID="txtOrderBillingCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                                    </td>
                                    <td style="width: 10%">&nbsp;</td>
                                </tr>
                            </table>
                        </asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnSaveOrderDetail" CssClass="button" Text="Save Order" OnClick="btnSaveOrderDetail_Click" runat="server" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnSaveOrderDetailReturn" CssClass="button" Text="Save and Return" OnClick="btnSaveOrderDetailReturn_Click" runat="server" />
                    </td>
                    <td style="text-align: right">
                        <asp:Button ID="btnCreateNewOrder2" Text="Create Order" CssClass="button" OnClick="btnDisplayCreateOrder2_Click" runat="server" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3"><uc:OrderDetails id="ucOrderDetails" visible="false" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="3" style="white-space: nowrap;">
                        <asp:Button ID="btnViewPayments" CssClass="button" Text="Process Payments" OnClick="btnViewPayments_Click" runat="server" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnSendInvoice" CssClass="button" Text="Send Invoice" OnClick="btnSendInvoice_Click" runat="server" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnSendOrderReceipt" CssClass="button" Text="Send Order Receipt" OnClick="btnSendOrderReceipt_Click" runat="server" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnCancelOrderDetail" CssClass="button" Text="Back to List" OnClick="btnCancelOrderDetail_Click" runat="server" />
                        &nbsp;&nbsp;
            
                        <a id="lnkViewOrderHistory" href="#" class="action-link" runat="server">Order History</a>
                    </td>
                </tr>
            </table>


        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcOrderAdjustment" Visible="false" runat="server">
        <asp:Label ID="lblCompanyName" CssClass="dataValueStrong" runat="server" /><br />
        
        <fieldset class="commonControls">
            <legend class="commonControls">
                <asp:Literal ID="ltrOrderAdjustmentMode" runat="server" />&nbsp;<asp:Literal ID="ltrOrderNumber" runat="server" />
                <a id="orderAdjustmentHelp" href="#" onclick="return false;"><img id="Img4" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
            </legend>

             <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Category:</td>
                    <td>
                        <asp:DropDownList ID="ddlCategoryId" CssClass="inputText" AutoPostBack="true" OnSelectedIndexChanged="ddlCategoryId_Changed" runat="server" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                 <tr>
                    <td class="contentLabelRight">Product:</td>
                    <td>
                        <asp:DropDownList ID="ddlProductId" CssClass="inputText" AutoPostBack="true" OnSelectedIndexChanged="ddlProductId_Changed" runat="server" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Product SKU:</td>
                    <td><asp:TextBox ID="txtProductSku" CssClass="inputText" Width="200" MaxLength="50" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Item Description:</td>
                    <td><asp:TextBox ID="txtOrderItemName" CssClass="inputText" Width="300" MaxLength="100" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Exhibitor Notes (shown on invoice):</td>
                    <td>
                        <asp:TextBox ID="txtOrderItemExhibitorNotes" TextMode="MultiLine" Columns="55" Rows="4" CssClass="inputText" runat="server" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Quantity:</td>
                    <td><asp:TextBox ID="txtOrderItemQuantity" CssClass="inputText" Width="50" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Unit Price:</td>
                    <td>
                        <asp:TextBox ID="txtOrderItemUnitPrice" CssClass="inputText" Width="100" runat="server" />
                        &nbsp;<a id="lnkAllPrices" href="#" onclick="return false;">View All Pricing</a>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Unit Descriptor:</td>
                    <td><asp:TextBox ID="txtOrderItemUnitDescriptor" CssClass="inputText" Width="150" MaxLength="25" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Pricing Label:</td>
                    <td><asp:TextBox ID="txtOrderItemUnitPriceDescription" CssClass="inputText" Width="150" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Sort Order:</td>
                    <td><asp:TextBox ID="txtOrderItemIndex" CssClass="inputText" Width="20" MaxLength="5" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionDivider" colspan="3">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Additional Charge Type:</td>
                    <td>
                        <asp:Label ID="ltrAdditionalChargeType" runat="server" />
                        <asp:HiddenField ID="hdnAdditionalChargeAmt" runat="server" />
                        <asp:HiddenField ID="hdnAdditionalChargeType" runat="server" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Additional Charge Amount:</td>
                    <td>
                        <asp:TextBox ID="txtOrderItemAdditionalChargeAmount" CssClass="inputText" Width="100" runat="server" />
                        &nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkRecalculateAdditionalCharge" Text="<- Recalc Additional Charge" OnClick="lnkRecalculateAdditionalCharge_Click" runat="server" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Discount Deadline:</td>
                    <td>
                        <asp:Label ID="lblDiscountDeadline" CssClass="colData" runat="server" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Late Fee Type</td>
                    <td>
                        <asp:Label ID="lblLateFeeDeadline" CssClass="colData" runat="server" />&nbsp;
                        <asp:Label ID="ltrLateFee" CssClass="colData" runat="server" />
                        <asp:HiddenField ID="hdnLateFeeDeadline" runat="server" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Late Fee Amount:</td>
                    <td>
                        <asp:TextBox ID="txtOrderItemLateFeeAmount" CssClass="inputText" Width="100" runat="server" />
                        &nbsp;&nbsp;<asp:LinkButton ID="lnkRecalculateLateFee" Text="<- Recalc Late Fee" OnClick="lnkRecalculateLateFee_Click" runat="server" />
                        <asp:HiddenField ID="hdnLateFeeAmt" runat="server" />
                        <asp:HiddenField ID="hdnLateFeeType" runat="server" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Sales Tax</td>
                    <td>
                        <asp:TextBox ID="txtSalesTax" CssClass="inputText" style="background-color: #C0C0C0;" width="65" runat="server" />&nbsp;%
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Sales Tax Amount:</td>
                    <td>
                        <asp:TextBox ID="txtOrderItemSalesTaxAmount" CssClass="inputText" Width="100" runat="server" />
                        &nbsp;<asp:LinkButton ID="lnkRecalculateSalesTax" Text="<- Recalc Tax Amt." OnClick="lnkRecalculateSalesTax_Click" runat="server" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionDivider" colspan="3">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Required Attribute 1 (ex: Color: Blue):</td>
                    <td><telerik:RadComboBox ID="cboOrderItemRequiredAttribute1" MarkFirstMatch="true" AllowCustomText="true" MaxLength="255" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Required Attribute 2 (ex: Size: Small):</td>
                    <td><telerik:RadComboBox ID="cboOrderItemRequiredAttribute2" MarkFirstMatch="true" AllowCustomText="true" MaxLength="255" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Required Attribute 3 (ex: Start Time: 9:00 AM):</td>
                    <td><telerik:RadComboBox ID="cboOrderItemRequiredAttribute3" MarkFirstMatch="true" AllowCustomText="true" MaxLength="255" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Required Attribute 4 (ex: End Time: 10:00 AM):</td>
                    <td><telerik:RadComboBox ID="cboOrderItemRequiredAttribute4" MarkFirstMatch="true" AllowCustomText="true" MaxLength="255" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>

                <asp:PlaceHolder ID="plcAdditionalInfoResponses" runat="server">
                    <tr>
                        <td colspan="3" valign="top">
                            <asp:Repeater ID="rptrAdditionalInfoQuestions" OnItemDataBound="rptrAdditionalInfoQuestions_DataBound" runat="server">
                                <HeaderTemplate>
                                    <table border="0" cellpadding="2" cellspacing="1" width="100%">
                                        <tr>
                                            <th colspan="3">Additional Info Responses</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                        <tr>
                                            <td class="contentLabelRight">Question:</td>
                                            <td>
                                                <asp:TextBox ID="txtQuestion" TextMode="MultiLine" CssClass="inputText" Columns="75" Rows="2" runat="server" /><br />
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="contentLabelRight">Response:</td>
                                            <td><asp:TextBox ID="txtResponse" TextMode="MultiLine" CssClass="inputText" Columns="100" Rows="3" MaxLength="1000" runat="server" /></td>
                                            <td>&nbsp;</td>
                                        </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                        
                            </asp:Repeater>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                 <tr>
                    <td class="sectionDivider" colspan="3">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Interal (Admin-Only) Notes:</td>
                    <td>
                        <asp:TextBox ID="txtOrderItemNotes" TextMode="MultiLine" Columns="55" Rows="4" CssClass="inputText" runat="server" />
                        <br />
                        <asp:Label ID="lblOrderAuditTrail" CssClass="techieInfo" runat="server" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionDivider" colspan="3">&nbsp;</td>
                </tr>
            </table>

            <center>
                <asp:Button ID="btnSaveOrderItem" CssClass="button" Text="Recalculate and Save" OnClick="btnSaveOrderItem_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnSaveOrderItemNoCalc" CssClass="button" Text="Save w/o Recalculating" OnClick="btnSaveOrderItemNoCalc_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnCancelOrderItemAdjust" CssClass="button" Text="Cancel" OnClick="btnCancelOrderItemAdjust_Click" runat="server" />
            </center>

        </fieldset>

        <asp:HiddenField ID="hdnItemTypeCd" runat="server" />
    </asp:PlaceHolder>

    <asp:HiddenField ID="hdnExhibitorId" runat="server" />
    <asp:HiddenField ID="hdnOrderId" runat="server" />
    <asp:HiddenField ID="hdnOrderItemId" runat="server" />
    

    <asp:Panel CssClass="outerPopup" Style="display: none;" runat="server" ID="pnlOuterCreateOrder">
        <asp:Panel Width="550px" CssClass="innerPopup" runat="server" ID="pnlCreateOrder">
            <fieldset class="commonControls">
                <legend class="commonControls">Create Order</legend>
                <asp:Label ID="lblCreateOrderError" CssClass="informational" runat="server" />
                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Exhibitor</td>
                        <td>
                            <asp:DropDownList ID="ddlCreateOrderExhibitor" CssClass="inputText" AutoPostBack="false" runat="server"></asp:DropDownList>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Order Date:</td>
                        <td>
                            <asp:TextBox ID="txtCreateOrderDate" Visible="true" Width="150" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Payment Type:</td>
                        <td>
                            <asp:RadioButtonList ID="rbtnLstCreateOrderPaymentType" CssClass="inputText" runat="server" />

                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </table>
                <br />
                <center>
                    <asp:Button ID="btnCreateOrder" CssClass="button" Text="Create New Order" OnClick="btnCreateOrder_Click" runat="server" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnCancelCreateOrder" CssClass="button" Text="Cancel" onclick="btnCancelCreateOrder_Click" runat="server" />
                </center>
            </fieldset>
        </asp:Panel>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="MPE" runat="server" TargetControlID="dummyBtn" PopupControlID="pnlOuterCreateOrder" BackgroundCssClass="modalBackground"  DropShadow="true" CancelControlID="btnCancelCreateOrder" />
    <cc1:RoundedCornersExtender ID="RCE" runat="server" TargetControlID="pnlCreateOrder" BorderColor="black" Radius="6" />

    <asp:Button ID="dummyBtn" style="visibility: hidden;" runat="server" />


     <telerik:RadToolTip runat="server" ID="orderAdjustmentTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="orderAdjustmentHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(orderAdjustmentTT, "OrderAdjustmentHelp")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="allPricingTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="lnkAllPrices" IsClientID="true" Position="TopRight"
        Title="Product Pricing For:">
        <b><asp:Literal ID="ltrAllPricingProductName" runat="server" /></b><br /><br />
        Show Floor Price: <asp:Literal ID="ltrShowFloorPrice" runat="server" /><br />
        Standard Price: <asp:Literal ID="ltrStandardPrice" runat="server" /><br />
        Advanced Price: <asp:Literal ID="ltrAdvancedPrice" runat="server" />
        </telerik:RadToolTip>
</asp:Content>
