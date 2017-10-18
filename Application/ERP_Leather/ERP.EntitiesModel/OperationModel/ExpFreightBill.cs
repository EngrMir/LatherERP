using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class ExpFreightBill
    {
        public long FreightBillID { get; set; }
        public string FreightBillNo { get; set; }
        public string FreightBillRef { get; set; }
        public string FreightBillDate { get; set; }
        public long? LCID { get; set; }
        public string LCNo { get; set; }
        public long? PIID { get; set; }
        public string PINo { get; set; }
        public string PIDate { get; set; }
        public long? BLID { get; set; }
        public string BLNo { get; set; }
        public string BLDate { get; set; }
        public long? CIID { get; set; }
        public string CINo { get; set; }
        public string CIRefNo { get; set; }
        public string CIDate { get; set; }
        public string OrdDeliveryMode { get; set; }
        public int? FreightAgentID { get; set; }
        public string FreightAgentCode { get; set; }
        public string FreightAgentName { get; set; }
        public string NotifyTo { get; set; }
        public string MSNo { get; set; }
        public string MSDate { get; set; }
        public string ShipmentOf { get; set; }
        public string ShipmentFrom { get; set; }
        public string ShipmentTo { get; set; }
        public string WBNo { get; set; }
        public string WBDate { get; set; }
        public decimal? FreightWeight { get; set; }
        public decimal? FreightRate { get; set; }
        public decimal? FreightValue { get; set; }
        public byte? FreightBillCurrency { get; set; }
        public byte? ExchangeCurrency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? ExchangeValue { get; set; }
        public decimal? THCharge { get; set; }
        public decimal? AirWayBill { get; set; }
        public decimal? FCSMYCCharge { get; set; }
        public decimal? SSCMCCCharge { get; set; }
        public decimal? OtherCharge { get; set; }
        public decimal? LocalCarringCharge { get; set; }
        public decimal? CustomCharge { get; set; }
        public decimal? VATAsReceipt { get; set; }
        public decimal? LoadUnloadCharge { get; set; }
        public decimal? GSPEXpence { get; set; }
        public decimal? AgencyCommision { get; set; }
        public decimal? SpecialDeliveryCharge { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? AdvanceAmt { get; set; }
        public decimal? NetFreightAmt { get; set; }
        public decimal? TrminalCharge { get; set; }
        public decimal? ExamineCharge { get; set; }
        public decimal? AmendmentCharge { get; set; }
        public string FreightBillNote { get; set; }
        public string RecordStatus { get; set; }
    }
}
