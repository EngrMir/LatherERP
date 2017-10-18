using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Text;

namespace ERP_Leather.Controllers
{
    public class EXPLCOpeningController : Controller
    {

        DalSysStore objDalStore = new DalSysStore();

        private DalEXPLCOpening objDalExpLCOpening;
        private DalExportPI objExpPI;
        private ValidationMsg _vmMsg;
        private UnitOfWork repository;
        private int _userId;



        public EXPLCOpeningController()
        {
            _vmMsg = new ValidationMsg();
            objDalExpLCOpening = new DalEXPLCOpening();
            objExpPI = new DalExportPI();
            repository = new UnitOfWork();
        }

        [CheckUserAccess("EXPLCOpening/EXPLCOpening")]
        public ActionResult EXPLCOpening()
        {
            ViewBag.formTitle = "Export LC Opening";
            ViewBag.Currency = new SelectList(repository.CurrencyRepository.Get(filter: ob => ob.IsActive == true), "CurrencyID", "CurrencyName");
            return View();
        }


        #region Start EXPCnfBill Save & Update
        [HttpPost]
        public ActionResult Save(EXPLCOpening dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);

            _vmMsg = objDalExpLCOpening.Save(dataSet, _userId);
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult Update(EXP_LCOpening dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            _vmMsg = objDalExpLCOpening.Update(dataSet, _userId);
            return Json(new { msg = _vmMsg });
        }
        #endregion



