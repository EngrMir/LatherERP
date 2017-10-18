using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqSelectionReport
    {
        public int StoreId { get; set; }

        public int SupllierId { get; set; }
        public int PurchaseId { get; set; }
        public string  DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool IsActive { get; set; }
    }
}
