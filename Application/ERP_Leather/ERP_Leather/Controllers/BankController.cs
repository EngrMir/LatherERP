using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using System;
using System.Web.Mvc;
using System.Linq;
using ERP_Leather.ActionFilters;


namespace ERP_Leather.Controllers
{
    public class BankController : Controller
    {
        private UnitOfWork _unit;
        private DalSysBank _dalSysBank;
        private ValidationMsg _vmMsg;

        public BankController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysBank = new DalSysBank();
            _unit = new UnitOfWork();
        }

        [CheckUserAccess("Bank/Bank")]
        public ActionResult Bank()
        {
            ViewBag.formTiltle = "Bank Form";
            return View();
        }

        [HttpPost]
        public ActionResult Save(SysBank model)
        {
            if (model != null && model.BankID != 0)
            {
                _vmMsg = _dalSysBank.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysBank.Save(model, Convert.ToInt32(Session["UserID"]), "Bank/Bank");
            }
            return Json(new { BankID = _dalSysBank.GetBankID(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult Delete(string BankID)
        {
            _vmMsg = _dalSysBank.Delete(BankID);
            return Json(new { msg = _vmMsg });
        }

        public ActionResult DelBranch(int branchId)
        {
            _vmMsg = _dalSysBank.DelBranch(branchId);
            return Json(_vmMsg, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetBankList()
        {
            var sysBank = _dalSysBank.GetAll().OrderByDescending(s => s.BankID);
            return Json(sysBank, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBankById(int bankId)
        {
            var bank = _unit.BankRepository.GetByID(bankId);
            var result = new
            {
                bank.BankID,
                bank.BankCode,
                bank.BankCategory,
                bank.BankName,
                bank.BankType,
                bank.BankBINNo,
                bank.BankSwiftCode,
                IsActive = bank.IsActive == true ? "Active" : "Inactive",
                Branches = _unit.BranchRepository.Get().Where(ob => ob.BankID == bank.BankID).Select(branch => new
                {
                    branch.BranchID,
                    branch.BankID,
                    branch.BanchCode,
                    branch.BranchName,
                    branch.Address1,
                    branch.Address2,
                    branch.Address3,
                    branch.LCLimit,
                    branch.LCOpened,
                    branch.LCBalance,
                    branch.LCMargin,
                    branch.LCMarginUsed,
                    branch.LCMarginBalance,
                    branch.BranchSwiftCode,
                    branch.LIMLimit,
                    branch.LIMTaken,
                    branch.LIMBalance,
                    IsActive = branch.IsActive == true ? "Active" : "Inactive"
                }).ToList()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}