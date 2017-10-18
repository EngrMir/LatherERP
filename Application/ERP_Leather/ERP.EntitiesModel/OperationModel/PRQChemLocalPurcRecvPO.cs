using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRQChemLocalPurcRecvPO
    {
        public long POReceiveID { get; set; }
        public long? ReceiveID { get; set; }
        public int? OrderID { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public string Remark { get; set; }
        public string RecordStatus { get; set; }
        public int? CheckedBy { get; set; }
        public string CheckDate { get; set; }
    }
}
