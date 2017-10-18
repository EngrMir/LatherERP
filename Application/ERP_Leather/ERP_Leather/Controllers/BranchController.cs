using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using System;
using System.Web.Mvc;
using System.Linq;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class BranchController : Controller
    {
        DalSysBank objDalSysBank = new DalSysBank();

        private DalSysBranch _dalSysBranch;
        private ValidationMsg _vmMsg;

        public BranchController()
        {
            _vmMsg = new ValidationMsg();
            _dalSysBranch = new DalSysBranch();
        }
        [CheckUserAccess("Branch/Branch")]
        public ActionResult Branch()
        {
            ViewBag.formTiltle = "Branch Form";
            ViewBag.ddlBankNameList = objDalSysBank.GetAllActiveBank();
            return View();
        }

        [HttpPost]
        public ActionResult Branch(SysBranch model)
        {
            if (model != null && model.BranchID != 0)
            {
                _vmMsg = _dalSysBranch.Update(model, Convert.ToInt32(Session["UserID"]));
            }
            else
            {
                _vmMsg = _dalSysBranch.Save(model, Convert.ToInt32(Session["UserID"]));
            }
            return Json(new { BranchID = _dalSysBranch.GetBranchID(), msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult Delete(string BranchID)
        {
            _vmMsg = _dalSysBranch.Delete(BranchID);
            return Json(new { msg = _vmMsg });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetBranchList()
        {
            var sysBranch = _dalSysBranch.GetAll().OrderByDescending(s => s.BranchID);
            return Json(sysBranch, JsonRequestBehavior.AllowGet);
        }
	}
}