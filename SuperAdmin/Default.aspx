<%@ Page Title="" Language="C#" MasterPageFile="SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.Default" %>
<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Super Admin Menu</h1>

<ul>
    <li><a class="hyperLink" href="~/SuperAdmin/UserManager.aspx" runat="server">User Management</a></li>
    <li><a id="A3" class="hyperLink" href="~/SuperAdmin/OwnerConfig.aspx" runat="server">Owner Config</a></li>
    <li><a id="A4" class="hyperLink" href="~/SuperAdmin/ShowStats.aspx" runat="server">Show Stats</a></li>
    <li><a id="A1" class="hyperLink" href="~/SuperAdmin/EmailLog.aspx" runat="server">Email Logs</a></li>
    <li><a id="A6" class="hyperLink" href="~/SuperAdmin/Transactions.aspx" runat="server">Transaction Logs</a></li>
    <li><a id="A2" class="hyperLink" href="~/SuperAdmin/SessionLog.aspx" runat="server">Session Logs</a></li>
    <li><a id="A9" class="hyperLink" href="~/SuperAdmin/HelpContent.aspx" runat="server">Help Content</a></li>
    <li><a id="A5" class="hyperLink" href="~/SuperAdmin/Utils.aspx" runat="server">Utilities</a></li>
    <li><a id="A7" class="hyperLink" href="~/SuperAdmin/ServiceMgr.aspx" runat="server">Service Mgr</a></li>
    <li><a id="A8" class="hyperLink" href="~/SuperAdmin/Sql.aspx" runat="server">SQL</a></li>
    <li><a id="A10" class="hyperLink" href="~/SuperAdmin/Archive.aspx" runat="server">Archive</a></li>
    <li><a class="hyperLink" href="#" runat="server">Site Admin</a></li>
    <li><a class="hyperLink" href="#" runat="server">Run Reports</a></li>
</ul>
</asp:Content>
