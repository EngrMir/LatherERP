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
    public class PreDefineValueController : Controller
    {
        private DalPreDefineValue _dalSysPreDefineValue;
        private SysPreDefineValueFor _sysPreDefineValue = new SysPreDefineValueFor();

        private ValidationMsg _vmMsg;

        public PreDefineValueController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysPreDefineValue = new DalPreDefineValue();
        }

        [CheckUserAccess("PreDefineValue/PreDefineValue")]
        public ActionResult PreDefineValue()
        {
            ViewBag.formTiltle = "PreDefine Value Form";
            return View();
        }

        [HttpPost]
        public ActionResult PreDefineValue(SysPreDefineValueFor model)
        {
            if (model != null && model.PreDefineValueForID != 0)
            {
                _vmMsg = _dalSysPreDefineValue.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysPreDefineValue.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { PreDefineValueForID = _dalSysPreDefineValue.GetPreDefineValueForId(), msg = _vmMsg });
        }

        public ActionResult GetPreDefineValueList(string PreDefineValueForID) 
        {
            var _sysPreDefineValue = _dalSysPreDefineValue.GetPreDefineValueList(PreDefineValueForID).OrderByDescending(m=>m.PreDefineValueID);
            return Json(_sysPreDefineValue, JsonRequestBehavior.AllowGet); 
        }

        [HttpPost]
        public ActionResult Delete(string PreDefineValueForID)
        {
            _vmMsg = _dalSysPreDefineValue.Delete(PreDefineValueForID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedPreDefineValue(string PreDefineValueId)
        {
            _vmMsg = _dalSysPreDefineValue.DeletedPreDefineValue(PreDefineValueId);
            return Json(new { msg = _vmMsg });
        }

        public ActionResult GetConcernPageList()
        {
            var buyerAgentList = _dalSysPreDefineValue.GetConcernPageList();
            return Json(buyerAgentList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPreDefineValueForSearchList()
        {
            var searchList = _dalSysPreDefineValue.GetPreDefineValueForForSearchList().OrderByDescending(m => m.PreDefineValueForID);
            return Json(searchList, JsonRequestBehavior.AllowGet);
        }
    }
}