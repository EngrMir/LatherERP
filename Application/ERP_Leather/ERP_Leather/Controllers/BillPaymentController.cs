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
    public class BillPaymentController : Controller
    {
        DalSysCurrency objCurrency = new DalSysCurrency();
        DalSysPaymentType objdDalSysPaymentType = new DalSysPaymentType();
        DalSysPaymentMethod objDalSysPaymentMethod = new DalSysPaymentMethod();

        private DalPrqBillPayment Dalobject;
        private ValidationMsg _vmMsg;

        public BillPaymentController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalPrqBillPayment();
        }

        [CheckUserAccess("BillPayment/BillPayment")]
        public ActionResult BillPayment()
        {
            ViewBag.formTiltle = "Bill Payment";
            ViewBag.ddlPaymentCurrencyList = objCurrency.GetAllActiveCurrency();
            ViewBag.PaymentTypeList = objdDalSysPaymentType.GetAllPaymentTypeList();
            ViewBag.PaymentMethodList = objDalSysPaymentMethod.GetAllPaymentMethodList();
            return View();
        }

        [HttpPost]
        public ActionResult BillPayment(PrqBillPayment model)
        {
            _vmMsg = model.PaymentID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"])) : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { PaymentID = Dalobject.GetPaymentID(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult BillPaymentChecked(string paymentId, string checkComment, string recordStatus)
        {
            _vmMsg = Dalobject.BillPaymentChecked(paymentId, checkComment, recordStatus, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult BillPaymentRecommended(string paymentId, string recommendedComment, string recordStatus)
        {
            _vmMsg = Dalobject.BillPaymentRecommended(paymentId, recommendedComment, recordStatus, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult BillPaymentApproved(string paymentId, string approveComment, string recordStatus)
        {
            _vmMsg = Dalobject.BillPaymentApproved(paymentId, approveComment, recordStatus, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult GetSupplierBillPaymentAmount(PrqBillPayment model)
        {
            var billPaymentAmount = Dalobject.GetSupplierBillPaymentAmount(model);
            return Json(billPaymentAmount, JsonRequestBehavior.AllowGet);
        }

        //[Authorize()]
        public ActionResult GetSupplierList()
        {
            var supplier = Dalobject.GetSupplierList();
            return Json(supplier, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBillRefList(string supplierId)
        {
            int supid = string.IsNullOrEmpty(supplierId) ? 0 : Convert.ToInt32(supplierId);
            var challanList = Dalobject.GetBillRefList(supid);
            return Json(challanList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPayBillRefInfo(string PaymentID)
        {
            int payID = string.IsNullOrEmpty(PaymentID) ? 0 : Convert.ToInt32(PaymentID);
            var PayBillRefList = Dalobject.GetPayBillRefInfo(payID);
            return Json(PayBillRefList, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetBillPaymentInformation(string supplierid)
        //{
        //    var supid = string.IsNullOrEmpty(supplierid) ? 0 : Convert.ToInt32(supplierid);
        //    var billPaymentlist = Dalobject.GetBillPaymentList(supid);
        //    return Json(billPaymentlist, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetBillPaymentInformation()
        {
            //var supid = string.IsNullOrEmpty(supplierid) ? 0 : Convert.ToInt32(supplierid);
            var billPaymentlist = Dalobject.GetBillPaymentList();
            return Json(billPaymentlist, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletedBillPayment(string paymentId)
        {
            int paymentid = string.IsNullOrEmpty(paymentId) ? 0 : Convert.ToInt32(paymentId);
            _vmMsg = Dalobject.DeletedBillPayment(paymentid);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DeletedBillRefGridData(string SupplierBillID)
        {
            if (!string.IsNullOrEmpty(SupplierBillID))
            {
                int supplierBillId = Convert.ToInt32(SupplierBillID);
                _vmMsg = Dalobject.DeletedBillRefGridData(supplierBillId);
            }
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSupplierListForSearch()
        {
            var supplierAgentList = Dalobject.GetSupplierListForSearch();
            return Json(supplierAgentList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillPaymentListForSearch(string supplier)
        {
            PrqBillPayment model = new PrqBillPayment();
            model.BillPaymentList = Dalobject.GetBillPaymentList().Where(m => m.SupplierName.StartsWith(supplier)).ToList();
            model.Count = model.BillPaymentList.Count > 1 ? model.Count = 0 : model.Count = 1;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}