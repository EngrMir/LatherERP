using System;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqPurchaseChallan : CommonStatusInformation
    {
        public long ChallanID { get; set; }
        public string ChallanNo { get; set; }
        public long PurchaseID { get; set; }
        public int SourceID { get; set; }
        public string SourceName { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public string ReceiveStore { get; set; }
        public string ChallanCategory { get; set; }
        public string ChallanNote { get; set; }
        public string ChallanDate { get; set; }
    }
}
