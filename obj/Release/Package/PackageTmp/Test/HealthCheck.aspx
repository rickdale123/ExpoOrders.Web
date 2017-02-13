<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HealthCheck.aspx.cs" Inherits="ExpoOrders.Web.Test.HealthCheck" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ExpoOrders - Health Check Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnRefresh" Text="Refresh" OnClick="btnRefresh_Click" runat="server" />
        <br /><br />

            <asp:GridView EnableViewState="true" OnRowDataBound="grdvValidationTests_RowDataBound" OnRowCommand="grdvValidationTests_RowCommand"
                    runat="server" ID="grdvValidationTests" AllowPaging="false" AllowSorting="false"
                    AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                    RowStyle-CssClass="item" EmptyDataText="No tests to Run." GridLines="Both">
                    <Columns>
                        <asp:TemplateField HeaderText="Test Name">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkBtnRunTest" CommandName="RunTest" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Result">
                            <ItemTemplate>
                                <asp:Label ID="lblTestResult" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Message">
                            <ItemTemplate>
                                <asp:Label ID="lblTestMessage" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Notes">
                            <ItemTemplate>
                                <asp:Label ID="lblTestNotes" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
         </asp:GridView>
    </div>
    </form>
</body>
</html>
