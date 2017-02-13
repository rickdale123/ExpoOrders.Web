using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using System.IO;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class Archive : BaseSuperAdminPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }

        }

        private void InitializePage()
        {
            plcShowDetail.Visible = false;
            txtShowId.Text = string.Empty;


            if (Request.QueryString["showid"] != null)
            {
                int showId = Util.ConvertInt32(Request.QueryString["showid"]);
                txtShowId.Text = showId.ToString();
                LoadShowDetails(showId);
            }

        }

        private void LoadShowDetails(int showId)
        {
            if (showId > 0)
            {
                plcShowDetail.Visible = true;
                SuperAdminController cntrl = new SuperAdminController();
                Show show = cntrl.GetAllShows().Where(s => s.ShowId == showId).FirstOrDefault();
                if (show != null)
                {
                    string assetsRootPath = Server.MapPath(string.Format("~/Assets/Shows/{0}/", show.ShowGuid.ToString()));
                    lblGuidLocation.Text = assetsRootPath;
                    lblUploadSize.Text = DetermineFolderSize(Server.MapPath(string.Format("~/Uploads/Shows/{0}/", show.ShowGuid.ToString())), SearchOption.TopDirectoryOnly);

                    lblAssetsRootSize.Text = DetermineFolderSize(assetsRootPath, SearchOption.TopDirectoryOnly);
                    lblAttachmentSize.Text = DetermineFolderSize(Server.MapPath(string.Format("~/Assets/Shows/{0}/Attachments/", show.ShowGuid.ToString())), SearchOption.TopDirectoryOnly);
                    lblDownloadSize.Text = DetermineFolderSize(Server.MapPath(string.Format("~/Assets/Shows/{0}/Downloads/", show.ShowGuid.ToString())), SearchOption.TopDirectoryOnly);
                    lblProductSize.Text = DetermineFolderSize(Server.MapPath(string.Format("~/Assets/Shows/{0}/Products/", show.ShowGuid.ToString())), SearchOption.TopDirectoryOnly);

                    lblTotalAssetsFolderSize.Text = DetermineFolderSize(assetsRootPath, SearchOption.AllDirectories);
                }
            }
        }

        private string DetermineFolderSize(string folderPath, SearchOption option)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            return Util.ToByteString(dirInfo.EnumerateFiles("*.*", option).Sum(fi => fi.Length));
        }

        protected void btnLoadShow_Click(object sender, EventArgs e)
        {
            LoadShowDetails(Util.ConvertInt32(txtShowId.Text.Trim()));
        }
    }
}