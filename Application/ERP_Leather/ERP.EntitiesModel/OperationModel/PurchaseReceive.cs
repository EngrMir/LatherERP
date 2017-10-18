using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.EntitiesModel.BaseModel;

namespace ERP.EntitiesModel.OperationModel
{
    public class PurchaseReceive : CommonStatusInformation
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }

        public int SupplierAddressID { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public long PurchaseID { get; set; }
        public string PurchaseNo { get; set; }
        public string PurchaseCategory { get; set; }
        public string PurchaseType { get; set; }
        public string PurchaseYear { get; set; }
        public string PurchaseDate { get; set; }
        public DateTime TempPurchaseDate { get; set; }
        public string PurchaseNote { get; set; }
        public string CheckedBy { get; set; }
        public decimal TotalItem { get; set; }

        public virtual ICollection<PrqPurchaseChallan> ChallanList { get; set; }
        public virtual ICollection<PrqPurchaseChallanItem> ChallanItemList { get; set; }

    }


    public class PurchaseReceiveTest:CommonStatusInformation
    {
        public PrqPurchase PurchaseInformation { get; set; }
        public virtual ICollection<PrqPurchaseChallan> ChallanList { get; set; }
        public virtual ICollection<PrqPurchaseChallanItem> ChallanItemList { get; set; }

    }
}
