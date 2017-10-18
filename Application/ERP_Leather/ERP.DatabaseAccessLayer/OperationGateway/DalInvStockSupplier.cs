using ERP.DatabaseAccessLayer.DB;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalInvStockSupplier
    {
        private readonly BLC_DEVEntities _context;

        public DalInvStockSupplier()
        {
            _context = new BLC_DEVEntities();
        }
    }
}
