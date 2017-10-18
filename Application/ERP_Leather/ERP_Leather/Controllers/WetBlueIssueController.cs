using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class WetBlueIssueController : Controller
    {
        private ValidationMsg _vmMsg;
        private readonly DalWetBlueIssue _objWbIssue;
        private UnitOfWork _unit;
        private string _pageMode;
        public WetBlueIssueController()
        {
            _vmMsg = new ValidationMsg();
            _objWbIssue = new DalWetBlueIssue();
            _unit = new UnitOfWork();
        }
        [CheckUserAccess("WetBlueIssue/WetBlueIssue")]
        public ActionResult WetBlueIssue()
        {
            ViewBag.formTiltle = "Wet Blue Leather Issue From Store";
            ViewBag.ddlStoreFrom = new DalSysStore().GetAllActiveWetBlueLeatherStore();
            //ViewBag.ddlStoreTo = new DalSysStore().GetAllActiveWetBlueProductionStore();

            return View();

        }
        [HttpPost]
        public ActionResult WetBlueIssue(InvWetBlueIssue model)
        {
            _vmMsg = model != null && model.WetBlueIssueID == 0
                ? _objWbIssue.SaveData(model, Convert.ToInt32(Session["UserID"]), "WetBlueIssue/WetBlueIssue")
                : _objWbIssue.UpdateData(model, Convert.ToInt32(Session["UserID"]));

            return Json(new { msg = _vmMsg }, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult TupleDemo()
        //{
        //    var allModels = new Tuple<List<Course>,
        //    List<Faculty>, List<Student>>
        //    (_repository.GetCourses(), _repository.GetFaculties(), _repository.GetStudents()) { };
        //    return View(allModels);
        //}     
        public ActionResult GetWetBlueIssueAllInfo(long wetBlueIssueId, string issueCategory, byte storeId)
        {
            var issueInfo = _objWbIssue.GetWetBlueIssueAllInfo(wetBlueIssueId, issueCategory, storeId);
            return Json(issueInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllWetBlueIssueList(string pageMode)
        {
            var issues = _objWbIssue.GetAllWetBlueIssueList(pageMode);
            return Json(issues, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeletedWetBlueIssue(long wetBlueIssueId)
        {
            _vmMsg = _objWbIssue.DeletedWetBlueIssue(wetBlueIssueId);
            return Json(new { msg = _vmMsg });

        }
        public ActionResult DeletedWetBlueIssueRef(long wetBlueIssueRefId, string recordStatus)
        {
            _vmMsg = _objWbIssue.DeletedWetBlueIssueRef(wetBlueIssueRefId, recordStatus);
            return Json(new { msg = _vmMsg });

        }
        public ActionResult DeletedWBSIssueItem(long wBSIssueItemId, string recordStatus)
        {
            _vmMsg = _objWbIssue.DeletedWBSIssueItem(wBSIssueItemId, recordStatus);
            return Json(new { msg = _vmMsg });

        }


        public ActionResult CheckData(long wetBlueIssueId, string chkComment)
        {
            _vmMsg = _objWbIssue.CheckData(wetBlueIssueId, chkComment, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmData(long wetBlueIssueId, string cnfComment)
        {
            _vmMsg = _objWbIssue.ConfirmData(wetBlueIssueId, cnfComment, Convert.ToInt32(Session["UserID"]));

            return Json(new { msg = _vmMsg }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetAllSLSBuyerOrderOG(int buyerId)
        {
            //var result =
            //    _unit.SlsBuyerOrederRepository.Get()
            //        .Where(ob => ob.BuyerID == buyerId && ob.BuyerOrderStatus == "OG")
            //        .Select(b => new { b.BuyerOrderID, b.BuyerOrderNo, b.BuyerOrderStatus });
            //return Json(result, JsonRequestBehavior.AllowGet);

            var wbObj = _objWbIssue.GetAllSLSBuyerOrderOG(buyerId);
            return Json(wbObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWetBlueStockForCrust(byte storeId, string gradeName, string supplierName, string purchaseNo)
        {
            var wbObj = _objWbIssue.GetWetBlueStockForCrust(storeId, gradeName, supplierName, purchaseNo);
            return Json(wbObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSupplierListByStoreId(byte storeId)
        {
            var wbObj = _objWbIssue.GetSupplierListByStoreId(storeId);
            return Json(wbObj, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetSupplierListSearchById(string supplier, int storeId)
        //{
        //    supplier = supplier.ToUpper();
        //    var supplierData = _objWbIssue.GetSupplierListByStoreId(storeId).Where(ob => ob.SupplierName.StartsWith(supplier)).ToList();
        //    return Json(supplierData, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult GetItemsByBuyerRefId(long wetBlueIssueRefId, byte storeId, long wetBlueIssueId)
        {
            var wbObj = _objWbIssue.GetIssueItemsByRef(wetBlueIssueRefId, storeId, wetBlueIssueId, "");
            return Json(wbObj, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetAllActiveProductionNCrustStore(string issueCategory)
        {
            var storeList = new object();
            switch (issueCategory)
            {
                case "PROD":
                    storeList = new DalSysStore().GetAllActiveWetBlueProductionStore();
                    break;
                case "SRTF":
                    storeList = new DalSysStore().GetAllActiveWetBlueLeatherStore();
                    break;
            }
            return Json(storeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllReqBuyerItems(string requisitionId)
        {
            var wbObj = _objWbIssue.GetAllReqBuyerItems(Convert.ToInt64(requisitionId));
            return Json(wbObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetArticleInfo()
        {
            var packItemList = new DalSysArticle().GetAllActiveArticle();
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetChallanArticleInfo()
        {
            var packItemList = _objWbIssue.GetAllActiveChallanArticle();
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllRequisitionForCrust()
        {
            var data = _objWbIssue.GetAllRequisitionForCrust();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllRequisitionItemsForCrust(long requisitionId)
        {
            var wbObj = _objWbIssue.GetAllRequisitionItemsForCrust(requisitionId);
            return Json(wbObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChallanAllInfo(long articleChallanId)
        {
            var packItemList = _objWbIssue.GetChallanAllInfo(articleChallanId);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllBuyerOrder()
        {
            var result =
                _unit.SlsBuyerOrederRepository.Get()
                    .Select(b => new { b.BuyerOrderID, b.BuyerOrderNo, b.OrderNo });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}