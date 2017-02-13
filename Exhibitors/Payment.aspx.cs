using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


using ExpoOrders.Common;
using ExpoOrders.Entities;
using ExpoOrders.Controllers;
using System.Collections.Specialized;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace ExpoOrders.Web.Exhibitors
{

    public partial class Payment : BaseExhibitorPage
    {
        ShoppingCartController _ctrl;
        ShoppingCartController Cntrl
        {
            get
            {
                if (_ctrl == null)
                {
                    _ctrl = new ShoppingCartController();
                }
                return _ctrl;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.OnNavigationItemCallBack = HandleNavigationItemClicked;
            Master.BodyOnLoadScript("loadPaymentOptions();");

            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, ExhibitorPageEnum.Payment);
            Master.NoSubNavigation = true;

            plcPaymentSelection.Visible = true;
            chkSaveNewCreditCard.Text = string.Format("I authorize this card to be used for any additional services provided by {0}.", CurrentUser.CurrentShow.Owner.OwnerName);
            plcOrderReview.Visible = false;
            lnkCorrectPaymentProblem.Visible = false;

            List<string> allowablePaymentTypes = Util.ParseDelimitedList(CurrentUser.CurrentShow.AllowablePaymentTypeList, ';');
            bool creditCardsAllowed = allowablePaymentTypes.Contains(PaymentTypeEnum.CreditCard.ToString());

            if (creditCardsAllowed)
            {
                LoadCreditCardsOnFile();
                LoadNewCreditCardOptions();
            }
            else
            {
                rptrExistingCreditCards.Visible = false;
            }
            

            btnPayByCard.Visible = creditCardsAllowed;
            btnPayByCheck.Visible = allowablePaymentTypes.Contains(PaymentTypeEnum.Check.ToString());
            btnPayByWire.Visible = allowablePaymentTypes.Contains(PaymentTypeEnum.Wire.ToString());
            btnNoPayment.Visible = allowablePaymentTypes.Contains(PaymentTypeEnum.None.ToString());

            //Special Exhibitors get the priviledge of not paying by credit card or check
            if (!btnNoPayment.Visible)
            {
                Exhibitor currentExhibitorDetails = Cntrl.GetExhibitor(CurrentUser.CurrentExhibitor.ExhibitorId);
                if (currentExhibitorDetails.AllowSpecialCheckout.HasValue && currentExhibitorDetails.AllowSpecialCheckout.Value == true)
                {
                    btnNoPayment.Visible = true;
                }
            }

            WebUtil.DefaultTextBoxValue(txtCreditCardCountry, "USA");
        }

        private void LoadNewCreditCardOptions()
        {
            txtCreditCardEmail.Text = DefaultEmail(CurrentUser);

            LoadCreditCardTypes();
            LoadCreditCardExpirationOptions();

            chkSaveNewCreditCard.Checked = true;
        }

        private void LoadCreditCardsOnFile()
        {
            List<CreditCard> cardsOnFile = Cntrl.GetAvailableCreditCards(CurrentUser);

            if (cardsOnFile != null && cardsOnFile.Count > 0)
            {
               // cardsOnFile.Add(new CreditCard() { CreditCardId = 0, NameOnCard = "Add New Credit Card", EmailAddress = string.Empty });

                rptrExistingCreditCards.DataSource = cardsOnFile;
                rptrExistingCreditCards.DataBind();
                rptrExistingCreditCards.Visible = true;
                hdnNoCardsOnFile.Value = false.ToString();

                newCreditCardDetails.Attributes["style"] = "display: none;";
            }
            else
            {
                newCreditCardDetails.Attributes["style"] = "display: block;";
                rptrExistingCreditCards.Visible = false;

                hdnNoCardsOnFile.Value = true.ToString();
            }
        }

        private void LoadCreditCardTypes()
        {
            if (!IsPostBack)
            {
                ddlCreditCardType.Items.Clear();
                ddlCreditCardType.DataSource = Cntrl.GetCreditCardTypesList(CurrentUser.CurrentShow.MerchantAccountConfigId);
                ddlCreditCardType.DataTextField = "Name";
                ddlCreditCardType.DataValueField = "CreditCardTypeCd";
                ddlCreditCardType.DataBind();
                ddlCreditCardType.Items.Insert(0, new ListItem { Text = "-- Select One --", Value = "-1" });
            }
            
        }

        private void LoadCreditCardExpirationOptions()
        {
            if (!IsPostBack)
            {
                ddlCreditCardExpMonth.Items.Clear();
                for (int iMonth = 1; iMonth <= 12; iMonth++)
                {
                    ddlCreditCardExpMonth.Items.Add(new ListItem(iMonth.ToString().PadLeft(2, '0'), iMonth.ToString()));
                }

                ddlCreditCardExpYear.Items.Clear();
                for (int iYear = 0; iYear <= 10; iYear++)
                {
                    int yearItem = DateTime.Now.Year + iYear;
                    ddlCreditCardExpYear.Items.Add(new ListItem(yearItem.ToString(), yearItem.ToString()));
                }
            }
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            Master.HideDynamicContent();
        }

        private CreditCard BuildCreditCard()
        {
            CreditCard newCreditCard = new CreditCard();
            newCreditCard.ExhibitorId = CurrentUser.CurrentExhibitor.ExhibitorId;
            newCreditCard.CreditCardTypeCd = ddlCreditCardType.SelectedValue;

            txtCreditCardNumber.Text = txtCreditCardNumber.Text.Trim().Replace(" ", string.Empty);
            newCreditCard.SetCreditCardNumber(txtCreditCardNumber.Text.Trim());
            newCreditCard.NameOnCard = txtCreditCardName.Text.Trim();
            newCreditCard.ExpirationMonth = ddlCreditCardExpMonth.SelectedValue;
            newCreditCard.ExpirationYear = ddlCreditCardExpYear.SelectedValue;
            newCreditCard.SetCreditCardSecurityCode(txtCreditCardSecurityCode.Text.Trim());
            newCreditCard.EmailAddress = txtCreditCardEmail.Text.Trim();

            newCreditCard.BillingAddressId = 0;

            newCreditCard.ActiveFlag = (chkSaveNewCreditCard.Checked);
            
            return newCreditCard;
        }

        private Address BuildBillingAddress()
        {
            Address billingAddress = new Address();

            billingAddress.AddressId = 0;
            billingAddress.Street1 = txtCreditCardAddressLine1.Text.Trim();
            billingAddress.Street2 = txtCreditCardAddressLine2.Text.Trim();
            billingAddress.City = txtCreditCardCity.Text.Trim();
            billingAddress.StateProvinceRegion = txtCreditCardState.Text.Trim();
            billingAddress.PostalCode = txtCreditCardPostalCode.Text.Trim();
            billingAddress.Country = txtCreditCardCountry.Text.Trim();

            return billingAddress;
        }

        private void LoadOrderReview(PaymentTypeEnum paymentType, CreditCard creditCard, Address billingAddress, bool saveCreditCard)
        {
            plcPaymentSelection.Visible = false;
            plcOrderReview.Visible = true;

            string termsAndConditionsGroupName = "Terms and Conditions";
            if (!string.IsNullOrEmpty(CurrentUser.CurrentShow.TermsAndConditionsGroupName))
            {
                termsAndConditionsGroupName = CurrentUser.CurrentShow.TermsAndConditionsGroupName;
            }
            ltrTermsAndConditionsGroupName.Text = termsAndConditionsGroupName;


            if (CurrentUser.CurrentShow.IsTermsAndConditionsRequired)
            {
                chkTermsConditions.Visible = true;
                string termsAndConditionsLabel = "I Accept the Terms and Conditions";
                if (!string.IsNullOrEmpty(CurrentUser.CurrentShow.TermsAndConditionsLabel))
                {
                    termsAndConditionsLabel = CurrentUser.CurrentShow.TermsAndConditionsLabel;
                }
                chkTermsConditions.Text = termsAndConditionsLabel;
            }
            else
            {
                chkTermsConditions.Visible = false;
            }
            
            ltrTermsAndConditions.Text = this.CurrentUser.CurrentShow.TermsAndConditions;

            ltrOrderPaymentType.Text = paymentType.GetDescription();
            hdnPaymentType.Value = paymentType.ToString();

            if (paymentType == PaymentTypeEnum.CreditCard)
            {
                plcOrderCreditCard.Visible = true;

                if (creditCard != null)
                {
                    if (creditCard.CreditCardType == null)
                    {
                        creditCard.CreditCardType = Cntrl.GetCreditCardTypeByCode(creditCard.CreditCardTypeCd);
                    }

                    hdnSaveCreditCard.Value = saveCreditCard.ToString().ToLower();
                    hdnSelectedCreditCardId.Value = SelectedCreditCardId.ToString();
                    txtOrderEmailAddress.Text = HtmlEncode(creditCard.EmailAddress);
                    ltrOrderNameOnCreditCard.Text = HtmlEncode(creditCard.NameOnCard);
                    ltrOrderCreditCardType.Text = HtmlEncode(creditCard.CreditCardType.Name);
                    hdnCreditCardType.Value = creditCard.CreditCardTypeCd;
                    ltrOrderCreditCardNumber.Text = HtmlEncode(creditCard.CardNumberMasked);
                    hdnOrderCreditCardNumber.Value = HtmlEncode(creditCard.CardNumber);
                    ltrOrderCreditCardExpMonth.Text = HtmlEncode(creditCard.ExpirationMonth);
                    ltrOrderCreditExpYear.Text = HtmlEncode(creditCard.ExpirationYear);
                    ltrOrderCreditCardSecurityCode.Text = HtmlEncode(creditCard.SecurityCodeMasked);
                    hdnOrderCreditCardSecurityCode.Value = HtmlEncode(creditCard.SecurityCode);
                }

                if (billingAddress != null)
                {
                    ltrOrderStreet1.Text = HtmlEncode(billingAddress.Street1);
                    ltrOrderStreet2.Text = HtmlEncode(billingAddress.Street2);
                    ltrOrderCity.Text = HtmlEncode(billingAddress.City);
                    ltrOrderStateProvinceRegion.Text = HtmlEncode(billingAddress.StateProvinceRegion);
                    ltrOrderPostalCode.Text = HtmlEncode(billingAddress.PostalCode);
                    ltrOrderCountry.Text = HtmlEncode(billingAddress.Country);
                }
            }
            else if(paymentType == PaymentTypeEnum.Check)
            {
                plcOrderCreditCard.Visible = false;
                txtOrderEmailAddress.Text = DefaultEmail(CurrentUser);
            }
            else if (paymentType == PaymentTypeEnum.Wire)
            {
                plcOrderCreditCard.Visible = false;
                txtOrderEmailAddress.Text = DefaultEmail(CurrentUser);
            }
            else if (paymentType == PaymentTypeEnum.None)
            {
                plcOrderCreditCard.Visible = false;
                txtOrderEmailAddress.Text = DefaultEmail(CurrentUser);
            }

            Entities.ShoppingCart shoppingCart = Cntrl.GetUserCart(CurrentUser);

            if (shoppingCart != null && shoppingCart.ShoppingCartItems != null && shoppingCart.ShoppingCartItems.Count > 0)
            {
                shoppingCart.CalculateItemSubTotals(this.CurrentUser.CurrentExhibitor, DateTime.Now);
                ucOrderDetails.Populate(Cntrl, CurrentUser.CurrentExhibitor, shoppingCart, false, CurrentUser.CurrentShow.CurrencySymbol, CurrentUser.CurrentShow);
            }
        }

        private string DefaultEmail(ExpoOrdersUser currentUser)
        {
            string email = currentUser.EmailAddress;

            if (string.IsNullOrEmpty(email))
            {
                email = currentUser.CurrentExhibitor.PrimaryEmailAddress;
            }
            return email;
        }

        private int SelectedCreditCardId
        {
            get
            {
                int creditCardId = -1;
                if (Util.ConvertBool(hdnNoCardsOnFile.Value))
                {
                    creditCardId = 0;
                }
                else
                {
                    if (Request.Form["SelectedCreditCard"] == null)
                    {
                        creditCardId = - 1;
                    }
                    else
                    {
                        creditCardId = Util.ConvertInt32(Request.Form["SelectedCreditCard"]);
                    }
                }

                return creditCardId;
            }
        }
        

        public string IsChecked(int creditCardId)
        {
            //Radio button is a dynamic control, becasue the Repeater sux
            // Therefore, the radiot butotn retained might come from the page postng back wiht failures
            // OR when clickging 'Back to Payment Options' after a failure.
            int creditCardIdToSelect = SelectedCreditCardId;
            if (SelectedCreditCardId == -1 && this.hdnSelectedCreditCardId.Value.Length > 0)
            {
                creditCardIdToSelect = Util.ConvertInt32(hdnSelectedCreditCardId.Value);
            }
            
            if (creditCardId == creditCardIdToSelect)
            {
                return "checked";
            }
                
            return string.Empty;
        }

        protected void btnPayByCard_Click(object sender, EventArgs e)
        {
            CreditCard creditCard = null;
            Address billingAddress = null;

            ValidationResults creditCardErrors = new ValidationResults();

            int creditCardSelected = SelectedCreditCardId;

            switch (creditCardSelected)
            {
                case -1:
                    PageErrors.ValidationGroup = "SelectCreditCard";
                    creditCardErrors.AddResult(new ValidationResult("Please select or input Credit Card information for this order.", null, null, null, null));
                    break;
                case 0:
                    PageErrors.ValidationGroup = "NewCreditCard";
                    creditCard = BuildCreditCard();
                    billingAddress = BuildBillingAddress();

                    creditCardErrors = Cntrl.ValidateCreditCard(creditCard, billingAddress);

                    break;
                default:
                    if (creditCardSelected > 0)
                    {
                        AccountController acctCtrl = new AccountController();

                        creditCard = acctCtrl.GetCreditCardById(creditCardSelected);
                        billingAddress = creditCard.Address;
                    }
                    break;
            }

            if (creditCardErrors.IsValid)
            {
                LoadOrderReview(PaymentTypeEnum.CreditCard, creditCard, billingAddress, (chkSaveNewCreditCard.Checked));
            }
            else
            {
                PageErrors.AddErrorMessages(creditCardErrors, PageErrors.ValidationGroup);
                LoadCreditCardsOnFile();
            }
        }

        protected void btnPayByCheck_Click(object sender, EventArgs e)
        {
            LoadOrderReview(PaymentTypeEnum.Check, null, null, false);
        }

        protected void btnPayByWire_Click(object sender, EventArgs e)
        {
            LoadOrderReview(PaymentTypeEnum.Wire, null, null, false);
        }

        protected void btnNoPayment_Click(object sender, EventArgs e)
        {
            LoadOrderReview(PaymentTypeEnum.None, null, null, false);
        }

        protected void btnBackToCart_Click(object sender, EventArgs e)
        {
            Server.Transfer("ShoppingCart.aspx", false);
        }

        protected void btnSubmitOrder_Click(object sender, EventArgs e)
        {
            lnkCorrectPaymentProblem.Visible = false;

            ValidationResults orderErrors = new ValidationResults();

            if (CurrentUser.CurrentShow.IsTermsAndConditionsRequired && chkTermsConditions.Visible)
            {
                if (!chkTermsConditions.Checked)
                {
                    string termsAndConditionsGroupName = "Terms and Conditions";
                    if (!string.IsNullOrEmpty(CurrentUser.CurrentShow.TermsAndConditionsGroupName))
                    {
                        termsAndConditionsGroupName = CurrentUser.CurrentShow.TermsAndConditionsGroupName;
                    }
                    orderErrors.AddResult(new ValidationResult(string.Format("Please check the {0} checkbox.", termsAndConditionsGroupName), null, null, null, null));
                }
            }

            if (txtOrderEmailAddress.Text.Trim().Length <= 0)
            {
                orderErrors.AddResult(new ValidationResult("Please provide an Email Address for this order.", null, null, null, null));
            }

            bool isNewCreditCard = false;
            //Must also validation Credit Authorization and/or Check details
            if (orderErrors.IsValid)
            {
                Order newOrder = BuildNewOrder();

                bool saveNewCreditCard = false;
                if (newOrder.PaymentType == PaymentTypeEnum.CreditCard)
                {
                    isNewCreditCard = Util.ConvertInt32(hdnSelectedCreditCardId.Value) == 0;

                    if (isNewCreditCard
                    && Util.ConvertBool(hdnSaveCreditCard.Value))
                    {
                        saveNewCreditCard = true;
                    }
                }

                OrderConfirmation orderConfirmation = Cntrl.CreateOrder(CurrentUser, newOrder, saveNewCreditCard);

                if (!orderConfirmation.Errors.IsValid)
                {
                    orderErrors = orderConfirmation.Errors;
                }
                else
                {
                    ConfirmationOrder = orderConfirmation;

                    CurrentUser.ShoppingCartItemCount = 0;
                    this.Master.PopulateShoppingCart(CurrentUser);
                }
            }

            if (!orderErrors.IsValid)
            {
                PageErrors.ValidationGroup = "OrderSubmit";
                PageErrors.AddErrorMessages(orderErrors, PageErrors.ValidationGroup);


                if (Util.ErrorsContainKey(orderErrors, "credit_card_auth_failure"))
                {
                    lnkCorrectPaymentProblem.Visible = true;
                    lnkCorrectPaymentProblem.Text = "Edit Credit Card Information";

                    if (isNewCreditCard)
                    {
                        lnkCorrectPaymentProblem.CommandName = "NewCard";
                        lnkCorrectPaymentProblem.CommandArgument = string.Empty;
                    }
                    else
                    {
                        lnkCorrectPaymentProblem.CommandName = "ExistingCard";
                        lnkCorrectPaymentProblem.CommandArgument = hdnSelectedCreditCardId.Value;
                    }
                }
            }
            else
            {
                Server.Transfer("Orders.aspx", false);
            }
        }

        protected void lnkCorrectPaymentProblem_Click(object sender, EventArgs e)
        {
            if (sender is LinkButton)
            {
                LinkButton lnkCorrectPaymentProblem = (LinkButton)sender;

                switch (lnkCorrectPaymentProblem.CommandName)
                {
                    case "NewCard":
                        LoadPage();
                        break;
                    case "ExistingCard":
                        HttpContext.Current.Items.Add("edit_credit_card_id", lnkCorrectPaymentProblem.CommandArgument); //temporarily setting to session
                        Server.Transfer("Account.aspx", false);
                        break;
                    default:
                        break;
                }
                
            }
        }

        protected void btnChoosePaymentMethod_Click(object sender, EventArgs e)
        {
            LoadPage();
        }

        private Order BuildNewOrder()
        {
            Order order = new Order(OrderTypeEnum.BoothOrder);
            order.ActiveFlag = true;
            order.PaymentType = (PaymentTypeEnum) Enum.Parse(typeof(PaymentTypeEnum), hdnPaymentType.Value, true);

            if (order.PaymentType == PaymentTypeEnum.CreditCard)
            {
                order.CreditCardTypeCd = hdnCreditCardType.Value;
                order.CreditCardNameOnCard = ltrOrderNameOnCreditCard.Text.Trim();
                order.CreditCardNumber = hdnOrderCreditCardNumber.Value;
                order.CreditCardExpirationMonth = ltrOrderCreditCardExpMonth.Text;
                order.CreditCardExpirationYear = ltrOrderCreditExpYear.Text;

                if (!string.IsNullOrEmpty(order.CreditCardExpirationMonth))
                {
                    order.CreditCardExpirationDate = string.Format("{0}/{1}", order.CreditCardExpirationMonth, order.CreditCardExpirationYear);
                }

                order.CreditCardSecurityCode = hdnOrderCreditCardSecurityCode.Value;

                order.BillingAddressStreet1 = ltrOrderStreet1.Text;
                order.BillingAddressStreet2 = ltrOrderStreet2.Text;
                order.BillingAddressCity = ltrOrderCity.Text;
                order.BillingAddressStateProvinceRegion = ltrOrderStateProvinceRegion.Text;
                order.BillingAddressPostalCode = ltrOrderPostalCode.Text;
                order.BillingAddressCountry = ltrOrderCountry.Text;

            }
            else if (order.PaymentType == PaymentTypeEnum.Check)
            {
                //Billing address will come from the Exhibitor company address, nothing else todo here

            }
            else if (order.PaymentType == PaymentTypeEnum.Wire)
            {
                //Billing address will come from the Exhibitor company address, nothing else todo here

            }

            order.OrderEmailAddress = txtOrderEmailAddress.Text.Trim();
            order.OrderTermsConditions = ltrTermsAndConditions.Text.Trim();

            return order;
        }

        protected void rptrExistingCreditCards_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CreditCard card = (CreditCard)e.Item.DataItem;

                HtmlTableRow trCreditCard = (HtmlTableRow)e.Item.FindControl("trCreditCard");

                trCreditCard.Attributes["class"] = (e.Item.ItemType == ListItemType.AlternatingItem) ? "altItem" : "item";

                Literal ltrCreditCardType = (Literal)e.Item.FindControl("ltrCreditCardType");

                if (card.CreditCardId > 0)
                {
                    if (card.CreditCardType != null)
                    {
                        ltrCreditCardType.Text = card.CreditCardType.Name;
                    }
                }
            }
        }
    }
}