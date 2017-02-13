using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace ExpoOrders.Web.Owners
{
    public partial class PaymentDetail : BaseOwnerPage
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
            HideFriendlyMessage();

            ddlOrderId.Items.Clear();
            txtTransactionDate.Text = string.Empty;
            txtTransactionAmount.Text = string.Empty;
            txtPaymentAmount.Text = string.Empty;
            lblPaymentTransactionNotes.Text = string.Empty;
            trNotes.Visible = false;

            if (Request["paymentid"] != null)
            {
                int paymentTrxId = Util.ConvertInt32(Request["paymentid"]);

                OrderAdminController cntrl = new OrderAdminController();
                PaymentTransaction paymentTrx = cntrl.GetPaymentTransaction(paymentTrxId);

                if (paymentTrx != null)
                {
                    lblPaymentTransactionId.Text = paymentTrx.PaymentTransactionId.ToString();

                    txtTransactionAmount.Text = String.Format("{0:#,##0.00}", paymentTrx.TransactionAmount);
                    txtPaymentAmount.Text = String.Format("{0:#,##0.00}", paymentTrx.PaymentAmount);

                    txtTransactionDate.Text = paymentTrx.TransactionDate.ToShortDateString();

                    txtRequestParams.Text = paymentTrx.RequestParamsDecrypted;
                    txtResponseParams.Text = paymentTrx.ResponseParamsDecrypted;

                    lblPaymentTransactionNotes.Text = Server.HtmlEncode(paymentTrx.Notes);

                    trNotes.Visible = !String.IsNullOrEmpty(paymentTrx.Notes);


                    txtRequestParams.ReadOnly = true;
                    txtResponseParams.ReadOnly = true;

                    txtUserName.Text = string.Empty;
                    if (paymentTrx.UserId.HasValue)
                    {
                        lblUserId.Text = paymentTrx.UserId.ToString();
                        if (paymentTrx.aspnet_Users != null)
                        {
                            ExtendedUserInfo extInfo = paymentTrx.aspnet_Users.ExtendedUserInfos.FirstOrDefault();

                            if (extInfo != null)
                            {
                                txtUserName.Text = string.Format("{0} {1}", extInfo.FirstName, extInfo.LastName);
                            }
                        }
                    }

                    
                    List<Order> orderIds = cntrl.GetExhibitorOrders(paymentTrx.ExhibitorId).Where(o => o.ActiveFlag == true).Where(o => o.OrderType == OrderTypeEnum.BoothOrder).ToList();

                    if (orderIds != null && orderIds.Count > 0)
                    {
                        this.ddlOrderId.DataTextField = "OrderId";
                        this.ddlOrderId.DataValueField = "OrderId";
                        this.ddlOrderId.DataSource = orderIds;
                        this.ddlOrderId.DataBind();
                    }

                    if (paymentTrx.OrderId.HasValue)
                    {
                        WebUtil.SelectListItemByValue(ddlOrderId, paymentTrx.OrderId.Value);
                    }
                    
                }
            }

            LoadStyleSheet(CurrentUser.CurrentShow);
        }

        public void LoadStyleSheet(Entities.Show currentShow)
        {
            this.OwnerStyleSheet.Href = string.Format("~/Assets/Shows/{0}/style.css", currentShow.ShowGuid);
        }

        protected void btnSavePaymentDetail_Click(object sender, EventArgs e)
        {
            OrderAdminController cntrl = new OrderAdminController();

            PaymentTransaction paymentTrx = new PaymentTransaction();
            paymentTrx.PaymentTransactionId = Util.ConvertInt32(lblPaymentTransactionId.Text);
            paymentTrx.OrderId = Util.ConvertInt32(ddlOrderId.SelectedValue);
            paymentTrx.TransactionAmount = Util.ConvertDecimal(txtTransactionAmount.Text.Trim());
            paymentTrx.PaymentAmount = Util.ConvertDecimal(txtPaymentAmount.Text.Trim());
            paymentTrx.TransactionDate = Util.ConvertDateTime(txtTransactionDate.Text.Trim());

            ValidationResults errors = cntrl.UpdatePaymentTransaction(CurrentUser, paymentTrx);

            if (errors.IsValid)
            {
                this.DisplayFriendlyMessage("Payment Trx Details have been saved.");
            }
            else
            {
                this.PageErrors.AddErrorMessages(errors);
            }
        }

        public void HideFriendlyMessage()
        {
            this.plcFriendlyMessage.Visible = this.ltrFriendlyMessage.Visible = false;
            this.ltrFriendlyMessage.Text = string.Empty;
        }

        public void DisplayFriendlyMessage(string message)
        {
            this.plcFriendlyMessage.Visible =
                this.ltrFriendlyMessage.Visible = true;

            this.ltrFriendlyMessage.Text += message + "<br/>";
        }
    }
}