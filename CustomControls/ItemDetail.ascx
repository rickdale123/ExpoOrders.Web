<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemDetail.ascx.cs" Inherits="ExpoOrders.Web.CustomControls.ItemDetail" %>
<%@ Register Src="~/CustomControls/FormQuestions.ascx" TagPrefix="uc" TagName="FormQuestions" %>
<%@ Register Src="~/CustomControls/PricingLabels.ascx" TagPrefix="uc" TagName="PricingLabels" %>

<h1><asp:Literal ID="ltrParentCategoryName" runat="server" /></h1>
        
    <fieldset class="commonControls">
        <legend class="commonControls"><asp:Literal ID="ltrCategoryName" runat="server" /></legend>
                
            <div class="ProductImage">    
                <asp:Image ID="imgProductDetailImage" border="0" runat="server" />
                &nbsp;
            </div>

            <asp:PlaceHolder ID="plcAdditionalImages" Visible="false" runat="server">
                <ul class="additionalProductImages">
                    <asp:Repeater ID="rptrAdditionalImages" OnItemDataBound="rptrAdditionalImages_ItemDataBound" runat="server">
                        <ItemTemplate>
                            <li class="additionalImage">
                                <a id="lnkAdditionalImage" href="#" runat="server"><asp:Image ID="imgAdditionalImage" width="60" height="55" CssClass="thumbnailImage" runat="server" /></a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </asp:PlaceHolder>
                        
            <div class="ProductName">
                <asp:Literal ID="ltrProductName" runat="server" />
            </div>
                        
            <div class="ProductDescription">
                <asp:Literal ID="ltrProductDescription" runat="server" />
            </div>
                    
    </fieldset>

    <br />

        <fieldset class="commonControls">
            <legend class="commonControls">Pricing Details</legend>
                    
             <uc:PricingLabels id="ucPricingLabels" visible="false" runat="server" />

            <asp:PlaceHolder ID="plcSubmissionDeadline" Visible="false" runat="server">
                Submission Deadline: <asp:Literal ID="ltrSubmissionDeadline" runat="server" /><br />
            </asp:PlaceHolder>

            <table border="0" width="100%" cellpadding="2" cellspacing="0">
                <tr class="Item">
                    <td class="contentLabel" align="right">&nbsp;</td>
                    <td class="lateIndicator" align="right"></td>
                    <td width="50">&nbsp;</td>
                    <td width="100" class="contentLabel" align="right">Your Cost</td>
                    <td rowspan="6" width="25%">&nbsp;</td>
                </tr>
                <tr class="altItem">
                    <td class="contentLabel" align="right"><asp:Label ID="lblUnitPriceLabel" Text="Unit Price" runat="server" /></td>
                    <td align="right"><asp:Label ID="lblUnitPrice" runat="server" /></td>
                    <td>&nbsp;</td>
                    <td align="right"><span id="TotalUnitPrice"><asp:Literal ID="ltrTotalUnitPrice" runat="server" /></span></td>
                </tr>
                <tr class="item">
                    <td class="contentLabel" align="right">Additional Charges</td>
                    <td align="right"><asp:Literal ID="ltrAdditionalCharge" runat="server" /></td>
                    <td>&nbsp;</td>
                    <td align="right"><span id="TotalAdditionalCharges"><asp:Literal ID="ltrTotalAdditionalCharges" runat="server" /></span></td>
                </tr>
                <tr class="altItem">
                    <td class="contentLabel" align="right">Late Fee</td>
                    <td align="right"><asp:Literal ID="ltrLateFee" runat="server" /></td>
                    <td>&nbsp;</td>
                    <td align="right"><span id="TotalLateFee"><asp:Literal ID="ltrTotalLateFee" runat="server" /></span></td>
                </tr>
                <tr class="Item">
                    <td class="contentLabel" align="right">Sales Tax</td>
                    <td align="right"><asp:Literal ID="ltrSalesTax" runat="server" /></td>
                    <td>&nbsp;</td>
                    <td align="right"><span id="TotalSalesTax"><asp:Literal ID="ltrTotalSalesTax" runat="server" /></span></td>
                </tr>
                <tr class="Item">
                    <td colspan="4"><hr /></td>
                </tr>
                <tr class="Item">
                    <td colspan="3" align="right">
                        <asp:Literal ID="ltrQuantityLabel" Text="Quantity" runat="server" />
                    </td>
                    <td align="left">
                        <asp:TextBox ID="txtQuantity" CssClass="inputText" Text="0" MaxLength="6" size="5" runat="server" />
                    </td>
                    <td width="25%">&nbsp;<input type="button" id="btnCalcCost" onclick="calcCost();" value="Calculate Cost" class="button" /></td>
                </tr>

                <asp:PlaceHolder ID="plcMinimumQuantityRequired" Visible="false" runat="server">
                    <tr class="Item">
                        <td colspan="3" align="right">
                            <asp:Label ID="lblMinimumQuantityRequired" CssClass="informational" Text="Quantity" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                        <td width="25%">&nbsp;</td>
                    </tr>
                </asp:PlaceHolder>
                

                <asp:PlaceHolder ID="plcRequiredAttribute1" visible="false" runat="server">
                    <tr class="Item">
                        <td colspan="3" align="right">
                            <asp:Label ID="lblRequiredAttributeLabel1" CssClass="dataLabel" runat="server" />
                        </td>
                        <td align="left"><asp:DropDownList ID="ddlRequiredAttributeChoices1" runat="server" /></td>
                        <td width="25%">&nbsp;</td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcRequiredAttribute2" visible="false" runat="server">
                    <tr class="Item">
                        <td colspan="3" align="right">
                            <asp:Label ID="lblRequiredAttributeLabel2" CssClass="dataLabel" runat="server" />
                        </td>
                        <td align="left"><asp:DropDownList ID="ddlRequiredAttributeChoices2" runat="server" /></td>
                        <td width="25%">&nbsp;</td>
                    </tr>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plcRequiredAttribute3" visible="false" runat="server">
                    <tr class="Item">
                        <td colspan="3" align="right">
                            <asp:Label ID="lblRequiredAttributeLabel3" CssClass="dataLabel" runat="server" />
                        </td>
                        <td align="left"><asp:DropDownList ID="ddlRequiredAttributeChoices3" runat="server" /></td>
                        <td width="25%">&nbsp;</td>
                    </tr>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plcRequiredAttribute4" visible="false" runat="server">
                    <tr class="Item">
                        <td colspan="3" align="right">
                            <asp:Label ID="lblRequiredAttributeLabel4" CssClass="dataLabel" runat="server" />
                        </td>
                        <td align="left"><asp:DropDownList ID="ddlRequiredAttributeChoices4" runat="server" /></td>
                        <td width="25%">&nbsp;</td>
                    </tr>
                </asp:PlaceHolder>

                <tr>
                    <td colspan="3" class="contentLabel" align="right">Total Cost</td>
                    <td align="left"><span id="TotalCost" class="contentStrong"><asp:Literal ID="ltrTotalCost" runat="server" /></span></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="5">&nbsp;</td>
                </tr>
                <tr id="trAdditionalInfo" runat="server">
                    <td colspan="5" valign="top">
                                    
                        <fieldset class="additionalInfo">
                            <legend class="additionalInfo">Additional Information</legend>

                            <uc:FormQuestions id="ucFormQuestions" visible="false" runat="server" />

                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="5" align="center">
                        <asp:Button ID="btnAddToCart" OnClick="btnAddToCart_Click" Text="Add To Cart" CssClass="button" ValidationGroup="AddToCart" runat="server" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnBackToCategory" OnClick="btnBackToCategory_Click" ValidationGroup="none" Text="Back to List" CssClass="button" runat="server" />
                        <br /><br />
                        <asp:Label ID="lblWarningMessage" CssClass="errorMessage" Visible="false" runat="server" />
                    </td>
                </tr>
            </table>
        </fieldset>
              

<input type="hidden" id="hdnUnitPrice" value="" runat="server" />
<input type="hidden" id="hdnAdditionalChargeAmt" value="" runat="server" />
<input type="hidden" id="hdnAdditionalChargeType" value="" runat="server" />
<input type="hidden" id="hdnLateFeeAmt" value="" runat="server" />
<input type="hidden" id="hdnLateFeeType" value="" runat="server" />
<input type="hidden" id="hdnSalesTaxPct" value="" runat="server" />
<input type="hidden" id="hdnCurrencySymbol" value="" runat="server" />
<input type="hidden" id="hdnIsCalcByAttribs" value="" runat="server" />