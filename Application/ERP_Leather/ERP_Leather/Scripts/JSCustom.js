//--------------------------------------- Updated Code Library for Bengal Leather--------------------------------------------
function ResetOnlyMasterPageData() {
    $(".txtBox").val("");
    $("#MessageText").html("");
}

function ResetOnlyParentData() {
    $(".txtBox").val("");
}

function CheckRecordStatusForButton(recordStatus) {
    switch (recordStatus) {
        case "CNF":
            $("#btnCheck").attr("disabled", "disabled");
            $("#btnConfirm").attr("disabled", "disabled");
            $("#btnSave").attr("disabled", "disabled");
            $("#btnDelete").attr("disabled", "disabled");
            $("#btnApprove").removeAttr("disabled");
            break;
        case "Confirmed":
            $("#btnCheck").attr("disabled", "disabled");
            $("#btnConfirm").attr("disabled", "disabled");
            $("#btnSave").attr("disabled", "disabled");
            $("#btnDelete").attr("disabled", "disabled");
            $("#btnApprove").removeAttr("disabled");
            break;
        case "CHK":
            $("#btnCheck").attr("disabled", "disabled");
            $("#btnConfirm").removeAttr("disabled");
            $("#btnSave").removeAttr("disabled");
            $("#btnDelete").removeAttr("disabled");
            $("#btnApprove").removeAttr("disabled");
            break;
        case "Checked":
            $("#btnCheck").attr("disabled", "disabled");
            $("#btnConfirm").removeAttr("disabled");
            $("#btnSave").removeAttr("disabled");
            $("#btnDelete").removeAttr("disabled");
            break;

        case "APV":
            $("#btnCheck").attr("disabled", "disabled");
            $("#btnConfirm").attr("disabled", "disabled");
            $("#btnSave").attr("disabled", "disabled");
            $("#btnDelete").attr("disabled", "disabled");
            $("#btnApprove").attr("disabled", "disabled");
            $("#btnSubmit").attr("disabled", "disabled");
            break;
        case "Approved":
            $("#btnCheck").attr("disabled", "disabled");
            $("#btnConfirm").attr("disabled", "disabled");
            $("#btnSave").attr("disabled", "disabled");
            $("#btnDelete").attr("disabled", "disabled");
            $("#btnApprove").attr("disabled", "disabled");
            $("#btnSubmit").attr("disabled", "disabled");
            break;
        default:
            $("#btnCheck").removeAttr("disabled");
            $("#btnConfirm").removeAttr("disabled");
            $("#btnSave").removeAttr("disabled");
            $("#btnDelete").removeAttr("disabled");
            $("#btnApprove").removeAttr("disabled");
            break;
        case "RCV":
            $("#btnCheck").attr("disabled", "disabled");
            $("#btnConfirm").attr("disabled", "disabled");
            $("#btnSave").attr("disabled", "disabled");
            $("#btnDelete").attr("disabled", "disabled");
            $("#btnApprove").removeAttr("disabled");
            break;
        case "Received":
            $("#btnCheck").attr("disabled", "disabled");
            $("#btnConfirm").attr("disabled", "disabled");
            $("#btnSave").attr("disabled", "disabled");
            $("#btnDelete").attr("disabled", "disabled");
            $("#btnApprove").removeAttr("disabled");
            break;
        case "ACK":
            $("#btnCheck").attr("disabled", "disabled");
            $("#btnConfirm").attr("disabled", "disabled");
            $("#btnSave").attr("disabled", "disabled");
            $("#btnDelete").attr("disabled", "disabled");
            $("#btnApprove").removeAttr("disabled");
            break;
    }
}

//Clear operational  Message
function ClearOperationalMessage() {
    $('.FormBody').click(function () {
        $("#MessageText").html("");
    });
}

function ClearRequiredTextBoxRedColor() {
    $(".txtBox").on('input', function () {
        $(".txtBox").css("border-color", ""); //Clear Red Color
    });

    $(".txtBox").on('change', function () {
        $(".txtBox").css("border-color", ""); //Clear Red Color
    });

    //$(".RequiredTextBox").on('input', function () {
    //    $(".RequiredTextBox").css("border-color", "white"); //Clear Red Color
    //});

    //$(".DropDown").on('change', function () {
    //    $(".RequiredTextBox").css("border-color", "white"); //Clear Red Color
    //});
}

