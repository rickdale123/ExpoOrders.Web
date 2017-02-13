<%@ Page Title="User Login" Language="C#" MasterPageFile="DefaultMaster.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ExpoOrders.Web.Login" %>
<%@ MasterType VirtualPath="~/DefaultMaster.Master" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderContent" runat="server">

    <script language="javascript" type="text/javascript">
        function focusLogin() {
            try {
                var loginBox = document.getElementById('LoginBodyContent_ExpoOrdersLogin_UserName');
                if (loginBox)
                    loginBox.focus();
            }
            catch (e)
            { }
        }
    </script>

</asp:Content>
<asp:Content ID="LoginBodyContent" ContentPlaceHolderID="LoginBodyContent" runat="server">

    <asp:PlaceHolder ID="plcLogin" Visible="true" runat="server">
        <table class="contentContainer" align="center">
            <tr>
                <td align="center" valign="top">
                    <asp:PlaceHolder ID="plcLoginArea" Visible="false" runat="server">
                        <asp:Login ID="ExpoOrdersLogin" runat="server" CssClass="loginArea" FailureText="Bad User Name and/or Password" UserNameRequiredErrorMessage="User Name is required." PasswordRequiredErrorMessage="Password is required." 
                            OnAuthenticate="ExpoOrdersLogin_Authenticate" DisplayRememberMe="false" RememberMeSet="false">
                            
                                <HyperLinkStyle CssClass="loginLink" />
                            
                                <ValidatorTextStyle ForeColor="Red" />
                                <FailureTextStyle CssClass="errorMessage" />
                                <InstructionTextStyle CssClass="loginInstructions"/>
                                <LoginButtonStyle CssClass="submitButton"/>
                                <TextBoxStyle CssClass="input-text" Width="200"/>
                                <TitleTextStyle CssClass="loginTitle" />
                                <LabelStyle CssClass="labelNoWrap" />
                                <CheckBoxStyle CssClass="check" />
                           </asp:Login>

                           <asp:LinkButton ID="lnkBtnForgotPassword" CssClass="loginLink" Text="Forgot Your Password?" OnClick="lnkBtnForgotPassword_Click" runat="server" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="plcPasswordReminder" Visible="false" runat="server">

                        <asp:Label ID="lblPasswordError" CssClass="errorMessageBlock" runat="server" />
                        <br />
                        <asp:Label ID="lblEmailAddress" CssClass="label" Text="Email Address" runat="server" /> <asp:TextBox ID="txtEmailAddress" CssClass="inputText" Width="200" MaxLength="255" runat="server" />

                        <br />

                        <asp:Button ID="btnSendPassword" CssClass="button" Text="Send Password" OnClick="btnSendPassword_Click" runat="server" />
                        <br /><br />
                        <asp:LinkButton ID="lnkBtnLogin" CssClass="loginLink" Text="Back to Login" OnClick="lnkBtnLogin_Click" runat="server" />
                    </asp:PlaceHolder>
                </td>
            </tr>
        </table>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="plcOwnerShowList" Visible="false" runat="server">
        <table class="showListingContainer" border="0" align="center">
            <tr>
                <td align="center" valign="top">
                    <asp:Repeater ID="rptrOwnerShowList" OnItemDataBound="rptrOwnerShowList_ItemDataBound" runat="server">
                        <HeaderTemplate>
                            <table id="showListing" border="0">
                                <tr>
                                    <td colspan="4"><div class="showListingText"><asp:Literal ID="ownerShowListingText" Text='<%#CurrentOwnerShowListingText %>' runat="server"/></div></td>
                                </tr>
                                <tr>
                                    <th>Show Name</th>
                                    <th>Quick Facts</th>
                                    <th>Dates</th>
                                    <th>Location</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                                <tr>
                                    <td class="showName" width="50">
                                        <a id="lnkShowLogin" class="showNameLink" runat="server"><%#DataBinder.Eval(Container.DataItem, "ShowName")%></a>
                                    </td>
                                    <td><a id="lnkQuickFactsFile" class="showPdfLink" target="_blank" runat="server">View PDF</a></td>
                                    <td class="showDates"><%#Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "ShowDatesDisplay").ToString())%></td>
                                    <td class="showLocation">
                                        <%#Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "VenueName").ToString())%>
                                        <br />
                                        <%#Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "VenueLocation").ToString())%>
                                    </td>
                                </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </td>
            </tr>
        </table>
    </asp:PlaceHolder>

</asp:Content>
