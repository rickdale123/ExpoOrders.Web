using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

using ExpoOrders.Entities;
using ExpoOrders.Common;
using ExpoOrders.Controllers;

namespace ExpoOrders.Web.CustomControls
{
    public partial class UserDetail : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Clear(string validationGroup)
        {
            setValidationGroup(validationGroup);
            this.hdnUserId.Value = string.Empty;
            this.txtUserFirstName.Text = string.Empty;
            this.txtUserLastName.Text = string.Empty;
            this.txtUserTitle.Text = string.Empty;
            this.txtUserEmailAddress.Text = string.Empty;
            this.txtUserPhone.Text = string.Empty;

            this.txtUserName.Text = lblUserName.Text = string.Empty;
            
            this.hdnActualUserName.Value = string.Empty;
            this.txtUserPassword.Text = string.Empty;
        }

        private void setValidationGroup(string validationGroup)
        {
            reqPassword.ValidationGroup = validationGroup;
            reqUserEmailAddress.ValidationGroup = validationGroup;
            reqUserName.ValidationGroup = validationGroup;
        }

        public void Populate(string validationGroup, UserContainer userInfo)
        {
            Populate(validationGroup, userInfo, false, false);
        }

        public void Populate(string validationGroup, UserContainer userInfo, bool useActualUserName, bool readonlyUserName)
        {
            Visible = true;

            setValidationGroup(validationGroup);

            hdnUserId.Value = string.Empty;
            hdnIsPrimary.Value = string.Empty;

            txtUserTitle.Text = string.Empty;
            txtUserFirstName.Text = string.Empty;
            txtUserLastName.Text = string.Empty;

            txtUserEmailAddress.Text = string.Empty;
            txtUserPhone.Text = string.Empty;

            txtUserName.Text = lblUserName.Text = string.Empty;
            txtUserPassword.Text = string.Empty;

            txtUserName.Visible = lblUserName.Visible = false;

            if (userInfo != null)
            {
                hdnUserId.Value = userInfo.UserId.ToString();
                hdnIsPrimary.Value = userInfo.IsPrimary.ToString();

                txtUserTitle.Text = userInfo.Title;
                txtUserFirstName.Text = userInfo.FirstName;
                txtUserLastName.Text = userInfo.LastName;

                txtUserEmailAddress.Text = userInfo.Email;
                txtUserPhone.Text = userInfo.Phone;

                if (useActualUserName)
                {
                    txtUserName.Text = lblUserName.Text = userInfo.ActualUserName;
                }
                else
                {
                    txtUserName.Text = lblUserName.Text = userInfo.PreferredUserName;
                }

                hdnActualUserName.Value = userInfo.ActualUserName;
                
                txtUserPassword.Text = userInfo.Password;
            }

            if (readonlyUserName)
            {
                txtUserName.Visible = false;
                lblUserName.Visible = true;
            }
            else
            {
                txtUserName.Visible = true;
                lblUserName.Visible = false;
            }
        }

        public UserContainer BuildUserContainer()
        {
            UserContainer user = new UserContainer();
            user.UserId = this.hdnUserId.Value;
            user.FirstName = this.txtUserFirstName.Text.Trim();
            user.LastName = this.txtUserLastName.Text.Trim();
            user.Title = this.txtUserTitle.Text.Trim();
            user.Phone = this.txtUserPhone.Text.Trim();
            user.Email = this.txtUserEmailAddress.Text.Trim();
            user.PreferredUserName = this.txtUserName.Visible ? this.txtUserName.Text.Trim() : this.lblUserName.Text.Trim();
            user.Password = this.txtUserPassword.Text.Trim();
            user.ActualUserName = hdnActualUserName.Value;
            user.IsPrimary = Util.ConvertBool(hdnIsPrimary.Value);
            user.Active = true;

            return user;
        }
    }
}