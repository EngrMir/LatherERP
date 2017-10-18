using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class LcmLcOpening
    {
        public int LCID { get; set; }
        public string LCNo { get; set; }
        public  string LCDate { get; set; }
        public decimal LCAmount { get; set; }
        public int LCAmountCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ExchangeValue { get; set; }
        public  string IssueDate { get; set; }
        public  string ExpiryDate { get; set; }
        public string ExpiryPlace { get; set; }

        public int NNDSendingTime { get; set; }
        public string LCANo { get; set; }
        public string IRCNo { get; set; }
        public string VatRegNo { get; set; }
        public string TINNo { get; set; }
        public string BINNo { get; set; }
        public string ICNNo { get; set; }
        public  string ICNDate { get; set; }
        public  string LastShipmentDate { get; set; }
        public int BankID { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public int BranchID { get; set; }
        
        public int AdvisingBank { get; set; }
        public int BeneficiaryAddressID { get; set; }
        public string BeneficiaryAddress{ get; set; }
        public int PIID { get; set; }
        public int ExchangeCurrency { get; set; }
        public string PINo { get; set; }
        public string LCReviceNo { get; set; }
        public string RunningStatus { get; set; }
        public string LCState { get; set; }
        public string LCStatus { get; set; }
        public string RecordStatus { get; set; }
        public string ApprovalAdvice { get; set; }
        public string BankAddress { get; set; }
        public string BankSwiftCode { get; set; }
        public decimal LCLimitAmount { get; set; }
        public int LCLimitCurrencyID { get; set; }
        public decimal LCMarginBalance { get; set; }
        public string Beneficiary { get; set; }
        public decimal LCLimitRange { get; set; }
        public  string PIDate { get; set; }
        public string CurrencyName { get; set; }
        public String AdvisingBankName { get; set; }
        public String BeneficiaryName { get; set; }
        public decimal LCMargin { get; set; }
        public int CIID { get; set; }
        public string CINo { get; set; }
        //#################################
        public int? InsuranceID { get; set; }
        public string InsuranceNo { get; set; }
        public byte? PICurrency { get; set; }
        public string PaymentTerm { get; set; }
        public string PaymentMode { get; set; }
        public string DeferredDays { get; set; }
        public int? CountryOforigin { get; set; }
        public int? BeneficiaryBank { get; set; }
        public string DeliveryTerm { get; set; }
        public int? PortofDischarge { get; set; }
        public string DeliveryMode { get; set; }
        public string Transshipment { get; set; }
        public string PartialShipment { get; set; }
        public string GoodDelivery { get; set; }
        public int? PortofLoading { get; set; }
        public string ShippingMark { get; set; }
        public string ExpectedShipmentTime { get; set; }
        public string Packing { get; set; }
        public string PreShipmentIns { get; set; }
        public int? OfferValidityDays { get; set; }

        public int PresentationDays { get; set; }
        public string ConfirmStatus { get; set; }
        public string OpeningBINNo { get; set; }
        public string AdvisingBINNo { get; set; }

    }

    public class LCM_LCOpeningBank
    {
        public int BankID { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public decimal LCLimit { get; set; }
        public decimal LCMargin { get; set; }
        public string CurrencyName { get; set; }
        public string BankBINNo { get; set; }

    }

    public class ChemicalPI
    {
        public int PIID { get; set; }
        public int BranchID { get; set; }
        public String PINo { get; set; }
        public String PIDate { get; set; }
        public String BankSwiftCode { get; set; }
        public String AdvisingBankName { get; set; }
        public String BeneficiaryBankName { get; set; }
        public int AdvisingBankID { get; set; }
        public int BeneficiaryBankID { get; set; }
        public int Beneficiary { get; set; }
        public int BeneficiaryAddressID { get; set; }
        public String BeneficiaryName { get; set; }
        public int BuyerAddressID { get; set; }
        public String BenificiaryAddress { get; set; }
        //########
        public byte? PICurrency { get; set; }
        public string PaymentTerm { get; set; }
        public string PaymentMode { get; set; }
        public string DeferredDays { get; set; }
        public int? CountryOforigin { get; set; }
        public int? BeneficiaryBank { get; set; }
        public string DeliveryTerm { get; set; }
        public int? PortofDischarge { get; set; }
        public string DeliveryMode { get; set; }
        public string Transshipment { get; set; }
        public string PartialShipment { get; set; }
        public string GoodDelivery { get; set; }
        public int? PortofLoading { get; set; }
        public string ShippingMark { get; set; }
        public Nullable<int> ExpectedShipmentTime { get; set; }
        public string Packing { get; set; }
        public string PreShipmentIns { get; set; }
        public int? OfferValidityDays { get; set; }


        public decimal? ExchangeValue { get; set; }
          public decimal? ExchangeRate { get; set; }
        public decimal? ActualPrice { get; set; }
    }

    public class GetLCList
    {
        public int LCID { get; set; }
        public string LCNo { get; set; }
        public string SupplierName { get; set; }
        public string LCDate { get; set; }
        public decimal LCAmount { get; set; }
        public decimal LCMargin { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ExchangeAmount { get; set; }
    }
}
