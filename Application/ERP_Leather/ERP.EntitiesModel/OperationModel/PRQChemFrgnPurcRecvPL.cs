using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PRQChemFrgnPurcRecvPL
    {
        public int PLReceiveID { get; set; }
        public int? ReceiveID { get; set; }
        public int? PLID { get; set; }
        public string PLNo { get; set; }
        public string PLDate { get; set; }
        public int? CIID { get; set; }
        public string CINo { get; set; }
        public string CIDate { get; set; }
        public int? LCID { get; set; }
        public string LCNo { get; set; }
        public string LCDate { get; set; }
        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public int PIID { get; set; }
        public string PINo { get; set; }
        public string PIDate { get; set; }
        public string Remark { get; set; }
        public string RecordStatus { get; set; }
        public int? CheckedBy { get; set; }
        public string CheckDate { get; set; }
    }
}
