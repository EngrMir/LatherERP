using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqSupplierBillItem : CommonStatusInformation
    {
        public long BillItemID { get; set; }
        public long SupplierBillID { get; set; }
        public byte ItemTypeID { get; set; }
        public byte ItemSizeID { get; set; }
        public decimal ItemQty { get; set; }
        public decimal? RejectQty { get; set; }
        public decimal? ItemRate { get; set; }
        public decimal? ApproveRate { get; set; }
        public decimal? Amount { get; set; }
        public byte? AmtUnit { get; set; }
        public decimal? AvgArea { get; set; }
        public byte? AreaUnit { get; set; }
        public string Remarks { get; set; }
        
        #region ShowData

        public string ItemTypeName { get; set; }
        public string ItemSizeName { get; set; }
        public string AmtUnitName { get; set; }
        public string AreaUnitName { get; set; }

        #endregion
    }
}
