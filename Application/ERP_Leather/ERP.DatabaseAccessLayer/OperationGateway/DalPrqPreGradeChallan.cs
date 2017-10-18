using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqPreGradeChallan
    {
        private readonly BLC_DEVEntities _context;

        public DalPrqPreGradeChallan()
        {
            _context = new BLC_DEVEntities();
        }
    }
}
