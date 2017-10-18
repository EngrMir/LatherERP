using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqPurchaseChallanItem
    {
        private readonly BLC_DEVEntities _context;

        public DalPrqPurchaseChallanItem()
        {
            _context = new BLC_DEVEntities();
        }

        public List<Prq_PurchaseChallanItem> GetPrqPurchaseChallanItemsById(string challanId)
        {
            var query = "SELECT * FROM [dbo].[Prq_PurchaseChallanItem] WHERE ChallanID = '" + challanId + "'";
            var purchaseChallanItemList = _context.Database.SqlQuery<Prq_PurchaseChallanItem>(query);
            return purchaseChallanItemList.ToList();
        }

        public string GetItemName(int itemTypeID)
        {
            return _context.Sys_ItemType.Where(m => m.ItemTypeID == itemTypeID).SingleOrDefault().ItemTypeName;
        }

        public string GetSizeName(int itemTypeID)
        {
            return _context.Sys_Size.Where(m => m.SizeID == itemTypeID).SingleOrDefault().SizeName;
        }

        public string GetUnitName(int unitID)
        {
            return _context.Sys_Unit.Where(u => u.UnitID == unitID).SingleOrDefault().UnitName;
        }
    }
}
