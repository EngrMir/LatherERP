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
//using System.Web.Mvc;





namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalIssueToProduction
    {
        private readonly BLC_DEVEntities _context;

        public DalIssueToProduction()
        {
            _context = new BLC_DEVEntities();
        }

        //For Requisition LOV
        public List<PRDChemProdReq> GetRequisitionFromFixedStore(string _RequisitionFrom)
        {
            var Data = (from r in _context.PRD_ChemProdReq.AsEnumerable()
                        where (r.RequisitionTo).ToString() == _RequisitionFrom && r.RequisitionState== "RNG" && r.RecordStatus== "APV"


                        join s in _context.SYS_Store on r.RequisitionFrom equals s.StoreID into Stores
                        from s in Stores.DefaultIfEmpty()

                        orderby r.RequisitionID descending
                        select new PRDChemProdReq
                        {
                            RequisitionID = r.RequisitionID,
                            RequisitionNo = r.RequisitionNo,
                            RequisitionCategory = DalCommon.ReturnRequisitionCategory(r.RequisitionCategory),
                            RequisitionType = DalCommon.ReturnOrderType(r.RequisitionType),
                            RequiredByTime= Convert.ToByte(r.RequiredByTime),
                            RequisitionFrom= r.RequisitionFrom,
                            RequisitionFromName= (s==null? "": s.StoreName),
                            ReqRaisedOn = Convert.ToDateTime(r.ReqRaisedOn).ToString("dd'/'MM'/'yyyy"),
                        }).ToList();
            return Data;
        }

        // After Requisition LOV
        public PRDChemProdReq GetRequisitionDetailsFromFixedStore(int _RequisitionID, byte _RequisitionAt)
        {
            var FinalStock = DalChemicalStock.ItemWiseStockInSpecificChemicalStore(_RequisitionAt);

            var model = new PRDChemProdReq();

            var RequisitionInfo = (from r in _context.PRD_ChemProdReq.AsEnumerable()
                                   where r.RequisitionID == _RequisitionID

                                   from u in _context.Users.Where(x => x.UserID == r.ReqRaisedBy).DefaultIfEmpty()

                                   select new PRDChemProdReq
                                   {
                                       RequisitionID = r.RequisitionID,
                                       RequisitionNo = r.RequisitionNo,
                                       JobOrderID = Convert.ToInt16(r.JobOrderID),
                                       JobOrderNo = r.JobOrderNo,
                                       ReqRaisedOn = (Convert.ToDateTime(r.ReqRaisedOn)).ToString("dd'/'MM'/'yyyy"),
                                       ReqRaisedBy = Convert.ToInt16(r.ReqRaisedBy),
                                       ReqRaisedByName = (u == null ? null : (u.FirstName + " " + u.MiddleName +" " + u.LastName)),
                                       RequiredByTime = Convert.ToByte(r.RequiredByTime),
                                       RequisitionCategory = DalCommon.ReturnRequisitionCategory(r.RequisitionCategory),
                                       RequisitionType = DalCommon.ReturnOrderType(r.RequisitionType),
                                       IssueTo= r.RequisitionFrom,
                                       IssueFrom= _RequisitionAt
                                   }).FirstOrDefault();

            model.RequisitionInfo = RequisitionInfo;

            var RequisitionItemList = (from i in _context.PRD_ChemProdReqItem.AsEnumerable()
                                       where i.RequisitionID == _RequisitionID

                                       from it in _context.Sys_ChemicalItem.Where(x => x.ItemID == i.ItemID).DefaultIfEmpty()

                                       join s in FinalStock on i.ItemID equals s.ItemID into badhon
                                       from item in badhon.DefaultIfEmpty()

                                       join su in _context.Sys_Unit on (item == null? 0: item.UnitID) equals su.UnitID into StockUnits
                                       from su in StockUnits.DefaultIfEmpty()


                                       join sup in _context.Sys_Supplier on (item == null ? 0 : item.SupplierID) equals sup.SupplierID into badhon2
                                       from finalitem in badhon2.DefaultIfEmpty()

                                       from siu in _context.Sys_Unit.Where(x => x.UnitID == i.SizeUnit).DefaultIfEmpty()
                                       from ru in _context.Sys_Unit.Where(x => x.UnitID == i.RequisitionUnit).DefaultIfEmpty()

                                       from ps in _context.Sys_Size.Where(x => x.SizeID == i.PackSize).DefaultIfEmpty()

                                       select new PRDChemProdReqItem
                                       {
                                           ItemID = i.ItemID,
                                           ItemName = (it == null ? null : it.ItemName),

                                           StockQty = (item == null ? 0 : item.ClosingQty),
                                           StockUnitName = (su == null ? null : su.UnitName),
                                           SupplierID = (finalitem == null ? 0 : finalitem.SupplierID),
                                           SupplierName = (finalitem == null ? null : finalitem.SupplierName),

                                           PackSizeName = (ps == null ? null : ps.SizeName),
                                           SizeUnitName = (siu == null ? null : siu.UnitName),
                                           PackQty = Convert.ToInt16(i.PackQty),
                                           RequsitionQty = i.RequsitionQty,
                                           RequisitionUnitName = (ru == null ? null : ru.UnitName),

                                           IssuePackSizeName= "Press F9",
                                           IssueUnitName="",

                                           //IssuePackSize = Convert.ToByte(i.PackSize),
                                           //IssuePackSizeName = (ps == null ? null : ps.SizeName),

                                           //IssueSizeUnit = Convert.ToByte(i.SizeUnit),
                                           //IssueSizeUnitName = (siu == null ? null : siu.UnitName),
                                           //IssuePackQty = Convert.ToInt16(i.PackQty),
                                           //IssueQty = i.RequsitionQty,
                                           //IssueUnit = i.RequisitionUnit,
                                           //IssueUnitName = (ru == null ? null : ru.UnitName),
                                           ItemSource = "Via Requisition"
                                       }).ToList();	



            model.RequisitionItemList = RequisitionItemList;

            return model;

        }

        public long Save(INVStoreTrans model, int userId, string pageUrl)
        {
            long CurrentTransactionID = 0;
            long CurrentRequestID = 0;
            
                using (TransactionScope transaction = new TransactionScope())
                {
                    try
                    {
                        using (_context)
                        {
                            string GetTransactionNo;
                            if(model.PageName=="Adjustment")
                            {
                                GetTransactionNo = DalCommon.GetPreDefineNextCodeByUrl("ChemicalStockAdjustment/ChemicalStockAdjustment");
                            }
                            else if(model.PageName=="InventoryAdjustment")
                            {
                                //Need Attention
                                GetTransactionNo = DalCommon.GetPreDefineNextCodeByUrl("ChemicalStockAdjustment/ChemicalStockAdjustment");
                            }
                            else 
                            {
                                GetTransactionNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);
                            }
                         

                            if (GetTransactionNo != null)
                            {
                                #region New_Transaction_Insert

                                INV_StoreTrans objIssue = new INV_StoreTrans();

                                objIssue.TransactionNo = GetTransactionNo;
                                objIssue.TransactionDate = DalCommon.SetDate(model.TransactionDate);
                                if (model.AdjustmentType == "Addition")
                                {
                                    objIssue.TransactionCategory = "RCV";
                                }
                                else
                                {
                                    objIssue.TransactionCategory = "ISU";
                                }
                                
                                
                                objIssue.TransactionFrom = model.TransactionFrom;
                                

                                if(model.PageName=="Adjustment")
                                {
                                    objIssue.TransactionType = "ADI";
                                }
                                else if (model.PageName == "InventoryAdjustment" & model.AdjustmentType== "Addition")
                                {
                                    objIssue.TransactionType = "ADR";
                                }
                                else if (model.PageName == "InventoryAdjustment" & model.AdjustmentType == "Subtraction")
                                {
                                    objIssue.TransactionType = "ADI";
                                }
                                else
                                {
                                    objIssue.TransactionType = "ITP";
                                    objIssue.TransactionTo = model.TransactionTo;
                                }


                                objIssue.RecordStatus = "NCF";
                                objIssue.TransactionState = "RNG";
                                objIssue.TransactionStatus = "TRI";
                                objIssue.SetBy = userId;
                                objIssue.SetOn = DateTime.Now;

                                _context.INV_StoreTrans.Add(objIssue);
                                _context.SaveChanges();
                                CurrentTransactionID = objIssue.TransactionID;

                                #endregion

                                #region New Transaction Request Insert
                                if (model.PageName != "InventoryAdjustment")
                                {
                                    var objRequest = new INV_StoreTransRequest();

                                    objRequest.TransRequestNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);
                                    objRequest.TransactionID = CurrentTransactionID;

                                    if (model.PageName == "Adjustment")
                                    {
                                        objRequest.ReqFromDate = DalCommon.SetDate(model.ReqFromDate);
                                        objRequest.ReqToDate = DalCommon.SetDate(model.ReqToDate);
                                    }
                                    else
                                    {
                                        objRequest.RequisitionID = Convert.ToInt32(model.RefTransactionID);
                                        objRequest.RequestNo = model.RefTransactionNo;
                                    }
                                    objRequest.Remark = model.Remark;
                                    objRequest.SetBy = userId;
                                    objRequest.SetOn = DateTime.Now;

                                    _context.INV_StoreTransRequest.Add(objRequest);
                                    _context.SaveChanges();
                                    CurrentRequestID = objRequest.TransRequestID;
                                }
                                
                                #endregion

                                #region Item Insert
                                if (model.InvStoreTransItemList != null)
                                {
                                    foreach (var item in model.InvStoreTransItemList)
                                    {
                                        INV_StoreTransItem objItem = new INV_StoreTransItem();

                                        objItem.TransactionID = CurrentTransactionID;

                                        if(model.PageName!="InventoryAdjustment")
                                            objItem.TransRequestID = CurrentRequestID;
                                        
                                        objItem.ItemID = item.ItemID;
                                        if (item.SupplierID == 0)
                                            objItem.SupplierID = null;
                                        else
                                            objItem.SupplierID = item.SupplierID;
                                        objItem.TransactionQty = item.TransactionQty;

                                        objItem.TransactionUnit = DalCommon.GetUnitCode(item.TransactionUnitName);


                                        if(item.PackSizeName!=null)
                                            objItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);

                                        if (item.SizeUnitName != null)
                                            objItem.SizeUnit = DalCommon.GetUnitCode(item.SizeUnitName);
                                        objItem.PackQty = item.PackQty;
                                        
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
                    catch (Exception e)
                    {
                        CurrentTransactionID = 0;
                        return CurrentTransactionID;
                    }
                    
                }
                return CurrentTransactionID;
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

                        if (model.AdjustmentType == "Addition")
                        {
                            CurrentTransaction.TransactionCategory = "RCV";
                        }
                        else
                        {
                            CurrentTransaction.TransactionCategory = "ISU";
                        }
                        if (model.PageName == "Adjustment")
                        {
                            CurrentTransaction.TransactionType = "ADI";
                        }
                        else if (model.PageName == "InventoryAdjustment" & model.AdjustmentType == "Addition")
                        {
                            CurrentTransaction.TransactionType = "ADR";
                        }
                        else if (model.PageName == "InventoryAdjustment" & model.AdjustmentType == "Subtraction")
                        {
                            CurrentTransaction.TransactionType = "ADI";
                        }
                        else
                        {
                            CurrentTransaction.TransactionType = "ITP";
                            CurrentTransaction.TransactionTo = model.TransactionTo;
                        }
                            
                        CurrentTransaction.Remarks = model.Remark;
                        
                            


                        CurrentTransaction.TransactionDate = DalCommon.SetDate(model.TransactionDate);
                        CurrentTransaction.ModifiedBy = userId;
                        CurrentTransaction.ModifiedOn = DateTime.Now;
                        //_context.SaveChanges();

                        #endregion

                        #region Request Infromation Update

                        if (model.PageName != "InventoryAdjustment")
                        {
                            if (model.PageName == "Adjustment")
                            {
                                var CurrentRequest = (from r in _context.INV_StoreTransRequest
                                                      where r.TransactionID == model.TransactionID
                                                      select r).FirstOrDefault();

                                CurrentRequest.ReqFromDate = DalCommon.SetDate(model.ReqFromDate);
                                CurrentRequest.ReqToDate = DalCommon.SetDate(model.ReqToDate);
                                CurrentRequest.ModifiedBy = userId;
                                CurrentRequest.ModifiedOn = DateTime.Now;
                                CurrentRequest.Remark = model.Remark;
                            }
                            else
                            {
                                var CurrentRequest = (from r in _context.INV_StoreTransRequest
                                                      where r.TransactionID == model.TransactionID &&
                                                      r.RequisitionID == model.RefTransactionID
                                                      select r).FirstOrDefault();
                                CurrentRequest.Remark = model.Remark;
                                CurrentRequest.ModifiedBy = userId;
                                CurrentRequest.ModifiedOn = DateTime.Now;
                            }
                        }
                        
                       
                       
                        #endregion

                        #region Update Requisition ItemList

                        if (model.InvStoreTransItemList != null)
                        {
                            foreach (var item in model.InvStoreTransItemList)
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
                                    if (item.SupplierID == 0)
                                        objItem.SupplierID = null;
                                    else
                                        objItem.SupplierID = item.SupplierID;
                                    objItem.TransactionQty = item.TransactionQty;

                                    objItem.TransactionUnit = DalCommon.GetUnitCode(item.TransactionUnitName);


                                    if (item.PackSizeName != null)
                                        objItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);

                                    if (item.SizeUnitName != null)
                                        objItem.SizeUnit = DalCommon.GetUnitCode(item.SizeUnitName);


                                    objItem.PackQty = item.PackQty;
                                    objItem.ItemSource = DalCommon.ReturnItemSource(item.ItemSource);
                                    
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
                                    if (item.SupplierID == 0)
                                        FoundItem.SupplierID = null;
                                    else
                                        FoundItem.SupplierID = item.SupplierID;
                                    FoundItem.TransactionQty = item.TransactionQty;

                                    FoundItem.TransactionUnit = DalCommon.GetUnitCode(item.TransactionUnitName);


                                    if (item.PackSizeName != null)
                                        FoundItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);

                                    if (item.SizeUnitName != null)
                                        FoundItem.SizeUnit = DalCommon.GetUnitCode(item.SizeUnitName);


                                    FoundItem.PackQty = item.PackQty;
                                    FoundItem.ModifiedBy = userId;
                                    FoundItem.ModifiedOn = DateTime.Now;

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

        public string GetTransactionNo(long _TransactionID)
        {
            using (_context)
            {
                var TransactionNo = (from p in _context.INV_StoreTrans.AsEnumerable()
                                     where p.TransactionID == _TransactionID
                                     select p.TransactionNo).FirstOrDefault();

                return TransactionNo;
            }

        }

        public List<PRDChemProdReqItem> GetTransactionItemList(long _TransactionID, string _IssueFrom)
        {
                using (var context = new BLC_DEVEntities())
                {
                    var FinalStock = DalChemicalStock.ItemWiseStockInSpecificChemicalStore(Convert.ToByte(_IssueFrom));

                    var RequisitionID = (from r in context.INV_StoreTransRequest.AsEnumerable()
                                         where r.TransactionID == _TransactionID
                                         select r.RequisitionID
                                         ).FirstOrDefault();

                    var Data = (from i in context.INV_StoreTransItem.AsEnumerable()
                                where i.TransactionID == _TransactionID

                                from r in context.PRD_ChemProdReqItem.Where(x => x.RequisitionID == RequisitionID & x.ItemID==i.ItemID).DefaultIfEmpty()

                                join reqsize in context.Sys_Size on (r==null?null:r.PackSize) equals reqsize.SizeID into ReqPackSizes
                                from reqsize in ReqPackSizes.DefaultIfEmpty()

                                join reqsizeunit in context.Sys_Unit on (r == null ? null : r.SizeUnit) equals reqsizeunit.UnitID into ReqPackSizeUnits
                                from reqsizeunit in ReqPackSizeUnits.DefaultIfEmpty()

                                join requnit in context.Sys_Unit on (r == null ? 0 : r.RequisitionUnit) equals requnit.UnitID into ReqUnits
                                from requnit in ReqUnits.DefaultIfEmpty()

                                join s in FinalStock on i.ItemID equals s.ItemID into badhon
                                from item in badhon.DefaultIfEmpty()

                                join su in context.Sys_Unit on (item == null ? 0 : item.UnitID) equals su.UnitID into StockUnits
                                from su in StockUnits.DefaultIfEmpty()

                                join sup in context.Sys_Supplier on (item == null ? 0 : item.SupplierID) equals sup.SupplierID into badhon2
                                from finalitem in badhon2.DefaultIfEmpty()

                                from it in context.Sys_ChemicalItem.Where(x => x.ItemID == i.ItemID).DefaultIfEmpty()
                                from sup in context.Sys_Supplier.Where(x => x.SupplierID == i.SupplierID).DefaultIfEmpty()
                                from iu in context.Sys_Unit.Where(x => x.UnitID == i.TransactionUnit).DefaultIfEmpty() 
                                from issuesize in context.Sys_Size.Where(x => x.SizeID == i.PackSize).DefaultIfEmpty()
                                from issuesizeunit in context.Sys_Unit.Where(x => x.UnitID == i.SizeUnit).DefaultIfEmpty() 

                                select new PRDChemProdReqItem
                                {
                                    TransItemID = i.TransItemID,
                                    ItemID = Convert.ToInt16(i.ItemID),
                                    ItemName = (it == null ? null : it.ItemName),

                                    StockQty = (item == null ? 0 : item.ClosingQty),
                                    StockUnitName = (su == null ? null : su.UnitName),
                                    SupplierID = (finalitem == null ? 0 : finalitem.SupplierID),
                                    SupplierName = (finalitem == null ? null : finalitem.SupplierName),
                                    
                                    PackSizeName = (reqsize == null ? null : reqsize.SizeName),
                                    SizeUnitName = (reqsizeunit == null ? null : reqsizeunit.UnitName),
                                    PackQty = (r == null ? 0 : Convert.ToInt16(r.PackQty)),
                                    RequsitionQty = (r == null ? 0 : r.RequsitionQty),
                                    RequisitionUnitName = (requnit == null ? null : requnit.UnitName),

                                    IssuePackSize = Convert.ToByte(i.PackSize),
                                    IssuePackSizeName = (issuesize == null ? null : issuesize.SizeName),
                                    IssueSizeUnit = Convert.ToByte(i.SizeUnit),
                                    IssueSizeUnitName = (issuesizeunit == null ? null : issuesizeunit.UnitName),
                                    IssuePackQty = Convert.ToInt16(i.PackQty),
                                    IssueQty = Convert.ToDecimal(i.TransactionQty),
                                    IssueUnitName = (iu == null ? null : iu.UnitName),
                                    
                                    ItemSource = DalCommon.ReturnItemSource(i.ItemSource)
                                }).ToList();

                    foreach (var item in Data)
                    {
                        if (item.ItemSource == "From Order")
                        {
                            item.PackSizeName = null;
                            item.SizeUnitName = null;
                            item.PackQty = 0;
                            item.RequsitionQty = 0;
                            item.RequisitionUnitName = null;
                        }
                    }
                    return Data;
                }
        }

        public List<PRDChemProdReqItem> GetTransactionItemListForStockAdjust(long _TransactionID, string _IssueFrom)
        {
            using (var context = new BLC_DEVEntities())
            {
                var StockResult = DalChemicalStock.CombinationWiseStockInSpecificProductionFloor(Convert.ToByte(_IssueFrom));


                var Data = (from i in context.INV_StoreTransItem.AsEnumerable()
                            where i.TransactionID == _TransactionID

                            join s in StockResult on new { ItemID = (i.ItemID== null? 0: i.ItemID), SupplierID = (i.SupplierID==null? 0 : i.SupplierID),
                                UnitID = (i.TransactionUnit==null?0:i.TransactionUnit) } equals
                                new { ItemID = s.ItemID, SupplierID = s.SupplierID, UnitID = s.UnitID } into StockInfo
                            from item in StockInfo.DefaultIfEmpty()

                            join su in context.Sys_Unit on (item == null ? 0 : item.UnitID) equals su.UnitID into StockUnits
                            from su in StockUnits.DefaultIfEmpty()

                            join sup in context.Sys_Supplier on (item == null ? 0 : item.SupplierID) equals sup.SupplierID into Suppliers
                            from sup in Suppliers.DefaultIfEmpty()

                            from it in context.Sys_ChemicalItem.Where(x => x.ItemID == i.ItemID).DefaultIfEmpty()


                            from iu in context.Sys_Unit.Where(x => x.UnitID == i.TransactionUnit).DefaultIfEmpty()


                            select new PRDChemProdReqItem
                            {
                                TransItemID = i.TransItemID,
                                ItemID = i.ItemID,
                                ItemName = (it == null ? null : it.ItemName),

                                StockQty = (item == null ? 0 : item.ClosingQty),
                                StockUnitName = (su == null ? null : su.UnitName),
                                SupplierID = (sup == null ? 0 : sup.SupplierID),
                                SupplierName = (sup == null ? null : sup.SupplierName),
                                IssueQty = i.TransactionQty,
                                IssueUnit = i.TransactionUnit,
                                IssueUnitName = (iu == null ? "" : iu.UnitName)
                            }).ToList();

                return Data;
            }
        }

        //For Search Grid
        public List<INVStoreTrans> GetTransactionInfoForSearch(string _TransactionType)
        {
            using(_context)
            {
                var AllData = (from t in _context.INV_StoreTrans
                               where t.TransactionCategory == "ISU" & t.TransactionType == _TransactionType

                               join r in _context.INV_StoreTransRequest on t.TransactionID equals r.TransactionID into ReqInfo
                               from r in ReqInfo.DefaultIfEmpty()

                               from tf in _context.SYS_Store.Where(x => (x.StoreID).ToString() == t.TransactionFrom).DefaultIfEmpty()
                               from tt in _context.SYS_Store.Where(x => (x.StoreID).ToString() == t.TransactionTo).DefaultIfEmpty()

                               orderby t.TransactionID descending
                               select new INVStoreTrans
                               {
                                   TransactionID = t.TransactionID,
                                   TransactionNo = t.TransactionNo,
                                   TransactionDateTemp = (t.TransactionDate),
                                   TransactionFrom = (tf == null ? null : tf.StoreName),
                                   TransactionTo = (tt == null ? null : tt.StoreName),
                                   RecordStatus = (t.RecordStatus),
                                   ReqFromDateTemp = (r.ReqFromDate),
                                   ReqToDateTemp = (r.ReqToDate),
                                   RequisitionNo= r.RequestNo
                               }).ToList();

                return AllData;
            }
        }

        public int getIssueToProductionRecordCount(string _TransactionType)
        {
            using(BLC_DEVEntities context= new BLC_DEVEntities())
            {
                return context.INV_StoreTrans.Where(t => t.TransactionCategory == "ISU" & t.TransactionType == _TransactionType).Count();
            }
        }

        public INVStoreTrans GetTransactionDetailsAfterSearch(long _TransactionID)
        {
            var model = new INVStoreTrans();

            
            using(_context)
            {
                var RequisitionID = (from r in _context.INV_StoreTransRequest
                                     where r.TransactionID == _TransactionID
                                     select r.RequisitionID
                                     ).FirstOrDefault();

                if(RequisitionID!=null)
                {
                    var TransactionInfo = (from t in _context.INV_StoreTrans.AsEnumerable()
                                           where t.TransactionID == _TransactionID

                                           join req in _context.INV_StoreTransRequest on t.TransactionID equals req.TransactionID into Requests
                                           from req in Requests.DefaultIfEmpty()

                                           //from req in _context.INV_StoreTransRequest.Where(x => x.TransactionID == _TransactionID).DefaultIfEmpty()

                                           join r in _context.PRD_ChemProdReq on RequisitionID equals r.RequisitionID into Requisitions
                                           from r in Requisitions.DefaultIfEmpty()

                                           //from r in _context.PRD_ChemProdReq.Where(x => x.RequisitionID == RequisitionID).DefaultIfEmpty()

                                           join u in _context.Users on (r == null ? null : r.ReqRaisedBy) equals u.UserID into UserInfo
                                           from u in UserInfo.DefaultIfEmpty()

                                           //from u in _context.Users.Where(x => x.UserID == r.ReqRaisedBy).DefaultIfEmpty()
                                           select new INVStoreTrans
                                           {
                                               TransactionID = t.TransactionID,
                                               TransactionNo = t.TransactionNo,
                                               IssueFrom = t.TransactionFrom,
                                               IssueTo = t.TransactionTo,
                                               TransactionDate = (Convert.ToDateTime(t.TransactionDate)).ToString("dd'/'MM'/'yyyy"),
                                               RecordStatus = t.RecordStatus,
                                               RequisitionID = (r == null ? 0 : r.RequisitionID),
                                               RequisitionNo = (r == null ? null : r.RequisitionNo),
                                               RequisitionCategory = (r == null ? null : DalCommon.ReturnRequisitionCategory(r.RequisitionCategory)),
                                               RequisitionType = (r == null ? null : DalCommon.ReturnOrderType(r.RequisitionType)),
                                               RequiredByTime = (r == null ? null : (r.RequiredByTime).ToString()),
                                               ReqRaisedOn = (r == null ? null : (Convert.ToDateTime(r.ReqRaisedOn)).ToString("dd'/'MM'/'yyyy")),
                                               ReqRaisedByName = (u == null ? null : (u.FirstName + " " + u.MiddleName + " " + u.LastName)),
                                               JobOrderNo = (r == null ? null : r.JobOrderNo),
                                               Remark = (req == null ? null : req.Remark),
                                               TransactionType = t.TransactionType

                                           }).FirstOrDefault();


                    model.TransactionInfo = TransactionInfo;

                    var TransactionItemList = GetTransactionItemList(_TransactionID, TransactionInfo.IssueFrom);

                    model.TransactionItemList = TransactionItemList;

                    return model;
                }

                else
                {
                    var TransactionInfo = (from t in _context.INV_StoreTrans.AsEnumerable()
                                           where t.TransactionID == _TransactionID

                                           join req in _context.INV_StoreTransRequest on t.TransactionID equals req.TransactionID into Requests
                                           from req in Requests.DefaultIfEmpty()

                                           //from req in _context.INV_StoreTransRequest.Where(x => x.TransactionID == _TransactionID).DefaultIfEmpty()

                                           select new INVStoreTrans
                                           {
                                               TransactionID = t.TransactionID,
                                               TransactionNo = t.TransactionNo,
                                               IssueFrom = t.TransactionFrom,
                                               IssueTo = t.TransactionTo,
                                               TransactionDate = (Convert.ToDateTime(t.TransactionDate)).ToString("dd'/'MM'/'yyyy"),
                                               RecordStatus = t.RecordStatus,
                                               Remark = (req == null ? null : req.Remark),
                                               ReqToDate = (Convert.ToDateTime(req.ReqToDate)).ToString("dd'/'MM'/'yyyy"),
                                               ReqFromDate = (Convert.ToDateTime(req.ReqFromDate)).ToString("dd'/'MM'/'yyyy"),
                                               TransactionType = t.TransactionType
                                           }).FirstOrDefault();


                    model.TransactionInfo = TransactionInfo;

                    if(TransactionInfo.TransactionType=="ADI")
                    {
                        var TransactionItemList = GetTransactionItemListForStockAdjust(_TransactionID, TransactionInfo.IssueFrom);
                        model.TransactionItemList = TransactionItemList;
                    }
                    else
                    {
                        var TransactionItemList = GetTransactionItemList(_TransactionID, TransactionInfo.IssueFrom);
                        model.TransactionItemList = TransactionItemList;
                    }

                    

                    

                    return model;
                }
            }
        }

        public bool DeleteTransactionItem(string _TransItemID)
        {
            try
            {
                var TransactionItem = (from c in _context.INV_StoreTransItem.AsEnumerable()
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
                var TransRequisition = (from r in _context.INV_StoreTransRequest.AsEnumerable()
                                        where r.TransactionID.ToString() == _TransactionID
                                        select r).FirstOrDefault();

                _context.INV_StoreTransRequest.Remove(TransRequisition);

                var Transaction = (from c in _context.INV_StoreTrans.AsEnumerable()
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

        public bool CheckIssueToProductionTransaction(string _TransactionID, string _CheckComment)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {


                        var TransactionInfo = (from p in _context.INV_StoreTrans.AsEnumerable()
                                               where (p.TransactionID).ToString() == _TransactionID
                                               select p).FirstOrDefault();
                        TransactionInfo.CheckComments = _CheckComment;

                        TransactionInfo.RecordStatus = "CHK";

                        _context.SaveChanges();

                    }
                    Transaction.Complete();
                }

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
            
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                        Msg= DalChemicalStock.SubtractChemicalFromChemicalStore(_TransactionID, _CheckComment, _UserID, "Issue To Production");
                            
                        if (Msg.ReturnId != 0)
                        {
                            Msg= DalChemicalStock.AddChemicalInProductionFloor(_TransactionID, _CheckComment, _UserID, "Issue To Production");
                        }
                        
                        if (Msg.ReturnId == 1)
                        {
                            using (var context = new BLC_DEVEntities())
                            {
                                var RequisitionList = (from r in context.INV_StoreTransRequest
                                                   where r.TransactionID == _TransactionID
                                                   select r).ToList();

                            foreach (var rec in RequisitionList)
                            {
                                var Requisition = (from r in context.PRD_ChemProdReq
                                                   where r.RequisitionID == rec.RequisitionID
                                                   select r).FirstOrDefault();

                                Requisition.RequisitionState = "CMP";
                            }
                            context.SaveChanges();
                            }
                        }
                    Transaction.Complete();
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

        public List<PRDChemProdReqItem> GetStockPackSizeInfo(string _StoreID, int _ItemID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var StockResult = (from p in context.INV_ChemStockSupplier
                                   where (p.StoreID).ToString() == _StoreID && p.ItemID == _ItemID
                                   group p by new
                                   {
                                       p.SupplierID,
                                       p.PackSize,
                                       p.SizeUnit
                                   } into g
                                   select new
                                   {
                                       TransectionID = g.Max(p => p.TransectionID),
                                       PackSize = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.PackSize).FirstOrDefault(),
                                       SizeUnit = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SizeUnit).FirstOrDefault(),
                                       UnitID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault(),
                                       SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                                       PackClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.PackClosingQty).FirstOrDefault(),
                                       ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault(),
                                   }).ToList();


                var Data = (from s in StockResult
                            where s.ClosingQty > 0
                            from si in context.Sys_Size.AsEnumerable().Where(x => x.SizeID == Convert.ToByte(s.PackSize)).DefaultIfEmpty()
                            from su in context.Sys_Unit.AsEnumerable().Where(x => x.UnitID == Convert.ToByte(s.SizeUnit)).DefaultIfEmpty()

                            join stockUnit in context.Sys_Unit on s.UnitID equals stockUnit.UnitID into StockUnits
                            from stockUnit in StockUnits.DefaultIfEmpty()

                            from sup in context.Sys_Supplier.AsEnumerable().Where(x => x.SupplierID == Convert.ToInt16(s.SupplierID)).DefaultIfEmpty()
                            select new PRDChemProdReqItem
                            {
                                PackSize = Convert.ToByte(s.PackSize),
                                PackSizeName = si.SizeName,
                                SizeUnit = Convert.ToByte(s.SizeUnit),
                                SizeUnitName = su.UnitName,
                                SupplierID = s.SupplierID,
                                SupplierName = sup.SupplierName,
                                PackQty = Convert.ToInt32(s.PackClosingQty),
                                StockQty = s.ClosingQty,
                                IssueUnit = Convert.ToByte(s.UnitID),
                                IssueUnitName = (stockUnit == null ? null : stockUnit.UnitName)
                            }).ToList();

                return Data;
            }

        }

        public List<SysChemicalItem> GetAvailableChemicalInStock(byte _RequisitionAt)
        {
            var FinalStock = DalChemicalStock.ItemWiseStockInSpecificChemicalStore(_RequisitionAt);

            var data = (from s in FinalStock
                        where s.ClosingQty > 0
                        join c in _context.Sys_ChemicalItem on s.ItemID equals c.ItemID into Chemicals
                        from c in Chemicals.DefaultIfEmpty()

                        join i in _context.Sys_ItemType on (c.ItemTypeID==null? null: c.ItemTypeID) equals i.ItemTypeID into ItemTypes
                        from i in ItemTypes.DefaultIfEmpty()


                        select new SysChemicalItem
                        {
                            ItemID = Convert.ToInt32(s.ItemID),
                            ItemName = c == null ? null : c.ItemName,
                            //ItemCategory = DalCommon.ReturnChemicalItemCategory(c.ItemCategory),
                            //ItemTypeName = DalCommon.GetItemTypeName(Convert.ToByte(c.ItemTypeID)),
                            ItemCategory=c.ItemCategory,
                            ItemTypeName= (i==null? null: i.ItemTypeName),
                            StockQty = s.ClosingQty
                        }).ToList();

            foreach(var item in data)
            {
                item.ItemCategory = DalCommon.ReturnChemicalItemCategory(item.ItemCategory);
            }

            return data;
        }

    }
}
