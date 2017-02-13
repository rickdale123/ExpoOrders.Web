using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ExpoOrders.Common;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;
using ExpoOrders.Entities;
using ExpoOrders.Web.CustomControls;
using ExpoOrders.Controllers;

namespace ExpoOrders.Web
{
    public static class WebUtil
    {
        public enum HostEnum
        {
            [CodeValue("expoorders")]
            ExpoOrders = 0,
            [CodeValue("acme")]
            Acme,
            [CodeValue("alliance")]
            Alliance,
            [CodeValue("summit")]
            Summit,
            [CodeValue("inhouse")]
            InHouse
        }

       
        public static string CurrentHost()
        {
           return HttpContext.Current.Request.Url.Host;
        }

        #region Web UI helpers

        public static void SelectListItemByValue(RadioButtonList rdoLst, string valueToFind)
        {
            if (rdoLst != null)
            {
                ListItem li = rdoLst.Items.FindByValue(valueToFind);
                if (li != null)
                {
                    li.Selected = true;
                }
            }
        }

        public static void SelectListItemByValue(DropDownList ddl, string valueToFind)
        {
            if (ddl != null)
            {
                if (ddl.Items.Count > 0)
                {
                    ListItem itemToSelect = ddl.Items.FindByValue(valueToFind);
                    if (itemToSelect != null)
                    {
                        itemToSelect.Selected = true;
                    }
                }
            }
        }

        public static void SelectListItemByValue(System.Web.UI.WebControls.DropDownList ddl, int valueToFind)
        {
            SelectListItemByValue(ddl, valueToFind.ToString());
        }

        public static void SelectListItemByValue(System.Web.UI.WebControls.DropDownList ddl, int? valueToFind)
        {
            if (valueToFind.HasValue)
            {
                SelectListItemByValue(ddl, valueToFind.Value);
            }
        }

        public static void DefaultTextBoxValue(TextBox txt, string val)
        {
            if (txt != null)
            {
                if (string.IsNullOrEmpty(txt.Text.Trim()))
                {
                    txt.Text = val;
                }
            }
        }

        public static void RenameListItem(DropDownList ddl, string valueToFind, string newDescription)
        {
            if (ddl != null && ddl.Items.Count > 0)
            {
                ListItem li = ddl.Items.FindByValue(valueToFind);
                if (li != null)
                {
                    li.Text = newDescription;
                }
            }
        }

        public static void LoadNavigationActionChoices(TabConfigController tabMgr, int tabNbr, DropDownList ddlAction)
        {
            if (ddlAction != null)
            {
                ddlAction.Enabled = true;
                ddlAction.Visible = true;
                ddlAction.Items.Clear();
                ddlAction.DataSource = tabMgr.GetNavLinkActions().Where(n => n.NavigationActionId == (int)NavigationLink.NavigationalAction.FileDownload
                                                                    || n.NavigationActionId == (int)NavigationLink.NavigationalAction.ShowCategory
                                                                    || n.NavigationActionId == (int)NavigationLink.NavigationalAction.HtmlContent
                                                                    || n.NavigationActionId == (int)NavigationLink.NavigationalAction.TextOnly
                                                                    || n.NavigationActionId == (int)NavigationLink.NavigationalAction.DynamicForm
                                                                    || n.NavigationActionId == (int)NavigationLink.NavigationalAction.AdvanceWarehouseLabel
                                                                    || n.NavigationActionId == (int)NavigationLink.NavigationalAction.DirectShowSiteLabel
                                                                    || n.NavigationActionId == (int)NavigationLink.NavigationalAction.OutboundShippingLabel).OrderBy(n => n.SortOrder);
                ddlAction.DataTextField = "Name";
                ddlAction.DataValueField = "NavigationActionId";
                ddlAction.DataBind();
                ddlAction.Items.Insert(0, new ListItem { Text = "-- Select --", Value = "-1" });

                WebUtil.RenameListItem(ddlAction, (int)NavigationLink.NavigationalAction.FileDownload, "Download a File");
                WebUtil.RenameListItem(ddlAction, (int)NavigationLink.NavigationalAction.HtmlContent, "Display Html Content");
                WebUtil.RenameListItem(ddlAction, (int)NavigationLink.NavigationalAction.TextOnly, "Plain Text Only");
                WebUtil.RenameListItem(ddlAction, (int)NavigationLink.NavigationalAction.DynamicForm, "Load Dynamic Form");
                WebUtil.RenameListItem(ddlAction, (int)NavigationLink.NavigationalAction.AdvanceWarehouseLabel, "Open Advance Warehouse Shipping Label");
                WebUtil.RenameListItem(ddlAction, (int)NavigationLink.NavigationalAction.DirectShowSiteLabel, "Open Direct Show Site Shipping Label");
                WebUtil.RenameListItem(ddlAction, (int)NavigationLink.NavigationalAction.OutboundShippingLabel, "Jump to Outbound Shipping Label Screen");

                if (tabNbr == 3)
                {
                    WebUtil.RenameListItem(ddlAction, (int)NavigationLink.NavigationalAction.ShowCategory, "Load Category");
                }
                else
                {
                    WebUtil.RenameListItem(ddlAction, (int)NavigationLink.NavigationalAction.ShowCategory, "Jump To Category");
                }
            }
            
        }


