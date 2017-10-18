using System;
using System.Globalization;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.AppSetupModel;
using System.Transactions;
using System.Linq;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalStoreTransferIssue
    {
        private readonly BLC_DEVEntities _context;

        public DalStoreTransferIssue()
        {
            _context = new BLC_DEVEntities();
        }

        //For LOV in Grid
        public List<PRDChemProdReqItem> GetStoreWiseChemicalItemStock(string _IssueFrom)
        {
            using (_context)
            {
                var StockResult = (from p in _context.INV_ChemStockSupplier
                                   where p.StoreID.ToString()==_IssueFrom
                                   group p by new
                                   {
                                       //p.StoreID,
                                       p.SupplierID,
                                       p.PackSize,
                                       p.ItemID,
                                       p.SizeUnit,
                                       p.UnitID
                                   } into g
                                   select new
                                   {
                                       TransectionID = g.Max(p => p.TransectionID),
                                       //StoreID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.StoreID).FirstOrDefault(),
                                       ItemID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ItemID).FirstOrDefault(),
                                       SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                                       //UnitID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault(),
                                       PackSize = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.PackSize).FirstOrDefault(),
                                       SizeUnit = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SizeUnit).FirstOrDefault(),
                                       PackQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.PackClosingQty).FirstOrDefault(),
                                       ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault(),
                                       StockUnit = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault()

                                   }).ToList();



                var Data = (from s in StockResult
                            where s.ClosingQty > 0

                            join i in _context.Sys_ChemicalItem on (s == null ? null : s.ItemID) equals i.ItemID into Items
                            from i in Items.DefaultIfEmpty()


                            join sup in _context.Sys_Supplier on (s == null ? 0 : s.SupplierID) equals sup.SupplierID into Suppliers
                            from sup in Suppliers.DefaultIfEmpty()

                            join si in _context.Sys_Size on (s == null ? 0 : s.PackSize) equals si.SizeID into Sizes
                            from si in Sizes.DefaultIfEmpty()

                            join siu in _context.Sys_Unit on (s == null ? 0 : s.SizeUnit) equals siu.UnitID into Units
                            from siu in Units.DefaultIfEmpty()

                            join su in _context.Sys_Unit on (s == null ? 0 : s.StockUnit) equals su.UnitID into StockUnits
                            from su in StockUnits.DefaultIfEmpty()

                            orderby (i == null ? null : i.ItemName)

                            select new PRDChemProdReqItem
                            {
                                ItemID = s.ItemID,
                                ItemName = (i == null ? null : i.ItemName),

                                SupplierID = s.SupplierID,
                                SupplierName = (sup == null ? null : sup.SupplierName),

                                PackSize = s.PackSize,
                                PackSizeName = (si == null ? null : si.SizeName),

                                SizeUnit = s.SizeUnit,
                                SizeUnitName = (siu == null ? null : siu.UnitName),

                                StockQty = s.ClosingQty,
                                StockUnit = s.StockUnit,
                                StockUnitName = (su == null ? null : su.UnitName),

                                PackQty = (Int32)(s.PackQty)

                            }).ToList();

                return Data;
            }

        }


        //after Save For Grid Data
        public List<PRDChemProdReqItem> GetTransactionItemList(long _TransactionID, string _IssueFrom)
        {
            using (var context = new BLC_DEVEntities())
            {
                var StockResult = (from p in context.INV_ChemStockSupplier
                                   where p.StoreID.ToString()==_IssueFrom
                                   group p by new
                                   {
                                       //p.StoreID,
                                       p.SupplierID,
                                       p.PackSize,
                                       p.ItemID,
                                       p.SizeUnit,
                                       p.UnitID
                                   } into g
                                   select new
                                   {
                                       TransectionID = g.Max(p => p.TransectionID),
                                       //StoreID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.StoreID).FirstOrDefault(),
                                       ItemID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ItemID).FirstOrDefault(),
                                       SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                                       //UnitID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault(),
                                       PackSize = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.PackSize).FirstOrDefault(),
                                       SizeUnit = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SizeUnit).FirstOrDefault(),
                                       PackQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.PackClosingQty).FirstOrDefault(),
                                       ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault(),
                                       StockUnit = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault()
                                   });

                var Data = (from i in context.INV_StoreTransItem
                            where i.TransactionID == _TransactionID

                            // Stock Size
                            from s in StockResult.Where(x => x.ItemID == i.ItemID && x.PackSize == i.PackSize && x.SizeUnit == i.SizeUnit && x.SupplierID == i.SupplierID).DefaultIfEmpty()

                            
                            join si in context.Sys_Size on (s == null ? null : s.PackSize) equals si.SizeID into StockSize
                            from si in StockSize.DefaultIfEmpty()

                            join siu in context.Sys_Unit on (s == null ? null : s.SizeUnit) equals siu.UnitID into StockSizeUnit
                            from siu in StockSizeUnit.DefaultIfEmpty()

                            join su in context.Sys_Unit on (s == null ? null : s.StockUnit) equals su.UnitID into StockUnit
                            from su in StockUnit.DefaultIfEmpty()
                            // Stock Size


                            join it in context.Sys_ChemicalItem on (i == null ? null : i.ItemID) equals it.ItemID into Chemicals
                            from it in Chemicals.DefaultIfEmpty()

                            join sup in context.Sys_Supplier on (i == null ? null : i.SupplierID) equals sup.SupplierID into Suppliers
                            from sup in Suppliers.DefaultIfEmpty()

                            join iu in context.Sys_Unit on (i == null ? null : i.TransactionUnit) equals iu.UnitID into IUnits
                            from iu in IUnits.DefaultIfEmpty()

                            join issuesize in context.Sys_Size on (i == null ? null : i.PackSize) equals issuesize.SizeID into IssueSizes
                            from issuesize in IssueSizes.DefaultIfEmpty()

                            join issuesizeunit in context.Sys_Unit on (i == null ? null : i.SizeUnit) equals issuesizeunit.UnitID into IssueSizeUnit
                            from issuesizeunit in IssueSizeUnit.DefaultIfEmpty()

                            orderby (it == null ? null : it.ItemName)
                            
                            select new PRDChemProdReqItem
                            {
                                TransItemID = i.TransItemID,
                                ItemID = i.ItemID,
                                ItemName = (it == null ? null : it.ItemName),
                                SupplierID = i.SupplierID,
                                SupplierName = (sup == null ? null : sup.SupplierName),

                                PackSize = (s == null ? 0 : s.PackSize),
                                PackSizeName = (si == null ? null : si.SizeName),
                                SizeUnit = (s == null ? 0 : s.SizeUnit),
                                SizeUnitName = (siu == null ? null : siu.UnitName),
                                PackQty = (s == null ? 0 : (Int32)(s.PackQty)),

                                StockQty = (s == null ? 0 : s.ClosingQty),
                                StockUnit = (s == null ? 0 : s.StockUnit),
                                StockUnitName = (su == null ? null : su.UnitName),

                                IssuePackSize = i.PackSize,
                                IssuePackSizeName = (issuesize == null ? null : issuesize.SizeName),
                                IssueSizeUnit = i.SizeUnit,
                                IssueSizeUnitName = (issuesizeunit == null ? null : issuesizeunit.UnitName),
                                IssuePackQty = i.PackQty,
                                IssueQty = i.TransactionQty,
                                IssueUnitName = (iu == null ? null : iu.UnitName)
                            }).ToList();
                return Data;
            }
        }


        //For Search Grid
        public List<INVStoreTrans> GetTransactionInfoForSearch()
        {
            using (_context)
            {
                var Data = (from t in _context.INV_StoreTrans.AsEnumerable()
                            where t.TransactionCategory == "ISU" & t.TransactionType == "STI"
                            from tf in _context.SYS_Store.Where(x => (x.StoreID).ToString() == t.TransactionFrom).DefaultIfEmpty()
                            from tt in _context.SYS_Store.Where(x => (x.StoreID).ToString() == t.TransactionTo).DefaultIfEmpty()

                            orderby t.TransactionID descending

                            select new INVStoreTrans
                            {
                                TransactionID = t.TransactionID,
                                TransactionNo = t.TransactionNo,
                                TransactionDate = (Convert.ToDateTime(t.TransactionDate)).ToString("dd'/'MM'/'yyyy"),
                                TransactionFrom = (tf == null ? null : tf.StoreName),
                                TransactionTo = (tt == null ? null : tt.StoreName),
                                RecordStatus = DalCommon.ReturnRecordStatus(t.RecordStatus)
                            }).ToList();

                return Data;
            }
        }

        public INVStoreTrans GetTransactionDetailsAfterSearch(long _TransactionID)
        {
            var model = new INVStoreTrans();


            using (_context)
            {
                var TransactionInfo = (from t in _context.INV_StoreTrans.AsEnumerable()
                                       where t.TransactionID == _TransactionID

                                       select new INVStoreTrans
                                       {
                                           TransactionID = t.TransactionID,
                                           TransactionNo = t.TransactionNo,
                                           IssueFrom = t.TransactionFrom,
                                           IssueTo = t.TransactionTo,
                                           TransactionDate = (Convert.ToDateTime(t.TransactionDate)).ToString("dd'/'MM'/'yyyy"),
                                           RecordStatus = t.RecordStatus,
                                           Remark = t.Remarks
                                       }).FirstOrDefault();


                model.TransactionInfo = TransactionInfo;


                var TransactionItemList = GetTransactionItemList(_TransactionID, TransactionInfo.IssueFrom);

                model.TransactionItemList = TransactionItemList;

                return model;
            }
        }

        public long Save(INVStoreTrans model, int userId, string pageUrl)
        {
            long CurrentTransactionID = 0;
            
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {

                    using (_context)
                    {
                        var GetTransactionNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);

                        if (GetTransactionNo != null)
                        {
                            #region New_Transaction_Insert

                            INV_StoreTrans objIssue = new INV_StoreTrans();

                            objIssue.TransactionNo = GetTransactionNo;
                            objIssue.TransactionDate = DalCommon.SetDate(model.TransactionDate);
                            objIssue.TransactionCategory = "ISU";
                            objIssue.TransactionType = "STI";
                            objIssue.TransactionFrom = model.IssueFrom;
                            objIssue.TransactionTo = model.IssueTo;

                            objIssue.TransactionStatus = "TRI";


                            objIssue.RecordStatus = "NCF";
                            objIssue.SetBy = userId;
                            objIssue.SetOn = DateTime.Now;

                            _context.INV_StoreTrans.Add(objIssue);
                            _context.SaveChanges();
                            CurrentTransactionID = objIssue.TransactionID;
                            #endregion

                            #region Item Insert
                            if (model.TransactionItemList != null)
                            {
                                foreach (var item in model.TransactionItemList)
                                {
                                    INV_StoreTransItem objItem = new INV_StoreTransItem();

                                    objItem.TransactionID = CurrentTransactionID;
                                    
                                    objItem.ItemID = item.ItemID;
                                    objItem.SupplierID = item.SupplierID;

                                    objItem.TransactionQty = item.IssueQty;

                                    objItem.TransactionUnit = DalCommon.GetUnitCode(item.IssueUnitName);


                                    objItem.PackSize = DalCommon.GetSizeCode(item.IssuePackSizeName);
                                    objItem.SizeUnit = DalCommon.GetUnitCode(item.IssueSizeUnitName);


                                    objItem.PackQty = item.IssuePackQty;
                                    //objItem.ItemSource = DalCommon.ReturnItemSource(item.ItemSource);
                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;

                                    _context.INV_StoreTransItem.Add(objItem);
                                    //_context.SaveChanges();
                                }

                            }
                            #endregion
                        }
                        _context.SaveChanges();

                    }
                    transaction.Complete();
                }
                return CurrentTransactionID;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public string GetTransactionNo(long _TransactionID)
        {
            using (_context)
            {
                var TransactionNo = (from p in _context.INV_StoreTrans
                                     where p.TransactionID == _TransactionID
                                     select p.TransactionNo).FirstOrDefault();

                return TransactionNo;
            }

        }

        public int Update(INVStoreTrans model, int userId)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (_context)
                    {

                        #region Transaction_Informaiton_Update
                        var CurrentTransaction = (from p in _context.INV_StoreTrans
                                                  where p.TransactionID == model.TransactionID
                                                  select p).FirstOrDefault();


                        CurrentTransaction.TransactionDate = DalCommon.SetDate(model.TransactionDate);
                        CurrentTransaction.Remarks = model.Remark;
                        CurrentTransaction.ModifiedBy = userId;
                        CurrentTransaction.ModifiedOn = DateTime.Now;
                        //_context.SaveChanges();

                        #endregion

                        #region Update Requisition ItemList
                        if (model.TransactionItemList != null)
                        {
                            foreach (var item in model.TransactionItemList)
                            {

                                var checkRequisitionItem = (from i in _context.INV_StoreTransItem
                                                            where i.TransItemID == item.TransItemID
                                                            select i).Any();

                                #region New_Item_Insertion
                                if (!checkRequisitionItem)
                                {
                                    INV_StoreTransItem objItem = new INV_StoreTransItem();

                                    objItem.TransactionID = model.TransactionID;
                                    objItem.ItemID = item.ItemID;
                                    objItem.SupplierID = item.SupplierID;
                                    objItem.TransactionQty = item.IssueQty;
                                    objItem.TransactionUnit = DalCommon.GetUnitCode(item.IssueUnitName);
                                    objItem.PackSize = DalCommon.GetSizeCode(item.IssuePackSizeName);
                                    objItem.SizeUnit = DalCommon.GetUnitCode(item.IssueSizeUnitName);
                                    objItem.PackQty = item.IssuePackQty;
                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;
                                    _context.INV_StoreTransItem.Add(objItem);
                                    //_context.SaveChanges();
                                }
                                #endregion

                                #region Existing_Item_Update
                                else
                                {
                                    var FoundItem = (from i in _context.INV_StoreTransItem
                                                     where i.TransItemID == item.TransItemID
                                                     select i).FirstOrDefault();
                                    FoundItem.ItemID = item.ItemID;
                                    FoundItem.SupplierID = item.SupplierID;
                                    FoundItem.TransactionQty = item.IssueQty;
                                    FoundItem.TransactionUnit = DalCommon.GetUnitCode(item.IssueUnitName);
                                    FoundItem.PackSize = DalCommon.GetSizeCode(item.IssuePackSizeName);
                                    FoundItem.SizeUnit = DalCommon.GetUnitCode(item.IssueSizeUnitName);
                                    FoundItem.PackQty = item.IssuePackQty;
                                    FoundItem.ModifiedOn = DateTime.Now;
                                    FoundItem.ModifiedBy = userId;
                                    //_context.SaveChanges();

                                }
                                #endregion
                            }
                        }

                        #endregion

                        _context.SaveChanges();
                    }

                    transaction.Complete();


                }
                return 1;
            }
            catch (Exception e)
            {
                return 0;
            }

        }

        public bool DeleteTransactionItem(string _TransItemID)
        {
            try
            {
                var TransactionItem = (from c in _context.INV_StoreTransItem
                                       where (c.TransItemID).ToString() == _TransItemID
                                       select c).FirstOrDefault();

                _context.INV_StoreTransItem.Remove(TransactionItem);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteTransaction(string _TransactionID)
        {
            try
            {
                var Transaction = (from c in _context.INV_StoreTrans
                                   where (c.TransactionID).ToString() == _TransactionID
                                   select c).FirstOrDefault();

                _context.INV_StoreTrans.Remove(Transaction);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ValidationMsg ConfirmIssueToProductionTransaction(long _TransactionID, string _CheckComment, int _UserID)
        {
            ValidationMsg Msg = new ValidationMsg();

            Msg = DalChemicalStock.SubtractChemicalFromChemicalStore(_TransactionID, _CheckComment, _UserID,
                "Store Transfer Issue");

            return Msg;
        }

    }
}
