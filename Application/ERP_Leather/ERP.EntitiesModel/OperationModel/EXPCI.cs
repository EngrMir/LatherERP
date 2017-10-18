using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPCI
    {
        public long CIID { get; set; }
        public string CINo { get; set; }
        public string CIRefNo { get; set; }
        public string CIDate { get; set; }
        public string CIType { get; set; }
        public decimal? CIAmount { get; set; }
        public byte? CICurrency { get; set; }
        public byte? ExchangeCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ExchangeValue { get; set; }
        public string CINote { get; set; }
        public string ExportNo { get; set; }
        public string ExportDate { get; set; }
        public string OnAccountRiskOf { get; set; }
        public string NotifyParty { get; set; }
        public string ShipmentFrom { get; set; }
        public string ShipmentTo { get; set; }
        public string DrawnUnder { get; set; }
        public string PaymentTerms { get; set; }
        public string MarksAndNumber { get; set; }
        public string RecordStatus { get; set; }
        public string RecordStatusName { get; set; }
        public string PriceLevel { get; set; }

        public string OrdDeliveryMode { get; set; }
        public string OrdDeliveryModeName { get; set; }
        public decimal? CIFootQty { get; set; }
        public decimal? CIFootUnitPrice { get; set; }
        public decimal? CIFootTotalPrice { get; set; }
        public decimal? CIMeterQty { get; set; }
        public decimal? CIMeterUnitPrice { get; set; }
        public decimal? CIMeterTotalPrice { get; set; }

        public virtual IList<EXPCIPI> ExpCIPIList { get; set; }
        public virtual IList<EXPCIPIItem> ExpCIPIItemList { get; set; }
        public virtual IList<EXPCIPIItemColor> ExpCIPIItemColorList { get; set; }
        public virtual IList<SlsBuyerOrderPrice> BuyerOrderPriceList { get; set; }
    }
}
