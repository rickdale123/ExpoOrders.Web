<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="ExpoOrders.Web.Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Oops, Sorry for the error.</title>

    <style type="text/css">
        body { }
        table.errorTable {border-width: 1; border: solid;}
        th{text-align: center; color:Red; font-size: large;}
        li {font-size: 9px; color:Gray; font-size: 8px;}
        .errorMessage { color: Gray; font-size: 10px;}
        .title { color: Blue; font-size: medium; }
     </style>

</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    <table class="errorTable" border="0" cellpadding="2" cellspacing="2" width="75%" align="center">
        <tr>
            <th>We're Sorry, an Application Exception has occurred and the system was unable to process your request properly.</th>
        </tr>
        <tr>
            <td><asp:Literal ID="ltrErrorMessage" runat="server" /></td>
        </tr>
        <tr>
            <td class="title">
                The system administrator has been notified of the error.  Please click the Back 
                button and then 'F5-Refresh' to try the request again, or contact your administrator for assistance: <a href="mailto:support@expoorders.com?subject=Application Exception">support@expoorders.com</a>
            </td>
        </tr>
        <tr>
            <td class="errorMessage">
                Message Details<br />
                <asp:Label CssClass="errorMessage" ID="lblErrorMessage" runat="server" />
            </td>
        </tr>
        
    </table>
    
    </div>
    </form>
</body>
</html>
