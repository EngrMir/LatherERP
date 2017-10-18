using System;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqPreGradeChallan : CommonStatusInformation
    {
        public long SelectionID { get; set; }
        public long ChallanSelectionID { get; set; }
        public long ChallanID { get; set; }
        public long PurchaseID { get; set; }
        public decimal? ChallanReceiveQty { get; set; }
        public decimal? ChallanSelectionQty { get; set; }
        public string Remarks { get; set; }

    }
}
