using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Entities;
using ExpoOrders.Controllers;

using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Web.Security;
using System.Text.RegularExpressions;
using Telerik.Web.UI;

namespace ExpoOrders.Web
{
    
    public abstract class BasePage : System.Web.UI.Page
    {
        public enum PageRedirect { UserLogin, ExhibitorHome, OwnerHome, SuperAdminHome, FAQ }
         
        #region Members

        private bool _requiresSsl = true;
        public bool RequiresSsl
        {
            get { return _requiresSsl; }
            set { _requiresSsl = value; }
        }
       
        #endregion

        #region Overrides


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            EnforceSSLRequirement();
            EstablishCachingRules();
            base.OnLoad(e);
        }

        #endregion

        #region Methods

        public List<string> ConvertStringArray(List<int> idList)
        {
            List<string> stringArray = new List<string>();

            idList.ForEach(i => stringArray.Add(i.ToString()));

            return stringArray;

        }

        public Help GetHelpContent(string helpCode)
        {
            HtmlContentController cntrl = new HtmlContentController();
            return cntrl.GetHelpContentByCode(helpCode);
        }

        public string GetHelpText(string helpCode)
        {
            HtmlContentController cntrl = new HtmlContentController();
            Help helpContent = cntrl.GetHelpContentByCode(helpCode);
            return helpContent != null ? helpContent.HelpText : string.Empty;
        }

        public string ConfigureToolTip(RadToolTip rtt, string helpCode)
        {
            //REturning String since the inline Html Text works better than setting the .RadTooltip.Text programmatically
            string content = string.Empty;
            Help helpDetail = GetHelpContent(helpCode);

            if (helpDetail != null)
            {
                content = helpDetail.HelpText;

                if (rtt != null)
                {
                    rtt.Title = helpDetail.HelpTitle;
                }
            }

            return content;
        }


        protected string HtmlEncode(object val)
        {
            return WebUtil.HtmlEncode(val);
        }

        protected string GetAlternatingClassName(int itemIndex, string itemClassName, string altItemClassName)
        {
            return Util.GetAlternatingClassName(itemIndex, itemClassName, altItemClassName);
        }

        protected int DesiredQueryStringShowId()
        {
            int showId = 0;


            if (Request.QueryString["showId"] != null)
            {
                showId = Util.ConvertInt32(Request.QueryString["showId"]);
            }
            else if (Request.QueryString["show"] != null)
            {
                showId = Util.ConvertInt32(Request.QueryString["show"]);
            }
            else if (Request.QueryString["id"] != null)
            {
                showId = Util.ConvertInt32(Request.QueryString["id"]);
            }
            else if (Request.QueryString["s"] != null)
            {
                showId = Util.ConvertInt32(Request.QueryString["s"]);
            }

            if (showId <= 0)
            {
                string queryString = Request.QueryString.ToString();
                if (!string.IsNullOrEmpty(queryString))
                {
                    showId = Util.ConvertInt32(queryString);
                }
            }

            return showId;
        }

        public string FormatHtmlEncoded(object data)
        {
            string encodedData = string.Empty;
            if (data != null)
            {
                encodedData = Server.HtmlEncode(data.ToString().Trim());
            }
            return encodedData;
        }

        public string QueryStringValue(string parameterName)
        {
            string queryStringVal = string.Empty;

            if (Request[parameterName] != null)
            {
                queryStringVal = Request[parameterName].ToString();
            }
            
            return queryStringVal;
        }
        
        private void EnforceSSLRequirement()
        {
            if (RequiresSsl && FormsAuthentication.RequireSSL && !Request.IsSecureConnection)
            {
                Response.Redirect(BuildSslUrl(Request.Url.ToString()));
            }
        }

        public string BuildSslUrl(string httpUrl)
        {
            if (FormsAuthentication.RequireSSL)
            {
                return Regex.Replace(httpUrl, "http", "https");
            }
            else
            {
                return Regex.Replace(httpUrl, "https", "http");
            }
            
        }

        public string BuildSslUrl(string host, string relativeUrl)
        {
            string httpPrefix = "http://";
            if (FormsAuthentication.RequireSSL)
            {
                httpPrefix = "https://";
            }

            return string.Format("{0}{1}{2}", httpPrefix, host, relativeUrl);
        }

        protected void EstablishCachingRules()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Response.Headers["Cache-Control"] = "no-cache, no-store, max-age=0, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "Fri, 01 Jan 1990 00:00:00 GMT";
        }

        protected void RedirectToPage(PageRedirect page)
        {
            switch (page)
            {
                case PageRedirect.UserLogin:
                    Server.Transfer("~/Login.aspx");
                    break;
                case PageRedirect.FAQ:
                    Server.Transfer("~/Default.aspx");
                    break;
                case PageRedirect.SuperAdminHome:
                    Server.Transfer("~/SuperAdmin/Default.aspx");
                    break;
                case PageRedirect.ExhibitorHome:
                    Server.Transfer("~/Exhibitors/Default.aspx");
                    break;
                default:
                    Server.Transfer("~/Default.aspx");
                    break;
            }
        }

        #endregion

        #region Page Entities

        private ExpoOrdersUser _currentUser = null;
        protected ExpoOrdersUser CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    if (Context != null && Context.Session != null)
                    {
                        if (Context.Session["CurrentUser"] != null)
                        {
                            _currentUser = (ExpoOrdersUser)Context.Session["CurrentUser"];
                        }
                        else
                        {
                            _currentUser = new ExpoOrdersUser();
                            Context.Session["CurrentUser"] = _currentUser;
                        }
                    }
                }
                return _currentUser;
            }
            
        }
        #endregion

        #region Cookies

        public string ReadCookieValue(string name)
        {
            string cookieVal = string.Empty;

            HttpCookie cookie = Request.Cookies[name];

            if (cookie != null)
            {
                cookieVal = cookie.Value;
            }

            return cookieVal;
        }

        public void WriteCookieValue(string name, string value)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = value;
            cookie.Expires = DateTime.Now.AddYears(2);
            Response.Cookies.Add(cookie);
        }

        
        public SortDirection DetermineSortDirection(bool autoChangeSortDir, string column, string columnCookieName, string sortDirCookieName)
        {
            var sortDir = SortDirection.Ascending;

            var lastSortExp = ReadCookieValue(columnCookieName);
            var lastSortDir = ReadCookieValue(sortDirCookieName);

            if (autoChangeSortDir)
            {
                if (column == lastSortExp)
                {
                    if (lastSortDir == SortDirection.Ascending.ToString())
                    {
                        sortDir = SortDirection.Descending;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lastSortDir))
                {
                    sortDir = (SortDirection)Enum.Parse(typeof(SortDirection), lastSortDir);
                }
            }


            WriteCookieValue(columnCookieName, column);
            WriteCookieValue(sortDirCookieName, sortDir.ToString());

            return sortDir;
        }

        public void PutSortDirectionImage(GridView grdvw, string sortExpression, SortDirection sortDir)
        {
            Image imageCntrl = new Image();

            string sortFileName = (sortDir == SortDirection.Descending) ? "arrow-up.png" : "arrow-down.png";
            imageCntrl.ImageUrl = WebUtil.OwnerRelativeSharedFilePath(CurrentUser.CurrentOwner, sortFileName);

            try
            {
                if (grdvw != null && grdvw.HeaderRow != null && sortExpression != null)
                {
                    grdvw.HeaderRow.Cells[WebUtil.GetColumnIndex(grdvw, sortExpression)].Controls.Add(imageCntrl);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                grdvw.HeaderRow.Cells[0].Controls.Add(imageCntrl);
            }
        }

        #endregion


    }
}