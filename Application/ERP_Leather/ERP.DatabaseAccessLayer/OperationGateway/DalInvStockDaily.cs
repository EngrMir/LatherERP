using ERP.DatabaseAccessLayer.DB;


namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalInvStockDaily
    {
        private readonly BLC_DEVEntities _context;

        public DalInvStockDaily()
        {
            _context = new BLC_DEVEntities();
        }
    }
}
