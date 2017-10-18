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
    public class CrustLeatherQCController : Controller
    {
        DalSysStore objDalSysStore = new DalSysStore();
        private DalCrustLeatherQC Dalobject;
        private ValidationMsg _vmMsg;

        public CrustLeatherQCController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalCrustLeatherQC();
        }

        [CheckUserAccess("CrustLeatherQC/CrustLeatherQC")]
        public ActionResult CrustLeatherQC()
        {
            ViewBag.formTiltle = "Crust Leather QC";
            ViewBag.ddlCrustProductionFloorList = objDalSysStore.GetAllActiveWetBlueProductionStore();
            ViewBag.ddlCrustQCStoreList = objDalSysStore.GetAllActiveCrustQCStore();
            return View();
        }
        [HttpPost]
        public ActionResult CrustLeatherQC(PrdCrustLeatherQC model)
        {
            _vmMsg = model.CrustLeatherQCID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "CrustLeatherQC/CrustLeatherQC") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { CrustLeatherQCID = Dalobject.GetQCID(), CrustLeatherQCNo = Dalobject.GetQCNo(), msg = _vmMsg });
        }

        public ActionResult GetQCScheduleItemInfo(string ProductionFloor)
        {
            var crustYearMonthScheduleList = Dalobject.GetScheduleItemInfo(ProductionFloor);
            return Json(crustYearMonthScheduleList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCrustLeatherQCInfo()
        {
            var crustLeatherQCInfoList = Dalobject.GetCrustLeatherQCInfo();
            return Json(crustLeatherQCInfoList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorAndGradeList(string ProductionFloor, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ColorID, string ArticleChallanID)
        {
            PrdCrustLeatherQC model = new PrdCrustLeatherQC();
            //if (string.IsNullOrEmpty(ScheduleItemID)) return Json(model, JsonRequestBehavior.AllowGet);
            model.PrdCrustLeatherQCItemList = Dalobject.GetScheduleColorList(ProductionFloor, BuyerID, BuyerOrderID, ArticleID, ItemTypeID, LeatherStatusID, ColorID, ArticleChallanID);
            if (model.PrdCrustLeatherQCItemList.Count > 0)
                model.PrdCrustLeatherQCSelectionList = Dalobject.GetScheduleGradeList(ProductionFloor, BuyerID, BuyerOrderID, ArticleID, ItemTypeID, LeatherStatusID, ColorID, ArticleChallanID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllGridList(string CrustLeatherQCID)
        {
            PrdCrustLeatherQC model = new PrdCrustLeatherQC();
            if (string.IsNullOrEmpty(CrustLeatherQCID)) return Json(model, JsonRequestBehavior.AllowGet);
            model.PrdCrustLeatherQCItemList = Dalobject.GetQCColorList(CrustLeatherQCID);
            if (model.PrdCrustLeatherQCItemList.Count > 0)
                model.PrdCrustLeatherQCSelectionList = Dalobject.GetQCSelectionList(model.PrdCrustLeatherQCItemList[0].CLQCItemID.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConfirmedCrustQC(PrdCrustLeatherQC model)
        {
            _vmMsg = Dalobject.ConfirmedCrustQC(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult CheckedCrustProductionSchedule(PrdCrustLeatherQC model)
        {
            _vmMsg = Dalobject.CheckedCrustProductionSchedule(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedCrustLeatherQC(string CrustLeatherQCID)
        {
            long crustLeatherQCID = Convert.ToInt64(CrustLeatherQCID);
            _vmMsg = Dalobject.DeletedCrustLeatherQC(crustLeatherQCID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult DeletedCrustQCItem(string ScheduleDateID, string RecordStatus)
        public ActionResult DeletedCrustQCItem(string CLQCItemID)
        {
            long itemID = Convert.ToInt64(CLQCItemID);
            //_vmMsg = Dalobject.DeletedCrustQCItem(scheduleDateID, RecordStatus);
            _vmMsg = Dalobject.DeletedCrustQCItem(itemID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult DeletedQCSelection(string SchedulePurchaseID, string RecordStatus)
        public ActionResult DeletedQCSelection(string CLQCSelectionID)
        {
            long QCSelectionID = Convert.ToInt64(CLQCSelectionID);
            //_vmMsg = Dalobject.DeletedScheduleDrum(schedulePurchaseID, RecordStatus);
            _vmMsg = Dalobject.DeletedQCSelection(QCSelectionID);
            return Json(new { msg = _vmMsg });
        }
    }
}