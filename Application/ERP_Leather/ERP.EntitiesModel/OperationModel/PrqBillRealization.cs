using System;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqBillRealization : CommonStatusInformation
    {
        public int SupplierID { get; set; }
        public string PurchaseYear { get; set; }
        public decimal? OpenigBalance { get; set; }
        public string OpeningBalStatus { get; set; }
        public decimal? TotalBillAmt { get; set; }
        public decimal? FakeApproveBillAmt { get; set; }
        public decimal? ApproveBillAmt { get; set; }
        public decimal? PaidAmt { get; set; }
        public decimal? DueAmt { get; set; }
        public decimal? CompensateAmt { get; set; }
        public decimal? ClosingBalance { get; set; }
        public string ClosingBalStatus { get; set; }
        public string Remarks { get; set; }
        public bool PaymentStatus { get; set; }
        public int? CheckedBy { get; set; }
        public string CheckDate { get; set; }
        public string CheckComment { get; set; }
        public int? ApprovedBy { get; set; }
        public string ApproveDate { get; set; }
        public string ApproveComment { get; set; }
    }
}
