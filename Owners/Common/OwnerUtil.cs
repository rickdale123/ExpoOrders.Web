using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using ExpoOrders.Common;
using ExpoOrders.Entities;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.Security;
using ExpoOrders.Controllers;

namespace ExpoOrders.Web.Owners.Common
{
    public class OwnerUtil
    {
        public static List<NavigationLink> BuildAllActiveNavLinks(int tabNbr, TabLink tab, bool includeInactive)
        {
            List<NavigationLink> navLinks = RetrieveAllActiveNavLinks(tab, includeInactive);

            return navLinks.Where(n => n.NavigationActionId == (int)NavigationLink.NavigationalAction.FileDownload
                                        || n.NavigationActionId == (int)NavigationLink.NavigationalAction.ShowCategory
                                        || n.NavigationActionId == (int)NavigationLink.NavigationalAction.HtmlContent 
                                        || n.NavigationActionId == (int)NavigationLink.NavigationalAction.TextOnly
                                        || n.NavigationActionId == (int)NavigationLink.NavigationalAction.DynamicForm
                                        || n.NavigationActionId == (int)NavigationLink.NavigationalAction.AdvanceWarehouseLabel
                                        || n.NavigationActionId == (int)NavigationLink.NavigationalAction.DirectShowSiteLabel
                                        || n.NavigationActionId == (int)NavigationLink.NavigationalAction.OutboundShippingLabel).ToList();;
        }

        public static List<NavigationLink> RetrieveAllActiveNavLinks(TabLink tab, bool includeInactive)
        {
            List<NavigationLink> linksToRender = new List<NavigationLink>();

            if (!includeInactive)
            {
                tab.NavigationLinks.Where(l => l.ActiveFlag == true && l.ParentNavigationLink == null).ToList().ForEach<NavigationLink>(
                    link =>
                    {
                        linksToRender.Add(link);
                        if (link.ChildNavigationLinks.Count > 0)
                        {
                            link.ChildNavigationLinks
                         .Where(n => n.ActiveFlag == true)
                         .OrderBy(n => n.SortOrder).ToList().ForEach<NavigationLink>(
                         childLink =>
                         {
                             linksToRender.Add(childLink);
                         }
                         );
                        }
                    });
            }
            else
            {
                tab.NavigationLinks.Where(t => t.ParentNavigationLink == null).ToList().ForEach<NavigationLink>(
                   link =>
                   {
                       linksToRender.Add(link);
                       if (link.ChildNavigationLinks.Count > 0)
                       {
                           link.ChildNavigationLinks
                        .OrderBy(n => n.SortOrder).ToList().ForEach<NavigationLink>(
                        childLink =>
                        {
                            linksToRender.Add(childLink);
                        }
                        );
                       }
                   });
            }
            return linksToRender;
        }

        public static List<NavigationLink> BuildTabSubNavLinks(Show show)
        {
            List<NavigationLink> links = new List<NavigationLink>();
            int linkNumber = 1;
            show.TabLinks.OrderBy(t => t.TabNumber).ToList().ForEach(t =>
                {
                    links.Add(new NavigationLink((int)t.TabNumber, string.Format("Tab {0} Editor", linkNumber), NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PostBack, linkNumber));
                    linkNumber++;
                });

            return links;
        }

        public static List<NavigationLink> BuildContentExplorerSubNavLinks()
        {
            List<NavigationLink> links = new List<NavigationLink>();

            links.Add(
                    new NavigationLink(1, "Html Content", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PostBack, (int)ContentExplorerPageMode.HtmlContent)
                    );

            links.Add(
                   new NavigationLink(2, "File Downloads", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PostBack, (int)ContentExplorerPageMode.FileDownload)
                   );

            links.Add(
                    new NavigationLink(3, "Dynamic Forms", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PostBack, (int)ContentExplorerPageMode.DynamicForm)
                    );

            return links;
        }

