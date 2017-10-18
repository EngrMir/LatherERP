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
    public class StoreController : Controller
    {
        private DalSysStore _dalSysStore;
        private SysStore _sysStore = new SysStore();

        private ValidationMsg _vmMsg;

        public StoreController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysStore = new DalSysStore();
        }

        [CheckUserAccess("Store/Store")]
        public ActionResult Store()
        {
            ViewBag.formTiltle = "Store Form";
            return View();
        }

        [HttpPost]
        public ActionResult Store(SysStore model)
        {
            if (model != null && model.StoreID != 0)
            {
                _vmMsg = _dalSysStore.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysStore.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { StoreID = _dalSysStore.GetStoreId(), msg = _vmMsg });
        }
        [HttpPost]
        public ActionResult Delete(string storeId)
        {
            _vmMsg = _dalSysStore.Delete(storeId, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetStoreList()
        {
            var sysSource = _dalSysStore.GetAll().OrderByDescending(s => s.StoreID);
            return Json(sysSource, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetWetBlueStoreList()
        {
            var sysSource = _dalSysStore.GetAllActiveWetBlueStore().OrderByDescending(s => s.StoreID);
            return Json(sysSource, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAllProductionStoreList()
        {
            var sysSource = _dalSysStore.GetAllActiveProductionStore().OrderByDescending(s => s.StoreID);
            return Json(sysSource, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetStoreListSearchById(string store)
        {
            store = store.ToUpper();
            var supplierData = _dalSysStore.GetAll().Where(ob => ob.StoreName.StartsWith(store)).ToList();
            return Json(supplierData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStoreListForSearch()
        {
            var supplierAgentList = _dalSysStore.GetStoreListForSearch();
            return Json(supplierAgentList, JsonRequestBehavior.AllowGet);
        }


	}
}