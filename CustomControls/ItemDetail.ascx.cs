using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Entities;
using ExpoOrders.Common;
using System.Web.UI.HtmlControls;

namespace ExpoOrders.Web.CustomControls
{
    public partial class ItemDetail : System.Web.UI.UserControl
    {
        private ExpoOrdersUser _currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        private Action<int, ProductTypeEnum, decimal> _itemSelected;
        public Action<int, ProductTypeEnum, decimal> ItemSelected
        {
            set
            {
                _itemSelected = value;
            }
        }

        private Action<int, string> _backToCategory;
        public Action<int, string> BackToCategory
        {
            set { _backToCategory = value; }
        }


        public string BuildImageSource(string imageName)
        {
            return imageName;
        }

        public void PopulateControl(ExpoOrdersUser currentUser, Product productDetail, string parentCategoryName, string categoryName, Form additionalInfoForm)
        {
            DateTime dteNow = DateTime.Now;
            lblWarningMessage.Text = string.Empty;
            lblWarningMessage.Visible = false;
            btnAddToCart.Visible = true;

            lblMinimumQuantityRequired.Text = string.Empty;
            plcMinimumQuantityRequired.Visible = false;

            _currentUser = currentUser;

            ltrTotalUnitPrice.Text =
                ltrTotalAdditionalCharges.Text =
                ltrTotalLateFee.Text =
                ltrTotalSalesTax.Text =
                ltrTotalCost.Text = Server.HtmlEncode(Util.FormatCurrency(0, currentUser.CurrentShow.CurrencySymbol));

            ucFormQuestions.Visible = false;
            ltrQuantityLabel.Text = string.IsNullOrEmpty(productDetail.QuantityLabel) ? "Quantity" : productDetail.QuantityLabel;

            txtQuantity.Enabled = true;
            txtQuantity.Text = (0).ToString();
            hdnAdditionalChargeAmt.Value = (0).ToString();
            hdnIsCalcByAttribs.Value = (0).ToString();

            hdnAdditionalChargeType.Value = string.Empty;
            hdnLateFeeAmt.Value = (0).ToString();
            hdnLateFeeType.Value = string.Empty;
            hdnSalesTaxPct.Value = (0).ToString();
            hdnUnitPrice.Value = (0).ToString();
            hdnCurrencySymbol.Value = currentUser.CurrentShow.CurrencySymbol;

            plcRequiredAttribute1.Visible = false;
            lblRequiredAttributeLabel1.Text = string.Empty;
            lblRequiredAttributeLabel1.CssClass = "dataLabel";
            ddlRequiredAttributeChoices1.Items.Clear();

            plcRequiredAttribute2.Visible = false;
            lblRequiredAttributeLabel2.Text = string.Empty;
            lblRequiredAttributeLabel2.CssClass = "dataLabel";
            ddlRequiredAttributeChoices2.Items.Clear();

            plcRequiredAttribute3.Visible = false;
            lblRequiredAttributeLabel3.Text = string.Empty;
            lblRequiredAttributeLabel3.CssClass = "dataLabel";
            ddlRequiredAttributeChoices3.Items.Clear();

            plcRequiredAttribute4.Visible = false;
            lblRequiredAttributeLabel4.Text = string.Empty;
            lblRequiredAttributeLabel4.CssClass = "dataLabel";
            ddlRequiredAttributeChoices4.Items.Clear();

            lblUnitPriceLabel.CssClass = string.Empty;
            lblUnitPrice.CssClass = string.Empty;

            if (productDetail != null && productDetail.ProductId > 0)
            {
                if (productDetail.MinimumQuantityRequired.HasValue && productDetail.MinimumQuantityRequired.Value > 0)
                {
                    plcMinimumQuantityRequired.Visible = true;
                    lblMinimumQuantityRequired.Text = string.Format("Required Minimum: {0}", productDetail.MinimumQuantityRequired.Value);
                }

                if (!string.IsNullOrEmpty(productDetail.RequiredAttribute1Label))
                {
                    PaintRequiredAttributes(productDetail.CalcQuantityFromAttributesFlag, productDetail.RequiredAttribute1Label, productDetail.RequiredAttribute1ChoiceList, plcRequiredAttribute1, ddlRequiredAttributeChoices1, lblRequiredAttributeLabel1);
                }

                if (!string.IsNullOrEmpty(productDetail.RequiredAttribute2Label))
                {
                    PaintRequiredAttributes(productDetail.CalcQuantityFromAttributesFlag, productDetail.RequiredAttribute2Label, productDetail.RequiredAttribute2ChoiceList, plcRequiredAttribute2, ddlRequiredAttributeChoices2, lblRequiredAttributeLabel2);
                }

                if (!string.IsNullOrEmpty(productDetail.RequiredAttribute3Label))
                {
                    PaintRequiredAttributes(null, productDetail.RequiredAttribute3Label, productDetail.RequiredAttribute3ChoiceList, plcRequiredAttribute3, ddlRequiredAttributeChoices3, lblRequiredAttributeLabel3);
                }

                if (!string.IsNullOrEmpty(productDetail.RequiredAttribute4Label))
                {
                    PaintRequiredAttributes(null, productDetail.RequiredAttribute4Label, productDetail.RequiredAttribute4ChoiceList, plcRequiredAttribute4, ddlRequiredAttributeChoices4, lblRequiredAttributeLabel4);
                }

                if (productDetail.CalcQuantityFromAttributesFlag.HasValue && productDetail.CalcQuantityFromAttributesFlag.Value == true)
                {
                    txtQuantity.Enabled = false;
                    hdnIsCalcByAttribs.Value = (1).ToString();
                }

                this.Visible = true;

                DisplayAdditionalInfo(additionalInfoForm);

                ltrParentCategoryName.Text = parentCategoryName;
                ltrCategoryName.Text = ltrCategoryName.Text;

                if (!String.IsNullOrEmpty(productDetail.ImageName))
                {
                    imgProductDetailImage.Visible = true;
                    imgProductDetailImage.ImageUrl = string.Format("~/Assets/Shows/{0}/Products/{1}", currentUser.CurrentShow.ShowGuid, productDetail.ImageName);
                    imgProductDetailImage.AlternateText = imgProductDetailImage.ToolTip = productDetail.ProductName;
                }
                else
                {
                    imgProductDetailImage.Visible = false;
                }

                rptrAdditionalImages.Visible = false;
                plcAdditionalImages.Visible = false;
                if (productDetail.ContainsAdditionalImages)
                {
                    rptrAdditionalImages.Visible = true;
                    rptrAdditionalImages.DataSource = productDetail.AllProductImages(true);
                    rptrAdditionalImages.DataBind();
                    plcAdditionalImages.Visible = true;
                }

                ltrProductName.Text = productDetail.ProductName;
                ltrProductDescription.Text = productDetail.ProductDescription;
                if (productDetail.SubmissionDeadline.HasValue)
                {
                    plcSubmissionDeadline.Visible = true;
                    ltrSubmissionDeadline.Text = productDetail.SubmissionDeadline.Value.ToShortDateString();
                }

                decimal actualUnitPrice = productDetail.DetermineCurrentUnitPrice(dteNow, true);

                lblUnitPriceLabel.Text = productDetail.DetermineCurrentUnitPriceDescription(dteNow, true);
                lblUnitPrice.Text = Server.HtmlEncode(Util.FormatUnitPrice(actualUnitPrice, productDetail.UnitDescriptor, _currentUser.CurrentShow.CurrencySymbol));


                //Populate the Wacky Pricing Labels based on Rollovers from advanced and various deadlines.
                ucPricingLabels.Populate(_currentUser, productDetail);

                hdnUnitPrice.Value = actualUnitPrice.ToString();

                if (productDetail.AdditionalChargesApply())
                {
                    AdditionalChargeTypeEnum additionalChargeType = Enum<AdditionalChargeTypeEnum>.Parse(productDetail.AdditionalChargeType, true);
                    string additionalChargeDisplay = string.Empty;
                    switch (additionalChargeType)
                    {
                        case AdditionalChargeTypeEnum.Flat:
                            additionalChargeDisplay = Server.HtmlEncode(Util.FormatUnitPrice(productDetail.AdditionalChargeAmount.Value, string.Empty, _currentUser.CurrentShow.CurrencySymbol));
                            break;
                        case AdditionalChargeTypeEnum.PerUnit:
                            additionalChargeDisplay = Server.HtmlEncode(Util.FormatUnitPrice(productDetail.AdditionalChargeAmount.Value, "ea", _currentUser.CurrentShow.CurrencySymbol));
                            break;
                        case AdditionalChargeTypeEnum.PctTotal:
                            additionalChargeDisplay = Util.FormatPercentage(productDetail.AdditionalChargeAmount);
                            break;
                        default:
                            break;
                    }


                    ltrAdditionalCharge.Text = additionalChargeDisplay;
                    hdnAdditionalChargeAmt.Value = productDetail.AdditionalChargeAmount.Value.ToString();
                    hdnAdditionalChargeType.Value = productDetail.AdditionalChargeType.ToString();
                }
                else
                {

                    ltrAdditionalCharge.Text = string.Empty;
                    hdnAdditionalChargeAmt.Value = (0).ToString();
                    hdnAdditionalChargeType.Value = string.Empty;
                }


                //late fees
                if (productDetail.LateFeesApply(DateTime.Now))
                {
                    LateFeeTypeEnum lateFeeType = Enum<LateFeeTypeEnum>.Parse(productDetail.LateFeeType, true);
                    string lateFeeDisplay = string.Empty;
                    switch (lateFeeType)
                    {
                        case LateFeeTypeEnum.Flat:
                            lateFeeDisplay = Server.HtmlEncode(Util.FormatUnitPrice(productDetail.LateFeeAmount.Value, string.Empty, _currentUser.CurrentShow.CurrencySymbol));
                            break;
                        case LateFeeTypeEnum.PerUnit:
                            lateFeeDisplay = Server.HtmlEncode(Util.FormatUnitPrice(productDetail.LateFeeAmount.Value, "ea", _currentUser.CurrentShow.CurrencySymbol));
                            break;
                        case LateFeeTypeEnum.PctTotal:
                            lateFeeDisplay = Util.FormatPercentage(productDetail.LateFeeAmount);
                            break;
                        default:
                            break;
                    }


                    this.ltrLateFee.Text = lateFeeDisplay;
                    this.hdnLateFeeAmt.Value = productDetail.LateFeeAmount.Value.ToString();
                    this.hdnLateFeeType.Value = productDetail.LateFeeType.ToString();
                }
                else
                {
                    this.ltrLateFee.Text = string.Empty;
                    this.hdnLateFeeAmt.Value = (0).ToString();
                    this.hdnLateFeeType.Value = (0).ToString();
                }

                if (currentUser.CurrentExhibitor.TaxExemptFlag)
                {
                    ltrSalesTax.Text = "(exempt)";
                    hdnSalesTaxPct.Value = "0";
                }
                else
                {
                    if (productDetail.TaxExemptFlag.HasValue && productDetail.TaxExemptFlag.Value == true)
                    {
                        ltrSalesTax.Text = string.Empty;
                        hdnSalesTaxPct.Value = "0";
                    }
                    else
                    {
                        if (productDetail.SalesTaxPercent.HasValue)
                        {
                            ltrSalesTax.Text = Util.FormatPercentage(productDetail.SalesTaxPercent);
                            hdnSalesTaxPct.Value = productDetail.SalesTaxPercent.Value.ToString();
                        }
                        else
                        {
                            ltrSalesTax.Text = string.Empty;
                            hdnSalesTaxPct.Value = "0";
                        }
                    }
                }

                btnBackToCategory.CommandArgument = productDetail.CategoryId.ToString();

                btnAddToCart.CommandName = productDetail.ProductTypeCd;
                btnAddToCart.CommandArgument = productDetail.ProductId.ToString();

                if (productDetail.SubmissionDeadline != null && productDetail.SubmissionDeadline.HasValue)
                {
                    if (productDetail.IsPassedSubmissionDeadline(DateTime.Now))
                    {
                        LateSubmissionBehaviorEnum lateBehavior = Enum<LateSubmissionBehaviorEnum>.Parse(productDetail.LateBehaviorCd, true);

                        if (lateBehavior == LateSubmissionBehaviorEnum.Prevent)
                        {
                            btnAddToCart.Visible = false;
                            if (!string.IsNullOrEmpty(productDetail.LateMessage))
                            {
                                lblWarningMessage.Visible = true;
                                lblWarningMessage.Text = productDetail.LateMessage;
                            }
                            
                        }
                        else if (lateBehavior == LateSubmissionBehaviorEnum.Warning)
                        {
                            btnAddToCart.Visible = true;
                            if (!string.IsNullOrEmpty(productDetail.LateMessage))
                            {
                                lblWarningMessage.Visible = true;
                                lblWarningMessage.Text = productDetail.LateMessage;
                            }
                            
                        }
                    }
                }
            }
        }

