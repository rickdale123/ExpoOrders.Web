<%@ Page Title="" Language="C#" MasterPageFile="DefaultMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ExpoOrders.Web.Default" %>
<%@ MasterType VirtualPath="~/DefaultMaster.Master" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderContent" runat="server">

</asp:Content>
<asp:Content ID="LoginBodyContent" ContentPlaceHolderID="LoginBodyContent" runat="server">
    <table width="100%" border="0">
        <tr>
            <td height="24" valign="top">&nbsp;</td>
            <td valign="top">&nbsp;</td>
        </tr>
        <tr>
            <td height="24" valign="top">&nbsp;</td>
            <td valign="top">&nbsp;</td>
        </tr>
        <tr>
            <td width="425" height="113" valign="top"><table width="250" border="1" align="center" cellpadding="10" cellspacing="0">
              <tr>
                <td><div align="center"><strong><a id="lnkExhibitorLogin" class="strongLink" runat="server">Exhibitor <br />Login</a></strong></div></td>
                </tr>
            </table>
            </td>
            <td width="426" valign="top">
                <table width="250" border="1" align="center" cellpadding="10" cellspacing="0">
                    <tr>
                        <td><div align="center"><strong><a id="lnkContractorLogin" class="strongLink" runat="server">General Contractor<br />Login</a></strong></div></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
