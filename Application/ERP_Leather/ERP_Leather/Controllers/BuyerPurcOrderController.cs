using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP_Leather.ActionFilters;
using Microsoft.Ajax.Utilities;

namespace ERP_Leather.Controllers
{
    public class BuyerPurcOrderController : Controller
    {
        private int _userId;
        private readonly UnitOfWork _unit;
        private readonly DalSysCurrency _objCurrency;
        private ValidationMsg _validationMsg;
        private readonly DalSlsBuyerOrder _dalSlsBuyerOrder;
        private readonly BLC_DEVEntities _context;

        public BuyerPurcOrderController()
        {
            _unit = new UnitOfWork();
            _context = new BLC_DEVEntities();
            _objCurrency = new DalSysCurrency();
            _validationMsg = new ValidationMsg();
            _dalSlsBuyerOrder = new DalSlsBuyerOrder();
        }

        [CheckUserAccess("BuyerPurcOrder/BuyerPurcOrder")]
        public ActionResult BuyerPurcOrder()
        {
            ViewBag.CurrencyList = _objCurrency.GetAllActiveCurrency();
            return View();
        }

        public ActionResult GetBuyer()
        {
            var buyers = _unit.SysBuyerRepository.Get().Where(ob => ob.BuyerCategory == "Buyer" && ob.IsActive == true).ToList();
            var result = buyers.Select(buyer => new
            {
                buyer.BuyerID,
                buyer.BuyerCode,
                buyer.BuyerName,
                buyer.BuyerType,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID && ob.IsActive == true).BuyerAddressID,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID && ob.IsActive == true).Address
            }).ToList();
            return Json(result.OrderBy(ob => ob.BuyerName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalBuyer(string buyerType)
        {
            var buyers = _unit.SysBuyerRepository.Get().Where(ob => ob.BuyerCategory == "Buyer" && (ob.IsActive == true && ob.BuyerType == buyerType)).ToList();
            var result = buyers.Select(buyer => new
            {
                buyer.BuyerID,
                buyer.BuyerCode,
                buyer.BuyerName,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID && ob.IsActive == true).BuyerAddressID,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID && ob.IsActive == true).Address
            }).ToList();
            return Json(result.OrderBy(ob => ob.BuyerName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBuyerById(int buyerId)
        {
            var buyer = _unit.SysBuyerRepository.GetByID(buyerId);
            var result = new
            {
                buyer.BuyerID,
                buyer.BuyerCode,
                buyer.BuyerName,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID).BuyerAddressID,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID).Address
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBuyerAgents(int buyerId)
        {
            var buyers = _unit.BuyerAgent.Get().Where(ob => ob.BuyerID == buyerId && ob.IsActive == true).ToList();
            var result = buyers.Select(buyer => new
            {
                buyer.BuyerAgentID,
                buyer.AgentID,
                AgentName = _unit.SysBuyerRepository.GetByID(buyer.AgentID).BuyerName,
                buyer.AgentType
            }).ToList().OrderBy(ob => ob.AgentName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalAgent()
        {
            var agents =
                _unit.SysBuyerRepository.Get()
                    .Where(
                        ob => ob.BuyerCategory == "Buyer Agent" && ob.BuyerType == "Local Agent" && ob.IsActive == true)
                    .ToList();

            var result = agents.Select(agent => new
            {
                agent.BuyerID,
                agent.BuyerCode,
                agent.BuyerName,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == agent.BuyerID).BuyerAddressID,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == agent.BuyerID).Address
            }).ToList();
            return Json(result.OrderBy(ob => ob.BuyerName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalAgentById(int agentId)
        {
            var agent = _unit.BuyerAgent.GetByID(agentId);
            var result = new
            {
                agent.BuyerAgentID,
                agent.AgentID,
                agent.BuyerID,
                AgentCode = _unit.SysBuyerRepository.GetByID(agent.AgentID).BuyerCode,
                AgentName = _unit.SysBuyerRepository.GetByID(agent.AgentID).BuyerName
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetForeignAgent()
        {
            var agents =
                _unit.SysBuyerRepository.Get()
                    .Where(
                        ob =>
                            ob.BuyerCategory == "Buyer Agent" && ob.BuyerType == "Foreign Agent" && ob.IsActive == true)
                    .ToList();
            var result = agents.Select(agent => new
            {
                agent.BuyerID,
                agent.BuyerCode,
                agent.BuyerName,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == agent.BuyerID).BuyerAddressID,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == agent.BuyerID).Address
            }).ToList();
            return Json(result.OrderBy(ob => ob.BuyerName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetForeignAgentById(int agentId)
        {
            var agent = _unit.BuyerAgent.GetByID(agentId);
            var result = new
            {
                agent.BuyerAgentID,
                agent.AgentID,
                agent.BuyerID,
                AgentCode = _unit.SysBuyerRepository.GetByID(agent.AgentID).BuyerCode,
                AgentName = _unit.SysBuyerRepository.GetByID(agent.AgentID).BuyerName
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAgentsBuyer(int agentId)
        {
            var agent = _unit.BuyerAgent.GetByID(agentId);
            var buyer = _unit.SysBuyerRepository.GetByID(agent.BuyerID);
            var result = new
            {
                buyer.BuyerID,
                buyer.BuyerName,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID && ob.IsActive == true).BuyerAddressID,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID && ob.IsActive == true).Address,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetArticles()
        {
            var articles = _unit.ArticleRepository.Get().ToList();
            var result = articles.Select(article => new
            {
                article.ArticleID,
                article.ArticleNo,
                article.ArticleName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColors()
        {
            var colors = _unit.SysColorRepository.Get().Where(ob => ob.IsActive == true).ToList();
            var result = colors.Select(color => new
            {
                color.ColorID,
                color.ColorCode,
                color.ColorName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetThicknessUnit()
        {
            var units = _unit.SysUnitRepository.Get().Where(ob => ob.UnitCategory == "Thickness").ToList();
            var result = units.Select(unit => new
            {
                unit.UnitID,
                unit.UnitName
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBuyerOrder()
        {
            var orders = _unit.SlsBuyerOrederRepository.Get().ToList();
            var result = orders.Select(order => new
            {
                order.BuyerOrderID,
                order.BuyerOrderNo,
                order.OrderNo,
                BuyerOrderDate = string.Format("{0:dd/MM/yyyy}", order.BuyerOrderDate),
                order.BuyerID,
                BuyerName = order.BuyerID == null ? "" : _unit.SysBuyerRepository.GetByID(order.BuyerID).BuyerName,
                RecordStatus = DalCommon.ReturnRecordStatus(order.RecordStatus)
            }).ToList();
            return Json(result.OrderByDescending(ob => ob.BuyerOrderID), JsonRequestBehavior.AllowGet);
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
                BuyerOrderCategory = order.BuyerOrderCategory,
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
                }).OrderBy(ob => ob.OrdDeliveryDate).ToList(),
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(SlsBuyerOrder model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalSlsBuyerOrder.Save(model, _userId, "BuyerPurcOrder/BuyerPurcOrder");
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Check(long orderId, string comment)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalSlsBuyerOrder.Check(orderId, _userId, comment);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confirm(long orderId, string comment)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalSlsBuyerOrder.Confirm(orderId, _userId, comment);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(long orderId, string del)
        {
            if (del == "all")
            {
                _validationMsg = _dalSlsBuyerOrder.Delete(orderId);
            }
            if (del == "item")
            {
                _validationMsg = _dalSlsBuyerOrder.DeleteItem(orderId);
            }
            if (del == "color")
            {
                _validationMsg = _dalSlsBuyerOrder.DeleteItemColor(orderId);
            }
            if (del == "dlvry")
            {
                _validationMsg = _dalSlsBuyerOrder.DeleteDelivery(orderId);
            }
            
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemsByOrderId(long orderId)
        {
            var items = _unit.SlsBuyerOrderItemRepository.Get().Where(ob => ob.BuyerOrderID == orderId).ToList();
            var result = items.Select(item => new
            {
                item.BuyerOrderItemID,
                item.BuyerOrderID,
                item.commodity,
                item.HSCode,
                item.ArticleID,
                ArticleName =
                    item.ArticleID == null ? "" : _unit.ArticleRepository.GetByID(item.ArticleID).ArticleName,
                item.ArticleNo,
                item.AvgSize,
                item.AvgSizeUnit,
                AvgSizeUnitName =
                    item.AvgSizeUnit == null ? "" : _unit.SysUnitRepository.GetByID(item.AvgSizeUnit).UnitName,
                item.SideDescription,
                item.SelectionRange,
                item.Thickness,
                item.ThicknessUnit,
                ThicknessUnitName =
                    item.ThicknessUnit == null
                        ? ""
                        : _unit.SysUnitRepository.GetByID(item.ThicknessUnit).UnitName,
                item.ThicknessAt,
                item.ItemTypeID,
                ItemTypeName = item.ItemTypeID == null ? "" : _unit.SysItemTypeRepository.GetByID(item.ItemTypeID).ItemTypeName,
                item.LeatherTypeID,
                LeatherTypeName = item.LeatherTypeID == null ? "" : _unit.SysLeatherTypeRepository.GetByID(item.LeatherTypeID).LeatherTypeName,
                item.LeatherStatusID,
                LeatherStatusName = item.LeatherStatusID == null ? "" : _unit.SysLeatherStatusRepo.GetByID(item.LeatherStatusID).LeatherStatusName,
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
                            ColorQty = color.ColorQty,
                            ColorUnit = color.ColorUnit,
                            ColorUnitName =
                                color.ColorUnit == null
                                    ? ""
                                    : _unit.SysUnitRepository.GetByID(color.ColorUnit).UnitName,
                            
                        }).ToList()
            }).ToList();
            return Json(result.OrderBy(ob => ob.BuyerOrderID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemTypes()
        {
            var itemTypes =
                _unit.SysItemTypeRepository.Get()
                    .Where(ob => ob.ItemTypeCategory == "Leather" && ob.IsDelete == false)
                    .ToList();
            var result = itemTypes.Select(itemType => new
            {
                itemType.ItemTypeID,
                itemType.ItemTypeName
            }).ToList();
            return Json(result.OrderBy(ob => ob.ItemTypeName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeatherType()
        {
            var lthrTyp = _unit.SysLeatherTypeRepository.Get().Where(ob => ob.IsActive && !ob.IsDelete).ToList();
            var result = lthrTyp.Select(typ => new
            {
                typ.LeatherTypeID,
                typ.LeatherTypeName
            }).ToList();

            return Json(result.OrderBy(ob => ob.LeatherTypeName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeatherStatus()
        {
            var lthrStat = _unit.SysLeatherStatusRepo.Get().Where(ob => ob.IsActive && !ob.IsDelete).ToList();
            var result = lthrStat.Select(stat => new
            {
                stat.LeatherStatusID,
                stat.LeatherStatusName
            }).ToList();
            return Json(result.OrderBy(ob => ob.LeatherStatusName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBuyerListForSearch()
        {
            var buyerList = _dalSlsBuyerOrder.GetBuyerListForSearch();
            return Json(buyerList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBuyerList(string buyer)
        {
            var sysBuyer = new SysBuyer();
            var buyerList = _dalSlsBuyerOrder.GetBuyerList(buyer);
            sysBuyer.Count = buyerList.Count > 1 ? 0 : 1;
            sysBuyer.BuyerList = buyerList;
            return Json(sysBuyer, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocalAgentListForSearch(string lAgent)
        {
            var buyerList = _dalSlsBuyerOrder.GetLocalAgentListForSearch(lAgent);
            return Json(buyerList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalAgentList(string lAgent)
        {
            //var sysBuyer = new SysBuyer();
            //var lAgentList = _dalSlsBuyerOrder.GetLocalAgentList(lAgent);
            ////sysBuyer.Count = lAgentList.Count > 1 ? 0 : 1;
            //sysBuyer.AgentList = lAgentList;

            var searchList =
               _context.Sys_Buyer.Where(
                   m => m.IsActive != false && m.BuyerCategory == "Buyer Agent" && m.BuyerType == "Local Agent" &&
                       m.BuyerName.StartsWith(lAgent)).ToList();
            var result = searchList.Select(buyer => new
            {
                buyer.BuyerID,
                buyer.BuyerCode,
                buyer.BuyerName,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID).BuyerAddressID,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID).Address
            });

            return Json(result.OrderBy(ob => ob.BuyerName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetForeignAgentListForSearch(string lAgent)
        {
            var buyerList = _dalSlsBuyerOrder.GetForeignAgentListForSearch(lAgent);
            return Json(buyerList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetForeignAgentList(string lAgent)
        {
            //var sysBuyer = new SysBuyer();
            //var lAgentList = _dalSlsBuyerOrder.GetLocalAgentList(lAgent);
            ////sysBuyer.Count = lAgentList.Count > 1 ? 0 : 1;
            //sysBuyer.AgentList = lAgentList;

            var searchList =
               _context.Sys_Buyer.Where(
                   m => m.IsActive != false && m.BuyerCategory == "Buyer Agent" && m.BuyerType == "Foreign Agent" &&
                       m.BuyerName.StartsWith(lAgent)).ToList();
            var result = searchList.Select(buyer => new
            {
                buyer.BuyerID,
                buyer.BuyerCode,
                buyer.BuyerName,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID).BuyerAddressID,
                _unit.BuyerAddressRepository.Get().FirstOrDefault(ob => ob.BuyerID == buyer.BuyerID).Address
            });

            return Json(result.OrderBy(ob => ob.BuyerName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetArticleForSearch(string article)
        {
            var articleList = _dalSlsBuyerOrder.GetArticleListForSearch(article);
            return Json(articleList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetArticleList(string article)
        {
            var sysArticle = new SysArticle();
            var articleList = _dalSlsBuyerOrder.GetArticleList(article);
            //sysBuyer.Count = lAgentList.Count > 1 ? 0 : 1;
            sysArticle.ArticleList = articleList;
            return Json(sysArticle, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetColorForSearch(string color)
        {
            var colorList = _dalSlsBuyerOrder.GetColorListForSearch(color);
            return Json(colorList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorList(string color)
        {
            var sysColor = new SysColor();
            var colorList = _dalSlsBuyerOrder.GetColorList(color);
            //sysBuyer.Count = lAgentList.Count > 1 ? 0 : 1;
            sysColor.ColorList = colorList;
            return Json(sysColor, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllBuyerPurchaseOrder()
        {
            var data = _dalSlsBuyerOrder.GetAllBuyerPurchaseOrder();
            return Json(data, JsonRequestBehavior.AllowGet);
        
        }

        public JsonResult CheckUniqueOrderNo(string orderNo)
        {
            bool isTrue = _dalSlsBuyerOrder.CheckUniqueOrderNo(orderNo);
            return new JsonResult { Data = isTrue };
        }

        public ActionResult GetOrderListForSearch()
        {
            var order =
                _unit.SlsBuyerOrederRepository.Get()
                    .Select(ob => ob.OrderNo)
                    .ToList();
            return Json(order, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderForSearch(string orderNo)
        {
            var bills = _context.SLS_BuyerOrder.Where(m => m.OrderNo.StartsWith(orderNo)).ToList();
            var result = bills.Select(order => new
            {
                order.BuyerOrderID,
                order.BuyerOrderNo,
                order.OrderNo,
                BuyerOrderDate = string.Format("{0:dd/MM/yyyy}", order.BuyerOrderDate),
                order.BuyerID,
                BuyerName = order.BuyerID == null ? "" : _unit.SysBuyerRepository.GetByID(order.BuyerID).BuyerName,
                RecordStatus = DalCommon.ReturnRecordStatus(order.RecordStatus)
            }).ToList();

            return Json(result.OrderByDescending(ob => ob.BuyerOrderID), JsonRequestBehavior.AllowGet);
        }
    }
}


