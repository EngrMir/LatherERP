using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalInvFinishedLeatherIssueFromStore
    {
        private BLC_DEVEntities _context;
        private int _mode;
        private ValidationMsg _vmMsg;
        private UnitOfWork _unit;


        public DalInvFinishedLeatherIssueFromStore()
        {
            _context = new BLC_DEVEntities();
            _unit = new UnitOfWork();
            _vmMsg = new ValidationMsg();

        }
        public object GetOrderByBuyer(byte storeId, long buyerId)
        {
            var query = "SELECT inv.TransectionID, inv.BuyerID, (SELECT BuyerName FROM Sys_Buyer WHERE (BuyerID = inv.BuyerID)) AS BuyerName, inv.BuyerOrderID,(SELECT BuyerOrderNo FROM SLS_BuyerOrder WHERE (BuyerOrderID = inv.BuyerOrderID)) AS BuyerOrderNo, inv.ItemTypeID, (SELECT ItemTypeName FROM Sys_ItemType WHERE (ItemTypeID = inv.ItemTypeID)) AS ItemTypeName, inv.LeatherTypeID, (SELECT LeatherTypeName FROM Sys_LeatherType WHERE (LeatherTypeID = inv.LeatherTypeID)) AS LeatherTypeName, inv.LeatherStatusID, (SELECT LeatherStatusName FROM Sys_LeatherStatus WHERE (LeatherStatusID = inv.LeatherStatusID)) AS LeatherStatusName, inv.ArticleID, inv.ArticleNo, (SELECT ArticleName FROM Sys_Article WHERE (ArticleID = inv.ArticleID)) AS ArticleName, inv.ClosingStockPcs, inv.ClosingStockSide, inv.ClosingStockArea FROM INV_FinishBuyerStock AS inv INNER JOIN (SELECT MAX(TransectionID) AS TransectionID, StoreID, BuyerID, BuyerOrderID, ItemTypeID, LeatherTypeID, LeatherStatusID, ArticleID FROM INV_FinishBuyerStock GROUP BY StoreID, BuyerID, BuyerOrderID, ItemTypeID, LeatherTypeID, LeatherStatusID, ArticleID) AS sup ON inv.TransectionID = sup.TransectionID WHERE (inv.StoreID = " + storeId + ") AND (inv.BuyerID =" + buyerId + ") AND (inv.ClosingStockPcs > 0)";
            var orders = _context.Database.SqlQuery<InvFinishedLeatherIssueItem>(query).ToList();
            return orders;
        }

        public object GetColorByComb(byte storeId, long buyerId, long orderId, byte itemTypeId, byte leatherTypeId, byte leatherStatusID, int articleId)
        {
            var query = "select inv.TransectionID, inv.ColorID,(select ColorName from dbo.Sys_Color where ColorID = inv.ColorID)ColorName, inv.GradeID,(select GradeName from dbo.Sys_Grade where GradeID = inv.GradeID)GradeName, inv.ClosingStockPcs, inv.ClosingStockSide, inv.ClosingStockArea, inv.FinishQCLabel from dbo.INV_FinishBuyerStock inv INNER JOIN (select MAX(TransectionID)TransectionID,StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID from dbo.INV_FinishBuyerStock group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID) sup ON inv.TransectionID=sup.TransectionID where inv.StoreID = " + storeId + " and inv.BuyerID = " + buyerId + " and inv.BuyerOrderID = " + orderId + " and inv.ItemTypeID = " + itemTypeId + " and inv.LeatherTypeID = " + leatherTypeId + " and inv.LeatherStatusID = " + leatherStatusID + " and inv.ArticleID = " + articleId + " and inv.ClosingStockPcs>0";
            var colors = _context.Database.SqlQuery<InvFinishedLeatherIssueColor>(query).ToList();
            return colors;
        }

        public object GetBuyerPass(int storeId)
        {
            var buyers = _unit.FinishBuyerStock.Get().Where(ob => ob.StoreID == storeId && ob.FinishQCLabel == "PASS").Select(ob => new
            {
                ob.BuyerID,
                _unit.SysBuyerRepository.GetByID(ob.BuyerID).BuyerCode,
                _unit.SysBuyerRepository.GetByID(ob.BuyerID).BuyerName,
                _unit.SysBuyerRepository.GetByID(ob.BuyerID).BuyerType,
                BuyerAddressId = _unit.BuyerAddressRepository.Get().FirstOrDefault(oc => oc.BuyerID == ob.BuyerID).BuyerAddressID,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(oc => oc.BuyerID == ob.BuyerID).Address
            }).ToList();

            return buyers.OrderBy(ob => ob.BuyerName);
        }

        public object GetBuyerFail(int storeId)
        {
            var buyers =
                    _unit.FinishBuyerStock.Get()
                        .Where(ob => ob.StoreID == storeId && ob.FinishQCLabel == "FAIL")
                        .Select(ob => new
                        {
                            ob.BuyerID,
                            _unit.SysBuyerRepository.GetByID(ob.BuyerID).BuyerCode,
                            _unit.SysBuyerRepository.GetByID(ob.BuyerID).BuyerName,
                            _unit.SysBuyerRepository.GetByID(ob.BuyerID).BuyerType,
                            BuyerAddressId =
                                _unit.BuyerAddressRepository.Get()
                                    .FirstOrDefault(oc => oc.BuyerID == ob.BuyerID)
                                    .BuyerAddressID,
                            _unit.BuyerAddressRepository.Get().FirstOrDefault(oc => oc.BuyerID == ob.BuyerID).Address
                        }).ToList();
            return buyers.OrderBy(ob => ob.BuyerName);
        }

        public object GetBuyerAll(int storeId)
        {
            var buyers =
                   _unit.FinishBuyerStock.Get()
                       .Where(ob => ob.StoreID == storeId)
                       .Select(ob => new
                       {
                           ob.BuyerID,
                           _unit.SysBuyerRepository.GetByID(ob.BuyerID).BuyerCode,
                           _unit.SysBuyerRepository.GetByID(ob.BuyerID).BuyerName,
                           _unit.SysBuyerRepository.GetByID(ob.BuyerID).BuyerType,
                           BuyerAddressId =
                               _unit.BuyerAddressRepository.Get()
                                   .FirstOrDefault(oc => oc.BuyerID == ob.BuyerID)
                                   .BuyerAddressID,
                           _unit.BuyerAddressRepository.Get().FirstOrDefault(oc => oc.BuyerID == ob.BuyerID).Address
                       }).ToList();
            return buyers.OrderBy(ob => ob.BuyerName);
        }

        public ValidationMsg Save(InvFinishLeatherIssue model, int userId, string url)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var issue = ConvertIssue(model, userId, url);

                        if (model.FinishLeatherIssueID == 0)
                        {
                            _context.INV_FinishLeatherIssue.Add(issue);
                            _context.SaveChanges();
                            _mode = 1;
                        }
                        else
                        {
                            _context.SaveChanges();
                            _mode = 2;
                        }
                        if (model.Items != null)
                        {
                            foreach (var item in model.Items)
                            {
                                var issueItem = ConvertIssueItem(item, issue.FinishLeatherIssueID, userId);
                                if (item.FinishLeatherIssueItemID == 0)
                                {
                                    _context.INV_FinishLeatherIssueItem.Add(issueItem);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    _context.SaveChanges();
                                }
                                if (item.Colors != null)
                                {
                                    foreach (var color in item.Colors)
                                    {
                                        var itemColor = ConvertIssueItemColor(color, issueItem.FinishLeatherIssueItemID,
                                            issue.FinishLeatherIssueID, userId);
                                        if (color.FinishLeatherIssueColorID == 0)
                                        {
                                            _context.INV_FinishLeatherIssueColor.Add(itemColor);
                                            //_context.SaveChanges();
                                        }
                                        else
                                        {
                                            //_context.SaveChanges();
                                        }
                                    }
                                    _context.SaveChanges();
                                }
                            }
                        }
                        tx.Complete();
                        if (_mode == 1)
                        {
                            _vmMsg.ReturnId = issue.FinishLeatherIssueID;
                            _vmMsg.ReturnCode = issue.FinishLeatherIssueNo;
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved successfully.";
                        }
                        if (_mode == 2)
                        {
                            _vmMsg.ReturnId = issue.FinishLeatherIssueID;
                            _vmMsg.Type = Enums.MessageType.Update;
                            _vmMsg.Msg = "Updated successfully.";
                        }
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        private INV_FinishLeatherIssue ConvertIssue(InvFinishLeatherIssue model, int userId, string url)
        {
            var num = new Random();
            var entity = model.FinishLeatherIssueID == 0 ? new INV_FinishLeatherIssue() : 
                (from b in _context.INV_FinishLeatherIssue.AsQueryable()
                where b.FinishLeatherIssueID == model.FinishLeatherIssueID
                select b).FirstOrDefault();

            entity.FinishLeatherIssueID = model.FinishLeatherIssueID;
            entity.FinishLeatherIssueNo = num.Next().ToString(CultureInfo.InvariantCulture); //model.FinishLeatherIssueNo ?? DalCommon.GetPreDefineNextCodeByUrl(url);
            entity.FinishLeatherIssueDate = DalCommon.SetDate(model.FinishLeatherIssueDate);
            entity.IssueCategory = model.IssueCategory;
            entity.IssueFor = model.IssueFor;
            entity.IssueFrom = model.IssueFrom;
            entity.IssueTo = model.IssueTo;
            entity.RecordStatus = model.RecordStatus ?? "NCF";
            entity.SetOn = model.FinishLeatherIssueID == 0
                ? DateTime.Now
                : _unit.FinishLeatherIssue.GetByID(model.FinishLeatherIssueID).SetOn;
            entity.SetBy = model.FinishLeatherIssueID == 0
                ? userId
                : _unit.FinishLeatherIssue.GetByID(model.FinishLeatherIssueID).SetBy;
            entity.ModifiedOn = model.FinishLeatherIssueID == 0 ? (DateTime?)null : DateTime.Now;
            entity.ModifiedBy = model.FinishLeatherIssueID == 0 ? (int?)null : userId;

            return entity;
        }

        private INV_FinishLeatherIssueItem ConvertIssueItem(InvFinishedLeatherIssueItem model, long issueId, int userId)
        {
            var entity = model.FinishLeatherIssueItemID == 0 ? new INV_FinishLeatherIssueItem() : 
                (from b in _context.INV_FinishLeatherIssueItem.AsQueryable()
                where b.FinishLeatherIssueItemID == model.FinishLeatherIssueItemID
                select b).FirstOrDefault();

            entity.FinishLeatherIssueItemID = model.FinishLeatherIssueItemID;
            entity.FinishLeatherIssueID = issueId;
            entity.RequisitionDateID = null;
            //entity.RequisitionNo = null;
            entity.BuyerID = model.BuyerID;
            entity.BuyerOrderID = model.BuyerOrderID;
            entity.ArticleID = model.ArticleID;
            entity.ArticleNo = model.ArticleNo;
            entity.ArticleChallanNo = model.ArticleChallanNo;
            entity.ItemTypeID = model.ItemTypeID;
            entity.LeatherTypeID = model.LeatherTypeID;
            entity.LeatherStatusID = model.LeatherStatusID;
            entity.SetOn = model.FinishLeatherIssueItemID == 0
                ? DateTime.Now
                : _unit.FinishLeatherIssueItem.GetByID(model.FinishLeatherIssueItemID).SetOn;
            entity.SetBy = model.FinishLeatherIssueItemID == 0
                ? userId
                : _unit.FinishLeatherIssueItem.GetByID(model.FinishLeatherIssueItemID).SetBy;
            entity.ModifiedOn = model.FinishLeatherIssueItemID == 0 ? (DateTime?)null : DateTime.Now;
            entity.ModifiedBy = model.FinishLeatherIssueItemID == 0 ? (int?)null : userId;
            return entity;
        }

        private INV_FinishLeatherIssueColor ConvertIssueItemColor(InvFinishedLeatherIssueColor model, long itemId, long issueId, int userId)
        {
            var entity = model.FinishLeatherIssueColorID == 0 ? new INV_FinishLeatherIssueColor() :
                (from b in _context.INV_FinishLeatherIssueColor.AsQueryable()
                where b.FinishLeatherIssueColorID == model.FinishLeatherIssueColorID
                select b).FirstOrDefault();

            entity.FinishLeatherIssueColorID = model.FinishLeatherIssueColorID;
            entity.FinishLeatherIssueItemID = itemId;
            entity.FinishLeatherIssueID = issueId;
            entity.ColorID = model.ColorID;
            entity.GradeID = model.GradeID;
            entity.FinishQCLabel = model.FinishQCLabel;
            entity.IssuePcs = model.IssuePcs ?? 0;
            entity.IssueSide = model.IssueSide ?? 0;
            entity.IssueArea = model.IssueArea ?? 0;
            entity.SideArea = model.SideArea;
            entity.AreaUnit = model.AreaUnit;
            entity.SetOn = model.FinishLeatherIssueColorID == 0
                ? DateTime.Now
                : _unit.FinishLeatherIssueColor.GetByID(model.FinishLeatherIssueColorID).SetOn;
            entity.SetBy = model.FinishLeatherIssueColorID == 0
                ? userId
                : _unit.FinishLeatherIssueColor.GetByID(model.FinishLeatherIssueColorID).SetBy;
            entity.ModifiedBy = model.FinishLeatherIssueColorID == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.FinishLeatherIssueColorID == 0 ? (DateTime?)null : DateTime.Now;
            return entity; 
        }


        public ValidationMsg DeleteAll(long id)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_unit)
                    {
                        var issue = _unit.FinishLeatherIssue.GetByID(id);
                        if (issue != null)
                        {
                            var items =
                                _unit.FinishLeatherIssueItem.Get().Where(ob => ob.FinishLeatherIssueID == id).ToList();
                            if (items.Count > 0)
                            {
                                foreach (var item in items)
                                {
                                    var colors =
                                        _unit.FinishLeatherIssueColor.Get()
                                            .Where(ob => ob.FinishLeatherIssueItemID == item.FinishLeatherIssueItemID)
                                            .ToList();
                                    if (colors.Count > 0)
                                    {
                                        foreach (var color in colors)
                                        {
                                            _unit.FinishLeatherIssueColor.Delete(color.FinishLeatherIssueColorID);
                                        }
                                    }
                                    _unit.FinishLeatherIssueItem.Delete(item.FinishLeatherIssueItemID);
                                }
                            }
                            _unit.FinishLeatherIssue.Delete(issue.FinishLeatherIssueID);
                            _unit.IsSaved();
                            tx.Complete();
                            _vmMsg.Type = Enums.MessageType.Delete;
                            _vmMsg.Msg = "Issue deleted successfully";
                        }
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to delete";
            }
            return _vmMsg;
        }

        public ValidationMsg DeleteItem(long id)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_unit)
                    {
                        var item = _unit.FinishLeatherIssueItem.GetByID(id);
                        if (item != null)
                        {
                            var colors =
                                _unit.FinishLeatherIssueColor.Get().Where(ob => ob.FinishLeatherIssueItemID == id).ToList();
                            if (colors.Count > 0)
                            {
                                foreach (var color in colors)
                                {
                                    _unit.FinishLeatherIssueColor.Delete(color);
                                }

                            }
                            _unit.FinishLeatherIssueItem.Delete(item);
                            _unit.IsSaved();
                            _vmMsg.Type = Enums.MessageType.Delete;
                            _vmMsg.Msg = "Item deleted successfully.";
                        }
                        tx.Complete();
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to delete item.";
            }
            return _vmMsg;
        }

        public ValidationMsg DeleteColor(long id)
        {
            try
            {
                var color = _unit.FinishLeatherIssueColor.GetByID(id);
                if (color != null)
                {
                    _unit.FinishLeatherIssueColor.Delete(color);
                    _unit.IsSaved();
                }
                _vmMsg.Type = Enums.MessageType.Delete;
                _vmMsg.Msg = "Item color deleted successfully.";
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to delete item color.";
            }
            return _vmMsg;
        }

        public ValidationMsg Check(long issueId, int userId, string comment)
        {
            try
            {
                var issue = _unit.FinishLeatherIssue.GetByID(issueId);
                issue.RecordStatus = "CHK";
                issue.CheckDate = DateTime.Now;
                issue.CheckedBy = userId;
                issue.CheckNote = comment;
                _unit.IsSaved();
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Checked successfully.";
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to check.";
            }
            return _vmMsg;
        }


        public ValidationMsg Confirm(InvFinishLeatherIssue model, int userId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {

                    using (_context)
                    {

                        var originalEntityYearMonth = _context.INV_FinishLeatherIssue
                            .First(m => m.FinishLeatherIssueID == model.FinishLeatherIssueID);
                        originalEntityYearMonth.RecordStatus = "CNF";
                        originalEntityYearMonth.ModifiedBy = userId;
                        originalEntityYearMonth.ModifiedOn = DateTime.Now;

                        #region Update Store Records

                        if (model.Items != null)
                        {
                            foreach (var objQCItem in model.Items)
                            {
                                if (objQCItem.Colors != null)
                                {
                                    foreach (var objQCSelection in objQCItem.Colors)
                                    {
                                        var currentDate = DateTime.Now.Date;

                                        #region Daily_Stock_Update for Store from

                                        var CheckDate = (from ds in _context.INV_FinishStockDaily.AsQueryable()
                                                         where ds.StockDate == currentDate
                                                             && ds.StoreID == model.IssueFrom
                                                             && ds.ArticleID == objQCItem.ArticleID
                                                             && ds.ItemTypeID == objQCItem.ItemTypeID
                                                             && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                             && ds.ColorID == objQCSelection.ColorID
                                                             && ds.GradeID == objQCSelection.GradeID
                                                         select ds).Any();

                                        if (CheckDate)
                                        {
                                            var CurrentItem = (from ds in _context.INV_FinishStockDaily.AsQueryable()
                                                               where ds.StockDate == currentDate
                                                                     && ds.StoreID == model.IssueFrom
                                                             && ds.ArticleID == objQCItem.ArticleID
                                                             && ds.ItemTypeID == objQCItem.ItemTypeID
                                                             && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                             && ds.ColorID == objQCSelection.ColorID
                                                             && ds.GradeID == objQCSelection.GradeID
                                                               select ds).FirstOrDefault();

                                            CurrentItem.IssueStockPcs = CurrentItem.IssueStockPcs +
                                                                   objQCSelection.IssuePcs;
                                            CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockPcs -
                                                                     objQCSelection.IssuePcs;

                                            CurrentItem.IssueStockSide = CurrentItem.IssueStockSide +
                                                                   objQCSelection.IssueSide;
                                            CurrentItem.ClosingStockSide = CurrentItem.ClosingStockSide -
                                                                     objQCSelection.IssueSide;

                                            CurrentItem.IssueStockArea = CurrentItem.IssueStockArea +
                                                                   objQCSelection.IssueArea;
                                            CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockArea -
                                                                     objQCSelection.IssueArea;
                                        }
                                        else
                                        {
                                            var PreviousRecord =
                                                (from ds in _context.INV_FinishStockDaily.AsQueryable()
                                                 where ds.StockDate == currentDate
                                                       && ds.StoreID == model.IssueFrom
                                                             && ds.ArticleID == objQCItem.ArticleID
                                                             && ds.ItemTypeID == objQCItem.ItemTypeID
                                                             && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                             && ds.ColorID == objQCSelection.ColorID
                                                             && ds.GradeID == objQCSelection.GradeID
                                                 orderby ds.StockDate
                                                 select ds).LastOrDefault();

                                            var objStockDaily = new INV_FinishStockDaily();

                                            objStockDaily.StockDate = currentDate;

                                            objStockDaily.StoreID = Convert.ToByte(model.IssueFrom);
                                            objStockDaily.ItemTypeID = objQCItem.ItemTypeID;
                                            objStockDaily.LeatherTypeID = objQCItem.LeatherTypeID;
                                            objStockDaily.LeatherStatusID = objQCItem.LeatherStatusID;
                                            objStockDaily.ArticleID = objQCItem.ArticleID;
                                            objStockDaily.ColorID = objQCSelection.ColorID;
                                            objStockDaily.GradeID = objQCSelection.GradeID;
                                            objStockDaily.FinishQCLabel = objQCSelection.FinishQCLabel;

                                            objStockDaily.OpeningStockPcs = (PreviousRecord == null
                                                ? 0
                                                : PreviousRecord.ClosingStockPcs);
                                            objStockDaily.ReceiveStockPcs = 0;
                                            objStockDaily.IssueStockPcs = objQCSelection.IssuePcs;
                                            objStockDaily.ClosingStockPcs = objStockDaily.OpeningStockPcs -
                                                                       objQCSelection.IssuePcs;

                                            objStockDaily.OpeningStockSide = (PreviousRecord == null
                                                 ? 0
                                                 : PreviousRecord.ClosingStockSide);
                                            objStockDaily.ReceiveStockSide = 0;
                                            objStockDaily.IssueStockSide = objQCSelection.IssueSide;
                                            objStockDaily.ClosingStockSide = objStockDaily.OpeningStockSide -
                                                                       objQCSelection.IssueSide;

                                            objStockDaily.OpeningStockArea = (PreviousRecord == null
                                                ? 0
                                                : PreviousRecord.ClosingStockArea);
                                            objStockDaily.ReceiveStockArea = 0;
                                            objStockDaily.IssueStockArea = objQCSelection.IssueArea;
                                            objStockDaily.ClosingStockArea = objStockDaily.OpeningStockArea -
                                                                       objQCSelection.IssueArea;

                                            _context.INV_FinishStockDaily.Add(objStockDaily);
                                            //_context.SaveChanges();
                                        }

                                        #endregion

                                        #region Buyer_Stock_Update for Store from

                                        var CheckBuyerStock =
                                            (from ds in _context.INV_FinishBuyerStock.AsQueryable()
                                             where ds.BuyerID == objQCItem.BuyerID
                                             && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                   && ds.StoreID == model.IssueFrom
                                                             && ds.ArticleID == objQCItem.ArticleID
                                                             && ds.ItemTypeID == objQCItem.ItemTypeID
                                                             && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                             && ds.ColorID == objQCSelection.ColorID
                                                             && ds.GradeID == objQCSelection.GradeID
                                             select ds).Any();

                                        if (!CheckBuyerStock)
                                        {
                                            var PreviousRecord =
                                                (from ds in _context.INV_FinishBuyerStock.AsQueryable()
                                                 where ds.BuyerID == objQCItem.BuyerID
                                                 && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                       && ds.StoreID == model.IssueFrom
                                                                 && ds.ArticleID == objQCItem.ArticleID
                                                                 && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                 && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                 && ds.ColorID == objQCSelection.ColorID
                                                                 && ds.GradeID == objQCSelection.GradeID
                                                 select ds).LastOrDefault();

                                            var objStockSupplier = new INV_FinishBuyerStock();

                                            objStockSupplier.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                            objStockSupplier.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                            objStockSupplier.StoreID = Convert.ToByte(model.IssueFrom);
                                            objStockSupplier.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                            objStockSupplier.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                            objStockSupplier.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                            objStockSupplier.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                            objStockSupplier.GradeID = objQCSelection.GradeID;
                                            objStockSupplier.LeatherTypeID = objQCItem.LeatherTypeID;
                                            objStockSupplier.FinishQCLabel = objQCSelection.FinishQCLabel;
                                            objStockSupplier.OpeningStockPcs = PreviousRecord.ClosingStockPcs;
                                            objStockSupplier.ReceiveStockPcs = 0;
                                            objStockSupplier.IssueStockPcs = objQCSelection.IssuePcs;
                                            objStockSupplier.ClosingStockPcs = PreviousRecord.ClosingStockPcs -
                                                                          objQCSelection.IssuePcs;

                                            objStockSupplier.OpeningStockSide = PreviousRecord.ClosingStockSide;
                                            objStockSupplier.ReceiveStockSide = 0;
                                            objStockSupplier.IssueStockSide = objQCSelection.IssueSide;
                                            objStockSupplier.ClosingStockSide = PreviousRecord.ClosingStockSide -
                                                                          objQCSelection.IssueSide;

                                            objStockSupplier.OpeningStockArea = PreviousRecord.ClosingStockArea;
                                            objStockSupplier.ReceiveStockArea = 0;
                                            objStockSupplier.IssueStockArea = objQCSelection.IssueArea;
                                            objStockSupplier.ClosingStockArea = PreviousRecord.ClosingStockArea -
                                                                          objQCSelection.IssueArea;


                                            _context.INV_FinishBuyerStock.Add(objStockSupplier);
                                            //_context.SaveChanges();
                                        }
                                        else
                                        {
                                            var LastBuyerStock =
                                                (from ds in _context.INV_FinishBuyerStock.AsQueryable()
                                                 where ds.BuyerID == objQCItem.BuyerID
                                                 && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                       && ds.StoreID == model.IssueFrom
                                                                 && ds.ArticleID == objQCItem.ArticleID
                                                                 && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                 && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                 && ds.ColorID == objQCSelection.ColorID
                                                                 && ds.GradeID == objQCSelection.GradeID
                                                 orderby ds.TransectionID descending
                                                 select ds).FirstOrDefault();


                                            var objStockSupplier = new INV_FinishBuyerStock();

                                            objStockSupplier.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                            objStockSupplier.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                            objStockSupplier.StoreID = Convert.ToByte(model.IssueFrom);
                                            objStockSupplier.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                            objStockSupplier.LeatherTypeID = objQCItem.LeatherTypeID;
                                            objStockSupplier.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                            objStockSupplier.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                            objStockSupplier.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                            objStockSupplier.GradeID = objQCSelection.GradeID;
                                            objStockSupplier.FinishQCLabel = objQCSelection.FinishQCLabel;
                                            objStockSupplier.OpeningStockPcs = LastBuyerStock.ClosingStockPcs;
                                            objStockSupplier.ReceiveStockPcs = 0;
                                            objStockSupplier.IssueStockPcs = objQCSelection.IssuePcs;
                                            objStockSupplier.ClosingStockPcs = LastBuyerStock.ClosingStockPcs -
                                                                          objQCSelection.IssuePcs;

                                            objStockSupplier.OpeningStockSide = LastBuyerStock.ClosingStockSide;
                                            objStockSupplier.ReceiveStockSide = 0;
                                            objStockSupplier.IssueStockSide = objQCSelection.IssueSide;
                                            objStockSupplier.ClosingStockSide = LastBuyerStock.ClosingStockSide -
                                                                          objQCSelection.IssueSide;

                                            objStockSupplier.OpeningStockArea = LastBuyerStock.ClosingStockArea;
                                            objStockSupplier.ReceiveStockArea = 0;
                                            objStockSupplier.IssueStockArea = objQCSelection.IssueArea;
                                            objStockSupplier.ClosingStockArea = LastBuyerStock.ClosingStockArea -
                                                                          objQCSelection.IssueArea;

                                            _context.INV_FinishBuyerStock.Add(objStockSupplier);
                                            //_context.SaveChanges();
                                        }

                                        #endregion

                                        #region Item_Stock_Update for Store from

                                        var CheckItemStock = (from ds in _context.INV_FinishStockItem.AsQueryable()
                                                              where ds.StoreID == model.IssueFrom
                                                                 && ds.ArticleID == objQCItem.ArticleID
                                                                 && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                 && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                 && ds.ColorID == objQCSelection.ColorID
                                                                 && ds.GradeID == objQCSelection.GradeID
                                                              select ds).Any();

                                        if (!CheckItemStock)
                                        {
                                            var PreviousRecord =
                                                (from ds in _context.INV_FinishStockItem.AsQueryable()
                                                 where ds.StoreID == model.IssueFrom
                                                                 && ds.ArticleID == objQCItem.ArticleID
                                                                 && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                 && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                 && ds.ColorID == objQCSelection.ColorID
                                                                 && ds.GradeID == objQCSelection.GradeID
                                                 select ds).LastOrDefault();

                                            var objStockItem = new INV_FinishStockItem();

                                            objStockItem.StoreID = Convert.ToByte(model.IssueFrom);
                                            objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                            objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                            objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                            objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                            objStockItem.GradeID = objQCSelection.GradeID;
                                            objStockItem.LeatherTypeID = objQCItem.LeatherTypeID;
                                            objStockItem.FinishQCLabel = objQCSelection.FinishQCLabel;
                                            objStockItem.OpeningStockPcs = PreviousRecord.ClosingStockPcs;
                                            objStockItem.ReceiveStockPcs = 0;
                                            objStockItem.IssueStockPcs = objQCSelection.IssuePcs;
                                            objStockItem.ClosingStockPcs = PreviousRecord.ClosingStockPcs -
                                                                      objQCSelection.IssuePcs;

                                            objStockItem.OpeningStockSide = PreviousRecord.ClosingStockSide;
                                            objStockItem.ReceiveStockSide = 0;
                                            objStockItem.IssueStockSide = objQCSelection.IssueSide;
                                            objStockItem.ClosingStockSide = PreviousRecord.ClosingStockSide -
                                                                      objQCSelection.IssueSide;

                                            objStockItem.OpeningStockArea = PreviousRecord.ClosingStockArea;
                                            objStockItem.ReceiveStockArea = 0;
                                            objStockItem.IssueStockArea = objQCSelection.IssueArea;
                                            objStockItem.ClosingStockArea = PreviousRecord.ClosingStockArea -
                                                                      objQCSelection.IssueArea;

                                            _context.INV_FinishStockItem.Add(objStockItem);
                                            //_context.SaveChanges();
                                        }
                                        else
                                        {
                                            var LastItemInfo = (from ds in _context.INV_FinishStockItem
                                                                where ds.StoreID == model.IssueFrom
                                                                 && ds.ArticleID == objQCItem.ArticleID
                                                                 && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                 && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                 && ds.ColorID == objQCSelection.ColorID
                                                                 && ds.GradeID == objQCSelection.GradeID
                                                                orderby ds.TransectionID descending
                                                                select ds).FirstOrDefault();

                                            var objStockItem = new INV_FinishStockItem();

                                            objStockItem.StoreID = Convert.ToByte(model.IssueFrom);
                                            objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                            objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                            objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                            objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                            objStockItem.GradeID = objQCSelection.GradeID;
                                            objStockItem.LeatherTypeID = objQCItem.LeatherTypeID;
                                            objStockItem.FinishQCLabel = objQCSelection.FinishQCLabel;
                                            objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
                                            objStockItem.ReceiveStockPcs = 0;
                                            objStockItem.IssueStockPcs = objQCSelection.IssuePcs;
                                            objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs -
                                                                      objQCSelection.IssuePcs;

                                            objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
                                            objStockItem.ReceiveStockSide = 0;
                                            objStockItem.IssueStockSide = objQCSelection.IssueSide;
                                            objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide -
                                                                      objQCSelection.IssueSide;

                                            objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea;
                                            objStockItem.ReceiveStockArea = 0;
                                            objStockItem.IssueStockArea = objQCSelection.IssueArea;
                                            objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea -
                                                                      objQCSelection.IssueArea;

                                            _context.INV_FinishStockItem.Add(objStockItem);
                                            //_context.SaveChanges();
                                        }

                                        #endregion

                                        if (model.IssueCategory == "PKNG")
                                        {
                                            #region Daily Stock Update for Store to
                                            var CheckDate2 = (from ds in _context.INV_FinishStockDaily.AsQueryable()
                                                              where ds.StockDate == currentDate
                                                                  && ds.StoreID == model.IssueTo
                                                                  && ds.ArticleID == objQCItem.ArticleID
                                                                  && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                  && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                  && ds.ColorID == objQCSelection.ColorID
                                                                  && ds.GradeID == objQCSelection.GradeID
                                                              select ds).Any();

                                            if (CheckDate2)
                                            {
                                                var CurrentItem = (from ds in _context.INV_FinishStockDaily.AsQueryable()
                                                                   where ds.StockDate == currentDate
                                                                         && ds.StoreID == model.IssueTo
                                                                 && ds.ArticleID == objQCItem.ArticleID
                                                                 && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                 && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                 && ds.ColorID == objQCSelection.ColorID
                                                                 && ds.GradeID == objQCSelection.GradeID
                                                                   select ds).FirstOrDefault();

                                                CurrentItem.IssueStockPcs = CurrentItem.ReceiveStockPcs +
                                                                       objQCSelection.IssuePcs;
                                                CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockPcs +
                                                                         objQCSelection.IssuePcs;

                                                CurrentItem.IssueStockSide = CurrentItem.ReceiveStockSide +
                                                                       objQCSelection.IssueSide;
                                                CurrentItem.ClosingStockSide = CurrentItem.ClosingStockSide +
                                                                         objQCSelection.IssueSide;

                                                CurrentItem.IssueStockArea = CurrentItem.ReceiveStockArea +
                                                                       objQCSelection.IssueArea;
                                                CurrentItem.ClosingStockPcs = CurrentItem.ClosingStockArea +
                                                                         objQCSelection.IssueArea;
                                            }
                                            else
                                            {
                                                var PreviousRecord =
                                                    (from ds in _context.INV_FinishStockDaily.AsQueryable()
                                                     where ds.StockDate == currentDate
                                                           && ds.StoreID == model.IssueTo
                                                                 && ds.ArticleID == objQCItem.ArticleID
                                                                 && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                 && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                 && ds.ColorID == objQCSelection.ColorID
                                                                 && ds.GradeID == objQCSelection.GradeID
                                                     orderby ds.StockDate
                                                     select ds).LastOrDefault();

                                                var objStockDaily = new INV_FinishStockDaily();

                                                objStockDaily.StockDate = currentDate;

                                                objStockDaily.StoreID = Convert.ToByte(model.IssueTo);
                                                objStockDaily.ItemTypeID = objQCItem.ItemTypeID;
                                                objStockDaily.LeatherStatusID = objQCItem.LeatherStatusID;
                                                objStockDaily.ArticleID = objQCItem.ArticleID;
                                                objStockDaily.ColorID = objQCSelection.ColorID;
                                                objStockDaily.GradeID = objQCSelection.GradeID;
                                                objStockDaily.LeatherTypeID = objQCItem.LeatherTypeID;
                                                objStockDaily.FinishQCLabel = objQCSelection.FinishQCLabel;
                                                objStockDaily.OpeningStockPcs = (PreviousRecord == null
                                                    ? 0
                                                    : PreviousRecord.ClosingStockPcs);
                                                objStockDaily.ReceiveStockPcs = objQCSelection.IssuePcs;
                                                objStockDaily.IssueStockPcs = 0;
                                                objStockDaily.ClosingStockPcs = objStockDaily.OpeningStockPcs +
                                                                           objQCSelection.IssuePcs;

                                                objStockDaily.OpeningStockSide = (PreviousRecord == null
                                                     ? 0
                                                     : PreviousRecord.ClosingStockSide);
                                                objStockDaily.ReceiveStockSide = objQCSelection.IssueSide;
                                                objStockDaily.IssueStockSide = 0;
                                                objStockDaily.ClosingStockSide = objStockDaily.OpeningStockSide +
                                                                           objQCSelection.IssueSide;

                                                objStockDaily.OpeningStockArea = (PreviousRecord == null
                                                    ? 0
                                                    : PreviousRecord.ClosingStockArea);
                                                objStockDaily.ReceiveStockArea = objQCSelection.IssueArea;
                                                objStockDaily.IssueStockArea = 0;
                                                objStockDaily.ClosingStockArea = objStockDaily.OpeningStockArea +
                                                                           objQCSelection.IssueArea;

                                                _context.INV_FinishStockDaily.Add(objStockDaily);
                                                //_context.SaveChanges();
                                            }
                                            #endregion

                                            #region Buyer_Stock_Update for Store To

                                            var CheckBuyerStock2 =
                                                (from ds in _context.INV_FinishBuyerStock.AsQueryable()
                                                 where ds.BuyerID == objQCItem.BuyerID
                                                 && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                       && ds.StoreID == model.IssueTo
                                                                 && ds.ArticleID == objQCItem.ArticleID
                                                                 && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                 && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                 && ds.ColorID == objQCSelection.ColorID
                                                                 && ds.GradeID == objQCSelection.GradeID
                                                 select ds).Any();

                                            if (!CheckBuyerStock2)
                                            {
                                                var PreviousRecord =
                                                    (from ds in _context.INV_FinishBuyerStock.AsQueryable()
                                                     where ds.BuyerID == objQCItem.BuyerID
                                                     && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                           && ds.StoreID == model.IssueTo
                                                                     && ds.ArticleID == objQCItem.ArticleID
                                                                     && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                     && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                     && ds.ColorID == objQCSelection.ColorID
                                                                     && ds.GradeID == objQCSelection.GradeID
                                                     select ds).LastOrDefault();

                                                var objStockSupplier = new INV_FinishBuyerStock();

                                                objStockSupplier.BuyerID = Convert.ToInt32(objQCItem.BuyerID);
                                                objStockSupplier.BuyerOrderID = Convert.ToInt32(objQCItem.BuyerOrderID);
                                                objStockSupplier.StoreID = Convert.ToByte(model.IssueTo);
                                                objStockSupplier.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockSupplier.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockSupplier.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockSupplier.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                                objStockSupplier.GradeID = objQCSelection.GradeID;
                                                objStockSupplier.LeatherTypeID = objQCItem.LeatherTypeID;
                                                objStockSupplier.FinishQCLabel = objQCSelection.FinishQCLabel;
                                                objStockSupplier.OpeningStockPcs = PreviousRecord == null ? 0 : 
                                                    PreviousRecord.ClosingStockPcs;
                                                objStockSupplier.ReceiveStockPcs = objQCSelection.IssuePcs;
                                                objStockSupplier.IssueStockPcs = 0;
                                                objStockSupplier.ClosingStockPcs = (PreviousRecord == null ? 0 : 
                                                    PreviousRecord.ClosingStockPcs) + objQCSelection.IssuePcs;
                                                objStockSupplier.OpeningStockSide = PreviousRecord == null ? 0 :
                                                    PreviousRecord.ClosingStockSide;
                                                objStockSupplier.ReceiveStockSide = objQCSelection.IssueSide;
                                                objStockSupplier.IssueStockSide = 0;
                                                objStockSupplier.ClosingStockSide = (PreviousRecord == null ? 0 
                                                    : PreviousRecord.ClosingStockSide) + objQCSelection.IssueSide;
                                                objStockSupplier.OpeningStockArea = PreviousRecord == null ? 0 :
                                                    PreviousRecord.ClosingStockArea;
                                                objStockSupplier.ReceiveStockArea = objQCSelection.IssueArea;
                                                objStockSupplier.IssueStockArea = 0;
                                                objStockSupplier.ClosingStockArea = (PreviousRecord == null ? 0 : 
                                                    PreviousRecord.ClosingStockArea) + objQCSelection.IssueArea;
                                                _context.INV_FinishBuyerStock.Add(objStockSupplier);
                                                //_context.SaveChanges();
                                            }
                                            else
                                            {
                                                var LastBuyerStock =
                                                    (from ds in _context.INV_FinishBuyerStock.AsQueryable()
                                                     where ds.BuyerID == objQCItem.BuyerID
                                                     && ds.BuyerOrderID == objQCItem.BuyerOrderID
                                                           && ds.StoreID == model.IssueTo
                                                                     && ds.ArticleID == objQCItem.ArticleID
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
                                                objStockSupplier.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockSupplier.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockSupplier.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                                objStockSupplier.GradeID = objQCSelection.GradeID;
                                                objStockSupplier.LeatherTypeID = objQCItem.LeatherTypeID;
                                                objStockSupplier.FinishQCLabel = objQCSelection.FinishQCLabel;
                                                objStockSupplier.OpeningStockPcs = LastBuyerStock.ClosingStockPcs;
                                                objStockSupplier.ReceiveStockPcs = objQCSelection.IssuePcs;
                                                objStockSupplier.IssueStockPcs = 0;
                                                objStockSupplier.ClosingStockPcs = LastBuyerStock.ClosingStockPcs +
                                                                              objQCSelection.IssuePcs;

                                                objStockSupplier.OpeningStockSide = LastBuyerStock.ClosingStockSide;
                                                objStockSupplier.ReceiveStockSide = objQCSelection.IssueSide;
                                                objStockSupplier.IssueStockSide = 0;
                                                objStockSupplier.ClosingStockSide = LastBuyerStock.ClosingStockSide +
                                                                              objQCSelection.IssueSide;

                                                objStockSupplier.OpeningStockArea = LastBuyerStock.ClosingStockArea;
                                                objStockSupplier.ReceiveStockArea = objQCSelection.IssueArea;
                                                objStockSupplier.IssueStockArea = 0;
                                                objStockSupplier.ClosingStockArea = LastBuyerStock.ClosingStockArea +
                                                                              objQCSelection.IssueArea;

                                                _context.INV_FinishBuyerStock.Add(objStockSupplier);
                                                //_context.SaveChanges();
                                            }

                                            #endregion

                                            #region Item_Stock_Update for Store To


                                            var CheckItemStock2 = (from ds in _context.INV_FinishStockItem.AsQueryable()
                                                                   where ds.StoreID == model.IssueTo
                                                                      && ds.ArticleID == objQCItem.ArticleID
                                                                      && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                      && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                      && ds.ColorID == objQCSelection.ColorID
                                                                      && ds.GradeID == objQCSelection.GradeID
                                                                   select ds).Any();

                                            if (!CheckItemStock2)
                                            {
                                                var PreviousRecord =
                                                    (from ds in _context.INV_FinishStockItem.AsQueryable()
                                                     where ds.StoreID == model.IssueTo
                                                                     && ds.ArticleID == objQCItem.ArticleID
                                                                     && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                     && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                     && ds.ColorID == objQCSelection.ColorID
                                                                     && ds.GradeID == objQCSelection.GradeID
                                                     select ds).LastOrDefault();

                                                var objStockItem = new INV_FinishStockItem();

                                                objStockItem.StoreID = Convert.ToByte(model.IssueTo);
                                                objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                                objStockItem.GradeID = objQCSelection.GradeID;
                                                objStockItem.LeatherTypeID = objQCItem.LeatherTypeID;
                                                objStockItem.FinishQCLabel = objQCSelection.FinishQCLabel;
                                                objStockItem.OpeningStockPcs = PreviousRecord == null ? 0 : 
                                                    PreviousRecord.ClosingStockPcs;
                                                objStockItem.ReceiveStockPcs = objQCSelection.IssuePcs;
                                                objStockItem.IssueStockPcs = 0;
                                                objStockItem.ClosingStockPcs = (PreviousRecord == null ? 0 : 
                                                    PreviousRecord.ClosingStockPcs) + objQCSelection.IssuePcs;
                                                objStockItem.OpeningStockSide = PreviousRecord == null ? 0 : 
                                                    PreviousRecord.ClosingStockSide;
                                                objStockItem.ReceiveStockSide = objQCSelection.IssueSide;
                                                objStockItem.IssueStockSide = 0;
                                                objStockItem.ClosingStockSide = (PreviousRecord == null ? 0 : 
                                                    PreviousRecord.ClosingStockSide) + objQCSelection.IssueSide;
                                                objStockItem.OpeningStockArea = PreviousRecord == null ? 0 : 
                                                    PreviousRecord.ClosingStockArea;
                                                objStockItem.ReceiveStockArea = objQCSelection.IssueArea;
                                                objStockItem.IssueStockArea = 0;
                                                objStockItem.ClosingStockArea = (PreviousRecord == null ? 0 :
                                                    PreviousRecord.ClosingStockArea) + objQCSelection.IssueArea;
                                                _context.INV_FinishStockItem.Add(objStockItem);
                                                //_context.SaveChanges();
                                            }
                                            else
                                            {
                                                var LastItemInfo = (from ds in _context.INV_FinishStockItem
                                                                    where ds.StoreID == model.IssueTo
                                                                     && ds.ArticleID == objQCItem.ArticleID
                                                                     && ds.ItemTypeID == objQCItem.ItemTypeID
                                                                     && ds.LeatherStatusID == objQCItem.LeatherStatusID
                                                                     && ds.ColorID == objQCSelection.ColorID
                                                                     && ds.GradeID == objQCSelection.GradeID
                                                                    orderby ds.TransectionID descending
                                                                    select ds).FirstOrDefault();

                                                var objStockItem = new INV_FinishStockItem();

                                                objStockItem.StoreID = Convert.ToByte(model.IssueTo);
                                                objStockItem.ItemTypeID = Convert.ToByte(objQCItem.ItemTypeID);
                                                objStockItem.LeatherStatusID = Convert.ToByte(objQCItem.LeatherStatusID);
                                                objStockItem.ArticleID = Convert.ToInt32(objQCItem.ArticleID);
                                                objStockItem.ColorID = Convert.ToInt32(objQCSelection.ColorID);
                                                objStockItem.GradeID = objQCSelection.GradeID;
                                                objStockItem.LeatherTypeID = objQCItem.LeatherTypeID;
                                                objStockItem.FinishQCLabel = objQCSelection.FinishQCLabel;
                                                objStockItem.OpeningStockPcs = LastItemInfo.ClosingStockPcs;
                                                objStockItem.ReceiveStockPcs = objQCSelection.IssuePcs;
                                                objStockItem.IssueStockPcs = 0;
                                                objStockItem.ClosingStockPcs = LastItemInfo.ClosingStockPcs +
                                                                          objQCSelection.IssuePcs;

                                                objStockItem.OpeningStockSide = LastItemInfo.ClosingStockSide;
                                                objStockItem.ReceiveStockSide = objQCSelection.IssueSide;
                                                objStockItem.IssueStockSide = 0;
                                                objStockItem.ClosingStockSide = LastItemInfo.ClosingStockSide +
                                                                          objQCSelection.IssueSide;

                                                objStockItem.OpeningStockArea = LastItemInfo.ClosingStockArea;
                                                objStockItem.ReceiveStockArea = objQCSelection.IssueArea;
                                                objStockItem.IssueStockArea = 0;
                                                objStockItem.ClosingStockArea = LastItemInfo.ClosingStockArea +
                                                                          objQCSelection.IssueArea;

                                                _context.INV_FinishStockItem.Add(objStockItem);
                                                //_context.SaveChanges();
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
                        _vmMsg.Msg = "Issued successfully.";
                    }
                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to issue.";
            }
            return _vmMsg;
        }
    }
}
