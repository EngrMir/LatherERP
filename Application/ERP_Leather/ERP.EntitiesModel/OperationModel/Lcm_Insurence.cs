using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class Lcm_Insurence
    {
        public int InsuranceID { get; set; }
        public string InsuranceNo { get; set; }
        public int InsuranceCompany { get; set; }
        public string InsuranceCompanyName { get; set; }
        public Nullable<int> LCID { get; set; }
        public string LCNo { get; set; }
        public string LCNumber { get; set; }
        public string CoverNoteDate { get; set; }
        public string MoneyReceiptNo { get; set; }
        public string MoneyReceiptDate { get; set; }
        public string ClauseEndorsement { get; set; }
        public string InsuranceChallanNo { get; set; }
        public string InsurancChallanDate { get; set; }
        public string PolicyNo { get; set; }
        public string PolicyDate { get; set; }
        public Nullable<decimal> InvoiceAmount { get; set; }
        public Nullable<byte> InvouceCurrency { get; set; }
        public Nullable<decimal> IncreasePercent { get; set; }
        public Nullable<decimal> IncreaseAmount { get; set; }
        public Nullable<decimal> SumInsured { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<byte> ExchangeRateCurrency { get; set; }
        public Nullable<decimal> ExchangedSumInsured { get; set; }
        public Nullable<decimal> MarinePremiumPercent { get; set; }
        public Nullable<decimal> MarinePremiumAmount { get; set; }
        public Nullable<decimal> DiscountPercent { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> WarSRCCPercent { get; set; }
        public Nullable<decimal> WarSRCCPercentAmount { get; set; }
        public Nullable<decimal> NetPremium { get; set; }
        public Nullable<decimal> VatPercent { get; set; }
        public Nullable<decimal> VatAmount { get; set; }
        public Nullable<decimal> StampDutyRetio { get; set; }
        public Nullable<decimal> StampDutyAmount { get; set; }
        public Nullable<decimal> GrandPremium { get; set; }
        public Nullable<decimal> RefundPercent { get; set; }
        public Nullable<decimal> RefundAmount { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public string RevisionNo { get; set; }
        public string RevisionDate { get; set; }
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
        public int CIID { get; set; }
        public string CINo { get; set; }
        public string LCDate { get; set; }
        public string CIDate { get; set; }
        public string PIDate { get; set; }
        public string PIID { get; set; }
        public string PINo { get; set; }
        public string SupplierName { get; set; }
    }
}
