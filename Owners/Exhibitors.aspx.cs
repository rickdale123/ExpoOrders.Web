using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using ExpoOrders.Web.CustomControls;
using ExpoOrders.Web.Owners.Common;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System.Drawing;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using System.Text;

namespace ExpoOrders.Web.Owners
{
    public enum ExhibitorPageMode { Exhibitors = 1, UploadExhibitors = 2, ExhibitorDetail = 3 }

    public partial class Exhibitors : BaseOwnerPage
    {

        #region Manager Objects


        private OwnerAdminController _adminCntrl = null;
        public OwnerAdminController OwnerAdminMgr
        {
            get
            {
                if (_adminCntrl == null)
                {
                    _adminCntrl = new OwnerAdminController();
                }

                return _adminCntrl;

            }
        }

        private HtmlContentController _htmlContentCntrl = null;
        public HtmlContentController HtmlContentMgr
        {
            get
            {
                if (_htmlContentCntrl == null)
                {
                    _htmlContentCntrl = new HtmlContentController();
                }

                return _htmlContentCntrl;

            }
        }
        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.NavigationItemCallBack = this.HandleNavigationItemClicked;
            this.Master.PreviewShowCallBack = this.PreviewShow;

            if (!Page.IsPostBack)
            {
                LoadPage();
            }
        }

        private void LoadPage()
        {
            this.Master.LoadMasterPage(base.CurrentUser, base.CurrentUser.CurrentShow, OwnerPage.Exhibitors, OwnerTabEnum.Exhibitors);
            this.Master.LoadSubNavigation("Exhibitors", OwnerUtil.BuildExhibitorsSubNavLinks());

            LoadPageMode(ExhibitorPageMode.Exhibitors);

            if (!string.IsNullOrEmpty(JumpMode))
            {
                if (JumpMode == "ManageCreditCards")
                {
                    LoadExhibitorCreditCards();
                }
            }
        }

        private void LoadPageMode(ExhibitorPageMode mode)
        {
            if (this.Master.CurrentSubNavigationLinks != null && this.Master.CurrentSubNavigationLinks.Count > 0)
            {
                NavigationLink linkToSelect = this.Master.CurrentSubNavigationLinks.SingleOrDefault<NavigationLink>(l => l.TargetId == (int)mode);
                this.Master.SelectNavigationItem(linkToSelect.NavigationLinkId);

                if (LinkToExhibitorId > 0)
                {
                    LoadPageMode(LinkToExhibitorId, ExhibitorPageMode.ExhibitorDetail);
                }
                else
                {
                    LoadPageMode(linkToSelect.NavigationLinkId, (int)linkToSelect.TargetId);
                }
            }
        }

        private void LoadPageMode(int navLinkId, int targetId)
        {
            LoadPageMode(navLinkId, (ExhibitorPageMode)Enum.Parse(typeof(ExhibitorPageMode), targetId.ToString(), true));
        }

        private void LoadPageMode(int navLinkId, ExhibitorPageMode currentPageMode)
        {
            plcExhibitorUpload.Visible =
            plcExhibitorList.Visible =
            plcManageExhibitor.Visible =
            plcExhibitorUserList.Visible =
            plcUserInformation.Visible =
            plcCreditCardDetail.Visible = 
            plcManageCreditCards.Visible =
            plcManageAddresses.Visible = 
            plcAddressList.Visible = 
            plcAddressDetail.Visible = false;

            switch (currentPageMode)
            {
                case ExhibitorPageMode.Exhibitors:
                    plcExhibitorList.Visible = true;
                    InitializeExhibitorSearch();
                    break;
                case ExhibitorPageMode.ExhibitorDetail:
                    plcExhibitorList.Visible = false;
                    ManageExhibitor(navLinkId);
                    break;
                case ExhibitorPageMode.UploadExhibitors:
                    plcExhibitorUpload.Visible = true;
                    ltrUploadNote.Text = GetHelpText("ExhibitorUploadNote");
                    plcExhibitorUploadResults.Visible = false;
                    break;
            }
        }

        private void InitializeExhibitorSearch()
        {
            txtSearchBoothNumber.Text = string.Empty;
            txtSearchPrimaryEmailAddress.Text = string.Empty;
            txtSearchExhibitorId.Text = string.Empty;
            LoadExhibitorSearchList(cboSearchExhibitorId);
            cboSearchExhibitorId.SelectedIndex = 0;
            chkSearchIncludeInactive.Checked = false;

            rbtnAllExhibitorOrders.Checked = true;
            rbtnOnlyExhibitorsWithOrders.Checked = false;
            rbtnOnlyExhibitorsWithNoOrders.Checked = false;


            rbtnAllExhibitorCreditCards.Checked = true;
            rbtnOnlyExhibitorsWithNoCreditCards.Checked = false;
            rbtnOnlyExhibitorsWithCreditCards.Checked = false;

            cboExhibitorClassification.Items.Clear();
            cboExhibitorClassification.ClearSelection();

            if (!string.IsNullOrEmpty(CurrentUser.CurrentOwner.ClassificationList))
            {
                plcExhibitorClassificationSearch.Visible = true;
                foreach (string item in Util.ParseDelimitedList(CurrentUser.CurrentOwner.ClassificationList, ';'))
                {
                    var itemVal = item.Trim();
                    if (!string.IsNullOrEmpty(itemVal))
                    {
                        cboExhibitorClassification.Items.Insert(0, new RadComboBoxItem { Text = itemVal, Value = itemVal });
                    }
                }

                cboExhibitorClassification.Items.Insert(0, new RadComboBoxItem { Text = " ", Value = "" });
            }

            plcExhibitorDisplay.Visible = false;

        }

        private void LoadExhibitorsList(SearchCriteria search, bool changeSortDir, string sortExpression = null)
        {
            if (string.IsNullOrEmpty(sortExpression))
            {
                sortExpression = ReadCookieValue("ExhibitorListSortColumn");
            }

            if (string.IsNullOrEmpty(sortExpression))
            {
                sortExpression = "ExhibitorCompanyNameDisplay";
            }

            if (search == null)
            {
                search = new SearchCriteria();
            }
            lblExhibitorListRowCount.Text = "0 rows to display.<br/>";

            List<Exhibitor> exhibitors = this.OwnerAdminMgr.SearchExhibitors(CurrentUser, CurrentUser.CurrentShow.ShowId, search);//.OrderBy(e => e.ExhibitorCompanyName).ToList();
            if (exhibitors != null)
            {
                lblExhibitorListRowCount.Text = string.Format("({0} rows displayed.)<br/>", exhibitors.Count);
            }

            SortDirection sortDir = DetermineSortDirection(changeSortDir, sortExpression, "ExhibitorListSortColumn", "ExhibitorListSortDir");
            WebUtil.SortList(exhibitors, sortExpression, sortDir);

            grdvwExhibitorList.DataSource = exhibitors;
            grdvwExhibitorList.DataBind();

            PutSortDirectionImage(grdvwExhibitorList, sortExpression, sortDir);

            plcExhibitorDisplay.Visible = true;
        }

