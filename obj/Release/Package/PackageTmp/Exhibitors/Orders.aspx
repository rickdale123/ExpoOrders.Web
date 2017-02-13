<%@ Page Title="" Language="C#" MasterPageFile="~/Exhibitors/ExhibitorsMaster.Master" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="ExpoOrders.Web.Exhibitors.Orders" %>
<%@ MasterType VirtualPath="~/Exhibitors/ExhibitorsMaster.Master" %>

<%@ Register Src="~/CustomControls/OrderDetail.ascx" TagPrefix="uc" TagName="OrderDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

<asp:PlaceHolder ID="plcOrderList" Visible="false" runat="server">

<h1><asp:Literal ID="ltrPageHeading" runat="server" /></h1>

    
        <asp:PlaceHolder ID="plcOrderConfirmationMessage" Visible="false" runat="server">
            <asp:Label ID="lblOrderConfirmationMessage" CssClass="dataValue" runat="server" /><br />
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plcExhibitorButtons" Visible="false" runat="server">
            <asp:Button ID="btnViewExhibitorInvoice" CssClass="button" Text="View Invoice" runat="server" />
            <br /><br />
        </asp:PlaceHolder>
        

        <asp:Repeater ID="rptrExhibitorOrders" OnItemDataBound="rptrExhibitorOrders_ItemDataBound" runat="server">
            <HeaderTemplate>
            </HeaderTemplate>
            <ItemTemplate>
                <table id="tblOrder" runat="server" class="item" border="1" cellspacing="0" cellpadding="2" style="border-width:1px;border-style:solid;border-collapse:collapse;" width="100%">
				    <tr>
					    <td class="colData" width="250" valign="top">
                            <asp:Label ID="lblOrderDate" CssClass="label" runat="server">Order Date:</asp:Label><br />
                            <asp:Label ID="lblOrderDateValue" CssClass="dataValueStrong" runat="server" /><br />

                            <asp:LinkButton ID="lnkBtnOrderDetail" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "OrderId")%>' OnClick="lnkBtnOrderDetail_Click" runat="server">
                                View Order Detail
                            </asp:LinkButton>
                            <asp:PlaceHolder ID="plcViewReceipt" Visible="false" runat="server">
                                &nbsp;|&nbsp;<a id="lnkPrintReceipt" href="#" runat="server">View Order Receipt</a>
                            </asp:PlaceHolder>

                            <br /><br />

                            <asp:Label ID="lblConfirmationNumber" CssClass="labelStrong" runat="server">Order Number:</asp:Label>&nbsp;
                            <asp:LinkButton ID="lnkBtnOrderDetail2" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "OrderId")%>' OnClick="lnkBtnOrderDetail_Click" runat="server">
                                <%#DataBinder.Eval(Container.DataItem, "ConfirmationNumberDisplay")%>
                            </asp:LinkButton>
                            <br />

                            <asp:Label ID="lblUserName" CssClass="labelStrong" runat="server">Placed By:</asp:Label>&nbsp;
                            <asp:Label ID="lblUserNameValue" CssClass="dataValue" runat="server" /><br />

                            <asp:Label ID="lblEmailConfirmation" CssClass="labelStrong" runat="server">Email:</asp:Label>&nbsp;
                            <asp:Label ID="lblEmailConfirmationValue" CssClass="dataValue" runat="server" />
                            
                        </td>
                        <td class="colData" valign="top">
                            <asp:Label ID="lblOrderStatus" CssClass="labelStrong" runat="server">Status:</asp:Label>&nbsp;
                            <asp:Label ID="lblOrderStatusValue" runat="server" />

                            <br />
                            <asp:Label ID="lblOrderTotal" CssClass="labelStrong" runat="server">Order Total:</asp:Label>&nbsp;
                            <asp:Label ID="lblOrderTotalValue" CssClass="dataValue" runat="server" />
                            <br /><br />

                            <asp:Repeater ID="rptrProductOrderItems" OnItemDataBound="rptrProductOrderItems_ItemDataBound" runat="server">
                                <HeaderTemplate>
                                    <asp:Label ID="lblOrderItemsLabel" CssClass="labelStrong" runat="server">Items Ordered:</asp:Label><br />
                                    <table width="100%" border="0" cellspacing="0" cellpadding="2">
                                        <tr>
                                            <td width="65" class="label">Quantity</td>
                                            <td class="label">Item Description</td>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                        <tr>
                                            <td class="colDataCenter"><asp:label ID="lblOrderItemQuantity" CssClass="colData" runat="server" /></td>
                                            <td class="colData"><asp:label ID="lblOrderItemDescriptionValue" CssClass="colData" runat="server" /></td>
                                        </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>

                            <asp:Repeater ID="rptrFormOrderItems" runat="server">
                                <HeaderTemplate></HeaderTemplate>
                                <ItemTemplate><%#DataBinder.Eval(Container.DataItem, "ItemDescription")%><br /></ItemTemplate>
                                <FooterTemplate></FooterTemplate>
                            </asp:Repeater>
                        </td>
					</tr>
				</table>
                <br />
            </ItemTemplate>

            <FooterTemplate>
            </FooterTemplate>
        </asp:Repeater>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plcNoOrders" Visible="false" runat="server">
    No Orders have been processed yet.<br /><br />
    Please check your <a href="ShoppingCart.aspx">Shopping Cart</a> for pending items that you wish to purchase and 'Proceed to Checkout'.
</asp:PlaceHolder>


<asp:PlaceHolder ID="plcOrderDetail" Visible="false" runat="server">

    <h1>Order Detail</h1>

    <uc:OrderDetails id="ucOrderDetails" visible="false" runat="server" />

    <center>
        <asp:Button ID="btnBackToList" Text="Back to List" CssClass="button" OnClick="btnBackToList_Click" runat="server" />
    </center>
    <br />

</asp:PlaceHolder>
</asp:Content>
