<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdmin/SuperAdminMaster.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="SessionLog.aspx.cs" Inherits="ExpoOrders.Web.SuperAdmin.SessionLog" %>
<%@ MasterType VirtualPath="~/SuperAdmin/SuperAdminMaster.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

Session Start Date: <telerik:RadDatePicker ID="txtStartDate" DateInput-CssClass="inputText" CssClass="inputText" Width="100px" runat="server" AutoPostBack="false"
                        DateInput-EmptyMessage="MaxDate" MinDate="01/01/1000" MaxDate="01/01/3000">
                        <Calendar ShowRowHeaders="false">
                            <SpecialDays>
                                <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                            </SpecialDays>
                        </Calendar>
                    </telerik:RadDatePicker>

&nbsp;&nbsp;
<asp:CheckBox ID="chkOnlyUserIds" Checked="true" Text="Only Display Valid User Logins" runat="server" />
<asp:Button ID="btnGo" Text="Search" CssClass="button" OnClick="btnGo_Click" runat="server" />&nbsp;&nbsp;
<asp:Button ID="btnShowAll" CssClass="button" Text="Show All" OnClick="btnShowAll_Click" runat="server" />
<hr />

    <asp:PlaceHolder ID="plcSessionLogs" Visible="true" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls">Session Logs</legend>

            <asp:Label ID="lblRowCount" Text="" CssClass="techieInfo" runat="server" />
            <asp:GridView EnableViewState="false" OnRowDataBound="grdvSessionLogs_RowDataBound"
                runat="server" ID="grdvSessionLog" AllowPaging="false" AllowSorting="true"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="dataRowAlt"
                RowStyle-CssClass="dataRow" GridLines="Both"
                EmptyDataText="No Logs to display.">
                <Columns>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Session Dates" ItemStyle-VerticalAlign="Top" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%#Convert.ToInt64(DataBinder.Eval(Container, "RowIndex")) + 1%>.&nbsp;
                            SessionId: <%# DataBinder.Eval(Container.DataItem, "AspSessionId")%><br />
                            Session Start: <%# DataBinder.Eval(Container.DataItem, "SessionStartDate")%><br />
                            Session End: <%# DataBinder.Eval(Container.DataItem, "SessionEndDate")%><br />
                            LogIn: <%# DataBinder.Eval(Container.DataItem, "LogInDate")%><br />
                            LogOut: <%# DataBinder.Eval(Container.DataItem, "LogOutDate")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Ids" ItemStyle-VerticalAlign="Top" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            
                            Host Address: <%# DataBinder.Eval(Container.DataItem, "HostAddress")%><br /><br />
                            UserId: <asp:LinkButton ID="lnkViewUserId" runat="Server"><%# DataBinder.Eval(Container.DataItem, "UserId")%></asp:LinkButton><br />
                            UserName: <asp:Literal ID="ltrUserName" runat="server" /><br /><br />
                            Machine: <%# DataBinder.Eval(Container.DataItem, "MachineName")%><br />
                            App Version: <%# DataBinder.Eval(Container.DataItem, "ApplicationVersion")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Browser Capability" ItemStyle-VerticalAlign="Top" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            Info Xml:<br />
                                <textarea class="inputText" rows="5" cols="65">User Agent: <%# DataBinder.Eval(Container.DataItem, "UserAgent")%><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "SessionLogXml").ToString())%>
                                </textarea>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </fieldset>
    </asp:PlaceHolder>
</asp:Content>
