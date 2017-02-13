

#region Using Statements
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
using ExpoOrders.Web.Owners.Common;
using System.Data;
using System.IO;
using System.Web.UI.HtmlControls;

#endregion

namespace ExpoOrders.Web.Owners
{

    public partial class Products : BaseOwnerPage
    {
        
        

        #region singleton

        private ProductController _cntrl = null;
        private ProductController Cntrl
        {
            get
            {
                if (_cntrl == null)
                {
                    _cntrl = new ProductController();
                }
                return _cntrl;
            }
        }
        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {

            this.Master.NavigationItemCallBack = this.HandleNavigationItemClicked;
            this.Master.PreviewShowCallBack = this.PreviewShow;

            this.ucFormQuestionEditorList.ItemSelected = this.AdditionalInfoQuestionSelected;
            this.ucFormQuestionEditorList.ItemDeleted = this.AdditionalInfoQuestionDeleted;


            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            List<NavigationLink> _pageNavigationLinks = OwnerUtil.BuildShowAdminNavLinks(CurrentUser, CurrentUser.CurrentShow.ShowId);
            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, OwnerPage.Products, OwnerTabEnum.ShowDetail);
            this.Master.LoadSubNavigation("Show Admin", _pageNavigationLinks);
            LoadPageMode();
            LoadCategoryList();
        }

        private void LoadPageMode()
        {
            if (this.Master.CurrentSubNavigationLinks != null && this.Master.CurrentSubNavigationLinks.Count > 0)
            {
                NavigationLink linkToSelect = this.Master.CurrentSubNavigationLinks.SingleOrDefault<NavigationLink>(l => l.TargetId == (int)ShowSettingPageMode.Products);
                this.Master.SelectNavigationItem(linkToSelect.NavigationLinkId);
                LoadPageMode(linkToSelect.NavigationLinkId, (int)linkToSelect.TargetId);
            }
        }

        private void LoadPageMode(int navLinkId, int targetId)
        {
            ShowSettingPageMode currentPageMode = (ShowSettingPageMode)Enum.Parse(typeof(ShowSettingPageMode), targetId.ToString(), true);

            switch (currentPageMode)
            {
                case ShowSettingPageMode.Products:

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
                default:
                    Server.Transfer(string.Format("{0}?targetId={1}", OwnerPage.ShowSettings.GetCodeValue(), (int)currentPageMode));
                    break;
            }
        }

        private void LoadCategoryList()
        {
            txtDefaultSubmissionDeadline.Text = string.Empty;
            txtDefaultDiscountDeadline.Text = string.Empty;
            txtDefaultEarlyBirdDeadline.Text = string.Empty;
            btnToggleAdvancedPricingCountdown.Text = string.Empty;
            
            txtDefaultLateFeeDeadline.Text = string.Empty;
            txtDefaultSalesTax.Text = string.Empty;

            txtCategorySubmissionDeadline.Text = string.Empty;
            txtCategoryDiscountDeadline.Text = string.Empty;
            txtCategoryEarlyBirdDeadline.Text = string.Empty;
            txtCategoryLateFeeDeadline.Text = string.Empty;
            txtCategorySalesTax.Text = string.Empty;

            if (CurrentUser.CurrentShow.DefaultSubmissionDeadline.HasValue)
            {
                txtDefaultSubmissionDeadline.Text = 
                    txtCategorySubmissionDeadline.Text = CurrentUser.CurrentShow.DefaultSubmissionDeadline.Value.ToShortDateString();
            }

            if (CurrentUser.CurrentShow.DefaultDiscountDeadline.HasValue)
            {
                txtDefaultDiscountDeadline.Text = 
                    txtCategoryDiscountDeadline.Text = CurrentUser.CurrentShow.DefaultDiscountDeadline.Value.ToShortDateString();
            }

            if (CurrentUser.CurrentShow.DefaultEarlyBirdDeadline.HasValue)
            {
                txtDefaultEarlyBirdDeadline.Text = 
                    txtCategoryEarlyBirdDeadline.Text = CurrentUser.CurrentShow.DefaultEarlyBirdDeadline.Value.ToShortDateString();
            }

            if (CurrentUser.CurrentShow.DisplayAdvancedPricingCountDown.HasValue && CurrentUser.CurrentShow.DisplayAdvancedPricingCountDown.Value == true)
            {
                PaintAdvancedPricingCountDownControls(true);
            }
            else
            {
                PaintAdvancedPricingCountDownControls(false);
            }

            if (CurrentUser.CurrentShow.DefaultLateFeeDeadline.HasValue)
            {
                txtDefaultLateFeeDeadline.Text = 
                    txtCategoryLateFeeDeadline.Text = CurrentUser.CurrentShow.DefaultLateFeeDeadline.Value.ToShortDateString();
            }

            if (CurrentUser.CurrentShow.DefaultSalesTax.HasValue)
            {
                txtDefaultSalesTax.Text = 
                    txtCategorySalesTax.Text = CurrentUser.CurrentShow.DefaultSalesTaxDisplayFormat;
            }

            plcCategories.Visible = true;
            plcCategoryDetail.Visible = false;
            plcProductList.Visible = false;
            plcProductDetail.Visible = false;

            ltrCategoryMode.Text = "Category Listing";

            bool displayInactive = chkIncludeInactive.Checked;
            List<Category> categoryList = categoryList = Cntrl.GetCategoryList(CurrentUser.CurrentShow.ShowId);
            if (!displayInactive)
            {
                categoryList = categoryList.Where(cat => cat.ActiveFlag == true).ToList();
            }

            grdvwCategoryList.DataSource = categoryList;
            grdvwCategoryList.DataBind();
        }

        private void PaintAdvancedPricingCountDownControls(bool enabled)
        {
            const string BTN_DISABLE_ADVANCED_COUNTDOWN = "Hide it";
            const string MSG_ADVANCED_COUNTDOWN_DISABLED = "Countdown is currently hidden.";

            const string BTN_ENABLE_ADVANCED_COUNTDOWN = "Show it";
            const string MSG_ADVANCED_COUNTDOWN_ENABLED = "Countdown is currently displayed.";

            if (enabled)
            {
                ltrAdvancedPricingCountDownStatus.Text = MSG_ADVANCED_COUNTDOWN_ENABLED;
                btnToggleAdvancedPricingCountdown.Text = BTN_DISABLE_ADVANCED_COUNTDOWN;
            }
            else
            {
                ltrAdvancedPricingCountDownStatus.Text = MSG_ADVANCED_COUNTDOWN_DISABLED;
                btnToggleAdvancedPricingCountdown.Text = BTN_ENABLE_ADVANCED_COUNTDOWN;
            }
        }

        private void LoadCategoryDetail(int categoryId)
        {
            plcCategories.Visible = false;
            plcCategoryDetail.Visible = true;
            plcProductList.Visible = false;
            plcProductDetail.Visible = false;

            lblCategoryId.Text = string.Empty;
            txtCategoryName.Text = string.Empty;
            txtCategorySortOrder.Text = string.Empty;
            hdnCategoryIconFileName.Value = string.Empty;
            plcCategoryPricingUpdate.Visible = false;

            CurrentCategoryId = categoryId;

            plcSaveNewCategory.Visible = false;

            plcProductList.Visible = false;

            if (categoryId > 0)
            {
                ltrCategoryMode.Text = "Category Detail";

                Category category = Cntrl.GetCategory(categoryId);

                if (category != null)
                {
                    lblCategoryId.Text = category.CategoryId.ToString();
                    plcCategoryPricingUpdate.Visible = true;
                    lnkCategoryPricingUpdate.Attributes.Add("onClick", "launchCascadingPriceUpdateDialog(" + category.CategoryId.ToString() + "); return false;");
                    txtCategoryName.Text = category.CategoryName;
                    txtCategorySortOrder.Text = category.SortOrder.ToString();
                    PaintCategoryIconFileNameControls(category.IconFileName);
                    plcProductList.Visible = true;

                    grdvwProductList.DataSource = category.Products.Where(prod => prod.ActiveFlag == true).OrderBy(prod => prod.SortOrder);
                    grdvwProductList.DataBind();
                }
            }
            else
            {
                plcSaveNewCategory.Visible = true;
                ltrCategoryMode.Text = "New Category";
                plcProductList.Visible = false;
                txtCategorySortOrder.Text = "1";
            }
        }

        private void PaintCategoryIconFileNameControls(string categoryIconFileName)
        {

            if (!string.IsNullOrEmpty(categoryIconFileName))
            {
                lnkCategoryIconFile.HRef = string.Format("~/Assets/Shows/{0}/{1}", CurrentUser.CurrentShow.ShowGuid, categoryIconFileName);
                lblCategoryIconFileName.Text = categoryIconFileName;
                lnkCategoryIconFile.Visible = lblCategoryIconFileName.Visible = true;
                btnRemoveCategoryIconFile.Visible = true;
            }
            else
            {
                btnRemoveCategoryIconFile.Visible = false;
                lnkCategoryIconFile.Visible = lblCategoryIconFileName.Visible = false;
            }

            hdnCategoryIconFileName.Value = categoryIconFileName;
        }

