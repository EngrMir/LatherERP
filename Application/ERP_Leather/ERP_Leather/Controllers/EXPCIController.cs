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
    public class EXPCIController : Controller
    {
        private DalEXPCI Dalobject;
        private ValidationMsg _vmMsg;
        DalSysCurrency objCurrency = new DalSysCurrency();

        public EXPCIController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalEXPCI();
        }

        [CheckUserAccess("EXPCI/EXPCI")]
        public ActionResult EXPCI()
        {
            ViewBag.formTiltle = "Commercial Invoice";
            ViewBag.ddlCurrencyList = objCurrency.GetAllActiveCurrency();
            return View();
        }

        [HttpPost]
        public ActionResult EXPCI(EXPCI model)
        {
            _vmMsg = model.CIID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "EXPCI/EXPCI") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { CIID = Dalobject.GetCIID(), CINo = Dalobject.GetCINo(), CIPIID = Dalobject.GetCIPIID(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult ConfirmedEXPCI(EXPCI model)
        {
            _vmMsg = Dalobject.ConfirmedEXPCI(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult CheckedEXPCI(EXPCI model)
        {
            _vmMsg = Dalobject.ConfirmedEXPCI(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        public ActionResult GetBuyerList()
        {
            var buyerlist = Dalobject.GetBuyerList();
            return Json(buyerlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCIInformation()
        {
            return Json(Dalobject.GetCIInformation(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCIPIInformation(string CIID)
        {
            return Json(Dalobject.GetCIPIInformation(CIID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExpPIList(string _BuyerID)
        {
            return Json(Dalobject.GetExpPIList(_BuyerID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExpPIItemAndColorList(string PIID, string OrdDeliveryMode)
        {
            EXPCI model = new EXPCI();
            if (string.IsNullOrEmpty(PIID)) return Json(model, JsonRequestBehavior.AllowGet);
            model.ExpCIPIItemList = Dalobject.GetExpPIItemList(PIID, OrdDeliveryMode);
            if (model.ExpCIPIItemList.Count > 0)
            {
                model.ExpCIPIItemColorList = Dalobject.GetExpPIItemColorList(model.ExpCIPIItemList[0].PIItemID.ToString(), OrdDeliveryMode);
                //model.BuyerOrderPriceList = Dalobject.GetBuyerOrderPriceList(PIID);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExpPIItemColorList(string PIItemID, string OrdDeliveryMode)
        {
            var packItemList = Dalobject.GetExpPIItemColorList(PIItemID, OrdDeliveryMode);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExpCIItemColorList(string CIPIItemID, string PIItemID, string OrdDeliveryMode)
        {
            var packItemList = Dalobject.GetExpCIItemColorList(CIPIItemID);
            if (packItemList.Count > 0)
                return Json(packItemList, JsonRequestBehavior.AllowGet);
            else
                packItemList = Dalobject.GetExpPIItemColorList(PIItemID, OrdDeliveryMode);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllGridList(string CIPIID)
        {
            EXPCI model = new EXPCI();
            if (string.IsNullOrEmpty(CIPIID)) return Json(model, JsonRequestBehavior.AllowGet);
            model.ExpCIPIItemList = Dalobject.GetExpCIPIItemList(CIPIID);
            if (model.ExpCIPIItemList.Count > 0)
            {
                model.ExpCIPIItemColorList = Dalobject.GetExpCIItemColorList(model.ExpCIPIItemList[0].CIPIItemID.ToString());
                //model.BuyerOrderPriceList = Dalobject.GetBuyerOrderPriceList(PIID);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedCIInfo(string CIID)
        {
            long? cIID = Convert.ToInt64(CIID);
            _vmMsg = Dalobject.DeletedCIInfo(cIID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedCIPIItem(string CIPIItemID)
        {
            long? cIPIItemID = Convert.ToInt64(CIPIItemID);
            _vmMsg = Dalobject.DeletedCIPIItem(cIPIItemID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedCIPIItemColor(string CIPIItemColorID)
        {
            long? cIPIItemColorID = Convert.ToInt64(CIPIItemColorID);
            _vmMsg = Dalobject.DeletedCIPIItemColor(cIPIItemColorID);
            return Json(new { msg = _vmMsg });
        }
    }
}