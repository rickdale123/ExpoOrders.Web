<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HtmlPreviewer.aspx.cs" Inherits="ExpoOrders.Web.Owners.HtmlPreviewer" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="pageHead" runat="server">
    <title>ExpoOrders - Html Preview</title>
    <link id="OwnerStyleSheet" href="Styles/style.css" rel="Stylesheet" type="text/css" />
</head>
<body class="pageBody">
    <form id="mainForm" runat="server">
    <telerik:RadScriptManager ID="ScriptManager1" runat="server" />
        <div id="container">
        <fieldset class="commonControls">
            <legend class="commonControls">Email Preview</legend>
                <table width="800" border="0" cellpadding="2" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">To:</td>
                        <td>
                            <asp:Label ID="lblEmailTo" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">From:</td>
                        <td>
                            <asp:Label ID="lblEmailFrom" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Reply-To:</td>
                        <td>
                            <asp:Label ID="lblEmailReplyTo" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Subject:</td>
                        <td>
                            <asp:Label ID="lblEmailSubject" runat="server" />
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Body:</td>
                        <td colspan="2">
                            <telerik:radeditor ID="htmlContentEditor" ContentAreaMode="Div" CssClass="htmlEditorContent" Enabled="false" runat="server" 
                                Skin="Outlook" Width="95%" ToolbarMode="ShowOnFocus">
                            </telerik:radeditor>
                        </td>
                    </tr>
                
                     <tr>
                        <td colspan="3" align="center">
                            <input type="button" class="button" value="Close Window" onclick="javascript: window.close();" />
                        </td>
                    </tr>
                </table>

            </fieldset>
        </div>
    </form>
</body>
</html>
