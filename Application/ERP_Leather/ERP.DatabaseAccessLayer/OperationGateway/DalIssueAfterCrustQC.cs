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
    public class DalIssueAfterCrustQC
    {
        private readonly BLC_DEVEntities _context;
        private ValidationMsg _vmMsg;
        public long CrustLeatherIssueID = 0;
        private int stockError = 0;
        public string CrustLeatherIssueNo = string.Empty;
        public DalIssueAfterCrustQC()
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

                            INV_CrustLeatherIssue tblQC = SetToModelObject(model, userid);
                            _context.INV_CrustLeatherIssue.Add(tblQC);
                            _context.SaveChanges();

                            #endregion

                            #region Save QCItem Records

                            if (model.IssueItemList != null)
                            {
                                foreach (InvCrustLeatherIssueItem objQCItem in model.IssueItemList)
                                {
                                    objQCItem.CrustLeatherIssueID = tblQC.CrustLeatherIssueID;
                                    INV_CrustLeatherIssueItem tblQCItem = SetToModelObject(objQCItem, userid);
                                    _context.INV_CrustLeatherIssueItem.Add(tblQCItem);
                                    _context.SaveChanges();

                                    #region Save QCSelection List

                                    if (model.IssueColorList != null)
                                    {
                                        foreach (InvCrustLeatherIssueColor objQCSelection in model.IssueColorList)
                                        {
                                            objQCSelection.CrustLeatherIssueItemID = tblQCItem.CrustLeatherIssueItemID;
                                            objQCSelection.CrustLeatherIssueID = tblQC.CrustLeatherIssueID;
                                            INV_CrustLeatherIssueColor tblQCSelection = SetToModelObject(objQCSelection, userid);
                                            _context.INV_CrustLeatherIssueColor.Add(tblQCSelection);
                                        }
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            _context.SaveChanges();
                            tx.Complete();
                            CrustLeatherIssueID = tblQC.CrustLeatherIssueID;
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
                    _vmMsg.Msg = "Schedule Year,Month Combination Data Already Exit.";
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

                        INV_CrustLeatherIssue CurrentEntity = SetToModelObject(model, userid);
                        var OriginalEntity = _context.INV_CrustLeatherIssue.First(m => m.CrustLeatherIssueID == model.CrustLeatherIssueID);

                        OriginalEntity.CrustLeatherIssueDate = CurrentEntity.CrustLeatherIssueDate;
                        OriginalEntity.IssueFrom = CurrentEntity.IssueFrom;
                        OriginalEntity.IssueFor = CurrentEntity.IssueFor;
                        OriginalEntity.IssueTo = CurrentEntity.IssueTo;
                        OriginalEntity.IssueQCStatus = CurrentEntity.IssueQCStatus;

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
                                    INV_CrustLeatherIssueItem tblQCItem = SetToModelObject(objQCItem, userid);
                                    _context.INV_CrustLeatherIssueItem.Add(tblQCItem);
                                    _context.SaveChanges();
                                    objQCItem.CrustLeatherIssueItemID = tblQCItem.CrustLeatherIssueItemID;
                                }
                                else
                                {
                                    INV_CrustLeatherIssueItem CurrEntity = SetToModelObject(objQCItem, userid);
                                    var OrgrEntity = _context.INV_CrustLeatherIssueItem.First(m => m.CrustLeatherIssueItemID == objQCItem.CrustLeatherIssueItemID);

                                    OrgrEntity.BuyerID = CurrEntity.BuyerID;
                                    OrgrEntity.BuyerOrderID = CurrEntity.BuyerOrderID;
                                    OrgrEntity.ArticleID = CurrEntity.ArticleID;
                                    OrgrEntity.ArticleNo = CurrEntity.ArticleNo;
                                    OrgrEntity.ItemTypeID = CurrEntity.ItemTypeID;
                                    OrgrEntity.LeatherTypeID = CurrEntity.LeatherTypeID;
                                    OrgrEntity.LeatherStatusID = CurrEntity.LeatherStatusID;
                                    OrgrEntity.CrustQCLabel = CurrEntity.CrustQCLabel;
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

                                            INV_CrustLeatherIssueColor tblQCSelection = SetToModelObject(objQCSelection, userid);
                                            _context.INV_CrustLeatherIssueColor.Add(tblQCSelection);
                                        }
                                        else
                                        {
                                            INV_CrustLeatherIssueColor CurEntity = SetToModelObject(objQCSelection, userid);
                                            var OrgEntity = _context.INV_CrustLeatherIssueColor.First(m => m.CrustLeatherIssueColorID == objQCSelection.CrustLeatherIssueColorID);

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
                    _vmMsg.Msg = "Schedule Year,Month Combination Data Already Exit.";
                }
            }
            return _vmMsg;
        }

        public INV_CrustLeatherIssue SetToModelObject(InvCrustLeatherIssue model, int userid)
        {
            INV_CrustLeatherIssue Entity = new INV_CrustLeatherIssue();

            Entity.CrustLeatherIssueID = model.CrustLeatherIssueID;
            Entity.CrustLeatherIssueNo = model.CrustLeatherIssueNo;
            Entity.CrustLeatherIssueDate = DalCommon.SetDate(model.CrustLeatherIssueDate);
            Entity.IssueFrom = model.IssueFrom;
            Entity.IssueCategory = "IAQC";//model.IssueCategory;
            Entity.IssueFor = model.IssueFor;
            Entity.IssueTo = model.IssueTo;
            Entity.IssueQCStatus = model.IssueQCStatus;
            Entity.RecordStatus = "NCF";
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public INV_CrustLeatherIssueItem SetToModelObject(InvCrustLeatherIssueItem model, int userid)
        {
            INV_CrustLeatherIssueItem Entity = new INV_CrustLeatherIssueItem();

            Entity.CrustLeatherIssueItemID = model.CrustLeatherIssueItemID;
            Entity.CrustLeatherIssueID = model.CrustLeatherIssueID;
            Entity.BuyerID = model.BuyerID;
            Entity.BuyerOrderID = model.BuyerOrderID;
            Entity.ArticleID = model.ArticleID;
            Entity.ArticleNo = model.ArticleNo;
            Entity.ArticleChallanID = model.ArticleChallanID;
            Entity.ArticleChallanNo = model.ArticleChallanNo;
            Entity.ItemTypeID = model.ItemTypeID;
            Entity.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
            Entity.LeatherStatusID = model.LeatherStatusID;
            Entity.CrustQCLabel = model.CrustQCLabel;
            Entity.SetOn = DateTime.Now;
            Entity.SetBy = userid;
            Entity.IPAddress = GetIPAddress.LocalIPAddress();

            return Entity;
        }

        public INV_CrustLeatherIssueColor SetToModelObject(InvCrustLeatherIssueColor model, int userid)
        {
            INV_CrustLeatherIssueColor Entity = new INV_CrustLeatherIssueColor();

            Entity.CrustLeatherIssueColorID = model.CrustLeatherIssueColorID;
            Entity.CrustLeatherIssueItemID = model.CrustLeatherIssueItemID;
            Entity.CrustLeatherIssueID = model.CrustLeatherIssueID;
            Entity.ColorID = model.ColorID;
            Entity.ArticleColorNo = model.ArticleColorNo;
            //Entity.GradeID = model.GradeID;
            Entity.GradeRange = model.GradeRange;
            Entity.IssuePcs = model.IssuePcs;
            Entity.IssueSide = model.IssueSide;
            Entity.IssueArea = model.IssueArea;
            if (string.IsNullOrEmpty(model.AreaUnitName))
                Entity.AreaUnit = null;
            else
                Entity.AreaUnit = _context.Sys_Unit.Where(m => m.UnitName == model.AreaUnitName).FirstOrDefault().UnitID;
            Entity.CrustQCLabel = model.CrustQCLabel;
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

        public List<InvCrustLeatherIssueItem> GetScheduleItemInfo(string IssueFrom, string CrustQCLabel)
        {
            if (!string.IsNullOrEmpty(IssueFrom))
            {
                if (CrustQCLabel == "ALL")
                {
                    var query = @"select inv.TransectionID,
                            inv.BuyerID,(select BuyerName from dbo.Sys_Buyer where BuyerID = inv.BuyerID)BuyerName,
                            inv.BuyerOrderID,(select BuyerOrderNo from dbo.SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID)BuyerOrderNo,
                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
                            --inv.LeatherTypeID,(select LeatherTypeName from dbo.Sys_LeatherType where LeatherTypeID = inv.LeatherTypeID)LeatherTypeName,
                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,inv.ArticleChallanID,inv.ArticleChallanNo,
                            inv.ClosingStockPcs from dbo.INV_CrustQCStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID from dbo.INV_CrustQCStock
                            where (CrustQCLabel = 'PASS' or CrustQCLabel = 'FAIL') group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID) sup
                            ON inv.TransectionID=sup.TransectionID
                            where inv.StoreID = " + IssueFrom + " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";
                    var allData = _context.Database.SqlQuery<InvCrustLeatherIssueItem>(query).ToList();
                    return allData;
                }
                else
                {
                    var query = @"select inv.TransectionID,
                            inv.BuyerID,(select BuyerName from dbo.Sys_Buyer where BuyerID = inv.BuyerID)BuyerName,
                            inv.BuyerOrderID,(select BuyerOrderNo from dbo.SLS_BuyerOrder where BuyerOrderID = inv.BuyerOrderID)BuyerOrderNo,
                            inv.ItemTypeID,(select ItemTypeName from dbo.Sys_ItemType where ItemTypeID = inv.ItemTypeID)ItemTypeName,
                            --inv.LeatherTypeID,(select LeatherTypeName from dbo.Sys_LeatherType where LeatherTypeID = inv.LeatherTypeID)LeatherTypeName,
                            inv.LeatherStatusID,(select LeatherStatusName from dbo.Sys_LeatherStatus where LeatherStatusID = inv.LeatherStatusID)LeatherStatusName,
                            inv.ArticleID,(select ArticleName from dbo.Sys_Article where ArticleID = inv.ArticleID)ArticleName,inv.ArticleChallanID,inv.ArticleChallanNo,
                            inv.ClosingStockPcs from dbo.INV_CrustQCStock inv
                            INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID from dbo.INV_CrustQCStock
                            where CrustQCLabel = '" + CrustQCLabel + "'" + " group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID) sup " +
                            "ON inv.TransectionID=sup.TransectionID " +
                            "where inv.StoreID = " + IssueFrom + " and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";
                    var allData = _context.Database.SqlQuery<InvCrustLeatherIssueItem>(query).ToList();
                    return allData;
                }
            }
            return null;
        }

        public List<InvCrustLeatherIssueColor> GetColorAndGradeList(string IssueFrom, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ArticleChallanID, string CrustQCLabel)
        {
            if (!string.IsNullOrEmpty(IssueFrom))
            {
                if (CrustQCLabel == "ALL")
                {
                    if (string.IsNullOrEmpty(BuyerID))
                    {
                        var query = @"select inv.TransectionID,
                                inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.GradeRange,
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.CrustQCLabel from dbo.INV_CrustQCStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel from dbo.INV_CrustQCStock
                                group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID IS NULL" +
                                " and inv.BuyerOrderID   IS NULL " + "and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + "and inv.ArticleChallanID = " + ArticleChallanID +
                                " and (inv.CrustQCLabel = 'PASS' or inv.CrustQCLabel = 'FAIL') and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

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
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.CrustQCLabel from dbo.INV_CrustQCStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel from dbo.INV_CrustQCStock
                                group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID = " + BuyerID +
                                " and inv.BuyerOrderID IS NULL and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + "and inv.ArticleChallanID = " + ArticleChallanID +
                                " and (inv.CrustQCLabel = 'PASS' or inv.CrustQCLabel = 'FAIL') and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

                            var allData = _context.Database.SqlQuery<InvCrustLeatherIssueColor>(query).ToList();
                            List<InvCrustLeatherIssueColor> searchList = allData.ToList();
                            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueColor>();
                        }
                        else
                        {
                            var query = @"select inv.TransectionID,
                                inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.GradeRange,
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.CrustQCLabel from dbo.INV_CrustQCStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel from dbo.INV_CrustQCStock
                                group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID = " + BuyerID +
                                " and inv.BuyerOrderID = " + BuyerOrderID + " and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + "and inv.ArticleChallanID = " + ArticleChallanID +
                                " and (inv.CrustQCLabel = 'PASS' or inv.CrustQCLabel = 'FAIL') and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

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
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.CrustQCLabel from dbo.INV_CrustQCStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel from dbo.INV_CrustQCStock
                                group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID IS NULL" +
                                " and inv.BuyerOrderID IS NULL" + " and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + "and inv.ArticleChallanID = " + ArticleChallanID +
                                " and inv.CrustQCLabel = '" + CrustQCLabel + "' and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

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
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.CrustQCLabel from dbo.INV_CrustQCStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel from dbo.INV_CrustQCStock
                                group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID = " + BuyerID +
                                " and inv.BuyerOrderID IS NULL and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + "and inv.ArticleChallanID = " + ArticleChallanID +
                                " and inv.CrustQCLabel = '" + CrustQCLabel + "' and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

                            var allData = _context.Database.SqlQuery<InvCrustLeatherIssueColor>(query).ToList();
                            List<InvCrustLeatherIssueColor> searchList = allData.ToList();
                            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueColor>();
                        }
                        else
                        {
                            var query = @"select inv.TransectionID,
                                inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName,inv.ArticleColorNo,inv.GradeRange,
                                inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea,inv.CrustQCLabel from dbo.INV_CrustQCStock inv
                                INNER JOIN (select MAX(TransectionID)TransectionID,BuyerID,StoreID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel from dbo.INV_CrustQCStock
                                group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherStatusID,ArticleID,ColorID,CrustQCLabel) sup
                                ON inv.TransectionID=sup.TransectionID
                                where inv.StoreID = " + IssueFrom + " and inv.BuyerID = " + BuyerID +
                                " and inv.BuyerOrderID = " + BuyerOrderID + " and inv.ItemTypeID = " + ItemTypeID +
                                " and inv.LeatherStatusID = " + LeatherStatusID + " and inv.ArticleID = " + ArticleID + "and inv.ArticleChallanID = " + ArticleChallanID +
                                " and inv.CrustQCLabel = '" + CrustQCLabel + "' and (inv.ClosingStockPcs>0 or inv.ClosingStockSide>0 or inv.ClosingStockArea>0)";

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

            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;
            Model.ArticleColorNo = Entity.ArticleColorNo;
            Model.GradeRange = Entity.GradeRange;
            //Model.GradeID = Entity.GradeID;
            //Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;
            Model.ClosingStockPcs = Entity.ClosingStockPcs;
            Model.ClosingStockSide = Entity.ClosingStockSide;
            Model.ClosingStockArea = Entity.ClosingStockArea;
            Model.IssuePcs = Entity.ClosingStockPcs;
            Model.IssueSide = Entity.ClosingStockSide;
            Model.IssueArea = Entity.ClosingStockArea;
            Model.CrustQCLabel = Entity.CrustQCLabel;
            Model.AreaUnitName = "SFT";

            return Model;
        }

        public List<InvCrustLeatherIssueItem> GetQCColorList(string CrustLeatherIssueID)
        {
            long? crustLeatherQCID = Convert.ToInt64(CrustLeatherIssueID);
            List<INV_CrustLeatherIssueItem> searchList = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueID == crustLeatherQCID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssueItem>();
        }

        public InvCrustLeatherIssueItem SetToBussinessObject(INV_CrustLeatherIssueItem Entity)
        {
            InvCrustLeatherIssueItem Model = new InvCrustLeatherIssueItem();

            Model.CrustLeatherIssueItemID = Entity.CrustLeatherIssueItemID;
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
            Model.LeatherStatusName = Entity.LeatherStatusID == null ? "" : _context.Sys_LeatherStatus.Where(m => m.LeatherStatusID == Entity.LeatherStatusID).FirstOrDefault().LeatherStatusName;

            return Model;
        }

        public List<InvCrustLeatherIssueColor> GetQCSelectionList(string CrustLeatherIssueItemID, string StoreId)
        {

            long? qcItemID = Convert.ToInt64(CrustLeatherIssueItemID);
            byte storeId = Convert.ToByte(StoreId);
            List<INV_CrustLeatherIssueColor> searchList = _context.INV_CrustLeatherIssueColor.Where(m => m.CrustLeatherIssueItemID == qcItemID).OrderByDescending(m => m.CrustLeatherIssueColorID).ToList();
            return searchList.Select(c => SetToBussinessObject(c, storeId)).ToList<InvCrustLeatherIssueColor>();

        }

        public InvCrustLeatherIssueColor SetToBussinessObject(INV_CrustLeatherIssueColor Entity, byte storeId)
        {
            InvCrustLeatherIssueColor Model = new InvCrustLeatherIssueColor();

            Model.CrustLeatherIssueColorID = Entity.CrustLeatherIssueColorID;
            Model.CrustLeatherIssueItemID = Entity.CrustLeatherIssueItemID;
            Model.CrustLeatherIssueID = Entity.CrustLeatherIssueID;
            Model.ColorID = Entity.ColorID;
            Model.ColorName = Entity.ColorID == null ? "" : _context.Sys_Color.Where(m => m.ColorID == Entity.ColorID).FirstOrDefault().ColorName;

            //Model.GradeID = Entity.GradeID;
            //Model.GradeName = Entity.GradeID == null ? "" : _context.Sys_Grade.Where(m => m.GradeID == Entity.GradeID).FirstOrDefault().GradeName;

            Model.ArticleColorNo = Entity.ArticleColorNo;
            Model.GradeRange = Entity.GradeRange;

            var BuyerID = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueItemID == Entity.CrustLeatherIssueItemID).FirstOrDefault().BuyerID;
            var BuyerOrderID = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueItemID == Entity.CrustLeatherIssueItemID).FirstOrDefault().BuyerOrderID;
            var ItemTypeID = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueItemID == Entity.CrustLeatherIssueItemID).FirstOrDefault().ItemTypeID;
            var LeatherTypeID = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueItemID == Entity.CrustLeatherIssueItemID).FirstOrDefault().LeatherTypeID;
            var LeatherStatusID = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueItemID == Entity.CrustLeatherIssueItemID).FirstOrDefault().LeatherStatusID;
            var ArticleID = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueItemID == Entity.CrustLeatherIssueItemID).FirstOrDefault().ArticleID;

            Model.ClosingStockPcs = _context.INV_CrustQCStock.Where(ma => ma.StoreID == storeId
                && ma.BuyerID == BuyerID
                && ma.BuyerOrderID == BuyerOrderID
                && ma.ItemTypeID == ItemTypeID
                && ma.LeatherTypeID == LeatherTypeID
                && ma.LeatherStatusID == LeatherStatusID
                && ma.ArticleID == ArticleID
                && ma.ColorID == Entity.ColorID
                //&& ma.GradeID == Entity.GradeID
                && ma.CrustQCLabel == Entity.CrustQCLabel).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockPcs;
            Model.ClosingStockSide = _context.INV_CrustQCStock.Where(ma => ma.StoreID == storeId
                && ma.BuyerID == BuyerID
                && ma.BuyerOrderID == BuyerOrderID
                && ma.ItemTypeID == ItemTypeID
                && ma.LeatherTypeID == LeatherTypeID
                && ma.LeatherStatusID == LeatherStatusID
                && ma.ArticleID == ArticleID
                && ma.ColorID == Entity.ColorID
                //&& ma.GradeID == Entity.GradeID
                && ma.CrustQCLabel == Entity.CrustQCLabel).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockSide;
            Model.ClosingStockArea = _context.INV_CrustQCStock.Where(ma => ma.StoreID == storeId
                && ma.BuyerID == BuyerID
                && ma.BuyerOrderID == BuyerOrderID
                && ma.ItemTypeID == ItemTypeID
                && ma.LeatherTypeID == LeatherTypeID
                && ma.LeatherStatusID == LeatherStatusID
                && ma.ArticleID == ArticleID
                && ma.ColorID == Entity.ColorID
                //&& ma.GradeID == Entity.GradeID
                && ma.CrustQCLabel == Entity.CrustQCLabel).OrderByDescending(m => m.TransectionID).FirstOrDefault().ClosingStockArea;
            Model.IssuePcs = Entity.IssuePcs;
            Model.IssueSide = Entity.IssueSide;
            Model.IssueArea = Entity.IssueArea;
            Model.AreaUnit = Entity.AreaUnit;
            Model.AreaUnitName = Entity.AreaUnit == null ? "" : _context.Sys_Unit.Where(m => m.UnitID == Entity.AreaUnit).FirstOrDefault().UnitName;
            Model.CrustQCLabel = Entity.CrustQCLabel;

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
                        var originalEntityYearMonth = _context.INV_CrustLeatherIssue.First(m => m.CrustLeatherIssueID == model.CrustLeatherIssueID);
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

                                        decimal? issuePcs = objQCSelection.IssuePcs == null ? 0 : objQCSelection.IssuePcs;
                                        decimal? issueSide = objQCSelection.IssueSide == null ? 0 : objQCSelection.IssueSide;
                                        decimal? issueArea = objQCSelection.IssueArea == null ? 0 : objQCSelection.IssueArea;

                                        if ((objQCSelection.ClosingStockPcs >= objQCSelection.IssuePcs) && (objQCSelection.ClosingStockSide >= objQCSelection.IssueSide) && (objQCSelection.ClosingStockArea >= objQCSelection.IssueArea))
                                        {
                                            #region Issue From QC Store

                                            var CheckItemStock = (from ds in _context.INV_CrustQCStock
                                                                  where ds.StoreID == model.IssueFrom
                                                                  && ds.BuyerID == objQCItem.BuyerID
                                                                  && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                  && ds.ArticleID == objQCItem.ArticleID
                                                                  && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                  && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                  && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                  && ds.ColorID == objQCSelection.ColorID
                                                                      //&& ds.GradeID == objQCSelection.GradeID
                                                                  && ds.CrustQCLabel == objQCSelection.CrustQCLabel
                                                                  select ds).Any();

                                            if (!CheckItemStock)
                                            {
                                                var PreviousRecord = (from ds in _context.INV_CrustQCStock
                                                                      where ds.StoreID == model.IssueFrom
                                                                      && ds.BuyerID == objQCItem.BuyerID
                                                                      && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                      && ds.ArticleID == objQCItem.ArticleID
                                                                      && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                      && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                      && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                      && ds.ColorID == objQCSelection.ColorID
                                                                          //&& ds.GradeID == objQCSelection.GradeID
                                                                      && ds.CrustQCLabel == objQCSelection.CrustQCLabel
                                                                      orderby ds.TransectionID descending
                                                                      select ds).FirstOrDefault();

                                                var objStockItem = new INV_CrustQCStock();

                                                if (objQCItem.BuyerID == null)
                                                    objStockItem.BuyerID = null;
                                                else
                                                    objStockItem.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                if (objQCItem.BuyerOrderID == null)
                                                    objStockItem.BuyerOrderID = null;
                                                else
                                                    objStockItem.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                objStockItem.StoreID = Convert.ToByte(model.IssueFrom);
                                                objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);

                                                objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                //objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                objStockItem.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                objStockItem.GradeRange = objQCSelection.GradeRange;

                                                //objStockItem.GradeID = objQCSelection.GradeID;

                                                if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                objStockItem.CrustQCLabel = objQCSelection.CrustQCLabel;
                                                objStockItem.OpeningStockPcs = PreviousRecord.ClosingStockPcs;
                                                objStockItem.ReceiveStockPcs = 0;
                                                objStockItem.IssueStockPcs = objQCSelection.IssuePcs;
                                                objStockItem.ClosingStockPcs = PreviousRecord.ClosingStockPcs - objQCSelection.IssuePcs;

                                                objStockItem.OpeningStockSide = PreviousRecord.ClosingStockSide;
                                                objStockItem.ReceiveStockSide = 0;
                                                objStockItem.IssueStockSide = objQCSelection.IssueSide;
                                                objStockItem.ClosingStockSide = PreviousRecord.ClosingStockSide - objQCSelection.IssueSide;

                                                objStockItem.OpeningStockArea = PreviousRecord.ClosingStockArea;
                                                objStockItem.ReceiveStockArea = 0;
                                                objStockItem.IssueStockArea = objQCSelection.IssueArea;
                                                objStockItem.ClosingStockArea = PreviousRecord.ClosingStockArea - objQCSelection.IssueArea;

                                                _context.INV_CrustQCStock.Add(objStockItem);
                                                _context.SaveChanges();
                                            }
                                            else
                                            {
                                                var LastItemInfo = (from ds in _context.INV_CrustQCStock
                                                                    where ds.StoreID == model.IssueFrom
                                                                    && ds.BuyerID == objQCItem.BuyerID
                                                                    && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                    && ds.ArticleID == objQCItem.ArticleID
                                                                    && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                    && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                    && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                    && ds.ColorID == objQCSelection.ColorID
                                                                        //&& ds.GradeID == objQCSelection.GradeID
                                                                    && ds.CrustQCLabel == objQCSelection.CrustQCLabel
                                                                    orderby ds.TransectionID descending
                                                                    select ds).FirstOrDefault();

                                                var objStockItem = new INV_CrustQCStock();

                                                if (objQCItem.BuyerID == null)
                                                    objStockItem.BuyerID = null;
                                                else
                                                    objStockItem.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                if (objQCItem.BuyerOrderID == null)
                                                    objStockItem.BuyerOrderID = null;
                                                else
                                                    objStockItem.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                objStockItem.StoreID = Convert.ToByte(model.IssueFrom);
                                                objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);

                                                objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                objStockItem.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                objStockItem.GradeRange = objQCSelection.GradeRange;

                                                if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                    objStockItem.ClosingStockAreaUnit = null;
                                                else
                                                    objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                objStockItem.CrustQCLabel = objQCSelection.CrustQCLabel;

                                                objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
                                                objStockItem.ReceiveStockPcs = 0;
                                                objStockItem.IssueStockPcs = objQCSelection.IssuePcs;
                                                objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs - objQCSelection.IssuePcs;

                                                objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
                                                objStockItem.ReceiveStockSide = 0;
                                                objStockItem.IssueStockSide = objQCSelection.IssueSide;
                                                objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide - objQCSelection.IssueSide;

                                                objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea;
                                                objStockItem.ReceiveStockArea = 0;
                                                objStockItem.IssueStockArea = objQCSelection.IssueArea;
                                                objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea - objQCSelection.IssueArea;

                                                _context.INV_CrustQCStock.Add(objStockItem);
                                                _context.SaveChanges();
                                            }

                                            #endregion

                                            if (model.IssueFor == "Crust Store")
                                            {
                                                #region Receive Crust Store

                                                #region Daily Stock

                                                var CheckDate2 = (from ds in _context.INV_CrustStockDaily
                                                                  where ds.StockDate == currentDate
                                                                        && ds.StoreID == model.IssueTo
                                                                        && ds.ArticleID == objQCItem.ArticleID
                                                                        && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                        && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                        && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                        && ds.ColorID == objQCSelection.ColorID
                                                                      //&& ds.GradeID == objQCSelection.GradeID
                                                                        && ds.CrustQCLabel == objQCSelection.CrustQCLabel
                                                                  select ds).Any();

                                                if (CheckDate2)
                                                {
                                                    var CurrentItem =
                                                        (from ds in _context.INV_CrustStockDaily
                                                         where ds.StockDate == currentDate
                                                               && ds.StoreID == model.IssueTo
                                                               && ds.ArticleID == objQCItem.ArticleID
                                                               && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                               && ds.ItemTypeID == objQCItem.ItemTypeID
                                                               && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                               && ds.ColorID == objQCSelection.ColorID
                                                             //&& ds.GradeID == objQCSelection.GradeID
                                                               && ds.CrustQCLabel == objQCSelection.CrustQCLabel
                                                         select ds).FirstOrDefault();

                                                    CurrentItem.IssueStockPcs = CurrentItem.ReceiveStockPcs + objQCSelection.IssuePcs;
                                                    CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockPcs + objQCSelection.IssuePcs;

                                                    CurrentItem.IssueStockSide = CurrentItem.ReceiveStockSide + objQCSelection.IssueSide;
                                                    CurrentItem.ClosingStockSide = CurrentItem.ClosingStockSide + objQCSelection.IssueSide;

                                                    CurrentItem.IssueStockArea = CurrentItem.ReceiveStockArea + objQCSelection.IssueArea;
                                                    CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockArea + objQCSelection.IssueArea;
                                                    _context.SaveChanges();
                                                }
                                                else
                                                {
                                                    var PreviousRecord =
                                                        (from ds in _context.INV_CrustStockDaily
                                                         where ds.StoreID == model.IssueTo
                                                               && ds.ArticleID == objQCItem.ArticleID
                                                               && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                               && ds.ItemTypeID == objQCItem.ItemTypeID
                                                               && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                               && ds.ColorID == objQCSelection.ColorID
                                                             //&& ds.GradeID == objQCSelection.GradeID
                                                               && ds.CrustQCLabel == objQCSelection.CrustQCLabel
                                                         orderby ds.TransectionID descending
                                                         select ds).FirstOrDefault();

                                                    var objStockDaily = new INV_CrustStockDaily();

                                                    objStockDaily.StockDate = currentDate;

                                                    objStockDaily.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockDaily.ItemTypeID = objQCItem.ItemTypeID;
                                                    objStockDaily.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                    objStockDaily.LeatherStatusID = objQCItem.LeatherStatusID;
                                                    objStockDaily.ArticleID = objQCItem.ArticleID;
                                                    objStockDaily.ColorID = objQCSelection.ColorID;

                                                    objStockDaily.ArticleChallanID = objQCItem.ArticleChallanID;
                                                    objStockDaily.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                    objStockDaily.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                    objStockDaily.GradeRange = objQCSelection.GradeRange;

                                                    //objStockSupplier.GradeID = objQCSelection.GradeID;

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockDaily.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockDaily.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                    objStockDaily.CrustQCLabel = objQCSelection.CrustQCLabel;
                                                    objStockDaily.OpeningStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs);
                                                    objStockDaily.ReceiveStockPcs = objQCSelection.IssuePcs;
                                                    objStockDaily.IssueStockPcs = 0;
                                                    objStockDaily.ClosingStockPcs = objStockDaily.OpeningStockPcs + objQCSelection.IssuePcs;

                                                    objStockDaily.OpeningStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide);
                                                    objStockDaily.ReceiveStockSide = objQCSelection.IssueSide;
                                                    objStockDaily.IssueStockSide = 0;
                                                    objStockDaily.ClosingStockSide = objStockDaily.OpeningStockSide + objQCSelection.IssueSide;

                                                    objStockDaily.OpeningStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea);
                                                    objStockDaily.ReceiveStockArea = objQCSelection.IssueArea;
                                                    objStockDaily.IssueStockArea = 0;
                                                    objStockDaily.ClosingStockArea = objStockDaily.OpeningStockArea + objQCSelection.IssueArea;

                                                    _context.INV_CrustStockDaily.Add(objStockDaily);
                                                    _context.SaveChanges();
                                                }

                                                #endregion

                                                #region Buyer Stock

                                                var CheckBuyerStock2 =
                                                    (from ds in _context.INV_CrustBuyerStock
                                                     where ds.BuyerID == objQCItem.BuyerID
                                                           && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                           && ds.StoreID == model.IssueTo
                                                           && ds.ArticleID == objQCItem.ArticleID
                                                           && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                           && ds.ItemTypeID == objQCItem.ItemTypeID
                                                           && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                           && ds.ColorID == objQCSelection.ColorID
                                                         //&& ds.GradeID == objQCSelection.GradeID
                                                           && ds.CrustQCLabel == objQCSelection.CrustQCLabel
                                                     select ds).Any();

                                                if (!CheckBuyerStock2)
                                                {
                                                    var PreviousRecord =
                                                        (from ds in _context.INV_CrustBuyerStock
                                                         where ds.BuyerID == objQCItem.BuyerID
                                                               && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                               && ds.StoreID == model.IssueTo
                                                               && ds.ArticleID == objQCItem.ArticleID
                                                               && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                               && ds.ItemTypeID == objQCItem.ItemTypeID
                                                               && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                               && ds.ColorID == objQCSelection.ColorID
                                                             //&& ds.GradeID == objQCSelection.GradeID
                                                               && ds.CrustQCLabel == objQCSelection.CrustQCLabel
                                                         orderby ds.TransectionID descending
                                                         select ds).FirstOrDefault();

                                                    var objStockSupplier = new INV_CrustBuyerStock();

                                                    if (objQCItem.BuyerID == null)
                                                        objStockSupplier.BuyerID = null;
                                                    else
                                                        objStockSupplier.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                    if (objQCItem.BuyerOrderID == null)
                                                        objStockSupplier.BuyerOrderID = null;
                                                    else
                                                        objStockSupplier.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                    objStockSupplier.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockSupplier.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                    objStockSupplier.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                    objStockSupplier.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                    objStockSupplier.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                    objStockSupplier.ColorID = Convert.ToInt32(objQCSelection.ColorID);

                                                    objStockSupplier.ArticleChallanID = objQCItem.ArticleChallanID;
                                                    objStockSupplier.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                    objStockSupplier.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                    objStockSupplier.GradeRange = objQCSelection.GradeRange;

                                                    //objStockSupplier.GradeID = objQCSelection.GradeID;


                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockSupplier.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockSupplier.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                    objStockSupplier.CrustQCLabel = objQCSelection.CrustQCLabel;

                                                    objStockSupplier.OpeningStockPcs = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs;
                                                    objStockSupplier.ReceiveStockPcs = objQCSelection.IssuePcs;
                                                    objStockSupplier.IssueStockPcs = 0;
                                                    objStockSupplier.ClosingStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs) + objQCSelection.IssuePcs;

                                                    objStockSupplier.OpeningStockSide = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide;
                                                    objStockSupplier.ReceiveStockSide = objQCSelection.IssueSide;
                                                    objStockSupplier.IssueStockSide = 0;
                                                    objStockSupplier.ClosingStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide) + objQCSelection.IssueSide;

                                                    objStockSupplier.OpeningStockArea = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea;
                                                    objStockSupplier.ReceiveStockArea = objQCSelection.IssueArea;
                                                    objStockSupplier.IssueStockArea = 0;
                                                    objStockSupplier.ClosingStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea) + objQCSelection.IssueArea;

                                                    _context.INV_CrustBuyerStock.Add(objStockSupplier);
                                                    _context.SaveChanges();
                                                }
                                                else
                                                {
                                                    var LastBuyerStock =
                                                        (from ds in _context.INV_CrustBuyerStock
                                                         where ds.BuyerID == objQCItem.BuyerID
                                                               && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                               && ds.StoreID == model.IssueTo
                                                               && ds.ArticleID == objQCItem.ArticleID
                                                               && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                               && ds.ItemTypeID == objQCItem.ItemTypeID
                                                               && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                               && ds.ColorID == objQCSelection.ColorID
                                                             //&& ds.GradeID == objQCSelection.GradeID
                                                               && ds.CrustQCLabel == objQCSelection.CrustQCLabel
                                                         orderby ds.TransectionID descending
                                                         select ds).FirstOrDefault();


                                                    var objStockSupplier = new INV_CrustBuyerStock();

                                                    if (objQCItem.BuyerID == null)
                                                        objStockSupplier.BuyerID = null;
                                                    else
                                                        objStockSupplier.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                    if (objQCItem.BuyerOrderID == null)
                                                        objStockSupplier.BuyerOrderID = null;
                                                    else
                                                        objStockSupplier.BuyerOrderID =
                                                            Convert.ToInt32(objQCItem.BuyerOrderID);
                                                    objStockSupplier.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockSupplier.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                    objStockSupplier.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                    objStockSupplier.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                    objStockSupplier.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                    objStockSupplier.ColorID = Convert.ToInt32(objQCSelection.ColorID);

                                                    objStockSupplier.ArticleChallanID = objQCItem.ArticleChallanID;
                                                    objStockSupplier.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                    objStockSupplier.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                    objStockSupplier.GradeRange = objQCSelection.GradeRange;

                                                    //objStockSupplier.GradeID = objQCSelection.GradeID;

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockSupplier.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockSupplier.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                    objStockSupplier.CrustQCLabel = objQCSelection.CrustQCLabel;

                                                    objStockSupplier.OpeningStockPcs = LastBuyerStock.ClosingStockPcs;
                                                    objStockSupplier.ReceiveStockPcs = objQCSelection.IssuePcs;
                                                    objStockSupplier.IssueStockPcs = 0;
                                                    objStockSupplier.ClosingStockPcs = LastBuyerStock.ClosingStockPcs + objQCSelection.IssuePcs;

                                                    objStockSupplier.OpeningStockSide = LastBuyerStock.ClosingStockSide;
                                                    objStockSupplier.ReceiveStockSide = objQCSelection.IssueSide;
                                                    objStockSupplier.IssueStockSide = 0;
                                                    objStockSupplier.ClosingStockSide = LastBuyerStock.ClosingStockSide + objQCSelection.IssueSide;

                                                    objStockSupplier.OpeningStockArea = LastBuyerStock.ClosingStockArea;
                                                    objStockSupplier.ReceiveStockArea = objQCSelection.IssueArea;
                                                    objStockSupplier.IssueStockArea = 0;
                                                    objStockSupplier.ClosingStockArea = LastBuyerStock.ClosingStockArea + objQCSelection.IssueArea;

                                                    _context.INV_CrustBuyerStock.Add(objStockSupplier);
                                                    _context.SaveChanges();
                                                }

                                                #endregion

                                                #region Item Stock

                                                var CheckItemStock2 =
                                                    (from ds in _context.INV_CrustStockItem
                                                     where ds.StoreID == model.IssueTo
                                                           && ds.ArticleID == objQCItem.ArticleID
                                                           && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                           && ds.ItemTypeID == objQCItem.ItemTypeID
                                                           && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                           && ds.ColorID == objQCSelection.ColorID
                                                         //&& ds.GradeID == objQCSelection.GradeID
                                                           && ds.CrustQCLabel == objQCSelection.CrustQCLabel
                                                     select ds).Any();

                                                if (!CheckItemStock2)
                                                {
                                                    var PreviousRecord =
                                                        (from ds in _context.INV_CrustStockItem
                                                         where ds.StoreID == model.IssueTo
                                                               && ds.ArticleID == objQCItem.ArticleID
                                                               && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                               && ds.ItemTypeID == objQCItem.ItemTypeID
                                                               && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                               && ds.ColorID == objQCSelection.ColorID
                                                             //&& ds.GradeID == objQCSelection.GradeID
                                                               && ds.CrustQCLabel == objQCSelection.CrustQCLabel
                                                         orderby ds.TransectionID descending
                                                         select ds).FirstOrDefault();

                                                    var objStockItem = new INV_CrustStockItem();

                                                    objStockItem.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                    objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                    objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                    objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                    objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);

                                                    objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                    objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                    objStockItem.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                    objStockItem.GradeRange = objQCSelection.GradeRange;

                                                    //objStockItem.GradeID = objQCSelection.GradeID;

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockItem.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                    objStockItem.CrustQCLabel = objQCSelection.CrustQCLabel;

                                                    objStockItem.OpeningStockPcs = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs;
                                                    objStockItem.ReceiveStockPcs = objQCSelection.IssuePcs;
                                                    objStockItem.IssueStockPcs = 0;
                                                    objStockItem.ClosingStockPcs = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockPcs) + objQCSelection.IssuePcs;

                                                    objStockItem.OpeningStockSide = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide;
                                                    objStockItem.ReceiveStockSide = objQCSelection.IssueSide;
                                                    objStockItem.IssueStockSide = 0;
                                                    objStockItem.ClosingStockSide = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockSide) + objQCSelection.IssueSide;

                                                    objStockItem.OpeningStockArea = PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea;
                                                    objStockItem.ReceiveStockArea = objQCSelection.IssueArea;
                                                    objStockItem.IssueStockArea = 0;
                                                    objStockItem.ClosingStockArea = (PreviousRecord == null ? 0 : PreviousRecord.ClosingStockArea) + objQCSelection.IssueArea;

                                                    _context.INV_CrustStockItem.Add(objStockItem);
                                                    _context.SaveChanges();
                                                }
                                                else
                                                {
                                                    var LastItemInfo = (from ds in _context.INV_CrustStockItem
                                                                        where ds.StoreID == model.IssueTo
                                                                              && ds.ArticleID == objQCItem.ArticleID
                                                                              && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                              && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                              && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                              && ds.ColorID == objQCSelection.ColorID
                                                                        //&& ds.GradeID == objQCSelection.GradeID
                                                                        orderby ds.TransectionID descending
                                                                        select ds).FirstOrDefault();

                                                    var objStockItem = new INV_CrustStockItem();

                                                    objStockItem.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                    objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Crust").FirstOrDefault().LeatherTypeID);
                                                    objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                    objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                    objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);

                                                    objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                    objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                    objStockItem.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                    objStockItem.GradeRange = objQCSelection.GradeRange;

                                                    //objStockItem.GradeID = objQCSelection.GradeID;

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockItem.ClosingStockAreaUnit = null;
                                                    else
                                                        objStockItem.ClosingStockAreaUnit = _context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID;
                                                    objStockItem.CrustQCLabel = objQCSelection.CrustQCLabel;

                                                    objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
                                                    objStockItem.ReceiveStockPcs = objQCSelection.IssuePcs;
                                                    objStockItem.IssueStockPcs = 0;
                                                    objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs + objQCSelection.IssuePcs;

                                                    objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
                                                    objStockItem.ReceiveStockSide = objQCSelection.IssueSide;
                                                    objStockItem.IssueStockSide = 0;
                                                    objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide + objQCSelection.IssueSide;

                                                    objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea;
                                                    objStockItem.ReceiveStockArea = objQCSelection.IssueArea;
                                                    objStockItem.IssueStockArea = 0;
                                                    objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea + objQCSelection.IssueArea;

                                                    _context.INV_CrustStockItem.Add(objStockItem);
                                                    _context.SaveChanges();
                                                }

                                                #endregion

                                                #endregion
                                            }
                                            else if (model.IssueFor == "Finish Production")
                                            {
                                                #region Receive Finish Production

                                                var CheckItemStock1 = (from ds in _context.PRD_FinishLeatherProductionStock
                                                                       where ds.StoreID == model.IssueTo
                                                                    && ds.BuyerID == objQCItem.BuyerID
                                                                    && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                    && ds.ArticleID == objQCItem.ArticleID
                                                                    && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                    && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                    && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                    && ds.ColorID == objQCSelection.ColorID
                                                                       //&& ds.GradeID == objQCSelection.GradeID
                                                                       select ds).Any();
                                                if (!CheckItemStock1)
                                                {
                                                    PRD_FinishLeatherProductionStock objStockItem = new PRD_FinishLeatherProductionStock();

                                                    if (objQCItem.BuyerID == null)
                                                        objStockItem.BuyerID = null;
                                                    else
                                                        objStockItem.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                    if (objQCItem.BuyerOrderID == null)
                                                        objStockItem.BuyerOrderID = null;
                                                    else
                                                        objStockItem.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);

                                                    objStockItem.StoreID = Convert.ToByte(model.IssueTo);
                                                    //objStockItem.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                    //objStockItem.BuyerOrderID = objQCItem.BuyerOrderID;
                                                    objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                    objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                    objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                    objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                    objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);

                                                    objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                    objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                    objStockItem.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                    objStockItem.GradeRange = objQCSelection.GradeRange;

                                                    objStockItem.QCStatus = "REP";
                                                    //objStockItem.GradeID = Convert.ToByte(objQCSelection.GradeID);

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockItem.AreaUnit = null;
                                                    else
                                                        objStockItem.AreaUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID);
                                                    objStockItem.OpeningPcs = 0;
                                                    objStockItem.ReceivePcs = objQCSelection.IssuePcs;
                                                    objStockItem.IssuePcs = 0;
                                                    objStockItem.ClosingProductionPcs = objQCSelection.IssuePcs;

                                                    objStockItem.OpeningSide = 0;
                                                    objStockItem.ReceiveSide = objQCSelection.IssueSide == null ? 0 : objQCSelection.IssueSide;
                                                    objStockItem.IssueSide = 0;
                                                    objStockItem.ClosingProductionSide = objQCSelection.IssueSide == null ? 0 : objQCSelection.IssueSide;

                                                    objStockItem.OpeningArea = 0;
                                                    objStockItem.ReceiveArea = objQCSelection.IssueArea;
                                                    objStockItem.IssueArea = 0;
                                                    objStockItem.ClosingProductionArea = objQCSelection.IssueArea;

                                                    _context.PRD_FinishLeatherProductionStock.Add(objStockItem);
                                                    _context.SaveChanges();
                                                }
                                                else
                                                {
                                                    var LastItemInfo = (from ds in _context.PRD_FinishLeatherProductionStock
                                                                        where ds.StoreID == model.IssueTo
                                                                    && ds.BuyerID == objQCItem.BuyerID
                                                                    && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                                    && ds.ArticleID == objQCItem.ArticleID
                                                                    && ds.ArticleChallanID == objQCItem.ArticleChallanID
                                                                    && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                    && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                    && ds.ColorID == objQCSelection.ColorID
                                                                        //&& ds.GradeID == objQCSelection.GradeID
                                                                        orderby ds.TransectionID descending
                                                                        select ds).FirstOrDefault();

                                                    PRD_FinishLeatherProductionStock objStockItem = new PRD_FinishLeatherProductionStock();

                                                    if (objQCItem.BuyerID == null)
                                                        objStockItem.BuyerID = null;
                                                    else
                                                        objStockItem.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                    if (objQCItem.BuyerOrderID == null)
                                                        objStockItem.BuyerOrderID = null;
                                                    else
                                                        objStockItem.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);

                                                    //objStockItem.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                    //objStockItem.BuyerOrderID = objQCItem.BuyerOrderID;

                                                    objStockItem.StoreID = Convert.ToByte(model.IssueTo);
                                                    objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                    objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                    objStockItem.LeatherTypeID = Convert.ToByte(_context.Sys_LeatherType.Where(m => m.LeatherTypeName == "Finish").FirstOrDefault().LeatherTypeID);
                                                    objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                    objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);

                                                    objStockItem.ArticleChallanID = objQCItem.ArticleChallanID;
                                                    objStockItem.ArticleChallanNo = objQCItem.ArticleChallanNo;
                                                    objStockItem.ArticleColorNo = Convert.ToInt32(objQCSelection.ArticleColorNo);
                                                    objStockItem.GradeRange = objQCSelection.GradeRange;

                                                    objStockItem.QCStatus = "REP";

                                                    //objStockItem.GradeID = Convert.ToByte(objQCSelection.GradeID);

                                                    if (string.IsNullOrEmpty(objQCSelection.AreaUnitName))
                                                        objStockItem.AreaUnit = null;
                                                    else
                                                        objStockItem.AreaUnit = Convert.ToByte(_context.Sys_Unit.Where(m => m.UnitName == objQCSelection.AreaUnitName).FirstOrDefault().UnitID);

                                                    objStockItem.OpeningPcs = LastItemInfo.ClosingProductionPcs;
                                                    objStockItem.ReceivePcs = objQCSelection.IssuePcs;
                                                    objStockItem.IssuePcs = 0;
                                                    objStockItem.ClosingProductionPcs = LastItemInfo.ClosingProductionPcs + objQCSelection.IssuePcs;

                                                    objStockItem.OpeningSide = LastItemInfo.ClosingProductionSide;
                                                    objStockItem.ReceiveSide = objQCSelection.IssueSide;
                                                    objStockItem.IssueSide = 0;
                                                    objStockItem.ClosingProductionSide = LastItemInfo.ClosingProductionSide + objQCSelection.IssueSide;

                                                    objStockItem.OpeningArea = LastItemInfo.OpeningArea;
                                                    objStockItem.ReceiveArea = objQCSelection.IssueArea;
                                                    objStockItem.IssueArea = 0;
                                                    objStockItem.ClosingProductionArea = LastItemInfo.ClosingProductionArea + objQCSelection.IssueArea;

                                                    _context.PRD_FinishLeatherProductionStock.Add(objStockItem);
                                                    _context.SaveChanges();
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
            List<INV_CrustLeatherIssue> searchList = _context.INV_CrustLeatherIssue.Where(m => m.IssueCategory == "IAQC").OrderByDescending(m => m.CrustLeatherIssueID).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<InvCrustLeatherIssue>();
        }

        public InvCrustLeatherIssue SetToBussinessObject(INV_CrustLeatherIssue Entity)
        {
            InvCrustLeatherIssue Model = new InvCrustLeatherIssue();

            Model.CrustLeatherIssueID = Entity.CrustLeatherIssueID;
            Model.CrustLeatherIssueNo = Entity.CrustLeatherIssueNo;
            Model.CrustLeatherIssueDate = string.IsNullOrEmpty(Entity.CrustLeatherIssueDate.ToString()) ? string.Empty : Convert.ToDateTime(Entity.CrustLeatherIssueDate).ToString("dd/MM/yyyy");
            Model.IssueFrom = Entity.IssueFrom;
            Model.IssueFromName = Entity.IssueFrom == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.IssueFrom).FirstOrDefault().StoreName;
            Model.IssueTo = Entity.IssueTo;
            Model.IssueToName = Entity.IssueTo == null ? "" : _context.SYS_Store.Where(m => m.StoreID == Entity.IssueTo).FirstOrDefault().StoreName;
            Model.IssueCategory = "After QC";
            Model.IssueFor = Entity.IssueFor;
            Model.IssueQCStatus = Entity.IssueQCStatus;
            Model.RecordStatus = Entity.RecordStatus;
            return Model;
        }

        public ValidationMsg DeletedCrustLeatherIssue(long CrustLeatherIssueID, string RecordStatus)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                if (RecordStatus != "CNF")
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

        public ValidationMsg DeletedCrustLeatherIssueItem(long CrustLeatherIssueItemID, string RecordStatus)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                //var CrustLeatherIssueID = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueItemID == QCItemID).FirstOrDefault().CrustLeatherIssueID;
                //var RecordStatus = _context.INV_CrustLeatherIssue.First(m => m.CrustLeatherIssueID == CrustLeatherIssueID).RecordStatus;
                if (RecordStatus != "CNF")
                {
                    var scheduleList = _context.INV_CrustLeatherIssueColor.Where(m => m.CrustLeatherIssueItemID == CrustLeatherIssueItemID).ToList();
                    if (scheduleList.Count > 0)
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Child Record Found.";
                    }
                    else
                    {
                        var deleteElement = _context.INV_CrustLeatherIssueItem.First(m => m.CrustLeatherIssueItemID == CrustLeatherIssueItemID);
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

        public ValidationMsg DeletedCrustLeatherIssueColor(long crustLeatherIssueColorID, string RecordStatus)
        {
            _vmMsg = new ValidationMsg();
            try
            {
                //var QCItemID = _context.INV_CrustLeatherIssueColor.Where(m => m.CrustLeatherIssueColorID == QCSelectionID).FirstOrDefault().CrustLeatherIssueItemID;
                //var CrustLeatherIssueID = _context.INV_CrustLeatherIssueItem.Where(m => m.CrustLeatherIssueItemID == QCItemID).FirstOrDefault().CrustLeatherIssueID;
                //var RecordStatus = _context.INV_CrustLeatherIssue.First(m => m.CrustLeatherIssueID == CrustLeatherIssueID).RecordStatus;
                if (RecordStatus != "CNF")
                {
                    var deleteElement = _context.INV_CrustLeatherIssueColor.First(m => m.CrustLeatherIssueColorID == crustLeatherIssueColorID);
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

        public List<SysStore> GetIssueFromAndToList(string storeType)
        {
            var storeCategory = "";
            switch (storeType)
            {
                case "Crust Store":
                    storeType = "Crust";
                    storeCategory = "Leather";
                    break;
                case "Finish Production":
                    storeType = "FN Production";
                    storeCategory = "Production";
                    break;
            }
            List<SYS_Store> searchList = _context.SYS_Store.Where(m => m.IsActive && !m.IsDelete && m.StoreCategory == storeCategory && m.StoreType == storeType).ToList();
            return searchList.Select(c => SetToBussinessObject(c)).ToList<SysStore>();
        }

        public SysStore SetToBussinessObject(SYS_Store Entity)
        {
            SysStore Model = new SysStore();

            Model.StoreID = Entity.StoreID;
            Model.StoreName = Entity.StoreName;

            return Model;
        }
    }
}
