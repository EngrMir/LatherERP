using System;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.BusinessLogicLayer.OperationManager;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP_Leather.ActionFilters;
using ERP.DatabaseAccessLayer.Utility;
using System.Linq;


namespace ERP_Leather.Controllers
{
    public class WBRequisitionForCrustingController : Controller
    {
        DalWBRequisitionForCrusting objDal = new DalWBRequisitionForCrusting();
        BllWBRequisitionForCrusting objBll = new BllWBRequisitionForCrusting();
        UnitOfWork repository = new UnitOfWork();
        [HttpGet]
        [CheckUserAccess("WBRequisitionForCrusting/WBRequisitionForCrusting")]
        public ActionResult WBRequisitionForCrusting()
        {
            ViewBag.formTiltle = "Wet Blue Requisition For Crusting";
            ViewBag.ProductionFloor = DalCommon.GetStoreListForFixedCategoryType("Production", "CR Production");
            ViewBag.ConcernStore = DalCommon.GetStoreListForFixedCategoryType("Leather", "Wet Blue");
            return View();
        }

        [HttpPost]
        [CheckUserAccess("WBRequisitionForCrusting/WBRequisitionForCrusting")]
        public ActionResult WBRequisitionForCrusting(PRDYearMonthCrustReqItem model)
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

        public ActionResult GetAllYearMonthCombination(string _ScheduleFor)
        {
            return Json(objDal.GetAllYearMonthCombinationForRequisition(_ScheduleFor), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNewRequisition(string _YearMonID, string _PrepareDate)
        {
            var ScheduleID = objDal.AddNewRequisition(_YearMonID, _PrepareDate);
            return Json(new { ScheduleID = ScheduleID, ScheduleNo = objDal.ReturnScheduleNo(ScheduleID) },
                    JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNewRequisitionDate(string _ScheduleID, string _RequiredDate)
        {
            var RequisitionDateID = objDal.AddNewRequisitionDate(_ScheduleID, _RequiredDate);
            return Json(new { RequisitionDateID = RequisitionDateID, RequisitionNo = objDal.ReturnRequisitionNo(RequisitionDateID) },
                    JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllRequisition(string _YearMonID)
        {
            return Json(objDal.GetAllRequisition(_YearMonID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllRequisitionDate(string _ScheduleID)
        {
            return Json(objDal.GetAllRequisitionDate(_ScheduleID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllActiveColor()
        {
            return Json(objDal.GetAllActiveColor(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllActiveArticle()
        {
            return Json(objDal.GetAllActiveArticle(), JsonRequestBehavior.AllowGet);
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


        public ActionResult GetArticleChallanList(int? _BuyerID, int? _ArticleID)
        {
            var Data = objDal.GetArticleChallanList(_BuyerID, _ArticleID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorListAccordingToArticleChallan(int? _BuyerOrderItemID, long _ArticleChallanID)
        {
            var Data = objDal.GetColorListAccordingToArticleChallan(_BuyerOrderItemID, _ArticleChallanID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        #region Challan Popup Grid
        public ActionResult GetChallanInfo(int? buyerID, int? articleID)
        {
            if (buyerID == null || articleID == null)
            {
                var result = (from temp in repository.SysArticleChallanRepository.Get()
                              select new
                              {
                                  ArticleChallanID = temp.ArticleChallanID,
                                  ArticleChallanNo = temp.ArticleChallanNo,
                                  ChallanNote = temp.ChallanNote,
                                  PreparationDate = Convert.ToDateTime(temp.PreparationDate).ToString("dd/MM/yyyy")
                              }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            {
                var result = (from temp in repository.SysArticleChallanRepository.Get()
                              where temp.BuyerID == buyerID && temp.ArticleID == articleID
                              select new
                              {
                                  ArticleChallanID = temp.ArticleChallanID,
                                  ArticleChallanNo = temp.ArticleChallanNo,
                                  ChallanNote = temp.ChallanNote,
                                  PreparationDate = Convert.ToDateTime(temp.PreparationDate).ToString("dd/MM/yyyy")
                              }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult SearchChallanByFirstName(string search, int? buyerID, int? articleID)
        {
            if (search == null)
            {
                var result = from temp in repository.SysArticleChallanRepository.Get()
                             where temp.BuyerID == buyerID && temp.ArticleID == articleID
                             select new
                             {
                                 ArticleChallanID = temp.ArticleChallanID,
                                 ArticleChallanNo = temp.ArticleChallanNo,
                                 ChallanNote = temp.ChallanNote,
                                 PreparationDate = Convert.ToDateTime(temp.PreparationDate).ToString("dd/MM/yyyy")
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                search = search.ToUpper();
                var result = from temp in repository.SysArticleChallanRepository.Get()
                             where ((temp.ArticleChallanNo.ToUpper().StartsWith(search) || temp.ArticleChallanNo.ToUpper() == search))
                             select new
                             {
                                 ArticleChallanID = temp.ArticleChallanID,
                                 ArticleChallanNo = temp.ArticleChallanNo,
                                 ChallanNote = temp.ChallanNote,
                                 PreparationDate = Convert.ToDateTime(temp.PreparationDate).ToString("dd/MM/yyyy")
                             };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetChallanAutocompleteData()
        {
            var search = repository.SysArticleChallanRepository.Get().Select(ob => ob.ArticleChallanNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        #endregion

	}
}