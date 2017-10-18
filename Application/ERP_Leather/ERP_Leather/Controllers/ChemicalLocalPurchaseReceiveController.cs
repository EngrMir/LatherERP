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
    public class ChemicalLocalPurchaseReceiveController : Controller
    {
        DalSysStore objDalSysStore = new DalSysStore();
        DalSysCurrency objDalSysCurrency = new DalSysCurrency();
        DalSysSupplier objDalSysSupplier = new DalSysSupplier();
        DalSysSize objDalSysSize = new DalSysSize();
        DalSysUnit objDalSysUnit = new DalSysUnit();
        private DalChemicalLocalPurchaseReceive Dalobject;
        private ValidationMsg _vmMsg;

        public ChemicalLocalPurchaseReceiveController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalChemicalLocalPurchaseReceive();
        }

        [CheckUserAccess("ChemicalLocalPurchaseReceive/ChemicalLocalPurchaseReceive")]
        public ActionResult ChemicalLocalPurchaseReceive()
        {
            ViewBag.formTiltle = "Chemical Local Purchase Receive";
            ViewBag.ddlStoreList = objDalSysStore.GetAllActiveChemicalStore();
            ViewBag.ddlCurrencyList = objDalSysCurrency.GetAllActiveCurrency();
            return View();
        }

        [HttpPost]
        public ActionResult ChemicalLocalPurchaseReceive(PRQChemLocalPurcRecv model)
        {
            _vmMsg = model.ReceiveID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "ChemicalLocalPurchaseReceive/ChemicalLocalPurchaseReceive") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { ReceiveID = Dalobject.GetReceiveID(), ReceiveNo = Dalobject.GetReceiveNo(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult ConfirmChemicalPurchase(PRQChemLocalPurcRecv model)
        {
            _vmMsg = Dalobject.ConfirmChemicalPurchase(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult CheckedChemicalPurchase(string receiveId, string checkComment)
        {
            _vmMsg = Dalobject.CheckedChemicalPurchase(receiveId, checkComment, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        public ActionResult GetChemicalSupplierList()
        {
            var supplierlist = Dalobject.getSupplierList();
            return Json(supplierlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackingInfoList(string supplierid)
        {
            var orderList = Dalobject.GetPackingInfoList(supplierid);
            return Json(orderList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackingItemList(string OrderID)
        {
            var packingInfoList = Dalobject.GetPackingItemList(OrderID);
            return Json(packingInfoList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackingAfterSaveList(string ReceiveID)
        {
            var packItemList = Dalobject.GetPackingAfterSaveList(ReceiveID);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackingItemAfterSaveList(string ReceiveID)
        {
            var packItemList = Dalobject.GetRecvItemListList(ReceiveID);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChallanAfterSaveList(string ReceiveID)
        {
            var challanList = Dalobject.GetChallanList(ReceiveID);
            return Json(challanList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllChemicalSupplierList(string supplier)
        {
            SysSupplier sysSupplier = new SysSupplier();

            var supplierList = objDalSysSupplier.GetAllChemicalSupplierList(supplier);
            if (supplierList.Count > 1)
            {
                sysSupplier.Count = 0;
            }
            else
            {
                sysSupplier.Count = 1;
            }
            sysSupplier.ChemicalSupplierList = supplierList;
            return Json(sysSupplier, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChemicalPurchaseReceiveList()
        {
            var chemicalPurchaseReceiveList = Dalobject.GetChemicalPurchaseReceiveList();
            return Json(chemicalPurchaseReceiveList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllGridList(string ReceiveID)
        {
            PRQChemLocalPurcRecv model = new PRQChemLocalPurcRecv();
            if (ReceiveID != null)
            {
                model.PrqChemLocalPurcRecvPOList = Dalobject.GetPurcRecvPlList(ReceiveID);
                model.PrqChemLocalPurcRecvChallanList = Dalobject.GetChallanList(ReceiveID);
                model.PrqChemLocalPurcRecvItemList = Dalobject.GetRecvItemListList(ReceiveID);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveChemicalPackSize()
        {
            var chemicalItemSizeList = objDalSysSize.GetAllActiveChemicalPackSize();
            return Json(chemicalItemSizeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveChemicalPackUnit()
        {
            var chemicalUnitList = objDalSysUnit.GetAllActiveChemicalPack();
            return Json(chemicalUnitList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveChemicalUnit()
        {
            var chemicalUnitList = objDalSysUnit.GetAllActiveLeatherChemical();
            return Json(chemicalUnitList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChemicalItemList()
        {
            var chemicalList = Dalobject.GetChemicalItemList();
            return Json(chemicalList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedReceive(string ReceiveID)
        {
            long receiveID = Convert.ToInt64(ReceiveID);
            _vmMsg = Dalobject.DeletedReceive(receiveID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedReceiveOrder(string POReceiveID)
        {
            long pOReceiveID = Convert.ToInt64(POReceiveID);
            _vmMsg = Dalobject.DeletedReceiveOrder(pOReceiveID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedReceiveChallan(string ReceiveChallanID)
        {
            long receiveChallanID = Convert.ToInt64(ReceiveChallanID);
            _vmMsg = Dalobject.DeletedReceiveChallan(receiveChallanID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedReceiveItem(string ReceiveItemID)
        {
            long receiveItemID = Convert.ToInt64(ReceiveItemID);
            _vmMsg = Dalobject.DeletedReceiveItem(receiveItemID);
            return Json(new { msg = _vmMsg });
        }
	}
}