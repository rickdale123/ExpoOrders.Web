using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;

namespace ExpoOrders.Web.Owners
{
    public partial class OrderHistory : BaseOwnerPage
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
            if (Request["orderid"] != null)
            {
                int orderId = Util.ConvertInt32(Request["orderid"]);

                OrderAdminController orderCntrl = new OrderAdminController();
                Order orderDetail = orderCntrl.GetOrderHistory(orderId);

                if (orderDetail != null)
                {
                    lblOrderNotes.Text = orderDetail.Notes;

                    grdvwOrderHistory.DataSource = orderDetail.OrderItems.OrderBy(oi => oi.OrderItemId).ToList();
                    grdvwOrderHistory.DataBind();

                }
            }

            LoadStyleSheet(CurrentUser.CurrentShow);
        }

        public void LoadStyleSheet(Entities.Show currentShow)
        {
            this.OwnerStyleSheet.Href = string.Format("~/Assets/Shows/{0}/style.css", currentShow.ShowGuid);
        }


    }
}