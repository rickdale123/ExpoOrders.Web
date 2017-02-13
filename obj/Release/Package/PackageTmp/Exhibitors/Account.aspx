<%@ Page Title="" Language="C#" MasterPageFile="~/Exhibitors/ExhibitorsMaster.Master" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="ExpoOrders.Web.Exhibitors.Account" %>
<%@ MasterType VirtualPath="~/Exhibitors/ExhibitorsMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

<CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

<asp:PlaceHolder id="plcEditProfile" runat="server">
    <fieldset class="commonControls">
        <legend class="commonControls">Profile Information</legend>
        <h3>Exhibitor Information</h3>
         <table border="0" width="100%" cellpadding="1" cellspacing="0">
            <tr>
                <td class="contentLabelRight">Company Name</td>
                <td>
                    <asp:Label ID="lblReadOnlyCompanyName" Visible="false" runat="server" />
                    <asp:TextBox ID="txtCompanyName" Visible="false" Width="200" CssClass="inputText" MaxLength="100" runat="server" />

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" CssClass="errorMessage" ErrorMessage="Company Name is Required"  EnableClientScript="false" runat="server" ControlToValidate="txtCompanyName" ValidationGroup="ProfileInformation">Missing Company Name</asp:RequiredFieldValidator>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">Address</td>
                <td><asp:TextBox ID="txtAddressLine1" Width="200" CssClass="inputText" MaxLength="100" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">&nbsp;</td>
                <td><asp:TextBox ID="txtAddressLine2" Width="200" CssClass="inputText" MaxLength="100" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">City</td>
                <td><asp:TextBox ID="txtCity" Width="150" CssClass="inputText" MaxLength="100" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">State/Province/Region</td>
                <td><asp:TextBox ID="txtState" Width="150" CssClass="inputText" MaxLength="100" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">Postal Code</td>
                <td><asp:TextBox ID="txtPostalCode" Width="50" CssClass="inputText" MaxLength="50" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">Country</td>
                <td><asp:TextBox ID="txtCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">Phone</td>
                <td><asp:TextBox ID="txtCompanyPhone" Width="175" CssClass="inputText" MaxLength="50" runat="server" /><span class="informational">(xxx) xxx-xxxx ext. xxxxx</span></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">Primary Email</td>
                <td><asp:TextBox ID="txtCompanyEmailAddress" Width="200" CssClass="inputText" MaxLength="150" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">Tax Exempt?</td>
                <td>
                    <asp:Label ID="lblReadOnlyTaxExempt" Visible="true" runat="server" />
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="3">&nbsp;</td>
            </tr>
        </table>
        <h3>User Information</h3>
        <table border="0" width="100%" cellpadding="1" cellspacing="0">
            <tr>
                <td class="contentLabelRight">Title</td>
                <td><asp:TextBox ID="txtUserTitle" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">First Name</td>
                <td>
                    <asp:TextBox ID="txtUserFirstName" Width="125" CssClass="inputText" MaxLength="50" runat="server" />
                    &nbsp;
                    Last Name&nbsp;
                    <asp:TextBox ID="txtUserLastName" Width="125" CssClass="inputText" MaxLength="50" runat="server" />
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">Email</td>
                <td><asp:TextBox ID="txtUserEmailAddress" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td class="contentLabelRight">Phone</td>
                <td><asp:TextBox ID="txtUserPhone" Width="175" CssClass="inputText" MaxLength="50" runat="server" /><span class="informational">(xxx) xxx-xxxx ext. xxxxx</span></td>
                <td style="width:10%">&nbsp;</td>
            </tr>
        </table>  
        <br />
        <center>
            <asp:Button ID="btnSaveProfile" CssClass="button" Text="Save Profile" ValidationGroup="ProfileInformation" OnClick="btnSaveProfile_Click" runat="server" />
            <asp:HiddenField runat="server" ID="hdnCurrentExhibitorId" Value="0" />
            <asp:HiddenField runat="server" ID="hdnCurrentUserName" Value="" />
        </center>
    </fieldset>
</asp:PlaceHolder>


