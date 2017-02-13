using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Entities;
using ExpoOrders.Common;
using System.Web.UI.HtmlControls;
using ExpoOrders.Controllers;

namespace ExpoOrders.Web.CustomControls
{
    public enum LineItemAdjustmentEnum { Edit, Delete, Add }

    public partial class OrderDetail : System.Web.UI.UserControl
    {
        private string CurrencySymbolForShow;
        private bool OrderAdjustable = false;

        private Action<int, int, LineItemAdjustmentEnum> _orderAdjust;
        public Action<int, int, LineItemAdjustmentEnum> AdjustOrder
        {
            set
            {
                _orderAdjust = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        public void Populate(BaseController controller, Entities.Order order, bool displayOrderHeader, string currencySymbol, Show show)
        {
            Populate(controller, order, displayOrderHeader, currencySymbol, false, false, show);
        }

        public void Populate(BaseController controller, Entities.Order order, bool displayOrderHeader, string currencySymbol, bool orderAdjustable, bool companyBoothVisible, Show show)
        {
            OrderAdjustable = orderAdjustable;
            this.Visible = true;
            CurrencySymbolForShow = currencySymbol;
            plcOrderHeader.Visible = displayOrderHeader;
            plcCreditCardInfo.Visible = false;
            plcBillingInfo.Visible = false;

            plcCompanyBoothInfo.Visible = companyBoothVisible;
            if (companyBoothVisible)
            {
                lblCompanyNameValue.Text = WebUtil.HtmlEncode(order.Exhibitor.ExhibitorCompanyName);
                lblBoothNumberValue.Text = order.Exhibitor.BoothNumber;

                lblBoothDescriptionValue.Text = order.Exhibitor.BoothDescription;

                //Some clients DO NOT EVER want to display Booth Number or Description (they don't use the concept of 'Booth')
                if (show.DisplayBoothNumberLabel.HasValue && show.DisplayBoothNumberLabel.Value == true)
                {
                    lblBoothNumber.Visible = lblBoothNumberValue.Visible = !string.IsNullOrEmpty(order.Exhibitor.BoothNumber);
                    lblBoothDescription.Visible = lblBoothDescriptionValue.Visible = !string.IsNullOrEmpty(order.Exhibitor.BoothDescription);
                }
                else
                {
                    lblBoothNumber.Visible = lblBoothNumberValue.Visible = false;
                    lblBoothDescription.Visible = lblBoothDescriptionValue.Visible = false;
                }
                

            }

            if (plcOrderHeader.Visible)
            {
                lblOrderDateValue.Text = order.OrderDateDisplay;
                lblUserNameValue.Text = order.OrderedBy;
                lblOrderStatusValue.Text = order.OrderStatusDisplay;
                lblOrderPaymentTypeValue.Text = order.PaymentType.GetDescription();
                lblConfirmationNumberValue.Text = order.ConfirmationNumberDisplay;
            }

            if (order.OrderType == OrderTypeEnum.BoothOrder)
            {
                plcOrderItems.Visible = true;
                plcFormQuestions.Visible = false;

                if (OrderAdjustable)
                {
                    lnkBtnAddOrderLineItem.Visible = true;
                    lnkBtnAddOrderLineItem.CommandArgument = string.Format("{0}:{1}", order.OrderId, 0);
                }

                rptrOrderItems.DataSource = order.OrderItems.Where(i => i.ActiveFlag == true).OrderBy(i => i.ItemIndex).ThenBy(i=> i.OrderItemId);
                rptrOrderItems.DataBind();

                rptrOrderItems.Visible = true;

                lnkPrintReceipt.Visible = true;
                lnkPrintReceipt.Attributes.Add("onclick", string.Format("launchOrderReceipt({0}, {1}); return false;", (int)ReportEnum.OrderReceipt, order.OrderId));

                if (orderAdjustable)
                {
                    this.plcDeliveryReceipt.Visible = lnkPrintDeliveryReceipt.Visible = true;
                    lnkPrintDeliveryReceipt.Attributes.Add("onclick", string.Format("launchOrderReceipt({0}, {1}); return false;", (int)ReportEnum.DeliveryReceipt, order.OrderId));
                }
                

                this.Visible = true;

                Literal ltrOrderTotal = (Literal)rptrOrderItems.Controls[rptrOrderItems.Controls.Count - 1].FindControl("ltrOrderTotal");
                if (ltrOrderTotal != null)
                {
                    ltrOrderTotal.Text = Server.HtmlEncode(Util.FormatCurrency(order.OrderTotal.Value, CurrencySymbolForShow));
                }

                if (order.PaymentType == PaymentTypeEnum.CreditCard)
                {
                    plcPaymentType.Visible = true;

                    if (!String.IsNullOrEmpty(order.BillingAddressStreet1))
                    {
                        plcBillingInfo.Visible = true;

                        lblBillingAddressValue.Text = WebUtil.HtmlEncode(order.BillingAddressStreet1);
                        if (!string.IsNullOrEmpty(order.BillingAddressStreet2))
                        {
                            lblBillingAddressValue.Text += "<br/>" + WebUtil.HtmlEncode(order.BillingAddressStreet2);
                        }
                        lblBillingCity.Text = WebUtil.HtmlEncode(order.BillingAddressCity);
                        lblBillingState.Text = WebUtil.HtmlEncode(order.BillingAddressStateProvinceRegion);
                        lblBillingPostalCode.Text = WebUtil.HtmlEncode(order.BillingAddressPostalCode);
                        lblBillingCountry.Text = WebUtil.HtmlEncode(order.BillingAddressCountry);
                    }

                    if (!string.IsNullOrEmpty(order.CreditCardTypeCd))
                    {
                        plcCreditCardInfo.Visible = true;

                        if (!string.IsNullOrEmpty(order.CreditCardTypeCd))
                        {
                            CreditCardType cardType = controller.GetCreditCardTypeByCode(order.CreditCardTypeCd);
                            if (cardType != null)
                            {
                                lblCreditCardType.Text = cardType.Name;
                            }
                        }

                        lblCreditCardNameOnCard.Text = WebUtil.HtmlEncode(order.CreditCardNameOnCard);
                        lblCreditCardNumber.Text = WebUtil.HtmlEncode(order.CreditCardNumberMasked);
                        lblCreditCardSecurityCode.Text = WebUtil.HtmlEncode(order.CreditCardSecurityCodeDecrypted);
                        lblCreditCardExpirationDate.Text = order.CreditCardExpirationDate;
                    }
                }
            }
            else if (order.OrderType == OrderTypeEnum.Form)
            {
                plcBillingInfo.Visible = false;
                plcPaymentType.Visible = false;
                plcOrderItems.Visible = false;
                plcFormQuestions.Visible = true;

                lnkPrintReceipt.Visible = true;
                lnkPrintReceipt.Attributes.Add("onclick", string.Format("launchOrderReceipt({0}, {1}); return false;", (int) ReportEnum.FormSubmission, order.OrderId));

                rptrFormOrder.DataSource = order.OrderItems;
                rptrFormOrder.DataBind();
                rptrFormOrder.Visible = true;
            }
        }

        public void Populate(BaseController controller, Exhibitor exhibitor, Entities.ShoppingCart shoppingCart, bool displayOrderHeader, string currencySymbol, Show show)
        {
            CurrencySymbolForShow = currencySymbol;

            plcOrderHeader.Visible = displayOrderHeader;
            plcOrderItems.Visible = true;
            plcFormQuestions.Visible = false;

            shoppingCart.CalculateItemSubTotals(exhibitor, DateTime.Now);

            rptrOrderItems.DataSource = shoppingCart.ShoppingCartItems.OrderBy(i1 => i1.Product.Category.SortOrder).ThenBy(i2 => i2.Product.SortOrder);
            rptrOrderItems.DataBind();

            rptrOrderItems.Visible = true;

            this.Visible = true;


            Literal ltrOrderTotal = (Literal)rptrOrderItems.Controls[rptrOrderItems.Controls.Count - 1].FindControl("ltrOrderTotal");
            if (ltrOrderTotal != null)
            {
                ltrOrderTotal.Text = Server.HtmlEncode(Util.FormatCurrency(shoppingCart.OrderTotal.Value, CurrencySymbolForShow));
            }
        }

        protected void rptrOrderItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                PlaceHolder plcLineItemAdjustmentControls = (PlaceHolder)e.Item.FindControl("plcLineItemAdjustmentControls");
                PlaceHolder plcPrintInstallDismantleReport = (PlaceHolder)e.Item.FindControl("plcPrintInstallDismantleReport");
                PlaceHolder plcExhibitorNotes = (PlaceHolder)e.Item.FindControl("plcExhibitorNotes");

                LinkButton lnkBtnEditOrderLineItem = (LinkButton)e.Item.FindControl("lnkBtnEditOrderLineItem");
                LinkButton lnkBtnDeleteOrderLineItem = (LinkButton)e.Item.FindControl("lnkBtnDeleteOrderLineItem");
                HtmlAnchor lnkPrintInstallDismantle = (HtmlAnchor)e.Item.FindControl("lnkPrintInstallDismantle");

                Label lblOrderItemInsertedDateTime = (Label)e.Item.FindControl("lblOrderItemInsertedDateTime");

                Label lblOrderItemId = (Label)e.Item.FindControl("lblOrderItemId");
                
                HtmlTableRow trShoppingCartItem = (HtmlTableRow)e.Item.FindControl("trShoppingCartItem");
                HtmlTableRow trItemAdditionalInfo = (HtmlTableRow)e.Item.FindControl("trItemAdditionalInfo");

                Literal ltrQuantity = (Literal)e.Item.FindControl("ltrQuantity");

                Label lblProductCategory = (Label)e.Item.FindControl("lblProductCategory");
                Label lblProductName = (Label)e.Item.FindControl("lblProductName");
                Label lblExhibitorNotes = (Label)e.Item.FindControl("lblExhibitorNotes");

                PlaceHolder plcRequiredAttribute1 = (PlaceHolder)e.Item.FindControl("plcRequiredAttribute1");
                Label lblRequiredAttribute1 = (Label)e.Item.FindControl("lblRequiredAttribute1");
                PlaceHolder plcRequiredAttribute2 = (PlaceHolder)e.Item.FindControl("plcRequiredAttribute2");
                Label lblRequiredAttribute2 = (Label)e.Item.FindControl("lblRequiredAttribute2");

                PlaceHolder plcRequiredAttribute3 = (PlaceHolder)e.Item.FindControl("plcRequiredAttribute3");
                Label lblRequiredAttribute3 = (Label)e.Item.FindControl("lblRequiredAttribute3");
                PlaceHolder plcRequiredAttribute4 = (PlaceHolder)e.Item.FindControl("plcRequiredAttribute4");
                Label lblRequiredAttribute4 = (Label)e.Item.FindControl("lblRequiredAttribute4");

                Label lblUnitPrice = (Label)e.Item.FindControl("lblUnitPrice");
                Label lblUnitPriceDescription = (Label)e.Item.FindControl("lblUnitPriceDescription");
                PlaceHolder plcUnitPriceDescription = (PlaceHolder)e.Item.FindControl("plcUnitPriceDescription");

                lblUnitPrice.CssClass = string.Empty;

                Literal ltrLateFees = (Literal)e.Item.FindControl("ltrLateFees");
                Literal ltrAdditionalCharges = (Literal)e.Item.FindControl("ltrAdditionalCharges");
                Literal ltrSalesTax = (Literal)e.Item.FindControl("ltrSalesTax");

                Literal ltrSubTotal = (Literal)e.Item.FindControl("ltrSubTotal");

                Repeater rptItemAdditionalInfo = (Repeater)e.Item.FindControl("rptItemAdditionalInfo");


                plcLineItemAdjustmentControls.Visible = false;
                plcPrintInstallDismantleReport.Visible = false;
                plcExhibitorNotes.Visible = false;
                
                trShoppingCartItem.Attributes["class"] =
                    trItemAdditionalInfo.Attributes["class"] = (e.Item.ItemType == ListItemType.AlternatingItem) ? "altItem" : "item";

                plcRequiredAttribute1.Visible = plcRequiredAttribute2.Visible = false;

                if (e.Item.DataItem is ShoppingCartItem)
                {
                    ShoppingCartItem cartItem = e.Item.DataItem as ShoppingCartItem;

                    ltrQuantity.Text = cartItem.Quantity.Value.ToString();
                    lblProductCategory.Text = cartItem.Product.Category.CategoryName;
                    lblProductName.Text = cartItem.Product.ProductName;

                    if (!string.IsNullOrEmpty(cartItem.RequiredAttribute1))
                    {
                        plcRequiredAttribute1.Visible = true;
                        lblRequiredAttribute1.Text = cartItem.RequiredAttribute1;
                    }

                    if (!string.IsNullOrEmpty(cartItem.RequiredAttribute2))
                    {
                        plcRequiredAttribute2.Visible = true;
                        lblRequiredAttribute2.Text = cartItem.RequiredAttribute2;
                    }

                    if (!string.IsNullOrEmpty(cartItem.RequiredAttribute3))
                    {
                        plcRequiredAttribute3.Visible = true;
                        lblRequiredAttribute3.Text = cartItem.RequiredAttribute3;
                    }

                    if (!string.IsNullOrEmpty(cartItem.RequiredAttribute4))
                    {
                        plcRequiredAttribute4.Visible = true;
                        lblRequiredAttribute4.Text = cartItem.RequiredAttribute4;
                    }

                    plcUnitPriceDescription.Visible = true;
                    lblUnitPriceDescription.Text = cartItem.Product.DetermineCurrentUnitPriceDescription(DateTime.Now, true);
                    lblUnitPrice.Text = Server.HtmlEncode(Util.FormatUnitPrice(cartItem.Product.DetermineCurrentUnitPrice(DateTime.Now, true), cartItem.Product.UnitDescriptor, CurrencySymbolForShow));

                    ltrLateFees.Text = Server.HtmlEncode(Util.FormatCurrency(cartItem.LateFees, CurrencySymbolForShow));
                    ltrAdditionalCharges.Text = Server.HtmlEncode(Util.FormatCurrency(cartItem.AdditionalCharges, CurrencySymbolForShow));
                    ltrSalesTax.Text = Server.HtmlEncode(Util.FormatCurrency(cartItem.SalesTaxCharges, CurrencySymbolForShow));
                    ltrSubTotal.Text = Server.HtmlEncode(Util.FormatCurrency(cartItem.SubTotalCharges, CurrencySymbolForShow));

                    //Paint Additional Info
                    trItemAdditionalInfo.Visible = false;
                    if (cartItem.AdditionalInfoes.Count > 0)
                    {
                        trItemAdditionalInfo.Visible = true;
                        rptItemAdditionalInfo.Visible = true;
                        rptItemAdditionalInfo.DataSource = cartItem.AdditionalInfoes;
                        rptItemAdditionalInfo.DataBind();
                    }
                }
                else if(e.Item.DataItem is OrderItem)
                {   
                    OrderItem orderItem = e.Item.DataItem as OrderItem;

                    if (OrderAdjustable)
                    {
                        lblOrderItemId.Text = string.Format("Line Item Id:{0}", orderItem.OrderItemId);

                        lblOrderItemInsertedDateTime.Text = orderItem.InsertDateTime.ToString();

                        plcLineItemAdjustmentControls.Visible = true;
                        lnkBtnEditOrderLineItem.CommandArgument = 
                           lnkBtnDeleteOrderLineItem.CommandArgument = string.Format("{0}:{1}", orderItem.OrderId, orderItem.OrderItemId);

                        lnkBtnEditOrderLineItem.ToolTip = lnkBtnDeleteOrderLineItem.ToolTip = string.Format("Order Item Id {0}", orderItem.OrderItemId); 

                        lnkBtnDeleteOrderLineItem.Attributes.Add("onClick", "return confirm('Are you sure you want to delete this line item?');");

                        if (orderItem.Product != null)
                        {
                            if (!string.IsNullOrEmpty(orderItem.Product.InstallDismantleInd))
                            {
                                plcPrintInstallDismantleReport.Visible = true;

                                lnkPrintInstallDismantle.Visible = true;
                                lnkPrintInstallDismantle.Attributes.Add("onclick", string.Format("launchInstallDismantleReport({0}, {1}); return false;", (int)ReportEnum.InstallDismantleReport, orderItem.OrderItemId));
                            }
                        }
                    }

                    ltrQuantity.Text = orderItem.Quantity.Value.ToString();
                    lblProductCategory.Text = orderItem.CategoryName;
                    lblProductName.Text = orderItem.ItemDescription;

                    if (!string.IsNullOrEmpty(orderItem.ExhibitorNotes))
                    {
                        plcExhibitorNotes.Visible = true;
                        lblExhibitorNotes.Text = orderItem.ExhibitorNotes;
                    }

                    if (!string.IsNullOrEmpty(orderItem.RequiredAttribute1))
                    {
                        plcRequiredAttribute1.Visible = true;
                        lblRequiredAttribute1.Text = orderItem.RequiredAttribute1;
                    }

                    if (!string.IsNullOrEmpty(orderItem.RequiredAttribute2))
                    {
                        plcRequiredAttribute2.Visible = true;
                        lblRequiredAttribute2.Text = orderItem.RequiredAttribute2;
                    }

                    if (!string.IsNullOrEmpty(orderItem.RequiredAttribute3))
                    {
                        plcRequiredAttribute3.Visible = true;
                        lblRequiredAttribute3.Text = orderItem.RequiredAttribute3;
                    }

                    if (!string.IsNullOrEmpty(orderItem.RequiredAttribute4))
                    {
                        plcRequiredAttribute4.Visible = true;
                        lblRequiredAttribute4.Text = orderItem.RequiredAttribute4;
                    }

                    if (orderItem.EarlyBirdFlag.HasValue && orderItem.EarlyBirdFlag.Value == true)
                    {
                        lblUnitPrice.CssClass = "earlyBirdPricing";
                    }
                    lblUnitPrice.Text = Server.HtmlEncode(Util.FormatUnitPrice(orderItem.UnitPrice, orderItem.UnitDescriptor, CurrencySymbolForShow));

                    if (!string.IsNullOrEmpty(orderItem.UnitPriceDescription))
                    {
                        plcUnitPriceDescription.Visible = true;
                        lblUnitPriceDescription.Text = orderItem.UnitPriceDescription;
                    }

                    ltrLateFees.Text = Server.HtmlEncode(Util.FormatCurrency(orderItem.LateFeeAmount.Value, CurrencySymbolForShow));
                    ltrAdditionalCharges.Text = Server.HtmlEncode(Util.FormatCurrency(orderItem.AdditionalChargeAmount.Value, CurrencySymbolForShow));
                    ltrSalesTax.Text = Server.HtmlEncode(Util.FormatCurrency(orderItem.SalesTaxAmount.Value, CurrencySymbolForShow));

                    if (orderItem.SubTotalAmount.HasValue)
                    {
                        ltrSubTotal.Text = Server.HtmlEncode(Util.FormatCurrency(orderItem.SubTotalAmount.Value, CurrencySymbolForShow));
                    }

                    //Paint Additional Info
                    trItemAdditionalInfo.Visible = false;
                    if (orderItem.Responses.Count > 0)
                    {
                        trItemAdditionalInfo.Visible = true;
                        rptItemAdditionalInfo.Visible = true;
                        rptItemAdditionalInfo.DataSource = orderItem.Responses;
                        rptItemAdditionalInfo.DataBind();

                    }
                }
            }
        }

