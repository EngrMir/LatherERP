using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ERP.EntitiesModel.AppSetupModel;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.BaseModel;
using ERP_Leather.ActionFilters;
using System;

namespace ERP_Leather.Controllers
{
    public class CurrencyController : Controller
    {

        private DalSysCurrency _dalSysCurrency;
        private SysCurrency _sysCurrency = new SysCurrency();
        private ValidationMsg _vmMsg;

        

        public CurrencyController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysCurrency = new DalSysCurrency();
        }
        [CheckUserAccess("Currency/Currency")]
        public ActionResult Currency()
        {
            ViewBag.formTiltle = "Currency Form";
            return View(_sysCurrency);
        }

        [HttpPost]
        public ActionResult Currency(SysCurrency model)
        {
            if (model != null && model.CurrencyID != 0)
            {
                _vmMsg = _dalSysCurrency.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysCurrency.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { CurrencyID = _dalSysCurrency.GetCurrencyID(), msg = _vmMsg });
        }


        [HttpPost]
        public ActionResult Delete(string currencyId)
        {
            _vmMsg = _dalSysCurrency.Delete(currencyId, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCurrencyList()
        {
            var sysSource = _dalSysCurrency.GetAll().OrderByDescending(s => s.CurrencyID);
            return Json(sysSource, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveCurrency()
        {
            var sysSource = _dalSysCurrency.GetAllActiveCurrency().OrderByDescending(s => s.CurrencyID);
            return Json(sysSource, JsonRequestBehavior.AllowGet);
        }
    }
}