        public static List<NavigationLink> BuildReportsSubNavLinks()
        {
            List<NavigationLink> links = new List<NavigationLink>();

            if (Roles.IsUserInRole(UserRoleEnum.ExhibitorReports.GetCodeValue()))
            {
                links.Add(
                    new NavigationLink(0, "Exhibitor Reports", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.TextOnly, 0)
                    );
                links.Add(
                      new NavigationLink(1, "Exhibitor List", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.ExhibitorList)
                      );

                links.Add(
                      new NavigationLink(28, "Contact List", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.ContactList)
                      );

                links.Add(
                     new NavigationLink(29, "Outbound Shipping Labels", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.OutboundShippingLabelList)
                     );

                links.Add(
                     new NavigationLink(30, "Outbound Shipping Labels 2", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.OutboundShippingLabelList2)
                     );

                links.Add(
                        new NavigationLink(2, "Exhibitor Invoice", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.ExhibitorInvoice)
                        );

                links.Add(
                    new NavigationLink(3, "Order Summary - By Category", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.OrderSummaryByCategory)
                    );

                links.Add(
                    new NavigationLink(4, "Order Summary - By Exhibitor", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.OrderSummaryByExhibitor)
                    );

                links.Add(
                   new NavigationLink(5, "Order Receipts", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.OrderReceipt)
                   );

                links.Add(
                   new NavigationLink(10, "Delivery Receipts", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.DeliveryReceipt)
                   );

                links.Add(
                    new NavigationLink(6, "Form Submissions", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.FormSubmission)
                    );

                links.Add(
                    new NavigationLink(7, "User Logins", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.UserLoginReport)
                    );

                links.Add(
                   new NavigationLink(9, "Credit Cards On File", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.ExhibitorCreditCards)
                   );

                links.Add(
                    new NavigationLink(8, "Exhibitor Price List", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.PriceList)
                    );

            }

            if (Roles.IsUserInRole(UserRoleEnum.OperationsReports.GetCodeValue()))
            {
                links.Add(
                    new NavigationLink(20, "Operations Reports", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.TextOnly, 0)
                    );

                links.Add(
                       new NavigationLink(21, "Inventory Report", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.InventoryReport)
                       );

                links.Add(
                        new NavigationLink(22, "Delivery Report", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.DeliveryReport)
                            );
                links.Add(
                        new NavigationLink(23, "Load List", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.InventorySummary)
                            );

                links.Add(
                        new NavigationLink(24, "Installation/Dismantle", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.InstallDismantleReport)
                            );
                links.Add(
                        new NavigationLink(26, "Bad OrderItem Report", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.BadLineItemReport)
                            );
            }


            if (Roles.IsUserInRole(UserRoleEnum.FinancialReports.GetCodeValue()))
            {
                links.Add(
                    new NavigationLink(40, "Financial Reports", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.TextOnly, 0)
                    );

                links.Add(
                   new NavigationLink(44, "Credit Card Payments", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.CreditCardPayments)
                   );

                links.Add(
                   new NavigationLink(45, "Check Payments", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.CheckPayments)
                   );

                links.Add(
                  new NavigationLink(46, "Wire Payments", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.WirePayments)
                  );

                links.Add(
                     new NavigationLink(41, "Billing Summary", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.BillingSummary)
                     );

                links.Add(
                    new NavigationLink(42, "Receivables", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.Receivables)
                    );

                links.Add(
                    new NavigationLink(47, "Payment Transactions", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.Transactions)
                    );

                links.Add(
                    new NavigationLink(43, "General Ledger", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.GeneralLedger)
                    );
            }
            
            return links;
        }

        public static List<NavigationLink> BuildCombinedReportsSubNavLinks()
        {
            List<NavigationLink> links = new List<NavigationLink>();
            if (Roles.IsUserInRole(UserRoleEnum.FinancialReports.GetCodeValue()))
            {
                links.Add(
                    new NavigationLink(50, "Summary Reports", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.TextOnly, 0)
                    );

                links.Add(
                   new NavigationLink(51, "Total Receipts", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ReportEnum.CombinedShowReceipts)
                   );
            }
            return links;
        }

        public static List<NavigationLink> BuildExhibitorsSubNavLinks()
        {
            List<NavigationLink> links = new List<NavigationLink>();

            links.Add(
                    new NavigationLink(1, "Exhibitor List", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PostBack, (int)ExhibitorPageMode.Exhibitors)
                    );

            links.Add(
                   new NavigationLink(2, "Upload Exhibitors", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PostBack, (int)ExhibitorPageMode.UploadExhibitors)
                   );

            return links;
        }

