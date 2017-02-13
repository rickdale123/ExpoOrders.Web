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


using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.Web.UI.HtmlControls;

namespace ExpoOrders.Web
{
    public partial class Login : BasePage
    {
        protected string CurrentOwnerShowListingText = string.Empty;
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

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Disable that pesky "ReturnUrl" internal behavior of the LoginControl
                if (Request["ReturnUrl"] != null)
                {
                    Response.Redirect("login.aspx");
                }

                LoadPage();
            }
        }

        private void LoadPage()
        {
            plcOwnerShowList.Visible = false;

            plcLoginArea.Visible = true;
            plcPasswordReminder.Visible = false;

            Master.AppendOnLoad("focusLogin();");
            int ownerId = Master.LoadHost();

            int showId = 0;
            bool displayMgrLogin = false;
            if (Request["mgr"] != null)
            {
                PaintManagerLogin();
                displayMgrLogin = true;
            }
            else
            {
               showId = DesiredQueryStringShowId();
            }

            
            if (showId > 0)
            {
                ExpoOrdersLogin.TitleText = "User Login";
                Show currentShow = RetrieveShowInfo(showId);
                
                Master.LoadShowInfo(currentShow);

                Owner owner = Controller.GetOwnerById(ownerId);

                if (currentShow != null)
                {
                    if (!string.IsNullOrEmpty(currentShow.LoginInfoText))
                    {
                        ExpoOrdersLogin.InstructionText = currentShow.LoginInfoText;
                    }
                    
                    if (!string.IsNullOrEmpty(currentShow.LoginContactInfo))
                    {
                        Master.DisplayContactInfo(currentShow.LoginContactInfo);
                    }
                    else
                    {
                        if (owner != null)
                        {
                            Master.LoadOwnerContactInfo(owner);
                        }
                    }
                }
            }
            else
            {
                Master.LoadShowInfo(null);
                if (ownerId > 0)
                {
                    DisplayOwnerShowList(ownerId, displayMgrLogin);
                }
            }
        }

        private void PaintManagerLogin()
        {
            Master.ManagerLoginEnabled = false;
            plcOwnerShowList.Visible = false;

            plcLoginArea.Visible = true;
            plcPasswordReminder.Visible = false;
            ExpoOrdersLogin.TitleText = "Manager Login";
        }

        private void DisplayOwnerShowList(int ownerId, bool displayMgrLogin)
        {
            if (ownerId > 0)
            {
                Owner owner = Controller.GetOwnerById(ownerId);

                if (owner != null)
                {
                    CurrentOwnerShowListingText = owner.ShowListingInfoText;
                    Master.LoadOwnerContent(owner);
                }

                if (displayMgrLogin)
                {
                    PaintManagerLogin();
                }
                else
                {
                    Master.ManagerLoginEnabled = true;
                    List<Show> ownerShows = Controller.FindOwnerShows(ownerId);
                    if (ownerShows != null && ownerShows.Count > 0)
                    {
                        if (ownerShows.Count == 1 && !Util.ConvertBool(Request.QueryString["admin"]))
                        {
                            Response.Redirect(string.Format("Login.aspx?showid={0}", ownerShows[0].ShowId));
                        }
                        else
                        {
                            plcLogin.Visible = false;
                            plcOwnerShowList.Visible = true;
                            rptrOwnerShowList.DataSource = ownerShows;
                            rptrOwnerShowList.DataBind();

                            //ltrExhibitorLoginNote.Text = GetHelpText("ExhibitorLoginNote");
                        }
                    }
                }
            }
        }

        private Show RetrieveShowInfo(int showId)
        {
            ShowController cntrl = new ShowController();
            return cntrl.FindShowForLogin(showId, WebUtil.CurrentHost());
        }

        protected void lnkBtnForgotPassword_Click(object sender, EventArgs e)
        {
            lblPasswordError.Text = string.Empty;
            lblPasswordError.Visible = false;
            txtEmailAddress.Text = string.Empty;
            plcPasswordReminder.Visible = true;
            plcLoginArea.Visible = false;
        }

        protected void lnkBtnLogin_Click(object sender, EventArgs e)
        {
            plcPasswordReminder.Visible = false;
            plcLoginArea.Visible = true;
        }

        protected void btnSendPassword_Click(object sender, EventArgs e)
        {
            LoginController logCntrl = new LoginController();

            lblPasswordError.Text = string.Empty;
            lblPasswordError.Visible = false;

            ValidationResults errors = logCntrl.EmailPasswordReminder(Request.Url.Host, DesiredQueryStringShowId(), txtEmailAddress.Text.Trim());

            if (errors.IsValid)
            {
                lblPasswordError.Text = string.Format("An Email has been sent to {0}", txtEmailAddress.Text.Trim());
                lblPasswordError.Visible = true;
            }
            else
            {
                foreach (ValidationResult error in errors)
                {
                    lblPasswordError.Text += error.Message + "<br/>";
                    lblPasswordError.Visible = true;
                }
            }
        }

        protected void rptrOwnerShowList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Show s = e.Item.DataItem as Show;

                if (s != null)
                {
                    HtmlAnchor lnkShowLogin = (HtmlAnchor)e.Item.FindControl("lnkShowLogin");
                    HtmlAnchor lnkQuickFactsFile = (HtmlAnchor)e.Item.FindControl("lnkQuickFactsFile");
                    if (lnkShowLogin != null)
                    {
                        lnkShowLogin.HRef = string.Format("Login.aspx?showid={0}", s.ShowId);
                    }

                    if (lnkQuickFactsFile != null)
                    {
                        lnkQuickFactsFile.Visible = false;
                        if (!string.IsNullOrEmpty(s.QuickFactsFileName))
                        {
                            lnkQuickFactsFile.HRef = string.Format("~/Assets/Shows/{0}/{1}", s.ShowGuid, s.QuickFactsFileName);
                            lnkQuickFactsFile.Visible = true;
                        }
                    }

                }
            }
        }

        #endregion

        #region Page Init

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        #endregion

        #region Membership Authentication

        protected void ExpoOrdersLogin_Authenticate(object sender, AuthenticateEventArgs e)
        {
            LoginResults loginResults = Controller.AuthenticateUser(Session.SessionID, Request.Url.Host, ExpoOrdersLogin.UserName.Trim(), ExpoOrdersLogin.Password.Trim(), DesiredQueryStringShowId());

            e.Authenticated = loginResults.Errors.IsValid;

            if (loginResults.Errors.IsValid)
            {
                ExpoOrdersLogin.DestinationPageUrl = loginResults.DestinationUrl;
            }
            else if (!loginResults.Errors.IsValid)
            {
                ExpoOrdersLogin.FailureText = Util.AllValidationErrors(loginResults.Errors);
            }
        }
        #endregion

    }
}