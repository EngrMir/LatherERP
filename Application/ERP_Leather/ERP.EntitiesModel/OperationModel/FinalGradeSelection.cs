using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class wbSelection
    {
        public byte StoreID { get; set; }
        public long WBSelectionID { get; set; }
        public string WBSelectionNo { get; set; }
        public string SelectionDate { get; set; }
        public Nullable<int> SelectedBy { get; set; }
        public string SelectionComments { get; set; }
        public Nullable<int> ConfirmedBy { get; set; }
        public Nullable<System.DateTime> ConfirmedDate { get; set; }
        public string ConfirmedNote { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public string CheckNote { get; set; }
        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public string RecordState { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }
        public decimal ProductionQty { get; set; }
        public string AverageThickness { get; set; }
        public string LessCut { get; set; }
        public string GrainOff { get; set; }
        public DateTime ProductionDate { get; set; }

        public virtual ICollection<wbSelectionItem> wbSelectionItem { get; set; }


    }

    public class wbSelectionItem
    {
        public long WBSelectItemID { get; set; }
        public string WBSelectItemNo { get; set; }
        public Nullable<long> WBSelectionID { get; set; }
        public int SupplierID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public Nullable<long> PurchaseID { get; set; }
        public long TransectionID { get; set; }

        public string ItemTypeID { get; set; }
        public Nullable<byte> LeatherTypeID { get; set; }
        public Nullable<byte> LeatherStatusID { get; set; }
        public Nullable<decimal> ClosingProductionkPcs { get; set; }
        public Nullable<decimal> ClosingProductionSide { get; set; }
        public Nullable<decimal> ClosingProductionArea { get; set; }
        public string UnitID { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }
        public byte? StoreID { get; set; }
        public string StoreName { get; set; }

        #region Badhon
        //public long WBSelectionID { get; set; }
        public string WBSelectionNo { get; set; }
        public string SelectionDate { get; set; }
        public Nullable<int> SelectedBy { get; set; }
        public string SelectionComments { get; set; }
        public Nullable<int> ConfirmedBy { get; set; }
        public Nullable<System.DateTime> ConfirmedDate { get; set; }
        public string ConfirmedNote { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public string CheckNote { get; set; }
        #endregion



        public wbSelectionItem FormInfo { get; set; }
        public virtual ICollection<wbSelectionGrade> wbSelectionGrade { get; set; }

    }

    public class wbSelectionGrade
    {
        public long WBSelectionGradeID { get; set; }
        public Nullable<long> WBSelectItemID { get; set; }
        public string WBSelectItemNo { get; set; }
        public Nullable<long> WBSelectionID { get; set; }
        public Nullable<short> GradeID { get; set; }
        public Nullable<decimal> GradeQty { get; set; }
        public Nullable<decimal> GradeSide { get; set; }
        public Nullable<decimal> GradeArea { get; set; }
        public string AreaUnitID { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }
        public Nullable<byte> SizeID { get; set; }
        public decimal? SizeQty1 { get; set; }
        public decimal? SizeQty2 { get; set; }
        public decimal? SizeQty3 { get; set; }
        public decimal? SizeQty4 { get; set; }
        public decimal? SizeQty5 { get; set; }
        public decimal? SizeQty6 { get; set; }
        public decimal? SizeQty7 { get; set; }
        public decimal? SizeQty8 { get; set; }
        public decimal? SizeQty9 { get; set; }
        public decimal? SizeQty10 { get; set; }


    }

    public class wbSelectionFormData
    {
        public string ItemTypeName { get; set; }
        public int ItemTypeID { get; set; }
        public int SelectionDue { get; set; }
        public int SelectionComplete { get; set; }
        public string StoreName { get; set; }
        public int StoreID { get; set; }
        public int ProductionQty { get; set; }
        public int PurchaseQty { get; set; }
        public DateTime PurchaseDate { get; set; }
        public long PurchaseID { get; set; }
        public string SelectionComments { get; set; }
        public string Address { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public int SupplierID { get; set; }
        public string SelectionDate { get; set; }
        public string WBSelectionNo { get; set; }
        public long WBSelectionID { get; set; }
        public int LeatherStatusID { get; set; }
        public string LeatherStatusName { get; set; }
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public decimal ClosingProductionkPcs { get; set; }
        public decimal ClosingProductionSide { get; set; }
        public decimal ClosingProductionArea { get; set; }
        public string SelectedBy { get; set; }
        public int UserID { get; set; }

        public int ProductionDue { get; set; }
    }

    public class UspWBSelectionData
    {
        public byte StoreID { get; set; }
        public int SupplierID { get; set; }
        public long PurchaseID { get; set; }
        public byte ItemTypeID { get; set; }
        public byte LeatherStatusID { get; set; }
        public decimal PurchaseQty { get; set; }
        public string PurchaseNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal ProductionQty { get; set; }
        public decimal ProductionDue { get; set; }
        public string StoreName { get; set; }
        public string ItemTypeName { get; set; }
        public string LeatherStatusName { get; set; }
        public byte UnitID { get; set; }
        public string UnitName { get; set; }
        public decimal ClosingProductionkPcs { get; set; }
        public decimal ClosingProductionArea { get; set; }
        public decimal ClosingProductionSide { get; set; }

        public int SelectionComplete { get; set; }
        public int SelectionDue { get; set; }

        public int TotalProductionQty { get; set; }



    }

    public class wbshowField
    {
        public long? PurchaseID { get; set; }
        public byte? ItemTypeID { get; set; }
        public byte? LeatherTypeID { get; set; }
        public byte? LeatherStatusID { get; set; }

        #region Show Field

        public string PurchaseNo { get; set; }
        public string PurchaseDate { get; set; }
        public string ItemTypeName { get; set; }
        public string LeatherTypeName { get; set; }
        public string LeatherStatusName { get; set; }
        public decimal? ProductionQty { get; set; }
        public decimal? SelectionQty { get; set; }
        public byte AreaUnit { get; set; }
        public decimal? SelectionDueQty { get; set; }
        public decimal? ProductionSideQty { get; set; }

        public decimal? SelectionSideQty { get; set; }
        public decimal? SelectionSideDueQty { get; set; }
        public decimal? SelectionCompSide { get; set; }

        public string UnitName { get; set; }
        public decimal? PurchaseQty { get; set; }
        public decimal? ProductionDue { get; set; }
        #endregion
    }

    public class GradeSelectionSearchPopup
    {
        public long? WBSelectionID { get; set; }
        public byte? StoreID { get; set; }
        public string WBSelectionNo { get; set; }
        public DateTime? SelectionDate { get; set; }
        public int? SupplierID { get; set; }
        public int? UserID { get; set; }
        public byte? UnitID { get; set; }
        public string SelectionComments { get; set; }
        public string RecordStatus { get; set; }
        public long? PurchaseID { get; set; }
        public string PurchaseNo { get; set; }
        public byte? ItemTypeID { get; set; }
        public byte? LeatherStatusID { get; set; }
        public string StoreName { get; set; }
        public string SupplierName { get; set; }
        public string SelectedBy { get; set; }
        public string ItemTypeName { get; set; }
        public string LeatherStatusName { get; set; }
        public decimal? ClosingProductionkPcs { get; set; }
        public decimal? ClosingProductionArea { get; set; }
        public decimal? ClosingProductionSide { get; set; }
        public string SupplierCode { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public long? WBSelectionGradeID { get; set; }
        public decimal PurchaseQty { get; set; }
        public decimal ProductionQty { get; set; }
        public decimal ProductionDue { get; set; }
        public decimal SelectionComplete { get; set; }
        public decimal SelectionDue { get; set; }
        public decimal SelectionSideDue { get; set; }
        public virtual List<GradeList> WbGradeList { get; set; }
        public string AverageThickness { get; set; }
        public string LessCut { get; set; }
        public string GrainOff { get; set; }
        public DateTime ProductionDate { get; set; }
         public decimal? SizeQty1 { get; set; }
        public decimal? SizeQty2 { get; set; }
        public decimal? SizeQty3 { get; set; }
        public decimal? SizeQty4 { get; set; }
        public decimal? SizeQty5 { get; set; }
        public decimal? SizeQty6 { get; set; }
        public decimal? SizeQty7 { get; set; }
        public decimal? SizeQty8 { get; set; }
        public decimal? SizeQty9 { get; set; }
        public decimal? SizeQty10 { get; set; }

    }
    public class GradeList
    {
        public long? WBSelectionGradeID { get; set; }
        public short? GradeID { get; set; }
        public string GradeName { get; set; }
        public decimal? GradeQuantity { get; set; }
        public decimal? SizeQty1 { get; set; }
        public decimal? SizeQty2{ get; set; }
        public decimal? SizeQty3 { get; set; }
        public decimal? SizeQty4 { get; set; }
        public decimal? SizeQty5{ get; set; }
        public decimal? SizeQty6 { get; set; }
        public decimal? SizeQty7 { get; set; }
        public decimal? SizeQty8 { get; set; }
        public decimal? SizeQty9 { get; set; }
        public decimal? SizeQty10 { get; set; }
        public decimal? GradeSide { get; set; }
        public decimal? GradeArea { get; set; }
        public byte? UnitID { get; set; }
        public byte? SizeID { get; set; }
        public string SizeName { get; set; }
    }
}
