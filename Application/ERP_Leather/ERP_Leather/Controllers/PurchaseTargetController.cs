using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.BusinessLogicLayer.OperationManager;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.BaseModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class PurchaseTargetController : Controller
    {
        private DalPrqPurchaseYearTarget Dalobject;
        private ValidationMsg _vmMsg;

        public PurchaseTargetController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalPrqPurchaseYearTarget();
        }

        [CheckUserAccess("PurchaseTarget/PrqPurchaseYearPeriodItemv")]
        public ActionResult PrqPurchaseYearPeriodItemv()
        {
            //ViewBag.formTiltle = "Raw Hide Leather Purchase Year & Period Define";
            ViewBag.formTiltle = "Raw Hide Leather Purchase Target";
            return View();
        }

        [HttpPost]
        public ActionResult PrqPurchaseYearPeriodItemv(PrqPurchaseYearTarget model)
        {
            _vmMsg = model.YearID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"])) : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { YearID = Dalobject.GetYearID(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult YearlyTargetConfirmed(PrqPurchaseYearTarget model)
        {
            _vmMsg = Dalobject.YearlyTargetConfirmed(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult YearlyTargetRevisioned(PrqPurchaseYearTarget model)
        {
            _vmMsg = Dalobject.YearlyTargetRevisioned(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        public ActionResult GetYearList()
        {
            var yearData = Dalobject.GetYearList();
            return Json(yearData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPeriodList(string yearId)
        {
            PrqPurchaseYearTarget model = new PrqPurchaseYearTarget();
            int yearid = string.IsNullOrEmpty(yearId) ? 0 : Convert.ToInt32(yearId);
            var yearData = Dalobject.GetYearData(yearid);
            if (yearData != null)
            {
                model.PurchaseYearPeriodList = Dalobject.GetPeriodList(yearid);
                model.YearEndDate = yearData.FirstOrDefault().YearEndDate;
                model.YearStartDate = yearData.FirstOrDefault().YearStartDate;
                //model.YearStartDate = model.PurchaseYearPeriodList.Count > 0 ? model.PurchaseYearPeriodList[0].EndDate : yearData.FirstOrDefault().YearStartDate;
                model.PeriodID = model.PurchaseYearPeriodList.Count == 1 ? model.PurchaseYearPeriodList[0].PeriodID : 0;
            }


            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPeriodItemList(string periodId)
        {
            var periodid = string.IsNullOrEmpty(periodId) ? 0 : Convert.ToInt32(periodId);
            var periodItemList = Dalobject.GetPeriodItemList(periodid);
            return Json(periodItemList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedTargetYear(string yearId)
        {
            int yearid = Convert.ToInt16(yearId);
            _vmMsg = Dalobject.DeletedTargetYear(yearid);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedPeriod(string periodId, string RecordStatus)
        {
            int periodid = Convert.ToInt16(periodId);
            _vmMsg = Dalobject.DeletedPeriod(periodid, RecordStatus);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedPeriodItem(string periodItemId, string RecordStatus)
        {
            int perItemId = Convert.ToInt16(periodItemId);
            _vmMsg = Dalobject.DeletedPeriodItem(perItemId, RecordStatus);
            return Json(new { msg = _vmMsg });
        }
    }
}