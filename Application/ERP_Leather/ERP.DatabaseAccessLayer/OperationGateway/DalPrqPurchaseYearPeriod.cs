using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.BaseModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalPrqPurchaseYearPeriod
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;

        public DalPrqPurchaseYearPeriod()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(PrqPurchaseYearPeriod model)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Save Detail Records

                if (model.PurchaseYearPeriodItemList != null)
                {
                    foreach (PrqPurchaseYearPeriodItem objPrqPurchaseYearPeriodItem in model.PurchaseYearPeriodItemList)
                    {
                        if (objPrqPurchaseYearPeriodItem.PeriodItemID == 0)
                        {
                            objPrqPurchaseYearPeriodItem.PeriodID = model.PeriodID;
                            Prq_PurchaseYearPeriodItem tblPurchaseYearPeriodItem = SetToModelObject(objPrqPurchaseYearPeriodItem);
                            _context.Prq_PurchaseYearPeriodItem.Add(tblPurchaseYearPeriodItem);
                        }
                        else
                        {
                            Prq_PurchaseYearPeriodItem CurrentEntity = SetToModelObject(objPrqPurchaseYearPeriodItem);
                            var OriginalEntity = _context.Prq_PurchaseYearPeriodItem.First(m => m.PeriodItemID == objPrqPurchaseYearPeriodItem.PeriodItemID);

                            OriginalEntity.ItemTypeID = CurrentEntity.ItemTypeID;
                            OriginalEntity.LeatherType = CurrentEntity.LeatherType;
                            OriginalEntity.LeatherStatus = CurrentEntity.LeatherStatus;
                            OriginalEntity.SizeID = CurrentEntity.SizeID;
                            OriginalEntity.TargetQuantity = CurrentEntity.TargetQuantity;
                            OriginalEntity.UnitID = CurrentEntity.UnitID;
                            OriginalEntity.TargetValue = CurrentEntity.TargetValue;
                            OriginalEntity.CurrencyID = CurrentEntity.CurrencyID;
                        }
                    }
                }
                _context.SaveChanges();

                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Save Successfully.";
                #endregion
            }
            catch (Exception e)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public Prq_PurchaseYearPeriod SetToModelObject(PrqPurchaseYearPeriod model)
        {
            Prq_PurchaseYearPeriod Entity = new Prq_PurchaseYearPeriod();

            Entity.StartDate = Convert.ToDateTime(Convert.ToDateTime(model.StartDate).ToString("dd/MM/yyyy"));
            Entity.EndDate = Convert.ToDateTime(Convert.ToDateTime(model.EndDate).ToString("dd/MM/yyyy"));
            Entity.YearID = Convert.ToInt16(_context.Prq_PurchaseYearTarget.DefaultIfEmpty().Max(m => m.YearID == null ? 0 : m.YearID));
            Entity.RecordStatus = "12";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = Convert.ToInt32(25);
            Entity.ModifiedOn = DateTime.Now;
            Entity.ModifiedBy = Convert.ToInt32(25);
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public Prq_PurchaseYearPeriodItem SetToModelObject(PrqPurchaseYearPeriodItem model)
        {
            Prq_PurchaseYearPeriodItem Entity = new Prq_PurchaseYearPeriodItem();

            Entity.PeriodItemID = model.PeriodItemID;
            Entity.PeriodID = model.PeriodID;
            Entity.ItemTypeID = _context.Sys_ItemType.Where(m => m.ItemTypeName == model.ItemTypeName).FirstOrDefault().ItemTypeID;
            Entity.LeatherType = 1;
            //Entity.LeatherStatus = model.LeatherStatus;
            Entity.LeatherStatus = _context.Sys_LeatherStatus.Where(m => m.LeatherStatusName == model.LeatherStatusName).FirstOrDefault().LeatherStatusID;
            Entity.SizeID = _context.Sys_Size.Where(m => m.SizeName == model.SizeName).FirstOrDefault().SizeID;
            Entity.PeriodID = model.PeriodID;
            Entity.TargetQuantity = model.TargetQuantity;
            Entity.TargetValue = model.TargetValue;
            Entity.UnitID = _context.Sys_Unit.Where(m => m.UnitName == model.UnitName).FirstOrDefault().UnitID;
            Entity.CurrencyID = _context.Sys_Currency.Where(m => m.CurrencyName == model.CurrencyName).FirstOrDefault().CurrencyID;
            Entity.RecordStatus = "1";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = Convert.ToInt32(25);
            Entity.IPAddress = string.Empty;

            return Entity;
        }

        public List<PrqPurchaseYearTarget> GetPurchaseYear()
        {
            List<PrqPurchaseYearTarget> iLstSysCurrency = (from sc in _context.Prq_PurchaseYearTarget
                                                           select new PrqPurchaseYearTarget
                                                           {
                                                               YearID = sc.YearID,
                                                               PurchaseYear = sc.PurchaseYear
                                                           }).ToList();

            return iLstSysCurrency;
        }

        public List<PrqPurchaseYearPeriod> GetPurchaseYearPeriod(int yearid)
        {
            List<Prq_PurchaseYearPeriod> searchList = _context.Prq_PurchaseYearPeriod.Where(m => m.YearID == yearid).ToList(); //using table
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrqPurchaseYearPeriod>();
        }

        public PrqPurchaseYearPeriod SetToBussinessObject(Prq_PurchaseYearPeriod Entity)
        {
            PrqPurchaseYearPeriod Model = new PrqPurchaseYearPeriod();

            Model.PeriodID = Entity.PeriodID;
            Model.PeriodName = GetShortMonthName(Entity.StartDate.Month) + "-" + GetShortMonthName(Entity.EndDate.Month);

            return Model;
        }

        public List<PrqPurchaseYearPeriodItem> GetPeriodItemList(int periodid)
        {
            List<Prq_PurchaseYearPeriodItem> searchList = _context.Prq_PurchaseYearPeriodItem.Where(m => m.PeriodID == periodid).ToList(); //using table
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

            return Model;
        }

        private string GetShortMonthName(int month)
        {
            switch (month)
            {
                case 1:
                    return "Jan";
                case 2:
                    return "Feb";
                case 3:
                    return "Mar";
                case 4:
                    return "Apr";
                case 5:
                    return "May";
                case 6:
                    return "Jun";
                case 7:
                    return "Jul";
                case 8:
                    return "Aug";
                case 9:
                    return "Sep";
                case 10:
                    return "Oct";
                case 11:
                    return "Nov";
                case 12:
                    return "Dec";
                default:
                    return "";
            }
        }
    }
}

