<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormQuestionEditorList.ascx.cs" Inherits="ExpoOrders.Web.CustomControls.FormQuestionEditorList" %>

    <asp:GridView EnableViewState="true" runat="server" ID="grdvwFormQuestionList" AllowPaging="false" AllowSorting="false"
                AutoGenerateColumns="false" CellPadding="2" CellSpacing="0" AlternatingRowStyle-CssClass="altItem"
                RowStyle-CssClass="item" OnRowDataBound="grdvwFormQuestionList_RowDataBound" GridLines="None"
                OnRowCommand="grdvwFormQuestionList_RowCommand" EmptyDataText="There are currently no questions associated.">
                <Columns>
                    <asp:TemplateField HeaderText="Order">
                        <ItemTemplate>
                            <asp:Literal ID="ltrQuestionSortOrder" Text="" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Question Text">
                        <ItemTemplate>
                             <asp:LinkButton Visible="true" ID="lbtnManageQuestion" Text='<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "QuestionText").ToString())%>' CommandName="EditQuestion"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "QuestionId")%>' runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Response Type">
                        <ItemTemplate>
                            <asp:Literal ID="ltrQuestionResponseType" Text="" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Required?">
                        <ItemTemplate>
                            <asp:Literal ID="ltrQuestionRequired" Text="" runat="server" /></ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton Visible="true" ID="lbtnDeleteQuestion" Text="Delete" CommandName="DeleteQuestion"
                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "QuestionId")%>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>