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

namespace ExpoOrders.Web.Owners
{
    public partial class Tab1Editor : BaseTabEditorPage
    {

        #region Manager Objects
        private TabConfigController _tabMgr = null;
        public TabConfigController TabMgr
        {
            get
            {
                if (_tabMgr == null)
                {
                    return new TabConfigController();
                }
                else
                {
                    return _tabMgr;
                }
            }
        }
        #endregion


        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.NavigationItemCallBack = this.HandleNavigationItemClicked;
            this.Master.PreviewShowCallBack = this.PreviewShow;

            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, OwnerPage.Tab1Editor, OwnerTabEnum.ShowDetail);
            LoadSubNavLinks();
        }

        private void LoadSubNavLinks()
        {
            List<NavigationLink> pageSubNavLinks = OwnerUtil.BuildShowAdminNavLinks(CurrentUser, CurrentUser.CurrentShow.ShowId);
            this.Master.LoadSubNavigation("Show Admin", pageSubNavLinks);
            this.LoadPageMode(pageSubNavLinks, ShowSettingPageMode.Tab1Editor);
        }


        private void LoadPageMode(List<NavigationLink> pageSubNavLinks, ShowSettingPageMode mode)
        {
            NavigationLink linkToSelect = pageSubNavLinks.SingleOrDefault<NavigationLink>(l => l.TargetId == (int)mode);

            this.Master.SelectNavigationItem(linkToSelect.NavigationLinkId);
            LoadPageMode(linkToSelect.NavigationLinkId, linkToSelect.TargetId.Value);
        }

        private void LoadPageMode(int navLinkId, int targetId)
        {
            if (navLinkId <= 0)
            {
                navLinkId = 1;
                targetId = (int)ShowSettingPageMode.Tab1Editor;
                this.Master.SelectNavigationItem(navLinkId);
            }

            ShowSettingPageMode currentPageMode = (ShowSettingPageMode)Enum.Parse(typeof(ShowSettingPageMode), targetId.ToString(), true);

            plcTabEditor.Visible =
                plcNavigationEditor.Visible =
                plcManageNavigationLink.Visible = false;

            switch (currentPageMode)
            {
                case ShowSettingPageMode.Tab1Editor:
                    LoadTabInformation();
                    plcTabEditor.Visible = true;
                    plcNavigationEditor.Visible = true;
                    break;
                default:
                    base.HandleNavigationPostBack(currentPageMode);
                    break;
            }

        }

        #endregion

        #region Control Events

        protected void btnSaveTab_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "TabInformation";
            if (Page.IsValid)
            {
                int tabId = Util.ConvertInt32(hdnTabLinkId.Value);
                if (tabId > 0)
                {
                    NavigationLink.NavigationalAction navAction = Enum<NavigationLink.NavigationalAction>.Parse(ddlTabNavigationAction.SelectedValue);
                    int? tabTarget = null;
   
                    if (navAction == NavigationLink.NavigationalAction.HtmlContent)
                    {
                        tabTarget = Util.ConvertInt32(ddlTabTargetHtmlContent.SelectedValue);
                    }
                    else if (navAction == NavigationLink.NavigationalAction.SelectNavigationItem)
                    {
                        tabTarget = Util.ConvertInt32(this.ddlTabTargetNavItem.SelectedValue);
                    }
                    this.TabMgr.SaveTab(tabId, txtTabLinkText.Text.Trim(), navAction, tabTarget, chkTabVisible.Checked, txtTabNumber.Text.Trim());
                    if (Page.IsValid)
                    {
                        this.Master.DisplayFriendlyMessage("Tab configuration saved.");
                        LoadSubNavLinks();
                    }
                }
            }
        }
        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            this.LoadPageMode(navLinkId, targetId);
        }

        protected void ddlAction_Changed(object sender, EventArgs e)
        {
            if (ddlAction.SelectedIndex > 0)
            {
                int actionId = Util.ConvertInt32(ddlAction.SelectedValue);
                LoadTargetAssets(actionId);
            }
        }

        protected void ddlTabNavigationAction_Changed(object sender, EventArgs e)
        {
            TabNavigationActionSelected();
        }

        private void TabNavigationActionSelected()
        {
            if (!string.IsNullOrEmpty(ddlTabNavigationAction.SelectedValue))
            {
                NavigationLink.NavigationalAction tabNavigationAction = Enum<NavigationLink.NavigationalAction>.Parse(ddlTabNavigationAction.SelectedValue, true);

                plcTabTarget.Visible = false;

                LoadTabTargets(tabNavigationAction, ddlTabTargetHtmlContent, ddlTabTargetNavItem, plcTabTarget, lblTabTarget, hdnTabTargetId);
            }
        }
      
        protected void grdvwNavigationLinkList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                NavigationLink link = (NavigationLink)e.Row.DataItem;

                Literal ltrNavigationItemType = (Literal)e.Row.FindControl("ltrNavigationItemType");

                if (link.NavigationItemType != null && link.NavigationItemType.Name != null)
                {
                    ltrNavigationItemType.Text = link.NavigationItemType.Name.Trim();
                }
                
                Literal ltrAction = (Literal)e.Row.FindControl("ltrAction");
                if (link.NavigationAction != null && link.NavigationAction.Name != null)
                {
                    ltrAction.Text = link.NavigationAction.Name.Trim();
                }
                
                Literal ltrTargetId = (Literal)e.Row.FindControl("ltrTargetId");
                ltrTargetId.Text = link.TargetId.ToString();
                Literal ltrSortOrder = (Literal)e.Row.FindControl("ltrSortOrder");

                if (link.SortOrder.HasValue)
                {
                    ltrSortOrder.Text = link.SortOrder.ToString().Trim();
                }
                
                Literal ltrActive = (Literal)e.Row.FindControl("ltrActive");


                LinkButton lbtnActivateNavLink = (LinkButton)e.Row.FindControl("lbtnActivateNavLink");
                LinkButton lbtnDeactivateNavLink = (LinkButton)e.Row.FindControl("lbtnDeactivateNavLink");

                lbtnActivateNavLink.CommandArgument
                    = lbtnDeactivateNavLink.CommandArgument = link.NavigationLinkId.ToString();

                if (link.ActiveFlag)
                {
                    ltrActive.Text = "Yes";
                    lbtnDeactivateNavLink.Visible = true;
                    lbtnActivateNavLink.Visible = false;
                }
                else
                {
                    ltrActive.Text = "No";
                    lbtnActivateNavLink.Visible = true;
                    lbtnDeactivateNavLink.Visible = false;
                }
            }
        }

        protected void grdvwNavigationLinkList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int navLinkId = Util.ConvertInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "EditNavLink":
                    ManageNavLink(navLinkId);
                    break;
                case "DeactivateNavLink":
                    DeactivateNavLink(navLinkId);
                    break;
                case "ActivateNavLink":
                    ActivateNavLink(navLinkId);
                    break;
            }

        }

        private void ActivateNavLink(int navLinkId)
        {
            TabMgr.ActivateNavLink(navLinkId);
            this.Master.DisplayFriendlyMessage("Navigation link activated.");
            LoadTabInformation();
        }

        private void DeactivateNavLink(int navLinkId)
        {
            TabMgr.DeactivateNavLink(navLinkId);
            this.Master.DisplayFriendlyMessage("Navigation link de-activated.");
            LoadTabInformation();
        }

        private void ManageNavLink(int navLinkId)
        {
            OwnerUtil.ClearPlaceHolderControl(plcManageNavigationLink);
            OwnerUtil.ClearPlaceHolderControl(plcTargetAsset);

            hdnNavigationLinkId.Value = navLinkId.ToString();
            if (navLinkId > 0)
            {
                plcTargetAsset.Visible = false;
                
                NavigationLink link = TabMgr.GetNavigationLink(navLinkId);
                txtNavigationLinkText.Text = link.LinkText.Trim();
                if (link.SortOrder.HasValue)
                {
                    txtSortOrder.Text = link.SortOrder.ToString();
                }
                if (link.NavigationItemTypeId.HasValue)
                {
                    ddlItemType.ClearSelection();
                    WebUtil.SelectListItemByValue(ddlItemType, link.NavigationItemTypeId);
                }
                if (link.TargetId.HasValue)
                {
                    hdnNavLinkTargetId.Value = link.TargetId.ToString();
                }

                if (link.ParentNavigationLinkId.HasValue && link.ParentNavigationLinkId > 0)
                {
                    ddlParentLink.ClearSelection();
                    WebUtil.SelectListItemByValue(ddlParentLink, link.ParentNavigationLinkId);
                }

                if (link.NavigationActionId.HasValue && link.NavigationActionId > 0)
                {
                    ddlAction.ClearSelection();
                    WebUtil.SelectListItemByValue(ddlAction, link.NavigationActionId);
                    LoadTargetAssets(Util.ConvertInt32(ddlAction.SelectedValue));
                }

            }
            plcNavigationEditor.Visible = false;
            plcTabEditor.Visible = false;
            plcManageNavigationLink.Visible = true;

        }

        protected void grdvwNavigationLinkList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdvwNavigationLinkList.PageIndex = e.NewPageIndex;
            LoadTabInformation();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTabInformation();
        }

        protected void btnAddNavLink_Click(object sender, EventArgs e)
        {
            ManageNavLink(0);
        }

        protected void btnSaveNavigationLink_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "NavigationLinkInformation";
            if (Page.IsValid)
            {
                SaveNavigationLink();
                if (Page.IsValid)
                {
                    LoadSubNavLinks();
                    this.Master.DisplayFriendlyMessage("Navigation Link saved.");
                }
            }
        }

        private void SaveNavigationLink()
        {
            NavigationLink linkToSave = new NavigationLink();
            linkToSave.NavigationLinkId = Util.ConvertInt32(hdnNavigationLinkId.Value);

            if (ddlParentLink.SelectedIndex > 0)
            {
                linkToSave.ParentNavigationLinkId = Util.ConvertInt32(ddlParentLink.SelectedValue);
            }

            linkToSave.LinkText = txtNavigationLinkText.Text.Trim();
            linkToSave.ActiveFlag = true;

            if (ddlAction.SelectedIndex > 0)
            {
                linkToSave.NavigationActionId = Util.ConvertInt32(ddlAction.SelectedValue);
            }

            if (ddlItemType.SelectedIndex > 0)
            {
                linkToSave.NavigationItemTypeId = Util.ConvertInt32(ddlItemType.SelectedValue);
            }

            if (ddlTargetAsset.SelectedIndex > 0)
            {
                linkToSave.TargetId = Util.ConvertInt32(ddlTargetAsset.SelectedValue);
            }

            linkToSave.SortOrder = string.IsNullOrEmpty(txtSortOrder.Text) ? 1 : Util.ConvertInt32(txtSortOrder.Text);


            ValidationResults errors = TabMgr.SaveNavigationLink(linkToSave, Util.ConvertInt32(hdnTabLinkId.Value));

            if (!errors.IsValid)
            {
                this.PageErrors.AddErrorMessages(errors);
            }
        }

        protected void btnReturnToNavList_Click(object sender, EventArgs e)
        {
            LoadSubNavLinks();
        }

        #endregion

        #region Methods

        private void LoadTabInformation()
        {
            TabLink tab = TabMgr.GetTabByShowAndTabNumber(this.CurrentUser.CurrentShow.ShowId, 1);
            hdnTabLinkId.Value = tab.TabLinkId.ToString();
            txtTabLinkText.Text = tab.Text.Trim();
            txtTabNumber.Text = tab.TabNumber.HasValue ? tab.TabNumber.ToString() : string.Empty;

            hdnTabTargetId.Value = string.Empty;
            if (tab.TargetId.HasValue)
            {
                hdnTabTargetId.Value = tab.TargetId.Value.ToString();
            }

            LoadTabNavigationActions(tab, ddlTabNavigationAction);            

            chkTabVisible.Checked = tab.Visible;

            List<NavigationLink> navLinks = OwnerUtil.BuildAllActiveNavLinks(1, tab, chkIncludeInactive.Checked);

            grdvwNavigationLinkList.DataSource = navLinks;
            grdvwNavigationLinkList.DataBind();

            LoadTabTargetLinks(ddlTabTargetNavItem, navLinks);

            LoadParentLinks(tab, ddlParentLink);
            LoadNavLinkActions();
            LoadNavLinkItemTypes();

            TabNavigationActionSelected();
        }


        #region Load DataLookups
        private void LoadNavLinkActions()
        {
            WebUtil.LoadNavigationActionChoices(TabMgr, 1, ddlAction);
        }

        private void LoadNavLinkItemTypes()
        {
            ddlItemType.Items.Clear();
            ddlItemType.DataSource = TabMgr.GetNavigationItemTypes();
            ddlItemType.DataTextField = "Name";
            ddlItemType.DataValueField = "NavigationItemTypeId";
            ddlItemType.DataBind();
            ddlItemType.Items.Insert(0, new ListItem { Text = "-- Select --", Value = "-1" });
        }


        private void LoadTargetAssets(int actionId)
        {
            if (actionId > 0)
            {
                switch (actionId)
                {
                    case (int)NavigationLink.NavigationalAction.HtmlContent:
                        ddlTargetAsset.Items.Clear();
                        ddlTargetAsset.DataSource = new HtmlContentController().GetHtmlContentListByShowId(this.CurrentUser.CurrentShow.ShowId, false).OrderBy(h => h.Title);

                        ddlTargetAsset.DataTextField = "Title";
                        ddlTargetAsset.DataValueField = "HtmlContentId";
                        ddlTargetAsset.DataBind();
                        ddlTargetAsset.Items.Insert(0, new ListItem { Text = "-- Select --", Value = "-1" });
                        ltrTargetAsset.Text = "Html Content";
                        if (Util.ConvertInt32(hdnNavLinkTargetId.Value) > 0)
                        {
                            ddlTargetAsset.ClearSelection();
                            WebUtil.SelectListItemByValue(ddlTargetAsset, hdnNavLinkTargetId.Value);
                        }
                        plcTargetAsset.Visible = true;
                        break;
                    case (int)NavigationLink.NavigationalAction.FileDownload:
                        ddlTargetAsset.Items.Clear();
                        ddlTargetAsset.DataSource = new HtmlContentController().GetFileDowloadListByShowId(this.CurrentUser.CurrentShow.ShowId, false).OrderBy(f => f.FileName);

                        ddlTargetAsset.DataTextField = "FileName";
                        ddlTargetAsset.DataValueField = "FileDownLoadId";
                        ddlTargetAsset.DataBind();
                        ddlTargetAsset.Items.Insert(0, new ListItem { Text = "-- Select --", Value = "-1" });
                        ltrTargetAsset.Text = "File Download";
                        if (Util.ConvertInt32(hdnNavLinkTargetId.Value) > 0)
                        {
                            ddlTargetAsset.ClearSelection();
                            WebUtil.SelectListItemByValue(ddlTargetAsset, hdnNavLinkTargetId.Value);
                        }
                        plcTargetAsset.Visible = true;
                        break;
                    case (int)NavigationLink.NavigationalAction.ShowCategory:
                        ddlTargetAsset.Items.Clear();

                        ddlTargetAsset.DataSource = new ProductController().GetCategoryList(CurrentUser.CurrentShow.ShowId).Where(f => f.ActiveFlag).OrderBy(f => f.CategoryName);
                        ddlTargetAsset.DataTextField = "CategoryName";
                        ddlTargetAsset.DataValueField = "CategoryId";
                        ddlTargetAsset.DataBind();
                        ddlTargetAsset.Items.Insert(0, new ListItem { Text = "-- Select --", Value = "-1" });
                        ltrTargetAsset.Text = "Product Category";
                        if (Util.ConvertInt32(hdnNavLinkTargetId.Value) > 0)
                        {
                            ddlTargetAsset.ClearSelection();
                            WebUtil.SelectListItemByValue(ddlTargetAsset, hdnNavLinkTargetId.Value);
                        }
                        plcTargetAsset.Visible = true;
                        break;
                    case (int)NavigationLink.NavigationalAction.DynamicForm:
                        ddlTargetAsset.Items.Clear();
                        ddlTargetAsset.DataSource = new HtmlContentController().GetFormListByShowId(this.CurrentUser.CurrentShow.ShowId, false).OrderBy(f => f.FormName);

                        ddlTargetAsset.DataTextField = "FormName";
                        ddlTargetAsset.DataValueField = "FormId";
                        ddlTargetAsset.DataBind();
                        ddlTargetAsset.Items.Insert(0, new ListItem { Text = "-- Select --", Value = "-1" });
                        ltrTargetAsset.Text = "Form";
                        if (Util.ConvertInt32(hdnNavLinkTargetId.Value) > 0)
                        {
                            ddlTargetAsset.ClearSelection();
                            WebUtil.SelectListItemByValue(ddlTargetAsset, hdnNavLinkTargetId.Value);
                        }
                        plcTargetAsset.Visible = true;
                        break;
                    default:
                        plcTargetAsset.Visible = false;
                        break;
                }


            }
        }
        #endregion

        #endregion
    }
}