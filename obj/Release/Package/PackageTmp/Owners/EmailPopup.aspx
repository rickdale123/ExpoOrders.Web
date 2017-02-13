<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailPopup.aspx.cs" ValidateRequest="false" Inherits="ExpoOrders.Web.Owners.EmailPopup" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="pageHead" runat="server">
    <title>ExpoOrders - Send Email</title>
    <link id="OwnerStyleSheet" href="Style/owner.css" rel="Stylesheet" type="text/css" />
    <link href="../Style/CommonAdmin.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript">
        function launchEmailPreview() {
            var newWindow = window.open('../Owners/HtmlPreviewer.aspx', 'emailPreview', 'height=500, width=800,location=0,status=1,scrollbars=1,resizable=yes');
            if (window.focus && newWindow) {
                newWindow.focus();
            }
        }
    </script>
</head>
<body class="pageBody">
    <form id="mainForm" runat="server">
        <telerik:RadScriptManager ID="ScriptManager1" runat="server" />
    <div id="container" style="width: 100%">
            <table width="800" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td style="white-space: nowrap;">
                        <CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />
                        
                        <asp:PlaceHolder ID="plcFriendlyMessage" Visible="false" runat="server">
                            <table class="friendlyMessage" border="0" width="100%" cellpadding="3" cellspacing="0">
                                <tr>
                                    <td class="friendlyMessage">
                                        <asp:Literal ID="ltrFriendlyMessage" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </asp:PlaceHolder>

                        <CustomControls:ValidationErrors ID="validationErrors" CssClass="errorMessageBlock" runat="server" />

                        <asp:PlaceHolder ID="plcEmailMessage" runat="server">
                            <fieldset class="commonControls">
                                <legend class="commonControls">
                                    <asp:Literal runat="server" ID="ltrControlTitle" Text="Send Email"></asp:Literal></legend>
                                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                                    <tr>
                                        <td class="contentLabelRight">To:</td>
                                        <td>
                                            <asp:TextBox ID="txtToAddress" Visible="true" Width="400" CssClass="inputText" runat="server" />
                                            <br />
                                            <asp:LinkButton ID="lnkBtnViewUsers" Text="View Recipients" OnClick="lnkBtnViewUsers_Click" runat="server" /> 
                                        </td>
                                        <td style="width: 10%">&nbsp;</td>
                                    </tr>
                                    <asp:PlaceHolder ID="plcEmailRecipients" Visible="false" runat="server">
                                        <tr>
                                            <td class="contentLabelRight">Recipients:</td>
                                            <td>
                                                <asp:Repeater ID="rptrRecipientList" OnItemDataBound="rptrRecipientList_ItemDataBound" runat="server">
                                                    <HeaderTemplate>
                                                        <table border="0" cellpadding="1" cellspacing="0">
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                            <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="chkRecipient" runat="server" />
                                                                    <asp:HiddenField ID="hdnRecipientValue" runat="server" />
                                                                </td>
                                                                <td><asp:Label ID="lblRecipientName" CssClass="colData" runat="server" /></td>
                                                                <td>&nbsp;
                                                                    <asp:LinkButton ID="lnkPreview" Text="preview" CommandName="Preview" OnClick="lnkPreview_Click" runat="server" />
                                                                </td>
                                                            </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </table>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </td>
                                            <td style="width: 10%">&nbsp;</td>
                                        </tr>
                                    </asp:PlaceHolder>
                                    <tr>
                                        <td class="contentLabelRight">From:</td>
                                        <td>
                                            <asp:TextBox ID="txtFromAddress" Visible="true" Width="400" CssClass="inputText"
                                                runat="server" />
                                            <asp:RequiredFieldValidator ID="reqValFromAddress" CssClass="errorMessage" ErrorMessage="At least one From Address is required."
                                                EnableClientScript="false" runat="server" ControlToValidate="txtFromAddress"
                                                ValidationGroup="EmailInformation">Missing From Address</asp:RequiredFieldValidator>
                                        </td>
                                        <td style="width: 10%">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="contentLabelRight">
                                            ReplyTo:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtReplyToAddress" Visible="true" Width="400" CssClass="inputText" runat="server" />
                                        </td>
                                        <td style="width: 10%">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="contentLabelRight">Choose Email Template:</td>
                                        <td>
                                            <asp:DropDownList ID="ddlEmailTemplate" Enabled="true" Visible="true" CssClass="inputText" AutoPostBack="true" OnSelectedIndexChanged="ddlEmailTemplate_IndexChanged" runat="server" />
                                        </td>
                                        <td style="width: 10%">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="contentLabelRight">Tokens:</td>
                                        <td>
                                            <asp:TextBox ID="txtTokens" Enabled="true" Visible="true" Width="400" TextMode="MultiLine" Rows="4" CssClass="inputText" runat="server" />
                                        </td>
                                        <td style="width: 10%">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="contentLabelRight">
                                            Subject
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtSubject" Visible="true" Width="400" CssClass="inputText" MaxLength="200" runat="server" />
                                        </td>
                                        <td style="width: 10%">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="contentLabel" colspan="3">
                                            Message<br />
                                            <telerik:radeditor ID="htmlContentEditor" ContentFilters="MakeUrlsAbsolute" Width="95%" ContentAreaMode="Iframe" EnableResize="false" CssClass="htmlEditorContent" EditModes="Design, Html" runat="server" Skin="Outlook">
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
                                    <tr>
                                        <td class="contentLabelRight">
                                            Attachment(s)
                                        </td>
                                        <td>
                                            <asp:Repeater ID="rptrAttachmentList" OnItemDataBound="rptrAttachmentList_DataBound" Visible="false" runat="server">
                                                <ItemTemplate>
                                                    <a id="lnkFileAttachment" target="_blank" runat="server" />&nbsp;
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </td>
                                        <td style="width: 10%">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td colspan="3"><hr /></td>
                                    </tr>
                                    <tr>
                                        <td class="contentLabelRight">Attach New File: </td>
                                        <td><asp:FileUpload ID="fupFileUpload" runat="server" /><asp:Button ID="btnUploadAttachment" Text="Attach File" CssClass="button" OnClick="btnUploadAttachment_Click" runat="server" /></td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </table>
                                <br />
                                <center>
                                    <asp:Button ID="btnSendEmail" CssClass="button" Text="Send Email" OnClick="btnSendEmail_onclick" ValidationGroup="EmailInformation" runat="server" />
                                    <asp:HiddenField ID="hdnEditorMode" Value="0" runat="server" />
                                </center>
                            </fieldset>

                        </asp:PlaceHolder>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