        private void PaintRequiredAttributes(bool? isCalcFromAttributes, string attributeLabel, string choiceList, PlaceHolder plc, DropDownList ddl, Label lbl)
        {
            if (!string.IsNullOrEmpty(attributeLabel) && !string.IsNullOrEmpty(choiceList))
            {
                ddl.Items.Clear();

                plc.Visible = true;
                lbl.Text = attributeLabel;

                ddl.Items.Add(new ListItem(string.Empty, string.Empty));

                char[] delimiter = new char[] { ';' };
                foreach (string choice in choiceList.Split(delimiter, StringSplitOptions.RemoveEmptyEntries))
                {
                    ddl.Items.Add(new ListItem(choice, choice));
                }

                if (isCalcFromAttributes.HasValue && isCalcFromAttributes.Value == true)
                {
                    ddl.Attributes.Add("onChange", "calcCost();");
                }
            }
            else
            {
                plc.Visible = false;
            }
        }

        public List<QuestionAnswer> GetAdditionalInfoResponses(ref List<string> missingRequiredQuestions)
        {
            return ucFormQuestions.GetResponses(ref missingRequiredQuestions);
        }

        public bool RequiresAttribute1()
        {
            return plcRequiredAttribute1.Visible;
        }

        public bool RequiresAttribute2()
        {
            return plcRequiredAttribute2.Visible;
        }

