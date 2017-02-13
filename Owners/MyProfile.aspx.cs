using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace ExpoOrders.Web.Owners
{
    public enum MyProfilePageMode { ChangePassword = 1, OwnerInfo = 2 }
    public partial class MyProfile : BaseOwnerPage
    {
        #region Public Members
        private OwnerAdminController _mgr = null;
        public OwnerAdminController OwnerAdminMgr
        {
            get
            {
                if (_mgr == null)
                {
                    _mgr = new OwnerAdminController();
                }
                return _mgr;
            }
        }
        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.NavigationItemCallBack = this.HandleNavigationItemClicked;
            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            //this.Master.NoSubNavigation = true;
            this.Master.LoadMasterPage(this.CurrentUser, OwnerPage.MyProfile);
            this.Master.LoadSubNavigation("My Profile", OwnerAdminMgr.GetMyProfileSubNavigation());

            LoadPageMode(MyProfilePageMode.ChangePassword);

        }

        private void LoadPageMode(MyProfilePageMode mode)
        {
            if (this.Master.CurrentSubNavigationLinks != null && this.Master.CurrentSubNavigationLinks.Count > 0)
            {
                NavigationLink linkToSelect = this.Master.CurrentSubNavigationLinks.SingleOrDefault<NavigationLink>(l => l.TargetId == (int)mode);
                this.Master.SelectNavigationItem(linkToSelect.NavigationLinkId);
                LoadPageMode(linkToSelect.NavigationLinkId, (int)linkToSelect.TargetId);
            }

        }

        private void LoadPageMode(int navLinkId, int targetId)
        {
            MyProfilePageMode currentPageMode = (MyProfilePageMode)Enum.Parse(typeof(MyProfilePageMode), targetId.ToString(), true);

            plcChangePassword.Visible = false;

            switch (currentPageMode)
            {
                case MyProfilePageMode.ChangePassword:
                    plcChangePassword.Visible = true;
                    break;
                case MyProfilePageMode.OwnerInfo:
                    break;
            }

        }
        #endregion

        #region Change Password

        protected void btnSavePassword_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "PasswordChange";

            if ((string.IsNullOrEmpty(txtPassword1.Text.Trim()) == true) ||
                (string.IsNullOrEmpty(txtPassword2.Text.Trim()) == true) ||
                (txtPassword1.Text.Trim() != txtPassword2.Text.Trim()))
            {
                ValidationResult error = new ValidationResult("Passwords must match", null, null, null, null);
                PageErrors.AddErrorMessage(error, PageErrors.ValidationGroup);
            }

            if (Page.IsValid)
            {
                SavePassword();
                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Password saved.");
                }
            }
        }

        private void SavePassword()
        {
            ValidationResults errors = OwnerAdminMgr.SavePassword(CurrentUser, txtPassword1.Text.Trim());
            if (!errors.IsValid)
            {
                this.PageErrors.AddErrorMessages(errors, this.PageErrors.ValidationGroup);
            }
        }
        #endregion

        #region Control Events
        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            this.LoadPageMode(navLinkId, targetId);
        }
        #endregion

    }
}