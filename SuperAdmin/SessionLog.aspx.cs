using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Entities;
using ExpoOrders.Controllers;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class SessionLog : BaseSuperAdminPage
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
            txtStartDate.SelectedDate = DateTime.Now;
            plcSessionLogs.Visible = false;
        }

        private void BindSearchResults(SearchCriteria searchCriteria)
        {

            this.lblRowCount.Text = "0 rows to display.<br/>";

            LoginController cntrl = new LoginController();

            var sessions = cntrl.SearchSessionLogs(searchCriteria).OrderByDescending(l => l.SessionStartDate).ToList();
            if (sessions != null && sessions.Count > 0)
            {
                lblRowCount.Text = string.Format("({0} rows displayed.)<br/>", sessions.Count);
            }

            grdvSessionLog.DataSource = sessions;
            grdvSessionLog.DataBind();

            plcSessionLogs.Visible = true;
        }

        private SearchCriteria BuildSearchCriteria(bool showAll)
        {

            SearchCriteria search = new SearchCriteria();

            search.OnlyValidUsers = chkOnlyUserIds.Checked;

            if (!showAll)
            {
                search.StartDate = txtStartDate.SelectedDate;
            }

            return search;
        }


        protected void btnGo_Click(object sender, EventArgs e)
        {
            SearchCriteria search = BuildSearchCriteria(false);
           
            BindSearchResults(search);
        }

        protected void btnShowAll_Click(object sender, EventArgs e)
        {
            SearchCriteria search = BuildSearchCriteria(true);

            BindSearchResults(search);
        }

        protected void grdvSessionLogs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Literal ltrUserName = (Literal)e.Row.FindControl("ltrUserName");
                ExpoOrders.Entities.SessionLog log = (ExpoOrders.Entities.SessionLog)e.Row.DataItem;

                if (log.UserId.HasValue)
                {
                    if (log.aspnet_Users != null)
                    {
                        ltrUserName.Text = log.aspnet_Users.PersonName;
                    }
                }
                
            }
        }
    }
}