<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderDetail.ascx.cs" Inherits="ExpoOrders.Web.CustomControls.OrderDetail" %>

<asp:PlaceHolder ID="plcOrderHeader" Visible="false" runat="server">
    <table id="tblOrder" runat="server" class="item" border="1" cellspacing="0" cellpadding="2" style="border-width:1px;border-style:solid;border-collapse:collapse;" width="100%">
        <tr>
	        <td class="colData" width="250" valign="top">
                
                <asp:PlaceHolder ID="plcCompanyBoothInfo" Visible="false" runat="server">
              
                    <asp:Label ID="lblCompanyNameValue" CssClass="dataValueStrong" runat="server" /><br />

                    <asp:Label ID="lblBoothNumber" CssClass="label" runat="server">Booth #</asp:Label>&nbsp;
                    <asp:Label ID="lblBoothNumberValue" CssClass="dataValueStrong" runat="server" /><br />

                    <asp:Label ID="lblBoothDescription" CssClass="label" runat="server">Size</asp:Label>&nbsp;
                    <asp:Label ID="lblBoothDescriptionValue" CssClass="dataValueStrong" runat="server" /><br />

                </asp:PlaceHolder>

                <asp:Label ID="lblOrderDate" CssClass="label" runat="server">Order Date</asp:Label><br />
                <asp:Label ID="lblOrderDateValue" CssClass="dataValueStrong" runat="server" /><br />
                <a id="lnkPrintReceipt" class="action-link" href="#" runat="server">View Order Receipt</a>
                <asp:PlaceHolder ID="plcDeliveryReceipt" Visible="false" runat="server">
                    |
                    <a id="lnkPrintDeliveryReceipt" class="action-link" visible="false" href="#" runat="server">Delivery Receipt</a>
                </asp:PlaceHolder>
                
                <br /><br />
                

                <asp:Label ID="lblConfirmationNumber" CssClass="labelStrong" runat="server">Order Number:</asp:Label>&nbsp;
                <asp:Label ID="lblConfirmationNumberValue" CssClass="dataValue" runat="server" /><br />

                <asp:Label ID="lblUserName" CssClass="labelStrong" runat="server">Placed By:</asp:Label>&nbsp;
                <asp:Label ID="lblUserNameValue" CssClass="dataValue" runat="server" /><br />
            </td>
            <td class="colData" valign="top">
                <asp:Label ID="lblOrderStatus" CssClass="labelStrong" runat="server">Status:</asp:Label>&nbsp;
                <asp:Label ID="lblOrderStatusValue" runat="server" /><br />

                <asp:PlaceHolder ID="plcPaymentType" Visible="true" runat="server">
                    <asp:Label ID="lblOrderPaymentType" CssClass="labelStrong" runat="server">Payment Type:</asp:Label>&nbsp;
                    <asp:Label ID="lblOrderPaymentTypeValue" runat="server" /><br />
                </asp:PlaceHolder>
                

                <asp:PlaceHolder ID="plcBillingInfo" Visible="false" runat="server">
                    <asp:Label ID="lblBillingAddress" CssClass="labelStrong" runat="server">Billing Address:</asp:Label>&nbsp;
                    <asp:Label ID="lblBillingAddressValue" CssClass="dataValue" runat="server" /><br />
                    <asp:Label ID="lblBillingCity" CssClass="dataValue" runat="server" />,&nbsp;
                    <asp:Label ID="lblBillingState" CssClass="dataValue" runat="server" /> &nbsp;&nbsp;
                    <asp:Label ID="lblBillingPostalCode" CssClass="dataValue" runat="server" /><br />
                    <asp:Label ID="lblBillingCountry" CssClass="dataValue" runat="server" /><br /><br />

                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plcCreditCardInfo" Visible="false" runat="server">
                    <asp:Label ID="lblCreditCardInfo" CssClass="labelStrong" runat="server">Credit Card:</asp:Label>&nbsp;
                    <asp:Label ID="lblCreditCardType" CssClass="dataValue" runat="server" /><br />
                    <asp:Label ID="lblCreditCardNameOnCard" CssClass="dataValue" runat="server" /><br />
                    <asp:Label ID="lblCreditCardNumber" CssClass="dataValue" runat="server" />&nbsp;&nbsp;
                    <asp:Label ID="lblCreditCardExpirationDate" CssClass="dataValue" runat="server" /><br />
                    <asp:Label ID="lblCreditCardSecurityCode" CssClass="dataValue" runat="server" />
                </asp:PlaceHolder>
            </td>
        </tr>
    </table>

    <hr />

</asp:PlaceHolder>

