using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class HelpContent : BaseSuperAdminPage
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
            DisplayHelpList();
        }


        private void DisplayHelpList()
        {
            plcHelpDetail.Visible = false;
            plcHelpList.Visible = true;

            lblHelpListRowCount.Text = "0 rows to display.<br/>";

            List<Help> helpItems = Cntrl.GetHelpContentList();
            if (helpItems != null && helpItems.Count > 0)
            {
                lblHelpListRowCount.Text = string.Format("({0} rows displayed.)<br/>", helpItems.Count);
            }

            this.grdvHelpList.DataSource = helpItems.OrderBy(h => h.SortOrder);
            this.grdvHelpList.DataBind();
        }

        private void LoadHelpDetail(int helpId)
        {
            this.plcHelpList.Visible = false;
            this.plcHelpDetail.Visible = true;

            SelectedHelpId = helpId;

            if (helpId > 0)
            {
                Help helpDetail = Cntrl.GetHelpContent(helpId);

                if (helpDetail != null)
                {
                    SelectedHelpId = helpDetail.HelpId;

                    lblHelpId.Text = SelectedHelpId.ToString();

                    txtHelpTitle.Text = helpDetail.HelpTitle;
                    txtHelpCode.Text = helpDetail.HelpCode;
                    txtHelpSection.Text = helpDetail.HelpSection;
                    txtHelpText.Text = helpDetail.HelpText;
                    txtSortOrder.Text = helpDetail.SortOrder.ToString();
                    chkActiveFlag.Checked = helpDetail.ActiveFlag;
                }
            }
            else
            {
                lblHelpId.Text = string.Empty;
                txtHelpTitle.Text = string.Empty;
                txtHelpCode.Text = string.Empty;
                txtHelpSection.Text = string.Empty;
                txtHelpText.Text = string.Empty;
                txtSortOrder.Text = string.Empty;
                chkActiveFlag.Checked = true;
            }
        }

        private int SelectedHelpId
        {
            get
            {
                return Util.ConvertInt32(this.hdnHelpId.Value);
            }
            set
            {
                this.hdnHelpId.Value = value.ToString();
            }
        }

        protected void grdvHelpList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "EditHelp":
                    LoadHelpDetail(id);
                    break;
            }
        }

        protected void grdvHelpList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

            }
        }

        protected void btnAddHelpContent_Click(object sender, EventArgs e)
        {
            LoadHelpDetail(0);
        }

        protected void btnCancelSaveHelp_Click(Object sender, EventArgs e)
        {
            DisplayHelpList();
        }

        protected void btnSaveHelpDetail_Click(object sender, EventArgs e)
        {
            Help helpDetail = new Help();

            helpDetail.HelpId = SelectedHelpId;
            helpDetail.HelpSection = txtHelpSection.Text.Trim();
            helpDetail.HelpCode = txtHelpCode.Text.Trim();
            helpDetail.HelpTitle = txtHelpTitle.Text.Trim();
            helpDetail.HelpText = txtHelpText.Text.Trim();

            helpDetail.SortOrder = Util.ConvertInt32(txtSortOrder.Text.Trim());
            helpDetail.ActiveFlag = chkActiveFlag.Checked;

            ValidationResults errors = Cntrl.SaveHelpContent(CurrentUser, helpDetail);

            if (errors.IsValid)
            {
                DisplayHelpList();
                Master.DisplayFriendlyMessage("Help Saved.");
            }
            else
            {
                Master.AddErrorMessages(errors);
            }

        }

    }
}