using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExpoOrders.Controllers;

namespace ExpoOrders.Web.SuperAdmin
{
    public class BaseSuperAdminPage : BasePage
    {
        private SuperAdminController _cntrl;
        public SuperAdminController Cntrl
        {
            get
            {
                if (_cntrl == null)
                {
                    _cntrl = new SuperAdminController();
                }
                return _cntrl;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Context != null && Context.Session != null)
            {
                if (!CurrentUser.IsSuperAdmin)
                {
                    RedirectToPage(PageRedirect.UserLogin);
                }
            }
            
        }

        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
        }


    }
}