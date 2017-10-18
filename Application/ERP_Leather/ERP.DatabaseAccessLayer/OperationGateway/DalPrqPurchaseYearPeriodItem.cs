using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqPurchaseYearPeriodItem
    {
        private readonly BLC_DEVEntities _context;

        public DalPrqPurchaseYearPeriodItem()
        {
            _context = new BLC_DEVEntities();
        }
    }
}
