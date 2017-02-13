using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Common;
using ExpoOrders.Entities;
using ExpoOrders.Controllers;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.Web.Security;

namespace ExpoOrders.Web.Owners
{
    public partial class Users : BaseOwnerPage
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
            this.Master.LoadMasterPage(this.CurrentUser, OwnerPage.Users);
            this.Master.LoadSubNavigation("Users", OwnerAdminMgr.GetUserSubNavigation());

            Master.SelectNavigationItem(1);
            DisplayUserList();

        }

        private void DisplayUserList()
        {
            plcUserDetail.Visible = false;
            plcUserList.Visible = true;

            grdvwOwnerUserList.DataSource = OwnerAdminMgr.GetOwnerUsers(CurrentUser);
            grdvwOwnerUserList.DataBind();
        }

        private void EditUser(string userId)
        {
            plcUserList.Visible = false;
            plcUserDetail.Visible = true;

            LoadAvailableUserRoles();

            hdnUserId.Value = string.Empty;
            txtAllowableShowIdList.Text = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                hdnUserId.Value = userId;

                UserContainer userInfo = OwnerAdminMgr.GetMemberShipUser(userId, true);
                ucUserDetail.Populate(null, userInfo, true, true);

                PopulateExistingUserRoles(userInfo);

                txtAllowableShowIdList.Text = userInfo.AllowableShowIdList;
            }
            else
            {
                this.ucUserDetail.Populate(null, null, true, false);
            }
        }

        private void PopulateExistingUserRoles(UserContainer userInfo)
        {
            foreach (string role in Roles.GetRolesForUser(userInfo.ActualUserName))
            {
                ListItem li = chkLstUserRoles.Items.FindByValue(role);
                if (li != null)
                {
                    li.Selected = true;
                }
            }
        }

        private void LoadAvailableUserRoles()
        {
            chkLstUserRoles.Items.Clear();
            //chkLstUserRoles.Items.Add(UserRoleEnum.Owner.GetCodeValue());
            chkLstUserRoles.Items.Add(UserRoleEnum.UserManagement.GetCodeValue());
            chkLstUserRoles.Items.Add(UserRoleEnum.ShowManagement.GetCodeValue());
            chkLstUserRoles.Items.Add(UserRoleEnum.ExhibitorManagement.GetCodeValue());
            chkLstUserRoles.Items.Add(UserRoleEnum.OrderProcessing.GetCodeValue());
            chkLstUserRoles.Items.Add(UserRoleEnum.ExhibitorReports.GetCodeValue());
            chkLstUserRoles.Items.Add(UserRoleEnum.OperationsReports.GetCodeValue());
            chkLstUserRoles.Items.Add(UserRoleEnum.FinancialReports.GetCodeValue());
            
            
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            //this.LoadPageMode(navLinkId, targetId);

            if (targetId == 2)
            {
                EditUser(null);
            }
            else
            {
                DisplayUserList();
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            EditUser(null);
        }

        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            string userId = hdnUserId.Value.Trim();

            UserContainer userDetail = ucUserDetail.BuildUserContainer();
            userDetail.ActualUserName = userDetail.PreferredUserName;

            userDetail.IsOwner = true;
            userDetail.OwnerId = CurrentUser.CurrentOwner.OwnerId;
            userDetail.AllowableShowIdList = txtAllowableShowIdList.Text.Trim().Replace(",", ";").Replace(" ", string.Empty);
            List<string> userRoles = new List<string>();

            userRoles.Add(UserRoleEnum.Owner.GetCodeValue());

            foreach (ListItem li in chkLstUserRoles.Items)
            {
                if (li.Selected)
                {
                    userRoles.Add(li.Value);
                }
            }

            ValidationResults errors = this.OwnerAdminMgr.SaveUser(CurrentUser, userDetail, userRoles);

            if (errors.IsValid)
            {
                DisplayUserList();
                Master.DisplayFriendlyMessage("User Saved.");
            }
            else
            {
                this.PageErrors.AddErrorMessages(errors);
            }
        }

        protected void btnCancelSaveUser_Click(object sender, EventArgs e)
        {
            DisplayUserList();
        }

        protected void grdvwOwnerUserList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UserContainer user = (UserContainer) e.Row.DataItem;

                LinkButton lbtnDeleteUser = (LinkButton)e.Row.FindControl("lbtnDeleteUser");

                lbtnDeleteUser.Attributes.Add("onClick", "Javascript: return confirm('Are you sure you want to delete this user?');");
            }
        }

        protected void grdvwOwnerUserList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string userId = e.CommandArgument.ToString();
            switch (e.CommandName)
            {
                case "DeleteUser":
                    DeleteUser(userId);
                    break;
                case "EditUser":
                    EditUser(userId);
                    break;
            }
        }

        private void DeleteUser(string userId)
        {
            ValidationResults errors = OwnerAdminMgr.DeactivateUser(CurrentUser, userId);

            if (errors.IsValid)
            {
                DisplayUserList();
                Master.DisplayFriendlyMessage("User Removed Succcessfully.");
            }
            else
            {
                PageErrors.AddErrorMessages(errors);
            }
        }

    }
}