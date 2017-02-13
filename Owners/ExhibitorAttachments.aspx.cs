using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Common;
using System.IO;
using System.Web.UI.HtmlControls;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;

namespace ExpoOrders.Web.Owners
{
    public partial class ExhibitorAttachments : BaseOwnerPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["exhibitorId"] != null)
            {
                this.OwnerStyleSheet.Href = string.Format("~/Assets/Shows/{0}/style.css", CurrentUser.CurrentShow.ShowGuid);
                LoadExhibitorAttachments(Util.ConvertInt32(Request.QueryString["exhibitorId"]));
            }
        }

        private void LoadExhibitorAttachments(int exhibitorId)
        {
            grdvFileList.DataSource = null;
            grdvFileList.DataBind();

            ltrExhibitorName.Text = string.Empty;
            CurrentExhibitorId = 0;
            
            OwnerAdminController cntrl = new OwnerAdminController();

            Exhibitor exhibitor = cntrl.GetExhibitorById(exhibitorId);

            if (exhibitor != null && exhibitor.ExhibitorId > 0)
            {
                ltrExhibitorName.Text = exhibitor.ExhibitorCompanyNameDisplay;
                CurrentExhibitorId = exhibitorId;

                plcExhibitorAttachments.Visible = true;
                string storagePath = string.Format("{0}{1}/Exhibitor/{2}", Server.MapPath("~/Assets/Shows/"), CurrentUser.CurrentShow.ShowGuid.ToString(), CurrentExhibitorId);
                DirectoryInfo dirInfo = new DirectoryInfo(storagePath);
                if (dirInfo.Exists)
                {
                    FileInfo[] files = dirInfo.GetFiles();
                    if (files.Length > 0)
                    {
                        grdvFileList.DataSource = files;
                        grdvFileList.DataBind();
                        grdvFileList.Visible = true;
                    }
                }
            }
        }

        private string SaveAttachment(FileUpload fileUploader)
        {
            string storagePath = string.Format("{0}{1}/Exhibitor/{2}", Server.MapPath("~/Assets/Shows/"), this.CurrentUser.CurrentShow.ShowGuid.ToString(), CurrentExhibitorId);
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }
            
            FileInfo fileInfo = new FileInfo(fileUploader.PostedFile.FileName);
            string savedFileName = Util.CleanFileName(fileInfo.Name);
            fileUploader.SaveAs(storagePath + "\\" + savedFileName);
            return savedFileName;
        }

        private void DeleteFileName(string fileName)
        {
            string filePath = Server.MapPath(string.Format("~/Assets/Shows/{0}/Exhibitor/{1}/{2}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), CurrentExhibitorId, fileName));

            File.Delete(filePath);
        }

        protected void lnkbtnRefreshFileList_Click(object sender, EventArgs e)
        {
            LoadExhibitorAttachments(CurrentExhibitorId);
        }


        protected void btnUploadFile_Click(object sender, EventArgs e)
        {
            if (fupUploadFile.HasFile)
            {
                SaveAttachment(fupUploadFile);
                LoadExhibitorAttachments(CurrentExhibitorId);
            }
        }

        protected void grdvFileList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                FileInfo fileItem = (FileInfo)e.Row.DataItem;
                HtmlAnchor lnkViewFile = (HtmlAnchor)e.Row.FindControl("lnkViewFile");
                Image imgFileImage = (Image)e.Row.FindControl("imgFileImage");
                Label lblFileType = (Label)e.Row.FindControl("lblFileType");
                Label lblFileSize = (Label)e.Row.FindControl("lblFileSize");

                LinkButton lbtnDeleteFile = (LinkButton)e.Row.FindControl("lbtnDeleteFile");

                if (lbtnDeleteFile != null)
                {
                    lbtnDeleteFile.Attributes.Add("onClick", "return confirm('Sure you want to delete this file?');");
                }

                FileInfo fi = new FileInfo(Server.MapPath(string.Format("~/Assets/Shows/{0}/Exhibitor/{1}/{2}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), CurrentExhibitorId, fileItem.Name)));

                lblFileSize.Text = Util.ToByteString(fi.Length);
                lblFileType.Text = fi.Extension;

                switch (fileItem.Extension.ToLower())
                {
                    case ".gif":
                    case ".jpg":
                    case ".png":
                    case ".bmp":
                        imgFileImage.Visible = true;
                        imgFileImage.ImageUrl = string.Format("~/Assets/Shows/{0}/Exhibitor/{1}/{2}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), CurrentExhibitorId, fileItem.Name);
                        imgFileImage.AlternateText = fileItem.Name;
                        break;
                    default:
                        imgFileImage.Visible = false;
                        break;
                }


                lnkViewFile.HRef = string.Format("~/Assets/Shows/{0}/Exhibitor/{1}/{2}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), CurrentExhibitorId, fileItem.Name);
            }
        }

        protected void grdvFileList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string fileName = e.CommandArgument.ToString();
            switch (e.CommandName)
            {
                case "DeleteFile":
                    DeleteFileName(fileName);
                    break;
            }

            LoadExhibitorAttachments(CurrentExhibitorId);
        }


        private int CurrentExhibitorId
        {
            get
            {
                return Util.ConvertInt32(hdnExhibitorId.Value);
            }
            set
            {
                hdnExhibitorId.Value = value.ToString();
            }
        }

    }
}