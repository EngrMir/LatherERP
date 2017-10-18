using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
   public  class ChemicalPurchaseItemModel
    {
         public int ItemID { get; set; }
         public string ItemName { get; set; }
         public string HSCode { get; set; }
         public int? SafetyStock { get; set; }
         public decimal? StockQty { get; set; }
         public decimal? PipelineQty { get; set; }
         public decimal? TotalQty { get; set; }
         public int? ReorderLevel { get; set; }
         public decimal ProdReqQty { get; set; }
         public byte? UnitID { get; set; }
         public string UnitName { get; set; }

    }
}
