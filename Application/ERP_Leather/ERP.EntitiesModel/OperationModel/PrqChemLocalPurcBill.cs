using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqChemLocalPurcBill
    {
        public long BillId { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int? SupplierAddressId { get; set; }
        public string SupplierAddress { get; set; }
        public string PurchaseYear { get; set; }
        public string SupplierBillNo { get; set; }
        public string SupBillDate { get; set; }
        public string BillCategory { get; set; }
        public string BillType { get; set; }
        public decimal? BillAmt { get; set; }
        public string BillStatus { get; set; }
        public byte? Currency { get; set; }
        public decimal? VatAmt { get; set; }
        public decimal? ApprovedAmt { get; set; }
        public decimal? OtherCost { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmt { get; set; }
        public decimal? PayableAmt { get; set; }
        public byte? ExchangCurrency { get; set; }
        public decimal? ExchangRate { get; set; }
        public decimal? ExchangValue { get; set; }
        public string Remarks { get; set; }
        public string CheckComments { get; set; }
        public string ApprovalComments { get; set; }
        public string RecordStatus { get; set; }
        //public List<PrqChemLocalPurcBillItem> Items { get; set; }
        public List<PrqChemLocalPurcBillRef> References { get; set; } 
    }

    public class PrqChemLocalPurcBillRef
    {
        public long BillRefId { get; set; }
        public long? BillId { get; set; }
        public long? ReceiveID { get; set; }
        public string ReceiveNo { get; set; }
        public string ReceiveDate { get; set; }
        public int? OrderID { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public string Remark { get; set; }
        public List<PrqChemLocalPurcBillItem> Items { get; set; } 
    }

    public class PrqChemLocalPurcBillItem
    {
        public long BillItemId { get; set; }
        public long? BillRefId { get; set; }
        public int? ItemId { get; set; }
        public string ItemName { get; set; }
        public int? SupplierId { get; set; }
        public byte? PackSize { get; set; }
        public string PackSizeName { get; set; }
        public byte? SizeUnit { get; set; }
        public string SizeUnitName { get; set; }
        public decimal? PackQty { get; set; }
        public decimal? ReceiveQty { get; set; }
        public byte? UnitId { get; set; }
        public string UnitName { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public string Remark { get; set; }
        public long? ReceiveID { get; set; }
        public int? OrderID { get; set; }
    }
}
