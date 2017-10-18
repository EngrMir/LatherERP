using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class ReportBillModel
    {
        public string SupplierID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string PurchaseYear { get; set; }
        public string StoreID { get; set; }
        public string ItemTypeID { get; set; }
        public string ApproximateRate { get; set; }
        public string BillID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public string ReportName { get; set; }
        public string ReportType { get; set; }
    }
}