        public static void RenameListItem(DropDownList ddl, int valueToFind, string newDescription)
        {
            RenameListItem(ddl, valueToFind.ToString(), newDescription);
        }

        public static string WebVersionNumber()
        {
            Version vers = Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("{0}.{1}.{2}.{3}", vers.Major, vers.Minor, vers.Build, vers.Revision);
        }

        public static string RequestorBrowserInformation(HttpRequest request)
        {
            StringBuilder sb = new StringBuilder();

            using(XmlWriter w = XmlWriter.Create(sb))
            {

                HttpBrowserCapabilities bc = request.Browser;
                w.WriteStartElement("BrowserCapability");

                w.WriteElementString("Type", bc.Type);
                w.WriteElementString("Browser", bc.Browser);
                w.WriteElementString("Version", bc.Version);
                w.WriteElementString("MajorVersion", bc.MajorVersion.ToString());
                w.WriteElementString("MinorVersion", bc.MinorVersion.ToString());
                w.WriteElementString("Platform", bc.Platform);
                w.WriteElementString("Beta", bc.Beta.ToString());
                w.WriteElementString("Crawler", bc.Crawler.ToString());
                w.WriteElementString("AOL", bc.AOL.ToString());
                w.WriteElementString("Win16", bc.Win16.ToString());
                w.WriteElementString("Win32", bc.Win32.ToString());
                w.WriteElementString("Frames", bc.Frames.ToString());
                w.WriteElementString("Tables", bc.Tables.ToString());
                w.WriteElementString("Cookies", bc.Cookies.ToString());
                w.WriteElementString("VBScript", bc.VBScript.ToString());
                w.WriteElementString("EcmaScriptVersion", bc.EcmaScriptVersion.ToString());
                w.WriteElementString("JavaApplets", bc.JavaApplets.ToString());
                w.WriteElementString("ActiveXControls", bc.ActiveXControls.ToString());

                w.WriteEndElement();

            }

            return sb.ToString();

        }

        public static string HtmlEncode(object val)
        {
            string htmlEncodedVal = string.Empty;
            if (val != null)
            {
                htmlEncodedVal = HttpContext.Current.Server.HtmlEncode(val.ToString());
            }
            return htmlEncodedVal;
        }

        public static string OwnerRelativeSharedFolder(Owner owner)
        {
            return OwnerRelativeSharedFolder(owner.CommonFolder);
        }

