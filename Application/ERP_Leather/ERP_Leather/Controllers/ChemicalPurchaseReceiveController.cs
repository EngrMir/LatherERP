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
    public class ChemicalPurchaseReceiveController : Controller
    {
        DalSysStore objDalSysStore = new DalSysStore();
        DalSysCurrency objDalSysCurrency = new DalSysCurrency();
        DalSysSupplier objDalSysSupplier = new DalSysSupplier();
        DalSysSize objDalSysSize = new DalSysSize();
        DalSysUnit objDalSysUnit = new DalSysUnit();
        private DalPRQChemFrgnPurcRecv Dalobject;// = new DalPRQChemFrgnPurcRecv();
        private ValidationMsg _vmMsg;

        public ChemicalPurchaseReceiveController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalPRQChemFrgnPurcRecv();
        }

        [CheckUserAccess("ChemicalPurchaseReceive/ChemicalPurchaseReceive")]
        public ActionResult ChemicalPurchaseReceive()
        {
            ViewBag.formTiltle = "Chemical Foreign Purchase Receive";
            ViewBag.ddlStoreList = objDalSysStore.GetAllActiveChemicalStore();
            ViewBag.ddlCurrencyList = objDalSysCurrency.GetAllActiveCurrency();
            return View();
        }

        [HttpPost]
        public ActionResult ChemicalPurchaseReceive(PRQChemFrgnPurcRecv model)
        {
            _vmMsg = model.ReceiveID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "ChemicalPurchaseReceive/ChemicalPurchaseReceive") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { ReceiveID = Dalobject.GetReceiveID(), ReceiveNo = Dalobject.GetReceiveNo(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult ConfirmChemicalPurchase(PRQChemFrgnPurcRecv model)
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
            var chemicalSupplier = Dalobject.getSupplierList();
            return Json(chemicalSupplier, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackingInfoList(string supplierid)
        {
            var PlList = Dalobject.GetPackingInfoList(supplierid);
            return Json(PlList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackingItemList(string PLID)
        {
            var packingInfoList = Dalobject.GetPackingItemList(PLID);
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
            PRQChemFrgnPurcRecv model = new PRQChemFrgnPurcRecv();
            if (ReceiveID != null)
            {
                model.PrqChemFrgnPurcRecvPlList = Dalobject.GetPurcRecvPlList(ReceiveID);
                model.PrqChemFrgnPurcRecvChallanList = Dalobject.GetChallanList(ReceiveID);
                model.PrqChemFrgnPurcRecvItemList = Dalobject.GetRecvItemListList(ReceiveID);
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
        public ActionResult DeletedReceivePL(string PLReceiveID)
        {
            long pLReceiveID = Convert.ToInt64(PLReceiveID);
            _vmMsg = Dalobject.DeletedReceivePL(pLReceiveID);
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