function CheckRequiredTextBoxMaxLength(TextBoxId, length) {
    if ((jQuery.trim($('#' + TextBoxId).val()).length == 0) || (jQuery.trim($('#' + TextBoxId).val()).length > length)) {
        $("#MessageText").html("Please Enter the Required Data Properly.");
        $("#MessageText").css({ 'color': 'red', 'font-size': 'larger', 'font-weight': 'bold', 'text-align': 'left' });
        $('#' + TextBoxId).css("border-color", "red");
        saveStatus = 0;
    }
    else {
        $('#' + TextBoxId).css("border-color", "");
    }
}

// Change Event in all TextBox
function CheckUnSavedOnlyMasterData() {

    jQuery(".txtBox").on('input', function () {
        changeStatus = 1;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredTextBox").val()).length > 0)
            $(".RequiredTextBox").css("border-color", "white");
    });
    jQuery("textarea").on('input', function () {
        changeStatus = 1;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredTextBox").val()).length > 0)
            $(".RequiredTextBox").css("border-color", "white");
    });
    $(".Dropdown").change(function () {
        changeStatus = 1;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredTextBox").val()).length > 0)
            $(".RequiredTextBox").css("border-color", "white");
    });
    $(".noChangeTypeText").change(function () {
        changeStatus = 0;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredTextBox").val()).length > 0)
            $(".RequiredTextBox").css("border-color", "white");
    });
    jQuery(".datePicker").change('input', function () {
        changeStatus = 1;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredTextBox").val()).length > 0)
            $(".RequiredTextBox").css("border-color", "white");
    });
}

function CheckUnSavedMasterData() {
    $(".txtBox").change(function () {
        changeStatus = 1;
        $(".txtBox").css("border-color", ""); //Clear Red Color
    });
}

function CheckUnSavedMasterDetailData() {
    $(".txtBox").change(function () {
        changeStatus = 1;
        $(".txtBox").css("border-color", ""); //Clear Red Color
    });
    $(".grdKendo").on('input', function () {
        changeStatus = 1;
    });
}

var SaveChangeWindow;
function OpenSaveChangesDialog() {
    SaveChangeWindow = $('#SaveChangeWindow').kendoWindow({
        actions: ["Minimize", "Maximize", "Close"],
        visible: false,
        width: "210px",
        height: "auto",
        title: "Confirmation!",
        position: { top: 100, left: 400 },
        modal: true,
        draggable: true
    }).data('kendoWindow');
    SaveChangeWindow.open();
}

function CloseSaveChangesDialog() {
    SaveChangeWindow.close();
}


var SaveChangeWindowOneButton;
function OpenSaveChangesDialogOneButton() {
    SaveChangeWindowOneButton = $('#ConfirmWindow').kendoWindow({
        actions: ["Minimize", "Maximize", "Close"],
        visible: false,
        width: "180px",
        height: "auto",
        title: "Confirmation!",
        position: { top: 100, left: 400 },
        modal: true,
        draggable: true
    }).data('kendoWindow');
    SaveChangeWindowOneButton.open();
}

function CloseSaveChangesDialogOneButton() {
    SaveChangeWindowOneButton.close();
}


// Setting the kendo DeleteConfirmWindow to take confirmation before deleting grid row
var ConfirmWindow;
function OpenConfirmationDialog() {
    ConfirmWindow = $('#DeleteConfirmWindow').kendoWindow({
        width: "200px",
        padding: "0px 0px 0px 100px",
        title: "Confirmation!",
        visible: false,
        resizable: false,
        draggable: true,
        position: { top: 200, left: 400 }
    }).data('kendoWindow');
    ConfirmWindow.open();
}

function CloseConfirmationDialog() {
    ConfirmWindow.close();
}

function ClearOperationMsgTextBoxRedColor() {
    $(".RequiredTextBox").css("border-color", "white"); //Clear Red Color
}


// Setting the kendo DeleteConfirmWindow to take confirmation before deleting grid row
var ExecuteConfirmWindow;
function OpenExecuteConfirmationDialog() {
    ExecuteConfirmWindow = $('#ExecuteConfirmWindow').kendoWindow({
        width: "200px",
        padding: "0px 0px 0px 100px",
        title: "Confirmation!",
        visible: false,
        resizable: false,
        draggable: true,
        position: { top: 200, left: 400 }
    }).data('kendoWindow');
    ExecuteConfirmWindow.open();
}

function CloseConfirmationExecuteDialog() {
    ExecuteConfirmWindow.close();
}






