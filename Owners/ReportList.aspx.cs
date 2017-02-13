using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using ExpoOrders.Web.Owners.Common;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using AjaxControlToolkit;
using System.Text;

namespace ExpoOrders.Web.Owners
{
    public partial class ReportList : BaseOwnerPage
    {
        #region ManagerObjects

        private OwnerAdminController _ownerCtrl = null;
        public OwnerAdminController OwnerCtrl
        {
            get
            {
                if (_ownerCtrl == null)
                {
                    _ownerCtrl = new OwnerAdminController();
                }

                return _ownerCtrl;
            }
        }

        #endregion

        #region Private Members

        private List<NavigationLink> _pageNavigationLinks;
        AppSettingsReader _appSettingsReader = new AppSettingsReader();

        #endregion

        #region Public Members

        private OwnerAdminController _adminMgr = null;
        public OwnerAdminController AdminMgr
        {
            get
            {
                if (_adminMgr == null)
                {
                    _adminMgr = new OwnerAdminController();
                }

                return _adminMgr;
            }
        }
        private ReportController _reportCtrlr = null;
        public ReportController ReportCtrlr
        {
            get
            {
                if (_reportCtrlr == null)
                {
                    _reportCtrlr = new ReportController(ReportControllerType.Remote);
                }

                return _reportCtrlr;
            }
        }

        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.NavigationItemCallBack = this.HandleNavigationItemClicked;
            this.Master.PreviewShowCallBack = this.PreviewShow;

            bool loadGenericOwnerTabs = false;

            if (Request["combined"] != null && Request["combined"] == "1")
            {
                loadGenericOwnerTabs = true;
                CurrentUser.CurrentShow = null;
                _pageNavigationLinks = OwnerUtil.BuildCombinedReportsSubNavLinks();
            }
            else
            {
                _pageNavigationLinks = OwnerUtil.BuildReportsSubNavLinks();
            }
            
            if (!Page.IsPostBack)
            {
                LoadPage(loadGenericOwnerTabs);
            }
        }

