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
using System.Transactions;
using System.Linq;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public static class DalChemicalStock
    {
        public static List<ChemicalStockInfoModel> ItemWiseStockInAllChemicalStore()
        {
            using(var context= new BLC_DEVEntities() )
            {
                var StoreIDS = (from s in context.SYS_Store
                                where s.StoreCategory == "Chemical" & s.StoreType == "Chemical"
                                select s.StoreID).ToList();

                var StockResult = (from p in context.INV_ChemStockSupplier
                                   where StoreIDS.Contains(p.StoreID)
                                   group p by new
                                   {
                                       p.StoreID,
                                       p.SupplierID,
                                       p.PackSize,
                                       p.ItemID,
                                       p.SizeUnit,
                                       p.UnitID
                                   } into g
                                   select new
                                   {
                                       TransectionID = g.Max(p => p.TransectionID),
                                       StoreID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.StoreID).FirstOrDefault(),
                                       ItemID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ItemID).FirstOrDefault(),
                                       SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                                       ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault(),
                                       StockUnit = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault()
                                   }).ToList();

                var FinalStock = (from s in StockResult
                                  group s by new
                                  {
                                      s.ItemID
                                  } into g
                                  select new ChemicalStockInfoModel
                                  {
                                      ItemID = g.Select(p => p.ItemID).FirstOrDefault(),
                                      ClosingQty = g.Sum(x => x.ClosingQty),
                                      SupplierID = g.Select(p => p.SupplierID).FirstOrDefault(),
                                  }).ToList();

                return FinalStock;
            }
            
        }

        public static List<ChemicalStockInfoModel> ItemWiseStockInSpecificChemicalStore(byte _StoreID)
        {
            using(var context= new BLC_DEVEntities())
            {
                var StockResultProductionFloor = (from p in context.INV_ChemStockSupplier
                                                  where p.StoreID == _StoreID
                                                  group p by new
                                                  {
                                                      p.SupplierID,
                                                      p.ItemID,
                                                      p.PackSize,
                                                      p.SizeUnit,
                                                      p.UnitID
                                                  } into g
                                                  select new
                                                  {
                                                      TransectionID = g.Max(p => p.TransectionID),
                                                      StoreID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.StoreID).FirstOrDefault(),
                                                      ItemID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ItemID).FirstOrDefault(),
                                                      SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                                                      ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault(),
                                                      UnitID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault()
                                                  }).ToList();

                var FinalStock = (from s in StockResultProductionFloor
                                                 group s by new
                                                 {
                                                     s.ItemID
                                                 } into g
                                                 select new ChemicalStockInfoModel
                                                 {
                                                     ItemID = g.Select(p => p.ItemID).FirstOrDefault(),
                                                     ClosingQty = g.Sum(x => x.ClosingQty),
                                                     SupplierID = g.Select(p => p.SupplierID).FirstOrDefault(),
                                                     UnitID = g.Select(x => x.UnitID).FirstOrDefault(),
                                                 }).ToList();

                return FinalStock;
            }
        }

        public static List<ChemicalStockInfoModel> ItemWiseStockInSpecificProductionFloor(byte _StoreID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var StockResultProductionFloor = (from p in context.INV_ChemStockSupplier
                                                  where p.StoreID == _StoreID
                                                  group p by new
                                                  {
                                                      p.SupplierID,
                                                      p.ItemID,
                                                      p.UnitID
                                                  } into g
                                                  select new
                                                  {
                                                      TransectionID = g.Max(p => p.TransectionID),
                                                      StoreID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.StoreID).FirstOrDefault(),
                                                      ItemID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ItemID).FirstOrDefault(),
                                                      SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                                                      ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault(),
                                                      StockUnit = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault()
                                                  }).ToList();

                var FinalStock = (from s in StockResultProductionFloor
                                  group s by new
                                  {
                                      s.ItemID
                                  } into g
                                  select new ChemicalStockInfoModel
                                  {
                                      ItemID = g.Select(p => p.ItemID).FirstOrDefault(),
                                      ClosingQty = g.Sum(x => x.ClosingQty),
                                      SupplierID = g.Select(p => p.SupplierID).FirstOrDefault(),
                                  }).ToList();

                return FinalStock;
            }
        }

        public static List<ChemicalStockInfoModel> CombinationWiseStockInSpecificProductionFloor(byte _StoreID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var StockResultProductionFloor = (from p in context.INV_ChemStockSupplier
                                                  where p.StoreID == _StoreID
                                                  group p by new
                                                  {
                                                      p.SupplierID,
                                                      p.ItemID,
                                                      p.UnitID
                                                  } into g
                                                  select new ChemicalStockInfoModel
                                                  {
                                                      TransectionID = g.Max(p => p.TransectionID),
                                                      StoreID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.StoreID).FirstOrDefault(),
                                                      ItemID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ItemID).FirstOrDefault(),
                                                      SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                                                      ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault(),
                                                      UnitID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault()
                                                  }).ToList();



                return StockResultProductionFloor;
            }
        }

        public static List<ChemicalStockInfoModel> CombinationWiseStockInSpecificChemicalStore(byte _StoreID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var stockResultInChemicalStore = (from p in context.INV_ChemStockSupplier
                                                  where p.StoreID == _StoreID
                                                  group p by new
                                                  {
                                                      p.SupplierID,
                                                      p.ItemID,
                                                      p.PackSize,
                                                      p.SizeUnit,
                                                      p.UnitID
                                                  } into g
                                                  select new ChemicalStockInfoModel
                                                  {
                                                      TransectionID = g.Max(p => p.TransectionID),
                                                      StoreID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.StoreID).FirstOrDefault(),
                                                      ItemID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ItemID).FirstOrDefault(),
                                                      SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                                                      ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault(),
                                                      UnitID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault()
                                                  }).ToList();



                return stockResultInChemicalStore;
            }
        }

        public static ValidationMsg SubtractChemicalFromChemicalStore(long _TransactionID, string _CheckComment, int _UserID, string _remark)
        {
            ValidationMsg Msg = new ValidationMsg();
            Msg.ReturnId = 1;
            Msg.Msg = "Confirmation Successful";
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (var context = new BLC_DEVEntities())
                    {
                        var TransactionInfo = (from p in context.INV_StoreTrans
                                               where p.TransactionID == _TransactionID
                                               select p).FirstOrDefault();

                        var TransactionItemList = (from i in context.INV_StoreTransItem
                                                   where i.TransactionID == _TransactionID
                                                   select new INVStoreTransItem
                                                   {
                                                       ItemID = i.ItemID,
                                                       PackSize = i.PackSize,
                                                       SizeUnit = i.SizeUnit,
                                                       SupplierID = i.SupplierID,
                                                       TransactionID = i.TransactionID,
                                                       PackQty = i.PackQty,
                                                       TransactionQty = i.TransactionQty,
                                                       TransactionUnit = i.TransactionUnit,
                                                   }).ToList();

                        foreach (var aTransaction in TransactionItemList)
                        {
                            #region Chemical Supplier Stock Update

                            var FoundItemSupplier = (from i in context.INV_ChemStockSupplier
                                                     where i.SupplierID == aTransaction.SupplierID && (i.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                                     i.ItemID == aTransaction.ItemID && i.PackSize == aTransaction.PackSize && i.SizeUnit == aTransaction.SizeUnit &&
                                                     i.UnitID == aTransaction.TransactionUnit
                                                     orderby i.TransectionID descending
                                                     select i).FirstOrDefault();

                            if (FoundItemSupplier == null)
                            {
                                Msg.Msg = "Item Does Not Exist in Stock";
                                Msg.ReturnId = 0;
                            }
                            else
                            {
                                if (FoundItemSupplier.ClosingQty >= aTransaction.TransactionQty & FoundItemSupplier.PackClosingQty >= aTransaction.PackQty)
                                {
                                    var NewItem = new INV_ChemStockSupplier();

                                    NewItem.SupplierID = Convert.ToInt16(aTransaction.SupplierID);
                                    NewItem.StoreID = Convert.ToByte(TransactionInfo.TransactionFrom);
                                    NewItem.ItemID = aTransaction.ItemID;
                                    NewItem.PackSize = aTransaction.PackSize;
                                    NewItem.SizeUnit = aTransaction.SizeUnit;
                                    NewItem.UnitID = aTransaction.TransactionUnit;
                                    NewItem.OpeningQty = FoundItemSupplier.ClosingQty;
                                    NewItem.ReceiveQty = 0;
                                    NewItem.IssueQty = aTransaction.TransactionQty;
                                    NewItem.ClosingQty = FoundItemSupplier.ClosingQty - aTransaction.TransactionQty;
                                    NewItem.PackOpeningQty = FoundItemSupplier.PackClosingQty;
                                    NewItem.PackIssueQty = aTransaction.PackQty;
                                    NewItem.PackReceiveQty = 0;
                                    NewItem.PackClosingQty = FoundItemSupplier.PackClosingQty - aTransaction.PackQty;
                                    NewItem.SetBy = _UserID;
                                    NewItem.SetOn = DateTime.Now.Date;
                                    NewItem.Remark = _remark;

                                    context.INV_ChemStockSupplier.Add(NewItem);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    Msg.Msg = "Not Enough Quantity in Stock";
                                    Msg.ReturnId = 0;
                                }

                            }
                            #endregion

                            #region Chemical Item Stock Update

                            var FoundItem = (from i in context.INV_ChemStockItem
                                             where (i.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                             i.ItemID == aTransaction.ItemID && i.PackSize == aTransaction.PackSize && i.SizeUnit == aTransaction.SizeUnit && i.UnitID == aTransaction.TransactionUnit
                                             orderby i.TransectionID descending
                                             select i).FirstOrDefault();

                            if (FoundItem == null)
                            {
                                Msg.Msg = "Item Does Not Exist in Stock";
                                Msg.ReturnId = 0;
                            }
                            else
                            {
                                if (FoundItem.ClosingQty >= aTransaction.TransactionQty & FoundItem.PackClosingQty >= aTransaction.PackQty)
                                {
                                    var NewItem = new INV_ChemStockItem();

                                    NewItem.StoreID = Convert.ToByte(TransactionInfo.TransactionFrom);
                                    NewItem.ItemID = aTransaction.ItemID;
                                    NewItem.PackSize = aTransaction.PackSize;
                                    NewItem.SizeUnit = aTransaction.SizeUnit;
                                    NewItem.UnitID = aTransaction.TransactionUnit;
                                    NewItem.OpeningQty = FoundItem.ClosingQty;
                                    NewItem.IssueQty = aTransaction.TransactionQty;
                                    NewItem.ReceiveQty = 0;
                                    NewItem.ClosingQty = FoundItem.ClosingQty - aTransaction.TransactionQty;
                                    NewItem.PackOpeningQty = FoundItem.PackClosingQty;
                                    NewItem.PackIssueQty = aTransaction.PackQty;
                                    NewItem.PackReceiveQty = 0;
                                    NewItem.PackClosingQty = FoundItem.PackClosingQty - aTransaction.PackQty;
                                    NewItem.SetBy = _UserID;
                                    NewItem.SetOn = DateTime.Now.Date;
                                    NewItem.Remark = _remark;

                                    context.INV_ChemStockItem.Add(NewItem);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    Msg.Msg = "Not Enough Quantity in Stock";
                                    Msg.ReturnId = 0;
                                }

                            }

                            #endregion

                            #region Chemical Daily Stock Update

                            var currentDate = DateTime.Now.Date;

                            var CurrentDateItem = (from i in context.INV_ChemStockDaily
                                                   where i.StockDate == currentDate && (i.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                                   i.ItemID == aTransaction.ItemID && i.PackSize == aTransaction.PackSize && i.SizeUnit == aTransaction.SizeUnit &&
                                                   i.UnitID == aTransaction.TransactionUnit
                                                   select i).FirstOrDefault();


                            if (CurrentDateItem != null)
                            {
                                if (CurrentDateItem.ClosingQty >= aTransaction.TransactionQty & CurrentDateItem.PackClosingQty >= aTransaction.PackQty)
                                {
                                    CurrentDateItem.IssueQty = CurrentDateItem.IssueQty + aTransaction.TransactionQty;
                                    CurrentDateItem.ClosingQty = CurrentDateItem.ClosingQty - aTransaction.TransactionQty;

                                    CurrentDateItem.PackIssueQty = CurrentDateItem.PackIssueQty + aTransaction.PackQty;
                                    CurrentDateItem.PackClosingQty = CurrentDateItem.PackClosingQty - aTransaction.PackQty;

                                    CurrentDateItem.ModifiedBy = _UserID;
                                    CurrentDateItem.ModifiedOn = DateTime.Now.Date;
                                    CurrentDateItem.Remark = _remark;
                                    context.SaveChanges();
                                }
                                else
                                {
                                    Msg.Msg = "Not Enough Quantity in Stock";
                                    Msg.ReturnId = 0;
                                }
                            }
                            else
                            {
                                var PreviousDateItem = (from i in context.INV_ChemStockDaily
                                                        where (i.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                                        i.ItemID == aTransaction.ItemID && i.PackSize == aTransaction.PackSize && i.SizeUnit == aTransaction.SizeUnit &&
                                                        i.UnitID == aTransaction.TransactionUnit
                                                        orderby i.TransectionID descending
                                                        select i).FirstOrDefault();
                                if (PreviousDateItem != null)
                                {
                                    if (PreviousDateItem.ClosingQty >= aTransaction.TransactionQty & PreviousDateItem.PackClosingQty >= aTransaction.PackQty)
                                    {
                                        var NewItem = new INV_ChemStockDaily();

                                        NewItem.StockDate = DateTime.Now.Date;
                                        NewItem.StoreID = Convert.ToByte(TransactionInfo.TransactionFrom);
                                        NewItem.ItemID = aTransaction.ItemID;
                                        NewItem.PackSize = aTransaction.PackSize;
                                        NewItem.SizeUnit = aTransaction.SizeUnit;
                                        NewItem.UnitID = aTransaction.TransactionUnit;
                                        NewItem.OpeningQty = PreviousDateItem.ClosingQty;
                                        NewItem.IssueQty = aTransaction.TransactionQty;
                                        NewItem.ReceiveQty = 0;
                                        NewItem.ClosingQty = PreviousDateItem.ClosingQty - aTransaction.TransactionQty;
                                        NewItem.PackOpeningQty = PreviousDateItem.PackClosingQty;
                                        NewItem.PackIssueQty = aTransaction.PackQty;
                                        NewItem.PackReceiveQty = 0;
                                        NewItem.PackClosingQty = PreviousDateItem.PackClosingQty - aTransaction.PackQty;
                                        NewItem.SetOn = DateTime.Now.Date;
                                        NewItem.SetBy = _UserID;
                                        NewItem.Remark = _remark;

                                        context.INV_ChemStockDaily.Add(NewItem);
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        Msg.Msg = "Not Enough Quantity in Stock";
                                        Msg.ReturnId = 0;
                                    }

                                }
                                else
                                {
                                    Msg.Msg = "Item Does Not Exist in Stock";
                                    Msg.ReturnId = 0;
                                }


                            }
                            #endregion

                        }
                        if (Msg.ReturnId == 1)
                        {
                            TransactionInfo.CheckComments = _CheckComment;

                            TransactionInfo.RecordStatus = "CNF";

                            TransactionInfo.TransactionState = "CMP";
                            TransactionInfo.TransactionStatus = "TRO";

                            context.SaveChanges();
                            Transaction.Complete();
                        }
                    }

                }

                return Msg;
            }
            catch (Exception e)
            {
                Msg.ReturnId = 0;
                Msg.Msg = "Confirmation Failed";
                return Msg;
            }
        }

        public static ValidationMsg AddChemicalInChemicalStore(long _TransactionID, string _CheckComment, int _UserID, string _remark)
        {
            ValidationMsg Msg = new ValidationMsg();
            Msg.ReturnId = 1;
            Msg.Msg = "Confirmation Successful";
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (var context = new BLC_DEVEntities())
                    {
                        var TransactionInfo = (from p in context.INV_StoreTrans
                                               where p.TransactionID == _TransactionID
                                               select p).FirstOrDefault();


                        var TransactionItemList = (from i in context.INV_StoreTransItem
                                                   where i.TransactionID == _TransactionID
                                                   select new INVStoreTransItem
                                                   {
                                                       ItemID = i.ItemID,
                                                       PackSize = i.PackSize,
                                                       SizeUnit = i.SizeUnit,
                                                       SupplierID = i.SupplierID,
                                                       TransactionID = i.TransactionID,
                                                       PackQty = i.PackQty,
                                                       TransactionQty = i.TransactionQty,
                                                       TransactionUnit = i.TransactionUnit
                                                   }).ToList();

                        foreach (var item in TransactionItemList)
                        {
                            var currentDate = DateTime.Now.Date;

                            #region Daily_Stock_Update

                            var CheckDate = (from ds in context.INV_ChemStockDaily
                                             where ds.ItemID == item.ItemID && (ds.StoreID).ToString() == TransactionInfo.TransactionTo
                                              && ds.PackSize == item.PackSize && ds.SizeUnit == item.SizeUnit && ds.StockDate == currentDate
                                              && ds.UnitID == item.TransactionUnit
                                             select ds).Any();

                            if (CheckDate)
                            {
                                var CurrentItem = (from ds in context.INV_ChemStockDaily
                                                   where ds.ItemID == item.ItemID && (ds.StoreID).ToString() == TransactionInfo.TransactionTo
                                                    && ds.PackSize == item.PackSize && ds.SizeUnit == item.SizeUnit
                                                    && ds.StockDate == currentDate && ds.UnitID == item.TransactionUnit
                                                   select ds).FirstOrDefault();

                                CurrentItem.ReceiveQty = CurrentItem.ReceiveQty + item.TransactionQty;
                                CurrentItem.IssueQty = 0;
                                CurrentItem.ClosingQty = CurrentItem.ClosingQty + item.TransactionQty;
                                CurrentItem.PackReceiveQty = CurrentItem.PackReceiveQty + item.PackQty;
                                CurrentItem.PackIssueQty = 0;
                                CurrentItem.PackClosingQty = CurrentItem.PackClosingQty + item.PackQty;
                                CurrentItem.ModifiedBy = _UserID;
                                CurrentItem.ModifiedOn = DateTime.Now.Date;
                                CurrentItem.Remark = "Store Transfer Receive";
                                context.SaveChanges();
                            }
                            else
                            {
                                var PreviousRecord = (from ds in context.INV_ChemStockDaily
                                                      where ds.ItemID == item.ItemID && (ds.StoreID).ToString() == TransactionInfo.TransactionTo
                                                       && ds.PackSize == item.PackSize && ds.SizeUnit == item.SizeUnit && ds.UnitID == item.TransactionUnit
                                                      orderby ds.TransectionID descending
                                                      select ds).FirstOrDefault();

                                INV_ChemStockDaily objStockDaily = new INV_ChemStockDaily();

                                objStockDaily.ItemID = item.ItemID;
                                objStockDaily.StoreID = Convert.ToByte(TransactionInfo.TransactionTo);
                                objStockDaily.UnitID = item.TransactionUnit;
                                objStockDaily.PackSize = item.PackSize;
                                objStockDaily.SizeUnit = item.SizeUnit;

                                objStockDaily.OpeningQty = (PreviousRecord == null ? 0 : PreviousRecord.ClosingQty);
                                objStockDaily.ReceiveQty = item.TransactionQty;
                                objStockDaily.IssueQty = 0;
                                objStockDaily.ClosingQty = objStockDaily.OpeningQty + item.TransactionQty;

                                objStockDaily.PackOpeningQty = (PreviousRecord == null ? 0 : PreviousRecord.PackClosingQty);
                                objStockDaily.PackReceiveQty = item.PackQty;
                                objStockDaily.PackIssueQty = 0;
                                objStockDaily.PackClosingQty = objStockDaily.PackOpeningQty + item.PackQty;

                                objStockDaily.StockDate = DateTime.Now.Date;

                                objStockDaily.SetOn = DateTime.Now.Date;
                                objStockDaily.SetBy = _UserID;
                                objStockDaily.Remark = "Store Transfer Receive";

                                context.INV_ChemStockDaily.Add(objStockDaily);
                                context.SaveChanges();

                            }

                            #endregion

                            #region Supplier_Stock_Update

                            var CheckSupplierStockForReceive = (from i in context.INV_ChemStockSupplier
                                                                where i.SupplierID == item.SupplierID && (i.StoreID).ToString() == TransactionInfo.TransactionTo &&
                                                                i.ItemID == item.ItemID && i.PackSize == item.PackSize && i.SizeUnit == item.SizeUnit &&
                                                                i.UnitID == item.TransactionUnit
                                                                select i).Any();

                            if (!CheckSupplierStockForReceive)
                            {
                                INV_ChemStockSupplier objStockSupplier = new INV_ChemStockSupplier();

                                objStockSupplier.SupplierID = Convert.ToInt16(item.SupplierID);
                                objStockSupplier.StoreID = Convert.ToByte(TransactionInfo.TransactionTo);
                                objStockSupplier.ItemID = item.ItemID;
                                objStockSupplier.PackSize = item.PackSize;
                                objStockSupplier.SizeUnit = item.SizeUnit;
                                objStockSupplier.UnitID = item.TransactionUnit;


                                objStockSupplier.OpeningQty = 0;
                                objStockSupplier.ReceiveQty = item.TransactionQty;
                                objStockSupplier.IssueQty = 0;
                                objStockSupplier.ClosingQty = item.TransactionQty;

                                objStockSupplier.PackOpeningQty = 0;
                                objStockSupplier.PackReceiveQty = item.PackQty;
                                objStockSupplier.PackIssueQty = 0;
                                objStockSupplier.PackClosingQty = item.PackQty;

                                objStockSupplier.SetOn = DateTime.Now.Date;
                                objStockSupplier.SetBy = _UserID;
                                objStockSupplier.Remark = "Store Transfer Receive";


                                context.INV_ChemStockSupplier.Add(objStockSupplier);
                                context.SaveChanges();
                            }
                            else
                            {
                                var LastSupplierStock = (from i in context.INV_ChemStockSupplier
                                                         where i.SupplierID == item.SupplierID && (i.StoreID).ToString() == TransactionInfo.TransactionTo &&
                                                         i.ItemID == item.ItemID && i.PackSize == item.PackSize && i.SizeUnit == item.SizeUnit &&
                                                         i.UnitID == item.TransactionUnit
                                                         orderby i.TransectionID descending
                                                         select i).FirstOrDefault();

                                INV_ChemStockSupplier objStockSupplier = new INV_ChemStockSupplier();

                                objStockSupplier.SupplierID = Convert.ToInt16(item.SupplierID);
                                objStockSupplier.StoreID = Convert.ToByte(TransactionInfo.TransactionTo);
                                objStockSupplier.ItemID = item.ItemID;
                                objStockSupplier.PackSize = item.PackSize;
                                objStockSupplier.SizeUnit = item.SizeUnit;
                                objStockSupplier.UnitID = item.TransactionUnit;


                                objStockSupplier.OpeningQty = LastSupplierStock.ClosingQty;
                                objStockSupplier.ReceiveQty = item.TransactionQty;
                                objStockSupplier.IssueQty = 0;
                                objStockSupplier.ClosingQty = LastSupplierStock.ClosingQty + item.TransactionQty;


                                objStockSupplier.PackOpeningQty = LastSupplierStock.PackClosingQty;
                                objStockSupplier.PackReceiveQty = item.PackQty;
                                objStockSupplier.PackIssueQty = 0;
                                objStockSupplier.PackClosingQty = LastSupplierStock.PackClosingQty + item.PackQty;

                                objStockSupplier.SetBy = _UserID;
                                objStockSupplier.SetOn = DateTime.Now.Date;
                                objStockSupplier.Remark = "Store Transfer Receive";

                                context.INV_ChemStockSupplier.Add(objStockSupplier);
                                context.SaveChanges();

                            }




                            #endregion

                            #region Item_Stock_Update

                            var CheckItemStockForReceive = (from i in context.INV_ChemStockItem
                                                            where (i.StoreID).ToString() == TransactionInfo.TransactionTo &&
                                                            i.ItemID == item.ItemID && i.PackSize == item.PackSize && i.SizeUnit == item.SizeUnit &&
                                                            i.UnitID == item.TransactionUnit
                                                            select i).Any();


                            if (!CheckItemStockForReceive)
                            {
                                INV_ChemStockItem objStockItem = new INV_ChemStockItem();

                                objStockItem.StoreID = Convert.ToByte(TransactionInfo.TransactionTo);
                                objStockItem.ItemID = item.ItemID;
                                objStockItem.PackSize = item.PackSize;
                                objStockItem.SizeUnit = item.SizeUnit;
                                objStockItem.UnitID = item.TransactionUnit;


                                objStockItem.OpeningQty = 0;
                                objStockItem.ReceiveQty = item.TransactionQty;
                                objStockItem.IssueQty = 0;
                                objStockItem.ClosingQty = item.TransactionQty;

                                objStockItem.PackOpeningQty = 0;
                                objStockItem.PackReceiveQty = item.PackQty;
                                objStockItem.PackIssueQty = 0;
                                objStockItem.PackClosingQty = item.PackQty;

                                objStockItem.SetOn = DateTime.Now.Date;
                                objStockItem.SetBy = _UserID;
                                objStockItem.Remark = "Store Transfer Receive";


                                context.INV_ChemStockItem.Add(objStockItem);
                                context.SaveChanges();
                            }
                            else
                            {
                                var LastItemInfo = (from i in context.INV_ChemStockItem
                                                    where (i.StoreID).ToString() == TransactionInfo.TransactionTo &&
                                                    i.ItemID == item.ItemID && i.PackSize == item.PackSize && i.SizeUnit == item.SizeUnit &&
                                                    i.UnitID == item.TransactionUnit
                                                    orderby i.TransectionID descending
                                                    select i).FirstOrDefault();

                                INV_ChemStockItem objStockItem = new INV_ChemStockItem();

                                objStockItem.StoreID = Convert.ToByte(TransactionInfo.TransactionTo);
                                objStockItem.ItemID = item.ItemID;
                                objStockItem.PackSize = item.PackSize;
                                objStockItem.SizeUnit = item.SizeUnit;
                                objStockItem.UnitID = item.TransactionUnit;


                                objStockItem.OpeningQty = LastItemInfo.ClosingQty;
                                objStockItem.ReceiveQty = item.TransactionQty;
                                objStockItem.IssueQty = 0;
                                objStockItem.ClosingQty = LastItemInfo.ClosingQty + item.TransactionQty;


                                objStockItem.PackOpeningQty = LastItemInfo.PackClosingQty;
                                objStockItem.PackReceiveQty = item.PackQty;
                                objStockItem.PackIssueQty = 0;
                                objStockItem.PackClosingQty = LastItemInfo.PackClosingQty + item.PackQty;

                                objStockItem.SetOn = DateTime.Now.Date;
                                objStockItem.SetBy = _UserID;
                                objStockItem.Remark = "Store Transfer Receive";

                                context.INV_ChemStockItem.Add(objStockItem);
                                context.SaveChanges();
                            }

                            #endregion

                        }
                        if (Msg.ReturnId == 1)
                        {
                            TransactionInfo.CheckComments = _CheckComment;
                            TransactionInfo.RecordStatus = "CNF";
                            context.SaveChanges();
                            Transaction.Complete();
                        }
                    }

                }

                return Msg;
            }
            catch (Exception e)
            {
                Msg.ReturnId = 0;
                Msg.Msg = "Confirmation Failed";
                return Msg;
            }
        }

        public static ValidationMsg AddChemicalInProductionFloor(long _TransactionID, string _CheckComment, int _UserID, string _remark)
        {
            ValidationMsg Msg = new ValidationMsg();
            Msg.ReturnId = 1;
            Msg.Msg = "Confirmation Successful";
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (var context = new BLC_DEVEntities())
                    {
                        var TransactionInfo = (from p in context.INV_StoreTrans
                                               where p.TransactionID == _TransactionID
                                               select p).FirstOrDefault();


                        var TransactionItemList = (from i in context.INV_StoreTransItem
                                                   where i.TransactionID == _TransactionID
                                                   select new INVStoreTransItem
                                                   {
                                                       ItemID = i.ItemID,
                                                       PackSize = i.PackSize,
                                                       SizeUnit = i.SizeUnit,
                                                       SupplierID = i.SupplierID,
                                                       TransactionID = i.TransactionID,
                                                       PackQty = i.PackQty,
                                                       TransactionQty = i.TransactionQty,
                                                       TransactionUnit = i.TransactionUnit
                                                   }).ToList();

                        foreach (var item in TransactionItemList)
                        {
                            var currentDate = DateTime.Now.Date;

                            #region Daily_Stock_Update

                            var CurrentDateItem2 = (from ds in context.INV_ChemStockDaily
                                                    where ds.ItemID == item.ItemID && (ds.StoreID).ToString() == TransactionInfo.TransactionTo && ds.UnitID == item.TransactionUnit
                                                     && ds.StockDate == currentDate
                                                    select ds).FirstOrDefault();

                            if (CurrentDateItem2 != null)
                            {
                                CurrentDateItem2.ReceiveQty = CurrentDateItem2.ReceiveQty + item.TransactionQty;
                                CurrentDateItem2.ClosingQty = CurrentDateItem2.ClosingQty + item.TransactionQty;
                                CurrentDateItem2.ModifiedBy = _UserID;
                                CurrentDateItem2.ModifiedOn = DateTime.Now.Date;
                                CurrentDateItem2.Remark = "Issue To Production";
                            }
                            else
                            {
                                var PreviousDateItem2 = (from ds in context.INV_ChemStockDaily
                                                         where ds.ItemID == item.ItemID && (ds.StoreID).ToString() == TransactionInfo.TransactionTo &&
                                                         ds.UnitID == item.TransactionUnit
                                                         orderby ds.TransectionID descending
                                                         select ds).FirstOrDefault();

                                INV_ChemStockDaily objStockDaily = new INV_ChemStockDaily();

                                objStockDaily.ItemID = item.ItemID;
                                objStockDaily.StoreID = Convert.ToByte(TransactionInfo.TransactionTo);
                                objStockDaily.UnitID = item.TransactionUnit;
                                objStockDaily.OpeningQty = (PreviousDateItem2 == null ? 0 : PreviousDateItem2.ClosingQty);
                                objStockDaily.ReceiveQty = item.TransactionQty;
                                objStockDaily.IssueQty = 0;
                                objStockDaily.ClosingQty = objStockDaily.OpeningQty + item.TransactionQty;
                                objStockDaily.StockDate = DateTime.Now.Date;
                                objStockDaily.SetOn = DateTime.Now.Date;
                                objStockDaily.SetBy = _UserID;
                                objStockDaily.Remark = "Issue To Production";

                                context.INV_ChemStockDaily.Add(objStockDaily);
                                context.SaveChanges();
                            }

                            #endregion

                            #region Supplier_Stock_Update

                            var FoundItemSupplier2 = (from i in context.INV_ChemStockSupplier
                                                      where i.SupplierID == item.SupplierID && (i.StoreID).ToString() == TransactionInfo.TransactionTo &&
                                                      i.ItemID == item.ItemID & i.UnitID == item.TransactionUnit
                                                      orderby i.TransectionID descending
                                                      select i).FirstOrDefault();

                            if (FoundItemSupplier2 == null)
                            {
                                INV_ChemStockSupplier objStockSupplier = new INV_ChemStockSupplier();

                                objStockSupplier.SupplierID = Convert.ToInt16(item.SupplierID);
                                objStockSupplier.StoreID = Convert.ToByte(TransactionInfo.TransactionTo);
                                objStockSupplier.ItemID = item.ItemID;
                                objStockSupplier.UnitID = item.TransactionUnit;
                                objStockSupplier.OpeningQty = 0;
                                objStockSupplier.ReceiveQty = item.TransactionQty;
                                objStockSupplier.IssueQty = 0;
                                objStockSupplier.ClosingQty = item.TransactionQty;
                                objStockSupplier.SetOn = DateTime.Now.Date;
                                objStockSupplier.SetBy = _UserID;
                                objStockSupplier.Remark = "Issue To Production";

                                context.INV_ChemStockSupplier.Add(objStockSupplier);
                                context.SaveChanges();
                            }
                            else
                            {
                                INV_ChemStockSupplier objStockSupplier = new INV_ChemStockSupplier();

                                objStockSupplier.SupplierID = Convert.ToInt16(item.SupplierID);
                                objStockSupplier.StoreID = Convert.ToByte(TransactionInfo.TransactionTo);
                                objStockSupplier.ItemID = item.ItemID;
                                objStockSupplier.UnitID = item.TransactionUnit;
                                objStockSupplier.OpeningQty = FoundItemSupplier2.ClosingQty;
                                objStockSupplier.ReceiveQty = item.TransactionQty;
                                objStockSupplier.IssueQty = 0;
                                objStockSupplier.ClosingQty = FoundItemSupplier2.ClosingQty + item.TransactionQty;
                                objStockSupplier.SetBy = _UserID;
                                objStockSupplier.SetOn = DateTime.Now.Date;
                                objStockSupplier.Remark = "Issue To Production";

                                context.INV_ChemStockSupplier.Add(objStockSupplier);
                                context.SaveChanges();
                            }

                            #endregion

                            #region Item_Stock_Update

                            var FoundItem2 = (from i in context.INV_ChemStockItem
                                              where (i.StoreID).ToString() == TransactionInfo.TransactionTo &&
                                              i.ItemID == item.ItemID & i.UnitID == item.TransactionUnit
                                              orderby i.TransectionID descending
                                              select i).FirstOrDefault();

                            if (FoundItem2 == null)
                            {
                                INV_ChemStockItem objStockItem = new INV_ChemStockItem();

                                objStockItem.StoreID = Convert.ToByte(TransactionInfo.TransactionTo);
                                objStockItem.ItemID = item.ItemID;
                                objStockItem.UnitID = item.TransactionUnit;
                                objStockItem.OpeningQty = 0;
                                objStockItem.ReceiveQty = item.TransactionQty;
                                objStockItem.IssueQty = 0;
                                objStockItem.ClosingQty = item.TransactionQty;
                                objStockItem.SetOn = DateTime.Now.Date;
                                objStockItem.SetBy = _UserID;
                                objStockItem.Remark = "Issue To Production";

                                context.INV_ChemStockItem.Add(objStockItem);
                                context.SaveChanges();
                            }
                            else
                            {
                                INV_ChemStockItem objStockItem = new INV_ChemStockItem();

                                objStockItem.StoreID = Convert.ToByte(TransactionInfo.TransactionTo);
                                objStockItem.ItemID = item.ItemID;
                                objStockItem.UnitID = item.TransactionUnit;
                                objStockItem.OpeningQty = FoundItem2.ClosingQty;
                                objStockItem.IssueQty = 0;
                                objStockItem.ReceiveQty = item.TransactionQty;
                                objStockItem.ClosingQty = FoundItem2.ClosingQty + item.TransactionQty;
                                objStockItem.SetOn = DateTime.Now.Date;
                                objStockItem.SetBy = _UserID;
                                objStockItem.Remark = "Issue To Production";

                                context.INV_ChemStockItem.Add(objStockItem);
                                context.SaveChanges();
                            }

                            #endregion

                        }
                        if (Msg.ReturnId == 1)
                        {
                            TransactionInfo.CheckComments = _CheckComment;
                            TransactionInfo.RecordStatus = "CNF";
                            context.SaveChanges();
                            Transaction.Complete();
                        }
                    }

                }

                return Msg;
            }
            catch (Exception e)
            {
                Msg.ReturnId = 0;
                Msg.Msg = "Confirmation Failed";
                return Msg;
            }
        }
    }
}
