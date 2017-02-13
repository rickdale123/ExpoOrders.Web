using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using ExpoOrders.Common;

namespace ExpoOrders.Web.CustomControls
{
    public partial class CategoryItems : System.Web.UI.UserControl
    {

        private int _currentProductIndex = 0;
        private string _currentProductName = string.Empty;

        private ExpoOrdersUser _currentUser;
        private Action<int> _itemSelected;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        public Action<int> ItemSelected
        {
            set
            {
                _itemSelected = value;  
            }
        }

        public string ParentCategoryName
        {
            get
            {
                return ltrParentCategoryName.Text;
            }
        }

        public string CategoryName
        {
            get
            {
                return ltrCategoryName.Text;
            }
        }

        public int PopulateControl(ExpoOrdersUser currentUser, Category category, string parentCategoryName)
        {
            int numberProducts = 0;
            _currentUser = currentUser;
            if (category != null)
            {
                this.Visible = true;

                ltrParentCategoryName.Text = parentCategoryName;
                ltrCategoryName.Text = category.CategoryName;
                
                //Not going to use the Category Description, will be Headings from CategoryDisplay table
                //ltrCategoryDescription.Text = category.CategoryDescription;
                var products = category.Products.Where(p => p.ActiveFlag == true && p.VisibleToExhibitors == true).OrderBy(p => p.SortOrder).ToList();
                rptrProducts.DataSource = products;
                rptrProducts.DataBind();

                this.Visible = true;

                numberProducts = products.Count;
            }
            return numberProducts;
        }

        protected void btnItemDetail_Click(object sender, EventArgs e)
        {
            Button btnItemDetail = (Button)sender;

            if (btnItemDetail != null)
            {
                ProductTypeEnum prodType = Enum<ProductTypeEnum>.Parse(btnItemDetail.CommandName, true);

                int productId = Int32.Parse(btnItemDetail.CommandArgument);
                if (productId > 0)
                {
                    _itemSelected(productId);
                }
            }
        }

        protected void rptrProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                _currentProductIndex = e.Item.ItemIndex;
                _currentProductName = string.Empty;

                Product productDetail = (Product)e.Item.DataItem;

                ProductTypeEnum productType = Enum<ProductTypeEnum>.Parse(productDetail.ProductTypeCd, true);

                PlaceHolder plcHeading = (PlaceHolder)e.Item.FindControl("plcHeading");
                PlaceHolder plcProduct = (PlaceHolder)e.Item.FindControl("plcProduct");

