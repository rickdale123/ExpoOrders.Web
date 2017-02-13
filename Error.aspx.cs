using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using ExpoOrders.Common;

namespace ExpoOrders.Web
{
    public partial class Error : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPage();
            }

        }

        private void LoadPage()
        {
            object lastException = HttpContext.Current.Items["LastException"];

            if (lastException != null)
            {
                DisplayErrors((Exception)lastException);
            }
        }

        private void DisplayErrors(Exception lastError)
        {
            ltrErrorMessage.Text = lastError.Message;
            lblErrorMessage.Text = Util.AllExceptions(lastError);
        }

        protected override void OnInit(EventArgs e)
        {
            base.RequiresSsl = false;
            base.OnInit(e);
        }
    }
}