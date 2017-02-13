#region using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using ExpoOrders.Web.Owners.Common;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using ExpoOrders.Web.CustomControls;
using AjaxControlToolkit;
using Telerik.Web.UI;

#endregion

namespace ExpoOrders.Web.Owners
{
    public partial class Orders : BaseOwnerPage
    {
        #region Private Members

        private OrderAdminController _cntrl;
        private OrderAdminController Cntrl
        {
            get
            {
                if (_cntrl == null)
                {
                    _cntrl = new OrderAdminController();
                }
                return _cntrl;
            }
        }
        #endregion

        #region Public Members
        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.NavigationItemCallBack = this.HandleNavigationItemClicked;
            this.Master.PreviewShowCallBack = this.PreviewShow;
            this.ucOrderDetails.AdjustOrder = this.AdjustOrder;

            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            ClearOtherPricing();

            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, OwnerPage.Orders, OwnerTabEnum.Orders);
            this.Master.LoadSubNavigation("Orders", OwnerUtil.BuildOrdersSubNavLinks());
            this.Master.SelectNavigationItem(1);

            if (LinkToOrderId > 0)
            {
                LoadOrderDetail(LinkToOrderId);
            }
            else if (LinkToExhibitorId > 0)
            {
                LoadOrderDetailFromExhibitorId(LinkToExhibitorId);
            }
            else
            {
                InitializeOrderSearch();
            }
        }

        private void LoadOrderDetailFromExhibitorId(int exhibitorId)
        {
            InitializeOrderSearch();
            SearchCriteria searchCriteria = new SearchCriteria();
            searchCriteria.ExhibitorId = exhibitorId;
            LoadOrderList(searchCriteria);
        }

        private void LoadOrderFilters()
        {
            if (CurrentUser.CurrentShow == null)
            {
                throw new Exception("Current Show is null.");
            }

            List<OrderStatusCount> orderFilters = Cntrl.GetOrderFilters(CurrentUser.CurrentShow.ShowId);

            DisplayFilterText(orderFilters, this.lnkBtnBoothOrderSubmitted, OrderTypeEnum.BoothOrder, OrderStatusEnum.Submitted);
            DisplayFilterText(orderFilters, this.lnkBtnBoothOrderAccepted, OrderTypeEnum.BoothOrder, OrderStatusEnum.Accepted);
            DisplayFilterText(orderFilters, this.lnkBtnBoothOrderRejected, OrderTypeEnum.BoothOrder, OrderStatusEnum.Rejected);
            DisplayFilterText(orderFilters, this.lnkBtnBoothOrderDeleted, OrderTypeEnum.BoothOrder, OrderStatusEnum.Deleted);

            DisplayFilterText(orderFilters, this.lnkBtnFormOrderSubmitted, OrderTypeEnum.Form, OrderStatusEnum.Submitted);
            DisplayFilterText(orderFilters, this.lnkBtnFormOrderAccepted, OrderTypeEnum.Form, OrderStatusEnum.Accepted);
            DisplayFilterText(orderFilters, this.lnkBtnFormOrderRejected, OrderTypeEnum.Form, OrderStatusEnum.Rejected);
            DisplayFilterText(orderFilters, this.lnkBtnFormOrderDeleted, OrderTypeEnum.Form, OrderStatusEnum.Deleted);

        }

        private void DisplayFilterText(List<OrderStatusCount> orderFilters, LinkButton lnkBtn, OrderTypeEnum orderTypeToFind, OrderStatusEnum orderStatusToFind)
        {
            orderFilters.ForEach(item =>
            {
                if (item.OrderStatus == orderStatusToFind
                    && item.OrderType == orderTypeToFind)
                {
                    lnkBtn.Text = string.Format("{0} - ({1})", orderStatusToFind.ToString(), item.NumberOfOrders);
                    lnkBtn.CommandArgument = string.Format("{0}:{1}", orderTypeToFind.ToString(), orderStatusToFind.ToString());
                    lnkBtn.Enabled = item.NumberOfOrders > 0;
                }
            });
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            InitializeOrderSearch();
        }

        private void InitializeOrderSearch()
        {
            plcOrderList.Visible = true;
            plcOrderDetail.Visible = false;
            LoadOrderFilters();

            LoadExhibitorSearchList(cboSearchExhibitorId);
        }

        private void LoadOrderList(SearchCriteria searchCriteria)
        {

            OrderAdminController cntrl = new OrderAdminController();

            lblExhibitorOrderListRowCount.Text = "0 rows to display.<br/>";

            List<Order> orders = cntrl.SearchExhibitorOrders(CurrentUser, CurrentUser.CurrentShow.ShowId, searchCriteria);

            if (orders != null)
            {
                lblExhibitorOrderListRowCount.Text = string.Format("({0} rows displayed.)<br/>", orders.Count);
            }

            grdvwExhibitorOrderList.DataSource = orders;
            grdvwExhibitorOrderList.DataBind();

            DisplayOrderList(true);

            btnAcceptSelectedOrders.Visible = btnAcceptSelectedOrders2.Visible = (orders != null && orders.Count > 0);
        }

        private void DisplayOrderList(bool visible)
        {
            grdvwExhibitorOrderList.Visible = 
                btnAcceptSelectedOrders.Visible =
                btnAcceptSelectedOrders2.Visible = visible;
        }

        private void LoadOrderDetail(int orderId)
        {
            cboJumpToOrderId.Visible = false;
            ltrOrderId.Text = string.Empty;
            plcApplyCreditCard.Visible = false;
            plcOrderAdjustment.Visible = false;
            plcOrderList.Visible = false;
            plcOrderDetail.Visible = true;

            ResetClassificationDropDown(plcClassification, ddlClassification);

            //Reset Order controls
            CurrentOrderId = orderId;
            hdnExhibitorId.Value = string.Empty;
            ucOrderDetails.Visible = false;
            plcOrderPaymentType.Visible = false;
            plcOrderUserId.Visible = false;
            plcOrderBillingAddress.Visible = false;

            txtOrderEmailAddress.Text = string.Empty;
            txtOrderBillingAddressLine1.Text = string.Empty;
            txtOrderBillingAddressLine2.Text = string.Empty;
            txtOrderBillingCity.Text = string.Empty;
            txtOrderBillingState.Text = string.Empty;
            txtOrderBillingPostalCode.Text = string.Empty;
            txtOrderBillingCountry.Text = string.Empty;

            if (orderId > 0)
            {
                OrderAdminController cntrl = new OrderAdminController();
                Order order = cntrl.GetExhibitorOrder(orderId);

                if (order != null)
                {
                    ltrOrderId.Text = order.OrderId.ToString();

                    LoadCreditCardDropdown(orderId);

                    PopulateUserList(order);

                    if (order.ExhibitorId.HasValue)
                    {
                        CurrentExhibitorId = order.ExhibitorId.Value;

                        SelectClassification(ddlClassification, order.Exhibitor.Classification);
                    }
                    

                    string url = string.Format("OrderHistory.aspx?orderid={0}", orderId);
                    lnkViewOrderHistory.Attributes.Add("onClick", string.Format("openPopupWindow('OrderHistory', '{0}', 600, 500); return false;", url));
                    lnkViewOrderHistory2.Attributes.Add("onClick", string.Format("openPopupWindow('OrderHistory', '{0}', 600, 500); return false;", url));

                    lnkBtnExhibitorName.Text = order.Exhibitor.ExhibitorCompanyName;
                    lnkBtnExhibitorName.CommandArgument = order.Exhibitor.ExhibitorId.ToString();

                    lnkPrintInvoice.Visible = true;
                    lnkPrintInvoice.Attributes.Add("onclick", string.Format("launchExhibitorInvoice({0}, {1}, {2}); return false;", (int)ReportEnum.ExhibitorInvoice, order.Exhibitor.ShowId, order.Exhibitor.ExhibitorId));
                    lbtnViewCallLogs.Visible = true;

                    PopulateOrderStatusItems();

                    if (order.OrderTypeCd == OrderTypeEnum.BoothOrder.ToString())
                    {
                        PopulatePaymentTypes();
                        plcOrderBillingAddress.Visible = true;

                        txtOrderEmailAddress.Text = order.OrderEmailAddress;
                        txtOrderBillingAddressLine1.Text = order.BillingAddressStreet1;
                        txtOrderBillingAddressLine2.Text = order.BillingAddressStreet2;
                        txtOrderBillingCity.Text = order.BillingAddressCity;
                        txtOrderBillingState.Text = order.BillingAddressStateProvinceRegion;
                        txtOrderBillingPostalCode.Text = order.BillingAddressPostalCode;
                        txtOrderBillingCountry.Text = order.BillingAddressCountry;
                    }

                    ucOrderDetails.Populate(cntrl, order, true, CurrentUser.CurrentShow.CurrencySymbol, true, true, CurrentUser.CurrentShow);
                    WebUtil.SelectListItemByValue(ddlOrderStatus, order.OrderStatusCd);
                    WebUtil.SelectListItemByValue(ddlPaymentType, order.PaymentTypeCd);

                    if (order.PaymentTypeCd == PaymentTypeEnum.CreditCard.ToString())
                    {
                        plcApplyCreditCard.Visible = true;

                        if (string.IsNullOrEmpty(order.CreditCardNumberMasked))
                        {
                            ltrApplyCreditCardLabel.Text = "Apply New Credit Card";
                        }
                        else
                        {
                            ltrApplyCreditCardLabel.Text = "Change Credit Card";
                        }
                    }

                    List<Order> otherOrders = order.Exhibitor.Orders.Where(o => o.OrderId != orderId && o.ActiveFlag == true).ToList();

                    if (otherOrders != null && otherOrders.Count > 0)
                    {
                        cboJumpToOrderId.DataTextField = "OrderId";
                        cboJumpToOrderId.DataValueField = "OrderId";
                        cboJumpToOrderId.DataSource = otherOrders;
                        cboJumpToOrderId.DataBind();

                        cboJumpToOrderId.Items.Insert(0, new RadComboBoxItem("[ View Order# ]", ""));
                        cboJumpToOrderId.Visible = true;
                    }
                }
            }
        }

        private void PopulateOrderStatusItems()
        {
            ddlOrderStatus.Items.Clear();
            ddlOrderStatus.Enabled = true;
            ddlOrderStatus.Visible = true;
            
            ddlOrderStatus.Items.Add(new ListItem("-- Select One --", OrderStatusEnum.NotSet.ToString()));
            ddlOrderStatus.Items.Add(new ListItem("Submitted", OrderStatusEnum.Submitted.ToString()));
            ddlOrderStatus.Items.Add(new ListItem("Accepted", OrderStatusEnum.Accepted.ToString()));
            ddlOrderStatus.Items.Add(new ListItem("Rejected", OrderStatusEnum.Rejected.ToString()));
            ddlOrderStatus.Items.Add(new ListItem("Deleted", OrderStatusEnum.Deleted.ToString()));
        }

        private void PopulateUserList(Order orderDetail)
        {
            plcOrderUserId.Visible = true;
            ddlOrderUserId.Items.Clear();

            orderDetail.Exhibitor.Users.ForEach(u =>
                {
                    ddlOrderUserId.Items.Add(new ListItem(u.PersonName, u.UserId.ToString()));
                }
            );

            //Add in the Admin Owners
            OwnerAdminController ownerAdminMgr = new OwnerAdminController();
            List<UserContainer> owners = ownerAdminMgr.GetOwnerUsers(CurrentUser);
            if (owners != null)
            {
                foreach (UserContainer owner in owners)
                {
                    ddlOrderUserId.Items.Add(new ListItem(string.Format("{0} {1}", owner.FirstName, owner.LastName), owner.UserId.ToString()));
                }
            }

            ListItem li = ddlOrderUserId.Items.FindByValue(orderDetail.UserId.ToString());
            if (li != null)
            {
                li.Selected = true;
            }

        }

        private void PopulatePaymentTypes()
        {
            plcOrderPaymentType.Visible = ddlPaymentType.Visible = true;
            ddlPaymentType.Items.Clear();

            ddlPaymentType.Items.Clear();
            ddlPaymentType.Items.Add(new ListItem("Credit Card", PaymentTypeEnum.CreditCard.ToString()));
            ddlPaymentType.Items.Add(new ListItem("Check", PaymentTypeEnum.Check.ToString()));
            ddlPaymentType.Items.Add(new ListItem("Wire Transfer", PaymentTypeEnum.Wire.ToString()));
            ddlPaymentType.Items.Add(new ListItem("Exhibitor Fund Transfer", PaymentTypeEnum.EEFT.ToString()));
            ddlPaymentType.Items.Add(new ListItem("None", PaymentTypeEnum.None.ToString()));

            ddlPaymentType.Items.Insert(0, new ListItem("", ""));
        }
        

        #endregion

        #region Control Events

        protected void grdvwExhibitorOrderList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Order currentOrder = (Order)e.Row.DataItem;

                OrderStatusEnum currentOrderStatus = Enum<OrderStatusEnum>.Parse(currentOrder.OrderStatusCd);

                CheckBox chkAcceptOrder = (CheckBox) e.Row.FindControl("chkAcceptOrder");
                HiddenField hdnRowOrderId = (HiddenField)e.Row.FindControl("hdnRowOrderId");

                hdnRowOrderId.Value = currentOrder.OrderId.ToString();

                if (currentOrderStatus == OrderStatusEnum.Submitted)
                {
                    chkAcceptOrder.Visible = true;
                }
                else
                {
                    chkAcceptOrder.Visible = false;
                }
            }
        }


        protected void grdvwExhibitorOrderList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int orderId = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "EditOrder":
                    LoadOrderDetail(orderId);
                    break;
            }
        }

        private void SaveOrderLineItemAdjustment()
        {
            DateTime now = DateTime.Now;

            OrderItem orderItem = new OrderItem();
            orderItem.ActiveFlag = true;
            orderItem.OrderId = CurrentOrderId;
            orderItem.OrderItemId = Util.ConvertInt32(hdnOrderItemId.Value);
            orderItem.ItemTypeCd = hdnItemTypeCd.Value;

            if (!string.IsNullOrEmpty(ddlCategoryId.SelectedValue))
            {
                orderItem.CategoryName = ddlCategoryId.SelectedItem.Text;
                orderItem.CategoryId = Util.ConvertInt32(ddlCategoryId.SelectedValue);
            }

            if (!string.IsNullOrEmpty(ddlProductId.SelectedValue))
            {
                orderItem.ProductId = Util.ConvertInt32(ddlProductId.SelectedValue);
            }

            orderItem.ProductSku = txtProductSku.Text.Trim();
            orderItem.ItemDescription = txtOrderItemName.Text.Trim();
            orderItem.ItemIndex = Util.ConvertInt32(txtOrderItemIndex.Text.Trim());
            orderItem.EarlyBirdFlag = false;
            orderItem.Quantity = Util.ConvertDecimal(txtOrderItemQuantity.Text.Trim());
            orderItem.UnitPrice = Util.ConvertDecimal(txtOrderItemUnitPrice.Text.Trim());
            orderItem.UnitDescriptor = txtOrderItemUnitDescriptor.Text.Trim();
            orderItem.UnitPriceDescription = txtOrderItemUnitPriceDescription.Text.Trim();
            orderItem.AdditionalChargeAmount = Util.ConvertDecimal(txtOrderItemAdditionalChargeAmount.Text.Trim());
            orderItem.LateFeeAmount = Util.ConvertDecimal(txtOrderItemLateFeeAmount.Text.Trim());
            orderItem.SalesTaxAmount = Util.ConvertDecimal(txtOrderItemSalesTaxAmount.Text.Trim());

            orderItem.RequiredAttribute1 = cboOrderItemRequiredAttribute1.Text.Trim();
            orderItem.RequiredAttribute2 = cboOrderItemRequiredAttribute2.Text.Trim();
            orderItem.RequiredAttribute3 = cboOrderItemRequiredAttribute3.Text.Trim();
            orderItem.RequiredAttribute4 = cboOrderItemRequiredAttribute4.Text.Trim();

            orderItem.Notes = txtOrderItemNotes.Text.Trim() + Environment.NewLine + lblOrderAuditTrail.Text;
            orderItem.ExhibitorNotes = txtOrderItemExhibitorNotes.Text.Trim();
            orderItem.UpdateBy = CurrentUser.DisplayUserName;
            orderItem.UpdateDateTime = now;
            orderItem.InsertDateTime = now;
            orderItem.InsertUserId = CurrentUser.UserId;

            List<Response> additionalInfoResponses = null;

            if (plcAdditionalInfoResponses.Visible)
            {
                if (rptrAdditionalInfoQuestions.Visible && rptrAdditionalInfoQuestions.Items.Count > 0)
                {
                    additionalInfoResponses = new List<Response>();
                    foreach (RepeaterItem item in rptrAdditionalInfoQuestions.Items)
                    {
                        TextBox txtQuestion = (TextBox)item.FindControl("txtQuestion");
                        TextBox txtResponse = (TextBox)item.FindControl("txtResponse");
                        additionalInfoResponses.Add(new Response() { Question = txtQuestion.Text.Trim(), ResponseText = txtResponse.Text.Trim() });
                    }
                }
            }

            ValidationResults errors = this.Cntrl.SaveOrderItem(CurrentUser, orderItem, additionalInfoResponses);

            if (errors.IsValid)
            {
                this.Master.DisplayFriendlyMessage("Order Item Saved.");
                LoadOrderDetail(CurrentOrderId);
            }
            else
            {
                this.PageErrors.AddErrorMessages(errors);
            }
        }

        private void LoadCreditCardDropdown(int orderId)
        {
            cboApplyCreditCardId.ClearSelection();
            cboApplyCreditCardId.Items.Clear();

            AccountController AccountMgr = new AccountController(CurrentUser);
            cboApplyCreditCardId.Visible = true;
            cboApplyCreditCardId.DataValueField = "CreditCardId";
            cboApplyCreditCardId.DataTextField = "CardListDisplay";
            cboApplyCreditCardId.DataSource = AccountMgr.GetCreditCardListByOrder(orderId);
            cboApplyCreditCardId.DataBind();

            cboApplyCreditCardId.Items.Insert(0, new ListItem("-- Select One --", string.Empty));

        }

        protected void btnApplyCreditCard_Click(object sender, EventArgs e)
        {
            ValidationResults errors = Cntrl.ApplyCreditCardToOrder(CurrentUser, CurrentOrderId, Util.ConvertInt32(cboApplyCreditCardId.SelectedValue));

            if (errors.IsValid)
            {
                Master.DisplayFriendlyMessage("Credit Card Details applied.");
                LoadOrderDetail(CurrentOrderId);
            }
            else
            {
                PageErrors.AddErrorMessages(errors);
            }
        }

        protected void btnSaveOrderItemNoCalc_Click(object sender, EventArgs e)
        {
            SaveOrderLineItemAdjustment();
        }

        protected void btnSaveOrderItem_Click(object sender, EventArgs e)
        {
            RecalculateAdditionalCharge();
            RecalculateLateFee();
            RecalculateSalesTax();

            SaveOrderLineItemAdjustment();
        }

        private void SaveOrderDetail(bool returnToList)
        {
            OrderAdminController cntrl = new OrderAdminController();
            Order order = new Order();
            order.OrderId = CurrentOrderId;

            order.ExhibitorId = CurrentExhibitorId;

            string exhibitorClassification = null;
            if (plcClassification.Visible && ddlClassification.Visible)
            {
                exhibitorClassification = ddlClassification.SelectedValue;
            }
            
            order.OrderStatusCd = ddlOrderStatus.SelectedValue;
            order.UserId = new Guid(ddlOrderUserId.SelectedValue);

            if (plcOrderPaymentType.Visible && ddlPaymentType.Visible)
            {
                order.OrderTypeCd = OrderTypeEnum.BoothOrder.ToString();
                order.PaymentTypeCd = ddlPaymentType.SelectedValue;

                order.OrderEmailAddress = txtOrderEmailAddress.Text.Trim();
                order.BillingAddressStreet1 = txtOrderBillingAddressLine1.Text.Trim();
                order.BillingAddressStreet2 = txtOrderBillingAddressLine2.Text.Trim();
                order.BillingAddressCity = txtOrderBillingCity.Text.Trim();
                order.BillingAddressStateProvinceRegion = txtOrderBillingState.Text.Trim();
                order.BillingAddressPostalCode = txtOrderBillingPostalCode.Text.Trim();
                order.BillingAddressCountry = txtOrderBillingCountry.Text.Trim();
            }
            else
            {
                order.OrderTypeCd = OrderTypeEnum.Form.ToString();
            }

            ValidationResults errors = cntrl.UpdateOrderDetails(CurrentUser, order, exhibitorClassification);

            if (errors.IsValid)
            {
                if (returnToList)
                {
                    InitializeOrderSearch();
                }
                else
                {
                    LoadOrderDetail(CurrentOrderId);
                }
                
                this.Master.DisplayFriendlyMessage("Order Saved!");
            }
            else
            {
                this.PageErrors.AddErrorMessages(errors);
            }
        }

        protected void btnSaveOrderDetail_Click(object sender, EventArgs e)
        {
            SaveOrderDetail(false);
        }

        protected void btnSaveOrderDetailReturn_Click(object sender, EventArgs e)
        {
            SaveOrderDetail(true);
        }

        protected void btnCancelOrderDetail_Click(object sender, EventArgs e)
        {
            InitializeOrderSearch();
        }

        protected void lnkBtnOrderFilter_Click(object sender, EventArgs e)
        {
            if (sender is LinkButton)
            {
                string commandArg = ((LinkButton)sender).CommandArgument;

                SearchCriteria searchCriteria = new SearchCriteria();
                searchCriteria.OrderType = Enum<OrderTypeEnum>.Parse(commandArg.Split(':')[0].Trim());
                searchCriteria.OrderStatus = Enum<OrderStatusEnum>.Parse(commandArg.Split(':')[1].Trim());

                LoadOrderList(searchCriteria);
            }
        }

        protected void btnSearchOrders_Click(object sender, EventArgs e)
        {
            SearchCriteria searchCriteria = new SearchCriteria();
            searchCriteria.BoothNumber = txtSearchBoothNumber.Text.Trim();
            searchCriteria.OrderId = Util.ConvertInt32(txtSearchOrderNumber.Text.Trim());

            if (cboSearchExhibitorId.SelectedIndex > 0)
            {
                searchCriteria.ExhibitorId = Util.ConvertInt32(cboSearchExhibitorId.SelectedValue);
            }
            else
            {
                searchCriteria.ExhibitorId = Util.ConvertInt32(txtSearchExhibitorId.Text.Trim());
            }
            LoadOrderList(searchCriteria);
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearchExhibitorId.Text = txtSearchBoothNumber.Text = txtSearchOrderNumber.Text = string.Empty;
            cboSearchExhibitorId.SelectedIndex = 0;
            DisplayOrderList(false);
        }

        protected void btnViewPayments_Click(object sender, EventArgs e)
        {
            LinkToOrderId = CurrentOrderId;
            Server.Transfer("Payments.aspx", false);
        }

      
        private void AdjustOrder(int orderId, int orderItemId, LineItemAdjustmentEnum mode)
        {
            if (mode == LineItemAdjustmentEnum.Delete)
            {
                DeleteOrderLineItem(orderId, orderItemId);
            }
            else if (mode == LineItemAdjustmentEnum.Add)
            {
                LoadOrderAdjustment(orderId, 0);
            }
            else if (mode == LineItemAdjustmentEnum.Edit)
            {
                LoadOrderAdjustment(orderId, orderItemId);
            }
        }

        private void DeleteOrderLineItem(int orderId, int orderItemId)
        {
            ValidationResults errors = this.Cntrl.DeleteOrderLineItem(CurrentUser, orderId, orderItemId);

            if (errors.IsValid)
            {
                LoadOrderDetail(orderId);
            }
            else
            {
                this.PageErrors.AddErrorMessages(errors);
            }
        }

        private void LoadOrderAdjustment(int orderId, int orderItemId)
        {
            txtSalesTax.Text = (0).ToString();
            if (CurrentUser.CurrentShow.DefaultSalesTax.HasValue)
            {
                txtSalesTax.Text = (CurrentUser.CurrentShow.DefaultSalesTax.Value * 100).ToString();
            }
            
            plcOrderAdjustment.Visible = true;
            plcAdditionalInfoResponses.Visible = false;
            plcOrderList.Visible = false;
            plcOrderDetail.Visible = false;
            ddlProductId.Visible = ddlCategoryId.Visible = true;

            ClearAttributeSelections();
            ClearOtherPricing();

            Order order = Cntrl.GetExhibitorOrder(orderId);
            if (order != null)
            {
                lblCompanyName.Text = order.Exhibitor.ExhibitorCompanyName;
                ltrOrderNumber.Text = string.Format("Order #{0}", order.OrderId);

                CurrentOrderId = order.OrderId;

                if (orderItemId == 0)
                {
                    ltrOrderAdjustmentMode.Text = "Add New Line Item";
                    hdnOrderItemId.Value = "0";

                    LoadCategoryDropdown(0);
                    LoadProductDropdown(0, 0);

                    hdnItemTypeCd.Value = OrderItemType.Product.ToString();
                    txtProductSku.Text = string.Empty;
                    txtOrderItemName.Text = string.Empty;
                    txtOrderItemName.ReadOnly = false;
                    txtOrderItemIndex.Text = string.Empty;
                    txtOrderItemQuantity.Text = string.Empty;
                    txtOrderItemUnitPrice.Text = string.Empty;
                    txtOrderItemUnitDescriptor.Text = string.Empty;
                    txtOrderItemUnitPriceDescription.Text = string.Empty;
                    txtOrderItemAdditionalChargeAmount.Text = string.Empty;
                    txtOrderItemLateFeeAmount.Text = string.Empty;
                    txtOrderItemSalesTaxAmount.Text = string.Empty;

                    cboOrderItemRequiredAttribute1.Text = string.Empty;
                    cboOrderItemRequiredAttribute2.Text = string.Empty;
                    cboOrderItemRequiredAttribute3.Text = string.Empty;
                    cboOrderItemRequiredAttribute4.Text = string.Empty;

                    txtOrderItemNotes.Text = string.Empty;
                    txtOrderItemExhibitorNotes.Text = string.Empty;
                    lblOrderAuditTrail.Text = string.Format("{0}: Line Item Added by {1}", DateTime.Now, CurrentUser.DisplayUserName);
                        
                }
                else
                {
                    ltrOrderAdjustmentMode.Text = "Adjust Existing Line Item";

                    OrderItem orderItemToAdjust = order.OrderItems.FirstOrDefault(oi => oi.OrderItemId == orderItemId);
                    if (orderItemToAdjust != null)
                    {
                        int selectedCategoryId = 0;
                        int selectedProductId = 0;

                        if (orderItemToAdjust.CategoryId.HasValue)
                        {
                            selectedCategoryId = orderItemToAdjust.CategoryId.Value;
                        }

                        if (orderItemToAdjust.ProductId.HasValue)
                        {
                            selectedProductId = orderItemToAdjust.ProductId.Value;
                        }

                        LoadCategoryDropdown(selectedCategoryId);
                        LoadProductDropdown(selectedCategoryId, selectedProductId);

                        PaintOtherPricing(orderItemToAdjust.Product);

                        hdnOrderItemId.Value = orderItemToAdjust.OrderItemId.ToString();
                        hdnItemTypeCd.Value = orderItemToAdjust.ItemTypeCd;
                        txtProductSku.Text = orderItemToAdjust.ProductSku;
                        
                        txtOrderItemName.Text = orderItemToAdjust.ItemDescription;
                        if (selectedProductId > 0)
                        {
                            txtOrderItemName.ReadOnly = true;
                        }
                        txtOrderItemIndex.Text = orderItemToAdjust.ItemIndex.ToString();
                        //chkEarlyBirdFlag.Checked = (orderItemToAdjust.EarlyBirdFlag.HasValue && orderItemToAdjust.EarlyBirdFlag.Value == true);
                        txtOrderItemQuantity.Text = orderItemToAdjust.Quantity.ToString();
                        txtOrderItemUnitPrice.Text = orderItemToAdjust.UnitPriceDisplayFormat;
                        txtOrderItemUnitDescriptor.Text = orderItemToAdjust.UnitDescriptor;
                        txtOrderItemUnitPriceDescription.Text = orderItemToAdjust.UnitPriceDescription;
                        txtOrderItemAdditionalChargeAmount.Text = orderItemToAdjust.AdditionalChargeAmountDisplayFormat;
                        txtOrderItemLateFeeAmount.Text = orderItemToAdjust.LateFeeAmountDisplayFormat;
                        txtOrderItemSalesTaxAmount.Text = orderItemToAdjust.SalesTaxAmountDisplayFormat;

                        cboOrderItemRequiredAttribute1.Text = orderItemToAdjust.RequiredAttribute1;
                        cboOrderItemRequiredAttribute2.Text = orderItemToAdjust.RequiredAttribute2;
                        cboOrderItemRequiredAttribute3.Text = orderItemToAdjust.RequiredAttribute3;
                        cboOrderItemRequiredAttribute4.Text = orderItemToAdjust.RequiredAttribute4;

                        txtOrderItemNotes.Text = string.Empty;
                        txtOrderItemExhibitorNotes.Text = orderItemToAdjust.ExhibitorNotes;
                        lblOrderAuditTrail.Text = string.Format("{0}: Line Item Modified by {1}", DateTime.Now, CurrentUser.DisplayUserName);

                        LoadProductPricingDetails(orderItemToAdjust.Product, false);

                        if (orderItemToAdjust.Responses != null && orderItemToAdjust.Responses.Count > 0)
                        {
                            List<Response> additionalInfoResponses = new List<Response>();
                            orderItemToAdjust.Responses.OrderBy(r => r.SortOrder).ForEach(r =>
                            {
                                additionalInfoResponses.Add(new Response(){ Question = r.Question, ResponseText= r.ResponseText});

                            });
                            rptrAdditionalInfoQuestions.DataSource = additionalInfoResponses;
                            rptrAdditionalInfoQuestions.DataBind();
                            plcAdditionalInfoResponses.Visible = true;
                        }
                    }
                }   
            }
            else
            {
                this.PageErrors.AddErrorMessage("Missing Order for Id: " + orderId.ToString());
            }
        }


        private void LoadProductPricingDetails(Product productDetail, bool repopulateOriginalAdditionalResponses)
        {
            int exhibitorId = CurrentExhibitorId;

            txtSalesTax.Text = (0).ToString();
            lblDiscountDeadline.Text = string.Empty;
            lblLateFeeDeadline.Text = string.Empty;
            hdnLateFeeDeadline.Value = string.Empty;

            ltrAdditionalChargeType.Text = string.Empty;
            hdnAdditionalChargeAmt.Value = (0).ToString();
            hdnAdditionalChargeType.Value = string.Empty;

            ltrLateFee.Text = string.Empty;
            hdnLateFeeAmt.Value = (0).ToString();
            hdnLateFeeType.Value = (0).ToString();

            plcAdditionalInfoResponses.Visible = false;

            Exhibitor exhibitor = Cntrl.GetExhibitorById(exhibitorId);

            ClearAttributeSelections();

            if (productDetail != null)
            {
                if (productDetail.DiscountDeadline.HasValue)
                {
                    lblDiscountDeadline.Text = productDetail.DiscountDeadline.Value.ToShortDateString();
                }

                if (productDetail.LateFeeDeadline.HasValue)
                {
                    lblLateFeeDeadline.Text = hdnLateFeeDeadline.Value = productDetail.LateFeeDeadline.Value.ToShortDateString();
                }

                if (productDetail.AdditionalChargesApply())
                {
                    AdditionalChargeTypeEnum additionalChargeType = Enum<AdditionalChargeTypeEnum>.Parse(productDetail.AdditionalChargeType, true);
                    string additionalChargeDisplay = string.Empty;
                    switch (additionalChargeType)
                    {
                        case AdditionalChargeTypeEnum.Flat:
                            additionalChargeDisplay = Server.HtmlEncode(Util.FormatUnitPrice(productDetail.AdditionalChargeAmount.Value, string.Empty, CurrentUser.CurrentShow.CurrencySymbol));
                            break;
                        case AdditionalChargeTypeEnum.PerUnit:
                            additionalChargeDisplay = Server.HtmlEncode(Util.FormatUnitPrice(productDetail.AdditionalChargeAmount.Value, "ea", CurrentUser.CurrentShow.CurrencySymbol));
                            break;
                        case AdditionalChargeTypeEnum.PctTotal:
                            additionalChargeDisplay = Util.FormatPercentage(productDetail.AdditionalChargeAmount);
                            break;
                        default:
                            break;
                    }


                    ltrAdditionalChargeType.Text = additionalChargeDisplay;
                    hdnAdditionalChargeAmt.Value = productDetail.AdditionalChargeAmount.Value.ToString();
                    hdnAdditionalChargeType.Value = productDetail.AdditionalChargeType.ToString();
                }


                //late fees
                if (productDetail.LateFeesApply(DateTime.Now))
                {
                    LateFeeTypeEnum lateFeeType = Enum<LateFeeTypeEnum>.Parse(productDetail.LateFeeType, true);
                    string lateFeeDisplay = string.Empty;
                    switch (lateFeeType)
                    {
                        case LateFeeTypeEnum.Flat:
                            lateFeeDisplay = Server.HtmlEncode(Util.FormatUnitPrice(productDetail.LateFeeAmount.Value, string.Empty, CurrentUser.CurrentShow.CurrencySymbol));
                            break;
                        case LateFeeTypeEnum.PerUnit:
                            lateFeeDisplay = Server.HtmlEncode(Util.FormatUnitPrice(productDetail.LateFeeAmount.Value, "ea", CurrentUser.CurrentShow.CurrencySymbol));
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

                if (exhibitor != null && exhibitor.TaxExemptFlag)
                {
                    txtSalesTax.Text = "(exempt)";
                }
                else
                {
                    if (productDetail.TaxExemptFlag.HasValue && productDetail.TaxExemptFlag.Value == true)
                    {
                        txtSalesTax.Text = "(exempt)";
                    }
                    else
                    {
                        if (productDetail.SalesTaxPercent.HasValue)
                        {
                            //lblSalesTax.Text = Util.FormatPercentage(productDetail.SalesTaxPercent);
                            txtSalesTax.Text = (productDetail.SalesTaxPercent.Value * 100).ToString();
                        }
                        else
                        {
                            txtSalesTax.Text = "0";
                        }
                    }
                }

                if (productDetail.AdditionalInfoFormId.HasValue && productDetail.AdditionalInfoFormId.Value > 0)
                {
                    FormController formCtrl = new FormController();
                    Form additionalInfoForm = formCtrl.GetFormInfoById(productDetail.AdditionalInfoFormId.Value);
                    if (additionalInfoForm != null && additionalInfoForm.Questions != null)
                    {
                        List<Response> questionList = new List<Response>();

                        additionalInfoForm.Questions.OrderBy(q => q.SortOrder).ForEach(q =>
                            {
                                questionList.Add(new Response() {Question = q.QuestionText, ResponseText = string.Empty});
                            });

                        rptrAdditionalInfoQuestions.DataSource = questionList;
                        rptrAdditionalInfoQuestions.DataBind();

                        plcAdditionalInfoResponses.Visible = true;

                        if (repopulateOriginalAdditionalResponses)
                        {
                            RepopulateOriginalAdditionalResponses();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(productDetail.RequiredAttribute1ChoiceList))
                {
                    BuildAttributeList(cboOrderItemRequiredAttribute1, productDetail.RequiredAttribute1Label, productDetail.RequiredAttribute1ChoiceList);
                }

                if (!string.IsNullOrEmpty(productDetail.RequiredAttribute2ChoiceList))
                {
                    BuildAttributeList(cboOrderItemRequiredAttribute2, productDetail.RequiredAttribute2Label, productDetail.RequiredAttribute2ChoiceList);
                }

                if (!string.IsNullOrEmpty(productDetail.RequiredAttribute3ChoiceList))
                {
                    BuildAttributeList(cboOrderItemRequiredAttribute3, productDetail.RequiredAttribute3Label, productDetail.RequiredAttribute3ChoiceList);
                }

                if (!string.IsNullOrEmpty(productDetail.RequiredAttribute4ChoiceList))
                {
                    BuildAttributeList(cboOrderItemRequiredAttribute4, productDetail.RequiredAttribute4Label, productDetail.RequiredAttribute4ChoiceList);
                }
            }
        }

        private void RepopulateOriginalAdditionalResponses()
        {
            //Puts the original AdditionalInfo responses into the new Repeater when user changes the product type
            // connie: 7/12/2012: 
            if (rptrAdditionalInfoQuestions != null)
            {
                if (rptrAdditionalInfoQuestions.Items.Count > 0)
                {
                    int orderId = Util.ConvertInt32(hdnOrderId.Value);
                    int orderItemId = Util.ConvertInt32(hdnOrderItemId.Value);

                    if (orderId > 0 && orderItemId > 0)
                    {
                        Order order = Cntrl.GetExhibitorOrder(orderId);
                        OrderItem orderItemToAdjust = order.OrderItems.FirstOrDefault(oi => oi.OrderItemId == orderItemId);

                        if (orderItemToAdjust != null)
                        {
                            if (orderItemToAdjust.Responses != null && orderItemToAdjust.Responses.Count > 0)
                            {
                                int itemIndex = 0;
                                List<Response> originalAdditionalInfoResponses = new List<Response>();
                                orderItemToAdjust.Responses.OrderBy(r => r.SortOrder).ForEach(r =>
                                {
                                    PopulateAdditionalInfoResponse(itemIndex, r.ResponseText);
                                    itemIndex++;
                                });
                            }
                        }
                    }
                }
            }
        }

        private void PopulateAdditionalInfoResponse(int itemIndex, string responseText)
        {
            if (rptrAdditionalInfoQuestions.Items.Count > itemIndex)
            {
                RepeaterItem rptrItem = rptrAdditionalInfoQuestions.Items[itemIndex];
                if (rptrItem != null)
                {
                    TextBox txtResponse = (TextBox)rptrItem.FindControl("txtResponse");

                    if (txtResponse != null && !string.IsNullOrEmpty(responseText))
                    {
                        txtResponse.Text = responseText;
                    }
                }
            }
        }

        private void BuildAttributeList(RadComboBox cbo, string attributeLabel, string choiceList)
        {
            if (cbo != null)
            {
                cbo.Items.Clear();

                char[] delimiter = new char[] { ';' };
                foreach (string choice in choiceList.Split(delimiter, StringSplitOptions.RemoveEmptyEntries))
                {
                    cbo.Items.Add(new RadComboBoxItem(string.Format("{0}: {1}", attributeLabel, choice)));
                }
            }
        }

        protected void rptrAdditionalInfoQuestions_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Response addInfoResponse = (Response)e.Item.DataItem;

                TextBox txtQuestion = (TextBox)e.Item.FindControl("txtQuestion");
                TextBox txtResponse = (TextBox)e.Item.FindControl("txtResponse");

                txtQuestion.Text = addInfoResponse.Question;
                txtResponse.Text = addInfoResponse.ResponseText;
            }
        }

        private void LoadCategoryDropdown(int selectedCategoryId)
        {
            ddlCategoryId.Items.Clear();

            ProductController productCntl = new ProductController();

            ddlCategoryId.DataTextField = "CategoryName";
            ddlCategoryId.DataValueField = "CategoryId";
            ddlCategoryId.DataSource = productCntl.GetCategoryList(CurrentUser.CurrentShow.ShowId).Where(cat => cat.ActiveFlag == true).OrderBy(cat => cat.CategoryName).ToList();
            ddlCategoryId.DataBind();

            ddlCategoryId.Items.Insert(0, new ListItem("-- Select One --", string.Empty));

            if (selectedCategoryId > 0)
            {
                WebUtil.SelectListItemByValue(ddlCategoryId, selectedCategoryId);
            }
        }

        private void ClearAttributeSelections()
        {
            cboOrderItemRequiredAttribute1.Items.Clear();
            cboOrderItemRequiredAttribute2.Items.Clear();
            cboOrderItemRequiredAttribute3.Items.Clear();
            cboOrderItemRequiredAttribute4.Items.Clear();
        }

        private void LoadProductDropdown(int categoryId, int selectedProductId)
        {
            ddlProductId.Items.Clear();

            if (categoryId > 0)
            {
                ProductController productCntl = new ProductController();
                Category category = productCntl.GetCategory(categoryId);
                if (category != null)
                {
                    ddlProductId.DataTextField = "ProductName";
                    ddlProductId.DataValueField = "ProductId";
                    string productTypeItem = ProductTypeEnum.Item.ToString();
                    ddlProductId.DataSource = category.Products.Where(p => p.ActiveFlag == true && p.ProductTypeCd == productTypeItem).OrderBy(p => p.SortOrder);  //only show active products
                    ddlProductId.DataBind();
                }
            }
            
            ddlProductId.Items.Insert(0, new ListItem("-- Select One --", string.Empty));

            if (selectedProductId > 0)
            {
                WebUtil.SelectListItemByValue(ddlProductId, selectedProductId);
            }
        }

        protected void btnCancelOrderItemAdjust_Click(object sender, EventArgs e)
        {
            LoadOrderDetail(CurrentOrderId);
        }

        protected void btnSendInvoice_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "SendInvoiceInformation";

            int exhibitorId = CurrentExhibitorId;

            if (exhibitorId > 0)
            {
                ValidationResults errors = base.SendExhibitorInvoice(CurrentUser, exhibitorId);
                if (!errors.IsValid)
                {
                    PageErrors.AddErrorMessages(errors);
                }
            }
        }

        protected void btnSendOrderReceipt_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "SendInvoiceInformation";

            int exhibitorId = CurrentExhibitorId;

            if (exhibitorId > 0)
            {
                ValidationResults errors = base.SendExhibitorOrderReceipt(CurrentUser, exhibitorId, CurrentOrderId);
                if (!errors.IsValid)
                {
                    PageErrors.AddErrorMessages(errors);
                }
            }
        }


        protected void ddlCategoryId_Changed(object sender, EventArgs e)
        {
            if (sender is DropDownList)
            {
                int selectedCategoryId = Util.ConvertInt32(((DropDownList)sender).SelectedValue);

                LoadProductDropdown(selectedCategoryId, Util.ConvertInt32(ddlProductId.SelectedValue));
                ClearAttributeSelections();
            }
        }

        protected void ddlProductId_Changed(object sender, EventArgs e)
        {
            ClearOtherPricing();

            txtOrderItemName.ReadOnly = false;
            if (sender is DropDownList)
            {
                int selectedProductId = Util.ConvertInt32(((DropDownList)sender).SelectedValue);

                if (selectedProductId > 0)
                {
                    txtSalesTax.Text = "0";

                    ProductController prodCntrl = new ProductController();
                    Product productDetail = prodCntrl.GetProductById(selectedProductId);

                    if (productDetail != null)
                    {
                        txtProductSku.Text = productDetail.ProductSku;
                        txtOrderItemName.Text = productDetail.ProductName;
                        txtOrderItemName.ReadOnly = true;
                        DateTime dteNow = DateTime.Now;
                        txtOrderItemUnitPrice.Text = Util.FormatAmountDisplay(productDetail.DetermineCurrentUnitPrice(dteNow, true));
                        txtOrderItemUnitPriceDescription.Text = productDetail.DetermineCurrentUnitPriceDescription(dteNow, true);

                        PaintOtherPricing(productDetail);

                        if (!string.IsNullOrEmpty(productDetail.RequiredAttribute1Label))
                        {
                            cboOrderItemRequiredAttribute1.Text = productDetail.RequiredAttribute1Label + ":";
                        }

                        if (!string.IsNullOrEmpty(productDetail.RequiredAttribute2Label))
                        {
                            cboOrderItemRequiredAttribute2.Text = productDetail.RequiredAttribute2Label + ":";
                        }

                        if (!string.IsNullOrEmpty(productDetail.RequiredAttribute3Label))
                        {
                            cboOrderItemRequiredAttribute3.Text = productDetail.RequiredAttribute3Label + ":";
                        }

                        if (!string.IsNullOrEmpty(productDetail.RequiredAttribute4Label))
                        {
                            cboOrderItemRequiredAttribute4.Text = productDetail.RequiredAttribute4Label + ":";
                        }

                        LoadProductPricingDetails(productDetail, true);

                        txtOrderItemUnitDescriptor.Text = productDetail.UnitDescriptor;
                    }
                }
            }
        }

        private void ClearOtherPricing()
        {

            ltrAllPricingProductName.Text = string.Empty;
            ltrShowFloorPrice.Text = string.Empty;
            ltrAdvancedPrice.Text = string.Empty;
            ltrStandardPrice.Text = string.Empty;

        }
        private void PaintOtherPricing(Product productDetail)
        {
            if (productDetail != null)
            {
                ltrAllPricingProductName.Text = productDetail.ProductName;
                if (productDetail.UnitPrice.HasValue)
                {
                    ltrShowFloorPrice.Text = Util.FormatAmountDisplay(productDetail.UnitPrice.Value);
                }

                if (productDetail.EarlyBirdPrice.HasValue)
                {
                    ltrAdvancedPrice.Text = Util.FormatAmountDisplay(productDetail.EarlyBirdPrice.Value);
                }

                if (productDetail.DiscountUnitPrice.HasValue)
                {
                    ltrStandardPrice.Text = Util.FormatAmountDisplay(productDetail.DiscountUnitPrice.Value);
                }
            }
        }

        protected void btnAcceptSelectedOrders_Click(object sender, EventArgs e)
        {
            List<Order> orderList = new List<Order>();
            
            foreach (GridViewRow gr in grdvwExhibitorOrderList.Rows)
            {
                CheckBox chkAcceptOrder = (CheckBox)gr.FindControl("chkAcceptOrder");

                if (chkAcceptOrder != null && chkAcceptOrder.Checked)
                {
                    HiddenField hdnRowOrderId = (HiddenField)gr.FindControl("hdnRowOrderId");

                    Order ord = new Order();
                    ord.OrderId = Util.ConvertInt32(hdnRowOrderId.Value);
                    ord.OrderStatusCd = OrderStatusEnum.Accepted.GetCodeValue(); ;

                    orderList.Add(ord);
                }
            }

            if (orderList != null && orderList.Count > 0)
            {
                OrderAdminController cntrl = new OrderAdminController();

                ValidationResults errors = cntrl.UpdateOrderStatuses(orderList, OrderStatusEnum.Accepted);

                if (errors.IsValid)
                {
                    InitializeOrderSearch();
                    this.Master.DisplayFriendlyMessage("Orders Accepted!");

                    DisplayOrderList(false);
                }
                else
                {
                    this.PageErrors.AddErrorMessages(errors);
                }
            }
        }

        protected void lnkBtnExhibitorName_Click(object sender, EventArgs e)
        {
            if (sender is LinkButton)
            {
                LinkToExhibitorDetail(Util.ConvertInt32(((LinkButton)sender).CommandArgument));
            }
        }

        private void RecalculateAdditionalCharge()
        {
            decimal totalAdditionalCharge = 0;
            if (!string.IsNullOrEmpty(hdnAdditionalChargeType.Value) && !string.IsNullOrEmpty(hdnAdditionalChargeAmt.Value))
            {
                decimal qty = Util.ConvertDecimal(txtOrderItemQuantity.Text);
                AdditionalChargeTypeEnum additionalChargeType = Enum<AdditionalChargeTypeEnum>.Parse(hdnAdditionalChargeType.Value, true);
                decimal additionalChargeAmt = Util.ConvertDecimal(hdnAdditionalChargeAmt.Value);
                if (additionalChargeType == AdditionalChargeTypeEnum.Flat)
                {
                    totalAdditionalCharge = additionalChargeAmt;
                }
                else if (additionalChargeType == AdditionalChargeTypeEnum.PerUnit)
                {
                    totalAdditionalCharge = (additionalChargeAmt * qty);
                }
                else if (additionalChargeType == AdditionalChargeTypeEnum.PctTotal)
                {
                    decimal totalUnitCost = ( qty * Util.ConvertDecimal(txtOrderItemUnitPrice.Text));
                    totalAdditionalCharge = (totalUnitCost * additionalChargeAmt);
                }
                else
                {
                    totalAdditionalCharge = Util.ConvertDecimal(hdnAdditionalChargeAmt.Value);
                }

                totalAdditionalCharge = Util.RoundDecimal(totalAdditionalCharge);
            }
            
            txtOrderItemAdditionalChargeAmount.Text = Util.FormatAmount(totalAdditionalCharge);
        }

        private void RecalculateLateFee()
        {
            decimal totalLateFee = 0;

            decimal lateFeeAmt = Util.ConvertDecimal(hdnLateFeeAmt.Value);
            
            DateTime? lateFeeDeadline = null;
            if (!string.IsNullOrEmpty(hdnLateFeeDeadline.Value))
            {
                lateFeeDeadline = Util.ConvertNullDateTime(hdnLateFeeDeadline.Value);
            }
            if (Util.IsPassedDeadline(lateFeeDeadline, DateTime.Now) && lateFeeAmt > 0)
            {
                decimal qty = Util.ConvertDecimal(txtOrderItemQuantity.Text);
                LateFeeTypeEnum lateFeeType = Enum<LateFeeTypeEnum>.Parse(hdnLateFeeType.Value, true);
                if (lateFeeType == LateFeeTypeEnum.Flat)
                {
                    totalLateFee = lateFeeAmt;
                }
                else if (lateFeeType == LateFeeTypeEnum.PerUnit)
                {
                    totalLateFee = (lateFeeAmt * qty);
                }
                else if (lateFeeType == LateFeeTypeEnum.PctTotal)
                {
                    decimal totalUnitCost = (qty * Util.ConvertDecimal(txtOrderItemUnitPrice.Text));
                    totalLateFee = (totalUnitCost * lateFeeAmt);
                }
                else
                {
                    totalLateFee = lateFeeAmt;
                }
                totalLateFee = Util.RoundDecimal(totalLateFee);
            }

            txtOrderItemLateFeeAmount.Text = Util.FormatAmount(totalLateFee);
        }

        private void RecalculateSalesTax()
        {
            // _salesTaxCharges = ((decimal)this.Product.SalesTaxPercent.Value) * (_totalUnitPrice + _additionalCharges + _lateFees);
            decimal salesTaxPct = (Util.ConvertDecimal(txtSalesTax.Text) / 100);

            decimal unitCost = Util.ConvertDecimal(txtOrderItemUnitPrice.Text.Trim()) * Util.ConvertDecimal(txtOrderItemQuantity.Text.Trim());
            decimal addCharges = Util.ConvertDecimal(txtOrderItemAdditionalChargeAmount.Text.Trim());
            decimal lateFees = Util.ConvertDecimal(txtOrderItemLateFeeAmount.Text.Trim());

            decimal totalSalesTaxAmt = (salesTaxPct * (unitCost + addCharges + lateFees));
            txtOrderItemSalesTaxAmount.Text = Util.FormatAmount(totalSalesTaxAmt);
        }

        protected void lnkRecalculateLateFee_Click(object sender, EventArgs e)
        {
            RecalculateLateFee();
        }

        protected void lnkRecalculateAdditionalCharge_Click(object sender, EventArgs e)
        {
            RecalculateAdditionalCharge();
        }
        

        protected void lnkRecalculateSalesTax_Click(object sender, EventArgs e)
        {
            RecalculateSalesTax();
        }

        private void DisplayCreateOrder(int exhibitorIdToSelect)
        {
            txtCreateOrderDate.Text = DateTime.Now.ToString();

            lblCreateOrderError.Text = string.Empty;
            lblCreateOrderError.Visible = false;

            OwnerAdminController cntrl = new OwnerAdminController();
            ddlCreateOrderExhibitor.Items.Clear();
            ddlCreateOrderExhibitor.DataValueField = "ExhibitorId";
            ddlCreateOrderExhibitor.DataTextField = "ExhibitorCompanyName";
            ddlCreateOrderExhibitor.DataSource = cntrl.GetExhibitors(CurrentUser.CurrentShow.ShowId, true);
            ddlCreateOrderExhibitor.DataBind();

            ddlCreateOrderExhibitor.Items.Insert(0, new ListItem("-- Select One --", string.Empty));

            if (exhibitorIdToSelect > 0)
            {
                WebUtil.SelectListItemByValue(ddlCreateOrderExhibitor, exhibitorIdToSelect);
            }

            this.rbtnLstCreateOrderPaymentType.Items.Clear();
            this.rbtnLstCreateOrderPaymentType.Items.Add(new ListItem("Credit Card", PaymentTypeEnum.CreditCard.ToString()));
            this.rbtnLstCreateOrderPaymentType.Items.Add(new ListItem("Check", PaymentTypeEnum.Check.ToString()));
            this.rbtnLstCreateOrderPaymentType.Items.Add(new ListItem("Wire Transfer", PaymentTypeEnum.Wire.ToString()));
            this.rbtnLstCreateOrderPaymentType.Items.Add(new ListItem("Exhibitor Fund Transfer", PaymentTypeEnum.EEFT.ToString()));
            this.rbtnLstCreateOrderPaymentType.Items[0].Selected = true;

            this.MPE.Show();
        }

        protected void btnDisplayCreateOrder2_Click(object sender, EventArgs e)
        {
            DisplayCreateOrder(CurrentExhibitorId);
        }
        protected void btnDisplayCreateOrder_Click(object sender, EventArgs e)
        {
            DisplayCreateOrder(Util.ConvertInt32(cboSearchExhibitorId.SelectedValue));
        }

        protected void btnCreateOrder_Click(object sender, EventArgs e)
        {
            Order newOrder = new Order();
            newOrder.OrderGuid = Guid.NewGuid();
            newOrder.ActiveFlag = true;
            newOrder.ExhibitorId = Util.ConvertInt32(ddlCreateOrderExhibitor.SelectedValue);
            newOrder.OrderDate = Util.ConvertDateTime(txtCreateOrderDate.Text);
            
            newOrder.OrderStatusCd = OrderStatusEnum.Accepted.GetCodeValue();
            newOrder.OrderTypeCd = OrderTypeEnum.BoothOrder.GetCodeValue();
            newOrder.PaymentTypeCd = Enum<PaymentTypeEnum>.Parse(rbtnLstCreateOrderPaymentType.SelectedValue).ToString();
            newOrder.OrderTotal = 0;

            ValidationResults errors = Cntrl.CreateNewOrder(CurrentUser, newOrder);
            if (errors.IsValid)
            {
                //SearchCriteria search = new SearchCriteria();
                //search.OrderId = newOrder.OrderId;
                //this.LoadOrderList(search);
                this.LoadOrderDetail(newOrder.OrderId);
                this.MPE.Hide();
            }
            else
            {
                lblCreateOrderError.Text = Util.AllValidationErrors(errors);
                lblCreateOrderError.Visible = true;
                this.MPE.Show();
            }
        }

        protected void cboJumpToOrderId_Changed(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            int jumpToOrderId = Util.ConvertInt32(cboJumpToOrderId.SelectedValue);
            if (jumpToOrderId > 0)
            {
                LoadOrderDetail(jumpToOrderId);
            }
        }

        protected void ddlClassification_Changed(object sender, EventArgs e)
        {
            OrderAdminController orderCntrl = new OrderAdminController();
            orderCntrl.SaveClassification(CurrentUser, CurrentExhibitorId, ddlClassification.SelectedValue);
            this.Master.DisplayFriendlyMessage("Classification Saved.");
        }

        protected void btnCancelCreateOrder_Click(object sender, EventArgs e)
        {
            this.MPE.Hide();
        }

        protected void lbtnViewCallLogs_Click(object sender, EventArgs e)
        {
            LaunchCallLogViewer(null, CurrentExhibitorId);
        }

        private int CurrentExhibitorId
        {
            get { return Util.ConvertInt32(hdnExhibitorId.Value); }
            set { hdnExhibitorId.Value = value.ToString(); }
        }

        private int CurrentOrderId
        {
            get { return Util.ConvertInt32(hdnOrderId.Value); }
            set { hdnOrderId.Value = value.ToString(); }
        }

        #endregion
        

    }
}