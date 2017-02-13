#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.UI.HtmlControls;

using ExpoOrders.Controllers;
using ExpoOrders.Common;
using ExpoOrders.Entities;
#endregion

namespace ExpoOrders.Web.Owners
{
    public partial class OwnerLanding : System.Web.UI.MasterPage
    {
        #region Private Members
        OwnerAdminController _ownerAdminCtrlr;
        #endregion

        #region Public Members
        public OwnerPage CurrentPage { get; set; }
        public List<TabLink> CurrentTopNavigationLinks { get; set; }
        public List<NavigationLink> CurrentSubNavigationLinks { get; set; }
        public OwnerAdminController OwnerAdminCtrlr
        {
            get
            {
                if (_ownerAdminCtrlr == null)
                {
                    _ownerAdminCtrlr = new OwnerAdminController();
                }
                return _ownerAdminCtrlr;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Header.DataBind();

            if (!this.IsPostBack)
            {
                LoadPage();
            }
            else
            {
                this.HideFriendlyMessage();
            }
        }

        public void LoadPage()
        {
            lblWebVersionNumber.Text = string.Format("Build {0}", WebUtil.WebVersionNumber());
        }

        public void BodyOnLoadScript(string onloadScript)
        {
            this.Body.Attributes["onLoad"] = onloadScript;
        }

        public void LoadMasterPage(ExpoOrdersUser currentUser, OwnerPage currentPage)
        {
            this.CurrentPage = currentPage;

            LoadOwnerStyle(currentUser);

            ltrCurrentUser.Text = string.Format("{0}", currentUser.DisplayUserName);
            List<TabLink> topNavigationLinks = this.OwnerAdminCtrlr.GetOwnerLandingTopNavigation();
            CurrentTopNavigationLinks = topNavigationLinks;

            this.TabsList.DataSource = topNavigationLinks;
            this.TabsList.DataBind();

            this.FooterNavigationLinks.DataSource = topNavigationLinks;
            this.FooterNavigationLinks.DataBind();

            if (NoSubNavigation == false)
            {
                topNavigationLinks.ForEach(tab =>
                {
                    if (tab.Page == EnumHelper.GetCodeValue(this.CurrentPage))
                    {
                        ICollection<NavigationLink> linksToRender = tab.NavigationLinks
                            .Where(n => n.ParentNavigationLinkId == null && n.ActiveFlag == true)
                            .OrderBy(p => p.SortOrder).ToList();
                        LoadSubNavigation(tab.Text, linksToRender);
                    }
                });
            }
        }

        public void LoadOwnerStyle(ExpoOrdersUser currentUser)
        {
            if (currentUser.CurrentOwner != null)
            {
                this.OwnerStyleSheet.Href = WebUtil.OwnerRelativeSharedFilePath(currentUser.CurrentOwner, currentUser.CurrentOwner.StyleSheetFileName);
                this.LeftLogo.Src = WebUtil.OwnerRelativeSharedFilePath(currentUser.CurrentOwner, currentUser.CurrentOwner.LogoFileName);
            }            
        }

        public void LoadSubNavigation(string subHeader, ICollection<NavigationLink> navLinks)
        {
            this.plcSubNavigationArea.Visible = true;

            LoadNavigationList(navLinks, SubNavigationList);

            this.SubNavigationHeaderText = subHeader;

        }

        private void LoadNavigationList(ICollection<NavigationLink> navLinks, Repeater navList)
        {
            //combines all the parents + children into one list (for simplicity)
            List<NavigationLink> linksToRender = new List<NavigationLink>();
            navLinks.ForEach<NavigationLink>(
                link =>
                {
                    linksToRender.Add(link);
                    if (link.ChildNavigationLinks.Count > 0)
                    {
                        link.ChildNavigationLinks
                     .Where(n => n.ActiveFlag == true)
                     .OrderBy(n => n.SortOrder).ToList().ForEach<NavigationLink>(
                     childLink =>
                     {
                         linksToRender.Add(childLink);
                     }
                     );
                    }
                }

                );

            if (linksToRender.Count > 0)
            {
                CurrentSubNavigationLinks = linksToRender;
                navList.DataSource = linksToRender;
                navList.DataBind();
                navList.Visible = true;
            }
        }


        public void DisplayTabContent(TabLink tab)
        {
            if (tab != null)
            {
                switch (tab.NavigationActionEnum)
                {
                    case NavigationLink.NavigationalAction.HtmlContent:
                        if (tab.TargetId.HasValue)
                        {
                            HtmlContentController controller = new HtmlContentController();
                            DisplayDynamicContent(controller.GetHtmlContentById(tab.TargetId.Value));
                        }
                        break;
                    case NavigationLink.NavigationalAction.SelectNavigationItem:
                        if (tab.TargetId.HasValue)
                        {
                            LinkButton lnkItem = FindNavLink(tab.TargetId.Value);
                            if (lnkItem != null)
                            {
                                SubNavClicked(lnkItem);
                            }
                        }

                        break;
                }
            }
        }

        public void HideDynamicContent()
        {
            DisplayDynamicContent(string.Empty, string.Empty);
        }

        public void DisplayDynamicContent(HtmlContent content)
        {
            this.DisplayDynamicContent(string.Empty, content);
        }

        public void DisplayDynamicContent(string contentHeader, HtmlContent content)
        {
            if (content != null)
            {
                DisplayDynamicContent(contentHeader, content.DynamicHtmlContent);
            }
            else
            {
                DisplayDynamicContent(string.Empty, string.Empty);
            }
        }

        public void DisplayDynamicContent(string contentHeader, string content)
        {
            if (!String.IsNullOrEmpty(contentHeader))
            {
                plcDynamicContentHeader.Visible = ltrDynamicContentHeader.Visible = true;
                ltrDynamicContentHeader.Text = contentHeader;
            }
            else
            {
                plcDynamicContentHeader.Visible = ltrDynamicContentHeader.Visible = false;
            }

            if (!String.IsNullOrEmpty(content))
            {
                ltrDynamicContent.Text = content;

                plcDynamicContent.Visible =
                       ltrDynamicContent.Visible = true;
            }
            else
            {
                plcDynamicContent.Visible =
                       ltrDynamicContent.Visible = false;
            }
        }

        public void HideFriendlyMessage()
        {
            this.plcFriendlyMessage.Visible = this.ltrFriendlyMessage.Visible = false;
            this.ltrFriendlyMessage.Text = string.Empty;
        }

        public void DisplayFriendlyMessage(string message)
        {
            this.plcFriendlyMessage.Visible =
                this.ltrFriendlyMessage.Visible = true;

            this.ltrFriendlyMessage.Text += message + "<br/>";
        }

        public void DisplayFriendlyMessage(List<string> messages)
        {
            messages.ForEach(msg => { DisplayFriendlyMessage(msg); });
        }

        private bool _noSubNavigation = false;
        public bool NoSubNavigation
        {
            get
            {
                return _noSubNavigation;
            }
            set
            {
                _noSubNavigation = value;
                tdSubNavigationArea.Attributes["class"] = value ? "no-subNavigation-area" : "subNavigation-area";
            }
        }

        private Action<int, string, NavigationLink.NavigationalAction, int> _navItemCallBack;
        public Action<int, string, NavigationLink.NavigationalAction, int> NavigationItemCallBack
        {
            get
            {
                return _navItemCallBack;
            }
            set
            {
                _navItemCallBack = value;
            }
        }


        public string SubNavigationHeaderText
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    SubNavHeaderText.Text = value;
                    SubNavHeaderText.Visible = true;
                }
                else
                {
                    SubNavHeaderText.Visible = false;
                }

            }
        }


        protected void TabsList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item
                || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                TabLink tabData = (TabLink)e.Item.DataItem;
                HtmlAnchor tabLink = (HtmlAnchor)e.Item.FindControl("tabLink");
                HtmlGenericControl tabListItem = (HtmlGenericControl)e.Item.FindControl("tabListItem");

                tabLink.InnerHtml = string.Format("<span>{0}</span>", tabData.Text);
                tabLink.HRef = tabData.Page;
                tabLink.Attributes["class"] = "tabItem";
                if (tabData.Page == EnumHelper.GetCodeValue(this.CurrentPage))
                {
                    tabListItem.Attributes["class"] = tabLink.Attributes["class"] = "tabItemSelected";
                }
                tabListItem.Visible = tabData.Visible;

            }
        }

        protected void FooterNavigationLinks_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item
                || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                TabLink tabData = (TabLink)e.Item.DataItem;
                HyperLink lnkFooterNavigationLink = (HyperLink)e.Item.FindControl("lnkFooterNavigationLink");
                lnkFooterNavigationLink.Text = tabData.Text;
                lnkFooterNavigationLink.NavigateUrl = tabData.Page;
            }
        }

        protected void SubNavigationList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item
                || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                NavigationLink navLinkData = (NavigationLink)e.Item.DataItem;
                CreateRepeaterItemTemplate(navLinkData, e);
            }
        }


        private static void CreateRepeaterItemTemplate(NavigationLink navLinkData, RepeaterItemEventArgs e)
        {

            HtmlGenericControl NavigationListItem = (HtmlGenericControl)e.Item.FindControl("NavigationListItem");
            Label lblItem = (Label)e.Item.FindControl("lblItem");
            LinkButton lnkItem = (LinkButton)e.Item.FindControl("lnkItem");


            lblItem.Visible = false;
            lnkItem.Visible = false;

            if (Enum<NavigationLink.NavigationalItemType>.Parse(navLinkData.NavigationItemType.Name) == NavigationLink.NavigationalItemType.Nav)
            {
                lnkItem.CssClass = lblItem.CssClass = "navItem";
                NavigationListItem.Attributes["class"] = "navItem";
            }
            else if (Enum<NavigationLink.NavigationalItemType>.Parse(navLinkData.NavigationItemType.Name) == NavigationLink.NavigationalItemType.SubNav)
            {
                lnkItem.CssClass = lblItem.CssClass = "subNavItem";
                NavigationListItem.Attributes["class"] = "subNavItem";
            }

            NavigationLink.NavigationalAction navAction = Enum<NavigationLink.NavigationalAction>.Parse(navLinkData.NavigationAction.Name);

            int targetId = 0;
            if (navLinkData.TargetId.HasValue)
            {
                targetId = navLinkData.TargetId.Value;
            }

            //Navigation Action (PostBack, Showcategory, etc) will go in Commandname
            lnkItem.CommandName = navAction.ToString();

            //CommandArgument will be the ID of the NavLink
            lnkItem.CommandArgument = string.Format("{0}|{1}|{2}|{3}|{4}", navLinkData.NavigationLinkId, navLinkData.LinkText, targetId, lnkItem.CssClass, navLinkData.ParentNavigationLinkId);

            switch (navAction)
            {
                case NavigationLink.NavigationalAction.TextOnly:
                    lblItem.Visible = true;
                    lblItem.Text = navLinkData.LinkText;
                    break;
                case NavigationLink.NavigationalAction.FileDownload:
                    lblItem.Visible = false;
                    lnkItem.Visible = true;
                    lnkItem.Text = navLinkData.LinkText;
                    if (navLinkData.TargetId.HasValue)
                    {
                        lnkItem.Attributes.Add("OnClick", string.Format("launchFileDownload({0}); return false;", navLinkData.TargetId));
                    }
                    else
                    {
                        lnkItem.Attributes.Add("OnClick", "return false;");
                    }

                    break;
                case NavigationLink.NavigationalAction.AdvanceWarehouseLabel:
                case NavigationLink.NavigationalAction.DirectShowSiteLabel:
                    lblItem.Visible = false;
                    lnkItem.Visible = true;
                    lnkItem.Text = navLinkData.LinkText;
                    if (navLinkData.TargetId.HasValue)
                    {
                        lnkItem.Attributes.Add("OnClick", string.Format("launchLabelGenerator('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'); return false;", navLinkData.TargetId, "", "", "", "", "", "", ""));
                    }
                    else
                    {
                        lnkItem.Attributes.Add("OnClick", "return false;");
                    }

                    break;
                default:
                    lnkItem.Visible = true;
                    lnkItem.Text = navLinkData.LinkText;

                    break;
            }

        }


        private string[] ParseLinkItemCommandArguments(LinkButton lnkItem)
        {
            char[] delimiter = new char[] { '|' };

            return lnkItem.CommandArgument.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
        }

        protected void lnkItem_Click(object sender, EventArgs e)
        {
            if (_navItemCallBack != null)
            {
                if (sender is LinkButton)
                {
                    LinkButton lnkItem = (LinkButton)sender;

                    SubNavClicked(lnkItem);
                }
            }
        }

        private void SubNavClicked(LinkButton lnkItem)
        {
            NavigationLink.NavigationalAction navAction = Enum<NavigationLink.NavigationalAction>.Parse(lnkItem.CommandName, true);

            string[] linkAttributes = ParseLinkItemCommandArguments(lnkItem);

            int navLinkId = Int32.Parse(linkAttributes[0]);
            string navLinkText = linkAttributes[1];
            int targetId = Int32.Parse(linkAttributes[2]);

            this.SelectNavigationItem(lnkItem);

            _navItemCallBack(navLinkId, navLinkText, navAction, targetId);
        }

        public void SelectNavigationItem(int navLinkIdToSelect)
        {
            //Reset all non-selected items
            foreach (RepeaterItem item in this.SubNavigationList.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    LinkButton lnkItem = (LinkButton)item.FindControl("lnkItem");
                    HtmlGenericControl NavigationListItem = (HtmlGenericControl)item.FindControl("NavigationListItem");

                    if (lnkItem != null)
                    {
                        string[] commandArgs = ParseLinkItemCommandArguments(lnkItem);

                        if (commandArgs.Length > 0)
                        {
                            if (navLinkIdToSelect == Int32.Parse(commandArgs[0]))
                            {
                                lnkItem.CssClass = NavigationListItem.Attributes["class"] = commandArgs[3] + "Selected";
                            }
                            else
                            {
                                lnkItem.CssClass = NavigationListItem.Attributes["class"] = commandArgs[3];
                            }
                        }
                    }
                }
            }
        }

        public string ParentNavigationText(int currentNavLinkId)
        {
            string parentNavText = string.Empty;

            LinkButton lnkItem = FindNavLink(currentNavLinkId);
            if (lnkItem != null)
            {
                string[] commandArgs = ParseLinkItemCommandArguments(lnkItem);

                parentNavText = commandArgs[1];
                if (commandArgs.Length >= 5)
                {
                    string parentIdCommandArg = commandArgs[4];
                    if (parentIdCommandArg != null)
                    {
                        int parentNavLinkId = Int32.Parse(parentIdCommandArg);
                        if (parentNavLinkId > 0)
                        {
                            LinkButton lnkParent = FindNavLink(parentNavLinkId);
                            if (lnkParent != null)
                            {
                                parentNavText = ParseLinkItemCommandArguments(lnkParent)[1];
                            }
                        }
                    }
                }
            }

            return parentNavText;
        }

        private LinkButton FindNavLink(int navLinkIdToFind)
        {
            LinkButton lnkItemToFind = null;
            foreach (RepeaterItem item in this.SubNavigationList.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    LinkButton currentLinkButton = (LinkButton)item.FindControl("lnkItem");

                    if (currentLinkButton != null)
                    {
                        string[] commandArgs = ParseLinkItemCommandArguments(currentLinkButton);

                        if (commandArgs.Length > 0)
                        {
                            if (navLinkIdToFind == Int32.Parse(commandArgs[0]))
                            {
                                lnkItemToFind = currentLinkButton;
                                break;
                            }
                        }
                    }
                }
            }

            return lnkItemToFind;
        }


        private void SelectNavigationItem(LinkButton itemToSelect)
        {
            string[] selectedItemCommandArgs = ParseLinkItemCommandArguments(itemToSelect);

            SelectNavigationItem(Int32.Parse(selectedItemCommandArgs[0]));
        }

    }
}