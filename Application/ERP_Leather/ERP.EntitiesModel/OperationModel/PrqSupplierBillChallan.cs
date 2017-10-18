namespace ERP.EntitiesModel.OperationModel
{
    public class PrqSupplierBillChallan
    {
        public long SupplierBillID { get; set; }
        public long ChallanID { get; set; }
        public string ChallanNo { get; set; }
        public string ChallanDate { get; set; }
        public decimal Quantity { get; set; }
        public string ChallanCategory { get; set; }
        public bool IsDelete { get; set; }
        public long? PurchaseID { get; set; }
    }
}
