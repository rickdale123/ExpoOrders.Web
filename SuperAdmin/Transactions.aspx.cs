using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Entities;
using ExpoOrders.Controllers;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class Transactions : System.Web.UI.Page
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
            this.txtTransactionDate.SelectedDate = DateTime.Now;
            this.plcTransactionLogs.Visible = false;
        }

        private void BindSearchResults(bool searchAll)
        {
            this.lblTransactionLogRowCount.Text = "0 rows to display.<br/>";

            SearchCriteria searchCriteria = BuildSearchCriteria(searchAll);
            SuperAdminController cntrl = new SuperAdminController();

            List<PaymentTransaction> trxs = cntrl.GetPaymentTransactions(searchCriteria).OrderBy(pt => pt.TransactionDate).ToList();

            if (trxs != null && trxs.Count > 0)
            {
                lblTransactionLogRowCount.Text = string.Format("({0} rows displayed.)<br/>", trxs.Count);
            }

            this.grdvTrxLog.DataSource = trxs;
            this.grdvTrxLog.DataBind();

            this.plcTransactionLogs.Visible = true;
        }

        private SearchCriteria BuildSearchCriteria(bool searchAll)
        {
            SearchCriteria search = new SearchCriteria();
            if (!searchAll)
            {
                search.StartDate = this.txtTransactionDate.SelectedDate;
            }

            //if (chkOnlyShowErrors.Checked)
            //{
            //    search.StatusCode = "E";
            //}

            return search;
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            BindSearchResults(false);
        }

        protected void grdvTrxLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                PaymentTransaction pt = (PaymentTransaction)e.Row.DataItem;

                string owner = pt.RequestParamsDecrypted;
            }
        }
    }
}