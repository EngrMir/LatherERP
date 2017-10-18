using System;
using System.Text;
using System.Web;
using System.Data;
using System.Linq;
using DatabaseUtility;
using System.Transactions;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.AppSetupModel;


namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalCrustedLeatherTransferTreatment
    {
        private UnitOfWork repository = new UnitOfWork();
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        

        public long CLTransferFromID = 0;
        public long CLTransferToID = 0;
        public long CLTransferID = 0;
        public string CLTransferNo = string.Empty;
        private bool save;
        private int stockOver = 0;
        private readonly string _connString;


        public DalCrustedLeatherTransferTreatment()
        {
            _vmMsg = new ValidationMsg();
            _context = new BLC_DEVEntities();
        }


        #region Get All ID Value
        public long GetCLTransferID()
        {
            return CLTransferID;
        }

        public long GetCLTransferToID()
        {
            return CLTransferToID;
        }

        public long GetCLTransferFromID()
        {
            return CLTransferFromID;
        }

        public string GetCLTransferNo()
        {
            return CLTransferNo;
        }
        #endregion

        #region Get All QC Stock Data on Selected Store From Popup

        //Retrive Data From QC Stock Depend on Selected Store 
        public List<clTransfer> GetCrustTransferPopUpGridDetail(string TransactionStore)
        {
            if (!string.IsNullOrEmpty(TransactionStore))
            {
                var queryString = @"SELECT	
		                                    inv.TransectionID,
                                            inv.BuyerID BuyerID,
	                                        ISNULL((select BuyerName from Sys_Buyer where BuyerID = inv.BuyerID),'')BuyerName,
                                            inv.BuyerOrderID BuyerOrderID,
	                                        ISNULL((select BuyerOrderNo from SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID),'')BuyerOrderNo,
                                            ISNULL(inv.ItemTypeID,0)ItemTypeID,
	                                        ISNULL((select ItemTypeName from Sys_ItemType where ItemTypeID = inv.ItemTypeID),'')ItemTypeName,
                                            ISNULL(inv.LeatherTypeID,0)LeatherTypeID,
	                                        ISNULL((select LeatherTypeName from Sys_LeatherType where LeatherTypeID = inv.LeatherTypeID),'')LeatherTypeName,
                                            ISNULL(inv.LeatherStatusID,0)LeatherStatusID,
	                                        ISNULL((select LeatherStatusName from Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID),'')LeatherStatusName,
                                            ISNULL(inv.ArticleID,0)ArticleID,
	                                        ISNULL(inv.ArticleNo,'')ArticleNo,
											ISNULL(inv.ArticleChallanID,'')ArticleChallanID,
	                                        ISNULL(inv.ArticleChallanNo,'')ArticleChallanNo,
                                            ISNULL(inv.ArticleColorNo,'')ArticleColorNo,
	                                        ISNULL((select ArticleName from Sys_Article where ArticleID = inv.ArticleID),'')ArticleName,
                                            ISNULL(inv.ColorID,0)ColorID,
	                                        ISNULL(inv.GradeID,0)GradeID,
	                                        ISNULL(inv.GradeRange,0)GradeRange,
	                                        ISNULL((select ColorName from Sys_Color where ColorID = inv.ColorID),'')ColorName,
	                                        ISNULL(inv.CrustQCLabel,'')CrustQCLabel,
                                            ISNULL(inv.ClosingStockPcs,0)ClosingStockPcs,
	                                        ISNULL(inv.ClosingStockSide,0)ClosingStockSide,
	                                        ISNULL(inv.ClosingStockArea,0)ClosingStockArea,
	                                        ISNULL(inv.ClosingStockAreaUnit,0)ClosingStockAreaUnit,
											ISNULL((select UnitName from Sys_Unit where UnitID = inv.ClosingStockAreaUnit),'')UnitName

                                    FROM	dbo.INV_CrustQCStock inv
									INNER JOIN (SELECT		MAX(TransectionID)TransectionID,
															StoreID,
															BuyerID,
															BuyerOrderID,
															ArticleID,
															ArticleChallanID, 	
															ItemTypeID,
															LeatherTypeID,
															LeatherStatusID,
															ColorID
															
					                            FROM		INV_CrustQCStock
                                                GROUP BY	StoreID,BuyerID,BuyerOrderID,ArticleID,ArticleChallanID,ItemTypeID,LeatherTypeID,LeatherStatusID,ColorID) sup
									ON inv.TransectionID=sup.TransectionID
                                    WHERE	inv.StoreID = '" + TransactionStore + "'  and	inv.ClosingStockPcs>0  ORDER BY	inv.TransectionID DESC";
                var allData = _context.Database.SqlQuery<clTransfer>(queryString).ToList();
                return allData.ToList();
            }
            return null;
        }

        //Put Data into Mater Item Grid According To Combination of QC Stock
        public List<clTransferForm> GetCrustTransferMasterGridDetail(string TransactionStore, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ColorID,string ArticleChallanID)
        {
            if (!string.IsNullOrEmpty(TransactionStore))
            {
                if (!string.IsNullOrEmpty(BuyerID))
                {
                    if (string.IsNullOrEmpty(BuyerOrderID))
                    {
                        var query = @"SELECT						
									inv.TransectionID,
                                    inv.BuyerID BuyerID,
	                                ISNULL((SELECT BuyerName FROM Sys_Buyer WHERE BuyerID = inv.BuyerID),'')BuyerName,
                                    inv.BuyerOrderID BuyerOrderID,
	                                ISNULL((SELECT BuyerOrderNo FROM SLS_BuyerOrder WHERE BuyerOrderID = inv.BuyerOrderID),'')BuyerOrderNo,
                                    ISNULL(inv.ItemTypeID,0)ItemTypeID,
	                                ISNULL((SELECT ItemTypeName FROM Sys_ItemType WHERE ItemTypeID = inv.ItemTypeID),'')ItemTypeName,
                                    ISNULL(inv.LeatherTypeID,0)LeatherTypeID,
	                                ISNULL((SELECT LeatherTypeName FROM Sys_LeatherType WHERE LeatherTypeID = inv.LeatherTypeID),'')LeatherTypeName,
                                    ISNULL(inv.LeatherStatusID,0)LeatherStatusID,
	                                ISNULL((SELECT LeatherStatusName FROM Sys_LeatherStatus WHERE LeatherStatusID = inv.LeatherStatusID),'')LeatherStatusName,
                                    ISNULL(inv.ArticleID,0)ArticleID,
	                                ISNULL(inv.ArticleNo,'')ArticleNo,
									ISNULL(inv.ArticleChallanID,'')ArticleChallanID,
	                                ISNULL(inv.ArticleChallanNo,'')ArticleChallanNo,
                                    ISNULL(inv.ArticleColorNo,'')ArticleColorNo,
	                                ISNULL((SELECT ArticleName FROM Sys_Article WHERE ArticleID = inv.ArticleID),'')ArticleName,
                                    ISNULL(inv.ColorID,0)ColorID,
	                                ISNULL(inv.GradeID,0)GradeID,
	                                ISNULL(inv.GradeRange,0)GradeRange,
	                                ISNULL((SELECT ColorName FROM Sys_Color WHERE ColorID = inv.ColorID),'')ColorName,
									ISNULL((SELECT UnitName FROM Sys_Unit WHERE UnitID = inv.ClosingStockAreaUnit),'')UnitName,
	                                ISNULL(inv.CrustQCLabel,'')CrustQCLabel,

									ISNULL(inv.ClosingStockPcs,0)QCStockPcs,
									ISNULL(inv.ClosingStockSide,0)QCStockSide,
									ISNULL(inv.ClosingStockArea,0)QCStockArea,
									ISNULL(inv.ClosingStockAreaUnit,0)AreaUnit
				                    

                            FROM	dbo.INV_CrustQCStock inv
							INNER JOIN (SELECT		MAX(TransectionID)TransectionID,
													ISNULL(StoreID,0)StoreID,
													ISNULL(BuyerID,0)BuyerID,
													ISNULL(BuyerOrderID,0)BuyerOrderID,
													ISNULL(ArticleID,0)ArticleID,
													ISNULL(ArticleChallanID,0)ArticleChallanID,
													ISNULL(ItemTypeID,0)ItemTypeID,
													ISNULL(LeatherTypeID,0)LeatherTypeID,
													ISNULL(LeatherStatusID,0)LeatherStatusID,
													ISNULL(ColorID,0)ColorID
															
										FROM		INV_CrustQCStock
										GROUP BY	StoreID,BuyerID,BuyerOrderID,ArticleID,ArticleChallanID,ItemTypeID,LeatherTypeID,LeatherStatusID,ColorID) sup
                            ON			inv.TransectionID=sup.TransectionID 
							WHERE		inv.StoreID = " + TransactionStore +
                                       " AND inv.BuyerID = " + BuyerID +
                                       " AND inv.BuyerOrderID IS NULL AND inv.ArticleID = " + ArticleID +
                                       " AND inv.ArticleChallanID = " + ArticleChallanID +
                                       " AND inv.ItemTypeID = " + ItemTypeID +
                                       " AND inv.LeatherStatusID = " + LeatherStatusID +
                                       " AND inv.ColorID = " + ColorID +
                                       " AND inv.ClosingStockPcs>0";

                        var allData = _context.Database.SqlQuery<clTransferForm>(query).ToList();
                        return allData;

                    }
                    else
                    {
                        var query = @"SELECT						
									inv.TransectionID,
                                    inv.BuyerID BuyerID,
	                                ISNULL((SELECT BuyerName FROM Sys_Buyer WHERE BuyerID = inv.BuyerID),'')BuyerName,
                                    inv.BuyerOrderID BuyerOrderID,
	                                ISNULL((SELECT BuyerOrderNo FROM SLS_BuyerOrder WHERE BuyerOrderID = inv.BuyerOrderID),'')BuyerOrderNo,
                                    ISNULL(inv.ItemTypeID,0)ItemTypeID,
	                                ISNULL((SELECT ItemTypeName FROM Sys_ItemType WHERE ItemTypeID = inv.ItemTypeID),'')ItemTypeName,
                                    ISNULL(inv.LeatherTypeID,0)LeatherTypeID,
	                                ISNULL((SELECT LeatherTypeName FROM Sys_LeatherType WHERE LeatherTypeID = inv.LeatherTypeID),'')LeatherTypeName,
                                    ISNULL(inv.LeatherStatusID,0)LeatherStatusID,
	                                ISNULL((SELECT LeatherStatusName FROM Sys_LeatherStatus WHERE LeatherStatusID = inv.LeatherStatusID),'')LeatherStatusName,
                                    ISNULL(inv.ArticleID,0)ArticleID,
	                                ISNULL(inv.ArticleNo,'')ArticleNo,
									ISNULL(inv.ArticleChallanID,'')ArticleChallanID,
	                                ISNULL(inv.ArticleChallanNo,'')ArticleChallanNo,
                                    ISNULL(inv.ArticleColorNo,'')ArticleColorNo,
	                                ISNULL((SELECT ArticleName FROM Sys_Article WHERE ArticleID = inv.ArticleID),'')ArticleName,
                                    ISNULL(inv.ColorID,0)ColorID,
	                                ISNULL(inv.GradeID,0)GradeID,
	                                ISNULL(inv.GradeRange,0)GradeRange,
	                                ISNULL((SELECT ColorName FROM Sys_Color WHERE ColorID = inv.ColorID),'')ColorName,
									ISNULL((SELECT UnitName FROM Sys_Unit WHERE UnitID = inv.ClosingStockAreaUnit),'')UnitName,
	                                ISNULL(inv.CrustQCLabel,'')CrustQCLabel,

									ISNULL(inv.ClosingStockPcs,0)QCStockPcs,
									ISNULL(inv.ClosingStockSide,0)QCStockSide,
									ISNULL(inv.ClosingStockArea,0)QCStockArea,
									ISNULL(inv.ClosingStockAreaUnit,0)AreaUnit
				                    

                            FROM	dbo.INV_CrustQCStock inv
							INNER JOIN (SELECT		MAX(TransectionID)TransectionID,
													ISNULL(StoreID,0)StoreID,
													ISNULL(BuyerID,0)BuyerID,
													ISNULL(BuyerOrderID,0)BuyerOrderID,
													ISNULL(ArticleID,0)ArticleID,
													ISNULL(ArticleChallanID,0)ArticleChallanID,
													ISNULL(ItemTypeID,0)ItemTypeID,
													ISNULL(LeatherTypeID,0)LeatherTypeID,
													ISNULL(LeatherStatusID,0)LeatherStatusID,
													ISNULL(ColorID,0)ColorID
															
										FROM		INV_CrustQCStock
										GROUP BY	StoreID,BuyerID,BuyerOrderID,ArticleID,ArticleChallanID,ItemTypeID,LeatherTypeID,LeatherStatusID,ColorID) sup
                            ON			inv.TransectionID=sup.TransectionID 
							WHERE		inv.StoreID = " + TransactionStore +
                                        " AND inv.BuyerID = " + BuyerID +
                                        " AND inv.BuyerOrderID = " + BuyerOrderID +
                                        " AND inv.ArticleID = " + ArticleID +
                                        " AND inv.ArticleChallanID = " + ArticleChallanID +
                                        " AND inv.ItemTypeID = " + ItemTypeID +
                                        " AND inv.LeatherStatusID = " + LeatherStatusID +
                                        " AND inv.ColorID = " + ColorID +
                                        " AND inv.ClosingStockPcs>0";

                        var allData = _context.Database.SqlQuery<clTransferForm>(query).ToList();
                        return allData;
                    }
                }
                else
                {
                    var query = @"SELECT						
									inv.TransectionID,
                                    inv.BuyerID BuyerID,
	                                ISNULL((SELECT BuyerName FROM Sys_Buyer WHERE BuyerID = inv.BuyerID),'')BuyerName,
                                    inv.BuyerOrderID BuyerOrderID,
	                                ISNULL((SELECT BuyerOrderNo FROM SLS_BuyerOrder WHERE BuyerOrderID = inv.BuyerOrderID),'')BuyerOrderNo,
                                    ISNULL(inv.ItemTypeID,0)ItemTypeID,
	                                ISNULL((SELECT ItemTypeName FROM Sys_ItemType WHERE ItemTypeID = inv.ItemTypeID),'')ItemTypeName,
                                    ISNULL(inv.LeatherTypeID,0)LeatherTypeID,
	                                ISNULL((SELECT LeatherTypeName FROM Sys_LeatherType WHERE LeatherTypeID = inv.LeatherTypeID),'')LeatherTypeName,
                                    ISNULL(inv.LeatherStatusID,0)LeatherStatusID,
	                                ISNULL((SELECT LeatherStatusName FROM Sys_LeatherStatus WHERE LeatherStatusID = inv.LeatherStatusID),'')LeatherStatusName,
                                    ISNULL(inv.ArticleID,0)ArticleID,
	                                ISNULL(inv.ArticleNo,'')ArticleNo,
									ISNULL(inv.ArticleChallanID,'')ArticleChallanID,
	                                ISNULL(inv.ArticleChallanNo,'')ArticleChallanNo,
                                    ISNULL(inv.ArticleColorNo,'')ArticleColorNo,
	                                ISNULL((SELECT ArticleName FROM Sys_Article WHERE ArticleID = inv.ArticleID),'')ArticleName,
                                    ISNULL(inv.ColorID,0)ColorID,
	                                ISNULL(inv.GradeID,0)GradeID,
	                                ISNULL(inv.GradeRange,0)GradeRange,
	                                ISNULL((SELECT ColorName FROM Sys_Color WHERE ColorID = inv.ColorID),'')ColorName,
									ISNULL((SELECT UnitName FROM Sys_Unit WHERE UnitID = inv.ClosingStockAreaUnit),'')UnitName,
	                                ISNULL(inv.CrustQCLabel,'')CrustQCLabel,

									ISNULL(inv.ClosingStockPcs,0)QCStockPcs,
									ISNULL(inv.ClosingStockSide,0)QCStockSide,
									ISNULL(inv.ClosingStockArea,0)QCStockArea,
									ISNULL(inv.ClosingStockAreaUnit,0)AreaUnit
				                    

                            FROM	dbo.INV_CrustQCStock inv
							INNER JOIN (SELECT		MAX(TransectionID)TransectionID,
													ISNULL(StoreID,0)StoreID,
													ISNULL(BuyerID,0)BuyerID,
													ISNULL(BuyerOrderID,0)BuyerOrderID,
													ISNULL(ArticleID,0)ArticleID,
													ISNULL(ArticleChallanID,0)ArticleChallanID,
													ISNULL(ItemTypeID,0)ItemTypeID,
													ISNULL(LeatherTypeID,0)LeatherTypeID,
													ISNULL(LeatherStatusID,0)LeatherStatusID,
													ISNULL(ColorID,0)ColorID
															
										FROM		INV_CrustQCStock
										GROUP BY	StoreID,BuyerID,BuyerOrderID,ArticleID,ArticleChallanID,ItemTypeID,LeatherTypeID,LeatherStatusID,ColorID) sup
                            ON			inv.TransectionID=sup.TransectionID 
							WHERE		inv.StoreID = " + TransactionStore +
                                            " AND inv.BuyerID IS NULL "+
                                            " AND inv.BuyerOrderID IS NULL AND inv.ArticleID = " + ArticleID +
                                            " AND inv.ArticleChallanID = " + ArticleChallanID +
                                            " AND inv.ItemTypeID = " + ItemTypeID +
                                            " AND inv.LeatherStatusID = " + LeatherStatusID +
                                            " AND inv.ColorID = " + ColorID +
                                            " AND inv.ClosingStockPcs>0";

                    var allData = _context.Database.SqlQuery<clTransferForm>(query).ToList();
                    return allData;
                }
            }
            return null;
        }

        //Put Data into Child Item Grid According To Combination of QC Stock
        public List<clTransferTo> GetCrustTransferChildGridDetail(string TransactionStore, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ColorID, string ArticleChallanID)
        {
            if (!string.IsNullOrEmpty(TransactionStore))
            {
                if (!string.IsNullOrEmpty(BuyerID))
                {
                    if (string.IsNullOrEmpty(BuyerOrderID))
                    {
                        var query = @"SELECT						
									inv.TransectionID,
                                    inv.BuyerID BuyerID,
	                                ISNULL((SELECT BuyerName FROM Sys_Buyer WHERE BuyerID = inv.BuyerID),'')BuyerName,
                                    inv.BuyerOrderID BuyerOrderID,
	                                ISNULL((SELECT BuyerOrderNo FROM SLS_BuyerOrder WHERE BuyerOrderID = inv.BuyerOrderID),'')BuyerOrderNo,
                                    ISNULL(inv.ItemTypeID,0)ItemTypeID,
	                                ISNULL((SELECT ItemTypeName FROM Sys_ItemType WHERE ItemTypeID = inv.ItemTypeID),'')ItemTypeName,
                                    ISNULL(inv.LeatherTypeID,0)LeatherTypeID,
	                                ISNULL((SELECT LeatherTypeName FROM Sys_LeatherType WHERE LeatherTypeID = inv.LeatherTypeID),'')LeatherTypeName,
                                    ISNULL(inv.LeatherStatusID,0)LeatherStatusID,
	                                ISNULL((SELECT LeatherStatusName FROM Sys_LeatherStatus WHERE LeatherStatusID = inv.LeatherStatusID),'')LeatherStatusName,
                                    ISNULL(inv.ArticleID,0)ArticleID,
	                                ISNULL(inv.ArticleNo,'')ArticleNo,
									ISNULL(inv.ArticleChallanID,'')ArticleChallanID,
	                                ISNULL(inv.ArticleChallanNo,'')ArticleChallanNo,
                                    ISNULL(inv.ArticleColorNo,'')ArticleColorNo,
	                                ISNULL((SELECT ArticleName FROM Sys_Article WHERE ArticleID = inv.ArticleID),'')ArticleName,
                                    ISNULL(inv.ColorID,0)ColorID,
	                                ISNULL(inv.GradeID,0)GradeID,
	                                ISNULL(inv.GradeRange,0)GradeRange,
	                                ISNULL((SELECT ColorName FROM Sys_Color WHERE ColorID = inv.ColorID),'')ColorName,
									ISNULL((SELECT UnitName FROM Sys_Unit WHERE UnitID = inv.ClosingStockAreaUnit),'')UnitName,
	                                ISNULL(inv.CrustQCLabel,'')CrustQCLabel,

									ISNULL(inv.ClosingStockPcs,0)ToStockPcs,
									ISNULL(inv.ClosingStockSide,0)ToStockSide,
									ISNULL(inv.ClosingStockArea,0)ToStockArea,
									ISNULL(inv.ClosingStockAreaUnit,0)AreaUnit
				                    

                            FROM	dbo.INV_CrustQCStock inv
							INNER JOIN (SELECT		MAX(TransectionID)TransectionID,
													ISNULL(StoreID,0)StoreID,
													ISNULL(BuyerID,0)BuyerID,
													ISNULL(BuyerOrderID,0)BuyerOrderID,
													ISNULL(ArticleID,0)ArticleID,
													ISNULL(ArticleChallanID,0)ArticleChallanID,
													ISNULL(ItemTypeID,0)ItemTypeID,
													ISNULL(LeatherTypeID,0)LeatherTypeID,
													ISNULL(LeatherStatusID,0)LeatherStatusID,
													ISNULL(ColorID,0)ColorID
															
										FROM		INV_CrustQCStock
										GROUP BY	StoreID,BuyerID,BuyerOrderID,ArticleID,ArticleChallanID,ItemTypeID,LeatherTypeID,LeatherStatusID,ColorID) sup
                            ON			inv.TransectionID=sup.TransectionID 
							WHERE		inv.StoreID = " + TransactionStore +
                                       " AND inv.BuyerID = " + BuyerID +
                                       " AND inv.BuyerOrderID IS NULL AND inv.ArticleID = " + ArticleID +
                                       " AND inv.ArticleChallanID = " + ArticleChallanID +
                                       " AND inv.ItemTypeID = " + ItemTypeID +
                                       " AND inv.LeatherStatusID = " + LeatherStatusID +
                                       " AND inv.ColorID = " + ColorID +
                                       " AND inv.ClosingStockPcs>0";

                        var allData = _context.Database.SqlQuery<clTransferTo>(query).ToList();
                        List<clTransferTo> searchList = allData.ToList();
                        return searchList.Select(c => SetToCrustQCStockBussinessObject(c)).ToList<clTransferTo>();

                    }
                    else
                    {
                        var query = @"SELECT						
									inv.TransectionID,
                                    inv.BuyerID BuyerID,
	                                ISNULL((SELECT BuyerName FROM Sys_Buyer WHERE BuyerID = inv.BuyerID),'')BuyerName,
                                    inv.BuyerOrderID BuyerOrderID,
	                                ISNULL((SELECT BuyerOrderNo FROM SLS_BuyerOrder WHERE BuyerOrderID = inv.BuyerOrderID),'')BuyerOrderNo,
                                    ISNULL(inv.ItemTypeID,0)ItemTypeID,
	                                ISNULL((SELECT ItemTypeName FROM Sys_ItemType WHERE ItemTypeID = inv.ItemTypeID),'')ItemTypeName,
                                    ISNULL(inv.LeatherTypeID,0)LeatherTypeID,
	                                ISNULL((SELECT LeatherTypeName FROM Sys_LeatherType WHERE LeatherTypeID = inv.LeatherTypeID),'')LeatherTypeName,
                                    ISNULL(inv.LeatherStatusID,0)LeatherStatusID,
	                                ISNULL((SELECT LeatherStatusName FROM Sys_LeatherStatus WHERE LeatherStatusID = inv.LeatherStatusID),'')LeatherStatusName,
                                    ISNULL(inv.ArticleID,0)ArticleID,
	                                ISNULL(inv.ArticleNo,'')ArticleNo,
									ISNULL(inv.ArticleChallanID,'')ArticleChallanID,
	                                ISNULL(inv.ArticleChallanNo,'')ArticleChallanNo,
                                    ISNULL(inv.ArticleColorNo,'')ArticleColorNo,
	                                ISNULL((SELECT ArticleName FROM Sys_Article WHERE ArticleID = inv.ArticleID),'')ArticleName,
                                    ISNULL(inv.ColorID,0)ColorID,
	                                ISNULL(inv.GradeID,0)GradeID,
	                                ISNULL(inv.GradeRange,0)GradeRange,
	                                ISNULL((SELECT ColorName FROM Sys_Color WHERE ColorID = inv.ColorID),'')ColorName,
									ISNULL((SELECT UnitName FROM Sys_Unit WHERE UnitID = inv.ClosingStockAreaUnit),'')UnitName,
	                                ISNULL(inv.CrustQCLabel,'')CrustQCLabel,

									ISNULL(inv.ClosingStockPcs,0)ToStockPcs,
									ISNULL(inv.ClosingStockSide,0)ToStockSide,
									ISNULL(inv.ClosingStockArea,0)ToStockArea,
									ISNULL(inv.ClosingStockAreaUnit,0)AreaUnit
				                    

                            FROM	dbo.INV_CrustQCStock inv
							INNER JOIN (SELECT		MAX(TransectionID)TransectionID,
													ISNULL(StoreID,0)StoreID,
													ISNULL(BuyerID,0)BuyerID,
													ISNULL(BuyerOrderID,0)BuyerOrderID,
													ISNULL(ArticleID,0)ArticleID,
													ISNULL(ArticleChallanID,0)ArticleChallanID,
													ISNULL(ItemTypeID,0)ItemTypeID,
													ISNULL(LeatherTypeID,0)LeatherTypeID,
													ISNULL(LeatherStatusID,0)LeatherStatusID,
													ISNULL(ColorID,0)ColorID
															
										FROM		INV_CrustQCStock
										GROUP BY	StoreID,BuyerID,BuyerOrderID,ArticleID,ArticleChallanID,ItemTypeID,LeatherTypeID,LeatherStatusID,ColorID) sup
                            ON			inv.TransectionID=sup.TransectionID 
							WHERE		inv.StoreID = " + TransactionStore +
                                        " AND inv.BuyerID = " + BuyerID +
                                        " AND inv.BuyerOrderID = " + BuyerOrderID +
                                        " AND inv.ArticleID = " + ArticleID +
                                        " AND inv.ArticleChallanID = " + ArticleChallanID +
                                        " AND inv.ItemTypeID = " + ItemTypeID +
                                        " AND inv.LeatherStatusID = " + LeatherStatusID +
                                        " AND inv.ColorID = " + ColorID +
                                        " AND inv.ClosingStockPcs>0";

                        var allData = _context.Database.SqlQuery<clTransferTo>(query).ToList();
                        List<clTransferTo> searchList = allData.ToList();
                        return searchList.Select(c => SetToCrustQCStockBussinessObject(c)).ToList<clTransferTo>();
                    }
                }
                else
                {
                    var query = @"SELECT						
									inv.TransectionID,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         inv.BuyerID BuyerID,
	                                ISNULL((SELECT BuyerName FROM Sys_Buyer WHERE BuyerID = inv.BuyerID),'')BuyerName,
                                    ISNULL(inv.BuyerOrderID,0)BuyerOrderID,
	                                ISNULL((SELECT BuyerOrderNo FROM SLS_BuyerOrder WHERE BuyerOrderID = inv.BuyerOrderID),'')BuyerOrderNo,
                                    ISNULL(inv.ItemTypeID,0)ItemTypeID,
	                                ISNULL((SELECT ItemTypeName FROM Sys_ItemType WHERE ItemTypeID = inv.ItemTypeID),'')ItemTypeName,
                                    ISNULL(inv.LeatherTypeID,0)LeatherTypeID,
	                                ISNULL((SELECT LeatherTypeName FROM Sys_LeatherType WHERE LeatherTypeID = inv.LeatherTypeID),'')LeatherTypeName,
                                    ISNULL(inv.LeatherStatusID,0)LeatherStatusID,
	                                ISNULL((SELECT LeatherStatusName FROM Sys_LeatherStatus WHERE LeatherStatusID = inv.LeatherStatusID),'')LeatherStatusName,
                                    ISNULL(inv.ArticleID,0)ArticleID,
	                                ISNULL(inv.ArticleNo,'')ArticleNo,
									ISNULL(inv.ArticleChallanID,'')ArticleChallanID,
	                                ISNULL(inv.ArticleChallanNo,'')ArticleChallanNo,
                                    ISNULL(inv.ArticleColorNo,'')ArticleColorNo,
	                                ISNULL((SELECT ArticleName FROM Sys_Article WHERE ArticleID = inv.ArticleID),'')ArticleName,
                                    ISNULL(inv.ColorID,0)ColorID,
	                                ISNULL(inv.GradeID,0)GradeID,
	                                ISNULL(inv.GradeRange,0)GradeRange,
	                                ISNULL((SELECT ColorName FROM Sys_Color WHERE ColorID = inv.ColorID),'')ColorName,
									ISNULL((SELECT UnitName FROM Sys_Unit WHERE UnitID = inv.ClosingStockAreaUnit),'')UnitName,
	                                ISNULL(inv.CrustQCLabel,'')CrustQCLabel,

									ISNULL(inv.ClosingStockPcs,0)ToStockPcs,
									ISNULL(inv.ClosingStockSide,0)ToStockSide,
									ISNULL(inv.ClosingStockArea,0)ToStockArea,
									ISNULL(inv.ClosingStockAreaUnit,0)AreaUnit
				                    

                            FROM	dbo.INV_CrustQCStock inv
							INNER JOIN (SELECT		MAX(TransectionID)TransectionID,
													ISNULL(StoreID,0)StoreID,
													ISNULL(BuyerID,0)BuyerID,
													ISNULL(BuyerOrderID,0)BuyerOrderID,
													ISNULL(ArticleID,0)ArticleID,
													ISNULL(ArticleChallanID,0)ArticleChallanID,
													ISNULL(ItemTypeID,0)ItemTypeID,
													ISNULL(LeatherTypeID,0)LeatherTypeID,
													ISNULL(LeatherStatusID,0)LeatherStatusID,
													ISNULL(ColorID,0)ColorID
															
										FROM		INV_CrustQCStock
										GROUP BY	StoreID,BuyerID,BuyerOrderID,ArticleID,ArticleChallanID,ItemTypeID,LeatherTypeID,LeatherStatusID,ColorID) sup
                            ON			inv.TransectionID=sup.TransectionID 
							WHERE		inv.StoreID = " + TransactionStore +
                                            " AND inv.BuyerID IS NULL " +
                                            " AND inv.BuyerOrderID IS NULL AND inv.ArticleID = " + ArticleID +
                                            " AND inv.ArticleChallanID = " + ArticleChallanID +
                                            " AND inv.ItemTypeID = " + ItemTypeID +
                                            " AND inv.LeatherStatusID = " + LeatherStatusID +
                                            " AND inv.ColorID = " + ColorID +
                                            " AND inv.ClosingStockPcs>0";

                    var allData = _context.Database.SqlQuery<clTransferTo>(query).ToList();
                    List<clTransferTo> searchList = allData.ToList();
                    return searchList.Select(c => SetToCrustQCStockBussinessObject(c)).ToList<clTransferTo>();
                }
            }
            return null;
        }



        public clTransferTo SetToCrustQCStockBussinessObject(clTransferTo Entity)
        {
            clTransferTo Model = new clTransferTo();

            Model.CLTransferFromID = Entity.CLTransferFromID;
            Model.CLTransferToID = Entity.CLTransferToID;
            Model.CLTransferID = Entity.CLTransferID;
            Model.CLTransferNo = Entity.CLTransferNo;
            Model.StoreID = Entity.StoreID;
            Model.StoreName = Entity.StoreID == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.StoreID).FirstOrDefault().StoreName;
            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleName = Entity.ArticleName == null ? "" : _context.Sys_Article.Where(m => m.ArticleID == Entity.ArticleID).FirstOrDefault().ArticleName;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;
            Model.ArticleChallanID = Entity.ArticleChallanID;
            Model.ArticleColorNo = Entity.ArticleColorNo;
            Model.CrustQCLabel = Entity.CrustQCLabel;

            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            Model.LeatherTypeName = Entity.LeatherTypeID == null ? "" : _context.Sys_LeatherType.Where(m => m.LeatherTypeID == Entity.LeatherTypeID).FirstOrDefault().LeatherTypeName;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            Model.GradeRange = Entity.GradeRange;
            

            Model.QCStockPcs = Entity.QCStockPcs;
            Model.QCStockSide = Entity.QCStockSide;
            Model.QCStockArea = Entity.QCStockArea;



            Model.ToStockPcs = Entity.ToStockPcs;
            Model.ToStockSide = Entity.ToStockSide;
            Model.ToStockArea = Entity.ToStockArea;
            Model.AreaUnit = Entity.AreaUnit;
            Model.UnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;


            return Model;
        }



        #endregion

        #region RETRIVE All Child Grid Data From Pop Up
        public List<clTransfer> GetAllBuyerGridData()
        {
            var queryString = @"SELECT  
                                            cqs.BuyerID,
                                            b.BuyerName,
                                            SUM(cqs.ClosingStockPcs)ClosingStockPcs,
                                            SUM(cqs.ClosingStockArea)ClosingStockArea,
                                            SUM(cqs.ClosingStockSide)ClosingStockSide 
                                FROM        INV_CrustQCStock cqs
                                INNER JOIN  Sys_Buyer b ON cqs.BuyerID=b.BuyerID 
                                GROUP BY    cqs.BuyerID,b.BuyerName";

            var allData = _context.Database.SqlQuery<clTransfer>(queryString).ToList();
            return allData.ToList();
        }

        public List<clTransfer> GetAllBuyerOrderGridData()
        {
            var queryString = @"SELECT  
                                            cqs.BuyerOrderID,
                                            b.BuyerOrderNo,
                                            SUM(cqs.ClosingStockPcs)ClosingStockPcs,
                                            SUM(cqs.ClosingStockArea)ClosingStockArea,
                                            SUM(cqs.ClosingStockSide)ClosingStockSide 
                                FROM        INV_CrustQCStock cqs
                                INNER JOIN  SLS_BuyerOrder b ON cqs.BuyerOrderID=b.BuyerOrderID 
                                GROUP BY    cqs.BuyerOrderID,b.BuyerOrderNo";

            var allData = _context.Database.SqlQuery<clTransfer>(queryString).ToList();
            return allData.ToList();
        }

        public List<clTransfer> GetAllArticleGridData()
        {
            var queryString = @"SELECT  
                                            cqs.ArticleID,
                                            cqs.ArticleNo,
											a.ArticleName,
                                            SUM(cqs.ClosingStockPcs)ClosingStockPcs,
                                            SUM(cqs.ClosingStockArea)ClosingStockArea,
                                            SUM(cqs.ClosingStockSide)ClosingStockSide 
                                FROM        INV_CrustQCStock cqs
								INNER JOIN  Sys_Article a ON a.ArticleID=cqs.ArticleID 
                                GROUP BY    cqs.ArticleID,cqs.ArticleNo,a.ArticleName";

            var allData = _context.Database.SqlQuery<clTransfer>(queryString).ToList();
            return allData.ToList();
        }

        public List<clTransfer> GetAllColorGridData()
        {
            var queryString = @"SELECT  
                                            cqs.ColorID,
											cqs.ArticleColorNo,
                                            b.ColorName,
                                            SUM(cqs.ClosingStockPcs)ClosingStockPcs,
                                            SUM(cqs.ClosingStockArea)ClosingStockArea,
                                            SUM(cqs.ClosingStockSide)ClosingStockSide 
                                FROM        INV_CrustQCStock cqs
                                INNER JOIN  Sys_Color b ON cqs.ColorID=b.ColorID 
                                GROUP BY    cqs.ColorID,b.ColorName, cqs.ArticleColorNo";

            var allData = _context.Database.SqlQuery<clTransfer>(queryString).ToList();
            return allData.ToList();
        }

        public List<clTransfer> GetAllItemTypeGridData()
        {
            var queryString = @"SELECT  
                                            cqs.ItemTypeID,
                                            b.ItemTypeName,
                                            SUM(cqs.ClosingStockPcs)ClosingStockPcs,
                                            SUM(cqs.ClosingStockArea)ClosingStockArea,
                                            SUM(cqs.ClosingStockSide)ClosingStockSide 
                                FROM        INV_CrustQCStock cqs
                                INNER JOIN  Sys_ItemType b ON cqs.ItemTypeID=b.ItemTypeID 
                                GROUP BY    cqs.ItemTypeID,b.ItemTypeName";

            var allData = _context.Database.SqlQuery<clTransfer>(queryString).ToList();
            return allData.ToList();
        }

        public List<clTransfer> GetAllLeatherStatusGridData()
        {
            var queryString = @"SELECT  
                                            cqs.LeatherStatusID,
                                            b.LeatherStatusName,
                                            SUM(cqs.ClosingStockPcs)ClosingStockPcs,
                                            SUM(cqs.ClosingStockArea)ClosingStockArea,
                                            SUM(cqs.ClosingStockSide)ClosingStockSide 
                                FROM        INV_CrustQCStock cqs
                                INNER JOIN  Sys_LeatherStatus b ON cqs.LeatherStatusID=b.LeatherStatusID 
                                GROUP BY    cqs.LeatherStatusID,b.LeatherStatusName";

            var allData = _context.Database.SqlQuery<clTransfer>(queryString).ToList();
            return allData.ToList();
        }

        public List<clTransfer> GetAllActiveCrustStore()
        {
            var queryString = @"	SELECT	StoreID,
			                                StoreName 
	                                FROM	SYS_Store 
	                                WHERE 
			                                StoreType='Crust' 
		                                AND StoreCategory='Leather' 
		                                AND IsActive='true' 
		                                AND IsDelete='False'";

            var allData = _context.Database.SqlQuery<clTransfer>(queryString).ToList();
            return allData.ToList();
        }

        #endregion

        #region SAVE Crust Leather Transfer DATA
        public ValidationMsg Save(clTransfer model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {

                        model.CLTransferNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.CLTransferNo != null)
                        {
                            #region Save Finish Transfer

                            INV_CLTransfer tblCLTransfer = SetToCLTransferModelObject(model, userid);
                            _context.INV_CLTransfer.Add(tblCLTransfer);
                            _context.SaveChanges();
                            #endregion

                            #region Save Finish Leather Transfer From
                            if (model.TransferFromList != null)
                            {
                                foreach (clTransferForm objclTransferFrom in model.TransferFromList)
                                {
                                    objclTransferFrom.CLTransferID = tblCLTransfer.CLTransferID;
                                    objclTransferFrom.CLTransferNo = tblCLTransfer.CLTransferNo;

                                    INV_CLTransferFrom tblCLTransferFrom = SetToCLTransferFromModelObject(objclTransferFrom, userid);
                                    _context.INV_CLTransferFrom.Add(tblCLTransferFrom);
                                    _context.SaveChanges();

                                    #region Save Finish Leather Transfer To

                                    if (model.TransferToList != null)
                                    {
                                        //if (objclTransferFrom.ArticleChallanNo == model.TransferToList[0].ArticleChallanNo)
                                        //{
                                        foreach (clTransferTo objclTransferTo in model.TransferToList)
                                        {
                                            objclTransferTo.CLTransferFromID = tblCLTransferFrom.CLTransferFromID;
                                            objclTransferTo.CLTransferID = tblCLTransfer.CLTransferID;
                                            objclTransferTo.CLTransferNo = tblCLTransfer.CLTransferNo;

                                            INV_CLTransferTo tblCLTransferTo = SetToCLTransferToModelObject(objclTransferTo, userid);
                                            _context.INV_CLTransferTo.Add(tblCLTransferTo);
                                            _context.SaveChanges();
                                        }
                                        //}
                                    }
                                    #endregion
                                }
                            }
                            #endregion

                            _context.SaveChanges();

                            tx.Complete();
                            CLTransferID = tblCLTransfer.CLTransferID;
                            CLTransferNo = tblCLTransfer.CLTransferNo;

                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Crust Leather No Predefine Value not Found.";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Crust Leather No Data Already Exit.";
                }
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public INV_CLTransfer SetToCLTransferModelObject(clTransfer model, int userid)
        {
            INV_CLTransfer Entity = new INV_CLTransfer();

            Entity.CLTransferID = model.CLTransferID;
            Entity.CLTransferNo = model.CLTransferNo;
            Entity.CLTransferDate = DalCommon.SetDate(model.CLTransferDate);
            Entity.CLTransferCategory = model.CLTransferCategory;
            Entity.TranrsferType = model.TranrsferType;
            Entity.TransactionStore = model.TransactionStore;
            Entity.IssueStore = model.IssueStore;
            Entity.IssueNote = model.IssueNote;
            Entity.CheckNote = model.CheckNote;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.ModifiedOn = DateTime.Now;
            Entity.ModifiedBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }
        public INV_CLTransferFrom SetToCLTransferFromModelObject(clTransferForm model, int userid)
        {
            INV_CLTransferFrom Entity = new INV_CLTransferFrom();

            Entity.CLTransferID = model.CLTransferID;
            Entity.CLTransferNo = model.CLTransferNo == null ? "" : model.CLTransferNo; 
            Entity.CLTransferFromID = model.CLTransferFromID;
            Entity.StoreID = model.StoreID == null ? 0 : model.StoreID;
            Entity.BuyerID = model.BuyerID == null ? 0 : model.BuyerID; 
            Entity.BuyerOrderID = model.BuyerOrderID == null ? 0 : model.BuyerOrderID;
            Entity.ArticleID = model.ArticleID == null ? 0 : model.ArticleID;
            Entity.ArticleNo = model.ArticleNo == null ? "" : model.ArticleNo;
            Entity.ColorID = model.ColorID == null ? 0 : model.ColorID;
            Entity.ItemTypeID = model.ItemTypeID == null ? 0 : model.ItemTypeID; 
            Entity.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
            Entity.LeatherStatusID = model.LeatherStatusID == null ? 0 : model.LeatherStatusID;
            Entity.CrustQCLabel = model.CrustQCLabel == null ? "" : model.CrustQCLabel;
            Entity.GradeRange = model.GradeRange == null ? "" : model.GradeRange;
            Entity.QCStockPcs = model.QCStockPcs == null ? 0 : model.QCStockPcs;
            Entity.QCStockSide = model.QCStockSide == null ? 0 : model.QCStockSide;
            Entity.QCStockArea = model.QCStockArea == null ? 0 : model.QCStockArea;
            Entity.AreaUnit = model.AreaUnit == null ? 0 : model.AreaUnit;
            Entity.ArticleChallanNo = model.ArticleChallanNo == null ? "" : model.ArticleChallanNo;
            Entity.ArticleChallanID = model.ArticleChallanID == null ? 0 : model.ArticleChallanID;
            Entity.ArticleColorNo = model.ArticleColorNo == null ? 0 : model.ArticleColorNo; ; 
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.ModifiedOn = DateTime.Now;
            Entity.ModifiedBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }
        public INV_CLTransferTo SetToCLTransferToModelObject(clTransferTo model, int userid)
        {
            INV_CLTransferTo Entity = new INV_CLTransferTo();

            Entity.CLTransferToID = model.CLTransferToID;
            Entity.CLTransferID = model.CLTransferID;
            Entity.CLTransferFromID = model.CLTransferFromID;
            Entity.CLTransferNo = model.CLTransferNo == null ? "" : model.CLTransferNo; 
            Entity.StoreID = model.StoreID == null ? 0 : model.StoreID;
            Entity.BuyerID = model.BuyerID == null ? 0 : model.BuyerID;
            Entity.BuyerOrderID = model.BuyerOrderID == null ? 0 : model.BuyerOrderID;
            Entity.ArticleID = model.ArticleID == null ? 0 : model.ArticleID;
            Entity.ArticleNo = model.ArticleNo == null ? "" : model.ArticleNo;
            Entity.ColorID = model.ColorID == null ? 0 : model.ColorID;
            Entity.ItemTypeID = model.ItemTypeID == null ? 0 : model.ItemTypeID; 
            Entity.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
            Entity.LeatherStatusID = model.LeatherStatusID == null ? 0 : model.LeatherStatusID;
            Entity.CrustQCLabel = model.CrustQCLabel == null ? "" : model.CrustQCLabel;
            Entity.GradeRange = model.GradeRange == null ? "" : model.GradeRange;
            Entity.ToStockPcs = model.ToStockPcs == null ? 0 : model.ToStockPcs;
            Entity.ToStockSide = model.ToStockSide == null ? 0 : model.ToStockSide;
            Entity.ToStockArea = model.ToStockArea == null ? 0 : model.ToStockArea;
            Entity.AreaUnit = model.AreaUnit == null ? 0 : model.AreaUnit;
            Entity.ArticleChallanNo = model.ArticleChallanNo == null ? "" : model.ArticleChallanNo;
            Entity.ArticleChallanID = model.ArticleChallanID == null ? 0 : model.ArticleChallanID;
            Entity.ArticleColorNo = model.ArticleColorNo;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.ModifiedOn = DateTime.Now;
            Entity.ModifiedBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        #endregion

        #region UPDATE Crust Leather Transfer DATA

        public ValidationMsg Update(clTransfer model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region Crust Leather Transfer

                        INV_CLTransfer IssueEntity = SetToCLTransferModelObject(model, userid);
                        var OriginalEntity = _context.INV_CLTransfer.First(m => m.CLTransferID == model.CLTransferID);

                        OriginalEntity.CLTransferID = IssueEntity.CLTransferID;
                        OriginalEntity.CLTransferCategory = IssueEntity.CLTransferCategory;
                        OriginalEntity.CLTransferDate = IssueEntity.CLTransferDate;
                        OriginalEntity.TranrsferType = IssueEntity.TranrsferType;
                        OriginalEntity.TransactionStore = IssueEntity.TransactionStore;
                        OriginalEntity.IssueStore = IssueEntity.IssueStore;
                        OriginalEntity.IssueNote = IssueEntity.IssueNote;
                        OriginalEntity.CheckNote = IssueEntity.CheckNote;
                        OriginalEntity.RecordStatus = IssueEntity.RecordStatus;


                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;
                        _context.SaveChanges();
                        #endregion

                        #region Save NEW Data & Update Existing Crust Leather Transfer Data

                        if (model.TransferFromList != null)
                        {
                            foreach (clTransferForm objCLTransferItem in model.TransferFromList)
                            {
                                objCLTransferItem.CLTransferID = model.CLTransferID;
                                objCLTransferItem.CLTransferNo = model.CLTransferNo;


                                INV_CLTransferFrom CurrentIssueItemEntity = SetToCLTransferFromModelObject(objCLTransferItem, userid);
                                var OriginalIssueItemEntity = _context.INV_CLTransferFrom.First(m => m.CLTransferFromID == objCLTransferItem.CLTransferFromID);


                                OriginalIssueItemEntity.CLTransferID = CurrentIssueItemEntity.CLTransferID;
                                OriginalIssueItemEntity.CLTransferNo = CurrentIssueItemEntity.CLTransferNo;
                                OriginalIssueItemEntity.BuyerID = CurrentIssueItemEntity.BuyerID;
                                OriginalIssueItemEntity.BuyerOrderID = CurrentIssueItemEntity.BuyerOrderID;
                                OriginalIssueItemEntity.ItemTypeID = CurrentIssueItemEntity.ItemTypeID;
                                OriginalIssueItemEntity.LeatherStatusID = CurrentIssueItemEntity.LeatherStatusID;
                                OriginalIssueItemEntity.ColorID = CurrentIssueItemEntity.ColorID;
                                OriginalIssueItemEntity.GradeRange = CurrentIssueItemEntity.GradeRange;
                                OriginalIssueItemEntity.ArticleChallanID = CurrentIssueItemEntity.ArticleChallanID;
                                OriginalIssueItemEntity.ArticleColorNo = CurrentIssueItemEntity.ArticleColorNo;
                                OriginalIssueItemEntity.ArticleID = CurrentIssueItemEntity.ArticleID;
                                OriginalIssueItemEntity.ArticleNo = CurrentIssueItemEntity.ArticleNo;
                                OriginalIssueItemEntity.ArticleChallanNo = CurrentIssueItemEntity.ArticleChallanNo;
                                OriginalIssueItemEntity.CrustQCLabel = CurrentIssueItemEntity.CrustQCLabel;
                                OriginalIssueItemEntity.SetOn = CurrentIssueItemEntity.SetOn;
                                OriginalIssueItemEntity.SetBy = CurrentIssueItemEntity.SetBy;


                                OriginalIssueItemEntity.ModifiedBy = userid;
                                OriginalIssueItemEntity.ModifiedOn = DateTime.Now;
                                OriginalIssueItemEntity.IPAddress = GetIPAddress.LocalIPAddress();
                                OriginalIssueItemEntity.SetOn = DateTime.Now;
                                OriginalIssueItemEntity.SetBy = userid;


                                #region Save CrustQC Records

                                if (model.TransferToList != null)
                                {
                                    foreach (clTransferTo objCLTransferToItem in model.TransferToList)
                                    {
                                        objCLTransferToItem.CLTransferID = model.CLTransferID;
                                        objCLTransferToItem.CLTransferNo = model.CLTransferNo;
                                        objCLTransferToItem.CLTransferFromID = objCLTransferItem.CLTransferFromID;

                                        if (objCLTransferToItem.CLTransferToID == 0)
                                        {
                                            objCLTransferToItem.CLTransferFromID = objCLTransferItem.CLTransferFromID;
                                            objCLTransferToItem.CLTransferID = model.CLTransferID;
                                            objCLTransferToItem.CLTransferNo = model.CLTransferNo;
                                            //objCLTransferToItem.ScheduleProductionNo = objCLTransferItem.ScheduleProductionNo;

                                            INV_CLTransferTo tblQCSelection = SetToCLTransferToModelObject(objCLTransferToItem, userid);
                                            _context.INV_CLTransferTo.Add(tblQCSelection);
                                        }
                                        else
                                        {
                                            INV_CLTransferTo CurrIssueGradeEntity = SetToCLTransferToModelObject(objCLTransferToItem, userid);
                                            var OrgrEntity = _context.INV_CLTransferTo.First(m => m.CLTransferToID == objCLTransferToItem.CLTransferToID);

                                            //OrgrEntity.ScheduleID = CurrEntity.ScheduleID;
                                            //OrgrEntity.ProductionNo = CurrEntity.ProductionNo;
                                            OrgrEntity.CLTransferToID = CurrIssueGradeEntity.CLTransferToID;
                                            //OrgrEntity.WBSIssueGradeNo = CurrIssueGradeEntity.WBSIssueGradeNo;
                                            //OrgrEntity.WBSIssueItemID = CurrIssueGradeEntity.WBSIssueItemID;
                                            OrgrEntity.BuyerID = CurrIssueGradeEntity.BuyerID;
                                            OrgrEntity.BuyerOrderID = CurrIssueGradeEntity.BuyerOrderID;
                                            OrgrEntity.ArticleID = CurrIssueGradeEntity.ArticleID;
                                            OrgrEntity.ArticleNo = CurrIssueGradeEntity.ArticleNo;
                                            OrgrEntity.ArticleChallanNo = CurrIssueGradeEntity.ArticleChallanNo;
                                            OrgrEntity.CrustQCLabel = CurrIssueGradeEntity.CrustQCLabel;
                                            OrgrEntity.ItemTypeID = CurrIssueGradeEntity.ItemTypeID;
                                            OrgrEntity.LeatherStatusID = CurrIssueGradeEntity.LeatherStatusID;
                                            OrgrEntity.ColorID = CurrIssueGradeEntity.ColorID;
                                            OrgrEntity.GradeRange = CurrIssueGradeEntity.GradeRange;
                                            OrgrEntity.ArticleChallanID = CurrIssueGradeEntity.ArticleChallanID;
                                            OrgrEntity.ArticleColorNo = CurrIssueGradeEntity.ArticleColorNo;
                                            OrgrEntity.ToStockPcs = CurrIssueGradeEntity.ToStockPcs;
                                            OrgrEntity.ToStockSide = CurrIssueGradeEntity.ToStockSide;
                                            OrgrEntity.ToStockArea = CurrIssueGradeEntity.ToStockArea;
                                            OrgrEntity.AreaUnit = CurrIssueGradeEntity.AreaUnit;

                                            OrgrEntity.ModifiedBy = userid;
                                            OrgrEntity.ModifiedOn = DateTime.Now;
                                            OrgrEntity.IPAddress = GetIPAddress.LocalIPAddress();
                                            OrgrEntity.SetOn = DateTime.Now;
                                            OrgrEntity.SetBy = userid;
                                        }
                                    }
                                }

                                #endregion
                            }
                        }

                        #endregion



                        _context.SaveChanges();
                        tx.Complete();
                        CLTransferID = model.CLTransferID;
                        CLTransferNo = model.CLTransferNo;
                        _vmMsg.Type = Enums.MessageType.Update;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                }

                #endregion
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }
        public clTransferTo SetToBussinessObject(INV_CLTransferTo Entity)
        {
            clTransferTo Model = new clTransferTo();

            Model.CLTransferToID = Entity.CLTransferToID;
            Model.CLTransferFromID = Entity.CLTransferFromID;
            Model.CLTransferID = Entity.CLTransferID;

            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;
            Model.CrustQCLabel = Entity.CrustQCLabel;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.GradeRange = Entity.GradeRange;
            Model.ArticleChallanID = Entity.ArticleChallanID;
            Model.ArticleColorNo = Entity.ArticleColorNo;
            //Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;

            Model.TransferPcs = Entity.ToStockPcs;
            Model.TransferSide = Entity.ToStockSide;
            Model.TransferArea = Entity.ToStockArea;
            Model.TransferAreaUnit = Entity.AreaUnit;
            Model.UnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;

            Model.ToStockPcs = Entity.ToStockPcs;
            Model.ToStockSide = Entity.ToStockSide;
            Model.ToStockArea = Entity.ToStockArea;
            Model.AreaUnit = Entity.AreaUnit;


            return Model;
        }
        public clTransferForm SetToBussinessObject(clTransferForm Entity)
        {
            clTransferForm Model = new clTransferForm();

            Model.CLTransferFromID = Entity.CLTransferFromID;
            Model.CLTransferID = Entity.CLTransferID;
            Model.CLTransferNo = Entity.CLTransferNo;


            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;
            Model.CrustQCLabel = Entity.CrustQCLabel;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.GradeRange = Entity.GradeRange;
            Model.ArticleChallanID = Entity.ArticleChallanID;
            Model.ArticleColorNo = Entity.ArticleColorNo;
            //Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;

            Model.QCStockPcs = Entity.QCStockPcs;
            Model.QCStockSide = Entity.QCStockSide;
            Model.QCStockArea = Entity.QCStockArea;
            Model.TransferAreaUnit = Entity.AreaUnit;
            Model.AreaUnit = Entity.AreaUnit;
            Model.UnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;

            return Model;
        }
        public clTransfer SetToBussinessObject(clTransfer Entity)
        {
            clTransfer Model = new clTransfer();

            Model.CLTransferFromID = Entity.CLTransferFromID;
            Model.CLTransferID = Entity.CLTransferID;
            Model.CLTransferNo = Entity.CLTransferNo;


            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;
            Model.CrustQCLabel = Entity.CrustQCLabel;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.GradeRange = Entity.GradeRange;
            Model.ArticleChallanID = Entity.ArticleChallanID;
            Model.ArticleColorNo = Entity.ArticleColorNo;
            // Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;

            Model.QCStockPcs = Entity.QCStockPcs;
            Model.QCStockSide = Entity.QCStockSide;
            Model.QCStockArea = Entity.QCStockArea;
            Model.TransferAreaUnit = Entity.AreaUnit;
            Model.AreaUnit = Entity.AreaUnit;
            Model.UnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;

            return Model;
        }

        #endregion

        #region SEARCH Crust Leather Transfer DATA


        public List<clTransfer> GetclTransferInfo()
        {
            var query = @"SELECT   
        							ISNULL(ct.CLTransferID,0)CLTransferID,
        							ISNULL(ct.CLTransferNo,0)CLTransferNo,
        							ISNULL(ct.CLTransferCategory,0)CLTransferCategory,
        							ISNULL(ct.TranrsferType,0)TranrsferType,
                                    CONVERT(VARCHAR(12),ct.CLTransferDate, 106) CLTransferDate,
        							ISNULL(ct.TransactionStore,0)TransactionStore,
        							(SELECT StoreName FROM dbo.SYS_Store WHERE StoreID = ct.TransactionStore)TransactionStoreName,
        							ISNULL(ct.IssueStore,0)IssueStore,
        							(SELECT StoreName FROM dbo.SYS_Store WHERE StoreID = ct.IssueStore)IssueStoreName,
        							ISNULL(ct.RecordStatus,0)RecordStatus
                        FROM	    INV_CLTransfer ct
                        ORDER BY    ct.CLTransferID DESC";

            //var result = _context.Database.SqlQuery<clTransfer>(query).ToList();
            //return result;
            var allData = _context.Database.SqlQuery<clTransfer>(query).ToList();
            List<clTransfer> searchList = allData.ToList();
            return searchList.Select(c => SetToCrustLeatherBussinessObject(c)).ToList<clTransfer>();


        }

        //Search Master Data
        //public List<clTransfer> GetclTransferInfo()
        //{
        //    List<INV_CLTransfer> searchList = _context.INV_CLTransfer.OrderByDescending(m => m.CLTransferID).ToList();
        //    return searchList.Select(c => SetToCrustLeatherBussinessObject(c)).ToList<clTransfer>();
        //}

        public clTransfer SetToCrustLeatherBussinessObject(clTransfer Entity)
        {
            clTransfer Model = new clTransfer();

            Model.CLTransferID = Entity.CLTransferID;
            Model.CLTransferNo = Entity.CLTransferNo;
            Model.CLTransferCategory = Entity.CLTransferCategory;
            Model.TranrsferType = Entity.TranrsferType;
            Model.CLTransferDate = string.IsNullOrEmpty(Entity.CLTransferDate.ToString()) ? string.Empty : Convert.ToDateTime(Entity.CLTransferDate).ToString("dd/MM/yyyy");
            Model.TransactionStore = Entity.TransactionStore;
            Model.TransactionStoreName = Entity.TransactionStore == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.TransactionStore).FirstOrDefault().StoreName;
            Model.IssueStore = Entity.IssueStore;
            Model.IssueNote = Entity.IssueNote;
            Model.CheckNote = Entity.CheckNote;
            Model.IssueStoreName = Entity.IssueStore == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.IssueStore).FirstOrDefault().StoreName;
            Model.RecordStatus = Entity.RecordStatus;
            return Model;
        }

        //Search First Grid Data
        public List<clTransferForm> GetCLTransferFromList(string CLTransferID)
        {
            long? clTransferID = Convert.ToInt64(CLTransferID);
            List<INV_CLTransferFrom> searchList = _context.INV_CLTransferFrom.Where(m => m.CLTransferID == clTransferID).ToList();
            return searchList.Select(c => SetToCrustLeatherTransferFromBussinessObject(c)).ToList<clTransferForm>();
        }

        public clTransferForm SetToCrustLeatherTransferFromBussinessObject(INV_CLTransferFrom Entity)
        {
            clTransferForm Model = new clTransferForm();

            Model.CLTransferFromID = Entity.CLTransferFromID;
            Model.CLTransferNo = Entity.CLTransferNo;
            Model.StoreID = Entity.StoreID;
            Model.BuyerID = Entity.BuyerID;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.ArticleID = Entity.ArticleID;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.ColorID = Entity.ColorID;


            var CLTransferID = _context.INV_CLTransferFrom.Where(m => m.CLTransferFromID == Entity.CLTransferFromID).FirstOrDefault().CLTransferID;
            var storeId = _context.INV_CLTransfer.Where(m => m.CLTransferID == Entity.CLTransferID).FirstOrDefault().TransactionStore;
            var BuyerID = _context.INV_CLTransferFrom.Where(m => m.CLTransferFromID == Entity.CLTransferFromID).FirstOrDefault().BuyerID;
            var BuyerOrderID = _context.INV_CLTransferFrom.Where(m => m.CLTransferFromID == Entity.CLTransferFromID).FirstOrDefault().BuyerOrderID;
            var ItemTypeID = _context.INV_CLTransferFrom.Where(m => m.CLTransferFromID == Entity.CLTransferFromID).FirstOrDefault().ItemTypeID;
            var LeatherTypeID = _context.INV_CLTransferFrom.Where(m => m.CLTransferFromID == Entity.CLTransferFromID).FirstOrDefault().LeatherTypeID;
            var LeatherStatusID = _context.INV_CLTransferFrom.Where(m => m.CLTransferFromID == Entity.CLTransferFromID).FirstOrDefault().LeatherStatusID;
            var ArticleID = _context.INV_CLTransferFrom.Where(m => m.CLTransferFromID == Entity.CLTransferFromID).FirstOrDefault().ArticleID;
            var ColorID = _context.INV_CLTransferFrom.Where(m => m.CLTransferFromID == Entity.CLTransferFromID).FirstOrDefault().ColorID;

            Model.QCStockPcs = _context.INV_CrustQCStock.Where(ma => ma.StoreID == storeId
                && ma.BuyerID == BuyerID
                && ma.BuyerOrderID == BuyerOrderID
                && ma.ItemTypeID == ItemTypeID
                && ma.LeatherTypeID == LeatherTypeID
                && ma.LeatherStatusID == LeatherStatusID
                && ma.ArticleID == ArticleID
                // && ma.GradeID == Entity.GradeID
                && ma.ColorID == Entity.ColorID).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockPcs;
            Model.QCStockSide = _context.INV_CrustQCStock.Where(ma => ma.StoreID == storeId
                && ma.BuyerID == BuyerID
                && ma.BuyerOrderID == BuyerOrderID
                && ma.ItemTypeID == ItemTypeID
                && ma.LeatherTypeID == LeatherTypeID
                && ma.LeatherStatusID == LeatherStatusID
                && ma.ArticleID == ArticleID
                //&& ma.GradeID == Entity.GradeID
                && ma.ColorID == Entity.ColorID).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockSide;
            Model.QCStockArea = _context.INV_CrustQCStock.Where(ma => ma.StoreID == storeId
                && ma.BuyerID == BuyerID
                && ma.BuyerOrderID == BuyerOrderID
                && ma.ItemTypeID == ItemTypeID
                && ma.LeatherTypeID == LeatherTypeID
                && ma.LeatherStatusID == LeatherStatusID
                && ma.ArticleID == ArticleID
                //&& ma.GradeID == Entity.GradeID
               && ma.ColorID == Entity.ColorID).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockArea;

            Model.AreaUnit = _context.INV_CrustQCStock.Where(ma => ma.StoreID == storeId
                && ma.BuyerID == BuyerID
                && ma.BuyerOrderID == BuyerOrderID
                && ma.ItemTypeID == ItemTypeID
                && ma.LeatherTypeID == LeatherTypeID
                && ma.LeatherStatusID == LeatherStatusID
                && ma.ArticleID == ArticleID
                //&& ma.GradeID == Entity.GradeID
                && ma.ColorID == Entity.ColorID).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockAreaUnit;

            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;
            Model.ArticleChallanID = Entity.ArticleChallanID;
            Model.ArticleColorNo = Entity.ArticleColorNo;
            Model.CrustQCLabel = Entity.CrustQCLabel;
            Model.GradeRange = Entity.GradeRange;
            Model.TransactionStoreName = Entity.StoreID == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.StoreID).FirstOrDefault().StoreName;
            Model.IssueStoreName = Entity.StoreID == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.StoreID).FirstOrDefault().StoreName;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.LeatherStatusName = Entity.BuyerID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleName = Entity.ArticleID == null ? "" : _context.Sys_Article.Where(m => m.ArticleID == Entity.ArticleID).FirstOrDefault().ArticleName;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            //Model.GradeName = Entity.ItemTypeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.UnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;

            return Model;
        }

        //Search Second Grid Data
        public List<clTransferTo> GetCLTransferToList(string CLTransferFromID)
        {

            long? clTransferFromID = Convert.ToInt64(CLTransferFromID);
            List<INV_CLTransferTo> searchList = _context.INV_CLTransferTo.Where(m => m.CLTransferFromID == clTransferFromID).OrderByDescending(m => m.CLTransferToID).ToList();
            return searchList.Select(c => SetToCrustLeatherTransferToBussinessObject(c)).ToList<clTransferTo>();

        }

        public clTransferTo SetToCrustLeatherTransferToBussinessObject(INV_CLTransferTo Entity)
        {
            clTransferTo Model = new clTransferTo();

            Model.CLTransferToID = Entity.CLTransferToID;
            Model.CLTransferFromID = Entity.CLTransferFromID;
            Model.CLTransferID = Entity.CLTransferID;
            Model.CLTransferNo = Entity.CLTransferNo;
            Model.StoreID = Entity.StoreID;

            Model.IssueStoreName = Entity.StoreID == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.StoreID).FirstOrDefault().StoreName;
            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleName = Entity.ArticleID == null ? "" : _context.Sys_Article.Where(m => m.ArticleID == Entity.ArticleID).FirstOrDefault().ArticleName;
            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;
            Model.ArticleChallanID = Entity.ArticleChallanID;
            Model.ArticleColorNo = Entity.ArticleColorNo;
            Model.CrustQCLabel = Entity.CrustQCLabel;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.BuyerID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            Model.GradeRange = Entity.GradeRange;
            //Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.ToStockPcs = Entity.ToStockPcs;
            Model.ToStockSide = Entity.ToStockSide;
            Model.ToStockArea = Entity.ToStockArea;
            Model.AreaUnit = Entity.AreaUnit;
            Model.UnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;

            return Model;
        }

        #endregion


        #region CONFIRM Crust Leather Transfer Treatment
        public ValidationMsg ConfirmCrustleatherTransferTreatmentStock(long CLTransferID, int userid)
        {
            try
            {
                var dt = new DataTable();
                using (var tx = new TransactionScope())
                {
                    using (var conn = new SqlConnection(_connString))
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "UspConfirmCrustLeatherTransferTreatment";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@CLTransferID", SqlDbType.BigInt).Value = CLTransferID;
                            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userid;
                            using (var adp = new SqlDataAdapter(cmd))
                            {
                                adp.Fill(dt);
                            }
                        }

                    }
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt.Rows[0]["IsStock"]))
                        {
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Confirmation Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Not Enough Quantity In Stock.";
                            return _vmMsg;

                        }
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Not Enough Quantity In Stock.";
                        return _vmMsg;

                    }

                }
            }

            catch (Exception ex)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Confirmation Failed.";
            }

            return _vmMsg;
        }
        #endregion

    }
}

