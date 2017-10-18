using ERP.DatabaseAccessLayer.DB;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqBillPaymentReference
    {
        private readonly BLC_DEVEntities _context;

        public DalPrqBillPaymentReference()
        {
            _context = new BLC_DEVEntities();
        }
    }
}
