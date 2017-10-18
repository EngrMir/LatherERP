using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System.Data.Entity.Validation;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalSlsBuyerOrder
    {
        private UnitOfWork _unit;
        private ValidationMsg _validationMsg;
        private int _mode;
        private bool _save;
        private readonly BLC_DEVEntities _context;
        public DalSlsBuyerOrder()
        {
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            _mode = 0;
            _context = new BLC_DEVEntities();
        }

        public ValidationMsg Save(SlsBuyerOrder model, int userId, string url)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var order = ConvertOrder(model, userId, url);
                        if (model.BuyerOrderID == 0)
                        {
                            _context.SLS_BuyerOrder.Add(order);
                            _context.SaveChanges();
                            _mode = 1;
                        }
                        else
                        {
                            _context.SaveChanges();
                            _mode = 2;
                        }
                        if (model.OrderItems != null)
                        {
                            foreach (var item in model.OrderItems)
                            {
                                var orderItem = ConvertOrderItem(item, userId, order.BuyerOrderID);
                                if (item.BuyerOrderItemID == 0)
                                {
                                    _context.SLS_BuyerOrderItem.Add(orderItem);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    _context.SaveChanges();
                                }
                                if (item.ItemColors != null)
                                {
                                    foreach (var color in item.ItemColors)
                                    {
                                        var itemColor = ConvertOrderItemColor(color, userId, orderItem.BuyerOrderItemID,
                                            order.BuyerOrderID);
                                        if (color.BuyerOrdItemColorID == 0)
                                        {
                                            _context.SLS_BuyerOrderItemColor.Add(itemColor);
                                            _context.SaveChanges();
                                        }
                                        else
                                        {
                                            _context.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        if (model.OrderDelivery != null)
                        {
                            foreach (var x in model.OrderDelivery)
                            {
                                var orderDelivery = ConvertOrderDelivery(x, userId, order.BuyerOrderID);
                                if (x.BuyerOrderDeliveryID == 0)
                                {
                                    _context.SLS_BuyerOrderDelivery.Add(orderDelivery);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    _context.SaveChanges();
                                }
                            }
                        }
                        tx.Complete();
                        if (_mode == 1)
                        {
                            _validationMsg.ReturnId = order.BuyerOrderID;
                            _validationMsg.ReturnCode = order.OrderNo;
                            _validationMsg.Type = Enums.MessageType.Success;
                            _validationMsg.Msg = "Saved successfully.";
                        }
                        if (_mode == 2)
                        {
                            _validationMsg.ReturnId = order.BuyerOrderID;
                            _validationMsg.Type = Enums.MessageType.Update;
                            _validationMsg.Msg = "Updated successfully.";
                        }
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                //StringBuilder sb = new StringBuilder();
                //foreach (var eve in e.EntityValidationErrors)
                //{
                //    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //                                    eve.Entry.Entity.GetType().Name,
                //                                    eve.Entry.State));
                //    foreach (var ve in eve.ValidationErrors)
                //    {
                //        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                //                                    ve.PropertyName,
                //                                    ve.ErrorMessage));
                //    }
                //}
                //throw new DbEntityValidationException(sb.ToString(), e);
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to save.";
            }
            return _validationMsg;
        }

        public ValidationMsg Check(long buyerOrderId, int userId, string comment)
        {
            try
            {
                var order = _unit.SlsBuyerOrederRepository.GetByID(buyerOrderId);
                if (order != null)
                {
                    if (order.RecordStatus == "CHK" || order.RecordStatus == "CNF")
                    {
                        _validationMsg.Type = Enums.MessageType.Error;
                        _validationMsg.Msg = "Order is already checked.";
                    }
                    else
                    {
                        order.CheckedBy = userId;
                        order.CheckDate = DateTime.Now;
                        order.CheckNote = comment;
                        order.RecordStatus = "CHK";
                        _unit.SlsBuyerOrederRepository.Update(order);
                    }
                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.Type = Enums.MessageType.Success;
                    _validationMsg.Msg = "Checked successfully.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to check.";
            }
            return _validationMsg;
        }

        public ValidationMsg Confirm(long buyerOrderId, int userId, string comment)
        {
            try
            {
                var order = _unit.SlsBuyerOrederRepository.GetByID(buyerOrderId);
                if (order != null)
                {
                    if (order.RecordStatus == "CNF")
                    {
                        _validationMsg.Type = Enums.MessageType.Error;
                        _validationMsg.Msg = "Order is already confirmed.";
                    }
                    else
                    {
                        order.ApprovedBy = userId;
                        order.ApproveDate = DateTime.Now;
                        order.ApprovalNote = comment;
                        order.RecordStatus = "CNF";

                        _unit.SlsBuyerOrederRepository.Update(order);
                    }

                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.Type = Enums.MessageType.Success;
                    _validationMsg.Msg = "Confirmed successfully.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to confirm.";
            }
            return _validationMsg;
        }

        public ValidationMsg DeleteItemColor(long colorId)
        {
            try
            {
                var color = _unit.SlsBuyerOrderItemColorRepository.GetByID(colorId);
                if (color != null)
                {
                    _unit.SlsBuyerOrderItemColorRepository.Delete(color);
                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.Type = Enums.MessageType.Delete;
                    _validationMsg.Msg = "Color deleted successfully.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete color.";
            }
            return _validationMsg;
        }

        public ValidationMsg DeleteItem(long itemId)
        {
            try
            {
                var item = _unit.SlsBuyerOrderItemRepository.GetByID(itemId);
                if (item != null)
                {
                    var colors =
                        _unit.SlsBuyerOrderItemColorRepository.Get()
                            .Where(ob => ob.BuyerOrderItemID == item.BuyerOrderItemID)
                            .ToList();
                    if (colors.Count > 0)
                    {
                        foreach (var color in colors)
                        {
                            _unit.SlsBuyerOrderItemColorRepository.Delete(color);
                        }
                    }
                    _unit.SlsBuyerOrderItemRepository.Delete(item);
                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.Type = Enums.MessageType.Delete;
                    _validationMsg.Msg = "Item deleted successfully.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete item.";

            }
            return _validationMsg;
        }

        public ValidationMsg Delete(long orderId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        var order = _context.SLS_BuyerOrder.FirstOrDefault(ob => ob.BuyerOrderID == orderId);
                        if (order != null)
                        {
                            var items = _context.SLS_BuyerOrderItem.Where(ob => ob.BuyerOrderID == orderId).ToList();
                            if (items.Count > 0)
                            {
                                foreach (var item in items)
                                {
                                    var colors =
                                        _context.SLS_BuyerOrderItemColor.Where(
                                            ob => ob.BuyerOrderItemID == item.BuyerOrderItemID).ToList();

                                    if (colors.Count > 0)
                                    {
                                        foreach (var color in colors)
                                        {
                                            _context.SLS_BuyerOrderItemColor.Remove(color);
                                        }
                                    }
                                    _context.SLS_BuyerOrderItem.Remove(item);
                                }
                            }

                            var dlvry = _context.SLS_BuyerOrderDelivery.Where(ob => ob.BuyerOrderID == orderId).ToList();
                            if (dlvry.Count > 0)
                            {
                                foreach (var dlv in dlvry)
                                {
                                    _context.SLS_BuyerOrderDelivery.Remove(dlv);
                                }
                            }
                            _context.SLS_BuyerOrder.Remove(order);
                            _context.SaveChanges();
                        }
                    }
                    tx.Complete();
                }

                _validationMsg.Type = Enums.MessageType.Delete;
                _validationMsg.Msg = "Order successfully deleted.";

            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete order.";
            }
            return _validationMsg;
        }

        public ValidationMsg SaveAck(SlsBuyerOrder model, int userId)
        {
            try
            {
                var entity = _unit.SlsBuyerOrederRepository.GetByID(model.BuyerOrderID);
                entity.AcknowledgementStatus = model.AcknowledgementStatus;
                entity.AckDate = model.AckDate == null ? (DateTime?)null : DalCommon.SetDate(model.AckDate);
                entity.ProposedShipmentDate = model.ProposedShipmentDate == null ? (DateTime?)null : DalCommon.SetDate(model.ProposedShipmentDate);
                entity.DeliveryLastDate = model.DeliveryLastDate == null ? (DateTime?)null : DalCommon.SetDate(model.DeliveryLastDate);
                entity.ExpectedProdStartDate = model.ExpectedProdStartDate == null ? (DateTime?)null : DalCommon.SetDate(model.ExpectedProdStartDate);
                entity.AcknowledgeNote = model.AcknowledgeNote;
                entity.ModifiedBy = userId;
                entity.ModifiedOn = DateTime.Now;
                _unit.SlsBuyerOrederRepository.Update(entity);
                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.ReturnId = entity.BuyerOrderID;
                    _validationMsg.Type = Enums.MessageType.Success;
                    _validationMsg.Msg = "Saved successfully.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to save.";
            }
            return _validationMsg;
        }

        public ValidationMsg ConfirmAck(long id, int userId, string comment)
        {
            try
            {
                var entity = _unit.SlsBuyerOrederRepository.GetByID(id);
                if (entity != null)
                {
                    entity.AcknowledgeNote = comment;
                    entity.RecordStatus = "ACK";
                    entity.BuyerOrderStatus = "OG";
                    entity.AcknowledgedBy = userId;
                    entity.AckDate = DateTime.Now;
                    _unit.SlsBuyerOrederRepository.Update(entity);
                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.Type = Enums.MessageType.Success;
                    _validationMsg.Msg = "Confirmed successfully.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to confirm.";
            }
            return _validationMsg;
        }

        private SLS_BuyerOrder ConvertOrder(SlsBuyerOrder model, int userId, string url)
        {
            var entity = model.BuyerOrderID == 0 ? new SLS_BuyerOrder() : (from b in _context.SLS_BuyerOrder.AsEnumerable()
                                                                           where b.BuyerOrderID == model.BuyerOrderID
                                                                           select b).FirstOrDefault();
            entity.BuyerOrderID = model.BuyerOrderID;
            entity.BuyerOrderNo = model.BuyerOrderNo ?? DalCommon.GetPreDefineNextCodeByUrl(url);
            entity.OrderNo = model.OrderNo;
            entity.BuyerOrderDate = model.BuyerOrderDate == null ? DateTime.Now : DalCommon.SetDate(model.BuyerOrderDate);
            entity.OrderFrom = model.OrderFrom;
            entity.BuyerOrderCategory = model.BuyerOrderCategory;
            entity.BuyerID = model.BuyerID;
            entity.BuyerAddressID = model.BuyerAddressID;
            entity.BuyerLocalAgentID = model.BuyerLocalAgentID;
            entity.BuyerForeignAgentID = model.BuyerForeignAgentID;
            entity.BuyerRef = model.BuyerRef;
            entity.ExpectedShipmentDate = model.ExpectedShipmentDate == null ? DateTime.Now : DalCommon.SetDate(model.ExpectedShipmentDate);
            entity.RevisionNo = model.RevisionNo;
            entity.RevisionDate = DalCommon.SetDate(model.RevisionDate);
            entity.OrderCurrency = model.OrderCurrency;
            entity.BuyerOrderStatus = model.BuyerOrderStatus == null ? "OD" : DalCommon.ReverseOrderStatus(model.BuyerOrderStatus);
            entity.RecordStatus = model.BuyerOrderID == 0
                ? "NCF"
                : _unit.SlsBuyerOrederRepository.GetByID(model.BuyerOrderID).RecordStatus;
            entity.SetOn = model.BuyerOrderID == 0
                ? DateTime.Now
                : _unit.SlsBuyerOrederRepository.GetByID(model.BuyerOrderID).SetOn;
            entity.SetBy = model.BuyerOrderID == 0
                ? userId
                : _unit.SlsBuyerOrederRepository.GetByID(model.BuyerOrderID).SetBy;
            entity.ModifiedBy = model.BuyerOrderID == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.BuyerOrderID == 0 ? (DateTime?)null : DateTime.Now;
            entity.OrderCurrency = model.OrderCurrency;
            entity.TotalFootQty = model.TotalFootQty;
            entity.TotalMeterQty = model.TotalMeterQty;
            entity.PriceLevel = model.PriceLevel;
            return entity;
        }

        private SLS_BuyerOrderItem ConvertOrderItem(SlsBuyerOrderItem model, int userId, long buyerOrderId)
        {
            var entity = model.BuyerOrderItemID == 0 ? new SLS_BuyerOrderItem() : (from b in _context.SLS_BuyerOrderItem.AsEnumerable()
                                                                                   where b.BuyerOrderItemID == model.BuyerOrderItemID
                                                                                   select b).FirstOrDefault();
            entity.BuyerOrderItemID = model.BuyerOrderItemID;
            entity.BuyerOrderID = model.BuyerOrderID ?? buyerOrderId;
            entity.commodity = model.commodity;
            entity.HSCode = model.HSCode;
            entity.ArticleID = model.ArticleID;
            entity.ArticleNo = model.ArticleNo;
            entity.AvgSize = model.AvgSize;
            entity.AvgSizeUnit = model.AvgSizeUnit == null ? _context.Sys_Unit.FirstOrDefault(ob => ob.UnitName == model.AvgSizeUnitName).UnitID : model.AvgSizeUnit;
            entity.SideDescription = model.SideDescription;
            entity.SelectionRange = model.SelectionRange;
            entity.Thickness = model.Thickness;
            entity.ThicknessUnit = model.ThicknessUnit == null ? _context.Sys_Unit.FirstOrDefault(ob => ob.UnitName == model.ThicknessUnitName).UnitID : model.ThicknessUnit;
            entity.ThicknessAt = model.ThicknessAt;
            entity.LeatherStatusID = model.LeatherStatusID;
            entity.ItemTypeID = model.ItemTypeID;
            entity.LeatherTypeID = model.LeatherTypeID;
            entity.SetOn = model.BuyerOrderItemID == 0
                ? DateTime.Now
                : _unit.SlsBuyerOrderItemRepository.GetByID(model.BuyerOrderItemID).SetOn;
            entity.SetBy = model.BuyerOrderItemID == 0
                ? userId
                : _unit.SlsBuyerOrderItemRepository.GetByID(model.BuyerOrderItemID).SetBy;
            entity.ModifiedBy = model.BuyerOrderItemID == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.BuyerOrderItemID == 0 ? (DateTime?)null : DateTime.Now;
            entity.ArticleFootQty = model.ArticleFootQty;
            entity.SeaFootUnitPrice = model.SeaFootUnitPrice;
            entity.SeaFootTotalPrice = model.SeaFootTotalPrice;
            entity.AirFootUnitPrice = model.AirFootUnitPrice;
            entity.AirFootTotalPrice = model.AirFootTotalPrice;
            entity.RoadFootUnitPrice = model.RoadFootUnitPrice;
            entity.RoadFootTotalPrice = model.RoadFootTotalPrice;
            entity.ArticleMeterQty = model.ArticleMeterQty;
            entity.SeaMeterUnitPrice = model.SeaMeterUnitPrice;
            entity.SeaMeterTotalPrice = model.SeaMeterTotalPrice;
            entity.AirMeterUnitPrice = model.AirMeterUnitPrice;
            entity.AirMeterTotalPrice = model.AirMeterTotalPrice;
            entity.RoadMeterUnitPrice = model.RoadMeterUnitPrice;
            entity.RoadMeterTotalPrice = model.RoadMeterTotalPrice;

            return entity;
        }

        private SLS_BuyerOrderItemColor ConvertOrderItemColor(SlsBuyerOrderItemColor model, int userId,
            long buyerOrderItemId, long? buyerOrderId)
        {
            var entity = model.BuyerOrdItemColorID == 0 ? new SLS_BuyerOrderItemColor() : (from b in _context.SLS_BuyerOrderItemColor.AsEnumerable()
                                                                                           where b.BuyerOrdItemColorID == model.BuyerOrdItemColorID
                                                                                           select b).FirstOrDefault();
            entity.BuyerOrdItemColorID = model.BuyerOrdItemColorID;
            entity.BuyerOrderItemID = model.BuyerOrderItemID ?? buyerOrderItemId;
            entity.BuyerOrderID = model.BuyerOrderID ?? buyerOrderId;
            entity.ColorID = model.ColorID;
            entity.ColorFootQty = model.ColorFootQty;
            entity.SeaFootUnitPrice = model.SeaFootUnitPrice;
            entity.SeaFootTotalPrice = model.SeaFootTotalPrice;
            entity.AirFootUnitPrice = model.AirFootUnitPrice;
            entity.AirFootTotalPrice = model.AirFootTotalPrice;
            entity.RoadFootUnitPrice = model.RoadFootUnitPrice;
            entity.RoadFootTotalPrice = model.RoadFootTotalPrice;
            entity.ColorMeterQty = model.ColorMeterQty;
            entity.SeaMeterUnitPrice = model.SeaMeterUnitPrice;
            entity.SeaMeterTotalPrice = model.SeaMeterTotalPrice;
            entity.AirMeterUnitPrice = model.AirMeterUnitPrice;
            entity.AirMeterTotalPrice = model.AirMeterTotalPrice;
            entity.RoadMeterUnitPrice = model.RoadMeterUnitPrice;
            entity.RoadMeterTotalPrice = model.RoadMeterTotalPrice;
            entity.SetBy = model.BuyerOrdItemColorID == 0
                ? userId
                : _unit.SlsBuyerOrderItemColorRepository.GetByID(model.BuyerOrdItemColorID).SetBy;
            entity.SetOn = model.BuyerOrdItemColorID == 0
                ? DateTime.Now
                : _unit.SlsBuyerOrderItemColorRepository.GetByID(model.BuyerOrdItemColorID).SetOn;
            entity.ModifiedBy = model.BuyerOrdItemColorID == 0 ? (int?)null : userId;
            entity.ModifiedOn = model.BuyerOrdItemColorID == 0 ? (DateTime?)null : DateTime.Now;

            return entity;
        }

        private SLS_BuyerOrderDelivery ConvertOrderDelivery(SlsBuyerOrderDelivery model, int userId, long buyerOrderId)
        {
            var entity = model.BuyerOrderDeliveryID == 0
                ? new SLS_BuyerOrderDelivery()
                : (from b in _context.SLS_BuyerOrderDelivery.AsEnumerable()
                   where b.BuyerOrderDeliveryID == model.BuyerOrderDeliveryID
                   select b).FirstOrDefault();

            entity.BuyerOrderDeliveryID = model.BuyerOrderDeliveryID;
            entity.BuyerOrderID = buyerOrderId;
            entity.OrdDeliverySL = model.OrdDeliverySL;
            entity.OrdDeliveryDate = model.OrdDeliveryDate.Contains("/")
                ? DalCommon.SetDate(model.OrdDeliveryDate.Trim())
                : DalCommon.SetDate(Convert.ToDateTime(model.OrdDeliveryDate).ToString("dd/MM/yyyy"));
            entity.ArticleID = model.ArticleID;
            entity.ColorID = model.ColorID;
            entity.OrdDateFootQty = model.OrdDateFootQty;
            entity.OrdDateMeterQty = model.OrdDateMeterQty;
            entity.SetOn = model.BuyerOrderDeliveryID == 0
                ? DateTime.Now
                : _unit.SlsBuyerOrderDeliveryRepository.GetByID(model.BuyerOrderDeliveryID).SetOn;
            entity.SetBy = model.BuyerOrderDeliveryID == 0
                ? userId
                : _unit.SlsBuyerOrderDeliveryRepository.GetByID(model.BuyerOrderDeliveryID).SetBy;
            entity.ModifiedOn = model.BuyerOrderDeliveryID == 0 ? (DateTime?)null : DateTime.Now;
            entity.ModifiedBy = model.BuyerOrderDeliveryID == 0 ? (int?)null : userId;
            entity.IsActive = true;
            return entity;
        }

        public IEnumerable<string> GetBuyerListForSearch()
        {
            return _context.Sys_Buyer.Where(m => m.IsActive != false).Select(m => m.BuyerName).ToList();
        }

        public List<SysBuyer> GetBuyerList(string buyer)
        {
            List<Sys_Buyer> searchList = _context.Sys_Buyer.Where(m => m.IsActive != false && m.BuyerName.StartsWith(buyer)).ToList();

            return searchList.Select(SetToBuyerModel).ToList();
        }

        private SysBuyer SetToBuyerModel(Sys_Buyer entity)
        {
            var model = new SysBuyer();
            model.BuyerID = entity.BuyerID;
            model.BuyerName = entity.BuyerName;
            model.BuyerCode = entity.BuyerCode;
            model.BuyerAddressID =
                _context.Sys_BuyerAddress.FirstOrDefault(ob => ob.BuyerID == entity.BuyerID && ob.IsActive == true)
                    .BuyerAddressID.ToString(CultureInfo.InvariantCulture);
            model.Address =
                _context.Sys_BuyerAddress.FirstOrDefault(ob => ob.BuyerID == entity.BuyerID && ob.IsActive == true)
                    .Address;
            return model;
        }

        public IEnumerable<string> GetLocalAgentListForSearch(string lAgent)
        {
            return
                _context.Sys_Buyer.Where(
                    m => m.IsActive != false && m.BuyerCategory == "Buyer Agent" && m.BuyerType == "Local Agent")
                    .Select(m => m.BuyerName)
                    .ToList();
        }

        public List<SysBuyer> GetLocalAgentList(string lAgent)
        {
            List<Sys_Buyer> searchList =
                _context.Sys_Buyer.Where(
                    m => m.IsActive != false && m.BuyerCategory == "Buyer Agent" && m.BuyerType == "Local Agent" &&
                        m.BuyerName.StartsWith(lAgent)).ToList();
            return searchList.Select(SetToLocalAgentModel).ToList();
        }

        private SysBuyer SetToLocalAgentModel(Sys_Buyer entity)
        {
            var model = new SysBuyer();
            model.BuyerID = entity.BuyerID;
            model.BuyerName = entity.BuyerName;
            model.BuyerCode = entity.BuyerCode;
            model.BuyerAddressID =
                _context.Sys_BuyerAddress.FirstOrDefault(ob => ob.BuyerID == entity.BuyerID && ob.IsActive == true)
                    .BuyerAddressID.ToString(CultureInfo.InvariantCulture);
            model.Address =
                _context.Sys_BuyerAddress.FirstOrDefault(ob => ob.BuyerID == entity.BuyerID && ob.IsActive == true)
                    .Address;
            return model;
        }

        public IEnumerable<string> GetForeignAgentListForSearch(string lAgent)
        {
            return
                _context.Sys_Buyer.Where(
                    m => m.IsActive != false && m.BuyerCategory == "Buyer Agent" && m.BuyerType == "Foreign Agent")
                    .Select(m => m.BuyerName)
                    .ToList();
        }

        public List<SysBuyer> GetForeignAgentList(string lAgent)
        {
            List<Sys_Buyer> searchList =
                _context.Sys_Buyer.Where(
                    m =>
                        m.IsActive != false && m.BuyerCategory == "Buyer Agent" && m.BuyerType == "Foreign Agent" &&
                        m.BuyerName.StartsWith(lAgent)).ToList();

            return searchList.Select(SetToForeignAgentModel).ToList();
        }

        private SysBuyer SetToForeignAgentModel(Sys_Buyer entity)
        {
            var model = new SysBuyer();
            model.BuyerID = entity.BuyerID;
            model.BuyerName = entity.BuyerName;
            model.BuyerCode = entity.BuyerCode;
            model.BuyerAddressID =
                _context.Sys_BuyerAddress.FirstOrDefault(ob => ob.BuyerID == entity.BuyerID && ob.IsActive == true)
                    .BuyerAddressID.ToString(CultureInfo.InvariantCulture);
            model.Address =
                _context.Sys_BuyerAddress.FirstOrDefault(ob => ob.BuyerID == entity.BuyerID && ob.IsActive == true)
                    .Address;
            return model;
        }

        public IEnumerable<string> GetArticleListForSearch(string article)
        {
            return _context.Sys_Article.Where(m => m.IsActive != false).Select(m => m.ArticleName).ToList();
        }

        public List<SysArticle> GetArticleList(string article)
        {
            List<Sys_Article> searchList = _context.Sys_Article.Where(m => m.IsActive != false && m.ArticleName.StartsWith(article)).ToList();

            return searchList.Select(SetToArticleModel).ToList();
        }

        private SysArticle SetToArticleModel(Sys_Article entity)
        {
            var model = new SysArticle();
            model.ArticleID = entity.ArticleID;
            model.ArticleName = entity.ArticleName;
            model.ArticleNo = entity.ArticleNo;
            return model;
        }

        public IEnumerable<string> GetColorListForSearch(string color)
        {
            return _context.Sys_Color.Where(m => m.IsActive != false).Select(m => m.ColorName).ToList();
        }

        public List<SysColor> GetColorList(string color)
        {
            List<Sys_Color> searchList =
                _context.Sys_Color.Where(m => m.IsActive != false && m.ColorName.StartsWith(color)).ToList();

            return searchList.Select(SetToColorModel).ToList();
        }

        private SysColor SetToColorModel(Sys_Color entity)
        {
            var model = new SysColor();
            model.ColorID = entity.ColorID;
            model.ColorName = entity.ColorName;
            model.ColorCode = entity.ColorCode;
            return model;
        }

        public ValidationMsg DeleteDelivery(long orderId)
        {
            try
            {
                var dlvry = _unit.SlsBuyerOrderDeliveryRepository.Get().Where(ob => ob.BuyerOrderID == orderId).ToList();
                foreach (var x in dlvry)
                {
                    _unit.SlsBuyerOrderDeliveryRepository.Delete(x);
                }
                _save = _unit.IsSaved();
                if (_save)
                {
                    _validationMsg.Type = Enums.MessageType.Delete;
                    _validationMsg.Msg = "Color deleted successfully.";
                }
            }
            catch
            {
                _validationMsg.Type = Enums.MessageType.Error;
                _validationMsg.Msg = "Failed to delete delivery.";
            }


            return _validationMsg;
        }
        public List<SlsBuyerOrder> GetAllBuyerPurchaseOrder()
        {
            using (_context)
            {
                var data = (from o in _context.SLS_BuyerOrder.AsEnumerable()
                            //where o.RecordStatus =="CNF"
                            orderby o.BuyerOrderID descending
                            select new SlsBuyerOrder
                            {
                                BuyerOrderID = o.BuyerOrderID,
                                BuyerOrderNo = o.BuyerOrderNo,
                                OrderNo=o.OrderNo,
                                BuyerOrderDate = Convert.ToDateTime(o.BuyerOrderDate).ToString("dd'/'MM'/'yyyy"),
                            });
                return data.ToList();
            }

        }

        public bool CheckUniqueOrderNo(string orderNo)
        {
            var query = new StringBuilder();
            query.Append("SELECT  uci.BuyerOrderNo FROM [dbo].[SLS_BuyerOrder] AS uci");
            query.Append(" WHERE [BuyerOrderNo] = '" + orderNo + "'");

            var checkVal = _context.Database.SqlQuery<string>(query.ToString()).FirstOrDefault();
            return (Convert.ToString(checkVal) == orderNo) ? true : false;
        }
    }
}
