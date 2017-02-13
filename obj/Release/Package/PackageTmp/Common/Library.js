
function loadPage() {

}

function launchShowPreview(showId) {
    var newWindow = window.open('../Exhibitors/Default.aspx', 'showPreview', 'width=800,height=500,location=0,status=1,scrollbars=1,resizable=yes');
    if (window.focus && newWindow) {
        newWindow.focus()
    }   
}

function launchEmailEditor() {
    var newWindow = window.open('../Owners/EmailPopup.aspx', 'emailEditor', 'width=800,height=500,location=0,status=1,scrollbars=1,resizable=yes');
    if (window.focus && newWindow) {
        newWindow.focus()
    }
}

function launchEmailLog(userId, exhibitorId) {
    var newWindow = window.open('../Owners/EmailLog.aspx?userId=' + userId + '&exhibitorId=' + exhibitorId, 'emailLog', 'width=800,height=500,location=0,status=1,scrollbars=1,resizable=yes');
    if (window.focus && newWindow) {
        newWindow.focus()
    }
}

function launchCallLog(userId, exhibitorId) {
    var newWindow = window.open('../Owners/CallLog.aspx?userId=' + userId + '&exhibitorId=' + exhibitorId, 'callLog', 'width=800,height=500,location=0,status=1,scrollbars=1,resizable=yes');
    if (window.focus && newWindow) {
        newWindow.focus()
    }
}

function convertBool(val) {
    return (val == '1' || val.toLowerCase() == 'true');
}

function hiddenValue(ctrlId) {
    var hdn = document.getElementById(ctrlId);

    if (hdn) {
        return hdn.value;
    }
    else {
        return '';
    }
}


function hideObject(objId) {
    var obj = document.getElementById(objId);

    if (obj) {
        obj.style.display = 'none';
    }
}

function showObject(objId) {
    var obj = document.getElementById(objId);

    if (obj) {
        obj.style.display = 'inline-block';
    }
}

function selectedRadioButtonValue(ctrlName) {
    var radioVal = null;
    var ctrl = document.getElementsByName(ctrlName);
    if (ctrl) {
        for (i = 0; i < ctrl.length; i++) {
            if (ctrl[i].checked) {
                radioVal = ctrl[i].value;
            }
        }
    }
    return radioVal;
}

function launchFileDownload(targetId) {

    var width = 400;
    var height = 450;
    var left = (screen.width - width) / 2;
    var top = (screen.height - height) / 2;
    var params = 'width=' + width + ', height=' + height;
    params += ', top=' + top + ', left=' + left;
    params += ', directories=no';
    params += ', location=no';
    params += ', menubar=no';
    params += ', resizable=yes';
    params += ', scrollbars=yes';
    params += ', status=no';
    params += ', toolbar=no';
    var newWindow = window.open('Download.aspx?target=' + targetId, 'fileDownload', params);
    if (window.focus && newWindow) {
        newWindow.focus() 
    }
    return false;
}

function launchReport(reportId, queryStringParams) {

    var width = 400;
    var height = 450;
    var left = (screen.width - width) / 2;
    var top = (screen.height - height) / 2;
    var params = 'width=' + width + ', height=' + height;
    params += ', top=' + top + ', left=' + left;
    params += ', directories=no';
    params += ', location=no';
    params += ', menubar=no';
    params += ', resizable=yes';
    params += ', scrollbars=yes';
    params += ', status=no';
    params += ', toolbar=no';
    var newWindow = window.open('../Exhibitors/Report.aspx?reportId='
    + reportId + '&' + queryStringParams, '_blank', params);

    if (window.focus && newWindow) {
        newWindow.focus()
    }
    return false;
}

function launchLabelReport(reportId, companyname, street1, street2, street3, street4, city, state, postalcode, country) {

    var queryString = 'companyname='
    + encodeURIComponent(companyname) + '&'
    + 'street1='
    + encodeURIComponent(street1) + '&'
    + 'street2='
    + encodeURIComponent(street2) + '&'
    + 'street3='
    + encodeURIComponent(street3) + '&'
    + 'street4='
    + encodeURIComponent(street4) + '&'
    + 'city='
    + encodeURIComponent(city) + '&'
    + 'state='
    + encodeURIComponent(state) + '&'
    + 'postalcode='
    + encodeURIComponent(postalcode) + '&'
    + 'country='
    + encodeURIComponent(country);

   launchReport(reportId, queryString);
}

function generateOutboundShippingLabel(reportId) {

    var companyname = document.getElementById('PageContent_txtCompanyName').value;
    var street1 = document.getElementById('PageContent_txtAddressLine1').value;
    var street2 = document.getElementById('PageContent_txtAddressLine2').value;
    var street3 = document.getElementById('PageContent_txtAddressLine3').value;
    var street4 = document.getElementById('PageContent_txtAddressLine4').value;

    var city = document.getElementById('PageContent_txtCity').value;
    var state = document.getElementById('PageContent_txtState').value;
    var postalCode = document.getElementById('PageContent_txtPostalCode').value;
    var country = document.getElementById('PageContent_txtCountry').value;
    launchLabelReport(reportId, companyname, street1, street2, street3, street4, city, state, postalCode, country);
}

