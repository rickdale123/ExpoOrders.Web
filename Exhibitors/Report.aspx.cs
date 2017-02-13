using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using Microsoft.Reporting.WebForms;

namespace ExpoOrders.Web.Exhibitors
{
    public partial class Report : BasePage
    {

        #region Manager Objects

        private ReportController _reportMgr = null;
        ReportController ReportMgr
        {
            get
            {
                if (_reportMgr == null)
                {
                    _reportMgr = new ReportController(ReportControllerType.Remote);
                }
                return _reportMgr;
            }
        }
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            if (Request["reportId"] != null)
            {
                int currentReportId = int.Parse(Request["reportId"].ToString());
                if (currentReportId > 0)
                {
                    ReportEnum currentReport = (ReportEnum)Enum.Parse(typeof(ReportEnum), currentReportId.ToString());

                    if (currentReport != ReportEnum.NotSet)
                    {
                        PrepareViewer();
                        PrepareReport(currentReport);
                    }
                }
            }
        }

        #endregion

        #region Prepare Report
        private void PrepareViewer()
        {
            this.viewReportViewer.Visible = false;
            this.viewReportViewer.ProcessingMode = ProcessingMode.Remote;
           
        }

        public void PrepareReport(ReportEnum currentReport)
        {

            //Set the current Reporting Services Report to Run
            ReportMgr.SetCurrentServerReport(this.viewReportViewer.ServerReport);

            //Initalize the report to prepare for rendering.
            ReportMgr.PrepareServerReport(currentReport);

            //Prepare the parameters that the reprot requires
            List<ReportParameter> reportParams = null;

            if (currentReport == ReportEnum.DirectShowSiteLabel ||
                currentReport == ReportEnum.AdvanceWarehouseLabel)
            {
                reportParams = GetParametersNoRequest();
            }
            else if (currentReport == ReportEnum.OutboundShippingLabel)
            {
                reportParams = GetLabelParameters();
            }
            else if (currentReport == ReportEnum.OrderReceipt || currentReport == ReportEnum.DeliveryReceipt)
            {
                reportParams = new List<ReportParameter>();
                reportParams.Add(new ReportParameter("ShowId", "0", true));
                reportParams.Add(new ReportParameter("ExhibitorId", "0", true));
                reportParams.Add(new ReportParameter("OrderId", ReadQueryString("orderId")));
            }
            else if (currentReport == ReportEnum.ExhibitorInvoice)
            {
                reportParams = new List<ReportParameter>();

                string exhibitorId = ReadQueryString("exhibitorId");

                if (!string.IsNullOrEmpty(exhibitorId))
                {
                    if (CurrentUser.CurrentExhibitor != null && !CurrentUser.CurrentExhibitor.IsPreviewMode)
                    {
                        if(CurrentUser.CurrentExhibitor.ExhibitorId.ToString() != exhibitorId)
                        {
                            throw new Exception(string.Format("Unauthorized Attempt to view Invoice {0} ({1}).", exhibitorId, CurrentUser.CurrentExhibitor.ExhibitorId.ToString()));
                        }
                    }
                }

                reportParams.Add(new ReportParameter("ShowId", ReadQueryString("showId"), true));
                reportParams.Add(new ReportParameter("ExhibitorId", exhibitorId, true));
            }
            else if (currentReport == ReportEnum.FormSubmission)
            {
                reportParams = new List<ReportParameter>();
                reportParams.Add(new ReportParameter("ShowId", "0", true));
                reportParams.Add(new ReportParameter("ExhibitorId", "0", true));
                reportParams.Add(new ReportParameter("FormName", "", true));
                reportParams.Add(new ReportParameter("OrderId", ReadQueryString("orderId")));
            }
            else if (currentReport == ReportEnum.InstallDismantleReport)
            {
                reportParams = new List<ReportParameter>();
                reportParams.Add(new ReportParameter("ShowId", CurrentUser.CurrentShow.ShowId.ToString(), true));
                reportParams.Add(new ReportParameter("ExhibitorId", "0", true));
                string boothNumber = null;
                reportParams.Add(new ReportParameter("BoothNumber", boothNumber, true));
                reportParams.Add(new ReportParameter("OrderItemId", ReadQueryString("orderItemId")));

                List<int> categoryIdFilter = new List<int>();
                List<int> productIdFilter = new List<int>();
                ProductController prodCntrl = new ProductController();
                List<Category> categories = prodCntrl.GetCategoryList(CurrentUser.CurrentShow.ShowId).Where(cat => cat.ActiveFlag == true).OrderBy(cat => cat.CategoryName).ToList();

                categories = categories.Where(c => c.Products.Count(p => p.InstallDismantleInd == "I" || p.InstallDismantleInd == "D") > 0).ToList();
                categories.ForEach(c =>
                {
                    categoryIdFilter.Add(c.CategoryId);
                });

                List<Product> products = prodCntrl.GetProductFilterList(CurrentUser, CurrentUser.CurrentShow.ShowId, categoryIdFilter);
                
                products = products.Where(p => p.InstallDismantleInd == "I" || p.InstallDismantleInd == "D").ToList();

                products.ForEach(p =>
                {
                    productIdFilter.Add(p.ProductId);
                });

                string[] instDistCategoryListFilter = ConvertStringArray(categoryIdFilter).ToArray();
                string[] instDistProductListFilter = ConvertStringArray(productIdFilter).ToArray();

                reportParams.Add(new ReportParameter("CategoryIdList", instDistCategoryListFilter, true));
                reportParams.Add(new ReportParameter("ProductIdList", instDistProductListFilter, true));

                reportParams.Add(new ReportParameter("SortColumn", "ExhibitorCompanyName", true));
            }

            //Apply the parameters if any to the report being executed by the Report Controller
            if (reportParams != null && reportParams.Count > 0)
            {
                ReportMgr.SetReportParameters(reportParams);
            }

            //Render the Report Back to the Output Stream
            DisplayPDF(currentReport, "PDF");
        }

