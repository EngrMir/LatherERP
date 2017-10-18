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
    public class EXPBankLoanController : Controller
    {
        private UnitOfWork objRepository = new UnitOfWork();
        private DalEXPBankLoan objDalEXPBankLoan = new DalEXPBankLoan();
        private ValidationMsg objValMssg = new ValidationMsg();
        private int _userId;

        [CheckUserAccess("EXPBankLoan/EXPBankLoan")]
        public ActionResult EXPBankLoan()
        {
            ViewBag.Currency = new SelectList(objRepository.CurrencyRepository.Get(filter: ob => ob.IsActive == true), "CurrencyID", "CurrencyName");
            ViewBag.formTiltle = "Export Bank Loan";
            return View();
        }



        #region Start EXPBankLoan Save & Update

        [HttpPost]
        public ActionResult SaveAndUpdate(EXPBankLoan model)
        {
            objValMssg = model.expBankLoanList[0].BankLoanID == 0 ? objDalEXPBankLoan.Save(model, Convert.ToInt32(Session["UserID"]), "EXPBankLoan/EXPBankLoan") :
                                                    objDalEXPBankLoan.Update(model, Convert.ToInt32(Session["UserID"]));
            return Json(new { msg = objValMssg });
        }

        #endregion



        // ************************ Bank Pop UP ***********************
        public ActionResult GetBankList()
        {
            var bankList = objDalEXPBankLoan.GetLCBankList();
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
        // ********************* END of Bank Pop UP ****************


        // ************************ CI Pop UP ***********************
        public ActionResult GetCIList()
        {
            var bankList = objDalEXPBankLoan.GetCIList();
            return Json(bankList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchCIbyCINo(string search)//InsuranceNo
        {
            var lcInfo = (from temp in objRepository.ExpCommercialInvoiceRepository.Get().Where(ob => (ob.CIRefNo.StartsWith(search) || ob.CIRefNo == search)).OrderByDescending(o => o.CIRefNo)

                          select new
                          {
                              CIRefNo = temp.CIRefNo,
                          });

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BtnSearchCIByCINo(string search)
        {
            var lcInfo = (from temp in objRepository.ExpCommercialInvoiceRepository.Get().Where(ob => (ob.CIRefNo.StartsWith(search) || ob.CIRefNo == search)).OrderByDescending(o => o.CIRefNo)

                          select new
                          {
                              CIRefNo = temp.CIRefNo,
                          });

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAutoCompleteCIPopupData()
        {
            var data = objRepository.ExpCommercialInvoiceRepository.Get().Select(ob => ob.CIRefNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // ********************* END of CI Pop UP ****************



        // ************************ Loan Head Pop UP ***********************
        public ActionResult GetHeadList()
        {
            var bankList = objDalEXPBankLoan.GetHeadList();
            return Json(bankList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchHeadbyHeadName(string search)//InsuranceNo
        {
            var lcInfo = (from temp in objRepository.SysTransHeadRepository.Get().Where(ob => (ob.HeadName.StartsWith(search) || ob.HeadName == search)).OrderByDescending(o => o.HeadName)

                          select new
                          {
                              HeadCode = temp.HeadCode,
                              HeadName = temp.HeadName,
                          });

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BtnSearchHeadbyHeadName(string search)
        {
            var lcInfo = (from temp in objRepository.SysTransHeadRepository.Get().Where(ob => (ob.HeadName.StartsWith(search) || ob.HeadName == search)).OrderByDescending(o => o.HeadName)

                          select new
                          {
                              HeadCode = temp.HeadCode,
                              HeadName = temp.HeadName,
                          });

            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAutoCompleteHeadPopupData()
        {
            var data = objRepository.SysTransHeadRepository.Get().Select(ob => ob.HeadName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // ********************* END of CI Pop UP ****************





        // ##################### Search Start #############
        public ActionResult GetCnFBillInfo()
        {
            var listCnFBillInfo = (from temp in objRepository.ExpBankLoanRepository.Get().Where(ob => ob.RunningStatus == "RNG").AsEnumerable()
                                   join temp2 in objRepository.BankRepository.Get() on temp.BankID equals temp2.BankID
                                   join temp3 in objRepository.BranchRepository.Get() on temp.BranchID equals temp3.BranchID
                                   join temp4 in objRepository.SysTransHeadRepository.Get() on temp.LoanHead equals temp4.HeadID

                                   select new
                                   {
                                       BankLoanID = temp.BankLoanID == null ? 0 : temp.BankLoanID,
                                       BankLoanNo = temp.BankLoanNo == null ? "" : temp.BankLoanNo,
                                       LoanHead = temp.LoanHead == null ? 0 : temp.LoanHead,
                                       HeadID = temp4.HeadID == null ? 0 : temp4.HeadID,
                                       HeadName = temp4.HeadName == null ? "" : temp4.HeadName,
                                       BankID = temp.BankID == null ? 0 : temp.BankID,
                                       BankCode = temp2.BankCode == null ? "" : temp2.BankCode,
                                       BankName = temp2.BankName == null ? "" : temp2.BankName,
                                       BranchID = temp.BranchID == null ? 0 : temp.BranchID,
                                       BanchCode = temp3.BanchCode == null ? "" : temp3.BanchCode,
                                       BranchName = temp3.BranchName == null ? "" : temp3.BranchName,
                                       LoanAmt = temp.LoanAmt == null ? 0 : temp.LoanAmt,
                                       RunningStatus = temp.RunningStatus == null ? "" : temp.RunningStatus,
                                       ApprovalNote = temp.ApprovalNote == null ? "" : temp.ApprovalNote,
                                       RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),

                                   }).OrderByDescending(ob => ob.BankLoanID);
            return Json(listCnFBillInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLcmCnFBillInfoByLCNo(string search)//InsuranceNo
        {
            var lcInfo = (from temp in objRepository.ExpBankLoanRepository.Get().Where(ob => ob.RunningStatus == "RNG").AsEnumerable()
                                   join temp2 in objRepository.BankRepository.Get() on temp.BankID equals temp2.BankID
                                   join temp3 in objRepository.BranchRepository.Get() on temp.BranchID equals temp3.BranchID
                                   join temp4 in objRepository.SysTransHeadRepository.Get() on temp.LoanHead equals temp4.HeadID

                                   select new
                                   {
                                       BankLoanID = temp.BankLoanID == null ? 0 : temp.BankLoanID,
                                       BankLoanNo = temp.BankLoanNo == null ? "" : temp.BankLoanNo,
                                       LoanHead = temp.LoanHead == null ? 0 : temp.LoanHead,
                                       HeadID = temp4.HeadID == null ? 0 : temp4.HeadID,
                                       HeadName = temp4.HeadName == null ? "" : temp4.HeadName,
                                       BankID = temp.BankID == null ? 0 : temp.BankID,
                                       BankCode = temp2.BankCode == null ? "" : temp2.BankCode,
                                       BankName = temp2.BankName == null ? "" : temp2.BankName,
                                       BranchID = temp.BranchID == null ? 0 : temp.BranchID,
                                       BanchCode = temp3.BanchCode == null ? "" : temp3.BanchCode,
                                       BranchName = temp3.BranchName == null ? "" : temp3.BranchName,
                                       LoanAmt = temp.LoanAmt == null ? 0 : temp.LoanAmt,
                                       RunningStatus = temp.RunningStatus == null ? "" : temp.RunningStatus,
                                       ApprovalNote = temp.ApprovalNote == null ? "" : temp.ApprovalNote,
                                       RecordStatus = DalCommon.ReturnRecordStatus(temp.RecordStatus),

                          }).OrderByDescending(ob => ob.BankLoanID);
            return Json(lcInfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAutoCompleteData()
        {
            var data = objRepository.ExpBankLoanRepository.Get().Select(ob => ob.BankLoanID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBankLoanByBankLoanID()
        {
            var searchBankLoanDetails = objDalEXPBankLoan.SearchBankLoanDetails();
            return Json(searchBankLoanDetails, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetBankLoanOnSelectedBank(long BankID, long BranchID)
        {
            var searchBankLoanDetails = objDalEXPBankLoan.SearchBankLoanDetailsOnSelectedBank(BankID, BranchID);
            return Json(searchBankLoanDetails, JsonRequestBehavior.AllowGet);

        }
        // ##################### Search END ##############




        //##################### Confirm Data #############

        public ActionResult ConfirmRecordStatus(string bankLoanID)
        {
            try
            {
                if (bankLoanID != "")
                {
                    EXP_BankLoan ob = objRepository.ExpBankLoanRepository.GetByID(Convert.ToInt32(bankLoanID));
                    if (ob.RecordStatus == "NCF")
                    {
                        ob.RecordStatus = "CNF";
                        objRepository.ExpBankLoanRepository.Update(ob);
                        int flag = objRepository.Save();
                        if (flag == 1)
                        {
                            objValMssg.Type = Enums.MessageType.Success;
                            objValMssg.Msg = "Confirmed Successfully.";
                        }
                        else
                        {
                            objValMssg.Type = Enums.MessageType.Error;
                            objValMssg.Msg = "Confirmed Faild.";
                        }
                    }
                    else
                    {
                        objValMssg.Type = Enums.MessageType.Error;
                        objValMssg.Msg = "Record Status has not found.";
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
            return Json(new { msg = objValMssg });
        }





        //##################### Loan CLosing #############

        public ActionResult CloseLoanData(long BankLoanID)
        {
            var data = objRepository.ExpBankLoanRepository.GetByID(Convert.ToInt64(BankLoanID));
            if (data != null)
            {
                if (data.RunningStatus == "RNG")
                {
                    data.RunningStatus = "CLS";
                    int flag = objRepository.Save();
                    if (flag > 0)
                    {
                        objValMssg.Type = Enums.MessageType.Success;
                        objValMssg.Msg = "Success";
                    }
                    else
                    {
                        objValMssg.Type = Enums.MessageType.Error;
                        objValMssg.Msg = "Error";
                    }
                }
                else
                {
                    objValMssg.Type = Enums.MessageType.Error;
                    objValMssg.Msg = "CNF";
                }

            }
            return Json(objValMssg, JsonRequestBehavior.AllowGet);
        }



        //###################### Loan Closing ##########










        //###################### DELETE GRID DATA ##########

        public ActionResult DeleteBankLoanDetailData(long BankLoanID)
        {
            var data = objRepository.ExpBankLoanRepository.GetByID(Convert.ToInt64(BankLoanID));
            if (data != null)
            {
                if (data.RecordStatus != "CNF")
                {
                    objRepository.ExpBankLoanRepository.Delete(data);
                    int flag = objRepository.Save();
                    if (flag > 0)
                    {
                        objValMssg.Type = Enums.MessageType.Success;
                        objValMssg.Msg = "Success";
                    }
                    else
                    {
                        objValMssg.Type = Enums.MessageType.Error;
                        objValMssg.Msg = "Error";
                    }
                }
                else
                {
                    objValMssg.Type = Enums.MessageType.Error;
                    objValMssg.Msg = "CNF";
                }

            }
            return Json(objValMssg, JsonRequestBehavior.AllowGet);
        }




        //###################### END OF DELETE GRID DATA ###


        //###################### DELETE DATA ###############


        public ActionResult DeleteBankLoanList(int bankLoanID)
        {
            objValMssg = objDalEXPBankLoan.DeleteBankLoanList(bankLoanID);//DeletePackingList(cnfBillID);
            return Json(objValMssg, JsonRequestBehavior.AllowGet);
        }


    }
}


