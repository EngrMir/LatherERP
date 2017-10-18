using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqSupplierBillChallan
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public DalPrqSupplierBillChallan()
        {
            _vmMsg = new ValidationMsg();
            _context = new BLC_DEVEntities();
        }

        //public List<PrqSupplierBillChallan> GetSupplierChallans(string supplierBillId)
        //{
        //    string query = "SELECT * FROM [dbo].[Prq_SupplierBillChallan] " +
        //                   "WHERE SupplierBillID = '" + supplierBillId + "' AND IsDelete = '" + false + "'";

        //    var challanIdList = _context.Database.SqlQuery<PrqSupplierBillChallan>(query).ToList();
        //    return challanIdList;
        //}

        public ValidationMsg DeleteSupplierBillChallan(long billId, long challanId)
        {
            try
            {
                var deleteItem =_context.Prq_SupplierBillChallan.FirstOrDefault(item => item.SupplierBillID == billId && item.ChallanID == challanId);

                if (deleteItem != null)
                {
                    deleteItem.IsDelete = true;
                    _context.SaveChanges();
                    _vmMsg.Type = Enums.MessageType.Delete;
                    _vmMsg.Msg = "Challan record has been successfully removed.";
                }

            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Unsuccessful to remove the challan record.";
            }
            return _vmMsg;
        }
    }
}
