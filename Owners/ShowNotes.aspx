<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowNotes.aspx.cs" Inherits="ExpoOrders.Web.Owners.ShowNotes" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="pageHead" runat="server">
    <title>ExpoOrders - Dialog</title>
    <link id="OwnerStyleSheet" href="Styles/style.css" rel="Stylesheet" type="text/css" />
    <script id="LibraryJS" type="text/javascript" src="../Common/Library.js" language="javascript"></script>

    <script language="javascript" type="text/javascript">

        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }


        function saveNoteCallBack(showId) {
            //create the argument that will be returned to the parent page
            var oArg = new Object();

            oArg.showId = showId;

            //get a reference to the current RadWindow
            var oWnd = GetRadWindow();
            oWnd.close(oArg);

        }

        function closeMe() {
            GetRadWindow().close(null);
        }


    </script>
</head>
<body class="pageBody">
    <form id="mainForm" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server" />
    <telerik:RadFormDecorator ID="RadFormDecorator1" DecoratedControls="All" runat="server" />

    <div id="container" style="width: 100%">

        <fieldset class="commonControls">
            <legend class="commonControls">Internal Notes: <asp:Literal ID="ltrShowName" runat="server" /></legend>

            <asp:PlaceHolder ID="plcShowNotes" Visible="true" runat="server">
                
                <asp:TextBox ID="txtOwnerNotes" TextMode="MultiLine" CssClass="inputText" Columns="45" Rows="3" runat="server" />

                <asp:Button ID="btnSubmit" Text="Add Your Note" CssClass="button" runat="server" OnClick="btnSaveNotes_Click" />

                <hr />
                <p>
                    <asp:Literal ID="lblOwnerNotes" runat="server" />
                </p>


                <hr />
                <asp:LinkButton ID="lnkClearOwnerNotes" Text="Clear All Notes" CssClass="action-link" runat="server" OnClick="btnClearOwnerNotes_Click" />
            </asp:PlaceHolder>

            <asp:HiddenField ID="hdnShowId" runat="server" />
        </fieldset>

        <asp:PlaceHolder ID="plcShowNoteFiles" runat="server">
            <fieldset class="commonControls">
                <legend class="commonControls">Attachments: </legend>

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
                    
                    </Columns>
                </asp:GridView>
            </fieldset>

        </asp:PlaceHolder>
        
    </div>

    </form>
</body>
</html>
