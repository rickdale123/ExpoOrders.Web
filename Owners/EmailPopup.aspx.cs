using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.Web.UI.HtmlControls;
using System.IO;

namespace ExpoOrders.Web.Owners
{
    public partial class EmailPopup : BaseOwnerPage
    {
        #region Manager Objects
        private HtmlContentController _contentMgr = null;
        public HtmlContentController ContentMgr
        {
            get
            {
                if (_contentMgr == null)
                {
                    _contentMgr = new HtmlContentController();
                }

                return _contentMgr;
            }
        }

        private OwnerAdminController _ownerAdminMgr = null;
        public OwnerAdminController OwnerAdminMgr
        {
            get
            {
                if (_ownerAdminMgr == null)
                {
                    _ownerAdminMgr = new OwnerAdminController();
                }

                return _ownerAdminMgr;

            }
        }

        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            validationErrors.Visible = false;
            LoadEmailTemplates();

            if (this.CurrentUser.EmailTransportObject.SelectedExhibitorIds.Count > 0)
            {
                PopulateEmailControls(CurrentUser.EmailTransportObject.EmailType);
            }
            else
            {
                PopulateEmailControls(ContentTypeEnum.EmailTemplate);
            }

            LoadStyleSheet(CurrentUser.CurrentShow);

            ConfigureImageManager(htmlContentEditor, CurrentUser.CurrentShow.ShowGuid, true, false);
        }

        private void LoadStyleSheet(Entities.Show currentShow)
        {
            this.OwnerStyleSheet.Href = string.Format("~/Assets/Shows/{0}/style.css", currentShow.ShowGuid);
        }

        private void PopulateEmailControls(ContentTypeEnum editorMode)
        {
            lnkBtnViewUsers.Text = "View Recipients";
            plcEmailMessage.Visible = true;
            EmailTransportEntity emailInfo = CurrentUser.EmailTransportObject;
            hdnEditorMode.Value = editorMode.ToString();

            if (emailInfo != null)
            {
                if (emailInfo.SelectedExhibitorIds != null && emailInfo.SelectedExhibitorIds.Count > 0)
                {
                    string exhibitorPluralityLabel = emailInfo.SelectedExhibitorIds.Count > 1 ? "exhibitors" : "exhibitor";
                    lnkBtnViewUsers.Text += string.Format(" ({0} {1})", emailInfo.SelectedExhibitorIds.Count, exhibitorPluralityLabel);
                    if (editorMode != ContentTypeEnum.EmailTemplate)
                    {
                        txtToAddress.Text = string.Format("{0} ({1} {2})", CurrentUser.CurrentShow.ShowName, emailInfo.SelectedExhibitorIds.Count, exhibitorPluralityLabel);
                        txtToAddress.Enabled = false;
                        txtFromAddress.Text = CurrentUser.CurrentShow.OrderFromEmail;
                        txtReplyToAddress.Text = CurrentUser.CurrentShow.OrderReplyEmail;
                        PopulateTokenList();
                        PopulateMessageBodyFromTemplate(editorMode);

                        PopulateAttachmentList(emailInfo.AttachmentList);
                    }
                    else
                    {
                        if (emailInfo.ToEmailAddressList != null)
                        {

                            if (emailInfo.ToEmailAddressList.Count > 0)
                            {
                                StringBuilder toAddressList = new StringBuilder();

                                emailInfo.ToEmailAddressList.ForEach(
                                    emailAddress =>
                                    {
                                        toAddressList.Append(string.Format("{0};", emailAddress));
                                    }
                                    );

                                txtToAddress.Text = toAddressList.ToString();
                            }
                        }

                        txtFromAddress.Text = CurrentUser.CurrentShow.OrderFromEmail;
                        txtReplyToAddress.Text = CurrentUser.CurrentShow.OrderReplyEmail;
                        PopulateTokenList();
                    }
                }
            }
        }


