using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoOrders.Controllers;
using System.Data;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class Sql : BaseSuperAdminPage
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
            Master.ClearErrors();
            lblRowCount.Visible = false;
            grdvSqlRows.Visible = false;
        }

        private void RunSql()
        {
            Master.ClearErrors();
            lblRowCount.Visible = true;
            grdvSqlRows.Visible = false;

            SuperAdminController cntlr = new SuperAdminController();
            DataTable tbl = cntlr.RunSql(CurrentUser, txtSql.Text.Trim());

            if (tbl != null && tbl.Rows.Count > 0)
            {
                lblRowCount.Text = string.Format("Displaying {0} rows.", tbl.Rows.Count);
                grdvSqlRows.DataSource = tbl;
                grdvSqlRows.DataBind();
                grdvSqlRows.Visible = true;
            }
            else
            {
                lblRowCount.Text = "No rows to display.";
            }
        }

        protected void btnRunSql_Click(object sender, EventArgs e)
        {
            try
            {
                RunSql();
            }
            catch (Exception ex)
            {
                Master.AddErrorMessage(ex);
            }
        }
    }
}