        private void UpdateCategoryActiveFlag(int categoryId, bool activeFlag)
        {
            ValidationResults errors = Cntrl.UpdateCategoryActiveFlag(CurrentUser, CurrentUser.CurrentShow.ShowId, categoryId, activeFlag);

            if (!errors.IsValid)
            {
                PageErrors.AddErrorMessages(errors, PageErrors.ValidationGroup);
            }
            else
            {
                LoadCategoryList();
            }
        }

        private void LoadProductDetail(int categoryId, int productId, bool loadingNewProduct)
        {
            btnCancelAddAdditionalQuestion.Visible = false;

            lblShowCurrencySymbol.Text = lblShowCurrencySymbol2.Text = lblShowCurrencySymbol3.Text = CurrentUser.CurrentShow.CurrencySymbol;

            ucFormQuestionEditorList.Visible = false;
            ucFormQuestionEditor.ClearQuestion();

            plcProductDetail.Visible = true;
            plcCategoryDetail.Visible = false;
            plcCategories.Visible = false;
            plcProductList.Visible = false;

            plcFileProductTypeControls.Visible = plcItemProductTypeControls.Visible = false;

            ResetProductDetailControls();

            CurrentCategoryId = categoryId;
            CurrentProductId = productId;

            hdnAdditionalInfoFormId.Value = string.Empty;

            this.ltrQuestionEditorLabel.Text = "Add New Additional Info Question";

            this.btnAddAdditionalInfoQuestion.Text = "Add Question";

            PopulateProductTypes();

            lnkRemoveProductImage.Visible = false;

            lnkRemoveAdditionalImage1.Visible = false;
            lnkRemoveAdditionalImage2.Visible = false;
            lnkRemoveAdditionalImage3.Visible = false;
            lnkRemoveAdditionalImage4.Visible = false;
            lnkRemoveAdditionalImage5.Visible = false;

            lnkFileDownload.Visible = false;
            lblFileDownloadFileName.Text = string.Empty;

            rdoLstProductBundle.ClearSelection();

            if (productId > 0)
            {
                ltrCategoryMode.Text = "Edit Product/Service";
                plcNewProductSaveControls.Visible = false;
                plcProductExistingProductDetails.Visible = true;
                PaintExistingProductDetails(productId, loadingNewProduct);
            }
            else
            {
                PopulateLateBehaviors();
                txtProductSortOrder.Text = (grdvwProductList.Rows.Count + 1).ToString();
                ltrCategoryMode.Text = "Add New Product/Service";
                plcNewProductSaveControls.Visible = true;
                plcProductExistingProductDetails.Visible = false;

                if (CurrentUser.CurrentShow.DefaultSubmissionDeadline.HasValue)
                {
                    this.txtSubmissionDeadline.Text = CurrentUser.CurrentShow.DefaultSubmissionDeadline.Value.ToShortDateString();
                }
                else
                {
                    txtSubmissionDeadline.Text = string.Empty;
                }
            }
        }

        private void ResetProductDetailControls()
        {
            CurrentProductId = 0;
            lblProductId.Text = string.Empty;
            txtProductName.Text = string.Empty;
            txtProductDescription.Content = string.Empty;
            txtProductSortOrder.Text = "1";

            txtSubmissionDeadline.Text = string.Empty;
            txtLateMessage.Text = string.Empty;

            chkTaxExempt.Checked = false;
            chkVisibleToExhibitors.Checked = true;

        }

