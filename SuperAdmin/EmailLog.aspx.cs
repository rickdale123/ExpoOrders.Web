using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class EmailLog : BaseSuperAdminPage
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
            plcEmailLog.Visible = false;
        }

        private void UpdateSelectedEmailStatus(string newEmailStatus)
        {
            List<int> emailIds = new List<int>();
            if (grdvEmailLog.Visible = true && grdvEmailLog.Rows.Count > 0)
            {
                foreach (GridViewRow row in grdvEmailLog.Rows)
                {
                    CheckBox chkSelected = (CheckBox)row.FindControl("chkSelected");
                    HiddenField hdnEmailId = (HiddenField)row.FindControl("hdnEmailId");

                    if (chkSelected.Checked)
                    {
                        emailIds.Add(Util.ConvertInt32(hdnEmailId.Value));
                    }
                }
            }

            if (emailIds.Count > 0)
            {
                SuperAdminController cntrl = new SuperAdminController();
                cntrl.UpdateEmailStatus(CurrentUser, emailIds, newEmailStatus);
            }
            
        }

        private void BindSearchResults(bool searchAll)
        {
            lblEmailLogRowCount.Text = "0 rows to display.<br/>";

            SearchCriteria searchCriteria = BuildSearchCriteria(searchAll);
            SuperAdminController cntrl = new SuperAdminController();

            List<Email> emails = cntrl.GetEmailLogs(searchCriteria).OrderByDescending(e => e.SendDate).ToList();

            if (emails != null && emails.Count > 0)
            {
                lblEmailLogRowCount.Text = string.Format("({0} rows displayed.)<br/>", emails.Count);
            }
            
            grdvEmailLog.DataSource = emails;
            grdvEmailLog.DataBind();

            plcEmailLog.Visible = true;
        }

        private SearchCriteria BuildSearchCriteria(bool searchAll)
        {
            SearchCriteria search = new SearchCriteria();
            if (!searchAll)
            {
                search.StartDate = txtStartDate.SelectedDate;
            }
            
            if (chkOnlyShowErrors.Checked)
            {
                search.StatusCode = "E";
            }

            return search;
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            BindSearchResults(false);
        }

        protected void btnShowAll_Click(object sender, EventArgs e)
        {
            BindSearchResults(true);
        }

        protected void btnUpdateAllStatus_Click(object sender, EventArgs e)
        {
            UpdateSelectedEmailStatus(ddlEmailStatus.SelectedItem.Value);
            BindSearchResults(false);
        }

        protected void grdvEmailLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lnkUpdateStatus = (LinkButton)e.Row.FindControl("lnkUpdateStatus");
                Email emailLog = (Email)e.Row.DataItem;

                TextBox txtEmailBody = (TextBox)e.Row.FindControl("txtEmailBody");
                txtEmailBody.Text = emailLog.Body;

                lnkUpdateStatus.CommandName = emailLog.EmailId.ToString();

                if (emailLog.StatusCode == "U")
                {
                    lnkUpdateStatus.CommandArgument = "S";
                    lnkUpdateStatus.Text = "Sent";
                }
                else
                {
                    lnkUpdateStatus.CommandArgument = "U";
                    lnkUpdateStatus.Text = "UnSent";
                }

                if (emailLog.StatusCode == "E")
                {
                    e.Row.BackColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void lnkUpdateStatus_Click(object sender, EventArgs e)
        {
            if (sender is LinkButton)
            {
                LinkButton lnk = (LinkButton)sender;

                int emailId = Util.ConvertInt32(lnk.CommandName);

                SuperAdminController cntrl = new SuperAdminController();
                cntrl.UpdateEmailStatus(CurrentUser, emailId, lnk.CommandArgument);
                
            }
        }
    }
}