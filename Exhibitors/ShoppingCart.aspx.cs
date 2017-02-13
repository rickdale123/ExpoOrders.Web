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
    public partial class ShoppingCart : BaseExhibitorPage
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
            this.Master.OnNavigationItemCallBack = HandleNavigationItemClicked;

            if (!IsPostBack)
            {
                LoadPage();
            }

        }

        private void LoadPage()
        {
            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, ExhibitorPageEnum.ShoppingCart);
            this.Master.NoSubNavigation = true;

            DisplayCartItems();
        }

        private void DisplayCartItems()        
        {
            plcEmptyShoppingCart.Visible = false;
            
            Entities.ShoppingCart shoppingCart = Cntrl.GetUserCart(CurrentUser);

            if (shoppingCart != null && shoppingCart.ShoppingCartItems != null && shoppingCart.ShoppingCartItems.Count > 0)
            {
                CurrentUser.ShoppingCartItemCount = shoppingCart.TotalItemCount;
                this.Master.PopulateShoppingCart(CurrentUser);

                btnCheckOut.Visible = true;

                shoppingCart.CalculateItemSubTotals(this.CurrentUser.CurrentExhibitor, DateTime.Now);

                rptrShoppingCartItems.DataSource = shoppingCart.ShoppingCartItems.OrderBy(i1 => i1.Product.Category.SortOrder).ThenBy(i2 => i2.Product.SortOrder);
                rptrShoppingCartItems.DataBind();

                rptrShoppingCartItems.Visible = true;

                plcShoppingCartDetails.Visible = true;
                
                Literal ltrOrderTotal = (Literal) rptrShoppingCartItems.Controls[rptrShoppingCartItems.Controls.Count - 1].FindControl("ltrOrderTotal");
                if (ltrOrderTotal != null)
                {
                    ltrOrderTotal.Text = Server.HtmlEncode(Util.FormatCurrency(shoppingCart.OrderTotal.Value, CurrentUser.CurrentShow.CurrencySymbol));
                }
            }
            else
            {
                CurrentUser.ShoppingCartItemCount = 0;
                Master.PopulateShoppingCart(CurrentUser);
                rptrShoppingCartItems.Visible = false;
                plcEmptyShoppingCart.Visible = true;
                btnCheckOut.Visible = false;
            }
        }

        protected void rptrShoppingCartItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                ShoppingCartItem cartItem = e.Item.DataItem as ShoppingCartItem;

                //cartItem.CalculateCharges(CurrentUser.CurrentExhibitor, DateTime.Now);

                HtmlTableRow trShoppingCartItem = (HtmlTableRow)e.Item.FindControl("trShoppingCartItem");
                HtmlTableRow trItemAdditionalInfo = (HtmlTableRow)e.Item.FindControl("trItemAdditionalInfo");

                TextBox txtQuantity = (TextBox)e.Item.FindControl("txtQuantity");
                Label lblProductCategory = (Label)e.Item.FindControl("lblProductCategory");
                Label lblProductName = (Label)e.Item.FindControl("lblProductName");

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
                
                Label lblLateFees = (Label)e.Item.FindControl("lblLateFees");
                Label lblAdditionalCharges = (Label)e.Item.FindControl("lblAdditionalCharges");
                Label lblSalesTax = (Label)e.Item.FindControl("lblSalesTax");

                Label lblSubTotal = (Label)e.Item.FindControl("lblSubTotal");
                
                LinkButton lnkDeleteItem = (LinkButton)e.Item.FindControl("lnkDeleteItem");

                lblUnitPrice.CssClass = string.Empty;
                lblUnitPriceDescription.Text = cartItem.Product.DetermineCurrentUnitPriceDescription(DateTime.Now, true);

                trShoppingCartItem.Attributes["class"] =
                    trItemAdditionalInfo.Attributes["class"] = (e.Item.ItemType == ListItemType.AlternatingItem) ? "altItem" : "item";

                txtQuantity.Enabled = true;
                txtQuantity.Text = cartItem.Quantity.Value.ToString();

                lblProductCategory.Text = cartItem.Product.Category.CategoryName;
                lblProductName.Text = cartItem.Product.ProductName;

                plcRequiredAttribute1.Visible = plcRequiredAttribute2.Visible = false;

                if (cartItem.Product.CalcQuantityFromAttributesFlag.HasValue && cartItem.Product.CalcQuantityFromAttributesFlag.Value == true)
                {
                    txtQuantity.Enabled = false;
                }

                if(!string.IsNullOrEmpty(cartItem.RequiredAttribute1))
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


                lblUnitPrice.Text = Server.HtmlEncode(Util.FormatUnitPrice(cartItem.Product.DetermineCurrentUnitPrice(DateTime.Now, true), cartItem.Product.UnitDescriptor, CurrentUser.CurrentShow.CurrencySymbol));


                lblLateFees.Text = Server.HtmlEncode(Util.FormatCurrency(cartItem.LateFees, CurrentUser.CurrentShow.CurrencySymbol));
                lblAdditionalCharges.Text = Server.HtmlEncode(Util.FormatCurrency(cartItem.AdditionalCharges, CurrentUser.CurrentShow.CurrencySymbol));
                lblSalesTax.Text = Server.HtmlEncode(Util.FormatCurrency(cartItem.SalesTaxCharges, CurrentUser.CurrentShow.CurrencySymbol));
                lblSubTotal.Text = Server.HtmlEncode(Util.FormatCurrency(cartItem.SubTotalCharges, CurrentUser.CurrentShow.CurrencySymbol));

                lnkDeleteItem.CommandArgument = cartItem.ShoppingCartItemId.ToString();

                //Paint Additional Info
                trItemAdditionalInfo.Visible = false;
                if (cartItem.AdditionalInfoes.Count > 0)
                {
                    trItemAdditionalInfo.Visible = true;

                    Repeater rptItemAdditionalInfo = (Repeater)e.Item.FindControl("rptItemAdditionalInfo");

                    rptItemAdditionalInfo.Visible = true;
                    rptItemAdditionalInfo.DataSource = cartItem.AdditionalInfoes;
                    rptItemAdditionalInfo.DataBind();

                }

            }
        }

        protected void rptItemAdditionalInfo_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                AdditionalInfo additionalInfo = (AdditionalInfo)e.Item.DataItem;

                if (additionalInfo != null)
                {
                    Label lblAdditionalInfoQuestion = (Label)e.Item.FindControl("lblAdditionalInfoQuestion");
                    Label lblAdditionalInfoAnswer = (Label)e.Item.FindControl("lblAdditionalInfoAnswer");

                    lblAdditionalInfoQuestion.Text = additionalInfo.Question;
                    lblAdditionalInfoAnswer.Text = additionalInfo.Response;

                }
            }
        }
        protected void lnkDeleteItem_Click(object sender, EventArgs e)
        {
            if (sender is LinkButton)
            {
                LinkButton lnkDeleteItem = (LinkButton)sender;

                int shoppingCartItemId = Int32.Parse(lnkDeleteItem.CommandArgument);

                Cntrl.RemoveShoppingCartItem(shoppingCartItemId);
                DisplayCartItems();
            }
        }


        private bool UpdateShoppingCart()
        {
            bool updated = false;
            if (Page.IsValid)
            {
                Dictionary<int, decimal> itemQuantities = new Dictionary<int, decimal>();
                foreach (RepeaterItem item in this.rptrShoppingCartItems.Items)
                {
                    LinkButton lnkDeleteItem = (LinkButton)item.FindControl("lnkDeleteItem");
                    TextBox txtQuantity = (TextBox)item.FindControl("txtQuantity");
                    itemQuantities.Add(Int32.Parse(lnkDeleteItem.CommandArgument), Decimal.Parse(txtQuantity.Text.Trim()));
                }


                //Intentionally using a different instance of controller, so the updates will 'take'
                ShoppingCartController shoppingCartUpdateCntrl = new ShoppingCartController();

                ValidationResults updateValidations = shoppingCartUpdateCntrl.UpdateShoppingCartItems(this.CurrentUser, itemQuantities);
                if (updateValidations.IsValid)
                {
                    updated = true;
                }
                else
                {
                    this.PageErrors.Visible = true;
                    this.PageErrors.AddErrorMessages(updateValidations, "ShoppingCartUpdate");
                }
            }
            else
            {
                this.PageErrors.Visible = true;
            }

            return updated;
        }

        protected void btnUpdateShoppingCart_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "ShoppingCartUpdate";

            if (UpdateShoppingCart())
            {
                DisplayCartItems();
            }
            
        }

        protected void btnContinueShopping_Click(object sender, EventArgs e)
        {
            Server.Transfer("Booth.aspx", false);
        }

        protected void btnCheckOut_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "ShoppingCartUpdate";

            if (UpdateShoppingCart())
            {
                Server.Transfer("Payment.aspx", false);
            }
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            this.Master.DisplayFriendlyMessage("NavLinkId:" + navLinkId.ToString() + " Action: " + action.ToString() + " TargetId:" + targetId.ToString());
        }


        protected void btnBackToCart_Click(object sender, EventArgs e)
        {
            DisplayCartItems();
        }


        
    }
}