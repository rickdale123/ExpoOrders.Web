<%@ Page Title="" Language="C#" MasterPageFile="~/Owners/OwnersMaster.Master" AutoEventWireup="true" CodeBehind="ReportList.aspx.cs" Inherits="ExpoOrders.Web.Owners.ReportList" %>

<%@ MasterType VirtualPath="~/Owners/OwnersMaster.Master" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">
    
    <CustomControls:ValidationErrors ID="PageErrors" CssClass="errorMessageBlock" runat="server" />

    <asp:PlaceHolder ID="plcSearchCriteria" visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Reporting Criteria</legend>
                <table class="searchTable" width="500" border="0" cellpadding="1" cellspacing="1">
                    <tr>
                        <td class="searchOption" valign="top">
                            <table border="0" width="100%" cellpadding="2" cellspacing="1">                                

                                <asp:PlaceHolder ID="plcGenericExhibitorCriteria" runat="server" Visible="false">
                                    <tr>
                                        <td style="text-align: right;"><asp:Literal Text="Select Exhibitor: " ID="ltrSelectExibitor" runat="server"/></td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="cboExhibitorId" CssClass="inputText" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcOrderSummaryCriteria" runat="server" Visible="false">
                                    <tr>
                                        <td style="text-align: right;"><asp:Literal Text="Category: " ID="ltrSelectCategory" runat="server"/></td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="cboCategoryId" CssClass="inputText" OnSelectedIndexChanged="cboCategory_selectedIndexChanged" AutoPostBack="true" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr> 
                                    <tr>
                                        <td style="text-align: right;"><asp:Literal Text="Product: " ID="ltrSelectProduct" runat="server"/></td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="cboProduct" CssClass="inputText" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcOrderReceiptCriteria" runat="server" Visible="false">
                                    <tr>
                                        <td style="text-align: right;">Report By:</td>
                                        <td>
                                            <asp:RadioButtonList ID="rblOrderReceiptCriteriaChoice" OnSelectedIndexChanged="rblOrderReceiptCriteriaChoice_onSelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal" runat="server">
                                                <asp:ListItem Text="By Exhibitor" Value="0" />
                                                <asp:ListItem Text="By Order #" Value="1" />
                                            </asp:RadioButtonList>
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">
                                            <asp:Literal Visible="false" Text="Select Order: " ID="ltrOrderId" runat="server"/>
                                            <asp:Literal Visible="false" Text="Select Exhibitor: " ID="ltrSelectOrderReceiptExhibitor" runat="server"/>
                                        </td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="cboOrderId" CssClass="inputText" Visible="false" runat="server" />
                                            <asp:DropDownList ID="cboOrderReceiptExhibitorId" CssClass="inputText" Visible="false" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                    <asp:PlaceHolder ID="plcFormNameSelection" runat="server" Visible="false">
                                        <tr>
                                            <td style="text-align: right;">Form:</td>
                                            <td style="white-space: nowrap">
                                                <asp:DropDownList ID="cboFormName" CssClass="inputText" runat="server" />
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>            
                                    </asp:PlaceHolder>
                                </asp:PlaceHolder>
        
                                <asp:PlaceHolder ID="plcExhibitorListCriteria" runat="server" Visible="false">
                                    <tr>
                                        <td style="text-align: right;"><asp:Literal Visible="true" Text="Sort By:" ID="ltrExhibitorListSort" runat="server"/></td>
                                        <td style="white-space: nowrap">
                                            <asp:RadioButtonList RepeatDirection="Horizontal" ID="rblExhibitorListSortChoice" runat="server">
                                                <asp:ListItem Text="Company Name" Selected="True" Value="0" />
                                                <asp:ListItem Text="Booth Number" Value="1" />
                                            </asp:RadioButtonList>
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcBillingSummaryCriteria" runat="server" Visible="false">
                                    <tr>
                                        <td style="text-align: right;"><asp:Literal Text="Select Exhibitor: " ID="Literal2" runat="server"/></td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="cboBillingSummaryExhibitorId" CssClass="inputText" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">Sort By:</td>
                                        <td style="white-space: nowrap">
                                            <asp:RadioButtonList RepeatDirection="Horizontal" ID="rdoBillingSummarySort" runat="server">
                                                <asp:ListItem Text="Company Name" Selected="True" Value="company" />
                                                <asp:ListItem Text="Booth Number" Value="booth" />
                                            </asp:RadioButtonList>
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plcDeliveryReport" runat="server" Visible="false">
                                    <tr>
                                        <td style="text-align: right;"><asp:Literal ID="ltrDelRptExhibitor" Visible="true" Text="Select Exhibitor: " runat="server"/></td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="cboDelRptExhibitorId" CssClass="inputText" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">Sort By:</td>
                                        <td style="white-space: nowrap">
                                            <asp:RadioButtonList ID="rdoBtnDelRptSortBy" RepeatDirection="Horizontal" runat="server">
                                                <asp:ListItem Text="Company Name" Selected="True" Value="0" />
                                                <asp:ListItem Text="Booth Number" Value="1" />
                                            </asp:RadioButtonList>
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                    
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plcPriceListReport" runat="server" Visible = "false">
                                    <tr>
                                        <td style="text-align: right;"><asp:Literal ID="Literal3" Text="Category: " runat="server"/></td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="cboPriceListRptCategory" CssClass="inputText" OnSelectedIndexChanged="cboPriceListRptCategory_selectedIndexChanged" AutoPostBack="true" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr> 
                                    <tr>
                                        <td style="text-align: right;"><asp:Literal ID="Literal4" Text="Product: " runat="server"/></td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="cboPriceListRptProduct" CssClass="inputText" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plcInstallDismantleFilters" runat="server" Visible="false">
                                    <tr>
                                        <td style="text-align: right; white-space: nowrap;">Exhibitor:</td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="ddlExhibitor2" CssClass="inputText" OnSelectedIndexChanged="ddlExhibitor2_IndexChanged" AutoPostBack="true" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right; white-space: nowrap;">Order Line Item:</td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="ddlExhibitorOrderItem" CssClass="inputText" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;"><asp:Literal Visible="true" Text="Sort By:" ID="lblInstallDismantleSort" runat="server"/></td>
                                        <td style="white-space: nowrap">
                                            <asp:RadioButtonList RepeatDirection="Horizontal" ID="rblInstallDismantleSort" runat="server">
                                                <asp:ListItem Text="Company Name" Selected="True" Value="0" />
                                                <asp:ListItem Text="Booth Number" Value="1" />
                                            </asp:RadioButtonList>
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plcMultipleCategoryFilter" runat="server" Visible="false">
                                    <tr>
                                        <td style="vertical-align: text-top; white-space: nowrap;" colspan="3">
                                            Category Filter:&nbsp;
                                                <asp:RadioButtonList ID="rdoEnableMultiCategoryFilter" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rdoEnableMultiCategoryFilter_Changed" runat="server">
                                                    <asp:ListItem Value="0" Selected="True">Display All</asp:ListItem>
                                                    <asp:ListItem Value="1">Filter by Selection</asp:ListItem>
                                                </asp:RadioButtonList>
                                            <br />
                                            <asp:CheckBoxList ID="chkLstCategoryIdFilter" Visible="false" RepeatColumns="2" RepeatDirection="Vertical" AutoPostBack="true" OnSelectedIndexChanged="chkLstCategoryIdFilter_IndexChanged" runat="server" />
                                        </td>
                                    </tr>
                                    <asp:PlaceHolder ID="plcMultipleProductFilter" runat="server" Visible="false">
                                        <tr>
                                            <td style="vertical-align: text-top; white-space: nowrap;" colspan="3">
                                                Product Filter:&nbsp;
                                                 <asp:RadioButtonList ID="rdoEnableMultiProductFilter" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rdoEnableMultiProductFilter_Changed" runat="server">
                                                    <asp:ListItem Value="0" Selected="True">Display All</asp:ListItem>
                                                    <asp:ListItem Value="1">Filter by Selection</asp:ListItem>
                                                </asp:RadioButtonList><br />
                                                <asp:CheckBoxList ID="chkLstProductIdFilter" Visible="false" RepeatColumns="2" RepeatDirection="Vertical" AutoPostBack="false" runat="server" />
                                            </td>
                                        </tr>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcMultiCategorySortBy" runat="server" Visible="false">
                                        <tr>
                                            <td style="text-align: right;"><asp:Literal Visible="true" Text="Sort By:" ID="ltrMultiCategorySortByLabel" runat="server"/></td>
                                            <td style="white-space: nowrap">
                                                <asp:RadioButtonList RepeatDirection="Horizontal" ID="rdoBtnMultiCategorySortyBy" runat="server">
                                                    <asp:ListItem Text="Company Name" Selected="True" Value="0" />
                                                    <asp:ListItem Text="Booth Number" Value="1" />
                                                </asp:RadioButtonList>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </asp:PlaceHolder>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plcTransactionTypeFilters" runat="server" Visible = "false">
                                    <tr>
                                        <td style="vertical-align: text-top; white-space: nowrap;" colspan="3">
                                        Transaction Types:<br />
                                            <asp:CheckBoxList ID="chklstTransactionTypeCd" CssClass="inputText" RepeatColumns="2" RepeatDirection="Horizontal" RepeatLayout="Table" runat="server">
                                            <asp:ListItem Value="Authorize" Selected="True">Authorize</asp:ListItem>
                                            <asp:ListItem Value="Settle" Selected="True">Settle</asp:ListItem>
                                            <asp:ListItem Value="Sale" Selected="True">Sale</asp:ListItem>
                                            <asp:ListItem Value="Refund" Selected="True">Refund</asp:ListItem>
                                            <asp:ListItem Value="Void" Selected="True">Void</asp:ListItem>
                                            <asp:ListItem Value="Manual" Selected="True">Manual</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plcBoothNumberFilter" runat="server" Visible="false">
                                    <tr>
                                        <td style="text-align: right; white-space: nowrap;">Booth Number:</td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="cboBoothNumber" CssClass="inputText" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcExhibitorClassification" runat="server" Visible="false">
                                    <tr>
                                        <td style="text-align: right; white-space: nowrap;">Classification:</td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="cboExhibitorClassification" CssClass="inputText" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcSuccessFilter" runat="server" Visible="false">
                                    <tr>
                                        <td style="vertical-align: text-top; white-space: nowrap;" colspan="3">
                                        Successful Trx?:
                                            <asp:DropDownList ID="ddlSuccessFilter" CssClass="inputText" runat="server">
                                            <asp:ListItem Value="-1" Selected="True">-- Either --</asp:ListItem>
                                            <asp:ListItem Value="1">Successful</asp:ListItem>
                                            <asp:ListItem Value="0">Failed</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcActiveFilter" runat="server" Visible="false">
                                    <tr>
                                        <td style="vertical-align: text-top; white-space: nowrap;" colspan="3">
                                        Deleted?:
                                            <asp:DropDownList ID="ddlActiveFilter" CssClass="inputText" runat="server">
                                            <asp:ListItem Value="-1" Selected="True">-- Either --</asp:ListItem>
                                            <asp:ListItem Value="1">Only Active</asp:ListItem>
                                            <asp:ListItem Value="0">Only Deleted</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plcStartEndDates" runat="server" Visible="false">
                                    <tr>
                                        <td style="text-align: right; white-space: nowrap;"><asp:Label ID="lblStartEndDateLabel" Text="Order Date: " runat="server" /></td>
                                        <td style="white-space: nowrap">
                                            Between:
                                            <asp:TextBox ID="txtStartDate" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                                            <cc1:CalendarExtender ID="calExtStartDate" TargetControlID="txtStartDate" runat="server" />
                                            and
                                            <asp:TextBox ID="txtEndDate" Visible="true" Width="100" CssClass="inputText" MaxLength="50" runat="server" />
                                            <cc1:CalendarExtender ID="calExtEndDate" TargetControlID="txtEndDate" runat="server" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plcOutboundAddressFilter" runat="server" Visible="false">
                                    <tr>
                                        <td style="text-align: right; white-space: nowrap;">Address Filter:</td>
                                        <td style="white-space: nowrap">
                                            <asp:DropDownList ID="ddlOutboundAddressFilter" CssClass="inputText" runat="server">
                                                <asp:ListItem Value="0" Selected="True">-- All --</asp:ListItem>
                                                <asp:ListItem Value="1">Only Exhibitor Addresses</asp:ListItem>
                                                <asp:ListItem Value="2">Only Outbound Addresses</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </asp:PlaceHolder>
                                
                                <asp:PlaceHolder ID="plcViewReportButton" runat="server" Visible="false">
                                    <tr>
                                        <td colspan="3" style="text-align: center;">
                                            <asp:Button ID="btnViewReport" Text="View Report" CssClass="button" OnClick="btnViewReport_OnClick" runat="server" />
                                            &nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnViewRawData" Text="Raw Data" CssClass="button" OnClick="btnViewRawData_OnClick" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <asp:Literal ID="ltrReportNote" runat="server" />
                                        </td>   
                                    </tr>
                                </asp:PlaceHolder>
                            </table>


                        </td>
                    </tr>
                </table>
          </fieldset>

    </asp:PlaceHolder>
        
    <asp:PlaceHolder ID="plcReportViewer" Visible="false" runat="server">
        <div class="informational">
            Note: Please export to PDF, Excel or Word format for printing hard copies of a report.
        </div>
        <rsweb:ReportViewer Width="100%" Visible="false" ID="rptViewer" runat="server" Font-Names="Verdana"
            Font-Size="8pt" InteractiveDeviceInfos="(Collection)" WaitMessageFont-Names="Verdana" ShowPrintButton="false" AsynchRendering="false"
            WaitMessageFont-Size="14pt">
        </rsweb:ReportViewer>
    </asp:PlaceHolder>
    
            
    <asp:HiddenField runat="server" ID="hdnPageMode" Value="0" /> 
    <asp:HiddenField ID="hdnCurrentReportId" runat="server" />
</asp:Content>
