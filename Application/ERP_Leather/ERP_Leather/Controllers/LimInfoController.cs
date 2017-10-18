using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.OperationGateway;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.AppSetupModel;
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
    public class LimInfoController : Controller
    {

        private UnitOfWork objRepository = new UnitOfWork();
        private DalLCM_LimInfo objDalLCM_LimInfo = new DalLCM_LimInfo();
        private ValidationMsg objValMssg = new ValidationMsg();
        private int _userId;


        [CheckUserAccess("LimInfo/LimInfo")]
        public ActionResult LimInfo()
        {
            ViewBag.Currency = new SelectList(objRepository.CurrencyRepository.Get(filter: ob => ob.IsActive == true), "CurrencyID", "CurrencyName");
            ViewBag.formTiltle = "Loan Agent Imported Marchandise";
            return View();

        }




        // ##################### Bank Pop UP ##############

        public ActionResult GetBankList()
        {
            var bankList = objDalLCM_LimInfo.GetLCBankList();
            return Json(bankList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchBankbyBankName(string search)//InsuranceNo
        {
            var lcInfo = (from temp in objRepository.BankRepository.Get().Where(ob => (ob.BankName.StartsWith(search) || ob.BankName == search)).OrderByDescending(o => o.BankName)

                          select new
                          {
                              BankName = temp.BankName,
                          });

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BtnSearchBankByBankName(string search)
        {
            var lcInfo = (from temp in objRepository.BankRepository.Get().Where(ob => (ob.BankName.StartsWith(search) || ob.BankName == search)).OrderByDescending(o => o.BankName)

                          select new
                          {
                              BankName = temp.BankName,
                          });

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAutoCompleteBankPopupData()
        {
            var data = objRepository.BankRepository.Get().Select(ob => ob.BankCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        // ##################### END of Bank Pop UP ##############

        public ActionResult Save(LCM_LimInfo dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);

            objValMssg = objDalLCM_LimInfo.SaveLCM_LimInfo(dataSet, "LimInfo/LimInfo", _userId);
            return Json(new { msg = objValMssg });
        }

        [HttpPost]
        public ActionResult Update(LCM_LimInfo dataSet)
        {
            _userId = Convert.ToInt32(Session["UserID"]);
            objValMssg = objDalLCM_LimInfo.UpdateLCM_LimInfo(dataSet, _userId);
            return Json(new { msg = objValMssg });
        }


        // ##################### Search Start ##############
        public ActionResult GetLcmLimInfo()
        {
            var listLcmLimInfo = (from temp in objRepository.LimInfoRepository.Get().AsEnumerable()
                                  join temp2 in objRepository.BankRepository.Get() on temp.LimBankID equals temp2.BankID
                                  join temp3 in objRepository.BranchRepository.Get() on temp.LimBranchID equals temp3.BranchID
                                  select new
                                  {
                                      LimID = temp.LimID,
                                      LCNo = temp.LCNo,
                                      LimNo = temp.LimNo,
                                      BankID = temp2.BankID,
                                      BranchID = temp3.BranchID,//temp.LCNo,
                                      BankCode = temp2.BankCode,
                                      BankName = temp2.BankName,
                                      BanchCode = temp3.BanchCode,
                                      BranchName = temp3.BranchName,
                                      LimBankID = temp.LimBankID,
                                      LimLimit = temp.LimLimit,
                                      LimBalance = temp.LimBalance,
                                      LimBranchID = temp.LimBranchID,
                                      LCID = temp.LCID,
                                      RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),

                                  }).OrderByDescending(ob => ob.LimID); //obLCOpeningDAL.GetLCOpeningInfo();
            return Json(listLcmLimInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLcmLimInfoByLCNo(string search)//InsuranceNo
        {
            var lcInfo = (from temp in objRepository.LimInfoRepository.Get().Where(ob => ob.LCNo.StartsWith(search) || ob.LCNo == search).AsEnumerable()
                          join temp2 in objRepository.BankRepository.Get() on temp.LimBankID equals temp2.BankID
                          join temp3 in objRepository.BranchRepository.Get() on temp.LimBranchID equals temp3.BranchID
                          select new
                          {
                              LimBankID = temp.LimBankID,
                              LimBranchID = temp.LimBranchID,
                              LimID = temp.LimID,
                              LimNo = temp.LimNo,
                              LCID = temp.LCID,
                              LCNo = temp.LCNo,
                              LimLimit = temp.LimLimit,
                              LimBalance = temp.LimBalance,
                              BranchID = temp3.BranchID,//temp.LCNo,
                              BankID = temp2.BankID,
                              BankCode = temp2.BankCode,
                              BankName = temp2.BankName,
                              BanchCode = temp3.BanchCode,
                              BranchName = temp3.BranchName,
                              RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus)
                          }).OrderByDescending(ob => ob.LimID);

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompleteData()
        {
            var data = objRepository.LimInfoRepository.Get().Select(ob => ob.LCNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLimInfoByLCID(string LimID)
        {
            int id = Convert.ToInt32(LimID);
            LCM_LimInfo dataSet = objRepository.LimInfoRepository.GetByID(id);

            LCMLimInfo ob = new LCMLimInfo();

            ob.LCNo = dataSet.LCNo == null ? "" : dataSet.LCNo;
            ob.LCID = Convert.ToInt32(dataSet.LCID);
            ob.LimNo = dataSet.LimNo == null ? "" : dataSet.LimNo;
            ob.LimID = dataSet.LimID;
            ob.LimBankID = Convert.ToInt32(dataSet.LimBankID);
            ob.LimBranchID = Convert.ToInt32(dataSet.LimBranchID);
            ob.BranchName = objRepository.BranchRepository.GetByID(dataSet.LimBranchID == null ? 0 : dataSet.LimBranchID).BranchName;
            ob.BankName = objRepository.BankRepository.GetByID(dataSet.LimBankID == null ? 0 : dataSet.LimBankID).BankName;
            ob.BankID = objRepository.BankRepository.GetByID(dataSet.LimBankID == null ? 0 : dataSet.LimBankID).BankID;
            ob.BranchID = objRepository.BranchRepository.GetByID(dataSet.LimBranchID == null ? 0 : dataSet.LimBranchID).BranchID;
            ob.LimLimit = dataSet.LimLimit == null ? 0 : dataSet.LimLimit;
            ob.LimBalance = dataSet.LimBalance == null ? 0 : dataSet.LimBalance;
            ob.LimDate = Convert.ToDateTime(dataSet.LimDate).ToString("dd/MM/yyyy");
            ob.LimCurrency = (byte)dataSet.LimCurrency;
            ob.ExchangeRate = (decimal)dataSet.ExchangeRate == null ? 0 : (decimal)dataSet.ExchangeRate;
            ob.ExchangeValue = (decimal)dataSet.ExchangeValue == null ? 0 : (decimal)dataSet.ExchangeValue;
            ob.ExchangeCurrency = (byte)dataSet.ExchangeCurrency;

            ob.LoanAmount = (decimal)dataSet.LoanAmount == null ? 0 : (decimal)dataSet.LoanAmount;
            ob.InterestPersent = (decimal)dataSet.InterestPersent == null ? 0 : (decimal)dataSet.InterestPersent;
            ob.InterestAmount = (decimal)dataSet.InterestAmount == null ? 0 : (decimal)dataSet.InterestAmount;
            ob.AmountToBePaid = (decimal)dataSet.AmountToBePaid == null ? 0 : (decimal)dataSet.AmountToBePaid;
            ob.AdjustmentTime = Convert.ToInt32(dataSet.AdjustmentTime == null ? 0 : dataSet.AdjustmentTime);
            ob.TotalAmountToBePaid = (decimal)dataSet.TotalAmountToBePaid == null ? 0 : (decimal)dataSet.TotalAmountToBePaid;
            ob.OtherCharges = (decimal)dataSet.OtherCharges == null ? 0 : (decimal)dataSet.OtherCharges;
            ob.AcceptanceCommission = (decimal)dataSet.AcceptanceCommission == null ? 0 : (decimal)dataSet.AcceptanceCommission;
            ob.HandlingCharge = (decimal)dataSet.HandlingCharge == null ? 0 : (decimal)dataSet.HandlingCharge;
            ob.PaidAmount = (decimal)dataSet.PaidAmount == null ? 0 : (decimal)dataSet.PaidAmount;

           // ob.LCMarginTransfer = (decimal)dataSet.LCMarginTransfer == null ? 0 : (decimal)dataSet.LCMarginTransfer;

            if (dataSet.LCMarginTransfer != 0)
            {
                ob.LCMarginTransfer = dataSet.LCMarginTransfer;
            }
            else
            {
                ob.LCMarginTransfer = 0;
            }
            //ob.LimMarginTrans = (decimal)dataSet.LimMarginTrans == null ? 0 : (decimal)dataSet.LimMarginTrans;

            if (dataSet.LimMarginTrans != 0)
            {
                ob.LimMarginTrans = dataSet.LimMarginTrans;
            }
            else
            {
                ob.LimMarginTrans = 0;
            }
            //ob.LimMarginTransPercnt = (decimal)dataSet.LimMarginTransPercnt == null ? 0 : (decimal)dataSet.LimMarginTransPercnt;

            if (dataSet.LimMarginTransPercnt != 0)
            {
                ob.LimMarginTransPercnt = dataSet.LimMarginTransPercnt;
            }
            else
            {
                ob.LimMarginTransPercnt = 0;
            }
            ob.InterestCalcDate = Convert.ToDateTime(dataSet.InterestCalcDate).ToString("dd/MM/yyyy");//dataSet.InterestCalcDate;
           // ob.CalcInterestAmt = (decimal)dataSet.CalcInterestAmt == null ? 0 : (decimal)dataSet.CalcInterestAmt;

            if (dataSet.CalcInterestAmt != 0)
            {
                ob.CalcInterestAmt = dataSet.CalcInterestAmt;
            }
            else
            {
                ob.CalcInterestAmt = 0;
            }
           // ob.TransCash = (decimal)dataSet.TransCash == null ? 0 : (decimal)dataSet.TransCash;

            if (dataSet.TransCash != 0)
            {
                ob.TransCash = dataSet.TransCash;
            }
            else
            {
                ob.TransCash = 0;
            }
           // ob.CalcAmtToPaid = (decimal)dataSet.CalcAmtToPaid == null ? 0 : (decimal)dataSet.CalcAmtToPaid;

            if (dataSet.CalcAmtToPaid != 0)
            {
                ob.CalcAmtToPaid = dataSet.CalcAmtToPaid;
            }
            else
            {
                ob.CalcAmtToPaid = 0;
            }
            //ob.LimAdjustDr = (decimal)dataSet.LimAdjustDr == null ? 0 : (decimal)dataSet.LimAdjustDr;

            if (dataSet.LimAdjustDr != 0)
            {
                ob.LimAdjustDr = dataSet.LimAdjustDr;
            }
            else
            {
                ob.LimAdjustDr = 0;
            }
           // ob.LimAdjustCr = (decimal)dataSet.LimAdjustCr == null ? 0 : (decimal)dataSet.LimAdjustCr;

            if (dataSet.LimAdjustCr != 0)
            {
                ob.LimAdjustCr = dataSet.LimAdjustCr;
            }
            else
            {
                ob.LimAdjustCr = 0;
            }
            //ob.ExciseDuty = (decimal)dataSet.ExciseDuty == null ? 0 : (decimal)dataSet.ExciseDuty;

            if (dataSet.ExciseDuty != 0)
            {
                ob.ExciseDuty = dataSet.ExciseDuty;
            }
            else
            {
                ob.ExciseDuty = 0;
            }
            //ob.TotalCalcAmtToPaid = (decimal)dataSet.TotalCalcAmtToPaid == null ? 0 : (decimal)dataSet.TotalCalcAmtToPaid;

            if (dataSet.TotalCalcAmtToPaid != 0)
            {
                ob.TotalCalcAmtToPaid = dataSet.TotalCalcAmtToPaid;
            }
            else
            {
                ob.TotalCalcAmtToPaid = 0;
            }

            ob.RecordStatus = dataSet.RecordStatus;
            ob.Remarks = dataSet.Remarks;




            return Json(ob, JsonRequestBehavior.AllowGet);

        }

        // ##################### Search END ##############


        public ActionResult GetLcDetailsLimInfoByLimId(string LimID)//to bring data from grid to main page.....use
        {
            int limId = Convert.ToInt32(LimID);
            var limInfo = objRepository.LimInfoRepository.GetByID(limId);
            LCM_LimInfo ob = new LCM_LimInfo();
            if (limInfo != null)
            {
                ob.LimID = limInfo.LimID;
                ob.LimNo = limInfo.LimNo;
                ob.LimLimit = limInfo.LimLimit;
                ob.LimBalance = limInfo.LimBalance;
                ob.LCID = limInfo.LCID;
                ob.LCNo = limInfo.LCNo;
                ob.LimCurrency = limInfo.Sys_Currency.CurrencyID;
                ob.ExchangeCurrency = limInfo.ExchangeCurrency;
                ob.ExchangeRate = Convert.ToDecimal(limInfo.ExchangeRate);
                ob.ExchangeValue = Convert.ToDecimal(limInfo.ExchangeValue);
                ob.LoanAmount = limInfo.LoanAmount;


                ob.InterestPersent = limInfo.InterestPersent;
                ob.InterestAmount = limInfo.InterestAmount;
                ob.AmountToBePaid = limInfo.AmountToBePaid;
                ob.AdjustmentTime = limInfo.AdjustmentTime;
                ob.AcceptanceCommission = limInfo.AcceptanceCommission;
                ob.HandlingCharge = limInfo.HandlingCharge;
                ob.TotalAmountToBePaid = limInfo.TotalAmountToBePaid;
                //ob.LastShipmentDate = Convert.ToDateTime(limInfo.LastShipmentDate).ToString("dd/MM/yyyy");
                ob.LimBankID = limInfo.Sys_Bank.BankID;

                var branch = objRepository.BranchRepository.GetByID(Convert.ToInt32(limInfo.LimBranchID));
                ob.LimBranchID = limInfo.Sys_Branch.BranchID;
                ob.RecordStatus = limInfo.RecordStatus;
            }
            return Json(ob, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRecordStatus(string remarks, string limID)
        {
            try
            {
                if ((remarks != "" && limID != ""))
                {
                    LCM_LimInfo ob = objRepository.LimInfoRepository.GetByID(Convert.ToInt32(limID));
                    // ob.ApprovalAdvice = note;
                    ob.RecordStatus = "NCF";
                    objRepository.LimInfoRepository.Update(ob);
                    int flag = objRepository.Save();
                    if (flag == 1)
                    {
                        objValMssg.Type = Enums.MessageType.Success;
                        objValMssg.Msg = "Confirmed Successfully.";
                    }
                    else
                    {
                        objValMssg.Type = Enums.MessageType.Error;
                        objValMssg.Msg = "Confirme Faild.";
                    }
                }
                else
                {
                    objValMssg.Type = Enums.MessageType.Error;
                    objValMssg.Msg = "Data Save First Before Checked.";
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
            return Json(new { msg = objValMssg });
        }

        //public ActionResult ConfirmRecordStatus(LCMLimInfo model, string limID)
        //{
        //    try
        //    {
        //        if (limID != "")
        //        {
        //            LCM_LimInfo ob = objRepository.LimInfoRepository.GetByID(Convert.ToInt32(limID));
        //            if (ob.RecordStatus == "NCF")
        //            {
        //                ob.RecordStatus = "CNF";
        //                ob.CheckedBy = Convert.ToInt32(Session["UserID"]);
        //                ob.CheckDate = DateTime.Now;
        //                objRepository.LimInfoRepository.Update(ob);
        //                int flag = objRepository.Save();
        //                if (flag == 1)
        //                {
        //                    objValMssg.Type = Enums.MessageType.Success;
        //                    objValMssg.Msg = "Confirme Successfully.";
        //                }
        //                else
        //                {
        //                    objValMssg.Type = Enums.MessageType.Error;
        //                    objValMssg.Msg = "Confirmed Faild.";
        //                }
        //            }
        //            else
        //            {
        //                objValMssg.Type = Enums.MessageType.Error;
        //                objValMssg.Msg = "Record Status is Empty.";
        //            }
        //        }

        //    }
        //    catch (DbEntityValidationException e)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        foreach (var eve in e.EntityValidationErrors)
        //        {
        //            sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
        //                                            eve.Entry.Entity.GetType().Name,
        //                                            eve.Entry.State));
        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
        //                                            ve.PropertyName,
        //                                            ve.ErrorMessage));
        //            }
        //        }
        //        throw new DbEntityValidationException(sb.ToString(), e);
        //    }
        //    return Json(new { msg = objValMssg });
        //}


        #region Confirm Data & Update Stock
        [HttpPost]
        public ActionResult ConfirmRecordStatus(string limID, string branchID)
        {
            objValMssg = objDalLCM_LimInfo.ConfirmLimInformation(Convert.ToInt32(Session["UserID"]), limID, branchID);
            return Json(new { msg = objValMssg });
        }
        #endregion



        public ActionResult DeleteLimInfoList(int limID)
        {
            objValMssg = objDalLCM_LimInfo.DeleteLimInfoList(limID);
            return Json(objValMssg, JsonRequestBehavior.AllowGet);
        }


    }
}