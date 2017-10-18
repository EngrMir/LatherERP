using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class LCMLCRetirement
    {


        public int LCRetirementID { get; set; }
        public string LCRetirementNo { get; set; }
        public string LCRetirementDate { get; set; }
        public Nullable<int> LCID { get; set; }
        public string LCNo { get; set; }

        public Nullable<decimal> BillValue { get; set; }
        public Nullable<decimal> InterestPersent { get; set; }
        public Nullable<decimal> InterestAmount { get; set; }
        public Nullable<decimal> BankCommissionAmt { get; set; }
        public Nullable<decimal> SwiftCharge { get; set; }
        public Nullable<decimal> OtherCharge { get; set; }
        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<decimal> LCMargin { get; set; }
        public int CIID { get; set; }


        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> LCAmount { get; set; }
        public string LCDate { get; set; }
        public DateTime? CIDate { get; set; }
 
        public Nullable<decimal> CITotalValue { get; set; }
        public string CINo { get; set; }

        public Nullable<decimal> LessMargin { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }

        public int SupplierID { get; set; }
        public string SupplierName { get; set; } 

        public Nullable<byte> LCRCurrency { get; set; }
        public Nullable<byte> ExchangeCurrency { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<decimal> ExchangeValue { get; set; }

        public Nullable<decimal> ExchangeValueInvoice { get; set; }

        public Nullable<decimal> ExchangeRateInvoice { get; set; }

        public string SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public int ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }



        public int ReturnId { get; set; }
        public string ReturnCode { get; set; }
        public string CurrencyName { get; set; }


    }
}
