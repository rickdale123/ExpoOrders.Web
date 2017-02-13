<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnersMaster.Master" AutoEventWireup="true" CodeBehind="Exhibitors.aspx.cs" Inherits="ExpoOrders.Web.Owners.Exhibitors" %>

<%@ MasterType VirtualPath="~/Owners/OwnersMaster.Master" %>
<%@ Register Src="~/CustomControls/UserDetail.ascx" TagPrefix="uc" TagName="UserDetail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">
    

    <script type="text/javascript">
        //<![CDATA[
        function launchExhibitorAttachments(exhibitorId) {
            var oWnd = radopen("ExhibitorAttachments.aspx?exhibitorId=" + exhibitorId, "ExhibitorAttachments").setSize(500, 400);
        }

        function OnClientClose(oWnd, args) {
            //get the transferred arguments
            var arg = args.get_argument();

            if (arg) {
                var exhibitorId = arg.exhibitorId;
            }
        }

        
        $(document).ready(function(){
            var allCheckBoxSelector = '#<%=grdvwExhibitorList.ClientID%> input[id*="chkAll"]:checkbox';
            var checkBoxSelector = '#<%=grdvwExhibitorList.ClientID%> input[id*="chkSelected"]:checkbox';

            $(allCheckBoxSelector).click (function () {
                var checkedStatus = this.checked;
                $(checkBoxSelector).each(function () {
                    $(this).prop('checked', checkedStatus);
                });
            });

            var allPrimaryUsersCheckBox = '#<%=grdvwExhibitorUserList.ClientID%> input[id*="chkPrimaryFlag"]:checkbox';

            $(allPrimaryUsersCheckBox).on('change', function () {
                $(allPrimaryUsersCheckBox).not(this).prop('checked', false);
            });
         });
            
        //]]>
    </script>

    <telerik:RadWindowManager ID="RadWindowManager1" ShowContentDuringLoad="false" VisibleStatusbar="false" ReloadOnShow="true" runat="server" EnableShadow="true">
        <Windows>
            <telerik:RadWindow ID="modalExhibitorAttachments" runat="server" Behaviors="Close" OnClientClose="OnClientClose" NavigateUrl="ExhibitorAttachments.aspx">

            </telerik:RadWindow>
        
        </Windows>
    </telerik:RadWindowManager>
    <CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />
    <asp:PlaceHolder ID="plcExhibitorUpload" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Exhibitor Upload</legend>
            <h3>Upload File Details</h3>

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">
                        Select Exhibitor File (csv)
                    </td>
                    <td>
                        <asp:FileUpload ID="fupExhibitorFile" runat="server" /><asp:Button ID="btnUploadFile" Text="Upload File" CssClass="button" OnClick="btnUploadFile_Click" runat="server" />
                        &nbsp;
                        <a id="exhibitorUploadHelp" href="#" onclick="return false;"><img id="Img4" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="3">
                        <br />
                        <a href="../Templates/ExhibitorLoad_Template.csv?v=2" target="_blank">Download CSV Template&nbsp;&nbsp;</a>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td colspan="3"><asp:Literal ID="ltrUploadNote" runat="server" /></td>
                </tr>
            </table>
            <asp:PlaceHolder ID="plcExhibitorUploadResults" Visible="false" runat="server">
                <asp:GridView EnableViewState="true" OnRowDataBound="grdvExhibitorUploadResults_RowDataBound"
                    runat="server" ID="grdvExhibitorUploadResults" AllowPaging="false" AllowSorting="true"
                    AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                    RowStyle-CssClass="item" EmptyDataText="No exhibitors Uploaded." GridLines="None">
                    <Columns>
                        <asp:TemplateField HeaderText="Booth Number">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "BoothNumber")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Username">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "UserName")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Person Name">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "ExhibitorName")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Company Name">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "CompanyName")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Primary Email">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "PrimaryEmail")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Import Results">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "ImportResults")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:PlaceHolder>
        </fieldset>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcExhibitorList" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Exhibitors List</legend>

                <table class="searchTable" border="0" cellpadding="1" cellspacing="1">
                    <tr>
                        <td class="searchOption" valign="top">
                            <b>Search On:</b><br />
                            <table border="0" width="100%" cellpadding="2" cellspacing="1">
                                <tr>
                                    <td style="text-align: right; white-space: nowrap;">Company Name</td>
                                    <td>
                                        <telerik:RadComboBox ID="cboSearchExhibitorId" MarkFirstMatch="true" AllowCustomText="false" Width="350" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right; white-space: nowrap;">Invoice # (Exb. Id)</td>
                                    <td><asp:TextBox ID="txtSearchExhibitorId" CssClass="inputText" Width="100" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td style="text-align: right; white-space: nowrap;">Booth #</td>
                                    <td><asp:TextBox ID="txtSearchBoothNumber" CssClass="inputText" Width="100" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td style="text-align: right; white-space: nowrap;">Email Address</td>
                                    <td><asp:TextBox ID="txtSearchPrimaryEmailAddress" CssClass="inputText" Width="275" runat="server" /></td>
                                </tr>
                                <asp:PlaceHolder ID="plcExhibitorClassificationSearch" Visible="false" runat="server">
                                    <tr>
                                        <td style="text-align: right; white-space:nowrap;">Classification</td>
                                        <td>
                                            <telerik:RadComboBox ID="cboExhibitorClassification" MarkFirstMatch="true" AllowCustomText="true" runat="server" />
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                                <tr>
                                    <td colspan="2">
                                        <asp:RadioButton ID="rbtnAllExhibitorOrders" Text="All Exhibitors," GroupName="ExhibitorOrders" Checked="true" runat="server" />
                                        or Exhibitors that&nbsp;
                                        <asp:RadioButton ID="rbtnOnlyExhibitorsWithOrders" Text="have ordered something" GroupName="ExhibitorOrders" runat="server" />
                                        <asp:RadioButton ID="rbtnOnlyExhibitorsWithNoOrders" Text="have not placed an order yet" GroupName="ExhibitorOrders" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:RadioButton ID="rbtnAllExhibitorCreditCards" Text="All Exhibitors," GroupName="ExhibitorCreditCards" Checked="true" runat="server" />
                                        or Exhibitors that&nbsp;
                                        <asp:RadioButton ID="rbtnOnlyExhibitorsWithNoCreditCards" Text="have no credit card on file" GroupName="ExhibitorCreditCards" runat="server" />
                                        <asp:RadioButton ID="rbtnOnlyExhibitorsWithCreditCards" Text="that have a credit card on file" GroupName="ExhibitorCreditCards" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right; white-space: nowrap;">Include Inactive?</td>
                                    <td><asp:checkbox id="chkSearchIncludeInactive" runat="server" /> </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="text-align: right;">
                                        <asp:Button ID="btnSearch" Text="Search" CssClass="button" OnClick="btnSearch_Click" runat="server" />
                                        &nbsp;
                                        <asp:Button ID="btnClearSearch" Text="Reset" CssClass="button" OnClick="btnClearSearch_Click" runat="server" />
                                        &nbsp;
                                        <asp:Button ID="btnAddExhibitor" CssClass="button" Text="Create Exhibitor" OnClick="btnAddExhibitor_Click" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

                <asp:PlaceHolder ID="plcExhibitorDisplay" Visible="false" runat="server">

                    <asp:Label ID="lblExhibitorListRowCount" Text="" CssClass="techieInfo" runat="server" />

                    <div style="text-align: left;">
                        <br />
                        <asp:Button ID="btnSendWelcomeKits" runat="server" Visible="true" OnClick="btnSendWelcomeKits_Click" Text="Send Welcome Kit(s)" CssClass="button" />
                        &nbsp;
                        <asp:Button ID="btnSendEmails" runat="server" Visible="true" OnClick="btnSendEmails_Click" Text="Send Email" CssClass="button" />
                        &nbsp;
                        <asp:Button ID="btnSendInvoices" runat="server" Visible="true" OnClick="btnSendInvoices_Click" Text="Send Invoices" CssClass="button" />
                        &nbsp;
                        <asp:Button ID="btnCreateExhibitor" runat="server" Visible="true" OnClick="btnCreateNewExhbitor_Click" Text="Create Exhibitor" CssClass="button" />
                        &nbsp;
                    <asp:Button ID="btnSaveExhibitorBooths1" runat="server" Visible="true" OnClick="btnSaveExhibitorList_Click" Text="Update Booth #s" CssClass="button" />
                    </div>
                    <br />
            

                    <asp:GridView EnableViewState="true"
                        runat="server" ID="grdvwExhibitorList" AllowPaging="false" AllowSorting="true"
                        AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                        RowStyle-CssClass="item" OnRowDataBound="grdvwExhibitorList_RowDataBound" OnRowCommand="grdvwExhibitorList_RowCommand"
                        OnSorting="grdvwExhibitorList_Sorting"
                        EmptyDataText="No Exhibitors to display." PageSize="200" GridLines="None">
                        <Columns>
                            <asp:TemplateField HeaderText="" HeaderStyle-Wrap="false">
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkAll" runat="server" /></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelected" runat="server" /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Wrap="false" SortExpression="ExhibitorId">
                                <HeaderTemplate>
                                    <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="ExhibitorId">Inv# (ID)</asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblExhibitorId" Text='<%# DataBinder.Eval(Container.DataItem, "ExhibitorId")%>' runat="server" />
                                    <asp:HiddenField ID="hdnExhibitorId" Value='<%# DataBinder.Eval(Container.DataItem, "ExhibitorId")%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Wrap="false" HeaderStyle-Width="150px" SortExpression="ExhibitorCompanyNameDisplay">
                                <HeaderTemplate>
                                    <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="ExhibitorCompanyNameDisplay">Company Name</asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:LinkButton Visible="true" ID="lbtnManageExhibitor" Text='<%# DataBinder.Eval(Container.DataItem, "ExhibitorCompanyNameDisplay")%>'
                                        ToolTip='<%# "ExhibitorId: " + DataBinder.Eval(Container.DataItem, "ExhibitorId")%>'
                                        CommandName="EditExhibitor" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ExhibitorId")%>' runat="server" />
                                    <asp:Literal ID="ltrExhibitorName" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Wrap="false" SortExpression="PrimaryContactName">
                                <HeaderTemplate>
                                    <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="PrimaryContactName">Contact Info</asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "PrimaryContactName")%><br />
                                    <a id="lnkMailToExhibitor" class="mailTo" runat="server"></a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Wrap="false" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center" SortExpression="BoothNumber">
                                <HeaderTemplate>
                                    <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="BoothNumber">Booth #</asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox id="txtBoothNumber" Width="75" Text='<%# DataBinder.Eval(Container.DataItem, "BoothNumber")%>' CssClass="inputText" runat="server" /><br />
                                    <asp:TextBox id="txtBoothDescription" Width="100" Text='<%# DataBinder.Eval(Container.DataItem, "BoothDescription")%>' CssClass="inputText" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Center" HeaderText="Active?" SortExpression="ActiveFlag">
                                <HeaderTemplate>
                                    <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="ActiveFlag">Active?</asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Literal ID="ltrActive" runat="server" /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Wrap="false" HeaderText="Invited?" SortExpression="InvitedFlag">
                                <HeaderTemplate>
                                    <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="InvitedFlag">Invited?</asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Literal ID="ltrInvited" runat="server" /></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                                <ItemTemplate>
                                    <asp:LinkButton Visible="false" ID="lbtnShowUserList" Text="Users" CommandName="ShowUserList" class="action-link" runat="server" />
                                       &nbsp;|&nbsp;
                                       </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center" HeaderStyle-Wrap="false">
                                <ItemTemplate>
                                    <asp:LinkButton Visible="true" ID="lbtnCallLog" Text="Call Log" class="action-link" CommandName="ViewCallLog" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ExhibitorId")%>' runat="server" />
                                    |
                                    <asp:LinkButton Visible="true" ID="lbtnEmailLog" Text="Email" class="action-link" CommandName="ViewEmailLog" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ExhibitorId")%>' runat="server" />
                                    <br />
                                    <a id="lnkExhibitorAttachments" href="#" class="action-link" runat="server">Attachments</a>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <br />
                    <asp:Button ID="btnSaveExhibitorBooths2" OnClick="btnSaveExhibitorList_Click" Text="Update Booth #s" CssClass="button" runat="server" />
                 </asp:PlaceHolder>
        </fieldset>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcManageExhibitor" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Manage Exhibitor</legend>

            <asp:PlaceHolder ID="plcExhibitorActionButtons" Visible="false" runat="server">
                <div>
                    <asp:Button ID="btnSendWelcomeKit" OnClick="btnSendWelcomeKit_Click" Text="Send Welcome Kit" CssClass="button" runat="server" />
                    &nbsp;
                    <asp:Button ID="btnSendInvoice" OnClick="btnSendInvoice_Click" Text="Send Invoice" CssClass="button" runat="server" />
                    &nbsp;
                    <asp:Button ID="btnSendPasswordReminder" OnClick="btnSendPasswordReminder_Click" Text="Send Pwd Reminder" CssClass="button" runat="server" />
                    &nbsp;
                    <asp:Button ID="btnSendEmail" OnClick="btnSendEmail_Click" Text="Send Email" CssClass="button" runat="server" />
                    &nbsp;
                    <asp:Button ID="btnCreateNewExhibitor2" runat="server" Visible="true" OnClick="btnCreateNewExhbitor_Click" Text="Create Exhibitor" CssClass="button" />
                </div>
            </asp:PlaceHolder>

            <h3>Profile Information</h3>
            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Exhibitor Id</td>
                    <td><asp:Label ID="lblExhibitorId" CssClass="techieInfo" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">*Company Name</td>
                    <td>
                        <asp:TextBox ID="txtCompanyName" Visible="true" Width="225" CssClass="inputText" MaxLength="100" runat="server" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" CssClass="errorMessage"
                            ErrorMessage="Company Name is Required" EnableClientScript="false" runat="server"
                            ControlToValidate="txtCompanyName" ValidationGroup="ProfileInformation">Missing Company Name</asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Booth Number</td>
                    <td>
                        <asp:TextBox ID="txtBoothNumber" Visible="true" Width="225" CssClass="inputText" MaxLength="50" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Booth Size</td>
                    <td>
                        <asp:TextBox ID="txtBoothDescription" Visible="true" Width="225" CssClass="inputText" MaxLength="50" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Booth Notes</td>
                    <td>
                        <asp:TextBox ID="txtBoothNotes" Visible="true" TextMode="MultiLine" MaxLength="250" Columns="30" Rows="2" CssClass="inputText" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Address</td>
                    <td>
                        <asp:TextBox ID="txtAddressLine1" Width="225" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">&nbsp;</td>
                    <td>
                        <asp:TextBox ID="txtAddressLine2" Width="225" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">City</td>
                    <td>
                        <asp:TextBox ID="txtCity" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">State/Province/Region</td>
                    <td>
                        <asp:TextBox ID="txtState" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Postal Code</td>
                    <td>
                        <asp:TextBox ID="txtPostalCode" Width="50" CssClass="inputText" MaxLength="50" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Country</td>
                    <td>
                        <asp:TextBox ID="txtCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Phone</td>
                    <td>
                        <asp:TextBox ID="txtCompanyPhone" Width="175" CssClass="inputText" MaxLength="50" runat="server" /><span class="informational">(xxx) xxx-xxxx ext. xxxxx</span>
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">*Primary Email</td>
                    <td>
                        <asp:TextBox ID="txtCompanyEmailAddress" Width="200" CssClass="inputText" MaxLength="150" runat="server" />
                        <asp:RequiredFieldValidator ID="reqCompanyEmailAddress" CssClass="errorMessage"
                            ErrorMessage="Primary Email is Required" EnableClientScript="false" runat="server"
                            ControlToValidate="txtCompanyEmailAddress" ValidationGroup="ProfileInformation">Missing Primary Email</asp:RequiredFieldValidator>
                        &nbsp;
                        <a id="primaryEmailHelp" href="#" onclick="return false;"><img id="Img1" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">
                        Tax Exempt?
                    </td>
                    <td>
                        <asp:CheckBox ID="chkTaxExempt" Visible="true" Checked="false" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">
                        Allow Special Checkout<br />
                        (No Payment Type)?
                    </td>
                    <td>
                        <asp:CheckBox ID="chkAllowSpecialCheckout" Visible="true" Checked="false" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                 <tr>
                    <td class="contentLabelRight">Active?</td>
                    <td>
                        <asp:RadioButtonList ID="rdoListActiveFlag" CssClass="inputText" RepeatDirection="Horizontal" runat="server">
                            <asp:ListItem Value="0">No, Deleted</asp:ListItem>
                            <asp:ListItem Value="1">Yes, Active</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <asp:PlaceHolder ID="plcClassification" Visible="false" runat="server">
                    <tr>
                        <td class="contentLabelRight">Classification</td>
                        <td><telerik:RadComboBox ID="ddlClassification" MarkFirstMatch="true" AllowCustomText="false" Visible="false" runat="server" /></td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td class="contentLabelRight">
                        Invoice Notes<br />(displayed on invoice)
                   </td>
                    <td>
                        <asp:TextBox ID="txtExternalNotes" Visible="true" TextMode="MultiLine" Columns="50" Rows="3" CssClass="inputText" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="3" valign="top">
                        <fieldset class="commonControls">
                            <legend class="commonControls">Primary User Info</legend>
                            <uc:UserDetail ID="ucPrimaryUserDetail" runat="server" />
                        </fieldset>        
                    </td>
                </tr>
                <tr>
                    <td colspan="3"><hr /></td>
                </tr>
                <tr>
                    <td colspan="3" class="techieInfo">
                        Interal Notes (non-exhibitor facing)<br />
                        <asp:TextBox ID="txtInternalNotes" Visible="true" TextMode="MultiLine" Columns="75" Rows="5" CssClass="techieInfo" runat="server" />
                    </td>
                </tr>
            </table>
            
            <br />
            <center>
                <asp:Button ID="btnSaveExhibitor" CssClass="button" Text="Save" ValidationGroup="ProfileInformation" OnClick="btnSaveExhibitor_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnSaveExhibitorReturn" CssClass="button" Text="Save &amp; Return" ValidationGroup="ProfileInformation" OnClick="btnSaveExhibitorReturn_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnSaveCreateNew" CssClass="button" Text="Save &amp; Create New" ValidationGroup="ProfileInformation" OnClick="btnSaveCreateExhibitorReturn_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnCancelSaveExhibitor" CssClass="button" Text="Cancel &amp; Return" OnClick="btnCancelSaveExhibitor_Click" runat="server" />

                <br /><br />
                <asp:PlaceHolder ID="plcExhibitorNavLinks" Visible="false" runat="server">
                    <asp:LinkButton ID="lbtnShowUsers" OnClick="lbtnShowUsers_Click" Text="Additional User List" class="action-link" runat="server" />
                    |
                    <asp:LinkButton ID="lbtnShowAddresses" OnClick="lbtnManageAddresses_Click" Text="Outbound Addresses" class="action-link" runat="server" />
                    |
                    <a id="lnkExhibitorAttachments" href="#" class="action-link" runat="server">File Attachments</a><br />

                    <asp:LinkButton ID="lbtnManageCreditCards" OnClick="lbtnManageCreditCards_Click" Text="Manage Credit Cards" class="action-link" runat="server" />
                    |
                    <asp:LinkButton ID="lbtnViewPayments" OnClick="lbtnViewPayments_Click" Text="View Payments" class="action-link" runat="server" />
                    |
                    <asp:LinkButton ID="lbtnViewOrders" OnClick="lbtnViewOrders_Click" Text="View Orders" class="action-link" runat="server" />
                    |
                    <asp:LinkButton ID="lbtnViewCallLogs" OnClick="lbtnViewCallLogs_Click" Text="Call Log" class="action-link" runat="server" />
                    |
                    <asp:LinkButton ID="lbtnViewEmailLogs" OnClick="lbtnViewEmailLogs_Click" Text="Email Log" class="action-link" runat="server" />
                    
                </asp:PlaceHolder>
            </center>
            <asp:HiddenField runat="server" ID="hdnExhibitorId" Value="0" />
            <asp:HiddenField runat="server" ID="hdnExhibitorAddressId" Value="0" />
        </fieldset>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcExhibitorUserList" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">User List for Exhibitor:
                <asp:LinkButton ID="lbtnExhibitorUserList" OnClick="lbtnExhibitorUserList_Click" runat="server" /></legend>
            <asp:GridView EnableViewState="true"
                runat="server" ID="grdvwExhibitorUserList" AllowPaging="false" AllowSorting="true"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                RowStyle-CssClass="item" OnRowDataBound="grdvwExhibitorUserList_RowDataBound" GridLines="None"
                OnRowCommand="grdvwExhibitorUserList_RowCommand" EmptyDataText="No users to display.">
                <Columns>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Username">
                        <ItemTemplate>
                               <asp:LinkButton Visible="true" ID="lbtnManageUser2" Text="Edit" CommandName="ManageUser" runat="server" />
                                <asp:HiddenField ID="hdnUserId" Value='<%# DataBinder.Eval(Container.DataItem, "UserId")%>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="First Name">
                        <ItemTemplate>
                            <asp:Literal ID="ltrFirstName" Text="" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Last Name">
                        <ItemTemplate>
                            <asp:Literal ID="ltrLastName" Text="" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Email">
                        <ItemTemplate>
                            <asp:Literal ID="ltrUserEmail" Text="" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Primary?">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkPrimaryFlag" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:LinkButton Visible="false" ID="lbtnManageUser" Text="Edit" CommandName="ManageUser" runat="server" />
                                &nbsp;|&nbsp;
                            <asp:LinkButton Visible="true" ID="lbtnEmailLog" Text="Email Log" CommandName="ViewEmailLog" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserId")%>' runat="server" />

                            <asp:PlaceHolder ID="plcUserActivation" Visible="false" runat="server">
                                &nbsp;|&nbsp;
                                <asp:LinkButton Visible="true" ID="lbtnDeactivateUser" Text="Remove" CommandName="DeactivateUser" runat="server" />
                                <asp:LinkButton Visible="false" ID="lbtnActivateUser" Text="Activate" CommandName="ActivateUser" runat="server" />
                            </asp:PlaceHolder>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <br />
            <asp:Button ID="btnAddUser" CssClass="button" Text="Create User" OnClick="btnAddUser_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Button ID="btnUserListRefresh" CssClass="button" Text="Refresh List" OnClick="btnUserListRefresh_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Button ID="btnCancelManageUsers" CssClass="button" Text="Cancel &amp; Return" OnClick="btnCancelManageUsers_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Button ID="btnSaveUsers" CssClass="button" Text="Save List" OnClick="btnSaveUsers_Click" runat="server" />
        </fieldset>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcUserInformation" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">User Information</legend>
            <h3>User Information</h3>

            <uc:UserDetail ID="ucUserDetail" runat="server" />
            <br />
            <center>
                <asp:Button ID="btnSaveUser" CssClass="button" Text="Save User" ValidationGroup="UserInformation" OnClick="btnSaveUser_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnCancelSaveUser" CssClass="button" Text="Cancel &amp; Return" OnClick="btnCancelSaveUser_Click" runat="server" />
            </center>
        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder id="plcManageCreditCards" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Credit Cards</legend>

            <asp:PlaceHolder ID="plcCreditCardList" runat="server">
                <asp:Repeater ID="rptCreditCardList" runat="server" OnItemDataBound="rptCreditCardList_ItemDataBound" OnItemCommand="btnEdit_ItemCommand">
                    <HeaderTemplate>
                        <h3>Existing Credit Cards on File</h3>
                        <table width="100%" border="0" CellPadding="2" CellSpacing="0">
                            <tr class="Item">
                                <td class="colHeader">Name on Card</td>
                                <td class="colHeader">Card Number</td>
                                <td class="colHeader">Expiration Date</td>
                                <td class="colHeader">Email Address</td>
                                <td>&nbsp;</td>
                                <td width="10%">&nbsp;</td>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                         <tr id="trCreditCard" class="Item" runat="server">
                                <td><asp:Linkbutton commandname="EditCard" ID="lnkEditCardName" runat="server" Text="Edit" CssClass="action-link" /></td>
                                <td><asp:Literal id="ltrCreditCardNumber" Text="" runat="server" /></td>
                                <td><asp:Literal id="ltrCreditCardExpirationDate" Text="" runat="server" /></td>
                                <td><asp:Literal id="ltrCreditCardEmailAddress" Text="" runat="server" /></td>
                                <td style="white-space: nowrap;">
                                    <asp:Linkbutton commandname="EditCard" ID="btnEditCard" runat="server" Text="Edit" CssClass="action-link" />
                                    |
                                    <asp:LinkButton commandname="DeleteCard" id="btnDeleteCard" runat="server" Text="Delete" CssClass="action-link" />
                                </td>
                                <td width="10%">&nbsp;</td>
                          </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
        
            <asp:Button ID="btnAddCreditCard" CssClass="button" Text="Add Credit Card" OnClick="btnAddCreditCard_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Button ID="btnCancelCreditCardDetail" CssClass="button" Text="Cancel &amp; Return" OnClick="btnCancelCreditCardDetail_Click" runat="server" />

            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plcCreditCardDetail" Visible="false" runat="server">
            
                <h3>Credit Card Information</h3>
                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Name on Card</td>
                        <td>
                            <asp:TextBox ID="txtCreditCardName" Width="200" CssClass="inputText" MaxLength="50" runat="server" />

                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="errorMessage" EnableClientScript="false" ErrorMessage="Name is Required" runat="server" ControlToValidate="txtCreditCardName" ValidationGroup="CreditCardInfo">Missing Name</asp:RequiredFieldValidator>
                        </td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">Card Type</td>
                        <td>
                            <asp:DropDownList ID="ddlCreditCardType" CssClass="inputText" runat="server" />

                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="errorMessage" ErrorMessage="Card Type is Required" EnableClientScript="false" runat="server" ControlToValidate="ddlCreditCardType" ValidationGroup="CreditCardInfo">Missing Card Type</asp:RequiredFieldValidator>
                        </td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">Card Number</td>
                        <td>
                            <input type="hidden" id="hdnCreditCardId" runat="server" />
                            <input type="hidden" id="hdnCreditCardAddressId" runat="server" />
                            <asp:TextBox ID="txtCreditCardNumber" Width="200" CssClass="inputText" MaxLength="50" runat="server" />

                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" CssClass="errorMessage" ErrorMessage="Card Type is Required"  EnableClientScript="false" runat="server" ControlToValidate="txtCreditCardNumber" ValidationGroup="CreditCardInfo">Missing Card Type</asp:RequiredFieldValidator>
                        </td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">Expiration Date</td>
                        <td>
                            <asp:DropDownList ID="ddlCreditCardExpMonth" CssClass="inputText" runat="server" />&nbsp;/&nbsp;<asp:DropDownList ID="ddlCreditCardExpYear" CssClass="inputText" runat="server" />
                        
                            <asp:CustomValidator ID="customCreditCardExpDateValidator" CssClass="errorMessage" ValidationGroup="CreditCardInfo" EnableClientScript="false" runat="server"></asp:CustomValidator>
                            &nbsp;<asp:Label ID="Label7" CssClass="contentLabel" runat="server">Security Code</asp:Label>
                            <asp:TextBox ID="txtCreditCardSecurityCode" Width="60" CssClass="inputText" MaxLength="10" runat="server" /> 

                        </td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                
                    <tr>
                        <td class="contentLabelRight">Address</td>
                        <td><asp:TextBox ID="txtCreditCardAddressLine1" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td><asp:TextBox ID="txtCreditCardAddressLine2" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">City</td>
                        <td><asp:TextBox ID="txtCreditCardCity" Width="125" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">State/Province/Region</td>
                        <td><asp:TextBox ID="txtCreditCardState" Width="150" CssClass="inputText" MaxLength="20" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Postal Code</td>
                        <td><asp:TextBox ID="txtCreditCardPostalCode" Width="50" CssClass="inputText" MaxLength="20" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Country</td>
                        <td><asp:TextBox ID="txtCreditCardCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">Email Address</td>
                        <td><asp:TextBox ID="txtCreditCardEmail" Width="200" CssClass="inputText" MaxLength="150" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                </table>
                <br />
                <center>
                    <asp:Button ID="btnSaveCreditCard" CssClass="button" Text="Save Credit Card" OnClick="btnSaveCreditCard_Click" ValidationGroup="CreditCardInfo" runat="server" />
                    &nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnCancelCreditCard" CssClass="button" Text="Cancel/Back to List" OnClick="btnCancelCreditCard_Click" runat="server" />
                </center>
            </asp:PlaceHolder>
        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcManageAddresses" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Exhibitor Addresses</legend>
            <asp:PlaceHolder ID="plcAddressList" Visible="false" runat="server">
                <asp:GridView EnableViewState="true"
                    runat="server" ID="grdvwAddressList" AllowPaging="false" AllowSorting="true"
                    AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                    RowStyle-CssClass="item" OnRowDataBound="grdvwAddressList_RowDataBound" GridLines="None"
                    OnRowCommand="grdvwAddressList_RowCommand" EmptyDataText="No Addresses to display.">
                    <Columns>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="">
                            <ItemTemplate>
                               
                                <asp:Label ID="lblAddressType" CssClass="techieInfo" runat="server" />
                                <br />
                                <asp:Literal ID="ltrOtherFullAddress" runat="server" />
                                <br />
                                <asp:LinkButton Visible="true" ID="lbtnManageAddress" Text="Edit" CssClass="action-link" CommandName="ManageAddress" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressId")%>' runat="server" />
                                    &nbsp;|&nbsp;
                                <asp:LinkButton Visible="true" ID="lbtnDeleteAddress" Text="Delete" CssClass="action-link" CommandName="DeleteAddress" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressId")%>' runat="server" />
                                <hr />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <br />
                <asp:Button ID="btnAddAddress" CssClass="button" Text="Create Address" OnClick="btnAddAddress_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnAddressList" CssClass="button" Text="Refresh List" OnClick="btnAddressListRefresh_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnCancelManageAddress" CssClass="button" Text="Cancel/Back to Exhibitor Detail" OnClick="btnCancelManageAddress_Click" runat="server" />
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plcAddressDetail" Visible="false" runat="server">
            
                <input type="hidden" id="hdnAddressId" runat="server" />

                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Address type</td>
                        <td><asp:DropDownList ID="ddlOtherAddressType" CssClass="inputText" runat="server">
                            <asp:ListItem Value="Outbound" Selected="True">Outbound Shipping</asp:ListItem>
                        </asp:DropDownList></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Address</td>
                        <td><asp:TextBox ID="txtOtherAddressLine1" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td><asp:TextBox ID="txtOtherAddressLine2" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td><asp:TextBox ID="txtOtherAddressLine3" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td><asp:TextBox ID="txtOtherAddressLine4" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">City</td>
                        <td><asp:TextBox ID="txtOtherAddressCity" Width="125" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">State/Province/Region</td>
                        <td><asp:TextBox ID="txtOtherAddressState" Width="150" CssClass="inputText" MaxLength="20" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Postal Code</td>
                        <td><asp:TextBox ID="txtOtherAddressPostalCode" Width="50" CssClass="inputText" MaxLength="20" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Country</td>
                        <td><asp:TextBox ID="txtOtherAddressCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                </table>
                <br />
                <center>
                    <asp:Button ID="btnSaveAddress" CssClass="button" Text="Save Address" OnClick="btnSaveAddress_Click" ValidationGroup="CreditCardInfo" runat="server" />
                    &nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnCancelSaveAddress" CssClass="button" Text="Cancel/Back to List" OnClick="btnCancelSaveAddress_Click" runat="server" />
                </center>
            </asp:PlaceHolder>

        </fieldset>
    </asp:PlaceHolder>


    <telerik:RadToolTip runat="server" ID="exhibitorUploadTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="exhibitorUploadHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(exhibitorUploadTT, "ExhibitorUploadHelp")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="primaryEmailTT" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="primaryEmailHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(primaryEmailTT, "PrimaryEmailHelp")%>
    </telerik:RadToolTip>

</asp:Content>
