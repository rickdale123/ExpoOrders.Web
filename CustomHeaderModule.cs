using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpoOrders.Web
{
    public class CustomHeaderModule : IHttpModule
    {
        public void Init(HttpApplication cxt)
        {
            cxt.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        public void Dispose()
        {

        }

        void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Set("Server", "ExpoOrders.com");
        }

    }
}