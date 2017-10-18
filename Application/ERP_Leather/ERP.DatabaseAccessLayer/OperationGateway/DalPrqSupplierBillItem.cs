using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqSupplierBillItem
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public DalPrqSupplierBillItem()
        {
            _context = new BLC_DEVEntities();
            _vmMsg = new ValidationMsg();
        }

        public List<PrqSupplierBillItem> GetPrqSupplierBillItems(long supplierBillId)
        {
            List<Prq_SupplierBillItem> bills =
                _context.Prq_SupplierBillItem.Where(w => w.SupplierBillID == supplierBillId).ToList();

            return bills.Select(SetToBussinessObject).ToList();
        }

        public PrqSupplierBillItem SetToBussinessObject(Prq_SupplierBillItem entity)
        {
            var model = new PrqSupplierBillItem();

            model.SupplierBillID = entity.SupplierBillID;
            model.BillItemID = entity.BillItemID;
            model.ItemTypeID = entity.ItemType;
            model.ItemTypeName =
                _context.Sys_ItemType.Where(w => w.ItemTypeID == entity.ItemType).FirstOrDefault().ItemTypeName;
            model.ItemSizeID = entity.ItemSize;
            model.ItemSizeName = _context.Sys_Size.Where(w => w.SizeID == entity.ItemSize).FirstOrDefault().SizeName;
            model.ItemQty = entity.ItemQty;
            model.RejectQty = entity.RejectQty;
            model.ItemRate = entity.ItemRate;
            model.ApproveRate = entity.ApproveRate;
            model.Amount = entity.Amount;
            model.AmtUnit = entity.AmtUnit ?? 0 ;
            if (entity.AmtUnit != 0)
            {
                model.AmtUnitName = _context.Sys_Currency.Where(w => w.CurrencyID == entity.AmtUnit).FirstOrDefault().CurrencyName;
            }
            model.AvgArea = entity.AvgArea;
            model.AreaUnit = entity.AreaUnit ?? 0;
            if (entity.AreaUnit != 0)
            {
                model.AreaUnitName = _context.Sys_Unit.Where(w => w.UnitID == entity.AreaUnit).FirstOrDefault().UnitName;
            }
            model.Remarks = entity.Remarks;

            return model;
        }

        public List<PrqSupplierBillItem> GetItemListForChallan(string challanID)
        {
            using (var Context = new BLC_DEVEntities())
            {
                var ItemList = (from c in Context.Prq_PurchaseChallanItem.AsEnumerable()
                                where (c.ChallanID).ToString(CultureInfo.InvariantCulture) == challanID
                                //select new PrqPurchaseChallanItem
                                select new PrqSupplierBillItem
                                {
                                    //ChallanItemID = c.ChallanItemID,
                                    //ChallanID = c.ChallanID,
                                    ItemTypeID = c.ItemTypeID,
                                    ItemTypeName = _context.Sys_ItemType.Where(m => m.ItemTypeID == c.ItemTypeID).FirstOrDefault().ItemTypeName,
                                    ItemSizeID = c.ItemSizeID,
                                    ItemSizeName = _context.Sys_Size.Where(m => m.SizeID == c.ItemSizeID).FirstOrDefault().SizeName,
                                    AreaUnit = c.UnitID,
                                    AreaUnitName = _context.Sys_Unit.Where(m => m.UnitID == c.UnitID).FirstOrDefault().UnitName,
                                    //Description = c.Description,
                                    ItemQty = c.ChallanQty
                                    //,
                                    //ReceiveQty = c.ReceiveQty,
                                    //Remark = c.Remark
                                });

                return ItemList.ToList();
            }
        }

        public ValidationMsg DeleteBillItem(long itemId)
        {
            try
            {
                Prq_SupplierBillItem deleteSupplierBillItem =
                    _context.Prq_SupplierBillItem.SingleOrDefault(w => w.BillItemID == itemId);
                _context.Prq_SupplierBillItem.Remove(deleteSupplierBillItem);
                _context.SaveChanges();
                _vmMsg.Type = Enums.MessageType.Delete;
                _vmMsg.Msg = "Bill item is deleted successfully ";
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to delete bill item.";
            }
            return _vmMsg;
        }
    }
}
