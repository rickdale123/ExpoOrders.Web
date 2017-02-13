<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnersMaster.Master" ValidateRequest="false" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="Products.aspx.cs" Inherits="ExpoOrders.Web.Owners.Products" %>
<%@ MasterType VirtualPath="~/Owners/OwnersMaster.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="~/CustomControls/FormQuestionEditorList.ascx" TagPrefix="uc" TagName="FormQuestionEditorList" %>
<%@ Register Src="~/CustomControls/FormQuestionEditor.ascx" TagPrefix="uc" TagName="FormQuestionEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

<script type="text/javascript">
        //<![CDATA[
    function launchCascadingPriceUpdateDialog(categoryId) {
        var oWnd = radopen("ModalDialog.aspx?mode=pricing&show=true&categoryId=" + categoryId, "modalPricingUpdates");
    }


    function refreshCategoryList(categoryId) {
        if (categoryId) {
            var btnRefreshCategoryPricing = document.getElementById("<%= btnRefreshCategoryPricing.ClientID %>");

            if (btnRefreshCategoryPricing) {
                btnRefreshCategoryPricing.click();
            }
        }
    }

    function OnClientClose(oWnd, args) {
        //get the transferred arguments
        var arg = args.get_argument();

        if (arg) {
            var categoryId = arg.categoryId;

            var btnRefreshCategoryPricing = document.getElementById("<%= btnRefreshCategoryPricing.ClientID %>");

            if (btnRefreshCategoryPricing) {
                btnRefreshCategoryPricing.click();
            }
        }
    }
        //]]>
    </script>


<telerik:RadWindowManager ID="RadWindowManager1" ShowContentDuringLoad="false" VisibleStatusbar="false" ReloadOnShow="true" runat="server" EnableShadow="true">
    <Windows>
        <telerik:RadWindow ID="modalPricingUpdates" runat="server" Behaviors="Close" OnClientClose="OnClientClose" NavigateUrl="ModalDialog.aspx">
        </telerik:RadWindow>
        
    </Windows>
</telerik:RadWindowManager>

<CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

    <fieldset class="commonControls">
        <legend class="commonControls"><asp:Literal ID="ltrCategoryMode" runat="server" /></legend>

        <asp:PlaceHolder ID="plcCategories" Visible="false" runat="server">

        <fieldset class="commonControls">
            <legend class="commonControls">Apply Show Level Default Values</legend>

            <table border="0" cellpadding="0" cellspacing="1" width="100%">
                <tr>
                    <td class="contentLabelRight">&nbsp;</td>
                    <td style="text-align:right; white-space: nowrap;">
                        <a href="#" class="action-link" onclick="launchCascadingPriceUpdateDialog(0); return false;">Apply Pricing % Update (All Categories)</a>
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Default Submission Deadline</td>
                    <td>
                        <asp:TextBox ID="txtDefaultSubmissionDeadline" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                        <cc1:CalendarExtender ID="calExtSubmissionDeadline" TargetControlID="txtDefaultSubmissionDeadline" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Standard Price Deadline</td>
                    <td>
                        <asp:TextBox ID="txtDefaultDiscountDeadline" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                        <cc1:CalendarExtender ID="calExtDiscountDeadline" TargetControlID="txtDefaultDiscountDeadline" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Advanced Price Deadline</td>
                    <td nowrap="nowrap">
                        <asp:TextBox ID="txtDefaultEarlyBirdDeadline" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                        <cc1:CalendarExtender ID="calEarlyBirdDeadline" TargetControlID="txtDefaultEarlyBirdDeadline" runat="server" />
                        
                        <span class="informational"><asp:Literal ID="ltrAdvancedPricingCountDownStatus" runat="server" /></span>
                        <asp:Button ID="btnToggleAdvancedPricingCountdown" Text="En/Disable Countdown Ticker" OnClick="btnToggleAdvancedPricingCountdown_Click" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Late Fee Deadline</td>
                    <td>
                        <asp:TextBox ID="txtDefaultLateFeeDeadline" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                        <cc1:CalendarExtender ID="calLateFeeDeadline" TargetControlID="txtDefaultLateFeeDeadline" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Tax:</td>
                    <td><asp:TextBox ID="txtDefaultSalesTax" CssClass="inputText" Width="35" MaxLength="5" runat="server" />%</td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight"><asp:Button ID="btnApplyShowDefaults" Text="Apply Values" CssClass="button" OnClick="btnApplyShowDefaults_Click" runat="server" /></td>
                    <td>
                        <asp:RadioButton ID="rdoApplyOnlyToExisting" Text="Only those products with pre-existing values" Checked="true" GroupName="ApplyCondition" runat="server" /><br />
                        <asp:RadioButton ID="rdoApplyToAll" Text="Apply to ALL products, regardless of pre-existing value" GroupName="ApplyCondition" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
            </table>
        </fieldset>
        
         <asp:GridView EnableViewState="true" runat="server" ID="grdvwCategoryList" AllowPaging="false" AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" 
            AlternatingRowStyle-CssClass="altItem" RowStyle-CssClass="item" OnRowCommand="grdvwCategoryList_RowCommand" OnRowDataBound="grdvwCategoryList_RowDataBound"
                EmptyDataText="There are currently no categories." GridLines="None">
                <Columns>

                    <asp:TemplateField HeaderText="Category" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" HeaderStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton Visible="true" ID="lbtnManageCategory" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "CategoryName").ToString())%>' CommandName="EditCategory"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CategoryId")%>' ToolTip='<%# "CategoryId: " + DataBinder.Eval(Container.DataItem, "CategoryId")%>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Icon">
                        <ItemTemplate>
                            <asp:Image ID="imgCategoryIcon" width="60" height="60" BorderStyle="None" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Sort Order">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "SortOrder")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Active?">
                        <ItemTemplate>
                            <asp:Literal ID="ltrActive" Text='' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton Visible="true" ID="lbtnDeleteCategory" Text="Delete" CommandName="DeleteCategory"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CategoryId")%>' runat="server" />
                            <asp:LinkButton Visible="true" ID="lbtnRestoreCategory" Text="Restore" CommandName="RestoreCategory"
                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CategoryId")%>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                </Columns>
            </asp:GridView>
            <br />

            <asp:Button ID="btnAddCategory" Text="Create a Category" CssClass="button" OnClick="btnAddCategory_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Button ID="btnDisplayCopyCategory" Text="Copy Category from a Show" CssClass="button" OnClick="btnDisplayCopyCategory_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Button ID="btnRefresh" CssClass="button" Text="Refresh" OnClick="btnRefresh_Click" runat="server" />
            &nbsp;
            <asp:CheckBox ID="chkIncludeInactive" runat="server" Text="Show Inactive Categories" Checked="false" />

        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plcCategoryDetail" Visible="false" runat="server">
            <asp:PlaceHolder ID="plcCategoryPricingUpdate" Visible="false" runat="server">
            
                <fieldset class="commonControls">
                <legend class="commonControls">Apply Cascading Category Values</legend>

                <table border="0" cellpadding="0" cellspacing="1" width="100%">
                    <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td style="text-align:right; white-space: nowrap;">
                            <a id="lnkCategoryPricingUpdate" href="#" class="action-link" runat="server">Apply Pricing % Update (for Category)</a>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Submission Deadline</td>
                        <td>
                            <asp:TextBox ID="txtCategorySubmissionDeadline" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                            <cc1:CalendarExtender ID="CalendarExtender2" TargetControlID="txtCategorySubmissionDeadline" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Standard Price Deadline</td>
                        <td>
                            <asp:TextBox ID="txtCategoryDiscountDeadline" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                            <cc1:CalendarExtender ID="CalendarExtender3" TargetControlID="txtCategoryDiscountDeadline" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Advanced Price Deadline</td>
                        <td>
                            <asp:TextBox ID="txtCategoryEarlyBirdDeadline" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                            <cc1:CalendarExtender ID="CalendarExtender4" TargetControlID="txtCategoryEarlyBirdDeadline" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Late Fee Deadline</td>
                        <td>
                            <asp:TextBox ID="txtCategoryLateFeeDeadline" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                            <cc1:CalendarExtender ID="CalendarExtender5" TargetControlID="txtCategoryLateFeeDeadline" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Tax:</td>
                        <td><asp:TextBox ID="txtCategorySalesTax" CssClass="inputText" Width="35" MaxLength="5" runat="server" />%</td>
                        <td>&nbsp;</td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight"><asp:Button ID="btnApplyCategoryValues" Text="Apply Values" CssClass="button" OnClick="btnApplyCategoryDefaults_Click" runat="server" /></td>
                        <td colspan="2">
                            <asp:RadioButton ID="rdoApplyCategoryOnlyToExisting" Text="Only those products with pre-existing values" Checked="true" GroupName="ApplyCategoryCondition" runat="server" /><br />
                            <asp:RadioButton ID="rdoApplyCategoryToAll" Text="Apply to ALL products, regardless of pre-existing value" GroupName="ApplyCategoryCondition" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </table>
            </fieldset>
        </asp:PlaceHolder>

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Category Id</td>
                    <td>
                        <asp:Label ID="lblCategoryId" CssClass="techieInfo" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Category Name:</td>
                    <td><asp:TextBox ID="txtCategoryName" MaxLength="100" Width="250" CssClass="inputText" runat="server" /></td>
                    <td width="50%">&nbsp;</td>
                </tr>
                 <tr>
                    <td class="contentLabelRight">Display Icon:</td>
                    <td>
                        <a id="lnkCategoryIconFile" target="_blank" runat="server"><asp:Label ID="lblCategoryIconFileName" runat="server" /></a>
                            &nbsp;&nbsp;
                            <asp:Button ID="btnRemoveCategoryIconFile" CssClass="button" Text="Remove Icon" OnClick="btnRemoveCategoryIconFile_Click" runat="server" />

                            <asp:FileUpload ID="fupUploadCategoryIcon" runat="server" />
                            <asp:HiddenField ID="hdnCategoryIconFileName" runat="server" />
                    </td>
                    <td width="50%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Sort Order:</td>
                    <td><asp:TextBox ID="txtCategorySortOrder" MaxLength="3" Width="25" CssClass="inputText" runat="server" /></td>
                    <td width="50%">&nbsp;</td>
                </tr>
            </table>
            
            <hr />
            
            <asp:PlaceHolder ID="plcProductList" Visible="false" runat="server">
            
                <asp:GridView EnableViewState="true" runat="server" ID="grdvwProductList" AllowPaging="false" AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" 
                AlternatingRowStyle-CssClass="altItem" RowStyle-CssClass="item" OnRowCommand="grdvwProductList_RowCommand" OnRowDataBound="grdvwProductList_RowDataBound"
                    EmptyDataText="There are currently no Products." GridLines="None">
                    <Columns>

                        <asp:TemplateField HeaderText="Product Name" HeaderStyle-Width="150px">
                            <ItemTemplate>
                                <asp:LinkButton Visible="true" ID="lbtnManageProduct" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "ProductName").ToString())%>' CommandName="EditProduct"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ProductId")%>' ToolTip='<%# String.Format("Id {0}", DataBinder.Eval(Container.DataItem, "ProductId"))%>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Show Floor Price" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox ID="txtListItemProductPrice" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "UnitPriceDisplayFormat").ToString())%>' MaxLength="10" Width="60" CssClass="inputTextRight" runat="server" />
                                <asp:HiddenField ID="hdnProductId" Value='<%# DataBinder.Eval(Container.DataItem, "ProductId")%>' runat="server" />
                                <asp:HiddenField ID="hdnProductTypeCd" Value='<%# DataBinder.Eval(Container.DataItem, "ProductTypeCd")%>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Standard Price" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox ID="txtListItemProductDiscountPrice" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "DiscountUnitPriceDisplayFormat").ToString())%>' MaxLength="10" Width="60" CssClass="inputTextRight" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Advanced Price" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox ID="txtListItemProductEarlyBirdPrice" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "EarlyBirdPriceDisplayFormat").ToString())%>' MaxLength="10" Width="60" CssClass="inputTextRight" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Sales Tax %">
                            <ItemTemplate>
                                <asp:TextBox ID="txtListItemSalesTax" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "SalesTaxDisplayFormat").ToString())%>' MaxLength="10" Width="30" CssClass="inputTextRight" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Sort Order">
                            <ItemTemplate>
                                <asp:TextBox ID="txtListItemProductSortOrder" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "SortOrder").ToString())%>' MaxLength="3" Width="25" CssClass="inputText" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Item Type">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "ProductTypeCd")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton Visible="true" ID="lbtnDeleteProduct" Text="Delete" CommandName="DeleteProduct"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ProductId")%>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                
                <center>
                    <asp:Button ID="btnSaveCategoryPricing" CssClass="button" Text="Save" OnClick="btnSaveCategoryPricing_Click" runat="server" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnAddProduct" CssClass="button" Text="Create a Product" OnClick="btnAddProduct_Click" runat="server" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnDisplayCopyProduct" CssClass="button" Text="Copy Product from a Show" OnClick="btnDisplayCopyProduct_Click" runat="server" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnCancelCategoryDetail" CssClass="button" Text="Cancel" OnClick="btnCancelCategoryDetail_Click" runat="server" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnRefreshCategoryPricing" CssClass="button" Text="Refresh" OnClick="btnRefreshCategoryPricing_Click" runat="server" />
                </center>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plcSaveNewCategory" Visible="false" runat="server">
                <asp:Button ID="btnSaveNewCategory" CssClass="button" Text="Add Category" OnClick="btnSaveNewCategory_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnCancelSaveNewCategory" CssClass="button" Text="Cancel" OnClick="btnCancelSaveNewCategory_Click" runat="server" />
            </asp:PlaceHolder>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plcProductDetail" Visible="false" runat="server">

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Product Id</td>
                    <td><asp:Label ID="lblProductId" CssClass="techieInfo" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Product Type:</td>
                    <td><asp:DropDownList ID="ddlProductType" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">
                        <asp:RequiredFieldValidator ID="txtProductNameValidator" ErrorMessage="Item Name is required." EnableClientScript="false" ValidationGroup="SaveNewProduct" ControlToValidate="txtProductName" CssClass="errorIndicator" runat="server">*</asp:RequiredFieldValidator>
                        Item Name/Title:
                    </td>
                    <td><asp:TextBox id="txtProductName" CssClass="inputText" MaxLength="200" Width="350" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Description:</td>
                    <td>
                        <telerik:radeditor ID="txtProductDescription" Width="500" Height="350" ContentFilters="MakeUrlsAbsolute" ContentAreaMode="Iframe" EnableResize="false" CssClass="htmlEditorContent" runat="server" Skin="Outlook">
                            <Tools>
                                <telerik:EditorToolGroup Tag="MainToolbar">
                                    <telerik:EditorTool Name="SelectAll" ShortCut="CTRL+A" />
                                    <telerik:EditorTool Name="Cut" />
                                    <telerik:EditorTool Name="Copy" ShortCut="CTRL+C" />
                                    <telerik:EditorTool Name="Paste" ShortCut="CTRL+V" />
                                    <telerik:EditorToolStrip Name="PasteStrip">
                                    </telerik:EditorToolStrip>
                                    <telerik:EditorSeparator />
                                    <telerik:EditorSplitButton Name="Undo">
                                    </telerik:EditorSplitButton>
                                    <telerik:EditorSplitButton Name="Redo">
                                    </telerik:EditorSplitButton>
                                </telerik:EditorToolGroup>
                                 <telerik:EditorToolGroup>
                                    <telerik:EditorTool Name="Bold" ShortCut="CTRL+B" />
                                    <telerik:EditorTool Name="Italic" ShortCut="CTRL+I" />
                                    <telerik:EditorTool Name="Underline" ShortCut="CTRL+U" />
                                    <telerik:EditorTool Name="StrikeThrough" />
                                    <telerik:EditorSeparator />
                                    <telerik:EditorTool Name="JustifyLeft" />
                                    <telerik:EditorTool Name="JustifyCenter" />
                                    <telerik:EditorTool Name="JustifyRight" />
                                    <telerik:EditorTool Name="JustifyFull" />
                                    <telerik:EditorTool Name="JustifyNone" />
                                    <telerik:EditorSeparator />
                                    <telerik:EditorTool Name="Indent" />
                                    <telerik:EditorTool Name="Outdent" />
                                    <telerik:EditorSeparator />
                                    <telerik:EditorTool Name="InsertOrderedList" />
                                    <telerik:EditorTool Name="InsertUnorderedList" />
                                    <telerik:EditorSeparator />
                                    <telerik:EditorTool Name="ToggleTableBorder" />
                                </telerik:EditorToolGroup>
                                <telerik:EditorToolGroup Tag="InsertToolbar">
                                    <telerik:EditorTool Name="ImageManager" ShortCut="CTRL+G" />
                                    <telerik:EditorTool Name="LinkManager" ShortCut="CTRL+K" />
                                    <telerik:EditorTool Name="Unlink" ShortCut="CTRL+SHIFT+K" />
                                </telerik:EditorToolGroup>
                                <telerik:EditorToolGroup>
                                    <telerik:EditorDropDown Name="FormatBlock">
                                    </telerik:EditorDropDown>
                                    <telerik:EditorDropDown Name="FontName">
                                    </telerik:EditorDropDown>
                                    <telerik:EditorDropDown Name="RealFontSize">
                                    </telerik:EditorDropDown>
                                </telerik:EditorToolGroup>
                                <telerik:EditorToolGroup>
                                    <telerik:EditorTool Name="Superscript" />
                                    <telerik:EditorTool Name="Subscript" />
                                    <telerik:EditorTool Name="InsertParagraph" />
                                    <telerik:EditorTool Name="InsertGroupbox" />
                                    <telerik:EditorTool Name="InsertHorizontalRule" />
                                </telerik:EditorToolGroup>
                                
                                <telerik:EditorToolGroup>
                                    <telerik:EditorSplitButton Name="ForeColor">
                                    </telerik:EditorSplitButton>
                                    <telerik:EditorSplitButton Name="BackColor">
                                    </telerik:EditorSplitButton>
                                </telerik:EditorToolGroup>
                                <telerik:EditorToolGroup Tag="DropdownToolbar">
                                    <telerik:EditorSplitButton Name="InsertSymbol">
                                    </telerik:EditorSplitButton>
                                    <telerik:EditorToolStrip Name="InsertTable">
                                    </telerik:EditorToolStrip>
                                    <telerik:EditorSeparator />
                                    <telerik:EditorTool Name="ConvertToLower" />
                                    <telerik:EditorTool Name="ConvertToUpper" />
                                </telerik:EditorToolGroup>
                            </Tools>
                            <CssFiles>
                                <telerik:EditorCssFile Value="/Style/CommonAdmin.css" />
                            </CssFiles>
                            <Paragraphs>
                                <telerik:EditorParagraph Tag="H1" Title="Header 1" />
                                <telerik:EditorParagraph Tag="H2" Title="Header 2" />
                                <telerik:EditorParagraph Tag="h3" Title="Header 3" />
                                <telerik:EditorParagraph Tag="strong" Title="Strong" />
                                <telerik:EditorParagraph Tag="p" Title="Paragraph" />
                                <telerik:EditorParagraph Tag="" Title="Normal" />
                            </Paragraphs>
                            <Content>
                            </Content>
                        </telerik:radeditor>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Sort Order:</td>
                    <td><asp:TextBox ID="txtProductSortOrder" CssClass="inputText" MaxLength="3" Width="25" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="sectionDivider" colspan="3">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Submission Deadline:</td>
                    <td><asp:TextBox  ID="txtSubmissionDeadline" CssClass="inputText" Width="100" maxlength="100" runat="server" /><cc1:CalendarExtender ID="calExtender" TargetControlID="txtSubmissionDeadline" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Late Behavior:</td>
                    <td><asp:DropDownList ID="ddlLateBehavior" CssClass="inputText" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Late Message:</td>
                    <td><asp:TextBox ID="txtLateMessage" TextMode="MultiLine" Columns="50" Rows="3" CssClass="inputText" runat="server" /></td>
                    <td>&nbsp;</td>
                </tr>

                <asp:PlaceHolder ID="plcNewProductSaveControls" Visible="false" runat="server">
                    <tr>
                        <td colspan="3" align="center">
                            <asp:Button ID="btnSaveNewProduct" CssClass="button" Text="Save and Continue" OnClick="btnSaveNewProduct_Click" ValidationGroup="SaveNewProduct" runat="server" />
                            &nbsp;&nbsp;
                            <asp:Button ID="btnCancelSaveNewProduct" CssClass="button" Text="Cancel" OnClick="btnCancelSaveNewProduct_Click" runat="server" />
                        </td>
                    </tr>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plcProductExistingProductDetails" Visible="false" runat="server">
                
                    <asp:PlaceHolder ID="plcFileProductTypeControls" Visible="false" runat="server">
                        <tr>
                            <td class="sectionDivider" colspan="3">&nbsp;</td>
                        </tr>
                        <asp:HiddenField ID="hdnFileDownLoadId" runat="server" />
                        <tr>
                            <td class="contentLabelRight">File Name:</td>
                            <td>
                                <a id="lnkFileDownload" target="_blank" runat="server"><asp:Label ID="lblFileDownloadFileName" runat="server" /></a>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">New File: </td>
                            <td><asp:FileUpload ID="fupFileDownload" runat="server" /><asp:Button ID="btnUploadFileDownload" Text="Upload File" CssClass="button" OnClick="btnUploadFileDownload_Click" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="plcItemProductTypeControls" Visible="false" runat="server">
                        <tr>
                            <td class="sectionDivider" colspan="3">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Product Sku:</td>
                            <td><asp:TextBox ID="txtProductSku" CssClass="inputText" MaxLength="50" size="50" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Show Floor Price:</td>
                            <td><asp:Label id="lblShowCurrencySymbol" runat="server" />&nbsp;<asp:TextBox ID="txtUnitPrice" CssClass="inputText" Width="65" MaxLength="10" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Standard Price:</td>
                            <td><asp:Label id="lblShowCurrencySymbol2" runat="server" />&nbsp;<asp:TextBox ID="txtDiscountUnitPrice" CssClass="inputText" Width="65" MaxLength="10" runat="server" />
                            &nbsp;&nbsp;Order by date: <asp:TextBox  ID="txtDiscountDeadline" CssClass="inputText" Width="100" maxlength="100" runat="server" /><cc1:CalendarExtender ID="CalendarExtender1" TargetControlID="txtDiscountDeadline" runat="server" />
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                         <tr>
                            <td class="contentLabelRight">Advanced Price:</td>
                            <td><asp:Label id="lblShowCurrencySymbol3" runat="server" />&nbsp;<asp:TextBox ID="txtEarlyBirdPrice" CssClass="inputText" Width="65" MaxLength="10" runat="server" />
                                &nbsp;&nbsp;Order by date: <asp:TextBox  ID="txtEarlyBirdDeadline" CssClass="inputText" Width="100" maxlength="100" runat="server" /><cc1:CalendarExtender ID="calExtEarlyBirdDeadline" TargetControlID="txtEarlyBirdDeadline" runat="server" />
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Unit Descriptor:</td>
                            <td><asp:TextBox ID="txtUnitDescriptor" CssClass="inputText" Width="125" MaxLength="25" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Quantity Label:</td>
                            <td><asp:TextBox ID="txtQuantityLabel" CssClass="inputText" Width="125" MaxLength="25" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Required Minimum:</td>
                            <td><asp:TextBox ID="txtMinimumQuantityRequired" CssClass="inputText" Width="125" MaxLength="25" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Visible to Exhibitors?:</td>
                            <td>
                                <asp:CheckBox ID="chkVisibleToExhibitors" runat="server" />
                                 <span class="informational">(Unchecked will ONLY be available via Order Adjustment screen)</span>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Tax Exempt?</td>
                            <td>
                                <asp:CheckBox ID="chkTaxExempt" runat="server" />
                                 <span class="informational">(Exempt will never apply tax, or be 'cascaded' to)</span>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Tax:</td>
                            <td><asp:TextBox ID="txtSalesTax" CssClass="inputText" Width="35" MaxLength="5" runat="server" />%</td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="sectionDivider" colspan="3">&nbsp;</td>
                        </tr>
                         <tr>
                            <td class="contentLabelRight">Late Fee Deadline:</td>
                            <td><asp:TextBox  ID="txtLateFeeDeadline" CssClass="inputText" Width="100" maxlength="100" runat="server" /><cc1:CalendarExtender ID="calExtenderLateFeeDeadline" TargetControlID="txtLateFeeDeadline" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Late Fee Type:</td>
                            <td><asp:DropDownList ID="ddlLateFeeType" CssClass="inputText" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Late Fee Amount:</td>
                            <td><asp:TextBox ID="txtLateFeeAmount" CssClass="inputText" size="35" MaxLength="10" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="sectionDivider" colspan="3">&nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="3"><h3>Images</h3></td>
                        </tr>


                        <tr class="item">
                            <td colspan="3"><asp:Image ID="imgProductImage" width="60" height="55" border="0" runat="server" /></td>
                        </tr>
                        <tr class="item">
                            <td class="contentLabelRight">Primary Image:</td>
                            <td>
                                <a id="imgProductLink" target="_blank" runat="server"><asp:Label ID="lblImageName" runat="server" /></a>
                                &nbsp;&nbsp;
                                
                                <asp:LinkButton ID="lnkRemoveProductImage" Text="Remove" OnClick="lnkRemoveProductImage_Click" runat="server" />
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr class="item">
                            <td class="contentLabelRight">New Image:</td>
                            <td><asp:FileUpload ID="fupProductImage" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>

                        <tr class="altItem">
                            <td colspan="3"><asp:Image ID="imgAdditionalImage1" width="60" height="55" border="0" runat="server" /></td>
                        </tr>
                        <tr class="altItem">
                            <td class="contentLabelRight">Additional Image 1:</td>
                            <td>
                                <a id="imgAdditionalImageLink1" target="_blank" runat="server"><asp:Label ID="lblAdditionalImageName1" runat="server" /></a>
                                &nbsp;&nbsp;
                                
                                <asp:LinkButton ID="lnkRemoveAdditionalImage1" Text="Remove" OnClick="lnkRemoveAdditionalImage1_Click" runat="server" />
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr class="altItem">
                            <td class="contentLabelRight">New Image:</td>
                            <td><asp:FileUpload ID="fupAdditionalImage1" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>

                        <tr class="item">
                            <td colspan="3"><asp:Image ID="imgAdditionalImage2" width="60" height="55" border="0" runat="server" /></td>
                        </tr>
                        <tr class="item">
                            <td class="contentLabelRight">Additional Image 2:</td>
                            <td>
                                <a id="imgAdditionalImageLink2" target="_blank" runat="server"><asp:Label ID="lblAdditionalImageName2" runat="server" /></a>
                                &nbsp;&nbsp;
                                
                                <asp:LinkButton ID="lnkRemoveAdditionalImage2" Text="Remove" OnClick="lnkRemoveAdditionalImage2_Click" runat="server" />
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr class="item">
                            <td class="contentLabelRight">New Image:</td>
                            <td><asp:FileUpload ID="fupAdditionalImage2" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>

                        <tr class="altItem">
                            <td colspan="3"><asp:Image ID="imgAdditionalImage3" width="60" height="55" border="0" runat="server" /></td>
                        </tr>
                        <tr class="altItem">
                            <td class="contentLabelRight">Additional Image 3:</td>
                            <td>
                                <a id="imgAdditionalImageLink3" target="_blank" runat="server"><asp:Label ID="lblAdditionalImageName3" runat="server" /></a>
                                &nbsp;&nbsp;
                                
                                <asp:LinkButton ID="lnkRemoveAdditionalImage3" Text="Remove" OnClick="lnkRemoveAdditionalImage3_Click" runat="server" />
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr class="altItem">
                            <td class="contentLabelRight">New Image:</td>
                            <td><asp:FileUpload ID="fupAdditionalImage3" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>

                        <tr class="item">
                            <td colspan="3"><asp:Image ID="imgAdditionalImage4" width="60" height="55" border="0" runat="server" /></td>
                        </tr>
                        <tr class="item">
                            <td class="contentLabelRight">Additional Image 4:</td>
                            <td>
                                <a id="imgAdditionalImageLink4" target="_blank" runat="server"><asp:Label ID="lblAdditionalImageName4" runat="server" /></a>
                                &nbsp;&nbsp;
                                
                                <asp:LinkButton ID="lnkRemoveAdditionalImage4" Text="Remove" OnClick="lnkRemoveAdditionalImage4_Click" runat="server" />
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr class="item">
                            <td class="contentLabelRight">New Image:</td>
                            <td><asp:FileUpload ID="fupAdditionalImage4" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>

                        <tr class="altItem">
                            <td colspan="3"><asp:Image ID="imgAdditionalImage5" width="60" height="55" border="0" runat="server" /></td>
                        </tr>
                        <tr class="altItem">
                            <td class="contentLabelRight">Additional Image 5:</td>
                            <td>
                                <a id="imgAdditionalImageLink5" target="_blank" runat="server"><asp:Label ID="lblAdditionalImageName5" runat="server" /></a>
                                &nbsp;&nbsp;
                                
                                <asp:LinkButton ID="lnkRemoveAdditionalImage5" Text="Remove" OnClick="lnkRemoveAdditionalImage5_Click" runat="server" />
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr class="altItem">
                            <td class="contentLabelRight">New Image:</td>
                            <td><asp:FileUpload ID="fupAdditionalImage5" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>

                        <tr>
                            <td colspan="3" align="center"><asp:Button ID="btnUploadAll" OnClick="btnUploadAllImages_Click" runat="server" Text="Upload All Images" /></td>
                        </tr>
                        
                        <tr>
                            <td class="sectionDivider" colspan="3">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Additional Charge Type:</td>
                            <td><asp:DropDownList ID="ddlAdditionalChargeType" CssClass="inputText" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Additional Charge Amount:</td>
                            <td><asp:TextBox ID="txtAdditionalCharge" CssClass="inputText" size="35" MaxLength="10" runat="server" /></td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Required Attribute 1 Label:</td>
                            <td valign="top">
                                <asp:TextBox ID="txtRequiredAttributeLabel1" CssClass="inputText" MaxLength="50" size="35" runat="server" />
                                <span class="informational">(ex: Color)</span>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Attribute 1 Choices:</td>
                            <td valign="top">
                                <asp:TextBox ID="txtRequiredAttributeChoiceList1" CssClass="inputText" TextMode="MultiLine" Rows="3" Columns="40" runat="server" />
                                <span class="informational">(ex: Red;White;Blue)</span>
                            </td>
                            <td class="informational">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Required Attribute 2 Label:</td>
                            <td valign="top">
                                <asp:TextBox ID="txtRequiredAttributeLabel2" CssClass="inputText" MaxLength="50" size="35" runat="server" />
                                <span class="informational">(ex: Size)</span>
                            </td>
                            <td class="informational">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Attribute 2 Choices:</td>
                            <td valign="top">
                                <asp:TextBox ID="txtRequiredAttributeChoiceList2" CssClass="inputText" TextMode="MultiLine" Rows="3" Columns="40" runat="server" />
                                <span class="informational">(ex: Small;Medium;Large;Extra Large)</span>
                            </td>
                            <td class="informational">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Use First 2 attributes as multiplier?<br />
                                <span class="informational">(#hrs x #workers)</span></td>
                            <td valign="top">
                                <asp:RadioButtonList ID="rdoLstCalcQuantityFromAttributes" CssClass="inputText" RepeatDirection="Horizontal" runat="server">
                                    <asp:ListItem Value="0">No</asp:ListItem>
                                    <asp:ListItem Value="1">Yes</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td class="informational">&nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="3">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Attribute 3 Label:</td>
                            <td valign="top">
                                <asp:TextBox ID="txtRequiredAttributeLabel3" CssClass="inputText" MaxLength="50" size="35" runat="server" />
                                <span class="informational">(ex: Color)</span>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Attribute 3 Choices:</td>
                            <td valign="top">
                                <asp:TextBox ID="txtRequiredAttributeChoiceList3" CssClass="inputText" TextMode="MultiLine" Rows="3" Columns="40" runat="server" />
                                <span class="informational">(ex: Red;White;Blue)</span>
                            </td>
                            <td class="informational">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Attribute 4 Label:</td>
                            <td valign="top">
                                <asp:TextBox ID="txtRequiredAttributeLabel4" CssClass="inputText" MaxLength="50" size="35" runat="server" />
                                <span class="informational">(ex: Size)</span>
                            </td>
                            <td class="informational">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Attribute 4 Choices:</td>
                            <td valign="top">
                                <asp:TextBox ID="txtRequiredAttributeChoiceList4" CssClass="inputText" TextMode="MultiLine" Rows="3" Columns="40" runat="server" />
                                <span class="informational">(ex: Small;Medium;Large;Extra Large)</span>
                            </td>
                            <td class="informational">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="sectionDivider" colspan="3">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Display on Install/Dismantle<br /> Report?</td>
                            <td valign="top">
                                <asp:RadioButtonList ID="rdoBtnLstInstallDismantleInd" CssClass="inputText" RepeatDirection="Horizontal" runat="server">
                                    <asp:ListItem Value="">Neither</asp:ListItem>
                                    <asp:ListItem Value="I">as Installation</asp:ListItem>
                                    <asp:ListItem Value="D">as Dismantle</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="sectionDivider" colspan="3">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Is this a Product Bundle?</td>
                            <td valign="top">
                                <asp:RadioButtonList ID="rdoLstProductBundle" CssClass="inputText" AutoPostBack="true" OnSelectedIndexChanged="rdoLstProductBundle_Changed" RepeatDirection="Horizontal" runat="server">
                                    <asp:ListItem Value="N">No</asp:ListItem>
                                    <asp:ListItem Value="Y">Yes</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <asp:PlaceHolder ID="plcProductAssociations" Visible="false" runat="server">
                            <tr>
                                <td colspan="3">
                                    <fieldset class="commonControls">
                                        <legend class="commonControls">Items/Services Included with this Product:</legend>

                                            <asp:GridView EnableViewState="true" runat="server" ID="grdvwAssociatedProducts" AllowPaging="false" AllowSorting="false"
                                                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                                                RowStyle-CssClass="item" OnRowDataBound="grdvwAssociatedProducts_RowDataBound" GridLines="None"
                                                OnRowCommand="grdvwAssociatedProducts_RowCommand" EmptyDataText="There are no associated Products.">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Category" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="200">
                                                        <ItemTemplate>
                                                            <%# DataBinder.Eval(Container.DataItem, "AssociatedProduct.Category.CategoryName")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Item/Service" HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="250">
                                                        <ItemTemplate>
                                                            <%# DataBinder.Eval(Container.DataItem, "AssociatedProduct.ProductName")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Qty Included" HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                                        <ItemTemplate>
                                                            <%# DataBinder.Eval(Container.DataItem, "AssociatedQuantity")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                     <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton Visible="true" ID="lbtnDeleteAssociatedProduct" Text="Delete" CommandName="DeleteAssociatedProduct"
                                                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AssociatedProductId")%>' runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>

                                            <fieldset class="commonControls">
                                                <legend class="commonControls">Add Associated Product</legend>
                                                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                                                    <tr>
                                                        <td class="contentLabel">Product</td>
                                                        <td class="contentLabel">Quantity</td>
                                                        <td style="width: 25%">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <telerik:RadComboBox ID="cboAddAssociatedProductId" MaxHeight="400" DataValueField="ProductId" DataTextField="ProductName" Width="300" MarkFirstMatch="true" AllowCustomText="true" runat="server" />
                                                        </td>
                                                        <td><asp:TextBox ID="txtAddAssociatedProductQuantity" MaxLength="4" size="3" CssClass="inputText" runat="server" /></td>
                                                        <td><asp:Button ID="btnAddAssociatedProduct" Text="Add" CssClass="button" OnClick="btnAddAssociatedProduct_Click" runat="server" /></td>
                                                    </tr>
                                                </table>
                                            </fieldset>
                                    </fieldset>
                                </td>
                            </tr>
                        </asp:PlaceHolder>
                        <tr>
                            <td class="sectionDivider" colspan="3">&nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="3" class="altItem">
                                <asp:HiddenField ID="hdnAdditionalInfoFormId" runat="server" />
                    
                                <fieldset class="commonControls">
                                    <legend class="commonControls">Additional Info</legend>

                                    <uc:FormQuestionEditorList ID="ucFormQuestionEditorList" runat="server" />

                                    <br />
                                    <fieldset class="commonControls">
                                        <legend class="commonControls"><asp:Literal ID="ltrQuestionEditorLabel" Text="Add New Question" runat="server" /></legend>
                                        <uc:FormQuestionEditor ID="ucFormQuestionEditor" runat="server" /><br />

                                        <asp:Button ID="btnAddAdditionalInfoQuestion" CssClass="button" Text="Add Question" OnClick="btnAddAdditionalInfoQuestion_Click" runat="server" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="btnCancelAddAdditionalQuestion" Visible="false" CssClass="button" Text="Cancel" OnClick="btnCancelAddAdditionalQuestion_Click" runat="server" />
                                    </fieldset>
                                </fieldset>
                            </td>
                        </tr>
                    
                    </asp:PlaceHolder>
                    <tr>
                        <td colspan="3" align="center">
                            <asp:Button ID="btnSaveProduct" CssClass="button" Text="Save Product Detail" OnClick="btnSaveProduct_Click" runat="server" />
                            &nbsp;&nbsp;
                            <asp:Button ID="btnCancelSaveProduct" CssClass="button" Text="Cancel" OnClick="btnCancelSaveProduct_Click" runat="server" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </table>
        </asp:PlaceHolder>

        <asp:HiddenField ID="hdnCategoryId" runat="server" />
        <asp:HiddenField ID="hdnProductId" runat="server" />
    </fieldset>

    <asp:Panel CssClass="outerPopup" Style="display: none;" runat="server" ID="pnlOuterCopyCategory">
        <asp:Panel Width="400px" CssClass="innerPopup" runat="server" ID="pnlCopyCategory">
            <fieldset class="commonControls">
                <legend class="commonControls">Copy Category</legend>
                <h3><asp:Label ID="lblCopyCategoryLabel" runat="server" /></h3>
                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Show Name</td>
                        <td>
                            <asp:DropDownList ID="ddlCopyCategoryShowList" CssClass="inputText" AutoPostBack="true" OnSelectedIndexChanged="ddlCopyCategoryShowList_IndexChanged" runat="server"></asp:DropDownList>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Category</td>
                        <td>
                            <asp:DropDownList ID="ddlCopyCategoryList" CssClass="inputText" AutoPostBack="false" runat="server"></asp:DropDownList>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </table>
                <br />
                <center>
                    <asp:Button ID="btnCopyCategory" CssClass="button" OnClick="btnCopyCategory_Click" Text="Copy Category" ValidationGroup="CopyCategory" runat="server" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnCancelCopyCategory" CssClass="button" Text="Cancel" ValidationGroup="CopyCategory" onclick="btnCancelCopyCategory_Click" runat="server" />
                </center>
            </fieldset>
        </asp:Panel>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="MPE" runat="server" TargetControlID="dummyBtn" PopupControlID="pnlOuterCopyCategory" BackgroundCssClass="modalBackground"  DropShadow="true" CancelControlID="btnCancelCopyCategory" />
    <cc1:RoundedCornersExtender ID="RCE" runat="server" TargetControlID="pnlCopyCategory" BorderColor="black" Radius="6" />


    <asp:Panel CssClass="outerPopup" Style="display: none;" runat="server" ID="pnlOuterCopyProduct">
        <asp:Panel Width="400px" CssClass="innerPopup" runat="server" ID="pnlCopyProduct">
            <fieldset class="commonControls">
                <legend class="commonControls">Copy Product</legend>
                <h3><asp:Label ID="lblCopyProductLabel" runat="server" /></h3>
                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Show Name</td>
                        <td>
                            <asp:DropDownList ID="ddlCopyProductShowList" CssClass="inputText" AutoPostBack="true" OnSelectedIndexChanged="ddlCopyProductShowList_IndexChanged" runat="server"></asp:DropDownList>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Category</td>
                        <td>
                            <asp:DropDownList ID="ddlCopyProductCategoryList" CssClass="inputText" AutoPostBack="true" OnSelectedIndexChanged="ddlCopyProductCategoryList_IndexChanged" runat="server"></asp:DropDownList>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Product</td>
                        <td>
                            <asp:DropDownList ID="ddlCopyProductProductList" CssClass="inputText" AutoPostBack="false" runat="server"></asp:DropDownList>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </table>
                <br />
                <center>
                    <asp:Button ID="btnCopyProduct" CssClass="button" OnClick="btnCopyProduct_Click" Text="Copy Product" ValidationGroup="CopyProduct" runat="server" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnCancelCopyProduct" CssClass="button" Text="Cancel" ValidationGroup="CopyProduct" onclick="btnCancelCopyProduct_Click" runat="server" />
                </center>
            </fieldset>
        </asp:Panel>
    </asp:Panel>
    
    <cc1:ModalPopupExtender ID="MPE2" runat="server" TargetControlID="dummyBtn" PopupControlID="pnlOuterCopyProduct" BackgroundCssClass="modalBackground"  DropShadow="true" CancelControlID="btnCancelCopyProduct" />
    <cc1:RoundedCornersExtender ID="RCE2" runat="server" TargetControlID="pnlCopyProduct" BorderColor="black" Radius="6" />


    <asp:Button ID="dummyBtn" style="visibility: hidden;" runat="server" />
</asp:Content>
