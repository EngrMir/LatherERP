$(document).ready(function () {

    //--------------------------------------Start of defining Datasource & Datagrid----------------------------------------------------------------

    var ChallanGridDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "ChallanID",
                fields: {
                    ChallanID: { editable: false },
                    ChallanNo: { editable: true },
                    SourceID: { editable: true },
                    SourceName: { editable: true },
                    LocationID: { editable: true },
                    LocationName: { editable: true },
                    ChallanNote: { editable: true },
                    ChallanDate: { editable: true },
                    ReceiveStore: { editable: true },
                    ChallanCategory: { editable: true },
                }
            }
        },
        pageSize: 10
    });

    var ChallanGrid = $("#ChallanGrid").kendoGrid({
        dataSource: ChallanGridDataSource,
        pageable: true,
        editable: true,
        selectable: "row",
        navigatable: true,
        //toolbar: ["create"],
        filterable: true,
        sortable: true,
        height: 250,
        columns: [
              { field: "ChallanID", title: "Challan ID", width: "60px" },
              { field: "ChallanNo", title: "S. Challan No", width: "60px" },
              { field: "ChallanDate", title: "S.Challan Date", width: "80px", format: "{0:dd/MM/yyyy}", editor: dateTimeEditor },
              { field: "SourceName", title: "Source", width: "60px", attributes: { "class": "Source" } },
              { field: "LocationName", title: "Location", width: "60px", attributes: { "class": "Location" } },
              { field: "ChallanCategory", title: "Challan Category", width: "60px", editor: ddlChallanCategory },
              { field: "ReceiveStore", title: "Receive Store", width: "60px", editor: ddlReceiveStore },
              { field: "ChallanNote", title: "Remarks", width: "70px" }

        ]

    });


    var ItemGridDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "ChallanItemID",
                fields: {
                    ChallanItemID: { editable: false },
                    ChallanID: { editable: false },
                    ItemCategory: { editable: true },
                    ItemTypeID: { editable: true },
                    SizeID: { editable: true },
                    UnitID: { editable: true },
                    Description: { editable: true },
                    ChallanQty: { editable: true },
                    ReceiveQty: { editable: true },
                    Remark: { editable: true }

                }
            }
        },
        pageSize: 10

    });

    var ItemGrid = $("#ItemGrid").kendoGrid({
        dataSource: ItemGridDataSource,
        pageable: true,
        editable: true,
        edit: function (e) {
            var input = e.container.find(".k-input");
            var value = input.val();
            input.keyup(function () {
                value = input.val();
            });
            $("[name='ChallanQty']", e.container).blur(function (e) {
                var grid = $("#ItemGrid").data("kendoGrid");
                var row = $(this).closest("tr");
                var item = grid.dataItem(row);
                item.ReceiveQty = value;
            });
        },
        selectable: "row",
        navigatable: true,
        filterable: true,
        sortable: true,
        height: 250,
        //toolbar: ["create"],
        columns: [
                { field: "ChallanItemID", title: "Challan Item ID", width: "60px" },
                { field: "ChallanID", title: "ChallanID", width: "60px" },
                { field: "ItemTypeID", title: "Item Type", editor: ddlItemType, width: "60px" },
                { field: "SizeID", title: "Size", editor: ddlSize, width: "50px" },
                { field: "UnitID", title: "Unit", editor: ddlSize, width: "50px", editor: ddlUnit },
                { field: "Description", title: "Description", width: "70px" },
                { field: "ChallanQty", title: "Challan Qty", width: "60px" },
                { field: "ReceiveQty", title: "Receive Qty", width: "60px", attributes: { "class": "HolidayName" } },
                { field: "Remark", title: "Remarks", width: "70px" },
        ]

    });



    var SearchGridDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "PurchaseID",
                fields: {
                    PurchaseID: { editable: false },
                    SupplierID: { editable: false },
                    SupplierName: { editable: false },
                    SupplierAddressID: { editable: false },
                    Address: { editable: false },
                    PurchaseCategory: { editable: false },
                    PurchaseType: { editable: false },
                    PurchaseYear: { editable: false },
                    PurchaseDate: { editable: false }
                }
            }
        },
        pageSize: 10,
        transport: {
            read: {
                url: "/Purchase/GetPurchaseInformation",
                type: "GET",
                dataType: "JSON"
            }
        }
    });

    var SearchGrid = $("#SearchWindowGrid").kendoGrid({
        dataSource: SearchGridDataSource,
        pageable: true,
        groupable: true,
        editable: true,
        selectable: "row",
        navigatable: true,
        filterable: true,
        sortable: true,
        height: 320,
        columns: [
              { field: "PurchaseID", title: "Purchase Number", width: "60px" },
              { field: "SupplierName", title: "Supplier Name", width: "60px" },
              { field: "Address", title: "Supplier Address", width: "80px" },
              { field: "PurchaseCategory", title: "Purchase Category", width: "60px" },
              { field: "PurchaseType", title: "Purchase Type", width: "60px" },
              { field: "PurchaseYear", title: "Purchase Year", width: "60px" },
              { field: "PurchaseDate", title: "Purchase Date", width: "70px" }
        ]
    });

    var SupplierGridDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "SupplierID",
                fields: {
                    SupplierID: { editable: false },
                    SupplierName: { editable: false },
                    Address: { editable: false },
                    SupplierAddressID: { editable: false },
                    ContactPerson: { editable: false },
                    ContactNumber: { editable: false },
                }
            }
        },
        transport: {
            read: {
                url: "/Common/GetSupplierList",
                type: "GET",
                dataType: "JSON"
            }
        },
        pageSize: 10

    });

    var SupplierGrid = $("#SupplierWindowGrid").kendoGrid({
        dataSource: SupplierGridDataSource,
        pageable: true,
        editable: true,
        selectable: "row",
        navigatable: true,
        filterable: true,
        sortable: true,
        height: 300,
        columns: [
              { field: "SupplierID", title: "Supplier ID", width: "60px", filterable: false, sortable: false },
              { field: "SupplierName", title: "Supplier Name", width: "60px" },
              { field: "Address", title: "Address", width: "60px" },
              { field: "ContactPerson", title: "Contact Person", width: "60px" },
              { field: "ContactNumber", title: "Contact Number", width: "60px" }
        ]
    });

    var LocationGridDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "LocationID",
                fields: {
                    LocationID: { editable: false },
                    LocationName: { editable: false }
                }
            }
        },
        transport: {
            read: {
                url: "/Common/GetLocationList",
                type: "GET",
                dataType: "JSON"
            }
        },
        pageSize: 10

    });

    var LocationGrid = $("#LocationWindowGrid").kendoGrid({
        dataSource: LocationGridDataSource,
        pageable: true,
        editable: true,
        selectable: "row",
        navigatable: true,
        filterable: true,
        sortable: true,
        height: 300,
        columns: [
              { field: "LocationID", title: "Location ID", width: "60px", filterable: false, sortable: false },
              { field: "LocationName", title: "Location Name", width: "60px" }
        ]

    });

    var SourceGridDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "SourceID",
                fields: {
                    SourceID: { editable: false },
                    SourceName: { editable: false }
                }
            }
        },
        transport: {
            read: {
                url: "/Common/GetSourceList",
                type: "GET",
                dataType: "JSON"
            }
        },
        pageSize: 10

    });

    var SourceGrid = $("#SourceWindowGrid").kendoGrid({
        dataSource: SourceGridDataSource,
        pageable: true,
        editable: true,
        selectable: "row",
        navigatable: true,
        filterable: true,
        sortable: true,
        height: 300,
        columns: [
              { field: "SourceID", title: "Source ID", width: "60px", filterable: false, sortable: false },
              { field: "SourceName", title: "Source Name", width: "60px" }
        ]

    });

    //--------------------------------------End of defining Datasource & Datagrid----------------------------------------------------------------




    //--------------------------------------Start of Defining Datasource & Dropdown-----------------------------------------------------------------
   
    var ddlChallanCategoryDataSource = [{ text: "Real", value: "Real" }, { text: "Fake", value: "Fake" }];

  
    function ddlChallanCategory(container, options) {
        $('<input id="ChallanCategory" data-bind="value:' + options.field + '"/>')
                .appendTo(container)
                .kendoDropDownList({
                    dataTextField: "text",
                    dataValueField: "value",
                    optionLabel: "Select",
                    change: function (e) {
                        changeStatus = 1;
                    },
                    dataSource: ddlChallanCategoryDataSource
                });
    };



    var ddlStoreDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "StoreID",
                fields: {
                    StoreID: { editable: false },
                    StoreName: { editable: false }
                }
            }
        },
        pageSize: 10,
        transport: {
            read: {
                url: "/Common/GetAllStore",
                type: "GET",
                dataType: "JSON"
            }
        }
    });

    function ddlReceiveStore(container, options) {
        $('<input id="ReceiveStore" data-bind="value:' + options.field + '"/>')
                .appendTo(container)
                .kendoDropDownList({
                    dataTextField: "StoreName",
                    dataValueField: "StoreID",
                    optionLabel: "Select",
                    autoBind: false,
                    change: function (e) {
                        changeStatus = 1;
                    },
                    dataSource: ddlStoreDataSource
                });
    };

    
    var ddlSizeDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "SizeID",
                fields: {
                    SizeID: { editable: false },
                    SizeName: { editable: false }
                }
            }
        },
        pageSize: 10,
        transport: {
            read: {
                url: "/Common/GetAllItemSize",
                type: "GET",
                dataType: "JSON"
            }
        }
    });

    function ddlSize(container, options) {
        $('<input name="SizeID" data-bind="value:' + options.field + '"/>')
                .appendTo(container)
                .kendoDropDownList({
                    dataTextField: "SizeName",
                    dataValueField: "SizeID",
                    optionLabel: "Select",
                    autoBind: false,
                    change: function (e) {
                        changeStatus = 1;
                    },
                    dataSource: ddlSizeDataSource
                });
    };


    
    var ddlUnitDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "UnitID",
                fields: {
                    UnitID: { editable: false },
                    UnitName: { editable: false }
                }
            }
        },
        pageSize: 10,
        transport: {
            read: {
                url: "/Common/GetAllUnit",
                type: "GET",
                dataType: "JSON"
            }
        }
    });

    function ddlUnit(container, options) {
        $('<input name="UnitID" data-bind="value:' + options.field + '"/>')
                .appendTo(container)
                .kendoDropDownList({
                    dataTextField: "UnitName",
                    dataValueField: "UnitID",
                    optionLabel: "Select",
                    autoBind: false,
                    change: function (e) {
                        changeStatus = 1;
                    },
                    dataSource: ddlUnitDataSource
                });
    };


  
    var ddlItemTypeDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "ItemTypeID",
                fields: {
                    ItemTypeID: { editable: false },
                    ItemTypeName: { editable: false }
                }
            }
        },
        pageSize: 10,
        transport: {
            read: {
                url: "/Common/GetAllItemType",
                type: "GET",
                dataType: "JSON"
            }
        }
    });

    function ddlItemType(container, options) {
        $('<input name="ItemTypeID" data-bind="value:' + options.field + '"/>')
                .appendTo(container)
                .kendoDropDownList({
                    dataTextField: "ItemTypeName",
                    dataValueField: "ItemTypeID",
                    optionLabel: "Select",
                    autoBind: false,
                    change: function (e) {
                        changeStatus = 1;
                    },
                    dataSource: ddlItemTypeDataSource
                });
    };

    //--------------------------------------Start of Defining Datasource & Dropdown-----------------------------------------------------------------

    //----------------------------------------To open all the pop up windows--------------------------------------------------------------------

    $(document).on('keyup', '.Location', function (e) {
        if (e.which == 120) {
            SelectedItemOfListOfValue('LocationWindowGrid');
            LocationWindow.open(); //Open Popup
        }

    });

    $(document).on('keyup', '.Source', function (e) {
        if (e.which == 120) {
            SelectedItemOfListOfValue('SourceWindowGrid');
            SourceWindow.open(); //Open Popup
        }

    });

    $(document.body).keypress(function (e) {

        if ($("#SupplierID").is(":focus")) {
            if (e.keyCode == 120) {
                SelectedItemOfListOfValue('SupplierWindowGrid');
                SupplierWindow.open();
            }
        }
    });

    $("#btnSearch").click(function () {
        SelectedItemOfListOfValue('SearchWindowGrid');
        SearchWindow.open();
    })

    //----------------------------------------End of opening all the pop up windows--------------------------------------------------------------------

    //---------------------------------------------Start of defining popup Window-----------------------------------------------

    var SearchWindow = $('#SearchWindow').kendoWindow({
        actions: ["Minimize", "Maximize", "Close"],
        visible: false,
        width: "auto",
        height: "auto",
        title: "Purchase Search",
        position: { top: 50, left: 5 },
        modal: true,
        draggable: true
    }).data('kendoWindow');


    var SupplierWindow = $('#SupplierWindow').kendoWindow({
        actions: ["Minimize", "Maximize", "Close"],
        visible: false,
        width: "auto",
        height: "auto",
        title: "List Of Supplier",
        position: { top: 50, left: 5 },
        modal: true,
        draggable: true
    }).data('kendoWindow');

    var SourceWindow = $('#SourceWindow').kendoWindow({
        actions: ["Minimize", "Maximize", "Close"],
        visible: false,
        width: "700px",
        height: "auto",
        title: "List Of Source",
        position: { top: 100, left: 300 },
        modal: true,
        draggable: true
    }).data('kendoWindow');

    var LocationWindow = $('#LocationWindow').kendoWindow({
        actions: ["Minimize", "Maximize", "Close"],
        visible: false,
        width: "700px",
        height: "auto",
        title: "List Of Location",
        position: { top: 100, left: 300 },
        modal: true,
        draggable: true
    }).data('kendoWindow');

    //---------------------------------------------End of defining popup Window-----------------------------------------------


    //-------------------------------------------------Common Function--------------------------------------------------------

    //To select the first row as default & to clear the filter while loading the grid
    function SelectedItemOfListOfValue(GridName) {
        var grid = $('#' + GridName).data("kendoGrid");
        //Reload Grid
        grid.dataSource.read();
        //Clear Filter
        $('#' + GridName).data("kendoGrid").dataSource.filter([]);
        //Select First Row
        $('#' + GridName).data().kendoGrid.bind('dataBound', function (e) {
            this.element.find('tbody tr:first').addClass('k-state-selected');
        });
    }

    //To Bring Data from List of Value Grid to Main Page
    function ListOfValueGridEvent(GridName) {
        var grid = $('#' + GridName).data("kendoGrid");
        var selectedItem = (grid.dataItem(grid.select())); //Selected Row
        changeStatus = 1;
        $.each(selectedItem, function (key, value) {
            if (value != null && value != 0)
                $('#' + key).val(value);
        });
    }

    function ResetOnlyMasterPageData() {
        $(":text").val("");
        $(".hiddenField").val("");
        $("textarea").val("");
        $("#ActiveStatus").val("Active");
        $("#CrudStatus").val(1);
        changeStatus = 0;
        resetEditStatus = 0;
        dataitem = {};
        $(".SetFocus").focus();
        //ClearDetailGridFilterAndPaging();
    }

    //-------------------------------------------------Common Function--------------------------------------------------------



    //-----------------------------------------------Handling all the pop up window events------------------------------------------


    //To bring data from search window to main page
    function SearchWindowEvents() {
        var grid = $("#SearchWindowGrid").data("kendoGrid");
        var selectedItem = (grid.dataItem(grid.select())); //Selected Row

        $.ajax({
            url: '/Purchase/GetDetailPurchaseInformation',
            data: ({ "PurchaseNumber": selectedItem.PurchaseID }),
            type: 'GET',
            contentType: 'application/json;',
            dataType: 'json',
            success: function (response) {
                //ResetData();
                if (response.PurchaseInformation != null) {
                    $.each(response.PurchaseInformation, function (key, value) {
                        if (value != null && value != 0)
                            $('#' + key).val(value);
                    });
                }

                $("#ChallanGrid").data("kendoGrid").dataSource.data(response.ChallanList);
                $("#ItemGrid").data("kendoGrid").dataSource.data(response.ChallanItemList);

            }
        });

    }
    $('#btnSearchWindowOK').click(function () {
        SearchWindowEvents();
        SearchWindow.close();
    });


    // Handling button click for Supplier window grid
    $('#btnSupplierWindowOK').click(function () {
        ListOfValueGridEvent('SupplierWindowGrid');
        SupplierWindow.close();
    });
    $('#btnSupplierWindowCancel').click(function () {
        SupplierWindow.close();
    });
    $("#SupplierWindow").dblclick(function () {
        ListOfValueGridEvent('SupplierWindowGrid');
        SupplierWindow.close();
    });
    $("#SupplierWindow").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            ListOfValueGridEvent('SupplierWindowGrid');
            SupplierWindow.close();
        }
            // Close Window with escape key
        else if (keycode == '27') {
            SupplierWindow.close();
        }
        event.stopPropagation();
    });


    //Handling button click for Location window grid
    function LocationWindowEvent() {
        var WindowGrid = $("#LocationWindowGrid").data("kendoGrid");
        var WindowGridRow = WindowGrid.select();
        var WindowGridItem = WindowGrid.dataItem(WindowGridRow);

        var PageGrid = $("#ChallanGrid").data("kendoGrid");
        var PageGridItem = PageGrid.dataItem(PageGrid.select());

        PageGridItem.set("LocationName", WindowGridItem.LocationName);
        PageGridItem.set("LocationID", WindowGridItem.LocationID);
    };
    $('#btnLocationWindowOK').click(function () {
        LocationWindowEvent();
        LocationWindow.close();
    });
    $('#btnLocationWindowCancel').click(function () {
        LocationWindow.close();
    });
    $("#LocationWindow").dblclick(function () {
        LocationWindowEvent();
        LocationWindow.close();
    });
    $("#LocationWindow").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            LocationWindowEvent();
            LocationWindow.close();
        }
            // Close Window with escape key
        else if (keycode == '27') {
            LocationWindow.close();
        }
        event.stopPropagation();
    });


    //Handling button click for Source window grid
    function SourceWindowEvent() {
        var WindowGrid = $("#SourceWindowGrid").data("kendoGrid");
        var WindowGridRow = WindowGrid.select();
        var WindowGridItem = WindowGrid.dataItem(WindowGridRow);

        var PageGrid = $("#ChallanGrid").data("kendoGrid");
        var PageGridItem = PageGrid.dataItem(PageGrid.select());

        PageGridItem.set("SourceName", WindowGridItem.SourceName);
        PageGridItem.set("SourceID", WindowGridItem.SourceID);
    };
    $('#btnSourceWindowOK').click(function () {
        SourceWindowEvent();
        SourceWindow.close();
    });
    $('#btnSourceWindowCancel').click(function () {
        SourceWindow.close();
    });
    $("#SourceWindow").dblclick(function () {
        SourceWindowEvent();
        SourceWindow.close();
    });
    $("#SourceWindow").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            SourceWindowEvent();
            SourceWindow.close();
        }
            // Close Window with escape key
        else if (keycode == '27') {
            SourceWindow.close();
        }
        event.stopPropagation();
    });

    //-----------------------------------------------End of Handling all the pop up window events------------------------------------------



    // create DatePicker from input HTML element
    $(".datePicker").kendoDatePicker({
        format: "dd/MM/yyyy",
        value: new Date()
    });

    // Display Date into kendo Grid Row
    function dateTimeEditor(container, options) {
        $('<input data-text-field="' + options.field + '" data-value-field="' + options.field + '" data-bind="value:' + options.field + '" data-format="' + options.format + '"/>')
                .appendTo(container)
                .kendoDatePicker({
                    value: new Date()
                });
    }


    //------------------------------------To Reset Data----------------------------------------
    $("#btnReset").click(function () {
        ResetOnlyMasterPageData();
        $("#ItemGrid").data('kendoGrid').dataSource.data([]);
        $("#ChallanGrid").data('kendoGrid').dataSource.data([]);
    })
    //------------------------------------To Reset Data----------------------------------------



    //------------------------------To Add new row to any grid----------------------------------------------------
    $("#ChallanGrid").keyup(function (e) {
        if (e.keyCode == 13) {
            var CheckRowCreation = 1;

            var ChallanGridDataSource = $("#ChallanGrid").data("kendoGrid").dataSource;
            var ChallanGridData = ChallanGridDataSource.data();

            var ItemGridDataSource = $("#ItemGrid").data("kendoGrid").dataSource;
            var ItemGridData = ItemGridDataSource.data();
                        
            for (var i = 0; i < ChallanGridData.length; i++) {
                if (ChallanGridData[i].isNew()) {
                    CheckRowCreation = 0;
                    break;
                }
            }

            if (CheckRowCreation == 1)
            {
                var grid = $("#ChallanGrid").data("kendoGrid");
                grid.addRow();
                $("#ItemGrid").data('kendoGrid').dataSource.data([]);
            }
            else if(CheckRowCreation==0)
            {
                alert("You have unsaved child data.");
            }
        }
    });

    $("#ItemGrid").keyup(function (e) {
        if (e.keyCode == 13) {
            var grid = $("#ItemGrid").data("kendoGrid");
            grid.addRow();
        }
    });
    //------------------------------End of Adding new row to any grid----------------------------------------------------



    //To face child data into another grid for a grid row on click
    $("#ChallanGrid").on("click", "table", function (e) {
        var ParentGrid = $("#ChallanGrid").data("kendoGrid");
        var ParentGridRow = ParentGrid.select();
        var ParentGridItem = ParentGrid.dataItem(ParentGridRow);

        if (ParentGridItem.ChallanID != "") {
            $.ajax({
                url: '/Purchase/GetItemListForChallan',
                type: 'GET',
                data: { "ChallanID": ParentGridItem.ChallanID },
                success: function (response) {

                    if (response != null) {

                        $("#ItemGrid").data("kendoGrid").dataSource.data(response);
                        changeStatus = 0;
                    }

                } //End of Success Call
            });
        }
        else
        {
            $("#ItemGrid").data('kendoGrid').dataSource.data([]);
        }

    });
    
});//End of Document.Ready()


