#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ExpoOrders.Entities;
using ExpoOrders.Common;
using ExpoOrders.Controllers;
using Microsoft.Reporting.WebForms;
using System.Text;
#endregion

namespace ExpoOrders.Web.Exhibitors
{

    public partial class Shipping : BaseExhibitorPage
    {
        #region Public Members
       
        private ReportController _reportMgr = null;
        public ReportController ReportMgr
        {
            get
            {
                if (_reportMgr == null)
                {
                    _reportMgr = new ReportController(ReportControllerType.Remote);
                }

                return _reportMgr;
            }
        }

        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.OnNavigationItemCallBack = this.HandleNavigationItemClicked;
            this.Master.DisplayCountDownTicker(CurrentUser.CurrentShow);


            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, ExhibitorPageEnum.ShippingInfo);

            if (TempNavTabRedirect != null && TempNavTabRedirect.TargetId.HasValue)
            {
                LoadOutboundShippingLabel(TempNavTabRedirect.NavLinkId.Value, TempNavTabRedirect.TargetId.Value);
                ResetNavTabRedirect();
            }
            else
            {
                //Display the Default content that May be defined at the Tab level
                TabLink currentTab = FindCurrentTab(ExhibitorPageEnum.ShippingInfo);
                if (currentTab != null)
                {
                    this.Master.DisplayTabContent(currentTab);
                }
            }
            
        }

        #endregion

        #region Navigation Related

        private void LoadOutboundShippingLabel(int navLinkId, int targetId)
        {
            btnGenerateOutboundShippingLabel.Attributes.Add("OnClick", string.Format("generateOutboundShippingLabel('{0}'); return false;", targetId));
            LoadExhibitorCompanyData();
            LoadOutboundShippingAdddresses();

            plcOutboundAddress.Visible = true;
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            plcOutboundAddress.Visible = false;
            WebUtil.HideForm(plcDynamicForm, ucFormQuestions);
            this.Master.HideDynamicContent();

            switch (action)
            {
                case NavigationLink.NavigationalAction.OutboundShippingLabel:
                    LoadOutboundShippingLabel(navLinkId, targetId);
                    break;
                case NavigationLink.NavigationalAction.HtmlContent:
                    this.Master.DisplayDynamicContent(string.Empty, HtmlController.GetHtmlContentById(targetId));
                    break;
                case NavigationLink.NavigationalAction.ShowCategory:
                    RedirectBoothCategory(navLinkId, targetId, this.Master.ParentNavigationText(navLinkId));
                    break;
                case NavigationLink.NavigationalAction.DynamicForm:
                    if (targetId > 0)
                    {
                        hdnFormId.Value = targetId.ToString();
                        WebUtil.DisplayForm(CurrentUser, this.FormMgr.GetFormInfoById(targetId), plcDynamicForm, ucFormQuestions, lblFormSubmissionDeadlineError, ltrFormName, ltrFormDescription, btnSubmitForm);
                    }
                    break;
                default:
                    this.Master.DisplayFriendlyMessage("Unhandled NavLinkId:" + navLinkId.ToString() + " Action: " + action.ToString() + " TargetId:" + targetId.ToString());
                    break;
            }
        }

        protected void btnSubmitForm_Click(object sender, EventArgs e)
        {
            List<string> missingRequiredQuestions = new List<string>();
            List<QuestionAnswer> questionAnswerList = this.ucFormQuestions.GetResponses(ref missingRequiredQuestions);

            if (missingRequiredQuestions.Count == 0)
            {
                OrderConfirmation confirmation = this.FormMgr.CreateFormOrder(CurrentUser, questionAnswerList, Util.ConvertInt32(hdnFormId.Value));

                if (confirmation != null && confirmation.Errors.IsValid)
                {
                    ConfirmationOrder = confirmation;

                    Server.Transfer("Orders.aspx", false);
                }
                else
                {
                    PageErrors.AddErrorMessage(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult("There was an error submitting your form. Please try again.", null, null, null, null), "FormSubmission");
                }
            }
            else
            {

                missingRequiredQuestions.ForEach(missing =>
                {
                    PageErrors.AddErrorMessage(new Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult(string.Format("A response to '{0}' is required.", missing), null, null, null, null), "FormSubmission");
                }
                );
            }
        }

        #endregion

        #region Company Outbound Shipping Info
       

        private void LoadExhibitorCompanyData()
        {
            txtCompanyName.Text = this.CurrentUser.CurrentExhibitor.ExhibitorCompanyName;

            txtAddressLine1.Text = this.CurrentUser.CurrentExhibitor.Address.Street1;
            txtAddressLine2.Text = this.CurrentUser.CurrentExhibitor.Address.Street2;
            txtAddressLine3.Text = string.Empty;
            txtAddressLine4.Text = string.Empty;
            txtCity.Text = this.CurrentUser.CurrentExhibitor.Address.City;
            txtPostalCode.Text = this.CurrentUser.CurrentExhibitor.Address.PostalCode;
            txtState.Text = this.CurrentUser.CurrentExhibitor.Address.StateProvinceRegion;
            txtCountry.Text = this.CurrentUser.CurrentExhibitor.Address.Country;
        }

        private void LoadOutboundShippingAdddresses()
        {
            plcAddressList.Visible = false;
            AccountController acctCntrl = new AccountController();
            
            AccountController accountMgr = new AccountController(CurrentUser);
            List<Address> outboundAddresses = accountMgr.GetExhibitorById(CurrentUser.CurrentExhibitor.ExhibitorId).Addresses.Where(a => a.ActiveFlag == true && a.AddressTypeCd == "Outbound").ToList();

            if (outboundAddresses != null && outboundAddresses.Count > 0)
            {
                plcAddressList.Visible = true;
                grdvwAddressList.DataSource = outboundAddresses;
                grdvwAddressList.DataBind();
            }
            
        }

        protected void grdvwAddressList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Address address = (Address)e.Row.DataItem;

                Literal ltrOtherFullAddress = (Literal)e.Row.FindControl("ltrOtherFullAddress");
                Label lblAddressType = (Label)e.Row.FindControl("lblAddressType");


                string addressType = address.AddressTypeCd;
                if (address.AddressTypeCd == "Outbound")
                {
                    addressType = "Outbound Shipping";
                }
                lblAddressType.Text = addressType;

                StringBuilder fullAddress = new StringBuilder();
                if (!string.IsNullOrEmpty(address.Street1))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street1, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street2))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street2, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street3))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street3, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street4))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street4, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street5))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street5, "<br/>"));
                }

                //string.Concat(address.Street1, "<br/>", address.Street2, "<br/>", address.Street3, "<br/>", address.Street4, "<br/>", address.Street5).Trim());
                if (string.Concat(address.City + address.StateProvinceRegion + address.PostalCode).Trim().Length > 0)
                {
                    fullAddress.Append(string.Format("{0}, {1}  {2}", address.City, address.StateProvinceRegion, address.PostalCode));
                }

                if (!string.IsNullOrEmpty(address.Country))
                {
                    fullAddress.Append(address.Country);
                }

                ltrOtherFullAddress.Text = fullAddress.ToString();

            }
        }

        protected void grdvwAddressList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int addressId = Util.ConvertInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "LoadAddress":
                    LoadAddress(addressId);
                    break;
            }
        }

        private void LoadAddress(int addressId)
        {
            AccountController accountMgr = new AccountController(CurrentUser);
            Address addressOnFile = accountMgr.GetAddress(CurrentUser, addressId);

            if (addressOnFile != null)
            {
                txtCompanyName.Text = this.CurrentUser.CurrentExhibitor.ExhibitorCompanyName;

                txtAddressLine1.Text = addressOnFile.Street1;
                txtAddressLine2.Text = addressOnFile.Street2;
                txtAddressLine3.Text = addressOnFile.Street3;
                txtAddressLine4.Text = addressOnFile.Street4;
                txtCity.Text = addressOnFile.City;
                txtPostalCode.Text = addressOnFile.PostalCode;
                txtState.Text = addressOnFile.StateProvinceRegion;
                txtCountry.Text = addressOnFile.Country;
            }
        }


        #endregion



    }
}