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

    public class DalWetBlueProductionSchedule
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long YearMonID = 0;
        public long ScheduleDateID = 0;
        public string ScheduleNo = string.Empty;
        private int stockError = 0;
        private decimal sidePcs = 0;
        private decimal productionPcs = 0;

        public DalWetBlueProductionSchedule()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(PrdYearMonth model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.ScheduleNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.ScheduleNo != null)
                        {
                            #region YearMonth

                            PRD_YearMonth tblYearMonth = SetToModelObject(model, userid);
                            _context.PRD_YearMonth.Add(tblYearMonth);
                            _context.SaveChanges();

                            #endregion

                            #region Schedule

                            model.YearMonID = tblYearMonth.YearMonID;
                            PRD_YearMonthSchedule tblYearMonthSchedule = SetToYearMonthScheduleModelObject(model, userid);
                            _context.PRD_YearMonthSchedule.Add(tblYearMonthSchedule);
                            _context.SaveChanges();

                            #endregion

                            #region Save ScheduleDate Records

                            if (model.PrdYearMonthScheduleDateList != null)
                            {
                                foreach (PrdYearMonthScheduleDate objPrdYearMonthScheduleDate in model.PrdYearMonthScheduleDateList)
                                {
                                    objPrdYearMonthScheduleDate.ScheduleID = tblYearMonthSchedule.ScheduleID;
                                    objPrdYearMonthScheduleDate.ProductionStatus = "Schedule";
                                    //objPrdYearMonthScheduleDate.ProductionNo = DalCommon.GetPreDefineNextCodeByUrl("IssueToProduction/IssueToProduction");
                                    objPrdYearMonthScheduleDate.ProductionNo = DalCommon.GetPreDefineNextCodeByUrl("ChemicalConsumption/ChemConsumption");
                                    objPrdYearMonthScheduleDate.ScheduleStartDate = objPrdYearMonthScheduleDate.ScheduleStartDate.Contains("/") ? objPrdYearMonthScheduleDate.ScheduleStartDate : Convert.ToDateTime(objPrdYearMonthScheduleDate.ScheduleStartDate).ToString("dd/MM/yyyy");
                                    objPrdYearMonthScheduleDate.ScheduleEndDate = objPrdYearMonthScheduleDate.ScheduleEndDate.Contains("/") ? objPrdYearMonthScheduleDate.ScheduleEndDate : Convert.ToDateTime(objPrdYearMonthScheduleDate.ScheduleEndDate).ToString("dd/MM/yyyy");

                                    PRD_YearMonthScheduleDate tblInvStoreTransRequest = SetToModelObject(objPrdYearMonthScheduleDate, userid);
                                    _context.PRD_YearMonthScheduleDate.Add(tblInvStoreTransRequest);
                                    _context.SaveChanges();

                                    #region Save Schedule Purchase Item List

                                    if (model.PrdYearMonthSchedulePurchaseList != null)
                                    {
                                        foreach (PrdYearMonthSchedulePurchase objPrdYearMonthSchedulePurchase in model.PrdYearMonthSchedulePurchaseList)
                                        {
                                            objPrdYearMonthSchedulePurchase.ScheduleDateID = tblInvStoreTransRequest.ScheduleDateID;
                                            var processEffect = _context.Sys_ProductionProces.Where(m => m.ProcessID == model.ProductionProcessID).FirstOrDefault().ProcessEffect;
                                            if (processEffect == "CL")
                                            {
                                                if ((objPrdYearMonthSchedulePurchase.ProductionSide == null) || (objPrdYearMonthSchedulePurchase.ProductionSide == null))
                                                    sidePcs = 0;
                                                else
                                                    sidePcs = (decimal)(objPrdYearMonthSchedulePurchase.ProductionSide / 2);

                                                if ((objPrdYearMonthSchedulePurchase.ProductionPcs == null) || (objPrdYearMonthSchedulePurchase.ProductionPcs == null))
                                                    productionPcs = 0;
                                                else
                                                    productionPcs = (decimal)(objPrdYearMonthSchedulePurchase.ProductionPcs);


                                                productionPcs = (decimal)(productionPcs + sidePcs);

                                                //if (objPrdYearMonthSchedulePurchase.ClosingQty >
                                                //    objPrdYearMonthSchedulePurchase.ProductionPcs)
                                                if (objPrdYearMonthSchedulePurchase.ClosingQty >= productionPcs)
                                                {
                                                    PRD_YearMonthSchedulePurchase tblYearMonthSchedulePurchase =
                                                        SetToModelObject(objPrdYearMonthSchedulePurchase, userid);
                                                    _context.PRD_YearMonthSchedulePurchase.Add(
                                                        tblYearMonthSchedulePurchase);
                                                }
                                                else
                                                {
                                                    stockError = 1;
                                                }
                                            }
                                            else
                                            {
                                                PRD_YearMonthSchedulePurchase tblYearMonthSchedulePurchase =
                                                         SetToModelObject(objPrdYearMonthSchedulePurchase, userid);
                                                _context.PRD_YearMonthSchedulePurchase.Add(
                                                    tblYearMonthSchedulePurchase);
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            if (stockError == 0)
                            {
                                _context.SaveChanges();
                                tx.Complete();
                                YearMonID = tblYearMonth.YearMonID;
                                ScheduleNo = model.ScheduleNo;
                                _vmMsg.Type = Enums.MessageType.Success;
                                _vmMsg.Msg = "Saved Successfully.";
                            }
                            else
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "Not Enoung Quantity in Stock.";
                            }
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "ScheduleNo Predefine Value not Found.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Schedule Year,Month Combination Data Already Exit.";
                }
            }
            return _vmMsg;
        }

        public ValidationMsg Update(PrdYearMonth model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region YearMonth

                        PRD_YearMonth CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.PRD_YearMonth.First(m => m.YearMonID == model.YearMonID);

                        OriginalEntity.ScheduleYear = CurrentEntity.ScheduleYear;
                        OriginalEntity.ScheduleMonth = CurrentEntity.ScheduleMonth;
                        OriginalEntity.ScheduleFor = CurrentEntity.ScheduleFor;
                        OriginalEntity.ProductionFloor = CurrentEntity.ProductionFloor;
                        OriginalEntity.ConcernStore = CurrentEntity.ConcernStore;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #endregion

                        #region Schedule

                        //if (model.ScheduleNo == "Auto Generated")
                        if (string.IsNullOrEmpty(model.ScheduleNo))
                        {
                            model.YearMonID = model.YearMonID;
                            model.ScheduleNo = DalCommon.GetPreDefineNextCodeByUrl("WetBlueProductionSchedule/WetBlueProductionSchedule");
                            PRD_YearMonthSchedule tblYearMonthSchedule = SetToYearMonthScheduleModelObject(model, userid);
                            _context.PRD_YearMonthSchedule.Add(tblYearMonthSchedule);
                            _context.SaveChanges();
                        }
                        else
                        {
                            PRD_YearMonthSchedule CurrentScheduleEntity = SetToYearMonthScheduleModelObject(model, userid);
                            var OriginalScheduleEntity = _context.PRD_YearMonthSchedule.First(m => m.ScheduleNo == model.ScheduleNo);

                            OriginalScheduleEntity.PrepareDate = CurrentScheduleEntity.PrepareDate;
                            OriginalScheduleEntity.ProductionProcessID = CurrentScheduleEntity.ProductionProcessID;
                            OriginalScheduleEntity.ModifiedBy = userid;
                            OriginalScheduleEntity.ModifiedOn = DateTime.Now;
                        }


                        #endregion

                        #region Save YearMonthScheduleDate List

                        if (model.PrdYearMonthScheduleDateList != null)
                        {
                            foreach (PrdYearMonthScheduleDate objPrdYearMonthScheduleDate in model.PrdYearMonthScheduleDateList)
                            {
                                if (objPrdYearMonthScheduleDate.ScheduleDateID == 0)
                                {
                                    objPrdYearMonthScheduleDate.ScheduleID = _context.PRD_YearMonthSchedule.Where(m => m.ScheduleNo == model.ScheduleNo).FirstOrDefault().ScheduleID;
                                    objPrdYearMonthScheduleDate.ProductionStatus = "Schedule";
                                    //objPrdYearMonthScheduleDate.ProductionNo = DalCommon.GetPreDefineNextCodeByUrl("IssueToProduction/IssueToProduction");
                                    objPrdYearMonthScheduleDate.ProductionNo = DalCommon.GetPreDefineNextCodeByUrl("ChemicalConsumption/ChemConsumption");
                                    objPrdYearMonthScheduleDate.ScheduleStartDate = objPrdYearMonthScheduleDate.ScheduleStartDate.Contains("/") ? objPrdYearMonthScheduleDate.ScheduleStartDate : Convert.ToDateTime(objPrdYearMonthScheduleDate.ScheduleStartDate).ToString("dd/MM/yyyy");
                                    objPrdYearMonthScheduleDate.ScheduleEndDate = objPrdYearMonthScheduleDate.ScheduleEndDate.Contains("/") ? objPrdYearMonthScheduleDate.ScheduleEndDate : Convert.ToDateTime(objPrdYearMonthScheduleDate.ScheduleEndDate).ToString("dd/MM/yyyy");

                                    PRD_YearMonthScheduleDate tblYearMonthScheduleDate = SetToModelObject(objPrdYearMonthScheduleDate, userid);
                                    _context.PRD_YearMonthScheduleDate.Add(tblYearMonthScheduleDate);
                                    _context.SaveChanges();
                                    ScheduleDateID = tblYearMonthScheduleDate.ScheduleDateID;
                                }
                                else
                                {
                                    ScheduleDateID = objPrdYearMonthScheduleDate.ScheduleDateID;
                                    //objPrdYearMonthScheduleDate.ScheduleID = _context.PRD_YearMonthSchedule.Where(m => m.ScheduleNo == model.ScheduleNo).FirstOrDefault().ScheduleID;
                                    objPrdYearMonthScheduleDate.ScheduleStartDate = objPrdYearMonthScheduleDate.ScheduleStartDate.Contains("/") ? objPrdYearMonthScheduleDate.ScheduleStartDate : Convert.ToDateTime(objPrdYearMonthScheduleDate.ScheduleStartDate).ToString("dd/MM/yyyy");
                                    objPrdYearMonthScheduleDate.ScheduleEndDate = objPrdYearMonthScheduleDate.ScheduleEndDate.Contains("/") ? objPrdYearMonthScheduleDate.ScheduleEndDate : Convert.ToDateTime(objPrdYearMonthScheduleDate.ScheduleEndDate).ToString("dd/MM/yyyy");

                                    PRD_YearMonthScheduleDate CurrEntity = SetToModelObject(objPrdYearMonthScheduleDate, userid);
                                    var OrgrEntity = _context.PRD_YearMonthScheduleDate.First(m => m.ScheduleDateID == objPrdYearMonthScheduleDate.ScheduleDateID);

                                    //OrgrEntity.ScheduleID = CurrEntity.ScheduleID;
                                    //OrgrEntity.ProductionNo = CurrEntity.ProductionNo;
                                    OrgrEntity.ScheduleStartDate = CurrEntity.ScheduleStartDate;
                                    OrgrEntity.ScheduleEndDate = CurrEntity.ScheduleEndDate;
                                    OrgrEntity.SchedulePcs = CurrEntity.SchedulePcs;
                                    OrgrEntity.ScheduleSide = CurrEntity.ScheduleSide;
                                    OrgrEntity.ScheduleWeight = CurrEntity.ScheduleWeight;
                                    OrgrEntity.ScheduleWeightUnit = CurrEntity.ScheduleWeightUnit;
                                    OrgrEntity.ProductionStatus = CurrEntity.ProductionStatus;
                                    OrgrEntity.Remark = CurrEntity.Remark;
                                    OrgrEntity.ModifiedBy = userid;
                                    OrgrEntity.ModifiedOn = DateTime.Now;
                                }
                            }
                        }

                        #endregion

                        #region Save YearMonthSchedulePurchase Records

                        if (model.PrdYearMonthSchedulePurchaseList != null)
                        {
                            foreach (PrdYearMonthSchedulePurchase objPrdYearMonthSchedulePurchase in model.PrdYearMonthSchedulePurchaseList)
                            {
                                if (objPrdYearMonthSchedulePurchase.SchedulePurchaseID == 0)
                                {
                                    var processEffect = _context.Sys_ProductionProces.Where(m => m.ProcessID == model.ProductionProcessID).FirstOrDefault().ProcessEffect;
                                    if (processEffect == "CL")
                                    {

                                        //if (objPrdYearMonthSchedulePurchase.ClosingQty >=
                                        //    objPrdYearMonthSchedulePurchase.ProductionPcs)
                                        if ((objPrdYearMonthSchedulePurchase.ProductionSide == null) || (objPrdYearMonthSchedulePurchase.ProductionSide == null))
                                            sidePcs = 0;
                                        else
                                            sidePcs = (decimal)(objPrdYearMonthSchedulePurchase.ProductionSide / 2);

                                        if ((objPrdYearMonthSchedulePurchase.ProductionPcs == null) || (objPrdYearMonthSchedulePurchase.ProductionPcs == null))
                                            productionPcs = 0;
                                        else
                                            productionPcs = (decimal)(objPrdYearMonthSchedulePurchase.ProductionPcs);


                                        productionPcs = (decimal)(productionPcs + sidePcs);
                                        if (objPrdYearMonthSchedulePurchase.ClosingQty >= productionPcs)
                                        {
                                            objPrdYearMonthSchedulePurchase.ScheduleDateID = ScheduleDateID;
                                            PRD_YearMonthSchedulePurchase tblChemLocalPurcRecvItem =
                                                SetToModelObject(objPrdYearMonthSchedulePurchase, userid);
                                            _context.PRD_YearMonthSchedulePurchase.Add(tblChemLocalPurcRecvItem);
                                        }
                                        else
                                        {
                                            stockError = 1;
                                        }
                                    }
                                    else
                                    {
                                        objPrdYearMonthSchedulePurchase.ScheduleDateID = ScheduleDateID;
                                        PRD_YearMonthSchedulePurchase tblChemLocalPurcRecvItem =
                                            SetToModelObject(objPrdYearMonthSchedulePurchase, userid);
                                        _context.PRD_YearMonthSchedulePurchase.Add(tblChemLocalPurcRecvItem);
                                    }
                                }
                                else
                                {
                                    var processEffect = _context.Sys_ProductionProces.Where(m => m.ProcessID == model.ProductionProcessID).FirstOrDefault().ProcessEffect;
                                    if (processEffect == "CL")
                                    {
                                        //if (objPrdYearMonthSchedulePurchase.ClosingQty >=
                                        //    objPrdYearMonthSchedulePurchase.ProductionPcs)
                                        if ((objPrdYearMonthSchedulePurchase.ProductionSide == null) || (objPrdYearMonthSchedulePurchase.ProductionSide == null))
                                            sidePcs = 0;
                                        else
                                            sidePcs = (decimal)(objPrdYearMonthSchedulePurchase.ProductionSide / 2);

                                        if ((objPrdYearMonthSchedulePurchase.ProductionPcs == null) || (objPrdYearMonthSchedulePurchase.ProductionPcs == null))
                                            productionPcs = 0;
                                        else
                                            productionPcs = (decimal)(objPrdYearMonthSchedulePurchase.ProductionPcs);


                                        productionPcs = (decimal)(productionPcs + sidePcs);
                                        if (objPrdYearMonthSchedulePurchase.ClosingQty >= productionPcs)
                                        {
                                            PRD_YearMonthSchedulePurchase CurEntity =
                                                SetToModelObject(objPrdYearMonthSchedulePurchase, userid);
                                            var OrgEntity =
                                                _context.PRD_YearMonthSchedulePurchase.First(
                                                    m =>
                                                        m.SchedulePurchaseID ==
                                                        objPrdYearMonthSchedulePurchase.SchedulePurchaseID);

                                            OrgEntity.MachineID = CurEntity.MachineID;
                                            OrgEntity.MachineNo = CurEntity.MachineNo;
                                            OrgEntity.SupplierID = CurEntity.SupplierID;
                                            OrgEntity.PurchaseID = CurEntity.PurchaseID;
                                            OrgEntity.ItemTypeID = CurEntity.ItemTypeID;
                                            OrgEntity.LeatherTypeID = CurEntity.LeatherTypeID;
                                            OrgEntity.LeatherStatusID = CurEntity.LeatherStatusID;
                                            OrgEntity.ProductionPcs = CurEntity.ProductionPcs;
                                            OrgEntity.ProductionSide = CurEntity.ProductionSide;
                                            OrgEntity.ProductionWeight = CurEntity.ProductionWeight;
                                            OrgEntity.WeightUnit = CurEntity.WeightUnit;
                                            OrgEntity.Remark = CurEntity.Remark;
                                            OrgEntity.PurchaseSign = CurEntity.PurchaseSign;
                                            OrgEntity.ModifiedBy = userid;
                                            OrgEntity.ModifiedOn = DateTime.Now;
                                        }
                                        else
                                        {
                                            stockError = 1;
                                        }
                                    }
                                    else
                                    {
                                        PRD_YearMonthSchedulePurchase CurEntity =
                                                SetToModelObject(objPrdYearMonthSchedulePurchase, userid);
                                        var OrgEntity =
                                            _context.PRD_YearMonthSchedulePurchase.First(
                                                m =>
                                                    m.SchedulePurchaseID ==
                                                    objPrdYearMonthSchedulePurchase.SchedulePurchaseID);

                                        OrgEntity.MachineID = CurEntity.MachineID;
                                        OrgEntity.MachineNo = CurEntity.MachineNo;
                                        OrgEntity.SupplierID = CurEntity.SupplierID;
                                        OrgEntity.PurchaseID = CurEntity.PurchaseID;
                                        OrgEntity.ItemTypeID = CurEntity.ItemTypeID;
                                        OrgEntity.LeatherTypeID = CurEntity.LeatherTypeID;
                                        OrgEntity.LeatherStatusID = CurEntity.LeatherStatusID;
                                        OrgEntity.ProductionPcs = CurEntity.ProductionPcs;
                                        OrgEntity.ProductionSide = CurEntity.ProductionSide;
                                        OrgEntity.ProductionWeight = CurEntity.ProductionWeight;
                                        OrgEntity.WeightUnit = CurEntity.WeightUnit;
                                        OrgEntity.Remark = CurEntity.Remark;
                                        OrgEntity.PurchaseSign = CurEntity.PurchaseSign;
                                        OrgEntity.ModifiedBy = userid;
                                        OrgEntity.ModifiedOn = DateTime.Now;
                                    }
                                }
                            }
                        }

                        #endregion

                        if (stockError == 0)
                        {
                            _context.SaveChanges();
                            tx.Complete();
                            YearMonID = model.YearMonID;
                            ScheduleNo = model.ScheduleNo;
                            _vmMsg.Type = Enums.MessageType.Update;
                            _vmMsg.Msg = "Updated Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Not Enoung Quantity in Stock.";
                        }
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

        public PRD_YearMonth SetToModelObject(PrdYearMonth model, int userid)
        {
            PRD_YearMonth Entity = new PRD_YearMonth();

            Entity.YearMonID = model.YearMonID;
            Entity.ScheduleYear = model.ScheduleYear;
            Entity.ScheduleMonth = model.ScheduleMonth;
            Entity.ScheduleFor = "WBP";//model.ScheduleFor;
            Entity.ProductionFloor = model.ProductionFloor;
            Entity.ConcernStore = model.ConcernStore;
            Entity.Remarks = model.Remarks;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public PRD_YearMonthSchedule SetToYearMonthScheduleModelObject(PrdYearMonth model, int userid)
        {
            PRD_YearMonthSchedule Entity = new PRD_YearMonthSchedule();

            Entity.YearMonID = model.YearMonID;
            Entity.ScheduleNo = model.ScheduleNo;
            Entity.PrepareDate = DalCommon.SetDate(model.PrepareDate);
            Entity.ScheduleStatus = "SCH";
            Entity.ProductionProcessID = model.ProductionProcessID;
            Entity.Remarks = model.Remarks;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public PRD_YearMonthScheduleDate SetToModelObject(PrdYearMonthScheduleDate model, int userid)
        {
            PRD_YearMonthScheduleDate Entity = new PRD_YearMonthScheduleDate();

            Entity.ScheduleDateID = model.ScheduleDateID;
            Entity.ScheduleID = model.ScheduleID;
            Entity.ProductionNo = model.ProductionNo;
            Entity.ScheduleStartDate = DalCommon.SetDate(model.ScheduleStartDate);
            Entity.ScheduleEndDate = DalCommon.SetDate(model.ScheduleEndDate);
            Entity.SchedulePcs = model.SchedulePcs;
            Entity.ScheduleSide = model.ScheduleSide;
            Entity.ScheduleWeight = model.ScheduleWeight;

            if (string.IsNullOrEmpty(model.UnitName))
                Entity.ScheduleWeightUnit = null;
            else
                Entity.ScheduleWeightUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.UnitName).FirstOrDefault().UnitID);
            switch (model.ProductionStatus)
            {
                case "Schedule":
                    Entity.ProductionStatus = "SCH";
                    break;
                case "On Going":
                    Entity.ProductionStatus = "ONG";
                    break;
                case "Postponed":
                    Entity.ProductionStatus = "POS";
                    break;
                case "Completed":
                    Entity.ProductionStatus = "CMP";
                    break;
                default:
                    Entity.ProductionStatus = "SCH";
                    break;
            }
            Entity.RecordStatus = "NCF";
            Entity.Remark = model.Remark;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public PRD_YearMonthSchedulePurchase SetToModelObject(PrdYearMonthSchedulePurchase model, int userid)
        {
            PRD_YearMonthSchedulePurchase Entity = new PRD_YearMonthSchedulePurchase();

            Entity.SchedulePurchaseID = model.SchedulePurchaseID;
            Entity.ScheduleDateID = model.ScheduleDateID;
            Entity.ProductionNo = model.ProductionNo;
            Entity.MachineID = model.MachineID;
            Entity.MachineNo = model.MachineNo == "Press F9" ? null : model.MachineNo;
            Entity.SupplierID = model.SupplierID;
            Entity.PurchaseID = model.PurchaseID;
            if (string.IsNullOrEmpty(model.ItemTypeName))
                Entity.ItemTypeID = null;
            else
                Entity.ItemTypeID = Convert.ToByte(_context.Sys_ItemType.Where(m => m.ItemTypeName == model.ItemTypeName).FirstOrDefault().ItemTypeID);
            if (string.IsNullOrEmpty(model.LeatherTypeName))
                Entity.LeatherTypeID = null;
            else
                Entity.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == model.LeatherTypeName).FirstOrDefault().LeatherTypeID);
            if (string.IsNullOrEmpty(model.LeatherStatusName))
                Entity.LeatherStatusID = null;
            else
                Entity.LeatherStatusID = Convert.ToByte(_context.Sys_LeatherStatus.Where(m => m.LeatherStatusName == model.LeatherStatusName).FirstOrDefault().LeatherStatusID);
            //Entity.ProductionPcs = model.ProductionPcs == null ? 0 : model.ProductionPcs;
            //Entity.ProductionSide = model.ProductionSide == null ? 0 : model.ProductionSide;
            Entity.ProductionPcs = model.ProductionPcs;
            Entity.ProductionSide = model.ProductionSide;
            Entity.ProductionWeight = model.ProductionWeight;
            if (string.IsNullOrEmpty(model.UnitName))
                Entity.WeightUnit = null;
            else
                Entity.WeightUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == model.UnitName).FirstOrDefault().UnitID);
            Entity.Remark = model.Remark;
            Entity.PurchaseSign = model.PurchaseSign;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public long GetYearMonID()
        {
            return YearMonID;
        }

        public string GetScheduleNo()
        {
            return ScheduleNo;
        }

        public List<PrdYearMonth> GetWetBlueYearMonthList()
        {
            List<PRD_YearMonth> searchList = _context.PRD_YearMonth.Where(m => m.ScheduleFor == "WBP").OrderByDescending(m => m.YearMonID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdYearMonth>();
        }

        public PrdYearMonth SetToBussinessObject(PRD_YearMonth Entity)
        {
            PrdYearMonth Model = new PrdYearMonth();

            Model.YearMonID = Entity.YearMonID;
            Model.ScheduleYear = Entity.ScheduleYear;
            Model.ScheduleMonth = Entity.ScheduleMonth;
            switch (Model.ScheduleMonth)
            {
                case "01":
                    Model.ScheduleMonthName = "January";
                    break;
                case "02":
                    Model.ScheduleMonthName = "February";
                    break;
                case "03":
                    Model.ScheduleMonthName = "March";
                    break;
                case "04":
                    Model.ScheduleMonthName = "April";
                    break;
                case "05":
                    Model.ScheduleMonthName = "May";
                    break;
                case "06":
                    Model.ScheduleMonthName = "June";
                    break;
                case "07":
                    Model.ScheduleMonthName = "July";
                    break;
                case "08":
                    Model.ScheduleMonthName = "August";
                    break;
                case "09":
                    Model.ScheduleMonthName = "September";
                    break;
                case "10":
                    Model.ScheduleMonthName = "October";
                    break;
                case "11":
                    Model.ScheduleMonthName = "November";
                    break;
                case "12":
                    Model.ScheduleMonthName = "December";
                    break;
                default:
                    Model.ScheduleMonthName = "";
                    break;
            }
            Model.ScheduleFor = Entity.ScheduleFor;
            switch (Model.ScheduleFor)
            {
                case "WBP":
                    Model.ScheduleForName = "Wet Blue Production";
                    break;
                case "WBR":
                    Model.ScheduleForName = "Wet Bule Requisition";
                    break;
                case "CRP":
                    Model.ScheduleForName = "Crust Production";
                    break;
                case "CRR":
                    Model.ScheduleForName = "Crust Requisition";
                    break;
                case "FNP":
                    Model.ScheduleForName = "Finished Production";
                    break;
                case "FNR":
                    Model.ScheduleForName = "Finished Requisition";
                    break;
                default:
                    Model.ScheduleForName = "";
                    break;
            }
            Model.ProductionFloor = Entity.ProductionFloor;
            Model.ProductionFloorName = Model.ProductionFloor == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Model.ProductionFloor).FirstOrDefault().StoreName;
            Model.ConcernStore = Entity.ConcernStore;
            Model.ConcernStoreName = Model.ConcernStore == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Model.ConcernStore).FirstOrDefault().StoreName;
            Model.RecordStatus = Entity.RecordStatus;

            return Model;
        }

        public List<PrdYearMonth> GetWetBlueYearMonthScheduleList(string YearMonID)
        {
            if (!string.IsNullOrEmpty(YearMonID))
            {
                long? yearMonID = Convert.ToInt64(YearMonID);
                List<PRD_YearMonthSchedule> searchList = _context.PRD_YearMonthSchedule.Where(m => m.YearMonID == yearMonID).OrderByDescending(m => m.ScheduleID).ToList();
                return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdYearMonth>();
            }
            return null;
        }

        public List<PrdYearMonthScheduleDate> GetYearMonthScheduleDateList(string ScheduleNo)
        {
            long? scheduleID = Convert.ToInt64(_context.PRD_YearMonthSchedule.Where(m => m.ScheduleNo == ScheduleNo).FirstOrDefault().ScheduleID);
            List<PRD_YearMonthScheduleDate> searchList = _context.PRD_YearMonthScheduleDate.Where(m => m.ScheduleID == scheduleID).OrderByDescending(m => m.ScheduleDateID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdYearMonthScheduleDate>();
        }

        public List<PrdYearMonthSchedulePurchase> GetYearMonthSchedulePurchaseList(string ScheduleDateID, string StoreId)
        {
            long scheduleDateID = Convert.ToInt64(ScheduleDateID);
            byte storeId = Convert.ToByte(StoreId);
            List<PRD_YearMonthSchedulePurchase> searchList = _context.PRD_YearMonthSchedulePurchase.Where(m => m.ScheduleDateID == scheduleDateID).OrderByDescending(m => m.SchedulePurchaseID).ToList();
            return searchList.Select(c => SetToBussinessObject(c, storeId)).ToList<PrdYearMonthSchedulePurchase>();
        }

        public List<PrdYearMonthSchedulePurchase> GetYearMonthSchedulePurchaseList(string ScheduleDateID)
        {
            long scheduleDateID = Convert.ToInt64(ScheduleDateID);
            List<PRD_YearMonthSchedulePurchase> searchList = _context.PRD_YearMonthSchedulePurchase.Where(m => m.ScheduleDateID == scheduleDateID).OrderByDescending(m => m.SchedulePurchaseID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdYearMonthSchedulePurchase>();
        }

        public PrdYearMonthScheduleDate SetToBussinessObject(PRD_YearMonthScheduleDate Entity)
        {
            PrdYearMonthScheduleDate Model = new PrdYearMonthScheduleDate();

            Model.ScheduleDateID = Entity.ScheduleDateID;
            Model.ScheduleID = Entity.ScheduleID;
            Model.ProductionNo = Entity.ProductionNo;
            Model.ScheduleStartDate = Convert.ToDateTime(Entity.ScheduleStartDate).ToString("dd/MM/yyyy");
            Model.ScheduleEndDate = Convert.ToDateTime(Entity.ScheduleEndDate).ToString("dd/MM/yyyy");
            Model.SchedulePcs = Entity.SchedulePcs;
            Model.ScheduleSide = Entity.ScheduleSide;
            Model.ScheduleWeight = Entity.ScheduleWeight;
            Model.ScheduleWeightUnit = Entity.ScheduleWeightUnit;
            Model.UnitName = Entity.ScheduleWeightUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.ScheduleWeightUnit).FirstOrDefault().UnitName;
            switch (Entity.ProductionStatus)
            {
                case "SCH":
                    Model.ProductionStatus = "Schedule";
                    break;
                case "ONG":
                    Model.ProductionStatus = "On Going";
                    break;
                case "POS":
                    Model.ProductionStatus = "Postponed";
                    break;
                case "CMP":
                    Model.ProductionStatus = "Completed";
                    break;
                default:
                    Model.ProductionStatus = "";
                    break;
            }
            Model.Remark = Entity.Remark;

            return Model;
        }

        public PrdYearMonthSchedulePurchase SetToBussinessObject(PRD_YearMonthSchedulePurchase Entity, byte storeId)
        {
            PrdYearMonthSchedulePurchase Model = new PrdYearMonthSchedulePurchase();

            Model.SchedulePurchaseID = Entity.SchedulePurchaseID;
            Model.ScheduleDateID = Entity.ScheduleDateID;
            Model.ProductionNo = Entity.ProductionNo;
            Model.MachineID = Entity.MachineID;
            Model.MachineNo = string.IsNullOrEmpty(Entity.MachineNo) ? "Press F9" : Entity.MachineNo;
            Model.SupplierID = Entity.SupplierID;
            Model.SupplierName = Entity.SupplierID == null ? "Press F9" : _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierName;
            Model.PurchaseID = Entity.PurchaseID;
            Model.PurchaseNo = Entity.PurchaseID == null ? "Press F9" : _context.Prq_Purchase.Where(m => m.PurchaseID == Entity.PurchaseID).FirstOrDefault().PurchaseNo;
            Model.PurchaseDate = Entity.PurchaseID == null ? "" : Convert.ToDateTime(_context.Prq_Purchase.Where(m => m.PurchaseID == Entity.PurchaseID).FirstOrDefault().PurchaseDate).ToString("dd/MM/yyyy");
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            Model.LeatherTypeName = Entity.LeatherTypeID == null ? "" : _context.Sys_LeatherType.Where(m => m.LeatherTypeID == Entity.LeatherTypeID).FirstOrDefault().LeatherTypeName;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            if (_context.Inv_StockSupplier.Where(m => m.SupplierID == Entity.SupplierID && m.StoreID == storeId && m.ItemTypeID == Entity.ItemTypeID && m.LeatherType == Entity.LeatherTypeID && m.LeatherStatusID == Entity.LeatherStatusID && m.PurchaseID == Entity.PurchaseID).FirstOrDefault() != null)
            {
                Model.ClosingQty = _context.Inv_StockSupplier.Where(m => m.SupplierID == Entity.SupplierID && m.StoreID == storeId && m.ItemTypeID == Entity.ItemTypeID && m.LeatherType == Entity.LeatherTypeID && m.LeatherStatusID == Entity.LeatherStatusID && m.PurchaseID == Entity.PurchaseID).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingQty;
            }
            Model.ProductionPcs = Entity.ProductionPcs;
            Model.ProductionSide = Entity.ProductionSide;
            Model.ProductionWeight = Entity.ProductionWeight;
            Model.WeightUnit = Entity.WeightUnit;
            Model.UnitName = Entity.WeightUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.WeightUnit).FirstOrDefault().UnitName;
            Model.Remark = Entity.Remark;
            Model.PurchaseSign = Entity.PurchaseSign;

            return Model;
        }

        public PrdYearMonthSchedulePurchase SetToBussinessObject(PRD_YearMonthSchedulePurchase Entity)
        {
            PrdYearMonthSchedulePurchase Model = new PrdYearMonthSchedulePurchase();

            Model.MachineID = Entity.MachineID;
            Model.MachineNo = string.IsNullOrEmpty(Entity.MachineNo) ? "Press F9" : Entity.MachineNo;

            Model.SupplierID = Entity.SupplierID;
            Model.SupplierName = Entity.SupplierID == null ? "Press F9" : _context.Sys_Supplier.Where(m => m.SupplierID == Entity.SupplierID).FirstOrDefault().SupplierName;
            Model.PurchaseID = Entity.PurchaseID;
            Model.PurchaseNo = Entity.PurchaseID == null ? "Press F9" : _context.Prq_Purchase.Where(m => m.PurchaseID == Entity.PurchaseID).FirstOrDefault().PurchaseNo;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            Model.LeatherTypeName = Entity.LeatherTypeID == null ? "" : _context.Sys_LeatherType.Where(m => m.LeatherTypeID == Entity.LeatherTypeID).FirstOrDefault().LeatherTypeName;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            //if (_context.Inv_StockSupplier.Where(m => m.SupplierID == Entity.SupplierID && m.StoreID == storeId && m.ItemTypeID == Entity.ItemTypeID && m.LeatherType == Entity.LeatherTypeID && m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault() != null)
            //{
            //    Model.ClosingQty = _context.Inv_StockSupplier.Where(m => m.SupplierID == Entity.SupplierID && m.StoreID == storeId && m.ItemTypeID == Entity.ItemTypeID && m.LeatherType == Entity.LeatherTypeID && m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().ClosingQty;
            //}
            Model.ProductionPcs = Entity.ProductionPcs;
            Model.ProductionSide = Entity.ProductionSide;
            Model.ProductionWeight = Entity.ProductionWeight;
            //Model.WeightUnit = Entity.WeightUnit;
            //Model.UnitName = Entity.WeightUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.WeightUnit).FirstOrDefault().UnitName;
            Model.Remark = Entity.Remark;
            Model.PurchaseSign = Entity.PurchaseSign;

            return Model;
        }

        public List<PrdYearMonthSchedulePurchase> GetPurchaseNoInfo()
        {
            List<Prq_Purchase> searchList = _context.Prq_Purchase.ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdYearMonthSchedulePurchase>();
        }

        public PrdYearMonthSchedulePurchase SetToBussinessObject(Prq_Purchase Entity)
        {
            PrdYearMonthSchedulePurchase Model = new PrdYearMonthSchedulePurchase();

            Model.PurchaseID = Entity.PurchaseID;
            Model.PurchaseNo = _context.Prq_Purchase.Where(m => m.PurchaseID == Entity.PurchaseID).FirstOrDefault().PurchaseNo;

            return Model;
        }

        public ValidationMsg DeletedYearMonthSchedule(string ScheduleNo)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var ScheduleID = _context.PRD_YearMonthSchedule.Where(m => m.ScheduleNo == ScheduleNo).FirstOrDefault().ScheduleID;
                var scheduleList = _context.PRD_YearMonthScheduleDate.Where(m => m.ScheduleID == ScheduleID).ToList();
                if (scheduleList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteYearMonthSchedule = _context.PRD_YearMonthSchedule.First(m => m.ScheduleID == ScheduleID);
                    _context.PRD_YearMonthSchedule.Remove(deleteYearMonthSchedule);
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

        public ValidationMsg DeletedYearMonthScheduleDate(long ScheduleDateID, string RecordStatus)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                if (RecordStatus != "CNF")
                {
                    var scheduleList = _context.PRD_YearMonthSchedulePurchase.Where(m => m.ScheduleDateID == ScheduleDateID).ToList();
                    if (scheduleList.Count > 0)
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Child Record Found.";
                    }
                    else
                    {
                        var deleteElement = _context.PRD_YearMonthScheduleDate.First(m => m.ScheduleDateID == ScheduleDateID);
                        _context.PRD_YearMonthScheduleDate.Remove(deleteElement);
                        _context.SaveChanges();

                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Deleted Successfully.";
                    }
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Confirmed Record Can not be Deleted.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        public ValidationMsg DeletedYearMonthSchedulePurchase(long schedulePurchaseID, string RecordStatus)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                if (RecordStatus != "CNF")
                {
                    var deleteElement = _context.PRD_YearMonthSchedulePurchase.First(m => m.SchedulePurchaseID == schedulePurchaseID);
                    _context.PRD_YearMonthSchedulePurchase.Remove(deleteElement);

                    _context.SaveChanges();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Confirmed Record Can not be Deleted.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        //public List<PrdYearMonth> GetWetBlueProductionSchedule()
        //{
        //    List<PRD_YearMonthSchedule> searchList = _context.PRD_YearMonthSchedule.OrderByDescending(m => m.ScheduleID).ToList();
        //    return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdYearMonth>();
        //}

        public PrdYearMonth SetToBussinessObject(PRD_YearMonthSchedule Entity)
        {
            PrdYearMonth Model = new PrdYearMonth();

            Model.ScheduleID = Entity.ScheduleID;
            Model.ScheduleNo = Entity.ScheduleNo;
            Model.YearMonID = Convert.ToInt64(Entity.YearMonID);
            Model.PrepareDate = Convert.ToDateTime(Entity.PrepareDate).ToString("dd/MM/yyyy");
            Model.ProductionProcessID = Entity.ProductionProcessID;
            Model.ProcessName = Entity.ProductionProcessID == null ? "" : _context.Sys_ProductionProces.Where(m => m.ProcessID == Entity.ProductionProcessID).FirstOrDefault().ProcessName;
            Model.ScheduleYear = Entity.YearMonID == null ? "" : _context.PRD_YearMonth.Where(m => m.YearMonID == Entity.YearMonID).FirstOrDefault().ScheduleYear;
            Model.ScheduleMonth = Entity.YearMonID == null ? "" : _context.PRD_YearMonth.Where(m => m.YearMonID == Entity.YearMonID).FirstOrDefault().ScheduleMonth;
            switch (Model.ScheduleMonth)
            {
                case "01":
                    Model.ScheduleMonthName = "January";
                    break;
                case "02":
                    Model.ScheduleMonthName = "February";
                    break;
                case "03":
                    Model.ScheduleMonthName = "March";
                    break;
                case "04":
                    Model.ScheduleMonthName = "April";
                    break;
                case "05":
                    Model.ScheduleMonthName = "May";
                    break;
                case "06":
                    Model.ScheduleMonthName = "June";
                    break;
                case "07":
                    Model.ScheduleMonthName = "July";
                    break;
                case "08":
                    Model.ScheduleMonthName = "August";
                    break;
                case "09":
                    Model.ScheduleMonthName = "September";
                    break;
                case "10":
                    Model.ScheduleMonthName = "October";
                    break;
                case "11":
                    Model.ScheduleMonthName = "November";
                    break;
                case "12":
                    Model.ScheduleMonthName = "December";
                    break;
                default:
                    Model.ScheduleMonthName = "";
                    break;
            }
            Model.ScheduleFor = Entity.YearMonID == null ? "" : _context.PRD_YearMonth.Where(m => m.YearMonID == Entity.YearMonID).FirstOrDefault().ScheduleFor;
            switch (Model.ScheduleFor)
            {
                case "WBP":
                    Model.ScheduleForName = "Wet Blue Production";
                    break;
                case "WBR":
                    Model.ScheduleForName = "Wet Bule Requisition";
                    break;
                case "CRP":
                    Model.ScheduleForName = "Crust Requisition";
                    break;
                case "CRR":
                    Model.ScheduleForName = "Crust Requisition";
                    break;
                case "FNP":
                    Model.ScheduleForName = "Finished Production";
                    break;
                default:
                    Model.ScheduleForName = "";
                    break;
            }
            Model.ProductionFloor = Entity.YearMonID == null ? null : _context.PRD_YearMonth.Where(m => m.YearMonID == Entity.YearMonID).FirstOrDefault().ProductionFloor;
            Model.ConcernStore = Entity.YearMonID == null ? null : _context.PRD_YearMonth.Where(m => m.YearMonID == Entity.YearMonID).FirstOrDefault().ConcernStore;
            Model.ProductionFloorName = Model.ProductionFloor == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Model.ProductionFloor).FirstOrDefault().StoreName;
            Model.RecordStatus = Entity.RecordStatus;

            return Model;
        }
        public List<PrdYearMonth> GetWetBlueProductionSchedule()
        {
            var query = @"select SCH.ScheduleID,SCH.ScheduleNo,SCH.ScheduleStatus,SCH.YearMonID,SCH.ProductionProcessID,CONVERT(VARCHAR(12),SCH.PrepareDate,106) PrepareDate  from dbo.PRD_YearMonthSchedule SCH
                        inner join dbo.PRD_YearMonth on SCH.YearMonID = dbo.PRD_YearMonth.YearMonID
                        where dbo.PRD_YearMonth.ScheduleFor='WBP' order by SCH.ScheduleID desc";
            var allData = _context.Database.SqlQuery<PrdYearMonth>(query).ToList();
            List<PrdYearMonth> searchList = allData.ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdYearMonth>();
        }

        public PrdYearMonth SetToBussinessObject(PrdYearMonth Entity)
        {
            PrdYearMonth Model = new PrdYearMonth();

            Model.ScheduleID = Entity.ScheduleID;
            Model.ScheduleNo = Entity.ScheduleNo;
            Model.YearMonID = Convert.ToInt64(Entity.YearMonID);
            Model.PrepareDate = Convert.ToDateTime(Entity.PrepareDate).ToString("dd/MM/yyyy");
            Model.ProductionProcessID = Entity.ProductionProcessID;
            Model.ProcessName = Entity.ProductionProcessID == null ? "" : _context.Sys_ProductionProces.Where(m => m.ProcessID == Entity.ProductionProcessID).FirstOrDefault().ProcessName;
            Model.ScheduleYear = Entity.YearMonID == null ? "" : _context.PRD_YearMonth.Where(m => m.YearMonID == Entity.YearMonID).FirstOrDefault().ScheduleYear;
            Model.ScheduleMonth = Entity.YearMonID == null ? "" : _context.PRD_YearMonth.Where(m => m.YearMonID == Entity.YearMonID).FirstOrDefault().ScheduleMonth;
            switch (Model.ScheduleMonth)
            {
                case "01":
                    Model.ScheduleMonthName = "January";
                    break;
                case "02":
                    Model.ScheduleMonthName = "February";
                    break;
                case "03":
                    Model.ScheduleMonthName = "March";
                    break;
                case "04":
                    Model.ScheduleMonthName = "April";
                    break;
                case "05":
                    Model.ScheduleMonthName = "May";
                    break;
                case "06":
                    Model.ScheduleMonthName = "June";
                    break;
                case "07":
                    Model.ScheduleMonthName = "July";
                    break;
                case "08":
                    Model.ScheduleMonthName = "August";
                    break;
                case "09":
                    Model.ScheduleMonthName = "September";
                    break;
                case "10":
                    Model.ScheduleMonthName = "October";
                    break;
                case "11":
                    Model.ScheduleMonthName = "November";
                    break;
                case "12":
                    Model.ScheduleMonthName = "December";
                    break;
                default:
                    Model.ScheduleMonthName = "";
                    break;
            }
            Model.ScheduleFor = Entity.YearMonID == null ? "" : _context.PRD_YearMonth.Where(m => m.YearMonID == Entity.YearMonID).FirstOrDefault().ScheduleFor;
            switch (Model.ScheduleFor)
            {
                case "WBP":
                    Model.ScheduleForName = "Wet Blue Production";
                    break;
                case "WBR":
                    Model.ScheduleForName = "Wet Bule Requisition";
                    break;
                case "CRP":
                    Model.ScheduleForName = "Crust Requisition";
                    break;
                case "CRR":
                    Model.ScheduleForName = "Crust Requisition";
                    break;
                case "FNP":
                    Model.ScheduleForName = "Finished Production";
                    break;
                default:
                    Model.ScheduleForName = "";
                    break;
            }
            Model.ScheduleStatus = Entity.ScheduleStatus;
            switch (Model.ScheduleStatus)
            {
                case "SCH":
                    Model.ScheduleStatusName = "Schedule";
                    break;
                case "CMP":
                    Model.ScheduleStatusName = "Completed";
                    break;
                //case "CRP":
                //    Model.ScheduleForName = "Crust Requisition";
                //    break;
                //case "CRR":
                //    Model.ScheduleForName = "Crust Requisition";
                //    break;
                //case "FNP":
                //    Model.ScheduleForName = "Finished Production";
                //    break;
                default:
                    Model.ScheduleForName = "";
                    break;
            }
            Model.ProductionFloor = Entity.YearMonID == null ? null : _context.PRD_YearMonth.Where(m => m.YearMonID == Entity.YearMonID).FirstOrDefault().ProductionFloor;
            Model.ConcernStore = Entity.YearMonID == null ? null : _context.PRD_YearMonth.Where(m => m.YearMonID == Entity.YearMonID).FirstOrDefault().ConcernStore;
            Model.ProductionFloorName = Model.ProductionFloor == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Model.ProductionFloor).FirstOrDefault().StoreName;
            Model.RecordStatus = Entity.RecordStatus;

            return Model;
        }

        public ValidationMsg ConfirmedWetBlueProductionSchedule(PrdYearMonth model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityYearMonth = _context.PRD_YearMonth.First(m => m.YearMonID == model.YearMonID);
                        originalEntityYearMonth.RecordStatus = "CNF";
                        originalEntityYearMonth.ModifiedBy = userid;
                        originalEntityYearMonth.ModifiedOn = DateTime.Now;

                        var originalEntityYearMonthSchedule = _context.PRD_YearMonthSchedule.First(m => m.ScheduleNo == model.ScheduleNo);
                        originalEntityYearMonthSchedule.RecordStatus = "CNF";
                        originalEntityYearMonthSchedule.ModifiedBy = userid;
                        originalEntityYearMonthSchedule.ModifiedOn = DateTime.Now;

                        if (model.PrdYearMonthScheduleDateList.Count > 0)
                        {
                            foreach (PrdYearMonthScheduleDate objPrdYearMonthScheduleDate in model.PrdYearMonthScheduleDateList)
                            {
                                var originalEntityYearMonthScheduleDate = _context.PRD_YearMonthScheduleDate.First(m => m.ScheduleDateID == objPrdYearMonthScheduleDate.ScheduleDateID);
                                originalEntityYearMonthScheduleDate.RecordStatus = "CNF";
                                originalEntityYearMonthScheduleDate.ModifiedBy = userid;
                                originalEntityYearMonthScheduleDate.ModifiedOn = DateTime.Now;
                            }
                            if (model.PrdYearMonthSchedulePurchaseList.Count > 0)
                            {
                                foreach (PrdYearMonthSchedulePurchase objPrdYearMonthSchedulePurchase in model.PrdYearMonthSchedulePurchaseList)
                                {
                                    var originalEntityYearMonthSchedulePurchase = _context.PRD_YearMonthSchedulePurchase.First(m => m.SchedulePurchaseID == objPrdYearMonthSchedulePurchase.SchedulePurchaseID);
                                    originalEntityYearMonthSchedulePurchase.RecordStatus = "CNF";
                                    originalEntityYearMonthSchedulePurchase.ModifiedBy = userid;
                                    originalEntityYearMonthSchedulePurchase.ModifiedOn = DateTime.Now;
                                }
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

        public ValidationMsg CheckedWetBlueProductionSchedule(PrdYearMonth model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityYearMonth = _context.PRD_YearMonth.First(m => m.YearMonID == model.YearMonID);
                        originalEntityYearMonth.RecordStatus = "CHK";
                        originalEntityYearMonth.ModifiedBy = userid;
                        originalEntityYearMonth.ModifiedOn = DateTime.Now;

                        var originalEntityYearMonthSchedule = _context.PRD_YearMonthSchedule.First(m => m.ScheduleNo == model.ScheduleNo);
                        originalEntityYearMonthSchedule.RecordStatus = "CHK";
                        originalEntityYearMonthSchedule.ModifiedBy = userid;
                        originalEntityYearMonthSchedule.ModifiedOn = DateTime.Now;

                        if (model.PrdYearMonthScheduleDateList.Count > 0)
                        {
                            foreach (PrdYearMonthScheduleDate objPrdYearMonthScheduleDate in model.PrdYearMonthScheduleDateList)
                            {
                                var originalEntityYearMonthScheduleDate = _context.PRD_YearMonthScheduleDate.First(m => m.ScheduleDateID == objPrdYearMonthScheduleDate.ScheduleDateID);
                                originalEntityYearMonthScheduleDate.RecordStatus = "CHK";
                                originalEntityYearMonthScheduleDate.ModifiedBy = userid;
                                originalEntityYearMonthScheduleDate.ModifiedOn = DateTime.Now;
                            }
                            if (model.PrdYearMonthSchedulePurchaseList.Count > 0)
                            {
                                foreach (PrdYearMonthSchedulePurchase objPrdYearMonthSchedulePurchase in model.PrdYearMonthSchedulePurchaseList)
                                {
                                    var originalEntityYearMonthSchedulePurchase = _context.PRD_YearMonthSchedulePurchase.First(m => m.SchedulePurchaseID == objPrdYearMonthSchedulePurchase.SchedulePurchaseID);
                                    originalEntityYearMonthSchedulePurchase.RecordStatus = "CHK";
                                    originalEntityYearMonthSchedulePurchase.ModifiedBy = userid;
                                    originalEntityYearMonthSchedulePurchase.ModifiedOn = DateTime.Now;
                                }
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

        public ValidationMsg ConcealWetBlueProductionSchedule(string ScheduleDateID, string ProductionStatus, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        if (!string.IsNullOrEmpty(ScheduleDateID))
                        {
                            long scheduleDateID = Convert.ToInt64(ScheduleDateID);
                            var originalEntityYearMonthScheduleDate = _context.PRD_YearMonthScheduleDate.First(m => m.ScheduleDateID == scheduleDateID);
                            originalEntityYearMonthScheduleDate.RecordStatus = "CONS";
                            originalEntityYearMonthScheduleDate.ModifiedBy = userid;
                            originalEntityYearMonthScheduleDate.ModifiedOn = DateTime.Now;
                            _context.SaveChanges();

                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Conceal Successfully.";
                        }
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

        public ValidationMsg ExecuteWetBlueProductionSchedule(PrdYearMonth model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        if (model.PrdYearMonthScheduleDateList[0].ScheduleDateID != 0)
                        {
                            long scheduleDateID = Convert.ToInt64(model.PrdYearMonthScheduleDateList[0].ScheduleDateID);
                            var productionStatus = _context.PRD_YearMonthScheduleDate.Where(m => m.ScheduleDateID == scheduleDateID).FirstOrDefault().ProductionStatus;
                            var processEffect = _context.Sys_ProductionProces.Where(m => m.ProcessID == model.ProductionProcessID).FirstOrDefault().ProcessEffect;
                            if ((productionStatus == "SCH") && (processEffect == "CL"))
                            {
                                #region Update Store Records

                                if (model.PrdYearMonthSchedulePurchaseList != null)
                                {
                                    foreach (PrdYearMonthSchedulePurchase objPrdYearMonthSchedulePurchase in model.PrdYearMonthSchedulePurchaseList)
                                    {
                                        if ((objPrdYearMonthSchedulePurchase.ProductionSide == null) || (objPrdYearMonthSchedulePurchase.ProductionSide == null))
                                            sidePcs = 0;
                                        else
                                            sidePcs = (decimal)(objPrdYearMonthSchedulePurchase.ProductionSide / 2);

                                        if ((objPrdYearMonthSchedulePurchase.ProductionPcs == null) || (objPrdYearMonthSchedulePurchase.ProductionPcs == null))
                                            productionPcs = 0;
                                        else
                                            productionPcs = (decimal)(objPrdYearMonthSchedulePurchase.ProductionPcs);

                                        productionPcs = (decimal)(productionPcs + sidePcs);

                                        if (objPrdYearMonthSchedulePurchase.ClosingQty >= productionPcs)
                                        {
                                            #region Supplier_Stock_Update

                                            var CheckSupplierStock = (from i in _context.Inv_StockSupplier
                                                                      where i.SupplierID == objPrdYearMonthSchedulePurchase.SupplierID
                                                                      && i.ItemTypeID == objPrdYearMonthSchedulePurchase.ItemTypeID
                                                                      && i.LeatherType == objPrdYearMonthSchedulePurchase.LeatherTypeID
                                                                      && i.LeatherStatusID == objPrdYearMonthSchedulePurchase.LeatherStatusID
                                                                      && i.StoreID == model.ConcernStore
                                                                      && i.PurchaseID == objPrdYearMonthSchedulePurchase.PurchaseID
                                                                      select i).Any();

                                            if (!CheckSupplierStock)
                                            {
                                                var LastSupplierStock = (from i in _context.Inv_StockSupplier
                                                                         where i.SupplierID == objPrdYearMonthSchedulePurchase.SupplierID
                                                                         && i.ItemTypeID == objPrdYearMonthSchedulePurchase.ItemTypeID
                                                                         && i.LeatherType == objPrdYearMonthSchedulePurchase.LeatherTypeID
                                                                         && i.LeatherStatusID == objPrdYearMonthSchedulePurchase.LeatherStatusID
                                                                         && i.StoreID == model.ConcernStore
                                                                         && i.PurchaseID == objPrdYearMonthSchedulePurchase.PurchaseID
                                                                         orderby i.TransectionID descending
                                                                         select i).FirstOrDefault();

                                                Inv_StockSupplier tblStockSupplier = new Inv_StockSupplier();

                                                tblStockSupplier.SupplierID = Convert.ToInt32(objPrdYearMonthSchedulePurchase.SupplierID);
                                                tblStockSupplier.StoreID = Convert.ToByte(model.ConcernStore);
                                                tblStockSupplier.ItemTypeID = Convert.ToByte(objPrdYearMonthSchedulePurchase.ItemTypeID);
                                                tblStockSupplier.UnitID = LastSupplierStock.UnitID;
                                                tblStockSupplier.PurchaseID = objPrdYearMonthSchedulePurchase.PurchaseID;

                                                tblStockSupplier.OpeningQty = LastSupplierStock.ClosingQty;
                                                tblStockSupplier.ReceiveQty = 0;
                                                tblStockSupplier.IssueQty = productionPcs;// objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs;
                                                tblStockSupplier.ClosingQty = LastSupplierStock.ClosingQty - productionPcs;// (objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs);
                                                tblStockSupplier.UpdateReason = "Wet Blue Production Schedule";
                                                _context.Inv_StockSupplier.Add(tblStockSupplier);
                                                _context.SaveChanges();
                                            }
                                            else
                                            {
                                                var LastSupplierStock = (from i in _context.Inv_StockSupplier
                                                                         where i.SupplierID == objPrdYearMonthSchedulePurchase.SupplierID
                                                                         && i.ItemTypeID == objPrdYearMonthSchedulePurchase.ItemTypeID
                                                                         && i.LeatherType == objPrdYearMonthSchedulePurchase.LeatherTypeID
                                                                         && i.LeatherStatusID == objPrdYearMonthSchedulePurchase.LeatherStatusID
                                                                         && i.StoreID == model.ConcernStore
                                                                         && i.PurchaseID == objPrdYearMonthSchedulePurchase.PurchaseID
                                                                         orderby i.TransectionID descending
                                                                         select i).FirstOrDefault();

                                                Inv_StockSupplier objStockSupplier = new Inv_StockSupplier();

                                                objStockSupplier.SupplierID = Convert.ToInt32(objPrdYearMonthSchedulePurchase.SupplierID);
                                                objStockSupplier.StoreID = Convert.ToByte(model.ConcernStore);
                                                //objStockSupplier.RefChallanID = objPrdYearMonthSchedulePurchase.ChallanID;
                                                objStockSupplier.ItemTypeID = Convert.ToByte(objPrdYearMonthSchedulePurchase.ItemTypeID);
                                                objStockSupplier.LeatherType = Convert.ToByte(objPrdYearMonthSchedulePurchase.LeatherTypeID);
                                                objStockSupplier.LeatherStatusID = Convert.ToByte(objPrdYearMonthSchedulePurchase.LeatherStatusID);
                                                objStockSupplier.UnitID = LastSupplierStock.UnitID;
                                                objStockSupplier.PurchaseID = objPrdYearMonthSchedulePurchase.PurchaseID;

                                                objStockSupplier.OpeningQty = LastSupplierStock.ClosingQty;
                                                objStockSupplier.ReceiveQty = 0;
                                                objStockSupplier.IssueQty = productionPcs;// objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs;
                                                objStockSupplier.ClosingQty = LastSupplierStock.ClosingQty - productionPcs;// (objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs);
                                                //objStockSupplier.ClosingQty = LastSupplierStock.ClosingQty - (objPrdYearMonthSchedulePurchase.ProductionPcs + (objPrdYearMonthSchedulePurchase.ProductionSide));
                                                objStockSupplier.UpdateReason = "Wet Blue Production Schedule";

                                                _context.Inv_StockSupplier.Add(objStockSupplier);
                                                _context.SaveChanges();
                                            }

                                            #endregion

                                            #region Item_Stock_Update

                                            var CheckItemStock = (from i in _context.Inv_StockItem
                                                                  where i.ItemTypeID == objPrdYearMonthSchedulePurchase.ItemTypeID
                                                                  && i.LeatherType == objPrdYearMonthSchedulePurchase.LeatherTypeID
                                                                  && i.LeatherStatus == objPrdYearMonthSchedulePurchase.LeatherStatusID
                                                                  && i.StoreID == model.ConcernStore
                                                                  select i).Any();

                                            if (!CheckItemStock)
                                            {
                                                var LastItemInfo = (from i in _context.Inv_StockItem
                                                                    where i.ItemTypeID == objPrdYearMonthSchedulePurchase.ItemTypeID
                                                                    && i.LeatherType == objPrdYearMonthSchedulePurchase.LeatherTypeID
                                                                    && i.LeatherStatus == objPrdYearMonthSchedulePurchase.LeatherStatusID
                                                                    && i.StoreID == model.ConcernStore
                                                                    orderby i.TransectionID descending
                                                                    select i).FirstOrDefault();

                                                Inv_StockItem objStockItem = new Inv_StockItem();

                                                objStockItem.ItemTypeID = Convert.ToByte(objPrdYearMonthSchedulePurchase.ItemTypeID);
                                                objStockItem.LeatherType = Convert.ToByte(objPrdYearMonthSchedulePurchase.LeatherTypeID);
                                                objStockItem.LeatherStatus = Convert.ToByte(objPrdYearMonthSchedulePurchase.LeatherStatusID);
                                                objStockItem.StoreID = Convert.ToByte(model.ConcernStore);
                                                objStockItem.UnitID = LastItemInfo.UnitID;

                                                objStockItem.OpeningQty = LastItemInfo.ClosingQty;
                                                objStockItem.IssueQty = productionPcs;// objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs;
                                                objStockItem.ReceiveQty = 0;
                                                objStockItem.ClosingQty = LastItemInfo.ClosingQty - productionPcs;// (objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs);

                                                _context.Inv_StockItem.Add(objStockItem);
                                                _context.SaveChanges();
                                            }
                                            else
                                            {
                                                var LastItemInfo = (from i in _context.Inv_StockItem
                                                                    where i.ItemTypeID == objPrdYearMonthSchedulePurchase.ItemTypeID
                                                                    && i.LeatherType == objPrdYearMonthSchedulePurchase.LeatherTypeID
                                                                    && i.LeatherStatus == objPrdYearMonthSchedulePurchase.LeatherStatusID
                                                                    && i.StoreID == model.ConcernStore
                                                                    orderby i.TransectionID descending
                                                                    select i).FirstOrDefault();


                                                Inv_StockItem objStockItem = new Inv_StockItem();

                                                objStockItem.ItemTypeID = LastItemInfo.ItemTypeID;
                                                objStockItem.LeatherType = LastItemInfo.LeatherType;
                                                objStockItem.LeatherStatus = LastItemInfo.LeatherStatus;
                                                objStockItem.StoreID = LastItemInfo.StoreID;
                                                objStockItem.UnitID = LastItemInfo.UnitID;

                                                objStockItem.OpeningQty = LastItemInfo.ClosingQty;
                                                objStockItem.ReceiveQty = 0;
                                                objStockItem.IssueQty = productionPcs;// objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs;
                                                objStockItem.ClosingQty = LastItemInfo.ClosingQty - productionPcs;// (objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs);

                                                _context.Inv_StockItem.Add(objStockItem);
                                                _context.SaveChanges();
                                            }

                                            #endregion

                                            #region Daily_Stock_Update

                                            var currentDate = System.DateTime.Now.Date;

                                            var CheckDate = (from ds in _context.Inv_StockDaily
                                                             where ds.ItemTypeID == objPrdYearMonthSchedulePurchase.ItemTypeID
                                                             && ds.StoreID == model.ConcernStore
                                                             && ds.LeatherStatus == objPrdYearMonthSchedulePurchase.LeatherStatusID
                                                             && ds.LeatherType == objPrdYearMonthSchedulePurchase.LeatherTypeID
                                                             && ds.StockDate == currentDate
                                                             select ds).Any();

                                            if (CheckDate)
                                            {
                                                var CurrentItem = (from ds in _context.Inv_StockDaily
                                                                   where ds.ItemTypeID == objPrdYearMonthSchedulePurchase.ItemTypeID
                                                                   && ds.StoreID == model.ConcernStore
                                                                   && ds.LeatherStatus == objPrdYearMonthSchedulePurchase.LeatherStatusID
                                                                   && ds.LeatherType == objPrdYearMonthSchedulePurchase.LeatherTypeID
                                                                   && ds.StockDate == currentDate
                                                                   select ds).FirstOrDefault();

                                                CurrentItem.DailyIssueQty = CurrentItem.DailyIssueQty + productionPcs;// (objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs);
                                                CurrentItem.ClosingQty = CurrentItem.ClosingQty - productionPcs;// (objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs);
                                                _context.SaveChanges();
                                            }
                                            else
                                            {
                                                var PreviousRecord = (from ds in _context.Inv_StockDaily
                                                                      where ds.ItemTypeID == objPrdYearMonthSchedulePurchase.ItemTypeID
                                                                      && ds.StoreID == model.ConcernStore
                                                                      && ds.LeatherStatus == objPrdYearMonthSchedulePurchase.LeatherStatusID
                                                                      && ds.LeatherType == objPrdYearMonthSchedulePurchase.LeatherTypeID
                                                                      orderby ds.TransectionID descending
                                                                      select ds).FirstOrDefault();

                                                Inv_StockDaily objStockDaily = new Inv_StockDaily();

                                                objStockDaily.StockDate = currentDate;

                                                objStockDaily.ItemTypeID = Convert.ToByte(objPrdYearMonthSchedulePurchase.ItemTypeID);
                                                objStockDaily.StoreID = Convert.ToByte(model.ConcernStore);
                                                objStockDaily.UnitID = PreviousRecord.UnitID;
                                                objStockDaily.LeatherStatus = Convert.ToByte(objPrdYearMonthSchedulePurchase.LeatherStatusID);
                                                objStockDaily.LeatherType = Convert.ToByte(objPrdYearMonthSchedulePurchase.LeatherTypeID);

                                                objStockDaily.OpeningQty = (PreviousRecord == null ? 0 : PreviousRecord.ClosingQty);
                                                objStockDaily.DailyReceiveQty = 0;
                                                objStockDaily.DailyIssueQty = productionPcs;// objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs;
                                                objStockDaily.ClosingQty = objStockDaily.OpeningQty - productionPcs;// (objPrdYearMonthSchedulePurchase.ProductionPcs + sidePcs);

                                                _context.Inv_StockDaily.Add(objStockDaily);
                                                _context.SaveChanges();
                                            }

                                            #endregion
                                        }
                                        else
                                        {
                                            stockError = 1;
                                        }
                                    }
                                }

                                #endregion
                            }
                            switch (model.PrdYearMonthScheduleDateList[0].ProductionStatus)
                            {
                                case "Schedule":
                                case "On Going":
                                    {
                                        if (productionStatus != "CMP")
                                        {
                                            var OrgrEntity = _context.PRD_YearMonthScheduleDate.First(m => m.ScheduleDateID == scheduleDateID);

                                            OrgrEntity.ProductionStatus = "ONG";
                                            OrgrEntity.ModifiedBy = userid;
                                            OrgrEntity.ModifiedOn = DateTime.Now;
                                            _context.SaveChanges();
                                        }
                                    }
                                    break;
                                case "Postponed":
                                    {
                                        if ((productionStatus == "SCH") || (productionStatus == "ONG"))
                                        {
                                            var OrgrEntity = _context.PRD_YearMonthScheduleDate.First(m => m.ScheduleDateID == scheduleDateID);

                                            OrgrEntity.ProductionStatus = "POS ";
                                            OrgrEntity.ModifiedBy = userid;
                                            OrgrEntity.ModifiedOn = DateTime.Now;
                                            _context.SaveChanges();
                                        }
                                    }
                                    break;
                                case "Completed":
                                    {
                                        if ((productionStatus == "POS") || (productionStatus == "ONG"))
                                        {
                                            var OrgrEntity = _context.PRD_YearMonthScheduleDate.First(m => m.ScheduleDateID == scheduleDateID);

                                            OrgrEntity.ProductionStatus = "CMP";
                                            OrgrEntity.RecordStatus = "CNF";
                                            OrgrEntity.ModifiedBy = userid;
                                            OrgrEntity.ModifiedOn = DateTime.Now;
                                            if (processEffect == "CL")
                                            {
                                                if (model.PrdYearMonthSchedulePurchaseList != null)
                                                {
                                                    foreach (PrdYearMonthSchedulePurchase objPrdYearMonthSchedulePurchase in model.PrdYearMonthSchedulePurchaseList)
                                                    {
                                                        //if (objPrdYearMonthSchedulePurchase.ClosingQty >= objPrdYearMonthSchedulePurchase.ProductionPcs)
                                                        //{
                                                        #region Receive Data To WetBlue Production Floor

                                                        var CheckItemStock = (from i in _context.PRD_WetBlueProductionStock
                                                                              where i.ItemTypeID == objPrdYearMonthSchedulePurchase.ItemTypeID
                                                                                  && i.LeatherTypeID == objPrdYearMonthSchedulePurchase.LeatherTypeID
                                                                                  && i.LeatherStatusID == objPrdYearMonthSchedulePurchase.LeatherStatusID
                                                                                  && i.StoreID == model.ProductionFloor
                                                                                  && i.SupplierID == objPrdYearMonthSchedulePurchase.SupplierID
                                                                                  && i.PurchaseID == objPrdYearMonthSchedulePurchase.PurchaseID
                                                                              select i).Any();
                                                        if (!CheckItemStock)
                                                        {
                                                            PRD_WetBlueProductionStock objStockItem = new PRD_WetBlueProductionStock();

                                                            objStockItem.SupplierID = Convert.ToInt32(objPrdYearMonthSchedulePurchase.SupplierID);
                                                            objStockItem.StoreID = Convert.ToByte(model.ProductionFloor);
                                                            objStockItem.ItemTypeID = Convert.ToByte(objPrdYearMonthSchedulePurchase.ItemTypeID);
                                                            objStockItem.LeatherTypeID = Convert.ToByte(objPrdYearMonthSchedulePurchase.LeatherTypeID);
                                                            objStockItem.LeatherStatusID = Convert.ToByte(objPrdYearMonthSchedulePurchase.LeatherStatusID);
                                                            objStockItem.PurchaseID = objPrdYearMonthSchedulePurchase.PurchaseID;

                                                            objStockItem.OpeningPcs = 0;
                                                            objStockItem.ReceivePcs = objPrdYearMonthSchedulePurchase.ProductionPcs == null ? 0 : objPrdYearMonthSchedulePurchase.ProductionPcs;
                                                            objStockItem.IssuePcs = 0;
                                                            objStockItem.ClosingProductionkPcs = objPrdYearMonthSchedulePurchase.ProductionPcs == null ? 0 : objPrdYearMonthSchedulePurchase.ProductionPcs;

                                                            objStockItem.OpeningSide = 0;
                                                            objStockItem.ReceiveSide = objPrdYearMonthSchedulePurchase.ProductionSide == null ? 0 : objPrdYearMonthSchedulePurchase.ProductionSide;
                                                            objStockItem.IssueSide = 0;
                                                            objStockItem.ClosingProductionSide = objPrdYearMonthSchedulePurchase.ProductionSide == null ? 0 : objPrdYearMonthSchedulePurchase.ProductionSide;

                                                            objStockItem.OpeningArea = 0;
                                                            objStockItem.ReceiveArea = 0;
                                                            objStockItem.IssueArea = 0;
                                                            objStockItem.ClosingProductionArea = 0;

                                                            _context.PRD_WetBlueProductionStock.Add(objStockItem);
                                                            _context.SaveChanges();
                                                        }
                                                        else
                                                        {
                                                            var LastItemInfo = (from i in _context.PRD_WetBlueProductionStock
                                                                                where i.ItemTypeID == objPrdYearMonthSchedulePurchase.ItemTypeID
                                                                                    && i.LeatherTypeID == objPrdYearMonthSchedulePurchase.LeatherTypeID
                                                                                    && i.LeatherStatusID == objPrdYearMonthSchedulePurchase.LeatherStatusID
                                                                                    && i.StoreID == model.ProductionFloor
                                                                                    && i.SupplierID == objPrdYearMonthSchedulePurchase.SupplierID
                                                                                    && i.PurchaseID == objPrdYearMonthSchedulePurchase.PurchaseID
                                                                                orderby i.TransectionID descending
                                                                                select i).FirstOrDefault();

                                                            PRD_WetBlueProductionStock objStockItem = new PRD_WetBlueProductionStock();

                                                            objStockItem.SupplierID = Convert.ToInt32(objPrdYearMonthSchedulePurchase.SupplierID);
                                                            objStockItem.StoreID = Convert.ToByte(model.ProductionFloor);
                                                            objStockItem.ItemTypeID = Convert.ToByte(objPrdYearMonthSchedulePurchase.ItemTypeID);
                                                            objStockItem.LeatherTypeID = Convert.ToByte(objPrdYearMonthSchedulePurchase.LeatherTypeID);
                                                            objStockItem.LeatherStatusID = Convert.ToByte(objPrdYearMonthSchedulePurchase.LeatherStatusID);
                                                            objStockItem.PurchaseID = objPrdYearMonthSchedulePurchase.PurchaseID;

                                                            objStockItem.OpeningPcs = LastItemInfo.ClosingProductionkPcs;
                                                            objStockItem.ReceivePcs = objPrdYearMonthSchedulePurchase.ProductionPcs == null ? 0 : objPrdYearMonthSchedulePurchase.ProductionPcs;
                                                            objStockItem.IssuePcs = 0;
                                                            objStockItem.ClosingProductionkPcs = (LastItemInfo.ClosingProductionkPcs == null ? 0 : LastItemInfo.ClosingProductionkPcs) + (objPrdYearMonthSchedulePurchase.ProductionPcs == null ? 0 : objPrdYearMonthSchedulePurchase.ProductionPcs);

                                                            objStockItem.OpeningSide = LastItemInfo.ClosingProductionSide;
                                                            objStockItem.ReceiveSide = objPrdYearMonthSchedulePurchase.ProductionSide == null ? 0 : objPrdYearMonthSchedulePurchase.ProductionSide;
                                                            objStockItem.IssueSide = 0;
                                                            objStockItem.ClosingProductionSide = (LastItemInfo.ClosingProductionSide == null ? 0 : LastItemInfo.ClosingProductionSide) + (objPrdYearMonthSchedulePurchase.ProductionSide == null ? 0 : objPrdYearMonthSchedulePurchase.ProductionSide);

                                                            objStockItem.OpeningArea = 0;
                                                            objStockItem.ReceiveArea = 0;
                                                            objStockItem.IssueArea = 0;
                                                            objStockItem.ClosingProductionArea = 0;

                                                            _context.PRD_WetBlueProductionStock.Add(objStockItem);
                                                            _context.SaveChanges();
                                                        }
                                                        #endregion
                                                        //}
                                                        //else
                                                        //{
                                                        //    stockError = 1;
                                                        //}
                                                    }
                                                }
                                            }
                                            //_context.SaveChanges();
                                        }
                                    }
                                    break;
                            }
                        }
                        if (stockError == 0)
                        {
                            _context.SaveChanges();
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Execute Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Not Enoung Quantity in Stock.";
                        }
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Execute.";
            }
            return _vmMsg;
        }

        public List<PrdYearMonthSchedulePurchase> GetLeatherInfoList(string ConcernStore, string SupplierID)
        {
            if (!string.IsNullOrEmpty(ConcernStore) && !string.IsNullOrEmpty(SupplierID))
            {
                var query = "select inv.TransectionID," +
                    //" inv.SupplierID,(select SupplierName from dbo.Sys_Supplier where SupplierID = inv.SupplierID)SupplierName," +
                            " inv.PurchaseID,(select PurchaseNo from dbo.Prq_Purchase where PurchaseID = inv.PurchaseID)PurchaseNo," +
                            " CONVERT(VARCHAR(10), (select PurchaseDate from dbo.Prq_Purchase where PurchaseID = inv.PurchaseID), 103) PurchaseDate," +
                            " (select distinct top 1 (select SourceName from Sys_Source where SourceID = Prq_PurchaseChallan.SourceID)  from Prq_PurchaseChallan where Prq_PurchaseChallan.PurchaseID =inv.PurchaseID)SourceName," +
                            " inv.StoreID,(select StoreName from dbo.SYS_Store where StoreID = inv.StoreID)StoreName," +
                            " inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName," +
                            " inv.LeatherType LeatherTypeID,(select LeatherTypeName from dbo.Sys_LeatherType where LeatherTypeID = inv.LeatherType)LeatherTypeName," +
                            " inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName," +
                            " inv.ClosingQty from dbo.Inv_StockSupplier inv " +
                            " INNER JOIN (select MAX(TransectionID)TransectionID,SupplierID,StoreID,ItemTypeID,LeatherType,LeatherStatusID,PurchaseID from dbo.Inv_StockSupplier" +
                            " group by SupplierID,StoreID,ItemTypeID,LeatherType,LeatherStatusID,PurchaseID) sup" +
                            " ON inv.TransectionID=sup.TransectionID" +
                            " where inv.StoreID = " + ConcernStore + " and inv.SupplierID = " + SupplierID +
                            " and inv.ClosingQty>0";
                var allData = _context.Database.SqlQuery<PrdYearMonthSchedulePurchase>(query).ToList();
                return allData;
            }
            return null;
        }

        public List<SysSupplier> GetSupplierFromStoreList(string ConcernStore)
        {
            if (string.IsNullOrEmpty(ConcernStore)) return null;
            var query = "select DISTINCT inv.SupplierID,(select SupplierName from dbo.Sys_Supplier where SupplierID = inv.SupplierID)SupplierName," +
                        " (select SupplierCode from dbo.Sys_Supplier where SupplierID = inv.SupplierID)SupplierCode from dbo.Inv_StockSupplier inv " +
                        " INNER JOIN (select MAX(TransectionID)TransectionID,SupplierID,StoreID,ItemTypeID,LeatherType,LeatherStatusID,PurchaseID from dbo.Inv_StockSupplier" +
                        " group by SupplierID,StoreID,ItemTypeID,LeatherType,LeatherStatusID,PurchaseID) sup" +
                        " ON inv.TransectionID=sup.TransectionID" +
                        " where inv.StoreID = " + ConcernStore + " and inv.ClosingQty>0";

            var allData = _context.Database.SqlQuery<SysSupplier>(query).ToList();
            return allData;
        }

        public List<string> GetSupplierListForSearch()
        {
            var query = "select (select SupplierName from dbo.Sys_Supplier where SupplierID = inv.SupplierID)SupplierName from dbo.Inv_StockSupplier inv " +
                        " INNER JOIN (select MAX(TransectionID)TransectionID,SupplierID,StoreID,ItemTypeID,LeatherType,LeatherStatusID,PurchaseID from dbo.Inv_StockSupplier" +
                        " group by SupplierID,StoreID,ItemTypeID,LeatherType,LeatherStatusID,PurchaseID) sup" +
                        " ON inv.TransectionID=sup.TransectionID" +
                        " where inv.ClosingQty>0";
            //" where inv.StoreID = " + ConcernStore + " and inv.ClosingQty>0";

            var allData = _context.Database.SqlQuery<string>(query).ToList();
            return allData;
        }

        public List<SysSupplier> GetSupplierList(string supplier, string ConcernStore)
        {
            var query = "select inv.SupplierID,(select SupplierName from dbo.Sys_Supplier where SupplierID = inv.SupplierID)SupplierName," +
                        " (select SupplierCode from dbo.Sys_Supplier where SupplierID = inv.SupplierID)SupplierCode from dbo.Inv_StockSupplier inv " +
                        " INNER JOIN (select MAX(TransectionID)TransectionID,SupplierID,StoreID,ItemTypeID,LeatherType,LeatherStatusID,PurchaseID from dbo.Inv_StockSupplier" +
                        " group by SupplierID,StoreID,ItemTypeID,LeatherType,LeatherStatusID,PurchaseID) sup" +
                        " ON inv.TransectionID=sup.TransectionID" +
                        " where inv.StoreID = " + ConcernStore + " and (select SupplierName from dbo.Sys_Supplier where SupplierID = inv.SupplierID) = '" + supplier + "'  and inv.ClosingQty>0";

            var allData = _context.Database.SqlQuery<SysSupplier>(query).ToList();
            return allData;
        }

        public List<PrdYearMonthScheduleDate> GetProductionNoList(string ProductionFloor, string ScheduleID)
        {
            var query = @"select dbo.PRD_YearMonthSchedule.ScheduleNo,dbo.PRD_YearMonthScheduleDate.ProductionNo,
                        CONVERT(VARCHAR(12),dbo.PRD_YearMonthScheduleDate.ScheduleStartDate,106)ScheduleStartDate,
                        CONVERT(VARCHAR(12),dbo.PRD_YearMonthScheduleDate.ScheduleEndDate,106)ScheduleEndDate,
                        dbo.PRD_YearMonthScheduleDate.SchedulePcs,dbo.PRD_YearMonthScheduleDate.ScheduleSide,
                        dbo.PRD_YearMonthScheduleDate.ScheduleWeight,dbo.PRD_YearMonthScheduleDate.ScheduleDateID,
                        (select UnitName from dbo.Sys_Unit where UnitID = dbo.PRD_YearMonthScheduleDate.ScheduleWeightUnit)ScheduleWeightUnitName,
                        (select ProcessName from dbo.Sys_ProductionProces where dbo.Sys_ProductionProces.ProcessID = dbo.PRD_YearMonthSchedule.ProductionProcessID)ProcessName
                        from dbo.PRD_YearMonthSchedule
						inner join dbo.PRD_YearMonthScheduleDate on dbo.PRD_YearMonthSchedule.ScheduleID = dbo.PRD_YearMonthScheduleDate.ScheduleID
						inner join dbo.PRD_YearMonth on dbo.PRD_YearMonthSchedule.YearMonID = dbo.PRD_YearMonth.YearMonID
                        where dbo.PRD_YearMonthSchedule.ScheduleStatus = 'SCH'
						AND dbo.PRD_YearMonthScheduleDate.ProductionStatus = 'CMP'
						AND dbo.PRD_YearMonthSchedule.ProductionProcessID = 1
                        AND dbo.PRD_YearMonth.ProductionFloor = " + ProductionFloor + "";

//            var query = @"select dbo.PRD_YearMonthSchedule.ScheduleNo,dbo.PRD_YearMonthScheduleDate.ProductionNo,
//                        CONVERT(VARCHAR(12),dbo.PRD_YearMonthScheduleDate.ScheduleStartDate,106)ScheduleStartDate,
//                        CONVERT(VARCHAR(12),dbo.PRD_YearMonthScheduleDate.ScheduleEndDate,106)ScheduleEndDate,
//                        dbo.PRD_YearMonthScheduleDate.SchedulePcs,dbo.PRD_YearMonthScheduleDate.ScheduleSide,
//                        dbo.PRD_YearMonthScheduleDate.ScheduleWeight,dbo.PRD_YearMonthScheduleDate.ScheduleDateID,
//                        (select UnitName from dbo.Sys_Unit where UnitID = dbo.PRD_YearMonthScheduleDate.ScheduleWeightUnit)ScheduleWeightUnitName,
//                        (select ProcessName from dbo.Sys_ProductionProces where dbo.Sys_ProductionProces.ProcessID = dbo.PRD_YearMonthSchedule.ProductionProcessID)ProcessName
//                        from dbo.PRD_YearMonthSchedule
//						inner join dbo.PRD_YearMonthScheduleDate on dbo.PRD_YearMonthSchedule.ScheduleID = dbo.PRD_YearMonthScheduleDate.ScheduleID
//						inner join dbo.PRD_YearMonth on dbo.PRD_YearMonthSchedule.YearMonID = dbo.PRD_YearMonth.YearMonID
//                        where dbo.PRD_YearMonthSchedule.ScheduleStatus = 'SCH'
//						AND dbo.PRD_YearMonthScheduleDate.ProductionStatus = 'CMP'
//						AND dbo.PRD_YearMonthSchedule.ProductionProcessID = 1
//                        AND dbo.PRD_YearMonthSchedule.ScheduleID = 30076 AND dbo.PRD_YearMonth.ProductionFloor = " + ProductionFloor + "";
            var allData = _context.Database.SqlQuery<PrdYearMonthScheduleDate>(query).ToList();
            return allData;

            //List<PrdYearMonthScheduleDate> searchList = allData.ToList();
            //return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdYearMonthScheduleDate>();
        }

        //public PrdYearMonthScheduleDate SetToBussinessObject(PrdYearMonthScheduleDate Entity)
        //{
        //    PrdYearMonthScheduleDate Model = new PrdYearMonthScheduleDate();

        //    Model.ScheduleNo = Entity.ScheduleNo;
        //    Model.ProductionNo = Entity.ProductionNo;
        //    Model.ScheduleStartDate = Entity.ScheduleStartDate;
        //    Model.ScheduleEndDate = Entity.ScheduleEndDate;

        //    Model.SchedulePcs = Entity.SchedulePcs;
        //    Model.ScheduleSide = Entity.ScheduleSide;

        //    Model.ScheduleWeight = Entity.ScheduleWeight;
        //    Model.ScheduleWeightUnitName = Entity.ScheduleWeightUnitName;
        //    Model.ProcessName = Entity.ProcessName;

        //    return Model;
        //}

        public ValidationMsg WetBlueProductionScheduleClosed(string ScheduleNo, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityYearMonthSchedule = _context.PRD_YearMonthSchedule.First(m => m.ScheduleNo == ScheduleNo);
                        originalEntityYearMonthSchedule.ScheduleStatus = "CMP";
                        originalEntityYearMonthSchedule.ModifiedBy = userid;
                        originalEntityYearMonthSchedule.ModifiedOn = DateTime.Now;

                        _context.SaveChanges();
                        tx.Complete();
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Schedule Closed Successfully.";
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
    }
}
