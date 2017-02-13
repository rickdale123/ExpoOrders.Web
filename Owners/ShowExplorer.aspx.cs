
#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ExpoOrders.Common;
using ExpoOrders.Entities;
using ExpoOrders.Controllers;
using ExpoOrders.Web.Owners.Common;
using System.Web.UI.HtmlControls;
using System.Text;
#endregion

namespace ExpoOrders.Web.Owners
{
    [Serializable]
    public enum ShowSettingPageMode
    {
        ShowSettings = 1,
        Tab1Editor = 3,
        Tab2Editor = 4,
        Tab3Editor = 5,
        Tab4Editor = 6,
        Tab5Editor = 7,
        Assets = 8,
        ShowOrderConfiguration = 9,
        Products = 10,
        Orders = 11,
        Payments = 12
    }

    public partial class ShowExplorer : BaseOwnerPage
    {
        #region Private Members
        private ShowController _showCtrl;
        private OwnerAdminController _ownerCtrlr;
        private AccountController _accountCtrl;
        #endregion

        #region Public Members
        public ShowController ShowCtrl
        {
            get
            {
                if (_showCtrl == null)
                {
                    _showCtrl = new ShowController();
                }
                return _showCtrl;
            }
        }
        public OwnerAdminController OwnerCtrlr
        {
            get
            {
                if (_ownerCtrlr == null)
                {
                    _ownerCtrlr = new OwnerAdminController();
                }
                return _ownerCtrlr;
            }
        }

        public AccountController AccountCtrl
        {
            get
            {
                if (_accountCtrl == null)
                {
                    _accountCtrl = new AccountController();
                }
                return _accountCtrl;
            }
        }
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.NavigationItemCallBack = this.HandleNavigationItemClicked;
            this.Master.PreviewShowCallBack = this.PreviewShow;

            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, OwnerPage.ShowSettings, OwnerTabEnum.ShowDetail);
            this.Master.LoadSubNavigation("Show Admin", OwnerUtil.BuildShowAdminNavLinks(CurrentUser, CurrentUser.CurrentShow.ShowId));

