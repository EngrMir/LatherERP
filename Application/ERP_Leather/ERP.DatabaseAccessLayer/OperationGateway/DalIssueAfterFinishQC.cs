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
    public class DalIssueAfterFinishQC
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long CrustLeatherIssueID = 0;
        //public long ScheduleDateID = 0;
        public string CrustLeatherIssueNo = string.Empty;
        private int stockError = 0;
        public DalIssueAfterFinishQC()
        {
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(InvCrustLeatherIssue model, int userid, string pageUrl)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        model.CrustLeatherIssueNo = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);//DalCommon.GetPreDefineValue("1", "00045");
                        if (model.CrustLeatherIssueNo != null)
                        {
                            #region CrustLeatherQC

                            INV_FinishLeatherIssue tblQC = SetToModelObject(model, userid);
                            _context.INV_FinishLeatherIssue.Add(tblQC);
                            _context.SaveChanges();

                            #endregion

                            #region Save QCItem Records

                            if (model.IssueItemList != null)
                            {
                                foreach (InvCrustLeatherIssueItem objQCItem in model.IssueItemList)
                                {
                                    objQCItem.CrustLeatherIssueID = tblQC.FinishLeatherIssueID;
                                    INV_FinishLeatherIssueItem tblQCItem = SetToModelObject(objQCItem, userid);
                                    _context.INV_FinishLeatherIssueItem.Add(tblQCItem);
                                    _context.SaveChanges();

                                    #region Save QCSelection List

                                    if (model.IssueColorList != null)
                                    {
                                        foreach (InvCrustLeatherIssueColor objQCSelection in model.IssueColorList)
                                        {
                                            objQCSelection.CrustLeatherIssueItemID = tblQCItem.FinishLeatherIssueItemID;
                                            objQCSelection.CrustLeatherIssueID = tblQC.FinishLeatherIssueID;
                                            INV_FinishLeatherIssueColor tblQCSelection = SetToModelObject(objQCSelection, userid);
                                            _context.INV_FinishLeatherIssueColor.Add(tblQCSelection);
                                        }
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            _context.SaveChanges();
                            tx.Complete();
                            CrustLeatherIssueID = tblQC.FinishLeatherIssueID;
                            CrustLeatherIssueNo = model.CrustLeatherIssueNo;

                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "CrustLeatherIssueNo Predefine Value not Found.";
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
                    _vmMsg.Msg = "IssueNo Already Exit.";
                }
            }
            return _vmMsg;
        }

        public ValidationMsg Update(InvCrustLeatherIssue model, int userid)
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

                        INV_FinishLeatherIssue CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.INV_FinishLeatherIssue.First(m => m.FinishLeatherIssueID == model.CrustLeatherIssueID);

                        OriginalEntity.FinishLeatherIssueDate = CurrentEntity.FinishLeatherIssueDate;

                        OriginalEntity.IssueCategory = CurrentEntity.IssueCategory;
                        OriginalEntity.IssueFor = CurrentEntity.IssueFor;

                        OriginalEntity.IssueQCStatus = CurrentEntity.IssueQCStatus;

                        OriginalEntity.IssueFrom = CurrentEntity.IssueFrom;
                        OriginalEntity.IssueTo = CurrentEntity.IssueTo;

                        OriginalEntity.ModifiedBy = userid;
                        OriginalEntity.ModifiedOn = DateTime.Now;

                        #endregion

                        #region Save QCItem List

                        if (model.IssueItemList != null)
                        {
                            foreach (InvCrustLeatherIssueItem objQCItem in model.IssueItemList)
                            {
                                if (objQCItem.CrustLeatherIssueItemID == 0)
                                {
                                    objQCItem.CrustLeatherIssueID = model.CrustLeatherIssueID;
                                    INV_FinishLeatherIssueItem tblQCItem = SetToModelObject(objQCItem, userid);
                                    _context.INV_FinishLeatherIssueItem.Add(tblQCItem);
                                    _context.SaveChanges();
                                    objQCItem.CrustLeatherIssueItemID = tblQCItem.FinishLeatherIssueItemID;
                                }
                                else
                                {
                                    INV_FinishLeatherIssueItem CurrEntity = SetToModelObject(objQCItem, userid);
                                    var OrgrEntity = _context.INV_FinishLeatherIssueItem.First(m => m.FinishLeatherIssueItemID == objQCItem.CrustLeatherIssueItemID);

                                    OrgrEntity.BuyerID = CurrEntity.BuyerID;
                                    OrgrEntity.BuyerOrderID = CurrEntity.BuyerOrderID;
                                    OrgrEntity.ArticleID = CurrEntity.ArticleID;
                                    OrgrEntity.ArticleNo = CurrEntity.ArticleNo;

                                    OrgrEntity.ArticleChallanID = CurrEntity.ArticleChallanID;
                                    OrgrEntity.ArticleChallanNo = CurrEntity.ArticleChallanNo;

                                    OrgrEntity.ItemTypeID = CurrEntity.ItemTypeID;
                                    OrgrEntity.LeatherTypeID = CurrEntity.LeatherTypeID;
                                    OrgrEntity.LeatherStatusID = CurrEntity.LeatherStatusID;
                                    //OrgrEntity.Remarks = CurrEntity.Remarks;
                                    OrgrEntity.ModifiedBy = userid;
                                    OrgrEntity.ModifiedOn = DateTime.Now;
                                }

                                #region Save QCSelection Records

                                if (model.IssueColorList != null)
                                {
                                    foreach (InvCrustLeatherIssueColor objQCSelection in model.IssueColorList)
                                    {
                                        if (objQCSelection.CrustLeatherIssueColorID == 0)
                                        {
                                            objQCSelection.CrustLeatherIssueItemID = objQCItem.CrustLeatherIssueItemID;
                                            objQCSelection.CrustLeatherIssueID = model.CrustLeatherIssueID;

                                            INV_FinishLeatherIssueColor tblQCSelection = SetToModelObject(objQCSelection, userid);
                                            _context.INV_FinishLeatherIssueColor.Add(tblQCSelection);
                                        }
                                        else
                                        {
                                            INV_FinishLeatherIssueColor CurEntity = SetToModelObject(objQCSelection, userid);
                                            var OrgEntity = _context.INV_FinishLeatherIssueColor.First(m => m.FinishLeatherIssueColorID == objQCSelection.CrustLeatherIssueColorID);

                                            OrgEntity.ColorID = CurEntity.ColorID;
                                            OrgEntity.GradeID = CurEntity.GradeID;
                                            OrgEntity.IssuePcs = CurEntity.IssuePcs;
                                            OrgEntity.IssueSide = CurEntity.IssueSide;
                                            OrgEntity.IssueArea = CurEntity.IssueArea;
                                            //OrgEntity.ProductionAreaUnit = CurEntity.ProductionAreaUnit;
                                            //OrgEntity.QCFailRemarks = CurEntity.QCFailRemarks;
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
                        CrustLeatherIssueID = model.CrustLeatherIssueID;
                        CrustLeatherIssueNo = model.CrustLeatherIssueNo;
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
                    _vmMsg.Msg = "IssueNo Already Exit.";
                }
            }
            return _vmMsg;
        }

        public INV_FinishLeatherIssue SetToModelObject(InvCrustLeatherIssue model, int userid)
        {
            INV_FinishLeatherIssue Entity = new INV_FinishLeatherIssue();

            Entity.FinishLeatherIssueID = model.CrustLeatherIssueID;
            Entity.FinishLeatherIssueNo = model.CrustLeatherIssueNo;
            Entity.FinishLeatherIssueDate = DalCommon.SetDate(model.CrustLeatherIssueDate);
            Entity.IssueFrom = model.IssueFrom;
            Entity.IssueCategory = model.IssueCategory;
            Entity.IssueFor = model.IssueFor;
            Entity.IssueTo = model.IssueTo;
            Entity.IssueQCStatus = model.IssueQCStatus;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public INV_FinishLeatherIssueItem SetToModelObject(InvCrustLeatherIssueItem model, int userid)
        {
            INV_FinishLeatherIssueItem Entity = new INV_FinishLeatherIssueItem();

            Entity.FinishLeatherIssueItemID = model.CrustLeatherIssueItemID;
            Entity.FinishLeatherIssueID = model.CrustLeatherIssueID;
            Entity.BuyerID = model.BuyerID;
            Entity.BuyerOrderID = model.BuyerOrderID;
            Entity.ArticleID = model.ArticleID;
            Entity.ArticleNo = model.ArticleNo;
            Entity.ArticleChallanID = model.ArticleChallanID;
            Entity.ArticleChallanNo = model.ArticleChallanNo;
            Entity.ItemTypeID = model.ItemTypeID;
            Entity.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID); ;
            Entity.LeatherStatusID = model.LeatherStatusID;
            //Entity.CrustQCLabel = model.CrustQCLabel;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public INV_FinishLeatherIssueColor SetToModelObject(InvCrustLeatherIssueColor model, int userid)
        {
            INV_FinishLeatherIssueColor Entity = new INV_FinishLeatherIssueColor();

            Entity.FinishLeatherIssueColorID = model.CrustLeatherIssueColorID;
            Entity.FinishLeatherIssueItemID = model.CrustLeatherIssueItemID;
            Entity.FinishLeatherIssueID = model.CrustLeatherIssueID;
            Entity.ArticleColorNo = model.ArticleColorNo;
            Entity.ColorID = model.ColorID;
            Entity.GradeID = model.GradeID;
            Entity.IssuePcs = model.IssuePcs;
            Entity.IssueSide = model.IssueSide;
            Entity.IssueArea = model.IssueArea;
            if (string.IsNullOrEmpty(model.AreaUnitName))
                Entity.AreaUnit = null;
            else
                Entity.AreaUnit = _context.Sys_Unit.Where(m => m.UnitName == model.AreaUnitName).FirstOrDefault().UnitID;
            Entity.FinishQCLabel = model.CrustQCLabel;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public long GetQCID()
        {
            return CrustLeatherIssueID;
        }

        public string GetQCNo()
        {
            return CrustLeatherIssueNo;
        }

        //        public List<InvCrustLeatherIssueItem> GetScheduleItemInfo(string IssueFrom)
        //        {
        //            if (!string.IsNullOrEmpty(IssueFrom))
        //            {
        //                var query = @"select inv.TransectionID,
        //                            inv.BuyerID,(select BuyerName from dbo.Sys_Buyer where BuyerID = inv.BuyerID)BuyerName,
        //                            inv.BuyerOrderID,(select BuyerOrderNo from dbo.SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID)BuyerOrderNo,
        //                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
        //                            inv.LeatherTypeID,(select LeatherTypeName from dbo.Sys_LeatherType where LeatherTypeID = inv.LeatherTypeID)LeatherTypeName,
        //                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
        //                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,
        //                            inv.ClosingStockPcs from dbo.INV_FinishBuyerQCStock inv
        //                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID from dbo.INV_FinishBuyerQCStock
        //                            group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID) sup
        //                            ON inv.TransectionID=sup.TransectionID
        //                            where inv.StoreID = " + IssueFrom + " and inv.ClosingStockPcs>0";
        //                var allData = _context.Database.SqlQuery<InvCrustLeatherIssueItem>(query).ToList();
        //                return allData;
        //            }
        //            return null;
        //        }

        //public List<InvCrustLeatherIssueItem> GetScheduleItemInfo(string IssueFrom, string IssueCategory, string CrustQCLabel)
        public List<InvCrustLeatherIssueItem> GetScheduleItemInfo(string IssueFrom, string IssueCategory)
        {
            if (!string.IsNullOrEmpty(IssueFrom))
            {
                if (IssueCategory == "AOQC")
                {
                    //if (CrustQCLabel == "ALL")
                    //{
                    var query = @"select inv.TransectionID,
                            inv.BuyerID,(select BuyerName from dbo.Sys_Buyer where BuyerID = inv.BuyerID)BuyerName,
                            inv.BuyerOrderID,(select BuyerOrderNo from dbo.SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID)BuyerOrderNo,
                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
                            --inv.LeatherTypeID,(select LeatherTypeName from dbo.Sys_LeatherType where LeatherTypeID = inv.LeatherTypeID)LeatherTypeName,
                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,inv.ArticleChallanID,inv.ArticleChallanNo,
                            inv.ClosingStockPcs from dbo.INV_FinishBuyerStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID from dbo.INV_FinishBuyerStock
                            where FinishQCLabel = 'OQF' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID) sup
                            ON inv.TransectionID=sup.TransectionID
                            where inv.StoreID = " + IssueFrom + " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";
                    var allData = _context.Database.SqlQuery<InvCrustLeatherIssueItem>(query).ToList();
                    return allData;
                    //                    }
                    //                    else
                    //                    {
                    //                        var query = @"select inv.TransectionID,
                    //                            inv.BuyerID,(select BuyerName from dbo.Sys_Buyer where BuyerID = inv.BuyerID)BuyerName,
                    //                            inv.BuyerOrderID,(select BuyerOrderNo from dbo.SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID)BuyerOrderNo,
                    //                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
                    //                            --inv.LeatherTypeID,(select LeatherTypeName from dbo.Sys_LeatherType where LeatherTypeID = inv.LeatherTypeID)LeatherTypeName,
                    //                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
                    //                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,inv.ArticleChallanID,inv.ArticleChallanNo,
                    //                            inv.ClosingStockPcs from dbo.INV_FinishOwnQCStock inv
                    //                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID from dbo.INV_FinishOwnQCStock
                    //                            where FinishQCLabel = '" + CrustQCLabel + "'" + " group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID) sup " +
                    //                                "ON inv.TransectionID=sup.TransectionID " +
                    //                                "where inv.StoreID = " + IssueFrom + " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";
                    //                        var allData = _context.Database.SqlQuery<InvCrustLeatherIssueItem>(query).ToList();
                    //                        return allData;
                    //                    }
                }
                else
                {
                    //if (CrustQCLabel == "ALL")
                    //{
                    var query = @"select inv.TransectionID,
                            inv.BuyerID,(select BuyerName from dbo.Sys_Buyer where BuyerID = inv.BuyerID)BuyerName,
                            inv.BuyerOrderID,(select BuyerOrderNo from dbo.SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID)BuyerOrderNo,
                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
                            --inv.LeatherTypeID,(select LeatherTypeName from dbo.Sys_LeatherType where LeatherTypeID = inv.LeatherTypeID)LeatherTypeName,
                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,inv.ArticleChallanID,inv.ArticleChallanNo,
                            inv.ClosingStockPcs from dbo.INV_FinishBuyerStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID from dbo.INV_FinishBuyerStock
                            where FinishQCLabel = 'BQF' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID) sup
                            ON inv.TransectionID=sup.TransectionID
                            where inv.StoreID = " + IssueFrom + " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";
                    var allData = _context.Database.SqlQuery<InvCrustLeatherIssueItem>(query).ToList();
                    return allData;
                    //                    }
                    //                    else
                    //                    {
                    //                        var query = @"select inv.TransectionID,
                    //                            inv.BuyerID,(select BuyerName from dbo.Sys_Buyer where BuyerID = inv.BuyerID)BuyerName,
                    //                            inv.BuyerOrderID,(select BuyerOrderNo from dbo.SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID)BuyerOrderNo,
                    //                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
                    //                            --inv.LeatherTypeID,(select LeatherTypeName from dbo.Sys_LeatherType where LeatherTypeID = inv.LeatherTypeID)LeatherTypeName,
                    //                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
                    //                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,inv.ArticleChallanID,inv.ArticleChallanNo,
                    //                            inv.ClosingStockPcs from dbo.INV_FinishBuyerQCStock inv
                    //                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID from dbo.INV_FinishBuyerQCStock
                    //                            where FinishQCLabel = '" + CrustQCLabel + "'" + " group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID) sup " +
                    //                                "ON inv.TransectionID=sup.TransectionID " +
                    //                                "where inv.StoreID = " + IssueFrom + " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";
                    //                        var allData = _context.Database.SqlQuery<InvCrustLeatherIssueItem>(query).ToList();
                    //                        return allData;
                    //                    }
                }
            }
            return null;
        }

        //        public List<InvCrustLeatherIssueColor> GetColorAndGradeList(string IssueFrom, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string FinishQCLabel)
        //        {
        //            if (!string.IsNullOrEmpty(IssueFrom))
        //            {
        //                if (FinishQCLabel == "ALL")
        //                {
        //                    var query = @"select inv.TransectionID,
        //                                inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,
        //							    inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName,
        //                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.FinishQCLabel from dbo.INV_FinishBuyerQCStock inv
        //                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID,FinishQCLabel from dbo.INV_FinishBuyerQCStock
        //                                group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID,FinishQCLabel) sup
        //                                ON inv.TransectionID=sup.TransectionID
        //                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID = " + BuyerID +
        //                                " and inv.BuyerOrderID = " + BuyerOrderID + " and inv.ItemTypeID = " + ItemTypeID +
        //                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID +
        //                                " and (inv.FinishQCLabel = 'PASS' or inv.FinishQCLabel = 'FAIL') and inv.ClosingStockPcs>0";

        //                    var allData = _context.Database.SqlQuery<InvCrustLeatherIssueColor>(query).ToList();
        //                    List<InvCrustLeatherIssueColor> searchList = allData.ToList();
        //                    return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueColor>();
        //                }
        //                else
        //                {
        //                    var query = @"select inv.TransectionID,
        //                                inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,
        //							    inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName,
        //                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.FinishQCLabel from dbo.INV_FinishBuyerQCStock inv
        //                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID,FinishQCLabel from dbo.INV_FinishBuyerQCStock
        //                                group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID,FinishQCLabel) sup
        //                                ON inv.TransectionID=sup.TransectionID
        //                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID = " + BuyerID +
        //                                " and inv.BuyerOrderID = " + BuyerOrderID + " and inv.ItemTypeID = " + ItemTypeID +
        //                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID +
        //                                " and inv.FinishQCLabel = '" + FinishQCLabel + "' and inv.ClosingStockPcs>0";

        //                    var allData = _context.Database.SqlQuery<InvCrustLeatherIssueColor>(query).ToList();
        //                    List<InvCrustLeatherIssueColor> searchList = allData.ToList();
        //                    return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueColor>();
        //                }
        //            }
        //            return null;
        //        }

        //public List<InvCrustLeatherIssueColor> GetColorAndGradeList(string IssueFrom, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ArticleChallanID, string IssueCategory, string CrustQCLabel)
        public List<InvCrustLeatherIssueColor> GetColorAndGradeList(string IssueFrom, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ArticleChallanID, string IssueCategory)
        {
            if (!string.IsNullOrEmpty(IssueFrom))
            {
                if (IssueCategory == "AOQC")
                {
                    if (string.IsNullOrEmpty(BuyerID))
                    {
                        var query = @"select inv.TransectionID,
                                inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.GradeRange,
								inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName,
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.FinishQCLabel from dbo.INV_FinishBuyerStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID from dbo.INV_FinishBuyerStock
                                where FinishQCLabel = 'OQF' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID IS NULL" +
                                " and inv.BuyerOrderID   IS NULL " + "and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ArticleChallanID = " + ArticleChallanID +
                                " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

                        var allData = _context.Database.SqlQuery<InvCrustLeatherIssueColor>(query).ToList();
                        List<InvCrustLeatherIssueColor> searchList = allData.ToList();
                        return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueColor>();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(BuyerOrderID))
                        {
                            var query = @"select inv.TransectionID,
                                inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.GradeRange,
								inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName,
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.FinishQCLabel from dbo.INV_FinishBuyerStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID from dbo.INV_FinishBuyerStock
                                where FinishQCLabel = 'OQF' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID = " + BuyerID +
                                " and inv.BuyerOrderID IS NULL and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ArticleChallanID = " + ArticleChallanID +
                                " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

                            var allData = _context.Database.SqlQuery<InvCrustLeatherIssueColor>(query).ToList();
                            List<InvCrustLeatherIssueColor> searchList = allData.ToList();
                            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueColor>();
                        }
                        else
                        {
                            var query = @"select inv.TransectionID,
                                inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.GradeRange,
								inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName,
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.FinishQCLabel from dbo.INV_FinishBuyerStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID from dbo.INV_FinishBuyerStock
                                where FinishQCLabel = 'OQF' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID = " + BuyerID +
                                " and inv.BuyerOrderID = " + BuyerOrderID + " and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ArticleChallanID = " + ArticleChallanID +
                                " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

                            var allData = _context.Database.SqlQuery<InvCrustLeatherIssueColor>(query).ToList();
                            List<InvCrustLeatherIssueColor> searchList = allData.ToList();
                            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueColor>();
                        }
                    }

                }
                else
                {

                    if (string.IsNullOrEmpty(BuyerID))
                    {
                        var query = @"select inv.TransectionID,
                                inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.GradeRange,
								inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName,
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.FinishQCLabel from dbo.INV_FinishBuyerStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID from dbo.INV_FinishBuyerStock
                                where FinishQCLabel = 'BQF' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID IS NULL" +
                                " and inv.BuyerOrderID   IS NULL " + "and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ArticleChallanID = " + ArticleChallanID +
                                " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

                        var allData = _context.Database.SqlQuery<InvCrustLeatherIssueColor>(query).ToList();
                        List<InvCrustLeatherIssueColor> searchList = allData.ToList();
                        return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueColor>();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(BuyerOrderID))
                        {
                            var query = @"select inv.TransectionID,
                                inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.GradeRange,
								inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName,
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.FinishQCLabel from dbo.INV_FinishBuyerStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID from dbo.INV_FinishBuyerStock
                                where FinishQCLabel = 'BQF' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID = " + BuyerID +
                                " and inv.BuyerOrderID IS NULL and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ArticleChallanID = " + ArticleChallanID +
                                " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

                            var allData = _context.Database.SqlQuery<InvCrustLeatherIssueColor>(query).ToList();
                            List<InvCrustLeatherIssueColor> searchList = allData.ToList();
                            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueColor>();
                        }
                        else
                        {
                            var query = @"select inv.TransectionID,
                                inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.GradeRange,
								inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName,
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.FinishQCLabel from dbo.INV_FinishBuyerStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID from dbo.INV_FinishBuyerStock
                                where FinishQCLabel = 'BQF' group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,FinishQCLabel,GradeID) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID = " + BuyerID +
                                " and inv.BuyerOrderID = " + BuyerOrderID + " and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + " and inv.ArticleChallanID = " + ArticleChallanID +
                                " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

                            var allData = _context.Database.SqlQuery<InvCrustLeatherIssueColor>(query).ToList();
                            List<InvCrustLeatherIssueColor> searchList = allData.ToList();
                            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueColor>();
                        }
                    }

                }
            }
            return null;
        }
        public InvCrustLeatherIssueColor SetToBussinessObject(InvCrustLeatherIssueColor Entity)
        {
            InvCrustLeatherIssueColor Model = new InvCrustLeatherIssueColor();

            Model.ArticleColorNo = Entity.ArticleColorNo;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.GradeID = Entity.GradeID;
            Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.ClosingStockPcs = Entity.ClosingStockPcs;
            Model.ClosingStockSide = Entity.ClosingStockSide;
            Model.ClosingStockArea = Entity.ClosingStockArea;
            Model.CrustQCLabel = Entity.FinishQCLabel;

            Model.IssuePcs = Entity.ClosingStockPcs;
            Model.IssueSide = Entity.ClosingStockSide;
            Model.IssueArea = Entity.ClosingStockArea;

            Model.AreaUnitName = "SFT";

            return Model;
        }

        public List<InvCrustLeatherIssueItem> GetQCColorList(string CrustLeatherIssueID)
        {
            long? crustLeatherQCID = Convert.ToInt64(CrustLeatherIssueID);
            List<INV_FinishLeatherIssueItem> searchList = _context.INV_FinishLeatherIssueItem.Where(m => m.FinishLeatherIssueID == crustLeatherQCID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueItem>();
        }

        public InvCrustLeatherIssueItem SetToBussinessObject(INV_FinishLeatherIssueItem Entity)
        {
            InvCrustLeatherIssueItem Model = new InvCrustLeatherIssueItem();

            Model.CrustLeatherIssueItemID = Entity.FinishLeatherIssueItemID;
            Model.BuyerID = Entity.BuyerID;
            Model.BuyerName = Entity.BuyerID == null ? "" : _context.Sys_Buyer.Where(m => m.BuyerID == Entity.BuyerID).FirstOrDefault().BuyerName;
            Model.BuyerOrderID = Entity.BuyerOrderID;
            Model.BuyerOrderNo = Entity.BuyerOrderID == null ? "" : _context.SLS_BuyerOrder.Where(m => m.BuyerOrderID == Entity.BuyerOrderID).FirstOrDefault().BuyerOrderNo;
            Model.ArticleID = Entity.ArticleID;
            Model.ArticleName = Entity.ArticleID == null ? "" : _context.Sys_Article.Where(m => m.ArticleID == Entity.ArticleID).FirstOrDefault().ArticleName;
            Model.ArticleNo = Entity.ArticleNo;
            Model.ArticleChallanID = Entity.ArticleChallanID;
            Model.ArticleChallanNo = Entity.ArticleChallanNo;
            Model.ItemTypeID = Entity.ItemTypeID;
            Model.ItemTypeName = Entity.ItemTypeID == null ? "" : _context.Sys_ItemType.Where(m => m.ItemTypeID == Entity.ItemTypeID).FirstOrDefault().ItemTypeName;
            Model.LeatherTypeID = Entity.LeatherTypeID;
            Model.LeatherStatusID = Entity.LeatherStatusID;
            Model.LeatherStatusName = Entity.BuyerID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;
            //Model.IssuePcs = Entity.IssuePcs;
            //Model.IssueArea = Entity.IssueArea;
            //Model.AreaUnit = Entity.Productionrea;

            return Model;
        }

        public List<InvCrustLeatherIssueColor> GetQCSelectionList(string CrustLeatherIssueItemID, string StoreId, string QCTransactionOf)
        {

            long? qcItemID = Convert.ToInt64(CrustLeatherIssueItemID);
            byte storeId = Convert.ToByte(StoreId);
            List<INV_FinishLeatherIssueColor> searchList = _context.INV_FinishLeatherIssueColor.Where(m => m.FinishLeatherIssueItemID == qcItemID).OrderByDescending(m => m.FinishLeatherIssueColorID).ToList();
            return searchList.Select(c => SetToBussinessObject(c, storeId, QCTransactionOf)).ToList<InvCrustLeatherIssueColor>();

        }

        public InvCrustLeatherIssueColor SetToBussinessObject(INV_FinishLeatherIssueColor Entity, byte storeId, string QCTransactionOf)
        {
            InvCrustLeatherIssueColor Model = new InvCrustLeatherIssueColor();

            Model.CrustLeatherIssueColorID = Entity.FinishLeatherIssueColorID;
            Model.CrustLeatherIssueItemID = Entity.FinishLeatherIssueItemID;
            Model.CrustLeatherIssueID = Entity.FinishLeatherIssueID;
            Model.ArticleColorNo = Entity.ArticleColorNo;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.GradeID = Entity.GradeID;
            Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;

            var BuyerID = _context.INV_FinishLeatherIssueItem.Where(m => m.FinishLeatherIssueItemID == Entity.FinishLeatherIssueItemID).FirstOrDefault().BuyerID;
            var BuyerOrderID = _context.INV_FinishLeatherIssueItem.Where(m => m.FinishLeatherIssueItemID == Entity.FinishLeatherIssueItemID).FirstOrDefault().BuyerOrderID;
            var ItemTypeID = _context.INV_FinishLeatherIssueItem.Where(m => m.FinishLeatherIssueItemID == Entity.FinishLeatherIssueItemID).FirstOrDefault().ItemTypeID;
            var LeatherTypeID = _context.INV_FinishLeatherIssueItem.Where(m => m.FinishLeatherIssueItemID == Entity.FinishLeatherIssueItemID).FirstOrDefault().LeatherTypeID;
            var LeatherStatusID = _context.INV_FinishLeatherIssueItem.Where(m => m.FinishLeatherIssueItemID == Entity.FinishLeatherIssueItemID).FirstOrDefault().LeatherStatusID;
            var ArticleID = _context.INV_FinishLeatherIssueItem.Where(m => m.FinishLeatherIssueItemID == Entity.FinishLeatherIssueItemID).FirstOrDefault().ArticleID;
            var ArticleChallanID = _context.INV_FinishLeatherIssueItem.Where(m => m.FinishLeatherIssueItemID == Entity.FinishLeatherIssueItemID).FirstOrDefault().ArticleChallanID;

            if (QCTransactionOf == "AOQC")
            {
                Model.ClosingStockPcs = _context.INV_FinishBuyerStock.Where(ma => ma.StoreID == storeId
                    && ma.BuyerID == BuyerID
                    && ma.BuyerOrderID == BuyerOrderID
                    && ma.ItemTypeID == ItemTypeID
                    && ma.LeatherStatusID == LeatherStatusID
                    && ma.ArticleID == ArticleID
                    && ma.ArticleChallanID == ArticleChallanID
                    && ma.ColorID == Entity.ColorID
                    && ma.FinishQCLabel == "OQF"
                    //&& ma.GradeID == Entity.GradeID
                    ).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockPcs;
                Model.ClosingStockSide = _context.INV_FinishBuyerStock.Where(ma => ma.StoreID == storeId
                    && ma.BuyerID == BuyerID
                    && ma.BuyerOrderID == BuyerOrderID
                    && ma.ItemTypeID == ItemTypeID
                    && ma.LeatherStatusID == LeatherStatusID
                    && ma.ArticleID == ArticleID
                    && ma.ColorID == Entity.ColorID
                    && ma.FinishQCLabel == "OQF"
                    //&& ma.GradeID == Entity.GradeID
                    ).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockSide;
                Model.ClosingStockArea = _context.INV_FinishBuyerStock.Where(ma => ma.StoreID == storeId
                    && ma.BuyerID == BuyerID
                    && ma.BuyerOrderID == BuyerOrderID
                    && ma.ItemTypeID == ItemTypeID
                    && ma.LeatherStatusID == LeatherStatusID
                    && ma.ArticleID == ArticleID
                    && ma.ArticleChallanID == ArticleChallanID
                    && ma.ColorID == Entity.ColorID
                    && ma.FinishQCLabel == "OQF"
                    //&& ma.GradeID == Entity.GradeID
                    ).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockArea;
            }
            else if (QCTransactionOf == "ABQC")
            {
                Model.ClosingStockPcs = _context.INV_FinishBuyerStock.Where(ma => ma.StoreID == storeId
                    && ma.BuyerID == BuyerID
                    && ma.BuyerOrderID == BuyerOrderID
                    && ma.ItemTypeID == ItemTypeID
                    && ma.LeatherStatusID == LeatherStatusID
                    && ma.ArticleID == ArticleID
                    && ma.ArticleChallanID == ArticleChallanID
                    && ma.ColorID == Entity.ColorID
                    && ma.FinishQCLabel == "BQF"
                    && ma.GradeID == Entity.GradeID).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockPcs;
                Model.ClosingStockSide = _context.INV_FinishBuyerStock.Where(ma => ma.StoreID == storeId
                    && ma.BuyerID == BuyerID
                    && ma.BuyerOrderID == BuyerOrderID
                    && ma.ItemTypeID == ItemTypeID
                    && ma.LeatherStatusID == LeatherStatusID
                    && ma.ArticleID == ArticleID
                    && ma.ArticleChallanID == ArticleChallanID
                    && ma.ColorID == Entity.ColorID
                    && ma.FinishQCLabel == "BQF"
                    && ma.GradeID == Entity.GradeID).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockSide;
                Model.ClosingStockArea = _context.INV_FinishBuyerStock.Where(ma => ma.StoreID == storeId
                    && ma.BuyerID == BuyerID
                    && ma.BuyerOrderID == BuyerOrderID
                    && ma.ItemTypeID == ItemTypeID
                    && ma.LeatherStatusID == LeatherStatusID
                    && ma.ArticleID == ArticleID
                    && ma.ArticleChallanID == ArticleChallanID
                    && ma.ColorID == Entity.ColorID
                    && ma.FinishQCLabel == "BQF"
                    && ma.GradeID == Entity.GradeID).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockArea;
            }
            Model.IssuePcs = Entity.IssuePcs;
            Model.IssueSide = Entity.IssueSide;
            Model.IssueArea = Entity.IssueArea;
            Model.AreaUnit = Entity.AreaUnit;
            Model.AreaUnitName = Entity.AreaUnit == null ? "SFT" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;
            Model.CrustQCLabel = Entity.FinishQCLabel;

            return Model;
        }

        public ValidationMsg ConfirmedAfterCrustQC(InvCrustLeatherIssue model, int userid)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var originalEntityYearMonth = _context.INV_FinishLeatherIssue.First(m => m.FinishLeatherIssueID == model.CrustLeatherIssueID);
                        originalEntityYearMonth.RecordStatus = "CNF";
                        originalEntityYearMonth.ModifiedBy = userid;
                        originalEntityYearMonth.ModifiedOn = DateTime.Now;

                        #region Update Store Records

                        if (model.IssueItemList != null)
                        {
                            foreach (InvCrustLeatherIssueItem objQCItem in model.IssueItemList)
                            {
                                if (model.IssueColorList != null)
                                {
                                    foreach (InvCrustLeatherIssueColor objQCSelection in model.IssueColorList)
                                    {
                                        var currentDate = DateTime.Now.Date;
                                        if (objQCSelection.ClosingStockPcs >= objQCSelection.IssuePcs)
                                        {
                                            //if (model.IssueCategory == "ABQC")
                                            //{
                                            #region Issue From Buyer QC Store

                                            var CheckItemStock = (from ds in _context.INV_FinishBuyerStock
                                                                  where ds.StoreID == model.IssueFrom
                                                                        && ds.BuyerID == objQCItem.BuyerID
                                                                        && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                        && ds.ArticleID == objQCItem.ArticleID
                                                                        && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                        && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                        && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                        && ds.ColorID == objQCSelection.ColorID
                                                                        && ds.GradeID == objQCSelection.GradeID
                                                                        && ds.FinishQCLabel == model.IssueCategory
                                                                  select ds).Any();

                                            if (!CheckItemStock)
                                            {
                                                var PreviousRecord =
                                                    (from ds in _context.INV_FinishBuyerStock
                                                     where ds.StoreID == model.IssueFrom
                                                           && ds.BuyerID == objQCItem.BuyerID
                                                           && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                           && ds.ArticleID == objQCItem.ArticleID
                                                           && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                           && ds.ItemTypeID == objQCItem.ItemTypeID
                                                           && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                           && ds.ColorID == objQCSelection.ColorID
                                                           && ds.GradeID == objQCSelection.GradeID
                                                           && ds.FinishQCLabel == model.IssueCategory
                                                     orderby ds.TransectionID descending
                                                     select ds).FirstOrDefault();

                                                var objStockItem = new INV_FinishBuyerStock();

                                                objStockItem.BuyerID = objQCItem.BuyerID;
                                                objStockItem.BuyerOrderID = objQCItem.BuyerOrderID;
                                                objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                objStockItem.StoreID = Convert.ToByte(model.IssueFrom);
                                                objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);

                                                objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                                objStockItem.GradeID = objQCSelection.GradeID;

                                                if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                objStockItem.FinishQCLabel = objQCSelection.CrustQCLabel;
                                                objStockItem.OpeningStockPcs = PreviousRecord.ClosingStockPcs;
                                                objStockItem.ReceiveStockPcs = 0;
                                                objStockItem.IssueStockPcs = objQCSelection.IssuePcs ?? 0;
                                                objStockItem.ClosingStockPcs = PreviousRecord.ClosingStockPcs - (objQCSelection.IssuePcs ?? 0);

                                                objStockItem.OpeningStockSide = PreviousRecord.ClosingStockSide;
                                                objStockItem.ReceiveStockSide = 0;
                                                objStockItem.IssueStockSide = objQCSelection.IssueSide ?? 0;
                                                objStockItem.ClosingStockSide = PreviousRecord.ClosingStockSide - (objQCSelection.IssueSide ?? 0);

                                                objStockItem.OpeningStockArea = PreviousRecord.ClosingStockArea;
                                                objStockItem.ReceiveStockArea = 0;
                                                objStockItem.IssueStockArea = objQCSelection.IssueArea;
                                                objStockItem.ClosingStockArea = PreviousRecord.ClosingStockArea - (objQCSelection.IssueArea ?? 0);

                                                _context.INV_FinishBuyerStock.Add(objStockItem);
                                            }
                                            else
                                            {
                                                var LastItemInfo = (from ds in _context.INV_FinishBuyerStock
                                                                    where ds.StoreID == model.IssueFrom
                                                                          && ds.BuyerID == objQCItem.BuyerID
                                                                          && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                          && ds.ArticleID == objQCItem.ArticleID
                                                                          && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                          && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                          && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                          && ds.ColorID == objQCSelection.ColorID
                                                                          && ds.GradeID == objQCSelection.GradeID
                                                                    orderby ds.TransectionID descending
                                                                    select ds).FirstOrDefault();

                                                var objStockItem = new INV_FinishBuyerStock();

                                                objStockItem.BuyerID = objQCItem.BuyerID;
                                                objStockItem.BuyerOrderID = objQCItem.BuyerOrderID;
                                                objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                objStockItem.StoreID = Convert.ToByte(model.IssueFrom);
                                                objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                                objStockItem.GradeID = objQCSelection.GradeID;
                                                if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                objStockItem.FinishQCLabel = objQCSelection.CrustQCLabel;

                                                objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
                                                objStockItem.ReceiveStockPcs = 0;
                                                objStockItem.IssueStockPcs = objQCSelection.IssuePcs ?? 0;
                                                objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs - (objQCSelection.IssuePcs ?? 0);

                                                objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
                                                objStockItem.ReceiveStockSide = 0;
                                                objStockItem.IssueStockSide = objQCSelection.IssueSide ?? 0;
                                                objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide - (objQCSelection.IssueSide ?? 0);

                                                objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea;
                                                objStockItem.ReceiveStockArea = 0;
                                                objStockItem.IssueStockArea = objQCSelection.IssueArea ?? 0;
                                                objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea - (objQCSelection.IssueArea ?? 0);

                                                _context.INV_FinishBuyerStock.Add(objStockItem);
                                            }

                                            #endregion
                                            //}
                                            //else if (model.IssueCategory == "AOQC")
                                            //{
                                            //    #region Issue From Own QC Store

                                            //    var CheckownItemStock = (from ds in _context.INV_FinishOwnQCStock.AsEnumerable()
                                            //                             where ds.StoreID == model.IssueFrom
                                            //                                   && ds.BuyerID == objQCItem.BuyerID
                                            //                                   && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                            //                                   && ds.ArticleID == objQCItem.ArticleID
                                            //                                   && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                            //                                   && ds.ItemTypeID == objQCItem.ItemTypeID
                                            //                                   && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                            //                                   && ds.ColorID == objQCSelection.ColorID
                                            //                                   && ds.GradeID == objQCSelection.GradeID
                                            //                             select ds).Any();

                                            //    if (!CheckownItemStock)
                                            //    {
                                            //        var PreviousRecord =
                                            //            (from ds in _context.INV_FinishOwnQCStock.AsEnumerable()
                                            //             where ds.StoreID == model.IssueFrom
                                            //                   && ds.BuyerID == objQCItem.BuyerID
                                            //                   && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                            //                   && ds.ArticleID == objQCItem.ArticleID
                                            //                   && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                            //                   && ds.ItemTypeID == objQCItem.ItemTypeID
                                            //                   && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                            //                   && ds.ColorID == objQCSelection.ColorID
                                            //                   && ds.GradeID == objQCSelection.GradeID
                                            //             select ds).LastOrDefault();

                                            //        var objStockItem = new INV_FinishBuyerQCStock();

                                            //        objStockItem.BuyerID = objQCItem.BuyerID;
                                            //        objStockItem.BuyerOrderID = objQCItem.BuyerOrderID;
                                            //        objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                            //        objStockItem.StoreID = Convert.ToByte(model.IssueFrom);
                                            //        objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                            //        objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                            //        objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);

                                            //        objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                            //        objStockItem.GradeID = objQCSelection.GradeID;

                                            //        if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                            //            objStockItem.ClosingStockAreaUnit = null;
                                            //        else
                                            //            objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                            //        objStockItem.FinishQCLabel = objQCSelection.CrustQCLabel;
                                            //        objStockItem.OpeningStockPcs = PreviousRecord.ClosingStockPcs;
                                            //        objStockItem.ReceiveStockPcs = 0;
                                            //        objStockItem.IssueStockPcs = objQCSelection.IssuePcs ?? 0;
                                            //        objStockItem.ClosingStockPcs = PreviousRecord.ClosingStockPcs - objQCSelection.IssuePcs ?? 0;

                                            //        objStockItem.OpeningStockSide = PreviousRecord.ClosingStockSide;
                                            //        objStockItem.ReceiveStockSide = 0;
                                            //        objStockItem.IssueStockSide = objQCSelection.IssueSide ?? 0;
                                            //        objStockItem.ClosingStockSide = PreviousRecord.ClosingStockSide - objQCSelection.IssueSide ?? 0;

                                            //        objStockItem.OpeningStockArea = PreviousRecord.ClosingStockArea;
                                            //        objStockItem.ReceiveStockArea = 0;
                                            //        objStockItem.IssueStockArea = objQCSelection.IssueArea ?? 0;
                                            //        objStockItem.ClosingStockArea = PreviousRecord.ClosingStockArea - objQCSelection.IssueArea ?? 0;

                                            //        _context.INV_FinishBuyerQCStock.Add(objStockItem);
                                            //    }
                                            //    else
                                            //    {
                                            //        var LastItemInfo = (from ds in _context.INV_FinishOwnQCStock
                                            //                            where ds.StoreID == model.IssueFrom
                                            //                                  && ds.BuyerID == objQCItem.BuyerID
                                            //                                  && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                            //                                  && ds.ArticleID == objQCItem.ArticleID
                                            //                                  && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                            //                                  && ds.ItemTypeID == objQCItem.ItemTypeID
                                            //                                  && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                            //                                  && ds.ColorID == objQCSelection.ColorID
                                            //                                  && ds.GradeID == objQCSelection.GradeID
                                            //                            orderby ds.TransectionID descending
                                            //                            select ds).FirstOrDefault();

                                            //        var objStockItem = new INV_FinishOwnQCStock();

                                            //        objStockItem.BuyerID = objQCItem.BuyerID;
                                            //        objStockItem.BuyerOrderID = objQCItem.BuyerOrderID;
                                            //        objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                            //        objStockItem.StoreID = Convert.ToByte(model.IssueFrom);
                                            //        objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                            //        objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                            //        objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                            //        objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                            //        objStockItem.GradeID = objQCSelection.GradeID;
                                            //        if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                            //            objStockItem.ClosingStockAreaUnit = null;
                                            //        else
                                            //            objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                            //        objStockItem.FinishQCLabel = objQCSelection.CrustQCLabel;

                                            //        objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
                                            //        objStockItem.ReceiveStockPcs = 0;
                                            //        objStockItem.IssueStockPcs = objQCSelection.IssuePcs ?? 0;
                                            //        objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs - objQCSelection.IssuePcs ?? 0;

                                            //        objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
                                            //        objStockItem.ReceiveStockSide = 0;
                                            //        objStockItem.IssueStockSide = objQCSelection.IssueSide ?? 0;
                                            //        objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide - objQCSelection.IssueSide ?? 0;

                                            //        objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea;
                                            //        objStockItem.ReceiveStockArea = 0;
                                            //        objStockItem.IssueStockArea = objQCSelection.IssueArea ?? 0;
                                            //        objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea - objQCSelection.IssueArea ?? 0;

                                            //        _context.INV_FinishOwnQCStock.Add(objStockItem);
                                            //    }

                                            //    #endregion
                                            //}
                                            if (model.IssueFor == "Finish")
                                            {
                                                #region Receive

                                                #region Daily Stock

                                                var CheckDate2 = (from ds in _context.INV_FinishStockDaily
                                                                  where ds.StockDate == currentDate
                                                                        && ds.StoreID == model.IssueTo
                                                                        && ds.ArticleID == objQCItem.ArticleID
                                                                        && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                        && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                        && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                        && ds.ColorID == objQCSelection.ColorID
                                                                        && ds.GradeID == objQCSelection.GradeID
                                                                  select ds).Any();

                                                if (CheckDate2)
                                                {
                                                    var CurrentItem =
                                                        (from ds in _context.INV_FinishStockDaily
                                                         where ds.StockDate == currentDate
                                                               && ds.StoreID == model.IssueTo
                                                               && ds.ArticleID == objQCItem.ArticleID
                                                               && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                               && ds.ItemTypeID == objQCItem.ItemTypeID
                                                               && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                               && ds.ColorID == objQCSelection.ColorID
                                                               && ds.GradeID == objQCSelection.GradeID
                                                         select ds).FirstOrDefault();

                                                    CurrentItem.IssueStockPcs = CurrentItem.ReceiveStockPcs + objQCSelection.IssuePcs ?? 0;
                                                    CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockPcs + objQCSelection.IssuePcs ?? 0;

                                                    CurrentItem.IssueStockSide = CurrentItem.ReceiveStockSide + objQCSelection.IssueSide ?? 0;
                                                    CurrentItem.ClosingStockSide = CurrentItem.ClosingStockSide + objQCSelection.IssueSide ?? 0;

                                                    CurrentItem.IssueStockArea = CurrentItem.ReceiveStockArea + objQCSelection.IssueArea ?? 0;
                                                    CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockArea + objQCSelection.IssueArea ?? 0;
                                                }
                                                else
                                                {
                                                    var PreviousRecord =
                                                        (from ds in _context.INV_FinishStockDaily
                                                         where ds.StoreID == model.IssueTo
                                                               && ds.ArticleID == objQCItem.ArticleID
                                                               && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                               && ds.ItemTypeID == objQCItem.ItemTypeID
                                                               && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                               && ds.ColorID == objQCSelection.ColorID
                                                               && ds.GradeID == objQCSelection.GradeID
                                                         orderby ds.TransectionID descending
                                                         select ds).FirstOrDefault();

                                                    var objStockDaily = new INV_FinishStockDaily();

                                                    objStockDaily.StockDate = currentDate;

                                                    objStockDaily.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockDaily.ItemTypeID = objQCItem.ItemTypeID;
                                                    objStockDaily.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                    objStockDaily.LeatherStatusID = objQCItem.LeatherStatusID;
                                                    objStockDaily.ArticleID = objQCItem.ArticleID;
                                                    objStockDaily.ColorID = objQCSelection.ColorID;
                                                    objStockDaily.GradeID = objQCSelection.GradeID;
                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockDaily.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockDaily.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                    objStockDaily.FinishQCLabel = objQCSelection.CrustQCLabel;
                                                    objStockDaily.OpeningStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs);
                                                    objStockDaily.ReceiveStockPcs = objQCSelection.IssuePcs ?? 0;
                                                    objStockDaily.IssueStockPcs = 0;
                                                    objStockDaily.ClosingStockPcs = objStockDaily.OpeningStockPcs + objQCSelection.IssuePcs ?? 0;

                                                    objStockDaily.OpeningStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide);
                                                    objStockDaily.ReceiveStockSide = objQCSelection.IssueSide ?? 0;
                                                    objStockDaily.IssueStockSide = 0;
                                                    objStockDaily.ClosingStockSide = objStockDaily.OpeningStockSide + objQCSelection.IssueSide ?? 0;

                                                    objStockDaily.OpeningStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea);
                                                    objStockDaily.ReceiveStockArea = objQCSelection.IssueArea ?? 0;
                                                    objStockDaily.IssueStockArea = 0;
                                                    objStockDaily.ClosingStockArea = objStockDaily.OpeningStockArea + objQCSelection.IssueArea ?? 0;

                                                    _context.INV_FinishStockDaily.Add(objStockDaily);
                                                }

                                                #endregion

                                                #region Buyer Stock

                                                var CheckBuyerStock2 =
                                                    (from ds in _context.INV_FinishBuyerStock
                                                     where ds.BuyerID == objQCItem.BuyerID
                                                           && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                           && ds.StoreID == model.IssueTo
                                                           && ds.ArticleID == objQCItem.ArticleID
                                                           && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                           && ds.ItemTypeID == objQCItem.ItemTypeID
                                                           && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                           && ds.ColorID == objQCSelection.ColorID
                                                           && ds.GradeID == objQCSelection.GradeID
                                                     select ds).Any();

                                                if (!CheckBuyerStock2)
                                                {
                                                    var PreviousRecord =
                                                        (from ds in _context.INV_FinishBuyerStock
                                                         where ds.BuyerID == objQCItem.BuyerID
                                                               && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                               && ds.StoreID == model.IssueTo
                                                               && ds.ArticleID == objQCItem.ArticleID
                                                               && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                               && ds.ItemTypeID == objQCItem.ItemTypeID
                                                               && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                               && ds.ColorID == objQCSelection.ColorID
                                                               && ds.GradeID == objQCSelection.GradeID
                                                         orderby ds.TransectionID descending
                                                         select ds).FirstOrDefault();

                                                    var objStockSupplier = new INV_FinishBuyerStock();

                                                    objStockSupplier.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                    objStockSupplier.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                    objStockSupplier.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockSupplier.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                    objStockSupplier.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                    objStockSupplier.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                    objStockSupplier.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                    objStockSupplier.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                                    objStockSupplier.GradeID = objQCSelection.GradeID;

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockSupplier.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockSupplier.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                    objStockSupplier.FinishQCLabel = objQCSelection.CrustQCLabel;

                                                    objStockSupplier.OpeningStockPcs = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs;
                                                    objStockSupplier.ReceiveStockPcs = objQCSelection.IssuePcs ?? 0;
                                                    objStockSupplier.IssueStockPcs = 0;
                                                    objStockSupplier.ClosingStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs) + objQCSelection.IssuePcs ?? 0;
                                                    objStockSupplier.OpeningStockSide = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide;
                                                    objStockSupplier.ReceiveStockSide = objQCSelection.IssueSide ?? 0;
                                                    objStockSupplier.IssueStockSide = 0;
                                                    objStockSupplier.ClosingStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide) + objQCSelection.IssueSide ?? 0;

                                                    objStockSupplier.OpeningStockArea = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea;
                                                    objStockSupplier.ReceiveStockArea = objQCSelection.IssueArea ?? 0;
                                                    objStockSupplier.IssueStockArea = 0;
                                                    objStockSupplier.ClosingStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea) + objQCSelection.IssueArea ?? 0;

                                                    _context.INV_FinishBuyerStock.Add(objStockSupplier);
                                                }
                                                else
                                                {
                                                    var LastBuyerStock =
                                                        (from ds in _context.INV_FinishBuyerStock
                                                         where ds.BuyerID == objQCItem.BuyerID
                                                               && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                               && ds.StoreID == model.IssueTo
                                                               && ds.ArticleID == objQCItem.ArticleID
                                                               && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                               && ds.ItemTypeID == objQCItem.ItemTypeID
                                                               && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                               && ds.ColorID == objQCSelection.ColorID
                                                               && ds.GradeID == objQCSelection.GradeID
                                                         orderby ds.TransectionID descending
                                                         select ds).FirstOrDefault();


                                                    var objStockSupplier = new INV_FinishBuyerStock();

                                                    objStockSupplier.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                    objStockSupplier.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                    objStockSupplier.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockSupplier.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                    objStockSupplier.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                    objStockSupplier.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                    objStockSupplier.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                    objStockSupplier.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                                    objStockSupplier.GradeID = objQCSelection.GradeID;

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockSupplier.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockSupplier.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                    objStockSupplier.FinishQCLabel = objQCSelection.CrustQCLabel;

                                                    objStockSupplier.OpeningStockPcs = LastBuyerStock.ClosingStockPcs;
                                                    objStockSupplier.ReceiveStockPcs = objQCSelection.IssuePcs ?? 0;
                                                    objStockSupplier.IssueStockPcs = 0;
                                                    objStockSupplier.ClosingStockPcs = LastBuyerStock.ClosingStockPcs + objQCSelection.IssuePcs ?? 0;

                                                    objStockSupplier.OpeningStockSide = LastBuyerStock.ClosingStockSide;
                                                    objStockSupplier.ReceiveStockSide = objQCSelection.IssueSide ?? 0;
                                                    objStockSupplier.IssueStockSide = 0;
                                                    objStockSupplier.ClosingStockSide = LastBuyerStock.ClosingStockSide + objQCSelection.IssueSide ?? 0;

                                                    objStockSupplier.OpeningStockArea = LastBuyerStock.ClosingStockArea;
                                                    objStockSupplier.ReceiveStockArea = objQCSelection.IssueArea ?? 0;
                                                    objStockSupplier.IssueStockArea = 0;
                                                    objStockSupplier.ClosingStockArea = LastBuyerStock.ClosingStockArea + objQCSelection.IssueArea ?? 0;

                                                    _context.INV_FinishBuyerStock.Add(objStockSupplier);
                                                }

                                                #endregion

                                                #region Item Stock

                                                var CheckItemStock2 =
                                                    (from ds in _context.INV_FinishStockItem
                                                     where ds.StoreID == model.IssueTo
                                                           && ds.ArticleID == objQCItem.ArticleID
                                                           && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                           && ds.ItemTypeID == objQCItem.ItemTypeID
                                                           && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                           && ds.ColorID == objQCSelection.ColorID
                                                           && ds.GradeID == objQCSelection.GradeID
                                                     select ds).Any();

                                                if (!CheckItemStock2)
                                                {
                                                    var PreviousRecord =
                                                        (from ds in _context.INV_FinishStockItem
                                                         where ds.StoreID == model.IssueTo
                                                               && ds.ArticleID == objQCItem.ArticleID
                                                               && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                               && ds.ItemTypeID == objQCItem.ItemTypeID
                                                               && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                               && ds.ColorID == objQCSelection.ColorID
                                                               && ds.GradeID == objQCSelection.GradeID
                                                         orderby ds.TransectionID descending
                                                         select ds).FirstOrDefault();

                                                    var objStockItem = new INV_FinishStockItem();

                                                    objStockItem.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                    objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                    objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                    objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                    objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                                    objStockItem.GradeID = objQCSelection.GradeID;

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockItem.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;

                                                    objStockItem.FinishQCLabel = objQCSelection.CrustQCLabel;

                                                    objStockItem.OpeningStockPcs = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs;
                                                    objStockItem.ReceiveStockPcs = objQCSelection.IssuePcs ?? 0;
                                                    objStockItem.IssueStockPcs = 0;
                                                    objStockItem.ClosingStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs) + objQCSelection.IssuePcs;

                                                    objStockItem.OpeningStockSide = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide;
                                                    objStockItem.ReceiveStockSide = objQCSelection.IssueSide ?? 0;
                                                    objStockItem.IssueStockSide = 0;
                                                    objStockItem.ClosingStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide) + objQCSelection.IssueSide ?? 0;

                                                    objStockItem.OpeningStockArea = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea;
                                                    objStockItem.ReceiveStockArea = objQCSelection.IssueArea ?? 0;
                                                    objStockItem.IssueStockArea = 0;
                                                    objStockItem.ClosingStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea) + objQCSelection.IssueArea ?? 0;

                                                    _context.INV_FinishStockItem.Add(objStockItem);
                                                }
                                                else
                                                {
                                                    var LastItemInfo = (from ds in _context.INV_FinishStockItem
                                                                        where ds.StoreID == model.IssueTo
                                                                              && ds.ArticleID == objQCItem.ArticleID
                                                                              && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                              && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                              && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                              && ds.ColorID == objQCSelection.ColorID
                                                                              && ds.GradeID == objQCSelection.GradeID
                                                                        orderby ds.TransectionID descending
                                                                        select ds).FirstOrDefault();

                                                    var objStockItem = new INV_FinishStockItem();

                                                    objStockItem.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                    objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                    objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                    objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                    objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                                    objStockItem.GradeID = objQCSelection.GradeID;

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockItem.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;

                                                    objStockItem.FinishQCLabel = objQCSelection.CrustQCLabel;

                                                    objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
                                                    objStockItem.ReceiveStockPcs = objQCSelection.IssuePcs ?? 0;
                                                    objStockItem.IssueStockPcs = 0;
                                                    objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs + objQCSelection.IssuePcs ?? 0;

                                                    objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
                                                    objStockItem.ReceiveStockSide = objQCSelection.IssueSide;
                                                    objStockItem.IssueStockSide = 0;
                                                    objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide + objQCSelection.IssueSide ?? 0;

                                                    objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea;
                                                    objStockItem.ReceiveStockArea = objQCSelection.IssueArea ?? 0;
                                                    objStockItem.IssueStockArea = 0;
                                                    objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea + objQCSelection.IssueArea ?? 0;

                                                    _context.INV_FinishStockItem.Add(objStockItem);
                                                }

                                                #endregion

                                                #endregion
                                            }
                                            else if (model.IssueFor == "Buyer QC")
                                            {
                                                #region Receive QC PASS Stock

                                                var CheckItemStock1 = (from i in _context.INV_FinishBuyerQCStock
                                                                       where i.StoreID == model.IssueTo
                                                                             && i.BuyerID == objQCItem.BuyerID
                                                                             && i.BuyerOrderID == objQCItem.BuyerOrderID
                                                                             && i.ArticleID == objQCItem.ArticleID
                                                                             && i.ArticleChallanID == objQCItem.ArticleChallanID
                                                                             && i.ItemTypeID == objQCItem.ItemTypeID
                                                                             && i.LeatherStatusID == objQCItem.LeatherStatusID
                                                                             && i.ColorID == objQCSelection.ColorID
                                                                             && i.FinishQCLabel == "PASS"
                                                                       select i).Any();
                                                if (!CheckItemStock1)
                                                {
                                                    INV_FinishBuyerQCStock objStockItem = new INV_FinishBuyerQCStock();

                                                    objStockItem.StoreID = Convert.ToByte(model.IssueTo);

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
                                                    objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);

                                                    objStockItem.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                    objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                    objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                    objStockItem.GradeRange = objQCSelection.GradeRange;

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockItem.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;

                                                    objStockItem.FinishQCLabel = "PASS";

                                                    objStockItem.OpeningStockPcs = 0;
                                                    objStockItem.ReceiveStockPcs = objQCSelection.IssuePcs == null ? 0 : objQCSelection.IssuePcs;
                                                    objStockItem.IssueStockPcs = 0;
                                                    objStockItem.ClosingStockPcs = objQCSelection.IssuePcs == null ? 0 : objQCSelection.IssuePcs;

                                                    objStockItem.OpeningStockSide = 0;
                                                    objStockItem.ReceiveStockSide = objQCSelection.IssueSide == null ? 0 : objQCSelection.IssueSide;
                                                    objStockItem.IssueStockSide = 0;
                                                    objStockItem.ClosingStockSide = objQCSelection.IssueSide == null ? 0 : objQCSelection.IssueSide;

                                                    objStockItem.OpeningStockArea = 0;
                                                    objStockItem.ReceiveStockArea = objQCSelection.IssueArea == null ? 0 : objQCSelection.IssueArea;
                                                    objStockItem.IssueStockArea = 0;
                                                    objStockItem.ClosingStockArea = objQCSelection.IssueArea == null ? 0 : objQCSelection.IssueArea;

                                                    _context.INV_FinishBuyerQCStock.Add(objStockItem);
                                                    _context.SaveChanges();
                                                }
                                                else
                                                {
                                                    var LastItemInfo = (from i in _context.INV_FinishBuyerQCStock
                                                                        where i.StoreID == model.IssueTo
                                                                              && i.BuyerID == objQCItem.BuyerID
                                                                              && i.BuyerOrderID == objQCItem.BuyerOrderID
                                                                              && i.ArticleID == objQCItem.ArticleID
                                                                              && i.ArticleChallanID == objQCItem.ArticleChallanID
                                                                              && i.ItemTypeID == objQCItem.ItemTypeID
                                                                              && i.LeatherStatusID == objQCItem.LeatherStatusID
                                                                              && i.ColorID == objQCSelection.ColorID
                                                                              && i.FinishQCLabel == "PASS"
                                                                        orderby i.TransectionID descending
                                                                        select i).FirstOrDefault();

                                                    INV_FinishBuyerQCStock objStockItem = new INV_FinishBuyerQCStock();

                                                    objStockItem.StoreID = Convert.ToByte(model.IssueTo);

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
                                                    objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);

                                                    objStockItem.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                    objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                    objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                    objStockItem.GradeRange = objQCSelection.GradeRange;

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockItem.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;

                                                    objStockItem.FinishQCLabel = "PASS";

                                                    objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
                                                    objStockItem.ReceiveStockPcs = objQCSelection.IssuePcs == null ? 0 : objQCSelection.IssuePcs;
                                                    objStockItem.IssueStockPcs = 0;
                                                    objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs + objQCSelection.IssuePcs == null ? 0 : objQCSelection.IssuePcs;

                                                    objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
                                                    objStockItem.ReceiveStockSide = objQCSelection.IssueSide == null ? 0 : objQCSelection.IssueSide;
                                                    objStockItem.IssueStockSide = 0;
                                                    objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide + objQCSelection.IssueSide == null ? 0 : objQCSelection.IssueSide;

                                                    objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea;
                                                    objStockItem.ReceiveStockArea = objQCSelection.IssueArea == null ? 0 : objQCSelection.IssueArea;
                                                    objStockItem.IssueStockArea = 0;
                                                    objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea + objQCSelection.IssueArea == null ? 0 : objQCSelection.IssueArea;

                                                    _context.INV_FinishBuyerQCStock.Add(objStockItem);
                                                    _context.SaveChanges();
                                                }

                                                #endregion
                                            }
                                            else if (model.IssueFor == "Packing")
                                            {
                                                //var currentDate = DateTime.Now.Date;

                                                #region Receive QC PASS Stock

                                                var CheckDate2 = (from ds in _context.INV_FinishPackingStock
                                                                  where ds.StockDate == currentDate
                                                                              && ds.BuyerID == objQCItem.BuyerID
                                                                              && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                              && ds.StoreID == model.IssueTo
                                                                              && ds.ArticleID == objQCItem.ArticleID
                                                                              && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                              && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                              && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                              && ds.ColorID == objQCSelection.ColorID
                                                                              && ds.GradeID == objQCSelection.GradeID
                                                                  select ds).Any();

                                                if (CheckDate2)
                                                {
                                                    var CurrentItem = (from ds in _context.INV_FinishPackingStock
                                                                       where ds.StockDate == currentDate
                                                                             && ds.BuyerID == objQCItem.BuyerID
                                                                              && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                              && ds.StoreID == model.IssueTo
                                                                              && ds.ArticleID == objQCItem.ArticleID
                                                                              && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                              && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                              && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                              && ds.ColorID == objQCSelection.ColorID
                                                                              && ds.GradeID == objQCSelection.GradeID
                                                                       select ds).FirstOrDefault();

                                                    CurrentItem.IssueStockPcs = CurrentItem.ReceiveStockPcs + (objQCSelection.IssuePcs ?? 0);
                                                    CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockPcs + (objQCSelection.IssuePcs ?? 0);

                                                    CurrentItem.IssueStockSide = CurrentItem.ReceiveStockSide + (objQCSelection.IssueSide ?? 0);
                                                    CurrentItem.ClosingStockSide = CurrentItem.ClosingStockSide + (objQCSelection.IssueSide ?? 0);

                                                    CurrentItem.IssueStockArea = CurrentItem.ReceiveStockArea + (objQCSelection.IssueArea ?? 0);
                                                    CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockArea + (objQCSelection.IssueArea ?? 0);
                                                    CurrentItem.FinishQCLabel = "BQP";//Buyer QC Pass
                                                    _context.SaveChanges();
                                                }
                                                else
                                                {
                                                    var PreviousRecord = (from ds in _context.INV_FinishPackingStock
                                                                          where ds.StoreID == model.IssueTo
                                                                                && ds.BuyerID == objQCItem.BuyerID
                                                                              && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                              && ds.ArticleID == objQCItem.ArticleID
                                                                              && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                              && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                              && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                              && ds.ColorID == objQCSelection.ColorID
                                                                              && ds.GradeID == objQCSelection.GradeID
                                                                          orderby ds.TransectionID descending
                                                                          select ds).FirstOrDefault();

                                                    var objStockDaily = new INV_FinishPackingStock();

                                                    objStockDaily.StockDate = currentDate;

                                                    if (objQCItem.BuyerID == null)
                                                        objStockDaily.BuyerID = null;
                                                    else
                                                        objStockDaily.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                    if (objQCItem.BuyerOrderID == null)
                                                        objStockDaily.BuyerOrderID = null;
                                                    else
                                                        objStockDaily.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);

                                                    //objStockDaily.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                    //objStockDaily.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);

                                                    objStockDaily.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockDaily.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                    objStockDaily.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                    objStockDaily.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                    objStockDaily.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                    objStockDaily.ArticleChallanID = Convert.ToInt64(objQCItem.ArticleChallanID);
                                                    objStockDaily.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                    objStockDaily.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                    objStockDaily.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                                    objStockDaily.GradeID = objQCSelection.GradeID;

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockDaily.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockDaily.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;

                                                    objStockDaily.OpeningStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs);
                                                    objStockDaily.ReceiveStockPcs = objQCSelection.IssuePcs ?? 0;
                                                    objStockDaily.IssueStockPcs = 0;
                                                    objStockDaily.ClosingStockPcs = objStockDaily.OpeningStockPcs + objQCSelection.IssuePcs ?? 0;

                                                    objStockDaily.OpeningStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide);
                                                    objStockDaily.ReceiveStockSide = objQCSelection.IssuePcs ?? 0;
                                                    objStockDaily.IssueStockSide = 0;
                                                    objStockDaily.ClosingStockSide = objStockDaily.OpeningStockSide + objQCSelection.IssuePcs ?? 0;

                                                    objStockDaily.OpeningStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea);
                                                    objStockDaily.ReceiveStockArea = objQCSelection.IssueArea ?? 0;
                                                    objStockDaily.IssueStockArea = 0;
                                                    objStockDaily.ClosingStockArea = objStockDaily.OpeningStockArea + objQCSelection.IssueArea ?? 0;

                                                    objStockDaily.FinishQCLabel = "BQP";//Buyer QC Pass
                                                    _context.INV_FinishPackingStock.Add(objStockDaily);
                                                }

                                                #endregion
                                            }
                                        }
                                        else
                                        {
                                            stockError = 1;
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        if (stockError == 0)
                        {
                            _context.SaveChanges();
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Confirmed Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Not Enoung Quantity in Stock.";
                        }
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

        public List<InvCrustLeatherIssue> GetCrustLeatherIssueAfterQCInfo()
        {
            List<INV_FinishLeatherIssue> searchList = _context.INV_FinishLeatherIssue.OrderByDescending(m => m.FinishLeatherIssueID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssue>();
        }

        public InvCrustLeatherIssue SetToBussinessObject(INV_FinishLeatherIssue Entity)
        {
            InvCrustLeatherIssue Model = new InvCrustLeatherIssue();

            Model.CrustLeatherIssueID = Entity.FinishLeatherIssueID;
            Model.CrustLeatherIssueNo = Entity.FinishLeatherIssueNo;
            Model.CrustLeatherIssueDate = string.IsNullOrEmpty(Entity.FinishLeatherIssueDate.ToString()) ? string.Empty : Convert.ToDateTime(Entity.FinishLeatherIssueDate).ToString("dd/MM/yyyy");
            Model.IssueFrom = Entity.IssueFrom;
            Model.IssueFromName = Entity.IssueFrom == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.IssueFrom).FirstOrDefault().StoreName;
            Model.IssueTo = Entity.IssueTo;
            Model.IssueToName = Entity.IssueTo == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.IssueTo).FirstOrDefault().StoreName;
            Model.IssueQCStatus = Entity.IssueQCStatus;
            Model.IssueCategory = Entity.IssueCategory;
            switch (Model.IssueCategory)
            {
                case "AOQC":
                    Model.IssueCategoryName = "Own QC";
                    break;
                case "ABQC":
                    Model.IssueCategoryName = "Buyer QC";
                    break;
                default:
                    Model.IssueCategoryName = "";
                    break;
            }
            Model.IssueFor = Entity.IssueFor;
            switch (Model.IssueFor)
            {
                case "Buyer QC":
                    Model.IssueForName = "Buyer QC";
                    break;
                case "Finish":
                    Model.IssueForName = "Finish Store";
                    break;
                case "Packing":
                    Model.IssueForName = "Packing Store";
                    break;
                default:
                    Model.IssueForName = "";
                    break;
            }
            return Model;
        }

        public ValidationMsg DeletedCrustLeatherIssue(long CrustLeatherIssueID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var scheduleList = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueID == CrustLeatherIssueID).ToList();
                if (scheduleList.Count > 0)
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Child Record Found.";
                }
                else
                {
                    var deleteYearMonthSchedule = _context.INV_CrustLeatherIssue.First(m => m.CrustLeatherIssueID == CrustLeatherIssueID);
                    _context.INV_CrustLeatherIssue.Remove(deleteYearMonthSchedule);
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

        public ValidationMsg DeletedCrustLeatherIssueItem(long QCItemID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var CrustLeatherIssueID = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueItemID == QCItemID).FirstOrDefault().CrustLeatherIssueID;
                var RecordStatus = _context.INV_CrustLeatherIssue.First(m => m.CrustLeatherIssueID == CrustLeatherIssueID).RecordStatus;
                if (RecordStatus != "CNF")
                {
                    var scheduleList = _context.INV_CrustLeatherIssueColor.Where(m => m.CrustLeatherIssueItemID == QCItemID).ToList();
                    if (scheduleList.Count > 0)
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Child Record Found.";
                    }
                    else
                    {
                        var deleteElement = _context.INV_CrustLeatherIssueItem.First(m => m.CrustLeatherIssueItemID == QCItemID);
                        _context.INV_CrustLeatherIssueItem.Remove(deleteElement);
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

        public ValidationMsg DeletedCrustLeatherIssueColor(long QCSelectionID)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                var QCItemID = _context.INV_CrustLeatherIssueColor.Where(m => m.CrustLeatherIssueColorID == QCSelectionID).FirstOrDefault().CrustLeatherIssueItemID;
                var CrustLeatherIssueID = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueItemID == QCItemID).FirstOrDefault().CrustLeatherIssueID;
                var RecordStatus = _context.INV_CrustLeatherIssue.First(m => m.CrustLeatherIssueID == CrustLeatherIssueID).RecordStatus;
                if (RecordStatus != "CNF")
                {
                    var deleteElement = _context.INV_CrustLeatherIssueColor.First(m => m.CrustLeatherIssueColorID == QCSelectionID);
                    _context.INV_CrustLeatherIssueColor.Remove(deleteElement);

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
