<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryIconHeading.ascx.cs" Inherits="ExpoOrders.Web.CustomControls.CategoryIconHeading" %>

<h1><asp:Literal ID="ltrCategoryHeading" runat="server" /></h1>
    <div class="row">
        <asp:Repeater ID="rptrCategoryIcons" OnItemDataBound="rptrCategoryIcons_ItemDataBound" runat="server">
            <HeaderTemplate></HeaderTemplate>
            <ItemTemplate>
                <div class="col-xs-6 col-md-3">
                    
                    <a id="lnkViewCategory" href="#" class="thumbnail" runat="server">
                        <asp:Image ID="imgCategoryIcon" BorderStyle="None" runat="server" />
                        <asp:literal id="ltrCategoryText" runat="server" />
                     </a>
                </div>

            </ItemTemplate>
            <FooterTemplate></FooterTemplate>
        </asp:Repeater>

    </div>
