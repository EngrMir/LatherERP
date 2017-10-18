using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntitiesModel.OperationModel
{
    public class SupplierBillVM
    {
        public Int64 Id { get; set; }
        public string BillNo { get; set; }
        public IList<Challan> Challans = new List<Challan>();
        //public List<Challan> Challans { get; set; }
        public string PurchaseYear { get; set; }
        public string BillType { get; set; }
        public string Category { get; set; }
        public int TotalQty { get; set; }
        public int TotalRejectQuantity { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ApproveAmount { get; set; }
        public decimal OtherCost { get; set; }
        public decimal DiscountPer { get; set; }
        public decimal DiscountVal { get; set; }
        public decimal PayableAmount { get; set; }
        public string Currency { get; set; }
        public string Remark { get; set; }
        public IList<SupplierBillItemVM> SupplierBillItems = new List<SupplierBillItemVM>();
        //public List<SupplierBillItemVM> SupplierBillItems { get; set; }

    }

    public class SupplierBillItemVM
    {
        public long Id { get; set; }
        public long BillId { get; set; }
        public string ItemType { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public int RejectQuantity { get; set; }
        public decimal ItemRate { get; set; }
        public decimal ApproveRate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal AverageArea { get; set; }
        public string AreaUnit { get; set; }
        public string Remark { get; set; }
    }

    public class Challan
    {
        public long ChallanId { get; set; }
        public string ChallanRef { get; set; }
        public string ChallanCategory { get; set; }
    }
}
