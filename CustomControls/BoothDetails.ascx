<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BoothDetails.ascx.cs" Inherits="ExpoOrders.Web.CustomControls.BoothDetails" %>

<h1><asp:Literal ID="ltrContentHeader" Text="Current Booth Details" runat="server" /></h1>
<fieldset class="commonControls">
    <legend class="commonControls">
        <asp:Literal ID="ltrBoothDescription" runat="server" />
    </legend>

    <asp:Repeater ID="rptrDistinctCategories" OnItemDataBound="rptrDistinctCategories_ItemDataBound" runat="server">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <asp:Label ID="lblCategoryName" CssClass="labelStrong" runat="server"><%#Container.DataItem%></asp:Label><br />
            <table width="100%" border="0" cellspacing="0" cellpadding="2">
                <tr>
                    <td width="65" class="label">Quantity</td>
                    <td class="label">Item Description</td>
                </tr>
                <asp:Repeater ID="rptrDistinctBoothItems" runat="server">
                <ItemTemplate>
                    <tr>
                        <td class="colDataCenter"><%#DataBinder.Eval(Container.DataItem, "Quantity")%></td>
                        <td class="colData"><%#DataBinder.Eval(Container.DataItem, "ItemDescription")%>
                             <%#DataBinder.Eval(Container.DataItem, "RequiredAttribute1")%> 
                             <%#DataBinder.Eval(Container.DataItem, "RequiredAttribute2")%></td>
                    </tr>
                </ItemTemplate>
                </asp:Repeater>
            </table>
        </ItemTemplate>
        <FooterTemplate>
            
        </FooterTemplate>
        
    </asp:Repeater>
    
</fieldset>

