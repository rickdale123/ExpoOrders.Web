<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdmin/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="Transactions.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.Transactions" %>
<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

Trx Date: <telerik:RadDatePicker ID="txtTransactionDate" DateInput-CssClass="inputText" CssClass="inputText" runat="server" Width="100px" AutoPostBack="false"
                        DateInput-EmptyMessage="" MinDate="01/01/1000" MaxDate="01/01/3000">
                        <Calendar ShowRowHeaders="false">
                            <SpecialDays>
                                <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                            </SpecialDays>
                        </Calendar>
                    </telerik:RadDatePicker>

<asp:Button ID="btnGo" Text="Search" OnClick="btnGo_Click" runat="server" />

<hr />

    <asp:PlaceHolder ID="plcTransactionLogs" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Transaction Logs</legend>

            <asp:Label ID="lblTransactionLogRowCount" Text="" CssClass="techieInfo" runat="server" />
            <asp:GridView EnableViewState="true" OnRowDataBound="grdvTrxLog_RowDataBound"
                runat="server" ID="grdvTrxLog" AllowPaging="false" AllowSorting="true"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="dataRowAlt"
                RowStyle-CssClass="dataRow" GridLines="Both"
                EmptyDataText="No Logs to display.">
                <Columns>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText=" TrxDate" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:HiddenField ID="hdnPaymentTransactionId" Value='<%# DataBinder.Eval(Container.DataItem, "PaymentTransactionId")%>' runat="server" />
                            <%#Convert.ToInt64(DataBinder.Eval(Container, "RowIndex")) + 1%>.&nbsp;
                            <%# DataBinder.Eval(Container.DataItem, "TransactionDate")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="ExhibitorId">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "ExhibitorId")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="OrderId">
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "OrderId")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="PaymentGateway/Response">
                        <ItemTemplate>
                            Trxtype:<%# DataBinder.Eval(Container.DataItem, "TransactionTypeCd")%><br />
                            Success:<%# DataBinder.Eval(Container.DataItem, "SuccessFlag")%><br />
                            Active:<%# DataBinder.Eval(Container.DataItem, "ActiveFlag")%><br />
                            <%# DataBinder.Eval(Container.DataItem, "PaymentGatewayCd")%><br />
                            <%# DataBinder.Eval(Container.DataItem, "TransactionResponse")%>


                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Request">
                        <ItemTemplate>
                            <textarea rows="4" cols="30"><%# DataBinder.Eval(Container.DataItem, "RequestParamsDecrypted")%></textarea>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Response">
                        <ItemTemplate>
                            <textarea rows="4" cols="30"><%# DataBinder.Eval(Container.DataItem, "ResponseParamsDecrypted")%></textarea>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </fieldset>
    </asp:PlaceHolder>

</asp:Content>
