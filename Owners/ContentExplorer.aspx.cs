using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using ExpoOrders.Web.Owners.Common;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;

namespace ExpoOrders.Web.Owners
{
    public enum ContentExplorerPageMode { HtmlContent = 1, FileDownload = 2, DynamicForm = 3 }
    public partial class ContentExplorer : BaseOwnerPage
    {

        #region Private Members
        private List<NavigationLink> _pageNavigationLinks = OwnerUtil.BuildContentExplorerSubNavLinks();
        #endregion

        #region Manager Objects
        private HtmlContentController _contentMgr = null;
        public HtmlContentController ContentMgr
        {
            get
            {
                if (_contentMgr == null)
                {
                    return new HtmlContentController();
                }
                else
                {
                    return _contentMgr;
                }
            }
        }
        #endregion
       
        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.NavigationItemCallBack = this.HandleNavigationItemClicked;
            this.Master.PreviewShowCallBack = this.PreviewShow;

            this.ucFormQuestionEditorList.ItemSelected = ManageQuestion;
            this.ucFormQuestionEditorList.ItemDeleted = DeleteQuestion;


            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, OwnerPage.ContentExplorer, OwnerTabEnum.Content);
            this.Master.LoadSubNavigation("Content Explorer", _pageNavigationLinks);

            this.LoadPageMode(ContentExplorerPageMode.HtmlContent);

