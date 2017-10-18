using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalFinishedLeatherTransferTreatment
    {


        private UnitOfWork repository = new UnitOfWork();
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        //public string WBSelectionNo = string.Empty;

        public long FNTransferFromID = 0;
        public long FNTransferToID = 0;
        public long FNTransferID = 0;
        public string FNTransferNo = string.Empty;
        private bool save;

        public DalFinishedLeatherTransferTreatment()
        {
            _vmMsg = new ValidationMsg();
            _context = new BLC_DEVEntities();
        }


        public List<InvFNTransfer> GetQCStockDetail(string TransactionStore)//, string CLTransferCategory
        {
            var model= new InvFNTransfer();

            if (model.FNTransferCategory=="ABQC")
            {
            var queryString = @"	SELECT		    FBQS.TransectionID,
													ISNULL(FBQS.StoreID,0)StoreID,
													ISNULL(FBQS.BuyerID,0)BuyerID,
													ISNULL((select BuyerName from Sys_Buyer where BuyerID = FBQS.BuyerID),'')BuyerName,
													ISNULL(FBQS.BuyerOrderID,0)BuyerOrderID,
													ISNULL((select BuyerOrderNo from SLS_BuyerOrder where BuyerOrderID = FBQS.BuyerOrderID),'')BuyerOrderNo,
													ISNULL(FBQS.ItemTypeID,0)ItemTypeID,
													ISNULL((select ItemTypeName from Sys_ItemType where ItemTypeID = FBQS.ItemTypeID),'')ItemTypeName,
													ISNULL(FBQS.LeatherTypeID,0)LeatherTypeID,
													ISNULL((select LeatherTypeName from Sys_LeatherType where LeatherTypeID = FBQS.LeatherTypeID),'')LeatherTypeName,
													ISNULL(FBQS.LeatherStatusID,0)LeatherStatusID,
													ISNULL((select LeatherStatusName from Sys_LeatherStatus where LeatherStatusID = FBQS.LeatherStatusID),'')LeatherStatusName,
													ISNULL(FBQS.ArticleID,0)ArticleID,
													ISNULL(FBQS.ArticleNo,'')ArticleNo,
													ISNULL(FBQS.ArticleChallanNo,'')ArticleChallanNo,
													ISNULL(FBQS.ArticleColorNo,'')ArticleColorNo,
													ISNULL((select ArticleName from Sys_Article where ArticleID = FBQS.ArticleID),'')ArticleName,
													ISNULL(FBQS.ColorID,0)ColorID,
													ISNULL((select ColorName from Sys_Color where ColorID = FBQS.ColorID),'')ColorName,
													ISNULL(FBQS.FinishQCLabel,0)FinishQCLabel,
													ISNULL(FBQS.ClosingStockPcs,0)ClosingStockPcs

										FROM		INV_FinishBuyerQCStock FBQS		
										INNER JOIN
                                        
												(SELECT					MAX(TransectionID)TransectionID,
																		BuyerID,
																		StoreID,
																		ItemTypeID,
																		LeatherTypeID,
																		LeatherStatusID,
																		ArticleID,
																		ArticleChallanID,
																		ColorID 
					                                    FROM		INV_FinishBuyerQCStock
                                                        GROUP BY	StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,ArticleChallanID) sup
                                        ON FBQS.TransectionID=sup.TransectionID
                                        WHERE FBQS.StoreID = '" + TransactionStore + "' and FBQS.ClosingStockPcs>0 ORDER BY FBQS.TransectionID DESC";

            var stockList = _context.Database.SqlQuery<InvFNTransfer>(queryString);
            return stockList.ToList();
            }
            else if(model.FNTransferCategory=="AOQC")
            {
                var queryString = @"	SELECT		FOQS.TransectionID,
													ISNULL(FOQS.StoreID,0)StoreID,
													ISNULL(FOQS.BuyerID,0)BuyerID,
													ISNULL((select BuyerName from Sys_Buyer where BuyerID = FOQS.BuyerID),'')BuyerName,
													ISNULL(FOQS.BuyerOrderID,0)BuyerOrderID,
													ISNULL((select BuyerOrderNo from SLS_BuyerOrder where BuyerOrderID = FOQS.BuyerOrderID),'')BuyerOrderNo,
													ISNULL(FOQS.ItemTypeID,0)ItemTypeID,
													ISNULL((select ItemTypeName from Sys_ItemType where ItemTypeID = FOQS.ItemTypeID),'')ItemTypeName,
													ISNULL(FOQS.LeatherTypeID,0)LeatherTypeID,
													ISNULL((select LeatherTypeName from Sys_LeatherType where LeatherTypeID = FOQS.LeatherTypeID),'')LeatherTypeName,
													ISNULL(FOQS.LeatherStatusID,0)LeatherStatusID,
													ISNULL((select LeatherStatusName from Sys_LeatherStatus where LeatherStatusID = FOQS.LeatherStatusID),'')LeatherStatusName,
													ISNULL(FOQS.ArticleID,0)ArticleID,
													ISNULL(FOQS.ArticleNo,'')ArticleNo,
													ISNULL(FOQS.ArticleChallanNo,'')ArticleChallanNo,
													ISNULL(FOQS.ArticleColorNo,'')ArticleColorNo,
													ISNULL((select ArticleName from Sys_Article where ArticleID = FOQS.ArticleID),'')ArticleName,
													ISNULL(FOQS.ColorID,0)ColorID,
													ISNULL((select ColorName from Sys_Color where ColorID = FOQS.ColorID),'')ColorName,
													ISNULL(FOQS.FinishQCLabel,0)FinishQCLabel,
													ISNULL(FOQS.ClosingStockPcs,0)ClosingStockPcs

										FROM		INV_FinishOwnQCStock FOQS
										
										INNER JOIN
                                        
												(SELECT					MAX(TransectionID)TransectionID,
																		BuyerID,
																		StoreID,
																		ItemTypeID,
																		LeatherTypeID,
																		LeatherStatusID,
																		ArticleID,
																		ArticleChallanID,
																		ColorID 
					                                    FROM		INV_FinishBuyerQCStock
                                                        GROUP BY	StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,ArticleChallanID) sup
                                            ON FOQS.TransectionID=sup.TransectionID
                                        WHERE FOQS.StoreID = '" + TransactionStore + "' and FOQS.ClosingStockPcs>0 ORDER BY FOQS.TransectionID DESC";
                var stockList = _context.Database.SqlQuery<InvFNTransfer>(queryString);
                return stockList.ToList();
            }

            else
            {

                return null;
            }

        }



        public List<InvFNTransferFrom> GetFinishTransferFromGridDetail(string TransactionStore, string BuyerID, string ArticleID, string BuyerOrderID, string ItemTypeID, string LeatherStatusID)
        {
            if (!string.IsNullOrEmpty(TransactionStore))
            {
                var queryString = @"SELECT 
        		                        ISNULL(cqs.BuyerID,'')BuyerID,
        		                        ISNULL(cqs.ArticleID,'')ArticleID,
        		                        ISNULL(cqs.ItemTypeID,'')ItemTypeID,
        		                        ISNULL(cqs.LeatherStatusID,'')LeatherStatusID,
        		                        ISNULL(cqs.GradeID,'')GradeID,
        		                        ISNULL(cqs.ColorID,'')ColorID,
        		                        ISNULL(bo.BuyerOrderID,'')BuyerOrderID,

        		                        ISNULL(cqs.ArticleNo,0)ArticleNo,
        		                        ISNULL(cqs.ArticleChallanNo,0)ArticleChallanNo,
                                        ISNULL(cqs.StoreID,0)StoreID,
										ISNULL(b.BuyerName,'')BuyerName,
        		                        ISNULL(a.ArticleName,'')ArticleName,
        		                        ISNULL(s.StoreName,'')StoreName,
        		                        ISNULL(bo.BuyerOrderNo,0)BuyerOrderNo,
        		                        ISNULL(i.ItemTypeName,'')ItemTypeName,
        		                        ISNULL(l.LeatherStatusName,'')LeatherStatusName,
        		                        ISNULL(c.ColorName,'')ColorName,
        		                        ISNULL(g.GradeName,'')GradeName,
        		                        ISNULL(u.UnitName,'')UnitName,
        		                        ISNULL(cqs.FinishQCLabel,'')FinishQCLabel,
        		                        ISNULL(cqs.ClosingStockPcs,0)ClosingStockPcs,
        		                        ISNULL(cqs.ClosingStockSide,0)ClosingStockSide,
        		                        ISNULL(cqs.ClosingStockArea,0)ClosingStockArea,
        		                        ISNULL(cqs.ClosingStockAreaUnit,'')ClosingStockAreaUnit
                                FROM	INV_FinishQCStock cqs 
        		                        LEFT JOIN INV_FNTransferFrom f ON f.StoreID=cqs.StoreID
        		                        LEFT JOIN INV_FNTransfer ft ON ft.TransactionStore=cqs.StoreID
        		                        LEFT JOIN SYS_Store s ON s.StoreID=cqs.StoreID
        		                        LEFT JOIN Sys_Article a ON a.ArticleID=cqs.ArticleID
        		                        LEFT JOIN Sys_Color c ON c.ColorID=cqs.ColorID
        		                        LEFT JOIN Sys_Buyer b ON b.BuyerID=cqs.BuyerID
        		                        LEFT JOIN SLS_BuyerOrder bo ON bo.BuyerOrderID=cqs.BuyerOrderID
        		                        LEFT JOIN Sys_Grade g ON g.GradeID=cqs.GradeID
        		                        LEFT JOIN Sys_Unit u ON u.UnitID=cqs.ClosingStockAreaUnit
        		                        LEFT JOIN Sys_ItemType i ON i.ItemTypeID=cqs.ItemTypeID
        		                        LEFT JOIN Sys_LeatherStatus l ON l.LeatherStatusID=cqs.LeatherStatusID
                                WHERE	cqs.StoreID = " + TransactionStore + " and cqs.BuyerID = " + BuyerID + " and cqs.ArticleID = " + ArticleID + " and cqs.BuyerOrderID = " + BuyerOrderID + "and cqs.ItemTypeID = " + ItemTypeID + " and cqs.LeatherStatusID = " + LeatherStatusID + " and cqs.ClosingStockPcs>0 ORDER BY cqs.BuyerID DESC";



                var allData = _context.Database.SqlQuery<InvFNTransferFrom>(queryString).ToList();
                List<InvFNTransferFrom> searchList = allData.ToList();
                return searchList.Select(c => SetToFinishQCStockBussinessObject(c)).ToList<InvFNTransferFrom>();
            }
            return null;

        }
        public long GetFNTransferID()
        {
            return FNTransferID;
        }
        public string GetFNTransferNo()
        {
            return FNTransferNo;
        }

        //public object GetSearchInformation()
        //{
        //    using (_context)
        //    {
        //        var Data = (from r in _context.INV_FNTransfer.AsEnumerable()
        //                    //where r.RecordStatus == "NCF"

        //                    join s in _context.INV_FNTransferFrom on r.FNTransferID equals s.FNTransferID into FinishTransferFrom
        //                    from s in FinishTransferFrom.DefaultIfEmpty()

        //                    join y in _context.INV_FNTransferTo on (s == null ? null : s.FNTransferID) equals y.FNTransferID into FinishTransferTo
        //                    from y in FinishTransferTo.DefaultIfEmpty()

        //                    join st in _context.Sys_Buyer on (s == null ? null : s.BuyerID) equals st.BuyerID into Buyers
        //                    from st in Buyers.DefaultIfEmpty()

        //                    join c in _context.SLS_BuyerOrder on (s == null ? null : s.BuyerOrderID) equals c.BuyerOrderID into BuyerOrders
        //                    from c in BuyerOrders.DefaultIfEmpty()


        //                    select new 
        //                    {
        //                        FNTransferID = r.FNTransferID,
        //                        FNTransferFromID = s.FNTransferFromID,
        //                        FNTransferToID = y.FNTransferToID,
        //                        FNTransferNo = r.FNTransferNo,

        //                        FNTransferCategory = r.FNTransferCategory,
        //                        FNTransferDate = Convert.ToDateTime(r.FNTransferDate).ToString(),
        //                        TranrsferType = r.TranrsferType,
        //                        TransactionStore = r.TransactionStore,
        //                        IssueStore = r.IssueStore,
        //                        RecordStatus = (r.RecordStatus == "NCF" ? "Not Confirmed" : "Confirmed"),
        //                        BuyerID = (s == null ? 0 : s.BuyerID),
        //                        BuyerOrderID = (s == null ? 0 : s.BuyerOrderID),
        //                        //ArticleID = (s == null ? 0 :s.ArticleID),
        //                        ArticleNo = s.ArticleNo,
        //                        BuyerName = (st == null ? null : st.BuyerName),
        //                        BuyerOrderNo = (c == null ? null : c.BuyerOrderNo),

        //                    }).ToList();

        //        return Data;
        //    }
        //}

        public object GetMasterGridItemList(string _FNTransferID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var ScheduleItem = (from i in context.INV_FNTransferFrom.AsEnumerable()
                                    where i.FNTransferID.ToString() == _FNTransferID
                                    //join d in YearMonthScheduleDate on i.ScheduleDateID equals d.ScheduleDateID into Items
                                    //from d in Items.DefaultIfEmpty()

                                    join b in context.Sys_Buyer on i.BuyerID equals b.BuyerID into Buyers
                                    from b in Buyers.DefaultIfEmpty()

                                    join o in context.SLS_BuyerOrder on i.BuyerOrderID equals o.BuyerOrderID into Orders
                                    from o in Orders.DefaultIfEmpty()

                                    join a in context.Sys_Article on i.ArticleID equals a.ArticleID into Articles
                                    from a in Articles.DefaultIfEmpty()

                                    join it in context.Sys_ItemType on i.ItemTypeID equals it.ItemTypeID into ItemTypes
                                    from it in ItemTypes.DefaultIfEmpty()

                                    join ls in context.Sys_LeatherStatus on i.LeatherStatusID equals ls.LeatherStatusID into LeatherStatus
                                    from ls in LeatherStatus.DefaultIfEmpty()

                                    join u in context.Sys_Unit on i.AreaUnit equals u.UnitID into Units
                                    from u in Units.DefaultIfEmpty()

                                    join tu in context.Sys_Color on i.ColorID equals tu.ColorID into Colors
                                    from tu in Colors.DefaultIfEmpty()

                                    join g in context.Sys_Grade on i.GradeID equals g.GradeID into Grades
                                    from g in Grades.DefaultIfEmpty()


                                    select new
                                    {

                                        FNTransferID = i.FNTransferID,
                                        FNTransferNo = i.FNTransferNo,
                                        FNTransferFromID = i.FNTransferFromID,

                                        BuyerID = i.BuyerID,
                                        BuyerName = (i == null ? null : b.BuyerName),
                                        BuyerOrderID = i.BuyerOrderID,
                                        BuyerOrderNo = (i == null ? null : o.BuyerOrderNo),
                                        ArticleID = i.ArticleID,
                                        ArticleNo = i.ArticleNo,
                                        ArticleChallanNo = i.ArticleChallanNo,
                                        ArticleName = (a == null ? null : a.ArticleName),
                                        ItemTypeID = i.ItemTypeID,
                                        ItemTypeName = (i == null ? "" : it.ItemTypeName),
                                        LeatherStatusID = i.LeatherStatusID,
                                        LeatherStatusName = (i == null ? "" : ls.LeatherStatusName),
                                        ColorID = i.ColorID,
                                        ColorName = (i == null ? "" : tu.ColorName),
                                        GradeID = i.GradeID,
                                        GradeName = (i == null ? "" : g.GradeName),
                                        FinishQCLabel = i.FinishQCLabel,
                                        AreaUnit = i.AreaUnit,
                                        UnitName = (u == null ? "" : u.UnitName),
                                        QCStockPcs = i.QCStockPcs,
                                        QCStockSide = i.QCStockSide,
                                        QCStockArea = i.QCStockArea
                                    }).ToList();

                return ScheduleItem;
            }

        }
        public object GetChildGridItemListForMasterGridRow(string _FNTransferFromID)
        {
            using (var context = new BLC_DEVEntities())
            {
                var Data = (from i in context.INV_FNTransferTo.AsEnumerable()
                            where i.FNTransferFromID.ToString() == _FNTransferFromID

                            join b in context.Sys_Buyer on i.BuyerID equals b.BuyerID into Buyers
                            from b in Buyers.DefaultIfEmpty()

                            join o in context.SLS_BuyerOrder on i.BuyerOrderID equals o.BuyerOrderID into Orders
                            from o in Orders.DefaultIfEmpty()

                            join a in context.Sys_Article on i.ArticleID equals a.ArticleID into Articles
                            from a in Articles.DefaultIfEmpty()

                            join it in context.Sys_ItemType on i.ItemTypeID equals it.ItemTypeID into ItemTypes
                            from it in ItemTypes.DefaultIfEmpty()

                            join ls in context.Sys_LeatherStatus on i.LeatherStatusID equals ls.LeatherStatusID into LeatherStatus
                            from ls in LeatherStatus.DefaultIfEmpty()

                            join u in context.Sys_Unit on i.AreaUnit equals u.UnitID into Units
                            from u in Units.DefaultIfEmpty()

                            join tu in context.Sys_Color on i.ColorID equals tu.ColorID into Colors
                            from tu in Colors.DefaultIfEmpty()

                            join g in context.Sys_Grade on i.GradeID equals g.GradeID into Grades
                            from g in Grades.DefaultIfEmpty()

                            select new
                            {
                                FNTransferID = i.FNTransferID,
                                FNTransferNo = i.FNTransferNo,
                                FNTransferFromID = i.FNTransferFromID,
                                FNTransferToID = i.FNTransferToID,

                                BuyerID = i.BuyerID,
                                BuyerName = (i == null ? null : b.BuyerName),
                                BuyerOrderID = i.BuyerOrderID,
                                BuyerOrderNo = (i == null ? null : o.BuyerOrderNo),
                                ArticleID = i.ArticleID,
                                ArticleNo = i.ArticleNo,
                                ArticleChallanNo = i.ArticleChallanNo,
                                ArticleName = (a == null ? null : a.ArticleName),
                                ItemTypeID = i.ItemTypeID,
                                ItemTypeName = (i == null ? "" : it.ItemTypeName),
                                LeatherStatusID = i.LeatherStatusID,
                                LeatherStatusName = (i == null ? "" : ls.LeatherStatusName),
                                ColorID = i.ColorID,
                                ColorName = (i == null ? "" : tu.ColorName),
                                GradeID = i.GradeID,
                                GradeName = (i == null ? "" : g.GradeName),
                                FinishQCLabel = i.FinishQCLabel,
                                AreaUnit = i.AreaUnit,
                                UnitName = (u == null ? "" : u.UnitName),
                                ToStockPcs = i.ToStockPcs,
                                ToStockSide = i.ToStockSide,
                                ToStockArea = i.ToStockArea,


                            }).ToList();

                return Data;
            }
        }
        public List<InvFNTransferFrom> GetFNTransferFromAfterSave(string _FNTransferID)
        {
            var fnTransferID = Convert.ToInt64(_FNTransferID);
            List<INV_FNTransferFrom> searchList = _context.INV_FNTransferFrom.Where(m => m.FNTransferID == fnTransferID).OrderByDescending(m => m.FNTransferFromID).ToList();
            return searchList.Select(c => SetToFNTransferFromBusinessObject(c)).ToList<InvFNTransferFrom>();
        }
        public List<InvFNTransferTo> GetFNTransferToAfterSave(string _FNTransferFromID)
        {
            var fnTransferFromID = Convert.ToInt64(_FNTransferFromID);
            List<INV_FNTransferTo> searchList = _context.INV_FNTransferTo.Where(m => m.FNTransferFromID == fnTransferFromID).OrderByDescending(m => m.FNTransferToID).ToList();
            return searchList.Select(c => SetToFNTransferToBusinessObject(c)).ToList<InvFNTransferTo>();
        }



        public INV_FNTransfer SetToFNTransferModelObject(InvFNTransfer model, int userid)
        {
            INV_FNTransfer Entity = new INV_FNTransfer();

            Entity.FNTransferID = model.FNTransferID;
            Entity.FNTransferNo = model.FNTransferNo;
            Entity.FNTransferDate = DalCommon.SetDate(model.FNTransferDate);
            Entity.FNTransferCategory = model.FNTransferCategory;
            Entity.TranrsferType = model.TranrsferType;
            Entity.TransactionStore = model.TransactionStore;
            Entity.IssueStore = model.IssueStore;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            // Entity.SetBy = userid;
            Entity.ModifiedOn = DateTime.Now;
            Entity.ModifiedBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }
        public INV_FNTransferFrom SetToFNTransferFromModelObject(InvFNTransferFrom model, int userid)
        {
            INV_FNTransferFrom Entity = new INV_FNTransferFrom();


            Entity.FNTransferID = model.FNTransferID;
            Entity.FNTransferNo = model.FNTransferNo;
            Entity.FNTransferFromID = model.FNTransferFromID;
            Entity.StoreID = model.StoreID;
            Entity.BuyerID = model.BuyerID;
            Entity.BuyerOrderID = model.BuyerOrderID;
            Entity.ArticleID = model.ArticleID;
            Entity.ArticleNo = model.ArticleNo;
            Entity.ColorID = model.ColorID;
            Entity.ItemTypeID = model.ItemTypeID;
            Entity.LeatherTypeID = model.LeatherTypeID;
            Entity.LeatherStatusID = model.LeatherStatusID;
            Entity.FinishQCLabel = model.FinishQCLabel;
            Entity.GradeID = model.GradeID;
            Entity.QCStockPcs = model.ClosingStockPcs;
            Entity.QCStockSide = model.ClosingStockSide;
            Entity.QCStockArea = model.ClosingStockArea;
            Entity.AreaUnit = model.ClosingStockAreaUnit;
            Entity.ArticleChallanNo = model.ArticleChallanNo;
            //Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            //Entity.SetBy = userid;
            Entity.ModifiedOn = DateTime.Now;
            // Entity.ModifiedBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }
        public INV_FNTransferTo SetToFNTransferToModelObject(InvFNTransferTo model, int userid)
        {
            INV_FNTransferTo Entity = new INV_FNTransferTo();


            Entity.FNTransferID = model.FNTransferID;
            Entity.FNTransferNo = model.FNTransferNo;
            Entity.FNTransferFromID = model.FNTransferFromID;
            Entity.FNTransferToID = model.FNTransferToID;
            Entity.StoreID = model.StoreID;
            Entity.BuyerID = model.BuyerID;
            Entity.BuyerOrderID = model.BuyerOrderID;
            Entity.ArticleID = model.ArticleID;
            Entity.ArticleNo = model.ArticleNo;
            Entity.ColorID = model.ColorID;
            Entity.ItemTypeID = model.ItemTypeID;
            Entity.LeatherTypeID = model.LeatherTypeID;
            Entity.LeatherStatusID = model.LeatherStatusID;
            Entity.FinishQCLabel = model.FinishQCLabel;
            Entity.GradeID = model.GradeID;
            Entity.ToStockPcs = model.ToStockPcs;
            Entity.ToStockSide = model.ToStockSide;
            Entity.ToStockArea = model.ToStockArea;
            Entity.AreaUnit = model.AreaUnit;
            Entity.ArticleChallanNo = model.ArticleChallanNo;
            //Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            //Entity.SetBy = userid;
            Entity.ModifiedOn = DateTime.Now;
            // Entity.ModifiedBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }


        public InvFNTransfer SetToFNTransferBusinessObject(INV_FNTransfer Entity)
        {
            InvFNTransfer Model = new InvFNTransfer();

            Model.FNTransferID = Entity.FNTransferID;
            Model.FNTransferNo = Entity.FNTransferNo;
            Model.FNTransferDate = string.IsNullOrEmpty(Entity.FNTransferDate.ToString()) ? string.Empty : Convert.ToDateTime(Entity.FNTransferDate).ToString("dd/MM/yyyy");
            Model.FNTransferCategory = Entity.FNTransferCategory;
            Model.TranrsferType = Entity.TranrsferType;
            Model.TransactionStore = Entity.TransactionStore;
            Model.StoreName = Entity.TransactionStore == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.TransactionStore).FirstOrDefault().StoreName;
            Model.IssueStore = Entity.IssueStore;
            Model.StoreName = Entity.IssueStore == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.IssueStore).FirstOrDefault().StoreName;
            return Model;
        }
        public InvFNTransferFrom SetToFNTransferFromBusinessObject(INV_FNTransferFrom Entity)
        {
            InvFNTransferFrom Model = new InvFNTransferFrom();

            Model.FNTransferID = Entity.FNTransferID;
            Model.FNTransferNo = Entity.FNTransferNo;
            Model.FNTransferFromID = Entity.FNTransferFromID;
            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.StoreID = Entity.StoreID;
            Model.StoreName = Entity.StoreID == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.StoreID).FirstOrDefault().StoreName;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            Model.GradeID = Entity.GradeID;
            Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.FinishQCLabel = Entity.FinishQCLabel;
            Model.ClosingStockPcs = Entity.QCStockPcs;
            Model.ClosingStockSide = Entity.QCStockSide;
            Model.ClosingStockArea = Entity.QCStockArea;
            Model.ClosingStockAreaUnit = Entity.AreaUnit;
            Model.UnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;

            return Model;
        }
        public InvFNTransferTo SetToFNTransferToBusinessObject(INV_FNTransferTo Entity)
        {
            InvFNTransferTo Model = new InvFNTransferTo();

            Model.FNTransferID = Entity.FNTransferID;
            Model.FNTransferNo = Entity.FNTransferNo;
            Model.FNTransferFromID = Entity.FNTransferFromID;
            Model.FNTransferToID = Entity.FNTransferToID;
            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.StoreID = Entity.StoreID;
            Model.StoreName = Entity.StoreID == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.StoreID).FirstOrDefault().StoreName;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            Model.GradeID = Entity.GradeID;
            Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.FinishQCLabel = Entity.FinishQCLabel;
            Model.ToStockPcs = Entity.ToStockPcs;
            Model.ToStockSide = Entity.ToStockSide;
            Model.ToStockArea = Entity.ToStockArea;
            Model.AreaUnit = Entity.AreaUnit;
            Model.UnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;

            return Model;
        }

        public InvFNTransferFrom SetToFinishQCStockBussinessObject(InvFNTransferFrom Entity)
        {
            InvFNTransferFrom Model = new InvFNTransferFrom();

            Model.FNTransferID = Entity.FNTransferID;
            Model.FNTransferNo = Entity.FNTransferNo;
            Model.FNTransferFromID = Entity.FNTransferFromID;
            Model.StoreID = Entity.StoreID;
            Model.StoreName = Entity.StoreID == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.StoreID).FirstOrDefault().StoreName;
            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;

            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            //Model.LeatherTypeName = Entity.LeatherTypeID == null ? "" : _context.Sys_LeatherType.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            Model.GradeID = Entity.GradeID;
            Model.FinishQCLabel = Entity.FinishQCLabel;
            Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.ClosingStockPcs = Entity.ClosingStockPcs;
            Model.ClosingStockSide = Entity.ClosingStockSide;
            Model.ClosingStockArea = Entity.ClosingStockArea;
            Model.ClosingStockAreaUnit = Entity.ClosingStockAreaUnit;
            Model.UnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;
            // //Model.UnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;


            // Model.QCStockPcs = Entity.QCStockPcs == null ? 0 : _context.INV_FinishQCStock.Where(m => m.StoreID == Entity.StoreID).FirstOrDefault().ClosingStockPcs;
            // Model.QCStockSide = Entity.QCStockSide ==null ? 0 : _context.INV_FinishQCStock.Where(m => m.StoreID == Entity.StoreID).FirstOrDefault().ClosingStockPcs;
            // Model.QCStockArea = Entity.QCStockArea == null ? 0 : _context.INV_FinishQCStock.Where(m => m.StoreID == Entity.StoreID).FirstOrDefault().ClosingStockPcs;
            //// Model.AreaUnit = Entity.AreaUnit == _context.INV_FinishQCStock.Where(m => m.StoreID == Entity.StoreID).FirstOrDefault().ClosingStockAreaUnit;
            //// Model.IssueUnitName = Entity.AreaUnitID == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnitID).FirstOrDefault().UnitName;

            return Model;
        }



        public ValidationMsg Save(InvFNTransfer model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {

                        model.FNTransferNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.FNTransferNo != null)
                        {
                            #region Save Finish Transfer

                            INV_FNTransfer objTblFNTransfer = SetToFNTransferModelObject(model, userid);
                            _context.INV_FNTransfer.Add(objTblFNTransfer);
                            _context.SaveChanges();

                            #endregion

                            #region Save Finish Transfer From & FInish Transfer To

                            if (model.InvFNTransferFromList != null)
                            {
                                foreach (InvFNTransferFrom objfnTransferForm in model.InvFNTransferFromList)
                                {
                                    objfnTransferForm.FNTransferID = objTblFNTransfer.FNTransferID;
                                    INV_FNTransferFrom objTblFNTransferFrom = SetToFNTransferFromModelObject(objfnTransferForm, userid);
                                    _context.INV_FNTransferFrom.Add(objTblFNTransferFrom);
                                    //    _context.SaveChanges();

                                    #region Save Finish Transfer To List List

                                    if (model.InvFNTransferToList != null)
                                    {
                                        foreach (InvFNTransferTo objfnTransferTo in model.InvFNTransferToList)
                                        {
                                            objfnTransferTo.FNTransferID = objTblFNTransfer.FNTransferID;
                                            objfnTransferTo.FNTransferNo = objTblFNTransfer.FNTransferNo;
                                            objfnTransferTo.FNTransferFromID = objTblFNTransferFrom.FNTransferFromID;
                                            // objfnTransferTo.ScheduleProductionNo = objQCItem.ScheduleProductionNo;
                                            INV_FNTransferTo objTblFNTransferTo = SetToFNTransferToModelObject(objfnTransferTo, userid);
                                            _context.INV_FNTransferTo.Add(objTblFNTransferTo);
                                        }
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            _context.SaveChanges();
                            tx.Complete();
                            FNTransferID = objTblFNTransfer.FNTransferID;
                            FNTransferNo = objTblFNTransfer.FNTransferNo;

                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Finish Leather No Predefine Value not Found.";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Finish Leather No Data Already Exit.";
                }
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }
        public ValidationMsg Update(InvFNTransfer model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region Finish Leather Transfer Update

                        INV_FNTransfer CurrentEntity = SetToFNTransferModelObject(model, userid);
                        var OriginalEntity = _context.INV_FNTransfer.First(m => m.FNTransferID == model.FNTransferID);

                        OriginalEntity.FNTransferCategory = CurrentEntity.FNTransferCategory;
                        OriginalEntity.TranrsferType = CurrentEntity.TranrsferType;
                        OriginalEntity.TransactionStore = CurrentEntity.TransactionStore;
                        OriginalEntity.IssueStore = CurrentEntity.IssueStore;
                        OriginalEntity.FNTransferDate = CurrentEntity.FNTransferDate;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #endregion


                        #region Save Finish Transfer From Item List

                        if (model.InvFNTransferFromList != null)
                        {
                            foreach (InvFNTransferFrom objFNTransferFromItem in model.InvFNTransferFromList)
                            {
                                if (objFNTransferFromItem.FNTransferFromID == 0)
                                {
                                    objFNTransferFromItem.FNTransferID = model.FNTransferID;                 // confusion ase
                                    INV_FNTransferFrom tblFNTransferFromItem = SetToFNTransferFromModelObject(objFNTransferFromItem, userid);
                                    _context.INV_FNTransferFrom.Add(tblFNTransferFromItem);
                                    _context.SaveChanges();
                                    objFNTransferFromItem.FNTransferFromID = tblFNTransferFromItem.FNTransferFromID;
                                }
                                else
                                {
                                    INV_FNTransferFrom CurrEntity = SetToFNTransferFromModelObject(objFNTransferFromItem, userid);
                                    var OrgrEntity = _context.INV_FNTransferFrom.First(m => m.FNTransferFromID == objFNTransferFromItem.FNTransferFromID);

                                    OrgrEntity.FNTransferID = CurrEntity.FNTransferID;
                                    OrgrEntity.FNTransferNo = CurrEntity.FNTransferNo;
                                    OrgrEntity.FNTransferFromID = CurrEntity.FNTransferFromID;

                                    OrgrEntity.BuyerID = CurrEntity.BuyerID;
                                    OrgrEntity.BuyerOrderID = CurrEntity.BuyerOrderID;
                                    OrgrEntity.ArticleID = CurrEntity.ArticleID;
                                    OrgrEntity.ArticleNo = CurrEntity.ArticleNo;
                                    OrgrEntity.ColorID = CurrEntity.ColorID;
                                    OrgrEntity.ItemTypeID = CurrEntity.ItemTypeID;
                                    OrgrEntity.LeatherTypeID = CurrEntity.LeatherTypeID;
                                    OrgrEntity.LeatherStatusID = CurrEntity.LeatherStatusID;
                                    OrgrEntity.GradeID = CurrEntity.GradeID;
                                    OrgrEntity.FinishQCLabel = CurrEntity.FinishQCLabel;
                                    OrgrEntity.ArticleChallanNo = CurrEntity.ArticleChallanNo;
                                    OrgrEntity.QCStockPcs = CurrEntity.QCStockPcs;
                                    OrgrEntity.QCStockSide = CurrEntity.QCStockSide;
                                    OrgrEntity.QCStockArea = CurrEntity.QCStockArea;
                                    OrgrEntity.AreaUnit = CurrEntity.AreaUnit;

                                    OrgrEntity.ModifiedBy = userid;
                                    OrgrEntity.ModifiedOn = DateTime.Now;
                                }

                                #region Save Finish Transfer To Records

                                if (model.InvFNTransferToList != null)
                                {
                                    foreach (InvFNTransferTo objFNTransferToItem in model.InvFNTransferToList)
                                    {
                                        if (objFNTransferToItem.FNTransferToID == 0)
                                        {
                                            objFNTransferToItem.FNTransferFromID = objFNTransferFromItem.FNTransferFromID;
                                            objFNTransferToItem.FNTransferID = model.FNTransferID;
                                            objFNTransferToItem.FNTransferNo = model.FNTransferNo;


                                            INV_FNTransferTo tblFNTransferTo = SetToFNTransferToModelObject(objFNTransferToItem, userid);
                                            _context.INV_FNTransferTo.Add(tblFNTransferTo);
                                        }
                                        else
                                        {
                                            INV_FNTransferTo CurEntity = SetToFNTransferToModelObject(objFNTransferToItem, userid);
                                            var OrgEntity = _context.INV_FNTransferTo.First(m => m.FNTransferToID == objFNTransferToItem.FNTransferToID);

                                            OrgEntity.FNTransferID = CurEntity.FNTransferID;
                                            OrgEntity.FNTransferNo = CurEntity.FNTransferNo;
                                            OrgEntity.FNTransferFromID = CurEntity.FNTransferFromID;

                                            OrgEntity.BuyerID = CurEntity.BuyerID;
                                            OrgEntity.BuyerOrderID = CurEntity.BuyerOrderID;
                                            OrgEntity.ArticleID = CurEntity.ArticleID;
                                            OrgEntity.ArticleNo = CurEntity.ArticleNo;
                                            OrgEntity.ColorID = CurEntity.ColorID;
                                            OrgEntity.ItemTypeID = CurEntity.ItemTypeID;
                                            OrgEntity.LeatherTypeID = CurEntity.LeatherTypeID;
                                            OrgEntity.LeatherStatusID = CurEntity.LeatherStatusID;
                                            OrgEntity.GradeID = CurEntity.GradeID;
                                            OrgEntity.FinishQCLabel = CurEntity.FinishQCLabel;
                                            OrgEntity.ArticleChallanNo = CurEntity.ArticleChallanNo;
                                            OrgEntity.ToStockPcs = CurEntity.ToStockPcs;
                                            OrgEntity.ToStockSide = CurEntity.ToStockSide;
                                            OrgEntity.ToStockArea = CurEntity.ToStockArea;
                                            OrgEntity.AreaUnit = CurEntity.AreaUnit;

                                            OrgEntity.ModifiedBy = userid;
                                            OrgEntity.ModifiedOn = DateTime.Now;
                                        }
                                    }
                                }

                                #endregion
                            }
                        }

                        #endregion

                        _context.SaveChanges();
                        tx.Complete();
                        FNTransferID = model.FNTransferID;
                        FNTransferNo = model.FNTransferNo;
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

        //public ValidationMsg DeletedFinishLeatherTransferItem(string FNTransferNo, string RecordStatus)//DeletedCrustYearMonthSchedule
        //{
        //    _vmMsg = new ValidationMsg();
        //    try
        //    {
        //        var FNTransferID = _context.INV_FNTransfer.Where(m => m.FNTransferNo == FNTransferNo).FirstOrDefault().FNTransferID;
        //        var FNTransferFromList = _context.INV_FNTransferFrom.Where(m => m.FNTransferID == FNTransferID).ToList();
        //        if (FNTransferFromList.Count > 0)
        //        {
        //            _vmMsg.Type = Enums.MessageType.Error;
        //            _vmMsg.Msg = "Child Record Found.";
        //        }
        //        else
        //        {
        //            var deleteFNTransferData = _context.INV_FNTransfer.First(m => m.FNTransferID == FNTransferID);
        //            _context.INV_FNTransfer.Remove(deleteFNTransferData);
        //            _context.SaveChanges();

        //            _vmMsg.Type = Enums.MessageType.Success;
        //            _vmMsg.Msg = "Deleted Successfully.";
        //        }
        //    }
        //    catch
        //    {
        //        _vmMsg.Type = Enums.MessageType.Error;
        //        _vmMsg.Msg = "Failed to Delete.";
        //    }
        //    return _vmMsg;
        //}
        //public ValidationMsg DeletedFinishLeatherTransferFromItem(long _FNTransferFromID)//DeletedCrustScheduleColor
        //{
        //    _vmMsg = new ValidationMsg();
        //    try
        //    {
        //        if (RecordStatus != "CNF")
        //        {
        //            var FinishTransferFromList = _context.INV_FNTransferFrom.Where(m => m.FNTransferFromID == _FNTransferFromID).ToList();
        //            if (FinishTransferFromList.Count > 0)
        //            {
        //                _vmMsg.Type = Enums.MessageType.Error;
        //                _vmMsg.Msg = "Child Record Found.";
        //            }
        //            else
        //            {
        //                var deleteElement = _context.INV_FNTransferFrom.First(m => m.FNTransferFromID == _FNTransferFromID);
        //                _context.INV_FNTransferFrom.Remove(deleteElement);
        //                _context.SaveChanges();

        //                _vmMsg.Type = Enums.MessageType.Success;
        //                _vmMsg.Msg = "Deleted Successfully.";
        //            }
        //        }
        //        else
        //        {
        //            _vmMsg.Type = Enums.MessageType.Error;
        //            _vmMsg.Msg = "Confirmed Record Can not be Deleted.";
        //        }
        //    }
        //    catch
        //    {
        //        _vmMsg.Type = Enums.MessageType.Error;
        //        _vmMsg.Msg = "Failed to Delete.";
        //    }
        //    return _vmMsg;
        //}
        //public ValidationMsg DeletedFinishLeatherTransferToItem(long _FNTransferToID)//DeletedScheduleDrum
        //{
        //    _vmMsg = new ValidationMsg();
        //    try
        //    {
        //        if (RecordStatus != "CNF")
        //        {
        //            var deleteElement = _context.INV_FNTransferTo.First(m => m.FNTransferToID == _FNTransferToID);
        //            _context.INV_FNTransferTo.Remove(deleteElement);

        //            _context.SaveChanges();
        //            _vmMsg.Type = Enums.MessageType.Success;
        //            _vmMsg.Msg = "Deleted Successfully.";
        //        }
        //        else
        //        {
        //            _vmMsg.Type = Enums.MessageType.Error;
        //            _vmMsg.Msg = "Confirmed Record Can not be Deleted.";
        //        }
        //    }
        //    catch
        //    {
        //        _vmMsg.Type = Enums.MessageType.Error;
        //        _vmMsg.Msg = "Failed to Delete.";
        //    }
        //    return _vmMsg;
        //}




    }
}