//// Display Date into kendo Grid Row
//function dateTimeEditor(container, options) {
//    $('<input data-text-field="' + options.field + '" data-value-field="' + options.field + '" data-bind="value:' + options.field + '" data-format="' + options.format + '"/>')
//            .appendTo(container)
//            .kendoDatePicker({});
//}
//// Reset Data
//function ResetData() {
//    $(":text").val("");
//    $("textarea").val("");
//    $(".hiddenField").val("");
//    $("#CrudStatus").val(1);
//    displayFocus = 0;
//    changeStatus = 0;
//    selectedStatus = 0;
//    rowNumber = 0;
//    noOfRow = 0;
//    resetDisplayStatus = 0;
//    mstSearchStatus = 0;
//    mstSearchData = [];
//    mstSearchRowNumber = 0;
//    mstSearchNoOfRow = 0;
//    $(".SetFocus").focus();
//}

////function ResetOnlyMasterPageData() {
////    $(":text").val("");
////    //$(".hiddenField").val("");
////    $("textarea").val("");
////    //$("#ActiveStatus").val("Active");
////    //$("#CrudStatus").val(1);
////    //changeStatus = 0;
////    //resetEditStatus = 0;
////    //dataitem = {};
////    //$(".SetFocus").focus();
////    //ClearDetailGridFilterAndPaging();
////    $("#MessageText").html("");
////    $(".RequiredTextBox").css("border-color", "white"); //Clear Red Color
////    $(".RequiredDropdown").css("border-color", "white"); //Clear Red Color
////    $(".Dropdown").val("");
////}

//// Check Grid Mail Address
//function checkEmail(val) {
//    //var emailFormat = /^[A-Z0-9._%+-]+@@[A-Z0-9.-]+\.[A-Z]{2,4}$/i;
//    var pattern = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i);
//    if (val == null) {
//        return true; //if input is empty.
//    }
//    else if (pattern.test(val) == false) {
//        return false;
//    }
//    return true;
//}



//var SearchDialog;
//function OpenSearchDialog() {
//    //To Define kendoWindow for Search Master Data
//    SearchDialog = $('#SearchWindow').kendoWindow({
//        actions: ["Minimize", "Maximize", "Close"],
//        visible: false,
//        width: "650px",
//        height: "auto",
//        title: "Master Record",
//        position: { top: 100, left: 300 },
//        modal: true,
//        draggable: true
//    }).data('kendoWindow');
//    SearchDialog.open();
//}

//function CloseSearchDialog() {
//    SearchDialog.close();
//}

//var ListOfValueDialog;
////To Define kendoWindow for Search InfoForName(List Of Value) Data
//function OpenListOfValueDialog() {
//    ListOfValueDialog = $('#ListOfValueWindow').kendoWindow({
//        actions: ["Minimize", "Maximize", "Close"],
//        visible: false,
//        width: "500px",
//        height: "auto",
//        title: "Infomation List Of Value",
//        position: { top: 100, left: 400 },
//        modal: true,
//        draggable: true
//    }).data('kendoWindow');

//    ListOfValueDialog.open();
//}

//function CloseListOfValueDialog() {
//    ListOfValueDialog.close();
//}

//var ListOfValueDialog2;
////To Define kendoWindow for Search InfoForName(List Of Value) Data
//function OpenListOfValueDialog2() {
//    ListOfValueDialog2 = $('#ListOfValueWindow2').kendoWindow({
//        actions: ["Minimize", "Maximize", "Close"],
//        visible: false,
//        width: "500px",
//        height: "auto",
//        title: "Infomation List Of Value",
//        position: { top: 100, left: 400 },
//        modal: true,
//        draggable: true
//    }).data('kendoWindow');

//    ListOfValueDialog2.open();
//}

//function CloseListOfValueDialog2() {
//    ListOfValueDialog2.close();
//}


//// Setting the kendo DeleteConfirmWindow to take confirmation before deleting grid row
//var ConfirmWindow;
//function OpenConfirmationDialog() {
//    ConfirmWindow = $('#DeleteConfirmWindow').kendoWindow({
//        width: "300px",
//        padding: "0px 0px 0px 100px",
//        title: "Do You Want To Delete This Record?",
//        visible: false,
//        draggable: true,
//        position: { top: 200, left: 400 }
//    }).data('kendoWindow');

//    ConfirmWindow.open();
//}

//function CloseConfirmationDialog() {
//    ConfirmWindow.close();
//}

//// Setting the kendo DeleteConfirmWindow to take confirmation(Any Change Event)


