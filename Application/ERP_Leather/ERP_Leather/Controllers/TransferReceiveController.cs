using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.OperationGateway;
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
    public class TransferReceiveController : Controller
    {
        DalSysStore objStore = new DalSysStore();
        DalInvLeatherTransferRecieve DalObj = new DalInvLeatherTransferRecieve();

        private ValidationMsg _vmMsg;

        [CheckUserAccess("TransferReceive/TransferReceive")]
        public ActionResult TransferReceive()
        {
            ViewBag.formTiltle = "Raw Hide Leather Store Transfer Receive";
            ViewBag.ddlIssueFromToList = objStore.GetAllActiveStore();
            return View();
        }

        [HttpPost]
        public ActionResult TransferReceive(InvLeatherTransferRecieve model)
        {
            _vmMsg = model.ReceiveID == 0 ? DalObj.Save(model, Convert.ToInt32(Session["UserID"])) : DalObj.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { ReceiveID = DalObj.GetReceiveID(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult TransferReceiveChecked(string receiveId, string checkComment)
        {
            _vmMsg = DalObj.TransferReceiveChecked(Convert.ToInt32(receiveId), checkComment, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult StoreTransferReceiveConfirmed(InvLeatherTransferRecieve model)
        {
            _vmMsg = DalObj.StoreTransferReceiveConfirmed(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpGet]
        public ActionResult GetIssueList()
        {
            DalInvLeatherIssue dalChallan = new DalInvLeatherIssue();
            var challanList = dalChallan.GetIssueForReiceveList();
            return Json(challanList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIssueforReceiveList(string issueid)
        {
            DalInvLeatherIssue dalChallan = new DalInvLeatherIssue();
            var challanList = dalChallan.GetIssueForReiceveList(issueid).FirstOrDefault();
            return Json(challanList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransferReceiveItemFromIssueList(string issueid)
        {
            long issid = string.IsNullOrEmpty(issueid) ? 0 : Convert.ToInt64(issueid);
            var issueItemList = DalObj.GetTransferReceiveItemFromIssueList(issid);
            return Json(issueItemList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTransferReceiveItemList(string receiveID)
        {
            long recid = string.IsNullOrEmpty(receiveID) ? 0 : Convert.ToInt64(receiveID);
            var receiveItemListList = DalObj.GetTransferReceiveItemList(recid);
            return Json(receiveItemListList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransferReceiveList()
        {
            var transferReceiveList = DalObj.GetTransferReceiveList();
            return Json(transferReceiveList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedReceive(string receiveId)
        {
            int receiveid = string.IsNullOrEmpty(receiveId) ? 0 : Convert.ToInt16(receiveId);
            _vmMsg = DalObj.DeletedReceive(receiveid);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedReceiveItem(string itemReceiveId)
        {
            int itemReceiveid = string.IsNullOrEmpty(itemReceiveId) ? 0 : Convert.ToInt16(itemReceiveId);
            _vmMsg = DalObj.DeletedReceiveItem(itemReceiveid);
            return Json(new { msg = _vmMsg });
        }
    }
}