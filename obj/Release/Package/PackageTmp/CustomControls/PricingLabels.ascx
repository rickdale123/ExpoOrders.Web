<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PricingLabels.ascx.cs" Inherits="ExpoOrders.Web.CustomControls.PricingLabels" %>
<asp:PlaceHolder ID="plcAdvancedPricingSection" Visible="false" runat="server">
    <span class="earlyBirdPricing">
        Advanced Price <asp:Literal ID="ltrAdvancedPriceDeadline" runat="server" />&nbsp;<asp:Literal ID="ltrAdvancedPrice" runat="server" />
    </span>
    <br />
</asp:PlaceHolder>

<asp:PlaceHolder ID="plcStandardPricingSection" Visible="false" runat="server">
    Standard Price <asp:Literal ID="ltrStandardPriceDeadline" runat="server" />&nbsp;<asp:Literal ID="ltrStandardPrice" runat="server" />
    <br />
</asp:PlaceHolder>

<asp:placeholder ID="plcShowFloorPricingSection" Visible="false" runat="server">
    Show Floor Price &nbsp;<asp:Literal ID="ltrShowFloorPrice" runat="server" />
    <br />
</asp:placeholder>