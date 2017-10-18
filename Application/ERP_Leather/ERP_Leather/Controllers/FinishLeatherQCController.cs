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
    public class FinishLeatherQCController : Controller
    {
        DalSysStore objDalSysStore = new DalSysStore();
        private DalFinishLeatherQC Dalobject;
        private ValidationMsg _vmMsg;

        public FinishLeatherQCController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalFinishLeatherQC();
        }

        [CheckUserAccess("FinishLeatherQC/FinishLeatherQC")]
        public ActionResult FinishLeatherQC()
        {
            ViewBag.formTiltle = "Finish Leather Own QC";
            ViewBag.ddlFinishProductionFloorList = objDalSysStore.GetAllActiveFinishProductionStore();
            ViewBag.ddlBuyerQCStoreList = objDalSysStore.GetAllActiveFinishBuyerQCStore();
            ViewBag.ddlFinishStoreList = objDalSysStore.GetAllActiveStoreTransferReceiveStore();

            return View();
        }
        [HttpPost]
        public ActionResult FinishLeatherQC(PrdCrustLeatherQC model)
        {
            _vmMsg = model.CrustLeatherQCID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "FinishLeatherQC/FinishLeatherQC") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            //_vmMsg = model.CrustLeatherQCID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "ChemicalConsumption/ChemConsumption") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { CrustLeatherQCID = Dalobject.GetQCID(), CrustLeatherQCNo = Dalobject.GetQCNo(), msg = _vmMsg });
        }

        public ActionResult GetFinishIssueToList(string QCTransactionOf)
        {
            switch (QCTransactionOf)
            {
                case "FOQC":
                    {
                        var ddlFinishOwnQCStoreList = objDalSysStore.GetAllActiveFinishOwnQCStore();
                        return Json(ddlFinishOwnQCStoreList, JsonRequestBehavior.AllowGet);
                    }
                case "FBQC":
                    {
                        var ddlFinishBuyerQCStoreList = objDalSysStore.GetAllActiveFinishBuyerQCStore();
                        return Json(ddlFinishBuyerQCStoreList, JsonRequestBehavior.AllowGet);
                    }
                default:
                    return null;
            }
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
        public ActionResult GetAllGridList(string CrustLeatherQCID, string StoreId)
        {
            PrdCrustLeatherQC model = new PrdCrustLeatherQC();
            if (string.IsNullOrEmpty(CrustLeatherQCID)) return Json(model, JsonRequestBehavior.AllowGet);
            model.PrdCrustLeatherQCItemList = Dalobject.GetQCColorList(CrustLeatherQCID);
            if (model.PrdCrustLeatherQCItemList.Count > 0)
                model.PrdCrustLeatherQCSelectionList = Dalobject.GetQCSelectionList(model.PrdCrustLeatherQCItemList[0].CLQCItemID.ToString(), StoreId);
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

        public ActionResult GetGradeInfoInFromQCStock(string ProductionFloor, string BuyerID, string BuyerOrderID, string ArticleID, string ItemTypeID, string LeatherStatusID, string ColorID, string ArticleChallanID)
        {
            var gradeList = Dalobject.GetGradeInfoInFromQCStock(ProductionFloor, BuyerID, BuyerOrderID, ArticleID, ItemTypeID, LeatherStatusID,ColorID, ArticleChallanID);
            return Json(gradeList, JsonRequestBehavior.AllowGet);
        }
    }
}