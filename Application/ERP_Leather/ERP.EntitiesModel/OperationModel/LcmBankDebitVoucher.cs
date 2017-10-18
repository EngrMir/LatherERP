using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class LcmBankDebitVoucher
    {
        public int BDVID { get; set; }
        public string BDVNo { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BDVDate { get; set; }
        public Nullable<byte> BDVCurrency { get; set; }
        public Nullable<int> BankID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> LCID { get; set; }
        public string LCNo { get; set; }
        public Nullable<decimal> LCMargin { get; set; }
        public Nullable<decimal> CommissionAmt { get; set; }
        public Nullable<decimal> PostageAmt { get; set; }
        public Nullable<decimal> SwiftCharge { get; set; }
        public Nullable<decimal> SourceTaxAmt { get; set; }
        public Nullable<decimal> VatAmt { get; set; }
        public Nullable<decimal> StationaryExpense { get; set; }
        public Nullable<decimal> OtherCost { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<byte> ExchangeCurrency { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<decimal> ExchangeAmount { get; set; }
        public string Remarks { get; set; }
        public string RunningStatus { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public string CheckDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public string ApproveDate { get; set; }
        public string ApprovalAdvice { get; set; }
        public string SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }

        public Nullable<decimal> LCAmt { get; set; }
        public Nullable<decimal> VatOnSwift { get; set; }
        public Nullable<decimal> InsuranceCost { get; set; }
        public string AccountNo { get; set; }
        public Nullable<decimal> OpeningStampCharge { get; set; }
    }
}