////For Selecting First Row of Search Popup Grid
//function SelectedItemOfSearchGrid() {
//    var gridmst = $("#SearchWindowGrid").data("kendoGrid");
//    gridmst.dataSource.read();
//    $("#SearchWindowGrid").data("kendoGrid").dataSource.filter([]);
//    $('#SearchWindowGrid').data().kendoGrid.bind('dataBound', function (e) {
//        this.element.find('tbody tr:first').addClass('k-state-selected');
//    });
//}




//// Change Event in all TextBox
//function CheckUnSavedMasterDetailData() {

//    jQuery(":text").on('input', function () {
//        changeStatus = 1;
//        $("#MessageText").html("");
//        if (jQuery.trim($(".RequiredTextBox").val()).length > 0)
//            $(".RequiredTextBox").css("border-color", "white");
//    });

//    jQuery("textarea").on('input', function () {
//        changeStatus = 1;
//        $("#MessageText").html("");
//        if (jQuery.trim($(".RequiredTextBox").val()).length > 0)
//            $(".RequiredTextBox").css("border-color", "white");
//    });
//    $(".CommonDropDown").change(function () {
//        changeStatus = 1;
//        $("#MessageText").html("");
//        if (jQuery.trim($(".RequiredTextBox").val()).length > 0)
//            $(".RequiredTextBox").css("border-color", "white");
//    });
//    $(".noChangeTypeText").change(function () {
//        changeStatus = 0;
//        $("#MessageText").html("");
//        if (jQuery.trim($(".RequiredTextBox").val()).length > 0)
//            $(".RequiredTextBox").css("border-color", "white");
//    });
//    jQuery(".date").change('input', function () {
//        changeStatus = 1;
//        $("#MessageText").html("");
//        if (jQuery.trim($(".RequiredTextBox").val()).length > 0)
//            $(".RequiredTextBox").css("border-color", "white");
//    });
//    //// Change Event in PageDetailGrid Detail Grid
//    //jQuery("#PageDetailGrid").on('input', function () {
//    //    changeStatus = 1;
//    //});
//    // Change Event in PageDetailGrid Detail Grid
//    jQuery(".MainGrid").on('input', function () {
//        changeStatus = 1;
//    });
//}

//function DeleteOnlyMasterPageData(object, url) {
//    $.ajax({
//        url: url,
//        data: JSON.stringify(object),
//        type: 'POST',
//        contentType: 'application/json;',
//        dataType: 'json',
//        success: function (response) {
//            if (response == "Deletesuccess") {
//                $("#MessageText").html("Delete Successful");
//                $("#MessageText").css("color", "green");
//                ResetOnlyMasterPageData();
//                ClearDetailGridFilterAndPaging();
//                PageDetailGridReload();
//                changeStatus = 0;
//            }
//            else if (response == "ORA-02292") {
//                $("#MessageText").html("This Record Used In Another Transaction.");
//                $("#MessageText").css("color", "red");
//                PageDetailGridReload();
//                changeStatus = 0;
//            }
//            else {
//                $("#MessageText").html("Error Occurred.");
//                $("#MessageText").css("color", "red");
//                changeStatus = 0;
//            }
//        }
//    });
//}

////For Selecting First Row of Search Popup Grid
//function SelectedIFirstRowAndPageAndRemoveFilter(GridID) {
//    var gridmst = $("#" + GridID).data("kendoGrid");
//    gridmst.dataSource.read();
//    $("#" + GridID).data("kendoGrid").dataSource.filter([]);
//    $("#" + GridID).data().kendoGrid.bind('dataBound', function (e) {
//        this.element.find('tbody tr:first').addClass('k-state-selected');
//    });
//    $("#" + GridID).data("kendoGrid").dataSource.page(1); //First Page
//}


function ClearDetailGridFilterAndPaging(gridName) {
    $('#' + gridName).data("kendoGrid").dataSource.filter([]); //Clear Filter
    $('#' + gridName).data("kendoGrid").dataSource.page(1); //First Page
}
//Page Detail Grid Reload
function GridReload(gridName) {
    $('#' + gridName).data('kendoGrid').dataSource.read();
    $('#' + gridName).data('kendoGrid').refresh();
    //var PageDetailGrid = $("#PageDetailGrid").data("kendoGrid");
    //PageDetailGrid.dataSource.read();
}

function OnlyNumericValue(textField) {
    $('#' + textField).keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 190]) !== -1 ||
            // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
            // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || e.keyCode == 110 || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });
}

