using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPLeatherPI
    {
        public long PIID { get; set; }
        public string PINo { get; set; }
        public string PIDate { get; set; }
        public string PICategory { get; set; }
        public Nullable<long> BuyerOrderID { get; set; }
        public Nullable<DateTime> PIIssueDate { get; set; }
        public Nullable<int> BuyerID { get; set; }
        public Nullable<int> LocalAgent { get; set; }
        public Nullable<int> ForeignAgent { get; set; }
        public decimal? LocalComPercent { get; set; }
        public decimal? ForeignComPercent { get; set; }
        public Nullable<int> PIBeneficiary { get; set; }
        public Nullable<int> BeneficiaryAddressID { get; set; }
        public string PINotify { get; set; }
        public string PIConsignee { get; set; }
        public Nullable<byte> PICurrency { get; set; }
        public Nullable<byte> ExchangeCurrency { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public Nullable<decimal> ExchangeValue { get; set; }
        public string PaymentTerm { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentNote { get; set; }
        public string DeferredDays { get; set; }
        public Nullable<int> CountryOforigin { get; set; }
        public Nullable<int> BuyerBank { get; set; }
        public string BuyerBankAccount { get; set; }
        public string PriceLevel { get; set; }
        public Nullable<int> BeneficiaryBank { get; set; }
        public string DeliveryTerm { get; set; }
        public string DeliveryMode { get; set; }
        public Nullable<int> PortofLoading { get; set; }
        public Nullable<int> PortofDischarge { get; set; }
        public string Transshipment { get; set; }
        public string PartialShipment { get; set; }
        public string GoodDelivery { get; set; }
        public string ShippingMark { get; set; }
        public Nullable<int> ExpectedShipmentTime { get; set; }
        public Nullable<DateTime> ExpectedDeliveryDate { get; set; }
        public Nullable<DateTime> ActualDeliveryDate { get; set; }
        public string Packing { get; set; }
        public string PreShipmentIns { get; set; }
        public Nullable<int> OfferValidityDays { get; set; }
        public string OfferValidityNote { get; set; }


        public string OfferNote { get; set; }
        public string OtherTerm { get; set; }
        public Nullable<DateTime> PIRevisionDate { get; set; }
        public string PIState { get; set; }
        public string PIStatus { get; set; }
        public string RecordStatus { get; set; }
        public string PINote { get; set; }
        public string RunningStatus { get; set; }
        public Nullable<byte> CostIndicator { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public string ApprovalAdvice { get; set; }
        public Nullable<System.DateTime> SetOn { get; set; }
        public Nullable<int> SetBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string IPAddress { get; set; }



        #region Display field

        public string PICurrencyName { get; set; }
        public string PaymentName { get; set; }
        public string LocalAgentName { get; set; }
        public string ForeignAgentName { get; set; }
        public string BeneficiaryBankName { get; set; }
        public int BankID { get; set; }
        public string BankName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }

        public int BuyerAgentID { get; set; }
        public string BuyerName { get; set; }

        #endregion


        #region Badhon
        public int? BeneficiaryID { get; set; }
        public string BuyerOrderDate { get; set; }
        public string ShipmentDate { get; set; }
        public string ShipmentNote { get; set; }
        public string BankAccount { get; set; }
        public string InsuranceTerm { get; set; }
        public long SelectedPIItemID { get; set; }

        #endregion

        public IList<EXPPIItem> PIItem { get; set; }
        public IList<EXPPIItemColor> PIColor { get; set; }

        public IList<SlsBuyerOrderDeliveryBadhon> OrderDeliveryDates { get; set; }
        




    }
}