<asp:PlaceHolder id="plcManageCreditCards" runat="server">
    <fieldset class="commonControls">
        <legend class="commonControls">Credit Cards</legend>

        <asp:PlaceHolder ID="plcCreditCardList" runat="server">
            <asp:Repeater ID="rptCreditCardList" runat="server" OnItemDataBound="rptCreditCardList_ItemDataBound" OnItemCommand="btnEdit_ItemCommand">
                <HeaderTemplate>
                    <h3>Existing Credit Cards on File</h3>
                    <table width="100%" border="0" cellpadding="2" cellspacing="0">
                        <tr class="Item">
                            <td class="colHeader">Name on Card</td>
                            <td class="colHeader">Card Number</td>
                            <td class="colHeader">Expiration Date</td>
                            <td class="colHeader">Email Address</td>
                            <td>&nbsp;</td>
                            <td width="10%">&nbsp;</td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                     <tr id="trCreditCard" class="Item" runat="server">
                            <td><asp:Literal id="ltrNameOnCreditCard" Text="" runat="server" /></td>
                            <td><asp:Literal id="ltrCreditCardNumber" Text="" runat="server" /></td>
                            <td><asp:Literal id="ltrCreditCardExpirationDate" Text="" runat="server" /></td>
                            <td><asp:Literal id="ltrCreditCardEmailAddress" Text="" runat="server" /></td>
                            <td style="white-space: nowrap;">
                                <asp:LinkButton commandname="EditCard" type="button" ID="btnEditCard" runat="server" CssClass="action-link" Text="Edit" />
                                |
                                <asp:LinkButton commandname="DeleteCard" id="btnDeleteCard" type="button" runat="server" CssClass="action-link" Text="Delete" />
                            </td>
                            <td width="10%">&nbsp;</td>
                      </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        
        <asp:Button ID="btnAddCreditCard" CssClass="button" Text="Add Credit Card" OnClick="btnAddCreditCard_Click" runat="server" />

        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plcCreditCardDetail" Visible="false" runat="server">
            
            <h3>Credit Card Information</h3>
            <table border="0" width="100%" cellpadding="1" cellspacing="0">
                <tr>
                    <td class="contentLabelRight">Name on Card</td>
                    <td>
                        <asp:TextBox ID="txtCreditCardName" Width="200" CssClass="inputText" MaxLength="50" runat="server" />

                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="errorMessage" EnableClientScript="false" ErrorMessage="Name is Required" runat="server" ControlToValidate="txtCreditCardName" ValidationGroup="CreditCardInfo">Missing Name</asp:RequiredFieldValidator>
                    </td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                 <tr>
                    <td class="contentLabelRight">Card Type</td>
                    <td>
                        <asp:DropDownList ID="ddlCreditCardType" CssClass="inputText" runat="server" />

                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="errorMessage" ErrorMessage="Card Type is Required" EnableClientScript="false" runat="server" ControlToValidate="ddlCreditCardType" ValidationGroup="CreditCardInfo">Missing Card Type</asp:RequiredFieldValidator>
                    </td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                 <tr>
                    <td class="contentLabelRight">Card Number</td>
                    <td>
                        <input type="hidden" id="hdnCreditCardId" runat="server" />
                        <input type="hidden" id="hdnCreditCardAddressId" runat="server" />
                        <asp:TextBox ID="txtCreditCardNumber" Width="200" CssClass="inputText" MaxLength="50" runat="server" />

                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" CssClass="errorMessage" ErrorMessage="Card Type is Required"  EnableClientScript="false" runat="server" ControlToValidate="txtCreditCardNumber" ValidationGroup="CreditCardInfo">Missing Card Type</asp:RequiredFieldValidator>
                    </td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                 <tr>
                    <td class="contentLabelRight">Expiration Date</td>
                    <td>
                        <asp:DropDownList ID="ddlCreditCardExpMonth" CssClass="inputText" runat="server" />&nbsp;/&nbsp;<asp:DropDownList ID="ddlCreditCardExpYear" CssClass="inputText" runat="server" />
                        
                        <asp:CustomValidator ID="customCreditCardExpDateValidator" CssClass="errorMessage" ValidationGroup="CreditCardInfo" EnableClientScript="false" runat="server"></asp:CustomValidator>
                        &nbsp;<asp:Label ID="Label7" CssClass="contentLabel" runat="server">Security Code</asp:Label>
                        <asp:TextBox ID="txtCreditCardSecurityCode" Width="60" CssClass="inputText" MaxLength="10" runat="server" /> 

                    </td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                
                <tr>
                    <td class="contentLabelRight">Address</td>
                    <td><asp:TextBox ID="txtCreditCardAddressLine1" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">&nbsp;</td>
                    <td><asp:TextBox ID="txtCreditCardAddressLine2" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">City</td>
                    <td><asp:TextBox ID="txtCreditCardCity" Width="125" CssClass="inputText" MaxLength="50" runat="server" /></td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">State/Province/Region</td>
                    <td><asp:TextBox ID="txtCreditCardState" Width="150" CssClass="inputText" MaxLength="20" runat="server" /></td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Postal Code</td>
                    <td><asp:TextBox ID="txtCreditCardPostalCode" Width="50" CssClass="inputText" MaxLength="20" runat="server" /></td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                <tr>
                    <td class="contentLabelRight">Country</td>
                    <td><asp:TextBox ID="txtCreditCardCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" /></td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
                 <tr>
                    <td class="contentLabelRight">Email Address</td>
                    <td><asp:TextBox ID="txtCreditCardEmail" Width="200" CssClass="inputText" MaxLength="150" runat="server" /></td>
                    <td style="width:10%">&nbsp;</td>
                </tr>
            </table>
            <br />
            <center>
                <asp:Button ID="btnSaveCreditCard" CssClass="button" Text="Save Credit Card" OnClick="btnSaveCreditCard_Click" ValidationGroup="CreditCardInfo" runat="server" />
                &nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancelCreditCard" CssClass="button" Text="Cancel" OnClick="btnCancelCreditCard_Click" runat="server" />
            </center>
        </asp:PlaceHolder>
    </fieldset>
