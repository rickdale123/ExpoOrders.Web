<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnersMaster.Master" AutoEventWireup="true"
    CodeBehind="Tab1Editor.aspx.cs" Inherits="ExpoOrders.Web.Owners.Tab1Editor" %>

<%@ MasterType VirtualPath="~/Owners/OwnersMaster.Master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">
    <CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />
    <asp:PlaceHolder ID="plcTabEditor" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Tab 1 Editor (Informational Tab) &nbsp;&nbsp;<a id="tab1EditorHelp" href="#" onclick="return false;"><img id="Img5" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a></legend>
            <h3>Tab Information</h3>

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Tab Text</td>
                    <td>
                        <asp:TextBox ID="txtTabLinkText" Visible="true" Width="200" CssClass="inputText" MaxLength="200" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Tab Number</td>
                    <td>
                        <asp:TextBox ID="txtTabNumber" Visible="true" Width="25" CssClass="inputText" MaxLength="2" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Action When Clicked</td>
                    <td>
                        <asp:DropDownList ID="ddlTabNavigationAction" CssClass="inputText" AutoPostBack="true" OnSelectedIndexChanged="ddlTabNavigationAction_Changed" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <asp:PlaceHolder ID="plcTabTarget" runat="server" Visible="false">
                <tr>
                    <td class="contentLabelRight"><asp:Label ID="lblTabTarget" runat="server" /></td>
                    <td>
                        <asp:DropDownList ID="ddlTabTargetHtmlContent" CssClass="inputText" Visible="false" runat="server" />
                        <asp:DropDownList ID="ddlTabTargetNavItem" CssClass="inputText" Visible="false" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                </asp:PlaceHolder>
                <tr>
                    <td class="contentLabelRight">Visible?</td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkTabVisible" Visible="true" Checked="true" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
            </table>
            <br />
            <center>
                <asp:Button ID="btnSaveTab" CssClass="button" Text="Save Tab" ValidationGroup="TabInformation" OnClick="btnSaveTab_Click" runat="server" />
            </center>
            <asp:HiddenField runat="server" ID="hdnTabLinkId" Value="0" />
            <asp:HiddenField runat="server" ID="hdnTabTargetId" Value="" />
        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcNavigationEditor" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Navigation Editor</legend>
            <h3>Navigation Links</h3>

            <asp:GridView EnableViewState="true" OnPageIndexChanging="grdvwNavigationLinkList_PageIndexChanging"
                runat="server" ID="grdvwNavigationLinkList" AllowPaging="false" AllowSorting="true"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                RowStyle-CssClass="item" OnRowDataBound="grdvwNavigationLinkList_RowDataBound" GridLines="None"
                OnRowCommand="grdvwNavigationLinkList_RowCommand" EmptyDataText="There are currently no navigation links associated.">
                <Columns>
                    <asp:TemplateField HeaderText="Link Text">
                        <ItemTemplate>
                            <asp:LinkButton Visible="true" ID="lbtnManageNavLink" ToolTip='<%# "NavLinkId: " + DataBinder.Eval(Container.DataItem, "NavigationLinkId")%>' Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "LinkTextDisplay").ToString())%>' CommandName="EditNavLink"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "NavigationLinkId")%>' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                            <asp:Literal ID="ltrNavigationItemType" Text='' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Action">
                        <ItemTemplate>
                            <asp:Literal ID="ltrAction" Text='' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Target Id">
                        <ItemTemplate>
                            <asp:Literal ID="ltrTargetId" Text='' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Sort Order">
                        <ItemTemplate>
                            <asp:Literal ID="ltrSortOrder" Text='' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Active?">
                        <ItemTemplate>
                            <asp:Literal ID="ltrActive" Text='' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton Visible="false" ID="lbtnDeactivateNavLink" Text="Remove" CommandName="DeactivateNavLink"
                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton Visible="false" ID="lbtnActivateNavLink" Text="Restore" CommandName="ActivateNavLink"
                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <br />
            <asp:Button ID="btnAddNavLink" CssClass="button" Text="Create a link" OnClick="btnAddNavLink_Click" runat="server" />
            &nbsp;
            <asp:Button ID="btnRefresh" CssClass="button" Text="Refresh" OnClick="btnRefresh_Click" runat="server" />
            &nbsp;
            <asp:CheckBox ID="chkIncludeInactive" runat="server" Text="Show Inactive Navigation" Checked="false" />
        </fieldset>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcManageNavigationLink" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Manage Navigation Link</legend>
            <h3>Navigation Information</h3>

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">
                        Parent Link
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlParentLink" Visible="true" CssClass="inputText" runat="server" />
                    </td>
                    <td style="width: 10%">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="contentLabelRight">
                        Link Text
                    </td>
                    <td>
                        <asp:TextBox ID="txtNavigationLinkText" Visible="true" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="contentLabelRight">
                        Item Type
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlItemType" Visible="true" CssClass="inputText" runat="server" />
                    </td>
                    <td style="width: 10%">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="contentLabelRight">
                        Action
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlAction" AutoPostBack="true" OnSelectedIndexChanged="ddlAction_Changed" Visible="true" CssClass="inputText" />
                    </td>
                    <td style="width: 10%">
                        &nbsp;
                    </td>
                </tr>
                <asp:PlaceHolder ID="plcTargetAsset" runat="server" Visible="false">
                    <tr>
                        <td class="contentLabelRight">
                            <asp:Literal runat="server" Text="Target Id" ID="ltrTargetAsset" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlTargetAsset" Visible="true" CssClass="inputText" />
                            <asp:HiddenField ID="hdnNavLinkTargetId" Value="0" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td class="contentLabelRight">
                        Sort Order
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtSortOrder" MaxLength="3" Width="25" Visible="true" CssClass="inputText" />
                    </td>
                    <td style="width: 10%">
                        &nbsp;
                    </td>
                </tr>
            </table>
            <br />
            <center>
                <asp:Button ID="btnSaveNavigationLink" OnClick="btnSaveNavigationLink_Click" CssClass="button" Text="Save Link" ValidationGroup="NavigationLinkInformation" runat="server" />
                &nbsp;&nbsp;
                 <asp:Button ID="btnReturnToList" OnClick="btnReturnToNavList_Click" CssClass="button" Text="Return To List" runat="server" />
            </center>
            <asp:HiddenField runat="server" ID="hdnNavigationLinkId" Value="0" />
        </fieldset>
    </asp:PlaceHolder>

    <telerik:RadToolTip runat="server" ID="tab1EditorTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="tab1EditorHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(tab1EditorTT, "Tab1EditorHelp")%>
    </telerik:RadToolTip>
</asp:Content>