function NumericAndDecimalValue(textField) {
    $('#' + textField).keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
            // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });
}

//// Check Numeric field which can't be more than 10 digit & not required
//function CheckNumericLength(valu, length) {
//    var exe = /^[0-9]+$/;
//    if (valu == "") {
//        return true; //if input is empty.
//    }
//    else if (valu.search(exe) == -1) {
//        return false;
//    }
//    else if (valu.length > length) {
//        return false;
//    }
//    return true;
//}
//// Check Numeric field which can't be more than 10 digit & required
//function CheckRequiredNumericLength(val, length) {
//    var ex = /^[0-9]+$/;
//    if (val == "") {
//        return false;
//    }
//    if (val.search(ex) == -1) {
//        return false;
//    }
//    else if (val.length > length) {
//        return false;
//    }
//    else
//        return true;
//}

function parseDate(str) {
    var splitstr = str.split('/');
    return new Date(splitstr[2], splitstr[1] - 1, splitstr[0]);
}

function daydiff(first, second) {
    return (second - first) / (1000 * 60 * 60 * 24);
}
function CheckDiffTwoDates(first1, second1) {
    var diffdate = daydiff(parseDate($('#' + first1).val()), parseDate($('#' + second1).val()));
    if (diffdate < 0) {
        $("#MessageText").html("Please Enter the Required Data Properly.");
        $("#MessageText").css({ 'color': 'red', 'font-size': 'larger', 'font-weight': 'bold', 'text-align': 'left' });
        $('#' + second1).css("border-color", "red");
        saveStatus = 0;
    }
}
function daydiffSysDate(first, second) {
    return (first - second) / (1000 * 60 * 60 * 24);
}
function CheckDiffSysDateAndOneInpDates(first1, second1) {
    var diffdate = daydiffSysDate(parseDate(first1), parseDate($('#' + second1).val()));
    if (diffdate < 0) {
        $("#MessageText").html("Please Enter the Required Data Properly.");
        $("#MessageText").css({ 'color': 'red', 'font-size': 'larger', 'font-weight': 'bold', 'text-align': 'left' });
        $('#' + second1).css("border-color", "red");
        saveStatus = 0;
    }
}

//function CheckRqrdTextBoxMaxLength(TextBoxId, len, message) {
//    if ((jQuery.trim($('#' + TextBoxId).val()).length == 0) || (jQuery.trim($('#' + TextBoxId).val()).length < len)) {
//        $("#MessageText").html(message);
//        $("#MessageText").css("color", "red");
//        $('#' + TextBoxId).css("border-color", "red");
//        saveStatus = 0;
//    }
//}

//function CheckRequiredTextBoxMaxLength(TextBoxId, length) {
//    if ((jQuery.trim($('#' + TextBoxId).val()).length == 0) || (jQuery.trim($('#' + TextBoxId).val()).length > length)) {
//        $("#MessageText").html("Please Enter Data Properly");
//        $("#MessageText").css("color", "red");
//        $('#' + TextBoxId).css("border-color", "red");
//        saveStatus = 0;
//    }
//}

function CheckNormalTextBoxMaxLength(TextBoxId, length) {
    if (jQuery.trim($('#' + TextBoxId).val()).length > length) {
        $("#MessageText").html("Please Enter the Required Data Properly.");
        $("#MessageText").css({ 'color': 'red', 'font-size': 'larger', 'font-weight': 'bold', 'text-align': 'left' });
        $('#' + TextBoxId).css("border-color", "red");
        saveStatus = 0;
    }
}

function SetMasterData(data) {
    $.each(data, function (key, value) {
        $('#' + key).val(value);
    });
}

//function SetMasterDetailsData(data) {
//    $.each(data, function (key, value) {
//        $('#' + key).val(value);
//    });
//    $("#PageDetailGrid").data("kendoGrid").dataSource.data(data.DetailList);
//    //////////////////////////////////////////////// change date 11/02/2014
//    noOfRow = data.LastRow;
//    //rowNumber = 0;
//    //ClearDetailGridFilterAndPaging();
//    //mstSearchStatus = 0;
//    //changeStatus = 0;
//    $(".SetFocus").focus();
//}

