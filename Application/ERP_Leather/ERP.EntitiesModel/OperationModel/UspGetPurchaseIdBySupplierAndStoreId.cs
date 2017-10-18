using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class UspGetPurchaseIdBySupplierAndStoreId
    {
        public Nullable<long> PurchaseID { get; set; }
        public Nullable<decimal> ReceiveQty { get; set; }
        public string PurchaseDate { get; set; }
    }
}
