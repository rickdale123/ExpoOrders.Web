<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnersMaster.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="ContentExplorer.aspx.cs" Inherits="ExpoOrders.Web.Owners.ContentExplorer" %>

<%@ MasterType VirtualPath="~/Owners/OwnersMaster.Master" %>
<%@ Register Src="~/CustomControls/FormQuestionEditorList.ascx" TagPrefix="uc" TagName="FormQuestionEditorList" %>
<%@ Register Src="~/CustomControls/FormQuestionEditor.ascx" TagPrefix="uc" TagName="FormQuestionEditor" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

    <CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

    <asp:PlaceHolder ID="plcHtmlContentList" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Html Content Manager</legend>
            <h3>Html Content List</h3>

            <asp:GridView EnableViewState="true" OnPageIndexChanging="grdvwHtmlContentList_PageIndexChanging"
                runat="server" ID="grdvwHtmlContentList" AllowPaging="false" AllowSorting="true" GridLines="None"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                RowStyle-CssClass="item" OnRowDataBound="grdvwHtmlContentList_RowDataBound" OnRowCommand="grdvwHtmlContentList_RowCommand"
                EmptyDataText="There is current no html content associated.">
                <Columns>
                    <asp:TemplateField HeaderText="Title" ItemStyle-Wrap="false">
                        <ItemTemplate>
                             <asp:LinkButton Visible="true" ID="lbtnManageHtmlContent" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "Title").ToString())%>' 
                                CommandName="EditHtmlContent" ToolTip='<%# "HtmlContentId: " + DataBinder.Eval(Container.DataItem, "HtmlContentId")%>'
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "HtmlContentId")%>' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                            <asp:Literal ID="ltrHtmlContentType" Text='' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Modified On" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:label ID="lblHtmlContentModifiedOn" CssClass="techieInfo" runat="server"><%# DataBinder.Eval(Container.DataItem, "ModifiedOn")%></asp:label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Active?">
                        <ItemTemplate>
                            <asp:Literal ID="ltrActive" Text='' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton Visible="false" ID="lbtnDeactivateHtmlContent" Text="Remove" CommandName="DeactivateHtmlContent" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" runat="server" /></ItemTemplate>
                                
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton Visible="false" ID="lbtnActivateHtmlContent" Text="Restore" CommandName="ActivateHtmlContent"
                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <br />
            <asp:Button ID="btnAddHtmlContent" CssClass="button" Text="Create Content" OnClick="btnAddHtmlContent_Click" runat="server" />
            &nbsp;
            <asp:Button ID="btnRefreshHtmlContent" CssClass="button" Text="Refresh" OnClick="btnRefreshHtmlContent_Click" runat="server" />
            &nbsp;
            <asp:CheckBox ID="chkIncludeInactive" runat="server" Text="Show Inactive Content" Checked="false" />
        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcManageHtmlContent" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Manage HtmlContent</legend>
            <h3>Content Configuration</h3>

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Title</td>
                    <td>
                        <asp:TextBox ID="txtHtmlContentTitle" Visible="true" Width="250" CssClass="inputText" MaxLength="100" runat="server" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" CssClass="errorMessage"
                            ErrorMessage="Content Title is required." EnableClientScript="false" runat="server"
                            ControlToValidate="txtHtmlContentTitle" ValidationGroup="HtmlContentInformation">Missing Content Title.</asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Content Type</td>
                    <td>
                        <asp:DropDownList ID="ddlHtmlContentType" Visible="true" Width="200" CssClass="inputText" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <asp:PlaceHolder ID="plcEmailTokens" runat="server">
                    <tr>
                        <td class="contentLabelRight">Email Tokens:</td>
                        <td>
                            <asp:TextBox ID="txtTokens" Enabled="true" Visible="true" Width="400" TextMode="MultiLine" Rows="4" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">
                            &nbsp;
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td class="contentLabelRight">Email Subject</td>
                    <td>
                        <asp:TextBox ID="txtEmailSubject" CssClass="inputText" Width="350" MaxLength="200" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabel" colspan="3" valign="top">
                        Content:<br />

                       <telerik:radeditor ID="htmlContentEditor" ContentFilters="MakeUrlsAbsolute" ContentAreaMode="Iframe" EnableResize="false" CssClass="htmlEditorContent" runat="server" Skin="Outlook">
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
                </tr>
            </table>
            <br />
            <center>
                <asp:Button ID="btnSaveHtmlContent" CssClass="button" Text="Save Content" ValidationGroup="HtmlContentInformation" OnClick="btnSaveHtmlContent_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnCancelSaveHtmlContent" CssClass="button" Text="Cancel" OnClick="btnCancelSaveHtmlContent_Click" runat="server" />
                <asp:HiddenField runat="server" ID="hdnHtmlContentId" Value="0" />
            </center>
        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcFileDownloadManager" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">File Download Manager</legend>
            <h3>File Download List</h3>

            <asp:GridView EnableViewState="true" OnPageIndexChanging="grdvwFileDownloadList_PageIndexChanging"
                runat="server" ID="grdvwFileDownloadList" AllowPaging="false" AllowSorting="true"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                RowStyle-CssClass="item" OnRowDataBound="grdvwFileDownloadList_RowDataBound" GridLines="None"
                OnRowCommand="grdvwFileDownloadList_RowCommand" EmptyDataText="There are currently no file dowloads associated.">
                <Columns>
                    <asp:TemplateField HeaderText="File Name">
                        <ItemTemplate>
                            <asp:Label ID="lblMissingFile" Text="Missing File!" Visible="false" CssClass="errorMessage" runat="server" />
                             <asp:LinkButton Visible="true" ID="lbtnManageFileDownload" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "FileName").ToString())%>'
                                    CommandName="EditFileDownload" ToolTip='<%# "FileDownloadId: " + DataBinder.Eval(Container.DataItem, "FileDownloadId")%>'
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FileDownloadId")%>' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description">
                        <ItemTemplate>
                            <asp:Literal ID="ltrFileDownloadDescription" Text='' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Content Type">
                        <ItemTemplate>
                            <asp:Literal ID="ltrContentType" Text="" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="File Size" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:Literal ID="ltrFileSize" Text="" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Active?">
                        <ItemTemplate>
                            <asp:Literal ID="ltrActive" Text="" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:LinkButton Visible="false" ID="lbtnDeactivateFileDownload" Text="Remove"
                                CommandName="DeactivateFileDownload" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                        <a id="lnkViewFileDownload" target="_blank" runat="server">View</a>
                            &nbsp;
                            <asp:LinkButton Visible="false" ID="lbtnActivateFileDownload" Text="Restore" CommandName="ActivateFileDownload"
                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <br />
            <asp:Button ID="btnAddFileDownload" CssClass="button" Text="Create File Download" OnClick="btnAddFileDownload_Click" runat="server" />
            &nbsp;
            <asp:Button ID="btnRefreshFileDownloadList" CssClass="button" Text="Refresh" OnClick="btnRefreshFileDownloadList_Click" runat="server" />
            <asp:CheckBox ID="chkIncludeInactiveFileDownload" runat="server" Text="Show Inactive Files" Checked="false" />
        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcManageFileDownload" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Manage File Downloads</legend>

            <h3>File Download Configuration</h3>

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <asp:PlaceHolder ID="plcFileUpload" runat="server">
                    <tr>
                        <td colspan="2" class="contentLabel">
                            Upload new File: &nbsp;
                            <asp:FileUpload ID="fupFileDownload" runat="server" />&nbsp;<asp:Button ID="btnUploadFileDownload" OnClick="btnUploadFileDownload_Click" CssClass="button" runat="server" Text="Upload File" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcFileDownloadDetail" runat="server">
                    <tr>
                        <td class="contentLabelRight">File Name</td>
                        <td>
                            <asp:TextBox ID="txtFileName" Visible="true" Width="200" CssClass="inputText" MaxLength="100" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="errorMessage"
                                ErrorMessage="A File Name is required." EnableClientScript="false" runat="server"
                                ControlToValidate="txtFileName" ValidationGroup="FileDownloadInformation">Missing File Name.</asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">File Description</td>
                        <td>
                            <asp:TextBox ID="txtFileDescription" Visible="true" Width="200" Columns="50" Rows="4" TextMode="MultiLine" CssClass="inputText" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">PopUp Width</td>
                        <td>
                            <asp:TextBox ID="txtPopupWidth" Visible="true" runat="Server" Width="200" CssClass="inputText" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">PopUp Height</td>
                        <td>
                            <asp:TextBox ID="txtPopUpHeight" Visible="true" runat="Server" Width="200" CssClass="inputText" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Content Type</td>
                        <td>
                            <asp:TextBox ID="txtFileDownloadContentType" Visible="true" runat="Server" Width="200" CssClass="inputText" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </asp:PlaceHolder>
            </table>

            <br />

            <center>
                <asp:Button ID="btnSaveFileDownload" CssClass="button" Text="Save File Download" ValidationGroup="FileDownloadInformation" OnClick="btnSaveFileDownload_Click" runat="server" />
                    &nbsp;&nbsp;
                <asp:Button ID="btnCancelSaveFile" CssClass="button" Text="Cancel" OnClick="btnCancelSaveFile_Click" runat="server" />
                <asp:HiddenField runat="server" ID="hdnFileDownloadId" Value="0" />
            </center>

        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcDynamicFormList" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Dynamic Form Manager</legend>
            <h3>Dynamic Form List</h3>

            <asp:GridView EnableViewState="true" OnPageIndexChanging="grdvwDynamicFormList_PageIndexChanging"
                runat="server" ID="grdvwDynamicFormList" AllowPaging="false" AllowSorting="true" GridLines="None"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                RowStyle-CssClass="item" OnRowDataBound="grdvwDynamicFormList_RowDataBound" OnRowCommand="grdvwDynamicFormList_RowCommand"
                EmptyDataText="There are currently no forms associated.">
                <Columns>
                    <asp:TemplateField HeaderText="Form Name">
                        <ItemTemplate>
                            <asp:LinkButton Visible="true" ID="lbtnManageDynamicForm" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "FormName").ToString())%>' 
                                    CommandName="EditDynamicForm" ToolTip='<%# "FormId: " + DataBinder.Eval(Container.DataItem, "FormId")%>'
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FormId")%>' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description">
                        <ItemTemplate>
                            <asp:Literal ID="ltrFormDescription" Text='' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Submission Deadline">
                        <ItemTemplate>
                            <asp:Literal ID="ltrSubmissionDeadline" Text='' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Active?">
                        <ItemTemplate>
                            <asp:Literal ID="ltrActive" Text="" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton Visible="false" ID="lbtnDeactivateDynamicForm" Text="Remove"
                                CommandName="DeactivateDynamicForm" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton Visible="false" ID="lbtnActivateDynamicForm" Text="Restore" CommandName="ActivateDynamicForm"
                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <br />
            <asp:Button ID="btnAddDynamicForm" CssClass="button" Text="Create Form" OnClick="btnAddDynamicForm_Click" runat="server" />
            &nbsp;
            <asp:Button ID="btnRefreshDynamicFormList" CssClass="button" Text="Refresh" OnClick="btnRefreshDynamicFormList_Click" runat="server" />
            <asp:CheckBox ID="chkIncludeInactiveForms" runat="server" Text="Show Inactive Forms" Checked="false" />
        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcManageDynamicForm" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Manage Dynamic Form</legend>
            <h3>Dynamic Form Configuration</h3>

            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Form Name</td>
                    <td>
                        <asp:TextBox ID="txtFormName" Visible="true" Width="325" CssClass="inputText" MaxLength="100" runat="server" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="errorMessage"
                            ErrorMessage="A File Name is required." EnableClientScript="false" runat="server"
                            ControlToValidate="txtFormName" ValidationGroup="DynamicFormInformation">Missing Form Name.</asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Form Description</td>
                    <td>
                        <asp:TextBox ID="txtFormDescription" Visible="true" Columns="75" Rows="8" TextMode="MultiLine" CssClass="inputText" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Submission Deadline</td>
                    <td>
                        <asp:TextBox ID="txtSubmissionDeadline" Visible="true" runat="Server" Width="200" CssClass="inputText" />
                        <cc1:CalendarExtender ID="calextSubmissionDeadline" TargetControlID="txtSubmissionDeadline" runat="server" />
                    </td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
            </table>
            <br />
            <center>
                <asp:Button ID="btnSaveDynamicForm" CssClass="button" Text="Save Form" ValidationGroup="DynamicFormInformation" OnClick="btnSaveDynamicForm_Click" runat="server" />
                <asp:HiddenField runat="server" ID="hdnDynamicFormId" Value="0" />&nbsp;
                <asp:LinkButton runat="server" ID="lbtnConfigureQuestions" OnClick="lbtnConfigureQuestions_Click" Text="Configure Questions..." />
            </center>
        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcFormQuestionList" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Form Question Manager</legend>
            <h3>Question List</h3>
            <uc:FormQuestionEditorList id="ucFormQuestionEditorList" runat="server" />

            <br />
            <br />
            <asp:Button ID="btnAddQuestion" CssClass="button" Text="Create Question" OnClick="btnAddQuestion_Click" runat="server" />
            &nbsp;
            <asp:Button ID="btnRefeshQuestionList" CssClass="button" Text="Refresh" OnClick="btnRefreshQuestionList_Click" runat="server" /> &nbsp;
            <asp:Button ID="btnReturnToFormList" CssClass="button" Text="Cancel" OnClick="btnReturnToFormList_Click" runat="server" />

        </fieldset>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plcManageQuestion" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Manage Question</legend>
            <h3>Question Configuration</h3>

            <uc:FormQuestionEditor id="ucFormQuestionEditor" runat="server" />
            <br />
            <center>
                <asp:Button ID="btnSaveQuestion" CssClass="button" Text="Save Question" ValidationGroup="QuestionInformation" OnClick="btnSaveQuestion_Click" runat="server" />
                 &nbsp;
                <asp:Button ID="btnReturnToQuestionList" Text="Cancel" CssClass="button" OnClick="btnReturnToQuestionList_Click" runat="server" />
            </center>
        </fieldset>
    </asp:PlaceHolder>
</asp:Content>
