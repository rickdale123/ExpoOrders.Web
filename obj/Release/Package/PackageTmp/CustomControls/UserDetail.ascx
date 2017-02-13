<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserDetail.ascx.cs" Inherits="ExpoOrders.Web.CustomControls.UserDetail" %>
    <table border="0" width="100%" cellpadding="1" cellspacing="0">
        <tr>
            <td class="contentLabelRight">Title</td>
            <td>
                <asp:TextBox ID="txtUserTitle" Width="200" CssClass="inputText" MaxLength="50" runat="server" />
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">First Name</td>
            <td class="contentLabel">
                <asp:TextBox ID="txtUserFirstName" Width="125" CssClass="inputText" MaxLength="50" runat="server" />
                &nbsp; Last Name&nbsp;
                <asp:TextBox ID="txtUserLastName" Width="125" CssClass="inputText" MaxLength="50" runat="server" />
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">*UserName</td>
            <td>
                <asp:TextBox ID="txtUserName" Width="200" CssClass="inputText" MaxLength="50" Visible="true" runat="server" />
                <asp:RequiredFieldValidator ID="reqUserName" CssClass="errorMessage"
                            ErrorMessage="UserName is Required" EnableClientScript="false" runat="server"
                            ControlToValidate="txtUserName">Missing UserName</asp:RequiredFieldValidator>
                <asp:Label ID="lblUserName" Visible="false" runat="server" />
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">*Password</td>
            <td>
                <asp:TextBox ID="txtUserPassword" Width="200" CssClass="inputText" MaxLength="50" runat="server" />
                <asp:RequiredFieldValidator ID="reqPassword" CssClass="errorMessage"
                            ErrorMessage="Password is Required" EnableClientScript="false" runat="server"
                            ControlToValidate="txtUserPassword">Missing Password</asp:RequiredFieldValidator>
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">*Email</td>
            <td>
                <asp:TextBox ID="txtUserEmailAddress" Width="275" CssClass="inputText" MaxLength="50" runat="server" />
                <asp:RequiredFieldValidator ID="reqUserEmailAddress" CssClass="errorMessage"
                            ErrorMessage="Email is Required" EnableClientScript="false" runat="server"
                            ControlToValidate="txtUserEmailAddress">Missing Email</asp:RequiredFieldValidator>
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
        <tr>
            <td class="contentLabelRight">Phone</td>
            <td>
                <asp:TextBox ID="txtUserPhone" Width="175" CssClass="inputText" MaxLength="50" runat="server" /><span class="informational">(xxx) xxx-xxxx ext. xxxxx</span>
            </td>
            <td style="width: 10%">&nbsp;</td>
        </tr>
    </table>

    <input type="hidden" id="hdnUserId" runat="server" />
    <input type="hidden" id="hdnIsPrimary" runat="server" />
    <input type="hidden" id="hdnActualUserName" runat="server" />
