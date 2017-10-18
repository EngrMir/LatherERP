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
    public class DalPrdChemConsumption
    {
        private readonly BLC_DEVEntities _context;

        public DalPrdChemConsumption()
        {
            _context = new BLC_DEVEntities();
        }

        // For LOV
        public List<PrdYearMonthSchedule> GetScheduleInformation()
        {
            using(_context)
            {
                var Data = (from s in _context.PRD_YearMonthSchedule.AsEnumerable()
                            join y in _context.PRD_YearMonth on s.YearMonID equals y.YearMonID into YearMonthList
                            from ym in YearMonthList.DefaultIfEmpty()
                            where ym.ScheduleFor == "WBP"

                            join st in _context.SYS_Store on (ym == null ? null : ym.ProductionFloor) equals st.StoreID into StoreList
                            from stf in StoreList.DefaultIfEmpty()

                            join p in _context.Sys_ProductionProces on s.ProductionProcessID equals p.ProcessID into Process
                            from p2 in Process.DefaultIfEmpty()
                            select new PrdYearMonthSchedule
                            {
                                ScheduleID = s.ScheduleID,
                                ScheduleNo = s.ScheduleNo,
                                PrepareDate = (Convert.ToDateTime(s.PrepareDate)).ToString("dd'/'MM'/'yyyy"),
                                ScheduleYear = (ym == null ? null : ym.ScheduleYear),
                                ScheduleMonth = (ym == null ? null : DalCommon.ReturnMonthName(ym.ScheduleMonth)),
                                ProductionFloor = (ym == null ? null : ym.ProductionFloor),
                                ProductionFloorName = (stf == null ? null : stf.StoreName),
                                ProductionProcessID= s.ProductionProcessID,
                                ProductionProcessName= (p2== null? null: p2.ProcessName)
                            }).ToList();
                return Data;
            }
            
        }

        // For Search
        public List<PrdWBProduction> GetScheduleInformationForSearch()
        {
            using (_context)
            {
                var Data = (from s in _context.PRD_WBProduction.AsEnumerable()
                            //where 

                            join ymsd in _context.PRD_YearMonthScheduleDate on s.ScheduleDateID equals ymsd.ScheduleDateID into ScheduleDate
                            from ymsd2 in ScheduleDate.DefaultIfEmpty()

                            join yms in _context.PRD_YearMonthSchedule on ymsd2.ScheduleID equals yms.ScheduleID into Schedule
                            from yms2 in Schedule.DefaultIfEmpty()

                            join ym in _context.PRD_YearMonth on yms2.YearMonID equals ym.YearMonID into YearMonthList
                            from ym2 in YearMonthList.DefaultIfEmpty()
                            where ym2.ScheduleFor == "WBP"

                            join st in _context.SYS_Store on (ym2 == null ? null : ym2.ProductionFloor) equals st.StoreID into StoreList
                            from stf in StoreList.DefaultIfEmpty()

                            join pp in _context.Sys_ProductionProces on s.ProductionProcessID equals pp.ProcessID into ProductionProcess
                            from pp in ProductionProcess.DefaultIfEmpty()

                            orderby s.WBProductionID descending
                            select new PrdWBProduction
                            {
                                WBProductionID = s.WBProductionID,
                                ProductionNo = s.ProductionNo,
                                WBProductionStartDate = (Convert.ToDateTime(s.WBProductionStartDate)).ToString("dd'/'MM'/'yyyy"),
                                ProductionProcessID = s.ProductionProcessID,
                                ProductionProcessName = pp == null ? null : pp.ProcessName,
                                ScheduleNo = yms2.ScheduleNo,
                                ScheduleYear = (ym2 == null ? null : ym2.ScheduleYear),
                                ScheduleMonth = (ym2 == null ? null : DalCommon.ReturnMonthName(ym2.ScheduleMonth)),
                                ProductionFloor = (ym2 == null ? null : ym2.ProductionFloor),
                                ProductionFloorName = (stf == null ? null : stf.StoreName),
                                RecordStatus = DalCommon.ReturnRecordStatus(s.RecordStatus)
                            }).ToList();
                return Data;
            }

        }


        public PrdWBProduction GetDetailsSearchInformaiton(long _WBProductionID)
        {
            using(_context)
            {
                PrdWBProduction model = new PrdWBProduction();

                var FormInfo = (from s in _context.PRD_WBProduction.AsEnumerable()
                                where s.WBProductionID == _WBProductionID

                                join ymsd in _context.PRD_YearMonthScheduleDate on s.ScheduleDateID equals ymsd.ScheduleDateID into ScheduleDate
                                from ymsd2 in ScheduleDate.DefaultIfEmpty()

                                join yms in _context.PRD_YearMonthSchedule on ymsd2.ScheduleID equals yms.ScheduleID into Schedule
                                from yms2 in Schedule.DefaultIfEmpty()

                                join ym in _context.PRD_YearMonth on yms2.YearMonID equals ym.YearMonID into YearMonthList
                                from ym2 in YearMonthList.DefaultIfEmpty()
                                where ym2.ScheduleFor == "WBP"

                                join st in _context.SYS_Store on (ym2 == null ? null : ym2.ProductionFloor) equals st.StoreID into StoreList
                                from st2 in StoreList.DefaultIfEmpty()

                                join swu in _context.Sys_Unit on s.WBWeightUnit equals swu.UnitID into WeightUnit
                                from swu2 in WeightUnit.DefaultIfEmpty()

                                join a in _context.Sys_Article on s.ArticleID equals a.ArticleID into Article
                                from a2 in Article.DefaultIfEmpty()

                                join c in _context.Sys_Color on (a2 == null ? 0 : a2.ArticleColor) equals c.ColorID into Color
                                from c2 in Color.DefaultIfEmpty()

                                join r in _context.PRD_Recipe on s.RecipeID equals r.RecipeID into Recipe
                                from r2 in Recipe.DefaultIfEmpty()

                                join wu in _context.Sys_Unit on (r2 == null ? 0 : r2.BaseUnit) equals wu.UnitID into JustWeightUnit
                                from wu2 in JustWeightUnit.DefaultIfEmpty()

                                join wbpc in _context.PRD_WBProductionChemical on s.WBProductionID equals wbpc.WBProductionID into Chemical
                                from wbpc2 in Chemical.DefaultIfEmpty()

                                join pp in _context.Sys_ProductionProces on s.ProductionProcessID equals pp.ProcessID into ProductionProcess
                                from pp in ProductionProcess.DefaultIfEmpty()

                                select new PrdWBProduction
                                {
                                    WBProductionID = s.WBProductionID,
                                    ProductionNo = s.ProductionNo,
                                    ScheduleStartDate = (Convert.ToDateTime(s.WBProductionStartDate)).ToString("dd'/'MM'/'yyyy"),
                                    ScheduleEndDate = (Convert.ToDateTime(s.WBProductionEndDate)).ToString("dd'/'MM'/'yyyy"),
                                    ScheduleFor = "Wet Blue Production",
                                    ScheduleNo = yms2.ScheduleNo,
                                    ScheduleYear = (ym2 == null ? null : ym2.ScheduleYear),
                                    ScheduleMonth = (ym2 == null ? null : DalCommon.ReturnMonthName(ym2.ScheduleMonth)),
                                    ProductionFloor = (ym2 == null ? null : ym2.ProductionFloor),
                                    ProductionFloorName = (st2 == null ? null : st2.StoreName),
                                    PrepareDate = (yms2 == null ? null : (Convert.ToDateTime(yms2.PrepareDate)).ToString("dd'/'MM'/'yyyy")),

                                    SchedulePcs = s.WBProductionPcs,
                                    ScheduleSide = s.WBProductionSide,
                                    ScheduleWeight = s.WBProductionWeight,
                                    ScheduleWeightUnit = s.WBWeightUnit,
                                    ScheduleWeightUnitName = (swu2 == null ? null : swu2.UnitName),
                                    ProductionStatus = (ymsd2 == null ? null : DalCommon.ReturnProductionStatus(ymsd2.ProductionStatus)),
                                    Remark = (ymsd2 == null ? null : ymsd2.Remark),
                                    ProductionProcessID = s.ProductionProcessID,
                                    ProductionProcessName = (pp == null ? null : pp.ProcessName),
                                    RecipeID = s.RecipeID,
                                    RecipeName = (r2 == null ? null : r2.RecipeName),
                                    RecipeChallanNo = s.RecipeChallanNo,
                                    ArticleID = s.ArticleID,
                                    ArticleNo = s.ArticleNo,
                                    ArticleName = (a2 == null ? null : a2.ArticleName),
                                    ArticleColorName = (c2 == null ? null : c2.ColorName),
                                    BaseQuantity = (r2 == null ? 0 : r2.BaseQuantity),
                                    BaseUnitName = (wu2 == null ? null : wu2.UnitName),
                                    RequirementOn = (wbpc2 == null ? null : wbpc2.CalculationBase),
                                    RecordStatus = DalCommon.ReturnRecordStatus(s.RecordStatus),
                                    ConfirmeNote = s.ConfirmeNote
                                }).FirstOrDefault();

                model.FormInfo = FormInfo;

                model.LeatherList = GetLeatherListAfterSave(_WBProductionID);
                if(FormInfo.RequirementOn != "DR")
                    model.ChemicalListForShow = GetChemicalListAfterSave(_WBProductionID, FormInfo.ProductionFloor);


                return model;

            }
        }

        // For LOV
        public List<PrdYearMonthScheduleDate> GetProductionInformation(string _ScheduleID)
        {
            using (_context)
            {
                var Data = (from s in _context.PRD_YearMonthScheduleDate.AsEnumerable()
                            where (s.ScheduleID).ToString() == _ScheduleID

                            join u in _context.Sys_Unit on s.ScheduleWeightUnit equals u.UnitID into UnitName
                            from un in UnitName.DefaultIfEmpty()

                            select new PrdYearMonthScheduleDate
                            {
                                ScheduleDateID = s.ScheduleDateID,
                                SchedulePcs = s.SchedulePcs,
                                ScheduleSide = s.ScheduleSide,
                                ScheduleStartDate = (Convert.ToDateTime(s.ScheduleStartDate)).ToString("dd'/'MM'/'yyyy"),
                                ScheduleEndDate = (Convert.ToDateTime(s.ScheduleEndDate)).ToString("dd'/'MM'/'yyyy"),
                                ScheduleWeight = s.ScheduleWeight,
                                ScheduleWeightUnit = s.ScheduleWeightUnit,
                                ScheduleWeightUnitName = (un == null ? null : un.UnitName),
                                ProductionStatus = DalCommon.ReturnProductionStatus(s.ProductionStatus),
                                ProductionNo = s.ProductionNo,
                                Remark = s.Remark,
                            }).ToList();
                return Data;
            }

        }

        // After LOV
        public List<PrdYearMonthSchedulePurchase> GetProductionDetails(string _ScheduleDateID)
        {
            using (_context)
            {
                var ScheduleID = _context.PRD_YearMonthScheduleDate.Where(x => (x.ScheduleDateID).ToString() == _ScheduleDateID).Select(x => x.ScheduleID).FirstOrDefault();
                var Data = (from s in _context.PRD_YearMonthSchedulePurchase.AsEnumerable()
                            where (s.ScheduleDateID).ToString() == _ScheduleDateID

                            join sup in _context.Sys_Supplier on s.SupplierID equals sup.SupplierID into SupplierInfo
                            from sup2 in SupplierInfo.DefaultIfEmpty()

                            join i in _context.Sys_ItemType on s.ItemTypeID equals i.ItemTypeID into ItemType
                            from it in ItemType.DefaultIfEmpty()

                            join ls in _context.Sys_LeatherStatus on s.LeatherStatusID equals ls.LeatherStatusID into LeatherStatus
                            from ls2 in LeatherStatus.DefaultIfEmpty()

                            join lt in _context.Sys_LeatherType on s.LeatherTypeID equals lt.LeatherTypeID into LeatherType
                            from lt2 in LeatherType.DefaultIfEmpty()

                            join u in _context.Sys_Unit on s.WeightUnit equals u.UnitID into Unit
                            from u2 in Unit.DefaultIfEmpty()

                            join p in _context.Prq_Purchase on s.PurchaseID equals p.PurchaseID into Purchase
                            from p2 in Purchase.DefaultIfEmpty()

                            select new PrdYearMonthSchedulePurchase
                            {
                                SchedulePurchaseID= s.SchedulePurchaseID,
                                ScheduleDateID = s.ScheduleDateID,
                                ProductionNo= s.ProductionNo,
                                MachineID= s.MachineID,
                                MachineNo= s.MachineNo,
                                SupplierID= s.SupplierID,
                                SupplierName= (sup2==null? null: sup2.SupplierName),
                                PurchaseID= s.PurchaseID,
                                PurchaseNo= (p2==null? null: p2.PurchaseNo),
                                ItemTypeID= s.ItemTypeID,
                                ItemTypeName= (it==null? null: it.ItemTypeName),
                                LeatherStatusID= s.LeatherStatusID,
                                LeatherStatusName = (ls2 == null ? null : ls2.LeatherStatusName),
                                LeatherTypeID= s.LeatherTypeID,
                                LeatherTypeName = (lt2 == null ? null : lt2.LeatherTypeName),
                                ProductionPcs= s.ProductionPcs,
                                ProductionSide= s.ProductionSide,
                                ProductionWeight= s.ProductionWeight,
                                WeightUnit= s.WeightUnit,
                                UnitName= (u2==null?null:u2.UnitName),
                                Remark = s.Remark,
                            }).ToList();
                return Data;
            }

        }

        public List<Sys_ProductionProces> GetProductionProcessList()
        {
            var Data = (from p in _context.Sys_ProductionProces.AsEnumerable()
                        where p.ProcessCategory == "WB"
                        select new Sys_ProductionProces
                        {
                            ProcessID = p.ProcessID,
                            ProcessName = p.ProcessName
                        }).ToList();

            return Data;
        }


        public List<PrdRecipe> GetRecipeForFixedProcess(int _RecipeFor)
        {
            var Data = (from s in _context.PRD_Recipe.AsEnumerable()
                        where s.RecipeFor == _RecipeFor & s.RecordStatus=="APV"

                        from c in _context.Sys_Color.Where(x => x.ColorID == s.ArticleColor).DefaultIfEmpty()
                        from u in _context.Sys_Unit.Where(x => x.UnitID == s.BaseUnit).DefaultIfEmpty()
                        join a in _context.Sys_Article on s.ArticleID equals a.ArticleID into Article
                        from a2 in Article.DefaultIfEmpty()
                        select new PrdRecipe
                        {
                            RecipeID = s.RecipeID,
                            RecipeNo= s.RecipeNo,
                            RecipeName= s.RecipeName,
                            ArticleID= (a2==null? 0:(a2.ArticleID)),
                            ArticleNo = s.ArticleNo,
                            ArticleName = s.ArticleName,
                            ArticleChallanNo = s.ArticleChallanNo,
                            ArticleColor = s.ArticleColor,
                            ArticleColorName = (c == null ? null : c.ColorName),
                            BaseQuantity = s.BaseQuantity,
                            BaseUnit = s.BaseUnit,
                            BaseUnitName = (u == null ? null : u.UnitName)
                        }).ToList();

            return Data;
        }

        // For Recipe LOV
        public List<PRDChemProdReqItem> GetRecipeItemListForFixedRecipe(int _RecipeID, int _StoreID, decimal? _Weight, decimal? _Area)
        {
            decimal _Factor = 0;
            var RecipeDetails = (from r in _context.PRD_Recipe
                                 where r.RecipeID == _RecipeID
                                 select r).FirstOrDefault();

            if (RecipeDetails.CalculationBase == "WT")
            {
                _Factor = decimal.Round(Convert.ToDecimal(_Weight / RecipeDetails.BaseQuantity),2);
            }
            else if (RecipeDetails.CalculationBase == "AR")
            {
                _Factor = decimal.Round(Convert.ToDecimal(_Area / RecipeDetails.BaseQuantity), 2);
            }

            //Speciall Case: If a chemical has multiple supplier, then only that chemical will be 
            //shown only once with a default supplier. That's why supplier is not in group by list.
            var StockResult = (from p in _context.INV_ChemStockSupplier
                               where p.StoreID == _StoreID 
                               group p by new
                               {
                                   //p.SupplierID
                                   p.ItemID,
                                   p.UnitID
                               } into g
                               select new
                               {
                                   TransectionID = g.Max(p => p.TransectionID),
                                   StoreID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.StoreID).FirstOrDefault(),
                                   ItemID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ItemID).FirstOrDefault(),
                                   SupplierID = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.SupplierID).FirstOrDefault(),
                                   ClosingQty = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.ClosingQty).FirstOrDefault(),
                                   StockUnit = g.Where(p => p.TransectionID == g.Max(q => q.TransectionID)).Select(x => x.UnitID).FirstOrDefault()
                               });

            if (StockResult.ToList().Count > 0)
            {
                var Data = (from s in _context.PRD_RecipeItem
                            where s.RecipeID == _RecipeID

                            from i in _context.Sys_ChemicalItem.Where(x => x.ItemID == s.ItemID).DefaultIfEmpty()
                            from u in _context.Sys_Unit.Where(x => x.UnitID == s.UnitID).DefaultIfEmpty()


                            join fr in StockResult on s.ItemID equals fr.ItemID into FinalStocks
                            from item in FinalStocks.DefaultIfEmpty()

                            
                            join sup in _context.Sys_Supplier on (item == null ? 0 : item.SupplierID) equals sup.SupplierID into Suppliers
                            from finalitem in Suppliers.DefaultIfEmpty()

                            join su in _context.Sys_Unit on (item==null? null: item.StockUnit) equals su.UnitID into StockUnits
                            from su in StockUnits.DefaultIfEmpty()

                            orderby i.ItemName

                            select new PRDChemProdReqItem
                            {
                                ItemID = s.ItemID,
                                ItemName = (i == null ? null : i.ItemName),
                                RequiredQty = s.RequiredQty,
                                RequiredUnit = s.UnitID,
                                RequiredUnitName = (u == null ? null : u.UnitName),
                                RequsitionQty = s.RequiredQty,
                                PackSizeName = "",
                                PackQty = 0,
                                SizeUnitName = "",
                                RequisitionUnit = s.UnitID,
                                RequisitionUnitName = (u == null ? null : u.UnitName),

                                StockQty = (item == null ? 0 : item.ClosingQty),
                                StockUnitName = (su == null ? null : su.UnitName),
                                SupplierID = (finalitem == null ? 0 : finalitem.SupplierID),
                                SupplierName = (finalitem == null ? null : finalitem.SupplierName),

                                ItemSource = "Via Requisition"
                            }).ToList();
                

                foreach (var item in Data)
                {
                    var Qty = Decimal.Round(Convert.ToDecimal(item.RequiredQty) * (_Factor), 2);
                    item.RequiredQty = Qty;
                    item.RequsitionQty = Qty;
                    item.ApproveQty = Qty;
                }

                return Data;
            }
            else
            {
                var Data = (from s in _context.PRD_RecipeItem
                            where s.RecipeID == _RecipeID
                            from i in _context.Sys_ChemicalItem.Where(x => x.ItemID == s.ItemID).DefaultIfEmpty()
                            from u in _context.Sys_Unit.Where(x => x.UnitID == s.UnitID).DefaultIfEmpty()

                            orderby i.ItemName
                            select new PRDChemProdReqItem
                            {
                                ItemID = s.ItemID,
                                ItemName = (i == null ? null : i.ItemName),
                                RequiredQty = s.RequiredQty,
                                RequiredUnit = s.UnitID,
                                RequiredUnitName = (u == null ? null : u.UnitName),
                                RequsitionQty = s.RequiredQty,
                                PackSizeName = "",
                                PackQty = 0,
                                SizeUnitName = "",
                                RequisitionUnit = s.UnitID,
                                RequisitionUnitName = (u == null ? null : u.UnitName),
                                StockQty = 0,
                                ItemSource = "Via Requisition"
                            }).ToList();

                foreach (var item in Data)
                {
                    var Qty = Decimal.Round(Convert.ToDecimal(item.RequiredQty) * (_Factor), 2);
                    item.RequiredQty = Qty;
                    item.RequsitionQty = Qty;
                    item.ApproveQty = Qty;
                }

                return Data;
            }

        }



        public long Save(PrdWBProduction model, int userId, string pageURL)
        {

            long CurrentWBProductionID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    #region New_Production_Insert
                    using (_context)
                    {
                        PRD_WBProduction objProduction = new PRD_WBProduction();
                        
                        objProduction.WBProductionNo = DalCommon.GetPreDefineNextCodeByUrl("ChemicalConsumption/ChemConsumption");
                        objProduction.ProductionFor = "WB";
                        objProduction.ProductionNo = model.ProductionNo;
                        objProduction.ProductionFloor = model.ProductionFloor;
                        objProduction.ScheduleDateID = model.ScheduleDateID;
                        objProduction.WBProductionStartDate = DalCommon.SetDate(model.WBProductionStartDate);
                        objProduction.WBProductionEndDate = DalCommon.SetDate(model.WBProductionEndDate);
                        objProduction.WBProductionPcs = model.WBProductionPcs;
                        objProduction.WBProductionSide = model.WBProductionSide;
                        objProduction.WBProductionWeight = model.WBProductionWeight;
                        if (model.WBWeightUnit == 0)
                            objProduction.WBWeightUnit = null;
                        else
                            objProduction.WBWeightUnit = model.WBWeightUnit;

                        objProduction.RecipeID = model.RecipeID;

                        if (model.ArticleID == 0)
                            objProduction.ArticleID = null;
                        else
                            objProduction.ArticleID = model.ArticleID;

                        objProduction.ArticleNo = model.ArticleNo;
                        objProduction.RecipeChallanNo = model.RecipeChallanNo;
                        objProduction.ProductionProcessID = model.ProductionProcessID;


                        objProduction.RecordStatus = "NCF";
                        objProduction.SetOn = DateTime.Now;
                        objProduction.SetBy = userId;

                        _context.PRD_WBProduction.Add(objProduction);
                        _context.SaveChanges();

                        CurrentWBProductionID = objProduction.WBProductionID;


                        #region Leather Insert
                        if (model.LeatherList != null)
                        {
                            foreach (var LeatherItem in model.LeatherList)
                            {
                                PRD_WBProductionPurchase objLeather = new PRD_WBProductionPurchase();

                                objLeather.WBProductionID = CurrentWBProductionID;
                                objLeather.SchedulePurchaseID = LeatherItem.SchedulePurchaseID;
                                objLeather.ScheduleDateID = model.ScheduleDateID;
                                objLeather.ProductionNo = model.ProductionNo;
                                objLeather.MachineID = LeatherItem.MachineID;
                                objLeather.MachineNo = LeatherItem.MachineNo;
                                objLeather.SupplierID = LeatherItem.SupplierID;
                                objLeather.PurchaseID = LeatherItem.PurchaseID;
                                objLeather.ItemTypeID = DalCommon.GetItemTypeCode(LeatherItem.ItemTypeName);
                                objLeather.LeatherStatusID = DalCommon.GetLeatherStatusCode(LeatherItem.LeatherStatusName);
                                objLeather.LeatherTypeID = DalCommon.GetLeatherTypeCode(LeatherItem.LeatherTypeName);
                                objLeather.WBProductionPcs = LeatherItem.WBProductionPcs;
                                objLeather.WBProductionSide = LeatherItem.WBProductionSide;
                                objLeather.WBProductionWeight = LeatherItem.WBProductionWeight;

                                if (LeatherItem.UnitName == null)
                                    objLeather.WeightUnit = null;
                                else
                                    objLeather.WeightUnit = DalCommon.GetUnitCode(LeatherItem.UnitName);

                                objLeather.RecipeID = model.RecipeID;


                                if (model.ArticleID == 0)
                                    objLeather.ArticleID = null;
                                else
                                    objLeather.ArticleID = model.ArticleID;
                                
                                objLeather.ArticleNo = model.ArticleNo;
                                objLeather.RecipeChallanNo = model.RecipeChallanNo;
                                objLeather.ProductionProcessID = model.ProductionProcessID;
                                objLeather.Remarks = LeatherItem.Remarks;
                                objLeather.SetBy = userId;
                                objLeather.SetOn = DateTime.Now;
                                _context.PRD_WBProductionPurchase.Add(objLeather);
                                _context.SaveChanges();
                            }

                        }
                        #endregion

                        #region Chemical Insert
                        if (model.ChemicalList != null)
                        {
                            var CheckWBProductionPurchaseID = (from wbp in _context.PRD_WBProductionPurchase
                                                               where wbp.SchedulePurchaseID == model.ParentID &
                                                               wbp.WBProductionID == CurrentWBProductionID
                                                               select wbp.WBProductionPurchaseID).FirstOrDefault();

                            foreach (var ChemicalItem in model.ChemicalList)
                            {

                                PRD_WBProductionChemical objChemical = new PRD_WBProductionChemical();

                                

                                if (CheckWBProductionPurchaseID == 0)
                                    objChemical.WBProductionPurchaseID = null;
                                else
                                    objChemical.WBProductionPurchaseID = CheckWBProductionPurchaseID;

                                objChemical.WBProductionID = CurrentWBProductionID;

                                if (ChemicalItem.SchedulePurchaseID == 0)
                                    objChemical.SchedulePurchaseID = null;
                                else
                                    objChemical.SchedulePurchaseID = ChemicalItem.SchedulePurchaseID;

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

                                _context.PRD_WBProductionChemical.Add(objChemical);
                                //_context.SaveChanges();
                            }
                        }
                        #endregion

                        _context.SaveChanges();
                    }
                    #endregion

                    
                    transaction.Complete();
                }
                return CurrentWBProductionID;
            }
            catch (Exception e)
            {
                return 0;
            }

        }


        public int Update(PrdWBProduction model, int userId)
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
                                if(ChemicalItem.WBProdChemicalID==0)
                                {
                                    PRD_WBProductionChemical objChemical = new PRD_WBProductionChemical();

                                    var CheckWBProductionPurchaseID = (from wbp in _context.PRD_WBProductionPurchase
                                                                       where wbp.SchedulePurchaseID == ChemicalItem.SchedulePurchaseID &
                                                                       wbp.WBProductionID == model.WBProductionID
                                                                       select wbp.WBProductionPurchaseID).FirstOrDefault();

                                    if (CheckWBProductionPurchaseID == 0)
                                        objChemical.WBProductionPurchaseID = null;
                                    else
                                        objChemical.WBProductionPurchaseID = CheckWBProductionPurchaseID;

                                    objChemical.WBProductionID = model.WBProductionID;

                                    if (ChemicalItem.SchedulePurchaseID == 0)
                                        objChemical.SchedulePurchaseID = null;
                                    else
                                        objChemical.SchedulePurchaseID = ChemicalItem.SchedulePurchaseID;

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

                                    _context.PRD_WBProductionChemical.Add(objChemical);
                                   // _context.SaveChanges();
                                }
                                else
                                {
                                    #region Update_Existing_Chemical_Item

                                    var CurrentChemicalItem = (from c in _context.PRD_WBProductionChemical
                                                               where c.WBProdChemicalID == ChemicalItem.WBProdChemicalID
                                                               select c).FirstOrDefault();

                                    if (ChemicalItem.SchedulePurchaseID == 0)
                                        CurrentChemicalItem.SchedulePurchaseID = null;
                                    else
                                        CurrentChemicalItem.SchedulePurchaseID = ChemicalItem.SchedulePurchaseID;

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
                                    //_context.SaveChanges();
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

        public List<PrdWBProductionPurchase> GetLeatherListAfterSave(long WBProductionID)
        {
            using(var context= new BLC_DEVEntities())
            {
                var Data = (from l in context.PRD_WBProductionPurchase
                            where l.WBProductionID == WBProductionID

                            join sup in context.Sys_Supplier on l.SupplierID equals sup.SupplierID into SupplierInfo
                            from sup2 in SupplierInfo.DefaultIfEmpty()
                            join i in context.Sys_ItemType on l.ItemTypeID equals i.ItemTypeID into ItemType
                            from it in ItemType.DefaultIfEmpty()
                            join ls in context.Sys_LeatherStatus on l.LeatherStatusID equals ls.LeatherStatusID into LeatherStatus
                            from ls2 in LeatherStatus.DefaultIfEmpty()
                            join lt in context.Sys_LeatherType on l.LeatherTypeID equals lt.LeatherTypeID into LeatherType
                            from lt2 in LeatherType.DefaultIfEmpty()
                            join u in context.Sys_Unit on l.WeightUnit equals u.UnitID into Unit
                            from u2 in Unit.DefaultIfEmpty()
                            join p in context.Prq_Purchase on l.PurchaseID equals p.PurchaseID into Purchase
                            from p2 in Purchase.DefaultIfEmpty()
                            select new PrdWBProductionPurchase
                            {
                                WBProductionPurchaseID= l.WBProductionPurchaseID,
                                SchedulePurchaseID= l.SchedulePurchaseID,
                                MachineID = l.MachineID,
                                MachineNo = l.MachineNo,
                                SupplierID = l.SupplierID,
                                SupplierName = (sup2 == null ? null : sup2.SupplierName),
                                PurchaseID = l.PurchaseID,
                                PurchaseNo = (p2 == null ? null : p2.PurchaseNo),
                                ItemTypeID = l.ItemTypeID,
                                ItemTypeName = (it == null ? null : it.ItemTypeName),
                                LeatherTypeID = l.LeatherTypeID,
                                LeatherTypeName = (lt2 == null ? null : lt2.LeatherTypeName),
                                LeatherStatusID = l.LeatherStatusID,
                                LeatherStatusName = (ls2 == null ? null : ls2.LeatherStatusName),
                                ProductionPcs = l.WBProductionPcs,
                                ProductionSide = l.WBProductionSide,
                                ProductionWeight = l.WBProductionWeight,
                                WeightUnit = l.WeightUnit,
                                UnitName= (u2==null?null:u2.UnitName),
                                Remarks= l.Remarks
                            }).ToList();

                return Data;
            }
        }

        public List<PRDChemProdReqItem> GetChemicalListAfterSave(long WBProductionID, byte? _StoreID)
        {
            using (var context = new BLC_DEVEntities())
            {

                var StockResult = DalChemicalStock.CombinationWiseStockInSpecificProductionFloor(Convert.ToByte(_StoreID));

                var Data = (from c in context.PRD_WBProductionChemical.AsEnumerable()
                            where c.WBProductionID == WBProductionID

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
                                WBProdChemicalID = c.WBProdChemicalID,
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

        public List<PRDChemProdReqItem> GetChemicalItemListForFixedLeather(long _WBProductionID, long _WBProductionPurchaseID, byte? _StoreID)
        {
            using (var context = new BLC_DEVEntities())
            {

                var StockResult = DalChemicalStock.CombinationWiseStockInSpecificProductionFloor(Convert.ToByte(_StoreID));

                var Data = (from c in context.PRD_WBProductionChemical.AsEnumerable()
                            where c.WBProductionID == _WBProductionID & c.WBProductionPurchaseID== _WBProductionPurchaseID

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
                                WBProdChemicalID = c.WBProdChemicalID,
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


        public bool DeleteProductionItem(string _WBProdChemicalID)
        {
            try
            {
                var ProductionItem = (from c in _context.PRD_WBProductionChemical.AsEnumerable()
                                      where (c.WBProdChemicalID).ToString() == _WBProdChemicalID
                                      select c).FirstOrDefault();

                _context.PRD_WBProductionChemical.Remove(ProductionItem);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public bool DeleteChemicalConsumption(string _WBProductionID)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    var ProductionList = (from p in _context.PRD_WBProductionPurchase.AsEnumerable()
                                          where p.WBProductionID.ToString() == _WBProductionID
                                          select p).ToList();
                    foreach (var Leather in ProductionList)
                    {
                        _context.PRD_WBProductionPurchase.Remove(Leather);
                        _context.SaveChanges();
                    }
                    var ChemicalConsumption = (from c in _context.PRD_WBProduction.AsEnumerable()
                                               where c.WBProductionID.ToString() == _WBProductionID
                                               select c).FirstOrDefault();
                    _context.PRD_WBProduction.Remove(ChemicalConsumption);
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


        public bool CheckChemicalConsumtion(string _WBProductionID, string _ConfirmComment)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {
                        var TransactionInfo = (from p in _context.PRD_WBProduction.AsEnumerable()
                                               where (p.WBProductionID).ToString() == _WBProductionID
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


        public ValidationMsg ConfirmChemicalConsumption(long _WBProductionID, string _ConfirmComment, int _UserID)
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
                        var ChemicalConsumptionInfo = (from p in context.PRD_WBProduction
                                                       where p.WBProductionID == _WBProductionID
                                                       select p).FirstOrDefault();

                        var ChemicalConsumptionChemicalItemList = (from c in context.PRD_WBProductionChemical
                                                                   where c.WBProductionID == _WBProductionID
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
                            }
                            else
                            {
                                var FoundItem = (from i in context.INV_ChemStockSupplier
                                                 where i.SupplierID == item.SupplierID && i.StoreID == ChemicalConsumptionInfo.ProductionFloor &&
                                                 i.ItemID == item.ItemID & i.UnitID == item.UseUnitID
                                                 orderby i.TransectionID descending
                                                 select i).FirstOrDefault();

                                if (FoundItem.ClosingQty >= item.UseQty )
                                {
                                    var NewItem = new INV_ChemStockSupplier();

                                    NewItem.SupplierID = Convert.ToInt16(item.SupplierID);
                                    NewItem.StoreID = Convert.ToByte(ChemicalConsumptionInfo.ProductionFloor);
                                    NewItem.ItemID = item.ItemID;
                                    NewItem.UnitID = item.UseUnitID;
                                    NewItem.OpeningQty = FoundItem.ClosingQty;
                                    NewItem.IssueQty =  item.UseQty;
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

                                if (CurrentItem.ClosingQty >= item.UseQty )
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
                                    if (FoundItem.ClosingQty >= item.UseQty )
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
                                    }

                                }
                                else
                                {
                                    Msg.Msg = "Chemical Does Not Exist in Stock";
                                    Msg.ReturnId = 0;
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


        public List<SysChemicalItem> GetChemicalItemListInProductionFloor(byte _ProductionFloor)
        {
            var checkStoreCategory = (from s in _context.SYS_Store
                where s.StoreID == _ProductionFloor
                select s.StoreCategory).FirstOrDefault();

            List<ChemicalStockInfoModel> stockResult = null;

            if (checkStoreCategory == "Production")
            {
                stockResult = DalChemicalStock.CombinationWiseStockInSpecificProductionFloor(_ProductionFloor);
            }
            else
            {
                stockResult = DalChemicalStock.CombinationWiseStockInSpecificChemicalStore(_ProductionFloor);
            }
            
            
            var data = (from s in stockResult.AsEnumerable()

                        join c in _context.Sys_ChemicalItem on s.ItemID equals c.ItemID into Chemicals
                        from c in Chemicals.DefaultIfEmpty()

                        join sup in _context.Sys_Supplier on s.SupplierID equals sup.SupplierID into Suppliers
                        from sup in Suppliers.DefaultIfEmpty()

                        join su in _context.Sys_Unit on s.UnitID equals su.UnitID into StockUnits
                        from su in StockUnits.DefaultIfEmpty()

                        orderby c.ItemName

                        select new SysChemicalItem
                        {
                            ItemID = Convert.ToInt32(s.ItemID),
                            ItemName = c == null ? null : c.ItemName,
                            SupplierID = Convert.ToInt32(s.SupplierID),
                            SupplierName = (sup == null ? null : sup.SupplierName),
                            StockQty = s.ClosingQty,
                            StockUnit = s.UnitID,
                            StockUnitName = (su == null ? null : su.UnitName)
                        }).ToList();


            return data;
        }

    }
}
