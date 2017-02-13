using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.IO;

using ExpoOrders.Controllers;
using ExpoOrders.Entities;


namespace ExpoOrders.Web.Exhibitors
{
    public partial class Download : BaseExhibitorPage
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
            object targetObj = Request.QueryString["target"];
            if (targetObj != null)
            {
                FileDownloadController controller = new FileDownloadController();
                FileDownload file = controller.GetFileDownloadById(Int32.Parse(targetObj.ToString()));
                                
                if (file != null && !String.IsNullOrEmpty(file.FileName))
                {
                    string relativeFilePath = string.Format("~/Assets/Shows/{0}/Downloads/{1}", base.CurrentUser.CurrentShow.ShowGuid, file.FileName);

                    Response.Redirect(relativeFilePath);
                }
            }
        }

        private void downloadFile(string fileName, string fileUrl)
        {
            Page.Response.Clear();
            bool success = ResponseFile(Page.Request, Page.Response, fileName, fileUrl, 1024000);
            if (!success)
            {
                throw new Exception("Downloading Error!");
            }
                
            Page.Response.End();

        }
        public static bool ResponseFile(HttpRequest _Request, HttpResponse _Response, string _fileName, string _fullPath, long _speed)
        {
            try
            {
                FileStream myFile = new FileStream(_fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(myFile);
                try
                {
                    _Response.AddHeader("Accept-Ranges", "bytes");
                    _Response.Buffer = false;
                    long fileLength = myFile.Length;
                    long startBytes = 0;

                    int pack = 10240; //10K bytes
                    int sleep = (int)Math.Floor((double)(1000 * pack / _speed)) + 1;
                    if (_Request.Headers["Range"] != null)
                    {
                        _Response.StatusCode = 206;
                        string[] range = _Request.Headers["Range"].Split(new char[] { '=', '-' });
                        startBytes = Convert.ToInt64(range[1]);
                    }
                    _Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                    if (startBytes != 0)
                    {
                        _Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }
                    _Response.AddHeader("Connection", "Keep-Alive");
                    _Response.ContentType = "application/octet-stream";
                    _Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(_fileName, System.Text.Encoding.UTF8));

                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    int maxCount = (int)Math.Floor((double)((fileLength - startBytes) / pack)) + 1;

                    for (int i = 0; i < maxCount; i++)
                    {
                        if (_Response.IsClientConnected)
                        {
                            _Response.BinaryWrite(br.ReadBytes(pack));
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            i = maxCount;
                        }
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    br.Close();
                    myFile.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        } 
    }
}