//function RequestMasterDetailsData(RequestedURL) {
//    $.ajax({
//        url: RequestedURL,
//        type: 'GET',
//        contentType: 'application/json;',
//        dataType: 'json',
//        success: function (data) {
//            $("#CrudStatus").val(data.CrudStatus);
//            SetMasterDetailsData(data);
//            noOfRow = data.LastRow;
//            rowNumber = 0;
//            ClearDetailGridFilterAndPaging();
//        }
//    });
//    mstSearchStatus = 0;
//    changeStatus = 0;
//    $(".SetFocus").focus();
//}

//function RequestMasterDetailsOnlyTextBoxData(RequestedURL) {
//    $.ajax({
//        url: RequestedURL,
//        type: 'GET',
//        contentType: 'application/json;',
//        dataType: 'json',
//        success: function (data) {
//            $("#CrudStatus").val(data.CrudStatus);
//            SetMasterData(data);
//            noOfRow = data.LastRow;
//            rowNumber = 0;
//        }
//    });
//    mstSearchStatus = 0;
//    changeStatus = 0;
//    $(".SetFocus").focus();
//}

//function RequestMasterDetailsTextBoxAndGridData(RequestedURL) {
//    $.ajax({
//        url: RequestedURL,
//        type: 'GET',
//        contentType: 'application/json;',
//        dataType: 'json',
//        success: function (data) {
//            $("#CrudStatus").val(data.CrudStatus);
//            SetMasterData(data);
//            noOfRow = data.LastRow;
//            rowNumber = 0;
//        }
//    });
//    mstSearchStatus = 0;
//    changeStatus = 0;
//    $(".SetFocus").focus();
//}

//function RequestDetailsForSelectedSearchedItem(RequestedURL, MasterCode) {
//    $.ajax({
//        url: RequestedURL,
//        type: 'GET',
//        data: { "MasterCode": MasterCode },
//        success: function (data) {
//            $("#PageDetailGrid").data("kendoGrid").dataSource.data(data.DetailList);
//            ClearDetailGridFilterAndPaging();
//        }
//    });
//}

//function RequestMasterDetailsNextData(RequestedURL, rowNumber) {
//    $.ajax({
//        url: RequestedURL,
//        type: 'GET',
//        data: { "rowNumber": rowNumber },
//        success: function (data) {
//            SetMasterDetailsData(data);
//            noOfRow = data.LastRow;
//        }
//    });
//}

////To Bring Data from Search Grid to Main Page
//function SearchGridEvent() {
//    var grid = $("#SearchWindowGrid").data("kendoGrid");
//    var selectedItem = (grid.dataItem(grid.select())); //Selected Row
//    // Gets the data source from the grid.
//    var dataSource = $("#SearchWindowGrid").data("kendoGrid").dataSource;
//    // Gets the filter from the dataSource
//    var filters = dataSource.filter();
//    // Gets the full set of data from the data source
//    var allData = dataSource.data();
//    // Applies the filter to the data
//    var query = new kendo.data.Query(allData);
//    mstSearchData = query.filter(filters).data; //Search Master Data
//    if (mstSearchData.length > 0)
//        $(".SetFocus").focus();
//    if (allData.length != mstSearchData.length)// For Not Search Data
//        selectedStatus = 1;
//    mstSearchNoOfRow = mstSearchData.length - 1; //Search Number Of Row
//    mstSearchStatus = 1;
//    $("#CrudStatus").val(2);
//    SetMasterData(selectedItem);
//    RequestDetailsForSelectedSearchedItem('/HRMC_CVI01/GetDtlList', selectedItem.MasterCode);
//    changeStatus = 0;
//}

//function SearchWindowEvents() {
//    //Handling OK button click for search window grid
//    $('#btnSearchWindowOK').click(function () {
//        ClearOperationMsgTextBoxRedColor();
//        SearchGridEvent();
//        CloseSearchDialog();
//    });

//    //Handling Cancel button click for search window grid
//    $('#btnSearchWindowCancel').click(function () {
//        mstSearchStatus = 0;
//        selectedStatus = 0;
//        CloseSearchDialog();
//    });
//    //Handling double click for Selected search window grid
//    $("#SearchWindow").dblclick(function () {
//        ClearOperationMsgTextBoxRedColor();
//        SearchGridEvent();
//        CloseSearchDialog();
//    });

//    // Enter Key Event For search window grid
//    $("#SearchWindow").keypress(function (event) {
//        var keycode = (event.keyCode ? event.keyCode : event.which);
//        if (keycode == '13') {
//            ClearOperationMsgTextBoxRedColor();
//            SearchGridEvent();
//            CloseSearchDialog();
//        }
//            //Escape button press event
//        else if (keycode == '27') {
//            CloseSearchDialog();
//        }
//        event.stopPropagation();
//    });
//}

