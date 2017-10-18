using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvLeatherIssueItem : CommonStatusInformation
    {
        public long ItemIssueID { get; set; }
        public long IssueID { get; set; }
        public int SupplierID { get; set; }
        public long? ChallanID { get; set; }
        public long PurchaseID { get; set; }
        public long ChallanItemID { get; set; }
        public byte ItemType { get; set; }
        public byte LeatherType { get; set; }
        public byte LeatherStatus { get; set; }
        public decimal IssueQty { get; set; }
        public byte UnitID { get; set; }
        public int? IssueSide { get; set; }
        public string Remarks { get; set; }

        #region ShowField
        public byte? StoreIdIssueFrom { get; set; }
        public byte? StoreIdIssueTo { get; set; }
        public string ChallanNo { get; set; }
        public string PurchaseNo { get; set; }
        public string SupplierName { get; set; }
        public string ItemTypeName { get; set; }
        public string LeatherTypeName { get; set; }
        public string LeatherStatusName { get; set; }
        public string UnitName { get; set; }
        public decimal? StockQty { get; set; }
        public decimal ReceiveQty { get; set; }
        public int? ReceiveSide { get; set; }

        #endregion
    }
}
