<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModalDialog.aspx.cs" Inherits="ExpoOrders.Web.Owners.ModalDialog" %>
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


        function priceUpdatedCallBack(categoryId) {
            //create the argument that will be returned to the parent page
            var oArg = new Object();

            oArg.categoryId = categoryId;

            if (parent.refreshCategoryList) {
                parent.refreshCategoryList(categoryId);
            }

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


    <div id="container" style="width: 250px">
        <CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

        <fieldset class="commonControls">
            <legend class="commonControls"><asp:Literal ID="ltrModalDialogTitle" runat="server" /></legend>

            <asp:PlaceHolder ID="plcCascadingPriceUpdates" Visible="false" runat="server">
                <table border="0" cellpadding="1" cellspacing="2">
                    <tr>
                        <td class="contentLabelRight">Update Unit Price(s) by:</td>
                        <td><asp:TextBox ID="txtPercentPriceUpdate" CssClass="inputText" Width="35" MaxLength="5" runat="server" />%</td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Include Updates to:</td>
                        <td>&nbsp;</td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Advanced Price?</td>
                        <td valign="top">
                            <asp:RadioButtonList ID="rdoUpdateEarlyBirdPrice" CssClass="inputText" RepeatDirection="Horizontal" runat="server">
                                <asp:ListItem Value="1" Selected="True">Yes</asp:ListItem>
                                <asp:ListItem Value="0">No</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">Standard Price?</td>
                        <td valign="top">
                            <asp:RadioButtonList ID="rdoUpdateDiscountPrice" CssClass="inputText" RepeatDirection="Horizontal" runat="server">
                                <asp:ListItem Value="1" Selected="True">Yes</asp:ListItem>
                                <asp:ListItem Value="0">No</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </table>

                <asp:Button ID="btnSubmit" Text="Submit" CssClass="button" runat="server" OnClick="btnApplyPricingUpdate_Click" />

                <asp:HiddenField ID="hdnShowId" runat="server" />
                <asp:HiddenField ID="hdnCategoryId" runat="server" />
            </asp:PlaceHolder>

        </fieldset>
        
    </div>

    </form>
</body>
</html>