        #endregion

        #region Parameters
        private List<ReportParameter> GetParametersNoRequest()
        {
            List<ReportParameter> reportParams = new List<ReportParameter>();

            ReportParameter showIdParam = new ReportParameter();
            showIdParam.Name = "ShowId";
            showIdParam.Values.Add(this.CurrentUser.CurrentShow.ShowId.ToString());
            reportParams.Add(showIdParam);

            ReportParameter exhibitorIdParam = new ReportParameter();
            exhibitorIdParam.Name = "ExhibitorId";
            exhibitorIdParam.Values.Add(this.CurrentUser.CurrentExhibitor.ExhibitorId.ToString());
            reportParams.Add(exhibitorIdParam);

            return reportParams;
        }


        private List<ReportParameter> GetLabelParameters()
        {
            List<ReportParameter> parameters = new List<ReportParameter>();

            ReportParameter companyNameParam = new ReportParameter("CompanyName", ReadQueryString("companyname"));
            parameters.Add(companyNameParam);

            ReportParameter street1Param = new ReportParameter("Street1", ReadQueryString("street1"));
            parameters.Add(street1Param);

            ReportParameter street2Param = new ReportParameter("Street2", ReadQueryString("street2"));
            parameters.Add(street2Param);

            ReportParameter street3Param = new ReportParameter("Street3", ReadQueryString("street3"));
            parameters.Add(street3Param);

            ReportParameter street4Param = new ReportParameter("Street4", ReadQueryString("street4"));
            parameters.Add(street4Param);

            ReportParameter street5Param = new ReportParameter("Street5", ReadQueryString("street4"));
            parameters.Add(street5Param);

            ReportParameter cityParam = new ReportParameter("City", ReadQueryString("city"));
            parameters.Add(cityParam);

            ReportParameter stateParam = new ReportParameter("State", ReadQueryString("state"));
            parameters.Add(stateParam);

            ReportParameter postalCodeParam = new ReportParameter("PostalCode", ReadQueryString("postalcode"));
            parameters.Add(postalCodeParam);

            ReportParameter countryParam = new ReportParameter("Country", ReadQueryString("country"));
            parameters.Add(countryParam);

            parameters.AddRange(GetParametersNoRequest());
            
            return parameters;
        }

        #endregion

        #region Render Report
        private void DisplayPDF(ReportEnum currentReport, string renderType)
        {
            byte[] bytes = ReportMgr.RenderReport(renderType);
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", string.Format("inline;filename={0}.pdf", currentReport.ToString()));
            Response.ContentType = "application/pdf";
            Response.BinaryWrite(bytes);
            Response.End();
        }

        #endregion

        private string ReadQueryString(string key)
        {
            object queryString = Request.QueryString[key];
            if (queryString != null)
            {
                return queryString.ToString();
            }
            else
            {
                return string.Empty;
            }

        }

        

    }
}