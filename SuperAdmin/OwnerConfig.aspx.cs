using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Controllers;
using ExpoOrders.Common;
using ExpoOrders.Entities;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.IO;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class OwnerConfig : BaseSuperAdminPage
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
            HideAllPlaceholders();

            DisplayOwnerList();

        }

        private void HideAllPlaceholders()
        {
            plcMerchantAccountDetail.Visible = false;
            plcOwnerDetail.Visible = false;
            plcOwnerList.Visible = false;
            plcMerchantAccountList.Visible = false;
            plcOwnerHostConfigs.Visible = false;
        }

        private void DisplayOwnerList()
        {
            HideAllPlaceholders();
            plcOwnerList.Visible = true;
            RefreshOwnerHostConfig();

            this.grdvwOwnerList.DataSource = Cntrl.GetOwnerList().OrderBy(o => o.OwnerId);
            this.grdvwOwnerList.DataBind();
        }

        private void LoadOwnerDetail(int ownerId)
        {
            HideAllPlaceholders();
            plcOwnerDetail.Visible = true;

            ResetOwnerDetailControls();

            if (ownerId > 0)
            {
                Owner owner = Cntrl.GetOwner(ownerId);

                if (owner != null)
                {
                    SelectedOwnerId = owner.OwnerId;

                    lblOwnerId.Text = owner.OwnerId.ToString();
                    txtOwnerName.Text = owner.OwnerName;
                    txtPrimaryContactEmail.Text = owner.PrimaryContactEmail;
                    txtOwnerTerms.Text = owner.Terms;
                    txtPrimaryContactName.Text = owner.PrimaryContactName;

                    if (owner.Address != null)
                    {
                        hdnAddressId.Value = owner.OwnerAddressId.ToString();
                        txtAddressLine1.Text = owner.Address.Street1;
                        txtAddressLine2.Text = owner.Address.Street2;
                        txtAddressLine3.Text = owner.Address.Street3;
                        txtCity.Text = owner.Address.City;
                        txtState.Text = owner.Address.StateProvinceRegion;
                        txtPostalCode.Text = owner.Address.PostalCode;
                        txtCountry.Text = owner.Address.Country;
                    }

                    txtLogoFileName.Text = owner.LogoFileName;
                    txtCommonFolder.Text = owner.CommonFolder;

                    txtShoppingCartFullImage.Text = owner.ShoppingCartFullImage;
                    txtShoppingCartEmptyImage.Text = owner.ShoppingCartEmptyImage;

                    lnkCreateFolder.Visible = false;
                    if (!string.IsNullOrEmpty(owner.CommonFolder))
                    {
                        if (!Directory.Exists(WebUtil.OwnerSharedFileDirectory(owner)))
                        {
                            lnkCreateFolder.Visible = true;
                        }
                    }

                    txtStyleSheetFileName.Text = owner.StyleSheetFileName;
                    txtOwnerSubDomain.Text = owner.OwnerSubDomains;
                    txtSmtpConfigXml.Text = owner.SmtpConfigXmlDecrypted;

                    txtSortOrder.Text = owner.SortOrder.ToString();
                    chkActiveFlag.Checked = owner.ActiveFlag;
                    if (owner.CreatedOn.HasValue)
                    {
                        lblCreatedOn.Text = owner.CreatedOn.Value.ToString();
                    }

                    if (owner.ModifiedOn.HasValue)
                    {
                        lblModifiedOn.Text = owner.ModifiedOn.ToString();
                    }
                }
            }
        }

        private Owner BuildOwnerDetail()
        {
            Owner ownerDetail = new Owner();

            ownerDetail.OwnerId = SelectedOwnerId;

            ownerDetail.Address = new Address();
            ownerDetail.Address.AddressId = Util.ConvertInt32(hdnAddressId.Value);
            ownerDetail.OwnerName = txtOwnerName.Text.Trim();
            ownerDetail.PrimaryContactEmail = txtPrimaryContactEmail.Text.Trim();
            ownerDetail.Terms = txtOwnerTerms.Text.Trim();
            ownerDetail.PrimaryContactName = txtPrimaryContactName.Text.Trim();
            ownerDetail.Address.Street1 = txtAddressLine1.Text.Trim();
            ownerDetail.Address.Street2 = txtAddressLine2.Text.Trim();
            ownerDetail.Address.Street3 = txtAddressLine3.Text.Trim();
            ownerDetail.Address.City = txtCity.Text.Trim();
            ownerDetail.Address.StateProvinceRegion = txtState.Text.Trim();
            ownerDetail.Address.PostalCode = txtPostalCode.Text.Trim();
            ownerDetail.Address.Country = txtCountry.Text.Trim();

            ownerDetail.LogoFileName = txtLogoFileName.Text.Trim();
            ownerDetail.CommonFolder = txtCommonFolder.Text.Trim();
            ownerDetail.StyleSheetFileName = txtStyleSheetFileName.Text.Trim();
            ownerDetail.OwnerSubDomains = txtOwnerSubDomain.Text.Trim();
            ownerDetail.SetEncryptedSmtpConfigXml(txtSmtpConfigXml.Text.Trim());
            ownerDetail.SortOrder = Util.ConvertInt32(txtSortOrder.Text.Trim());
            ownerDetail.ActiveFlag = chkActiveFlag.Checked;

            ownerDetail.ShoppingCartFullImage = txtShoppingCartFullImage.Text.Trim();
            ownerDetail.ShoppingCartEmptyImage = txtShoppingCartEmptyImage.Text.Trim();

            return ownerDetail;
        }

        private void ResetOwnerDetailControls()
        {
            SelectedOwnerId = 0;
            hdnAddressId.Value = string.Empty;
            lblOwnerId.Text = string.Empty;
            txtOwnerName.Text = string.Empty;
            txtPrimaryContactEmail.Text = string.Empty;
            txtOwnerTerms.Text = string.Empty;
            txtPrimaryContactName.Text = string.Empty;
            txtAddressLine1.Text = txtAddressLine2.Text = txtAddressLine3.Text = string.Empty;
            txtCity.Text = string.Empty;
            txtState.Text = string.Empty;
            txtPostalCode.Text = string.Empty;
            txtCountry.Text = string.Empty;
            txtLogoFileName.Text = string.Empty;
            txtCommonFolder.Text = string.Empty;
            txtStyleSheetFileName.Text = string.Empty;
            txtOwnerSubDomain.Text = string.Empty;
            txtSmtpConfigXml.Text = string.Empty;
            txtSortOrder.Text = string.Empty;
            chkActiveFlag.Checked = true;
            lblCreatedOn.Text = string.Empty;
            lblModifiedOn.Text = string.Empty;

            txtShoppingCartEmptyImage.Text = string.Empty;
            txtShoppingCartFullImage.Text = string.Empty;
        }

        private void LoadOwnerMerchantAccounts(int ownerId)
        {
            HideAllPlaceholders();
            plcMerchantAccountList.Visible = true;

            SelectedOwnerId = ownerId;

            grvwMerchantAccountList.DataSource = Cntrl.GetOwnerMerchantAccounts(ownerId);
            grvwMerchantAccountList.DataBind();

        }

        private void LoadOwnerMerchantAccountDetail(int ownerId, int merchantAccountConfigId)
        {
            HideAllPlaceholders();
            plcMerchantAccountDetail.Visible = true;


            SelectedOwnerId = ownerId;
            lblMerchantAccountId.Text = string.Empty;
            SelectedMerchantAccountId = 0;
            txtMerchantAccountDescription.Text = string.Empty;
            ddlMerchantAccountPaymentGateway.Items.Clear();
            chkLstMerchantAccountCreditCards.Items.Clear();
            txtMerchantAccountConfigXml.Text = string.Empty;

            PopulatePaymentGatewayList();
            PopulateCreditCardList();

            if (merchantAccountConfigId > 0)
            {
                MerchantAccountConfig merchantConfig = Cntrl.GetMerchantAccountConfig(merchantAccountConfigId);

                if (merchantConfig != null)
                {
                    SelectedMerchantAccountId = merchantConfig.MerchantAccountConfigId;

                    lblMerchantAccountId.Text = merchantConfig.MerchantAccountConfigId.ToString();
                    txtMerchantAccountDescription.Text = merchantConfig.Description;
                    ListItem pg = ddlMerchantAccountPaymentGateway.Items.FindByValue(merchantConfig.PaymentGateway);
                    if (pg != null)
                    {
                        pg.Selected = true;
                    }

                    if (!string.IsNullOrEmpty(merchantConfig.PaymentGateway))
                    {
                        ListItem li = ddlMerchantAccountPaymentGateway.Items.FindByValue(merchantConfig.PaymentGateway);
                        if (li != null)
                        {
                            li.Selected = true;
                        }
                    }
                    if (merchantConfig.MerchantAccountCreditCardTypeXrefs != null)
                    {
                        merchantConfig.MerchantAccountCreditCardTypeXrefs.ForEach(cc =>
                        {
                            ListItem ccLi = chkLstMerchantAccountCreditCards.Items.FindByValue(cc.CreditCardTypeCd);
                            if (ccLi != null)
                            {
                                ccLi.Selected = true;
                            }
                        });
                    }
                    txtMerchantAccountConfigXml.Text = merchantConfig.ConfigXmlDecrypted;
                }
            }
        }

        private void PopulatePaymentGatewayList()
        {
            ddlMerchantAccountPaymentGateway.Items.Clear();

            ddlMerchantAccountPaymentGateway.Items.Add(new ListItem("Authorize.Net", "AuthorizeNET"));
            ddlMerchantAccountPaymentGateway.Items.Add(new ListItem("PayFlowPro", "PayFlowPro"));
            ddlMerchantAccountPaymentGateway.Items.Add(new ListItem("Sage", "Sage"));
            ddlMerchantAccountPaymentGateway.Items.Add(new ListItem("QuickBooks", "QuickBooks"));
            ddlMerchantAccountPaymentGateway.Items.Add(new ListItem("Payments Gateway", "PaymentsGateway"));

            ddlMerchantAccountPaymentGateway.Items.Insert(0, new ListItem("-- Select One --", string.Empty));
        }
        private void PopulateCreditCardList()
        {

            chkLstMerchantAccountCreditCards.Items.Clear();
            chkLstMerchantAccountCreditCards.DataTextField = "Name";
            chkLstMerchantAccountCreditCards.DataValueField = "CreditCardTypeCd";
            chkLstMerchantAccountCreditCards.DataSource = Cntrl.GetCreditCardTypes();
            chkLstMerchantAccountCreditCards.DataBind();
        }

        protected void grdvwOwnerList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

            }
        }

        protected void grdvwOwnerList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int ownerId = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "EditOwner":
                    LoadOwnerDetail(ownerId);
                    break;
                case "ViewMerchantAccounts":
                    LoadOwnerMerchantAccounts(ownerId);
                    break;

            }
        }

        protected void btnAddOwner_Click(object sender, EventArgs e)
        {
            LoadOwnerDetail(0);
        }

        protected void lnkCreateFolder_Click(object sender, EventArgs e)
        {
            string commonFolder = txtCommonFolder.Text.Trim();
            if (!Directory.Exists(WebUtil.OwnerSharedFileDirectory(commonFolder)))
            {
                Directory.CreateDirectory(WebUtil.OwnerSharedFileDirectory(commonFolder));
                Master.DisplayFriendlyMessage("Owner Common Folder Created.");
            }
        }

        protected void btnSaveOwnerDetail_Click(object sender, EventArgs e)
        {
            Owner ownerDetail = BuildOwnerDetail();
            ValidationResults errors = Cntrl.SaveOwnerConfig(CurrentUser, ownerDetail);

            if (errors.IsValid)
            {
                DisplayOwnerList();
                Master.DisplayFriendlyMessage("Owner Config Saved.");
            }
            else
            {
                Master.AddErrorMessages(errors);
            }

        }
        protected void btnCancelOwnerDetail_Click(object sender, EventArgs e)
        {
            DisplayOwnerList();
        }


        protected void btnCancelMerchantAccountList_Click(Object sender, EventArgs e)
        {
            this.DisplayOwnerList();
        }

        protected void btnAddMerchantAccount_Click(Object sender, EventArgs e)
        {
            this.LoadOwnerMerchantAccountDetail(SelectedOwnerId, 0);
        }

        protected void btnSaveMerchantAccountConfig_Click(Object sender, EventArgs e)
        {
            int ownerId = SelectedOwnerId;
            MerchantAccountConfig merchantAccountConfig = new MerchantAccountConfig();

            merchantAccountConfig.OwnerId = ownerId;
            merchantAccountConfig.MerchantAccountConfigId = SelectedMerchantAccountId;
            merchantAccountConfig.Description = txtMerchantAccountDescription.Text.Trim();
            merchantAccountConfig.PaymentGateway = ddlMerchantAccountPaymentGateway.SelectedValue;
            merchantAccountConfig.SetEncryptedConfigXml(this.txtMerchantAccountConfigXml.Text.Trim());

            List<string> creditCardsAccepted = new List<string>();
            foreach (ListItem li in chkLstMerchantAccountCreditCards.Items)
            {
                if (li.Selected)
                {
                    creditCardsAccepted.Add(li.Value);
                }
            }


            ValidationResults errors = Cntrl.SaveMerchantAccountConfig(CurrentUser, merchantAccountConfig, creditCardsAccepted);

            if (errors.IsValid)
            {
                LoadOwnerMerchantAccounts(ownerId);
                this.Master.DisplayFriendlyMessage("Merchant Account Saved.");
            }
            else
            {
                this.Master.AddErrorMessages(errors);
            }
        }

        protected void btnCancelSaveMerchantAccount_Click(Object sender, EventArgs e)
        {
            DisplayOwnerList();
        }

        protected void grvwMerchantAccountList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

            }
        }

        protected void grvwMerchantAccountList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int ownerId = Util.ConvertInt32(e.CommandArgument.ToString().Split(':')[0]);
            int merchantAccountConfigId = Util.ConvertInt32(e.CommandArgument.ToString().Split(':')[1]);
            switch (e.CommandName)
            {
                case "EditMerchantAccount":
                    LoadOwnerMerchantAccountDetail(ownerId, merchantAccountConfigId);
                    break;
            }
        }

        private void RefreshOwnerHostConfig()
        {
            plcOwnerHostConfigs.Visible = true;
            txtUrlHost.Text = string.Empty;
            txtHostConfigOwnerId.Text = string.Empty;
            txtCssFileName.Text = string.Empty;
            txtCompanyLogo.Text = string.Empty;
            txtContactEmail.Text = string.Empty;

            grdvwOwnerHostConfigList.DataSource = CommonConfig.OwnerHostConfigEntries;
            grdvwOwnerHostConfigList.DataBind();
        }

        protected void btnAddOwnerHostConfig_Click(object sender, EventArgs e)
        {
            OwnerHostConfig ownerHostConfig = new OwnerHostConfig();
            ownerHostConfig.OwnerId = Util.ConvertInt32(txtHostConfigOwnerId.Text.Trim());
            ownerHostConfig.UrlHost = txtUrlHost.Text.Trim();
            ownerHostConfig.CssFileName = txtCssFileName.Text.Trim();
            ownerHostConfig.CompanyLogo = txtCompanyLogo.Text.Trim();
            ownerHostConfig.ContactEmail = txtContactEmail.Text.Trim();

            LoginController loginCntrl = new LoginController();

            ValidationResults errors = loginCntrl.AddNewOwnerHostConfig(CurrentUser, ownerHostConfig);

            if (errors.IsValid)
            {
                RefreshOwnerHostConfig();
                Master.DisplayFriendlyMessage("Owner Config Saved.");
            }
            else
            {
                Master.AddErrorMessages(errors);
            }
        }

        protected void grdvwOwnerHostConfigList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

            }
        }

        protected void grdvwOwnerHostConfigList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "DeleteOwnerHostConfig":
                    LoginController cntrl = new LoginController();
                    cntrl.DeleteOwnerHostConfig(CurrentUser, e.CommandArgument.ToString());
                    RefreshOwnerHostConfig();
                    break;

            }
        }

        private int SelectedOwnerId
        {
            get
            {
                return Util.ConvertInt32(hdnOwnerId.Value);
            }
            set
            {
                hdnOwnerId.Value = value.ToString();
            }
        }

        private int SelectedMerchantAccountId
        {
            get
            {
                return Util.ConvertInt32(hdnMerchantAccountConfigId.Value);
            }
            set
            {
                hdnMerchantAccountConfigId.Value = value.ToString();
            }
        }

    }
}