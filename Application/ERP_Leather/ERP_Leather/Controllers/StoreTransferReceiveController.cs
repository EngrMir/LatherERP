using System;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.BusinessLogicLayer.OperationManager;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class StoreTransferReceiveController : Controller
    {
        DalProductionRequisition objDal = new DalProductionRequisition();
        BllProductionRequisition objBLL = new BllProductionRequisition();
        DalStoreTransferReceive objReceive = new DalStoreTransferReceive();
        BllStoreTransferReceive objBllReceive = new BllStoreTransferReceive();



        [HttpGet]
        [CheckUserAccess("StoreTransferReceive/StoreTransferReceive")]
        public ActionResult StoreTransferReceive()
        {
            //ViewBag.RequisitionFrom = objDal.GetCategoryTypeWiseStoreList("Chemical", "Production");
            ViewBag.RequisitionTo = objDal.GetCategoryTypeWiseStoreList("Chemical", "Chemical");
            ViewBag.RecipeFor = objDal.GetRecipeCategoryList();
            ViewBag.UnitForLeather = objDal.GetUnitListForFixedCategory("Leather");
            ViewBag.UnitForChemical = objDal.GetUnitListForFixedCategory("ChemicalPack");

            ViewBag.formTiltle = "Store Transfer Receive";
            return View();
        }

        [HttpPost]
        [CheckUserAccess("StoreTransferReceive/StoreTransferReceive")]
        public ActionResult StoreTransferReceive(INVStoreTrans model)
        {
            if (model.TransactionID == 0)
            {
                var msg = objBllReceive.Save(model, Convert.ToInt32(Session["UserID"]), "StoreTransferIssue/StoreTransferIssue");
                var TransactionID = objBllReceive.GetTransactionID();
                var TransactionNo = objReceive.GetTransactionNo(TransactionID);

                var TransactionItemList = objReceive.GetTransactionItemList(TransactionID);
                return Json(new { Msg = msg, TransactionID = TransactionID, TransactionNo = TransactionNo, TransactionItemList = TransactionItemList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = objBllReceive.Update(model, 1);
                var TransactionItemList = objReceive.GetTransactionItemList(model.TransactionID);

                return Json(new { Msg = msg, TransactionItemList = TransactionItemList }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTransactionInfoForSearch()
        {
            var Data = objReceive.GetTransactionInfoForSearch();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransactionDetailsAfterSearch(long _TransactionID)
        {
            var Data = objReceive.GetTransactionDetailsAfterSearch(_TransactionID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetIssuedItemForLOV()
        {
            var Data = objReceive.GetIssuedItemForLOV();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIssuedItemDetailsAfterLOV(long _TransactionID)
        {
            var Data = objReceive.GetIssuedItemDetailsAfterLOV(_TransactionID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetSupplierForChemical()
        {
            var Data = objReceive.GetSupplierForChemical();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmIssueToProductionTransaction(long _TransactionID, string _CheckComment)
        {

            var Data = objReceive.ConfirmIssueToProductionTransaction(_TransactionID, _CheckComment, Convert.ToInt32(Session["UserID"]));

            return Json(Data, JsonRequestBehavior.AllowGet);
        }
	}
}