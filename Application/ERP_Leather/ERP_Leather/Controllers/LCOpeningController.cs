using ERP.DatabaseAccessLayer.AppSetupGateway;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using ERP_Leather.ActionFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_Leather.Controllers
{
    public class LCOpeningController : Controller
    {
        private UnitOfWork repo;
        private DALLCOpening obLCOpeningDAL;
        private ValidationMsg _vmMsg;
        //
        // GET: /LCOpening/

        public LCOpeningController()
        {
            _vmMsg = new ValidationMsg();
            repo = new UnitOfWork();
            obLCOpeningDAL = new DALLCOpening();
        }

        [CheckUserAccess("LCOpening/LCOpening")]
        public ActionResult LCOpening()
        {
            DalProformaInvoice objDALPI = new DalProformaInvoice();
            ViewBag.Currency = new SelectList(repo.CurrencyRepository.Get(filter: ob => ob.IsActive == true), "CurrencyID", "CurrencyName");
            DalSysCurrency objCurrency = new DalSysCurrency();
            DalSysCountry objCountry = new DalSysCountry();
            DalSysBank objBank = new DalSysBank();
            DalSysPort objPort = new DalSysPort();

            ViewBag.formTiltle = "PI/Indent";
            ViewBag.CurrencyList = objCurrency.GetAllActiveCurrency();
            ViewBag.CountryList = objCountry.GetAll();
            ViewBag.BankList = objDALPI.GetAllBranchNameWithBank2();
            ViewBag.PortList = objPort.GetAll();
            return View();
        }


        public ActionResult GetBankAdditionalInfo(string bankId, string branchName)
        {
            int bank = Convert.ToInt32(bankId);
            var lstBank = repo.BranchRepository.Get(filter: ob => ob.BankID == bank && ob.BranchName == branchName && ob.IsActive == true);
            return Json(lstBank, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBenificiaryInfo()
        {
            var benificiaryInfo = obLCOpeningDAL.GetBeneficiaryInfo();
            return Json(benificiaryInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBeneficiarySearch()
        {
            var benificiaryInfo = repo.SysSupplierRepository.Get(filter: ob => ob.IsActive == true).Select(ob => ob.SupplierName);
            return Json(benificiaryInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchBenificiaryByBenificiary(string beni)
        {
            var benificiaryInfo = obLCOpeningDAL.SearchBenificiaryByBenificiary(beni);
            return Json(benificiaryInfo, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPIInfo(string lc)
        {
            var piInfo = obLCOpeningDAL.GetPIInfo();

            List<ChemicalPI> lstPI = new List<ChemicalPI>();
            foreach (var item in piInfo)
            {
                ChemicalPI obPI = new ChemicalPI();
                obPI.PIID = item.PIID;
                obPI.BranchID = item.BranchID;
                obPI.PINo = item.PINo;
                obPI.PIDate =item.PIDate;
                obPI.BankSwiftCode = item.BankSwiftCode;
                obPI.AdvisingBankID = item.AdvisingBankID;
                obPI.AdvisingBankName = item.AdvisingBankName;
                obPI.BeneficiaryBankID = item.BeneficiaryBankID;
                obPI.BeneficiaryBankName = item.BeneficiaryBankName;
                obPI.Beneficiary = item.Beneficiary;
                obPI.BeneficiaryName = item.BeneficiaryName;
                obPI.BuyerAddressID = item.BuyerAddressID;
                obPI.BenificiaryAddress = item.BenificiaryAddress;
                obPI.ActualPrice = Math.Round(Convert.ToDecimal(item.ActualPrice),2);
                //#####################
                obPI.PICurrency = item.PICurrency;
                obPI.PaymentTerm = item.PaymentTerm;
                obPI.PaymentMode = item.PaymentMode;
                obPI.DeferredDays = item.DeferredDays;
                obPI.CountryOforigin = item.CountryOforigin;
                obPI.BeneficiaryBank = item.BeneficiaryBankID;
                obPI.DeliveryTerm = item.DeliveryTerm;
                obPI.PortofDischarge = item.PortofDischarge;
                obPI.DeliveryMode = item.DeliveryMode;
                obPI.PortofLoading = item.PortofLoading;
                obPI.Transshipment = item.Transshipment;
                obPI.PartialShipment = item.PartialShipment;
                obPI.GoodDelivery = item.GoodDelivery;
                obPI.ShippingMark = item.ShippingMark;
                obPI.ExpectedShipmentTime = item.ExpectedShipmentTime;
                obPI.Packing = item.Packing;
                obPI.PreShipmentIns = item.PreShipmentIns;
                obPI.OfferValidityDays = item.OfferValidityDays;
                lstPI.Add(obPI);
            }

            return Json(lstPI, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPISearch()
        {
            var benificiaryInfo = repo.PRQ_ChemicalPIRepository.Get().Select(ob => ob.PINo);
            return Json(benificiaryInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchPIByPI(string beni)
        {
            var piInfo = repo.PRQ_ChemicalPIRepository.Get().Where(ob => ob.PINo.StartsWith(beni) || ob.PINo == beni);
            List<ChemicalPI> lstPI = new List<ChemicalPI>();
            foreach (var item in piInfo)
            {
                ChemicalPI obPI = new ChemicalPI();
                obPI.PIID = item.PIID;
                obPI.PINo = item.PINo;
                obPI.PIDate = Convert.ToDateTime(item.PIDate).ToShortDateString();
                lstPI.Add(obPI);
            }
            return Json(lstPI, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Save(LcmLcOpening dataSet)
        {
            _vmMsg = obLCOpeningDAL.SaveLCOpeningInfo(dataSet, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult Update(LcmLcOpening dataSet)
        {
            _vmMsg = obLCOpeningDAL.UpdateLCOpeningInfo(dataSet, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }


        public ActionResult GetLCBankList(string bankCategory, string bankType)
        {
            var lstBank = obLCOpeningDAL.GetLCBankList(bankCategory, bankType);
            return Json(lstBank, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLcAdvBenfBankList(string bankCategory, string bankType)
        {
            var lstBank = obLCOpeningDAL.GetLcAdvBenfBankList(bankCategory);
            return Json(lstBank, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult SearchLCBankByBank(string bank, string bankCategory, string bankType)
        {
            bank = bank.ToUpper();
            var lstBank = obLCOpeningDAL.GetLCBankList(bankCategory, bankType);
            var result = from temp in lstBank where (temp.BankName.ToUpper().StartsWith(bank) || temp.BankName.ToUpper() == bank) select temp;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLCBankNameList(string bankCategory, string bankType)
        {
            var lstBank = repo.BankRepository.Get(filter: ob => ob.BankCategory == bankCategory && ob.BankType == bankType && ob.IsActive == true).Select(ob => ob.BankName);
            return Json(lstBank, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLCOpeningInfo()
        {
            var lstLcOpening = from temp in repo.LCOpeningRepository.Get().OrderByDescending(o => o.LCID)
                               select new
                               {
                                   LCID = temp.LCID,
                                   LCNo = temp.LCNo,
                                   LCDate = temp.LCDate,
                                   LCAmount = temp.LCAmount,
                                   RecordStatus = temp.RecordStatus,
                                   LCMargin = temp.LCMargin,
                                   //                               ###################
                                   // InsuranceNo = repo.LcmInsuranceRpository.GetByID(Convert.ToInt32(temp.InsuranceID == null ? 0 : temp.InsuranceID)).InsuranceNo,
                                   InsuranceID = temp.InsuranceID,
                                   PICurrency = temp.PICurrency,
                                   PaymentTerm = temp.PaymentTerm,
                                   PaymentMode = temp.PaymentMode,
                                   DeferredDays = temp.DeferredDays,
                                   CountryOforigin = temp.CountryOforigin,
                                   BeneficiaryBank = temp.BeneficiaryBank,
                                   BeneficiaryAddressID = temp.BeneficiaryAddressID == null ? 0 : temp.BeneficiaryAddressID,
                                   DeliveryTerm = temp.DeliveryTerm,
                                   PortofDischarge = temp.PortofDischarge,
                                   DeliveryMode = temp.DeliveryMode,
                                   PortofLoading = temp.PortofLoading,
                                   Transshipment = temp.Transshipment,
                                   PartialShipment = temp.PartialShipment,
                                   GoodDelivery = temp.GoodDelivery,
                                   ShippingMark = temp.ShippingMark,
                                   ExpectedShipmentTime = temp.ExpectedShipmentTime,
                                   Packing = temp.Packing,
                                   PreShipmentIns = temp.PreShipmentIns,
                                   OfferValidityDays = temp.OfferValidityDays
                               };
            List<LcmLcOpening> lst = new List<LcmLcOpening>();
            foreach (var item in lstLcOpening)
            {
                LcmLcOpening ob = new LcmLcOpening();
                ob.LCID = item.LCID;
                ob.LCNo = item.LCNo;
                ob.LCDate = Convert.ToDateTime(item.LCDate).ToString("dd/MM/yyyy");
                ob.LCAmount = item.LCAmount;
                ob.RecordStatus = DalCommon.ReturnRecordStatus(item.RecordStatus);
                ob.LCMargin = Convert.ToDecimal(item.LCMargin);
                ob.InsuranceID = item.InsuranceID;
                ob.PICurrency = item.PICurrency;
                ob.PaymentTerm = item.PaymentTerm;
                ob.PaymentMode = item.PaymentMode;
                ob.DeferredDays = item.DeferredDays;
                ob.CountryOforigin = item.CountryOforigin;
                ob.BeneficiaryBank = item.BeneficiaryBank;
                ob.BeneficiaryAddressID = item.BeneficiaryAddressID == null ? 0 : Convert.ToInt32(item.BeneficiaryAddressID);
                if (ob.BeneficiaryAddressID != 0) { ob.BeneficiaryAddress = repo.SupplierAddressRepository.GetByID(ob.BeneficiaryAddressID).Address; }
                else { ob.BeneficiaryAddress = "N/A"; }
                ob.DeliveryTerm = item.DeliveryTerm;
                ob.PortofDischarge = item.PortofDischarge;
                ob.DeliveryMode = item.DeliveryMode;
                ob.PortofLoading = item.PortofLoading;
                ob.Transshipment = item.Transshipment;
                ob.PartialShipment = item.PartialShipment;
                ob.GoodDelivery = item.GoodDelivery;
                ob.ShippingMark = item.ShippingMark;
                ob.ExpectedShipmentTime = item.ExpectedShipmentTime.ToString();
                ob.Packing = item.Packing;
                ob.PreShipmentIns = item.PreShipmentIns;
                ob.OfferValidityDays = item.OfferValidityDays;
                lst.Add(ob);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLCByLCNo(string search)
        {
            var lcInfo = from temp in repo.LCOpeningRepository.Get().Where(ob => (ob.LCNo.StartsWith(search) || ob.LCNo == search)).OrderByDescending(o => o.LCID)
                         select new
                         {
                             LCID = temp.LCID,
                             LCNo = temp.LCNo,
                             LCDate = temp.LCDate.ToString("dd/MM/yyyy"),
                             LCAmount = temp.LCAmount,
                             RecordStatus = temp.RecordStatus,
                             LCMargin = temp.LCMargin
                         };

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BtnSearchLCByLCNo(string search)
        {
            var lcInfo = from temp in repo.LCOpeningRepository.Get().Where(ob => (ob.LCNo.StartsWith(search) || ob.LCNo == search)).OrderByDescending(o => o.LCID)
                         select new
                         {
                             LCID = temp.LCID,
                             LCNo = temp.LCNo,
                             LCDate = temp.LCDate.ToString("dd/MM/yyyy"),
                             LCAmount = temp.LCAmount,
                             RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),
                             LCMargin = temp.LCMargin

                         };

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLCNOList()
        {
            var lcInfo = repo.LCOpeningRepository.Get().OrderByDescending(o => o.LCID).Select(ob => ob.LCNo);
            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateRecordStatus(string note, string lcId)
        {
            _vmMsg = obLCOpeningDAL.UpdateRecordStatus(note, lcId, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }

        [HttpPost]
        public ActionResult UpdateConfRecordStatus(string lcId, string approvalAdvice)
        {
            _vmMsg = obLCOpeningDAL.UpdateConfRecordStatus(lcId, approvalAdvice, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = _vmMsg });
        }
        public ActionResult GetLcDetailsInfoByLcNo(string LCID)
        {
            int lcId = Convert.ToInt32(LCID);
            var lcInfo = repo.LCOpeningRepository.GetByID(lcId);
            LcmLcOpening ob = new LcmLcOpening();
            if (lcInfo != null)
            {
                ob.LCID = lcInfo.LCID;
                ob.LCNo = lcInfo.LCNo;
                ob.LCDate = lcInfo.LCDate.ToString("dd/MM/yyyy");
                ob.LCAmount = lcInfo.LCAmount;
                ob.LCAmountCurrency = lcInfo.LCCurrencyID;
                var id = repo.BranchRepository.GetByID(lcInfo.LCOpeningBank).LCCurrency;
                string currency = "";
                if (id != null)
                {
                    currency = repo.CurrencyRepository.GetByID(id).CurrencyName;
                }
                ob.CurrencyName = currency;//lcInfo.Sys_Currency.CurrencyName;
                ob.ExchangeRate = Convert.ToDecimal(lcInfo.ExchangeRate);
                ob.ExchangeValue = Convert.ToDecimal(lcInfo.ExchangeValue);
                ob.IssueDate = Convert.ToDateTime(lcInfo.IssueDate).ToString("dd/MM/yyyy");
                ob.ExpiryDate = Convert.ToDateTime(lcInfo.ExpiryDate).ToString("dd/MM/yyyy");
                ob.ExpiryPlace = lcInfo.ExpiryPlace;
                ob.NNDSendingTime = Convert.ToInt32(lcInfo.NNDSendingTime);
                ob.LCANo = lcInfo.LCANo;
                ob.IRCNo = lcInfo.IRCNo;
                ob.VatRegNo = lcInfo.VatRegNo;
                ob.TINNo = lcInfo.TINNo;
                ob.BINNo = lcInfo.BINNo;
                ob.ICNNo = lcInfo.ICNNo;
                ob.ICNDate = Convert.ToDateTime(lcInfo.ICNDate).ToString("dd/MM/yyyy");
                ob.LastShipmentDate = Convert.ToDateTime(lcInfo.LastShipmentDate).ToString("dd/MM/yyyy");
                ob.BankID = Convert.ToInt32(lcInfo.LCOpeningBank);
                var openBranch = repo.BranchRepository.GetByID(lcInfo.LCOpeningBank);
                var opensBank = repo.BankRepository.GetByID(openBranch.BankID);
                if (opensBank != null) { ob.OpeningBINNo = repo.BankRepository.GetByID(opensBank.BankID).BankBINNo; }

                var branch = repo.BranchRepository.GetByID(Convert.ToInt32(lcInfo.LCOpeningBank));
                ob.BranchID = Convert.ToInt32(lcInfo.LCOpeningBank);
                ob.BranchName = branch.BranchName;
                ob.LCLimitAmount = Convert.ToDecimal(branch.LCLimit);
                ob.LCLimitCurrencyID = Convert.ToInt32(branch.LCCurrency);

                ob.PresentationDays = Convert.ToInt32(lcInfo.PresentationDays);
                ob.ConfirmStatus = lcInfo.ConfirmStatus;

                var openBank = repo.BankRepository.GetByID(Convert.ToInt32(branch.BankID));
                ob.BankName = openBank.BankName;
                if (lcInfo.AdvisingBank != null)
                {
                    var branchAdvising = repo.BranchRepository.GetByID(Convert.ToInt32(lcInfo.AdvisingBank));
                    var advBank = repo.BankRepository.GetByID(Convert.ToInt32(branchAdvising.BankID));
                    ob.AdvisingBank = Convert.ToInt32(lcInfo.AdvisingBank);

                    var branchInfo = repo.BranchRepository.GetByID(Convert.ToInt32(lcInfo.AdvisingBank));
                    var bankInfo = repo.BankRepository.GetByID(branchInfo.BankID);

                    ob.AdvisingBINNo = advBank.BankBINNo == null ? "" : advBank.BankBINNo;
                    ob.AdvisingBankName = branchInfo.BranchName + ", " + bankInfo.BankName;
                    ob.BankSwiftCode = branchAdvising.BranchSwiftCode;
                }


                ob.BeneficiaryAddressID = Convert.ToInt32(lcInfo.BeneficiaryAddressID);
                var addressSupp = repo.SupplierAddressRepository.GetByID(Convert.ToInt32(lcInfo.BeneficiaryAddressID));
                if (addressSupp == null)
                {
                    ob.BeneficiaryAddress = "";
                }
                else { ob.BeneficiaryAddress = addressSupp.Address; }

                ob.LCMarginBalance = Convert.ToDecimal(lcInfo.LCMargin);
                var supp = repo.SysSupplierRepository.GetByID(lcInfo.Beneficiary);
                if (supp != null) { ob.BeneficiaryName = supp.SupplierName; }

                ob.PIID = lcInfo.PIID != null ? Convert.ToInt32(lcInfo.PIID) : 0;
                ob.ExchangeCurrency = lcInfo.ExchangeCurrency != null ? Convert.ToInt32(lcInfo.ExchangeCurrency) : 0;
                ob.PINo = lcInfo.PINo;
                var piInfo = repo.PRQ_ChemicalPIRepository.GetByID(lcInfo.PIID != null ? lcInfo.PIID : 0);
                if (piInfo == null)
                {
                    ob.PIDate = "";
                }
                else
                {
                    ob.PIDate = Convert.ToDateTime(piInfo.PIDate).ToShortDateString();
                }
                ob.LCReviceNo = lcInfo.LCReviceNo;
                ob.RunningStatus = lcInfo.RunningStatus;
                ob.LCState = lcInfo.LCState;
                ob.LCStatus = lcInfo.LCStatus;
                ob.RecordStatus = lcInfo.RecordStatus;
                ob.ApprovalAdvice = lcInfo.ApprovalAdvice;
                //ob.LCLimitAmount = lcInfo.LCAmount;
                //ob.LCLimitCurrencyID = lcInfo.LCCurrencyID != null?lcInfo.LCCurrencyID:0 ;
                ob.Beneficiary = lcInfo.Beneficiary != null ? lcInfo.Beneficiary.ToString() : "";
                ob.RecordStatus = lcInfo.RecordStatus;
                var flag = repo.LcmInsuranceRpository.GetByID(lcInfo.InsuranceID);
                if (flag != null)
                {
                    ob.InsuranceNo = flag.InsuranceNo;
                }
                else
                { ob.InsuranceNo = ""; }
            }
            return Json(ob, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPIDetailsInfo(string PINO)
        {
            PRQ_ChemicalPI lcPInfo = repo.PRQ_ChemicalPIRepository.Get(filter: ob => ob.PINo == PINO).FirstOrDefault();
            ViewBag.PINo = lcPInfo.PINo;
            ViewBag.PIDate = Convert.ToDateTime(lcPInfo.PIDate).ToString("dd/MM/yyyy");
            ViewBag.PICategory = lcPInfo.PICategory;
            if (lcPInfo.SupplierID != null)
            {
                ViewBag.SupplierID = repo.SysSupplierRepository.GetByID(lcPInfo.SupplierID).SupplierName;
            }
            else
            {
                ViewBag.SupplierID = "N/A";
            }
            if (lcPInfo.LocalAgent != null)
            {
                ViewBag.LocalAgent = repo.SysSupplierRepository.GetByID(lcPInfo.LocalAgent).SupplierName;
            }
            else {
                ViewBag.LocalAgent = "N/A";
            }
            if (lcPInfo.ForeignAgent != null)
            {
                ViewBag.ForeignAgent = repo.SysSupplierRepository.GetByID(lcPInfo.ForeignAgent).SupplierName;
            }
            else {
                ViewBag.ForeignAgent = "N/A";
            }
            if (lcPInfo.PICurrency != null)
            { ViewBag.PICurrency = repo.CurrencyRepository.GetByID(lcPInfo.PICurrency).CurrencyName; }
            else { ViewBag.PICurrency = "N/A"; }
            if (lcPInfo.ExchangeCurrency != null)
            { ViewBag.ExchangeCurrency = repo.CurrencyRepository.GetByID(lcPInfo.ExchangeCurrency).CurrencyName; }
            else { ViewBag.ExchangeCurrency = "N/A"; }
            ViewBag.ExchangeRate = lcPInfo.ExchangeRate == null ? 0 : lcPInfo.ExchangeRate;
            ViewBag.ExchangeValue = lcPInfo.ExchangeValue == null ? 0 : lcPInfo.ExchangeValue;
            ViewBag.PaymentTerm = lcPInfo.PaymentTerm;
            if (lcPInfo.PaymentMode == "ST") { 
             ViewBag.PaymentMode = "At Sight";
            }
            else if (lcPInfo.PaymentMode == "DF")
            {
                ViewBag.PaymentMode = "Defered";
            }
            else { ViewBag.PaymentMode = "N/A"; }

            ViewBag.DeferredDays = lcPInfo.DeferredDays == null ? "0" : lcPInfo.DeferredDays;
            if (lcPInfo.CountryOforigin != null)
            {
                ViewBag.CountryOforigin = repo.SysCountryRepository.GetByID(lcPInfo.CountryOforigin).CountryName;
            }
            if (lcPInfo.CheckedBy != null)
            {
                ViewBag.CheckedBy = repo.SysUserRepository.GetByID(lcPInfo.CheckedBy).MiddleName;
            }
            else
            {
                ViewBag.CheckedBy = "N/A";
            }

            ViewBag.CheckDate = lcPInfo.CheckDate == null ? "N/A" : lcPInfo.CheckDate.ToString();
            if (lcPInfo.ApprovedBy != null)
            {
                ViewBag.ApprovedBy = repo.SysUserRepository.GetByID(lcPInfo.ApprovedBy).MiddleName;
            }
            else
            {
                ViewBag.ApprovedBy = "N/A";
            }
            ViewBag.ApproveDate = lcPInfo.ApproveDate == null ? "N/A" : lcPInfo.ApproveDate.ToString(); ;

            return PartialView();

        }


        #region Insurance Popup Grid
        public ActionResult GetInsuranceInfo()
        {
            var result = repo.LcmInsuranceRpository.Get();
            List<Lcm_Insurence> lst = new List<Lcm_Insurence>();
            foreach (var item in result)
            {
                Lcm_Insurence ob = new Lcm_Insurence();
                ob.InsuranceID = item.InsuranceID;
                ob.InsuranceNo = item.InsuranceNo;
                //if (!string.IsNullOrEmpty(item.CoverNoteDate.ToString())) { ob.CoverNoteDate = Convert.ToDateTime(item.CoverNoteDate).ToString("dd/MM/yyyy"); }
                //else { ob.CoverNoteDate = ""; }
                ob.CoverNoteDate = item.CoverNoteDate.ToString();
                ob.InsuranceCompanyName = repo.BranchRepository.GetByID(item.InsuranceCompany).BranchName;

                lst.Add(ob);
            }
            return Json(lst.OrderByDescending(ob => ob.InsuranceID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchInsuranceByInsurance(string ins)
        {
            string InsuranceNo = ins.ToUpper();
            var info = repo.LcmInsuranceRpository.Get();
            List<Lcm_Insurence> lst = new List<Lcm_Insurence>();
            foreach (var item in info)
            {
                Lcm_Insurence ob = new Lcm_Insurence();
                ob.InsuranceID = item.InsuranceID;
                ob.InsuranceNo = item.InsuranceNo;
                //if (!string.IsNullOrEmpty(item.ApproveDate.ToString())) { ob.ApproveDate = Convert.ToDateTime(item.ApproveDate).ToString("dd/MM/yyyy"); }
                //else { ob.ApproveDate = ""; }
                ob.CoverNoteDate = item.CoverNoteDate.ToString();
                ob.InsuranceCompanyName = repo.BranchRepository.GetByID(item.InsuranceCompany).BranchName;

                //else { ob.InsuranceCompanyName = ""; }

                lst.Add(ob);
            }
            var result = from temp in lst where (temp.InsuranceNo.ToUpper().StartsWith(InsuranceNo) || temp.InsuranceNo.ToUpper() == InsuranceNo) select temp;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInsuranceAutocompleteData()
        {
            var search = repo.LcmInsuranceRpository.Get().Select(ob => ob.InsuranceNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}