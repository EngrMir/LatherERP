using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalInvLeatherIssueItem
    {
        private readonly BLC_DEVEntities _context;
        public List<SysSupplier> SupplierList = new List<SysSupplier>();
        public DalInvLeatherIssueItem()
        {
            _context = new BLC_DEVEntities();
        }

        public List<SysSupplier> GetSupplierList(int storeID)
        {
            var SupplierIDList = _context.Inv_StockSupplier.Where(m => m.StoreID == storeID).Select(m => m.SupplierID.ToString()).Distinct().ToList();

            foreach (var supplierID in SupplierIDList)
            {
                int supid = string.IsNullOrEmpty(supplierID) ? 0 : Convert.ToInt16(supplierID);
                var supplier = SetToBussinessObject(_context.Sys_Supplier.Where(m => m.SupplierID == supid).SingleOrDefault());
                SupplierList.Add(supplier);
            }
            return SupplierList;
        }

        public SysSupplier SetToBussinessObject(Sys_Supplier Entity)
        {
            SysSupplier Model = new SysSupplier();

            Model.SupplierID = Entity.SupplierID;
            Model.SupplierCode = Entity.SupplierCode;
            Model.SupplierName = Entity.SupplierName;

            return Model;
        }

        //public List<InvStockSupplier> GetLeatherInfoList(byte storeid, int supplierId)
        //{
        //    var queryString = "select SupplierID,StoreID,ItemTypeID,LeatherType,LeatherStatusID,UnitID,IssueQty,ClosingQty from dbo.Inv_StockSupplier where TransectionID in " +
        //                    "(select tt.TransectionID from (select dbo.Inv_StockSupplier.ItemTypeID,dbo.Inv_StockSupplier.LeatherType,dbo.Inv_StockSupplier.LeatherStatusID, MAX(dbo.Inv_StockSupplier.TransectionID) as TransectionID from dbo.Inv_StockSupplier " +
        //                    "where dbo.Inv_StockSupplier.StoreID =" + storeid + " and dbo.Inv_StockSupplier.SupplierID = " + supplierId + " " +
        //                    "group by  dbo.Inv_StockSupplier.ItemTypeID,dbo.Inv_StockSupplier.LeatherType,dbo.Inv_StockSupplier.LeatherStatusID) as tt)";
        //    var iChallanList = _context.Database.SqlQuery<InvStockSupplier>(queryString);
        //    return iChallanList.Select(c => SetToBussinessObject(c)).ToList<InvStockSupplier>();
        //}

        public List<PrdYearMonthSchedulePurchase> GetLeatherInfoList(string ConcernStore, string SupplierID)
        {
            if (!string.IsNullOrEmpty(ConcernStore) && !string.IsNullOrEmpty(SupplierID))
            {
                var query = "select inv.TransectionID," +
                    //" inv.SupplierID,(select SupplierName from dbo.Sys_Supplier where SupplierID = inv.SupplierID)SupplierName," +
                            " inv.PurchaseID,(select PurchaseNo from dbo.Prq_Purchase where PurchaseID = inv.PurchaseID)PurchaseNo," +
                            " CONVERT(VARCHAR(10), (select PurchaseDate from dbo.Prq_Purchase where PurchaseID = inv.PurchaseID), 103) PurchaseDate," +
                            " inv.StoreID,(select StoreName from dbo.SYS_Store where StoreID = inv.StoreID)StoreName," +
                            " inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName," +
                            " inv.LeatherType LeatherTypeID,(select LeatherTypeName from dbo.Sys_LeatherType where LeatherTypeID = inv.LeatherType)LeatherTypeName," +
                            " inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName," +
                            " inv.UnitID,(select UnitName from dbo.Sys_Unit where UnitID = inv.UnitID)UnitName," +
                            " inv.ClosingQty from dbo.Inv_StockSupplier inv " +
                            " INNER JOIN (select MAX(TransectionID)TransectionID,SupplierID,StoreID,ItemTypeID,LeatherType,LeatherStatusID,PurchaseID from dbo.Inv_StockSupplier" +
                            " group by SupplierID,StoreID,ItemTypeID,LeatherType,LeatherStatusID,PurchaseID) sup" +
                            " ON inv.TransectionID=sup.TransectionID" +
                            " where inv.StoreID = " + ConcernStore + " and inv.SupplierID = " + SupplierID +
                            " and inv.ClosingQty>0";
                var allData = _context.Database.SqlQuery<PrdYearMonthSchedulePurchase>(query).ToList();
                return allData;
            }
            else
                return null;
        }

        public InvStockSupplier SetToBussinessObject(InvStockSupplier Entity)
        {
            InvStockSupplier Model = new InvStockSupplier();

            Model.TransectionID = Entity.TransectionID;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherType = Entity.LeatherType;
            Model.LeatherTypeName = _context.Sys_LeatherType.Where(m => m.LeatherTypeID == Entity.LeatherType).FirstOrDefault().LeatherTypeName;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            Model.RefChallanID = Entity.RefChallanID;
            Model.IssueQty = Entity.IssueQty;
            Model.StockQty = Entity.ClosingQty;
            Model.UnitID = Entity.UnitID;
            Model.UnitName = _context.Sys_Unit.Where(m => m.UnitID == Entity.UnitID).FirstOrDefault().UnitName;

            return Model;
        }

        public List<InvLeatherIssueItem> GetLeatherIssueItemList(long issueid, byte storeid)
        {
            List<Inv_LeatherIssueItem> searchList = _context.Inv_LeatherIssueItem.Where(m => m.IssueID == issueid).OrderByDescending(m => m.ItemIssueID).ToList();
            return searchList.Select(c => SetToBussinessObject(c, storeid)).ToList<InvLeatherIssueItem>();
        }

        public InvLeatherIssueItem SetToBussinessObject(Inv_LeatherIssueItem Entity, byte storeid)
        {
            InvLeatherIssueItem Model = new InvLeatherIssueItem();

            Model.ItemIssueID = Entity.ItemIssueID;
            Model.SupplierID = Entity.SupplierID;
            Model.SupplierName = Entity.SupplierID == null ? "" : _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).SingleOrDefault().SupplierName;
            Model.ChallanID = Entity.ChallanID;
            Model.ChallanNo = Entity.ChallanID == null ? "" : _context.Prq_PurchaseChallan.Where(m => m.ChallanID == Entity.ChallanID).FirstOrDefault().ChallanNo;
            Model.PurchaseID = Entity.PurchaseID;
            Model.PurchaseNo = Entity.PurchaseID == null ? "" : _context.Prq_Purchase.Where(m => m.PurchaseID == Entity.PurchaseID).FirstOrDefault().PurchaseNo;
            Model.ItemType = Entity.ItemType;
            Model.ItemTypeName = Entity.ItemType == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemType).FirstOrDefault().ItemTypeName;
            Model.LeatherType = Entity.LeatherType;
            Model.LeatherTypeName = Entity.LeatherType == null ? "" : _context.Sys_LeatherType.Where(m => m.LeatherTypeID == Entity.LeatherType).FirstOrDefault().LeatherTypeName;
            Model.LeatherStatus = Entity.LeatherStatus;
            Model.LeatherStatusName = Entity.LeatherStatus == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatus).FirstOrDefault().LeatherStatusName;
            Model.IssueQty = Entity.IssueQty;
            if (_context.Inv_StockSupplier.Where(m => m.SupplierID == Entity.SupplierID && m.StoreID == storeid && m.ItemTypeID == Entity.ItemType && m.LeatherType == Entity.LeatherType && m.LeatherStatusID == Entity.LeatherStatus && m.PurchaseID == Entity.PurchaseID).OrderByDescending(m => m.TransectionID).FirstOrDefault() != null)
            {
                Model.StockQty = _context.Inv_StockSupplier.Where(m => m.SupplierID == Entity.SupplierID && m.StoreID == storeid && m.ItemTypeID == Entity.ItemType && m.LeatherType == Entity.LeatherType && m.LeatherStatusID == Entity.LeatherStatus && m.PurchaseID == Entity.PurchaseID).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingQty;
            }
            Model.UnitID = Entity.UnitID;
            Model.UnitName = _context.Sys_Unit.Where(m => m.UnitID == Entity.UnitID).FirstOrDefault().UnitName;
            Model.IssueSide = Entity.IssueSide;
            Model.Remarks = Entity.Remarks;

            return Model;
        }
    }
}
