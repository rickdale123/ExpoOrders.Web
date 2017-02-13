<%@ Page Title="" Language="C#" MasterPageFile="~/DefaultMaster.Master" AutoEventWireup="true" CodeBehind="Selector.aspx.cs" Inherits="ExpoOrders.Web.Selector" %>
<%@ MasterType VirtualPath="~/DefaultMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LoginBodyContent" runat="server">


<asp:PlaceHolder ID="plcShowSelector" Visible="false" runat="server">
    
    <asp:Repeater ID="rptrAvailableShows" runat="server">
        <HeaderTemplate>
            <h1>List of Shows you are invited to:</h1>
        </HeaderTemplate>
        <ItemTemplate>
           <asp:LinkButton ID="lnkShowId" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ShowId")%>' OnClick="lnkShowSelector_Click" runat="server">
                          <%#DataBinder.Eval(Container.DataItem, "ShowName")%>
           </asp:LinkButton>
           &nbsp;<%#DataBinder.Eval(Container.DataItem, "StartDate")%><br />
        </ItemTemplate>
    </asp:Repeater>
    
</asp:PlaceHolder>
    

</asp:Content>
