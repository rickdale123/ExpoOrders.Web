using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using ExpoOrders.Common;
using ExpoOrders.Entities;
using ExpoOrders.Controllers;

using Microsoft.Practices.EnterpriseLibrary.Validation;
using ExpoOrders.Web.CustomControls;
using System.Text;


namespace ExpoOrders.Web.Exhibitors
{
    [Serializable]
    public enum PageMode { EditProfile = 1, ManageCreditCards = 2, ChangePassword = 3, ChangeUsername = 4, ManageAddress = 5 }

    [Serializable]
    public enum CreditCardEditMode { NotSet = 0, AddCreditCard = 1, EditCreditCard = 2 }

    public partial class Account : BaseExhibitorPage
    {
        private List<NavigationLink> _pageNavigationLinks = BuildSubNavLinks();

        #region Manager Objects

        private AccountController _accountMgr = null;
        public AccountController AccountMgr
        {
            get
            {
                if (_accountMgr == null)
                {
                    return new AccountController();
                }
                else
                {
                    return _accountMgr;
                }
            }
        }
        private UploadController _uploadMgr = null;
        public UploadController UploadMgr
        {
            get
            {
                if (_uploadMgr == null)
                {
                    return new UploadController();
                }
                else
                {
                    return _uploadMgr;
                }
            }
        }
        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.OnNavigationItemCallBack = this.HandleNavigationItemClicked;
            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, ExhibitorPageEnum.MyAccount);
            this.Master.LoadSubNavigation("My Account", _pageNavigationLinks);

