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
    public class CLRequisitionForFinishingController : Controller
    {
        DalCLRequisitionForFinishing objDal = new DalCLRequisitionForFinishing();
        BllCLRequisitionForFinishing objBll = new BllCLRequisitionForFinishing();

        [HttpGet]
        [CheckUserAccess("CLRequisitionForFinishing/CLRequisitionForFinishing")]
        public ActionResult CLRequisitionForFinishing()
        {
            ViewBag.formTiltle = "Crust Leather Requisition For Finishing";
            ViewBag.ProductionFloor = DalCommon.GetStoreListForFixedCategoryType("Production", "FN Production");
            ViewBag.ConcernStore = DalCommon.GetStoreListForFixedCategoryType("Leather", "Crust");
            return View();
        }

        [HttpPost]
        [CheckUserAccess("CLRequisitionForFinishing/CLRequisitionForFinishing")]
        public ActionResult CLRequisitionForFinishing(PRDYearMonthCrustReqItem model)
        {
            var msg = objBll.Save(model, Convert.ToInt32(Session["UserID"]));

            var RequistionItemList = objDal.GetRequisitionItemList(model.RequisitionDateID.ToString());
            return Json(new { Msg = msg, RequistionItemList = RequistionItemList }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddNewYearMonth(string _ScheduleYear, string _ScheduleMonth, string _ProductionFloor, string _ConcernStore)
        {
            return Json(new { Msg = objDal.AddNewYearMonth(_ScheduleYear, _ScheduleMonth, _ProductionFloor, _ConcernStore), YearMonID = objDal.ReturnYearMonthID() },
                    JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNewRequisitionDate(string _ScheduleID, string _RequiredDate)
        {
            var RequisitionDateID = objDal.AddNewRequisitionDate(_ScheduleID, _RequiredDate);
            var RequisitionNo = objDal.ReturnRequisitionNo(RequisitionDateID);
            return Json(new { RequisitionDateID = RequisitionDateID, RequisitionNo = RequisitionNo },
                    JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllRequisitionDate(string _ScheduleID)
        {
            return Json(objDal.GetAllRequisitionDate(_ScheduleID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveColor()
        {
            return Json(objDal.GetAllActiveColor(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveBuyer()
        {
            return Json(objDal.GetAllActiveBuyer(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCategoryWiseUnit(string _UnitCategory)
        {
            return Json(objDal.GetCategoryWiseUnit(_UnitCategory), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllLeatherStatus()
        {
            return Json(objDal.GetAllLeatherStatus(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetScheduleList(string _ScheduleYear, string _ScheduleMonth, string _ProductionFloor)
        {
            return Json(objDal.GetScheduleList(_ScheduleYear, _ScheduleMonth, _ProductionFloor), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorListForScheduleItem(string _ScheduleItemID)
        {
            return Json(objDal.GetColorListForScheduleItem(_ScheduleItemID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBuyerOrderList(string _BuyerID)
        {
            return Json(objDal.GetBuyerOrderList(_BuyerID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorListForOrderItem(string _BuyerOrderItemID)
        {
            return Json(objDal.GetColorListForOrderItem(_BuyerOrderItemID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRequisitionItemList(string _RequisitionDateID)
        {
            return Json(objDal.GetRequisitionItemList(_RequisitionDateID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorListForRequisitionItem(string _RequisitionItemID)
        {
            return Json(objDal.GetColorListForRequisitionItem(_RequisitionItemID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteItem(string _RequisitionItemID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDal.DeleteItem(_RequisitionItemID);
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

        public ActionResult DeleteColorItem(string _ReqItemColorID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDal.DeleteColorItem(_ReqItemColorID);
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

        public ActionResult GetSearchInformation()
        {
            return Json(objDal.GetSearchInformation(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmRequisition(string _RequisitionDateID, string confirmComment)
        {
            bool checkConfirm = objDal.ConfirmRequisition(_RequisitionDateID, confirmComment);

            if (checkConfirm)
            {
                return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
	}
}