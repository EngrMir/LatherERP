using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalInvLeatherTransferRecieve
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long ReceiveID = 0;

        public DalInvLeatherTransferRecieve()
        {
            _context = new BLC_DEVEntities();
        }
        public ValidationMsg Save(InvLeatherTransferRecieve model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.SetBy = userid;
                        Inv_LeatherTransferRecieve tblLeatherTransferRecieve = SetToModelObject(model);
                        _context.Inv_LeatherTransferRecieve.Add(tblLeatherTransferRecieve);
                        _context.SaveChanges();

                        #region Save Detail Records

                        if (model.LeatherTransferReceiveItemList != null)
                        {
                            foreach (InvLeatherTransferReceiveItem objInvLeatherTransferReceiveItem in model.LeatherTransferReceiveItemList)
                            {
                                objInvLeatherTransferReceiveItem.SetBy = userid;
                                objInvLeatherTransferReceiveItem.ReceiveID = tblLeatherTransferRecieve.ReceiveID;
                                Inv_LeatherTransferReceiveItem tblPurchaseYearPeriod =
                                    SetToModelObject(objInvLeatherTransferReceiveItem);
                                _context.Inv_LeatherTransferReceiveItem.Add(tblPurchaseYearPeriod);
                            }
                        }
                        _context.SaveChanges();

                        #endregion

                        tx.Complete();
                        ReceiveID = tblLeatherTransferRecieve.ReceiveID;
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Save Successfully.";
                    }
                }
            }
            catch
            {
                ReceiveID = 0;
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public ValidationMsg Update(InvLeatherTransferRecieve model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        Inv_LeatherTransferRecieve CurrentEntity = SetToModelObject(model);
                        var OriginalEntity = _context.Inv_LeatherTransferRecieve.First(m => m.ReceiveID == model.ReceiveID);

                        OriginalEntity.ReceiveDate = CurrentEntity.ReceiveDate;// Convert.ToDateTime(Convert.ToDateTime(CurrentEntity.ReceiveDate).ToString("dd/MM/yyyy"));
                        OriginalEntity.ReceiveLocation = CurrentEntity.ReceiveLocation;
                        OriginalEntity.SetBy = userid;
                        OriginalEntity.SetOn = DateTime.Now;

                        #region Save Detail Records

                        if (model.LeatherTransferReceiveItemList != null)
                        {
                            foreach (InvLeatherTransferReceiveItem objInvLeatherTransferReceiveItem in model.LeatherTransferReceiveItemList)
                            {
                                if (objInvLeatherTransferReceiveItem.ItemReceiveID == 0)
                                {
                                    objInvLeatherTransferReceiveItem.ReceiveID = model.ReceiveID;
                                    objInvLeatherTransferReceiveItem.SetBy = userid;
                                    Inv_LeatherTransferReceiveItem tblPurchaseYearPeriod =
                                        SetToModelObject(objInvLeatherTransferReceiveItem);
                                    _context.Inv_LeatherTransferReceiveItem.Add(tblPurchaseYearPeriod);
                                }
                                else
                                {
                                    Inv_LeatherTransferReceiveItem CurEntity = SetToModelObject(objInvLeatherTransferReceiveItem);
                                    var OrgEntity = _context.Inv_LeatherTransferReceiveItem.First(m => m.ItemReceiveID == objInvLeatherTransferReceiveItem.ItemReceiveID);

                                    OrgEntity.ReceiveQty = CurEntity.ReceiveQty;
                                    OrgEntity.ReceiveSide = CurEntity.ReceiveSide;
                                    OrgEntity.SetBy = userid;
                                    OrgEntity.SetOn = DateTime.Now;
                                }
                            }
                        }
                        _context.SaveChanges();

                        #endregion

                        tx.Complete();
                        _vmMsg.Type = Enums.MessageType.Update;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                }
            }
            catch
            {
                ReceiveID = 0;
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }

        public ValidationMsg TransferReceiveChecked(int receiveId, string checkComment, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (_context)
                {
                    var OriginalEntity = _context.Inv_LeatherTransferRecieve.First(m => m.ReceiveID == receiveId);

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

        public ValidationMsg StoreTransferReceiveConfirmed(InvLeatherTransferRecieve model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var OriginalEntityRecieve = _context.Inv_LeatherTransferRecieve.First(m => m.ReceiveID == model.ReceiveID);

                        OriginalEntityRecieve.ApprovedBy = userid;
                        OriginalEntityRecieve.ApproveComment = model.ApproveComment;
                        OriginalEntityRecieve.RecordStatus = "CNF";

                        var OriginalEntityIssue = _context.Inv_LeatherIssue.First(m => m.IssueID == model.IssueID);

                        OriginalEntityIssue.ApprovedBy = userid;
                        OriginalEntityIssue.ApproveComment = model.ApproveComment;
                        OriginalEntityIssue.RecordStatus = "RCV";

                        #region Save Detail Records

                        if (model.LeatherTransferReceiveItemList != null)
                        {
                            foreach (InvLeatherTransferReceiveItem objLeatherIssueItem in model.LeatherTransferReceiveItemList)
                            {
                                objLeatherIssueItem.StoreID = model.ReceiveLocation;

                                #region storeReceive

                                #region Supplier_Stock_Update

                                var CheckSupplierStockForRec = (from i in _context.Inv_StockSupplier
                                                                where i.SupplierID == objLeatherIssueItem.SupplierID
                                                                && i.ItemTypeID == objLeatherIssueItem.ItemType
                                                                && i.LeatherType == objLeatherIssueItem.LeatherType
                                                                && i.LeatherStatusID == objLeatherIssueItem.LeatherStatus
                                                                && i.StoreID == objLeatherIssueItem.StoreID
                                                                && i.PurchaseID == objLeatherIssueItem.PurchaseID
                                                                select i).Any();

                                if (!CheckSupplierStockForRec)
                                {

                                    Inv_StockSupplier Entity = new Inv_StockSupplier();

                                    Entity.SupplierID = objLeatherIssueItem.SupplierID;
                                    Entity.StoreID = Convert.ToByte(objLeatherIssueItem.StoreID);
                                    Entity.RefChallanID = objLeatherIssueItem.ChallanID;
                                    Entity.PurchaseID = objLeatherIssueItem.PurchaseID;
                                    Entity.ItemTypeID = objLeatherIssueItem.ItemType;
                                    Entity.LeatherType = objLeatherIssueItem.LeatherType;
                                    Entity.LeatherStatusID = objLeatherIssueItem.LeatherStatus;
                                    Entity.UnitID = objLeatherIssueItem.UnitID;

                                    Entity.OpeningQty = 0;
                                    Entity.ReceiveQty = objLeatherIssueItem.ReceiveQty;
                                    Entity.IssueQty = 0;
                                    Entity.ClosingQty = objLeatherIssueItem.ReceiveQty;

                                    Entity.UpdateReason = "Store Transfer Receive";
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
                                                             && i.StoreID == objLeatherIssueItem.StoreID
                                                             && i.PurchaseID == objLeatherIssueItem.PurchaseID
                                                             orderby i.TransectionID descending
                                                             select i).FirstOrDefault();


                                    Inv_StockSupplier objStockSupplier = new Inv_StockSupplier();

                                    objStockSupplier.SupplierID = objLeatherIssueItem.SupplierID;
                                    objStockSupplier.StoreID = Convert.ToByte(objLeatherIssueItem.StoreID);
                                    objStockSupplier.RefChallanID = objLeatherIssueItem.ChallanID;
                                    objStockSupplier.PurchaseID = objLeatherIssueItem.PurchaseID;
                                    objStockSupplier.ItemTypeID = objLeatherIssueItem.ItemType;
                                    objStockSupplier.LeatherType = objLeatherIssueItem.LeatherType;
                                    objStockSupplier.LeatherStatusID = objLeatherIssueItem.LeatherStatus;
                                    objStockSupplier.UnitID = objLeatherIssueItem.UnitID;

                                    objStockSupplier.OpeningQty = LastSupplierStock.ClosingQty;
                                    objStockSupplier.ReceiveQty = objLeatherIssueItem.ReceiveQty;
                                    objStockSupplier.IssueQty = 0;
                                    objStockSupplier.ClosingQty = LastSupplierStock.ClosingQty + objLeatherIssueItem.ReceiveQty;

                                    objStockSupplier.UpdateReason = "Store Transfer Receive";

                                    _context.Inv_StockSupplier.Add(objStockSupplier);
                                    _context.SaveChanges();
                                }

                                #endregion

                                #region Item_Stock_Update

                                var CheckItemStockForRec = (from i in _context.Inv_StockItem
                                                            where i.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                            i.LeatherType == objLeatherIssueItem.LeatherType &&
                                                            i.LeatherStatus == objLeatherIssueItem.LeatherStatus &&
                                                            i.StoreID == objLeatherIssueItem.StoreID
                                                            select i).Any();

                                if (!CheckItemStockForRec)
                                {
                                    Inv_StockItem objStockItem = new Inv_StockItem();

                                    objStockItem.ItemTypeID = objLeatherIssueItem.ItemType;
                                    objStockItem.LeatherType = objLeatherIssueItem.LeatherType;
                                    objStockItem.LeatherStatus = objLeatherIssueItem.LeatherStatus;
                                    objStockItem.StoreID = Convert.ToByte(objLeatherIssueItem.StoreID);
                                    objStockItem.UnitID = objLeatherIssueItem.UnitID;

                                    objStockItem.OpeningQty = 0;
                                    objStockItem.ReceiveQty = objLeatherIssueItem.ReceiveQty;
                                    objStockItem.IssueQty = 0;
                                    objStockItem.ClosingQty = objLeatherIssueItem.ReceiveQty;

                                    _context.Inv_StockItem.Add(objStockItem);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    var LastItemInfo = (from i in _context.Inv_StockItem
                                                        where i.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                        i.LeatherType == objLeatherIssueItem.LeatherType &&
                                                        i.LeatherStatus == objLeatherIssueItem.LeatherStatus &&
                                                        i.StoreID == objLeatherIssueItem.StoreID
                                                        orderby i.TransectionID descending
                                                        select i).FirstOrDefault();

                                    Inv_StockItem objStockItem = new Inv_StockItem();

                                    objStockItem.ItemTypeID = LastItemInfo.ItemTypeID;
                                    objStockItem.LeatherType = LastItemInfo.LeatherType;
                                    objStockItem.LeatherStatus = LastItemInfo.LeatherStatus;
                                    objStockItem.StoreID = LastItemInfo.StoreID;
                                    objStockItem.UnitID = objLeatherIssueItem.UnitID;

                                    objStockItem.OpeningQty = LastItemInfo.ClosingQty;
                                    objStockItem.ReceiveQty = objLeatherIssueItem.ReceiveQty;
                                    objStockItem.IssueQty = 0;
                                    objStockItem.ClosingQty = LastItemInfo.ClosingQty + objLeatherIssueItem.ReceiveQty;

                                    _context.Inv_StockItem.Add(objStockItem);
                                    _context.SaveChanges();
                                }

                                #endregion

                                #region Daily_Stock_Update

                                var currentDate = System.DateTime.Now.Date;

                                var CheckDateForRec = (from ds in _context.Inv_StockDaily
                                                       where ds.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                       ds.StoreID == objLeatherIssueItem.StoreID
                                                        && ds.LeatherStatus == objLeatherIssueItem.LeatherStatus
                                                        && ds.LeatherType == objLeatherIssueItem.LeatherType && ds.StockDate == currentDate
                                                       select ds).Any();

                                if (CheckDateForRec)
                                {
                                    var CurrentItem = (from ds in _context.Inv_StockDaily
                                                       where ds.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                       ds.StoreID == objLeatherIssueItem.StoreID
                                                        && ds.LeatherStatus == objLeatherIssueItem.LeatherStatus
                                                        && ds.LeatherType == objLeatherIssueItem.LeatherType && ds.StockDate == currentDate
                                                       select ds).FirstOrDefault();

                                    CurrentItem.DailyReceiveQty = CurrentItem.DailyReceiveQty + objLeatherIssueItem.ReceiveQty;
                                    CurrentItem.ClosingQty = CurrentItem.ClosingQty + objLeatherIssueItem.ReceiveQty;
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    var PreviousRecord = (from ds in _context.Inv_StockDaily
                                                          where ds.ItemTypeID == objLeatherIssueItem.ItemType &&
                                                          ds.StoreID == objLeatherIssueItem.StoreID
                                                          && ds.LeatherStatus == objLeatherIssueItem.LeatherStatus &&
                                                          ds.LeatherType == objLeatherIssueItem.LeatherType
                                                          orderby ds.TransectionID descending
                                                          select ds).FirstOrDefault();

                                    Inv_StockDaily objStockDaily = new Inv_StockDaily();

                                    objStockDaily.ItemTypeID = objLeatherIssueItem.ItemType;
                                    objStockDaily.StoreID = Convert.ToByte(objLeatherIssueItem.StoreID);
                                    objStockDaily.UnitID = objLeatherIssueItem.UnitID;
                                    objStockDaily.LeatherStatus = objLeatherIssueItem.LeatherStatus;
                                    objStockDaily.LeatherType = objLeatherIssueItem.LeatherType;

                                    objStockDaily.OpeningQty = (PreviousRecord == null ? 0 : PreviousRecord.ClosingQty);
                                    objStockDaily.DailyReceiveQty = objLeatherIssueItem.ReceiveQty;
                                    objStockDaily.DailyIssueQty = 0;
                                    objStockDaily.ClosingQty = objStockDaily.OpeningQty + objLeatherIssueItem.ReceiveQty;

                                    objStockDaily.StockDate = currentDate;

                                    _context.Inv_StockDaily.Add(objStockDaily);
                                    _context.SaveChanges();
                                }

                                #endregion

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

        public Inv_StockSupplier SetToModelObjectStockSupplier(InvLeatherTransferReceiveItem model)
        {
            Inv_StockSupplier Entity = new Inv_StockSupplier();

            Entity.SupplierID = model.SupplierID;
            Entity.StoreID = model.StoreID;
            Entity.RefChallanID = model.ChallanID;
            Entity.PurchaseID = model.PurchaseID;
            Entity.ItemTypeID = model.ItemType;
            Entity.LeatherType = model.LeatherType;
            Entity.LeatherStatusID = model.LeatherStatus;
            Entity.UnitID = model.UnitID;
            Entity.OpeningQty = 0;// model.StockQty;
            Entity.ReceiveQty = model.ReceiveQty;
            Entity.IssueQty = 0;// model.IssueQty;
            Entity.ClosingQty = Entity.OpeningQty + model.ReceiveQty - Entity.IssueQty;
            Entity.UpdateReason = "store Transfer Receive";

            return Entity;
        }

        public Inv_StockItem SetToModelObjectStockItem(InvLeatherTransferReceiveItem model)
        {
            Inv_StockItem Entity = new Inv_StockItem();

            Entity.StoreID = model.StoreID;
            Entity.ItemTypeID = model.ItemType;
            Entity.LeatherType = model.LeatherType;
            Entity.LeatherStatus = model.LeatherStatus;
            Entity.UnitID = model.UnitID;
            Entity.OpeningQty = 0;
            Entity.ReceiveQty = model.ReceiveQty;
            Entity.IssueQty = 0;// model.IssueQty;
            Entity.ClosingQty = Entity.OpeningQty + model.ReceiveQty;// - model.IssueQty;

            return Entity;
        }

        public Inv_StockDaily SetToModelObjectDailyStock(InvLeatherTransferReceiveItem model)
        {
            Inv_StockDaily Entity = new Inv_StockDaily();

            Entity.StockDate = System.DateTime.Now.Date;
            Entity.StoreID = model.StoreID;
            Entity.ItemTypeID = model.ItemType;
            Entity.LeatherType = model.LeatherType;
            Entity.LeatherStatus = model.LeatherStatus;
            Entity.UnitID = model.UnitID;
            Entity.OpeningQty = 0;
            Entity.DailyReceiveQty = model.ReceiveQty;
            Entity.DailyIssueQty = 0;//model.IssueQty;
            Entity.ClosingQty = Entity.OpeningQty + model.ReceiveQty;// - model.IssueQty;

            return Entity;
        }

        public long GetReceiveID()
        {
            return ReceiveID;
        }
        public Inv_LeatherTransferRecieve SetToModelObject(InvLeatherTransferRecieve model)
        {
            Inv_LeatherTransferRecieve Entity = new Inv_LeatherTransferRecieve();

            Entity.ReceiveID = model.ReceiveID;
            Entity.IssueID = model.IssueID;
            Entity.ReceiveDate = DalCommon.SetDate(model.ReceiveDate);
            Entity.ReceiveLocation = model.ReceiveLocation;
            Entity.PurchaseYear = model.PurchaseYear;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public Inv_LeatherTransferReceiveItem SetToModelObject(InvLeatherTransferReceiveItem model)
        {
            Inv_LeatherTransferReceiveItem Entity = new Inv_LeatherTransferReceiveItem();

            Entity.ItemReceiveID = model.ItemReceiveID;
            Entity.ReceiveID = model.ReceiveID;// Convert.ToInt16(_context.Inv_LeatherTransferRecieve.DefaultIfEmpty().Max(m => m.IssueID == null ? 0 : m.IssueID));
            Entity.SupplierID = model.SupplierID;
            Entity.ChallanID = model.ChallanID;
            Entity.PurchaseID = model.PurchaseID;
            Entity.ItemType = model.ItemType;
            Entity.LeatherType = model.LeatherType;
            Entity.LeatherStatus = model.LeatherStatus;
            Entity.IssueQty = model.IssueQty;
            Entity.ReceiveQty = model.ReceiveQty;
            Entity.UnitID = model.UnitID;
            Entity.IssueSide = model.IssueSide;
            Entity.ReceiveSide = model.ReceiveSide;
            Entity.Remarks = model.Remarks;
            Entity.RecordStatus = "NCF";//model.RecordStatus;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;
            return Entity;
        }

        public List<InvLeatherIssueItem> GetTransferReceiveItemFromIssueList(long issueid)
        {
            List<Inv_LeatherIssueItem> searchList = _context.Inv_LeatherIssueItem.Where(m => m.IssueID == issueid).OrderByDescending(m => m.IssueID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvLeatherIssueItem>();
        }

        public InvLeatherIssueItem SetToBussinessObject(Inv_LeatherIssueItem Entity)
        {
            InvLeatherIssueItem Model = new InvLeatherIssueItem();

            Model.ItemIssueID = Entity.ItemIssueID;
            Model.SupplierID = Entity.SupplierID;
            Model.SupplierName = _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierName;
            Model.ChallanID = Entity.ChallanID;
            Model.ChallanNo = Entity.ChallanID == null ? "" : _context.Prq_PurchaseChallan.Where(m => m.ChallanID == Entity.ChallanID).FirstOrDefault().ChallanNo;
            Model.PurchaseID = Entity.PurchaseID;
            Model.PurchaseNo = _context.Prq_Purchase.Where(m => m.PurchaseID == Entity.PurchaseID).FirstOrDefault().PurchaseNo;
            Model.ItemType = Entity.ItemType;
            Model.ItemTypeName = _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemType).FirstOrDefault().ItemTypeName;
            Model.LeatherType = Entity.LeatherType;
            Model.LeatherTypeName = _context.Sys_LeatherType.Where(m => m.LeatherTypeID == Entity.LeatherType).FirstOrDefault().LeatherTypeName;
            Model.LeatherStatus = Entity.LeatherStatus;
            Model.LeatherStatusName = _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatus).FirstOrDefault().LeatherStatusName;
            Model.IssueQty = Entity.IssueQty;
            Model.IssueSide = Entity.IssueSide;

            Model.ReceiveQty = Entity.IssueQty;
            Model.ReceiveSide = Entity.IssueSide;

            Model.UnitID = Entity.UnitID;
            Model.UnitName = _context.Sys_Unit.Where(m => m.UnitID == Entity.UnitID).FirstOrDefault().UnitName;
            Model.Remarks = Entity.Remarks;

            return Model;
        }

        public List<InvLeatherTransferReceiveItem> GetTransferReceiveItemList(long receiveID)
        {
            List<Inv_LeatherTransferReceiveItem> searchList = _context.Inv_LeatherTransferReceiveItem.Where(m => m.ReceiveID == receiveID).OrderByDescending(m => m.ItemReceiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvLeatherTransferReceiveItem>();
        }

        public InvLeatherTransferReceiveItem SetToBussinessObject(Inv_LeatherTransferReceiveItem Entity)
        {
            InvLeatherTransferReceiveItem Model = new InvLeatherTransferReceiveItem();

            Model.ItemReceiveID = Entity.ItemReceiveID;
            Model.SupplierID = Entity.SupplierID;
            //Model.Supplier = _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierName;
            Model.SupplierName = _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierName;
            Model.ChallanID = Entity.ChallanID;
            Model.ChallanNo = Entity.ChallanID == null ? "" : _context.Prq_PurchaseChallan.Where(m => m.ChallanID == Entity.ChallanID).FirstOrDefault().ChallanNo;
            Model.PurchaseID = Entity.PurchaseID;
            Model.PurchaseNo = _context.Prq_Purchase.Where(m => m.PurchaseID == Entity.PurchaseID).FirstOrDefault().PurchaseNo;
            Model.ItemType = Entity.ItemType;
            Model.ItemTypeName = _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemType).FirstOrDefault().ItemTypeName;
            Model.LeatherType = Entity.LeatherType;
            Model.LeatherTypeName = _context.Sys_LeatherType.Where(m => m.LeatherTypeID == Entity.LeatherType).FirstOrDefault().LeatherTypeName;
            Model.LeatherStatus = Entity.LeatherStatus;
            Model.LeatherStatusName = _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatus).FirstOrDefault().LeatherStatusName;
            Model.IssueQty = Entity.IssueQty;
            Model.IssueSide = Entity.IssueSide;
            Model.ReceiveQty = Entity.ReceiveQty;
            Model.ReceiveSide = Entity.ReceiveSide;
            Model.UnitID = Entity.UnitID;
            Model.UnitName = _context.Sys_Unit.Where(m => m.UnitID == Entity.UnitID).FirstOrDefault().UnitName;
            Model.Remarks = Entity.Remarks;

            return Model;
        }

        public List<InvLeatherTransferRecieve> GetTransferReceiveList()
        {
            List<Inv_LeatherTransferRecieve> searchList = _context.Inv_LeatherTransferRecieve.OrderByDescending(m => m.ReceiveID).ToList();
            //List<Inv_LeatherTransferRecieve> searchList = _context.Inv_LeatherTransferRecieve.Where(m => m.RecordStatus == "NCF").OrderByDescending(m=>m.ReceiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvLeatherTransferRecieve>();
        }

        public InvLeatherTransferRecieve SetToBussinessObject(Inv_LeatherTransferRecieve Entity)
        {
            InvLeatherTransferRecieve Model = new InvLeatherTransferRecieve();

            Model.ReceiveID = Entity.ReceiveID;
            Model.IssueID = Entity.IssueID;
            Model.ReceiveDate = Entity.ReceiveDate.ToString("dd/MM/yyyy");
            Model.ReceiveLocation = Entity.ReceiveLocation;
            Model.ReceiveLocationName = Entity.ReceiveLocation == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.ReceiveLocation).FirstOrDefault().StoreName;
            Model.PurchaseYear = Entity.PurchaseYear;
            Model.RecordStatus = Entity.RecordStatus;

            return Model;
        }

        public ValidationMsg DeletedReceive(int receiveId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var issueItemList = _context.Inv_LeatherTransferReceiveItem.Where(m => m.ReceiveID == receiveId).ToList();

                if (issueItemList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteElement = _context.Inv_LeatherTransferRecieve.First(m => m.ReceiveID == receiveId);
                    _context.Inv_LeatherTransferRecieve.Remove(deleteElement);

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

        public ValidationMsg DeletedReceiveItem(int itemReceiveId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var deleteElement = _context.Inv_LeatherTransferReceiveItem.First(m => m.ItemReceiveID == itemReceiveId);
                _context.Inv_LeatherTransferReceiveItem.Remove(deleteElement);

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
    }
}
