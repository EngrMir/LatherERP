using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.BusinessLogicLayer.OperationManager;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class StockAdjustController : Controller
    {
        private DalInvLeatherStockAdjustRequest _objStockAdjustRequest;
        private ValidationMsg _vmMsg;

        DalSysStore objReceiveStore = new DalSysStore();
        DalSysLeatherType objLeatherType = new DalSysLeatherType();
        DalInvLeatherStockAdjustItem objDalAdjustItem = new DalInvLeatherStockAdjustItem();
        BllStockAdjust objBLLStockAdjust = new BllStockAdjust();
        


        public StockAdjustController()
        {
            _objStockAdjustRequest=new DalInvLeatherStockAdjustRequest();
            _vmMsg = new ValidationMsg();
        }

        [CheckUserAccess("StockAdjust/StockAdjust")]
        public ActionResult StockAdjust()
        {
            ViewBag.LeatherType = objLeatherType.GetAllActiveLeatherType();
            ViewBag.ReceiveStore = objReceiveStore.GetAllActiveStore();
            ViewBag.formTiltle = "Stock Adjustment Request";
            return View();
        }

        [HttpPost]
        public ActionResult StockAdjust(InvLeatherStockAdjustModel model)
        {
            if (model.RequestID == null)
            {
                var msg = objBLLStockAdjust.SaveAdjustItemRequest(model, Convert.ToInt32(Session["UserID"]));
                var requestID = objBLLStockAdjust.GetRequestID();
                var requestDetails = objDalAdjustItem.GetRequestDetails((requestID).ToString());

                return Json(new { Msg = msg, requestID = requestID, requestDetails = requestDetails }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = objBLLStockAdjust.UpdateAdjustItemRequest(model, Convert.ToInt32(Session["UserID"]));
                var requestDetails = objDalAdjustItem.GetRequestDetails(model.RequestID);

                return Json(new { Msg = msg, requestDetails = requestDetails }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult UpdateAdjustmentRequestWithItemValue(InvLeatherStockAdjustModel model)
        {
            var msg = objBLLStockAdjust.UpdateAdjustmentRequestWithItemValue(model);
            var requestDetails = objDalAdjustItem.GetRequestDetails(model.RequestID);

            return Json(new { Msg = msg, requestDetails = requestDetails }, JsonRequestBehavior.AllowGet);
        }

        // To Find Supplier list according to LeatherType & ReceiveStore
        [HttpGet]
        public ActionResult FindSupplierForAdjust(byte leatherType, byte receiveStore)
        {
            DalInvLeatherStockAdjustItem objStockAdjustItem= new DalInvLeatherStockAdjustItem();
            var AllSupplier = objStockAdjustItem.FindSupplierForAdjust(leatherType, receiveStore);

            return Json(AllSupplier, JsonRequestBehavior.AllowGet);
        }


        // To Find Challan list according to LeatherType & ReceiveStore
        [HttpGet]
        public ActionResult FindChallanForAdjust(byte leatherType, byte receiveStore, Int32 supplierID)
        {
            DalInvLeatherStockAdjustItem objStockAdjustItem = new DalInvLeatherStockAdjustItem();
            var AllChallan = objStockAdjustItem.FindChallanForAdjust(leatherType, receiveStore, supplierID);

            return Json(AllChallan, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmAdjustRequest(string requestID, string confirmComment)
        {
            DalInvLeatherStockAdjustItem objItem = new DalInvLeatherStockAdjustItem();
            bool checkConfirm = objItem.ConfirmAdjustRequest(requestID, confirmComment, Convert.ToInt32(Session["UserID"]));
            if(checkConfirm)
            {
                return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ApproveAdjustRequest(string requestID, string approvalComment )
        {
            DalInvLeatherStockAdjustItem objItem = new DalInvLeatherStockAdjustItem();
            bool checkApprove = objItem.ApproveAdjustRequest(requestID, approvalComment, Convert.ToInt32(Session["UserID"]));
            if (checkApprove)
            {
                return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult StockAdjustApprove()
        {
            ViewBag.LeatherType = objLeatherType.GetAllActiveLeatherType();
            ViewBag.ReceiveStore = objReceiveStore.GetAllActiveStore();
            ViewBag.formTiltle = "Stock Adjustment Approval";
            return View();
        }

        public ActionResult GetAllNCFRequestList()
        {
            DalInvLeatherStockAdjustItem objAdjustmentItem = new DalInvLeatherStockAdjustItem();
            var AllRequest = objAdjustmentItem.GetAllNCFRequestList();

            return Json(AllRequest, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllCNFRequestList()
        {
            DalInvLeatherStockAdjustItem objAdjustmentItem = new DalInvLeatherStockAdjustItem();
            var AllRequest = objAdjustmentItem.GetAllCNFRequestList();

            return Json(AllRequest, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRequestDetails(string RequestID)
        {
            DalInvLeatherStockAdjustItem objAdjustmentItem = new DalInvLeatherStockAdjustItem();

            var RequestDetails = objAdjustmentItem.GetRequestDetails(RequestID);
            return Json(RequestDetails, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteAdjustItem(string AdjustID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed" )
            {
                CheckStatus = objDalAdjustItem.DeleteAdjustItem(AdjustID);
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

        public ActionResult DeleteAdjustmentRequest(string RequestID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF")
            {
                CheckStatus = objDalAdjustItem.DeleteAdjustmentRequest(RequestID);
                if (CheckStatus)
                {
                    return Json(new { Msg = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (_PageStatus == "CNF")
            {
                return Json(new { Msg = "CNF" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Msg = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
	}
}