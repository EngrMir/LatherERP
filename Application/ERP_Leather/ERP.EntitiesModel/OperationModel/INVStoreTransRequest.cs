using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class INVStoreTransRequest : CommonStatusInformation
    {
        public long TransRequestID { get; set; }
        public string TransRequestNo { get; set; }
        public string RequestDate { get; set; }
        public long? TransactionID { get; set; }
        public long? RequestID { get; set; }
        public string RequestNo { get; set; }
        public string TransMethod { get; set; }
        public string Remark { get; set; }
    }
}
