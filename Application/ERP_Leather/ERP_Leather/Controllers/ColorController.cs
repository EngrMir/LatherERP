using System;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using System;
using System.Web.Mvc;
using System.Linq;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class ColorController : Controller
    {
        private DalSysColor _dalSysColor;
        private ValidationMsg _vmMsg;

        public ColorController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysColor = new DalSysColor();
        }

        [CheckUserAccess("Color/Color")]
        public ActionResult Color()
        {
            ViewBag.formTiltle = "Color Form";
            //ViewBag.autoValue = _dalSysColor.GetPreDefineValue("1", "00042");
            return View();
        }

        [HttpPost]
        public ActionResult Color(SysColor model)
        {
            if (model != null && model.ColorID != 0)
            {
                _vmMsg = _dalSysColor.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysColor.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { ColorID = _dalSysColor.GetColorID(), msg = _vmMsg });
        }


        public ActionResult GetPreDefineValue()
        {
            var preDefineValue = _dalSysColor.GetPreDefineValue("1", "00042");
            return Json(preDefineValue, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string ColorID)
        {
            _vmMsg = _dalSysColor.Delete(ColorID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetColorList()
        {
            var sysColor = _dalSysColor.GetAll().OrderByDescending(s => s.ColorID);
            return Json(sysColor, JsonRequestBehavior.AllowGet);
        }
    }
}