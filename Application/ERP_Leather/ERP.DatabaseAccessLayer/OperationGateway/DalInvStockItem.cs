using ERP.DatabaseAccessLayer.DB;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalInvStockItem
    {
        private readonly BLC_DEVEntities _context;

        public DalInvStockItem()
        {
            _context = new BLC_DEVEntities();
        }
    }
}