        private SearchCriteria BuildSearchCriteria()
        {
            SearchCriteria searchCriteria = new SearchCriteria();
            searchCriteria.BoothNumber = txtSearchBoothNumber.Text.Trim();

            if (cboSearchExhibitorId.SelectedIndex > 0)
            {
                searchCriteria.ExhibitorId = Util.ConvertInt32(cboSearchExhibitorId.SelectedValue);
            }
            else
            {
                searchCriteria.ExhibitorId = Util.ConvertInt32(txtSearchExhibitorId.Text.Trim());
            }
            
            searchCriteria.PrimaryEmailAddress = txtSearchPrimaryEmailAddress.Text.Trim();
            searchCriteria.IncludeInactive = chkSearchIncludeInactive.Checked;

            if (this.rbtnOnlyExhibitorsWithOrders.Checked)
            {
                searchCriteria.ExhibitorsWithOrders = true;
            }
            else if (rbtnOnlyExhibitorsWithNoOrders.Checked)
            {
                searchCriteria.ExhibitorsWithOrders = false;
            }
            else
            {
                searchCriteria.ExhibitorsWithOrders = null;
            }

            if (rbtnOnlyExhibitorsWithCreditCards.Checked)
            {
                searchCriteria.ExhibitorsWithCreditCards = true;
            }
            else if (rbtnOnlyExhibitorsWithNoCreditCards.Checked)
            {
                searchCriteria.ExhibitorsWithCreditCards = false;
            }
            else
            {
                searchCriteria.ExhibitorsWithCreditCards = null;
            }

            if (plcExhibitorClassificationSearch.Visible)
            {
                if (!string.IsNullOrEmpty(cboExhibitorClassification.Text))
                {
                    searchCriteria.ExhibitorClassification = cboExhibitorClassification.Text;
                }
            }
            
            return searchCriteria;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadExhibitorsList(BuildSearchCriteria(), false, null);
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            InitializeExhibitorSearch();
        }

        #endregion

        #region Control Events

        private void HandleNavigationItemClicked(int navLinkId, string linkText, NavigationLink.NavigationalAction action, int targetId)
        {
            this.LoadPageMode(navLinkId, targetId);
        }

        #region Exhibitor File Upload

        protected void btnUploadFile_Click(object sender, EventArgs e)
        {
            if (fupExhibitorFile.HasFile)
            {
                this.Master.DisplayFriendlyMessage("Uploading File...Please wait");
                string uploadFilePath = Server.MapPath(string.Format("~/Uploads/Shows/{0}/ExhibitorUpload_{1}.csv", CurrentUser.CurrentShow.ShowGuid, DateTime.Now.Ticks));
                fupExhibitorFile.SaveAs(uploadFilePath);

                UploadController uploadMgr = new UploadController(CurrentUser);

                string schemaTemplateFilePath = Server.MapPath("~/Templates/Schema.ini");
                List<ExhibitorFileRecord> exhibitorUploads = uploadMgr.UploadExhibitors(uploadFilePath, CurrentUser, CurrentUser.CurrentShow.ShowId, schemaTemplateFilePath);

                if (exhibitorUploads != null && exhibitorUploads.Count > 0)
                {

                    int countUploadErrors = CalcUploadErrors(exhibitorUploads);
                    
                    this.Master.DisplayFriendlyMessage(string.Format("Exhibitor Import Successful with {0} errors.", countUploadErrors));

                    plcExhibitorUploadResults.Visible = true;

                    grdvExhibitorUploadResults.Visible = true;
                    grdvExhibitorUploadResults.DataSource = exhibitorUploads;
                    grdvExhibitorUploadResults.DataBind();

                }
                else
                {
                    this.Master.DisplayFriendlyMessage("No Exhibitors were uploaded, check the file and try again.");
                }
            }
        }

        private int CalcUploadErrors(List<ExhibitorFileRecord> exhibitorUploads)
        {
            int errorCount = 0;
            if (exhibitorUploads != null && exhibitorUploads.Count > 0)
            {
                exhibitorUploads.ForEach(u =>
                {
                    if (!u.ImportErrors.IsValid)
                    {
                        errorCount++;
                    }
                });
            }

            return errorCount;
        }

        protected void fupldExhibitorFile_UploadFileError(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
        }
        #endregion

        #region Exhibitor Profile

