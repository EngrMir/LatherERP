using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class ChemLocalPurcBillPayController : Controller
    {
        private DalSysCurrency _objCurrency;
        private int _userId;
        private UnitOfWork _unit;
        private ValidationMsg _validationMsg;
        private readonly BLC_DEVEntities _context;
        private DalPrqChemLocalPurcBillPay _dalPrqChemLocalPurcBillPay;
        public ChemLocalPurcBillPayController()
        {
            _objCurrency = new DalSysCurrency();
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            _context = new BLC_DEVEntities();
            _dalPrqChemLocalPurcBillPay = new DalPrqChemLocalPurcBillPay();
        }


        [CheckUserAccess("ChemLocalPurcBillPay/ChemLocalPurcBillPay")]
        public ActionResult ChemLocalPurcBillPay()
        {
            ViewBag.CurrencyList = _objCurrency.GetAllActiveCurrency();
            return View();
        }

        public ActionResult GetSupplier()
        {
            var suppliers = new List<SupplierDetails>();
            var supplierId = _unit.ChemicalLocalPurchaseBillRepository.Get().Select(ob => ob.SupplierID).Distinct().ToList();
            foreach (var i in supplierId)
            {
                var supplier = new SupplierDetails();
                var suppInfo = _unit.SysSupplierRepository.GetByID(i);
                var suppAdd = _unit.SupplierAddressRepository.Get().FirstOrDefault(ob => ob.SupplierID == 1 && ob.IsActive);
                supplier.SupplierID = suppInfo.SupplierID;
                supplier.SupplierCode = suppInfo.SupplierCode;
                supplier.SupplierName = suppInfo.SupplierName;
                if (suppAdd != null) supplier.SupplierAddressID = suppAdd.SupplierAddressID.ToString();
                if (suppAdd != null) supplier.Address = suppAdd.Address;
                if (suppAdd != null) supplier.ContactNumber = suppAdd.ContactNumber;
                if (suppAdd != null) supplier.ContactPerson = suppAdd.ContactPerson;
                suppliers.Add(supplier);
            }
            return Json(suppliers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillById(long billId)
        {
            var bill = _unit.ChemicalLocalPurchaseBillRepository.GetByID(billId);
            var result = new LocalPurchaseBillForPay
            {
                BillID = bill.BillID,
                BillNo = bill.BillNo,
                SupplierBillNo = bill.SupplierBillNo,
                BillAmt = bill.BillAmt,
                VatAmt = bill.VatAmt,
                DiscountAmt = bill.DiscountAmt,
                PayableAmt = bill.PayableAmt,
                ApprovedAmt = bill.ApprovedAmt
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillPay()
        {
            var bills = _unit.ChemicalLocalPurchaseBillPaymentRepository.Get().ToList();
            var result = bills.Select(bill => new LocalPurchaseBillPayMin
            {
                PaymentID = bill.PaymentID,
                PaymentNo = bill.PaymentNo,
                PaymentDate = string.Format("{0:dd/MM/yyyy}", bill.PaymentDate),
                SupplierID = bill.SupplierID,
                SupplierCode = bill.SupplierID == 0 ? "": _unit.SysSupplierRepository.GetByID(bill.SupplierID).SupplierCode,
                SupplierName = bill.SupplierID == 0 ? "": _unit.SysSupplierRepository.GetByID(bill.SupplierID).SupplierName,
                RecordStatus = DalCommon.ReturnRecordStatus(bill.RecordStatus)
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillPayById(long paymentId)
        {
            var bill = _unit.ChemicalLocalPurchaseBillPaymentRepository.GetByID(paymentId);
            var result = new PrqChemLocalPurcBillPay
            {
                PaymentId = bill.PaymentID,
                PaymentNo = bill.PaymentNo,
                PaymentDate = string.Format("{0:dd/MM/yyyy}", bill.PaymentDate),
                SupplierId = bill.SupplierID,
                SupplierNo = bill.SupplierID == null
                    ? ""
                    : _unit.SysSupplierRepository.GetByID(bill.SupplierID).SupplierCode,
                SupplierName = bill.SupplierID == null
                    ? ""
                    : _unit.SysSupplierRepository.GetByID(bill.SupplierID).SupplierName,
                SupplierAddressId = bill.SupplierAddressID,
                SupplierAddress = bill.SupplierAddressID == null
                    ? ""
                    : _unit.SupplierAddressRepository.GetByID(bill.SupplierAddressID).Address,
                PurchaseYear = bill.PurchaseYear,
                PaymentType = bill.PaymentType,
                PaymentMethod = bill.PaymentMethod,
                Currency = bill.Currency,
                PaymentDoc = bill.PaymentDoc,
                BillAmount = bill.BillAmount,
                VatAmount = bill.VatAmount,
                DeductAmount = bill.DeductAmount,
                PaymentAmount = bill.PaymentAmount,
                PaymentStatus = bill.PaymentStatus,
                Remarks = bill.Remarks,
                RecordStatus = bill.RecordStatus,
                CheckedBy = bill.CheckedBy,
                CheckedByName = bill.CheckedBy == null
                    ? ""
                    : _unit.SysSupplierRepository.GetByID(bill.CheckedBy).SupplierName,
                CheckComments = bill.CheckComments,
                ApprovedBy = bill.ApprovedBy,
                ApprovedByName = bill.ApprovedBy == null
                    ? ""
                    : _unit.SysSupplierRepository.GetByID(bill.ApprovedBy).SupplierName,
                ApprovalAdvice = bill.ApprovalAdvice,
                References =
                    _unit.ChemicalBillPaymentReference.Get().Where(ob => ob.PaymentID == bill.PaymentID).Select(
                        reff => new PrqChemBillPymtRef
                        {
                            BillPaymtRefID = reff.BillPaymtRefID,
                            PaymentID = reff.PaymentID,
                            BillID = reff.BillID,
                            BillNo =
                                reff.BillID == null
                                    ? ""
                                    : _unit.ChemicalLocalPurchaseBillRepository.GetByID(reff.BillID).BillNo,
                            SupplierBillNo = reff.SupplierBillNo,
                            RecordStatus = reff.RecordStatus
                        }).ToList()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteRef(long paymentId)
        {
            var delete = _dalPrqChemLocalPurcBillPay.DeleteRef(paymentId);
            return Json(delete, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(long paymentId)
        {
            var delete = _dalPrqChemLocalPurcBillPay.Delete(paymentId);
            return Json(delete, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(PrqChemLocalPurcBillPay model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalPrqChemLocalPurcBillPay.Save(model, _userId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Check(long paymentId, string comment)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalPrqChemLocalPurcBillPay.Check(paymentId, _userId, comment);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confirm(long paymentId)
        {
            _validationMsg = _dalPrqChemLocalPurcBillPay.BillConfirm(paymentId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Approve(long paymentId, string comment)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalPrqChemLocalPurcBillPay.Approve(paymentId, _userId, comment);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }
    }

    public class LocalPurchaseBillForPay
    {
        public long BillID { get; set; }
        public string BillNo { get; set; }
        public string SupplierBillNo { get; set; }
        public decimal? BillAmt { get; set; }
        public decimal? VatAmt { get; set; }
        public decimal? DiscountAmt { get; set; }
        public decimal? PayableAmt { get; set; }
        public decimal? ApprovedAmt { get; set; }
    }

    public class LocalPurchaseBillPayMin
    {
        public long PaymentID { get; set; }
        public string PaymentNo { get; set; }
        public int? SupplierID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string PaymentDate { get; set; }
        public string RecordStatus { get; set; }
    }
}