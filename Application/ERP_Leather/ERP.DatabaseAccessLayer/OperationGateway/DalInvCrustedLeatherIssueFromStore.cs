using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using Microsoft.SqlServer.Server;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalInvCrustedLeatherIssueFromStore
    {
        private readonly BLC_DEVEntities _context;
        private readonly string _connString = string.Empty;
        private readonly UnitOfWork _unit;
        private readonly ValidationMsg _vmMsg;
        private int _mode;
        private bool _save;

        public DalInvCrustedLeatherIssueFromStore()
        {
            _vmMsg = new ValidationMsg();
            _unit = new UnitOfWork();
            _context = new BLC_DEVEntities();
            _connString = StrConnection.GetConnectionString();
        }
        public ValidationMsg Save(InvCrustLeatherIssue model, int userId, string url)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var issue = ConvertIssue(model, userId, url);

                        if (model.CrustLeatherIssueID == 0)
                        {
                            _context.INV_CrustLeatherIssue.Add(issue);
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
                                var issueItem = ConvertIssueItem(item, issue.CrustLeatherIssueID, userId);
                                if (item.CrustLeatherIssueItemID == 0)
                                {
                                    _context.INV_CrustLeatherIssueItem.Add(issueItem);
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
                                        var itemColor = ConvertIssueItemColor(color, issueItem.CrustLeatherIssueItemID,
                                            issue.CrustLeatherIssueID, userId);
                                        if (color.CrustLeatherIssueColorID == 0)
                                        {
                                            _context.INV_CrustLeatherIssueColor.Add(itemColor);
                                            //_context.SaveChanges();
                                        }
                                        else
                                        {
                                            //_context.SaveChanges();
                                        }
                                    }
                                }
                            }
                            _context.SaveChanges();
                        }
                        tx.Complete();
                        if (_mode == 1)
                        {
                            _vmMsg.ReturnId = issue.CrustLeatherIssueID;
                            _vmMsg.ReturnCode = issue.CrustLeatherIssueNo;
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Saved successfully.";
                        }
                        if (_mode == 2)
                        {
                            _vmMsg.ReturnId = issue.CrustLeatherIssueID;
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

        private INV_CrustLeatherIssue ConvertIssue(InvCrustLeatherIssue model, int userId, string url)
        {
            var entity = model.CrustLeatherIssueID == 0 ? new INV_CrustLeatherIssue() : (from b in _context.INV_CrustLeatherIssue.AsEnumerable()
                                                                                         where b.CrustLeatherIssueID == model.CrustLeatherIssueID
                                                                                         select b).FirstOrDefault();

            entity.CrustLeatherIssueID = model.CrustLeatherIssueID;
            entity.CrustLeatherIssueNo = model.CrustLeatherIssueNo ?? DalCommon.GetPreDefineNextCodeByUrl(url);
            entity.CrustLeatherIssueDate = model.CrustLeatherIssueDate == null ? DateTime.Now : DalCommon.SetDate(model.CrustLeatherIssueDate);
            entity.IssueCategory = model.IssueCategory;
            entity.IssueFor = model.IssueFor;
            entity.IssueFrom = model.IssueFrom;
            entity.IssueTo = model.IssueTo;
            entity.RecordStatus = model.RecordStatus ?? "NCF";
            entity.SetOn = model.CrustLeatherIssueID == 0
                ? DateTime.Now
                : _unit.CrustLeatherIssue.GetByID(model.CrustLeatherIssueID).SetOn;
            entity.SetBy = model.CrustLeatherIssueID == 0
                ? userId
                : _unit.CrustLeatherIssue.GetByID(model.CrustLeatherIssueID).SetBy;
            entity.ModifiedOn = model.CrustLeatherIssueID == 0 ? (DateTime?)null : DateTime.Now;
            entity.ModifiedBy = model.CrustLeatherIssueID == 0 ? (int?)null : userId;
            #region New Code

            //var put = entity.INV_CrustLeatherReceiveItem;
            //if (model.Items.Count > 0)
            //{
            //    foreach (var item in model.Items)
            //    {
            //        put.CrustLeatherIssueItemID = item.CrustLeatherIssueItemID;
            //        put.CrustLeatherIssueID = entity.CrustLeatherIssueID;
            //        put.RequisitionDateID = item.RequisitionDateID ?? null;
            //        //entity.RequisitionNo = null;
            //        put.CrustQCLabel = item.CrustQCLabel ?? null;
            //        put.BuyerID = item.BuyerID;
            //        put.BuyerOrderID = item.BuyerOrderID;
            //        put.ArticleID = item.ArticleID;
            //        put.ArticleNo = item.ArticleNo;
            //        put.ArticleChallanID = item.ArticleChallanID;
            //        put.ArticleChallanNo = item.ArticleChallanNo;
            //        put.ItemTypeID = item.ItemTypeID;
            //        put.LeatherTypeID = item.LeatherTypeID;
            //        put.LeatherStatusID = item.LeatherStatusID;
            //        put.SetOn = item.CrustLeatherIssueItemID == 0
            //            ? DateTime.Now
            //            : _unit.CrustLeatherIssueItem.GetByID(item.CrustLeatherIssueItemID).SetOn;
            //        entity.SetBy = item.CrustLeatherIssueItemID == 0
            //            ? userId
            //            : _unit.CrustLeatherIssueItem.GetByID(item.CrustLeatherIssueItemID).SetBy;
            //        entity.ModifiedOn = item.CrustLeatherIssueItemID == 0 ? (DateTime?)null : DateTime.Now;
            //        entity.ModifiedBy = item.CrustLeatherIssueItemID == 0 ? (int?)null : userId;

            //        var col = new INV_CrustLeatherIssueColor();
            //        if (item.Colors.Count > 0)
            //        {
            //            foreach (var color in item.Colors)
            //            {
            //                entity.CrustLeatherIssueColorID = model.CrustLeatherIssueColorID;
            //                entity.CrustLeatherIssueItemID = itemId;
            //                entity.CrustLeatherIssueID = issueId;
            //                entity.ArticleColorNo = entity.ArticleColorNo;
            //                entity.ColorID = model.ColorID;
            //                entity.GradeID = model.GradeID;
            //                entity.GradeRange = model.GradeRange;
            //                entity.CrustQCLabel = model.CrustQCLabel;
            //                entity.IssuePcs = model.IssuePcs ?? 0;
            //                entity.IssueSide = model.IssueSide ?? 0;
            //                entity.IssueArea = model.IssueArea ?? 0;
            //                entity.SideArea = model.SideArea;
            //                entity.AreaUnit = model.AreaUnit;
            //                entity.SetOn = model.CrustLeatherIssueColorID == 0
            //                    ? DateTime.Now
            //                    : _unit.CrustLeatherIssueColor.GetByID(model.CrustLeatherIssueColorID).SetOn;
            //                entity.SetBy = model.CrustLeatherIssueColorID == 0
            //                    ? userId
            //                    : _unit.CrustLeatherIssueColor.GetByID(model.CrustLeatherIssueColorID).SetBy;
            //                entity.ModifiedBy = model.CrustLeatherIssueColorID == 0 ? (int?)null : userId;
            //                entity.ModifiedOn = model.CrustLeatherIssueColorID == 0 ? (DateTime?)null : DateTime.Now;
            //                return entity;
            //            }
            //        }
            //    }

            //}
            #endregion
            return entity;
        }

        private INV_CrustLeatherIssueItem ConvertIssueItem(InvCrustLeatherIssueItem model, long issueId, int userId)
        {
            var entity = model.CrustLeatherIssueItemID == 0 ? new INV_CrustLeatherIssueItem() : (from b in _context.INV_CrustLeatherIssueItem.AsEnumerable()
                                                                                                 where b.CrustLeatherIssueItemID == model.CrustLeatherIssueItemID
                                                                                                 select b).FirstOrDefault();

            entity.CrustLeatherIssueItemID = model.CrustLeatherIssueItemID;
            entity.CrustLeatherIssueID = issueId;
            entity.RequisitionDateID = model.RequisitionDateID ?? null;
            //entity.RequisitionNo = null;
            entity.CrustQCLabel = model.CrustQCLabel ?? null;
            entity.BuyerID = model.BuyerID;
            entity.BuyerOrderID = model.BuyerOrderID;
            entity.ArticleID = model.ArticleID;
            entity.ArticleNo = model.ArticleNo;
            entity.ArticleChallanID = model.ArticleChallanID;
            entity.ArticleChallanNo = model.ArticleChallanNo;
            entity.ItemTypeID = model.ItemTypeID;
            entity.LeatherTypeID = model.LeatherTypeID;
            entity.LeatherStatusID = model.LeatherStatusID;
            entity.SetOn = model.CrustLeatherIssueItemID == 0
                ? DateTime.Now
                : _unit.CrustLeatherIssueItem.GetByID(model.CrustLeatherIssueItemID).SetOn;
            entity.SetBy = model.CrustLeatherIssueItemID == 0
                ? userId
                : _unit.CrustLeatherIssueItem.GetByID(model.CrustLeatherIssueItemID).SetBy;
            entity.ModifiedOn = model.CrustLeatherIssueItemID == 0 ? (DateTime?)null : DateTime.Now;
            entity.ModifiedBy = model.CrustLeatherIssueItemID == 0 ? (int?)null : userId;
            return entity;
        }

        private INV_CrustLeatherIssueColor ConvertIssueItemColor(InvCrustLeatherIssueColor model, long itemId,
            long issueId, int userId)
        {
            var entity = model.CrustLeatherIssueColorID == 0 ? new INV_CrustLeatherIssueColor() : (from b in _context.INV_CrustLeatherIssueColor.AsEnumerable()
                                                                                                   where b.CrustLeatherIssueColorID == model.CrustLeatherIssueColorID
                                                                                                   select b).FirstOrDefault();
            entity.CrustLeatherIssueColorID = model.CrustLeatherIssueColorID;
            entity.CrustLeatherIssueItemID = itemId;
            entity.CrustLeatherIssueID = issueId;
            entity.ArticleColorNo = entity.ArticleColorNo;
            entity.ColorID = model.ColorID;
            entity.GradeID = model.GradeID;
            entity.GradeRange = model.GradeRange;
            entity.CrustQCLabel = model.CrustQCLabel;
            entity.IssuePcs = model.IssuePcs ?? 0;
            entity.IssueSide = model.IssueSide ?? 0;
            entity.IssueArea = model.IssueArea ?? 0;
            entity.SideArea = model.SideArea;
            entity.AreaUnit = model.AreaUnit;
            entity.SetOn = model.CrustLeatherIssueColorID == 0
                ? DateTime.Now
                : _unit.CrustLeatherIssueColor.GetByID(model.CrustLeatherIssueColorID).SetOn;
            entity.SetBy = model.CrustLeatherIssueColorID == 0
                ? userId
                : _unit.CrustLeatherIssueColor.GetByID(model.CrustLeatherIssueColorID).SetBy;
            entity.ModifiedBy = model.CrustLeatherIssueColorID == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.CrustLeatherIssueColorID == 0 ? (DateTime?)null : DateTime.Now;
            return entity;
        }

        public object GetOrderByBuyer(byte storeId, long buyerId)
        {
            var query = "SELECT inv.TransectionID, inv.BuyerID, (SELECT BuyerName FROM Sys_Buyer WHERE (BuyerID = inv.BuyerID)) AS BuyerName, inv.BuyerOrderID,(SELECT BuyerOrderNo FROM SLS_BuyerOrder WHERE (BuyerOrderID = inv.BuyerOrderID)) AS BuyerOrderNo, inv.ItemTypeID, (SELECT ItemTypeName FROM Sys_ItemType WHERE (ItemTypeID = inv.ItemTypeID)) AS ItemTypeName, inv.LeatherTypeID, (SELECT LeatherTypeName FROM Sys_LeatherType WHERE (LeatherTypeID = inv.LeatherTypeID)) AS LeatherTypeName, inv.LeatherStatusID, (SELECT LeatherStatusName FROM Sys_LeatherStatus WHERE (LeatherStatusID = inv.LeatherStatusID)) AS LeatherStatusName, inv.ArticleID, inv.ArticleNo, (SELECT ArticleName FROM Sys_Article WHERE (ArticleID = inv.ArticleID)) AS ArticleName, inv.ClosingStockPcs FROM INV_CrustBuyerStock AS inv INNER JOIN (SELECT MAX(TransectionID) AS TransectionID, StoreID, BuyerID, BuyerOrderID, ItemTypeID, LeatherTypeID, LeatherStatusID, ArticleID FROM INV_CrustBuyerStock GROUP BY StoreID, BuyerID, BuyerOrderID, ItemTypeID, LeatherTypeID, LeatherStatusID, ArticleID) AS sup ON inv.TransectionID = sup.TransectionID WHERE (inv.StoreID = " + storeId + ") AND (inv.BuyerID =" + buyerId + ") AND (inv.ClosingStockPcs > 0)";
            var orders = _context.Database.SqlQuery<InvCrustLeatherIssueItem>(query).ToList();
            return orders;
        }

        public List<InvCrustLeatherIssueColor> GetColorByComb(byte storeId, string qcLabel, int? buyerId, long? orderId, int? articleId, long? articleChallanId, byte? itemTypeId, byte? leatherTypeId, byte? leatherStatusID)
        {
            var query = new StringBuilder();
            query.Append(" ");
            query.Append("select inv.TransectionID, inv.ColorID,(select ColorName from dbo.Sys_Color ");
            query.Append("where ColorID = inv.ColorID)ColorName, inv.GradeRange, ");
            query.Append("inv.ClosingStockPcs,inv.ClosingStockSide,inv.ClosingStockArea, inv.CrustQCLabel ");
            query.Append("from dbo.INV_CrustBuyerStock inv ");
            query.Append("INNER JOIN (select MAX(TransectionID)TransectionID,StoreID,BuyerID,BuyerOrderID, ");
            query.Append("ItemTypeID,LeatherTypeID,LeatherStatusID,ArticleID,ColorID,GradeID ");
            query.Append("from dbo.INV_CrustBuyerStock ");
            query.Append("group by StoreID,BuyerID,BuyerOrderID,ItemTypeID,LeatherTypeID, ");
            query.Append("LeatherStatusID,ArticleID,ColorID,GradeID) ");
            query.Append("sup ON inv.TransectionID=sup.TransectionID ");
            query.Append("where inv.StoreID =" + storeId + " and inv.ClosingStockPcs>0 ");
            if (qcLabel != null)
            {
                query.Append("and inv.CrustQCLabel='" + qcLabel + "' ");
            }
            if (buyerId != null)
            {
                query.Append("and inv.BuyerID=" + buyerId + " ");
            }
            if (orderId != null)
            {
                query.Append("and inv.BuyerOrderID=" + orderId + " ");
            }
            if (articleId != null)
            {
                query.Append("and inv.ArticleID=" + articleId + " ");
            }
            if (articleChallanId != null)
            {
                query.Append("and inv.ArticleChallanID=" + articleChallanId + " ");
            }
            if (itemTypeId != null)
            {
                query.Append("and inv.ItemTypeID=" + itemTypeId + " ");
            }
            if (leatherTypeId != null)
            {
                query.Append("and inv.LeatherTypeID=" + leatherTypeId + " ");
            }
            if (leatherStatusID != null)
            {
                query.Append("and inv.LeatherStatusID =" + leatherStatusID + " ");
            }
            var colors = _context.Database.SqlQuery<InvCrustLeatherIssueColor>(query.ToString()).ToList();
            return colors;
        }

        public ValidationMsg DeleteColor(long id)
        {
            try
            {
                //var color = _unit.CrustLeatherIssueColor.GetByID(id);
                var color = _context.INV_CrustLeatherIssueColor.FirstOrDefault(ob => ob.CrustLeatherIssueColorID == id);
                if (color != null)
                {
                    _context.INV_CrustLeatherIssueColor.Remove(color);
                    _context.SaveChanges();

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

        public ValidationMsg DeleteItem(long id)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        //var item = _unit.CrustLeatherIssueItem.GetByID(id);
                        var item =
                            _context.INV_CrustLeatherIssueItem.FirstOrDefault(ob => ob.CrustLeatherIssueItemID == id);
                        if (item != null)
                        {
                            var colors =
                                _context.INV_CrustLeatherIssueColor.Where(ob => ob.CrustLeatherIssueItemID == id)
                                    .ToList();

                            if (colors.Count > 0)
                            {
                                foreach (var color in colors)
                                {
                                    //_unit.CrustLeatherIssueColor.Delete(color);
                                    _context.INV_CrustLeatherIssueColor.Remove(color);
                                }

                            }
                            //_unit.CrustLeatherIssueItem.Delete(item);
                            _context.INV_CrustLeatherIssueItem.Remove(item);
                            //_unit.IsSaved();
                            _context.SaveChanges();
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

        public ValidationMsg DeleteAll(long id)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        //var issue = _unit.CrustLeatherIssue.GetByID(id);
                        var issue = _context.INV_CrustLeatherIssue.FirstOrDefault(ob => ob.CrustLeatherIssueID == id);
                        if (issue != null)
                        {
                            var items =
                                _context.INV_CrustLeatherIssueItem.Where(ob => ob.CrustLeatherIssueID == id).ToList();
                            //_unit.CrustLeatherIssueItem.Get().Where(ob => ob.CrustLeatherIssueID == id).ToList();
                            if (items.Count > 0)
                            {
                                foreach (var item in items)
                                {
                                    var colors =
                                        _context.INV_CrustLeatherIssueColor.Where(
                                            ob => ob.CrustLeatherIssueItemID == item.CrustLeatherIssueItemID).ToList();
                                    //_unit.CrustLeatherIssueColor.Get()
                                    //    .Where(ob => ob.CrustLeatherIssueItemID == item.CrustLeatherIssueItemID)
                                    //    .ToList();
                                    if (colors.Count > 0)
                                    {
                                        foreach (var color in colors)
                                        {
                                            //_unit.CrustLeatherIssueColor.Delete(color);
                                            _context.INV_CrustLeatherIssueColor.Remove(color);
                                        }
                                    }
                                    //_unit.CrustLeatherIssueItem.Delete(item);
                                    _context.INV_CrustLeatherIssueItem.Remove(item);
                                }
                            }
                            //_unit.CrustLeatherIssue.Delete(issue);
                            //_unit.IsSaved();
                            _context.INV_CrustLeatherIssue.Remove(issue);
                            _context.SaveChanges();
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

        public ValidationMsg Check(long issueId, int userId, string comment)
        {
            try
            {
                var issue = _unit.CrustLeatherIssue.GetByID(issueId);
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

        public ValidationMsg Confirm(long crustLeatherIssueId, int userId)
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
                            cmd.CommandText = "UspConfirmCrustIssueFromStore";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@CrustLeatherIssueId", SqlDbType.BigInt).Value = crustLeatherIssueId;
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

        public object GetReq(string year, string month, string cat, byte? floor)
        {

            var query = new StringBuilder();
            query.Append(" SELECT ISNULL(YMFRD.RequisitionDateID,0) RequisitionDateID, YMFRD.RequisitionNo, ");
            query.Append(" CONVERT(NVARCHAR(12), YMFRD.RequiredDate,106) RequiredDate, YMFRD.RequisitionStatus, ");
            query.Append(" YMFRD.RecordStatus, YM.ConcernStore IssueFrom, YM.ProductionFloor IssueTo");
            query.Append(" FROM PRD_YearMonth YM ");
            query.Append(" LEFT JOIN PRD_YearMonthSchedule YMS ON YMS.YearMonID=YM.YearMonID ");
            query.Append(" LEFT JOIN PRD_YearMonthFinishReqDate YMFRD ON YMFRD.ScheduleID=YMS.ScheduleID ");
            query.Append(" WHERE YM.ScheduleFor = 'CRR' AND YMFRD.RequisitionDateID != 0 ");
            if (month != "0")
            {
                query.Append(" AND YM.ScheduleMonth='" + month + "' ");
            }
            if (year != "0")
            {
                query.Append(" AND YM.ScheduleYear='" + year + "' ");
            }

            if (floor != 0)
            {
                query.Append(" AND YM.ProductionFloor=" + floor);
            }
            //"SELECT YMFRD.RequisitionDateID, YMFRD.RequisitionNo, YMFRD.RequiredDate, YMFRD.RequisitionStatus, YMFRD.RecordStatus " +
            //"FROM PRD_YearMonth YM " +
            //"LEFT JOIN PRD_YearMonthSchedule YMS ON YMS.YearMonID=YM.YearMonID " +
            //"LEFT JOIN PRD_YearMonthFinishReqDate YMFRD ON YMFRD.ScheduleID=YMS.ScheduleID " +
            //"WHERE YM.ScheduleMonth=" + month + " AND YM.ScheduleYear=" + year + " AND YM.ScheduleFor='" + cat + "' AND YM.ProductionFloor=" + floor;
            var items = _context.Database.SqlQuery<RequisitionVM>(query.ToString()).ToList();

            var results = items.Select(item => new
            {
                item.RequisitionDateID,
                item.RequisitionNo,
                RequiredDate = string.Format("{0:dd/MM/yyyy}", item.RequiredDate),
                RequisitionStatus = DalCommon.ReturnRequisitionStatus(item.RequisitionStatus),
                RecordStatus = DalCommon.ReturnRecordStatus(item.RecordStatus),
                item.IssueFrom,
                item.IssueTo
            }).ToList();

            return results.OrderByDescending(ob => ob.RequisitionDateID);
        }

        public List<InvCrustLeatherIssueItem> GetReqItemNColors(long reqDateId, byte? issueFrom, byte? issueTo, string qcLabel)
        {
            var items = _unit.FinishReqItem.Get().Where(ob => ob.RequisitionDateID == reqDateId).ToList();
            var result = new List<InvCrustLeatherIssueItem>();

            foreach (var item in items)
            {
                var rslt = new InvCrustLeatherIssueItem();
                rslt.RequisitionItemID = item.RequisitionItemID;
                rslt.RequisitionNo = item.RequisitionNo;
                rslt.RequisitionDateID = item.RequisitionDateID;
                rslt.BuyerID = item.BuyerID;
                rslt.BuyerName = item.BuyerID == null ? "" : _unit.SysBuyerRepository.GetByID(item.BuyerID).BuyerName;
                rslt.BuyerOrderID = item.BuyerOrderID;
                rslt.BuyerOrderNo = item.BuyerOrderID == null
                    ? ""
                    : _unit.SlsBuyerOrederRepository.GetByID(item.BuyerOrderID).BuyerOrderNo;
                rslt.ArticleID = item.ArticleID;
                rslt.ArticleNo = item.ArticleNo;
                rslt.ArticleChallanID = item.ArticleChallanID;
                rslt.ArticleChallanNo = item.ArticleChallanNo;
                rslt.ArticleName = item.ArticleID == null
                    ? ""
                    : _unit.ArticleRepository.GetByID(item.ArticleID).ArticleName;
                rslt.ItemTypeID = item.ItemTypeID;
                rslt.ItemTypeName = item.ItemTypeID == null
                    ? ""
                    : _unit.SysItemTypeRepository.GetByID(item.ItemTypeID).ItemTypeName;
                rslt.LeatherTypeID = item.LeatherTypeID;
                rslt.LeatherTypeName = item.LeatherTypeID == null
                    ? ""
                    : _unit.SysLeatherTypeRepository.GetByID(item.LeatherTypeID).LeatherTypeName;
                rslt.LeatherStatusID = item.LeatherStatusID;
                rslt.LeatherStatusName = item.LeatherStatusID == null
                    ? ""
                    : _unit.SysLeatherStatusRepo.GetByID(item.LeatherStatusID).LeatherStatusName;
                rslt.Colors = new List<InvCrustLeatherIssueColor>();
                var rqClrs = _unit.FinishReqItemColor.Get().Where(ob => ob.RequisitionItemID == item.RequisitionItemID);
                foreach (var rqClr in rqClrs)
                {
                    var clr = new InvCrustLeatherIssueColor();
                    clr.ColorID = rqClr.ColorID;
                    clr.ColorName = rqClr.ColorID == null
                        ? ""
                        : _unit.SysColorRepository.GetByID(rqClr.ColorID).ColorName;
                    var clsngStkPcs = _context.INV_CrustBuyerStock.FirstOrDefault(
                        ob => ob.StoreID == issueFrom && ob.BuyerID == item.BuyerID &&
                        ob.BuyerOrderID == item.BuyerOrderID && ob.ArticleID == item.ArticleID &&
                        ob.ItemTypeID == item.ItemTypeID && ob.LeatherTypeID == item.LeatherTypeID &&
                        ob.LeatherStatusID == item.LeatherStatusID && ob.ColorID == rqClr.ColorID
                        && ob.CrustQCLabel == qcLabel && ob.ArticleChallanNo == item.ArticleChallanNo);
                    clr.GradeRange = clsngStkPcs == null ? "" : clsngStkPcs.GradeRange;
                    clr.IssuePcs = rqClr.ColorPcs ?? 0;
                    clr.IssueSide = rqClr.ColorSide ?? 0;
                    clr.IssueArea = rqClr.ColorArea ?? 0;
                    //clr.AreaUnit = rqClr.AreaUnit;
                    //clr.AreaUnitName = rqClr.AreaUnit == null
                    //    ? ""
                    //    : _unit.SysUnitRepository.GetByID(rqClr.AreaUnit).UnitName;
                    clr.ClosingStockPcs = clsngStkPcs == null ? 0 : clsngStkPcs.ClosingStockPcs;
                    var clsngStkArea = _context.INV_CrustBuyerStock.FirstOrDefault(
                        ob => ob.StoreID == issueFrom && ob.BuyerID == item.BuyerID &&
                              ob.BuyerOrderID == item.BuyerOrderID && ob.ArticleID == item.ArticleID &&
                              ob.ItemTypeID == item.ItemTypeID && ob.LeatherTypeID == item.LeatherTypeID &&
                              ob.LeatherStatusID == item.LeatherStatusID && ob.ColorID == rqClr.ColorID
                              && ob.CrustQCLabel == qcLabel && ob.ArticleChallanNo == item.ArticleChallanNo);
                    clr.ClosingStockArea = clsngStkArea == null ? 0 : clsngStkArea.ClosingStockArea;
                    var clsngStkSide = _context.INV_CrustBuyerStock.FirstOrDefault(
                        ob => ob.StoreID == issueFrom && ob.BuyerID == item.BuyerID &&
                              ob.BuyerOrderID == item.BuyerOrderID && ob.ArticleID == item.ArticleID &&
                              ob.ItemTypeID == item.ItemTypeID && ob.LeatherTypeID == item.LeatherTypeID &&
                              ob.LeatherStatusID == item.LeatherStatusID && ob.ColorID == rqClr.ColorID
                              && ob.CrustQCLabel == qcLabel && ob.ArticleChallanNo == item.ArticleChallanNo);
                    clr.ClosingStockSide = clsngStkSide == null ? 0 : clsngStkSide.ClosingStockSide;
                    clr.CrustQCLabel = qcLabel;
                    clr.ArticleColorNo = rqClr.ArticleColorNo;
                    rslt.Colors.Add(clr);
                }
                result.Add(rslt);
            }

            return result;
        }

        public object GetByStoreId(int storeId, string qcLabel)
        {
            var query = new StringBuilder();

            query.Append("SELECT DISTINCT CBS.TransectionID, CBS.ArticleID, CBS.ArticleChallanID, CBS.ArticleChallanNo, ");
            query.Append("AR.ArticleName, CBS.BuyerID, B.BuyerName, CBS.BuyerOrderID, BO.BuyerOrderNo, CBS.ItemTypeID, ");
            query.Append(" IT.ItemTypeName, CBS.LeatherStatusID, LS.LeatherStatusName, CBS.LeatherTypeID, LT.LeatherTypeName ");
            query.Append("FROM INV_CrustBuyerStock CBS ");
            query.Append("INNER JOIN (SELECT MAX(TransectionID) TransectionID,[StoreID],[ArticleID], BuyerID, BuyerOrderID, ");
            query.Append("ItemTypeID, LeatherStatusID, LeatherTypeID ");
            query.Append("FROM dbo.INV_CrustBuyerStock  WHERE (ClosingStockPcs > 0 OR ClosingStockArea>0 OR ClosingStockSide>0) ");
            query.Append("AND  StoreID=" + storeId + " ");
            if (qcLabel != "ALL" || qcLabel != null)
            {
                query.Append("AND CrustQCLabel='" + qcLabel + "' ");
            }
            query.Append("GROUP BY [StoreID],[ArticleID], BuyerID, BuyerOrderID, ItemTypeID, LeatherStatusID, LeatherTypeID ) ");
            query.Append("SUB ON SUB.TransectionID=CBS.TransectionID ");
            query.Append("LEFT JOIN Sys_Article AR ON AR.ArticleID=CBS.ArticleID ");
            query.Append("LEFT JOIN Sys_Buyer B ON B.BuyerID=CBS.BuyerID ");
            query.Append("LEFT JOIN SLS_BuyerOrder BO ON BO.BuyerOrderID=CBS.BuyerOrderID ");
            query.Append("LEFT JOIN Sys_Color C ON C.ColorID=CBS.ColorID ");
            query.Append("LEFT JOIN Sys_Grade G ON G.GradeID=CBS.GradeID ");
            query.Append("LEFT JOIN Sys_ItemType IT ON IT.ItemTypeID=CBS.ItemTypeID ");
            query.Append("LEFT JOIN Sys_LeatherStatus LS ON LS.LeatherStatusID=CBS.LeatherStatusID ");
            query.Append("LEFT JOIN Sys_LeatherType LT ON LT.LeatherTypeID=CBS.LeatherTypeID ");

            var items = _context.Database.SqlQuery<ShowData>(query.ToString()).ToList();
            return items;
        }

        public object GetIssueById(long issueId)
        {
            //var query = "SELECT "
            return null;

        }

        public object GetClosingStkPcs(byte? storeId, int? buyerId, long? buyerOrderId, int? articleId, byte? itemTypeId,
            byte? leatherTypeId, byte? leatherStatusId, int? colorId, long? articleChallanId)
        {
            var query = new StringBuilder();
            query.Append("SELECT ClosingStockPcs");
            query.Append(" FROM INV_CrustBuyerStock");
            query.Append(" WHERE 1");
            if (storeId != null)
            {
                query.Append(" AND BuyerID =" + storeId + " ");
            }
            if (buyerId != null)
            {
                query.Append(" AND BuyerID =" + buyerId + " ");
            }
            if (buyerOrderId != null)
            {
                query.Append(" AND BuyerOrderID =" + buyerOrderId + " ");
            }
            if (articleId != null)
            {
                query.Append(" AND ArticleID =" + articleId + " ");
            }
            if (itemTypeId != null)
            {
                query.Append(" AND ItemTypeID =" + itemTypeId + " ");
            }
            if (leatherTypeId != null)
            {
                query.Append(" AND LeatherTypeID =" + leatherTypeId + " ");
            }
            if (leatherStatusId != null)
            {
                query.Append(" AND LeatherStatusID =" + leatherStatusId + " ");
            }
            if (colorId != null)
            {
                query.Append(" AND ColorID =" + colorId + " ");
            }
            if (articleChallanId != null)
            {
                query.Append(" AND ArticleChallanID =" + articleChallanId + " ");
            }

            var closingStockPcs = _context.Database.SqlQuery<decimal>(query.ToString());

            return closingStockPcs;
        }
    }

    public class RequisitionVM
    {
        public long RequisitionDateID { get; set; }
        public string RequisitionNo { get; set; }
        public string RequiredDate { get; set; }
        public string RequisitionStatus { get; set; }
        public string RecordStatus { get; set; }
        public byte? IssueFrom { get; set; }
        public byte? IssueTo { get; set; }
    }

    public class ShowData
    {
        public long TransectionID { get; set; }
        public string CrustQCLabel { get; set; }
        public int? ArticleID { get; set; }
        public string ArticleName { get; set; }
        public long? ArticleChallanID { get; set; }
        public string ArticleChallanNo { get; set; }
        public int? BuyerID { get; set; }
        public string BuyerName { get; set; }
        public long? BuyerOrderID { get; set; }
        public string BuyerOrderNo { get; set; }
        public int? ColorID { get; set; }
        public string ColorName { get; set; }
        public Int16? GradeID { get; set; }
        public string GradeName { get; set; }
        public byte? ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public byte? LeatherStatusID { get; set; }
        public string LeatherStatusName { get; set; }
        public byte? LeatherTypeID { get; set; }
        public string LeatherTypeName { get; set; }
    }
}
