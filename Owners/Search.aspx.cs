using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Common;
using ExpoOrders.Entities;
using ExpoOrders.Controllers;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;

namespace ExpoOrders.Web.Owners
{
    public partial class Search : BaseOwnerPage
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
            this.Master.NavigationItemCallBack = this.HandleNavigationItemClicked;
            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            this.Master.NoSubNavigation = true;
            this.Master.LoadMasterPage(this.CurrentUser, OwnerPage.Search);
            InitializeSearchScreen();
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            InitializeSearchScreen();
        }

        private void InitializeSearchScreen()
        {
            plcSearchCriteria.Visible = true;
            plcSearchResults.Visible = false;
            rptrSearchList.Visible = false;

            txtSearchExhibitorCompanyName.Text = string.Empty;
            txtSearchExhibitorId.Text = string.Empty;
            txtSearchOrderNumber.Text = string.Empty;

            txtSearchBoothNumber.Text = string.Empty;
            txtSearchEmailAddress.Text = string.Empty;
            txtSearchTransactionId.Text = string.Empty;

            txtLastDigitsCreditCard.Text = string.Empty;
            txtNameOnCreditCard.Text = string.Empty;

            rdoLstCreditCardOnFile.ClearSelection();
            rdoLstCreditCardOnFile.SelectedValue = "0";

            rdoLstIssuedCredit.ClearSelection();
            rdoLstIssuedCredit.SelectedValue = "0";

            rdoLstDeclinedCreditCard.ClearSelection();
            rdoLstDeclinedCreditCard.SelectedValue = "0";

            chkSearchIncludeInactive.Checked = false;

            txtSearchUserName.Text = string.Empty;
            txtSearchContactName.Text = string.Empty;

            cboSearchShowId.Items.Clear();
            cboSearchShowId.ClearSelection();

            cboSearchShowId.DataSource = this.OwnerAdminMgr.GetAllShows(CurrentUser, true).Where(s => s.ActiveFlag == true).OrderByDescending(s => s.StartDate).ToList();
            cboSearchShowId.DataBind();

            cboSearchShowId.Items.Insert(0, new RadComboBoxItem { Text = " ", Value = "-1" });

            cboExhibitorClassification.Items.Clear();
            cboExhibitorClassification.ClearSelection();

            if (!string.IsNullOrEmpty(CurrentUser.CurrentOwner.ClassificationList))
            {
                plcExhibitorClassificationSearch.Visible = true;
                foreach (string item in Util.ParseDelimitedList(CurrentUser.CurrentOwner.ClassificationList, ';'))
                {
                    var itemVal = item.Trim();
                    if (!string.IsNullOrEmpty(itemVal))
                    {
                        cboExhibitorClassification.Items.Insert(0, new RadComboBoxItem { Text = itemVal, Value = itemVal });
                    }
                }

                cboExhibitorClassification.Items.Insert(0, new RadComboBoxItem { Text = " ", Value = "" });
            }
        }

        private void LoadSearchResults()
        {
            plcSearchResults.Visible = true;

            SearchCriteria searchCriteria = BuildSearchCriteria();

            List < OwnerSearchResult > results = this.OwnerAdminMgr.SearchAllShows(CurrentUser, searchCriteria);

            if (results != null && results.Count > 0)
            {
                lblNoSearchResults.Visible = false;
                
                rptrSearchList.DataSource = results;
                rptrSearchList.DataBind();
                rptrSearchList.Visible = true;
            }
            else
            {
                lblNoSearchResults.Visible = true;
                rptrSearchList.Visible = false;
            }
        }

        private SearchCriteria BuildSearchCriteria()
        {
            SearchCriteria searchCriteria = new SearchCriteria();
            
            if (cboSearchShowId.SelectedIndex > 0)
            {
                searchCriteria.ShowId = Util.ConvertInt32(cboSearchShowId.SelectedValue);
            }

            searchCriteria.ExhibitorId = Util.ConvertInt32(txtSearchExhibitorId.Text.Trim());
            searchCriteria.OrderId = Util.ConvertInt32(txtSearchOrderNumber.Text.Trim());
            searchCriteria.BoothNumber = txtSearchBoothNumber.Text.Trim();

            searchCriteria.NameOnCreditCard = txtNameOnCreditCard.Text.Trim();
            searchCriteria.LastDigitsCreditCard = txtLastDigitsCreditCard.Text.Trim();
            
            searchCriteria.PrimaryEmailAddress = txtSearchEmailAddress.Text.Trim();

            searchCriteria.TransactionId = txtSearchTransactionId.Text.Trim();

            switch (rdoLstCreditCardOnFile.SelectedValue)
            {
                case "0":
                    searchCriteria.CreditCardOnFile = null;
                    break;
                case "1":
                    searchCriteria.CreditCardOnFile = true;
                    break;
                case "2":
                    searchCriteria.CreditCardOnFile = false;
                    break;
                default:
                    break;
            }

            switch (rdoLstIssuedCredit.SelectedValue)
            {
                case "0":
                    searchCriteria.IssuedCredit = null;
                    break;
                case "1":
                    searchCriteria.IssuedCredit = true;
                    break;
                case "2":
                    searchCriteria.IssuedCredit = false;
                    break;
                default:
                    break;
            }

            switch (rdoLstDeclinedCreditCard.SelectedValue)
            {
                case "0":
                    searchCriteria.DeclinedCreditCard = null;
                    break;
                case "1":
                    searchCriteria.DeclinedCreditCard = true;
                    break;
                case "2":
                    searchCriteria.DeclinedCreditCard = false;
                    break;
                default:
                    break;
            }

            searchCriteria.IncludeInactive = chkSearchIncludeInactive.Checked;
            searchCriteria.CompanyName = txtSearchExhibitorCompanyName.Text.Trim();
            searchCriteria.ContactName = txtSearchContactName.Text.Trim();
            searchCriteria.UserName = txtSearchUserName.Text.Trim();

            if (plcExhibitorClassificationSearch.Visible)
            {
                if (!string.IsNullOrEmpty(cboExhibitorClassification.Text))
                {
                    searchCriteria.ExhibitorClassification = cboExhibitorClassification.Text;
                }
            }

            return searchCriteria;
        }

        #region Control Events

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadSearchResults();
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            InitializeSearchScreen();
        }

        private int _currentSearchShowId = 0;
        protected void rptrSearchList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                OwnerSearchResult searchItem = (OwnerSearchResult)e.Item.DataItem;
                HtmlTableRow trShowName = (HtmlTableRow)e.Item.FindControl("trShowName");
                HtmlTableRow trColumnHeaders = (HtmlTableRow)e.Item.FindControl("trColumnHeaders");

                
                LinkButton lbtnGoToExhibitor = (LinkButton)e.Item.FindControl("lbtnGoToExhibitor");
                LinkButton lbtnCallLog = (LinkButton)e.Item.FindControl("lbtnCallLog");
                LinkButton lbtnEmailLog = (LinkButton)e.Item.FindControl("lbtnEmailLog");
                

                Literal ltrContactName = (Literal)e.Item.FindControl("ltrContactName");
                HtmlAnchor lnkMailToExhibitor = (HtmlAnchor)e.Item.FindControl("lnkMailToExhibitor");


                if (searchItem.ExhibitorShow.ShowId != _currentSearchShowId)
                {
                    _currentSearchShowId = searchItem.ExhibitorShow.ShowId;
                    trShowName.Visible = trColumnHeaders.Visible = true;
                }
                else
                {
                    trShowName.Visible = trColumnHeaders.Visible = false;
                }

                lbtnGoToExhibitor.CommandArgument 
                    = lbtnCallLog.CommandArgument
                    = lbtnEmailLog.CommandArgument
                    = string.Format("{0}:{1}", searchItem.ExhibitorShow.ShowId, searchItem.ExhibitorDetail.ExhibitorId);


                if (!string.IsNullOrEmpty(searchItem.ExhibitorUser.Email))
                {
                    lnkMailToExhibitor.HRef = string.Format("mailto:{0}", searchItem.ExhibitorUser.Email);
                    lnkMailToExhibitor.InnerText = searchItem.ExhibitorUser.Email;
                    lnkMailToExhibitor.Visible = true;
                }
                else
                {
                    lnkMailToExhibitor.Visible = false;
                }

                ltrContactName.Text = string.Format("{0} {1}", searchItem.ExhibitorUser.FirstName, searchItem.ExhibitorUser.LastName);

            }
        }

        protected void rptrSearchList_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            int showId = Util.ConvertInt32(e.CommandArgument.ToString().Split(':')[0].Trim());
            int exhibitorId = Util.ConvertInt32(e.CommandArgument.ToString().Split(':')[1].Trim());

            switch (e.CommandName)
            {
                case "GoToExhibitor":
                    LinkToExhibitorId = exhibitorId;
                    TransferToShowPage(showId, OwnerPage.Exhibitors);
                    break;
                case "ViewCallLog":
                    if (SetShowContext(showId))
                    {
                        LaunchCallLogViewer(null, exhibitorId);
                    }
                    break;
                case "ViewEmailLog":
                    if (SetShowContext(showId))
                    {
                        LaunchEmailLogViewer(null, exhibitorId);
                    }
                    break;
                default:
                    break;
            }
            
        }

        #endregion
    }
}