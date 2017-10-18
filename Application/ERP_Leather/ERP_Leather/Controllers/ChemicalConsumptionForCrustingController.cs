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
    public class ChemicalConsumptionForCrustingController : Controller
    {
        DalChemicalConsumptionForCrusting objDal = new DalChemicalConsumptionForCrusting();
        BllChemicalConsumptionForCrusting objBll = new BllChemicalConsumptionForCrusting();

        [HttpGet]
        [CheckUserAccess("ChemicalConsumptionForCrusting/ChemicalConsumptionForCrusting")]
        public ActionResult ChemicalConsumptionForCrusting()
        {
            ViewBag.formTiltle = "Chemical Consumption In Crusting Process";
            ViewBag.ProductionProcess = DalCommon.GetCategoryWiseProductionProcess("CR");
            return View();
        }

        [HttpPost]
        [CheckUserAccess("ChemicalConsumptionForCrusting/ChemicalConsumptionForCrusting")]
        public ActionResult ChemicalConsumptionForCrusting(ChemicalConsumptionForCrusting model)
        {
            if (model.CLProductionID == 0)
            {
                var msg = objBll.Save(model, Convert.ToInt32(Session["UserID"]), "ChemicalConsumptionForCrusting/ChemicalConsumptionForCrusting");
                long CLProductionID = objBll.GetProductionID();
                long CLProductionItemID=objDal.GetCLProductionItemID(CLProductionID);
                long CLProductionColorID = objDal.GetCLProductionColorID(CLProductionID); ;

                return Json(new { Msg = msg, CLProductionID = CLProductionID, CLProductionItemID = CLProductionItemID, CLProductionColorID = CLProductionColorID,
                    LeatherList = objDal.GetLeatherListAfterSave(CLProductionID), 
                    ChemicalList = objDal.GetChemicalListAfterSave(CLProductionID, model.ProductionFloor, model.CalculationBase) }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var msg = objBll.Update(model, Convert.ToInt32(Session["UserID"]));
                //var LeatherList = objDal.GetLeatherListAfterSave(model.WBProductionID);
                var ChemicalList = objDal.GetChemicalListAfterSave(model.CLProductionID, model.ProductionFloor, model.CalculationBase);


                return Json(new { Msg = msg, ChemicalList = ChemicalList }, JsonRequestBehavior.AllowGet);
            }
            
        }


        public ActionResult GetAllYearMonthCombinationForConsumption(string _ScheduleFor)
        {
            return Json(objDal.GetAllYearMonthCombinationForConsumption(_ScheduleFor), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllScheduleDate(string _ScheduleID)
        {
            return Json(objDal.GetAllScheduleDate(_ScheduleID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllScheduleItem(string _ScheduleDateID)
        {
            return Json(objDal.GetAllScheduleItem(_ScheduleDateID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllDrumList(string _SdulItemColorID)
        {
            return Json(objDal.GetAllDrumList(_SdulItemColorID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSearchInformaiton()
        {
            var Data = objDal.GetSearchInformaiton();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailsSearchInformaiton(long _CLProductionID)
        {
            var Data = objDal.GetDetailsSearchInformaiton(_CLProductionID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetChemicalItemListForFixedLeather(long _CLProductionID, long _CLProductionDrumID, byte? _StoreID)
        {
            var Data = objDal.GetChemicalItemListForFixedLeather(_CLProductionID, _CLProductionDrumID, _StoreID);

            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteProductionItem(string _CLProdChemicalID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed" || _PageStatus == "CHK" || _PageStatus == "Checked")
            {
                CheckStatus = objDal.DeleteProductionItem(_CLProdChemicalID);
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


        public ActionResult DeleteChemicalConsumption(string _CLProductionID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDal.DeleteChemicalConsumption(_CLProductionID);
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

        public ActionResult CheckChemicalConsumtion(string _CLProductionID, string _ConfirmComment)
        {

            var Data = objDal.CheckChemicalConsumtion(_CLProductionID, _ConfirmComment);

            if (Data)
            {
                return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ConfirmChemicalConsumption(long _CLProductionID, string _ConfirmComment)
        {

            var Data = objDal.ConfirmChemicalConsumption(_CLProductionID, _ConfirmComment, Convert.ToInt32(Session["UserID"]));

            return Json(Data, JsonRequestBehavior.AllowGet);
        }
	}
}