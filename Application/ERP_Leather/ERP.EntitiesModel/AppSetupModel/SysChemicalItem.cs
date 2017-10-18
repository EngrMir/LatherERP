using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.AppSetupModel
{
    public class SysChemicalItem
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string HSCode { get; set; }
        public string ItemCategory { get; set; }
        public string ItemCategoryName { get; set; }
        public byte? ItemTypeID { get; set; }
        public byte? UnitID { get; set; }
        public int? SafetyStock { get; set; }
        public int? ReorderLevel { get; set; }
        public int? ExpiryLimit { get; set; }
        public string IsActive { get; set; }

        #region Show Field
        public string ItemTypeName { get; set; }
        public string UnitName { get; set; }

        public decimal? StockQty { get; set; }
        public byte? StockUnit { get; set; }
        public string StockUnitName { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }

        #endregion
    }
}