        public static List<NavigationLink> BuildOrdersSubNavLinks()
        {
            List<NavigationLink> links = new List<NavigationLink>();

            links.Add(
                   new NavigationLink(1, "Process Orders", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PageLink, (int)ShowSettingPageMode.Orders)
                   );

            links.Add(
                    new NavigationLink(2, "Process Payments", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.PageLink, (int)ShowSettingPageMode.Payments)
                    );


            return links;
        }

        public static List<NavigationLink> BuildShowAdminNavLinks(ExpoOrdersUser currentUser, int currentShowId)
        {
            List<NavigationLink> links = new List<NavigationLink>();

            if (Roles.IsUserInRole(UserRoleEnum.ShowManagement.GetCodeValue()))
            {
                links.Add(
                   new NavigationLink(1, "Show Configuration", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.TextOnly, 0)
                   );

                links.Add(
                        new NavigationLink(2, "Venue/Dates/Addresses", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ShowSettingPageMode.ShowSettings)
                        );

                links.Add(
                      new NavigationLink(3, "Exhibitor Site Settings", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ShowSettingPageMode.ShowOrderConfiguration)
                      );

                links.Add(
                       new NavigationLink(4, "Site Design Files", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PostBack, (int)ShowSettingPageMode.Assets)
                       );

                links.Add(
                      new NavigationLink(5, "Categories & Products", NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PageLink, (int)ShowSettingPageMode.Products)
                      );

                links.Add(
                     new NavigationLink(7, "Tab Configuration", NavigationLink.NavigationalItemType.Nav, NavigationLink.NavigationalAction.TextOnly, 0)
                     );

                string tab1LinkText = "Tab One";
                string tab2LinkText = "Tab Two";
                string tab3LinkText = "Tab Three";
                string tab4LinkText = "Tab Four";
                string tab5LinkText = "Tab Five";

                if (currentShowId > 0)
                {
                    ShowController showCntrl = new ShowController(currentUser);
                    Show currentShow = showCntrl.GetShowById(currentShowId);
                    if (currentShow != null && currentShow.TabLinks != null && currentShow.TabLinks.Count >= 5)
                    {
                        tab1LinkText = "1. " + currentShow.TabLinks.ToList()[0].Text;
                        tab2LinkText = "2. " + currentShow.TabLinks.ToList()[1].Text;
                        tab3LinkText = "3. " + currentShow.TabLinks.ToList()[2].Text;
                        tab4LinkText = "4. " + currentShow.TabLinks.ToList()[3].Text;
                        tab5LinkText = "5. " + currentShow.TabLinks.ToList()[4].Text;
                    }
                }
                
                links.Add(
                     new NavigationLink(8, tab1LinkText, NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PageLink, (int)ShowSettingPageMode.Tab1Editor)
                     );
                links.Add(
                     new NavigationLink(9, tab2LinkText, NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PageLink, (int)ShowSettingPageMode.Tab2Editor)
                     );
                links.Add(
                     new NavigationLink(10, tab3LinkText, NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PageLink, (int)ShowSettingPageMode.Tab3Editor)
                     );
                links.Add(
                     new NavigationLink(11, tab4LinkText, NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PageLink, (int)ShowSettingPageMode.Tab4Editor)
                     );
                links.Add(
                     new NavigationLink(12, tab5LinkText, NavigationLink.NavigationalItemType.SubNav, NavigationLink.NavigationalAction.PageLink, (int)ShowSettingPageMode.Tab5Editor)
                     );
            }

            return links;
        }

        public static void ClearPlaceHolderControl(PlaceHolder placeHolderControl)
        {
            foreach (Control control in placeHolderControl.Controls)
            {
                if (control is PlaceHolder)
                {
                    ClearPlaceHolderControl((PlaceHolder) control);
                }
                else if (control is TextBox && control != null)
                {
                    TextBox textBox = (TextBox)control;
                    textBox.Text = string.Empty;
                }
                else if (control is DropDownList && control != null)
                {
                    DropDownList dropDownList = (DropDownList)control;
                    dropDownList.ClearSelection();
                }
            }
        }
    }
}