using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using System;
using System.Linq;
using System.Web.Mvc;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class ProductionProcesController : Controller
    {
        private DalSysProductionProces _dalSysProductionProces;
        private ValidationMsg _vmMsg;

        public ProductionProcesController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysProductionProces = new DalSysProductionProces();
        }

        [CheckUserAccess("ProductionProces/ProductionProces")]
        public ActionResult ProductionProces()
        {
            ViewBag.formTiltle = "Production Proces";
            return View();
        }

        [HttpPost]
        public ActionResult ProductionProces(SysProductionProces model)
        {
            if (model != null && model.ProcessID != 0)
            {
                _vmMsg = _dalSysProductionProces.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysProductionProces.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { ProcessID = _dalSysProductionProces.GetProcessID(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult Delete(string ProcessID)
        {
            _vmMsg = _dalSysProductionProces.Delete(ProcessID, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetProductionProcesList()
        {
            var sysProductionProces = _dalSysProductionProces.GetAll().OrderByDescending(s => s.ProcessID);
            return Json(sysProductionProces, JsonRequestBehavior.AllowGet);
        }
	}
}