using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class BuyerPurcOrderAckController : Controller
    {
        private readonly UnitOfWork _unit;
        private ValidationMsg _validationMsg;
        private DalSlsBuyerOrder _dalSlsBuyerOrder;
        private int _userId;
        public BuyerPurcOrderAckController()
        {
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            _dalSlsBuyerOrder = new DalSlsBuyerOrder();
        }

        [CheckUserAccess("BuyerPurcOrderAck/BuyerPurcOrderAck")]
        public ActionResult BuyerPurcOrderAck()
        {
            return View();
        }

        public ActionResult GetBuyerOrder(int buyerId)
        {
            var orders = _unit.SlsBuyerOrederRepository.Get().Where(ob => ob.BuyerID == buyerId).ToList();
            var result = orders.Select(order => new
            {
                order.BuyerOrderID,
                order.BuyerOrderNo,
                BuyerOrderDate = string.Format("{0:dd/MM/yyyy}", order.BuyerOrderDate),
                RecordStatus = DalCommon.ReturnRecordStatus(order.RecordStatus) 
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(SlsBuyerOrder model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalSlsBuyerOrder.SaveAck(model, _userId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confirm(long id, string comment)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalSlsBuyerOrder.ConfirmAck(id, _userId, comment);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBuyerOrderById(long orderId)
        {
            var order = _unit.SlsBuyerOrederRepository.GetByID(orderId);
            var result = new SlsBuyerOrder
            {
                BuyerOrderID = order.BuyerOrderID,
                BuyerOrderNo = order.BuyerOrderNo,
                BuyerOrderDate = string.Format("{0:dd/MM/yyyy}", order.BuyerOrderDate),
                OrderNo = order.OrderNo,
                BuyerOrderCategory = DalCommon.ReturnOrderCategory(order.BuyerOrderCategory),
                BuyerOrderStatus = DalCommon.ReturnOrderStatus(order.BuyerOrderStatus),
                OrderFrom = order.OrderFrom,
                BuyerID = order.BuyerID,
                BuyerName = order.BuyerID == null ? "" : _unit.SysBuyerRepository.GetByID(order.BuyerID).BuyerName,
                BuyerAddressID = order.BuyerAddressID,
                BuyerAddress =
                    order.BuyerAddressID == null
                        ? ""
                        : _unit.BuyerAddressRepository.GetByID(order.BuyerAddressID).Address,
                BuyerLocalAgentID = order.BuyerLocalAgentID,
                LocalAgentName = order.BuyerLocalAgentID == null ? "" : _unit.SysBuyerRepository.GetByID(order.BuyerLocalAgentID).BuyerName,
                //LocalAgentName =
                //    order.BuyerLocalAgentID == null
                //        ? ""
                //        : _unit.SysBuyerRepository.GetByID(_unit.BuyerAgent.GetByID(order.BuyerLocalAgentID).AgentID)
                //            .BuyerName,
                BuyerForeignAgentID = order.BuyerForeignAgentID,
                ForeignAgentName = order.BuyerForeignAgentID == null ? "" : _unit.SysBuyerRepository.GetByID(order.BuyerForeignAgentID).BuyerName,
                //ForeignAgentName =
                //    order.BuyerForeignAgentID == null
                //        ? ""
                //        : _unit.SysBuyerRepository.GetByID(_unit.BuyerAgent.GetByID(order.BuyerForeignAgentID).AgentID)
                //            .BuyerName,
                BuyerRef = order.BuyerRef,
                ExpectedShipmentDate = string.Format("{0:dd/MM/yyyy}", order.ExpectedShipmentDate),
                ProposedShipmentDate = string.Format("{0:dd/MM/yyyy}", order.ProposedShipmentDate),
                DeliveryLastDate = string.Format("{0:dd/MM/yyyy}", order.DeliveryLastDate),
                ExpectedProdStartDate = string.Format("{0:dd/MM/yyyy}", order.ExpectedProdStartDate),
                RevisionNo = order.RevisionNo,
                RevisionDate = string.Format("{0:dd/MM/yyyy}", order.RevisionDate),
                CheckNote = order.CheckNote,
                ApprovalNote = order.ApprovalNote,
                AcknowledgeNote = order.AcknowledgeNote,
                AcknowledgementStatus = order.AcknowledgementStatus,
                AckDate = string.Format("{0:dd/MM/yyyy}", order.AckDate),
                RecordStatus = order.RecordStatus,
                OrderCurrency = order.OrderCurrency,
                TotalFootQty = order.TotalFootQty,
                TotalMeterQty = order.TotalMeterQty,
                PriceLevel = order.PriceLevel,
                OrderItems = _unit.SlsBuyerOrderItemRepository.Get().Where(ob => ob.BuyerOrderID == order.BuyerOrderID)
                    .Select(item => new SlsBuyerOrderItem
                    {
                        BuyerOrderItemID = item.BuyerOrderItemID,
                        BuyerOrderID = item.BuyerOrderID,
                        commodity = item.commodity,
                        HSCode = item.HSCode,
                        ArticleID = item.ArticleID,
                        ArticleName =
                            item.ArticleID == null ? "" : _unit.ArticleRepository.GetByID(item.ArticleID).ArticleName,
                        ArticleNo = item.ArticleNo,
                        AvgSize = item.AvgSize,
                        AvgSizeUnit = item.AvgSizeUnit,
                        AvgSizeUnitName =
                            item.AvgSizeUnit == null ? "" : _unit.SysUnitRepository.GetByID(item.AvgSizeUnit).UnitName,
                        SideDescription = item.SideDescription,
                        SelectionRange = item.SelectionRange,
                        Thickness = item.Thickness,
                        ThicknessUnit = item.ThicknessUnit,
                        ThicknessUnitName =
                            item.ThicknessUnit == null
                                ? ""
                                : _unit.SysUnitRepository.GetByID(item.ThicknessUnit).UnitName,
                        ThicknessAt = item.ThicknessAt,
                        ItemTypeID = item.ItemTypeID,
                        ItemTypeName = item.ItemTypeID == null ? "" : _unit.SysItemTypeRepository.GetByID(item.ItemTypeID).ItemTypeName,
                        LeatherTypeID = item.LeatherTypeID,
                        LeatherTypeName = item.LeatherTypeID == null ? "" : _unit.SysLeatherTypeRepository.GetByID(item.LeatherTypeID).LeatherTypeName,
                        LeatherStatusID = item.LeatherStatusID,
                        LeatherStatusName = item.LeatherStatusID == null ? "" : _unit.SysLeatherStatusRepo.GetByID(item.LeatherStatusID).LeatherStatusName,
                        ArticleFootQty = item.ArticleFootQty,
                        ArticleMeterQty = item.ArticleMeterQty,
                        AirFootUnitPrice = item.AirFootUnitPrice,
                        AirFootTotalPrice = item.AirFootTotalPrice,
                        RoadFootUnitPrice = item.RoadFootUnitPrice,
                        RoadFootTotalPrice = item.RoadFootTotalPrice,
                        SeaFootUnitPrice = item.SeaFootUnitPrice,
                        SeaFootTotalPrice = item.SeaFootTotalPrice,
                        AirMeterUnitPrice = item.AirMeterUnitPrice,
                        AirMeterTotalPrice = item.AirMeterTotalPrice,
                        RoadMeterUnitPrice = item.RoadMeterUnitPrice,
                        RoadMeterTotalPrice = item.RoadMeterTotalPrice,
                        SeaMeterUnitPrice = item.SeaMeterUnitPrice,
                        SeaMeterTotalPrice = item.SeaMeterTotalPrice,
                        ItemColors =
                            _unit.SlsBuyerOrderItemColorRepository.Get()
                                .Where(ob => ob.BuyerOrderItemID == item.BuyerOrderItemID)
                                .Select(color => new SlsBuyerOrderItemColor
                                {
                                    BuyerOrdItemColorID = color.BuyerOrdItemColorID,
                                    BuyerOrderItemID = color.BuyerOrderItemID,
                                    BuyerOrderID = color.BuyerOrderID,
                                    ColorID = color.ColorID,
                                    ColorName =
                                        color.ColorID == null
                                            ? ""
                                            : _unit.SysColorRepository.GetByID(color.ColorID).ColorName,
                                    ColorFootQty = color.ColorFootQty,
                                    ColorMeterQty = color.ColorMeterQty,
                                    AirFootUnitPrice = color.AirFootUnitPrice,
                                    AirFootTotalPrice = color.AirFootTotalPrice,
                                    RoadFootUnitPrice = color.RoadFootUnitPrice,
                                    RoadFootTotalPrice = color.RoadFootTotalPrice,
                                    SeaFootUnitPrice = color.SeaFootUnitPrice,
                                    SeaFootTotalPrice = color.SeaFootTotalPrice,
                                    AirMeterUnitPrice = color.AirMeterUnitPrice,
                                    AirMeterTotalPrice = color.AirMeterTotalPrice,
                                    RoadMeterUnitPrice = color.RoadMeterUnitPrice,
                                    RoadMeterTotalPrice = color.RoadMeterTotalPrice,
                                    SeaMeterUnitPrice = color.SeaMeterUnitPrice,
                                    SeaMeterTotalPrice = color.SeaMeterTotalPrice,

                                }).ToList()
                    }).ToList(),
                OrderDelivery = _unit.SlsBuyerOrderDeliveryRepository.Get().Where(ob => ob.BuyerOrderID == order.BuyerOrderID).Select(dlvry => new SlsBuyerOrderDelivery
                {
                    BuyerOrderDeliveryID = dlvry.BuyerOrderDeliveryID,
                    BuyerOrderID = dlvry.BuyerOrderID,
                    ArticleID = dlvry.ArticleID,
                    ArticleName = dlvry.ArticleID == null ? "" : _unit.ArticleRepository.GetByID(dlvry.ArticleID).ArticleName,
                    ColorID = dlvry.ColorID,
                    ColorName = dlvry.ColorID == null ? "" : _unit.SysColorRepository.GetByID(dlvry.ColorID).ColorName,
                    OrdDeliverySL = dlvry.OrdDeliverySL,
                    OrdDeliveryDate = string.Format("{0: dd/MM/yyyy}", dlvry.OrdDeliveryDate),
                    OrdDateFootQty = dlvry.OrdDateFootQty,
                    OrdDateMeterQty = dlvry.OrdDateMeterQty
                }).OrderBy(ob => ob.OrdDeliverySL).ToList(),
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}