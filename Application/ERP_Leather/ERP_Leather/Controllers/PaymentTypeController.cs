using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class PaymentTypeController : Controller
    {
        private DalSysPaymentType _dalSysPaymentType;
        private SysPaymentType _sysPaymentType = new SysPaymentType();

        private ValidationMsg _vmMsg;

        public PaymentTypeController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysPaymentType = new DalSysPaymentType();
        }

        [CheckUserAccess("PaymentType/PaymentType")]
        public ActionResult PaymentType()
        {
            ViewBag.formTiltle = "PaymentType Form";
            return View(_sysPaymentType);
        }

        [HttpPost]
        public ActionResult PaymentType(SysPaymentType model)
        {
            if (model != null && model.ID != 0)
            {
                _vmMsg = _dalSysPaymentType.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysPaymentType.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { SourceID = _dalSysPaymentType.GetPaymentTypeID(), msg = _vmMsg });
        }

        public JsonResult GetPaymentType()
        {
            var sysPaymentType = _dalSysPaymentType.GetAll().OrderByDescending(s => s.ID);
            return Json(sysPaymentType, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeletePaymentType(string models)
        {
            _vmMsg = _dalSysPaymentType.Delete(models, Convert.ToInt32(Session["UserID"]));
            return Json( _vmMsg, JsonRequestBehavior.AllowGet);
        }
	}
}