<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryItems.ascx.cs" Inherits="ExpoOrders.Web.CustomControls.CategoryItems" %>
<%@ Register Src="~/CustomControls/PricingLabels.ascx" TagPrefix="uc" TagName="PricingLabels" %>

    <h1><asp:Literal ID="ltrParentCategoryName" runat="server" /></h1>

    <fieldset class="commonControls">
        <legend class="commonControls"><asp:Literal ID="ltrCategoryName" runat="server" /></legend>
                   
        <asp:Repeater ID="rptrProducts" OnItemDataBound="rptrProducts_ItemDataBound" runat="server">
            <HeaderTemplate>
            </HeaderTemplate>
            <ItemTemplate>

                <asp:PlaceHolder ID="plcHeading" Visible="false" runat="server">
                    <div class="sectionHeader"><asp:Literal ID="ltrSectionHeadingName" runat="server" /></div>
                    <asp:Literal ID="ltrSectionHeadingContent" runat="server" />
                    <br />
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plcProduct" Visible="false" runat="server">
                    <table id="tblProduct" runat="server" class="item" border="0" cellpadding="0" style="border-width:1px;border-style:solid;border-collapse:collapse;" width="100%">
						<tr>
							<td class="ProductImage" width="20" rowspan="2" style="white-space: nowrap;">
                                <a id="lnkProductImage" runat="server"><asp:Image ID="imgProductImage" width="60" height="55" CssClass="thumbnailImage" runat="server" /></a>

                                <asp:PlaceHolder ID="plcAdditionalImages" Visible="false" runat="server">
                                    <br/><a id="lnkMoreImages" class="AdditionalImagesLink" href="#" runat="server">Additional Images</a>

			                        <p style="display: none;">
                                        <asp:Repeater ID="rptrAdditionalImages" Visible="false" OnItemDataBound="rptrAdditionalImages_ItemDataBound" runat="server">
                                            <ItemTemplate>
                                                <a id="lnkAdditionalImage" rel="" title="" runat="server"><asp:Image id="imgAdditionalImage" class="last" AlternateText="" runat="server" /></a>
                                            </ItemTemplate>
                                        </asp:Repeater>
			                        </p>
                                </asp:PlaceHolder>
                            </td>
							<td width="350" valign="top">

                                <table border="0" width="350" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td class="ProductName" valign="top">
                                            <asp:Literal ID="ltrProductName" Visible="false" runat="server" />
                                            <a id="lnkProductDownload" href="#" visible="false" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ProductPrice" valign="top" style="white-space: nowrap;">
                                            <uc:PricingLabels id="ucPricingLabels" visible="false" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                                
                            </td>
							<td rowspan="2" valign="top" align="right" width="50"><asp:Button ID="btnItemDetail" CssClass="button" Text="Order" OnClick="btnItemDetail_Click" runat="server" /></td>
						</tr>
						<tr>
							<td class="ProductDescription" colspan="2">
                                <asp:PlaceHolder ID="plcSubmissionDeadline" Visible="false" runat="server">
                                    Submission Deadline: <asp:Literal ID="ltrSubmissionDeadline" runat="server" /><br />
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcProductDescription" Visible="false" runat="server">
                                    <asp:Literal ID="ltrProductDescription" runat="server" />
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plcWarningMessage" Visible="false" runat="server">
                                    <br /><br />
                                    <asp:Label ID="lblWarningMessage" CssClass="errorMessage" Visible="false" runat="server" />
                                </asp:PlaceHolder>
							</td>
						</tr>
				    </table>
                </asp:PlaceHolder>
                <br />
            </ItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:Repeater>

				
    </fieldset>