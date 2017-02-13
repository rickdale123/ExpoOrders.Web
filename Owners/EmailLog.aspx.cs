using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using System.Web.UI.HtmlControls;
using System.IO;
using Telerik.Web.UI;

namespace ExpoOrders.Web.Owners
{
    public partial class EmailLog : BaseOwnerPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        private void InitializePage()
        {
            rptrEmailLog.Visible = false;
            string userId = QueryStringValue("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                LoadEmailLog(userId, 0);
            }
            else
            {
                int exhibitorId = Util.ConvertInt32(QueryStringValue("ExhibitorId"));
                if (exhibitorId > 0)
                {
                    LoadEmailLog(null, exhibitorId);
                }
            }
           
            this.OwnerStyleSheet.Href = string.Format("~/Assets/Shows/{0}/style.css", CurrentUser.CurrentShow.ShowGuid);
        }

        private void LoadEmailLog(string userId, int exhibitorId)
        {
            lblNoEmails.Visible = false;

            EmailController cntrl = new EmailController();
            SearchCriteria search = new SearchCriteria();
            search.UserId = userId;
            search.ExhibitorId = exhibitorId;

            List<Email> emails = cntrl.SearchEmailLog(CurrentUser, search).OrderByDescending(e => e.SendDate).ToList();

            if (emails != null && emails.Count > 0)
            {
                rptrEmailLog.Visible = true;
                rptrEmailLog.DataSource = emails;
                rptrEmailLog.DataBind();
            }
            else
            {
                lblNoEmails.Visible = true;
            }

            emails = null;

        }

        protected void rptrEmailLog_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Email em = (Email) e.Item.DataItem;
                PlaceHolder plcAttachments = (PlaceHolder)e.Item.FindControl("plcAttachments");

                HtmlTableRow trProcessingDetails = (HtmlTableRow)e.Item.FindControl("trProcessingDetails");
                HtmlTableCell tdEmailStatus = (HtmlTableCell)e.Item.FindControl("tdEmailStatus");

                RadEditor htmlContentEditor = (RadEditor)e.Item.FindControl("htmlContentEditor");

                htmlContentEditor.Content = em.Body;
                plcAttachments.Visible = false;

                if (em.StatusCode == "E")
                {
                    tdEmailStatus.Attributes["class"] = "errorMessage";
                }

                trProcessingDetails.Visible = !string.IsNullOrEmpty(em.ProcessingDetails);

                if (!string.IsNullOrEmpty(em.Attachments))
                {
                    List<string> attachmentsToSend = em.Attachments.Split(new char[] { ';' }).ToList();

                    foreach (string attachment in attachmentsToSend)
                    {
                        FileInfo attachmentFileInfo = new FileInfo(attachment);

                        HtmlAnchor lnkFileAttachment = new HtmlAnchor();

                        lnkFileAttachment.Target = "_blank";
                        lnkFileAttachment.InnerText = attachmentFileInfo.Name;
                        lnkFileAttachment.HRef = string.Format("~/Assets/Shows/{0}/Attachments/{1}", CurrentUser.CurrentShow.ShowGuid, attachmentFileInfo.Name);
                        plcAttachments.Controls.Add(lnkFileAttachment);
                        plcAttachments.Controls.Add(new LiteralControl("<br/>"));
                        plcAttachments.Visible = true;
                    }
                }
            }
        }

    }
}