using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class ExpBankVoucher
    {
        public long BVID { get; set; }
        public string BVNo { get; set; }
        public string RefBVNo { get; set; }
        public string BVDate { get; set; }
        public string BVType { get; set; }
        public Nullable<int> BankID { get; set; }

        public string BankName { get; set; }
        public Nullable<int> BranchID { get; set; }

        public string BranchName { get; set; }
        public string ACNo { get; set; }
        public string ACName { get; set; }
        public Nullable<long> CIID { get; set; }

        public string CIRefNo { get; set; }

        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public string CheckNote { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public string ApprovalNote { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }


        public virtual ICollection<EXPBankVoucherDtl> EXPBankVoucherDtl { get; set; }
    }

    public class EXPBankVoucherDtl
    {
        public long BVDTLID { get; set; }
        public Nullable<long> BVID { get; set; }
        public Nullable<int> TransSL { get; set; }
        public string Narration { get; set; }
        public Nullable<int> TransHead { get; set; }

        public Nullable<int> HeadID { get; set; }
        public string HeadName { get; set; }

        public Nullable<decimal> BVCRAmt { get; set; }
        public Nullable<decimal> BVDRAmt { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<byte> Currency { get; set; }
        public string CurrencyName { get; set; }
        public Nullable<byte> ExchangeCurrency { get; set; }
        public string ExchangeCurrencyName { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<decimal> ExchangeAmount { get; set; }
        public string Remarks { get; set; }

        public Nullable<long> CIID { get; set; }

        public string CIRefNo { get; set; }
        public string RunningStatus { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public string CheckNote { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public string ApprovalNote { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }
    }
}
