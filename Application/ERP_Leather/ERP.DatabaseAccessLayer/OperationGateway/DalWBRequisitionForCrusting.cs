using System;
using System.Globalization;
using System.Text;
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
    public class DalWBRequisitionForCrusting
    {
        private readonly BLC_DEVEntities _context;
        private long YearMonID;

        public DalWBRequisitionForCrusting()
        {
            _context = new BLC_DEVEntities();
        }

        public string AddNewYearMonth(string _ScheduleYear, string _ScheduleMonth, string _ProductionFloor, string _ConcernStore)
        {
            try
            {
                var CheckYearMonth = (from r in _context.PRD_YearMonth.AsEnumerable()
                                      where r.ScheduleYear == _ScheduleYear && r.ScheduleMonth == _ScheduleMonth && r.ProductionFloor == Convert.ToByte(_ProductionFloor)
                                      && r.ScheduleFor == "WBR" && r.ConcernStore == Convert.ToByte(_ConcernStore)
                                      select r).Any();

                if (CheckYearMonth)
                {
                    return "Year Month Already Exists";
                }
                else
                {
                    var obj = new PRD_YearMonth();
                    obj.ScheduleYear = _ScheduleYear;
                    obj.ScheduleMonth = _ScheduleMonth;
                    obj.ProductionFloor = Convert.ToByte(_ProductionFloor);
                    obj.ConcernStore = Convert.ToByte(_ConcernStore);

                    obj.ScheduleFor = "WBR";
                    _context.PRD_YearMonth.Add(obj);
                    _context.SaveChanges();
                    YearMonID = obj.YearMonID;
                    return "Year Month Added Successfully";
                }

            }
            catch
            {
                return "Failed to Add New Year Month";
            }


        }

        public long AddNewRequisition(string _YearMonID, string _PrepareDate)
        {
            try
            {
                using (_context)
                {
                    PRD_YearMonthSchedule obj = new PRD_YearMonthSchedule();

                    obj.YearMonID = Convert.ToInt64(_YearMonID);
                    obj.PrepareDate = DalCommon.SetDate(_PrepareDate);
                    //Random r = new Random();
                    //obj.ScheduleNo = r.Next().ToString();
                    obj.ScheduleNo = DalCommon.GetPreDefineNextCodeByUrl("WetBlueProductionSchedule/WetBlueProductionSchedule");


                    _context.PRD_YearMonthSchedule.Add(obj);
                    _context.SaveChanges();

                    return obj.ScheduleID;
                }
            }
            catch
            {
                return 0;
            }

        }
        public long AddNewRequisitionDate(string _ScheduleID, string _RequiredDate)
        {
            try
            {
                using (_context)
                {
                    PRD_YearMonthCrustReqDate obj = new PRD_YearMonthCrustReqDate();

                    obj.ScheduleID = Convert.ToInt64(_ScheduleID);
                    obj.RequiredDate = DalCommon.SetDate(_RequiredDate);
                    //Random r = new Random();
                    //obj.RequisitionNo = r.Next().ToString();
                    obj.RequisitionNo = DalCommon.GetPreDefineNextCodeByUrl("WBRequisitionForCrusting/WBRequisitionForCrusting");
                    obj.RecordStatus = "NCF";


                    _context.PRD_YearMonthCrustReqDate.Add(obj);
                    _context.SaveChanges();

                    return obj.RequisitionDateID;
                }
            }
            catch
            {
                return 0;
            }

        }

        public string ReturnRequisitionNo(long _RequisitionDateID)
        {
            using (var context = new BLC_DEVEntities())
            {
                return context.PRD_YearMonthCrustReqDate.Where(x => x.RequisitionDateID == _RequisitionDateID).Select(x => x.RequisitionNo).FirstOrDefault();
            }

        }

        public string ReturnScheduleNo(long _ScheduleID)
        {
            using (var context = new BLC_DEVEntities())
            {
                return context.PRD_YearMonthSchedule.Where(x => x.ScheduleID == _ScheduleID).Select(x => x.ScheduleNo).FirstOrDefault();
            }

        }

        public long ReturnYearMonthID()
        {
            return YearMonID;
        }

        public List<PrdYearMonth> GetAllYearMonthCombination(string _ScheduleFor)
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

        public List<PrdYearMonth> GetAllYearMonthCombinationForRequisition(string _ScheduleFor)
        {
            using (_context)
            {
                var Data = (from r in _context.PRD_YearMonth.AsEnumerable()
                            where r.ScheduleFor == _ScheduleFor
                            join s in _context.SYS_Store on r.ProductionFloor equals s.StoreID into Stores
                            from s in Stores.DefaultIfEmpty()

                            join c in _context.SYS_Store on r.ConcernStore equals c.StoreID into ConcernStores
                            from c in ConcernStores.DefaultIfEmpty()

                            //join p in _context.PRD_YearMonthSchedule on r.YearMonID equals p.YearMonID into Schedules
                            //from p in Schedules.DefaultIfEmpty()

                            //join pp in _context.Sys_ProductionProces on (p == null ? null : p.ProductionProcessID) equals pp.ProcessID into Process
                            //from pp in Process.DefaultIfEmpty()

                            orderby r.YearMonID descending

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

                                //ScheduleID = (p == null ? 0 : p.ScheduleID),
                                //ScheduleNo = (p == null ? null : p.ScheduleNo),
                                //PrepareDate = (p == null ? null : Convert.ToDateTime(p.PrepareDate).ToString("dd'/'MM'/'yyyy")),
                                //ProductionProcessID = (p == null ? null : p.ProductionProcessID),
                                //ProductionProcessName = (pp == null ? null : pp.ProcessName)
                            }).ToList();

                return Data;
            }

        }

        public List<PrdYearMonthSchedule> GetAllRequisition(string _YearMonID)
        {
            using (_context)
            {
                var Data = (from r in _context.PRD_YearMonthSchedule.AsEnumerable()
                            where (r.YearMonID).ToString() == _YearMonID

                            join p in _context.Sys_ProductionProces on r.ProductionProcessID equals p.ProcessID into Process
                            from p in Process.DefaultIfEmpty()
                            select new PrdYearMonthSchedule
                            {
                                ScheduleID = r.ScheduleID,
                                ScheduleNo = r.ScheduleNo,
                                PrepareDate = Convert.ToDateTime(r.PrepareDate).ToString("dd'/'MM'/'yyyy"),
                                ProductionProcessID = r.ProductionProcessID,
                                ProductionProcessName = (p == null ? null : p.ProcessName)
                            }).ToList();

                return Data;
            }

        }
        public List<PRDYearMonthCrustReqDate> GetAllRequisitionDate(string _ScheduleID)
        {
            using (_context)
            {
                var Data = (from r in _context.PRD_YearMonthCrustReqDate.AsEnumerable()
                            where (r.ScheduleID).ToString() == _ScheduleID
                            select new PRDYearMonthCrustReqDate
                            {
                                RequisitionDateID = r.RequisitionDateID,
                                RequisitionNo = r.RequisitionNo,
                                RequiredDate = Convert.ToDateTime(r.RequiredDate).ToString("dd'/'MM'/'yyyy"),
                                RecordStatus = (r.RecordStatus == "NCF" ? "Not Confirmed" : "Confirmed")
                            }).ToList();

                return Data;
            }

        }

        public List<SysColor> GetAllActiveColor()
        {
            using (_context)
            {
                var Data = (from c in _context.Sys_Color.AsEnumerable()
                            where c.IsActive == true
                            select new SysColor
                            {
                                ColorID = c.ColorID,
                                ColorName = c.ColorName,
                                ColorCode = c.ColorCode
                            }).ToList();

                return Data;
            }
        }

        public List<SysArticle> GetAllActiveArticle()
        {
            using (_context)
            {
                var Data = (from c in _context.Sys_Article.AsEnumerable()
                            where c.IsActive == true

                            join cn in _context.Sys_Color on c.ArticleColor equals cn.ColorID into Colors
                            from cn in Colors.DefaultIfEmpty()
                            select new SysArticle
                            {
                                ArticleID = c.ArticleID,
                                ArticleName = c.ArticleName,
                                ArticleNo = c.ArticleNo,
                                //ArticleChallanNo = c.ArticleChallanNo,
                                ArticleColor = c.ArticleColor,
                                ColorName = (cn == null ? "" : cn.ColorName)
                                //ArticleCategory = (c.ArticleCategory)
                            }).ToList();

                return Data;
            }
        }

        public List<SysBuyer> GetAllActiveBuyer()
        {
            using (_context)
            {
                var Data = (from c in _context.Sys_Buyer.AsEnumerable()
                            where c.IsActive == true
                            orderby c.BuyerName
                            select new SysBuyer
                            {
                                BuyerID = c.BuyerID,
                                BuyerName = c.BuyerName,
                                BuyerCode = c.BuyerCode
                            }).ToList();

                return Data;
            }
        }

        public List<SysUnit> GetCategoryWiseUnit(string _UnitCategory)
        {
            using (_context)
            {
                var Data = (from u in _context.Sys_Unit.AsEnumerable()
                            where u.UnitCategory == _UnitCategory
                            select new SysUnit
                            {
                                UnitID = u.UnitID,
                                UnitName = u.UnitName
                            }).ToList();

                return Data;
            }


        }

        public List<SysLeatherStatus> GetAllLeatherStatus()
        {
            using (_context)
            {
                var Data = (from u in _context.Sys_LeatherStatus.AsEnumerable()
                            where u.IsActive & !u.IsDelete
                            select new SysLeatherStatus
                            {
                                LeatherStatusID = u.LeatherStatusID,
                                LeatherStatusName = u.LeatherStatusName
                            }).ToList();

                return Data;
            }


        }

        public List<PrdYearMonthCrustScheduleItem> GetScheduleList(string _ScheduleYear, string _ScheduleMonth, string _ProductionFloor)
        {
            using (_context)
            {
                var YearMonthID = (from y in _context.PRD_YearMonth.AsEnumerable()
                                   where y.ScheduleYear == _ScheduleYear & y.ScheduleMonth == _ScheduleMonth & (y.ProductionFloor).ToString() == _ProductionFloor &
                                   y.ScheduleFor == "CRP"
                                   select y.YearMonID).FirstOrDefault();

                var YearMonthSchedule = (from s in _context.PRD_YearMonthSchedule.AsEnumerable()
                                         where s.YearMonID == YearMonthID
                                         select new
                                         {
                                             ScheduleID = s.ScheduleID
                                         }).ToList();

                var YearMonthScheduleDate = (from s in YearMonthSchedule.AsEnumerable()
                                             join d in _context.PRD_YearMonthScheduleDate on s.ScheduleID equals d.ScheduleID into ScheduleDates
                                             from d in ScheduleDates.DefaultIfEmpty()
                                             select new
                                             {
                                                 ScheduleDateID = (d == null ? 0 : d.ScheduleDateID)
                                             }).ToList();

                var ScheduleItem = (from i in _context.PRD_YearMonthCrustScheduleItem.AsEnumerable()
                                    join d in YearMonthScheduleDate on i.ScheduleDateID equals d.ScheduleDateID into Items
                                    from d in Items.DefaultIfEmpty()

                                    join b in _context.Sys_Buyer on i.BuyerID equals b.BuyerID into Buyers
                                    from b in Buyers.DefaultIfEmpty()

                                    join o in _context.SLS_BuyerOrder on i.BuyerOrderID equals o.BuyerOrderID into Orders
                                    from o in Orders.DefaultIfEmpty()

                                    join u in _context.Sys_Unit on i.AvgSizeUnit equals u.UnitID into AVGUnits
                                    from u in AVGUnits.DefaultIfEmpty()

                                    join tu in _context.Sys_Unit on i.ThicknessUnit equals tu.UnitID into ThickUnits
                                    from tu in ThickUnits.DefaultIfEmpty()

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
                                        ItemTypeName = (it == null ? "" : it.ItemTypeName),

                                        LeatherStatusID = i.LeatherStatusID,
                                        LeatherStatusName = (ls == null ? "" : ls.LeatherStatusName),
                                        ArticleID = i.ArticleID,
                                        ArticleNo = i.ArticleNo,
                                        ArticleName = (a == null ? "" : a.ArticleName),
                                        ArticleChallanID= i.ArticleChallanID,
                                        ArticleChallanNo=i.ArticleChallanNo,
                                        AvgSize = i.AvgSize,
                                        AvgSizeUnit = i.AvgSizeUnit,
                                        AvgSizeUnitName = (u == null ? "" : u.UnitName),
                                        SideDescription = i.SideDescription,
                                        SelectionRange = i.SelectionRange,
                                        Thickness = i.Thickness,
                                        ThicknessUnit = i.ThicknessUnit,
                                        ThicknessUnitName = (tu == null ? "" : tu.UnitName),
                                        ThicknessAt = i.ThicknessAt == "AFSV" ? "After Shaving" : "After Finishing",
                                        Remarks = i.Remarks
                                    }).ToList();

                return ScheduleItem;
            }

        }

        public List<PrdYearMonthCrustScheduleColor> GetColorListForScheduleItem(string _ScheduleItemID)
        {
            using (_context)
            {
                var Data = (from c in _context.PRD_YearMonthCrustScheduleColor.AsEnumerable()
                            where c.ScheduleItemID.ToString() == _ScheduleItemID

                            join col in _context.Sys_Color on c.ColorID equals col.ColorID into Colors
                            from col in Colors.DefaultIfEmpty()

                            join u in _context.Sys_Unit on c.AreaUnit equals u.UnitID into Units
                            from u in Units.DefaultIfEmpty()

                            join wu in _context.Sys_Unit on c.WeightUnit equals wu.UnitID into WUnits
                            from wu in WUnits.DefaultIfEmpty()
                            select new PrdYearMonthCrustScheduleColor
                            {
                                SdulItemColorID = c.SdulItemColorID,
                                ColorID = c.ColorID,
                                ArticleColorNo= c.ArticleColorNo,
                                ColorName = (col == null ? null : col.ColorName),
                                ColorArea = c.ColorArea,
                                ColorPCS = c.ColorPCS,
                                ColorSide = c.ColorSide,
                                Remarks = c.Remarks,
                                AreaUnit = c.AreaUnit,
                                AreaUnitName = (u == null ? "" : u.UnitName),
                                ColorWeight = c.ColorWeight,
                                WeightUnit = c.WeightUnit,
                                WeightUnitName = (wu == null ? "" : wu.UnitName),
                                
                            }).ToList();

                return Data;
            }
        }

        public List<PrdYearMonthCrustScheduleColor> GetColorListForRequisitionItem(string _RequisitionItemID)
        {
            using (_context)
            {
                var Data = (from c in _context.PRD_YearMonthCrustReqItemColor.AsEnumerable()
                            where c.RequisitionItemID.ToString() == _RequisitionItemID

                            join col in _context.Sys_Color on c.ColorID equals col.ColorID into Colors
                            from col in Colors.DefaultIfEmpty()

                            join u in _context.Sys_Unit on c.AreaUnit equals u.UnitID into Units
                            from u in Units.DefaultIfEmpty()

                            orderby c.ArticleColorNo ascending
                            select new PrdYearMonthCrustScheduleColor
                            {
                                //SdulItemColorID = c.SdulItemColorID,
                                ReqItemColorID = c.ReqItemColorID,
                                RequisitionItemID = c.RequisitionItemID,
                                ColorID = c.ColorID,
                                ArticleColorNo= c.ArticleColorNo,
                                ColorName = (col == null ? null : col.ColorName),
                                ColorArea = c.ColorArea,
                                ColorPCS = c.ColorPcs,
                                ColorSide = c.ColorSide,
                                Remarks = c.Remarks,
                                AreaUnit = c.AreaUnit,
                                AreaUnitName = (u == null ? "" : u.UnitName)
                            }).ToList();

                return Data;
            }
        }

        public List<PrdYearMonthCrustScheduleColor> GetColorListForOrderItem(string _BuyerOrderItemID)
        {
            using (_context)
            {
                var Data = (from c in _context.SLS_BuyerOrderItemColor.AsEnumerable()
                            where c.BuyerOrderItemID.ToString() == _BuyerOrderItemID

                            join col in _context.Sys_Color on c.ColorID equals col.ColorID into Colors
                            from col in Colors.DefaultIfEmpty()

                            join u in _context.Sys_Unit on c.ColorUnit equals u.UnitID into Units
                            from u in Units.DefaultIfEmpty()
                            select new PrdYearMonthCrustScheduleColor
                            {
                                //SdulItemColorID = c.SdulItemColorID,
                                ColorID = c.ColorID,
                                ColorName = (col == null ? null : col.ColorName),
                                ColorArea = c.ColorQty,
                                //ColorPCS = c.ColorQty,
                                //ColorSide = c.ColorSide,
                                //Remarks = c.Remarks,
                                AreaUnit = c.ColorUnit,
                                AreaUnitName = (u == null ? "" : u.UnitName)
                            }).ToList();

                return Data;
            }
        }

        public List<SlsBuyerOrderItemBadhon> GetBuyerOrderList(string _BuyerID)
        {
            using (_context)
            {
                var Data = (from o in _context.SLS_BuyerOrder.AsEnumerable()
                            where o.BuyerID.ToString() == _BuyerID & o.RecordStatus == "ACK"
                            join i in _context.SLS_BuyerOrderItem on o.BuyerOrderID equals i.BuyerOrderID into Items
                            from i in Items.DefaultIfEmpty()

                            join ItemName in _context.Sys_ItemType on (i == null ? null : i.ItemTypeID) equals ItemName.ItemTypeID into ItemNames
                            from ItemName in ItemNames.DefaultIfEmpty()

                            join ls in _context.Sys_LeatherStatus on (i == null ? null : i.LeatherStatusID) equals ls.LeatherStatusID into LeatherStatus
                            from ls in LeatherStatus.DefaultIfEmpty()

                            join a in _context.Sys_Article on (i == null ? 0 : i.ArticleID) equals a.ArticleID into Articles
                            from a in Articles.DefaultIfEmpty()

                            join au in _context.Sys_Unit on (i == null ? 0 : i.AvgSizeUnit) equals au.UnitID into AVGUnits
                            from au in AVGUnits.DefaultIfEmpty()

                            join tu in _context.Sys_Unit on (i == null ? 0 : i.ThicknessUnit) equals tu.UnitID into ThickUnits
                            from tu in ThickUnits.DefaultIfEmpty()

                            orderby o.BuyerOrderID descending
                            select new SlsBuyerOrderItemBadhon
                            {
                                BuyerOrderID = o.BuyerOrderID,
                                BuyerOrderNo = o.BuyerOrderNo,
                                BuyerOrderDate = Convert.ToDateTime(o.BuyerOrderDate).ToString("dd'/'MM'/'yyyy"),
                                BuyerOrderItemID = (i == null ? 0 : i.BuyerOrderItemID),

                                ItemTypeID = (i == null ? 0 : i.ItemTypeID),
                                ItemTypeName = (ItemName == null ? "" : ItemName.ItemTypeName),

                                LeatherStatusID = (i == null ? 0 : i.LeatherStatusID),
                                LeatherStatusName = (ls == null ? "" : ls.LeatherStatusName),

                                ArticleID = (i == null ? 0 : i.ArticleID),
                                ArticleNo = (i == null ? null : i.ArticleNo),
                                ArticleChallanNo = (i == null ? null : i.ArticleChallanNo),
                                ArticleName = (a == null ? null : a.ArticleName),
                                AvgSize = (i == null ? null : i.AvgSize),
                                AvgSizeUnit = (i == null ? 0 : i.AvgSizeUnit),
                                AvgSizeUnitName = (au == null ? "" : au.UnitName),
                                SideDescription = (i == null ? null : i.SideDescription),
                                SelectionRange = (i == null ? null : i.SelectionRange),
                                Thickness = (i == null ? "" : i.Thickness),
                                ThicknessUnit = (i == null ? 0 : i.ThicknessUnit),
                                ThicknessUnitName = (tu == null ? "" : tu.UnitName),
                                ThicknessAt = (i == null ? "" : (i.ThicknessAt == "AFSV" ? "After Shaving" : "After Finishing")),
                            }).ToList();

                return Data;
            }
        }

        public long Save(PRDYearMonthCrustReqItem model, int userId)
        {
            long CurrentRequisitionItemID = 0;
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (_context)
                    {
                        var RequisitionDateInfo = (from r in _context.PRD_YearMonthCrustReqDate.AsEnumerable()
                                                   where r.RequisitionDateID == model.RequisitionDateID
                                                   select r).FirstOrDefault();

                        RequisitionDateInfo.Remark = model.ConfirmNote;
                        _context.SaveChanges();
                        
                        #region Update_Item_Information
                        if (model.ItemList != null)
                        {
                            foreach (var Item in model.ItemList)
                            {
                                #region New_Item_Insertion
                                if (Item.RequisitionItemID == 0)
                                {
                                    PRD_YearMonthCrustReqItem objItem = new PRD_YearMonthCrustReqItem();

                                    objItem.RequisitionDateID = Item.RequisitionDateID;
                                    objItem.ScheduleItemID = Item.ScheduleItemID;

                                    if (Item.ScheduleProductionNo != "Press F9")
                                    {
                                        objItem.ScheduleProductionNo = Item.ScheduleProductionNo;
                                    }


                                    //objItem.RequisitionNo = Convert.ToInt32(model.PurchaseID);
                                    objItem.BuyerID = Item.BuyerID;
                                    objItem.BuyerOrderID = Item.BuyerOrderID;
                                    objItem.ArticleID = Item.ArticleID;
                                    objItem.ArticleNo = Item.ArticleNo;

                                    if(Item.ArticleChallanID!=0)
                                    {
                                        objItem.ArticleChallanID = Item.ArticleChallanID;
                                        objItem.ArticleChallanNo = Item.ArticleChallanNo;
                                    }

                                    objItem.AvgSize = Item.AvgSize;

                                    if (Item.AvgSizeUnitName != null)
                                    {
                                        objItem.AvgSizeUnit = DalCommon.GetUnitCode(Item.AvgSizeUnitName);
                                    }


                                    objItem.SelectionRange = Item.SelectionRange;
                                    objItem.SideDescription = Item.SideDescription;
                                    objItem.Thickness = Item.Thickness;

                                    if (Item.ThicknessUnitName != null)
                                    {
                                        objItem.ThicknessUnit = DalCommon.GetUnitCode(Item.ThicknessUnitName);
                                    }
                                    if (Item.ThicknessAt != null)
                                    {
                                        objItem.ThicknessAt = (Item.ThicknessAt == "After Shaving" ? "AFSV" : "AFFN");
                                    }

                                    objItem.ItemTypeID = DalCommon.GetItemTypeCode(Item.ItemTypeName);
                                    objItem.LeatherStatusID = DalCommon.GetLeatherStatusCode(Item.LeatherStatusName);
                                    objItem.LeatherTypeID = (Item.LeatherTypeID);
                                    objItem.Remark = Item.Remarks;
                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId; ;

                                    _context.PRD_YearMonthCrustReqItem.Add(objItem);
                                    _context.SaveChanges();

                                    CurrentRequisitionItemID = objItem.RequisitionItemID;
                                }
                                #endregion

                                #region Existing_Item_Update
                                else if (Item.RequisitionItemID != 0)
                                {
                                    var CurrentItem = (from c in _context.PRD_YearMonthCrustReqItem.AsEnumerable()
                                                       where c.RequisitionItemID == Item.RequisitionItemID
                                                       select c).FirstOrDefault();

                                    CurrentItem.RequisitionDateID = Item.RequisitionDateID;
                                    //CurrentItem.RequisitionNo = Convert.ToInt32(model.PurchaseID);
                                    CurrentItem.BuyerID = Item.BuyerID;
                                    CurrentItem.BuyerOrderID = Item.BuyerOrderID;
                                    CurrentItem.ArticleID = Item.ArticleID;
                                    CurrentItem.ArticleNo = Item.ArticleNo;

                                    if (Item.ArticleChallanID != 0)
                                    {
                                        CurrentItem.ArticleChallanID = Item.ArticleChallanID;
                                        CurrentItem.ArticleChallanNo = Item.ArticleChallanNo;
                                    }


                                    
                                    CurrentItem.AvgSize = Item.AvgSize;
                                    if (Item.AvgSizeUnitName != null)
                                    {
                                        CurrentItem.AvgSizeUnit = DalCommon.GetUnitCode(Item.AvgSizeUnitName);
                                    }

                                    CurrentItem.SelectionRange = Item.SelectionRange;
                                    CurrentItem.SideDescription = Item.SideDescription;
                                    CurrentItem.Thickness = Item.Thickness;

                                    if (Item.ThicknessUnitName != null)
                                    {
                                        CurrentItem.ThicknessUnit = DalCommon.GetUnitCode(Item.ThicknessUnitName);
                                    }


                                    if (Item.ThicknessAt != null)
                                    {
                                        CurrentItem.ThicknessAt = (Item.ThicknessAt == "After Shaving" ? "AFSV" : "AFFN");
                                    }
                                    CurrentItem.ItemTypeID = DalCommon.GetItemTypeCode(Item.ItemTypeName);
                                    CurrentItem.LeatherStatusID = DalCommon.GetLeatherStatusCode(Item.LeatherStatusName);
                                    CurrentItem.LeatherTypeID = (Item.LeatherTypeID);
                                    CurrentItem.Remark = Item.Remarks;
                                    CurrentItem.ModifiedBy = userId;
                                    CurrentItem.ModifiedOn = DateTime.Now;
                                    _context.SaveChanges();

                                }
                                #endregion
                            }
                        }

                        #endregion

                        #region To_Find_ItemID_For_Items_If_Any

                        if (model.ColorList != null)
                        {
                            foreach (var Color in model.ColorList)
                            {
                                if (Color.RequisitionItemID != null && Color.RequisitionItemID != 0)
                                {
                                    CurrentRequisitionItemID = Convert.ToInt64(Color.RequisitionItemID);
                                    break;
                                }
                                else
                                {
                                    if (model.SelectedRequisitionItemID != 0)
                                    {
                                        CurrentRequisitionItemID = model.SelectedRequisitionItemID;
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion

                        #region Update_Item_Color_Information
                        if (model.ColorList != null)
                        {
                            foreach (var Color in model.ColorList)
                            {
                                #region New_Challan_Item_Insertion
                                if (Color.ReqItemColorID == 0)
                                {
                                    PRD_YearMonthCrustReqItemColor objColor = new PRD_YearMonthCrustReqItemColor();

                                    objColor.RequisitionItemID = CurrentRequisitionItemID;
                                    objColor.ColorID = Color.ColorID;
                                    objColor.ArticleColorNo = Color.ArticleColorNo;
                                    objColor.ColorPcs = (Color.ColorPCS);
                                    objColor.ColorSide = (Color.ColorSide);
                                    objColor.ColorArea = Color.ColorArea;
                                    if (Color.AreaUnitName != null)
                                    {
                                        objColor.AreaUnit = DalCommon.GetUnitCode(Color.AreaUnitName);
                                    }
                                    objColor.Remarks = Color.Remarks;
                                    objColor.SetBy = userId;
                                    objColor.SetOn = DateTime.Now;

                                    _context.PRD_YearMonthCrustReqItemColor.Add(objColor);
                                    _context.SaveChanges();
                                }
                                #endregion

                                #region Update_Existing_Challan_Item
                                else if (Color.ReqItemColorID != 0)
                                {
                                    var currentColorItem = (from c in _context.PRD_YearMonthCrustReqItemColor.AsEnumerable()
                                                            where c.ReqItemColorID == Color.ReqItemColorID
                                                            select c).FirstOrDefault();

                                    currentColorItem.RequisitionItemID = CurrentRequisitionItemID;
                                    currentColorItem.ColorID = Color.ColorID;
                                    currentColorItem.ArticleColorNo = Color.ArticleColorNo;
                                    currentColorItem.ColorPcs = (Color.ColorPCS);
                                    currentColorItem.ColorSide = (Color.ColorSide);
                                    currentColorItem.ColorArea = Color.ColorArea;
                                    if (Color.AreaUnitName != null)
                                    {
                                        currentColorItem.AreaUnit = DalCommon.GetUnitCode(Color.AreaUnitName);
                                    }
                                    currentColorItem.Remarks = Color.Remarks;
                                    currentColorItem.ModifiedBy = userId; ;
                                    currentColorItem.ModifiedOn = DateTime.Now;
                                    _context.SaveChanges();
                                }
                                #endregion

                            }
                        }
                        #endregion
                    }
                    transaction.Complete();
                }
                return CurrentRequisitionItemID;
            }
            catch (Exception e)
            {
                return 0;
            }

        }

        public List<PrdYearMonthCrustScheduleItem> GetRequisitionItemList(string _RequistionDateID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var ScheduleItem = (from i in context.PRD_YearMonthCrustReqItem.AsEnumerable()
                                    where i.RequisitionDateID.ToString() == _RequistionDateID
                                    //join d in YearMonthScheduleDate on i.ScheduleDateID equals d.ScheduleDateID into Items
                                    //from d in Items.DefaultIfEmpty()

                                    join b in context.Sys_Buyer on i.BuyerID equals b.BuyerID into Buyers
                                    from b in Buyers.DefaultIfEmpty()

                                    join o in context.SLS_BuyerOrder on i.BuyerOrderID equals o.BuyerOrderID into Orders
                                    from o in Orders.DefaultIfEmpty()

                                    join u in context.Sys_Unit on i.AvgSizeUnit equals u.UnitID into AVGUnits
                                    from u in AVGUnits.DefaultIfEmpty()

                                    join tu in context.Sys_Unit on i.ThicknessUnit equals tu.UnitID into ThickUnits
                                    from tu in ThickUnits.DefaultIfEmpty()

                                    join a in context.Sys_Article on i.ArticleID equals a.ArticleID into Articles
                                    from a in Articles.DefaultIfEmpty()

                                    join it in context.Sys_ItemType on i.ItemTypeID equals it.ItemTypeID into ItemTypes
                                    from it in ItemTypes.DefaultIfEmpty()

                                    join ls in context.Sys_LeatherStatus on i.LeatherStatusID equals ls.LeatherStatusID into LeatherStatus
                                    from ls in LeatherStatus.DefaultIfEmpty()

                                    select new PrdYearMonthCrustScheduleItem
                                    {
                                        //ScheduleItemID = i.ScheduleItemID,
                                        RequisitionItemID = i.RequisitionItemID,
                                        ScheduleProductionNo = i.ScheduleProductionNo,
                                        BuyerID = i.BuyerID,
                                        BuyerName = (b == null ? null : b.BuyerName),
                                        BuyerOrderID = i.BuyerOrderID,
                                        BuyerOrderNo = (o == null ? null : o.BuyerOrderNo),

                                        ItemTypeID = i.ItemTypeID,
                                        ItemTypeName = (it == null ? "" : it.ItemTypeName),

                                        LeatherStatusID = i.LeatherStatusID,
                                        LeatherStatusName = (ls == null ? "" : ls.LeatherStatusName),
                                        ArticleID = i.ArticleID,
                                        ArticleNo = i.ArticleNo,
                                        ArticleName = (a == null ? null : a.ArticleName),

                                        ArticleChallanID=i.ArticleChallanID,
                                        ArticleChallanNo= i.ArticleChallanNo,
                                        AvgSize = i.AvgSize,
                                        AvgSizeUnit = i.AvgSizeUnit,
                                        AvgSizeUnitName = (u == null ? "" : u.UnitName),
                                        SideDescription = i.SideDescription,
                                        SelectionRange = i.SelectionRange,
                                        Thickness = i.Thickness,
                                        ThicknessUnit = i.ThicknessUnit,
                                        ThicknessUnitName = (tu == null ? "" : tu.UnitName),
                                        ThicknessAt = (i.ThicknessAt == "AFSV" ? "After Shaving" : "After Finishing"),
                                        Remarks = i.Remark
                                    }).ToList();

                return ScheduleItem;
            }

        }
        public bool DeleteItem(string _RequisitionItemID)
        {
            try
            {
                var RequisitionItem = (from c in _context.PRD_YearMonthCrustReqItem.AsEnumerable()
                                       where (c.RequisitionItemID).ToString() == _RequisitionItemID
                                       select c).FirstOrDefault();

                _context.PRD_YearMonthCrustReqItem.Remove(RequisitionItem);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool DeleteColorItem(string _ReqItemColorID)
        {
            try
            {
                var Color = (from c in _context.PRD_YearMonthCrustReqItemColor.AsEnumerable()
                             where c.ReqItemColorID.ToString() == _ReqItemColorID
                             select c).FirstOrDefault();

                _context.PRD_YearMonthCrustReqItemColor.Remove(Color);
                _context.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public List<PRDYearMonthCrustReqDate> GetSearchInformation()
        {
            using (_context)
            {
                var Data = (from r in _context.PRD_YearMonthCrustReqDate.AsEnumerable()
                            //where r.RecordStatus == "NCF"

                            join s in _context.PRD_YearMonthSchedule on r.ScheduleID equals s.ScheduleID into Schedules
                            from s in Schedules.DefaultIfEmpty()

                            join y in _context.PRD_YearMonth on (s == null ? null : s.YearMonID) equals y.YearMonID into YearMonths
                            from y in YearMonths.DefaultIfEmpty()

                            join st in _context.SYS_Store on (y == null ? null : y.ProductionFloor) equals st.StoreID into Stores
                            from st in Stores.DefaultIfEmpty()

                            join c in _context.SYS_Store on (y == null ? null : y.ConcernStore) equals c.StoreID into ConcernStores
                            from c in ConcernStores.DefaultIfEmpty()
                            orderby r.RequisitionDateID descending
                            select new PRDYearMonthCrustReqDate
                            {
                                RequisitionDateID = r.RequisitionDateID,
                                ScheduleID = r.ScheduleID,
                                ScheduleNo = s == null ? null : s.ScheduleNo,

                                RequisitionNo = r.RequisitionNo,
                                RequiredDate = Convert.ToDateTime(r.RequiredDate).ToString("dd'/'MM'/'yyyy"),
                                RecordStatus = (r.RecordStatus == "NCF" ? "Not Confirmed" : "Confirmed"),
                                PrepareDate = Convert.ToDateTime(s == null ? null : s.PrepareDate).ToString("dd'/'MM'/'yyyy"),

                                YearMonID = (y == null ? 0 : y.YearMonID),
                                ScheduleMonth = (y == null ? null : y.ScheduleMonth),
                                ScheduleMonthName = (y == null ? null : DalCommon.ReturnMonthName(y.ScheduleMonth)),
                                ScheduleYear = (y == null ? null : y.ScheduleYear),
                                ProductionFloor = (y == null ? null : y.ProductionFloor),
                                ProductionFloorName = (st == null ? null : st.StoreName),
                                ConcernStore = (y == null ? null : y.ConcernStore),
                                ConcernStoreName = (c == null ? null : c.StoreName),
                                Remark = r.Remark
                            }).ToList();

                return Data;
            }
        }
        public bool ConfirmRequisition(string _RequisitionDateID, string confirmComment)
        {
            try
            {
                using (TransactionScope Transaction = new TransactionScope())
                {
                    using (_context)
                    {


                        var DateInfo = (from p in _context.PRD_YearMonthCrustReqDate.AsEnumerable()
                                        where (p.RequisitionDateID).ToString() == _RequisitionDateID
                                        select p).FirstOrDefault();
                        DateInfo.Remark = confirmComment;

                        DateInfo.RecordStatus = "CNF";

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
        public List<SysArticleChallan> GetArticleChallanList(int? _BuyerID, int? _ArticleID)
        {
            using (_context)
            {
                if(_BuyerID!=null)
                {
                    var Data = (from c in _context.Sys_ArticleChallan.AsEnumerable()

                                where c.BuyerID == _BuyerID && c.ArticleID == _ArticleID && c.RecordStatus == "CNF"

                                orderby c.ArticleChallanID descending
                                select new SysArticleChallan
                                {
                                    ArticleChallanID = c.ArticleChallanID,
                                    ArticleChallanNo = c.ArticleChallanNo,
                                    PreparationDate = Convert.ToDateTime(c.PreparationDate).ToString("dd'/'MM'/'yyyy"),
                                    ChallanNote = c.ChallanNote,
                                    ArticleNote = c.ArticleNote,
                                    ArticleArea = c.ArticleArea
                                }).ToList();
                    return Data;
                }
                else
                {
                    var Data = (from c in _context.Sys_ArticleChallan.AsEnumerable()

                                where c.ArticleID == _ArticleID && c.RecordStatus == "CNF"

                                orderby c.ArticleChallanID descending
                                select new SysArticleChallan
                                {
                                    ArticleChallanID = c.ArticleChallanID,
                                    ArticleChallanNo = c.ArticleChallanNo,
                                    PreparationDate = Convert.ToDateTime(c.PreparationDate).ToString("dd'/'MM'/'yyyy"),
                                    ChallanNote = c.ChallanNote,
                                    ArticleNote = c.ArticleNote,
                                    ArticleArea = c.ArticleArea
                                }).ToList();
                    return Data;
                }
                

                
            }
        }
        public List<PrdYearMonthCrustScheduleColor> GetColorListAccordingToArticleChallan(int? _BuyerOrderItemID, long _ArticleChallanID)
        {
            using (_context)
            {
                if(_BuyerOrderItemID!=null)
                {
                    var Data = (from c in _context.SLS_BuyerOrderItemColor.AsEnumerable()
                                where c.BuyerOrderItemID == _BuyerOrderItemID

                                join col in _context.Sys_Color on c.ColorID equals col.ColorID into Colors
                                from col in Colors.DefaultIfEmpty()

                                join u in _context.Sys_Unit on c.ColorUnit equals u.UnitID into Units
                                from u in Units.DefaultIfEmpty()

                                join a in _context.Sys_ArticleChallanColor.Where(x => x.ArticleChallanID == _ArticleChallanID) on c.ColorID equals a.ArticleColor into ArticleColors
                                from a in ArticleColors.DefaultIfEmpty()

                                orderby (a == null ? null : a.ArticleColorNo) ascending
                                select new PrdYearMonthCrustScheduleColor
                                {
                                    //SdulItemColorID = c.SdulItemColorID,
                                    ColorID = c.ColorID,
                                    ArticleColorNo = (a == null ? null : a.ArticleColorNo),
                                    ColorName = (col == null ? null : col.ColorName),
                                    ColorArea = c.ColorQty,
                                    //ColorPCS = c.ColorQty,
                                    //ColorSide = c.ColorSide,
                                    //Remarks = c.Remarks,
                                    AreaUnit = c.ColorUnit,
                                    AreaUnitName = (u == null ? "SFT" : u.UnitName)
                                }).ToList();

                    return Data;
                }

                else
                {
                    var Data = (from c in _context.Sys_ArticleChallanColor.AsEnumerable()
                                where c.ArticleChallanID == _ArticleChallanID

                                join col in _context.Sys_Color on c.ArticleColor equals col.ColorID into Colors
                                from col in Colors.DefaultIfEmpty()

                                orderby c.ArticleColorNo ascending
                                select new PrdYearMonthCrustScheduleColor
                                {
                                    ColorID = c.ArticleColor,
                                    ColorName = (col == null ? null : col.ColorName),
                                    ArticleColorNo = c.ArticleColorNo,
                                    
                                    AreaUnitName = "SFT"
                                }).ToList();

                    return Data;
                }
                
            }
        }
        
    }
}
