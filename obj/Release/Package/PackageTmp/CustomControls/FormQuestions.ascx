<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormQuestions.ascx.cs" Inherits="ExpoOrders.Web.CustomControls.FormQuestions" %>

            <span class="informational">An asterisk (<span class="requiredFieldIndicator">*</span>) denotes a required field</span>

                <table border="0" cellpadding="2" cellspacing="0">
                    <asp:Repeater ID="rptrDynamicForm" runat="server" OnItemDataBound="rptrDynamicForm_ItemDataBound">
                        <ItemTemplate>
                                <tr id="trQuestion" visible="false" runat="server">
                                    <td class="contentLabel" valign="top" align="right">
                                        <asp:label ID="lblRequired" runat="server" Visible = "false" Text="*" CssClass="requiredFieldIndicator"/>
                                        <asp:label runat="server" ID="lblQuestion" Text=""/>
                                    </td>
                                    <td valign="top">
                                        <asp:TextBox  ID="txtResponse" Visible="false" runat="server" CssClass="inputText" Width="200" maxlength="100" /><asp:RequiredFieldValidator EnableViewState="true" ID="reqValResponseTxt" runat="server" ControlToValidate="txtResponse" Visible="false" Enabled="true" CssClass="requiredFieldIndicator" ></asp:RequiredFieldValidator>
                                        <asp:RadioButtonList  ID="rbtnListResponse" Visible="false" runat="server" /><asp:RequiredFieldValidator  ID="reqValResponseRbtn" EnableViewState="true" runat="server" ControlToValidate="rbtnListResponse" Visible="false" Enabled="true" CssClass="requiredFieldIndicator" ></asp:RequiredFieldValidator>
                                        <asp:CheckBoxList ID="chkListResponse" Visible="false" runat="server" />
                                        <asp:CheckBox ID="chkSingleResponse" Visible="false" runat="server" />
                                        <asp:DropDownList  ID="ddlResponse" Visible="false" runat="server" /><asp:RequiredFieldValidator  ID="reqValResponseDDL" EnableViewState="true" runat="server" ControlToValidate="ddlResponse" Visible="false" Enabled="true" CssClass="requiredFieldIndicator" ></asp:RequiredFieldValidator>
                                        <cc1:CalendarExtender ID="calExtender" TargetControlID="txtResponse" runat="server" />
                                    </td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr id="trQuestionContentArea" visible="false" runat="server">
                                    <td colspan="2" valign="top"><asp:Literal ID="ltrQuestionContent" runat="server" /></td>
                                    <td>&nbsp;</td>
                                </tr>
                                
                        </ItemTemplate>
                    </asp:Repeater>
            </table>