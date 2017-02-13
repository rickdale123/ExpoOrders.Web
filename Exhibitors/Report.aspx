<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="ExpoOrders.Web.Exhibitors.Report" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>ExpoOrders</title>
</head>
<body>
    <form id="frmReport" runat="server">
     <div>
        <rsweb:ReportViewer Visible="false" ID="viewReportViewer" runat="server">
        </rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
