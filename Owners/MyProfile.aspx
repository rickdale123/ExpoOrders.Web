<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnerLanding.Master" AutoEventWireup="true" CodeBehind="MyProfile.aspx.cs" Inherits="ExpoOrders.Web.Owners.MyProfile" %>
<%@ MasterType VirtualPath="~/Owners/OwnerLanding.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

<CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

<asp:PlaceHolder id="plcChangePassword" Visible="false" runat="server">
    <fieldset class="commonControls">
        <legend class="commonControls">Change Password</legend>

        <table border="0" width="100%" cellpadding="0">
            <tr>
                <td align="right">Password</td>
                <td>
                    <asp:TextBox ID="txtPassword1" Width="175" CssClass="inputText" TextMode="Password" MaxLength="50" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="txtPassword1" CssClass="errorMessage" ErrorMessage="Enter a password" EnableClientScript="false" ValidationGroup="PasswordChange" runat="server">New Password is required</asp:RequiredFieldValidator>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td align="right">Confirm Password</td>
                <td>
                    <asp:TextBox ID="txtPassword2" Width="175" CssClass="inputText" TextMode="Password" MaxLength="50" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtPassword2" CssClass="errorMessage" ValidationGroup="PasswordChange" ErrorMessage="Confirm the password" EnableClientScript="false" runat="server">Password confirmation is required</asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="CustomValidator1" ControlToValidate="txtPassword2" CssClass="errorMessage" ValidationGroup="PasswordChange" EnableClientScript="false" runat="server"></asp:CustomValidator>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>

        </table>
        
        <center>
            <asp:Button ID="btnSavePassword" CssClass="button" Text="Save Password" OnClick="btnSavePassword_Click" ValidationGroup="PasswordChange" runat="server" />
        </center>

        
    </fieldset>
</asp:PlaceHolder>
</asp:Content>