            if (HttpContext.Current.Items.Contains("edit_credit_card_id"))
            {
                this.LoadPageMode(PageMode.ManageCreditCards);
                EditCreditCard(int.Parse(HttpContext.Current.Items["edit_credit_card_id"].ToString()));
                HttpContext.Current.Items.Remove("edit_credit_card_id");
            }
            else
            {
                this.LoadPageMode(PageMode.EditProfile);
            }
        }

        private void LoadPageMode(PageMode mode)
        {
            NavigationLink linkToSelect = _pageNavigationLinks.SingleOrDefault<NavigationLink>(l => l.TargetId == (int)mode);
            LoadPageMode(linkToSelect.NavigationLinkId, linkToSelect.TargetId.Value);
        }

        private void LoadPageMode(int navLinkId, int targetId)
        {
            if (navLinkId <= 0)
            {
                navLinkId = 1;
                targetId = (int)PageMode.EditProfile;
                this.Master.SelectNavigationItem(navLinkId);
            }

            PageMode currentPageMode = (PageMode)Enum.Parse(typeof(PageMode), targetId.ToString(), true);

            this.plcEditProfile.Visible =
                this.plcManageCreditCards.Visible =
                this.plcManageAddresses.Visible = 
                this.plcChangePassword.Visible =
                this.plcChangeUsername.Visible = false;

            switch (currentPageMode)
            {
                case PageMode.EditProfile:
                    this.plcEditProfile.Visible = true;
                    InitializeExhibitorInfo();
                    this.Master.SelectNavigationItem(1);
                    break;
                case PageMode.ManageCreditCards:
                    InitializeCreditCardInfo();
                    this.Master.SelectNavigationItem(2);
                    break;
                case PageMode.ChangePassword:
                    DisplayPasswordChange();
                    this.Master.SelectNavigationItem(3);
                    break;
                case PageMode.ChangeUsername:
                    this.plcChangeUsername.Visible = true;
                    break;
                case PageMode.ManageAddress:
                    InitializeExhibitorAddresses();
                    this.Master.SelectNavigationItem(4);
                    break;

            }
        }

        private void DisplayPasswordChange()
        {
            lblPreferredUserName.Text = string.Empty;

            UserContainer userInfo = this.AccountMgr.GetMemberShipUser(CurrentUser.UserId.ToString(), false);

            if (userInfo != null)
            {
                lblPreferredUserName.Text = WebUtil.HtmlEncode(userInfo.PreferredUserName);
            }
            this.plcChangePassword.Visible = true;
        }

        private static List<NavigationLink> BuildSubNavLinks()
        {
            List<NavigationLink> links = new List<NavigationLink>();

            links.Add(
                    new NavigationLink(1, "Profile Information", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PostBack, (int)PageMode.EditProfile)
                    );


            links.Add(
                    new NavigationLink(2, "Manage Credit Cards", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PostBack, (int)PageMode.ManageCreditCards)
                    );

            links.Add(
                    new NavigationLink(3, "Change Your Password", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PostBack, (int)PageMode.ChangePassword)
                );


            links.Add(
                    new NavigationLink(4, "Outbound Shipping Addresses", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PostBack, (int)PageMode.ManageAddress)
                );


            //RD: I'm not gonna allow them to Change Username, only password
            // they can call the Show Owner if they don't like their Username
            /*
            links.Add(
                    new NavigationLink(4, "Change Your Username", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PostBack, (int)PageMode.ChangeUsername)
                );
            */

            return links;
        }
        #endregion

        #region Control Events

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            this.LoadPageMode(navLinkId, targetId);
        }
        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "ProfileInformation";
            if (Page.IsValid)
            {
                SaveProfileInformation();

                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Profile information saved.");
                }
            }

        }

        
        #endregion

        #region Profile Information
        
        private void SaveProfileInformation()
        {
            ValidationResults errors = AccountMgr.ValidateExhibitorProfile(txtCompanyName.Text.Trim(), txtCompanyEmailAddress.Text.Trim(), txtUserFirstName.Text.Trim(), txtUserLastName.Text.Trim(), txtUserEmailAddress.Text.Trim());

            if (errors.IsValid)
            {
                //Save Exhibitor Profile Details
                Exhibitor exhibitorDetail = this.CurrentUser.CurrentExhibitor;
                exhibitorDetail.ActiveFlag = true;
                exhibitorDetail.CompanyAddressId = SaveProfileAddress(exhibitorDetail);
                exhibitorDetail.ExhibitorCompanyName = txtCompanyName.Text.Trim();
                exhibitorDetail.Phone = txtCompanyPhone.Text.Trim();
                exhibitorDetail.PrimaryEmailAddress = txtCompanyEmailAddress.Text.Trim();
                AccountMgr.UpdateExhibitor(exhibitorDetail);

                //Save Extended User Information
                CurrentUser.FirstName = txtUserFirstName.Text.Trim();
                CurrentUser.LastName = txtUserLastName.Text.Trim();
                CurrentUser.Phone = txtUserPhone.Text.Trim();
                CurrentUser.AspMembershipUser.Email = txtUserEmailAddress.Text.Trim();
                CurrentUser.Title = txtUserTitle.Text.Trim();
                AccountMgr.SaveCurrentUser(CurrentUser, false);
            }

            if (errors.IsValid)
            {
                //Refresh the CurrentExhibitor.Address
                LoginController loginCtrl = new LoginController(CurrentUser);
                CurrentUser.CurrentExhibitor = loginCtrl.RefreshExhibitorByUserIdAndShowId(new Guid(CurrentUser.AspMembershipUser.ProviderUserKey.ToString()), CurrentUser.CurrentShow.ShowId);
            }
            else
            {
                PageErrors.AddErrorMessages(errors);
            }

        }

        private int SaveProfileAddress(Exhibitor exhibitor)
        {
            //Todo: move this to the Controller level, no biz logic on pages, please
            Address addressToSave = new Address();
            if (exhibitor.CompanyAddressId > 0)
            {
                addressToSave.AddressId = exhibitor.Address.AddressId;
            }
            addressToSave.Street1 = txtAddressLine1.Text.Trim();
            addressToSave.Street2 = txtAddressLine2.Text.Trim();
            addressToSave.City = txtCity.Text.Trim();
            addressToSave.StateProvinceRegion = txtState.Text.Trim();
            addressToSave.PostalCode = txtPostalCode.Text.Trim();
            addressToSave.Country = txtCountry.Text.Trim();
            if (exhibitor.CompanyAddressId > 0)
            {
                AccountMgr.SaveAddress(CurrentUser, addressToSave);
            }
            else
            {
                AccountMgr.CreateAddress(CurrentUser, addressToSave);
            }

            return addressToSave.AddressId;
        }

        private void InitializeExhibitorInfo()
        {
            Exhibitor currentExhibitor = this.CurrentUser.CurrentExhibitor;
            ExpoOrdersUser currentUser = this.CurrentUser;

            if (currentExhibitor != null && currentExhibitor.ExhibitorId > 0)
            {
                lblReadOnlyTaxExempt.Text = currentExhibitor.TaxExemptFlag ? "Yes" : "No";

                txtCompanyName.Text = lblReadOnlyCompanyName.Text = currentExhibitor.ExhibitorCompanyName;
                if (currentExhibitor.CompanyNameEditableFlag || this.CurrentUser.IsOwner)
                {
                    txtCompanyName.Visible = true;
                    lblReadOnlyCompanyName.Visible = false;
                }
                else
                {
                    txtCompanyName.Visible = false;
                    lblReadOnlyCompanyName.Visible = true;
                }

                txtAddressLine1.Text = currentExhibitor.Address.Street1;
                txtAddressLine2.Text = currentExhibitor.Address.Street2;
                txtCity.Text = currentExhibitor.Address.City;

                txtState.Text = currentExhibitor.Address.StateProvinceRegion;

                txtPostalCode.Text = currentExhibitor.Address.PostalCode;
                txtCountry.Text = currentExhibitor.Address.Country;

                txtCompanyEmailAddress.Text = currentExhibitor.PrimaryEmailAddress;

                if (!string.IsNullOrEmpty(currentExhibitor.Phone))
                {
                    txtCompanyPhone.Text = currentExhibitor.Phone;
                }

                txtUserTitle.Text = currentUser.Title;
                txtUserFirstName.Text = currentUser.FirstName;
                txtUserLastName.Text = currentUser.LastName;
                txtUserEmailAddress.Text = currentUser.EmailAddress;

                if (!string.IsNullOrEmpty(currentUser.Phone))
                {
                    txtUserPhone.Text = currentUser.Phone;
                }
               
            }
        }

        protected void btnCancelProfile_Click(object sender, EventArgs e)
        {
            LoadPageMode(PageMode.EditProfile);
        }

        protected void btnCancelCreditCard_Click(object sender, EventArgs e)
        {
            LoadPageMode(PageMode.ManageCreditCards);
        }

        #endregion

        #region Manage Credit Cards

        private void InitializeCreditCardInfo()
        {
            this.plcManageCreditCards.Visible = true;

            this.plcCreditCardList.Visible = true;
            BindCreditCardList();

            this.plcCreditCardDetail.Visible = false;
            this.btnAddCreditCard.Visible = true;
        }

        private void BindCreditCardList()
        {
            this.rptCreditCardList.Visible = true;
            this.rptCreditCardList.DataSource = AccountMgr.GetCreditCardListByExhibitor(CurrentUser.CurrentExhibitor.ExhibitorId);
            this.rptCreditCardList.DataBind();
        }

        protected void btnAddCreditCard_Click(object sender, EventArgs e)
        {
            this.btnAddCreditCard.Visible = false;
            this.plcCreditCardDetail.Visible = true;
            this.plcCreditCardList.Visible = false;

            this.LoadCreditCardTypes();
            this.LoadCreditCardExpirationOptions();

            this.ClearPlaceHolderControl(plcCreditCardDetail);

            hdnCreditCardId.Value = "0";
            hdnCreditCardAddressId.Value = "0";

            txtCreditCardEmail.Text = CurrentUser.CurrentExhibitor.PrimaryEmailAddress;
        }

        private void ClearPlaceHolderControl(PlaceHolder placeHolderControl)
        {
            foreach (Control control in placeHolderControl.Controls)
            {
                if (control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    textBox.Text = string.Empty;
                }
                else if (control is DropDownList)
                {
                    DropDownList dropDownList = (DropDownList)control;
                    dropDownList.SelectedIndex = 0;
                }
            }

        }

        protected void btnSaveCreditCard_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "CreditCardInfo";
            if (txtCreditCardNumber.Text.Contains("*"))
            {
                ValidationResult error = new ValidationResult("The card number is invalid", null, null, null, null);
                PageErrors.AddErrorMessage(error, PageErrors.ValidationGroup);
            }

            if (Page.IsValid)
            {
                SaveCreditCard();

                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Credit card saved.");

                    BindCreditCardList();
                    plcCreditCardDetail.Visible = false;
                    plcCreditCardList.Visible = true;
                    btnAddCreditCard.Visible = true;
                }
            }
        }

        protected void rptCreditCardList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CreditCard card = (CreditCard)e.Item.DataItem;

                HtmlTableRow trCreditCard = (HtmlTableRow)e.Item.FindControl("trCreditCard");

                trCreditCard.Attributes["class"] = (e.Item.ItemType == ListItemType.AlternatingItem) ? "altItem" : "item";

                Literal ltrNameOnCreditCard = (Literal)e.Item.FindControl("ltrNameOnCreditCard");
                Literal ltrCreditCardNumber = (Literal)e.Item.FindControl("ltrCreditCardNumber");
                Literal ltrCreditCardExpirationDate = (Literal)e.Item.FindControl("ltrCreditCardExpirationDate");
                Literal ltrCreditCardEmailAddress = (Literal)e.Item.FindControl("ltrCreditCardEmailAddress");

                LinkButton btnEditCard = (LinkButton)e.Item.FindControl("btnEditCard");
                LinkButton btnDeleteCard = (LinkButton)e.Item.FindControl("btnDeleteCard");
                btnEditCard.CommandArgument = card.CreditCardId.ToString();
                btnDeleteCard.CommandArgument = card.CreditCardId.ToString();

                btnDeleteCard.Attributes.Add("OnClick", "return confirm('Are you sure you want to delete this card?');");

                ltrNameOnCreditCard.Text = WebUtil.HtmlEncode(card.NameOnCard);
                ltrCreditCardNumber.Text = WebUtil.HtmlEncode(card.CardNumberMasked);
                ltrCreditCardExpirationDate.Text = card.ExpirationMonth + "/" + card.ExpirationYear;
                ltrCreditCardEmailAddress.Text = WebUtil.HtmlEncode(card.EmailAddress);
            }
        }

        protected void btnEdit_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EditCard")
            {
                LinkButton btnEdit = (LinkButton)e.CommandSource;
                EditCreditCard(int.Parse(btnEdit.CommandArgument));
            }
            if (e.CommandName == "DeleteCard")
            {
                LinkButton btnDelete = (LinkButton)e.CommandSource;
                DeleteCreditCard(int.Parse(btnDelete.CommandArgument));
            }
        }

        private void EditCreditCard(int creditCardId)
        {
            PopulateCreditCardDetails(creditCardId);
            plcCreditCardList.Visible = false;
            plcCreditCardDetail.Visible = true;
        }

        private void DeleteCreditCard(int currentCardId)
        {
            AccountMgr.DeleteCreditCardById(currentCardId, true);
            BindCreditCardList();
            this.Master.DisplayFriendlyMessage("Credit card deleted.");

        }

        private void PopulateCreditCardDetails(int currentCardId)
        {
            this.LoadCreditCardTypes();
            this.LoadCreditCardExpirationOptions();

            CreditCard currentCard = AccountMgr.GetCreditCardById(currentCardId);
            hdnCreditCardId.Value = currentCard.CreditCardId.ToString();
            
            txtCreditCardName.Text = currentCard.NameOnCard;
            txtCreditCardNumber.Text = currentCard.CardNumberDecrypted;

            hdnCreditCardAddressId.Value = currentCard.Address.AddressId.ToString();
            txtCreditCardAddressLine1.Text = currentCard.Address.Street1;
            txtCreditCardAddressLine2.Text = currentCard.Address.Street2;
            txtCreditCardCity.Text = currentCard.Address.City;
            txtCreditCardState.Text = currentCard.Address.StateProvinceRegion;
            
            txtCreditCardPostalCode.Text = currentCard.Address.PostalCode;
            txtCreditCardCountry.Text = currentCard.Address.Country;
            txtCreditCardEmail.Text = currentCard.EmailAddress;

            txtCreditCardSecurityCode.Text = currentCard.SecurityCodeDecrypted;

            WebUtil.SelectListItemByValue(ddlCreditCardType, currentCard.CreditCardTypeCd);
            WebUtil.SelectListItemByValue(ddlCreditCardExpMonth, currentCard.ExpirationMonth);
            WebUtil.SelectListItemByValue(ddlCreditCardExpYear, currentCard.ExpirationYear);

        }


        private CreditCard BuildCreditCard()
        {
            CreditCard creditCard = new CreditCard();

            creditCard.ExhibitorId = CurrentUser.CurrentExhibitor.ExhibitorId;

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

        private void SaveCreditCard()
        {
            CreditCard creditCard = BuildCreditCard();
            Address billingAddress = BuildCreditBillingAddress();

            ValidationResults creditCardErrors = AccountMgr.StoreCreditCard(CurrentUser, creditCard, billingAddress, true);

            if (!creditCardErrors.IsValid)
            {
                PageErrors.AddErrorMessages(creditCardErrors, PageErrors.ValidationGroup);
            }

        }


        private void LoadCreditCardTypes()
        {
            ddlCreditCardType.Items.Clear();
            ddlCreditCardType.DataSource = AccountMgr.GetCreditCardTypesList(CurrentUser.CurrentShow.MerchantAccountConfigId);
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

        #endregion

        #region Exhibitor Addresses

        private void InitializeExhibitorAddresses()
        {
            this.plcManageCreditCards.Visible = false;
            this.plcCreditCardList.Visible = false;
            this.plcCreditCardDetail.Visible = false;

            this.plcManageAddresses.Visible = true;
            this.plcAddressDetail.Visible = false;
            this.plcAddressList.Visible = true;

            LoadExhibitorAddresses();
        }

        private void DeleteAddress(int addressId)
        {
            AccountMgr.DeleteAddress(CurrentUser, addressId);
            LoadExhibitorAddresses();
        }

        private void ManageAddress(int addressId)
        {
            plcManageAddresses.Visible = true;
            plcAddressList.Visible = false;
            plcAddressDetail.Visible = true;

            hdnAddressId.Value = addressId.ToString();

            ClearPlaceHolderControl(plcAddressDetail);

            if (addressId > 0)
            {
                Address currentAddress = AccountMgr.GetAddress(CurrentUser, addressId);

                if (currentAddress != null)
                {
                    WebUtil.SelectListItemByValue(ddlOtherAddressType, currentAddress.AddressTypeCd);
                    txtOtherAddressLine1.Text = currentAddress.Street1;
                    txtOtherAddressLine2.Text = currentAddress.Street2;
                    txtOtherAddressLine3.Text = currentAddress.Street3;
                    txtOtherAddressLine4.Text = currentAddress.Street4;
                    //txtOtherAddressLine5.Text = currentAddress.Street5;
                    txtOtherAddressCity.Text = currentAddress.City;
                    txtOtherAddressState.Text = currentAddress.StateProvinceRegion;
                    txtOtherAddressPostalCode.Text = currentAddress.PostalCode;
                    txtOtherAddressCountry.Text = currentAddress.Country;

                }
            }
        }

        protected void btnAddAddress_Click(object sender, EventArgs e)
        {
            ManageAddress(0);
        }

        protected void btnAddressListRefresh_Click(object sender, EventArgs e)
        {
            LoadExhibitorAddresses();
        }

        protected void btnCancelManageAddress_Click(object sender, EventArgs e)
        {
            this.LoadPageMode(PageMode.EditProfile);
        }

        protected void grdvwAddressList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Address address = (Address)e.Row.DataItem;

                LinkButton lbtnDeleteAddress = (LinkButton)e.Row.FindControl("lbtnDeleteAddress");
                Literal ltrOtherFullAddress = (Literal)e.Row.FindControl("ltrOtherFullAddress");
                Label lblAddressType = (Label)e.Row.FindControl("lblAddressType");


                lbtnDeleteAddress.Attributes.Add("OnClick", "return confirm('Sure you want to delete this address?');");

                string addressType = address.AddressTypeCd;
                if (address.AddressTypeCd == "Outbound")
                {
                    addressType = "Outbound Shipping";
                }
                lblAddressType.Text = addressType;

                StringBuilder fullAddress = new StringBuilder();
                if (!string.IsNullOrEmpty(address.Street1))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street1, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street2))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street2, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street3))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street3, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street4))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street4, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street5))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street5, "<br/>"));
                }

                //string.Concat(address.Street1, "<br/>", address.Street2, "<br/>", address.Street3, "<br/>", address.Street4, "<br/>", address.Street5).Trim());
                if (string.Concat(address.City + address.StateProvinceRegion + address.PostalCode).Trim().Length > 0)
                {
                    fullAddress.Append(string.Format("{0}, {1}  {2}", address.City, address.StateProvinceRegion, address.PostalCode));
                }

                if (!string.IsNullOrEmpty(address.Country))
                {
                    fullAddress.Append(address.Country);
                }

                ltrOtherFullAddress.Text = fullAddress.ToString();

            }
        }

        protected void grdvwAddressList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int addressId = Util.ConvertInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ManageAddress":
                    ManageAddress(addressId);
                    break;
                case "DeleteAddress":
                    DeleteAddress(addressId);
                    break;
            }
        }

        private void LoadExhibitorAddresses()
        {
            plcManageAddresses.Visible = true;
            plcAddressList.Visible = true;
            plcAddressDetail.Visible = false;
            BindExhibitorAddressList();

        }

        private void BindExhibitorAddressList()
        {
            plcAddressList.Visible = true;
            grdvwAddressList.DataSource = AccountMgr.GetExhibitorById(CurrentUser.CurrentExhibitor.ExhibitorId).Addresses.Where(a => a.ActiveFlag == true && a.AddressTypeCd == "Outbound");
            grdvwAddressList.DataBind();
        }


        protected void btnSaveAddress_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "AddressInformation";

            if (Page.IsValid)
            {
                Address address = new Address()
                {
                    AddressId = Util.ConvertInt32(hdnAddressId.Value),
                    Street1 = txtOtherAddressLine1.Text.Trim(),
                    Street2 = txtOtherAddressLine2.Text.Trim(),
                    Street3 = txtOtherAddressLine3.Text.Trim(),
                    Street4 = txtOtherAddressLine4.Text.Trim(),
                    //Street5 = txtOtherAddressLine5.Text.Trim(),
                    City = txtOtherAddressCity.Text.Trim(),
                    StateProvinceRegion = txtOtherAddressState.Text.Trim(),
                    PostalCode = txtOtherAddressPostalCode.Text.Trim(),
                    Country = txtOtherAddressCountry.Text.Trim(),
                    ActiveFlag = true,
                    ExhibitorId = CurrentUser.CurrentExhibitor.ExhibitorId,
                    AddressTypeCd = ddlOtherAddressType.SelectedValue

                };
                ValidationResults errors = AccountMgr.SaveAddress(CurrentUser, address);

                if (errors.IsValid)
                {
                    LoadExhibitorAddresses();
                    this.Master.DisplayFriendlyMessage("Address saved.");
                }
                else
                {
                    PageErrors.AddErrorMessages(errors, PageErrors.ValidationGroup);
                }
            }
        }

        protected void btnCancelSaveAddress_Click(object sender, EventArgs e)
        {
            LoadExhibitorAddresses();
        }

        #endregion

        #region Change Password

        protected void btnSavePassword_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "PasswordChange";
            if ((string.IsNullOrEmpty(txtPassword1.Text.Trim()) == true) || 
                (string.IsNullOrEmpty(txtPassword2.Text.Trim()) == true) || 
                (txtPassword1.Text.Trim() != txtPassword2.Text.Trim()))
            {
                ValidationResult error = new ValidationResult("Passwords must match.", null, null, null, null);
                PageErrors.AddErrorMessage(error, PageErrors.ValidationGroup);
            }

            if (Page.IsValid)
            {
                SavePassword();
                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Password was saved.");
                }
            }

        }

        private void SavePassword()
        {
            ValidationResults errors = this.AccountMgr.SavePassword(CurrentUser, txtPassword1.Text.Trim());
            if(!errors.IsValid)
            {
                this.PageErrors.AddErrorMessages(errors, this.PageErrors.ValidationGroup);
            }
        }
        #endregion

        #region Change Username
        protected void btnSaveUsername_Click(object sender, EventArgs e)
        {
            bool userNameChangeSupport = true;
            PageErrors.ValidationGroup = "UsernameChange";
            if (string.IsNullOrEmpty(txtUsername.Text.Trim()))
            {
                ValidationResult error = new ValidationResult("A new username is required", null, null, null, null);
                PageErrors.AddErrorMessage(error, PageErrors.ValidationGroup);
            }
            else if (!userNameChangeSupport)
            {
                ValidationResult error = new ValidationResult("Username changes are not currently supported.", null, null, null, null);
                PageErrors.AddErrorMessage(error, PageErrors.ValidationGroup);
            }

            if (Page.IsValid)
            {
                SaveUsername();
                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Username saved.");
                }
            }
        }

        private void SaveUsername()
        {
            ValidationResults errors = AccountMgr.SaveUserName(CurrentUser.AspMembershipUser.UserName, txtUsername.Text.Trim());
            if (!errors.IsValid)
            {
                this.PageErrors.AddErrorMessages(errors, this.PageErrors.ValidationGroup);
            }
        }

        #endregion

    }
}