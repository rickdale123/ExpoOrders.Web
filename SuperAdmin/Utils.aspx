<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdmin/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="Utils.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.Utils" %>
<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="plcFunctions" Visible="true" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Adhoc - Utils</legend>

            <h1>Change Show Owner</h1>
            Show: <asp:DropDownList ID="ddlShowId" CssClass="inputText" runat="server" /><br />
            New Owner:<asp:DropDownList ID="ddlOwnerId" CssClass="inputText" runat="server" /><br />

            <asp:Button ID="btnUpdateShowOwner" Text="Update OwnerId" CssClass="button" OnClick="btnUpdateShowOwner_Click" runat="server" />

               <hr />

            <asp:Button ID="btnThrowException" Text="Throw Exception, send email" CssClass="button" OnClick="btnThrowException_Click" runat="server" />

            
        </fieldset>
    </asp:PlaceHolder>
</asp:Content>

