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
    public class SourceController : Controller
    {
        private DalSysSource _dalSysSource;
        private SysSource _sysSource = new SysSource();
        private ValidationMsg _vmMsg;

        public SourceController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysSource = new DalSysSource();
        }

        [CheckUserAccess("Source/Source")]
        public ActionResult Source()
        {
            ViewBag.formTiltle = "Source Form";
            return View(_sysSource);
        }

        [HttpPost]
        public ActionResult Source(SysSource model)
        {
            if (model != null && model.SourceID != 0)
            {
                _vmMsg = _dalSysSource.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysSource.Create(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { SourceID = _dalSysSource.GetSourceID(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult Delete(string sourceId)
        {
            _vmMsg = _dalSysSource.Delete(sourceId, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetSourceList()
        {
            var sysSource = _dalSysSource.GetAll().OrderByDescending(s => s.SourceID);
            return Json(sysSource, JsonRequestBehavior.AllowGet);
        }
    }
}