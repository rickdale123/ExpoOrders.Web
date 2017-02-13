using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using Telerik.Web.UI;
using ExpoOrders.Web.CustomControls;

namespace ExpoOrders.Web.Owners
{
    public partial class Default : BaseOwnerPage
    {

        #region Private Members
        private ShowController _showCtrl;
        private OwnerAdminController _ownerCtrlr;
        #endregion

        #region Public Members
        public ShowController ShowCtrl
        {
            get
            {
                if (_showCtrl == null)
                {
                    _showCtrl = new ShowController();
                }
                return _showCtrl;
            }
        }
        public OwnerAdminController OwnerCtrlr
        {
            get
            {
                if (_ownerCtrlr == null)
                {
                    _ownerCtrlr = new OwnerAdminController();
                }
                return _ownerCtrlr;
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
            this.Master.NoSubNavigation = true;
            this.Master.LoadMasterPage(CurrentUser, OwnerPage.Welcome);

            LoadShowList(false);
        }

        private void LoadShowList(bool changeSortDir, string sortExpression = null)
        {
            if (string.IsNullOrEmpty(sortExpression))
            {
                sortExpression = ReadCookieValue("ShowListSortColumn");
            }

            if (string.IsNullOrEmpty(sortExpression))
            {
                sortExpression = "StartDate";
            }
            List<Show> showList = ShowCtrl.GetOwnedShowsByUserId(CurrentUser, chkIncludeInactive.Checked);

            SortDirection sortDir = DetermineSortDirection(changeSortDir, sortExpression, "ShowListSortColumn", "ShowListSortDir");
            WebUtil.SortList(showList, sortExpression, sortDir);
            grdvwShowList.DataSource = showList;
            grdvwShowList.DataBind();

            PutSortDirectionImage(grdvwShowList, sortExpression, sortDir);

        }


        #endregion

        #region Control Events

        protected void grdvwShowList_Sorting(object sender, GridViewSortEventArgs e)
        {
            LoadShowList(true, e.SortExpression);
        }

        protected void grdvwShowList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Show currentShow = (Show)e.Row.DataItem;

                Literal ltrStartDate = (Literal)e.Row.FindControl("ltrStartDate");
                ltrStartDate.Text = string.Format("{0:MM/dd/yyyy}", currentShow.StartDate);
                Literal ltrEndDate = (Literal)e.Row.FindControl("ltrEndDate");
                ltrEndDate.Text = string.Format("{0:MM/dd/yyyy}", currentShow.EndDate);

                LinkButton lbtnDeactivateShow = (LinkButton)e.Row.FindControl("lbtnDeactivateShow");
                LinkButton lbtnActivateShow = (LinkButton)e.Row.FindControl("lbtnActivateShow");
                Button btnCopyShow = (Button)e.Row.FindControl("btnCopyShow");

                lbtnDeactivateShow.CommandArgument = lbtnActivateShow.CommandArgument = currentShow.ShowId.ToString();


                Literal ltrActive = (Literal)e.Row.FindControl("ltrActive");
                Literal ltrDisplayOnOwnerLandingPage = (Literal)e.Row.FindControl("ltrDisplayOnOwnerLandingPage");

                ltrDisplayOnOwnerLandingPage.Text = (currentShow.DisplayOnOwnerLandingPage.HasValue && currentShow.DisplayOnOwnerLandingPage.Value == true) ? "Yes" : "No";
                
                if (currentShow.ActiveFlag)
                {
                    ltrActive.Text = "Yes";
                    lbtnDeactivateShow.Visible = true;
                    lbtnDeactivateShow.Attributes.Add("onClick", "return confirm('Sure you want to remove this Show?');");
                }
                else
                {
                    ltrActive.Text = "No";
                    lbtnActivateShow.Visible = true;
                }


                if(!UserHasRole(UserRoleEnum.ShowManagement))
                {
                    btnCopyShow.Visible = 
                    lbtnDeactivateShow.Visible =
                    lbtnActivateShow.Visible = false;
                }
            }
        }

        protected void grdvwShowList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int showId = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "EditShow":
                    TransferToShowPage(showId, OwnerPage.ShowSettings);
                    break;
                case "DeactivateShow":
                    this.DeactivateShow(showId);
                    break;
                case "ActivateShow":
                    this.ActivateShow(showId);
                    break;
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadShowList(false);
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            Show newShow = null;
            int showToCopy = Util.ConvertInt32(((Button)sender).CommandArgument);
            bool isNewShow = false;
            if (showToCopy > 0)
            {
                newShow = this.ShowCtrl.CopyShow(CurrentUser, showToCopy, BuildShowShell());
            }
            else
            {
                isNewShow = true;
                newShow = this.ShowCtrl.CreateShow(BuildShowShell());
            }

