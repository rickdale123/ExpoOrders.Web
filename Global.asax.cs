using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using Mindscape.Raygun4Net;


namespace ExpoOrders.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            LoginController cntrl = new LoginController();
            cntrl.RefreshOwnerHostConfigs();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (Util.IsAppSettingEnabled("SessionLogging.Enabled"))
            {
                LoginController cntrl = new LoginController();
                SessionLog log = new SessionLog();
                log.SessionLogId = Guid.NewGuid();
                log.SessionStartDate = DateTime.Now;
                log.AspSessionId = Session.SessionID;
                log.MachineName = Environment.MachineName;
                log.UserAgent = Util.ConvertEmptyString(Request.ServerVariables["HTTP_USER_AGENT"]);
                log.ApplicationVersion = WebUtil.WebVersionNumber();
                log.SessionLogXml = WebUtil.RequestorBrowserInformation(Request);
                log.HostAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.ServerVariables["REMOTE_ADDR"];

                cntrl.LogSessionStart(log);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            if (Util.IsAppSettingEnabled("SessionLogging.Enabled"))
            {
                LoginController cntrl = new LoginController();
                cntrl.LogSessionEnd(Session.SessionID);
            }
        }


        private void BuildRequestorLog()
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception lastEx = Server.GetLastError();
            if (lastEx != null)
            {
                if (HttpContext.Current.IsCustomErrorEnabled)
                {
                    new RaygunClient().Send(lastEx);
                }

                string sessionId = "{Unknown}";
                if (Session != null)
                {
                    sessionId = Session.SessionID;
                }
                EventLogger.LogException(sessionId, Server.GetLastError());
                HttpContext.Current.Items.Add("LastException", lastEx);
                Server.ClearError();
                if (HttpContext.Current.IsCustomErrorEnabled)
                {
                    Server.Transfer("~/Error.aspx", true);
                }
            }
        }
        
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}