using System;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PrqPurchase : CommonStatusInformation
    {
        public long PurchaseID { get; set; }
        public string PurchaseNo { get; set; }
        public int SupplierID { get; set; }

        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public int SupplierAddressID { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }

        public string PurchaseCategory { get; set; }
        public string PurchaseType { get; set; }
        public string PurchaseYear { get; set; }
        public string PurchaseDate { get; set; }
        public string PurchaseNote { get; set; }
        public int? CheckedBy { get; set; }
        public DateTime? CheckDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApproveDate { get; set; }

    }
}
