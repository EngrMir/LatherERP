using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class ChemPurcReqController : Controller
    {
        private ValidationMsg _vmMsg;
        private readonly DalChemPurcReq _objChemPurcReq;
        private string _pageMode;
        public ChemPurcReqController()
        {
            _vmMsg = new ValidationMsg();
            _objChemPurcReq = new DalChemPurcReq();
        }
        [CheckUserAccess("ChemPurcReq/ChemPurcReq")]
        public ActionResult ChemPurcReq()
        {
            ViewBag.formTiltle = "Chemical Purchase Requisition";
            ViewBag.ddlStoreList = new DalSysStore().GetAllActiveChemicalStore();
            ViewBag.ddlUserList = new DalUser().GetAllUserExceptSuper();
            
            return View();

        }
        [HttpPost]
        public ActionResult ChemPurcReq(PrqChemPurcReq modelReq)
        {
            _vmMsg = modelReq != null && modelReq.RequisitionID == 0
                ? _objChemPurcReq.SaveData(modelReq, Convert.ToInt32(Session["UserID"]), "ChemPurcReq/ChemPurcReq")
                : _objChemPurcReq.UpdateData(modelReq, Convert.ToInt32(Session["UserID"]));

            return Json(new { msg = _vmMsg }, JsonRequestBehavior.AllowGet);
        }
        [CheckUserAccess("ChemPurcReq/ChemPurcReqApproval")]
        public ActionResult ChemPurcReqApproval()
        {
            ViewBag.formTiltle = "Chemical Purchase Requisition Approval";
            ViewBag.ddlStoreList = new DalSysStore().GetAllActiveChemicalStore();
            
            return View();

        }
        [CheckUserAccess("ChemPurcReq/ChemOrderAgainstReq")]
        public ActionResult ChemOrderAgainstReq()
        {
            ViewBag.formTiltle = "Take Chemical Order Against Requisition";
            ViewBag.ddlStoreList = new DalSysStore().GetAllActiveChemicalStore();

            return View();

        }
        public ActionResult GetAllChemicalPurchaseItems()
        {
            var recipes = _objChemPurcReq.GetAllChemicalPurchaseItems();
             return Json(recipes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllNcfChemPurcReqList(string pageMode, string reqCategory)
        {
            var chemReq = _objChemPurcReq.GetAllNcfChemPurcReqList(pageMode,reqCategory);
            return Json(chemReq, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRequisitionItemList(int requisitionId)
        {
            var chemReqItems = _objChemPurcReq.GetRequisitionItemList(requisitionId);
            return Json(chemReqItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeletedChemPurcReq(int requisitionId)
        {
            _vmMsg = _objChemPurcReq.DeletedChemPurcReq(requisitionId);
            return Json(new { msg = _vmMsg });

        }

        public ActionResult DeletedChemPurcReqItem(long requisitionItemId, string recordStatus)
        {
            _vmMsg = _objChemPurcReq.DeletedChemPurcReqItem(requisitionItemId, recordStatus);
            return Json(new { msg = _vmMsg });

        }

        public ActionResult CheckData(int requisitionId,string chkComment)
        {
            _vmMsg = _objChemPurcReq.CheckData(requisitionId,chkComment, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmData(int requisitionId, string cnfComment)
        {
            _vmMsg = _objChemPurcReq.ConfirmData(requisitionId,cnfComment, Convert.ToInt32(Session["UserID"]));

            return Json(new { msg = _vmMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApprovedData(PrqChemPurcReq model)
        {
            _vmMsg = _objChemPurcReq.ApprovedData(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConcealChemPurcReq(string requisitionId,string recordStatus)
        {
            throw new NotImplementedException();
        }

        public ActionResult CreateChemicalOrderByRequisition(string requisitionIdList)
        {
            _vmMsg = _objChemPurcReq.CreateChemicalOrderByRequisition(requisitionIdList, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg }, JsonRequestBehavior.AllowGet);
        }
    }
}