using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    class LCMLCRetirement
    { 


        public int LCRetirementID { get; set; }
        public string LCRetirementNo { get; set; }
        public DateTime? LCRetirementDate { get; set; }
        public Nullable<int> LCID { get; set; }
        public string LCNo { get; set; }
        public Nullable<int> LCMargin { get; set; }
        public Nullable<decimal> BillValue { get; set; }
        public Nullable<decimal> InterestPersent { get; set; }
        public Nullable<decimal> InterestAmount { get; set; }
        public Nullable<decimal> BankCommissionAmt { get; set; }

        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<byte> LCRCurrency { get; set; }
        public Nullable<byte> ExchangeCurrency { get; set; }
        public Nullable<byte> ExchangeRate { get; set; }
        public Nullable<decimal> ExchangeValue { get; set; }
       
        public string SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public int ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }





    }
}
