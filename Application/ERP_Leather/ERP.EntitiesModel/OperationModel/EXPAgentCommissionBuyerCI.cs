using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPAgentCommissionBuyerCI
    {
        public long AgentComCIID { get; set; }
        public long AgentComID { get; set; }
        public int? BuyerAgentID { get; set; }
        public string BuyerAgentName { get; set; }
        public string AgentType { get; set; }
        public string LocalAgentType { get; set; }
        public string ForeignAgentType { get; set; }
        public long CIID { get; set; }
        public string CINo { get; set; }
        public decimal? CommissionPercent { get; set; }
        public decimal? CommissionAmount { get; set; }
        public decimal? AITPercent { get; set; }
        public decimal? AITAmount { get; set; }
        public decimal? PayableAmount { get; set; }
        public byte? ExchangeCurrency { get; set; }
        public string ExchangeCurrencyName { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? ExchangeAmount { get; set; }
        public byte? LocalCurrency { get; set; }
        public decimal? LocalRate { get; set; }
        public decimal? LocalAmount { get; set; }
        public string RecordStatus { get; set; }

        public long? PIID { get; set; }
        public string PINo { get; set; }
        public long? BuyerOrderID { get; set; }
        public string BuyerOrderNo { get; set; }
        public string BuyerName { get; set; }

        public int? LocalAgent { get; set; }
        public string LocalAgentName { get; set; }
        public int? ForeignAgent { get; set; }
        public string ForeignAgentName { get; set; }
        public decimal? LocalComPercent { get; set; }
        public decimal? ForeignComPercent { get; set; }
    }
}
