using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.EnterpriseLibrary.Validation;

using System.Configuration;
using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using ExpoOrders.Web;
using System.IO;

namespace ExpoOrders.Web
{
    public partial class DefaultMaster : System.Web.UI.MasterPage
    {
        private string _hostConfigContactUsEmail = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            this.AppendOnLoad("loadPage();");
            //lblWebVersionNumber.Text = string.Format("Build {0}", WebUtil.WebVersionNumber());
        }

        /// <summary>
        /// Returns the OwnerId of the Host, if applicable
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public int LoadHost(string host)
        {
            int ownerId = 0;
            plcOwnerHeader.Visible = true;
            plcShowHeader.Visible = false;

            OwnerHostConfigEntry hostConfigEntry = CommonConfig.OwnerHostConfigEntries.FirstOrDefault(o => o.UrlHost.ToLower() == host.ToLower());


            //string hostConfigs = Util.ReadAppSetting(string.Format("Host.Config.{0}", host.ToLower()));

            //string logoFileName = "expoorders-logo.png";
            string cssFileName = "expoorders.css";
            _hostConfigContactUsEmail = string.Empty;
            string commonFolder = string.Empty;
            if (hostConfigEntry != null)
            {
                //string[] hostParams = hostConfigs.Split(';');
                ownerId = hostConfigEntry.OwnerId;
                //logoFileName = hostConfig.CompanyLogo;
                cssFileName = hostConfigEntry.CssFileName;
                _hostConfigContactUsEmail = hostConfigEntry.ContactEmail;
                commonFolder = hostConfigEntry.OwnerCommonFolder;
            }

            string cacheBustingVersion = Util.ReadAppSetting("CSS.CacheBustingVersion");
            string hostLoginCssFile = string.Format("Login_{0}.css?v={1}", ownerId, cacheBustingVersion);
            cssFileName = string.Format("{0}?v={1}", cssFileName, cacheBustingVersion);

            this.HostStyleSheet.Href = WebUtil.OwnerRelativeSharedFilePath(commonFolder, cssFileName);

            this.HostLoginStyleSheet.Href = WebUtil.OwnerRelativeSharedFilePath(commonFolder, hostLoginCssFile);

            return ownerId;
        }

        public void LoadOwnerContent(Owner owner)
        {
            if (owner != null)
            {
                LoadOwnerWelcomeMessage(owner);
                LoadOwnerContactInfo(owner);
            }
        }

        public void LoadOwnerWelcomeMessage(Owner owner)
        {
            if (!string.IsNullOrEmpty(owner.WelcomeMessageText))
            {
                this.ownerWelcomeMessage.Text = owner.WelcomeMessageText;
            }
        }

        public void LoadOwnerContactInfo(Owner owner)
        {
            DisplayContactInfo(owner.ContactInfoHtml);
        }

        public void DisplayContactInfo(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                ownerContactUsHtml.Text = content;
                lnkContactUs.Visible = false;
            }
            else
            {
                lnkContactUs.Visible = true;
            }
        }

        public int LoadHost(ExpoOrders.Web.WebUtil.HostEnum host)
        {
            return LoadHost(host.ToString());
        }

        public int LoadHost()
        {
            return LoadHost(WebUtil.CurrentHost());
        }

        public void AppendOnLoad(string onloadFunction)
        {
            this.LoginBody.Attributes["OnLoad"] += onloadFunction;
        }

        public void LoadShowInfo(Show showDetail)
        {
            if (showDetail != null && showDetail.ActiveFlag == true)
            {
                plcOwnerHeader.Visible = false;
                plcShowHeader.Visible = true;

                plcShowInfo.Visible = true;
                lblShowName.Text = showDetail.ShowName;
                lblShowDates.Text = showDetail.ShowDatesDisplay;

                if (!string.IsNullOrEmpty(showDetail.OrderReplyEmail))
                {
                    DisplayContactUsLink(showDetail.OrderReplyEmail);
                }
                else
                {
                    DisplayContactUsLink(null);
                }

                this.HostStyleSheet.Href = string.Format("~/Assets/Shows/{0}/style.css", showDetail.ShowGuid.ToString());
            }
            else
            {
                DisplayContactUsLink(null);
                plcShowInfo.Visible = false;
            }
        }

        private void DisplayContactUsLink(string emailTo)
        {
            plcContactUs.Visible = true;
            if (string.IsNullOrEmpty(emailTo))
            {
                if (!string.IsNullOrEmpty(_hostConfigContactUsEmail))
                {
                    emailTo = _hostConfigContactUsEmail;
                }
                else
                {
                    emailTo = ConfigurationManager.AppSettings["ExpoOrders.ContactEmail"];
                }
            }


            lnkContactUs.HRef = string.Format("mailto:{0}", emailTo);
            lnkContactUs.Visible = true;
            
        }

        public bool ManagerLoginEnabled
        {
            get { return plcManagerLogin.Visible; }
            set { plcManagerLogin.Visible = value;  }
        }
    }
}