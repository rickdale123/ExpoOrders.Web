using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ExpoOrders.Entities;
using ExpoOrders.Common;
using AjaxControlToolkit;
using System.Web.UI.HtmlControls;

namespace ExpoOrders.Web.CustomControls
{
    public partial class FormQuestions : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void PopulateQuestions(ICollection<Question> questions)
        {
            this.Visible = true;
            rptrDynamicForm.DataSource = questions.OrderBy(q => q.SortOrder);
            rptrDynamicForm.DataBind();
        }

        public List<QuestionAnswer> GetResponses(ref List<string> missingRequiredQuestions)
        {
            if (missingRequiredQuestions == null)
            {
                missingRequiredQuestions = new List<string>();
            }

            List<QuestionAnswer> questionAnswerList = new List<QuestionAnswer>();
            int sortOrder = 1;

            foreach (RepeaterItem question in rptrDynamicForm.Items)
            {
                GetFormResponse(questionAnswerList, question, sortOrder, missingRequiredQuestions);
                sortOrder++;
            }
            return questionAnswerList;
        }

        protected void rptrDynamicForm_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
              e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Question question = (Question)e.Item.DataItem;

                if (question != null)
                {
                    HtmlTableRow trQuestion = (HtmlTableRow)e.Item.FindControl("trQuestion");
                    HtmlTableRow trQuestionContentArea = (HtmlTableRow)e.Item.FindControl("trQuestionContentArea");

                    trQuestionContentArea.Visible = trQuestion.Visible = false;
                    QuestionResponseTypeEnum responseType = (QuestionResponseTypeEnum)Enum.Parse(typeof(QuestionResponseTypeEnum), question.ResponseTypeCd, true);

                    if (responseType == QuestionResponseTypeEnum.Content)
                    {
                        trQuestionContentArea.Visible = true;
                        Literal ltrQuestionContent = (Literal)e.Item.FindControl("ltrQuestionContent");
                        ltrQuestionContent.Text = question.OptionsList;

                    }
                    else
                    {
                        trQuestion.Visible = true;

                        Label label = (Label)e.Item.FindControl("lblQuestion");
                        label.Text = question.QuestionText;
                        label.Visible = true;

                        Label requiredLabel = (Label)e.Item.FindControl("lblRequired");
                        requiredLabel.Visible = question.RequiredFlag;


                        QuestionDataTypeEnum dataType = QuestionDataTypeEnum.NotSet;

                        if (!string.IsNullOrEmpty(question.DataType))
                        {
                            dataType = (QuestionDataTypeEnum)Enum.Parse(typeof(QuestionDataTypeEnum), question.DataType, true);
                        }

                        switch (responseType)
                        {
                            case QuestionResponseTypeEnum.TextArea:
                            case QuestionResponseTypeEnum.TextBox:
                                TextBox textBox = (TextBox)e.Item.FindControl("txtResponse");
                                textBox.ValidationGroup = e.Item.ItemIndex.ToString();
                                RequiredFieldValidator requiredValidatorTxt = (RequiredFieldValidator)e.Item.FindControl("reqValResponseTxt");
                                ApplyRequiredValidator(e, question, requiredValidatorTxt);
                                ApplyTextBoxTemplate(question, responseType, textBox);
                                CalendarExtender calendarExtender = (CalendarExtender)e.Item.FindControl("calExtender");
                                if (dataType != QuestionDataTypeEnum.Date)
                                {
                                    calendarExtender.Enabled = false;
                                }
                                else
                                {
                                    calendarExtender.Enabled = true;
                                }
                                break;
                            case QuestionResponseTypeEnum.CheckBoxList:
                                CheckBoxList checkBoxList = (CheckBoxList)e.Item.FindControl("chkListResponse");
                                checkBoxList.ValidationGroup = e.Item.ItemIndex.ToString();
                                ApplyCheckBoxListTemplate(question, checkBoxList);
                                break;
                            case QuestionResponseTypeEnum.CheckBox:
                                CheckBox checkBox = (CheckBox)e.Item.FindControl("chkSingleResponse");
                                checkBox.ValidationGroup = e.Item.ItemIndex.ToString();
                                ApplyCheckBoxSingleTemplate(question, checkBox);
                                break;
                            case QuestionResponseTypeEnum.DropDown:
                                DropDownList dropDownList = (DropDownList)e.Item.FindControl("ddlResponse");
                                dropDownList.ValidationGroup = e.Item.ItemIndex.ToString();
                                RequiredFieldValidator requiredValidatorDDL = (RequiredFieldValidator)e.Item.FindControl("reqValResponseDDL");
                                ApplyRequiredValidator(e, question, requiredValidatorDDL);
                                ApplyDropDownTemplate(question, dropDownList);
                                break;
                            case QuestionResponseTypeEnum.RadioBtn:
                                RadioButtonList radioButtonList = (RadioButtonList)e.Item.FindControl("rbtnListResponse");
                                radioButtonList.ValidationGroup = e.Item.ItemIndex.ToString();
                                RequiredFieldValidator requiredValidatorRbtn = (RequiredFieldValidator)e.Item.FindControl("reqValResponseRbtn");
                                ApplyRequiredValidator(e, question, requiredValidatorRbtn);
                                ApplyRadioButtonListTemplate(question, radioButtonList);
                                break;
                            case QuestionResponseTypeEnum.Content:
                                throw new Exception("Type is Content");
                        }
                    }
                }
            }
        }

        private static void ApplyRequiredValidator(RepeaterItemEventArgs e, Question question, RequiredFieldValidator reqVal)
        {
            reqVal.Enabled = question.RequiredFlag;
            reqVal.Visible = question.RequiredFlag;
            reqVal.ErrorMessage = "*";
            reqVal.ValidationGroup = e.Item.ItemIndex.ToString();

        }

        private static void GetFormResponse(List<QuestionAnswer> questionResponseList, RepeaterItem item, int sortOrder, List<string> missingRequiredQuestions)
        {
            string currentQuestion = string.Empty;
            string currentAnswer = string.Empty;

            HtmlTableRow trQuestion = (HtmlTableRow)item.FindControl("trQuestion");
            HtmlTableRow trQuestionContentArea = (HtmlTableRow)item.FindControl("trQuestionContentArea");

            if (trQuestionContentArea.Visible)
            {
                Literal ltrQuestionContent = (Literal)item.FindControl("ltrQuestionContent");
                questionResponseList.Add(new QuestionAnswer(ltrQuestionContent.Text, string.Empty, sortOrder));
            }
            else if (trQuestion.Visible)
            {
                Label lblQuestion = (Label)item.FindControl("lblQuestion");
                TextBox txtResponse = (TextBox)item.FindControl("txtResponse");
                DropDownList ddlResponse = (DropDownList)item.FindControl("ddlResponse");
                CheckBoxList chkListResponse = (CheckBoxList)item.FindControl("chkListResponse");
                CheckBox chkSingleResponse = (CheckBox)item.FindControl("chkSingleResponse");
                RadioButtonList rBtnListResponse = (RadioButtonList)item.FindControl("rbtnListResponse");
                Label lblRequired = (Label)item.FindControl("lblRequired");

                if (lblQuestion != null)
                {
                    currentQuestion = lblQuestion.Text;
                }
                if (txtResponse != null && txtResponse.Visible)
                {
                    currentAnswer = txtResponse.Text;
                }
                if (ddlResponse != null && ddlResponse.Visible)
                {
                    currentAnswer = ddlResponse.SelectedValue;
                }
                if (chkListResponse != null && chkListResponse.Visible)
                {
                    foreach (ListItem checkBoxItem in chkListResponse.Items)
                    {
                        if (checkBoxItem.Selected)
                        {
                            currentAnswer = currentAnswer + checkBoxItem.Value.ToString() + ";";
                        }
                    }
                    currentAnswer = currentAnswer.TrimEnd(new char[] { ';' });
                }
                if (chkSingleResponse != null && chkSingleResponse.Visible)
                {
                    currentAnswer = chkSingleResponse.Checked ? "Yes" : (lblRequired.Visible) ? string.Empty : "No"; //If checkbox is required, then YES is the only acceptable answer
                }
                if (rBtnListResponse != null && rBtnListResponse.Visible)
                {
                    currentAnswer = rBtnListResponse.SelectedValue;
                }

                if (lblRequired.Visible && string.IsNullOrEmpty(currentAnswer))
                {
                    missingRequiredQuestions.Add(currentQuestion);
                }
                questionResponseList.Add(new QuestionAnswer(currentQuestion, currentAnswer, sortOrder));
            }
        }

        #region Question Response Format Templates

        private static void ApplyTextBoxTemplate(Question question, QuestionResponseTypeEnum responseType, TextBox responseTextBox)
        {
            responseTextBox.CssClass = "inputText";

            if (question.MaxLength != null && question.MaxLength > 0)
            {
                responseTextBox.MaxLength = (int)question.MaxLength;
            }

            if (responseType == QuestionResponseTypeEnum.TextArea)
            {
                responseTextBox.TextMode = TextBoxMode.MultiLine;
                responseTextBox.Columns = 50;
                responseTextBox.Rows = 4;
            }

            if (!string.IsNullOrEmpty(question.DataType))
            {
                QuestionDataTypeEnum datatype;
                if (Enum.TryParse<QuestionDataTypeEnum>(question.DataType, out datatype))
                {
                    switch (datatype)
                    {
                        case QuestionDataTypeEnum.Number:
                            responseTextBox.Attributes.Add("onkeypress", "onkeypress_number();");
                            break;
                    }
                }
            }

            responseTextBox.Visible = true;
        }

        private void ApplyCheckBoxListTemplate(Question question, CheckBoxList checkBoxList)
        {

            question.OptionsList.Split(new char[] { ';' }).ForEach(
                option => { checkBoxList.Items.Add(BuildListItem(option)); }
                );

            checkBoxList.Visible = true;
        }
        private void ApplyCheckBoxSingleTemplate(Question question, CheckBox checkBox)
        {
            checkBox.Visible = true;
        }
        private void ApplyRadioButtonListTemplate(Question question, RadioButtonList radioButtonList)
        {

            question.OptionsList.Split(new char[] { ';' }).ForEach(
                option => { radioButtonList.Items.Add(BuildListItem(option)); }
                );

            radioButtonList.Visible = true;
        }

        private ListItem BuildListItem(string option)
        {
            ListItem item = new ListItem();
            item.Text = option;
            item.Value = option;
            return item;
        }

        private void ApplyDropDownTemplate(Question question, DropDownList dropDownList)
        {
            dropDownList.CssClass = "inputText";
            dropDownList.Items.Add(new ListItem { Value = "", Text = "- Select One -" });
            question.OptionsList.Split(new char[] { ';' }).ForEach(
                option => { dropDownList.Items.Add(BuildListItem(option)); }
                );

            dropDownList.Visible = true;
        }

        #endregion

    }
}