        protected void grdvExhibitorUploadResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ExhibitorFileRecord exhibitorUpload = (ExhibitorFileRecord)e.Row.DataItem;
                if (!exhibitorUpload.ImportErrors.IsValid)
                {
                    e.Row.CssClass = "rowHighlightError";
                }
            }
        }

        protected void grdvwExhibitorList_Sorting(object sender, GridViewSortEventArgs e)
        {
            LoadExhibitorsList(BuildSearchCriteria(), true, e.SortExpression);
        }

        protected void grdvwExhibitorList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Exhibitor exhibitorRow = (Exhibitor)e.Row.DataItem;

                HtmlAnchor lnkMailToExhibitor = (HtmlAnchor)e.Row.FindControl("lnkMailToExhibitor");
                HtmlAnchor lnkExhibitorAttachments = (HtmlAnchor)e.Row.FindControl("lnkExhibitorAttachments");

                lnkExhibitorAttachments.Attributes.Add("onClick", string.Format("launchExhibitorAttachments({0}); return false;", exhibitorRow.ExhibitorId));

                if (!string.IsNullOrEmpty(exhibitorRow.PrimaryEmailAddress))
                {
                    lnkMailToExhibitor.HRef = string.Format("mailto:{0}", exhibitorRow.PrimaryEmailAddress);
                    lnkMailToExhibitor.InnerText = exhibitorRow.PrimaryEmailAddress;
                    lnkMailToExhibitor.Visible = true;
                }
                else
                {
                    lnkMailToExhibitor.Visible = false;
                }


                LinkButton lbtnManageExhibitor = (LinkButton)e.Row.FindControl("lbtnManageExhibitor");
                LinkButton lbtnShowUserList = (LinkButton)e.Row.FindControl("lbtnShowUserList");

                lbtnManageExhibitor.CommandArgument =
                    lbtnShowUserList.CommandArgument = exhibitorRow.ExhibitorId.ToString();

                Literal ltrActive = (Literal)e.Row.FindControl("ltrActive");

                Literal ltrInvited = (Literal)e.Row.FindControl("ltrInvited");

                if (exhibitorRow.ActiveFlag)
                {
                    ltrActive.Text = "Yes";
                    lbtnManageExhibitor.Visible = true;
                    lbtnShowUserList.Visible = true;
                }
                else
                {
                    ltrActive.Text = "No";
                }

                bool isInvited = exhibitorRow.InvitedFlag.HasValue ? (bool)exhibitorRow.InvitedFlag : false;

                if (isInvited)
                {
                    ltrInvited.Text = "Yes";
                }
                else
                {
                    ltrInvited.Text = "No";
                }
            }
        }

        protected void grdvwExhibitorList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int exhibitorId = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "EditExhibitor":
                    ManageExhibitor(exhibitorId);
                    break;
                case "ShowUserList":
                    ShowUserList(exhibitorId);
                    break;
                case "ViewEmailLog":
                    LaunchEmailLogViewer(null, exhibitorId);
                    break;
                case "ViewCallLog":
                    LaunchCallLogViewer(null, exhibitorId);
                    break;
            }
        }


        private void ManageExhibitor(int exhibitorId)
        {
            ResetClassificationDropDown(plcClassification, ddlClassification);

            plcExhibitorUserList.Visible = 
                plcManageCreditCards.Visible =
                plcManageAddresses.Visible = false;

            hdnExhibitorAddressId.Value = "0";

            CurrentExhibitorId = exhibitorId;

            this.ucPrimaryUserDetail.Clear("ProfileInformation");

            OwnerUtil.ClearPlaceHolderControl(plcManageExhibitor);

            rdoListActiveFlag.SelectedValue = "1";
            
            lblExhibitorId.Text = exhibitorId.ToString();
            plcExhibitorActionButtons.Visible = false;

            DisplayExhibitorLinks(false);
            if (exhibitorId > 0)
            {
                DisplayExhibitorLinks(true);

                Exhibitor exhibitor = OwnerAdminMgr.GetExhibitorById(exhibitorId);
                if (exhibitor != null)
                {
                    plcExhibitorActionButtons.Visible = true;

                    btnSendPasswordReminder.CommandArgument = btnSendEmail.CommandArgument = exhibitor.ExhibitorId.ToString();
                    
                    //Paint exhibitor details
                    txtCompanyName.Text = exhibitor.ExhibitorCompanyName;
                    txtBoothNumber.Text = exhibitor.BoothNumber;
                    txtBoothDescription.Text = exhibitor.BoothDescription;
                    txtBoothNotes.Text = exhibitor.BoothNotes;

                    txtInternalNotes.Text = exhibitor.InternalNotes;
                    txtExternalNotes.Text = exhibitor.ExternalNotes;

                    if (exhibitor.CompanyAddressId > 0)
                    {
                        //Paint exhibitor Address 
                        hdnExhibitorAddressId.Value = exhibitor.CompanyAddressId.ToString();
                        txtAddressLine1.Text = exhibitor.Address.Street1;
                        txtAddressLine2.Text = exhibitor.Address.Street2;
                        txtCity.Text = exhibitor.Address.City;
                        txtState.Text = exhibitor.Address.StateProvinceRegion;
                        txtPostalCode.Text = exhibitor.Address.PostalCode;
                        txtCountry.Text = exhibitor.Address.Country;
                    }

                    //Paint Primary User
                    UserContainer primaryUserContainer = this.OwnerAdminMgr.GetExhibitorPrimaryUser(CurrentUser, exhibitor);

                    if (primaryUserContainer != null)
                    {
                        ucPrimaryUserDetail.Populate("ProfileInformation", primaryUserContainer);
                    }

                    txtCompanyPhone.Text = exhibitor.Phone;
                    txtCompanyEmailAddress.Text = exhibitor.PrimaryEmailAddress;

                    chkTaxExempt.Checked = exhibitor.TaxExemptFlag;
                    chkAllowSpecialCheckout.Checked = exhibitor.AllowSpecialCheckout.HasValue && exhibitor.AllowSpecialCheckout.Value;

                    rdoListActiveFlag.SelectedValue = exhibitor.ActiveFlag ? "1" : "0";

                    lnkExhibitorAttachments.Attributes.Add("onClick", string.Format("launchExhibitorAttachments({0}); return false;", exhibitor.ExhibitorId));

                    SelectClassification(ddlClassification, exhibitor.Classification);
                }
            }

            plcManageExhibitor.Visible = true;
            plcExhibitorList.Visible = false;
        }

        private void SaveUserList(int exhibitorId)
        {
            List<UserContainer> primaryUserList = new List<UserContainer>();

            CurrentExhibitorId = exhibitorId;

            foreach (GridViewRow gr in grdvwExhibitorUserList.Rows)
            {
                CheckBox chkPrimaryFlag = (CheckBox)gr.FindControl("chkPrimaryFlag");
                HiddenField hdnUserId = (HiddenField)gr.FindControl("hdnUserId");

                UserContainer u = new UserContainer { UserId = hdnUserId.Value, IsPrimary = chkPrimaryFlag.Checked };
                primaryUserList.Add(u);
            }


            ValidationResults errors = OwnerAdminMgr.SaveUserList(CurrentUser, exhibitorId, primaryUserList);
            if (errors.IsValid)
            {
                this.Master.DisplayFriendlyMessage("User List Saved.");
            }
            else
            {
                PageErrors.AddErrorMessages(errors);     
            }
            
        }

        protected void btnAddExhibitor_Click(object sender, EventArgs e)
        {
            ManageExhibitor(0);
        }

        protected void btnSaveExhibitor_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "ProfileInformation";
            if (Page.IsValid)
            {
                SaveExhibitor();

                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Exhibitor Profile saved.");
                }
            }
        }

        protected void btnSaveExhibitorReturn_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "ProfileInformation";
            if (Page.IsValid)
            {
                SaveExhibitor();

                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Exhibitor Profile saved.");
                    InitializeExhibitorSearch();
                    plcExhibitorDisplay.Visible = false;
                    plcExhibitorList.Visible = true;
                    plcManageExhibitor.Visible = false;
                }
            }
        }

        protected void btnSaveCreateExhibitorReturn_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "ProfileInformation";
            if (Page.IsValid)
            {
                SaveExhibitor();

                if (Page.IsValid)
                {
                    this.Master.DisplayFriendlyMessage("Exhibitor Profile saved.");
                    ManageExhibitor(0);
                }
            }
        }

        protected void btnCreateNewExhbitor_Click(object sender, EventArgs e)
        {
            ManageExhibitor(0);
        }
        protected void btnCancelSaveExhibitor_Click(object sender, EventArgs e)
        {
            InitializeExhibitorSearch();
            plcExhibitorDisplay.Visible = false;
            plcExhibitorList.Visible = true;
            plcManageExhibitor.Visible = false;
        }

        private void SaveExhibitor()
        {
            Address exhibitorAddress = new Address();
            Exhibitor exhibitorDetail = new Exhibitor();
            UserContainer primaryUser = ucPrimaryUserDetail.BuildUserContainer();
            primaryUser.IsPrimary = true;
            primaryUser.Active = true;

            exhibitorDetail.ExhibitorId = CurrentExhibitorId;
            exhibitorDetail.ShowId = this.CurrentUser.CurrentShow.ShowId;

            exhibitorDetail.ExhibitorCompanyName = txtCompanyName.Text.Trim();
            exhibitorDetail.BoothNumber = txtBoothNumber.Text.Trim();
            exhibitorDetail.BoothDescription = txtBoothDescription.Text.Trim();
            exhibitorDetail.BoothNotes = txtBoothNotes.Text.Trim();

            exhibitorDetail.InternalNotes = txtInternalNotes.Text.Trim();
            exhibitorDetail.ExternalNotes = txtExternalNotes.Text.Trim();
            

            exhibitorDetail.Phone = txtCompanyPhone.Text.Trim();
            exhibitorDetail.PrimaryEmailAddress = txtCompanyEmailAddress.Text.Trim();

            exhibitorDetail.TaxExemptFlag = chkTaxExempt.Checked;
            exhibitorDetail.AllowSpecialCheckout = chkAllowSpecialCheckout.Checked;
            exhibitorDetail.ActiveFlag = rdoListActiveFlag.SelectedValue == "1";

            exhibitorAddress.AddressId = 0;
            exhibitorAddress.Street1 = txtAddressLine1.Text.Trim();
            exhibitorAddress.Street2 = txtAddressLine2.Text.Trim();
            exhibitorAddress.City = txtCity.Text.Trim();
            exhibitorAddress.StateProvinceRegion = txtState.Text.Trim();
            exhibitorAddress.PostalCode = txtPostalCode.Text.Trim();
            exhibitorAddress.Country = txtCountry.Text.Trim();

            if (plcClassification.Visible && ddlClassification.Visible)
            {
                exhibitorDetail.Classification = ddlClassification.SelectedValue;
            }
            else
            {
                exhibitorDetail.Classification = null;
            }


            List<string> userRoles = new List<string>();
            userRoles.Add("Exhibitor");

            ValidationResults errors = this.OwnerAdminMgr.SaveExhibitor(CurrentUser, exhibitorDetail, exhibitorAddress, primaryUser, userRoles, null);

            if (!errors.IsValid)
            {
                PageErrors.AddErrorMessages(errors, PageErrors.ValidationGroup);
            }
            else
            {
                //Reload the whole Page again, in order to get the New Exhibitor/Primary user to paint properly
                ManageExhibitor(exhibitorDetail.ExhibitorId);
            }
        }

        private void ShowUserList(int exhibitorId)
        {
            CurrentExhibitorId = exhibitorId;
            LoadExhibitorUserList(exhibitorId);
            plcExhibitorUserList.Visible = true;
            plcExhibitorList.Visible = false;
        }

        private void DisplayExhibitorLinks(bool display)
        {
            plcExhibitorNavLinks.Visible = display;
        }

        private void SaveExhibitorList()
        {

            List<Exhibitor> exhibitorBooths = new List<Exhibitor>();
            
            for (int rowindex = 0; rowindex < grdvwExhibitorList.Rows.Count; rowindex++)
            {
                TextBox txtBoothNumber = (TextBox)grdvwExhibitorList.Rows[rowindex].FindControl("txtBoothNumber");
                TextBox txtBoothDescription = (TextBox)grdvwExhibitorList.Rows[rowindex].FindControl("txtBoothDescription");
                HiddenField hdnExhibitorId = (HiddenField)grdvwExhibitorList.Rows[rowindex].FindControl("hdnExhibitorId");

                exhibitorBooths.Add(new Exhibitor()
                {
                    ExhibitorId = Util.ConvertInt32(hdnExhibitorId.Value), 
                    BoothNumber = txtBoothNumber.Text.Trim(),
                    BoothDescription = txtBoothDescription.Text.Trim()
                });
            }
            OwnerAdminMgr.SaveExhibitorBoothNumbers(CurrentUser, exhibitorBooths);

            this.Master.DisplayFriendlyMessage("Exhibitor Booth Numbers Saved.");
        }

        #endregion

        #region ExhibitorUserList

        private void LoadExhibitorUserList(int exhibitorId)
        {
            Exhibitor exhibitor = OwnerAdminMgr.GetExhibitorById(exhibitorId);
            if (exhibitor != null)
            {
                lbtnExhibitorUserList.Text = exhibitor.ExhibitorCompanyName;
                lbtnExhibitorUserList.CommandArgument = exhibitor.ExhibitorId.ToString();
                grdvwExhibitorUserList.DataSource = exhibitor.Users.Where(u => u.ExpoOrdersMembership.IsApproved == true);
                grdvwExhibitorUserList.DataBind();
            }
        }


        protected void lbtnViewPayments_Click(object sender, EventArgs e)
        {
            LinkToExhibitorId = CurrentExhibitorId;
            Server.Transfer("Payments.aspx", false);
        }

        protected void lbtnViewOrders_Click(object sender, EventArgs e)
        {
            LinkToExhibitorId = CurrentExhibitorId;
            Server.Transfer("Orders.aspx", false);
        }

        protected void lbtnViewCallLogs_Click(object sender, EventArgs e)
        {
            LaunchCallLogViewer(null, CurrentExhibitorId);
        }

        protected void lbtnViewEmailLogs_Click(object sender, EventArgs e)
        {
            LaunchEmailLogViewer(null, CurrentExhibitorId);
        }


        protected void lbtnExhibitorUserList_Click(object sender, EventArgs e)
        {
            ManageExhibitor(Util.ConvertInt32(lbtnExhibitorUserList.CommandArgument));
            plcExhibitorUserList.Visible = false;
        }

        private void LoadUserList()
        {
            LoadExhibitorUserList(CurrentExhibitorId);
            plcExhibitorUserList.Visible = true;
            plcManageExhibitor.Visible = false;
            plcUserInformation.Visible = false;
            plcAddressList.Visible = false;
            plcAddressDetail.Visible = false;
        }

        

        protected void lbtnShowUsers_Click(object sender, EventArgs e)
        {
            LoadUserList();    
        }

        protected void lbtnManageAddresses_Click(object sender, EventArgs e)
        {
            LoadExhibitorAddresses();
        }

        protected void lbtnManageCreditCards_Click(object sender, EventArgs e)
        {
            ReturnPage = null;
            LoadExhibitorCreditCards();
        }


        protected void grdvwExhibitorUserList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                User user = (User)e.Row.DataItem;
                ExtendedUserInfo userInfo = user.ExtendedUserInfos.FirstOrDefault();
                LinkButton lbtnManageUser = (LinkButton)e.Row.FindControl("lbtnManageUser");
                LinkButton lbtnActivateUser = (LinkButton)e.Row.FindControl("lbtnActivateUser");
                LinkButton lbtnDeactivateUser = (LinkButton)e.Row.FindControl("lbtnDeactivateUser");
                PlaceHolder plcUserActivation = (PlaceHolder)e.Row.FindControl("plcUserActivation");

                if (userInfo != null)
                {
                    Literal ltrFirstName = (Literal)e.Row.FindControl("ltrFirstName");
                    ltrFirstName.Text = userInfo.FirstName;

                    Literal ltrLastName = (Literal)e.Row.FindControl("ltrLastName");
                    ltrLastName.Text = userInfo.LastName;
                }

                if (user != null)
                {
                    LinkButton lbtnManageUser2 = (LinkButton)e.Row.FindControl("lbtnManageUser2");
                    lbtnManageUser2.Text = userInfo.PreferredUserName;
                    lbtnManageUser2.Visible = true;

                    Literal ltrUserEmail = (Literal)e.Row.FindControl("ltrUserEmail");
                    CheckBox chkPrimaryFlag = (CheckBox)e.Row.FindControl("chkPrimaryFlag");

                    chkPrimaryFlag.Checked = user.IsPrimary;

                    ltrUserEmail.Text = user.ExpoOrdersMembership.Email;

                    lbtnManageUser.CommandArgument = lbtnManageUser2.CommandArgument = lbtnActivateUser.CommandArgument
                                = lbtnDeactivateUser.CommandArgument = user.UserId.ToString();
                }

                if (user.IsPrimary)
                {
                    plcUserActivation.Visible = false;
                    lbtnDeactivateUser.Visible = false;
                }
                else
                {
                    lbtnDeactivateUser.Attributes.Add("onClick", "JavaScript: return confirm('Sure you want to Remove this user?');");
                    lbtnDeactivateUser.Visible = true;
                    plcUserActivation.Visible = true;
                }
                lbtnManageUser.Visible = true;
            }
        }

        protected void grdvwExhibitorUserList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string userId = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "ManageUser":
                    ManageUser(userId);
                    break;
                case "DeactivateUser":
                    DeactivateUser(userId);
                    break;
                case "ActivateUser":
                    ActivateUser(userId);
                    break;
                case "ViewEmailLog":
                    LaunchEmailLogViewer(userId, 0);
                    break;
                case "ViewCallLog":
                    LaunchCallLogViewer(userId, 0);
                    break;
            }
        }


        private void ActivateUser(string userId)
        {
            this.OwnerAdminMgr.ActivateUser(CurrentUser, userId);
            this.Master.DisplayFriendlyMessage("User activated.");
        }

        private void DeactivateUser(string userId)
        {
            this.OwnerAdminMgr.DeactivateUser(CurrentUser, userId);

            LoadExhibitorUserList(CurrentExhibitorId);
            this.Master.DisplayFriendlyMessage("User de-activated.");
        }

        private void ManageUser(string userId)
        {
            ucUserDetail.Clear("UserInformation");

            OwnerUtil.ClearPlaceHolderControl(plcUserInformation);

            if (!string.IsNullOrEmpty(userId))
            {
                UserContainer userInfo = OwnerAdminMgr.GetMemberShipUser(userId, false);
                ucUserDetail.Populate("UserInformation", userInfo);
            }

            plcExhibitorUserList.Visible = false;
            plcUserInformation.Visible = true;
            plcManageAddresses.Visible = false;
            plcAddressList.Visible = false;
            plcAddressDetail.Visible = false;
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            ManageUser(string.Empty);
        }

        protected void btnUserListRefresh_Click(object sender, EventArgs e)
        {
            LoadExhibitorUserList(CurrentExhibitorId);
        }

        protected void btnCancelManageUsers_Click(object sender, EventArgs e)
        {
            ManageExhibitor(CurrentExhibitorId);
        }

        protected void btnSaveUsers_Click(object sender, EventArgs e)
        {
            SaveUserList(CurrentExhibitorId);
        }

        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "UserInformation";

            UserContainer userInfo = ucUserDetail.BuildUserContainer();
            userInfo.Active = true;

            if (string.IsNullOrEmpty(userInfo.Password.Trim()))
            {
                ValidationResult error = new ValidationResult("Must provide a password.", null, null, null, null);
                PageErrors.AddErrorMessage(error, PageErrors.ValidationGroup);
            }

            if (Page.IsValid)
            {
                SaveUser(userInfo);
                if (Page.IsValid)
                {
                    LoadUserList();
                    this.Master.DisplayFriendlyMessage("User saved.");
                }
            }
        }

        private void SaveUser(UserContainer additionalUser)
        {

            List<string> userRoles = new List<string>();
            userRoles.Add("Exhibitor");
            ValidationResults errors = this.OwnerAdminMgr.SaveAdditionalExhibitorUser(CurrentUser, CurrentUser.CurrentShow.ShowId, CurrentExhibitorId, additionalUser, userRoles);

            if (!errors.IsValid)
            {
                PageErrors.AddErrorMessages(errors, PageErrors.ValidationGroup);
            }
        }

        #endregion

        private void LaunchMultiEmailEditor(ContentTypeEnum contentType)
        {
            PageErrors.ValidationGroup = "EmailInformation";
            ValidationResults errors = new ValidationResults();
            EmailTransportEntity emailTransportObject = this.CurrentUser.EmailTransportObject != null ? this.CurrentUser.EmailTransportObject : new EmailTransportEntity();
            emailTransportObject.SelectedExhibitorIds = new List<int>();
            emailTransportObject.ClearAttachmentList();

            bool preserveAttachmentList = false;
            for (int rowindex = 0; rowindex < grdvwExhibitorList.Rows.Count; rowindex++)
            {
                CheckBox chkSelected = (CheckBox)grdvwExhibitorList.Rows[rowindex].FindControl("chkSelected");
                HiddenField hdnExhibitorId = (HiddenField)grdvwExhibitorList.Rows[rowindex].FindControl("hdnExhibitorId");

                if (chkSelected != null)
                {
                    if (chkSelected.Checked)
                    {
                        if (hdnExhibitorId != null)
                        {
                            emailTransportObject.SelectedExhibitorIds.Add(Util.ConvertInt32(hdnExhibitorId.Value));

                            if (contentType == ContentTypeEnum.InvoiceEmail)
                            {
                                emailTransportObject.AttachmentList = new List<string>();
                                //put placeholder for {Generate Invoice}
                                emailTransportObject.AttachmentList.Add("{ExhibitorInvoice}");
                                preserveAttachmentList = true;
                            }
                            this.CurrentUser.EmailTransportObject = emailTransportObject;
                        }
                    }
                }
            }

            if (emailTransportObject.SelectedExhibitorIds != null & emailTransportObject.SelectedExhibitorIds.Count > 0)
            {

                this.LaunchEmailEditor(contentType, preserveAttachmentList);
            }
            else
            {
                errors.AddResult(new ValidationResult("You must select at least one Exhibitor.", null, null, null, null));
                PageErrors.AddErrorMessages(errors, "EmailInformation");
            }
        }

        protected void btnSendEmails_Click(object sender, EventArgs e)
        {
            LaunchMultiEmailEditor(ContentTypeEnum.BlankEmail);
        }

        protected void btnSendWelcomeKits_Click(object sender, EventArgs e)
        {
            LaunchMultiEmailEditor(ContentTypeEnum.WelcomeKit);
        }

        protected void btnSendInvoices_Click(object sender, EventArgs e)
        {
            LaunchMultiEmailEditor(ContentTypeEnum.InvoiceEmail);
        }

        protected void btnSendInvoice_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "SendInvoiceInformation";

            int exhibitorId = Util.ConvertInt32(lblExhibitorId.Text.Trim());

            if (exhibitorId > 0)
            {
                ValidationResults errors = base.SendExhibitorInvoice(CurrentUser, exhibitorId);
                if (!errors.IsValid)
                {
                    PageErrors.AddErrorMessages(errors);
                }
            }
        }
        
        protected void btnSendWelcomeKit_Click(object sender, EventArgs e)
        {
            EmailTransportEntity emailTransportObject = this.CurrentUser.EmailTransportObject != null ? this.CurrentUser.EmailTransportObject : new EmailTransportEntity();

            if (emailTransportObject.SelectedExhibitorIds == null)
            {
                emailTransportObject.SelectedExhibitorIds = new List<int>();
            }

            emailTransportObject.SelectedExhibitorIds.Clear();
            emailTransportObject.SelectedExhibitorIds.Add(CurrentExhibitorId);
            this.CurrentUser.EmailTransportObject = emailTransportObject;
            this.CurrentUser.EmailTransportObject.ClearAttachmentList();
            this.LaunchEmailEditor(ContentTypeEnum.WelcomeKit, false);
        }

        protected void btnSendPasswordReminder_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                int exhibitorId = Util.ConvertInt32(((Button)sender).CommandArgument);
                CurrentUser.EmailTransportObject = new EmailTransportEntity();
                CurrentUser.EmailTransportObject.SelectedExhibitorIds = new List<int>();
                CurrentUser.EmailTransportObject.SelectedExhibitorIds.Add(exhibitorId);
                this.CurrentUser.EmailTransportObject.ClearAttachmentList();

                this.LaunchEmailEditor(ContentTypeEnum.PasswordReminder, false);
            }   
        }

        protected void btnSendEmail_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                int exhibitorId = Util.ConvertInt32(((Button)sender).CommandArgument);
                CurrentUser.EmailTransportObject = new EmailTransportEntity();
                CurrentUser.EmailTransportObject.SelectedExhibitorIds = new List<int>();
                CurrentUser.EmailTransportObject.SelectedExhibitorIds.Add(exhibitorId);
                this.CurrentUser.EmailTransportObject.ClearAttachmentList();

                this.LaunchEmailEditor(ContentTypeEnum.BlankEmail, false);
            }   
        }

        protected void btnCancelSaveUser_Click(object sender, EventArgs e)
        {
            LoadUserList();
        }

        protected void btnSaveExhibitorList_Click(object sender, EventArgs e)
        {
            SaveExhibitorList();
        }
        #endregion

        #region Manage Address

        private void DeleteAddress(int addressId)
        {
            OwnerAdminMgr.DeleteExhibitorAddress(CurrentUser, addressId);
            LoadExhibitorAddresses();
        }

        private void ManageAddress(int addressId)
        {
            plcManageAddresses.Visible = true;
            plcAddressList.Visible = false;
            plcAddressDetail.Visible = true;

            hdnAddressId.Value = addressId.ToString();
            
            ClearPlaceHolderControl(plcAddressDetail);

            if (addressId > 0)
            {
                Address currentAddress = OwnerAdminMgr.GetExhibitorAddress(CurrentUser, addressId);

                if (currentAddress != null)
                {
                    WebUtil.SelectListItemByValue(ddlOtherAddressType, currentAddress.AddressTypeCd);
                    txtOtherAddressLine1.Text = currentAddress.Street1;
                    txtOtherAddressLine2.Text = currentAddress.Street2;
                    txtOtherAddressLine3.Text = currentAddress.Street3;
                    txtOtherAddressLine4.Text = currentAddress.Street4;
                    //txtOtherAddressLine5.Text = currentAddress.Street5;
                    txtOtherAddressCity.Text = currentAddress.City;
                    txtOtherAddressState.Text = currentAddress.StateProvinceRegion;
                    txtOtherAddressPostalCode.Text = currentAddress.PostalCode;
                    txtOtherAddressCountry.Text = currentAddress.Country;
                }
            }
        }

        protected void btnAddAddress_Click(object sender, EventArgs e)
        {
            ManageAddress(0);
        }

        protected void btnAddressListRefresh_Click(object sender, EventArgs e)
        {
            LoadExhibitorAddresses();
        }

        protected void btnCancelManageAddress_Click(object sender, EventArgs e)
        {
            ManageExhibitor(CurrentExhibitorId);
        }

        protected void grdvwAddressList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Address address = (Address)e.Row.DataItem;

                LinkButton lbtnDeleteAddress = (LinkButton)e.Row.FindControl("lbtnDeleteAddress");
                Literal ltrOtherFullAddress = (Literal)e.Row.FindControl("ltrOtherFullAddress");
                Label lblAddressType = (Label)e.Row.FindControl("lblAddressType");
                

                lbtnDeleteAddress.Attributes.Add("OnClick", "return confirm('Sure you want to delete this address?');");

                string addressType = address.AddressTypeCd;
                if (address.AddressTypeCd == "Outbound")
                {
                    addressType = "Outbound Shipping";
                }
                lblAddressType.Text = addressType;

                StringBuilder fullAddress = new StringBuilder();
                if(!string.IsNullOrEmpty(address.Street1))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street1, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street2))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street2, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street3))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street3, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street4))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street4, "<br/>"));
                }
                if (!string.IsNullOrEmpty(address.Street5))
                {
                    fullAddress.Append(string.Format("{0}{1}", address.Street5, "<br/>"));
                }

                //string.Concat(address.Street1, "<br/>", address.Street2, "<br/>", address.Street3, "<br/>", address.Street4, "<br/>", address.Street5).Trim());
                if (string.Concat(address.City + address.StateProvinceRegion + address.PostalCode).Trim().Length > 0)
                {
                    fullAddress.Append(string.Format("{0}, {1}  {2}", address.City, address.StateProvinceRegion, address.PostalCode));
                }

                if (!string.IsNullOrEmpty(address.Country))
                {
                    fullAddress.Append(address.Country);
                }

                ltrOtherFullAddress.Text = fullAddress.ToString();

            }
        }

        protected void grdvwAddressList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int addressId = Util.ConvertInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ManageAddress":
                    ManageAddress(addressId);
                    break;
                case "DeleteAddress":
                    DeleteAddress(addressId);
                    break;
            }
        }

        private void LoadExhibitorAddresses()
        {
            plcExhibitorUserList.Visible = false;
            plcManageExhibitor.Visible = false;
            
            plcManageCreditCards.Visible = false;

            plcManageAddresses.Visible = true;
            plcAddressList.Visible = true;
            plcAddressDetail.Visible = false;
            BindExhibitorAddressList();

        }

        private void BindExhibitorAddressList()
        {
            plcAddressList.Visible = true;
            grdvwAddressList.DataSource = OwnerAdminMgr.GetExhibitorById(CurrentExhibitorId).Addresses.Where(a => a.ActiveFlag == true && a.AddressTypeCd == "Outbound");
            grdvwAddressList.DataBind();
        }


        protected void btnSaveAddress_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "AddressInformation";

            if (Page.IsValid)
            {
                Address address = new Address()
                {
                    AddressId = Util.ConvertInt32(hdnAddressId.Value),
                    Street1 = txtOtherAddressLine1.Text.Trim(),
                    Street2 = txtOtherAddressLine2.Text.Trim(),
                    Street3 = txtOtherAddressLine3.Text.Trim(),
                    Street4 = txtOtherAddressLine4.Text.Trim(),
                    //Street5 = txtOtherAddressLine5.Text.Trim(),
                    City = txtOtherAddressCity.Text.Trim(),
                    StateProvinceRegion = txtOtherAddressState.Text.Trim(),
                    PostalCode = txtOtherAddressPostalCode.Text.Trim(),
                    Country = txtOtherAddressCountry.Text.Trim(),
                    ActiveFlag = true,
                    ExhibitorId = CurrentExhibitorId,
                    AddressTypeCd = ddlOtherAddressType.SelectedValue

                };
                ValidationResults errors = this.OwnerAdminMgr.SaveExhibitorAddress(CurrentUser, address);

                if (errors.IsValid)
                {
                    LoadExhibitorAddresses();
                    this.Master.DisplayFriendlyMessage("Address saved.");
                }
                else
                {
                    PageErrors.AddErrorMessages(errors, PageErrors.ValidationGroup);
                }
            }
        }

        protected void btnCancelSaveAddress_Click(object sender, EventArgs e)
        {
            LoadExhibitorAddresses();
        }

        #endregion


        #region Manage Credit Cards

        private void LoadExhibitorCreditCards()
        {
            plcExhibitorUserList.Visible = false;
            plcManageExhibitor.Visible = false;
            plcManageAddresses.Visible = false;
            plcManageCreditCards.Visible = true;

            plcCreditCardList.Visible = true;
            BindCreditCardList();

            plcCreditCardDetail.Visible = false;
            btnAddCreditCard.Visible = true;
        }

        private void BindCreditCardList()
        {
            AccountController AccountMgr = new AccountController(CurrentUser);
            this.rptCreditCardList.Visible = true;
            this.rptCreditCardList.DataSource = AccountMgr.GetCreditCardListByExhibitor(CurrentExhibitorId, true).ToList().Where(cc => cc.DeletedFlag == null || cc.DeletedFlag == false);
            this.rptCreditCardList.DataBind();
        }

        protected void btnCancelCreditCard_Click(object sender, EventArgs e)
        {
            LoadExhibitorCreditCards();
        }

        protected void btnCancelCreditCardDetail_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ReturnPage) && ReturnPage == "Payments")
            {
                LinkToPaymentDetail(CurrentExhibitorId);
            }
            else
            {
                ManageExhibitor(CurrentExhibitorId);
            }
        }

        protected void btnAddCreditCard_Click(object sender, EventArgs e)
        {
            this.btnAddCreditCard.Visible = false;
            this.plcCreditCardDetail.Visible = true;
            this.plcCreditCardList.Visible = false;

            this.LoadCreditCardTypes();
            this.LoadCreditCardExpirationOptions();

            this.ClearPlaceHolderControl(plcCreditCardDetail);

            hdnCreditCardId.Value = "0";
            hdnCreditCardAddressId.Value = "0";

            Exhibitor exhibitor = OwnerAdminMgr.GetExhibitorById(CurrentExhibitorId);
            if (exhibitor != null && !string.IsNullOrEmpty(exhibitor.PrimaryEmailAddress))
            {
                txtCreditCardEmail.Text = exhibitor.PrimaryEmailAddress;
            }
            
        }

        private void ClearPlaceHolderControl(PlaceHolder placeHolderControl)
        {
            foreach (Control control in placeHolderControl.Controls)
            {
                if (control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    textBox.Text = string.Empty;
                }
                else if (control is DropDownList)
                {
                    DropDownList dropDownList = (DropDownList)control;
                    dropDownList.SelectedIndex = 0;
                }
            }

        }

        protected void btnSaveCreditCard_Click(object sender, EventArgs e)
        {
            PageErrors.ValidationGroup = "CreditCardInfo";
            if (txtCreditCardNumber.Text.Contains("*"))
            {
                ValidationResult error = new ValidationResult("The card number is invalid", null, null, null, null);
                PageErrors.AddErrorMessage(error, PageErrors.ValidationGroup);
            }

            if (Page.IsValid)
            {
                SaveCreditCard();

                if (Page.IsValid)
                {
                    if (!string.IsNullOrEmpty(ReturnPage) && ReturnPage == "Payments")
                    {
                        LinkToPaymentDetail(CurrentExhibitorId);
                    }
                    else
                    {
                        this.Master.DisplayFriendlyMessage("Credit card saved.");

                        BindCreditCardList();
                        plcCreditCardDetail.Visible = false;
                        plcCreditCardList.Visible = true;
                        btnAddCreditCard.Visible = true;
                    }
                }
            }
        }

        protected void rptCreditCardList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CreditCard card = (CreditCard)e.Item.DataItem;

                HtmlTableRow trCreditCard = (HtmlTableRow)e.Item.FindControl("trCreditCard");

                trCreditCard.Attributes["class"] = (e.Item.ItemType == ListItemType.AlternatingItem) ? "altItem" : "item";

                LinkButton lnkEditCardName = (LinkButton)e.Item.FindControl("lnkEditCardName");
                Literal ltrCreditCardNumber = (Literal)e.Item.FindControl("ltrCreditCardNumber");
                Literal ltrCreditCardExpirationDate = (Literal)e.Item.FindControl("ltrCreditCardExpirationDate");
                Literal ltrCreditCardEmailAddress = (Literal)e.Item.FindControl("ltrCreditCardEmailAddress");

                LinkButton btnEditCard = (LinkButton)e.Item.FindControl("btnEditCard");
                LinkButton btnDeleteCard = (LinkButton)e.Item.FindControl("btnDeleteCard");
                btnEditCard.CommandArgument = card.CreditCardId.ToString();
                lnkEditCardName.CommandArgument = card.CreditCardId.ToString();
                btnDeleteCard.CommandArgument = card.CreditCardId.ToString();

                btnDeleteCard.Attributes.Add("OnClick", "return confirm('Are you sure you want to delete this card?');");

                lnkEditCardName.Text = card.NameOnCard;
                ltrCreditCardNumber.Text = card.CardNumberMasked;
                ltrCreditCardExpirationDate.Text = card.ExpirationMonth + "/" + card.ExpirationYear;
                ltrCreditCardEmailAddress.Text = card.EmailAddress;
            }
        }

        protected void btnEdit_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EditCard")
            {
                LinkButton btnEdit = (LinkButton)e.CommandSource;

                EditCreditCard(int.Parse(btnEdit.CommandArgument));

            }
            if (e.CommandName == "DeleteCard")
            {
                LinkButton btnDelete = (LinkButton)e.CommandSource;

                DeleteCreditCard(int.Parse(btnDelete.CommandArgument));
            }
        }

        private void EditCreditCard(int creditCardId)
        {
            PopulateCreditCardDetails(creditCardId);
            plcCreditCardList.Visible = false;
            plcCreditCardDetail.Visible = true;
        }

        private void DeleteCreditCard(int currentCardId)
        {
            AccountController AccountMgr = new AccountController(CurrentUser);
            AccountMgr.DeleteCreditCardById(currentCardId, true);
            BindCreditCardList();
            this.Master.DisplayFriendlyMessage("Credit card deleted.");

        }

        private void PopulateCreditCardDetails(int currentCardId)
        {
            this.LoadCreditCardTypes();
            this.LoadCreditCardExpirationOptions();

            AccountController AccountMgr = new AccountController(CurrentUser);

            CreditCard currentCard = AccountMgr.GetCreditCardById(currentCardId);
            hdnCreditCardId.Value = currentCard.CreditCardId.ToString();

            txtCreditCardName.Text = currentCard.NameOnCard;
            txtCreditCardNumber.Text = currentCard.CardNumberDecrypted;

            hdnCreditCardAddressId.Value = currentCard.Address.AddressId.ToString();
            txtCreditCardAddressLine1.Text = currentCard.Address.Street1;
            txtCreditCardAddressLine2.Text = currentCard.Address.Street2;
            txtCreditCardCity.Text = currentCard.Address.City;
            txtCreditCardState.Text = currentCard.Address.StateProvinceRegion;

            txtCreditCardPostalCode.Text = currentCard.Address.PostalCode;
            txtCreditCardCountry.Text = currentCard.Address.Country;
            txtCreditCardEmail.Text = currentCard.EmailAddress;

            txtCreditCardSecurityCode.Text = currentCard.SecurityCodeDecrypted;

            WebUtil.SelectListItemByValue(ddlCreditCardType, currentCard.CreditCardTypeCd);
            WebUtil.SelectListItemByValue(ddlCreditCardExpMonth, currentCard.ExpirationMonth);
            WebUtil.SelectListItemByValue(ddlCreditCardExpYear, currentCard.ExpirationYear);

        }


        private CreditCard BuildCreditCard()
        {
            CreditCard creditCard = new CreditCard();

            creditCard.ExhibitorId = CurrentExhibitorId;

            creditCard.CreditCardId = Int32.Parse(hdnCreditCardId.Value);
            creditCard.NameOnCard = txtCreditCardName.Text.Trim();
            creditCard.SetCreditCardNumber(txtCreditCardNumber.Text.Trim());
            creditCard.CreditCardTypeCd = ddlCreditCardType.SelectedValue;
            creditCard.ExpirationMonth = ddlCreditCardExpMonth.SelectedValue;
            creditCard.ExpirationYear = ddlCreditCardExpYear.SelectedValue;
            creditCard.SetCreditCardSecurityCode(txtCreditCardSecurityCode.Text.Trim());
            creditCard.EmailAddress = txtCreditCardEmail.Text.Trim();

            return creditCard;
        }

        private Address BuildCreditBillingAddress()
        {
            Address billingAddress = new Address();
            billingAddress.AddressId = Int32.Parse(hdnCreditCardAddressId.Value);
            billingAddress.Street1 = txtCreditCardAddressLine1.Text.Trim();
            billingAddress.Street2 = txtCreditCardAddressLine2.Text.Trim();
            billingAddress.City = txtCreditCardCity.Text.Trim();
            billingAddress.StateProvinceRegion = txtCreditCardState.Text.Trim();
            billingAddress.PostalCode = txtCreditCardPostalCode.Text.Trim();
            billingAddress.Country = txtCreditCardCountry.Text.Trim();
            return billingAddress;
        }

        private void SaveCreditCard()
        {
            CreditCard creditCard = BuildCreditCard();
            Address billingAddress = BuildCreditBillingAddress();

            AccountController AccountMgr = new AccountController(CurrentUser);
            ValidationResults creditCardErrors = AccountMgr.StoreCreditCard(CurrentUser, creditCard, billingAddress, true);

            if (!creditCardErrors.IsValid)
            {
                PageErrors.AddErrorMessages(creditCardErrors, PageErrors.ValidationGroup);
            }

        }


        private void LoadCreditCardTypes()
        {
            ddlCreditCardType.Items.Clear();
            AccountController AccountMgr = new AccountController(CurrentUser);
            ddlCreditCardType.DataSource = AccountMgr.GetCreditCardTypesList(CurrentUser.CurrentShow.MerchantAccountConfigId);
            ddlCreditCardType.DataTextField = "Name";
            ddlCreditCardType.DataValueField = "CreditCardTypeCd";
            ddlCreditCardType.DataBind();
            ddlCreditCardType.Items.Insert(0, new ListItem { Text = "-- Select One --", Value = "-1" });
        }

        private void LoadCreditCardExpirationOptions()
        {
            this.ddlCreditCardExpMonth.Items.Clear();
            for (int iMonth = 1; iMonth <= 12; iMonth++)
            {
                this.ddlCreditCardExpMonth.Items.Add(new ListItem(iMonth.ToString().PadLeft(2, '0'), iMonth.ToString()));
            }

            this.ddlCreditCardExpYear.Items.Clear();
            for (int iYear = 0; iYear <= 10; iYear++)
            {
                int yearItem = DateTime.Now.Year + iYear;
                this.ddlCreditCardExpYear.Items.Add(new ListItem(yearItem.ToString(), yearItem.ToString()));
            }
        }

        private int CurrentExhibitorId
        {
            get { return Util.ConvertInt32(hdnExhibitorId.Value); }
            set { hdnExhibitorId.Value = value.ToString(); }
        }
        #endregion


    }
}