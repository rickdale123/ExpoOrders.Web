using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using ExpoOrders.Entities;
using ExpoOrders.Common;
using ExpoOrders.Controllers;
using AjaxControlToolkit;

namespace ExpoOrders.Web.Exhibitors
{

    public partial class Forms : BaseExhibitorPage
    {

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
            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, ExhibitorPageEnum.RequiredForms);

            //Display the Default content that May be defined at the Tab level
            TabLink currentTab = FindCurrentTab(ExhibitorPageEnum.RequiredForms);
            if (currentTab != null && Request["formId"] == null)
            {
                this.Master.DisplayTabContent(currentTab);
            }
            else if (Request["formId"] != null && int.Parse(Request["formId"].ToString()) > 0)
            {
                WebUtil.DisplayForm(CurrentUser, this.FormMgr.GetFormInfoById(Util.ConvertInt32(Request["formId"])), plcDynamicForm, ucFormQuestions, lblFormSubmissionDeadlineError, ltrFormName, ltrFormDescription, btnSubmitForm);
            }
        }

        #region Process Form

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

        #region Navigation Related

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            WebUtil.HideForm(plcDynamicForm, ucFormQuestions);
            this.Master.HideDynamicContent();

            switch (action)
            {
                case NavigationLink.NavigationalAction.DynamicForm:
                    if (targetId > 0)
                    {
                        hdnFormId.Value = targetId.ToString();
                        WebUtil.DisplayForm(CurrentUser, this.FormMgr.GetFormInfoById(targetId), plcDynamicForm, ucFormQuestions, lblFormSubmissionDeadlineError, ltrFormName, ltrFormDescription, btnSubmitForm);
                    }
                    break;
                case NavigationLink.NavigationalAction.HtmlContent:
                    if (targetId > 0)
                    {
                        this.Master.DisplayDynamicContent(string.Empty, HtmlController.GetHtmlContentById(targetId));
                    }
                    break;
                case NavigationLink.NavigationalAction.ShowCategory:
                    RedirectBoothCategory(navLinkId, targetId, this.Master.ParentNavigationText(navLinkId));
                    break;
                case NavigationLink.NavigationalAction.OutboundShippingLabel:
                    RedirectOutboundShippingLabel(navLinkId, targetId);
                    break;
                default:
                    this.Master.DisplayFriendlyMessage("Unhandled NavLinkId:" + navLinkId.ToString() + " Action: " + action.ToString() + " TargetId:" + targetId.ToString());
                    break;
            }
            
        }
        
        #endregion

    }
}