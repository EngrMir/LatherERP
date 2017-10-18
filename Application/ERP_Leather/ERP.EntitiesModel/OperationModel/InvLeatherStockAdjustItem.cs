namespace ERP.EntitiesModel.OperationModel
{
    public class InvLeatherStockAdjustItem
    {
        public int AdjustID { get; set; }
        public int RequestID { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public long? ChallanID { get; set; }

        public string ChallanNo { get; set; }
        public string AdjustmentCause { get; set; }
        public byte ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }

        public byte StoreID { get; set; }
        public string StoreName { get; set; }
        public byte LeatherType { get; set; }
        public byte LeatherStatus { get; set; }
        public decimal ItemQty { get; set; }

        public decimal? StockQty { get; set; }
        public byte Unit { get; set; }
        public string UnitName { get; set; }
        public decimal? SideQty { get; set; }
        public decimal? ItemValue { get; set; }
        public string Remarks { get; set; }

        public long? PurchaseID { get; set; }

        public string PurchaseNo { get; set; }
    }
}
