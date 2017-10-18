using System.Collections.Generic;

namespace ERP.EntitiesModel.OperationModel
{
    public class CommercialInvoiceVM
    {
        public int CIID { get; set; }
        public string CINo { get; set; }
        public string CIDate { get; set; }
        public byte? CICurrency { get; set; }
        public string CIStatus { get; set; }       
        public int? LCID { get; set; }
        public string LCNo { get; set; }
        public string LCDate { get; set; }
        public int? PIID { get; set; }
        public string PINo { get; set; }
        public string OrderNo { get; set; }
        public byte? ExchangeCurrency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? ExchangeValue { get; set; }
        public string CINote { get; set; }
        public string RecordStatus { get; set; }
        public string ApprovalAdvice { get; set; }
        public List<CommercialInvoiceItemVM> Items { get; set; } 
    }

    public class CommercialInvoiceItemVM
    {
        public long CIItemID { get; set; }
        public int CIID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public byte? PackSize { get; set; }
        public string PackSizeName { get; set; }
        public byte? SizeUnit { get; set; }
        public string SizeUnitName { get; set; }
        public int? PackQty { get; set; }
        public decimal CIQty { get; set; }
        public byte CIUnit { get; set; }
        public string CIUnitName { get; set; }
        public decimal CIUnitPrice { get; set; }
        public decimal? CITotalPrice { get; set; }
        public int? SupplierID { get; set; }
        public int? ManufacturerID { get; set; }
        public string SupplierName { get; set; }
        public string ManufacturerName { get; set; }
    }
}
