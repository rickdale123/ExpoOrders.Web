using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Common;
using ExpoOrders.Entities;

namespace ExpoOrders.Web.CustomControls
{
    public partial class FormQuestionEditor : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public void ClearQuestion()
        {
            hdnQuestionId.Value = "0";
            txtQuestionText.Text = string.Empty;
            txtSortOrder.Text = string.Empty;
            chkQuestionRequired.Checked = false;
            txtMaxLength.Text = string.Empty;
            txtOptions.Text = string.Empty;
            txtDefaultValue.Text = string.Empty;
            ddlDataType.ClearSelection();
            ddlDataType.SelectedIndex = 0;
            ddlResponseType.ClearSelection();
            ddlResponseType.SelectedIndex = 0;
        }

        public void Populate(Question question)
        {
            if (question != null)
            {
                this.Visible = true;

                hdnQuestionId.Value = question.QuestionId.ToString();

                txtQuestionText.Text = Util.ConvertEmpty(question.QuestionText);
                txtSortOrder.Text = question.SortOrder.ToString();
                chkQuestionRequired.Checked = question.RequiredFlag;
                txtMaxLength.Text = question.MaxLength.HasValue ? question.MaxLength.ToString() : string.Empty;
                txtOptions.Text = Util.ConvertEmpty(question.OptionsList);
                txtDefaultValue.Text = Util.ConvertEmpty(question.DefaultValue);
                if (question.DataType != null)
                {
                    ddlDataType.ClearSelection();
                    WebUtil.SelectListItemByValue(ddlDataType, question.DataType.Trim());
                }
                if (question.ResponseTypeCd != null)
                {
                    ddlResponseType.ClearSelection();
                    WebUtil.SelectListItemByValue(ddlResponseType, question.ResponseTypeCd);
                }
            }
        }

        public Question BuildQuestion()
        {
            Question question = new Question();
            question.QuestionId = Util.ConvertInt32(hdnQuestionId.Value);

            question.DefaultValue = txtDefaultValue.Text.Trim();

            if (ddlDataType.SelectedIndex > 0)
            {
                question.DataType = ddlDataType.SelectedValue;
            }

            if (!string.IsNullOrEmpty(txtMaxLength.Text))
            {
                question.MaxLength = Util.ConvertInt32(txtMaxLength.Text.Trim());
            }

            question.OptionsList = txtOptions.Text.Trim();
            question.QuestionText = txtQuestionText.Text.Trim();
            question.RequiredFlag = chkQuestionRequired.Checked;

            if (ddlResponseType.SelectedIndex > 0)
            {
                question.ResponseTypeCd = ddlResponseType.SelectedValue;
            }

            if (!string.IsNullOrEmpty(txtSortOrder.Text))
            {
                question.SortOrder = Util.ConvertInt32(txtSortOrder.Text.Trim());
            }

            return question;

        }

        public void SetFocus()
        {
            txtQuestionText.Focus();
        }

        public int QuestionSortOrder
        {
            set
            {
                this.txtSortOrder.Text = value.ToString();
            }
        }
    }
}