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
    public class EXPLCRetirementController : Controller
    {
        private UnitOfWork objRepository = new UnitOfWork();
        private DalEXPLCRetirement objDalEXPLCRetirement = new DalEXPLCRetirement();
        private ValidationMsg objValMssg = new ValidationMsg();
        private int _userId;


        [CheckUserAccess("EXPLCRetirement/EXPLCRetirement")]
        public ActionResult EXPLCRetirement()
        {

            ViewBag.Currency = new SelectList(objRepository.CurrencyRepository.Get(filter: ob => ob.IsActive == true), "CurrencyID", "CurrencyName");
            ViewBag.formTiltle = "Export LC Retirement";
            return View();
        }


        public ActionResult GetLCNoList()
        {
            var lcList = objDalEXPLCRetirement.GetLCNoList();
            return Json(lcList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Save(EXP_LCRetirement dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);

            objValMssg = objDalEXPLCRetirement.SaveLCM_LCRetirement(dataSet, _userId, "EXPLCRetirement/EXPLCRetirement");
            return Json(new { msg = objValMssg });
        }

        [HttpPost]
        public ActionResult Update(EXP_LCRetirement dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            objValMssg = objDalEXPLCRetirement.UpdateLCM_LCRetirement(dataSet, _userId);
            return Json(new { msg = objValMssg });
        }




        // ##################### Search Start ##############
        public ActionResult GetLcmRetirementInfo()
        {
            var listLcmRetirementInfo = from temp in objRepository.EXPLCRetirementRepository.Get() 
                                        select new 
                                                { 
                                                  LCID = temp.LCID,
                                                  LCNo = temp.LCNo,
                                                  LCRetirementID = temp.LCRetirementID,
                                                  LCRetirementNo = temp.LCRetirementNo,
                                                  LCRetirementDate = string.Format("{0:dd/MM/yyyy}", temp.LCRetirementDate),//temp.LCRetirementDate,
                                                  BillValue = temp.BillValue,
                                                  RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),
                                                }; //obLCOpeningDAL.GetLCOpeningInfo();
            return Json(listLcmRetirementInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLcmRetirementInfoByLCNo(string search)//InsuranceNo
        {
            var lcInfo = from temp in objRepository.EXPLCRetirementRepository.Get().Where(ob => ob.LCNo.StartsWith(search) || ob.LCNo == search)
                         select new { temp.LCID, temp.LCNo, temp.LCRetirementID, temp.LCRetirementNo, temp.LCRetirementDate, temp.BillValue };

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var data = objRepository.EXPLCRetirementRepository.Get().Select(ob => ob.LCNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLcmRetirementByLCRetirementID(string LCRetirementID)
        {

            int id = Convert.ToInt32(LCRetirementID);
            EXP_LCRetirement dataSet = objRepository.EXPLCRetirementRepository.GetByID(id);

            EXPLCRetirement ob = new EXPLCRetirement();

            ob.LCNo = dataSet.LCNo == null ? "" : dataSet.LCNo;//dataSet.LCNo;
            ob.LCID = Convert.ToInt32(dataSet.LCID);
            ob.LCRetirementID = dataSet.LCRetirementID;
            ob.LCRetirementNo = dataSet.LCRetirementNo;
            ob.LCRetirementDate = Convert.ToDateTime(dataSet.LCRetirementDate).ToString("dd/MM/yyyy");
            ob.BillValue = Convert.ToInt32(dataSet.BillValue);
            int lcId = Convert.ToInt32(dataSet.LCID);



            var temp = objRepository.LcmBankDebitVoucherRpository.GetByID(dataSet.LCID);
            if (temp == null)
            {
                ob.LCMargin = '0';
            }
            else
            {
                ob.LCMargin = Convert.ToDecimal(temp);
            }

            ob.LCRCurrency = (byte)dataSet.LCRCurrency;
            //ob.ExchangeRate = (byte)dataSet.ExchangeRate;

            if (ob.ExchangeRate == null)
            {
                dataSet.ExchangeRate = 0;
            }
            else
            {
                ob.ExchangeRate = (byte)dataSet.ExchangeRate;
            }
            
            
            ob.ExchangeValue = (decimal)dataSet.ExchangeValue;
            ob.ExchangeCurrency = (byte)dataSet.ExchangeCurrency;

            ob.BankCommissionAmt = (decimal)dataSet.BankCommissionAmt;
            ob.OtherCharge = (decimal)dataSet.OtherCharge;
            ob.SwiftCharge = (decimal)dataSet.SwiftCharge;
            ob.InterestPersent = (byte)dataSet.InterestPersent;
            ob.InterestAmount = (decimal)dataSet.InterestAmount;
            ob.TotalAmount = (decimal)dataSet.TotalAmount;

            ob.RecordStatus = dataSet.RecordStatus;
            ob.Remarks = dataSet.Remarks;




            return Json(ob, JsonRequestBehavior.AllowGet);

        }

        // ##################### Search END ##############

        public ActionResult DeleteLCRetirementID(int lcRetirementID)
        {
            objValMssg = objDalEXPLCRetirement.DeleteLCRetirementID(lcRetirementID);//DeletePackingList(cnfBillID);
            return Json(objValMssg, JsonRequestBehavior.AllowGet);
        }






	}
}