        private void PaintExistingProductDetails(int productId, bool loadingNewProduct)
        {
            Product productDetail = Cntrl.GetProductById(productId);

            PopulateAdditionalChargeTypes();
            PopulateLateFeeTypes();
            PopulateLateBehaviors();

            lnkFileDownload.Visible = false;
            lblFileDownloadFileName.Text = string.Empty;

            txtEarlyBirdPrice.Text = string.Empty;
            txtEarlyBirdDeadline.Text = string.Empty;

            txtLateFeeDeadline.Text = string.Empty;

            rdoBtnLstInstallDismantleInd.ClearSelection();

            rdoLstProductBundle.ClearSelection();
            plcProductAssociations.Visible = false;

            btnSaveProduct.Visible = true;

            if (productDetail != null)
            {
                lblProductId.Text = productId.ToString();
                if (productDetail.SubmissionDeadline.HasValue)
                {
                    txtSubmissionDeadline.Text = productDetail.SubmissionDeadline.Value.ToShortDateString();
                }
                else
                {
                    txtSubmissionDeadline.Text = string.Empty;
                }

                WebUtil.SelectListItemByValue(ddlLateBehavior, productDetail.LateBehaviorCd);
                txtLateMessage.Text = productDetail.LateMessage;

                if (productDetail.AdditionalInfoFormId != null)
                {
                    hdnAdditionalInfoFormId.Value = productDetail.AdditionalInfoFormId.Value.ToString();
                }

                WebUtil.SelectListItemByValue(ddlProductType, productDetail.ProductTypeCd);

                txtProductName.Text = productDetail.ProductName;
                txtProductDescription.Content = productDetail.ProductDescription;
                ConfigureImageManager(txtProductDescription, CurrentUser.CurrentShow.ShowGuid, true, true);

                txtProductSortOrder.Text = productDetail.SortOrder.ToString();

                if (productDetail.ProductTypeCd == ProductTypeEnum.FileDownload.ToString())
                {
                    plcFileProductTypeControls.Visible = true;
                    if (productDetail.FileDownloadId.HasValue)
                    {
                        hdnFileDownLoadId.Value = productDetail.FileDownloadId.Value.ToString();
                        FileDownloadController fileController = new FileDownloadController();
                        FileDownload file = fileController.GetFileDownloadById(productDetail.FileDownloadId.Value);
                        if (file != null)
                        {
                            lblFileDownloadFileName.Text = file.FileName;
                            lnkFileDownload.Visible = true;
                            lnkFileDownload.HRef = string.Format("~/Assets/Shows/{0}/Downloads/{1}", CurrentUser.CurrentShow.ShowGuid, file.FileName);
                        }
                    }
                    else
                    {
                        hdnFileDownLoadId.Value = string.Empty;
                    }
                }
                else if (productDetail.ProductTypeCd == ProductTypeEnum.Item.ToString())
                {
                    plcItemProductTypeControls.Visible = true;
                    txtProductSku.Text = productDetail.ProductSku;


                    if (string.IsNullOrEmpty(productDetail.ImageName))
                    {
                        imgProductImage.Visible = false;
                    }
                    else
                    {
                        lnkRemoveProductImage.Visible = true;
                        imgProductImage.Visible = true;
                        imgProductImage.ImageUrl = imgProductLink.HRef = string.Format("~/Assets/Shows/{0}/Products/{1}", CurrentUser.CurrentShow.ShowGuid, productDetail.ImageName);
                        imgProductImage.AlternateText = productDetail.ProductName;
                    }

                    lblImageName.Text = productDetail.ImageName;

                    ConfigureAdditionalImageDisplay(productDetail, 1);
                    ConfigureAdditionalImageDisplay(productDetail, 2);
                    ConfigureAdditionalImageDisplay(productDetail, 3);
                    ConfigureAdditionalImageDisplay(productDetail, 4);
                    ConfigureAdditionalImageDisplay(productDetail, 5);


                    if (productDetail.DiscountDeadline.HasValue)
                    {
                        txtDiscountDeadline.Text = productDetail.DiscountDeadline.Value.ToShortDateString();
                    }
                    else
                    {
                        if (loadingNewProduct)
                        {
                            if (CurrentUser.CurrentShow.DefaultDiscountDeadline.HasValue)
                            {
                                txtDiscountDeadline.Text = CurrentUser.CurrentShow.DefaultDiscountDeadline.Value.ToShortDateString();
                            }
                            else
                            {
                                txtDiscountDeadline.Text = string.Empty;
                            }
                        }
                        else
                        {
                            txtDiscountDeadline.Text = string.Empty;
                        }
                    }

                    txtUnitDescriptor.Text = productDetail.UnitDescriptor;
                    txtQuantityLabel.Text = productDetail.QuantityLabel;
                    if (productDetail.MinimumQuantityRequired.HasValue)
                    {
                        txtMinimumQuantityRequired.Text = productDetail.MinimumQuantityRequired.Value.ToString();
                    }
                    else
                    {
                        txtMinimumQuantityRequired.Text = "0";
                    }

                    chkVisibleToExhibitors.Checked = productDetail.VisibleToExhibitors;

                    if (productDetail.UnitPrice.HasValue)
                    {
                        txtUnitPrice.Text = productDetail.UnitPriceDisplayFormat;
                    }
                    else
                    {
                        txtUnitPrice.Text = string.Empty;
                    }

                    if (productDetail.DiscountUnitPrice.HasValue)
                    {
                        txtDiscountUnitPrice.Text = productDetail.DiscountUnitPriceDisplayFormat;
                    }
                    else
                    {
                        txtDiscountUnitPrice.Text = string.Empty;
                    }

                    if (productDetail.EarlyBirdPrice.HasValue)
                    {
                        txtEarlyBirdPrice.Text = productDetail.EarlyBirdPriceDisplayFormat;
                        if (productDetail.EarlyBirdDeadline.HasValue)
                        {
                            txtEarlyBirdDeadline.Text = productDetail.EarlyBirdDeadline.Value.ToShortDateString();
                        }
                    }
                    else
                    {
                        txtEarlyBirdPrice.Text = string.Empty;
                        txtEarlyBirdDeadline.Text = string.Empty;
                    }

                   

                    if (productDetail.SalesTaxPercent.HasValue)
                    {
                        txtSalesTax.Text = productDetail.SalesTaxDisplayFormat;
                    }
                    else
                    {
                        if (loadingNewProduct)
                        {
                            txtSalesTax.Text = CurrentUser.CurrentShow.DefaultSalesTaxDisplayFormat;
                        }
                        else
                        {
                            txtSalesTax.Text = string.Empty;
                        }
                    }

                    if (productDetail.LateFeeDeadline.HasValue)
                    {
                        txtLateFeeDeadline.Text = productDetail.LateFeeDeadline.Value.ToShortDateString();
                    }
                    else
                    {
                        txtLateFeeDeadline.Text = string.Empty;
                    }

                    WebUtil.SelectListItemByValue(ddlLateFeeType, productDetail.LateFeeType);
                    if (productDetail.LateFeeAmount.HasValue)
                    {
                        txtLateFeeAmount.Text = productDetail.LateFeeAmountDisplayFormat;
                    }
                    else
                    {
                        txtLateFeeAmount.Text = string.Empty;
                    }


                    WebUtil.SelectListItemByValue(ddlAdditionalChargeType, productDetail.AdditionalChargeType);
                    if (productDetail.AdditionalChargeAmount.HasValue)
                    {
                        txtAdditionalCharge.Text = productDetail.AdditionalChargeAmountDisplayFormat;
                    }
                    else
                    {
                        txtAdditionalCharge.Text = string.Empty;
                    }

                    txtRequiredAttributeLabel1.Text = productDetail.RequiredAttribute1Label;
                    txtRequiredAttributeChoiceList1.Text = productDetail.RequiredAttribute1ChoiceList;

                    txtRequiredAttributeLabel2.Text = productDetail.RequiredAttribute2Label;
                    txtRequiredAttributeChoiceList2.Text = productDetail.RequiredAttribute2ChoiceList;

                    txtRequiredAttributeLabel3.Text = productDetail.RequiredAttribute3Label;
                    txtRequiredAttributeChoiceList3.Text = productDetail.RequiredAttribute3ChoiceList;

                    txtRequiredAttributeLabel4.Text = productDetail.RequiredAttribute4Label;
                    txtRequiredAttributeChoiceList4.Text = productDetail.RequiredAttribute4ChoiceList;

                    if (productDetail.CalcQuantityFromAttributesFlag.HasValue && productDetail.CalcQuantityFromAttributesFlag.Value == true)
                    {
                        rdoLstCalcQuantityFromAttributes.SelectedValue = "1";
                    }
                    else
                    {
                        rdoLstCalcQuantityFromAttributes.SelectedValue = "0";
                    }

                    if (!string.IsNullOrEmpty(productDetail.InstallDismantleInd))
                    {
                        ListItem li = rdoBtnLstInstallDismantleInd.Items.FindByValue(productDetail.InstallDismantleInd);
                        if (li != null)
                        {
                            li.Selected = true;
                        }
                    }


                    chkTaxExempt.Checked = (productDetail.TaxExemptFlag != null && productDetail.TaxExemptFlag.Value == true);

                    if (productDetail.AdditionalInfoFormId.HasValue)
                    {
                        this.LoadAdditionalInfoQuestions(productDetail.AdditionalInfoFormId.Value);
                    }

                    ucFormQuestionEditor.QuestionSortOrder = this.ucFormQuestionEditorList.QuestionCount + 1;

                    if (productDetail.IsBundle.HasValue && productDetail.IsBundle.Value == true)
                    {
                        WebUtil.SelectListItemByValue(rdoLstProductBundle, "Y");

                        plcProductAssociations.Visible = true;
                        LoadAssociatedProducts(productDetail.ProductAssociations);
                        LoadAssociatedProductsToAdd(CurrentProductId);
                    }
                    else
                    {
                        WebUtil.SelectListItemByValue(rdoLstProductBundle, "N");
                    }
                }
            }
        }

        private void ConfigureAdditionalImageDisplay(Product productDetail, int imageNbr)
        {
            string imageName = string.Empty;
            Label lblAdditionalImageName = null;
            Image imgAdditionalImage = null;
            HtmlAnchor imgAdditionalImageLink = null;
            LinkButton lnkRemoveAdditionalImage = null;

            switch (imageNbr)
            {
                case 1:
                    imageName = productDetail.AdditionalImageName1;
                    lblAdditionalImageName = lblAdditionalImageName1;
                    imgAdditionalImage = imgAdditionalImage1;
                    imgAdditionalImageLink = imgAdditionalImageLink1;
                    lnkRemoveAdditionalImage = lnkRemoveAdditionalImage1;
                    break;
                case 2:
                    imageName = productDetail.AdditionalImageName2;
                    lblAdditionalImageName = lblAdditionalImageName2;
                    imgAdditionalImage = imgAdditionalImage2;
                    imgAdditionalImageLink = imgAdditionalImageLink2;
                    lnkRemoveAdditionalImage = lnkRemoveAdditionalImage2;
                    break;
                case 3:
                    imageName = productDetail.AdditionalImageName3;
                    lblAdditionalImageName = lblAdditionalImageName3;
                    imgAdditionalImage = imgAdditionalImage3;
                    imgAdditionalImageLink = imgAdditionalImageLink3;
                    lnkRemoveAdditionalImage = lnkRemoveAdditionalImage3;
                    break;
                case 4:
                    imageName = productDetail.AdditionalImageName4;
                    lblAdditionalImageName = lblAdditionalImageName4;
                    imgAdditionalImage = imgAdditionalImage4;
                    imgAdditionalImageLink = imgAdditionalImageLink4;
                    lnkRemoveAdditionalImage = lnkRemoveAdditionalImage4;
                    break;
                case 5:
                    imageName = productDetail.AdditionalImageName5;
                    lblAdditionalImageName = lblAdditionalImageName5;
                    imgAdditionalImage = imgAdditionalImage5;
                    imgAdditionalImageLink = imgAdditionalImageLink5;
                    lnkRemoveAdditionalImage = lnkRemoveAdditionalImage5;
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(imageName))
            {

                imgAdditionalImage.Visible = false;
            }
            else
            {
                lnkRemoveAdditionalImage.Visible = true;
                imgAdditionalImage.Visible = true;
                imgAdditionalImage.ImageUrl = imgAdditionalImageLink.HRef = string.Format("~/Assets/Shows/{0}/Products/{1}", CurrentUser.CurrentShow.ShowGuid, imageName);
                imgAdditionalImage.AlternateText = imageName;
            }
            
            lblAdditionalImageName.Text = imageName;
        }

        private void PopulateLateFeeTypes()
        {
            ddlLateFeeType.Items.Clear();

            ddlLateFeeType.Items.Add(new ListItem(string.Empty, string.Empty));
            ddlLateFeeType.Items.Add(new ListItem("Flat Fee", LateFeeTypeEnum.Flat.ToString()));
            ddlLateFeeType.Items.Add(new ListItem("% of Order", LateFeeTypeEnum.PctTotal.ToString()));
            ddlLateFeeType.Items.Add(new ListItem("Per Unit Fee", LateFeeTypeEnum.PerUnit.ToString()));
        }

        private void PopulateAdditionalChargeTypes()
        {
            ddlAdditionalChargeType.Items.Clear();

            ddlAdditionalChargeType.Items.Add(new ListItem(string.Empty, string.Empty));
            ddlAdditionalChargeType.Items.Add(new ListItem("Flat Fee", AdditionalChargeTypeEnum.Flat.ToString()));
            ddlAdditionalChargeType.Items.Add(new ListItem("% of Order", AdditionalChargeTypeEnum.PctTotal.ToString()));
            ddlAdditionalChargeType.Items.Add(new ListItem("Per Unit Fee", AdditionalChargeTypeEnum.PerUnit.ToString()));
        }

