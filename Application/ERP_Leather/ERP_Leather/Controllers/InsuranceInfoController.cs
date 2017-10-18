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
    public class InsuranceInfoController : Controller
    {
        private UnitOfWork repo;
        private ValidationMsg _vmMsg;
        private DALLCOpening dalLC; 
        public InsuranceInfoController()
        {
             _vmMsg = new ValidationMsg();
             repo = new UnitOfWork();
             dalLC = new DALLCOpening();
        }
         [CheckUserAccess("InsuranceInfo/Insurance")]
        public ActionResult Insurance()
        {
            ViewBag.Currency = new SelectList(repo.CurrencyRepository.Get(filter: ob => ob.IsActive == true), "CurrencyID", "CurrencyName");
            return View();
        }
        public ActionResult GetLCNoInfo()
        {          
            var lstLcOpening = dalLC.GetLCList();            
            ViewBag.SearchTitle = "LC NO :";
            return Json(lstLcOpening, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchLCNoByLCNo(string search)
        {
            search = search.ToUpper();
            var result =(from temp in dalLC.GetLCList().ToList()              
                         where (temp.LCNo.ToUpper().StartsWith(search) || temp.LCNo.ToUpper() == search)
                        select temp).OrderByDescending(o => o.LCID);
               
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLCNoSearch()
        {
            var search = dalLC.GetLCList().ToList().Select(ob => ob.LCNo);
            return Json(search, JsonRequestBehavior.AllowGet);
        }
       
        [HttpPost]
        public ActionResult Save(LCM_Insurence dataSet)
        {
            try
            {                
                LCM_Insurence obInsurance = new LCM_Insurence();
                obInsurance.InsuranceChallanNo = dataSet.InsuranceChallanNo;
                obInsurance.InsuranceCompany = dataSet.InsuranceCompany;
                if (!string.IsNullOrEmpty(dataSet.LCNo))
                { 
                obInsurance.LCNo = dataSet.LCNo;
                obInsurance.LCID = Convert.ToInt32(dataSet.LCID);
                }
                obInsurance.InsuranceNo = dataSet.InsuranceNo;
                obInsurance.CoverNoteDate =dataSet.CoverNoteDate.ToString();
                obInsurance.MoneyReceiptNo = dataSet.MoneyReceiptNo;
                obInsurance.MoneyReceiptDate = DalCommon.SetDate(Convert.ToDateTime(dataSet.MoneyReceiptDate).ToString("dd/MM/yyyy"));
                obInsurance.ClauseEndorsement = dataSet.ClauseEndorsement;
                obInsurance.InsuranceChallanNo = dataSet.InsuranceChallanNo;
                obInsurance.InsurancChallanDate = DalCommon.SetDate(Convert.ToDateTime(dataSet.InsurancChallanDate).ToString("dd/MM/yyyy"));
                obInsurance.PolicyNo = dataSet.PolicyNo;
                obInsurance.PolicyDate = DalCommon.SetDate(Convert.ToDateTime(dataSet.PolicyDate).ToString("dd/MM/yyyy"));
                obInsurance.InvoiceAmount = dataSet.InvoiceAmount;
                if (dataSet.InvouceCurrency != 0) { obInsurance.InvouceCurrency = dataSet.InvouceCurrency; }                
                obInsurance.IncreasePercent = (decimal)dataSet.IncreasePercent;
                obInsurance.IncreaseAmount = (decimal)dataSet.IncreaseAmount;
                obInsurance.SumInsured = (decimal)dataSet.SumInsured;
                obInsurance.ExchangeRate = (decimal)dataSet.ExchangeRate;
                obInsurance.ExchangeRateCurrency = (byte)dataSet.ExchangeRateCurrency;
                obInsurance.ExchangedSumInsured = (decimal)dataSet.ExchangedSumInsured;
                obInsurance.MarinePremiumPercent = (decimal)dataSet.MarinePremiumPercent;
                obInsurance.MarinePremiumAmount = (decimal)dataSet.MarinePremiumAmount;
                obInsurance.DiscountPercent = (decimal)dataSet.DiscountPercent;
                obInsurance.DiscountAmount = (decimal)dataSet.DiscountAmount;
                obInsurance.WarSRCCPercent = (decimal)dataSet.WarSRCCPercent;
                obInsurance.WarSRCCPercentAmount = (decimal)dataSet.WarSRCCPercentAmount;
                obInsurance.NetPremium = (decimal)dataSet.NetPremium;
                obInsurance.VatPercent = (decimal)dataSet.VatPercent;
                obInsurance.VatAmount = (decimal)dataSet.VatAmount;
                //obInsurance.StampDutyRetio = (decimal)dataSet.StampDutyRetio;
                obInsurance.StampDutyAmount = (decimal)dataSet.StampDutyAmount;
                obInsurance.GrandPremium = (decimal)dataSet.GrandPremium;
                obInsurance.RefundPercent = (decimal)dataSet.RefundPercent;
                obInsurance.RefundAmount = (decimal)dataSet.RefundAmount;
                obInsurance.PaidAmount = (decimal)dataSet.PaidAmount;
                obInsurance.RevisionNo = dataSet.RevisionNo;
                obInsurance.RevisionDate = DalCommon.SetDate(Convert.ToDateTime(dataSet.RevisionDate).ToString("dd/MM/yyyy"));
                obInsurance.RunningStatus = dataSet.RunningStatus;
                obInsurance.RecordStatus = "NCF";
                obInsurance.SetOn = DateTime.Now;
                obInsurance.SetBy = Convert.ToInt32(Session["UserID"]);
                obInsurance.ModifiedOn = DateTime.Now;
                obInsurance.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                obInsurance.IPAddress = GetIPAddress.LocalIPAddress();
                if(!string.IsNullOrEmpty(dataSet.PIID.ToString()))
                {
                    obInsurance.PIID = Convert.ToInt32(dataSet.PIID);
                }

                if (!string.IsNullOrEmpty(dataSet.CIID.ToString())) { obInsurance.CIID = Convert.ToInt32(dataSet.CIID); }                
                obInsurance.CINo = dataSet.CINo;

                repo.LcmInsuranceRpository.Insert(obInsurance);
                int flag = repo.Save(); // obEntity.SaveChanges();
                if (flag == 1)
                {
                    _vmMsg.ReturnId = obInsurance.InsuranceID;
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Saved Successfully.";
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Saved Faild.";
                }
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return Json(new { msg = _vmMsg });
        }

        // ##################### Search Start ##############
        public ActionResult GetLcmInsuranceInfo()
        {
            List<Lcm_Insurence> lst = new List<Lcm_Insurence>();
            var lstLcmInsurance = repo.LcmInsuranceRpository.Get();
            foreach (var item in lstLcmInsurance)
            {
                Lcm_Insurence obIns = new Lcm_Insurence();
                obIns.InsuranceID = item.InsuranceID;
                obIns.InsuranceNo = item.InsuranceNo;
                var exist= (repo.LCOpeningRepository.Get(ob => ob.LCNo == item.LCNo)).ToList();
                if (exist.Count() > 0)
                {

                    obIns.LCNo = item.LCNo;
                    obIns.LCDate = Convert.ToDateTime((from t in repo.LCOpeningRepository.Get(ob => ob.LCNo ==  item.LCNo) select t.LCDate).FirstOrDefault()).ToString("dd/MM/yyyy");
                }
                else
                {
                    obIns.LCNo = "";
                    obIns.LCDate = "";
                }
                obIns.CoverNoteDate = item.CoverNoteDate;
                
                if (string.IsNullOrEmpty(item.CINo))
                {
                    obIns.CIDate = "";
                    obIns.CINo = "";
                }
                else
                {
                    obIns.CINo = item.CINo;
                obIns.CIDate = Convert.ToDateTime((from t in repo.CommercialInvoiceRepository.Get(ob => ob.CINo == (item.CINo == null ? "" : item.CINo)) select t.CIDate).FirstOrDefault()).ToString("dd/MM/yyyy");
                }
                if (item.PIID != null)
                {
                    obIns.PINo = repo.PRQ_ChemicalPIRepository.GetByID(Convert.ToInt32(item.PIID)).PINo;
                    obIns.PIDate = Convert.ToDateTime(repo.PRQ_ChemicalPIRepository.GetByID(Convert.ToInt32(item.PIID)).PIDate).ToString("dd/MM/yyyy");
                }
                else
                {
                    obIns.PINo = "";
                    obIns.PIDate = "";
                }

               
                obIns.PaidAmount = Convert.ToDecimal(item.PaidAmount) + Convert.ToDecimal(item.StampDutyAmount);
                obIns.RecordStatus = DalCommon.ReturnRecordStatus(item.RecordStatus);
                obIns.SupplierName = (from t in repo.PRQ_ChemicalPIRepository.Get()
                                 join t2 in repo.SysSupplierRepository.Get() on t.SupplierID equals t2.SupplierID
                                 where t.PIID == item.PIID select t2.SupplierName).FirstOrDefault();  
                lst.Add(obIns);
            }




            var lstData = from temp in lst
                                  select new
                                  {
                                      InsuranceID = temp.InsuranceID,
                                      InsuranceNo = temp.InsuranceNo,
                                      LCNo = temp.LCNo,
                                      LCDate = temp.LCDate,
                                      CoverNoteDate = temp.CoverNoteDate,
                                      CINo = temp.CINo,
                                      CIDate = temp.CIDate,
                                      PINo = temp.PINo,
                                      PIDate = temp.PIDate,
                                      SupplierName = temp.SupplierName,
                                      TotalPaidAmount = temp.PaidAmount,
                                      RecordStatus = temp.RecordStatus
                                  }; //obLCOpeningDAL.GetLCOpeningInfo();

            return Json(lstData.OrderByDescending(o => o.InsuranceID), JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLcmInsuranceByInsuranceNo(string search)
        {
            var lcInfo = from temp in repo.LcmInsuranceRpository.Get().Where(ob => (ob.InsuranceNo.StartsWith(search) || ob.InsuranceNo == search))
                         select new { 
                            InsuranceID = temp.InsuranceID, 
                            InsuranceNo = temp.InsuranceNo, 
                            LCNo= temp.LCNo, 
                            CoverNoteDate= temp.CoverNoteDate,                            
                            CIID = Convert.ToInt32(temp.CIID),
                            CINo = temp.CINo,
                            TotalPaidAmount = (temp.PaidAmount + temp.StampDutyAmount),
                            RecordStatus =DalCommon.ReturnRecordStatus(temp.RecordStatus)
                         };

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var data = repo.LcmInsuranceRpository.Get().Select(ob => ob.InsuranceNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // ##################### Search END ##############

        public ActionResult GetLcDetailsInfoByInsuranceID(string InsuranceID)
        {
            int id = Convert.ToInt32(InsuranceID);
            LCM_Insurence dataSet = repo.LcmInsuranceRpository.GetByID(id);

            Lcm_Insurence obInsurance = new Lcm_Insurence();
            obInsurance.InsuranceID = dataSet.InsuranceID;
            obInsurance.InsuranceChallanNo = dataSet.InsuranceChallanNo;
            obInsurance.InsuranceCompany = dataSet.InsuranceCompany;
            obInsurance.InsuranceCompanyName = repo.BranchRepository.GetByID(dataSet.InsuranceCompany).BranchName;
            if (!string.IsNullOrEmpty(dataSet.LCNo)) {            
            obInsurance.LCNumber = dataSet.LCNo;
            obInsurance.LCNo = Convert.ToString((from t in repo.LCOpeningRepository.Get(ob => ob.LCNo == dataSet.LCNo) select t.LCID).FirstOrDefault());             
            }
            obInsurance.InsuranceNo = dataSet.InsuranceNo;
            obInsurance.CoverNoteDate = dataSet.CoverNoteDate;
            obInsurance.MoneyReceiptNo = dataSet.MoneyReceiptNo;
            if (dataSet.MoneyReceiptDate.ToString() != "01/01/0001 12:00:00 AM") 
            { obInsurance.MoneyReceiptDate = Convert.ToDateTime(dataSet.MoneyReceiptDate).ToString("dd/MM/yyyy"); }
           
            obInsurance.ClauseEndorsement = dataSet.ClauseEndorsement;
            obInsurance.InsuranceChallanNo = dataSet.InsuranceChallanNo;
            if (dataSet.InsurancChallanDate.ToString() != "01/01/0001 12:00:00 AM") { obInsurance.InsurancChallanDate = Convert.ToDateTime(dataSet.InsurancChallanDate).ToString("dd/MM/yyyy"); }
           
            obInsurance.PolicyNo = dataSet.PolicyNo;
            if (dataSet.PolicyDate.ToString() != "01/01/0001 12:00:00 AM")
            {
                obInsurance.PolicyDate = Convert.ToDateTime(dataSet.PolicyDate).ToString("dd/MM/yyyy");
            }
            obInsurance.InvoiceAmount = dataSet.InvoiceAmount;
            obInsurance.InvouceCurrency = dataSet.InvouceCurrency;
            obInsurance.IncreasePercent = (decimal)dataSet.IncreasePercent;
            obInsurance.IncreaseAmount = (decimal)dataSet.IncreaseAmount;
            obInsurance.SumInsured = (decimal)dataSet.SumInsured;
            obInsurance.ExchangeRate = (decimal)dataSet.ExchangeRate;
            obInsurance.ExchangeRateCurrency = (byte)dataSet.ExchangeRateCurrency;
            obInsurance.ExchangedSumInsured = (decimal)dataSet.ExchangedSumInsured;
            obInsurance.MarinePremiumPercent = (decimal)dataSet.MarinePremiumPercent;
            obInsurance.MarinePremiumAmount = (decimal)dataSet.MarinePremiumAmount;
            obInsurance.DiscountPercent = (decimal)dataSet.DiscountPercent;
            obInsurance.DiscountAmount = (decimal)dataSet.DiscountAmount;
            obInsurance.WarSRCCPercent = (decimal)dataSet.WarSRCCPercent;
            obInsurance.WarSRCCPercentAmount = (decimal)dataSet.WarSRCCPercentAmount;
            obInsurance.NetPremium = (decimal)dataSet.NetPremium;
            obInsurance.VatPercent = (decimal)dataSet.VatPercent;
            obInsurance.VatAmount = (decimal)dataSet.VatAmount;
            //obInsurance.StampDutyRetio = (decimal)dataSet.StampDutyRetio;
            obInsurance.StampDutyAmount = (decimal)dataSet.StampDutyAmount;
            obInsurance.GrandPremium = (decimal)dataSet.GrandPremium;
            obInsurance.RefundPercent = (decimal)dataSet.RefundPercent;
            obInsurance.RefundAmount = (decimal)dataSet.RefundAmount;
            obInsurance.PaidAmount = (decimal)dataSet.PaidAmount;
            obInsurance.RevisionNo = dataSet.RevisionNo;
            if (dataSet.RevisionDate.ToString() != "01/01/0001 12:00:00 AM") { obInsurance.RevisionDate = Convert.ToDateTime(dataSet.RevisionDate).ToString("dd/MM/yyyy"); }
           
            obInsurance.RecordStatus = dataSet.RecordStatus;
            obInsurance.ApprovalAdvice = dataSet.ApprovalAdvice;
            obInsurance.CIID = Convert.ToInt32(dataSet.CIID);
            obInsurance.CINo = dataSet.CINo;
            if (dataSet.PIID != null) { obInsurance.PIID = dataSet.PIID.ToString(); }
            else { obInsurance.PIID = ""; }           
            return Json(obInsurance, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(Lcm_Insurence dataSet)
        {
            try
            {
                LCM_Insurence obInsurance = repo.LcmInsuranceRpository.GetByID(Convert.ToInt32(dataSet.InsuranceID));
                if (!obInsurance.RecordStatus.Equals("CNF"))
                {
                    obInsurance.InsuranceChallanNo = dataSet.InsuranceChallanNo;
                    obInsurance.InsuranceCompany = dataSet.InsuranceCompany;
                    obInsurance.LCID = dataSet.LCID;
                    obInsurance.LCNo = dataSet.LCNo;
                    obInsurance.InsuranceNo = dataSet.InsuranceNo;
                    obInsurance.CoverNoteDate =dataSet.CoverNoteDate==null?"": dataSet.CoverNoteDate.ToString();
                    obInsurance.MoneyReceiptNo = dataSet.MoneyReceiptNo;
                    obInsurance.MoneyReceiptDate = DalCommon.SetDate(dataSet.MoneyReceiptDate==null?"":dataSet.MoneyReceiptDate.ToString());
                    obInsurance.ClauseEndorsement = dataSet.ClauseEndorsement;
                    obInsurance.InsuranceChallanNo = dataSet.InsuranceChallanNo;
                    obInsurance.InsurancChallanDate = DalCommon.SetDate(dataSet.InsurancChallanDate==null?"": dataSet.InsurancChallanDate.ToString());
                    obInsurance.PolicyNo = dataSet.PolicyNo;
                    obInsurance.PolicyDate = DalCommon.SetDate(dataSet.PolicyDate==null?"": dataSet.PolicyDate.ToString());
                    obInsurance.InvoiceAmount = dataSet.InvoiceAmount;
                    if (dataSet.InvouceCurrency != 0) { obInsurance.InvouceCurrency = dataSet.InvouceCurrency; }                   
                    obInsurance.IncreasePercent = (decimal)dataSet.IncreasePercent;
                    obInsurance.IncreaseAmount = (decimal)dataSet.IncreaseAmount;
                    obInsurance.SumInsured =(decimal)dataSet.SumInsured;
                    obInsurance.ExchangeRate = (decimal)dataSet.ExchangeRate;
                    obInsurance.ExchangeRateCurrency = (byte)dataSet.ExchangeRateCurrency;
                    obInsurance.ExchangedSumInsured = (decimal)dataSet.ExchangedSumInsured;
                    obInsurance.MarinePremiumPercent = (decimal)dataSet.MarinePremiumPercent;
                    obInsurance.MarinePremiumAmount = (decimal)dataSet.MarinePremiumAmount;
                    obInsurance.DiscountPercent = (decimal)dataSet.DiscountPercent;
                    obInsurance.DiscountAmount = (decimal)dataSet.DiscountAmount;
                    obInsurance.WarSRCCPercent = (decimal)dataSet.WarSRCCPercent;
                    obInsurance.WarSRCCPercentAmount = (decimal)dataSet.WarSRCCPercentAmount;
                    obInsurance.NetPremium = (decimal)dataSet.NetPremium;
                    obInsurance.VatPercent = (decimal)dataSet.VatPercent;
                    obInsurance.VatAmount =(decimal)dataSet.VatAmount;
                    obInsurance.StampDutyAmount =(decimal)dataSet.StampDutyAmount;
                    obInsurance.GrandPremium =(decimal)dataSet.GrandPremium;
                    obInsurance.RefundPercent = (decimal)dataSet.RefundPercent;
                    obInsurance.RefundAmount = (decimal)dataSet.RefundAmount;
                    obInsurance.PaidAmount = (decimal)dataSet.PaidAmount;
                    obInsurance.RevisionNo = dataSet.RevisionNo;
                    obInsurance.RevisionDate = DalCommon.SetDate(dataSet.RevisionDate == null?"": dataSet.RevisionDate.ToString());
                    obInsurance.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                    obInsurance.ApprovalAdvice =dataSet.ApprovalAdvice;
                    if (dataSet.CIID != 0) { obInsurance.CIID = Convert.ToInt32(dataSet.CIID); }
                   
                    obInsurance.CINo = dataSet.CINo;
                    if (!string.IsNullOrEmpty(dataSet.PIID)) { obInsurance.PIID = Convert.ToInt32(dataSet.PIID); }
 
                    repo.LcmInsuranceRpository.Update(obInsurance);
                    int flag = repo.Save();
                    if (flag == 1)
                    {
                        _vmMsg.Type = Enums.MessageType.Success;
                        _vmMsg.Msg = "Updated Successfully.";
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Updated Faild.";
                    }
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Data already confirmed. You can't update this.";
                }
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return Json(new { msg = _vmMsg });
        }


        public ActionResult UpdateRecordStatus(string note, string insuranceID)
        {
            try
            {
                if (insuranceID != "")
                {
                        LCM_Insurence ob = repo.LcmInsuranceRpository.GetByID(Convert.ToInt32(insuranceID));
                        ob.ApprovalAdvice = note;
                        ob.RecordStatus = "CHK";
                        repo.LcmInsuranceRpository.Update(ob);
                        int flag = repo.Save();
                        if (flag == 1)
                        {
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Checked Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Checked Faild.";
                        }
                  
                }
                else
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Please save the record.";
                }
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return Json(new { msg = _vmMsg });
        }

        public ActionResult ConfirmRecordStatus(string insuranceID)
        {
            try
            { 
                if (insuranceID != "")
                {
                    LCM_Insurence ob = repo.LcmInsuranceRpository.GetByID(Convert.ToInt32(insuranceID));
                    if (ob.RecordStatus == "NCF")
                    {                       
                        ob.RecordStatus = "CNF";
                        ob.CheckedBy = Convert.ToInt32(Session["UserID"]);
                        ob.CheckDate = DateTime.Now;
                        repo.LcmInsuranceRpository.Update(ob);
                        int flag = repo.Save();
                        if (flag == 1)
                        {
                            _vmMsg.Type = Enums.MessageType.Success;
                            _vmMsg.Msg = "Confirmed Successfully.";
                        }
                        else
                        {
                            _vmMsg.Type = Enums.MessageType.Error;
                            _vmMsg.Msg = "Confirmed Faild.";
                        }
                    }
                    else
                    {
                        _vmMsg.Type = Enums.MessageType.Error;
                        _vmMsg.Msg = "Please Save the record.";
                    }
                }
               
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name,
                                                    eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                    ve.PropertyName,
                                                    ve.ErrorMessage));
                    }
                }
                throw new DbEntityValidationException(sb.ToString(), e);
            }
            return Json(new { msg = _vmMsg });
        }
        //[HttpPost]
        //public bool IsValidInsuranceNo(string no)
        //{
        //    return _utility.IsValid("LCM_Insurence", "InsuranceNo", no, "str");
        //}
	}
}