using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqPurchaseYearPeriodItemAudit : CommonStatusInformation
    {
        public long RevisieID { get; set; }
        public string RevisionReason { get; set; }
        public byte RevisionNo { get; set; }
        public int PeriodItemID { get; set; }
        public int PeriodID { get; set; }
        public byte ItemTypeID { get; set; }
        public byte LeatherType { get; set; }
        public byte LeatherStatus { get; set; }
        public byte SizeID { get; set; }
        public long? TargetQuantity { get; set; }
        public byte UnitID { get; set; }
        public decimal? TargetValue { get; set; }
        public byte CurrencyID { get; set; }
    }
}
