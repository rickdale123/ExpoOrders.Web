<%@ Page Title="" Language="C#" MasterPageFile="~/Exhibitors/ExhibitorsMaster.Master" AutoEventWireup="true" CodeBehind="Shipping.aspx.cs" Inherits="ExpoOrders.Web.Exhibitors.Shipping" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ MasterType VirtualPath="~/Exhibitors/ExhibitorsMaster.Master" %>

<%@ Register Src="~/CustomControls/FormQuestions.ascx" TagPrefix="uc" TagName="FormQuestions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

<CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

     <asp:PlaceHolder ID="plcOutboundAddress" Visible="false" runat="server">
        <table border="0" width="100%" cellpadding="1" cellspacing="0">
            <tr>
                <td class="contentLabelRight">Company Name</td>
                <td>
                    <asp:TextBox ID="txtCompanyName" Width="200" CssClass="inputText" Text="ACME Company name here" MaxLength="50" runat="server" />

                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">Address</td>
                <td><asp:TextBox ID="txtAddressLine1" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">&nbsp;</td>
                <td><asp:TextBox ID="txtAddressLine2" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">&nbsp;</td>
                <td><asp:TextBox ID="txtAddressLine3" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">&nbsp;</td>
                <td><asp:TextBox ID="txtAddressLine4" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">City</td>
                <td><asp:TextBox ID="txtCity" Width="125" CssClass="inputText" MaxLength="50" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">State/Province/Region</td>
                <td><asp:TextBox ID="txtState" Width="150" CssClass="inputText" MaxLength="100" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">Postal Code</td>
                <td><asp:TextBox ID="txtPostalCode" Width="50" CssClass="inputText" MaxLength="20" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">Country</td>
                <td><asp:TextBox ID="txtCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
        </table>
         <br />
        <center>
            <asp:Button ID="btnGenerateOutboundShippingLabel" CssClass="button" Text="Generate Shipping Label" ValidationGroup="OutboundShippingLabel" runat="server" />
        </center>

        <asp:PlaceHolder ID="plcAddressList" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Outbound Shipping Addresses</legend>
            
                <asp:GridView EnableViewState="true"
                    runat="server" ID="grdvwAddressList" AllowPaging="false" AllowSorting="true"
                    AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                    RowStyle-CssClass="item" OnRowDataBound="grdvwAddressList_RowDataBound" GridLines="None"
                    OnRowCommand="grdvwAddressList_RowCommand" EmptyDataText="No Addresses to display.">
                    <Columns>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="">
                            <ItemTemplate>
                                <asp:Label ID="lblAddressType" CssClass="techieInfo" runat="server" />
                                <br />
                                <asp:Literal ID="ltrOtherFullAddress" runat="server" />
                                <br />
                                <asp:LinkButton Visible="true" ID="lbtnLoadAddress" Text="Load this Address" CssClass="action-link" CommandName="LoadAddress" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressId")%>' runat="server" />
                                <hr />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </fieldset>
        </asp:PlaceHolder>
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
