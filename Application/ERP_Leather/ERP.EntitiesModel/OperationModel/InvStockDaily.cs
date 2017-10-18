using System;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvStockDaily
    {
        public long TransectionID { get; set; }
        public DateTime StockDate { get; set; }
        public byte StoreID { get; set; }
        public byte ItemTypeID { get; set; }
        public byte LeatherType { get; set; }
        public byte LeatherStatus { get; set; }
        public byte UnitID { get; set; }
        public decimal? OpeningQty { get; set; }
        public decimal? DailyReceiveQty { get; set; }
        public decimal? DailyIssueQty { get; set; }
        public decimal? ClosingQty { get; set; }
    }
}
