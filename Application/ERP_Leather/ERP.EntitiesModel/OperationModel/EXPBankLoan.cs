
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPBankLoan
    {

        public long BankLoanID { get; set; }
        public string BankLoanNo { get; set; }
        public Nullable<int> BankID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> LoanHead { get; set; }
        public Nullable<decimal> LoanLimit { get; set; }
        public string RefACNo { get; set; }
        public string RefACName { get; set; }
        public Nullable<byte> LoanCurrency { get; set; }
        public string InterestCategory { get; set; }
        public string LoanReceiveDate { get; set; }
        public Nullable<decimal> LoanAmt { get; set; }
        public Nullable<decimal> InterestPercent { get; set; }
        public Nullable<decimal> InterestAmt { get; set; }
        public Nullable<decimal> AmountToReturn { get; set; }
        public Nullable<decimal> InterestReturned { get; set; }
        public Nullable<decimal> PrincipleReturned { get; set; }
        public Nullable<decimal> ReturnedAmt { get; set; }
        public Nullable<decimal> InterestBalance { get; set; }
        public Nullable<decimal> PrincipleBalance { get; set; }
        public Nullable<decimal> BalanceAmt { get; set; }
        public string ClosingDate { get; set; }
        public Nullable<byte> ExchangeCurrency { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<decimal> ExchangeAmount { get; set; }
        public Nullable<long> CIID { get; set; }
        public string Remarks { get; set; }
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


        //************** DISPLAY FIELD *************

        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BanchCode { get; set; }
        public string BranchName { get; set; }
        public Nullable<int> HeadID { get; set; }
        public string HeadCode { get; set; }
        public string HeadName { get; set; }
        public string CINo { get; set; }

        public string ReferenceBankLoanNo { get; set; }
        public string CIRefNo { get; set; }
        public string CIDate { get; set; }



        public virtual IList<EXPBankLoan> expBankLoanList { get; set; }
        //*************** END OF DISPLAY FIELD ******

    }
}
