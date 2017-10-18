using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPLCRetirement
    {
        public long LCRetirementID { get; set; }
        public string LCRetirementNo { get; set; }
        public string LCRetirementDate { get; set; }
        public Nullable<long> LCID { get; set; }
        public string LCNo { get; set; }
        public Nullable<decimal> BillValue { get; set; }
        public Nullable<decimal> InterestPersent { get; set; }
        public Nullable<decimal> InterestAmount { get; set; }
        public Nullable<decimal> BankCommissionAmt { get; set; }
        public Nullable<decimal> SwiftCharge { get; set; }
        public Nullable<decimal> OtherCharge { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<byte> LCRCurrency { get; set; }
        public Nullable<byte> ExchangeCurrency { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<decimal> ExchangeValue { get; set; }



        public Nullable<decimal> ExchangeValueInvoice { get; set; }
        public Nullable<decimal> ExchangeRateInvoice { get; set; }

        //public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> LCAmount { get; set; }
        public Nullable<DateTime> LCDate { get; set; }
        public Nullable<DateTime> CIDate { get; set; }
        public Nullable<decimal> CITotalValue { get; set; }
        public string CINo { get; set; }
        public long CIID { get; set; }
        public Nullable<decimal> LCMargin { get; set; }



        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }



    }
}
