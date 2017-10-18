using System;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.BusinessLogicLayer.OperationManager;
using ERP.EntitiesModel.OperationModel;
using ERP.EntitiesModel.AppSetupModel;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class CrustChallanPreparationController : Controller
    {
        BllCrustChallanPreparation objBll = new BllCrustChallanPreparation();
        DalCrustChallanPreparation objDal = new DalCrustChallanPreparation();
        
        
        [CheckUserAccess("CrustChallanPreparation/CrustChallanPreparation")]
        public ActionResult CrustChallanPreparation()
        {
            ViewBag.formTiltle = "Crust Challan Preparation";
            return View();
        }

        [HttpPost]
        [CheckUserAccess("CrustChallanPreparation/CrustChallanPreparation")]
        public ActionResult CrustChallanPreparation(SysArticleChallan model)
        {
            if (model.ArticleChallanID == 0)
            {
                var msg = objBll.Save(model, Convert.ToInt32(Session["UserID"]));
                long ArticleChallanID = objBll.GetChallanID();

                var DetailList = objDal.GetArticleDetailListAfterSave(ArticleChallanID);
                var ColorList = objDal.GetArticleColorListAfterSave(ArticleChallanID);
                return Json(new { Msg = msg, ArticleChallanID = ArticleChallanID, DetailList = DetailList, ColorList = ColorList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = objBll.Update(model, Convert.ToInt32(Session["UserID"]));
                var DetailList = objDal.GetArticleDetailListAfterSave(model.ArticleChallanID);
                var ColorList = objDal.GetArticleColorListAfterSave(model.ArticleChallanID);


                return Json(new { Msg = msg, DetailList = DetailList, ColorList = ColorList }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSearchInformation()
        {
            return Json(objDal.GetSearchInformation(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChallanDetails(string _ArticleChallanID)
        {
             var DetailList = objDal.GetArticleDetailListAfterSave(Convert.ToInt64(_ArticleChallanID));
             var ColorList = objDal.GetArticleColorListAfterSave(Convert.ToInt64(_ArticleChallanID));
             return Json(new { DetailList = DetailList, ColorList = ColorList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteColorItem(string _ArticleChallanIDColor, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDal.DeleteColorItem(_ArticleChallanIDColor);
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


        public ActionResult DeleteArticleDetail(string _ArticleChallanDtlID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDal.DeleteArticleDetail(_ArticleChallanDtlID);
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

        public ActionResult DeleteChallan(string _ArticleChallanID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed")
            {
                CheckStatus = objDal.DeleteChallan(_ArticleChallanID);
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

        public ActionResult ConfirmArticleChallan(string _ArticleChallanID, string _ConfirmNote)
        {
            bool checkConfirm = objDal.ConfirmArticleChallan(_ArticleChallanID, _ConfirmNote);

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