//-----------------------------------To Save Data------------------------------------------------------------------------------------

var ChallanList = { "ChallanNo": "", "ChallanDate": "", "SourceID": "", "LocationID": "", "ChallanCategory": "", "ReceiveStore": "", "ChallanNote": "" };

var ChallanItemList = {
    "ChallanNo": "", "ItemCategory": "", "ItemTypeID": "", "ItemSizeID": "", "UnitID": "",
    "Description": "", "ChallanQty": "", "ReceiveQty": "", "Remark": ""
};
var FinalObject = {
    "SupplierID": "", "SupplierAddressID": "", "PurchaseID": "", "PurchaseCategory": "", "PurchaseYear": "", "PurchaseDate": "", "PurchaseNote": "", "CheckedBy": "",
    "ChallanList": [], "ChallanItemList": []
};

var saveStatus = 0;
function Save() {
    saveStatus = 0;

    if (saveStatus == 0) {
        FinalObject.SupplierID = $("#SupplierID").val();
        FinalObject.SupplierAddressID = $("#SupplierAddressID").val();
        FinalObject.PurchaseID = $("#PurchaseID").val();
        FinalObject.PurchaseCategory = $("#PurchaseCategory").val();
        FinalObject.PurchaseType = $("#PurchaseType").val();
        FinalObject.PurchaseYear = $("#PurchaseYear").val();
        FinalObject.PurchaseDate = $("#PurchaseDate").val();
        FinalObject.PurchaseNote = $("#PurchaseNote").val();


        var ChallanGridDataSource = $("#ChallanGrid").data("kendoGrid").dataSource;
        var ChallanGridData = ChallanGridDataSource.data();

        for (var i = ChallanGridData.length - 1; i >= 0; i--) {
            if (ChallanGridData[i].dirty) {

                ChallanList.ChallanNo = ChallanGridData[i].ChallanNo;
                ChallanList.ChallanDate = ChallanGridData[i].ChallanDate;
                ChallanList.SourceID = ChallanGridData[i].SourceID;
                ChallanList.LocationID = ChallanGridData[i].LocationID;
                ChallanList.ChallanCategory = ChallanGridData[i].ChallanCategory;
                ChallanList.ReceiveStore = ChallanGridData[i].ReceiveStore;
                ChallanList.ChallanNote = ChallanGridData[i].ChallanNote;

                FinalObject.ChallanList.push(ChallanList);
                ChallanList = { "ChallanNo": "", "ChallanDate": "", "SourceID": "", "LocationID": "", "ChallanCategory": "", "ReceiveStore": "", "ChallanNote": "" };
            }
        }

        var ChallanItemGridDataSource = $("#ItemGrid").data("kendoGrid").dataSource;
        var ChallanItemGridData = ChallanItemGridDataSource.data();

        for (var i = ChallanItemGridData.length - 1; i >= 0; i--) {
            if (ChallanItemGridData[i].dirty) {

                ChallanItemList.ChallanNo = ChallanItemGridData[i].ChallanNo;
                ChallanItemList.ItemTypeID = ChallanItemGridData[i].ItemTypeID;
                ChallanItemList.ItemSizeID = ChallanItemGridData[i].SizeID;
                ChallanItemList.UnitID = ChallanItemGridData[i].UnitID;
                ChallanItemList.ItemCategory = ChallanItemGridData[i].ItemCategory;


                ChallanItemList.Description = ChallanItemGridData[i].Description;
                ChallanItemList.ChallanQty = ChallanItemGridData[i].ChallanQty;
                ChallanItemList.ReceiveQty = ChallanItemGridData[i].ReceiveQty;
                ChallanItemList.Remark = ChallanItemGridData[i].Remark;


                FinalObject.ChallanItemList.push(ChallanItemList);
                ChallanItemList = {
                    "ChallanNo": "", "ItemCategory": "", "ItemTypeID": "", "ItemSizeID": "", "UnitID": "", "Description": "",
                    "ChallanQty": "", "ReceiveQty": "", "Remark": ""
                };
            }
        }

        $.ajax({
            url: '/Purchase/Purchase',
            data: JSON.stringify(FinalObject),
            type: 'POST',
            contentType: 'application/json;',
            dataType: 'json',
            success: function (response) {

                if (response.Msg.Type == 2) {
                    $('#MessageText').html(response.Msg.Msg);
                    $("#MessageText").css({ 'color': 'green', 'font-size': 'larger', 'font-weight': 'bold', 'text-align': 'center' });

                    $("#PurchaseID").val(response.purchaseID);

                    $("#ItemGrid").data('kendoGrid').dataSource.data([]);
                    $("#ChallanGrid").data('kendoGrid').dataSource.data([]);
                    
                }
                else if (response.Msg.Type == 3) {
                    $('#MessageText').html(response.Msg.Msg);
                    $("#MessageText").css({ 'color': 'green', 'font-size': 'larger', 'font-weight': 'bold', 'text-align': 'center' });
                    //$("#CrudStatus").val("2");
                    //PageDetailGridReload();
                }
                else {
                    $('#MessageText').html(response.Msg.Msg);
                    $("#MessageText").css({ 'color': 'red', 'font-size': 'larger', 'font-weight': 'bold', 'text-align': 'center' });
                }


            } //End of success call
        }); //End of ajax call
    }

    FinalObject = {
        "SupplierID": "", "SupplierAddressID": "", "PurchaseID": "", "PurchaseCategory": "", "PurchaseYear": "", "PurchaseDate": "", "PurchaseNote": "", "CheckedBy": "",
        "ChallanList": [], "ChallanItemList": []
    };
};

//-----------------------------------End of Saving Data------------------------------------------------------------------------------------