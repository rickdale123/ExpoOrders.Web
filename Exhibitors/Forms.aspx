<%@ Page Title="" Language="C#" MasterPageFile="~/Exhibitors/ExhibitorsMaster.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Forms.aspx.cs" Inherits="ExpoOrders.Web.Exhibitors.Forms" %>
<%@ Register Src="~/CustomControls/FormQuestions.ascx" TagPrefix="uc" TagName="FormQuestions" %>
<%@ MasterType VirtualPath="~/Exhibitors/ExhibitorsMaster.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">
    <CustomControls:ValidationErrors ID="PageErrors" ValidationGroup="FormSubmission" CssClass="errorMessageBlock" runat="server" />

    <asp:PlaceHolder ID="plcDynamicForm" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls"><asp:Literal ID="ltrFormName" runat="server" /></legend>
            
                <asp:Label ID="lblFormSubmissionDeadlineError" Visible="false" CssClass="errorMessage" runat="server" />

                <p><asp:Literal ID="ltrFormDescription" runat="server" /></p>

               <uc:FormQuestions id="ucFormQuestions" visible="false" runat="server" />

            <center>
                <asp:Button ID="btnSubmitForm" OnClick="btnSubmitForm_Click" Text="Submit" CssClass="button" ValidationGroup="FormSubmission" runat="server" />
            </center>
            

        </fieldset>
        <asp:HiddenField ID="hdnFormId" runat="server" EnableViewState="true" Value="0" Visible="true" />
    </asp:PlaceHolder>
</asp:Content>
