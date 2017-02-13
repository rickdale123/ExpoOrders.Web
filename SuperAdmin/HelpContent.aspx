<%@ Page Title="Help Content" Language="C#" MasterPageFile="~/SuperAdmin/SuperAdminMaster.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="HelpContent.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.HelpContent" %>
<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="plcHelpList" Visible="false" runat="server">

        <asp:Button ID="btnAddHelpContent" Text="Add Help" OnClick="btnAddHelpContent_Click" runat="server" />

        <fieldset class="commonControls">
            <legend class="commonControls">Help Content</legend>

            <asp:Label ID="lblHelpListRowCount" Text="" CssClass="techieInfo" runat="server" />
            <asp:GridView EnableViewState="true" OnRowDataBound="grdvHelpList_RowDataBound" OnRowCommand="grdvHelpList_RowCommand"
                runat="server" ID="grdvHelpList" AllowPaging="false" AllowSorting="true"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="dataRowAlt"
                RowStyle-CssClass="dataRow" GridLines="Both"
                EmptyDataText="No Content to display.">
                <Columns>
                    <asp:TemplateField HeaderText="Id">
                        <ItemTemplate>
                            <asp:LinkButton Visible="true" ID="lbtnManageHelp" Text='<%# DataBinder.Eval(Container.DataItem, "HelpId")%>'
                                CommandName="EditHelp" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "HelpId")%>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Title" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:LinkButton Visible="true" ID="lbtnManageHelpText" Text='<%# DataBinder.Eval(Container.DataItem, "HelpTitle")%>'
                                CommandName="EditHelp" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "HelpId")%>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Code">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "HelpCode")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Section">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "HelpSection")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Content">
                        <ItemTemplate>
                           <asp:TextBox ID="txtHelpContent" CssClass="inputText" Text=' <%# DataBinder.Eval(Container.DataItem, "HelpText")%>' TextMode="MultiLine" EnableViewState="true" Rows="5" Columns="75" runat="server"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Sort Order">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "SortOrder")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Active">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "ActiveFlag")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </fieldset>
    </asp:PlaceHolder>


    <asp:PlaceHolder ID="plcHelpDetail" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Help Content</legend>

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Help Id</td>
                    <td><asp:Label ID="lblHelpId" CssClass="colData" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Title</td>
                    <td>
                        <asp:TextBox ID="txtHelpTitle" Visible="true" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Code Value</td>
                    <td>
                        <asp:TextBox ID="txtHelpCode" Visible="true" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Section</td>
                    <td>
                        <asp:TextBox ID="txtHelpSection" Visible="true" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Content</td>
                    <td>
                        <asp:TextBox ID="txtHelpText" TextMode="MultiLine" CssClass="inputText" Columns="95" Rows="17" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Sort Order</td>
                    <td>
                        <asp:TextBox ID="txtSortOrder" Visible="true" Width="55" CssClass="inputText" MaxLength="5" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Active Flag</td>
                    <td>
                        <asp:CheckBox ID="chkActiveFlag" Visible="true" Checked="false" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
            </table>
            
            <asp:Button ID="btnSaveHelpDetail" Text="Save" CssClass="button" OnClick="btnSaveHelpDetail_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Button ID="btnCancelSaveHelp" Text="Cancel" CssClass="button" OnClick="btnCancelSaveHelp_Click" runat="server" />

            <asp:HiddenField ID="hdnHelpId" runat="server" />
            
        </fieldset>
    </asp:PlaceHolder>
</asp:Content>
