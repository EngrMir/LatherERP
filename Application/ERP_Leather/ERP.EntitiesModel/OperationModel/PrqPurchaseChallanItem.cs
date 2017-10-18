using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqPurchaseChallanItem : CommonStatusInformation
    {
        public long ChallanItemID { get; set; }
        public long ChallanID { get; set; }
        public string ItemCategory { get; set; }
        public string ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public string ItemSizeID { get; set; }
        public string ItemSizeName { get; set; }
        public string SizeID { get; set; }
        public string SizeName { get; set; }
        public string Description { get; set; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public decimal ChallanQty { get; set; }
        public decimal ReceiveQty { get; set; }
        public string Remark { get; set; }
    }
}