//function DisplayFilteredMasterSearchData(mstSearchData, mstSearchRowNumber) {
//    $.each(mstSearchData[mstSearchRowNumber], function (key, value) {
//        $('#' + key).val(value);
//    });
//    RequestDetailsForSelectedSearchedItem('/HRMC_CVI01/GetDtlList', mstSearchData[mstSearchRowNumber].MasterCode);
//}

//function DeletePageDetailsGridRow(RequestedURL, DetailCode) {
//    $.ajax({
//        url: RequestedURL,
//        type: 'GET',
//        data: { "DetailCode": DetailCode },
//        contentType: 'application/json;',
//        dataType: 'json',
//        success: function (response) {
//            if (response == "Deletesuccess") {
//                $("#MessageText").html("Delete Successful");
//                $("#MessageText").css("color", "green");
//            }
//            else {
//                $("#MessageText").html(response.Message);
//                $("#MessageText").css("color", "green");
//            }
//        }
//    });
//}

//function DeleteMasterData(RequestedURL, DataToDelete) {
//    var dataSource = $("#PageDetailGrid").data("kendoGrid").dataSource;
//    var deldata = dataSource.data();
//    if (deldata.length > 0) {
//        $("#MessageText").html("Child Record Found");
//        $("#MessageText").css("color", "red");
//    }
//    else {
//        //hrmcmnvlumstInfo.MasterCode = $("#MasterCode").val();
//        //hrmcmnvlumstInfo.CrudStatus = 3; //Deleted status
//        $.ajax({
//            url: RequestedURL,
//            data: JSON.stringify(DataToDelete),
//            type: 'POST',
//            contentType: 'application/json;',
//            dataType: 'json',
//            success: function (response) {
//                if (response == "Deletesuccess") {
//                    RequestMasterDetailsNextData('/HRMC_CVI01/GetNextData', rowNumber);
//                    $("#MessageText").html("Delete Successful");
//                    $("#MessageText").css("color", "green");
//                    changeStatus = 0;
//                    $("#CrudStatus").val("1");
//                    $(".SetFocus").focus();
//                }
//                else {
//                    $("#MessageText").html(response.Message);
//                    $("#MessageText").css("color", "green");
//                }
//            }
//        });
//    }
//}

//function RequestedNextDataForUpAndDownArrow(RequestedURL, rowNumber) {
//    $.ajax({
//        url: RequestedURL,
//        type: 'GET',
//        data: { "rowNumber": rowNumber },
//        success: function (data) {
//            SetMasterDetailsData(data);
//            ClearDetailGridFilterAndPaging();
//            changeStatus = 0;
//            $(".SetFocus").focus();
//        } //End of Success Call
//    });
//}

//function RequestedNextOnlyTextBoxDataForUpAndDownArrow(RequestedURL, rowNumber) {
//    $.ajax({
//        url: RequestedURL,
//        type: 'GET',
//        data: { "rowNumber": rowNumber },
//        success: function (data) {
//            SetMasterData(data);
//            changeStatus = 0;
//            $(".SetFocus").focus();
//        } //End of Success Call
//    });
//}

////Setting data in json format for kendo drop down control
//var ddlDataSourceYesNo = [{ text: "Yes", value: "Yes" }, { text: "No", value: "No" }];
////Setting Kendo drop down with above mentioned data
//function ddlYesNo(container, options) {
//    $('<input name="IntermailAddressStatus" data-bind="value:' + options.field + '"/>')
//            .appendTo(container)
//            .kendoDropDownList({
//                dataTextField: "text",
//                dataValueField: "value",
//                optionLabel: "Select",
//                autoBind: false,
//                change: function (e) {
//                    changeStatus = 1;
//                },
//                dataSource: ddlDataSourceYesNo
//            });
//};

////Setting data in json format for kendo drop down control
//var ddlDataSourceActiveInactive = [{ text: "Select", value: "Select" }, { text: "Active", value: "Active" }, { text: "Inactive", value: "Inactive" }];
////Setting Kendo drop down with above mentioned data
//function ddlActiveInactive(container, options) {
//    $('<input id="ActiveStatus"  data-text-field="text" data-value-field="value" data-bind="value:' + options.field + '"/>')
//            .appendTo(container)
//            .kendoDropDownList({
//                dataTextField: "text",
//                dataValueField: "value",
//                change: function (e) {
//                    changeStatus = 1;
//                },
//                dataSource: ddlDataSourceActiveInactive
//            });
//};
function getISODateTime(d) {
    // padding function
    var s = function (a, b) { return (1e15 + a + "").slice(-b) };
    // default date parameter
    if (typeof d === 'undefined') {
        d = new Date();
    };
    // return ISO datetime
    return s(d.getDate(), 2) + '/' +
        s(d.getMonth() + 1, 2) + '/' +
       d.getFullYear();
}

