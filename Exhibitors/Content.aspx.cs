using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;

namespace ExpoOrders.Web.Exhibitors
{
    public partial class Content : BaseExhibitorPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadPageContent();
            }
        }

        private void LoadPageContent()
        {
            if (CurrentUser.CurrentShow != null)
            {
                this.ShowStyleSheet.Href = string.Format("~/Assets/Shows/{0}/style.css", CurrentUser.CurrentShow.ShowGuid);
            }

            if (Request["contentType"] != null)
            {
                DisplayContentByType(Request["contentType"].ToString());
            }
            else if (Request["title"] != null)
            {
                DisplayContentByTitle(Request["title"].ToString());
            } 
        }

        private void DisplayContentByType(string contentType)
        {
            HtmlContentController cntrl = new HtmlContentController();
            DisplayHtmlContent(cntrl.GetHtmlContentListByShowId(CurrentUser.CurrentShow.ShowId, false).Where(h => h.ContentTypeCd == contentType).FirstOrDefault());
        }

        private void DisplayContentByTitle(string title)
        {
            HtmlContentController cntrl = new HtmlContentController();
            DisplayHtmlContent(cntrl.GetHtmlContentListByShowId(CurrentUser.CurrentShow.ShowId, false).Where(h => h.Title == title).FirstOrDefault());
        }

        private void DisplayHtmlContent(HtmlContent content)
        {
            if (content != null)
            {
                ltrPageContent.Text = content.DynamicHtmlContent;
            }
            else
            {
                ltrPageContent.Text = "Missing Content for display, please contact the event administrator for assistance.";
            }
        }
    }
}