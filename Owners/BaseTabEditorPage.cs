using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExpoOrders.Entities;
using System.Web.UI.WebControls;
using ExpoOrders.Web.Owners.Common;
using ExpoOrders.Common;

using ExpoOrders.Controllers;

namespace ExpoOrders.Web.Owners
{
    public class BaseTabEditorPage : BaseOwnerPage
    {

        public void LoadParentLinks(TabLink tab, DropDownList ddlParentLink)
        {
            ddlParentLink.Items.Clear();

            //RD: filter out the 'Nav' ones only
            ddlParentLink.DataSource = OwnerUtil.RetrieveAllActiveNavLinks(tab, false).Where(link => link.NavigationItemType != null && link.NavigationItemType.Name == NavigationLink.NavigationalItemType.Nav.ToString());

            ddlParentLink.DataTextField = "LinkText";
            ddlParentLink.DataValueField = "NavigationLinkId";
            ddlParentLink.DataBind();
            ddlParentLink.Items.Insert(0, new ListItem { Text = "-- Select --", Value = "-1" });
        }

        public void HandleNavigationPostBack(ShowSettingPageMode currentPageMode)
        {
            switch (currentPageMode)
            {
                case ShowSettingPageMode.Tab1Editor:
                    Server.Transfer(OwnerPage.Tab1Editor.GetCodeValue());
                    break;
                case ShowSettingPageMode.Tab2Editor:
                    Server.Transfer(OwnerPage.Tab2Editor.GetCodeValue());
                    break;
                case ShowSettingPageMode.Tab3Editor:
                    Server.Transfer(OwnerPage.Tab3Editor.GetCodeValue());
                    break;
                case ShowSettingPageMode.Tab4Editor:
                    Server.Transfer(OwnerPage.Tab4Editor.GetCodeValue());
                    break;
                case ShowSettingPageMode.Tab5Editor:
                    Server.Transfer(OwnerPage.Tab5Editor.GetCodeValue());
                    break;
                case ShowSettingPageMode.Products:
                    Server.Transfer(OwnerPage.Products.GetCodeValue());
                    break;
                default:
                    Server.Transfer(string.Format("{0}?targetId={1}", OwnerPage.ShowSettings.GetCodeValue(), (int)currentPageMode));
                    break;
            }
        }

        public void LoadTabNavigationActions(TabLink tab, DropDownList ddl)
        {
            if (ddl != null)
            {
                ddl.Items.Clear();
                ddl.Items.Add(new ListItem("Default", NavigationLink.NavigationalAction.NotSet.ToString()));

                ddl.Items.Add(new ListItem("Load Nav Item", NavigationLink.NavigationalAction.SelectNavigationItem.ToString()));
                ddl.Items.Add(new ListItem("Display Html", NavigationLink.NavigationalAction.HtmlContent.ToString()));

                if (tab.NavigationActionId.HasValue)
                {
                    NavigationLink.NavigationalAction tabActionToSelect = Enum<NavigationLink.NavigationalAction>.Parse(tab.NavigationActionId.ToString());
                    WebUtil.SelectListItemByValue(ddl, tabActionToSelect.ToString());
                }
            }
        }

        public void LoadTabTargets(NavigationLink.NavigationalAction tabAction, DropDownList ddlTabTargetHtmlContent, DropDownList ddlTabTargetNavItem, PlaceHolder plcTabTarget, Label lblTabTarget, HiddenField hdnTabTargetId)
        {
            ddlTabTargetNavItem.Visible = false;
            ddlTabTargetHtmlContent.Visible = false;
            

            if (tabAction == NavigationLink.NavigationalAction.HtmlContent)
            {
                ddlTabTargetHtmlContent.Items.Clear();

                plcTabTarget.Visible = true;
                lblTabTarget.Text = "Html Content:";

                ddlTabTargetHtmlContent.DataSource = new HtmlContentController().GetHtmlContentListByShowId(CurrentUser.CurrentShow.ShowId, false).Where(h => h.ActiveFlag).OrderBy(h => h.Title);
                ddlTabTargetHtmlContent.DataTextField = "Title";
                ddlTabTargetHtmlContent.DataValueField = "HtmlContentId";
                ddlTabTargetHtmlContent.DataBind();
                ddlTabTargetHtmlContent.Visible = true;

                WebUtil.SelectListItemByValue(ddlTabTargetHtmlContent, hdnTabTargetId.Value);
            }
            else if (tabAction == NavigationLink.NavigationalAction.SelectNavigationItem)
            {
                plcTabTarget.Visible = true;
                lblTabTarget.Text = "Nav Item #:";

                ddlTabTargetNavItem.Visible = true;

                WebUtil.SelectListItemByValue(ddlTabTargetNavItem, hdnTabTargetId.Value);
                
            }
        }

        public void LoadTabTargetLinks(DropDownList ddlTabTargetNavItem, List<NavigationLink> navLinks)
        {
            if (navLinks != null)
            {
                ddlTabTargetNavItem.DataTextField = "LinkText";
                ddlTabTargetNavItem.DataValueField = "NavigationLinkId";

                ddlTabTargetNavItem.DataSource = navLinks;
                ddlTabTargetNavItem.DataBind();
            }
        }

    }
}