using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_Leather.ActionFilters;


namespace ERP_Leather.Controllers
{
    public class WetBlueProductionScheduleController : Controller
    {
        DalSysStore objDalSysStore = new DalSysStore();
        DalSysLeatherType objDalSysLeatherType = new DalSysLeatherType();
        DalSysUnit objDalSysUnit = new DalSysUnit();
        DalSysProductionProces daiSysProductionProces = new DalSysProductionProces();
        DalSysMachine dalSysMachine = new DalSysMachine();
        private DalWetBlueProductionSchedule Dalobject;
        private ValidationMsg _vmMsg;

        public WetBlueProductionScheduleController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalWetBlueProductionSchedule();
        }

        [CheckUserAccess("WetBlueProductionSchedule/WetBlueProductionSchedule")]
        public ActionResult WetBlueProductionSchedule()
        {
            ViewBag.formTiltle = "Wet Blue Production Schedule";
            ViewBag.ddlProductionStoreList = objDalSysStore.GetAllActiveProductionStore();
            ViewBag.ddlProcessList = daiSysProductionProces.GetAllActiveWetBlueProcessList();
            ViewBag.ddlConcernStoreList = objDalSysStore.GetAllActiveStore();
            return View();
        }

        [HttpPost]
        public ActionResult WetBlueProductionSchedule(PrdYearMonth model)
        {
            _vmMsg = model.YearMonID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "WetBlueProductionSchedule/WetBlueProductionSchedule") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { YearMonID = Dalobject.GetYearMonID(), ScheduleNo = Dalobject.GetScheduleNo(), msg = _vmMsg });
        }

        public ActionResult GetWetBlueYearMonthList()
        {
            var crustYearMonth = Dalobject.GetWetBlueYearMonthList();
            return Json(crustYearMonth, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWetBlueYearMonthScheduleList(string YearMonID)
        {
            var crustYearMonthScheduleList = Dalobject.GetWetBlueYearMonthScheduleList(YearMonID);
            return Json(crustYearMonthScheduleList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetYearMonthSchedulePurchaseList(string ScheduleDateID, string StoreId)
        {
            var packItemList = Dalobject.GetYearMonthSchedulePurchaseList(ScheduleDateID, StoreId);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedYearMonthSchedule(string ScheduleNo)
        {
            _vmMsg = Dalobject.DeletedYearMonthSchedule(ScheduleNo);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedYearMonthScheduleDate(string ScheduleDateID, string RecordStatus)
        {
            long scheduleDateID = Convert.ToInt64(ScheduleDateID);
            _vmMsg = Dalobject.DeletedYearMonthScheduleDate(scheduleDateID, RecordStatus);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedYearMonthSchedulePurchase(string SchedulePurchaseID, string RecordStatus)
        {
            long schedulePurchaseID = Convert.ToInt64(SchedulePurchaseID);
            _vmMsg = Dalobject.DeletedYearMonthSchedulePurchase(schedulePurchaseID, RecordStatus);
            return Json(new { msg = _vmMsg });
        }

        public ActionResult GetWetBlueProductionSchedule()
        {
            var packItemList = Dalobject.GetWetBlueProductionSchedule();
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllGridList(string ScheduleNo, string StoreId)
        {
            PrdYearMonth model = new PrdYearMonth();
            if (string.IsNullOrEmpty(ScheduleNo)) return Json(model, JsonRequestBehavior.AllowGet);
            model.PrdYearMonthScheduleDateList = Dalobject.GetYearMonthScheduleDateList(ScheduleNo);
            if (model.PrdYearMonthScheduleDateList.Count > 0)
                model.PrdYearMonthSchedulePurchaseList = Dalobject.GetYearMonthSchedulePurchaseList(model.PrdYearMonthScheduleDateList[0].ScheduleDateID.ToString(), StoreId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveLeatherType()
        {
            var packItemList = objDalSysLeatherType.GetAllActiveLeatherType();
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveUnit()
        {
            var packItemList = objDalSysUnit.GetAllActiveLeatherUnit();
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMachineNoInfo()
        {
            var machineList = dalSysMachine.GetAllWetBlueProductionMachine();
            return Json(machineList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductionNoList(string ProductionFloor, string ScheduleID)
        {
            var machineList = Dalobject.GetProductionNoList(ProductionFloor, ScheduleID);
            return Json(machineList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetScheduleItemList(string ScheduleDateID)
        {
            var machineList = Dalobject.GetYearMonthSchedulePurchaseList(ScheduleDateID);
            return Json(machineList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPurchaseNoInfo()
        {
            var packItemList = Dalobject.GetPurchaseNoInfo();
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConfirmedWetBlueProductionSchedule(PrdYearMonth model)
        {
            _vmMsg = Dalobject.ConfirmedWetBlueProductionSchedule(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult CheckedWetBlueProductionSchedule(PrdYearMonth model)
        {
            _vmMsg = Dalobject.CheckedWetBlueProductionSchedule(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult ConcealWetBlueProductionSchedule(string ScheduleDateID, string ProductionStatus)
        {
            _vmMsg = Dalobject.ConcealWetBlueProductionSchedule(ScheduleDateID, ProductionStatus, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        public ActionResult ExecuteWetBlueProductionSchedule(PrdYearMonth model)
        {
            _vmMsg = Dalobject.ExecuteWetBlueProductionSchedule(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        public JsonResult GetSupplierFromStoreList(string ConcernStore)
        {
            var supplierAgentList = Dalobject.GetSupplierFromStoreList(ConcernStore);
            return Json(supplierAgentList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSupplierListForSearch()
        {
            //var supplierAgentList = Dalobject.GetSupplierListForSearch(ConcernStore);
            var supplierAgentList = Dalobject.GetSupplierListForSearch();
            return Json(supplierAgentList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeatherInfoList(string ConcernStore, string SupplierID)
        {
            var packItemList = Dalobject.GetLeatherInfoList(ConcernStore, SupplierID);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSupplierList(string storeId, string supplier)
        {
            SysSupplier sysSupplier = new SysSupplier();

            var supplierList = Dalobject.GetSupplierList(supplier, storeId);
            sysSupplier.Count = supplierList.Count > 1 ? 0 : 1;
            sysSupplier.SupplierList = supplierList;
            return Json(sysSupplier, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult WetBlueProductionScheduleClosed(string ScheduleNo)
        {
            _vmMsg = Dalobject.WetBlueProductionScheduleClosed(ScheduleNo, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }
    }
}
