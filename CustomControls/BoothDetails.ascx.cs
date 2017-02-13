using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Entities;

namespace ExpoOrders.Web.CustomControls
{
    public partial class BoothDetails : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private List<ExhibitorBoothItems> _boothItems = null;

        public void PopulateControl(ExpoOrdersUser currentUser, List<ExhibitorBoothItems> boothItems, Show currentShow)
        {
            _boothItems = boothItems;

            this.Visible = true;

            ltrBoothDescription.Text = string.Format("Booth # {0} ({1})", currentUser.CurrentExhibitor.BoothNumber, currentUser.CurrentExhibitor.BoothDescription);
            ltrBoothDescription.Visible = (currentShow.DisplayBoothNumberLabel.HasValue && currentShow.DisplayBoothNumberLabel.Value == true);

            if (boothItems != null && boothItems.Count > 0)
            {
                List<string> categories = new List<string>();

                boothItems.Sort(CategorySorted);
                boothItems.ForEach(item =>
                {
                    if (!categories.Contains(item.CategoryName))
                    {
                        categories.Add(item.CategoryName);
                    }
                }
                );

                if (categories.Count > 0)
                {
                    rptrDistinctCategories.DataSource = categories;
                    rptrDistinctCategories.DataBind();
                    rptrDistinctCategories.Visible = true;
                }
            }
        }

        protected void rptrDistinctCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rptrDistinctBoothItems = (Repeater)e.Item.FindControl("rptrDistinctBoothItems");

                string categoryName = (string) e.Item.DataItem;
                List<ExhibitorBoothItems> filteredItems = new List<ExhibitorBoothItems>();

                if (_boothItems != null && _boothItems.Count > 0)
                {
                    _boothItems.ForEach(item =>
                        {
                            if (item.CategoryName == categoryName)
                            {
                                filteredItems.Add(item);
                            }
                        });
                }

                if (filteredItems.Count > 0)
                {
                    rptrDistinctBoothItems.DataSource = filteredItems;
                    rptrDistinctBoothItems.DataBind();
                }
            }
        }

        private static int CategorySorted(ExhibitorBoothItems boothItem1, ExhibitorBoothItems boothItem2)
        {
            return boothItem1.CategorySortOrder.CompareTo(boothItem2.CategorySortOrder);
        }

        private void FindCategories(List<string> categories, Order order)
        {
            foreach (OrderItem item in order.OrderItems)
            {
                if(!categories.Contains(item.CategoryName))
                {
                    categories.Add(item.CategoryName);
                }
            }
        }
    }
}