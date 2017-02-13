<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnersMaster.Master" AutoEventWireup="true" EnableEventValidation="false" EnableViewState="true" CodeBehind="ShowExplorer.aspx.cs" ValidateRequest="false" Inherits="ExpoOrders.Web.Owners.ShowExplorer" %>
<%@ MasterType VirtualPath="~/Owners/OwnersMaster.Master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">
<script type="text/javascript">
    
    $(document).ready(function () {
        var allCheckBoxSelector = '#<%=grdvFileList.ClientID%> input[id*="chkAllFiles"]:checkbox';
        var checkBoxSelector = '#<%=grdvFileList.ClientID%> input[id*="chkFile"]:checkbox';

        $(allCheckBoxSelector).click(function () {
            var checkedStatus = this.checked;
            $(checkBoxSelector).each(function () {
                $(this).prop('checked', checkedStatus);
            });
        });

    });

    </script>

    <CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

    <asp:PlaceHolder ID="plcPageDetail" Visible="true" runat="server">

        <asp:PlaceHolder ID="plcShowInformation" runat="server">
            <fieldset class="commonControls">
                <legend class="commonControls">Show / Venue</legend>
                <h3>Show Information</h3>

                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Show Id</td>
                        <td>
                            <asp:label ID="lblShowId" CssClass="colData" runat="server" />
                            &nbsp;
                            <span class="techieInfo">
                                (<asp:literal ID="ltrShowGuid" runat="server" />)
                            </span>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Active?</td>
                        <td>
                            <asp:CheckBox ID="chkActiveFlag" runat="server" />
                            &nbsp;<asp:label ID="lblActiveDescription" CssClass="techieInfo" runat="server" />
                            &nbsp;
                            <a id="activeShowHelp" href="#" onclick="return false;"><img id="Img6" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Display on 'Upcoming Show List'?</td>
                        <td>
                            <asp:CheckBox ID="chkDisplayOnOwnerLanding" runat="server" />
                            &nbsp;<asp:label ID="lblDisplayOnOwnerLanding" CssClass="techieInfo" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Exhibitor Login Url(s):</td>
                        <td>
                            <asp:Repeater ID="rptrExhibitorLoginUrls" OnItemDataBound="rptrExhibitorLoginUrls_ItemDataBound" runat="server">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlnkExhibitorLogin" runat="server" /><br />
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>    
                    <tr>
                        <td class="contentLabelRight">Show Name</td>
                        <td>
                            <asp:TextBox ID="txtShowName" Visible="true" Width="275" CssClass="inputText" MaxLength="100" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShowName" CssClass="errorMessage"
                                ErrorMessage="Show Name is Required" EnableClientScript="false" runat="server"
                                ControlToValidate="txtShowName" ValidationGroup="ShowInformation">Missing Show Name</asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Start Date</td>
                        <td>
                            <asp:TextBox ID="txtStartDate" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                            <cc1:CalendarExtender ID="calExtStartDate" TargetControlID="txtStartDate" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorStartDate" CssClass="errorMessage"
                                ErrorMessage="Start Date is Required" EnableClientScript="false" runat="server"
                                ControlToValidate="txtStartDate" ValidationGroup="ShowInformation">Missing Start Date</asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">End Date</td>
                        <td>
                            <asp:TextBox ID="txtEndDate" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                            <cc1:CalendarExtender ID="calExtEndDate" TargetControlID="txtEndDate" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">Quick Facts File:</td>
                        <td>
                            <a id="lnkQuickFactsFile" target="_blank" runat="server"><asp:Label ID="lblQuickFactsFileName" runat="server" /></a>
                            &nbsp;&nbsp;
                            <asp:Button ID="btnRemoveQuickFactsFile" CssClass="button" Text="Remove File" OnClick="btnRemoveQuickFactsFile_Click" runat="server" />

                            <asp:FileUpload ID="fupUploadQuickFacts" runat="server" />
                            <asp:HiddenField ID="hdnQuickFactsFileName" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>

                    <asp:PlaceHolder ID="plcBoothTypesSupported" Visible="false" runat="server">
                    <tr>
                        <td class="contentLabelRight">Booth Types Supported:</td>
                        <td>
                            <asp:CheckBoxList ID="chkBoothTypesSupported" RepeatColumns="2" RepeatDirection="Vertical" AutoPostBack="false" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>

                    </asp:PlaceHolder>
                </table>
                <br />
                <h3>Venue Information</h3>
                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Venue Name</td>
                        <td>
                            <asp:TextBox ID="txtVenueName" Visible="true" Width="275" CssClass="inputText" MaxLength="100"
                                runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorVenueName" CssClass="errorMessage"
                                ErrorMessage="Venuw Name is Required" EnableClientScript="false" runat="server"
                                ControlToValidate="txtVenueName" ValidationGroup="ShowInformation">Missing Venue Name</asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Address</td>
                        <td>
                            <asp:TextBox ID="txtVenueAddressLine1" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtVenueAddressLine2" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtVenueAddressLine3" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">City</td>
                        <td>
                            <asp:TextBox ID="txtVenueCity" Width="150" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">State/Province/Region</td>
                        <td>
                            <asp:TextBox ID="txtVenueState" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Postal Code</td>
                        <td>
                            <asp:TextBox ID="txtVenuePostalCode" Width="50" CssClass="inputText" MaxLength="20" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Country</td>
                        <td>
                            <asp:TextBox ID="txtVenueCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </table>
                <br />
                <h3>Advance Warehouse Address</h3>
                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Address</td>
                        <td>
                            <asp:TextBox ID="txtWarehouseAddressLine1" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtWarehouseAddressLine2" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtWarehouseAddressLine3" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtWarehouseAddressLine4" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtWarehouseAddressLine5" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">City</td>
                        <td>
                            <asp:TextBox ID="txtWarehouseCity" Width="150" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">State/Province/Region</td>
                        <td>
                            <asp:TextBox ID="txtWarehouseState" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Postal Code</td>
                        <td>
                            <asp:TextBox ID="txtWarehousePostalCode" Width="50" CssClass="inputText" MaxLength="20" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Country</td>
                        <td>
                            <asp:TextBox ID="txtWarehouseCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </table>
                <br />
                <h3>Remit Address</h3>
                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Company Name</td>
                        <td>
                            <asp:TextBox ID="txtCompanyNameOnInvoice" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Address</td>
                        <td>
                            <asp:TextBox ID="txtRemitAddressLine1" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtRemitAddressLine2" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtRemitAddressLine3" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">City</td>
                        <td>
                            <asp:TextBox ID="txtRemitCity" Width="150" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">State/Province/Region</td>
                        <td>
                            <asp:TextBox ID="txtRemitState" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Postal Code</td>
                        <td>
                            <asp:TextBox ID="txtRemitPostalCode" Width="50" CssClass="inputText" MaxLength="20" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Country</td>
                        <td>
                            <asp:TextBox ID="txtRemitCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Phone</td>
                        <td>
                            <asp:TextBox ID="txtRemitPhone" Width="150" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td>
                            &nbsp;
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Additional Remit Line</td>
                        <td>
                            <asp:TextBox ID="txtAdditionalRemitLine" Width="275" CssClass="inputText" MaxLength="100" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </table>
                <br />
                <h3>Shipping Labels</h3>
                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Advance Warehouse Label Title</td>
                        <td>
                            <asp:TextBox ID="txtAdvanceWarehouseLabelText" Width="275" CssClass="inputText" MaxLength="100" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">Direct Show Site Label Title</td>
                        <td>
                            <asp:TextBox ID="txtDirectShowSiteLabelText" Width="275" CssClass="inputText" MaxLength="100" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">Outbound Label Title</td>
                        <td>
                            <asp:TextBox ID="txtOutboundLabelText" Width="275" CssClass="inputText" MaxLength="100" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Company Logo on Shipping Label:</td>
                        <td>
                            <a id="imgShippingLabelLogoLink" target="_blank" runat="server"><asp:Label ID="lblShippingLabelLogoFileName" runat="server" /></a>
                            &nbsp;&nbsp;
                            <asp:Button ID="btnRemoveShippingLabelLogo" CssClass="button" Text="Remove Logo" OnClick="btnRemoveShippingLabelLogo_Click" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Upload New Logo</td>
                        <td>
                            <asp:HiddenField ID="hdnShippingLabelLogoFileName" runat="server" />
                            <asp:FileUpload ID="fupUploadShippingLabelLogo" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="3"><asp:Image ID="imgShippingLabelLogo" width="60" height="55" BorderStyle="None" runat="server" /></td>
                    </tr>
                </table>
                <br />
                <center>
                    <asp:Button ID="btnSaveShowInfo" CssClass="button" Text="Save Show Info" OnClick="btnSaveShowInfo_Click" ValidationGroup="ShowInformation" runat="server" />
                </center>
            </fieldset>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcAssets" runat="server">
            <fieldset class="commonControls">
                <legend class="commonControls">Site Design Files <a id="siteDesignHelp" href="#" onclick="return false;"><img id="Img5" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a></legend>

                <p>
                    Only images and files that are used for the web site design (look and feel) belong in this list. Product images and file downloads should 
                    be placed in the 'Site Content' are, over <a href="ContentExplorer.aspx">here</a> and linked to via the navigation menu.
                </p>
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
                                <asp:Image ID="imgFileImage" width="60" height="60" BorderStyle="None" runat="server" />
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
                        <asp:TemplateField HeaderText="">
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkAllFiles" runat="server" /></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkFile" Checked="false" runat="server" />
                                    <asp:HiddenField ID="hdnFileName" Value='<%# DataBinder.Eval(Container.DataItem, "Name")%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                    
                    </Columns>
                </asp:GridView>

                <asp:Button ID="btnDeleteSelectedFiles" CssClass="button" Text="Delete Selected Files" OnClick="btnDeleteSelectedFiles_Click" runat="server" />
            </fieldset>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plcShowOrderConfig" runat="server">
            <fieldset class="commonControls">
                <legend class="commonControls">Show Configuration</legend>

                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td colspan="3"><h3>Transaction Settings</h3></td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Merchant Account Config</td>
                        <td>
                            <asp:DropDownList ID="ddlMerchantAccountConfigId" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Transaction on Submission</td>
                        <td>
                            <asp:DropDownList ID="ddlSubmissionTransactionTypeCd" CssClass="inputText" runat="server" />
                            &nbsp;
                            <a id="submissionTrxHelp" href="#" onclick="return false;"><img id="Img4" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Allowable Payment Types
                            <a id="paymentTypesHelp" href="#" onclick="return false;"><img id="Img3" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                        </td>
                        <td>
                            <asp:CheckBoxList ID="chkLstAllowablePaymentTypes" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Currency Symbol</td>
                        <td>
                            <asp:TextBox ID="txtCurrencySymbol" Width="50" CssClass="inputText" MaxLength="50" runat="server" />
                            &nbsp;
                            <span class="informational">(Pound = 0163; Euro = 0128) <i>Hold the ALT + [Number]</i></span>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="3"><h3>Email Settings</h3></td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Order 'From' Email</td>
                        <td>
                            <asp:TextBox ID="txtOrderFromEmail" Visible="true" Width="275" CssClass="inputText" MaxLength="100" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">'Reply To' Email <span class="techieInfo">(also 'Contact Us')</span></td>
                        <td>
                            <asp:TextBox ID="txtOrderReplyEmail" Visible="true" Width="275" CssClass="inputText" MaxLength="100" runat="server" />
                            <a id="helpReplyToEmail" href="#" onclick="return false;"><img id="Img1" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Order Notification Email</td>
                        <td>
                            <asp:TextBox ID="txtOrderNotificationEmail" Visible="true" Width="275" CssClass="inputText" MaxLength="100" runat="server" />
                            <a id="helpNotifyEmail" href="#" onclick="return false;"><img id="Img2" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Notifications</td>
                        <td>
                            <asp:CheckBoxList ID="chklstOwnerNotifications" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="3"><h3>Site Content</h3></td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Order Confirmation Message</td>
                        <td>
                            <asp:TextBox ID="txtOrderConfirmationMessage" TextMode="MultiLine" Columns="65" Rows="6" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Terms And Conditions Group Label</td>
                        <td><asp:TextBox ID="txtTermsAndConditionsGroupName" Visible="true" Width="275" CssClass="inputText" MaxLength="100" runat="server" /></td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Terms And Conditions Detail</td>
                        <td>
                            <asp:TextBox ID="txtTermsAndConditions" TextMode="MultiLine" Columns="65" Rows="6" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Exhibitor Must Accept Terms and Conditions?</td>
                        <td>
                            <asp:RadioButtonList ID="rdoTermsAndConditionsRequired" CssClass="inputText" RepeatDirection="Horizontal" runat="server">
                                <asp:ListItem Value="0">No</asp:ListItem>
                                <asp:ListItem Value="1">Yes</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">'I Accept...' Terms and Conditions Checkbox Label</td>
                        <td>
                            <asp:TextBox ID="txtTermsAndConditionsLabel" TextMode="MultiLine" Columns="65" Rows="6" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Display Booth # to exhibitors?</td>
                        <td>
                            <asp:RadioButtonList ID="rdoDisplayBoothNumberLabel" CssClass="inputText" RepeatDirection="Horizontal" runat="server">
                                <asp:ListItem Value="0">No</asp:ListItem>
                                <asp:ListItem Value="1">Yes</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Footer Content</td>
                        <td>
                            <asp:TextBox ID="txtFooterInfo" TextMode="MultiLine" Columns="65" Rows="6" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Login Informational Text</td>
                        <td>
                            <asp:TextBox ID="txtLoginInfoText" TextMode="MultiLine" Columns="65" Rows="6" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Login Page Contact Info</td>
                        <td>
                            <asp:TextBox ID="txtLoginContactInfo" TextMode="MultiLine" Columns="65" Rows="6" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="3"><h3>Invoices and Order Receipts</h3></td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Message on Invoice/Order Receipt</td>
                        <td>
                            <asp:TextBox ID="txtInvoiceMessage" TextMode="MultiLine" Columns="65" Rows="6" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Remit Logo:</td>
                        <td>
                            <a id="imgRemitLogoLink" target="_blank" runat="server"><asp:Label ID="lblRemitLogoFileName" runat="server" /></a>
                            &nbsp;&nbsp;
                            <asp:Button ID="btnRemoveRemitLogo" CssClass="button" Text="Remove Logo" OnClick="btnRemoveRemitLogo_Click" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Upload New Logo</td>
                        <td>
                            <asp:HiddenField ID="hdnRemitLogoFileName" runat="server" />
                            <asp:FileUpload ID="fupUploadRemitLogo" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="3" class="informational">
                            Note: Your company name, "<asp:Label ID="lblOwnerCompanyName" CssClass="strong" runat="server" />", will be used on the Order Receipt 
                            and Invoices, if you choose to omit a Logo.
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3"><asp:Image ID="imgRemitLogo" width="60" height="55" BorderStyle="None" runat="server" /></td>
                    </tr>
                    <tr>
                        <td colspan="3"><h3>Shopping Cart Icons</h3></td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Full Cart</td>
                        <td>
                            <asp:TextBox ID="txtShoppingCartFullImage" Visible="true" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Empty Cart</td>
                        <td>
                            <asp:TextBox ID="txtShoppingCartEmptyImage" Visible="true" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </table>

                <center>
                    <asp:Button ID="btnSaveOrderConfig" CssClass="button" Text="Save"  OnClick="btnSaveOrderConfig_Click" ValidationGroup="OrderConfiguration" runat="server" />
                </center>
            </fieldset>
        </asp:PlaceHolder>
    </asp:PlaceHolder>

    <telerik:RadToolTip runat="server" ID="ttReplyToEmail" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="helpReplyToEmail" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(ttReplyToEmail, "OwnerReplyToEmail")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="ttNotifyEmail" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="helpNotifyEmail" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(ttNotifyEmail, "OwnerNotifyEmail")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="paymentTypesTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="paymentTypesHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(paymentTypesTT, "PaymentTypesHelp")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="submissionTrxTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="submissionTrxHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(submissionTrxTT, "SubmissionTrxHelp")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="siteDesignTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="siteDesignHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(siteDesignTT, "SiteDesignHelp")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="activeShowTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="activeShowHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(activeShowTT, "ActiveShowHelp")%>
    </telerik:RadToolTip>
</asp:Content>
