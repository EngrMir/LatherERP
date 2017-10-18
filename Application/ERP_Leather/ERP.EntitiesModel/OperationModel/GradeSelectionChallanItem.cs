using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class GradeSelectionChallanItem
    {
        public byte ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public string PurchaseNo { get; set; }
        public decimal ReceiveQty { get; set; }
    }
}
