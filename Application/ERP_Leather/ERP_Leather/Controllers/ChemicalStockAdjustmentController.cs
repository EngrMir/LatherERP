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
    public class ChemicalStockAdjustmentController : Controller
    {
        DalProductionRequisition objDal = new DalProductionRequisition();
        BllIssueToProduction objBLL = new BllIssueToProduction();
        DalIssueToProduction objDalIssue = new DalIssueToProduction();
        DalChemicalStockAdjustment objDalAdjust = new DalChemicalStockAdjustment();

        [CheckUserAccess("ChemicalStockAdjustment/ChemicalStockAdjustment")]
        public ActionResult ChemicalStockAdjustment()
        {
            ViewBag.RequisitionTo = objDal.GetCategoryTypeWiseStoreList("Production", "FN Production");
            ViewBag.RecipeFor = objDal.GetRecipeCategoryList();
            ViewBag.UnitForLeather = objDal.GetUnitListForFixedCategory("Leather");
            ViewBag.UnitForChemical = objDal.GetUnitListForFixedCategory("ChemicalPack");

            ViewBag.formTiltle = "Finish Chemical Consumption";
            return View();
        }

        //[CheckUserAccess("ChemicalStockAdjustment/ChemicalStockAdjustment")]
        public ActionResult ChemicalCommonInventoryAdjustment()
        {
            ViewBag.RequisitionTo = objDalAdjust.GetChemicalandProductionStore();
            ViewBag.RecipeFor = objDal.GetRecipeCategoryList();
            ViewBag.UnitForLeather = objDal.GetUnitListForFixedCategory("Leather");
            ViewBag.UnitForChemical = objDal.GetUnitListForFixedCategory("ChemicalPack");

            ViewBag.formTiltle = "Chemical Inventory Adjustment";
            return View();
        }


        public ActionResult ConfirmChemicalStockAdjustTransaction(long _TransactionID, string _CheckComment)
        {

            var Data = objDalAdjust.ConfirmChemicalStockAdjustTransaction(_TransactionID, _CheckComment, Convert.ToInt32(Session["UserID"]));

            return Json(Data, JsonRequestBehavior.AllowGet);
        }
	}
}