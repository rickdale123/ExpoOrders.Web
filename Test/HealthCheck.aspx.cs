using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using System.Drawing;
using ExpoOrders.Common;

namespace ExpoOrders.Web.Test
{
    public partial class HealthCheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RunAllHealthChecks();
            }
        }

        private void RunAllHealthChecks()
        {
            List<ValidationTestRun> testRuns = new List<ValidationTestRun>();
            SuperAdminController cntrl = new SuperAdminController();
            cntrl.BuildTestList().ForEach(s => {
                testRuns.Add(cntrl.RunTest(s));
            });

            this.grdvValidationTests.DataSource = testRuns;
            this.grdvValidationTests.DataBind();

        }


        protected void grdvValidationTests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lnkBtnRunTest = (LinkButton)e.Row.FindControl("lnkBtnRunTest");
                Label lblTestResult = (Label)e.Row.FindControl("lblTestResult");
                Label lblTestMessage = (Label)e.Row.FindControl("lblTestMessage");
                Label lblTestNotes = (Label)e.Row.FindControl("lblTestNotes");

                ValidationTestRun testRun = (ValidationTestRun) e.Row.DataItem;

                lnkBtnRunTest.Text = testRun.TestName;
                lnkBtnRunTest.CommandArgument = testRun.TestName;

                lblTestNotes.Text = testRun.Notes;

                if (testRun.Results.IsValid)
                {
                    lblTestResult.Text = "PASS";
                    lblTestResult.ForeColor = Color.Green;
                    lblTestMessage.Text = string.Empty;
                    
                }
                else
                {
                    lblTestResult.Text = "FAIL";
                    lblTestResult.ForeColor = Color.Red;
                    lblTestMessage.Text = Util.AllValidationErrors(testRun.Results);
                }
            }
        }

        protected void grdvValidationTests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RunTest")
            {
                string testName = e.CommandArgument.ToString();
            }
            
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            this.RunAllHealthChecks();
        }


    }
}