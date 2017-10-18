using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqPurchaseYearPeriodItemAudit
    {
        private readonly BLC_DEVEntities _context;

        public DalPrqPurchaseYearPeriodItemAudit()
        {
            _context = new BLC_DEVEntities();
        }
    }
}
