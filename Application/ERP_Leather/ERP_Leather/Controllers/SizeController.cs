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
    public class SizeController : Controller
    {
        private DalSysSize _dalSysSize;
        private SysSize _sysSize = new SysSize();

        private ValidationMsg _vmMsg;

        public SizeController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysSize = new DalSysSize();
        }

        [CheckUserAccess("Size/Size")]
        public ActionResult Size()
        {
            ViewBag.formTiltle = "Size Form";
            return View();
        }

        [HttpPost]
        public ActionResult Size(SysSize model)
        {
            if (model != null && model.SizeID != 0)
            {
                _vmMsg = _dalSysSize.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysSize.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { SizeID = _dalSysSize.GetSizeId(), msg = _vmMsg });
        }
        [HttpPost]
        public ActionResult Delete(string sizeId)
        {
            _vmMsg = _dalSysSize.Delete(sizeId, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetSizeList()
        {
            var sysSource = _dalSysSize.GetAll().OrderByDescending(s => s.SizeID);
            return Json(sysSource, JsonRequestBehavior.AllowGet);
        }
    }
}