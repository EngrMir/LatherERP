using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqPurchaseYearPeriodItem : CommonStatusInformation
    {
        public int PeriodItemID { get; set; }
        public int LeatherStatusID { get; set; }
        public int PeriodID { get; set; }
        public byte ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public byte LeatherType { get; set; }
        public string LeatherTypeName { get; set; }
        public byte LeatherStatus { get; set; }
        public string LeatherStatusName { get; set; }
        public byte SizeID { get; set; }
        public string SizeName { get; set; }
        public long? TargetQuantity { get; set; }
        public byte UnitID { get; set; }
        public string UnitName { get; set; }
        public decimal? TargetValue { get; set; }
        public byte CurrencyID { get; set; }
        public string CurrencyName { get; set; }
    }
}
