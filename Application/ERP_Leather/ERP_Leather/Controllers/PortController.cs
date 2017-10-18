using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using System;
using System.Web.Mvc;
using System.Linq;
using ERP_Leather.ActionFilters;
using System.Web;

namespace ERP_Leather.Controllers
{
    public class PortController : Controller
    {
        private DalSysPort _dalSysPort;
        private ValidationMsg _vmMsg;

        public PortController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysPort = new DalSysPort();
        }

        [CheckUserAccess("Port/Port")]
        public ActionResult Port()
        {
            ViewBag.formTiltle = "Port Form";
            return View();
        }

        [HttpPost]
        public ActionResult Port(SysPort model)
        {
            if (model != null && model.PortID != 0)
            {
                _vmMsg = _dalSysPort.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysPort.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { PortID = _dalSysPort.GetPortID(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult Delete(string PortID)
        {
            _vmMsg = _dalSysPort.Delete(PortID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetPortList()
        {
            var sysPort = _dalSysPort.GetAll().OrderByDescending(s => s.PortID);
            return Json(sysPort, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            return View();
        }
    }
}