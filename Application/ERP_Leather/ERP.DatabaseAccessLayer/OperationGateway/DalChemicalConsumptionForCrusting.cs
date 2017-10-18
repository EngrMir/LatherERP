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
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.AppSetupModel;
using System.Transactions;
using System.Linq;


namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalChemicalConsumptionForCrusting
    {
        private readonly BLC_DEVEntities _context;

        public DalChemicalConsumptionForCrusting()
        {
            _context = new BLC_DEVEntities();
        }
        public List<PrdYearMonthScheduleDate> GetAllScheduleDate(string _ScheduleID)
        {
            using (_context)
            {
                var Data = (from r in _context.PRD_YearMonthScheduleDate.AsEnumerable()
                            where (r.ScheduleID).ToString() == _ScheduleID
                            select new PrdYearMonthScheduleDate
                            {
                                ScheduleDateID = r.ScheduleDateID,
                                ProductionNo=r.ProductionNo,
                                ProductionStatus = r.ProductionStatus,
                                ScheduleStartDate = Convert.ToDateTime(r.ScheduleStartDate).ToString("dd'/'MM'/'yyyy"),
                                ScheduleEndDate = Convert.ToDateTime(r.ScheduleEndDate).ToString("dd'/'MM'/'yyyy"),
                                //RecordStatus = (r.RecordStatus == "NCF" ? "Not Confirmed" : "Confirmed")
                            }).ToList();

                return Data;
            }

        }
        public List<PrdYearMonthCrustScheduleItem> GetAllScheduleItem(string _ScheduleDateID)
        {
            using (_context)
            {
                var ScheduleItem = (from i in _context.PRD_YearMonthCrustScheduleItem.AsEnumerable()
                                    where i.ScheduleDateID.ToString()== _ScheduleDateID

                                    join b in _context.Sys_Buyer on i.BuyerID equals b.BuyerID into Buyers
                                    from b in Buyers.DefaultIfEmpty()

                                    join o in _context.SLS_BuyerOrder on i.BuyerOrderID equals o.BuyerOrderID into Orders
                                    from o in Orders.DefaultIfEmpty()

                                    join u in _context.Sys_Unit on i.ScheduleAreaUnit equals u.UnitID into Units
                                    from u in Units.DefaultIfEmpty()

                                    join wu in _context.Sys_Unit on i.ThicknessUnit equals wu.UnitID into WeightUnits
                                    from wu in WeightUnits.DefaultIfEmpty()

                                    join a in _context.Sys_Article on i.ArticleID equals a.ArticleID into Articles
                                    from a in Articles.DefaultIfEmpty()

                                    join it in _context.Sys_ItemType on i.ItemTypeID equals it.ItemTypeID into ItemTypes
                                    from it in ItemTypes.DefaultIfEmpty()

                                    join ls in _context.Sys_LeatherStatus on i.LeatherStatusID equals ls.LeatherStatusID into LeatherStatus
                                    from ls in LeatherStatus.DefaultIfEmpty()

                                    select new PrdYearMonthCrustScheduleItem
                                    {
                                        ScheduleItemID = i.ScheduleItemID,
                                        ScheduleProductionNo = i.ScheduleProductionNo,
                                        BuyerID = i.BuyerID,
                                        BuyerName = (b == null ? null : b.BuyerName),
                                        BuyerOrderID = i.BuyerOrderID,
                                        BuyerOrderNo = (o == null ? null : o.BuyerOrderNo),
                                        ItemTypeID = i.ItemTypeID,
                                        ItemTypeName = (it == null ? null : it.ItemTypeName),
                                        LeatherStatusID = i.LeatherStatusID,
                                        LeatherStatusName = (ls == null ? null : ls.LeatherStatusName),
                                        ProductionArticleID = i.ArticleID,
                                        ProductionArticleNo = i.ArticleNo,
                                        ProductionArticleName = (a == null ? null : a.ArticleName),
                                        ProductionArticleChallanID= i.ArticleChallanID,
                                        ProductionArticleChallanNo = i.ArticleChallanNo,
                                        SchedulePcs= i.SchedulePcs,
                                        ScheduleSide= Convert.ToDecimal(i.ScheduleSide),
                                        ScheduleArea= i.ScheduleArea,
                                        ScheduleAreaUnit = i.ScheduleAreaUnit,
                                        ScheduleAreaUnitName = (u == null ? "" : u.UnitName),
                                        ScheduleWeight= i.ScheduleWeight,
                                        ScheduleWeightUnit= i.ScheduleWeightUnit,
                                        ScheduleWeightUnitName = (wu == null ? "" : wu.UnitName),
                                    }).ToList();

                return ScheduleItem;
            }

        }

        public List<PrdYearMonthCrustScheduleDrum> GetAllDrumList(string _SdulItemColorID)
        {
            using(_context)
            {
                var Data = (from d in _context.PRD_YearMonthCrustScheduleDrum.AsEnumerable()
                            where d.SdulItemColorID.ToString() == _SdulItemColorID

                            //join g in _context.Sys_Grade on d.GradeID equals g.GradeID into Grades
                            //from g in Grades.DefaultIfEmpty()

                            join au in _context.Sys_Unit on d.AreaUnit equals au.UnitID into AreaUnits
                            from au in AreaUnits.DefaultIfEmpty()

                            join wu in _context.Sys_Unit on d.WeightUnit equals wu.UnitID into WeightUnits
                            from wu in WeightUnits.DefaultIfEmpty()

                            select new PrdYearMonthCrustScheduleDrum
                            {
                                CrustSdulDrumID = d.CrustSdulDrumID,
                                MachineID = d.MachineID,
                                MachineNo = d.MachineNo,
                                //GradeID = d.GradeID,
                                //GradeName = (g == null ? null : g.GradeName),
                                GradeRange= d.GradeRange,
                                DrumPcs = d.DrumPcs,
                                DrumSide = d.DrumSide,
                                DrumArea = d.DrumArea,
                                AreaUnit = d.AreaUnit,
                                AreaUnitName = (au == null ? "" : au.UnitName),
                                DrumWeight = d.DrumWeight,
                                WeightUnit = d.WeightUnit,
                                WeightUnitName = (wu == null ? "" : wu.UnitName),
                                Remarks = d.Remarks
                            }).ToList();

                return Data;
            }
        }

        public List<PrdYearMonth> GetAllYearMonthCombinationForConsumption(string _ScheduleFor)
        {
            using (_context)
            {
                var Data = (from r in _context.PRD_YearMonth.AsEnumerable()
                            where r.ScheduleFor == _ScheduleFor
                            join s in _context.SYS_Store on r.ProductionFloor equals s.StoreID into Stores
                            from s in Stores.DefaultIfEmpty()

                            join c in _context.SYS_Store on r.ConcernStore equals c.StoreID into ConcernStores
                            from c in ConcernStores.DefaultIfEmpty()

                            join p in _context.PRD_YearMonthSchedule on r.YearMonID equals p.YearMonID into Schedules
                            from p in Schedules.DefaultIfEmpty()

                            join pp in _context.Sys_ProductionProces on (p == null ? null : p.ProductionProcessID) equals pp.ProcessID into Process
                            from pp in Process.DefaultIfEmpty()
                            //orderby p.ScheduleID descending

                            select new PrdYearMonth
                            {
                                YearMonID = r.YearMonID,
                                ScheduleMonth = r.ScheduleMonth,
                                ScheduleMonthName = DalCommon.ReturnMonthName(r.ScheduleMonth),
                                ScheduleYear = r.ScheduleYear,
                                ProductionFloor = r.ProductionFloor,
                                ProductionFloorName = (s == null ? null : s.StoreName),
                                ConcernStore = r.ConcernStore,
                                ConcernStoreName = (c == null ? null : c.StoreName),

                                ScheduleID = (p == null ? 0 : p.ScheduleID),
                                ScheduleNo = (p == null ? null : p.ScheduleNo),
                                PrepareDate = (p == null ? null : Convert.ToDateTime(p.PrepareDate).ToString("dd'/'MM'/'yyyy")),
                                ProductionProcessID = (p == null ? null : p.ProductionProcessID),
                                ProductionProcessName = (pp == null ? null : pp.ProcessName)
                            }).ToList();

                return Data;
            }

        }

        public long Save(ChemicalConsumptionForCrusting model, int userId, string pageURL)
        {

            long CurrentCLProductionID = 0;
            long CurrentCLProductionItemID = 0;
            long CurrentCLProductionColorID = 0;
            long CurrentCLProductionDrumID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    
                    using (_context)
                    {
                        #region New_Production_Insert
                        PRD_CLProduction objProduction = new PRD_CLProduction();

                        objProduction.CLProductionNo = DalCommon.GetPreDefineNextCodeByUrl("ChemicalConsumptionForCrusting/ChemicalConsumptionForCrusting");
                        objProduction.ProductionFor = "CR";
                        objProduction.ProductionFloor = model.ProductionFloor;
                        objProduction.ScheduleItemID = model.ScheduleItemID;
                        objProduction.ScheduleProductionNo = model.ScheduleProductionNo;
                        objProduction.ScheduleDateID = model.ScheduleDateID;
                        objProduction.ScheduleID = model.ScheduleID;
                        objProduction.YearMonID = model.YearMonID;
                        objProduction.CLProductionStartDate = DalCommon.SetDate(model.ScheduleStartDate);
                        objProduction.CLProductionEndDate = DalCommon.SetDate(model.ScheduleEndDate);
                        objProduction.ProductionProcessID = model.ProductionProcessID;
                        objProduction.RecordStatus = "NCF";
                        objProduction.SetOn = DateTime.Now;
                        objProduction.SetBy = userId;

                        _context.PRD_CLProduction.Add(objProduction);
                        _context.SaveChanges();

                        CurrentCLProductionID = objProduction.CLProductionID;
                        #endregion

                        #region Production Item Insertion
                        PRD_CLProductionItem objProductionItem = new PRD_CLProductionItem();

                        objProductionItem.CLProductionID = CurrentCLProductionID;
                        objProductionItem.ScheduleProductionNo = model.ScheduleProductionNo;
                        objProductionItem.BuyerID = model.BuyerID;
                        objProductionItem.BuyerOrderID = model.BuyerOrderID;

                        if (model.ProductionArticleID != 0)
                        {
                            objProductionItem.ArticleID = model.ProductionArticleID;
                            objProductionItem.ArticleNo = model.ProductionArticleNo;
                        }
                       
                        if(model.ProductionArticleChallanID!=0)
                        {
                            objProductionItem.ArticleChallanID = model.ProductionArticleChallanID;
                            objProductionItem.ArticleChallanNo = model.ProductionArticleChallanNo;
                        }
                        
                        objProductionItem.ProductionArea = model.ScheduleArea;
                        objProductionItem.AreaUnit = model.ScheduleAreaUnit;
                        objProductionItem.ProductionPcs = model.SchedulePcs;
                        objProductionItem.ProductionWeight = model.ScheduleWeight;
                        objProductionItem.ProductionWeightUnit = model.ScheduleWeightUnit;
                        objProductionItem.ProductionSide = model.ScheduleSide;
                        objProductionItem.ItemTypeID = model.ItemTypeID;
                        objProductionItem.LeatherStatusID = model.LeatherStatusID;
                        objProductionItem.RecordStatus = "NCF";
                        objProductionItem.SetBy = userId;
                        objProductionItem.SetOn = DateTime.Now;
                        //objProductionItem.Remark=

                        _context.PRD_CLProductionItem.Add(objProductionItem);
                        _context.SaveChanges();
                        CurrentCLProductionItemID = objProductionItem.CLProductionItemID;
                        #endregion

                        #region Production Color Insertion
                        PRD_CLProductionColor objProductionColor = new PRD_CLProductionColor();

                        objProductionColor.CLProductionItemID = CurrentCLProductionItemID;
                        objProductionColor.ColorID = model.ColorID;
                        objProductionColor.ArticleColorNo = model.ArticleColorNo;
                        objProductionColor.ColorArea = model.ColorArea;
                        objProductionColor.AreaUnit = model.AreaUnit;
                        objProductionColor.ColorPCS = model.ColorPCS;
                        objProductionColor.ColorSide = model.ColorSide;
                        objProductionColor.ColorWeight = model.ColorWeight;
                        objProductionColor.WeightUnit = model.WeightUnit;
                        objProductionColor.Remarks = model.Remarks;
                        objProductionColor.SetBy = userId;
                        objProductionColor.SetOn = DateTime.Now;

                        _context.PRD_CLProductionColor.Add(objProductionColor);
                        _context.SaveChanges();
                        CurrentCLProductionColorID = objProductionColor.CLProductionColorID;
                        #endregion


                        #region Leather Insert
                        if (model.LeatherList != null)
                        {
                            foreach (var LeatherItem in model.LeatherList)
                            {
                                PRD_CLProductionDrum objLeather = new PRD_CLProductionDrum();

                                objLeather.CLProductionColorID = CurrentCLProductionColorID;
                                objLeather.MachineID = LeatherItem.MachineID;
                                objLeather.MachineNo = LeatherItem.MachineNo;
                                objLeather.ColorID = LeatherItem.ColorID;
                                objLeather.GradeRange = LeatherItem.GradeRange;
                                objLeather.DrumArea = LeatherItem.DrumArea;
                                objLeather.DrumPcs = LeatherItem.DrumPcs;
                                objLeather.DrumSide = LeatherItem.DrumSide;
                                objLeather.AreaUnit = LeatherItem.AreaUnit;
                                objLeather.DrumWeight = LeatherItem.DrumWeight;
                                objLeather.WeightUnit = LeatherItem.WeightUnit;
                                objLeather.Remarks = LeatherItem.Remarks;
                                objLeather.SetBy = userId;
                                objLeather.SetOn = DateTime.Now;
                                
                                _context.PRD_CLProductionDrum.Add(objLeather);
                                _context.SaveChanges();
                                if(LeatherItem.CrustSdulDrumID== model.ParentID)
                                CurrentCLProductionDrumID = objLeather.CLProductionDrumID;
                            }

                        }
                        #endregion

                        #region Chemical Insert
                        if (model.ChemicalList != null)
                        {
                            foreach (var ChemicalItem in model.ChemicalList)
                            {

                                PRD_CLProductionChemical objChemical = new PRD_CLProductionChemical();

                                if (CurrentCLProductionDrumID != 0 && ChemicalItem.CalculationBase == "DR")
                                    objChemical.CLProductionDrumID = CurrentCLProductionDrumID;
                                else
                                    objChemical.CLProductionDrumID = null;
                                objChemical.CLProductionID = CurrentCLProductionID;
                                objChemical.CLProductionItemID = CurrentCLProductionItemID;
                                objChemical.CLProductionColorID = CurrentCLProductionColorID;

                                if(ChemicalItem.RecipeID!=0)
                                objChemical.RecipeID = ChemicalItem.RecipeID;
                                objChemical.ItemID = Convert.ToInt16(ChemicalItem.ItemID);
                                objChemical.CalculationBase = ChemicalItem.CalculationBase;
                                objChemical.RequiredQty = ChemicalItem.RequiredQty;

                                if (ChemicalItem.RequiredUnitName == null)
                                    objChemical.RequiredUnitID = null;
                                else
                                    objChemical.RequiredUnitID = DalCommon.GetUnitCode(ChemicalItem.RequiredUnitName);

                                objChemical.UseQty = ChemicalItem.UseQty;
                                objChemical.UseUnitID = DalCommon.GetUnitCode(ChemicalItem.UseUnitName);

                                if (ChemicalItem.SupplierID == 0)
                                    objChemical.SupplierID = null;
                                else
                                    objChemical.SupplierID = ChemicalItem.SupplierID;

                                objChemical.ItemSource = DalCommon.ReturnItemSource(ChemicalItem.ItemSource);
                                objChemical.SetBy = userId;
                                objChemical.SetOn = DateTime.Now;

                                _context.PRD_CLProductionChemical.Add(objChemical);
                                _context.SaveChanges();
                            }
                        }
                        #endregion


                    }
                    
                    transaction.Complete();
                }
                return CurrentCLProductionID;
            }
            catch (Exception e)
            {
                return 0;
            }

        }


        public int Update(ChemicalConsumptionForCrusting model, int userId)
        {
            //var CurrentRequisitionID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (_context)
                    {
                        #region Update_Chemical_Item_Information
                        if (model.ChemicalList != null)
                        {
                            foreach (var ChemicalItem in model.ChemicalList)
                            {
                                if (ChemicalItem.CLProdChemicalID == 0)
                                {
                                    PRD_CLProductionChemical objChemical = new PRD_CLProductionChemical();
                                   

                                    if (model.CLProductionDrumID != 0 && ChemicalItem.CalculationBase == "DR")
                                        objChemical.CLProductionDrumID = model.CLProductionDrumID;
                                    else
                                        objChemical.CLProductionDrumID = null;

                                    objChemical.CLProductionID = model.CLProductionID;
                                    objChemical.CLProductionItemID = model.CLProductionItemID;
                                    objChemical.CLProductionColorID = model.CLProductionColorID;

                                    if (ChemicalItem.RecipeID != 0)
                                    objChemical.RecipeID = ChemicalItem.RecipeID;
                                    objChemical.ItemID = Convert.ToInt16(ChemicalItem.ItemID);
                                    objChemical.CalculationBase = ChemicalItem.CalculationBase;
                                    objChemical.RequiredQty = ChemicalItem.RequiredQty;

                                    if (ChemicalItem.RequiredUnitName == null)
                                        objChemical.RequiredUnitID = null;
                                    else
                                        objChemical.RequiredUnitID = DalCommon.GetUnitCode(ChemicalItem.RequiredUnitName);

                                    objChemical.UseQty = ChemicalItem.UseQty;
                                    objChemical.UseUnitID = DalCommon.GetUnitCode(ChemicalItem.UseUnitName);

                                    if (ChemicalItem.SupplierID == 0)
                                        objChemical.SupplierID = null;
                                    else
                                        objChemical.SupplierID = ChemicalItem.SupplierID;

                                    objChemical.ItemSource = DalCommon.ReturnItemSource(ChemicalItem.ItemSource);
                                    objChemical.SetBy = userId;
                                    objChemical.SetOn = DateTime.Now;

                                    _context.PRD_CLProductionChemical.Add(objChemical);
                                }
                                else
                                {
                                    #region Update_Existing_Chemical_Item

                                    var CurrentChemicalItem = (from c in _context.PRD_CLProductionChemical
                                                               where c.CLProdChemicalID == ChemicalItem.CLProdChemicalID
                                                               select c).FirstOrDefault();


                                    if (ChemicalItem.RecipeID != 0)
                                    CurrentChemicalItem.RecipeID = ChemicalItem.RecipeID;
                                    CurrentChemicalItem.ItemID = Convert.ToInt16(ChemicalItem.ItemID);
                                    CurrentChemicalItem.CalculationBase = ChemicalItem.CalculationBase;
                                    CurrentChemicalItem.RequiredQty = ChemicalItem.RequiredQty;

                                    if (ChemicalItem.RequiredUnitName == null)
                                        CurrentChemicalItem.RequiredUnitID = null;
                                    else
                                        CurrentChemicalItem.RequiredUnitID = DalCommon.GetUnitCode(ChemicalItem.RequiredUnitName);

                                    CurrentChemicalItem.UseQty = ChemicalItem.UseQty;
                                    CurrentChemicalItem.UseUnitID = DalCommon.GetUnitCode(ChemicalItem.UseUnitName);

                                    if (ChemicalItem.SupplierID == 0)
                                        CurrentChemicalItem.SupplierID = null;
                                    else
                                        CurrentChemicalItem.SupplierID = ChemicalItem.SupplierID;

                                    CurrentChemicalItem.ItemSource = DalCommon.ReturnItemSource(ChemicalItem.ItemSource);

                                    CurrentChemicalItem.ModifiedBy = userId;
                                    CurrentChemicalItem.ModifiedOn = DateTime.Now;
                                    #endregion
                                }

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


        public List<PrdYearMonthCrustScheduleDrum> GetLeatherListAfterSave(long CLProductionID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var ItemID = context.PRD_CLProductionItem.Where(x => x.CLProductionID == CLProductionID).Select(x => x.CLProductionItemID).FirstOrDefault();

                var ColorID = context.PRD_CLProductionColor.Where(x => x.CLProductionItemID == ItemID).Select(x => x.CLProductionColorID).FirstOrDefault();

                var Data = (from d in context.PRD_CLProductionDrum
                            where d.CLProductionColorID == ColorID

                            join g in context.Sys_Grade on d.GradeID equals g.GradeID into Grades
                            from g in Grades.DefaultIfEmpty()

                            join au in context.Sys_Unit on d.AreaUnit equals au.UnitID into AreaUnits
                            from au in AreaUnits.DefaultIfEmpty()

                            join wu in context.Sys_Unit on d.WeightUnit equals wu.UnitID into WeightUnits
                            from wu in WeightUnits.DefaultIfEmpty()

                            select new PrdYearMonthCrustScheduleDrum
                            {
                                CLProductionDrumID = d.CLProductionDrumID,
                                CLProdChemicalID= d.CLProductionColorID,
                                MachineID = d.MachineID,
                                MachineNo = d.MachineNo,
                                GradeRange= d.GradeRange,
                                DrumPcs = d.DrumPcs,
                                DrumSide = d.DrumSide,
                                DrumArea = d.DrumArea,
                                AreaUnit = d.AreaUnit,
                                AreaUnitName = (au == null ? "" : au.UnitName),
                                DrumWeight = d.DrumWeight,
                                WeightUnit = d.WeightUnit,
                                WeightUnitName = (wu == null ? "" : wu.UnitName),
                                Remarks = d.Remarks
                            }).ToList();
                return Data;
            }
        }

        public long GetCLProductionItemID(long CLProductionID)
        {
            using(var context= new BLC_DEVEntities())
            {
                return context.PRD_CLProductionItem.Where(x => x.CLProductionID == CLProductionID).Select(x => x.CLProductionItemID).FirstOrDefault();
            }
        }
        public long GetCLProductionColorID(long CLProductionID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var ItemID = context.PRD_CLProductionItem.Where(x => x.CLProductionID == CLProductionID).Select(x => x.CLProductionItemID).FirstOrDefault();

                return context.PRD_CLProductionColor.Where(x => x.CLProductionItemID == ItemID).Select(x => x.CLProductionColorID).FirstOrDefault();
            }
        }

        public List<PRDChemProdReqItem> GetChemicalListAfterSave(long CLProductionID, byte? _StoreID, string _CalculationBase)
        {
            using (var context = new BLC_DEVEntities())
            {
                if(_CalculationBase=="DR")
                {
                    return null;
                }
                else
                {
                    //var StockResult = (from p in context.INV_ChemStockSupplier
                    //                   where p.StoreID == _StoreID 
                    //                   group p by new
                    //                   {
                    //                       p.SupplierID,
                    //                       p.ItemID,
                    //                       p.UnitID
                    //                   } into g
                    //                   select new 
                    //                   {
                    //                       TransectionID = g.Max(p => p.TransectionID),
                    //                       StoreID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.StoreID).FirstOrDefault(),
                    //                       ItemID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ItemID).FirstOrDefault(),
                    //                       SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                    //                       ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault(),
                    //                       StockUnit = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault()
                    //                   });

                    var StockResult = DalChemicalStock.CombinationWiseStockInSpecificProductionFloor(Convert.ToByte(_StoreID));

                   
                    var Data = (from c in context.PRD_CLProductionChemical.AsEnumerable()
                                where c.CLProductionID == CLProductionID

                                join i in context.Sys_ChemicalItem on c.ItemID equals i.ItemID into ChemicalItem
                                from i2 in ChemicalItem.DefaultIfEmpty()


                                join ru in context.Sys_Unit on c.RequiredUnitID equals ru.UnitID into RequiredUnit
                                from ru2 in RequiredUnit.DefaultIfEmpty()

                                join uu in context.Sys_Unit on c.UseUnitID equals uu.UnitID into UseUnit
                                from uu2 in UseUnit.DefaultIfEmpty()

                                join sup in context.Sys_Supplier on c.SupplierID equals sup.SupplierID into SupplierInfo
                                from sup2 in SupplierInfo.DefaultIfEmpty()

                                join s in StockResult on new { ItemID = c.ItemID, SupplierID = c.SupplierID, UnitID = c.UseUnitID } equals
                                new { ItemID = s.ItemID, SupplierID = s.SupplierID, UnitID = s.UnitID } into StockInfo
                                from item in StockInfo.DefaultIfEmpty()

                                join StockUnit in context.Sys_Unit on c.UseUnitID equals StockUnit.UnitID into StockUnits
                                from StockUnit in StockUnits.DefaultIfEmpty()

                                orderby i2.ItemName

                                select new PRDChemProdReqItem
                                {
                                    CLProdChemicalID = c.CLProdChemicalID,
                                    CLProductionDrumID = (c.CLProductionDrumID),
                                    ItemID = (c.ItemID),
                                    ItemName = (i2 == null ? null : i2.ItemName),
                                    RequiredQty = (c.RequiredQty),
                                    RequiredUnit = (c.RequiredUnitID),
                                    RequiredUnitName = (ru2 == null ? null : ru2.UnitName),
                                    RequsitionQty = (c.UseQty),
                                    RequisitionUnit = (c.UseUnitID),
                                    RequisitionUnitName = (uu2 == null ? null : uu2.UnitName),
                                    SupplierID = (c.SupplierID),
                                    SupplierName = (sup2 == null ? null : sup2.SupplierName),
                                    StockQty = (item == null ? 0 : (item.ClosingQty)),
                                    StockUnit = (item == null ? null : (item.UnitID)),
                                    StockUnitName = (StockUnit == null ? "" : StockUnit.UnitName)
                                }).ToList();

                    return Data;
                }
            }
        }

        public List<ChemicalConsumptionForCrusting> GetSearchInformaiton()
        {
            using(_context)
            {
                var Data = (from p in _context.PRD_CLProduction.AsEnumerable()

                            join pi in _context.PRD_CLProductionItem on p.CLProductionID equals pi.CLProductionID into Items
                            from pi in Items.DefaultIfEmpty()

                            join b in _context.Sys_Buyer on (pi == null ? null : pi.BuyerID) equals b.BuyerID into Buyers
                            from b in Buyers.DefaultIfEmpty()

                            join ba in _context.SLS_BuyerOrder on (pi == null ? null : pi.BuyerOrderID) equals ba.BuyerOrderID into BuyerOrders
                            from ba in BuyerOrders.DefaultIfEmpty()

                            join a in _context.Sys_Article on (pi == null ? null : pi.ArticleID) equals a.ArticleID into Articles
                            from a in Articles.DefaultIfEmpty()

                            join it in _context.Sys_ItemType on (pi == null ? null : pi.ItemTypeID) equals it.ItemTypeID into ItemTypes
                            from it in ItemTypes.DefaultIfEmpty()

                            join ls in _context.Sys_LeatherStatus on (pi == null ? null : pi.LeatherStatusID) equals ls.LeatherStatusID into LeatherStatus
                            from ls in LeatherStatus.DefaultIfEmpty()

                            join c in _context.PRD_CLProductionColor on (pi == null ? 0 : pi.CLProductionItemID) equals c.CLProductionItemID into Colors
                            from c in Colors.DefaultIfEmpty()

                            join cn in _context.Sys_Color on (c == null ? null : c.ColorID) equals cn.ColorID into ColorNames
                            from cn in ColorNames.DefaultIfEmpty()

                            join au in _context.Sys_Unit on (c == null ? null : c.AreaUnit) equals au.UnitID into AreaUnits
                            from au in AreaUnits.DefaultIfEmpty()

                            join wu in _context.Sys_Unit on (c == null ? null : c.WeightUnit) equals wu.UnitID into WeightUnits
                            from wu in WeightUnits.DefaultIfEmpty()

                            orderby p.CLProductionID descending

                            select new ChemicalConsumptionForCrusting
                            {
                                CLProductionID = p.CLProductionID,
                                ScheduleProductionNo = p.ScheduleProductionNo,
                                BuyerName = (b == null ? null : b.BuyerName),
                                BuyerOrderNo = (ba == null ? null : ba.BuyerOrderNo),
                                ArticleName = (a == null ? null : a.ArticleName),
                                ProductionArticleChallanNo = (pi == null ? null : pi.ArticleChallanNo),
                                ItemTypeName = (it == null ? null : it.ItemTypeName),
                                LeatherStatusName = (ls == null ? null : ls.LeatherStatusName),
                                ColorName = (cn == null ? null : cn.ColorName),
                                ColorArea = (c == null ? null : c.ColorArea),
                                AreaUnitName = (au == null ? "" : au.UnitName),
                                ColorWeight = (c == null ? null : c.ColorWeight),
                                WeightUnitName = (wu == null ? "" : wu.UnitName),
                                RecordStatus = DalCommon.ReturnRecordStatus(p.RecordStatus),
                                ScheduleStartDate = Convert.ToDateTime(p.CLProductionStartDate).ToString("dd/MM/yyyy"),
                                ScheduleEndDate = Convert.ToDateTime(p.CLProductionEndDate).ToString("dd/MM/yyyy")

                            }).ToList();

                return Data;
            }
        }

        public ChemicalConsumptionForCrusting GetDetailsSearchInformaiton(long _CLProductionID)
        {
            using (_context)
            {
                ChemicalConsumptionForCrusting model = new ChemicalConsumptionForCrusting();

                var FormInfo = (from p in _context.PRD_CLProduction
                                where p.CLProductionID == _CLProductionID

                                join pf in _context.SYS_Store on p.ProductionFloor equals pf.StoreID into ProductionFloors
                                from pf in ProductionFloors.DefaultIfEmpty()

                                join ym in _context.PRD_YearMonth on p.YearMonID equals ym.YearMonID into YearMonths
                                from ym in YearMonths.DefaultIfEmpty()

                                join yms in _context.PRD_YearMonthSchedule on p.ScheduleID equals yms.ScheduleID into YearMonthSchedules
                                from yms in YearMonthSchedules.DefaultIfEmpty()

                                join pp in _context.Sys_ProductionProces on (yms == null ? null : yms.ProductionProcessID) equals pp.ProcessID into ProductionProcess
                                from pp in ProductionProcess.DefaultIfEmpty()

                                join ymsd in _context.PRD_YearMonthScheduleDate on p.ScheduleDateID equals ymsd.ScheduleDateID into YearMonthScheduleDates
                                from ymsd in YearMonthScheduleDates.DefaultIfEmpty()

                                join pi in _context.PRD_CLProductionItem on p.CLProductionID equals pi.CLProductionID into Items
                                from pi in Items.DefaultIfEmpty()

                                join pau in _context.Sys_Unit on (pi == null ? null : pi.AreaUnit) equals pau.UnitID into ProductionAreaUnits
                                from pau in ProductionAreaUnits.DefaultIfEmpty()

                                join b in _context.Sys_Buyer on (pi == null ? null : pi.BuyerID) equals b.BuyerID into Buyers
                                from b in Buyers.DefaultIfEmpty()

                                join ba in _context.SLS_BuyerOrder on (pi == null ? null : pi.BuyerOrderID) equals ba.BuyerOrderID into BuyerOrders
                                from ba in BuyerOrders.DefaultIfEmpty()

                                join it in _context.Sys_ItemType on (pi == null ? null : pi.ItemTypeID) equals it.ItemTypeID into ItemTypes
                                from it in ItemTypes.DefaultIfEmpty()

                                join ls in _context.Sys_LeatherStatus on (pi == null ? null : pi.LeatherStatusID) equals ls.LeatherStatusID into LeatherStatus
                                from ls in LeatherStatus.DefaultIfEmpty()

                                join a in _context.Sys_Article on (pi == null ? null : pi.ArticleID) equals a.ArticleID into Articles
                                from a in Articles.DefaultIfEmpty()

                                join c in _context.PRD_CLProductionColor on (pi == null ? 0 : pi.CLProductionItemID) equals c.CLProductionItemID into Colors
                                from c in Colors.DefaultIfEmpty()

                                join cn in _context.Sys_Color on (c == null ? null : c.ColorID) equals cn.ColorID into ColorNames
                                from cn in ColorNames.DefaultIfEmpty()

                                join cau in _context.Sys_Unit on (c == null ? null : c.AreaUnit) equals cau.UnitID into ColorAreaUnits
                                from cau in ColorAreaUnits.DefaultIfEmpty()

                                join cwu in _context.Sys_Unit on (c == null ? null : c.WeightUnit) equals cwu.UnitID into ColorWeightUnits
                                from cwu in ColorWeightUnits.DefaultIfEmpty()

                                join d in _context.PRD_CLProductionDrum on (c == null ? 0 : c.CLProductionColorID) equals d.CLProductionColorID into Drums
                                from d in Drums.DefaultIfEmpty()


                                join che in _context.PRD_CLProductionChemical on p.CLProductionID equals che.CLProductionID into Chemicals
                                from che in Chemicals.DefaultIfEmpty()

                                join rec in _context.PRD_Recipe on (che == null ? null : che.RecipeID) equals rec.RecipeID into Recipes
                                from rec in Recipes.DefaultIfEmpty()

                                join bu in _context.Sys_Unit on (rec == null ? 0 : rec.BaseUnit) equals bu.UnitID into BaseUnits
                                from bu in BaseUnits.DefaultIfEmpty()

                                //join ra in _context.Sys_Article on (rec==null? null: rec.ArticleID) equals ra.ArticleID into RecipeArticles
                                //from ra in RecipeArticles.DefaultIfEmpty()

                                join co in _context.Sys_Color on (rec == null ? 0 : rec.ArticleColor) equals co.ColorID into RecipeColors
                                from co in RecipeColors.DefaultIfEmpty()

                                select new ChemicalConsumptionForCrusting
                                {
                                    CLProductionID = p.CLProductionID,
                                    ProductionFloor = p.ProductionFloor,
                                    ProductionFloorName = (pf == null ? null : pf.StoreName),
                                    ScheduleYear = (ym == null ? null : ym.ScheduleYear),
                                    ScheduleMonth = (ym == null ? null : ym.ScheduleMonth),
                                    //ScheduleMonthName = (ym == null ? null : DalCommon.ReturnMonthName(ym.ScheduleMonth)),

                                    ScheduleMonthName = (ym == null ? null : ym.ScheduleMonth),


                                    ScheduleID = (Int64)(p.ScheduleID),
                                    ScheduleNo = (yms == null ? null : yms.ScheduleNo),
                                    ProductionProcessID = (yms == null ? null : yms.ProductionProcessID),
                                    ProductionProcessName = (pp == null ? null : pp.ProcessName),
                                    RecordStatus = p.RecordStatus,
                                    ScheduleDateID = (Int64)(p.ScheduleDateID),
                                    ProductionNo = p.CLProductionNo,
                                    //ScheduleStartDate = Convert.ToDateTime(p.CLProductionStartDate).ToString("dd'/'MM'/'yyyy"),
                                    //ScheduleEndDate = Convert.ToDateTime(p.CLProductionEndDate).ToString("dd'/'MM'/'yyyy"),

                                    TempScheduleStartDate = p.CLProductionStartDate,
                                    TempScheduleEndDate = p.CLProductionEndDate,
                                    
                                    ScheduleProductionNo = (pi == null ? null : pi.ScheduleProductionNo),
                                    CLProductionItemID = (pi == null ? 0 : pi.CLProductionItemID),
                                    ScheduleItemID = (Int64)(p.ScheduleItemID),
                                    ProductionArticleChallanID = (pi == null ? null : pi.ArticleChallanID),
                                    ProductionArticleChallanNo = (pi == null ? null : pi.ArticleChallanNo),
                                    SchedulePcs = (pi == null ? 0 : pi.ProductionPcs),
                                    ScheduleSide = (pi == null ? 0 : (Int64)pi.ProductionSide),
                                    ScheduleArea = (pi == null ? 0 : pi.ProductionArea),
                                    ScheduleAreaUnit = (pi == null ? 0 : pi.AreaUnit),
                                    ScheduleAreaUnitName = (pau == null ? "" : pau.UnitName),
                                    BuyerID = (pi == null ? null : pi.BuyerID),
                                    BuyerOrderID = (pi == null ? null : pi.BuyerOrderID),
                                    BuyerName = (b == null ? null : b.BuyerName),
                                    BuyerOrderNo = (ba == null ? null : ba.BuyerOrderNo),
                                    ItemTypeID = (pi == null ? null : pi.ItemTypeID),
                                    ItemTypeName = (it == null ? null : it.ItemTypeName),
                                    LeatherStatusID = (pi == null ? null : pi.LeatherStatusID),
                                    LeatherStatusName = (ls == null ? null : ls.LeatherStatusName),
                                    ProductionArticleID = (pi == null ? null : pi.ArticleID),
                                    ProductionArticleNo = (pi == null ? null : pi.ArticleNo),
                                    ProductionArticleName = (a == null ? null : a.ArticleName),
                                    CLProductionColorID = (c == null ? 0 : c.CLProductionColorID),
                                    ColorID = (c == null ? 0 : c.ColorID),
                                    ArticleColorNo = (c == null ? null : c.ArticleColorNo),
                                    ColorName = (cn == null ? null : cn.ColorName),
                                    ColorArea = (c == null ? null : c.ColorArea),
                                    AreaUnit = (c == null ? null : c.AreaUnit),
                                    AreaUnitName = (cau == null ? "" : cau.UnitName),
                                    ColorPCS = (c == null ? null : c.ColorPCS),
                                    ColorSide = (c == null ? null : c.ColorSide),
                                    ColorWeight = (c == null ? null : c.ColorWeight),
                                    WeightUnit = (c == null ? null : c.WeightUnit),
                                    WeightUnitName = (cwu == null ? "" : cwu.UnitName),
                                    Remarks = (c == null ? null : c.Remarks),

                                    RecipeID = (che == null ? 0 : (Int32)(che.RecipeID)),
                                    RecipeName = (rec == null ? null : rec.RecipeName),
                                    ArticleID = (rec == null ? null : rec.ArticleID),
                                    ArticleNo = (rec == null ? null : rec.ArticleNo),
                                    ArticleName = (rec == null ? null : rec.ArticleName),
                                    BaseQuantity = (rec == null ? 0 : rec.BaseQuantity),
                                    BaseUnit = (rec == null ? (byte)0 : rec.BaseUnit),
                                    BaseUnitName = (bu == null ? "" : bu.UnitName),
                                    RequirementOn = (che == null ? null : che.CalculationBase),
                                    RecipeChallanNo = (rec == null ? null : rec.ArticleChallanNo),
                                    ArticleColorName = (co == null ? null : co.ColorName),
                                }).AsParallel().FirstOrDefault();

                FormInfo.ScheduleEndDate = Convert.ToDateTime(FormInfo.TempScheduleEndDate).ToString("dd'/'MM'/'yyyy");
                FormInfo.ScheduleStartDate = Convert.ToDateTime(FormInfo.TempScheduleStartDate).ToString("dd'/'MM'/'yyyy");
                FormInfo.ScheduleMonthName = DalCommon.ReturnMonthName(FormInfo.ScheduleMonthName);

                model.FormInfo = FormInfo;

                model.LeatherList = GetLeatherListAfterSave(_CLProductionID);
                if (FormInfo.RequirementOn != "DR")
                model.ChemicalListForShow = GetChemicalListAfterSave(_CLProductionID, FormInfo.ProductionFloor, FormInfo.CalculationBase);


                return model;

            }
        }


        public bool DeleteProductionItem(string _CLProdChemicalID)
        {
            try
            {
                var ProductionItem = (from c in _context.PRD_CLProductionChemical.AsEnumerable()
                                      where (c.CLProdChemicalID).ToString() == _CLProdChemicalID
                                      select c).FirstOrDefault();

                _context.PRD_CLProductionChemical.Remove(ProductionItem);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public bool DeleteChemicalConsumption(string _CLProductionID)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    var ProductionInfo = (from p in _context.PRD_CLProduction.AsEnumerable()
                                          where p.CLProductionID.ToString() == _CLProductionID
                                          select p).FirstOrDefault();

                    var ProductionItemInfo = (from p in _context.PRD_CLProductionItem.AsEnumerable()
                                              where p.CLProductionID.ToString() == _CLProductionID
                                              select p).FirstOrDefault();

                    var ColorInfo = (from p in _context.PRD_CLProductionColor.AsEnumerable()
                                     where p.CLProductionItemID == ProductionItemInfo.CLProductionItemID
                                     select p).FirstOrDefault();

                    var DrumInfo = (from p in _context.PRD_CLProductionDrum.AsEnumerable()
                                    where p.CLProductionColorID == ColorInfo.CLProductionColorID
                                    select p).ToList();

                    var ChemicalInfo = (from p in _context.PRD_CLProductionChemical.AsEnumerable()
                                        where p.CLProductionID.ToString() == _CLProductionID
                                        select p).ToList();


                    _context.PRD_CLProductionChemical.RemoveRange(ChemicalInfo);
                    _context.PRD_CLProductionDrum.RemoveRange(DrumInfo);
                    _context.PRD_CLProductionColor.Remove(ColorInfo);
                    _context.PRD_CLProductionItem.Remove(ProductionItemInfo);
                    _context.PRD_CLProduction.Remove(ProductionInfo);

                    _context.SaveChanges();
                    transaction.Complete();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public bool CheckChemicalConsumtion(string _CLProductionID, string _ConfirmComment)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {
                        var TransactionInfo = (from p in _context.PRD_CLProduction.AsEnumerable()
                                               where (p.CLProductionID).ToString() == _CLProductionID
                                               select p).FirstOrDefault();
                        TransactionInfo.ConfirmeNote = _ConfirmComment;
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


        public ValidationMsg ConfirmChemicalConsumption(long _CLProductionID, string _ConfirmComment, int _UserID)
        {
            ValidationMsg Msg = new ValidationMsg();
            Msg.ReturnId = 1;
            Msg.Msg = "Confirmation Successful";
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (var context = new BLC_DEVEntities())
                    {
                        var ChemicalConsumptionInfo = (from p in context.PRD_CLProduction
                                                       where p.CLProductionID == _CLProductionID
                                                       select p).FirstOrDefault();

                        
                        var ChemicalConsumptionChemicalItemList = (from c in context.PRD_CLProductionChemical
                                                                   where c.CLProductionID == _CLProductionID
                                                                   select c).ToList();


                        foreach (var item in ChemicalConsumptionChemicalItemList)
                        {
                            #region Chemical Supplier Stock Update

                            var CheckSupplierStock = (from i in context.INV_ChemStockSupplier
                                                      where i.SupplierID == item.SupplierID && i.StoreID == ChemicalConsumptionInfo.ProductionFloor &&
                                                      i.ItemID == item.ItemID & i.UnitID == item.UseUnitID
                                                      select i).Any();
                            if (!CheckSupplierStock)
                            {
                                Msg.Msg = "Chemical Does Not Exist in Stock";
                                Msg.ReturnId = 0;
                                break;
                            }
                            else
                            {
                                var FoundItem = (from i in context.INV_ChemStockSupplier
                                                 where i.SupplierID == item.SupplierID && i.StoreID == ChemicalConsumptionInfo.ProductionFloor &&
                                                 i.ItemID == item.ItemID & i.UnitID == item.UseUnitID
                                                 orderby i.TransectionID descending
                                                 select i).FirstOrDefault();

                                if (FoundItem.ClosingQty >= item.UseQty)
                                {
                                    var NewItem = new INV_ChemStockSupplier();

                                    NewItem.SupplierID = Convert.ToInt16(item.SupplierID);
                                    NewItem.StoreID = Convert.ToByte(ChemicalConsumptionInfo.ProductionFloor);
                                    NewItem.ItemID = item.ItemID;
                                    NewItem.UnitID = item.UseUnitID;
                                    NewItem.OpeningQty = FoundItem.ClosingQty;
                                    NewItem.IssueQty = item.UseQty;
                                    NewItem.ReceiveQty = 0;
                                    NewItem.ClosingQty = FoundItem.ClosingQty - item.UseQty;
                                    NewItem.SetBy = _UserID;
                                    NewItem.SetOn = DateTime.Now.Date;
                                    NewItem.Remark = "Consumed in Wet Blue Production";

                                    context.INV_ChemStockSupplier.Add(NewItem);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    Msg.Msg = "Not Enough Chemical in Stock";
                                    Msg.ReturnId = 0;
                                    break;
                                }

                            }
                            #endregion

                            #region Chemical Item Stock Update
                            var CheckItemStock = (from i in context.INV_ChemStockItem
                                                  where i.StoreID == ChemicalConsumptionInfo.ProductionFloor &&
                                                  i.ItemID == item.ItemID & i.UnitID == item.UseUnitID
                                                  select i).Any();

                            if (!CheckItemStock)
                            {
                                Msg.Msg = "Chemical Does Not Exist in Stock";
                                Msg.ReturnId = 0;
                                break;
                            }
                            else
                            {
                                var FoundItem = (from i in context.INV_ChemStockItem
                                                 where i.StoreID == ChemicalConsumptionInfo.ProductionFloor &&
                                                 i.ItemID == item.ItemID & i.UnitID == item.UseUnitID
                                                 orderby i.TransectionID descending
                                                 select i).FirstOrDefault();

                                if (FoundItem.ClosingQty >= item.UseQty)
                                {
                                    var NewItem = new INV_ChemStockItem();

                                    NewItem.StoreID = Convert.ToByte(ChemicalConsumptionInfo.ProductionFloor);
                                    NewItem.ItemID = item.ItemID;
                                    NewItem.UnitID = item.UseUnitID;
                                    NewItem.OpeningQty = FoundItem.ClosingQty;
                                    NewItem.IssueQty = item.UseQty;
                                    NewItem.ReceiveQty = 0;
                                    NewItem.ClosingQty = FoundItem.ClosingQty - item.UseQty;
                                    NewItem.SetBy = _UserID;
                                    NewItem.SetOn = DateTime.Now.Date;
                                    NewItem.Remark = "Consumed in Wet Blue Production";

                                    context.INV_ChemStockItem.Add(NewItem);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    Msg.Msg = "Not Enough Chemical in Stock";
                                    Msg.ReturnId = 0;
                                    break;
                                }

                            }

                            #endregion

                            #region Chemical Daily Stock Update
                            var currentDate = DateTime.Now.Date;
                            var CheckDailyStock = (from i in context.INV_ChemStockDaily
                                                   where i.StockDate == currentDate && i.StoreID == ChemicalConsumptionInfo.ProductionFloor &&
                                                   i.ItemID == item.ItemID & i.UnitID == item.UseUnitID
                                                   select i).Any();

                            if (CheckDailyStock)
                            {
                                var CurrentItem = (from i in context.INV_ChemStockDaily
                                                   where i.StockDate == currentDate && i.StoreID == ChemicalConsumptionInfo.ProductionFloor &&
                                                   i.ItemID == item.ItemID & i.UnitID == item.UseUnitID
                                                   select i).FirstOrDefault();

                                if (CurrentItem.ClosingQty >= item.UseQty)
                                {
                                    CurrentItem.IssueQty = CurrentItem.IssueQty + item.UseQty;
                                    CurrentItem.ClosingQty = CurrentItem.ClosingQty - item.UseQty;
                                    CurrentItem.Remark = "Consumed in Wet Blue Production";
                                    context.SaveChanges();
                                }
                                else
                                {
                                    Msg.Msg = "Not Enough Chemical in Stock";
                                    Msg.ReturnId = 0;
                                    break;
                                }


                            }
                            else
                            {
                                var FoundItem = (from i in context.INV_ChemStockDaily
                                                 where i.StoreID == ChemicalConsumptionInfo.ProductionFloor &&
                                                 i.ItemID == item.ItemID & i.UnitID == item.UseUnitID
                                                 orderby i.TransectionID descending
                                                 select i).FirstOrDefault();
                                if (FoundItem != null)
                                {
                                    if (FoundItem.ClosingQty >= item.UseQty)
                                    {
                                        var NewItem = new INV_ChemStockDaily();

                                        NewItem.StockDate = DateTime.Now.Date;
                                        NewItem.StoreID = Convert.ToByte(ChemicalConsumptionInfo.ProductionFloor);
                                        NewItem.ItemID = item.ItemID;
                                        NewItem.UnitID = item.UseUnitID;
                                        NewItem.OpeningQty = FoundItem.ClosingQty;
                                        NewItem.IssueQty = item.UseQty;
                                        NewItem.ReceiveQty = 0;
                                        NewItem.ClosingQty = FoundItem.ClosingQty - item.UseQty;
                                        NewItem.SetOn = DateTime.Now.Date;
                                        NewItem.SetBy = _UserID;
                                        NewItem.Remark = "Consumed in Wet Blue Production";

                                        context.INV_ChemStockDaily.Add(NewItem);
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        Msg.Msg = "Not Enough Chemical in Stock";
                                        Msg.ReturnId = 0;
                                        break;
                                    }

                                }
                                else
                                {
                                    Msg.Msg = "Chemical Does Not Exist in Stock";
                                    Msg.ReturnId = 0;
                                    break;
                                }


                            }
                            #endregion

                        }
                        if (Msg.ReturnId == 1)
                        {
                            ChemicalConsumptionInfo.ConfirmeNote = _ConfirmComment;

                            ChemicalConsumptionInfo.RecordStatus = "CNF";

                            context.SaveChanges();
                            Transaction.Complete();
                        }
                    }
                    
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


        public List<PRDChemProdReqItem> GetChemicalItemListForFixedLeather(long _CLProductionID, long _CLProductionDrumID, byte? _StoreID)
        {
            using (var context = new BLC_DEVEntities())
            {

                //var StockResult = (from p in context.INV_ChemStockSupplier
                //                   where p.StoreID == _StoreID
                //                   group p by new
                //                   {
                //                       p.SupplierID,
                //                       p.ItemID,
                //                       p.UnitID
                //                   } into g
                //                   select new
                //                   {
                //                       TransectionID = g.Max(p => p.TransectionID),
                //                       StoreID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.StoreID).FirstOrDefault(),
                //                       ItemID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ItemID).FirstOrDefault(),
                //                       SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                //                       ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault(),
                //                       StockUnit = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault()
                //                   });

                var StockResult = DalChemicalStock.CombinationWiseStockInSpecificProductionFloor(Convert.ToByte(_StoreID));

                var Data = (from c in context.PRD_CLProductionChemical.AsEnumerable()
                            where c.CLProductionID == _CLProductionID & c.CLProductionDrumID == _CLProductionDrumID

                            join i in context.Sys_ChemicalItem on c.ItemID equals i.ItemID into ChemicalItem
                            from i2 in ChemicalItem.DefaultIfEmpty()


                            join ru in context.Sys_Unit on c.RequiredUnitID equals ru.UnitID into RequiredUnit
                            from ru2 in RequiredUnit.DefaultIfEmpty()

                            join uu in context.Sys_Unit on c.UseUnitID equals uu.UnitID into UseUnit
                            from uu2 in UseUnit.DefaultIfEmpty()

                            join sup in context.Sys_Supplier on c.SupplierID equals sup.SupplierID into SupplierInfo
                            from sup2 in SupplierInfo.DefaultIfEmpty()

                            join s in StockResult on new { ItemID = c.ItemID, SupplierID = c.SupplierID, UnitID = c.UseUnitID } equals
                            new { ItemID = s.ItemID, SupplierID = s.SupplierID, UnitID = s.UnitID } into StockInfo
                            from item in StockInfo.DefaultIfEmpty()

                            join StockUnit in context.Sys_Unit on c.UseUnitID equals StockUnit.UnitID into StockUnits
                            from StockUnit in StockUnits.DefaultIfEmpty()

                            orderby i2.ItemName

                            select new PRDChemProdReqItem
                            {
                                CLProdChemicalID = c.CLProdChemicalID,
                                CLProductionDrumID = (c.CLProductionDrumID),
                                ItemID = (c.ItemID),
                                ItemName = (i2 == null ? null : i2.ItemName),
                                RequiredQty = (c.RequiredQty),
                                RequiredUnit = (c.RequiredUnitID),
                                RequiredUnitName = (ru2 == null ? null : ru2.UnitName),
                                RequsitionQty = (c.UseQty),
                                RequisitionUnit = (c.UseUnitID),
                                RequisitionUnitName = (uu2 == null ? null : uu2.UnitName),
                                SupplierID = (c.SupplierID),
                                SupplierName = (sup2 == null ? null : sup2.SupplierName),
                                StockQty = (item == null ? 0 : (item.ClosingQty)),
                                StockUnit = (item == null ? null : (item.UnitID)),
                                StockUnitName = (StockUnit == null ? "" : StockUnit.UnitName)
                            }).ToList();

                return Data;
            }
        }


        

    }
}
