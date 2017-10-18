using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class WBIStoreSelectionVM
    {
        //WetBlueStockSupplier
        public long TransectionID { get; set; }
        public Nullable<byte> StoreID { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<long> PurchaseID { get; set; }
        public Nullable<byte> ItemTypeID { get; set; }
        public Nullable<byte> LeatherTypeID { get; set; }
        public Nullable<byte> LeatherStatusID { get; set; }
        public Nullable<short> GradeID { get; set; }
        public Nullable<decimal> OpeningStockPcs { get; set; }
        public Nullable<decimal> OpeningStockSide { get; set; }
        public Nullable<decimal> OpeningStockArea { get; set; }
        public Nullable<decimal> ReceiveStockPcs { get; set; }
        public Nullable<decimal> ReceiveStockSide { get; set; }
        public Nullable<decimal> ReceiveStockArea { get; set; }
        public Nullable<decimal> IssueStockPcs { get; set; }
        public Nullable<decimal> IssueStockSide { get; set; }
        public Nullable<decimal> IssueStockArea { get; set; }
        public Nullable<decimal> ClosingStockPcs { get; set; }
        public Nullable<decimal> ClosingStockSide { get; set; }
        public Nullable<decimal> ClosingStockArea { get; set; }
        public Nullable<decimal> ClosingStockAreaUnit { get; set; }
        public Nullable<byte> AreaUnit { get; set; }


        public string RecordState { get; set; }


        public virtual IList<wbSelectionIssue> wbSelectionIssue { get; set; }
        public virtual IList<wbSelectionIssueItem> wbSelectionIssueItem { get; set; }
        public virtual IList<wbSelectionIssueGrade> wbSelectionIssueGrade { get; set; }
    }
    public class wbSelectionIssue
    {
        public long WBSIssueItemID { get; set; }
        public string WBSIssueGradeID { get; set; }
        public long IssueID { get; set; }
        public string IssueNo { get; set; }
        public string IssueDate { get; set; }
        public string IssueCategory { get; set; }
        public string IssueType { get; set; }
        public Nullable<byte> IssueFrom { get; set; }
        public string IssueFromName { get; set; }
        public Nullable<byte> IssueTo { get; set; }
        public string IssueToName { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<DateTime> CheckDate { get; set; }
        public string CheckNote { get; set; }
        public Nullable<int> ConfirmedBy { get; set; }
        public Nullable<DateTime> ConfirmDate { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }

        public virtual IList<wbSelectionIssueGrade> wbSelectionIssueGradeList { get; set; }


        public string RecordState { get; set; }


        #region Save Field
        public long WBSelectItemID { get; set; }
        // public string WBSIssueItemID { get; set; }
        public string WBSelectItemNo { get; set; }
        public Nullable<long> WBSelectionID { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<long> PurchaseID { get; set; }
        public Nullable<byte> ItemTypeID { get; set; }
        public Nullable<byte> LeatherTypeID { get; set; }
        public Nullable<byte> LeatherStatusID { get; set; }
        public Nullable<decimal> ProductionPcs { get; set; }
        public Nullable<decimal> ProductionSide { get; set; }
        public Nullable<decimal> ProductionArea { get; set; }
        public Nullable<byte> ProductionAreaUnit { get; set; }
        public string SelectedUnitName { get; set; }
        public string IssueUnitName { get; set; }

        public string SelectionDate { get; set; }
        public int? SelectedBy { get; set; }

        public string SupplierName { get; set; }
        public string PurchaseNo { get; set; }
        public string ItemTypeName { get; set; }
        public string LeatherTypeName { get; set; }
        public string LeatherStatusName { get; set; }
        public string UnitName { get; set; }
        public string WBSelectionNo { get; set; }

        public string SelectedByName { get; set; }
        public string SelectionRemarks { get; set; }



        public string LotNo { get; set; }
        public string Remarks { get; set; }

        #endregion

        #region MyRegion
        public Nullable<short> GradeID { get; set; }
        public string GradeName { get; set; }
        public Nullable<decimal> SelectedGradeQty { get; set; }
        public Nullable<decimal> SelectedGradeSide { get; set; }
        public Nullable<decimal> SelectedGradeArea { get; set; }
        public Nullable<byte> SelectedAreaUnitID { get; set; }
        //public string UnitName { get; set; }
        //public string SelectedUnitName { get; set; }
        //public string IssueUnitName { get; set; }

        public Nullable<decimal> ClosingStockPcs { get; set; }
        public Nullable<decimal> ClosingStockSide { get; set; }
        public Nullable<decimal> ClosingStockArea { get; set; }
        public Nullable<byte> ClosingStockAreaUnit { get; set; }
        public Nullable<byte> UnitID { get; set; }

        #endregion

        #region Newly added field for size Quantity

        public Nullable<decimal> SizeQty1 { get; set; }
        public Nullable<decimal> SizeQty2 { get; set; }
        public Nullable<decimal> SizeQty3 { get; set; }
        public Nullable<decimal> SizeQty4 { get; set; }
        public Nullable<decimal> SizeQty5 { get; set; }
        public Nullable<decimal> SizeQty6 { get; set; }
        public Nullable<decimal> SizeQty7 { get; set; }
        public Nullable<decimal> SizeQty8 { get; set; }
        public Nullable<decimal> SizeQty9 { get; set; }
        public Nullable<decimal> SizeQty10 { get; set; }

        public string SizeQtyRef { get; set; }

        #endregion


    }
    public class wbSelectionIssueItem
    {
        public long WBSelectItemID { get; set; }
        public string WBSelectItemNo { get; set; }
        public Nullable<long> WBSelectionID { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<long> PurchaseID { get; set; }
        public Nullable<byte> ItemTypeID { get; set; }
        public Nullable<byte> LeatherTypeID { get; set; }
        public Nullable<byte> LeatherStatusID { get; set; }
        public Nullable<decimal> ProductionPcs { get; set; }
        public Nullable<decimal> ProductionSide { get; set; }
        public Nullable<decimal> ProductionArea { get; set; }
        public Nullable<byte> ProductionAreaUnit { get; set; }
        public string LotNo { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }

        public string RecordState { get; set; }

        #region ShowField

        public string SupplierName { get; set; }
        public string PurchaseNo { get; set; }
        public string ItemTypeName { get; set; }
        public string LeatherTypeName { get; set; }
        public string LeatherStatusName { get; set; }
        public string UnitName { get; set; }
        public string WBSelectionNo { get; set; }
        //public string SelectionDate { get; set; }
        public int? SelectedBy { get; set; }
        public string SelectedByName { get; set; }
        public string SelectionRemarks { get; set; }

        #endregion
    }
    public class wbSelectionIssueGrade
    {
        public long WBSIssueGradeID { get; set; }
        public string WBSIssueGradeNo { get; set; }
        public Nullable<long> WBSelectionGradeID { get; set; }
        public Nullable<long> WBSIssueItemID { get; set; }
        public string WBSelectItemNo { get; set; }
        public Nullable<short> GradeID { get; set; }
        public Nullable<decimal> GradeQty { get; set; }
        public Nullable<decimal> GradeSide { get; set; }
        public Nullable<decimal> GradeArea { get; set; }
        public Nullable<byte> AreaUnitID { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }

        public string RecordState { get; set; }

        public virtual ICollection<wbSelectionIssueGrade> gradeList { get; set; }
        public virtual IList<wbSelectionIssueGrade> wbSelectionIssueGradeList { get; set; }
        #region MyRegion

        public string GradeName { get; set; }
        public Nullable<decimal> SelectedGradeQty { get; set; }
        public Nullable<decimal> SelectedGradeSide { get; set; }
        public Nullable<decimal> SelectedGradeArea { get; set; }
        public Nullable<byte> SelectedAreaUnitID { get; set; }
        public string UnitName { get; set; }
        public string SelectedUnitName { get; set; }
        public string IssueUnitName { get; set; }

        public Nullable<decimal> ClosingStockPcs { get; set; }
        public Nullable<decimal> ClosingStockSide { get; set; }
        public Nullable<decimal> ClosingStockArea { get; set; }
        public Nullable<byte> ClosingStockAreaUnit { get; set; }

        #endregion

        #region Newly added field for size Quantity

        public Nullable<decimal> SizeQty1 { get; set; }
        public Nullable<decimal> SizeQty2 { get; set; }
        public Nullable<decimal> SizeQty3 { get; set; }
        public Nullable<decimal> SizeQty4 { get; set; }
        public Nullable<decimal> SizeQty5 { get; set; }
        public Nullable<decimal> SizeQty6 { get; set; }
        public Nullable<decimal> SizeQty7 { get; set; }
        public Nullable<decimal> SizeQty8 { get; set; }
        public Nullable<decimal> SizeQty9 { get; set; }
        public Nullable<decimal> SizeQty10 { get; set; }

        public string SizeQtyRef { get; set; }
        public Nullable<long> WBSelectionID { get; set; }


        #endregion

    }

    public class wbSelectionSearch
    {

        public long IssueID { get; set; }
        public long WBSIssueItemID{ get; set; }
        public string WBSelectionNo { get; set; }
        public string IssueNo { get; set; }
        public string IssueDate { get; set; }
        public string SelectionDate { get; set; }
        public int SelectedBy { get; set; }
        public byte? IssueFrom { get; set; }
        public byte? IssueTo { get; set; }
        public string IssueCategory { get; set; }
        public byte? ProductionAreaUnit { get; set; }
        public string RecordStatus { get; set; }
        public string IssueFromName { get; set; }
        public string IssueToName { get; set; }
        public byte? UnitName { get; set; }
   

 
    }


}
