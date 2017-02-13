using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;

namespace ExpoOrders.Web.Owners
{
    public partial class CallLog : BaseOwnerPage
    {

        private CallLogController _cntrl;
        private CallLogController Cntrl
        {
            get
            {
                if (_cntrl == null)
                {
                    _cntrl = new CallLogController();
                }
                return _cntrl;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            ltrExhibitorCompanyName.Text = string.Empty;
            rptrCallLog.Visible = false;
            int exhibitorId = Util.ConvertInt32(QueryStringValue("ExhibitorId"));
            if (exhibitorId > 0)
            {
                LoadCallLog(exhibitorId);
            }
            this.OwnerStyleSheet.Href = string.Format("~/Assets/Shows/{0}/style.css", CurrentUser.CurrentShow.ShowGuid);
            ResetAddControls();
        }

        private void ResetAddControls()
        {
            lblCurrentUserName.Text = CurrentUser.DisplayUserName;
            lblCurrentDate.Text = DateTime.Now.ToString();
            txtCallDetails.Text = string.Empty;

            ddlExhibitorUserId.Items.Clear();

            ddlExhibitorUserId.DataValueField = "UserId";
            ddlExhibitorUserId.DataTextField = "PersonName";
            ddlExhibitorUserId.DataSource = Cntrl.GetExhibitorById(CurrentExhibitorId).Users;
            ddlExhibitorUserId.DataBind();

            ddlExhibitorUserId.Items.Insert(0, new ListItem { Text = "-- Select One --", Value = "" });

        }

        private void LoadCallLog(int exhibitorId)
        {
            rptrCallLog.Visible = false; 

            CurrentExhibitorId = exhibitorId;

            lblNoLogs.Visible = false;

            SearchCriteria search = new SearchCriteria();
            search.ExhibitorId = exhibitorId;

            List<ExpoOrders.Entities.CallLog> callLogs = Cntrl.RetrieveCallLogs(CurrentUser, search).Where(l => l.ActiveFlag == true).OrderByDescending(l => l.LogDateTime).ToList();

            if (callLogs != null && callLogs.Count > 0)
            {
                if (callLogs[0].Exhibitor != null)
                {
                    ltrExhibitorCompanyName.Text = callLogs[0].Exhibitor.ExhibitorCompanyName;
                }
                
                rptrCallLog.Visible = true;
                rptrCallLog.DataSource = callLogs;
                rptrCallLog.DataBind();
            }
            else
            {
                lblNoLogs.Visible = true;
            }

            callLogs = null;

        }

        protected void rptrCallLog_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Literal ltrOwnerPersonName = (Literal)e.Item.FindControl("ltrOwnerPersonName");
                Literal ltrExhibitorPersonName = (Literal)e.Item.FindControl("ltrExhibitorPersonName");

                LinkButton lbtnDeleteCallLog = (LinkButton)e.Item.FindControl("lbtnDeleteCallLog");

                ExpoOrders.Entities.CallLog callLog = (ExpoOrders.Entities.CallLog)e.Item.DataItem;

                lbtnDeleteCallLog.Visible = false;
                if (UserHasRole(UserRoleEnum.ShowManagement) || UserHasRole(UserRoleEnum.ExhibitorManagement))
                {
                    lbtnDeleteCallLog.Visible = true;
                    lbtnDeleteCallLog.Attributes.Add("onClick", "return confirm('Sure you want to remove this call log?');");
                }

                if (callLog.OwnerUser != null)
                {
                    ltrOwnerPersonName.Text = callLog.OwnerUser.PersonName;
                }

                if (callLog.ExhibitorUser != null)
                {
                    ltrExhibitorPersonName.Text = callLog.ExhibitorUser.PersonName;
                }
            }
        }

        protected void rptrCallLog_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            int callLogId = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "DeleteCallLog":
                    Cntrl.DeleteCallLog(CurrentUser, callLogId);
                    LoadCallLog(CurrentExhibitorId);
                    break;
                default:
                    break;
            }
        }

        protected void btnShowAddCallLog_Click(object sender, EventArgs e)
        {
            ResetAddControls();
            this.MPE.Show();
        }

        protected void btnInsertCallLog_Click(object sender, EventArgs e)
        {
            ExpoOrders.Entities.CallLog newLog = new ExpoOrders.Entities.CallLog();
            newLog.ExhibitorId = CurrentExhibitorId;

            if (!string.IsNullOrEmpty(ddlExhibitorUserId.SelectedValue))
            {
                newLog.ExhibitorUserId = new Guid(ddlExhibitorUserId.SelectedValue);
            }
            newLog.CallDetails = txtCallDetails.Text.Trim();

            if (!string.IsNullOrEmpty(newLog.CallDetails))
            {
                Cntrl.InsertCallLog(CurrentUser, newLog);
            }

            ResetAddControls();

            LoadCallLog(CurrentExhibitorId);
        }


        protected void btnCancelPopup_Click(object sender, EventArgs e)
        {
            ResetAddControls();
            this.MPE.Hide();
            LoadCallLog(CurrentExhibitorId);
        }

        private int CurrentExhibitorId
        {
            get { return Util.ConvertInt32(hdnExhibitorId.Value); }
            set { hdnExhibitorId.Value = value.ToString(); }
        }
    }
}