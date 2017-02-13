using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Controllers;
using ExpoOrders.Common;
using ExpoOrders.Entities;
using ExpoOrders.Web.CustomControls;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class ShowStats : BaseSuperAdminPage
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            LoadShowStats();

        }

        private void LoadShowStats()
        {
            lblListRowCount.Text = "0 rows to display.";
            lblInvoiceTotals.Text = string.Empty;

            plcShowStats.Visible = true;
            plcShowStatDetail.Visible = false;

            List<ShowStatistics> showStats = Cntrl.SearchShowStats(CurrentUser, chkExcludeRemoved.Checked, chkExcludeAlliance.Checked, chkExcludeReceived.Checked);

            if (showStats != null)
            {

                if (rdoListSortBy.SelectedValue == "0")
                {
                    showStats = showStats.OrderBy(s => s.OwnerName).ThenBy(s => s.ShowStartDate).ToList();
                }
                else if (rdoListSortBy.SelectedValue == "1")
                {
                    showStats = showStats.OrderBy(s => s.ShowStartDate).ThenBy(s => s.OwnerName).ToList();
                }
                lblListRowCount.Text = string.Format("({0} rows displayed.)", showStats.Count);

                decimal invoiceTotal = SumInvoiceTotal(showStats);
                decimal paymentsReceived = SumPaymentsReceived(showStats);
                decimal outstandingBalance = invoiceTotal - paymentsReceived;
                lblInvoiceTotals.Text = string.Format("Invoice Total {0} - Payments Received {1} = Outstanding Balance {2}", Util.FormatCurrency(invoiceTotal), Util.FormatCurrency(paymentsReceived), Util.FormatCurrency(outstandingBalance));
            }

            grdvwShowStats.DataSource = showStats;
            grdvwShowStats.DataBind();

            grdvwShowStats.Visible = true;
            hdnShowId.Value = string.Empty;
        }

        private decimal SumInvoiceTotal(List<ShowStatistics> showStats)
        {
            decimal invoiceTotal = 0;
            if (showStats != null)
            {
                showStats.ForEach(s =>
                {
                    if (s.InvoiceAmount.HasValue)
                    {
                        invoiceTotal += s.InvoiceAmount.Value;
                    }
                }
                );
            }
            return invoiceTotal;
        }
        private decimal SumPaymentsReceived(List<ShowStatistics> showStats)
        {
            decimal totalPaymentsReceived = 0;

            if (showStats != null)
            {
                showStats.ForEach(s =>
                {
                    if (s.AmountPaid.HasValue)
                    {
                        totalPaymentsReceived += s.AmountPaid.Value;
                    }
                }
                );
            }

            return totalPaymentsReceived;
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadShowStats();
        }

        protected void btnSaveShowStat_Click(object sender, EventArgs e)
        {
            SaveShowStat();
        }

        protected void btnCancelShowStat_Click(object sender, EventArgs e)
        {
            LoadShowStats();
        }

        protected void chkExclude_CheckChanged(object sender, EventArgs e)
        {
            LoadShowStats();
        }

        protected void grdvwShowStats_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int showId = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "EditShowStat":
                    EditShowStat(showId);
                    break;
                default:
                    throw new Exception(e.CommandName + " " + e.CommandArgument);
            }
        }

        private void EditShowStat(int showId)
        {
            Show show = Cntrl.GetShowById(showId);

            lblShowId.Text = string.Empty;
            lblShowName.Text = string.Empty;
            lblOwnerName.Text = string.Empty;
            txtInvoicedAmount.Text = string.Empty;
            txtInvoiceDate.SelectedDate = null;
            txtAmountPaid.Text = string.Empty;
            txtPaymentReceiveDate.SelectedDate = null;
            ddlShowInvoiceStatus.ClearSelection();

            if (show != null)
            {
             
                plcShowStats.Visible = false;
                plcShowStatDetail.Visible = true;

                hdnShowId.Value = showId.ToString();

                lblShowId.Text = show.ShowId.ToString();
                lblShowName.Text = show.ShowName;
                lblOwnerName.Text = show.Owner.OwnerName;
                txtInvoicedAmount.Text = Util.FormatAmount(show.InvoiceAmount);
                if (show.InvoiceDate != null)
                {
                    txtInvoiceDate.SelectedDate = show.InvoiceDate;
                }
                
                txtAmountPaid.Text = Util.FormatAmount(show.AmountPaid);
                if (show.PaymentReceiveDate != null)
                {
                    txtPaymentReceiveDate.SelectedDate = show.PaymentReceiveDate;
                }
                
                WebUtil.SelectListItemByValue(ddlShowInvoiceStatus, show.InvoiceStatus);
            }
        }

        private void SaveShowStat()
        {
            int showId = Util.ConvertInt32(hdnShowId.Value);
            Show showStats = new Show();
            showStats.ShowId = showId;
            showStats.InvoiceAmount = Util.ConvertNullDecimal(txtInvoicedAmount.Text.Trim());
            showStats.InvoiceDate = txtInvoiceDate.SelectedDate;
            showStats.AmountPaid = Util.ConvertNullDecimal(txtAmountPaid.Text.Trim());
            showStats.PaymentReceiveDate = txtPaymentReceiveDate.SelectedDate;
            showStats.InvoiceStatus = ddlShowInvoiceStatus.SelectedValue;
            ValidationResults errors = Cntrl.SaveShowStat(CurrentUser, showStats);
            if (errors.IsValid)
            {
                LoadShowStats();
            }
            else
            {
                this.PageErrors.AddErrorMessages(errors);
            }
        }
    }
}