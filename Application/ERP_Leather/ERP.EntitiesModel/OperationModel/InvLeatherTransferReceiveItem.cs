using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class InvLeatherTransferReceiveItem : CommonStatusInformation
    {
        public long ItemReceiveID { get; set; }
        public long ReceiveID { get; set; }
        public int SupplierID { get; set; }
        public long? ChallanID { get; set; }
        public long? PurchaseID { get; set; }
        public byte ItemType { get; set; }
        public byte LeatherType { get; set; }
        public byte LeatherStatus { get; set; }
        public decimal? IssueQty { get; set; }
        public decimal? ReceiveQty { get; set; }
        public byte UnitID { get; set; }
        public int? IssueSide { get; set; }
        public int? ReceiveSide { get; set; }
        public string Remarks { get; set; }

        #region ShowField
        public byte StoreID { get; set; }
        public long ItemIssueID { get; set; }
        public string ChallanNo { get; set; }
        public string PurchaseNo { get; set; }
        public string SupplierName { get; set; }
        public string ItemTypeName { get; set; }
        public string LeatherTypeName { get; set; }
        public string LeatherStatusName { get; set; }
        public string UnitName { get; set; }

        #endregion
    }
}
