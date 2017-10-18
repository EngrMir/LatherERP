using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPAgentCommission
    {
        public long AgentComID { get; set; }
        public string AgentComNo { get; set; }
        public string AgentComRef { get; set; }
        public string AgentComDate { get; set; }
        //public string AgentComCatg { get; set; }
        //public int? BuyerAgentID { get; set; }
        //public int? AgentID { get; set; }
        //public string BuyerAgentName { get; set; }
        public string RecordStatus { get; set; }
        public string RecordStatusName { get; set; }
        public long? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public long CIID { get; set; }
        public string CINo { get; set; }
        public string CIDate { get; set; }
        public decimal? CIAmount { get; set; }
        public decimal? ComOnAmount { get; set; }
        public byte? InvoiceCurrency { get; set; }
        public string InvoiceCurrencyName { get; set; }
        public byte? LocalCurrency { get; set; }
        public string LocalCurrencyName { get; set; }
        public virtual IList<EXPAgentCommissionBuyer> EXPAgentCommissionBuyerList { get; set; }
        public virtual IList<EXPAgentCommissionBuyerCI> EXPAgentCommissionBuyerCIList { get; set; }
        public long? PIID { get; set; }
        public string PINo { get; set; }
        public long? BuyerOrderID { get; set; }
        public string BuyerOrderNo { get; set; }
        //public string BuyerName { get; set; }
    }
}