        public bool RequiresAttribute3()
        {
            return plcRequiredAttribute3.Visible;
        }

        public bool RequiresAttribute4()
        {
            return plcRequiredAttribute4.Visible;
        }

        public string RequiredAttribute1Label
        {
            get
            {
                return lblRequiredAttributeLabel1.Text;
            }
        }

        public string RequiredAttribute1Selected
        {
            get
            {
                return ddlRequiredAttributeChoices1.SelectedItem.Value;
            }
        }


        public string RequiredAttribute2Label
        {
            get
            {
                return lblRequiredAttributeLabel2.Text;
            }
        }

        public string RequiredAttribute2Selected
        {
            get
            {
                return ddlRequiredAttributeChoices2.SelectedItem.Value;
            }
        }

        public string RequiredAttribute3Label
        {
            get
            {
                return lblRequiredAttributeLabel3.Text;
            }
        }

        public string RequiredAttribute3Selected
        {
            get
            {
                return ddlRequiredAttributeChoices3.SelectedItem.Value;
            }
        }

        public string RequiredAttribute4Label
        {
            get
            {
                return lblRequiredAttributeLabel4.Text;
            }
        }

        public string RequiredAttribute4Selected
        {
            get
            {
                return ddlRequiredAttributeChoices4.SelectedItem.Value;
            }
        }

