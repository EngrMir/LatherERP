using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqSupplierBill : CommonStatusInformation
    {
        public long SupplierBillID { get; set; }
        public int SupplierID { get; set; }
        
        public long? PurchaseID { get; set; }
        public int SupplierAddressID { get; set; }
        public string PurchaseYear { get; set; }
        public string SupplierBillNo { get; set; }
        public string SupplierBillRefNo { get; set; }
        public string BillDate { get; set; }
        public string BillCategory { get; set; }
        public string BillType { get; set; }
        public decimal? TotalQty { get; set; }
        public decimal? AvgPrice { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? TotalRejectQty { get; set; }
        public decimal? ApprovedPrice { get; set; }
        public decimal? ApprovedAmt { get; set; }
        public decimal? OtherCost { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmt { get; set; }
        public decimal? PayableAmt { get; set; }
        public string Remarks { get; set; }
        public int? CheckedBy { get; set; }
        public DateTime? CheckDate { get; set; }
        public string CheckComment { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string ApproveComment { get; set; }
        
        //public bool IsDelete { get; set; }
        public List<PrqSupplierBillChallan> SupplierBillChallanList { get; set; }
        public List<PrqSupplierBillItem> SupplierBillItemList { get; set; }

        #region Data for Display

        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierCode { get; set; }
        public string PurchaseNo { get; set; }
        #endregion  
    }

    public  class SupplierBill
    {
        public long SupplierBillID { get; set; }
        public string SupplierBillNo { get; set; }
        public string SupplierBillRefNo { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string BillDate { get; set; }
        public string PurchaseYear { get; set; }
        public string RecordStatus { get; set; }
        public string PurchaseNo { get; set; }
        public decimal? PurchaseQty { get; set; }
        public string SourceName { get; set; }
        public string ItemName { get; set; }
        public decimal? TotalQty { get; set; }
        public decimal? AvgPrice { get; set; }
        public decimal? PayableAmt { get; set; }
        public decimal? TotalAmt { get; set; }

    }
}
