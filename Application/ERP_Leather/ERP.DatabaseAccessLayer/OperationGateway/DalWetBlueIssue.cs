using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Caching;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalWetBlueIssue
    {
        private readonly BLC_DEVEntities _context;
        private readonly string _connString = string.Empty;
        private readonly ValidationMsg _vmMsg;
        private UnitOfWork _unit;
        bool _haveStockValue;
        public DalWetBlueIssue()
        {
            _context = new BLC_DEVEntities();
            _connString = StrConnection.GetConnectionString();
            _unit = new UnitOfWork();
            _vmMsg = new ValidationMsg();
            _haveStockValue = true;
            //_context.Database.CommandTimeout = 60;
        }

        public ValidationMsg SaveData(InvWetBlueIssue model, int userId, string pageUrl)
        {
            try
            {
                using (var tx = new TransactionScope())
                {

                    long currentIssueId = 0;
                    var currentCode = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);
                    using (_context)
                    {
                        var obj = new INV_WetBlueIssue
                        {
                            WetBlueIssueNo = currentCode,
                            IssueCategory = model.IssueCategory,
                            IssueFor = model.IssueFor,
                            WetBlueIssueDate = DalCommon.SetDate(model.WetBlueIssueDate),
                            IssueFrom = model.IssueFrom,
                            IssueTo = model.IssueTo,
                            IssuedBy = userId,
                            IssueDate = DateTime.Now,
                            IssueNote = model.IssueNote,
                            RequisitionDateID = model.RequisitionDateID,
                            CheckedBy = userId,
                            CheckDate = DateTime.Now,
                            CheckNote = model.CheckNote,
                            RecordStatus = "NCF",
                            SetOn = DateTime.Now,
                            SetBy = userId,
                            IPAddress = GetIPAddress.LocalIPAddress()
                        };
                        _context.INV_WetBlueIssue.Add(obj);
                        _context.SaveChanges();

                        currentIssueId = obj.WetBlueIssueID;
                        var currentWetBlueIssueRefIdList = string.Empty;
                        if (model.RefModels.Count > 0)
                        {
                            foreach (var objRef in model.RefModels.Select(wbiRef => new INV_WetBlueIssueRef()
                            {
                                WetBlueIssueID = currentIssueId,
                                WetBlueReqID = wbiRef.WetBlueReqID,
                                BuyerID = wbiRef.BuyerID == 0 ? null : wbiRef.BuyerID,
                                BuyerOrderID = wbiRef.BuyerOrderID == 0 ? null : wbiRef.BuyerOrderID,
                                ArticleID = wbiRef.ArticleID == 0 ? null : wbiRef.ArticleID,
                                ArticleNo = wbiRef.ArticleNo,
                                ArticleChallanNo = wbiRef.ArticleChallanNo,
                                ArticleChallanID = wbiRef.ArticleChallanID == 0 ? null : wbiRef.ArticleChallanID,
                                AvgSize = wbiRef.AvgSize,
                                AvgSizeUnit = wbiRef.AvgSizeUnit,
                                SideDescription = wbiRef.SideDescription,
                                SelectionRange = wbiRef.SelectionRange,
                                Thickness = wbiRef.Thickness,
                                ThicknessUnit = wbiRef.UnitID == 0 ? null : wbiRef.UnitID,
                                ThicknessAt = wbiRef.ThicknessAt,
                                SetOn = DateTime.Now,
                                SetBy = userId
                            }))
                            {
                                _context.INV_WetBlueIssueRef.Add(objRef);
                                _context.SaveChanges();
                                currentWetBlueIssueRefIdList += currentWetBlueIssueRefIdList == string.Empty
                                    ? objRef.WetBlueIssueRefID.ToString()
                                    : "," + objRef.WetBlueIssueRefID;
                            }
                        }

                        if (currentWetBlueIssueRefIdList != string.Empty)
                        {
                            if (!currentWetBlueIssueRefIdList.Contains(","))
                            {
                                if (model.ItemModels.Count > 0)
                                {
                                    foreach (
                                        var objItem in
                                            model.ItemModels.Select(item => new INV_WetBlueIssueItem()
                                            {
                                                WetBlueIssueRefID = Convert.ToInt64(currentWetBlueIssueRefIdList),
                                                WetBlueIssueID = currentIssueId,
                                                SupplierID = item.SupplierID,
                                                PurchaseID = item.PurchaseID,
                                                LotNo = item.LotNo,
                                                ItemTypeID = item.ItemTypeID,
                                                LeatherTypeID = _context.Sys_LeatherType.FirstOrDefault(m => m.LeatherTypeName == "Wet Blue").LeatherTypeID,
                                                LeatherStatusID = item.LeatherStatusID,
                                                GradeID = item.GradeID,
                                                SizeQtyRef = DalCommon.ConvertLeatherSizeTextToValue(item.SizeQtyRef),
                                                IssuePcs = item.IssuePcs,
                                                PieceToSide = item.PcsToSide,
                                                IssueSide = item.IssueSide,
                                                IssueArea = item.IssueArea,
                                                AreaUnit = item.AreaUnit,
                                                SetOn = DateTime.Now,
                                                SetBy = userId
                                            }))
                                    {
                                        _context.INV_WetBlueIssueItem.Add(objItem);
                                        _context.SaveChanges();

                                    }
                                }
                            }
                            else //for multiple refId
                            {
                                var refList = currentWetBlueIssueRefIdList.Split(',');

                                if (model.ItemModels.Count > 0)
                                {
                                    foreach (
                                        var objItem in
                                            model.ItemModels.Select(item => new INV_WetBlueIssueItem()
                                            {
                                                WetBlueIssueRefID = Convert.ToInt64(refList[0]),
                                                WetBlueIssueID = currentIssueId,
                                                SupplierID = item.SupplierID,
                                                PurchaseID = item.PurchaseID,
                                                LotNo = item.LotNo,
                                                ItemTypeID = item.ItemTypeID,
                                                LeatherTypeID = _context.Sys_LeatherType.FirstOrDefault(m => m.LeatherTypeName == "Wet Blue").LeatherTypeID,
                                                LeatherStatusID = item.LeatherStatusID,
                                                GradeID = item.GradeID,
                                                SizeQtyRef = DalCommon.ConvertLeatherSizeTextToValue(item.SizeQtyRef),
                                                IssuePcs = item.IssuePcs,
                                                PieceToSide = item.PcsToSide,
                                                IssueSide = item.IssueSide,
                                                IssueArea = item.IssueArea,
                                                AreaUnit = item.AreaUnit,
                                                SetOn = DateTime.Now,
                                                SetBy = userId
                                            }))
                                    {
                                        _context.INV_WetBlueIssueItem.Add(objItem);
                                        _context.SaveChanges();

                                    }
                                }
                            }
                        }
                        else
                        {
                            if (model.ItemModels.Count > 0)
                            {
                                foreach (
                                    var objItem in
                                        model.ItemModels.Select(item => new INV_WetBlueIssueItem()
                                        {
                                            WetBlueIssueID = currentIssueId,
                                            SupplierID = item.SupplierID,
                                            PurchaseID = item.PurchaseID,
                                            LotNo = item.LotNo,
                                            ItemTypeID = item.ItemTypeID,
                                            LeatherTypeID = _context.Sys_LeatherType.FirstOrDefault(m => m.LeatherTypeName == "Wet Blue").LeatherTypeID,
                                            LeatherStatusID = item.LeatherStatusID,
                                            GradeID = item.GradeID,
                                            SizeQtyRef = DalCommon.ConvertLeatherSizeTextToValue(item.SizeQtyRef),
                                            IssuePcs = item.IssuePcs,
                                            PieceToSide = item.PcsToSide,
                                            IssueSide = item.IssueSide,
                                            IssueArea = item.IssueArea,
                                            AreaUnit = item.AreaUnit,
                                            SetOn = DateTime.Now,
                                            SetBy = userId
                                        }))
                                {
                                    _context.INV_WetBlueIssueItem.Add(objItem);
                                    _context.SaveChanges();

                                }
                            }
                        }
                    }

                    tx.Complete();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Saved Successfully.";
                    _vmMsg.ReturnId = currentIssueId;
                    _vmMsg.ReturnCode = currentCode;
                }
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public ValidationMsg UpdateData(InvWetBlueIssue model, int userId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {

                    using (_context)
                    {

                        var objModel =
                            _context.INV_WetBlueIssue.FirstOrDefault(r => r.WetBlueIssueID == model.WetBlueIssueID);
                        if (objModel != null)
                        {
                            objModel.IssueCategory = model.IssueCategory;
                            objModel.IssueFor = model.IssueFor;
                            objModel.IssueFrom = model.IssueFrom;
                            objModel.IssueTo = model.IssueTo;
                            objModel.WetBlueIssueDate = DalCommon.SetDate(model.WetBlueIssueDate);
                            objModel.RequisitionDateID = model.RequisitionDateID;
                            objModel.ModifiedBy = userId;
                            objModel.ModifiedOn = DateTime.Now;
                        }
                        _context.SaveChanges();

                        if (model.RefModels.Count > 0 || model.ItemModels.Count > 0)
                        {
                            var currentWetBlueIssueRefId = (long)0;
                            if (model.RefModels.Count > 0)
                            {

                                foreach (var refItem in model.RefModels)
                                {

                                    if (refItem.WetBlueIssueRefID == 0)
                                    {
                                        var objRef = new INV_WetBlueIssueRef();
                                        objRef.WetBlueIssueID = objModel.WetBlueIssueID;
                                        objRef.BuyerID = refItem.BuyerID == 0 ? null : refItem.BuyerID;
                                        objRef.BuyerOrderID = refItem.BuyerOrderID == 0 ? null : refItem.BuyerOrderID;

                                        objRef.ArticleID = refItem.ArticleID == 0 ? null : refItem.ArticleID;
                                        objRef.ArticleChallanID = refItem.ArticleChallanID == 0 ? null : refItem.ArticleChallanID;
                                        objRef.ArticleChallanNo = refItem.ArticleChallanNo;
                                        objRef.AvgSize = refItem.AvgSize;
                                        objRef.SideDescription = refItem.SideDescription;
                                        objRef.SelectionRange = refItem.SelectionRange;
                                        objRef.Thickness = refItem.Thickness;
                                        objRef.ThicknessUnit = refItem.UnitID == 0 ? null : refItem.UnitID;
                                        objRef.ThicknessAt = refItem.ThicknessAt;


                                        objRef.SetOn = DateTime.Now;
                                        objRef.SetBy = userId;
                                        _context.INV_WetBlueIssueRef.Add(objRef);
                                        _context.SaveChanges();
                                        currentWetBlueIssueRefId = objRef.WetBlueIssueRefID;
                                    }
                                    else
                                    {
                                        var updateItem =
                                            _context.INV_WetBlueIssueRef.First(
                                                r => r.WetBlueIssueRefID == refItem.WetBlueIssueRefID);

                                        updateItem.BuyerID = refItem.BuyerID == 0 ? null : refItem.BuyerID;
                                        updateItem.BuyerOrderID = refItem.BuyerOrderID == 0 ? null : refItem.BuyerOrderID;
                                        updateItem.ArticleID = refItem.ArticleID == 0 ? null : refItem.ArticleID;
                                        updateItem.ArticleChallanID = refItem.ArticleChallanID == 0 ? null : refItem.ArticleChallanID;
                                        updateItem.ArticleChallanNo = refItem.ArticleChallanNo;
                                        updateItem.AvgSize = refItem.AvgSize;
                                        updateItem.SideDescription = refItem.SideDescription;
                                        updateItem.SelectionRange = refItem.SelectionRange;
                                        updateItem.Thickness = refItem.Thickness;
                                        updateItem.ThicknessUnit = refItem.UnitID == 0 ? null : refItem.UnitID;
                                        updateItem.ThicknessAt = refItem.ThicknessAt;

                                        updateItem.ModifiedOn = DateTime.Now;
                                        updateItem.ModifiedBy = userId;
                                        currentWetBlueIssueRefId = updateItem.WetBlueIssueRefID;
                                    }
                                    _context.SaveChanges();
                                }
                            }
                            if (model.ItemModels.Count > 0)
                            {
                                foreach (var item in model.ItemModels)
                                {

                                    if (item.WBSIssueItemID == 0)
                                    {
                                        var objItem = new INV_WetBlueIssueItem();

                                        //objItem.WetBlueIssueRefID = currentWetBlueIssueRefId == (long)0 ? item.WetBlueIssueRefID : currentWetBlueIssueRefId;
                                        objItem.WetBlueIssueRefID = item.WetBlueIssueRefID != (long)0 ? item.WetBlueIssueRefID : currentWetBlueIssueRefId;
                                        objItem.WetBlueIssueID = objModel.WetBlueIssueID;
                                        objItem.SupplierID = item.SupplierID;
                                        objItem.PurchaseID = item.PurchaseID;
                                        objItem.LotNo = item.LotNo;
                                        objItem.ItemTypeID = item.ItemTypeID;
                                        objItem.LeatherStatusID = item.LeatherStatusID;
                                        objItem.LeatherTypeID =
                                            _context.Sys_LeatherType.FirstOrDefault(m => m.LeatherTypeName == "Wet Blue")
                                                .LeatherTypeID;
                                        objItem.GradeID = item.GradeID;
                                        objItem.SizeQtyRef =DalCommon.ConvertLeatherSizeTextToValue(item.SizeQtyRef);
                                        objItem.IssuePcs = item.IssuePcs ?? 0;
                                        objItem.PieceToSide = item.PcsToSide ?? 0;
                                        objItem.IssueSide = item.IssueSide ?? 0;
                                        objItem.IssueArea = item.IssueArea ?? 0;

                                        objItem.ModifiedOn = DateTime.Now;
                                        objItem.ModifiedBy = userId;
                                        _context.INV_WetBlueIssueItem.Add(objItem);
                                    }
                                    else
                                    {
                                        var udItem =
                                            _context.INV_WetBlueIssueItem.First(r => r.WBSIssueItemID == item.WBSIssueItemID);
                                        // udItem.WetBlueIssueRefID = currentWetBlueIssueRefId == (long)0 ? item.WetBlueIssueRefID : currentWetBlueIssueRefId;
                                        udItem.WetBlueIssueRefID = item.WetBlueIssueRefID != (long)0 ? item.WetBlueIssueRefID : currentWetBlueIssueRefId;
                                        udItem.WetBlueIssueID = objModel.WetBlueIssueID;
                                        udItem.SupplierID = item.SupplierID;
                                        udItem.PurchaseID = item.PurchaseID;
                                        udItem.LotNo = item.LotNo;
                                        udItem.ItemTypeID = item.ItemTypeID;
                                        udItem.LeatherStatusID = item.LeatherStatusID;
                                        udItem.IssuePcs = item.IssuePcs ?? 0;
                                        udItem.PieceToSide = item.PcsToSide ?? 0;
                                        udItem.IssueSide = item.IssueSide ?? 0;
                                        udItem.IssueArea = item.IssueArea ?? 0;
                                        udItem.GradeID = item.GradeID;
                                        udItem.SizeQtyRef = item.SizeQtyRef;
                                        udItem.ModifiedOn = DateTime.Now;
                                        udItem.ModifiedBy = userId;

                                    }
                                    _context.SaveChanges();
                                }

                            }

                        }
                    }
                    tx.Complete();

                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Updated Successfully.";
                    _vmMsg.ReturnId = model.WetBlueIssueID;
                    _vmMsg.ReturnCode = model.WetBlueIssueNo;
                }
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }
        
        public IEnumerable<InvWetBlueIssue> GetAllWetBlueIssueList(string pageMode)
        {
            var query = new StringBuilder();
            if (pageMode.Equals("ENT"))
            {
                query.Append(
                    "SELECT BI.WetBlueIssueID,BI.WetBlueIssueNo,BI.IssueFor, BI.IssueFrom,SF.StoreName IssueFromName, BI.IssueTo,S.StoreName IssueToName, BI.IssueCategory,CONVERT(NVARCHAR(12), BI.WetBlueIssueDate,106) WetBlueIssueDate, " +
                    "ISNULL(CR.RequisitionDateID,0) RequisitionDateID,ISNULL(CR.RequisitionNo,'') RequisitionNo, " +
                    "CASE BI.RecordStatus  " +
                    "WHEN 'NCF' THEN 'Not Confirmed' " +
                    "WHEN 'CHK' THEN 'Checked' " +
                    "WHEN 'CNF' THEN 'Confirmed' " +
                    "WHEN 'APV' THEN 'Approved' END RecordStatus  " +
                    "FROM [INV_WetBlueIssue] BI  INNER JOIN Sys_Store S ON S.StoreID=BI.IssueTo  " +
                    "INNER JOIN Sys_Store SF ON SF.StoreID=BI.IssueFrom " +
                    "LEFT JOIN PRD_YearMonthCrustReqDate CR ON CR.RequisitionDateID=BI.RequisitionDateID ");
            }

            var items = _context.Database.SqlQuery<InvWetBlueIssue>(query.ToString());
            return items.ToList().OrderByDescending(o => o.WetBlueIssueID);
        }
        public InvWetBlueIssue GetWetBlueIssueAllInfo(long wetBlueIssueId, string issueCategory, byte storeId)
        {
            var model = new InvWetBlueIssue();
            var objIssue = _context.INV_WetBlueIssue.First(m => m.WetBlueIssueID == wetBlueIssueId);
            model.RecordStatus = objIssue.RecordStatus;
            model.WetBlueIssueNo = objIssue.WetBlueIssueNo;
            model.IssueFrom = objIssue.IssueFrom;
            model.IssueTo = objIssue.IssueTo;
            model.IssueFor = objIssue.IssueFor;

            if (issueCategory == "PROD")
            {
                var query = new StringBuilder();
                query.Append(" SELECT WI.WetBlueIssueRefID,ISNULL(B.BuyerID,0)BuyerID,ISNULL(B.BuyerName,'')BuyerName,ISNULL(BO.BuyerOrderID,0)BuyerOrderID,ISNULL(BO.BuyerOrderNo,'') BuyerOrderNo,");
                query.Append(" ISNULL(A.ArticleID,0)ArticleID,ISNULL(AC.ArticleChallanID,0) ArticleChallanID, ISNULL(AC.ArticleChallanNo,'') ArticleChallanNo,ISNULL(ISNULL(A.ArticleName,WI.ArticleNo),'') ArticleName ,ISNULL(WI.AvgSize,0)AvgSize,");
                query.Append(" ISNULL(WI.SideDescription,'')SideDescription,ISNULL(WI.SelectionRange,'')SelectionRange,");
                query.Append(" ISNULL(WI.Thickness,'')Thickness,ISNULL(WI.ThicknessUnit,0)ThicknessUnit,ISNULL(U.UnitID,0)UnitID,ISNULL(U.UnitName,'')UnitName,ISNULL(WI.ThicknessAt,'')ThicknessAt");
                query.Append(" FROM INV_WetBlueIssueRef WI ");
                query.Append(" LEFT JOIN Sys_Buyer B ON B.BuyerID=WI.BuyerID  ");
                query.Append(" LEFT JOIN SLS_BuyerOrder BO ON BO.BuyerOrderID=WI.BuyerOrderID ");
                query.Append(" LEFT JOIN Sys_Article A ON A.ArticleID=WI.ArticleID ");
                query.Append(" LEFT JOIN Sys_ArticleChallan AC ON AC.ArticleChallanID=WI.ArticleChallanID ");
                query.Append(" LEFT JOIN Sys_Unit U ON U.UnitID=WI.ThicknessUnit ");
                query.Append(" WHERE WI.WetBlueIssueID='" + wetBlueIssueId + "'");


                model.RefModels = _context.Database.SqlQuery<InvWetBlueIssueRef>(query.ToString()).OrderBy(o => o.WetBlueIssueRefID).ToList();

                long refId = 0;
                if (model.RefModels.Count <= 0)
                {
                    refId = 0;
                }
                else
                {
                    var invWetBlueIssueRef = model.RefModels.FirstOrDefault();
                    refId = invWetBlueIssueRef == null ? 0 : invWetBlueIssueRef.WetBlueIssueRefID;
                }
                model.ItemModels = GetIssueItemsByRef(Convert.ToInt64(refId), storeId, wetBlueIssueId, issueCategory);
            }
            else
            {
                model.ItemModels = GetIssueItemsByRef(Convert.ToInt64(0), storeId, wetBlueIssueId, issueCategory);
            }
            return model;
        }

        public ICollection<InvWetBlueIssueItem> GetIssueItemsByRef(long refId, byte storeId, long wetBlueIssueId, string issueCategory)
        {
            var query = new StringBuilder();
            query.Append(" SELECT W.WBSIssueItemID,S.SupplierID,S.SupplierName,P.PurchaseID,");
            query.Append(" P.PurchaseNo,IT.ItemTypeID,IT.ItemTypeName, LS.LeatherStatusID,");
            query.Append("CASE WBS.SizeQtyRef WHEN 'SizeQty1' THEN '12-15 sft' WHEN 'SizeQty2' THEN '16-20 sft' WHEN 'SizeQty3' THEN '21-25 sft' WHEN 'SizeQty4' THEN '26-30 sft' WHEN 'SizeQty5' THEN '31-35 sft' WHEN 'SideQty' THEN 'Side' WHEN 'AreaQty' THEN 'Area' END SizeQtyRef,");
            query.Append(" LS.LeatherStatusName,G.GradeID,G.GradeName,ISNULL(W.PieceToSide,0)PcsToSide,ISNULL(WBS.ClosingStockPcs,0) ClosingStockPcs, ");
            query.Append(" ISNULL(WBS.ClosingStockSide,0)ClosingStockSide,ISNULL(WBS.ClosingStockArea,0) ClosingStockArea,W.IssuePcs,W.IssueSide,");
            query.Append(" W.IssueArea,ISNULL(W.LotNo,'') LotNo FROM dbo.INV_WetBlueStockSupplier  WBS  ");
            query.Append(" INNER JOIN (SELECT MAX(TransectionID )TransectionID,[StoreID],[SupplierID],PurchaseID,ItemTypeID,LeatherStatusID,GradeID,LeatherTypeID,SizeQtyRef ");
            query.Append(" FROM dbo.INV_WetBlueStockSupplier  GROUP BY [StoreID],[SupplierID],PurchaseID,ItemTypeID,LeatherStatusID,GradeID,LeatherTypeID,SizeQtyRef ");
            query.Append(" ) AS SUB ON SUB.TransectionID=WBS.TransectionID ");
            query.Append(" INNER JOIN SYS_Store ST ON ST.StoreID=WBS.StoreID INNER JOIN Sys_ItemType IT ON IT.ItemTypeID=WBS.ItemTypeID  ");

            query.Append(" INNER JOIN Sys_LeatherStatus LS ON LS.LeatherStatusID=WBS.LeatherStatusID INNER JOIN Sys_Grade G ON G.GradeID=WBS.GradeID  ");
            query.Append(" INNER JOIN Prq_Purchase P ON P.PurchaseID=WBS.PurchaseID ");
            query.Append(" INNER JOIN Sys_Supplier S ON S.SupplierID=WBS.SupplierID ");
            query.Append(" INNER JOIN INV_WetBlueIssueItem W ON W.SupplierID=S.SupplierID AND W.PurchaseID=P.PurchaseID AND W.ItemTypeID=IT.ItemTypeID  ");
            query.Append(" AND W.LeatherStatusID=LS.LeatherStatusID AND W.GradeID=G.GradeID  ");
            query.Append(" WHERE ST.StoreID='" + storeId + "'");

            if (issueCategory == "PROD")
            {
                if (refId != 0)
                {
                    query.Append(" AND W.WetBlueIssueRefID ='" + refId + "'");
                }
                if (wetBlueIssueId != 0)
                {
                    query.Append(" AND W.WetBlueIssueID ='" + wetBlueIssueId + "'");
                }
            }
            else
            {
                if (wetBlueIssueId != 0)
                {
                    query.Append(" AND W.WetBlueIssueID ='" + wetBlueIssueId + "'");
                }

            }
            var items = _context.Database.SqlQuery<InvWetBlueIssueItem>(query.ToString()).OrderByDescending(o => o.WBSIssueItemID);
            return items.ToList();
        }


        public ValidationMsg DeletedWetBlueIssue(long wetBlueIssueId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var refItem = _context.INV_WetBlueIssueRef.Where(m => m.WetBlueIssueID == wetBlueIssueId).Select(s => s.WetBlueIssueRefID);

                        if (refItem.Any())
                        {
                            foreach (var rId in refItem)
                            {
                                long id = rId;
                                _context.INV_WetBlueIssueItem.RemoveRange(
                                    _context.INV_WetBlueIssueItem.Where(m => m.WetBlueIssueRefID == id));
                            }

                            _context.INV_WetBlueIssueRef.RemoveRange(
                                _context.INV_WetBlueIssueRef.Where(m => m.WetBlueIssueID == wetBlueIssueId));

                            _context.INV_WetBlueIssue.RemoveRange(
                                _context.INV_WetBlueIssue.Where(m => m.WetBlueIssueID == wetBlueIssueId));
                        }
                        else
                        {
                            _context.INV_WetBlueIssueItem.RemoveRange(
                                    _context.INV_WetBlueIssueItem.Where(m => m.WetBlueIssueID == wetBlueIssueId));

                            _context.INV_WetBlueIssue.RemoveRange(
                              _context.INV_WetBlueIssue.Where(m => m.WetBlueIssueID == wetBlueIssueId));
                        }

                        _context.SaveChanges();
                    }
                    tx.Complete();
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

        public ValidationMsg DeletedWetBlueIssueRef(long wetBlueIssueRefId, string recordStatus)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    if (recordStatus.Equals("Confirmed"))
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Confirmation record can not be deleted.";
                        return _vmMsg;
                    }
                    using (_context)
                    {
                        _context.INV_WetBlueIssueItem.RemoveRange(
                           _context.INV_WetBlueIssueItem.Where(m => m.WetBlueIssueRefID == wetBlueIssueRefId));

                        _context.INV_WetBlueIssueRef.RemoveRange(
                            _context.INV_WetBlueIssueRef.Where(m => m.WetBlueIssueRefID == wetBlueIssueRefId));

                        _context.SaveChanges();
                    }
                    tx.Complete();
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
        public ValidationMsg DeletedWBSIssueItem(long wBSIssueItemId, string recordStatus)
        {
            try
            {
                if (recordStatus.Equals("Confirmed"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Confirmation record can not be deleted.";
                    return _vmMsg;
                }
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        _context.INV_WetBlueIssueItem.RemoveRange(
                            _context.INV_WetBlueIssueItem.Where(m => m.WBSIssueItemID == wBSIssueItemId));

                        _context.SaveChanges();
                    }
                    tx.Complete();
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

        public ValidationMsg CheckData(long wetBlueIssueId, string chkComment, int userId)
        {
            var updateObj = _unit.SysInvWetBlueIssueRepository.GetByID(wetBlueIssueId);
            updateObj.CheckedBy = userId;
            updateObj.CheckDate = DateTime.Now;
            updateObj.RecordStatus = "CHK";
            updateObj.IssueNote = chkComment;
            _unit.SysInvWetBlueIssueRepository.Update(updateObj);
            if (_unit.IsSaved())
            {
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Checked Successfully.";
            }
            else
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to confirm check.";
            }
            return _vmMsg;
        }

        public ValidationMsg ConfirmData(long wetBlueIssueId, string cnfComment, int userId)
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
                            cmd.CommandText = "UspConfirmWetBlueIssue";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@WetBlueIssueId", SqlDbType.Int).Value = wetBlueIssueId;
                            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
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
                            _vmMsg.Msg = "Confirmed Successfully.";
                        }
                        else
                        {
                            if (Convert.ToBoolean(dt.Rows[0]["Duplicate"]))
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "Not Confirmed due to Duplicate Entry.";
                            }
                            else
                            {
                                _vmMsg.Type = Enums.MessageType.Error;
                                _vmMsg.Msg = "Not Enough Quantity In Stock.";
                            }
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

        private void AdjustStock(INV_WetBlueIssueRef objRef, INV_WetBlueIssue objModel, IEnumerable<INV_WetBlueIssueItem> objItemlist, int userId)
        {
            var leatherTypeId = _context.Sys_LeatherType.FirstOrDefault(m => m.LeatherTypeName == "Wet Blue").LeatherTypeID;
            var currentDate = DateTime.Now.Date;

            foreach (var item in objItemlist)
            {

                #region INV_WetBlueStockSupplier Update

                var isWbSupplierStock = (from i in _context.INV_WetBlueStockSupplier.AsEnumerable()
                                         where
                                             i.SupplierID == item.SupplierID && i.PurchaseID == item.PurchaseID && i.ItemTypeID == item.ItemTypeID &&
                                             i.LeatherStatusID == item.LeatherStatusID && i.StoreID == objModel.IssueFrom
                                             && i.GradeID == item.GradeID && i.LeatherTypeID == leatherTypeId
                                         select i).Any();
                var objStockSupplier = new INV_WetBlueStockSupplier();
                if (isWbSupplierStock)
                {
                    var lastSupplierStock = (from s in _context.INV_WetBlueStockSupplier.AsEnumerable()
                                             where
                                                 s.SupplierID == item.SupplierID && s.ItemTypeID == item.ItemTypeID &&
                                                 s.LeatherStatusID == item.LeatherStatusID && s.StoreID == objModel.IssueFrom
                                                 && s.GradeID == item.GradeID && s.PurchaseID == item.PurchaseID && s.LeatherTypeID == leatherTypeId
                                             orderby s.TransectionID descending
                                             select s).FirstOrDefault();

                    decimal iPcs = item.IssuePcs ?? 0;
                    decimal iSide = item.IssueSide ?? 0;
                    decimal iArea = item.IssueArea ?? 0;

                    if ((lastSupplierStock.ClosingStockPcs < iPcs) ||
                        (lastSupplierStock.ClosingStockSide < iSide) ||
                        (lastSupplierStock.ClosingStockArea < iArea))
                    {
                        _haveStockValue = false;
                        break;
                    }

                    objStockSupplier.SupplierID = item.SupplierID;
                    objStockSupplier.ItemTypeID = item.ItemTypeID;
                    objStockSupplier.PurchaseID = item.PurchaseID;
                    objStockSupplier.LeatherStatusID = item.LeatherStatusID;
                    objStockSupplier.StoreID = objModel.IssueFrom;
                    objStockSupplier.GradeID = item.GradeID;
                    objStockSupplier.LeatherTypeID = leatherTypeId;


                    objStockSupplier.OpeningStockPcs = lastSupplierStock.ClosingStockPcs;
                    objStockSupplier.ReceiveStockPcs = 0;
                    objStockSupplier.IssueStockPcs = item.IssuePcs ?? 0; //lastSupplierStock.IssueStockPcs + item.IssuePcs;
                    objStockSupplier.ClosingStockPcs = (lastSupplierStock.ClosingStockPcs) - (item.IssuePcs ?? 0);

                    objStockSupplier.OpeningStockSide = lastSupplierStock.ClosingStockSide;
                    objStockSupplier.ReceiveStockSide = 0;
                    objStockSupplier.IssueStockSide = item.IssueSide ?? 0;
                    objStockSupplier.ClosingStockSide = (lastSupplierStock.ClosingStockSide ?? 0) - (item.IssueSide ?? 0);

                    objStockSupplier.OpeningStockArea = lastSupplierStock.ClosingStockArea;
                    objStockSupplier.ReceiveStockArea = 0;
                    objStockSupplier.IssueStockArea = item.IssueArea;
                    objStockSupplier.ClosingStockArea = (lastSupplierStock.ClosingStockArea ?? 0) - (item.IssueArea ?? 0);
                    objStockSupplier.SetOn = DateTime.Now;
                    objStockSupplier.SetBy = userId;

                    _context.INV_WetBlueStockSupplier.Add(objStockSupplier);
                    _context.SaveChanges();
                }
                else // Never Happened
                {
                    objStockSupplier.SupplierID = item.SupplierID;
                    objStockSupplier.ItemTypeID = item.ItemTypeID;
                    objStockSupplier.PurchaseID = item.PurchaseID;
                    objStockSupplier.LeatherStatusID = item.LeatherStatusID;
                    objStockSupplier.StoreID = objModel.IssueFrom;
                    objStockSupplier.GradeID = item.GradeID;
                    objStockSupplier.LeatherTypeID = leatherTypeId;

                    objStockSupplier.OpeningStockPcs = 0;
                    objStockSupplier.ReceiveStockPcs = 0;
                    objStockSupplier.IssueStockPcs = item.IssuePcs;
                    objStockSupplier.ClosingStockPcs = item.IssuePcs;

                    objStockSupplier.OpeningStockSide = 0;
                    objStockSupplier.ReceiveStockSide = 0;
                    objStockSupplier.IssueStockSide = item.IssueSide;
                    objStockSupplier.ClosingStockSide = item.IssueSide;

                    objStockSupplier.OpeningStockArea = 0;
                    objStockSupplier.ReceiveStockArea = 0;
                    objStockSupplier.IssueStockArea = item.IssueArea;
                    objStockSupplier.ClosingStockArea = item.IssueArea;

                    _context.INV_WetBlueStockSupplier.Add(objStockSupplier);
                    _context.SaveChanges();
                }

                #endregion //End INV_WetBlueStockSupplier

                #region  INV_WetBlueItemStock Update

                var wbItemStock = (from i in _context.INV_WetBlueStockItem.AsEnumerable()
                                   where
                                       i.ItemTypeID == item.ItemTypeID && i.LeatherStatusID == item.LeatherStatusID &&
                                       i.StoreID == objModel.IssueFrom && i.LeatherTypeID == leatherTypeId
                                       && i.GradeID == item.GradeID
                                   select i).Any();
                var objItemStock = new INV_WetBlueStockItem();
                if (wbItemStock)
                {
                    var lastItemStock = (from s in _context.INV_WetBlueStockItem.AsEnumerable()
                                         where
                                             s.ItemTypeID == item.ItemTypeID &&
                                             s.LeatherStatusID == item.LeatherStatusID && s.StoreID == objModel.IssueFrom
                                             && s.GradeID == item.GradeID && s.LeatherTypeID == leatherTypeId
                                         orderby s.TransectionID descending
                                         select s).FirstOrDefault();


                    objItemStock.ItemTypeID = item.ItemTypeID;
                    objItemStock.LeatherStatusID = item.LeatherStatusID;
                    objItemStock.StoreID = objModel.IssueFrom;
                    objItemStock.GradeID = item.GradeID;
                    objItemStock.LeatherTypeID = leatherTypeId;

                    objItemStock.OpeningStockPcs = lastItemStock.OpeningStockPcs;
                    objItemStock.ReceiveStockPcs = 0;
                    objItemStock.IssueStockPcs = item.IssuePcs ?? 0;
                    objItemStock.ClosingStockPcs = lastItemStock.ClosingStockPcs - (item.IssuePcs ?? 0);

                    objItemStock.OpeningStockSide = lastItemStock.ClosingStockSide;
                    objItemStock.ReceiveStockSide = 0;
                    objItemStock.IssueStockSide = item.IssueSide ?? 0;
                    objItemStock.ClosingStockSide = lastItemStock.ClosingStockSide - (item.IssueSide ?? 0);

                    objItemStock.OpeningStockArea = lastItemStock.ClosingStockArea;
                    objItemStock.ReceiveStockArea = 0;
                    objItemStock.IssueStockArea = item.IssueArea ?? 0;
                    objItemStock.ClosingStockArea = lastItemStock.ClosingStockArea - (item.IssueArea ?? 0);

                    objStockSupplier.SetOn = DateTime.Now;
                    objStockSupplier.SetBy = userId;
                    _context.INV_WetBlueStockItem.Add(objItemStock);
                    _context.SaveChanges();
                }
                else // Never Happened
                {
                    objItemStock.ItemTypeID = item.ItemTypeID;
                    objItemStock.LeatherStatusID = item.LeatherStatusID;
                    objItemStock.StoreID = objModel.IssueFrom;
                    objItemStock.GradeID = item.GradeID;
                    objItemStock.LeatherTypeID = leatherTypeId;

                    objItemStock.OpeningStockPcs = 0;
                    objItemStock.ReceiveStockPcs = 0;
                    objItemStock.IssueStockPcs = item.IssuePcs;
                    objItemStock.ClosingStockPcs = item.IssuePcs;

                    objItemStock.OpeningStockSide = 0;
                    objItemStock.ReceiveStockSide = 0;
                    objItemStock.IssueStockSide = item.IssueSide;
                    objItemStock.ClosingStockSide = item.IssueSide;

                    objItemStock.OpeningStockArea = 0;
                    objItemStock.ReceiveStockArea = 0;
                    objItemStock.IssueStockArea = item.IssueArea;
                    objItemStock.ClosingStockArea = item.IssueArea;

                    _context.INV_WetBlueStockItem.Add(objItemStock);
                    _context.SaveChanges();
                }

                #endregion

                #region              INV_WetBlueStockDaily Update


                var checkDate = (from ds in _context.INV_WetBlueStockDaily.AsEnumerable()
                                 where ds.ItemTypeID == item.ItemTypeID &&
                                       ds.StoreID == objModel.IssueFrom
                                       && ds.LeatherStatusID == item.LeatherStatusID && ds.LeatherTypeID == leatherTypeId &&
                                       ds.GradeID == item.GradeID && ds.StockDate == currentDate
                                 select ds).Any();

                if (checkDate)
                {
                    var lastDailyStock = (from s in _context.INV_WetBlueStockDaily.AsEnumerable()
                                          where
                                              s.ItemTypeID == item.ItemTypeID &&
                                              s.LeatherStatusID == item.LeatherStatusID && s.StoreID == objModel.IssueFrom
                                              && s.GradeID == item.GradeID && s.StockDate == currentDate && s.LeatherTypeID == leatherTypeId
                                          orderby s.TransectionID descending
                                          select s).FirstOrDefault();


                    var objDailyStock = new INV_WetBlueStockDaily();

                    objDailyStock.IssueStockPcs = lastDailyStock.IssueStockPcs + (item.IssuePcs ?? 0);
                    objDailyStock.ClosingStockPcs = lastDailyStock.ClosingStockPcs - (item.IssuePcs ?? 0);

                    objDailyStock.IssueStockSide = lastDailyStock.IssueStockSide + (item.IssueSide ?? 0);
                    objDailyStock.ClosingStockSide = lastDailyStock.ClosingStockSide - (item.IssueSide ?? 0);

                    objDailyStock.IssueStockArea = lastDailyStock.IssueStockArea + (item.IssueArea ?? 0);
                    objDailyStock.ClosingStockArea = lastDailyStock.ClosingStockArea - (item.IssueArea ?? 0);

                    _context.SaveChanges();
                }
                else
                {
                    var previousRecord = (from ds in _context.INV_WetBlueStockDaily.AsEnumerable()
                                          where ds.ItemTypeID == item.ItemTypeID &&
                                                ds.StoreID == objModel.IssueFrom
                                                && ds.LeatherStatusID == item.LeatherStatusID && ds.LeatherTypeID == leatherTypeId &&
                                                ds.GradeID == item.GradeID
                                          orderby ds.StockDate
                                          select ds).LastOrDefault();

                    var objStockDaily = new INV_WetBlueStockDaily();

                    objStockDaily.ItemTypeID = item.ItemTypeID;
                    objStockDaily.StoreID = Convert.ToByte(objModel.IssueFrom);
                    objStockDaily.LeatherStatusID = item.LeatherStatusID;
                    objStockDaily.GradeID = item.GradeID;
                    objStockDaily.LeatherTypeID = leatherTypeId;
                    objStockDaily.OpeningStockPcs = (previousRecord == null ? 0 : previousRecord.ClosingStockPcs);
                    objStockDaily.ReceiveStockPcs = 0;
                    objStockDaily.IssueStockPcs = item.IssuePcs ?? 0;
                    objStockDaily.ClosingStockPcs = (previousRecord == null ? (item.IssuePcs ?? 0) : objStockDaily.OpeningStockPcs - (item.IssuePcs ?? 0));
                    //objStockDaily.ClosingStockPcs = item.IssuePcs;

                    objStockDaily.OpeningStockSide = (previousRecord == null ? 0 : previousRecord.ClosingStockSide);
                    objStockDaily.ReceiveStockSide = 0;
                    objStockDaily.IssueStockSide = item.IssueSide ?? 0;
                    objStockDaily.ClosingStockSide = (previousRecord == null ? (item.IssueSide ?? 0) : objStockDaily.OpeningStockSide - (item.IssueSide ?? 0));
                    //objStockDaily.ClosingStockSide = item.IssueSide;

                    objStockDaily.OpeningStockArea = (previousRecord == null ? 0 : previousRecord.ClosingStockArea);
                    objStockDaily.ReceiveStockArea = 0;
                    objStockDaily.IssueStockArea = item.IssueArea ?? 0;
                    //objStockDaily.ClosingStockArea = item.IssueArea;
                    objStockDaily.ClosingStockArea = (previousRecord == null ? (item.IssueArea ?? 0) : objStockDaily.OpeningStockArea - (item.IssueArea ?? 0));


                    objStockDaily.StockDate = currentDate;

                    _context.INV_WetBlueStockDaily.Add(objStockDaily);
                    _context.SaveChanges();
                }

                #endregion

                #region Insert into PRD_CrustLeatherProductionStock for Crust Process

                var leatherTypeIdForCrust =
                    _context.Sys_LeatherType.FirstOrDefault(m => m.LeatherTypeName == "Crust").LeatherTypeID;
                var isClps = (from c in _context.PRD_CrustLeatherProductionStock.AsEnumerable()
                              where
                                  c.StoreID == objModel.IssueTo && c.BuyerID == objRef.BuyerID && c.BuyerOrderID == objRef.BuyerOrderID &&
                                  c.ArticleID == objRef.ArticleID
                                  && c.ItemTypeID == item.ItemTypeID && c.LeatherTypeID == leatherTypeIdForCrust
                                  && c.LeatherStatusID == item.LeatherStatusID && c.ArticleChallanID == objRef.ArticleChallanID //c.GradeID == item.GradeID
                              select c).Any();

                var objClps = new PRD_CrustLeatherProductionStock();
                if (isClps)
                {
                    var lastClpStock = (from s in _context.PRD_CrustLeatherProductionStock.AsEnumerable()
                                        where
                                            s.StoreID == objModel.IssueTo && s.BuyerID == objRef.BuyerID &&
                                            s.BuyerOrderID == objRef.BuyerOrderID && s.ArticleID == objRef.ArticleID
                                            && s.ItemTypeID == item.ItemTypeID && s.LeatherTypeID == leatherTypeIdForCrust
                                            && s.LeatherStatusID == item.LeatherStatusID && s.ArticleChallanID == objRef.ArticleChallanID //s.GradeID == item.GradeID
                                        orderby s.TransectionID descending
                                        select s).FirstOrDefault();


                    if (lastClpStock == null) continue;

                    objClps.StoreID = (byte)objModel.IssueTo;
                    objClps.BuyerID = objRef.BuyerID;
                    objClps.BuyerOrderID = objRef.BuyerOrderID;
                    objClps.ArticleChallanNo = objRef.ArticleChallanNo;
                    objClps.ArticleChallanID = objRef.ArticleChallanID;
                    objClps.ArticleID = objRef.ArticleID;
                    objClps.ItemTypeID = item.ItemTypeID;
                    objClps.LeatherTypeID = leatherTypeIdForCrust;
                    objClps.LeatherStatusID = item.LeatherStatusID;
                    //objClps.GradeID = item.GradeID;

                    objClps.GradeRange = objRef.SelectionRange;

                    objClps.OpeningPcs = lastClpStock.ClosingProductionPcs ?? 0;
                    objClps.ReceivePcs = item.IssuePcs ?? 0;
                    objClps.IssuePcs = 0;
                    objClps.ClosingProductionPcs = (lastClpStock.ClosingProductionPcs ?? 0) + (item.IssuePcs ?? 0);

                    objClps.OpeningSide = lastClpStock.ClosingProductionSide ?? 0;
                    objClps.ReceiveSide = item.IssueSide ?? 0;
                    objClps.IssueSide = 0;
                    objClps.ClosingProductionSide = (lastClpStock.ClosingProductionSide ?? 0) + (item.IssueSide ?? 0);

                    objClps.OpeningArea = lastClpStock.ClosingProductionArea ?? 0;
                    objClps.ReceiveArea = item.IssueArea ?? 0;
                    objClps.IssueArea = 0;
                    objClps.ClosingProductionArea = (lastClpStock.ClosingProductionArea ?? 0) + (item.IssueArea ?? 0);
                    objClps.QCStatus = "REP";

                    objStockSupplier.SetOn = DateTime.Now;
                    objStockSupplier.SetBy = userId;
                    _context.PRD_CrustLeatherProductionStock.Add(objClps);
                    _context.SaveChanges();
                }
                else
                {
                    objClps.StoreID = (byte)objModel.IssueTo;
                    objClps.BuyerID = objRef.BuyerID;
                    objClps.BuyerOrderID = objRef.BuyerOrderID;
                    objClps.ArticleChallanNo = objRef.ArticleChallanNo;
                    objClps.ArticleChallanID = objRef.ArticleChallanID;
                    objClps.ArticleID = objRef.ArticleID;
                    objClps.ItemTypeID = item.ItemTypeID;
                    objClps.LeatherTypeID = leatherTypeIdForCrust;
                    objClps.LeatherStatusID = item.LeatherStatusID;
                    //objClps.GradeID = item.GradeID;
                    objClps.GradeRange = objRef.SelectionRange;

                    objClps.OpeningPcs = 0;
                    objClps.ReceivePcs = item.IssuePcs ?? 0;
                    objClps.IssuePcs = 0;
                    objClps.ClosingProductionPcs = item.IssuePcs ?? 0;

                    objClps.OpeningSide = 0;
                    objClps.ReceiveSide = item.IssueSide ?? 0;
                    objClps.IssueSide = 0;
                    objClps.ClosingProductionSide = item.IssueSide ?? 0;

                    objClps.OpeningArea = 0;
                    objClps.ReceiveArea = item.IssueArea ?? 0;
                    objClps.IssueArea = 0;
                    objClps.ClosingProductionArea = item.IssueArea ?? 0;

                    objClps.QCStatus = "REP";
                    objClps.SetOn = DateTime.Now;
                    objClps.SetBy = userId;

                    _context.PRD_CrustLeatherProductionStock.Add(objClps);
                    _context.SaveChanges();
                }

                #endregion
            }
        }
        //List<InvWetBlueIssueItem> GetWetBlueStockForCrust(byte storeId)
        public List<InvWetBlueIssueItem> GetWetBlueStockForCrust(byte storeId, string gradeName, string supplierName, string purchaseNo)
        {
            #region Old Code

            //var query = new StringBuilder();
            //using (_context)
            //{
            //    query.Append(" SELECT  G.GradeID,G.GradeName,S.SupplierID,S.SupplierName,ST.StoreID,St.StoreName, WBS.PurchaseID,P.PurchaseNo, WBS.ItemTypeID,IT.ItemTypeName, WBS.LeatherStatusID,LS.LeatherStatusName, ");
            //    query.Append(" ISNULL(WBS.ClosingStockPcs,0) ClosingStockPcs, ISNULL(WBS.ClosingStockSide,0)ClosingStockSide,ISNULL(WBS.ClosingStockArea,0) ClosingStockArea, ");
            //    query.Append(" CAST( ROUND(ISNULL(WBS.ClosingStockArea,0)/ IIF( ISNULL(WBS.ClosingStockPcs,0) + (ISNULL(WBS.ClosingStockSide,0)/2)=0,1,ISNULL(WBS.ClosingStockPcs,0) + (ISNULL(WBS.ClosingStockSide,0)/2)),2,2) AS decimal(18,2)) AverageStockArea    FROM dbo.INV_WetBlueStockSupplier WBS  ");
            //    query.Append(" INNER JOIN ( SELECT MAX(TransectionID) TransectionID,[StoreID],[SupplierID],PurchaseID,ItemTypeID,LeatherStatusID,GradeID,LeatherTypeID FROM dbo.INV_WetBlueStockSupplier ");
            //    query.Append(" WHERE  StoreID='" + storeId + "' ");
            //    query.Append(" GROUP BY [StoreID],[SupplierID],PurchaseID,ItemTypeID,LeatherStatusID,GradeID,LeatherTypeID  ");
            //    query.Append(" ) SUB  ON SUB.TransectionID=WBS.TransectionID ");
            //    query.Append(" INNER JOIN SYS_Store ST ON ST.StoreID=WBS.StoreID  ");
            //    query.Append(" INNER JOIN SYS_Supplier S ON S.SupplierID=WBS.SupplierID  ");
            //    query.Append(" INNER JOIN Sys_ItemType IT ON IT.ItemTypeID=WBS.ItemTypeID INNER JOIN Sys_LeatherStatus LS ON LS.LeatherStatusID=WBS.LeatherStatusID  ");
            //    query.Append(" INNER JOIN Sys_Grade G ON G.GradeID=WBS.GradeID  INNER JOIN Prq_Purchase P ON P.PurchaseID=WBS.PurchaseID  ");
            //    query.Append(" Where (WBS.ClosingStockPcs > 0 OR WBS.ClosingStockSide > 0 OR WBS.ClosingStockArea > 0) ");
            //    if (!string.IsNullOrEmpty(gradeName))
            //    {
            //        query.Append(" AND G.GradeName like '%" + gradeName + "%' ");
            //    }
            //    query.Append(" ORDER BY G.GradeName ");

            //    var items = _context.Database.SqlQuery<InvWetBlueIssueItem>(query.ToString());
            //    return items.ToList();
            //}
            #endregion

            #region Cache Mechanism
            //var dtCache = new DataTable();
            //if (HttpContext.Current.Cache["WBStockData"] != null)
            //{
            //    dtCache= (DataTable) HttpContext.Current.Cache["WBStockData"];
            //}
            //else
            //{
            //    using (var conn = new SqlConnection(_connString))
            //    {
            //        conn.Open();
            //        using (var cmd = new SqlCommand())
            //        {
            //            cmd.Connection = conn;
            //            cmd.CommandText = "GetWetBlueStockDataForCrust";
            //            cmd.CommandType = CommandType.StoredProcedure;
            //            cmd.Parameters.Add("@StoreId", SqlDbType.Int).Value = storeId;
            //            using (var adp = new SqlDataAdapter(cmd))
            //            {
            //                adp.Fill(dtCache);
            //            }
            //            HttpContext.Current.Cache.Add("WBStockData", dtCache, null, DateTime.Now.AddMinutes(2), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            //        }
            //    }
            //}
            //return GenericCommon.ConvertDataTableToList<InvWetBlueIssueItem>(dtCache); 
            #endregion
            var items = _context.Database.SqlQuery<InvWetBlueIssueItem>("EXEC FUspGetWetBlueStockForCrust @StoreID={0},@GradeName={1},@SupplierName={2},@PurchaseNo={3}", storeId, gradeName,supplierName,purchaseNo);
            return items.ToList();
        }

        public IEnumerable<SupplierDetails> GetSupplierListByStoreId(byte storeId)
        {
            var query = new StringBuilder();
            using (_context)
            {
                query.Append(" SELECT S.SupplierID,S.SupplierCode,S.SupplierName ,SA.Address,SA.ContactNumber,SA.ContactPerson FROM [INV_WetBlueStockSupplier] WBS ");
                query.Append(" INNER JOIN Sys_Supplier S ON S.SupplierID=WBS.SupplierID");
                query.Append(" LEFT JOIN Sys_SupplierAddress SA ON SA.SupplierID=S.SupplierID");
                query.Append(" WHERE WBS.StoreID='" + storeId + "' AND SA.IsActive=1 AND SA.IsDelete=0 AND S.IsActive=1");
                query.Append(" GROUP BY S.SupplierID,S.SupplierCode,S.SupplierName,SA.Address,SA.ContactNumber,SA.ContactPerson");
                var items = _context.Database.SqlQuery<SupplierDetails>(query.ToString());
                return items.ToList().OrderBy(o => o.SupplierName);
            }

        }

        public IEnumerable<BuyerOrderItem> GetAllSLSBuyerOrderOG(int buyerId)
        {
            var query = new StringBuilder();
            using (_context)
            {
                query.Append(" SELECT BO.BuyerOrderID,BO.BuyerOrderNo OrderNo,BO.OrderNo BuyerOrderNo,I.ArticleID,I.ArticleNo,A.ArticleName FROM SLS_BuyerOrder BO  ");
                query.Append(" LEFT JOIN SLS_BuyerOrderItem I ON I.BuyerOrderID=BO.BuyerOrderID");
                query.Append(" LEFT JOIN Sys_Article A ON A.ArticleID=I.ArticleID");
                query.Append(" WHERE BO.BuyerID='" + buyerId + "' AND BO.BuyerOrderStatus='OD'");
                var items = _context.Database.SqlQuery<BuyerOrderItem>(query.ToString());
                return items.ToList().OrderByDescending(o => o.BuyerOrderID);
            }
        }

        public IEnumerable<InvWetBlueIssueRef> GetAllReqBuyerItems(long requisitionId)
        {
            var query = new StringBuilder();
            using (this._context)
            {
                query.Append(" SELECT B.BuyerID,B.BuyerName,ISNULL(BO.BuyerOrderID,0) BuyerOrderID,BO.BuyerOrderNo,ISNULL(AC.ArticleChallanID,0)ArticleChallanID,AC.ArticleChallanNo, ");
                query.Append(" ISNULL(A.ArticleID,0)ArticleID,A.ArticleName,R.AvgSize,ISNULL(AU.UnitID,0) AvgSizeUnit,AU.UnitName AvgSizeUnitName,R.SideDescription,");
                query.Append(" R.SelectionRange,R.Thickness,ISNULL(TU.UnitID,0) UnitID,TU.UnitName,CASE WHEN  R.ThicknessAt='AFSV' THEN 'After Shaving' ELSE 'After Finishing' END ThicknessAt, R.Remark Remarks");
                query.Append(" FROM PRD_YearMonthCrustReqItem R");
                query.Append(" LEFT JOIN Sys_Buyer B ON B.BuyerID=R.BuyerID");
                query.Append(" LEFT JOIN SLS_BuyerOrder BO ON BO.BuyerOrderID=R.BuyerOrderID");
                query.Append(" LEFT JOIN Sys_ArticleChallan AC ON AC.ArticleChallanID=R.ArticleChallanID");
                query.Append(" LEFT JOIN Sys_Article A ON A.ArticleID=AC.ArticleID");
                query.Append(" INNER JOIN Sys_Unit AU ON AU.UnitID=R.AvgSizeUnit");
                query.Append(" INNER  JOIN Sys_Unit TU ON TU.UnitID=R.ThicknessUnit");
                query.Append(" WHERE RequisitionDateID='" + requisitionId + "'");

                var items = _context.Database.SqlQuery<InvWetBlueIssueRef>(query.ToString());
                return items.ToList().OrderByDescending(o => o.ArticleChallanID);
            }
        }
        public IEnumerable<PRDYearMonthCrustReqDate> GetAllRequisitionForCrust()
        {
            var query = new StringBuilder();
            using (_context)
            {
                query.Append(" SELECT T1.RequisitionDateID,T1.RequisitionNo,T1.RequisitionStatus,T1.RequiredDate,T1.RecordStatus,T1.ReqPcs,T1.ReqSide,T1.ReqArea,");
                query.Append(" ISNULL(T2.IssuePcs,0) IssuePcs,ISNULL(T2.IssueSide,0) IssueSide,ISNULL(T2.IssueArea,0) IssueArea,");
                query.Append(" CASE WHEN (T1.ReqPcs-ISNULL(T2.IssuePcs,0)) < 0 THEN 0 ELSE T1.ReqPcs-ISNULL(T2.IssuePcs,0) END  RemainPcs,");
                query.Append(" CASE WHEN  (T1.ReqSide-ISNULL(T2.IssueSide,0)) < 0 THEN 0 ELSE  (T1.ReqSide-ISNULL(T2.IssueSide,0)) END  RemainSide, CASE WHEN  (T1.ReqArea-ISNULL(T2.IssueArea,0)) < 0 THEN 0 ELSE  (T1.ReqArea-ISNULL(T2.IssueArea,0)) END RemainArea FROM (");
                query.Append(" SELECT YMC.RequisitionDateID,ISNULL(YMC.RequisitionNo,'')RequisitionNo,ISNULL(YMC.RequisitionStatus,'')RequisitionStatus,CONVERT(NVARCHAR(12),YMC.RequiredDate,106)RequiredDate,");
                query.Append(" CASE  YMC.RecordStatus WHEN 'CNF' THEN 'Confirmed' END RecordStatus ,ISNULL(SUM(CRIC.ColorPcs),0) ReqPcs,ISNULL(SUM(CRIC.ColorSide),0)ReqSide ,ISNULL(SUM(CRIC.ColorArea),0) ReqArea FROM PRD_YearMonthCrustReqDate  YMC");
                query.Append(" INNER JOIN PRD_YearMonthCrustReqItem CRI ON CRI.RequisitionDateID=YMC.RequisitionDateID");
                query.Append(" INNER JOIN PRD_YearMonthCrustReqItemColor CRIC ON CRIC.RequisitionItemID=CRI.RequisitionItemID");
                query.Append(" WHERE YMC.RecordStatus ='CNF'");
                query.Append(" GROUP BY YMC.RequisitionDateID,YMC.RequisitionNo,YMC.RequisitionStatus,YMC.RequiredDate,YMC.RecordStatus) T1");
                query.Append(" LEFT JOIN (");
                query.Append(" SELECT I.RequisitionDateID, ISNULL(SUM(II.IssuePcs),0)IssuePcs,ISNULL(SUM(II.IssueSide),0)IssueSide, ISNULL(SUM(II.IssueArea),0) IssueArea FROM INV_WetBlueIssue I");
                query.Append(" INNER JOIN INV_WetBlueIssueRef IR ON IR.WetBlueIssueID=I.WetBlueIssueID");
                query.Append(" INNER JOIN INV_WetBlueIssueItem II ON II.WetBlueIssueRefID=IR.WetBlueIssueRefID");
                query.Append(" GROUP BY I.RequisitionDateID) T2");
                query.Append(" ON T2.RequisitionDateID=T1.RequisitionDateID");
                query.Append(" WHERE (T1.ReqPcs-ISNULL(T2.IssuePcs,0) > 0 OR T1.ReqSide-ISNULL(T2.IssueSide,0)>0 OR T1.ReqArea-ISNULL(T2.IssueArea,0)>0 )");

                var items = _context.Database.SqlQuery<PRDYearMonthCrustReqDate>(query.ToString());
                return items.ToList().OrderByDescending(o => o.RequisitionDateID);
            }
        }
        public IEnumerable<SysArticleChallan> GetAllActiveChallanArticle()
        {
            var query = new StringBuilder();
            using (_context)
            {
                query.Append(" SELECT AC.ArticleChallanID, AC.ArticleChallanNo, A.ArticleID, A.ArticleName,B.BuyerID,B.BuyerName FROM Sys_ArticleChallan AC  ");
                query.Append(" INNER JOIN Sys_Article A ON A.ArticleID=AC.ArticleID");
                query.Append(" INNER JOIN Sys_Buyer B ON B.BuyerID=AC.BuyerID ");
                query.Append(" WHERE AC.IsActive=1");
                var items = _context.Database.SqlQuery<SysArticleChallan>(query.ToString());
                return items.ToList().OrderByDescending(o => o.ArticleChallanID);
            }
        }

        public IEnumerable<PRDYearMonthCrustReqItemColor> GetAllRequisitionItemsForCrust(long requisitionId)
        {
            using (_context)
            {
                var query = new StringBuilder();
                query.Append(" SELECT  CRIC.ReqItemColorID,C.ColorID,C.ColorName,  ");
                query.Append(" ISNULL((CRIC.ColorPcs),0) ReqPcs,ISNULL((CRIC.ColorSide),0)ReqSide ,ISNULL((CRIC.ColorArea),0) ReqArea");
                query.Append(" FROM PRD_YearMonthCrustReqDate  YMC");
                query.Append(" INNER JOIN PRD_YearMonthCrustReqItem CRI ON CRI.RequisitionDateID=YMC.RequisitionDateID");
                query.Append(" INNER JOIN PRD_YearMonthCrustReqItemColor CRIC ON CRIC.RequisitionItemID=CRI.RequisitionItemID");
                query.Append(" INNER JOIN Sys_Color C ON C.ColorID=CRIC.ColorID");
                query.Append(" WHERE YMC.RecordStatus ='CNF' ");
                if (requisitionId != 0)
                {
                    query.Append(" AND YMC.RequisitionDateID ='" + requisitionId + "'");
                }
                var items = _context.Database.SqlQuery<PRDYearMonthCrustReqItemColor>(query.ToString());
                return items.ToList().OrderBy(o => o.ReqItemColorID);
            }
        }

        public InvWetBlueIssueRef GetChallanAllInfo(long articleChallanId)
        {
            var query = new StringBuilder();
            using (_context)
            {
                query.Append(" SELECT  AC.ArticleChallanID, AC.ArticleChallanNo, A.ArticleID, A.ArticleName,B.BuyerID,B.BuyerName,  ");
                query.Append(" CD.SizeRange AvgSize,CD.PcsSideDescription SideDescription, ISNULL(CD.GradeRange,'') SelectionRange,CD.ThicknessRange Thickness,CD.ThicknessUnit UnitID,");
                query.Append(" U.UnitName,CD.ThicknessAt ThicknessAtID, IIF(CD.ThicknessAt='AFSV','After Shaving','After Finishing') ThicknessAt ");
                query.Append(" FROM Sys_ArticleChallan AC ");
                query.Append(" INNER JOIN Sys_ArticleChallanDetail CD ON CD.ArticleChallanID=AC.ArticleChallanID ");
                query.Append(" INNER JOIN Sys_Article A ON A.ArticleID=AC.ArticleID ");
                query.Append(" INNER JOIN Sys_Buyer B ON B.BuyerID=AC.BuyerID ");
                query.Append(" INNER JOIN Sys_Unit U ON U.UnitID=CD.ThicknessUnit ");
                query.Append(" WHERE AC.IsActive=1 AND AC.ArticleChallanID='" + articleChallanId + "'");
                var items = _context.Database.SqlQuery<InvWetBlueIssueRef>(query.ToString());
                return items.First();
            }
        }

        public IEnumerable<SysArticleChallan> GetAllActiveChallanArticleWithoutBuyer()
        {
            var query = new StringBuilder();
            using (_context)
            {
                query.Append(" SELECT AC.ArticleChallanID, AC.ArticleChallanNo, A.ArticleID, A.ArticleName FROM Sys_ArticleChallan AC  ");
                query.Append(" INNER JOIN Sys_Article A ON A.ArticleID=AC.ArticleID");
                query.Append(" WHERE AC.IsActive=1");
                var items = _context.Database.SqlQuery<SysArticleChallan>(query.ToString());
                return items.ToList().OrderByDescending(o => o.ArticleChallanID);
            }
        }
        public IEnumerable<SysArticle> GetAllActiveArticle()
        {
            var query = new StringBuilder();
            using (_context)
            {
                query.Append(" SELECT  ArticleID, ArticleNo, ArticleName ");
                query.Append(" FROM Sys_Article WHERE IsActive=1");
                var items = _context.Database.SqlQuery<SysArticle>(query.ToString());
                return items.ToList().OrderBy(o => o.ArticleName);
            }
        }
    }
}