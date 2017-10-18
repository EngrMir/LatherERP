using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class InsuranceController : Controller
    {


        private UnitOfWork objRepository = new UnitOfWork();
        private ValidationMsg objValMssg = new ValidationMsg();
        private DalInsurance _dalInsurance = new DalInsurance();
        private int _userId;


        [CheckUserAccess("Insurance/Insurances")]
        public ActionResult Insurances()
        {
            ViewBag.formTiltle = "Insurance Setup";
            return View();
        }


        // ##################### Save Start ##############
        [HttpPost]
        public ActionResult Insurances(SysBank model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            objValMssg = _dalInsurance.Save(model, _userId, "Insurance/Insurances");//, "Insurance/Insurance"
            return Json(new { msg = objValMssg, BankID = _dalInsurance.GetBankID() });
        }
        // ##################### Save End ##############

        // ##################### Update Start ##############
        [HttpPost]
        public ActionResult Update(SysBank model)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            objValMssg = _dalInsurance.Update(model, _userId);
            return Json(new { msg = objValMssg });
        }

        // ##################### Update End ##############

        // ##################### Search Start ##############
        public ActionResult GetInsuranceInfo()
        {
            var data = _dalInsurance.GetInsuranceList();

            var listInsuranceInfo = (from temp in data
                                     select new
                                     {
                                         BankID = temp.BankID,
                                         BranchID = temp.BranchID,
                                         BankCode = temp.BankCode,
                                         InsuranceCode = temp.BankCode,
                                         BranchCode = temp.BanchCode,
                                         BankName = temp.BankName,
                                         InsuranceName = temp.BankName,
                                         BranchName = temp.BranchName,
                                         BankCategory = temp.BankCategory,
                                         InsuranceCategoryName = temp.BankCategory == "INC" ? "Insurance" : "",
                                         BankType = temp.BankType,
                                         InsuranceTypeName = temp.BankType == "LOC" ? "Local" : "Foreign",
                                         IsActive = temp.IsActive = true ? "Active" : "Inactive"
                                     }).OrderBy(ob => ob.BankName);
            return Json(listInsuranceInfo, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SearchInsuranceByInsuranceNo(string search)
        {
            var lcInfo = (from temp in objRepository.BankRepository.Get().Where(ob => (ob.BankCode.StartsWith(search) || ob.BankCode == search)).AsEnumerable()
                          join temp2  in objRepository.BranchRepository.Get() on temp.BankID equals temp2.BankID
                         select new
                         {
                             BankID = temp.BankID,
                             BankCode = temp.BankCode,
                             InsuranceCode = temp.BankCode,
                             BankName = temp.BankName,

                             BranchID = temp2.BranchID,
                             BanchCode = temp2.BanchCode,
                             BranchCode = temp2.BanchCode,
                             BranchName = temp2.BranchName,

                             InsuranceName = temp.BankName,
                             BankCategory = temp.BankCategory,
                             InsuranceCategoryName = temp.BankCategory == "INC" ? "Insurance" : "",
                             BankType = temp.BankType,
                             InsuranceTypeName = temp.BankType == "LOC" ? "Local" : "Foreign",
                             IsActive = temp.IsActive == true ? "Active" : "Inactive"
                         }).OrderBy(ob => ob.BankName); 

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var data = objRepository.BankRepository.Get().Select(ob => ob.BankCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInsuranceInfoByBankID(string bankId)
        {
            var bank = objRepository.BankRepository.GetByID(Convert.ToInt32(bankId));
            var result = new
            {
                BankID = bank.BankID,
                BankCode = bank.BankCode,
                BankCategory = bank.BankCategory == "INC" ? "Insurance" : "",
                BankName = bank.BankName,
                BankType = bank.BankType == "LOC" ? "Local" : "Foreign",
                IsActive = bank.IsActive == true ? "Active" : "Inactive",
                Branches = objRepository.BranchRepository.Get().Where(ob => ob.BankID == bank.BankID).Select(branch => new
                {
                    branch.BranchID,
                    branch.BankID,
                    branch.BanchCode,
                    branch.BranchName,
                    branch.Address1,
                    branch.Address2,
                    branch.Address3,
                    branch.LCBalance,
                    IsActive = bank.IsActive == true ? "Active" : "Inactive",
                }).ToList()
            };
            return Json(result, JsonRequestBehavior.AllowGet);


        }

        // ##################### Search END ##############





        // ##################### END of Delete Grid Data ##############


        public ActionResult Delete(string bankID)
        {
            objValMssg = _dalInsurance.Delete(bankID);
            return Json(new { Msg = objValMssg });
        }



    }
}