using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysSupplierAgent
    {

        public int SupplierAgentID { get; set; }
        public int? AgentID { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string AgentType { get; set; }
        public string IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime SetOn { get; set; }
        public int SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
