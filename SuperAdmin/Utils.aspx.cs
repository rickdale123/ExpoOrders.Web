using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Controllers;
using ExpoOrders.Entities;
using ExpoOrders.Common;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace ExpoOrders.Web.SuperAdmin
{
    public partial class Utils : BaseSuperAdminPage
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
            plcFunctions.Visible = true;
            LoadDropDowns();
        }

        private void LoadDropDowns()
        {
            ddlShowId.Items.Clear();
            ddlOwnerId.Items.Clear();

            List<Show> shows = this.Cntrl.GetAllShows();

            shows.OrderBy(s => s.ShowId).ToList().ForEach(s =>
            {
                ddlShowId.Items.Add(new ListItem(string.Format("{0} - {1} ({2} - {3})", s.ShowId, s.ShowName, s.Owner.OwnerId, s.Owner.OwnerName), s.ShowId.ToString()));
            });

            ddlShowId.Visible = true;

            List<Owner> owners = this.Cntrl.GetOwnerList();
           
            owners.OrderBy(o => o.OwnerName).ToList().ForEach( o => {
                ddlOwnerId.Items.Add(new ListItem(string.Format("{0} - {1}", o.OwnerId, o.OwnerName), o.OwnerId.ToString()));
            });

            ddlOwnerId.Visible = true;   
        }


        protected void btnUpdateShowOwner_Click(object sender, EventArgs e)
        {
            this.Cntrl.ChangeShowOwner(Util.ConvertInt32(ddlShowId.SelectedValue), Util.ConvertInt32(ddlOwnerId.SelectedValue));

            Master.DisplayFriendlyMessage("Show Owner Updated.");
            LoadDropDowns();
        }

        protected void btnThrowException_Click(object sender, EventArgs e)
        {
            throw new Exception("Just testing the Exception handler");
        }
    }
}