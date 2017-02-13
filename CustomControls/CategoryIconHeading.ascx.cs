using ExpoOrders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ExpoOrders.Web.CustomControls
{
    public partial class CategoryIconHeading : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        List<CategoryIconItem> _IconList = new List<CategoryIconItem>();

        public void AddCategoryIcon(int categoryId, string categoryText, string iconFileName)
        {
            _IconList.Add(new CategoryIconItem(categoryId, categoryText, iconFileName));
        }

        ExpoOrdersUser _CurrentUser;
        public void FinalizeList(ExpoOrdersUser currentUser)
        {
            _CurrentUser = currentUser;
            this.rptrCategoryIcons.DataSource = _IconList;
            this.rptrCategoryIcons.DataBind();
        }


        public string Heading
        {
            set { ltrCategoryHeading.Text = value;  }
            get { return ltrCategoryHeading.Text;  }
        }

        
        protected void rptrCategoryIcons_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                HtmlAnchor lnkViewCategory = (HtmlAnchor)e.Item.FindControl("lnkViewCategory");
                Image imgCategoryIcon = (Image)e.Item.FindControl("imgCategoryIcon");
                Literal ltrCategoryText = (Literal)e.Item.FindControl("ltrCategoryText");

                CategoryIconItem categoryIcon = (CategoryIconItem) e.Item.DataItem;

                if(categoryIcon != null && !string.IsNullOrEmpty(categoryIcon.IconFileName))
                {
                    lnkViewCategory.HRef = string.Format("~/Exhibitors/Booth.aspx?viewcat={0}&parent={1}", categoryIcon.CategoryId.ToString(), Server.UrlEncode(this.Heading));

                    imgCategoryIcon.Visible = true;
                    imgCategoryIcon.ImageUrl = string.Format("~/Assets/Shows/{0}/{1}", _CurrentUser.CurrentShow.ShowGuid.ToString(), categoryIcon.IconFileName);
                    imgCategoryIcon.AlternateText = categoryIcon.IconFileName;

                    ltrCategoryText.Text = categoryIcon.CategoryText;
                }
            }
        }
    }

    public class CategoryIconItem
    {
        public int CategoryId { get; set; }
        public string CategoryText { get; set; }
        public string IconFileName { get; set; }

        public CategoryIconItem(int categoryId, string text, string iconFileName)
        {
            CategoryId = categoryId;
            CategoryText = text;
            IconFileName = iconFileName;
        }
    }
}