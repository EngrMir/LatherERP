using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.OperationModel;
using System.Transactions;
using System.Linq;


namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalInvLeatherStockAdjustItem
    {
        private readonly BLC_DEVEntities _context;


        public DalInvLeatherStockAdjustItem()
        {
            _context = new BLC_DEVEntities();
        }

        public List<Sys_Supplier> FindSupplierForAdjust(byte leatherType, byte ReceiveStore)
        {
            var SuppliersInStock = (from p in _context.Inv_StockSupplier
                                    where p.LeatherType == leatherType && p.StoreID == ReceiveStore
                                    group p by new
                                    {
                                        p.SupplierID,
                                        p.ItemTypeID,
                                        p.PurchaseID
                                    } into g
                                    select new
                                    {
                                        SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                                        ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault()
                                    }).ToList();

            //var FinalList = SuppliersInStock.GroupBy(x => x.SupplierID).Select(group => group.First());

            var SupplierList = (from s in SuppliersInStock
                                where s.ClosingQty>0

                               join sn in _context.Sys_Supplier on s.SupplierID equals sn.SupplierID into Suppliers
                               from sn in Suppliers.DefaultIfEmpty()

                               select new Sys_Supplier
                                {
                                    SupplierID = s.SupplierID,
                                    SupplierCode = sn.SupplierCode,
                                    SupplierName = sn.SupplierName
                                }).GroupBy(x => x.SupplierID).Select(group => group.First());


            //var FinalList = SupplierList.GroupBy(x => x.SupplierID).Select(group => group.First());

            return SupplierList.ToList();
        }

        public List<InvLeatherStockAdjustItem> FindChallanForAdjust(byte leatherType, byte receiveStore, Int32 supplierID)
        {
            var ItemWiseLastTransaction = (from t in _context.Inv_StockSupplier
                                           where t.LeatherType == leatherType && t.StoreID == receiveStore && t.SupplierID == supplierID
                                           group t by new
                                           {
                                               t.ItemTypeID,
                                               t.PurchaseID
                                           }
                                               into g
                                               select new
                                               {
                                                   ItemID = g.Key,
                                                   TransactionID = g.Max(p => p.TransectionID),
                                                   PurchaseID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.PurchaseID).FirstOrDefault(),
                                                   ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault()
                                               });

            var ChallanList = (from s in _context.Inv_StockSupplier

                               from t in ItemWiseLastTransaction
                               where t.TransactionID == s.TransectionID & t.ClosingQty > 0

                               from u in _context.Sys_Unit
                               where u.UnitID == s.UnitID

                               from i in _context.Sys_ItemType
                               where i.ItemTypeID == s.ItemTypeID

                               //Added Later
                               from c in _context.Prq_Purchase
                               where c.PurchaseID == t.PurchaseID
                               //Added Later

                               select new InvLeatherStockAdjustItem
                               {
                                   PurchaseID = s.PurchaseID,
                                   PurchaseNo = c.PurchaseNo,
                                   ItemTypeID = s.ItemTypeID,
                                   ItemTypeName = i.ItemTypeName,
                                   StockQty = s.ClosingQty,
                                   Unit = s.UnitID,
                                   UnitName = u.UnitName
                               }).ToList();

            return ChallanList;
        }

        public int SaveAdjustItemRequest(InvLeatherStockAdjustModel model, int userID)
        {

            var leatherTypeID = DalCommon.GetLeatherTypeCode("Raw Hide");
            var leatherStatusID = DalCommon.GetLeatherStatusCode("Raw Hide");
            using (TransactionScope transaction = new TransactionScope())
            {
                #region Insert

                Inv_LeatherStockAdjustRequest objAdjustmentRequest = new Inv_LeatherStockAdjustRequest();

                objAdjustmentRequest.PurchaseYear = model.PurchaseYear;
                objAdjustmentRequest.RequestDate = DalCommon.SetDate(model.RequestDate);
                objAdjustmentRequest.LeatherType = model.LeatherType;
                objAdjustmentRequest.StoreID = model.StoreID;
                objAdjustmentRequest.RecordStatus = "NCF";
                objAdjustmentRequest.SetBy = userID;
                objAdjustmentRequest.SetOn = DateTime.Now;

                _context.Inv_LeatherStockAdjustRequest.Add(objAdjustmentRequest);
                _context.SaveChanges();

                var currentRequestID = objAdjustmentRequest.RequestID;

                foreach (var item in model.StockAdjustItemList)
                {
                    Inv_LeatherStockAdjustItem objAdjustmentItems = new Inv_LeatherStockAdjustItem();

                    objAdjustmentItems.RequestID = currentRequestID;
                    objAdjustmentItems.SupplierID = item.SupplierID;
                    objAdjustmentItems.PurchaseID = item.ChallanID;
                    objAdjustmentItems.ItemTypeID = item.ItemTypeID;
                    objAdjustmentItems.LeatherType = model.LeatherType;
                    objAdjustmentItems.LeatherStatus = leatherStatusID;
                    objAdjustmentItems.ItemQty = item.ItemQty;
                    objAdjustmentItems.Unit = item.Unit;
                    objAdjustmentItems.Remarks = item.Remarks;
                    objAdjustmentItems.AdjustmentCause = item.AdjustmentCause;



                    _context.Inv_LeatherStockAdjustItem.Add(objAdjustmentItems);
                    _context.SaveChanges();
                }

                transaction.Complete();
                return currentRequestID;

                #endregion
            }
        }

        public int UpdateAdjustItemRequest(InvLeatherStockAdjustModel model, int userID)
        {
            var leatherTypeID = DalCommon.GetLeatherTypeCode("Raw Hide");
            var leatherStatusID = DalCommon.GetLeatherStatusCode("Raw Hide");

            //var requestdate = (model.RequestDate).ToString("dd'/'MM'/'yyyy");

            using (TransactionScope transaction = new TransactionScope())
            {
                var currentRequestID = Convert.ToInt32(model.RequestID);
                var currentRequest = (from r in _context.Inv_LeatherStockAdjustRequest.AsEnumerable()
                                      where r.RequestID == Convert.ToInt32(model.RequestID)
                                      select r).FirstOrDefault();



                currentRequest.RequestDate = DalCommon.SetDate(model.RequestDate);
                currentRequest.PurchaseYear = model.PurchaseYear;
                currentRequest.LeatherType = model.LeatherType;
                currentRequest.StoreID = model.StoreID;
                currentRequest.SetBy = userID;
                currentRequest.SetOn = DateTime.Now;
                _context.SaveChanges();

                if (model.StockAdjustItemList != null)
                {
                    foreach (var item in model.StockAdjustItemList)
                    {
                        if (item.AdjustID == 0)
                        {
                            Inv_LeatherStockAdjustItem objAdjustmentItems = new Inv_LeatherStockAdjustItem();

                            objAdjustmentItems.RequestID = Convert.ToInt32(model.RequestID);
                            objAdjustmentItems.SupplierID = item.SupplierID;
                            objAdjustmentItems.PurchaseID = item.ChallanID;
                            objAdjustmentItems.ItemTypeID = item.ItemTypeID;
                            objAdjustmentItems.LeatherType = model.LeatherType;
                            objAdjustmentItems.LeatherStatus = leatherStatusID;
                            objAdjustmentItems.ItemQty = item.ItemQty;
                            objAdjustmentItems.Unit = item.Unit;
                            objAdjustmentItems.Remarks = item.Remarks;
                            objAdjustmentItems.AdjustmentCause = item.AdjustmentCause;


                            _context.Inv_LeatherStockAdjustItem.Add(objAdjustmentItems);
                            _context.SaveChanges();
                        }
                        else
                        {
                            var CurrentItem = (from i in _context.Inv_LeatherStockAdjustItem.AsEnumerable()
                                               where i.AdjustID == item.AdjustID
                                               select i).FirstOrDefault();

                            CurrentItem.SupplierID = item.SupplierID;
                            CurrentItem.PurchaseID = item.ChallanID;
                            CurrentItem.ItemTypeID = item.ItemTypeID;
                            CurrentItem.LeatherType = model.LeatherType;
                            CurrentItem.LeatherStatus = leatherStatusID;
                            CurrentItem.ItemQty = item.ItemQty;
                            CurrentItem.Unit = item.Unit;
                            CurrentItem.Remarks = item.Remarks;
                            CurrentItem.AdjustmentCause = item.AdjustmentCause;
                            _context.SaveChanges();
                        }
                    }
                }

                transaction.Complete();
                return currentRequestID;
            }
        }

        public int UpdateAdjustmentRequestWithItemValue(InvLeatherStockAdjustModel model)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                foreach (var item in model.StockAdjustItemList)
                {
                    var CurrentItem = (from i in _context.Inv_LeatherStockAdjustItem.AsEnumerable()
                                       where i.AdjustID == item.AdjustID
                                       select i).FirstOrDefault();

                    CurrentItem.ItemValue = item.ItemValue;
                    _context.SaveChanges();
                }

                transaction.Complete();
            }

            return 1;
        }



        // Search List for Leather Stock Adjustment Request
        public List<InvLeatherStockAdjustRequest> GetAllNCFRequestList()
        {
            var AllRequest = (from r in _context.Inv_LeatherStockAdjustRequest.AsEnumerable()
                              where r.RecordStatus != "APV"
                              from s in _context.SYS_Store
                              where s.StoreID == r.StoreID
                              from t in _context.Sys_LeatherType
                              where t.LeatherTypeID == r.LeatherType

                              //from i in _context.Inv_LeatherStockAdjustItem.Where(x=>x.RequestID==r.RequestID).DefaultIfEmpty()
                              //from sp in _context.Sys_Supplier.Where(x=>x.SupplierID== i.SupplierID).DefaultIfEmpty()

                              orderby r.RequestDate descending

                              select new InvLeatherStockAdjustRequest
                              {
                                  RequestID = r.RequestID,
                                  PurchaseYear = r.PurchaseYear,
                                  RequestDate = (r.RequestDate).ToString("dd'/'MM'/'yyyy"),
                                  LeatherType = r.LeatherType,
                                  StoreName = s.StoreName,
                                  LeatherTypeName = t.LeatherTypeName,
                                  StoreID = r.StoreID,
                                  RecordStatus = DalCommon.ReturnRecordStatus(r.RecordStatus)
                                  //SupplierID= (i.SupplierID).ToString(),
                                  //SupplierName= sp.SupplierName,
                                  //ChallanID= (i.ChallanID).ToString()
                              }).ToList();

            return AllRequest;
        }

        public List<InvLeatherStockAdjustRequest> GetAllCNFRequestList()
        {
            var AllRequest = (from r in _context.Inv_LeatherStockAdjustRequest.AsEnumerable()
                              where r.RecordStatus == "CNF" || r.RecordStatus == "APV"
                              from s in _context.SYS_Store
                              where s.StoreID == r.StoreID
                              from t in _context.Sys_LeatherType
                              where t.LeatherTypeID == r.LeatherType

                              orderby r.RequestDate descending

                              select new InvLeatherStockAdjustRequest
                              {
                                  RequestID = r.RequestID,
                                  PurchaseYear = r.PurchaseYear,
                                  RequestDate = (r.RequestDate).ToString("dd'/'MM'/'yyyy"),
                                  LeatherType = r.LeatherType,
                                  StoreName = s.StoreName,
                                  LeatherTypeName = t.LeatherTypeName,
                                  StoreID = r.StoreID,
                                  RecordStatus = DalCommon.ReturnRecordStatus(r.RecordStatus)
                              }).ToList();

            return AllRequest;
        }

        public List<InvLeatherStockAdjustItem> GetRequestDetails(string requestID)
        {
            var requestInformation = (from r in _context.Inv_LeatherStockAdjustRequest.AsEnumerable()
                                      where (r.RequestID).ToString() == requestID
                                      select r).FirstOrDefault();

            var allItem = (from i in _context.Inv_LeatherStockAdjustItem.AsEnumerable()
                           where (i.RequestID).ToString() == requestID

                           from s in _context.Sys_Supplier
                           where s.SupplierID == i.SupplierID

                           from t in _context.Sys_ItemType
                           where t.ItemTypeID == i.ItemTypeID

                           from u in _context.Sys_Unit
                           where u.UnitID == i.Unit

                           from c in _context.Prq_Purchase
                           where c.PurchaseID == i.PurchaseID

                           select new InvLeatherStockAdjustItem
                           {
                               AdjustID = i.AdjustID,
                               SupplierID = i.SupplierID,
                               SupplierName = s.SupplierName,
                               PurchaseID = Convert.ToInt64(i.PurchaseID),
                               PurchaseNo = c.PurchaseNo,
                               ItemTypeID = i.ItemTypeID,
                               ItemTypeName = t.ItemTypeName,
                               Unit = i.Unit,
                               UnitName = u.UnitName,
                               ItemQty = i.ItemQty,
                               AdjustmentCause = i.AdjustmentCause,
                               //StockQty = Convert.ToDecimal(st.ClosingQty),
                               Remarks = i.Remarks,
                               ItemValue = i.ItemValue,
                               LeatherStatus = i.LeatherStatus,
                               LeatherType = i.LeatherType



                           }).ToList();

            foreach (var item in allItem)
            {
                var StockQuantity = _context.Inv_StockSupplier.Where(x => x.SupplierID == item.SupplierID &&
                    x.PurchaseID== item.PurchaseID && 
                    x.StoreID == requestInformation.StoreID &&
                    x.LeatherStatusID == item.LeatherStatus &&
                    x.ItemTypeID == item.ItemTypeID &&
                    x.LeatherType == item.LeatherType).OrderByDescending(x => x.TransectionID).FirstOrDefault();

                if (StockQuantity != null)
                    item.StockQty = Convert.ToDecimal(StockQuantity.ClosingQty);

            }


            return allItem;
        }

        public bool ConfirmAdjustRequest(string requestID, string confirmComment, int userID)
        {
            try
            {
                var currentRequest = (from s in _context.Inv_LeatherStockAdjustRequest.AsEnumerable()
                                      where s.RequestID == Convert.ToInt32(requestID)
                                      select s).FirstOrDefault();

                currentRequest.RecordStatus = "CNF";
                currentRequest.CheckComment = confirmComment;
                currentRequest.CheckedBy = userID;
                currentRequest.CheckDate = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool ApproveAdjustRequest(string requestID, string approveComment, int userID)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    var StockStatus = 1; //To check Stock Availability
                    var currentRequest = (from s in _context.Inv_LeatherStockAdjustRequest.AsEnumerable()
                                          where s.RequestID == Convert.ToInt32(requestID)
                                          select s).FirstOrDefault();
                    var ItemList = (from i in _context.Inv_LeatherStockAdjustItem.AsEnumerable()
                                    where i.RequestID == currentRequest.RequestID
                                    select i).ToList();
                    foreach (var item in ItemList)
                    {
                        #region Item Stock Update
                        var LastItem = (from i in _context.Inv_StockItem.AsEnumerable()
                                        where i.ItemTypeID == item.ItemTypeID && i.StoreID == currentRequest.StoreID && i.LeatherType == item.LeatherType && i.LeatherStatus == item.LeatherStatus
                                        orderby i.TransectionID descending
                                        select i).FirstOrDefault();

                        if (LastItem.ClosingQty >= item.ItemQty)
                        {
                            var objItemStock = new Inv_StockItem();

                            objItemStock.StoreID = LastItem.StoreID;
                            objItemStock.LeatherType = LastItem.LeatherType;
                            objItemStock.LeatherStatus = LastItem.LeatherStatus;
                            objItemStock.OpeningQty = LastItem.ClosingQty;
                            objItemStock.ReceiveQty = 0;
                            objItemStock.IssueQty = item.ItemQty;
                            objItemStock.ClosingQty = LastItem.ClosingQty - item.ItemQty;
                            objItemStock.ItemTypeID = LastItem.ItemTypeID;
                            objItemStock.UnitID = LastItem.UnitID;
                            

                            _context.Inv_StockItem.Add(objItemStock);
                            _context.SaveChanges();
                        }
                        else
                        {
                            StockStatus = 0;
                        }



                        #endregion
                        #region Supplier Stock Update

                        var LastSupplierItem = (from s in _context.Inv_StockSupplier.AsEnumerable()
                                                where s.ItemTypeID == item.ItemTypeID && s.StoreID == currentRequest.StoreID && s.LeatherType == item.LeatherType && s.LeatherStatusID == item.LeatherStatus
                                                && s.SupplierID == item.SupplierID && s.PurchaseID == item.PurchaseID
                                                orderby s.TransectionID descending
                                                select s).FirstOrDefault();

                        if (LastSupplierItem.ClosingQty >= item.ItemQty)
                        {
                            var objStockSupplier = new Inv_StockSupplier();
                            objStockSupplier.PurchaseID = LastSupplierItem.PurchaseID;
                            objStockSupplier.SupplierID = LastSupplierItem.SupplierID;
                            objStockSupplier.ItemTypeID = LastSupplierItem.ItemTypeID;
                            objStockSupplier.LeatherType = LastSupplierItem.LeatherType;
                            objStockSupplier.LeatherStatusID = LastSupplierItem.LeatherStatusID;
                            objStockSupplier.StoreID = LastSupplierItem.StoreID;
                            objStockSupplier.OpeningQty = LastSupplierItem.ClosingQty;
                            objStockSupplier.ReceiveQty = 0;
                            objStockSupplier.IssueQty = item.ItemQty;
                            objStockSupplier.ClosingQty = LastSupplierItem.ClosingQty - item.ItemQty;
                            objStockSupplier.UnitID = LastSupplierItem.UnitID;
                            objStockSupplier.UpdateReason = "Adjustment";

                            _context.Inv_StockSupplier.Add(objStockSupplier);
                            _context.SaveChanges();
                        }
                        else
                        {
                            StockStatus = 0;
                        }


                        #endregion
                        #region Daily Stock Update
                        var CheckCurrentDateItem = (from d in _context.Inv_StockDaily.AsEnumerable()
                                                    where d.ItemTypeID == item.ItemTypeID && d.StoreID == currentRequest.StoreID && d.LeatherType == item.LeatherType && d.LeatherStatus == item.LeatherStatus && d.StockDate == DateTime.Now.Date
                                                    select d).Any();


                        if (CheckCurrentDateItem)
                        {
                            var CurrentDateItem = (from d in _context.Inv_StockDaily.AsEnumerable()
                                                   where d.ItemTypeID == item.ItemTypeID && d.StoreID == currentRequest.StoreID && d.LeatherType == item.LeatherType && d.LeatherStatus == item.LeatherStatus && d.StockDate == DateTime.Now.Date
                                                   select d).FirstOrDefault();

                            if (CurrentDateItem.ClosingQty >= item.ItemQty)
                            {
                                CurrentDateItem.DailyIssueQty = item.ItemQty;
                                CurrentDateItem.ClosingQty = CurrentDateItem.ClosingQty - item.ItemQty;
                                _context.SaveChanges();
                            }
                            else
                            {
                                StockStatus = 0;
                            }


                        }
                        else
                        {
                            var CurrentDateItem = (from d in _context.Inv_StockDaily.AsEnumerable()
                                                   where d.ItemTypeID == item.ItemTypeID && d.StoreID == currentRequest.StoreID && d.LeatherType == item.LeatherType && d.LeatherStatus == item.LeatherStatus
                                                   orderby d.StockDate descending
                                                   select d).FirstOrDefault();
                            if (CurrentDateItem.ClosingQty >= item.ItemQty)
                            {
                                var objDailyStock = new Inv_StockDaily();

                                objDailyStock.StockDate = DateTime.Now.Date;
                                objDailyStock.ItemTypeID = CurrentDateItem.ItemTypeID;
                                objDailyStock.StoreID = CurrentDateItem.StoreID;
                                objDailyStock.LeatherType = CurrentDateItem.LeatherType;
                                objDailyStock.LeatherStatus = CurrentDateItem.LeatherStatus;
                                objDailyStock.UnitID = CurrentDateItem.UnitID;
                                objDailyStock.OpeningQty = CurrentDateItem.ClosingQty;
                                objDailyStock.DailyIssueQty = item.ItemQty;
                                objDailyStock.DailyReceiveQty = 0;
                                objDailyStock.ClosingQty = CurrentDateItem.ClosingQty - item.ItemQty;

                                _context.Inv_StockDaily.Add(objDailyStock);
                                _context.SaveChanges();
                            }
                            else
                            {
                                StockStatus = 0;
                            }

                        }
                        #endregion
                    }

                    if (StockStatus == 1)
                    {
                        currentRequest.RecordStatus = "APV";
                        currentRequest.ApproveComment = approveComment;
                        currentRequest.ApprovedBy = userID;
                        currentRequest.ApproveDate = DateTime.Now;
                        _context.SaveChanges();
                        transaction.Complete();
                        return true;
                    }
                    else
                    {
                        transaction.Dispose();
                        return false;
                    }

                }

            }
            catch (Exception e)
            {
                return false;
            }

        }


        public bool DeleteAdjustItem(string AdjustID)
        {
            try
            {
                var AdjustItem = (from c in _context.Inv_LeatherStockAdjustItem.AsEnumerable()
                                  where c.AdjustID == Convert.ToInt64(AdjustID)
                                  select c).FirstOrDefault();

                _context.Inv_LeatherStockAdjustItem.Remove(AdjustItem);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteAdjustmentRequest(string RequestID)
        {
            try
            {
                var Request = (from c in _context.Inv_LeatherStockAdjustRequest.AsEnumerable()
                               where c.RequestID == Convert.ToInt64(RequestID)
                               select c).FirstOrDefault();

                _context.Inv_LeatherStockAdjustRequest.Remove(Request);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
