using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class PurchasePeriodController : Controller
    {
        private DalPrqPurchaseYearPeriod Dalobject;
        private ValidationMsg _vmMsg;

        public PurchasePeriodController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalPrqPurchaseYearPeriod();
        }

        [CheckUserAccess("PurchasePeriod/PurchasePeriod")]
        public ActionResult PurchasePeriod()
        {
            ViewBag.formTiltle = "Raw Hide Leather Purchase Target Item";
            ViewBag.PurchaseYearList = Dalobject.GetPurchaseYear();
            return View();
        }

        [HttpPost]
        public ActionResult PurchasePeriod(PrqPurchaseYearPeriod model)
        {
            _vmMsg = Dalobject.Save(model);
            return new JsonResult { Data = _vmMsg };
        }

        public ActionResult GetAllActiveUnit()
        {
            DalSysUnit objSource = new DalSysUnit();

            var allData = objSource.GetAllActiveUnit();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllLeatherStatus()
        {
            DalSysLeatherStatus objSource = new DalSysLeatherStatus();

            var allData = objSource.GetAllLeatherStatus();
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPeriodList(string yearid)
        {
            int year = string.IsNullOrEmpty(yearid) ? 0 : Convert.ToInt32(yearid);
            var allData = Dalobject.GetPurchaseYearPeriod(year);
            return Json(allData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPeriodItemList(string periodid)
        {
            int period = string.IsNullOrEmpty(periodid) ? 0 : Convert.ToInt32(periodid);
            var allData = Dalobject.GetPeriodItemList(period);
            return Json(allData, JsonRequestBehavior.AllowGet);
        }
    }
}