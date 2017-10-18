using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class EXPAgentCommissionBuyer
    {
        public long AgentComID { get; set; }
        public string AgentComNo { get; set; }
        public int BuyerID { get; set; }
        public string BuyerName { get; set; }
        public string RecordStatus { get; set; }
    }
}
