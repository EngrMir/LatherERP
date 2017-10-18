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
    public class BankDebitVoucherController : Controller
    {
        private UnitOfWork repo;
        private ValidationMsg _vmMsg;
        private SysCommonUtility _utility;
        private DalBankDebitVoucher _dalBDV;
        public BankDebitVoucherController()
        {
            _vmMsg = new ValidationMsg();
             repo = new UnitOfWork();
             _utility = new SysCommonUtility();
             _dalBDV = new DalBankDebitVoucher();
        }

         [CheckUserAccess("BankDebitVoucher/Index")]
        public ActionResult Index()
        {           
            ViewBag.Currency = new SelectList(repo.CurrencyRepository.Get(filter: ob => ob.IsActive == true), "CurrencyID", "CurrencyName");
            return View();
        }
        public ActionResult GetBankDebitVoucherInfo()
        {
            var result = _dalBDV.GetBankDebitVoucherInfo();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchBankDebitVoucherByBDVNo(string search)
        {
            var result = _dalBDV.SearchBankDebitVoucherByBDVNo(search);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var search = repo.LcmBankDebitVoucherRpository.Get(orderBy: q => q.OrderByDescending(d => d.BDVID)).Select(ob => ob.BDVNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult Save(LcmBankDebitVoucher dataSet)
        {
            ValidationMsg _vmMsg = _dalBDV.Save(dataSet);
            return Json(new { msg = _vmMsg });
        }

        [HttpPost] 
        public ActionResult Update(LcmBankDebitVoucher dataSet)
        {
            ValidationMsg _vmMsg = _dalBDV.Update(dataSet);
            return Json(new { msg = _vmMsg });
        }
       


        public ActionResult GetBankDebitVoucherByBDVID(string BDVID)
        {
            var result = _dalBDV.GetBankDebitVoucherByBDVID(BDVID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRecordStatus(string note, string BDVID)
        {
            ValidationMsg _vmMsg = _dalBDV.UpdateRecordStatus(note, BDVID);
            return Json(new { msg = _vmMsg });
        }

        public ActionResult ConfirmRecordStatus(string BDVID)
        {
            ValidationMsg msg = _dalBDV.ConfirmRecordStatus(BDVID);
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public bool IsValidVoucherNo(string no)
        {
            return _utility.IsValid("LCM_BankDebitVoucher", "BDVNo", no, "str");
        }
	}
}