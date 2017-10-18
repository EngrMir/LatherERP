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
    public class DalCrustLeatherQC
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long CrustLeatherQCID = 0;
        //public long ScheduleDateID = 0;
        public string CrustLeatherQCNo = string.Empty;
        public DalCrustLeatherQC()
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

                            PRD_CrustLeatherQC tblQC = SetToModelObject(model, userid);
                            _context.PRD_CrustLeatherQC.Add(tblQC);
                            _context.SaveChanges();

                            #endregion

                            #region Save QCItem Records

                            if (model.PrdCrustLeatherQCItemList != null)
                            {
                                foreach (PrdCrustLeatherQCItem objQCItem in model.PrdCrustLeatherQCItemList)
                                {
                                    objQCItem.CrustLeatherQCID = tblQC.CrustLeatherQCID;
                                    PRD_CrustLeatherQCItem tblQCItem = SetToModelObject(objQCItem, userid);
                                    _context.PRD_CrustLeatherQCItem.Add(tblQCItem);
                                    _context.SaveChanges();

                                    #region Save QCSelection List

                                    if (model.PrdCrustLeatherQCSelectionList != null)
                                    {
                                        foreach (PrdCrustLeatherQCSelection objQCSelection in model.PrdCrustLeatherQCSelectionList)
                                        {
                                            objQCSelection.CLQCItemID = tblQCItem.CLQCItemID;
                                            objQCSelection.CrustLeatherQCID = tblQC.CrustLeatherQCID;
                                            objQCSelection.ScheduleItemID = objQCItem.ScheduleItemID;
                                            objQCSelection.ScheduleProductionNo = objQCItem.ScheduleProductionNo;
                                            PRD_CrustLeatherQCSelection tblQCSelection = SetToModelObject(objQCSelection, userid);
                                            _context.PRD_CrustLeatherQCSelection.Add(tblQCSelection);
                                        }
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            _context.SaveChanges();
                            tx.Complete();
                            CrustLeatherQCID = tblQC.CrustLeatherQCID;
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

                        PRD_CrustLeatherQC CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.PRD_CrustLeatherQC.First(m => m.CrustLeatherQCID == model.CrustLeatherQCID);

                        OriginalEntity.CrustLeatherQCDate = CurrentEntity.CrustLeatherQCDate;
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
                                    PRD_CrustLeatherQCItem tblQCItem = SetToModelObject(objQCItem, userid);
                                    _context.PRD_CrustLeatherQCItem.Add(tblQCItem);
                                    _context.SaveChanges();
                                    objQCItem.CLQCItemID = tblQCItem.CLQCItemID;
                                }
                                else
                                {
                                    PRD_CrustLeatherQCItem CurrEntity = SetToModelObject(objQCItem, userid);
                                    var OrgrEntity = _context.PRD_CrustLeatherQCItem.First(m => m.CLQCItemID == objQCItem.CLQCItemID);

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

                                            PRD_CrustLeatherQCSelection tblQCSelection = SetToModelObject(objQCSelection, userid);
                                            _context.PRD_CrustLeatherQCSelection.Add(tblQCSelection);
                                        }
                                        else
                                        {
                                            PRD_CrustLeatherQCSelection CurEntity = SetToModelObject(objQCSelection, userid);
                                            var OrgEntity = _context.PRD_CrustLeatherQCSelection.First(m => m.CLQCSelectionID == objQCSelection.CLQCSelectionID);

                                            OrgEntity.ScheduleItemID = CurEntity.ScheduleItemID;
                                            OrgEntity.ScheduleProductionNo = CurEntity.ScheduleProductionNo;
                                            //OrgEntity.GradeID = CurEntity.GradeID;
                                            OrgEntity.GradeRange = CurEntity.GradeRange;
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

        public PRD_CrustLeatherQC SetToModelObject(PrdCrustLeatherQC model, int userid)
        {
            PRD_CrustLeatherQC Entity = new PRD_CrustLeatherQC();

            Entity.CrustLeatherQCID = model.CrustLeatherQCID;
            Entity.CrustLeatherQCNo = model.CrustLeatherQCNo;
            Entity.CrustLeatherQCDate = DalCommon.SetDate(model.CrustLeatherQCDate);
            Entity.ProductionFloor = model.ProductionFloor;
            Entity.AfterQCIssueStore = model.QCStore;
            Entity.Remarks = model.Remarks;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public PRD_CrustLeatherQCItem SetToModelObject(PrdCrustLeatherQCItem model, int userid)
        {
            PRD_CrustLeatherQCItem Entity = new PRD_CrustLeatherQCItem();

            Entity.CLQCItemID = model.CLQCItemID;
            //Entity.CrustLeatherQCRefID = model.CrustLeatherQCRefID == 0 ? null : model.CrustLeatherQCRefID;
            Entity.CrustLeatherQCID = model.CrustLeatherQCID;
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
            Entity.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
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

        public PRD_CrustLeatherQCSelection SetToModelObject(PrdCrustLeatherQCSelection model, int userid)
        {
            PRD_CrustLeatherQCSelection Entity = new PRD_CrustLeatherQCSelection();

            Entity.CLQCSelectionID = model.CLQCSelectionID;
            Entity.CLQCItemID = model.CLQCItemID;
            Entity.CrustLeatherQCID = model.CrustLeatherQCID;
            Entity.ScheduleItemID = model.ScheduleItemID;
            Entity.ScheduleProductionNo = model.ScheduleProductionNo;
            //Entity.GradeID = model.GradeID;
            Entity.GradeRange = model.GradeRange;
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
                            inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleChallanID,inv.ArticleChallanNo,inv.ArticleColorNo
                            from dbo.PRD_CrustLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID from dbo.PRD_CrustLeatherProductionStock
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
                            inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleChallanID,inv.ArticleChallanNo,inv.ArticleColorNo
                            from dbo.PRD_CrustLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID from dbo.PRD_CrustLeatherProductionStock
                            where QCStatus ='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID) sup
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
                            inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleChallanID,inv.ArticleChallanNo,inv.ArticleColorNo
                            from dbo.PRD_CrustLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID from dbo.PRD_CrustLeatherProductionStock
                            where QCStatus ='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID) sup
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
                        var query = @"select inv.TransectionID,inv.GradeRange,
                            inv.ClosingProductionPcs ProductionPcs,inv.ClosingProductionSide ProductionSide,inv.ClosingProductionArea ProductionArea from dbo.PRD_CrustLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ArticleChallanID,ColorID from dbo.PRD_CrustLeatherProductionStock
                            where QCStatus ='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ArticleChallanID,ColorID) sup
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
                        var query = @"select inv.TransectionID,inv.GradeRange,
                            inv.ClosingProductionPcs ProductionPcs,inv.ClosingProductionSide ProductionSide,inv.ClosingProductionArea ProductionArea from dbo.PRD_CrustLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ArticleChallanID,ColorID from dbo.PRD_CrustLeatherProductionStock
                            where QCStatus ='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ArticleChallanID,ColorID) sup
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
                    var query = @"select inv.TransectionID,inv.GradeRange,
                            inv.ClosingProductionPcs ProductionPcs,inv.ClosingProductionSide ProductionSide,inv.ClosingProductionArea ProductionArea from dbo.PRD_CrustLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ArticleChallanID,ColorID from dbo.PRD_CrustLeatherProductionStock
                            where QCStatus ='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ArticleChallanID,ColorID) sup
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

            //Model.GradeID = Entity.GradeID;
            //Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.GradeRange = Entity.GradeRange;
            Model.ProductionPcs = Entity.ProductionPcs;
            Model.ProductionSide = Entity.ProductionSide;
            Model.ProductionArea = Entity.ProductionArea;

            Model.QCPassPcs = Entity.ProductionPcs;
            Model.QCPassSide = Entity.ProductionSide;
            Model.QCPassArea = Entity.ProductionArea;

            Model.QCPassAreaUnitName = "SFT";
            Model.QCFailAreaUnitName = "SFT";

            return Model;
        }

        public List<PrdCrustLeatherQCItem> GetQCColorList(string CrustLeatherQCID)
        {
            long? crustLeatherQCID = Convert.ToInt64(CrustLeatherQCID);
            List<PRD_CrustLeatherQCItem> searchList = _context.PRD_CrustLeatherQCItem.Where(m => m.CrustLeatherQCID == crustLeatherQCID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdCrustLeatherQCItem>();
        }

        public PrdCrustLeatherQCItem SetToBussinessObject(PRD_CrustLeatherQCItem Entity)
        {
            PrdCrustLeatherQCItem Model = new PrdCrustLeatherQCItem();

            Model.CLQCItemID = Entity.CLQCItemID;
            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleName = Entity.ArticleID == null ? "" : _context.Sys_Article.Where(m => m.ArticleID == Entity.ArticleID).FirstOrDefault().ArticleName;
            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleChallanID = Entity.ArticleChallanID;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.ArticleColorNo = Convert.ToInt32(Entity.ArticleColorNo);
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            Model.ProductionPcs = Entity.ProductionPcs;
            Model.ProductionSide = Entity.ProductionSide;
            Model.ProductionArea = Entity.Productionrea;

            return Model;
        }

        public List<PrdCrustLeatherQCSelection> GetQCSelectionList(string CLQCItemID)
        {

            long? qcItemID = Convert.ToInt64(CLQCItemID);
            List<PRD_CrustLeatherQCSelection> searchList = _context.PRD_CrustLeatherQCSelection.Where(m => m.CLQCItemID == qcItemID).OrderByDescending(m => m.CLQCSelectionID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdCrustLeatherQCSelection>();

        }

        public PrdCrustLeatherQCSelection SetToBussinessObject(PRD_CrustLeatherQCSelection Entity)
        {
            PrdCrustLeatherQCSelection Model = new PrdCrustLeatherQCSelection();

            Model.CLQCSelectionID = Entity.CLQCSelectionID;
            //Model.GradeID = Entity.GradeID;
            //Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.GradeRange = Entity.GradeRange;
            Model.ProductionPcs = Entity.ProductionPcs;
            Model.ProductionSide = Entity.ProductionSide;
            Model.ProductionArea = Entity.ProductionArea;
            Model.ProductionAreaUnit = Entity.ProductionAreaUnit;
            //Model.ProductionAreaUnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;
            var CLQCItemID = _context.PRD_CrustLeatherQCItem.Where(m => m.CLQCItemID == Entity.CLQCItemID).FirstOrDefault().CLQCItemID;
            var storeId = _context.PRD_CrustLeatherQC.Where(m => m.CrustLeatherQCID == Entity.CrustLeatherQCID).FirstOrDefault().ProductionFloor;
            var BuyerID = _context.PRD_CrustLeatherQCItem.Where(m => m.CLQCItemID == Entity.CLQCItemID).FirstOrDefault().BuyerID;
            var BuyerOrderID = _context.PRD_CrustLeatherQCItem.Where(m => m.CLQCItemID == Entity.CLQCItemID).FirstOrDefault().BuyerOrderID;
            var ItemTypeID = _context.PRD_CrustLeatherQCItem.Where(m => m.CLQCItemID == Entity.CLQCItemID).FirstOrDefault().ItemTypeID;
            var LeatherTypeID = _context.PRD_CrustLeatherQCItem.Where(m => m.CLQCItemID == Entity.CLQCItemID).FirstOrDefault().LeatherTypeID;
            var LeatherStatusID = _context.PRD_CrustLeatherQCItem.Where(m => m.CLQCItemID == Entity.CLQCItemID).FirstOrDefault().LeatherStatusID;
            var ArticleID = _context.PRD_CrustLeatherQCItem.Where(m => m.CLQCItemID == Entity.CLQCItemID).FirstOrDefault().ArticleID;

            Model.ProductionPcs = _context.PRD_CrustLeatherProductionStock.Where(ma => ma.StoreID == storeId
                && ma.BuyerID == BuyerID
                && ma.BuyerOrderID == BuyerOrderID
                && ma.ItemTypeID == ItemTypeID
                && ma.LeatherTypeID == LeatherTypeID
                && ma.LeatherStatusID == LeatherStatusID
                && ma.ArticleID == ArticleID
                && ma.GradeID == Entity.GradeID
                && ma.QCStatus == "PCM").OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingProductionPcs;
            Model.ProductionSide = _context.PRD_CrustLeatherProductionStock.Where(ma => ma.StoreID == storeId
                && ma.BuyerID == BuyerID
                && ma.BuyerOrderID == BuyerOrderID
                && ma.ItemTypeID == ItemTypeID
                && ma.LeatherTypeID == LeatherTypeID
                && ma.LeatherStatusID == LeatherStatusID
                && ma.ArticleID == ArticleID
                && ma.GradeID == Entity.GradeID
                && ma.QCStatus == "PCM").OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingProductionSide;
            Model.ProductionArea = _context.PRD_CrustLeatherProductionStock.Where(ma => ma.StoreID == storeId
                && ma.BuyerID == BuyerID
                && ma.BuyerOrderID == BuyerOrderID
                && ma.ItemTypeID == ItemTypeID
                && ma.LeatherTypeID == LeatherTypeID
                && ma.LeatherStatusID == LeatherStatusID
                && ma.ArticleID == ArticleID
                && ma.GradeID == Entity.GradeID
                && ma.QCStatus == "PCM").OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingProductionArea;

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

            return Model;
        }

        public List<PrdCrustLeatherQC> GetCrustLeatherQCInfo()
        {
            List<PRD_CrustLeatherQC> searchList = _context.PRD_CrustLeatherQC.OrderByDescending(m => m.CrustLeatherQCID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<PrdCrustLeatherQC>();
        }

        public PrdCrustLeatherQC SetToBussinessObject(PRD_CrustLeatherQC Entity)
        {
            PrdCrustLeatherQC Model = new PrdCrustLeatherQC();

            Model.CrustLeatherQCID = Entity.CrustLeatherQCID;
            Model.CrustLeatherQCNo = Entity.CrustLeatherQCNo;
            Model.CrustLeatherQCDate = string.IsNullOrEmpty(Entity.CrustLeatherQCDate.ToString()) ? string.Empty : Convert.ToDateTime(Entity.CrustLeatherQCDate).ToString("dd/MM/yyyy");
            Model.ProductionFloor = Entity.ProductionFloor;
            Model.ProductionFloorName = Entity.ProductionFloor == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.ProductionFloor).FirstOrDefault().StoreName;
            Model.QCStore = Entity.AfterQCIssueStore;
            Model.QCStoreName = Entity.AfterQCIssueStore == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.AfterQCIssueStore).FirstOrDefault().StoreName;
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
                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
                            inv.LeatherTypeID,(select LeatherTypeName from dbo.Sys_LeatherType where LeatherTypeID = inv.LeatherTypeID)LeatherTypeName,
                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,inv.ArticleChallanID,inv.ArticleChallanNo,inv.ArticleColorNo,
                            inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,
                            inv.ClosingProductionPcs,inv.ClosingProductionSide,inv.ClosingProductionArea from dbo.PRD_CrustLeatherProductionStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ArticleChallanID,ColorID from dbo.PRD_CrustLeatherProductionStock
                            where QCStatus ='PCM' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ArticleChallanID,ColorID) sup
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
                        var originalEntityYearMonth = _context.PRD_CrustLeatherQC.First(m => m.CrustLeatherQCID == model.CrustLeatherQCID);
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

                                        var CrustProductionStock = (from ds in _context.PRD_CrustLeatherProductionStock.AsEnumerable()
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
                                            var LastItemInfo = (from ds in _context.PRD_CrustLeatherProductionStock
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

                                                var objPCMStockItem = new PRD_CrustLeatherProductionStock();

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

                                                //objPCMStockItem.GradeID = objQCSelection.GradeID;

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

                                                _context.PRD_CrustLeatherProductionStock.Add(objPCMStockItem);
                                                _context.SaveChanges();

                                                #endregion

                                                #region Receive QC Quantity in CrustLeatherProductionStock

                                                //var CheckItemStock = (from i in _context.PRD_CrustLeatherProductionStock.AsEnumerable()
                                                //                      where i.StoreID == model.QCStore
                                                //                            && i.BuyerID == objQCItem.BuyerID
                                                //                            && i.BuyerOrderID == objQCItem.BuyerOrderID
                                                //                            && i.ArticleID == objQCItem.ArticleID
                                                //                            && i.ItemTypeID == objQCItem.ItemTypeID
                                                //                            && i.LeatherStatusID == objQCItem.LeatherStatusID
                                                //                            && i.ColorID == objQCItem.ColorID
                                                //                            && i.GradeID == objQCSelection.GradeID
                                                //                            && i.QCStatus == "IFQ"
                                                //                      select i).Any();
                                                //if (!CheckItemStock)
                                                //{
                                                //    PRD_CrustLeatherProductionStock objStockItem = new PRD_CrustLeatherProductionStock();

                                                //    objStockItem.StoreID = Convert.ToByte(model.QCStore);
                                                //    if (objQCItem.BuyerID == null)
                                                //        objStockItem.BuyerID = null;
                                                //    else
                                                //        objStockItem.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                //    if (objQCItem.BuyerOrderID == null)
                                                //        objStockItem.BuyerOrderID = null;
                                                //    else
                                                //        objStockItem.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                //    objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                //    objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                //    objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                //    objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                //    objStockItem.ColorID = Convert.ToInt32(objQCItem.ColorID);
                                                //    objStockItem.GradeID = Convert.ToSByte(objQCSelection.GradeID);

                                                //    if (string.IsNullOrEmpty(objQCSelection.QCPassAreaUnitName))
                                                //        objStockItem.AreaUnit = null;
                                                //    else
                                                //        objStockItem.AreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCPassAreaUnitName).FirstOrDefault().UnitID;

                                                //    objStockItem.QCStatus = "IFQ";

                                                //    objStockItem.OpeningPcs = 0;
                                                //    objStockItem.ReceivePcs = (objQCSelection.QCPassPcs == null ? 0 : objQCSelection.QCPassPcs) + (objQCSelection.QCFailPcs == null ? 0 : objQCSelection.QCFailPcs); //objQCSelection.QCPassPcs;
                                                //    objStockItem.IssuePcs = 0;
                                                //    objStockItem.ClosingProductionPcs = (objQCSelection.QCPassPcs == null ? 0 : objQCSelection.QCPassPcs) + (objQCSelection.QCFailPcs == null ? 0 : objQCSelection.QCFailPcs);

                                                //    objStockItem.OpeningSide = 0;
                                                //    objStockItem.ReceiveSide = (objQCSelection.QCPassSide == null ? 0 : objQCSelection.QCPassSide) + (objQCSelection.QCFailSide == null ? 0 : objQCSelection.QCFailSide);
                                                //    objStockItem.IssueSide = 0;
                                                //    objStockItem.ClosingProductionSide = (objQCSelection.QCPassSide == null ? 0 : objQCSelection.QCPassSide) + (objQCSelection.QCFailSide == null ? 0 : objQCSelection.QCFailSide);

                                                //    objStockItem.OpeningArea = 0;
                                                //    objStockItem.ReceiveArea = (objQCSelection.QCPassArea == null ? 0 : objQCSelection.QCPassArea) + (objQCSelection.QCFailArea == null ? 0 : objQCSelection.QCFailArea);
                                                //    objStockItem.IssueArea = 0;
                                                //    objStockItem.ClosingProductionArea = (objQCSelection.QCPassArea == null ? 0 : objQCSelection.QCPassArea) + (objQCSelection.QCFailArea == null ? 0 : objQCSelection.QCFailArea);

                                                //    _context.PRD_CrustLeatherProductionStock.Add(objStockItem);
                                                //    _context.SaveChanges();
                                                //}
                                                //else
                                                //{
                                                //    var LastItemInfo1 = (from i in _context.PRD_CrustLeatherProductionStock.AsEnumerable()
                                                //                         where i.StoreID == model.QCStore
                                                //                               && i.BuyerID == objQCItem.BuyerID
                                                //                               && i.BuyerOrderID == objQCItem.BuyerOrderID
                                                //                               && i.ArticleID == objQCItem.ArticleID
                                                //                               && i.ItemTypeID == objQCItem.ItemTypeID
                                                //                               && i.LeatherStatusID == objQCItem.LeatherStatusID
                                                //                               && i.ColorID == objQCItem.ColorID
                                                //                               && i.GradeID == objQCSelection.GradeID
                                                //                               && i.QCStatus == "IFQ"
                                                //                         orderby i.TransectionID descending
                                                //                         select i).FirstOrDefault();

                                                //    PRD_CrustLeatherProductionStock objStockItem1 = new PRD_CrustLeatherProductionStock();

                                                //    objStockItem1.StoreID = Convert.ToByte(model.QCStore);
                                                //    if (objQCItem.BuyerID == null)
                                                //        objStockItem1.BuyerID = null;
                                                //    else
                                                //        objStockItem1.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                //    if (objQCItem.BuyerOrderID == null)
                                                //        objStockItem1.BuyerOrderID = null;
                                                //    else
                                                //        objStockItem1.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                //    objStockItem1.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                //    objStockItem1.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                //    objStockItem1.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                //    objStockItem1.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                //    objStockItem1.ColorID = Convert.ToInt32(objQCItem.ColorID);
                                                //    objStockItem1.GradeID = Convert.ToSByte(objQCSelection.GradeID);

                                                //    if (string.IsNullOrEmpty(objQCSelection.QCFailAreaUnitName))
                                                //        objStockItem1.AreaUnit = null;
                                                //    else
                                                //        objStockItem1.AreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCFailAreaUnitName).FirstOrDefault().UnitID;

                                                //    objStockItem1.QCStatus = "IFQ";

                                                //    objPCMStockItem.OpeningPcs = LastItemInfo1.ClosingProductionPcs;
                                                //    objPCMStockItem.ReceivePcs = (objQCSelection.QCPassPcs == null ? 0 : objQCSelection.QCPassPcs) + (objQCSelection.QCFailPcs == null ? 0 : objQCSelection.QCFailPcs);
                                                //    objPCMStockItem.IssuePcs = 0;//
                                                //    objPCMStockItem.ClosingProductionPcs = LastItemInfo1.ClosingProductionPcs + ((objQCSelection.QCPassPcs == null ? 0 : objQCSelection.QCPassPcs) + (objQCSelection.QCFailPcs == null ? 0 : objQCSelection.QCFailPcs));

                                                //    objPCMStockItem.OpeningSide = LastItemInfo1.ClosingProductionSide;
                                                //    objPCMStockItem.ReceiveSide = (objQCSelection.QCPassSide == null ? 0 : objQCSelection.QCPassSide) + (objQCSelection.QCFailSide == null ? 0 : objQCSelection.QCFailSide);
                                                //    objPCMStockItem.IssueSide = 0;
                                                //    objPCMStockItem.ClosingProductionSide = LastItemInfo1.ClosingProductionSide + ((objQCSelection.QCPassSide == null ? 0 : objQCSelection.QCPassSide) + (objQCSelection.QCFailSide == null ? 0 : objQCSelection.QCFailSide));

                                                //    objPCMStockItem.OpeningArea = LastItemInfo1.ClosingProductionArea;
                                                //    objPCMStockItem.ReceiveArea = (objQCSelection.QCPassArea == null ? 0 : objQCSelection.QCPassArea) + (objQCSelection.QCFailArea == null ? 0 : objQCSelection.QCFailArea);
                                                //    objPCMStockItem.IssueArea = 0;
                                                //    objPCMStockItem.ClosingProductionArea = LastItemInfo1.ClosingProductionArea + ((objQCSelection.QCPassArea == null ? 0 : objQCSelection.QCPassArea) + (objQCSelection.QCFailArea == null ? 0 : objQCSelection.QCFailArea));

                                                //    _context.PRD_CrustLeatherProductionStock.Add(objStockItem1);
                                                //    _context.SaveChanges();
                                                //}

                                                #endregion
                                            }
                                        }

                                        #endregion

                                        if ((objQCSelection.QCPassPcs != null) || (objQCSelection.QCPassSide != null))
                                        {
                                            #region Receive QC PASS Stock

                                            var CheckItemStock = (from i in _context.INV_CrustQCStock.AsEnumerable()
                                                                  where i.StoreID == model.QCStore
                                                                        && i.BuyerID == objQCItem.BuyerID
                                                                        && i.BuyerOrderID == objQCItem.BuyerOrderID
                                                                        && i.ArticleID == objQCItem.ArticleID
                                                                        && i.ArticleChallanID == objQCItem.ArticleChallanID
                                                                        && i.ItemTypeID == objQCItem.ItemTypeID
                                                                        && i.LeatherStatusID == objQCItem.LeatherStatusID
                                                                        && i.ColorID == objQCItem.ColorID
                                                                      //&& i.GradeID == objQCSelection.GradeID
                                                                        && i.CrustQCLabel == "PASS"
                                                                  select i).Any();
                                            if (!CheckItemStock)
                                            {
                                                INV_CrustQCStock objStockItem = new INV_CrustQCStock();

                                                objStockItem.StoreID = Convert.ToByte(model.QCStore);
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

                                                //objStockItem.GradeID = Convert.ToSByte(objQCSelection.GradeID);
                                                if (string.IsNullOrEmpty(objQCSelection.QCPassAreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCPassAreaUnitName).FirstOrDefault().UnitID;
                                                objStockItem.CrustQCLabel = "PASS";

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

                                                _context.INV_CrustQCStock.Add(objStockItem);
                                                _context.SaveChanges();
                                            }
                                            else
                                            {
                                                var LastItemInfo = (from i in _context.INV_CrustQCStock.AsEnumerable()
                                                                    where i.StoreID == model.QCStore
                                                                          && i.BuyerID == objQCItem.BuyerID
                                                                          && i.BuyerOrderID == objQCItem.BuyerOrderID
                                                                          && i.ArticleID == objQCItem.ArticleID
                                                                          && i.ArticleChallanID == objQCItem.ArticleChallanID
                                                                          && i.ItemTypeID == objQCItem.ItemTypeID
                                                                          && i.LeatherStatusID == objQCItem.LeatherStatusID
                                                                          && i.ColorID == objQCItem.ColorID
                                                                        //&& i.GradeID == objQCSelection.GradeID
                                                                          && i.CrustQCLabel == "PASS"
                                                                    orderby i.TransectionID descending
                                                                    select i).FirstOrDefault();

                                                INV_CrustQCStock objStockItem = new INV_CrustQCStock();

                                                objStockItem.StoreID = Convert.ToByte(model.QCStore);
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

                                                //objStockItem.GradeID = Convert.ToSByte(objQCSelection.GradeID);
                                                if (string.IsNullOrEmpty(objQCSelection.QCPassAreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCPassAreaUnitName).FirstOrDefault().UnitID;
                                                objStockItem.CrustQCLabel = "PASS";

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

                                                _context.INV_CrustQCStock.Add(objStockItem);
                                                _context.SaveChanges();
                                            }

                                            #endregion
                                        }
                                        if ((objQCSelection.QCFailPcs != null) || (objQCSelection.QCFailSide != null))
                                        {
                                            #region Receive QC FAIL Stock

                                            var CheckItemStock = (from i in _context.INV_CrustQCStock.AsEnumerable()
                                                                  where i.StoreID == model.QCStore
                                                                        && i.BuyerID == objQCItem.BuyerID
                                                                        && i.BuyerOrderID == objQCItem.BuyerOrderID
                                                                        && i.ArticleID == objQCItem.ArticleID
                                                                        && i.ArticleChallanID == objQCItem.ArticleChallanID
                                                                        && i.ItemTypeID == objQCItem.ItemTypeID
                                                                        && i.LeatherStatusID == objQCItem.LeatherStatusID
                                                                        && i.ColorID == objQCItem.ColorID
                                                                      //&& i.GradeID == objQCSelection.GradeID
                                                                        && i.CrustQCLabel == "FAIL"
                                                                  select i).Any();
                                            if (!CheckItemStock)
                                            {
                                                INV_CrustQCStock objStockItem = new INV_CrustQCStock();

                                                objStockItem.StoreID = Convert.ToByte(model.QCStore);
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

                                                //objStockItem.GradeID = Convert.ToSByte(objQCSelection.GradeID);
                                                if (string.IsNullOrEmpty(objQCSelection.QCFailAreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCFailAreaUnitName).FirstOrDefault().UnitID;
                                                objStockItem.CrustQCLabel = "FAIL";

                                                objStockItem.OpeningStockPcs = 0;
                                                objStockItem.ReceiveStockPcs = objQCSelection.QCFailPcs == null ? 0 : objQCSelection.QCFailPcs;
                                                objStockItem.IssueStockPcs = 0;
                                                objStockItem.ClosingStockPcs = objQCSelection.QCFailPcs == null ? 0 : objQCSelection.QCFailPcs;

                                                objStockItem.OpeningStockSide = 0;
                                                objStockItem.ReceiveStockSide = objQCSelection.QCFailSide == null ? 0 : objQCSelection.QCFailSide;
                                                objStockItem.IssueStockSide = 0;
                                                objStockItem.ClosingStockSide = objQCSelection.QCFailSide == null ? 0 : objQCSelection.QCFailSide;

                                                objStockItem.OpeningStockArea = 0;
                                                objStockItem.ReceiveStockArea = objQCSelection.QCFailArea == null ? 0 : objQCSelection.QCFailArea;
                                                objStockItem.IssueStockArea = 0;
                                                objStockItem.ClosingStockArea = objQCSelection.QCFailArea == null ? 0 : objQCSelection.QCFailArea;

                                                _context.INV_CrustQCStock.Add(objStockItem);
                                                _context.SaveChanges();
                                            }
                                            else
                                            {
                                                var LastItemInfo = (from i in _context.INV_CrustQCStock.AsEnumerable()
                                                                    where i.StoreID == model.QCStore
                                                                          && i.BuyerID == objQCItem.BuyerID
                                                                          && i.BuyerOrderID == objQCItem.BuyerOrderID
                                                                          && i.ArticleID == objQCItem.ArticleID
                                                                          && i.ArticleChallanID == objQCItem.ArticleChallanID
                                                                          && i.ItemTypeID == objQCItem.ItemTypeID
                                                                          && i.LeatherStatusID == objQCItem.LeatherStatusID
                                                                          && i.ColorID == objQCItem.ColorID
                                                                        //&& i.GradeID == objQCSelection.GradeID
                                                                          && i.CrustQCLabel == "FAIL"
                                                                    orderby i.TransectionID descending
                                                                    select i).FirstOrDefault();

                                                INV_CrustQCStock objStockItem = new INV_CrustQCStock();

                                                objStockItem.StoreID = Convert.ToByte(model.QCStore);
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

                                                //objStockItem.GradeID = Convert.ToSByte(objQCSelection.GradeID);
                                                if (string.IsNullOrEmpty(objQCSelection.QCFailAreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.QCFailAreaUnitName).FirstOrDefault().UnitID;
                                                objStockItem.CrustQCLabel = "FAIL";

                                                objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
                                                objStockItem.ReceiveStockPcs = objQCSelection.QCFailPcs == null ? 0 : objQCSelection.QCFailPcs;
                                                objStockItem.IssueStockPcs = 0;
                                                objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs + objQCSelection.QCFailPcs == null ? 0 : objQCSelection.QCFailPcs;

                                                objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
                                                objStockItem.ReceiveStockSide = objQCSelection.QCFailSide == null ? 0 : objQCSelection.QCFailSide;
                                                objStockItem.IssueStockSide = 0;
                                                objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide + objQCSelection.QCFailSide == null ? 0 : objQCSelection.QCFailSide;

                                                objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea;
                                                objStockItem.ReceiveStockArea = objQCSelection.QCFailArea == null ? 0 : objQCSelection.QCFailArea;
                                                objStockItem.IssueStockArea = 0;
                                                objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea + objQCSelection.QCFailArea == null ? 0 : objQCSelection.QCFailArea;

                                                _context.INV_CrustQCStock.Add(objStockItem);
                                                _context.SaveChanges();
                                            }

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
                        var originalEntityYearMonth = _context.PRD_CrustLeatherQC.First(m => m.CrustLeatherQCID == model.CrustLeatherQCID);
                        originalEntityYearMonth.RecordStatus = "CHK";
                        originalEntityYearMonth.ModifiedBy = userid;
                        originalEntityYearMonth.ModifiedOn = DateTime.Now;

                        //var originalEntityYearMonthSchedule = _context.PRD_CrustLeatherQC.First(m => m.CrustLeatherQCNo == model.CrustLeatherQCNo);
                        //originalEntityYearMonthSchedule.RecordStatus = "CHK";
                        //originalEntityYearMonthSchedule.ModifiedBy = userid;
                        //originalEntityYearMonthSchedule.ModifiedOn = DateTime.Now;

                        //if (model.PrdCrustLeatherQCItemList.Count > 0)
                        //{
                        //    foreach (PrdYearMonthScheduleDate objPrdYearMonthScheduleDate in model.PrdCrustLeatherQCItemList)
                        //    {
                        //        var originalEntityYearMonthScheduleDate = _context.PRD_CrustLeatherQCItem.First(m => m.ScheduleDateID == objPrdYearMonthScheduleDate.ScheduleDateID);
                        //        originalEntityYearMonthScheduleDate.RecordStatus = "CHK";
                        //        originalEntityYearMonthScheduleDate.ModifiedBy = userid;
                        //        originalEntityYearMonthScheduleDate.ModifiedOn = DateTime.Now;
                        //    }
                        //    if (model.PrdCrustLeatherQCSelectionList.Count > 0)
                        //    {
                        //        foreach (PrdYearMonthSchedulePurchase objPrdYearMonthSchedulePurchase in model.PrdCrustLeatherQCSelectionList)
                        //        {
                        //            var originalEntityYearMonthSchedulePurchase = _context.PRD_CrustLeatherQCSelection.First(m => m.SchedulePurchaseID == objPrdYearMonthSchedulePurchase.SchedulePurchaseID);
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
                var scheduleList = _context.PRD_CrustLeatherQCItem.Where(m => m.CrustLeatherQCID == CrustLeatherQCID).ToList();
                if (scheduleList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteYearMonthSchedule = _context.PRD_CrustLeatherQC.First(m => m.CrustLeatherQCID == CrustLeatherQCID);
                    _context.PRD_CrustLeatherQC.Remove(deleteYearMonthSchedule);
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

        public ValidationMsg DeletedCrustQCItem(long QCItemID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var CrustLeatherQCID = _context.PRD_CrustLeatherQCItem.Where(m => m.CLQCItemID == QCItemID).FirstOrDefault().CrustLeatherQCID;
                var RecordStatus = _context.PRD_CrustLeatherQC.First(m => m.CrustLeatherQCID == CrustLeatherQCID).RecordStatus;
                if (RecordStatus != "CNF")
                {
                    var scheduleList = _context.PRD_CrustLeatherQCSelection.Where(m => m.CLQCItemID == QCItemID).ToList();
                    if (scheduleList.Count > 0)
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Child Record Found.";
                    }
                    else
                    {
                        var deleteElement = _context.PRD_CrustLeatherQCItem.First(m => m.CLQCItemID == QCItemID);
                        _context.PRD_CrustLeatherQCItem.Remove(deleteElement);
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

        public ValidationMsg DeletedQCSelection(long QCSelectionID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var QCItemID = _context.PRD_CrustLeatherQCSelection.Where(m => m.CLQCSelectionID == QCSelectionID).FirstOrDefault().CLQCItemID;
                var CrustLeatherQCID = _context.PRD_CrustLeatherQCItem.Where(m => m.CLQCItemID == QCItemID).FirstOrDefault().CrustLeatherQCID;
                var RecordStatus = _context.PRD_CrustLeatherQC.First(m => m.CrustLeatherQCID == CrustLeatherQCID).RecordStatus;
                if (RecordStatus != "CNF")
                {
                    var deleteElement = _context.PRD_CrustLeatherQCSelection.First(m => m.CLQCSelectionID == QCSelectionID);
                    _context.PRD_CrustLeatherQCSelection.Remove(deleteElement);

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
