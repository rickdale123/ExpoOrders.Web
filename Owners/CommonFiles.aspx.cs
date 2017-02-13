using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Controllers;
using ExpoOrders.Common;
using ExpoOrders.Entities;
using Telerik.Web.UI;
using System.IO;
using System.Web.UI.HtmlControls;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.Text;

namespace ExpoOrders.Web.Owners
{
    public partial class CommonFiles : BaseOwnerPage
    {
        public enum CommonPageMode { ImagesFiles = 1, OwnerSettings = 2, HostConfigs = 3 }

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
            if (!Directory.Exists(WebUtil.OwnerSharedFileDirectory(CurrentUser.CurrentOwner)))
            {
                throw new Exception("Cannot load Owner Folder, emtpy");
            }

            this.Master.LoadMasterPage(this.CurrentUser, OwnerPage.CommonFiles);
            this.Master.LoadSubNavigation("Common", OwnerAdminMgr.GetCommonFilesSubNavigation());

            Master.SelectNavigationItem(1);

            LoadPageMode(1, CommonPageMode.ImagesFiles);

        }

        private void LoadPageMode(int navLinkId, int targetId)
        {
            LoadPageMode(navLinkId, (CommonPageMode)Enum.Parse(typeof(CommonPageMode), targetId.ToString(), true));
        }

        private void LoadPageMode(int navLinkId, CommonPageMode currentPageMode)
        {
            plcFileList.Visible = false;
            plcOwnerConfig.Visible = false;
            plcOwnerHostConfigs.Visible = false;

            switch (currentPageMode)
            {
                case CommonPageMode.ImagesFiles:
                    LoadFileList();
                    break;
                case CommonPageMode.OwnerSettings:
                    LoadOwnerSettings();
                    break;
                case CommonPageMode.HostConfigs:
                    LoadOwnerHostConfigs();
                    break;
            }
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            this.LoadPageMode(navLinkId, targetId);
        }

        private string SaveCommonFile(FileUpload fileUploader)
        {
            FileInfo fileInfo = new FileInfo(fileUploader.PostedFile.FileName);

            string fileName = Util.CleanFileName(fileInfo.Name);
            fileUploader.SaveAs(WebUtil.OwnerSharedFilePath(CurrentUser.CurrentOwner, fileName));
            return fileName;
        }

        private void LoadFileList()
        {
            plcFileList.Visible = true;
            DirectoryInfo dirInfo = new DirectoryInfo(WebUtil.OwnerSharedFileDirectory(CurrentUser.CurrentOwner));
            grdvFileList.DataSource = dirInfo.GetFiles();
            grdvFileList.DataBind();
            grdvFileList.Visible = true;
        }

        private void LoadOwnerSettings()
        {
            plcOwnerConfig.Visible = true;

            txtClassifications.Text = string.Empty;
            txtContactInfoHtml.Text = string.Empty;
            txtShowListingInfoText.Text = string.Empty;
            txtWelcomeMessageText.Text = string.Empty;
            txtBoothTypeList.Text = string.Empty;

            Owner existingOwner = OwnerAdminMgr.GetOwner(CurrentUser.CurrentOwner.OwnerId);
            if (existingOwner != null)
            {
                lblCompanyName.Text = existingOwner.OwnerName;
                txtOwnerLogoFileName.Text = existingOwner.LogoFileName;
                txtOwnerStyleSheet.Text = existingOwner.StyleSheetFileName;
                //txtOwnerSmtpConfigXml.Text = existingOwner.SmtpConfigXmlDecrypted;
                txtOwnerShoppingCartImageFull.Text = existingOwner.ShoppingCartFullImage;
                txtOwnerShoppingCartImageEmpty.Text = existingOwner.ShoppingCartEmptyImage;
                txtClassifications.Text = Util.ParseListToCarriageReturns(existingOwner.ClassificationList, ';');
                txtAdditionalImageLinkText.Text = existingOwner.AdditionalImageLinkText;

                txtBoothTypeList.Text = Util.ParseListToCarriageReturns(existingOwner.BoothTypeList, ';');

                txtWelcomeMessageText.Text = existingOwner.WelcomeMessageText;
                txtShowListingInfoText.Text = existingOwner.ShowListingInfoText;
                txtContactInfoHtml.Text = existingOwner.ContactInfoHtml;
            }
        }


        private void LoadOwnerHostConfigs()
        {
            plcOwnerHostConfigs.Visible = true;

            grdvwOwnerHostConfigs.DataSource = CommonConfig.OwnerHostConfigEntries.Where(h => h.OwnerId == CurrentUser.CurrentOwner.OwnerId);
            grdvwOwnerHostConfigs.DataBind();
        }

        protected void grdvFileList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                FileInfo fileItem = (FileInfo)e.Row.DataItem;
                HtmlAnchor lnkViewFile = (HtmlAnchor)e.Row.FindControl("lnkViewFile");
                TextBox txtCopyLink = (TextBox)e.Row.FindControl("txtCopyLink");
                Image imgFileImage = (Image)e.Row.FindControl("imgFileImage");
                Label lblFileType = (Label)e.Row.FindControl("lblFileType");
                Label lblFileSize = (Label)e.Row.FindControl("lblFileSize");

                LinkButton lbtnDeleteFile = (LinkButton)e.Row.FindControl("lbtnDeleteFile");

                if (lbtnDeleteFile != null)
                {
                    lbtnDeleteFile.Attributes.Add("onClick", "return confirm('Sure you want to delete this file?');");
                }
                FileInfo fi = new FileInfo(WebUtil.OwnerSharedFilePath(CurrentUser.CurrentOwner, fileItem.Name));

