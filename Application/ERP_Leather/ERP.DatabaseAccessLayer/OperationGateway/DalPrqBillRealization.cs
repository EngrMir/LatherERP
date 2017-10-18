using ERP.DatabaseAccessLayer.DB;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqBillRealization
    {
        private readonly BLC_DEVEntities _context;

        public DalPrqBillRealization()
        {
            _context = new BLC_DEVEntities();
        }
    }
}