            if (Request["targetId"] != null && Util.ConvertInt32(Request["targetId"]) > 0)
            {
                int targetId = Util.ConvertInt32(Request["targetId"]);
                switch (targetId)
                {
                    case (int)ShowSettingPageMode.ShowSettings:
                        LoadPageMode(ShowSettingPageMode.ShowSettings);
                        break;
                    case (int)ShowSettingPageMode.Assets:
                        LoadPageMode(ShowSettingPageMode.Assets);
                        break;
                    case (int)ShowSettingPageMode.ShowOrderConfiguration:
                        LoadPageMode(ShowSettingPageMode.ShowOrderConfiguration);
                        break;
                    default:
                        LoadPageMode(ShowSettingPageMode.ShowSettings);
                        break;
                }
            }
            else
            {
                LoadPageMode(ShowSettingPageMode.ShowSettings);
            }
        }

        private void LoadPageMode(ShowSettingPageMode mode)
        {
            if (this.Master.CurrentSubNavigationLinks != null && this.Master.CurrentSubNavigationLinks.Count > 0)
            {
                NavigationLink linkToSelect = this.Master.CurrentSubNavigationLinks.SingleOrDefault<NavigationLink>(l => l.TargetId == (int)mode);
                this.Master.SelectNavigationItem(linkToSelect.NavigationLinkId);
                LoadPageMode(linkToSelect.NavigationLinkId, (int)linkToSelect.TargetId);
            }
            else
            {
                plcPageDetail.Visible = false;
            }

        }

        private void LoadPageMode(int navLinkId, int targetId)
        {
            plcPageDetail.Visible = true;
            ShowSettingPageMode currentPageMode = (ShowSettingPageMode)Enum.Parse(typeof(ShowSettingPageMode), targetId.ToString(), true);

            this.plcShowInformation.Visible =
            this.plcAssets.Visible =
            this.plcShowOrderConfig.Visible = this.plcBoothTypesSupported.Visible = false;

            switch (currentPageMode)
            {
                case ShowSettingPageMode.ShowSettings:
                    OwnerUtil.ClearPlaceHolderControl(plcShowInformation);
                    InitializeShowInformation(CurrentUser.CurrentShow.ShowId);
                    break;
                case ShowSettingPageMode.Assets:
                    LoadFileList();
                    break;
                case ShowSettingPageMode.Tab1Editor:
                    Server.Transfer(OwnerPage.Tab1Editor.GetCodeValue());
                    break;
                case ShowSettingPageMode.Tab2Editor:
                    Server.Transfer(OwnerPage.Tab2Editor.GetCodeValue());
                    break;
                case ShowSettingPageMode.Tab3Editor:
                    Server.Transfer(OwnerPage.Tab3Editor.GetCodeValue());
                    break;
                case ShowSettingPageMode.Tab4Editor:
                    Server.Transfer(OwnerPage.Tab4Editor.GetCodeValue());
                    break;
                case ShowSettingPageMode.Tab5Editor:
                    Server.Transfer(OwnerPage.Tab5Editor.GetCodeValue());
                    break;
                case ShowSettingPageMode.ShowOrderConfiguration:
                    OwnerUtil.ClearPlaceHolderControl(plcShowOrderConfig);
                    InitializeShowOrderConfigInformation();
                    break;
                case ShowSettingPageMode.Products:
                    Server.Transfer(OwnerPage.Products.GetCodeValue());
                    break;
            }
        }

        #endregion

        #region Initialize OwnerAdmin Information

        private void InitializeShowInformation(int showId)
        {
            this.plcShowInformation.Visible = true;
            this.plcBoothTypesSupported.Visible = false;

            if (!string.IsNullOrEmpty(CurrentUser.CurrentOwner.BoothTypeList))
            {
                plcBoothTypesSupported.Visible = true;
                foreach (string boothType in Util.ParseDelimitedList(CurrentUser.CurrentOwner.BoothTypeList, ';'))
                {
                    this.chkBoothTypesSupported.Items.Add(new ListItem(boothType));
                }
            }

            if (showId > 0)
            {
                ShowController cntrl = new ShowController();
                Show currentShow = cntrl.GetShowById(showId);

                if (currentShow != null)
                {
                    lblShowId.Text = currentShow.ShowId.ToString();
                    ltrShowGuid.Text = currentShow.ShowGuid.ToString();
                    chkActiveFlag.Checked = currentShow.ActiveFlag;

                    lblActiveDescription.Text = currentShow.ActiveFlag ? "(Live)" : "(Inactivated)";


                    if (currentShow.DisplayOnOwnerLandingPage.HasValue)
                    {
                        chkDisplayOnOwnerLanding.Checked = currentShow.DisplayOnOwnerLandingPage.Value;
                    }
                    else
                    {
                        chkDisplayOnOwnerLanding.Checked = true;
                    }

                    lblDisplayOnOwnerLanding.Text = chkDisplayOnOwnerLanding.Checked ? "(Yes, visible to the public)" : "(No)";

                    rptrExhibitorLoginUrls.DataSource = currentShow.Owner.OwnerSubDomains.Split(';');
                    rptrExhibitorLoginUrls.DataBind();

                    if (!string.IsNullOrEmpty(currentShow.CompanyNameOnInvoice))
                    {
                        txtCompanyNameOnInvoice.Text = currentShow.CompanyNameOnInvoice;
                    }
                    else
                    {
                        txtCompanyNameOnInvoice.Text = currentShow.Owner.OwnerName;
                    }
                    

                    txtShowName.Text = currentShow.ShowName.Trim();
                    txtStartDate.Text = currentShow.StartDate.HasValue ?
                        currentShow.StartDate.Value.ToShortDateString() : string.Empty;
                    txtEndDate.Text = currentShow.EndDate.HasValue ?
                        currentShow.EndDate.Value.ToShortDateString() : string.Empty;

                    txtAdvanceWarehouseLabelText.Text = currentShow.AdvanceWarehouseLabelText;
                    txtDirectShowSiteLabelText.Text = currentShow.DirectShowSiteLabelText;
                    txtOutboundLabelText.Text = currentShow.OutboundLabelText;

                    PaintShippingLabelLogoControls(currentShow.OutboundShippingLabelImage);

                    PaintQuickViewFileNameControls(currentShow.QuickFactsFileName);

                    if (!string.IsNullOrEmpty(currentShow.BoothTypesSupported))
                    {
                        if (!string.IsNullOrEmpty(CurrentUser.CurrentOwner.BoothTypeList))
                        {
                            foreach (string boothType in Util.ParseDelimitedList(currentShow.BoothTypesSupported, ';'))
                            {
                                var chk = this.chkBoothTypesSupported.Items.FindByText(boothType);
                                if (chk != null)
                                {
                                    chk.Selected = true;
                                }
                            }
                        }
                    }

                    if (currentShow.VenueAddressId > 0)
                    {
                        txtVenueName.Text = currentShow.VenueName;
                        txtVenueAddressLine1.Text = currentShow.VenueAddress.Street1;
                        txtVenueAddressLine2.Text = currentShow.VenueAddress.Street2;
                        txtVenueAddressLine3.Text = currentShow.VenueAddress.Street3;
                        txtVenueCity.Text = currentShow.VenueAddress.City;
                        txtVenueState.Text = currentShow.VenueAddress.StateProvinceRegion;
                        txtVenuePostalCode.Text = currentShow.VenueAddress.PostalCode;
                        txtVenueCountry.Text = currentShow.VenueAddress.Country;
                    }

                    if (currentShow.AdvanceWarehouseAddressId > 0)
                    {
                        txtWarehouseAddressLine1.Text = currentShow.AdvanceWarehouseAddress.Street1;
                        txtWarehouseAddressLine2.Text = currentShow.AdvanceWarehouseAddress.Street2;
                        txtWarehouseAddressLine3.Text = currentShow.AdvanceWarehouseAddress.Street3;
                        txtWarehouseCity.Text = currentShow.AdvanceWarehouseAddress.City;
                        txtWarehouseState.Text = currentShow.AdvanceWarehouseAddress.StateProvinceRegion;
                        txtWarehousePostalCode.Text = currentShow.AdvanceWarehouseAddress.PostalCode;
                        txtWarehouseCountry.Text = currentShow.AdvanceWarehouseAddress.Country;
                    }

                    if (currentShow.RemitPaymentAddressId > 0)
                    {
                        txtRemitAddressLine1.Text = currentShow.RemitAddress.Street1;
                        txtRemitAddressLine2.Text = currentShow.RemitAddress.Street2;
                        txtRemitAddressLine3.Text = currentShow.RemitAddress.Street3;
                        txtRemitCity.Text = currentShow.RemitAddress.City;
                        txtRemitState.Text = currentShow.RemitAddress.StateProvinceRegion;
                        txtRemitPostalCode.Text = currentShow.RemitAddress.PostalCode;
                        txtRemitCountry.Text = currentShow.RemitAddress.Country;
                        txtRemitPhone.Text = currentShow.RemitPhone;
                        txtAdditionalRemitLine.Text = currentShow.AdditionalRemitLine;
                    }
                }
            }
        }

        private void InitializeShowOrderConfigInformation()
        {
            plcShowOrderConfig.Visible = true;

            BuildNotificationList();

            Show currentShow = this.OwnerCtrlr.GetShowById(CurrentUser.CurrentShow.ShowId);

            if (currentShow != null)
            {
                PopulateSelectedNofications(currentShow);

                txtTermsAndConditions.Text = currentShow.TermsAndConditions;
                txtTermsAndConditionsGroupName.Text = currentShow.TermsAndConditionsGroupName;
                txtTermsAndConditionsLabel.Text = currentShow.TermsAndConditionsLabel;
                rdoTermsAndConditionsRequired.SelectedValue = (currentShow.IsTermsAndConditionsRequired) ? "1" : "0";
                rdoDisplayBoothNumberLabel.SelectedValue = (currentShow.DisplayBoothNumberLabel.HasValue && currentShow.DisplayBoothNumberLabel.Value == true) ? "1" : "0";

                txtOrderNotificationEmail.Text = currentShow.OrderNotificationEmail;
                txtOrderFromEmail.Text = currentShow.OrderFromEmail;
                txtOrderReplyEmail.Text = currentShow.OrderReplyEmail;
                txtCurrencySymbol.Text = string.IsNullOrEmpty(currentShow.CurrencySymbol) ? "$" : currentShow.CurrencySymbol;
                txtOrderConfirmationMessage.Text = currentShow.OrderConfirmationMessage;
                txtInvoiceMessage.Text = currentShow.InvoiceMessage;

                txtShoppingCartFullImage.Text = currentShow.ShoppingCartFullImage;
                txtShoppingCartEmptyImage.Text = currentShow.ShoppingCartEmptyImage;

                WebUtil.SelectListItemByValue(ddlMerchantAccountConfigId, currentShow.MerchantAccountConfigId);

                txtFooterInfo.Text = currentShow.FooterInfo;

                if (!string.IsNullOrEmpty(currentShow.LoginContactInfo))
                {
                    txtLoginContactInfo.Text = currentShow.LoginContactInfo;    
                }
                else
                {
                    txtLoginContactInfo.Text = currentShow.Owner.ContactInfoHtml;
                }

                txtLoginInfoText.Text = currentShow.LoginInfoText;

                LoadAllowablePaymentTypes(currentShow.AllowablePaymentTypeList);

                LoadSubmissionTransactions(currentShow.SubmissionTransactionTypeCd);
                LoadMerchantAccountConfigs(currentShow.MerchantAccountConfigId);

                PaintRemitLogoControls(currentShow.RemitLogoFileName);
            }
            
        }

        private void BuildNotificationList()
        {
            chklstOwnerNotifications.Items.Clear();

            chklstOwnerNotifications.Items.Add(new ListItem("Order Confirmation", OwnerNotificationEnum.OrderConfirmation.GetCodeValue()));
            chklstOwnerNotifications.Items.Add(new ListItem("Form Submissions", OwnerNotificationEnum.FormSubmission.GetCodeValue()));
            chklstOwnerNotifications.Items.Add(new ListItem("C.C. Authorizations", OwnerNotificationEnum.CreditCardAuthorization.GetCodeValue()));
            chklstOwnerNotifications.Items.Add(new ListItem("C.C. Settlements", OwnerNotificationEnum.CreditCardSettlement.GetCodeValue()));
            chklstOwnerNotifications.Items.Add(new ListItem("C.C. Sales", OwnerNotificationEnum.CreditCardSale.GetCodeValue()));
            chklstOwnerNotifications.Items.Add(new ListItem("C.C. Refund", OwnerNotificationEnum.CreditCardRefund.GetCodeValue()));
            chklstOwnerNotifications.Items.Add(new ListItem("C.C. Void", OwnerNotificationEnum.CreditCardVoid.GetCodeValue()));
            chklstOwnerNotifications.Items.Add(new ListItem("Check Payment", OwnerNotificationEnum.CheckPayment.GetCodeValue()));
        }

        private void PopulateSelectedNofications(Show show)
        {
            if (show.OwnerNotifications.Count > 0)
            {
                show.OwnerNotifications.ForEach(n =>
                {
                    ListItem li = chklstOwnerNotifications.Items.FindByValue(n);
                    if (li != null)
                    {
                        li.Selected = true;
                    }
                });
            }
        }

        private void LoadAllowablePaymentTypes(string currentAllowablePaymentTypes)
        {
            chkLstAllowablePaymentTypes.Items.Clear();

            List<string> allowablePaymentTypes = Util.ParseDelimitedList(currentAllowablePaymentTypes, ';');

            foreach (PaymentTypeEnum paymentType in Enum.GetValues(typeof(PaymentTypeEnum)))
            {
                if (paymentType != PaymentTypeEnum.NotSet)
                {
                    ListItem choice = new ListItem(paymentType.GetCodeValue(), paymentType.ToString());
                    chkLstAllowablePaymentTypes.Items.Add(choice);
                    if (allowablePaymentTypes.Contains(paymentType.ToString()))
                    {
                        choice.Selected = true;
                    }
                }
            }
        }

        private void LoadSubmissionTransactions(string currentSubmissionTransactionTypeCd)
        {
            ddlSubmissionTransactionTypeCd.Items.Clear();

            ddlSubmissionTransactionTypeCd.Items.Add(new ListItem("Authorize Only", TransactionTypeEnum.Authorize.ToString()));
            ddlSubmissionTransactionTypeCd.Items.Add(new ListItem("Sale (Auth and Settle)", TransactionTypeEnum.Sale.ToString()));

            ddlSubmissionTransactionTypeCd.Items.Insert(0, new ListItem("-- (none) --", string.Empty));

            WebUtil.SelectListItemByValue(ddlSubmissionTransactionTypeCd, currentSubmissionTransactionTypeCd);

        }

        private void LoadMerchantAccountConfigs(int? currentMerchantAccountConfigId)
        {
            ddlMerchantAccountConfigId.Items.Clear();

            ddlMerchantAccountConfigId.DataTextField = "Description";
            ddlMerchantAccountConfigId.DataValueField = "MerchantAccountConfigId";

            ddlMerchantAccountConfigId.DataSource = this.OwnerCtrlr.GetOwnerMerchantAccounts(CurrentUser.CurrentOwner.OwnerId);
            ddlMerchantAccountConfigId.DataBind();

            ddlMerchantAccountConfigId.Items.Insert(0, new ListItem("-- Select One --", string.Empty));

            WebUtil.SelectListItemByValue(ddlMerchantAccountConfigId, currentMerchantAccountConfigId);
        }

        private void LoadFileList()
        {
            plcAssets.Visible = true;
            string storagePath = string.Format("{0}{1}", Server.MapPath("~/Assets/Shows/"), CurrentUser.CurrentShow.ShowGuid.ToString());
            DirectoryInfo dirInfo = new DirectoryInfo(storagePath);
            grdvFileList.DataSource = dirInfo.GetFiles();
            grdvFileList.DataBind();
            grdvFileList.Visible = true;
        }
        #endregion

        #region SaveShowSettings


        #region Show / Venue / Warehouse /Remit
        private void SaveShowVenueInformation()
        {
            if (CurrentUser.CurrentShow.ShowId > 0)
            {
                CurrentUser.CurrentShow.ActiveFlag = chkActiveFlag.Checked;
                CurrentUser.CurrentShow.DisplayOnOwnerLandingPage = chkDisplayOnOwnerLanding.Checked;
                CurrentUser.CurrentShow.ShowName = txtShowName.Text.Trim();
                CurrentUser.CurrentShow.CompanyNameOnInvoice = txtCompanyNameOnInvoice.Text.Trim();
                CurrentUser.CurrentShow.StartDate = Util.ConvertNullDateTime(txtStartDate.Text.Trim());
                CurrentUser.CurrentShow.EndDate = Util.ConvertNullDateTime(txtEndDate.Text.Trim());
                CurrentUser.CurrentShow.VenueName = txtVenueName.Text.Trim();

                CurrentUser.CurrentShow.AdvanceWarehouseLabelText = txtAdvanceWarehouseLabelText.Text.Trim();
                CurrentUser.CurrentShow.DirectShowSiteLabelText = txtDirectShowSiteLabelText.Text.Trim();
                CurrentUser.CurrentShow.OutboundLabelText = txtOutboundLabelText.Text.Trim();

                if (CurrentUser.CurrentShow.VenueAddressId > 0)
                {
                    CurrentUser.CurrentShow.VenueAddress.Street1 = txtVenueAddressLine1.Text.Trim();
                    CurrentUser.CurrentShow.VenueAddress.Street2 = txtVenueAddressLine2.Text.Trim();
                    CurrentUser.CurrentShow.VenueAddress.Street3 = txtVenueAddressLine3.Text.Trim();
                    CurrentUser.CurrentShow.VenueAddress.City = txtVenueCity.Text.Trim();
                    CurrentUser.CurrentShow.VenueAddress.StateProvinceRegion = txtVenueState.Text.Trim();
                    CurrentUser.CurrentShow.VenueAddress.PostalCode = txtVenuePostalCode.Text.Trim();
                    CurrentUser.CurrentShow.VenueAddress.Country = txtVenueCountry.Text.Trim();
                    AccountCtrl.SaveAddress(CurrentUser, CurrentUser.CurrentShow.VenueAddress);
                }
                else
                {
                    Address venueAddressToSave = new Address();
                    venueAddressToSave.Street1 = txtVenueAddressLine1.Text.Trim();
                    venueAddressToSave.Street2 = txtVenueAddressLine2.Text.Trim();
                    venueAddressToSave.Street3 = txtVenueAddressLine3.Text.Trim();
                    venueAddressToSave.City = txtVenueCity.Text.Trim();
                    venueAddressToSave.StateProvinceRegion = txtVenueState.Text.Trim();
                    venueAddressToSave.PostalCode = txtVenuePostalCode.Text.Trim();
                    venueAddressToSave.Country = txtVenueCountry.Text.Trim();
                    AccountCtrl.CreateAddress(CurrentUser, venueAddressToSave);
                    CurrentUser.CurrentShow.VenueAddressId = venueAddressToSave.AddressId;
                }

                if (CurrentUser.CurrentShow.AdvanceWarehouseAddressId > 0)
                {
                    CurrentUser.CurrentShow.AdvanceWarehouseAddress.Street1 = txtWarehouseAddressLine1.Text.Trim();
                    CurrentUser.CurrentShow.AdvanceWarehouseAddress.Street2 = txtWarehouseAddressLine2.Text.Trim();
                    CurrentUser.CurrentShow.AdvanceWarehouseAddress.Street3 = txtWarehouseAddressLine3.Text.Trim();
                    CurrentUser.CurrentShow.AdvanceWarehouseAddress.Street4 = txtWarehouseAddressLine4.Text.Trim();
                    CurrentUser.CurrentShow.AdvanceWarehouseAddress.Street5 = txtWarehouseAddressLine5.Text.Trim();
                    CurrentUser.CurrentShow.AdvanceWarehouseAddress.City = txtWarehouseCity.Text.Trim();
                    CurrentUser.CurrentShow.AdvanceWarehouseAddress.StateProvinceRegion = txtWarehouseState.Text.Trim();
                    CurrentUser.CurrentShow.AdvanceWarehouseAddress.PostalCode = txtWarehousePostalCode.Text.Trim();
                    CurrentUser.CurrentShow.AdvanceWarehouseAddress.Country = txtWarehouseCountry.Text.Trim();
                    AccountCtrl.SaveAddress(CurrentUser, CurrentUser.CurrentShow.AdvanceWarehouseAddress);
                }
                else
                {
                    Address warehouseAddressToSave = new Address();
                    warehouseAddressToSave.Street1 = txtWarehouseAddressLine1.Text.Trim();
                    warehouseAddressToSave.Street2 = txtWarehouseAddressLine2.Text.Trim();
                    warehouseAddressToSave.Street3 = txtWarehouseAddressLine3.Text.Trim();
                    warehouseAddressToSave.Street4 = txtWarehouseAddressLine4.Text.Trim();
                    warehouseAddressToSave.Street5 = txtWarehouseAddressLine5.Text.Trim();
                    warehouseAddressToSave.City = txtWarehouseCity.Text.Trim();
                    warehouseAddressToSave.StateProvinceRegion = txtWarehouseState.Text.Trim();
                    warehouseAddressToSave.PostalCode = txtWarehousePostalCode.Text.Trim();
                    warehouseAddressToSave.Country = txtWarehouseCountry.Text.Trim();
                    AccountCtrl.CreateAddress(CurrentUser, warehouseAddressToSave);
                    CurrentUser.CurrentShow.AdvanceWarehouseAddressId = warehouseAddressToSave.AddressId;
                }


                if (CurrentUser.CurrentShow.RemitPaymentAddressId > 0)
                {
                    CurrentUser.CurrentShow.RemitAddress.Street1 = txtRemitAddressLine1.Text.Trim();
                    CurrentUser.CurrentShow.RemitAddress.Street2 = txtRemitAddressLine2.Text.Trim();
                    CurrentUser.CurrentShow.RemitAddress.Street3 = txtRemitAddressLine3.Text.Trim();
                    CurrentUser.CurrentShow.RemitAddress.City = txtRemitCity.Text.Trim();
                    CurrentUser.CurrentShow.RemitAddress.StateProvinceRegion = txtRemitState.Text.Trim();
                    CurrentUser.CurrentShow.RemitAddress.PostalCode = txtRemitPostalCode.Text.Trim();
                    CurrentUser.CurrentShow.RemitAddress.Country = txtRemitCountry.Text.Trim();
                    CurrentUser.CurrentShow.RemitPhone = txtRemitPhone.Text.Trim();
                    CurrentUser.CurrentShow.AdditionalRemitLine = txtAdditionalRemitLine.Text.Trim();

                    AccountCtrl.SaveAddress(CurrentUser, CurrentUser.CurrentShow.RemitAddress);
                }
                else
                {
                    Address RemitAddressToSave = new Address();
                    RemitAddressToSave.Street1 = txtRemitAddressLine1.Text.Trim();
                    RemitAddressToSave.Street2 = txtRemitAddressLine2.Text.Trim();
                    RemitAddressToSave.Street3 = txtRemitAddressLine3.Text.Trim();
                    RemitAddressToSave.City = txtRemitCity.Text.Trim();
                    RemitAddressToSave.StateProvinceRegion = txtRemitState.Text.Trim();
                    RemitAddressToSave.PostalCode = txtRemitPostalCode.Text.Trim();
                    RemitAddressToSave.Country = txtRemitCountry.Text.Trim();
                    AccountCtrl.CreateAddress(CurrentUser, RemitAddressToSave);
                    CurrentUser.CurrentShow.RemitPaymentAddressId = RemitAddressToSave.AddressId;
                }


                string shippingLabelFileName = null;
                if (fupUploadShippingLabelLogo.HasFile)
                {
                    hdnShippingLabelLogoFileName.Value = SaveFile(fupUploadShippingLabelLogo);
                    shippingLabelFileName = hdnShippingLabelLogoFileName.Value;
                    PaintShippingLabelLogoControls(shippingLabelFileName);
                }

                CurrentUser.CurrentShow.OutboundShippingLabelImage = shippingLabelFileName;

                string quickFactsFilename = null;
                if (fupUploadQuickFacts.HasFile)
                {
                    hdnQuickFactsFileName.Value = SaveFile(fupUploadQuickFacts);
                    quickFactsFilename = hdnQuickFactsFileName.Value;
                    PaintQuickViewFileNameControls(quickFactsFilename);
                }
                else
                {
                    quickFactsFilename = hdnQuickFactsFileName.Value;
                }
                CurrentUser.CurrentShow.QuickFactsFileName = quickFactsFilename;

                if (plcBoothTypesSupported.Visible)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(ListItem chk in chkBoothTypesSupported.Items)
                    {
                        if(chk.Selected)
                        {
                            sb.Append(chk.Text + ";");
                        }
                    }
                    CurrentUser.CurrentShow.BoothTypesSupported = sb.ToString();
                }
            }

            this.OwnerCtrlr.SaveShowVenueInformation(this.CurrentUser.CurrentShow);
            this.Master.DisplayFriendlyMessage("Show and Venue Information saved.");

        }
        #endregion

        #region Show Order Config

        private void SaveOrderConfig()
        {
            if (CurrentUser.CurrentShow.ShowId > 0)
            {
                CurrentUser.CurrentShow.OrderNotificationEmail = txtOrderNotificationEmail.Text.Trim();
                CurrentUser.CurrentShow.OrderFromEmail = txtOrderFromEmail.Text.Trim();
                CurrentUser.CurrentShow.OrderReplyEmail = txtOrderReplyEmail.Text.Trim();

                CurrentUser.CurrentShow.TermsAndConditions = txtTermsAndConditions.Text.Trim();
                CurrentUser.CurrentShow.TermsAndConditionsGroupName = txtTermsAndConditionsGroupName.Text.Trim();
                CurrentUser.CurrentShow.TermsAndConditionsLabel = txtTermsAndConditionsLabel.Text.Trim();
                CurrentUser.CurrentShow.TermsAndConditionsRequired = rdoTermsAndConditionsRequired.Items.FindByValue("1").Selected;
                CurrentUser.CurrentShow.DisplayBoothNumberLabel = rdoDisplayBoothNumberLabel.Items.FindByValue("1").Selected;

                CurrentUser.CurrentShow.CurrencySymbol = string.IsNullOrEmpty(txtCurrencySymbol.Text.Trim()) ? "$" : txtCurrencySymbol.Text.Trim();
                CurrentUser.CurrentShow.OrderConfirmationMessage = txtOrderConfirmationMessage.Text.Trim();
                CurrentUser.CurrentShow.InvoiceMessage = txtInvoiceMessage.Text.Trim();
                CurrentUser.CurrentShow.MerchantAccountConfigId = Util.ConvertInt32(ddlMerchantAccountConfigId.SelectedValue);
                CurrentUser.CurrentShow.SubmissionTransactionTypeCd = ddlSubmissionTransactionTypeCd.SelectedValue;
                CurrentUser.CurrentShow.FooterInfo = txtFooterInfo.Text.Trim();
                CurrentUser.CurrentShow.LoginContactInfo = txtLoginContactInfo.Text.Trim();
                CurrentUser.CurrentShow.AllowablePaymentTypeList = BuildSelectedPaymentTypes();
                CurrentUser.CurrentShow.OwnerNotifications = BuildNewOwnerNotificationList();
                CurrentUser.CurrentShow.LoginInfoText = txtLoginInfoText.Text.Trim();

                string remitLogoFileName = null;
                if (fupUploadRemitLogo.HasFile)
                {
                    hdnRemitLogoFileName.Value = SaveFile(fupUploadRemitLogo);
                    remitLogoFileName = hdnRemitLogoFileName.Value;
                    PaintRemitLogoControls(remitLogoFileName);
                }
                CurrentUser.CurrentShow.RemitLogoFileName = remitLogoFileName;
                

                CurrentUser.CurrentShow.ShoppingCartFullImage = txtShoppingCartFullImage.Text.Trim();
                CurrentUser.CurrentShow.ShoppingCartEmptyImage = txtShoppingCartEmptyImage.Text.Trim();

                OwnerCtrlr.SaveShowConfig(CurrentUser.CurrentShow);
                Master.DisplayFriendlyMessage("Show Order Configuration saved.");
            }
        }

        private List<string> BuildNewOwnerNotificationList()
        {
            List<string> ownerNotifs = new List<string>();

            foreach (ListItem li in chklstOwnerNotifications.Items)
            {
                if (li.Selected)
                {
                    ownerNotifs.Add(li.Value);
                }
            }
            return ownerNotifs;
        }


        private string BuildSelectedPaymentTypes()
        {
            List<string> allowablePaymentTypes = new List<string>();
            foreach (ListItem item in chkLstAllowablePaymentTypes.Items)
            {
                if (item.Selected)
                {
                    allowablePaymentTypes.Add(item.Value);
                }
            }

            return string.Join(";", allowablePaymentTypes.ToList());
        }

        private void PaintRemitLogoControls(string remitLogoFileName)
        {
            lblOwnerCompanyName.Text = CurrentUser.CurrentOwner.OwnerName;

            if (string.IsNullOrEmpty(remitLogoFileName))
            {
                imgRemitLogo.Visible = false;
                btnRemoveRemitLogo.Visible = false;
            }
            else
            {
                btnRemoveRemitLogo.Visible = true;
                imgRemitLogo.Visible = true;
                imgRemitLogo.ImageUrl = imgRemitLogoLink.HRef = string.Format("~/Assets/Shows/{0}/{1}", CurrentUser.CurrentShow.ShowGuid, remitLogoFileName);
                imgRemitLogo.AlternateText = remitLogoFileName;
            }

            lblRemitLogoFileName.Text = remitLogoFileName;

            hdnRemitLogoFileName.Value = remitLogoFileName;
        }

        private void PaintQuickViewFileNameControls(string quickViewFileName)
        {
           
            if (!string.IsNullOrEmpty(quickViewFileName))
            {
                lnkQuickFactsFile.HRef = string.Format("~/Assets/Shows/{0}/{1}", CurrentUser.CurrentShow.ShowGuid, quickViewFileName);
                lblQuickFactsFileName.Text = quickViewFileName;
                lnkQuickFactsFile.Visible = lblQuickFactsFileName.Visible = true;
                btnRemoveQuickFactsFile.Visible = true;
            }
            else
            {
                btnRemoveQuickFactsFile.Visible = false;
                lnkQuickFactsFile.Visible = lblQuickFactsFileName.Visible = false;
            }
            
            hdnQuickFactsFileName.Value = quickViewFileName;
        }

        private void PaintShippingLabelLogoControls(string shippingLabelLogoFileName)
        {
            if (string.IsNullOrEmpty(shippingLabelLogoFileName))
            {
                imgShippingLabelLogo.Visible = false;
                btnRemoveShippingLabelLogo.Visible = false;
            }
            else
            {
                btnRemoveShippingLabelLogo.Visible = true;
                imgShippingLabelLogo.Visible = true;
                imgShippingLabelLogo.ImageUrl = imgShippingLabelLogoLink.HRef = string.Format("~/Assets/Shows/{0}/{1}", CurrentUser.CurrentShow.ShowGuid, shippingLabelLogoFileName);
                imgShippingLabelLogo.AlternateText = shippingLabelLogoFileName;
            }

            lblShippingLabelLogoFileName.Text = shippingLabelLogoFileName;

            hdnShippingLabelLogoFileName.Value = shippingLabelLogoFileName;
        }
        #endregion


        #endregion

        #region Control Events

        protected void btnDeleteSelectedFiles_Click(object sender, EventArgs e)
        {
            List<string> fileNames = new List<string>();
            if (this.grdvFileList.Visible = true && grdvFileList.Rows.Count > 0)
            {
                foreach (GridViewRow row in grdvFileList.Rows)
                {
                    CheckBox chkFile = (CheckBox)row.FindControl("chkFile");
                    HiddenField hdnFileName = (HiddenField)row.FindControl("hdnFileName");

                    if (chkFile.Checked)
                    {
                        fileNames.Add(hdnFileName.Value);
                    }
                }
            }

            if (fileNames != null && fileNames.Count > 0)
            {
                fileNames.ForEach(f =>
                {
                    DeleteFileName(f);
                });
            }

            LoadFileList();
        }

        protected void btnRemoveRemitLogo_Click(object sender, EventArgs e)
        {
            OwnerCtrlr.RemoveShowRemitLogo(CurrentUser, CurrentUser.CurrentShow);
            InitializeShowOrderConfigInformation();
        }

        protected void btnRemoveQuickFactsFile_Click(object sender, EventArgs e)
        {
            OwnerCtrlr.RemoveQuickFactsFile(CurrentUser, CurrentUser.CurrentShow);
            InitializeShowInformation(CurrentUser.CurrentShow.ShowId);
        }

        protected void btnRemoveShippingLabelLogo_Click(object sender, EventArgs e)
        {
            OwnerCtrlr.RemoveShowShippingLabelLogo(CurrentUser, CurrentUser.CurrentShow);
            InitializeShowInformation(CurrentUser.CurrentShow.ShowId);
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            this.LoadPageMode(navLinkId, targetId);
        }

        protected void btnSaveShowInfo_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveShowVenueInformation();
            }
        }

        protected void btnSaveOrderConfig_Click(object sender, EventArgs e)
        {
            SaveOrderConfig();
        }
        
        protected void btnUploadFile_Click(object sender, EventArgs e)
        {
            if (fupUploadFile.HasFile)
            {
                SaveFile(fupUploadFile);
                LoadFileList();
                Master.DisplayFriendlyMessage("File uploaded.");
            }
        }

       

        protected void grdvFileList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                FileInfo fileItem = (FileInfo)e.Row.DataItem;
                HtmlAnchor lnkViewFile = (HtmlAnchor) e.Row.FindControl("lnkViewFile");
                Image imgFileImage = (Image)e.Row.FindControl("imgFileImage");
                Label lblFileType = (Label)e.Row.FindControl("lblFileType");
                Label lblFileSize = (Label)e.Row.FindControl("lblFileSize");

                LinkButton lbtnDeleteFile = (LinkButton)e.Row.FindControl("lbtnDeleteFile");

                if (lbtnDeleteFile != null)
                {
                    lbtnDeleteFile.Attributes.Add("onClick", "return confirm('Sure you want to delete this file?');");
                }

                FileInfo fi = new FileInfo(Server.MapPath(string.Format("~/Assets/Shows/{0}/{1}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), fileItem.Name)));

                lblFileSize.Text = Util.ToByteString(fi.Length);
                lblFileType.Text = fi.Extension;

                switch (fileItem.Extension.ToLower())
                {
                    case ".gif":
                    case ".jpg":
                    case ".png":
                    case ".bmp":
                        imgFileImage.Visible = true;
                        imgFileImage.ImageUrl = string.Format("~/Assets/Shows/{0}/{1}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), fileItem.Name);
                        imgFileImage.AlternateText = fileItem.Name;
                        break;
                    default:
                        imgFileImage.Visible = false;
                        break;
                }


                lnkViewFile.HRef = string.Format("~/Assets/Shows/{0}/{1}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), fileItem.Name);
            }
        }

        protected void lnkbtnRefreshFileList_Click(object sender, EventArgs e)
        {
            LoadFileList();
        }

        protected void grdvFileList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string fileName = e.CommandArgument.ToString();
            switch (e.CommandName)
            {
                case "DeleteFile":
                    DeleteFileName(fileName);
                    break;
            }

            LoadFileList();
        }

        private void DeleteFileName(string fileName)
        {
            string filePath = Server.MapPath(string.Format("~/Assets/Shows/{0}/{1}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), fileName));

            File.Delete(filePath);
        }

        protected void rptrExhibitorLoginUrls_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string domainUrl = e.Item.DataItem.ToString();

                HyperLink hlnkExhibitorLogin = (HyperLink)e.Item.FindControl("hlnkExhibitorLogin");
                hlnkExhibitorLogin.Target = "_new";

                hlnkExhibitorLogin.NavigateUrl = hlnkExhibitorLogin.Text = string.Format("https://{0}/Login.aspx?showid={1}", domainUrl, CurrentUser.CurrentShow.ShowId);
            }
        }

        #endregion

    }
}