        private void PopulateMessageBodyFromTemplate(ContentTypeEnum editorMode)
        {
            HtmlContentController cntrl = new HtmlContentController();
            HtmlContent content = cntrl.GetHtmlContentListByShowId(CurrentUser.CurrentShow.ShowId, false).Where(h => h.ContentTypeCd == editorMode.ToString()).FirstOrDefault();

            if (content != null)
            {
                this.htmlContentEditor.Content = content.DynamicHtmlContent;
                ConfigureImageManager(htmlContentEditor, CurrentUser.CurrentShow.ShowGuid, true, false);

                txtSubject.Text = content.EmailSubject;

                SelectEmailTemplateItem(content.HtmlContentId);
            }
        }

        private void SelectEmailTemplateItem(int emailTemplateId)
        {
            if (ddlEmailTemplate.Items.Count > 0)
            {
                ListItem li = ddlEmailTemplate.Items.FindByValue(emailTemplateId.ToString());
                if (li != null)
                {
                    li.Selected = true;
                }
            }
        }
       
        private void PopulateTokenList()
        {
            StringBuilder sb = new StringBuilder();

            foreach (DynamicContentTokens token in Enum.GetValues(typeof(DynamicContentTokens)))
            {
                sb.Append(string.Format("{0}\n", token.GetCodeValue()));
            }

            txtTokens.Text = sb.ToString();
        }

        private void PopulateAttachmentList(List<string> attachmentList)
        {
            //Please evalulate for NULL as needed.
            if (attachmentList != null)
            {
                List<string> fileAttachments = new List<string>();

                foreach (string attachment in attachmentList)
                {
                    FileInfo attachmentFileInfo = new FileInfo(attachment);
                    fileAttachments.Add(attachmentFileInfo.Name);
                }

                rptrAttachmentList.DataSource = fileAttachments;
                rptrAttachmentList.DataBind();
                rptrAttachmentList.Visible = true;
            }
        }

        protected void btnSendEmail_onclick(object sender, EventArgs e)
        {
            ValidationResults errors = new ValidationResults();

            if(string.IsNullOrEmpty(txtSubject.Text.Trim()))
            {
                errors.AddResult(new ValidationResult("Must provide a Subject.", null, null, null,null));
            }
            if (string.IsNullOrEmpty(this.htmlContentEditor.Content.Trim()))
            {
                errors.AddResult(new ValidationResult("Must provide a Body.", null, null, null,null));
            }

            if (errors.IsValid)
            {
                ContentTypeEnum editorMode = Enum<ContentTypeEnum>.Parse(hdnEditorMode.Value, true);
                SendEmail(txtToAddress.Text.Trim(), txtFromAddress.Text.Trim(), txtReplyToAddress.Text.Trim(), txtSubject.Text.Trim(), this.htmlContentEditor.Content, editorMode);
            }
            else
            {
                validationErrors.AddErrorMessages(errors);
            }
        }

        protected void rptrAttachmentList_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string attachmentFileName = (string)e.Item.DataItem;
                HtmlAnchor lnkFileAttachment = (HtmlAnchor)e.Item.FindControl("lnkFileAttachment");
                lnkFileAttachment.InnerText = attachmentFileName;

