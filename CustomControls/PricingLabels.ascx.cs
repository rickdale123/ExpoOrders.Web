using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Entities;
using ExpoOrders.Common;

namespace ExpoOrders.Web.CustomControls
{
    public partial class PricingLabels : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Populate(ExpoOrdersUser currentUser, Product productDetail)
        {
            this.Visible = true;
            //Start: Wacky Pricing Labels (hierarchical, with rollovers from Advanced to Standard)
            DateTime dteNow = DateTime.Now;

            //Show Floor Price (only shown when there is a value provided)
            bool hasShowFloorPrice = false;
            if (productDetail.UnitPrice.HasValue)
            {
                hasShowFloorPrice = true;
                plcShowFloorPricingSection.Visible = true;
                ltrShowFloorPrice.Text = Server.HtmlEncode(Util.FormatUnitPrice(productDetail.UnitPrice, productDetail.UnitDescriptor, currentUser.CurrentShow.CurrencySymbol));
            }

            //Advanced Price (Show it when the Early Bird Price has a value and the date is not passed)
            if (productDetail.EarlyBirdPrice.HasValue && !Util.IsPassedDeadline(productDetail.EarlyBirdDeadline, dteNow))
            {
                plcAdvancedPricingSection.Visible = true;
                ltrAdvancedPrice.Text = Server.HtmlEncode(Util.FormatUnitPrice(productDetail.EarlyBirdPrice, productDetail.UnitDescriptor, currentUser.CurrentShow.CurrencySymbol));
                if (productDetail.EarlyBirdDeadline.HasValue)
                {
                    ltrAdvancedPriceDeadline.Visible = true;
                    ltrAdvancedPriceDeadline.Text = " (" + productDetail.EarlyBirdDeadline.Value.ToShortDateString() + ")";
                }
                else
                {
                    ltrAdvancedPriceDeadline.Visible = false;
                }
            }

            //Standard Price (shown when Standard Pricing is available OR when the Advanced Price has rolled over INTO the Standard price)
            bool displayStandardPricingSection = false;
            Decimal? derivedStandardPrice = productDetail.DiscountUnitPrice;
            DateTime? derivedStandardPriceDeadline = productDetail.DiscountDeadline;

            if (productDetail.DiscountUnitPrice.HasValue)
            {
                displayStandardPricingSection = true;
            }
            else if (!productDetail.DiscountUnitPrice.HasValue
                && productDetail.EarlyBirdPrice.HasValue && Util.IsPassedDeadline(productDetail.EarlyBirdDeadline, dteNow))
            {
                if (hasShowFloorPrice && !productDetail.DiscountDeadline.HasValue || Util.IsPassedDeadline(productDetail.DiscountDeadline, dteNow))
                {
                    displayStandardPricingSection = false;
                }
                else
                {
                    displayStandardPricingSection = true;
                    derivedStandardPrice = productDetail.EarlyBirdPrice;
                    derivedStandardPriceDeadline = productDetail.DiscountDeadline;
                }
            }

            //Hide Standard Pricing when Show Floor Price exists and past the Standard Deadline
            if (productDetail.UnitPrice.HasValue && Util.IsPassedDeadline(derivedStandardPriceDeadline, dteNow))
            {
                displayStandardPricingSection = false;
            }

            if (displayStandardPricingSection)
            {
                if (!hasShowFloorPrice)
                {
                    if (Util.IsPassedDeadline(derivedStandardPriceDeadline, dteNow))
                    {
                        derivedStandardPriceDeadline = null;
                    }
                }
                plcStandardPricingSection.Visible = true;
                ltrStandardPrice.Text = Server.HtmlEncode(Util.FormatUnitPrice(derivedStandardPrice, productDetail.UnitDescriptor, currentUser.CurrentShow.CurrencySymbol));

                if (derivedStandardPriceDeadline.HasValue)
                {
                    ltrStandardPriceDeadline.Visible = true;
                    ltrStandardPriceDeadline.Text = " (" + derivedStandardPriceDeadline.Value.ToShortDateString() + ")";
                }
                else
                {
                    ltrStandardPriceDeadline.Visible = false;
                }
            }
            //END: Wacky Pricing Labels


        }
    }
}