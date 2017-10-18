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
    public class DalFinishLeatherQC
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long CrustLeatherQCID = 0;
        //public long ScheduleDateID = 0;
        public string CrustLeatherQCNo = string.Empty;
        public DalFinishLeatherQC()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(PrdCrustLeatherQC model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.CrustLeatherQCNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.CrustLeatherQCNo != null)
                        {
                            #region CrustLeatherQC

                            PRD_FinishLeatherQC tblQC = SetToModelObject(model, userid);
                            _context.PRD_FinishLeatherQC.Add(tblQC);
                            _context.SaveChanges();

                            #endregion

                            #region Save QCItem Records

                            if (model.PrdCrustLeatherQCItemList != null)
                            {
                                foreach (PrdCrustLeatherQCItem objQCItem in model.PrdCrustLeatherQCItemList)
                                {
                                    objQCItem.CrustLeatherQCID = tblQC.FinishLeatherQCID;
                                    PRD_FinishLeatherQCItem tblQCItem = SetToModelObject(objQCItem, userid);
                                    _context.PRD_FinishLeatherQCItem.Add(tblQCItem);
                                    _context.SaveChanges();

                                    #region Save QCSelection List

                                    if (model.PrdCrustLeatherQCSelectionList != null)
                                    {
                                        foreach (PrdCrustLeatherQCSelection objQCSelection in model.PrdCrustLeatherQCSelectionList)
                                        {
                                            objQCSelection.CLQCItemID = tblQCItem.FNQCItemID;
                                            objQCSelection.CrustLeatherQCID = tblQC.FinishLeatherQCID;
                                            objQCSelection.ScheduleItemID = objQCItem.ScheduleItemID;
                                            objQCSelection.ScheduleProductionNo = objQCItem.ScheduleProductionNo;
                                            PRD_FinishLeatherQCSelection tblQCSelection = SetToModelObject(objQCSelection, userid);
                                            _context.PRD_FinishLeatherQCSelection.Add(tblQCSelection);
                                        }
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            _context.SaveChanges();
                            tx.Complete();
                            CrustLeatherQCID = tblQC.FinishLeatherQCID;
                            CrustLeatherQCNo = model.CrustLeatherQCNo;

                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "CrustLeatherQCNo Predefine Value not Found.";
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

        public ValidationMsg Update(PrdCrustLeatherQC model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                #region Update

                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        #region CrustLeatherQC

                        PRD_FinishLeatherQC CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.PRD_FinishLeatherQC.First(m => m.FinishLeatherQCID == model.CrustLeatherQCID);

                        OriginalEntity.FinishLeatherQCDate = CurrentEntity.FinishLeatherQCDate;
                        OriginalEntity.QCTransactionOf = CurrentEntity.QCTransactionOf;
                        OriginalEntity.ProductionFloor = CurrentEntity.ProductionFloor;
                        OriginalEntity.AfterQCIssueStore = CurrentEntity.AfterQCIssueStore;
                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #endregion


                        #region Save QCItem List

                        if (model.PrdCrustLeatherQCItemList != null)
                        {
                            foreach (PrdCrustLeatherQCItem objQCItem in model.PrdCrustLeatherQCItemList)
                            {
                                if (objQCItem.SdulItemColorID == 0)
                                {
                                    objQCItem.CrustLeatherQCID = model.CrustLeatherQCID;
                                    PRD_FinishLeatherQCItem tblQCItem = SetToModelObject(objQCItem, userid);
                                    _context.PRD_FinishLeatherQCItem.Add(tblQCItem);
                                    _context.SaveChanges();
                                    objQCItem.CLQCItemID = tblQCItem.FNQCItemID;
                                }
                                else
                                {
                                    PRD_FinishLeatherQCItem CurrEntity = SetToModelObject(objQCItem, userid);
                                    var OrgrEntity = _context.PRD_FinishLeatherQCItem.First(m => m.FNQCItemID == objQCItem.CLQCItemID);

                                    OrgrEntity.ScheduleItemID = CurrEntity.ScheduleItemID;
                                    OrgrEntity.ScheduleProductionNo = CurrEntity.ScheduleProductionNo;
                                    OrgrEntity.BuyerID = CurrEntity.BuyerID;
                                    OrgrEntity.BuyerOrderID = CurrEntity.BuyerOrderID;
                                    OrgrEntity.ArticleID = CurrEntity.ArticleID;
                                    OrgrEntity.ArticleNo = CurrEntity.ArticleNo;
                                    OrgrEntity.ColorID = CurrEntity.ColorID;
                                    OrgrEntity.ItemTypeID = CurrEntity.ItemTypeID;
                                    OrgrEntity.LeatherTypeID = CurrEntity.LeatherTypeID;
                                    OrgrEntity.LeatherStatusID = CurrEntity.LeatherStatusID;
                                    OrgrEntity.ProductionPcs = CurrEntity.ProductionPcs;
                                    OrgrEntity.ProductionSide = CurrEntity.ProductionSide;
                                    OrgrEntity.Productionrea = CurrEntity.Productionrea;
                                    OrgrEntity.Remarks = CurrEntity.Remarks;

                                    OrgrEntity.ModifiedBy = userid;
                                    OrgrEntity.ModifiedOn = DateTime.Now;
                                }

                                #region Save QCSelection Records

                                if (model.PrdCrustLeatherQCSelectionList != null)
                                {
                                    foreach (PrdCrustLeatherQCSelection objQCSelection in model.PrdCrustLeatherQCSelectionList)
                                    {
                                        if (objQCSelection.CLQCSelectionID == 0)
                                        {
                                            objQCSelection.CLQCItemID = objQCItem.CLQCItemID;
                                            objQCSelection.CrustLeatherQCID = model.CrustLeatherQCID;
                                            objQCSelection.ScheduleItemID = objQCItem.ScheduleItemID;
                                            objQCSelection.ScheduleProductionNo = objQCItem.ScheduleProductionNo;

                                            PRD_FinishLeatherQCSelection tblQCSelection = SetToModelObject(objQCSelection, userid);
                                            _context.PRD_FinishLeatherQCSelection.Add(tblQCSelection);
                                        }
                                        else
                                        {
                                            PRD_FinishLeatherQCSelection CurEntity = SetToModelObject(objQCSelection, userid);
                                            var OrgEntity = _context.PRD_FinishLeatherQCSelection.First(m => m.FNQCSelectionID == objQCSelection.CLQCSelectionID);

                                            OrgEntity.ScheduleItemID = CurEntity.ScheduleItemID;
                                            OrgEntity.ScheduleProductionNo = CurEntity.ScheduleProductionNo;
                                            OrgEntity.GradeID = CurEntity.GradeID;
                                            OrgEntity.ProductionPcs = CurEntity.ProductionPcs;
                                            OrgEntity.ProductionSide = CurEntity.ProductionSide;
                                            OrgEntity.ProductionArea = CurEntity.ProductionArea;
                                            OrgEntity.ProductionAreaUnit = CurEntity.ProductionAreaUnit;
                                            OrgEntity.QCPassPcs = CurEntity.QCPassPcs;
                                            OrgEntity.QCPassSide = CurEntity.QCPassSide;
                                            OrgEntity.QCPassArea = CurEntity.QCPassArea;
                                            OrgEntity.QCPassAreaUnit = CurEntity.QCPassAreaUnit;
                                            OrgEntity.QCPassRemarks = CurEntity.QCPassRemarks;
                                            OrgEntity.QCFailPcs = CurEntity.QCFailPcs;
                                            OrgEntity.QCFailSide = CurEntity.QCFailSide;
                                            OrgEntity.QCFailArea = CurEntity.QCFailArea;
                                            OrgEntity.QCFailAreaUnit = CurEntity.QCFailAreaUnit;
                                            OrgEntity.QCFailRemarks = CurEntity.QCFailRemarks;

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
                        CrustLeatherQCID = model.CrustLeatherQCID;
                        CrustLeatherQCNo = model.CrustLeatherQCNo;
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

        public PRD_FinishLeatherQC SetToModelObject(PrdCrustLeatherQC model, int userid)
        {
            PRD_FinishLeatherQC Entity = new PRD_FinishLeatherQC();

            Entity.FinishLeatherQCID = model.CrustLeatherQCID;
            Entity.FinishLeatherQCNo = model.CrustLeatherQCNo;
            Entity.FinishLeatherQCDate = DalCommon.SetDate(model.CrustLeatherQCDate);
            Entity.ProductionFloor = model.ProductionFloor;
            Entity.AfterQCIssueFloor = model.AfterQCIssueFloor;
            Entity.AfterQCIssueStore = model.AfterQCIssueStore;
            Entity.QCTransactionOf = "FOQC";//Own QC
            Entity.Remarks = model.Remarks;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public PRD_FinishLeatherQCItem SetToModelObject(PrdCrustLeatherQCItem model, int userid)
        {
            PRD_FinishLeatherQCItem Entity = new PRD_FinishLeatherQCItem();

            Entity.FNQCItemID = model.CLQCItemID;
            //Entity.CrustLeatherQCRefID = model.CrustLeatherQCRefID == 0 ? null : model.CrustLeatherQCRefID;
            Entity.FinishLeatherQCID = model.CrustLeatherQCID;
            Entity.ScheduleItemID = model.ScheduleItemID;
            Entity.ScheduleProductionNo = model.ScheduleProductionNo;
            Entity.BuyerID = model.BuyerID;
            Entity.BuyerOrderID = model.BuyerOrderID;
            Entity.ArticleID = model.ArticleID;
            Entity.ArticleNo = model.ArticleNo;
            Entity.ArticleChallanID = model.ArticleChallanID;
            Entity.ArticleChallanNo = model.ArticleChallanNo;
            Entity.ColorID = model.ColorID;
            Entity.ArticleColorNo = model.ArticleColorNo;
            Entity.ItemTypeID = model.ItemTypeID;
            Entity.LeatherTypeID = model.LeatherTypeID;
            Entity.LeatherStatusID = model.LeatherStatusID;
            Entity.ProductionPcs = model.ProductionPcs;
            Entity.ProductionSide = model.ProductionSide;
            Entity.Productionrea = model.ProductionArea;
            Entity.Remarks = model.Remarks;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public PRD_FinishLeatherQCSelection SetToModelObject(PrdCrustLeatherQCSelection model, int userid)
        {
            PRD_FinishLeatherQCSelection Entity = new PRD_FinishLeatherQCSelection();

            Entity.FNQCSelectionID = model.CLQCSelectionID;
            Entity.FNQCItemID = model.CLQCItemID;
            Entity.FinishLeatherQCID = model.CrustLeatherQCID;
            Entity.ScheduleItemID = model.ScheduleItemID;
            Entity.ScheduleProductionNo = model.ScheduleProductionNo;
            Entity.GradeRange = model.GradeRange;
            Entity.GradeID = model.GradeID;
            Entity.ProductionPcs = model.ProductionPcs;
            Entity.ProductionSide = model.ProductionSide;
            Entity.ProductionArea = model.ProductionArea;
            Entity.ProductionAreaUnit = model.ProductionAreaUnit;
            Entity.QCPassPcs = model.QCPassPcs;
            Entity.QCPassSide = model.QCPassSide;
            Entity.QCPassArea = model.QCPassArea;
            if (string.IsNullOrEmpty(model.QCPassAreaUnitName))
                Entity.QCPassAreaUnit = null;
            else
                Entity.QCPassAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == model.QCPassAreaUnitName).FirstOrDefault().UnitID;
            Entity.QCPassRemarks = model.QCPassRemarks;
            Entity.QCFailPcs = model.QCFailPcs;
            Entity.QCFailSide = model.QCFailSide;
            Entity.QCFailArea = model.QCFailArea;
            Entity.QCFailRemarks = model.QCFailRemarks;

            if (string.IsNullOrEmpty(model.QCFailAreaUnitName))
                Entity.QCFailAreaUnit = null;
            else
                Entity.QCFailAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == model.QCFailAreaUnitName).FirstOrDefault().UnitID;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public long GetQCID()
        {
            return CrustLeatherQCID;
        }

        public string GetQCNo()
        {
            return CrustLeatherQCNo;
        }

        public List<PrdCrustLeatherQCItem> GetScheduleColorList(string ProductionFloor, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ColorID, string ArticleChallanID)
        {
            if (!string.IsNullOrEmpty(ProductionFloor))
            {
                if (!string.IsNullOrEmpty(BuyerID))
                {
                    if (string.IsNullOrEmpty(BuyerOrderID))
                    {
                        var query = @"select inv.TransectionID,
                            inv.BuyerID,(select BuyerName from dbo.Sys_Buyer where BuyerID = inv.BuyerID)BuyerName,
                            inv.BuyerOrderID,(select BuyerOrderNo from dbo.SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID)BuyerOrderNo,
                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,
                            inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.ArticleChallanID,inv.ArticleChallanNo,
                            sup.ProductionPcs,sup.ProductionSide,sup.ProductionArea from dbo.PRD_FinishLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,SUM(ClosingProductionPcs) ProductionPcs,SUM(ClosingProductionSide) ProductionSide,SUM(ClosingProductionArea) ProductionArea from dbo.PRD_FinishLeatherProductionStock
                            where QCStatus ='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID) sup
                            ON inv.TransectionID=sup.TransectionID
                            where inv.StoreID = " + ProductionFloor + " and inv.BuyerID = " + BuyerID + " and inv.BuyerOrderID  IS NULL and inv.ItemTypeID = " + ItemTypeID + " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ArticleChallanID = " + ArticleChallanID + " and inv.ColorID = " + ColorID + " and (inv.ClosingProductionPcs>0 or inv.ClosingProductionSide>0 or inv.ClosingProductionArea>0)";
                        var allData = _context.Database.SqlQuery<PrdCrustLeatherQCItem>(query).ToList();
                        return allData;
                    }
                    else
                    {
                        var query = @"select inv.TransectionID,
                            inv.BuyerID,(select BuyerName from dbo.Sys_Buyer where BuyerID = inv.BuyerID)BuyerName,
                            inv.BuyerOrderID,(select BuyerOrderNo from dbo.SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID)BuyerOrderNo,
                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,
                            inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.ArticleChallanID,inv.ArticleChallanNo,
                            sup.ProductionPcs,sup.ProductionSide,sup.ProductionArea from dbo.PRD_FinishLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,SUM(ClosingProductionPcs) ProductionPcs,SUM(ClosingProductionSide) ProductionSide,SUM(ClosingProductionArea) ProductionArea from dbo.PRD_FinishLeatherProductionStock
                            where QCStatus ='PCM' group group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID) sup
                            ON inv.TransectionID=sup.TransectionID
                            where inv.StoreID = " + ProductionFloor + " and inv.BuyerID = " + BuyerID + " and and inv.BuyerOrderID = " + BuyerOrderID + " and inv.ItemTypeID = " + ItemTypeID + " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ArticleChallanID = " + ArticleChallanID + " and inv.ColorID = " + ColorID + " and (inv.ClosingProductionPcs>0 or inv.ClosingProductionSide>0 or inv.ClosingProductionArea>0)";
                        var allData = _context.Database.SqlQuery<PrdCrustLeatherQCItem>(query).ToList();
                        return allData;
                    }
                }
                else
                {
                    var query = @"select inv.TransectionID,
                            inv.BuyerID,(select BuyerName from dbo.Sys_Buyer where BuyerID = inv.BuyerID)BuyerName,
                            inv.BuyerOrderID,(select BuyerOrderNo from dbo.SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID)BuyerOrderNo,
                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,
                            inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.ArticleChallanID,inv.ArticleChallanNo,
                            sup.ProductionPcs,sup.ProductionSide,sup.ProductionArea from dbo.PRD_FinishLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,SUM(ClosingProductionPcs) ProductionPcs,SUM(ClosingProductionSide) ProductionSide,SUM(ClosingProductionArea) ProductionArea from dbo.PRD_FinishLeatherProductionStock
                            where QCStatus ='PCM' group group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID) sup
                            ON inv.TransectionID=sup.TransectionID
                            where inv.StoreID = " + ProductionFloor + " and inv.BuyerID IS NULL and inv.BuyerOrderID  IS NULL and inv.ItemTypeID = " + ItemTypeID + " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ArticleChallanID = " + ArticleChallanID + " and inv.ColorID = " + ColorID + " and (inv.ClosingProductionPcs>0 or inv.ClosingProductionSide>0 or inv.ClosingProductionArea>0)";
                    var allData = _context.Database.SqlQuery<PrdCrustLeatherQCItem>(query).ToList();
                    return allData;
                }
            }
            return null;
        }

        public List<PrdCrustLeatherQCSelection> GetScheduleGradeList(string ProductionFloor, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ColorID, string ArticleChallanID)
        {
            if (!string.IsNullOrEmpty(ProductionFloor))
            {
                if (!string.IsNullOrEmpty(BuyerID))
                {
                    if (string.IsNullOrEmpty(BuyerOrderID))
                    {
                        var query = @"select inv.TransectionID,
                            inv.GradeRange ShowGradeRange,
                            --inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName,
                            inv.ClosingProductionPcs ProductionPcs,inv.ClosingProductionSide ProductionSide,inv.ClosingProductionArea ProductionArea from dbo.PRD_FinishLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID from dbo.PRD_FinishLeatherProductionStock
                            where QCStatus ='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID) sup
                            ON inv.TransectionID=sup.TransectionID
                            where inv.StoreID = " + ProductionFloor + " and inv.BuyerID = " + BuyerID +
                            " and inv.BuyerOrderID IS NULL and inv.ItemTypeID = " + ItemTypeID +
                            " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID +
                            " and inv.ColorID = " + ColorID + " and inv.ArticleChallanID = " + ArticleChallanID + " and (inv.ClosingProductionPcs>0 or inv.ClosingProductionSide>0 or inv.ClosingProductionArea>0)";
                        var allData = _context.Database.SqlQuery<PrdCrustLeatherQCSelection>(query).ToList();
                        List<PrdCrustLeatherQCSelection> searchList = allData.ToList();
                        return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdCrustLeatherQCSelection>();
                    }
                    else
                    {
                        var query = @"select inv.TransectionID,
                            inv.GradeRange ShowGradeRange,
                            --inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName,
                            inv.ClosingProductionPcs ProductionPcs,inv.ClosingProductionSide ProductionSide,inv.ClosingProductionArea ProductionArea from dbo.PRD_FinishLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID from dbo.PRD_FinishLeatherProductionStock
                            where QCStatus ='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID) sup
                            ON inv.TransectionID=sup.TransectionID
                            where inv.StoreID = " + ProductionFloor + " and inv.BuyerID = " + BuyerID +
                            " and inv.BuyerOrderID = " + BuyerOrderID + " and inv.ItemTypeID = " + ItemTypeID +
                            " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID +
                            " and inv.ColorID = " + ColorID + " and inv.ArticleChallanID = " + ArticleChallanID + " and (inv.ClosingProductionPcs>0 or inv.ClosingProductionSide>0 or inv.ClosingProductionArea>0)";
                        var allData = _context.Database.SqlQuery<PrdCrustLeatherQCSelection>(query).ToList();
                        List<PrdCrustLeatherQCSelection> searchList = allData.ToList();
                        return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdCrustLeatherQCSelection>();
                    }
                }
                else
                {
                    var query = @"select inv.TransectionID,
                            inv.GradeRange ShowGradeRange,
                            --inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName,
                            inv.ClosingProductionPcs ProductionPcs,inv.ClosingProductionSide ProductionSide,inv.ClosingProductionArea ProductionArea from dbo.PRD_FinishLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID from dbo.PRD_FinishLeatherProductionStock
                            where QCStatus ='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID) sup
                            ON inv.TransectionID=sup.TransectionID
                            where inv.StoreID = " + ProductionFloor + " and inv.BuyerID IS NULL" +
                            " and inv.BuyerOrderID IS NULL and inv.ItemTypeID = " + ItemTypeID +
                            " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID +
                            " and inv.ColorID = " + ColorID + " and inv.ArticleChallanID = " + ArticleChallanID + " and (inv.ClosingProductionPcs>0 or inv.ClosingProductionSide>0 or inv.ClosingProductionArea>0)";
                    var allData = _context.Database.SqlQuery<PrdCrustLeatherQCSelection>(query).ToList();
                    List<PrdCrustLeatherQCSelection> searchList = allData.ToList();
                    return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdCrustLeatherQCSelection>();
                }
            }
            return null;
        }

        public PrdCrustLeatherQCSelection SetToBussinessObject(PrdCrustLeatherQCSelection Entity)
        {
            PrdCrustLeatherQCSelection Model = new PrdCrustLeatherQCSelection();

            Model.ShowGradeRange = Entity.ShowGradeRange;
            //Model.GradeID = Entity.GradeID;
            //Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            //Model.GradeName = "Press F9";
            Model.ProductionPcs = Entity.ProductionPcs;
            Model.ProductionSide = Entity.ProductionSide;
            Model.ProductionArea = Entity.ProductionArea;
            Model.QCPassAreaUnitName = "SFT";
            Model.QCFailAreaUnitName = "SFT";

            return Model;
        }

        public List<PrdCrustLeatherQCItem> GetQCColorList(string CrustLeatherQCID)
        {
            long? crustLeatherQCID = Convert.ToInt64(CrustLeatherQCID);
            List<PRD_FinishLeatherQCItem> searchList = _context.PRD_FinishLeatherQCItem.Where(m => m.FinishLeatherQCID == crustLeatherQCID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdCrustLeatherQCItem>();
        }

        public PrdCrustLeatherQCItem SetToBussinessObject(PRD_FinishLeatherQCItem Entity)
        {
            PrdCrustLeatherQCItem Model = new PrdCrustLeatherQCItem();

            Model.CLQCItemID = Entity.FNQCItemID;
            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleName = Entity.ArticleID == null ? "" : _context.Sys_Article.Where(m => m.ArticleID == Entity.ArticleID).FirstOrDefault().ArticleName;
            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleChallanID = Entity.ArticleChallanID;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;
            Model.ArticleColorNo = Entity.ArticleColorNo;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.BuyerID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            Model.ProductionPcs = Entity.ProductionPcs;
            Model.ProductionSide = Entity.ProductionSide;
            Model.ProductionArea = Entity.Productionrea;

            return Model;
        }

        public List<PrdCrustLeatherQCSelection> GetQCSelectionList(string CLQCItemID, string StoreId)
        {
            byte storeId = Convert.ToByte(StoreId);
            long? qcItemID = Convert.ToInt64(CLQCItemID);
            List<PRD_FinishLeatherQCSelection> searchList = _context.PRD_FinishLeatherQCSelection.Where(m => m.FNQCItemID == qcItemID).OrderByDescending(m => m.FNQCSelectionID).ToList();
            return searchList.Select(c => SetToBussinessObject(c, storeId)).ToList<PrdCrustLeatherQCSelection>();

        }

        public PrdCrustLeatherQCSelection SetToBussinessObject(PRD_FinishLeatherQCSelection Entity, byte storeId)
        {
            PrdCrustLeatherQCSelection Model = new PrdCrustLeatherQCSelection();

            Model.CLQCSelectionID = Entity.FNQCSelectionID;
            Model.GradeRange = Entity.GradeRange;
            //Model.GradeID = Entity.GradeID;
            //Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            //Model.ProductionPcs = Entity.ProductionPcs;
            //Model.ProductionSide = Entity.ProductionSide;
            //Model.ProductionArea = Entity.ProductionArea;
            //Model.ProductionAreaUnit = Entity.ProductionAreaUnit;
            //Model.ProductionAreaUnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;
            Model.QCPassPcs = Entity.QCPassPcs;
            Model.QCPassSide = Entity.QCPassSide;
            Model.QCPassArea = Entity.QCPassArea;
            Model.QCPassAreaUnit = Entity.QCPassAreaUnit;
            Model.QCPassAreaUnitName = Entity.QCPassAreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.QCPassAreaUnit).FirstOrDefault().UnitName;
            Model.QCPassRemarks = Entity.QCPassRemarks;
            Model.QCFailPcs = Entity.QCFailPcs;
            Model.QCFailSide = Entity.QCFailSide;
            Model.QCFailArea = Entity.QCFailArea;
            Model.QCFailAreaUnit = Entity.QCFailAreaUnit;
            Model.QCFailAreaUnitName = Entity.QCFailAreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.QCFailAreaUnit).FirstOrDefault().UnitName;
            Model.QCFailRemarks = Entity.QCFailRemarks;

            var BuyerID = _context.PRD_FinishLeatherQCItem.Where(m => m.FNQCItemID == Entity.FNQCItemID).FirstOrDefault().BuyerID;
            var BuyerOrderID = _context.PRD_FinishLeatherQCItem.Where((m => m.FNQCItemID == Entity.FNQCItemID)).FirstOrDefault().BuyerOrderID;
            var ItemTypeID = _context.PRD_FinishLeatherQCItem.Where((m => m.FNQCItemID == Entity.FNQCItemID)).FirstOrDefault().ItemTypeID;
            var LeatherTypeID = _context.PRD_FinishLeatherQCItem.Where((m => m.FNQCItemID == Entity.FNQCItemID)).FirstOrDefault().LeatherTypeID;
            var LeatherStatusID = _context.PRD_FinishLeatherQCItem.Where((m => m.FNQCItemID == Entity.FNQCItemID)).FirstOrDefault().LeatherStatusID;
            var ArticleID = _context.PRD_FinishLeatherQCItem.Where((m => m.FNQCItemID == Entity.FNQCItemID)).FirstOrDefault().ArticleID;
            var ArticleChallanID = _context.PRD_FinishLeatherQCItem.Where((m => m.FNQCItemID == Entity.FNQCItemID)).FirstOrDefault().ArticleChallanID;
            var ColorID = _context.PRD_FinishLeatherQCItem.Where((m => m.FNQCItemID == Entity.FNQCItemID)).FirstOrDefault().ColorID;

            Model.ShowGradeRange = _context.PRD_FinishLeatherProductionStock.
               Where(ma => ma.StoreID == storeId
                   && ma.BuyerID == BuyerID
                   && ma.BuyerOrderID == BuyerOrderID
                   && ma.ItemTypeID == ItemTypeID
                   && ma.LeatherStatusID == LeatherStatusID
                   && ma.ArticleID == ArticleID
                   && ma.ArticleChallanID == ArticleChallanID
                   && ma.ColorID == ColorID
                //&& ma.GradeID == Entity.GradeID
                   ).OrderByDescending(m => m.TransectionID).FirstOrDefault().GradeRange;

            Model.ProductionPcs = _context.PRD_FinishLeatherProductionStock.
                Where(ma => ma.StoreID == storeId
                    && ma.BuyerID == BuyerID
                    && ma.BuyerOrderID == BuyerOrderID
                    && ma.ItemTypeID == ItemTypeID
                    && ma.LeatherStatusID == LeatherStatusID
                    && ma.ArticleID == ArticleID
                    && ma.ArticleChallanID == ArticleChallanID
                    && ma.ColorID == ColorID
                //&& ma.GradeID == Entity.GradeID
                    ).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingProductionPcs;
            Model.ProductionSide = _context.PRD_FinishLeatherProductionStock.
                Where(ma => ma.StoreID == storeId
                    && ma.BuyerID == BuyerID
                    && ma.BuyerOrderID == BuyerOrderID
                    && ma.ItemTypeID == ItemTypeID
                    && ma.LeatherStatusID == LeatherStatusID
                    && ma.ArticleID == ArticleID
                    && ma.ArticleChallanID == ArticleChallanID
                    && ma.ColorID == ColorID
                //&& ma.GradeID == Entity.GradeID
                    ).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingProductionSide;
            Model.ProductionArea = _context.PRD_FinishLeatherProductionStock.
                Where(ma => ma.StoreID == storeId
                    && ma.BuyerID == BuyerID
                    && ma.BuyerOrderID == BuyerOrderID
                    && ma.ItemTypeID == ItemTypeID
                    && ma.LeatherStatusID == LeatherStatusID
                    && ma.ArticleID == ArticleID
                    && ma.ArticleChallanID == ArticleChallanID
                    && ma.ColorID == ColorID
                //&& ma.GradeID == Entity.GradeID
                    ).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingProductionArea;

            return Model;
        }

        public List<PrdCrustLeatherQC> GetCrustLeatherQCInfo()
        {
            List<PRD_FinishLeatherQC> searchList = _context.PRD_FinishLeatherQC.Where(m => m.QCTransactionOf == "FOQC").OrderByDescending(m => m.FinishLeatherQCID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdCrustLeatherQC>();
        }

        public PrdCrustLeatherQC SetToBussinessObject(PRD_FinishLeatherQC Entity)
        {
            PrdCrustLeatherQC Model = new PrdCrustLeatherQC();

            Model.CrustLeatherQCID = Entity.FinishLeatherQCID;
            Model.CrustLeatherQCNo = Entity.FinishLeatherQCNo;
            Model.CrustLeatherQCDate = string.IsNullOrEmpty(Entity.FinishLeatherQCDate.ToString()) ? string.Empty : Convert.ToDateTime(Entity.FinishLeatherQCDate).ToString("dd/MM/yyyy");
            Model.QCTransactionOf = Entity.QCTransactionOf;
            if (Entity.QCTransactionOf == "FOQC")
            {
                Model.QCTransactionOfName = "Own QC";
                Entity.QCTransactionOf = "Own QC";
            }
            //else
            //    Model.QCTransactionOfName = "Buyer QC";
            Model.ProductionFloor = Entity.ProductionFloor;
            Model.ProductionFloorName = Entity.ProductionFloor == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.ProductionFloor).FirstOrDefault().StoreName;
            Model.AfterQCIssueFloor = Entity.AfterQCIssueFloor;
            Model.AfterQCIssueFloorName = Entity.AfterQCIssueFloor == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.AfterQCIssueFloor).FirstOrDefault().StoreName;

            Model.AfterQCIssueStore = Entity.AfterQCIssueStore;
            Model.AfterQCIssueStoreName = Entity.AfterQCIssueStore == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.AfterQCIssueStore).FirstOrDefault().StoreName;

            Model.RecordStatus = Entity.RecordStatus;

            return Model;
        }

        public List<PrdCrustLeatherQCItem> GetScheduleItemInfo(string ProductionFloor)
        {
            if (!string.IsNullOrEmpty(ProductionFloor))
            {
                var query = @"select inv.TransectionID,
                            inv.BuyerID,(select BuyerName from dbo.Sys_Buyer where BuyerID = inv.BuyerID)BuyerName,
                            inv.BuyerOrderID,(select BuyerOrderNo from dbo.SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID)BuyerOrderNo,
                            --inv.StoreID,(select StoreName from dbo.SYS_Store where StoreID = inv.StoreID)StoreName,
                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
                            inv.LeatherTypeID,(select LeatherTypeName from dbo.Sys_LeatherType where LeatherTypeID = inv.LeatherTypeID)LeatherTypeName,
                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,
                            inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.ArticleChallanID,inv.ArticleChallanNo,
                            inv.ClosingProductionPcs from dbo.PRD_FinishLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID from dbo.PRD_FinishLeatherProductionStock
                            where QCStatus ='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID) sup
                            ON inv.TransectionID=sup.TransectionID
                            where inv.StoreID = " + ProductionFloor + " and (inv.ClosingProductionPcs>0 or inv.ClosingProductionSide>0 or inv.ClosingProductionArea>0)";
                var allData = _context.Database.SqlQuery<PrdCrustLeatherQCItem>(query).ToList();
                return allData;
            }
            return null;
        }

        public ValidationMsg ConfirmedCrustQC(PrdCrustLeatherQC model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityYearMonth = _context.PRD_FinishLeatherQC.First(m => m.FinishLeatherQCID == model.CrustLeatherQCID);
                        originalEntityYearMonth.RecordStatus = "CNF";
                        originalEntityYearMonth.ModifiedBy = userid;
                        originalEntityYearMonth.ModifiedOn = DateTime.Now;

                        #region Update Store Records

                        if (model.PrdCrustLeatherQCItemList != null)
                        {
                            foreach (PrdCrustLeatherQCItem objQCItem in model.PrdCrustLeatherQCItemList)
                            {
                                if (model.PrdCrustLeatherQCSelectionList != null)
                                {
                                    foreach (PrdCrustLeatherQCSelection objQCSelection in model.PrdCrustLeatherQCSelectionList)
                                    {
                                        #region Issue QC Store From Crust Production

                                        var CrustProductionStock = (from ds in _context.PRD_FinishLeatherProductionStock.AsEnumerable()
                                                                    where ds.StoreID == model.ProductionFloor
                                                                    && ds.BuyerID == objQCItem.BuyerID
                                                                    && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                    && ds.ArticleID == objQCItem.ArticleID
                                                                    && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                    && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                    && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                    && ds.ColorID == objQCItem.ColorID
                                                                        //&& ds.GradeID == objQCSelection.GradeID
                                                                    && ds.QCStatus == "PCM"
                                                                    select ds).Any();

                                        if (CrustProductionStock)
                                        {
                                            var LastItemInfo = (from ds in _context.PRD_FinishLeatherProductionStock
                                                                where ds.StoreID == model.ProductionFloor
                                                                && ds.BuyerID == objQCItem.BuyerID
                                                                && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                && ds.ArticleID == objQCItem.ArticleID
                                                                && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                && ds.ColorID == objQCItem.ColorID
                                                                    //&& ds.GradeID == objQCSelection.GradeID
                                                                && ds.QCStatus == "PCM"
                                                                orderby ds.TransectionID descending
                                                                select ds).FirstOrDefault();

                                            decimal? QCPcsQty = (objQCSelection.QCPassPcs == null ? 0 : objQCSelection.QCPassPcs) + (objQCSelection.QCFailPcs == null ? 0 : objQCSelection.QCFailPcs);
                                            decimal? QCSideQty = (objQCSelection.QCPassSide == null ? 0 : objQCSelection.QCPassSide) + (objQCSelection.QCFailSide == null ? 0 : objQCSelection.QCFailSide);
                                            decimal? QCAreaQty = (objQCSelection.QCPassArea == null ? 0 : objQCSelection.QCPassArea) + (objQCSelection.QCFailArea == null ? 0 : objQCSelection.QCFailArea);

                                            if ((LastItemInfo.ClosingProductionPcs >= QCPcsQty) && (LastItemInfo.ClosingProductionSide >= QCSideQty) && (LastItemInfo.ClosingProductionArea >= QCAreaQty))
                                            {
                                                #region Production Completed

                                                var objPCMStockItem = new PRD_FinishLeatherProductionStock();

                                                if (objQCItem.BuyerID == null)
                                                    objPCMStockItem.BuyerID = null;
                                                else
                                                    objPCMStockItem.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                if (objQCItem.BuyerOrderID == null)
                                                    objPCMStockItem.BuyerOrderID = null;
                                                else
                                                    objPCMStockItem.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                objPCMStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                objPCMStockItem.StoreID = Convert.ToByte(model.ProductionFloor);
                                                objPCMStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objPCMStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objPCMStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objPCMStockItem.ColorID = Convert.ToInt32(objQCItem.ColorID);

                                                objPCMStockItem.ArticleColorNo = Convert.ToInt32(objQCItem.ArticleColorNo);
                                                objPCMStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                objPCMStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                objPCMStockItem.GradeRange = objQCSelection.GradeRange;

                                                if ((string.IsNullOrEmpty(objQCSelection.QCPassAreaUnitName)) && (string.IsNullOrEmpty(objQCSelection.QCFailAreaUnitName)))
                                                    objPCMStockItem.AreaUnit = null;
                                                else
                                                    objPCMStockItem.AreaUnit = !(string.IsNullOrEmpty(objQCSelection.QCPassAreaUnitName)) ? _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCPassAreaUnitName).FirstOrDefault().UnitID : _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCFailAreaUnitName).FirstOrDefault().UnitID;

                                                objPCMStockItem.QCStatus = "PCM";

                                                objPCMStockItem.OpeningPcs = LastItemInfo.ClosingProductionPcs;
                                                objPCMStockItem.ReceivePcs = 0;
                                                objPCMStockItem.IssuePcs = (objQCSelection.QCPassPcs == null ? 0 : objQCSelection.QCPassPcs) + (objQCSelection.QCFailPcs == null ? 0 : objQCSelection.QCFailPcs);
                                                objPCMStockItem.ClosingProductionPcs = LastItemInfo.ClosingProductionPcs - ((objQCSelection.QCPassPcs == null ? 0 : objQCSelection.QCPassPcs) + (objQCSelection.QCFailPcs == null ? 0 : objQCSelection.QCFailPcs));

                                                objPCMStockItem.OpeningSide = LastItemInfo.ClosingProductionSide;
                                                objPCMStockItem.ReceiveSide = 0;
                                                objPCMStockItem.IssueSide = (objQCSelection.QCPassSide == null ? 0 : objQCSelection.QCPassSide) + (objQCSelection.QCFailSide == null ? 0 : objQCSelection.QCFailSide);
                                                objPCMStockItem.ClosingProductionSide = LastItemInfo.ClosingProductionSide - ((objQCSelection.QCPassSide == null ? 0 : objQCSelection.QCPassSide) + (objQCSelection.QCFailSide == null ? 0 : objQCSelection.QCFailSide));

                                                objPCMStockItem.OpeningArea = LastItemInfo.ClosingProductionArea;
                                                objPCMStockItem.ReceiveArea = 0;
                                                objPCMStockItem.IssueArea = (objQCSelection.QCPassArea == null ? 0 : objQCSelection.QCPassArea) + (objQCSelection.QCFailArea == null ? 0 : objQCSelection.QCFailArea);
                                                objPCMStockItem.ClosingProductionArea = LastItemInfo.ClosingProductionArea - ((objQCSelection.QCPassArea == null ? 0 : objQCSelection.QCPassArea) + (objQCSelection.QCFailArea == null ? 0 : objQCSelection.QCFailArea));

                                                _context.PRD_FinishLeatherProductionStock.Add(objPCMStockItem);
                                                _context.SaveChanges();

                                                #endregion
                                            }
                                        }

                                        #endregion

                                        if ((objQCSelection.QCPassPcs != null) || (objQCSelection.QCPassSide != null))
                                        {
                                            #region Receive QC PASS Stock

                                            var CheckItemStock = (from i in _context.INV_FinishBuyerQCStock.AsEnumerable()
                                                                  where i.StoreID == model.AfterQCIssueFloor
                                                                        && i.BuyerID == objQCItem.BuyerID
                                                                        && i.BuyerOrderID == objQCItem.BuyerOrderID
                                                                        && i.ArticleID == objQCItem.ArticleID
                                                                        && i.ArticleChallanID == objQCItem.ArticleChallanID
                                                                        && i.ItemTypeID == objQCItem.ItemTypeID
                                                                        && i.LeatherStatusID == objQCItem.LeatherStatusID
                                                                        && i.ColorID == objQCItem.ColorID
                                                                        && i.FinishQCLabel == "PASS"
                                                                  select i).Any();
                                            if (!CheckItemStock)
                                            {
                                                INV_FinishBuyerQCStock objStockItem = new INV_FinishBuyerQCStock();

                                                objStockItem.StoreID = Convert.ToByte(model.AfterQCIssueFloor);

                                                if (objQCItem.BuyerID == null)
                                                    objStockItem.BuyerID = null;
                                                else
                                                    objStockItem.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                if (objQCItem.BuyerOrderID == null)
                                                    objStockItem.BuyerOrderID = null;
                                                else
                                                    objStockItem.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);

                                                objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                objStockItem.ColorID = Convert.ToInt32(objQCItem.ColorID);

                                                objStockItem.ArticleColorNo = Convert.ToInt32(objQCItem.ArticleColorNo);
                                                objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                objStockItem.GradeRange = objQCSelection.GradeRange;

                                                if (string.IsNullOrEmpty(objQCSelection.QCPassAreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCPassAreaUnitName).FirstOrDefault().UnitID;

                                                objStockItem.FinishQCLabel = "PASS";

                                                objStockItem.OpeningStockPcs = 0;
                                                objStockItem.ReceiveStockPcs = objQCSelection.QCPassPcs == null ? 0 : objQCSelection.QCPassPcs;
                                                objStockItem.IssueStockPcs = 0;
                                                objStockItem.ClosingStockPcs = objQCSelection.QCPassPcs == null ? 0 : objQCSelection.QCPassPcs;

                                                objStockItem.OpeningStockSide = 0;
                                                objStockItem.ReceiveStockSide = objQCSelection.QCPassSide == null ? 0 : objQCSelection.QCPassSide;
                                                objStockItem.IssueStockSide = 0;
                                                objStockItem.ClosingStockSide = objQCSelection.QCPassSide == null ? 0 : objQCSelection.QCPassSide;

                                                objStockItem.OpeningStockArea = 0;
                                                objStockItem.ReceiveStockArea = objQCSelection.QCPassArea == null ? 0 : objQCSelection.QCPassArea;
                                                objStockItem.IssueStockArea = 0;
                                                objStockItem.ClosingStockArea = objQCSelection.QCPassArea == null ? 0 : objQCSelection.QCPassArea;

                                                _context.INV_FinishBuyerQCStock.Add(objStockItem);
                                                _context.SaveChanges();
                                            }
                                            else
                                            {
                                                var LastItemInfo = (from i in _context.INV_FinishBuyerQCStock.AsEnumerable()
                                                                    where i.StoreID == model.AfterQCIssueFloor
                                                                          && i.BuyerID == objQCItem.BuyerID
                                                                          && i.BuyerOrderID == objQCItem.BuyerOrderID
                                                                          && i.ArticleID == objQCItem.ArticleID
                                                                          && i.ArticleChallanID == objQCItem.ArticleChallanID
                                                                          && i.ItemTypeID == objQCItem.ItemTypeID
                                                                          && i.LeatherStatusID == objQCItem.LeatherStatusID
                                                                          && i.ColorID == objQCItem.ColorID
                                                                          && i.FinishQCLabel == "PASS"
                                                                    orderby i.TransectionID descending
                                                                    select i).FirstOrDefault();

                                                INV_FinishBuyerQCStock objStockItem = new INV_FinishBuyerQCStock();

                                                objStockItem.StoreID = Convert.ToByte(model.AfterQCIssueFloor);

                                                if (objQCItem.BuyerID == null)
                                                    objStockItem.BuyerID = null;
                                                else
                                                    objStockItem.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                if (objQCItem.BuyerOrderID == null)
                                                    objStockItem.BuyerOrderID = null;
                                                else
                                                    objStockItem.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);

                                                objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockItem.ColorID = Convert.ToInt32(objQCItem.ColorID);

                                                objStockItem.ArticleColorNo = Convert.ToInt32(objQCItem.ArticleColorNo);
                                                objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                objStockItem.GradeRange = objQCSelection.GradeRange;

                                                if (string.IsNullOrEmpty(objQCSelection.QCPassAreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCPassAreaUnitName).FirstOrDefault().UnitID;

                                                objStockItem.FinishQCLabel = "PASS";

                                                objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
                                                objStockItem.ReceiveStockPcs = objQCSelection.QCPassPcs == null ? 0 : objQCSelection.QCPassPcs;
                                                objStockItem.IssueStockPcs = 0;
                                                objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs + objQCSelection.QCPassPcs == null ? 0 : objQCSelection.QCPassPcs;

                                                objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
                                                objStockItem.ReceiveStockSide = objQCSelection.QCPassSide == null ? 0 : objQCSelection.QCPassSide;
                                                objStockItem.IssueStockSide = 0;
                                                objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide + objQCSelection.QCPassSide == null ? 0 : objQCSelection.QCPassSide;

                                                objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea;
                                                objStockItem.ReceiveStockArea = objQCSelection.QCPassArea == null ? 0 : objQCSelection.QCPassArea;
                                                objStockItem.IssueStockArea = 0;
                                                objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea + objQCSelection.QCPassArea == null ? 0 : objQCSelection.QCPassArea;

                                                _context.INV_FinishBuyerQCStock.Add(objStockItem);
                                                _context.SaveChanges();
                                            }

                                            #endregion
                                        }
                                        if ((objQCSelection.QCFailPcs != null) || (objQCSelection.QCFailSide != null))
                                        {
                                            var currentDate = DateTime.Now.Date;

                                            #region Receive QC FAIL Stock

                                            #region Daily Stock

                                            var CheckDate2 = (from ds in _context.INV_FinishStockDaily.AsEnumerable()
                                                              where ds.StockDate == currentDate
                                                                    && ds.StoreID == model.AfterQCIssueStore
                                                                    && ds.ArticleID == objQCItem.ArticleID
                                                                    && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                    && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                    && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                    && ds.ColorID == objQCItem.ColorID
                                                              //&& ds.GradeID == objQCSelection.GradeID
                                                              select ds).Any();

                                            if (CheckDate2)
                                            {
                                                var CurrentItem = (from ds in _context.INV_FinishStockDaily.AsEnumerable()
                                                                   where ds.StockDate == currentDate
                                                                         && ds.StoreID == model.AfterQCIssueStore
                                                                         && ds.ArticleID == objQCItem.ArticleID
                                                                         && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                         && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                         && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                         && ds.ColorID == objQCItem.ColorID
                                                                   //&& ds.GradeID == objQCSelection.GradeID
                                                                   select ds).FirstOrDefault();

                                                CurrentItem.IssueStockPcs = CurrentItem.ReceiveStockPcs + objQCSelection.QCFailPcs ?? 0;
                                                CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockPcs + objQCSelection.QCFailPcs ?? 0;

                                                CurrentItem.IssueStockSide = CurrentItem.ReceiveStockSide + objQCSelection.QCFailSide ?? 0;
                                                CurrentItem.ClosingStockSide = CurrentItem.ClosingStockSide + objQCSelection.QCFailSide ?? 0;

                                                CurrentItem.IssueStockArea = CurrentItem.ReceiveStockArea + objQCSelection.QCFailArea ?? 0;
                                                CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockArea + objQCSelection.QCFailArea ?? 0;
                                                CurrentItem.FinishQCLabel = "OQF";//Own QC Fail
                                                _context.SaveChanges();
                                            }
                                            else
                                            {
                                                var PreviousRecord = (from ds in _context.INV_FinishStockDaily.AsEnumerable()
                                                                      where ds.StoreID == model.AfterQCIssueStore
                                                                            && ds.ArticleID == objQCItem.ArticleID
                                                                            && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                            && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                            && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                            && ds.ColorID == objQCItem.ColorID
                                                                      //&& ds.GradeID == objQCSelection.GradeID
                                                                      orderby ds.StockDate
                                                                      select ds).LastOrDefault();

                                                var objStockDaily = new INV_FinishStockDaily();

                                                objStockDaily.StockDate = currentDate;

                                                objStockDaily.StoreID = Convert.ToByte(model.AfterQCIssueStore);
                                                objStockDaily.ItemTypeID = objQCItem.ItemTypeID;
                                                objStockDaily.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                objStockDaily.LeatherStatusID = objQCItem.LeatherStatusID;
                                                objStockDaily.ArticleID = objQCItem.ArticleID;
                                                objStockDaily.ColorID = objQCItem.ColorID;

                                                objStockDaily.ArticleColorNo = Convert.ToInt32(objQCItem.ArticleColorNo);
                                                objStockDaily.ArticleChallanID = objQCItem.ArticleChallanID;
                                                objStockDaily.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                objStockDaily.GradeRange = objQCSelection.GradeRange;

                                                if (string.IsNullOrEmpty(objQCSelection.QCFailAreaUnitName))
                                                    objStockDaily.ClosingStockAreaUnit = null;
                                                else
                                                    objStockDaily.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCFailAreaUnitName).FirstOrDefault().UnitID;
                                                objStockDaily.OpeningStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs);
                                                objStockDaily.ReceiveStockPcs = objQCSelection.QCFailPcs ?? 0;
                                                objStockDaily.IssueStockPcs = 0;
                                                objStockDaily.ClosingStockPcs = objStockDaily.OpeningStockPcs + objQCSelection.QCFailPcs ?? 0;

                                                objStockDaily.OpeningStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide);
                                                objStockDaily.ReceiveStockSide = objQCSelection.QCFailSide ?? 0;
                                                objStockDaily.IssueStockSide = 0;
                                                objStockDaily.ClosingStockSide = objStockDaily.OpeningStockSide + objQCSelection.QCFailSide ?? 0;

                                                objStockDaily.OpeningStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea);
                                                objStockDaily.ReceiveStockArea = objQCSelection.QCFailArea ?? 0;
                                                objStockDaily.IssueStockArea = 0;
                                                objStockDaily.ClosingStockArea = objStockDaily.OpeningStockArea + objQCSelection.QCFailArea ?? 0;

                                                objStockDaily.FinishQCLabel = "OQF";//Own QC Fail
                                                _context.INV_FinishStockDaily.Add(objStockDaily);
                                            }

                                            #endregion

                                            #region Buyer Stock

                                            var CheckBuyerStock2 = (from ds in _context.INV_FinishBuyerStock.AsEnumerable()
                                                                    where ds.BuyerID == objQCItem.BuyerID
                                                                          && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                          && ds.StoreID == model.AfterQCIssueStore
                                                                          && ds.ArticleID == objQCItem.ArticleID
                                                                          && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                          && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                          && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                          && ds.ColorID == objQCItem.ColorID
                                                                    //&& ds.GradeID == objQCSelection.GradeID
                                                                    select ds).Any();

                                            if (!CheckBuyerStock2)
                                            {
                                                var PreviousRecord = (from ds in _context.INV_FinishBuyerStock.AsEnumerable()
                                                                      where ds.BuyerID == objQCItem.BuyerID
                                                                            && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                            && ds.StoreID == model.AfterQCIssueStore
                                                                            && ds.ArticleID == objQCItem.ArticleID
                                                                            && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                            && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                            && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                            && ds.ColorID == objQCItem.ColorID
                                                                      //&& ds.GradeID == objQCSelection.GradeID
                                                                      select ds).LastOrDefault();

                                                var objStockSupplier = new INV_FinishBuyerStock();

                                                if (objQCItem.BuyerID == null)
                                                    objStockSupplier.BuyerID = null;
                                                else
                                                    objStockSupplier.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                if (objQCItem.BuyerOrderID == null)
                                                    objStockSupplier.BuyerOrderID = null;
                                                else
                                                    objStockSupplier.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);

                                                //objStockSupplier.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                //objStockSupplier.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                objStockSupplier.StoreID = Convert.ToByte(model.AfterQCIssueStore);
                                                objStockSupplier.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockSupplier.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                objStockSupplier.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockSupplier.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockSupplier.ColorID = Convert.ToInt32(objQCItem.ColorID);

                                                objStockSupplier.ArticleColorNo = Convert.ToInt32(objQCItem.ArticleColorNo);
                                                objStockSupplier.ArticleChallanID = objQCItem.ArticleChallanID;
                                                objStockSupplier.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                objStockSupplier.GradeRange = objQCSelection.GradeRange;

                                                //objStockSupplier.GradeID = objQCSelection.GradeID;

                                                if (string.IsNullOrEmpty(objQCSelection.QCFailAreaUnitName))
                                                    objStockSupplier.ClosingStockAreaUnit = null;
                                                else
                                                    objStockSupplier.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCFailAreaUnitName).FirstOrDefault().UnitID;
                                                //objStockSupplier.FinishQCLabel = objQCSelection.CrustQCLabel;

                                                objStockSupplier.OpeningStockPcs = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs;
                                                objStockSupplier.ReceiveStockPcs = objQCSelection.QCFailPcs ?? 0;
                                                objStockSupplier.IssueStockPcs = 0;
                                                objStockSupplier.ClosingStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs) + objQCSelection.QCFailPcs ?? 0;
                                                objStockSupplier.OpeningStockSide = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide;
                                                objStockSupplier.ReceiveStockSide = objQCSelection.QCFailSide ?? 0;
                                                objStockSupplier.IssueStockSide = 0;
                                                objStockSupplier.ClosingStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide) + objQCSelection.QCFailSide ?? 0;

                                                objStockSupplier.OpeningStockArea = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea;
                                                objStockSupplier.ReceiveStockArea = objQCSelection.QCFailArea ?? 0;
                                                objStockSupplier.IssueStockArea = 0;
                                                objStockSupplier.ClosingStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea) + objQCSelection.QCFailArea ?? 0;

                                                objStockSupplier.FinishQCLabel = "OQF";//Own QC Fail
                                                _context.INV_FinishBuyerStock.Add(objStockSupplier);
                                            }
                                            else
                                            {
                                                var LastBuyerStock = (from ds in _context.INV_FinishBuyerStock.AsEnumerable()
                                                                      where ds.BuyerID == objQCItem.BuyerID
                                                                            && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                            && ds.StoreID == model.AfterQCIssueStore
                                                                            && ds.ArticleID == objQCItem.ArticleID
                                                                            && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                            && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                            && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                            && ds.ColorID == objQCItem.ColorID
                                                                      //&& ds.GradeID == objQCSelection.GradeID
                                                                      orderby ds.TransectionID descending
                                                                      select ds).FirstOrDefault();


                                                var objStockSupplier = new INV_FinishBuyerStock();

                                                if (objQCItem.BuyerID == null)
                                                    objStockSupplier.BuyerID = null;
                                                else
                                                    objStockSupplier.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                if (objQCItem.BuyerOrderID == null)
                                                    objStockSupplier.BuyerOrderID = null;
                                                else
                                                    objStockSupplier.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                //objStockSupplier.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                //objStockSupplier.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                objStockSupplier.StoreID = Convert.ToByte(model.AfterQCIssueStore);
                                                objStockSupplier.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockSupplier.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                objStockSupplier.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockSupplier.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockSupplier.ColorID = Convert.ToInt32(objQCItem.ColorID);


                                                objStockSupplier.ArticleColorNo = Convert.ToInt32(objQCItem.ArticleColorNo);
                                                objStockSupplier.ArticleChallanID = objQCItem.ArticleChallanID;
                                                objStockSupplier.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                objStockSupplier.GradeRange = objQCSelection.GradeRange;

                                                if (string.IsNullOrEmpty(objQCSelection.QCFailAreaUnitName))
                                                    objStockSupplier.ClosingStockAreaUnit = null;
                                                else
                                                    objStockSupplier.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCFailAreaUnitName).FirstOrDefault().UnitID;

                                                //objStockSupplier.FinishQCLabel = objQCSelection.CrustQCLabel;

                                                objStockSupplier.OpeningStockPcs = LastBuyerStock.ClosingStockPcs;
                                                objStockSupplier.ReceiveStockPcs = objQCSelection.QCFailPcs ?? 0;
                                                objStockSupplier.IssueStockPcs = 0;
                                                objStockSupplier.ClosingStockPcs = LastBuyerStock.ClosingStockPcs + objQCSelection.QCFailPcs ?? 0;

                                                objStockSupplier.OpeningStockSide = LastBuyerStock.ClosingStockSide;
                                                objStockSupplier.ReceiveStockSide = objQCSelection.QCFailSide ?? 0;
                                                objStockSupplier.IssueStockSide = 0;
                                                objStockSupplier.ClosingStockSide = LastBuyerStock.ClosingStockSide + objQCSelection.QCFailSide ?? 0;

                                                objStockSupplier.OpeningStockArea = LastBuyerStock.ClosingStockArea;
                                                objStockSupplier.ReceiveStockArea = objQCSelection.QCFailArea ?? 0;
                                                objStockSupplier.IssueStockArea = 0;
                                                objStockSupplier.ClosingStockArea = LastBuyerStock.ClosingStockArea + objQCSelection.QCFailArea ?? 0;

                                                objStockSupplier.FinishQCLabel = "OQF";//Own QC Fail
                                                _context.INV_FinishBuyerStock.Add(objStockSupplier);
                                            }

                                            #endregion

                                            #region Item Stock

                                            var CheckItemStock2 = (from ds in _context.INV_FinishStockItem.AsEnumerable()
                                                                   where ds.StoreID == model.AfterQCIssueStore
                                                                         && ds.ArticleID == objQCItem.ArticleID
                                                                         && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                         && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                         && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                         && ds.ColorID == objQCItem.ColorID
                                                                         && ds.GradeID == objQCSelection.GradeID
                                                                   select ds).Any();

                                            if (!CheckItemStock2)
                                            {
                                                var PreviousRecord = (from ds in _context.INV_FinishStockItem.AsEnumerable()
                                                                      where ds.StoreID == model.AfterQCIssueStore
                                                                            && ds.ArticleID == objQCItem.ArticleID
                                                                            && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                            && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                            && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                            && ds.ColorID == objQCItem.ColorID
                                                                            && ds.GradeID == objQCSelection.GradeID
                                                                      select ds).LastOrDefault();

                                                var objStockItem = new INV_FinishStockItem();

                                                objStockItem.StoreID = Convert.ToByte(model.AfterQCIssueStore);
                                                objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockItem.ColorID = Convert.ToInt32(objQCItem.ColorID);

                                                objStockItem.ArticleColorNo = Convert.ToInt32(objQCItem.ArticleColorNo);
                                                objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                objStockItem.GradeRange = objQCSelection.GradeRange;

                                                //objStockItem.GradeID = objQCSelection.GradeID;

                                                if (string.IsNullOrEmpty(objQCSelection.QCFailAreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCFailAreaUnitName).FirstOrDefault().UnitID;

                                                //objStockItem.FinishQCLabel = objQCSelection.CrustQCLabel;

                                                objStockItem.OpeningStockPcs = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs;
                                                objStockItem.ReceiveStockPcs = objQCSelection.QCFailPcs ?? 0;
                                                objStockItem.IssueStockPcs = 0;
                                                objStockItem.ClosingStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs) + objQCSelection.QCFailPcs;

                                                objStockItem.OpeningStockSide = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide;
                                                objStockItem.ReceiveStockSide = objQCSelection.QCFailSide ?? 0;
                                                objStockItem.IssueStockSide = 0;
                                                objStockItem.ClosingStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide) + objQCSelection.QCFailSide ?? 0;

                                                objStockItem.OpeningStockArea = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea;
                                                objStockItem.ReceiveStockArea = objQCSelection.QCFailArea ?? 0;
                                                objStockItem.IssueStockArea = 0;
                                                objStockItem.ClosingStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea) + objQCSelection.QCFailArea ?? 0;

                                                objStockItem.FinishQCLabel = "OQF";//Own QC Fail
                                                _context.INV_FinishStockItem.Add(objStockItem);
                                            }
                                            else
                                            {
                                                var LastItemInfo = (from ds in _context.INV_FinishStockItem
                                                                    where ds.StoreID == model.AfterQCIssueStore
                                                                          && ds.ArticleID == objQCItem.ArticleID
                                                                          && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                          && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                          && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                          && ds.ColorID == objQCItem.ColorID
                                                                    //&& ds.GradeID == objQCSelection.GradeID
                                                                    orderby ds.TransectionID descending
                                                                    select ds).FirstOrDefault();

                                                var objStockItem = new INV_FinishStockItem();

                                                objStockItem.StoreID = Convert.ToByte(model.AfterQCIssueStore);
                                                objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockItem.ColorID = Convert.ToInt32(objQCItem.ColorID);

                                                objStockItem.ArticleColorNo = Convert.ToInt32(objQCItem.ArticleColorNo);
                                                objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                objStockItem.GradeRange = objQCSelection.GradeRange;

                                                if (string.IsNullOrEmpty(objQCSelection.QCFailAreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCFailAreaUnitName).FirstOrDefault().UnitID;

                                                //objStockItem.FinishQCLabel = objQCSelection.CrustQCLabel;

                                                objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
                                                objStockItem.ReceiveStockPcs = objQCSelection.QCFailPcs ?? 0;
                                                objStockItem.IssueStockPcs = 0;
                                                objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs + objQCSelection.QCFailPcs ?? 0;

                                                objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
                                                objStockItem.ReceiveStockSide = objQCSelection.QCFailSide;
                                                objStockItem.IssueStockSide = 0;
                                                objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide + objQCSelection.QCFailSide ?? 0;

                                                objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea;
                                                objStockItem.ReceiveStockArea = objQCSelection.QCFailArea ?? 0;
                                                objStockItem.IssueStockArea = 0;
                                                objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea + objQCSelection.QCFailArea ?? 0;

                                                objStockItem.FinishQCLabel = "OQF";//Own QC Fail
                                                _context.INV_FinishStockItem.Add(objStockItem);
                                            }

                                            #endregion

                                            #endregion
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

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

        public ValidationMsg CheckedCrustProductionSchedule(PrdCrustLeatherQC model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityYearMonth = _context.PRD_FinishLeatherQC.First(m => m.FinishLeatherQCID == model.CrustLeatherQCID);
                        originalEntityYearMonth.RecordStatus = "CHK";
                        originalEntityYearMonth.ModifiedBy = userid;
                        originalEntityYearMonth.ModifiedOn = DateTime.Now;

                        //var originalEntityYearMonthSchedule = _context.PRD_YearMonthSchedule.First(m => m.CrustLeatherQCNo == model.CrustLeatherQCNo);
                        //originalEntityYearMonthSchedule.RecordStatus = "CHK";
                        //originalEntityYearMonthSchedule.ModifiedBy = userid;
                        //originalEntityYearMonthSchedule.ModifiedOn = DateTime.Now;

                        //if (model.PrdCrustLeatherQCItemList.Count > 0)
                        //{
                        //    foreach (PrdYearMonthScheduleDate objPrdYearMonthScheduleDate in model.PrdCrustLeatherQCItemList)
                        //    {
                        //        var originalEntityYearMonthScheduleDate = _context.PRD_YearMonthScheduleDate.First(m => m.ScheduleDateID == objPrdYearMonthScheduleDate.ScheduleDateID);
                        //        originalEntityYearMonthScheduleDate.RecordStatus = "CHK";
                        //        originalEntityYearMonthScheduleDate.ModifiedBy = userid;
                        //        originalEntityYearMonthScheduleDate.ModifiedOn = DateTime.Now;
                        //    }
                        //    if (model.PrdCrustLeatherQCSelectionList.Count > 0)
                        //    {
                        //        foreach (PrdYearMonthSchedulePurchase objPrdYearMonthSchedulePurchase in model.PrdCrustLeatherQCSelectionList)
                        //        {
                        //            var originalEntityYearMonthSchedulePurchase = _context.PRD_YearMonthSchedulePurchase.First(m => m.SchedulePurchaseID == objPrdYearMonthSchedulePurchase.SchedulePurchaseID);
                        //            originalEntityYearMonthSchedulePurchase.RecordStatus = "CHK";
                        //            originalEntityYearMonthSchedulePurchase.ModifiedBy = userid;
                        //            originalEntityYearMonthSchedulePurchase.ModifiedOn = DateTime.Now;
                        //        }
                        //    }
                        //}

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

        public ValidationMsg DeletedCrustLeatherQC(long CrustLeatherQCID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var scheduleList = _context.PRD_FinishLeatherQC.Where(m => m.FinishLeatherQCID == CrustLeatherQCID).ToList();
                if (scheduleList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteYearMonthSchedule = _context.PRD_FinishLeatherQC.First(m => m.FinishLeatherQCID == CrustLeatherQCID);
                    _context.PRD_FinishLeatherQC.Remove(deleteYearMonthSchedule);
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

        //public ValidationMsg DeletedCrustQCItem(long ScheduleDateID, string RecordStatus)
        public ValidationMsg DeletedCrustQCItem(long QCItemID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var CrustLeatherQCID = _context.PRD_FinishLeatherQCItem.Where(m => m.FNQCItemID == QCItemID).FirstOrDefault().FinishLeatherQCID;
                var RecordStatus = _context.PRD_FinishLeatherQC.First(m => m.FinishLeatherQCID == CrustLeatherQCID).RecordStatus;
                if (RecordStatus != "CNF")
                {
                    var scheduleList = _context.PRD_FinishLeatherQCSelection.Where(m => m.FNQCItemID == QCItemID).ToList();
                    if (scheduleList.Count > 0)
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Child Record Found.";
                    }
                    else
                    {
                        var deleteElement = _context.PRD_FinishLeatherQCItem.First(m => m.FNQCItemID == QCItemID);
                        _context.PRD_FinishLeatherQCItem.Remove(deleteElement);
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

        //public ValidationMsg DeletedScheduleDrum(long schedulePurchaseID, string RecordStatus)
        public ValidationMsg DeletedQCSelection(long QCSelectionID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var QCItemID = _context.PRD_FinishLeatherQCSelection.Where(m => m.FNQCSelectionID == QCSelectionID).FirstOrDefault().FNQCItemID;
                var CrustLeatherQCID = _context.PRD_FinishLeatherQCItem.Where(m => m.FNQCItemID == QCItemID).FirstOrDefault().FinishLeatherQCID;
                var RecordStatus = _context.PRD_FinishLeatherQC.First(m => m.FinishLeatherQCID == CrustLeatherQCID).RecordStatus;
                if (RecordStatus != "CNF")
                {
                    var deleteElement = _context.PRD_FinishLeatherQCSelection.First(m => m.FNQCSelectionID == QCSelectionID);
                    _context.PRD_FinishLeatherQCSelection.Remove(deleteElement);

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

        public List<PrdYearMonthCrustScheduleDrum> GetGradeInfoInFromQCStock(string ProductionFloor, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ColorID, string ArticleChallanID)
        {
            if (!string.IsNullOrEmpty(ProductionFloor))
            {
                if (!string.IsNullOrEmpty(BuyerID))
                {
                    if (string.IsNullOrEmpty(BuyerOrderID))
                    {
                        var query = @"select inv.TransectionID,inv.GradeRange,inv.ClosingProductionPcs ProductionPcs,inv.ClosingProductionSide ProductionSide,inv.ClosingProductionArea ProductionArea from dbo.PRD_FinishLeatherProductionStock inv
                     INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,BuyerOrderID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ArticleChallanID from dbo.PRD_FinishLeatherProductionStock
                     where QCStatus='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ArticleChallanID) sup
                     ON inv.TransectionID=sup.TransectionID
                     where inv.StoreID = " + ProductionFloor + " and inv.BuyerID = " + BuyerID + " and inv.BuyerOrderID IS NULL and inv.ItemTypeID = " + ItemTypeID + " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ColorID = " + ColorID + "  and inv.ArticleChallanID = " + ArticleChallanID + " and (inv.ClosingProductionPcs>0 or inv.ClosingProductionSide>0 or inv.ClosingProductionArea>0)";
                        var allData = _context.Database.SqlQuery<PrdYearMonthCrustScheduleDrum>(query).ToList();
                        List<PrdYearMonthCrustScheduleDrum> searchList = allData.ToList();
                        return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdYearMonthCrustScheduleDrum>();
                    }
                    else
                    {
                        var query = @"select inv.TransectionID,inv.GradeRange,inv.ClosingProductionPcs ProductionPcs,inv.ClosingProductionSide ProductionSide,inv.ClosingProductionArea ProductionArea from dbo.PRD_FinishLeatherProductionStock inv
                     INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,BuyerOrderID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ArticleChallanID from dbo.PRD_FinishLeatherProductionStock
                     where QCStatus='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ArticleChallanID) sup
                     ON inv.TransectionID=sup.TransectionID
                     where inv.StoreID = " + ProductionFloor + " and inv.BuyerID = " + BuyerID + " and inv.BuyerOrderID = " + BuyerOrderID + " and inv.ItemTypeID = " + ItemTypeID + " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ColorID = " + ColorID + "  and inv.ArticleChallanID = " + ArticleChallanID + " and (inv.ClosingProductionPcs>0 or inv.ClosingProductionSide>0 or inv.ClosingProductionArea>0)";
                        var allData = _context.Database.SqlQuery<PrdYearMonthCrustScheduleDrum>(query).ToList();
                        List<PrdYearMonthCrustScheduleDrum> searchList = allData.ToList();
                        return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdYearMonthCrustScheduleDrum>();
                    }
                }
                else
                {
                    var query = @"select inv.TransectionID,inv.GradeRange,inv.ClosingProductionPcs ProductionPcs,inv.ClosingProductionSide ProductionSide,inv.ClosingProductionArea ProductionArea from dbo.PRD_FinishLeatherProductionStock inv
                     INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,BuyerOrderID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ArticleChallanID from dbo.PRD_FinishLeatherProductionStock
                     where QCStatus='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ArticleChallanID) sup
                     ON inv.TransectionID=sup.TransectionID
                     where inv.StoreID = " + ProductionFloor + " and inv.BuyerID IS NULL and inv.BuyerOrderID  IS NULL and inv.ItemTypeID = " + ItemTypeID + " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ColorID = " + ColorID + "  and inv.ArticleChallanID = " + ArticleChallanID + " and (inv.ClosingProductionPcs>0 or inv.ClosingProductionSide>0 or inv.ClosingProductionArea>0)";
                    var allData = _context.Database.SqlQuery<PrdYearMonthCrustScheduleDrum>(query).ToList();
                    List<PrdYearMonthCrustScheduleDrum> searchList = allData.ToList();
                    return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdYearMonthCrustScheduleDrum>();
                }
            }
            else
                return null;
        }

        public PrdYearMonthCrustScheduleDrum SetToBussinessObject(PrdYearMonthCrustScheduleDrum Entity)
        {
            PrdYearMonthCrustScheduleDrum Model = new PrdYearMonthCrustScheduleDrum();

            Model.GradeRange = Entity.GradeRange;
            Model.ProductionPcs = Entity.ProductionPcs;
            Model.ProductionSide = Entity.ProductionSide;
            Model.ProductionArea = Entity.ProductionArea;

            return Model;
        }
    }
}
