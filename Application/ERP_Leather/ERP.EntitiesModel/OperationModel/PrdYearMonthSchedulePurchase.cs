using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrdYearMonthSchedulePurchase
    {
        public long SchedulePurchaseID { get; set; }
        public long? ScheduleDateID { get; set; }
        public string ProductionNo { get; set; }
        public int? MachineID { get; set; }
        public string MachineNo { get; set; }
        public int? SupplierID { get; set; }
        public long? PurchaseID { get; set; }
        public byte? ItemTypeID { get; set; }
        public byte? LeatherTypeID { get; set; }
        public byte? LeatherStatusID { get; set; }
        public decimal? ProductionPcs { get; set; }
        public decimal? ProductionSide { get; set; }
        public decimal? ProductionWeight { get; set; }
        public byte? UnitID { get; set; }
        public byte? WeightUnit { get; set; }
        public string UnitName { get; set; }
        public string Remark { get; set; }
        public string PurchaseSign { get; set; }
        public string RecordStatus { get; set; }
        public long ProductionProcessID { get; set; }

        #region Show Field

        public byte? StoreID { get; set; }
        public string StoreName { get; set; }
        public string SupplierName { get; set; }
        public string PurchaseNo { get; set; }
        public string PurchaseDate { get; set; }
        public string SourceName { get; set; }
        public string ItemTypeName { get; set; }
        public string LeatherTypeName { get; set; }
        public string LeatherStatusName { get; set; }
        public decimal? ClosingQty { get; set; }

        #endregion
    }
}
