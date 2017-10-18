using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRQChemicalPI : CommonStatusInformation
    {
        public int PIID { get; set; }
        public string PINo { get; set; }
        public string PIDate { get; set; }

        public string PICategory { get; set; }
        public int? OrderID { get; set; }
        public string OrderNo { get; set; }
        public string PIReceiveDate { get; set; }
        public int SupplierID { get; set; }

        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierContactNumber { get; set; }

        public int LocalAgent { get; set; }
        public string LocalAgentCode { get; set; }
        public string LocalAgentName { get; set; }
        public string LocalAgentAddress { get; set; }
        public string LocalAgentContactNumber { get; set; }

        public int ForeignAgent { get; set; }
        public string ForeignAgentCode { get; set; }
        public string ForeignAgentName { get; set; }
        public string ForeignAgentAddress { get; set; }
        public string ForeignAgentContactNumber { get; set; }
        public int BuyerID { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public int BuyerAddressID { get; set; }
        public string Address { get; set; }
        public string BuyerContactNumber { get; set; }

        public byte PICurrency { get; set; }
        public byte ExchangeCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ExchangeValue { get; set; }
        public string PaymentTerm { get; set; }
        public string PaymentMode { get; set; }

        public string DeferredDays { get; set; }
        public int CountryOforigin { get; set; }
        public string CountryOforiginName { get; set; }
        public int BeneficiaryBank { get; set; }
        public string BeneficiaryBankName { get; set; }
        public int AdvisingBank { get; set; }

        public string DeliveryTerm { get; set; }
        public string DeliveryMode { get; set; }
        public int PortofLoading { get; set; }
        public string PortofLoadingName { get; set; }
        public int PortofDischarge { get; set; }
        public string PortofDischargeName { get; set; }
        public string Transshipment { get; set; }
        public string PartialShipment { get; set; }

        public string GoodDelivery { get; set; }
        public string ShippingMark { get; set; }
        public int ExpectedShipmentTime { get; set; }

        public string Packing { get; set; }
        public string PreShipmentIns { get; set; }
        public int OfferValidityDays { get; set; }
        public string OfferValidityNote { get; set; }
        public string PIRevisionDate { get; set; }
        public string PIState { get; set; }

        public string PIStatus { get; set; }
        //public string RecordStatus { get; set; }
        public string PINote { get; set; }

        public string RunningStatus { get; set; }
        public int CostIndicator { get; set; }
        public int CheckedBy { get; set; }
        public string CheckDate { get; set; }
        public string ApprovedBy { get; set; }
        public string ApproveDate { get; set; }
        public string ApprovalAdvice { get; set; }

        public decimal? FreightCharge { get; set; }
        public decimal? FreightChargeExtra { get; set; }

        public virtual ICollection<PRQChemicalPIItem> PIItemList { get; set; }

    }
}
