using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPLCOpening
    {
        public long LCID { get; set; }
        public string LCNo { get; set; }
        public string RefLCNo { get; set; }
        public string LCDate { get; set; }
        public decimal LCAmount { get; set; }
        public byte LCCurrencyID { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<byte> ExchangeCurrency { get; set; }
        public Nullable<decimal> ExchangeValue { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
        public string ExpiryPlace { get; set; }
        public Nullable<int> NNDSendingTime { get; set; }
        public string LCANo { get; set; }
        public string IRCNo { get; set; }
        public string VatRegNo { get; set; }
        public string TINNo { get; set; }
        public string BINNo { get; set; }
        public string ICNNo { get; set; }
        public string ICNDate { get; set; }
        public string LastShipmentDate { get; set; }
        public Nullable<int> LCOpeningBank { get; set; }
        public Nullable<int> AdvisingBank { get; set; }
        public Nullable<int> Beneficiary { get; set; }
        public Nullable<int> BeneficiaryAddressID { get; set; }
        public Nullable<long> PIID { get; set; }
        public string LCReviceNo { get; set; }
        public Nullable<DateTime> LCReviceDate { get; set; }
        public string RunningStatus { get; set; }
        public string LCState { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<DateTime> CheckDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<DateTime> ApproveDate { get; set; }
        public string ApprovalAdvice { get; set; }
        public Nullable<DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }



        public string Remarks { get; set; }

        #region Display Field/Necessary Field

        public int BankID { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }

        public string LcOpeningBankName { get; set; }

        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string BanchCode { get; set; }
        public string PIDate { get; set; }
        public string AdvisingBankName { get; set; }
        public string LCCurrencyName { get; set; }


        public string BranchSwiftCode { get; set; }
        public string BeneficiaryName { get; set; }
        public string SupplierName { get; set; }
        public Nullable<int> PIBeneficiary { get; set; }

        public int BuyerID { get; set; }
        public string BuyerName { get; set; }


        public int BuyerAddressID { get; set; }
        public string Address { get; set; }

        public string Address1 { get; set; }

        public string PINo { get; set; }


        public Nullable<byte> CurrencyID { get; set; }

        //public Nullable<byte> PICurrency { get; set; }
        //public string CurrencyName { get; set; }

        public decimal LCLimit { get; set; }
        public decimal LCMargin { get; set; }
        public decimal LCBalance { get; set; }
        #endregion


        #region PI SHOW DETAILS

        public Nullable<byte> PICurrency { get; set; }
        public string PICurrencyName { get; set; }
        public string LocalAgentName { get; set; }
        public string ForeignAgentName { get; set; }
        public string PaymentName { get; set; }

        public string PaymentTerm { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentNote { get; set; }
        public string DeferredDays { get; set; }

        public string BeneficiaryBankName { get; set; }
        public Nullable<int> LocalAgent { get; set; }
        public Nullable<int> ForeignAgent { get; set; }
        public Nullable<int> OfferValidityDays { get; set; }

        public Nullable<int> BuyerBank { get; set; }
        public string BuyerBankAccount { get; set; }
        public string BuyerBankName { get; set; }


        public Nullable<int> BeneficiaryBank { get; set; }
        public string BankAccount { get; set; }
        public string SellerBankName { get; set; }


        public Nullable<int> PortofLoading { get; set; }
        public Nullable<int> PortofDischarge { get; set; }

        public string PICategory { get; set; }
        public string LoadingPortName { get; set; }
        public string DischargePortName { get; set; }

        #endregion




    }
}
