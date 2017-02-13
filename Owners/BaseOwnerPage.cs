using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using ExpoOrders.Entities;
using ExpoOrders.Common;
using ExpoOrders.Controllers;
using AjaxControlToolkit;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Telerik.Web.UI;
using System.Configuration;
using System.IO;

namespace ExpoOrders.Web.Owners
{
    public class BaseOwnerPage : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Context != null && Context.Session != null)
            {
                if (!CurrentUser.IsOwner)
                {
                    RedirectToPage(PageRedirect.UserLogin);
                }
            }
        }

        public void LoadExhibitorSearchList(RadComboBox ddl)
        {
            if (ddl != null)
            {
                ddl.ClearSelection();
                OwnerAdminController ownerCntrl = new OwnerAdminController();
                ddl.DataSource = ownerCntrl.GetExhibitors(this.CurrentUser.CurrentShow.ShowId, false);
                ddl.DataTextField = "ExhibitorCompanyName";
                ddl.DataValueField = "ExhibitorId";
                ddl.DataBind();
                ddl.Items.Insert(0, new RadComboBoxItem { Text = "-- All --", Value = "0" });
                ddl.SelectedIndex = 0;
            }
            
        }

        public string SaveFile(FileUpload fileUploader)
        {
            string storagePath = string.Format("{0}{1}", Server.MapPath("~/Assets/Shows/"), this.CurrentUser.CurrentShow.ShowGuid.ToString());
            FileInfo fileInfo = new FileInfo(fileUploader.PostedFile.FileName);
            string fileName = Util.CleanFileName(fileInfo.Name);
            fileUploader.SaveAs(storagePath + "\\" + fileName);
            return fileName;
        }

        public void PreviewShow(int showId)
        {
            DummyUpPreviewExhibitor(showId);
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowPreview", string.Format("<script language='javascript'>launchShowPreview({0});</script>", showId));
        }

        public void LaunchEmailEditor(ContentTypeEnum emailType, bool preserveAttachmentList)
        {
            if (!preserveAttachmentList)
            {
                this.CurrentUser.EmailTransportObject.ClearAttachmentList();
            }

            this.CurrentUser.EmailTransportObject.EmailType = emailType;
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EmailEditor", "<script language='javascript'>launchEmailEditor();</script>");
        }

        public void LaunchEmailLogViewer(string userId, int exhibitorId)
        {
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EmailLog", string.Format("<script language='javascript'>launchEmailLog('{0}', {1});</script>", userId, exhibitorId));
        }

        public void LaunchCallLogViewer(string userId, int exhibitorId)
        {
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CallLog", string.Format("<script language='javascript'>launchCallLog('{0}', {1});</script>", userId, exhibitorId));
        }

        public void DummyUpPreviewExhibitor(int showId)
        {
            Exhibitor dummyExhibitor = new Exhibitor();

            dummyExhibitor.BoothDescription = "Preview Sample";
            dummyExhibitor.BoothNumber = "12345";
            dummyExhibitor.ActiveFlag = true;
            dummyExhibitor.IsPreviewMode = true;
            dummyExhibitor.ExhibitorId = FindDummyExhibitorId(showId);
            dummyExhibitor.Address = new Address() { Street1 = "123 Some Lane", Street2 = "Apt 107", Street3="c/o Dummy Co.", City = "Big Town", StateProvinceRegion = "A State", PostalCode="12345", Country = "USA" };

            CurrentUser.IsExhibitor = true;
           
            //Reset the current show to force a data refresh of the links
            LoginController loginCntrl = new LoginController(CurrentUser);

            CurrentUser.CurrentShow = loginCntrl.GetShowById(showId);

            this.CurrentUser.CurrentExhibitor = dummyExhibitor;
        }

        private int FindDummyExhibitorId(int showId)
        {
            int dummyExhibitorId = 0;
            OwnerAdminController cntrl = new OwnerAdminController();

            Exhibitor dummyExhibitor = cntrl.FindDummyExhibitor(showId);

            if (dummyExhibitor != null)
            {
                dummyExhibitorId = dummyExhibitor.ExhibitorId;
            }
            return dummyExhibitorId;
        }

        public void LinkToExhibitorDetail(int exhibitorId)
        {
            LinkToExhibitorId = exhibitorId;
            Server.Transfer("Exhibitors.aspx", false);
        }

        public void LinkToOrderDetail(int orderId)
        {
            LinkToOrderId = orderId;
            Server.Transfer("Orders.aspx", false);
        }

        public void LinkToPaymentDetail(int exhibitorId)
        {
            ReturnPage = null;
            LinkToExhibitorId = exhibitorId;
            Server.Transfer("Payments.aspx", false);
        }

        public string JumpMode
        {
            get
            {
                string jump = string.Empty;

                if (HttpContext.Current.Items.Contains("JumpMode"))
                {
                    jump = HttpContext.Current.Items["JumpMode"].ToString();
                }
                return jump;
            }
            set
            {
                HttpContext.Current.Items.Add("JumpMode", value);
            }
        }

        public string ReturnPage
        {
            get
            {
                string returnPage = string.Empty;

                if (HttpContext.Current.Session["ReturnPage"] != null)
                {
                    returnPage = HttpContext.Current.Session["ReturnPage"].ToString();
                }
                return returnPage;
            }
            set
            {
                HttpContext.Current.Session["ReturnPage"] = value;
            }
        }

        public int LinkToOrderId
        {
            get
            {
                int orderId = 0;

                if (HttpContext.Current.Items.Contains("OrderId"))
                {
                    orderId = Util.ConvertInt32(HttpContext.Current.Items["OrderId"]);
                }
                return orderId;
            }
            set
            {
                HttpContext.Current.Items.Add("OrderId", value);
            }
        }

        public int LinkToExhibitorId
        {
            get
            {
                int exhibitorId = 0;

                if (HttpContext.Current.Items.Contains("ExhibitorId"))
                {
                    exhibitorId = Util.ConvertInt32(HttpContext.Current.Items["ExhibitorId"]);
                }
                return exhibitorId;
            }
            set
            {
                HttpContext.Current.Items.Add("ExhibitorId", value);
            }
        }

        public ValidationResults SendExhibitorInvoice(ExpoOrdersUser currentUser, int exhibitorId)
        {
            int showId = CurrentUser.CurrentShow.ShowId;
            Guid showGuid = CurrentUser.CurrentShow.ShowGuid;

            ReportController reportCtrl = new ReportController(ReportControllerType.Remote);
            reportCtrl.SetCurrentServerReport(new Microsoft.Reporting.WebForms.ServerReport());
            reportCtrl.PrepareServerReport(ReportEnum.ExhibitorInvoice);

            List<ReportParameter> reportParams = new List<ReportParameter>();
            reportParams.Add(new ReportParameter("ExhibitorId", exhibitorId.ToString()));
            reportParams.Add(new ReportParameter("ShowId", showId.ToString()));
            reportCtrl.SetReportParameters(reportParams);

            string invoiceOutputFileName = string.Format("ExhibitorInvoice_{0}_{1}.pdf", exhibitorId, DateTime.Now.Ticks);
            string invoiceOutputFilePath = Server.MapPath(string.Format("~/Assets/Shows/{0}/Attachments/{1}", showGuid, invoiceOutputFileName));

            ValidationResults errors = reportCtrl.RenderReportToPDFFile(invoiceOutputFilePath);

            if (errors.IsValid)
            {
                EmailTransportEntity emailTransportObject = this.CurrentUser.EmailTransportObject == null ? new EmailTransportEntity() : this.CurrentUser.EmailTransportObject;
                if (emailTransportObject.SelectedExhibitorIds != null)
                {
                    emailTransportObject.ClearSelectedExhibitors();
                    emailTransportObject.SelectedExhibitorIds.Add(exhibitorId);
                }
                else
                {
                    emailTransportObject.SelectedExhibitorIds = new List<int>();
                    emailTransportObject.SelectedExhibitorIds.Add(exhibitorId);
                }

                emailTransportObject.EmailType = ContentTypeEnum.InvoiceEmail;

                if (emailTransportObject.AttachmentList != null)
                {
                    emailTransportObject.ClearAttachmentList();
                    emailTransportObject.AttachmentList.Add(invoiceOutputFilePath);
                }
                else
                {
                    emailTransportObject.AttachmentList = new List<string>();
                    emailTransportObject.AttachmentList.Add(invoiceOutputFilePath);
                }

                this.CurrentUser.EmailTransportObject = emailTransportObject;
                LaunchEmailEditor(emailTransportObject.EmailType, true);
            }

            return errors;
        }

        public ValidationResults SendExhibitorOrderReceipt(ExpoOrdersUser currentUser, int exhibitorId, int orderId)
        {
            int showId = CurrentUser.CurrentShow.ShowId;
            Guid showGuid = CurrentUser.CurrentShow.ShowGuid;

            ReportController reportCtrl = new ReportController(ReportControllerType.Remote);
            reportCtrl.SetCurrentServerReport(new Microsoft.Reporting.WebForms.ServerReport());
            reportCtrl.PrepareServerReport(ReportEnum.OrderReceipt);

            List<ReportParameter> reportParams = new List<ReportParameter>();
            reportParams.Add(new ReportParameter("ShowId", showId.ToString()));
            reportParams.Add(new ReportParameter("ExhibitorId", exhibitorId.ToString()));
            reportParams.Add(new ReportParameter("OrderId", orderId.ToString()));
            reportCtrl.SetReportParameters(reportParams);

            string invoiceOutputFileName = string.Format("OrderReceipt_{0}_{1}.pdf", exhibitorId, DateTime.Now.Ticks);
            string invoiceOutputFilePath = Server.MapPath(string.Format("~/Assets/Shows/{0}/Attachments/{1}", showGuid, invoiceOutputFileName));

            ValidationResults errors = reportCtrl.RenderReportToPDFFile(invoiceOutputFilePath);

            if (errors.IsValid)
            {
                EmailTransportEntity emailTransportObject = this.CurrentUser.EmailTransportObject == null ? new EmailTransportEntity() : this.CurrentUser.EmailTransportObject;
                if (emailTransportObject.SelectedExhibitorIds != null)
                {
                    emailTransportObject.ClearSelectedExhibitors();
                    emailTransportObject.SelectedExhibitorIds.Add(exhibitorId);
                }
                else
                {
                    emailTransportObject.SelectedExhibitorIds = new List<int>();
                    emailTransportObject.SelectedExhibitorIds.Add(exhibitorId);
                }

                emailTransportObject.EmailType = ContentTypeEnum.OrderConfirmation;

                if (emailTransportObject.AttachmentList != null)
                {
                    emailTransportObject.ClearAttachmentList();
                    emailTransportObject.AttachmentList.Add(invoiceOutputFilePath);
                }
                else
                {
                    emailTransportObject.AttachmentList = new List<string>();
                    emailTransportObject.AttachmentList.Add(invoiceOutputFilePath);
                }

                this.CurrentUser.EmailTransportObject = emailTransportObject;
                LaunchEmailEditor(emailTransportObject.EmailType, true);
            }

            return errors;
        }


        public string StageExhibitorInvoice(Show show, int exhibitorId, UserContainer targetUser)
        {
            string exhibitorInvoiceStagingPath = Server.MapPath(string.Format("~/Assets/Shows/{0}/Attachments/", show.ShowGuid));

            ReportController reportCtrl = new ReportController(ReportControllerType.Remote);
            reportCtrl.SetCurrentServerReport(new Microsoft.Reporting.WebForms.ServerReport());
            reportCtrl.PrepareServerReport(ReportEnum.ExhibitorInvoice);

            List<ReportParameter> reportParams = new List<ReportParameter>();
            reportParams.Add(new ReportParameter("ExhibitorId", exhibitorId.ToString()));
            reportParams.Add(new ReportParameter("ShowId", show.ShowId.ToString()));
            reportCtrl.SetReportParameters(reportParams);

            string invoiceOutputFileName = string.Format("ExhibitorInvoice_{0}_{1}.pdf", exhibitorId, targetUser.UserId);
            string invoiceOutputFilePath = exhibitorInvoiceStagingPath + invoiceOutputFileName;

            ValidationResults errors = reportCtrl.RenderReportToPDFFile(invoiceOutputFilePath);

            if (!errors.IsValid)
            {
                exhibitorInvoiceStagingPath = string.Empty;
            }

            return exhibitorInvoiceStagingPath;
        }


        public bool UserHasRole(UserRoleEnum role)
        {
           return Roles.IsUserInRole(role.GetCodeValue());
        }

        public void TransferToShowPage(int showId, OwnerPage page)
        {
            if (SetShowContext(showId))
            {
                Server.Transfer(page.GetCodeValue());
            }
        }

        public bool SetShowContext(int showId)
        {
            bool showSet = false;
            ShowController showCntrl = new ShowController();
            Show selectedShow = showCntrl.GetShowById(showId);
            if (selectedShow != null)
            {
                ExpoOrdersUser user = this.CurrentUser;
                user.CurrentShow = selectedShow;
                Session["CurrentUser"] = user;
                showSet = true;
            }
            return showSet;
        }

        public void ConfigureImageManager(RadEditor htmlContentEditor, Guid showGuid, bool allowUploads, bool allowDeletes)
        {
            //htmlContentEditor.ImageManager.SearchPatterns = new string[] { "*.*" };

            htmlContentEditor.DocumentManager.ViewPaths = new string[] { string.Format("~/Assets/Shows/{0}/", showGuid) };
            htmlContentEditor.ImageManager.ViewPaths = new string[] { string.Format("~/Assets/Shows/{0}/", showGuid) };

            if (allowDeletes)
            {
                htmlContentEditor.ImageManager.DeletePaths = new string[] { string.Format("~/Assets/Shows/{0}/", showGuid) };
                htmlContentEditor.DocumentManager.DeletePaths = new string[] { string.Format("~/Assets/Shows/{0}/", showGuid) };
            }

            if (allowUploads)
            {
                htmlContentEditor.ImageManager.UploadPaths = new string[] { string.Format("~/Assets/Shows/{0}/", showGuid) };
                htmlContentEditor.ImageManager.MaxUploadFileSize = Util.ConvertInt32(ConfigurationManager.AppSettings["MaxUploadFileSizeBytes"]);
            }
        }

        public void ResetClassificationDropDown(PlaceHolder plcClassification, RadComboBox ddlClassification)
        {
            if (ddlClassification != null)
            {
                ddlClassification.Visible = false;
                plcClassification.Visible = false;
                ddlClassification.SelectedValue = string.Empty;
                ddlClassification.Items.Clear();

                if (!string.IsNullOrEmpty(CurrentUser.CurrentOwner.ClassificationList))
                {
                    bool showingClassification = false;
                    foreach (string item in Util.ParseDelimitedList(CurrentUser.CurrentOwner.ClassificationList, ';'))
                    {
                        var itemVal = item.Trim();
                        if (!string.IsNullOrEmpty(itemVal))
                        {
                            ddlClassification.Items.Add(new Telerik.Web.UI.RadComboBoxItem(itemVal, itemVal));
                            showingClassification = true;
                        }
                    }

                    if (showingClassification)
                    {
                        ddlClassification.Items.Insert(0, new RadComboBoxItem("-- Classification --", string.Empty));
                        ddlClassification.Visible = true;
                        plcClassification.Visible = true;
                    }
                }
            }
        }

        public void SelectClassification(RadComboBox ddlClassification, string valueToSelect)
        {
            if (!string.IsNullOrEmpty(valueToSelect))
            {
                //Add a new item if the current selected value doesn't exist
                if(ddlClassification.Items.FindItemByValue(valueToSelect) == null)
                {
                    ddlClassification.Items.Add(new RadComboBoxItem(valueToSelect, valueToSelect));
                }

                ddlClassification.Items.FindItemByValue(valueToSelect).Selected = true;
            }
        }

    }
}