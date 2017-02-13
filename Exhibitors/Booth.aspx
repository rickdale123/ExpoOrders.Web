<%@ Page Title="" Language="C#" MasterPageFile="~/Exhibitors/ExhibitorsMaster.Master" AutoEventWireup="true" CodeBehind="Booth.aspx.cs" Inherits="ExpoOrders.Web.Exhibitors.Booth" %>
<%@ MasterType VirtualPath="~/Exhibitors/ExhibitorsMaster.Master" %>

<%@ Register Src="~/CustomControls/BoothDetails.ascx" TagPrefix="uc" TagName="BoothDetails" %>
<%@ Register Src="~/CustomControls/CategoryItems.ascx" TagPrefix="uc" TagName="CategoryItems" %>
<%@ Register Src="~/CustomControls/ItemDetail.ascx" TagPrefix="uc" TagName="ItemDetail" %>
<%@ Register Src="~/CustomControls/FormQuestions.ascx" TagPrefix="uc" TagName="FormQuestions" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">

    <script language="javascript" type="text/javascript">

        function calcCost() {

        var isValid = false;

        var txtQuantity = document.getElementById('PageContent_ucItemDetail_txtQuantity');
        var hdnUnitPrice = document.getElementById('PageContent_ucItemDetail_hdnUnitPrice');
        var hdnAdditionalChargeAmt = document.getElementById('PageContent_ucItemDetail_hdnAdditionalChargeAmt');
        var additionalChargeType = document.getElementById('PageContent_ucItemDetail_hdnAdditionalChargeType').value;
        var hdnLateFeeAmt = document.getElementById('PageContent_ucItemDetail_hdnLateFeeAmt');
        var lateFeeType = document.getElementById('PageContent_ucItemDetail_hdnLateFeeType').value;
        var hdnSalesTaxPct = document.getElementById('PageContent_ucItemDetail_hdnSalesTaxPct');
        var currencySymbol = hiddenValue('PageContent_ucItemDetail_hdnCurrencySymbol');
        var hdnIsCalcByAttribs = document.getElementById('PageContent_ucItemDetail_hdnIsCalcByAttribs');


        var TotalUnitPrice = document.getElementById('TotalUnitPrice');
        var TotalAdditionalCharges = document.getElementById('TotalAdditionalCharges');
        var TotalLateFee = document.getElementById('TotalLateFee');
        var TotalSalesTax = document.getElementById('TotalSalesTax');
        var TotalCost = document.getElementById('TotalCost');

        var btnAddToCart = document.getElementById('PageContent_ucItemDetail_btnAddToCart');


        if (hdnIsCalcByAttribs.value == '1') {
            var reqAttrib1 = getRequiredAttribSelection(1);
            var reqAttrib2 = getRequiredAttribSelection(2);
            txtQuantity.value = reqAttrib1 * reqAttrib2;
        }

        var qty = parseFloat(txtQuantity.value);
        txtQuantity.value = qty;

        var totalCost = 0;

        TotalUnitPrice.innerHTML = (0).formatMoney(2, '.', ',', currencySymbol);
        TotalAdditionalCharges.innerHTML = (0).formatMoney(2, '.', ',', currencySymbol);
        TotalLateFee.innerHTML = (0).formatMoney(2, '.', ',', currencySymbol);
        TotalSalesTax.innerHTML = (0).formatMoney(2, '.', ',', currencySymbol);

        if (qty > 0) {
            isValid = true;

            var unitPrice = parseFloat(hdnUnitPrice.value);
            var totalUnitPrice = (unitPrice * qty);
            var totalAddtlCharge = 0;
            var totalLateFee = 0;
            var totalSalesTax = 0;

            /*additional charge calc */
            if (additionalChargeType == 'Flat') {
                totalAddtlCharge = parseFloat(hdnAdditionalChargeAmt.value);
            }
            else if (additionalChargeType == 'PerUnit') {
                totalAddtlCharge = parseFloat(hdnAdditionalChargeAmt.value) * qty;
            }
            else if (additionalChargeType == 'PctTotal') {
                totalAddtlCharge = totalUnitPrice * parseFloat(hdnAdditionalChargeAmt.value);
            }
            else {
                totalAddtlCharge = parseFloat(hdnAdditionalChargeAmt.value);
            }


            /*late fee charge calc */
            if (lateFeeType == 'Flat') {
                totalLateFee = parseFloat(hdnLateFeeAmt.value);
            }
            else if (lateFeeType == 'PerUnit') {
                totalLateFee = parseFloat(hdnLateFeeAmt.value) * qty;
            }
            else if (lateFeeType == 'PctTotal') {
                totalLateFee = totalUnitPrice * parseFloat(hdnLateFeeAmt.value);
            }
            else {
                totalLateFee = parseFloat(hdnLateFeeAmt.value);
            }


            /*Tax calc */
            var taxPct = parseFloat(hdnSalesTaxPct.value);
            totalSalesTax = taxPct * (totalUnitPrice + totalAddtlCharge + totalLateFee);

            TotalUnitPrice.innerHTML = (totalUnitPrice).formatMoney(2, '.', ',', currencySymbol);
            TotalAdditionalCharges.innerHTML = (totalAddtlCharge).formatMoney(2, '.', ',', currencySymbol);
            TotalLateFee.innerHTML = (totalLateFee).formatMoney(2, '.', ',', currencySymbol);
            TotalSalesTax.innerHTML = (totalSalesTax).formatMoney(2, '.', ',', currencySymbol);

            totalCost = totalUnitPrice + totalAddtlCharge + totalLateFee + totalSalesTax;

        }

        TotalCost.innerHTML = totalCost.formatMoney(2, '.', ',', currencySymbol);
        btnAddToCart.disabled = !isValid;

    }

    function getRequiredAttribSelection(attribNbr) {
        var selectedAttribValue = '';

        var cntrlId = 'PageContent_ucItemDetail_ddlRequiredAttributeChoices' + attribNbr;
        var cntrl = document.getElementById(cntrlId);
        if (cntrl) {
            selectedAttribValue = cntrl.value;
        }
        else {
            cntrlId = 'PageContent_ucItemDetail_txtRequiredAttributeChoices' + attribNbr;
            cntrl = document.getElementById(cntrlId);
            if (cntrl) {
                selectedAttribValue = cntrl.value;
            }
        }

        if (selectedAttribValue != '') {
            return parseFloat(selectedAttribValue);
        }
        else {
            return 0;
        }
    }


    function swapImageSrc(imgDetail, imgThumbnail) {
        var imgDetailCntrl = document.getElementById(imgDetail);
        var imgThumbnailCntrl = document.getElementById(imgThumbnail);

        imgDetailCntrl.src = imgThumbnailCntrl.src;
    }

