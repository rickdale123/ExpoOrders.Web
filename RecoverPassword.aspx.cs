using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Entities;
using ExpoOrders.Common;
using ExpoOrders.Controllers;

namespace ExpoOrders.Web
{
    public partial class RecoverPassword : BasePage
    {
        #region Manager Objects
        private LoginController _pageMgr = null;
        public LoginController PageMgr
        {
            get
            {
                if (_pageMgr == null)
                {
                    _pageMgr = new LoginController(CurrentUser);
                }

                return _pageMgr;
            }
        }
        #endregion
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            this.Master.LoadHost();
        }

        
    }
}