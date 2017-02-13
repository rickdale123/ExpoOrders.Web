<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdmin/SuperAdminMaster.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="OwnerConfig.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.OwnerConfig" %>
<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="plcOwnerList" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Owner List</legend>

            <asp:Button ID="btnAddOwner" Text="Add Owner" CssClass="button" OnClick="btnAddOwner_Click" runat="server" />

                <asp:GridView EnableViewState="true"
                        runat="server" ID="grdvwOwnerList" AllowPaging="false" AllowSorting="false"
                        AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="dataRowAlt"
                        RowStyle-CssClass="dataRow" OnRowDataBound="grdvwOwnerList_RowDataBound" OnRowCommand="grdvwOwnerList_RowCommand"
                        EmptyDataText="There are currently no owners." PageSize="200" GridLines="Both">
                        <Columns>
                            <asp:TemplateField HeaderText="Owner Id" HeaderStyle-Wrap="false">
                                <ItemTemplate>
                                    <asp:Literal ID="ltrOwnerId" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "OwnerId").ToString())%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Company Name" HeaderStyle-Width="150px">
                                <ItemTemplate>
                                    <asp:LinkButton Visible="true" ID="lbtnManageOwner" Text='<%# DataBinder.Eval(Container.DataItem, "OwnerName")%>'
                                        CommandName="EditOwner" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OwnerId")%>' runat="server" />
                                    <asp:Literal ID="ltrOwnerName" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Primary Contact">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "PrimaryContactName")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkBtnMerchantAccounts" Text="Merchant Accounts" CommandName="ViewMerchantAccounts" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OwnerId")%>'  runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
        </fieldset>

    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcOwnerDetail" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Owner Detail</legend>

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Owner Id</td>
                    <td><asp:Label ID="lblOwnerId" CssClass="colData" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Company Name</td>
                    <td>
                        <asp:TextBox ID="txtOwnerName" Visible="true" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Primary Contact</td>
                    <td>
                        <asp:TextBox ID="txtPrimaryContactName" Visible="true" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Primary Email</td>
                    <td>
                        <asp:TextBox ID="txtPrimaryContactEmail" Visible="true" Width="200" CssClass="inputText" MaxLength="50" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Address</td>
                    <td>
                        <asp:TextBox ID="txtAddressLine1" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">
                        &nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="txtAddressLine2" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">
                        &nbsp;
                    </td>
                </tr>
                 <tr>
                    <td class="contentLabelRight">
                        &nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="txtAddressLine3" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="contentLabelRight">City</td>
                    <td><asp:TextBox ID="txtCity" Width="150" CssClass="inputText" MaxLength="100" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">
                        State/Province/Region
                    </td>
                    <td>
                        <asp:TextBox ID="txtState" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="contentLabelRight">
                        Postal Code
                    </td>
                    <td>
                        <asp:TextBox ID="txtPostalCode" Width="50" CssClass="inputText" MaxLength="50" runat="server" />
                    </td>
                    <td style="width: 10%">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="contentLabelRight">
                        Country
                    </td>
                    <td>
                        <asp:TextBox ID="txtCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Mgr Logo FileName</td>
                    <td>
                        <asp:TextBox ID="txtLogoFileName" Visible="true" Width="200" CssClass="inputText" MaxLength="50" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                 <tr>
                    <td class="contentLabelRight">Shopping Cart Logo (Full)</td>
                    <td>
                        <asp:TextBox ID="txtShoppingCartFullImage" Visible="true" Width="200" CssClass="inputText" MaxLength="50" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                 <tr>
                    <td class="contentLabelRight">Shopping Cart Logo (Empty)</td>
                    <td>
                        <asp:TextBox ID="txtShoppingCartEmptyImage" Visible="true" Width="200" CssClass="inputText" MaxLength="50" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight" style="white-space: nowrap;">Common Folder Name</td>
                    <td>
                        <asp:TextBox ID="txtCommonFolder" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                        &nbsp;<asp:LinkButton ID="lnkCreateFolder" Text="Create Folder" OnClick="lnkCreateFolder_Click" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Mgr Stylesheet FileName</td>
                    <td>
                        <asp:TextBox ID="txtStyleSheetFileName" Visible="true" Width="200" CssClass="inputText" MaxLength="50" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Owner SubDomain</td>
                    <td>
                        <asp:TextBox ID="txtOwnerSubDomain" Visible="true" Width="200" CssClass="inputText" MaxLength="150" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Smtp Config</td>
                    <td>
                        <asp:TextBox ID="txtSmtpConfigXml" Visible="true" TextMode="MultiLine" Rows="10" Columns="150" CssClass="inputText" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Terms</td>
                    <td>
                        <asp:TextBox ID="txtOwnerTerms" Visible="true" Width="200" CssClass="inputText" MaxLength="50" runat="server" />
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
                    <td class="contentLabelRight">
                        Active Flag
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActiveFlag" Visible="true" Checked="false" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Created On</td>
                    <td class="informational">
                        <asp:Label ID="lblCreatedOn" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Modified On</td>
                    <td class="informational">
                        <asp:Label ID="lblModifiedOn" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
            </table>

            
            <asp:Button ID="btnSaveOwnerDetail" Text="Save" CssClass="button" OnClick="btnSaveOwnerDetail_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Button ID="btnCancelOwnerDetail" Text="Cancel" CssClass="button" OnClick="btnCancelOwnerDetail_Click" runat="server" />

            <asp:HiddenField ID="hdnAddressId" runat="server" />
            
        </fieldset>
    </asp:PlaceHolder>

    
    <asp:PlaceHolder ID="plcMerchantAccountList" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Merchant Accounts</legend>

            <asp:Button ID="btnAddMerchantAccount" Text="Add Merchant Account" CssClass="button" OnClick="btnAddMerchantAccount_Click" runat="server" />
            &nbsp;
            <asp:Button ID="btnCancelMerchantAccountList" Text="Back" CssClass="button" OnClick="btnCancelMerchantAccountList_Click" runat="server" />

            <asp:GridView EnableViewState="true"
                    runat="server" ID="grvwMerchantAccountList" AllowPaging="false" AllowSorting="true"
                    AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="dataRowAlt"
                    RowStyle-CssClass="dataRow" OnRowDataBound="grvwMerchantAccountList_RowDataBound" OnRowCommand="grvwMerchantAccountList_RowCommand"
                    EmptyDataText="There are currently no Merchant Accounts." PageSize="200" GridLines="None">
                    <Columns>
                            
                        <asp:TemplateField HeaderText="Description">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "Description")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkBtnEditMerchantAccount" Text="Edit" CommandName="EditMerchantAccount" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "OwnerId") + ":" + DataBinder.Eval(Container.DataItem, "MerchantAccountConfigId")%>'  runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcMerchantAccountDetail" Visible="false" runat="server">

        <fieldset class="commonControls">
            <legend class="commonControls">Merchant Account Detail</legend>

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Merchant Account Id</td>
                    <td><asp:Label ID="lblMerchantAccountId" CssClass="colData" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Description</td>
                    <td><asp:TextBox ID="txtMerchantAccountDescription" Visible="true" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Payment Gateway</td>
                    <td><asp:DropDownList ID="ddlMerchantAccountPaymentGateway" CssClass="inputText" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Credit Cards Accepted</td>
                    <td><asp:CheckBoxList ID="chkLstMerchantAccountCreditCards" CssClass="inputText" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Config Xml</td>
                    <td><asp:TextBox ID="txtMerchantAccountConfigXml" TextMode="MultiLine" CssClass="inputText" Columns="70" Rows="7" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
            </table>
        </fieldset>
    
        <asp:Button ID="btnSaveMerchantAccountConfig" Text="Save" CssClass="button" OnClick="btnSaveMerchantAccountConfig_Click" runat="server" />
        &nbsp;&nbsp;
        <asp:Button ID="btnCancelMerchantAccountConfig" Text="Cancel" CssClass="button" OnClick="btnCancelSaveMerchantAccount_Click" runat="server" />

        <asp:HiddenField ID="hdnMerchantAccountConfigId" runat="server" />
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcOwnerHostConfigs" Visible="false" runat="server">
    
        <hr />

        Owner Host Configs:<br />
        <fieldset class="commonControls">
        <legend class="commonControls">New Entry</legend>

            UrlHost:<asp:TextBox ID="txtUrlHost" CssClass="inputText" runat="server" />
            OwnerId:<asp:TextBox ID="txtHostConfigOwnerId" CssClass="inputText" runat="server" />
            Css File Name:<asp:TextBox ID="txtCssFileName" CssClass="inputText" runat="server" />
            Company Logo:<asp:TextBox ID="txtCompanyLogo" CssClass="inputText" runat="server" />
            Contact Email:<asp:TextBox ID="txtContactEmail" CssClass="inputText" runat="server" />

            <asp:Button ID="btnAddOwnerHostConfig" Text="Add Owner Host Config" CssClass="button" OnClick="btnAddOwnerHostConfig_Click" runat="server" />

            <hr />
            <asp:GridView EnableViewState="true"
                runat="server" ID="grdvwOwnerHostConfigList" AllowPaging="false" AllowSorting="false"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="dataRowAlt"
                RowStyle-CssClass="dataRow" OnRowDataBound="grdvwOwnerHostConfigList_RowDataBound" OnRowCommand="grdvwOwnerHostConfigList_RowCommand"
                EmptyDataText="There are currently no owner configs." PageSize="200" GridLines="None">
                <Columns>
                    <asp:TemplateField HeaderText="Url Host" >
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "UrlHost")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Owner Id" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "OwnerId")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Css File Name">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "CssFileName")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Company Logo">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "CompanyLogo")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Contact Email">
                        <ItemTemplate>
                         <%# DataBinder.Eval(Container.DataItem, "ContactEmail")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkBtnDeleteOwnerHostConfig" Text="Delete" CommandName="DeleteOwnerHostConfig" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UrlHost")%>'  runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
             </Columns>
            </asp:GridView>

        </fieldset>
    </asp:PlaceHolder>

    <asp:HiddenField ID="hdnOwnerId" runat="server" />
    
</asp:Content>
