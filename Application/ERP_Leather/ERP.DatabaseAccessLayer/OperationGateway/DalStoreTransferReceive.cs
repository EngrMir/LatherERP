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
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using System.Transactions;
using System.Linq;


namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalStoreTransferReceive
    {
        private readonly BLC_DEVEntities _context;

        public DalStoreTransferReceive()
        {
            _context = new BLC_DEVEntities();
        }


        public List<INVStoreTrans> GetIssuedItemForLOV()
        {
            using (_context)
            {
                var Data = (from t in _context.INV_StoreTrans.AsEnumerable()
                            where t.TransactionCategory == "ISU" & t.TransactionType == "STI" & t.RecordStatus == "CNF" & t.TransactionStatus == "TRI"
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
                                //RecordStatus = DalCommon.ReturnRecordStatus(t.RecordStatus)
                            }).ToList();

                return Data;
            }
        }


        public INVStoreTrans GetIssuedItemDetailsAfterLOV(long _TransactionID)
        {
            var model = new INVStoreTrans();


            using (_context)
            {
                var TransactionInfo = (from t in _context.INV_StoreTrans
                                       where t.TransactionID == _TransactionID

                                       select new INVStoreTrans
                                       {
                                           //TransactionID = t.TransactionID,
                                           //TransactionNo = t.TransactionNo,
                                           IssueFrom = t.TransactionFrom,
                                           IssueTo = t.TransactionTo,
                                           //TransactionDate = (Convert.ToDateTime(t.TransactionDate)).ToString("dd'/'MM'/'yyyy"),
                                           //RecordStatus = t.RecordStatus,
                                           //Remark = t.Remarks
                                       }).FirstOrDefault();


                model.TransactionInfo = TransactionInfo;


                var TransactionItemList = GetTransactionItemList(_TransactionID);

                model.TransactionItemList = TransactionItemList;

                return model;
            }
        }


        public List<PRDChemProdReqItem> GetTransactionItemList(long _TransactionID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var Data = (from i in context.INV_StoreTransItem
                            where i.TransactionID == _TransactionID
                            from it in context.Sys_ChemicalItem.Where(x => x.ItemID == i.ItemID).DefaultIfEmpty()
                            from sup in context.Sys_Supplier.Where(x => x.SupplierID == i.SupplierID).DefaultIfEmpty()
                            from iu in context.Sys_Unit.Where(x => x.UnitID == i.TransactionUnit).DefaultIfEmpty()
                            from issuesize in context.Sys_Size.Where(x => x.SizeID == i.PackSize).DefaultIfEmpty()
                            from issuesizeunit in context.Sys_Unit.Where(x => x.UnitID == i.SizeUnit).DefaultIfEmpty()

                            orderby (it == null ? null : it.ItemName)
                            select new PRDChemProdReqItem
                            {
                                TransItemID = i.TransItemID,
                                ItemID = i.ItemID,
                                ItemName = (it == null ? null : it.ItemName),
                                SupplierID = i.SupplierID,
                                SupplierName = (sup == null ? null : sup.SupplierName),

                                IssuePackSize = i.PackSize,
                                IssuePackSizeName = (issuesize == null ? null : issuesize.SizeName),

                                IssueSizeUnit = i.SizeUnit,
                                IssueSizeUnitName = (issuesizeunit == null ? null : issuesizeunit.UnitName),
                                IssuePackQty = i.PackQty,
                                IssueQty = i.TransactionQty,
                                IssueUnitName = (iu == null ? null : iu.UnitName),

                                PackSize = i.PackSize,
                                PackSizeName = (issuesize == null ? null : issuesize.SizeName),
                                SizeUnit = i.SizeUnit,
                                SizeUnitName = (issuesizeunit == null ? null : issuesizeunit.UnitName),

                                PackQty = i.PackQty,
                                ReceiveQty= i.TransactionQty,
                                ReceiveUnit= i.TransactionUnit,
                                ReceiveUnitName= (iu == null ? null : iu.UnitName)
                               
                            }).ToList();
                return Data;
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

                            INV_StoreTrans objReceive = new INV_StoreTrans();

                            objReceive.TransactionNo = GetTransactionNo;
                            objReceive.TransactionDate = DalCommon.SetDate(model.TransactionDate);
                            objReceive.TransactionCategory = "RCV";
                            objReceive.TransactionType = "STR";
                            objReceive.TransactionFrom = model.IssueFrom;
                            objReceive.TransactionTo = model.IssueTo;
                            objReceive.RefTransactionID = model.RefTransactionID;
                            objReceive.RefTransactionNo = model.RefTransactionNo;

                            objReceive.TransactionStatus = "TRI";


                            objReceive.RecordStatus = "NCF";
                            objReceive.SetBy = userId;
                            objReceive.SetOn = DateTime.Now;

                            _context.INV_StoreTrans.Add(objReceive);
                            _context.SaveChanges();
                            CurrentTransactionID = objReceive.TransactionID;
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

                                    objItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);
                                    objItem.SizeUnit = DalCommon.GetUnitCode(item.SizeUnitName);
                                    objItem.PackQty = item.PackQty;
                                    objItem.TransactionQty = item.ReceiveQty;
                                    objItem.TransactionUnit = DalCommon.GetUnitCode(item.ReceiveUnitName);

                                    if(item.IssuePackSizeName!=null)
                                    {
                                        objItem.RefPackSize = DalCommon.GetSizeCode(item.IssuePackSizeName);
                                        objItem.RefSizeUnit = DalCommon.GetUnitCode(item.IssueSizeUnitName);
                                        objItem.RefPackQty = item.IssuePackQty;
                                        objItem.RefTransactionQty = item.IssueQty;
                                        objItem.RefTransactionUnit = DalCommon.GetUnitCode(item.IssueUnitName);
                                    }
                                    
                                    
                                    //objItem.ItemSource = DalCommon.ReturnItemSource(item.ItemSource);
                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;

                                    _context.INV_StoreTransItem.Add(objItem);
                                    
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
                return CurrentTransactionID;
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

                        CurrentTransaction.RefTransactionID = model.RefTransactionID;
                        CurrentTransaction.RefTransactionNo = model.RefTransactionNo;
                        CurrentTransaction.TransactionFrom = model.IssueFrom;
                        CurrentTransaction.TransactionTo = model.IssueTo;

                        CurrentTransaction.TransactionDate = DalCommon.SetDate(model.TransactionDate);
                        CurrentTransaction.Remarks = model.Remark;
                        CurrentTransaction.ModifiedBy = userId;
                        CurrentTransaction.ModifiedOn = DateTime.Now;
                        //_context.SaveChanges();

                        #endregion

                        #region Update Transaction ItemList
                        if (model.TransactionItemList != null)
                        {
                            foreach (var item in model.TransactionItemList)
                            {

                                var checkTransactionItem = (from i in _context.INV_StoreTransItem
                                                            where i.TransItemID == item.TransItemID
                                                            select i).Any();

                                #region New_Item_Insertion
                                if (!checkTransactionItem)
                                {
                                    INV_StoreTransItem objItem = new INV_StoreTransItem();

                                    objItem.TransactionID = model.TransactionID;

                                    objItem.ItemID = item.ItemID;
                                    objItem.SupplierID = item.SupplierID;

                                    objItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);
                                    objItem.SizeUnit = DalCommon.GetUnitCode(item.SizeUnitName);
                                    objItem.PackQty = item.PackQty;
                                    objItem.TransactionQty = item.ReceiveQty;
                                    objItem.TransactionUnit = DalCommon.GetUnitCode(item.ReceiveUnitName);

                                    if (item.IssuePackSizeName != null)
                                    {
                                        objItem.RefPackSize = DalCommon.GetSizeCode(item.IssuePackSizeName);
                                        objItem.RefSizeUnit = DalCommon.GetUnitCode(item.IssueSizeUnitName);
                                        objItem.RefPackQty = item.IssuePackQty;
                                        objItem.RefTransactionQty = item.IssueQty;
                                        objItem.RefTransactionUnit = DalCommon.GetUnitCode(item.IssueUnitName);
                                    }


                                    //objItem.ItemSource = DalCommon.ReturnItemSource(item.ItemSource);
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

                                    FoundItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);
                                    FoundItem.SizeUnit = DalCommon.GetUnitCode(item.SizeUnitName);
                                    FoundItem.PackQty = item.PackQty;
                                    FoundItem.TransactionQty = item.ReceiveQty;
                                    FoundItem.TransactionUnit = DalCommon.GetUnitCode(item.ReceiveUnitName);

                                    if (item.IssuePackSizeName != null)
                                    {
                                        FoundItem.RefPackSize = DalCommon.GetSizeCode(item.IssuePackSizeName);
                                        FoundItem.RefSizeUnit = DalCommon.GetUnitCode(item.IssueSizeUnitName);
                                        FoundItem.RefPackQty = item.IssuePackQty;
                                        FoundItem.RefTransactionQty = item.IssueQty;
                                        FoundItem.RefTransactionUnit = DalCommon.GetUnitCode(item.IssueUnitName);
                                    }


                                    //objItem.ItemSource = DalCommon.ReturnItemSource(item.ItemSource);
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

        //For Search Grid
        public List<INVStoreTrans> GetTransactionInfoForSearch()
        {
            using (_context)
            {
                var Data = (from t in _context.INV_StoreTrans.AsEnumerable()
                            where t.TransactionCategory == "RCV" & t.TransactionType == "STR"
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
                                           Remark = t.Remarks,
                                           RefTransactionID= Convert.ToInt64(t.RefTransactionID),
                                           RefTransactionNo= t.RefTransactionNo
                                       }).FirstOrDefault();


                model.TransactionInfo = TransactionInfo;


                var TransactionItemList = GetTransactionItemList(_TransactionID);

                model.TransactionItemList = TransactionItemList;

                return model;
            }
        }

        public List<SysSupplier> GetSupplierForChemical()
        {
            using(_context)
            {
                var Data = (from s in _context.Sys_Supplier.AsEnumerable()
                            where s.SupplierCategory == "Chemical"

                            orderby s.SupplierName
                            select new SysSupplier
                            {
                                SupplierID = s.SupplierID,
                                SupplierCode = s.SupplierCode,
                                SupplierName = s.SupplierName
                            }).ToList();

                return Data;
            }
        }

        public ValidationMsg ConfirmIssueToProductionTransaction(long _TransactionID, string _CheckComment, int _UserID)
        {
            ValidationMsg Msg = new ValidationMsg();
            Msg = DalChemicalStock.AddChemicalInChemicalStore(_TransactionID, _CheckComment, _UserID,
                "Store Transfer Receive");

            return Msg;
        }


    }
}
