using ERP.DatabaseAccessLayer.DB;


namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalInvLeatherTransferReceiveItem
    {
        private readonly BLC_DEVEntities _context;

        public DalInvLeatherTransferReceiveItem()
        {
            _context = new BLC_DEVEntities();
        }
    }
}
