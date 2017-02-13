<%@ Page Title="" Language="C#" MasterPageFile="~/Exhibitors/ExhibitorsMaster.Master" AutoEventWireup="true" CodeBehind="ShoppingCart.aspx.cs" Inherits="ExpoOrders.Web.Exhibitors.ShoppingCart" %>
<%@ MasterType VirtualPath="~/Exhibitors/ExhibitorsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

<CustomControls:ValidationErrors ID="PageErrors" EnableClientScript="true" CssClass="errorMessageBlock" runat="server" />

<asp:PlaceHolder id="plcShoppingCart" Visible="true" runat="server">

    <fieldset class="commonControls">
        <legend class="commonControls">Shopping Cart</legend>

         <asp:PlaceHolder ID="plcEmptyShoppingCart" Visible="false" runat="server">
            <center>Shopping cart contains no items.</center>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plcShoppingCartDetails" Visible="false" runat="server">

            <asp:Repeater ID="rptrShoppingCartItems" OnItemDataBound="rptrShoppingCartItems_ItemDataBound" runat="server">
                <HeaderTemplate>
                    <table class="shoppingCart" border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
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
                            <td class="colData">
                                <asp:RequiredFieldValidator ID="txtQuantityRequired" ErrorMessage="Quantity must have a value" EnableClientScript="false" ValidationGroup="ShoppingCartUpdate" ControlToValidate="txtQuantity" CssClass="errorIndicator" runat="server">*</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="txtQuantityNumeric" ErrorMessage="Quantity must be a numeric value." ValidationExpression="^-?\d*\.?\d*" EnableClientScript="false" ValidationGroup="ShoppingCartUpdate" ControlToValidate="txtQuantity" CssClass="errorIndicator" runat="server">*</asp:RegularExpressionValidator>
                                <asp:TextBox ID="txtQuantity" CssClass="inputText" MaxLength="6" width="50" runat="server" />
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
                            </td>
                            <td class="colDataRight">
                                <asp:Label ID="lblUnitPrice" runat="server" />
                                <br />
                                <asp:Label ID="lblUnitPriceDescription" CssClass="earlyBirdPricing" runat="server" />
                            </td>
                            <td class="colDataRight">
                                Late Fees <asp:Label ID="lblLateFees" runat="server" /><br />
                                Additional Charges <asp:Label ID="lblAdditionalCharges" runat="server" /><br />
                                Sales Tax <asp:Label ID="lblSalesTax" runat="server" />
                            </td>
                            <td class="colDataRight"><asp:Label ID="lblSubTotal" runat="server" /></td>
                            <td class="colData"><asp:LinkButton ID="lnkDeleteItem" CssClass="action-link" Text="remove" OnClick="lnkDeleteItem_Click" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr id="trItemAdditionalInfo" runat="server">
                            <td>&nbsp;</td>
                            <td class="colAdditionalInfo" colspan="6">
                                <asp:Repeater id="rptItemAdditionalInfo" OnItemDataBound="rptItemAdditionalInfo_DataBound" runat="server">
                                    <HeaderTemplate>
                                        <a href="#">Additional Info:</a><br />
                                        <ul class="itemAdditionalInfo">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <asp:Label ID="lblAdditionalInfoQuestion" runat="server" />
                                            <asp:Label ID="lblAdditionalInfoAnswer" runat="server" /><br />
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
                            <td><asp:Button ID="btnUpdateShoppingCart" Text="Update" CssClass="button" OnClick="btnUpdateShoppingCart_Click" ValidationGroup="ShoppingCartUpdate" runat="server" /></td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td class="orderTotalLabel"><b>Order Total</b></td>
                            <td class="orderTotalAmount"><asp:Literal ID="ltrOrderTotal" runat="server" /></td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                        </tr>
                    </table>
                    <br />
                     <asp:Label ID="lblCheckoutNote" CssClass="termsConditions" runat="server">
                        Note: Items added to the Shopping Cart are not processed as an Order until you complete the 'Checkout' process.
                    </asp:Label>
                    <br />
                </FooterTemplate>
            </asp:Repeater>
        
            <br />
            <center>
                <asp:Button ID="btnContinueShopping" CssClass="button" Text="Continue Shopping" ValidationGroup="ShoppingCartUpdate" OnClick="btnContinueShopping_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnCheckOut" CssClass="button" Text="Proceed to Checkout" ValidationGroup="ShoppingCartUpdate" OnClick="btnCheckOut_Click" runat="server" />
            </center>
        </asp:PlaceHolder>
 

        </fieldset>
</asp:PlaceHolder>

</asp:Content>
