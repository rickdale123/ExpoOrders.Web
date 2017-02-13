using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Entities;
using ExpoOrders.Web.Owners.Common;
using ExpoOrders.Common;
using ExpoOrders.Controllers;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Reporting.WebForms;
using System.Drawing;
using System.Web.UI.HtmlControls;

namespace ExpoOrders.Web.Owners
{
    public partial class Payments : BaseOwnerPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MaintainScrollPositionOnPostBack = true;

            this.Master.NavigationItemCallBack = this.HandleNavigationItemClicked;
            this.Master.PreviewShowCallBack = this.PreviewShow;

            if (!IsPostBack)
            {
                LoadPage();
            }
            else
            {
                if (!string.IsNullOrEmpty(lblExhibitorId.Text))
                {
                    btnRefreshPage.Visible = true;
                }
                else
                {
                    btnRefreshPage.Visible = false;
                }
            }
        }

        private void LoadPage()
        {
            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, OwnerPage.Orders, OwnerTabEnum.Orders);
            this.Master.LoadSubNavigation("Payments", OwnerUtil.BuildOrdersSubNavLinks());
            this.Master.SelectNavigationItem(2);

            if (LinkToOrderId > 0)
            {
                LoadFromOrderId(LinkToOrderId);
            }
            else if (LinkToExhibitorId > 0)
            {
                LoadPaymentDetail(LinkToExhibitorId);
            }
            else
            {
                InitializeOrderSearch();
            }
        }

        private void LoadFromOrderId(int orderId)
        {
            OrderAdminController orderCntrl = new OrderAdminController();
            Order currentOrder = orderCntrl.GetExhibitorOrder(orderId);
            if (currentOrder != null && currentOrder.ExhibitorId.HasValue)
            {
                LoadPaymentDetail(currentOrder.ExhibitorId.Value);
            }
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            LoadPage();
        }

        private void InitializeOrderSearch()
        {
            plcExhibitorList.Visible = true;
            plcPaymentDetail.Visible = false;
            plcPendingPayments.Visible = false;

            LoadExhibitorSearchList(cboSearchExhibitorId);
        }

        private void LoadPaymentDetail(int exhibitorId)
        {
            bool showInactive = chkShowAllInactive.Checked;

            lnkPrintInvoice.Attributes.Add("onclick", string.Format("launchExhibitorInvoice({0}, {1}, {2}); return false;", (int)ReportEnum.ExhibitorInvoice, CurrentUser.CurrentShow.ShowId, exhibitorId));
            lbtnViewCallLogs.Visible = true;
            lbtnViewEmailLogs.Visible = true;

            CurrentExhibitorId = exhibitorId;

            plcCreditCardSweepResults.Visible = false;
            plcPaymentDetail.Visible = true;
            plcPendingPayments.Visible = false;
            DisplayExhibitorList(false);
            plcCreditCardSweepResults.Visible = false;

            PaymentController cntrl = new PaymentController();
            ExhibitorPaymentDetails details = cntrl.GetExhibitorPaymentDetail(exhibitorId);

            ResetClassificationDropDown(plcClassification, ddlClassification);

            if (details != null && details.ExhibitorDetail != null)
            {
                plcPaymentDetail.Visible = true;
                lblExhibitorId.Text = details.ExhibitorDetail.ExhibitorId.ToString();
                lnkBtnExhibitorName.Text = details.ExhibitorDetail.ExhibitorCompanyName;
                lnkBtnExhibitorName.CommandArgument = details.ExhibitorDetail.ExhibitorId.ToString();

                SelectClassification(ddlClassification, details.ExhibitorDetail.Classification);

                lblBoothNumber.Text = string.Format("{0} ({1})", details.ExhibitorDetail.BoothNumber, details.ExhibitorDetail.BoothDescription);

                string acceptedOrderStatus = OrderStatusEnum.Accepted.ToString();

                List<Order> creditCardOrders = details.Orders.Where(o => o.PaymentTypeCd == PaymentTypeEnum.CreditCard.ToString() && o.OrderStatusCd == acceptedOrderStatus).ToList();
                if (!showInactive)
                {
                    creditCardOrders = creditCardOrders.Where(o => o.ActiveFlag == true).ToList();
                }

                Decimal totalCreditCardOrders = SumOrderTotals(creditCardOrders);
                lblTotalCreditCardOrders.Text = Util.FormatAmountDisplay(totalCreditCardOrders);
                grdvwCreditCardOrders.DataSource = creditCardOrders;
                grdvwCreditCardOrders.DataBind();

                List<Order> checkOrders = details.Orders.Where(o => o.PaymentTypeCd == PaymentTypeEnum.Check.ToString() && o.OrderStatusCd == acceptedOrderStatus).ToList();
                if (!showInactive)
                {
                    checkOrders = checkOrders.Where(o => o.ActiveFlag == true).ToList();
                }
                Decimal totalCheckOrders = SumOrderTotals(checkOrders);
                lblTotalCheckOrders.Text = Util.FormatAmountDisplay(totalCheckOrders);
                grdvwCheckOrders.DataSource = checkOrders;
                grdvwCheckOrders.DataBind();

                List<Order> wireOrders = details.Orders.Where(o => o.PaymentTypeCd == PaymentTypeEnum.Wire.ToString() && o.OrderStatusCd == acceptedOrderStatus).ToList();
                if (!showInactive)
                {
                    wireOrders = wireOrders.Where(o => o.ActiveFlag == true).ToList();
                }

                Decimal totalWireOrders = SumOrderTotals(wireOrders);
                lblTotalWireOrders.Text = Util.FormatAmountDisplay(totalWireOrders);
                grdvwWireOrders.DataSource = wireOrders;
                grdvwWireOrders.DataBind();

                List<Order> exhibitorFundsTransferOrders = details.Orders.Where(o => o.PaymentTypeCd == PaymentTypeEnum.EEFT.ToString() && o.OrderStatusCd == acceptedOrderStatus).ToList();
                if (!showInactive)
                {
                    exhibitorFundsTransferOrders = exhibitorFundsTransferOrders.Where(o => o.ActiveFlag == true).ToList();
                }

                Decimal totalExhibitorFundsTransferOrders = SumOrderTotals(exhibitorFundsTransferOrders);
                lblTotalExhibitorFundsTransferOrders.Text = Util.FormatAmountDisplay(totalExhibitorFundsTransferOrders);
                grdvwExhibitorFundsTransferOrders.DataSource = exhibitorFundsTransferOrders;
                grdvwExhibitorFundsTransferOrders.DataBind();


                List<PaymentTransaction> creditCardPayments = details.Payments.Where(p => (p.TransactionTypeCd == TransactionTypeEnum.Authorize.ToString()
                                                || p.TransactionTypeCd == TransactionTypeEnum.Refund.ToString()
                                                || p.TransactionTypeCd == TransactionTypeEnum.Sale.ToString()
                                                || p.TransactionTypeCd == TransactionTypeEnum.Settle.ToString()
                                                || p.TransactionTypeCd == TransactionTypeEnum.Void.ToString()
                                                || p.TransactionTypeCd == TransactionTypeEnum.Manual.ToString()
                                                )
                                            ).ToList();

                if (!showInactive)
                {
                    creditCardPayments = creditCardPayments.Where(p => p.ActiveFlag == true).ToList();
                }

                Decimal totalCreditCardPayments = SumPaymentTotals(creditCardPayments);
                lblTotalCreditCardPayments.Text = Util.FormatAmountDisplay(totalCreditCardPayments);
                grdvwCreditCardPayments.DataSource = creditCardPayments;
                grdvwCreditCardPayments.DataBind();

                List<PaymentTransaction> checkPayments = details.Payments.Where(p => p.TransactionTypeCd == TransactionTypeEnum.CheckPayment.ToString()).ToList();
                if (!showInactive)
                {
                    checkPayments = checkPayments.Where(p => p.ActiveFlag == true).ToList();
                }

                Decimal totalCheckPayments = SumPaymentTotals(checkPayments);
                lblTotalCheckPayments.Text = Util.FormatAmountDisplay(totalCheckPayments);
                grdvwCheckPayments.DataSource = checkPayments;
                grdvwCheckPayments.DataBind();

                List<PaymentTransaction> wirePayments = details.Payments.Where(p => p.TransactionTypeCd == TransactionTypeEnum.Wire.ToString()).ToList();
                if (!showInactive)
                {
                    wirePayments = wirePayments.Where(p => p.ActiveFlag == true).ToList();
                }
                Decimal totalWirePayments = SumPaymentTotals(wirePayments);
                lblTotalWirePayments.Text = Util.FormatAmountDisplay(totalWirePayments);
                grdvwWirePayments.DataSource = wirePayments;
                grdvwWirePayments.DataBind();

                List<PaymentTransaction> exhibitorFundsTransferPayments = details.Payments.Where(p => p.TransactionTypeCd == TransactionTypeEnum.ExhibitorEFT.ToString()).ToList();
                if (!showInactive)
                {
                    exhibitorFundsTransferPayments = exhibitorFundsTransferPayments.Where(p => p.ActiveFlag == true).ToList();
                }
                Decimal totalExhibitorFundsTransferPayments = SumPaymentTotals(exhibitorFundsTransferPayments);
                lblTotalExhibitorFundsTransferPayments.Text = Util.FormatAmountDisplay(totalExhibitorFundsTransferPayments);
                grdvwExhibitorFundsTransferPayments.DataSource = exhibitorFundsTransferPayments;
                grdvwExhibitorFundsTransferPayments.DataBind();



                Decimal grandTotalBalanceAmount = (totalCreditCardOrders + totalCheckOrders + totalWireOrders + totalExhibitorFundsTransferOrders) - (totalCreditCardPayments + totalCheckPayments + totalWirePayments + totalExhibitorFundsTransferPayments);
                lblGrandTotalBalanceAmount.Text = Util.FormatAmountDisplay(grandTotalBalanceAmount);
            }
        }

        private Decimal SumOrderTotals(List<Order> orders)
        {
            Decimal orderTotals = 0;

            orders.ForEach(o =>
            {
                orderTotals += o.OrderTotal.HasValue ? o.OrderTotal.Value : 0;
            });

            return orderTotals;
        }

        private Decimal SumPaymentTotals(List<PaymentTransaction> payments)
        {
            Decimal paymentTotals = 0;

            payments.ForEach(p =>
            {
                paymentTotals += p.PaymentAmount;
            });

            return paymentTotals;
        }

        private void LoadExhibitorList(SearchCriteria searchCriteria)
        {
            plcPendingPayments.Visible = false;
            plcPaymentDetail.Visible = false;
            plcCreditCardSweepResults.Visible = false;
            lblExhibitorListRowCount.Text = "0 rows to display.<br/>";

            PaymentController cntrl = new PaymentController();

            List<ExhibitorTotalOrderPayment> totalOrderPayments = cntrl.SearchExhibitors(CurrentUser.CurrentShow.ShowId, searchCriteria);
            if (totalOrderPayments != null)
            {
                lblExhibitorListRowCount.Text = string.Format("({0} rows displayed.)<br/>", totalOrderPayments.Count);
            }

            grdvwExhibitorList.DataSource = totalOrderPayments;
            grdvwExhibitorList.DataBind();

            DisplayExhibitorList(true);
        }

        private void DisplayExhibitorList(bool visible)
        {
            plcExhibitorList.Visible = grdvwExhibitorList.Visible = visible;
        }

        protected void btnRefreshPage_Click(object sender, EventArgs e)
        {
            LoadPaymentDetail(Util.ConvertInt32(lblExhibitorId.Text));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchCriteria searchCriteria = new SearchCriteria();
            searchCriteria.BoothNumber = txtSearchBoothNumber.Text.Trim();

            searchCriteria.TransactionId = txtSearchTrxId.Text.Trim();

            if (cboSearchExhibitorId.SelectedIndex > 0)
            {
                searchCriteria.ExhibitorId = Util.ConvertInt32(cboSearchExhibitorId.SelectedValue);
            }
            else
            {
                searchCriteria.ExhibitorId = Util.ConvertInt32(txtSearchExhibitorId.Text.Trim());
            }

            LoadExhibitorList(searchCriteria);
        }

        protected void ddlClassification_Changed(object sender, EventArgs e)
        {
            OrderAdminController orderCntrl = new OrderAdminController();
            orderCntrl.SaveClassification(CurrentUser, CurrentExhibitorId, ddlClassification.SelectedValue);
            this.Master.DisplayFriendlyMessage("Classification Saved.");
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearchExhibitorId.Text = string.Empty;
            txtSearchBoothNumber.Text = string.Empty;
            txtSearchTrxId.Text = string.Empty;

            cboSearchExhibitorId.SelectedIndex = 0;

            DisplayExhibitorList(true);
            plcExhibitorList.Visible = true;

        }

        protected void btnFindPendingCreditCardPayments_Click(object sender, EventArgs e)
        {
            plcPaymentDetail.Visible = false;
            plcExhibitorList.Visible = false;

            lblPendingCreditCardPaymentRowCount.Text = "0 rows to display.<br/>";

            OrderAdminController orderCntrl = new OrderAdminController();
            List<OutstandingCreditCardPayment> outstandingPayments = orderCntrl.FindOutstandingCreditCardOrders(CurrentUser.CurrentShow.ShowId);

            if (outstandingPayments != null)
            {
                lblPendingCreditCardPaymentRowCount.Text = string.Format("({0} rows displayed.)<br/>", outstandingPayments.Count);
            }

            grdvwPendingPaymentList.DataSource = outstandingPayments; 
            grdvwPendingPaymentList.DataBind();

            plcPendingPayments.Visible = true;
            plcCreditCardSweepResults.Visible = false;
        }


        protected void grdvwExhibitorList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

            }
        }

        protected void grdvwCreditCardPayments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                PaymentTransaction creditCardTrx = (PaymentTransaction)e.Row.DataItem;

                LinkButton lnkBtnDeleteCreditCardPayment = (LinkButton)e.Row.FindControl("lnkBtnDeleteCreditCardPayment");
                string confirmMsg = string.Format("Are you sure you want to remove this {0} credit card payment?", Util.FormatCurrency(creditCardTrx.PaymentAmount, CurrentUser.CurrentShow.CurrencySymbol));
                lnkBtnDeleteCreditCardPayment.Attributes.Add("onClick", string.Format("return confirm('{0}');", confirmMsg));


                HtmlAnchor lnkViewPaymentDetail = (HtmlAnchor)e.Row.FindControl("lnkViewPaymentDetail");
                string url = string.Format("PaymentDetail.aspx?paymentid={0}", creditCardTrx.PaymentTransactionId);
                lnkViewPaymentDetail.Attributes.Add("onClick", string.Format("openPopupWindow('PaymentDetail', '{0}', 600, 500); return false;", url));

                if (creditCardTrx.ActiveFlag == false)
                {
                    e.Row.BackColor = Color.Red;
                }
            }
        }

        protected void grdvwCreditCardOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DisplayOrderDetail")
            {
                LinkToOrderDetail(Util.ConvertInt32(e.CommandArgument));
            }
        }

        protected void grdvwCheckOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DisplayOrderDetail")
            {
                LinkToOrderDetail(Util.ConvertInt32(e.CommandArgument));
            }
        }

        protected void grdvwWireOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DisplayOrderDetail")
            {
                LinkToOrderDetail(Util.ConvertInt32(e.CommandArgument));
            }
        }

        protected void grdvwExhibitorFundsTransferOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DisplayOrderDetail")
            {
                LinkToOrderDetail(Util.ConvertInt32(e.CommandArgument));
            }
        }


        protected void grdvwExhibitorList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "LoadPaymentDetail")
            {
                LoadPaymentDetail(Util.ConvertInt32(e.CommandArgument));
            }
        }

        protected void grdvwPendingPaymentList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }

        protected void grdvwPendingPaymentList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int exhibitorId = Util.ConvertInt32(e.CommandArgument.ToString().Split(':')[0]);
            int orderId = Util.ConvertInt32(e.CommandArgument.ToString().Split(':')[1]);
            
            if (e.CommandName == "DisplayOrderDetail")
            {
                LinkToOrderDetail(orderId);
            }
            else if (e.CommandName == "LoadPaymentDetail")
            {
                LoadPaymentDetail(exhibitorId);
            }
        }

        protected void grdvwCheckPayments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                PaymentTransaction checkTrx = (PaymentTransaction) e.Row.DataItem;
                LinkButton lnkBtnDeleteCheckPayment = (LinkButton) e.Row.FindControl("lnkBtnDeleteCheckPayment");
                string confirmMsg = string.Format("Are you sure you want to remove this {0} payment for check # {1}?", Util.FormatCurrency(checkTrx.PaymentAmount, CurrentUser.CurrentShow.CurrencySymbol), checkTrx.CheckNumber);
                lnkBtnDeleteCheckPayment.Attributes.Add("onClick", string.Format("return confirm('{0}');", confirmMsg));

                HtmlAnchor lnkViewPaymentDetail = (HtmlAnchor)e.Row.FindControl("lnkViewPaymentDetail");
                string url = string.Format("PaymentDetail.aspx?paymentid={0}", checkTrx.PaymentTransactionId);
                lnkViewPaymentDetail.Attributes.Add("onClick", string.Format("openPopupWindow('PaymentDetail', '{0}', 600, 500); return false;", url));

                if (checkTrx.ActiveFlag == false)
                {
                    e.Row.BackColor = Color.Red;
                }
            }
        }

        protected void grdvwCreditCardPayments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCreditCardPayment")
            {
                PaymentController paymentCntrl = new PaymentController();
                ValidationResults errors = paymentCntrl.DeletePaymentTransaction(CurrentUser, Util.ConvertInt32(e.CommandArgument));
                if (errors.IsValid)
                {
                    LoadPaymentDetail(CurrentExhibitorId);
                }
                else
                {
                    this.MaintainScrollPositionOnPostBack = false;
                    this.PageErrors.AddErrorMessages(errors);
                }
            }
        }

        protected void grdvwCheckPayments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCheckPayment")
            {
                PaymentController paymentCntrl = new PaymentController();
                ValidationResults errors = paymentCntrl.DeletePaymentTransaction(CurrentUser, Util.ConvertInt32(e.CommandArgument));
                if (errors.IsValid)
                {
                    LoadPaymentDetail(CurrentExhibitorId);
                }
                else
                {
                    this.MaintainScrollPositionOnPostBack = false;
                    this.PageErrors.AddErrorMessages(errors);
                }
            }
            else if (e.CommandName == "DisplayOrderDetail")
            {
                LinkToOrderDetail(Util.ConvertInt32(e.CommandArgument));
            }
        }

        protected void grdvwWirePayments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                PaymentTransaction wireTrx = (PaymentTransaction)e.Row.DataItem;
                LinkButton lnkBtnDeleteWirePayment = (LinkButton)e.Row.FindControl("lnkBtnDeleteWirePayment");
                string confirmMsg = string.Format("Are you sure you want to remove payment for this {0} wire transfer?", Util.FormatCurrency(wireTrx.PaymentAmount, CurrentUser.CurrentShow.CurrencySymbol));
                lnkBtnDeleteWirePayment.Attributes.Add("onClick", string.Format("return confirm('{0}');", confirmMsg));

                HtmlAnchor lnkViewPaymentDetail = (HtmlAnchor)e.Row.FindControl("lnkViewPaymentDetail");
                string url = string.Format("PaymentDetail.aspx?paymentid={0}", wireTrx.PaymentTransactionId);
                lnkViewPaymentDetail.Attributes.Add("onClick", string.Format("openPopupWindow('PaymentDetail', '{0}', 600, 500); return false;", url));

                if (wireTrx.ActiveFlag == false)
                {
                    e.Row.BackColor = Color.Red;
                }
            }
        }

        protected void grdvwWirePayments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteWirePayment")
            {
                PaymentController paymentCntrl = new PaymentController();
                ValidationResults errors = paymentCntrl.DeletePaymentTransaction(CurrentUser, Util.ConvertInt32(e.CommandArgument));
                if (errors.IsValid)
                {
                    LoadPaymentDetail(CurrentExhibitorId);
                }
                else
                {
                    this.MaintainScrollPositionOnPostBack = false;
                    this.PageErrors.AddErrorMessages(errors);
                }
            }
            else if (e.CommandName == "DisplayOrderDetail")
            {
                LinkToOrderDetail(Util.ConvertInt32(e.CommandArgument));
            }
        }




        protected void grdvwExhibitorFundsTransferPayments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                PaymentTransaction eeftTrx = (PaymentTransaction)e.Row.DataItem;
                LinkButton lnkBtnDeleteExhibitorFundsTransferPayment = (LinkButton)e.Row.FindControl("lnkBtnDeleteExhibitorFundsTransferPayment");
                string confirmMsg = string.Format("Are you sure you want to remove payment for this {0} exhibitor funds transfer?", Util.FormatCurrency(eeftTrx.PaymentAmount, CurrentUser.CurrentShow.CurrencySymbol));
                lnkBtnDeleteExhibitorFundsTransferPayment.Attributes.Add("onClick", string.Format("return confirm('{0}');", confirmMsg));

                HtmlAnchor lnkViewPaymentDetail = (HtmlAnchor)e.Row.FindControl("lnkViewPaymentDetail");
                string url = string.Format("PaymentDetail.aspx?paymentid={0}", eeftTrx.PaymentTransactionId);
                lnkViewPaymentDetail.Attributes.Add("onClick", string.Format("openPopupWindow('PaymentDetail', '{0}', 600, 500); return false;", url));

                if (eeftTrx.ActiveFlag == false)
                {
                    e.Row.BackColor = Color.Red;
                }
            }
        }

        protected void grdvwExhibitorFundsTransferPayments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteExhibitorFundsTransferPayment")
            {
                PaymentController paymentCntrl = new PaymentController();
                ValidationResults errors = paymentCntrl.DeletePaymentTransaction(CurrentUser, Util.ConvertInt32(e.CommandArgument));
                if (errors.IsValid)
                {
                    LoadPaymentDetail(CurrentExhibitorId);
                }
                else
                {
                    this.MaintainScrollPositionOnPostBack = false;
                    this.PageErrors.AddErrorMessages(errors);
                }
            }
            else if (e.CommandName == "DisplayOrderDetail")
            {
                LinkToOrderDetail(Util.ConvertInt32(e.CommandArgument));
            }
        }



        protected void btnBackToList_Click(object sender, EventArgs e)
        {
            plcExhibitorList.Visible = true;
            plcPaymentDetail.Visible = false;
            plcPendingPayments.Visible = false;
        }

        protected void btnRefreshLists_Click(object sender, EventArgs e)
        {
            LoadPaymentDetail(CurrentExhibitorId);
        }

        protected void btnDisplayAddCreditCardPayment_Click(object sender, EventArgs e)
        {

            plcAddCreditCardPayment.Visible = true;
            btnDisplayAddCreditCardPayment.Visible = false;
            ddlAddCreditCardTransactionType.Focus();

            OrderAdminController orderCntrl = new OrderAdminController();

            List<Order> orderIds = orderCntrl.GetExhibitorOrders(CurrentExhibitorId).Where(o => o.ActiveFlag == true).Where(o=>o.OrderType == OrderTypeEnum.BoothOrder).ToList();

            if (orderIds == null && orderIds.Count <= 0)
            {
                this.PageErrors.AddErrorMessage("Exhibitor must have an Order on file before allowing payments.");
            }
            else
            {
                this.ddlCreditCardOrderId.DataTextField = "OrderId";
                this.ddlCreditCardOrderId.DataValueField = "OrderId";
                this.ddlCreditCardOrderId.DataSource = orderIds;
                this.ddlCreditCardOrderId.DataBind();

                ddlAddCreditCardTransactionType.Items.Clear();
                ddlAddCreditCardTransactionType.Items.Add(new ListItem("Authorize Only", TransactionTypeEnum.Authorize.ToString()));
                ddlAddCreditCardTransactionType.Items.Add(new ListItem("Settle", TransactionTypeEnum.Settle.ToString()));
                ddlAddCreditCardTransactionType.Items.Add(new ListItem("Sale", TransactionTypeEnum.Sale.ToString()));
                ddlAddCreditCardTransactionType.Items.Add(new ListItem("Refund", TransactionTypeEnum.Refund.ToString()));
                ddlAddCreditCardTransactionType.Items.Add(new ListItem("Void", TransactionTypeEnum.Void.ToString()));
                ddlAddCreditCardTransactionType.Items.Add(new ListItem("Manual Credit", TransactionTypeEnum.Manual.ToString()));

                ddlAddCreditCardTransactionType.Items.Insert(0, new ListItem("-- Select One--", string.Empty));

                txtAddCreditCardAmount.Text = string.Empty;
                txtCreditCardTransactionId.Text = string.Empty;
                txtCreditCardTransactionId.Text = string.Empty;
                ltrCreditCardCurrencySymbol.Text = CurrentUser.CurrentShow.CurrencySymbol;
                plcNewOrderItem.Visible = false;
                plcNewExpirationDate.Visible = false;
                txtNewExpirationDate.Text = string.Empty;
                txtAddOrderLineItemDescription.Text = string.Empty;

                ddlCreditCardId.Items.Clear();

                AccountController acctCntrl = new AccountController();
                List<CreditCard> cardList = acctCntrl.GetCreditCardListByExhibitor(CurrentExhibitorId, true).Where(cc => cc.DeletedFlag == null || cc.DeletedFlag == false).ToList();

                ddlCreditCardId.DataValueField = "CreditCardId";
                ddlCreditCardId.DataTextField = "CardListDisplay";
                ddlCreditCardId.DataSource = cardList;
                ddlCreditCardId.DataBind();

                ddlCreditCardId.Items.Insert(0, new ListItem("-- Select One--", string.Empty));
                ddlCreditCardId.Items.Insert(1, new ListItem("[New Card]", "0"));

                plcNewCreditCard.Visible = false;
            }
        }

        private void ResetCreditCardControls()
        {
            plcNewOrderItem.Visible = false;
            plcNewExpirationDate.Visible = false;
            txtNewExpirationDate.Text = string.Empty;

            txtAddOrderLineItemDescription.Text = string.Empty;
            lblCreditCardAmountHelpInfo.Text = string.Empty;
            txtAddCreditCardAmount.Text = "0.00";
        }

        protected void ddlAddCreditCardTransactionType_Changed(object sender, EventArgs e)
        {
            this.MaintainScrollPositionOnPostBack = true;
            txtAddCreditCardAmount.Enabled = true;

            ResetCreditCardControls();

            if (!string.IsNullOrEmpty(ddlAddCreditCardTransactionType.SelectedValue))
            {
                TransactionTypeEnum trxType = Enum<TransactionTypeEnum>.Parse(ddlAddCreditCardTransactionType.SelectedValue);

                plcTransactionId.Visible = false;
                plcCreditCardDetails.Visible = false;
                switch (trxType)
                {
                    case TransactionTypeEnum.Settle:
                        plcTransactionId.Visible = true;
                        break;
                    case TransactionTypeEnum.Refund:
                        plcCreditCardDetails.Visible = plcTransactionId.Visible = true;
                        plcNewOrderItem.Visible = true;
                        plcNewExpirationDate.Visible = true;
                        lblCreditCardAmountHelpInfo.Text = "(enter amount greater than zero)";
                        break;
                    case TransactionTypeEnum.Authorize:
                    case TransactionTypeEnum.Sale:
                        plcCreditCardDetails.Visible = true;
                        break;
                    case TransactionTypeEnum.Void:
                        plcTransactionId.Visible = true;
                        txtAddCreditCardAmount.Enabled = false;
                        break;
                    case TransactionTypeEnum.Manual:
                        plcCreditCardDetails.Visible = plcTransactionId.Visible = true;
                        lblCreditCardAmountHelpInfo.Text = "(enter amount greater than zero)";
                        plcNewOrderItem.Visible = true;
                        break;
                    default:
                        plcCreditCardDetails.Visible = plcTransactionId.Visible = true;
                        break;
                }
            }
        }
        
        protected void ddlCreditCardId_Changed(object sender, EventArgs e)
        {
            plcNewCreditCard.Visible = false;

            if (!string.IsNullOrEmpty(ddlCreditCardId.SelectedValue))
            {
                int creditCardId = Util.ConvertInt32(ddlCreditCardId.SelectedValue);

                AccountController acctCntrl = new AccountController();
                if (creditCardId > 0)
                {
                    CreditCard existingCard = acctCntrl.GetCreditCardById(creditCardId);
                    txtAddCreditCardAmount.Focus();
                }
                else
                {
                    plcNewCreditCard.Visible = true;
                    txtCreditCardName.Focus();
                    //Refresh Credit Card Controls
                    LoadCreditCardExpirationOptions();
                    LoadCreditCardTypes(acctCntrl);

                    OwnerUtil.ClearPlaceHolderControl(plcNewCreditCard);

                    chkSaveCreditCard.Checked = true;
                    hdnCreditCardId.Value = "0";
                    hdnCreditCardAddressId.Value = "0";
                }
            }
            else
            {
                txtAddCreditCardAmount.Focus();
            }
        }

        private void LoadCreditCardTypes(AccountController acctCntrl)
        {
            ddlCreditCardType.Items.Clear();
            ddlCreditCardType.DataSource = acctCntrl.GetCreditCardTypesList(this.CurrentUser.CurrentShow.MerchantAccountConfigId);
            ddlCreditCardType.DataTextField = "Name";
            ddlCreditCardType.DataValueField = "CreditCardTypeCd";
            ddlCreditCardType.DataBind();
            ddlCreditCardType.Items.Insert(0, new ListItem { Text = "-- Select One --", Value = "-1" });
        }

        private void LoadCreditCardExpirationOptions()
        {
            this.ddlCreditCardExpMonth.Items.Clear();
            for (int iMonth = 1; iMonth <= 12; iMonth++)
            {
                this.ddlCreditCardExpMonth.Items.Add(new ListItem(iMonth.ToString().PadLeft(2, '0'), iMonth.ToString()));
            }

            this.ddlCreditCardExpYear.Items.Clear();
            for (int iYear = 0; iYear <= 10; iYear++)
            {
                int yearItem = DateTime.Now.Year + iYear;
                this.ddlCreditCardExpYear.Items.Add(new ListItem(yearItem.ToString(), yearItem.ToString()));
            }
        }

        private void HideCreditCardPaymentDetails()
        {
            plcAddCreditCardPayment.Visible = false;
            btnDisplayAddCreditCardPayment.Visible = true;
            this.MaintainScrollPositionOnPostBack = false;
        }

        protected void btnCancelAddCreditCardPayment_Click(object sender, EventArgs e)
        {
            HideCreditCardPaymentDetails();
        }

        protected void btnAddCreditCardPayment_Click(object sender, EventArgs e)
        {
            PaymentTransaction creditCardTrx = new PaymentTransaction();
            creditCardTrx.ExhibitorId = CurrentExhibitorId;
            creditCardTrx.OrderId = Util.ConvertInt32(ddlCreditCardOrderId.SelectedValue);
            creditCardTrx.TransactionTypeCd = ddlAddCreditCardTransactionType.SelectedValue;

            if (plcTransactionId.Visible)
            {
                creditCardTrx.TransactionId = this.txtCreditCardTransactionId.Text.Trim();
            }

            if (plcCreditCardDetails.Visible)
            {
                if (!string.IsNullOrEmpty(ddlCreditCardId.SelectedValue))
                {
                    creditCardTrx.CreditCardId = Util.ConvertInt32(ddlCreditCardId.SelectedValue);
                }

                if (plcNewCreditCard.Visible)
                {
                    creditCardTrx.CreditCard = BuildCreditCard(CurrentExhibitorId);
                    creditCardTrx.CreditCard.Address = BuildCreditBillingAddress();
                    creditCardTrx.SaveNewCreditCard = chkSaveCreditCard.Checked;
                }
            }

            if (plcNewOrderItem.Visible && txtAddOrderLineItemDescription.Visible && !string.IsNullOrEmpty(txtAddOrderLineItemDescription.Text.Trim()))
            {
                creditCardTrx.NewOrderLineItemDescription = txtAddOrderLineItemDescription.Text.Trim();
            }
            
            creditCardTrx.TransactionAmount = Util.ConvertDecimal(txtAddCreditCardAmount.Text.Trim());


            if (plcNewExpirationDate.Visible)
            {
                if (!string.IsNullOrEmpty(txtNewExpirationDate.Text))
                {
                    creditCardTrx.NewCreditCardExpirationDate = txtNewExpirationDate.Text.Trim();
                }
            }

            PaymentController payCntrl = new PaymentController();

            ValidationResults errors = payCntrl.ProcessManualCreditCardTransaction(CurrentUser, creditCardTrx, string.Empty);

            this.LoadPaymentDetail(CurrentExhibitorId);

            if (errors.IsValid)
            {
                ResetCreditCardControls();

                HideCreditCardPaymentDetails();

                txtAddCreditCardAmount.Text = string.Empty;
                this.Master.DisplayFriendlyMessage("Credit Card Transaction Processed.");
            }
            else
            {
                this.MaintainScrollPositionOnPostBack = false;
                this.PageErrors.AddErrorMessages(errors);
            }
        }

        private CreditCard BuildCreditCard(int exhibitorId)
        {
            CreditCard creditCard = new CreditCard();

            creditCard.ExhibitorId = exhibitorId;

            creditCard.CreditCardId = Int32.Parse(hdnCreditCardId.Value);
            creditCard.NameOnCard = txtCreditCardName.Text.Trim();
            creditCard.SetCreditCardNumber(txtCreditCardNumber.Text.Trim());
            creditCard.CreditCardTypeCd = ddlCreditCardType.SelectedValue;
            creditCard.ExpirationMonth = ddlCreditCardExpMonth.SelectedValue;
            creditCard.ExpirationYear = ddlCreditCardExpYear.SelectedValue;
            creditCard.SetCreditCardSecurityCode(txtCreditCardSecurityCode.Text.Trim());
            creditCard.EmailAddress = txtCreditCardEmail.Text.Trim();

            return creditCard;
        }

        private Address BuildCreditBillingAddress()
        {
            Address billingAddress = new Address();
            billingAddress.AddressId = Int32.Parse(hdnCreditCardAddressId.Value);
            billingAddress.Street1 = txtCreditCardAddressLine1.Text.Trim();
            billingAddress.Street2 = txtCreditCardAddressLine2.Text.Trim();
            billingAddress.City = txtCreditCardCity.Text.Trim();
            billingAddress.StateProvinceRegion = txtCreditCardState.Text.Trim();
            billingAddress.PostalCode = txtCreditCardPostalCode.Text.Trim();
            billingAddress.Country = txtCreditCardCountry.Text.Trim();
            return billingAddress;
        }

        protected void btnDisplayAddCheckPayment_Click(object sender, EventArgs e)
        {

            plcAddCheckPayment.Visible = true;
            btnDisplayAddCheckPayment.Visible = false;
            OwnerUtil.ClearPlaceHolderControl(plcAddCheckPayment);

            ltrCheckCurrencySymbol.Text = CurrentUser.CurrentShow.CurrencySymbol;
            txtAddCheckAmount.Text = string.Empty;
            txtCheckReceiveDate.Text = DateTime.Now.ToShortDateString();
            txtCheckNumber.Text = string.Empty;
            txtCheckPayor.Text = string.Empty;

            OrderAdminController orderCntrl = new OrderAdminController();

            List<Order> orderIds = orderCntrl.GetExhibitorOrders(CurrentExhibitorId);

            if (orderIds == null && orderIds.Count <= 0)
            {
                this.PageErrors.AddErrorMessage("Exhibitor must have an Order on file before allowing payments.");
            }
            else
            {
                this.ddlCheckOrderId.DataTextField = "OrderId";
                this.ddlCheckOrderId.DataValueField = "OrderId";
                this.ddlCheckOrderId.DataSource = orderIds;
                this.ddlCheckOrderId.DataBind();
            }
        }

        protected void btnAddCheckPayment_Click(object sender, EventArgs e)
        {
            PaymentController cntrl = new PaymentController();

            PaymentTransaction trx = new PaymentTransaction();
            trx.OrderId = Util.ConvertInt32(ddlCheckOrderId.SelectedValue);

            trx.ExhibitorId = CurrentExhibitorId;
            trx.ActiveFlag = true;
            trx.CheckReceivedDate = Util.ConvertNullDateTime(txtCheckReceiveDate.Text.Trim());
            trx.CheckPayor = txtCheckPayor.Text.Trim();
            trx.CheckNumber = txtCheckNumber.Text.Trim();
            trx.PaymentAmount = Util.ConvertDecimal(txtAddCheckAmount.Text.Trim());
            trx.SuccessFlag = true;

            ValidationResults errors = cntrl.AddCheckPayment(CurrentUser, trx);

            if (errors.IsValid)
            {
                this.Master.DisplayFriendlyMessage("Check Payment Added.");
                LoadPaymentDetail(CurrentExhibitorId);
                HideCheckPaymentDetails();
            }
            else
            {
                this.MaintainScrollPositionOnPostBack = false;
                this.PageErrors.AddErrorMessages(errors);
            }
        }

        private void HideCheckPaymentDetails()
        {
            plcAddCheckPayment.Visible = false;
            btnDisplayAddCheckPayment.Visible = true;
            this.MaintainScrollPositionOnPostBack = false;
        }

        protected void btnCancelAddCheckPayment_Click(object sender, EventArgs e)
        {
            HideCheckPaymentDetails();
        }

        protected void btnDisplayAddWirePayment_Click(object sender, EventArgs e)
        {
            plcAddWirePayment.Visible = true;
            btnDisplayAddWirePayment.Visible = false;
            OwnerUtil.ClearPlaceHolderControl(plcAddWirePayment);

            txtAddWireAmount.Text = string.Empty;
            ltrWireCurrencySymbol.Text = CurrentUser.CurrentShow.CurrencySymbol;
            txtWireReceiveDate.Text = DateTime.Now.ToShortDateString();
            txtWireDetails.Text = string.Empty;

            OrderAdminController orderCntrl = new OrderAdminController();

            List<Order> orderIds = orderCntrl.GetExhibitorOrders(CurrentExhibitorId);

            if (orderIds == null && orderIds.Count <= 0)
            {
                this.PageErrors.AddErrorMessage("Exhibitor must have an Order on file before allowing payments.");
            }
            else
            {
                this.ddlWireOrderId.DataTextField = "OrderId";
                this.ddlWireOrderId.DataValueField = "OrderId";
                this.ddlWireOrderId.DataSource = orderIds;
                this.ddlWireOrderId.DataBind();
            }
        }

        protected void btnDisplayAddExhibitorFundsTransferPayment_Click(object sender, EventArgs e)
        {
            plcAddExhibitorFundsTransferPayment.Visible = true;
            btnDisplayAddExhibitorFundsTransferPayment.Visible = false;
            OwnerUtil.ClearPlaceHolderControl(plcAddExhibitorFundsTransferPayment);

            txtAddExhibitorFundsTransferAmount.Text = string.Empty;
            ltrExhibitorFundsTransferCurrencySymbol.Text = CurrentUser.CurrentShow.CurrencySymbol;
            txtExhibitorFundsTransferReceivedDate.Text = DateTime.Now.ToString();
            txtExhibitorFundsTransferDetails.Text = string.Empty;

            OrderAdminController orderCntrl = new OrderAdminController();

            List<Order> orderIds = orderCntrl.GetExhibitorOrders(CurrentExhibitorId);

            if (orderIds == null && orderIds.Count <= 0)
            {
                this.PageErrors.AddErrorMessage("Exhibitor must have an Order on file before allowing payments.");
            }
            else
            {
                this.ddlExhibitorFundsTransferOrderId.DataTextField = "OrderId";
                this.ddlExhibitorFundsTransferOrderId.DataValueField = "OrderId";
                this.ddlExhibitorFundsTransferOrderId.DataSource = orderIds;
                this.ddlExhibitorFundsTransferOrderId.DataBind();
            }
           
        }

        protected void btnAddWirePayment_Click(object sender, EventArgs e)
        {
            PaymentController cntrl = new PaymentController();

            PaymentTransaction trx = new PaymentTransaction();
            trx.OrderId = Util.ConvertInt32(ddlWireOrderId.SelectedValue);

            trx.ExhibitorId = CurrentExhibitorId;
            trx.ActiveFlag = true;
            trx.WireReceivedDate = Util.ConvertNullDateTime(txtWireReceiveDate.Text.Trim());
            trx.WireDetails = txtWireDetails.Text.Trim();
            trx.PaymentAmount = Util.ConvertDecimal(txtAddWireAmount.Text.Trim());
            trx.SuccessFlag = true;

            ValidationResults errors = cntrl.AddWirePayment(CurrentUser, trx);

            if (errors.IsValid)
            {
                this.Master.DisplayFriendlyMessage("Wire Payment Added.");
                LoadPaymentDetail(CurrentExhibitorId);
                HideWirePaymentDetails();
            }
            else
            {
                this.MaintainScrollPositionOnPostBack = false;
                this.PageErrors.AddErrorMessages(errors);
            }
        }

        protected void btnAddExhibitorFundsTransferPayment_Click(object sender, EventArgs e)
        {
            PaymentController cntrl = new PaymentController();

            PaymentTransaction trx = new PaymentTransaction();
            trx.OrderId = Util.ConvertInt32(ddlExhibitorFundsTransferOrderId.SelectedValue);

            trx.ExhibitorId = CurrentExhibitorId;
            trx.ActiveFlag = true;
            trx.EEFTReceivedDate = Util.ConvertNullDateTime(txtExhibitorFundsTransferReceivedDate.Text.Trim());
            trx.EEFTDetails = txtExhibitorFundsTransferDetails.Text.Trim();
            trx.PaymentAmount = Util.ConvertDecimal(txtAddExhibitorFundsTransferAmount.Text.Trim());
            trx.SuccessFlag = true;

            ValidationResults errors = cntrl.AddExhibitorFundsTransferPayment(CurrentUser, trx);

            if (errors.IsValid)
            {
                this.Master.DisplayFriendlyMessage("Exhibitor Funds Transfer Payment Added.");
                LoadPaymentDetail(CurrentExhibitorId);
                HideExhibitorFundsTransferPaymentDetails();
            }
            else
            {
                this.MaintainScrollPositionOnPostBack = false;
                this.PageErrors.AddErrorMessages(errors);
            }
        }

        private void HideExhibitorFundsTransferPaymentDetails()
        {
            plcAddExhibitorFundsTransferPayment.Visible = false;
            btnDisplayAddExhibitorFundsTransferPayment.Visible = true;
            this.MaintainScrollPositionOnPostBack = false;
        }

        private void HideWirePaymentDetails()
        {
            plcAddWirePayment.Visible = false;
            btnDisplayAddWirePayment.Visible = true;
            this.MaintainScrollPositionOnPostBack = false;
        }

        protected void btnCancelAddExhibitorFundsTransferPayment_Click(object sender, EventArgs e)
        {
            HideExhibitorFundsTransferPaymentDetails();
        }

        protected void btnCancelAddWirePayment_Click(object sender, EventArgs e)
        {
            HideWirePaymentDetails();
        }

        protected void lnkBtnExhibitorName_Click(object sender, EventArgs e)
        {
            if (sender is LinkButton)
            {
                LinkToExhibitorDetail(Util.ConvertInt32(((LinkButton)sender).CommandArgument));
            }
        }

        protected void btnSendInvoice_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "SendInvoiceInformation";

            int exhibitorId = Util.ConvertInt32(lblExhibitorId.Text.Trim());

            if (exhibitorId > 0)
            {
                ValidationResults errors = base.SendExhibitorInvoice(CurrentUser, exhibitorId);
                if (!errors.IsValid)
                {
                    PageErrors.AddErrorMessages(errors);
                }
            }
        }

        protected void btnSweepOrders_Click(object sender, EventArgs e)
        {
            lblCreditCardSweepRowCount.Text = string.Empty;

            List<OutstandingCreditCardPayment> selectedOrders = new List<OutstandingCreditCardPayment>();
            foreach (GridViewRow row in grdvwPendingPaymentList.Rows)
            {
                CheckBox chkPendingPaymentSelected = (CheckBox)row.FindControl("chkPendingPaymentSelected");
                HiddenField hdnOrderId = (HiddenField)row.FindControl("hdnOrderId");

                if (chkPendingPaymentSelected.Checked)
                {

                    selectedOrders.Add(new OutstandingCreditCardPayment(){
                        OrderId = Util.ConvertInt32(hdnOrderId.Value)
                    });
                }
            }

            lblCreditCardSweepRowCount.Text = "0 rows to display.<br/>";

            if (selectedOrders.Count > 0)
            {
                PaymentController paymentCntrl = new PaymentController();
                List<OutstandingCreditCardPayment> sweepResults = paymentCntrl.SweepCreditCardOrders(CurrentUser, selectedOrders, string.Empty);
                if (sweepResults != null)
                {
                    lblCreditCardSweepRowCount.Text = string.Format("({0} rows displayed.)<br/>", sweepResults.Count);
                }

                grdvSweepResults.DataSource = sweepResults;
                grdvSweepResults.DataBind();

                plcPendingPayments.Visible = false;
                plcCreditCardSweepResults.Visible = true;
            }
        }

        protected void grdvSweepResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                OutstandingCreditCardPayment sweep = (OutstandingCreditCardPayment)e.Row.DataItem;
                if (!sweep.SweepErrors.IsValid)
                {
                    e.Row.CssClass = "rowHighlightError";
                }
            }
        }

        protected void grdvSweepResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DisplayOrderDetail")
            {
                LinkToOrderDetail(Util.ConvertInt32(e.CommandArgument));
            }
            else if (e.CommandName == "LoadPaymentDetail")
            {
                LoadPaymentDetail(Util.ConvertInt32(e.CommandArgument));
            }
        }

        protected void lbtnManageCreditCards_Click(object sender, EventArgs e)
        {
            JumpMode = "ManageCreditCards";
            ReturnPage = "Payments";
            LinkToExhibitorDetail(CurrentExhibitorId);
        }

        protected void lbtnViewCallLogs_Click(object sender, EventArgs e)
        {
            LaunchCallLogViewer(null, CurrentExhibitorId);
        }

        protected void lbtnViewEmailLogs_Click(object sender, EventArgs e)
        {
            LaunchEmailLogViewer(null, CurrentExhibitorId);
        }

        private int CurrentExhibitorId
        {
            get { return Util.ConvertInt32(hdnExhibitorId.Value); }
            set { hdnExhibitorId.Value = value.ToString(); }
        }
      
    }
}