using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using ExpoOrders.Common;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class SuperAdminMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Header.DataBind();
            if (!IsPostBack)
            {
            }
            else
            {
                this.HideFriendlyMessage();
            }
        }


        public void HideFriendlyMessage()
        {
            this.plcFriendlyMessage.Visible = this.ltrFriendlyMessage.Visible = false;
            this.ltrFriendlyMessage.Text = string.Empty;
        }

        public void DisplayFriendlyMessage(string message)
        {
            this.plcFriendlyMessage.Visible =
                this.ltrFriendlyMessage.Visible = true;

            this.ltrFriendlyMessage.Text += message + "<br/>";
        }

        public void DisplayFriendlyMessage(List<string> messages)
        {
            messages.ForEach(msg => { DisplayFriendlyMessage(msg); });
        }

        public void ClearErrors()
        {
            this.PageErrors.Visible = false;
        }

        public void AddErrorMessages(ValidationResults errors)
        {
            this.PageErrors.Visible = true;
            this.PageErrors.AddErrorMessages(errors);
        }

        public void AddErrorMessages(ValidationResults errors, string validationGroup)
        {
            this.PageErrors.Visible = true;
            foreach (ValidationResult error in errors)
            {
                this.PageErrors.AddErrorMessage(error, validationGroup);
            }
        }

        public void AddErrorMessage(string error)
        {
            this.PageErrors.Visible = true;
            this.PageErrors.AddErrorMessage(error);
        }

        public void AddErrorMessage(Exception ex)
        {
            AddErrorMessage(Util.AllExceptions(ex));
        }
    }
}