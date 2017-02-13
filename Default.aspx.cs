using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using System.Text.RegularExpressions;
using System.Web.Security;

using System.Configuration;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace ExpoOrders.Web
{
    public partial class Default : BasePage
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
            
            string loginPage = base.BuildSslUrl(Request.Url.Host, Page.ResolveUrl("~/Login.aspx"));

            int showId = DesiredQueryStringShowId();

            bool redirectToLogin = false;
            if (showId > 0)
            {
                redirectToLogin = true;
            }
            else if (Request.Url.Host.ToLower() == ConfigurationManager.AppSettings["Host.ExpoOrders"])
            {
                lnkExhibitorLogin.HRef = lnkContractorLogin.HRef = loginPage;
                this.Master.LoadHost();
            }
            else
            {
                redirectToLogin = true;
            }

            if (redirectToLogin)
            {
                Response.Redirect(string.Format("{0}?{1}", loginPage, Request.QueryString), true);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.RequiresSsl = false;
            base.OnInit(e);
        }

    }
}