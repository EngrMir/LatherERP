using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.BusinessLogicLayer.OperationManager;
using ERP.EntitiesModel.OperationModel;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class BankVoucherController : Controller
    {
        DalBankVoucher objDalBankVoucher = new DalBankVoucher();
        BllBankVoucher objBLL = new BllBankVoucher();

        [CheckUserAccess("BankVoucher/BankVoucher")]
        public ActionResult BankVoucher()
        {
            DalSysCurrency objCurrency = new DalSysCurrency();

            ViewBag.CurrencyList = objCurrency.GetAllActiveCurrency();

            ViewBag.formTiltle = "Bank Voucher";
            return View();
        }

        [HttpPost]
        [CheckUserAccess("BankVoucher/BankVoucher")]
        public ActionResult BankVoucher(ExpBankVoucher model)
        {
            if (model.BVID == 0)
            {

                var msg = objBLL.Save(model, Convert.ToInt32(Session["UserID"]), "BankVoucher/BankVoucher");
                var BVID = objBLL.GetBVID();
                //var TransactionNo = objDalBankVoucher.GetTransactionNo(TransactionID);

                var VouchertemList = objDalBankVoucher.GetVouchertemList(BVID);
                return Json(new { Msg = msg, BVID = BVID, VouchertemList = VouchertemList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = objBLL.Update(model, Convert.ToInt32(Session["UserID"]));

                var VouchertemList = objDalBankVoucher.GetVouchertemList(model.BVID);
                return Json(new { Msg = msg, VouchertemList = VouchertemList }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTransactionInfoForSearch()
        {
            var Data = objDalBankVoucher.GetTransactionInfoForSearch();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransactionDetailsAfterSearch(long _BVID)
        {
            var Data = objDalBankVoucher.GetVouchertemList(_BVID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalBankList()
        {
            var Data = objDalBankVoucher.GetCategoryTypeWiseBankList("BNK", "LOC");
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBranchListForSpecificBank(long _BankID)
        {
            var Data = objDalBankVoucher.GetBranchListForSpecificBank(_BankID);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCIList()
        {
            var Data = objDalBankVoucher.GetCIList();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteVoucherItem(string _BVDTLID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed" || _PageStatus == "CHK" || _PageStatus == "Checked")
            {
                CheckStatus = objDalBankVoucher.DeleteVoucherItem(_BVDTLID);
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

        public ActionResult DeleteVoucher(string _BVID, string _PageStatus)
        {
            bool CheckStatus = false; ;
            if (_PageStatus == "NCF" || _PageStatus == "Not Confirmed" || _PageStatus == "CHK" || _PageStatus == "Checked")
            {
                CheckStatus = objDalBankVoucher.DeleteVoucher(_BVID);
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

        public ActionResult ConfirmVoucher(long _BVID, string _CheckComment)
        {

            var Data = objDalBankVoucher.ConfirmVoucher(_BVID, _CheckComment);

            return Json(Data, JsonRequestBehavior.AllowGet);
        }
	}
}