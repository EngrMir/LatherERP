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
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using System.Configuration;

namespace ERP_Leather.Controllers
{
    public class CrustingProductionScheduleController : Controller
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
        private DalCrustingProductionSchedule Dalobject;
        private ValidationMsg _vmMsg;

        public CrustingProductionScheduleController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalCrustingProductionSchedule();
        }
        [CheckUserAccess("CrustingProductionSchedule/CrustingProductionSchedule")]
        public ActionResult CrustingProductionSchedule()
        {
            ViewBag.formTiltle = "Crust Production Schedule";
            ViewBag.ddlCrustProductionFloorList = objDalSysStore.GetAllActiveWetBlueProductionStore();
            ViewBag.ddlProcessList = daiSysProductionProces.GetAllActiveCrustProcessList();
            ViewBag.ddlItemTypeList = daliItemType.GetAllActiveItemTypeLeather();
            ViewBag.ddlLeatherStatusList = dalSysLeatherStatus.GetAllLeatherStatus();
            ViewBag.ddlThicknessUnitList = dalSysUnit.GetAllActiveThicknessUnit();
            ViewBag.ddlAvgSizeUnitList = dalSysUnit.GetAllActiveLeatherUnit();
            return View();
        }
        [HttpPost]
        public ActionResult CrustingProductionSchedule(PrdCrustYearMonth model)
        {
            _vmMsg = model.YearMonID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "CrustingProductionSchedule/CrustingProductionSchedule") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { YearMonID = Dalobject.GetYearMonID(), ScheduleID = Dalobject.GetYearMonScheduleID(), ScheduleNo = Dalobject.GetYearMonScheduleNo(), ScheduleDateID = Dalobject.GetYearMonScheduleDateID(), ProductionNo = Dalobject.GetYearMonScheduleDateNo(), ScheduleItemID = Dalobject.GetYearMonScheduleItemID(), ScheduleProductionNo = Dalobject.GetYearMonScheduleItemNo(), msg = _vmMsg });
        }
        public ActionResult GetCrustYearMonthSchedule()
        {
            var crustYearMonth = Dalobject.GetCrustYearMonthSchedule();
            return Json(crustYearMonth, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCrustYearMonth()
        {
            var crustYearMonth = Dalobject.GetCrustYearMonth();
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
        public ActionResult GetGradeInfoInProductionFloor(string ProductionFloor, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ArticleChallanID)
        {
            var machineList = Dalobject.GetGradeInfoInProductionFloor(ProductionFloor, BuyerID, BuyerOrderID, ArticleID, ItemTypeID, LeatherStatusID, ArticleChallanID);
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
        public ActionResult GetAllActiveBuyerByCategory()
        {
            var packItemList = dalSysBuyer.GetAllActiveBuyerByCategory();
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
            return Json(new { RecordStatus = "CNF", msg = _vmMsg });
            //return Json(new { msg = _vmMsg });
        }
        [HttpPost]
        public ActionResult CheckedCrustProductionSchedule(PrdCrustYearMonth model)
        {
            _vmMsg = Dalobject.CheckedCrustProductionSchedule(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedCrustYearMonthSchedule(string YearMonID, string ScheduleID, string ScheduleDateID, string ScheduleItemID, string RecordStatus)
        {
            _vmMsg = Dalobject.DeletedCrustYearMonthSchedule(YearMonID, ScheduleID, ScheduleDateID, ScheduleItemID, RecordStatus);
            return Json(new { msg = _vmMsg });
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedCrustScheduleColor(string SdulItemColorID, string RecordStatus)
        {
            long sdulItemColorID = Convert.ToInt64(SdulItemColorID);
            _vmMsg = Dalobject.DeletedCrustScheduleColor(sdulItemColorID, RecordStatus);
            return Json(new { msg = _vmMsg });
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedScheduleDrum(string CrustSdulDrumID, string RecordStatus)
        {
            long crustSdulDrumID = Convert.ToInt64(CrustSdulDrumID);
            _vmMsg = Dalobject.DeletedScheduleDrum(crustSdulDrumID, RecordStatus);
            return Json(new { msg = _vmMsg });
        }
        public ActionResult GetCrustProductionScheduleItemForSearch()
        {
            var crustYearMonthScheduleList = Dalobject.GetCrustProductionScheduleItemForSearch();
            return Json(crustYearMonthScheduleList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorListAccordingToArticleChallan(int? _BuyerOrderItemID, long _ArticleChallanID)
        {
            var Data = Dalobject.GetColorListAccordingToArticleChallan(_BuyerOrderItemID, _ArticleChallanID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        
    }
}