            if (newShow != null && newShow.ShowId > 0 && !newShow.ShowGuid.Equals(Guid.Empty))
            {
                DateTime logStart = DateTime.Now;

                CreateShowFolders(newShow, showToCopy);
                ShowCtrl.InsertAuditLog(CurrentUser, string.Format("Create Folders for ShowId {0}", newShow.ShowId), logStart, DateTime.Now);
                if (isNewShow)
                {
                    CreateDefaultStyleSheet(newShow);
                }
                TransferToShowPage(newShow.ShowId, OwnerPage.ShowSettings);
            }
        }


        protected void btnAddShow_Click(object sender, EventArgs e)
        {
            ltrModalPopupTitle.Text = "Create a Show";
            this.btnOk.Text = "Save";
            this.btnOk.CommandArgument = string.Empty;
            this.MPE.Show();
        }

        protected void btnCopyShow_Click(object sender, EventArgs e)
        {
            ltrModalPopupTitle.Text = "Copy a Show";
            this.btnOk.Text = "Copy";
            this.btnOk.CommandArgument = ((Button)sender).CommandArgument;

            this.MPE.Show();
        }

        protected void btnCancelPopup_Click(object sender, EventArgs e)
        {
            this.btnOk.Text = "Save";
        }

        #endregion

        #region Methods

        private void CreateShowFolders(Show newShow, int showToCopy)
        {
            CreateAssetsFolder(newShow);
            CreateUploadFolder(newShow);

            if (showToCopy > 0)
            {
                Show oldShow = this.ShowCtrl.GetShowById(showToCopy);
                if (oldShow != null && !oldShow.ShowGuid.Equals(Guid.Empty))
                {
                    CopyShowFiles(oldShow.ShowGuid, newShow.ShowGuid);
                }
            }
        }

        private void CreateUploadFolder(Show newShow)
        {
            string uploadsDirectory = Server.MapPath("~/Uploads/Shows/");
            DirectoryInfo dirInfo = new DirectoryInfo(uploadsDirectory);

            if (dirInfo.Exists)
            {
                dirInfo.CreateSubdirectory(newShow.ShowGuid.ToString());
            }
        }

        private void CreateAssetsFolder(Show newShow)
        {
            string assetsDirectory = Server.MapPath("~/Assets/Shows/");
            string defaultShowAssetsDirectory = Server.MapPath("~/Assets/Shows/Default/Owner_{0}");

            DirectoryInfo assetDirectoryInfo = new DirectoryInfo(assetsDirectory);
            DirectoryInfo defaultShowAssetsDirectoryInfo = new DirectoryInfo(string.Format(defaultShowAssetsDirectory, CurrentUser.CurrentOwner.OwnerId));
            if (assetDirectoryInfo.Exists)
            {
                DirectoryInfo currentShowAssetDirectoryInfo =
                    assetDirectoryInfo.CreateSubdirectory(newShow.ShowGuid.ToString());

                //Also Create the 'Downloads' folder
                currentShowAssetDirectoryInfo.CreateSubdirectory("Downloads");
                currentShowAssetDirectoryInfo.CreateSubdirectory("Products");
                currentShowAssetDirectoryInfo.CreateSubdirectory("Attachments");

                if (currentShowAssetDirectoryInfo != null && defaultShowAssetsDirectoryInfo.Exists)
                {
                    //CurrentUser.CurrentOwner.LogoFileName
                    //Copy over all of the default assets for the given show
                    List<FileInfo> defaultFiles = defaultShowAssetsDirectoryInfo.GetFiles().Where(f =>
                            f.Name.ToLower().Contains("leftlogo")
                            || f.Name.ToLower().Contains("rightlogo")
                            || f.Name.ToLower().Contains("style")
                            || f.Name.ToLower().Contains("tab")
                            || f.Name.ToLower().Contains("navigation")
                            || f.Name.ToLower().Contains("bullet")
                        ).ToList();

                    defaultFiles.ForEach(f =>
                        {
                            f.CopyTo(string.Format("{0}/{1}", currentShowAssetDirectoryInfo.FullName, f.Name));
                        });
                }
            }
        }

        private void CopyShowFiles(Guid oldShowGuid, Guid newShowGuid)
        {
            string oldRootDirectory = Server.MapPath("~/Assets/Shows/{0}");
            string newRootDirectory = Server.MapPath("~/Assets/Shows/{0}");

            //Copy Root Files
            string oldRootFolder = string.Format(oldRootDirectory, oldShowGuid.ToString());
            string newRootFolder = string.Format(newRootDirectory, newShowGuid.ToString());
            CopyFiles(oldRootFolder, newRootFolder);

            //Copy Assets Files
            CopyFiles(oldRootFolder + "/Downloads", newRootFolder + "/Downloads");

            //Copy Product Files
            CopyFiles(oldRootFolder + "/Products", newRootFolder + "/Products");

        }

        private void CopyFiles(string sourceFolderName, string destinationFolderName)
        {
            DirectoryInfo sourceFolder = new DirectoryInfo(sourceFolderName);
            DirectoryInfo destinationFolder = new DirectoryInfo(destinationFolderName);

            if (sourceFolder != null && sourceFolder.Exists)
            {
                List<FileInfo> sourceFiles = sourceFolder.GetFiles().ToList();

                sourceFiles.ForEach(f =>
                {
                    f.CopyTo(string.Format("{0}/{1}", destinationFolder.FullName, f.Name), true);
                });
            }
        }

        private void CreateDefaultStyleSheet(Show newShow)
        {
            string newRootDirectory = Server.MapPath("~/Assets/Shows/{0}");
            string newRootFolder = string.Format(newRootDirectory, newShow.ShowGuid.ToString());
            string existingOwnerStylesheetPath = Server.MapPath(string.Format("~/Owners/Style/{0}",CurrentUser.CurrentOwner.StyleSheetFileName));

            if (File.Exists(existingOwnerStylesheetPath))
            {
                string newStyleSheetFilePath = string.Format(@"{0}\{1}", newRootFolder, "style.css");
                File.Copy(existingOwnerStylesheetPath, newStyleSheetFilePath);
            }
        }

        private Show BuildShowShell()
        {
            Show newShow = new Show();

            newShow.ActiveFlag = true;
            newShow.ShowGuid = Guid.NewGuid();
            newShow.ShowName = txtShowName.Text;

            newShow.StartDate = Util.ConvertNullDateTime(txtStartDate.Text.Trim());
            newShow.EndDate = Util.ConvertNullDateTime(txtEndDate.Text.Trim());

            //Associate this Owner with Show Record
            newShow.OwnerId = this.CurrentUser.CurrentOwner.OwnerId;
            newShow.VenueName = string.Empty;
            //Create Venue Address Shell Record
            newShow.VenueAddress = BuildAddressShell();
            //Create Warehouse Address Shell Record
            newShow.AdvanceWarehouseAddress = BuildAddressShell();
            //Create Remit Address Shell Record
            newShow.RemitAddress = BuildAddressShell();
            //Build the default Tabs
            newShow.TabLinks = BuildDefaultTabLinks();
            newShow.CreatedOn = DateTime.Now;
            newShow.ModifiedOn = DateTime.Now;
            newShow.ActiveFlag = true;
           
            return newShow;
        }

        private ICollection<TabLink> BuildDefaultTabLinks()
        {
            List<TabLink> defaultTabs = new List<TabLink>();
            int maxTabs = 5;
            for (int tabNumber = 1; tabNumber <= maxTabs; tabNumber++)
            {
                TabLink tab = new TabLink();
                tab.ActiveFlag = true;
                tab.TabNumber = tabNumber;
                tab.Visible = true;
                switch (tab.TabNumber)
                {
                    case 1:
                        tab.Text = "Show Info";
                        tab.Page = ExhibitorPageEnum.ShowInfo.GetCodeValue();
                        break;
                    case 2:
                        tab.Text = "Required Forms";
                        tab.Page = ExhibitorPageEnum.RequiredForms.GetCodeValue();
                        tab.NavigationActionId = (int)NavigationLink.NavigationalAction.SelectNavigationItem;
                        break;
                    case 3:
                        tab.Text = "My Booth";
                        tab.Page = ExhibitorPageEnum.MyBooth.GetCodeValue();
                        break;
                    case 4:
                        tab.Text = "Shipping Labels";
                        tab.Page = ExhibitorPageEnum.ShippingInfo.GetCodeValue();
                        break;
                    case 5:
                        tab.Text = "Contact Info";
                        tab.Page = ExhibitorPageEnum.ContactInfo.GetCodeValue();
                        tab.NavigationActionId = (int)NavigationLink.NavigationalAction.SelectNavigationItem;
                        break;
                }

                defaultTabs.Add(tab);
            }
            return defaultTabs;
        }

        private Address BuildAddressShell()
        {
            return new Address
            {
                ActiveFlag = true,
                City = string.Empty,
                StateProvinceRegion = string.Empty,
                Country = string.Empty,
                Street3 = string.Empty,
                Street2 = string.Empty,
                Street1 = string.Empty,
                PostalCode = string.Empty
            };
        }

        private void DeactivateShow(int showId)
        {
            if (showId > 0)
            {
                this.ShowCtrl.DeactivateShowById(showId);
                LoadShowList(false);
                this.Master.DisplayFriendlyMessage("Show de-activated.");
            }
        }

        private void ActivateShow(int showId)
        {
            if (showId > 0)
            {
                this.ShowCtrl.ActivateShowById(showId);
                LoadShowList(false);
                this.Master.DisplayFriendlyMessage("Show activated.");
            }
        }


        private string SortField
        {
            get
            {
                string sortField = ReadCookieValue("ShowListSortField");

                if (sortField == null)
                {
                    sortField = "ShowName";
                }
                return sortField;
            }
        }
        #endregion


    }
}