<asp:PlaceHolder ID="plcOrderItems" Visible="false" runat="server">

    <asp:LinkButton ID="lnkBtnAddOrderLineItem" CssClass="action-link" Text="Add Line Item" CommandName="AddOrderItem" OnClick="lnkBtnEditOrderLineItem_Click" Visible="false" runat="server" />

    <asp:Repeater ID="rptrOrderItems" OnItemDataBound="rptrOrderItems_ItemDataBound" runat="server">
        <HeaderTemplate>
            <table class="shoppingCart" border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td>&nbsp;</td>
                    <td class="colHeader">Quantity</td>
                    <td class="colHeader">Description</td>
                    <td class="colHeader">Unit Price</td>
                    <td class="colHeader">Additional Charges</td>
                    <td class="colHeader">Amount</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
                <tr id="trShoppingCartItem" runat="server">
                    <td class="colData" style="white-space: nowrap;">
                        <asp:PlaceHolder ID="plcLineItemAdjustmentControls" Visible="false" runat="server">
                            
                            <asp:LinkButton ID="lnkBtnEditOrderLineItem" CssClass="action-link" Text="Edit" CommandName="EditOrderItem" OnClick="lnkBtnEditOrderLineItem_Click" runat="server" />
                            &nbsp;|&nbsp;
                            <asp:LinkButton ID="lnkBtnDeleteOrderLineItem" CssClass="action-link" Text="Delete" CommandName="DeleteOrderItem" OnClick="lnkBtnEditOrderLineItem_Click" runat="server" />

                            <asp:PlaceHolder ID="plcPrintInstallDismantleReport" Visible="false" runat="server">
                                <br />
                                <a id="lnkPrintInstallDismantle" href="#" runat="server">View WorkOrder</a>
                            </asp:PlaceHolder>
                            <br />
                            <asp:Label ID="lblOrderItemInsertedDateTime" runat="server" /><br />
                            <asp:Label ID="lblOrderItemId" CssClass="techieInfo" runat="server" />
                        </asp:PlaceHolder>
                    </td>
                    <td class="colDataCenter">
                        <asp:Literal ID="ltrQuantity" runat="server" />
                    </td>
                    <td class="colData">
                        <asp:Label ID="lblProductCategory" CssClass="ItemCategory" Visible="true" runat="server" /><br />
                        <asp:Label ID="lblProductName" CssClass="ItemProductName" Visible="true" runat="server" /><br />

                        <asp:PlaceHolder ID="plcRequiredAttribute1" Visible="false" runat="server">
                            <br /><asp:Label ID="lblRequiredAttribute1" CssClass="ItemRequiredAttribute" runat="server" />
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="plcRequiredAttribute2" Visible="false" runat="server">
                            <br /><asp:Label ID="lblRequiredAttribute2" CssClass="ItemRequiredAttribute" runat="server" />
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="plcRequiredAttribute3" Visible="false" runat="server">
                            <br /><asp:Label ID="lblRequiredAttribute3" CssClass="ItemRequiredAttribute" runat="server" />
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="plcRequiredAttribute4" Visible="false" runat="server">
                            <br /><asp:Label ID="lblRequiredAttribute4" CssClass="ItemRequiredAttribute" runat="server" />
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="plcExhibitorNotes" runat="server">
                            <br /><i><asp:Label ID="lblExhibitorNotes" runat="server" /></i>
                        </asp:PlaceHolder>
                    </td>
                    <td class="colDataRight">
                        <asp:Label ID="lblUnitPrice" runat="server" />
                        <asp:PlaceHolder ID="plcUnitPriceDescription" Visible="false" runat="server">
                            <br />
                            <asp:Label ID="lblUnitPriceDescription" CssClass="earlyBirdPricing" runat="server" />
                        </asp:PlaceHolder>
                    </td>
                    <td class="colDataRight">
                        Late Fees <asp:Literal ID="ltrLateFees" runat="server" /><br />
                        Additional Charges <asp:Literal ID="ltrAdditionalCharges" runat="server" /><br />
                        Sales Tax <asp:Literal ID="ltrSalesTax" runat="server" />
                    </td>
                    <td class="colDataRight"><asp:Literal ID="ltrSubTotal" runat="server" /></td>
                    <td class="colData">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr id="trItemAdditionalInfo" runat="server">
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td class="colAdditionalInfo" colspan="6">
                        <asp:Repeater id="rptItemAdditionalInfo" OnItemDataBound="rptItemAdditionalInfo_DataBound" runat="server">
                            <HeaderTemplate>
                                <a href="#">Additional Info:</a><br />
                                <ul class="itemAdditionalInfo">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li>
                                    <asp:Literal ID="ltrAdditionalInfoQuestion" runat="server" />
                                    <asp:Literal ID="ltrAdditionalInfoAnswer" runat="server" /><br />
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
        </ItemTemplate>
        <FooterTemplate>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td class="orderTotalLabel"><b>Order Total</b></td>
                    <td class="orderTotalAmount"><asp:Literal ID="ltrOrderTotal" runat="server" /></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:PlaceHolder>
        

<asp:PlaceHolder ID="plcFormQuestions" Visible="false" runat="server">
    <asp:Repeater ID="rptrFormOrder" OnItemDataBound="rptrFormOrder_ItemDataBound" runat="server">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <asp:Label ID="lblFormName" CssClass="labelStrong" runat="server">Form Name:</asp:Label>&nbsp; 
            <asp:Label ID="lblFormNameValue" CssClass="dataValueStrong" runat="server"><%#DataBinder.Eval(Container.DataItem, "ItemDescription")%></asp:Label><br />

            <asp:Repeater id="rptrFormQuestions" OnItemDataBound="rptItemAdditionalInfo_DataBound" runat="server">
                <HeaderTemplate>
                    <ul class="itemAdditionalInfo">
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <asp:Literal ID="ltrAdditionalInfoQuestion" runat="server" />&nbsp;
                        <b><asp:Literal ID="ltrAdditionalInfoAnswer" runat="server" /></b><br />
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
        </ItemTemplate>
        <FooterTemplate>
        </FooterTemplate>
    </asp:Repeater>
</asp:PlaceHolder>

            