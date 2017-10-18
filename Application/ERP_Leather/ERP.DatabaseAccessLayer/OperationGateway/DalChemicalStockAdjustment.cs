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
    public class DalChemicalStockAdjustment
    {
        public List<SysStore> GetChemicalandProductionStore()
        {
            using (var context = new BLC_DEVEntities())
            {
                var storeList = (from s in context.SYS_Store
                    where (s.StoreCategory == "Chemical" & s.StoreType == "Chemical") || s.StoreCategory == "Production"
                    orderby s.StoreCategory
                    select new SysStore
                    {
                        StoreID = s.StoreID,
                        StoreName = s.StoreName
                    }).ToList();

                return storeList;
            }
        }

        public ValidationMsg ConfirmChemicalStockAdjustTransaction(long _TransactionID, string _CheckComment, int _UserID)
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

                        var RequestInfo = (from r in context.INV_StoreTransRequest
                                           where r.TransactionID == _TransactionID
                                           select r).FirstOrDefault();

                        //if(checkDateInfo)
                        //{
                        //    
                        //}

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
                                #region Chemical Supplier Stock Update
                                var CheckSupplierStock = (from i in context.INV_ChemStockSupplier
                                                          where i.SupplierID == item.SupplierID && (i.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                                          i.ItemID == item.ItemID && i.UnitID == item.TransactionUnit
                                                          select i).Any();
                                if (!CheckSupplierStock)
                                {
                                    Msg.Msg = "Item Does Not Exist in Stock";
                                    Msg.ReturnId = 0;
                                    break;
                                }
                                else
                                {
                                    var FoundItem = (from i in context.INV_ChemStockSupplier
                                                     where i.SupplierID == item.SupplierID && (i.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                                     i.ItemID == item.ItemID && i.UnitID == item.TransactionUnit
                                                     orderby i.TransectionID descending
                                                     select i).FirstOrDefault();

                                    if (FoundItem.ClosingQty >= item.TransactionQty)
                                    {
                                        var NewItem = new INV_ChemStockSupplier();

                                        NewItem.SupplierID = Convert.ToInt16(item.SupplierID);
                                        NewItem.StoreID = Convert.ToByte(TransactionInfo.TransactionFrom);
                                        NewItem.ItemID = item.ItemID;
                                       
                                        NewItem.UnitID = item.TransactionUnit;

                                        NewItem.OpeningQty = FoundItem.ClosingQty;
                                        NewItem.ReceiveQty = 0;
                                        NewItem.IssueQty = item.TransactionQty;
                                        NewItem.ClosingQty = FoundItem.ClosingQty - item.TransactionQty;

                                        NewItem.SetBy = _UserID;
                                        NewItem.SetOn = DateTime.Now.Date;
                                        NewItem.Remark = "Consumed In Finish Production";

                                        context.INV_ChemStockSupplier.Add(NewItem);
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        Msg.Msg = "Not Enough Quantity in Stock";
                                        Msg.ReturnId = 0;
                                        break;
                                    }

                                }
                                #endregion

                                #region Chemical Item Stock Update
                                var CheckItemStock = (from i in context.INV_ChemStockItem
                                                      where (i.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                                      i.ItemID == item.ItemID && i.UnitID == item.TransactionUnit
                                                      select i).Any();

                                if (!CheckItemStock)
                                {
                                    Msg.Msg = "Item Does Not Exist in Stock";
                                    Msg.ReturnId = 0;
                                    break;
                                }
                                else
                                {
                                    var FoundItem = (from i in context.INV_ChemStockItem
                                                     where (i.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                                     i.ItemID == item.ItemID && i.UnitID == item.TransactionUnit
                                                     orderby i.TransectionID descending
                                                     select i).FirstOrDefault();

                                    if (FoundItem.ClosingQty >= item.TransactionQty )
                                    {
                                        var NewItem = new INV_ChemStockItem();

                                        NewItem.StoreID = Convert.ToByte(TransactionInfo.TransactionFrom);
                                        NewItem.ItemID = item.ItemID;
                                       
                                        NewItem.UnitID = item.TransactionUnit;

                                        NewItem.OpeningQty = FoundItem.ClosingQty;
                                        NewItem.IssueQty = item.TransactionQty;
                                        NewItem.ReceiveQty = 0;
                                        NewItem.ClosingQty = FoundItem.ClosingQty - item.TransactionQty;

                                       
                                        NewItem.SetBy = _UserID;
                                        NewItem.SetOn = DateTime.Now.Date;
                                        NewItem.Remark = "Consumed In Finish Production";

                                        context.INV_ChemStockItem.Add(NewItem);
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        Msg.Msg = "Not Enough Quantity in Stock";
                                        Msg.ReturnId = 0;
                                        break;
                                    }

                                }

                                #endregion

                                #region Chemical Daily Stock Update
                                var MaximumTransactionDate = (from d in context.INV_ChemStockDaily.AsEnumerable()
                                                              where (d.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                                                d.ItemID == item.ItemID && d.UnitID == item.TransactionUnit

                                                              orderby d.TransectionID descending
                                                              select d.StockDate).FirstOrDefault();

                                if (MaximumTransactionDate > RequestInfo.ReqToDate)
                                {
                                    Msg.Msg = "Transaction To Date is Backdated.";
                                    Msg.ReturnId = 0;
                                    break;
                                }
                                else
                                {
                                    var CheckDailyStock = (from i in context.INV_ChemStockDaily
                                                           where i.StockDate == RequestInfo.ReqToDate && (i.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                                           i.ItemID == item.ItemID && i.UnitID == item.TransactionUnit
                                                           select i).Any();

                                    if (CheckDailyStock)
                                    {
                                        var CurrentItem = (from i in context.INV_ChemStockDaily
                                                           where i.StockDate == RequestInfo.ReqToDate && (i.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                                           i.ItemID == item.ItemID && i.UnitID == item.TransactionUnit
                                                           select i).FirstOrDefault();

                                        if (CurrentItem.ClosingQty >= item.TransactionQty)
                                        {
                                            CurrentItem.IssueQty = CurrentItem.IssueQty + item.TransactionQty;
                                            CurrentItem.ClosingQty = CurrentItem.ClosingQty - item.TransactionQty;

                                            CurrentItem.ModifiedBy = _UserID;
                                            CurrentItem.ModifiedOn = DateTime.Now.Date;
                                            CurrentItem.Remark = "Consumed In Finish Production";
                                            context.SaveChanges();
                                        }
                                        else
                                        {
                                            Msg.Msg = "Not Enough Quantity in Stock";
                                            Msg.ReturnId = 0;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        var FoundItem = (from i in context.INV_ChemStockDaily
                                                         where (i.StoreID).ToString() == TransactionInfo.TransactionFrom &&
                                                         i.ItemID == item.ItemID && i.UnitID == item.TransactionUnit
                                                         orderby i.TransectionID descending
                                                         select i).FirstOrDefault();
                                        if (FoundItem != null)
                                        {
                                            if (FoundItem.ClosingQty >= item.TransactionQty)
                                            {
                                                var NewItem = new INV_ChemStockDaily();

                                                NewItem.StockDate = Convert.ToDateTime(RequestInfo.ReqToDate);
                                                NewItem.StoreID = Convert.ToByte(TransactionInfo.TransactionFrom);
                                                NewItem.ItemID = item.ItemID;

                                                NewItem.UnitID = item.TransactionUnit;

                                                NewItem.OpeningQty = FoundItem.ClosingQty;
                                                NewItem.IssueQty = item.TransactionQty;
                                                NewItem.ReceiveQty = 0;
                                                NewItem.ClosingQty = FoundItem.ClosingQty - item.TransactionQty;

                                                NewItem.SetOn = DateTime.Now.Date;
                                                NewItem.SetBy = _UserID;
                                                NewItem.Remark = "Consumed In Finish Production";

                                                context.INV_ChemStockDaily.Add(NewItem);
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                Msg.Msg = "Not Enough Quantity in Stock";
                                                Msg.ReturnId = 0;
                                                break;
                                            }

                                        }
                                        else
                                        {
                                            Msg.Msg = "Item Does Not Exist in Stock";
                                            Msg.ReturnId = 0;
                                            break;
                                        }
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
    }
}
