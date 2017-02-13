<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DynamicReportViewer.ascx.cs" Inherits="ExpoOrders.Web.CustomControls.DynamicReportViewer" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<div>
    <rsweb:ReportViewer id="rptViewer"  Enabled="true" runat="server">
    </rsweb:ReportViewer>
</div>