using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ERP_Leather.Controllers
{
    public class EXPBankDebitVoucherController : Controller
    {
        private DalEXPBankDebitVoucher Dalobject;
        private ValidationMsg _vmMsg;
        DalSysCurrency objCurrency = new DalSysCurrency();
        public EXPBankDebitVoucherController()
        {
            _vmMsg = new ValidationMsg();
            Dalobject = new DalEXPBankDebitVoucher();
        }
        //
       [CheckUserAccess("EXPBankDebitVoucher/EXPBankDebitVoucher")]
        public ActionResult EXPBankDebitVoucher()
        {
            ViewBag.formTiltle = "Bank Debit Voucher";
            ViewBag.ddlCurrencyList = objCurrency.GetAllActiveCurrency();
            return View();
        }
        [HttpPost]
        public ActionResult EXPBankDebitVoucher(EXPBankDebitVoucher model)
        {
            _vmMsg = model.BDVID == 0 ? Dalobject.Save(model, Convert.ToInt32(Session["UserID"]), "WetBlueProductionSchedule/WetBlueProductionSchedule") : Dalobject.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { BDVID = Dalobject.GetBDVID(), BDVNo = Dalobject.GetBDVNo(),  msg = _vmMsg });
        }
        public ActionResult GetBDVInformation()
        {
            return Json(Dalobject.GetBDVInformation(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ConfirmedEXPBDV(string BDVID)
        {
            _vmMsg = Dalobject.ConfirmedEXPBDV(BDVID, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }
        [HttpPost]
        public ActionResult CheckedEXPBDV(string BDVID)
        {
            _vmMsg = Dalobject.CheckedEXPBDV(BDVID, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }
	}
}