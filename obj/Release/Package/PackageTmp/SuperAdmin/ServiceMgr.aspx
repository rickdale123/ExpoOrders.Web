<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdmin/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="ServiceMgr.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.ServiceMgr" %>
<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    domain: <asp:TextBox ID="txtDomain" Text="" CssClass="inputText" runat="server" /><br />
    userId: <asp:TextBox ID="txtUserId" Text="admin" CssClass="inputText" runat="server" /><br />
    pwd: <asp:TextBox ID="txtPassword" TextMode="Password" CssClass="inputText" runat="server" /><br />
    
    host address: <asp:TextBox ID="txtHostAddress" Text="localhost" CssClass="inputText" runat="server" /><br />
    Filter by Service Name: <asp:TextBox ID="txtFilterServiceName" Text="SQL Server Reporting Services" CssClass="inputText" runat="server" />
    <asp:Button ID="btnLoadServiceList" Text="Load Service List" OnClick="btnLoadServiceList_Click" runat="server" /><br />
    <hr />
    
     <asp:GridView EnableViewState="true" OnRowDataBound="grdvServices_RowDataBound" OnRowCommand="grdvServices_RowCommand"
                runat="server" ID="grdvServices" AllowPaging="false" AllowSorting="true"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="dataRowAlt"
                RowStyle-CssClass="dataRow" GridLines="Both"
                EmptyDataText="No Logs to display.">
        <Columns>
        
            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Service Name">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "ServiceName")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Description">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "Description")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Status">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "Status")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="StartMode">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container.DataItem, "StartMode")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Commands">
                <ItemTemplate>
                    <asp:Button ID="btnStartService" Text="Start" CommandArgument='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "ServiceName").ToString())%>' CommandName="Start" runat="server" />
                    <asp:Button ID="btnRestartService" Text="Restart" CommandArgument='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "ServiceName").ToString())%>' CommandName="Restart" runat="server" />
                    <asp:Button ID="btnStopService" Text="Stop" CommandArgument='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "ServiceName").ToString())%>' CommandName="Stop" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
