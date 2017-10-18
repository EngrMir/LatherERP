using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class ChemicalStockInfoModel
    {
        public int? ItemID { get; set; }
        public int? SupplierID { get; set; }
        public decimal? ClosingQty { get; set; }
        public byte? UnitID { get; set; }

        public byte? StoreID { get; set; }
        public long TransectionID { get; set; }


    }
}
