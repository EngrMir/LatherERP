using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
  public  class InvLeatherStockAdjustModel
  {
      public string RequestID { get; set; }
      public string RequestDate { get; set; }
      public byte LeatherType { get; set; }
      public byte StoreID { get; set; }
      public string PurchaseYear { get; set; }
  
      public virtual ICollection<InvLeatherStockAdjustItem>  StockAdjustItemList { get; set; }
  }
}