        private void DisplayAdditionalInfo(Form additionalInfoForm)
        {
            trAdditionalInfo.Visible = false;
            if (additionalInfoForm != null && additionalInfoForm.Questions != null && additionalInfoForm.Questions.Count > 0)
            {
                ucFormQuestions.Visible = true;
                trAdditionalInfo.Visible = true;
                ucFormQuestions.PopulateQuestions(additionalInfoForm.Questions);
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (_itemSelected != null)
            {
                decimal qty = 0.0m;
                if (!txtQuantity.Enabled)
                {
                    qty = Util.ConvertDecimal(ddlRequiredAttributeChoices1.SelectedValue) * Util.ConvertDecimal(ddlRequiredAttributeChoices2.SelectedValue);
                }
                else
                {
                    qty = Util.ConvertDecimal(txtQuantity.Text);
                }
                _itemSelected(Util.ConvertInt32(btnAddToCart.CommandArgument), Enum<ProductTypeEnum>.Parse(btnAddToCart.CommandName, true), qty);
            }
        }

        protected void btnBackToCategory_Click(object sender, EventArgs e)
        {
            if( sender is Button)
            {
                int categoryId = Util.ConvertInt32(((Button)sender).CommandArgument);
                _backToCategory(categoryId, ltrParentCategoryName.Text);
            }   
        }

        public void HighlightRequiredAttribute(int attribNumber, bool isError)
        {
            Label lbl = null;
            if (attribNumber == 1)
            {
                lbl = lblRequiredAttributeLabel1;
            }
            else if (attribNumber == 2)
            {
                lbl = lblRequiredAttributeLabel2;
            }
            else if (attribNumber == 3)
            {
                lbl = lblRequiredAttributeLabel3;
            }
            else if (attribNumber == 4)
            {
                lbl = lblRequiredAttributeLabel4;
            }
            if (lbl != null)
            {
                lbl.CssClass = isError ? "dataLabelError" : "dataLabel";
            }

        }

        protected void rptrAdditionalImages_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Image imgAdditionalImage = (Image)e.Item.FindControl("imgAdditionalImage");
                HtmlAnchor lnkAdditionalImage = (HtmlAnchor)e.Item.FindControl("lnkAdditionalImage");

                string imageName = (string)e.Item.DataItem;

                if (!string.IsNullOrEmpty(imageName))
                {
                    imgAdditionalImage.Visible = true;
                    imgAdditionalImage.AlternateText = imgAdditionalImage.ToolTip = lnkAdditionalImage.Title = imageName;
                    imgAdditionalImage.ImageUrl = string.Format("~/Assets/Shows/{0}/Products/{1}", _currentUser.CurrentShow.ShowGuid, imageName);
                    lnkAdditionalImage.Attributes.Add("onClick", string.Format("swapImageSrc('{0}', '{1}'); return false;", imgProductDetailImage.ClientID, imgAdditionalImage.ClientID));
                }
                else
                {
                    imgAdditionalImage.Visible = false;
                }
            }
        }
        
    }
}