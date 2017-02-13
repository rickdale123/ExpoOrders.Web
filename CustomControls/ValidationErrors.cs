using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

using ExpoOrders.Entities;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace ExpoOrders.Web.CustomControls
{
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.Demand, Level=AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ValidationErrors : System.Web.UI.WebControls.ValidationSummary
    {
        public ValidationErrors()
        {
        }

        
        public string ErrorHeader
        {
            get { return base.HeaderText; }
            set { base.HeaderText = value; }
        }

        public void DisplayException(Exception ex)
        {
            Exception innerEx = ex;
            do
            {
                AddErrorMessage(innerEx.Message);
                innerEx = innerEx.InnerException;
            }
            while (innerEx != null);
        }

        public void AddErrorMessage(string errorMessage)
        {
            ValidationResult error = new ValidationResult(errorMessage, null, null, null, null);
            AddErrorMessage(error, this.ValidationGroup);
        }

        public void AddErrorMessage(ValidationResult error, string validationGroup)
        {
            this.ValidationGroup = validationGroup;
            this.Page.Form.Controls.Add(new CustomValidator()
            {
                Text = error.Tag,
                ErrorMessage = error.Message,
                Visible = false,
                IsValid = false,
                ValidationGroup = validationGroup
            });

            this.Visible = true;
        }

        public void AddErrorMessages(Exception lastException)
        {
            Exception ex = lastException;
            do
            {
                AddErrorMessage( new ValidationResult(ex.Message, null, null, null, null), this.ValidationGroup);

                ex = ex.InnerException;
            }
            while (ex != null);
        }


        public void AddErrorMessages(ValidationResults errors)
        {
            AddErrorMessages(errors, this.ValidationGroup);
        }

        public void AddErrorMessages(ValidationResults errors, string validationGroup)
        {
            foreach (ValidationResult error in errors)
            {
                this.AddErrorMessage(error, validationGroup);
            }
        }

    }
}