using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;

namespace ERP_Leather.Controllers
{
    public class DeliveryChallanController : Controller
    {

        private UnitOfWork objRepository = new UnitOfWork();
        private DalDeliveryChallan objDalDeliveryChallan = new DalDeliveryChallan();
        private ValidationMsg objValMssg = new ValidationMsg();
        private int _userId;

        [CheckUserAccess("DeliveryChallan/DeliveryChallan")]
        public ActionResult DeliveryChallan()
        {
            ViewBag.formTiltle = "Delivery Challan";
            return View();
        }

        public ActionResult GetCIList()
        {
            var CIList = objDalDeliveryChallan.GetCIList();
            return Json(CIList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPLList()
        {
            var PLList = objDalDeliveryChallan.GetPLList();
            return Json(PLList, JsonRequestBehavior.AllowGet);
        }



        // ##################### Save & Update Start ##############
        #region Start EXPCnfBill Save , Update & DELETE
        [HttpPost]
        public ActionResult Save(EXPDeliveryChallan dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);

            objValMssg = objDalDeliveryChallan.Save(dataSet, _userId);
            return Json(new { msg = objValMssg });
        }

        [HttpPost]
        public ActionResult Update(EXPDeliveryChallan dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            objValMssg = objDalDeliveryChallan.Update(dataSet, _userId);
            return Json(new { msg = objValMssg });
        }

        [HttpGet]
        public ActionResult Delete(long deliverChallanID)
        {
            objValMssg = objDalDeliveryChallan.Delete(deliverChallanID);
            return Json(new { Msg = objValMssg });
        }
        #endregion
        // ##################### Save & Update End ##############

        // ##################### Search Start ##############
        public ActionResult GetInsuranceInfo()
        {
            var listLcmLimInfo = (from temp in objRepository.EXPDeliveryChallanRepository.Get().AsEnumerable()
                                  //join temp2 in objRepository.EXPDeliveryChallanCIRepository.Get() on temp.DeliverChallanID equals temp2.DeliverChallanID
                                  //join temp3 in objRepository.ExpCommercialInvoiceRepository.Get() on temp2.CIID equals temp3.CIID
                                  //join temp4 in objRepository.ExpPackingListRepository.Get() on temp2.PLID equals temp4.PLID
                                  select new
                                  {
                                      DeliverChallanID = temp.DeliverChallanID,
                                      DeliverChallanNo = temp.DeliverChallanNo,
                                      DeliverChallanDate = Convert.ToDateTime(temp.DeliverChallanDate).ToString("dd/MM/yyyy"),//temp.DeliverChallanDate,
                                      TruckNo = temp.TruckNo,
                                      DeliveryChallanNote = temp.DeliveryChallanNote,

                                      //CIID = temp2.CIID,
                                      //CINo = temp3.CINo,
                                      //CIAmount = temp3.CIAmount,
                                      //CIDate = Convert.ToDateTime(temp3.CIDate).ToString("dd/MM/yyyy"),
                                      //PLID = temp2.PLID,
                                      //PLNo = temp4.PLNo,
                                      //PLDate = Convert.ToDateTime(temp4.PLDate).ToString("dd/MM/yyyy"),
                                      RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),

                                  }).OrderByDescending(ob => ob.DeliverChallanID); //obLCOpeningDAL.GetLCOpeningInfo();
            return Json(listLcmLimInfo, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SearchInsuranceByInsuranceNo(string search)
        {
            var lcInfo = (from temp in objRepository.EXPDeliveryChallanRepository.Get().Where(ob => (ob.DeliverChallanNo.StartsWith(search) || ob.DeliverChallanNo == search)).AsEnumerable()
                          //join temp2 in objRepository.EXPDeliveryChallanCIRepository.Get() on temp.DeliverChallanID equals temp2.DeliverChallanID
                          //join temp3 in objRepository.ExpCommercialInvoiceRepository.Get() on temp2.CIID equals temp3.CIID
                          //join temp4 in objRepository.ExpPackingListRepository.Get() on temp2.PLID equals temp4.PLID


                          select new
                          {
                              DeliverChallanID = temp.DeliverChallanID,
                              DeliverChallanNo = temp.DeliverChallanNo,
                              DeliverChallanDate = Convert.ToDateTime(temp.DeliverChallanDate).ToString("dd/MM/yyyy"),//temp.DeliverChallanDate,
                              TruckNo = temp.TruckNo,
                              DeliveryChallanNote = temp.DeliveryChallanNote,

                              //CIID = temp2.CIID,
                              //CINo = temp3.CINo,
                              //CIAmount = temp3.CIAmount,
                              //CIDate = Convert.ToDateTime(temp3.CIDate).ToString("dd/MM/yyyy"),
                              //PLID = temp2.PLID,
                              //PLNo = temp4.PLNo,
                              //PLDate = Convert.ToDateTime(temp4.PLDate).ToString("dd/MM/yyyy"),
                              RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),

                          }).OrderByDescending(ob => ob.DeliverChallanID); //obLCOpeningDAL.GetLCOpeningInfo();

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var data = objRepository.EXPDeliveryChallanRepository.Get().Select(ob => ob.DeliverChallanNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInsuranceInfoByBankID(string DeliverChallanID)
        {
            var searchIssueDetails = objDalDeliveryChallan.SearchDeliveryChallanCI(DeliverChallanID);
            return Json(searchIssueDetails, JsonRequestBehavior.AllowGet);


        }

        // ##################### Search END ##############


        #region Confirm Data & Update Stock
        [HttpPost]
        public ActionResult ConfirmRecordStatus(string deliverChallanID)
        {
            objValMssg = objDalDeliveryChallan.ConfirmDeliveryChallan(Convert.ToInt32(Session["UserID"]), deliverChallanID);
            return Json(new { msg = objValMssg });
        }
        #endregion










    }
}





















//using System;
//using System.Collections.Generic;
//using System.Data.Entity.Validation;
//using System.Linq;
//using System.Text;
//using System.Web;
//using System.Web.Mvc;
//using ERP.DatabaseAccessLayer.DB;
//using ERP.DatabaseAccessLayer.OperationGateway;
//using ERP.DatabaseAccessLayer.Utility;
//using ERP.EntitiesModel.AppSetupModel;
//using ERP.EntitiesModel.BaseModel;
//using ERP.EntitiesModel.OperationModel;
//using ERP_Leather.ActionFilters;

//namespace ERP_Leather.Controllers
//{
//    public class DeliveryChallanController : Controller
//    {

//        private UnitOfWork objRepository = new UnitOfWork();
//        private DalDeliveryChallan objDalDeliveryChallan = new DalDeliveryChallan();
//        private ValidationMsg objValMssg = new ValidationMsg();
//        private int _userId;


//        public ActionResult DeliveryChallan()
//        {
//            ViewBag.formTiltle = "Delivery Challan";
//            return View();
//        }

//        public ActionResult GetCIList()
//        {
//            var CIList = objDalDeliveryChallan.GetCIList();
//            return Json(CIList, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult GetPLList()
//        {
//            var PLList = objDalDeliveryChallan.GetPLList();
//            return Json(PLList, JsonRequestBehavior.AllowGet);
//        }



//        // ##################### Save & Update Start ##############
//        #region Start EXPCnfBill Save & Update
//        //[HttpPost]
//        //public ActionResult Save(EXPDeliveryChallan dataSet)
//        //{
//        //    _userId = Convert.ToInt32(Session["UserID"]);

//        //    objValMssg = objDalDeliveryChallan.Save(dataSet, _userId);
//        //    return Json(new { msg = objValMssg });
//        //}

//        //[HttpPost]
//        //public ActionResult Update(EXPDeliveryChallan dataSet)
//        //{
//        //    _userId = Convert.ToInt32(Session["UserID"]);
//        //    objValMssg = objDalDeliveryChallan.Update(dataSet, _userId);
//        //    return Json(new { msg = objValMssg });
//        //}
//        #endregion
//        // ##################### Save & Update End ##############


//        // ##################### Search Start ##############
//        //public ActionResult GetDeliveryInfo()
//        //{
//        //    var listLcmLimInfo = (from temp in objRepository.EXPDeliveryChallanRepository.Get().AsEnumerable()
//        //                          join temp2 in objRepository.EXPDeliveryChallanCIRepository.Get() on temp.DeliverChallanID equals temp2.DeliverChallanID
//        //                          join temp3 in objRepository.ExpCommercialInvoiceRepository.Get() on temp2.CIID equals temp3.CIID
//        //                          join temp4 in objRepository.ExpPackingListRepository.Get() on temp2.PLID equals temp4.PLID
//        //                          select new
//        //                          {
//        //                              LCID = temp.LCID,
//        //                              LCNo = temp.LCNo,
//        //                              PIID = temp.PIID,
//        //                              PINo = temp4.PINo,
//        //                              LCDate = Convert.ToDateTime(temp.LCDate).ToString("dd/MM/yyyy"),
//        //                              PIDate = Convert.ToDateTime(temp4.PIDate).ToString("dd/MM/yyyy"),
//        //                              LCAmount = temp.LCAmount,
//        //                              LCOpeningBank = temp.LCOpeningBank,
//        //                              BankID = temp2.BankID,
//        //                              BankCode = temp2.BankCode,
//        //                              BankName = temp2.BankName,
//        //                              //AdvisingBankName = temp3.BranchName,
//        //                              BranchID = temp3.BranchID,
//        //                              BranchCode = temp3.BanchCode,
//        //                              Address = temp3.Address1,
//        //                              LCLimit = temp3.LCLimit,
//        //                              BranchSwiftCode = temp3.BranchSwiftCode,
//        //                              //BranchName = temp3.BranchName,
//        //                              AdvisingBankName = temp3.BranchName,
//        //                              RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),

//        //                          }).OrderByDescending(ob => ob.LCID); //obLCOpeningDAL.GetLCOpeningInfo();

//        //    return Json(listLcmLimInfo, JsonRequestBehavior.AllowGet);
//        //}


//        //public ActionResult SearchInsuranceByInsuranceNo(string search)
//        //{
//        //    var lcInfo = (from temp in objRepository.BankRepository.Get().Where(ob => (ob.BankCode.StartsWith(search) || ob.BankCode == search)).AsEnumerable()
//        //                  join temp2 in objRepository.BranchRepository.Get() on temp.BankID equals temp2.BankID
//        //                  select new
//        //                  {
//        //                      BankID = temp.BankID,
//        //                      BankCode = temp.BankCode,
//        //                      InsuranceCode = temp.BankCode,
//        //                      BankName = temp.BankName,

//        //                      BranchID = temp2.BranchID,
//        //                      BanchCode = temp2.BanchCode,
//        //                      BranchCode = temp2.BanchCode,
//        //                      BranchName = temp2.BranchName,

//        //                      InsuranceName = temp.BankName,
//        //                      BankCategory = temp.BankCategory,
//        //                      InsuranceCategoryName = temp.BankCategory == "INC" ? "Insurance" : "",
//        //                      BankType = temp.BankType,
//        //                      InsuranceTypeName = temp.BankType == "LOC" ? "Local" : "Foreign",
//        //                      IsActive = temp.IsActive == true ? "Active" : "Inactive"
//        //                  }).OrderBy(ob => ob.BankName);

//        //    return Json(lcInfo, JsonRequestBehavior.AllowGet);
//        //}

//        //public ActionResult GetAutoCompleteData()
//        //{
//        //    var data = objRepository.BankRepository.Get().Select(ob => ob.BankCode);
//        //    return Json(data, JsonRequestBehavior.AllowGet);
//        //}

//        //public ActionResult GetInsuranceInfoByBankID(string bankId)
//        //{
//        //    var bank = objRepository.BankRepository.GetByID(Convert.ToInt32(bankId));
//        //    var result = new
//        //    {
//        //        BankID = bank.BankID,
//        //        BankCode = bank.BankCode,
//        //        BankCategory = bank.BankCategory == "INC" ? "Insurance" : "",
//        //        BankName = bank.BankName,
//        //        BankType = bank.BankType == "LOC" ? "Local" : "Foreign",
//        //        IsActive = bank.IsActive == true ? "Active" : "Inactive",
//        //        Branches = objRepository.BranchRepository.Get().Where(ob => ob.BankID == bank.BankID).Select(branch => new
//        //        {
//        //            branch.BranchID,
//        //            branch.BankID,
//        //            branch.BanchCode,
//        //            branch.BranchName,
//        //            branch.Address1,
//        //            branch.Address2,
//        //            branch.Address3,
//        //            branch.LCBalance,
//        //            IsActive = bank.IsActive == true ? "Active" : "Inactive",
//        //        }).ToList()
//        //    };
//        //    return Json(result, JsonRequestBehavior.AllowGet);


//        //}

//        // ##################### Search END ##############



//    }
//}