            this.Master.SelectNavigationItem(1);
        }

        private void LoadPageMode(ContentExplorerPageMode mode)
        {
            NavigationLink linkToSelect = _pageNavigationLinks.SingleOrDefault<NavigationLink>(l => l.TargetId == (int)mode);

            LoadPageMode(linkToSelect.NavigationLinkId, linkToSelect.TargetId.Value);
        }

        private void LoadPageMode(int navLinkId, int targetId)
        {
            if (navLinkId <= 0)
            {
                navLinkId = 1;
                targetId = (int)ContentExplorerPageMode.HtmlContent;
                this.Master.SelectNavigationItem(navLinkId);
            }

            ContentExplorerPageMode currentPageMode = (ContentExplorerPageMode)Enum.Parse(typeof(ContentExplorerPageMode), targetId.ToString(), true);

            plcHtmlContentList.Visible =
            plcManageHtmlContent.Visible =
            plcFileDownloadManager.Visible =
            plcManageFileDownload.Visible =
            plcDynamicFormList.Visible =
            plcManageDynamicForm.Visible =
            plcFormQuestionList.Visible =
            plcManageQuestion.Visible = false;

            switch (currentPageMode)
            {
                case ContentExplorerPageMode.HtmlContent:
                    LoadHtmlContentList();
                    LoadHtmlContentTypes();
                    plcHtmlContentList.Visible = true;
                    break;
                case ContentExplorerPageMode.FileDownload:
                    LoadFileDownloadsList();
                    break;
                case ContentExplorerPageMode.DynamicForm:
                    LoadDynamicFormsList();
                    plcDynamicFormList.Visible = true;
                    break;
            }

        }

        #endregion

        #region Control Events

        #region HtmlContent
        protected void grdvwHtmlContentList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HtmlContent htmlContent = (HtmlContent)e.Row.DataItem;

                Literal ltrType = (Literal)e.Row.FindControl("ltrHtmlContentType");
                ltrType.Text = htmlContent.ContentTypeCd;

                Literal ltrActive = (Literal)e.Row.FindControl("ltrActive");

                LinkButton lbtnActivateHtmlContent = (LinkButton)e.Row.FindControl("lbtnActivateHtmlContent");
                LinkButton lbtnDeactivateHtmlContent = (LinkButton)e.Row.FindControl("lbtnDeactivateHtmlContent");

                lbtnActivateHtmlContent.CommandArgument
                   = lbtnDeactivateHtmlContent.CommandArgument = htmlContent.HtmlContentId.ToString();

                if (htmlContent.ActiveFlag)
                {
                    ltrActive.Text = "Yes";
                    lbtnDeactivateHtmlContent.Visible = true;
                    lbtnActivateHtmlContent.Visible = false;
                }
                else
                {
                    ltrActive.Text = "No";
                    lbtnActivateHtmlContent.Visible = true;
                    lbtnDeactivateHtmlContent.Visible = false;
                }

            }
        }

        protected void grdvwHtmlContentList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int htmlContentId = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "EditHtmlContent":
                    ManageHtmlContent(htmlContentId);
                    break;
                case "DeactivateHtmlContent":
                    DeactivateHtmlContent(htmlContentId);
                    break;
                case "ActivateHtmlContent":
                    ActivateHtmlContent(htmlContentId);
                    break;
            }
        }

        private void ActivateHtmlContent(int htmlContentId)
        {
            ContentMgr.ActivateHtmlContent(htmlContentId);
            this.Master.DisplayFriendlyMessage("Html Content activated.");
            LoadHtmlContentList();
        }

        private void DeactivateHtmlContent(int htmlContentId)
        {
            ContentMgr.DeactivateHtmlContent(htmlContentId);
            this.Master.DisplayFriendlyMessage("Html Content de-activated.");
            LoadHtmlContentList();
        }

        private void ManageHtmlContent(int htmlContentId)
        {
            hdnHtmlContentId.Value = htmlContentId.ToString();
            OwnerUtil.ClearPlaceHolderControl(plcManageHtmlContent);

            htmlContentEditor.Content = string.Empty;

            if (htmlContentId > 0)
            {
                HtmlContent content = ContentMgr.GetHtmlContentById(htmlContentId);
                txtHtmlContentTitle.Text = content.Title;
                if (!string.IsNullOrEmpty(content.ContentTypeCd))
                {
                    ddlHtmlContentType.ClearSelection();
                    WebUtil.SelectListItemByValue(ddlHtmlContentType, content.ContentTypeCd);
                }
                txtEmailSubject.Text = content.EmailSubject;
                htmlContentEditor.Content = content.DynamicHtmlContent;
            }

            PopulateTokenList();
            plcManageHtmlContent.Visible = true;
            plcHtmlContentList.Visible = false;

            ConfigureImageManager(htmlContentEditor, CurrentUser.CurrentShow.ShowGuid, true, true);
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

        protected void grdvwHtmlContentList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdvwHtmlContentList.PageIndex = e.NewPageIndex;
            LoadHtmlContentList();
        }

        protected void btnRefreshHtmlContent_Click(object sender, EventArgs e)
        {
            LoadHtmlContentList();
        }

        protected void btnAddHtmlContent_Click(object sender, EventArgs e)
        {
            ManageHtmlContent(0);
        }

        protected void btnSaveHtmlContent_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "HtmlContentInformation";
            if (Page.IsValid)
            {
                SaveHtmlContent();
                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Html Content saved.");
                    LoadHtmlContentList();
                }
            }
        }

        protected void btnCancelSaveHtmlContent_Click(object sender, EventArgs e)
        {
            LoadHtmlContentList();
        }

        private void SaveHtmlContent()
        {
            HtmlContent content = new HtmlContent();
            content.HtmlContentId = Util.ConvertInt32(hdnHtmlContentId.Value);
            content.ShowId = this.CurrentUser.CurrentShow.ShowId;
            
            content.ActiveFlag = true;
            content.Title = txtHtmlContentTitle.Text.Trim();

            content.EmailSubject = txtEmailSubject.Text.Trim();

            content.ContentTypeCd = ddlHtmlContentType.SelectedValue;

            content.DynamicHtmlContent = this.htmlContentEditor.Content;

            ValidationResults errors = ContentMgr.SaveContent(CurrentUser, content);
            if (!errors.IsValid)
            {
                this.PageErrors.AddErrorMessages(errors);
            }
        }
        #endregion

        #region FileDownloads

        protected void grdvwFileDownloadList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                FileDownload fileDownload = (FileDownload)e.Row.DataItem;

                Literal ltrFileDownloadDescription = (Literal)e.Row.FindControl("ltrFileDownloadDescription");
                ltrFileDownloadDescription.Text = string.IsNullOrEmpty(fileDownload.FileDescription) ? string.Empty : fileDownload.FileDescription.Trim();

                Literal ltrFileSize = (Literal)e.Row.FindControl("ltrFileSize");

                Literal ltrContentType = (Literal)e.Row.FindControl("ltrContentType");

                Literal ltrActive = (Literal)e.Row.FindControl("ltrActive");

                LinkButton lbtnActivateFileDownload = (LinkButton)e.Row.FindControl("lbtnActivateFileDownload");
                LinkButton lbtnDeactivateFileDownload = (LinkButton)e.Row.FindControl("lbtnDeactivateFileDownload");

                HtmlAnchor lnkViewFileDownload = (HtmlAnchor)e.Row.FindControl("lnkViewFileDownload");
                Label lblMissingFile = (Label)e.Row.FindControl("lblMissingFile");
                

                lbtnActivateFileDownload.CommandArgument
                    = lbtnDeactivateFileDownload.CommandArgument = fileDownload.FileDownloadId.ToString();

                if (fileDownload.ActiveFlag)
                {
                    ltrActive.Text = "Yes";
                    lbtnDeactivateFileDownload.Visible = true;
                    lbtnActivateFileDownload.Visible = false;
                }
                else
                {
                    ltrActive.Text = "No";
                    lbtnActivateFileDownload.Visible = true;
                    lbtnDeactivateFileDownload.Visible = false;
                }

                if (!string.IsNullOrEmpty(fileDownload.FileName))
                {
                    try
                    {
                        string filePath = Server.MapPath(string.Format("~/Assets/Shows/{0}/Downloads/{1}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), fileDownload.FileName));
                        if (File.Exists(filePath))
                        {
                            FileInfo fi = new FileInfo(filePath);

                            ltrContentType.Text = fi.Extension;
                            ltrFileSize.Text = Util.ToByteString(fi.Length);

                            lblMissingFile.Visible = false;
                            lnkViewFileDownload.Visible = true;
                            lnkViewFileDownload.HRef = string.Format("~/Assets/Shows/{0}/Downloads/{1}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), fileDownload.FileName);
                        }
                        else
                        {
                            lblMissingFile.Visible = true;
                            lnkViewFileDownload.Visible = false;
                        }
                    }
                    catch (Exception)
                    {
                        lblMissingFile.Visible = true;
                        lnkViewFileDownload.Visible = false;
                    }
                }
            }
        }

        protected void btnUploadFileDownload_Click(object sender, EventArgs e)
        {
            string fileName = UploadFile();

            if (!string.IsNullOrEmpty(fileName))
            {
                txtFileName.Text = fileName;
            }
            plcFileDownloadDetail.Visible = true;
            btnSaveFileDownload.Visible = true;
        }

        private string UploadFile()
        {
            string fileName = string.Empty;

            string downloadDirectory = Server.MapPath(string.Format("~/Assets/Shows/{0}/Downloads", CurrentUser.CurrentShow.ShowGuid));

            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }

            if (this.fupFileDownload.HasFile)
            {
                fileName = Util.CleanFileName(fupFileDownload.FileName);
                string filePath = string.Format("{0}/{1}", downloadDirectory, fileName);

                fupFileDownload.SaveAs(filePath);
            }

            return fileName;
        }

        protected void btnCancelSaveFile_Click(object sender, EventArgs e)
        {
            LoadPageMode(ContentExplorerPageMode.FileDownload);
        }

        protected void grdvwFileDownloadList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int fileDownloadId = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "EditFileDownload":
                    ManageFileDownload(fileDownloadId);
                    break;
                case "DeactivateFileDownload":
                    DeactivateFileDownload(fileDownloadId);
                    break;
                case "ActivateFileDownload":
                    ActivateFileDownload(fileDownloadId);
                    break;
            }
        }

        private void ActivateFileDownload(int fileDownloadId)
        {
            ContentMgr.ActivateFileDownload(fileDownloadId);
            this.Master.DisplayFriendlyMessage("File Download activated.");
            LoadFileDownloadsList();
        }

        private void DeactivateFileDownload(int fileDownloadId)
        {
            ContentMgr.DeactivateFileDownload(fileDownloadId);
            this.Master.DisplayFriendlyMessage("File Download de-activated.");
            LoadFileDownloadsList();
        }

        private void ManageFileDownload(int fileDownloadId)
        {
            plcManageFileDownload.Visible = true;
            hdnFileDownloadId.Value = fileDownloadId.ToString();
            OwnerUtil.ClearPlaceHolderControl(plcManageFileDownload);

            plcFileDownloadDetail.Visible = false;
            btnSaveFileDownload.Visible = false;

            if (fileDownloadId > 0)
            {
                btnSaveFileDownload.Visible = true;
                plcFileDownloadDetail.Visible = true;

                FileDownload fileDownload = ContentMgr.GetFileDownloadById(fileDownloadId);
                txtFileName.Text = Util.ConvertEmpty(fileDownload.FileName);
                txtFileDescription.Text = Util.ConvertEmpty(fileDownload.FileDescription);
                txtFileDownloadContentType.Text = Util.ConvertEmpty(fileDownload.ContentType);
                txtPopUpHeight.Text = fileDownload.PopUpHeight.HasValue ? fileDownload.PopUpHeight.ToString() : string.Empty;
                txtPopupWidth.Text = fileDownload.PopUpWidth.HasValue ? fileDownload.PopUpWidth.ToString() : string.Empty;
            }

            plcFileDownloadManager.Visible = false;
            
        }

        protected void grdvwFileDownloadList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdvwFileDownloadList.PageIndex = e.NewPageIndex;
            LoadFileDownloadsList();
        }

        protected void btnRefreshFileDownloadList_Click(object sender, EventArgs e)
        {
            LoadFileDownloadsList();
        }

        protected void btnAddFileDownload_Click(object sender, EventArgs e)
        {
            ManageFileDownload(0);
        }

        protected void btnSaveFileDownload_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "FileDownloadInformation";
            if (Page.IsValid)
            {
                SaveFileDownload();
                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("File Download details saved.");
                    LoadFileDownloadsList();
                }
            }
        }

        private void SaveFileDownload()
        {
            int fileDownloadId = Util.ConvertInt32(hdnFileDownloadId.Value);

            FileDownload fileDownload = null;

            string fileName = UploadFile();

            if (fileDownloadId > 0)
            {
                fileDownload = ContentMgr.GetFileDownloadById(fileDownloadId);
            }
            else
            {
                fileDownload = new FileDownload();
                fileDownload.ShowId = this.CurrentUser.CurrentShow.ShowId;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = txtFileName.Text;
            }

            fileDownload.FileName = fileName;
            fileDownload.FileDescription = txtFileDescription.Text;
            fileDownload.ContentType = txtFileDownloadContentType.Text;
            fileDownload.PopUpHeight = string.IsNullOrEmpty(txtPopUpHeight.Text) ? 0 : Util.ConvertInt32(txtPopUpHeight.Text.Trim());
            fileDownload.PopUpWidth = string.IsNullOrEmpty(txtPopupWidth.Text) ? 0 : Util.ConvertInt32(txtPopupWidth.Text.Trim());
            fileDownload.ActiveFlag = true;

            if (fileDownload.FileDownloadId > 0)
            {
                ContentMgr.SaveFileDownload(fileDownload);
            }
            else
            {
                ContentMgr.CreateFileDownload(fileDownload);
            }
        }

        #endregion

        #region Dynamic Forms

        protected void grdvwDynamicFormList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Form form = (Form)e.Row.DataItem;

                Literal ltrFormDescription = (Literal)e.Row.FindControl("ltrFormDescription");
                ltrFormDescription.Text = string.IsNullOrEmpty(form.FormDescription) ? string.Empty : form.FormDescription.Trim();

                Literal ltrFormDeadline = (Literal)e.Row.FindControl("ltrSubmissionDeadline");
                ltrFormDeadline.Text = form.SubmissionDeadline.HasValue ? string.Format("{0:MM/dd/yyyy}", form.SubmissionDeadline) : string.Empty;

                Literal ltrActive = (Literal)e.Row.FindControl("ltrActive");

                LinkButton lbtnActivateDynamicForm = (LinkButton)e.Row.FindControl("lbtnActivateDynamicForm");
                LinkButton lbtnDeactivateDynamicForm = (LinkButton)e.Row.FindControl("lbtnDeactivateDynamicForm");

                lbtnActivateDynamicForm.CommandArgument
                   = lbtnDeactivateDynamicForm.CommandArgument = form.FormId.ToString();

                if (form.ActiveFlag)
                {
                    ltrActive.Text = "Yes";
                    lbtnDeactivateDynamicForm.Visible = true;
                    lbtnActivateDynamicForm.Visible = false;
                }
                else
                {
                    ltrActive.Text = "No";
                    lbtnActivateDynamicForm.Visible = true;
                    lbtnDeactivateDynamicForm.Visible = false;
                }

            }
        }

        protected void grdvwDynamicFormList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int formId = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "EditDynamicForm":
                    ManageDynamicForm(formId);
                    break;
                case "DeactivateDynamicForm":
                    DeactivateDynamicForm(formId);
                    break;
                case "ActivateDynamicForm":
                    ActivateDynamicForm(formId);
                    break;
            }
        }

        private void ActivateDynamicForm(int formId)
        {
            ContentMgr.ActivateDynamicForm(formId);
            this.Master.DisplayFriendlyMessage("Dynamic Form activated.");
            LoadDynamicFormsList();
        }

        private void DeactivateDynamicForm(int formId)
        {
            ContentMgr.DeactivateDynamicForm(formId);
            this.Master.DisplayFriendlyMessage("Dynamic Form de-activated.");
            LoadDynamicFormsList();
        }

        private void ManageDynamicForm(int formId)
        {
            hdnDynamicFormId.Value = formId.ToString();
            OwnerUtil.ClearPlaceHolderControl(plcManageDynamicForm);
            Form form = null;
            if (formId > 0)
            {
                form = ContentMgr.GetFormById(formId);
                if (form != null)
                {
                    txtFormName.Text = form.FormName.Trim();
                    txtFormDescription.Text = Util.ConvertEmpty(form.FormDescription);
                    txtSubmissionDeadline.Text = form.SubmissionDeadline.HasValue ? string.Format("{0:MM/dd/yyyy}", form.SubmissionDeadline) : string.Empty;
                }
            }

            plcManageDynamicForm.Visible = true;
            plcDynamicFormList.Visible = false;
        }

        protected void grdvwDynamicFormList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdvwDynamicFormList.PageIndex = e.NewPageIndex;
            LoadDynamicFormsList();
        }

        protected void btnRefreshDynamicFormList_Click(object sender, EventArgs e)
        {
            LoadDynamicFormsList();
        }

        protected void btnAddDynamicForm_Click(object sender, EventArgs e)
        {
            ManageDynamicForm(0);
        }

        protected void btnSaveDynamicForm_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "DynamicFormInformation";
            if (Page.IsValid)
            {
                SaveDynamicForm();
                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Dynamic Form saved.");
                    LoadDynamicFormsList();
                }
            }
        }

        private void SaveDynamicForm()
        {
            Form form = new Form();

            form.FormId = Util.ConvertInt32(hdnDynamicFormId.Value);
            form.ShowId = this.CurrentUser.CurrentShow.ShowId;

            form.FormTypeCd = FormTypeEnum.Dynamic.ToString();

            form.ActiveFlag = true;
            form.FormName = txtFormName.Text.Trim();
            form.FormDescription = txtFormDescription.Text.Trim();
            if (!string.IsNullOrEmpty(txtSubmissionDeadline.Text.Trim()))
            {
                form.SubmissionDeadline = Convert.ToDateTime(txtSubmissionDeadline.Text.Trim());
            }

            ContentMgr.SaveForm(form);
            hdnDynamicFormId.Value = form.FormId.ToString();
        }

        protected void lbtnConfigureQuestions_Click(object sender, EventArgs e)
        {
            LoadQuestionList(Util.ConvertInt32(hdnDynamicFormId.Value));
            plcFormQuestionList.Visible = true;
            plcManageDynamicForm.Visible = false;
        }

        #endregion

        #region Form Questions

        private void ManageQuestion(int questionId)
        {

            Question question = null;
            OwnerUtil.ClearPlaceHolderControl(plcManageQuestion);
            ucFormQuestionEditor.ClearQuestion();

            if (questionId > 0)
            {
                Form form = ContentMgr.GetFormById(Util.ConvertInt32(hdnDynamicFormId.Value));

                if (form != null)
                {
                    question = form.Questions.Where(q => q.QuestionId == questionId).SingleOrDefault();
                    if (question != null)
                    {
                        ucFormQuestionEditor.Populate(question);
                    }
                }
            }

            plcManageQuestion.Visible = true;
            plcFormQuestionList.Visible = false;
        }

        private void DeleteQuestion(int questionId)
        {
            ContentMgr.DeleteFormQuestion(questionId);

            LoadQuestionList(Util.ConvertInt32(hdnDynamicFormId.Value));
            plcManageQuestion.Visible = false;
            plcFormQuestionList.Visible = true;
        }


        protected void btnRefreshQuestionList_Click(object sender, EventArgs e)
        {
            LoadQuestionList(Util.ConvertInt32(hdnDynamicFormId.Value));
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            ManageQuestion(0);
        }

        protected void btnSaveQuestion_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "QuestionInformation";
            if (Page.IsValid)
            {
                SaveQuestion();
                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Question saved.");
                    LoadQuestionList(Util.ConvertInt32(hdnDynamicFormId.Value));
                    plcManageQuestion.Visible = false;
                    plcFormQuestionList.Visible = true;
                }
            }
        }

        private void SaveQuestion()
        {
            Question question = ucFormQuestionEditor.BuildQuestion();
            question.FormId = Util.ConvertInt32(hdnDynamicFormId.Value);

            ValidationResults errors = ContentMgr.SaveQuestion(question);

            if (!errors.IsValid)
            {
                PageErrors.AddErrorMessages(errors);
            }
        }

        protected void btnReturnToFormList_Click(object sender, EventArgs e)
        {
            LoadDynamicFormsList();
            plcDynamicFormList.Visible = true;
            plcFormQuestionList.Visible = false;
        }

        protected void btnReturnToQuestionList_Click(object sender, EventArgs e)
        {
            LoadQuestionList(Util.ConvertInt32(hdnDynamicFormId.Value));
            plcManageQuestion.Visible = false;
            plcFormQuestionList.Visible = true;
        }

        #endregion

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            this.LoadPageMode(navLinkId, targetId);
        }
        #endregion

        #region Methods

        private void LoadHtmlContentList()
        {
            plcManageHtmlContent.Visible = false;

            List<HtmlContent> htmlContentList = ContentMgr.GetHtmlContentListByShowId(this.CurrentUser.CurrentShow.ShowId, chkIncludeInactive.Checked);
            grdvwHtmlContentList.DataSource = htmlContentList;
            grdvwHtmlContentList.DataBind();

            plcHtmlContentList.Visible = true;
        }

        private void LoadHtmlContentTypes()
        {
            ddlHtmlContentType.Items.Clear();

            foreach (ContentTypeEnum contentType in Enum.GetValues(typeof(ContentTypeEnum)))
            {
                ddlHtmlContentType.Items.Add(new ListItem(contentType.ToString(), contentType.ToString()));
            }
        }

        private void LoadDynamicFormsList()
        {
            List<Form> formList = ContentMgr.GetFormListByShowId(this.CurrentUser.CurrentShow.ShowId, chkIncludeInactiveForms.Checked);
            grdvwDynamicFormList.DataSource = formList;
            grdvwDynamicFormList.DataBind();
        }

        private void LoadFileDownloadsList()
        {
            List<FileDownload> fileDownloadList = ContentMgr.GetFileDowloadListByShowId(this.CurrentUser.CurrentShow.ShowId, chkIncludeInactiveFileDownload.Checked);
            grdvwFileDownloadList.DataSource = fileDownloadList;
            grdvwFileDownloadList.DataBind();

            plcFileDownloadManager.Visible = true;
            plcManageFileDownload.Visible = false;

        }

        private void LoadQuestionList(int formId)
        {
            Form currentForm = ContentMgr.GetFormById(formId);
            if (currentForm != null)
            {
                ucFormQuestionEditorList.Populate(currentForm.Questions.OrderBy(q => q.SortOrder).ToList());

            }

        }
        #endregion



    }
}
