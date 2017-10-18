using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.BaseModel;
using System.Transactions;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqPurchaseYearTarget
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long YearID = 0;
        public int PeriodID = 0;
        private int error = 0;
        public DalPrqPurchaseYearTarget()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(PrqPurchaseYearTarget model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                IList<string> periodlist = new List<string>();
                periodlist.Add(model.YearStartDate);
                foreach (var period in model.PeriodList)
                {
                    periodlist.Add(period.StartDate.Contains("/") ? period.StartDate : Convert.ToDateTime(period.StartDate).ToString("dd/MM/yyyy"));
                    periodlist.Add(period.EndDate.Contains("/") ? period.EndDate : Convert.ToDateTime(period.EndDate).ToString("dd/MM/yyyy"));
                }

                for (int i = 1; i <= periodlist.Count - 1; i++)
                {
                    double day = (DalCommon.SetDate(periodlist[i]) - DalCommon.SetDate(periodlist[i - 1])).TotalDays;
                    if (i != 1)
                    {
                        if (day <= 0)
                        {
                            error = 1;
                            break;
                        }
                    }
                }
                if (error == 0)
                {
                    #region Save

                    using (var tx = new TransactionScope())
                    {
                        using (_context)
                        {
                            var exitData = _context.Prq_PurchaseYearTarget.Where(m => m.PurchaseYear == model.PurchaseYear).ToList();
                            if (exitData.Count > 0)
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "Alreary Exit This Year.";
                            }
                            else
                            {
                                model.SetBy = userid;
                                Prq_PurchaseYearTarget tblPurchaseYearTarget = SetToModelObject(model);
                                _context.Prq_PurchaseYearTarget.Add(tblPurchaseYearTarget);
                                _context.SaveChanges();

                                #region Save Period Records

                                if (model.PurchaseYearPeriodList != null)
                                {
                                    foreach (PrqPurchaseYearPeriod objPrqPurchaseYearPeriod in model.PurchaseYearPeriodList)
                                    {
                                        ////var yearStartDate = Convert.ToDateTime(objPrqPurchaseYearPeriod.StartDate).Date;
                                        ////var yearEndDate = Convert.ToDateTime(objPrqPurchaseYearPeriod.EndDate).Date;
                                        //objPrqPurchaseYearPeriod.SetBy = userid;
                                        //objPrqPurchaseYearPeriod.StartDate =
                                        //    Convert.ToDateTime(objPrqPurchaseYearPeriod.StartDate)
                                        //        .Date.ToString("dd/MM/yyyy");
                                        //objPrqPurchaseYearPeriod.EndDate =
                                        //    Convert.ToDateTime(objPrqPurchaseYearPeriod.EndDate)
                                        //        .Date.ToString("dd/MM/yyyy");

                                        objPrqPurchaseYearPeriod.StartDate = objPrqPurchaseYearPeriod.StartDate.Contains("/") ? objPrqPurchaseYearPeriod.StartDate : Convert.ToDateTime(objPrqPurchaseYearPeriod.StartDate).ToString("dd/MM/yyyy");
                                        objPrqPurchaseYearPeriod.EndDate = objPrqPurchaseYearPeriod.EndDate.Contains("/") ? objPrqPurchaseYearPeriod.EndDate : Convert.ToDateTime(objPrqPurchaseYearPeriod.EndDate).ToString("dd/MM/yyyy");
                                        objPrqPurchaseYearPeriod.YearID = tblPurchaseYearTarget.YearID;
                                        Prq_PurchaseYearPeriod tblPurchaseYearPeriod =
                                            SetToModelObject(objPrqPurchaseYearPeriod);
                                        _context.Prq_PurchaseYearPeriod.Add(tblPurchaseYearPeriod);

                                        _context.SaveChanges();

                                        #region Save Period Item List

                                        if (model.PurchaseYearPeriodItemList != null)
                                        {
                                            foreach (
                                                PrqPurchaseYearPeriodItem objPrqPurchaseYearPeriodItem in
                                                    model.PurchaseYearPeriodItemList)
                                            {
                                                objPrqPurchaseYearPeriodItem.PeriodID =
                                                    tblPurchaseYearPeriod.PeriodID;
                                                objPrqPurchaseYearPeriodItem.SetBy = userid;
                                                Prq_PurchaseYearPeriodItem tblPurchaseYearPeriodItem =
                                                    SetToModelObject(objPrqPurchaseYearPeriodItem);
                                                _context.Prq_PurchaseYearPeriodItem.Add(tblPurchaseYearPeriodItem);
                                            }
                                        }

                                        #endregion
                                    }
                                }
                                _context.SaveChanges();

                                #endregion

                                tx.Complete();
                                YearID = tblPurchaseYearTarget.YearID;
                                _vmMsg.Type = Enums.MessageType.Success;
                                _vmMsg.Msg = "Saved Successfully.";
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Period is not Properly Define.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }

            return _vmMsg;
        }

        public ValidationMsg Update(PrqPurchaseYearTarget model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                IList<string> periodlist = new List<string>();
                periodlist.Add(model.YearStartDate);
                foreach (var period in model.PeriodList)
                {
                    periodlist.Add(period.StartDate.Contains("/") ? period.StartDate : Convert.ToDateTime(period.StartDate).ToString("dd/MM/yyyy"));
                    periodlist.Add(period.EndDate.Contains("/") ? period.EndDate : Convert.ToDateTime(period.EndDate).ToString("dd/MM/yyyy"));
                }

                for (int i = 1; i <= periodlist.Count - 1; i++)
                {
                    double day = (DalCommon.SetDate(periodlist[i]) - DalCommon.SetDate(periodlist[i - 1])).TotalDays;
                    if (day < 0)
                    {
                        error = 1;
                        break;
                    }
                }
                if (error == 0)
                {
                    #region Update

                    using (var tx = new TransactionScope())
                    {
                        using (_context)
                        {
                            Prq_PurchaseYearTarget CurrentEntity = SetToModelObject(model);
                            var OriginalEntity = _context.Prq_PurchaseYearTarget.First(m => m.YearID == model.YearID);

                            OriginalEntity.PurchaseYear = CurrentEntity.PurchaseYear;
                            OriginalEntity.YearStartDate = CurrentEntity.YearStartDate;
                            OriginalEntity.YearEndDate = CurrentEntity.YearEndDate;
                            OriginalEntity.ModifiedBy = userid;
                            OriginalEntity.ModifiedOn = DateTime.Now;

                            #region Save Period Records

                            if (model.PurchaseYearPeriodList != null)
                            {
                                foreach (PrqPurchaseYearPeriod objPrqPurchaseYearPeriod in model.PurchaseYearPeriodList)
                                {
                                    objPrqPurchaseYearPeriod.StartDate = objPrqPurchaseYearPeriod.StartDate.Contains("/") ? objPrqPurchaseYearPeriod.StartDate : Convert.ToDateTime(objPrqPurchaseYearPeriod.StartDate).ToString("dd/MM/yyyy");
                                    objPrqPurchaseYearPeriod.EndDate = objPrqPurchaseYearPeriod.EndDate.Contains("/") ? objPrqPurchaseYearPeriod.EndDate : Convert.ToDateTime(objPrqPurchaseYearPeriod.EndDate).ToString("dd/MM/yyyy");

                                    //objPrqPurchaseYearPeriod.StartDate =
                                    //    Convert.ToDateTime(objPrqPurchaseYearPeriod.StartDate)
                                    //        .Date.ToString("dd/MM/yyyy");
                                    //objPrqPurchaseYearPeriod.EndDate =
                                    //    Convert.ToDateTime(objPrqPurchaseYearPeriod.EndDate)
                                    //        .Date.ToString("dd/MM/yyyy");

                                    if (objPrqPurchaseYearPeriod.PeriodID == 0)
                                    {
                                        objPrqPurchaseYearPeriod.YearID = model.YearID;
                                        objPrqPurchaseYearPeriod.SetBy = userid;
                                        Prq_PurchaseYearPeriod tblPurchaseYearPeriod = SetToModelObject(objPrqPurchaseYearPeriod);
                                        _context.Prq_PurchaseYearPeriod.Add(tblPurchaseYearPeriod);
                                        PeriodID = tblPurchaseYearPeriod.PeriodID;
                                    }
                                    else
                                    {
                                        Prq_PurchaseYearPeriod CurEntity = SetToModelObject(objPrqPurchaseYearPeriod);
                                        var OrgEntity = _context.Prq_PurchaseYearPeriod.First(m => m.PeriodID == objPrqPurchaseYearPeriod.PeriodID);

                                        OrgEntity.StartDate = CurEntity.StartDate;
                                        OrgEntity.EndDate = CurEntity.EndDate;
                                        OrgEntity.ModifiedBy = userid;
                                        OrgEntity.ModifiedOn = DateTime.Now;
                                    }

                                    #region Save Period Item List

                                    if (model.PurchaseYearPeriodItemList != null)
                                    {
                                        foreach (
                                            PrqPurchaseYearPeriodItem objPrqPurchaseYearPeriodItem in
                                                model.PurchaseYearPeriodItemList)
                                        {
                                            if (objPrqPurchaseYearPeriodItem.PeriodItemID == 0)
                                            {
                                                objPrqPurchaseYearPeriodItem.PeriodID = PeriodID;
                                                objPrqPurchaseYearPeriodItem.SetBy = userid;
                                                Prq_PurchaseYearPeriodItem tblPurchaseYearPeriodItem =
                                                    SetToModelObject(objPrqPurchaseYearPeriodItem);
                                                _context.Prq_PurchaseYearPeriodItem.Add(tblPurchaseYearPeriodItem);
                                            }
                                            else
                                            {
                                                Prq_PurchaseYearPeriodItem CurrEntity =
                                                    SetToModelObject(objPrqPurchaseYearPeriodItem);
                                                var OrgrEntity =
                                                    _context.Prq_PurchaseYearPeriodItem.First(
                                                        m =>
                                                            m.PeriodItemID ==
                                                            objPrqPurchaseYearPeriodItem.PeriodItemID);

                                                OrgrEntity.ItemTypeID = CurrEntity.ItemTypeID;
                                                OrgrEntity.LeatherType = CurrEntity.LeatherType;
                                                OrgrEntity.LeatherStatus = CurrEntity.LeatherStatus;
                                                OrgrEntity.SizeID = CurrEntity.SizeID;
                                                OrgrEntity.TargetQuantity = CurrEntity.TargetQuantity;
                                                OrgrEntity.UnitID = CurrEntity.UnitID;
                                                OrgrEntity.TargetValue = CurrEntity.TargetValue;
                                                OrgrEntity.CurrencyID = CurrEntity.CurrencyID;
                                                OrgrEntity.RecordStatus = CurrEntity.RecordStatus;
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }
                            else
                            {
                                #region Save Period Item List

                                if (model.PurchaseYearPeriodItemList != null)
                                {
                                    foreach (
                                        PrqPurchaseYearPeriodItem objPrqPurchaseYearPeriodItem in
                                            model.PurchaseYearPeriodItemList)
                                    {
                                        if (objPrqPurchaseYearPeriodItem.PeriodItemID == 0)
                                        {
                                            objPrqPurchaseYearPeriodItem.PeriodID = model.PeriodID;
                                            Prq_PurchaseYearPeriodItem tblPurchaseYearPeriodItem =
                                                SetToModelObject(objPrqPurchaseYearPeriodItem);
                                            _context.Prq_PurchaseYearPeriodItem.Add(tblPurchaseYearPeriodItem);
                                        }
                                        else
                                        {
                                            Prq_PurchaseYearPeriodItem CurrEntity =
                                                SetToModelObject(objPrqPurchaseYearPeriodItem);
                                            var OrgrEntity =
                                                _context.Prq_PurchaseYearPeriodItem.First(
                                                    m => m.PeriodItemID == objPrqPurchaseYearPeriodItem.PeriodItemID);

                                            OrgrEntity.ItemTypeID = CurrEntity.ItemTypeID;
                                            OrgrEntity.LeatherType = CurrEntity.LeatherType;
                                            OrgrEntity.LeatherStatus = CurrEntity.LeatherStatus;
                                            OrgrEntity.SizeID = CurrEntity.SizeID;
                                            OrgrEntity.TargetQuantity = CurrEntity.TargetQuantity;
                                            OrgrEntity.UnitID = CurrEntity.UnitID;
                                            OrgrEntity.TargetValue = CurrEntity.TargetValue;
                                            OrgrEntity.CurrencyID = CurrEntity.CurrencyID;
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
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Period is not Properly Define.";
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }

        public long GetYearID()
        {
            return YearID;
        }

        public ValidationMsg YearlyTargetConfirmed(PrqPurchaseYearTarget model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityYear = _context.Prq_PurchaseYearTarget.First(m => m.YearID == model.YearID);
                        originalEntityYear.RecordStatus = "CNF";

                        var originalEntity = _context.Prq_PurchaseYearPeriod.First(m => m.PeriodID == model.PeriodID);
                        originalEntity.RecordStatus = "CNF";

                        if (model.PurchaseYearPeriodItemList != null)
                        {
                            foreach (PrqPurchaseYearPeriodItem objPrqPurchaseYearPeriodItem in model.PurchaseYearPeriodItemList)
                            {
                                var originalEntityItem = _context.Prq_PurchaseYearPeriodItem.First(m => m.PeriodItemID == objPrqPurchaseYearPeriodItem.PeriodItemID);
                                originalEntityItem.RecordStatus = "CNF";
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

        public ValidationMsg YearlyTargetRevisioned(PrqPurchaseYearTarget model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var originalEntity = _context.Prq_PurchaseYearPeriod.First(m => m.PeriodID == model.PeriodID);
                originalEntity.RecordStatus = "NCF";

                foreach (PrqPurchaseYearPeriodItem objPrqPurchaseYearPeriodItem in model.PurchaseYearPeriodItemList)
                {
                    objPrqPurchaseYearPeriodItem.PeriodID = model.PeriodID;
                    objPrqPurchaseYearPeriodItem.SetBy = userid;
                    Prq_PurchaseYearPeriodItemAudit tblPrqPurchaseYearPeriodItemAudit = SetToModelObjectAudit(objPrqPurchaseYearPeriodItem);
                    _context.Prq_PurchaseYearPeriodItemAudit.Add(tblPrqPurchaseYearPeriodItemAudit);
                }

                _context.SaveChanges();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Revisioned Successfully.";
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Revisioned.";
            }
            return _vmMsg;
        }
        public Prq_PurchaseYearTarget SetToModelObject(PrqPurchaseYearTarget model)
        {
            Prq_PurchaseYearTarget Entity = new Prq_PurchaseYearTarget();

            Entity.YearStartDate = DalCommon.SetDate(model.YearStartDate);// Convert.ToDateTime(Convert.ToDateTime(model.YearStartDate).ToString("dd/MM/yyyy"));
            Entity.YearEndDate = DalCommon.SetDate(model.YearEndDate);//Convert.ToDateTime(Convert.ToDateTime(model.YearEndDate).ToString("dd/MM/yyyy"));
            Entity.PurchaseYear = model.PurchaseYear;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public Prq_PurchaseYearPeriod SetToModelObject(PrqPurchaseYearPeriod model)
        {
            Prq_PurchaseYearPeriod Entity = new Prq_PurchaseYearPeriod();

            Entity.StartDate = DalCommon.SetDate(model.StartDate);// Convert.ToDateTime(Convert.ToDateTime(model.StartDate).ToString("dd/MM/yyyy"));
            Entity.EndDate = DalCommon.SetDate(model.EndDate);// Convert.ToDateTime(Convert.ToDateTime(model.EndDate).ToString("dd/MM/yyyy"));
            Entity.YearID = model.YearID;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;

            return Entity;
        }

        public Prq_PurchaseYearPeriodItem SetToModelObject(PrqPurchaseYearPeriodItem model)
        {
            Prq_PurchaseYearPeriodItem Entity = new Prq_PurchaseYearPeriodItem();

            Entity.PeriodItemID = model.PeriodItemID;
            Entity.PeriodID = model.PeriodID;
            Entity.ItemTypeID = _context.Sys_ItemType.Where(m => m.ItemTypeName == model.ItemTypeName).FirstOrDefault().ItemTypeID;
            Entity.LeatherType = 1;
            Entity.LeatherStatus = _context.Sys_LeatherStatus.Where(m => m.LeatherStatusName == model.LeatherStatusName).FirstOrDefault().LeatherStatusID;
            Entity.SizeID = _context.Sys_Size.Where(m => m.SizeName == model.SizeName).FirstOrDefault().SizeID;
            Entity.PeriodID = model.PeriodID;
            Entity.TargetQuantity = model.TargetQuantity;
            Entity.TargetValue = model.TargetValue;
            Entity.UnitID = _context.Sys_Unit.Where(m => m.UnitName == model.UnitName).FirstOrDefault().UnitID;
            Entity.CurrencyID = _context.Sys_Currency.Where(m => m.CurrencyName == model.CurrencyName).FirstOrDefault().CurrencyID;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public Prq_PurchaseYearPeriodItemAudit SetToModelObjectAudit(PrqPurchaseYearPeriodItem model)
        {
            Prq_PurchaseYearPeriodItemAudit Entity = new Prq_PurchaseYearPeriodItemAudit();

            Entity.RevisionReason = "I";
            Entity.RevisionNo = 1;

            Entity.PeriodItemID = model.PeriodItemID;
            Entity.PeriodID = model.PeriodID;
            Entity.ItemTypeID = _context.Sys_ItemType.Where(m => m.ItemTypeName == model.ItemTypeName).FirstOrDefault().ItemTypeID;
            Entity.LeatherType = 1;
            Entity.LeatherStatus = _context.Sys_LeatherStatus.Where(m => m.LeatherStatusName == model.LeatherStatusName).FirstOrDefault().LeatherStatusID;
            Entity.SizeID = _context.Sys_Size.Where(m => m.SizeName == model.SizeName).FirstOrDefault().SizeID;
            Entity.PeriodID = model.PeriodID;
            Entity.TargetQuantity = model.TargetQuantity;
            Entity.TargetValue = model.TargetValue;
            Entity.UnitID = _context.Sys_Unit.Where(m => m.UnitName == model.UnitName).FirstOrDefault().UnitID;
            Entity.CurrencyID = _context.Sys_Currency.Where(m => m.CurrencyName == model.CurrencyName).FirstOrDefault().CurrencyID;
            Entity.RecordStatus = "CNF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = model.SetBy;
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public List<PrqPurchaseYearTarget> GetYearList()
        {
            List<Prq_PurchaseYearTarget> searchList = _context.Prq_PurchaseYearTarget.OrderByDescending(m => m.YearID).ToList(); //using table
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrqPurchaseYearTarget>();
        }

        public List<PrqPurchaseYearTarget> GetYearData(int yearid)
        {
            List<Prq_PurchaseYearTarget> searchList = _context.Prq_PurchaseYearTarget.Where(m => m.YearID == yearid).ToList(); //using table
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrqPurchaseYearTarget>();
        }

        public PrqPurchaseYearTarget SetToBussinessObject(Prq_PurchaseYearTarget Entity)
        {
            PrqPurchaseYearTarget Model = new PrqPurchaseYearTarget();

            Model.YearID = Entity.YearID;
            Model.PurchaseYear = Entity.PurchaseYear;
            Model.YearStartDate = Entity.YearStartDate.Date.ToString("dd/MM/yyyy");
            Model.YearEndDate = Entity.YearEndDate.Date.ToString("dd/MM/yyyy");
            Model.RecordStatus = Entity.RecordStatus;

            return Model;
        }

        public List<PrqPurchaseYearPeriod> GetPeriodList(int yearid)
        {
            List<Prq_PurchaseYearPeriod> searchList = _context.Prq_PurchaseYearPeriod.Where(m => m.YearID == yearid).OrderByDescending(m => m.PeriodID).ToList(); //using table
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrqPurchaseYearPeriod>();
        }

        public PrqPurchaseYearPeriod SetToBussinessObject(Prq_PurchaseYearPeriod Entity)
        {
            PrqPurchaseYearPeriod Model = new PrqPurchaseYearPeriod();

            Model.PeriodID = Entity.PeriodID;
            Model.YearID = Entity.YearID;
            Model.StartDate = Entity.StartDate.ToString("dd/MM/yyyy");
            Model.EndDate = Entity.EndDate.ToString("dd/MM/yyyy");
            Model.RecordStatus = Entity.RecordStatus;
            return Model;
        }

        public List<PrqPurchaseYearPeriodItem> GetPeriodItemList(int periodid)
        {
            List<Prq_PurchaseYearPeriodItem> searchList = _context.Prq_PurchaseYearPeriodItem.Where(m => m.PeriodID == periodid).OrderByDescending(m => m.PeriodItemID).ToList(); //using table
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrqPurchaseYearPeriodItem>();
        }

        public PrqPurchaseYearPeriodItem SetToBussinessObject(Prq_PurchaseYearPeriodItem Entity)
        {
            PrqPurchaseYearPeriodItem Model = new PrqPurchaseYearPeriodItem();

            Model.PeriodItemID = Entity.PeriodItemID;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherType = Entity.LeatherType;
            //Model.LeatherTypeName = _context.Sys_LeatherType.Where(m => m.LeatherTypeID == Entity.LeatherType).FirstOrDefault().LeatherTypeName;
            Model.LeatherStatusID = Entity.LeatherStatus;
            Model.LeatherStatusName = _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatus).FirstOrDefault().LeatherStatusName;
            Model.SizeID = Entity.SizeID;
            Model.SizeName = _context.Sys_Size.Where(m => m.SizeID == Entity.SizeID).FirstOrDefault().SizeName;
            Model.TargetQuantity = Entity.TargetQuantity;
            Model.UnitID = Entity.UnitID;
            Model.UnitName = _context.Sys_Unit.Where(m => m.UnitID == Entity.UnitID).FirstOrDefault().UnitName;
            Model.TargetValue = Entity.TargetValue;
            Model.CurrencyID = Entity.CurrencyID;
            Model.CurrencyName = _context.Sys_Currency.Where(m => m.CurrencyID == Entity.CurrencyID).FirstOrDefault().CurrencyName;
            Model.RecordStatus = Entity.RecordStatus;

            return Model;
        }

        public List<PrqPurchaseYearTarget> GetPurchaseYear()
        {
            var AllYear = (from y in _context.Prq_PurchaseYearTarget.AsEnumerable()
                           where y.YearStatus != "Closed"
                           select new PrqPurchaseYearTarget
                           {
                               //YearID = y.YearID,
                               PurchaseYear = y.PurchaseYear
                           }).ToList();

            return AllYear;
        }

        public ValidationMsg DeletedTargetYear(int yearId)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var periodList = _context.Prq_PurchaseYearPeriod.Where(m => m.YearID == yearId).ToList();

                if (periodList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteElement = _context.Prq_PurchaseYearTarget.First(m => m.YearID == yearId);
                    _context.Prq_PurchaseYearTarget.Remove(deleteElement);

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

        public ValidationMsg DeletedPeriod(int periodId, string RecordStatus)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                if (RecordStatus != "CNF")
                {
                    var periodItemList = _context.Prq_PurchaseYearPeriodItem.Where(m => m.PeriodID == periodId).ToList();
                    if (periodItemList.Count > 0)
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Child Record Found.";
                    }
                    else
                    {
                        foreach (var prqPurchaseYearPeriodItem in periodItemList)
                        {
                            var deletePeriodItem = _context.Prq_PurchaseYearPeriodItem.First(m => m.PeriodItemID == prqPurchaseYearPeriodItem.PeriodItemID);
                            _context.Prq_PurchaseYearPeriodItem.Remove(deletePeriodItem);
                        }
                        var deleteElement = _context.Prq_PurchaseYearPeriod.First(m => m.PeriodID == periodId);
                        _context.Prq_PurchaseYearPeriod.Remove(deleteElement);
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

        public ValidationMsg DeletedPeriodItem(int periodItemId, string RecordStatus)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                if (RecordStatus != "CNF")
                {
                    var deleteElement = _context.Prq_PurchaseYearPeriodItem.First(m => m.PeriodItemID == periodItemId);
                    _context.Prq_PurchaseYearPeriodItem.Remove(deleteElement);
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
    }
}