                lblFileSize.Text = Util.ToByteString(fi.Length);
                lblFileType.Text = fi.Extension;

                switch (fileItem.Extension.ToLower())
                {
                    case ".gif":
                    case ".jpg":
                    case ".png":
                    case ".bmp":
                        imgFileImage.Visible = true;
                        imgFileImage.ImageUrl = WebUtil.OwnerRelativeSharedFilePath(CurrentUser.CurrentOwner, fileItem.Name);
                        imgFileImage.AlternateText = fileItem.Name;
                        break;
                    default:
                        imgFileImage.Visible = false;
                        break;
                }

                lnkViewFile.HRef = WebUtil.OwnerRelativeSharedFilePath(CurrentUser.CurrentOwner, fileItem.Name);

                string fullUrl = string.Format("http://{0}{1}", Request.Url.Host, VirtualPathUtility.ToAbsolute(lnkViewFile.HRef));
                //lnkCopyUrl.Attributes.Add("onClick", string.Format("copyLink('{0}', '{1}'); return false;", fullUrl, fileItem.Name));

                txtCopyLink.Text = string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", Server.UrlPathEncode(fullUrl), fileItem.Name);
            }
        }


        protected void btnUploadFile_Click(object sender, EventArgs e)
        {
            if (fupUploadFile.HasFile)
            {
                SaveCommonFile(fupUploadFile);
                LoadFileList();
                Master.DisplayFriendlyMessage("File uploaded.");
            }
        }

        protected void lnkbtnRefreshFileList_Click(object sender, EventArgs e)
        {
            LoadFileList();
        }

        protected void grdvFileList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string fileName = e.CommandArgument.ToString();
            switch (e.CommandName)
            {
                case "DeleteFile":
                    DeleteFile(fileName);
                    break;
            }
        }

        private void DeleteFile(string fileName)
        {
            string filePath = WebUtil.OwnerSharedFilePath(CurrentUser.CurrentOwner, fileName);

            File.Delete(filePath);
            LoadFileList();
        }

        protected void btnSaveOwnerConfig_Click(object sender, EventArgs e)
        {
            Owner ownerToSave = new Owner();
            ownerToSave.OwnerId = CurrentUser.CurrentOwner.OwnerId;
            ownerToSave.StyleSheetFileName = txtOwnerStyleSheet.Text.Trim();
            ownerToSave.LogoFileName = txtOwnerLogoFileName.Text.Trim();
            //ownerToSave.SetEncryptedSmtpConfigXml(txtOwnerSmtpConfigXml.Text.Trim());
            ownerToSave.ShoppingCartEmptyImage = txtOwnerShoppingCartImageEmpty.Text.Trim();
            ownerToSave.ShoppingCartFullImage = txtOwnerShoppingCartImageFull.Text.Trim();
            ownerToSave.ClassificationList = txtClassifications.Text.Trim().Replace(Environment.NewLine, ";");
            ownerToSave.AdditionalImageLinkText = txtAdditionalImageLinkText.Text.Trim();

            ownerToSave.BoothTypeList = txtBoothTypeList.Text.Trim().Replace(Environment.NewLine, ";");

            ownerToSave.ContactInfoHtml = txtContactInfoHtml.Text.Trim();
            ownerToSave.ShowListingInfoText = txtShowListingInfoText.Text.Trim();
            ownerToSave.WelcomeMessageText = txtWelcomeMessageText.Text.Trim();

            ValidationResults errors = this.OwnerAdminMgr.SaveOwnerConfig(CurrentUser, ownerToSave);

            if (errors.IsValid)
            {
                //Refresh all database settings.
                this.CurrentUser.CurrentOwner = this.OwnerAdminMgr.GetOwner(CurrentUser.CurrentOwner.OwnerId);
                this.Master.DisplayFriendlyMessage("Saved.");
            }
            else
            {
                PageErrors.AddErrorMessages(errors);
            }

            

        }

        protected void btnCancelSaveOwnerConfig_Click(object sender, EventArgs e)
        {
            LoadPageMode(1, CommonPageMode.ImagesFiles);
        }

        protected void btnSaveOwnerHostConfigs_Click(object sender, EventArgs e)
        {

            List<OwnerHostConfig> ownerHostConfigs = new List<OwnerHostConfig>();

            foreach (GridViewRow row in grdvwOwnerHostConfigs.Rows)
            {
                OwnerHostConfig ownerHostConfig = new OwnerHostConfig();
                ownerHostConfig.OwnerId = CurrentUser.CurrentOwner.OwnerId;
                ownerHostConfig.UrlHost = ((Label)row.FindControl("lblHostAddress")).Text.Trim();
                ownerHostConfig.CssFileName = ((TextBox)row.FindControl("txtOwnerHostCssFileName")).Text.Trim();
                ownerHostConfig.ContactEmail = ((TextBox)row.FindControl("txtOwnerHostContactEmail")).Text.Trim();

                ownerHostConfigs.Add(ownerHostConfig);
            }

            ValidationResults errors = this.OwnerAdminMgr.SaveOwnerHostConfigs(CurrentUser, ownerHostConfigs);

            if (errors.IsValid)
            {
                this.OwnerAdminMgr.RefreshOwnerHostConfigs();
                this.Master.DisplayFriendlyMessage("Saved.");
            }
            else
            {
                this.PageErrors.AddErrorMessages(errors);
            }

        }

        protected void btnCancelSaveOwnerHostConfigs_Click(object sender, EventArgs e)
        {
            LoadPageMode(1, CommonPageMode.ImagesFiles);
        }

        protected void grdvwOwnerHostConfigs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
        }

        protected void grdvwOwnerHostConfigs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }


    }
}