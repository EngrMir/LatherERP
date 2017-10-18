using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalInvLeatherStockAdjustRequest
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public DalInvLeatherStockAdjustRequest()
        {
            _context = new BLC_DEVEntities();
            _vmMsg=new ValidationMsg();
        }

        public ValidationMsg Save(InvLeatherStockAdjustModel model)
        {
            using (var tx = new TransactionScope())
            {
                using (_context)
                {

                    
                }

                tx.Complete();

            }

            return _vmMsg;
        }
    }
}
