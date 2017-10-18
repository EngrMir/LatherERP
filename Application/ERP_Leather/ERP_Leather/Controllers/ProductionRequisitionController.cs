using System;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.BusinessLogicLayer.OperationManager;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP_Leather.ActionFilters;
using System.Collections.Generic;
using System.Linq;


namespace ERP_Leather.Controllers
{
    public class ProductionRequisitionController : Controller
    {
        DalProductionRequisition objDal = new DalProductionRequisition();
        BllProductionRequisition objBLL = new BllProductionRequisition();

        [HttpGet]
        [CheckUserAccess("ProductionRequisition/ProductionRequisition")]
        public ActionResult ProductionRequisition()
        {
            ViewBag.RequisitionFrom = objDal.GetStoreListForFixedCategory("Production");
            ViewBag.RequisitionTo = objDal.GetCategoryTypeWiseStoreList("Chemical", "Chemical");
            ViewBag.RecipeFor = objDal.GetRecipeCategoryList();
            ViewBag.UnitForLeather = objDal.GetUnitListForFixedCategory("Leather");
            ViewBag.UnitForChemical = objDal.GetUnitListForFixedCategory("ChemicalPack");
            ViewBag.UnitForThickness = objDal.GetUnitListForFixedCategory("Thickness");

            ViewBag.formTiltle = "Production Requisition";
            return View();
        }

        [HttpPost]
        [CheckUserAccess("ProductionRequisition/ProductionRequisition")]
        public ActionResult ProductionRequisition(PRDChemProdReq model)
        {
            if (model.RequisitionID == 0)
            {
                //Convert.ToInt32(Session["UserID"])
                var msg = objBLL.Save(model, 1, "ProductionRequisition/ProductionRequisition");
                var RequisitionID = objBLL.GetRequisitionID();
                var RequisitionNo = objDal.GetRequisitionNo(RequisitionID);
                var RequisitionTo = objDal.GetRequisitionToInfo(RequisitionID);
                var RequisitionItemList = objDal.GetRequisitionItemList(RequisitionID, RequisitionTo);
                return Json(new { Msg = msg, RequisitionID = RequisitionID, RequisitionNo = RequisitionNo, RequisitionItemList = RequisitionItemList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = objBLL.Update(model, 1);
                var RequisitionTo = objDal.GetRequisitionToInfo(model.RequisitionID);
                var RequisitionItemList = objDal.GetRequisitionItemList(model.RequisitionID, RequisitionTo);

                return Json(new { Msg = msg, RequisitionItemList = RequisitionItemList }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRecipeForFixedCategory(int _RecipeFor)
        {
            var Data = objDal.GetRecipeForFixedCategory(_RecipeFor);

            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRecipeItemListForFixedRecipe(int _RecipeID, byte _RequisitionTo, byte _RequisitionFrom)
        {
            var Data = objDal.GetRecipeItemListForFixedRecipe(_RecipeID, _RequisitionTo, _RequisitionFrom);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecalculateItemQuantity(int _RecipeID, decimal _Factor, byte _StoreID)
        {
            var Data = objDal.RecalculateItemQuantity(_RecipeID, _Factor, _StoreID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRequistionInfoForSearch(int pageSize, int skip)
        {
            var AllData = objDal.GetRequistionInfoForSearch();

            var filterCollection = KendoGridFilterCollection.BuildCollection(Request);
            var filteredData = AllData.MultipleFilter(filterCollection.Filters);
            var FinalData = filteredData.Skip(skip).Take(pageSize).ToList();

            foreach (var item in FinalData)
            {
                item.RequisitionCategory = DalCommon.ReturnRequisitionCategory(item.RequisitionCategory);
                item.RequisitionType = (item.RequisitionType == "UR" ? "Urgent" : "Normal");
                item.ReqRaisedOn = (Convert.ToDateTime(item.ReqRaisedOnTemp)).ToString("dd'/'MM'/'yyyy");
                item.RecordStatus = (item.RecordStatus == "NCF" ? "Not Confirmed" : "Approved");
            }

            var total = filteredData.Count();
            return Json(new { total = total, data = FinalData }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRequisitionDetailsAfterSearch(int _RequisitionID)
        {
            var Data = objDal.GetRequisitionDetailsAfterSearch(_RequisitionID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteRequisitionItem(string _RequisitionItemID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed"||_PageStatus == "CHK" || _PageStatus == "Checked")
            {
                CheckStatus = objDal.DeleteRequisitionItem(_RequisitionItemID);
                if (CheckStatus)
                {
                    return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (_PageStatus == "CNF" || _PageStatus == "Confirmed" || _PageStatus == "APV" || _PageStatus == "Approved")
            {
                return Json(new { Msg = "CNF" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteRequisition(string _RequisitionID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed" || _PageStatus == "CHK" || _PageStatus == "Checked")
            {
                CheckStatus = objDal.DeleteRequisition(_RequisitionID);
                if (CheckStatus)
                {
                    return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (_PageStatus == "CNF" || _PageStatus == "Confirmed" || _PageStatus == "APV" || _PageStatus == "Approved")
            {
                return Json(new { Msg = "CNF" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CheckRequisition(string _RequisitionID, string _CheckComment)
        {

            var Data = objDal.CheckRequisition(_RequisitionID, _CheckComment);

            if (Data)
            {
                return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ApproveRequisition(string _RequisitionID)
        {

            var Data = objDal.ApproveRequisition(_RequisitionID);

            if (Data)
            {
                return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetChemicalItemWithStock(byte _RequisitionFrom, byte _RequisitionTo)
        {
            return Json(objDal.GetChemicalItemWithStock(_RequisitionFrom, _RequisitionTo), JsonRequestBehavior.AllowGet);
        }

       
	}
}