        private void PopulateProductTypes()
        {
            ddlProductType.Items.Clear();

            ddlProductType.Items.Add(new ListItem(string.Empty));
            ddlProductType.Items.Add(new ListItem("Product/Service", ProductTypeEnum.Item.ToString()));
            ddlProductType.Items.Add(new ListItem("Vendor Form", ProductTypeEnum.FileDownload.ToString()));
            ddlProductType.Items.Add(new ListItem("Section Heading", ProductTypeEnum.SectionHeading.ToString()));

        }

        private void PopulateLateBehaviors()
        {
            ddlLateBehavior.Items.Clear();

            ddlLateBehavior.Items.Add(new ListItem("Allow Orders", LateSubmissionBehaviorEnum.Allow.ToString()));
            ddlLateBehavior.Items.Add(new ListItem("Warning Message", LateSubmissionBehaviorEnum.Warning.ToString()));
            ddlLateBehavior.Items.Add(new ListItem("Prevent Orders", LateSubmissionBehaviorEnum.Prevent.ToString()));
        }

        private void LoadAdditionalInfoQuestions(int additionalInfoFormId)
        {
            FormController formCtrl = new FormController();
            Form additionalInfoForm = formCtrl.GetFormInfoById(additionalInfoFormId);
            if (additionalInfoForm != null)
            {
                ucFormQuestionEditorList.Populate(additionalInfoForm.Questions.OrderBy(q => q.SortOrder).ToList());
            }
        }

        private void DeleteProduct(int categoryId, int productId)
        {
            ValidationResults errors = Cntrl.DeleteProduct(productId);

            if (!errors.IsValid)
            {
                PageErrors.AddErrorMessages(errors, PageErrors.ValidationGroup);
            }
            else
            {
                this.Master.DisplayFriendlyMessage("Product Deleted");
                LoadCategoryDetail(categoryId);
            }
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            LoadPageMode(navLinkId, targetId);
        }

        #endregion

        #region Control Events

        protected void btnToggleAdvancedPricingCountdown_Click(object sender, EventArgs e)
        {
            bool newToggleValue = Cntrl.ToggleAdvancedPricingCountdown(CurrentUser, CurrentUser.CurrentShow);

            string msg = "Countdown Ticker has been disabled.";
            if (newToggleValue == true)
            {
                PaintAdvancedPricingCountDownControls(true);
                msg = "Countdown Ticker has been enabled.";
            }
            else
            {
                PaintAdvancedPricingCountDownControls(false);
                msg = "Countdown Ticker has been disabled.";
            }
            this.Master.DisplayFriendlyMessage(msg);
        }

