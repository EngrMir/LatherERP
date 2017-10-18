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
    public class DalPRQChemFrgnPurcRecv
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public int ReceiveID = 0;
        public string ReceiveNo = string.Empty;
        //public decimal? totalRCVQuantity = 0;

        public DalPRQChemFrgnPurcRecv()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(PRQChemFrgnPurcRecv model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                if (model.PrqChemFrgnPurcRecvPlList != null)
                {
                    if (model.PrqChemFrgnPurcRecvItemList != null)
                    {
                        using (var tx = new TransactionScope())
                        {
                            using (_context)
                            {
                                model.ReceiveNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                                if (model.ReceiveNo != null)
                                {
                                    #region Receive

                                    PRQ_ChemFrgnPurcRecv tblChemFrgnPurcRecv = SetToModelObject(model, userid);
                                    _context.PRQ_ChemFrgnPurcRecv.Add(tblChemFrgnPurcRecv);
                                    _context.SaveChanges();

                                    #region Save Pl Records

                                    if (model.PrqChemFrgnPurcRecvPlList != null)
                                    {
                                        foreach (
                                            PRQChemFrgnPurcRecvPL objPrqChemFrgnPurcRecvPl in
                                                model.PrqChemFrgnPurcRecvPlList)
                                        {
                                            objPrqChemFrgnPurcRecvPl.ReceiveID = tblChemFrgnPurcRecv.ReceiveID;
                                            PRQ_ChemFrgnPurcRecvPL tblChemFrgnPurcRecvPL =
                                                SetToModelObject(objPrqChemFrgnPurcRecvPl, userid);
                                            _context.PRQ_ChemFrgnPurcRecvPL.Add(tblChemFrgnPurcRecvPL);
                                            _context.SaveChanges();

                                            #region Save Pl Item List

                                            if (model.PrqChemFrgnPurcRecvItemList != null)
                                            {
                                                foreach (
                                                    PRQChemFrgnPurcRecvItem objPRQChemFrgnPurcRecvItem in
                                                        model.PrqChemFrgnPurcRecvItemList)
                                                {
                                                    objPRQChemFrgnPurcRecvItem.PLReceiveID =
                                                        tblChemFrgnPurcRecvPL.PLReceiveID;
                                                    objPRQChemFrgnPurcRecvItem.ReceiveID = tblChemFrgnPurcRecv.ReceiveID;
                                                    objPRQChemFrgnPurcRecvItem.ExpiryDate =
                                                        objPRQChemFrgnPurcRecvItem.ExpiryDate.Contains("/")
                                                            ? objPRQChemFrgnPurcRecvItem.ExpiryDate
                                                            : Convert.ToDateTime(objPRQChemFrgnPurcRecvItem.ExpiryDate)
                                                                .ToString("dd/MM/yyyy");
                                                    PRQ_ChemFrgnPurcRecvItem tblPRQChemFrgnPurcRecvItem =
                                                        SetToModelObject(objPRQChemFrgnPurcRecvItem, userid);
                                                    _context.PRQ_ChemFrgnPurcRecvItem.Add(tblPRQChemFrgnPurcRecvItem);
                                                }
                                            }

                                            #endregion

                                            #region Save Challan List

                                            if (model.PrqChemFrgnPurcRecvChallanList != null)
                                            {
                                                foreach (
                                                    PRQChemFrgnPurcRecvChallan objPRQChemFrgnPurcRecvChallan in
                                                        model.PrqChemFrgnPurcRecvChallanList)
                                                {
                                                    objPRQChemFrgnPurcRecvChallan.Currency = model.Currency;
                                                    objPRQChemFrgnPurcRecvChallan.ExchangeCurrency =
                                                        model.ExchangeCurrency;
                                                    objPRQChemFrgnPurcRecvChallan.ExchangeRate = model.ExchangeRate;
                                                    //objPRQChemFrgnPurcRecvChallan.ExchangeValue = model.ExchangeValue;

                                                    objPRQChemFrgnPurcRecvChallan.PLReceiveID =
                                                        tblChemFrgnPurcRecvPL.PLReceiveID;
                                                    objPRQChemFrgnPurcRecvChallan.ReceiveID =
                                                        tblChemFrgnPurcRecv.ReceiveID;
                                                    if (objPRQChemFrgnPurcRecvChallan.ChallanDate == null)
                                                        objPRQChemFrgnPurcRecvChallan.ChallanDate = null;
                                                    else
                                                        objPRQChemFrgnPurcRecvChallan.ChallanDate =
                                                            objPRQChemFrgnPurcRecvChallan.ChallanDate.Contains("/")
                                                                ? objPRQChemFrgnPurcRecvChallan.ChallanDate
                                                                : Convert.ToDateTime(
                                                                    objPRQChemFrgnPurcRecvChallan.ChallanDate)
                                                                    .ToString("dd/MM/yyyy");
                                                    PRQ_ChemFrgnPurcRecvChallan tblPRQChemFrgnPurcRecvChallan =
                                                        SetToModelObject(objPRQChemFrgnPurcRecvChallan, userid);
                                                    _context.PRQ_ChemFrgnPurcRecvChallan.Add(
                                                        tblPRQChemFrgnPurcRecvChallan);
                                                }
                                            }

                                            #endregion
                                        }
                                    }
                                    _context.SaveChanges();

                                    #endregion

                                    tx.Complete();
                                    ReceiveID = tblChemFrgnPurcRecv.ReceiveID;
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
                if (ex.InnerException.InnerException.Message.Contains("UK_PRQ_ChemFrgnPurcRecvChallan_ReceiveChallanNo"))
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

        public ValidationMsg Update(PRQChemFrgnPurcRecv model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        PRQ_ChemFrgnPurcRecv CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.PRQ_ChemFrgnPurcRecv.First(m => m.ReceiveID == model.ReceiveID);

                        OriginalEntity.SupplierID = CurrentEntity.SupplierID;
                        OriginalEntity.SupplierAddressID = CurrentEntity.SupplierAddressID;
                        OriginalEntity.ReceiveNo = CurrentEntity.ReceiveNo;
                        OriginalEntity.ReceiveStore = CurrentEntity.ReceiveStore;
                        OriginalEntity.ReceiveDate = CurrentEntity.ReceiveDate;
                        OriginalEntity.Remark = CurrentEntity.Remark;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #region Save Period Records

                        if (model.PrqChemFrgnPurcRecvPlList != null)
                        {
                            if (model.PrqChemFrgnPurcRecvItemList != null)
                            {
                                foreach (
                                    PRQChemFrgnPurcRecvItem objPRQChemFrgnPurcRecvItem in
                                        model.PrqChemFrgnPurcRecvItemList)
                                {

                                    if (objPRQChemFrgnPurcRecvItem.ReceiveItemID == 0)
                                    {
                                        //objPRQChemFrgnPurcRecvItem.PLReceiveID = model.PLReceiveID;
                                        objPRQChemFrgnPurcRecvItem.ReceiveID = model.ReceiveID;
                                        objPRQChemFrgnPurcRecvItem.ExpiryDate =
                                            objPRQChemFrgnPurcRecvItem.ExpiryDate.Contains("/")
                                                ? objPRQChemFrgnPurcRecvItem.ExpiryDate
                                                : Convert.ToDateTime(objPRQChemFrgnPurcRecvItem.ExpiryDate)
                                                    .ToString("dd/MM/yyyy");
                                        PRQ_ChemFrgnPurcRecvItem tblPRQChemFrgnPurcRecvItem =
                                            SetToModelObject(objPRQChemFrgnPurcRecvItem, userid);
                                        _context.PRQ_ChemFrgnPurcRecvItem.Add(tblPRQChemFrgnPurcRecvItem);
                                    }
                                    else
                                    {
                                        objPRQChemFrgnPurcRecvItem.ExpiryDate =
                                            objPRQChemFrgnPurcRecvItem.ExpiryDate.Contains("/")
                                                ? objPRQChemFrgnPurcRecvItem.ExpiryDate
                                                : Convert.ToDateTime(objPRQChemFrgnPurcRecvItem.ExpiryDate)
                                                    .ToString("dd/MM/yyyy");
                                        PRQ_ChemFrgnPurcRecvItem CurEntity = SetToModelObject(
                                            objPRQChemFrgnPurcRecvItem, userid);
                                        var OrgEntity =
                                            _context.PRQ_ChemFrgnPurcRecvItem.First(
                                                m => m.ReceiveItemID == objPRQChemFrgnPurcRecvItem.ReceiveItemID);

                                        OrgEntity.PLReceiveID = CurEntity.PLReceiveID;
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

                            if (model.PrqChemFrgnPurcRecvChallanList != null)
                            {
                                foreach (PRQChemFrgnPurcRecvChallan objPRQChemFrgnPurcRecvChallan in model.PrqChemFrgnPurcRecvChallanList)
                                {
                                    if (objPRQChemFrgnPurcRecvChallan.ReceiveChallanID == 0)
                                    {
                                        objPRQChemFrgnPurcRecvChallan.Currency = model.Currency;
                                        objPRQChemFrgnPurcRecvChallan.ExchangeCurrency = model.ExchangeCurrency;
                                        objPRQChemFrgnPurcRecvChallan.ExchangeRate = model.ExchangeRate;
                                        //objPRQChemFrgnPurcRecvChallan.ExchangeValue = model.ExchangeValue;

                                        objPRQChemFrgnPurcRecvChallan.ReceiveID = model.ReceiveID;
                                        objPRQChemFrgnPurcRecvChallan.ChallanDate =
                                            objPRQChemFrgnPurcRecvChallan.ChallanDate.Contains("/")
                                                ? objPRQChemFrgnPurcRecvChallan.ChallanDate
                                                : Convert.ToDateTime(objPRQChemFrgnPurcRecvChallan.ChallanDate)
                                                    .ToString("dd/MM/yyyy");
                                        PRQ_ChemFrgnPurcRecvChallan tblPRQChemFrgnPurcRecvChallan =
                                            SetToModelObject(objPRQChemFrgnPurcRecvChallan, userid);
                                        _context.PRQ_ChemFrgnPurcRecvChallan.Add(tblPRQChemFrgnPurcRecvChallan);
                                    }
                                    else
                                    {
                                        objPRQChemFrgnPurcRecvChallan.Currency = model.Currency;
                                        objPRQChemFrgnPurcRecvChallan.ExchangeCurrency = model.ExchangeCurrency;
                                        objPRQChemFrgnPurcRecvChallan.ExchangeRate = model.ExchangeRate;
                                        //objPRQChemFrgnPurcRecvChallan.ExchangeValue = model.ExchangeValue;

                                        objPRQChemFrgnPurcRecvChallan.ChallanDate =
                                            objPRQChemFrgnPurcRecvChallan.ChallanDate.Contains("/")
                                                ? objPRQChemFrgnPurcRecvChallan.ChallanDate
                                                : Convert.ToDateTime(objPRQChemFrgnPurcRecvChallan.ChallanDate)
                                                    .ToString("dd/MM/yyyy");
                                        PRQ_ChemFrgnPurcRecvChallan CurrEntity = SetToModelObject(objPRQChemFrgnPurcRecvChallan, userid);
                                        var OrgrEntity =
                                            _context.PRQ_ChemFrgnPurcRecvChallan.First(
                                                m => m.ReceiveChallanID == objPRQChemFrgnPurcRecvChallan.ReceiveChallanID);

                                        OrgrEntity.ReceiveChallanNo = CurrEntity.ReceiveChallanNo;
                                        OrgrEntity.ChallanDate = CurrEntity.ChallanDate;
                                        OrgrEntity.CarringCost = CurrEntity.CarringCost;
                                        OrgrEntity.LaborCost = CurrEntity.LaborCost;
                                        OrgrEntity.OtherCost = CurrEntity.OtherCost;
                                        OrgrEntity.Currency = CurrEntity.Currency;
                                        OrgrEntity.ExchangeCurrency = CurrEntity.ExchangeCurrency;
                                        OrgrEntity.ExchangeRate = CurrEntity.ExchangeRate;
                                        OrgrEntity.ExchangeValue = CurrEntity.ExchangeValue;
                                        OrgrEntity.CostIndicator = CurrEntity.CostIndicator;
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
                if (ex.InnerException.InnerException.Message.Contains("UK_PRQ_ChemFrgnPurcRecvChallan_ReceiveChallanNo"))
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

        public int GetReceiveID()
        {
            return ReceiveID;
        }

        public string GetReceiveNo()
        {
            return ReceiveNo;
        }
        public ValidationMsg ConfirmChemicalPurchase(PRQChemFrgnPurcRecv model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (TransactionScope Transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    using (_context)
                    {
                        var OriginalEntity = _context.PRQ_ChemFrgnPurcRecv.First(m => m.ReceiveID == model.ReceiveID);

                        OriginalEntity.ApprovedBy = userid;
                        OriginalEntity.ApproveDate = DateTime.Now;
                        OriginalEntity.ApprovalAdvice = model.ApproveComment;
                        OriginalEntity.RecordStatus = "CNF";

                        byte? storeId = model.ReceiveStore;

                        if (model.PrqChemFrgnPurcRecvItemList != null)
                        {
                            foreach (var chemicalItem in model.PrqChemFrgnPurcRecvItemList)
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

                                    objStockDaily.Remark = "Foreign Receive";
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

                                    objStockSupplier.Remark = "Foreign Receive";
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

                                    objStockSupplier.Remark = "Foreign Receive";
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

                                    objStockItem.Remark = "Foreign Receive";
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

                                    objStockItem.Remark = "Foreign Receive";
                                    objStockItem.SetOn = DateTime.Now;
                                    objStockItem.SetBy = userid;

                                    _context.INV_ChemStockItem.Add(objStockItem);
                                    _context.SaveChanges();
                                }

                                #endregion
                            }

                            #region Update Status

                            var plid =
                                _context.PRQ_ChemFrgnPurcRecvPL.Where(m => m.ReceiveID == model.ReceiveID)
                                    .FirstOrDefault()
                                    .PLID;
                            var lcid =
                                _context.LCM_PackingList.Where(m => m.PLID == plid)
                                    .FirstOrDefault()
                                    .LCID;
                            var piid = _context.LCM_LCOpening.Where(m => m.LCID == lcid).FirstOrDefault().PIID;

                            var ordid = _context.PRQ_ChemicalPI.Where(m => m.PIID == piid).FirstOrDefault().OrderID;

                            var curPackEntity = _context.LCM_PackingList.First(m => m.PLID == plid);
                            curPackEntity.RecordStatus = "RCV";

                            var curOrdEntity = _context.PRQ_ChemFrgnPurcOrdr.First(m => m.OrderID == ordid);
                            //curOrdEntity.RecordStatus = "RCV";
                            curOrdEntity.OrderState = "CMP";
                            curOrdEntity.OrderStatus = "GDRV";

                            #endregion

                            _context.SaveChanges();
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "There is no item to receive.";
                        }

                        //_context.SaveChanges();
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
                var OriginalEntity = _context.PRQ_ChemFrgnPurcRecv.First(m => m.ReceiveID == rcvId);

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

        public PRQ_ChemFrgnPurcRecv SetToModelObject(PRQChemFrgnPurcRecv model, int userid)
        {
            PRQ_ChemFrgnPurcRecv Entity = new PRQ_ChemFrgnPurcRecv();

            Entity.ReceiveID = model.ReceiveID;
            Entity.ReceiveNo = model.ReceiveNo;
            Entity.ReceiveDate = DalCommon.SetDate(model.ReceiveDate);
            Entity.ReceiveCategory = "FPR";
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

        public PRQ_ChemFrgnPurcRecvPL SetToModelObject(PRQChemFrgnPurcRecvPL model, int userid)
        {
            PRQ_ChemFrgnPurcRecvPL Entity = new PRQ_ChemFrgnPurcRecvPL();

            Entity.PLReceiveID = model.PLReceiveID;
            Entity.ReceiveID = model.ReceiveID;
            Entity.PLID = model.PLID;
            Entity.PLNo = model.PLNo;
            Entity.CIID = model.CIID;
            Entity.CINo = model.CINo;
            Entity.LCID = model.LCID;
            Entity.LCNo = model.LCNo;
            Entity.Remark = model.Remark;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public PRQ_ChemFrgnPurcRecvItem SetToModelObject(PRQChemFrgnPurcRecvItem model, int userid)
        {
            PRQ_ChemFrgnPurcRecvItem Entity = new PRQ_ChemFrgnPurcRecvItem();

            Entity.ReceiveItemID = model.ReceiveItemID;
            Entity.PLReceiveID = model.PLReceiveID;
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

        public PRQ_ChemFrgnPurcRecvChallan SetToModelObject(PRQChemFrgnPurcRecvChallan model, int userid)
        {
            PRQ_ChemFrgnPurcRecvChallan Entity = new PRQ_ChemFrgnPurcRecvChallan();

            Entity.ReceiveChallanID = model.ReceiveChallanID;
            Entity.PLReceiveID = model.PLReceiveID;
            Entity.ReceiveID = model.ReceiveID;
            Entity.ReceiveChallanNo = model.ReceiveChallanNo;
            if (model.ChallanDate == null)
                Entity.ChallanDate = null;
            else
                Entity.ChallanDate = DalCommon.SetDate(model.ChallanDate);
            Entity.CarringCost = model.CarringCost;
            Entity.LaborCost = model.LaborCost;
            Entity.OtherCost = model.OtherCost;
            Entity.Currency = model.Currency;
            Entity.ExchangeCurrency = model.ExchangeCurrency;
            Entity.ExchangeRate = model.ExchangeRate;
            Entity.ExchangeValue = (Convert.ToDecimal(model.CarringCost) + Convert.ToDecimal(model.LaborCost) + Convert.ToDecimal(model.OtherCost)) * Convert.ToDecimal(model.ExchangeRate);
            Entity.CostIndicator = model.CostIndicator;
            Entity.Remark = model.Remark;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public List<PRQChemFrgnPurcRecvPL> GetPackingInfoList(string supplierid)
        {
            var query = @"select ord.OrderID,ord.OrderNo,CONVERT(NVARCHAR(12),ord.OrderDate,105)OrderDate,
                        CPI.PIID,CPI.PINo,CONVERT(NVARCHAR(12),CPI.PIDate,105)PIDate,
                        LC.LCID,LC.LCNo,CONVERT(NVARCHAR(12),LC.LCDate,105)LCDate,
                        CI.CIID,CI.CINo,CONVERT(NVARCHAR(12),CI.CIDate,105)CIDate,
                        PL.PLID,PL.PLNo,CONVERT(NVARCHAR(12),PL.PLDate,105)PLDate
                        from dbo.PRQ_ChemFrgnPurcOrdr ord
                        inner join dbo.PRQ_ChemicalPI CPI on CPI.OrderID = ord.OrderID
                        inner join dbo.LCM_LCOpening LC on CPI.PIID = LC.PIID
                        inner join dbo.LCM_CommercialInvoice CI on CI.LCID = LC.LCID
                        inner join dbo.LCM_PackingList PL on PL.CIID = CI.CIID
                        where ord.OrderCategory='FPO' and ord.OrderStatus ='ORIN' and ord.OrderState='RNG' and ord.RecordStatus='CNF'
                        and ord.SupplierID = " + supplierid + "";
            var allData = _context.Database.SqlQuery<PRQChemFrgnPurcRecvPL>(query).ToList();
            return allData;
            //List<LCM_PackingList> searchList = _context.LCM_PackingList.Where(m => m.RecordStatus == "CNF").OrderByDescending(m => m.PLID).ToList();
            //return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemFrgnPurcRecvPL>();
        }

        public List<PRQChemFrgnPurcRecv> GetChemicalPurchaseReceiveList()
        {
            List<PRQ_ChemFrgnPurcRecv> searchList = _context.PRQ_ChemFrgnPurcRecv.OrderByDescending(m => m.ReceiveID).ToList();
            //List<PRQ_ChemFrgnPurcRecv> searchList = _context.PRQ_ChemFrgnPurcRecv.Where(m => m.RecordStatus != "CNF").OrderByDescending(m => m.ReceiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemFrgnPurcRecv>();
        }

        public PRQChemFrgnPurcRecvPL SetToBussinessObject(LCM_PackingList Entity)
        {
            PRQChemFrgnPurcRecvPL Model = new PRQChemFrgnPurcRecvPL();

            //Model.PLReceiveID = Entity.PLReceiveID;
            //Model.ReceiveID = Entity.ReceiveID;
            Model.PLID = Entity.PLID;
            Model.PLNo = Entity.PLNo;
            Model.PLDate = Convert.ToDateTime(Entity.PLDate).ToString("dd/MM/yyyy");
            Model.CIID = Entity.CIID;
            Model.CINo = Entity.CINo;
            Model.LCID = Entity.LCID;
            Model.LCNo = Entity.LCNo;
            Model.LCDate = Convert.ToDateTime(_context.LCM_LCOpening.Where(m => m.LCID == Entity.LCID).SingleOrDefault().LCDate).ToString("dd/MM/yyyy");

            return Model;
        }

        public List<PRQChemFrgnPurcRecvItem> GetPackingItemList(string PLID)
        {
            int plid = Convert.ToInt32(PLID);
            List<LCM_PackingListItem> searchList = _context.LCM_PackingListItem.Where(m => m.PLID == plid).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemFrgnPurcRecvItem>();
        }

        public List<PRQChemFrgnPurcRecvPL> GetPackingAfterSaveList(string ReceiveID)
        {
            int receiveID = Convert.ToInt32(ReceiveID);
            List<PRQ_ChemFrgnPurcRecvPL> searchList = _context.PRQ_ChemFrgnPurcRecvPL.Where(m => m.ReceiveID == receiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemFrgnPurcRecvPL>();
        }

        public List<PRQChemFrgnPurcRecvItem> GetRecvItemListList(string ReceiveID)
        {
            int receiveID = Convert.ToInt32(ReceiveID);
            List<PRQ_ChemFrgnPurcRecvItem> searchList = _context.PRQ_ChemFrgnPurcRecvItem.Where(m => m.ReceiveID == receiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemFrgnPurcRecvItem>();
        }

        public List<PRQChemFrgnPurcRecvChallan> GetChallanList(string ReceiveID)
        {
            int receiveID = Convert.ToInt32(ReceiveID);
            List<PRQ_ChemFrgnPurcRecvChallan> searchList = _context.PRQ_ChemFrgnPurcRecvChallan.Where(m => m.ReceiveID == receiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemFrgnPurcRecvChallan>();
        }

        public List<PRQChemFrgnPurcRecvPL> GetPurcRecvPlList(string ReceiveID)
        {
            int receiveID = Convert.ToInt32(ReceiveID);
            List<PRQ_ChemFrgnPurcRecvPL> searchList = _context.PRQ_ChemFrgnPurcRecvPL.Where(m => m.ReceiveID == receiveID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PRQChemFrgnPurcRecvPL>();
        }

        public PRQChemFrgnPurcRecvPL SetToBussinessObject(PRQ_ChemFrgnPurcRecvPL Entity)
        {
            PRQChemFrgnPurcRecvPL Model = new PRQChemFrgnPurcRecvPL();

            Model.PLReceiveID = Entity.PLReceiveID;
            Model.ReceiveID = Entity.ReceiveID;
            Model.PLID = Entity.PLID;
            Model.PLNo = Entity.PLNo;
            Model.PLDate = Convert.ToDateTime(_context.LCM_PackingList.Where(m => m.PLID == Entity.PLID).SingleOrDefault().PLDate).ToString("dd/MM/yyyy");
            //Model.PLDate = Entity.PLDate.ToString("dd/MM/yyyy");
            Model.CIID = Entity.CIID;
            Model.CINo = Entity.CINo;
            Model.LCID = Entity.LCID;
            Model.LCNo = Model.LCNo = _context.LCM_LCOpening.Where(m => m.LCID == Entity.LCID).SingleOrDefault().LCNo;
            Model.LCDate = Convert.ToDateTime(_context.LCM_LCOpening.Where(m => m.LCID == Entity.LCID).SingleOrDefault().LCDate).ToString("dd/MM/yyyy");
            Model.Remark = Entity.Remark;
            //Model.ExchangeRate = Entity.ExchangeRate;
            //Model.ExchangeValue = Entity.ExchangeValue;
            //Model.CostIndicator = Entity.CostIndicator;
            Model.Remark = Entity.Remark;

            return Model;
        }

        public PRQChemFrgnPurcRecvItem SetToBussinessObject(LCM_PackingListItem Entity)
        {
            PRQChemFrgnPurcRecvItem Model = new PRQChemFrgnPurcRecvItem();

            //Model.ReceiveItemID = Entity.ReceiveItemID;
            //Model.PLReceiveID = Entity.PLReceiveID;
            //Model.ReceiveID = Entity.ReceiveID;
            Model.ItemID = Entity.ItemID;
            Model.ChemicalName = Entity.ItemID == null ? "" : _context.Sys_ChemicalItem.Where(m => m.ItemID == Entity.ItemID).FirstOrDefault().ItemName;
            Model.SupplierID = Entity.SupplierID;
            Model.ExpiryDate = Convert.ToDateTime(DateTime.Now.AddDays(Convert.ToInt32(_context.Sys_ChemicalItem.Where(m => m.ItemID == Entity.ItemID).FirstOrDefault().ExpiryLimit))).ToString("dd/MM/yyyy");
            //Model.BatchNo = Entity.BatchNo;
            //Model.LotNo = Entity.LotNo;
            Model.PackSizeID = Entity.PackSize;
            Model.SizeUnitID = Entity.SizeUnit;
            Model.PackSize = Entity.PackSize == null ? "" : _context.Sys_Size.Where(m => m.SizeID == Entity.PackSize).FirstOrDefault().SizeName;
            Model.SizeUnit = Entity.SizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.SizeUnit).FirstOrDefault().UnitName;
            //Model.UnitName = "";
            Model.UnitID = Entity.PLUnit;
            Model.UnitName = Entity.PLUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.PLUnit).FirstOrDefault().UnitName;
            Model.PackQty = Entity.PackQty;
            try
            {
                Model.ReceiveQty = Convert.ToInt32(Model.PackSize) * Model.PackQty;
                //totalRCVQuantity += Model.ReceiveQty;
            }
            catch
            {
                Model.ReceiveQty = 0;
            }

            Model.UnitID = Entity.PLUnit;
            Model.ManufacturerID = Entity.ManufacturerID;
            //Model.TotalRCVQuantity = totalRCVQuantity;

            return Model;
        }

        public PRQChemFrgnPurcRecv SetToBussinessObject(PRQ_ChemFrgnPurcRecv Entity)
        {
            PRQChemFrgnPurcRecv Model = new PRQChemFrgnPurcRecv();

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
            Model.ReceiveStore = Entity.ReceiveStore;
            Model.ReceiveStoreName = _context.SYS_Store.Where(m => m.StoreID == Entity.ReceiveStore).FirstOrDefault().StoreName == null ? null : _context.SYS_Store.Where(m => m.StoreID == Entity.ReceiveStore).FirstOrDefault().StoreName;
            Model.Remark = Entity.Remark;
            var challan = _context.PRQ_ChemFrgnPurcRecvChallan.Where(m => m.ReceiveID == Model.ReceiveID).FirstOrDefault();
            if (challan != null)
            {
                Model.Currency = challan.Currency;
                Model.ExchangeCurrency = challan.ExchangeCurrency;
                Model.ExchangeRate = challan.ExchangeRate;
            }

            var lcInfo = _context.PRQ_ChemFrgnPurcRecvPL.Where(m => m.ReceiveID == Model.ReceiveID).FirstOrDefault();
            if (lcInfo != null)
            {
                Model.LCID = lcInfo.LCID;
                Model.LCNo = _context.LCM_LCOpening.Where(m => m.LCID == lcInfo.LCID).SingleOrDefault().LCNo;// lcInfo.LCNo;
                Model.LCDate = Convert.ToDateTime(_context.LCM_LCOpening.Where(m => m.LCID == lcInfo.LCID).SingleOrDefault().LCDate).ToString("dd/MM/yyyy");
            }

            Model.RecordStatus = Entity.RecordStatus;
            if (Entity.RecordStatus == "NCF")
                Model.RecordStatusName = "Not Confirmed";
            else if (Entity.RecordStatus == "CNF")
                Model.RecordStatusName = "Confirmed";
            else if (Entity.RecordStatus == "CHK")
                Model.RecordStatusName = "Checked";
            return Model;
        }

        public PRQChemFrgnPurcRecvChallan SetToBussinessObject(PRQ_ChemFrgnPurcRecvChallan Entity)
        {
            PRQChemFrgnPurcRecvChallan Model = new PRQChemFrgnPurcRecvChallan();

            Model.ReceiveChallanID = Entity.ReceiveChallanID;
            Model.PLReceiveID = Entity.PLReceiveID;
            Model.ReceiveID = Entity.ReceiveID;
            Model.ReceiveChallanNo = Entity.ReceiveChallanNo;
            Model.ChallanDate = Convert.ToDateTime(Entity.ChallanDate).ToString("dd/MM/yyyy");
            Model.CarringCost = Entity.CarringCost;
            Model.LaborCost = Entity.LaborCost;
            Model.OtherCost = Entity.OtherCost;
            Model.Currency = Entity.Currency;
            Model.ExchangeCurrency = Entity.ExchangeCurrency;
            Model.ExchangeRate = Entity.ExchangeRate;
            Model.ExchangeValue = Entity.ExchangeValue;
            Model.CostIndicator = Entity.CostIndicator;
            Model.Remark = Entity.Remark;

            return Model;
        }

        public PRQChemFrgnPurcRecvItem SetToBussinessObject(PRQ_ChemFrgnPurcRecvItem Entity)
        {
            PRQChemFrgnPurcRecvItem Model = new PRQChemFrgnPurcRecvItem();

            Model.ReceiveItemID = Entity.ReceiveItemID;
            Model.PLReceiveID = Entity.PLReceiveID;
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
                //var challanList = _context.PRQ_ChemFrgnPurcRecvChallan.Where(m => m.ReceiveID == receiveID).ToList();
                //var itemList = _context.PRQ_ChemFrgnPurcRecvItem.Where(m => m.ReceiveID == receiveID).ToList();

                var plList = _context.PRQ_ChemFrgnPurcRecvItem.Where(m => m.ReceiveID == receiveID).ToList();

                //if ((challanList.Count > 0) || (itemList.Count > 0))
                if (plList.Count > 0) 
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    //var deleteplElement = _context.PRQ_ChemFrgnPurcRecvPL.First(m => m.ReceiveID == receiveID);
                    //_context.PRQ_ChemFrgnPurcRecvPL.Remove(deleteplElement);

                    var deleteElement = _context.PRQ_ChemFrgnPurcRecv.First(m => m.ReceiveID == receiveID);
                    _context.PRQ_ChemFrgnPurcRecv.Remove(deleteElement);

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

        public ValidationMsg DeletedReceivePL(long PLReceiveID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var challanList = _context.PRQ_ChemFrgnPurcRecvChallan.Where(m => m.PLReceiveID == PLReceiveID).ToList();
                var itemList = _context.PRQ_ChemFrgnPurcRecvItem.Where(m => m.PLReceiveID == PLReceiveID).ToList();

                if ((challanList.Count > 0) || (itemList.Count > 0))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteplElement = _context.PRQ_ChemFrgnPurcRecvPL.First(m => m.PLReceiveID == PLReceiveID);
                    _context.PRQ_ChemFrgnPurcRecvPL.Remove(deleteplElement);
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
                var deleteElement = _context.PRQ_ChemFrgnPurcRecvChallan.First(m => m.ReceiveChallanID == ReceiveChallanID);
                _context.PRQ_ChemFrgnPurcRecvChallan.Remove(deleteElement);
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
                var deleteElement = _context.PRQ_ChemFrgnPurcRecvItem.First(m => m.ReceiveItemID == ReceiveItemID);
                _context.PRQ_ChemFrgnPurcRecvItem.Remove(deleteElement);

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
        public List<PRQChemFrgnPurcRecv> getSupplierList()
        {
            var query = @"select distinct SupplierID,
                                            (select SupplierCode from dbo.Sys_Supplier where SupplierID = dbo.PRQ_ChemFrgnPurcOrdr.SupplierID)SupplierCode,
                                            (select SupplierName from dbo.Sys_Supplier where SupplierID = dbo.PRQ_ChemFrgnPurcOrdr.SupplierID)SupplierName,
                                            (select Address from Sys_SupplierAddress where SupplierID = dbo.PRQ_ChemFrgnPurcOrdr.SupplierID and IsActive=1)Address from dbo.PRQ_ChemFrgnPurcOrdr
                                            where OrderCategory='FPO' and OrderStatus ='ORIN' and OrderState='RNG' and RecordStatus='CNF'";
            //            var query = @"select SupplierID, 
            //                        (select SupplierCode from dbo.Sys_Supplier where SupplierID = dbo.PRQ_ChemFrgnPurcOrdr.SupplierID)SupplierCode,
            //                        (select SupplierName from dbo.Sys_Supplier where SupplierID = dbo.PRQ_ChemFrgnPurcOrdr.SupplierID)SupplierName,
            //                        (select Address from Sys_SupplierAddress where SupplierID = dbo.PRQ_ChemFrgnPurcOrdr.SupplierID and IsActive=1)Address,
            //						OrderID,OrderNo,CONVERT(NVARCHAR(12),OrderDate,105)OrderDate
            //						from dbo.PRQ_ChemFrgnPurcOrdr
            //                        where OrderCategory='FPO' and OrderStatus ='ORIN' and OrderState='RNG' and RecordStatus='CNF'";
            var allData = _context.Database.SqlQuery<PRQChemFrgnPurcRecv>(query).ToList();
            return allData;
        }
    }
}
