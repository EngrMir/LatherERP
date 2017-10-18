namespace ERP.EntitiesModel.OperationModel
{
    public class InvStockSupplier
    {
        public long TransectionID { get; set; }
        public int SupplierID { get; set; }
        public byte StoreID { get; set; }
        public long RefChallanID { get; set; }
        public byte ItemTypeID { get; set; }
        public byte LeatherType { get; set; }
        public byte LeatherStatusID { get; set; }
        public byte UnitID { get; set; }
        public decimal? OpeningQty { get; set; }
        public decimal? ReceiveQty { get; set; }
        public decimal? IssueQty { get; set; }
        public decimal? ClosingQty { get; set; }
        public string UpdateReason { get; set; }

        #region ShowField

        public string ItemTypeName { get; set; }
        public string LeatherTypeName { get; set; }
        public string LeatherStatusName { get; set; }
        public string UnitName { get; set; }
        public decimal? StockQty { get; set; }

        #endregion
    }
}
