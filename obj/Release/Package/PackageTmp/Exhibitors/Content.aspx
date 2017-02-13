<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Content.aspx.cs" Inherits="ExpoOrders.Web.Exhibitors.Content" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
        <title>ExpoOrders.com - Content</title>
        <script id="LibraryJS" type="text/javascript" src="<%# Page.ResolveUrl("~/Common/Library.js")%>" language="javascript"></script>
    
    <link id="ShowStyleSheet" rel="Stylesheet" type="text/css" />
</head>
<body id="Body" class="pageBody" runat="server">
    <form id="frmContent" runat="server">
        <div id="container">

        
        <div id="mainContent">
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Literal ID="ltrPageContent" runat="server" />
                    </td>
                </tr>
        </div>
        </div>
    </form>
</body>
</html>
