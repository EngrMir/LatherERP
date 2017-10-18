using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class INVStoreTransChallan : CommonStatusInformation
    {
        public long TransChallanID { get; set; }
        public long? TransactionID { get; set; }
        public string TransChallanNo { get; set; }
        public string RefChallanNo { get; set; }
        public string ChallanDate { get; set; }
        public decimal? CarringCost { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? OtherCost { get; set; }
        public byte Currency { get; set; }
        public string CurrencyName { get; set; }
        public string Remark { get; set; }
    }
}