        public static string OwnerRelativeSharedFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
               // throw new Exception("Owner Folder is empty.");
            }
            return string.Format("~/Assets/Owner/{0}/", folder);
        }

        public static string OwnerRelativeSharedFilePath(Owner owner, string fileName)
        {
            return string.Format("{0}{1}", OwnerRelativeSharedFolder(owner), fileName);
        }

        public static string ShowRelativeFolder(Show show)
        {
            if (show != null)
            {
                // throw new Exception("Show is empty.");
            }
            return string.Format("~/Assets/Shows/{0}/", show.ShowGuid.ToString());
        }


        public static string OwnerRelativeSharedFilePath(string folder, string fileName)
        {
            return string.Format("{0}{1}", OwnerRelativeSharedFolder(folder), fileName);
        }

        public static string ShowRelativeFilePath(Show show, string fileName)
        {
            return string.Format("{0}{1}", ShowRelativeFolder(show), fileName);
        }

        public static string OwnerSharedFileDirectory(Owner owner)
        {
            return OwnerSharedFileDirectory(owner.CommonFolder);
        }

        public static string OwnerSharedFileDirectory(string folder)
        {
            return HttpContext.Current.Server.MapPath(OwnerRelativeSharedFolder(folder));
        }

        public static string OwnerSharedFilePath(Owner owner, string fileName)
        {
            return string.Format("{0}{1}", OwnerSharedFileDirectory(owner), fileName);
        }

        public static string FormatFormSubmissionDeadlineMessage(Form form)
        {
            string message = "The deadline for submitting this form was {0}.";

            if (form != null && form.SubmissionDeadline.HasValue)
            {
                message = string.Format(message, form.SubmissionDeadline.Value.ToShortDateString());
            }
            return message;
        }

        public static void DisplayForm(ExpoOrdersUser currentUser, Form currentForm, PlaceHolder plcDynamicForm, FormQuestions ucFormQuestions, Label lblFormSubmissionDeadlineError, Literal ltrFormName, Literal ltrFormDescription, Button btnSubmitForm)
        {
            plcDynamicForm.Visible = ucFormQuestions.Visible = true;
            lblFormSubmissionDeadlineError.Visible = false;

            if (currentForm != null)
            {
                ltrFormName.Text = currentForm.FormName;
                ltrFormDescription.Text = GetFormDescription(currentUser, currentForm);

                ltrFormName.Visible = ltrFormDescription.Visible = true;

                if (currentForm.SubmissionDeadline.HasValue && Util.IsPassedDeadline(currentForm.SubmissionDeadline, DateTime.Now))
                {
                    lblFormSubmissionDeadlineError.Text = WebUtil.FormatFormSubmissionDeadlineMessage(currentForm);

                    lblFormSubmissionDeadlineError.Visible = true;
                    ucFormQuestions.Visible = false;
                    btnSubmitForm.Visible = false;

                }
                else
                {
                    lblFormSubmissionDeadlineError.Visible = false;
                    
                    ucFormQuestions.Visible = true;
                    btnSubmitForm.Visible = true;

                    ucFormQuestions.PopulateQuestions(currentForm.Questions
                                                .OrderBy(q => q.SortOrder)
                                                .ToList());
                }
            }
        }

        private static string GetFormDescription(ExpoOrdersUser currentUser, Form currentForm)
        {
            string submissionDeadline = currentForm.SubmissionDeadline != null ? currentForm.SubmissionDeadline.Value.ToShortDateString().ToString() : string.Empty;
            string descriptionWithDeadline = string.Format("Please submit this form to {1} by {0}", submissionDeadline, currentUser.CurrentShow.Owner.OwnerName);
            return currentForm.SubmissionDeadline != null ? string.Format("{0} {1}", currentForm.FormDescription, descriptionWithDeadline) : currentForm.FormDescription;
        }

        public static void HideForm(PlaceHolder plcDynamicForm, FormQuestions ucFormQuestions)
        {
            plcDynamicForm.Visible = ucFormQuestions.Visible = false;
        }

        public static void SortList<T>(List<T> dataSource, string fieldName, SortDirection sortDirection)
        {
            PropertyInfo propInfo = typeof(T).GetProperty(fieldName);
            Comparison<T> compare = delegate(T a, T b)
            {
                bool asc = sortDirection == SortDirection.Ascending;
                object valueA = asc ? propInfo.GetValue(a, null) : propInfo.GetValue(b, null);
                object valueB = asc ? propInfo.GetValue(b, null) : propInfo.GetValue(a, null);

                return valueA is IComparable ? ((IComparable)valueA).CompareTo(valueB) : 0;
            };
            dataSource.Sort(compare);
        }

        public static int GetColumnIndex(GridView gv, string sortExpression)
        {
            int i = 0;

            foreach (DataControlField col in gv.Columns)
            {
                if (col.SortExpression == sortExpression)
                    break;
                i++;
            }

            return i;
        }


        #endregion

    }
}