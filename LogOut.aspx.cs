using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Security;
using ExpoOrders.Controllers;


namespace ExpoOrders.Web
{
    public partial class LogOut : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int showId = 0;
                string loginUrl = string.Empty;
                bool isExhibitor = false;
                if (CurrentUser.IsExhibitor && CurrentUser.CurrentShow != null)
                {
                    isExhibitor = true;
                    showId = CurrentUser.CurrentShow.ShowId;
                }


                LogUserOut();

                if (isExhibitor && showId > 0)
                {
                    //Must retain the ?showid={0}
                    Response.Redirect(string.Format("~/Login.aspx?showid={0}", showId), true);
                }
                else
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
                
            }
            
        }

        private void LogUserOut()
        {
            LoginController cntrl = new LoginController();
            cntrl.LogUserSignOut(Session.SessionID, CurrentUser.UserId);

            CurrentUser.LogOff();

            FormsAuthentication.SignOut();

        }

        protected override void OnInit(EventArgs e)
        {
            base.RequiresSsl = false;
            base.OnInit(e);
        }
    }
}