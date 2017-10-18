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
using ERP.EntitiesModel.AppSetupModel;
using System.Transactions;
using System.Linq;


namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalProductionRequisition
    {
        private readonly BLC_DEVEntities _context;

        public DalProductionRequisition()
        {
            _context = new BLC_DEVEntities();
        }

        public List<SysStore> GetCategoryTypeWiseStoreList(string _Category, string _Type)
        {
            var Data = (from s in _context.SYS_Store.AsEnumerable()
                        where s.StoreCategory == _Category && s.StoreType == _Type && s.IsActive && !s.IsDelete
                        select new SysStore
                        {
                            StoreID = s.StoreID,
                            StoreName = s.StoreName
                        }).ToList();

            return Data;
        }

        public List<SysStore> GetStoreListForFixedCategory(string _Category)
        {
            var Data = (from s in _context.SYS_Store.AsEnumerable()
                        where s.StoreCategory == _Category && s.IsActive && !s.IsDelete
                        select new SysStore
                        {
                            StoreID = s.StoreID,
                            StoreName = s.StoreName
                        }).ToList();

            return Data;
        }

        public List<SysProductionProces> GetRecipeCategoryList()
        {
            var Data = (from s in _context.Sys_ProductionProces.AsEnumerable()
                        where s.IsActive == true
                        select new SysProductionProces
                        {
                            ProcessID = s.ProcessID,
                            ProcessCategoryName= s.ProcessName
                            //ProcessCategory = DalCommon.ReturnProductionProcessCategoryName(s.ProcessCategory)

                        }).ToList();

            return Data;
        }

        // For Recipe LOV
        public List<PrdRecipe> GetRecipeForFixedCategory(int _RecipeFor)
        {
            var Data = (from s in _context.PRD_Recipe.AsEnumerable()
                        where s.RecipeFor == _RecipeFor

                        from c in _context.Sys_Color.Where(x => x.ColorID == s.ArticleColor).DefaultIfEmpty()
                        from u in _context.Sys_Unit.Where(x => x.UnitID == s.BaseUnit).DefaultIfEmpty()
                        select new PrdRecipe
                        {
                            RecipeID= s.RecipeID,
                            RecipeNo = s.RecipeNo,
                            RecipeName= s.RecipeName,
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
        public List<PRDChemProdReqItem> GetRecipeItemListForFixedRecipe(int _RecipeID, byte _RequisitionTo, byte _RequisitionFrom)
        {
            try
            {

                var FinalStock = DalChemicalStock.ItemWiseStockInSpecificChemicalStore(_RequisitionTo);

                var FinalStockProductionFloor = DalChemicalStock.ItemWiseStockInSpecificProductionFloor(_RequisitionFrom);

                if(FinalStock!= null)
                {
                    var Data = (from s in _context.PRD_RecipeItem.AsEnumerable()
                                where s.RecipeID == _RecipeID
                                from i in _context.Sys_ChemicalItem.Where(x => x.ItemID == s.ItemID).DefaultIfEmpty()

                                from u in _context.Sys_Unit.Where(x => x.UnitID == s.UnitID).DefaultIfEmpty()

                                join st in FinalStock on i.ItemID equals st.ItemID into badhon
                                from item in badhon.DefaultIfEmpty()
                                join sup in _context.Sys_Supplier on (item == null ? 0 : item.SupplierID) equals sup.SupplierID into badhon2
                                from finalitem in badhon2.DefaultIfEmpty()

                                join ps in FinalStockProductionFloor on i.ItemID equals ps.ItemID into ProductionStock
                                from ps in ProductionStock.DefaultIfEmpty()

                                select new PRDChemProdReqItem
                                {
                                    ItemID = s.ItemID,
                                    ItemName = (i == null ? null : i.ItemName),
                                    RequiredQty = s.RequiredQty,
                                    RequiredUnit = s.UnitID,
                                    RequiredUnitName = (u == null ? null : u.UnitName),
                                    RequsitionQty = s.RequiredQty,
                                    ApproveQty = s.RequiredQty,
                                    ApproveUnit = s.UnitID,
                                    ApproveUnitName = (u == null ? null : u.UnitName),
                                    PackSizeName = "",
                                    PackQty = 0,
                                    SizeUnitName = "",
                                    RequisitionUnit = s.UnitID,
                                    RequisitionUnitName = (u == null ? null : u.UnitName),

                                    StockQty = (item == null ? 0 : item.ClosingQty),
                                    //StockUnitName = (su == null ? null : su.UnitName),
                                    ProductionStock= (ps==null? "0": ps.ClosingQty.ToString()),
                                    SupplierID = (finalitem == null ? 0 : finalitem.SupplierID),
                                    SupplierName = (finalitem == null ? null : finalitem.SupplierName),
                                    
                                    ItemSource = "Via Requisition"
                                }).ToList();

                    return Data;
                }
                    
                else
                {
                    var Data = (from s in _context.PRD_RecipeItem.AsEnumerable()
                                where s.RecipeID == _RecipeID
                                from i in _context.Sys_ChemicalItem.Where(x => x.ItemID == s.ItemID).DefaultIfEmpty()
                                from u in _context.Sys_Unit.Where(x => x.UnitID == s.UnitID).DefaultIfEmpty()

                                join ps in FinalStockProductionFloor on i.ItemID equals ps.ItemID into ProductionStock
                                from ps in ProductionStock.DefaultIfEmpty()

                                select new PRDChemProdReqItem
                                {
                                    ItemID = s.ItemID,
                                    ItemName = (i == null ? null : i.ItemName),
                                    RequiredQty = s.RequiredQty,
                                    RequiredUnit = s.UnitID,
                                    RequiredUnitName = (u == null ? null : u.UnitName),
                                    ProductionStock = (ps == null ? "0" : ps.ClosingQty.ToString()),
                                    RequsitionQty = s.RequiredQty,
                                    ApproveQty = s.RequiredQty,
                                    ApproveUnit = s.UnitID,
                                    ApproveUnitName = (u == null ? null : u.UnitName),
                                    PackSizeName = "",
                                    PackQty = 0,
                                    SizeUnitName = "",
                                    RequisitionUnit = s.UnitID,
                                    RequisitionUnitName = (u == null ? null : u.UnitName),
                                    StockQty = 0,
                                    ItemSource = "Via Requisition"
                                }).ToList();
                    return Data;
                }
            }
            catch(Exception e)
            {
                return null;
            }
            
        }


        public List<PRDChemProdReqItem> RecalculateItemQuantity(int _RecipeID, decimal _Factor, byte _StoreID)
        {
            var FinalStock = DalChemicalStock.ItemWiseStockInSpecificChemicalStore(_StoreID);

            if (FinalStock.Count > 0)
            {
                var Data = (from s in _context.PRD_RecipeItem.AsEnumerable()
                            where s.RecipeID == _RecipeID
                            from i in _context.Sys_ChemicalItem.Where(x => x.ItemID == s.ItemID).DefaultIfEmpty()
                            from u in _context.Sys_Unit.Where(x => x.UnitID == s.UnitID).DefaultIfEmpty()


                            join st in FinalStock on i.ItemID equals st.ItemID into badhon
                            from item in badhon.DefaultIfEmpty()
                            join sup in _context.Sys_Supplier on (item == null ? 0 : item.SupplierID) equals sup.SupplierID into badhon2
                            from finalitem in badhon2.DefaultIfEmpty()
                            
                            select new PRDChemProdReqItem
                            {
                                ItemID = s.ItemID,
                                ItemName = (i == null ? null : i.ItemName),
                                RequiredQty = s.RequiredQty,
                                RequiredUnit = s.UnitID,
                                RequiredUnitName = (u == null ? null : u.UnitName),
                                RequsitionQty = s.RequiredQty,
                                ApproveQty = s.RequiredQty,
                                ApproveUnit = s.UnitID,
                                ApproveUnitName = (u == null ? null : u.UnitName),
                                PackSizeName = "",
                                PackQty = 0,
                                SizeUnitName = "",
                                RequisitionUnit = s.UnitID,
                                RequisitionUnitName = (u == null ? null : u.UnitName),

                                StockQty = (item == null ? 0 : item.ClosingQty),
                                //StockUnitName = (su == null ? null : su.UnitName),
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
                var Data = (from s in _context.PRD_RecipeItem.AsEnumerable()
                            where s.RecipeID == _RecipeID
                            from i in _context.Sys_ChemicalItem.Where(x => x.ItemID == s.ItemID).DefaultIfEmpty()
                            from u in _context.Sys_Unit.Where(x => x.UnitID == s.UnitID).DefaultIfEmpty()
                            select new PRDChemProdReqItem
                            {
                                ItemID = s.ItemID,
                                ItemName = (i == null ? null : i.ItemName),
                                RequiredQty = s.RequiredQty,
                                RequiredUnit = s.UnitID,
                                RequiredUnitName = (u == null ? null : u.UnitName),
                                RequsitionQty = s.RequiredQty,
                                ApproveQty = s.RequiredQty,
                                ApproveUnit = s.UnitID,
                                ApproveUnitName = (u == null ? null : u.UnitName),
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


        public List<SysUnit> GetUnitListForFixedCategory(string _Category)
        {
            var Data = (from u in _context.Sys_Unit.AsEnumerable()
                        where u.UnitCategory == _Category
                        select new SysUnit
                        {
                            UnitID = u.UnitID,
                            UnitName = u.UnitName
                        }).ToList();

            return Data;
        }


        public int Save(PRDChemProdReq model, int userId, string pageUrl)
        {
            int CurrentRequisitionID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {

                    using (_context)
                    {
                        var GetRequisitionNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);

                        if (GetRequisitionNo != null)
                        {
                            #region New_Requisition_Insert

                            PRD_ChemProdReq objRequisition = new PRD_ChemProdReq();

                            objRequisition.RequisitionNo = GetRequisitionNo;
                            objRequisition.RequisitionCategory = model.RequisitionCategory;
                            objRequisition.RequisitionType = model.RequisitionType;
                            objRequisition.RequisitionFrom = model.RequisitionFrom;
                            objRequisition.RequisitionTo = model.RequisitionTo;
                            objRequisition.RequiredByTime = model.RequiredByTime;
                            objRequisition.ReqRaisedBy = userId;
                            objRequisition.ReqRaisedOn = DalCommon.SetDate((model.ReqRaisedOn).ToString());
                            if (model.RecipeFor == 0)
                                objRequisition.RecipeFor = null;
                            else
                                objRequisition.RecipeFor = model.RecipeFor;
                            if (model.RecipeID == 0)
                                objRequisition.RecipeID = null;
                            else
                                objRequisition.RecipeID = model.RecipeID;
                            objRequisition.ArticleNo = model.ArticleNo;

                            objRequisition.Thickness = model.Thickness;
                            if (model.ThicknessUnit != null)
                                objRequisition.ThicknessUnit = model.ThicknessUnit;
                            objRequisition.LeatherSize = model.LeatherSize;
                            if (model.SizeUnit != null)
                                objRequisition.SizeUnit = model.SizeUnit;
                            objRequisition.Selection = model.Selection;
                            objRequisition.ProductionQty = model.ProductionQty;

                            objRequisition.RequisitionState = "RNG";
                            objRequisition.RequisitionStatus = "PND";
                            objRequisition.RecordStatus = "NCF";
                            objRequisition.SetBy = userId;
                            objRequisition.SetOn = DateTime.Now;

                            

                            //if (model.BuyerAddressID == 0)
                            //    objPI.BeneficiaryAddressID = null;
                            //else
                            //    objPI.BeneficiaryAddressID = model.BuyerAddressID;

                            _context.PRD_ChemProdReq.Add(objRequisition);
                            _context.SaveChanges();
                            CurrentRequisitionID = objRequisition.RequisitionID;
                            #endregion

                            #region Item Insert
                            if (model.RequisitionItemList != null)
                            {
                                foreach (var item in model.RequisitionItemList)
                                {
                                    PRD_ChemProdReqItem objItem = new PRD_ChemProdReqItem();
                                    objItem.RequisitionID = CurrentRequisitionID;
                                    objItem.ItemID = Convert.ToInt32(item.ItemID);

                                    if (item.SupplierID == 0)
                                        objItem.SupplierID = null;
                                    else
                                        objItem.SupplierID = item.SupplierID;
                                    //objItem.SupplierID = item.SupplierID;

                                    objItem.RequiredQty = item.RequiredQty;


                                    if (item.RequiredUnitName == null)
                                        objItem.RequiredUnit = null;
                                    else
                                        objItem.RequiredUnit = DalCommon.GetUnitCode(item.RequiredUnitName);

                                    
                                    objItem.RequsitionQty = Convert.ToDecimal(item.RequsitionQty);
                                    objItem.RequisitionUnit = DalCommon.GetUnitCode(item.RequisitionUnitName);

                                    if (item.PackSizeName == null)
                                        objItem.PackSize = null;
                                    else
                                        objItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);

                                    //objItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);

                                    if (item.SizeUnitName == null)
                                        objItem.SizeUnit = null;
                                    else
                                        objItem.SizeUnit = DalCommon.GetUnitCode(item.SizeUnitName);
                                    //objItem.SizeUnit = DalCommon.GetUnitCode(item.SizeUnitName);


                                    objItem.PackQty = item.PackQty;
                                    objItem.ApproveQty = item.ApproveQty;
                                    objItem.ApproveUnit = DalCommon.GetUnitCode(item.ApproveUnitName);
                                    //objItem.ManufacturerID = item.ManufacturerID;
                                    objItem.ItemSource = DalCommon.ReturnItemSource(item.ItemSource);
                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;

                                    _context.PRD_ChemProdReqItem.Add(objItem);
                                    _context.SaveChanges();
                                }

                            }
                            #endregion
                        }

                    }
                    transaction.Complete();
                }
                return CurrentRequisitionID;
            }
            catch (Exception e)
            {
                return 0;
            }
        }


        public int Update(PRDChemProdReq model, int userId)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (_context)
                    {

                        #region PI_Informaiton_Update
                        var CurrentRequisition = (from p in _context.PRD_ChemProdReq.AsEnumerable()
                                                  where p.RequisitionID == model.RequisitionID
                                                  select p).FirstOrDefault();

                        CurrentRequisition.RequisitionCategory = model.RequisitionCategory;
                        CurrentRequisition.RequisitionType = model.RequisitionType;
                        CurrentRequisition.RequisitionFrom = model.RequisitionFrom;
                        CurrentRequisition.RequisitionTo = model.RequisitionTo;
                        CurrentRequisition.RequiredByTime = model.RequiredByTime;
                        CurrentRequisition.ReqRaisedBy = 1;
                        CurrentRequisition.ReqRaisedOn = DalCommon.SetDate((model.ReqRaisedOn).ToString());

                        if (model.RecipeFor == 0)
                            CurrentRequisition.RecipeFor = null;
                        else
                            CurrentRequisition.RecipeFor = model.RecipeFor;
                        if (model.RecipeID == 0)
                            CurrentRequisition.RecipeID = null;
                        else
                            CurrentRequisition.RecipeID = model.RecipeID;

                        //CurrentRequisition.RecipeFor = model.RecipeFor;
                        //CurrentRequisition.RecipeID = model.RecipeID;
                        CurrentRequisition.ArticleNo = model.ArticleNo;

                        CurrentRequisition.Thickness = model.Thickness;
                        if (model.ThicknessUnit != null)
                            CurrentRequisition.ThicknessUnit = model.ThicknessUnit;
                        CurrentRequisition.LeatherSize = model.LeatherSize;
                        if (model.SizeUnit != null)
                            CurrentRequisition.SizeUnit = model.SizeUnit;
                        CurrentRequisition.Selection = model.Selection;
                        CurrentRequisition.ProductionQty = model.ProductionQty;
                        CurrentRequisition.ModifiedBy = userId;
                        CurrentRequisition.ModifiedOn = DateTime.Now;
                        
                        _context.SaveChanges();
                        #endregion

                        #region Update Requisition ItemList
                        if (model.RequisitionItemList != null)
                        {
                            foreach (var item in model.RequisitionItemList)
                            {

                                var checkRequisitionItem = (from i in _context.PRD_ChemProdReqItem.AsEnumerable()
                                                            where i.RequisitionItemID == item.RequisitionItemID
                                                   select i).Any();

                                #region New_Requisition_Insertion
                                if (!checkRequisitionItem)
                                {
                                    PRD_ChemProdReqItem objItem = new PRD_ChemProdReqItem();
                                    objItem.RequisitionID = model.RequisitionID;
                                    objItem.ItemID = Convert.ToInt32(item.ItemID);

                                    if (item.SupplierID == 0)
                                        objItem.SupplierID = null;
                                    else
                                        objItem.SupplierID = item.SupplierID;

                                    objItem.RequiredQty = item.RequiredQty;


                                    if (item.RequiredUnitName == null)
                                        objItem.RequiredUnit = null;
                                    else
                                        objItem.RequiredUnit = DalCommon.GetUnitCode(item.RequiredUnitName);

                                    objItem.RequsitionQty = Convert.ToDecimal(item.RequsitionQty);
                                    objItem.RequisitionUnit = DalCommon.GetUnitCode(item.RequisitionUnitName);

                                    if (item.PackSizeName == null)
                                        objItem.PackSize = null;
                                    else
                                        objItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);

                                    //objItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);

                                    if (item.SizeUnitName == null)
                                        objItem.SizeUnit = null;
                                    else
                                        objItem.SizeUnit = DalCommon.GetUnitCode(item.SizeUnitName);


                                    objItem.PackQty = item.PackQty;
                                    objItem.ApproveQty = item.ApproveQty;
                                    objItem.ApproveUnit = DalCommon.GetUnitCode(item.ApproveUnitName);
                                    //objItem.ManufacturerID = item.ManufacturerID;
                                    objItem.ItemSource = DalCommon.ReturnItemSource(item.ItemSource);
                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;

                                    _context.PRD_ChemProdReqItem.Add(objItem);
                                    _context.SaveChanges();


                                }
                                #endregion

                                #region Existing_Requisition_Update
                                else
                                {
                                    var FoundRequisition = (from i in _context.PRD_ChemProdReqItem.AsEnumerable()
                                                   where i.RequisitionItemID == item.RequisitionItemID
                                                   select i).FirstOrDefault();

                                    FoundRequisition.RequisitionID = model.RequisitionID;
                                    FoundRequisition.ItemID = Convert.ToInt32(item.ItemID);

                                    if (item.SupplierID == 0)
                                        FoundRequisition.SupplierID = null;
                                    else
                                        FoundRequisition.SupplierID = item.SupplierID;

                                    FoundRequisition.RequiredQty = item.RequiredQty;

                                    if (item.RequiredUnitName == null)
                                        FoundRequisition.RequiredUnit = null;
                                    else
                                        FoundRequisition.RequiredUnit = DalCommon.GetUnitCode(item.RequiredUnitName);

                                    FoundRequisition.RequsitionQty = Convert.ToDecimal(item.RequsitionQty);
                                    FoundRequisition.RequisitionUnit = DalCommon.GetUnitCode(item.RequisitionUnitName);
                                    if (item.PackSizeName == null)
                                        FoundRequisition.PackSize = null;
                                    else
                                        FoundRequisition.PackSize = DalCommon.GetSizeCode(item.PackSizeName);

                                    //objItem.PackSize = DalCommon.GetSizeCode(item.PackSizeName);

                                    if (item.SizeUnitName == null)
                                        FoundRequisition.SizeUnit = null;
                                    else
                                        FoundRequisition.SizeUnit = DalCommon.GetUnitCode(item.SizeUnitName);
                                    FoundRequisition.PackQty = item.PackQty;
                                    FoundRequisition.ApproveQty = item.ApproveQty;
                                    FoundRequisition.ApproveUnit = DalCommon.GetUnitCode(item.ApproveUnitName);
                                    FoundRequisition.ItemSource = DalCommon.ReturnItemSource(item.ItemSource);
                                    FoundRequisition.ModifiedOn = DateTime.Now;
                                    FoundRequisition.ModifiedBy = userId;

                                    _context.SaveChanges();

                                }
                                #endregion
                            }
                        }

                        #endregion
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

        public string GetRequisitionNo(long _RequisitionID)
        {
            using (_context)
            {
                var RequisitionNo = (from p in _context.PRD_ChemProdReq.AsEnumerable()
                                     where p.RequisitionID == _RequisitionID
                                     select p.RequisitionNo).FirstOrDefault();

                return RequisitionNo;
            }

        }

        public byte GetRequisitionToInfo(long _RequisitionID)
        {
            using (var context= new BLC_DEVEntities())
            {
                var RequisitionTo = (from p in context.PRD_ChemProdReq.AsEnumerable()
                                     where p.RequisitionID == _RequisitionID
                                     select p.RequisitionTo).FirstOrDefault();

                return RequisitionTo;
            }

        }

        // After Save & Update to get fresh data into page grid
        public List<PRDChemProdReqItem> GetRequisitionItemList(int _RequisitionID, byte _StoreID)
        {
            using(var context= new BLC_DEVEntities())
            {
                var FinalStock = DalChemicalStock.ItemWiseStockInSpecificChemicalStore(_StoreID);

                var Data = (from r in context.PRD_ChemProdReqItem.AsEnumerable()
                            where r.RequisitionID == _RequisitionID

                            from i in context.Sys_ChemicalItem.Where(x => x.ItemID == r.ItemID).DefaultIfEmpty()

                            from u in context.Sys_Unit.Where(x => x.UnitID == r.RequiredUnit).DefaultIfEmpty()

                            from su in context.Sys_Unit.Where(x => x.UnitID == r.SizeUnit).DefaultIfEmpty()

                            from rq in context.Sys_Unit.Where(x => x.UnitID == r.RequisitionUnit).DefaultIfEmpty()

                            from ap in context.Sys_Unit.Where(x => x.UnitID == r.ApproveUnit).DefaultIfEmpty()

                            from size in context.Sys_Size.Where(x => x.SizeID == r.PackSize).DefaultIfEmpty()

                            join s in FinalStock on i.ItemID equals s.ItemID into StockInfo
                            from item in StockInfo.DefaultIfEmpty()
                            join sup in context.Sys_Supplier on (item == null ? 0 : item.SupplierID) equals sup.SupplierID into badhon2
                            from finalitem in badhon2.DefaultIfEmpty()

                            select new PRDChemProdReqItem
                            {
                                RequisitionItemID = r.RequisitionItemID,
                                ItemID = r.ItemID,
                                ItemName = (i == null ? null : i.ItemName),
                                RequiredQty = Convert.ToDecimal(r.RequiredQty),
                                RequiredUnit = Convert.ToByte(r.RequiredUnit),
                                RequiredUnitName = (u == null ? null : u.UnitName),
                               
                                PackSize = Convert.ToByte(r.PackSize),
                                PackSizeName = (size == null ? null : size.SizeName),
                                SizeUnit = Convert.ToByte(r.SizeUnit),
                                SizeUnitName = (su == null ? null : su.UnitName),
                                PackQty = Convert.ToInt32(r.PackQty),
                                RequsitionQty = r.RequsitionQty,
                                RequisitionUnit = r.RequisitionUnit,
                                RequisitionUnitName = (rq == null ? null : rq.UnitName),
                                ApproveQty = Convert.ToDecimal(r.ApproveQty),
                                ApproveUnit = Convert.ToByte(r.ApproveUnit),
                                ApproveUnitName = (ap == null ? null : ap.UnitName),

                                StockQty = (item == null ? 0 : (item.ClosingQty)),
                                //StockUnitName = (su == null ? null : su.UnitName),
                                SupplierID = (finalitem == null ? 0 : finalitem.SupplierID),
                                SupplierName = (finalitem == null ? null : finalitem.SupplierName)

                                
                            }).ToList();

                return Data;
            }
            
        }

        public IEnumerable<PRDChemProdReq> GetRequistionInfoForSearch()
        {
            var Data = (from r in _context.PRD_ChemProdReq
                        
                        join rf in _context.SYS_Store on (r.RequisitionFrom == null ? 0 : r.RequisitionFrom) equals rf.StoreID into requisitionfrom
                        from rf in requisitionfrom.DefaultIfEmpty()

                        join rt in _context.SYS_Store on (r.RequisitionTo == null ? 0 : r.RequisitionTo) equals rt.StoreID into requisitionto
                        from rt in requisitionto.DefaultIfEmpty()

                        orderby r.RequisitionNo descending
                        select new PRDChemProdReq
                        {
                            RequisitionID = r.RequisitionID,
                            RequisitionNo = r.RequisitionNo,
                            RequisitionCategory = (r.RequisitionCategory),
                            RequisitionType = r.RequisitionType ,
                            ReqRaisedOnTemp = (r.ReqRaisedOn),
                            RecordStatus = r.RecordStatus,
                            RequisitionFromName = (rf == null ? null : rf.StoreName),
                            RequisitionToName = (rt == null ? null : rt.StoreName)
                        });
            return Data;
        }

        public PRDChemProdReq GetRequisitionDetailsAfterSearch(int _RequisitionID)
        {
            var model = new PRDChemProdReq();


            var RequisitionInfo = (from r in _context.PRD_ChemProdReq.AsEnumerable()
                                   where r.RequisitionID == _RequisitionID

                                   join re in _context.PRD_Recipe on (r==null?null:r.RecipeID) equals re.RecipeID into Recepies
                                   from re in Recepies.DefaultIfEmpty()

                                   join c in _context.Sys_Color on (re==null? null: re.ArticleColor) equals c.ColorID into Colors
                                   from c in Colors.DefaultIfEmpty()

                                   join u in _context.Sys_Unit on (re == null ? 0 : re.BaseUnit) equals u.UnitID into Units
                                   from u in Units.DefaultIfEmpty()

                                   select new PRDChemProdReq
                                   {
                                       RequisitionID = r.RequisitionID,
                                       RequisitionNo = r.RequisitionNo,
                                       RequisitionCategory = r.RequisitionCategory,
                                       RequisitionType = r.RequisitionType,
                                       ReqRaisedOn = (Convert.ToDateTime(r.ReqRaisedOn)).ToString("dd'/'MM'/'yyyy"),
                                       RequiredByTime= (r.RequiredByTime==0? null: r.RequiredByTime),
                                       ReqRaisedBy = (r.ReqRaisedBy),
                                       RequisitionFrom = r.RequisitionFrom,
                                       RequisitionTo = r.RequisitionTo,
                                       RecipeFor = (r.RecipeFor),
                                       RecipeID = (r.RecipeID),
                                       ArticleNo= r.ArticleNo,
                                       ArticleName= (re==null? null: re.ArticleName),
                                       ArticleChallanNo = (re == null ? null : re.ArticleChallanNo),
                                       ArticleColor = (re == null ? null : re.ArticleColor),
                                       ArticleColorName = (c == null ? null : c.ColorName),
                                       BaseQuantity= (re == null ? null : (re.BaseQuantity).ToString()),
                                       //BaseUnit = (re == null ? null : re.BaseUnit),
                                       BaseUnitName = (u == null ? null : u.UnitName),
                                       LeatherSize = r.LeatherSize,
                                       SizeUnit = (r.SizeUnit),
                                       Selection = r.Selection,
                                       Thickness = r.Thickness,
                                       ThicknessUnit = (r.ThicknessUnit),
                                       ProductionQty = (r.ProductionQty == 0 ? null : r.ProductionQty),
                                       ProductionQuantityUnit = (u == null ? null : u.UnitName),
                                       RecordStatus= r.RecordStatus
                                   }).FirstOrDefault();

            model.RequisitionInfo = RequisitionInfo;

            model.RequisitionItemList = GetRequisitionItemList(_RequisitionID, RequisitionInfo.RequisitionTo); 

            return model;

        }

        public bool DeleteRequisitionItem(string _RequisitionItemID)
        {
            try
            {
                var RequisitionItem = (from c in _context.PRD_ChemProdReqItem.AsEnumerable()
                                       where c.RequisitionItemID == Convert.ToInt64(_RequisitionItemID)
                                       select c).FirstOrDefault();

                _context.PRD_ChemProdReqItem.Remove(RequisitionItem);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteRequisition(string _RequisitionID)
        {
            try
            {
                var Requisition = (from c in _context.PRD_ChemProdReq.AsEnumerable()
                          where c.RequisitionID == Convert.ToInt64(_RequisitionID)
                          select c).FirstOrDefault();

                _context.PRD_ChemProdReq.Remove(Requisition);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool CheckRequisition(string _RequisitionID, string _CheckComment)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {
                        var RequisitionInfo = (from p in _context.PRD_ChemProdReq.AsEnumerable()
                                               where (p.RequisitionID).ToString() == _RequisitionID
                                               select p).FirstOrDefault();

                        RequisitionInfo.ApprovalAdviceNote = _CheckComment;

                        RequisitionInfo.RecordStatus = "CHK";

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

        public bool ApproveRequisition(string _RequisitionID)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {


                        var RequisitionInfo = (from p in _context.PRD_ChemProdReq.AsEnumerable()
                                               where (p.RequisitionID).ToString() == _RequisitionID
                                               select p).FirstOrDefault();
                        

                        RequisitionInfo.RecordStatus = "APV";

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

        public List<SysChemicalItem> GetChemicalItemWithStock(byte _RequisitionFrom, byte _RequisitionTo)
        {

            var FinalStock = DalChemicalStock.ItemWiseStockInSpecificChemicalStore(_RequisitionTo);

            var FinalStockProductionFloor = DalChemicalStock.ItemWiseStockInSpecificProductionFloor(_RequisitionFrom);

            var Data = (from c in _context.Sys_ChemicalItem.AsEnumerable()
                        where c.IsActive == true

                        join fs in FinalStock on c.ItemID equals fs.ItemID into FinalStocks
                        from fs in FinalStocks.DefaultIfEmpty()

                        join ps in FinalStockProductionFloor on c.ItemID equals ps.ItemID into ProductionStock
                        from ps in ProductionStock.DefaultIfEmpty()

                        join it in _context.Sys_ItemType on c.ItemTypeID equals it.ItemTypeID into Items
                        from it2 in Items.DefaultIfEmpty()

                        orderby c.ItemName
                        select new SysChemicalItem
                        {
                            ItemID = c.ItemID,
                            ItemName = c.ItemName,
                            ItemCategory = DalCommon.ReturnChemicalItemCategory(c.ItemCategory),
                            ItemTypeID = c.ItemTypeID,
                            ItemTypeName = (it2 == null ? null : it2.ItemTypeName),
                            StockQty = (fs == null ? 0 : fs.ClosingQty),
                            SafetyStock = ps == null ? 0 : Convert.ToInt32(ps.ClosingQty) //Actually Production Stock
                        }).ToList();

            return Data;
        }

    }
}
