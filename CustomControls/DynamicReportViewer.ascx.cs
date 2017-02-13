#region Using Statements
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

using ExpoOrders.Entities;
using ExpoOrders.Common;
#endregion

namespace ExpoOrders.Web.CustomControls
{
    public partial class DynamicReportViewer : System.Web.UI.UserControl
    {
        #region Public Members

        public ExpoOrders.Entities.Report ExpoOrdersReport { get; set; }
        public bool Enabled { get; set; }
        public Unit Width { get; set; }
        public Unit Height { get; set; }
        public ProcessingMode ReportProcesinngMode { get; set; }
        public bool ShowCredentialPrompts { get; set; }
        public bool ShowBackButton { get; set; }
        public bool ShowParameterPrompt { get; set; }
        public bool ShowPromptAreaButton { get; set; }
        public bool ShowDocumentMapButton { get; set; }
        public bool ShowPageNavigationControls { get; set; }
        public string ReportName { get; set; }
        public List<ReportParameter> ReportParameters { get; set; }
        public string ReportServerUrl { get; set; }
        public string ReportPath { get; set; }
        public ReportServerCredentials ReportCredentials {get; set;}
        public string ExportFormat { get; set; }
        public bool ShowExportControls { get; set; }
        public bool ShowFindControls { get; set; }
        public bool ShowToolbar { get; set; }

        #endregion

        public DynamicReportViewer()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeReportViewer();
        }

        #region Viewer Initialization

        public void InitializeReportViewer()
        {
           
            rptViewer.Width = Width;
            rptViewer.Height = Height;
            rptViewer.ShowToolBar = ShowToolbar;
            rptViewer.ShowExportControls = ShowExportControls;
            rptViewer.ShowFindControls = ShowFindControls;
            rptViewer.ShowCredentialPrompts = ShowCredentialPrompts;
            rptViewer.ShowBackButton = ShowBackButton;
            rptViewer.ProcessingMode = ReportProcesinngMode;
            rptViewer.ShowParameterPrompts = ShowParameterPrompt;
            rptViewer.ShowPromptAreaButton = ShowPromptAreaButton;
            rptViewer.ShowDocumentMapButton = ShowDocumentMapButton;
            rptViewer.ShowPageNavigationControls = ShowPageNavigationControls;

            ServerReport currentReport = rptViewer.ServerReport;
            currentReport.ReportServerUrl = new Uri(ReportServerUrl);
            currentReport.ReportPath = ReportPath;
            currentReport.ReportServerCredentials = ReportCredentials;
            currentReport.SetParameters(ReportParameters);
            currentReport.Render(ExportFormat);
        }

        #endregion

       
    }
}