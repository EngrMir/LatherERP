using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqPreGradeSelectedData
    {
        private readonly BLC_DEVEntities _context;

        public DalPrqPreGradeSelectedData()
        {
            _context = new BLC_DEVEntities();
        }

        //public List<UspConfirmGradeSelection_Result> GradeSelectionConfirm(long PurchaseID, int ItemTypeID, decimal ReceiveQty)
        //{
        //    return _context.UspConfirmGradeSelection(PurchaseID, ItemTypeID, ReceiveQty).ToList();
        //}
    }
}
