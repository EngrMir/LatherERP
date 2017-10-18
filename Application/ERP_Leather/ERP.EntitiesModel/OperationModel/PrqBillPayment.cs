using System;
using ERP.EntitiesModel.BaseModel;
using System.Collections.Generic;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqBillPayment : CommonStatusInformation
    {
        public long PaymentID { get; set; }
        public string PaymentDate { get; set; }
        public int SupplierID { get; set; }
        public int SupplierAddressID { get; set; }
        public string PurchaseYear { get; set; }
        public byte PaymentType { get; set; }
        public string PaymentTypeName { get; set; }
        public byte PaymentMethod { get; set; }
        public byte Currency { get; set; }
        public string PaymentDoc { get; set; }
        public decimal BillAmount { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? DeductAmount { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string Remarks { get; set; }
        //public string RecordStatus { get; set; }
        public bool PaymentStatus { get; set; }
        public int? CheckedBy { get; set; }
        public DateTime? CheckDate { get; set; }
        public string CheckComment { get; set; }
        public int? RecommendedBy { get; set; }
        public DateTime? RecommendedDate { get; set; }
        public string RecommendedComment { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string ApproveComment { get; set; }

        public virtual IList<PrqBillPaymentReference> BillPaymentReferenceList { get; set; }

        #region Show Data
        public string SupplierName { get; set; }
        public string Address { get; set; }
        public string SupplierCode { get; set; }

        public int? Count { get; set; }
        public IList<PrqBillPayment> BillPaymentList = new List<PrqBillPayment>();
        public string RecordStatusName { get; set; }
        #endregion
    }
}
