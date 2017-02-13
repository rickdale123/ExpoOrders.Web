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
    public partial class ModalDialog : BaseOwnerPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        private void InitializePage()
        {
            this.OwnerStyleSheet.Href = string.Format("~/Assets/Shows/{0}/style.css", CurrentUser.CurrentShow.ShowGuid);

            plcCascadingPriceUpdates.Visible = true;
            ltrModalDialogTitle.Text = "Cascading Price Updates";

            hdnShowId.Value = CurrentUser.CurrentShow.ShowId.ToString();

            int categoryId = Util.ConvertInt32(Request.QueryString["CategoryId"]);
            if (categoryId > 0)
            {
                ltrModalDialogTitle.Text += string.Format(" (category: {0})", categoryId);
                hdnCategoryId.Value = categoryId.ToString();
            }

        }

        protected void btnApplyPricingUpdate_Click(object sender, EventArgs e)
        {
            var includeDiscountPrice = rdoUpdateDiscountPrice.SelectedValue;

            OwnerAdminController ownerCntrl = new OwnerAdminController();

            int categoryId = Util.ConvertInt32(hdnCategoryId.Value);
            ValidationResults errors = ownerCntrl.CascadePriceUpdate(CurrentUser, Util.ConvertInt32(hdnShowId.Value), categoryId, Util.ConvertDecimal(this.txtPercentPriceUpdate.Text.Trim()), rdoUpdateDiscountPrice.SelectedValue == "1", rdoUpdateEarlyBirdPrice.SelectedValue == "1");

            if (errors.IsValid)
            {
                if (categoryId > 0)
                {
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "closeWindow", "<script language='javascript'>priceUpdatedCallBack(" + categoryId.ToString() + ");</script>");
                }
                else
                {
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "closeWindow", "<script language='javascript'>closeMe();</script>");
                }
                
            }
            else
            {
                this.PageErrors.AddErrorMessages(errors);
            }
        }
    }
}