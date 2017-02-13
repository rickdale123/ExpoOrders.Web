<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormQuestionEditor.ascx.cs" Inherits="ExpoOrders.Web.CustomControls.FormQuestionEditor" %>

<table border="0" width="100%" cellpadding="1" cellspacing="0">
    <tr>
        <td class="contentLabelRight">Sort Order</td>
        <td>
            <asp:TextBox ID="txtSortOrder" Visible="true" Width="25" CssClass="inputText" runat="server" />
        </td>
        <td style="width: 10%">&nbsp;</td>
    </tr>
    <tr>
        <td class="contentLabelRight">Question Text</td>
        <td>
            <asp:TextBox ID="txtQuestionText" Visible="true" TextMode="MultiLine" Columns="65" Rows="8" CssClass="inputText" MaxLength="200" runat="server" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" CssClass="errorMessage"
                ErrorMessage="Question Text is required." EnableClientScript="false" runat="server"
                ControlToValidate="txtQuestionText" ValidationGroup="QuestionInformation">Missing Question Text.</asp:RequiredFieldValidator>
        </td>
        <td style="width: 10%">&nbsp;</td>
    </tr>
    <tr>
        <td class="contentLabelRight">Response Type</td>
        <td>
            <asp:DropDownList ID="ddlResponseType" Visible="true" runat="Server" Width="200"
                CssClass="inputText">
                <asp:ListItem Text="-- select --" Value="-1" />
                <asp:ListItem Text="DropDown List" Value="dropdown" />
                <asp:ListItem Text="Checkbox" Value="checkbox" />
                <asp:ListItem Text="Checkbox List" Value="checkboxlist" />
                <asp:ListItem Text="Radio Button" Value="radiobtn" />
                <asp:ListItem Text="Textbox (single line)" Value="textbox" />
                <asp:ListItem Text="Textarea (multi-line)" Value="textarea" />
                <asp:ListItem Text="Content Area" Value="content" />
                
            </asp:DropDownList>
        </td>
        <td style="width: 10%">&nbsp;</td>
    </tr>
    <tr>
        <td class="contentLabelRight">Choices (Semi-colon delimited)<br />ex: Red;White;Blue</td>
        <td>
            <asp:TextBox ID="txtOptions" Visible="true" Columns="55" Rows="3" TextMode="MultiLine" CssClass="inputText" runat="server" />
        </td>
        <td style="width: 10%">&nbsp;</td>
    </tr>
    <tr>
        <td class="contentLabelRight">Required?</td>
        <td><asp:CheckBox ID="chkQuestionRequired" Visible="true" runat="Server" /></td>
        <td style="width: 10%">&nbsp;</td>
    </tr>
    <tr>
        <td class="contentLabelRight">Max Length</td>
        <td>
            <asp:TextBox ID="txtMaxLength" Visible="true" Width="200" CssClass="inputText" runat="server" />
        </td>
        <td style="width: 10%">&nbsp;</td>
    </tr>
    <tr>
        <td class="contentLabelRight">Default Value</td>
        <td>
            <asp:TextBox ID="txtDefaultValue" Visible="true" Width="200" CssClass="inputText" runat="server" />
        </td>
        <td style="width: 10%">&nbsp;</td>
    </tr>
    <tr>
        <td class="contentLabelRight">Data Type</td>
        <td>
            <asp:DropDownList ID="ddlDataType" Visible="true" Width="200" CssClass="inputText" runat="server">
                <asp:ListItem Text="-- select --" Value="-1" />
                <asp:ListItem Text="date" Value="date" />
                <asp:ListItem Text="number" Value="number" />
                <asp:ListItem Text="text" Value="text" />
            </asp:DropDownList>
        </td>
        <td style="width: 10%">&nbsp;</td>
    </tr>
</table>
<asp:HiddenField runat="server" ID="hdnQuestionId" Value="0" />
