using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqChemLocalPurcBillPay
    {
        public long PaymentId { get; set; }
        public string PaymentNo { get; set; }
        public string PaymentDate { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierNo { get; set; }
        public string SupplierName { get; set; }
        public int? SupplierAddressId { get; set; }
        public string SupplierAddress { get; set; }
        public string PurchaseYear { get; set; }
        public string PaymentType { get; set; }
        public string PaymentMethod { get; set; }
        public byte? Currency { get; set; }
        public string PaymentDoc { get; set; }
        public decimal? BillAmount { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? DeductAmount { get; set; }
        public decimal? PaymentAmount { get; set; }
        public bool? PaymentStatus { get; set; }
        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public int? CheckedBy { get; set; }
        public string CheckedByName { get; set; }
        public string CheckComments { get; set; }
        public int? ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public string ApprovalAdvice { get; set; }
        public List<PrqChemBillPymtRef> References { get; set; }        
    }

    public class PrqChemBillPymtRef
    {
        public long BillPaymtRefID { get; set; }
        public long? PaymentID { get; set; }
        public long? BillID { get; set; }
        public string BillNo { get; set; } 
        public string SupplierBillNo { get; set; }
        public string RecordStatus { get; set; }
    }

}
