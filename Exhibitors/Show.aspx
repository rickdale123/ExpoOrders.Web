<%@ Page Title="" Language="C#" MasterPageFile="~/Exhibitors/ExhibitorsMaster.Master" AutoEventWireup="true" CodeBehind="Show.aspx.cs" Inherits="ExpoOrders.Web.Exhibitors.Show" %>
<%@ MasterType VirtualPath="~/Exhibitors/ExhibitorsMaster.Master" %>

<%@ Register Src="~/CustomControls/FormQuestions.ascx" TagPrefix="uc" TagName="FormQuestions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

<CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

<asp:PlaceHolder ID="plcShowInfo" Visible="false" runat="server">
   
        <div id="showInfo">

            
            <h2><asp:Literal ID="ltrShowName" runat="server" /></h2>
            
            <h4><asp:Literal ID="ltrShowDates" runat="server" /></h4><br />
            
            <h3><asp:Literal ID="ltrVenueName" runat="server" /></h3>
            <p><asp:Literal ID="ltrVenueAddress" runat="server" /></p>
        </div>

</asp:PlaceHolder>

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
