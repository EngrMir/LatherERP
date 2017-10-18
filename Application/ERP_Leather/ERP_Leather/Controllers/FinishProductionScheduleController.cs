using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.BaseModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class FinishProductionScheduleController : Controller
    {
        DalSysStore objDalSysStore = new DalSysStore();
        DalSysProductionProces daiSysProductionProces = new DalSysProductionProces();
        DalSysItemType daliItemType = new DalSysItemType();
        DalSysLeatherStatus dalSysLeatherStatus = new DalSysLeatherStatus();
        DalSysUnit dalSysUnit = new DalSysUnit();
        DalSysMachine dalSysMachine = new DalSysMachine();
        DalSysGrade dalSysGrade = new DalSysGrade();
        DalSysColor dalSysColor = new DalSysColor();
        DalSysBuyer dalSysBuyer = new DalSysBuyer();
        DalSysArticle dalSysArticle = new DalSysArticle();
        private DalFinishProductionSchedule Dalobject;
        private ValidationMsg _vmMsg;

        public FinishProductionScheduleController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalFinishProductionSchedule();
        }
        [CheckUserAccess("FinishProductionSchedule/FinishProductionSchedule")]
        public ActionResult FinishProductionSchedule()
        {
            ViewBag.formTiltle = "Finish Production Schedule";
            ViewBag.ddlFinishProductionFloorList = objDalSysStore.GetAllActiveFinishProductionStore();
            ViewBag.ddlProcessList = daiSysProductionProces.GetAllActiveFinishProcessList();
            ViewBag.ddlItemTypeList = daliItemType.GetAllActiveItemTypeLeather();
            ViewBag.ddlLeatherStatusList = dalSysLeatherStatus.GetAllLeatherStatus();
            ViewBag.ddlThicknessUnitList = dalSysUnit.GetAllActiveThicknessUnit();
            ViewBag.ddlAvgSizeUnitList = dalSysUnit.GetAllActiveLeatherUnit();
            return View();
        }
        [HttpPost]
        public ActionResult FinishProductionSchedule(PrdCrustYearMonth model)
        {
            _vmMsg = model.YearMonID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "FinishProductionSchedule/FinishProductionSchedule") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            //_vmMsg = model.YearMonID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "ChemicalConsumption/ChemConsumption") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { YearMonID = Dalobject.GetYearMonID(), ScheduleID = Dalobject.GetYearMonScheduleID(), ScheduleNo = Dalobject.GetYearMonScheduleNo(), ScheduleDateID = Dalobject.GetYearMonScheduleDateID(), ProductionNo = Dalobject.GetYearMonScheduleDateNo(), ScheduleItemID = Dalobject.GetYearMonScheduleItemID(), ScheduleProductionNo = Dalobject.GetYearMonScheduleItemNo(), msg = _vmMsg });
        }
        public ActionResult GetFinishYearMonth()
        {
            var crustYearMonth = Dalobject.GetFinishYearMonth();
            return Json(crustYearMonth, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetYearMonthScheduleList(string YearMonID)
        {
            var crustYearMonthScheduleList = Dalobject.GetYearMonthScheduleList(YearMonID);
            return Json(crustYearMonthScheduleList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetYearMonthScheduleDateInfo(string ScheduleID)
        {
            var crustYearMonthScheduleList = Dalobject.GetYearMonthScheduleDateInfo(ScheduleID);
            return Json(crustYearMonthScheduleList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetScheduleItemInfo(string ScheduleDateID)
        {
            var crustYearMonthScheduleList = Dalobject.GetScheduleItemInfo(ScheduleDateID);
            return Json(crustYearMonthScheduleList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllGridList(string ScheduleItemID)
        {
            PrdCrustYearMonth model = new PrdCrustYearMonth();
            if (string.IsNullOrEmpty(ScheduleItemID)) return Json(model, JsonRequestBehavior.AllowGet);
            model.PrdYearMonthCrustScheduleColorList = Dalobject.YearMonthCrustScheduleColorList(ScheduleItemID);
            //if (model.PrdYearMonthCrustScheduleColorList.Count > 0)
            //    model.PrdYearMonthCrustScheduleDrumList = Dalobject.YearMonthCrustScheduleDrumList(model.PrdYearMonthCrustScheduleColorList[0].SdulItemColorID.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult YearMonthCrustScheduleDrumList(string SdulItemColorID)
        //{
        //    var machineList = Dalobject.YearMonthCrustScheduleDrumList(SdulItemColorID);
        //    return Json(machineList, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult YearMonthCrustScheduleDrumList(string SdulItemColorID, string ProductionFloor, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID)
        {
            var machineList = Dalobject.YearMonthCrustScheduleDrumList(SdulItemColorID, ProductionFloor, BuyerID, BuyerOrderID, ArticleID, ItemTypeID, LeatherStatusID);
            return Json(machineList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMachineNoInfo()
        {
            var machineList = dalSysMachine.GetAllCrustLeatherProductionMachine();
            return Json(machineList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGradeInfo()
        {
            var machineList = dalSysGrade.GetAllActiveGrade();
            return Json(machineList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetColorInfo()
        {
            var machineList = dalSysColor.GetAllActiveColor();
            return Json(machineList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllActiveUnit()
        {
            var packItemList = dalSysUnit.GetAllActiveLeatherUnit();
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllActiveBuyer()
        {
            var packItemList = dalSysBuyer.GetAllActiveBuyer().Where(m => m.BuyerCategory == "Buyer").ToList();
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetColorListForOrderItem(string _BuyerOrderItemID)
        {
            return Json(Dalobject.GetColorListForOrderItem(_BuyerOrderItemID), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetArticleInfo()
        {
            var packItemList = dalSysArticle.GetAllActiveArticle();
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExecuteCrustProductionSchedule(PrdCrustYearMonth model)
        {
            _vmMsg = Dalobject.ExecuteCrustProductionSchedule(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }
        [HttpPost]
        public ActionResult ConfirmedCrustProductionSchedule(PrdCrustYearMonth model)
        {
            _vmMsg = Dalobject.ConfirmedCrustProductionSchedule(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }
        [HttpPost]
        public ActionResult CheckedCrustProductionSchedule(PrdCrustYearMonth model)
        {
            _vmMsg = Dalobject.CheckedCrustProductionSchedule(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetFinishYearMonthSchedule()
        {
            var crustYearMonth = Dalobject.GetFinishYearMonthSchedule();
            return Json(crustYearMonth, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeletedCrustYearMonthSchedule(string ScheduleNo)
        {
            _vmMsg = Dalobject.DeletedCrustYearMonthSchedule(ScheduleNo);
            return Json(new { msg = _vmMsg });
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedCrustScheduleColor(string ScheduleDateID, string RecordStatus)
        {
            long scheduleDateID = Convert.ToInt64(ScheduleDateID);
            _vmMsg = Dalobject.DeletedCrustScheduleColor(scheduleDateID, RecordStatus);
            return Json(new { msg = _vmMsg });
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedScheduleDrum(string SchedulePurchaseID, string RecordStatus)
        {
            long schedulePurchaseID = Convert.ToInt64(SchedulePurchaseID);
            _vmMsg = Dalobject.DeletedScheduleDrum(schedulePurchaseID, RecordStatus);
            return Json(new { msg = _vmMsg });
        }

        public ActionResult GetGradeInfoInProductionFloor(string ProductionFloor, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ColorID, string ArticleChallanID)
        {
            var machineList = Dalobject.GetGradeInfoInProductionFloor(ProductionFloor, BuyerID, BuyerOrderID, ArticleID, ItemTypeID, LeatherStatusID,ColorID, ArticleChallanID);
            return Json(machineList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorInfoInProductionFloorStock(string ProductionFloor, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ColorID, string ArticleChallanID)
        {
            var colorInfoList = Dalobject.GetColorInfoInProductionFloorStock(ProductionFloor, BuyerID, BuyerOrderID, ArticleID, ItemTypeID, LeatherStatusID, ColorID, ArticleChallanID);
            return Json(colorInfoList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChallanInfo(int? buyerID, int? articleID)
        {
            var challanInfoList = Dalobject.GetChallanInfo(buyerID, articleID);
            return Json(challanInfoList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFinishProductionFloorStock(string ProductionFloor)
        {
            var productionFloorStockList = Dalobject.GetFinishProductionFloorStock(ProductionFloor);
            return Json(productionFloorStockList, JsonRequestBehavior.AllowGet);
        }
    }
}