function IsNumber(id) {
    var val = $("#" + id + "").val();
    if (!$.isNumeric(val)) {
        $("#" + id + "").val('');
    }
}


function nameCheck(element) {
    $('#' + element).blur(function () {
        var stri = $('#' + element).val();//the input element
        var characters = "/ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var flag = false;
        for (var x = 0; x < stri.length; x++) {
            var ch = stri.charAt(x);
            var n = characters.indexOf(ch);
            if (n === -1) {//why does it always resolve to true
                flag = true;
                break;
            }
            else {
            }
        }
        if (flag) {
            $('#MessageText').html("Please input letters only");
            $("#MessageText").css({ 'color': 'red', 'font-size': 'larger', 'font-weight': 'bold', 'text-align': 'center' });
            $('#' + element).val("");
            setTimeout(function () {
                $('#' + element).focus();
            }, 1);
        }
    });
}

function numberCheck(element) {
    $('#' + element).blur(function () {
        var stri = $('#' + element).val();//the input element
        var characters = "0123456789.";
        var flag = false;
        for (var x = 0; x < stri.length; x++) {
            var ch = stri.charAt(x);
            var n = characters.indexOf(ch);
            if (n === -1) {//why does it always resolve to true
                flag = true;
                break;
            }
            else {
            }
        }
        if (flag) {
            $('#MessageText').html("Please input positive numbers only");
            $("#MessageText").css({ 'color': 'red', 'font-size': 'larger', 'font-weight': 'bold', 'text-align': 'center' });
            $('#' + element).val("");
            $('#' + element).css({ 'box-shadow': '1px 1px 1px red' });
            setTimeout(function () {
                $('#' + element).focus();
            }, 1);
            return 1;
        } else {
            $('#' + element).css({ 'box-shadow': 'none' });
        }
        return 0;
    });
}

function MeterToFootConversion(meter) {
    var result = meter / 0.0929;
    parseFloat(result);
    return result;
}

function FootToMeterConversion(foot) {
    var result = foot * 0.0929;
    parseFloat(result);
    return result;
}

function UnitPriceConversionToMeter(price) {
    var result = price / 0.0929;
    parseFloat(result);
    return result;
}

function UnitPriceConversionToFoot(price) {
    var result = price * 0.0929;
    parseFloat(result);
    return result;
}

function numChk(element) {
    var stri = $('#' + element).val();//the input element
    var characters = "0123456789.";
    var flag = false;
    for (var x = 0; x < stri.length; x++) {
        var ch = stri.charAt(x);
        var n = characters.indexOf(ch);
        if (n === -1) {//why does it always resolve to true
            flag = true;
            break;
        }
        else {
        }
    }
    if (flag) {
        $('#MessageText').html("Please input positive numbers only");
        $("#MessageText").css({ 'color': 'red', 'font-size': 'larger', 'font-weight': 'bold', 'text-align': 'center' });
        $('#' + element).val("");
        $('#' + element).css({ 'box-shadow': '1px 1px 1px red' });
        setTimeout(function () {
            $('#' + element).focus();
        }, 1);
        return 1;
    } else {
        $('#' + element).css({ 'box-shadow': 'none' });
    }
    return 0;
}

function FocusOnField() {
    $(".txtField").focus(function () {
        $(this).css({ 'box-shadow': '1px 1px 1px gray' });
    });
}

function FocusOutField() {
    $(".txtField").focusout(function () {
        $(this).css({ 'box-shadow': 'none' });
    });
}

//var adjustSerial = function(delItem, curntSerial) {
//    for (var i = 0; i < curntSerial.length; i++) {
//        if (curntSerial[i] > delItem) {
//            curntSerial[i] = curntSerial[i] - 1;
//        }
//    }
//    return curntSerial;
//};   

var buyerType = function(buyer) {
    switch (buyer) {
        case "FRN":
            return "Foreign";
            break;
        case "LCL":
            return "Local";
            break;
        case "SLF":
            return "Self";
            break;
        default:
            return "";
    }
};