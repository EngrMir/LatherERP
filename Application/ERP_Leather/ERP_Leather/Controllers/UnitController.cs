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
    public class UnitController : Controller
    {
        private DalSysUnit _dalSysUnit;
        private SysUnit _sysUnit = new SysUnit();

        private ValidationMsg _vmMsg;

        public UnitController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysUnit = new DalSysUnit();
        }

        [CheckUserAccess("Unit/Unit")]
        public ActionResult Unit()
        {
            ViewBag.formTiltle = "Unit Form";
            return View(_sysUnit);
        }

        [HttpPost]
        public ActionResult Unit(SysUnit model)
        {
            if (model != null && model.UnitID != 0)
            {
                _vmMsg = _dalSysUnit.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysUnit.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { SourceID = _dalSysUnit.GetUnitID(), msg = _vmMsg });
        }

        //public JsonResult GetUnit()
        //{
        //    return new JsonResult { Data = _dalSysUnit.GetAll().OrderByDescending(s => s.UnitID) };
        //}

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetUnit()
        {
            var sysUnit = _dalSysUnit.GetAll().OrderByDescending(s => s.UnitID);
            return Json(sysUnit, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAllLeatherUnit()
        {
            var sysUnit = _dalSysUnit.GetAllActiveLeatherUnit().OrderByDescending(s => s.UnitID);
            return Json(sysUnit, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteUnit(string unitId)
        {
            _vmMsg = _dalSysUnit.Delete(unitId, Convert.ToInt32(Session["UserID"]));
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
            
        }
    }
}