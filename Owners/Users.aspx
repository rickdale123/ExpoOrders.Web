<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnerLanding.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="ExpoOrders.Web.Owners.Users" %>
<%@ MasterType VirtualPath="~/Owners/OwnerLanding.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="~/CustomControls/UserDetail.ascx" TagPrefix="uc" TagName="UserDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

<CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

<asp:PlaceHolder id="plcUserList" Visible="false" runat="server">
    <fieldset class="commonControls">
        <legend class="commonControls">Current Users</legend>

        <asp:Button ID="btnAddUser" CssClass="button" Text="Add User" OnClick="btnAddUser_Click" runat="server" />

        <asp:GridView EnableViewState="true" ID="grdvwOwnerUserList"
            runat="server"  AllowPaging="false" AllowSorting="true"
            AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
            RowStyle-CssClass="item" OnRowDataBound="grdvwOwnerUserList_RowDataBound" GridLines="None"
            OnRowCommand="grdvwOwnerUserList_RowCommand" EmptyDataText="No users to display.">
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="First Name">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FirstName")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Last Name">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "LastName")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Username">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "ActualUserName")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Email">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "Email")%>
                     </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:LinkButton Visible="true" ID="lbtnManageUser" Text="Edit" CommandName="EditUser" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserId")%>' runat="server" />
                            &nbsp;|&nbsp;
                        <asp:LinkButton Visible="true" ID="lbtnDeleteUser" Text="Remove" CommandName="DeleteUser" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserId")%>' runat="server" />

                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

    </fieldset>
</asp:PlaceHolder>


<asp:PlaceHolder id="plcUserDetail" Visible="false" runat="server">
    <fieldset class="commonControls">
        <legend class="commonControls">User Detail</legend>

        <uc:UserDetail ID="ucUserDetail" runat="server" /><br />
        User Roles:<asp:CheckBoxList ID="chkLstUserRoles" runat="server" /><br />
        Allowable Show Ids <asp:TextBox ID="txtAllowableShowIdList" cssclass="inputText" runat="server" />
        &nbsp;<a id="lnkAllowableListHelp" href="#" onclick="return false;"><img id="Img1" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
        <span class="techieInfo">(Semi-colon delimited list of Show ids that are visible to the user Ex: 321;543;555)</span><br />
        <center>
            <asp:Button ID="btnSaveUser" CssClass="button" Text="Save" OnClick="btnSaveUser_Click" runat="server" />
            &nbsp;&nbsp;
            <asp:Button ID="btnCancelSaveUser" CssClass="button" Text="Cancel" OnClick="btnCancelSaveUser_Click" runat="server" />
        </center>

        
    </fieldset>

    <asp:HiddenField ID="hdnUserId" runat="server" />
</asp:PlaceHolder>


<telerik:RadToolTip runat="server" ID="rttAllowableShowsListHelp" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="lnkAllowableListHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(rttAllowableShowsListHelp, "AllowableShowsList")%>
    </telerik:RadToolTip>

</asp:Content>
