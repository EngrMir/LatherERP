using System;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvLeatherStockAdjustRequest : CommonStatusInformation
    {
        public int RequestID { get; set; }
        public string RequestDate { get; set; }
        public byte LeatherType { get; set; }
        public string LeatherTypeName { get; set; }
        public byte StoreID { get; set; }
        public string StoreName { get; set; }
        public string PurchaseYear { get; set; }
        public int? CheckedBy { get; set; }
        public DateTime? CheckDate { get; set; }
        public string CheckComment { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string ApproveComment { get; set; }

        
        
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ChallanID { get; set; }


    }
}
