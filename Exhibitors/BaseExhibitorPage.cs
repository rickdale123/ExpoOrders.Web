using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ExpoOrders.Entities;
using ExpoOrders.Common;
using ExpoOrders.Controllers;

namespace ExpoOrders.Web.Exhibitors
{
    //just testing.
    public class BaseExhibitorPage : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Context != null && Context.Session != null)
            {
                if (!CurrentUser.IsExhibitor)
                {
                    RedirectToPage(PageRedirect.UserLogin);
                }
            }
            
        }

        protected TabLink FindCurrentTab(ExhibitorPageEnum currentPage)
        {
            return CurrentUser.CurrentShow.TabLinks.Where<TabLink>(
                    link => link.Page == currentPage.GetCodeValue()
                    ).SingleOrDefault<TabLink>();
        }

        private HtmlContentController _htmlCtrl;
        public HtmlContentController HtmlController
        {
            get
            {
                if (_htmlCtrl == null)
                {
                    _htmlCtrl = new HtmlContentController();
                }
                return _htmlCtrl;
            }
        }

        private FormController _formMgr = null;
        public FormController FormMgr
        {
            get
            {
                if (_formMgr == null)
                {
                    return new FormController();
                }
                else
                {
                    return _formMgr;
                }
            }
        }

        public OrderConfirmation ConfirmationOrder
        {
            get
            {
                OrderConfirmation confirm = null;
                if (HttpContext.Current.Items["OrderConfirmation"] != null)
                {
                    confirm = HttpContext.Current.Items["OrderConfirmation"] as OrderConfirmation;
                }

                return confirm;
            }

            set
            {
                if (value != null)
                {
                    HttpContext.Current.Items.Add("OrderConfirmation", value);
                }
                else
                {
                    if (HttpContext.Current.Items["OrderConfirmation"] != null)
                    {
                        HttpContext.Current.Items.Remove("OrderConfirmation");
                    }
                }
            }
        }

        public void RedirectBoothCategory(int navLinkId, int targetId, string categoryDisplay)
        {
            TempNavTabRedirect = new NavTabRedirect(navLinkId, targetId, categoryDisplay);
            Server.Transfer("Booth.aspx", false);
        }

        public void RedirectOutboundShippingLabel(int navLinkId, int targetId)
        {
            TempNavTabRedirect = new NavTabRedirect(navLinkId, targetId, string.Empty);
            Server.Transfer("Shipping.aspx", false);
        }

        public void ResetNavTabRedirect()
        {
            TempNavTabRedirect = null;
        }
        public NavTabRedirect TempNavTabRedirect
        {
            get
            {
                NavTabRedirect _tempCategory = null;
                if (HttpContext.Current.Items["TempNavTabRedirect"] != null)
                {
                    _tempCategory = HttpContext.Current.Items["TempNavTabRedirect"] as NavTabRedirect;
                }

                return _tempCategory;
            }

            set
            {
                if (value != null)
                {
                    HttpContext.Current.Items.Add("TempNavTabRedirect", value);
                }
                else
                {
                    if (HttpContext.Current.Items["TempNavTabRedirect"] != null)
                    {
                        HttpContext.Current.Items.Remove("TempNavTabRedirect");
                    }
                }
            }
        }
    }

    public class NavTabRedirect
    {
        public int? NavLinkId;
        public int? TargetId;
        public string DisplayHeading;

        public NavTabRedirect(int navLinkId, int targetId, string displayHeading)
        {
            NavLinkId = navLinkId;
            TargetId = targetId;
            DisplayHeading = displayHeading;
        }
    }

}