$(document).ready(function () {

    var ChallanGridDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "ChallanID",
                fields: {
                    ChallanID: { editable: true },
                    ChallanNo: { editable: true },
                    SourceID: { editable: true },
                    SourceName: { editable: true },
                    LocationID: { editable: true },
                    LocationName: { editable: true },
                    PurchaseNote: { editable: true },
                    ChallanDate: { editable: true }
                }
            }
        },
        pageSize: 10,
        transport: {
            read: {
                url: "/Purchase/GetAllChallan",
                type: "GET"
            }
        },

    });
    var ChallanGrid = $("#ChallanGrid").kendoGrid({
        dataSource: ChallanGridDataSource,
        pageable: true,
        editable: true,
        selectable: "row",
        navigatable: true,
        toolbar: ["create"],
        filterable: true,
        sortable: true,
        detailInit: detailInit,
        dataBound: function () {
            this.expandRow(this.tbody.find("tr.k-master-row").first());
        },
        height: 300,
        columns: [
              { field: "ChallanID", title: "Challan ID", width: "60px" },
              { field: "ChallanNo", title: "S. Challan No", width: "60px" },
              { field: "ChallanDate", title: "S.Challan Date", width: "80px", format: "{0:dd/MM/yyyy}", editor: dateTimeEditor },
              { field: "SourceName", title: "Source", width: "60px", attributes: { "class": "Source" } },
              { field: "LocationName", title: "Location", width: "60px", attributes: { "class": "Location" } },
              { field: "PurchaseNote", title: "Remarks", width: "70px" }

        ]

    });

    function detailInit(e) {
        $('<div id="ItemGrid"></div>').appendTo(e.detailCell).kendoGrid({
            dataSource: {
                type: "JSON",
                transport: {
                    read: {
                        url: "/Purchase/GetAllChallanItem",
                        type: "POST"
                    }
                },

                pageSize: 5,
                filter: { field: "ChallanID", operator: "eq", value: e.data.ChallanID }
            },

            scrollable: false,
            dataBound: function () {
                this.expandRow(this.tbody.find("tr.k-master-row").first());
            },
            //sortable: true,
            pageable: true,
            selectable: 'row',
            editable: true,
            toolbar: ["create", "cancel"],
            editable: "popup",
            batch: true,
            columns: [
                { field: "ChallanNo", title: "Temp", width: "60px" },
                { field: "ItemTypeID", title: "Item Type",  width: "60px" },
                { field: "SizeID", title: "Size", width: "70px", attributes: { "class": "HolidayName" } },
                { field: "Description", title: "Description", width: "70px" },
                { field: "ChallanQty", title: "Challan Qty", width: "60px" },
                { field: "ReceiveQty", title: "Receive Qty", width: "60px", attributes: { "class": "HolidayName" } },
                { field: "Remark", title: "Remarks", width: "70px" },
            ]
        });
    }

    var ItemGridDataSource = new kendo.data.DataSource({
        schema: {
            model: {
                id: "ChallanItemID",
                fields: {
                    ChallanItemID: { editable: true },
                    ChallanID: { editable: true },
                    ItemCategory: { editable: true },
                    ItemTypeID: { editable: true },
                    ItemSizeID: { editable: true },
                    Description: { editable: true },
                    ChallanQty: { editable: true },
                    ReceiveQty: { editable: true },
                    Remark: { editable: true }

                }
            }
        },
        pageSize: 10,
        filter: { field: "ChallanNo", operator: "eq", value: e.data.EmployeeID },
        transport: {
            read: {
                url: "/Purchase/GetAllChallanItem",
                type: "GET"
            }
        },

    });


    
    $("#ChallanGrid").keypress(function (e) {
        if (e.keyCode == 13) {
            var grid = $("#ChallanGrid").data("kendoGrid");
            grid.addRow();

        }
    });

    $("#ItemGrid").keypress(function (e) {
        if (e.keyCode == 13) {

            var ParentGrid = $("#ChallanGrid").data("kendoGrid");
            var ParentGridRow = ParentGrid.select();
            var ParentGridItem = ParentGrid.dataItem(ParentGridRow);

            var ChildGrid = $("#ItemGrid").data("kendoGrid");
            if (ParentGridItem.ChallanID != "")
                ChildGrid.dataSource.add({ ChallanNo: ParentGridItem.ChallanNo });
            else
                alert("Save Challan Data First.")

        }
    });

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


});//End of Document.Ready()


