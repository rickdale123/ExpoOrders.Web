<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdmin/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="UserManager.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.UserManager" %>
<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>

<%@ Register Src="~/CustomControls/UserDetail.ascx" TagPrefix="uc" TagName="UserDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>User Manager</h1>

<asp:PlaceHolder ID="plcSelectUser" visible="false" runat="server">

    Users<br />
    <asp:Button ID="btnAddUser" Text="Add User" CssClass="button" OnClick="btnAddUser_Click" runat="server" />
    <asp:Label ID="lblUserRowCount" Text="" CssClass="techieInfo" runat="server" />
            <asp:GridView EnableViewState="true" OnRowDataBound="grdvUsers_RowDataBound"
                runat="server" ID="grdvUsers" AllowPaging="false" AllowSorting="true" OnRowCommand="grdvUsers_RowCommand"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="dataRowAlt"
                RowStyle-CssClass="dataRow" GridLines="Both"
                EmptyDataText="No users to display.">
                <Columns>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="UserName" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%#Convert.ToInt64(DataBinder.Eval(Container, "RowIndex")) + 1%>.&nbsp;
                            <asp:LinkButton Visible="true" ID="lbtnManageUser" Text='<%# DataBinder.Eval(Container.DataItem, "UserName")%>'
                                        CommandName="EditUser" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserId")%>' runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Owner">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "OwnerName")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Role" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "RoleName")%>

                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Name">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "FirstName")%>  <%# DataBinder.Eval(Container.DataItem, "LastName")%>

                        </ItemTemplate>
                    </asp:TemplateField>
                   
                </Columns>
            </asp:GridView>

</asp:PlaceHolder>

<asp:PlaceHolder ID="plcEditUser" visible="false" runat="server">
    
    User Type: <asp:DropDownList ID="ddlUserType" CssClass="inputText" runat="server" /><br />
    <uc:UserDetail ID="ucUserDetail" runat="server" /><br />
    User Roles:<asp:CheckBoxList ID="chkLstUserRoles" runat="server" /><br />
    Is Active? <asp:CheckBox ID="chkActive" runat="server" /><br />
    Locked Out? <asp:CheckBox ID="chkLockedOut" runat="server" /><br />
    Allowable Show Id List:<asp:TextBox ID="txtAllowableShowIdList" CssClass="inputText" runat="server" />(semi-colon delimited list of Ids)<br />
    <br />

    <asp:Button ID="btnSaveUser" Text="Save" CssClass="button" OnClick="btnSaveUser_Click" runat="server" />
    &nbsp&nbsp;
    <asp:Button ID="btnCancelUser" Text="Cancel" CssClass="button" OnClick="btnCancelUser_Click" runat="server" />
    &nbsp;&nbsp;
    <asp:Button ID="btnDeleteUser" Text="Delete" CssClass="button" OnClick="btnDeleteUser_Click" runat="server" />
</asp:PlaceHolder>

</asp:Content>