        //********************** PI INFORMATION **************************//
        public ActionResult GetPIInfo()
        {
            var piInfo = objDalExpLCOpening.GetPIInfo();
            return Json(piInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPISearch()
        {
            var benificiaryInfo = repository.ExpLeatherPI.Get().Select(ob => ob.PINo);
            return Json(benificiaryInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchPIByPI(string beni)
        {
            var piInfo = repository.ExpLeatherPI.Get().Where(ob => ob.PINo.StartsWith(beni) || ob.PINo == beni);
            List<EXPLeatherPI> lstPI = new List<EXPLeatherPI>();
            foreach (var item in piInfo)
            {
                EXPLeatherPI obPI = new EXPLeatherPI();
                obPI.PIID = item.PIID;
                obPI.PINo = item.PINo;
                obPI.PIDate = Convert.ToDateTime(item.PIDate).ToString("dd/MM/yyyy");
                obPI.BeneficiaryAddressID = item.BeneficiaryAddressID;
                obPI.PIBeneficiary = item.PIBeneficiary;

                lstPI.Add(obPI);
            }
            return Json(lstPI, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPIGrandTotalSFT(long  _PIID)
        {
            var piInfo = objExpPI.GetGrandTotalSFT(_PIID);
            return Json(piInfo, JsonRequestBehavior.AllowGet);
        }

        //********************** END OF PI INFORMATION **************************//

        //**********************LC OPENING BANK **************************//
        public ActionResult GetLCBankList(string bankCategory, string bankType)
        {
            var lstBank = objDalExpLCOpening.GetLCBankList(bankCategory, bankType);
            return Json(lstBank, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAdvisingBankList(string bankCategory, string bankType)
        {
            var lstBank = objDalExpLCOpening.GetAdvisingBankList(bankCategory, bankType);
            return Json(lstBank, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchLCBankByBank(string bank, string bankCategory, string bankType)
        {
            bank = bank.ToUpper();
            var lstBank = objDalExpLCOpening.GetLCBankList(bankCategory, bankType);
            var result = from temp in lstBank where (temp.BankName.ToUpper().StartsWith(bank) || temp.BankName.ToUpper() == bank) select temp;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLCBankNameList(string bankCategory, string bankType)
        {
            var lstBank = repository.BankRepository.Get(filter: ob => ob.BankCategory == bankCategory && ob.BankType == bankType && ob.IsActive == true).Select(ob => ob.BankName);
            return Json(lstBank, JsonRequestBehavior.AllowGet);
        }

        //********************** END OF LC OPENING BANK **************************//

        //**********************PI DETAILS INFO **************************//
        public string GetPIDetailsInfo(string piNo)
        {
            StringBuilder sb = new StringBuilder();
            var lstBank = objDalExpLCOpening.GetPIDetailsInfo(piNo);
            foreach (var item in lstBank)
            {
                sb.Append("<table class='table table-bordered'>");

                sb.Append("<tr><td>PI No</td><td>");
                sb.Append("'" + piNo + "'");
                sb.Append("</td><td>PI Category</td><td>");

                sb.Append("'" + item.PICategory + "'");
                sb.Append(" </td></tr>");

                sb.Append("<tr><td>PI Date</td><td>");
                sb.Append("'" + item.PIDate + "'");
                sb.Append("</td><td>PI Currency Name</td><td>");

                sb.Append("'" + item.PICurrencyName + "'");
                sb.Append(" </td></tr>");

                sb.Append("<tr><td>Local Agent Name</td><td>");
                sb.Append("'" + item.LocalAgentName + "'");
                sb.Append("</td><td>Foreign Agent name</td><td>");

                sb.Append("'" + item.ForeignAgentName + "'");
                sb.Append(" </td></tr>");



                sb.Append("<tr><td>Buyer Bank Name</td><td>");
                sb.Append("'" + item.BuyerBankName + "'");
                sb.Append("</td><td>Buyer Bank Acc.</td><td>");

                sb.Append("'" + item.BuyerBankAccount + "'");
                sb.Append(" </td></tr>");


                sb.Append("<tr><td>Seller Bank Name</td><td>");
                sb.Append("'" + item.SellerBankName + "'");
                sb.Append("</td><td>Seller Bank Acc.</td><td>");

                sb.Append("'" + item.BankAccount + "'");
                sb.Append(" </td></tr>");




                sb.Append("<tr><td>Payment Mode</td><td>");
                sb.Append("'" + item.PaymentMode + "'");
                sb.Append("</td><td>Port of Loading</td><td>");

                sb.Append("'" + item.LoadingPortName + "'");
                sb.Append(" </td></tr>");




                sb.Append("<tr><td>Port of Discharge</td><td>");
                sb.Append("'" + item.DischargePortName + "'");


                sb.Append("<tr><td>Offer Validity Days</td><td>");
                sb.Append("'" + item.OfferValidityDays + "'");
                
                sb.Append(" </td></tr>   </table>");


            }
            return sb.ToString();
        }


        //**********************END OF PI DETAILS INFO **************************//



        // ##################### Search Start ##############
        public ActionResult GetLCInfo()
        {
            var listLcmLimInfo = (from temp in repository.ExpLCOpening.Get().AsEnumerable()
                                  join temp2 in repository.CurrencyRepository.Get() on temp.LCCurrencyID equals temp2.CurrencyID
                                  //join temp3 in repository.BankRepository.Get() on temp2.BankID equals temp3.BankID
                                  join temp4 in repository.ExpLeatherPI.Get() on temp.PIID equals temp4.PIID
                                  select new
                                  {
                                      LCID = temp.LCID,
                                      LCNo = temp.LCNo,
                                      LCDate = Convert.ToDateTime(temp.LCDate).ToString("dd/MM/yyyy"),
                                      LastShipmentDate = Convert.ToDateTime(temp.LastShipmentDate).ToString("dd/MM/yyyy"),
                                      ExpiryDate = Convert.ToDateTime(temp.ExpiryDate).ToString("dd/MM/yyyy"),
                                      LCAmount = temp.LCAmount,
                                      LCCurrencyID = temp.LCCurrencyID,
                                      PICurrencyName = temp2.CurrencyName,
                                      PIID = temp.PIID,
                                      PINo = temp4.PINo,
                                      
                                      PIDate = Convert.ToDateTime(temp4.PIDate).ToString("dd/MM/yyyy"),
                                      Remarks = temp.Remarks,
                                      RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus)
                                      //LCOpeningBank = temp.LCOpeningBank,
                                      //BankName = temp2.BranchName,
                                      //BankID = temp3.BankID,
                                      //BankCode = temp3.BankCode,
                                      //AdvisingBank = temp.AdvisingBank,
                                      //AdvisingBankName = temp3.BankName,
                                      //BranchID = temp2.BranchID,
                                      //BranchCode = temp2.BanchCode,

                                      //Address = temp2.Address1,
                                      //LCLimit = temp2.LCLimit,
                                      //BranchSwiftCode = temp2.BranchSwiftCode,


                                  }).OrderByDescending(ob => ob.LCID); //obLCOpeningDAL.GetLCOpeningInfo();
            return Json(listLcmLimInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLCInfoByLCNo(string search)//InsuranceNo
        {
            var lcInfo = (from temp in repository.ExpLCOpening.Get().Where(ob => ob.LCNo.StartsWith(search) || ob.LCNo == search).AsEnumerable()
                          join temp2 in repository.BankRepository.Get() on temp.LCOpeningBank equals temp2.BankID
                          join temp3 in repository.BranchRepository.Get() on temp.AdvisingBank equals temp3.BranchID
                          join temp4 in repository.ExpLeatherPI.Get() on temp.PIID equals temp4.PIID


                          select new
                          {
                              LCID = temp.LCID,
                              LCNo = temp.LCNo,
                              PIID = temp.PIID,
                              PINo = temp4.PINo,
                              LCDate = Convert.ToDateTime(temp.LCDate).ToString("dd/MM/yyyy"),
                              LCAmount = temp.LCAmount,
                              LCOpeningBank = temp.LCOpeningBank,
                              BankID = temp2.BankID,
                              BankCode = temp2.BankCode,
                              BankName = temp2.BankName,
                              //AdvisingBankName = temp3.BranchName,
                              BranchID = temp3.BranchID,
                              BanchCode = temp3.BanchCode,
                              BranchName = temp3.BranchName,
                              AdvisingBankName = temp3.BranchName,
                              RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),
                          }).OrderByDescending(ob => ob.LCID);

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var data = repository.ExpLCOpening.Get().Select(ob => ob.LCNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLCDetailInfoByLCID(string LCID)
        {
            int id = Convert.ToInt32(LCID);
            EXP_LCOpening dataSet = repository.ExpLCOpening.GetByID(id);

            EXPLCOpening ob = new EXPLCOpening();

            ob.LCNo = dataSet.LCNo == null ? "" : dataSet.LCNo;
            ob.LCID = Convert.ToInt32(dataSet.LCID);
            ob.LCDate = Convert.ToDateTime(dataSet.LCDate).ToString("dd/MM/yyyy");
            ob.LCAmount = (decimal)dataSet.LCAmount == null ? 0 : (decimal)dataSet.LCAmount;
            ob.LCCurrencyID = (byte)dataSet.LCCurrencyID;

            ob.PIID = dataSet.PIID;
            ob.PINo = repository.ExpLeatherPI.GetByID(dataSet.PIID == null ? 0 : dataSet.PIID).PINo;

            ob.LastShipmentDate = Convert.ToDateTime(dataSet.LastShipmentDate).ToString("dd/MM/yyyy");
            ob.ExpiryDate = Convert.ToDateTime(dataSet.ExpiryDate).ToString("dd/MM/yyyy");
            ob.ExpiryPlace = dataSet.ExpiryPlace == null ? "" : dataSet.ExpiryPlace;
            ob.RecordStatus = dataSet.RecordStatus;
            ob.Remarks = dataSet.Remarks;
            return Json(ob, JsonRequestBehavior.AllowGet);

        }

        // ##################### Search END ##############



        // ############################ CONFIRM RECORD STATUS ##########

        public ActionResult ConfirmRecordStatus(string lcId)
        {
            _vmMsg = objDalExpLCOpening.ConfirmRecordStatus(lcId, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        // ############################# END OF CONFIRM RECORD STATUS #################
    }
}