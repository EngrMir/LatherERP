using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class LeatherIssueController : Controller
    {
        DalSysStore objStore = new DalSysStore();
        private DalInvLeatherIssue DalObj;
        private ValidationMsg _vmMsg;

        public LeatherIssueController()
        {
            _vmMsg = new ValidationMsg();
            DalObj = new DalInvLeatherIssue();
        }

        [HttpGet]
        [CheckUserAccess("LeatherIssue/LeatherIssue")]
        public ActionResult LeatherIssue()
        {
            ViewBag.formTiltle = "Raw Hide Leather Issue";
            ViewBag.ddlIssueFromToList = objStore.GetAllRawHideStore();
            return View();
        }

        [HttpPost]
        public ActionResult LeatherIssue(InvLeatherIssue model)
        {
            _vmMsg = model.IssueID == 0 ? DalObj.Save(model, Convert.ToInt32(Session["UserID"])) : DalObj.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { IssueID = DalObj.GetIssueID(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult LeatherIssueChecked(string issueId, string checkComment)
        {
            _vmMsg = DalObj.LeatherIssueChecked(Convert.ToInt32(issueId),checkComment, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult LeatherIssueConfirmed(InvLeatherIssue model)
        {
            _vmMsg = DalObj.LeatherIssueConfirmed(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        public ActionResult GetSupplierList(string StoreID)
        {
            DalInvLeatherIssueItem dalLeatherIssueItem = new DalInvLeatherIssueItem();
            int storeid = string.IsNullOrEmpty(StoreID) ? 0 : Convert.ToInt16(StoreID);
            var supplierIDList = dalLeatherIssueItem.GetSupplierList(storeid);
            return Json(supplierIDList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChallanList(string SupplierID)
        {
            var challanList = DalObj.GetChallanListInfo(SupplierID);
            return Json(challanList, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetLeatherInfoList(string storeid, string supplierid)
        //{
        //    DalInvLeatherIssueItem dalChallan = new DalInvLeatherIssueItem();
        //    byte storid = Convert.ToByte(storeid);
        //    int suppliid = string.IsNullOrEmpty(supplierid) ? 0 : Convert.ToInt32(supplierid);
        //    var leatherInfoList = dalChallan.GetLeatherInfoList(storid, suppliid);
        //    return Json(leatherInfoList, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetLeatherInfoList(string ConcernStore, string SupplierID)
        {
            DalInvLeatherIssueItem dalInvLeatherIssueItem = new DalInvLeatherIssueItem();
            var packItemList = dalInvLeatherIssueItem.GetLeatherInfoList(ConcernStore, SupplierID);
            return Json(packItemList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeatherIssueList()
        {
            //int issId = string.IsNullOrEmpty(issueId) ? 0 : Convert.ToInt16(issueId);
            var issueList = DalObj.GetLeatherIssueList();
            return Json(issueList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeatherIssueItemList(string issueId, string IssueFrom)
        {
            DalInvLeatherIssueItem dalChallan = new DalInvLeatherIssueItem();
            long issueid = string.IsNullOrEmpty(issueId) ? 0 : Convert.ToInt64(issueId);
            byte storeid = Convert.ToByte(IssueFrom);
            var leatherInfoList = dalChallan.GetLeatherIssueItemList(issueid, storeid);
            return Json(leatherInfoList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIssueFromAndToList(string issuefor)
        {
            var issueFromAndToList = DalObj.GetIssueFromAndToList(issuefor);
            return Json(issueFromAndToList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedIssue(string issueId)
        {
            int issueid = string.IsNullOrEmpty(issueId) ? 0 : Convert.ToInt16(issueId);
            _vmMsg = DalObj.DeletedIssue(issueid);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedIssueItem(string itemIssueId)
        {
            int itemissueId = string.IsNullOrEmpty(itemIssueId) ? 0 : Convert.ToInt16(itemIssueId);
            _vmMsg = DalObj.DeletedIssueItem(itemissueId);
            return Json(new { msg = _vmMsg });
        }

        public JsonResult GetSupplierListForSearch()
        {
            //var supplierAgentList = DalObj.GetSupplierListForSearch(StoreID);
            var supplierAgentList = DalObj.GetSupplierListForSearch();
            return Json(supplierAgentList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBillPaymentListForSearch(string storeId, string supplier)
        {
            DalInvLeatherIssueItem dalChallan = new DalInvLeatherIssueItem();
            SysSupplier model = new SysSupplier();
            int storeid = string.IsNullOrEmpty(storeId) ? 0 : Convert.ToInt16(storeId);
            model.SupplierList = dalChallan.GetSupplierList(storeid).Where(m => m.SupplierName.StartsWith(supplier)).ToList();
            model.Count = model.SupplierList.Count > 1 ? model.Count = 0 : model.Count = 1;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}