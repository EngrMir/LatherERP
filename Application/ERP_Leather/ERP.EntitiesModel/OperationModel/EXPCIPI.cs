using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPCIPI
    {
        public long CIPIID { get; set; }
        public long? CIID { get; set; }
        public long? PIID { get; set; }
        public long? LCID { get; set; }
        public string PIStatus { get; set; }
        public decimal? PIAmount { get; set; }
        public byte? PICurrency { get; set; }
        public byte? ExchangeCurrency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? ExchangeValue { get; set; }
        public string RecordStatus { get; set; }
        public string Remarks { get; set; }

        public decimal? PIFootQty { get; set; }
        public decimal? PIFootUnitPrice { get; set; }
        public decimal? PIFootTotalPrice { get; set; }
        public decimal? PIMeterQty { get; set; }
        public decimal? PIMeterUnitPrice { get; set; }
        public decimal? PIMeterTotalPrice { get; set; }

        #region Display

        public string CINo { get; set; }
        public string PINo { get; set; }
        public string LCNo { get; set; }
        public long? BuyerOrderID { get; set; }
        public string BuyerOrderNo { get; set; }
        public long? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public string PIDate { get; set; }
        public string PaymentTerms { get; set; }

        #endregion
    }
}
