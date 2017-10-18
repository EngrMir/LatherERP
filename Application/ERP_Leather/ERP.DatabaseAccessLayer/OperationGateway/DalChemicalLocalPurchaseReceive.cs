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
    public class DalChemicalLocalPurchaseReceive
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long ReceiveID = 0;
        public string ReceiveNo = string.Empty;

        public DalChemicalLocalPurchaseReceive()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(PRQChemLocalPurcRecv model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                if (model.PrqChemLocalPurcRecvPOList != null)
                {
                    if (model.PrqChemLocalPurcRecvItemList != null)
                    {
                        using (var tx = new TransactionScope())
                        {
                            using (_context)
                            {
                                model.ReceiveNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                                if (model.ReceiveNo != null)
                                {
                                    #region Receive

                                    PRQ_ChemLocalPurcRecv tblChemLocalPurcRecv = SetToModelObject(model, userid);
                                    _context.PRQ_ChemLocalPurcRecv.Add(tblChemLocalPurcRecv);
                                    _context.SaveChanges();

                                    #region Save Po Records

                                    if (model.PrqChemLocalPurcRecvPOList != null)
                                    {
                                        foreach (
                                            PRQChemLocalPurcRecvPO objPrqChemLocalPurcRecvPo in
                                                model.PrqChemLocalPurcRecvPOList)
                                        {
                                            objPrqChemLocalPurcRecvPo.ReceiveID = tblChemLocalPurcRecv.ReceiveID;
                                            PRQ_ChemLocalPurcRecvPO tblChemLocalPurcRecvPo =
                                                SetToModelObject(objPrqChemLocalPurcRecvPo, userid);
                                            _context.PRQ_ChemLocalPurcRecvPO.Add(tblChemLocalPurcRecvPo);
                                            _context.SaveChanges();

                                            #region Save Pl Item List

                                            if (model.PrqChemLocalPurcRecvItemList != null)
                                            {
                                                foreach (
                                                    PRQChemLocalPurcRecvItem objPrqChemLocalPurcRecvItem in
                                                        model.PrqChemLocalPurcRecvItemList)
                                                {
                                                    objPrqChemLocalPurcRecvItem.POReceiveID =
                                                        tblChemLocalPurcRecvPo.POReceiveID;
                                                    objPrqChemLocalPurcRecvItem.ReceiveID = tblChemLocalPurcRecv.ReceiveID;
                                                    objPrqChemLocalPurcRecvItem.ExpiryDate =
                                                        objPrqChemLocalPurcRecvItem.ExpiryDate.Contains("/")
                                                            ? objPrqChemLocalPurcRecvItem.ExpiryDate
                                                            : Convert.ToDateTime(objPrqChemLocalPurcRecvItem.ExpiryDate)
                                                                .ToString("dd/MM/yyyy");
                                                    PRQ_ChemLocalPurcRecvItem tblChemLocalPurcRecvItem =
                                                        SetToModelObject(objPrqChemLocalPurcRecvItem, userid);
                                                    _context.PRQ_ChemLocalPurcRecvItem.Add(tblChemLocalPurcRecvItem);
                                                }
                                            }

                                            #endregion

                                            #region Save Challan List

                                            if (model.PrqChemLocalPurcRecvChallanList != null)
                                            {
                                                foreach (
                                                    PRQChemLocalPurcRecvChallan objPrqChemLocalPurcRecvChallan in
                                                        model.PrqChemLocalPurcRecvChallanList)
                                                {
                                                    //objPrqChemLocalPurcRecvChallan.Currency = model.Currency;
                                                    ////objPrqChemLocalPurcRecvChallan.ExchangeCurrency =
                                                    ////    model.ExchangeCurrency;
                                                    ////objPrqChemLocalPurcRecvChallan.ExchangeRate = model.ExchangeRate;
                                                    //////objPrqChemLocalPurcRecvChallan.ExchangeValue = model.ExchangeValue;

                                                    objPrqChemLocalPurcRecvChallan.POReceiveID =
                                                        tblChemLocalPurcRecvPo.POReceiveID;
                                                    objPrqChemLocalPurcRecvChallan.ReceiveID =
                                                        tblChemLocalPurcRecv.ReceiveID;
                                                    objPrqChemLocalPurcRecvChallan.ChallanDate =
                                                        objPrqChemLocalPurcRecvChallan.ChallanDate.Contains("/")
                                                            ? objPrqChemLocalPurcRecvChallan.ChallanDate
                                                            : Convert.ToDateTime(
                                                                objPrqChemLocalPurcRecvChallan.ChallanDate)
                                                                .ToString("dd/MM/yyyy");
                                                    objPrqChemLocalPurcRecvChallan.ReceiveChallanNo = DalCommon.GetPreDefineNextCodeByUrl("ChemicalConsumption/ChemConsumption");
                                                    PRQ_ChemLocalPurcRecvChallan tblChemLocalPurcRecvChallan =
                                                        SetToModelObject(objPrqChemLocalPurcRecvChallan, userid);
                                                    _context.PRQ_ChemLocalPurcRecvChallan.Add(
                                                        tblChemLocalPurcRecvChallan);
                                                }
                                            }

                                            #endregion
                                        }
                                    }
                                    _context.SaveChanges();

                                    #endregion

                                    tx.Complete();
                                    ReceiveID = tblChemLocalPurcRecv.ReceiveID;
                                    ReceiveNo = model.ReceiveNo;

                                    #endregion

                                    _vmMsg.Type = Enums.MessageType.Success;
                                    _vmMsg.Msg = "Saved Successfully.";
                                }
                                else
                                {
                                    _vmMsg.Type = Enums.MessageType.Error;
                                    _vmMsg.Msg = "ReceiveNo Predefine Value not Found.";
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
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UK_PRQ_ChemLocalPurcRecvChallan_ReceiveChallanNo"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Challan No Already Exist.";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Failed to Save.";
                }
            }
            return _vmMsg;
        }

        public ValidationMsg Update(PRQChemLocalPurcRecv model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        PRQ_ChemLocalPurcRecv CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.PRQ_ChemLocalPurcRecv.First(m => m.ReceiveID == model.ReceiveID);

                        OriginalEntity.SupplierID = CurrentEntity.SupplierID;
                        OriginalEntity.SupplierAddressID = CurrentEntity.SupplierAddressID;
                        OriginalEntity.ReceiveNo = CurrentEntity.ReceiveNo;
                        OriginalEntity.ReceiveStore = CurrentEntity.ReceiveStore;
                        OriginalEntity.ReceiveDate = CurrentEntity.ReceiveDate;
                        OriginalEntity.Remark = CurrentEntity.Remark;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #region Save Period Records

                        if (model.PrqChemLocalPurcRecvPOList != null)
                        {
                            if (model.PrqChemLocalPurcRecvItemList != null)
                            {
                                foreach (
                                    PRQChemLocalPurcRecvItem objPrqChemLocalPurcRecvItem in
                                        model.PrqChemLocalPurcRecvItemList)
                                {

                                    if (objPrqChemLocalPurcRecvItem.ReceiveItemID == 0)
                                    {
                                        //objPrqChemLocalPurcRecvItem.POReceiveID = model.POReceiveID;
                                        objPrqChemLocalPurcRecvItem.ReceiveID = model.ReceiveID;
                                        objPrqChemLocalPurcRecvItem.ExpiryDate =
                                            objPrqChemLocalPurcRecvItem.ExpiryDate.Contains("/")
                                                ? objPrqChemLocalPurcRecvItem.ExpiryDate
                                                : Convert.ToDateTime(objPrqChemLocalPurcRecvItem.ExpiryDate)
                                                    .ToString("dd/MM/yyyy");
                                        PRQ_ChemLocalPurcRecvItem tblChemLocalPurcRecvItem =
                                            SetToModelObject(objPrqChemLocalPurcRecvItem, userid);
                                        _context.PRQ_ChemLocalPurcRecvItem.Add(tblChemLocalPurcRecvItem);
                                    }
                                    else
                                    {
                                        objPrqChemLocalPurcRecvItem.ExpiryDate =
                                            objPrqChemLocalPurcRecvItem.ExpiryDate.Contains("/")
                                                ? objPrqChemLocalPurcRecvItem.ExpiryDate
                                                : Convert.ToDateTime(objPrqChemLocalPurcRecvItem.ExpiryDate)
                                                    .ToString("dd/MM/yyyy");
                                        PRQ_ChemLocalPurcRecvItem CurEntity = SetToModelObject(
                                            objPrqChemLocalPurcRecvItem, userid);
                                        var OrgEntity =
                                            _context.PRQ_ChemLocalPurcRecvItem.First(
                                                m => m.ReceiveItemID == objPrqChemLocalPurcRecvItem.ReceiveItemID);

                                        OrgEntity.POReceiveID = CurEntity.POReceiveID;
                                        OrgEntity.SupplierID = CurEntity.SupplierID;
                                        OrgEntity.BatchNo = CurEntity.BatchNo;
                                        OrgEntity.LotNo = CurEntity.LotNo;
                                        OrgEntity.ExpiryDate = CurEntity.ExpiryDate;
                                        OrgEntity.PackSize = CurEntity.PackSize;
                                        OrgEntity.SizeUnit = CurEntity.SizeUnit;
                                        OrgEntity.PackQty = CurEntity.PackQty;
                                        OrgEntity.ReceiveQty = CurEntity.ReceiveQty;
                                        OrgEntity.UnitID = CurEntity.UnitID;
                                        OrgEntity.ManufacturerID = CurEntity.ManufacturerID;
                                        OrgEntity.Remark = CurEntity.Remark;
                                        OrgEntity.ModifiedBy = userid;
                                        OrgEntity.ModifiedOn = DateTime.Now;
                                    }
                                }
                            }

                            #region Save challan List

                            if (model.PrqChemLocalPurcRecvChallanList != null)
                            {
                                foreach (PRQChemLocalPurcRecvChallan objPrqChemLocalPurcRecvChallan in model.PrqChemLocalPurcRecvChallanList)
                                {
                                    if (objPrqChemLocalPurcRecvChallan.ReceiveChallanID == 0)
                                    {
                                        //objPrqChemLocalPurcRecvChallan.Currency = model.Currency;
                                        ////objPrqChemLocalPurcRecvChallan.ExchangeCurrency = model.ExchangeCurrency;
                                        ////objPrqChemLocalPurcRecvChallan.ExchangeRate = model.ExchangeRate;
                                        //////objPrqChemLocalPurcRecvChallan.ExchangeValue = model.ExchangeValue;

                                        objPrqChemLocalPurcRecvChallan.ReceiveID = model.ReceiveID;
                                        objPrqChemLocalPurcRecvChallan.ChallanDate =
                                            objPrqChemLocalPurcRecvChallan.ChallanDate.Contains("/")
                                                ? objPrqChemLocalPurcRecvChallan.ChallanDate
                                                : Convert.ToDateTime(objPrqChemLocalPurcRecvChallan.ChallanDate)
                                                    .ToString("dd/MM/yyyy");
                                        PRQ_ChemLocalPurcRecvChallan tblChemLocalPurcRecvChallan =
                                            SetToModelObject(objPrqChemLocalPurcRecvChallan, userid);
                                        _context.PRQ_ChemLocalPurcRecvChallan.Add(tblChemLocalPurcRecvChallan);
                                    }
                                    else
                                    {
                                        //objPrqChemLocalPurcRecvChallan.Currency = model.Currency;
                                        ////objPrqChemLocalPurcRecvChallan.ExchangeCurrency = model.ExchangeCurrency;
                                        ////objPrqChemLocalPurcRecvChallan.ExchangeRate = model.ExchangeRate;
                                        //////objPrqChemLocalPurcRecvChallan.ExchangeValue = model.ExchangeValue;

                                        objPrqChemLocalPurcRecvChallan.ChallanDate =
                                            objPrqChemLocalPurcRecvChallan.ChallanDate.Contains("/")
                                                ? objPrqChemLocalPurcRecvChallan.ChallanDate
                                                : Convert.ToDateTime(objPrqChemLocalPurcRecvChallan.ChallanDate)
                                                    .ToString("dd/MM/yyyy");
                                        PRQ_ChemLocalPurcRecvChallan CurrEntity = SetToModelObject(objPrqChemLocalPurcRecvChallan, userid);
                                        var OrgrEntity =
                                            _context.PRQ_ChemLocalPurcRecvChallan.First(
                                                m => m.ReceiveChallanID == objPrqChemLocalPurcRecvChallan.ReceiveChallanID);

                                        //OrgrEntity.ReceiveChallanNo = CurrEntity.ReceiveChallanNo;
                                        OrgrEntity.SupChallanNo = CurrEntity.SupChallanNo;
                                        OrgrEntity.ChallanDate = CurrEntity.ChallanDate;
                                        OrgrEntity.CarringCost = CurrEntity.CarringCost;
                                        OrgrEntity.LaborCost = CurrEntity.LaborCost;
                                        OrgrEntity.OtherCost = CurrEntity.OtherCost;
                                        OrgrEntity.Currency = CurrEntity.Currency;
                                        //OrgrEntity.ExchangeCurrency = CurrEntity.ExchangeCurrency;
                                        //OrgrEntity.ExchangeRate = CurrEntity.ExchangeRate;
                                        //OrgrEntity.ExchangeValue = CurrEntity.ExchangeValue;
                                        //OrgrEntity.CostIndicator = CurrEntity.CostIndicator;
                                        OrgrEntity.Remark = CurrEntity.Remark;
                                        OrgrEntity.ModifiedBy = userid;
                                        OrgrEntity.ModifiedOn = DateTime.Now;
                                        OrgrEntity.RecordStatus = CurrEntity.RecordStatus;
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

            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UK_PRQ_ChemLocalPurcRecvChallan_ReceiveChallanNo"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Challan No Already Exist.";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Failed to Update.";
                }
            }
            return _vmMsg;
        }

        public long GetReceiveID()
        {
            return ReceiveID;
        }

        public string GetReceiveNo()
        {
            return ReceiveNo;
        }
        public ValidationMsg ConfirmChemicalPurchase(PRQChemLocalPurcRecv model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {
                        var OriginalEntity = _context.PRQ_ChemLocalPurcRecv.First(m => m.ReceiveID == model.ReceiveID);

                        OriginalEntity.ApprovedBy = userid;
                        OriginalEntity.ApproveDate = DateTime.Now;
                        OriginalEntity.ApprovalAdvice = model.ApproveComment;
                        OriginalEntity.RecordStatus = "CNF";

                        byte? storeId = model.ReceiveStore;

                        if (model.PrqChemLocalPurcRecvItemList != null)
                        {
                            foreach (var chemicalItem in model.PrqChemLocalPurcRecvItemList)
                            {
                                byte? unitId = null;
                                if (!string.IsNullOrEmpty(chemicalItem.UnitName))
                                    unitId = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == chemicalItem.UnitName).FirstOrDefault().UnitID);

                                byte? packSize = null;
                                if (!string.IsNullOrEmpty(chemicalItem.PackSize))
                                    packSize = Convert.ToByte(_context.Sys_Size.Where(m => m.SizeName == chemicalItem.PackSize).FirstOrDefault().SizeID);

                                byte? sizeUnit = null;
                                if (!string.IsNullOrEmpty(chemicalItem.SizeUnit))
                                    sizeUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == chemicalItem.SizeUnit).FirstOrDefault().UnitID);

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

                                    CurrentItem.ReceiveQty = CurrentItem.ReceiveQty + chemicalItem.ReceiveQty;
                                    CurrentItem.ClosingQty = CurrentItem.ClosingQty + chemicalItem.ReceiveQty;

                                    CurrentItem.PackReceiveQty = CurrentItem.PackReceiveQty + chemicalItem.PackQty;
                                    CurrentItem.PackClosingQty = CurrentItem.PackClosingQty + chemicalItem.PackQty;

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
                                    objStockDaily.ReceiveQty = chemicalItem.ReceiveQty;
                                    objStockDaily.IssueQty = 0;
                                    objStockDaily.ClosingQty = objStockDaily.OpeningQty + chemicalItem.ReceiveQty;

                                    objStockDaily.PackOpeningQty = (PreviousRecord == null ? 0 : PreviousRecord.PackClosingQty);
                                    objStockDaily.PackReceiveQty = chemicalItem.PackQty;
                                    objStockDaily.PackIssueQty = 0;
                                    objStockDaily.PackClosingQty = objStockDaily.PackOpeningQty +
                                    chemicalItem.PackQty;

                                    objStockDaily.Remark = "Local Receive";
                                    objStockDaily.SetOn = DateTime.Now;
                                    objStockDaily.SetBy = userid;

                                    _context.INV_ChemStockDaily.Add(objStockDaily);
                                    _context.SaveChanges();
                                }

                                #endregion

                                #region Supplier_Stock_Update

                                var CheckSupplierStock = (from ds in _context.INV_ChemStockSupplier
                                                          where ds.SupplierID == model.SupplierID
                                                            && ds.StoreID == storeId
                                                            && ds.ItemID == chemicalItem.ItemID
                                                            && ds.UnitID == unitId
                                                            && ds.PackSize == packSize
                                                            && ds.SizeUnit == sizeUnit
                                                          select ds).Any();

                                if (!CheckSupplierStock)
                                {
                                    INV_ChemStockSupplier objStockSupplier = new INV_ChemStockSupplier();

                                    objStockSupplier.SupplierID = Convert.ToInt32(model.SupplierID);

                                    objStockSupplier.StoreID = Convert.ToByte(storeId);
                                    objStockSupplier.ItemID = chemicalItem.ItemID;
                                    objStockSupplier.UnitID = unitId;
                                    objStockSupplier.PackSize = Convert.ToByte(packSize);
                                    objStockSupplier.SizeUnit = Convert.ToByte(sizeUnit);

                                    objStockSupplier.OpeningQty = 0;
                                    objStockSupplier.ReceiveQty = chemicalItem.ReceiveQty;
                                    objStockSupplier.IssueQty = 0;
                                    objStockSupplier.ClosingQty = chemicalItem.ReceiveQty;

                                    objStockSupplier.PackReceiveQty = chemicalItem.PackQty;
                                    objStockSupplier.PackOpeningQty = 0;
                                    objStockSupplier.PackIssueQty = 0;
                                    objStockSupplier.PackClosingQty = chemicalItem.PackQty;

                                    objStockSupplier.Remark = "Local Receive";
                                    objStockSupplier.SetOn = DateTime.Now;
                                    objStockSupplier.SetBy = userid;

                                    _context.INV_ChemStockSupplier.Add(objStockSupplier);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    var LastSupplierStock = (from ds in _context.INV_ChemStockSupplier
                                                             where ds.SupplierID == model.SupplierID
                                                                && ds.StoreID == storeId
                                                                && ds.ItemID == chemicalItem.ItemID
                                                                && ds.UnitID == unitId
                                                                && ds.PackSize == packSize
                                                                && ds.SizeUnit == sizeUnit
                                                             orderby ds.TransectionID descending
                                                             select ds).FirstOrDefault();

                                    INV_ChemStockSupplier objStockSupplier = new INV_ChemStockSupplier();

                                    objStockSupplier.SupplierID = Convert.ToInt32(model.SupplierID);

                                    objStockSupplier.StoreID = Convert.ToByte(storeId);
                                    objStockSupplier.ItemID = chemicalItem.ItemID;
                                    objStockSupplier.UnitID = unitId;
                                    objStockSupplier.PackSize = Convert.ToByte(packSize);
                                    objStockSupplier.SizeUnit = Convert.ToByte(sizeUnit);

                                    objStockSupplier.OpeningQty = LastSupplierStock.ClosingQty;
                                    objStockSupplier.ReceiveQty = chemicalItem.ReceiveQty;
                                    objStockSupplier.IssueQty = 0;
                                    objStockSupplier.ClosingQty = LastSupplierStock.ClosingQty + chemicalItem.ReceiveQty;

                                    objStockSupplier.PackReceiveQty = chemicalItem.PackQty;
                                    objStockSupplier.PackOpeningQty = LastSupplierStock.PackClosingQty;
                                    objStockSupplier.PackIssueQty = 0;
                                    objStockSupplier.PackClosingQty = LastSupplierStock.PackClosingQty + chemicalItem.PackQty;

                                    objStockSupplier.Remark = "Local Receive";
                                    objStockSupplier.SetOn = DateTime.Now;
                                    objStockSupplier.SetBy = userid;

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
                                    objStockItem.ReceiveQty = chemicalItem.ReceiveQty;
                                    objStockItem.ClosingQty = chemicalItem.ReceiveQty;

                                    objStockItem.PackOpeningQty = 0;
                                    objStockItem.PackIssueQty = 0;
                                    objStockItem.PackReceiveQty = chemicalItem.PackQty;
                                    objStockItem.PackClosingQty = chemicalItem.PackQty;

                                    objStockItem.Remark = "Local Receive";
                                    objStockItem.SetOn = DateTime.Now;
                                    objStockItem.SetBy = userid;

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
                                    objStockItem.ReceiveQty = chemicalItem.ReceiveQty;
                                    objStockItem.ClosingQty = LastItemInfo.ClosingQty + chemicalItem.ReceiveQty;

                                    objStockItem.PackOpeningQty = LastItemInfo.PackClosingQty;
                                    objStockItem.PackIssueQty = 0;
                                    objStockItem.PackReceiveQty = chemicalItem.PackQty;
                                    objStockItem.PackClosingQty = LastItemInfo.PackClosingQty + chemicalItem.PackQty;

                                    objStockItem.Remark = "Local Receive";
                                    objStockItem.SetOn = DateTime.Now;
                                    objStockItem.SetBy = userid;

                                    _context.INV_ChemStockItem.Add(objStockItem);
                                    _context.SaveChanges();
                                }

                                #endregion
                            }

                            #region Update Status

                            var ordid = _context.PRQ_ChemLocalPurcRecvPO.Where(m => m.ReceiveID == model.ReceiveID).FirstOrDefault().OrderID;

                            var curChemLocalPurcRecvPOEntity = _context.PRQ_ChemLocalPurcRecvPO.First(m => m.OrderID == ordid);
                            curChemLocalPurcRecvPOEntity.RecordStatus = "RCV";

                            var curChemFrgnPurcOrdrEntity = _context.PRQ_ChemFrgnPurcOrdr.First(m => m.OrderID == ordid);
                            curChemFrgnPurcOrdrEntity.OrderState = "CMP";
                            curChemFrgnPurcOrdrEntity.OrderStatus = "GDRV";

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
                var OriginalEntity = _context.PRQ_ChemLocalPurcRecv.First(m => m.ReceiveID == rcvId);

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

        public PRQ_ChemLocalPurcRecv SetToModelObject(PRQChemLocalPurcRecv model, int userid)
        {
            PRQ_ChemLocalPurcRecv Entity = new PRQ_ChemLocalPurcRecv();

            Entity.ReceiveID = model.ReceiveID;
            Entity.ReceiveNo = model.ReceiveNo;
            Entity.ReceiveDate = DalCommon.SetDate(model.ReceiveDate);
            Entity.ReceiveCategory = "LPR";
            Entity.ReceiveType = "PRR";
            Entity.SupplierID = model.SupplierID;
            Entity.SupplierAddressID = model.SupplierAddressID;
            Entity.ReceiveStore = model.ReceiveStore;
            Entity.Remark = model.Remark;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public PRQ_ChemLocalPurcRecvPO SetToModelObject(PRQChemLocalPurcRecvPO model, int userid)
        {
            PRQ_ChemLocalPurcRecvPO Entity = new PRQ_ChemLocalPurcRecvPO();

            Entity.POReceiveID = model.POReceiveID;
            Entity.ReceiveID = model.ReceiveID;
            Entity.OrderID = model.OrderID;
            Entity.OrderNo = model.OrderNo;
            //Entity.CIID = model.CIID;
            //Entity.CINo = model.CINo;
            //Entity.LCID = model.LCID;
            //Entity.LCNo = model.LCNo;
            Entity.Remark = model.Remark;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public PRQ_ChemLocalPurcRecvItem SetToModelObject(PRQChemLocalPurcRecvItem model, int userid)
        {
            PRQ_ChemLocalPurcRecvItem Entity = new PRQ_ChemLocalPurcRecvItem();

            Entity.ReceiveItemID = model.ReceiveItemID;
            Entity.POReceiveID = model.POReceiveID;
            Entity.ReceiveID = model.ReceiveID;
            Entity.ItemID = model.ItemID;
            Entity.SupplierID = model.SupplierID;
            Entity.BatchNo = model.BatchNo;
            Entity.LotNo = model.LotNo;
            Entity.ExpiryDate = DalCommon.SetDate(model.ExpiryDate);
            if (string.IsNullOrEmpty(model.PackSize))
                Entity.PackSize = null;
            else
                Entity.PackSize = Convert.ToByte(_context.Sys_Size.Where(m => m.SizeName == model.PackSize).FirstOrDefault().SizeID);
            if (string.IsNullOrEmpty(model.SizeUnit))
                Entity.SizeUnit = null;
            else
                Entity.SizeUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.SizeUnit).FirstOrDefault().UnitID);
            Entity.PackQty = model.PackQty;
            Entity.ReceiveQty = model.ReceiveQty;
            if (string.IsNullOrEmpty(model.UnitName))
                Entity.UnitID = null;
            else
                Entity.UnitID = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.UnitName).FirstOrDefault().UnitID);
            Entity.ManufacturerID = model.ManufacturerID;
            Entity.Remark = model.Remark;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public PRQ_ChemLocalPurcRecvChallan SetToModelObject(PRQChemLocalPurcRecvChallan model, int userid)
        {
            PRQ_ChemLocalPurcRecvChallan Entity = new PRQ_ChemLocalPurcRecvChallan();

            Entity.ReceiveChallanID = model.ReceiveChallanID;
            Entity.POReceiveID = model.POReceiveID;
            Entity.ReceiveID = model.ReceiveID;
            Entity.ReceiveChallanNo = model.ReceiveChallanNo;
            Entity.SupChallanNo = model.SupChallanNo;
            if (model.ChallanDate != null)
            {
                Entity.ChallanDate = DalCommon.SetDate(model.ChallanDate);
            }

            Entity.CarringCost = model.CarringCost;
            Entity.LaborCost = model.LaborCost;
            Entity.OtherCost = model.OtherCost;
            Entity.Currency = model.Currency;
            //Entity.ExchangeCurrency = model.ExchangeCurrency;
            //Entity.ExchangeRate = model.ExchangeRate;
            //Entity.ExchangeValue = (Convert.ToDecimal(model.CarringCost) + Convert.ToDecimal(model.LaborCost) + Convert.ToDecimal(model.OtherCost)) * Convert.ToDecimal(model.ExchangeRate);
            //Entity.CostIndicator = model.CostIndicator;
            Entity.Remark = model.Remark;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public List<PRQChemLocalPurcRecvPO> GetPackingInfoList(string supplierid)
        {
            if (!string.IsNullOrEmpty(supplierid))
            {
                int supid = Convert.ToInt32(supplierid);
                List<PRQ_ChemFrgnPurcOrdr> searchList = _context.PRQ_ChemFrgnPurcOrdr.Where(m => m.RecordStatus == "CNF" && m.OrderCategory == "LPO" && m.SupplierID == supid && m.OrderState == "RNG" && m.OrderStatus == "ORIN").OrderByDescending(m => m.OrderID).ToList();
                return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemLocalPurcRecvPO>();
            }
            else
                return null;
        }

        public List<PRQChemLocalPurcRecv> GetChemicalPurchaseReceiveList()
        {
            List<PRQ_ChemLocalPurcRecv> searchList = _context.PRQ_ChemLocalPurcRecv.OrderByDescending(m => m.ReceiveID).ToList();
            //List<PRQ_ChemLocalPurcRecv> searchList = _context.PRQ_ChemLocalPurcRecv.Where(m => m.RecordStatus != "CNF").OrderByDescending(m => m.ReceiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemLocalPurcRecv>();
        }

        public PRQChemLocalPurcRecvPO SetToBussinessObject(PRQ_ChemFrgnPurcOrdr Entity)
        {
            PRQChemLocalPurcRecvPO Model = new PRQChemLocalPurcRecvPO();

            Model.OrderID = Entity.OrderID;
            Model.OrderNo = Entity.OrderNo;
            Model.OrderDate = Convert.ToDateTime(Entity.OrderDate).ToString("dd/MM/yyyy");

            return Model;
        }

        public List<PRQChemLocalPurcRecvItem> GetPackingItemList(string orderid)
        {
            int ordid = Convert.ToInt32(orderid);
            List<PRQ_ChemFrgnPurcOrdrItem> searchList = _context.PRQ_ChemFrgnPurcOrdrItem.Where(m => m.OrderID == ordid).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemLocalPurcRecvItem>();
        }

        public List<PRQChemLocalPurcRecvPO> GetPackingAfterSaveList(string ReceiveID)
        {
            int receiveID = Convert.ToInt32(ReceiveID);
            List<PRQ_ChemLocalPurcRecvPO> searchList = _context.PRQ_ChemLocalPurcRecvPO.Where(m => m.ReceiveID == receiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemLocalPurcRecvPO>();
        }

        public List<PRQChemLocalPurcRecvItem> GetRecvItemListList(string ReceiveID)
        {
            int receiveID = Convert.ToInt32(ReceiveID);
            List<PRQ_ChemLocalPurcRecvItem> searchList = _context.PRQ_ChemLocalPurcRecvItem.Where(m => m.ReceiveID == receiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemLocalPurcRecvItem>();
        }

        public List<PRQChemLocalPurcRecvChallan> GetChallanList(string ReceiveID)
        {
            int receiveID = Convert.ToInt32(ReceiveID);
            List<PRQ_ChemLocalPurcRecvChallan> searchList = _context.PRQ_ChemLocalPurcRecvChallan.Where(m => m.ReceiveID == receiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemLocalPurcRecvChallan>();
        }

        public List<PRQChemLocalPurcRecvPO> GetPurcRecvPlList(string ReceiveID)
        {
            int receiveID = Convert.ToInt32(ReceiveID);
            List<PRQ_ChemLocalPurcRecvPO> searchList = _context.PRQ_ChemLocalPurcRecvPO.Where(m => m.ReceiveID == receiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemLocalPurcRecvPO>();
        }

        public PRQChemLocalPurcRecvPO SetToBussinessObject(PRQ_ChemLocalPurcRecvPO Entity)
        {
            PRQChemLocalPurcRecvPO Model = new PRQChemLocalPurcRecvPO();

            Model.POReceiveID = Entity.POReceiveID;
            Model.ReceiveID = Entity.ReceiveID;
            Model.OrderID = Entity.OrderID;
            Model.OrderNo = Entity.OrderNo;
            Model.OrderDate = Convert.ToDateTime(_context.PRQ_ChemFrgnPurcOrdr.Where(m => m.OrderID == Entity.OrderID).SingleOrDefault().OrderDate).ToString("dd/MM/yyyy");

            Model.Remark = Entity.Remark;

            return Model;
        }

        public PRQChemLocalPurcRecvItem SetToBussinessObject(PRQ_ChemFrgnPurcOrdrItem Entity)
        {
            PRQChemLocalPurcRecvItem Model = new PRQChemLocalPurcRecvItem();

            Model.ItemID = Entity.ItemID;
            Model.ChemicalName = Entity.ItemID == null ? "" : _context.Sys_ChemicalItem.Where(m => m.ItemID == Entity.ItemID).FirstOrDefault().ItemName;
            Model.SupplierID = Entity.SupplierID;
            Model.ExpiryDate = Convert.ToDateTime(DateTime.Now.AddDays(Convert.ToInt32(_context.Sys_ChemicalItem.Where(m => m.ItemID == Entity.ItemID).FirstOrDefault().ExpiryLimit))).ToString("dd/MM/yyyy");
            Model.PackSizeID = Entity.PackSize;
            Model.SizeUnitID = Entity.SizeUnit;
            Model.PackSize = Entity.PackSize == null ? "" : _context.Sys_Size.Where(m => m.SizeID == Entity.PackSize).FirstOrDefault().SizeName;
            Model.SizeUnit = Entity.SizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.SizeUnit).FirstOrDefault().UnitName;
            Model.UnitID = Entity.OrderUnit;
            Model.UnitName = Entity.OrderUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.OrderUnit).FirstOrDefault().UnitName;
            //Model.UnitName = "";
            Model.PackQty = Entity.PackQty;
            try
            {
                Model.ReceiveQty = Convert.ToInt32(Model.PackSize) * Model.PackQty;
            }
            catch
            {
                Model.ReceiveQty = 0;
            }

            //Model.UnitID = Entity.PLUnit;
            Model.ManufacturerID = Entity.ManufacturerID;

            return Model;
        }

        public PRQChemLocalPurcRecv SetToBussinessObject(PRQ_ChemLocalPurcRecv Entity)
        {
            PRQChemLocalPurcRecv Model = new PRQChemLocalPurcRecv();

            Model.ReceiveID = Entity.ReceiveID;
            Model.ReceiveNo = Entity.ReceiveNo;
            Model.ReceiveDate = Convert.ToDateTime(Entity.ReceiveDate).ToString("dd/MM/yyyy");
            Model.ReceiveCategory = Entity.ReceiveCategory;
            Model.ReceiveType = Entity.ReceiveType;
            Model.SupplierID = Entity.SupplierID;
            if (Entity.SupplierID != null)
            {
                Model.SupplierName = _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierName == null ? null : _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierName;
                Model.SupplierCode = _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierCode == null ? null : _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierCode;
            }
            Model.SupplierAddressID = Entity.SupplierAddressID;
            Model.ReceiveStore = Convert.ToByte(Entity.ReceiveStore);
            Model.ReceiveStoreName = _context.SYS_Store.Where(m => m.StoreID == Entity.ReceiveStore).FirstOrDefault().StoreName == null ? null : _context.SYS_Store.Where(m => m.StoreID == Entity.ReceiveStore).FirstOrDefault().StoreName;
            Model.Remark = Entity.Remark;
            //var challan = _context.PRQ_ChemLocalPurcRecvChallan.Where(m => m.ReceiveID == Model.ReceiveID).FirstOrDefault();
            //if (challan != null)
            //{
            //    Model.Currency = challan.Currency;
            //    Model.ExchangeCurrency = challan.ExchangeCurrency;
            //    Model.ExchangeRate = challan.ExchangeRate;
            //}
            Model.RecordStatus = Entity.RecordStatus;
            if (Entity.RecordStatus == "NCF")
                Model.RecordStatusName = "Not Confirmed";
            else if (Entity.RecordStatus == "CNF")
                Model.RecordStatusName = "Confirmed";
            else if (Entity.RecordStatus == "CHK")
                Model.RecordStatusName = "Checked";

            return Model;
        }

        public PRQChemLocalPurcRecvChallan SetToBussinessObject(PRQ_ChemLocalPurcRecvChallan Entity)
        {
            PRQChemLocalPurcRecvChallan Model = new PRQChemLocalPurcRecvChallan();

            Model.ReceiveChallanID = Entity.ReceiveChallanID;
            Model.POReceiveID = Entity.POReceiveID;
            Model.ReceiveID = Entity.ReceiveID;
            Model.ReceiveChallanNo = Entity.ReceiveChallanNo;
            Model.SupChallanNo = Entity.SupChallanNo;
            Model.ChallanDate = Convert.ToDateTime(Entity.ChallanDate).ToString("dd/MM/yyyy");
            Model.CarringCost = Entity.CarringCost;
            Model.LaborCost = Entity.LaborCost;
            Model.OtherCost = Entity.OtherCost;
            Model.Currency = Entity.Currency;
            //Model.ExchangeCurrency = Entity.ExchangeCurrency;
            //Model.ExchangeRate = Entity.ExchangeRate;
            //Model.ExchangeValue = Entity.ExchangeValue;
            //Model.CostIndicator = Entity.CostIndicator;
            Model.Remark = Entity.Remark;

            return Model;
        }

        public PRQChemLocalPurcRecvItem SetToBussinessObject(PRQ_ChemLocalPurcRecvItem Entity)
        {
            PRQChemLocalPurcRecvItem Model = new PRQChemLocalPurcRecvItem();

            Model.ReceiveItemID = Entity.ReceiveItemID;
            Model.POReceiveID = Entity.POReceiveID;
            Model.ReceiveID = Entity.ReceiveID;
            Model.ItemID = Entity.ItemID;
            Model.SupplierID = Entity.SupplierID;
            Model.ExpiryDate = Convert.ToDateTime(Entity.ExpiryDate).ToString("dd/MM/yyyy");
            Model.ChemicalName = Entity.ItemID == null ? null : _context.Sys_ChemicalItem.Where(m => m.ItemID == Entity.ItemID).FirstOrDefault().ItemName;
            Model.BatchNo = Entity.BatchNo;
            Model.LotNo = Entity.LotNo;
            Model.PackSizeID = Entity.PackSize;
            Model.SizeUnitID = Entity.SizeUnit;
            Model.PackSize = Entity.PackSize == null ? "" : _context.Sys_Size.Where(m => m.SizeID == Entity.PackSize).FirstOrDefault().SizeName;
            Model.SizeUnit = Entity.SizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.SizeUnit).FirstOrDefault().UnitName;
            Model.PackQty = Entity.PackQty;
            Model.ReceiveQty = Entity.ReceiveQty;
            Model.UnitName = Entity.UnitID != null ? _context.Sys_Unit.Where(m => m.UnitID == Entity.UnitID).FirstOrDefault().UnitName : "";
            Model.ManufacturerID = Entity.ManufacturerID;
            Model.Remark = Entity.Remark;

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

        public ValidationMsg DeletedReceive(long receiveID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                //var challanList = _context.PRQ_ChemLocalPurcRecvChallan.Where(m => m.ReceiveID == receiveID).ToList();
                //var itemList = _context.PRQ_ChemLocalPurcRecvItem.Where(m => m.ReceiveID == receiveID).ToList();
                var POList = _context.PRQ_ChemLocalPurcRecvPO.Where(m => m.ReceiveID == receiveID).ToList();
                //if ((challanList.Count > 0) || (itemList.Count > 0))
                if (POList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    //var deleteplElement = _context.PRQ_ChemLocalPurcRecvPO.First(m => m.ReceiveID == receiveID);
                    //_context.PRQ_ChemLocalPurcRecvPO.Remove(deleteplElement);

                    var deleteElement = _context.PRQ_ChemLocalPurcRecv.First(m => m.ReceiveID == receiveID);
                    _context.PRQ_ChemLocalPurcRecv.Remove(deleteElement);

                    _context.SaveChanges();

                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";
                }
            }
            catch(Exception e)
            {
                if (e.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "This Received Already Used.";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Failed to Delete.";
                }
            }
            return _vmMsg;
        }

        public ValidationMsg DeletedReceiveOrder(long POReceiveID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var challanList = _context.PRQ_ChemLocalPurcRecvChallan.Where(m => m.POReceiveID == POReceiveID).ToList();
                var itemList = _context.PRQ_ChemLocalPurcRecvItem.Where(m => m.POReceiveID == POReceiveID).ToList();

                if ((challanList.Count > 0) || (itemList.Count > 0))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteElement = _context.PRQ_ChemLocalPurcRecvPO.First(m => m.POReceiveID == POReceiveID);
                    _context.PRQ_ChemLocalPurcRecvPO.Remove(deleteElement);
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
        public ValidationMsg DeletedReceiveChallan(long ReceiveChallanID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var deleteElement = _context.PRQ_ChemLocalPurcRecvChallan.First(m => m.ReceiveChallanID == ReceiveChallanID);
                _context.PRQ_ChemLocalPurcRecvChallan.Remove(deleteElement);
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

        public ValidationMsg DeletedReceiveItem(long ReceiveItemID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var deleteElement = _context.PRQ_ChemLocalPurcRecvItem.First(m => m.ReceiveItemID == ReceiveItemID);
                _context.PRQ_ChemLocalPurcRecvItem.Remove(deleteElement);

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

        public List<SupplierDetails> getSupplierList()
        {
            var query = @"select distinct SupplierID,
                        (select SupplierCode from dbo.Sys_Supplier where SupplierID = dbo.PRQ_ChemFrgnPurcOrdr.SupplierID)SupplierCode,
                        (select SupplierName from dbo.Sys_Supplier where SupplierID = dbo.PRQ_ChemFrgnPurcOrdr.SupplierID)SupplierName,
                        (select Address from Sys_SupplierAddress where SupplierID = dbo.PRQ_ChemFrgnPurcOrdr.SupplierID and IsActive=1)Address from dbo.PRQ_ChemFrgnPurcOrdr
                        where OrderCategory='LPO' and OrderStatus ='ORIN' and OrderState='RNG' and RecordStatus='CNF'";
            var allData = _context.Database.SqlQuery<SupplierDetails>(query).ToList();
            return allData;

        }
    }
}
