using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvLoanReturnRequestData
    {
        public long RequestID { get; set; }
        public string ReceiveReqNo { get; set; }   
        public string ReturnReqNo { get; set; }
        public string RequestDate { get; set; }
        public string RecordStatus { get; set; }
     
    }
}
