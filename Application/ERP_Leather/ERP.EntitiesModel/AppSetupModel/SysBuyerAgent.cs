using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysBuyerAgent
    {
        public int BuyerAgentID { get; set; }
        public int? AgentID { get; set; }
        public int? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public string BuyerCode { get; set; }
        public string AgentType { get; set; }
        public string IsActive { get; set; }
        public DateTime SetOn { get; set; }
        public int SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string AgentCode { get; set; }
        public string AgentName { get; set; }

        public IList<SysBuyerAgent> AgentList = new List<SysBuyerAgent>();
    }
}
