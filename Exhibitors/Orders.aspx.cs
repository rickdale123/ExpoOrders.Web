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

namespace ExpoOrders.Web.Exhibitors
{
    public partial class Orders : BaseExhibitorPage
    {
        AccountController _ctrl;
        AccountController Cntrl
        {
            get
            {
                if (_ctrl == null)
                {
                    _ctrl = new AccountController();
                }
                return _ctrl;
            }
        }

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
            ltrPageHeading.Text = "Order History";

            DisplayInvoiceButton();

            
            plcOrderConfirmationMessage.Visible = lblOrderConfirmationMessage.Visible = false;
            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, ExhibitorPageEnum.MyOrders);
            Master.NoSubNavigation = true;

            LoadOrders();
        }

        private void DisplayInvoiceButton()
        {
            plcExhibitorButtons.Visible = true;
            btnViewExhibitorInvoice.Visible = true;

            btnViewExhibitorInvoice.Attributes.Add("onclick", string.Format("launchExhibitorInvoice({0}, {1}, {2}); return false;", (int)ReportEnum.ExhibitorInvoice, CurrentUser.CurrentShow.ShowId, CurrentUser.CurrentExhibitor.ExhibitorId));

        }

        private void LoadOrders()
        {
            plcOrderList.Visible = false;
            plcOrderDetail.Visible = false;
            plcNoOrders.Visible = false;

            int confirmationOrderId = 0;
            if (ConfirmationOrder != null && ConfirmationOrder.OrderId > 0)
            {
                ltrPageHeading.Text = "Order Confirmation";
                plcOrderConfirmationMessage.Visible = lblOrderConfirmationMessage.Visible = true;
                lblOrderConfirmationMessage.Text = CurrentUser.CurrentShow.OrderConfirmationMessage;

                confirmationOrderId = ConfirmationOrder.OrderId;
                ConfirmationOrder = null;
            }

            List<Order> exhibitorOrders = Cntrl.RetrieveExhibitorOrders(CurrentUser, confirmationOrderId).Where(o => o.ActiveFlag == true && o.OrderStatusCd != OrderStatusEnum.Deleted.ToString()).ToList();
            if (exhibitorOrders != null && exhibitorOrders.Count > 0)
            {
                plcOrderList.Visible = true;

                rptrExhibitorOrders.DataSource = exhibitorOrders;
                rptrExhibitorOrders.DataBind();
            }
            else
            {
                plcNoOrders.Visible = true;
            }
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            this.Master.DisplayFriendlyMessage("Unhandled NavLinkId:" + navLinkId.ToString() + " Action: " + action.ToString() + " TargetId:" + targetId.ToString());
        }

        protected void rptrExhibitorOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Order order = (Order)e.Item.DataItem;

                HtmlTable tblOrder = (HtmlTable)e.Item.FindControl("tblOrder");
                LinkButton lnkBtnOrderDetail = (LinkButton)e.Item.FindControl("lnkBtnOrderDetail");
                LinkButton lnkBtnOrderDetail2 = (LinkButton)e.Item.FindControl("lnkBtnOrderDetail2");
                
                Label lblOrderDateValue = (Label)e.Item.FindControl("lblOrderDateValue");
                Label lblUserNameValue = (Label)e.Item.FindControl("lblUserNameValue");
                Label lblEmailConfirmationValue = (Label)e.Item.FindControl("lblEmailConfirmationValue");
                

                Label lblOrderTotal = (Label)e.Item.FindControl("lblOrderTotal");
                Label lblOrderTotalValue = (Label)e.Item.FindControl("lblOrderTotalValue");
                Label lblOrderStatusValue = (Label)e.Item.FindControl("lblOrderStatusValue");
                PlaceHolder plcViewReceipt = (PlaceHolder)e.Item.FindControl("plcViewReceipt");
                HtmlAnchor lnkPrintReceipt = (HtmlAnchor)e.Item.FindControl("lnkPrintReceipt");

                Repeater rptrProductOrderItems = (Repeater)e.Item.FindControl("rptrProductOrderItems");
                Repeater rptrFormOrderItems = (Repeater)e.Item.FindControl("rptrFormOrderItems");

                tblOrder.Attributes["class"] = (e.Item.ItemType == ListItemType.AlternatingItem) ? "altItem" : "item";

                lblOrderDateValue.Text = order.OrderDateDisplay;
                lblUserNameValue.Text = order.OrderedBy;
                lblEmailConfirmationValue.Text = order.OrderEmailAddress;
                
                lblOrderStatusValue.Text = order.OrderStatusDisplay;

                if (order.OrderType == OrderTypeEnum.BoothOrder)
                {
                    
                    rptrFormOrderItems.Visible = false;
                    lblOrderTotal.Visible = lblOrderTotalValue.Visible = true;
                    lblOrderTotalValue.Text = Server.HtmlEncode(Util.FormatCurrency(order.OrderTotal.Value, CurrentUser.CurrentShow.CurrencySymbol));

                    plcViewReceipt.Visible = true;
                    lnkPrintReceipt.Visible = true;
                    lnkPrintReceipt.Attributes.Add("onclick", string.Format("launchOrderReceipt({0}, {1}); return false;", (int)ReportEnum.OrderReceipt, order.OrderId));

                    if (order.OrderItems.Count > 0)
                    {
                        rptrProductOrderItems.DataSource = order.OrderItems;
                        rptrProductOrderItems.DataBind();
                        rptrProductOrderItems.Visible = true;
                    }
                    else
                    {
                        rptrProductOrderItems.Visible = false;
                    }
                }
                else
                {
                    rptrProductOrderItems.Visible = false;
                    
                    lblOrderTotal.Visible = lblOrderTotalValue.Visible = false;

                    plcViewReceipt.Visible = true;
                    lnkPrintReceipt.Visible = true;
                    lnkPrintReceipt.Attributes.Add("onclick", string.Format("launchOrderReceipt({0}, {1}); return false;", (int)ReportEnum.FormSubmission, order.OrderId));


                    if (order.OrderItems.Count > 0)
                    {
                        rptrFormOrderItems.DataSource = order.OrderItems;
                        rptrFormOrderItems.DataBind();
                        rptrFormOrderItems.Visible = true;
                    }
                    else
                    {
                        rptrFormOrderItems.Visible = false;
                    }
                }
            }
        }


        protected void rptrProductOrderItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                OrderItem orderItem = (OrderItem)e.Item.DataItem;

                Label lblOrderItemQuantity = (Label)e.Item.FindControl("lblOrderItemQuantity");
                Label lblOrderItemDescriptionValue = (Label)e.Item.FindControl("lblOrderItemDescriptionValue");

                if (orderItem.Quantity.HasValue && orderItem.Quantity.Value > 0)
                {
                    lblOrderItemQuantity.Text = orderItem.Quantity.Value.ToString();
                }

                lblOrderItemDescriptionValue.Text = orderItem.ItemDescription;

            }
        }
        

        protected void lnkBtnOrderDetail_Click(object sender, EventArgs e)
        {
            plcOrderList.Visible = false;
            plcOrderDetail.Visible = false;

            if (sender is LinkButton)
            {
                LinkButton lnkBtn = (LinkButton)sender;
                int orderId = Util.ConvertInt32(lnkBtn.CommandArgument);

                Order orderDetail = Cntrl.RetrieveExhibitorOrderDetail(CurrentUser, orderId);

                if (orderDetail != null)
                {
                    plcOrderDetail.Visible = true;
                    ucOrderDetails.Visible = true;
                    ucOrderDetails.Populate(Cntrl, orderDetail, true, CurrentUser.CurrentShow.CurrencySymbol, CurrentUser.CurrentShow);
                }
            }
        }

        protected void btnBackToList_Click(object sender, EventArgs e)
        {
            this.LoadPage();
        }

    }
}