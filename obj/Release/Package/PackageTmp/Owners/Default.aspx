<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnerLanding.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ExpoOrders.Web.Owners.Default" %>
<%@ MasterType VirtualPath="~/Owners/OwnerLanding.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

    <CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />
    <asp:PlaceHolder ID="plcManageShows" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">
                Show Listing&nbsp;&nbsp;<a id="lnkShowListingHelp" href="#" onclick="return false;"><img src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
            </legend>
            
            <span class="informational"></span>

            <asp:PlaceHolder ID="plcShowList" runat="server">

                <asp:GridView runat="server" ID="grdvwShowList"
                    AllowPaging="false" AllowSorting="true" AutoGenerateColumns="false" CellPadding="3"
                    CellSpacing="0" AlternatingRowStyle-CssClass="item" RowStyle-CssClass="altItem"
                    OnRowDataBound="grdvwShowList_RowDataBound" OnRowCommand="grdvwShowList_RowCommand" 
                     OnSorting="grdvwShowList_Sorting"
                    EmptyDataText="No shows to display." GridLines="None" HeaderStyle-Wrap="false">
                    <Columns>
                        <asp:TemplateField ItemStyle-Wrap="false" HeaderStyle-Wrap="false" ItemStyle-VerticalAlign="Top" SortExpression="ShowId">
                            <HeaderTemplate>
                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="ShowId">Id</asp:LinkButton>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:LinkButton Visible="true" ID="lbtnManageShowById" Text='<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' CommandName="EditShow"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-Wrap="false" SortExpression="ShowName">
                            <HeaderTemplate>
                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="ShowName">Show</asp:LinkButton>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:LinkButton Visible="true" ID="lbtnManageShow" Text='<%# DataBinder.Eval(Container.DataItem, "ShowName")%>' CommandName="EditShow"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' runat="server" /><br />
                                    &nbsp;<%# DataBinder.Eval(Container.DataItem, "VenueName")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Wrap="false" HeaderStyle-Wrap="false" SortExpression="StartDate">
                            <HeaderTemplate>
                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="StartDate">Start Date</asp:LinkButton>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Literal ID="ltrStartDate" runat="server" /></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Wrap="false" HeaderStyle-Wrap="false" SortExpression="EndDate">
                            <HeaderTemplate>
                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="EndDate">End Date</asp:LinkButton>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Literal ID="ltrEndDate" Text="" runat="server" /></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" SortExpression="ActiveFlag" HeaderStyle-Wrap="false">
                             <HeaderTemplate>
                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="ActiveFlag">Active?</asp:LinkButton>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Literal ID="ltrActive" Text="" runat="server" /></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" SortExpression="DisplayOnOwnerLandingPage" HeaderStyle-Wrap="false">
                             <HeaderTemplate>
                                <asp:LinkButton runat="server" CommandName="Sort" CommandArgument="DisplayOnOwnerLandingPage">Public?</asp:LinkButton>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Literal ID="ltrDisplayOnOwnerLandingPage" Text="" runat="server" /></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:LinkButton Visible="false" ID="lbtnDeactivateShow" Text="Remove" CommandName="DeactivateShow"
                                    CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:LinkButton Visible="false" ID="lbtnActivateShow" Text="Restore" CommandName="ActivateShow"
                                    CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Button ID="btnCopyShow" Text="Copy" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ShowId")%>' OnClick="btnCopyShow_Click" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <br />
                <asp:Button ID="dummyBtn" style="visibility: hidden;" runat="server" />
                <asp:Button ID="btnAddShow" CssClass="button" Text="Create a Show" OnClick="btnAddShow_Click" runat="server" />
                &nbsp;
                <asp:Button ID="btnRefresh" CssClass="button" Text="Refresh" OnClick="btnRefresh_Click" runat="server" />
                &nbsp;
                <asp:CheckBox ID="chkIncludeInactive" runat="server" Text="Show Inactive Shows" Checked="false" />

                <a id="InActiveHelp" href="#" onclick="return false;"><img id="Img1" src="~/Images/help_16x16.gif" alt="Help" style="border-width: 0px;" runat="server" /></a>
            </asp:PlaceHolder>
        </fieldset>
    </asp:PlaceHolder>

    <asp:Panel CssClass="outerPopup" Style="display: none;" runat="server" ID="pnlOuterCreateShow">
        <asp:Panel Width="400px" CssClass="innerPopup" runat="server" ID="pnlCreateShow">
            <fieldset class="commonControls">
                <legend class="commonControls"><asp:Literal ID="ltrModalPopupTitle" runat="server" /></legend>
                <h3>Show Information</h3>
                <table border="0" width="100%" cellpadding="1" cellspacing="0">
                    <tr>
                        <td class="contentLabelRight">Show Name</td>
                        <td>
                            <asp:TextBox ID="txtShowName" Visible="true" Width="250" TabIndex="0" CssClass="inputText" MaxLength="100" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShowName" CssClass="errorMessage"
                                ErrorMessage="Show Name is Required" EnableClientScript="false" runat="server"
                                ControlToValidate="txtShowName" ValidationGroup="ShowInformation">Missing Show Name</asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">Start Date</td>
                        <td>
                            <asp:TextBox ID="txtStartDate" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                            <cc1:CalendarExtender ID="calExtStartDate" TargetControlID="txtStartDate" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorStartDate" CssClass="errorMessage"
                                ErrorMessage="Start Date is Required" EnableClientScript="false" runat="server"
                                ControlToValidate="txtStartDate" ValidationGroup="ShowInformation">Missing Start Date</asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="contentLabelRight">End Date</td>
                        <td>
                            <asp:TextBox ID="txtEndDate" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                            <cc1:CalendarExtender ID="calExtEndDate" TargetControlID="txtEndDate" runat="server" />
                            
                        </td>
                        <td style="width: 10%">&nbsp;</td>
                    </tr>
                </table>
                <br />
                <center>
                    <asp:Button ID="btnOk" CssClass="button" OnClick="btnOk_Click" Text="Save Show" ValidationGroup="ShowInformation" runat="server" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnCancel" CssClass="button" Text="Cancel" ValidationGroup="ShowInformation" onclick="btnCancelPopup_Click" runat="server" />
                </center>
            </fieldset>
        </asp:Panel>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="MPE" runat="server" TargetControlID="dummyBtn" PopupControlID="pnlOuterCreateShow" BackgroundCssClass="modalBackground"  DropShadow="true" CancelControlID="btnCancel" />
    <cc1:RoundedCornersExtender ID="RCE" runat="server" TargetControlID="pnlCreateShow" BorderColor="black" Radius="6" />

    <telerik:RadToolTip runat="server" ID="rttShowList" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="lnkShowListingHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(rttShowList, "OwnerShowList")%>
    </telerik:RadToolTip>

    <telerik:RadToolTip runat="server" ID="rttInActiveHelp" Skin="Web20" HideEvent="ManualClose"
        Width="350px" ShowEvent="onmouseover" RelativeTo="Element" Animation="Resize"
        TargetControlID="InActiveHelp" IsClientID="true" Position="TopRight"
        Title="tbd">
        <%=ConfigureToolTip(rttInActiveHelp, "OwnerInactiveShow")%>
    </telerik:RadToolTip>


</asp:Content>
