using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvWetBlueIssueItem
    {
        public long WBSIssueItemID { get; set; }
        public long? WetBlueIssueRefID { get; set; }
        public long? WetBlueIssueID { get; set; }
        public int? SupplierID { get; set; }
        public string SupplierName { get; set; }
        public byte? StoreID { get; set; }
        public string StoreName { get; set; }
        public long? PurchaseID { get; set; }
        public string PurchaseNo { get; set; }
        public string LotNo { get; set; }
        public byte? ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public byte? LeatherTypeID { get; set; }
        public byte? LeatherStatusID { get; set; }
        public string LeatherStatusName { get; set; }
        public decimal? IssuePcs { get; set; }
        public decimal? IssueSide { get; set; }
        public decimal? IssueArea { get; set; }
        public byte? AreaUnit { get; set; }
        public DateTime? SetOn { get; set; }
        public int? SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string IPAddress { get; set; }
        public Int16? GradeID { get; set; }
        public string GradeName { get; set; }
        public byte? SizeID { get; set; }
        public string SizeName { get; set; }
        public string SizeQtyRef { get; set; }
        
        public decimal? ClosingStockPcs { get; set; }
        public decimal? PcsToSide { get; set; }
        public decimal? ClosingStockSide { get; set; }
        public decimal? ClosingStockArea { get; set; }
        public decimal? AverageStockArea { get; set; }
        



    }
}
