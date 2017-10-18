using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class INVStoreTransItem : CommonStatusInformation
    {
        public long TransItemID { get; set; }
        public long? TransactionID { get; set; }
        public long? TransRequestID { get; set; }
        public int? ItemID { get; set; }
        public string ChemicalName { get; set; }
        public int? SupplierID { get; set; }
        public string Supplier { get; set; }
        public int? ManufacturerID { get; set; }
        public decimal? TransactionQty { get; set; }
        public decimal? StockQty { get; set; }
        public decimal? PackStockQty { get; set; }
        public byte? TransactionUnit { get; set; }
        public string TransactionUnitName { get; set; }
        public byte? PackSize { get; set; }
        public string PackSizeName { get; set; }
        public byte? SizeUnit { get; set; }
        public string SizeUnitName { get; set; }
        public int? PackQty { get; set; }
        public decimal? RefTransactionQty { get; set; }
        public byte RefTransactionUnit { get; set; }
        public string RefTransactionUnitName { get; set; }
        public byte? RefPackSize { get; set; }
        public string RefPackSizeName { get; set; }
        public byte? RefSizeUnit { get; set; }
        public string RefSizeUnitName { get; set; }
        public int? RefPackQty { get; set; }
        public string ItemSource { get; set; }
        public decimal? PackClosingQty { get; set; }
        public decimal? ClosingQty { get; set; }
        //public decimal? StockQty { get; set; }
    }
}