        protected void grdvwCategoryList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int categoryId = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "EditCategory":
                    LoadCategoryDetail(categoryId);
                    break;
                case "DeleteCategory":
                    UpdateCategoryActiveFlag(categoryId, false);
                    break;
                case "RestoreCategory":
                    UpdateCategoryActiveFlag(categoryId, true);
                    break;
                default:
                    throw new Exception(e.CommandName + " " + e.CommandArgument);
            }
        }

        protected void grdvwProductList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int productId = Util.ConvertInt32(e.CommandArgument);
            int categoryId = CurrentCategoryId;

            switch (e.CommandName)
            {
                case "EditProduct":
                    LoadProductDetail(categoryId, productId, false);
                    break;
                case "DeleteProduct":
                    DeleteProduct(categoryId, productId);
                    break;
            }
        }

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            LoadCategoryDetail(0);
        }

        protected void btnCancelCategoryDetail_Click(object sender, EventArgs e)
        {
            CurrentCategoryId = 0;
            CurrentProductId = 0;
            hdnAdditionalInfoFormId.Value = string.Empty;
            hdnFileDownLoadId.Value = string.Empty;

            LoadPage();
        }

        protected void btnRefreshCategoryPricing_Click(object sender, EventArgs e)
        {
            LoadCategoryDetail(CurrentCategoryId);
        }


        protected void btnSaveNewCategory_Click(object sender, EventArgs e)
        {
            Category newCategory = new Category();
            newCategory.ActiveFlag = true;
            newCategory.ShowId = CurrentUser.CurrentShow.ShowId;
            newCategory.CategoryName = txtCategoryName.Text.Trim();
            newCategory.SortOrder = Util.ConvertInt32(txtCategorySortOrder.Text.Trim());
            newCategory.IconFileName = SaveCategoryIconFile();

            ValidationResults errors = Cntrl.SaveNewCategory(newCategory);

            if (errors.IsValid)
            {
                LoadCategoryDetail(newCategory.CategoryId);
            }
            else
            {
                PageErrors.AddErrorMessages(errors, PageErrors.ValidationGroup);
            }
        }

        private string SaveCategoryIconFile()
        {
            string categoryIconFilename = null;
            if (fupUploadCategoryIcon.HasFile)
            {
                hdnCategoryIconFileName.Value = SaveFile(fupUploadCategoryIcon);
                categoryIconFilename = hdnCategoryIconFileName.Value;
                PaintCategoryIconFileNameControls(categoryIconFilename);
            }
            else
            {
                categoryIconFilename = hdnCategoryIconFileName.Value;
            }
            return categoryIconFilename;
        }

        protected void btnCancelSaveNewCategory_Click(object sender, EventArgs e)
        {
            LoadPage();
        }

        protected void btnRemoveCategoryIconFile_Click(object sender, EventArgs e)
        {
            int categoryId = CurrentCategoryId;

            Cntrl.RemoveCategoryIconFile(CurrentUser, CurrentUser.CurrentShow, categoryId);
            LoadCategoryDetail(categoryId);
        }

        protected void btnSaveCategoryPricing_Click(object sender, EventArgs e)
        {
            int categoryId = CurrentCategoryId;
            List<ProductItemDetail> prodList = new List<ProductItemDetail>();

            Category category = new Category();
            category.CategoryId = categoryId;
            category.CategoryName = txtCategoryName.Text.Trim();
            category.SortOrder = Util.ConvertInt32(txtCategorySortOrder.Text.Trim());
            category.IconFileName = SaveCategoryIconFile();

            if (categoryId > 0 && grdvwProductList.Visible)
            {
                //Build list of Product Prices from the list
                foreach (GridViewRow row in grdvwProductList.Rows)
                {
                    TextBox txtListItemProductPrice = (TextBox)row.FindControl("txtListItemProductPrice");
                    TextBox txtListItemProductDiscountPrice = (TextBox)row.FindControl("txtListItemProductDiscountPrice");
                    TextBox txtListItemProductEarlyBirdPrice = (TextBox)row.FindControl("txtListItemProductEarlyBirdPrice");
                    TextBox txtListItemSalesTax = (TextBox)row.FindControl("txtListItemSalesTax");
                    TextBox txtListItemProductSortOrder = (TextBox)row.FindControl("txtListItemProductSortOrder");

                    HiddenField hdnProductId = (HiddenField)row.FindControl("hdnProductId");
                    HiddenField hdnProductTypeCd = (HiddenField)row.FindControl("hdnProductTypeCd");

                    ProductTypeEnum productType = Enum<ProductTypeEnum>.Parse(hdnProductTypeCd.Value, true);

                    if (txtListItemProductPrice != null && hdnProductId != null && hdnProductTypeCd != null)
                    {
                        ProductItemDetail prodItem = new ProductItemDetail();
                        prodItem.ProductId = Util.ConvertInt32(hdnProductId.Value);
                        prodItem.SortOrder = Util.ConvertInt32(txtListItemProductSortOrder.Text.Trim());
                        prodItem.ProductType = productType;

                        if (productType == ProductTypeEnum.Item)
                        {
                            prodItem.Price = Util.ConvertNullDecimal(txtListItemProductPrice.Text.Trim());

                            prodItem.DiscountPrice = Util.ConvertNullDecimal(txtListItemProductDiscountPrice.Text.Trim());
                            prodItem.EarlyBirdPrice = Util.ConvertNullDecimal(txtListItemProductEarlyBirdPrice.Text.Trim());

                            if (txtListItemSalesTax.Enabled)
                            {
                                prodItem.SalesTax = ParsePercentageValue(txtListItemSalesTax);
                            }
                            else
                            {
                                prodItem.SalesTax = null;
                            }
                        }

                        prodList.Add(prodItem);
                    }
                }
            }

            ValidationResults errors = Cntrl.SaveCategoryProductList(CurrentUser, category, prodList);

            if (errors.IsValid)
            {
                LoadCategoryDetail(categoryId);
                this.Master.DisplayFriendlyMessage("Category Detail has been saved.");
            }
            else
            {
                this.PageErrors.AddErrorMessages(errors);
            }
        }

        private double? ParsePercentageValue(TextBox txtControl)
        {
            double? val = null;

            if (txtControl.Text.Trim().Length > 0)
            {
                val = Util.ConvertNullDouble(txtControl.Text.Trim()) / 100;
            }

            return val;
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            LoadProductDetail(CurrentCategoryId, 0, true);
        }


        protected void btnUploadFileDownload_Click(object sender, EventArgs e)
        {
            int categoryId = CurrentCategoryId;
            int productId = CurrentProductId;
            ValidationResults errors = new ValidationResults();
            if (this.fupFileDownload.HasFile)
            {
                string fileName = Util.CleanFileName(fupFileDownload.FileName);
                string filePath = Server.MapPath(string.Format("~/Assets/Shows/{0}/Downloads/{1}", CurrentUser.CurrentShow.ShowGuid, fileName));

                fupFileDownload.SaveAs(filePath);

                FileDownload file = Cntrl.CreateFileDownload(CurrentUser, productId, fileName);

                hdnFileDownLoadId.Value = file.FileDownloadId.ToString();
            }
            else
            {
                errors.AddResult(new ValidationResult("Must choose a File for uploading.", null, null, null, null));
            }

            if (errors.IsValid)
            {
                this.Master.DisplayFriendlyMessage("File Uploaded.");
                this.LoadProductDetail(categoryId, productId, false);
            }
            else
            {
                PageErrors.AddErrorMessages(errors, PageErrors.ValidationGroup);
            }
        }

        private void UploadAndSaveImages()
        {
            if (SaveProductDetail(false))
            {
                int categoryId = CurrentCategoryId;
                int productId = CurrentProductId;

                DoUpload(categoryId, productId, 0);
                DoUpload(categoryId, productId, 1);
                DoUpload(categoryId, productId, 2);
                DoUpload(categoryId, productId, 3);
                DoUpload(categoryId, productId, 4);
                DoUpload(categoryId, productId, 5);

                this.Master.DisplayFriendlyMessage("Image(s) Uploaded.");
                this.LoadProductDetail(categoryId, productId, false);

            }
        }

        private void DoUpload(int categoryId, int productId, int additionalImageNbr)
        {
            FileUpload fup = null;

            switch (additionalImageNbr)
            {
                case 0:
                    fup = fupProductImage;
                    break;
                case 1:
                    fup = fupAdditionalImage1;
                    break;
                case 2:
                    fup = fupAdditionalImage2;
                    break;
                case 3:
                    fup = fupAdditionalImage3;
                    break;
                case 4:
                    fup = fupAdditionalImage4;
                    break;
                case 5:
                    fup = fupAdditionalImage5;
                    break;
                default:
                    break;
            }

            if (fup.HasFile)
            {
                string fileName = Util.CleanFileName(fup.FileName);
                string filePath = Server.MapPath(string.Format("~/Assets/Shows/{0}/Products/{1}", CurrentUser.CurrentShow.ShowGuid, fileName));

                fup.SaveAs(filePath);

                Cntrl.UpdateProductImage(productId, fileName, additionalImageNbr);
            }
           
        }

        protected void btnUploadAllImages_Click(object sender, EventArgs e)
        {
            UploadAndSaveImages();
        }

        private void RemoveProductImage(int additionalImageNbr)
        {
            int categoryId = CurrentCategoryId;
            int productId = CurrentProductId;

            ValidationResults errors = Cntrl.RemoveProductImage(CurrentUser, productId, additionalImageNbr);

            if (errors.IsValid)
            {
                this.Master.DisplayFriendlyMessage("Image Removed.");
                this.LoadProductDetail(categoryId, productId, false);
            }
            else
            {
                PageErrors.AddErrorMessages(errors, PageErrors.ValidationGroup);
            }
        }

        protected void lnkRemoveProductImage_Click(object sender, EventArgs e)
        {
            RemoveProductImage(0);
        }

        protected void lnkRemoveAdditionalImage1_Click(object sender, EventArgs e)
        {
            RemoveProductImage(1);
        }

        protected void lnkRemoveAdditionalImage2_Click(object sender, EventArgs e)
        {
            RemoveProductImage(2);
        }

        protected void lnkRemoveAdditionalImage3_Click(object sender, EventArgs e)
        {
            RemoveProductImage(3);
        }

        protected void lnkRemoveAdditionalImage4_Click(object sender, EventArgs e)
        {
            RemoveProductImage(4);
        }

        protected void lnkRemoveAdditionalImage5_Click(object sender, EventArgs e)
        {
            RemoveProductImage(5);
        }

        protected void btnCancelSaveNewProduct_Click(object sender, EventArgs e)
        {
            LoadCategoryDetail(CurrentCategoryId);
        }

        protected void btnSaveNewProduct_Click(object sender, EventArgs e)
        {
            Product newProduct = new Product();
            newProduct.ActiveFlag = true;
            newProduct.ProductName = txtProductName.Text.Trim();
            newProduct.ProductDescription = txtProductDescription.Content;
            newProduct.SortOrder = Util.ConvertInt32(txtProductSortOrder.Text.Trim());

            newProduct.ProductTypeCd = ddlProductType.SelectedValue;
            newProduct.CategoryId = CurrentCategoryId;

            newProduct.SubmissionDeadline = Util.ConvertNullDateTime(txtSubmissionDeadline.Text.Trim());
            newProduct.LateBehaviorCd = ddlLateBehavior.SelectedValue;
            newProduct.LateMessage = txtLateMessage.Text.Trim();
            newProduct.CalcQuantityFromAttributesFlag = false;
            newProduct.VisibleToExhibitors = true;

            ValidationResults errors = Cntrl.CreateProductShell(CurrentUser, newProduct);

            if (errors.IsValid)
            {
                if (newProduct.ProductTypeCd == ProductTypeEnum.SectionHeading.ToString())
                {
                    //Nothing else to Update for Headings, Save and reload Category Detail
                    this.LoadCategoryDetail(CurrentCategoryId);
                }
                else
                {
                    //Load the additional product detail controls for UPDATE
                    this.LoadProductDetail(newProduct.CategoryId, newProduct.ProductId, true);
                }
            }
            else
            {
                PageErrors.AddErrorMessages(errors, PageErrors.ValidationGroup);
            }
        }

        private bool SaveProductDetail(bool continueAfterSaving)
        {
            bool saved = false;
            try
            {
                int categoryId = CurrentCategoryId;
                Product product = new Product();
                product.ActiveFlag = true;
                product.ProductId = CurrentProductId;
                product.CategoryId = categoryId;
                product.ProductTypeCd = ddlProductType.SelectedValue;

                product.ProductName = txtProductName.Text.Trim();
                product.ProductDescription = txtProductDescription.Content;
                product.SortOrder = Util.ConvertInt32(txtProductSortOrder.Text.Trim());

                product.SubmissionDeadline = Util.ConvertNullDateTime(txtSubmissionDeadline.Text.Trim());
                product.LateBehaviorCd = ddlLateBehavior.SelectedValue;
                product.LateMessage = txtLateMessage.Text.Trim();

                if (product.ProductTypeCd == ProductTypeEnum.SectionHeading.ToString())
                {
                    product.VisibleToExhibitors = true;

                }
                else if (product.ProductTypeCd == ProductTypeEnum.FileDownload.ToString())
                {
                    product.VisibleToExhibitors = true;
                    if (hdnFileDownLoadId.Value.Length > 0)
                    {
                        product.FileDownloadId = Util.ConvertInt32(hdnFileDownLoadId.Value);
                    }
                }
                else if (product.ProductTypeCd == ProductTypeEnum.Item.ToString())
                {
                    product.FileDownloadId = null;
                    product.ProductSku = txtProductSku.Text.Trim();

                    product.ImageName = lblImageName.Text;
                    product.AdditionalImageName1 = lblAdditionalImageName1.Text;
                    product.AdditionalImageName2 = lblAdditionalImageName2.Text;
                    product.AdditionalImageName3 = lblAdditionalImageName3.Text;
                    product.AdditionalImageName4 = lblAdditionalImageName4.Text;
                    product.AdditionalImageName5 = lblAdditionalImageName5.Text;

                    product.SubmissionDeadline = Util.ConvertNullDateTime(txtSubmissionDeadline.Text.Trim());
                    product.DiscountDeadline = Util.ConvertNullDateTime(txtDiscountDeadline.Text.Trim());
                    product.UnitDescriptor = txtUnitDescriptor.Text.Trim();
                    product.QuantityLabel = txtQuantityLabel.Text.Trim();
                    product.MinimumQuantityRequired = Util.ConvertInt32(txtMinimumQuantityRequired.Text);
                    product.VisibleToExhibitors = chkVisibleToExhibitors.Checked;

                    product.UnitPrice = Util.ConvertNullDecimal(txtUnitPrice.Text.Trim());
                    product.DiscountUnitPrice = Util.ConvertNullDecimal(txtDiscountUnitPrice.Text.Trim());

                    product.EarlyBirdPrice = Util.ConvertNullDecimal(txtEarlyBirdPrice.Text.Trim());
                    if(!string.IsNullOrEmpty(txtEarlyBirdDeadline.Text.Trim()))
                    {
                        product.EarlyBirdDeadline = DateTime.Parse(txtEarlyBirdDeadline.Text.Trim());
                    }

                    product.SalesTaxPercent = ParsePercentageValue(txtSalesTax);

                    if (!string.IsNullOrEmpty(txtLateFeeDeadline.Text.Trim()))
                    {
                        product.LateFeeDeadline = DateTime.Parse(txtLateFeeDeadline.Text.Trim());
                    }
                    product.LateFeeType = ddlLateFeeType.SelectedValue;
                    product.LateFeeAmount = Util.ConvertNullDecimal(txtLateFeeAmount.Text.Trim());
                    if (product.LateFeeType == LateFeeTypeEnum.PctTotal.ToString())
                    {
                        product.LateFeeAmount = product.LateFeeAmount / 100;
                    }

                    product.AdditionalChargeType = ddlAdditionalChargeType.SelectedValue;
                    product.AdditionalChargeAmount = Util.ConvertNullDecimal(txtAdditionalCharge.Text.Trim());

                    if (product.AdditionalChargeType == AdditionalChargeTypeEnum.PctTotal.ToString())
                    {
                        product.AdditionalChargeAmount = product.AdditionalChargeAmount / 100;
                    }

                    product.AdditionalInfoFormId = Util.ConvertNullInteger(hdnAdditionalInfoFormId.Value);
                    product.RequiredAttribute1Label = txtRequiredAttributeLabel1.Text.Trim();
                    product.RequiredAttribute1ChoiceList = txtRequiredAttributeChoiceList1.Text.Trim();
                    product.RequiredAttribute2Label = txtRequiredAttributeLabel2.Text.Trim();
                    product.RequiredAttribute2ChoiceList = txtRequiredAttributeChoiceList2.Text.Trim();

                    product.RequiredAttribute3Label = txtRequiredAttributeLabel3.Text.Trim();
                    product.RequiredAttribute3ChoiceList = txtRequiredAttributeChoiceList3.Text.Trim();
                    product.RequiredAttribute4Label = txtRequiredAttributeLabel4.Text.Trim();
                    product.RequiredAttribute4ChoiceList = txtRequiredAttributeChoiceList4.Text.Trim();

                    product.CalcQuantityFromAttributesFlag = rdoLstCalcQuantityFromAttributes.Items.FindByValue("1").Selected;

                    product.InstallDismantleInd = rdoBtnLstInstallDismantleInd.SelectedValue;

                    product.TaxExemptFlag = chkTaxExempt.Checked;

                    product.IsBundle = rdoLstProductBundle.SelectedValue == "Y";

                }

                ValidationResults errors = Cntrl.SaveProduct(CurrentUser, product);

                if (errors.IsValid)
                {
                    if (continueAfterSaving)
                    {
                        LoadCategoryDetail(categoryId);
                        this.Master.DisplayFriendlyMessage("Product Saved.");
                    }
                }
                else
                {
                    PageErrors.AddErrorMessages(errors, PageErrors.ValidationGroup);
                    Page.MaintainScrollPositionOnPostBack = false;
                }

                saved = errors.IsValid;
                
            }
            catch (Exception ex)
            {
                EventLogger.LogException(ex);
                this.PageErrors.AddErrorMessages(ex);
            }
            return saved;
        }

        private void LoadAssociatedProducts(int productId)
        {
            LoadAssociatedProducts(Cntrl.GetProductById(productId).ProductAssociations);
        }

        private void LoadAssociatedProducts(ICollection<ProductAssociation> assocProducts)
        {
           grdvwAssociatedProducts.DataSource = assocProducts.OrderBy(pa => pa.InsertDateTime).ToList();
           grdvwAssociatedProducts.DataBind();
        }

        protected void rdoLstProductBundle_Changed(object sender, EventArgs e)
        {
            if (rdoLstProductBundle.SelectedValue == "Y")
            {
                plcProductAssociations.Visible = true;
                grdvwAssociatedProducts.Visible = true;
                LoadAssociatedProducts(CurrentProductId);
                LoadAssociatedProductsToAdd(CurrentProductId);
            }
            else
            {
                plcProductAssociations.Visible = false;
                grdvwAssociatedProducts.Visible = false;
            }
        }

        protected void grdvwAssociatedProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int associatedProductId = Util.ConvertInt32(e.CommandArgument);
            if (e.CommandName == "DeleteAssociatedProduct")
            {
                Cntrl.DeleteAssociatedProduct(CurrentUser, CurrentProductId, associatedProductId);
                LoadAssociatedProducts(CurrentProductId);
                LoadAssociatedProductsToAdd(CurrentProductId);
            }
        }

        protected void grdvwAssociatedProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }

        protected void btnAddAssociatedProduct_Click(object sender, EventArgs e)
        {
            ProductAssociation pa = new ProductAssociation();
            pa.ProductId = CurrentProductId;
            pa.AssociatedProductId = Util.ConvertInt32(cboAddAssociatedProductId.SelectedValue);
            pa.AssociatedQuantity = Util.ConvertInt32(txtAddAssociatedProductQuantity.Text);

            ValidationResults errors = Cntrl.AddAssociatedProduct(CurrentUser, pa);

            if (errors.IsValid)
            {
                LoadAssociatedProducts(CurrentProductId);
                LoadAssociatedProductsToAdd(CurrentProductId);
            }
            else
            {
                PageErrors.AddErrorMessages(errors);
            }
        }

        private void LoadAssociatedProductsToAdd(int productId)
        {
            txtAddAssociatedProductQuantity.Text = string.Empty;

            cboAddAssociatedProductId.Items.Clear();
            cboAddAssociatedProductId.ClearSelection();

            List<Product> availAssociatedProducts = Cntrl.FindAvailableAssociatedProducts(CurrentUser, CurrentUser.CurrentShow.ShowId, productId);
            cboAddAssociatedProductId.DataSource = availAssociatedProducts;
            cboAddAssociatedProductId.DataBind();

            cboAddAssociatedProductId.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("-- Select One --", string.Empty));

            cboAddAssociatedProductId.Items[0].Selected = true;
        }
        

        protected void btnSaveProduct_Click(object sender, EventArgs e)
        {
            SaveProductDetail(true);
        }

        protected void btnCancelSaveProduct_Click(object sender, EventArgs e)
        {
            this.LoadCategoryDetail(CurrentCategoryId);
        }

        protected void btnCancelAddAdditionalQuestion_Click(object sender, EventArgs e)
        {
            LoadProductDetail(CurrentCategoryId, CurrentProductId, false);
        }

        protected void btnAddAdditionalInfoQuestion_Click(object sender, EventArgs e)
        {
            if (SaveProductDetail(false))
            {
                Form additionalInfoForm = new Form();

                additionalInfoForm.FormId = Util.ConvertInt32(hdnAdditionalInfoFormId.Value);
                additionalInfoForm.ShowId = this.CurrentUser.CurrentShow.ShowId;
                additionalInfoForm.FormTypeCd = FormTypeEnum.Product.ToString();
                additionalInfoForm.ActiveFlag = true;
                additionalInfoForm.FormName = string.Format("{0}:{1}", "Product", CurrentProductId);

                Question question = ucFormQuestionEditor.BuildQuestion();

                ValidationResults errors = Cntrl.SaveAdditionalInfoQuestion(CurrentProductId, additionalInfoForm, question);

                if (errors.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Question Saved.");

                    hdnAdditionalInfoFormId.Value = additionalInfoForm.FormId.ToString();

                    LoadProductDetail(CurrentCategoryId, CurrentProductId, false);
                    this.Master.DisplayFriendlyMessage("Associated Product has been added.");
                }
                else
                {
                    PageErrors.AddErrorMessages(errors);
                }
            }
        }

        protected void grdvwCategoryList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Category categoryDetail = (Category)e.Row.DataItem;

                Image imgCategoryIcon = (Image)e.Row.FindControl("imgCategoryIcon");
                LinkButton lbtnDeleteCategory = (LinkButton)e.Row.FindControl("lbtnDeleteCategory");
                LinkButton lbtnRestoreCategory = (LinkButton)e.Row.FindControl("lbtnRestoreCategory");
                Literal ltrActive = (Literal)e.Row.FindControl("ltrActive");

                if (categoryDetail.ActiveFlag)
                {
                    ltrActive.Text = "Yes";
                    lbtnDeleteCategory.Visible = true;
                    lbtnDeleteCategory.Attributes.Add("onClick", "javascript: return confirm('Sure you want to Remove this category?');");
                    lbtnRestoreCategory.Visible = false;
                }
                else
                {
                    ltrActive.Text = "No";
                    lbtnDeleteCategory.Visible = false;
                    lbtnRestoreCategory.Visible = true;
                }

                if (!string.IsNullOrEmpty(categoryDetail.IconFileName))
                {
                    imgCategoryIcon.Visible = true;
                    imgCategoryIcon.ImageUrl = string.Format("~/Assets/Shows/{0}/{1}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), categoryDetail.IconFileName);
                    imgCategoryIcon.AlternateText = categoryDetail.IconFileName;
                }
                else
                {
                    imgCategoryIcon.Visible = false;
                }
            }
        }


        protected void grdvwProductList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Product productDetail = (Product)e.Row.DataItem;

                TextBox txtListItemProductPrice = (TextBox)e.Row.FindControl("txtListItemProductPrice");
                TextBox txtListItemProductDiscountPrice = (TextBox)e.Row.FindControl("txtListItemProductDiscountPrice");
                TextBox txtListItemProductEarlyBirdPrice = (TextBox)e.Row.FindControl("txtListItemProductEarlyBirdPrice");
                TextBox txtListItemSalesTax = (TextBox)e.Row.FindControl("txtListItemSalesTax");

                txtListItemProductPrice.Visible = txtListItemProductDiscountPrice.Visible = txtListItemProductEarlyBirdPrice.Visible = txtListItemSalesTax.Visible = false;

                if (productDetail.ProductTypeCd == ProductTypeEnum.Item.ToString())
                {
                    if (productDetail.TaxExemptFlag.HasValue && productDetail.TaxExemptFlag.Value == true)
                    {
                        txtListItemSalesTax.Visible = true;
                        txtListItemSalesTax.Enabled = false;
                        txtListItemSalesTax.Text = "n/a";
                        txtListItemSalesTax.ToolTip = "Tax Exempt";
                    }
                    txtListItemProductPrice.Visible = txtListItemProductDiscountPrice.Visible = txtListItemProductEarlyBirdPrice.Visible = txtListItemSalesTax.Visible = true;
                }
            }
        }


        private void AdditionalInfoQuestionSelected(int questionId)
        {
            Question question = null;
            ucFormQuestionEditor.ClearQuestion();

            btnCancelAddAdditionalQuestion.Visible = false;

            if (questionId > 0)
            {
                btnAddAdditionalInfoQuestion.Text = "Save Question";
                btnCancelAddAdditionalQuestion.Visible = true;
                ucFormQuestionEditor.SetFocus();

                ltrQuestionEditorLabel.Text = "Edit Question";

                Form form = this.Cntrl.GetAdditionalFormById(Util.ConvertInt32(this.hdnAdditionalInfoFormId.Value));

                if (form != null)
                {
                    question = form.Questions.Where(q => q.QuestionId == questionId).SingleOrDefault();
                    if (question != null)
                    {
                        ucFormQuestionEditor.Populate(question);
                    }
                }
            }
        }

        private void AdditionalInfoQuestionDeleted(int questionId)
        {
            this.Cntrl.DeleteAdditionalInfoQuestion(questionId);
            this.LoadProductDetail(CurrentCategoryId, CurrentProductId, false);
        }

        protected void btnApplyShowDefaults_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtDefaultSubmissionDeadline.Text.Trim()))
                {
                    CurrentUser.CurrentShow.DefaultSubmissionDeadline = DateTime.Parse(txtDefaultSubmissionDeadline.Text.Trim());
                }
                else
                {
                    CurrentUser.CurrentShow.DefaultSubmissionDeadline = null;
                }

                if (!string.IsNullOrEmpty(txtDefaultDiscountDeadline.Text.Trim()))
                {
                    CurrentUser.CurrentShow.DefaultDiscountDeadline = DateTime.Parse(txtDefaultDiscountDeadline.Text.Trim());
                }
                else
                {
                    CurrentUser.CurrentShow.DefaultDiscountDeadline = null;
                }

                if (!string.IsNullOrEmpty(txtDefaultEarlyBirdDeadline.Text.Trim()))
                {
                    CurrentUser.CurrentShow.DefaultEarlyBirdDeadline = DateTime.Parse(txtDefaultEarlyBirdDeadline.Text.Trim());
                }
                else
                {
                    CurrentUser.CurrentShow.DefaultEarlyBirdDeadline = null;
                }

                if (!string.IsNullOrEmpty(txtDefaultLateFeeDeadline.Text.Trim()))
                {
                    CurrentUser.CurrentShow.DefaultLateFeeDeadline = DateTime.Parse(txtDefaultLateFeeDeadline.Text.Trim());
                }
                else
                {
                    CurrentUser.CurrentShow.DefaultLateFeeDeadline = null;
                }

                if (!string.IsNullOrEmpty(txtDefaultSalesTax.Text.Trim()))
                {
                    CurrentUser.CurrentShow.DefaultSalesTax = ParsePercentageValue(txtDefaultSalesTax).Value;
                }

                ValidationResults errors = Cntrl.ApplyShowLevelDefaults(CurrentUser, rdoApplyToAll.Checked);
                if (errors.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Deadlines and Sales Tax applied.");
                }
                else
                {
                    this.PageErrors.AddErrorMessages(errors);
                }
            }
            catch (Exception ex)
            {
                EventLogger.LogException(ex);
                this.PageErrors.AddErrorMessages(ex);
            }
        }

        protected void btnApplyCategoryDefaults_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime? dteSubmissionDeadline = Util.ConvertNullDateTime(txtCategorySubmissionDeadline.Text.Trim());
                DateTime? dteDiscountDeadline = Util.ConvertNullDateTime(txtCategoryDiscountDeadline.Text.Trim());
                DateTime? dteEarlyBirdDeadline = Util.ConvertNullDateTime(txtCategoryEarlyBirdDeadline.Text.Trim());
                DateTime? dteLateFeeDeadline = Util.ConvertNullDateTime(txtCategoryLateFeeDeadline.Text.Trim());
                Double? dblSalesTax = ParsePercentageValue(txtCategorySalesTax).Value;
                

                ValidationResults errors = Cntrl.ApplyCategoryLevelDefaults(CurrentUser, CurrentCategoryId, rdoApplyCategoryToAll.Checked, dteSubmissionDeadline, dteDiscountDeadline, dteEarlyBirdDeadline, dteLateFeeDeadline, dblSalesTax);
                if (errors.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Deadlines and Sales Tax applied.");
                    this.LoadCategoryDetail(CurrentCategoryId);
                }
                else
                {
                    this.PageErrors.AddErrorMessages(errors);
                }
            }
            catch(Exception ex)
            {
                EventLogger.LogException(ex);
                this.PageErrors.AddErrorMessages(ex);
            }
        }
        

        protected void btnDisplayCopyCategory_Click(object sender, EventArgs e)
        {
            lblCopyCategoryLabel.Text = "Pick Show and Category to copy";

            ddlCopyCategoryList.Enabled = false;
            ddlCopyCategoryList.Items.Clear();
            ddlCopyCategoryList.Items.Insert(0, new ListItem("-- Select One --", string.Empty));

            ddlCopyCategoryShowList.Items.Clear();

            this.MPE.Show();

            ShowController cntrl = new ShowController();
            List<Show> shows = cntrl.GetOwnedShowsByUserId(CurrentUser, true).Where(s => s.ActiveFlag == true && s.ShowId != CurrentUser.CurrentShow.ShowId).ToList();

            if (shows != null)
            {
                shows.ForEach(s =>
                    {
                        string showStartDate = string.Empty;
                        if (s.StartDate.HasValue)
                        {
                            showStartDate = s.StartDate.Value.ToShortDateString();
                        }
                        ddlCopyCategoryShowList.Items.Add(new ListItem(string.Format("{0} - {1}", s.ShowName, showStartDate), s.ShowId.ToString()));
                    });
            }

            ddlCopyCategoryShowList.Items.Insert(0, new ListItem("-- Select One --", string.Empty));
        }

        protected void btnDisplayCopyProduct_Click(object sender, EventArgs e)
        {
            lblCopyProductLabel.Text = "Pick Show/Category to copy from";

            ddlCopyProductCategoryList.Enabled = false;
            ddlCopyProductCategoryList.Items.Clear();
            ddlCopyProductCategoryList.Items.Insert(0, new ListItem("-- Select One --", string.Empty));

            ddlCopyProductProductList.Enabled = false;
            ddlCopyProductProductList.Items.Clear();
            ddlCopyProductProductList.Items.Insert(0, new ListItem("-- Select One --", string.Empty));

            ddlCopyProductShowList.Items.Clear();

            this.MPE2.Show();

            ShowController cntrl = new ShowController();
            List<Show> shows = cntrl.GetOwnedShowsByUserId(CurrentUser, true).Where(s => s.ActiveFlag == true && s.ShowId != CurrentUser.CurrentShow.ShowId).ToList();

            if (shows != null)
            {
                shows.ForEach(s =>
                    {
                        string showStartDate = string.Empty;
                        if (s.StartDate.HasValue)
                        {
                            showStartDate = s.StartDate.Value.ToShortDateString();
                        }
                        ddlCopyProductShowList.Items.Add(new ListItem(string.Format("{0} - {1}", s.ShowName, showStartDate), s.ShowId.ToString()));
                    });
            }

            ddlCopyProductShowList.Items.Insert(0, new ListItem("-- Select One --", string.Empty));
        }

        protected void ddlCopyProductShowList_IndexChanged(object sender, EventArgs e)
        {
            this.ddlCopyProductCategoryList.Items.Clear();
            this.ddlCopyProductCategoryList.Enabled = true;
            if (!string.IsNullOrEmpty(this.ddlCopyProductShowList.SelectedValue))
            {
                int selectedShowId = Util.ConvertInt32(this.ddlCopyProductShowList.SelectedValue);
                List<Category> categories = Cntrl.GetCategoryList(selectedShowId).Where(cat => cat.ActiveFlag == true).ToList();

                if (categories != null)
                {
                    categories.ForEach(c =>
                    {
                        ddlCopyProductCategoryList.Items.Add(new ListItem(c.CategoryName, c.CategoryId.ToString()));
                    });
                }
            }

            this.ddlCopyProductCategoryList.Items.Insert(0, new ListItem("-- Select One --", string.Empty));
            this.MPE2.Show();
        }

        protected void ddlCopyProductCategoryList_IndexChanged(object sender, EventArgs e)
        {
            this.ddlCopyProductProductList.Items.Clear();
            this.ddlCopyProductProductList.Enabled = true;
            if (!string.IsNullOrEmpty(this.ddlCopyProductCategoryList.SelectedValue))
            {
                int selectedShowId = Util.ConvertInt32(this.ddlCopyProductShowList.SelectedValue);
                int selectedCategoryId = Util.ConvertInt32(this.ddlCopyProductCategoryList.SelectedValue);
                List<Product> products = Cntrl.GetProductList(selectedShowId, selectedCategoryId).Where(p => p.ProductTypeCd == "Item").ToList();

                if (products != null)
                {
                    products.ForEach(p =>
                    {
                        ddlCopyProductProductList.Items.Add(new ListItem(p.ProductName, p.ProductId.ToString()));
                    });
                }
            }

            this.ddlCopyProductProductList.Items.Insert(0, new ListItem("-- Select One --", string.Empty));
            this.MPE2.Show();
        }

        protected void btnCopyProduct_Click(object sender, EventArgs e)
        {
            int selectedShowId = Util.ConvertInt32(ddlCopyProductShowList.SelectedValue);
            int selectedCategoryId = Util.ConvertInt32(ddlCopyProductCategoryList.SelectedValue);
            int selectedProductId = Util.ConvertInt32(ddlCopyProductProductList.SelectedValue);

            ValidationResults errors = Cntrl.CopyProduct(CurrentUser, CurrentUser.CurrentShow.ShowId, CurrentCategoryId, selectedProductId);

            if (!errors.IsValid)
            {
                lblCopyProductLabel.Text = Util.AllValidationErrors(errors);
                this.MPE2.Show();
            }
            else
            {
                CopyProductFiles(selectedProductId);
                this.MPE2.Hide();
                LoadCategoryDetail(CurrentCategoryId);
                this.Master.DisplayFriendlyMessage("Product and files copied.");
            }
        }

        protected void btnCancelCopyProduct_Click(object sender, EventArgs e)
        {
            this.MPE2.Hide();
        }


        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCategoryList();
        }

        protected void btnCancelCopyCategory_Click(object sender, EventArgs e)
        {
            this.MPE.Hide();
        }

        protected void btnCopyCategory_Click(object sender, EventArgs e)
        {
            int selectedShowId = Util.ConvertInt32(ddlCopyCategoryShowList.SelectedValue);
            int selectedCategoryId = Util.ConvertInt32(ddlCopyCategoryList.SelectedValue);

            ValidationResults errors = Cntrl.CopyCategory(CurrentUser, CurrentUser.CurrentShow.ShowId, selectedCategoryId);

            if (!errors.IsValid)
            {
                lblCopyCategoryLabel.Text = Util.AllValidationErrors(errors);
                this.MPE.Show();
            }
            else
            {
                CopyCategoryFiles(selectedCategoryId);
                this.MPE.Hide();
                LoadCategoryList();
                this.Master.DisplayFriendlyMessage("Category and files copied.");
            }

        }

        private void CopyCategoryFiles(int categoryId)
        {
            List<ShowFile> fileList = Cntrl.BuildCategoryFilesToCopy(categoryId);

            if (fileList != null & fileList.Count > 0)
            {
                fileList.ForEach(f =>
                {
                    if (!string.IsNullOrEmpty(f.FileName))
                    {
                        if (f.FileSource == "Product")
                        {
                            CopyFile(f.ShowGuid, CurrentUser.CurrentShow.ShowGuid, "Products", f.FileName);
                        }
                        else if (f.FileSource == "FileDownload")
                        {
                            CopyFile(f.ShowGuid, CurrentUser.CurrentShow.ShowGuid, "Downloads", f.FileName);
                        }
                        else
                        {
                            CopyFile(f.ShowGuid, CurrentUser.CurrentShow.ShowGuid, string.Empty, f.FileName);
                        }
                    }
                });
            }
        }

        private void CopyProductFiles(int categoryId)
        {
            List<ShowFile> fileList = Cntrl.BuildProductFilesToCopy(categoryId);

            if (fileList != null & fileList.Count > 0)
            {
                fileList.ForEach(f =>
                {
                    if (!string.IsNullOrEmpty(f.FileName))
                    {
                        if (f.FileSource == "Product")
                        {
                            CopyFile(f.ShowGuid, CurrentUser.CurrentShow.ShowGuid, "Products", f.FileName);
                        }
                        else if (f.FileSource == "FileDownload")
                        {
                            CopyFile(f.ShowGuid, CurrentUser.CurrentShow.ShowGuid, "Downloads", f.FileName);
                        }
                        else
                        {
                            CopyFile(f.ShowGuid, CurrentUser.CurrentShow.ShowGuid, string.Empty, f.FileName);
                        }
                    }
                });
            }
        }

        private void CopyFile(Guid sourceShowGuid, Guid targetShowGuid, string assetFolderName, string fileName)
        {
            string sourceFilePath = Server.MapPath(string.Format("~/Assets/Shows/{0}/" + assetFolderName + "/{1}", sourceShowGuid.ToString(), fileName));
            string targetFilePath = Server.MapPath(string.Format("~/Assets/Shows/{0}/" + assetFolderName + "/{1}", targetShowGuid.ToString(), fileName));


            if (File.Exists(sourceFilePath))
            {
                if (File.Exists(targetFilePath))
                {
                    File.Delete(targetFilePath);
                }

                File.Copy(sourceFilePath, targetFilePath, true);
            }
        }


        protected void ddlCopyCategoryShowList_IndexChanged(object sender, EventArgs e)
        {
            this.ddlCopyCategoryList.Items.Clear();
            this.ddlCopyCategoryList.Enabled = true;
            if (!string.IsNullOrEmpty(this.ddlCopyCategoryShowList.SelectedValue))
            {
                int selectedShowId = Util.ConvertInt32(this.ddlCopyCategoryShowList.SelectedValue);
                List<Category> categories = Cntrl.GetCategoryList(selectedShowId).Where(cat => cat.ActiveFlag == true).ToList();

                if (categories != null)
                {
                    categories.ForEach(c =>
                    {
                        ddlCopyCategoryList.Items.Add(new ListItem(c.CategoryName, c.CategoryId.ToString()));
                    });
                }
            }

            this.ddlCopyCategoryList.Items.Insert(0, new ListItem("-- Select One --", string.Empty));
            this.MPE.Show();
        }

        #endregion

        #region Properties

        private int CurrentCategoryId
        {
            get { return Util.ConvertInt32(hdnCategoryId.Value); }
            set { hdnCategoryId.Value = value.ToString(); }
        }

        private int CurrentProductId
        {
            get { return Util.ConvertInt32(hdnProductId.Value); }
            set { hdnProductId.Value = value.ToString(); }
        }



        #endregion

    }
}