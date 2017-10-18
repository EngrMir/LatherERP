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
    public class ExportFormController : Controller
    {
        private int _userId;
        DalSysCurrency _objCurrency;
        UnitOfWork _unit;
        ValidationMsg _validationMsg;
        private DalExportForm _dalExportForm;

        public ExportFormController()
        {
            _objCurrency = new DalSysCurrency();
            _unit = new UnitOfWork();
            _validationMsg = new ValidationMsg();
            _dalExportForm = new DalExportForm();
        }
        // GET: ExportForm
        [CheckUserAccess("ExportForm/ExportForm")]
        public ActionResult ExportForm()
        {
            ViewBag.PackUnitList = _unit.SysUnitRepository.Get();
            ViewBag.Currency = _unit.CurrencyRepository.Get();
            ViewBag.Comodity = _unit.SysComodityRepository.Get();
            return View();
        }

        public ActionResult Save(ExpExportForm model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalExportForm.Save(model, _userId, "");
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExpFormList()
        {
            var expFrms = _unit.ExportFormRepository.Get().ToList();
            var result = expFrms.Select(expFrm => new
            {
                expFrm.ExportFormID,
                expFrm.ExportFormNo,
                ExportFormDate4 = string.Format("{0:dd/MM/yyyy}", expFrm.ExportFormDate4),
                DealerName = expFrm.DealerID == null ? "" : _unit.BranchRepository.GetByID(expFrm.DealerID).BranchName,
                CINo = expFrm.CIID == null ? "" : _unit.ExpCommercialInvoiceRepository.GetByID(expFrm.CIID).CINo,
                PLNo = expFrm.PLID == null ? "" : _unit.ExpPackingListRepository.GetByID(expFrm.PLID).PLNo,
                RecordStatus = DalCommon.ReturnRecordStatus(expFrm.RecordStatus)
            }).ToList();

            return Json(result.OrderByDescending(ob => ob.ExportFormID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExportForm(long id)
        {
            var expFrm = _dalExportForm.GetExpFrm(id);
            return Json(expFrm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confirm(long id)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalExportForm.Confirm(id, _userId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Check(long id)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _validationMsg = _dalExportForm.Check(id, _userId);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(long id)
        {
            _validationMsg = _dalExportForm.Delete(id);
            return Json(_validationMsg, JsonRequestBehavior.AllowGet);
        }
    }
}