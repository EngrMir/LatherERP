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

    public class DalFinishedLeatherStoreTransferReceive
    {
        private UnitOfWork repository = new UnitOfWork();
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;

        public long FNTransferFromID = 0;
        public long FNTransferToID = 0;
        public long FNTransferID = 0;
        public string FNTransferNo = string.Empty;

        public long FNReceiveID = 0;
        public string FNReceiveNo = string.Empty;
        private bool save;

        //*********************************************************** GET ALL DATA *****************************************************************
        public DalFinishedLeatherStoreTransferReceive()
        {
            _vmMsg = new ValidationMsg();
            _context = new BLC_DEVEntities();
        }
        public List<InvFinishLeatherReceive> GetPopUpDetail(string IssueFrom)
        {
            var queryString = @"SELECT												
					                            fli.FinishLeatherIssueID,
					
				                                ISNULL(fli.FinishLeatherIssueNo,'')FinishLeatherIssueNo,
				                                CASE  fli.IssueCategory	
						                                WHEN 'STTF' THEN 'Store Transfer'
				                                END IssueCategory,
				                                fli.IssueFrom,
				                                ISNULL(s.StoreName,'')IssueFromName,
				                                CASE  fli.RecordStatus	
					                                WHEN 'NCF' THEN 'Not Confirmed'
					                                WHEN 'CHK' THEN 'Checked'
					                                WHEN 'CNF' THEN 'Confirmed'
				                                END RecordStatus
                                FROM			INV_FinishLeatherIssue fli
	
                                LEFT JOIN		SYS_Store s ON fli.IssueFrom=s.StoreID
                                    WHERE		    fli.IssueFrom='" + IssueFrom + "' AND fli.IssueCategory='STTF' AND fli.RecordStatus='CNF' ORDER BY	fli.FinishLeatherIssueID DESC";
                var stockList = _context.Database.SqlQuery<InvFinishLeatherReceive>(queryString);
                return stockList.ToList();
        }
        public List<InvFinishLeatherReceiveItem> GetItemGridStockDetail(long FinishLeatherIssueID)
        {
            var queryString = @"SELECT	
											fli.FinishLeatherIssueID,
											ISNULL(fli.FinishLeatherIssueNo,'') FinishLeatherIssueNo,
											CASE  fli.IssueCategory	
												WHEN 'STTF' THEN 'Store Transfer'
											END IssueCategory,
											fli.IssueFrom,
				                            ISNULL(s.StoreName,'')IssueFromName,
											CASE  fli.RecordStatus	
														WHEN 'NCF' THEN 'Not Confirmed'
														WHEN 'CHK' THEN 'Checked'
														WHEN 'CNF' THEN 'Confirmed'
											END RecordStatus,
											flii.FinishLeatherIssueID,
											flii.FinishLeatherIssueItemID,
											flii.BuyerID,
                                            flii.BuyerOrderID,
											flii.ArticleID,
											flii.ItemTypeID,
											flii.LeatherTypeID,
											flii.LeatherStatusID,
											ISNULL(b.BuyerName,'')BuyerName,
											ISNULL(bo.BuyerOrderNo,'')BuyerOrderNo,
											ISNULL(flii.ArticleNo,'')ArticleNo,
											ISNULL(flii.ArticleColorNo,0)ArticleColorNo,
											ISNULL(flii.ArticleChallanNo,'')ArticleChallanNo,
											ISNULL(i.ItemTypeName,'')ItemTypeName,
											ISNULL(l.LeatherStatusName,'')LeatherStatusName
									
								
									FROM
												(SELECT 
														FinishLeatherIssueID,
														FinishLeatherIssueNo,
														FinishLeatherIssueDate,
														IssueCategory,
														IssueFrom,
														IssueTo,
														RecordStatus 
												FROM	INV_FinishLeatherIssue 
												WHERE	IssueCategory='STTF' AND RecordStatus='CNF' )fli
									INNER JOIN		
												(SELECT 
														FinishLeatherIssueID,
														FinishLeatherIssueItemID,
														BuyerID,
														BuyerOrderID,
														ArticleID,
														ArticleNo,
														ArticleChallanNo,
														ArticleColorNo,
														ItemTypeID,
														LeatherTypeID,
														LeatherStatusID 
												FROM	INV_FinishLeatherIssueItem )flii	ON		fli.FinishLeatherIssueID=flii.FinishLeatherIssueID
									

									LEFT JOIN			Sys_Buyer b ON flii.BuyerID=b.BuyerID
                                    LEFT JOIN			SLS_BuyerOrder bo ON flii.BuyerOrderID=bo.BuyerOrderID
                                    LEFT JOIN			Sys_ItemType i ON flii.ItemTypeID=i.ItemTypeID
                                    LEFT JOIN			Sys_LeatherStatus l ON flii.LeatherStatusID=l.LeatherStatusID
                                    LEFT JOIN			SYS_Store s ON fli.IssueFrom=s.StoreID
									WHERE fli.FinishLeatherIssueID='" + FinishLeatherIssueID + "'";
            var stockList = _context.Database.SqlQuery<InvFinishLeatherReceiveItem>(queryString);
            return stockList.ToList();
        }
        public List<InvFinishLeatherReceiveColor> GetColorGridStockDetail(long FinishLeatherIssueItemID)
        {
            var queryString = @"SELECT	
											flic.FinishLeatherIssueID,
											flic.FinishLeatherIssueItemID,
											flic.FinishLeatherIssueColorID,
											flic.ColorID,
											ISNULL(c.ColorName,0)ColorName,
											flic.GradeID,
											ISNULL(g.GradeName,0)GradeName,
											ISNULL(flic.FinishQCLabel,'')FinishQCLabel,
											ISNULL(flic.IssuePcs,0)IssuePcs,
											ISNULL(flic.IssueSide,0)IssueSide,
											ISNULL(flic.IssueArea,0)IssueArea,
											ISNULL(flic.SideArea,0)SideArea,
											ISNULL(flic.AreaUnit,0)AreaUnit,
                                            ISNULL(u.UnitName,'')UnitName,
											ISNULL(flic.ArticleColorNo,0)ArticleColorNo
									FROM
												
												(SELECT 
														FinishLeatherIssueID,
														FinishLeatherIssueItemID
												 FROM	INV_FinishLeatherIssueItem )flii	
												 
									INNER JOIN	(SELECT 
														FinishLeatherIssueID,
														FinishLeatherIssueItemID,
														FinishLeatherIssueColorID,
														ColorID,
														GradeID,
														FinishQCLabel,
														IssuePcs,
														IssueSide,
														IssueArea,
														SideArea,
														AreaUnit,
														ArticleColorNo 
												FROM	INV_FinishLeatherIssueColor) flic	ON		flii.FinishLeatherIssueItemID=flic.FinishLeatherIssueItemID		
									LEFT JOIN			Sys_Grade g ON flic.GradeID=g.GradeID
									LEFT JOIN			Sys_Color c ON flic.ColorID=c.ColorID
                                    LEFT JOIN			Sys_Unit u ON flic.AreaUnit=u.UnitID
	                                WHERE               flii.FinishLeatherIssueItemID='" + FinishLeatherIssueItemID + "'";
            var stockList = _context.Database.SqlQuery<InvFinishLeatherReceiveColor>(queryString);
            return stockList.ToList();
        }
        public List<InvFinishLeatherReceive> GetIssueFromStockDetail()
        {
            var queryString = @"SELECT 
				                            s.StoreID,
				                            s.StoreCode,
				                            s.StoreName 
		                        FROM	    INV_FinishLeatherIssue fli
		                        INNER JOIN  SYS_Store s ON s.StoreID = fli.IssueFrom
								WHERE       s.StoreCategory='Leather' AND s.StoreType='Finish' AND IsActive='true' AND IsDelete='False'
		                        ORDER BY    fli.FinishLeatherIssueID DESC";

            var stockList = _context.Database.SqlQuery<InvFinishLeatherReceive>(queryString);
            return stockList.ToList();

        }

        //*********************************************************** END OF GET ALL DATA *****************************************************************


        // ###########################################   SAVE,UPDATE,DELETE,CONFIRM #######################################################################

        public long GetFNReceiveID()
        {
            return FNReceiveID;
        }

        public string GetFNReceiveNo()
        {
            return FNReceiveNo;
        }




        #region SAVE,UPDATE,DELETE,CONFIRM
        public ValidationMsg Save(InvFinishLeatherReceive model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {

                        model.FNReceiveNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.FNReceiveNo != null)
                        {
                            #region Save Finish Leather Receive Data

                            INV_FinishLeatherReceive tblFinishLeatherReceive = SetToModelObject(model, userid);
                            _context.INV_FinishLeatherReceive.Add(tblFinishLeatherReceive);
                            _context.SaveChanges();
                            #endregion

                            #region Save Finish Leather Receive Item Data
                            if (model.InvFinishLeatherReceiveItemList != null)
                            {
                                foreach (InvFinishLeatherReceiveItem objFinishLeatherReceiveItem in model.InvFinishLeatherReceiveItemList)
                                {
                                    objFinishLeatherReceiveItem.FNReceiveID = tblFinishLeatherReceive.FNReceiveID;
                                    INV_FinishLeatherReceiveItem tblFinishLeatherReceiveItem = SetToIssueItemModelObject(objFinishLeatherReceiveItem, userid);
                                    _context.INV_FinishLeatherReceiveItem.Add(tblFinishLeatherReceiveItem);
                                    _context.SaveChanges();

                                    #region Save Finish Leather Receive Color Data

                                    if (model.InvFinishLeatherReceiveColorList != null)
                                    {
                                        if (objFinishLeatherReceiveItem.FNReceiveItemID != null)
                                        {
                                            foreach (InvFinishLeatherReceiveColor objFinishLeatherReceiveColor in model.InvFinishLeatherReceiveColorList)
                                            {
                                                objFinishLeatherReceiveColor.FNReceiveID = tblFinishLeatherReceiveItem.FNReceiveID;
                                                objFinishLeatherReceiveColor.FNReceiveItemID = tblFinishLeatherReceiveItem.FNReceiveItemID;


                                                INV_FinishLeatherReceiveColor tblFinishLeatherReceiveColor = SetToReceiveColorModelObject(objFinishLeatherReceiveColor, userid);
                                                _context.INV_FinishLeatherReceiveColor.Add(tblFinishLeatherReceiveColor);
                                                _context.SaveChanges();
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion

                            _context.SaveChanges();

                            tx.Complete();
                            //FNReceiveID = tblFinishLeatherReceive.FNReceiveID;
                            //FNReceiveNo = tblFinishLeatherReceive.FNReceiveNo;

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

        public ValidationMsg Update(InvFinishLeatherReceive model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {


                        var _LeatherType = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                        

                        #region Crust Leather Transfer

                        INV_FinishLeatherReceive IssueEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.INV_FinishLeatherReceive.First(m => m.FNReceiveID == model.FNReceiveID);

                        OriginalEntity.FNReceiveID = IssueEntity.FNReceiveID;
                        OriginalEntity.FNReceiveNo = IssueEntity.FNReceiveNo;
                        OriginalEntity.FNReceiveDate = IssueEntity.FNReceiveDate;
                        OriginalEntity.ReceiveCategory = IssueEntity.ReceiveCategory;
                        OriginalEntity.IssueFrom = IssueEntity.IssueFrom;
                        OriginalEntity.ReceiveAt = IssueEntity.ReceiveAt;
                        OriginalEntity.RecordStatus = IssueEntity.RecordStatus;
                        OriginalEntity.ReceiveNote = IssueEntity.ReceiveNote;
                        OriginalEntity.CheckNote = IssueEntity.CheckNote;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        _context.SaveChanges();
                        #endregion

                       #region Save NEW Data & Update Existing Crust Leather Transfer Data

                        if (model.InvFinishLeatherReceiveItemList != null)
                        {
                            foreach (InvFinishLeatherReceiveItem objFinishedLeatherIssueItem in model.InvFinishLeatherReceiveItemList)
                            {
                                objFinishedLeatherIssueItem.FNReceiveID = model.FNReceiveID;



                                INV_FinishLeatherReceiveItem CurrentIssueItemEntity = SetToIssueItemModelObject(objFinishedLeatherIssueItem, userid);
                                var OriginalIssueItemEntity = _context.INV_FinishLeatherReceiveItem.First(m => m.FNReceiveID == model.FNReceiveID);


                                OriginalIssueItemEntity.FNReceiveID = CurrentIssueItemEntity.FNReceiveID;
                                OriginalIssueItemEntity.FNReceiveItemID = CurrentIssueItemEntity.FNReceiveItemID;
                                //OriginalIssueItemEntity.FinishLeatherIssueID = CurrentIssueItemEntity.FinishLeatherIssueID;
                                //OriginalIssueItemEntity.FinishLeatherIssueItemID = CurrentIssueItemEntity.FinishLeatherIssueItemID;
                                OriginalIssueItemEntity.BuyerID = CurrentIssueItemEntity.BuyerID;
                                OriginalIssueItemEntity.BuyerOrderID = CurrentIssueItemEntity.BuyerOrderID;
                                OriginalIssueItemEntity.ItemTypeID = CurrentIssueItemEntity.ItemTypeID;
                                OriginalIssueItemEntity.LeatherStatusID = CurrentIssueItemEntity.LeatherStatusID;
                                OriginalIssueItemEntity.LeatherTypeID = _LeatherType;
                                //OriginalIssueItemEntity.ColorID = CurrentIssueItemEntity.ColorID;
                                //OriginalIssueItemEntity.GradeRange = CurrentIssueItemEntity.GradeRange;
                                //OriginalIssueItemEntity.ArticleChallanID = CurrentIssueItemEntity.ArticleChallanID;
                                //OriginalIssueItemEntity.ArticleColorNo = CurrentIssueItemEntity.ArticleColorNo;
                                OriginalIssueItemEntity.ArticleID = CurrentIssueItemEntity.ArticleID;
                                OriginalIssueItemEntity.ArticleNo = CurrentIssueItemEntity.ArticleNo;
                                //OriginalIssueItemEntity.ArticleChallanNo = CurrentIssueItemEntity.ArticleChallanNo;
                                OriginalIssueItemEntity.FinishQCLabel = CurrentIssueItemEntity.FinishQCLabel;
                                OriginalIssueItemEntity.ModifiedBy = userid;
                                OriginalIssueItemEntity.ModifiedOn = DateTime.Now;
                                OriginalIssueItemEntity.IPAddress = GetIPAddress.LocalIPAddress();



                                #region Save CrustQC Records

                                if (model.InvFinishLeatherReceiveColorList != null)
                                {
                                    foreach (InvFinishLeatherReceiveColor objCLTransferToItem in model.InvFinishLeatherReceiveColorList)
                                    {
                                        objCLTransferToItem.FNReceiveID = model.FNReceiveID;
                                        objCLTransferToItem.FNReceiveItemID = objFinishedLeatherIssueItem.FNReceiveItemID;
                                        // objCLTransferToItem.CLTransferFromID = objCLTransferItem.CLTransferFromID;

                                        //if (objCLTransferToItem.CLTransferToID == 0)
                                        //{
                                        //    objCLTransferToItem.CLTransferFromID = objCLTransferItem.CLTransferFromID;
                                        //    objCLTransferToItem.CLTransferID = model.CLTransferID;
                                        //    objCLTransferToItem.CLTransferNo = model.CLTransferNo;
                                        //    //objCLTransferToItem.ScheduleProductionNo = objCLTransferItem.ScheduleProductionNo;

                                        //    INV_CLTransferTo tblQCSelection = SetToCLTransferToModelObject(objCLTransferToItem, userid);
                                        //    _context.INV_CLTransferTo.Add(tblQCSelection);
                                        //}
                                        //else
                                        //{
                                        INV_FinishLeatherReceiveColor CurrIssueGradeEntity = SetToReceiveColorModelObject(objCLTransferToItem, userid);
                                        var OrgrEntity = _context.INV_FinishLeatherReceiveColor.First(m => m.FNReceiveColorID == objCLTransferToItem.FNReceiveColorID);


                                        //OrgrEntity.FNReceiveColorID = CurrIssueGradeEntity.FNReceiveColorID;
                                        //OrgrEntity.FNReceiveItemID = CurrIssueGradeEntity.FNReceiveItemID;
                                        //OrgrEntity.FNReceiveID = CurrIssueGradeEntity.FNReceiveID;
                                        //OrgrEntity.FinishLeatherIssueID = CurrIssueGradeEntity.FinishLeatherIssueID;
                                        //OrgrEntity.FinishLeatherIssueItemID = CurrIssueGradeEntity.FinishLeatherIssueItemID;
                                        //OrgrEntity.FinishLeatherIssueColorID = CurrIssueGradeEntity.FinishLeatherIssueColorID;


                                        OrgrEntity.ColorID = CurrIssueGradeEntity.ColorID;
                                        OrgrEntity.GradeID = CurrIssueGradeEntity.GradeID;
                                        OrgrEntity.FinishQCLabel = CurrIssueGradeEntity.FinishQCLabel;
                                        OrgrEntity.IssuePcs = CurrIssueGradeEntity.IssuePcs;
                                        OrgrEntity.IssueSide = CurrIssueGradeEntity.IssueSide;
                                        OrgrEntity.IssueArea = CurrIssueGradeEntity.IssueArea;
                                        OrgrEntity.SideArea = CurrIssueGradeEntity.SideArea;
                                        OrgrEntity.ReceivePcs = CurrIssueGradeEntity.ReceivePcs;
                                        OrgrEntity.ReceiveSide = CurrIssueGradeEntity.ReceiveSide;
                                        OrgrEntity.ReceiveArea = CurrIssueGradeEntity.ReceiveArea;
                                        OrgrEntity.ReceiveSideArea = CurrIssueGradeEntity.ReceiveSideArea;
                                        //OrgrEntity.ArticleColorNo = CurrIssueGradeEntity.ArticleColorNo;
                                        OrgrEntity.AreaUnit = CurrIssueGradeEntity.AreaUnit;

                                        OrgrEntity.ModifiedBy = userid;
                                        OrgrEntity.ModifiedOn = DateTime.Now;
                                        OrgrEntity.IPAddress = GetIPAddress.LocalIPAddress();
                                        OrgrEntity.SetOn = DateTime.Now;
                                        OrgrEntity.SetBy = userid;
                                        // }
                                    }
                                }

                                #endregion
                            }
                        }

                        #endregion 



                        _context.SaveChanges();
                        tx.Complete();
                        FNReceiveID = model.FNReceiveID;
                        FNReceiveNo = model.FNReceiveNo;
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
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Update.";
            }
            return _vmMsg;
        }


        #region SEARCH Finish Store Transfer DATA

        public List<InvFinishLeatherReceive> SearchMasterInfo()
        {
            var query = @"SELECT		flr.*  ,
                                        fli.FinishLeatherIssueNo        
                            FROM
                                (SELECT 
                                        FNReceiveID,
                                        FNReceiveNo,
                                        CONVERT(VARCHAR(12),FNReceiveDate, 106) FNReceiveDate,
                                        CASE	ReceiveCategory	
                                                WHEN 'STTF' THEN 'Store Transfer'
                                        END ReceiveCategory,
                                        IssueFrom,
                                        (SELECT StoreName FROM SYS_Store WHERE IssueFrom=StoreID)IssueFromName,
                                        ReceiveAt,
                                        (SELECT StoreName FROM SYS_Store WHERE IssueFrom=StoreID)ReceiveAtName,
                                        CASE RecordStatus	WHEN 'NCF' THEN 'Not Confirm'
                                                            WHEN 'CNF' THEN 'Confirmed'
                                                            WHEN 'CHK' THEN 'Checked'
                                                            WHEN 'APV' THEN 'Approved'
                                        END RecordStatus,
                                        ISNULL(ReceiveNote,'')ReceiveNote,
                                        ISNULL(CheckNote,'')CheckNote
                                FROM	INV_FinishLeatherReceive )flr
                                INNER JOIN INV_FinishLeatherReceiveItem flri ON flr.FNReceiveID=flri.FNReceiveID
                                INNER JOIN INV_FinishLeatherIssue fli ON flri.FinishLeatherIssueID=fli.FinishLeatherIssueID";
            var result = _context.Database.SqlQuery<InvFinishLeatherReceive>(query).ToList();
            return result;

        }



        public List<InvFinishLeatherReceive> SearchItemInfo(long FNReceiveID)
        {
            var query = @"SELECT		
		                        flri.*,
		                        b.BuyerName,
		                        bo.BuyerOrderNo,
		                        i.ItemTypeName,
		                        l.LeatherStatusName
		                                
                                FROM
			                        (SELECT 
				                        ISNULL(FNReceiveID,0)FNReceiveID,
				                        ISNULL(FNReceiveItemID,0)FNReceiveItemID,
				                        ISNULL(FinishLeatherIssueID,0)FinishLeatherIssueID,								
				                        ISNULL(FinishLeatherIssueItemID,0)FinishLeatherIssueItemID,
				                        ISNULL(BuyerID,0)BuyerID,
				                        ISNULL(BuyerOrderID,0)BuyerOrderID,
				                        ISNULL(ArticleID,0)ArticleID,
				                        ISNULL(ArticleNo,'')ArticleNo,
				                        ISNULL(ArticleChallanNo,'')ArticleChallanNo,
				                        ISNULL(ItemTypeID,0)ItemTypeID,
				                        ISNULL(LeatherTypeID,0)LeatherTypeID,
				                        ISNULL(LeatherStatusID,0)LeatherStatusID 
	                        FROM	    INV_FinishLeatherReceiveItem )flri 
		                        
		                    LEFT JOIN			Sys_Buyer b					ON flri.BuyerID=b.BuyerID
		                    LEFT JOIN			SLS_BuyerOrder bo			ON flri.BuyerOrderID=bo.BuyerOrderID
		                    LEFT JOIN			Sys_ItemType i				ON flri.ItemTypeID=i.ItemTypeID
		                    LEFT JOIN			Sys_LeatherStatus l			ON flri.LeatherStatusID=l.LeatherStatusID
                            WHERE       flri.FNReceiveID='" + FNReceiveID + "'";
            var result = _context.Database.SqlQuery<InvFinishLeatherReceive>(query).ToList();
            return result;

        }


        public List<InvFinishLeatherReceive> SearchReceiveItemColorDetailInfo(long FNReceiveItemID)
        {
            var query = @"SELECT			
	                            flrc.*,
	                            c.ColorName,
	                            g.GradeName,
	                            u.UnitName
                            FROM
	                            (SELECT 
				                            ISNULL(FNReceiveID,0)FNReceiveID,
				                            ISNULL(FNReceiveItemID,0)FNReceiveItemID,
				                            ISNULL(FNReceiveColorID,0)FNReceiveColorID,
				                            ISNULL(FinishLeatherIssueID,0)FinishLeatherIssueID,
				                            ISNULL(FinishLeatherIssueItemID,0)FinishLeatherIssueItemID,
				                            ISNULL(FinishLeatherIssueColorID,0)FinishLeatherIssueColorID,
				                            ISNULL(ColorID,0)ColorID,
				                            ISNULL(GradeID,0)GradeID,
				                            ISNULL(IssuePcs,0)IssuePcs,
				                            ISNULL(IssueSide,0)IssueSide,
				                            ISNULL(IssueArea,0)IssueArea,
				                            ISNULL(SideArea,0)SideArea,
				                            ISNULL(ReceivePcs,0)ReceivePcs,
				                            ISNULL(ReceiveSide,0)ReceiveSide,
				                            ISNULL(ReceiveArea,0)ReceiveArea,
				                            ISNULL(ReceiveSideArea,0)ReceiveSideArea,
				                            ISNULL(AreaUnit,0)AreaUnit,
				                            ISNULL(FinishQCLabel,'')FinishQCLabel
                            FROM			INV_FinishLeatherReceiveColor )flrc	
                            LEFT JOIN		Sys_Color c ON flrc.ColorID=c.ColorID
                            LEFT JOIN		Sys_Grade g ON flrc.GradeID=g.GradeID
                            LEFT JOIN		Sys_Unit u ON flrc.AreaUnit=u.UnitID
                                WHERE       flrc.FNReceiveItemID='" + FNReceiveItemID + "'";
            var result = _context.Database.SqlQuery<InvFinishLeatherReceive>(query.ToString()).ToList();
            return result;
        }

        #endregion


        #endregion

        // ########################################### END OF SAVE,UPDATE,DELETE,CONFIRM #######################################################################


        //************************************************************ For Saving use Entity & Model Data Mapping **********************************
        public INV_FinishLeatherReceive SetToModelObject(InvFinishLeatherReceive model, int userid)
        {
            INV_FinishLeatherReceive Entity = new INV_FinishLeatherReceive();

            Entity.FNReceiveID = model.FNReceiveID;
            Entity.FNReceiveNo = model.FNReceiveNo;
            Entity.FNReceiveDate = DalCommon.SetDate(model.FNReceiveDate);
            Entity.ReceiveCategory = "STTF";
            Entity.ReceiveFor = model.ReceiveFor == null ? "" : model.ReceiveFor;
            Entity.IssueFrom = model.IssueFrom == null ? 0 : model.IssueFrom;
            Entity.ReceiveAt = model.ReceiveAt == null ? 0 : model.ReceiveAt;
            Entity.ReceiveNote = model.ReceiveNote == null ? "" : model.ReceiveNote;
            Entity.CheckNote = model.CheckNote == null ? "" : model.CheckNote;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            //Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }
        public INV_FinishLeatherReceiveItem SetToIssueItemModelObject(InvFinishLeatherReceiveItem model, int userid)
        {
            INV_FinishLeatherReceiveItem Entity = new INV_FinishLeatherReceiveItem();

            Entity.FNReceiveID = model.FNReceiveID;
            Entity.FNReceiveItemID = model.FNReceiveItemID;
            Entity.FinishLeatherIssueID = model.FinishLeatherIssueID;
            Entity.FinishLeatherIssueItemID = model.FinishLeatherIssueItemID;
            Entity.BuyerID = model.BuyerID == null ? 0 : model.BuyerID;
            Entity.BuyerOrderID = model.BuyerOrderID == null ? 0 : model.BuyerOrderID;
            Entity.ArticleID = model.ArticleID == null ? 0 : model.ArticleID;
            Entity.ArticleNo = model.ArticleNo == null ? "" : model.ArticleNo;
            //Entity.ArticleChallanID = model.ArticleChallanID == null ? 0 : model.ArticleChallanID;
            Entity.ArticleChallanNo = model.ArticleChallanNo == null ? "" : model.ArticleChallanNo;
            Entity.LeatherStatusID = model.LeatherStatusID == null ? 0 : model.LeatherStatusID;
            Entity.ItemTypeID = model.ItemTypeID == null ? 0 : model.ItemTypeID;
            Entity.LeatherTypeID = model.LeatherTypeID == null ? 0 : model.LeatherTypeID;

            //var _LeatherType = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
            //Entity.LeatherStatusID = model.LeatherStatusID == null ? 0 : model.LeatherStatusID;
            Entity.FinishQCLabel = model.FinishQCLabel == null ? "" : model.FinishQCLabel;
            //Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            //Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }
        public INV_FinishLeatherReceiveColor SetToReceiveColorModelObject(InvFinishLeatherReceiveColor model, int userid)
        {
            INV_FinishLeatherReceiveColor Entity = new INV_FinishLeatherReceiveColor();

            //Entity.FNReceiveColorID = model.FNReceiveColorID;
            Entity.FNReceiveItemID = model.FNReceiveItemID;
            Entity.FNReceiveID = model.FNReceiveID;
            Entity.FinishLeatherIssueID = model.FinishLeatherIssueID;
            Entity.FinishLeatherIssueItemID = model.FinishLeatherIssueItemID;
            Entity.FinishLeatherIssueColorID = model.FinishLeatherIssueColorID;
            Entity.ColorID = model.ColorID == null ? 0 : model.ColorID;
            Entity.GradeID = model.GradeID == null ? 0 : model.GradeID;
            Entity.FinishQCLabel = model.FinishQCLabel == null ? "" : model.FinishQCLabel;
            Entity.IssuePcs = model.IssuePcs == null ? 0 : model.IssuePcs;
            Entity.IssueSide = model.IssueSide == null ? 0 : model.IssueSide;
            Entity.IssueArea = model.IssueArea == null ? 0 : model.IssueArea;
            Entity.SideArea = model.SideArea == null ? 0 : model.SideArea;
            Entity.AreaUnit = model.AreaUnit == null ? 0 : model.AreaUnit;
            Entity.ReceivePcs = model.ReceivePcs == null ? 0 : model.ReceivePcs;
            Entity.ReceiveSide = model.ReceiveSide == null ? 0 : model.ReceiveSide;
            Entity.ReceiveArea = model.ReceiveArea == null ? 0 : model.ReceiveArea;
            Entity.ReceiveSideArea = model.ReceiveSideArea == null ? 0 : model.ReceiveSideArea;
           // Entity.ArticleColorNo = model.ArticleColorNo == null ? 0 : model.ArticleColorNo;
           // Entity.GradeRange = model.GradeRange == null ? "" : model.GradeRange;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }
        //*********************************************************** END of Entity & Model Data Mapping ********************************************
    }
}
