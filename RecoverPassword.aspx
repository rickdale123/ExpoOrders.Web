<%@ Page Title="" Language="C#" MasterPageFile="~/DefaultMaster.Master" AutoEventWireup="true" CodeBehind="RecoverPassword.aspx.cs" Inherits="ExpoOrders.Web.RecoverPassword" %>
<%@ MasterType VirtualPath="~/DefaultMaster.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LoginBodyContent" runat="server">
    <br />
    <table class="loginContainer" align="center">
        <tr>
            <td width="285" valign="top">

                <asp:PasswordRecovery ID="ExpoOrdersPasswordRecovery" runat="server"
                    CssClass="loginArea"
                    SuccessPageUrl=""
                    SuccessText="Your password has been emailed to the email address on file. <a href='login.aspx'> Login </a>" 
                    UserNameTitleText="Password Reset">
                    <SubmitButtonStyle CssClass="submitButton" />
        
                    <HyperLinkStyle CssClass="loginLink"/>
                                <ValidatorTextStyle CssClass="errorMessage" />
                                <FailureTextStyle CssClass="errorMessage" />
                                <InstructionTextStyle CssClass="loginInstructions"/>
                  
                                <TextBoxStyle CssClass="input-text" Width="150"/>
                                <TitleTextStyle CssClass="loginTitle" />
                                <LabelStyle CssClass="label" />
                    
                    <MailDefinition From="ExpoOrdersAdmin-NoReply@expoorders.com" IsBodyHtml="true" 
                        Subject="Your Password has been reset. " BodyFileName="~/Templates/PasswordResetSuccess.txt" >
                    </MailDefinition>
                    <SuccessTextStyle />
                    <TitleTextStyle />
                </asp:PasswordRecovery>

            <a class="loginLink" href="Login.aspx">Back to Login page</a>
            </td>
        </tr>
    </table>
<br /><br />
</asp:Content>
