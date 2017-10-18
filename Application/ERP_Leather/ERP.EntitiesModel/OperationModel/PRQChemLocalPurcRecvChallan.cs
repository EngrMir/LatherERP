using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRQChemLocalPurcRecvChallan
    {
        public long ReceiveChallanID { get; set; }
        public long? POReceiveID { get; set; }
        public long? ReceiveID { get; set; }
        public string ReceiveChallanNo { get; set; }
        public string SupChallanNo { get; set; }
        public string ChallanDate { get; set; }
        public decimal? CarringCost { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? OtherCost { get; set; }
        public byte? Currency { get; set; }
        public string Remark { get; set; }
    }
}