                if (attachmentFileName == "{ExhibitorInvoice}")
                {
                    lnkFileAttachment.HRef = "#";
                    lnkFileAttachment.Attributes.Add("onclick", "return false;");
                }
                else
                {
                    lnkFileAttachment.HRef = string.Format("~/Assets/Shows/{0}/Attachments/{1}", CurrentUser.CurrentShow.ShowGuid, attachmentFileName);
                }
                
            }
        }

        protected void btnUploadAttachment_Click(object sender, EventArgs e)
        {
            if (this.fupFileUpload.HasFile)
            {
                string filePath = Server.MapPath(string.Format("~/Assets/Shows/{0}/Attachments/{1}", CurrentUser.CurrentShow.ShowGuid, Util.CleanFileName(fupFileUpload.FileName)));

                fupFileUpload.SaveAs(filePath);

                EmailTransportEntity emailInfo = CurrentUser.EmailTransportObject;
                if (emailInfo.AttachmentList == null)
                {
                    emailInfo.AttachmentList = new List<string>();
                }
                emailInfo.AttachmentList.Add(filePath);
                PopulateAttachmentList(emailInfo.AttachmentList);
            }
        }
       

        #endregion

        #region Control Events

        private void SendEmail(string toAddress, string fromAddress, string replyAddress, string subject, string body, ContentTypeEnum editorMode)
        {
            PageErrors.ValidationGroup = "EmailInformation";
            int emailCount = 0;

            bool saveInviteFlag = (editorMode == ContentTypeEnum.WelcomeKit);

            List<string> userExclusionList = BuildUserExclusionList();

            if (CurrentUser.EmailTransportObject != null)
            {
                if (AttachmentContains(CurrentUser.EmailTransportObject, "{ExhibitorInvoice}"))
                {
                    CurrentUser.EmailTransportObject.AttachmentStagingFolderPath = GenerateExhibitorInvoices(CurrentUser.CurrentShow.ShowId, CurrentUser.EmailTransportObject, userExclusionList);
                }
            }

            emailCount = ContentMgr.EmailSelectedExhibitors(this.CurrentUser, body, fromAddress, replyAddress, subject, true, saveInviteFlag, userExclusionList);

            if (emailCount > 0)
            {
                DisplayFriendlyMessage(string.Format("{0} Message(s) sent.", emailCount));

                CurrentUser.EmailTransportObject.ClearAttachmentList();
                CurrentUser.EmailTransportObject.ClearSelectedExhibitors();
                plcEmailMessage.Visible = false;
            }
            else
            {
                PageErrors.AddErrorMessage("No Emails sent, check the logs and/or recipient list.");
            }
        }

        private string GenerateExhibitorInvoices(int showId, EmailTransportEntity emailTransObj, List<string> userExclusionList)
        {
            string exhibitorInvoiceStagingPath = string.Empty;
            if (emailTransObj.SelectedExhibitorIds != null)
            {
                emailTransObj.SelectedExhibitorIds.ForEach(exhibitorId =>
                {
                    List<UserContainer> targetUsers = ContentMgr.FindTargetUsers(showId, exhibitorId);

                    targetUsers.ForEach(tu =>
                    {
                        if (!userExclusionList.Contains(tu.UserId))
                        {
                            exhibitorInvoiceStagingPath = base.StageExhibitorInvoice(CurrentUser.CurrentShow, exhibitorId, tu);
                        }
                    });
                }
                );
            }
            return exhibitorInvoiceStagingPath;
        }

        private bool AttachmentContains(EmailTransportEntity emailTransObject, string attachment)
        {
            bool contains = false;
            if (emailTransObject != null && emailTransObject.AttachmentList != null && emailTransObject.AttachmentList.Count > 0)
            {
                contains = emailTransObject.AttachmentList.Contains(attachment);
            }
            return contains;
    }

        private List<string> BuildUserExclusionList()
        {
            List<string> excludedUserIds = new List<string>();
            if (plcEmailRecipients.Visible && this.rptrRecipientList.Visible && rptrRecipientList.Items.Count > 0)
            {
                foreach (RepeaterItem item in rptrRecipientList.Items)
                {
                    CheckBox chkRecipient = (CheckBox)item.FindControl("chkRecipient");
                    HiddenField hdnRecipientValue = (HiddenField)item.FindControl("hdnRecipientValue");
                    if (!chkRecipient.Checked)
                    {
                        excludedUserIds.Add(hdnRecipientValue.Value);
                    }
                }
                
            }
            return excludedUserIds;
        }

        public void DisplayFriendlyMessage(string message)
        {
            plcFriendlyMessage.Visible =
                ltrFriendlyMessage.Visible = true;

            ltrFriendlyMessage.Text += message + "<br/>";
        }

        protected void lnkBtnViewUsers_Click(object sender, EventArgs e)
        {
            plcEmailRecipients.Visible = true;

            rptrRecipientList.Visible = false;

            if (CurrentUser.EmailTransportObject.SelectedExhibitorIds != null)
            {
                EmailController cntrl = new EmailController();

                Dictionary<string, string> userEmails = cntrl.GetUserEmailList(CurrentUser, CurrentUser.EmailTransportObject.SelectedExhibitorIds);

                if (userEmails != null)
                {
                    rptrRecipientList.DataSource = userEmails;
                    rptrRecipientList.DataBind();
                    rptrRecipientList.Visible = true;
                }
            }
        }

        protected void lnkPreview_Click(object sender, EventArgs e)
        {
            if (sender is LinkButton)
            {
                string recipientUserId = ((LinkButton)sender).CommandArgument;

                CurrentUser.EmailPreview = new EmailPreviewContainer();

                CurrentUser.EmailPreview.ToUserId = recipientUserId;

                CurrentUser.EmailPreview.EmailFrom = txtFromAddress.Text.Trim();
                CurrentUser.EmailPreview.EmailReplyTo = txtReplyToAddress.Text.Trim();
                CurrentUser.EmailPreview.EmailSubject = txtSubject.Text.Trim();
                CurrentUser.EmailPreview.EmailBody = this.htmlContentEditor.Content.Trim();

                CurrentUser.EmailPreview.ShowId = CurrentUser.CurrentShow.ShowId;

                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "emailPreview", "<script language='javascript'>launchEmailPreview();</script>");

                
            }
        }

        protected void rptrRecipientList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CheckBox chkRecipient = (CheckBox)e.Item.FindControl("chkRecipient");
                HiddenField hdnRecipientValue = (HiddenField)e.Item.FindControl("hdnRecipientValue");
                LinkButton lnkPreview = (LinkButton)e.Item.FindControl("lnkPreview");

                KeyValuePair<string, string> itemData = (KeyValuePair<string, string>)e.Item.DataItem;
                chkRecipient.Text = itemData.Value;
                chkRecipient.Checked = true;
                hdnRecipientValue.Value = lnkPreview.CommandArgument = itemData.Key;
            }
        }

        private void LoadEmailTemplates()
        {
            ddlEmailTemplate.Items.Clear();

            HtmlContentController cntrl = new HtmlContentController();
            List<HtmlContent> emailTemplates = cntrl.GetHtmlContentListByShowId(CurrentUser.CurrentShow.ShowId, false).Where(h => 
                            h.ContentTypeCd == ContentTypeEnum.EmailTemplate.ToString()
                        || h.ContentTypeCd == ContentTypeEnum.PasswordReminder.ToString()
                        || h.ContentTypeCd == ContentTypeEnum.InvoiceEmail.ToString()
                        || h.ContentTypeCd == ContentTypeEnum.WelcomeKit.ToString()).OrderBy(h => h.ContentTypeCd).OrderBy(h => h.Title).ToList();

            if (emailTemplates != null && emailTemplates.Count > 0)
            {
                ddlEmailTemplate.DataValueField = "HtmlContentId";
                ddlEmailTemplate.DataTextField = "Title";
                ddlEmailTemplate.DataSource = emailTemplates;
                ddlEmailTemplate.DataBind();
            }

            ddlEmailTemplate.Items.Insert(0, new ListItem("-- Select One --", string.Empty));
        }

        private void PopulateFromEmailTemplateId(int emailTemplateId)
        {
            HtmlContentController cntrl = new HtmlContentController();
            HtmlContent emailContent = cntrl.GetHtmlContentById(emailTemplateId);
            if (emailContent != null)
            {
                txtSubject.Text = emailContent.EmailSubject;
                this.htmlContentEditor.Content = emailContent.DynamicHtmlContent;
                ConfigureImageManager(htmlContentEditor, CurrentUser.CurrentShow.ShowGuid, true, false);
            }
        }

        protected void ddlEmailTemplate_IndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlEmailTemplate.SelectedValue))
            {
                PopulateFromEmailTemplateId(Util.ConvertInt32(ddlEmailTemplate.SelectedValue));
            }
        }

        #endregion
    }
}