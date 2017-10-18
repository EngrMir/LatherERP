using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class LCMLimInfo
    {
        public int LimID { get; set; }
        public string LimNo { get; set; }
        public string LimDate { get; set; }
        public Nullable<decimal> LimLimit { get; set; }
        public Nullable<decimal> LimBalance { get; set; }
        public Nullable<int> LimBankID { get; set; }
        public Nullable<int> LimBranchID { get; set; }
        public Nullable<int> LCID { get; set; }
        public Nullable<decimal> TotalCalcAmtToPaid { get; set; }
        public string LCNo { get; set; }
        public Nullable<decimal> LoanAmount { get; set; }
        public Nullable<decimal> InterestPersent { get; set; }
        public Nullable<decimal> InterestAmount { get; set; }
        public Nullable<decimal> AmountToBePaid { get; set; }
        public Nullable<int> AdjustmentTime { get; set; }
        public Nullable<decimal> OtherCharges { get; set; }
        public Nullable<decimal> AcceptanceCommission { get; set; }
        public Nullable<decimal> HandlingCharge { get; set; }
        public Nullable<decimal> TotalAmountToBePaid { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<byte> LimCurrency { get; set; }
        public Nullable<byte> ExchangeCurrency { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<decimal> ExchangeValue { get; set; }
        public string RecordStatus { get; set; }
        public string Remarks { get; set; }








        public Nullable<decimal> LCMarginTransfer { get; set; }
        public Nullable<decimal> LimMarginTrans { get; set; }
        public Nullable<decimal> LimMarginTransPercnt { get; set; }
        public string InterestCalcDate { get; set; }
        public Nullable<decimal> CalcInterestAmt { get; set; }
        public Nullable<decimal> ExciseDuty { get; set; }
        public Nullable<decimal> TransCash { get; set; }
        public Nullable<decimal> CalcAmtToPaid { get; set; }

        public Nullable<decimal> LimAdjustDr { get; set; }
        public Nullable<decimal> LimAdjustCr { get; set; }





        public string CheckedBy { get; set; }
        public string CheckDate { get; set; }
        public string SetOn { get; set; }
        public string SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string IPAddress { get; set; }
        public string BankCategory { get; set; }
        public string BankType { get; set; }
        public int CurrencyID { get; set; }
        public string CurrencyName { get; set; }


        public int BankID { get; set; }
        public string BankCode { get; set; }

        public string BanchCode { get; set; }
        public string BankName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public Nullable<decimal> LCLimit { get; set; }
        //public Nullable<decimal> LCMargin { get; set; }
        //public Nullable<decimal> LIMLimit { get; set; }
        //public Nullable<decimal> LIMTaken { get; set; }
        //public Nullable<decimal> LIMBalance { get; set; }
        public virtual IList<LCMLimInfo> lcmLimInfoList { get; set; }

    }


    public class sysBranch
    {

        public int BankID { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public Nullable<decimal> LCLimit { get; set; }
        public Nullable<decimal> LCMargin { get; set; }
        public Nullable<decimal> LIMLimit { get; set; }
        public Nullable<decimal> LIMTaken { get; set; }
        public Nullable<decimal> LIMBalance { get; set; }



    }








}



