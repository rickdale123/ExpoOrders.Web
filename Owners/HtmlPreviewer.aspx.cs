using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Entities;
using ExpoOrders.Controllers;

namespace ExpoOrders.Web.Owners
{
    public partial class HtmlPreviewer : BaseOwnerPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            LoadStyleSheet(CurrentUser.CurrentShow);

            if (CurrentUser.EmailPreview != null)
            {
                HtmlContentController cntrl = new HtmlContentController();
                Email emailContent = cntrl.BuildEmailPreview(CurrentUser, CurrentUser.EmailPreview);
                if (emailContent != null)
                {
                    lblEmailTo.Text = emailContent.ToAddress;
                    lblEmailFrom.Text = emailContent.FromAddress;
                    lblEmailReplyTo.Text = emailContent.ReplyAddress;
                    lblEmailSubject.Text = emailContent.Subject;

                    this.htmlContentEditor.Content = emailContent.Body;
                }
                
            }
        }


        public void LoadStyleSheet(Entities.Show currentShow)
        {
            this.OwnerStyleSheet.Href = string.Format("~/Assets/Shows/{0}/style.css", currentShow.ShowGuid);
        }

        private void WriteContents()
        {
            Response.Clear();
            Response.Write("<html><body>Hello <b>World</b>.</body></html>");
        }
    }
}