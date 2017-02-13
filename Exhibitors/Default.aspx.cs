using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Entities;
using ExpoOrders.Common;

namespace ExpoOrders.Web.Exhibitors
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string defaultPageName = "Show.aspx";

            if (CurrentUser == null)
            {
                throw new Exception("currentUser is null");
            }

            if (CurrentUser.CurrentShow == null)
            {
                throw new Exception("currentShow is null");
            }

            TabLink firstTab = CurrentUser.CurrentShow.TabLinks.OrderBy(t => t.TabNumber).FirstOrDefault<TabLink>(t => t.Visible == true);

            if (firstTab != null)
            {
                defaultPageName = firstTab.Page;
            }
            Server.Transfer(string.Concat("~/Exhibitors/", defaultPageName), false);
        }

    }
}