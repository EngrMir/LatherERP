using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Transactions;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalLoanReturnReceive
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long TransactionID = 0;
        public string TransactionNo = string.Empty;

        public DalLoanReturnReceive()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(INVStoreTrans model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                if (model.InvStoreTransRequestList != null)
                {
                    if (model.InvStoreTransItemList != null)
                    {
                        using (var tx = new TransactionScope())
                        {
                            using (_context)
                            {
                                model.TransactionNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                                if (model.TransactionNo != null)
                                {
                                    #region Receive

                                    INV_StoreTrans tblChemLocalPurcRecv = SetToModelObject(model, userid);
                                    _context.INV_StoreTrans.Add(tblChemLocalPurcRecv);
                                    _context.SaveChanges();

                                    #region Save Po Records

                                    if (model.InvStoreTransRequestList != null)
                                    {
                                        foreach (
                                            INVStoreTransRequest objInvStoreTransRequest in
                                                model.InvStoreTransRequestList)
                                        {
                                            objInvStoreTransRequest.TransactionID = tblChemLocalPurcRecv.TransactionID;
                                            objInvStoreTransRequest.TransRequestNo = DalCommon.GetPreDefineNextCodeByUrl("IssueToProduction/IssueToProduction");

                                            INV_StoreTransRequest tblInvStoreTransRequest =
                                                SetToModelObject(objInvStoreTransRequest, userid);
                                            _context.INV_StoreTransRequest.Add(tblInvStoreTransRequest);
                                            _context.SaveChanges();

                                            #region Save Pl Item List

                                            if (model.InvStoreTransItemList != null)
                                            {
                                                foreach (
                                                    INVStoreTransItem objInvStoreTransItem in
                                                        model.InvStoreTransItemList)
                                                {
                                                    objInvStoreTransItem.TransRequestID =
                                                        tblInvStoreTransRequest.TransRequestID;
                                                    objInvStoreTransItem.TransactionID = tblChemLocalPurcRecv.TransactionID;

                                                    INV_StoreTransItem tblChemLocalPurcRecvItem =
                                                        SetToModelObject(objInvStoreTransItem, userid);
                                                    _context.INV_StoreTransItem.Add(tblChemLocalPurcRecvItem);
                                                }
                                            }

                                            #endregion

                                            #region Save Challan List

                                            if (model.InvStoreTransChallanList != null)
                                            {
                                                foreach (
                                                    INVStoreTransChallan objInvStoreTransChallan in
                                                        model.InvStoreTransChallanList)
                                                {

                                                    //objInvStoreTransChallan.TransRequestID =
                                                    //    tblInvStoreTransRequest.TransRequestID;
                                                    objInvStoreTransChallan.TransactionID =
                                                        tblChemLocalPurcRecv.TransactionID;
                                                    objInvStoreTransChallan.ChallanDate =
                                                        objInvStoreTransChallan.ChallanDate.Contains("/")
                                                            ? objInvStoreTransChallan.ChallanDate
                                                            : Convert.ToDateTime(
                                                                objInvStoreTransChallan.ChallanDate)
                                                                .ToString("dd/MM/yyyy");
                                                    objInvStoreTransChallan.TransChallanNo = DalCommon.GetPreDefineNextCodeByUrl("ChemicalConsumption/ChemConsumption");
                                                    INV_StoreTransChallan tblChemLocalPurcRecvChallan =
                                                        SetToModelObject(objInvStoreTransChallan, userid);
                                                    _context.INV_StoreTransChallan.Add(
                                                        tblChemLocalPurcRecvChallan);
                                                }
                                            }

                                            #endregion
                                        }
                                    }
                                    _context.SaveChanges();

                                    #endregion

                                    tx.Complete();
                                    TransactionID = tblChemLocalPurcRecv.TransactionID;
                                    TransactionNo = model.TransactionNo;

                                    #endregion

                                    _vmMsg.Type = Enums.MessageType.Success;
                                    _vmMsg.Msg = "Saved Successfully.";
                                }
                                else
                                {
                                    _vmMsg.Type = Enums.MessageType.Error;
                                    _vmMsg.Msg = "TransactionNo Predefine Value not Found.";
                                }
                            }
                        }
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Please Enter Chemical item to receive.";
                    }
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "There is no packing list to receive.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public ValidationMsg Update(INVStoreTrans model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        INV_StoreTrans CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.INV_StoreTrans.First(m => m.TransactionID == model.TransactionID);

                        OriginalEntity.TransactionFrom = CurrentEntity.TransactionFrom;
                        OriginalEntity.TransactionTo = CurrentEntity.TransactionTo;
                        OriginalEntity.TransactionDate = CurrentEntity.TransactionDate;
                        OriginalEntity.FromSource = CurrentEntity.FromSource;
                        OriginalEntity.Remarks = CurrentEntity.Remarks;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #region Save Period Records

                        if (model.InvStoreTransRequestList != null)
                        {
                            if (model.InvStoreTransItemList != null)
                            {
                                foreach (
                                    INVStoreTransItem objInvStoreTransItem in
                                        model.InvStoreTransItemList)
                                {

                                    if (objInvStoreTransItem.TransItemID == 0)
                                    {
                                        objInvStoreTransItem.TransactionID = model.TransactionID;
                                        INV_StoreTransItem tblChemLocalPurcRecvItem =
                                            SetToModelObject(objInvStoreTransItem, userid);
                                        _context.INV_StoreTransItem.Add(tblChemLocalPurcRecvItem);
                                    }
                                    else
                                    {
                                        INV_StoreTransItem CurEntity = SetToModelObject(
                                            objInvStoreTransItem, userid);
                                        var OrgEntity =
                                            _context.INV_StoreTransItem.First(
                                                m => m.TransItemID == objInvStoreTransItem.TransItemID);

                                        OrgEntity.TransRequestID = CurEntity.TransRequestID;
                                        OrgEntity.SupplierID = CurEntity.SupplierID;
                                        OrgEntity.PackSize = CurEntity.PackSize;
                                        OrgEntity.SizeUnit = CurEntity.SizeUnit;
                                        OrgEntity.PackQty = CurEntity.PackQty;
                                        OrgEntity.TransactionQty = CurEntity.TransactionQty;
                                        OrgEntity.TransactionUnit = CurEntity.TransactionUnit;
                                        OrgEntity.ManufacturerID = CurEntity.ManufacturerID;
                                        OrgEntity.ModifiedBy = userid;
                                        OrgEntity.ModifiedOn = DateTime.Now;
                                    }
                                }
                            }

                            #region Save challan List

                            if (model.InvStoreTransChallanList != null)
                            {
                                foreach (INVStoreTransChallan objInvStoreTransChallan in model.InvStoreTransChallanList)
                                {
                                    if (objInvStoreTransChallan.TransChallanID == 0)
                                    {

                                        objInvStoreTransChallan.TransactionID = model.TransactionID;
                                        objInvStoreTransChallan.ChallanDate =
                                            objInvStoreTransChallan.ChallanDate.Contains("/")
                                                ? objInvStoreTransChallan.ChallanDate
                                                : Convert.ToDateTime(objInvStoreTransChallan.ChallanDate)
                                                    .ToString("dd/MM/yyyy");
                                        objInvStoreTransChallan.TransChallanNo = DalCommon.GetPreDefineNextCodeByUrl("ChemicalConsumption/ChemConsumption");
                                        INV_StoreTransChallan tblChemLocalPurcRecvChallan =
                                            SetToModelObject(objInvStoreTransChallan, userid);
                                        _context.INV_StoreTransChallan.Add(tblChemLocalPurcRecvChallan);
                                    }
                                    else
                                    {
                                        objInvStoreTransChallan.ChallanDate =
                                            objInvStoreTransChallan.ChallanDate.Contains("/")
                                                ? objInvStoreTransChallan.ChallanDate
                                                : Convert.ToDateTime(objInvStoreTransChallan.ChallanDate)
                                                    .ToString("dd/MM/yyyy");
                                        INV_StoreTransChallan CurrEntity = SetToModelObject(objInvStoreTransChallan, userid);
                                        var OrgrEntity =
                                            _context.INV_StoreTransChallan.First(
                                                m => m.TransChallanID == objInvStoreTransChallan.TransChallanID);

                                        //OrgrEntity.TransChallanNo = CurrEntity.TransChallanNo;
                                        OrgrEntity.RefChallanNo = CurrEntity.RefChallanNo;
                                        OrgrEntity.ChallanDate = CurrEntity.ChallanDate;
                                        OrgrEntity.CarringCost = CurrEntity.CarringCost;
                                        OrgrEntity.LaborCost = CurrEntity.LaborCost;
                                        OrgrEntity.OtherCost = CurrEntity.OtherCost;
                                        OrgrEntity.Currency = CurrEntity.Currency;

                                        OrgrEntity.Remark = CurrEntity.Remark;
                                        OrgrEntity.ModifiedBy = userid;
                                        OrgrEntity.ModifiedOn = DateTime.Now;
                                    }
                                }
                            }

                            #endregion
                        }

                        _context.SaveChanges();

                        #endregion

                        tx.Complete();
                        _vmMsg.Type = Enums.MessageType.Update;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                }

                #endregion
            }

            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }

        public long GetReceiveID()
        {
            return TransactionID;
        }

        public string GetReceiveNo()
        {
            return TransactionNo;
        }

        public INV_StoreTrans SetToModelObject(INVStoreTrans model, int userid)
        {
            INV_StoreTrans Entity = new INV_StoreTrans();

            Entity.TransactionID = model.TransactionID;
            Entity.TransactionNo = model.TransactionNo;
            Entity.TransactionDate = DalCommon.SetDate(model.TransactionDate);
            Entity.TransactionCategory = "RCV";
            Entity.TransactionType = "LRR";
            Entity.FromSource = model.FromSource;
            Entity.TransactionFrom = model.TransactionFrom;
            Entity.TransactionTo = model.TransactionTo;
            Entity.RefTransactionID = null;
            Entity.RefTransactionNo = null;
            Entity.TransactionState = "FLR";
            Entity.TransactionStatus = "TRI";
            Entity.RecordStatus = "NCF";
            Entity.Remarks = model.Remark;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public INV_StoreTransRequest SetToModelObject(INVStoreTransRequest model, int userid)
        {
            INV_StoreTransRequest Entity = new INV_StoreTransRequest();

            Entity.TransRequestID = model.TransRequestID;
            Entity.TransRequestNo = model.TransRequestNo;
            Entity.TransactionID = model.TransactionID;
            Entity.RequestID = model.RequestID;
            Entity.RequestNo = model.RequestNo;
            Entity.TransMethod = model.TransMethod; ;
            Entity.Remark = model.Remark;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public INV_StoreTransItem SetToModelObject(INVStoreTransItem model, int userid)
        {
            INV_StoreTransItem Entity = new INV_StoreTransItem();

            Entity.TransItemID = model.TransItemID;
            Entity.TransRequestID = model.TransRequestID;
            Entity.TransactionID = model.TransactionID;
            Entity.ItemID = model.ItemID;
            Entity.SupplierID = model.SupplierID;
            Entity.ManufacturerID = model.ManufacturerID;
            if (string.IsNullOrEmpty(model.PackSizeName))
                Entity.PackSize = null;
            else
                Entity.PackSize = Convert.ToByte(_context.Sys_Size.Where(m => m.SizeName == model.PackSizeName).FirstOrDefault().SizeID);
            if (string.IsNullOrEmpty(model.SizeUnitName))
                Entity.SizeUnit = null;
            else
                Entity.SizeUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.SizeUnitName).FirstOrDefault().UnitID);
            Entity.PackQty = model.PackQty;


            if (string.IsNullOrEmpty(model.RefPackSizeName))
                Entity.RefPackSize = null;
            else
                Entity.RefPackSize = Convert.ToByte(_context.Sys_Size.Where(m => m.SizeName == model.RefPackSizeName).FirstOrDefault().SizeID);
            if (string.IsNullOrEmpty(model.RefSizeUnitName))
                Entity.RefSizeUnit = null;
            else
                Entity.RefSizeUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.RefSizeUnitName).FirstOrDefault().UnitID);
            Entity.RefPackQty = model.RefPackQty;

            Entity.TransactionQty = model.TransactionQty;
            if (string.IsNullOrEmpty(model.TransactionUnitName))
                Entity.TransactionUnit = null;
            else
                Entity.TransactionUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.TransactionUnitName).FirstOrDefault().UnitID);
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public INV_StoreTransChallan SetToModelObject(INVStoreTransChallan model, int userid)
        {
            INV_StoreTransChallan Entity = new INV_StoreTransChallan();

            Entity.TransChallanID = model.TransChallanID;
            Entity.TransactionID = model.TransactionID;
            Entity.TransChallanNo = model.TransChallanNo;
            Entity.RefChallanNo = model.RefChallanNo;

            if (model.ChallanDate != null)
            {
                Entity.ChallanDate = DalCommon.SetDate(model.ChallanDate);
            }

            Entity.CarringCost = model.CarringCost;
            Entity.LaborCost = model.LaborCost;
            Entity.OtherCost = model.OtherCost;
            //Entity.Currency = model.Currency;

            Entity.Remark = model.Remark;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public ValidationMsg ConfirmChemicalPurchase(INVStoreTrans model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {
                        var OriginalEntity = _context.INV_StoreTrans.First(m => m.TransactionID == model.TransactionID);

                        OriginalEntity.RecordStatus = "CNF";

                        byte? storeId = null;

                        if (!string.IsNullOrEmpty(model.TransactionTo))
                            storeId = Convert.ToByte(model.TransactionTo);

                        if (model.InvStoreTransItemList != null)
                        {
                            foreach (var chemicalItem in model.InvStoreTransItemList)
                            {
                                byte? unitId = null;
                                if (!string.IsNullOrEmpty(chemicalItem.TransactionUnitName))
                                    unitId = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == chemicalItem.TransactionUnitName).FirstOrDefault().UnitID);

                                byte? packSize = null;
                                if (!string.IsNullOrEmpty(chemicalItem.PackSizeName))
                                    packSize = Convert.ToByte(_context.Sys_Size.Where(m => m.SizeName == chemicalItem.PackSizeName).FirstOrDefault().SizeID);

                                byte? sizeUnit = null;
                                if (!string.IsNullOrEmpty(chemicalItem.SizeUnitName))
                                    sizeUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == chemicalItem.SizeUnitName).FirstOrDefault().UnitID);

                                var currentDate = DateTime.Now.Date;

                                #region Daily_Stock_Update

                                var CheckDate = (from ds in _context.INV_ChemStockDaily
                                                 where ds.StockDate == currentDate
                                                       && ds.StoreID == storeId
                                                       && ds.ItemID == chemicalItem.ItemID
                                                       && ds.UnitID == unitId
                                                       && ds.PackSize == packSize
                                                       && ds.SizeUnit == sizeUnit
                                                 select ds).Any();

                                if (CheckDate)
                                {
                                    var CurrentItem = (from ds in _context.INV_ChemStockDaily
                                                       where ds.StockDate == currentDate
                                                             && ds.StoreID == storeId
                                                             && ds.ItemID == chemicalItem.ItemID
                                                             && ds.UnitID == unitId
                                                             && ds.PackSize == packSize
                                                             && ds.SizeUnit == sizeUnit
                                                       select ds).FirstOrDefault();

                                    CurrentItem.ReceiveQty = CurrentItem.ReceiveQty + chemicalItem.TransactionQty;
                                    CurrentItem.ClosingQty = CurrentItem.ClosingQty + chemicalItem.TransactionQty;

                                    CurrentItem.PackReceiveQty = CurrentItem.PackReceiveQty + chemicalItem.PackQty;
                                    CurrentItem.PackClosingQty = CurrentItem.PackClosingQty + chemicalItem.PackQty;

                                    CurrentItem.Remark = "Loan Return Receive";

                                    _context.SaveChanges();
                                }
                                else
                                {
                                    var PreviousRecord = (from ds in _context.INV_ChemStockDaily
                                                          where ds.StoreID == storeId
                                                             && ds.ItemID == chemicalItem.ItemID
                                                             && ds.UnitID == unitId
                                                             && ds.PackSize == packSize
                                                             && ds.SizeUnit == sizeUnit
                                                          orderby ds.TransectionID descending
                                                          select ds).FirstOrDefault();

                                    INV_ChemStockDaily objStockDaily = new INV_ChemStockDaily();

                                    objStockDaily.StockDate = currentDate;

                                    objStockDaily.StoreID = Convert.ToByte(storeId);
                                    objStockDaily.ItemID = chemicalItem.ItemID;
                                    objStockDaily.UnitID = unitId;
                                    objStockDaily.PackSize = Convert.ToByte(packSize);
                                    objStockDaily.SizeUnit = Convert.ToByte(sizeUnit);

                                    objStockDaily.OpeningQty = (PreviousRecord == null ? 0 : PreviousRecord.ClosingQty);
                                    objStockDaily.ReceiveQty = chemicalItem.TransactionQty;
                                    objStockDaily.IssueQty = 0;
                                    objStockDaily.ClosingQty = objStockDaily.OpeningQty + chemicalItem.TransactionQty;

                                    objStockDaily.PackOpeningQty = (PreviousRecord == null ? 0 : PreviousRecord.PackClosingQty);
                                    objStockDaily.PackReceiveQty = chemicalItem.PackQty;
                                    objStockDaily.PackIssueQty = 0;
                                    objStockDaily.PackClosingQty = objStockDaily.PackOpeningQty +
                                    chemicalItem.PackQty;

                                    objStockDaily.SetBy = userid;
                                    objStockDaily.SetOn = System.DateTime.Now;
                                    objStockDaily.Remark = "Loan Return Receive";

                                    _context.INV_ChemStockDaily.Add(objStockDaily);
                                    _context.SaveChanges();
                                }

                                #endregion

                                #region Supplier_Stock_Update

                                var CheckSupplierStock = (from ds in _context.INV_ChemStockSupplier
                                                          where ds.SupplierID == chemicalItem.SupplierID
                                                            && ds.StoreID == storeId
                                                            && ds.ItemID == chemicalItem.ItemID
                                                            && ds.UnitID == unitId
                                                            && ds.PackSize == packSize
                                                            && ds.SizeUnit == sizeUnit
                                                          select ds).Any();

                                if (!CheckSupplierStock)
                                {
                                    INV_ChemStockSupplier objStockSupplier = new INV_ChemStockSupplier();

                                    objStockSupplier.SupplierID = Convert.ToInt32(chemicalItem.SupplierID);

                                    objStockSupplier.StoreID = Convert.ToByte(storeId);
                                    objStockSupplier.ItemID = chemicalItem.ItemID;
                                    objStockSupplier.UnitID = unitId;
                                    objStockSupplier.PackSize = Convert.ToByte(packSize);
                                    objStockSupplier.SizeUnit = Convert.ToByte(sizeUnit);

                                    objStockSupplier.OpeningQty = 0;
                                    objStockSupplier.ReceiveQty = chemicalItem.TransactionQty;
                                    objStockSupplier.IssueQty = 0;
                                    objStockSupplier.ClosingQty = chemicalItem.TransactionQty;

                                    objStockSupplier.PackReceiveQty = chemicalItem.PackQty;
                                    objStockSupplier.PackOpeningQty = 0;
                                    objStockSupplier.PackIssueQty = 0;
                                    objStockSupplier.PackClosingQty = chemicalItem.PackQty;

                                    objStockSupplier.SetBy = userid;
                                    objStockSupplier.SetOn = System.DateTime.Now;
                                    objStockSupplier.Remark = "Loan Return Receive";

                                    _context.INV_ChemStockSupplier.Add(objStockSupplier);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    var LastSupplierStock = (from ds in _context.INV_ChemStockSupplier
                                                             where ds.SupplierID == chemicalItem.SupplierID
                                                                && ds.StoreID == storeId
                                                                && ds.ItemID == chemicalItem.ItemID
                                                                && ds.UnitID == unitId
                                                                && ds.PackSize == packSize
                                                                && ds.SizeUnit == sizeUnit
                                                             orderby ds.TransectionID descending
                                                             select ds).FirstOrDefault();

                                    INV_ChemStockSupplier objStockSupplier = new INV_ChemStockSupplier();

                                    objStockSupplier.SupplierID = Convert.ToInt32(chemicalItem.SupplierID);

                                    objStockSupplier.StoreID = Convert.ToByte(storeId);
                                    objStockSupplier.ItemID = chemicalItem.ItemID;
                                    objStockSupplier.UnitID = unitId;
                                    objStockSupplier.PackSize = Convert.ToByte(packSize);
                                    objStockSupplier.SizeUnit = Convert.ToByte(sizeUnit);

                                    objStockSupplier.OpeningQty = LastSupplierStock.ClosingQty;
                                    objStockSupplier.ReceiveQty = chemicalItem.TransactionQty;
                                    objStockSupplier.IssueQty = 0;
                                    objStockSupplier.ClosingQty = LastSupplierStock.ClosingQty + chemicalItem.TransactionQty;

                                    objStockSupplier.PackReceiveQty = chemicalItem.PackQty;
                                    objStockSupplier.PackOpeningQty = LastSupplierStock.PackClosingQty;
                                    objStockSupplier.PackIssueQty = 0;
                                    objStockSupplier.PackClosingQty = LastSupplierStock.PackClosingQty + chemicalItem.PackQty;

                                    objStockSupplier.SetBy = userid;
                                    objStockSupplier.SetOn = System.DateTime.Now;
                                    objStockSupplier.Remark = "Loan Return Receive";

                                    _context.INV_ChemStockSupplier.Add(objStockSupplier);
                                    _context.SaveChanges();
                                }

                                #endregion

                                #region Item_Stock_Update

                                var CheckItemStock = (from ds in _context.INV_ChemStockItem
                                                      where ds.ItemID == chemicalItem.ItemID
                                                            && ds.StoreID == storeId
                                                            && ds.UnitID == unitId
                                                            && ds.PackSize == packSize
                                                            && ds.SizeUnit == sizeUnit
                                                      select ds).Any();

                                if (!CheckItemStock)
                                {
                                    INV_ChemStockItem objStockItem = new INV_ChemStockItem();

                                    objStockItem.ItemID = chemicalItem.ItemID;

                                    objStockItem.StoreID = Convert.ToByte(storeId);
                                    objStockItem.UnitID = unitId;
                                    objStockItem.PackSize = Convert.ToByte(packSize);
                                    objStockItem.SizeUnit = Convert.ToByte(sizeUnit);

                                    objStockItem.OpeningQty = 0;
                                    objStockItem.IssueQty = 0;
                                    objStockItem.ReceiveQty = chemicalItem.TransactionQty;
                                    objStockItem.ClosingQty = chemicalItem.TransactionQty;

                                    objStockItem.PackOpeningQty = 0;
                                    objStockItem.PackIssueQty = 0;
                                    objStockItem.PackReceiveQty = chemicalItem.PackQty;
                                    objStockItem.PackClosingQty = chemicalItem.PackQty;

                                    objStockItem.SetBy = userid;
                                    objStockItem.SetOn = System.DateTime.Now;
                                    objStockItem.Remark = "Loan Return Receive";

                                    _context.INV_ChemStockItem.Add(objStockItem);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    var LastItemInfo = (from ds in _context.INV_ChemStockItem
                                                        where ds.ItemID == chemicalItem.ItemID
                                                           && ds.StoreID == storeId
                                                           && ds.UnitID == unitId
                                                           && ds.PackSize == packSize
                                                           && ds.SizeUnit == sizeUnit
                                                        orderby ds.TransectionID descending
                                                        select ds).FirstOrDefault();

                                    INV_ChemStockItem objStockItem = new INV_ChemStockItem();

                                    objStockItem.ItemID = chemicalItem.ItemID;

                                    objStockItem.StoreID = Convert.ToByte(storeId);
                                    objStockItem.UnitID = unitId;
                                    objStockItem.PackSize = Convert.ToByte(packSize);
                                    objStockItem.SizeUnit = Convert.ToByte(sizeUnit);

                                    objStockItem.OpeningQty = LastItemInfo.ClosingQty;
                                    objStockItem.IssueQty = 0;
                                    objStockItem.ReceiveQty = chemicalItem.TransactionQty;
                                    objStockItem.ClosingQty = LastItemInfo.ClosingQty + chemicalItem.TransactionQty;

                                    objStockItem.PackOpeningQty = LastItemInfo.PackClosingQty;
                                    objStockItem.PackIssueQty = 0;
                                    objStockItem.PackReceiveQty = chemicalItem.PackQty;
                                    objStockItem.PackClosingQty = LastItemInfo.PackClosingQty + chemicalItem.PackQty;

                                    objStockItem.SetBy = userid;
                                    objStockItem.SetOn = System.DateTime.Now;
                                    objStockItem.Remark = "Loan Return Receive";

                                    _context.INV_ChemStockItem.Add(objStockItem);
                                    _context.SaveChanges();
                                }

                                #endregion
                            }

                            #region Update Status

                            var reqsId = _context.INV_StoreTransRequest.Where(m => m.TransactionID == model.TransactionID).FirstOrDefault().RequestID;

                            var curChemLocalPurcRecvPOEntity = _context.INV_StoreTrans.First(m => m.TransactionID == model.TransactionID);
                            curChemLocalPurcRecvPOEntity.RecordStatus = "LRR";//Loan Return Receive

                            var curChemFrgnPurcOrdrEntity = _context.INV_TransRequest.First(m => m.RequestID == reqsId);
                            curChemFrgnPurcOrdrEntity.RecordStatus = "LRR";//Loan Return Receive

                            #endregion

                            _context.SaveChanges();
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "There is no item to receive.";
                        }
                    }
                    Transaction.Complete();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Confirmed Successfully.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Confirmed.";
            }
            return _vmMsg;
        }

        public ValidationMsg CheckedChemicalPurchase(string receiveId, string checkComment, int checkedby)
        {
            long rcvId = string.IsNullOrEmpty(receiveId) ? 0 : Convert.ToInt64(receiveId);
            _vmMsg = new ValidationMsg();
            try
            {
                var OriginalEntity = _context.INV_StoreTrans.First(m => m.TransactionID == rcvId);

                OriginalEntity.CheckedBy = checkedby;
                OriginalEntity.CheckDate = DateTime.Now;
                OriginalEntity.CheckComments = checkComment;
                OriginalEntity.RecordStatus = "CHK";

                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Checked Successfully.";
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Checked.";
            }
            return _vmMsg;
        }

        public List<INVStoreTransRequest> GetLoanRequestInfoList(string RequestID)
        {
            if (!string.IsNullOrEmpty(RequestID))
            {
                long? requestID = Convert.ToInt64(RequestID);
                List<INV_TransRequest> searchList = _context.INV_TransRequest.Where(m => m.RecordStatus == "ISU" && m.RequestType == "LNIR").OrderByDescending(m => m.RequestID).ToList();
                return searchList.Select(c => SetToBussinessObject(c)).ToList<INVStoreTransRequest>();
            }
            else
                return null;
        }

        public List<INVStoreTrans> GetChemicalPurchaseReceiveList()
        {
            List<INV_StoreTrans> searchList = _context.INV_StoreTrans.Where(m => m.TransactionCategory == "RCV" && m.TransactionType == "LRR" && m.TransactionState == "FLR" && m.TransactionStatus == "TRI").OrderByDescending(m => m.TransactionID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<INVStoreTrans>();
        }

        public INVStoreTransRequest SetToBussinessObject(INV_TransRequest Entity)
        {
            INVStoreTransRequest Model = new INVStoreTransRequest();

            Model.RequestID = Entity.RequestID;
            Model.RequestNo = Entity.RequestNo;
            Model.RequestDate = string.IsNullOrEmpty(Entity.RequestDate.ToString()) ? string.Empty : Convert.ToDateTime(Entity.RequestDate).ToString("dd/MM/yyyy");
            Model.TransMethod = Entity.ReturnMethod;

            return Model;
        }

        public List<INVStoreTransItem> GetLoanRequestItemList(string requestId)
        {
            int reqsId = Convert.ToInt32(requestId);
            List<INV_TransRequestItem> searchList = _context.INV_TransRequestItem.Where(m => m.RequestID == reqsId).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<INVStoreTransItem>();
        }

        public List<INVStoreTransRequest> GetLoanRequestAfterSaveList(string TransactionID)
        {
            int transId = Convert.ToInt32(TransactionID);
            List<INV_StoreTransRequest> searchList = _context.INV_StoreTransRequest.Where(m => m.TransactionID == transId).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<INVStoreTransRequest>();
        }

        public List<INVStoreTransItem> GetRecvItemListList(string TransactionID)
        {
            int transId = Convert.ToInt32(TransactionID);
            List<INV_StoreTransItem> searchList = _context.INV_StoreTransItem.Where(m => m.TransactionID == transId).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<INVStoreTransItem>();
        }

        public List<INVStoreTransChallan> GetChallanList(string TransactionID)
        {
            int transId = Convert.ToInt32(TransactionID);
            try
            {
                List<INV_StoreTransChallan> searchList =
                    _context.INV_StoreTransChallan.Where(m => m.TransactionID == transId).ToList();
                return searchList.Select(c => SetToBussinessObject(c)).ToList<INVStoreTransChallan>();
            }
            catch
            {
                return null;
            }

        }

        public List<INVStoreTransRequest> GetPurcRecvPlList(string TransactionID)
        {
            int transId = Convert.ToInt32(TransactionID);
            List<INV_StoreTransRequest> searchList = _context.INV_StoreTransRequest.Where(m => m.TransactionID == transId).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<INVStoreTransRequest>();
        }

        public INVStoreTransRequest SetToBussinessObject(INV_StoreTransRequest Entity)
        {
            INVStoreTransRequest Model = new INVStoreTransRequest();

            Model.TransRequestID = Entity.TransRequestID;
            Model.TransactionID = Entity.TransactionID;
            Model.RequestID = Entity.RequestID;
            Model.RequestNo = Entity.RequestNo;
            Model.RequestDate = Convert.ToDateTime(_context.INV_TransRequest.Where(m => m.RequestID == Entity.RequestID).SingleOrDefault().RequestDate).ToString("dd/MM/yyyy");

            Model.Remark = Entity.Remark;

            return Model;
        }

        public INVStoreTransItem SetToBussinessObject(INV_TransRequestItem Entity)
        {
            INVStoreTransItem Model = new INVStoreTransItem();

            Model.ItemID = Entity.ItemID;
            Model.ChemicalName = Entity.ItemID == null ? "" : _context.Sys_ChemicalItem.Where(m => m.ItemID == Entity.ItemID).FirstOrDefault().ItemName;
            Model.SupplierID = Entity.RefSupplierID;
            Model.Supplier = Entity.RefSupplierID == null ? "Press F9" : _context.Sys_Supplier.Where(m => m.SupplierID == Entity.RefSupplierID).FirstOrDefault().SupplierName;

            Model.RefPackSize = Entity.PackSize;
            Model.RefPackSizeName = Entity.PackSize == null ? "" : _context.Sys_Size.Where(m => m.SizeID == Entity.PackSize).FirstOrDefault().SizeName;
            Model.RefSizeUnit = Entity.SizeUnit;
            Model.RefSizeUnitName = Entity.SizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.SizeUnit).FirstOrDefault().UnitName;
            Model.RefPackQty = Entity.PackQty;

            Model.PackSize = Entity.PackSize;
            Model.PackSizeName = Entity.PackSize == null ? "" : _context.Sys_Size.Where(m => m.SizeID == Entity.PackSize).FirstOrDefault().SizeName;
            Model.SizeUnit = Entity.SizeUnit;
            Model.SizeUnitName = Entity.SizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.SizeUnit).FirstOrDefault().UnitName;
            Model.PackQty = Entity.PackQty;

            Model.TransactionQty = Entity.TransQty;
            //Model.TransactionUnitName = Entity.TransUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.TransUnit).FirstOrDefault().UnitName; ;

            Model.TransactionUnitName = "KG";

            //Model.TransactionUnitName = "";
            //try
            //{
            //    Model.TransactionQty = Convert.ToInt32(Model.PackSizeName) * Model.PackQty;
            //}
            //catch
            //{
            //    Model.TransactionQty = 0;
            //}
            //Model.ManufacturerID = Entity.ManufacturerID;

            return Model;
        }

        public INVStoreTrans SetToBussinessObject(INV_StoreTrans Entity)
        {
            INVStoreTrans Model = new INVStoreTrans();

            Model.TransactionID = Entity.TransactionID;
            Model.TransactionNo = Entity.TransactionNo;
            Model.TransactionDate = Convert.ToDateTime(Entity.TransactionDate).ToString("dd/MM/yyyy");
            Model.TransactionCategory = Entity.TransactionCategory;
            Model.TransactionType = Entity.TransactionType;
            Model.FromSource = Entity.FromSource;
            Model.TransactionFrom = Entity.TransactionFrom;

            if (Entity.FromSource == "STR")
            {
                if (!string.IsNullOrEmpty(Entity.TransactionFrom))
                {
                    byte transactionFrom = Convert.ToByte(Entity.TransactionFrom);
                    Model.TransactionFromName = _context.SYS_Store.Where(m => m.StoreID == transactionFrom).FirstOrDefault().StoreName == null ? null : _context.SYS_Store.Where(m => m.StoreID == transactionFrom).FirstOrDefault().StoreName;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Entity.TransactionFrom))
                {
                    int SupplierID = Convert.ToInt32(Entity.TransactionFrom);
                    Model.SupplierID = SupplierID;
                    Model.SupplierName = _context.Sys_Supplier.Where(m => m.SupplierID == SupplierID).FirstOrDefault().SupplierName == null ? null : _context.Sys_Supplier.Where(m => m.SupplierID == SupplierID).FirstOrDefault().SupplierName;
                    Model.TransactionFromName = _context.Sys_Supplier.Where(m => m.SupplierID == SupplierID).FirstOrDefault().SupplierName == null ? null : _context.Sys_Supplier.Where(m => m.SupplierID == SupplierID).FirstOrDefault().SupplierName;
                    Model.SupplierCode = _context.Sys_Supplier.Where(m => m.SupplierID == SupplierID).FirstOrDefault().SupplierCode == null ? null : _context.Sys_Supplier.Where(m => m.SupplierID == SupplierID).FirstOrDefault().SupplierCode;
                }
            }
            Model.TransactionTo = Entity.TransactionTo;
            Model.Remark = Entity.Remarks;
            byte transactionTo = Convert.ToByte(Entity.TransactionTo);
            Model.TransactionToName = _context.SYS_Store.Where(m => m.StoreID == transactionTo).FirstOrDefault().StoreName == null ? null : _context.SYS_Store.Where(m => m.StoreID == transactionTo).FirstOrDefault().StoreName;

            Model.RecordStatus = Entity.RecordStatus;
            if (Entity.RecordStatus == "NCF")
                Model.RecordStatusName = "Not Confirmed";
            else if (Entity.RecordStatus == "CNF")
                Model.RecordStatusName = "Confirmed";
            else if (Entity.RecordStatus == "CHK")
                Model.RecordStatusName = "Checked";
            else if (Entity.RecordStatus == "RCV")
                Model.RecordStatusName = "Received";

            return Model;
        }

        public INVStoreTransChallan SetToBussinessObject(INV_StoreTransChallan Entity)
        {
            INVStoreTransChallan Model = new INVStoreTransChallan();

            Model.TransChallanID = Entity.TransChallanID;
            Model.TransactionID = Entity.TransactionID;
            Model.TransChallanNo = Entity.TransChallanNo;
            Model.RefChallanNo = Entity.RefChallanNo;
            Model.ChallanDate = Convert.ToDateTime(Entity.ChallanDate).ToString("dd/MM/yyyy");
            Model.CarringCost = Entity.CarringCost;
            Model.LaborCost = Entity.LaborCost;
            Model.OtherCost = Entity.OtherCost;
            //Model.Currency = Entity.Currency;
            Model.Remark = Entity.Remark;

            return Model;
        }

        public INVStoreTransItem SetToBussinessObject(INV_StoreTransItem Entity)
        {
            INVStoreTransItem Model = new INVStoreTransItem();

            Model.TransItemID = Entity.TransItemID;
            Model.TransRequestID = Entity.TransRequestID;
            Model.TransactionID = Entity.TransactionID;
            Model.ItemID = Entity.ItemID;
            Model.ChemicalName = Entity.ItemID == null ? "" : _context.Sys_ChemicalItem.Where(m => m.ItemID == Entity.ItemID).FirstOrDefault().ItemName;
            Model.SupplierID = Entity.SupplierID;
            Model.Supplier = Entity.SupplierID == null ? "Press F9" : _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierName;

            Model.RefPackSize = Entity.RefPackSize;
            Model.RefPackSizeName = Entity.RefPackSize == null ? "" : _context.Sys_Size.Where(m => m.SizeID == Entity.RefPackSize).FirstOrDefault().SizeName;
            Model.RefSizeUnit = Entity.RefSizeUnit;
            Model.RefSizeUnitName = Entity.RefSizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.RefSizeUnit).FirstOrDefault().UnitName;
            Model.RefPackQty = Entity.RefPackQty;

            Model.PackSize = Entity.PackSize;
            Model.PackSizeName = Entity.PackSize == null ? "" : _context.Sys_Size.Where(m => m.SizeID == Entity.PackSize).FirstOrDefault().SizeName;
            Model.SizeUnit = Entity.SizeUnit;
            Model.SizeUnitName = Entity.SizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.SizeUnit).FirstOrDefault().UnitName;
            Model.PackQty = Entity.PackQty;
            Model.TransactionQty = Entity.TransactionQty;
            Model.TransactionUnitName = Entity.TransactionUnit != null ? _context.Sys_Unit.Where(m => m.UnitID == Entity.TransactionUnit).FirstOrDefault().UnitName : "";
            Model.ManufacturerID = Entity.ManufacturerID;

            return Model;
        }

        public List<SysChemicalItem> GetChemicalItemList()
        {
            List<Sys_ChemicalItem> searchList = _context.Sys_ChemicalItem.OrderBy(m => m.ItemID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysChemicalItem>();
        }

        public SysChemicalItem SetToBussinessObject(Sys_ChemicalItem Entity)
        {
            SysChemicalItem Model = new SysChemicalItem();

            Model.ItemID = Entity.ItemID;
            Model.HSCode = Entity.HSCode;
            Model.ItemName = Entity.ItemName;

            return Model;
        }

        public ValidationMsg DeletedReceive(int transId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var challanList = _context.INV_StoreTransChallan.Where(m => m.TransactionID == transId).ToList();
                var itemList = _context.INV_StoreTransItem.Where(m => m.TransactionID == transId).ToList();

                if ((challanList.Count > 0) || (itemList.Count > 0))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteplElement = _context.INV_StoreTransRequest.First(m => m.TransactionID == transId);
                    _context.INV_StoreTransRequest.Remove(deleteplElement);

                    var deleteElement = _context.INV_StoreTrans.First(m => m.TransactionID == transId);
                    _context.INV_StoreTrans.Remove(deleteElement);

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

        public ValidationMsg DeletedReceiveChallan(int TransChallanID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var deleteElement = _context.INV_StoreTransChallan.First(m => m.TransChallanID == TransChallanID);
                _context.INV_StoreTransChallan.Remove(deleteElement);
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

        public ValidationMsg DeletedReceiveItem(int TransItemID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var deleteElement = _context.INV_StoreTransItem.First(m => m.TransItemID == TransItemID);
                _context.INV_StoreTransItem.Remove(deleteElement);

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

        public List<TransRequest> GetLoanReceiveRequestList()
        {
            List<INV_TransRequest> searchList = _context.INV_TransRequest.Where(m => m.RequestType == "LNIR" && m.RecordStatus == "ISU").OrderByDescending(m => m.RequestID).ToList();
            return searchList.Select(c => SetToBussinessObjectLonRcvRqst(c)).ToList<TransRequest>();
        }

        public TransRequest SetToBussinessObjectLonRcvRqst(INV_TransRequest Entity)
        {
            TransRequest Model = new TransRequest();

            Model.RequestID = Entity.RequestID;
            Model.RequestNo = Entity.RequestNo;
            Model.RequestDate = string.IsNullOrEmpty(Entity.RequestDate.ToString()) ? string.Empty : Convert.ToDateTime(Entity.RequestDate).ToString("dd/MM/yyyy");
            //Model.ReturnMethod = Entity.ReturnMethod;
            if (Entity.ReturnMethod == "DTD")
                Model.ReturnMethod = "Dolar to Dolar";
            else if (Entity.ReturnMethod == "EOI")
                Model.ReturnMethod = "Exchange Other Item";
            else if (Entity.ReturnMethod == "ESI")
                Model.ReturnMethod = "Exchange Other Item";
            else
                Model.ReturnMethod = "";
            Model.ExpectetReturnTime = Entity.ExpectetReturnTime.ToString();
            Model.FromSource = Entity.FromSource;
            if (Entity.FromSource == "STR")
            {
                if (!string.IsNullOrEmpty(Entity.RequestFrom))
                {
                    byte transactionFrom = Convert.ToByte(Entity.RequestFrom);
                    Model.RequestFromName = _context.SYS_Store.Where(m => m.StoreID == transactionFrom).FirstOrDefault().StoreName == null ? null : _context.SYS_Store.Where(m => m.StoreID == transactionFrom).FirstOrDefault().StoreName;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Entity.RequestFrom))
                {
                    int SupplierID = Convert.ToInt32(Entity.RequestFrom);
                    Model.RequestFromName = _context.Sys_Supplier.Where(m => m.SupplierID == SupplierID).FirstOrDefault().SupplierName == null ? null : _context.Sys_Supplier.Where(m => m.SupplierID == SupplierID).FirstOrDefault().SupplierName;
                }
            }

            Model.RequestTo = Entity.RequestTo;
            Model.RequestFrom = Entity.RequestFrom;
            byte transactionTo = Convert.ToByte(Entity.RequestTo);
            Model.RequestToName = _context.SYS_Store.Where(m => m.StoreID == transactionTo).FirstOrDefault().StoreName == null ? null : _context.SYS_Store.Where(m => m.StoreID == transactionTo).FirstOrDefault().StoreName;

            return Model;
        }

        public List<SysSupplier> GetSupplierSoruceList()
        {
            var query = @"select distinct cast(RequestTo as int) SupplierID,(select SupplierName from Sys_Supplier
                        where SupplierID = RequestTo)SupplierName,(select SupplierCode from Sys_Supplier
                        where SupplierID = RequestTo)SupplierCode from INV_TransRequest
                        where FromSource = 'SUP' and RequestType='LNIR' and RecordStatus='RCV'";
            return _context.Database.SqlQuery<SysSupplier>(query).ToList();
        }

        public List<SysStore> GetStoreSoruceList()
        {
            var query = @"select distinct cast(RequestTo as tinyint) StoreID,(select StoreName from SYS_Store
                        where StoreID = RequestTo)StoreName,(select StoreCode from SYS_Store
                        where StoreID = RequestTo)StoreCode from INV_TransRequest
                        where FromSource = 'STR' and RequestType='LNIR' and RecordStatus='RCV'";
            return _context.Database.SqlQuery<SysStore>(query).ToList();
        }
    }
}