function launchOrderReceipt(reportId, orderId) {
    var queryString = 'orderId=' + orderId;
    launchReport(reportId, queryString);
}

function launchInstallDismantleReport(reportId, orderItemId) {
    var queryString = 'orderItemId=' + orderItemId;
    launchReport(reportId, queryString);
}

function launchExhibitorInvoice(reportId, showId, exhibitorId) {
    var queryString = 'showId=' + showId + '&exhibitorId=' + exhibitorId;
    launchReport(reportId, queryString);
}


function formSubmitting() {

}

Number.prototype.formatMoney = function (c, d, t, symb) {
    if (!symb)
        symb = '$';

    var n = this, c = isNaN(c = Math.abs(c)) ? 2 : c, d = d == undefined ? ',' : d, t = t == undefined ? '.' : t, s = n < 0 ? '-' : '',
    i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + '', j = (j = i.length) > 3 ? j % 3 : 0;
    return symb + s + (j ? i.substr(0, j) + t : '') + i.substr(j).replace(/(\d{3})(?=\d)/g, symb + '1' + t)
    + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : '');
};

function closeWindow(win) {
    if (win == null)
        win = window;

    win.close();
}


function closeCurrentWindow() {
    window.close();
}



function submitWait() {

    // <form onsubmit="document.body.style.cursor='wait';"

}


function noenter() {
    return !(window.event && window.event.keyCode == 13);
}



function isValidKey(valid) {
    // verify that the key being pressed will insert a 
    // valid character (valid) into the field. 
    // if not, remove the character from the keyboard buffer.
    var intKey = event.keyCode
    var strKey = String.fromCharCode(intKey)

    if (valid.indexOf(strKey) == -1) {
        event.keyCode = null;
        return false;
    } else {
        return true;
    }
}

function isInValidKey(intKey) {
    // BETA does not like |, ^ or ~
    var inValid = "|^~";
    var strKey = String.fromCharCode(intKey)
    if (inValid.indexOf(strKey) == -1) {
        return false;
    } else {
        event.keyCode = null;
        return true;
    }
}

function isInValidKeypress(inValid) {
    var intKey = event.keyCode
    var strKey = String.fromCharCode(intKey)
    if (inValid.indexOf(strKey) == -1) {
        return false;
    } else {
        event.keyCode = null;
        return true;
    }
}



//Force User Input to be alphabetical, no punctuation
function onkeypress_alpha() {
    var valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ\t";
    return isValidKey(valid);
}

//Force User Input to be alphanumeric, no punctuation
function onkeypress_alphanumeric() {
    var valid = " 0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ\t";
    return isValidKey(valid);
}

function onkeypress_text() {
    // |, ^ and ~ cause issues on BETA, we will not allow these
    var key = event.keyCode
    return !isInValidKey(key);
}

function onkeypress_InvalidSpecialChars() {
    var invalid = "<>^|~";
    return isInValidKeypress(invalid);
}

function onkeypress_number() {
    var valid = "0123456789\t";
    return isValidKey(valid);
}

function onkeypress_decimal() {
    var valid = "0123456789.\t";
    return isValidKey(valid);
}

function openPopupWindow(popupName, url, popupWidth, popupHeight) {
    var popupLeft = Math.floor((screen.width - popupWidth) / 2);
    var popupTop = Math.floor((screen.height - popupHeight) / 2);
    var popupWindow;
    var displayOptions = 'toolbar=no,menubar=no,location=no,status=yes,scrollbars=yes,resizable=yes,width=' + popupWidth + ',height=' + popupHeight + ',top=' + popupTop + ',left=' + popupLeft;

    popupWindow = window.open(url, popupName, displayOptions);
    popupWindow.focus();
    return popupWindow;
}


function MM_preloadImages() { //v3.0
    var d = document; if (d.images) {
        if (!d.MM_p) d.MM_p = new Array();
        var i, j = d.MM_p.length, a = MM_preloadImages.arguments; for (i = 0; i < a.length; i++)
            if (a[i].indexOf("#") != 0) { d.MM_p[j] = new Image; d.MM_p[j++].src = a[i]; }
    }
}
function MM_swapImgRestore() { //v3.0
    var i, x, a = document.MM_sr; for (i = 0; a && i < a.length && (x = a[i]) && x.oSrc; i++) x.src = x.oSrc;
}
function MM_findObj(n, d) { //v4.01
    var p, i, x; if (!d) d = document; if ((p = n.indexOf("?")) > 0 && parent.frames.length) {
        d = parent.frames[n.substring(p + 1)].document; n = n.substring(0, p);
    }
    if (!(x = d[n]) && d.all) x = d.all[n]; for (i = 0; !x && i < d.forms.length; i++) x = d.forms[i][n];
    for (i = 0; !x && d.layers && i < d.layers.length; i++) x = MM_findObj(n, d.layers[i].document);
    if (!x && d.getElementById) x = d.getElementById(n); return x;
}

function MM_swapImage() { //v3.0
    var i, j = 0, x, a = MM_swapImage.arguments; document.MM_sr = new Array; for (i = 0; i < (a.length - 2); i += 3)
        if ((x = MM_findObj(a[i])) != null) { document.MM_sr[j++] = x; if (!x.oSrc) x.oSrc = x.src; x.src = a[i + 2]; }
}