<%@ Page Title="" Language="C#" MasterPageFile="~/Exhibitors/ExhibitorsMaster.Master" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="ExpoOrders.Web.Exhibitors.Payment" %>
<%@ MasterType VirtualPath="~/Exhibitors/ExhibitorsMaster.Master" %>

<%@ Register Src="~/CustomControls/OrderDetail.ascx" TagPrefix="uc" TagName="OrderDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
<script language="javascript" type="text/javascript">

    function loadPaymentOptions() {
        paintNewCreditCard();
    }

    function paintNewCreditCard() {

        if (convertBool(hiddenValue('PageContent_hdnNoCardsOnFile'))) {
            displayNewCreditCard(true);
        }
        else {
            displayNewCreditCard(selectedRadioButtonValue('SelectedCreditCard') == 0);
        }
    }

    function onCreditCardSelected(rdo) {
        paintNewCreditCard();
    }

    function displayNewCreditCard(visible) {

        if (visible) {
            showObject('PageContent_newCreditCardDetails');
        }
        else {
            hideObject('PageContent_newCreditCardDetails');
        }
    }

</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">
<CustomControls:ValidationErrors ID="PageErrors" EnableClientScript="true" CssClass="errorMessageBlock" runat="server" />

    <asp:PlaceHolder ID="plcPaymentSelection" Visible="false" runat="server">
     
        <fieldset class="commonControls">
            <legend class="commonControls">Payment Options</legend>

            <div id="payByCreditCard">

            <asp:Repeater ID="rptrExistingCreditCards" OnItemDataBound="rptrExistingCreditCards_ItemDataBound" EnableViewState="false" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                        <tr class="Item">
                            <td>&nbsp;</td>
                            <td class="colHeader">Card Type</td>
                            <td class="colHeader">Name on Card</td>
                            <td class="colHeader">Card Number</td>
                            <td class="colHeader">Expiration Date</td>
                            <td class="colHeader">Email Address</td>
                            <td>&nbsp;</td>
                            <td width="10%">&nbsp;</td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr id="trCreditCard" class="Item" runat="server">
                        <td class="colData">
                            <input type="radio" id='rdoSelectedCreditCard_<%#DataBinder.Eval(Container.DataItem, "CreditCardId")%>' 
                                value="<%# DataBinder.Eval(Container.DataItem, "CreditCardId")%>" 
                                name="SelectedCreditCard" 
                                onclick="onCreditCardSelected(this);"
                                <%# IsChecked( (int)(DataBinder.Eval(Container.DataItem, "CreditCardId"))) %>

                                />
                        </td>
                        <td class="colData"><asp:Literal ID="ltrCreditCardType" runat="server" /></td>
                        <td class="colData"><%# HtmlEncode(DataBinder.Eval(Container.DataItem, "NameOnCard"))%></td>
                        <td class="colData"><%# HtmlEncode(DataBinder.Eval(Container.DataItem, "CardNumberMasked"))%></td>
                        <td class="colData"><%# string.Format("{0}/{1}", DataBinder.Eval(Container.DataItem, "ExpirationMonth"), DataBinder.Eval(Container.DataItem, "ExpirationYear")) %></td>
                        <td class="colData"><%# HtmlEncode(DataBinder.Eval(Container.DataItem, "EmailAddress"))%></td>
                        <td>&nbsp;</td>
                        <td width="10%">&nbsp;</td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        <tr>
                            <td class="colData"><input type="radio" id="rdoSelectedCreditCard_0" 
                                    name="SelectedCreditCard" onclick="onCreditCardSelected(this);" 
                                    value="0" 
                                    <%# IsChecked(0) %>
                                    /></td>
                            <td class="colData" colspan="6">(Use New Credit Card) </td>
                            <td width="10%">&nbsp;</td>
                        </tr>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

                <div id="newCreditCardDetails" style="display: none;" runat="server">
                    <table border="0" width="100%" cellpadding="1" cellspacing="0">
                        <tr>
                            <td class="contentLabelRight">Name on Card:&nbsp;</td>
                            <td>
                                <asp:TextBox ID="txtCreditCardName" Width="200" CssClass="inputText" MaxLength="50" runat="server" />

                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="errorMessage" EnableClientScript="false" ErrorMessage="Name is Required" runat="server" ControlToValidate="txtCreditCardName" ValidationGroup="CreditCardInfo">Missing Name</asp:RequiredFieldValidator>
                            </td>
                            <td style="width:10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Card Type:&nbsp;</td>
                            <td>
                                <asp:DropDownList ID="ddlCreditCardType" CssClass="inputText" runat="server" />

                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="errorMessage" ErrorMessage="Card Type is Required" EnableClientScript="false" runat="server" ControlToValidate="ddlCreditCardType" ValidationGroup="CreditCardInfo">Missing Card Type</asp:RequiredFieldValidator>
                            </td>
                            <td style="width:10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Card Number:&nbsp;</td>
                            <td>
                                <asp:TextBox ID="txtCreditCardNumber" Width="200" CssClass="inputText" MaxLength="50" runat="server" />
                                &nbsp;<span class="techieInfo">Please enter all digits, no spaces (ex: 4321432143214321)</span>

                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" CssClass="errorMessage" ErrorMessage="Card Type is Required"  EnableClientScript="false" runat="server" ControlToValidate="txtCreditCardNumber" ValidationGroup="CreditCardInfo">Missing Card Type</asp:RequiredFieldValidator>
                            </td>
                            <td style="width:10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Expiration Date:&nbsp;</td>
                            <td class="colData" style="white-space: nowrap">
                                <asp:DropDownList ID="ddlCreditCardExpMonth" CssClass="inputText" runat="server" />&nbsp;/&nbsp;<asp:DropDownList ID="ddlCreditCardExpYear" CssClass="inputText" runat="server" />
                        
                                <asp:CustomValidator ID="customCreditCardExpDateValidator" CssClass="errorMessage" ValidationGroup="CreditCardInfo" EnableClientScript="false" runat="server"></asp:CustomValidator>
                                &nbsp;<asp:Label ID="Label7" CssClass="contentLabel" runat="server">&nbsp;Security Code (CCV):&nbsp;</asp:Label>
                                <asp:TextBox ID="txtCreditCardSecurityCode" Width="60" CssClass="inputText" MaxLength="10" runat="server" />
                                <span class="techieInfo">Typically a separate group of <a href="http://en.wikipedia.org/wiki/Card_security_code" class="techieInfo" target="_blank">3 digits</a> to the right of the signature strip</span>
                            </td>
                            <td style="width:10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Address:&nbsp;</td>
                            <td><asp:TextBox ID="txtCreditCardAddressLine1" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                            <td style="width:10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td><asp:TextBox ID="txtCreditCardAddressLine2" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                            <td style="width:10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">City:&nbsp;</td>
                            <td><asp:TextBox ID="txtCreditCardCity" Width="125" CssClass="inputText" MaxLength="50" runat="server" /></td>
                            <td style="width:10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">State/Province/Region:&nbsp;</td>
                            <td><asp:TextBox ID="txtCreditCardState" Width="200" CssClass="inputText" MaxLength="150" runat="server" /></td>
                            <td style="width:10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Postal Code:&nbsp;</td>
                            <td><asp:TextBox ID="txtCreditCardPostalCode" Width="50" CssClass="inputText" MaxLength="20" runat="server" /></td>
                            <td style="width:10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Country:&nbsp;</td>
                            <td><asp:TextBox ID="txtCreditCardCountry" Width="200" CssClass="inputText" MaxLength="150" runat="server" /></td>
                            <td style="width:10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Email Address:&nbsp;</td>
                            <td><asp:TextBox ID="txtCreditCardEmail" Width="200" CssClass="inputText" MaxLength="150" runat="server" /></td>
                            <td style="width:10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td colspan="2"><asp:CheckBox ID="chkSaveNewCreditCard" class="inputText" Text="Save Credit Card on file for later?" runat="server" /></td>
                        </tr>
                    </table>

                </div>
            </div>
            
            <br />

            <center>
                <asp:Button ID="btnPayByCard" CssClass="button" Text="Pay By Credit Card" ValidationGroup="PayByCreditCard" OnClick="btnPayByCard_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnPayByCheck" CssClass="button" Text="Pay By Check" ValidationGroup="OrderReview" OnClick="btnPayByCheck_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnPayByWire" CssClass="button" Text="Pay By Wire Transfer" ValidationGroup="OrderReview" OnClick="btnPayByWire_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnNoPayment" CssClass="button" Text="Checkout with No Payment" ValidationGroup="OrderReview" OnClick="btnNoPayment_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnBackToCart" CssClass="button" Text="Back to Shopping Cart" OnClick="btnBackToCart_Click" runat="server" />

            </center>
            <br />

        </fieldset>

    </asp:PlaceHolder>


    <asp:PlaceHolder ID="plcOrderReview" Visible="false" runat="server">
    
        <fieldset class="commonControls">
            <legend class="commonControls">Order Review</legend>
            
            
            <uc:OrderDetails id="ucOrderDetails" visible="false" runat="server" />

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Payment Type:&nbsp;</td>
                    <td class="colData"><asp:literal ID="ltrOrderPaymentType" runat="server" /></td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                <asp:PlaceHolder ID="plcOrderCreditCard" runat="server">
                    <tr>
                        <td class="contentLabelRight">Name on Card:&nbsp;</td>
                        <td class="colData"><asp:Literal ID="ltrOrderNameOnCreditCard" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Card Type:&nbsp;</td>
                        <td class="colData">
                            <asp:Literal ID="ltrOrderCreditCardType" runat="server" />
                            &nbsp;&nbsp;<asp:LinkButton ID="lnkCorrectPaymentProblem" CssClass="errorMessage" OnClick="lnkCorrectPaymentProblem_Click" runat="server" />
                            <input type="hidden" id="hdnCreditCardType" value="" runat="server" />
                        </td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Card Number:&nbsp;</td>
                        <td class="colData">
                            <asp:Literal ID="ltrOrderCreditCardNumber" runat="server" />
                            <input type="hidden" id="hdnOrderCreditCardNumber" value="" runat="server" />
                        </td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                        <tr>
                        <td class="contentLabelRight">Expiration Date:&nbsp;</td>
                        <td class="colData" style="white-space: nowrap">
                            <asp:Literal ID="ltrOrderCreditCardExpMonth" runat="server" />&nbsp;/&nbsp;<asp:Literal ID="ltrOrderCreditExpYear" runat="server" />
                        
                            &nbsp;<asp:Label ID="Label1" CssClass="contentLabel" runat="server">&nbsp;Security Code:&nbsp;</asp:Label>
                            <asp:Literal ID="ltrOrderCreditCardSecurityCode" runat="server" />
                            <input type="hidden" id="hdnOrderCreditCardSecurityCode" value="" runat="server" />
                        </td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Address:&nbsp;</td>
                        <td class="colData"><asp:Literal ID="ltrOrderStreet1" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td class="colData"><asp:Literal ID="ltrOrderStreet2" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">City:&nbsp;</td>
                        <td class="colData"><asp:Literal ID="ltrOrderCity" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">State/Province/Region:&nbsp;</td>
                        <td class="colData"><asp:Literal ID="ltrOrderStateProvinceRegion" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Postal Code:&nbsp;</td>
                        <td class="colData"><asp:Literal ID="ltrOrderPostalCode" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Country:&nbsp;</td>
                        <td class="colData"><asp:Literal ID="ltrOrderCountry" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td class="contentLabelRight">Email Address:&nbsp;</td>
                    <td><asp:TextBox ID="txtOrderEmailAddress" CssClass="inputText" Width="200" runat="server" /></td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="termsConditions" colspan="3">
                        <fieldset class="commonControls">
                            <legend class="commonControls"><asp:literal ID="ltrTermsAndConditionsGroupName" runat="server" /></legend>
                            <asp:literal ID="ltrTermsAndConditions" runat="server" /><br /><br />
                            <asp:CheckBox ID="chkTermsConditions" Text="I Accept the Terms and Conditions" runat="server" />
                        </fieldset>
                    </td>
                </tr>
            </table>

            <center>
            
                <asp:Button ID="btnSubmitOrder" CssClass="button" Text="Submit Order" ValidationGroup="OrderSubmit" OnClick="btnSubmitOrder_Click" runat="server" />    
                &nbsp;
                &nbsp;
                <asp:Button ID="btnChoosePaymentMethod" CssClass="button" Text="Choose a Payment Option" OnClick="btnChoosePaymentMethod_Click" runat="server" />
                &nbsp;
                &nbsp;
                <asp:Button ID="btnBackToCart2" CssClass="button" Text="Back to Shopping Cart" OnClick="btnBackToCart_Click" runat="server" />

            </center>
            <br />
        </fieldset>

        
        <input type="hidden" id="hdnPaymentType" value="" runat="server" />
        <input type="hidden" id="hdnSaveCreditCard" value="" runat="server" />
        <input type="hidden" id="hdnSelectedCreditCardId" value="" runat="server" />
        

    </asp:PlaceHolder>
    <input type="hidden" id="hdnNoCardsOnFile" value="false" runat="server" />
    
</asp:Content>
