namespace ERP.EntitiesModel.OperationModel
{
    public class PrqBillPaymentReference
    {
        public long PaymentID { get; set; }
        public long SupplierBillID { get; set; }
        public string SupplierBillNo { get; set; }
        //public string SupplierBillRef { get; set; }
        public string RecordStatus { get; set; }
    }
}