</asp:PlaceHolder>


<asp:PlaceHolder id="plcChangePassword" runat="server">
    <fieldset class="commonControls">
        <legend class="commonControls">Change Password</legend>

        <table border="0" width="100%" cellpadding="0">
           <tr>
                <td align="right">User Name:</td>
                <td>
                    <asp:Label ID="lblPreferredUserName" CssClass="dataLabel" runat="server" />
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr> 
            <tr>
                <td align="right">New Password:</td>
                <td>
                    <asp:TextBox ID="txtPassword1" Width="175" CssClass="inputText" TextMode="Password" MaxLength="50" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="txtPassword1" CssClass="errorMessage" ErrorMessage="Enter a password" EnableClientScript="false" ValidationGroup="PasswordChange" runat="server">New Password is required</asp:RequiredFieldValidator>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td align="right">Confirm Password:</td>
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

<asp:PlaceHolder id="plcChangeUsername" runat="server">
    <fieldset class="commonControls">
        <legend class="commonControls">Change Username</legend>

        <table border="0" width="100%" cellpadding="0">
            <tr>
                <td align="right">New Username</td>
                <td>
                    <asp:TextBox ID="txtUsername" Width="175" CssClass="inputText" MaxLength="50" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" ControlToValidate="txtUserName" CssClass="errorMessage" ErrorMessage="Provide a username" ValidationGroup="UsernameChange" EnableClientScript="false" runat="server">Username is required</asp:RequiredFieldValidator>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
        </table>
        
        <center>
            <asp:Button ID="btnSaveUsername" CssClass="button" Text="Save Username" OnClick="btnSaveUsername_Click" ValidationGroup="UsernameChange" runat="server" />
        </center>

    </fieldset>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plcManageAddresses" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Outbound Shipping Addresses</legend>
            <asp:PlaceHolder ID="plcAddressList" Visible="false" runat="server">
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
                                <asp:LinkButton Visible="true" ID="lbtnManageAddress" Text="Edit" CssClass="action-link" CommandName="ManageAddress" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressId")%>' runat="server" />
                                    &nbsp;|&nbsp;
                                <asp:LinkButton Visible="true" ID="lbtnDeleteAddress" Text="Delete" CssClass="action-link" CommandName="DeleteAddress" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressId")%>' runat="server" />
                                <hr />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <br />
                <asp:Button ID="btnAddAddress" CssClass="button" Text="Create Address" OnClick="btnAddAddress_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnAddressList" CssClass="button" Text="Refresh List" OnClick="btnAddressListRefresh_Click" runat="server" />
                &nbsp;&nbsp;
                <asp:Button ID="btnCancelManageAddress" CssClass="button" Text="Cancel" OnClick="btnCancelManageAddress_Click" runat="server" />
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plcAddressDetail" Visible="false" runat="server">
            
                <input type="hidden" id="hdnAddressId" runat="server" />

                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Address type</td>
                        <td><asp:DropDownList ID="ddlOtherAddressType" CssClass="inputText" runat="server">
                            <asp:ListItem Value="Outbound" Selected="True">Outbound Shipping</asp:ListItem>
                        </asp:DropDownList></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Address</td>
                        <td><asp:TextBox ID="txtOtherAddressLine1" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td><asp:TextBox ID="txtOtherAddressLine2" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td><asp:TextBox ID="txtOtherAddressLine3" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                     <tr>
                        <td class="contentLabelRight">&nbsp;</td>
                        <td><asp:TextBox ID="txtOtherAddressLine4" Width="200" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">City</td>
                        <td><asp:TextBox ID="txtOtherAddressCity" Width="125" CssClass="inputText" MaxLength="50" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">State/Province/Region</td>
                        <td><asp:TextBox ID="txtOtherAddressState" Width="150" CssClass="inputText" MaxLength="20" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Postal Code</td>
                        <td><asp:TextBox ID="txtOtherAddressPostalCode" Width="50" CssClass="inputText" MaxLength="20" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Country</td>
                        <td><asp:TextBox ID="txtOtherAddressCountry" Width="150" CssClass="inputText" MaxLength="100" runat="server" /></td>
                        <td style="width:10%">&nbsp;</td>
                    </tr>
                </table>
                <br />
                <center>
                    <asp:Button ID="btnSaveAddress" CssClass="button" Text="Save Address" OnClick="btnSaveAddress_Click" ValidationGroup="CreditCardInfo" runat="server" />
                    &nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnCancelSaveAddress" CssClass="button" Text="Cancel" OnClick="btnCancelSaveAddress_Click" runat="server" />
                </center>
            </asp:PlaceHolder>

        </fieldset>
    </asp:PlaceHolder>


</asp:Content>
