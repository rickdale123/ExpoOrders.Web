using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Entities;
using ExpoOrders.Common;

namespace ExpoOrders.Web.CustomControls
{
    public partial class FormQuestionEditorList : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Populate(List<Question> questions)
        {
            if (questions != null)
            {
                this.Visible = true;
                grdvwFormQuestionList.DataSource = questions;
                grdvwFormQuestionList.DataBind();
            }
        }

        public int QuestionCount
        {
            get 
            {
                if (this.grdvwFormQuestionList.Visible && this.grdvwFormQuestionList != null)
                {
                    return this.grdvwFormQuestionList.Rows.Count;
                }
                else
                {
                    return 0;
                }
            }
        
        }

        protected void grdvwFormQuestionList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Question question = (Question)e.Row.DataItem;
              
                Literal ltrQuestionSortOrder = (Literal)e.Row.FindControl("ltrQuestionSortOrder");
                ltrQuestionSortOrder.Text = question.SortOrder.ToString();

                Literal ltrQuestionResponseType = (Literal)e.Row.FindControl("ltrQuestionResponseType");
                ltrQuestionResponseType.Text = question.ResponseTypeCd.Trim();

                Literal ltrQuestionRequired = (Literal)e.Row.FindControl("ltrQuestionRequired");
                ltrQuestionRequired.Text = (question.RequiredFlag == true) ? "Yes" : "No";
                
            }
        }

        protected void grdvwFormQuestionList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int questionId = Util.ConvertInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "EditQuestion":
                    if (_itemSelected != null)
                    {
                        _itemSelected(questionId);
                    }
                    break;
                case "DeleteQuestion":
                    if (_itemDeleted != null)
                    {
                        _itemDeleted(questionId);
                    }
                    break;
            }

        }

        private Action<int> _itemSelected;
        public Action<int> ItemSelected
        {
            set
            {
                _itemSelected = value;
            }
        }

        private Action<int> _itemDeleted;
        public Action<int> ItemDeleted
        {
            set
            {
                _itemDeleted = value;
            }
        }


    }
}