</script>


<script type="text/javascript">
    $(document).ready(function () {

        var nbrProducts = <%=NumberProducts %>;

        for (var i = 0; i <= nbrProducts; i++) {

            var groupName = 'thumbnailGroup_' + i;

            $("a[rel=" + groupName + "]").fancybox({
                'transitionIn': 'none',
                'titleShow': true,
                'showNavArrows': true,
                'modal': false,
                'margin': 150,
                'cyclic': true,
                'transitionOut': 'none',
                'titlePosition': 'over',
                'titleFormat': function (title, currentArray, currentIndex, currentOpts) {
                    return '<span id="fancybox-title-over">Image ' + (currentIndex + 1) + ' / ' + currentArray.length + (title.length ? ' &nbsp; ' + title : '') + '</span>';
                }
            });
        }
    });


    function showImageGroup(groupNbr) {

        var groupName = 'thumbnailGroup_' + groupNbr;
        var groupBox = $("a[rel=" + groupName + "]:first");

        groupBox.trigger('click');
    }
	</script>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">

    <CustomControls:ValidationErrors ID="PageErrors" ValidationGroup="AddToCart" CssClass="errorMessageBlock" runat="server" />

    <uc:BoothDetails id="ucBoothDetails" visible="false" runat="server" />

    <asp:PlaceHolder ID="plcCategoryIcons" runat="server">


    </asp:PlaceHolder>

    <uc:CategoryItems id="ucCategoryItems" visible="false" runat="server" />

    <uc:ItemDetail ID="ucItemDetail" Visible="false" runat="server" />
   

   <asp:PlaceHolder ID="plcDynamicForm" Visible="false" runat="server">
        <fieldset class="commonControls">
            <legend class="commonControls"><asp:Literal ID="ltrFormName" runat="server" /></legend>
            
                <asp:Label ID="lblFormSubmissionDeadlineError" Visible="false" CssClass="errorMessage" runat="server" />

                <p><asp:Literal ID="ltrFormDescription" runat="server" /></p>

               <uc:FormQuestions id="ucFormQuestions" visible="false" runat="server" />

            <center>
                <asp:Button ID="btnSubmitForm" OnClick="btnSubmitForm_Click" Text="Submit" CssClass="button" ValidationGroup="FormSubmission" runat="server" />
            </center>
            
        </fieldset>
        <asp:HiddenField ID="hdnFormId" runat="server" EnableViewState="true" Value="0" Visible="true" />
    </asp:PlaceHolder>

</asp:Content>
