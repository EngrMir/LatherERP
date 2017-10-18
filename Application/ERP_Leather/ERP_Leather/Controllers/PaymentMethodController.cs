using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class PaymentMethodController : Controller
    {
        private DalSysPaymentMethod _dalPaymentMethod;
        private SysPaymentMethod _sysCurrencyPaymentMethod = new SysPaymentMethod();
        private ValidationMsg _vmMsg;

        public PaymentMethodController()
        {
            _vmMsg = new ValidationMsg();
            _dalPaymentMethod = new DalSysPaymentMethod();
        }

        [CheckUserAccess("PaymentMethod/PaymentMethod")]
        public ActionResult PaymentMethod()
        {
            ViewBag.formTiltle = "Payment Method Form";
            return View(_sysCurrencyPaymentMethod);
        }

        [HttpPost]
        public ActionResult PaymentMethod(SysPaymentMethod model)
        {
            if (model != null && model.ID != 0)
            {
                _vmMsg = _dalPaymentMethod.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalPaymentMethod.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { SourceID = _dalPaymentMethod.GetPaymentMethodID(), msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Get()
        {
            var sysUnit = _dalPaymentMethod.GetAll().OrderByDescending(s => s.ID);
            return Json(sysUnit, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            _vmMsg = _dalPaymentMethod.Delete(id, Convert.ToInt32(Session["UserID"]));
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }
    }
}