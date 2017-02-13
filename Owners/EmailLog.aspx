<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailLog.aspx.cs" Inherits="ExpoOrders.Web.Owners.EmailLog" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="pageHead" runat="server">
    <title>ExpoOrders - Email Log</title>
    <link id="OwnerStyleSheet" href="Style/owner.css" rel="Stylesheet" type="text/css" />
    <script id="LibraryJS" type="text/javascript" src="../Common/Library.js" language="javascript"></script>

    <script language="javascript" type="text/javascript">
        function displayEmailBody(emailId) {

            hideObject('displayEmailBody_' + emailId);
            showObject('hideEmailBody_' + emailId);
            showObject('emailBody_' + emailId);
        }

        function hideEmailBody(emailId) {
            showObject('displayEmailBody_' + emailId);
            hideObject('hideEmailBody_' + emailId);
            hideObject('emailBody_' + emailId);
        }
    </script>
</head>
<body class="pageBody">
    <form id="mainForm" runat="server">
    <telerik:RadScriptManager ID="ScriptManager1" runat="server" />
    <div id="container" style="width: 100%">

        <fieldset class="commonControls">
            <legend class="commonControls">Email Logs</legend>
            <a href="#" onclick="window.close();" class="action-link" style="float: right;">[x] - Close</a>

            <asp:Label ID="lblNoEmails" CssClass="friendlyMessage" Text="No Emails to view" Visible="false" runat="server" />

            <asp:Repeater ID="rptrEmailLog" OnItemDataBound="rptrEmailLog_ItemDataBound" runat="server">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <table border="0" cellpadding="1" cellspacing="2">
                        <tr>
                            <td class="contentLabelRight">Date To Send:</td>
                            <td><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "SendDate").ToString())%></td>
                            <td style="width: 10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Status:</td>
                            <td id="tdEmailStatus" runat="server"><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "StatusDescription").ToString())%></td>
                            <td style="width: 10%">&nbsp;</td>
                        </tr>
                        <tr id="trProcessingDetails" visible="false" runat="server">
                            <td class="contentLabelRight">Processing Details:</td>
                            <td class="techieInfo"><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "ProcessingDetails").ToString())%></td>
                            <td style="width: 10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Date Sent:</td>
                            <td><%# DataBinder.Eval(Container.DataItem, "DateSent")%></td>
                            <td style="width: 10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">To:</td>
                            <td><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "EmailRecipient").ToString())%></td>
                            <td style="width: 10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">From:</td>
                            <td><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "FromAddress").ToString())%></td>
                            <td style="width: 10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Reply To:</td>
                            <td><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "ReplyAddress").ToString())%></td>
                            <td style="width: 10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Subject:</td>
                            <td><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "Subject").ToString())%></td>
                            <td style="width: 10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Attachment(s):</td>
                            <td><asp:PlaceHolder ID="plcAttachments" runat="server" /></td>
                            <td style="width: 10%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="contentLabelRight">Body:</td>
                            <td>
                                <a name='email_<%# DataBinder.Eval(Container.DataItem, "EmailId")%>' />
                                <a id='displayEmailBody_<%# DataBinder.Eval(Container.DataItem, "EmailId")%>' title='EmailId: <%# DataBinder.Eval(Container.DataItem, "EmailId")%>' style="display: block;" href='#email_<%# DataBinder.Eval(Container.DataItem, "EmailId")%>' onclick='displayEmailBody(<%# DataBinder.Eval(Container.DataItem, "EmailId")%>)'>[ + View]</a>
                                <a id='hideEmailBody_<%# DataBinder.Eval(Container.DataItem, "EmailId")%>' title='EmailId: <%# DataBinder.Eval(Container.DataItem, "EmailId")%>' style="display: none;" href='#email_<%# DataBinder.Eval(Container.DataItem, "EmailId")%>' onclick='hideEmailBody(<%# DataBinder.Eval(Container.DataItem, "EmailId")%>)'>[ - Hide]</a><br />
                                
                                <div id='emailBody_<%# DataBinder.Eval(Container.DataItem, "EmailId")%>' style="display: none; border: 1px solid silver; width: 100%">
                                    <telerik:radeditor ID="htmlContentEditor" ContentAreaMode="IFrame" EnableResize="false" CssClass="htmlEditorContent" Enabled="false" runat="server" 
                                        Skin="Outlook" Width="95%" ToolbarMode="ShowOnFocus">

                                        <CssFiles>
                                            <telerik:EditorCssFile Value="/Style/CommonAdmin.css" />
                                        </CssFiles>
                                    </telerik:radeditor>
                                </div>
                            </td>
                            <td style="width: 10%">&nbsp;</td>
                        </tr>
                    </table>
                    <hr />
                </ItemTemplate>
                <FooterTemplate>
                
                </FooterTemplate>
            </asp:Repeater>

        </fieldset>
        
    </div>

    </form>
</body>
</html>
