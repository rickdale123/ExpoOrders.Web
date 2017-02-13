<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdmin/SuperAdminMaster.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Sql.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.Sql" %>
<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<fieldset class="commonControls">
            <legend class="commonControls">Sql Admin</legend>

            Sql:<br /><asp:TextBox ID="txtSql" TextMode="MultiLine" CssClass="inputText" Columns="55" Rows="5" runat="server" />
            <asp:Button ID="btnRunSql" Text="Run Sql" OnClick="btnRunSql_Click" runat="server" />
            <hr />
            <asp:Label ID="lblRowCount" Text="" CssClass="techieInfo" runat="server" />
            <asp:GridView EnableViewState="true" 
                runat="server" ID="grdvSqlRows" AllowPaging="false" AllowSorting="false"
                AutoGenerateColumns="true" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="dataRowAlt"
                RowStyle-CssClass="dataRow" GridLines="Both"
                EmptyDataText="No rows to display.">
            </asp:GridView>
        </fieldset>

</asp:Content>
