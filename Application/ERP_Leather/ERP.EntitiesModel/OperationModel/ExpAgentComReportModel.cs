using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class ExpAgentComReportModel
    {
        public long AgentComID { get; set; }
        public string AgentComNo { get; set; }
        public long? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public long? AgentID { get; set; }
        public string AgentName { get; set; }
        public long CIID { get; set; }
        public string CINo { get; set; }
        public long? PIID { get; set; }
        public string PINo { get; set; }
        public long BVID { get; set; }
        public string BVNo { get; set; }
        public int? BankID { get; set; }
        public string BankName { get; set; }
        public long? BLID { get; set; }
        public string BLNo { get; set; }
        public long? TPortID { get; set; }
        public string TPortName { get; set; }
        public long? PortID { get; set; }
        public string PortName { get; set; }
        public long FreightBillID { get; set; }
        public string FreightBillNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        
         
    }
     
}
