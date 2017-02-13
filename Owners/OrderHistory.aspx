<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderHistory.aspx.cs" ValidateRequest="false" Inherits="ExpoOrders.Web.Owners.OrderHistory" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="pageHead" runat="server">
    <title>ExpoOrders - Order History</title>
    <link id="OwnerStyleSheet" href="Styles/style.css" rel="Stylesheet" type="text/css" />
</head>
<body class="pageBody">
    <form id="mainForm" runat="server">
        <div id="container" style="width: 100%">
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                 <tr>
                    <td class="contentLabel">Order Notes:</td>
                    <td><asp:Label ID="lblOrderNotes" runat="server" /></td>
                    <td style="width: 10%">&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="3" style="white-space: nowrap;">
                         <asp:GridView EnableViewState="true" GridLines="None"
                                    runat="server" ID="grdvwOrderHistory" AllowPaging="false" AllowSorting="true" Width="90%"
                                    AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                                    RowStyle-CssClass="item"
                                    EmptyDataText="No order history to display.">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Line Item Id" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <%# DataBinder.Eval(Container.DataItem, "OrderItemId")%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Notes" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <pre><%# FormatHtmlEncoded(DataBinder.Eval(Container.DataItem, "Notes"))%></pre>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Updated On" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <%# DataBinder.Eval(Container.DataItem, "UpdateDateTime")%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Updated By" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <%# DataBinder.Eval(Container.DataItem, "UpdateBy")%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