                if (productType == ProductTypeEnum.SectionHeading)
                {
                    plcHeading.Visible = true;
                    plcProduct.Visible = false;

                    Literal ltrSectionHeadingName = (Literal)e.Item.FindControl("ltrSectionHeadingName");
                    Literal ltrSectionHeadingContent = (Literal)e.Item.FindControl("ltrSectionHeadingContent");

                    ltrSectionHeadingName.Text = productDetail.ProductName;
                    ltrSectionHeadingContent.Text = productDetail.ProductDescription;

                }
                else if (productType == ProductTypeEnum.Item || productType == ProductTypeEnum.FileDownload)
                {
                    
                    plcHeading.Visible = false;
                    plcProduct.Visible = true;

                    HtmlTable tblProduct = (HtmlTable)e.Item.FindControl("tblProduct");
                    Image imgProductImage = (Image)e.Item.FindControl("imgProductImage");
                    HtmlAnchor lnkProductImage = (HtmlAnchor)e.Item.FindControl("lnkProductImage");
                    PlaceHolder plcAdditionalImages = (PlaceHolder)e.Item.FindControl("plcAdditionalImages");
                    HtmlAnchor lnkMoreImages = (HtmlAnchor)e.Item.FindControl("lnkMoreImages");
                    Repeater rptrAdditionalImages = (Repeater)e.Item.FindControl("rptrAdditionalImages");
                    

                    Literal ltrProductName = (Literal)e.Item.FindControl("ltrProductName");
                    HtmlAnchor lnkProductDownload = (HtmlAnchor)e.Item.FindControl("lnkProductDownload");

                    Button btnItemDetail = (Button)e.Item.FindControl("btnItemDetail");

                    PlaceHolder plcSubmissionDeadline = (PlaceHolder)e.Item.FindControl("plcSubmissionDeadline");
                    Literal ltrSubmissionDeadline = (Literal)e.Item.FindControl("ltrSubmissionDeadline");

                    PricingLabels ucPricingLabels = (PricingLabels)e.Item.FindControl("ucPricingLabels");
                    
                    PlaceHolder plcProductDescription = (PlaceHolder)e.Item.FindControl("plcProductDescription");
                    Literal ltrProductDescription = (Literal)e.Item.FindControl("ltrProductDescription");

                    PlaceHolder plcWarningMessage = (PlaceHolder)e.Item.FindControl("plcWarningMessage");
                    Label lblWarningMessage = (Label)e.Item.FindControl("lblWarningMessage");


                    lnkProductDownload.Visible = ltrProductName.Visible = lblWarningMessage.Visible = false;
                    ucPricingLabels.Visible = false;

                    if (productDetail != null)
                    {
                        _currentProductName = productDetail.ProductName;
                        tblProduct.Attributes["class"] = (e.Item.ItemType == ListItemType.AlternatingItem) ? "altItem" : "item";

                        ProductTypeEnum prodType = ProductTypeEnum.NotSet;
                        if (!String.IsNullOrEmpty(productDetail.ProductTypeCd))
                        {
                            prodType = Enum<ProductTypeEnum>.Parse(productDetail.ProductTypeCd, true);
                        }

                        btnItemDetail.CommandName = prodType.ToString();
                        btnItemDetail.CommandArgument = productDetail.ProductId.ToString();

                        if (prodType == ProductTypeEnum.FileDownload)
                        {
                            if (productDetail.FileDownloadId.HasValue)
                            {
                                lnkProductDownload.Visible = true;
                                lnkProductDownload.InnerHtml = productDetail.ProductName;
                                lnkProductDownload.HRef = "#";
                                string downloadJavascript = string.Format("launchFileDownload({0}); return false;", productDetail.FileDownloadId.Value);
                                lnkProductDownload.Attributes.Add("onClick", downloadJavascript);

                                btnItemDetail.Attributes.Add("onClick", downloadJavascript);
                            }
                            else
                            {
                                btnItemDetail.Attributes.Add("OnClick", "return false;");
                            }
                        }
                        else
                        {
                            ltrProductName.Visible = true;
                            ltrProductName.Text = productDetail.ProductName;
                        }

                        //Populate the Wacky Pricing Labels based on Rollovers from advanced and various deadlines.
                        ucPricingLabels.Populate(_currentUser, productDetail);

                        ltrProductDescription.Text = productDetail.ProductDescription;

                        if (prodType == ProductTypeEnum.FileDownload)
                        {
                            ucPricingLabels.Visible = false;

                            btnItemDetail.Text = "Download";
                        }
                        else
                        {
                            btnItemDetail.Text = "Order";
                        }

                        if (!String.IsNullOrEmpty(productDetail.ImageName))
                        {
                            imgProductImage.Visible = lnkProductImage.Visible = true;
                            imgProductImage.ImageUrl = lnkProductImage.HRef = string.Format("~/Assets/Shows/{0}/Products/{1}", _currentUser.CurrentShow.ShowGuid, productDetail.ImageName);
                            imgProductImage.AlternateText = productDetail.ProductName;

                            lnkProductImage.Title = productDetail.ProductName;
                            lnkProductImage.Attributes.Add("rel", string.Format("thumbnailGroup_{0}", _currentProductIndex));

                        }
                        else
                        {
                            imgProductImage.Visible = lnkProductImage.Visible = false;
                        }

                        if (productDetail.ContainsAdditionalImages)
                        {
                            plcAdditionalImages.Visible = true;
                            rptrAdditionalImages.Visible = true;
                            lnkMoreImages.Visible = true;

                            lnkMoreImages.Attributes.Add("onClick", string.Format("showImageGroup({0}); return false;", _currentProductIndex));

                            string additionalImageText = productDetail.Category.Show.Owner.AdditionalImageLinkText;
                            if (!string.IsNullOrEmpty(additionalImageText))
                            {
                                lnkMoreImages.InnerText = additionalImageText;
                            }

                            rptrAdditionalImages.DataSource = productDetail.AllProductImages(false);
                            rptrAdditionalImages.DataBind();

                        }
                        else
                        {
                            plcAdditionalImages.Visible = false;
                            rptrAdditionalImages.Visible = false;
                            lnkMoreImages.Visible = false;
                        }

                        if (productDetail.SubmissionDeadline.HasValue)
                        {
                            plcSubmissionDeadline.Visible = ltrSubmissionDeadline.Visible = true;
                            ltrSubmissionDeadline.Text = productDetail.SubmissionDeadline.Value.ToShortDateString();

                            if (productDetail.IsPassedSubmissionDeadline(DateTime.Now))
                            {
                                LateSubmissionBehaviorEnum lateBehavior = Enum<LateSubmissionBehaviorEnum>.Parse(productDetail.LateBehaviorCd, true);

                                if (lateBehavior == LateSubmissionBehaviorEnum.Prevent)
                                {
                                    lnkProductDownload.Disabled = true;
                                    btnItemDetail.Visible = false;
                                    plcWarningMessage.Visible = lblWarningMessage.Visible = true;
                                    lblWarningMessage.Text = productDetail.LateMessage;
                                }
                                else if (lateBehavior == LateSubmissionBehaviorEnum.Warning)
                                {
                                    btnItemDetail.Visible = true;
                                    plcWarningMessage.Visible = lblWarningMessage.Visible = true;
                                    lblWarningMessage.Text = productDetail.LateMessage;
                                }
                            }
                        }
                        else
                        {
                            plcSubmissionDeadline.Visible = false;
                        }

                        if (!String.IsNullOrEmpty(productDetail.ProductDescription))
                        {
                            plcProductDescription.Visible = ltrProductDescription.Visible = true;
                            ltrProductDescription.Text = productDetail.ProductDescription;
                        }
                        else
                        {
                            plcProductDescription.Visible = false;
                        }
                    }
                }
            }
        }

        protected void rptrAdditionalImages_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string imageName = e.Item.DataItem.ToString();

                Image imgAdditionalImage = (Image)e.Item.FindControl("imgAdditionalImage");
                HtmlAnchor lnkAdditionalImage = (HtmlAnchor)e.Item.FindControl("lnkAdditionalImage");


                imgAdditionalImage.ImageUrl = lnkAdditionalImage.HRef = string.Format("~/Assets/Shows/{0}/Products/{1}", _currentUser.CurrentShow.ShowGuid, imageName);

                lnkAdditionalImage.Attributes.Add("rel", string.Format("thumbnailGroup_{0}", _currentProductIndex));

                lnkAdditionalImage.Title = _currentProductName;

            }
        }
    }
}