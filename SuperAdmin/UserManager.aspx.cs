using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

using ExpoOrders.Controllers;
using ExpoOrders.Common;
using ExpoOrders.Entities;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class UserManager : BaseSuperAdminPage
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
            DisplayUsers();
        }

        private void DisplayUsers()
        {
            plcSelectUser.Visible = true;
            plcEditUser.Visible = false;
            LoadUsers();
        }

        private void LoadUsers()
        {
            grdvUsers.Visible = true;

            lblUserRowCount.Text = "0 rows to display.<br/>";

            DataTable dtUsers = Cntrl.FindAllUsers();

            if (dtUsers != null && dtUsers.Rows.Count > 0)
            {
                lblUserRowCount.Text = string.Format("({0} users displayed.)<br/>", dtUsers.Rows.Count);
                grdvUsers.DataSource = dtUsers;
                grdvUsers.DataBind();
            }
        }

        protected void grdvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string userId = e.CommandArgument.ToString();
            switch (e.CommandName)
            {
                case "EditUser":
                    PickAUser(userId);
                    break;
            }
        }

        protected void grdvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }
        

        private void LoadUserDetail(string userId)
        {
            plcEditUser.Visible = true;
            plcSelectUser.Visible = false;

            txtAllowableShowIdList.Text = string.Empty;

            LoadUserTypes();

            btnDeleteUser.Visible = false;

            LoadAvailableUserRoles();

            if (!string.IsNullOrEmpty(userId))
            {
                btnDeleteUser.Visible = true;
                btnDeleteUser.CommandArgument = userId;
                btnDeleteUser.Attributes.Add("onclick", "return confirm('sure you want to delete this User?');");
                UserContainer userInfo = Cntrl.GetMemberShipUser(userId, true);
                ucUserDetail.Populate(null, userInfo, true, true);

                PopulateExistingUserRoles(userInfo);

                chkActive.Checked = userInfo.Active;
                chkLockedOut.Checked = userInfo.LockedOut;

                if (userInfo.IsSuperAdmin)
                {
                    WebUtil.SelectListItemByValue(ddlUserType, "{SuperAdmin}");
                }
                else if (userInfo.IsOwner)
                {
                    txtAllowableShowIdList.Text = userInfo.AllowableShowIdList;
                    WebUtil.SelectListItemByValue(ddlUserType, userInfo.OwnerId);
                }
            }
            else
            {
                ucUserDetail.Populate(null, null, true, false);
                chkActive.Checked = true;
                chkLockedOut.Checked = false;
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
            foreach (string role in Roles.GetAllRoles())
            {
                if (role != "Exhibitor")
                {
                    chkLstUserRoles.Items.Add(role);
                }
            }
        }

        private void LoadUserTypes()
        {
            ddlUserType.Items.Clear();
            List<Owner> owners = Cntrl.GetOwnerList();
            ddlUserType.DataTextField = "OwnerName";
            ddlUserType.DataValueField = "OwnerId";
            ddlUserType.DataSource = owners;
            ddlUserType.DataBind();

            ddlUserType.Items.Insert(0, new ListItem("-- Select One --", ""));
            ddlUserType.Items.Insert(1, new ListItem("{SuperAdmin}", "{SuperAdmin}"));
            
        }

        private void PickAUser(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                LoadUserDetail(userId);
            }
            else
            {
                ValidationResults errors = new ValidationResults();
                errors.AddResult(new ValidationResult("Must pick a user first.", null, null, null, null));
                Master.AddErrorMessages(errors);
            }
        }

      

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            LoadUserDetail(null);
        }

        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            UserContainer userDetail = ucUserDetail.BuildUserContainer();
            userDetail.ActualUserName = userDetail.PreferredUserName;

            List<string> userRoles = new List<string>();
            
            if (ddlUserType.SelectedValue == "{SuperAdmin}")
            {
                userDetail.IsSuperAdmin = true;
            }
            else
            {
                userDetail.IsOwner = true;
                userDetail.OwnerId = Util.ConvertInt32(ddlUserType.SelectedValue);
                userDetail.AllowableShowIdList = txtAllowableShowIdList.Text.Trim().Replace(",", ";").Replace(" ", string.Empty);
            }

            foreach (ListItem li in chkLstUserRoles.Items)
            {
                if (li.Selected)
                {
                    userRoles.Add(li.Value);
                }
            }

            userDetail.Active = chkActive.Checked;
            userDetail.LockedOut = chkLockedOut.Checked;

            OwnerAdminController ownerMgr = new OwnerAdminController();
            ValidationResults errors = ownerMgr.SaveUser(CurrentUser, userDetail, userRoles);

            if (errors.IsValid)
            {
                DisplayUsers();
                Master.DisplayFriendlyMessage("User Saved.");
            }
            else
            {
                Master.AddErrorMessages(errors);
            }
        }

        protected void btnCancelUser_Click(object sender, EventArgs e)
        {
            DisplayUsers();
        }

        protected void btnDeleteUser_Click(object sender, EventArgs e)
        {
            Cntrl.DeleteUser(((Button)sender).CommandArgument);
            this.DisplayUsers();
            this.Master.DisplayFriendlyMessage("User Deleted.");
        }
        
    }
}