using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Transactions;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalEXPCI
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long CIID = 0;
        public long CIPIID = 0;
        public long CIPIItemID = 0;
        public string CINo = string.Empty;

        public DalEXPCI()
        {
            _context = new BLC_DEVEntities();
        }
        public ValidationMsg Save(EXPCI model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Save

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.CINo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.CINo != null)
                        {
                            #region CI

                            EXP_CI tblEXPCI = SetToModelObject(model, userid);
                            _context.EXP_CI.Add(tblEXPCI);
                            _context.SaveChanges();

                            #endregion

                            #region CIPI

                            model.ExpCIPIList[0].CIID = tblEXPCI.CIID;
                            EXP_CIPI tblEXPCIPI = SetToModelObject(model.ExpCIPIList[0], userid);
                            _context.EXP_CIPI.Add(tblEXPCIPI);
                            _context.SaveChanges();

                            #endregion

                            #region CIPIItem

                            if (model.ExpCIPIItemList != null)
                            {
                                foreach (EXPCIPIItem objEXPCIPIItem in model.ExpCIPIItemList)
                                {
                                    objEXPCIPIItem.CIPIID = tblEXPCIPI.CIPIID;

                                    EXP_CIPIItem tblEXPCIPIItem = SetToModelObject(objEXPCIPIItem, userid);
                                    _context.EXP_CIPIItem.Add(tblEXPCIPIItem);
                                    _context.SaveChanges();

                                    #region CIPIItemColor

                                    if (model.ExpCIPIItemColorList != null)
                                    {
                                        if (objEXPCIPIItem.ArticleID == model.ExpCIPIItemColorList[0].ArticleID)
                                        {
                                            foreach (EXPCIPIItemColor objEXPCIPIItemColor in model.ExpCIPIItemColorList)
                                            {
                                                objEXPCIPIItemColor.CIPIItemID = tblEXPCIPIItem.CIPIItemID;
                                                objEXPCIPIItemColor.MaterialNo = objEXPCIPIItem.MaterialNo;

                                                objEXPCIPIItemColor.AvgSize = objEXPCIPIItem.AvgSize;
                                                if (string.IsNullOrEmpty(objEXPCIPIItem.AvgSizeUnitName))
                                                    objEXPCIPIItemColor.AvgSizeUnit = null;
                                                else
                                                    objEXPCIPIItemColor.AvgSizeUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == objEXPCIPIItem.AvgSizeUnitName).FirstOrDefault().UnitID);
                                                objEXPCIPIItemColor.SideDescription = objEXPCIPIItem.SideDescription;
                                                objEXPCIPIItemColor.SelectionRange = objEXPCIPIItem.SelectionRange;
                                                objEXPCIPIItemColor.Thickness = objEXPCIPIItem.Thickness;
                                                if (string.IsNullOrEmpty(objEXPCIPIItem.ThicknessUnitName))
                                                    objEXPCIPIItemColor.ThicknessUnit = null;
                                                else
                                                    objEXPCIPIItemColor.ThicknessUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == objEXPCIPIItem.ThicknessUnitName).FirstOrDefault().UnitID);
                                                objEXPCIPIItemColor.ThicknessAt = objEXPCIPIItem.ThicknessAt;

                                                EXP_CIPIItemColor tblEXPCIPIItemColor = SetToModelObject(objEXPCIPIItemColor, userid);
                                                _context.EXP_CIPIItemColor.Add(tblEXPCIPIItemColor);
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            _context.SaveChanges();
                            tx.Complete();
                            CIID = tblEXPCI.CIID;
                            CINo = model.CINo;
                            CIPIID = tblEXPCIPI.CIPIID;
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "CINo Predefine Value not Found.";
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Exp CI No Already Exit.";
                }
            }
            return _vmMsg;
        }

        public ValidationMsg Update(EXPCI model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region CI

                        EXP_CI CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.EXP_CI.First(m => m.CIID == model.CIID);

                        OriginalEntity.CIRefNo = CurrentEntity.CIRefNo;
                        OriginalEntity.CIDate = CurrentEntity.CIDate;
                        OriginalEntity.CIType = CurrentEntity.CIType;
                        OriginalEntity.CICurrency = CurrentEntity.CICurrency;
                        OriginalEntity.ExchangeCurrency = CurrentEntity.ExchangeCurrency;
                        OriginalEntity.ExchangeRate = CurrentEntity.ExchangeRate;
                        OriginalEntity.ExchangeValue = CurrentEntity.ExchangeValue;
                        OriginalEntity.CINote = CurrentEntity.CINote;
                        OriginalEntity.ExportNo = CurrentEntity.ExportNo;
                        OriginalEntity.ExportDate = CurrentEntity.ExportDate;
                        OriginalEntity.OnAccountRiskOf = CurrentEntity.OnAccountRiskOf;
                        OriginalEntity.NotifyParty = CurrentEntity.NotifyParty;
                        OriginalEntity.ShipmentFrom = CurrentEntity.ShipmentFrom;
                        OriginalEntity.ShipmentTo = CurrentEntity.ShipmentTo;
                        OriginalEntity.DrawnUnder = CurrentEntity.DrawnUnder;
                        OriginalEntity.PaymentTerms = CurrentEntity.PaymentTerms;
                        OriginalEntity.MarksAndNumber = CurrentEntity.MarksAndNumber;

                        OriginalEntity.OrdDeliveryMode = CurrentEntity.OrdDeliveryMode;
                        OriginalEntity.PriceLevel = CurrentEntity.PriceLevel;
                        OriginalEntity.CIFootQty = CurrentEntity.CIFootQty;
                        OriginalEntity.CIFootUnitPrice = CurrentEntity.CIFootUnitPrice;
                        OriginalEntity.CIMeterQty = CurrentEntity.CIMeterQty;
                        OriginalEntity.CIMeterUnitPrice = CurrentEntity.CIMeterUnitPrice;
                        OriginalEntity.CIAmount = CurrentEntity.CIAmount;

                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #endregion

                        #region CIPI

                        //if (string.IsNullOrEmpty(model.CINo))
                        if (model.ExpCIPIList[0].CIPIID == 0)
                        {
                            model.ExpCIPIList[0].CIID = model.CIID;
                            EXP_CIPI tblEXPCIPI = SetToModelObject(model.ExpCIPIList[0], userid);
                            _context.EXP_CIPI.Add(tblEXPCIPI);
                            _context.SaveChanges();
                            CIPIID = tblEXPCIPI.CIPIID;
                        }
                        else
                        {
                            CIPIID = model.ExpCIPIList[0].CIPIID;
                            EXP_CIPI CurrentCIPIEntity = SetToModelObject(model.ExpCIPIList[0], userid);
                            var OriginalCIPIEntity = _context.EXP_CIPI.First(m => m.CIPIID == CIPIID);

                            //OriginalCIPIEntity.CIID = CurrentCIPIEntity.CIID;
                            OriginalCIPIEntity.PIID = CurrentCIPIEntity.PIID;
                            OriginalCIPIEntity.LCID = CurrentCIPIEntity.LCID;
                            OriginalCIPIEntity.PIStatus = CurrentCIPIEntity.PIStatus;
                            OriginalCIPIEntity.PIAmount = CurrentCIPIEntity.PIAmount;

                            OriginalCIPIEntity.PIFootQty = CurrentCIPIEntity.PIFootQty;
                            OriginalCIPIEntity.PIFootUnitPrice = CurrentCIPIEntity.PIFootUnitPrice;
                            OriginalCIPIEntity.PIFootTotalPrice = CurrentCIPIEntity.PIAmount;

                            OriginalCIPIEntity.PIMeterQty = CurrentCIPIEntity.PIMeterQty;
                            OriginalCIPIEntity.PIMeterUnitPrice = CurrentCIPIEntity.PIMeterUnitPrice;
                            OriginalCIPIEntity.PIMeterTotalPrice = CurrentCIPIEntity.PIAmount;

                            OriginalCIPIEntity.PIAmount = CurrentCIPIEntity.PIAmount;

                            OriginalCIPIEntity.PICurrency = CurrentCIPIEntity.PICurrency;
                            OriginalCIPIEntity.ExchangeCurrency = CurrentCIPIEntity.ExchangeCurrency;
                            OriginalCIPIEntity.ExchangeRate = CurrentCIPIEntity.ExchangeRate;
                            OriginalCIPIEntity.ExchangeValue = CurrentCIPIEntity.ExchangeValue;
                            OriginalCIPIEntity.Remarks = CurrentCIPIEntity.Remarks;
                            OriginalCIPIEntity.ModifiedBy = userid;
                            OriginalCIPIEntity.ModifiedOn = DateTime.Now;
                        }

                        #endregion

                        #region CIPIItem

                        if (model.ExpCIPIItemList != null)
                        {
                            foreach (EXPCIPIItem objEXPCIPIItem in model.ExpCIPIItemList)
                            {
                                if (objEXPCIPIItem.CIPIItemID == 0)
                                {
                                    objEXPCIPIItem.CIPIID = CIPIID;
                                    EXP_CIPIItem tblYearMonthScheduleDate = SetToModelObject(objEXPCIPIItem, userid);
                                    _context.EXP_CIPIItem.Add(tblYearMonthScheduleDate);
                                    _context.SaveChanges();
                                    CIPIItemID = tblYearMonthScheduleDate.CIPIItemID;
                                }
                                else
                                {
                                    CIPIItemID = objEXPCIPIItem.CIPIItemID;
                                    EXP_CIPIItem CurrEntity = SetToModelObject(objEXPCIPIItem, userid);
                                    var OrgrEntity = _context.EXP_CIPIItem.First(m => m.CIPIItemID == objEXPCIPIItem.CIPIItemID);

                                    //OrgrEntity.CIPIID = CurrEntity.CIPIID;
                                    OrgrEntity.Commodity = CurrEntity.Commodity;
                                    OrgrEntity.ArticleID = CurrEntity.ArticleID;
                                    OrgrEntity.ArticleNo = CurrEntity.ArticleNo;
                                    OrgrEntity.ItemTypeID = CurrEntity.ItemTypeID;
                                    OrgrEntity.LeatherTypeID = CurrEntity.LeatherTypeID;
                                    OrgrEntity.LeatherStatusID = CurrEntity.LeatherStatusID;
                                    OrgrEntity.MaterialNo = CurrEntity.MaterialNo;
                                    OrgrEntity.AvgSize = CurrEntity.AvgSize;
                                    OrgrEntity.SideDescription = CurrEntity.SideDescription;
                                    OrgrEntity.SelectionRange = CurrEntity.SelectionRange;
                                    OrgrEntity.Thickness = CurrEntity.Thickness;
                                    OrgrEntity.ThicknessAt = CurrEntity.ThicknessAt;
                                    OrgrEntity.MeterCIQty = CurrEntity.MeterCIQty;
                                    OrgrEntity.MeterUnitPrice = CurrEntity.MeterUnitPrice;
                                    OrgrEntity.MeterTotalPrice = CurrEntity.MeterTotalPrice;
                                    OrgrEntity.FootCIQty = CurrEntity.FootCIQty;
                                    OrgrEntity.FootUnitPrice = CurrEntity.FootUnitPrice;
                                    OrgrEntity.FootTotalPrice = CurrEntity.FootTotalPrice;
                                    OrgrEntity.PackQty = CurrEntity.PackQty;
                                    OrgrEntity.Remarks = CurrEntity.Remarks;
                                    OrgrEntity.ModifiedBy = userid;
                                    OrgrEntity.ModifiedOn = DateTime.Now;
                                }
                            }
                        }

                        #endregion

                        #region CIPIItemColor

                        if (model.ExpCIPIItemColorList != null)
                        {
                            foreach (EXPCIPIItemColor objEXPCIPIItemColor in model.ExpCIPIItemColorList)
                            {
                                if (objEXPCIPIItemColor.CIPIItemColorID == 0)
                                {
                                    objEXPCIPIItemColor.CIPIItemID = CIPIItemID;
                                    EXP_CIPIItemColor tblEXPCIPIItemColor = SetToModelObject(objEXPCIPIItemColor, userid);
                                    _context.EXP_CIPIItemColor.Add(tblEXPCIPIItemColor);
                                }
                                else
                                {
                                    EXP_CIPIItemColor CurEntity = SetToModelObject(objEXPCIPIItemColor, userid);
                                    var OrgEntity = _context.EXP_CIPIItemColor.First(m => m.CIPIItemColorID == objEXPCIPIItemColor.CIPIItemColorID);

                                    OrgEntity.ColorID = CurEntity.ColorID;
                                    OrgEntity.MaterialNo = CurEntity.MaterialNo;
                                    OrgEntity.AvgSize = CurEntity.AvgSize;
                                    OrgEntity.SideDescription = CurEntity.SideDescription;
                                    OrgEntity.SelectionRange = CurEntity.SelectionRange;
                                    OrgEntity.Thickness = CurEntity.Thickness;
                                    OrgEntity.ThicknessAt = CurEntity.ThicknessAt;
                                    OrgEntity.MeterColorQty = CurEntity.MeterColorQty;
                                    OrgEntity.MeterUnitPrice = CurEntity.MeterUnitPrice;
                                    OrgEntity.MeterTotalPrice = CurEntity.MeterTotalPrice;
                                    OrgEntity.FootColorQty = CurEntity.FootColorQty;
                                    OrgEntity.FootUnitPrice = CurEntity.FootUnitPrice;
                                    OrgEntity.FootTotalPrice = CurEntity.FootTotalPrice;
                                    OrgEntity.PackQty = CurEntity.PackQty;
                                    OrgEntity.Remarks = CurEntity.Remarks;
                                    OrgEntity.ModifiedBy = userid;
                                    OrgEntity.ModifiedOn = DateTime.Now;
                                }
                            }
                        }

                        #endregion

                        _context.SaveChanges();
                        tx.Complete();
                        CIID = model.CIID;
                        CINo = model.CINo;
                        _vmMsg.Type = Enums.MessageType.Update;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";

                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Schedule Year,Month Combination Data Already Exit.";
                }
            }
            return _vmMsg;
        }

        public EXP_CI SetToModelObject(EXPCI model, int userid)
        {
            EXP_CI Entity = new EXP_CI();

            Entity.CIID = model.CIID;
            Entity.CIRefNo = model.CIRefNo;
            Entity.CINo = model.CINo;
            Entity.CIDate = DalCommon.SetDate(model.CIDate);
            Entity.CIType = model.CIType;
            Entity.CICurrency = model.CICurrency;
            Entity.ExchangeCurrency = model.ExchangeCurrency;
            Entity.ExchangeRate = model.ExchangeRate;
            Entity.ExchangeValue = model.ExchangeValue;
            Entity.CINote = model.CINote;
            Entity.ExportNo = model.ExportNo;
            if (model.ExportDate == null)
                Entity.ExportDate = null;
            else
                Entity.ExportDate = DalCommon.SetDate(model.ExportDate);
            Entity.OnAccountRiskOf = model.OnAccountRiskOf;
            Entity.NotifyParty = model.NotifyParty;
            Entity.ShipmentFrom = model.ShipmentFrom;
            Entity.ShipmentTo = model.ShipmentTo;
            Entity.DrawnUnder = model.DrawnUnder;
            Entity.PaymentTerms = model.PaymentTerms;
            Entity.MarksAndNumber = model.MarksAndNumber;

            Entity.OrdDeliveryMode = model.OrdDeliveryMode;
            Entity.PriceLevel = model.PriceLevel;

            Entity.CIFootQty = model.CIFootQty;
            Entity.CIFootUnitPrice = model.CIFootUnitPrice;
            Entity.CIMeterQty = model.CIMeterQty;
            Entity.CIMeterUnitPrice = model.CIMeterUnitPrice;
            Entity.CIAmount = model.CIAmount;
            Entity.CIFootTotalPrice = model.CIAmount;
            Entity.CIMeterTotalPrice = model.CIAmount;

            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public EXP_CIPI SetToModelObject(EXPCIPI model, int userid)
        {
            EXP_CIPI Entity = new EXP_CIPI();

            Entity.CIPIID = model.CIPIID;
            Entity.CIID = model.CIID;
            Entity.PIID = model.PIID;
            Entity.LCID = model.LCID;
            Entity.PIStatus = model.PIStatus;
            Entity.PIAmount = model.PIAmount;
            Entity.PICurrency = model.PICurrency;
            Entity.ExchangeCurrency = model.ExchangeCurrency;
            Entity.ExchangeRate = model.ExchangeRate;
            Entity.ExchangeValue = model.ExchangeValue;
            Entity.Remarks = model.Remarks;

            Entity.PIFootQty = model.PIFootQty;
            Entity.PIFootUnitPrice = model.PIFootUnitPrice;
            Entity.PIMeterQty = model.PIMeterQty;
            Entity.PIMeterUnitPrice = model.PIMeterUnitPrice;
            Entity.PIAmount = model.PIAmount;
            Entity.PIFootTotalPrice = model.PIAmount;
            Entity.PIMeterTotalPrice = model.PIAmount;

            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public EXP_CIPIItem SetToModelObject(EXPCIPIItem model, int userid)
        {
            EXP_CIPIItem Entity = new EXP_CIPIItem();

            Entity.CIPIItemID = model.CIPIItemID;
            Entity.PIItemID = model.PIItemID;
            Entity.CIPIID = model.CIPIID;
            Entity.Commodity = model.Commodity;
            Entity.ArticleID = model.ArticleID;
            Entity.ArticleNo = model.ArticleNo;
            Entity.ItemTypeID = model.ItemTypeID;
            Entity.LeatherTypeID = model.LeatherTypeID;
            Entity.LeatherStatusID = model.LeatherStatusID;
            Entity.MaterialNo = model.MaterialNo;
            Entity.AvgSize = model.AvgSize;

            Entity.AvgSizeUnit = model.AvgSizeUnit;
            //if (string.IsNullOrEmpty(model.AvgSizeUnitName))
            //    Entity.AvgSizeUnit = null;
            //else
            //    Entity.AvgSizeUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.AvgSizeUnitName).FirstOrDefault().UnitID);
            Entity.SideDescription = model.SideDescription;
            Entity.SelectionRange = model.SelectionRange;
            Entity.Thickness = model.Thickness.ToString();
            //if (string.IsNullOrEmpty(model.ThicknessUnitName))
            //    Entity.ThicknessUnit = null;
            //else
            //    Entity.ThicknessUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.ThicknessUnitName).FirstOrDefault().UnitID);
            Entity.ThicknessAt = model.ThicknessAt;
            Entity.ThicknessUnit = model.ThicknessUnit;
            Entity.MeterCIQty = model.MeterCIQty;
            Entity.MeterUnitPrice = model.MeterUnitPrice;
            Entity.MeterTotalPrice = model.MeterTotalPrice;
            Entity.FootCIQty = model.FootCIQty;
            Entity.FootUnitPrice = model.FootUnitPrice;
            Entity.FootTotalPrice = model.FootTotalPrice;
            Entity.PackQty = model.PackQty;
            if (string.IsNullOrEmpty(model.PackUnitName))
                Entity.PackUnit = null;
            else
                Entity.PackUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.PackUnitName).FirstOrDefault().UnitID);
            Entity.RecordStatus = "NCF";
            Entity.Remarks = model.Remarks;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public EXP_CIPIItemColor SetToModelObject(EXPCIPIItemColor model, int userid)
        {
            EXP_CIPIItemColor Entity = new EXP_CIPIItemColor();

            Entity.CIPIItemColorID = model.CIPIItemColorID;
            Entity.CIPIItemID = model.CIPIItemID;
            Entity.MaterialNo = model.MaterialNo;
            Entity.ColorID = model.ColorID;
            Entity.AvgSize = model.AvgSize;
            if (string.IsNullOrEmpty(model.AvgSizeUnitName))
                Entity.AvgSizeUnit = null;
            else
                Entity.AvgSizeUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.AvgSizeUnitName).FirstOrDefault().UnitID);
            Entity.SideDescription = model.SideDescription;
            Entity.SelectionRange = model.SelectionRange;
            Entity.Thickness = model.Thickness;
            if (string.IsNullOrEmpty(model.ThicknessUnitName))
                Entity.ThicknessUnit = null;
            else
                Entity.ThicknessUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.ThicknessUnitName).FirstOrDefault().UnitID);
            Entity.ThicknessAt = model.ThicknessAt;
            Entity.MeterColorQty = model.MeterColorQty;
            Entity.MeterUnitPrice = model.MeterUnitPrice;
            Entity.MeterTotalPrice = model.MeterTotalPrice;
            Entity.FootColorQty = model.FootColorQty;
            Entity.FootUnitPrice = model.FootUnitPrice;
            Entity.FootTotalPrice = model.FootTotalPrice;
            Entity.PackQty = model.PackQty;
            if (string.IsNullOrEmpty(model.PackUnitName))
                Entity.PackUnit = null;
            else
                Entity.PackUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.PackUnitName).FirstOrDefault().UnitID);
            //Entity.RecordStatus = "NCF";
            Entity.Remarks = model.Remarks;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public long GetCIID()
        {
            return CIID;
        }

        public string GetCINo()
        {
            return CINo;
        }

        public long GetCIPIID()
        {
            return CIPIID;
        }

        public ValidationMsg ConfirmedEXPCI(EXPCI model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityCI = _context.EXP_CI.First(m => m.CIID == model.CIID);
                        originalEntityCI.RecordStatus = "CNF";
                        originalEntityCI.ModifiedBy = userid;
                        originalEntityCI.ModifiedOn = DateTime.Now;

                        CIPIID = model.ExpCIPIList[0].CIPIID;
                        var originalEntityCIPI = _context.EXP_CIPI.First(m => m.CIPIID == CIPIID);
                        originalEntityCIPI.RecordStatus = "CNF";
                        originalEntityCIPI.ModifiedBy = userid;
                        originalEntityCIPI.ModifiedOn = DateTime.Now;

                        if (model.ExpCIPIItemList.Count > 0)
                        {
                            foreach (EXPCIPIItem objEXPCIPIItem in model.ExpCIPIItemList)
                            {
                                var originalEntityCIPIItem = _context.EXP_CIPIItem.First(m => m.CIPIItemID == objEXPCIPIItem.CIPIItemID);
                                originalEntityCIPIItem.RecordStatus = "CNF";
                                originalEntityCIPIItem.ModifiedBy = userid;
                                originalEntityCIPIItem.ModifiedOn = DateTime.Now;
                            }
                        }

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

        public ValidationMsg CheckedEXPCI(EXPCI model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityCI = _context.EXP_CI.First(m => m.CIID == model.CIID);
                        originalEntityCI.RecordStatus = "CHK";
                        originalEntityCI.ModifiedBy = userid;
                        originalEntityCI.ModifiedOn = DateTime.Now;

                        CIPIID = model.ExpCIPIList[0].CIPIID;
                        var originalEntityCIPI = _context.EXP_CIPI.First(m => m.CIPIID == CIPIID);
                        originalEntityCIPI.RecordStatus = "CHK";
                        originalEntityCIPI.ModifiedBy = userid;
                        originalEntityCIPI.ModifiedOn = DateTime.Now;

                        if (model.ExpCIPIItemList.Count > 0)
                        {
                            foreach (EXPCIPIItem objEXPCIPIItem in model.ExpCIPIItemList)
                            {
                                var originalEntityCIPIItem = _context.EXP_CIPIItem.First(m => m.CIPIItemID == objEXPCIPIItem.CIPIItemID);
                                originalEntityCIPIItem.RecordStatus = "CHK";
                                originalEntityCIPIItem.ModifiedBy = userid;
                                originalEntityCIPIItem.ModifiedOn = DateTime.Now;
                            }
                        }

                        _context.SaveChanges();

                        tx.Complete();
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Checked Successfully.";
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Checked.";
            }
            return _vmMsg;
        }

        public List<EXPCI> GetCIInformation()
        {
            List<EXP_CI> searchList = _context.EXP_CI.OrderByDescending(m => m.CIID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPCI>();
        }

        public EXPCI SetToBussinessObject(EXP_CI Entity)
        {
            EXPCI model = new EXPCI();

            model.CIID = Entity.CIID;
            model.CINo = Entity.CINo;
            model.CIRefNo = Entity.CIRefNo;
            model.CIDate = Convert.ToDateTime(Entity.CIDate).ToString("dd/MM/yyyy");
            model.CIType = Entity.CIType;
            model.CIAmount = Entity.CIAmount;
            model.CICurrency = Entity.CICurrency;
            model.ExchangeCurrency = Entity.ExchangeCurrency;
            model.ExchangeRate = Convert.ToDecimal(Entity.ExchangeRate);
            model.ExchangeValue = Convert.ToDecimal(Entity.ExchangeValue);
            model.CINote = Entity.CINote;
            model.ExportNo = Entity.ExportNo;
            if (Entity.ExportDate == null)
                model.ExportDate = "";
            else
                model.ExportDate = Convert.ToDateTime(Entity.ExportDate).ToString("dd/MM/yyyy");
            model.OnAccountRiskOf = Entity.OnAccountRiskOf;
            model.NotifyParty = Entity.NotifyParty;
            model.ShipmentFrom = Entity.ShipmentFrom;
            model.ShipmentTo = Entity.ShipmentTo;
            model.DrawnUnder = Entity.DrawnUnder;
            model.PaymentTerms = Entity.PaymentTerms;
            model.MarksAndNumber = Entity.MarksAndNumber;

            model.CIFootQty = Entity.CIFootQty;
            model.CIFootUnitPrice = Entity.CIFootUnitPrice;
            model.CIMeterQty = Entity.CIMeterQty;
            model.CIMeterUnitPrice = Entity.CIMeterUnitPrice;
            model.CIAmount = Entity.CIAmount;

            model.PriceLevel = Entity.PriceLevel;
            model.OrdDeliveryMode = Entity.OrdDeliveryMode;
            if (Entity.OrdDeliveryMode == "BA")
                model.OrdDeliveryModeName = "By Air";
            else if (Entity.OrdDeliveryMode == "BS")
                model.OrdDeliveryModeName = "By Bus";
            else if (Entity.OrdDeliveryMode == "BR")
                model.OrdDeliveryModeName = "By Road";
            else
                model.OrdDeliveryModeName = "";

            model.RecordStatus = Entity.RecordStatus;

            if (Entity.RecordStatus == "NCF")
                model.RecordStatusName = "Not Confirmed";
            else if (Entity.RecordStatus == "CNF")
                model.RecordStatusName = "Confirmed";
            else if (Entity.RecordStatus == "CHK")
                model.RecordStatusName = "Checked";
            else
                model.RecordStatusName = "";

            return model;
        }

        public List<EXPCIPI> GetCIPIInformation(string CIID)
        {
            if (!string.IsNullOrEmpty(CIID))
            {
                long ciid = Convert.ToInt64(CIID);
                List<EXP_CIPI> searchList = _context.EXP_CIPI.Where(m => m.CIID == ciid).OrderByDescending(m => m.CIPIID).ToList();
                return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPCIPI>();
            }
            else
                return null;
        }

        public EXPCIPI SetToBussinessObject(EXP_CIPI Entity)
        {
            EXPCIPI model = new EXPCIPI();

            model.CIPIID = Entity.CIPIID;
            model.CIID = Entity.CIID;
            model.CINo = Entity.CIID == null ? "" : _context.EXP_CI.Where(m => m.CIID == Entity.CIID).FirstOrDefault().CINo;

            model.PIID = Entity.PIID;
            model.PINo = Entity.PIID == null ? "" : _context.EXP_LeatherPI.Where(m => m.PIID == Entity.PIID).FirstOrDefault().PINo;

            model.BuyerOrderID = Entity.PIID == null ? null : _context.EXP_LeatherPI.Where(m => m.PIID == Entity.PIID).FirstOrDefault().BuyerOrderID;
            model.BuyerOrderNo = Entity.PIID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == model.BuyerOrderID).FirstOrDefault().OrderNo;

            model.BuyerID = model.BuyerOrderID == null ? null : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == model.BuyerOrderID).FirstOrDefault().BuyerID;
            model.BuyerName = model.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == model.BuyerID).FirstOrDefault().BuyerName;

            //model.PIID = Entity.PIID;
            //model.PINo = Entity.PIID == null ? "" : _context.EXP_LeatherPI.Where(m => m.PIID == Entity.PIID).FirstOrDefault().PINo;

            model.LCID = Entity.LCID;
            model.LCNo = Entity.LCID == null ? "" : _context.EXP_LCOpening.Where(m => m.LCID == Entity.LCID).FirstOrDefault().LCNo;
            model.PIStatus = Entity.PIStatus;
            model.PIAmount = Entity.PIAmount;
            model.PICurrency = Entity.PICurrency;
            model.ExchangeCurrency = Entity.ExchangeCurrency;
            model.ExchangeRate = Entity.ExchangeRate;
            model.ExchangeValue = Entity.ExchangeValue;
            model.Remarks = Entity.Remarks;

            model.PIFootQty = Entity.PIFootQty;
            model.PIFootUnitPrice = Entity.PIFootUnitPrice;
            model.PIMeterQty = Entity.PIMeterQty;
            model.PIMeterUnitPrice = Entity.PIMeterUnitPrice;
            model.PIAmount = Entity.PIAmount;

            return model;
        }

        public List<SysBuyer> GetBuyerList()
        {
            var query = @"select distinct BuyerID,
                        (select BuyerName from dbo.Sys_Buyer where BuyerID = dbo.EXP_LeatherPI.BuyerID)BuyerName,
                        (select BuyerCategory from dbo.Sys_Buyer where BuyerID = dbo.EXP_LeatherPI.BuyerID)BuyerCategory,
                        (select BuyerType from dbo.Sys_Buyer where BuyerID = dbo.EXP_LeatherPI.BuyerID)BuyerType,
                        (select Address from Sys_BuyerAddress where BuyerID = dbo.EXP_LeatherPI.BuyerID and IsActive=1)Address from dbo.EXP_LeatherPI
                        where RecordStatus='CNF'";
            var allData = _context.Database.SqlQuery<SysBuyer>(query).ToList();
            return allData;

        }

        public List<EXPCIPI> GetExpPIList(string _BuyerID)
        {
            int buyerid = Convert.ToInt32(_BuyerID);
            List<EXP_LeatherPI> searchList = _context.EXP_LeatherPI.Where(m => m.BuyerID == buyerid).OrderBy(m => m.PIID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPCIPI>();
        }

        public EXPCIPI SetToBussinessObject(EXP_LeatherPI Entity)
        {
            EXPCIPI Model = new EXPCIPI();

            Model.PIID = Entity.PIID;
            Model.PINo = Entity.PINo;
            Model.PIDate = Convert.ToDateTime(Entity.PIDate).ToString("dd/MM/yyyy");
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? null : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().OrderNo;
            var lc = _context.EXP_LCOpening.Where(m => m.PIID == Entity.PIID).FirstOrDefault();
            if (lc == null)
                Model.LCID = null;
            else
            {
                Model.LCID = lc.LCID;
                Model.LCNo = lc.LCNo;
            }
            if (Entity.PaymentMode == "ST")
                Model.PaymentTerms = "Sight";
            else if (Entity.PaymentMode == "DF")
                Model.PaymentTerms = "Defered";

            return Model;
        }

        public List<EXPCIPIItem> GetExpPIItemList(string PIID, string OrdDeliveryMode)
        {
            long? pIID = Convert.ToInt64(PIID);
            List<EXP_PIItem> searchList = _context.EXP_PIItem.Where(m => m.PIID == pIID).OrderByDescending(m => m.PIItemID).ToList();
            return searchList.Select(c => SetToBussinessObject(c, OrdDeliveryMode)).ToList<EXPCIPIItem>();
        }

        public EXPCIPIItem SetToBussinessObject(EXP_PIItem Entity, string OrdDeliveryMode)
        {
            EXPCIPIItem Model = new EXPCIPIItem();

            Model.PIItemID = Entity.PIItemID;
            Model.PIID = Entity.PIID;
            Model.Commodity = Entity.Commodity;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleName = Entity.ArticleID == null ? "" : _context.Sys_Article.Where(m => m.ArticleID == Entity.ArticleID).FirstOrDefault().ArticleName;
            Model.AvgSize = Entity.AvgSize;
            Model.AvgSizeUnit = Entity.AvgSizeUnit;
            Model.AvgSizeUnitName = Entity.AvgSizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AvgSizeUnit).FirstOrDefault().UnitName;
            Model.SideDescription = Entity.SideDescription;
            Model.SelectionRange = Entity.SelectionRange;
            Model.Thickness = Entity.Thickness;
            Model.ThicknessUnit = Entity.ThicknessUnit;
            Model.ThicknessUnitName = Entity.ThicknessUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.ThicknessUnit).FirstOrDefault().UnitName;
            Model.ThicknessAt = Entity.ThicknessAt;
            if (Entity.ThicknessAt == "AFSV")
                Model.ThicknessAtName = "After Saving";
            if (OrdDeliveryMode == "BA")
            {
                Model.FootCIQty = Entity.ArticleFootQty;
                Model.FootUnitPrice = Entity.AirFootUnitPrice;
                Model.FootTotalPrice = Entity.AirFootTotalPrice;

                Model.MeterCIQty = Entity.ArticleMeterQty;
                Model.MeterUnitPrice = Entity.AirMeterUnitPrice;
                Model.MeterTotalPrice = Entity.AirMeterTotalPrice;
            }
            else if (OrdDeliveryMode == "BS")
            {
                Model.FootCIQty = Entity.ArticleFootQty;
                Model.FootUnitPrice = Entity.SeaFootUnitPrice;
                Model.FootTotalPrice = Entity.SeaFootTotalPrice;

                Model.MeterCIQty = Entity.ArticleMeterQty;
                Model.MeterUnitPrice = Entity.SeaMeterUnitPrice;
                Model.MeterTotalPrice = Entity.SeaMeterTotalPrice;
            }
            else if (OrdDeliveryMode == "BR")
            {
                Model.FootCIQty = Entity.ArticleFootQty;
                Model.FootUnitPrice = Entity.RoadFootUnitPrice;
                Model.FootTotalPrice = Entity.RoadFootTotalPrice;

                Model.MeterCIQty = Entity.ArticleMeterQty;
                Model.MeterUnitPrice = Entity.RoadMeterUnitPrice;
                Model.MeterTotalPrice = Entity.RoadMeterTotalPrice;
            }
            else
            {
                //Model.FootCIQty = Entity.ArticleFootQty;
                //Model.FootUnitPrice = Entity.RoadFootUnitPrice;
                //Model.FootTotalPrice = Entity.RoadFootTotalPrice;

                //Model.MeterCIQty = Entity.ArticleMeterQty;
                //Model.MeterUnitPrice = Entity.RoadMeterUnitPrice;
                //Model.MeterTotalPrice = Entity.RoadMeterTotalPrice;
            }

            return Model;
        }

        public List<EXPCIPIItemColor> GetExpPIItemColorList(string PIItemID, string OrdDeliveryMode)
        {
            long pIItemID = Convert.ToInt64(PIItemID);
            List<EXP_PIItemColor> searchList = _context.EXP_PIItemColor.Where(m => m.PIItemID == pIItemID).OrderByDescending(m => m.PIItemID).ToList();
            return searchList.Select(c => SetToBussinessObject(c, OrdDeliveryMode)).ToList<EXPCIPIItemColor>();
        }

        public EXPCIPIItemColor SetToBussinessObject(EXP_PIItemColor Entity, string OrdDeliveryMode)
        {
            EXPCIPIItemColor Model = new EXPCIPIItemColor();

            //Model.CIPIItemColorID = Entity.CIPIItemColorID;
            //Model.CIPIItemID = Entity.CIPIItemID;
            //Model.MaterialNo = Entity.MaterialNo;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            //Model.AvgSize = Entity.AvgSize;
            //Model.AvgSizeUnit = Entity.AvgSizeUnit;
            //Model.AvgSizeUnitName = Entity.AvgSizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AvgSizeUnit).FirstOrDefault().UnitName;
            //Model.SideDescription = Entity.SideDescription;
            //Model.SelectionRange = Entity.SelectionRange;
            //Model.Thickness = Entity.Thickness;
            //Model.ThicknessUnit = Entity.ThicknessUnit;
            //Model.ThicknessUnitName = Entity.ThicknessUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.ThicknessUnit).FirstOrDefault().UnitName;
            //Model.ThicknessAt = Entity.ThicknessAt;
            if (OrdDeliveryMode == "BA")
            {
                Model.FootColorQty = Entity.ColorFootQty;
                Model.FootUnitPrice = Entity.AirFootUnitPrice;
                Model.FootTotalPrice = Entity.AirFootTotalPrice;

                Model.MeterColorQty = Entity.ColorMeterQty;
                Model.MeterUnitPrice = Entity.AirMeterUnitPrice;
                Model.MeterTotalPrice = Entity.AirMeterTotalPrice;

            }
            else if (OrdDeliveryMode == "BS")
            {
                Model.FootColorQty = Entity.ColorFootQty;
                Model.FootUnitPrice = Entity.SeaFootUnitPrice;
                Model.FootTotalPrice = Entity.SeaFootTotalPrice;

                Model.MeterColorQty = Entity.ColorMeterQty;
                Model.MeterUnitPrice = Entity.SeaMeterUnitPrice;
                Model.MeterTotalPrice = Entity.SeaMeterTotalPrice;
            }
            else if (OrdDeliveryMode == "BR")
            {
                Model.FootColorQty = Entity.ColorFootQty;
                Model.FootUnitPrice = Entity.RoadFootUnitPrice;
                Model.FootTotalPrice = Entity.RoadFootTotalPrice;

                Model.MeterColorQty = Entity.ColorMeterQty;
                Model.MeterUnitPrice = Entity.RoadMeterUnitPrice;
                Model.MeterTotalPrice = Entity.RoadMeterTotalPrice;
            }
            else
            {
                //Model.FootCIQty = Entity.ArticleFootQty;
                //Model.FootUnitPrice = Entity.RoadFootUnitPrice;
                //Model.FootTotalPrice = Entity.RoadFootTotalPrice;

                //Model.MeterCIQty = Entity.ArticleMeterQty;
                //Model.MeterUnitPrice = Entity.RoadMeterUnitPrice;
                //Model.MeterTotalPrice = Entity.RoadMeterTotalPrice;
            }

            //Model.PackQty = Entity.PackQty;
            //Model.PackUnit = Entity.PackUnit;
            //Model.AvgSizeUnitName = Entity.PackUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.PackUnit).FirstOrDefault().UnitName;
            //Model.Remarks = Entity.Remarks;

            return Model;
        }

        public List<EXPCIPIItem> GetExpCIPIItemList(string CIPIID)
        {
            long? cIPIID = Convert.ToInt64(CIPIID);
            List<EXP_CIPIItem> searchList = _context.EXP_CIPIItem.Where(m => m.CIPIID == cIPIID).OrderBy(m => m.CIPIItemID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPCIPIItem>();
        }

        public EXPCIPIItem SetToBussinessObject(EXP_CIPIItem Entity)
        {
            EXPCIPIItem Model = new EXPCIPIItem();

            Model.CIPIItemID = Entity.CIPIItemID;
            Model.PIItemID = Entity.PIItemID;
            Model.CIPIID = Entity.CIPIID;
            Model.Commodity = Entity.Commodity;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleName = Entity.ArticleID == null ? "" : _context.Sys_Article.Where(m => m.ArticleID == Entity.ArticleID).FirstOrDefault().ArticleName;
            Model.AvgSize = Entity.AvgSize;
            Model.AvgSizeUnit = Entity.AvgSizeUnit;
            Model.AvgSizeUnitName = Entity.AvgSizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AvgSizeUnit).FirstOrDefault().UnitName;
            Model.SideDescription = Entity.SideDescription;
            Model.SelectionRange = Entity.SelectionRange;
            Model.Thickness = Entity.Thickness;
            Model.ThicknessUnit = Entity.ThicknessUnit;
            Model.ThicknessUnitName = Entity.ThicknessUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.ThicknessUnit).FirstOrDefault().UnitName;
            Model.ThicknessAt = Entity.ThicknessAt;
            if (Entity.ThicknessAt == "AFSV")
                Model.ThicknessAtName = "After Shaving";
            Model.MeterCIQty = Entity.MeterCIQty;
            Model.MeterUnitPrice = Entity.MeterUnitPrice;
            Model.MeterTotalPrice = Entity.MeterTotalPrice;
            Model.FootCIQty = Entity.FootCIQty;
            Model.FootUnitPrice = Entity.FootUnitPrice;
            Model.FootTotalPrice = Entity.FootTotalPrice;
            Model.PackQty = Entity.PackQty;
            Model.PackUnit = Entity.PackUnit;
            Model.PackUnitName = Entity.PackUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.PackUnit).FirstOrDefault().UnitName;
            Model.Remarks = Entity.Remarks;


            return Model;
        }

        public List<EXPCIPIItemColor> GetExpCIItemColorList(string CIPIItemID)
        {
            long ciPIItemID = Convert.ToInt64(CIPIItemID);
            List<EXP_CIPIItemColor> searchList = _context.EXP_CIPIItemColor.Where(m => m.CIPIItemID == ciPIItemID).OrderByDescending(m => m.CIPIItemID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<EXPCIPIItemColor>();
        }

        public EXPCIPIItemColor SetToBussinessObject(EXP_CIPIItemColor Entity)
        {
            EXPCIPIItemColor Model = new EXPCIPIItemColor();

            Model.CIPIItemColorID = Entity.CIPIItemColorID;
            Model.CIPIItemID = Entity.CIPIItemID;
            Model.MaterialNo = Entity.MaterialNo;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.AvgSize = Entity.AvgSize;
            Model.AvgSizeUnit = Entity.AvgSizeUnit;
            Model.AvgSizeUnitName = Entity.AvgSizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AvgSizeUnit).FirstOrDefault().UnitName;
            Model.SideDescription = Entity.SideDescription;
            Model.SelectionRange = Entity.SelectionRange;
            Model.Thickness = Entity.Thickness;
            Model.ThicknessUnit = Entity.ThicknessUnit;
            Model.ThicknessUnitName = Entity.ThicknessUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.ThicknessUnit).FirstOrDefault().UnitName;
            Model.ThicknessAt = Entity.ThicknessAt;

            //if (Entity.MeterColorQty == null)
            //    Model.MeterColorQty = null;// ? null : Convert.ToDecimal(String.Format("{0:0.00}", Entity.MeterColorQty)); //Entity.MeterColorQty;
            //else
            //    Model.MeterUnitPrice = Convert.ToDecimal(String.Format("{0:0.00}", Entity.MeterUnitPrice)); //Entity.MeterUnitPrice;
            //if (Entity.MeterColorQty == null)
            //    Model.MeterTotalPrice = null;
            //else
            //    Model.MeterTotalPrice = Entity.MeterTotalPrice == null ? 0 : Convert.ToDecimal(String.Format("{0:0.00}", Entity.MeterTotalPrice)); //Entity.MeterTotalPrice;

            Model.MeterColorQty = Entity.MeterColorQty;
            Model.MeterUnitPrice = Entity.MeterUnitPrice;
            Model.MeterTotalPrice = Entity.MeterTotalPrice;

            Model.FootColorQty = Entity.FootColorQty;
            Model.FootUnitPrice = Entity.FootUnitPrice;
            Model.FootTotalPrice = Entity.FootTotalPrice;
            Model.PackQty = Entity.PackQty;
            Model.PackUnit = Entity.PackUnit;
            Model.AvgSizeUnitName = Entity.PackUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.PackUnit).FirstOrDefault().UnitName;
            Model.Remarks = Entity.Remarks;

            return Model;
        }

        public List<SlsBuyerOrderPrice> GetBuyerOrderPriceList(string PIID)
        {
            long pIID = Convert.ToInt64(PIID);
            long buyerOrderID = Convert.ToInt64(_context.EXP_LeatherPI.Where(m => m.PIID == pIID).FirstOrDefault().BuyerOrderID);
            List<SLS_BuyerOrderPrice> searchList = _context.SLS_BuyerOrderPrice.Where(m => m.BuyerOrderID == buyerOrderID).OrderByDescending(m => m.BuyerOrderPriceID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SlsBuyerOrderPrice>();
        }

        public SlsBuyerOrderPrice SetToBussinessObject(SLS_BuyerOrderPrice Entity)
        {
            SlsBuyerOrderPrice Model = new SlsBuyerOrderPrice();

            Model.PIDeliveryMode = Entity.PIDeliveryMode;
            if (Entity.PIDeliveryMode == "BA")
                Model.OrdDeliveryModeName = "By Air";
            else if (Entity.PIDeliveryMode == "BS")
                Model.OrdDeliveryModeName = "By Bus";
            else if (Entity.PIDeliveryMode == "BR")
                Model.OrdDeliveryModeName = "By Road";
            else
                Model.OrdDeliveryModeName = "";
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleName = Entity.ArticleID == null ? "" : _context.Sys_Article.Where(m => m.ArticleID == Entity.ArticleID).FirstOrDefault().ArticleName;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.PIAverageUnitPrice = Entity.PIAverageUnitPrice;
            Model.PIDeliveryModeNote = Entity.PIDeliveryModeNote;
            //Model.AvgSizeUnitName = Entity.AvgSizeUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AvgSizeUnit).FirstOrDefault().UnitName;
            //Model.SideDescription = Entity.SideDescription;
            //Model.SelectionRange = Entity.SelectionRange;
            //Model.Thickness = Entity.Thickness;
            //Model.ThicknessUnit = Entity.ThicknessUnit;
            //Model.ThicknessUnitName = Entity.ThicknessUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.ThicknessUnit).FirstOrDefault().UnitName;
            //Model.ThicknessAt = Entity.ThicknessAt;

            //Model.MeterColorQty = Entity.MeterColorQty;
            //Model.MeterUnitPrice = Entity.MeterUnitPrice;
            //Model.MeterTotalPrice = Entity.MeterTotalPrice;
            //Model.FootColorQty = Entity.FootColorQty;
            //Model.FootUnitPrice = Entity.FootUnitPrice;

            ////Model.FootTotalPrice = Entity.FootTotalPrice;

            //Model.FootTotalPrice = Entity.FootColorQty * Entity.FootUnitPrice;

            //Model.PackQty = Entity.PackQty;
            //Model.PackUnit = Entity.PackUnit;
            //Model.AvgSizeUnitName = Entity.PackUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.PackUnit).FirstOrDefault().UnitName;
            //Model.Remarks = Entity.Remarks;

            return Model;
        }

        public ValidationMsg DeletedCIInfo(long? CIID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var itemList = _context.EXP_CIPI.Where(m => m.CIID == CIID).ToList();

                if (itemList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteplElement = _context.EXP_CI.First(m => m.CIID == CIID);
                    _context.EXP_CI.Remove(deleteplElement);
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

        public ValidationMsg DeletedCIPIItem(long? CIPIItemID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var itemList = _context.EXP_CIPIItemColor.Where(m => m.CIPIItemID == CIPIItemID).ToList();

                if (itemList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteElement = _context.EXP_CIPIItem.First(m => m.CIPIItemID == CIPIItemID);
                    _context.EXP_CIPIItem.Remove(deleteElement);
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

        public ValidationMsg DeletedCIPIItemColor(long? CIPIItemColorID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var deleteElement = _context.EXP_CIPIItemColor.First(m => m.CIPIItemColorID == CIPIItemColorID);
                _context.EXP_CIPIItemColor.Remove(deleteElement);

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
