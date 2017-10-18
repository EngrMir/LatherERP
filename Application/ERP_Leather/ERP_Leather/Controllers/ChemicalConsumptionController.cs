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
    public class ChemicalConsumptionController : Controller
    {
        DalPrdChemConsumption objDal = new DalPrdChemConsumption();
        BllWetBlueChemicalConsumption objBll = new BllWetBlueChemicalConsumption();
        
        [HttpGet]
        [CheckUserAccess("ChemicalConsumption/ChemConsumption")]   
        public ActionResult ChemConsumption()
        {
            ViewBag.formTiltle = "Chemical Consumption In Wet Blue Process";
            ViewBag.ProductionProcess = objDal.GetProductionProcessList();
            return View();
        }

        [HttpPost]
        [CheckUserAccess("ChemicalConsumption/ChemConsumption")]
        public ActionResult ChemConsumption(PrdWBProduction model)
        {
            if (model.WBProductionID == 0)
            {
                var msg = objBll.Save(model, Convert.ToInt32(Session["UserID"]), "ChemConsumption/ChemicalConsumption");
                long WBProductionID = objBll.GetProductionID();

                var LeatherList = objDal.GetLeatherListAfterSave(WBProductionID);
                var ChemicalList = objDal.GetChemicalListAfterSave(WBProductionID, model.ProductionFloor);
                return Json(new { Msg = msg, WBProductionID = WBProductionID, LeatherList = LeatherList, ChemicalList = ChemicalList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = objBll.Update(model, Convert.ToInt32(Session["UserID"]));
                var LeatherList = objDal.GetLeatherListAfterSave(model.WBProductionID);
                var ChemicalList = objDal.GetChemicalListAfterSave(model.WBProductionID, model.ProductionFloor);


                return Json(new { Msg = msg, LeatherList = LeatherList, ChemicalList = ChemicalList }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetScheduleInformation()
        {
            var Data = objDal.GetScheduleInformation();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductionInformation(string _ScheduleID)
        {
            var Data = objDal.GetProductionInformation(_ScheduleID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductionDetails(string _ScheduleDateID)
        {
            var Data = objDal.GetProductionDetails(_ScheduleDateID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRecipeForFixedProcess(int _RecipeFor)
        {
            var Data = objDal.GetRecipeForFixedProcess(_RecipeFor);

            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRecipeItemListForFixedRecipe(int _RecipeID, int _StoreID, decimal? _Weight, decimal? _Area)
        {
            var Data = objDal.GetRecipeItemListForFixedRecipe(_RecipeID, _StoreID, _Weight, _Area);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSearchInformaiton()
        {
            var Data = objDal.GetScheduleInformationForSearch();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailsSearchInformaiton(long _WBProductionID)
        {
            var Data = objDal.GetDetailsSearchInformaiton(_WBProductionID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChemicalItemListForFixedLeather(long _WBProductionID, long _WBProductionPurchaseID, byte? _StoreID)
        {
            var Data = objDal.GetChemicalItemListForFixedLeather(_WBProductionID, _WBProductionPurchaseID, _StoreID);

            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteProductionItem(string _WBProdChemicalID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed" || _PageStatus == "CHK" || _PageStatus == "Checked")
            {
                CheckStatus = objDal.DeleteProductionItem(_WBProdChemicalID);
                if (CheckStatus)
                {
                    return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (_PageStatus == "CNF" || _PageStatus == "Confirmed")
            {
                return Json(new { Msg = "CNF" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteChemicalConsumption(string _WBProductionID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDal.DeleteChemicalConsumption(_WBProductionID);
                if (CheckStatus)
                {
                    return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (_PageStatus == "CNF" || _PageStatus == "Confirmed")
            {
                return Json(new { Msg = "CNF" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckChemicalConsumtion(string _WBProductionID, string _ConfirmComment)
        {

            var Data = objDal.CheckChemicalConsumtion(_WBProductionID, _ConfirmComment);

            if (Data)
            {
                return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ConfirmChemicalConsumption(long _WBProductionID, string _ConfirmComment)
        {

            var Data = objDal.ConfirmChemicalConsumption(_WBProductionID, _ConfirmComment, Convert.ToInt32(Session["UserID"]));

            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChemicalItemListInProductionFloor(byte _ProductionFloor)
        {
            return Json(objDal.GetChemicalItemListInProductionFloor(_ProductionFloor), JsonRequestBehavior.AllowGet);
        }
	}
}