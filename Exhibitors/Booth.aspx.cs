using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using ExpoOrders.Entities;
using ExpoOrders.Common;
using ExpoOrders.Controllers;

using Microsoft.Practices.EnterpriseLibrary.Validation;
using ExpoOrders.Web.CustomControls;

namespace ExpoOrders.Web.Exhibitors
{
    public partial class Booth : BaseExhibitorPage
    {
        private List<Category> _ShowCategories = null;

        private int _nbrProducts = 0;
        public int NumberProducts
        {
            get { return _nbrProducts; }
            set { _nbrProducts = value; }
        }

        private BoothController _ctrl;
        private BoothController boothController
        {
            get
            {
                if (_ctrl == null)
                {
                    _ctrl = new BoothController();
                }
                return _ctrl;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ucItemDetail.BackToCategory = BackToCategory;
            ucCategoryItems.ItemSelected = CategoryItemSelected;
            ucItemDetail.ItemSelected = ItemDetailSelected;

            this.Master.OnNavigationItemCallBack = HandleNavigationItemClicked;

            //When a Category is clicked
            bool viewingCategory = (Request.QueryString["viewcat"] != null);
            //When a Category is being viewed from a different tab
            bool isTempNavRedirect = (TempNavTabRedirect != null && TempNavTabRedirect.TargetId.HasValue);

            if (viewingCategory == false && isTempNavRedirect == false)
            {
                this.Master.OnCategoryItemPainted = HandleCategoryNavItemPainted;
                this.Master.OnSubNavigationLoadComplete = HandleSubNavigationLoadComplete;
            }

            this.Master.DisplayCountDownTicker(CurrentUser.CurrentShow);

            if (!IsPostBack)
            {
                LoadPage(viewingCategory, isTempNavRedirect);
            }
        }

        private void LoadPage(bool viewCategory, bool isTempNavRedirect)
        {

            this.Master.LoadMasterPage(CurrentUser, CurrentUser.CurrentShow, ExhibitorPageEnum.MyBooth);

            if (viewCategory)
            {
                //Viewing a Category from the Selection
                LoadCategory(Util.ConvertInt32(Request.QueryString["viewcat"]), Request.QueryString["parent"]);
            }
            else if (isTempNavRedirect)
            {
                //Loading a Category from a different Tab
                LoadCategory(TempNavTabRedirect.TargetId.Value, TempNavTabRedirect.DisplayHeading);
                ResetNavTabRedirect();
            }
            else
            {
                //Loads current booth details beneath the Icons???
                AccountController ctrl = new AccountController();
                List<ExhibitorBoothItems> boothItems = null;
                if (!CurrentUser.CurrentExhibitor.IsPreviewMode)
                {
                    boothItems = ctrl.GetBoothItemSummary(CurrentUser);
                }
                if (boothItems == null || boothItems.Count <= 0)
                {
                    this.ucBoothDetails.Visible = false;
                }
                else
                {
                    this.ucBoothDetails.PopulateControl(CurrentUser, boothItems, CurrentUser.CurrentShow);
                }
            }
        }

        private string _LastCategoryHeading = string.Empty;
        private CategoryIconHeading _LastCategoryIconHeadingControl = null;

        private void HandleSubNavigationLoadComplete(int numberOfItems)
        {
            if (_LastCategoryIconHeadingControl != null)
            {
                //Force the last Icon heading to databind when the naviagion load is complete;
                _LastCategoryIconHeadingControl.FinalizeList(this.CurrentUser);
            }
        }
        private void HandleCategoryNavItemPainted(NavigationLink navLink)
        {
            int categoryId = navLink.TargetId.HasValue ? navLink.TargetId.Value : 0;
            string categoryDisplayText = navLink.LinkText;

            string categoryIconHeading = navLink.LinkText;
            if (navLink.ParentNavigationLink != null)
            {
                categoryIconHeading = navLink.ParentNavigationLink.LinkText;
            }

            if (categoryIconHeading != _LastCategoryHeading)
            {
                _LastCategoryHeading = categoryIconHeading;

                if (_LastCategoryIconHeadingControl != null)
                {
                    _LastCategoryIconHeadingControl.FinalizeList(this.CurrentUser);
                }
                _LastCategoryIconHeadingControl = null; //force a new one to be created
            }

            if (_ShowCategories == null)
            {
                ProductController prodCntrl = new ProductController();
                _ShowCategories = prodCntrl.GetCategoryList(CurrentUser.CurrentShow.ShowId);
            }

            Category theCategory = _ShowCategories.Where(c => c.CategoryId == categoryId).FirstOrDefault();
            if (theCategory != null)
            {
                if (!string.IsNullOrEmpty(theCategory.IconFileName))
                {
                    if (_LastCategoryIconHeadingControl == null)
                    {
                        CategoryIconHeading catHeadingCntrl = (CategoryIconHeading)LoadControl("~/CustomControls/CategoryIconHeading.ascx");
                        catHeadingCntrl.Heading = _LastCategoryHeading;
                        plcCategoryIcons.Controls.Add(catHeadingCntrl);
                        _LastCategoryIconHeadingControl = catHeadingCntrl;
                    }

                    _LastCategoryIconHeadingControl.AddCategoryIcon(theCategory.CategoryId, categoryDisplayText, theCategory.IconFileName);
                }
            }
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            WebUtil.HideForm(plcDynamicForm, ucFormQuestions);
            this.Master.HideDynamicContent();

            ucBoothDetails.Visible = false;
            ucCategoryItems.Visible = false;
            ucItemDetail.Visible = false;

            switch(action)
            {
                case NavigationLink.NavigationalAction.ShowCategory:
                    LoadCategory(targetId, this.Master.ParentNavigationText(navLinkId));
                    break;
                case NavigationLink.NavigationalAction.DynamicForm:
                    if (targetId > 0)
                    {
                        hdnFormId.Value = targetId.ToString();
                        WebUtil.DisplayForm(CurrentUser, this.FormMgr.GetFormInfoById(targetId), plcDynamicForm, ucFormQuestions, lblFormSubmissionDeadlineError, ltrFormName, ltrFormDescription, btnSubmitForm);
                    }
                    break;

                case NavigationLink.NavigationalAction.HtmlContent:
                    this.Master.DisplayDynamicContent(string.Empty, HtmlController.GetHtmlContentById(targetId));
                    break;
                case NavigationLink.NavigationalAction.OutboundShippingLabel:
                    RedirectOutboundShippingLabel(navLinkId, targetId);
                    break;
                default:
                    this.Master.DisplayFriendlyMessage("Unhandled NavLinkId:" + navLinkId.ToString() + " Action: " + action.ToString() + " TargetId:" + targetId.ToString());
                    break;

            }
        }

        private void LoadCategory(int categoryId, string parentCategoryName)
        {
            ucBoothDetails.Visible = false;
            ucCategoryItems.Visible = false;
            ucItemDetail.Visible = false;
            WebUtil.HideForm(plcDynamicForm, ucFormQuestions);

            Category category = boothController.GetCategoryById(categoryId);
            NumberProducts = this.ucCategoryItems.PopulateControl(base.CurrentUser, category, parentCategoryName);
        }

        private void CategoryItemSelected(int productId)
        {
            Product productDetail = this.boothController.GetProductById(productId);

            Master.HideDynamicContent();
            ucBoothDetails.Visible = false;
            ucCategoryItems.Visible = false;
            WebUtil.HideForm(plcDynamicForm, ucFormQuestions);

            Form additionalInfoForm = null;
            if (productDetail.AdditionalInfoFormId.HasValue && productDetail.AdditionalInfoFormId.Value > 0)
            {
                FormController formCtrl = new FormController();
                additionalInfoForm = formCtrl.GetFormInfoById(productDetail.AdditionalInfoFormId.Value);
            }
            ucItemDetail.PopulateControl(CurrentUser, productDetail, ucCategoryItems.ParentCategoryName, ucCategoryItems.CategoryName, additionalInfoForm);

        }

        private void BackToCategory(int categoryId, string parentCategoryName)
        {
            LoadCategory(categoryId, parentCategoryName);
        }

        private void ItemDetailSelected(int productId, ProductTypeEnum productType, decimal quantity)
        {

            if (CurrentUser.CurrentExhibitor.IsPreviewMode)
            {
               PageErrors.AddErrorMessage("Shopping Cart is disabled in 'Exibitor Preview Mode'.");
            }
            else
            {
                ShoppingCartItem newShoppingCartItem = new ShoppingCartItem();
                newShoppingCartItem.ProductId = productId;
                newShoppingCartItem.Quantity = quantity;

                List<string> missingRequiredQuestions = null;
                List<QuestionAnswer> questionAnswerList = this.ucItemDetail.GetAdditionalInfoResponses(ref missingRequiredQuestions);

                ucItemDetail.HighlightRequiredAttribute(1, false);
                ucItemDetail.HighlightRequiredAttribute(2, false);
                if (ucItemDetail.RequiresAttribute1())
                {
                    if (string.IsNullOrEmpty(ucItemDetail.RequiredAttribute1Selected))
                    {
                        ucItemDetail.HighlightRequiredAttribute(1, true);
                        PageErrors.AddErrorMessage(new ValidationResult(string.Format("Missing Required value for {0}.", ucItemDetail.RequiredAttribute1Label), null, null, null, null), "AddToCart");
                    }
                    else
                    {
                        newShoppingCartItem.RequiredAttribute1 = string.Format("{0}: {1}", ucItemDetail.RequiredAttribute1Label, ucItemDetail.RequiredAttribute1Selected);
                    }
                }

                if (ucItemDetail.RequiresAttribute2())
                {
                    if (string.IsNullOrEmpty(ucItemDetail.RequiredAttribute2Selected))
                    {
                        PageErrors.AddErrorMessage(new ValidationResult(string.Format("Missing Required value for {0}.", ucItemDetail.RequiredAttribute2Label), null, null, null, null), "AddToCart");
                        ucItemDetail.HighlightRequiredAttribute(2, true);
                    }
                    else
                    {
                        newShoppingCartItem.RequiredAttribute2 = string.Format("{0}: {1}", ucItemDetail.RequiredAttribute2Label, ucItemDetail.RequiredAttribute2Selected);
                    }
                }

                if (ucItemDetail.RequiresAttribute3())
                {
                    if (string.IsNullOrEmpty(ucItemDetail.RequiredAttribute3Selected))
                    {
                        PageErrors.AddErrorMessage(new ValidationResult(string.Format("Missing Required value for {0}.", ucItemDetail.RequiredAttribute3Label), null, null, null, null), "AddToCart");
                        ucItemDetail.HighlightRequiredAttribute(3, true);
                    }
                    else
                    {
                        newShoppingCartItem.RequiredAttribute3 = string.Format("{0}: {1}", ucItemDetail.RequiredAttribute3Label, ucItemDetail.RequiredAttribute3Selected);
                    }
                }

                if (ucItemDetail.RequiresAttribute4())
                {
                    if (string.IsNullOrEmpty(ucItemDetail.RequiredAttribute4Selected))
                    {
                        PageErrors.AddErrorMessage(new ValidationResult(string.Format("Missing Required value for {0}.", ucItemDetail.RequiredAttribute4Label), null, null, null, null), "AddToCart");
                        ucItemDetail.HighlightRequiredAttribute(4, true);
                    }
                    else
                    {
                        newShoppingCartItem.RequiredAttribute4 = string.Format("{0}: {1}", ucItemDetail.RequiredAttribute4Label, ucItemDetail.RequiredAttribute4Selected);
                    }
                }


                if (missingRequiredQuestions != null && missingRequiredQuestions.Count > 0)
                {
                    missingRequiredQuestions.ForEach(missing =>
                    {
                        PageErrors.AddErrorMessage(new ValidationResult(string.Format("A response to '{0}' is required.", missing), null, null, null, null), "AddToCart");
                    }
                        );
                }

                if (this.IsValid)
                {
                    ValidationResults results = boothController.AddShoppingCartItem(CurrentUser, newShoppingCartItem, questionAnswerList);

                    if (results.IsValid)
                    {
                        Server.Transfer("ShoppingCart.aspx", false);
                    }
                    else
                    {
                        PageErrors.AddErrorMessages(results, "AddToCart");
                    }
                }
            }
        }

        protected void btnSubmitForm_Click(object sender, EventArgs e)
        {
            List<string> missingRequiredQuestions = new List<string>();
            List<QuestionAnswer> questionAnswerList = this.ucFormQuestions.GetResponses(ref missingRequiredQuestions);

            if (missingRequiredQuestions.Count == 0)
            {
                OrderConfirmation confirmation = this.FormMgr.CreateFormOrder(CurrentUser, questionAnswerList, Util.ConvertInt32(hdnFormId.Value));

                if (confirmation != null && confirmation.Errors.IsValid)
                {
                    ConfirmationOrder = confirmation;

                    Server.Transfer("Orders.aspx", false);

                }
                else
                {
                    PageErrors.AddErrorMessage(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult("There was an error submitting your form. Please try again.", null, null, null, null), "FormSubmission");
                }
            }
            else
            {

                missingRequiredQuestions.ForEach(missing =>
                {
                    PageErrors.AddErrorMessage(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(string.Format("A response to '{0}' is required.", missing), null, null, null, null), "FormSubmission");
                }
                );
            }
        }
    }
}