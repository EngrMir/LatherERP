using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdCrustLeatherQCRef
    {
        public long CrustLeatherQCRefID { get; set; }
        public long CrustLeatherQCID { get; set; }
        public long CLProductionID { get; set; }
        public string Remarks { get; set; }
        public string RecordStatus { get; set; }
        public string RecordState { get; set; }
    }
}
