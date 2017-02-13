using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Controllers;
using ExpoOrders.Common;
using ExpoOrders.Entities;
using System.Web.UI.HtmlControls;

namespace ExpoOrders.Web.Owners
{
    public partial class Stats : BaseOwnerPage
    {
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
            this.Master.NoSubNavigation = true;
            this.Master.NavigationItemCallBack = this.HandleNavigationItemClicked;
            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            this.Master.LoadMasterPage(this.CurrentUser, OwnerPage.Stats);
            InitializeScreen();
        }

         private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            InitializeScreen();
        }

         private void InitializeScreen()
         {

         }

         protected void btnDisplayStats_Click(object sender, EventArgs e)
         {
             LoadShowStats();
         }

         private void LoadShowStats()
         {
             plcShowStats.Visible = true;
             SearchCriteria searchCriteria = new SearchCriteria();
             searchCriteria.IncludeInactive = chkShowInActive.Checked;

             lblNoSearchResults.Visible = false;
             rptrShowStats.Visible = false;

             List<ShowStatistics> showStats = OwnerAdminMgr.SearchShowStats(CurrentUser, searchCriteria).OrderByDescending(s => s.ShowStartDate).ToList();

             if (showStats != null && showStats.Count > 0)
             {
                 rptrShowStats.DataSource = showStats;
                 rptrShowStats.DataBind();
                 rptrShowStats.Visible = true;
             }
             else
             {
                 lblNoSearchResults.Visible = true;
             }
             
         }

         protected void rptrShowStats_ItemCommand(object sender, RepeaterCommandEventArgs e)
         {
             int showId = Util.ConvertInt32(e.CommandArgument);
             switch (e.CommandName)
             {
                 case "GoToShow":
                     TransferToShowPage(showId, OwnerPage.Reports);
                     break;
             }
         }


         //protected void rptrShowStats_ItemDataBound(object sender, RepeaterItemEventArgs e)
         //{
         //    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
         //    {
         //        Repeater rptrShowExhibitor = (Repeater)e.Item.FindControl("rptrShowExhibitor");

         //        ShowStatistics showStat = (ShowStatistics) e.Item.DataItem;

         //        LoadExhibitorList(showStat.ShowId, rptrShowExhibitor);
         //    }
         //}

         private void LoadExhibitorList(int showId, Repeater rptrShowExhibitor)
         {
             SearchCriteria searchCriteria = new SearchCriteria();
             searchCriteria.ShowId = showId;
             searchCriteria.ExhibitorHasBalance = true;

             List<Exhibitor> exhibitors = OwnerAdminMgr.SearchExhibitors(CurrentUser, showId, searchCriteria);

             rptrShowExhibitor.DataSource = exhibitors;
             rptrShowExhibitor.DataBind();
             rptrShowExhibitor.Visible = true;
         }

         private int _currentSearchShowId = 0;

         protected void rptrShowExhibitor_ItemDataBound(object sender, RepeaterItemEventArgs e)
         {
             if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
             {

                 HtmlTableRow trColumnHeaders = (HtmlTableRow)e.Item.FindControl("trColumnHeaders");
                 LinkButton lbtnGoToExhibitor = (LinkButton)e.Item.FindControl("lbtnGoToExhibitor");
                 HtmlAnchor lnkPrintInvoice = (HtmlAnchor)e.Item.FindControl("lnkPrintInvoice");
                 HtmlAnchor lnkMailToExhibitor = (HtmlAnchor)e.Item.FindControl("lnkMailToExhibitor");
                 Repeater rptrShowExhibitor = (Repeater)e.Item.FindControl("rptrShowExhibitor");

                 Exhibitor exhibitor = (Exhibitor)e.Item.DataItem;


                 if (exhibitor.ShowId != _currentSearchShowId)
                 {
                     _currentSearchShowId = exhibitor.ShowId;
                     trColumnHeaders.Visible = true;
                 }
                 else
                 {
                     trColumnHeaders.Visible = false;
                 }

                 lnkPrintInvoice.Attributes.Add("onclick", string.Format("launchExhibitorInvoice({0}, {1}, {2}); return false;", (int)ReportEnum.ExhibitorInvoice, exhibitor.ShowId, exhibitor.ExhibitorId));

                 lbtnGoToExhibitor.CommandArgument = string.Format("{0}:{1}", exhibitor.ShowId, exhibitor.ExhibitorId);
                 if (!string.IsNullOrEmpty(exhibitor.PrimaryEmailAddress))
                 {
                     lnkMailToExhibitor.HRef = string.Format("mailto:{0}", exhibitor.PrimaryEmailAddress);
                     lnkMailToExhibitor.InnerText = exhibitor.PrimaryEmailAddress;
                     lnkMailToExhibitor.Visible = true;
                 }
                 else
                 {
                     lnkMailToExhibitor.Visible = false;
                 }

             }
         }

         protected void rptrShowExhibitor_ItemCommand(object sender, RepeaterCommandEventArgs e)
         {
             int showId = Util.ConvertInt32(e.CommandArgument.ToString().Split(':')[0].Trim());
             int exhibitorId = Util.ConvertInt32(e.CommandArgument.ToString().Split(':')[1].Trim());

             switch (e.CommandName)
             {
                 case "GoToExhibitor":
                     LinkToExhibitorId = exhibitorId;
                     TransferToShowPage(showId, OwnerPage.Exhibitors);
                     break;
             }
         }
        
    }
}