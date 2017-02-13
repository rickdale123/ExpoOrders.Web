using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.Profile;
using System.Web.ApplicationServices;

using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using ExpoOrders.Common;


namespace ExpoOrders.Web
{
    public partial class Selector : BasePage
    {
        #region Manager objects

        LoginController _ctrl;
        LoginController Controller
        {
            get
            {
                if (_ctrl == null)
                {
                    _ctrl = new LoginController(base.CurrentUser);
                }
                return _ctrl;
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


            List<Show> availableShows = Controller.GetSubscribedShows(CurrentUser, Request.Url.Host, 0);

            if (availableShows != null && availableShows.Count > 0)
            {
                plcShowSelector.Visible = true;
                rptrAvailableShows.Visible = true;
                rptrAvailableShows.DataSource = availableShows;
                rptrAvailableShows.DataBind();
            }
            else
            {
                throw new Exception("No shows to display");
            }
        }

        protected void lnkShowSelector_Click(object sender, EventArgs e)
        {
            if (sender is LinkButton)
            {
                LinkButton lnkBtn = (LinkButton)sender;

                Controller.SetExhibitorCurrentShow(Controller.GetSubscribedShows(CurrentUser, Request.Url.Host, Util.ConvertInt32(lnkBtn.CommandArgument)).FirstOrDefault());

                RedirectToPage(PageRedirect.ExhibitorHome);

            }

        }
    }
}