        protected void rptItemAdditionalInfo_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Literal ltrAdditionalInfoQuestion = (Literal)e.Item.FindControl("ltrAdditionalInfoQuestion");
                Literal ltrAdditionalInfoAnswer = (Literal)e.Item.FindControl("ltrAdditionalInfoAnswer");
                if (e.Item.DataItem is AdditionalInfo)
                {
                    AdditionalInfo additionalInfo = (AdditionalInfo)e.Item.DataItem;

                    if (additionalInfo != null)
                    {
                        ltrAdditionalInfoQuestion.Text = additionalInfo.Question;
                        ltrAdditionalInfoAnswer.Text = additionalInfo.Response;

                    }
                }
                else if (e.Item.DataItem is Response)
                {
                    Response response = (Response)e.Item.DataItem;
                    ltrAdditionalInfoQuestion.Text = response.Question;
                    ltrAdditionalInfoAnswer.Text = response.ResponseText;
                }
                
            }
        }

        protected void rptrFormOrder_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                OrderItem orderItem = (OrderItem)e.Item.DataItem;

                if (orderItem != null)
                {
                    Repeater rptrFormQuestions = (Repeater) e.Item.FindControl("rptrFormQuestions");

                    if (orderItem.Responses != null && orderItem.Responses.Count > 0)
                    {
                        rptrFormQuestions.DataSource = orderItem.Responses;
                        rptrFormQuestions.DataBind();
                        rptrFormQuestions.Visible = true;
                    }
                }
            }
        }

        protected void lnkBtnEditOrderLineItem_Click(object sender, EventArgs e)
        {
            LinkButton lnkBtn = (LinkButton)sender;

            int orderId = 0;
            int orderItemId = 0;

            if (lnkBtn.CommandArgument.Contains(":"))
            {
                orderId = Util.ConvertInt32(lnkBtn.CommandArgument.Split(':')[0]);
                orderItemId = Util.ConvertInt32(lnkBtn.CommandArgument.Split(':')[1]);
            }
            
            switch (lnkBtn.CommandName)
            {
                case "EditOrderItem":
                    _orderAdjust(orderId, orderItemId, LineItemAdjustmentEnum.Edit);
                    break;
                case "DeleteOrderItem":
                    _orderAdjust(orderId, orderItemId, LineItemAdjustmentEnum.Delete);
                    break;
                case "AddOrderItem":
                    _orderAdjust(orderId, orderItemId, LineItemAdjustmentEnum.Add);
                    break;
                default:
                    break;
            }
        }
    }
}