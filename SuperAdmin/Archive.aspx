<%@ Page Title="Archive Tool" Language="C#" MasterPageFile="~/SuperAdmin/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="Archive.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.Archive" %>

<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

Show Id: <asp:TextBox ID="txtShowId" Runat="server" /><br />
<asp:Button ID="btnLoadShow" Text="Go" onclick="btnLoadShow_Click" runat="server" />
<hr />

<asp:PlaceHolder ID="plcShowDetail" Visible="false" runat="server">

<asp:Label ID="lblMessage" runat="server" />

Show Guid (Location): <asp:Label ID="lblGuidLocation" runat="server" /><br />
<br />
Upload folder size: <asp:Label ID="lblUploadSize" runat="server" /><br />
Assets Root: <asp:Label ID="lblAssetsRootSize" runat="server" /><br />
Attachments: <asp:Label ID="lblAttachmentSize" runat="server" /><br />
Downloads: <asp:Label ID="lblDownloadSize" runat="server" /><br />
Products: <asp:Label ID="lblProductSize" runat="server" /><br />
<hr />
Total Assets Folder Size: <asp:Label ID="lblTotalAssetsFolderSize" runat="server" /><br />

</asp:PlaceHolder>
</asp:Content>
