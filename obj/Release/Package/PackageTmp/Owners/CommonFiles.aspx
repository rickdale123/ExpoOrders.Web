<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnerLanding.Master" AutoEventWireup="true" CodeBehind="CommonFiles.aspx.cs" Inherits="ExpoOrders.Web.Owners.CommonFiles" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Owners/OwnerLanding.Master" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script language="javascript" type="text/javascript">
        function showHideCopyLink(idx) {
            showObject('copyLinkTextBox_' + idx);
            hideObject('copyLinkButton_' + idx);
            showObject('copyLinkInstructions_' + idx);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

<CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

    <asp:PlaceHolder id="plcFileList" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Images &amp; Files</legend>
                    <h3>Upload new file</h3>

                    <table border="0" width="100%" cellpadding="1" cellspacing="0">
                        <tr>
                            <td class="contentLabelRight">Upload File</td>
                            <td>
                                <asp:FileUpload ID="fupUploadFile" runat="server" /><asp:Button ID="btnUploadFile" Text="Upload" CssClass="button" OnClick="btnUploadFile_Click" runat="server" />
                            </td>
                            <td style="width: 10%">&nbsp;</td>
                        </tr>
                    </table>
                    <hr />

                    <h3>File List</h3>
                    <asp:LinkButton ID="lnkbtnRefreshFileList" Text="Refresh List" OnClick="lnkbtnRefreshFileList_Click" runat="server" />
                    <asp:GridView EnableViewState="true" OnRowDataBound="grdvFileList_RowDataBound" OnRowCommand="grdvFileList_RowCommand"
                        runat="server" ID="grdvFileList" AllowPaging="false" AllowSorting="true"
                        AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                        RowStyle-CssClass="item"  GridLines="None"
                        EmptyDataText="No Files to display.">
                        <Columns>
                            <asp:TemplateField HeaderText="Preview">
                                <ItemTemplate>
                                    <asp:Image ID="imgFileImage" width="60" height="60" border="0" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="File Name">
                                <ItemTemplate>
                                    <a id="lnkViewFile" target="_blank" runat="server"><%# DataBinder.Eval(Container.DataItem, "Name")%></a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="File Size" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <asp:Label ID="lblFileSize" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="File Type">
                                <ItemTemplate>
                                    <asp:Label ID="lblFileType" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:LinkButton Visible="true" ID="lbtnDeleteFile" Text="Delete" CommandName="DeleteFile"
                                        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Name")%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Html Link">
                                <ItemTemplate>
                                    <a id='copyLinkButton_<%# DataBinder.Eval(Container, "RowIndex") %>' href="#" class="action-link" onclick='showHideCopyLink(<%# DataBinder.Eval(Container, "RowIndex") %>); return false;'>Create Link</a>
                                    <div class="techieInfo" id='copyLinkInstructions_<%# DataBinder.Eval(Container, "RowIndex") %>' style="display: none;">(Copy/Paste from the Textbox below)</div>
                                    <div id='copyLinkTextBox_<%# DataBinder.Eval(Container, "RowIndex") %>' style="display: none;"><asp:TextBox ID="txtCopyLink" TextMode="MultiLine" CssClass="techieInfo" Columns="55" Rows="4" runat="server" /></div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
            </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder id="plcOwnerConfig" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Site Settings</legend>

            <h3>Manager Site Settings</h3>
            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Company Name</td>
                    <td><asp:Label ID="lblCompanyName" CssClass="colData" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Logo Filename</td>
                    <td>
                        <asp:TextBox ID="txtOwnerLogoFileName" Width="225" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Stylesheet name</td>
                    <td>
                        <asp:TextBox ID="txtOwnerStyleSheet" Width="225" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
             </table>

             <h3>Exhibitor Site Settings</h3>
             <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Shopping Cart Image (Full)</td>
                    <td>
                        <asp:TextBox ID="txtOwnerShoppingCartImageFull" Width="225" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Shopping Cart Image (Empty)</td>
                    <td>
                        <asp:TextBox ID="txtOwnerShoppingCartImageEmpty" Width="225" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Additional Image Link Text</td>
                    <td>
                        <asp:TextBox ID="txtAdditionalImageLinkText" Width="225" CssClass="inputText" MaxLength="50" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
            </table>

            <h3>Classifications (1 per line)</h3>
             <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td>
                        <asp:TextBox ID="txtClassifications" TextMode="MultiLine" Columns="55" Rows="10" CssClass="inputText" runat="server" />
                    </td>
                </tr>
            </table>

            <h3>Welcome Message (On Show Listing Page)</h3>
             <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td>
                        <asp:TextBox ID="txtWelcomeMessageText" TextMode="MultiLine" Columns="55" Rows="3" CssClass="inputText" runat="server" />
                    </td>
                </tr>
            </table>

            <h3>Show Listing Message (On Show Listing Page)</h3>
             <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td>
                        <asp:TextBox ID="txtShowListingInfoText" TextMode="MultiLine" Columns="55" Rows="5" CssClass="inputText" runat="server" />
                    </td>
                </tr>
            </table>

            <h3>Contact Information (on Show Listing Page)</h3>
             <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td>
                        <asp:TextBox ID="txtContactInfoHtml" TextMode="MultiLine" Columns="55" Rows="5" CssClass="inputText" runat="server" />
                    </td>
                </tr>
            </table>

             <h3>Booth Types Supported (1 per line)</h3>
             <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td>
                        <asp:TextBox ID="txtBoothTypeList" TextMode="MultiLine" Columns="55" Rows="7" CssClass="inputText" runat="server" />
                    </td>
                </tr>
            </table>

            <asp:Button ID="btnSaveOwnerConfig" CssClass="button" Text="Save" OnClick="btnSaveOwnerConfig_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Button ID="btnCancelSaveOwnerConfig" CssClass="button" Text="Cancel" OnClick="btnCancelSaveOwnerConfig_Click" runat="server" />
        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder id="plcOwnerHostConfigs" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Landing Page Configuration</legend>

            <asp:GridView EnableViewState="true"
                runat="server" ID="grdvwOwnerHostConfigs" AllowPaging="false" AllowSorting="true"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                RowStyle-CssClass="item" OnRowDataBound="grdvwOwnerHostConfigs_RowDataBound" OnRowCommand="grdvwOwnerHostConfigs_RowCommand"
                EmptyDataText="No rows to display." PageSize="200" GridLines="None">
                <Columns>
                    <asp:TemplateField HeaderText="Host Address">
                        <ItemTemplate>
                            <asp:Label ID="lblHostAddress" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "UrlHost").ToString())%>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="CSS Filename">
                        <ItemTemplate>
                            <asp:TextBox ID="txtOwnerHostCssFileName" Width="225" CssClass="inputText" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "CssFileName").ToString())%>' MaxLength="100" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="'Contact Us' Email">
                        <ItemTemplate>
                            <asp:TextBox ID="txtOwnerHostContactEmail" Width="225" CssClass="inputText" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "ContactEmail").ToString())%>' MaxLength="100" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Button ID="btnSaveOwnerHostConfigs" CssClass="button" Text="Save" OnClick="btnSaveOwnerHostConfigs_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Button ID="btnCancelSaveOwnerHostConfigs" CssClass="button" Text="Cancel" OnClick="btnCancelSaveOwnerHostConfigs_Click" runat="server" />
        </fieldset>
    </asp:PlaceHolder>
</asp:Content>
