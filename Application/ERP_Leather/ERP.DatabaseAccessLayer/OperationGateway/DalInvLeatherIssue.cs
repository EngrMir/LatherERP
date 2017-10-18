using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseUtility;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.BaseModel;
using System.Transactions;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalInvLeatherIssue
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long IssueID = 0;
        public DalInvLeatherIssue()
        {
            _context = new BLC_DEVEntities();
        }
        public ValidationMsg Save(InvLeatherIssue model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.SetBy = userid;
                        Inv_LeatherIssue tblLeatherIssue = SetToModelObject(model);
                        _context.Inv_LeatherIssue.Add(tblLeatherIssue);
                        _context.SaveChanges();

                        #region Save Detail Records

                        if (model.LeatherIssueItemList != null)
                        {
                            foreach (InvLeatherIssueItem objInvLeatherIssueItem in model.LeatherIssueItemList)
                            {
                                objInvLeatherIssueItem.SetBy = userid;
                                objInvLeatherIssueItem.IssueID = tblLeatherIssue.IssueID;
                                Inv_LeatherIssueItem tblPurchaseYearPeriod =
                                    SetToModelObject(objInvLeatherIssueItem);
                                _context.Inv_LeatherIssueItem.Add(tblPurchaseYearPeriod);
                            }
                        }
                        _context.SaveChanges();

                        #endregion

                        tx.Complete();
                        IssueID = tblLeatherIssue.IssueID;
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Saved Successfully.";
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public ValidationMsg Update(InvLeatherIssue model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        Inv_LeatherIssue CurrentEntity = SetToModelObject(model);
                        var OriginalEntity = _context.Inv_LeatherIssue.First(m => m.IssueID == model.IssueID);

                        OriginalEntity.IssueDate = CurrentEntity.IssueDate;// Convert.ToDateTime(CurrentEntity.IssueDate).Date;
                        OriginalEntity.IssueFor = CurrentEntity.IssueFor;
                        OriginalEntity.IssueRef = CurrentEntity.IssueRef;
                        OriginalEntity.IssueFrom = CurrentEntity.IssueFrom;
                        OriginalEntity.IssueTo = CurrentEntity.IssueTo;
                        OriginalEntity.JobOrderNo = CurrentEntity.JobOrderNo;
                        OriginalEntity.PurchaseYear = CurrentEntity.PurchaseYear;
                        OriginalEntity.SetBy = userid;
                        OriginalEntity.SetOn = DateTime.Now;

                        #region Save Detail Records

                        if (model.LeatherIssueItemList != null)
                        {
                            foreach (InvLeatherIssueItem objInvLeatherIssueItem in model.LeatherIssueItemList)
                            {
                                if (objInvLeatherIssueItem.ItemIssueID == 0)
                                {
                                    objInvLeatherIssueItem.IssueID = model.IssueID;
                                    Inv_LeatherIssueItem tblPurchaseYearPeriod =
                                        SetToModelObject(objInvLeatherIssueItem);
                                    _context.Inv_LeatherIssueItem.Add(tblPurchaseYearPeriod);
                                }
                                else
                                {
                                    Inv_LeatherIssueItem CurEntity = SetToModelObject(objInvLeatherIssueItem);
                                    var OrgEntity = _context.Inv_LeatherIssueItem.First(m => m.ItemIssueID == objInvLeatherIssueItem.ItemIssueID);

                                    OrgEntity.SupplierID = CurEntity.SupplierID;
                                    OrgEntity.ChallanID = CurEntity.ChallanID;
                                    OrgEntity.ItemType = CurEntity.ItemType;
                                    OrgEntity.LeatherType = CurEntity.LeatherType;
                                    OrgEntity.LeatherStatus = CurEntity.LeatherStatus;
                                    OrgEntity.IssueQty = CurEntity.IssueQty;
                                    OrgEntity.UnitID = CurEntity.UnitID;
                                    OrgEntity.IssueSide = CurEntity.IssueSide;
                                    OrgEntity.Remarks = CurEntity.Remarks;
                                    OrgEntity.SetBy = userid;
                                    OrgEntity.SetOn = DateTime.Now;
                                }
                            }
                        }
                        _context.SaveChanges();

                        #endregion

                        tx.Complete();
                        IssueID = model.IssueID;
                        _vmMsg.Type = Enums.MessageType.Update;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }

        public ValidationMsg LeatherIssueChecked(int issueId, string checkComment, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (_context)
                {
                    var OriginalEntity = _context.Inv_LeatherIssue.First(m => m.IssueID == issueId);

                    OriginalEntity.CheckedBy = userid;
                    OriginalEntity.CheckComment = checkComment;
                    OriginalEntity.RecordStatus = "CHK";

                    _context.SaveChanges();

                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Checked Successfully.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Checked.";
            }
            return _vmMsg;
        }

        public ValidationMsg LeatherIssueConfirmed(InvLeatherIssue model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region Update Record Status

                        var OriginalEntity = _context.Inv_LeatherIssue.First(m => m.IssueID == model.IssueID);

                        OriginalEntity.ApprovedBy = userid;
                        OriginalEntity.ApproveComment = model.ApproveComment;
                        OriginalEntity.RecordStatus = "CNF";

                        #endregion

                        #region Update Store

                        if (model.LeatherIssueItemList != null)
                        {
                            foreach (InvLeatherIssueItem objLeatherIssueItem in model.LeatherIssueItemList)
                            {
                                objLeatherIssueItem.StoreIdIssueFrom = model.IssueFrom;
                                objLeatherIssueItem.StoreIdIssueTo = model.IssueTo;

                                #region Store Issue

                                #region Supplier_Stock_Update

                                var CheckSupplierStock = (from i in _context.Inv_StockSupplier
                                                          where i.SupplierID == objLeatherIssueItem.SupplierID
                                                          && i.ItemTypeID == objLeatherIssueItem.ItemType
                                                          && i.LeatherType == objLeatherIssueItem.LeatherType
                                                          && i.LeatherStatusID == objLeatherIssueItem.LeatherStatus
                                                          && i.StoreID == objLeatherIssueItem.StoreIdIssueFrom
                                                          && i.PurchaseID == objLeatherIssueItem.PurchaseID
                                                          select i).Any();

                                if (!CheckSupplierStock)
                                {
                                    Inv_StockSupplier tblStockSupplier = SetToModelObjectStockSupplier(objLeatherIssueItem);
                                    _context.Inv_StockSupplier.Add(tblStockSupplier);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    var LastSupplierStock = (from i in _context.Inv_StockSupplier
                                                             where i.SupplierID == objLeatherIssueItem.SupplierID
                                                             && i.ItemTypeID == objLeatherIssueItem.ItemType
                                                             && i.LeatherType == objLeatherIssueItem.LeatherType
                                                             && i.LeatherStatusID == objLeatherIssueItem.LeatherStatus
                                                             && i.StoreID == objLeatherIssueItem.StoreIdIssueFrom
                                                             && i.PurchaseID == objLeatherIssueItem.PurchaseID
                                                             orderby i.TransectionID descending
                                                             select i).FirstOrDefault();


                                    Inv_StockSupplier objStockSupplier = new Inv_StockSupplier();

                                    objStockSupplier.SupplierID = objLeatherIssueItem.SupplierID;
                                    objStockSupplier.StoreID = Convert.ToByte(objLeatherIssueItem.StoreIdIssueFrom);
                                    objStockSupplier.RefChallanID = objLeatherIssueItem.ChallanID;
                                    objStockSupplier.PurchaseID = objLeatherIssueItem.PurchaseID;
                                    objStockSupplier.ItemTypeID = objLeatherIssueItem.ItemType;
                                    objStockSupplier.LeatherType = objLeatherIssueItem.LeatherType;
                                    objStockSupplier.LeatherStatusID = objLeatherIssueItem.LeatherStatus;
                                    objStockSupplier.UnitID = objLeatherIssueItem.UnitID;

                                    objStockSupplier.OpeningQty = LastSupplierStock.ClosingQty;
                                    objStockSupplier.ReceiveQty = 0;
                                    objStockSupplier.IssueQty = objLeatherIssueItem.IssueQty;
                                    objStockSupplier.ClosingQty = LastSupplierStock.ClosingQty - objLeatherIssueItem.IssueQty;
                                    objStockSupplier.UpdateReason = "issue";

                                    _context.Inv_StockSupplier.Add(objStockSupplier);
                                    _context.SaveChanges();
                                }

                                #endregion

                                #region Item_Stock_Update

                                var CheckItemStock = (from i in _context.Inv_StockItem
                                                      where i.ItemTypeID == objLeatherIssueItem.ItemType
                                                      && i.LeatherType == objLeatherIssueItem.LeatherType
                                                      && i.LeatherStatus == objLeatherIssueItem.LeatherStatus
                                                      && i.StoreID == objLeatherIssueItem.StoreIdIssueFrom
                                                      select i).Any();

                                if (!CheckItemStock)
                                {
                                    Inv_StockItem objStockItem = new Inv_StockItem();

                                    objStockItem.ItemTypeID = objLeatherIssueItem.ItemType;
                                    objStockItem.LeatherType = objLeatherIssueItem.LeatherType;
                                    objStockItem.LeatherStatus = objLeatherIssueItem.LeatherStatus;
                                    objStockItem.StoreID = Convert.ToByte(objLeatherIssueItem.StoreIdIssueFrom);
                                    objStockItem.UnitID = objLeatherIssueItem.UnitID;

                                    objStockItem.OpeningQty = objLeatherIssueItem.StockQty;
                                    objStockItem.IssueQty = objLeatherIssueItem.IssueQty;
                                    objStockItem.ReceiveQty = 0;
                                    objStockItem.ClosingQty = objLeatherIssueItem.StockQty - objLeatherIssueItem.IssueQty;

                                    _context.Inv_StockItem.Add(objStockItem);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    var LastItemInfo = (from i in _context.Inv_StockItem
                                                        where i.ItemTypeID == objLeatherIssueItem.ItemType
                                                        && i.LeatherType == objLeatherIssueItem.LeatherType
                                                        && i.LeatherStatus == objLeatherIssueItem.LeatherStatus
                                                        && i.StoreID == objLeatherIssueItem.StoreIdIssueFrom
                                                        orderby i.TransectionID descending
                                                        select i).FirstOrDefault();


                                    Inv_StockItem objStockItem = new Inv_StockItem();


                                    objStockItem.ItemTypeID = LastItemInfo.ItemTypeID;
                                    objStockItem.LeatherType = LastItemInfo.LeatherType;
                                    objStockItem.LeatherStatus = LastItemInfo.LeatherStatus;
                                    objStockItem.StoreID = LastItemInfo.StoreID;
                                    objStockItem.UnitID = objLeatherIssueItem.UnitID;

                                    objStockItem.OpeningQty = LastItemInfo.ClosingQty;
                                    objStockItem.ReceiveQty = 0;
                                    objStockItem.IssueQty = objLeatherIssueItem.IssueQty;
                                    objStockItem.ClosingQty = LastItemInfo.ClosingQty - objLeatherIssueItem.IssueQty;

                                    _context.Inv_StockItem.Add(objStockItem);
                                    _context.SaveChanges();
                                }

                                #endregion

                                #region Daily_Stock_Update

                                var currentDate = System.DateTime.Now.Date;

                                var CheckDate = (from ds in _context.Inv_StockDaily
                                                 where ds.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                 ds.StoreID == objLeatherIssueItem.StoreIdIssueFrom
                                                  && ds.LeatherStatus == objLeatherIssueItem.LeatherStatus &&
                                                  ds.LeatherType == objLeatherIssueItem.LeatherType && ds.StockDate == currentDate
                                                 select ds).Any();

                                if (CheckDate)
                                {
                                    var CurrentItem = (from ds in _context.Inv_StockDaily
                                                       where ds.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                       ds.StoreID == objLeatherIssueItem.StoreIdIssueFrom
                                                        && ds.LeatherStatus == objLeatherIssueItem.LeatherStatus &&
                                                        ds.LeatherType == objLeatherIssueItem.LeatherType && ds.StockDate == currentDate
                                                       select ds).FirstOrDefault();

                                    CurrentItem.DailyIssueQty = CurrentItem.DailyIssueQty + objLeatherIssueItem.IssueQty;
                                    CurrentItem.ClosingQty = CurrentItem.ClosingQty - objLeatherIssueItem.IssueQty;
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    var PreviousRecord = (from ds in _context.Inv_StockDaily
                                                          where ds.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                          ds.StoreID == objLeatherIssueItem.StoreIdIssueFrom
                                                          && ds.LeatherStatus == objLeatherIssueItem.LeatherStatus &&
                                                          ds.LeatherType == objLeatherIssueItem.LeatherType
                                                          orderby ds.TransectionID descending
                                                          select ds).FirstOrDefault();

                                    Inv_StockDaily objStockDaily = new Inv_StockDaily();

                                    objStockDaily.ItemTypeID = objLeatherIssueItem.ItemType;
                                    objStockDaily.StoreID = Convert.ToByte(objLeatherIssueItem.StoreIdIssueFrom);
                                    objStockDaily.UnitID = objLeatherIssueItem.UnitID;
                                    objStockDaily.LeatherStatus = objLeatherIssueItem.LeatherStatus;
                                    objStockDaily.LeatherType = objLeatherIssueItem.LeatherType;

                                    objStockDaily.OpeningQty = (PreviousRecord == null ? 0 : PreviousRecord.ClosingQty);
                                    objStockDaily.DailyReceiveQty = 0;
                                    objStockDaily.DailyIssueQty = objLeatherIssueItem.IssueQty;
                                    objStockDaily.ClosingQty = objStockDaily.OpeningQty - objLeatherIssueItem.IssueQty;

                                    objStockDaily.StockDate = currentDate;

                                    _context.Inv_StockDaily.Add(objStockDaily);
                                    _context.SaveChanges();
                                }

                                #endregion

                                #endregion

                                #region storeReceive

                                //if (model.IssueFor == "Production" || model.IssueFor == "Job Order")
                                if (model.IssueFor == "Job Order")
                                {
                                    #region Supplier_Stock_Update

                                    var CheckSupplierStockForRec = (from i in _context.Inv_StockSupplier
                                                                    where i.SupplierID == objLeatherIssueItem.SupplierID
                                                                    && i.ItemTypeID == objLeatherIssueItem.ItemType
                                                                    && i.LeatherType == objLeatherIssueItem.LeatherType
                                                                    && i.LeatherStatusID == objLeatherIssueItem.LeatherStatus
                                                                    && i.StoreID == objLeatherIssueItem.StoreIdIssueTo
                                                                    && i.PurchaseID == objLeatherIssueItem.PurchaseID
                                                                    select i).Any();

                                    if (!CheckSupplierStockForRec)
                                    {

                                        Inv_StockSupplier Entity = new Inv_StockSupplier();

                                        Entity.SupplierID = objLeatherIssueItem.SupplierID;
                                        Entity.StoreID = Convert.ToByte(objLeatherIssueItem.StoreIdIssueTo);
                                        Entity.RefChallanID = objLeatherIssueItem.ChallanID;
                                        Entity.PurchaseID = objLeatherIssueItem.PurchaseID;
                                        Entity.ItemTypeID = objLeatherIssueItem.ItemType;
                                        Entity.LeatherType = objLeatherIssueItem.LeatherType;
                                        Entity.LeatherStatusID = objLeatherIssueItem.LeatherStatus;
                                        Entity.UnitID = objLeatherIssueItem.UnitID;

                                        Entity.OpeningQty = 0;
                                        Entity.ReceiveQty = objLeatherIssueItem.IssueQty;
                                        Entity.IssueQty = 0;
                                        Entity.ClosingQty = objLeatherIssueItem.IssueQty;

                                        Entity.UpdateReason = "Job Order Receive";
                                        _context.Inv_StockSupplier.Add(Entity);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        var LastSupplierStock = (from i in _context.Inv_StockSupplier
                                                                 where i.SupplierID == objLeatherIssueItem.SupplierID
                                                                 && i.ItemTypeID == objLeatherIssueItem.ItemType
                                                                 && i.LeatherType == objLeatherIssueItem.LeatherType
                                                                 && i.LeatherStatusID == objLeatherIssueItem.LeatherStatus
                                                                 && i.StoreID == objLeatherIssueItem.StoreIdIssueTo
                                                                 && i.PurchaseID == objLeatherIssueItem.PurchaseID
                                                                 orderby i.TransectionID descending
                                                                 select i).FirstOrDefault();


                                        Inv_StockSupplier objStockSupplier = new Inv_StockSupplier();

                                        objStockSupplier.SupplierID = objLeatherIssueItem.SupplierID;
                                        objStockSupplier.StoreID = Convert.ToByte(objLeatherIssueItem.StoreIdIssueTo);
                                        objStockSupplier.RefChallanID = objLeatherIssueItem.ChallanID;
                                        objStockSupplier.PurchaseID = objLeatherIssueItem.PurchaseID;
                                        objStockSupplier.ItemTypeID = objLeatherIssueItem.ItemType;
                                        objStockSupplier.LeatherType = objLeatherIssueItem.LeatherType;
                                        objStockSupplier.LeatherStatusID = objLeatherIssueItem.LeatherStatus;
                                        objStockSupplier.UnitID = objLeatherIssueItem.UnitID;

                                        objStockSupplier.OpeningQty = LastSupplierStock.ClosingQty;
                                        objStockSupplier.ReceiveQty = objLeatherIssueItem.IssueQty;
                                        objStockSupplier.IssueQty = 0;
                                        objStockSupplier.ClosingQty = LastSupplierStock.ClosingQty + objLeatherIssueItem.IssueQty;

                                        objStockSupplier.UpdateReason = "Job Order Receive";

                                        _context.Inv_StockSupplier.Add(objStockSupplier);
                                        _context.SaveChanges();
                                    }

                                    #endregion

                                    #region Item_Stock_Update

                                    var CheckItemStockForRec = (from i in _context.Inv_StockItem
                                                                where i.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                                i.LeatherType == objLeatherIssueItem.LeatherType &&
                                                                i.LeatherStatus == objLeatherIssueItem.LeatherStatus &&
                                                                i.StoreID == objLeatherIssueItem.StoreIdIssueTo
                                                                select i).Any();

                                    if (!CheckItemStockForRec)
                                    {
                                        Inv_StockItem objStockItem = new Inv_StockItem();

                                        objStockItem.ItemTypeID = objLeatherIssueItem.ItemType;
                                        objStockItem.LeatherType = objLeatherIssueItem.LeatherType;
                                        objStockItem.LeatherStatus = objLeatherIssueItem.LeatherStatus;
                                        objStockItem.StoreID = Convert.ToByte(objLeatherIssueItem.StoreIdIssueTo);
                                        objStockItem.UnitID = objLeatherIssueItem.UnitID;

                                        objStockItem.OpeningQty = 0;
                                        objStockItem.ReceiveQty = objLeatherIssueItem.IssueQty;
                                        objStockItem.IssueQty = 0;
                                        objStockItem.ClosingQty = objLeatherIssueItem.IssueQty;

                                        _context.Inv_StockItem.Add(objStockItem);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        var LastItemInfo = (from i in _context.Inv_StockItem
                                                            where i.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                            i.LeatherType == objLeatherIssueItem.LeatherType &&
                                                            i.LeatherStatus == objLeatherIssueItem.LeatherStatus &&
                                                            i.StoreID == objLeatherIssueItem.StoreIdIssueTo
                                                            orderby i.TransectionID descending
                                                            select i).FirstOrDefault();


                                        Inv_StockItem objStockItem = new Inv_StockItem();


                                        objStockItem.ItemTypeID = LastItemInfo.ItemTypeID;
                                        objStockItem.LeatherType = LastItemInfo.LeatherType;
                                        objStockItem.LeatherStatus = LastItemInfo.LeatherStatus;
                                        objStockItem.StoreID = LastItemInfo.StoreID;
                                        objStockItem.UnitID = objLeatherIssueItem.UnitID;

                                        objStockItem.OpeningQty = LastItemInfo.ClosingQty;
                                        objStockItem.ReceiveQty = objLeatherIssueItem.IssueQty;
                                        objStockItem.IssueQty = 0;
                                        objStockItem.ClosingQty = LastItemInfo.ClosingQty + objLeatherIssueItem.IssueQty;

                                        _context.Inv_StockItem.Add(objStockItem);
                                        _context.SaveChanges();
                                    }

                                    #endregion

                                    #region Daily_Stock_Update

                                    //var currentDate = System.DateTime.Now.Date;

                                    var CheckDateForRec = (from ds in _context.Inv_StockDaily
                                                           where ds.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                           ds.StoreID == objLeatherIssueItem.StoreIdIssueTo
                                                            && ds.LeatherStatus == objLeatherIssueItem.LeatherStatus
                                                            && ds.LeatherType == objLeatherIssueItem.LeatherType && ds.StockDate == currentDate
                                                           select ds).Any();

                                    if (CheckDateForRec)
                                    {
                                        var CurrentItem = (from ds in _context.Inv_StockDaily
                                                           where ds.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                           ds.StoreID == objLeatherIssueItem.StoreIdIssueTo
                                                            && ds.LeatherStatus == objLeatherIssueItem.LeatherStatus
                                                            && ds.LeatherType == objLeatherIssueItem.LeatherType && ds.StockDate == currentDate
                                                           select ds).FirstOrDefault();

                                        CurrentItem.DailyReceiveQty = CurrentItem.DailyReceiveQty + objLeatherIssueItem.IssueQty;
                                        CurrentItem.ClosingQty = CurrentItem.ClosingQty + objLeatherIssueItem.IssueQty;
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        var PreviousRecord = (from ds in _context.Inv_StockDaily
                                                              where ds.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                              ds.StoreID == objLeatherIssueItem.StoreIdIssueTo
                                                              && ds.LeatherStatus == objLeatherIssueItem.LeatherStatus &&
                                                              ds.LeatherType == objLeatherIssueItem.LeatherType
                                                              orderby ds.TransectionID descending
                                                              select ds).FirstOrDefault();

                                        Inv_StockDaily objStockDaily = new Inv_StockDaily();

                                        objStockDaily.ItemTypeID = objLeatherIssueItem.ItemType;
                                        objStockDaily.StoreID = Convert.ToByte(objLeatherIssueItem.StoreIdIssueTo);
                                        objStockDaily.UnitID = objLeatherIssueItem.UnitID;
                                        objStockDaily.LeatherStatus = objLeatherIssueItem.LeatherStatus;
                                        objStockDaily.LeatherType = objLeatherIssueItem.LeatherType;

                                        objStockDaily.OpeningQty = (PreviousRecord == null ? 0 : PreviousRecord.ClosingQty);
                                        objStockDaily.DailyReceiveQty = objLeatherIssueItem.IssueQty;
                                        objStockDaily.DailyIssueQty = 0;
                                        objStockDaily.ClosingQty = objStockDaily.OpeningQty + objLeatherIssueItem.IssueQty;

                                        objStockDaily.StockDate = currentDate;

                                        _context.Inv_StockDaily.Add(objStockDaily);
                                        _context.SaveChanges();
                                    }

                                    #endregion
                                }

                                #endregion

                                #region Update Record Status

                                var OriginalIssueItemEntity = _context.Inv_LeatherIssueItem.First(m => m.ItemIssueID == objLeatherIssueItem.ItemIssueID);

                                OriginalIssueItemEntity.RecordStatus = "CNF";

                                #endregion
                            }
                        }

                        #endregion

                        _context.SaveChanges();

                        tx.Complete();
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Confirmed Successfully.";
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Confirmed.";
            }
            return _vmMsg;
        }

        public List<PrqPurchaseChallan> GetChallanListInfo(string supplierId)
        {
            var queryString = "SELECT distinct ChallanID,ChallanNo,ChallanCategory FROM [dbo].[Prq_PurchaseChallan]" +
                              " WHERE ChallanID IN (SELECT RefChallanID from [dbo].[Inv_StockSupplier]" +
                              " WHERE SupplierID = '" + supplierId + "')";
            var iChallanList = _context.Database.SqlQuery<PrqPurchaseChallan>(queryString);
            return iChallanList.ToList();
        }

        public long GetIssueID()
        {
            return IssueID;
        }
        public Inv_LeatherIssue SetToModelObject(InvLeatherIssue model)
        {
            Inv_LeatherIssue Entity = new Inv_LeatherIssue();

            Entity.IssueID = model.IssueID;
            Entity.IssueDate = DalCommon.SetDate(model.IssueDate);// Convert.ToDateTime(model.IssueDate).Date;
            Entity.IssueFor = model.IssueFor;
            Entity.IssueRef = model.IssueRef;
            Entity.IssueFrom = model.IssueFrom;
            Entity.IssueTo = model.IssueTo;
            Entity.JobOrderNo = model.JobOrderNo;
            Entity.PurchaseYear = model.PurchaseYear;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public Inv_StockSupplier SetToModelObjectStockSupplier(InvLeatherIssueItem model)
        {
            Inv_StockSupplier Entity = new Inv_StockSupplier();

            Entity.SupplierID = model.SupplierID;
            Entity.StoreID = Convert.ToByte(model.StoreIdIssueFrom);
            Entity.RefChallanID = model.ChallanID;
            Entity.ItemTypeID = model.ItemType;
            Entity.LeatherType = model.LeatherType;
            Entity.LeatherStatusID = model.LeatherStatus;
            Entity.UnitID = model.UnitID;
            Entity.OpeningQty = model.StockQty;
            Entity.ReceiveQty = 0;
            Entity.IssueQty = model.IssueQty;
            Entity.ClosingQty = model.StockQty - model.IssueQty;
            Entity.UpdateReason = "issue";
            return Entity;
        }

        public Inv_StockItem SetToModelObjectStockItem(InvLeatherIssueItem model)
        {
            Inv_StockItem Entity = new Inv_StockItem();

            Entity.StoreID = Convert.ToByte(model.StoreIdIssueFrom);
            Entity.ItemTypeID = model.ItemType;
            Entity.LeatherType = model.LeatherType;
            Entity.LeatherStatus = model.LeatherStatus;
            Entity.UnitID = model.UnitID;
            Entity.OpeningQty = model.StockQty;
            Entity.ReceiveQty = 0;
            Entity.IssueQty = model.IssueQty;
            Entity.ClosingQty = model.StockQty - model.IssueQty;

            return Entity;
        }

        public Inv_StockDaily SetToModelObjectDailyStock(InvLeatherIssueItem model)
        {
            Inv_StockDaily Entity = new Inv_StockDaily();

            Entity.StockDate = System.DateTime.Now.Date;
            Entity.StoreID = Convert.ToByte(model.StoreIdIssueFrom);
            Entity.ItemTypeID = model.ItemType;
            Entity.LeatherType = model.LeatherType;
            Entity.LeatherStatus = model.LeatherStatus;
            Entity.UnitID = model.UnitID;
            Entity.OpeningQty = model.StockQty;
            Entity.DailyReceiveQty = 0;
            Entity.DailyIssueQty = model.IssueQty;
            Entity.ClosingQty = model.StockQty - model.IssueQty;

            return Entity;
        }

        public Inv_StockSupplier SetToModelObjectStockSupplierReceive(InvLeatherIssueItem model)
        {
            Inv_StockSupplier Entity = new Inv_StockSupplier();

            Entity.SupplierID = model.SupplierID;
            Entity.StoreID = Convert.ToByte(model.StoreIdIssueTo);
            Entity.RefChallanID = model.ChallanID;
            Entity.ItemTypeID = model.ItemType;
            Entity.LeatherType = model.LeatherType;
            Entity.LeatherStatusID = model.LeatherStatus;
            Entity.UnitID = model.UnitID;
            Entity.OpeningQty = 0;
            Entity.ReceiveQty = model.IssueQty;
            Entity.IssueQty = 0;
            Entity.ClosingQty = model.IssueQty;
            Entity.UpdateReason = "issue";
            return Entity;
        }

        public Inv_StockItem SetToModelObjectStockItemReceive(InvLeatherIssueItem model)
        {
            Inv_StockItem Entity = new Inv_StockItem();

            Entity.StoreID = Convert.ToByte(model.StoreIdIssueTo);
            Entity.ItemTypeID = model.ItemType;
            Entity.LeatherType = model.LeatherType;
            Entity.LeatherStatus = model.LeatherStatus;
            Entity.UnitID = model.UnitID;
            Entity.OpeningQty = 0;
            Entity.ReceiveQty = model.IssueQty;
            Entity.IssueQty = 0;
            Entity.ClosingQty = model.IssueQty;

            return Entity;
        }

        public Inv_StockDaily SetToModelObjectDailyStockReceive(InvLeatherIssueItem model)
        {
            Inv_StockDaily Entity = new Inv_StockDaily();

            Entity.StockDate = System.DateTime.Now.Date;
            Entity.StoreID = Convert.ToByte(model.StoreIdIssueTo);
            Entity.ItemTypeID = model.ItemType;
            Entity.LeatherType = model.LeatherType;
            Entity.LeatherStatus = model.LeatherStatus;
            Entity.UnitID = model.UnitID;
            Entity.OpeningQty = 0;
            Entity.DailyReceiveQty = model.IssueQty;
            Entity.DailyIssueQty = 0;
            Entity.ClosingQty = model.IssueQty;

            return Entity;
        }

        public Inv_LeatherIssueItem SetToModelObject(InvLeatherIssueItem model)
        {
            Inv_LeatherIssueItem Entity = new Inv_LeatherIssueItem();

            Entity.ItemIssueID = model.ItemIssueID;
            Entity.IssueID = model.IssueID;// Convert.ToInt16(_context.Inv_LeatherIssue.DefaultIfEmpty().Max(m => m.IssueID == null ? 0 : m.IssueID));
            Entity.SupplierID = model.SupplierID;
            Entity.ChallanID = model.ChallanID;
            Entity.PurchaseID = model.PurchaseID;//_context.Prq_PurchaseChallan.Where(m => m.ChallanID == model.ChallanID).FirstOrDefault().PurchaseID;
            Entity.ItemType = model.ItemType;
            Entity.LeatherType = model.LeatherType;
            Entity.LeatherStatus = model.LeatherStatus;
            Entity.IssueQty = model.IssueQty;
            Entity.UnitID = model.UnitID;
            Entity.IssueSide = model.IssueSide;
            Entity.Remarks = model.Remarks;
            Entity.RecordStatus = "NCF";//model.RecordStatus;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;
            return Entity;
        }

        public List<InvLeatherIssue> GetLeatherIssueList()
        {
            List<Inv_LeatherIssue> searchList = _context.Inv_LeatherIssue.OrderByDescending(m => m.IssueID).ToList();
            //List<Inv_LeatherIssue> searchList = _context.Inv_LeatherIssue.Where(m => m.RecordStatus == "NCF").OrderByDescending(m => m.IssueID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvLeatherIssue>();
        }

        public List<InvLeatherIssue> GetIssueForReiceveList()
        {
            List<Inv_LeatherIssue> searchList = _context.Inv_LeatherIssue.Where(m => m.RecordStatus == "CNF" && m.IssueFor == "Store Transfer").OrderByDescending(m => m.IssueID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvLeatherIssue>();
        }

        public List<InvLeatherIssue> GetIssueForReiceveList(string issueid)
        {
            int issid = Convert.ToInt32(issueid);
            List<Inv_LeatherIssue> searchList = _context.Inv_LeatherIssue.Where(m => m.IssueID == issid).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvLeatherIssue>();
        }

        public InvLeatherIssue SetToBussinessObject(Inv_LeatherIssue Entity)
        {
            InvLeatherIssue Model = new InvLeatherIssue();

            Model.IssueID = Entity.IssueID;
            Model.IssueDate = Entity.IssueDate.Date.ToString("dd/MM/yyyy");
            Model.IssueRef = Entity.IssueRef;
            Model.IssueFor = Entity.IssueFor;
            //switch (Entity.IssueFor)
            //{
            //    case "Leather":
            //        Model.IssueForName = "Store Transfer";
            //        break;
            //    case "Production":
            //        Model.IssueForName = "Production";
            //        break;
            //    case "Job Order":
            //        Model.IssueForName = "Job Order";
            //        break;
            //}
            Model.IssueFrom = Entity.IssueFrom;
            Model.IssueFromName = Entity.IssueFrom == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.IssueFrom).FirstOrDefault().StoreName;
            Model.IssueTo = Entity.IssueTo;
            Model.IssueToName = Entity.IssueTo == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.IssueTo).FirstOrDefault().StoreName;
            Model.JobOrderNo = Entity.JobOrderNo;
            Model.JobOrderNo = Entity.JobOrderNo;
            Model.PurchaseYear = Entity.PurchaseYear;
            Model.RecordStatus = Entity.RecordStatus;
            switch (Entity.RecordStatus)
            {
                case "NCF":
                    Model.RecordStatusName = "Not Confirmed";
                    break;
                case "CNF":
                    Model.RecordStatusName = "Confirmed";
                    break;
                case "APV":
                    Model.RecordStatusName = "Approved";
                    break;
                case "RCV":
                    Model.RecordStatusName = "Received";
                    break;
                default:
                    Model.RecordStatusName = "";
                    break;
            }

            return Model;
        }

        public List<SysStore> GetIssueFromAndToList(string storeType)
        {
            var storeCategory = "";
            switch (storeType)
            {
                case "Store Transfer":
                    storeType = "Raw Hide";
                    storeCategory = "Leather";
                    break;
                case "Job Order":
                    storeCategory = "Leather";
                    break;
            }
            List<SYS_Store> searchList = _context.SYS_Store.Where(m => m.StoreCategory == storeCategory && m.StoreType == storeType).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysStore>();
        }

        public SysStore SetToBussinessObject(SYS_Store Entity)
        {
            SysStore Model = new SysStore();

            Model.StoreID = Entity.StoreID;
            Model.StoreName = Entity.StoreName;

            return Model;
        }

        public ValidationMsg DeletedIssue(int issueId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var issueItemList = _context.Inv_LeatherIssueItem.Where(m => m.IssueID == issueId).ToList();

                if (issueItemList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteElement = _context.Inv_LeatherIssue.First(m => m.IssueID == issueId);
                    _context.Inv_LeatherIssue.Remove(deleteElement);

                    _context.SaveChanges();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        public ValidationMsg DeletedIssueItem(int itemIssueId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var deleteElement = _context.Inv_LeatherIssueItem.First(m => m.ItemIssueID == itemIssueId);
                _context.Inv_LeatherIssueItem.Remove(deleteElement);

                _context.SaveChanges();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Deleted Successfully.";
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }
        public List<string> GetSupplierListForSearch()
        {
            var supplierList = new List<string>();
            foreach (var billPaymentSupplier in _context.Inv_StockSupplier.Select(m => m.SupplierID).Distinct().ToList())
            {
                supplierList.Add(_context.Sys_Supplier.Where(m => m.SupplierID == billPaymentSupplier).FirstOrDefault().SupplierName);
            }
            return supplierList;
        }
    }
}