        private void LoadPage(bool loadGenericOwnerTabs)
        {
            btnViewRawData.Visible = false;
            this.PageErrors.Visible = false;

            plcSearchCriteria.Visible = false;
            if (loadGenericOwnerTabs)
            {
                this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, OwnerPage.Reports, OwnerTabEnum.CombinedReports);
            }
            else
            {
                this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, OwnerPage.Reports, OwnerTabEnum.Reports);
            }
            
            this.Master.LoadSubNavigation("Reports", _pageNavigationLinks);
        }

        private void LoadPageMode(ReportEnum report)
        {
            NavigationLink linkToSelect = _pageNavigationLinks.SingleOrDefault<NavigationLink>(l => l.TargetId == (int)report);

            LoadPageMode(linkToSelect.NavigationLinkId, linkToSelect.TargetId.Value);
        }

        private void LoadPageMode(int navLinkId, int targetId)
        {
            btnViewRawData.Visible = false;

            plcSearchCriteria.Visible = true;
            if (navLinkId <= 0)
            {
                navLinkId = 1;
                targetId = (int)ReportEnum.NotSet;
                this.Master.SelectNavigationItem(navLinkId);
            }

            ReportEnum currentReport = (ReportEnum)Enum.Parse(typeof(ReportEnum), targetId.ToString(), true);
            hdnPageMode.Value = targetId.ToString();

            plcGenericExhibitorCriteria.Visible =
            plcOrderSummaryCriteria.Visible =
            plcOrderReceiptCriteria.Visible =
            plcExhibitorListCriteria.Visible =
            plcBillingSummaryCriteria.Visible =
            plcDeliveryReport.Visible =
            plcPriceListReport.Visible =
            plcViewReportButton.Visible =
            plcTransactionTypeFilters.Visible =
            plcBoothNumberFilter.Visible =
            plcInstallDismantleFilters.Visible =
            plcStartEndDates.Visible =
            plcSuccessFilter.Visible =
            plcActiveFilter.Visible =
            plcExhibitorClassification.Visible =
            plcOutboundAddressFilter.Visible = false;

            plcMultiCategorySortBy.Visible = false;

            plcMultipleProductFilter.Visible = plcMultipleCategoryFilter.Visible = false;

            rdoEnableMultiCategoryFilter.ClearSelection();
            rdoEnableMultiCategoryFilter.Items[0].Selected = true;

            rdoEnableMultiProductFilter.ClearSelection();
            rdoEnableMultiProductFilter.Items[0].Selected = true;

            plcReportViewer.Visible = rptViewer.Visible = false;

            rblOrderReceiptCriteriaChoice.ClearSelection();

            hdnCurrentReportId.Value = ((int)currentReport).ToString();

            txtStartDate.Text = txtEndDate.Text = string.Empty;

            switch (currentReport)
            {
                case ReportEnum.DeliveryReport:
                case ReportEnum.InventoryReport:
                case ReportEnum.FormSubmission:
                case ReportEnum.OutboundShippingLabelList:
                case ReportEnum.OutboundShippingLabelList2:
                    btnViewRawData.Visible = true;
                    break;
                default:
                    btnViewRawData.Visible = false;
                    break;
            }
           
            if (currentReport == ReportEnum.CombinedShowReceipts)
            {
                ltrReportNote.Text = GetHelpText("TotalReceiptsReport");
            }
            
            switch (currentReport)
            {
                case ReportEnum.NotSet:
                    break;
                case ReportEnum.ExhibitorList:
                    plcExhibitorListCriteria.Visible = true;
                    DisplayBoothNumberFilter();
                    DisplayGenericViewReportButton();
                    break;
                case ReportEnum.BillingSummary:
                    plcBillingSummaryCriteria.Visible = true;
                    LoadExhibitorList(cboBillingSummaryExhibitorId);
                    DisplayBoothNumberFilter();
                    DisplayGenericViewReportButton();
                    break;
                case ReportEnum.OrderSummaryByCategory:
                    LoadCategoryList(cboCategoryId, cboProduct);
                    plcOrderSummaryCriteria.Visible = true;
                    DisplayBoothNumberFilter();
                    DisplayGenericViewReportButton();
                    break;
                case ReportEnum.InventoryReport:
                    InitializeMultiCategoryFilters();
                    DisplayBoothNumberFilter();
                    DisplayGenericViewReportButton();
                    ltrMultiCategorySortByLabel.Text = "Sort by Category, Product then ";
                    plcMultiCategorySortBy.Visible = true;
                    plcStartEndDates.Visible = true;
                    lblStartEndDateLabel.Text = "Order Date: ";
                    break;
                case ReportEnum.InventorySummary:
                    InitializeMultiCategoryFilters();
                    DisplayBoothNumberFilter();
                    DisplayGenericViewReportButton();
                    plcMultiCategorySortBy.Visible = false;
                    break;
                case ReportEnum.OrderReceipt:
                case ReportEnum.DeliveryReceipt:
                case ReportEnum.FormSubmission:
                    plcOrderReceiptCriteria.Visible = true;
                    LoadFormNameSelections();
                    DisplayBoothNumberFilter();
                    break;
                case ReportEnum.GeneralLedger:
                    DisplayGenericViewReportButton();
                    break;
                case ReportEnum.DeliveryReport:
                    plcDeliveryReport.Visible = true;
                    LoadExhibitorList(cboDelRptExhibitorId);
                    InitializeMultiCategoryFilters();
                    DisplayBoothNumberFilter();
                    DisplayGenericViewReportButton();
                    plcStartEndDates.Visible = true;
                    lblStartEndDateLabel.Text = "Order Date: ";
                    break;
                case ReportEnum.ExhibitorInvoice:
                case ReportEnum.OrderSummaryByExhibitor:
                case ReportEnum.UserLoginReport:
                case ReportEnum.ExhibitorUsers:
                case ReportEnum.ExhibitorCreditCards:
                case ReportEnum.OutboundShippingLabelList:
                case ReportEnum.OutboundShippingLabelList2:
                    DisplayExhibitorDropDownFilter();
                    DisplayBoothNumberFilter();
                    DisplayGenericViewReportButton();
                    break;
                case ReportEnum.ContactList:
                    DisplayExhibitorDropDownFilter();
                    DisplayBoothNumberFilter();
                    DisplayGenericViewReportButton();
                    DisplayClassificationFilter();
                    break;
                case ReportEnum.Receivables:
                    DisplayGenericViewReportButton();
                    break;
                case ReportEnum.PriceList:
                    plcPriceListReport.Visible = true;
                    LoadCategoryList(cboPriceListRptCategory, cboPriceListRptProduct);
                    DisplayGenericViewReportButton();
                    break;
                case ReportEnum.CreditCardPayments:
                    DisplayExhibitorDropDownFilter();
                    plcTransactionTypeFilters.Visible = true;
                    DisplayBoothNumberFilter();
                    DisplayGenericViewReportButton();
                    plcStartEndDates.Visible = true;
                    lblStartEndDateLabel.Text = "Transaction Date: ";
                    break;
                case ReportEnum.CheckPayments:
                case ReportEnum.WirePayments:
                    DisplayExhibitorDropDownFilter();
                    DisplayBoothNumberFilter();
                    DisplayGenericViewReportButton();
                    break;
                case ReportEnum.InstallDismantleReport:
                    InitializeMultiCategoryFilters();
                    plcInstallDismantleFilters.Visible = true;
                    LoadExhibitorList(ddlExhibitor2);
                    LoadExhibitorOrderItems();
                    DisplayBoothNumberFilter();
                    DisplayGenericViewReportButton();
                    break;
                case ReportEnum.CombinedShowReceipts:
                    DisplayGenericViewReportButton();
                    plcStartEndDates.Visible = true;
                    DefaultOwnerDates(CurrentUser.CurrentOwner);
                    lblStartEndDateLabel.Text = "Date: ";
                    break;
                case ReportEnum.Transactions:
                    DisplayExhibitorDropDownFilter();
                    plcSuccessFilter.Visible = true;
                    plcActiveFilter.Visible = true;
                    DisplayGenericViewReportButton();
                    plcStartEndDates.Visible = true;
                    lblStartEndDateLabel.Text = "Transaction Date: ";
                    break;
                case ReportEnum.BadLineItemReport:
                    DisplayGenericViewReportButton();
                    break;
                default:
                    DisplayGenericViewReportButton();
                    break;
            }

            if (currentReport == ReportEnum.OutboundShippingLabelList || currentReport == ReportEnum.OutboundShippingLabelList2)
            {
                DisplayOutboundAddressFilter();
            }

        }

        private void DisplayOutboundAddressFilter()
        {
            plcOutboundAddressFilter.Visible = true;
            ddlOutboundAddressFilter.SelectedValue = "0";
        }

        private void DisplayBoothNumberFilter()
        {
            plcBoothNumberFilter.Visible = cboBoothNumber.Visible = true;

            cboBoothNumber.Items.Clear();

            cboBoothNumber.DataSource = OwnerCtrl.GetExhibitorBoothNumberFilter(this.CurrentUser.CurrentShow.ShowId, false);
            cboBoothNumber.DataTextField = "BoothNumber";
            cboBoothNumber.DataValueField = "BoothNumber";
            cboBoothNumber.DataBind();
            cboBoothNumber.Items.Insert(0, new ListItem { Text = "-- All --", Value = "" });
            cboBoothNumber.SelectedIndex = 0;
        }

        private void InitializeMultiCategoryFilters()
        {
            plcMultipleCategoryFilter.Visible = true;
            rdoEnableMultiCategoryFilter.Items[0].Selected = true;
            chkLstCategoryIdFilter.Visible = false;

            plcMultipleProductFilter.Visible = false;
            rdoEnableMultiProductFilter.Items[0].Selected = true;
            chkLstProductIdFilter.Visible = false;
        }

        private void LoadMultipleCategoryList()
        {
            chkLstCategoryIdFilter.Items.Clear();

            ProductController prodCntrl = new ProductController();

            List<Category> categories = prodCntrl.GetCategoryList(CurrentUser.CurrentShow.ShowId).Where(cat => cat.ActiveFlag == true).OrderBy(cat => cat.SortOrder).ThenBy(c => c.CategoryName).ToList();

            if (ParseCurrentReport() == ReportEnum.InstallDismantleReport)
            {
                categories = categories.Where(c => c.Products.Count(p=> p.InstallDismantleInd == "I" || p.InstallDismantleInd == "D") > 0).ToList();
            }
            chkLstCategoryIdFilter.DataSource = categories;
            chkLstCategoryIdFilter.DataTextField = "CategoryName";
            chkLstCategoryIdFilter.DataValueField = "CategoryId";
            chkLstCategoryIdFilter.DataBind();

            chkLstCategoryIdFilter.Visible = true;
        }

        private void LoadMultipleProductList()
        {
            chkLstProductIdFilter.Items.Clear();

            ProductController prodCntrl = new ProductController();

            List<Product> products = prodCntrl.GetProductFilterList(CurrentUser, CurrentUser.CurrentShow.ShowId, SelectedMultipleCategories());

            if (ParseCurrentReport() == ReportEnum.InstallDismantleReport)
            {
                products = products.Where(p => p.InstallDismantleInd == "I" || p.InstallDismantleInd == "D").ToList();
            }
            chkLstProductIdFilter.DataSource = products;
            chkLstProductIdFilter.DataTextField = "ProductName";
            chkLstProductIdFilter.DataValueField = "ProductId";
            chkLstProductIdFilter.DataBind();

            chkLstProductIdFilter.Visible = true;
        }

        private void LoadExhibitorOrderItems()
        {
            ddlExhibitorOrderItem.Items.Clear();

            ProductController prodCntrl = new ProductController();

            int selectedExhibitorId = Util.ConvertInt32(ddlExhibitor2.SelectedValue);

            List<OrderItem> orderItems = prodCntrl.GetExhibitorOrderItemFilters(CurrentUser, CurrentUser.CurrentShow.ShowId, selectedExhibitorId);
            ddlExhibitorOrderItem.DataSource = orderItems;
            ddlExhibitorOrderItem.DataTextField = "ItemDescription";
            ddlExhibitorOrderItem.DataValueField = "OrderItemId";
            ddlExhibitorOrderItem.DataBind();

            ddlExhibitorOrderItem.Visible = true;

            ddlExhibitorOrderItem.Items.Insert(0, new ListItem("-- All --", string.Empty));
        }

        protected void rdoEnableMultiCategoryFilter_Changed(object sender, EventArgs e)
        {
            if (((RadioButtonList)sender).Items[0].Selected)
            {
                chkLstCategoryIdFilter.Visible = false;
                chkLstProductIdFilter.Visible = false;
                chkLstProductIdFilter.Items.Clear();
                plcMultipleProductFilter.Visible = false;
            }
            else
            {
                LoadMultipleCategoryList();
            }
        }

        protected void rdoEnableMultiProductFilter_Changed(object sender, EventArgs e)
        {
            if (((RadioButtonList)sender).Items[0].Selected)
            {
                chkLstProductIdFilter.Visible = false;
            }
            else
            {
                LoadMultipleProductList();
            }
        }
        
        protected void chkLstCategoryIdFilter_IndexChanged(object sender, EventArgs e)
        {
            List<int> selectedCategories = SelectedMultipleCategories();

            if (selectedCategories.Count > 0)
            {
                plcMultipleProductFilter.Visible = true;

                if (!rdoEnableMultiProductFilter.Items[0].Selected)
                {
                    LoadMultipleProductList();
                }
            }
            else
            {
                plcMultipleProductFilter.Visible = false;
            }
        }

        private List<int> SelectedMultipleCategories()
        {
            List<int> selectedCategories = new List<int>();

            foreach (ListItem li in chkLstCategoryIdFilter.Items)
            {
                if (li.Selected)
                {
                    selectedCategories.Add(Util.ConvertInt32(li.Value));
                }
            }
            return selectedCategories; 
        }

        private void DisplayClassificationFilter()
        {
            this.plcExhibitorClassification.Visible = cboExhibitorClassification.Visible = false;

            this.cboExhibitorClassification.Items.Clear();
            this.cboExhibitorClassification.SelectedValue = string.Empty;

            if (!string.IsNullOrEmpty(CurrentUser.CurrentOwner.ClassificationList))
            {
                bool showingClassification = false;
                foreach (string item in Util.ParseDelimitedList(CurrentUser.CurrentOwner.ClassificationList, ';'))
                {
                    var itemVal = item.Trim();
                    if (!string.IsNullOrEmpty(itemVal))
                    {
                        cboExhibitorClassification.Items.Insert(0, new ListItem { Text = itemVal, Value = itemVal });
                        showingClassification = true;
                    }
                }

                if (showingClassification)
                {
                    plcExhibitorClassification.Visible = true;
                    cboExhibitorClassification.Visible = true;
                    cboExhibitorClassification.Items.Insert(0, new ListItem { Text = "-- All --", Value = "" });
                    cboExhibitorClassification.SelectedValue = string.Empty;
                }
            }
        }

        private void DisplayExhibitorDropDownFilter()
        {
            LoadExhibitorList(cboExhibitorId);
            plcGenericExhibitorCriteria.Visible = true;
        }

        private void DisplayGenericViewReportButton()
        {
            plcViewReportButton.Visible = true;
        }

        private void LoadCategoryList(DropDownList ddlCat, DropDownList ddlProd)
        {
            if (ddlCat != null)
            {
                ddlCat.ClearSelection();
                LoadProductList(0, ddlProd);

                ProductController prodCntrl = new ProductController();
                
                List<Category> categories = prodCntrl.GetCategoryList(CurrentUser.CurrentShow.ShowId).Where(cat => cat.ActiveFlag == true).OrderBy(cat => cat.SortOrder).ThenBy(cat => cat.CategoryName).ToList();
                ddlCat.DataSource = categories;
                ddlCat.DataTextField = "CategoryName";
                ddlCat.DataValueField = "CategoryId";
                ddlCat.DataBind();
                ddlCat.Items.Insert(0, new ListItem { Text = "-- All --", Value = "0" });
                ddlCat.SelectedIndex = 0;
            }
        }

        private void LoadProductList(int categoryId, DropDownList ddl)
        {
            ddl.Items.Clear();

            if (categoryId > 0)
            {
                ProductController prodCntrl = new ProductController();

                Category category = prodCntrl.GetCategory(categoryId);
                if (category != null)
                {
                    ddl.DataSource = category.Products.Where(prod => prod.ActiveFlag == true && prod.ProductTypeCd == "Item").OrderBy(prod => prod.SortOrder).ThenBy(prod => prod.ProductName).ToList();
                    ddl.DataTextField = "ProductName";
                    ddl.DataValueField = "ProductId";
                    ddl.DataBind();
                }
            }

            ddl.Items.Insert(0, new ListItem { Text = "-- All --", Value = "0" });
            ddl.SelectedIndex = 0;
        }

        private void DefaultOwnerDates(Owner owner)
        {
            bool useStandardDefaults = true;
            if (owner != null)
            {
                List<DateTime> minMaxDates = this.OwnerCtrl.FindDefaultOwnerTransactionDates(CurrentUser.CurrentOwner);
                if (minMaxDates != null && minMaxDates.Count > 1)
                {
                    useStandardDefaults = false;
                    txtStartDate.Text = minMaxDates[0].ToShortDateString();
                    txtEndDate.Text = minMaxDates[1].ToShortDateString();
                }
            }

            if(useStandardDefaults)
            {
                txtStartDate.Text = DateTime.Now.AddYears(-1).ToShortDateString();
                txtEndDate.Text = DateTime.Now.ToShortDateString();
            }
        }

        private void LoadExhibitorList(DropDownList ddl)
        {
            if (ddl != null)
            {
                ddl.DataSource = OwnerCtrl.GetExhibitors(this.CurrentUser.CurrentShow.ShowId, false);
                ddl.DataTextField = "ExhibitorCompanyName";
                ddl.DataValueField = "ExhibitorId";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem { Text = "-- All --", Value = "0" });
                ddl.SelectedIndex = 0;
            }
        }

        private void LoadOrderIdList(DropDownList ddl)
        {
            if (ddl != null)
            {
                ReportEnum currentReport = ParseCurrentReport();

                OrderTypeEnum orderTypeFilter = OrderTypeEnum.NotSet;

                if (currentReport == ReportEnum.FormSubmission)
                {
                    orderTypeFilter = OrderTypeEnum.Form;
                }
                else
                {
                    orderTypeFilter = OrderTypeEnum.BoothOrder;
                }
                OrderAdminController orderCtrl = new OrderAdminController();
                ddl.DataSource = orderCtrl.GetAllOrdersForShow(CurrentUser.CurrentShow.ShowId, orderTypeFilter);
                ddl.DataTextField = "OrderId";
                ddl.DataValueField = "OrderId";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem { Text = "-- All --", Value = "0" });
                ddl.SelectedIndex = 0;
            }
        }

        private void LoadFormNames(DropDownList ddl)
        {
            if (ddl != null)
            {
                OrderAdminController orderCtrl = new OrderAdminController();
                var formList = orderCtrl.GetOrderFormNames(CurrentUser.CurrentShow.ShowId);
                ddl.DataSource = formList;

                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem { Text = "-- All --", Value = "" });
                ddl.SelectedIndex = 0;
            }
        }

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            this.LoadPageMode(navLinkId, targetId);
        }

        #endregion

        #region Methods

        private void PrepareViewer(ReportEnum report, List<ReportParameter> reportParams, bool rawDataReport)
        {
            string reportName = report.GetCodeValue();

            if (rawDataReport)
            {
                reportName = string.Format("{0}RawData", reportName);
            }

            AppSettingsReader appSettings = new AppSettingsReader();
            string reportServerUrl = _appSettingsReader.GetValue("ReportServerUrl", typeof(string)).ToString();
            string reportServerUsername = _appSettingsReader.GetValue("ReportServerUserName", typeof(string)).ToString();
            string reportServerPassword = _appSettingsReader.GetValue("ReportServerPassword", typeof(string)).ToString();
            string reportServerDomain = _appSettingsReader.GetValue("ReportServerDomain", typeof(string)).ToString();
            string reportServerPath = _appSettingsReader.GetValue("ReportPath", typeof(string)).ToString();
            ReportServerCredentials reportCredentials = new ReportServerCredentials(reportServerUsername, reportServerPassword, reportServerDomain);

            rptViewer.Reset();
            plcReportViewer.Visible = rptViewer.Visible = false;
            rptViewer.ShowParameterPrompts = false;
            rptViewer.ProcessingMode = ProcessingMode.Remote;
            rptViewer.ServerReport.ReportServerUrl = new Uri(reportServerUrl);
            rptViewer.ServerReport.ReportPath = string.Format(@"{0}{1}", reportServerPath, reportName);
            rptViewer.ServerReport.ReportServerCredentials = reportCredentials;
            if (reportParams != null && reportParams.Count > 0)
            {
                rptViewer.ServerReport.SetParameters(reportParams);
            }
            plcReportViewer.Visible = rptViewer.Visible = true;
        }

        private ReportEnum ParseCurrentReport()
        {
            ReportEnum currentReport = ReportEnum.NotSet;
            if (!string.IsNullOrEmpty(hdnCurrentReportId.Value))
            {
                currentReport = (ReportEnum)Enum.Parse(typeof(ReportEnum), hdnCurrentReportId.Value, true);
            }
            return currentReport;
        }

        #endregion

        #region Control Events

        #region Exhibitor Related Reports

        private string GetSortExhibitorListSortColumn()
        {
            return rblExhibitorListSortChoice.SelectedIndex == 0 ? "ExhibitorCompanyName" : "BoothNumber";
        }

        private string GetDeliveryReportSortColumn()
        {
            return rdoBtnDelRptSortBy.SelectedIndex == 0 ? "ExhibitorCompanyName" : "BoothNumber";
        }

        private string GetMultiCategorySortColumn()
        {
            return this.rdoBtnMultiCategorySortyBy.SelectedIndex == 0 ? "ExhibitorCompanyName" : "BoothNumber";
        }

        private string GetInstallDismantleSortColumn()
        {
            return this.rblInstallDismantleSort.SelectedIndex == 0 ? "ExhibitorCompanyName" : "BoothNumber";
        }

        #endregion

        #region Inventory Related Reports

        protected void btnViewRawData_OnClick(object sender, EventArgs e)
        {
            RunReport(true);
        }

        protected void btnViewReport_OnClick(object sender, EventArgs e)
        {
            RunReport(false);
        }

        private void RunReport(bool rawDataReport)
        {
            try
            {
                List<ReportParameter> reportParams = new List<ReportParameter>();

                ReportEnum report = ParseCurrentReport();

                if (report != ReportEnum.CombinedShowReceipts)
                {
                    reportParams.Add(new ReportParameter("ShowId", this.CurrentUser.CurrentShow.ShowId.ToString(), false));
                }

                if (report == ReportEnum.CombinedShowReceipts)
                {
                    reportParams.Add(new ReportParameter("OwnerId", this.CurrentUser.CurrentOwner.OwnerId.ToString(), false));
                    string[] allowableSubfirmList = ConvertStringArray(BuildAllowableShowList(CurrentUser)).ToArray();
                    reportParams.Add(new ReportParameter("AllowableShowIds", allowableSubfirmList, false));
                }

                switch (report)
                {
                    case ReportEnum.PriceList:
                        GetSelectedItem(reportParams, "CategoryId", this.cboPriceListRptCategory);
                        GetSelectedItem(reportParams, "ProductId", this.cboPriceListRptProduct);
                        break;
                    case ReportEnum.BillingSummary:
                        GetSelectedItem(reportParams, "ExhibitorId", cboBillingSummaryExhibitorId);

                        reportParams.Add(new ReportParameter("OrderBy", rdoBillingSummarySort.SelectedValue, true));
                        break;
                    case ReportEnum.OrderReceipt:
                    case ReportEnum.DeliveryReceipt:
                    case ReportEnum.FormSubmission:
                        if (rblOrderReceiptCriteriaChoice.SelectedValue == "0")
                        {
                            //OrderId = 0
                            GetSelectedItem(reportParams, "ExhibitorId", cboOrderReceiptExhibitorId);
                            reportParams.Add(new ReportParameter("OrderId", "0", true));
                        }
                        else if (rblOrderReceiptCriteriaChoice.SelectedValue == "1")
                        {
                            GetSelectedOrders(reportParams, cboOrderId);
                            reportParams.Add(new ReportParameter("ExhibitorId", "0", true));
                        }

                        if (report == ReportEnum.FormSubmission)
                        {
                            GetSelectedFormName(reportParams, cboFormName);
                        }
                        break;
                    case ReportEnum.OrderSummaryByCategory:
                        GetSelectedItem(reportParams, "CategoryId", cboCategoryId);
                        GetSelectedItem(reportParams, "ProductId", cboProduct);
                        break;
                    case ReportEnum.ExhibitorList:
                        reportParams.Add(new ReportParameter("SortColumn", GetSortExhibitorListSortColumn(), false));
                        break;
                    case ReportEnum.DeliveryReport:
                        GetSelectedItem(reportParams, "ExhibitorId", cboDelRptExhibitorId);
                        List<int> categoryIdList = BuildMultiCategoryFilterParams();
                        string[] categoryListFilter = ConvertStringArray(categoryIdList).ToArray();
                        string[] productListFilter = ConvertStringArray(BuildMultiProductFilterParams(categoryIdList)).ToArray();

                        reportParams.Add(new ReportParameter("CategoryIdList", categoryListFilter, true));
                        reportParams.Add(new ReportParameter("ProductIdList", productListFilter, true));
                        reportParams.Add(new ReportParameter("OrderBy", GetDeliveryReportSortColumn(), true));

                        break;
                    case ReportEnum.ExhibitorInvoice:
                    case ReportEnum.OrderSummaryByExhibitor:
                    case ReportEnum.UserLoginReport:
                    case ReportEnum.ExhibitorUsers:
                    case ReportEnum.ExhibitorCreditCards:
                    case ReportEnum.OutboundShippingLabelList:
                    case ReportEnum.OutboundShippingLabelList2:
                        GetSelectedItem(reportParams, "ExhibitorId", cboExhibitorId);
                        break;
                    case ReportEnum.ContactList:
                        GetSelectedItem(reportParams, "ExhibitorId", cboExhibitorId);
                        GetSelectedClassification(reportParams);
                        break;
                    case ReportEnum.CreditCardPayments:
                        GetSelectedItem(reportParams, "ExhibitorId", cboExhibitorId);
                        reportParams.Add(new ReportParameter("TransactionTypeCdList", BuildTransationTypeFilter(), true));

                        break;
                    case ReportEnum.CheckPayments:
                    case ReportEnum.WirePayments:
                        GetSelectedItem(reportParams, "ExhibitorId", cboExhibitorId);
                        break;
                    case ReportEnum.InventoryReport:
                    case ReportEnum.InventorySummary:
                        List<int> invCategoryIdList = BuildMultiCategoryFilterParams();
                        string[] invCategoryListFilter = ConvertStringArray(invCategoryIdList).ToArray();
                        string[] invProductListFilter = ConvertStringArray(BuildMultiProductFilterParams(invCategoryIdList)).ToArray();

                        reportParams.Add(new ReportParameter("CategoryIdList", invCategoryListFilter, true));
                        reportParams.Add(new ReportParameter("ProductIdList", invProductListFilter, true));
                        if (plcMultiCategorySortBy.Visible)
                        {
                            reportParams.Add(new ReportParameter("SortColumn", GetMultiCategorySortColumn(), true));
                        }

                        break;
                    case ReportEnum.InstallDismantleReport:

                        GetSelectedItem(reportParams, "ExhibitorId", ddlExhibitor2);
                        GetSelectedItem(reportParams, "OrderItemId", ddlExhibitorOrderItem);

                        List<int> instDistCategoryIdList = BuildMultiCategoryFilterParams();
                        string[] instDistCategoryListFilter = ConvertStringArray(instDistCategoryIdList).ToArray();
                        string[] instDistProductListFilter = ConvertStringArray(BuildMultiProductFilterParams(instDistCategoryIdList)).ToArray();

                        reportParams.Add(new ReportParameter("CategoryIdList", instDistCategoryListFilter, true));
                        reportParams.Add(new ReportParameter("ProductIdList", instDistProductListFilter, true));

                        reportParams.Add(new ReportParameter("SortColumn", GetInstallDismantleSortColumn(), true));

                        break;
                    case ReportEnum.Transactions:
                        GetSelectedItem(reportParams, "ExhibitorId", cboExhibitorId);
                        GetSelectedItem(reportParams, "PaymentTransactionActiveFlag", ddlActiveFilter, "-1");
                        GetSelectedItem(reportParams, "PaymentTransactionSuccessFlag", ddlSuccessFilter, "-1");
                        break;
                    default:
                        break;
                }

                if (plcOutboundAddressFilter.Visible)
                {
                    GetSelectedItem(reportParams, "OutboundAddressFilter", ddlOutboundAddressFilter);
                }

                if (plcStartEndDates.Visible)
                {
                    string startDate = "1/1/1753 12:00:00 AM";
                    string endDate = "12/31/9999 11:59:59 PM";

                    if (!string.IsNullOrEmpty(txtStartDate.Text))
                    {
                        startDate = txtStartDate.Text + " 12:00:00 AM";
                    }
                    if (!string.IsNullOrEmpty(txtEndDate.Text))
                    {
                        endDate = txtEndDate.Text + " 11:59:59 PM";
                    }

                    reportParams.Add(new ReportParameter("StartDate", startDate, true));
                    reportParams.Add(new ReportParameter("EndDate", endDate, true));
                }

                if (plcBoothNumberFilter.Visible)
                {
                    string boothNumberFilter = null;
                    if (!string.IsNullOrEmpty(cboBoothNumber.SelectedValue))
                    {
                        boothNumberFilter = cboBoothNumber.SelectedValue.Trim();
                    }
                    else
                    {
                        boothNumberFilter = null;
                    }
                    reportParams.Add(new ReportParameter("BoothNumber", boothNumberFilter, true));
                }


                PrepareViewer(report, reportParams, rawDataReport);

            }
            catch (Exception ex)
            {
                PageErrors.DisplayException(ex);
                EventLogger.LogException(ex);
            }
        }

        private string[] BuildTransationTypeFilter()
        {
            List<string> transactionTypes = new List<string>();

            foreach (ListItem li in chklstTransactionTypeCd.Items)
            {
                if (li.Selected)
                {
                    transactionTypes.Add(li.Value);
                }
            }

            return transactionTypes.ToArray();
        }

        private List<int> BuildMultiCategoryFilterParams()
        {
            List<int> categoryIdFilter = new List<int>();

            if (rdoEnableMultiCategoryFilter.Items[0].Selected)
            {
                ProductController prodCntrl = new ProductController();
                List<Category> categories = prodCntrl.GetCategoryList(CurrentUser.CurrentShow.ShowId).Where(cat => cat.ActiveFlag == true).OrderBy(cat => cat.CategoryName).ToList();

                if (ParseCurrentReport() == ReportEnum.InstallDismantleReport)
                {
                    categories = categories.Where(c => c.Products.Count(p => p.InstallDismantleInd == "I" || p.InstallDismantleInd == "D") > 0).ToList();
                }
                categories.ForEach(c =>
                    {
                        categoryIdFilter.Add(c.CategoryId);
                    });

            }
            else
            {
                foreach (ListItem li in chkLstCategoryIdFilter.Items)
                {
                    if (li.Selected)
                    {
                        categoryIdFilter.Add(Util.ConvertInt32(li.Value));
                    }
                }
            }
           
            return categoryIdFilter;
        }

        private List<int> BuildAllowableShowList(ExpoOrdersUser currentUser)
        {
            List<int> showIds = new List<int>();

            if (currentUser.AllowableShowIds != null && currentUser.AllowableShowIds.Count > 0)
            {
                showIds = currentUser.AllowableShowIds;
            }
            else
            {
                //Get ALL the dam show ids
                ShowController cntrl = new ShowController(currentUser);
                cntrl.GetOwnedShowsByUserId(currentUser, true).ForEach(s =>
                {
                    showIds.Add(s.ShowId);
                });
            }

            return showIds;
        }
        private List<int> BuildMultiProductFilterParams(List<int> categoryFilter)
        {
            List<int> productIdFilter = new List<int>();

            if (rdoEnableMultiCategoryFilter.Items[0].Selected || rdoEnableMultiProductFilter.Items[0].Selected)
            {
                ProductController prodCntrl = new ProductController();

                List<Product> products = prodCntrl.GetProductFilterList(CurrentUser, CurrentUser.CurrentShow.ShowId, categoryFilter);
                if (ParseCurrentReport() == ReportEnum.InstallDismantleReport)
                {
                    products = products.Where(p => p.InstallDismantleInd == "I" || p.InstallDismantleInd == "D").ToList();
                }
                products.ForEach(p =>
                {
                    productIdFilter.Add(p.ProductId);
                });
            }
            else
            {
                
                foreach (ListItem li in chkLstProductIdFilter.Items)
                {
                    if (li.Selected)
                    {
                        productIdFilter.Add(Util.ConvertInt32(li.Value));
                    }
                }
            }
            
            return productIdFilter;
        }

        protected void cboCategory_selectedIndexChanged(object sender, EventArgs e)
        {
            LoadProductList(Util.ConvertInt32(cboCategoryId.SelectedValue), cboProduct);
        }

        protected void cboPriceListRptCategory_selectedIndexChanged(object sender, EventArgs e)
        {
            LoadProductList(Util.ConvertInt32(this.cboPriceListRptCategory.SelectedValue), this.cboPriceListRptProduct);
        }

        protected void ddlExhibitor2_IndexChanged(object sender, EventArgs e)
        {
            LoadExhibitorOrderItems();
        }

        

        #endregion

        #region Order Receipt Reports

        protected void rblOrderReceiptCriteriaChoice_onSelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblOrderReceiptCriteriaChoice.SelectedIndex == 0)
            {
                plcReportViewer.Visible = rptViewer.Visible = false;
                ltrOrderId.Visible = false;
                cboOrderId.Visible = false;
                LoadExhibitorList(cboOrderReceiptExhibitorId);
                ltrSelectOrderReceiptExhibitor.Visible = true;
                cboOrderReceiptExhibitorId.Visible = true;
                DisplayGenericViewReportButton();
            }
            else
            {
                plcReportViewer.Visible = rptViewer.Visible = false;
                ltrSelectOrderReceiptExhibitor.Visible = false;
                cboOrderReceiptExhibitorId.Visible = false;
                LoadOrderIdList(cboOrderId);
                ltrOrderId.Visible = true;
                cboOrderId.Visible = true;
                DisplayGenericViewReportButton();
            }
        }

        private void LoadFormNameSelections()
        {
            if (ParseCurrentReport() == ReportEnum.FormSubmission)
            {
                plcFormNameSelection.Visible = true;
                cboFormName.Visible = true;
                LoadFormNames(cboFormName);
            }
            else
            {
                plcFormNameSelection.Visible = false;
            }
        }

        private void GetSelectedClassification(List<ReportParameter> reportParams)
        {
            string classificationFilter = null;
            if (plcExhibitorClassification.Visible && cboExhibitorClassification.Visible)
            {
                
                if (!string.IsNullOrEmpty(cboExhibitorClassification.SelectedValue))
                {
                    classificationFilter = cboExhibitorClassification.SelectedValue.Trim();
                }
                else
                {
                    classificationFilter = null;
                }
            }

            reportParams.Add(new ReportParameter("Classification", classificationFilter, true));
        }

        private void GetSelectedItem(List<ReportParameter> reportParams, string paramName, DropDownList ddl)
        {
            GetSelectedItem(reportParams, paramName, ddl, "0");
        }

        private void GetSelectedItem(List<ReportParameter> reportParams, string paramName, DropDownList ddl, string notSelectedValue)
        {
            if (ddl.SelectedIndex == 0)
            {
                //ExhibitorId = 0, triggers the Where clause to NOT Filter
                reportParams.Add(new ReportParameter(paramName, notSelectedValue, true));
            }
            else
            {
                //The user only selected a single exhibitor, so only send one ExhibitorId value
                reportParams.Add(new ReportParameter(paramName, ddl.SelectedValue, true));
            }
        }

        private void GetSelectedOrders(List<ReportParameter> reportParams, DropDownList ddl)
        {
            if (ddl.SelectedIndex == 0)
            {
                //OrderId = 0 triggers the WHERE clause to NOT filter any, better than doing the IN clause which is extra overhead
                reportParams.Add(new ReportParameter("OrderId", "0", true));
            }
            else
            {
                //The user only selected a single order, so only send one OrderId value
                reportParams.Add(new ReportParameter("OrderId", ddl.SelectedValue, true));
            }
        }

        private void GetSelectedFormName(List<ReportParameter> reportParams, DropDownList ddl)
        {
            if (ddl.SelectedIndex == 0)
            {
                reportParams.Add(new ReportParameter("FormName", "", true));
            }
            else
            {
                reportParams.Add(new ReportParameter("FormName", cboFormName.SelectedValue, true));
            }
        }

        #endregion

        #endregion

    }
}
