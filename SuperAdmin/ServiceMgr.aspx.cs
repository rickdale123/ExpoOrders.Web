using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Management;
using System.Diagnostics;
using System.Collections.Generic;
using System.ServiceProcess;
using ExpoOrders.Common;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class ServiceMgr : BaseSuperAdminPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ResetPage();
            }
        }

        private void ResetPage()
        {
            Master.ClearErrors();
            grdvServices.Visible = false;
        }

        private void LoadServiceList()
        {
            grdvServices.Visible = false;
            List<Win32ServiceEntity> svcList = GetServiceEntities();

            if (svcList != null && svcList.Count > 0)
            {
                grdvServices.Visible = true;
                grdvServices.DataSource = svcList.OrderBy(svc => svc.ServiceName);
                grdvServices.DataBind();
            }
            else
            {
                Master.AddErrorMessage("No Services to Display");
            }
        }

        private List<Win32ServiceEntity> GetServiceEntities()
        {
            List<Win32ServiceEntity> svcList = new List<Win32ServiceEntity>();

            try
            {
                ManagementObjectCollection lst = GetManagementObjects();
                
                if (lst != null)
                {
                    svcList = MapServiceList(lst);
                }
            }
            catch (Exception ex)
            {
                Master.AddErrorMessage(ex);
            }

            return svcList;

        }

        private ManagementObjectCollection GetManagementObjects()
        {
            ManagementObjectCollection lst = null;
            if (txtHostAddress.Text.Trim().ToLower() == "localhost")
            {
                ManagementPath spoolerPath = new ManagementPath("Win32_Service");
                ManagementClass servicesManager = new ManagementClass(spoolerPath);
                lst = servicesManager.GetInstances();
            }
            else
            {
                ConnectionOptions options = new ConnectionOptions();
                options.Username = txtUserId.Text.Trim();
                options.Password = txtPassword.Text.Trim();
                options.EnablePrivileges = true;
                options.Authority = "ntlmdomain:" + txtDomain.Text.Trim(); //"ntlmdomain:XTCH";

                ManagementScope scope = new ManagementScope();

                //Create the scope that will connect to the default root for WMI 
                string server = string.Format(@"\\{0}\root\CIMV2", txtHostAddress.Text.Trim());
                scope = new ManagementScope(server, options);
                scope.Connect();

                //Create a path to the services with the default options 
                ObjectGetOptions opt = new ObjectGetOptions(null, TimeSpan.MaxValue, true);
                ManagementPath spoolerPath = new ManagementPath("Win32_Service");
                ManagementClass servicesManager = new ManagementClass(scope, spoolerPath, opt);

                lst = servicesManager.GetInstances();
            }

            return lst;
        }

        private List<Win32ServiceEntity> MapServiceList(ManagementObjectCollection lst)
        {
            List<Win32ServiceEntity> svcList = new List<Win32ServiceEntity>();

            if (lst != null)
            {
                foreach (ManagementObject mo in lst)
                {
                    
                    Win32ServiceEntity entity = new Win32ServiceEntity();

                    bool addIt = true;
                    try
                    {
                        string status = mo["Started"].Equals(true) ? "Started" : "Stopped";
                        entity.ServiceName = mo["DisplayName"].ToString();
                        entity.Status = status;
                        entity.StartMode = mo["StartMode"].ToString();
                        entity.Description = mo["description"] == null ? "" : mo["description"].ToString();
                        if (!string.IsNullOrEmpty(txtFilterServiceName.Text.Trim()))
                        {
                            if (!entity.ServiceName.Contains(txtFilterServiceName.Text.Trim()))
                            {
                                addIt = false;
                            }
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        Master.AddErrorMessage(ex);
                    }

                    if (addIt)
                    {
                        svcList.Add(entity);
                    }
                    
                }
            }

            return svcList;
        }

        protected void btnLoadServiceList_Click(object sender, EventArgs e)
        {
            Master.ClearErrors();
            LoadServiceList();
        }

        protected void grdvServices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Win32ServiceEntity svcEntity = (Win32ServiceEntity)e.Row.DataItem;
                Button btnStartService = e.Row.FindControl("btnStartService") as Button;
                Button btnRestartService = e.Row.FindControl("btnRestartService") as Button;
                Button btnStopService = e.Row.FindControl("btnStopService") as Button;

                if (svcEntity.Status == "Started")
                {
                    btnStartService.Visible = false;
                    btnStopService.Visible = true;
                    btnRestartService.Visible = true;
                }
                else
                {
                    btnStartService.Visible = true;
                    btnStopService.Visible = false;
                    btnRestartService.Visible = false;
                }
            }
        }

        protected void grdvServices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Master.ClearErrors();

            string serviceName = e.CommandArgument.ToString();

            StopStartService(e.CommandArgument.ToString(), e.CommandName);

            LoadServiceList();
        }

        private void StopStartService(string serviceName, string action)
        {
            try
            {
                ServiceController svcCtrl = new ServiceController(serviceName, Environment.MachineName);
                //ManagementObjectCollection lst = GetManagementObjects();

                //if (lst != null && lst.Count > 0)
                //{
                //ManagementObject service = FindService(lst, serviceName);
                if (svcCtrl != null)
                {
                    if (action == "Start")
                    {
                        //service.InvokeMethod("StartService", null);
                        svcCtrl.Start();
                    }
                    else if (action == "Restart")
                    {
                        svcCtrl.Stop();
                        svcCtrl.Start();

                        //service.InvokeMethod("StopService", null);
                        //service.InvokeMethod("StartService", null);
                    }
                    else if (action == "Stop")
                    {
                        svcCtrl.Stop();
                        //service.InvokeMethod("StopService", null);
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                Master.AddErrorMessage(ex);
            }
        }

        private ManagementObject FindService(ManagementObjectCollection lst, string serviceName)
        {
            ManagementObject service = null;
            if (lst != null)
            {
                foreach (ManagementObject svc in lst)
                {
                    if (svc["Name"].ToString() == serviceName)
                    {
                        service = svc;
                        break;
                    }
                }
            }

            return service;
        }
    }

    [Serializable]
    public class Win32ServiceEntity
    {
        private string _serviceName = string.Empty;
        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; }
        }

        private string _status = string.Empty;
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _startMode = string.Empty;
        public string StartMode
        {
            get { return _startMode; }
            set { _startMode = value; }
        }
    }
}