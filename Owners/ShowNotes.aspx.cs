using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Common;
using ExpoOrders.Controllers;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using ExpoOrders.Entities;
using System.IO;
using System.Web.UI.HtmlControls;

namespace ExpoOrders.Web.Owners
{
    public partial class ShowNotes : BaseOwnerPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadShowNoteAttachments(CurrentUser.CurrentShow.ShowId);

                if (Request.QueryString["showId"] != null)
                {
                    this.OwnerStyleSheet.Href = string.Format("~/Assets/Shows/{0}/style.css", CurrentUser.CurrentShow.ShowGuid);
                    LoadOwnerNotes(Util.ConvertInt32(Request.QueryString["showId"]));
                }
            }
        }

        private void LoadOwnerNotes(int showId)
        {
            ltrShowName.Text = string.Empty;
            lblOwnerNotes.Text = string.Empty;
            lnkClearOwnerNotes.Visible = false;

            OwnerAdminController ctrl = new OwnerAdminController();
            Show show = ctrl.GetShowById(showId);

            if (show != null)
            {
                ltrShowName.Text = show.ShowName;
                hdnShowId.Value = show.ShowId.ToString();
                if (!string.IsNullOrEmpty(show.OwnerNotes))
                {
                    lblOwnerNotes.Text = show.OwnerNotes.Replace(Environment.NewLine, "<br/>");

                    lnkClearOwnerNotes.Visible = true;
                    lnkClearOwnerNotes.Attributes.Add("Onclick", "return confirm('Sure you want to remove ALL notes for this show?');");
                }
                
            }
        }

        protected void btnSaveNotes_Click(object sender, EventArgs e)
        {
            OwnerAdminController ownerCntrl = new OwnerAdminController();

            int showId = Util.ConvertInt32(hdnShowId.Value);

            ValidationResults errors = ownerCntrl.SaveShowNote(CurrentUser, Util.ConvertInt32(hdnShowId.Value), txtOwnerNotes.Text.Trim());
            if (showId > 0)
            {
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "closeWindow", "<script language='javascript'>saveNoteCallBack(" + showId.ToString() + ");</script>");
            }
            else
            {
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "closeWindow", "<script language='javascript'>closeMe();</script>");
            }
        }

        protected void btnClearOwnerNotes_Click(object sender, EventArgs e)
        {
            OwnerAdminController ownerCntrl = new OwnerAdminController();

            int showId = Util.ConvertInt32(hdnShowId.Value);

            ownerCntrl.RemoveOwnerNotes(CurrentUser, showId);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "closeWindow", "<script language='javascript'>closeMe();</script>");
        }

        private void LoadShowNoteAttachments(int showId)
        {
            grdvFileList.DataSource = null;
            grdvFileList.DataBind();

            if (showId > 0)
            {
                plcShowNoteFiles.Visible = true;
                string storagePath = string.Format("{0}{1}/Notes", Server.MapPath("~/Assets/Shows/"), CurrentUser.CurrentShow.ShowGuid.ToString());
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

        private string SaveShowFile(FileUpload fileUploader)
        {
            string storagePath = string.Format("{0}{1}/Notes", Server.MapPath("~/Assets/Shows/"), this.CurrentUser.CurrentShow.ShowGuid.ToString());
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }
            
            FileInfo fileInfo = new FileInfo(fileUploader.PostedFile.FileName);
            string fileName = Util.CleanFileName(fileInfo.Name);
            fileUploader.SaveAs(storagePath + "\\" + fileName);
            return fileName;
        }

        private void DeleteFileName(string fileName)
        {
            string filePath = Server.MapPath(string.Format("~/Assets/Shows/{0}/Notes/{1}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), fileName));

            File.Delete(filePath);
        }

        protected void lnkbtnRefreshFileList_Click(object sender, EventArgs e)
        {
            LoadShowNoteAttachments(CurrentUser.CurrentShow.ShowId);
        }


        protected void btnUploadFile_Click(object sender, EventArgs e)
        {
            if (fupUploadFile.HasFile)
            {
                SaveShowFile(fupUploadFile);
                LoadShowNoteAttachments(CurrentUser.CurrentShow.ShowId);
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

                FileInfo fi = new FileInfo(Server.MapPath(string.Format("~/Assets/Shows/{0}/Notes/{1}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), fileItem.Name)));

                lblFileSize.Text = Util.ToByteString(fi.Length);
                lblFileType.Text = fi.Extension;

                switch (fileItem.Extension.ToLower())
                {
                    case ".gif":
                    case ".jpg":
                    case ".png":
                    case ".bmp":
                        imgFileImage.Visible = true;
                        imgFileImage.ImageUrl = string.Format("~/Assets/Shows/{0}/Notes/{1}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), fileItem.Name);
                        imgFileImage.AlternateText = fileItem.Name;
                        break;
                    default:
                        imgFileImage.Visible = false;
                        break;
                }


                lnkViewFile.HRef = string.Format("~/Assets/Shows/{0}/Notes/{1}", this.CurrentUser.CurrentShow.ShowGuid.ToString(), fileItem.Name);
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

            LoadShowNoteAttachments(CurrentUser.CurrentShow.ShowId);
        }

    }
}